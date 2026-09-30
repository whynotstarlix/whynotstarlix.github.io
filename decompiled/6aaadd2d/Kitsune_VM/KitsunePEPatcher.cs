using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Kitsune_VM;

public static class KitsunePEPatcher
{
	internal const uint INDEX_MAGIC = 1263817577u;

	internal const uint DATA_MAGIC = 1263817572u;

	public static void Patch(string exePath, byte[] indexPacket, byte[][] dataPackets)
	{
		byte[] exe = File.ReadAllBytes(exePath);
		int num = 1 + dataPackets.Length;
		byte[] array = ExpandHeadersIfNeeded(exe, num * 40);
		int num2 = BitConverter.ToInt32(array, 60);
		if (BitConverter.ToUInt32(array, num2) != 17744)
		{
			throw new Exception("KitsunePEPatcher: invalid PE signature: " + exePath);
		}
		int num3 = num2 + 4;
		int num4 = BitConverter.ToUInt16(array, num3 + 2);
		int num5 = BitConverter.ToUInt16(array, num3 + 16);
		int num6 = num3 + 20;
		BitConverter.ToUInt16(array, num6);
		uint a = BitConverter.ToUInt32(array, num6 + 36);
		uint a2 = BitConverter.ToUInt32(array, num6 + 32);
		uint num7 = BitConverter.ToUInt32(array, num6 + 60);
		int num8 = num6 + num5;
		int num9 = num8 + num4 * 40;
		int num10 = num8 + (num4 - 1) * 40;
		uint num11 = BitConverter.ToUInt32(array, num10 + 12);
		uint num12 = BitConverter.ToUInt32(array, num10 + 8);
		uint num13 = BitConverter.ToUInt32(array, num10 + 20);
		uint num14 = BitConverter.ToUInt32(array, num10 + 16);
		uint num15 = AlignUp(num13 + num14, a);
		uint num16 = AlignUp(num11 + num12, a2);
		List<byte> list = new List<byte>(array);
		while ((uint)list.Count < num15)
		{
			list.Add(0);
		}
		uint length = num15;
		List<byte[]> list2 = new List<byte[]>();
		list2.Add(indexPacket);
		list2.AddRange(dataPackets);
		Random random = new Random();
		int num17 = num9;
		int num18 = -1;
		for (int i = 0; i < list2.Count; i++)
		{
			byte[] array2 = list2[i];
			uint num19 = ((i == 0) ? 1263817577u : 1263817572u);
			uint num20 = ((i != 0) ? ((uint)(i - 1)) : 0u);
			int num21 = ((i == 0) ? (12 + array2.Length + 32) : (12 + array2.Length));
			uint num22 = AlignUp((uint)num21, a);
			uint num23 = (uint)num21;
			byte[] array3 = new byte[8];
			for (int j = 0; j < 8; j++)
			{
				array3[j] = (byte)(97 + random.Next(26));
			}
			byte[] array4 = new byte[40];
			Buffer.BlockCopy(array3, 0, array4, 0, 8);
			WriteU32(array4, 8, num23);
			WriteU32(array4, 12, num16);
			WriteU32(array4, 16, num22);
			WriteU32(array4, 20, num15);
			WriteU32(array4, 36, 1073741888u);
			for (int k = 0; k < 40; k++)
			{
				list[num17 + k] = array4[k];
			}
			num17 += 40;
			while ((uint)list.Count < num15)
			{
				list.Add(0);
			}
			list.Add((byte)(num19 & 0xFF));
			list.Add((byte)((num19 >> 8) & 0xFF));
			list.Add((byte)((num19 >> 16) & 0xFF));
			list.Add((byte)((num19 >> 24) & 0xFF));
			list.Add((byte)(array2.Length & 0xFF));
			list.Add((byte)((array2.Length >> 8) & 0xFF));
			list.Add((byte)((array2.Length >> 16) & 0xFF));
			list.Add((byte)((array2.Length >> 24) & 0xFF));
			list.Add((byte)(num20 & 0xFF));
			list.Add((byte)((num20 >> 8) & 0xFF));
			list.Add((byte)((num20 >> 16) & 0xFF));
			list.Add((byte)((num20 >> 24) & 0xFF));
			list.AddRange(array2);
			if (i == 0)
			{
				num18 = list.Count;
				for (int l = 0; l < 32; l++)
				{
					list.Add(0);
				}
			}
			while ((uint)list.Count < num15 + num22)
			{
				list.Add(0);
			}
			num16 = AlignUp(num16 + num23, a2);
			num15 = AlignUp((uint)list.Count, a);
		}
		int num24 = num4 + num;
		list[num3 + 2] = (byte)(num24 & 0xFF);
		list[num3 + 3] = (byte)((num24 >> 8) & 0xFF);
		uint v = AlignUp(num16, a2);
		int off = num6 + 56;
		WriteU32(list, off, v);
		if (num18 >= 0)
		{
			byte[] array5 = ComputeVmCrc(list, (int)length);
			for (int m = 0; m < 32; m++)
			{
				list[num18 + m] = array5[m];
			}
		}
		File.WriteAllBytes(exePath, list.ToArray());
		Console.WriteLine($"[OK] PE sections added: {dataPackets.Length} data sections + index, total {list.Count} bytes");
	}

