using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class AppInfoExtractor
{
	internal string PackageName;

	internal string AppName;

	internal string ActivityName;

	internal static AppInfoExtractor GetApkInfo(string apkFile)
	{
		AppInfoExtractor appInfoExtractor = new AppInfoExtractor();
		try
		{
			string input = string.Empty;
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
				process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "hd-aapt.exe");
				process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "dump badging \"{0}\"", new object[1] { apkFile });
				process.Start();
				input = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
			}
			Match match = new Regex("package:\\sname='(.+?)'").Match(input);
			appInfoExtractor.PackageName = match.Groups[1].Value;
			if (!string.IsNullOrEmpty(appInfoExtractor.PackageName))
			{
				match = new Regex("application:\\slabel='(.+)'\\sicon='(.+?)'").Match(input);
				appInfoExtractor.AppName = match.Groups[1].Value;
				appInfoExtractor.AppName = Regex.Replace(appInfoExtractor.AppName, "[\\x22\\\\\\/:*?|<>]", "");
				match.Groups[2].Value.Replace("/", "\\");
				match = new Regex("launchable\\sactivity\\sname='(.+?)'").Match(input);
				appInfoExtractor.ActivityName = match.Groups[1].Value;
			}
		}
		catch
		{
			Logger.Error("Error getting file info");
		}
		return appInfoExtractor;
	}
}
