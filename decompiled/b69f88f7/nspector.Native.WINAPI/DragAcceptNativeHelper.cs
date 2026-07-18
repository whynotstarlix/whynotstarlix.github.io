using System;
using System.Runtime.InteropServices;

namespace nspector.Native.WINAPI;

internal static class DragAcceptNativeHelper
{
	internal const int MSGFLT_ADD = 1;

	internal const int MSGFLT_REMOVE = 2;

	internal const int MSGFLT_ALLOW = 1;

	internal const int MSGFLT_DISALLOW = 2;

	internal const int MSGFLT_RESET = 3;

	internal const int WM_DROPFILES = 563;

	internal const int WM_COPYDATA = 74;

	internal const int WM_COPYGLOBALDATA = 73;

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr ChangeWindowMessageFilter(int message, int dwFlag);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr ChangeWindowMessageFilterEx(IntPtr handle, int message, int action, IntPtr pChangeFilterStruct);

	[DllImport("shell32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
	public static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);
}
