using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nspector.Common.Helper;

public static class GithubVersionHelper
{
	private const string _repoUrl = "https://api.github.com/repos/Orbmu2k/nvidiaProfileInspector/releases/latest";

	public static async Task<bool> IsUpdateAvailableAsync()
	{
		try
		{
			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			using HttpClient httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromSeconds(10.0);
			httpClient.DefaultRequestHeaders.Add("User-Agent", "nvidiaProfileInspector/" + currentVersion.ToString());
			HttpResponseMessage response = await httpClient.GetAsync("https://api.github.com/repos/Orbmu2k/nvidiaProfileInspector/releases/latest");
			if (!response.IsSuccessStatusCode)
			{
				return false;
			}
			string tagName = ExtractJsonString(await response.Content.ReadAsStringAsync(), "tag_name");
			if (string.IsNullOrEmpty(tagName))
			{
				return false;
			}
			string versionString = tagName.TrimStart(new char[1] { 'v' }).Trim();
			if (Version.TryParse(versionString, out Version latestVersion))
			{
				return latestVersion > currentVersion;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private static string ExtractJsonString(string json, string fieldName)
	{
		string pattern = "\"" + fieldName + "\"\\s*:\\s*\"([^\"\\\\]*(\\\\.[^\"\\\\]*)*)\"";
		Match match = Regex.Match(json, pattern);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			value = value.Replace("\\\"", "\"");
			value = value.Replace("\\\\", "\\");
			value = value.Replace("\\n", "\n");
			value = value.Replace("\\r", "\r");
			return value.Replace("\\t", "\t");
		}
		return null;
	}
}
