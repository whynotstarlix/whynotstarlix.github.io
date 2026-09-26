using System;
using System.IO;
using System.Runtime.InteropServices;

namespace __002B_002A_0029_0025_002B_003E_005B_007E_002B;

internal static class _002B_003F_002E_005B_0021_003E_0029_0029_002B_005D
{
	internal static Delegate _003E_005B_0040_002E_0025_007B_0025_0029_0029_005B(byte[] _007D_003D_0024_002A_0028_003D_002C_003F_0021_003C, RuntimeTypeHandle _002F_007B_0021_005B_003E_0021_002F_002D_003F_003C)
	{
		return Marshal.GetDelegateForFunctionPointer(_002C_003A_003C_003E_003D_005E_0040_005E_005B_002E(_007D_003D_0024_002A_0028_003D_002C_003F_0021_003C), Type.GetTypeFromHandle(_002F_007B_0021_005B_003E_0021_002F_002D_003F_003C));
	}

	private static IntPtr _002C_003A_003C_003E_003D_005E_0040_005E_005B_002E(byte[] _0023_005D_005D_003A_0021_005D_003B_007B_0024_003D)
	{
		if (_0023_005D_005D_003A_0021_005D_003B_007B_0024_003D == null || _0023_005D_005D_003A_0021_005D_003B_007B_0024_003D.Length < 3)
		{
			throw new InvalidOperationException("Native import descriptor is invalid.");
		}
		try
		{
			using MemoryStream memoryStream = new MemoryStream(_0023_005D_005D_003A_0021_005D_003B_007B_0024_003D, writable: false);
			using BinaryReader binaryReader = new BinaryReader(memoryStream);
			string text = binaryReader.ReadString();
			string text2 = binaryReader.ReadString();
			byte b = binaryReader.ReadByte();
			if (memoryStream.Position != memoryStream.Length || text.Length == 0 || text2.Length == 0)
			{
				throw new InvalidOperationException("Native import descriptor is invalid.");
			}
			IntPtr intPtr = _007D_002D_007D_0025__002C_007B_007C_0040_002B(text);
			if (intPtr == IntPtr.Zero)
			{
				throw new DllNotFoundException(text);
			}
			IntPtr intPtr2 = _003A_003C_0040_005B_0040_003F_0024_003F_0029_005B(intPtr, text2, b);
			if (intPtr2 == IntPtr.Zero)
			{
				throw new InvalidOperationException("Native entry point was not found: " + text2);
			}
			return intPtr2;
		}
		finally
		{
			_007D_003F_005B_0026_005E_0024_003B_003C_0021_003A._0026_003E_002B_005B_0023_003C_007C_002F_005B_0025(_0023_005D_005D_003A_0021_005D_003B_007B_0024_003D);
		}
	}

	private static IntPtr _003A_003C_0040_005B_0040_003F_0024_003F_0029_005B(IntPtr _0024_005B_003D_005B_007C_0040_003C_005B_002E_005E, string @_003E_007B_0025_003A_0021_0029_007D_002B_002B, byte _005B_003E_003C_0040_007B_007B_007B_0040_003D_)
	{
		if (_005B_003E_003C_0040_007B_007B_007B_0040_003D_ != 0 && !@_003E_007B_0025_003A_0021_0029_007D_002B_002B.EndsWith("A", StringComparison.Ordinal) && !@_003E_007B_0025_003A_0021_0029_007D_002B_002B.EndsWith("W", StringComparison.Ordinal))
		{
			string text = ((_005B_003E_003C_0040_007B_007B_007B_0040_003D_ == 1) ? "A" : "W");
			IntPtr intPtr = _0024_0028_002C_0024_007C_0021_0024_002E_0021_0024(_0024_005B_003D_005B_007C_0040_003C_005B_002E_005E, @_003E_007B_0025_003A_0021_0029_007D_002B_002B + text, 0);
			if (intPtr != IntPtr.Zero)
			{
				return intPtr;
			}
		}
		return _0024_0028_002C_0024_007C_0021_0024_002E_0021_0024(_0024_005B_003D_005B_007C_0040_003C_005B_002E_005E, @_003E_007B_0025_003A_0021_0029_007D_002B_002B, 0);
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
	private static extern IntPtr _0026_0021_0024_0028_002C_003A_005E_003A_007D_0024(string _002C_002F_003D_005B_002A_0028_007C_007E_002A_005E);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr _002F_002B_0023_003A_0040_0040_007C_005B_005E_002C(IntPtr _007D_005D_0040_007E_0024_002E_003D_0024_0024_002E, string _002F_0023_002B_0021_002F_0025_007D_002A_002F_002F);

	[DllImport("kernel32.dll", EntryPoint = "GetProcAddress", ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr _005D_0029_0040_0026_0029_007C_0040_0026_003E_0040(IntPtr __002F_005E_003C_0026__0029_007E_002B_007D, IntPtr _003C_002F_007C_002D_005B_003E_0040_0025_0021_007D);

	private static IntPtr _007D_002D_007D_0025__002C_007B_007C_0040_002B(string _003D_0023_003D_0026_0029_0023_002C_0025_002D_003F)
	{
		return _0026_0021_0024_0028_002C_003A_005E_003A_007D_0024(_003D_0023_003D_0026_0029_0023_002C_0025_002D_003F);
	}

	private static IntPtr _0024_0028_002C_0024_007C_0021_0024_002E_0021_0024(IntPtr _0028_002E_002F_007B_003E_002D_005D_002F_007D_007D, string _002E_0026_0040_007B_0040_0029_002B_003B_003E_007C, int _0023_007C_0029_007E_003E_002A_0021_007D_0028_002F)
	{
		if (_0028_002E_002F_007B_003E_002D_005D_002F_007D_007D == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		if (_002E_0026_0040_007B_0040_0029_002B_003B_003E_007C.Length > 1 && _002E_0026_0040_007B_0040_0029_002B_003B_003E_007C[0] == '#' && int.TryParse(_002E_0026_0040_007B_0040_0029_002B_003B_003E_007C.Substring(1), out var result) && result > 0 && result <= 65535)
		{
			return _005D_0029_0040_0026_0029_007C_0040_0026_003E_0040(_0028_002E_002F_007B_003E_002D_005D_002F_007D_007D, new IntPtr(result));
		}
		return _002F_002B_0023_003A_0040_0040_007C_005B_005E_002C(_0028_002E_002F_007B_003E_002D_005D_002F_007D_007D, _002E_0026_0040_007B_0040_0029_002B_003B_003E_007C);
	}
}
