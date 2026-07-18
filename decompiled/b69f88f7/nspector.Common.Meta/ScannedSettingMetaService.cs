using System.Collections.Generic;
using System.Linq;
using nspector.Native.NVAPI2;

namespace nspector.Common.Meta;

internal class ScannedSettingMetaService : ISettingMetaService
{
	private readonly List<CachedSettings> CachedSettings;

	public SettingMetaSource Source => SettingMetaSource.ScannedSettings;

	public ScannedSettingMetaService(List<CachedSettings> cachedSettings)
	{
		CachedSettings = cachedSettings;
	}

	public NVDRS_SETTING_TYPE? GetSettingValueType(uint settingId)
	{
		return CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(settingId))?.SettingType;
	}

	public string GetSettingName(uint settingId)
	{
		CachedSettings cachedSettings = CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(settingId));
		if (cachedSettings != null)
		{
			return $"0x{settingId:X8} ({cachedSettings.ProfileCount} Profiles)";
		}
		return null;
	}

	public string GetGroupName(uint settingId)
	{
		return null;
	}

	public string GetAlternateNames(uint settingId)
	{
		return null;
	}

	public uint? GetDwordDefaultValue(uint settingId)
	{
		return null;
	}

	public string GetStringDefaultValue(uint settingId)
	{
		return null;
	}

	public List<SettingValue<string>> GetStringValues(uint settingId)
	{
		return CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(settingId))?.SettingValues.Select((CachedSettingValue s) => new SettingValue<string>(Source)
		{
			Value = s.ValueStr,
			ValueName = $"'{s.ValueStr.Trim()}' ({s.ProfileNames})"
		}).ToList();
	}

	public List<SettingValue<uint>> GetDwordValues(uint settingId)
	{
		return CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(settingId))?.SettingValues.Select((CachedSettingValue s) => new SettingValue<uint>(Source)
		{
			Value = s.Value,
			ValueName = $"0x{s.Value:X8} ({s.ProfileNames})"
		}).ToList();
	}

	public List<uint> GetSettingIds()
	{
		return CachedSettings.Select((CachedSettings c) => c.SettingId).ToList();
	}

	public byte[] GetBinaryDefaultValue(uint settingId)
	{
		return null;
	}

	public List<SettingValue<byte[]>> GetBinaryValues(uint settingId)
	{
		return CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(settingId))?.SettingValues.Select((CachedSettingValue s) => new SettingValue<byte[]>(Source)
		{
			Value = s.ValueBin,
			ValueName = $"{DrsUtil.GetBinaryString(s.ValueBin)} ({s.ProfileNames})"
		}).ToList();
	}
}
