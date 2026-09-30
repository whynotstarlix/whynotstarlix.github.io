using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM;

public class KitsuneMethodPatcher
{
	private ModuleDef _module;

	private IMethod _hostExecuteRef;

	private IMethod _hostRegisterArgRef;

	private IMethod _hostRetrieveResultRef;

	public KitsuneMethodPatcher(ModuleDef module)
	{
		_module = module;
	}

	public void Initialize()
	{
		foreach (MethodDef method in (_module.Find("Kitsune_VM_Stub.KitsuneHost", false) ?? throw new Exception("KitsuneMethodPatcher: type KitsuneHost not found in the module. Make sure VM types were injected before patching.")).Methods)
		{
			if (method.Name == "Execute" && method.Parameters.Count == 2)
			{
				_hostExecuteRef = (IMethod)(object)method;
			}
			else if (method.Name == "RegisterArg" && method.Parameters.Count == 1)
			{
				_hostRegisterArgRef = (IMethod)(object)method;
			}
			else if (method.Name == "RetrieveResult" && method.Parameters.Count == 1)
			{
				_hostRetrieveResultRef = (IMethod)(object)method;
			}
		}
		if (_hostExecuteRef == null)
		{
			throw new Exception("KitsuneMethodPatcher: method KitsuneHost.Execute(int, long[]) not found.");
		}
		if (_hostRegisterArgRef == null)
		{
			throw new Exception("KitsuneMethodPatcher: method KitsuneHost.RegisterArg(object) not found.");
		}
		if (_hostRetrieveResultRef == null)
		{
			throw new Exception("KitsuneMethodPatcher: method KitsuneHost.RetrieveResult(long) not found.");
		}
	}

	public void PatchMethod(MethodDef method, int bytecodeIndex)
	{
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected Obj, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected Obj, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected Obj, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected Obj, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected Obj, but got Unknown
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Expected Obj, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected Obj, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected Obj, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected Obj, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected Obj, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected Obj, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected Obj, but got Unknown
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected Obj, but got Unknown
		if (_hostExecuteRef == null)
		{
			throw new InvalidOperationException("KitsuneMethodPatcher: call Initialize() before PatchMethod().");
		}
		int count = method.Parameters.Count;
		CilBody body = method.Body;
		body.Instructions.Clear();
		body.Variables.Clear();
		body.ExceptionHandlers.Clear();
		ITypeDefOrRef val = Extensions.ToTypeDefOrRef((TypeSig)(object)_module.CorLibTypes.Int64);
		body.Instructions.Add(Instruction.CreateLdcI4(bytecodeIndex));
		if (count > 0)
		{
			body.Instructions.Add(Instruction.CreateLdcI4(count));
			body.Instructions.Add(new Instruction(OpCodes.Newarr, (object)val));
			for (int i = 0; i < count; i++)
			{
				body.Instructions.Add(new Instruction(OpCodes.Dup));
				body.Instructions.Add(Instruction.CreateLdcI4(i));
				body.Instructions.Add(new Instruction(OpCodes.Ldarg, (object)method.Parameters[i]));
				TypeSig type = method.Parameters[i].Type;
				if (type != null && (type.IsValueType || type.IsPrimitive))
				{
					body.Instructions.Add(new Instruction(OpCodes.Conv_I8));
				}
				else
				{
					Extensions.ToTypeDefOrRef((TypeSig)(object)_module.CorLibTypes.Object);
					if (type != null && type.IsGenericInstanceType)
					{
						body.Instructions.Add(new Instruction(OpCodes.Box, (object)Extensions.ToTypeDefOrRef(type)));
					}
					body.Instructions.Add(new Instruction(OpCodes.Call, (object)_hostRegisterArgRef));
				}
				body.Instructions.Add(new Instruction(OpCodes.Stelem_I8));
			}
		}
		else
		{
			body.Instructions.Add(new Instruction(OpCodes.Ldnull));
		}
		body.Instructions.Add(new Instruction(OpCodes.Call, (object)_hostExecuteRef));
		if (method.ReturnType.FullName == "System.Void")
		{
			body.Instructions.Add(new Instruction(OpCodes.Pop));
		}
		else
		{
			string fullName = method.ReturnType.FullName;
			bool flag = !method.ReturnType.IsValueType;
			switch (fullName)
			{
			case "System.Int32":
			case "System.Boolean":
			case "System.Byte":
			case "System.Int16":
			case "System.UInt32":
				body.Instructions.Add(new Instruction(OpCodes.Conv_I4));
				break;
			default:
				if (flag && fullName != "System.Int64" && fullName != "System.UInt64")
				{
					body.Instructions.Add(new Instruction(OpCodes.Call, (object)_hostRetrieveResultRef));
					ITypeDefOrRef val2 = Extensions.ToTypeDefOrRef(method.ReturnType);
					if (val2 != null)
					{
						body.Instructions.Add(new Instruction(OpCodes.Castclass, (object)val2));
					}
				}
				break;
			}
		}
		body.Instructions.Add(new Instruction(OpCodes.Ret));
		body.UpdateInstructionOffsets();
	}
}
