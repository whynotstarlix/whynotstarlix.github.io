using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kitsune_VM.Obfuscation.Strings.Runtime;

internal static class XorStringHelper
{
	public static string DecStr(string enc)
	{
		byte[] array = Convert.FromBase64String(enc);
		int num = 0;
		byte[] destinationArray = new byte[8];
		Array.Copy(array, num, destinationArray, 0, 8);
		num += 8;
		byte b = array[num++];
		byte[] array2 = new byte[4];
		Array.Copy(array, num, array2, 0, 4);
		num += 4;
		byte[] array3 = new byte[16];
		Array.Copy(array, num, array3, 0, 16);
		num += 16;
		byte[] array4 = new byte[32];
		Array.Copy(array, num, array4, 0, 32);
		num += 32;
		BitConverter.ToUInt32(array, num);
		num += 4;
		byte[] array5 = new byte[array.Length - num];
		Array.Copy(array, num, array5, 0, array5.Length);
		byte[] array6;
		using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
		{
			aesCryptoServiceProvider.KeySize = 256;
			aesCryptoServiceProvider.BlockSize = 128;
			aesCryptoServiceProvider.Mode = CipherMode.CBC;
			aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			aesCryptoServiceProvider.Key = array4;
			aesCryptoServiceProvider.IV = array3;
			using ICryptoTransform transform = aesCryptoServiceProvider.CreateDecryptor();
			using MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
			{
				cryptoStream.Write(array5, 0, array5.Length);
			}
			array6 = memoryStream.ToArray();
		}
		byte[] array7 = new byte[array6.Length];
		for (int i = 0; i < array6.Length; i++)
		{
			array7[i] = (byte)(array6[i] ^ array2[i % 4]);
		}
		byte[] array8 = new byte[array7.Length];
		for (int j = 0; j < array7.Length; j++)
		{
			array8[j] = (byte)(array7[j] - b);
		}
		return Encoding.UTF8.GetString(array8);
	}
}
