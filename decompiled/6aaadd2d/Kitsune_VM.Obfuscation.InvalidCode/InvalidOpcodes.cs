using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.InvalidCode;

internal static class InvalidOpcodes
{
	private static readonly Random _rnd = new Random();

	private static readonly OpCode[] _prefixOpcodes = new OpCode[3]
	{
		OpCodes.Volatile,
		OpCodes.Unaligned,
		OpCodes.Constrained
	};

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null, HashSet<TypeDef> excludedForStub = null)
	{
		TypeDef[] typeRefs = (from t in module.GetTypes()
			where !t.IsGlobalModuleType
			select t).Take(32).ToArray();
		foreach (TypeDef type in module.GetTypes())
		{
			if ((excluded != null && excluded.Contains(type)) || (excludedForStub != null && !excludedForStub.Contains(type)))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.HasBody && !method.IsConstructor && !method.IsStaticConstructor && (excludedMethods == null || !excludedMethods.Contains(method)))
				{
					InsertDeadBlocks(method, typeRefs, module);
				}
			}
		}
	}

	private static void InsertDeadBlocks(MethodDef method, TypeDef[] typeRefs, ModuleDef module)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		try
		{
			method.Body.SimplifyBranches();
		}
		catch
		{
			return;
		}
		IList<Instruction> instructions = method.Body.Instructions;
		for (int num = instructions.Count - 1; num >= 0; num--)
		{
			if ((instructions[num].OpCode == OpCodes.Br || instructions[num].OpCode == OpCodes.Leave) && num < instructions.Count - 1)
			{
				object operand = instructions[num].Operand;
				Instruction val = (Instruction)((operand is Instruction) ? operand : null);
				if (val != null && val == instructions[num + 1])
				{
					List<Instruction> list = BuildDeadBlock(typeRefs, module);
					if (list.Count != 0)
					{
						Instruction item = new Instruction(OpCodes.Br, (object)val);
						list.Add(item);
						for (int num2 = list.Count - 1; num2 >= 0; num2--)
						{
							instructions.Insert(num + 1, list[num2]);
						}
					}
				}
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

	private static List<Instruction> BuildDeadBlock(TypeDef[] typeRefs, ModuleDef module)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected Obj, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected Obj, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected Obj, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected Obj, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected Obj, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected Obj, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected Obj, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected Obj, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected Obj, but got Unknown
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected Obj, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected Obj, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected Obj, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected Obj, but got Unknown
		List<Instruction> list = new List<Instruction>();
		switch (_rnd.Next(4))
		{
		case 0:
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(new Instruction(OpCodes.Xor));
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(new Instruction(OpCodes.Mul));
			list.Add(new Instruction(OpCodes.Pop));
			break;
		case 1:
			if (typeRefs.Length != 0)
			{
				TypeDef val = typeRefs[_rnd.Next(typeRefs.Length)];
				list.Add(new Instruction(OpCodes.Ldtoken, (object)module.Import(val)));
				list.Add(new Instruction(OpCodes.Pop));
			}
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(new Instruction(OpCodes.Pop));
			break;
		case 2:
			list.Add(new Instruction(OpCodes.Ldstr, (object)GenerateJunkString()));
			list.Add(new Instruction(OpCodes.Pop));
			list.Add(new Instruction(OpCodes.Ldstr, (object)GenerateJunkString()));
			list.Add(new Instruction(OpCodes.Pop));
			break;
		case 3:
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(new Instruction(OpCodes.Neg));
			list.Add(Instruction.CreateLdcI4(_rnd.Next()));
			list.Add(new Instruction(OpCodes.Or));
			list.Add(new Instruction(OpCodes.Pop));
			list.Add(new Instruction(OpCodes.Ldstr, (object)GenerateJunkString()));
			list.Add(new Instruction(OpCodes.Pop));
			break;
		}
		return list;
	}

	private static string GenerateJunkString()
	{
		int num = 4 + _rnd.Next(12);
		char[] array = new char[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = "abcdefghijklmnopqrstuvwxyz0123456789_"[_rnd.Next("abcdefghijklmnopqrstuvwxyz0123456789_".Length)];
		}
		return new string(array);
	}
}
