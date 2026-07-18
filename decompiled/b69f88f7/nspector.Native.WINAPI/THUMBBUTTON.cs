using System;
using System.Runtime.InteropServices;

namespace nspector.Native.WINAPI;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct THUMBBUTTON
{
	private int dwMask;

	private uint iId;

	private uint iBitmap;

	private IntPtr hIcon;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
	private string szTip;

	private int dwFlags;
}
