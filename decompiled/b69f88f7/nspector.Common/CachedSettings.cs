using System.Collections.Generic;
using System.Linq;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class CachedSettings
{
	internal uint SettingId;

	internal List<CachedSettingValue> SettingValues = new List<CachedSettingValue>();

	internal uint ProfileCount = 0u;

	internal NVDRS_SETTING_TYPE SettingType = NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE;

	internal CachedSettings()
	{
	}

	internal CachedSettings(uint settingId, NVDRS_SETTING_TYPE settingType)
	{
		SettingId = settingId;
		SettingType = settingType;
	}

	internal void AddDwordValue(uint valueDword, string Profile)
	{
		CachedSettingValue cachedSettingValue = SettingValues.FirstOrDefault((CachedSettingValue s) => s.Value == valueDword);
		if (cachedSettingValue == null)
		{
			SettingValues.Add(new CachedSettingValue(valueDword, Profile));
		}
		else
		{
			cachedSettingValue.ProfileNames.Append(", " + Profile);
			cachedSettingValue.ValueProfileCount++;
		}
		ProfileCount++;
	}

	internal void AddStringValue(string valueStr, string Profile)
	{
		CachedSettingValue cachedSettingValue = SettingValues.FirstOrDefault((CachedSettingValue s) => s.ValueStr == valueStr);
		if (cachedSettingValue == null)
		{
			SettingValues.Add(new CachedSettingValue(valueStr, Profile));
		}
		else
		{
			cachedSettingValue.ProfileNames.Append(", " + Profile);
			cachedSettingValue.ValueProfileCount++;
		}
		ProfileCount++;
	}

	internal void AddBinaryValue(byte[] valueBin, string Profile)
	{
		CachedSettingValue cachedSettingValue = SettingValues.FirstOrDefault((CachedSettingValue s) => s.ValueBin.SequenceEqual(valueBin));
		if (cachedSettingValue == null)
		{
			SettingValues.Add(new CachedSettingValue(valueBin, Profile));
		}
		else
		{
			cachedSettingValue.ProfileNames.Append(", " + Profile);
			cachedSettingValue.ValueProfileCount++;
		}
		ProfileCount++;
	}
}
