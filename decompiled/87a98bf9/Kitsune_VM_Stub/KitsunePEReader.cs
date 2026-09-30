using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Kitsune_VM_Stub;

internal static class KitsunePEReader
{
	private const uint INDEX_MAGIC = 1263817577u;

	private const uint DATA_MAGIC = 1263817572u;

	internal static void ReadIndex(out byte[] reversePerm, out byte[] masterKey, out int totalChunks, out int[] methChunkId, out int[] methOffset, out int[] methLength, out byte[][] encryptedByLogical, out byte[] crcExpected, out byte protectionFlags, out byte[] vmCrcExpected)
	{
		string text = Assembly.GetExecutingAssembly().Location;
		if (string.IsNullOrEmpty(text))
		{
			text = Process.GetCurrentProcess().MainModule.FileName;
		}
		byte[] array = File.ReadAllBytes(text);
		int num = BitConverter.ToInt32(array, 60) + 4;
		int num2 = BitConverter.ToUInt16(array, num + 2);
		int num3 = BitConverter.ToUInt16(array, num + 16);
		int num4 = num + 20 + num3;
		byte[] array2 = null;
		int num5 = -1;
		int num6 = 0;
		byte[][] array3 = new byte[num2][];
		for (int i = 0; i < num2; i++)
		{
			int num7 = num4 + i * 40;
			uint num8 = BitConverter.ToUInt32(array, num7 + 20);
			uint num9 = BitConverter.ToUInt32(array, num7 + 16);
			if (num8 == 0 || num9 < 12)
			{
				continue;
			}
			uint num10 = BitConverter.ToUInt32(array, (int)num8);
			if (num10 != 1263817577 && num10 != 1263817572)
			{
				continue;
			}
			int num11 = BitConverter.ToInt32(array, (int)(num8 + 4));
			int num12 = BitConverter.ToInt32(array, (int)(num8 + 8));
			int num13 = (int)(num8 + 12);
			if (num11 > 0 && num13 + num11 <= array.Length)
			{
				byte[] array4 = new byte[num11];
				Buffer.BlockCopy(array, num13, array4, 0, num11);
				if (num10 == 1263817577)
				{
					array2 = array4;
					num5 = (int)num8;
					num6 = num11;
				}
				else if (num12 >= 0 && num12 < array3.Length)
				{
					array3[num12] = array4;
				}
			}
		}
		vmCrcExpected = new byte[32];
		if (num5 >= 0)
		{
			int num14 = num5 + 12 + num6;
			if (num14 + 32 <= array.Length)
			{
				Buffer.BlockCopy(array, num14, vmCrcExpected, 0, 32);
			}
		}
		if (array2 == null)
		{
			throw new Exception("KitsunePEReader: index section not found in PE.");
		}
		byte[] array5 = KitsuneCrypto.Decrypt(array2, KitsuneCrypto.BootstrapKey);
		int num15 = 0;
		totalChunks = BitConverter.ToInt32(array5, num15);
		num15 += 4;
		byte[] array6 = new byte[256];
		Buffer.BlockCopy(array5, num15, array6, 0, 256);
		num15 += 256;
		reversePerm = KitsuneCrypto.BuildReversePerm(array6);
		masterKey = new byte[32];
		Buffer.BlockCopy(array5, num15, masterKey, 0, 32);
		num15 += 32;
		encryptedByLogical = new byte[totalChunks][];
		for (int j = 0; j < totalChunks; j++)
		{
			int num16 = BitConverter.ToInt32(array5, num15);
			num15 += 4;
			if (num16 >= 0 && num16 < array3.Length)
			{
				encryptedByLogical[j] = array3[num16];
			}
		}
		int num17 = BitConverter.ToInt32(array5, num15);
		num15 += 4;
		methChunkId = new int[num17];
		methOffset = new int[num17];
		methLength = new int[num17];
		for (int k = 0; k < num17; k++)
		{
			methChunkId[k] = BitConverter.ToInt32(array5, num15);
			num15 += 4;
		}
		for (int l = 0; l < num17; l++)
		{
			methOffset[l] = BitConverter.ToInt32(array5, num15);
			num15 += 4;
		}
		for (int m = 0; m < num17; m++)
		{
			methLength[m] = BitConverter.ToInt32(array5, num15);
			num15 += 4;
		}
		crcExpected = new byte[32];
		if (num15 + 32 <= array5.Length)
		{
			Buffer.BlockCopy(array5, num15, crcExpected, 0, 32);
			num15 += 32;
		}
		protectionFlags = ((num15 < array5.Length) ? array5[num15] : byte.MaxValue);
	}
}
