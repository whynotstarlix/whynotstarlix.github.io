using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Kitsune_VM.Obfuscation.Resources.Runtime;

internal static class ResourceDecryptHelper
{
	private static Dictionary<string, byte[]> _cache;

	public static Stream DecRes(Assembly asm, string name)
	{
		if (_cache == null)
		{
			_cache = new Dictionary<string, byte[]>();
		}
		if (_cache.TryGetValue(name, out var value))
		{
			return new MemoryStream(value);
		}
		Stream manifestResourceStream = asm.GetManifestResourceStream(name);
		if (manifestResourceStream == null)
		{
			return null;
		}
		byte[] array = new byte[(int)manifestResourceStream.Length];
		manifestResourceStream.Read(array, 0, array.Length);
		manifestResourceStream.Close();
		if (array.Length < 4 || array[0] != 75 || array[1] != 69 || array[2] != 78 || array[3] != 67)
		{
			return new MemoryStream(array);
		}
		byte[] array2 = Decrypt(array);
		_cache[name] = array2;
		return new MemoryStream(array2);
	}

	private static byte[] Decrypt(byte[] data)
	{
		byte[] array = new byte[16];
		Array.Copy(data, 4, array, 0, 16);
		int num = data.Length - 20;
		byte[] array2 = new byte[num];
		Array.Copy(data, 20, array2, 0, num);
		byte[] array3 = new byte[16];
		byte[] array4 = new byte[16];
		for (int i = 0; i < 16; i++)
		{
			array3[i] = (byte)(array[i] ^ array[(i + 7) % 16]);
			array4[i] = (byte)(array[(i + 3) % 16] ^ 0x5A);
		}
		using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
		{
			rijndaelManaged.Key = array3;
			rijndaelManaged.IV = array4;
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			using MemoryStream stream = new MemoryStream(array2);
			using CryptoStream cryptoStream = new CryptoStream(stream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read);
			MemoryStream memoryStream = new MemoryStream();
			byte[] array5 = new byte[4096];
			int count;
			while ((count = cryptoStream.Read(array5, 0, array5.Length)) > 0)
			{
				memoryStream.Write(array5, 0, count);
			}
			array2 = memoryStream.ToArray();
		}
		byte b = array[0];
		for (int j = 0; j < array2.Length; j++)
		{
			byte b2 = array2[j];
			array2[j] = (byte)(b2 ^ b ^ array[j % 16]);
			b = b2;
		}
		for (int k = 0; k < array2.Length; k++)
		{
			int num2 = array[k % 16] & 7;
			if (num2 != 0)
			{
				array2[k] = (byte)((array2[k] >> num2) | (array2[k] << 8 - num2));
			}
		}
		return array2;
	}
}
