using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.FakeAttributes;

internal class FakeAttributes
{
	private static readonly List<(string Namespace, string Name)> FakeProtectors = new List<(string, string)>
	{
		("ConfuserEx.Runtime", "ConfusedByAttribute"),
		("ConfuserEx", "AntiTamperProtection"),
		("ConfuserEx", "AntiDumpProtection"),
		("Dotfuscator", "SuppressIldasm"),
		("Dotfuscator.Runtime", "ObfuscatedByDotfuscator"),
		("SmartAssembly.Attributes", "PoweredByAttribute"),
		("SmartAssembly.Attributes", "DoNotCaptureVariablesAttribute"),
		("Eazfuscator.NET", "ObfuscationAttribute"),
		("Eazfuscator.NET", "SkipObfuscationAttribute"),
		("Babel.NET", "ObfuscatedAttribute"),
		("Babel.Licensing", "AssemblyLicenseAttribute"),
		("DeepSea.Obfuscator", "ObfuscatedByDeepSea"),
		("DeepSea.Runtime", "EncryptedAttribute"),
		("Agile.NET", "ObfuscatedAttribute"),
		("Agile.NET.Runtime", "SkipAttribute"),
		("NETReactor.Runtime", "ObfuscatedByReactor"),
		("NETReactor", "AntiDecompilerAttribute"),
		("Obfuscar", "ObfuscateAssemblyAttribute"),
		("Obfuscar.Runtime", "SkipRenameAttribute"),
		("Virbox.Runtime", "ProtectedByVirbox"),
		("Virbox", "VirboxLicenseAttribute"),
		("Themida", "ProtectedByThemida"),
		("WinLicense", "WinLicenseAttribute")
	};

	private static readonly List<(string Namespace, string Name, string Version)> AssemblyLevelMarkers = new List<(string, string, string)>
	{
		("ConfuserEx", "ConfuserExAttribute", "1.6.0.0"),
		("ConfuserEx", "RuntimeAttribute", "1.6.0.0"),
		("Eazfuscator.NET", "RuntimeAttribute", "22.0.0.0"),
		("SmartAssembly", "PoweredBySmartAssembly", "8.0.0.0"),
		("NETReactor", "ProtectedAttribute", "5.0.0.0"),
		("Babel.NET", "RuntimeAttribute", "10.0.0.0")
	};

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected Obj, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00c8: Expected Obj, but got Unknown
		Random random = new Random();
		ITypeDefOrRef val = module.Import(typeof(Attribute));
		List<(string, string)> list = new List<(string, string)>(FakeProtectors);
		Shuffle(list, random);
		int num = random.Next(4, 10);
		for (int i = 0; i < num && i < list.Count; i++)
		{
			var (text, text2) = list[i];
			try
			{
				TypeDefUser val2 = new TypeDefUser(UTF8String.op_Implicit(text), UTF8String.op_Implicit(text2), val)
				{
					Attributes = (TypeAttributes)1048832
				};
				MemberRefUser val3 = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (IMemberRefParent)(object)val);
				MethodDefUser val4 = new MethodDefUser(UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)0, (MethodAttributes)6150)
				{
					Body = new CilBody()
				};
				((MethodDef)val4).Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				((MethodDef)val4).Body.Instructions.Add(Instruction.Create(OpCodes.Call, (MemberRef)(object)val3));
				((MethodDef)val4).Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				((TypeDef)val2).Methods.Add((MethodDef)(object)val4);
				module.Types.Add((TypeDef)(object)val2);
				ApplyFakeAttribute(module, (TypeDef)(object)val2, (MethodDef)(object)val4, random);
			}
			catch
			{
			}
		}
		AddAssemblyMarkers(module, val, random);
	}

	private static void AddAssemblyMarkers(ModuleDef module, ITypeDefOrRef baseAttr, Random rnd)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected Obj, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected Obj, but got Unknown
		//IL_00b5: Expected Obj, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected Obj, but got Unknown
		if (module.Assembly == null)
		{
			return;
		}
		List<(string, string, string)> list = new List<(string, string, string)>(AssemblyLevelMarkers);
		Shuffle(list, rnd);
		int num = rnd.Next(1, 3);
		for (int i = 0; i < num && i < list.Count; i++)
		{
			var (text, text2, _) = list[i];
			try
			{
				TypeDefUser val = new TypeDefUser(UTF8String.op_Implicit(text), UTF8String.op_Implicit(text2), baseAttr)
				{
					Attributes = (TypeAttributes)1048832
				};
				MemberRefUser val2 = new MemberRefUser(module, UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (IMemberRefParent)(object)baseAttr);
				MethodDefUser val3 = new MethodDefUser(UTF8String.op_Implicit(".ctor"), MethodSig.CreateInstance((TypeSig)(object)module.CorLibTypes.Void), (MethodImplAttributes)0, (MethodAttributes)6150)
				{
					Body = new CilBody()
				};
				((MethodDef)val3).Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				((MethodDef)val3).Body.Instructions.Add(Instruction.Create(OpCodes.Call, (MemberRef)(object)val2));
				((MethodDef)val3).Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				((TypeDef)val).Methods.Add((MethodDef)(object)val3);
				module.Types.Add((TypeDef)(object)val);
				((LazyList<CustomAttribute>)(object)module.Assembly.CustomAttributes).Add(new CustomAttribute((ICustomAttributeType)(object)val3));
			}
			catch
			{
			}
		}
	}

	private static void ApplyFakeAttribute(ModuleDef module, TypeDef attrType, MethodDef ctor, Random rnd)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		try
		{
			List<TypeDef> list = new List<TypeDef>(module.GetTypes());
			if (list.Count != 0)
			{
				((LazyList<CustomAttribute>)(object)list[rnd.Next(list.Count)].CustomAttributes).Add(new CustomAttribute((ICustomAttributeType)(object)ctor));
			}
		}
		catch
		{
		}
	}

	private static void Shuffle<T>(List<T> list, Random rnd)
	{
		for (int num = list.Count - 1; num > 0; num--)
		{
			int num2 = rnd.Next(num + 1);
			int index = num;
			int index2 = num2;
			T value = list[num2];
			T value2 = list[num];
			list[index] = value;
			list[index2] = value2;
		}
	}
}
