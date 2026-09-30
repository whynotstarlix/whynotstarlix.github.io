using System;
using System.Collections.Generic;
using Kitsune_VM.Obfuscation.ConstantFolding;
using Kitsune_VM.Obfuscation.CtrlFlow;
using Kitsune_VM.Obfuscation.FakeAttributes;
using Kitsune_VM.Obfuscation.InstructionSubstitution;
using Kitsune_VM.Obfuscation.IntProtection;
using Kitsune_VM.Obfuscation.InvalidCode;
using Kitsune_VM.Obfuscation.Junk;
using Kitsune_VM.Obfuscation.LocalVariableSplitting;
using Kitsune_VM.Obfuscation.Mutation;
using Kitsune_VM.Obfuscation.Proxy;
using Kitsune_VM.Obfuscation.Rename;
using Kitsune_VM.Obfuscation.Resources;
using Kitsune_VM.Obfuscation.Strings;
using Kitsune_VM.Obfuscation.Trap;
using dnlib.DotNet;

namespace Kitsune_VM.Obfuscation;

public static class KitsuneObfuscator
{
	public static void Apply(ModuleDef module, ObfuscatorOptions opts = null)
	{
		if (opts == null)
		{
			opts = new ObfuscatorOptions();
		}
		HashSet<TypeDef> excluded = opts.ExcludedTypes ?? new HashSet<TypeDef>();
		HashSet<MethodDef> excludedMethods = opts.ExcludedMethods ?? new HashSet<MethodDef>();
		if (opts.ResourceEncryption)
		{
			Log("  [Obf] ResourceEncryption...");
			try
			{
				ResourceEncryption.Execute(module, excluded, excludedMethods, opts);
			}
			catch (Exception ex)
			{
				LogErr("ResourceEncryption", ex);
			}
		}
		if (opts.StringEncryption)
		{
			Log("  [Obf] StringEncryption...");
			try
			{
				StringEncryption.Execute(module, excluded, excludedMethods, opts.StringEncryptAlsoTypes, opts);
			}
			catch (Exception ex2)
			{
				LogErr("StringEncryption", ex2);
			}
		}
		if (opts.Junk)
		{
			Log("  [Obf] Junk injection...");
			try
			{
				Junks.Execute(module);
			}
			catch (Exception ex3)
			{
				LogErr("Junk", ex3);
			}
		}
		if (opts.IntProtection)
		{
			Log("  [Obf] IntProtection...");
			try
			{
				XorInt.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex4)
			{
				LogErr("IntProtection", ex4);
			}
		}
		if (opts.ConstantFolding)
		{
			Log("  [Obf] ConstantFoldingPrevention...");
			try
			{
				ConstantFoldingPrevention.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex5)
			{
				LogErr("ConstantFolding", ex5);
			}
		}
		if (opts.Mutation)
		{
			Log("  [Obf] Mutation...");
			try
			{
				Kitsune_VM.Obfuscation.Mutation.Mutation.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex6)
			{
				LogErr("Mutation", ex6);
			}
		}
		if (opts.LocalVariableSplit)
		{
			Log("  [Obf] LocalVariableSplitting...");
			try
			{
				Kitsune_VM.Obfuscation.LocalVariableSplitting.LocalVariableSplitting.Execute(module, excluded, null, excludedMethods);
			}
			catch (Exception ex7)
			{
				LogErr("LocalVariableSplit", ex7);
			}
		}
		if (opts.ControlFlow)
		{
			Log("  [Obf] ControlFlow...");
			try
			{
				ControlFlowObfuscation.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex8)
			{
				LogErr("ControlFlow", ex8);
			}
		}
		if (opts.ProxyString)
		{
			Log("  [Obf] ProxyString...");
			try
			{
				ProxyString.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex9)
			{
				LogErr("ProxyString", ex9);
			}
		}
		if (opts.ProxyMethods)
		{
			Log("  [Obf] ProxyMethods...");
			try
			{
				ProxyMethods.Execute(module, excluded, null, excludedMethods);
			}
			catch (Exception ex10)
			{
				LogErr("ProxyMethods", ex10);
			}
		}
		if (opts.ProxyCall)
		{
			Log("  [Obf] ProxyCall...");
			try
			{
				ProxyCall.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex11)
			{
				LogErr("ProxyCall", ex11);
			}
		}
		if (opts.ProxyInt)
		{
			Log("  [Obf] ProxyInt...");
			try
			{
				ProxyInt.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex12)
			{
				LogErr("ProxyInt", ex12);
			}
		}
		if (opts.ProxyArithmetic)
		{
			Log("  [Obf] ProxyArithmetic...");
			try
			{
				ProxyArithmetic.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex13)
			{
				LogErr("ProxyArithmetic", ex13);
			}
		}
		if (opts.FakeAttributes)
		{
			Log("  [Obf] FakeAttributes...");
			try
			{
				Kitsune_VM.Obfuscation.FakeAttributes.FakeAttributes.Execute(module, excluded);
			}
			catch (Exception ex14)
			{
				LogErr("FakeAttributes", ex14);
			}
		}
		if (opts.Rename)
		{
			Log("  [Obf] Rename...");
			try
			{
				Renamer.Execute(module, opts.ExcludedFromRename, opts.NoRenameTypes, opts.NoRenameMembers);
			}
			catch (Exception ex15)
			{
				LogErr("Rename", ex15);
			}
		}
		if (opts.DecompilerTrap)
		{
			Log("  [Obf] DecompilerTrap...");
			try
			{
				DecompilerTrap.Execute(module);
			}
			catch (Exception ex16)
			{
				LogErr("DecompilerTrap", ex16);
			}
		}
		if (opts.InstructionSubstitution)
		{
			Log("  [Obf] InstructionSubstitution...");
			try
			{
				Kitsune_VM.Obfuscation.InstructionSubstitution.InstructionSubstitution.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex17)
			{
				LogErr("InstructionSubstitution", ex17);
			}
		}
		if (opts.InvalidOpcodes)
		{
			Log("  [Obf] InvalidOpcodes...");
			try
			{
				InvalidOpcodes.Execute(module, excluded, excludedMethods);
			}
			catch (Exception ex18)
			{
				LogErr("InvalidOpcodes", ex18);
			}
		}
		if (opts.InvalidMethods)
		{
			Log("  [Obf] InvalidMethods...");
			try
			{
				InvalidMethods.Execute(module, excluded);
			}
			catch (Exception ex19)
			{
				LogErr("InvalidMethods", ex19);
			}
		}
	}

	private static void Log(string msg)
	{
		Console.WriteLine(msg);
	}

	private static void LogErr(string pass, Exception ex)
	{
		Console.WriteLine("  [Obf][WARN] " + pass + " failed: " + ex.Message);
	}
}
