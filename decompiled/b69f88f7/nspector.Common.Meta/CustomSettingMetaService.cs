using System;
using System.Collections.Generic;
using System.Linq;
using nspector.Common.CustomSettings;
using nspector.Common.Helper;
using nspector.Native.NVAPI2;

namespace nspector.Common.Meta;

internal class CustomSettingMetaService : ISettingMetaService
{
	private readonly CustomSettingNames customSettings;

	private readonly SettingMetaSource _source;

	public SettingMetaSource Source => _source;

	public CustomSettingMetaService(CustomSettingNames customSettings, SettingMetaSource sourceOverride = SettingMetaSource.CustomSettings)
	{
		this.customSettings = customSettings;
		_source = sourceOverride;
	}

	public NVDRS_SETTING_TYPE? GetSettingValueType(uint settingId)
	{
		return MapType(customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId))?.DataType);
	}

	private NVDRS_SETTING_TYPE? MapType(string type)
	{
		if (string.IsNullOrEmpty(type))
		{
			return null;
		}
		return type.ToLowerInvariant() switch
		{
			"dword" => NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE, 
			"string" => NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE, 
			"binary" => NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE, 
			_ => throw new ArgumentOutOfRangeException(type), 
		};
	}

	private string ProcessNameReplacements(string friendlyName)
	{
		return DlssHelper.ReplaceDlssVersions(friendlyName);
	}

	public string GetSettingName(uint settingId)
	{
		CustomSetting customSetting = customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId));
		if (customSetting != null)
		{
			return ProcessNameReplacements(customSetting.UserfriendlyName);
		}
		return null;
	}

	public uint? GetDwordDefaultValue(uint settingId)
	{
		return customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId))?.DefaultValue;
	}

	public string GetStringDefaultValue(uint settingId)
	{
		return null;
	}

	public List<SettingValue<string>> GetStringValues(uint settingId)
	{
		return null;
	}

	public List<SettingValue<uint>> GetDwordValues(uint settingId)
	{
		CustomSetting customSetting = customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId));
		if (customSetting != null)
		{
			int i = 0;
			return customSetting.SettingValues.Select((CustomSettingValue x) => new SettingValue<uint>(Source)
			{
				ValuePos = i++,
				Value = x.SettingValue,
				ValueName = ((_source == SettingMetaSource.CustomSettings) ? ProcessNameReplacements(x.UserfriendlyName) : (DrsUtil.GetDwordString(x.SettingValue) + " " + ProcessNameReplacements(x.UserfriendlyName)))
			}).ToList();
		}
		return null;
	}

	public List<uint> GetSettingIds()
	{
		return customSettings.Settings.Select((CustomSetting x) => x.SettingId).ToList();
	}

	public string GetGroupName(uint settingId)
	{
		CustomSetting customSetting = customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId));
		if (customSetting != null && !string.IsNullOrWhiteSpace(customSetting.GroupName))
		{
			return customSetting.GroupName;
		}
		return null;
	}

	public string GetAlternateNames(uint settingId)
	{
		return customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId))?.AlternateNames;
	}

	public byte[] GetBinaryDefaultValue(uint settingId)
	{
		return null;
	}

	public List<SettingValue<byte[]>> GetBinaryValues(uint settingId)
	{
		return null;
	}

	public bool IsSettingHidden(uint settingId)
	{
		CustomSetting customSetting = customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId));
		if (DrsSettingsServiceBase.DriverVersion > 0f)
		{
			if ((double)DrsSettingsServiceBase.DriverVersion > 425.31 && (settingId & 0xFF000000u) == 1879048192)
			{
				return true;
			}
			if (customSetting == null)
			{
				return false;
			}
			if (customSetting.MinRequiredDriverVersion > 0f && customSetting.MinRequiredDriverVersion > DrsSettingsServiceBase.DriverVersion)
			{
				return true;
			}
			if (customSetting.MaxRequiredDriverVersion > 0f && customSetting.MaxRequiredDriverVersion < DrsSettingsServiceBase.DriverVersion)
			{
				return true;
			}
		}
		return customSetting?.Hidden ?? false;
	}

	public string GetDescription(uint settingId)
	{
		return customSettings.Settings.FirstOrDefault((CustomSetting x) => x.SettingId.Equals(settingId))?.Description;
	}
}
