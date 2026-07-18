using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

internal class CommonHandlers : IDisposable
{
	public delegate void MacroBookmarkChanged(string fileName, bool wasBookmarked);

	public delegate void MacroSettingsChanged(MacroRecording record);

	public delegate void ShortcutKeysChanged(bool isEnabled);

	public delegate void ShortcutKeysRefresh();

	public delegate void MacroDeleted(string fileName);

	public delegate void OverlayStateChanged(bool isEnabled);

	public delegate void MacroButtonVisibilityChanged(bool isVisible);

	public delegate void OperationSyncButtonVisibilityChanged(bool isVisible);

	public delegate void OBSResponseTimeout();

	public delegate void ScreenRecorderStateTransitioning();

	public delegate void BTvDownloaderMinimized();

	public delegate void GamepadButtonVisibilityChanged(bool visibility);

	public delegate void ScreenRecordingStateChanged(bool isRecording);

	public delegate void VolumeChanged(int volumeLevel);

	public delegate void VolumeMuted(bool muted);

	public delegate void GameGuideButtonVisibilityChanged(bool visibility);

	private MainWindow ParentWindow;

	internal static bool sIsRecordingVideo = false;

	internal static string sRecordingInstance = "";

	private static bool sIsOBSStartingStopping = false;

	private static bool sDownloading;

	private LegacyDownloader mDownloader;

	private static CustomMessageWindow sWindow;

	internal ShortcutConfig mShortcutsConfigInstance;

	internal static string mSavedVideoRecordingFilePath = null;

	private System.Timers.Timer mObsResponseTimeoutTimer;

	private long mDownloadedSize;

	private long mLastSizeChecked;

	private DispatcherTimer mDownloadStatusTimer;

	private float mRecorderSizeMb;

	private bool disposedValue;

	public event MacroBookmarkChanged MacroBookmarkChangedEvent;

	public event MacroSettingsChanged MacroSettingChangedEvent;

	public event ShortcutKeysChanged ShortcutKeysChangedEvent;

	public event ShortcutKeysRefresh ShortcutKeysRefreshEvent;

	public event MacroDeleted MacroDeletedEvent;

	public event OverlayStateChanged OverlayStateChangedEvent;

	public event MacroButtonVisibilityChanged MacroButtonVisibilityChangedEvent;

	public event OperationSyncButtonVisibilityChanged OperationSyncButtonVisibilityChangedEvent;

	public event OBSResponseTimeout OBSResponseTimeoutEvent;

	public event ScreenRecorderStateTransitioning ScreenRecorderStateTransitioningEvent;

	public event BTvDownloaderMinimized BTvDownloaderMinimizedEvent;

	public event GamepadButtonVisibilityChanged GamepadButtonVisibilityChangedEvent;

	public event ScreenRecordingStateChanged ScreenRecordingStateChangedEvent;

	public event VolumeChanged VolumeChangedEvent;

	public event VolumeMuted VolumeMutedEvent;

	public event GameGuideButtonVisibilityChanged GameGuideButtonVisibilityChangedEvent;

	internal void OnVolumeMuted(bool muted)
	{
		this.VolumeMutedEvent?.Invoke(muted);
	}

	internal void OnVolumeChanged(int volumeLevel)
	{
		this.VolumeChangedEvent?.Invoke(volumeLevel);
	}

	internal void OnScreenRecordingStateChanged(bool isRecording)
	{
		this.ScreenRecordingStateChangedEvent?.Invoke(isRecording);
	}

	internal void OnGamepadButtonVisibilityChanged(bool visiblity)
	{
		this.GamepadButtonVisibilityChangedEvent?.Invoke(visiblity);
	}

	internal void OnGameGuideButtonVisibilityChanged(bool visiblity)
	{
		this.GameGuideButtonVisibilityChangedEvent?.Invoke(visiblity);
	}

	private void OnOBSResponseTimeout()
	{
		this.OBSResponseTimeoutEvent?.Invoke();
	}

	private void OnBTvDownloaderMinimized()
	{
		this.BTvDownloaderMinimizedEvent?.Invoke();
	}

	internal void OnScreenRecorderStateTransitioning()
	{
		this.ScreenRecorderStateTransitioningEvent?.Invoke();
	}

	internal void OnMacroButtonVisibilityChanged(bool isVisible)
	{
		this.MacroButtonVisibilityChangedEvent?.Invoke(isVisible);
	}

	internal void OnOperationSyncButtonVisibilityChanged(bool isVisible)
	{
		this.OperationSyncButtonVisibilityChangedEvent?.Invoke(isVisible);
	}

	internal void OnMacroBookmarkChanged(string fileName, bool wasBookmarked)
	{
		foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
		{
			dictWindow.Value.mCommonHandler?.MacroBookmarkChangedEvent?.Invoke(fileName, wasBookmarked);
		}
	}

	internal static void OnMacroSettingChanged(MacroRecording record)
	{
		foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
		{
			dictWindow.Value.mCommonHandler?.MacroSettingChangedEvent?.Invoke(record);
		}
	}

	internal static void OnMacroDeleted(string fileName)
	{
		foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
		{
			dictWindow.Value.mCommonHandler?.MacroDeletedEvent?.Invoke(fileName);
		}
	}

	internal void OnShortcutKeysChanged(bool isEnabled)
	{
		this.ShortcutKeysChangedEvent?.Invoke(isEnabled);
	}

	internal void OnShortcutKeysRefresh()
	{
		this.ShortcutKeysRefreshEvent?.Invoke();
	}

	internal void OnOverlayStateChanged(bool isEnabled)
	{
		this.OverlayStateChangedEvent?.Invoke(isEnabled);
	}

	internal CommonHandlers(MainWindow window)
	{
		ParentWindow = window;
	}

	public void LocationButtonHandler()
	{
		ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_MAP", "com.location.provider", "com.location.provider.MapsActivity", "ico_fakegps", isSwitch: true, isLaunch: true);
	}

