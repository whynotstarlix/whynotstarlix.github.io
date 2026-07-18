using System.Collections.Generic;
using System.Text;
using nspector.Native.NVAPI2;

namespace nspector.Common.Meta;

internal class DriverSettingMetaService : ISettingMetaService
{
	private readonly Dictionary<uint, SettingMeta> _settingMetaCache = new Dictionary<uint, SettingMeta>();

	private readonly List<uint> _settingIds;

	public SettingMetaSource Source => SettingMetaSource.DriverSettings;

	public DriverSettingMetaService()
	{
		_settingIds = InitSettingIds();
	}

	private List<uint> InitSettingIds()
	{
		List<uint> settingIds = new List<uint>();
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_EnumAvailableSettingIds(out settingIds, 512u);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_EnumAvailableSettingIds", nvAPI_Status);
		}
		return settingIds;
	}

	private SettingMeta GetDriverSettingMetaInternal(uint settingId)
	{
		if ((settingId & 0xFFFFF000u) == 281530368)
		{
			return null;
		}
		NVDRS_SETTING_VALUES settingValues = new NVDRS_SETTING_VALUES
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VALUES_VER
		};
		uint pMaxNumValues = 255u;
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_EnumAvailableSettingValues(settingId, ref pMaxNumValues, ref settingValues);
		switch (nvAPI_Status)
		{
		case NvAPI_Status.NVAPI_SETTING_NOT_FOUND:
			return null;
		default:
			throw new NvapiException("DRS_EnumAvailableSettingValues", nvAPI_Status);
		case NvAPI_Status.NVAPI_OK:
		{
			StringBuilder stringBuilder = new StringBuilder(2048);
			nvAPI_Status = NvapiDrsWrapper.DRS_GetSettingNameFromId(settingId, stringBuilder);
			if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
			{
				throw new NvapiException("DRS_GetSettingNameFromId", nvAPI_Status);
			}
			string text = stringBuilder.ToString();
			if (string.IsNullOrWhiteSpace(text))
			{
				text = DrsUtil.GetDwordString(settingId);
			}
			SettingMeta settingMeta = new SettingMeta
			{
				SettingType = settingValues.settingType,
				SettingName = text
			};
			if (settingValues.settingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
			{
				settingMeta.DefaultDwordValue = settingValues.defaultValue.dwordValue;
				settingMeta.DwordValues = new List<SettingValue<uint>>();
				for (int i = 0; i < settingValues.numSettingValues; i++)
				{
					settingMeta.DwordValues.Add(new SettingValue<uint>(Source)
					{
						Value = settingValues.settingValues[i].dwordValue,
						ValueName = DrsUtil.GetDwordString(settingValues.settingValues[i].dwordValue)
					});
				}
			}
			if (settingValues.settingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
			{
				settingMeta.DefaultStringValue = settingValues.defaultValue.stringValue;
				settingMeta.StringValues = new List<SettingValue<string>>();
				for (int j = 0; j < settingValues.numSettingValues; j++)
				{
					string stringValue = settingValues.settingValues[j].stringValue;
					if (stringValue != null)
					{
						settingMeta.StringValues.Add(new SettingValue<string>(Source)
						{
							Value = stringValue,
							ValueName = stringValue
						});
					}
				}
			}
			if (settingValues.settingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
			{
				settingMeta.DefaultBinaryValue = settingValues.defaultValue.binaryValue;
				settingMeta.BinaryValues = new List<SettingValue<byte[]>>();
				for (int k = 0; k < settingValues.numSettingValues; k++)
				{
					byte[] binaryValue = settingValues.settingValues[k].binaryValue;
					if (binaryValue != null)
					{
						settingMeta.BinaryValues.Add(new SettingValue<byte[]>(Source)
						{
							Value = binaryValue,
							ValueName = DrsUtil.GetBinaryString(binaryValue)
						});
					}
				}
			}
			return settingMeta;
		}
		}
	}

	private SettingMeta GetSettingsMeta(uint settingId)
	{
		if (_settingMetaCache.ContainsKey(settingId))
		{
			return _settingMetaCache[settingId];
		}
		SettingMeta driverSettingMetaInternal = GetDriverSettingMetaInternal(settingId);
		if (driverSettingMetaInternal != null)
		{
			_settingMetaCache.Add(settingId, driverSettingMetaInternal);
			return driverSettingMetaInternal;
		}
		return null;
	}

	public string GetSettingName(uint settingId)
	{
		return GetSettingsMeta(settingId)?.SettingName;
	}

	public uint? GetDwordDefaultValue(uint settingId)
	{
		return GetSettingsMeta(settingId)?.DefaultDwordValue;
	}

	public string GetStringDefaultValue(uint settingId)
	{
		return GetSettingsMeta(settingId)?.DefaultStringValue;
	}

	public List<SettingValue<string>> GetStringValues(uint settingId)
	{
		return GetSettingsMeta(settingId)?.StringValues;
	}

	public List<SettingValue<uint>> GetDwordValues(uint settingId)
	{
		return GetSettingsMeta(settingId)?.DwordValues;
	}

	public List<uint> GetSettingIds()
	{
		return _settingIds;
	}

	public NVDRS_SETTING_TYPE? GetSettingValueType(uint settingId)
	{
		return GetSettingsMeta(settingId)?.SettingType;
	}

	public string GetGroupName(uint settingId)
	{
		return null;
	}

	public string GetAlternateNames(uint settingId)
	{
		return null;
	}

	public byte[] GetBinaryDefaultValue(uint settingId)
	{
		return GetSettingsMeta(settingId)?.DefaultBinaryValue;
	}

	public List<SettingValue<byte[]>> GetBinaryValues(uint settingId)
	{
		return GetSettingsMeta(settingId)?.BinaryValues;
	}
}
