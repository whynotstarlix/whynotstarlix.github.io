using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.Helper;

public static class InjectHelper
{
	private class InjectContext : ImportMapper
	{
		public readonly Dictionary<IDnlibDef, IDnlibDef> Mep = new Dictionary<IDnlibDef, IDnlibDef>();

		public readonly ModuleDef TargetModule;

		public Importer Importer
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
		}

		public InjectContext(ModuleDef target)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			TargetModule = target;
			Importer = new Importer(target, (ImporterOptions)7, default(GenericParamContext), (ImportMapper)(object)this);
		}

		public override ITypeDefOrRef Map(ITypeDefOrRef typeDefOrRef)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected Obj, but got Unknown
			TypeDef val = (TypeDef)(object)((typeDefOrRef is TypeDef) ? typeDefOrRef : null);
			if (val != null && Mep.ContainsKey((IDnlibDef)(object)val))
			{
				return (ITypeDefOrRef)(TypeDef)Mep[(IDnlibDef)(object)val];
			}
			return null;
		}

		public override IMethod Map(MethodDef methodDef)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			if (Mep.ContainsKey((IDnlibDef)(object)methodDef))
			{
				return (IMethod)(MethodDef)Mep[(IDnlibDef)(object)methodDef];
			}
			return null;
		}

		public override IField Map(FieldDef fieldDef)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			if (Mep.ContainsKey((IDnlibDef)(object)fieldDef))
			{
				return (IField)(FieldDef)Mep[(IDnlibDef)(object)fieldDef];
			}
			return null;
		}
	}

	private static TypeDefUser Clone(TypeDef origin)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected Obj, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected Obj, but got Unknown
		TypeDefUser val = new TypeDefUser(origin.Namespace, origin.Name)
		{
			Attributes = origin.Attributes
		};
		if (origin.ClassLayout != null)
		{
			((TypeDef)val).ClassLayout = (ClassLayout)new ClassLayoutUser(origin.ClassLayout.PackingSize, origin.ClassSize);
		}
		foreach (GenericParam genericParameter in origin.GenericParameters)
		{
			((TypeDef)val).GenericParameters.Add((GenericParam)new GenericParamUser(genericParameter.Number, genericParameter.Flags, UTF8String.op_Implicit("-")));
		}
		return val;
	}

	private static MethodDefUser Clone(MethodDef origin)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected Obj, but got Unknown
		MethodDefUser val = new MethodDefUser(origin.Name, (MethodSig)null, origin.ImplAttributes, origin.Attributes);
		foreach (GenericParam genericParameter in origin.GenericParameters)
		{
			((MethodDef)val).GenericParameters.Add((GenericParam)new GenericParamUser(genericParameter.Number, genericParameter.Flags, UTF8String.op_Implicit("-")));
		}
		return val;
	}

	private static FieldDefUser Clone(FieldDef origin)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected Obj, but got Unknown
		return new FieldDefUser(origin.Name, (FieldSig)null, origin.Attributes);
	}

	private static TypeDef PopulateContext(TypeDef typeDef, InjectContext ctx)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		if (!ctx.Mep.TryGetValue((IDnlibDef)(object)typeDef, out var value))
		{
			value = (IDnlibDef)(object)Clone(typeDef);
			ctx.Mep[(IDnlibDef)(object)typeDef] = value;
		}
		TypeDef val = (TypeDef)value;
		foreach (TypeDef nestedType in typeDef.NestedTypes)
		{
			val.NestedTypes.Add(PopulateContext(nestedType, ctx));
		}
		foreach (MethodDef method in typeDef.Methods)
		{
			MethodDefUser val2 = Clone(method);
			ctx.Mep[(IDnlibDef)(object)method] = (IDnlibDef)(object)val2;
			val.Methods.Add((MethodDef)(object)val2);
		}
		foreach (FieldDef field in typeDef.Fields)
		{
			FieldDefUser val3 = Clone(field);
			ctx.Mep[(IDnlibDef)(object)field] = (IDnlibDef)(object)val3;
			val.Fields.Add((FieldDef)(object)val3);
		}
		return val;
	}

	private static void CopyTypeDef(TypeDef typeDef, InjectContext ctx)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		TypeDef val = (TypeDef)ctx.Mep[(IDnlibDef)(object)typeDef];
		Importer importer;
		object baseType;
		if (typeDef.BaseType != null)
		{
			importer = ctx.Importer;
			baseType = importer.Import(typeDef.BaseType);
		}
		else
		{
			baseType = null;
		}
		val.BaseType = (ITypeDefOrRef)baseType;
		foreach (InterfaceImpl @interface in typeDef.Interfaces)
		{
			IList<InterfaceImpl> interfaces = val.Interfaces;
			importer = ctx.Importer;
			interfaces.Add((InterfaceImpl)new InterfaceImplUser(importer.Import(@interface.Interface)));
		}
	}

	private static void CopyMethodDef(MethodDef methodDef, InjectContext ctx)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected Obj, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected Obj, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected Obj, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected Obj, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected Obj, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected Obj, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected Obj, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Expected Obj, but got Unknown
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Expected Obj, but got Unknown
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Expected Obj, but got Unknown
		MethodDef val = (MethodDef)ctx.Mep[(IDnlibDef)(object)methodDef];
		Importer importer = ctx.Importer;
		val.Signature = importer.Import(methodDef.Signature);
		val.Parameters.UpdateParameterTypes();
		if (methodDef.ImplMap != null)
		{
			val.ImplMap = (ImplMap)new ImplMapUser((ModuleRef)new ModuleRefUser(ctx.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);
		}
		Enumerator<CustomAttribute> enumerator = ((LazyList<CustomAttribute>)(object)methodDef.CustomAttributes).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CustomAttribute current = enumerator.Current;
				importer = ctx.Importer;
				CustomAttribute val2 = new CustomAttribute((ICustomAttributeType)importer.Import((IMethod)(object)current.Constructor));
				foreach (CAArgument constructorArgument in current.ConstructorArguments)
				{
					CAArgument current2 = constructorArgument;
					IList<CAArgument> constructorArguments = val2.ConstructorArguments;
					importer = ctx.Importer;
					constructorArguments.Add(new CAArgument(importer.Import(current2.Type), current2.Value));
				}
				((LazyList<CustomAttribute>)(object)val.CustomAttributes).Add(val2);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (!methodDef.HasBody)
		{
			return;
		}
		val.Body = new CilBody(methodDef.Body.InitLocals, (IList<Instruction>)new List<Instruction>(), (IList<ExceptionHandler>)new List<ExceptionHandler>(), (IList<Local>)new List<Local>())
		{
			MaxStack = methodDef.Body.MaxStack
		};
		Dictionary<object, object> bodyMap = new Dictionary<object, object>();
		Enumerator<Local> enumerator3 = methodDef.Body.Variables.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				Local current3 = enumerator3.Current;
				importer = ctx.Importer;
				Local val3 = new Local(importer.Import(current3.Type))
				{
					Name = current3.Name,
					Attributes = current3.Attributes
				};
				val.Body.Variables.Add(val3);
				bodyMap[current3] = val3;
			}
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
		}
		foreach (Instruction instruction in methodDef.Body.Instructions)
		{
			Instruction val4 = new Instruction(instruction.OpCode, (object)null);
			object operand = instruction.Operand;
			IType val5 = (IType)((operand is IType) ? operand : null);
			if (val5 != null)
			{
				importer = ctx.Importer;
				val4.Operand = importer.Import(val5);
			}
			else
			{
				object operand2 = instruction.Operand;
				IMethod val6 = (IMethod)((operand2 is IMethod) ? operand2 : null);
				if (val6 != null)
				{
					importer = ctx.Importer;
					val4.Operand = importer.Import(val6);
				}
				else
				{
					object operand3 = instruction.Operand;
					IField val7 = (IField)((operand3 is IField) ? operand3 : null);
					if (val7 != null)
					{
						importer = ctx.Importer;
						val4.Operand = importer.Import(val7);
					}
					else
					{
						val4.Operand = instruction.Operand;
					}
				}
			}
			val.Body.Instructions.Add(val4);
			bodyMap[instruction] = val4;
		}
		foreach (Instruction instruction2 in val.Body.Instructions)
		{
			if (instruction2.Operand != null && bodyMap.ContainsKey(instruction2.Operand))
			{
				instruction2.Operand = bodyMap[instruction2.Operand];
			}
			else if (instruction2.Operand is Instruction[] source)
			{
				instruction2.Operand = source.Where((Instruction x) => x != null && bodyMap.ContainsKey(x)).Select((Func<Instruction, Instruction>)((Instruction x) =>
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Expected Obj, but got Unknown
					return (Instruction)bodyMap[x];
				})).ToArray();
			}
		}
		foreach (ExceptionHandler exceptionHandler in methodDef.Body.ExceptionHandlers)
		{
			IList<ExceptionHandler> exceptionHandlers = val.Body.ExceptionHandlers;
			ExceptionHandler val8 = new ExceptionHandler(exceptionHandler.HandlerType);
			object catchType;
			if (exceptionHandler.CatchType != null)
			{
				importer = ctx.Importer;
				catchType = importer.Import(exceptionHandler.CatchType);
			}
			else
			{
				catchType = null;
			}
			val8.CatchType = (ITypeDefOrRef)catchType;
			val8.TryStart = (Instruction)bodyMap[exceptionHandler.TryStart];
			val8.TryEnd = ((exceptionHandler.TryEnd == null) ? ((Instruction)null) : ((Instruction)bodyMap[exceptionHandler.TryEnd]));
			val8.HandlerStart = (Instruction)bodyMap[exceptionHandler.HandlerStart];
			val8.HandlerEnd = ((exceptionHandler.HandlerEnd == null) ? ((Instruction)null) : ((Instruction)bodyMap[exceptionHandler.HandlerEnd]));
			val8.FilterStart = ((exceptionHandler.FilterStart == null) ? ((Instruction)null) : ((Instruction)bodyMap[exceptionHandler.FilterStart]));
			exceptionHandlers.Add(val8);
		}
		val.Body.SimplifyMacros((IList<Parameter>)val.Parameters);
		val.Body.OptimizeMacros();
	}

	private static void CopyFieldDef(FieldDef fieldDef, InjectContext ctx)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		FieldDef val = (FieldDef)ctx.Mep[(IDnlibDef)(object)fieldDef];
		Importer importer = ctx.Importer;
		val.Signature = importer.Import(fieldDef.Signature);
	}

	private static void Copy(TypeDef typeDef, InjectContext ctx, bool copySelf)
	{
		if (copySelf)
		{
			CopyTypeDef(typeDef, ctx);
		}
		foreach (TypeDef nestedType in typeDef.NestedTypes)
		{
			Copy(nestedType, ctx, copySelf: true);
		}
		foreach (MethodDef method in typeDef.Methods)
		{
			CopyMethodDef(method, ctx);
		}
		foreach (FieldDef field in typeDef.Fields)
		{
			CopyFieldDef(field, ctx);
		}
	}

	public static TypeDef Inject(TypeDef typeDef, ModuleDef target)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		InjectContext injectContext = new InjectContext(target);
		PopulateContext(typeDef, injectContext);
		Copy(typeDef, injectContext, copySelf: true);
		return (TypeDef)injectContext.Mep[(IDnlibDef)(object)typeDef];
	}

	public static MethodDef Inject(MethodDef methodDef, ModuleDef target)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected Obj, but got Unknown
		InjectContext injectContext = new InjectContext(target);
		injectContext.Mep[(IDnlibDef)(object)methodDef] = (IDnlibDef)(object)Clone(methodDef);
		CopyMethodDef(methodDef, injectContext);
		return (MethodDef)injectContext.Mep[(IDnlibDef)(object)methodDef];
	}

	public static IEnumerable<IDnlibDef> Inject(TypeDef typeDef, TypeDef newType, ModuleDef target)
	{
		InjectContext injectContext = new InjectContext(target);
		injectContext.Mep[(IDnlibDef)(object)typeDef] = (IDnlibDef)(object)newType;
		PopulateContext(typeDef, injectContext);
		Copy(typeDef, injectContext, copySelf: false);
		return ((IEnumerable<IDnlibDef>)injectContext.Mep.Values).Except((IEnumerable<IDnlibDef>)new IDnlibDef[1] { (IDnlibDef)newType });
	}
}
