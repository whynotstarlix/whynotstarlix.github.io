using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AppIconModel : INotifyPropertyChanged
{
	private AppInfo mAppInfoItem;

	private Visibility mAppIconVisibility;

	private string mImageName;

	private string mAppName;

	private string mAppNameTooltip;

	private TextTrimming mAppNameTextTrimming;

	private TextWrapping mAppNameTextWrapping;

	private bool mIsGamepadCompatible;

	private bool mIsGamepadConnected;

	private bool mIsRedDotVisible;

	private AppIncompatType mAppIncompatType;

	[CompilerGenerated]
	private AppIconLocation _003CIconLocation_003Ek__BackingField;

	private string mApplyImageBorder;

	public string PackageName { get; set; }

	public string ActivityName { get; set; }

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

	public string ApkUrl { get; set; }

	public bool IsGifIcon { get; set; }

	public bool IsAppSuggestionActive { get; set; }

	public bool mIsAppRemovable { get; set; }

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

	public bool IsInstalledApp { get; set; }

	public int MyAppPriority { get; set; }

	public bool IsRerollIcon { get; set; }

	public string ApkFilePath { get; set; }

	public bool mIsAppInstalled { get; set; }

	public AppIconLocation IconLocation
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CIconLocation_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CIconLocation_003Ek__BackingField = value;
		}
	}

	public double IconHeight { get; set; }

	public double IconWidth { get; set; }

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

	public AppIconModel()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		PackageName = string.Empty;
		ActivityName = string.Empty;
		mImageName = string.Empty;
		mAppName = string.Empty;
		mAppNameTextWrapping = (TextWrapping)1;
		ApkUrl = string.Empty;
		mIsAppRemovable = true;
		IsInstalledApp = true;
		MyAppPriority = 999;
		ApkFilePath = string.Empty;
		mIsAppInstalled = true;
		IconHeight = 60.0;
		IconWidth = 60.0;
		mApplyImageBorder = string.Empty;
		base._002Ector();
	}

	protected void OnPropertyChanged(string property)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
	}

	private void Init(string package, string appName)
	{
		if (AppHandler.ListIgnoredApps.Contains<string>(package, StringComparer.InvariantCultureIgnoreCase) || string.Equals(PackageName, "macro_recorder", StringComparison.InvariantCulture))
		{
			AppIconVisibility = (Visibility)(-3206359 + (1641656986 >> 589011657));
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
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		AppSuggestionInfo = appSuggestionInfo;
		AppName = appSuggestionInfo.AppName;
		ActivityName = appSuggestionInfo.AppActivity;
		IsAppSuggestionActive = true;
		AppNameTooltip = (string.IsNullOrEmpty(AppSuggestionInfo.ToolTip) ? AppName : null);
		AppNameTextWrapping = (TextWrapping)(0x20C ^ 0x20E);
		AppNameTextTrimming = (TextTrimming)1;
		if (((Dictionary<string, string>)(object)AppSuggestionInfo.ExtraPayload).ContainsKey("click_generic_action"))
		{
			GenericAction val = EnumHelper.Parse<GenericAction>(((Dictionary<string, string>)(object)AppSuggestionInfo.ExtraPayload)["click_generic_action"], (GenericAction)(1845254890 - (0x64D95828 | 0x69AB4AC2)));
			int num = ((301579931 > 2014250256) ? 597 : 448);
			if ((val & num) != 0)
			{
				mIsAppRemovable = false;
				AppNameTextWrapping = (TextWrapping)1;
				AppNameTextTrimming = (TextTrimming)0;
			}
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
			string promotionDirectory = RegistryStrings.PromotionDirectory;
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] array = new object[0x139473B9 ^ 0x139473BB];
			array[0] = AppSuggestionInfo.IconBorderId;
			array[1] = "app_suggestion_icon_border";
			ApplyBorder(Path.Combine(promotionDirectory, string.Format(invariantCulture, "{0}{1}.png", array)));
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
		IconLocation = (AppIconLocation)(-1438892030 + (245454607 << 2064184270));
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
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)0);
	}

	internal void UpdateAppDownloadProgress(int percent)
	{
		DownloadPercentage = percent;
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)(-1186546323 ^ -1186546321));
	}

	internal void DownloadFailed()
	{
		IsDownLoadingFailed = true;
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)1);
	}

	internal void DownloadCompleted(string filePath)
	{
		IsDownloading = false;
		IsInstalling = true;
		ApkFilePath = filePath;
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)(-1720641787 + (0x240C208E | 0x6682C87C)));
	}

	internal void ApkInstallStart(string filePath)
	{
		mIsAppInstalled = false;
		IsInstalling = true;
		IsInstallingFailed = false;
		ApkFilePath = filePath;
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)(2004832227 - (0x77274ECB | 0x61584B9E)));
	}

	internal void ApkInstallFailed()
	{
		if (!mIsAppInstalled)
		{
			IsInstallingFailed = true;
		}
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)(-314182292 ^ -314182295));
	}

	internal void ApkInstallCompleted()
	{
		mIsAppInstalled = true;
		IsInstalling = false;
		mAppDownloadingEvent?.Invoke((AppIconDownloadingPhases)(-121 + (2133064028 >> 1318625496)));
	}
}
