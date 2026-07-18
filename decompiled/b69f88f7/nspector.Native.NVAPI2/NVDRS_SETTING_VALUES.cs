using System.Runtime.InteropServices;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NVDRS_SETTING_VALUES
{
	public uint version;

	public uint numSettingValues;

	public NVDRS_SETTING_TYPE settingType;

	public NVDRS_SETTING_UNION defaultValue;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
	public NVDRS_SETTING_UNION[] settingValues;
}
