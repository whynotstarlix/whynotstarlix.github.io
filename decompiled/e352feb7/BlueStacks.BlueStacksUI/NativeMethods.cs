using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal static class NativeMethods
{
	internal struct Win32Point
	{
		public int X;

		public int Y;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr SetWindowsHookEx(int idHook, GlobalKeyBoardMouseHooks.LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr SetWindowsHookEx(int idHook, GlobalKeyBoardMouseHooks.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

	[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool RegisterRawInputDevices(RawInputClass.RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetCursorPos(ref Win32Point pt);

	[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

	[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

	[DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern uint FindMimeFromData(uint pBC, [MarshalAs(UnmanagedType.LPStr)] string pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer, uint cbSize, [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed, uint dwMimeFlags, out uint ppwzMimeOut, uint dwReserverd);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

	[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool GetMonitorInfo(IntPtr hmonitor, [In][Out] WindowWndProcHandler.MONITORINFOEX info);

	[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr SHAppBarMessage(int msg, ref WindowWndProcHandler.APPBARDATA data);
}
