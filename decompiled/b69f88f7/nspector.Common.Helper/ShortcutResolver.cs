using System.IO;
using nspector.Native.WINAPI;

namespace nspector.Common.Helper;

public class ShortcutResolver
{
	public static string GetUrlFromInternetShortcut(string filePath)
	{
		string[] array = File.ReadAllLines(filePath);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.StartsWith("URL="))
			{
				string[] array3 = text.Split(new char[1] { '=' });
				if (array3.Length != 0)
				{
					return array3[1];
				}
			}
		}
		return "";
	}

	public static string ResolveExecuteable(string filename, out string profileName)
	{
		FileInfo fileInfo = new FileInfo(filename);
		profileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
		try
		{
			return fileInfo.Extension.ToLowerInvariant() switch
			{
				".lnk" => ResolveFromShellLinkFile(fileInfo.FullName), 
				".url" => ResolveFromUrlFile(fileInfo.FullName), 
				".exe" => fileInfo.Name, 
				_ => "", 
			};
		}
		catch
		{
			return "";
		}
	}

	private static string ResolveFromShellLinkFile(string filename)
	{
		ShellLink shellLink = new ShellLink(filename);
		if (shellLink.Arguments.StartsWith("steam://rungameid/"))
		{
			SteamAppResolver steamAppResolver = new SteamAppResolver();
			return steamAppResolver.ResolveExeFromSteamUrl(shellLink.Arguments);
		}
		FileInfo fileInfo = new FileInfo(shellLink.Target);
		if (fileInfo.Name.ToLowerInvariant() == "steam.exe" && shellLink.Arguments.Contains("-applaunch"))
		{
			SteamAppResolver steamAppResolver2 = new SteamAppResolver();
			return steamAppResolver2.ResolveExeFromSteamArguments(shellLink.Arguments);
		}
		if (fileInfo.Extension.ToLowerInvariant().Equals(".exe"))
		{
			return fileInfo.Name;
		}
		return "";
	}

	private static string ResolveFromUrlFile(string filename)
	{
		string urlFromInternetShortcut = GetUrlFromInternetShortcut(filename);
		if (urlFromInternetShortcut.StartsWith("steam://rungameid/"))
		{
			SteamAppResolver steamAppResolver = new SteamAppResolver();
			return steamAppResolver.ResolveExeFromSteamUrl(urlFromInternetShortcut);
		}
		return "";
	}
}
