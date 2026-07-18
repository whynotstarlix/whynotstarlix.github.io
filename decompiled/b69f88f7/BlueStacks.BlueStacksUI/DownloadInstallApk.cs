using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class DownloadInstallApk
{
	internal const string mIconUrl = "https://cloud.bluestacks.com/app/icon?pkg={0}&fallback=false";

	private Thread mDownloadThread;

	public bool mIsDownloading;

	private LegacyDownloader mDownloader;

	private static Dictionary<string, SerialWorkQueue> mSerialQueue = new Dictionary<string, SerialWorkQueue>();

	internal static List<string> sDownloadedApkList = new List<string>();

	internal static List<string> sApkInstalledFromChooser = new List<string>();

	private MainWindow ParentWindow;

	public DownloadInstallApk(MainWindow mainWindow)
	{
		ParentWindow = mainWindow;
	}

	public static SerialWorkQueue SerialWorkQueueInstaller(string vmName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		if (!mSerialQueue.ContainsKey(vmName))
		{
			mSerialQueue[vmName] = new SerialWorkQueue();
		}
		return mSerialQueue[vmName];
	}

	internal void DownloadAndInstallAppFromJson(string campaignJson)
	{
		try
		{
			JObject obj = JObject.Parse(campaignJson);
			string iconUrl = "";
			string appName = "";
			string apkUrl = "";
			string package = "";
			JsonExtensions.AssignIfContains<string>((JToken)(object)obj, "app_icon_url", (Action<string>)delegate(string x)
			{
				iconUrl = x.Trim();
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)obj, "app_name", (Action<string>)delegate(string x)
			{
				appName = x.Trim();
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)obj, "app_url", (Action<string>)delegate(string x)
			{
				apkUrl = x.Trim();
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)obj, "app_pkg", (Action<string>)delegate(string x)
			{
				package = x.Trim();
			});
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (string.IsNullOrEmpty(apkUrl))
				{
					ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(package, (PlayStoreAction)0);
				}
				else
				{
					DownloadAndInstallApp(iconUrl, appName, apkUrl, package, isLaunchAfterInstall: true, isDeleteApk: true);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Info("Error in Fle. Json string : " + campaignJson + "Error: " + ex.ToString());
		}
	}

	internal void DownloadAndInstallApp(string iconUrl, string appName, string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null && !ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName).IsAppSuggestionActive)
		{
			if (ParentWindow.mAppHandler.IsAppInstalled(packageName))
			{
				if (!string.IsNullOrEmpty(timestamp))
				{
					bool flag = true;
					DateTime dateTime = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
					DateTime maxValue = DateTime.MaxValue;
					if (((Dictionary<string, DateTime>)(object)ParentWindow.mAppHandler.CdnAppdict).ContainsKey(packageName))
					{
						maxValue = ((Dictionary<string, DateTime>)(object)ParentWindow.mAppHandler.CdnAppdict)[packageName];
						if (dateTime <= maxValue)
						{
							flag = false;
						}
					}
					if (flag)
					{
						CustomMessageWindow val = new CustomMessageWindow();
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_INSTALL_UPDATE", "");
						BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_APP_UPGRADE", "");
						val.AddButton((ButtonColors)4, "STRING_UPGRADE_TEXT", (EventHandler)delegate
						{
							DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
						}, (string)null, false, (object)null);
						val.AddButton((ButtonColors)2, "STRING_CONTINUE_ANYWAY", (EventHandler)delegate
						{
							ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName);
						}, (string)null, false, (object)null);
						((Window)val).Owner = (Window)(object)ParentWindow;
						((Window)val).ShowDialog();
					}
					else
					{
						ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName);
					}
				}
				else
				{
					ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName);
				}
			}
			else
			{
				ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home");
			}
		}
		else
		{
			DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
		}
	}

	internal void DownloadApk(string iconUrl, string appName, string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp)
	{
		if (!string.IsNullOrEmpty(apkUrl))
		{
			Logger.Info("apkUrl: " + apkUrl);
			Utils.TinyDownloader(iconUrl, packageName + ".png", "", false);
			ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, apkUrl, this);
			DownloadApk(apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
		}
	}

	public void DownloadApk(string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
	{
		string text = Path.Combine(RegistryStrings.DataDir, "DownloadedApk");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string path = Regex.Replace(packageName + ".apk", "[\\x22\\\\\\/:*?|<>]", " ");
		string apkFilePath = Path.Combine(text, path);
		Logger.Info("Downloading Apk file to: " + apkFilePath);
		ParentWindow.mWelcomeTab.mHomeAppManager.DownloadStarted(packageName);
		ClientStats.SendClientStatsAsync("download", "unknown", "app_install", packageName);
		mDownloadThread = new Thread((ThreadStart)delegate
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_009f: Expected O, but got Unknown
			//IL_009f: Expected O, but got Unknown
			string apkUrl2 = apkUrl;
			if (IsContainsGoogleAdId(apkUrl2))
			{
				apkUrl = AddGoogleAdidWithApk(apkUrl2);
			}
			apkUrl = BlueStacksUIUtils.GetFinalRedirectedUrl(apkUrl);
			if (!string.IsNullOrEmpty(apkUrl))
			{
				mIsDownloading = true;
				mDownloader = new LegacyDownloader(3, apkUrl, apkFilePath);
				mDownloader.Download((UpdateProgressCallback)delegate(int percent)
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.UpdateAppDownloadProgress(packageName, percent);
				}, (DownloadCompletedCallback)delegate(string filePath)
				{
					ClientStats.SendClientStatsAsync("download", "success", "app_install", packageName);
					mIsDownloading = false;
					ParentWindow.mWelcomeTab.mHomeAppManager.DownloadCompleted(packageName, filePath);
					InstallApk(packageName, filePath, isLaunchAfterInstall, isDeleteApk, timestamp);
					sDownloadedApkList.Add(packageName);
				}, (ExceptionCallback)delegate(Exception ex)
				{
					ClientStats.SendClientStatsAsync("download", "fail", "app_install", packageName);
					ParentWindow.mWelcomeTab.mHomeAppManager.DownloadFailed(packageName);
					Logger.Error("Failed to download file: {0}. err: {1}", new object[2] { apkFilePath, ex.Message });
				}, (ContentTypeCallback)null, (SizeDownloadedCallback)null, (PayloadInfoCallback)null);
			}
		})
		{
			IsBackground = true
		};
		mDownloadThread.Start();
	}

	private string AddGoogleAdidWithApk(string apkUrl)
	{
		Logger.Info("In AddGoogleAdidWithApk");
		string newValue = "google_aid=00000000-0000-0000-0000-000000000000";
		try
		{
			JObject val = JObject.Parse(HTTPUtils.SendRequestToGuest("getGoogleAdID", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
			if (((object)val["result"]).ToString() == "ok")
			{
				string text = ((object)val["googleadid"]).ToString();
				string newValue2 = string.Format(CultureInfo.InvariantCulture, "google_aid={0}", new object[1] { text });
				return apkUrl.Replace("google_aid={google_aid}", newValue2);
			}
			return apkUrl.Replace("google_aid={google_aid}", newValue);
		}
		catch (Exception ex)
		{
			Logger.Error("Error in adding google adId" + ex.ToString());
			return apkUrl.Replace("google_aid={google_aid}", newValue);
		}
	}

	private static bool IsContainsGoogleAdId(string apkUrl)
	{
		if (apkUrl.ToLower(CultureInfo.InvariantCulture).Contains("google_aid={google_aid}"))
		{
			return true;
		}
		return false;
	}

	internal void AbortApkDownload(string packageName)
	{
		ClientStats.SendClientStatsAsync("download", "cancel", "app_install", packageName);
		if (mDownloader != null)
		{
			mDownloader.AbortDownload();
		}
		if (mDownloadThread != null)
		{
			mDownloadThread.Abort();
		}
	}

	internal void ChooseAndInstallApk()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog
		{
			Filter = "Android Files (*.apk, *.xapk)|*.apk; *.xapk",
			Multiselect = true,
			RestoreDirectory = true
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				string[] fileNames = ((FileDialog)val).FileNames;
				foreach (string text in fileNames)
				{
					Logger.Info("File Selected : " + text);
					InstallApk(text, addToChooseApkList: true);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void InstallApk(string apkPath, bool addToChooseApkList = false)
	{
		Logger.Info("Console: Installing apk: {0}", new object[1] { apkPath });
		string package = string.Empty;
		string appName = string.Empty;
		if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
		{
			JToken val = Utils.ExtractInfoFromXapk(apkPath);
			if (val != null)
			{
				package = JsonExtensions.GetValue(val, "package_name");
				appName = JsonExtensions.GetValue(val, "name");
				Logger.Debug("Package name from manifest.json.." + package);
			}
		}
		else
		{
			AppInfoExtractor apkInfo = AppInfoExtractor.GetApkInfo(apkPath);
			package = apkInfo.PackageName;
			appName = apkInfo.AppName;
		}
		if (addToChooseApkList)
		{
			sApkInstalledFromChooser.Add(package);
		}
		if (!string.IsNullOrEmpty(package))
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, this);
			}, new object[0]);
		}
		InstallApk(package, apkPath, isLaunchAfterInstall: false, isDeleteApk: false);
	}

	internal void InstallApk(string packageName, string apkPath, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, apkPath);
		SerialWorkQueueInstaller(ParentWindow.mVmName).Enqueue((Work)delegate
		{
			Logger.Info("Installing apk: {0}", new object[1] { apkPath });
			int num = BluestacksProcessHelper.RunApkInstaller(apkPath, isSilentInstall: true, ParentWindow.mVmName);
			Logger.Info("Apk installer exit code: {0}", new object[1] { num });
			if (num == 0)
			{
				if (sDownloadedApkList.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName);
					sDownloadedApkList.Remove(packageName);
					UpdateCdnAppEntry(isAdd: true, packageName, timestamp);
				}
				else if (sApkInstalledFromChooser.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName);
					sApkInstalledFromChooser.Remove(packageName);
				}
				ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
				if (isLaunchAfterInstall)
				{
					ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
				}
				Logger.Info("Installation successful.");
				if (isDeleteApk)
				{
					File.Delete(apkPath);
				}
				Logger.Info("Install Completed : " + packageName);
			}
			else
			{
				if (sDownloadedApkList.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture));
					sDownloadedApkList.Remove(packageName);
				}
				else if (sApkInstalledFromChooser.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture));
					sApkInstalledFromChooser.Remove(packageName);
				}
				ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>
				{
					{
						"errcode",
						Convert.ToString(num, CultureInfo.InvariantCulture)
					},
					{ "precode", "0" },
					{ "app_pkg", packageName }
				});
				ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallFailed(packageName);
			}
		});
	}

	internal int InstallFLEApk(string packageName, string apkPath)
	{
		Logger.Info("Installing apk: {0}", new object[1] { apkPath });
		int num = BluestacksProcessHelper.RunApkInstaller(apkPath, isSilentInstall: true, ParentWindow.mVmName);
		Logger.Info("Apk installer exit code: {0}", new object[1] { num });
		if (num == 0)
		{
			if (sDownloadedApkList.Contains(packageName))
			{
				ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName);
				sDownloadedApkList.Remove(packageName);
				UpdateCdnAppEntry(isAdd: true, packageName, "");
			}
			else if (sApkInstalledFromChooser.Contains(packageName))
			{
				ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName);
				sApkInstalledFromChooser.Remove(packageName);
			}
			ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
			Logger.Info("Installation successful.");
			File.Delete(apkPath);
		}
		else
		{
			if (sDownloadedApkList.Contains(packageName))
			{
				ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture));
				sDownloadedApkList.Remove(packageName);
			}
			else if (sApkInstalledFromChooser.Contains(packageName))
			{
				ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture));
				sApkInstalledFromChooser.Remove(packageName);
			}
			ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>
			{
				{
					"errcode",
					Convert.ToString(num, CultureInfo.InvariantCulture)
				},
				{ "precode", "0" },
				{ "app_pkg", packageName }
			});
		}
		Logger.Info("Install Completed : " + packageName);
		return num;
	}

	internal void UninstallApp(string packageName)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		SerialWorkQueueInstaller(ParentWindow.mVmName).Enqueue((Work)delegate
		{
			Logger.Info("Uninstall started : " + packageName);
			Dictionary<string, string> dictionary = new Dictionary<string, string> { ["package"] = packageName };
			try
			{
				JArray val = JArray.Parse(HTTPUtils.SendRequestToAgent("uninstall", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64", true));
				try
				{
					if (!JObject.Parse(((object)val[0]).ToString())["success"].ToObject<bool>())
					{
						ClientStats.SendClientStatsAsync("uninstall", "fail", "app_install", packageName);
					}
					else
					{
						UpdateCdnAppEntry(isAdd: false, packageName, "");
					}
				}
				catch
				{
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to uninstall app. Err: " + ex.Message);
			}
			Logger.Info("Uninstall completed for " + packageName);
		});
	}

	internal void UpdateCdnAppEntry(bool isAdd, string packageName, string timestamp)
	{
		DateTime timestamp2 = DateTime.MinValue;
		if (!string.IsNullOrEmpty(timestamp))
		{
			timestamp2 = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
		}
		ParentWindow.mAppHandler.WriteXMl(isAdd, packageName, timestamp2);
	}
}
