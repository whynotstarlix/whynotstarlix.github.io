using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Proxy;

internal class ProxyArithmetic
{
	private static readonly Random _rnd = new Random();

	private static readonly Dictionary<Code, string> _supported = new Dictionary<Code, string>
	{
		{
			(Code)88,
			"Add"
		},
		{
			(Code)89,
			"Sub"
		},
		{
			(Code)90,
			"Mul"
		},
		{
			(Code)97,
			"Xor"
		},
		{
			(Code)95,
			"And"
		},
		{
			(Code)96,
			"Or"
		}
	};

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		TypeDef val = CreateContainer(module);
		module.Types.Add(val);
		Dictionary<Code, MethodDef> cache = new Dictionary<Code, MethodDef>();
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

	private static void ProcessMethod(ModuleDef module, MethodDef method, TypeDef container, Dictionary<Code, MethodDef> cache)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		IList<Instruction> instructions = method.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			Code code = instructions[i].OpCode.Code;
			if (_supported.ContainsKey(code) && _rnd.Next(5) < 2)
			{
				if (!cache.TryGetValue(code, out var value))
				{
					value = BuildWrapperMethod(module, code, _supported[code]);
					container.Methods.Add(value);
					cache[code] = value;
				}
				instructions[i].OpCode = OpCodes.Call;
				instructions[i].Operand = value;
			}
		}
	}

	private static MethodDef BuildWrapperMethod(ModuleDef module, Code op, string tag)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected Obj, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected Obj, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected I4, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected Obj, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected Obj, but got Unknown
		CorLibTypeSig @int = module.CorLibTypes.Int32;
		string text = "W_" + tag + "_" + _rnd.Next(65535).ToString("X4");
		MethodSig val = MethodSig.CreateStatic((TypeSig)(object)@int, (TypeSig)(object)@int, (TypeSig)(object)@int);
		MethodDefUser val2 = new MethodDefUser(UTF8String.op_Implicit(text), val, (MethodAttributes)145);
		CilBody val3 = (((MethodDef)val2).Body = new CilBody());
		val3.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
		val3.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
		if (_rnd.Next(2) == 0)
		{
			int num = _rnd.Next(1, int.MaxValue);
			val3.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num));
			val3.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num));
			val3.Instructions.Add(Instruction.Create(OpCodes.Xor));
			val3.Instructions.Add(Instruction.Create(OpCodes.Pop));
		}
		switch (op - 88)
		{
		case 0:
			val3.Instructions.Add(Instruction.Create(OpCodes.Add));
			break;
		case 1:
			val3.Instructions.Add(Instruction.Create(OpCodes.Sub));
			break;
		case 2:
			val3.Instructions.Add(Instruction.Create(OpCodes.Mul));
			break;
		case 9:
			val3.Instructions.Add(Instruction.Create(OpCodes.Xor));
			break;
		case 7:
			val3.Instructions.Add(Instruction.Create(OpCodes.And));
			break;
		case 8:
			val3.Instructions.Add(Instruction.Create(OpCodes.Or));
			break;
		}
		val3.Instructions.Add(Instruction.Create(OpCodes.Ret));
		val3.MaxStack = 4;
		ParamDefUser item = new ParamDefUser(UTF8String.op_Implicit("a"), (ushort)1);
		((MethodDef)val2).ParamDefs.Add((ParamDef)(object)item);
		ParamDefUser item2 = new ParamDefUser(UTF8String.op_Implicit("b"), (ushort)2);
		((MethodDef)val2).ParamDefs.Add((ParamDef)(object)item2);
		return (MethodDef)(object)val2;
	}

	private static TypeDef CreateContainer(ModuleDef module)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		string text = "A" + _rnd.Next(65535).ToString("X4");
		return (TypeDef)new TypeDefUser(UTF8String.op_Implicit(""), UTF8String.op_Implicit(text), ((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef)
		{
			Attributes = (TypeAttributes)384
		};
	}
}
