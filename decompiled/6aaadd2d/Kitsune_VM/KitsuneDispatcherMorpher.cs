using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM;

public static class KitsuneDispatcherMorpher
{
	private static readonly Random _rnd = new Random();

	public static bool Morph(ModuleDef module)
	{
		try
		{
			TypeDef val = module.Find("Kitsune_VM_Stub.KitsuneDispatcher", false);
			if (val == null)
			{
				Console.WriteLine("  [Morph] KitsuneDispatcher type not found — skipped.");
				return false;
			}
			MethodDef val2 = null;
			foreach (MethodDef method in val.Methods)
			{
				if (method.Name == "Execute" && method.HasBody && method.Parameters.Count == 2)
				{
					val2 = method;
					break;
				}
			}
			if (val2 == null)
			{
				Console.WriteLine("  [Morph] Execute method not found — skipped.");
				return false;
			}
			TypeDef val3 = module.Find("Kitsune_VM_Stub.KitsuneOpcode", false);
			if (val3 == null)
			{
				Console.WriteLine("  [Morph] KitsuneOpcode enum not found — skipped.");
				return false;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (FieldDef field in val3.Fields)
			{
				if (field.IsLiteral && field.Constant != null)
				{
					object value = field.Constant.Value;
					int value2 = ((value is byte b) ? b : Convert.ToInt32(value));
					dictionary[UTF8String.op_Implicit(field.Name)] = value2;
				}
			}
			List<KeyValuePair<MethodDef, int>> list = new List<KeyValuePair<MethodDef, int>>();
			foreach (MethodDef method2 in val.Methods)
			{
				if (method2.Name.StartsWith("H_"))
				{
					string key = UTF8String.op_Implicit(method2.Name.Substring(2));
					if (dictionary.TryGetValue(key, out var value3))
					{
						list.Add(new KeyValuePair<MethodDef, int>(method2, value3));
					}
				}
			}
			if (list.Count == 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (MethodDef method3 in val.Methods)
				{
					stringBuilder.Append(UTF8String.op_Implicit(method3.Name)).Append(", ");
				}
				Console.WriteLine("  [Morph] No H_* handlers matched opcode enum. Dispatcher methods: " + stringBuilder);
				Console.WriteLine("  [Morph] Opcode enum values found: " + dictionary.Count);
				return false;
			}
			int num = _rnd.Next(4);
			val2.Body = num switch
			{
				0 => BuildIfElseBody(list, module), 
				1 => BuildGroupedIfElseBody(list, module), 
				2 => BuildBinaryChainBody(list, module), 
				_ => BuildJumpTableBody(list, module, val) ?? BuildIfElseBody(list, module), 
			};
			ShuffleHandlerMethodOrder(val);
			Console.WriteLine($"  [Morph] Execute regenerated: style={num}, {list.Count} handlers in random order");
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("  [Morph] Failed: " + ex.Message);
			return false;
		}
	}

	private static CilBody BuildIfElseBody(List<KeyValuePair<MethodDef, int>> handlers, ModuleDef module)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected Obj, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected Obj, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected Obj, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected Obj, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected Obj, but got Unknown
		Shuffle(handlers);
		CilBody val = new CilBody();
		val.KeepOldMaxStack = false;
		val.MaxStack = 2;
		List<Instruction> list = new List<Instruction>();
		List<Instruction[]> list2 = new List<Instruction[]>();
		foreach (KeyValuePair<MethodDef, int> handler in handlers)
		{
			Instruction val2 = new Instruction(OpCodes.Ldarg_0);
			list.Add(val2);
			list2.Add(new Instruction[3]
			{
				val2,
				new Instruction(OpCodes.Call, (object)handler.Key),
				new Instruction(OpCodes.Ret)
			});
		}
		Instruction val3 = BuildThrowStartPlaceholder();
		for (int i = 0; i < handlers.Count; i++)
		{
			val.Instructions.Add(new Instruction(OpCodes.Ldarg_1));
			val.Instructions.Add(CreateLdcI4(handlers[i].Value));
			val.Instructions.Add(new Instruction(OpCodes.Beq, (object)list[i]));
			if (_rnd.Next(2) == 0)
			{
				AppendJunkInstruction(val);
			}
		}
		val.Instructions.Add(new Instruction(OpCodes.Br, (object)val3));
		foreach (Instruction[] item2 in list2)
		{
			foreach (Instruction item in item2)
			{
				val.Instructions.Add(item);
			}
		}
		BuildThrowBlockAt(val, module, val3);
		return val;
	}

