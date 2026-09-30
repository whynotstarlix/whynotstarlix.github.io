using System;
using System.Collections.Generic;
using Kitsune_VM.Obfuscation.Helper;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Junk;

internal class Junks
{
	public static void Execute(ModuleDef module)
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected Obj, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected Obj, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		List<MethodDef> list = new List<MethodDef>();
		int num = random.Next(20, 60);
		for (int i = 0; i < num; i++)
		{
			TypeDef val = CreateNewClass();
			bool flag = random.Next(0, 3) == 1;
			MethodDef val2 = (flag ? CreateCctor(module) : CreateCtor(module));
			for (int j = 0; j < random.Next(2, 6); j++)
			{
				if (random.Next(2) == 0)
				{
					val.Fields.Add(CreateField(module, (TypeSig)(object)module.CorLibTypes.Int32, RandomFieldAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					val.Fields.Add(CreateField(module, (TypeSig)(object)module.CorLibTypes.Boolean, RandomFieldAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					val.Fields.Add(CreateField(module, (TypeSig)(object)module.CorLibTypes.String, RandomFieldAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					val.Fields.Add(CreateField(module, (TypeSig)(object)module.CorLibTypes.Int64, RandomFieldAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					val.Methods.Add((MethodDef)(object)GetIntFunc(module, GenerateBody(module, "Int32", random), RandomMethodAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					val.Methods.Add((MethodDef)(object)GetStrFunc(module, GenerateBody(module, "String", random), RandomMethodAttrs(random, flag)));
				}
				if (random.Next(2) == 0)
				{
					MethodDefUser item = VoidMethod(module, GenerateBody(module, "Void", random), RandomMethodAttrs(random, flag));
					val.Methods.Add((MethodDef)(object)item);
					if (flag)
					{
						list.Add((MethodDef)(object)item);
					}
				}
			}
			val.Methods.Add(val2);
			val2.Body = new CilBody();
			if (!flag)
			{
				MemberRefUser val3 = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (IMemberRefParent)(object)((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef);
				val2.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
				val2.Body.Instructions.Add(OpCodes.Call.ToInstruction((MemberRef)(object)val3));
			}
			foreach (FieldDef field in val.Fields)
			{
				if ((!flag || field.IsStatic) && (flag || !field.IsStatic))
				{
					switch (field.FieldType.TypeName)
					{
					case "Int64":
					{
						long num2 = ((long)random.Next(int.MinValue, int.MaxValue) << 32) | (uint)random.Next();
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I8, num2));
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, (IField)(object)field));
						break;
					}
					case "Boolean":
						val2.Body.Instructions.Add(Instruction.Create((random.Next(2) == 1) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0));
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, (IField)(object)field));
						break;
					case "String":
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, Methods.GenerateString()));
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, (IField)(object)field));
						break;
					case "Int32":
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next()));
						val2.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, (IField)(object)field));
						break;
					}
				}
			}
			val2.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
			module.Types.Add(val);
		}
		if (list.Count < 2)
		{
			return;
		}
		int num3 = random.Next(1, Math.Max(2, list.Count / 2));
		for (int k = 0; k < num3; k++)
		{
			MethodDef val4 = list[random.Next(list.Count)];
			MethodDef val5 = list[random.Next(list.Count)];
			if (val4 != val5)
			{
				IList<Instruction> instructions = val4.Body.Instructions;
				int num4 = instructions.Count - 1;
				if (num4 >= 0 && instructions[num4].OpCode == OpCodes.Ret)
				{
					instructions.Insert(num4, Instruction.Create(OpCodes.Call, (IMethod)(object)val5));
				}
			}
		}
	}

	private static CilBody GenerateBody(ModuleDef module, string returnType, Random rnd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected Obj, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected Obj, but got Unknown
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Expected Obj, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected Obj, but got Unknown
		CilBody val = new CilBody();
		switch (returnType)
		{
		case "String":
		{
			Local val5 = new Local((TypeSig)(object)module.CorLibTypes.String, Methods.GenerateString());
			val.Variables.Add(val5);
			val.Instructions.Add(Instruction.Create(OpCodes.Ldstr, Methods.GenerateString()));
			val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val5));
			if (rnd.Next(2) == 0)
			{
				val.Instructions.Add(Instruction.Create(OpCodes.Ldstr, Methods.GenerateString()));
				val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val5));
			}
			val.Instructions.Add(Instruction.Create(OpCodes.Ldloc, val5));
			val.Instructions.Add(Instruction.Create(OpCodes.Ret));
			break;
		}
		case "Int32":
		{
			Local val4 = new Local((TypeSig)(object)module.CorLibTypes.Int32, Methods.GenerateString());
			val.Variables.Add(val4);
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val4));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldloc, val4));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			val.Instructions.Add(Instruction.Create(OpCodes.Add));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			val.Instructions.Add(Instruction.Create(OpCodes.Xor));
			val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val4));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldloc, val4));
			val.Instructions.Add(Instruction.Create(OpCodes.Ret));
			break;
		}
		case "Int64":
		{
			Local val3 = new Local((TypeSig)(object)module.CorLibTypes.Int64, Methods.GenerateString());
			val.Variables.Add(val3);
			long num = ((long)rnd.Next(int.MinValue, int.MaxValue) << 32) | (uint)rnd.Next();
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I8, num));
			val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val3));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldloc, val3));
			val.Instructions.Add(Instruction.Create(OpCodes.Ret));
			break;
		}
		default:
		{
			Local val2 = new Local((TypeSig)(object)module.CorLibTypes.Int32, Methods.GenerateString());
			val.Variables.Add(val2);
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			val.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			val.Instructions.Add(Instruction.Create(OpCodes.Add));
			val.Instructions.Add(Instruction.Create(OpCodes.Stloc, val2));
			val.Instructions.Add(Instruction.Create(OpCodes.Ret));
			break;
		}
		}
		return val;
	}

	private static TypeDef CreateNewClass()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		return (TypeDef)new TypeDefUser(UTF8String.op_Implicit(Methods.GenerateString()))
		{
			Attributes = (TypeAttributes)1048704,
			Namespace = UTF8String.op_Implicit(Methods.GenerateString())
		};
	}

	private static MethodDef CreateCctor(ModuleDef module)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		return (MethodDef)new MethodDefUser(UTF8String.op_Implicit(".cctor"), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)0, (MethodAttributes)6161);
	}

	private static MethodDef CreateCtor(ModuleDef module)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		return (MethodDef)new MethodDefUser(UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)0, (MethodAttributes)6150);
	}

	private static FieldDef CreateField(ModuleDef module, TypeSig typeSig, FieldAttributes attrs)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		return (FieldDef)new FieldDefUser(UTF8String.op_Implicit(Methods.GenerateString()), new FieldSig(typeSig), attrs);
	}

	private static MethodDefUser VoidMethod(ModuleDef module, CilBody body, MethodAttributes attrs)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		return new MethodDefUser(UTF8String.op_Implicit(Methods.GenerateString()), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)0, attrs)
		{
			Body = body
		};
	}

	private static MethodDefUser GetStrFunc(ModuleDef module, CilBody body, MethodAttributes attrs)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		return new MethodDefUser(UTF8String.op_Implicit(Methods.GenerateString()), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.String), (MethodImplAttributes)0, attrs)
		{
			Body = body
		};
	}

	private static MethodDefUser GetIntFunc(ModuleDef module, CilBody body, MethodAttributes attrs)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		return new MethodDefUser(UTF8String.op_Implicit(Methods.GenerateString()), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.Int32), (MethodImplAttributes)0, attrs)
		{
			Body = body
		};
	}

	private static FieldAttributes RandomFieldAttrs(Random rnd, bool isStatic)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		FieldAttributes val = rnd.Next(3) switch
		{
			0 => (FieldAttributes)6, 
			2 => (FieldAttributes)3, 
			_ => (FieldAttributes)1, 
		};
		if (isStatic)
		{
			val = (FieldAttributes)(val | 0x10);
		}
		return val;
	}

	private static MethodAttributes RandomMethodAttrs(Random rnd, bool isStatic)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		MethodAttributes val = rnd.Next(3) switch
		{
			0 => (MethodAttributes)6, 
			2 => (MethodAttributes)3, 
			_ => (MethodAttributes)1, 
		};
		if (isStatic)
		{
			val = (MethodAttributes)(val | 0x10);
		}
		return (MethodAttributes)(val | 0x80);
	}
}
