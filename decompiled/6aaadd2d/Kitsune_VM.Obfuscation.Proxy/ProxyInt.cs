using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Proxy;

internal class ProxyInt
{
	private static readonly Random _rnd = new Random();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		TypeDef val = CreateContainer(module);
		module.Types.Add(val);
		Dictionary<int, MethodDef> cache = new Dictionary<int, MethodDef>();
		foreach (TypeDef item in module.GetTypes().ToList())
		{
			if ((excluded != null && excluded.Contains(item)) || item.IsGlobalModuleType || item == val)
			{
				continue;
			}
			foreach (MethodDef item2 in item.Methods.ToList())
			{
				if (item2.HasBody && !item2.IsConstructor && !item2.IsStaticConstructor && (excludedMethods == null || !excludedMethods.Contains(item2)))
				{
					try
					{
						ProcessMethod(module, item2, val, cache);
					}
					catch
					{
					}
				}
			}
		}
	}

	private static void ProcessMethod(ModuleDef module, MethodDef method, TypeDef container, Dictionary<int, MethodDef> cache)
	{
		IList<Instruction> instructions = method.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (!IsLdc(instructions[i]))
			{
				continue;
			}
			int val = GetVal(instructions[i]);
			if ((val < -1 || val > 8) && _rnd.Next(2) == 0)
			{
				if (!cache.TryGetValue(val, out var value))
				{
					value = BuildProxyMethod(module, container, val);
					container.Methods.Add(value);
					cache[val] = value;
				}
				instructions[i].OpCode = OpCodes.Call;
				instructions[i].Operand = value;
			}
		}
	}

	private static MethodDef BuildProxyMethod(ModuleDef module, TypeDef container, int val)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected Obj, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		new Importer(module);
		CorLibTypeSig @int = module.CorLibTypes.Int32;
		uint num = (uint)val;
		string text = "P_" + num + "_" + _rnd.Next(65535).ToString("X4");
		MethodSig val2 = MethodSig.CreateStatic((TypeSig)(object)@int);
		MethodDefUser val3 = new MethodDefUser(UTF8String.op_Implicit(text), val2, (MethodAttributes)145);
		CilBody val4 = (((MethodDef)val3).Body = new CilBody());
		switch (_rnd.Next(5))
		{
		case 0:
		{
			int num5 = _rnd.Next(4096, int.MaxValue);
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, val ^ num5));
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num5));
			val4.Instructions.Add(Instruction.Create(OpCodes.Xor));
			break;
		}
		case 1:
		{
			int num4 = _rnd.Next(256, 32767);
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, val + num4));
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num4));
			val4.Instructions.Add(Instruction.Create(OpCodes.Sub));
			break;
		}
		case 2:
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, ~val));
			val4.Instructions.Add(Instruction.Create(OpCodes.Not));
			break;
		case 3:
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, ~val));
			val4.Instructions.Add(Instruction.Create(OpCodes.Neg));
			val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, 1));
			val4.Instructions.Add(Instruction.Create(OpCodes.Sub));
			break;
		default:
			if (val >= 0 && val <= 1073741823)
			{
				int num2 = _rnd.Next(1, 3);
				val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, val << num2));
				val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num2));
				val4.Instructions.Add(Instruction.Create(OpCodes.Shr));
			}
			else
			{
				int num3 = _rnd.Next(4096, int.MaxValue);
				val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, val ^ num3));
				val4.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num3));
				val4.Instructions.Add(Instruction.Create(OpCodes.Xor));
			}
			break;
		}
		val4.Instructions.Add(Instruction.Create(OpCodes.Ret));
		val4.MaxStack = 2;
		return (MethodDef)(object)val3;
	}

	private static TypeDef CreateContainer(ModuleDef module)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		string text = "P" + _rnd.Next(65535).ToString("X4");
		return (TypeDef)new TypeDefUser(UTF8String.op_Implicit(""), UTF8String.op_Implicit(text), ((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef)
		{
			Attributes = (TypeAttributes)384
		};
	}

	private static bool IsLdc(Instruction i)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Code code = i.OpCode.Code;
		return ((object)code/*cast due to constrained. prefix*/).ToString().StartsWith("Ldc_I4");
	}

	private static int GetVal(Instruction i)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		Code code = i.OpCode.Code;
		return (code - 21) switch
		{
			0 => -1, 
			1 => 0, 
			2 => 1, 
			3 => 2, 
			4 => 3, 
			5 => 4, 
			6 => 5, 
			7 => 6, 
			8 => 7, 
			9 => 8, 
			10 => (sbyte)i.Operand, 
			_ => (int)i.Operand, 
		};
	}
}
