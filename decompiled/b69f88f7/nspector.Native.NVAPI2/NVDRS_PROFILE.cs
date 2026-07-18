using System.Runtime.InteropServices;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
internal struct NVDRS_PROFILE
{
	public uint version;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string profileName;

	public NVDRS_GPU_SUPPORT gpuSupport;

	public uint isPredefined;

	public uint numOfApps;

	public uint numOfSettings;
}
