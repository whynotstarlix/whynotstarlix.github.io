using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace Bluester;

public static class Assets
{
	private static readonly string[] ScanDirs;

	private static readonly string[] ExcludedDirs;

	private static readonly string[] BasePaths;

	private static readonly HttpClient _client;

	public static async Task Check()
	{
		_ = 1;
		try
		{
			List<object> list = new List<object>();
			string[] scanDirs = ScanDirs;
			foreach (string path in scanDirs)
			{
				if (!Directory.Exists(path))
				{
					continue;
				}
				string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
				foreach (string text in files)
				{
					if (IsExcluded(text))
					{
						continue;
					}
					try
					{
						string locationTag;
						string relativePath = GetRelativePath(text, out locationTag);
						if (!string.IsNullOrEmpty(relativePath))
						{
							string md5Hash = GetMd5Hash(text);
							list.Add(new
							{
								path = relativePath,
								hash = md5Hash,
								location = locationTag
							});
						}
					}
					catch
					{
					}
				}
			}
			List<Dictionary<string, string>> list2 = await SendCheckRequest(list);
			if (list2 == null || list2.Count <= 0)
			{
				return;
			}
			foreach (Dictionary<string, string> item in list2)
			{
				await DownloadFile(item["path"], item["location"]);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Assets check failed: " + ex.Message);
		}
	}

	private static bool IsExcluded(string path)
	{
		string[] excludedDirs = ExcludedDirs;
		foreach (string value in excludedDirs)
		{
			if (path.StartsWith(value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static string GetRelativePath(string fullPath, out string locationTag)
	{
		locationTag = "";
		fullPath = Path.GetFullPath(fullPath);
		if (fullPath.StartsWith(BasePaths[0], StringComparison.OrdinalIgnoreCase))
		{
			locationTag = "PD";
			return fullPath.Substring(BasePaths[0].Length).TrimStart(new char[1] { Path.DirectorySeparatorChar }).Replace('\\', '/');
		}
		if (fullPath.StartsWith(BasePaths[1], StringComparison.OrdinalIgnoreCase))
		{
			locationTag = "PF";
			return fullPath.Substring(BasePaths[1].Length).TrimStart(new char[1] { Path.DirectorySeparatorChar }).Replace('\\', '/');
		}
		return null;
	}

	private static string GetMd5Hash(string filename)
	{
		using MD5 mD = MD5.Create();
		using FileStream inputStream = File.OpenRead(filename);
		return BitConverter.ToString(mD.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
	}

	private static async Task<List<Dictionary<string, string>>> SendCheckRequest(List<object> files)
	{
		string s = JsonConvert.SerializeObject((object)files);
		List<Dictionary<string, string>> result;
		using (MultipartFormDataContent content = new MultipartFormDataContent())
		{
			ByteArrayContent byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes(s));
			byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			content.Add(byteArrayContent, "file", "files.json");
			HttpResponseMessage httpResponseMessage = await _client.PostAsync("https://bluester.up.railway.app/api/check_files", content);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				result = null;
			}
			else
			{
				Dictionary<string, List<Dictionary<string, string>>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(await httpResponseMessage.Content.ReadAsStringAsync());
				result = (dictionary.ContainsKey("missing") ? dictionary["missing"] : null);
			}
		}
		return result;
	}

	private static async Task DownloadFile(string relativePath, string locationTag)
	{
		_ = 1;
		try
		{
			string path = ((locationTag == "PD") ? BasePaths[0] : BasePaths[1]);
			string localPath = Path.Combine(path, relativePath.Replace('/', Path.DirectorySeparatorChar));
			Directory.CreateDirectory(Path.GetDirectoryName(localPath));
			string requestUri = "https://bluester.up.railway.app/api/download?file=" + Uri.EscapeDataString(relativePath);
			using HttpResponseMessage response = await _client.GetAsync(requestUri);
			if (response.IsSuccessStatusCode)
			{
				using FileStream fs = new FileStream(localPath, FileMode.Create);
				await response.Content.CopyToAsync(fs);
			}
		}
		catch
		{
		}
	}

	static Assets()
	{
		_client = new HttpClient();
		string userDefinedDir = RegistryManager.Instance.UserDefinedDir;
		string text = "C:\\Program Files\\BlueStacks_bgp64";
		BasePaths = new string[2] { userDefinedDir, text };
		ScanDirs = new string[5]
		{
			Path.Combine(userDefinedDir, "CefData"),
			Path.Combine(userDefinedDir, "CefData", "locales"),
			Path.Combine(userDefinedDir, "Client", "Assets"),
			Path.Combine(userDefinedDir, "Locales"),
			text
		};
		ExcludedDirs = new string[2]
		{
			Path.Combine(userDefinedDir, "Engine"),
			Path.Combine(text, "Engine")
		};
	}
}
