using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace nspector.Common.Helper;

public class SteamAppResolver
{
	public const string SteamExeName = "steam.exe";

	public const string SteamUrlPattern = "steam://rungameid/";

	public const string SteamArgumentPattern = "-applaunch";

	private byte[] _appinfoBytes;

	public SteamAppResolver()
	{
		string steamAppInfoLocation = GetSteamAppInfoLocation();
		if (File.Exists(steamAppInfoLocation))
		{
			_appinfoBytes = File.ReadAllBytes(steamAppInfoLocation);
		}
		else
		{
			_appinfoBytes = null;
		}
	}

	private string GetSteamAppInfoLocation()
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam", writable: false);
		if (registryKey != null)
		{
			string text = (string)registryKey.GetValue("SteamPath", null);
			if (text != null)
			{
				return Path.Combine(text, "appcache\\appinfo.vdf");
			}
		}
		return "";
	}

	public string ResolveExeFromSteamUrl(string url)
	{
		if (url.StartsWith("steam://rungameid/"))
		{
			string s = url.Substring("steam://rungameid/".Length);
			int result = 0;
			if (int.TryParse(s, out result))
			{
				return FindCommonExecutableForApp(result);
			}
		}
		return "";
	}

	public string ResolveExeFromSteamArguments(string arguments)
	{
		if (arguments.Contains("-applaunch"))
		{
			Regex regex = new Regex("-applaunch\\s+(?<appid>\\d+)");
			foreach (Match item in regex.Matches(arguments))
			{
				string s = item.Result("${appid}");
				int result = 0;
				if (int.TryParse(s, out result))
				{
					return FindCommonExecutableForApp(result);
				}
			}
		}
		return "";
	}

	private string FindCommonExecutableForApp(int appid)
	{
		List<string> list = FindAllExecutablesForApp(appid);
		if (list.Count > 0)
		{
			return new FileInfo(list[0]).Name;
		}
		return "";
	}

	private List<string> FindAllExecutablesForApp(int appid)
	{
		if (_appinfoBytes == null)
		{
			return new List<string>();
		}
		byte[] bytes = BitConverter.GetBytes(appid);
		int offset = 0;
		byte[] array = new byte[5]
		{
			8,
			bytes[0],
			bytes[1],
			bytes[2],
			bytes[3]
		};
		byte[] pattern = new byte[8] { 0, 108, 97, 117, 110, 99, 104, 0 };
		int num = FindOffset(_appinfoBytes, array, offset);
		if (num == -1)
		{
			return new List<string>();
		}
		offset = num + array.Length;
		int num2 = FindOffset(_appinfoBytes, pattern, offset);
		if (num2 == -1)
		{
			return new List<string>();
		}
		offset = num2;
		List<string> executables = new List<string>();
		FindExecutables(_appinfoBytes, ref offset, ref executables);
		return executables;
	}

	private void FindExecutables(byte[] bytes, ref int offset, ref List<string> executables)
	{
		while (true)
		{
			byte b = ReadByte(bytes, ref offset);
			if (b == 8)
			{
				break;
			}
			string text = ReadCString(bytes, ref offset);
			string text2 = "";
			switch (b)
			{
			case 0:
				FindExecutables(bytes, ref offset, ref executables);
				break;
			case 1:
				text2 = ReadCString(bytes, ref offset);
				if (text == "executable" && text2.EndsWith(".exe"))
				{
					executables.Add(text2);
				}
				break;
			case 2:
				offset += 4;
				break;
			case 7:
				offset += 8;
				break;
			}
		}
	}

	private static int FindOffset(byte[] bytes, byte[] pattern, int offset = 0, byte? wildcard = null)
	{
		for (int i = offset; i < bytes.Length; i++)
		{
			if (pattern[0] != bytes[i] || bytes.Length - i < pattern.Length)
			{
				continue;
			}
			bool flag = true;
			for (int j = 1; j < pattern.Length; j++)
			{
				if (!flag)
				{
					break;
				}
				if (bytes[i + j] != pattern[j] && ((wildcard.HasValue && wildcard != pattern[j]) || !wildcard.HasValue))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	private static byte ReadByte(byte[] bytes, ref int offset)
	{
		offset++;
		return bytes[offset - 1];
	}

	private static string ReadCString(byte[] bytes, ref int offset)
	{
		int i;
		for (i = offset; bytes[i] != 0; i++)
		{
		}
		int index = offset;
		int num = i - offset;
		offset += num + 1;
		return Encoding.UTF8.GetString(bytes, index, num);
	}
}
