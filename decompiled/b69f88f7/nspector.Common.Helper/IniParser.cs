using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nspector.Common.Helper;

public class IniParser
{
	public Dictionary<string, Dictionary<string, string>> Data { get; } = new Dictionary<string, Dictionary<string, string>>();

	public void Load(string filePath)
	{
		using StreamReader streamReader = new StreamReader(filePath);
		string text = null;
		string text2;
		while ((text2 = streamReader.ReadLine()) != null)
		{
			text2 = text2.Trim();
			if (string.IsNullOrEmpty(text2) || text2.StartsWith(";") || text2.StartsWith("#"))
			{
				continue;
			}
			if (text2.StartsWith("[") && text2.EndsWith("]"))
			{
				text = text2.Substring(1, text2.Length - 2).Trim();
				if (!Data.ContainsKey(text))
				{
					Data[text] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				}
			}
			else if (text != null && Enumerable.Contains(text2, '='))
			{
				int num = text2.IndexOf('=');
				string key = text2.Substring(0, num).Trim();
				string value = text2.Substring(num + 1).Trim();
				Data[text][key] = value;
			}
		}
	}

	public string? GetValue(string section, string key)
	{
		if (Data.TryGetValue(section, out var value) && value.TryGetValue(key, out var value2))
		{
			return value2;
		}
		return null;
	}

	public List<string> GetSections()
	{
		return Data.Keys.ToList();
	}
}
