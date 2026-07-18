using System;
using System.Globalization;
using System.Linq;
using nspector.Common.Meta;

namespace nspector.Common;

public static class DrsUtil
{
	public static string StringValueRaw = "Text";

	public static string GetDwordString(uint dword)
	{
		return $"0x{dword:X8}";
	}

	public static uint ParseDwordByInputSafe(string input)
	{
		uint result = 0u;
		if (input.ToLowerInvariant().StartsWith("0x"))
		{
			try
			{
				int num = input.IndexOf(' ');
				int length = ((num > 2) ? (num - 2) : (input.Length - 2));
				result = uint.Parse(input.Substring(2, length), NumberStyles.AllowHexSpecifier);
			}
			catch
			{
			}
		}
		else
		{
			try
			{
				result = uint.Parse(input);
			}
			catch
			{
			}
		}
		return result;
	}

	internal static uint ParseDwordSettingValue(SettingMeta meta, string text)
	{
		return meta.DwordValues.FirstOrDefault((SettingValue<uint> x) => x.ValueName != null && x.ValueName.Equals(text))?.Value ?? ParseDwordByInputSafe(text);
	}

	internal static string GetDwordSettingValueName(SettingMeta meta, uint dwordValue)
	{
		SettingValue<uint> settingValue = meta.DwordValues.FirstOrDefault((SettingValue<uint> x) => x.Value.Equals(dwordValue));
		return (settingValue == null) ? GetDwordString(dwordValue) : settingValue.ValueName;
	}

	internal static string ParseStringSettingValue(SettingMeta meta, string text)
	{
		SettingValue<string> settingValue = meta.StringValues?.FirstOrDefault((SettingValue<string> x) => x.ValueName != null && x.ValueName.Equals(text));
		if (settingValue != null)
		{
			return settingValue.Value;
		}
		return text;
	}

	internal static string GetStringSettingValueName(SettingMeta meta, string stringValue)
	{
		SettingValue<string> settingValue = meta.StringValues.FirstOrDefault((SettingValue<string> x) => x.Value.Equals(stringValue));
		return (settingValue == null) ? stringValue : settingValue.ValueName;
	}

	public static string GetBinaryString(byte[] binaryValue)
	{
		if (binaryValue == null)
		{
			return "";
		}
		if (binaryValue.Length == 8)
		{
			return $"0x{BitConverter.ToUInt64(binaryValue, 0):X16}";
		}
		return BitConverter.ToString(binaryValue);
	}

	internal static string GetBinarySettingValueName(SettingMeta meta, byte[] binaryValue)
	{
		SettingValue<byte[]> settingValue = meta.BinaryValues?.FirstOrDefault((SettingValue<byte[]> x) => x.Value.Equals(binaryValue));
		return (settingValue == null) ? GetBinaryString(binaryValue) : settingValue.ValueName;
	}

	internal static byte[] ParseBinarySettingValue(SettingMeta meta, string text)
	{
		SettingValue<byte[]> settingValue = meta.BinaryValues.FirstOrDefault((SettingValue<byte[]> x) => x.ValueName != null && x.ValueName.Equals(text));
		if (settingValue != null)
		{
			return settingValue.Value;
		}
		return ParseBinaryByInputSafe(text);
	}

	public static byte[] ParseBinaryByInputSafe(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}
		if (input.StartsWith("0x"))
		{
			int num = input.IndexOf(' ');
			int length = ((num > 2) ? (num - 2) : (input.Length - 2));
			ulong value = ulong.Parse(input.Substring(2, length), NumberStyles.AllowHexSpecifier);
			return BitConverter.GetBytes(value);
		}
		if (input.Contains("-"))
		{
			return Array.ConvertAll(input.Split(new char[1] { '-' }), (string s) => Convert.ToByte(s, 16));
		}
		return null;
	}
}
