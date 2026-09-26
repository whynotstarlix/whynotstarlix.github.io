using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace __002B_002A_0029_0025_002B_003E_005B_007E_002B;

internal static class _005D_003D_0029_007C_0025__002E_002C_003D_0040
{
	private const int _003E_002B_0024_002A_007E_003A_007B_002F_003B_007B = 96;

	private const int _007B_003B_002B_005B_005D_005D__002D_003A_002E = 65536;

	internal unsafe static bool _007E_002A__003E_005E_002E_0021_005D_005B_003B(byte[] _003B_003F_002F_0026_005E_007B_007C_003A_002C_005E, out IntPtr _003B_007C_003B_005D_003C_002F_005E_007B_002B_007D, out int _002D_002C__0021_0029_005B_007B_0026_0025_002F)
	{
		_003B_007C_003B_005D_003C_002F_005E_007B_002B_007D = IntPtr.Zero;
		_002D_002C__0021_0029_005B_007B_0026_0025_002F = 0;
		if (_003B_003F_002F_0026_005E_007B_007C_003A_002C_005E == null || _003B_003F_002F_0026_005E_007B_007C_003A_002C_005E.Length == 0 || _003B_003F_002F_0026_005E_007B_007C_003A_002C_005E.Length > 8)
		{
			return false;
		}
		if (!_0029_007C_003E_002F_003E_002D_0023_002A_005D_003C())
		{
			return false;
		}
		Module module = _007E_003D_007C_0021_007D_0024_007B_007E_005D_003A._007B_003F_0021_002E_0029_007B_002C_0040_002A_003D(typeof(_005D_003D_0029_007C_0025__002E_002C_003D_0040));
		string fullyQualifiedName;
		try
		{
			fullyQualifiedName = module.FullyQualifiedName;
		}
		catch (Exception ex) when ((ex is NotSupportedException || ex is IOException) ? true : false)
		{
			return false;
		}
		if (string.IsNullOrEmpty(fullyQualifiedName) || fullyQualifiedName[0] == '<')
		{
			return false;
		}
		IntPtr intPtr;
		try
		{
			intPtr = Marshal.GetHINSTANCE(module);
		}
		catch (Exception ex2) when ((ex2 is ArgumentException || ex2 is PlatformNotSupportedException || ex2 is NotSupportedException || ex2 is TargetInvocationException) ? true : false)
		{
			intPtr = IntPtr.Zero;
		}
		if (intPtr == IntPtr.Zero || intPtr == new IntPtr(-1))
		{
			return false;
		}
		byte* ptr = (byte*)intPtr.ToPointer();
		if (_002A_005E_0029_002C_007B_002A_0023_0024_003D_0028(ptr) != 23117)
		{
			return false;
		}
		int num = _003D_003A_0040_002E_007C_005D_002A_007C_0026_0023(ptr + 60);
		if (num < 64 || num > 65536)
		{
			return false;
		}
		byte* ptr2 = ptr + num;
		if (_003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr2) != 17744)
		{
			return false;
		}
		byte* ptr3 = ptr2 + 4;
		ushort num2 = _002A_005E_0029_002C_007B_002A_0023_0024_003D_0028(ptr3 + 2);
		ushort num3 = _002A_005E_0029_002C_007B_002A_0023_0024_003D_0028(ptr3 + 16);
		if (num2 == 0 || num2 > 96 || num3 < 64)
		{
			return false;
		}
		byte* ptr4 = ptr3 + 20;
		ushort num4 = _002A_005E_0029_002C_007B_002A_0023_0024_003D_0028(ptr4);
		if (num4 != 267 && num4 != 523)
		{
			return false;
		}
		uint num5 = _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr4 + 56);
		uint num6 = _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr4 + 60);
		int num7 = checked(num + 24 + num3);
		long num8 = num7 + (long)num2 * 40L;
		if (num5 == 0 || num5 > int.MaxValue || num6 == 0 || num6 > num5 || num7 < 0 || num8 > num6)
		{
			return false;
		}
		byte* ptr5 = ptr + num7;
		for (int i = 0; i < num2; i++)
		{
			byte* ptr6 = ptr5 + i * 40;
			if (_003F_002B_005E_002D_0023_005D_0040_007C_0029_003A(ptr6, _003B_003F_002F_0026_005E_007B_007C_003A_002C_005E))
			{
				uint num9 = _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr6 + 8);
				uint num10 = _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr6 + 12);
				uint num11 = _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(ptr6 + 16);
				uint num12 = ((num9 == 0) ? num11 : num9);
				if (num12 >= 256 && num12 <= int.MaxValue && num12 <= num5 && num10 != 0 && num10 <= num5 - num12)
				{
					IntPtr intPtr2 = new IntPtr(ptr + num10);
					_003B_007C_003B_005D_003C_002F_005E_007B_002B_007D = intPtr2;
					_002D_002C__0021_0029_005B_007B_0026_0025_002F = checked((int)num12);
					return true;
				}
			}
		}
		return false;
	}

	private unsafe static bool _003F_002B_005E_002D_0023_005D_0040_007C_0029_003A(byte* _003E_0028_002D_005E_002A_007B_0025_007E_003E_003B, byte[] _0029_002A__003F_003C_003F_0029_002A_0028_003B)
	{
		for (int i = 0; i < 8; i++)
		{
			byte b = (byte)((i < _0029_002A__003F_003C_003F_0029_002A_0028_003B.Length) ? _0029_002A__003F_003C_003F_0029_002A_0028_003B[i] : 0);
			if (_003E_0028_002D_005E_002A_007B_0025_007E_003E_003B[i] != b)
			{
				return false;
			}
		}
		return true;
	}

	private static bool _0029_007C_003E_002F_003E_002D_0023_002A_005D_003C()
	{
		PlatformID platform = Environment.OSVersion.Platform;
		if (platform != PlatformID.Win32NT && platform != PlatformID.Win32S && platform != PlatformID.Win32Windows)
		{
			return platform == PlatformID.WinCE;
		}
		return true;
	}

	private unsafe static ushort _002A_005E_0029_002C_007B_002A_0023_0024_003D_0028(byte* _007B_003C_0025_002B_003C_0024__007E_007E_003F)
	{
		return (ushort)(*_007B_003C_0025_002B_003C_0024__007E_007E_003F | (_007B_003C_0025_002B_003C_0024__007E_007E_003F[1] << 8));
	}

	private unsafe static int _003D_003A_0040_002E_007C_005D_002A_007C_0026_0023(byte* _003B_0025_003B_007D_002F_0024_002F_002B_007E_003B)
	{
		return *_003B_0025_003B_007D_002F_0024_002F_002B_007E_003B | (_003B_0025_003B_007D_002F_0024_002F_002B_007E_003B[1] << 8) | (_003B_0025_003B_007D_002F_0024_002F_002B_007E_003B[2] << 16) | (_003B_0025_003B_007D_002F_0024_002F_002B_007E_003B[3] << 24);
	}

	private unsafe static uint _003F_002E_003B_003C_003B_007C_0026_002B_0024_005B(byte* _005D_002C_003F_003D_002C_007D_0026_002B_002F_005D)
	{
		return (uint)_003D_003A_0040_002E_007C_005D_002A_007C_0026_0023(_005D_002C_003F_003D_002C_007D_0026_002B_002F_005D);
	}
}
