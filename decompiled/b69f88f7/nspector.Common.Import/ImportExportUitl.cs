using System;
using nspector.Native.NVAPI2;

namespace nspector.Common.Import;

internal class ImportExportUitl
{
	public static bool AreDrsSettingEqualToProfileSetting(NVDRS_SETTING drsSetting, ProfileSetting profileSetting)
	{
		ProfileSetting profileSetting2 = ConvertDrsSettingToProfileSetting(drsSetting);
		return profileSetting.SettingValue.Equals(profileSetting2.SettingValue);
	}

	public static ProfileSetting ConvertDrsSettingToProfileSetting(NVDRS_SETTING setting)
	{
		return new ProfileSetting
		{
			SettingId = setting.settingId,
			SettingNameInfo = setting.settingName,
			SettingValue = ConvertSettingValueToString(setting),
			ValueType = MapValueType(setting.settingType)
		};
	}

	private static string ConvertSettingValueToString(NVDRS_SETTING setting)
	{
		NVDRS_SETTING_UNION nVDRS_SETTING_UNION = setting.currentValue;
		if (setting.isCurrentPredefined == 1)
		{
			nVDRS_SETTING_UNION = setting.predefinedValue;
		}
		return setting.settingType switch
		{
			NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE => nVDRS_SETTING_UNION.dwordValue.ToString(), 
			NVDRS_SETTING_TYPE.NVDRS_STRING_TYPE => nVDRS_SETTING_UNION.ansiStringValue, 
			NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE => nVDRS_SETTING_UNION.stringValue, 
			NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE => Convert.ToBase64String(nVDRS_SETTING_UNION.binaryValue), 
			_ => throw new Exception("invalid setting type"), 
		};
	}

	private static SettingValueType MapValueType(NVDRS_SETTING_TYPE input)
	{
		return input switch
		{
			NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE => SettingValueType.Binary, 
			NVDRS_SETTING_TYPE.NVDRS_STRING_TYPE => SettingValueType.AnsiString, 
			NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE => SettingValueType.String, 
			_ => SettingValueType.Dword, 
		};
	}

	public static NVDRS_SETTING ConvertProfileSettingToDrsSetting(ProfileSetting setting)
	{
		return new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER,
			settingId = setting.SettingId,
			settingType = MapValueType(setting.ValueType),
			settingLocation = NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION,
			currentValue = ConvertStringToSettingUnion(setting.ValueType, setting.SettingValue)
		};
	}

	private static NVDRS_SETTING_UNION ConvertStringToSettingUnion(SettingValueType valueType, string valueString)
	{
		NVDRS_SETTING_UNION result = default(NVDRS_SETTING_UNION);
		switch (valueType)
		{
		case SettingValueType.Dword:
			result.dwordValue = uint.Parse(valueString);
			break;
		case SettingValueType.String:
			result.stringValue = valueString;
			break;
		case SettingValueType.AnsiString:
			result.ansiStringValue = valueString;
			break;
		case SettingValueType.Binary:
			result.binaryValue = Convert.FromBase64String(valueString);
			break;
		default:
			throw new Exception("invalid value type");
		}
		return result;
	}

	private static NVDRS_SETTING_TYPE MapValueType(SettingValueType input)
	{
		return input switch
		{
			SettingValueType.Binary => NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE, 
			SettingValueType.AnsiString => NVDRS_SETTING_TYPE.NVDRS_STRING_TYPE, 
			SettingValueType.String => NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE, 
			_ => NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE, 
		};
	}
}
