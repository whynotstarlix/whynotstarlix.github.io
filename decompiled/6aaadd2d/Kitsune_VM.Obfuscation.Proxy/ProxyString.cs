using System;
using System.Collections.Generic;
using System.Linq;
using Kitsune_VM.Obfuscation.Helper;
using Kitsune_VM.Obfuscation.Strings;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Proxy;

internal class ProxyString
{
	private static readonly Random rnd = new Random();

	private static TypeDef _container;

	private static MethodDef _decStrMethod;

	private static readonly Dictionary<string, MethodDef> _cache = new Dictionary<string, MethodDef>();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		_cache.Clear();
		_decStrMethod = module.GlobalType.Methods.FirstOrDefault((MethodDef m) => m.Name == "DecStr");
		if (_decStrMethod == null)
		{
			return;
		}
		_container = CreateContainer(module);
		module.Types.Add(_container);
		foreach (TypeDef item in module.GetTypes().ToList())
		{
			if ((excluded != null && excluded.Contains(item)) || item.Name == "Resources")
			{
				continue;
			}
			UTF8String val = item.Namespace;
			if ((val != null && val.Contains("Properties")) || item.IsGlobalModuleType || item == _container)
			{
				continue;
			}
			foreach (MethodDef item2 in item.Methods.ToList())
			{
				if (IsSafe(item2) && item2 != item2.Module.EntryPoint && !item2.DeclaringType.Name.Contains("Program") && (excludedMethods == null || !excludedMethods.Contains(item2)))
				{
					try
					{
						ProcessMethod(module, item2);
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
		return true;
	}

	private static void ProcessMethod(ModuleDef module, MethodDef method)
	{
		IList<Instruction> instructions = method.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (instructions[i].OpCode != OpCodes.Ldstr)
			{
				continue;
			}
			string text = instructions[i].Operand?.ToString();
			if (!string.IsNullOrEmpty(text) && !IsBase64(text))
			{
				MethodDef orCreateProxy = GetOrCreateProxy(module, text);
				if (orCreateProxy != null)
				{
					instructions[i].OpCode = OpCodes.Call;
					instructions[i].Operand = orCreateProxy;
				}
			}
		}
		method.Body.OptimizeMacros();
		method.Body.OptimizeBranches();
	}

	private static MethodDef GetOrCreateProxy(ModuleDef module, string value)
	{
		if (_cache.TryGetValue(value, out var value2))
		{
			return value2;
		}
		MethodDef val = BuildProxy(module, value);
		if (val == null)
		{
			return null;
		}
		_container.Methods.Add(val);
		_cache[value] = val;
		return val;
	}

	private static MethodDef BuildProxy(ModuleDef module, string value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		try
		{
			string text = StringEncryption.EncStr(value);
			CilBody val = new CilBody();
			InsertJunk(val.Instructions);
			val.Instructions.Add(Instruction.Create(OpCodes.Ldstr, text));
			val.Instructions.Add(Instruction.Create(OpCodes.Call, (IMethod)(object)_decStrMethod));
			val.Instructions.Add(Instruction.Create(OpCodes.Ret));
			val.OptimizeMacros();
			MethodSig val2 = MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.String);
			return (MethodDef)new MethodDefUser(UTF8String.op_Implicit(Methods.GenerateString(16)), val2, (MethodImplAttributes)0, (MethodAttributes)147)
			{
				Body = val
			};
		}
		catch
		{
			return null;
		}
	}

	private static void InsertJunk(IList<Instruction> instrs)
	{
		switch (rnd.Next(3))
		{
		case 0:
			instrs.Add(Instruction.Create(OpCodes.Nop));
			break;
		case 1:
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			instrs.Add(Instruction.Create(OpCodes.Pop));
			break;
		case 2:
			instrs.Add(Instruction.Create(OpCodes.Ldnull));
			instrs.Add(Instruction.Create(OpCodes.Pop));
			break;
		}
	}

	private static bool IsBase64(string s)
	{
		if (string.IsNullOrEmpty(s) || s.Length % 4 != 0)
		{
			return false;
		}
		foreach (char c in s)
		{
			if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
			{
				return false;
			}
		}
		return true;
	}

	private static TypeDef CreateContainer(ModuleDef module)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		return (TypeDef)new TypeDefUser(UTF8String.op_Implicit(Methods.GenerateString()), UTF8String.op_Implicit(Methods.GenerateString()), ((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef)
		{
			Attributes = (TypeAttributes)384
		};
	}
}
