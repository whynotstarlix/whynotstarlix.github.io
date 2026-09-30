using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Kitsune_VM.Obfuscation.Helper;
using Kitsune_VM.Obfuscation.Resources.Runtime;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.IO;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.Resources;

internal static class ResourceEncryption
{
	private static readonly Random _rnd = new Random();

	private static readonly HashSet<string> _injectedNames = new HashSet<string> { "DecRes", "Decrypt" };

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null, ObfuscatorOptions obfOpts = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected Obj, but got Unknown
		if (((IEnumerable)module.Resources).OfType<EmbeddedResource>().ToList().Count == 0)
		{
			return;
		}
		MethodDef operand = InjectDecRes(module, obfOpts);
		for (int i = 0; i < ((LazyList<Resource>)(object)module.Resources).Count; i++)
		{
			Resource val = ((LazyList<Resource>)(object)module.Resources)[i];
			EmbeddedResource val2 = (EmbeddedResource)(object)((val is EmbeddedResource) ? val : null);
			if (val2 != null)
			{
				DataReader val3 = val2.CreateReader();
				byte[] array = val3.ReadRemainingBytes();
				if (array.Length < 4 || array[0] != 75 || array[1] != 69 || array[2] != 78 || array[3] != 67)
				{
					byte[] array2 = Encrypt(array);
					((LazyList<Resource>)(object)module.Resources)[i] = (Resource)new EmbeddedResource(((Resource)val2).Name, array2, ((Resource)val2).Attributes);
				}
			}
		}
		foreach (TypeDef type in module.GetTypes())
		{
			if (excluded != null && excluded.Contains(type))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (!method.HasBody || _injectedNames.Contains(UTF8String.op_Implicit(method.Name)) || (excludedMethods != null && excludedMethods.Contains(method)))
				{
					continue;
				}
				bool flag = false;
				IList<Instruction> instructions = method.Body.Instructions;
				for (int j = 0; j < instructions.Count; j++)
				{
					if (instructions[j].OpCode != OpCodes.Callvirt && instructions[j].OpCode != OpCodes.Call)
					{
						continue;
					}
					object operand2 = instructions[j].Operand;
					IMethod val4 = (IMethod)((operand2 is IMethod) ? operand2 : null);
					if (val4 != null && !(((IFullName)val4).Name != "GetManifestResourceStream"))
					{
						MethodSig methodSig = val4.MethodSig;
						if (methodSig != null && ((MethodBaseSig)methodSig).Params.Count == 1 && !(((MethodBaseSig)methodSig).Params[0].FullName != "System.String"))
						{
							instructions[j].OpCode = OpCodes.Call;
							instructions[j].Operand = operand;
							flag = true;
						}
					}
				}
				if (flag)
				{
					try
					{
						method.Body.SimplifyBranches();
						method.Body.OptimizeBranches();
					}
					catch
					{
					}
				}
			}
		}
	}

	private static MethodDef InjectDecRes(ModuleDef module, ObfuscatorOptions obfOpts)
	{
		TypeDef val = InjectHelper.Inject(ModuleDefMD.Load(typeof(ResourceDecryptHelper).Module).ResolveTypeDef(MDToken.ToRID(typeof(ResourceDecryptHelper).MetadataToken)), module);
		val.Namespace = UTF8String.op_Implicit("");
		module.Types.Add(val);
		obfOpts?.ExcludedTypes.Add(val);
		return val.Methods.First((MethodDef m) => m.Name == "DecRes");
	}

	private static byte[] Encrypt(byte[] plain)
	{
		byte[] array = new byte[16];
		_rnd.NextBytes(array);
		byte[] array2 = (byte[])plain.Clone();
		for (int i = 0; i < array2.Length; i++)
		{
			int num = array[i % 16] & 7;
			if (num != 0)
			{
				array2[i] = (byte)((array2[i] << num) | (array2[i] >> 8 - num));
			}
		}
		byte b = array[0];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = (byte)(array2[j] ^ b ^ array[j % 16]);
			b = array2[j];
		}
		byte[] array3 = new byte[16];
		byte[] array4 = new byte[16];
		for (int k = 0; k < 16; k++)
		{
			array3[k] = (byte)(array[k] ^ array[(k + 7) % 16]);
			array4[k] = (byte)(array[(k + 3) % 16] ^ 0x5A);
		}
		using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
		{
			rijndaelManaged.Key = array3;
			rijndaelManaged.IV = array4;
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			using ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
			array2 = cryptoTransform.TransformFinalBlock(array2, 0, array2.Length);
		}
		byte[] array5 = new byte[20 + array2.Length];
		array5[0] = 75;
		array5[1] = 69;
		array5[2] = 78;
		array5[3] = 67;
		Array.Copy(array, 0, array5, 4, 16);
		Array.Copy(array2, 0, array5, 20, array2.Length);
		return array5;
	}
}
