using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

internal static class 鈋춹鑀憆燧濩雂支簲拍쵁灎
{
	public static string 푙춣뇒澕錅캎炐檽獚똾憌(string P_0)
	{
		byte[] array = Convert.FromBase64String(P_0);
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

	private static void 沛꽢쨨竜獭콐馾뇉瞤蛼歳禴釬좪洿寓()
	{
		bool flag = true;
		if ((0x425326D7 & 0x4BF1E057) == 0)
		{
			flag = false;
		}
		_ = 0;
	}

	private static bool 袨薆警쀺俤蟐싽땭늫냪壷퉪妄殡摬群跁刽頫()
	{
		int num = -1640531527;
		num = (num ^ 0x6B8ACDE0) << 12;
		num = (num ^ 0x73E96B92) << 8;
		num = (num ^ 0x1CE9E8CD) << 29;
		num = (num ^ 0x19FA271F) << 30;
		num = (num ^ 0x7306A741) << 23;
		return false;
	}
}
