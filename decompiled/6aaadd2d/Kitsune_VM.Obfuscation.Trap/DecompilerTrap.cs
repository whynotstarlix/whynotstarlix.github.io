using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.Trap;

internal class DecompilerTrap
{
	private static readonly Random Rnd = new Random();

	public static void Execute(ModuleDef module, int trapTypeCount = 4)
	{
		try
		{
			AddSuppressIldasmAttr(module);
		}
		catch
		{
		}
		for (int i = 0; i < trapTypeCount; i++)
		{
			try
			{
				module.Types.Add(BuildTrapType(module));
			}
			catch
			{
			}
		}
	}

	private static TypeDef BuildTrapType(ModuleDef module)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		TypeDefUser val = new TypeDefUser(UTF8String.op_Implicit(RandName()), UTF8String.op_Implicit(RandName()), ((TypeDefOrRefSig)module.CorLibTypes.Object).TypeDefOrRef)
		{
			Attributes = (TypeAttributes)1048960
		};
		try
		{
			((TypeDef)val).Methods.Add(BuildLocallocMethod(module));
		}
		catch
		{
		}
		if (Rnd.Next(2) == 0)
		{
			try
			{
				((TypeDef)val).Methods.Add(BuildFilterMethod(module));
			}
			catch
			{
			}
		}
		else
		{
			try
			{
				((TypeDef)val).Methods.Add(BuildFaultMethod(module));
			}
			catch
			{
			}
		}
		return (TypeDef)(object)val;
	}

	private static MethodDef BuildFilterMethod(ModuleDef module)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected Obj, but got Unknown
		CilBody val = new CilBody
		{
			InitLocals = true
		};
		Instruction val2 = Instr(OpCodes.Ldc_I4, Rnd.Next());
		Instruction item = Instr(OpCodes.Pop);
		Instruction val3 = Instr(OpCodes.Leave_S, null);
		Instruction val4 = Instr(OpCodes.Pop);
		Instruction item2 = Instr(OpCodes.Ldc_I4_1);
		Instruction item3 = Instr(OpCodes.Endfilter);
		Instruction val5 = Instr(OpCodes.Pop);
		Instruction item4 = Instr(OpCodes.Ldc_I4, Rnd.Next());
		Instruction item5 = Instr(OpCodes.Pop);
		Instruction val6 = Instr(OpCodes.Leave_S, null);
		Instruction val7 = Instr(OpCodes.Ret);
		val.Instructions.Add(val2);
		val.Instructions.Add(item);
		val.Instructions.Add(val3);
		val.Instructions.Add(val4);
		val.Instructions.Add(item2);
		val.Instructions.Add(item3);
		val.Instructions.Add(val5);
		val.Instructions.Add(item4);
		val.Instructions.Add(item5);
		val.Instructions.Add(val6);
		val.Instructions.Add(val7);
		val3.Operand = val7;
		val6.Operand = val7;
		val.ExceptionHandlers.Add(new ExceptionHandler((ExceptionHandlerType)1)
		{
			TryStart = val2,
			TryEnd = val4,
			FilterStart = val4,
			HandlerStart = val5,
			HandlerEnd = val7
		});
		return MakeTrapMethod(module, val);
	}

	private static MethodDef BuildFaultMethod(ModuleDef module)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected Obj, but got Unknown
		CilBody val = new CilBody
		{
			InitLocals = true
		};
		Instruction val2 = Instr(OpCodes.Ldc_I4, Rnd.Next());
		Instruction item = Instr(OpCodes.Pop);
		Instruction val3 = Instr(OpCodes.Leave_S, null);
		Instruction val4 = Instr(OpCodes.Ldc_I4, Rnd.Next());
		Instruction item2 = Instr(OpCodes.Pop);
		Instruction item3 = Instr(OpCodes.Endfinally);
		Instruction val5 = Instr(OpCodes.Ret);
		val.Instructions.Add(val2);
		val.Instructions.Add(item);
		val.Instructions.Add(val3);
		val.Instructions.Add(val4);
		val.Instructions.Add(item2);
		val.Instructions.Add(item3);
		val.Instructions.Add(val5);
		val3.Operand = val5;
		val.ExceptionHandlers.Add(new ExceptionHandler((ExceptionHandlerType)4)
		{
			TryStart = val2,
			TryEnd = val4,
			HandlerStart = val4,
			HandlerEnd = val5
		});
		return MakeTrapMethod(module, val);
	}

	private static MethodDef BuildLocallocMethod(ModuleDef module)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected Obj, but got Unknown
		CilBody val = new CilBody
		{
			InitLocals = false
		};
		val.Instructions.Add(Instr(OpCodes.Ldc_I4, Rnd.Next(8, 64)));
		val.Instructions.Add(Instr(OpCodes.Localloc));
		val.Instructions.Add(Instr(OpCodes.Pop));
		val.Instructions.Add(Instr(OpCodes.Ret));
		return MakeTrapMethod(module, val);
	}

	private static void AddSuppressIldasmAttr(ModuleDef module)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected Obj, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected Obj, but got Unknown
		if (module.Assembly != null)
		{
			AssemblyRef assemblyRef = module.CorLibTypes.AssemblyRef;
			TypeRefUser val = new TypeRefUser(module, UTF8String.op_Implicit("System.Runtime.CompilerServices"), UTF8String.op_Implicit("SuppressIldasmAttribute"), (IResolutionScope)(object)assemblyRef);
			MemberRefUser val2 = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (IMemberRefParent)(object)val);
			((LazyList<CustomAttribute>)(object)module.Assembly.CustomAttributes).Add(new CustomAttribute((ICustomAttributeType)(object)val2));
		}
	}

	private static MethodDef MakeTrapMethod(ModuleDef module, CilBody body)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		return (MethodDef)new MethodDefUser(UTF8String.op_Implicit(RandName()), MethodSig.CreateStatic((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)8, (MethodAttributes)147)
		{
			Body = body
		};
	}

	private static Instruction Instr(OpCode op)
	{
		return Instruction.Create(op);
	}

	private static Instruction Instr(OpCode op, int v)
	{
		return Instruction.Create(op, v);
	}

	private static Instruction Instr(OpCode op, Instruction t)
	{
		return Instruction.Create(op, t);
	}

	private static string RandName()
	{
		return Guid.NewGuid().ToString("N").Substring(0, 16);
	}
}
