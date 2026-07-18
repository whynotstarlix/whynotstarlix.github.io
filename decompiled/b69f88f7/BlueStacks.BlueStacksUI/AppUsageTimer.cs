using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public static class AppUsageTimer
{
	internal static Dictionary<string, Dictionary<string, long>> sDictAppUsageInfo = new Dictionary<string, Dictionary<string, long>>();

	private static Dictionary<string, long> sResetQuestDict = new Dictionary<string, long>();

	private static Stopwatch sStopwatch = new Stopwatch();

	private static string sLastAppPackage = null;

	private static string sLastVMName = null;

	private static SessionSwitchEventHandler sessionSwitchHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

	private static readonly byte[] bytes = new byte[13]
	{
		73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
		100, 101, 118
	};

	internal static void StartTimer(string vmName, string packageName)
	{
		StopTimer();
		sLastAppPackage = packageName;
		sLastVMName = vmName;
		sStopwatch.Reset();
		sStopwatch.Start();
	}

	internal static void StopTimer()
	{
		if (!sStopwatch.IsRunning || string.IsNullOrEmpty(sLastAppPackage))
		{
			return;
		}
		sStopwatch.Stop();
		long num = (long)sStopwatch.Elapsed.TotalSeconds;
		if (sDictAppUsageInfo.ContainsKey(sLastVMName))
		{
			if (sDictAppUsageInfo[sLastVMName].ContainsKey(sLastAppPackage))
			{
				sDictAppUsageInfo[sLastVMName][sLastAppPackage] += num;
			}
			else
			{
				sDictAppUsageInfo[sLastVMName].Add(sLastAppPackage, num);
			}
			sDictAppUsageInfo[sLastVMName]["TotalUsage"] += num;
		}
		else
		{
			sDictAppUsageInfo.Add(sLastVMName, new Dictionary<string, long> { { "TotalUsage", num } });
			sDictAppUsageInfo[sLastVMName].Add(sLastAppPackage, num);
		}
		sLastAppPackage = string.Empty;
	}

	internal static Dictionary<string, Dictionary<string, long>> GetRealtimeDictionary()
	{
		if (sStopwatch.IsRunning && !string.IsNullOrEmpty(sLastAppPackage))
		{
			Dictionary<string, Dictionary<string, long>> dictionary = new Dictionary<string, Dictionary<string, long>>();
			foreach (KeyValuePair<string, Dictionary<string, long>> item in sDictAppUsageInfo)
			{
				dictionary.Add(item.Key, item.Value.ToDictionary((KeyValuePair<string, long> _) => _.Key, (KeyValuePair<string, long> _) => _.Value));
			}
			long num = (long)sStopwatch.Elapsed.TotalSeconds;
			if (dictionary.ContainsKey(sLastVMName))
			{
				if (dictionary[sLastVMName].ContainsKey(sLastAppPackage))
				{
					dictionary[sLastVMName][sLastAppPackage] += num;
				}
				else
				{
					dictionary[sLastVMName].Add(sLastAppPackage, num);
				}
				dictionary[sLastVMName]["TotalUsage"] += num;
			}
			else
			{
				dictionary.Add(sLastVMName, new Dictionary<string, long> { { "TotalUsage", num } });
				dictionary[sLastVMName].Add(sLastAppPackage, num);
			}
			return dictionary;
		}
		return sDictAppUsageInfo;
	}

	internal static long GetTotalTimeForPackageAcrossInstances(string packageName)
	{
		long num = 0L;
		try
		{
			foreach (KeyValuePair<string, Dictionary<string, long>> item in sDictAppUsageInfo)
			{
				IEnumerable<KeyValuePair<string, long>> source = item.Value.Where((KeyValuePair<string, long> _) => string.Equals(_.Key, packageName, StringComparison.OrdinalIgnoreCase));
				if (source.Any())
				{
					num += source.First().Value;
				}
			}
			if (!string.IsNullOrEmpty(sLastAppPackage) && string.Compare(sLastAppPackage, packageName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				num += (long)sStopwatch.Elapsed.TotalSeconds;
			}
			Logger.Debug("Total time for package " + packageName + " " + num);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetTotalTimeForPackageAcrossInstances. Err : " + ex.ToString());
		}
		return num;
	}

	internal static long GetTotalTimeForAllPackages()
	{
		long num = 0L;
		try
		{
			foreach (KeyValuePair<string, Dictionary<string, long>> item in sDictAppUsageInfo)
			{
				long num2 = 0L;
				IEnumerable<KeyValuePair<string, long>> source = item.Value.Where((KeyValuePair<string, long> _) => string.Compare(_.Key, "Home", StringComparison.OrdinalIgnoreCase) == 0);
				if (source.Any())
				{
					num2 += source.First().Value;
				}
				source = item.Value.Where((KeyValuePair<string, long> _) => string.Compare(_.Key, "TotalUsage", StringComparison.OrdinalIgnoreCase) == 0);
				if (source.Any())
				{
					num += source.First().Value;
					num -= num2;
				}
			}
			if (!string.IsNullOrEmpty(sLastAppPackage) && !string.Equals(sLastAppPackage, "Home", StringComparison.InvariantCulture))
			{
				num += (long)sStopwatch.Elapsed.TotalSeconds;
			}
			Logger.Debug("Total time for all packages " + num);
			if (num < 0)
			{
				return 0L;
			}
			return num;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetTotalTimeForAllPackages " + ex.ToString());
		}
		return 0L;
	}

	internal static long GetTotalTimeForPackageAfterReset(string packageName)
	{
		try
		{
			long totalTimeForPackageAcrossInstances = GetTotalTimeForPackageAcrossInstances(packageName);
			if (totalTimeForPackageAcrossInstances < 0)
			{
				return 0L;
			}
			return totalTimeForPackageAcrossInstances;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetTotalTimeForPackageAfterReset. Err : " + ex.ToString());
		}
		return 0L;
	}

	internal static void AddPackageForReset(string package, long time)
	{
		sResetQuestDict[package] = time;
	}

	internal static void SessionEventHandler()
	{
		SystemEvents.SessionSwitch += sessionSwitchHandler;
	}

	private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if ((int)e.Reason == 7)
		{
			StopTimer();
		}
		else if ((int)e.Reason == 8)
		{
			StartTimerAfterResume();
		}
	}

	internal static void DetachSessionEventHandler()
	{
		SystemEvents.SessionSwitch -= sessionSwitchHandler;
	}

	private static void StartTimerAfterResume()
	{
		try
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(sLastVMName))
			{
				MainWindow mainWindow = BlueStacksUIUtils.DictWindows[sLastVMName];
				if (mainWindow != null && mainWindow.mTopBar.mAppTabButtons.SelectedTab != null)
				{
					StartTimer(sLastVMName, mainWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in starting timer after sleep. Err : " + ex.ToString());
		}
	}

	internal static void SaveData()
	{
		StopTimer();
		RegistryManager.Instance.AInfo = EncryptString(JsonConvert.SerializeObject((object)sDictAppUsageInfo));
	}

	internal static string EncryptString(string encryptString)
	{
		string userGuid = RegistryManager.Instance.UserGuid;
		byte[] array = Encoding.Unicode.GetBytes(encryptString);
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, bytes);
		using Aes aes = Aes.Create();
		aes.Key = rfc2898DeriveBytes.GetBytes(32);
		aes.IV = rfc2898DeriveBytes.GetBytes(16);
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		encryptString = Convert.ToBase64String(memoryStream.ToArray());
		return encryptString;
	}

	public static string DecryptString(string decryptString)
	{
		string userGuid = RegistryManager.Instance.UserGuid;
		decryptString = decryptString?.Replace(" ", "+");
		byte[] array = Convert.FromBase64String(decryptString);
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, bytes);
		using Aes aes = Aes.Create();
		aes.Key = rfc2898DeriveBytes.GetBytes(32);
		aes.IV = rfc2898DeriveBytes.GetBytes(16);
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		decryptString = Encoding.Unicode.GetString(memoryStream.ToArray());
		return decryptString;
	}
}
