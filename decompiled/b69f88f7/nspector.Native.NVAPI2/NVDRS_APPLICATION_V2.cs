using System.Runtime.InteropServices;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
internal struct NVDRS_APPLICATION_V2
{
	public uint version;

	public uint isPredefined;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string appName;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string userFriendlyName;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string launcher;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string fileInFolder;
}
