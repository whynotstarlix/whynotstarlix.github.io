using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.InvalidCode;

internal static class InvalidMethods
{
	private static readonly Random _rnd = new Random();

	private static readonly string[] _verbPrefixes = new string[11]
	{
		"Get", "Set", "Init", "Load", "Save", "Compute", "Validate", "Process", "Update", "Check",
		"Build"
	};

	private static readonly string[] _nounSuffixes = new string[10] { "State", "Buffer", "Context", "Handle", "Token", "Cache", "Entry", "Record", "Segment", "Block" };

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<TypeDef> excludedForStub = null)
	{
		foreach (TypeDef type in module.GetTypes())
		{
			if (!type.IsGlobalModuleType && (excluded == null || !excluded.Contains(type)) && (excludedForStub == null || excludedForStub.Contains(type)) && !type.IsEnum && !type.IsInterface)
			{
				int num = 1 + _rnd.Next(3);
				for (int i = 0; i < num; i++)
				{
					InjectFakeMethod(type, module);
				}
			}
		}
	}

	private static void InjectFakeMethod(TypeDef type, ModuleDef module)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected Obj, but got Unknown
		string text = _verbPrefixes[_rnd.Next(_verbPrefixes.Length)] + _nounSuffixes[_rnd.Next(_nounSuffixes.Length)] + _rnd.Next(100);
		TypeSig val = _rnd.Next(3) switch
		{
			0 => (TypeSig)(object)module.CorLibTypes.Void, 
			1 => (TypeSig)(object)module.CorLibTypes.Int32, 
			_ => (TypeSig)(object)module.CorLibTypes.Boolean, 
		};
		MethodDefUser val2 = new MethodDefUser(UTF8String.op_Implicit(text), MethodSig.CreateStatic(val), (MethodImplAttributes)0, (MethodAttributes)145);
		((MethodDef)val2).Body = BuildJunkBody(module, val);
		type.Methods.Add((MethodDef)(object)val2);
	}

	private static CilBody BuildJunkBody(ModuleDef module, TypeSig retType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected Obj, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected Obj, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected Obj, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected Obj, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected Obj, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected Obj, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected Obj, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected Obj, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected Obj, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected Obj, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected Obj, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected Obj, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected Obj, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected Obj, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected Obj, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected Obj, but got Unknown
		CilBody val = new CilBody();
		val.InitLocals = true;
		Local val2 = new Local((TypeSig)(object)module.CorLibTypes.Int32);
		Local val3 = new Local((TypeSig)(object)module.CorLibTypes.Boolean);
		val.Variables.Add(val2);
		val.Variables.Add(val3);
		IList<Instruction> instructions = val.Instructions;
		switch (_rnd.Next(3))
		{
		case 0:
		{
			instructions.Add(Instruction.CreateLdcI4(_rnd.Next(4, 32)));
			instructions.Add(new Instruction(OpCodes.Stloc, (object)val2));
			Instruction val5 = new Instruction(OpCodes.Ldloc, (object)val2);
			instructions.Add(val5);
			instructions.Add(Instruction.CreateLdcI4(1 + _rnd.Next(7)));
			instructions.Add(new Instruction(OpCodes.Add));
			instructions.Add(new Instruction(OpCodes.Stloc, (object)val2));
			instructions.Add(new Instruction(OpCodes.Ldloc, (object)val2));
			instructions.Add(Instruction.CreateLdcI4(_rnd.Next(100, 1000)));
			instructions.Add(new Instruction(OpCodes.Blt, (object)val5));
			AppendReturnDefault(instructions, retType);
			break;
		}
		case 1:
		{
			instructions.Add(Instruction.CreateLdcI4(-1640531527));
			instructions.Add(new Instruction(OpCodes.Stloc, (object)val2));
			for (int i = 0; i < 3 + _rnd.Next(5); i++)
			{
				instructions.Add(new Instruction(OpCodes.Ldloc, (object)val2));
				instructions.Add(Instruction.CreateLdcI4(_rnd.Next()));
				instructions.Add(new Instruction(OpCodes.Xor));
				instructions.Add(Instruction.CreateLdcI4(1 + _rnd.Next(31)));
				instructions.Add(new Instruction(OpCodes.Shl));
				instructions.Add(new Instruction(OpCodes.Stloc, (object)val2));
			}
			AppendReturnDefault(instructions, retType);
			break;
		}
		default:
		{
			instructions.Add(Instruction.CreateLdcI4(1));
			instructions.Add(new Instruction(OpCodes.Stloc, (object)val3));
			instructions.Add(Instruction.CreateLdcI4(_rnd.Next()));
			instructions.Add(Instruction.CreateLdcI4(_rnd.Next()));
			instructions.Add(new Instruction(OpCodes.And));
			instructions.Add(Instruction.CreateLdcI4(0));
			Instruction val4 = new Instruction(OpCodes.Ldc_I4_0);
			instructions.Add(new Instruction(OpCodes.Bne_Un, (object)val4));
			instructions.Add(Instruction.CreateLdcI4(0));
			instructions.Add(new Instruction(OpCodes.Stloc, (object)val3));
			instructions.Add(val4);
			instructions.Add(new Instruction(OpCodes.Pop));
			AppendReturnDefault(instructions, retType);
			break;
		}
		}
		val.MaxStack = 8;
		val.KeepOldMaxStack = true;
		try
		{
			val.OptimizeBranches();
		}
		catch
		{
		}
		return val;
	}

	private static Instruction AppendReturnDefault(IList<Instruction> instrs, TypeSig retType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected Obj, but got Unknown
		Instruction val;
		if ((int)retType.ElementType == 1)
		{
			val = new Instruction(OpCodes.Ret);
			instrs.Add(val);
		}
		else
		{
			val = Instruction.CreateLdcI4(0);
			instrs.Add(val);
			instrs.Add(new Instruction(OpCodes.Ret));
		}
		return val;
	}
}
