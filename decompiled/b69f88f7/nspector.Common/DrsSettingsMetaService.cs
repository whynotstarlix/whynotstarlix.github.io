using System;
using System.Collections.Generic;
using System.Linq;
using nspector.Common.CustomSettings;
using nspector.Common.Meta;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class DrsSettingsMetaService
{
	private ISettingMetaService ConstantMeta;

	private ISettingMetaService CustomMeta;

	public ISettingMetaService DriverMeta;

	private ISettingMetaService ScannedMeta;

	private ISettingMetaService ReferenceMeta;

	private readonly CustomSettingNames _customSettings;

	private readonly CustomSettingNames _referenceSettings;

	private List<MetaServiceItem> MetaServices = new List<MetaServiceItem>();

	private Dictionary<uint, SettingMeta> settingMetaCache = new Dictionary<uint, SettingMeta>();

	public DrsSettingsMetaService(CustomSettingNames customSettings, CustomSettingNames referenceSettings = null)
	{
		_customSettings = customSettings;
		_referenceSettings = referenceSettings;
		ResetMetaCache(initOnly: true);
	}

	public void ResetMetaCache(bool initOnly = false)
	{
		settingMetaCache = new Dictionary<uint, SettingMeta>();
		MetaServices = new List<MetaServiceItem>();
		CustomMeta = new CustomSettingMetaService(_customSettings);
		MetaServices.Add(new MetaServiceItem
		{
			ValueNamePrio = 1u,
			Service = CustomMeta
		});
		if (!initOnly)
		{
			DriverMeta = new DriverSettingMetaService();
			MetaServices.Add(new MetaServiceItem
			{
				ValueNamePrio = 5u,
				Service = DriverMeta
			});
			ConstantMeta = new ConstantSettingMetaService();
			MetaServices.Add(new MetaServiceItem
			{
				ValueNamePrio = 2u,
				Service = ConstantMeta
			});
			if (DrsServiceLocator.ScannerService != null)
			{
				ScannedMeta = new ScannedSettingMetaService(DrsServiceLocator.ScannerService.CachedSettings);
				MetaServices.Add(new MetaServiceItem
				{
					ValueNamePrio = 3u,
					Service = ScannedMeta
				});
			}
			if (_referenceSettings != null)
			{
				ReferenceMeta = new CustomSettingMetaService(_referenceSettings, SettingMetaSource.ReferenceSettings);
				MetaServices.Add(new MetaServiceItem
				{
					ValueNamePrio = 4u,
					Service = ReferenceMeta
				});
			}
		}
	}

	private NVDRS_SETTING_TYPE? GetSettingValueType(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			NVDRS_SETTING_TYPE? settingValueType = item.Service.GetSettingValueType(settingId);
			if (settingValueType.HasValue)
			{
				return settingValueType.Value;
			}
		}
		return NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE;
	}

	private string GetSettingName(uint settingId)
	{
		string result = null;
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			string settingName = item.Service.GetSettingName(settingId);
			if (!string.IsNullOrEmpty(settingName))
			{
				if (!settingName.StartsWith("0x"))
				{
					return settingName;
				}
				result = settingName;
			}
		}
		return result;
	}

	private string GetGroupName(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			string groupName = item.Service.GetGroupName(settingId);
			if (groupName != null)
			{
				return groupName;
			}
		}
		return null;
	}

	private string GetAlternateNames(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			string alternateNames = item.Service.GetAlternateNames(settingId);
			if (alternateNames != null)
			{
				return alternateNames;
			}
		}
		return null;
	}

	private uint GetDwordDefaultValue(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			uint? dwordDefaultValue = item.Service.GetDwordDefaultValue(settingId);
			if (dwordDefaultValue.HasValue)
			{
				return dwordDefaultValue.Value;
			}
		}
		return 0u;
	}

	private string GetStringDefaultValue(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			string stringDefaultValue = item.Service.GetStringDefaultValue(settingId);
			if (stringDefaultValue != null)
			{
				return stringDefaultValue;
			}
		}
		return null;
	}

	private byte[] GetBinaryDefaultValue(uint settingId)
	{
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			byte[] binaryDefaultValue = item.Service.GetBinaryDefaultValue(settingId);
			if (binaryDefaultValue != null)
			{
				return binaryDefaultValue;
			}
		}
		return null;
	}

	private List<SettingValue<T>> MergeSettingValues<T>(List<SettingValue<T>> a, List<SettingValue<T>> b)
	{
		if (b == null)
		{
			return a;
		}
		if (b.Count > 0 && b.First().ValueSource == SettingMetaSource.ScannedSettings)
		{
			a.AddRange(b);
		}
		else
		{
			List<T> currentNonScannedValues = (from xa in a
				where xa.ValueSource != SettingMetaSource.ScannedSettings
				select xa.Value).ToList();
			List<SettingValue<T>> collection = b.Where((SettingValue<T> xb) => !currentNonScannedValues.Contains(xb.Value)).ToList();
			a.AddRange(collection);
			foreach (SettingValue<T> settingValue in a)
			{
				SettingValue<T> settingValue2 = b.FirstOrDefault((SettingValue<T> x) => x.Value.Equals(settingValue.Value) && settingValue.ValueSource != SettingMetaSource.ScannedSettings);
				if (settingValue2 != null && settingValue2.ValueName != null)
				{
					settingValue.ValueName = settingValue2.ValueName;
					settingValue.ValueSource = settingValue2.ValueSource;
					settingValue.ValuePos = settingValue2.ValuePos;
				}
			}
		}
		SettingValue<T> settingValue3 = a.FirstOrDefault();
		if (settingValue3 != null && settingValue3 is IComparable)
		{
			return a.OrderBy((SettingValue<T> x) => x.Value).ToList();
		}
		return a.ToList();
	}

	private List<SettingValue<byte[]>> GetBinaryValues(uint settingId)
	{
		List<SettingValue<byte[]>> list = new List<SettingValue<byte[]>>();
		foreach (MetaServiceItem item in MetaServices.OrderByDescending((MetaServiceItem x) => x.ValueNamePrio))
		{
			list = MergeSettingValues(list, item.Service.GetBinaryValues(settingId));
		}
		return list;
	}

	private List<SettingValue<string>> GetStringValues(uint settingId)
	{
		List<SettingValue<string>> list = new List<SettingValue<string>>();
		foreach (MetaServiceItem item in MetaServices.OrderByDescending((MetaServiceItem x) => x.ValueNamePrio))
		{
			list = MergeSettingValues(list, item.Service.GetStringValues(settingId));
		}
		return list;
	}

	private List<SettingValue<uint>> GetDwordValues(uint settingId)
	{
		List<SettingValue<uint>> list = new List<SettingValue<uint>>();
		foreach (MetaServiceItem item in MetaServices.OrderByDescending((MetaServiceItem x) => x.ValueNamePrio))
		{
			list = MergeSettingValues(list, item.Service.GetDwordValues(settingId));
		}
		if (list != null)
		{
			list = (from v in list
				where !v.ValueName.EndsWith("_NUM") && !v.ValueName.EndsWith("_MASK") && (!v.ValueName.EndsWith("_MIN") || v.ValueName.Equals("PREFERRED_PSTATE_PREFER_MIN")) && (!v.ValueName.EndsWith("_MAX") || v.ValueName.Equals("PREFERRED_PSTATE_PREFER_MAX"))
				group v by v.ValueName into g
				select g.First((SettingValue<uint> t) => t.ValueName == g.Key) into v
				orderby v.ValueSource, v.ValuePos, v.ValueName
				select v).ToList();
		}
		return list;
	}

	public List<uint> GetSettingIds(SettingViewMode viewMode)
	{
		List<uint> list = new List<uint>();
		SettingMetaSource[] allowedSettingIdMetaSourcesForViewMode = GetAllowedSettingIdMetaSourcesForViewMode(viewMode);
		foreach (MetaServiceItem item in MetaServices.OrderBy((MetaServiceItem x) => x.Service.Source))
		{
			if (Enumerable.Contains(allowedSettingIdMetaSourcesForViewMode, item.Service.Source))
			{
				list.AddRange(item.Service.GetSettingIds());
			}
		}
		return list.Distinct().ToList();
	}

	private SettingMetaSource[] GetAllowedSettingIdMetaSourcesForViewMode(SettingViewMode viewMode)
	{
		return viewMode switch
		{
			SettingViewMode.CustomSettingsOnly => new SettingMetaSource[1] { SettingMetaSource.CustomSettings }, 
			SettingViewMode.IncludeScannedSetttings => new SettingMetaSource[5]
			{
				SettingMetaSource.ConstantSettings,
				SettingMetaSource.ScannedSettings,
				SettingMetaSource.CustomSettings,
				SettingMetaSource.DriverSettings,
				SettingMetaSource.ReferenceSettings
			}, 
			_ => new SettingMetaSource[2]
			{
				SettingMetaSource.CustomSettings,
				SettingMetaSource.DriverSettings
			}, 
		};
	}

	private SettingMetaSource[] GetAllowedSettingValueMetaSourcesForViewMode(SettingViewMode viewMode)
	{
		if (viewMode == SettingViewMode.CustomSettingsOnly)
		{
			return new SettingMetaSource[2]
			{
				SettingMetaSource.CustomSettings,
				SettingMetaSource.ScannedSettings
			};
		}
		return new SettingMetaSource[5]
		{
			SettingMetaSource.ConstantSettings,
			SettingMetaSource.ScannedSettings,
			SettingMetaSource.CustomSettings,
			SettingMetaSource.DriverSettings,
			SettingMetaSource.ReferenceSettings
		};
	}

	private SettingMeta CreateSettingMeta(uint settingId)
	{
		NVDRS_SETTING_TYPE? settingValueType = GetSettingValueType(settingId);
		string settingName = GetSettingName(settingId);
		string text = GetGroupName(settingId);
		if (text == null)
		{
			text = GetLegacyGroupName(settingId, settingName);
		}
		return new SettingMeta
		{
			SettingType = settingValueType,
			SettingName = settingName,
			GroupName = text,
			AlternateNames = GetAlternateNames(settingId),
			IsApiExposed = GetIsApiExposed(settingId),
			IsSettingHidden = GetIsSettingHidden(settingId),
			Description = GetDescription(settingId),
			DefaultDwordValue = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE) ? GetDwordDefaultValue(settingId) : 0u),
			DefaultStringValue = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE) ? GetStringDefaultValue(settingId) : null),
			DefaultBinaryValue = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE) ? GetBinaryDefaultValue(settingId) : null),
			DwordValues = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE) ? GetDwordValues(settingId) : null),
			StringValues = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE) ? GetStringValues(settingId) : null),
			BinaryValues = ((settingValueType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE) ? GetBinaryValues(settingId) : null)
		};
	}

	private SettingMeta PostProcessMeta(uint settingId, SettingMeta settingMeta, SettingViewMode viewMode)
	{
		SettingMeta settingMeta2 = new SettingMeta
		{
			DefaultDwordValue = settingMeta.DefaultDwordValue,
			DefaultStringValue = settingMeta.DefaultStringValue,
			DefaultBinaryValue = settingMeta.DefaultBinaryValue,
			SettingName = settingMeta.SettingName,
			SettingType = settingMeta.SettingType,
			GroupName = settingMeta.GroupName,
			AlternateNames = settingMeta.AlternateNames,
			IsApiExposed = settingMeta.IsApiExposed,
			IsSettingHidden = settingMeta.IsSettingHidden,
			Description = settingMeta.Description
		};
		if (string.IsNullOrEmpty(settingMeta2.SettingName))
		{
			settingMeta2.SettingName = $"0x{settingId:X8}";
		}
		SettingMetaSource[] allowedSourcesForViewMode = GetAllowedSettingValueMetaSourcesForViewMode(viewMode);
		if (settingMeta.DwordValues != null)
		{
			settingMeta2.DwordValues = settingMeta.DwordValues.Where((SettingValue<uint> x) => Enumerable.Contains(allowedSourcesForViewMode, x.ValueSource)).ToList();
		}
		if (settingMeta.StringValues != null)
		{
			settingMeta2.StringValues = settingMeta.StringValues.Where((SettingValue<string> x) => Enumerable.Contains(allowedSourcesForViewMode, x.ValueSource)).ToList();
		}
		if (settingMeta.BinaryValues != null)
		{
			settingMeta2.BinaryValues = settingMeta.BinaryValues.Where((SettingValue<byte[]> x) => Enumerable.Contains(allowedSourcesForViewMode, x.ValueSource)).ToList();
		}
		return settingMeta2;
	}

	public SettingMeta GetSettingMeta(uint settingId, SettingViewMode viewMode = SettingViewMode.Normal)
	{
		if (settingMetaCache.ContainsKey(settingId))
		{
			return PostProcessMeta(settingId, settingMetaCache[settingId], viewMode);
		}
		SettingMeta settingMeta = CreateSettingMeta(settingId);
		settingMetaCache.Add(settingId, settingMeta);
		return PostProcessMeta(settingId, settingMeta, viewMode);
	}

	private string GetLegacyGroupName(uint settingId, string settingName)
	{
		if (settingName == null)
		{
			return null;
		}
		if (settingName.ToUpper().Contains("SLI"))
		{
			return "6 - SLI";
		}
		if (settingName.ToUpper().Contains("STEREO"))
		{
			return "7 - Stereo";
		}
		if (settingName.StartsWith("0x"))
		{
			return "Unknown";
		}
		return "Other";
	}

	private bool GetIsApiExposed(uint settingId)
	{
		return MetaServices.FirstOrDefault((MetaServiceItem m) => m.Service.Source == SettingMetaSource.DriverSettings)?.Service.GetSettingIds().Contains(settingId) ?? false;
	}

	private bool GetIsSettingHidden(uint settingId)
	{
		MetaServiceItem metaServiceItem = MetaServices.FirstOrDefault((MetaServiceItem m) => m.Service.Source == SettingMetaSource.CustomSettings);
		MetaServiceItem metaServiceItem2 = MetaServices.FirstOrDefault((MetaServiceItem m) => m.Service.Source == SettingMetaSource.ReferenceSettings);
		return (metaServiceItem != null && ((CustomSettingMetaService)metaServiceItem.Service).IsSettingHidden(settingId)) || (metaServiceItem2 != null && ((CustomSettingMetaService)metaServiceItem2.Service).IsSettingHidden(settingId));
	}

	private string GetDescription(uint settingId)
	{
		MetaServiceItem metaServiceItem = MetaServices.FirstOrDefault((MetaServiceItem m) => m.Service.Source == SettingMetaSource.CustomSettings);
		string text = ((metaServiceItem != null) ? (((CustomSettingMetaService)metaServiceItem.Service).GetDescription(settingId) ?? "") : "");
		MetaServiceItem metaServiceItem2 = MetaServices.FirstOrDefault((MetaServiceItem m) => m.Service.Source == SettingMetaSource.ReferenceSettings);
		string text2 = ((metaServiceItem2 != null) ? (((CustomSettingMetaService)metaServiceItem2.Service).GetDescription(settingId) ?? "") : "");
		return (!string.IsNullOrEmpty(text)) ? text : text2;
	}
}
