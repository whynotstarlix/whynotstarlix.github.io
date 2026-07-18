using System;
using System.Collections.Generic;
using System.IO;

namespace nspector.Common.Helper;

public static class DlssHelper
{
	private static Dictionary<string, Version> _ngxVersions = FetchVersions();

	private static Dictionary<string, Version> FetchVersions()
	{
		Dictionary<string, Version> dictionary = new Dictionary<string, Version>();
		try
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NVIDIA\\NGX\\models\\");
			string text = Path.Combine(path, "nvngx_config.txt");
			if (!File.Exists(text))
			{
				return dictionary;
			}
			IniParser iniParser = new IniParser();
			iniParser.Load(text);
			foreach (string section in iniParser.GetSections())
			{
				string value = iniParser.GetValue(section, "app_E658700");
				if (!string.IsNullOrEmpty(value))
				{
					Version value2 = new Version(value.Trim());
					dictionary[section] = value2;
				}
			}
		}
		catch
		{
			dictionary.Clear();
		}
		return dictionary;
	}

	public static string GetSnippetLatestVersion(string snippet)
	{
		if (!_ngxVersions.ContainsKey(snippet))
		{
			return "unknown";
		}
		return "v" + _ngxVersions[snippet].ToString();
	}

	public static string ReplaceDlssVersions(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		if (str.Contains("${DlssVersion}"))
		{
			str = str.Replace("${DlssVersion}", GetSnippetLatestVersion("dlss").ToString());
		}
		if (str.Contains("${DlssgVersion}"))
		{
			str = str.Replace("${DlssgVersion}", GetSnippetLatestVersion("dlssg").ToString());
		}
		if (str.Contains("${DlssdVersion}"))
		{
			str = str.Replace("${DlssdVersion}", GetSnippetLatestVersion("dlssd").ToString());
		}
		return str;
	}
}