	public void ImageTranslationHandler()
	{
		Logger.Info("Saving screenshot automatically for image translater");
		if (ImageTranslateControl.Instance == null)
		{
			Bitmap val = CaptureSreenShot();
			try
			{
				ImageTranslateControl imageTranslateControl = new ImageTranslateControl(ParentWindow);
				imageTranslateControl.GetTranslateImage(val);
				ParentWindow.ShowDimOverlay(imageTranslateControl);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	internal static void ToggleFarmMode(bool newStatus)
	{
		RegistryManager.Instance.CurrentFarmModeStatus = newStatus;
		ThreadPool.QueueUserWorkItem(delegate
		{
			foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
			{
				try
				{
					dictWindow.Value.mFrontendHandler.SendFrontendRequestAsync("farmModeHandler", new Dictionary<string, string> { 
					{
						"enable",
						RegistryManager.Instance.CurrentFarmModeStatus.ToString(CultureInfo.InvariantCulture)
					} });
				}
				catch
				{
				}
			}
		});
	}

	internal void SearchAppCenter(string searchString)
	{
		AppTabButton tab = ParentWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
		if (tab?.GetBrowserControl()?.CefBrowser != null)
		{
			((WpfCefBrowser)tab.GetBrowserControl().CefBrowser).ExecuteJavaScript(string.Format(CultureInfo.InvariantCulture, "openSearch(\"{0}\")", new object[1] { HttpUtility.UrlEncode(searchString) }), ((WpfCefBrowser)tab.GetBrowserControl().CefBrowser).StartUrl, 0);
			ParentWindow.mTopBar.mAppTabButtons.GoToTab("appcenter");
		}
		else
		{
			ParentWindow.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl(null) + "&query=" + HttpUtility.UrlEncode(searchString), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter");
		}
	}

	internal void HideMacroRecorderWindow()
	{
		((Window)ParentWindow.MacroRecorderWindow).Owner = null;
		((Window)ParentWindow.MacroRecorderWindow).Hide();
		((CustomWindow)ParentWindow.MacroRecorderWindow).ShowWithParentWindow = false;
	}

	internal void RefreshMacroRecorderWindow()
	{
		((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children.Clear();
		ParentWindow.MacroRecorderWindow.Init();
	}

	internal static void RefreshAllMacroRecorderWindow()
	{
		try
		{
			foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
			{
				if (dictWindow.Value.MacroRecorderWindow != null)
				{
					((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children.Clear();
					dictWindow.Value.MacroRecorderWindow.Init();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("Error in refreshing operation recorder window" + ex);
		}
	}

	internal static void RefreshAllMacroWindowWithScroll()
	{
		try
		{
			foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
			{
				if (dictWindow.Value.MacroRecorderWindow != null)
				{
					((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children.Clear();
					dictWindow.Value.MacroRecorderWindow.Init();
					dictWindow.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("Error in refreshing operation recorder window" + ex);
		}
	}

	internal void ShowMacroRecorderWindow()
	{
		((Window)ParentWindow.MacroRecorderWindow).Owner = (Window)(object)ParentWindow;
		((CustomWindow)ParentWindow.MacroRecorderWindow).ShowWithParentWindow = true;
		((Window)ParentWindow.MacroRecorderWindow).Show();
		((Window)ParentWindow).Activate();
	}

	private Bitmap CaptureSreenShot()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		Point val = ((Visual)ParentWindow.mContentGrid).PointToScreen(new Point(0.0, 0.0));
		Point val2 = default(Point);
		((Point)(ref val2))._002Ector((double)Convert.ToInt32(((Point)(ref val)).X), (double)Convert.ToInt32(((Point)(ref val)).Y));
		Point val3 = ((Visual)ParentWindow.mContentGrid).PointToScreen(new Point((double)(int)((FrameworkElement)ParentWindow.mContentGrid).ActualWidth, (double)((int)((FrameworkElement)ParentWindow.mContentGrid).ActualHeight - 40)));
		Size size = new Size(Convert.ToInt32(((Point)(ref val3)).X - ((Point)(ref val2)).X), Convert.ToInt32(((Point)(ref val3)).Y - ((Point)(ref val2)).Y));
		Bitmap val4 = new Bitmap(size.Width, size.Height);
		Point point = new Point((int)((Point)(ref val2)).X, (int)((Point)(ref val2)).Y);
		Graphics val5 = Graphics.FromImage((Image)(object)val4);
		try
		{
			val5.CopyFromScreen(point, Point.Empty, size);
			return val4;
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
	}

	public void ScreenShotButtonHandler()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		try
		{
			string text = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss", CultureInfo.InvariantCulture);
			string text2 = ParentWindow.mTopBar.mAppTabButtons.SelectedTab.AppName;
			if (FeatureManager.Instance.IsCustomUIForNCSoft && !string.IsNullOrEmpty(ParentWindow.mNCTopBar.mAppName.Text))
			{
				text2 = ParentWindow.mNCTopBar.mAppName.Text;
			}
			string path = text2 + "_Screenshot_" + text + ".jpg";
			string filePath = Path.Combine(Path.GetTempPath(), path);
			ParentWindow.mFrontendHandler.GetScreenShot(filePath);
			try
			{
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					SoundPlayer val = new SoundPlayer(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "camera_shutter_click.wav"));
					try
					{
						val.Play();
						return;
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in screenshot button handler: {0}", new object[1] { ex });
		}
	}

	internal void PostScreenShotWork(string screenshotFileFullPath, bool showScreenShotSaved)
	{
		try
		{
			Logger.Debug("screen shot path..." + screenshotFileFullPath);
			if (RegistryManager.Instance.IsScreenshotsLocationPopupEnabled)
			{
				ShowScreenShotFolderUpdatePopup();
			}
			string text = RegistryManager.Instance.ScreenShotsPath;
			if (!StringExtensions.IsValidPath(text))
			{
				string screenshotDefaultPath = RegistryStrings.ScreenshotDefaultPath;
				if (!Directory.Exists(screenshotDefaultPath))
				{
					Directory.CreateDirectory(screenshotDefaultPath);
				}
				RegistryManager.Instance.ScreenShotsPath = screenshotDefaultPath;
				text = screenshotDefaultPath;
			}
			string fileName = Path.GetFileName(screenshotFileFullPath);
			string text2 = Path.Combine(text, fileName);
			Logger.Debug("Screen shot filename.." + text2);
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(screenshotFileFullPath, text2);
			ClientStats.SendMiscellaneousStatsAsync("MediaFileSaveSuccess", RegistryManager.Instance.UserGuid, "ScreenShot", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			if (showScreenShotSaved && RegistryManager.Instance.IsShowToastNotification)
			{
				ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_SAVED", ""));
			}
			if (showScreenShotSaved && ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
			{
				ParentWindow.mSidebar.ShowScreenshotSavedPopup(text2);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in post screenshot work: {0}", new object[1] { ex });
		}
	}

	private void ShowScreenShotFolderUpdatePopup()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
		string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
		CustomMessageWindow val = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
		val.AddButton((ButtonColors)4, "STRING_CHOOSE_CUSTOM", (EventHandler)ChooseCustomFolder, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_USE_CURRENT", (EventHandler)null, (string)null, false, (object)null);
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_CHOOSE_FOLDER_TEXT", "");
		((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
		val.BodyWarningTextBlock.Text = screenShotsPath;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
		ParentWindow.ShowDimOverlay();
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
		ParentWindow.HideDimOverlay();
		ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPopUp", screenShotsPath, RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	internal void AddCoordinatesToScriptText(double x, double y)
	{
		if (KMManager.sIsInScriptEditingMode && KMManager.CanvasWindow != null)
		{
			KMManager.CanvasWindow.SidebarWindow?.InsertXYInScript(x, y);
		}
	}

	private void ChooseCustomFolder(object sender, EventArgs e)
	{
		string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
		if (!Directory.Exists(screenShotsPath))
		{
			Directory.CreateDirectory(screenShotsPath);
		}
		ShowFolderBrowserDialog(screenShotsPath);
	}

	internal void ShowFolderBrowserDialog(string screenshotPath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		FolderBrowserDialog val = new FolderBrowserDialog
		{
			SelectedPath = screenshotPath,
			ShowNewFolderButton = true
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				string selectedPath = val.SelectedPath;
				Logger.Info("dialoge selected path.." + val.SelectedPath);
				bool flag = Utils.CheckWritePermissionForFolder(selectedPath);
				Logger.Info("Permission.." + flag + "..path.." + selectedPath);
				if (!flag)
				{
					ShowInvalidPathPopUp();
				}
				else
				{
					RegistryManager.Instance.ScreenShotsPath = selectedPath;
				}
			}
			else
			{
				RegistryManager.Instance.ScreenShotsPath = screenshotPath;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ShowInvalidPathPopUp()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		string defaultPicturePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bluestacks");
		CustomMessageWindow val = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
		val.AddButton((ButtonColors)4, "STRING_CHOOSE_ANOTHER", (EventHandler)ChooseCustomFolder, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_USE_DEFAULT", (EventHandler)delegate
		{
			RegistryManager.Instance.ScreenShotsPath = defaultPicturePath;
		}, (string)null, false, (object)null);
		((UIElement)val.BodyTextBlockTitle).Visibility = (Visibility)0;
		val.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_INVALID_PATH", "");
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val.BodyTextBlockTitle, TextBlock.ForegroundProperty, "DeleteComboTextForeground");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_USE_DEFAULT", "");
		((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
		val.BodyWarningTextBlock.Text = defaultPicturePath;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
		((Window)val).Close();
	}

	public void ShakeButtonHandler()
	{
		ParentWindow.Utils.ShakeWindow();
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("shake");
	}

	public void BackButtonHandler(bool receivedFromImap = false)
	{
		if (ParentWindow.mGuestBootCompleted)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				VmCmdHandler.RunCommand("back", ParentWindow.mVmName);
			});
			thread.IsBackground = true;
			thread.Start();
			if (ParentWindow.SendClientActions && !receivedFromImap)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				Dictionary<string, string> dictionary2 = new Dictionary<string, string> { { "EventAction", "BackButton" } };
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)0;
				dictionary.Add("operationData", JsonConvert.SerializeObject((object)dictionary2, serializerSettings));
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
			}
		}
	}

	public void OpenBrowserInPopup(Dictionary<string, string> payload)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				string localizedString = LocaleStrings.GetLocalizedString(payload["click_action_title"], "");
				string text = payload["click_action_value"].Trim();
				string urlWithParams = WebHelper.GetUrlWithParams(text);
				ClientStats.SendPopupBrowserStatsInMiscASync("request", text);
				PopupBrowserControl popupBrowserControl = new PopupBrowserControl();
				popupBrowserControl.Init(urlWithParams, localizedString, ParentWindow);
				ClientStats.SendPopupBrowserStatsInMiscASync("impression", text);
				ParentWindow.ShowDimOverlay(popupBrowserControl);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't open popup. An exception occured. {0}", new object[1] { ex });
			}
		}, new object[0]);
	}

	public void HomeButtonHandler(bool isLaunch = true, bool receivedFromImap = false)
	{
		ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", isLaunch);
		if (ParentWindow.SendClientActions && !receivedFromImap)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>
			{
				{ "EventAction", "HomeButton" },
				{
					"IsLaunch",
					isLaunch.ToString(CultureInfo.InvariantCulture)
				}
			};
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)0;
			dictionary.Add("operationData", JsonConvert.SerializeObject((object)dictionary2, serializerSettings));
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
		}
	}

	public void FullScreenButtonHandler(string source, string actionPerformed)
	{
		if (!ParentWindow.mResizeHandler.IsMinMaxEnabled)
		{
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (ParentWindow.mIsFullScreen)
			{
				ParentWindow.RestoreWindows();
				ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
				ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "RestoreFullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			}
			else
			{
				ParentWindow.FullScreenWindow();
				ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
				ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "Fullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			}
		}, new object[0]);
	}

	internal void AddToastPopup(Window window, string message, double duration = 1.3, bool isShowCloseImage = false)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CustomToastPopupControl val = new CustomToastPopupControl(window);
				if (isShowCloseImage)
				{
					val.Init(window, message, (Brush)(object)Brushes.Black, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)0, (Thickness?)null, 12, (Thickness?)null, (Brush)null, isShowCloseImage);
					((FrameworkElement)val).Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
				}
				else
				{
					val.Init(window, message, (Brush)(object)Brushes.Black, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)1, (Thickness?)null, 12, (Thickness?)null, (Brush)null, false);
				}
				val.ShowPopup(duration);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}, new object[0]);
	}

	internal void HandleClientOperation(string operationString)
	{
		try
		{
			JObject val = JObject.Parse(operationString);
			switch ((string)val["EventAction"])
			{
			case "RunApp":
				ParentWindow.mAppHandler.SendRunAppRequestAsync((string)val["Package"], (string)val["Activity"], receivedFromImap: true);
				break;
			case "BackButton":
				BackButtonHandler(receivedFromImap: true);
				break;
			case "HomeButton":
			{
				bool isLaunch = val["IsLaunch"].ToObject<bool>();
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					HomeButtonHandler(isLaunch, receivedFromImap: true);
				}, new object[0]);
				break;
			}
			case "TabSelected":
			{
				string tabKey2 = val["tabKey"].ToObject<string>();
				if (!string.IsNullOrEmpty(tabKey2))
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.mTopBar.mAppTabButtons.GoToTab(tabKey2, isLaunch: true, receivedFromImap: true);
					}, new object[0]);
				}
				break;
			}
			case "TabClosed":
			{
				string tabKey = val["tabKey"].ToObject<string>();
				bool sendStopAppToAndroid = val["sendStopAppToAndroid"].ToObject<bool>();
				bool forceClose = val["forceClose"].ToObject<bool>();
				if (!string.IsNullOrEmpty(tabKey))
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.mTopBar.mAppTabButtons.CloseTab(tabKey, sendStopAppToAndroid, forceClose, dontCheckQuitPopup: true, receivedFromImap: true);
					}, new object[0]);
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleClientOperation. OperationString: " + operationString + " Error:" + ex);
		}
	}

	private bool CheckForMacroVisibility()
	{
		if (!ParentWindow.mAppHandler.IsOneTimeSetupCompleted)
		{
			return true;
		}
		if (ShowMacroForSelectedApp(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey))
		{
			return true;
		}
		return false;
	}

	private static bool ShowMacroForSelectedApp(string appPackage)
	{
		if (PromotionObject.Instance.AppSpecificRulesList != null)
		{
			foreach (string appSpecificRules in PromotionObject.Instance.AppSpecificRulesList)
			{
				string text = appSpecificRules;
				if (appSpecificRules.EndsWith("*", StringComparison.InvariantCulture))
				{
					text = appSpecificRules.Substring(0, appSpecificRules.Length - 2);
				}
				if (text.StartsWith("~", StringComparison.InvariantCulture))
				{
					if (appPackage.StartsWith(text.Substring(1), StringComparison.InvariantCulture))
					{
						return false;
					}
				}
				else if (appPackage.StartsWith(text, StringComparison.InvariantCulture))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsCustomCursorEnableForApp(string appPackage)
	{
		try
		{
			if (!RegistryManager.Instance.CustomCursorEnabled || !FeatureManager.Instance.IsCustomCursorEnabled)
			{
				return false;
			}
			string empty = string.Empty;
			if (PromotionObject.Instance.CustomCursorExcludedAppsList != null)
			{
				foreach (string customCursorExcludedApps in PromotionObject.Instance.CustomCursorExcludedAppsList)
				{
					empty = customCursorExcludedApps;
					if (customCursorExcludedApps.EndsWith("*", StringComparison.InvariantCulture))
					{
						empty = customCursorExcludedApps.Substring(0, customCursorExcludedApps.Length - 1);
					}
					if (empty.StartsWith("~", StringComparison.InvariantCulture))
					{
						if (appPackage.StartsWith(empty.Substring(1), StringComparison.InvariantCulture))
						{
							return true;
						}
					}
					else if (appPackage.StartsWith(empty, StringComparison.InvariantCulture))
					{
						return false;
					}
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	internal void SetCustomCursorForApp(string appPackage)
	{
		ToggleCursorStyle(IsCustomCursorEnableForApp(appPackage));
	}

	internal void ClipMouseCursorHandler(bool forceDisable = false, bool switchState = true, string statAction = "", string sourceLocation = "")
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				return;
			}
			if (forceDisable)
			{
				ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = false;
			}
			else if (switchState)
			{
				ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = !ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped;
			}
			if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped)
			{
				RECT val = default(RECT);
				if (ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
				{
					ParentWindow.StaticComponents.mLastMappableWindowHandle = ParentWindow.mFrontendHandler.mFrontendHandle;
				}
				InteropWindow.GetWindowRect(ParentWindow.StaticComponents.mLastMappableWindowHandle, ref val);
				Point location = new Point(((RECT)(ref val)).Left, ((RECT)(ref val)).Top);
				Size size = new Size(((RECT)(ref val)).Right - ((RECT)(ref val)).Left, ((RECT)(ref val)).Bottom - ((RECT)(ref val)).Top);
				Cursor.Clip = new Rectangle(location, size);
				ParentWindow.OnCursorLockChanged(locked: true);
				ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
				if (!string.IsNullOrEmpty(statAction))
				{
					ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "LockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
					if (RegistryManager.Instance.IsShowToastNotification)
					{
						ParentWindow.ShowGeneralToast(string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNLOCK_CURSOR", ""), new object[1] { GetShortcutKeyFromName("STRING_TOGGLE_LOCK_CURSOR") }));
					}
				}
			}
			else
			{
				Cursor.Clip = Rectangle.Empty;
				ParentWindow.OnCursorLockChanged(locked: false);
				ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
				if (!string.IsNullOrEmpty(statAction))
				{
					ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "UnlockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ClipMouseCursorHandler. Exception: " + ex.ToString());
		}
	}

	internal string GetShortcutKeyFromName(string shortcutName, bool isBossKey = false)
	{
		try
		{
			if (mShortcutsConfigInstance == null)
			{
				return shortcutName switch
				{
					"STRING_TOGGLE_LOCK_CURSOR" => "Ctrl + Shift + F8", 
					"STRING_TOGGLE_KEYMAP_WINDOW" => "Ctrl + Shift + H", 
					"STRING_TOGGLE_OVERLAY" => "Ctrl + Shift + F6", 
					_ => "", 
				};
			}
			foreach (ShortcutKeys item in mShortcutsConfigInstance.Shortcut)
			{
				if (string.Equals(item.ShortcutName, shortcutName, StringComparison.InvariantCulture))
				{
					if (isBossKey)
					{
						return item.ShortcutKey;
					}
					string[] array = item.ShortcutKey.Split(new char[2] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					string text = string.Empty;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						text = text + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text2), "") + " + ";
					}
					if (!string.IsNullOrEmpty(text))
					{
						return text.Substring(0, text.Length - 3);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetShortcutKeyFromName: " + ex.ToString());
		}
		return "";
	}

	internal static void SaveMacroJson(MacroRecording record, string destFileName)
	{
		try
		{
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)1;
			string contents = JsonConvert.SerializeObject((object)record, serializerSettings);
			if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
			}
			File.WriteAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, Path.GetFileName(destFileName.ToLower(CultureInfo.InvariantCulture).Trim())), contents);
		}
		catch (Exception ex)
		{
			Logger.Error("Could not serialize the macro recording object. Ex: {0}", new object[1] { ex });
		}
	}

	internal void ToggleMacroAndSyncVisibility()
	{
		try
		{
			if (FeatureManager.Instance.ForceEnableMacroAndSync)
			{
				OnMacroButtonVisibilityChanged(isVisible: true);
				OnOperationSyncButtonVisibilityChanged(isVisible: true);
			}
			else if (FeatureManager.Instance.IsMacroRecorderEnabled || FeatureManager.Instance.IsOperationsSyncEnabled)
			{
				bool isVisible = CheckForMacroVisibility();
				if (FeatureManager.Instance.IsMacroRecorderEnabled)
				{
					OnMacroButtonVisibilityChanged(isVisible);
				}
				if (FeatureManager.Instance.IsOperationsSyncEnabled)
				{
					OnOperationSyncButtonVisibilityChanged(isVisible);
				}
			}
			else
			{
				OnMacroButtonVisibilityChanged(isVisible: false);
				OnOperationSyncButtonVisibilityChanged(isVisible: false);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ToggleMacroAndSyncVisibility: " + ex.ToString());
		}
	}

	private void ToggleCursorStyle(bool enable)
	{
		try
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			if (enable)
			{
				data.Add("path", RegistryStrings.CursorPath);
			}
			else
			{
				data.Add("path", string.Empty);
			}
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					HTTPUtils.SendRequestToEngine("setCursorStyle", data, ParentWindow.mVmName, 3000, (Dictionary<string, string>)null, false, 1, 0, "");
					SetDefaultCursorForClient();
				}
				catch (Exception ex2)
				{
					Logger.Error("Failed to send Show event to engine... err : " + ex2.ToString());
					SetDefaultCursorForClient();
				}
			});
		}
		catch (Exception)
		{
			SetDefaultCursorForClient();
		}
	}

	private void SetDefaultCursorForClient()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				try
				{
					Mouse.OverrideCursor = null;
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to set default cursor for client... err : " + ex.ToString());
				}
			}, new object[0]);
		});
	}

	public void LaunchSettingsWindow(string tabName = "")
	{
		if (MainWindow.SettingsWindow == null)
		{
			MainWindow.OpenSettingsWindow(ParentWindow, tabName);
		}
	}

	public void DMMSwitchKeyMapButtonHandler()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.EndsWith("_off", StringComparison.InvariantCulture))
			{
				ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.Replace("_off", string.Empty);
				BlueStacksUIBinding.Bind((Image)(object)ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
				BlueStacksUIBinding.Bind((Image)(object)ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
				ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: true);
				ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(enable: true);
			}
			else
			{
				CustomPictureBox mKeyMapSwitch = ParentWindow.mDmmBottomBar.mKeyMapSwitch;
				mKeyMapSwitch.ImageName += "_off";
				BlueStacksUIBinding.Bind((Image)(object)ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
				BlueStacksUIBinding.Bind((Image)(object)ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
				ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: false);
				ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(enable: false);
			}
			ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName;
		}, new object[0]);
	}

	public void SetDMMKeymapButtonsAndTransparency()
	{
		if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapUIVisible)
		{
			ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm((Visibility)0);
			ParentWindow.mDmmBottomBar.ShowKeyMapPopup(isShow: true);
			KMManager.ShowOverlayWindow(ParentWindow, isShow: true, isreload: true);
			BlueStacksUIBinding.Bind((Image)(object)ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
			if (ParentWindow.mDmmBottomBar.CurrentTransparency > 0.0)
			{
				SetTranslucentControlsBtnImageForDMM("eye");
			}
			else
			{
				SetTranslucentControlsBtnImageForDMM("eye_off");
			}
		}
		else
		{
			ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm((Visibility)2);
			ParentWindow.mDmmBottomBar.ShowKeyMapPopup(isShow: false);
			KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
		}
		if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapEnabled)
		{
			ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch";
			ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch";
			ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: true);
		}
		else
		{
			ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch_off";
			ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch_off";
			ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: false);
		}
	}

	public void EnableKeymapButtonsForDmm(Visibility isVisible)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)ParentWindow.mDmmBottomBar.mKeyMapButton).Visibility = isVisible;
		((UIElement)ParentWindow.mDmmBottomBar.mKeyMapSwitch).Visibility = isVisible;
		((UIElement)ParentWindow.mDmmBottomBar.mTranslucentControlsButton).Visibility = isVisible;
		((UIElement)ParentWindow.mDMMFST.mKeyMapButton).Visibility = isVisible;
		((UIElement)ParentWindow.mDMMFST.mKeyMapSwitch).Visibility = isVisible;
		((UIElement)ParentWindow.mDMMFST.mTranslucentControlsButton).Visibility = isVisible;
	}

	internal void SetTranslucentControlsBtnImageForDMM(string imageName)
	{
		ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName = imageName;
		ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton.ImageName = ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
		ParentWindow.mDMMFST.mTranslucentControlsButton.ImageName = ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
		ParentWindow.mDMMFST.mTranslucentControlsSliderButton.ImageName = ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
	}

	internal void KeyMapButtonHandler(string action, string location)
	{
		KMManager.ShowAdvancedSettings(ParentWindow);
		ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "KeyMap", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	public void DMMScreenshotHandler()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		FolderBrowserDialog val = new FolderBrowserDialog
		{
			ShowNewFolderButton = true,
			Description = LocaleStrings.GetLocalizedString("STRING_CHOOSE_SCREENSHOT_FOLDER_TEXT", "")
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog(Utils.GetIWin32Window(ParentWindow.Handle)) == 1 && !string.IsNullOrEmpty(val.SelectedPath))
			{
				string screenShotsPath = (Directory.Exists(val.SelectedPath) ? val.SelectedPath : RegistryStrings.ScreenshotDefaultPath);
				RegistryManager.Instance.ScreenShotsPath = screenShotsPath;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RecordVideoOfApp()
	{
		Logger.Debug("OBS start or stop status: {0}", new object[1] { sIsOBSStartingStopping });
		if (sIsOBSStartingStopping)
		{
			return;
		}
		sIsOBSStartingStopping = true;
		if (RegistryManager.Instance.IsScreenshotsLocationPopupEnabled)
		{
			ShowScreenShotFolderUpdatePopup();
		}
		string text = RegistryManager.Instance.ScreenShotsPath;
		if (!StringExtensions.IsValidPath(text))
		{
			if (!Directory.Exists(RegistryStrings.ScreenshotDefaultPath))
			{
				Directory.CreateDirectory(RegistryStrings.ScreenshotDefaultPath);
			}
			RegistryManager.Instance.ScreenShotsPath = RegistryStrings.ScreenshotDefaultPath;
			text = RegistryStrings.ScreenshotDefaultPath;
		}
		string text2 = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss.ff", CultureInfo.InvariantCulture);
		string path = string.Format(CultureInfo.InvariantCulture, Strings.ProductTopBarDisplayName + "_Recording_{0}.mp4", new object[1] { text2 });
		string filePath = Path.Combine(text, path);
		if (text == RegistryStrings.ScreenshotDefaultPath && !Directory.Exists(RegistryStrings.ScreenshotDefaultPath))
		{
			Directory.CreateDirectory(RegistryStrings.ScreenshotDefaultPath);
		}
		ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarting", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				sRecordingInstance = ParentWindow.mVmName;
				if (StreamManager.Instance == null)
				{
					StreamManager.Instance = new StreamManager(ParentWindow);
				}
				string text3 = ParentWindow.mFrontendHandler.mFrontendHandle.ToString();
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.RestrictWindowResize(enable: true);
					OnScreenRecorderStateTransitioning();
					StartLoadingTimeoutTimer();
				}, new object[0]);
				Process currentProcess = Process.GetCurrentProcess();
				StreamManager.Instance.Init(text3, currentProcess.Id.ToString(CultureInfo.InvariantCulture));
				StreamManager.sStopInitOBSQueue = false;
				try
				{
					StreamManager.Instance.StartObs();
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in StartObs: {0}", new object[1] { ex });
					ShowErrorRecordingVideoPopup();
					return;
				}
				StreamManager.Instance.SetMicVolume("0");
				StreamManager.Instance.SetHwnd(text3);
				StreamManager.Instance.SetSavePath(filePath);
				mSavedVideoRecordingFilePath = filePath;
				StreamManager.Instance.EnableVideoRecording(enable: true);
				StreamManager.Instance.StartRecordForVideo();
				sIsRecordingVideo = true;
			}
			catch (Exception ex2)
			{
				Logger.Error("Error in RecordVideoOfApp: {0}", new object[1] { ex2 });
			}
		});
	}

	private void StartLoadingTimeoutTimer()
	{
		if (mObsResponseTimeoutTimer == null)
		{
			mObsResponseTimeoutTimer = new System.Timers.Timer(20000.0);
			mObsResponseTimeoutTimer.Elapsed += ObsResponseTimeoutTimer_Elapsed;
			mObsResponseTimeoutTimer.AutoReset = false;
		}
		if (!mObsResponseTimeoutTimer.Enabled)
		{
			mObsResponseTimeoutTimer.Start();
		}
	}

	private void ObsResponseTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		OnOBSResponseTimeout();
		sIsRecordingVideo = false;
		sIsOBSStartingStopping = false;
		sRecordingInstance = "";
		ParentWindow.RestrictWindowResize(enable: false);
		if (StreamManager.Instance != null)
		{
			StreamManager.Instance.ShutDownForcefully();
		}
		ShowErrorRecordingVideoPopup();
	}

	internal void StopRecordVideo()
	{
		try
		{
			OnScreenRecorderStateTransitioning();
			StartLoadingTimeoutTimer();
			StreamManager.Instance.StopRecord();
		}
		catch (Exception ex)
		{
			Logger.Error("error in stop record video : {0}", new object[1] { ex });
		}
	}

	internal void RecordingStopped()
	{
		mObsResponseTimeoutTimer?.Stop();
		ParentWindow.RestrictWindowResize(enable: false);
		OnScreenRecordingStateChanged(isRecording: false);
		ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingDone", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	internal void DownloadAndLaunchRecording(string location, string action)
	{
		Logger.Debug("value of sRecordingInstance: {0} and sIsRecordingVideo: {1}", new object[2] { sRecordingInstance, sIsRecordingVideo });
		if (sIsRecordingVideo)
		{
			if (string.Equals(sRecordingInstance, ParentWindow.mVmName, StringComparison.InvariantCulture))
			{
				StopRecordVideo();
				ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStop", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			}
			else
			{
				ShowAlreadyRunningPopUpForOBS();
			}
			return;
		}
		if (Directory.Exists(RegistryStrings.ObsDir) && File.Exists(RegistryStrings.ObsBinaryPath))
		{
			if (!RegistryManager.Instance.IsBTVCheckedAfterUpdate && !IsBtvLatestVersionDownloaded())
			{
				DownloadObsPopup();
				ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			}
			else if (!ProcessUtils.FindProcessByName("HD-OBS"))
			{
				if (!InsufficientSpacePopup())
				{
					RecordVideoOfApp();
					ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStart", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
			}
			else
			{
				ShowAlreadyRunningPopUpForOBS();
			}
		}
		else
		{
			DownloadObsPopup();
			ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		RegistryManager.Instance.IsBTVCheckedAfterUpdate = true;
	}

	private bool InsufficientSpacePopup()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c0: Expected O, but got Unknown
		bool recording = true;
		double num = FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath);
		MouseButtonEventHandler val2 = default(MouseButtonEventHandler);
		while (num < 30.0 && recording)
		{
			RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
			string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_INSUFFICIENT_SPACE", "");
			val.AddButton((ButtonColors)4, "STRING_CHANGE_PATH", (EventHandler)ChooseCustomFolder, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_STOP_RECORDING", (EventHandler)delegate
			{
				recording = false;
			}, (string)null, false, (object)null);
			CustomPictureBox closeButton = val.CloseButton;
			MouseButtonEventHandler obj = val2;
			if (obj == null)
			{
				MouseButtonEventHandler val3 = delegate
				{
					recording = false;
				};
				MouseButtonEventHandler val4 = val3;
				val2 = val3;
				obj = val4;
			}
			((UIElement)closeButton).PreviewMouseUp += obj;
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_INSUFFICIENT_RECORDING_SPACE", "");
			((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
			val.BodyWarningTextBlock.Text = screenShotsPath;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
			num = FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath);
		}
		return !recording;
	}

	private static double FindAvailableSpaceinMB(string path)
	{
		double result = double.MaxValue;
		string pathRoot = Path.GetPathRoot(path);
		double num = Math.Pow(2.0, 20.0);
		DriveInfo[] drives = DriveInfo.GetDrives();
		foreach (DriveInfo driveInfo in drives)
		{
			if (driveInfo.IsReady && driveInfo.Name == pathRoot)
			{
				result = (double)driveInfo.AvailableFreeSpace / num;
			}
		}
		return result;
	}

	private void ShowAlreadyRunningPopUpForOBS()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageWindow val = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_NOT_START_RECORDER", "");
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_RECORDER_ALREADY_RUNNING", "");
		val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow;
		ParentWindow.ShowDimOverlay();
		((Window)val).ShowDialog();
		ParentWindow.HideDimOverlay();
	}

	private static bool IsBtvLatestVersionDownloaded()
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(new Uri(GetBtvUrl()).LocalPath);
		if (string.Compare(RegistryManager.Instance.CurrentBtvVersionInstalled, fileNameWithoutExtension, StringComparison.InvariantCulture) < 0)
		{
			return false;
		}
		return true;
	}

	private static string GetBtvUrl()
	{
		string url = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs4/btv/GetBTVFile");
		if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
		{
			url = RegistryManager.Instance.BtvDevServer;
		}
		return BTVManager.GetRedirectedUrl(url);
	}

	private void DownloadObsPopup()
	{
		if (sDownloading && sWindow != null && !((CustomWindow)sWindow).IsClosed)
		{
			DownloadObs(null, null);
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RECORDER_REQUIRED", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_VIDEO_RECORDER_DOWNLOAD_BODY", "");
			val.AddButton((ButtonColors)4, "STRING_DOWNLOAD_NOW", (EventHandler)DownloadObs, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)ParentWindow;
			val.ContentMaxWidth = 450.0;
			ParentWindow.ShowDimOverlay();
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}, new object[0]);
	}

	private void DownloadObs(object sender, EventArgs e)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		if (sDownloading && sWindow != null && !((CustomWindow)sWindow).IsClosed)
		{
			BTVManager.BringToFront((CustomWindow)(object)sWindow);
			return;
		}
		if (!BTVManager.IsDirectXComponentsInstalled())
		{
			CustomMessageWindow downloadReqWindow = new CustomMessageWindow();
			downloadReqWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
			downloadReqWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
			string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
			downloadReqWindow.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), (RequestNavigateEventHandler)delegate(object o, RequestNavigateEventArgs arg)
			{
				BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
				downloadReqWindow.CloseWindow();
			});
			downloadReqWindow.AddButton((ButtonColors)4, "STRING_DOWNLOAD_NOW", (EventHandler)delegate
			{
				BlueStacksUIUtils.OpenUrl(directXDownloadURL);
			}, (string)null, false, (object)null);
			((Window)downloadReqWindow).Owner = (Window)(object)ParentWindow;
			downloadReqWindow.ContentMaxWidth = 450.0;
			ParentWindow.ShowDimOverlay();
			((Window)downloadReqWindow).ShowDialog();
			ParentWindow.HideDimOverlay();
			return;
		}
		sDownloading = true;
		sWindow = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(sWindow.TitleTextBlock, "STRING_DOWNLOAD_ADDITIONAL", "");
		BlueStacksUIBinding.Bind(sWindow.BodyWarningTextBlock, "STRING_NOT_CLOSE_DOWNLOAD_COMPLETE", "");
		((UIElement)sWindow.BodyWarningTextBlock).Visibility = (Visibility)0;
		((UIElement)sWindow.BodyTextBlock).Visibility = (Visibility)2;
		sWindow.CloseButtonHandle((Predicate<object>)RecorderDownloadCancelledHandler, (object)null);
		CustomMessageWindow obj = sWindow;
		obj.MinimizeEventHandler = (EventHandler)Delegate.Combine(obj.MinimizeEventHandler, new EventHandler(BtvDownloadWindowMinimizedHandler));
		sWindow.ProgressBarEnabled = true;
		sWindow.IsWindowMinizable = true;
		sWindow.IsWindowClosable = true;
		((Window)sWindow).ShowInTaskbar = false;
		sWindow.IsWithoutButtons = true;
		sWindow.ContentMaxWidth = 450.0;
		sWindow.IsDraggable = true;
		((Window)sWindow).Owner = (Window)(object)ParentWindow;
		((CustomWindow)sWindow).IsShowGLWindow = true;
		((Window)sWindow).Show();
		Thread thread = new Thread((ThreadStart)delegate
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected O, but got Unknown
			//IL_00ba: Expected O, but got Unknown
			//IL_00ba: Expected O, but got Unknown
			//IL_00ba: Expected O, but got Unknown
			//IL_00ba: Expected O, but got Unknown
			string btvUrl = GetBtvUrl();
			if (btvUrl == null)
			{
				Logger.Error("The download url was null");
				ShowErrorDownloadingRecorder();
			}
			else
			{
				string fileName = Path.GetFileName(new Uri(btvUrl).LocalPath);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(new Uri(btvUrl).LocalPath);
				string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
				mDownloader = new LegacyDownloader(3, btvUrl, downloadPath);
				mDownloader.Download((UpdateProgressCallback)delegate(int percent)
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (sWindow != null)
						{
							((RangeBase)sWindow.CustomProgressBar).Value = percent;
						}
					}, new object[0]);
				}, (DownloadCompletedCallback)delegate
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (sWindow != null)
						{
							((RangeBase)sWindow.CustomProgressBar).Value = 100.0;
						}
					}, new object[0]);
					Logger.Info("Successfully downloaded BlueStacks TV");
					RegistryManager.Instance.CurrentBtvVersionInstalled = fileNameWithoutExtension;
					if (BTVManager.ExtractBTv(downloadPath))
					{
						Utils.DeleteFile(downloadPath);
						((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							//IL_0024: Unknown result type (might be due to invalid IL or missing references)
							//IL_0029: Unknown result type (might be due to invalid IL or missing references)
							//IL_003e: Unknown result type (might be due to invalid IL or missing references)
							//IL_0053: Unknown result type (might be due to invalid IL or missing references)
							//IL_0063: Unknown result type (might be due to invalid IL or missing references)
							//IL_006f: Unknown result type (might be due to invalid IL or missing references)
							CustomMessageWindow obj2 = sWindow;
							if (obj2 != null)
							{
								((Window)obj2).Close();
							}
							sWindow = null;
							if (!ParentWindow.mClosed)
							{
								CustomMessageWindow val = new CustomMessageWindow();
								BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_RECORDER_DOWNLOADED", "");
								BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_RECORDER_READY_BODY", "");
								val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
								((Window)val).Owner = (Window)(object)ParentWindow;
								val.ContentMaxWidth = 450.0;
								ParentWindow.ShowDimOverlay();
								((Window)val).ShowDialog();
								ParentWindow.HideDimOverlay();
							}
						}, new object[0]);
					}
					else
					{
						Utils.DeleteFile(downloadPath);
						ShowErrorDownloadingRecorder();
					}
				}, (ExceptionCallback)delegate(Exception ex)
				{
					Logger.Error("Failed to download file: {0}. err: {1}", new object[2] { downloadPath, ex.Message });
					if (!(ex.InnerException is OperationCanceledException))
					{
						ShowErrorDownloadingRecorder();
					}
				}, (ContentTypeCallback)null, (SizeDownloadedCallback)delegate(long size)
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (sWindow != null)
						{
							sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
							((ContentControl)sWindow.ProgressPercentageTextBlock).Content = ((float)size / 1048576f).ToString("F", CultureInfo.InvariantCulture) + " MB / " + mRecorderSizeMb.ToString("F", CultureInfo.InvariantCulture) + " MB ";
							mDownloadedSize = size;
						}
					}, new object[0]);
				}, (PayloadInfoCallback)delegate(long size)
				{
					mRecorderSizeMb = (float)size / 1048576f;
				});
				sDownloading = false;
			}
		});
		thread.IsBackground = true;
		thread.Start();
		mDownloadStatusTimer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 5)
		};
		mDownloadStatusTimer.Tick += DownloadStatusTimerTick;
		mDownloadStatusTimer.Start();
	}

	private void BtvDownloadWindowMinimizedHandler(object sender, EventArgs e)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			OnBTvDownloaderMinimized();
			((UIElement)ParentWindow).Focus();
		}, new object[0]);
	}

	private void DownloadStatusTimerTick(object sender, EventArgs e)
	{
		if ((!sDownloading && sWindow != null) || sWindow == null)
		{
			mDownloadStatusTimer.Stop();
			return;
		}
		try
		{
			if (mLastSizeChecked != mDownloadedSize)
			{
				mLastSizeChecked = mDownloadedSize;
				sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
			}
			else
			{
				sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_WAITING_FOR_INTERNET", "");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in DownloadStatusTimerTick. Exception: " + ex);
		}
	}

	private void ShowErrorDownloadingRecorder()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_DOWNLOAD_FAILED", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_ERROR_RECORDER_DOWNLOAD", "");
			val.AddButton((ButtonColors)4, "STRING_CLOSE", (EventHandler)null, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)ParentWindow;
			val.ContentMaxWidth = 450.0;
			ParentWindow.ShowDimOverlay();
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
			CustomMessageWindow obj = sWindow;
			if (obj != null)
			{
				((Window)obj).Close();
			}
			sWindow = null;
		}, new object[0]);
	}

	internal void ShowErrorRecordingVideoPopup()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_RECORDING_ERROR", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_RECORDING_ERROR_BODY", "");
			val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)ParentWindow;
			val.ContentMaxWidth = 450.0;
			ParentWindow.ShowDimOverlay();
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}, new object[0]);
	}

	private bool RecorderDownloadCancelledHandler(object sender)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		CustomMessageWindow cancelDownloadConfirmation = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(cancelDownloadConfirmation.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
		BlueStacksUIBinding.Bind(cancelDownloadConfirmation.BodyTextBlock, "STRING_DOWNLOAD_NOT_COMPLETE", "");
		cancelDownloadConfirmation.AddButton((ButtonColors)0, "STRING_CANCEL", (EventHandler)delegate
		{
			CustomMessageWindow obj = sWindow;
			if (obj != null)
			{
				((Window)obj).Close();
			}
			sWindow = null;
			LegacyDownloader obj2 = mDownloader;
			if (obj2 != null)
			{
				obj2.AbortDownload();
			}
		}, (string)null, false, (object)null);
		cancelDownloadConfirmation.AddButton((ButtonColors)2, "STRING_CONTINUE", (EventHandler)delegate
		{
			((Window)cancelDownloadConfirmation).DialogResult = true;
		}, (string)null, false, (object)null);
		((Window)cancelDownloadConfirmation).Owner = (Window)(object)ParentWindow;
		ParentWindow.ShowDimOverlay();
		bool? flag = ((Window)cancelDownloadConfirmation).ShowDialog();
		ParentWindow.HideDimOverlay();
		return flag == true;
	}

	public void RecordingStarted()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			sIsOBSStartingStopping = false;
			mObsResponseTimeoutTimer?.Stop();
			OnScreenRecordingStateChanged(isRecording: true);
			if (RegistryManager.Instance.IsShowToastNotification)
			{
				ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_STARTED", ""));
			}
		}, new object[0]);
		ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarted", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	public void StopMacroRecording()
	{
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopRecordingCombo");
		foreach (SingleMacroControl child in ((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children)
		{
			EnableScriptControl(child);
		}
		((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Visibility = (Visibility)0;
		((UIElement)ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn).Visibility = (Visibility)2;
		((UIElement)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Visibility = (Visibility)0;
		((Popup)ParentWindow.mTopBar.mMacroRecorderToolTipPopup).IsOpen = false;
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)ParentWindow.mNCTopBar.mMacroRecordGrid).Visibility = (Visibility)2;
			ParentWindow.mNCTopBar.mMacroRecordControl.StopTimer();
		}
		else
		{
			((UIElement)ParentWindow.mTopBar.mMacroRecordControl).Visibility = (Visibility)2;
			ParentWindow.mTopBar.mMacroRecordControl.StopTimer();
		}
	}

	public void StartMacroRecording()
	{
		ParentWindow.mIsMacroRecorderActive = true;
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ParentWindow.mNCTopBar.ShowRecordingIcons();
		}
		else
		{
			ParentWindow.mTopBar.ShowRecordingIcons();
		}
		((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Visibility = (Visibility)2;
		((UIElement)ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn).Visibility = (Visibility)0;
		foreach (SingleMacroControl child in ((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children)
		{
			DisableScriptControl(child);
		}
		ParentWindow.mCommonHandler.HideMacroRecorderWindow();
		((UIElement)ParentWindow).Focus();
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo");
	}

	internal void InitUiOnMacroPlayback(MacroRecording recording)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)ParentWindow).Focus();
			((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).IsEnabled = false;
			((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Opacity = 0.6;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				ParentWindow.mNCTopBar.ShowMacroPlaybackOnTopBar(recording);
				ParentWindow.mNCTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
			}
			else
			{
				ParentWindow.mTopBar.ShowMacroPlaybackOnTopBar(recording);
				ParentWindow.mTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
			}
			ParentWindow.mMacroPlaying = recording.Name;
			if (recording.RestartPlayer)
			{
				ParentWindow.StartTimerForAppPlayerRestart(recording.RestartPlayerAfterMinutes);
			}
		}, new object[0]);
	}

	internal void PlayMacroScript(MacroRecording record)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Visibility = (Visibility)0;
			((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).IsEnabled = false;
			((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Opacity = 0.6;
			foreach (SingleMacroControl child in ((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children)
			{
				if (child.mRecording.Name != record.Name)
				{
					DisableScriptControl(child);
				}
				else
				{
					((UIElement)child.mEditNameImg).IsEnabled = false;
				}
			}
			ParentWindow.MacroRecorderWindow.RunMacroOperation(record);
		}, new object[0]);
	}

	internal void FullMacroScriptPlayHandler(MacroRecording record)
	{
		string name = record.Name;
		ParentWindow.mCommonHandler.PlayMacroScript(record);
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
		}
		else
		{
			ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
		}
	}

	internal void StopMacroScriptHandling()
	{
		ParentWindow.MacroRecorderWindow.mBGMacroPlaybackWorker.CancelAsync();
		StopMacroPlaybackOperation();
		ParentWindow.SetMacroPlayBackEventHandle();
	}

	internal void StopMacroPlaybackOperation()
	{
		Logger.Info("In StopMacroPlaybackOperation");
		ParentWindow.mIsMacroPlaying = false;
		foreach (SingleMacroControl child in ((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children)
		{
			EnableScriptControl(child);
		}
		((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Visibility = (Visibility)0;
		((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).IsEnabled = true;
		((UIElement)ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn).Opacity = 1.0;
		((UIElement)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Visibility = (Visibility)0;
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ParentWindow.mNCTopBar.HideMacroPlaybackFromTopBar();
		}
		else
		{
			ParentWindow.mTopBar.HideMacroPlaybackFromTopBar();
		}
		ParentWindow.mMacroPlaying = string.Empty;
		if (ParentWindow.mMacroTimer != null && ParentWindow.mMacroTimer.Enabled)
		{
			ParentWindow.mMacroTimer.Enabled = false;
			ParentWindow.mMacroTimer.AutoReset = false;
			ParentWindow.mMacroTimer.Dispose();
		}
		((Popup)ParentWindow.mTopBar.mMacroRunningToolTipPopup).IsOpen = false;
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopMacroPlayback");
	}

	public static void EnableScriptControl(SingleMacroControl mScriptControl)
	{
		((UIElement)mScriptControl).Opacity = 1.0;
		((UIElement)mScriptControl.mBookmarkImg).IsEnabled = true;
		((UIElement)mScriptControl.mEditNameImg).IsEnabled = true;
		((UIElement)mScriptControl.mPlayScriptImg).IsEnabled = true;
		((UIElement)mScriptControl.mScriptSettingsImg).IsEnabled = true;
		((UIElement)mScriptControl.mDeleteScriptImg).IsEnabled = true;
	}

	public static void DisableScriptControl(SingleMacroControl mScriptControl)
	{
		((UIElement)mScriptControl).Opacity = 0.4;
		((UIElement)mScriptControl.mBookmarkImg).IsEnabled = false;
		((UIElement)mScriptControl.mEditNameImg).IsEnabled = false;
		((UIElement)mScriptControl.mPlayScriptImg).IsEnabled = false;
		((UIElement)mScriptControl.mScriptSettingsImg).IsEnabled = false;
		((UIElement)mScriptControl.mDeleteScriptImg).IsEnabled = false;
	}

	internal void CheckForMacroScriptOnRestart()
	{
		foreach (MacroRecording item in from MacroRecording macro in MacroGraph.Instance.Vertices
			where macro.PlayOnStart
			select macro)
		{
			InitUiAndPlayMacroScript(item);
		}
	}

	private void InitUiAndPlayMacroScript(MacroRecording record)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			RefreshMacroRecorderWindow();
			ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(record.Name.ToLower(CultureInfo.InvariantCulture));
			PlayMacroScript(record);
		}, new object[0]);
	}

	public static void OpenMediaFolder()
	{
		if (Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
		{
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = RegistryManager.Instance.ScreenShotsPath;
				process.Start();
			}
		}
	}

	public static void OpenMediaFolderWithFileSelected(string selectedFile)
	{
		if (Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
		{
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = "explorer.exe";
				process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "/select,\"{0}\"", new object[1] { selectedFile });
				process.Start();
			}
		}
	}

	internal void SetSidebarImageProperties(bool isVisible, CustomPictureBox cpb, TextBlock tb)
	{
		if (isVisible)
		{
			if (cpb != null)
			{
				cpb.ImageName = "sidebar_hide";
				BlueStacksUIBinding.Bind((Image)(object)cpb, "STRING_CLOSE_SIDEBAR");
			}
			if (tb != null)
			{
				BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
			}
		}
		else
		{
			if (cpb != null)
			{
				cpb.ImageName = "sidebar_show";
				BlueStacksUIBinding.Bind((Image)(object)cpb, "STRING_OPEN_SIDEBAR");
			}
			if (tb != null)
			{
				BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
			}
		}
	}

	internal void FlipSidebarVisibility(CustomPictureBox cpb, TextBlock tb)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (cpb.ImageName == "sidebar_hide")
		{
			((UIElement)ParentWindow.mSidebar).Visibility = (Visibility)2;
			cpb.ImageName = "sidebar_show";
			BlueStacksUIBinding.Bind((Image)(object)cpb, "STRING_OPEN_SIDEBAR");
			if (tb != null)
			{
				BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
			}
			ParentWindow.EngineInstanceRegistry.IsSidebarVisible = false;
		}
		else
		{
			((UIElement)ParentWindow.mSidebar).Visibility = (Visibility)0;
			cpb.ImageName = "sidebar_hide";
			BlueStacksUIBinding.Bind((Image)(object)cpb, "STRING_CLOSE_SIDEBAR");
			if (tb != null)
			{
				BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
			}
			ParentWindow.EngineInstanceRegistry.IsSidebarVisible = true;
		}
		ParentWindow.mSidebar.SidebarVisiblityChanged(((UIElement)ParentWindow.mSidebar).Visibility);
	}

	internal void InitShortcuts()
	{
		try
		{
			mShortcutsConfigInstance = ShortcutConfig.LoadShortcutsConfig();
			if (mShortcutsConfigInstance == null)
			{
				return;
			}
			List<ShortcutKeys> list = new List<ShortcutKeys>();
			foreach (ShortcutKeys item in mShortcutsConfigInstance.Shortcut)
			{
				if (string.Equals(item.ShortcutName, "STRING_MACRO_RECORDER", StringComparison.InvariantCulture))
				{
					if (!FeatureManager.Instance.IsMacroRecorderEnabled && !FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						list.Add(item);
					}
				}
				else if (string.Equals(item.ShortcutName, "STRING_SYNCHRONISER", StringComparison.InvariantCulture))
				{
					if (!FeatureManager.Instance.IsOperationsSyncEnabled)
					{
						list.Add(item);
					}
				}
				else if (string.Equals(item.ShortcutName, "STRING_TOGGLE_FARM_MODE", StringComparison.InvariantCulture) && FeatureManager.Instance.IsFarmingModeDisabled)
				{
					list.Add(item);
				}
			}
			foreach (ShortcutKeys item2 in list)
			{
				mShortcutsConfigInstance.Shortcut.Remove(item2);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error while init shortcut : {0}", new object[1] { ex });
		}
	}

	internal void SaveAndReloadShortcuts()
	{
		try
		{
			UsefulExtensionMethod.SaveUserDefinedShortcuts(mShortcutsConfigInstance);
			ReloadShortcutsForAllInstances();
			Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_save", (string)null, (string)null, (string)null, (string)null, (string)null, "Android", 0);
		}
		catch (Exception ex)
		{
			Logger.Error("Error saving shortcut registry" + ex.ToString());
		}
	}

	internal static void ReloadShortcutsForAllInstances()
	{
		foreach (string runningInstances in Utils.GetRunningInstancesList())
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(runningInstances))
			{
				BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.InitShortcuts();
				BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.ReloadBossKey();
				BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.ReloadTooltips();
			}
			HTTPUtils.SendRequestToEngineAsync("reloadShortcutsConfig", (Dictionary<string, string>)null, runningInstances, 0, (Dictionary<string, string>)null, false, 1, 0);
		}
	}

	internal void ReloadTooltips()
	{
		foreach (SidebarElement mListSidebarElement in ParentWindow.mSidebar.mListSidebarElements)
		{
			ParentWindow.mSidebar.SetSidebarElementTooltip(mListSidebarElement, mListSidebarElement.mSidebarElementTooltipKey);
		}
	}

	private void ReloadBossKey()
	{
		RegistryManager.Instance.BossKey = GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", isBossKey: true);
		if (string.IsNullOrEmpty(RegistryManager.Instance.BossKey))
		{
			GlobalKeyBoardMouseHooks.UnsetKey();
		}
		else
		{
			GlobalKeyBoardMouseHooks.SetKey(RegistryManager.Instance.BossKey);
		}
	}

	internal static void ArrangeWindowInTiles()
	{
		long columns = RegistryManager.Instance.TileWindowColumnCount;
		long rows = (long)Math.Ceiling((double)BlueStacksUIUtils.DictWindows.Count / (double)columns);
		double num = Screen.PrimaryScreen.WorkingArea.Height;
		double y = Screen.PrimaryScreen.WorkingArea.Top;
		double x = Screen.PrimaryScreen.WorkingArea.Left;
		int num2 = 0;
		foreach (KeyValuePair<string, MainWindow> item in BlueStacksUIUtils.DictWindows)
		{
			double windowWidth = Screen.PrimaryScreen.WorkingArea.Width / columns;
			double windowHeight = Screen.PrimaryScreen.WorkingArea.Height / rows;
			double overlapWidth = 0.0;
			double overlapHeight = 0.0;
			((DispatcherObject)item.Value).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Invalid comparison between Unknown and I4
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Invalid comparison between Unknown and I4
				if ((int)((Window)item.Value).WindowState == 1 || (int)((Window)item.Value).WindowState == 2)
				{
					item.Value.RestoreWindows(isReArrange: true);
				}
				KMManager.CloseWindows();
				if (item.Value.mAspectRatio < 1L)
				{
					if (item.Value.GetWidthFromHeight(windowHeight, isScaled: true, isIgnoreMinWidth: true) > windowWidth)
					{
						windowHeight = item.Value.GetHeightFromWidth(windowWidth, isScaled: true, isIgnoreMinWidth: true);
					}
					else
					{
						windowWidth = item.Value.GetWidthFromHeight(windowHeight, isScaled: true, isIgnoreMinWidth: true);
					}
					if (windowWidth < (double)item.Value.MinWidthScaled)
					{
						windowWidth = item.Value.MinWidthScaled;
						windowHeight = item.Value.GetHeightFromWidth(windowWidth, isScaled: true, isIgnoreMinWidth: true);
						CalculateOverlappingLength(windowWidth, windowHeight, rows, columns, out overlapWidth, out overlapHeight);
					}
					item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
				}
				else
				{
					if (item.Value.GetHeightFromWidth(windowWidth, isScaled: true, isIgnoreMinWidth: true) > windowHeight)
					{
						windowWidth = item.Value.GetWidthFromHeight(windowHeight, isScaled: true, isIgnoreMinWidth: true);
					}
					else
					{
						windowHeight = item.Value.GetHeightFromWidth(windowWidth, isScaled: true, isIgnoreMinWidth: true);
					}
					if (windowHeight < (double)item.Value.MinHeightScaled)
					{
						windowHeight = item.Value.MinHeightScaled;
						windowWidth = item.Value.GetWidthFromHeight(windowHeight, isScaled: true, isIgnoreMinWidth: true);
						CalculateOverlappingLength(windowWidth, windowHeight, rows, columns, out overlapWidth, out overlapHeight);
					}
					item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
				}
				if (!((Window)item.Value).Topmost)
				{
					((Window)item.Value).Topmost = true;
					ThreadPool.QueueUserWorkItem(delegate
					{
						((DispatcherObject)item.Value).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							((Window)item.Value).Topmost = false;
						}, new object[0]);
					});
				}
			}, new object[0]);
			x += windowWidth - overlapWidth;
			num = Math.Min(num, windowHeight);
			num2++;
			if (num2 % columns == 0L)
			{
				y += Math.Max(num - overlapHeight, 0.0);
				x = 0.0;
			}
		}
	}

	internal static void CalculateOverlappingLength(double windowWidth, double windowHeight, long rows, long columns, out double overlapWidth, out double overlapHeight)
	{
		overlapHeight = 0.0;
		overlapWidth = 0.0;
		if (windowWidth * (double)columns > (double)Screen.PrimaryScreen.WorkingArea.Width)
		{
			double num = windowWidth * (double)columns - (double)Screen.PrimaryScreen.WorkingArea.Width;
			overlapWidth = num / (double)(columns - 1);
		}
		if (windowHeight * (double)rows > (double)Screen.PrimaryScreen.WorkingArea.Height)
		{
			double num2 = windowHeight * (double)rows - (double)Screen.PrimaryScreen.WorkingArea.Height;
			overlapHeight = Math.Max(overlapHeight, num2 / (double)(rows - 1));
		}
	}

	internal static void ArrangeWindowInCascade()
	{
		double num = Screen.PrimaryScreen.WorkingArea.Top;
		double num2 = Screen.PrimaryScreen.WorkingArea.Bottom;
		double num3 = Screen.PrimaryScreen.WorkingArea.Left;
		double num4 = Screen.PrimaryScreen.WorkingArea.Right;
		double num5 = Screen.PrimaryScreen.WorkingArea.Width;
		double num6 = Screen.PrimaryScreen.WorkingArea.Height;
		double windowWidth = (int)(num5 / 3.0);
		double windowHeight = (int)(num6 / 3.0);
		double y = num;
		double x = num3;
		foreach (KeyValuePair<string, MainWindow> item in BlueStacksUIUtils.DictWindows)
		{
			_ = item.Value.Handle;
			((DispatcherObject)item.Value).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Invalid comparison between Unknown and I4
				if ((int)((Window)item.Value).WindowState == 1)
				{
					item.Value.RestoreWindows();
				}
				KMManager.CloseWindows();
				windowHeight = item.Value.GetHeightFromWidth(windowWidth);
				item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
				((UIElement)item.Value).Focus();
			}, new object[0]);
			x += 40.0;
			y += 40.0;
			if (y >= num2 || x >= num4)
			{
				y = num + 40.0;
				x = num3 + 40.0;
			}
		}
	}

	public void SetNcSoftStreamingStatus(string status)
	{
		if (status.Equals("on", StringComparison.InvariantCultureIgnoreCase))
		{
			SidebarElement elementFromTag = ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
			ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video_active");
			((FrameworkElement)elementFromTag.Image).Width = 44.0;
			((FrameworkElement)elementFromTag.Image).Height = 44.0;
			ParentWindow.mNCTopBar.ChangeTopBarColor("StreamingTopBarColor");
			((UIElement)ParentWindow.mNCTopBar.mStreamingTopbarGrid).Visibility = (Visibility)0;
			ParentWindow.mIsStreaming = true;
		}
		else
		{
			SidebarElement elementFromTag2 = ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
			ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video");
			((FrameworkElement)elementFromTag2.Image).Width = 24.0;
			((FrameworkElement)elementFromTag2.Image).Height = 24.0;
			ParentWindow.mNCTopBar.ChangeTopBarColor("TopBarColor");
			((UIElement)ParentWindow.mNCTopBar.mStreamingTopbarGrid).Visibility = (Visibility)2;
			ParentWindow.mIsStreaming = false;
		}
	}

	internal static void ArrangeWindow()
	{
		if (RegistryManager.Instance.ArrangeWindowMode == 0)
		{
			ArrangeWindowInTiles();
		}
		else
		{
			ArrangeWindowInCascade();
		}
	}

	internal void MuteUnmuteButtonHanlder()
	{
		if (ParentWindow.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted)
		{
			ParentWindow.Utils.UnmuteApplication(allInstances: false);
		}
		else
		{
			ParentWindow.Utils.MuteApplication(allInstances: false);
		}
	}

	internal static string GetMacroName(string baseSchemeName = "Macro")
	{
		_ = baseSchemeName.Length;
		int num = 1;
		while ((from MacroRecording macro in MacroGraph.Instance.Vertices
			select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[2]
		{
			baseSchemeName.ToLower(CultureInfo.InvariantCulture),
			num
		}).Trim()))
		{
			num++;
		}
		return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[2] { baseSchemeName, num });
	}

	internal void MouseMoveOverFrontend()
	{
		if (KMManager.sIsInScriptEditingMode && !ParentWindow.mIsWindowInFocus)
		{
			Logger.Info("Script focused");
			ParentWindow.mFrontendHandler.FocusFrontend();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (mObsResponseTimeoutTimer != null)
			{
				mObsResponseTimeoutTimer.Elapsed -= ObsResponseTimeoutTimer_Elapsed;
				mObsResponseTimeoutTimer.Dispose();
			}
			disposedValue = true;
		}
	}

	~CommonHandlers()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal static void ReloadMacroShortcutsForAllInstances()
	{
		foreach (string runningInstances in Utils.GetRunningInstancesList())
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(runningInstances))
			{
				HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", MainWindow.sMacroMapping, runningInstances, 0, (Dictionary<string, string>)null, false, 1, 0);
			}
		}
	}

	internal void GameGuideButtonHandler(string action, string location)
	{
		if (!ToggleGamepadAndKeyboardGuidance("default"))
		{
			KMManager.HandleInputMapperWindow(ParentWindow);
			_ = ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
		}
		ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "GameGuide", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	internal static string GetCompleteMacroRecordingPath(string macroName)
	{
		return Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture) + ".json");
	}

	internal bool ToggleGamepadAndKeyboardGuidance(string selectedTab)
	{
		return false;
	}

	internal void ToggleScrollOnEdgeMode(string enable)
	{
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("toggleScrollOnEdgeFeature", new Dictionary<string, string> { { "isEnabled", enable } });
	}

	internal bool CheckNativeGamepadState(string packageName)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "packageName", packageName } };
			string text = ((object)JObject.Parse(HTTPUtils.SendRequestToGuest("checknativegamepadstatus", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"))["isEnabled"]).ToString().Trim();
			Logger.Debug("NATIVE_GAMEPAD: isEnabled: " + text);
			if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in CheckNativeGampeadState: " + ex.ToString());
		}
		return false;
	}
}
