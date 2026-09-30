using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM;

public class KitsuneVmInjector
{
	private ModuleDef _targetModule;

	private static readonly string[] VM_TYPE_NAMES = new string[14]
	{
		"Kitsune_VM_Stub.KitsuneCrypto", "Kitsune_VM_Stub.KitsunePEReader", "Kitsune_VM_Stub.KitsuneOpcode", "Kitsune_VM_Stub.KitsuneHandlerType", "Kitsune_VM_Stub.KitsuneEHEntry", "Kitsune_VM_Stub.KitsuneRegisterFile", "Kitsune_VM_Stub.KitsuneState", "Kitsune_VM_Stub.KitsuneObjectHeap", "Kitsune_VM_Stub.KitsuneHeapRuntime", "Kitsune_VM_Stub.KitsuneExternalCallTable",
		"Kitsune_VM_Stub.KitsuneDispatcher", "Kitsune_VM_Stub.KitsuneGuard", "Kitsune_VM_Stub.KitsuneHost", "Kitsune_VM_Stub.ExternalCallHandler"
	};

	public KitsuneVmInjector(ModuleDef targetModule)
	{
		_targetModule = targetModule;
	}

	public void InjectVmTypes(string stubPath)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected Obj, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected Obj, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected Obj, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected Obj, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Expected Obj, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Expected Obj, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected Obj, but got Unknown
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists(stubPath))
		{
			throw new FileNotFoundException("KitsuneVmInjector: Stub EXE not found: " + stubPath + ". Build Kitsune_VM_Stub before running the virtualizer.");
		}
		ModuleDef val = (ModuleDef)(object)ModuleDefMD.Load(stubPath, (ModuleCreationOptions)null);
		List<KeyValuePair<TypeDef, TypeDefUser>> list = new List<KeyValuePair<TypeDef, TypeDefUser>>();
		string[] vM_TYPE_NAMES = VM_TYPE_NAMES;
		foreach (string text in vM_TYPE_NAMES)
		{
			if (_targetModule.Find(text, false) == null)
			{
				TypeDef val2 = val.Find(text, false);
				if (val2 == null)
				{
					Console.WriteLine("[WARN] Type not found in Stub: " + text);
					continue;
				}
				TypeDefUser val3 = new TypeDefUser(val2.Namespace, val2.Name, (ITypeDefOrRef)null);
				((TypeDef)val3).Attributes = val2.Attributes;
				_targetModule.Types.Add((TypeDef)(object)val3);
				list.Add(new KeyValuePair<TypeDef, TypeDefUser>(val2, val3));
				CreateNestedShells(val2, val3, list);
			}
		}
		Importer importer = new Importer(_targetModule, (ImporterOptions)7);
		foreach (KeyValuePair<TypeDef, TypeDefUser> item in list)
		{
			TypeDef key = item.Key;
			TypeDefUser value = item.Value;
			if (key.BaseType != null)
			{
				((TypeDef)value).BaseType = importer.Import(key.BaseType);
			}
			foreach (InterfaceImpl @interface in key.Interfaces)
			{
				((TypeDef)value).Interfaces.Add((InterfaceImpl)new InterfaceImplUser(importer.Import(@interface.Interface)));
			}
			foreach (FieldDef field in key.Fields)
			{
				FieldDefUser val4 = new FieldDefUser(field.Name, new FieldSig(importer.Import(field.FieldType)), field.Attributes);
				if (field.HasConstant && field.Constant != null)
				{
					((FieldDef)val4).Constant = (Constant)new ConstantUser(field.Constant.Value, field.Constant.Type);
				}
				((TypeDef)value).Fields.Add((FieldDef)(object)val4);
			}
			foreach (MethodDef method in key.Methods)
			{
				MethodDefUser val5 = new MethodDefUser(method.Name, importer.Import(method.MethodSig), method.ImplAttributes, method.Attributes);
				if (method.HasBody)
				{
					((MethodDef)val5).Body = CloneMethodBody(method.Body, importer);
				}
				foreach (ParamDef paramDef in method.ParamDefs)
				{
					((MethodDef)val5).ParamDefs.Add((ParamDef)new ParamDefUser(paramDef.Name, paramDef.Sequence, paramDef.Attributes));
				}
				((TypeDef)value).Methods.Add((MethodDef)(object)val5);
			}
			string text2 = (key.IsNested ? key.FullName : (UTF8String.op_Implicit(key.Namespace) + "." + UTF8String.op_Implicit(key.Name)));
			Console.WriteLine("[OK] Injected type: " + text2);
		}
		Dictionary<string, TypeDef> dictionary = new Dictionary<string, TypeDef>();
		foreach (TypeDef type in _targetModule.Types)
		{
			if (type.Namespace == "Kitsune_VM_Stub")
			{
				AddTypeDefToMap(type, dictionary);
			}
		}
		int num = 0;
		Queue<TypeDef> queue = new Queue<TypeDef>();
		foreach (TypeDef type2 in _targetModule.Types)
		{
			queue.Enqueue(type2);
		}
		while (queue.Count > 0)
		{
			TypeDef val6 = queue.Dequeue();
			foreach (TypeDef nestedType in val6.NestedTypes)
			{
				queue.Enqueue(nestedType);
			}
			ITypeDefOrRef baseType = val6.BaseType;
			TypeRef val7 = (TypeRef)(object)((baseType is TypeRef) ? baseType : null);
			if (val7 != null)
			{
				TypeDef val8 = LookupTypeRef(val7, dictionary);
				if (val8 != null)
				{
					val6.BaseType = (ITypeDefOrRef)(object)val8;
					num++;
				}
			}
			foreach (InterfaceImpl interface2 in val6.Interfaces)
			{
				ITypeDefOrRef val9 = interface2.Interface;
				TypeRef val10 = (TypeRef)(object)((val9 is TypeRef) ? val9 : null);
				if (val10 != null)
				{
					TypeDef val11 = LookupTypeRef(val10, dictionary);
					if (val11 != null)
					{
						interface2.Interface = (ITypeDefOrRef)(object)val11;
						num++;
					}
				}
			}
			foreach (FieldDef field2 in val6.Fields)
			{
				TypeSig val12 = FixupTypeSig(field2.FieldType, dictionary);
				if (val12 != field2.FieldType)
				{
					field2.FieldType = val12;
					num++;
				}
			}
			foreach (MethodDef method2 in val6.Methods)
			{
				MethodSig val13 = FixupMethodSig(method2.MethodSig, dictionary);
				if (val13 != method2.MethodSig)
				{
					method2.MethodSig = val13;
					num++;
				}
				if (!method2.HasBody)
				{
					continue;
				}
				CilBody body = method2.Body;
				Enumerator<Local> enumerator7 = body.Variables.GetEnumerator();
				try
				{
					while (enumerator7.MoveNext())
					{
						Local current12 = enumerator7.Current;
						TypeSig val14 = FixupTypeSig(current12.Type, dictionary);
						if (val14 != current12.Type)
						{
							current12.Type = val14;
							num++;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator7/*cast due to constrained. prefix*/).Dispose();
				}
				foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
				{
					ITypeDefOrRef catchType = exceptionHandler.CatchType;
					TypeRef val15 = (TypeRef)(object)((catchType is TypeRef) ? catchType : null);
					if (val15 != null)
					{
						TypeDef val16 = LookupTypeRef(val15, dictionary);
						if (val16 != null)
						{
							exceptionHandler.CatchType = (ITypeDefOrRef)(object)val16;
							num++;
						}
					}
				}
				foreach (Instruction instruction in body.Instructions)
				{
					if (FixupInstrOperand(instruction, dictionary))
					{
						num++;
					}
				}
			}
		}
		if (num > 0)
		{
			Console.WriteLine("[Fixup] References replaced: " + num);
		}
		bool flag = false;
		foreach (AssemblyRef assemblyRef in _targetModule.GetAssemblyRefs())
		{
			if (assemblyRef.Name == "Kitsune_VM_Stub")
			{
				Console.WriteLine("[WARN-AsmRef] AssemblyRef to Kitsune_VM_Stub still exists!");
				flag = true;
			}
		}
		if (!flag)
		{
			Console.WriteLine("[Fixup-OK] No remaining references to Kitsune_VM_Stub");
		}
	}

	private static void CreateNestedShells(TypeDef source, TypeDefUser parentShell, List<KeyValuePair<TypeDef, TypeDefUser>> pairs)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		foreach (TypeDef nestedType in source.NestedTypes)
		{
			TypeDefUser val = new TypeDefUser(UTF8String.op_Implicit(""), nestedType.Name, (ITypeDefOrRef)null);
			((TypeDef)val).Attributes = nestedType.Attributes;
			((TypeDef)parentShell).NestedTypes.Add((TypeDef)(object)val);
			pairs.Add(new KeyValuePair<TypeDef, TypeDefUser>(nestedType, val));
			CreateNestedShells(nestedType, val, pairs);
		}
	}

	private static void AddTypeDefToMap(TypeDef td, Dictionary<string, TypeDef> map)
	{
		map[td.FullName] = td;
		foreach (TypeDef nestedType in td.NestedTypes)
		{
			AddTypeDefToMap(nestedType, map);
		}
	}

	private static TypeDef LookupTypeRef(TypeRef tr, Dictionary<string, TypeDef> map)
	{
		IResolutionScope resolutionScope = tr.ResolutionScope;
		TypeRef val = (TypeRef)(object)((resolutionScope is TypeRef) ? resolutionScope : null);
		if (val == null)
		{
			string text = UTF8String.op_Implicit(tr.Namespace);
			string key = (string.IsNullOrEmpty(text) ? UTF8String.op_Implicit(tr.Name) : (text + "." + UTF8String.op_Implicit(tr.Name)));
			if (!map.TryGetValue(key, out var value))
			{
				return null;
			}
			return value;
		}
		TypeDef val2 = LookupTypeRef(val, map);
		if (val2 == null)
		{
			return null;
		}
		foreach (TypeDef nestedType in val2.NestedTypes)
		{
			if (nestedType.Name == tr.Name)
			{
				return nestedType;
			}
		}
		return null;
	}

	private static TypeSig FixupTypeSig(TypeSig sig, Dictionary<string, TypeDef> map)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected Obj, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected Obj, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected Obj, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected Obj, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected Obj, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected Obj, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected Obj, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected Obj, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected Obj, but got Unknown
		if (sig == null)
		{
			return null;
		}
		if (sig is ClassSig)
		{
			ITypeDefOrRef typeDefOrRef = ((TypeDefOrRefSig)(ClassSig)sig).TypeDefOrRef;
			TypeRef val = (TypeRef)(object)((typeDefOrRef is TypeRef) ? typeDefOrRef : null);
			if (val != null)
			{
				TypeDef val2 = LookupTypeRef(val, map);
				if (val2 != null)
				{
					return (TypeSig)new ClassSig((ITypeDefOrRef)(object)val2);
				}
			}
			return sig;
		}
		if (sig is ValueTypeSig)
		{
			ITypeDefOrRef typeDefOrRef2 = ((TypeDefOrRefSig)(ValueTypeSig)sig).TypeDefOrRef;
			TypeRef val3 = (TypeRef)(object)((typeDefOrRef2 is TypeRef) ? typeDefOrRef2 : null);
			if (val3 != null)
			{
				TypeDef val4 = LookupTypeRef(val3, map);
				if (val4 != null)
				{
					return (TypeSig)new ValueTypeSig((ITypeDefOrRef)(object)val4);
				}
			}
			return sig;
		}
		if (sig is SZArraySig)
		{
			SZArraySig val5 = (SZArraySig)sig;
			TypeSig val6 = FixupTypeSig(((TypeSig)val5).Next, map);
			if (val6 == ((TypeSig)val5).Next)
			{
				return sig;
			}
			return (TypeSig)new SZArraySig(val6);
		}
		if (sig is ArraySig)
		{
			ArraySig val7 = (ArraySig)sig;
			TypeSig val8 = FixupTypeSig(((TypeSig)val7).Next, map);
			if (val8 == ((TypeSig)val7).Next)
			{
				return sig;
			}
			return (TypeSig)new ArraySig(val8, ((ArraySigBase)val7).Rank, (IEnumerable<uint>)val7.Sizes, (IEnumerable<int>)val7.LowerBounds);
		}
		if (sig is ByRefSig)
		{
			ByRefSig val9 = (ByRefSig)sig;
			TypeSig val10 = FixupTypeSig(((TypeSig)val9).Next, map);
			if (val10 == ((TypeSig)val9).Next)
			{
				return sig;
			}
			return (TypeSig)new ByRefSig(val10);
		}
		if (sig is PtrSig)
		{
			PtrSig val11 = (PtrSig)sig;
			TypeSig val12 = FixupTypeSig(((TypeSig)val11).Next, map);
			if (val12 == ((TypeSig)val11).Next)
			{
				return sig;
			}
			return (TypeSig)new PtrSig(val12);
		}
		if (sig is GenericInstSig)
		{
			GenericInstSig val13 = (GenericInstSig)sig;
			bool flag = false;
			List<TypeSig> list = new List<TypeSig>();
			foreach (TypeSig genericArgument in val13.GenericArguments)
			{
				TypeSig val14 = FixupTypeSig(genericArgument, map);
				list.Add(val14);
				if (val14 != genericArgument)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return sig;
			}
			return (TypeSig)new GenericInstSig(val13.GenericType, (IList<TypeSig>)list);
		}
		return sig;
	}

	private static MethodSig FixupMethodSig(MethodSig sig, Dictionary<string, TypeDef> map)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected Obj, but got Unknown
		if (sig == null)
		{
			return null;
		}
		TypeSig val = FixupTypeSig(((MethodBaseSig)sig).RetType, map);
		bool flag = val != ((MethodBaseSig)sig).RetType;
		List<TypeSig> list = new List<TypeSig>();
		foreach (TypeSig item in ((MethodBaseSig)sig).Params)
		{
			TypeSig val2 = FixupTypeSig(item, map);
			list.Add(val2);
			if (val2 != item)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return sig;
		}
		return new MethodSig(((MethodBaseSig)sig).CallingConvention, ((MethodBaseSig)sig).GenParamCount, val, (IList<TypeSig>)list);
	}

	private static bool FixupInstrOperand(Instruction instr, Dictionary<string, TypeDef> typeMap)
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected Obj, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected Obj, but got Unknown
		bool result = false;
		object operand = instr.Operand;
		TypeRef val = (TypeRef)((operand is TypeRef) ? operand : null);
		if (val != null)
		{
			TypeDef val2 = LookupTypeRef(val, typeMap);
			if (val2 != null)
			{
				instr.Operand = val2;
				return true;
			}
		}
		else
		{
			object operand2 = instr.Operand;
			TypeSpec val3 = (TypeSpec)((operand2 is TypeSpec) ? operand2 : null);
			if (val3 != null)
			{
				if (val3.TypeSig != null)
				{
					TypeSig val4 = FixupTypeSig(val3.TypeSig, typeMap);
					if (val4 != val3.TypeSig)
					{
						val3.TypeSig = val4;
						result = true;
					}
				}
			}
			else
			{
				object operand3 = instr.Operand;
				MemberRef val5 = (MemberRef)((operand3 is MemberRef) ? operand3 : null);
				if (val5 != null)
				{
					IMemberRefParent val6 = val5.Class;
					TypeRef val7 = (TypeRef)(object)((val6 is TypeRef) ? val6 : null);
					if (val7 != null)
					{
						TypeDef val8 = LookupTypeRef(val7, typeMap);
						if (val8 != null)
						{
							val5.Class = (IMemberRefParent)(object)val8;
							result = true;
						}
					}
					else
					{
						IMemberRefParent val9 = val5.Class;
						TypeSpec val10 = (TypeSpec)(object)((val9 is TypeSpec) ? val9 : null);
						if (val10 != null && val10.TypeSig != null)
						{
							TypeSig val11 = FixupTypeSig(val10.TypeSig, typeMap);
							if (val11 != val10.TypeSig)
							{
								val10.TypeSig = val11;
								result = true;
							}
						}
					}
					CallingConventionSig signature = val5.Signature;
					MethodSig val12 = (MethodSig)(object)((signature is MethodSig) ? signature : null);
					if (val12 != null)
					{
						MethodSig val13 = FixupMethodSig(val12, typeMap);
						if (val13 != val12)
						{
							val5.Signature = (CallingConventionSig)(object)val13;
							result = true;
						}
					}
					else
					{
						CallingConventionSig signature2 = val5.Signature;
						FieldSig val14 = (FieldSig)(object)((signature2 is FieldSig) ? signature2 : null);
						if (val14 != null)
						{
							TypeSig val15 = FixupTypeSig(val14.Type, typeMap);
							if (val15 != val14.Type)
							{
								val5.Signature = (CallingConventionSig)new FieldSig(val15);
								result = true;
							}
						}
					}
				}
				else
				{
					object operand4 = instr.Operand;
					MethodSpec val16 = (MethodSpec)((operand4 is MethodSpec) ? operand4 : null);
					if (val16 != null)
					{
						CallingConventionSig instantiation = val16.Instantiation;
						GenericInstMethodSig val17 = (GenericInstMethodSig)(object)((instantiation is GenericInstMethodSig) ? instantiation : null);
						if (val17 != null)
						{
							bool flag = false;
							List<TypeSig> list = new List<TypeSig>();
							foreach (TypeSig genericArgument in val17.GenericArguments)
							{
								TypeSig val18 = FixupTypeSig(genericArgument, typeMap);
								list.Add(val18);
								if (val18 != genericArgument)
								{
									flag = true;
								}
							}
							if (flag)
							{
								val16.Instantiation = (CallingConventionSig)new GenericInstMethodSig((IList<TypeSig>)list);
								result = true;
							}
						}
					}
				}
			}
		}
		return result;
	}

	private CilBody CloneMethodBody(CilBody source, Importer importer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected Obj, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected Obj, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Invalid comparison between Unknown and I4
		CilBody val = new CilBody();
		val.InitLocals = source.InitLocals;
		val.MaxStack = source.MaxStack;
		Enumerator<Local> enumerator = source.Variables.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Local current = enumerator.Current;
				val.Variables.Add(new Local(importer.Import(current.Type), current.Name));
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>();
		foreach (Instruction instruction in source.Instructions)
		{
			Instruction item = (dictionary[instruction] = new Instruction(instruction.OpCode));
			val.Instructions.Add(item);
		}
		for (int i = 0; i < source.Instructions.Count; i++)
		{
			Instruction val3 = source.Instructions[i];
			Instruction val4 = val.Instructions[i];
			if (val3.Operand == null)
			{
				continue;
			}
			object operand = val3.Operand;
			Instruction val5 = (Instruction)((operand is Instruction) ? operand : null);
			if (val5 != null)
			{
				val4.Operand = dictionary[val5];
				continue;
			}
			if (val3.Operand is Instruction[] array)
			{
				Instruction[] array2 = new Instruction[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = dictionary[array[j]];
				}
				val4.Operand = array2;
				continue;
			}
			object operand2 = val3.Operand;
			IField val6 = (IField)((operand2 is IField) ? operand2 : null);
			if (val6 != null)
			{
				val4.Operand = importer.Import(val6);
				continue;
			}
			object operand3 = val3.Operand;
			IMethod val7 = (IMethod)((operand3 is IMethod) ? operand3 : null);
			if (val7 != null)
			{
				val4.Operand = importer.Import(val7);
				continue;
			}
			object operand4 = val3.Operand;
			ITypeDefOrRef val8 = (ITypeDefOrRef)((operand4 is ITypeDefOrRef) ? operand4 : null);
			if (val8 != null)
			{
				val4.Operand = importer.Import(val8);
				continue;
			}
			object operand5 = val3.Operand;
			Local val9 = (Local)((operand5 is Local) ? operand5 : null);
			if (val9 != null)
			{
				val4.Operand = val.Variables[val9.Index];
				continue;
			}
			object operand6 = val3.Operand;
			Parameter val10 = (Parameter)((operand6 is Parameter) ? operand6 : null);
			if (val10 != null)
			{
				val4.Operand = val10;
			}
			else
			{
				val4.Operand = val3.Operand;
			}
		}
		foreach (ExceptionHandler exceptionHandler in source.ExceptionHandlers)
		{
			ExceptionHandler val11 = new ExceptionHandler(exceptionHandler.HandlerType);
			val11.TryStart = dictionary[exceptionHandler.TryStart];
			val11.TryEnd = ((exceptionHandler.TryEnd != null) ? dictionary[exceptionHandler.TryEnd] : null);
			val11.HandlerStart = dictionary[exceptionHandler.HandlerStart];
			val11.HandlerEnd = ((exceptionHandler.HandlerEnd != null) ? dictionary[exceptionHandler.HandlerEnd] : null);
			if ((int)exceptionHandler.HandlerType == 0 && exceptionHandler.CatchType != null)
			{
				val11.CatchType = importer.Import(exceptionHandler.CatchType);
			}
			else if ((int)exceptionHandler.HandlerType == 1 && exceptionHandler.FilterStart != null)
			{
				val11.FilterStart = dictionary[exceptionHandler.FilterStart];
			}
			val.ExceptionHandlers.Add(val11);
		}
		val.UpdateInstructionOffsets();
		return val;
	}

	public void PrepareEncryptedSections(List<KitsuneTranslationResult> allResults, KitsuneExternalCallRegistry externalCallRegistry, KitsuneStringRegistry stringRegistry, KitsuneFieldRegistry fieldRegistry, KitsuneTypeRegistry typeRegistry, byte[] opcodePerm, byte protectionFlags, byte[] bootstrapKey, out byte[] indexPacket, out byte[][] dataPackets)
	{
		List<string> list = new List<string>();
		foreach (AssemblyRef assemblyRef in _targetModule.GetAssemblyRefs())
		{
			string fullName = assemblyRef.FullName;
			if (!string.IsNullOrEmpty(fullName) && assemblyRef.Name != "Kitsune_VM_Stub")
			{
				list.Add(fullName);
			}
		}
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
			binaryWriter.Write(list.Count);
			foreach (string item in list)
			{
				binaryWriter.Write(item);
			}
			binaryWriter.Write(externalCallRegistry.Serialize());
			binaryWriter.Write(stringRegistry.Serialize());
			binaryWriter.Write(fieldRegistry.Serialize());
			binaryWriter.Write(typeRegistry.Serialize());
			array = memoryStream.ToArray();
		}
		int count = allResults.Count;
		byte[] array2 = KitsuneVmCrypto.RandomBytes(32);
		uint num = uint.MaxValue;
		for (int i = 0; i < array2.Length; i++)
		{
			num ^= array2[i];
			for (int j = 0; j < 8; j++)
			{
				num = (((num & 1) != 0) ? ((num >> 1) ^ 0xEDB88320u) : (num >> 1));
			}
		}
		num ^= 0xFFFFFFFFu;
		byte b = (byte)((opcodePerm != null && opcodePerm.Length > 141) ? opcodePerm[141] : 141);
		byte[][] array3 = new byte[count][];
		for (int k = 0; k < count; k++)
		{
			uint num2 = num ^ (uint)k;
			using MemoryStream memoryStream2 = new MemoryStream();
			using BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2, Encoding.UTF8);
			byte[] array4 = new byte[5]
			{
				b,
				(byte)num2,
				(byte)(num2 >> 8),
				(byte)(num2 >> 16),
				(byte)(num2 >> 24)
			};
			byte[] array5 = new byte[array4.Length + allResults[k].Bytecode.Length];
			Buffer.BlockCopy(array4, 0, array5, 0, array4.Length);
			Buffer.BlockCopy(allResults[k].Bytecode, 0, array5, array4.Length, allResults[k].Bytecode.Length);
			binaryWriter2.Write(array5.Length);
			binaryWriter2.Write(array5);
			WriteEHTable(binaryWriter2, allResults[k].EHEntries);
			array3[k] = memoryStream2.ToArray();
		}
		int num3 = KitsuneVmCrypto.RandomInt(3, 7);
		int[] array6 = new int[count];
		int[] array7 = new int[count];
		int[] array8 = new int[count];
		MemoryStream[] array9 = new MemoryStream[num3];
		for (int l = 0; l < num3; l++)
		{
			array9[l] = new MemoryStream();
		}
		for (int m = 0; m < count; m++)
		{
			int num4 = m % num3;
			array6[m] = num4 + 1;
			array7[m] = (int)array9[num4].Position;
			array8[m] = array3[m].Length;
			array9[num4].Write(array3[m], 0, array3[m].Length);
		}
		int num5 = num3 + 1;
		byte[][] array10 = new byte[num5][];
		array10[0] = array;
		for (int n = 0; n < num3; n++)
		{
			array10[n + 1] = array9[n].ToArray();
		}
		int[] array11 = ShuffleIndices(num5);
		dataPackets = new byte[num5][];
		for (int num6 = 0; num6 < num5; num6++)
		{
			int num7 = array11[num6];
			dataPackets[num7] = KitsuneVmCrypto.Encrypt(array10[num6], array2, num6);
		}
		byte[] buffer = ComputeChunksCRC(dataPackets, array11, num5);
		byte[] plain;
		using (MemoryStream memoryStream3 = new MemoryStream())
		{
			using BinaryWriter binaryWriter3 = new BinaryWriter(memoryStream3);
			binaryWriter3.Write(num5);
			binaryWriter3.Write(opcodePerm);
			binaryWriter3.Write(array2);
			for (int num8 = 0; num8 < num5; num8++)
			{
				binaryWriter3.Write(array11[num8]);
			}
			binaryWriter3.Write(count);
			for (int num9 = 0; num9 < count; num9++)
			{
				binaryWriter3.Write(array6[num9]);
			}
			for (int num10 = 0; num10 < count; num10++)
			{
				binaryWriter3.Write(array7[num10]);
			}
			for (int num11 = 0; num11 < count; num11++)
			{
				binaryWriter3.Write(array8[num11]);
			}
			binaryWriter3.Write(buffer);
			binaryWriter3.Write(protectionFlags);
			plain = memoryStream3.ToArray();
		}
		indexPacket = KitsuneVmCrypto.Encrypt(plain, bootstrapKey);
		for (int num12 = ((LazyList<Resource>)(object)_targetModule.Resources).Count - 1; num12 >= 0; num12--)
		{
			if (((LazyList<Resource>)(object)_targetModule.Resources)[num12].Name == "KitsuneBytecode")
			{
				((LazyList<Resource>)(object)_targetModule.Resources).RemoveAt(num12);
				break;
			}
		}
		Console.WriteLine($"[OK] Bytecode prepared: {allResults.Count} methods, {num3} chunks (AES-256-CBC + XOR + Caesar)");
	}

	private static byte[] ComputeChunksCRC(byte[][] dataPackets, int[] logToPhys, int totalChunks)
	{
		using SHA256 sHA = SHA256.Create();
		using MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < totalChunks; i++)
		{
			byte[] array = dataPackets[logToPhys[i]];
			memoryStream.Write(array, 0, array.Length);
		}
		return sHA.ComputeHash(memoryStream.ToArray());
	}

	private static byte[][] SplitRandom(byte[] data, int n)
	{
		if (n <= 1)
		{
			return new byte[1][] { data };
		}
		Random random = new Random();
		int[] array = new int[n - 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = random.Next(1, data.Length);
		}
		Array.Sort(array);
		byte[][] array2 = new byte[n][];
		int num = 0;
		for (int j = 0; j < n - 1; j++)
		{
			int num2 = array[j] - num;
			if (num2 < 1)
			{
				num2 = 1;
			}
			array2[j] = new byte[num2];
			Buffer.BlockCopy(data, num, array2[j], 0, num2);
			num = array[j];
		}
		array2[n - 1] = new byte[data.Length - num];
		Buffer.BlockCopy(data, num, array2[n - 1], 0, array2[n - 1].Length);
		return array2;
	}

	private static int[] ShuffleIndices(int n)
	{
		Random random = new Random();
		int[] array = new int[n];
		for (int i = 0; i < n; i++)
		{
			array[i] = i;
		}
		for (int num = n - 1; num > 0; num--)
		{
			int num2 = random.Next(num + 1);
			int num3 = array[num];
			array[num] = array[num2];
			array[num2] = num3;
		}
		return array;
	}

	private static void WriteEHTable(BinaryWriter bw, List<KitsuneEHEntryRef> entries)
	{
		if (entries == null)
		{
			bw.Write(0);
			return;
		}
		bw.Write(entries.Count);
		foreach (KitsuneEHEntryRef entry in entries)
		{
			bw.Write(entry.VmTryStart);
			bw.Write(entry.VmTryEnd);
			bw.Write(entry.VmHandlerStart);
			bw.Write(entry.VmHandlerEnd);
			bw.Write((byte)entry.HandlerType);
			bw.Write(entry.CatchTypeId);
		}
	}

	public void PatchBootstrapKey(byte[] key)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected Obj, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected Obj, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected Obj, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected Obj, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected Obj, but got Unknown
		TypeDef val = null;
		foreach (TypeDef type in _targetModule.GetTypes())
		{
			if (type.Name == "KitsuneCrypto")
			{
				val = type;
				break;
			}
		}
		if (val == null)
		{
			throw new Exception("PatchBootstrapKey: KitsuneCrypto not found in target module");
		}
		MethodDef val2 = null;
		foreach (MethodDef method in val.Methods)
		{
			if (method.Name == "BuildBootstrapKey")
			{
				val2 = method;
				break;
			}
		}
		if (val2 == null)
		{
			throw new Exception("PatchBootstrapKey: BuildBootstrapKey not found in KitsuneCrypto");
		}
		CilBody val3 = new CilBody();
		val3.InitLocals = false;
		ITypeDefOrRef typeDefOrRef = ((TypeDefOrRefSig)_targetModule.CorLibTypes.Byte).TypeDefOrRef;
		val3.Instructions.Add(Instruction.CreateLdcI4(key.Length));
		val3.Instructions.Add(new Instruction(OpCodes.Newarr, (object)typeDefOrRef));
		for (int i = 0; i < key.Length; i++)
		{
			val3.Instructions.Add(new Instruction(OpCodes.Dup));
			val3.Instructions.Add(Instruction.CreateLdcI4(i));
			val3.Instructions.Add(Instruction.CreateLdcI4((int)key[i]));
			val3.Instructions.Add(new Instruction(OpCodes.Stelem_I1));
		}
		val3.Instructions.Add(new Instruction(OpCodes.Ret));
		val3.MaxStack = 4;
		val2.Body = val3;
	}

	public void PatchDebugMode(bool debugMode)
	{
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected Obj, but got Unknown
		TypeDef val = null;
		foreach (TypeDef type in _targetModule.GetTypes())
		{
			if (type.Name == "KitsuneHost")
			{
				val = type;
				break;
			}
		}
		if (val == null)
		{
			return;
		}
		FieldDef val2 = null;
		foreach (FieldDef field in val.Fields)
		{
			if (field.Name == "_debugMode")
			{
				val2 = field;
				break;
			}
		}
		if (val2 == null)
		{
			return;
		}
		MethodDef val3 = null;
		foreach (MethodDef method in val.Methods)
		{
			if (method.IsStaticConstructor && method.HasBody)
			{
				val3 = method;
				break;
			}
		}
		if (val3 == null)
		{
			return;
		}
		IList<Instruction> instructions = val3.Body.Instructions;
		for (int i = 1; i < instructions.Count; i++)
		{
			if (instructions[i].OpCode == OpCodes.Stsfld)
			{
				object operand = instructions[i].Operand;
				FieldDef val4 = (FieldDef)((operand is FieldDef) ? operand : null);
				if (val4 != null && val4 == val2)
				{
					instructions[i - 1] = (debugMode ? new Instruction(OpCodes.Ldc_I4_1) : new Instruction(OpCodes.Ldc_I4_0));
					break;
				}
			}
		}
	}
}
