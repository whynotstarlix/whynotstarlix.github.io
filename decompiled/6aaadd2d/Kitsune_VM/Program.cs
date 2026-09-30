using System;
using System.Collections.Generic;
using System.IO;
using Kitsune_VM.Obfuscation;
using Kitsune_VM.Obfuscation.FakeAttributes;
using Kitsune_VM.Obfuscation.InstructionSubstitution;
using Kitsune_VM.Obfuscation.IntProtection;
using Kitsune_VM.Obfuscation.InvalidCode;
using Kitsune_VM.Obfuscation.Mutation;
using Kitsune_VM.Obfuscation.Rename;
using Kitsune_VM.Obfuscation.Resources;
using Kitsune_VM.Obfuscation.Strings;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Kitsune_VM;

internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine("╔══════════════════════════════════════╗");
		Console.WriteLine("║     Kitsune VM Virtualizer v0.1      ║");
		Console.WriteLine("╚══════════════════════════════════════╝");
		Console.WriteLine();
		if (args.Length < 1)
		{
			Console.WriteLine("Usage: Kitsune_VM.exe <target.exe> [stub.exe] [flags]");
			Console.WriteLine();
			Console.WriteLine("  target.exe     — EXE to virtualize");
			Console.WriteLine("  stub.exe       — path to Kitsune_VM_Stub.exe (optional)");
			Console.WriteLine();
			Console.WriteLine("  Protection flags (all enabled by default):");
			Console.WriteLine("    --no-antidebug       disable debugger detection");
			Console.WriteLine("    --no-antidump        disable PE header erasure");
			Console.WriteLine("    --no-antivm          disable virtual machine detection");
			Console.WriteLine("    --no-vmcrc           disable VM code integrity check");
			Console.WriteLine();
			Console.WriteLine("  Obfuscation flags (all enabled by default):");
			Console.WriteLine("    --no-obfuscation     disable ALL obfuscation passes");
			Console.WriteLine("    --no-rename          disable renaming");
			Console.WriteLine("    --no-string-enc      disable string encryption");
			Console.WriteLine("    --no-resource-enc    disable resource encryption");
			Console.WriteLine("    --no-junk            disable junk class injection");
			Console.WriteLine("    --no-ctrlflow        disable control flow flattening");
			Console.WriteLine("    --no-proxy           disable proxy wrappers");
			Console.WriteLine("    --no-int-protection  disable integer obfuscation");
			Console.WriteLine("    --no-const-folding   disable constant folding prevention");
			Console.WriteLine("    --no-mutation        disable arithmetic mutation");
			Console.WriteLine("    --no-local-split     disable local variable XOR-splitting");
			Console.WriteLine("    --no-fake-attrs      disable fake attribute injection");
			Console.WriteLine("    --no-trap            disable decompiler traps");
			Console.WriteLine("    --no-instr-sub       disable instruction substitution");
			Console.WriteLine("    --no-invalid-opcodes disable dead-block invalid opcode injection");
			Console.WriteLine("    --no-invalid-methods disable fake method injection");
			Console.WriteLine("    --no-morph           disable metamorphic dispatcher");
			Console.WriteLine();
			Console.WriteLine("  File size:");
			Console.WriteLine("    --pump-size <MB>     append N MB of pseudo-random bytes to the output");
			Console.WriteLine();
			Console.WriteLine("  Verbosity:");
			Console.WriteLine("    --debug              enable verbose packer output (default: silent)");
			Console.ReadKey();
			return;
		}
		string text = args[0];
		string text2 = ((args.Length >= 2 && !args[1].StartsWith("--")) ? args[1] : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Kitsune_VM_Stub.exe"));
		byte b = 15;
		foreach (string obj in args)
		{
			if (obj == "--no-antidebug")
			{
				b &= 0xFE;
			}
			if (obj == "--no-antidump")
			{
				b &= 0xFD;
			}
			if (obj == "--no-antivm")
			{
				b &= 0xFB;
			}
			if (obj == "--no-vmcrc")
			{
				b &= 0xF7;
			}
		}
		bool debugMode = Array.Exists(args, (string a) => a == "--debug");
		int result = 0;
		for (int num = 0; num < args.Length - 1; num++)
		{
			if (args[num] == "--pump-size")
			{
				int.TryParse(args[num + 1], out result);
				break;
			}
		}
		bool flag = Array.Exists(args, (string a) => a == "--no-obfuscation");
		bool flag2 = Array.Exists(args, (string a) => a == "--no-rename");
		bool flag3 = Array.Exists(args, (string a) => a == "--no-string-enc");
		bool flag4 = Array.Exists(args, (string a) => a == "--no-junk");
		bool flag5 = Array.Exists(args, (string a) => a == "--no-ctrlflow");
		bool flag6 = Array.Exists(args, (string a) => a == "--no-proxy");
		bool flag7 = Array.Exists(args, (string a) => a == "--no-resource-enc");
		bool flag8 = Array.Exists(args, (string a) => a == "--no-instr-sub");
		bool flag9 = Array.Exists(args, (string a) => a == "--no-invalid-opcodes");
		bool flag10 = Array.Exists(args, (string a) => a == "--no-invalid-methods");
		bool flag11 = Array.Exists(args, (string a) => a == "--no-morph");
		bool flag12 = Array.Exists(args, (string a) => a == "--no-proxy-int");
		bool flag13 = Array.Exists(args, (string a) => a == "--no-proxy-arith");
		bool flag14 = Array.Exists(args, (string a) => a == "--no-int-protection");
		bool flag15 = Array.Exists(args, (string a) => a == "--no-const-folding");
		bool flag16 = Array.Exists(args, (string a) => a == "--no-mutation");
		bool flag17 = Array.Exists(args, (string a) => a == "--no-local-split");
		bool flag18 = Array.Exists(args, (string a) => a == "--no-fake-attrs");
		bool flag19 = Array.Exists(args, (string a) => a == "--no-trap");
		Console.WriteLine("Protections: antidebug=" + ((b & 1) != 0) + " antidump=" + ((b & 2) != 0) + " antivm=" + ((b & 4) != 0) + " vmcrc=" + ((b & 8) != 0));
		string text3 = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text) + "_kitsune.exe");
		if (!File.Exists(text))
		{
			Console.WriteLine("[ERROR] File not found: " + text);
			Console.ReadKey();
			return;
		}
		if (!File.Exists(text2))
		{
			Console.WriteLine("[ERROR] Stub not found: " + text2);
			Console.WriteLine("Build Kitsune_VM_Stub.exe and provide the path to it.");
			Console.ReadKey();
			return;
		}
		Console.WriteLine("Target: " + text);
		Console.WriteLine("Stub:   " + text2);
		Console.WriteLine("Output: " + text3);
		Console.WriteLine();
		ObfuscatorOptions obfuscatorOptions = new ObfuscatorOptions();
		if (flag)
		{
			obfuscatorOptions.Rename = false;
			obfuscatorOptions.ControlFlow = false;
			obfuscatorOptions.StringEncryption = false;
			obfuscatorOptions.IntProtection = false;
			obfuscatorOptions.ConstantFolding = false;
			obfuscatorOptions.Mutation = false;
			obfuscatorOptions.ProxyCall = false;
			obfuscatorOptions.ProxyMethods = false;
			obfuscatorOptions.ProxyString = false;
			obfuscatorOptions.Junk = false;
			obfuscatorOptions.LocalVariableSplit = false;
			obfuscatorOptions.FakeAttributes = false;
			obfuscatorOptions.ResourceEncryption = false;
			obfuscatorOptions.InstructionSubstitution = false;
			obfuscatorOptions.InvalidOpcodes = false;
			obfuscatorOptions.InvalidMethods = false;
			obfuscatorOptions.DecompilerTrap = false;
			obfuscatorOptions.MorphDispatcher = false;
			obfuscatorOptions.ProxyInt = false;
			obfuscatorOptions.ProxyArithmetic = false;
		}
		if (flag2)
		{
			obfuscatorOptions.Rename = false;
		}
		if (flag3)
		{
			obfuscatorOptions.StringEncryption = (obfuscatorOptions.ProxyString = false);
		}
		if (flag7)
		{
			obfuscatorOptions.ResourceEncryption = false;
		}
		if (flag4)
		{
			obfuscatorOptions.Junk = false;
		}
		if (flag5)
		{
			obfuscatorOptions.ControlFlow = false;
		}
		if (flag6)
		{
			obfuscatorOptions.ProxyCall = (obfuscatorOptions.ProxyMethods = (obfuscatorOptions.ProxyString = (obfuscatorOptions.ProxyInt = (obfuscatorOptions.ProxyArithmetic = false))));
		}
		if (flag12)
		{
			obfuscatorOptions.ProxyInt = false;
		}
		if (flag13)
		{
			obfuscatorOptions.ProxyArithmetic = false;
		}
		if (flag8)
		{
			obfuscatorOptions.InstructionSubstitution = false;
		}
		if (flag9)
		{
			obfuscatorOptions.InvalidOpcodes = false;
		}
		if (flag10)
		{
			obfuscatorOptions.InvalidMethods = false;
		}
		if (flag14)
		{
			obfuscatorOptions.IntProtection = false;
		}
		if (flag15)
		{
			obfuscatorOptions.ConstantFolding = false;
		}
		if (flag16)
		{
			obfuscatorOptions.Mutation = false;
		}
		if (flag17)
		{
			obfuscatorOptions.LocalVariableSplit = false;
		}
		if (flag18)
		{
			obfuscatorOptions.FakeAttributes = false;
		}
		if (flag19)
		{
			obfuscatorOptions.DecompilerTrap = false;
		}
		if (flag11)
		{
			obfuscatorOptions.MorphDispatcher = false;
		}
		try
		{
			Virtualize(text, text2, text3, b, obfuscatorOptions, debugMode);
			if (result > 0)
			{
				PumpFileSize(text3, result);
			}
			Console.WriteLine();
			Console.WriteLine("[DONE] " + text3);
		}
		catch (Exception ex)
		{
			Console.WriteLine();
			Console.WriteLine("[ERROR] " + ex.Message);
			Console.WriteLine(ex.StackTrace);
		}
		Console.ReadKey();
	}

	private static void PumpFileSize(string path, int addMB)
	{
		long num = (long)addMB * 1048576L;
		Console.WriteLine("Pumping file size: +" + addMB + " MB...");
		Random random = new Random();
		byte[] buffer = new byte[1048576];
		using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
		{
			long num2 = num;
			while (num2 > 0)
			{
				int num3 = (int)Math.Min(num2, 1048576L);
				if (num3 == 1048576)
				{
					random.NextBytes(buffer);
					fileStream.Write(buffer, 0, num3);
					num2 -= num3;
					continue;
				}
				byte[] buffer2 = new byte[num3];
				random.NextBytes(buffer2);
				fileStream.Write(buffer2, 0, num3);
				num2 -= num3;
				break;
			}
		}
		long length = new FileInfo(path).Length;
		Console.WriteLine($"       Final size: {length / 1048576} MB");
	}

	private static void Virtualize(string targetPath, string stubPath, string outputPath, byte protectionFlags, ObfuscatorOptions obfOpts = null, bool debugMode = false)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Expected Obj, but got Unknown
		//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd9: Unknown result type (might be due to invalid IL or missing references)
		Console.WriteLine("Step 1: Opening " + Path.GetFileName(targetPath) + "...");
		ModuleDef val = (ModuleDef)(object)ModuleDefMD.Load(targetPath, new ModuleCreationOptions());
		Console.WriteLine("       Module:  " + UTF8String.op_Implicit(val.Name));
		Console.WriteLine("       Runtime: " + val.RuntimeVersion);
		Console.WriteLine("Step 2: Analyzing methods...");
		List<MethodDef> list = new List<MethodDef>();
		foreach (TypeDef type in val.Types)
		{
			if (ShouldSkipType(type))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (ShouldVirtualize(method))
				{
					list.Add(method);
				}
			}
		}
		Console.WriteLine("       Methods found for virtualization: " + list.Count);
		if (list.Count == 0)
		{
			Console.WriteLine("[WARN] No methods to virtualize. Make sure target.exe contains methods with a body.");
			return;
		}
		if (obfOpts.ResourceEncryption)
		{
			Console.WriteLine("Step 2.5: Resource encryption (pre-translate)...");
			try
			{
				ResourceEncryption.Execute(val, null, null, obfOpts);
				obfOpts.ResourceEncryption = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine("  [WARN] ResourceEncryption failed: " + ex.Message);
			}
		}
		Console.WriteLine("Step 3: Translating IL → VM bytecode...");
		KitsuneExternalCallRegistry kitsuneExternalCallRegistry = new KitsuneExternalCallRegistry();
		KitsuneStringRegistry stringRegistry = new KitsuneStringRegistry();
		KitsuneFieldRegistry kitsuneFieldRegistry = new KitsuneFieldRegistry();
		KitsuneTypeRegistry kitsuneTypeRegistry = new KitsuneTypeRegistry();
		byte[] array = KitsuneVmCrypto.GenerateOpcodePerm();
		Console.WriteLine("       Opcode permutation: generated (" + array[0] + "," + array[1] + "," + array[2] + "...)");
		KitsuneILTranslator kitsuneILTranslator = new KitsuneILTranslator(kitsuneExternalCallRegistry, stringRegistry, kitsuneFieldRegistry, kitsuneTypeRegistry, array);
		List<KitsuneTranslationResult> list2 = new List<KitsuneTranslationResult>();
		List<MethodDef> list3 = new List<MethodDef>();
		foreach (MethodDef item in list)
		{
			string text = UTF8String.op_Implicit(item.DeclaringType.Name) + "::" + UTF8String.op_Implicit(item.Name);
			try
			{
				KitsuneTranslationResult kitsuneTranslationResult = kitsuneILTranslator.Translate(item);
				list2.Add(kitsuneTranslationResult);
				list3.Add(item);
				Console.WriteLine("       [OK] " + text + " → " + kitsuneTranslationResult.Bytecode.Length + " bytes" + ((kitsuneTranslationResult.EHEntries.Count > 0) ? (" (" + kitsuneTranslationResult.EHEntries.Count + " EH)") : ""));
			}
			catch (NotSupportedException ex2)
			{
				Console.WriteLine("       [SKIP] " + text + ": " + ex2.Message);
			}
			catch (Exception ex3)
			{
				Console.WriteLine("       [ERR]  " + text + ": " + ex3.Message);
			}
		}
		Console.WriteLine("       Successfully translated: " + list3.Count);
		if (list3.Count == 0)
		{
			Console.WriteLine("[WARN] No methods were translated.");
			return;
		}
		if (obfOpts == null)
		{
			obfOpts = new ObfuscatorOptions();
		}
		Dictionary<string, TypeDef> dictionary = new Dictionary<string, TypeDef>();
		foreach (TypeDef type2 in val.GetTypes())
		{
			dictionary[type2.FullName] = type2;
		}
		Console.WriteLine("Step 4: Injecting VM into the module...");
		HashSet<TypeDef> hashSet = new HashSet<TypeDef>(val.GetTypes());
		KitsuneVmInjector kitsuneVmInjector = new KitsuneVmInjector(val);
		kitsuneVmInjector.InjectVmTypes(stubPath);
		byte[] array2 = KitsuneVmCrypto.GenerateBootstrapKey();
		kitsuneVmInjector.PatchBootstrapKey(array2);
		kitsuneVmInjector.PatchDebugMode(debugMode);
		Console.WriteLine("       Bootstrap key: " + array2.Length + " bytes (unique per build)");
		if (obfOpts.MorphDispatcher)
		{
			Console.WriteLine("Step 4.5: Metamorphic dispatcher...");
			KitsuneDispatcherMorpher.Morph(val);
		}
		foreach (TypeDef type3 in val.GetTypes())
		{
			obfOpts.ExcludedTypes.Add(type3);
		}
		Console.WriteLine("Step 5: Patching methods...");
		KitsuneMethodPatcher kitsuneMethodPatcher = new KitsuneMethodPatcher(val);
		kitsuneMethodPatcher.Initialize();
		for (int i = 0; i < list3.Count; i++)
		{
			MethodDef val2 = list3[i];
			string text2 = UTF8String.op_Implicit(val2.DeclaringType.Name) + "::" + UTF8String.op_Implicit(val2.Name);
			try
			{
				kitsuneMethodPatcher.PatchMethod(val2, i);
				Console.WriteLine("       [OK] " + text2);
			}
			catch (Exception ex4)
			{
				Console.WriteLine("       [ERR] " + text2 + ": " + ex4.Message);
			}
		}
		foreach (MethodDef item2 in list3)
		{
			obfOpts.ExcludedMethods.Add(item2);
		}
		Console.WriteLine("Step 5.5: Obfuscation...");
		Dictionary<string, TypeDef> rtTypeMap = new Dictionary<string, TypeDef>();
		Dictionary<Tuple<TypeDef, string>, FieldDef> dictionary2 = new Dictionary<Tuple<TypeDef, string>, FieldDef>();
		Dictionary<ExternalCallEntry, MethodDef> dictionary3 = new Dictionary<ExternalCallEntry, MethodDef>();
		foreach (TypeDef type4 in val.GetTypes())
		{
			string key = KitsuneExternalCallRegistry.Normalize(type4.FullName);
			if (!rtTypeMap.ContainsKey(key))
			{
				rtTypeMap[key] = type4;
			}
			foreach (FieldDef field in type4.Fields)
			{
				Tuple<TypeDef, string> key2 = Tuple.Create<TypeDef, string>(type4, UTF8String.op_Implicit(field.Name));
				if (!dictionary2.ContainsKey(key2))
				{
					dictionary2[key2] = field;
				}
			}
		}
		foreach (ExternalCallEntry allEntry in kitsuneExternalCallRegistry.GetAllEntries())
		{
			if (!rtTypeMap.TryGetValue(allEntry.TypeName, out var value))
			{
				continue;
			}
			MethodDef val3 = null;
			foreach (MethodDef method2 in value.Methods)
			{
				if (!(UTF8String.op_Implicit(method2.Name) != allEntry.MethodName))
				{
					if (method2.MethodSig != null && ((MethodBaseSig)method2.MethodSig).Params.Count == allEntry.ParamTypeNames.Length)
					{
						val3 = method2;
						break;
					}
					if (val3 == null)
					{
						val3 = method2;
					}
				}
			}
			if (val3 != null)
			{
				dictionary3[allEntry] = val3;
			}
		}
		Console.WriteLine("       [Rename-prep] tracked types=" + rtTypeMap.Count + " fields=" + dictionary2.Count + " ext-methods=" + dictionary3.Count);
		KitsuneObfuscator.Apply(val, obfOpts);
		HashSet<TypeDef> hashSet2 = new HashSet<TypeDef>();
		foreach (TypeDef type5 in val.GetTypes())
		{
			if (!hashSet.Contains(type5))
			{
				hashSet2.Add(type5);
			}
		}
		HashSet<TypeDef> hashSet3 = new HashSet<TypeDef>();
		foreach (TypeDef type6 in val.GetTypes())
		{
			if (!hashSet2.Contains(type6))
			{
				hashSet3.Add(type6);
			}
		}
		HashSet<MethodDef> hashSet4 = new HashSet<MethodDef>(obfOpts.ExcludedMethods);
		foreach (TypeDef item3 in hashSet2)
		{
			foreach (MethodDef method3 in item3.Methods)
			{
				if (method3.HasBody && method3.Body.HasExceptionHandlers)
				{
					hashSet4.Add(method3);
				}
			}
		}
		if (obfOpts.StringEncryption && obfOpts.DecStrMethod != null)
		{
			Console.WriteLine("Step 5.6: Stub string encryption...");
			HashSet<TypeDef> hashSet5 = new HashSet<TypeDef>(hashSet2);
			hashSet5.Remove(obfOpts.DecStrMethod.DeclaringType);
			StringEncryption.EncryptTypes(val, obfOpts.DecStrMethod, hashSet5, hashSet4);
			Console.WriteLine("       Encrypted strings in " + hashSet5.Count + " stub types.");
		}
		if (obfOpts.IntProtection)
		{
			Console.WriteLine("Step 5.7: Stub IntProtection...");
			try
			{
				XorInt.Execute(val, hashSet3, hashSet4);
			}
			catch (Exception ex5)
			{
				Console.WriteLine("       [WARN] " + ex5.Message);
			}
		}
		if (obfOpts.Mutation)
		{
			Console.WriteLine("Step 5.9: Stub Mutation...");
			try
			{
				Mutation.Execute(val, hashSet3, hashSet4);
			}
			catch (Exception ex6)
			{
				Console.WriteLine("       [WARN] " + ex6.Message);
			}
		}
		if (obfOpts.InstructionSubstitution)
		{
			Console.WriteLine("Step 5.10: Stub InstructionSubstitution...");
			try
			{
				InstructionSubstitution.Execute(val, hashSet3, hashSet4);
			}
			catch (Exception ex7)
			{
				Console.WriteLine("       [WARN] " + ex7.Message);
			}
		}
		if (obfOpts.FakeAttributes)
		{
			Console.WriteLine("Step 5.13: Stub FakeAttributes...");
			try
			{
				FakeAttributes.Execute(val, hashSet3);
			}
			catch (Exception ex8)
			{
				Console.WriteLine("       [WARN] " + ex8.Message);
			}
		}
		if (obfOpts.InvalidMethods)
		{
			Console.WriteLine("Step 5.15: Stub InvalidMethods...");
			try
			{
				InvalidMethods.Execute(val, null, hashSet2);
			}
			catch (Exception ex9)
			{
				Console.WriteLine("       [WARN] " + ex9.Message);
			}
		}
		if (obfOpts.Rename)
		{
			Console.WriteLine("Step 5.16: Stub Rename...");
			try
			{
				Renamer.Execute(val, hashSet3);
			}
			catch (Exception ex10)
			{
				Console.WriteLine("       [WARN] " + ex10.Message);
			}
		}
		Func<string, string> applyRenameType = null;
		applyRenameType = (string clrName) =>
		{
			if (string.IsNullOrEmpty(clrName))
			{
				return clrName;
			}
			if (clrName.EndsWith("&"))
			{
				return applyRenameType(clrName.Substring(0, clrName.Length - 1)) + "&";
			}
			if (clrName.EndsWith("*"))
			{
				return applyRenameType(clrName.Substring(0, clrName.Length - 1)) + "*";
			}
			if (clrName.EndsWith("[]"))
			{
				return applyRenameType(clrName.Substring(0, clrName.Length - 2)) + "[]";
			}
			if (rtTypeMap.TryGetValue(clrName, out var value5))
			{
				return KitsuneExternalCallRegistry.Normalize(value5.FullName);
			}
			int num8 = clrName.IndexOf('[');
			if (num8 > 0 && clrName.EndsWith("]"))
			{
				string text5 = clrName.Substring(0, num8);
				string text6 = clrName.Substring(num8 + 1, clrName.Length - num8 - 2);
				List<string> list4 = new List<string>();
				int num9 = 0;
				int num10 = 0;
				for (int j = 0; j < text6.Length; j++)
				{
					if (text6[j] == '[')
					{
						num9++;
					}
					else if (text6[j] == ']')
					{
						num9--;
					}
					else if (text6[j] == ',' && num9 == 0)
					{
						list4.Add(text6.Substring(num10, j - num10).Trim());
						num10 = j + 1;
					}
				}
				list4.Add(text6.Substring(num10).Trim());
				string text7 = string.Join(",", list4.ConvertAll((string a) => applyRenameType(a)));
				return text5 + "[" + text7 + "]";
			}
			return clrName;
		};
		foreach (ExternalCallEntry allEntry2 in kitsuneExternalCallRegistry.GetAllEntries())
		{
			allEntry2.TypeName = applyRenameType(allEntry2.TypeName);
			if (dictionary3.TryGetValue(allEntry2, out var value2))
			{
				allEntry2.MethodName = UTF8String.op_Implicit(value2.Name);
			}
			for (int num = 0; num < allEntry2.ParamTypeNames.Length; num++)
			{
				allEntry2.ParamTypeNames[num] = applyRenameType(allEntry2.ParamTypeNames[num]);
			}
			if (allEntry2.MethodGenericArgs != null)
			{
				for (int num2 = 0; num2 < allEntry2.MethodGenericArgs.Length; num2++)
				{
					allEntry2.MethodGenericArgs[num2] = applyRenameType(allEntry2.MethodGenericArgs[num2]);
				}
			}
		}
		foreach (KitsuneFieldRegistry.FieldEntry allEntry3 in kitsuneFieldRegistry.GetAllEntries())
		{
			string typeName = allEntry3.TypeName;
			string fieldName = allEntry3.FieldName;
			if (rtTypeMap.TryGetValue(typeName, out var value3))
			{
				allEntry3.TypeName = KitsuneExternalCallRegistry.Normalize(value3.FullName);
				if (dictionary2.TryGetValue(Tuple.Create<TypeDef, string>(value3, fieldName), out var value4))
				{
					allEntry3.FieldName = UTF8String.op_Implicit(value4.Name);
				}
			}
		}
		foreach (KitsuneTypeRegistry.TypeEntry allEntry4 in kitsuneTypeRegistry.GetAllEntries())
		{
			allEntry4.TypeName = applyRenameType(allEntry4.TypeName);
			if (allEntry4.CtorArgTypeNames != null)
			{
				for (int num3 = 0; num3 < allEntry4.CtorArgTypeNames.Length; num3++)
				{
					allEntry4.CtorArgTypeNames[num3] = applyRenameType(allEntry4.CtorArgTypeNames[num3]);
				}
			}
		}
		Console.WriteLine("       [Obf] All registry names refreshed after rename.");
		Console.WriteLine("       [DEBUG] External call registry dump:");
		foreach (ExternalCallEntry allEntry5 in kitsuneExternalCallRegistry.GetAllEntries())
		{
			Console.WriteLine("         id=" + allEntry5.Id + " type=" + allEntry5.TypeName + " method=" + allEntry5.MethodName);
		}
		kitsuneVmInjector.PrepareEncryptedSections(list2, kitsuneExternalCallRegistry, stringRegistry, kitsuneFieldRegistry, kitsuneTypeRegistry, array, protectionFlags, array2, out var indexPacket, out var dataPackets);
		Console.WriteLine("Step 6: Saving " + Path.GetFileName(outputPath) + "...");
		HashSet<TypeDef> hashSet6 = new HashSet<TypeDef>();
		foreach (TypeDef type7 in val.GetTypes())
		{
			if (!hashSet.Contains(type7))
			{
				hashSet6.Add(type7);
			}
		}
		foreach (TypeDef item4 in hashSet6)
		{
			foreach (MethodDef method4 in item4.Methods)
			{
				if (method4.HasBody && method4.Body.MaxStack < 128)
				{
					method4.Body.MaxStack = 128;
				}
			}
		}
		foreach (TypeDef type8 in val.GetTypes())
		{
			foreach (MethodDef method5 in type8.Methods)
			{
				if (method5.HasBody)
				{
					method5.Body.SimplifyBranches();
					method5.Body.OptimizeBranches();
				}
			}
		}
		ModuleWriterOptions val4 = new ModuleWriterOptions(val);
		((ModuleWriterOptionsBase)val4).WritePdb = false;
		MetadataOptions metadataOptions = ((ModuleWriterOptionsBase)val4).MetadataOptions;
		metadataOptions.Flags = (MetadataFlags)(metadataOptions.Flags | 0x8000);
		val.Write(outputPath, val4);
		Console.WriteLine("Step 7: Adding encrypted PE sections...");
		KitsunePEPatcher.Patch(outputPath, indexPacket, dataPackets);
		ModuleDefMD val5 = ModuleDefMD.Load(outputPath, (ModuleCreationOptions)null);
		bool flag = false;
		HashSet<uint> hashSet7 = new HashSet<uint>();
		for (uint num4 = 1u; num4 <= val5.Metadata.TablesStream.TypeRefTable.Rows; num4++)
		{
			TypeRef val6 = val5.ResolveTypeRef(num4);
			if (val6 != null)
			{
				IResolutionScope resolutionScope = val6.ResolutionScope;
				while (resolutionScope is TypeRef)
				{
					resolutionScope = ((TypeRef)resolutionScope).ResolutionScope;
				}
				AssemblyRef val7 = (AssemblyRef)(object)((resolutionScope is AssemblyRef) ? resolutionScope : null);
				if (val7 != null && val7.Name == "Kitsune_VM_Stub")
				{
					Console.WriteLine("[WARN-TypeRef] RID=" + num4 + " " + val6.FullName);
					hashSet7.Add(num4);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Console.WriteLine("[Written-OK] No TypeRefs to Kitsune_VM_Stub");
		}
		else
		{
			for (uint num5 = 1u; num5 <= val5.Metadata.TablesStream.MemberRefTable.Rows; num5++)
			{
				MemberRef val8 = val5.ResolveMemberRef(num5);
				if (val8 == null)
				{
					continue;
				}
				IMemberRefParent val9 = val8.Class;
				TypeRef val10 = (TypeRef)(object)((val9 is TypeRef) ? val9 : null);
				if (val10 != null)
				{
					IResolutionScope resolutionScope2 = val10.ResolutionScope;
					while (resolutionScope2 is TypeRef)
					{
						resolutionScope2 = ((TypeRef)resolutionScope2).ResolutionScope;
					}
					AssemblyRef val11 = (AssemblyRef)(object)((resolutionScope2 is AssemblyRef) ? resolutionScope2 : null);
					if (val11 != null && val11.Name == "Kitsune_VM_Stub")
					{
						Console.WriteLine("[WARN-MemberRef] " + val10.FullName + "::" + UTF8String.op_Implicit(val8.Name));
					}
				}
			}
			for (uint num6 = 1u; num6 <= val5.Metadata.TablesStream.TypeSpecTable.Rows; num6++)
			{
				TypeSpec val12 = val5.ResolveTypeSpec(num6);
				if (val12 != null)
				{
					string text3 = ((val12.TypeSig != null) ? val12.TypeSig.FullName : "null");
					if (text3.Contains("Kitsune_VM_Stub"))
					{
						Console.WriteLine("[WARN-TypeSpec] RID=" + num6 + " " + text3);
					}
				}
			}
			for (uint num7 = 1u; num7 <= val5.Metadata.TablesStream.StandAloneSigTable.Rows; num7++)
			{
				StandAloneSig val13 = val5.ResolveStandAloneSig(num7);
				if (val13 != null && val13.Signature != null)
				{
					string text4 = ((object)val13.Signature).ToString();
					if (text4 != null && text4.Contains("Kitsune_VM_Stub"))
					{
						Console.WriteLine("[WARN-StandAloneSig] RID=" + num7 + " " + text4);
					}
				}
			}
		}
		((ModuleDef)val5).Dispose();
		long length = new FileInfo(targetPath).Length;
		long length2 = new FileInfo(outputPath).Length;
		Console.WriteLine($"       Size: {length / 1024} KB → {length2 / 1024} KB");
	}

	private static bool ShouldSkipType(TypeDef type)
	{
		if (type.Name == "<Module>")
		{
			return true;
		}
		if (type.Name.StartsWith("<>"))
		{
			return true;
		}
		if (type.Name.Contains("__DisplayClass"))
		{
			return true;
		}
		if (type.Name.Contains("__c"))
		{
			return true;
		}
		return false;
	}

	private static bool ShouldVirtualize(MethodDef method)
	{
		if (!method.HasBody)
		{
			return false;
		}
		if (method.Body.Instructions.Count == 0)
		{
			return false;
		}
		if (method.IsStaticConstructor)
		{
			return false;
		}
		if (method.IsConstructor)
		{
			return false;
		}
		if (method.Body.Instructions.Count <= 1)
		{
			return false;
		}
		if (method.HasGenericParameters)
		{
			return false;
		}
		return true;
	}
}
