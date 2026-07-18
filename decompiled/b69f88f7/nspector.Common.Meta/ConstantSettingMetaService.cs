using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nspector.Native.NVAPI2;
using nspector.Native.NvApi.DriverSettings;

namespace nspector.Common.Meta;

internal class ConstantSettingMetaService : ISettingMetaService
{
	private readonly Dictionary<ESetting, Type> settingEnumTypeCache;

	private string[] ignoreSettingNames = new string[4] { "TOTAL_DWORD_SETTING_NUM", "TOTAL_WSTRING_SETTING_NUM", "TOTAL_SETTING_NUM", "INVALID_SETTING_ID" };

	private HashSet<uint> settingIds;

	public SettingMetaSource Source => SettingMetaSource.ConstantSettings;

	public ConstantSettingMetaService()
	{
		settingEnumTypeCache = CreateSettingEnumTypeCache();
		GetSettingIds();
	}

	private Dictionary<ESetting, Type> CreateSettingEnumTypeCache()
	{
		Dictionary<ESetting, Type> dictionary = new Dictionary<ESetting, Type>();
		List<Type> source = (from t in Assembly.GetExecutingAssembly().GetTypes()
			where t.Namespace == "nspector.Native.NvApi.DriverSettings" && t.IsEnum && t.Name.StartsWith("EValues_")
			select t).ToList();
		List<string> list = Enum.GetNames(typeof(ESetting)).Distinct().ToList();
		foreach (string settingIdName in list)
		{
			if (!Enumerable.Contains(ignoreSettingNames, settingIdName))
			{
				Type type = source.FirstOrDefault((Type x) => settingIdName.Substring(0, settingIdName.Length - 3).Equals(x.Name.Substring(8)));
				if (type != null)
				{
					ESetting key = (ESetting)Enum.Parse(typeof(ESetting), settingIdName);
					dictionary.Add(key, type);
				}
			}
		}
		return dictionary;
	}

	public Type GetSettingEnumType(uint settingId)
	{
		if (settingEnumTypeCache.ContainsKey((ESetting)settingId))
		{
			return settingEnumTypeCache[(ESetting)settingId];
		}
		return null;
	}

	public NVDRS_SETTING_TYPE? GetSettingValueType(uint settingId)
	{
		return null;
	}

	public string GetSettingName(uint settingId)
	{
		if (settingIds.Contains(settingId))
		{
			ESetting eSetting = (ESetting)settingId;
			return eSetting.ToString();
		}
		return null;
	}

	public uint? GetDwordDefaultValue(uint settingId)
	{
		if (settingEnumTypeCache.ContainsKey((ESetting)settingId))
		{
			Type enumType = settingEnumTypeCache[(ESetting)settingId];
			string text = Enum.GetNames(enumType).FirstOrDefault((string x) => x.EndsWith("_DEFAULT"));
			if (text != null)
			{
				return (uint)Enum.Parse(enumType, text);
			}
		}
		return null;
	}

	public string GetStringDefaultValue(uint settingId)
	{
		return null;
	}

	public List<SettingValue<string>> GetStringValues(uint settingId)
	{
		return null;
	}

	private uint ParseEnumValue(Type enumType, string enumText)
	{
		try
		{
			return (uint)Enum.Parse(enumType, enumText);
		}
		catch (InvalidCastException)
		{
			int value = (int)Enum.Parse(enumType, enumText);
			byte[] bytes = BitConverter.GetBytes(value);
			return BitConverter.ToUInt32(bytes, 0);
		}
	}

	public List<SettingValue<uint>> GetDwordValues(uint settingId)
	{
		if (settingEnumTypeCache.ContainsKey((ESetting)settingId))
		{
			Type enumType = settingEnumTypeCache[(ESetting)settingId];
			List<string> source = (from x in Enum.GetNames(enumType)
				where !x.EndsWith("_DEFAULT") && !x.EndsWith("_NUM_VALUES")
				select x).ToList();
			return source.Select((string x) => new SettingValue<uint>(Source)
			{
				Value = ParseEnumValue(enumType, x),
				ValueName = DrsUtil.GetDwordString(ParseEnumValue(enumType, x)) + " " + x
			}).ToList();
		}
		return null;
	}

	public List<uint> GetSettingIds()
	{
		if (settingIds == null)
		{
			settingIds = new HashSet<uint>((from ESetting x in Enum.GetValues(typeof(ESetting))
				where !Enumerable.Contains(ignoreSettingNames, x.ToString())
				select x).Cast<uint>().Distinct().ToList());
		}
		return settingIds.ToList();
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
		return null;
	}

	public List<SettingValue<byte[]>> GetBinaryValues(uint settingId)
	{
		return null;
	}
}
