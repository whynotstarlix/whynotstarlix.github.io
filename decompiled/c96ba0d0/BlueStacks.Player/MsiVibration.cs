using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class MsiVibration
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate bool InitDll();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate bool ReleaseDll();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate void SetKBVibration(int milliSeconds);

	private static ReleaseDll releaseDll = null;

	private static SetKBVibration setKBVibration = null;

	private static IntPtr hModule = IntPtr.Zero;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

	[DllImport("kernel32.dll")]
	private static extern bool FreeLibrary(int hModule);

	public static bool Init()
	{
		if (SystemUtils.IsOs64Bit())
		{
			hModule = LoadLibrary("MsiKBVibration64.dll");
		}
		else
		{
			hModule = LoadLibrary("MsiKBVibration.dll");
		}
		if (hModule == IntPtr.Zero)
		{
			return false;
		}
		if (!((InitDll)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "InitDLL"), typeof(InitDll)))())
		{
			return false;
		}
		releaseDll = (ReleaseDll)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "ReleaseDLL"), typeof(ReleaseDll));
		setKBVibration = (SetKBVibration)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hModule, "SetKBVibration"), typeof(SetKBVibration));
		return true;
	}

	public static void Release()
	{
		releaseDll();
		FreeLibrary(hModule.ToInt32());
	}

	public static void SetVibration(int duration)
	{
		setKBVibration(duration);
	}
}
