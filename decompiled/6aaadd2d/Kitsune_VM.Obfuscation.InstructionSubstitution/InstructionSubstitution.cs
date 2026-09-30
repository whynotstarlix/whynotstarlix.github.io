using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.InstructionSubstitution;

internal static class InstructionSubstitution
{
	private static readonly Random _rnd = new Random();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		foreach (TypeDef type in module.GetTypes())
		{
			if (excluded != null && excluded.Contains(type))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.HasBody && !method.IsConstructor && !method.IsStaticConstructor && (excludedMethods == null || !excludedMethods.Contains(method)))
				{
					ProcessMethod(method);
				}
			}
		}
	}

	private static void ProcessMethod(MethodDef method)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected I4, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected Obj, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected Obj, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected Obj, but got Unknown
		try
		{
			method.Body.SimplifyBranches();
		}
		catch
		{
			return;
		}
		IList<Instruction> instructions = method.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			Code code = instructions[i].OpCode.Code;
			switch (code - 88)
			{
			case 0:
				instructions[i].OpCode = OpCodes.Neg;
				instructions[i].Operand = null;
				instructions.Insert(i + 1, new Instruction(OpCodes.Sub));
				i++;
				continue;
			case 1:
				instructions[i].OpCode = OpCodes.Neg;
				instructions[i].Operand = null;
				instructions.Insert(i + 1, new Instruction(OpCodes.Add));
				i++;
				continue;
			case 2:
				if (i > 0 && instructions[i - 1].IsLdcI4() && instructions[i - 1].GetLdcI4Value() == 2)
				{
					instructions[i - 1].OpCode = OpCodes.Ldc_I4;
					instructions[i - 1].Operand = 1;
					instructions[i].OpCode = OpCodes.Shl;
					instructions[i].Operand = null;
				}
				continue;
			}
			if ((int)code != 101)
			{
				if ((int)code == 102)
				{
					instructions[i].OpCode = OpCodes.Ldc_I4;
					instructions[i].Operand = -1;
					instructions.Insert(i + 1, new Instruction(OpCodes.Xor));
					i++;
				}
			}
			else
			{
				instructions[i].OpCode = OpCodes.Not;
				instructions[i].Operand = null;
				instructions.Insert(i + 1, Instruction.CreateLdcI4(1));
				instructions.Insert(i + 2, new Instruction(OpCodes.Add));
				i += 2;
			}
		}
		try
		{
			method.Body.OptimizeBranches();
		}
		catch
		{
		}
	}
}
