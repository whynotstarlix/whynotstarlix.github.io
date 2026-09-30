using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.ConstantFolding;

internal class ConstantFoldingPrevention
{
	private const int MaxInstructions = 300;

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		Random rnd = new Random();
		foreach (TypeDef type in module.GetTypes())
		{
			if (type.IsGlobalModuleType || (excluded != null && excluded.Contains(type)))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (IsSafe(method) && (excludedMethods == null || !excludedMethods.Contains(method)))
				{
					try
					{
						ProcessMethod(method, rnd);
					}
					catch
					{
					}
				}
			}
		}
	}

	private static bool IsSafe(MethodDef method)
	{
		if (!method.HasBody)
		{
			return false;
		}
		if (method.IsConstructor || method.IsStaticConstructor)
		{
			return false;
		}
		if (method.Body.HasExceptionHandlers)
		{
			return false;
		}
		if (method.Body.Instructions.Count > 300)
		{
			return false;
		}
		return true;
	}

	private static void ProcessMethod(MethodDef method, Random rnd)
	{
		CilBody body = method.Body;
		IList<Instruction> instructions = body.Instructions;
		body.SimplifyMacros((IList<Parameter>)method.Parameters);
		HashSet<Instruction> hashSet = new HashSet<Instruction>();
		foreach (Instruction item2 in instructions)
		{
			object operand = item2.Operand;
			Instruction val = (Instruction)((operand is Instruction) ? operand : null);
			if (val != null)
			{
				hashSet.Add(val);
			}
			else if (item2.Operand is Instruction[] array)
			{
				Instruction[] array2 = array;
				foreach (Instruction item in array2)
				{
					hashSet.Add(item);
				}
			}
		}
		List<int> list = new List<int>();
		for (int j = 0; j < instructions.Count; j++)
		{
			if (!hashSet.Contains(instructions[j]) && TryGetInt32(instructions[j], out var value) && (value < 0 || value > 4))
			{
				list.Add(j);
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			int num2 = list[num];
			if (num2 < instructions.Count && TryGetInt32(instructions[num2], out var value2))
			{
				Instruction[] array3 = BuildSafe(value2, rnd);
				instructions.RemoveAt(num2);
				for (int num3 = array3.Length - 1; num3 >= 0; num3--)
				{
					instructions.Insert(num2, array3[num3]);
				}
			}
		}
		body.OptimizeMacros();
		body.OptimizeBranches();
	}

	private static Instruction[] BuildSafe(int value, Random rnd)
	{
		int num = rnd.Next(1, int.MaxValue);
		return new Instruction[3]
		{
			Instruction.Create(OpCodes.Ldc_I4, value ^ num),
			Instruction.Create(OpCodes.Ldc_I4, num),
			Instruction.Create(OpCodes.Xor)
		};
	}

	private static bool TryGetInt32(Instruction instr, out int value)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected I4, but got Unknown
		value = 0;
		Code code = instr.OpCode.Code;
		switch (code - 21)
		{
		case 11:
			value = (int)instr.Operand;
			return true;
		case 10:
			value = (sbyte)instr.Operand;
			return true;
		case 0:
			value = -1;
			return true;
		case 1:
			value = 0;
			return true;
		case 2:
			value = 1;
			return true;
		case 3:
			value = 2;
			return true;
		case 4:
			value = 3;
			return true;
		case 5:
			value = 4;
			return true;
		case 6:
			value = 5;
			return true;
		case 7:
			value = 6;
			return true;
		case 8:
			value = 7;
			return true;
		case 9:
			value = 8;
			return true;
		default:
			return false;
		}
	}
}