	private static byte[] ComputeVmCrc(List<byte> bytes, int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = bytes[i];
		}
		using SHA256 sHA = SHA256.Create();
		return sHA.ComputeHash(array);
	}

	private static byte[] ExpandHeadersIfNeeded(byte[] exe, int bytesNeeded)
	{
		int num = BitConverter.ToInt32(exe, 60) + 4;
		int num2 = BitConverter.ToUInt16(exe, num + 2);
		int num3 = BitConverter.ToUInt16(exe, num + 16);
		int num4 = num + 20;
		uint a = BitConverter.ToUInt32(exe, num4 + 36);
		uint num5 = BitConverter.ToUInt32(exe, num4 + 60);
		int num6 = num4 + num3;
		int num7 = num6 + num2 * 40;
		int num8 = (int)num5 - num7;
		if (num8 >= bytesNeeded)
		{
			return exe;
		}
		uint num9 = AlignUp((uint)(bytesNeeded - num8), a);
		uint num10 = uint.MaxValue;
		for (int i = 0; i < num2; i++)
		{
			int num11 = num6 + i * 40;
			uint num12 = BitConverter.ToUInt32(exe, num11 + 20);
			if (num12 != 0 && num12 < num10)
			{
				num10 = num12;
			}
		}
		byte[] array = new byte[exe.Length + (int)num9];
		Buffer.BlockCopy(exe, 0, array, 0, (int)num10);
		Buffer.BlockCopy(exe, (int)num10, array, (int)(num10 + num9), exe.Length - (int)num10);
		for (int j = 0; j < num2; j++)
		{
			int num13 = num6 + j * 40;
			uint num14 = BitConverter.ToUInt32(array, num13 + 20);
			if (num14 != 0)
			{
				WriteU32(array, num13 + 20, num14 + num9);
			}
		}
		WriteU32(array, num4 + 60, num5 + num9);
		return array;
	}

	private static uint AlignUp(uint v, uint a)
	{
		if (a != 0)
		{
			return (v + a - 1) & ~(a - 1);
		}
		return v;
	}

	private static void WriteU32(byte[] b, int off, uint v)
	{
		b[off] = (byte)(v & 0xFF);
		b[off + 1] = (byte)((v >> 8) & 0xFF);
		b[off + 2] = (byte)((v >> 16) & 0xFF);
		b[off + 3] = (byte)((v >> 24) & 0xFF);
	}

	private static void WriteU32(List<byte> b, int off, uint v)
	{
		b[off] = (byte)(v & 0xFF);
		b[off + 1] = (byte)((v >> 8) & 0xFF);
		b[off + 2] = (byte)((v >> 16) & 0xFF);
		b[off + 3] = (byte)((v >> 24) & 0xFF);
	}
}
