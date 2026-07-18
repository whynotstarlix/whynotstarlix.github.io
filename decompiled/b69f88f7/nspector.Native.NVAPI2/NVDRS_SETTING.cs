using System.Runtime.InteropServices;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
internal struct NVDRS_SETTING
{
	public uint version;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string settingName;

	public uint settingId;

	public NVDRS_SETTING_TYPE settingType;

	public NVDRS_SETTING_LOCATION settingLocation;

	public uint isCurrentPredefined;

	public uint isPredefinedValid;

	public NVDRS_SETTING_UNION predefinedValue;

	public NVDRS_SETTING_UNION currentValue;
}
