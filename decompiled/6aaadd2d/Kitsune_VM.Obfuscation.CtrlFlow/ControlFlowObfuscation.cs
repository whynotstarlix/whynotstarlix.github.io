using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.CtrlFlow;

internal class ControlFlowObfuscation
{
	private static readonly Random rnd = new Random();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
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
						Process(method);
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
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		if (!method.HasBody || !method.Body.HasInstructions)
		{
			return false;
		}
		if (method.Body.Instructions.Count < 8)
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
		if ((int)method.ReturnType.ElementType != 1)
		{
			return false;
		}
		foreach (Instruction instruction in method.Body.Instructions)
		{
			if (instruction.Operand is Instruction)
			{
				return false;
			}
			if (instruction.Operand is Instruction[])
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Calli)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Jmp)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Endfinally)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Leave)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Leave_S)
			{
				return false;
			}
		}
		return true;
	}

	private static void Process(MethodDef method)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected Obj, but got Unknown
		CilBody body = method.Body;
		body.SimplifyMacros((IList<Parameter>)method.Parameters);
		List<Instruction> list = body.Instructions.ToList();
		Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>(list.Count);
		List<Instruction> list2 = new List<Instruction>(list.Count);
		foreach (Instruction item3 in list)
		{
			Instruction item = (dictionary[item3] = CloneSingle(item3));
			list2.Add(item);
		}
		foreach (Instruction item4 in list2)
		{
			RemapBranchOperand(item4, dictionary);
		}
		List<List<Instruction>> list3 = SplitSafe(list2, 5);
		if (list3.Count < 3)
		{
			return;
		}
		body.Instructions.Clear();
		Local val2 = new Local((TypeSig)(object)method.Module.CorLibTypes.Int32);
		body.Variables.Add(val2);
		Instruction val3 = Instruction.Create(OpCodes.Nop);
		IList<Instruction> instructions = body.Instructions;
		List<Instruction> list4 = new List<Instruction>();
		instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
		instructions.Add(Instruction.Create(OpCodes.Stloc, val2));
		instructions.Add(Instruction.Create(OpCodes.Br, val3));
		for (int i = 0; i < list3.Count; i++)
		{
			Instruction item2 = Instruction.Create(OpCodes.Nop);
			list4.Add(item2);
			instructions.Add(item2);
			foreach (Instruction item5 in list3[i])
			{
				if (item5.OpCode != OpCodes.Ret)
				{
					instructions.Add(item5);
				}
			}
			if (i < list3.Count - 1)
			{
				instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i + 1));
				instructions.Add(Instruction.Create(OpCodes.Stloc, val2));
				instructions.Add(Instruction.Create(OpCodes.Br, val3));
			}
		}
		instructions.Add(val3);
		instructions.Add(Instruction.Create(OpCodes.Ldloc, val2));
		instructions.Add(Instruction.Create(OpCodes.Switch, (IList<Instruction>)list4.ToArray()));
		instructions.Add(Instruction.Create(OpCodes.Ret));
		body.OptimizeBranches();
		body.OptimizeMacros();
		body.KeepOldMaxStack = false;
	}

	private static List<List<Instruction>> SplitSafe(List<Instruction> instrs, int size)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		List<List<Instruction>> list = new List<List<Instruction>>();
		List<Instruction> list2 = new List<Instruction>();
		int num = 0;
		int num2 = default;
		int num3 = default;
		foreach (Instruction instr in instrs)
		{
			instr.CalculateStackUsage(ref num2, ref num3);
			list2.Add(instr);
			num += num2 - num3;
			bool flag = num == 0 && (int)instr.OpCode.FlowControl == 5;
			if ((list2.Count >= size) & flag)
			{
				list.Add(list2);
				list2 = new List<Instruction>();
			}
		}
		if (list2.Count > 0)
		{
			list.Add(list2);
		}
		return list;
	}

	private static Instruction CloneSingle(Instruction instr)
	{
		if (instr.Operand == null)
		{
			return Instruction.Create(instr.OpCode);
		}
		if (instr.Operand is int num)
		{
			return Instruction.Create(instr.OpCode, num);
		}
		if (instr.Operand is sbyte b)
		{
			return Instruction.Create(instr.OpCode, b);
		}
		if (instr.Operand is long num2)
		{
			return Instruction.Create(instr.OpCode, num2);
		}
		if (instr.Operand is float num3)
		{
			return Instruction.Create(instr.OpCode, num3);
		}
		if (instr.Operand is double num4)
		{
			return Instruction.Create(instr.OpCode, num4);
		}
		if (instr.Operand is string text)
		{
			return Instruction.Create(instr.OpCode, text);
		}
		object operand = instr.Operand;
		IMethod val = (IMethod)((operand is IMethod) ? operand : null);
		if (val != null)
		{
			return Instruction.Create(instr.OpCode, val);
		}
		object operand2 = instr.Operand;
		IField val2 = (IField)((operand2 is IField) ? operand2 : null);
		if (val2 != null)
		{
			return Instruction.Create(instr.OpCode, val2);
		}
		object operand3 = instr.Operand;
		ITypeDefOrRef val3 = (ITypeDefOrRef)((operand3 is ITypeDefOrRef) ? operand3 : null);
		if (val3 != null)
		{
			return Instruction.Create(instr.OpCode, val3);
		}
		object operand4 = instr.Operand;
		Local val4 = (Local)((operand4 is Local) ? operand4 : null);
		if (val4 != null)
		{
			return Instruction.Create(instr.OpCode, val4);
		}
		object operand5 = instr.Operand;
		Parameter val5 = (Parameter)((operand5 is Parameter) ? operand5 : null);
		if (val5 != null)
		{
			return Instruction.Create(instr.OpCode, val5);
		}
		if (instr.Operand is Instruction)
		{
			return Instruction.Create(instr.OpCode, (Instruction)null);
		}
		if (instr.Operand is Instruction[] array)
		{
			return Instruction.Create(instr.OpCode, (IList<Instruction>)new Instruction[array.Length]);
		}
		return Instruction.Create(instr.OpCode);
	}

	private static void RemapBranchOperand(Instruction instr, Dictionary<Instruction, Instruction> map)
	{
		object operand = instr.Operand;
		Instruction val = (Instruction)((operand is Instruction) ? operand : null);
		if (val != null)
		{
			instr.Operand = (map.TryGetValue(val, out var value) ? value : val);
		}
		else
		{
			if (!(instr.Operand is Instruction[] array))
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && map.TryGetValue(array[i], out var value2))
				{
					array[i] = value2;
				}
			}
		}
	}
}
