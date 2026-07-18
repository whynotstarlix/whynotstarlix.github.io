using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class GameSettingViewModel : ViewModelBase
{
	private static readonly string sKnowMoreBaseUrl = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
	{
		WebHelper.GetServerHost(),
		"help_articles"
	})) + "&article=";

	private readonly MainWindow mParentWindow;

	private CursorMode mPreviousCursorMode;

	private CursorMode mCursorMode;

	private string mImageName;

	private string mAppName;

	private string mPackageName;

	private CurrentGame mCurrentGame;

	private Uri mLearnMoreUri;

	private Visibility mLearnMoreVisibility = (Visibility)2;

	private OtherAppGameSetting mOtherAppGameSetting;

	private FreeFireGameSettingViewModel mFreeFireGameSettingViewModel;

	private PubgGameSettingViewModel mPubgGameSettingViewModel;

	private CallOfDutyGameSettingViewModel mCallOfDutyGameSettingViewModel;

	private Visibility mShowGuideVisibility;

	private string mCustomCursorImageName;

	private CustomToastPopupControl mToastPopup;

	public GameSettingView View { get; set; }

	public CursorMode CursorMode
	{
		get
		{
			return mCursorMode;
		}
		set
		{
			((ViewModelBase)this).SetProperty<CursorMode>(ref mCursorMode, value, (string)null);
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
			((ViewModelBase)this).SetProperty<string>(ref mImageName, value, (string)null);
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
			((ViewModelBase)this).SetProperty<string>(ref mAppName, value, (string)null);
		}
	}

	public string PackageName
	{
		get
		{
			return mPackageName;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mPackageName, value, (string)null);
		}
	}

	public CurrentGame CurrentGame
	{
		get
		{
			return mCurrentGame;
		}
		set
		{
			((ViewModelBase)this).SetProperty<CurrentGame>(ref mCurrentGame, value, (string)null);
		}
	}

	public Uri LearnMoreUri
	{
		get
		{
			return mLearnMoreUri;
		}
		set
		{
			((ViewModelBase)this).SetProperty<Uri>(ref mLearnMoreUri, value, (string)null);
		}
	}

	public Visibility LearnMoreVisibility
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mLearnMoreVisibility;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<Visibility>(ref mLearnMoreVisibility, value, (string)null);
		}
	}

	public OtherAppGameSetting OtherAppGameSetting
	{
		get
		{
			return mOtherAppGameSetting;
		}
		set
		{
			((ViewModelBase)this).SetProperty<OtherAppGameSetting>(ref mOtherAppGameSetting, value, (string)null);
		}
	}

	public FreeFireGameSettingViewModel FreeFireGameSettingViewModel
	{
		get
		{
			return mFreeFireGameSettingViewModel;
		}
		set
		{
			((ViewModelBase)this).SetProperty<FreeFireGameSettingViewModel>(ref mFreeFireGameSettingViewModel, value, (string)null);
		}
	}

	public PubgGameSettingViewModel PubgGameSettingViewModel
	{
		get
		{
			return mPubgGameSettingViewModel;
		}
		set
		{
			((ViewModelBase)this).SetProperty<PubgGameSettingViewModel>(ref mPubgGameSettingViewModel, value, (string)null);
		}
	}

	public CallOfDutyGameSettingViewModel CallOfDutyGameSettingViewModel
	{
		get
		{
			return mCallOfDutyGameSettingViewModel;
		}
		set
		{
			((ViewModelBase)this).SetProperty<CallOfDutyGameSettingViewModel>(ref mCallOfDutyGameSettingViewModel, value, (string)null);
		}
	}

	public Visibility ShowGuideVisibility
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mShowGuideVisibility;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<Visibility>(ref mShowGuideVisibility, value, (string)null);
		}
	}

	public string CustomCursorImageName
	{
		get
		{
			return mCustomCursorImageName;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mCustomCursorImageName, value, (string)null);
		}
	}

	public ICommand SaveCommand { get; set; }

	public ICommand OpenGameGuideCommand { get; set; }

	public GameSettingViewModel(MainWindow owner)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		mParentWindow = owner;
		if (owner?.StaticComponents.mSelectedTabButton != null)
		{
			ImageName = ((owner != null) ? owner.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName : null);
			AppName = owner?.StaticComponents.mSelectedTabButton.AppLabel;
			PackageName = owner?.StaticComponents.mSelectedTabButton.PackageName;
			OpenGameGuideCommand = (ICommand)new RelayCommand2((Action<object>)OpenGameGuide);
			if (string.Equals(PackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
			{
				CustomCursorImageName = "yellow_cursor_brawl";
			}
			else
			{
				CustomCursorImageName = "yellow_cursor";
			}
			if (owner != null && owner.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
			{
				if ("com.dts.freefireth".Contains(PackageName))
				{
					CurrentGame = CurrentGame.FreeFire;
				}
				else if (ThirdParty.AllPUBGPackageNames.Contains(PackageName))
				{
					CurrentGame = CurrentGame.PubG;
				}
				else if (ThirdParty.AllCallOfDutyPackageNames.Contains(PackageName))
				{
					CurrentGame = CurrentGame.CallOfDuty;
				}
				else
				{
					CurrentGame = CurrentGame.OtherApp;
				}
			}
			else
			{
				CurrentGame = CurrentGame.None;
			}
			OtherAppGameSetting = new OtherAppGameSetting(mParentWindow, AppName, PackageName);
			FreeFireGameSettingViewModel = new FreeFireGameSettingViewModel(mParentWindow, AppName, PackageName);
			PubgGameSettingViewModel = new PubgGameSettingViewModel(mParentWindow, AppName, PackageName);
			CallOfDutyGameSettingViewModel = new CallOfDutyGameSettingViewModel(mParentWindow, AppName, PackageName);
		}
		SaveCommand = (ICommand)new RelayCommand2((Func<object, bool>)CanSave, (Action<object>)SaveGameSettings);
		Init();
	}

	public void Init()
	{
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Invalid comparison between Unknown and I4
		CursorMode = (RegistryManager.Instance.CustomCursorEnabled ? CursorMode.Custom : CursorMode.Normal);
		mPreviousCursorMode = CursorMode;
		if (mParentWindow.StaticComponents.mSelectedTabButton == null)
		{
			return;
		}
		ShowGuideVisibility = (Visibility)((mParentWindow.SelectedConfig.SelectedControlScheme == null || !mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any() || !mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany((IMAction action) => action.Guidance).Any()) ? 2 : 0);
		switch (CurrentGame)
		{
		case CurrentGame.PubG:
			LearnMoreVisibility = (Visibility)0;
			LearnMoreUri = new Uri(sKnowMoreBaseUrl + "game_settings_know_more_pubg");
			PubgGameSettingViewModel.Init();
			PubgGameSettingViewModel.LockOriginal();
			OtherAppGameSetting = PubgGameSettingViewModel;
			break;
		case CurrentGame.CallOfDuty:
			LearnMoreVisibility = (Visibility)0;
			LearnMoreUri = new Uri(sKnowMoreBaseUrl + "game_settings_know_more_callofduty");
			CallOfDutyGameSettingViewModel.Init();
			CallOfDutyGameSettingViewModel.LockOriginal();
			OtherAppGameSetting = CallOfDutyGameSettingViewModel;
			break;
		case CurrentGame.FreeFire:
			LearnMoreVisibility = (Visibility)0;
			LearnMoreUri = new Uri(sKnowMoreBaseUrl + "game_settings_know_more_freefire");
			FreeFireGameSettingViewModel.Init();
			FreeFireGameSettingViewModel.LockOriginal();
			OtherAppGameSetting = FreeFireGameSettingViewModel;
			break;
		case CurrentGame.OtherApp:
			OtherAppGameSetting.Init();
			OtherAppGameSetting.LockOriginal();
			if ((int)OtherAppGameSetting.PlayInLandscapeModeVisibility == 0 || (int)OtherAppGameSetting.PlayInPortraitModeVisibility == 0)
			{
				LearnMoreVisibility = (Visibility)0;
				LearnMoreUri = new Uri(sKnowMoreBaseUrl + "game_settings_know_more_sevendeadly");
			}
			else if ((int)ShowGuideVisibility == 2)
			{
				CurrentGame = CurrentGame.None;
				LearnMoreVisibility = (Visibility)2;
			}
			break;
		default:
			LearnMoreVisibility = (Visibility)2;
			break;
		}
	}

	private void OpenGameGuide(object obj)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (KMManager.sGuidanceWindow != null && (int)((UIElement)KMManager.sGuidanceWindow).Visibility == 0)
		{
			KMManager.sGuidanceWindow.Highlight();
		}
		else if (IsDirty())
		{
			CustomMessageWindow val = new CustomMessageWindow
			{
				Owner = (Window)(object)mParentWindow,
				WindowStartupLocation = (WindowStartupLocation)2
			};
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_SETTING_CLOSE_MESSAGE", "");
			val.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
			{
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				OpenGameGuide();
			}, (string)null, false, (object)null);
			((Window)val).ShowDialog();
		}
		else
		{
			OpenGameGuide();
		}
	}

	private void OpenGameGuide()
	{
	}

	private bool CanSave(object obj)
	{
		return IsDirty();
	}

	public bool IsDirty()
	{
		bool flag = false;
		switch (CurrentGame)
		{
		case CurrentGame.PubG:
			flag = PubgGameSettingViewModel.HasChanged();
			break;
		case CurrentGame.CallOfDuty:
			flag = CallOfDutyGameSettingViewModel.HasChanged();
			break;
		case CurrentGame.FreeFire:
			flag = FreeFireGameSettingViewModel.HasChanged();
			break;
		case CurrentGame.OtherApp:
			flag = OtherAppGameSetting.HasChanged();
			break;
		}
		if (!flag)
		{
			return mPreviousCursorMode != CursorMode;
		}
		return true;
	}

	private void SaveGameSettings(object obj)
	{
		bool flag = false;
		bool flag2 = false;
		switch (CurrentGame)
		{
		case CurrentGame.PubG:
			flag = PubgGameSettingViewModel.Save(flag);
			PubgGameSettingViewModel.LockOriginal();
			flag2 = true;
			break;
		case CurrentGame.CallOfDuty:
			flag = CallOfDutyGameSettingViewModel.Save(flag);
			CallOfDutyGameSettingViewModel.LockOriginal();
			flag2 = true;
			break;
		case CurrentGame.FreeFire:
			flag = FreeFireGameSettingViewModel.Save(flag);
			FreeFireGameSettingViewModel.LockOriginal();
			flag2 = true;
			break;
		case CurrentGame.OtherApp:
			flag = OtherAppGameSetting.Save(flag);
			OtherAppGameSetting.LockOriginal();
			flag2 = true;
			break;
		}
		if (flag2)
		{
			ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, AppName, "Save", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		if (mPreviousCursorMode != CursorMode)
		{
			RegistryManager.Instance.CustomCursorEnabled = CursorMode == CursorMode.Custom;
			foreach (MainWindow value in BlueStacksUIUtils.DictWindows.Values)
			{
				value.mCommonHandler.SetCustomCursorForApp(PackageName);
			}
			mPreviousCursorMode = CursorMode;
			ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "Is Custom Cursor", (CursorMode == CursorMode.Custom).ToString(CultureInfo.InvariantCulture), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
		if (flag)
		{
			RestartApp(mParentWindow, AppName);
		}
	}

	public static void SendGameSettingsEnabledToGuest(MainWindow parentWindow, bool enabled)
	{
		string text = "{";
		text += string.Format(CultureInfo.InvariantCulture, "\"package_name\":\"{0}\",", new object[1] { "com.dts.freefireth" });
		text += string.Format(CultureInfo.InvariantCulture, "\"game_settings_enabled\":\"{0}\"", new object[1] { enabled.ToString(CultureInfo.InvariantCulture) });
		text += "}";
		VmCmdHandler.RunCommandAsync(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2] { "gameSettingsEnabled", text }), (Action)null, (Control)null, parentWindow?.mVmName);
	}

	public static void SendGameSettingsStat(string statsTag)
	{
		ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, statsTag, string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	protected void AddToastPopup(string message)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		try
		{
			if (mToastPopup == null)
			{
				mToastPopup = new CustomToastPopupControl((UserControl)(object)View);
			}
			mToastPopup.Init((UserControl)(object)View, message, (Brush)null, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)2, (Thickness?)null, 12, (Thickness?)null, (Brush)null);
			mToastPopup.ShowPopup(1.3);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in showing toast popup: " + ex.ToString());
		}
	}

	public static void RestartApp(MainWindow parentWindow, string appName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		CustomMessageWindow val = new CustomMessageWindow
		{
			Owner = (Window)(object)parentWindow
		};
		string text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART", ""), new object[1] { appName });
		BlueStacksUIBinding.Bind(val.TitleTextBlock, text, "");
		string text2 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), new object[1] { appName });
		BlueStacksUIBinding.Bind(val.BodyTextBlock, text2, "");
		val.AddButton((ButtonColors)4, "STRING_RESTART_NOW", (EventHandler)delegate
		{
			if (MainWindow.SettingsWindow.ParentWindow == parentWindow)
			{
				BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)MainWindow.SettingsWindow);
			}
			Thread obj = new Thread((ThreadStart)delegate
			{
				parentWindow.mTopBar.mAppTabButtons.RestartTab(parentWindow.StaticComponents.mSelectedTabButton.PackageName);
			})
			{
				IsBackground = true
			};
			Logger.Info("Restarting Game Tab.");
			obj.Start();
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
		((Window)val).ShowDialog();
	}

	public void Reset()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)OtherAppGameSetting.ShowSensitivity == 0)
		{
			OtherAppGameSetting.Reset();
		}
	}
}
