using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class BlueStacksUpdater
{
	internal enum UpdateState
	{
		NO_UPDATE,
		UPDATE_AVAILABLE,
		DOWNLOADING,
		DOWNLOADED
	}

	internal enum UpdateType
	{
		FullUpdate,
		ClientOnly
	}

	private static MainWindow ParentWindow;

	internal static BackgroundWorker sCheckUpdateBackgroundWorker;

	private static UpdateDownloadProgress sUpdateDownloadProgress;

	internal static BlueStacksUpdateData sBstUpdateData = null;

	internal static Downloader sDownloader;

	private static UpdateState sUpdateState = UpdateState.NO_UPDATE;

	internal static bool IsDownloadingInHiddenMode { get; set; } = true;

	internal static UpdateState SUpdateState
	{
		get
		{
			return sUpdateState;
		}
		set
		{
			sUpdateState = value;
			BlueStacksUpdater.StateChanged?.Invoke();
		}
	}

	internal static event Action<Tuple<BlueStacksUpdateData, bool>> DownloadCompleted;

	internal static event Action StateChanged;

	public static void SetupBlueStacksUpdater(MainWindow window, bool isStartup)
	{
		ParentWindow = window;
		if (sCheckUpdateBackgroundWorker == null)
		{
			sCheckUpdateBackgroundWorker = new BackgroundWorker();
			sCheckUpdateBackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
			{
				bool flag = (bool)e.Argument;
				e.Result = new Tuple<BlueStacksUpdateData, bool>(sBstUpdateData = CheckForUpdate(!flag), flag);
			};
			sCheckUpdateBackgroundWorker.RunWorkerCompleted += CheckUpdateBackgroundWorker_RunWorkerCompleted;
		}
		if (!sCheckUpdateBackgroundWorker.IsBusy)
		{
			sCheckUpdateBackgroundWorker.RunWorkerAsync(isStartup);
		}
		else
		{
			Logger.Info("Not launching update checking thread, since already running");
		}
	}

	private static void CheckUpdateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		Tuple<BlueStacksUpdateData, bool> obj = (Tuple<BlueStacksUpdateData, bool>)e.Result;
		BlueStacksUpdateData item = ((Tuple<BlueStacksUpdateData>)(object)obj).Item1;
		bool item2 = obj.Item2;
		if (item.IsUpdateAvailble)
		{
			ParentWindow.mTopBar.mConfigButton.ImageName = "cfgmenu_update";
			((UIElement)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind(ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOAD_UPDATE", "");
			((UIElement)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage).Visibility = (Visibility)2;
			if (item.IsFullInstaller)
			{
				if (item2)
				{
					if (item.UpdateType.Equals("hard", StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Info("Forced full installer update, starting download.");
						DownloadNow(item, hiddenMode: true);
					}
					else if (item.UpdateType.Equals("soft", StringComparison.InvariantCultureIgnoreCase) && string.Compare(item.EngineVersion.Trim(), RegistryManager.Instance.LastUpdateSkippedVersion.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
					{
						ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopup);
						UpdatePrompt updatePrompt = new UpdatePrompt(item);
						((FrameworkElement)updatePrompt).Height = 215.0;
						((FrameworkElement)updatePrompt).Width = 400.0;
						UpdatePrompt updatePrompt2 = updatePrompt;
						new ContainerWindow(ParentWindow, (UserControl)(object)updatePrompt2, (int)((FrameworkElement)updatePrompt2).Width, (int)((FrameworkElement)updatePrompt2).Height);
					}
				}
			}
			else
			{
				Logger.Info("Only client installer update, starting download.");
				DownloadNow(item, hiddenMode: true);
			}
		}
		else
		{
			SUpdateState = UpdateState.NO_UPDATE;
		}
	}

	private static BlueStacksUpdateData CheckForUpdate(bool isManualCheck)
	{
		BlueStacksUpdateData blueStacksUpdateData = new BlueStacksUpdateData();
		try
		{
			string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/check_upgrade");
			Logger.Debug("The URL for checking upgrade: {0}", new object[1] { urlWithParams });
			string value = default(string);
			string text = default(string);
			string text2 = default(string);
			SystemUtils.GetOSInfo(ref value, ref text, ref text2);
			string value2 = InstallerArchitectures.AMD64;
			if (!SystemUtils.IsOs64Bit())
			{
				value2 = InstallerArchitectures.X86;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "installer_arch", value2 },
				{ "os", value },
				{
					"manual_check",
					isManualCheck.ToString(CultureInfo.InvariantCulture)
				}
			};
			string text3 = BstHttpClient.Post(urlWithParams, dictionary, (Dictionary<string, string>)null, false, string.Empty, 5000, 1, 0, false, "bgp64");
			Logger.Info("Response received for check for update: " + Environment.NewLine + text3);
			JObject val = JObject.Parse(text3);
			if (((object)val["update_available"]).ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase) && RegistryManager.Instance.FailedUpgradeVersion != ((object)val["update_details"][(object)"client_version"]).ToString())
			{
				blueStacksUpdateData.IsUpdateAvailble = true;
				blueStacksUpdateData.UpdateType = ((object)val["update_details"][(object)"upgrade_type"]).ToString();
				blueStacksUpdateData.IsFullInstaller = val["update_details"][(object)"is_full_installer"].ToObject<bool>();
				blueStacksUpdateData.Md5 = ((object)val["update_details"][(object)"md5"]).ToString();
				blueStacksUpdateData.ClientVersion = ((object)val["update_details"][(object)"client_version"]).ToString();
				blueStacksUpdateData.EngineVersion = ((object)val["update_details"][(object)"engine_version"]).ToString();
				blueStacksUpdateData.DownloadUrl = ((object)val["update_details"][(object)"download_url"]).ToString();
				blueStacksUpdateData.DetailedChangeLogsUrl = ((object)val["update_details"][(object)"detailed_changelogs_url"]).ToString();
				if (!Directory.Exists(RegistryManager.Instance.SetupFolder))
				{
					Directory.CreateDirectory(RegistryManager.Instance.SetupFolder);
				}
				if (blueStacksUpdateData.IsFullInstaller)
				{
					blueStacksUpdateData.UpdateDownloadLocation = Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + blueStacksUpdateData.ClientVersion + "_full.exe");
				}
				else
				{
					blueStacksUpdateData.UpdateDownloadLocation = Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + blueStacksUpdateData.ClientVersion + "_client.zip");
				}
				RegistryManager.Instance.DownloadedUpdateFile = blueStacksUpdateData.UpdateDownloadLocation;
				sBstUpdateData = blueStacksUpdateData;
				SUpdateState = UpdateState.UPDATE_AVAILABLE;
			}
			return blueStacksUpdateData;
		}
		catch (Exception ex)
		{
			Logger.Warning("Got error in checking for upgrade: {0}", new object[1] { ex.ToString() });
			return new BlueStacksUpdateData
			{
				IsTryAgain = true
			};
		}
	}

	private static void DownloadUpdate(BlueStacksUpdateData bluestacksUpdateData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		sDownloader = new Downloader();
		sDownloader.DownloadException += new DownloadExceptionEventHandler(Downloader_DownloadException);
		sDownloader.DownloadProgressPercentChanged += new DownloadProgressPercentChangedEventHandler(Downloader_DownloadProgressPercentChanged);
		sDownloader.DownloadFileCompleted += new DownloadFileCompletedEventHandler(Downloader_DownloadFileCompleted);
		sDownloader.UnsupportedResume += new UnsupportedResumeEventHandler(Downloader_UnsupportedResume);
		sDownloader.DownloadFile(bluestacksUpdateData.DownloadUrl, bluestacksUpdateData.UpdateDownloadLocation);
	}

	private static void Downloader_DownloadProgressPercentChanged(double percentDouble)
	{
		Logger.Info("File downloaded {0}%", new object[1] { percentDouble });
		int percent = Convert.ToInt32(Math.Floor(percentDouble));
		ParentWindow.mTopBar.ChangeDownloadPercent(percent);
		if (sUpdateDownloadProgress != null)
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((ContentControl)sUpdateDownloadProgress.mUpdateDownloadProgressPercentage).Content = percent + "%";
				((RangeBase)sUpdateDownloadProgress.mUpdateDownloadProgressBar).Value = percent;
				((ContentControl)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage).Content = percent + "%";
			}, new object[0]);
		}
	}

	private static void Downloader_UnsupportedResume(HttpStatusCode sc)
	{
		Logger.Error("UnsupportedResume, HTTPStatusCode: {0}", new object[1] { sc });
		File.Delete(sBstUpdateData.UpdateDownloadLocation);
		sDownloader.DownloadFile(sBstUpdateData.DownloadUrl, sBstUpdateData.UpdateDownloadLocation);
	}

	private static void Downloader_DownloadFileCompleted(object sender, EventArgs args)
	{
		Logger.Info("File downloaded successfully at {0}", new object[1] { sBstUpdateData?.UpdateDownloadLocation });
		DownloadComplete();
	}

	private static void Downloader_DownloadException(Exception e)
	{
		Logger.Error("Failed to download file: {0}. err: {1}", new object[2] { sBstUpdateData.DownloadUrl, e.Message });
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_UPGRADE_FAILED", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_SOME_ERROR_OCCURED_DOWNLOAD", "");
			val.AddButton((ButtonColors)4, "STRING_RETRY", (EventHandler)RetryDownload, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)DownloadCancelled, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
			((Window)sUpdateDownloadProgress).Hide();
		}, new object[0]);
	}

	private static void DownloadCancelled(object sender, EventArgs e)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private static void RetryDownload(object sender, EventArgs e)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			sDownloader.DownloadFile(sBstUpdateData.DownloadUrl, sBstUpdateData.UpdateDownloadLocation);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void DownloadComplete()
	{
		Logger.Info("Installer download completed");
		SUpdateState = UpdateState.DOWNLOADED;
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			BlueStacksUIBinding.Bind(ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_INSTALL_UPDATE", "");
			((UIElement)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage).Visibility = (Visibility)2;
			if (sUpdateDownloadProgress != null)
			{
				((Window)sUpdateDownloadProgress).Close();
			}
		}, new object[0]);
		BlueStacksUpdater.DownloadCompleted(new Tuple<BlueStacksUpdateData, bool>(sBstUpdateData, IsDownloadingInHiddenMode));
	}

	internal static void DownloadNow(BlueStacksUpdateData bstUpdateData, bool hiddenMode)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			IsDownloadingInHiddenMode = hiddenMode;
			SUpdateState = UpdateState.DOWNLOADING;
			if (File.Exists(bstUpdateData.UpdateDownloadLocation))
			{
				DownloadComplete();
			}
			else
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIBinding.Bind(ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOADING_UPDATE", "");
					((UIElement)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage).Visibility = (Visibility)0;
					((ContentControl)ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage).Content = "0%";
					sUpdateDownloadProgress = new UpdateDownloadProgress();
					((ContentControl)sUpdateDownloadProgress.mUpdateDownloadProgressPercentage).Content = "0%";
					((Window)sUpdateDownloadProgress).Owner = (Window)(object)ParentWindow;
					if (!hiddenMode)
					{
						((Window)sUpdateDownloadProgress).Show();
					}
				}, new object[0]);
				DownloadUpdate(bstUpdateData);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal static void ShowDownloadProgress()
	{
		if (sUpdateDownloadProgress != null)
		{
			((Window)sUpdateDownloadProgress).Show();
		}
	}

	internal static void CheckDownloadedUpdateFileAndUpdate()
	{
		using BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate
		{
			HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile);
		};
		backgroundWorker.RunWorkerCompleted += delegate
		{
			App.ExitApplication();
		};
		backgroundWorker.RunWorkerAsync();
	}

	internal static void HandleUpgrade(string downloadedFilePath)
	{
		if (CheckIfUpdateIsFullOrClientOnly(downloadedFilePath) == UpdateType.ClientOnly)
		{
			HandleClientOnlyUpgrade(downloadedFilePath);
		}
		else
		{
			HandleFullUpgrade(downloadedFilePath);
		}
		RegistryManager.Instance.UpdaterFileDeletePath = RegistryManager.Instance.DownloadedUpdateFile;
		RegistryManager.Instance.DownloadedUpdateFile = "";
	}

	private static void HandleFullUpgrade(string downloadedFilePath)
	{
		Logger.Info("In HandleFullUpgrade");
		BluestacksProcessHelper.RunUpdateInstaller(downloadedFilePath, "-u -upgradesourcepath BluestacksUI");
	}

	private static void HandleClientOnlyUpgrade(string downloadedFilePath)
	{
		Logger.Info("In HandleClientOnlyUpgrade");
		try
		{
			int num = ExtractingClientInstaller(downloadedFilePath);
			if (num == 0)
			{
				BluestacksProcessHelper.RunUpdateInstaller(Path.Combine(Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(downloadedFilePath)), "Bootstrapper.exe"), "");
				return;
			}
			Logger.Warning("Update extraction failed, ExitCode: {0}", new object[1] { num });
			File.Delete(downloadedFilePath);
		}
		catch (Exception ex)
		{
			Logger.Error("Some Error in Client Upgrade err: ", new object[1] { ex.ToString() });
		}
	}

	internal static bool CheckIfDownloadedFileExist()
	{
		string downloadedUpdateFile = RegistryManager.Instance.DownloadedUpdateFile;
		if (!string.IsNullOrEmpty(downloadedUpdateFile) && File.Exists(downloadedUpdateFile))
		{
			return true;
		}
		string updaterFileDeletePath = RegistryManager.Instance.UpdaterFileDeletePath;
		if (!string.IsNullOrEmpty(updaterFileDeletePath) && File.Exists(updaterFileDeletePath) && RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			try
			{
				File.Delete(updaterFileDeletePath);
				RegistryManager.Instance.UpdaterFileDeletePath = "";
			}
			catch (Exception ex)
			{
				Logger.Warning("Error in Deleting Updater File : ", new object[1] { ex.ToString() });
			}
		}
		return false;
	}

	private static UpdateType CheckIfUpdateIsFullOrClientOnly(string downloadedFilePath)
	{
		if (string.Equals(Path.GetExtension(downloadedFilePath), ".zip", StringComparison.InvariantCultureIgnoreCase))
		{
			return UpdateType.ClientOnly;
		}
		return UpdateType.FullUpdate;
	}

	private static int ExtractingClientInstaller(string updateFile)
	{
		string text = Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(updateFile));
		Logger.Info("Extracting Zip file {0} at {1}", new object[2] { updateFile, text });
		return MiscUtils.Extract7Zip(updateFile, text);
	}
}
