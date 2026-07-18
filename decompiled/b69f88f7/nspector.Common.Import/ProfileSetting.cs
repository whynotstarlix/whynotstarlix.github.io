using System;
using System.Xml.Serialization;

namespace nspector.Common.Import;

[Serializable]
public class ProfileSetting
{
	public string SettingNameInfo = "";

	[XmlElement(ElementName = "SettingID")]
	public uint SettingId = 0u;

	public string SettingValue = "0";

	public SettingValueType ValueType = SettingValueType.Dword;
}
