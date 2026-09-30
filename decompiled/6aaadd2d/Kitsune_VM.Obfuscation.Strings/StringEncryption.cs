using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Kitsune_VM.Obfuscation.Helper;
using Kitsune_VM.Obfuscation.Strings.Runtime;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Kitsune_VM.Obfuscation.Strings;

internal class StringEncryption
{
	private static readonly string[] Whitelist = new string[2] { "DecStr", "DecRes" };

	private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

	private static byte[] RngBytes(int count)
	{
		byte[] array = new byte[count];
		_rng.GetBytes(array);
		return array;
	}

	private static uint Crc32(byte[] data)
	{
		uint[] array = new uint[256];
		for (uint num = 0u; num < 256; num++)
		{
			uint num2 = num;
			for (int i = 0; i < 8; i++)
			{
				num2 = (((num2 & 1) != 0) ? (0xEDB88320u ^ (num2 >> 1)) : (num2 >> 1));
			}
			array[num] = num2;
		}
		uint num3 = uint.MaxValue;
		foreach (byte b in data)
		{
			num3 = array[(num3 ^ b) & 0xFF] ^ (num3 >> 8);
		}
		return num3 ^ 0xFFFFFFFFu;
	}

	public static string EncStr(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		byte[] sourceArray = RngBytes(8);
		byte b = RngBytes(1)[0];
		byte[] array = RngBytes(4);
		byte[] array2 = RngBytes(16);
		byte[] array3 = RngBytes(32);
		byte[] array4 = new byte[bytes.Length];
		for (int i = 0; i < bytes.Length; i++)
		{
			array4[i] = (byte)(bytes[i] + b);
		}
		byte[] array5 = new byte[array4.Length];
		for (int j = 0; j < array4.Length; j++)
		{
			array5[j] = (byte)(array4[j] ^ array[j % 4]);
		}
		byte[] array6;
		using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
		{
			aesCryptoServiceProvider.KeySize = 256;
			aesCryptoServiceProvider.BlockSize = 128;
			aesCryptoServiceProvider.Mode = CipherMode.CBC;
			aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			aesCryptoServiceProvider.Key = array3;
			aesCryptoServiceProvider.IV = array2;
			using ICryptoTransform transform = aesCryptoServiceProvider.CreateEncryptor();
			using MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
			{
				cryptoStream.Write(array5, 0, array5.Length);
			}
			array6 = memoryStream.ToArray();
		}
		uint num = Crc32(bytes);
		byte[] array7 = new byte[65 + array6.Length];
		int num2 = 0;
		Array.Copy(sourceArray, 0, array7, num2, 8);
		num2 += 8;
		array7[num2++] = b;
		Array.Copy(array, 0, array7, num2, 4);
		num2 += 4;
		Array.Copy(array2, 0, array7, num2, 16);
		num2 += 16;
		Array.Copy(array3, 0, array7, num2, 32);
		num2 += 32;
		array7[num2++] = (byte)num;
		array7[num2++] = (byte)(num >> 8);
		array7[num2++] = (byte)(num >> 16);
		array7[num2++] = (byte)(num >> 24);
		Array.Copy(array6, 0, array7, num2, array6.Length);
		return Convert.ToBase64String(array7);
	}

	private static MethodDef InjectDecStr(ModuleDef module, ObfuscatorOptions obfOpts = null)
	{
		TypeDef val = InjectHelper.Inject(ModuleDefMD.Load(typeof(XorStringHelper).Module).ResolveTypeDef(MDToken.ToRID(typeof(XorStringHelper).MetadataToken)), module);
		val.Namespace = UTF8String.op_Implicit("");
		module.Types.Add(val);
		obfOpts?.ExcludedTypes.Add(val);
		return val.Methods.First((MethodDef m) => m.Name == "DecStr");
	}

	public static void EncryptTypes(ModuleDef module, MethodDef dec, HashSet<TypeDef> targetTypes, HashSet<MethodDef> excludedMethods = null)
	{
		if (dec == null || targetTypes == null || targetTypes.Count == 0)
		{
			return;
		}
		foreach (TypeDef targetType in targetTypes)
		{
			foreach (MethodDef method in targetType.Methods)
			{
				if (method.HasBody && !method.IsConstructor && !method.IsStaticConstructor && !Enumerable.Contains(Whitelist, UTF8String.op_Implicit(method.Name)) && (excludedMethods == null || !excludedMethods.Contains(method)) && !method.Body.HasExceptionHandlers)
				{
					method.Body.SimplifyBranches();
					EncryptStringsInMethod(method, dec);
				}
			}
		}
	}

	private static void EncryptStringsInMethod(MethodDef method, MethodDef dec)
	{
		IList<Instruction> instructions = method.Body.Instructions;
		bool flag = false;
		for (int i = 0; i < instructions.Count; i++)
		{
			if (instructions[i].OpCode == OpCodes.Ldstr)
			{
				string text = instructions[i].Operand?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					instructions[i].Operand = EncStr(text);
					instructions.Insert(i + 1, Instruction.Create(OpCodes.Call, (IMethod)(object)dec));
					i++;
					flag = true;
				}
			}
		}
		if (flag)
		{
			CilBody body = method.Body;
			body.MaxStack += 1;
			method.Body.KeepOldMaxStack = true;
		}
	}

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<MethodDef> excludedMethods = null, HashSet<TypeDef> alsoEncrypt = null, ObfuscatorOptions obfOpts = null)
	{
		MethodDef val = InjectDecStr(module, obfOpts);
		if (obfOpts != null)
		{
			obfOpts.DecStrMethod = val;
			obfOpts.DecStrKey = null;
		}
		foreach (TypeDef type in module.GetTypes())
		{
			if (type.Name == "Resources" || type.Name == "Settings" || ((alsoEncrypt == null || !alsoEncrypt.Contains(type)) && excluded != null && excluded.Contains(type)))
			{
				continue;
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.HasBody && !method.IsConstructor && !method.IsStaticConstructor && !Enumerable.Contains(Whitelist, UTF8String.op_Implicit(method.Name)) && (excludedMethods == null || !excludedMethods.Contains(method)))
				{
					try
					{
						method.Body.SimplifyBranches();
					}
					catch
					{
						continue;
					}
					EncryptStringsInMethod(method, val);
					try
					{
						method.Body.OptimizeBranches();
					}
					catch
					{
					}
				}
			}
		}
	}
}
