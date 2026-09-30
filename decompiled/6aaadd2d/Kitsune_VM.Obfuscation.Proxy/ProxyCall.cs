using System;
using System.Collections.Generic;
using System.Linq;
using Kitsune_VM.Obfuscation.Helper;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Proxy;

internal class ProxyCall
{
	private static readonly Random rnd = new Random();

	private static TypeDef _container;

	private static readonly Dictionary<string, MethodDef> _cache = new Dictionary<string, MethodDef>();

	private static readonly HashSet<string> _proxyContainerNames = new HashSet<string>();

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null)
	{
		_cache.Clear();
		_proxyContainerNames.Clear();
		foreach (TypeDef type in module.GetTypes())
		{
			if (type.IsSealed && type.IsAbstract && !type.IsGlobalModuleType && type.Methods.All((MethodDef m) => m.IsStatic || m.IsConstructor))
			{
				_proxyContainerNames.Add(type.FullName);
			}
		}
		_container = CreateContainer(module);
		module.Types.Add(_container);
		foreach (TypeDef item in module.GetTypes().ToList())
		{
			if ((excluded != null && excluded.Contains(item)) || item.IsGlobalModuleType || item == _container || _proxyContainerNames.Contains(item.FullName))
			{
				continue;
			}
			foreach (MethodDef item2 in item.Methods.ToList())
			{
				if (item2.HasBody && !item2.IsConstructor && !item2.IsStaticConstructor && IsMethodSafe(item2) && (excludedMethods == null || !excludedMethods.Contains(item2)))
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
		foreach (MethodDef method in _container.Methods)
		{
			if (method.HasBody)
			{
				try
				{
					method.Body.SimplifyBranches();
					method.Body.UpdateInstructionOffsets();
				}
				catch
				{
				}
			}
		}
	}

	private static bool IsMethodSafe(MethodDef method)
	{
		if (!method.HasBody)
		{
			return false;
		}
		if (method.IsConstructor || method.IsStaticConstructor)
		{
			return false;
		}
		if (method.ImplMap != null || method.IsPinvokeImpl)
		{
			return false;
		}
		if (method.Body.HasExceptionHandlers)
		{
			return false;
		}
		if (method.DeclaringType.Name.Contains("AntiDebug"))
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
			if (instruction.OpCode == OpCodes.Leave)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Leave_S)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Endfinally)
			{
				return false;
			}
			if (instruction.OpCode == OpCodes.Endfilter)
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

	private static void ProcessMethod(ModuleDef module, MethodDef method)
	{
		IList<Instruction> instructions = method.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (instructions[i].OpCode != OpCodes.Call && instructions[i].OpCode != OpCodes.Callvirt)
			{
				continue;
			}
			object operand = instructions[i].Operand;
			IMethod val = (IMethod)((operand is IMethod) ? operand : null);
			if (val == null)
			{
				continue;
			}
			MethodSig methodSig = val.MethodSig;
			if (methodSig != null && ((CallingConventionSig)methodSig).HasThis)
			{
				continue;
			}
			MethodSig methodSig2 = val.MethodSig;
			if ((methodSig2 == null || !((CallingConventionSig)methodSig2).ContainsGenericParameter) && !(((IFullName)val).Name == ".ctor") && !(((IFullName)val).Name == ".cctor") && (((IMemberRef)val).DeclaringType == null || (!_proxyContainerNames.Contains(((IFullName)((IMemberRef)val).DeclaringType).FullName) && !(((IFullName)((IMemberRef)val).DeclaringType).FullName == _container.FullName))))
			{
				MethodDef orCreateProxy = GetOrCreateProxy(module, val);
				if (orCreateProxy != null)
				{
					instructions[i].OpCode = OpCodes.Call;
					instructions[i].Operand = orCreateProxy;
				}
			}
		}
		try
		{
			method.Body.SimplifyBranches();
			method.Body.UpdateInstructionOffsets();
		}
		catch
		{
		}
	}

	private static MethodDef GetOrCreateProxy(ModuleDef module, IMethod target)
	{
		string fullName = ((IFullName)target).FullName;
		if (_cache.TryGetValue(fullName, out var value))
		{
			return value;
		}
		MethodDef val = BuildProxy(module, target);
		if (val == null)
		{
			return null;
		}
		_container.Methods.Add(val);
		_cache[fullName] = val;
		return val;
	}

	private static MethodDef BuildProxy(ModuleDef module, IMethod target)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected Obj, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected Obj, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected Obj, but got Unknown
		MethodSig methodSig = target.MethodSig;
		if (methodSig == null)
		{
			return null;
		}
		MethodSig val;
		IMethod val2;
		try
		{
			val = new MethodSig((CallingConvention)0, ((MethodBaseSig)methodSig).GenParamCount, module.Import(((MethodBaseSig)methodSig).RetType), (IList<TypeSig>)((MethodBaseSig)methodSig).Params.Select((TypeSig p) => module.Import(p)).ToList());
			val2 = module.Import(target);
		}
		catch
		{
			return null;
		}
		CilBody val3 = new CilBody
		{
			InitLocals = true
		};
		IList<Instruction> instructions = val3.Instructions;
		InsertJunk(instructions);
		for (int num = 0; num < ((MethodBaseSig)methodSig).Params.Count; num++)
		{
			instructions.Add(Instruction.Create(OpCodes.Ldarg, num));
		}
		if (rnd.Next(2) == 0)
		{
			instructions.Add(Instruction.Create(OpCodes.Ldc_I4, rnd.Next()));
			instructions.Add(Instruction.Create(OpCodes.Pop));
		}
		instructions.Add(Instruction.Create(OpCodes.Call, val2));
		if ((int)((MethodBaseSig)methodSig).RetType.ElementType == 1 && rnd.Next(2) == 0)
		{
			InsertJunk(instructions);
		}
		instructions.Add(Instruction.Create(OpCodes.Ret));
		try
		{
			val3.SimplifyBranches();
			val3.UpdateInstructionOffsets();
		}
		catch
		{
			return null;
		}
		return (MethodDef)new MethodDefUser(UTF8String.op_Implicit(Methods.GenerateString(20)), val, (MethodImplAttributes)0, (MethodAttributes)150)
		{
			Body = val3
		};
	}

	private static void InsertJunk(IList<Instruction> instrs)
	{
		switch (rnd.Next(4))
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
		case 3:
		{
			int num = rnd.Next(1, 1000);
			int num2 = rnd.Next(1, 1000);
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, num));
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, num2));
			instrs.Add(Instruction.Create(OpCodes.Xor));
			instrs.Add(Instruction.Create(OpCodes.Pop));
			break;
		}
		}
	}

	private static TypeDef CreateContainer(ModuleDef module)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		return (TypeDef)new TypeDefUser(UTF8String.op_Implicit(string.Join(".", from _ in Enumerable.Range(0, rnd.Next(2, 5))
			select Methods.GenerateString())), UTF8String.op_Implicit(Methods.GenerateString()), ((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef)
		{
			Attributes = (TypeAttributes)1048960
		};
	}
}
