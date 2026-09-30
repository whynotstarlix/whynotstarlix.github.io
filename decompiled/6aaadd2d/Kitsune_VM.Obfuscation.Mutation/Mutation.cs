using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Mutation;

internal class Mutation
{
	private class BackupData
	{
		public List<Instruction> Instr;
	}

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
				if (!IsSafe(method) || (excludedMethods != null && excludedMethods.Contains(method)))
				{
					continue;
				}
				BackupData b = Backup(method);
				try
				{
					method.Body.SimplifyMacros((IList<Parameter>)method.Parameters);
					MutateConstants(method, rnd);
					InsertNopNoise(method, rnd);
					ArithmeticLight(method, rnd);
					MathMutations(method, rnd);
					method.Body.OptimizeBranches();
					method.Body.OptimizeMacros();
					if (!Validate(method))
					{
						Restore(method, b);
					}
				}
				catch
				{
					Restore(method, b);
				}
			}
		}
	}

	private static bool IsSafe(MethodDef m)
	{
		if (!m.HasBody)
		{
			return false;
		}
		if (m.IsConstructor || m.IsStaticConstructor)
		{
			return false;
		}
		if (m.Body.HasExceptionHandlers)
		{
			return false;
		}
		foreach (Instruction instruction in m.Body.Instructions)
		{
			if (instruction.OpCode == OpCodes.Calli)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Jmp)
			{
				return false;
			}
		}
		return true;
	}

	private static BackupData Backup(MethodDef m)
	{
		return new BackupData
		{
			Instr = m.Body.Instructions.ToList()
		};
	}

	private static void Restore(MethodDef m, BackupData b)
	{
		m.Body.Instructions.Clear();
		foreach (Instruction item in b.Instr)
		{
			m.Body.Instructions.Add(item);
		}
	}

	private static bool Validate(MethodDef m)
	{
		try
		{
			int num = 0;
			int num2 = default;
			int num3 = default;
			foreach (Instruction instruction in m.Body.Instructions)
			{
				instruction.CalculateStackUsage(ref num2, ref num3);
				num -= num3;
				if (num < 0)
				{
					return false;
				}
				num += num2;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static void MutateConstants(MethodDef m, Random rnd)
	{
		IList<Instruction> instructions = m.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (IsLdc(instructions[i]))
			{
				int val = GetVal(instructions[i]);
				if (val < -1 || val > 8)
				{
					int num = rnd.Next(1000, 10000);
					Instruction[] array = new Instruction[3]
					{
						Instruction.Create(OpCodes.Ldc_I4, val ^ num),
						Instruction.Create(OpCodes.Ldc_I4, num),
						Instruction.Create(OpCodes.Xor)
					};
					Replace(instructions, i, array);
					i += array.Length - 1;
				}
			}
		}
	}

	private static void InsertNopNoise(MethodDef m, Random rnd)
	{
		IList<Instruction> instructions = m.Body.Instructions;
		for (int i = 0; i < instructions.Count; i += rnd.Next(5, 10))
		{
			instructions.Insert(i, Instruction.Create(OpCodes.Nop));
		}
	}

	private static void ArithmeticLight(MethodDef m, Random rnd)
	{
		IList<Instruction> instructions = m.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (IsLdc(instructions[i]) && rnd.Next(4) == 0)
			{
				int val = GetVal(instructions[i]);
				int num = rnd.Next(10, 1000);
				Instruction[] array = new Instruction[3]
				{
					Instruction.Create(OpCodes.Ldc_I4, val + num),
					Instruction.Create(OpCodes.Ldc_I4, num),
					Instruction.Create(OpCodes.Sub)
				};
				Replace(instructions, i, array);
				i += array.Length - 1;
			}
		}
	}

	private static void MathMutations(MethodDef m, Random rnd)
	{
		IList<Instruction> instructions = m.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (IsLdc(instructions[i]) && rnd.Next(3) == 0)
			{
				int val = GetVal(instructions[i]);
				Instruction[] array = null;
				switch (rnd.Next(7))
				{
				case 0:
				{
					int num4 = rnd.Next(1, 32767);
					int num5 = rnd.Next(1, 32767);
					array = new Instruction[5]
					{
						Instruction.Create(OpCodes.Ldc_I4, val + num4 + num5),
						Instruction.Create(OpCodes.Ldc_I4, num5),
						Instruction.Create(OpCodes.Sub),
						Instruction.Create(OpCodes.Ldc_I4, num4),
						Instruction.Create(OpCodes.Sub)
					};
					break;
				}
				case 2:
					array = new Instruction[2]
					{
						Instruction.Create(OpCodes.Ldc_I4, ~val),
						Instruction.Create(OpCodes.Not)
					};
					break;
				case 3:
				{
					int num3 = rnd.Next(1, 32767);
					array = new Instruction[3]
					{
						Instruction.Create(OpCodes.Ldc_I4, val ^ num3),
						Instruction.Create(OpCodes.Ldc_I4, num3),
						Instruction.Create(OpCodes.Xor)
					};
					break;
				}
				case 4:
					array = new Instruction[4]
					{
						Instruction.Create(OpCodes.Ldc_I4, ~val),
						Instruction.Create(OpCodes.Neg),
						Instruction.Create(OpCodes.Ldc_I4, 1),
						Instruction.Create(OpCodes.Sub)
					};
					break;
				case 5:
				{
					int num2 = rnd.Next(1, 32767);
					array = new Instruction[5]
					{
						Instruction.Create(OpCodes.Ldc_I4, val + num2),
						Instruction.Create(OpCodes.Ldc_I4, num2),
						Instruction.Create(OpCodes.Sub),
						Instruction.Create(OpCodes.Ldc_I4, -1),
						Instruction.Create(OpCodes.And)
					};
					break;
				}
				default:
				{
					int num = rnd.Next(4096, int.MaxValue);
					array = new Instruction[3]
					{
						Instruction.Create(OpCodes.Ldc_I4, val ^ num),
						Instruction.Create(OpCodes.Ldc_I4, num),
						Instruction.Create(OpCodes.Xor)
					};
					break;
				}
				}
				if (array != null)
				{
					Replace(instructions, i, array);
					i += array.Length - 1;
				}
			}
		}
	}

	private static void Replace(IList<Instruction> instrs, int index, Instruction[] rep)
	{
		Instruction val = instrs[index];
		instrs.RemoveAt(index);
		for (int num = rep.Length - 1; num >= 0; num--)
		{
			instrs.Insert(index, rep[num]);
		}
		foreach (Instruction instr in instrs)
		{
			if (instr.Operand == val)
			{
				instr.Operand = rep[0];
			}
			if (!(instr.Operand is Instruction[] array))
			{
				continue;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == val)
				{
					array[i] = rep[0];
				}
			}
		}
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
