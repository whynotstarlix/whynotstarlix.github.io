using System;
using System.IO;
using System.Security.Cryptography;

namespace Kitsune_VM;

internal static class KitsuneVmCrypto
{
	private static readonly RNGCryptoServiceProvider _crng = new RNGCryptoServiceProvider();

	private static readonly Random _rnd = new Random();

	private static readonly uint[] _crcTab = BuildCrcTable();

	internal static byte[] GenerateBootstrapKey()
	{
		byte[] array = new byte[16 + _rnd.Next(49)];
		_crng.GetBytes(array);
		return array;
	}

	internal static byte[] RandomBytes(int n)
	{
		byte[] array = new byte[n];
		_crng.GetBytes(array);
		return array;
	}

	internal static int RandomInt(int min, int max)
	{
		return _rnd.Next(min, max);
	}

	internal static byte[] GenerateOpcodePerm()
	{
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			array[i] = (byte)i;
		}
		for (int num = 255; num > 0; num--)
		{
			int num2 = _rnd.Next(num + 1);
			byte b = array[num];
			array[num] = array[num2];
			array[num2] = b;
		}
		return array;
	}

	private static uint[] BuildCrcTable()
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
		return array;
	}

	internal static uint Crc32(byte[] data, int offset, int length)
	{
		uint num = uint.MaxValue;
		for (int i = offset; i < offset + length; i++)
		{
			num = _crcTab[(num ^ data[i]) & 0xFF] ^ (num >> 8);
		}
		return num ^ 0xFFFFFFFFu;
	}

	private static byte[] DeriveKey(byte[] master, byte[] salt, int chunkIndex)
	{
		using SHA256 sHA = SHA256.Create();
		byte[] array = new byte[master.Length + salt.Length + 4];
		Buffer.BlockCopy(master, 0, array, 0, master.Length);
		Buffer.BlockCopy(salt, 0, array, master.Length, salt.Length);
		array[master.Length + salt.Length] = (byte)chunkIndex;
		array[master.Length + salt.Length + 1] = (byte)(chunkIndex >> 8);
		array[master.Length + salt.Length + 2] = (byte)(chunkIndex >> 16);
		array[master.Length + salt.Length + 3] = (byte)(chunkIndex >> 24);
		return sHA.ComputeHash(array);
	}

	private static byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
	{
		using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		aesCryptoServiceProvider.KeySize = 256;
		aesCryptoServiceProvider.BlockSize = 128;
		aesCryptoServiceProvider.Mode = CipherMode.CBC;
		aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		aesCryptoServiceProvider.Key = key;
		aesCryptoServiceProvider.IV = iv;
		using ICryptoTransform transform = aesCryptoServiceProvider.CreateEncryptor();
		using MemoryStream memoryStream = new MemoryStream();
		using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
		{
			cryptoStream.Write(data, 0, data.Length);
		}
		return memoryStream.ToArray();
	}

	private static byte[] XorLayer(byte[] data, byte[] salt)
	{
		byte b = 0;
		foreach (byte b2 in salt)
		{
			b ^= b2;
		}
		byte[] array = new byte[data.Length];
		for (int j = 0; j < data.Length; j++)
		{
			array[j] = (byte)(data[j] ^ b);
		}
		return array;
	}

	private static byte[] CaesarEncrypt(byte[] data, byte[] salt)
	{
		int num = 0;
		foreach (byte b in salt)
		{
			num += b;
		}
		byte b2 = (byte)(num & 0xFF);
		byte[] array = new byte[data.Length];
		for (int j = 0; j < data.Length; j++)
		{
			array[j] = (byte)(data[j] + b2);
		}
		return array;
	}

	internal static byte[] Encrypt(byte[] plain, byte[] masterKey, int chunkIndex = 0)
	{
		byte[] array = RandomBytes(8);
		byte[] array2 = RandomBytes(16);
		uint value = Crc32(plain, 0, plain.Length);
		byte[] key = DeriveKey(masterKey, array, chunkIndex);
		byte[] buffer = CaesarEncrypt(XorLayer(AesEncrypt(plain, key, array2), array), array);
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(array);
		binaryWriter.Write(array2);
		binaryWriter.Write(value);
		binaryWriter.Write(plain.Length);
		binaryWriter.Write(buffer);
		return memoryStream.ToArray();
	}
}
