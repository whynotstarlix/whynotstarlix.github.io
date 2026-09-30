using System;
using System.IO;
using System.Security.Cryptography;

namespace Kitsune_VM_Stub;

internal static class KitsuneCrypto
{
	private static readonly uint[] _crcTab = BuildCrcTable();

	internal static byte[] BootstrapKey => BuildBootstrapKey();

	internal static byte[] BuildBootstrapKey()
	{
		return new byte[32]
		{
			126, 74, 156, 47, 179, 97, 213, 136, 242, 14,
			71, 161, 109, 59, 201, 84, 25, 117, 232, 44,
			148, 15, 91, 215, 131, 26, 110, 240, 39, 76,
			157, 56
		};
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

	private static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
	{
		using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		aesCryptoServiceProvider.KeySize = 256;
		aesCryptoServiceProvider.BlockSize = 128;
		aesCryptoServiceProvider.Mode = CipherMode.CBC;
		aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		aesCryptoServiceProvider.Key = key;
		aesCryptoServiceProvider.IV = iv;
		using ICryptoTransform transform = aesCryptoServiceProvider.CreateDecryptor();
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

	private static byte[] CaesarDecrypt(byte[] data, byte[] salt)
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
			array[j] = (byte)(data[j] - b2);
		}
		return array;
	}

	internal static byte[] Decrypt(byte[] packet, byte[] masterKey, int chunkIndex = 0)
	{
		using MemoryStream memoryStream = new MemoryStream(packet);
		using BinaryReader binaryReader = new BinaryReader(memoryStream);
		byte[] salt = binaryReader.ReadBytes(8);
		byte[] iv = binaryReader.ReadBytes(16);
		uint num = binaryReader.ReadUInt32();
		int num2 = binaryReader.ReadInt32();
		int count = (int)(memoryStream.Length - memoryStream.Position);
		byte[] data = XorLayer(CaesarDecrypt(binaryReader.ReadBytes(count), salt), salt);
		byte[] key = DeriveKey(masterKey, salt, chunkIndex);
		byte[] array = AesDecrypt(data, key, iv);
		if (array.Length != num2)
		{
			throw new Exception($"KitsuneCrypto: expected {num2} bytes, got {array.Length}");
		}
		if (Crc32(array, 0, array.Length) != num)
		{
			throw new Exception("KitsuneCrypto: CRC32 mismatch — data is corrupted.");
		}
		return array;
	}

	internal static byte[] BuildReversePerm(byte[] perm)
	{
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			array[perm[i]] = (byte)i;
		}
		return array;
	}
}
