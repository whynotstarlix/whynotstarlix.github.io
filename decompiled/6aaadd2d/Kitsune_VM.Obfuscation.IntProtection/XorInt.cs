using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.IntProtection;

internal class XorInt
{
	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		Random rnd = new Random();
		foreach (TypeDef type in module.GetTypes())
		{
			if (excluded != null && excluded.Contains(type))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.HasBody && IsMethodSafe(method) && (excludedMethods == null || !excludedMethods.Contains(method)))
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

	public static bool IsMethodSafe(MethodDef method)
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
		if (method.ImplMap != null || method.IsPinvokeImpl)
		{
			return false;
		}
		Instruction val = method.Body.Instructions.FirstOrDefault();
		if (val != null && (val.OpCode == OpCodes.Br || val.OpCode == OpCodes.Br_S))
		{
			return false;
		}
		foreach (Instruction instruction in method.Body.Instructions)
		{
			if (instruction.OpCode == OpCodes.Calli)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Jmp)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Arglist)
			{
				return false;
			}
		}
		try
		{
			int num = 0;
			int num2 = default;
			int num3 = default;
			foreach (Instruction instruction2 in method.Body.Instructions)
			{
				instruction2.CalculateStackUsage(ref num2, ref num3);
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

	private static void ProcessMethod(MethodDef method, Random rnd)
	{
		CilBody body = method.Body;
		IList<Instruction> instructions = body.Instructions;
		body.SimplifyMacros((IList<Parameter>)method.Parameters);
		body.SimplifyBranches();
		List<(int, Instruction[])> list = new List<(int, Instruction[])>();
		for (int i = 0; i < instructions.Count; i++)
		{
			if (IsLdcI4(instructions[i]))
			{
				int ldcI4Value = GetLdcI4Value(instructions[i]);
				list.Add((i, BuildI4Sequence(ldcI4Value, rnd)));
			}
			else if (instructions[i].OpCode == OpCodes.Ldc_I8)
			{
				long original = (long)instructions[i].Operand;
				list.Add((i, BuildI8Sequence(original, rnd)));
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			(int, Instruction[]) tuple = list[num];
			int item = tuple.Item1;
			Instruction[] item2 = tuple.Item2;
			Instruction oldTarget = instructions[item];
			instructions.RemoveAt(item);
			for (int num2 = item2.Length - 1; num2 >= 0; num2--)
			{
				instructions.Insert(item, item2[num2]);
			}
			RedirectBranches(instructions, body, oldTarget, item2[0]);
		}
		body.UpdateInstructionOffsets();
		body.OptimizeBranches();
		body.OptimizeMacros();
	}

	private static Instruction[] BuildI4Sequence(int original, Random rnd)
	{
		switch (rnd.Next(4))
		{
		case 0:
		{
			int num9 = rnd.Next(1, int.MaxValue);
			int num10 = original ^ num9;
			return new Instruction[3]
			{
				Instruction.Create(OpCodes.Ldc_I4, num10),
				Instruction.Create(OpCodes.Ldc_I4, num9),
				Instruction.Create(OpCodes.Xor)
			};
		}
		case 1:
		{
			int num6 = rnd.Next(1, int.MaxValue);
			int num7 = rnd.Next(1, int.MaxValue);
			int num8 = original ^ num6 ^ num7;
			return new Instruction[5]
			{
				Instruction.Create(OpCodes.Ldc_I4, num8),
				Instruction.Create(OpCodes.Ldc_I4, num6),
				Instruction.Create(OpCodes.Xor),
				Instruction.Create(OpCodes.Ldc_I4, num7),
				Instruction.Create(OpCodes.Xor)
			};
		}
		case 2:
		{
			int num4 = rnd.Next(1, int.MaxValue);
			int num5 = ~original ^ num4;
			return new Instruction[4]
			{
				Instruction.Create(OpCodes.Ldc_I4, num5),
				Instruction.Create(OpCodes.Not),
				Instruction.Create(OpCodes.Ldc_I4, num4),
				Instruction.Create(OpCodes.Xor)
			};
		}
		default:
		{
			int num = rnd.Next(1, int.MaxValue);
			int num2 = rnd.Next(1, 65535);
			int num3 = (original ^ num) + num2;
			return new Instruction[5]
			{
				Instruction.Create(OpCodes.Ldc_I4, num3),
				Instruction.Create(OpCodes.Ldc_I4, num2),
				Instruction.Create(OpCodes.Sub),
				Instruction.Create(OpCodes.Ldc_I4, num),
				Instruction.Create(OpCodes.Xor)
			};
		}
		}
	}

	private static Instruction[] BuildI8Sequence(long original, Random rnd)
	{
		switch (rnd.Next(3))
		{
		case 0:
		{
			long num6 = ((long)rnd.Next() << 32) | (uint)rnd.Next();
			long num7 = original ^ num6;
			return new Instruction[3]
			{
				Instruction.Create(OpCodes.Ldc_I8, num7),
				Instruction.Create(OpCodes.Ldc_I8, num6),
				Instruction.Create(OpCodes.Xor)
			};
		}
		case 1:
		{
			long num3 = ((long)rnd.Next() << 32) | (uint)rnd.Next();
			long num4 = ((long)rnd.Next() << 32) | (uint)rnd.Next();
			long num5 = original ^ num3 ^ num4;
			return new Instruction[5]
			{
				Instruction.Create(OpCodes.Ldc_I8, num5),
				Instruction.Create(OpCodes.Ldc_I8, num3),
				Instruction.Create(OpCodes.Xor),
				Instruction.Create(OpCodes.Ldc_I8, num4),
				Instruction.Create(OpCodes.Xor)
			};
		}
		default:
		{
			long num = ((long)rnd.Next() << 32) | (uint)rnd.Next();
			long num2 = ~original ^ num;
			return new Instruction[4]
			{
				Instruction.Create(OpCodes.Ldc_I8, num2),
				Instruction.Create(OpCodes.Not),
				Instruction.Create(OpCodes.Ldc_I8, num),
				Instruction.Create(OpCodes.Xor)
			};
		}
		}
	}

	private static void RedirectBranches(IList<Instruction> instrs, CilBody body, Instruction oldTarget, Instruction newTarget)
	{
		foreach (Instruction instr in instrs)
		{
			object operand = instr.Operand;
			Instruction val = (Instruction)((operand is Instruction) ? operand : null);
			if (val != null && val == oldTarget)
			{
				instr.Operand = newTarget;
			}
			if (!(instr.Operand is Instruction[] array))
			{
				continue;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == oldTarget)
				{
					array[i] = newTarget;
				}
			}
		}
		foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
		{
			if (exceptionHandler.TryStart == oldTarget)
			{
				exceptionHandler.TryStart = newTarget;
			}
			if (exceptionHandler.TryEnd == oldTarget)
			{
				exceptionHandler.TryEnd = newTarget;
			}
			if (exceptionHandler.HandlerStart == oldTarget)
			{
				exceptionHandler.HandlerStart = newTarget;
			}
			if (exceptionHandler.HandlerEnd == oldTarget)
			{
				exceptionHandler.HandlerEnd = newTarget;
			}
			if (exceptionHandler.FilterStart == oldTarget)
			{
				exceptionHandler.FilterStart = newTarget;
			}
		}
	}

	private static bool IsLdcI4(Instruction instr)
	{
		if (instr.OpCode != OpCodes.Ldc_I4 && instr.OpCode != OpCodes.Ldc_I4_0 && instr.OpCode != OpCodes.Ldc_I4_1 && instr.OpCode != OpCodes.Ldc_I4_2 && instr.OpCode != OpCodes.Ldc_I4_3 && instr.OpCode != OpCodes.Ldc_I4_4 && instr.OpCode != OpCodes.Ldc_I4_5 && instr.OpCode != OpCodes.Ldc_I4_6 && instr.OpCode != OpCodes.Ldc_I4_7 && instr.OpCode != OpCodes.Ldc_I4_8 && instr.OpCode != OpCodes.Ldc_I4_M1)
		{
			return instr.OpCode == OpCodes.Ldc_I4_S;
		}
		return true;
	}

	private static int GetLdcI4Value(Instruction instr)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		Code code = instr.OpCode.Code;
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
			10 => (sbyte)instr.Operand, 
			_ => (int)instr.Operand, 
		};
	}
}
