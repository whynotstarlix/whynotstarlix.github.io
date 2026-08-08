using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace LumaVM.RuntimeProtection;

public static class AntiTamper
{
	private const int DigestSize = 32;

	private const int PeHeaderOffset = 60;

	private const int PeOptionalHeaderOffset = 24;

	private const int PeChecksumOffset = 64;

	private const int Pe32DataDirectoryOffset = 96;

	private const int Pe64DataDirectoryOffset = 112;

	private const int SecurityDirectoryIndex = 4;

	private const int DataDirectoryEntrySize = 8;

	public static void Verify(byte[] marker, bool verifyHostImage)
	{
		byte[] array = null;
		byte[] array2 = null;
		byte[] array3 = null;
		try
		{
			if (marker == null || marker.Length == 0)
			{
				Fail();
			}
			string text = ResolveImagePath(verifyHostImage);
			if (string.IsNullOrEmpty(text) || !File.Exists(text))
			{
				Fail();
			}
			byte[] array4 = File.ReadAllBytes(text);
			try
			{
				array = NormalizeAuthenticodeData(array4);
			}
			finally
			{
				Clear(array4);
			}
			int num = checked(FindUniqueMarker(array, marker) + marker.Length);
			if (num > array.Length - 32)
			{
				Fail();
			}
			array2 = new byte[32];
			Buffer.BlockCopy(array, num, array2, 0, 32);
			Array.Clear(array, num, 32);
			using (SHA256 sHA = SHA256.Create())
			{
				array3 = sHA.ComputeHash(array);
			}
			if (!FixedTimeEquals(array2, array3))
			{
				Fail();
			}
		}
		catch (BadImageFormatException)
		{
			throw;
		}
		catch
		{
			Fail();
		}
		finally
		{
			Clear(array);
			Clear(array2);
			Clear(array3);
		}
	}

	private static string ResolveImagePath(bool verifyHostImage)
	{
		if (!verifyHostImage)
		{
			try
			{
				string location = typeof(AntiTamper).Assembly.Location;
				if (!string.IsNullOrEmpty(location) && File.Exists(location))
				{
					return location;
				}
			}
			catch
			{
			}
		}
		try
		{
			Process currentProcess = Process.GetCurrentProcess();
			string text = ((currentProcess.MainModule == null) ? null : currentProcess.MainModule.FileName);
			if (!string.IsNullOrEmpty(text) && File.Exists(text))
			{
				return text;
			}
		}
		catch
		{
		}
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length != 0 && File.Exists(commandLineArgs[0]))
			{
				return commandLineArgs[0];
			}
		}
		catch
		{
		}
		return null;
	}

	private static int FindUniqueMarker(byte[] image, byte[] marker)
	{
		int num = -1;
		int num2 = image.Length - marker.Length - 32;
		for (int i = 0; i <= num2; i++)
		{
			bool flag = true;
			for (int j = 0; j < marker.Length; j++)
			{
				if (image[i + j] != marker[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (num >= 0)
				{
					Fail();
				}
				num = i;
			}
		}
		if (num < 0)
		{
			Fail();
		}
		return num;
	}

	private static byte[] NormalizeAuthenticodeData(byte[] image)
	{
		byte[] array = (byte[])image.Clone();
		if (array.Length < 64)
		{
			return array;
		}
		int num = BitConverter.ToInt32(array, 60);
		if (num < 0 || num > array.Length - 4 || array[num] != 80 || array[num + 1] != 69 || array[num + 2] != 0 || array[num + 3] != 0)
		{
			return array;
		}
		int num2 = checked(num + 24);
		if (num2 < 0 || num2 > array.Length - 2)
		{
			return array;
		}
		int num3 = checked(BitConverter.ToUInt16(array, num2) switch
		{
			523 => num2 + 112, 
			267 => num2 + 96, 
			_ => -1, 
		});
		if (num3 < 0)
		{
			return array;
		}
		if (num2 <= array.Length - 64 - 4)
		{
			Array.Clear(array, num2 + 64, 4);
		}
		int num4 = checked(num3 + 32);
		if (num4 < 0 || num4 > array.Length - 8)
		{
			return array;
		}
		uint num5 = BitConverter.ToUInt32(array, num4);
		uint num6 = BitConverter.ToUInt32(array, num4 + 4);
		Array.Clear(array, num4, 8);
		if (num5 == 0 || num6 == 0 || num5 > (uint)array.Length || num6 > (uint)(array.Length - (int)num5))
		{
			return array;
		}
		byte[] array2 = new byte[checked(array.Length - (int)num6)];
		try
		{
			Buffer.BlockCopy(array, 0, array2, 0, (int)num5);
			Buffer.BlockCopy(array, checked((int)(num5 + num6)), array2, (int)num5, array.Length - checked((int)(num5 + num6)));
			return array2;
		}
		finally
		{
			Clear(array);
		}
	}

	private static bool FixedTimeEquals(byte[] left, byte[] right)
	{
		if (left == null || right == null || left.Length != right.Length)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < left.Length; i++)
		{
			num |= left[i] ^ right[i];
		}
		return num == 0;
	}

	private static void Clear(byte[] value)
	{
		if (value != null)
		{
			Array.Clear(value, 0, value.Length);
		}
	}

	private static void Fail()
	{
		throw new BadImageFormatException();
	}
}