	private static CilBody BuildGroupedIfElseBody(List<KeyValuePair<MethodDef, int>> handlers, ModuleDef module)
	{
		Dictionary<int, List<KeyValuePair<MethodDef, int>>> dictionary = new Dictionary<int, List<KeyValuePair<MethodDef, int>>>();
		foreach (KeyValuePair<MethodDef, int> handler in handlers)
		{
			int key = handler.Value & 0xF0;
			if (!dictionary.TryGetValue(key, out var value))
			{
				value = (dictionary[key] = new List<KeyValuePair<MethodDef, int>>());
			}
			value.Add(handler);
		}
		List<int> list2 = new List<int>(dictionary.Keys);
		Shuffle(list2);
		foreach (List<KeyValuePair<MethodDef, int>> value2 in dictionary.Values)
		{
			Shuffle(value2);
		}
		List<KeyValuePair<MethodDef, int>> list3 = new List<KeyValuePair<MethodDef, int>>();
		foreach (int item in list2)
		{
			list3.AddRange(dictionary[item]);
		}
		return BuildIfElseBody(list3, module);
	}

	private static CilBody BuildBinaryChainBody(List<KeyValuePair<MethodDef, int>> handlers, ModuleDef module)
	{
		handlers.Sort((KeyValuePair<MethodDef, int> a, KeyValuePair<MethodDef, int> b) => a.Value.CompareTo(b.Value));
		int num = _rnd.Next(1, handlers.Count);
		List<KeyValuePair<MethodDef, int>> range = handlers.GetRange(num, handlers.Count - num);
		List<KeyValuePair<MethodDef, int>> range2 = handlers.GetRange(0, num);
		Shuffle(range2);
		Shuffle(range);
		List<KeyValuePair<MethodDef, int>> list = new List<KeyValuePair<MethodDef, int>>(range);
		list.AddRange(range2);
		return BuildIfElseBody(list, module);
	}

