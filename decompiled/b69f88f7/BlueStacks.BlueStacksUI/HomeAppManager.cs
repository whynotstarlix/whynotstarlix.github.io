using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class HomeAppManager
{
	private Dictionary<string, AppIconModel> dictAppIcons = new Dictionary<string, AppIconModel>();

	private HomeApp mHomeApp;

	private MainWindow mParentWindow;

	internal static string BackgroundImagePath;

	public HomeAppManager(HomeApp homeApp, MainWindow parentWindow)
	{
		mHomeApp = homeApp;
		mParentWindow = parentWindow;
		InitSystemIcons();
		InitIcons();
	}

	internal void InitAppPromotionEvents()
	{
		if (PromotionObject.Instance != null)
		{
			PromotionObject.AppSuggestionHandler = (Action<bool>)Delegate.Combine(PromotionObject.AppSuggestionHandler, new Action<bool>(HomeApp_AppSuggestionHandler));
			mHomeApp?.InitUIAppPromotionEvents();
		}
	}

	private void HomeApp_AppSuggestionHandler(bool checkForAnimationIcon)
	{
		MainWindow mainWindow = mParentWindow;
		if (mainWindow == null)
		{
			return;
		}
		((DispatcherObject)mainWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				RemoveIconIfExists();
				lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
				{
					foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
					{
						if (!new JsonParser(mParentWindow.mVmName).IsAppInstalled(appSuggestion.AppPackage))
						{
							if (CheckIfPresentInRedDotShownRegistry(appSuggestion.AppPackage))
							{
								appSuggestion.IsShowRedDot = false;
							}
							AddAppSuggestionIcon(appSuggestion);
						}
						else
						{
							if (!CheckIfPresentInRedDotShownRegistry(appSuggestion.AppPackage) && appSuggestion.IsShowRedDot)
							{
								GetAppIcon(appSuggestion.AppPackage)?.AddRedDot();
							}
							else
							{
								appSuggestion.IsShowRedDot = false;
							}
							GetAppIcon(appSuggestion.AppPackage)?.AddPromotionBorderInstalledApp(appSuggestion);
						}
					}
				}
				bool flag = dictAppIcons.Keys.Intersect(ThirdParty.AllOneStorePackageNames).Any();
				foreach (string allOneStorePackageName in ThirdParty.AllOneStorePackageNames)
				{
					Utils.EnableDisableApp(allOneStorePackageName, flag, mParentWindow.mVmName);
				}
				mParentWindow.StaticComponents.PlayPauseGifs(isPlay: true);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in HomeApp_AppSuggestionHandler", new object[1] { ex });
			}
		}, new object[0]);
	}

	private void RemoveIconIfExists()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		List<string> list = new List<string>();
		JsonParser val = new JsonParser(mParentWindow.mVmName);
		foreach (AppIconModel icon in dictAppIcons.Values)
		{
			lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
			{
				if ((icon.IsAppSuggestionActive || (!icon.IsInstalledApp && PromotionObject.Instance.AppSuggestionList.Any((AppSuggestionPromotion _) => string.Equals(_.AppLocation, "more_apps", StringComparison.InvariantCulture)))) && !PromotionObject.Instance.AppSuggestionList.Any((AppSuggestionPromotion _) => string.Equals(_.AppPackage, icon.PackageName, StringComparison.InvariantCultureIgnoreCase)))
				{
					if (!val.IsAppInstalled(icon.PackageName))
					{
						list.Add(icon.PackageName);
					}
					else
					{
						icon.RemovePromotionBorderInstalledApp();
					}
				}
			}
		}
		foreach (string item in list)
		{
			RemoveAppIcon(item);
		}
	}

	private void InitSystemIcons()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		List<AppInfo> list = new JsonParser(string.Empty).GetAppList().ToList();
		mHomeApp?.InitMoreAppsIcon();
		foreach (AppInfo item in list)
		{
			if (string.Compare(item.Package, "com.android.vending", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item.Package, "com.google.android.play.games", StringComparison.OrdinalIgnoreCase) != 0)
			{
				AppIconModel newIconForKey = GetNewIconForKey(item.Package);
				newIconForKey.Init(item);
				newIconForKey.AppName = "Google Play";
				newIconForKey.IsInstalledApp = false;
				newIconForKey.AddToMoreAppsDock();
				mHomeApp?.AddMoreAppsDockPanelIcon(newIconForKey);
			}
			else
			{
				AppIconModel newIconForKey2 = GetNewIconForKey(item.Package);
				newIconForKey2.Init(item);
				newIconForKey2.IsInstalledApp = false;
				newIconForKey2.mIsAppRemovable = false;
				newIconForKey2.AppName = "Google Play";
				newIconForKey2.AddToInstallDrawer();
				mHomeApp?.AddInstallDrawerIcon(newIconForKey2);
			}
		}
	}

	internal void InitIcons()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		foreach (AppInfo item in new JsonParser(mParentWindow.mVmName).GetAppList().ToList())
		{
			AddIcon(item);
		}
	}

	internal AppInfo AddAppIcon(string package)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		AppInfo appInfoFromPackageName = new JsonParser(mParentWindow.mVmName).GetAppInfoFromPackageName(package);
		if (appInfoFromPackageName != null)
		{
			AddIcon(appInfoFromPackageName);
		}
		return appInfoFromPackageName;
	}

	private void AddAppSuggestionIcon(AppSuggestionPromotion appSuggestionInfo)
	{
		string appPackage = appSuggestionInfo.AppPackage;
		double height = 50.0;
		double width = 50.0;
		AppIconModel newIconForKey = GetNewIconForKey(appPackage);
		try
		{
			if (newIconForKey == null)
			{
				return;
			}
			newIconForKey.IsAppSuggestionActive = true;
			newIconForKey.PackageName = appPackage;
			if (appSuggestionInfo.IsShowRedDot)
			{
				newIconForKey.IsRedDotVisible = true;
			}
			newIconForKey.Init(appSuggestionInfo);
			if (appSuggestionInfo.IsEmailRequired && !RegistryManager.Instance.Guest[mParentWindow.mVmName].IsGoogleSigninDone)
			{
				return;
			}
			if (string.Equals(appSuggestionInfo.AppLocation, "dock", StringComparison.InvariantCultureIgnoreCase))
			{
				if (appSuggestionInfo.IconHeight != 0.0)
				{
					height = appSuggestionInfo.IconHeight;
				}
				if (appSuggestionInfo.IconWidth != 0.0)
				{
					width = appSuggestionInfo.IconWidth;
				}
				newIconForKey.AddToDock(height, width);
				mHomeApp?.AddDockPanelIcon(newIconForKey);
			}
			else if (string.Equals(appSuggestionInfo.AppLocation, "more_apps", StringComparison.InvariantCultureIgnoreCase))
			{
				newIconForKey.AddToMoreAppsDock();
				mHomeApp?.AddMoreAppsDockPanelIcon(newIconForKey);
			}
			else
			{
				newIconForKey.AddToInstallDrawer();
				mHomeApp?.AddInstallDrawerIcon(newIconForKey);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in adding app suggestion icon: " + ex.ToString());
		}
	}

	internal void AddIconWithRedDot(string appPackage)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
		{
			JsonParser val = new JsonParser(mParentWindow.mVmName);
			foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
			{
				if (string.Equals(appSuggestion.AppPackage, appPackage, StringComparison.InvariantCulture))
				{
					if (!val.IsAppInstalled(appSuggestion.AppPackage))
					{
						RemovePackageInRedDotShownRegistry(appSuggestion.AppPackage);
						AddAppSuggestionIcon(appSuggestion);
					}
					else
					{
						RemovePackageInRedDotShownRegistry(appSuggestion.AppPackage);
						GetAppIcon(appPackage)?.AddRedDot();
					}
				}
			}
		}
	}

	internal void AddMacroAppIcon(string package)
	{
		if (!string.IsNullOrEmpty(package))
		{
			string key = package + "_macro";
			AppIconModel newIconForKey = GetNewIconForKey(key);
			string appname = LocaleStrings.GetLocalizedString("STRING_REROLL_APP_PREFIX", "") + " - " + newIconForKey.AppName;
			newIconForKey.InitRerollIcon(package, appname);
			newIconForKey.AddToInstallDrawer();
			mHomeApp?.AddInstallDrawerIcon(newIconForKey);
		}
	}

	internal void AddAppIcon(string package, string appName, string apkUrl, DownloadInstallApk downloader)
	{
		if (!string.IsNullOrEmpty(package))
		{
			AppIconModel newIconForKey = GetNewIconForKey(package);
			newIconForKey.Init(package, appName, apkUrl);
			newIconForKey.AddToInstallDrawer();
			mHomeApp?.AddInstallDrawerIcon(newIconForKey, downloader);
		}
	}

	private void AddIcon(AppInfo item)
	{
		AppIconModel newIconForKey = GetNewIconForKey(item.Package);
		newIconForKey.Init(item);
		newIconForKey.AddToInstallDrawer();
		mHomeApp?.AddInstallDrawerIcon(newIconForKey);
	}

	private AppIconModel GetNewIconForKey(string key)
	{
		AppIconModel appIconModel = new AppIconModel();
		RemoveAppIcon(key, appIconModel);
		dictAppIcons[key] = appIconModel;
		return appIconModel;
	}

	internal bool CheckDictAppIconFor(string packagename, Predicate<AppIconModel> pred)
	{
		if (dictAppIcons.ContainsKey(packagename))
		{
			return pred(dictAppIcons[packagename]);
		}
		return false;
	}

	internal AppIconModel GetAppIcon(string packageName)
	{
		if (FeatureManager.Instance.IsCustomUIForNCSoft && packageName == BlueStacksUIUtils.sUserAccountPackageName)
		{
			Logger.Info("Setting packageName to com.android.vending when com.uncube.account is received");
			packageName = "com.android.vending";
		}
		AppIconModel result = null;
		if (dictAppIcons.ContainsKey(packageName) && !string.IsNullOrEmpty(packageName))
		{
			result = dictAppIcons[packageName];
		}
		return result;
	}

	internal AppIconModel GetMacroAppIcon(string packageName)
	{
		return GetAppIcon(packageName + "_macro");
	}

	internal void RemoveAppIcon(string package, AppIconModel newAppIconCreated = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (package != null && dictAppIcons.ContainsKey(package))
		{
			if (newAppIconCreated != null)
			{
				newAppIconCreated.AppIncompatType = dictAppIcons[package].AppIncompatType;
			}
			mHomeApp?.RemoveAppIconFromUI(dictAppIcons[package]);
			dictAppIcons.Remove(package);
		}
	}

	internal void RemoveAppAfterUninstall(string package)
	{
		GrmHandler.RemovePackageFromGrmList(package, mParentWindow.mVmName);
		RemoveAppIcon(package);
		RemoveAppIcon(package + "_macro");
		try
		{
			string path = Path.Combine(RegistryStrings.GadgetDir, Regex.Replace(package + ".png", "[\\x22\\\\\\/:*?|<>]", " "));
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Not able to delete image file : " + ex.ToString());
		}
	}

	internal void UpdateGamepadIcons(bool isGamepadConnected)
	{
		foreach (KeyValuePair<string, AppIconModel> dictAppIcon in dictAppIcons)
		{
			if (dictAppIcon.Value.IsGamepadCompatible)
			{
				dictAppIcon.Value.IsGamepadConnected = isGamepadConnected;
			}
		}
	}

	internal void OpenApp(string packageName, bool isCheckForGrm = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		AppIconModel appIcon = GetAppIcon(packageName);
		if (appIcon == null)
		{
			return;
		}
		if ((int)appIcon.AppIncompatType > 0 && isCheckForGrm)
		{
			GrmHandler.HandleCompatibility(appIcon.PackageName, mParentWindow.mVmName);
			return;
		}
		mParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: false, isLaunch: true);
		mParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = appIcon.PackageName;
		mParentWindow.mAppHandler.SendRunAppRequestAsync(appIcon.PackageName);
		if (appIcon.IsRedDotVisible)
		{
			appIcon.IsRedDotVisible = false;
			AddPackageInRedDotShownRegistry(appIcon.PackageName);
		}
		SendStats(appIcon.PackageName);
	}

	private static void SendStats(string packageName)
	{
		if (packageName == "com.android.vending")
		{
			ClientStats.SendGPlayClickStats(new Dictionary<string, string> { { "source", "bs3_myapps" } });
		}
		ClientStats.SendClientStatsAsync("init", "success", "app_activity", packageName);
	}

	private static bool CheckIfPresentInRedDotShownRegistry(string package)
	{
		string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
		if (!string.IsNullOrEmpty(redDotShownOnIcon))
		{
			char[] separator = new char[1] { ',' };
			string[] array = redDotShownOnIcon.Split(separator, StringSplitOptions.None);
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(package) && text.Equals(package, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void RemovePackageInRedDotShownRegistry(string appPackage)
	{
		string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
		char[] separator = new char[1] { ',' };
		string[] array = (from w in redDotShownOnIcon.Split(separator, StringSplitOptions.None)
			where !w.Contains(appPackage)
			select w).ToArray();
		string text = string.Empty;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + text2.ToString(CultureInfo.InvariantCulture) + ",";
			}
		}
		RegistryManager.Instance.RedDotShownOnIcon = text;
	}

	internal static void AddPackageInRedDotShownRegistry(string appPackage)
	{
		string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
		redDotShownOnIcon = (string.IsNullOrEmpty(redDotShownOnIcon) ? appPackage : (redDotShownOnIcon + "," + appPackage));
		RegistryManager.Instance.RedDotShownOnIcon = redDotShownOnIcon;
	}

	internal void DownloadStarted(string packageName)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].DownloadStarted();
			JObject val = new JObject
			{
				["PackageName"] = JToken.op_Implicit(packageName),
				["AppName"] = JToken.op_Implicit(dictAppIcons[packageName].AppName),
				["ApkUrl"] = JToken.op_Implicit(dictAppIcons[packageName].ApkUrl)
			};
			Publisher.PublishMessage((BrowserControlTags)8, mParentWindow.mVmName, val);
		}
	}

	internal void UpdateAppDownloadProgress(string packageName, int percent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].UpdateAppDownloadProgress(percent);
			JObject val = new JObject
			{
				["PackageName"] = JToken.op_Implicit(packageName),
				["DownloadPercent"] = JToken.op_Implicit(percent)
			};
			Publisher.PublishMessage((BrowserControlTags)10, mParentWindow.mVmName, val);
		}
	}

	internal void DownloadFailed(string packageName)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].DownloadFailed();
			if (FeatureManager.Instance.IsHtmlHome)
			{
				RemoveAppIcon(packageName);
			}
			Publisher.PublishMessage((BrowserControlTags)9, mParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)packageName)));
		}
	}

	internal void DownloadCompleted(string packageName, string filePath)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].DownloadCompleted(filePath);
			Publisher.PublishMessage((BrowserControlTags)11, mParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)packageName)));
		}
	}

	internal void ApkInstallStart(string packageName, string filePath)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].ApkInstallStart(filePath);
			JObject val = new JObject
			{
				["PackageName"] = JToken.op_Implicit(packageName),
				["AppName"] = JToken.op_Implicit(dictAppIcons[packageName].AppName),
				["ApkFilePath"] = JToken.op_Implicit(filePath)
			};
			Publisher.PublishMessage((BrowserControlTags)12, mParentWindow.mVmName, val);
		}
	}

	internal void ApkInstallFailed(string packageName)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].ApkInstallFailed();
			if (FeatureManager.Instance.IsHtmlHome)
			{
				RemoveAppIcon(packageName);
			}
			Publisher.PublishMessage((BrowserControlTags)13, mParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)packageName)));
		}
	}

	internal void ApkInstallCompleted(string packageName)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		if (dictAppIcons.ContainsKey(packageName))
		{
			dictAppIcons[packageName].ApkInstallCompleted();
			Publisher.PublishMessage((BrowserControlTags)14, mParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)packageName)));
		}
	}

	internal void HomeTabSwitchActions(bool isHomeTabSelected)
	{
		if (isHomeTabSelected)
		{
			HomeApp homeApp = mHomeApp;
			if (homeApp != null && ((UIElement)homeApp.mSearchTextBox).IsFocused)
			{
				SetSearchTextBoxFocus(100);
			}
			mParentWindow.mWelcomeTab.ReloadHomeTabIME();
			mParentWindow.StaticComponents.PlayPauseGifs(isPlay: true);
		}
		else
		{
			mParentWindow.StaticComponents.PlayPauseGifs(isPlay: false);
		}
	}

	internal void SetSearchTextBoxFocus(int delay)
	{
		MiscUtils.SetFocusAsync((UIElement)(object)mHomeApp?.mSearchTextBox, delay);
	}

	internal void EnableSearchTextBox(bool isEnable)
	{
		if (mHomeApp != null)
		{
			((UIElement)mHomeApp.mSearchTextBox).IsEnabled = isEnable;
		}
	}

	internal void ChangeHomeAppVisibility(Visibility visibility)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (mHomeApp != null)
		{
			((UIElement)mHomeApp).Visibility = visibility;
		}
	}

	internal void RestoreWallpaper()
	{
		mHomeApp?.RestoreWallpaperImage();
	}

	internal void ApplyWallpaper()
	{
		mHomeApp?.ApplyWallpaperImage();
	}

	internal void ClearAppRecommendationPool()
	{
		mHomeApp?.sAppRecommendationsPool.Clear();
	}

	internal void AddToAppRecommendationPool(RecommendedApps recomApp)
	{
		mHomeApp?.sAppRecommendationsPool.Add(recomApp);
	}

	internal void UpdateRecommendedAppsInstallStatus(string package)
	{
		mHomeApp?.UpdateRecommendedAppsInstallStatus(package);
	}

	internal void InitiateHtmlSidePanel()
	{
		HomeApp homeApp = mHomeApp;
		if (homeApp != null && !homeApp.SideHtmlBrowserInited)
		{
			mHomeApp?.InitiateSideHtmlBrowser();
		}
	}

	internal void DisposeHtmlSidePanel()
	{
		mHomeApp?.SideHtmlBrowser?.DisposeBrowser();
	}

	internal void ReinitHtmlSidePanel()
	{
		mHomeApp?.SideHtmlBrowser?.ReInitBrowser(BlueStacksUIUtils.GetHtmlSidePanelUrl());
	}

	internal void CloseHomeAppPopups()
	{
		if (mHomeApp != null)
		{
			((Popup)mHomeApp.mSuggestedAppPopUp).IsOpen = false;
			((Popup)mHomeApp.mMoreAppsDockPopup).IsOpen = false;
		}
	}

	internal void ChangeHomeAppLoadingGridVisibility(Visibility visibility)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (mHomeApp != null)
		{
			((UIElement)mHomeApp.mLoadingGrid).Visibility = visibility;
		}
	}

	internal double GetAppRecommendationsGridWidth()
	{
		HomeApp homeApp = mHomeApp;
		if (homeApp == null || !(((FrameworkElement)homeApp.mAppRecommendationsGrid).ActualWidth > 0.0))
		{
			return 0.0;
		}
		return ((FrameworkElement)mHomeApp.mAppRecommendationsGrid).ActualWidth;
	}

	internal void ShowDockIconTooltip(AppIconUI icon, bool isOpen)
	{
		if (mHomeApp != null)
		{
			if (isOpen)
			{
				mHomeApp.mDockIconText.Text = icon.mAppIconModel.AppName;
				((Popup)mHomeApp.mDockAppIconToolTipPopup).PlacementTarget = (UIElement)(object)icon.mAppImage;
				((Popup)mHomeApp.mDockAppIconToolTipPopup).IsOpen = true;
				((Popup)mHomeApp.mDockAppIconToolTipPopup).StaysOpen = true;
			}
			else
			{
				((Popup)mHomeApp.mDockAppIconToolTipPopup).IsOpen = false;
			}
		}
	}

	internal void CloseAppSuggestionPopup()
	{
		if (mHomeApp != null)
		{
			((Popup)mHomeApp.mSuggestedAppPopUp).IsOpen = false;
		}
	}

	internal void OpenAppSuggestionPopup(AppSuggestionPromotion appInfoForShowingPopup, UIElement appNameTextBlock, bool staysOpen = true)
	{
		if (mHomeApp != null && appInfoForShowingPopup.ToolTip != null)
		{
			((Popup)mHomeApp.mSuggestedAppPopUp).PlacementTarget = appNameTextBlock;
			((Popup)mHomeApp.mSuggestedAppPopUp).IsOpen = true;
			((Popup)mHomeApp.mSuggestedAppPopUp).StaysOpen = staysOpen;
			mHomeApp.mAppSuggestionPopUp.Text = appInfoForShowingPopup.ToolTip;
		}
	}

	static HomeAppManager()
	{
		BackgroundImagePath = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client\\Assets\\backgroundImage");
	}
}
