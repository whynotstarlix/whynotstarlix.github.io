using System.Collections.Generic;
using dnlib.DotNet;

namespace Kitsune_VM.Obfuscation;

public class ObfuscatorOptions
{
	public HashSet<TypeDef> ExcludedTypes = new HashSet<TypeDef>();

	public HashSet<TypeDef> ExcludedFromRename = new HashSet<TypeDef>();

	public HashSet<MethodDef> ExcludedMethods = new HashSet<MethodDef>();

	public HashSet<TypeDef> NoRenameTypes = new HashSet<TypeDef>();

	public Dictionary<TypeDef, HashSet<string>> NoRenameMembers = new Dictionary<TypeDef, HashSet<string>>();

	public bool ResourceEncryption = true;

	public bool Rename = true;

	public bool ControlFlow = true;

	public bool StringEncryption = true;

	public bool IntProtection = true;

	public bool ConstantFolding = true;

	public bool Mutation = true;

	public bool ProxyCall = true;

	public bool ProxyMethods = true;

	public bool ProxyString = true;

	public bool Junk = true;

	public bool LocalVariableSplit = true;

	public bool FakeAttributes = true;

	public bool DecompilerTrap = true;

	public bool InstructionSubstitution = true;

	public bool InvalidOpcodes = true;

	public bool InvalidMethods = true;

	public bool MorphDispatcher = true;

	public bool ProxyInt = true;

	public bool ProxyArithmetic = true;

	public HashSet<TypeDef> StringEncryptAlsoTypes = new HashSet<TypeDef>();

	public MethodDef DecStrMethod;

	public string DecStrKey;
}