	private static CilBody BuildJumpTableBody(List<KeyValuePair<MethodDef, int>> handlers, ModuleDef module, TypeDef dispatcherType)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected Obj, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected Obj, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected Obj, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected Obj, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected Obj, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected Obj, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected Obj, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected Obj, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected Obj, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected Obj, but got Unknown
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected Obj, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected Obj, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected Obj, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected Obj, but got Unknown
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected Obj, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected Obj, but got Unknown
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Expected Obj, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected Obj, but got Unknown
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Expected Obj, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Expected Obj, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected Obj, but got Unknown
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Expected Obj, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Expected Obj, but got Unknown
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Expected Obj, but got Unknown
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Expected Obj, but got Unknown
		FieldDef val = null;
		foreach (FieldDef field in dispatcherType.Fields)
		{
			if (UTF8String.op_Implicit(field.Name) == "_jt")
			{
				val = field;
				break;
			}
		}
		if (val == null)
		{
			return null;
		}
		ITypeDefOrRef typeRef = (ITypeDefOrRef)(object)module.CorLibTypes.GetTypeRef("System", "Action");
		MemberRefUser val2 = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void, (TypeSig)(object)module.CorLibTypes.Object, (TypeSig)(object)module.CorLibTypes.IntPtr), (IMemberRefParent)(object)typeRef);
		MemberRefUser val3 = new MemberRefUser(module, UTF8String.op_Implicit("Invoke"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (IMemberRefParent)(object)typeRef);
		Shuffle(handlers);
		CilBody val4 = new CilBody();
		val4.KeepOldMaxStack = false;
		val4.MaxStack = 5;
		Instruction item = new Instruction(OpCodes.Ldarg_0);
		val4.Instructions.Add(new Instruction(OpCodes.Ldarg_0));
		val4.Instructions.Add(new Instruction(OpCodes.Ldfld, (object)val));
		Instruction val5 = new Instruction(OpCodes.Ldarg_0);
		val4.Instructions.Add(new Instruction(OpCodes.Brtrue, (object)val5));
		val4.Instructions.Add(item);
		val4.Instructions.Add(new Instruction(OpCodes.Ldc_I4, (object)256));
		val4.Instructions.Add(new Instruction(OpCodes.Newarr, (object)typeRef));
		val4.Instructions.Add(new Instruction(OpCodes.Stfld, (object)val));
		foreach (KeyValuePair<MethodDef, int> handler in handlers)
		{
			val4.Instructions.Add(new Instruction(OpCodes.Ldarg_0));
			val4.Instructions.Add(new Instruction(OpCodes.Ldfld, (object)val));
			val4.Instructions.Add(CreateLdcI4(handler.Value));
			val4.Instructions.Add(new Instruction(OpCodes.Ldarg_0));
			val4.Instructions.Add(new Instruction(OpCodes.Ldftn, (object)handler.Key));
			val4.Instructions.Add(new Instruction(OpCodes.Newobj, (object)val2));
			val4.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
		}
		val4.Instructions.Add(val5);
		val4.Instructions.Add(new Instruction(OpCodes.Ldfld, (object)val));
		val4.Instructions.Add(new Instruction(OpCodes.Ldarg_1));
		val4.Instructions.Add(new Instruction(OpCodes.Ldelem_Ref));
		Instruction val6 = new Instruction(OpCodes.Callvirt, (object)val3);
		val4.Instructions.Add(new Instruction(OpCodes.Dup));
		val4.Instructions.Add(new Instruction(OpCodes.Brtrue, (object)val6));
		Instruction val7 = BuildThrowStartPlaceholder();
		val4.Instructions.Add(new Instruction(OpCodes.Pop));
		val4.Instructions.Add(new Instruction(OpCodes.Br, (object)val7));
		val4.Instructions.Add(val6);
		val4.Instructions.Add(new Instruction(OpCodes.Ret));
		BuildThrowBlockAt(val4, module, val7);
		return val4;
	}

	private static Instruction BuildThrowStartPlaceholder()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		return new Instruction(OpCodes.Ldstr, (object)"KitsuneVM: invalid opcode: ");
	}

	private static void BuildThrowBlockAt(CilBody body, ModuleDef module, Instruction throwStart)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected Obj, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected Obj, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected Obj, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected Obj, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected Obj, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected Obj, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected Obj, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected Obj, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected Obj, but got Unknown
		ITypeDefOrRef typeRef = (ITypeDefOrRef)(object)module.CorLibTypes.GetTypeRef("System", "Exception");
		MemberRefUser val = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void, (TypeSig)(object)module.CorLibTypes.String), (IMemberRefParent)(object)typeRef);
		ITypeDefOrRef typeRef2 = (ITypeDefOrRef)(object)module.CorLibTypes.GetTypeRef("System", "String");
		MemberRefUser val2 = new MemberRefUser(module, UTF8String.op_Implicit("Concat"), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.String, (TypeSig)(object)module.CorLibTypes.String, (TypeSig)(object)module.CorLibTypes.String), (IMemberRefParent)(object)typeRef2);
		ITypeDefOrRef typeRef3 = (ITypeDefOrRef)(object)module.CorLibTypes.GetTypeRef("System", "Object");
		MemberRefUser val3 = new MemberRefUser(module, UTF8String.op_Implicit("ToString"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.String), (IMemberRefParent)(object)typeRef3);
		ITypeDefOrRef val4 = null;
		foreach (TypeDef type in module.Types)
		{
			if (type.Name == "KitsuneOpcode")
			{
				val4 = (ITypeDefOrRef)(object)type;
				break;
			}
			foreach (TypeDef nestedType in type.NestedTypes)
			{
				if (nestedType.Name == "KitsuneOpcode")
				{
					val4 = (ITypeDefOrRef)(object)nestedType;
					break;
				}
			}
		}
		body.Instructions.Add(throwStart);
		if (val4 != null)
		{
			body.Instructions.Add(new Instruction(OpCodes.Ldarg_1));
			body.Instructions.Add(new Instruction(OpCodes.Box, (object)val4));
			body.Instructions.Add(new Instruction(OpCodes.Callvirt, (object)val3));
		}
		else
		{
			body.Instructions.Add(new Instruction(OpCodes.Ldstr, (object)"?"));
		}
		body.Instructions.Add(new Instruction(OpCodes.Call, (object)val2));
		body.Instructions.Add(new Instruction(OpCodes.Newobj, (object)val));
		body.Instructions.Add(new Instruction(OpCodes.Throw));
	}

	private static void AppendJunkInstruction(CilBody body)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected Obj, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected Obj, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected Obj, but got Unknown
		switch (_rnd.Next(4))
		{
		case 0:
			body.Instructions.Add(new Instruction(OpCodes.Nop));
			break;
		case 1:
			body.Instructions.Add(new Instruction(OpCodes.Nop));
			break;
		case 2:
			body.Instructions.Add(new Instruction(OpCodes.Nop));
			break;
		case 3:
			body.Instructions.Add(new Instruction(OpCodes.Nop));
			break;
		}
	}

	private static Instruction CreateLdcI4(int value)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected Obj, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected Obj, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected Obj, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected Obj, but got Unknown
		switch (value)
		{
		case 0:
			return new Instruction(OpCodes.Ldc_I4_0);
		case 1:
			return new Instruction(OpCodes.Ldc_I4_1);
		case 2:
			return new Instruction(OpCodes.Ldc_I4_2);
		case 3:
			return new Instruction(OpCodes.Ldc_I4_3);
		case 4:
			return new Instruction(OpCodes.Ldc_I4_4);
		case 5:
			return new Instruction(OpCodes.Ldc_I4_5);
		case 6:
			return new Instruction(OpCodes.Ldc_I4_6);
		case 7:
			return new Instruction(OpCodes.Ldc_I4_7);
		case 8:
			return new Instruction(OpCodes.Ldc_I4_8);
		default:
			if (value >= -128 && value <= 127)
			{
				return new Instruction(OpCodes.Ldc_I4_S, (object)(sbyte)value);
			}
			return new Instruction(OpCodes.Ldc_I4, (object)value);
		}
	}

	private static void Shuffle<T>(List<T> list)
	{
		for (int num = list.Count - 1; num > 0; num--)
		{
			int index = _rnd.Next(num + 1);
			T value = list[num];
			list[num] = list[index];
			list[index] = value;
		}
	}

	private static void ShuffleHandlerMethodOrder(TypeDef type)
	{
		List<MethodDef> list = new List<MethodDef>();
		List<MethodDef> list2 = new List<MethodDef>();
		foreach (MethodDef method in type.Methods)
		{
			((method.IsConstructor || method.IsStaticConstructor) ? list : list2).Add(method);
		}
		Shuffle(list2);
		type.Methods.Clear();
		foreach (MethodDef item in list)
		{
			type.Methods.Add(item);
		}
		foreach (MethodDef item2 in list2)
		{
			type.Methods.Add(item2);
		}
	}
}
