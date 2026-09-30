using System;
using System.Collections.Generic;
using Kitsune_VM.Obfuscation.Helper;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.LocalVariableSplitting;

internal class LocalVariableSplitting
{
	private static readonly Random rnd = new Random();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> allowed = null, HashSet<MethodDef> excludedMethods = null)
	{
		foreach (TypeDef type in module.GetTypes())
		{
			if (type.IsGlobalModuleType || (excluded != null && excluded.Contains(type)))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.HasBody && method.Body.HasInstructions && !method.IsConstructor && !method.IsStaticConstructor && !method.Body.HasExceptionHandlers && method.Body.Variables.Count != 0 && (allowed == null || allowed.Contains(method)) && (excludedMethods == null || !excludedMethods.Contains(method)))
				{
					try
					{
						ProcessMethod(method);
					}
					catch
					{
					}
				}
			}
		}
	}

	private static void ProcessMethod(MethodDef method)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected Obj, but got Unknown
		CilBody body = method.Body;
		body.SimplifyMacros((IList<Parameter>)method.Parameters);
		List<Local> list = new List<Local>();
		Enumerator<Local> enumerator = body.Variables.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Local current = enumerator.Current;
				if ((int)current.Type.ElementType == 8)
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (list.Count == 0)
		{
			return;
		}
		List<Local> list2 = PickRandom(list, Math.Max(1, list.Count / 2));
		Dictionary<Local, Local> dictionary = new Dictionary<Local, Local>();
		CorLibTypeSig @int = method.Module.CorLibTypes.Int32;
		foreach (Local item in list2)
		{
			Local val = new Local((TypeSig)(object)@int, Methods.GenerateString(8));
			body.Variables.Add(val);
			dictionary[item] = val;
		}
		int num = 0;
		foreach (KeyValuePair<Local, Local> item2 in dictionary)
		{
			int num2 = rnd.Next(int.MinValue, int.MaxValue);
			body.Instructions.Insert(num, Instruction.Create(OpCodes.Stloc, item2.Value));
			body.Instructions.Insert(num, Instruction.Create(OpCodes.Ldc_I4, num2));
			num += 2;
		}
		for (int i = num; i < body.Instructions.Count; i++)
		{
			Instruction val2 = body.Instructions[i];
			if (val2.OpCode == OpCodes.Ldloc)
			{
				object operand = val2.Operand;
				Local val3 = (Local)((operand is Local) ? operand : null);
				if (val3 != null && dictionary.TryGetValue(val3, out var value))
				{
					body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Xor));
					body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldloc, value));
					i += 2;
					continue;
				}
			}
			if (val2.OpCode == OpCodes.Stloc)
			{
				object operand2 = val2.Operand;
				Local val4 = (Local)((operand2 is Local) ? operand2 : null);
				if (val4 != null && dictionary.TryGetValue(val4, out var value2))
				{
					int num3 = rnd.Next(int.MinValue, int.MaxValue);
					body.Instructions.Insert(i, Instruction.Create(OpCodes.Xor));
					body.Instructions.Insert(i, Instruction.Create(OpCodes.Ldc_I4, num3));
					body.Instructions.Insert(i, Instruction.Create(OpCodes.Stloc, value2));
					body.Instructions.Insert(i, Instruction.Create(OpCodes.Ldc_I4, num3));
					i += 4;
				}
			}
		}
		body.OptimizeMacros();
	}

	private static List<T> PickRandom<T>(List<T> source, int count)
	{
		List<T> list = new List<T>(source);
		List<T> list2 = new List<T>();
		count = Math.Min(count, list.Count);
		for (int i = 0; i < count; i++)
		{
			int index = rnd.Next(list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		return list2;
	}
}
