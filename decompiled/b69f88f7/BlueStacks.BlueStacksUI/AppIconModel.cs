using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AppIconModel : INotifyPropertyChanged
{
	private AppInfo mAppInfoItem;

	private Visibility mAppIconVisibility;

	private string mImageName = string.Empty;

	private string mAppName = string.Empty;

	private string mAppNameTooltip;

	private TextTrimming mAppNameTextTrimming;

	private TextWrapping mAppNameTextWrapping = (TextWrapping)1;

	private bool mIsGamepadCompatible;

	private bool mIsGamepadConnected;

	private bool mIsRedDotVisible;

	private AppIncompatType mAppIncompatType;

	private string mApplyImageBorder = string.Empty;

	public string PackageName { get; set; } = string.Empty;

	public string ActivityName { get; set; } = string.Empty;

	public Visibility AppIconVisibility
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAppIconVisibility;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mAppIconVisibility = value;
			OnPropertyChanged("AppIconVisibility");
		}
	}

	public string ImageName
	{
		get
		{
			return mImageName;
		}
		set
		{
			mImageName = value;
			OnPropertyChanged("ImageName");
		}
	}

	public string AppName
	{
		get
		{
			return mAppName;
		}
		set
		{
			mAppName = value;
			OnPropertyChanged("AppName");
			AppNameTooltip = value;
		}
	}

	public string AppNameTooltip
	{
		get
		{
			return mAppNameTooltip;
		}
		set
		{
			mAppNameTooltip = value;
			OnPropertyChanged("AppNameTooltip");
		}
	}

	public TextTrimming AppNameTextTrimming
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAppNameTextTrimming;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mAppNameTextTrimming = value;
			OnPropertyChanged("AppNameTextTrimming");
		}
	}

	public TextWrapping AppNameTextWrapping
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAppNameTextWrapping;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mAppNameTextWrapping = value;
			OnPropertyChanged("AppNameTextWrapping");
		}
	}

	public bool IsGamepadCompatible
	{
		get
		{
			return mIsGamepadCompatible;
		}
		set
		{
			mIsGamepadCompatible = value;
			OnPropertyChanged("IsGamepadCompatible");
		}
	}

	public bool IsGamepadConnected
	{
		get
		{
			return mIsGamepadConnected;
		}
		set
		{
			mIsGamepadConnected = value;
			OnPropertyChanged("IsGamepadConnected");
		}
	}

	public bool IsRedDotVisible
	{
		get
		{
			return mIsRedDotVisible;
		}
		set
		{
			mIsRedDotVisible = value;
			OnPropertyChanged("IsRedDotVisible");
		}
	}

	public string ApkUrl { get; set; } = string.Empty;

	public bool IsGifIcon { get; set; }

	public bool IsAppSuggestionActive { get; set; }

	public bool mIsAppRemovable { get; set; } = true;

	public bool IsGl3App { get; set; }

	public AppIncompatType AppIncompatType
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAppIncompatType;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mAppIncompatType = value;
			OnPropertyChanged("AppIncompatType");
		}
	}

	public bool IsDownloading { get; set; }

	public double DownloadPercentage { get; set; }

	public bool IsInstalling { get; set; }

	public bool IsDownLoadingFailed { get; set; }

	public bool IsInstallingFailed { get; set; }

	public bool IsInstalledApp { get; set; } = true;

	public int MyAppPriority { get; set; } = 999;

	public bool IsRerollIcon { get; set; }

	public string ApkFilePath { get; set; } = string.Empty;

	public bool mIsAppInstalled { get; set; } = true;

	public AppIconLocation IconLocation { get; set; }

	public double IconHeight { get; set; } = 60.0;

	public double IconWidth { get; set; } = 60.0;

	public string ApplyImageBorder
	{
		get
		{
			return mApplyImageBorder;
		}
		set
		{
			mApplyImageBorder = value;
			OnPropertyChanged("ApplyImageBorder");
		}
	}

	public string mPromotionId { get; private set; }

	public AppSuggestionPromotion AppSuggestionInfo { get; private set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public event Action<AppIconDownloadingPhases> mAppDownloadingEvent;

	protected void OnPropertyChanged(string property)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
	}

	private void Init(string package, string appName)
	{
		if (AppHandler.ListIgnoredApps.Contains<string>(package, StringComparer.InvariantCultureIgnoreCase) || string.Equals(PackageName, "macro_recorder", StringComparison.InvariantCulture))
		{
			AppIconVisibility = (Visibility)2;
		}
		PackageName = package;
		AppName = appName;
		if (RegistryManager.Instance.IsShowIconBorder)
		{
			ApplyBorder("appFrameIcon");
		}
	}

	internal void InitRerollIcon(string package, string appname)
	{
		Init(package, appname);
		IsRerollIcon = true;
	}

	internal void Init(string package, string appName, string apkUrl)
	{
		Init(package, appName);
		ApkUrl = apkUrl;
	}

	internal void Init(AppInfo item)
	{
		mAppInfoItem = item;
		Init(item.Package, item.Name);
		LoadDownloadAppIcon();
		ActivityName = item.Activity;
		if (item.Gl3Required)
		{
			IsGl3App = true;
		}
		IsGamepadCompatible = item.IsGamepadCompatible;
		if (IsGamepadCompatible)
		{
			AppNameTextWrapping = (TextWrapping)1;
		}
	}

	internal void Init(AppSuggestionPromotion appSuggestionInfo)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		AppSuggestionInfo = appSuggestionInfo;
		AppName = appSuggestionInfo.AppName;
		ActivityName = appSuggestionInfo.AppActivity;
		IsAppSuggestionActive = true;
		AppNameTooltip = (string.IsNullOrEmpty(AppSuggestionInfo.ToolTip) ? AppName : null);
		AppNameTextWrapping = (TextWrapping)2;
		AppNameTextTrimming = (TextTrimming)1;
		if (((Dictionary<string, string>)(object)AppSuggestionInfo.ExtraPayload).ContainsKey("click_generic_action") && (EnumHelper.Parse<GenericAction>(((Dictionary<string, string>)(object)AppSuggestionInfo.ExtraPayload)["click_generic_action"], (GenericAction)65536) & 0x1C0) != 0)
		{
			mIsAppRemovable = false;
			AppNameTextWrapping = (TextWrapping)1;
			AppNameTextTrimming = (TextTrimming)0;
		}
		mIsAppInstalled = false;
		mPromotionId = AppSuggestionInfo.AppIconId;
		if (AppSuggestionInfo.AppIcon.EndsWith(".gif", StringComparison.InvariantCulture))
		{
			IsGifIcon = true;
		}
		ImageName = AppSuggestionInfo.AppIconPath;
		if (AppSuggestionInfo.IsIconBorder)
		{
			ApplyBorder(Path.Combine(RegistryStrings.PromotionDirectory, string.Format(CultureInfo.InvariantCulture, "{0}{1}.png", new object[2] { AppSuggestionInfo.IconBorderId, "app_suggestion_icon_border" })));
		}
	}

	private void LoadDownloadAppIcon()
	{
		if (string.IsNullOrEmpty(PackageName) || IsAppSuggestionActive)
		{
			return;
		}
		string path = Regex.Replace(PackageName + ".png", "[\\x22\\\\\\/:*?|<>]", " ");
		string text = Path.Combine(RegistryStrings.GadgetDir, path);
		if (!AppHandler.ListIgnoredApps.Contains<string>(PackageName, StringComparer.InvariantCultureIgnoreCase))
		{
			AppInfo obj = mAppInfoItem;
			if (!string.IsNullOrEmpty((obj != null) ? obj.Img : null) && !File.Exists(text) && File.Exists(Path.Combine(RegistryStrings.GadgetDir, mAppInfoItem.Img)))
			{
				File.Copy(Path.Combine(RegistryStrings.GadgetDir, mAppInfoItem.Img), text, overwrite: false);
			}
		}
		if (File.Exists(text))
		{
			ImageName = text;
		}
	}

	internal void AddRedDot()
	{
		IsRedDotVisible = true;
	}

	internal void AddToDock(double height = 50.0, double width = 50.0)
	{
		IconLocation = (AppIconLocation)1;
		IconHeight = height;
		IconWidth = width;
		mIsAppRemovable = false;
		if (((Dictionary<string, int>)(object)PromotionObject.Instance.DockOrder).ContainsKey(PackageName) && MyAppPriority != ((Dictionary<string, int>)(object)PromotionObject.Instance.DockOrder)[PackageName])
		{
			MyAppPriority = ((Dictionary<string, int>)(object)PromotionObject.Instance.DockOrder)[PackageName];
		}
	}

	internal void AddToMoreAppsDock(double height = 55.0, double width = 55.0)
	{
		IconLocation = (AppIconLocation)2;
		IconHeight = height;
		IconWidth = width;
		mIsAppRemovable = false;
		if (((Dictionary<string, int>)(object)PromotionObject.Instance.MoreAppsDockOrder).ContainsKey(PackageName) && MyAppPriority != ((Dictionary<string, int>)(object)PromotionObject.Instance.MoreAppsDockOrder)[PackageName])
		{
			MyAppPriority = ((Dictionary<string, int>)(object)PromotionObject.Instance.MoreAppsDockOrder)[PackageName];
		}
	}

	internal void AddToInstallDrawer()
	{
		if (string.Compare(PackageName, "com.android.vending", StringComparison.OrdinalIgnoreCase) == 0)
		{
			MyAppPriority = 1;
		}
		if (((Dictionary<string, int>)(object)PromotionObject.Instance.MyAppsOrder).ContainsKey(PackageName) && MyAppPriority != ((Dictionary<string, int>)(object)PromotionObject.Instance.MyAppsOrder)[PackageName])
		{
			MyAppPriority = ((Dictionary<string, int>)(object)PromotionObject.Instance.MyAppsOrder)[PackageName];
		}
	}

	internal void AddPromotionBorderInstalledApp(AppSuggestionPromotion appSuggestionInfo)
	{
		AppSuggestionInfo = appSuggestionInfo;
		if (AppSuggestionInfo.IsIconBorder)
		{
			ApplyBorder(AppSuggestionInfo.IconBorderId + "app_suggestion_icon_border");
		}
	}

	internal void RemovePromotionBorderInstalledApp()
	{
		IsAppSuggestionActive = false;
		ApplyBorder("");
	}

	private void ApplyBorder(string path)
	{
		if (mPromotionId == null)
		{
			ApplyImageBorder = path;
		}
	}

	internal void DownloadStarted()
	{
		mIsAppInstalled = false;
		IsDownLoadingFailed = false;
		IsDownloading = true;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)0);
	}

	internal void UpdateAppDownloadProgress(int percent)
	{
		DownloadPercentage = percent;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)2);
	}

	internal void DownloadFailed()
	{
		IsDownLoadingFailed = true;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)1);
	}

	internal void DownloadCompleted(string filePath)
	{
		IsDownloading = false;
		IsInstalling = true;
		ApkFilePath = filePath;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)3);
	}

	internal void ApkInstallStart(string filePath)
	{
		mIsAppInstalled = false;
		IsInstalling = true;
		IsInstallingFailed = false;
		ApkFilePath = filePath;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)4);
	}

	internal void ApkInstallFailed()
	{
		if (!mIsAppInstalled)
		{
			IsInstallingFailed = true;
		}
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)5);
	}

	internal void ApkInstallCompleted()
	{
		mIsAppInstalled = true;
		IsInstalling = false;
		this.mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)6);
	}
}
