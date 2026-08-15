using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class TopBar : UserControl, ITopBar, IComponentConnector
{
	private MainWindow mMainWindow;

	private SortedList<int, KeyValuePair<FrameworkElement, double>> mOptionsPriorityPanel;

	internal double mMinimumExpectedTopBarWidth;

	internal PerformanceState mSnailMode;

	private DispatcherTimer mMacroRunningPopupTimer;

	private DispatcherTimer mMacroRecordingPopupTimer;

	private ulong MB_MULTIPLIER;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid WindowHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTitleIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTitleTextGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mVersionText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DockPanel mOptionsDockPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSidebarButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMaximizeButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMinimizeButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mConfigButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Ellipse mSettingsBtnNotification;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mSettingsMenuPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mPreferenceDropDownBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal PreferenceDropDownControl mPreferenceDropDownControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHelpButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mUserAccountBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNotificationGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mNotificationCentreButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mNotificationCountBadge;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mNotificationCentrePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mNotificationCaret;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mNotificationCentreDropDownBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal NotificationDrawer mNotificationDrawerControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBtvButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mWarningButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOperationsSyncGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mSyncMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPlayPauseSyncButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStopSyncButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mSyncInstancesToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mUpwardArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mLocalConfigIndicator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AppTabButtons mAppTabButtons;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mVideoRecordingStatusGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal VideoRecordingStatus mVideoRecordStatusControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMacroGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroTopBarRecordControl mMacroRecordControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroTopBarPlayControl mMacroPlayControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMacroRecorderToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroRecordingTooltip;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mUpArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMacroRunningToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid grid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder4;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroRunningTooltip;

	private bool _contentLoaded;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	string ITopBar.AppName
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	string ITopBar.CharacterName
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public event PercentageChangedEventHandler PercentChanged;

	public static Point GetMousePosition()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NativeMethods.Win32Point pt = default(NativeMethods.Win32Point);
		NativeMethods.GetCursorPos(ref pt);
		return new Point((double)pt.X, (double)pt.Y);
	}

	public TopBar()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		mOptionsPriorityPanel = new SortedList<int, KeyValuePair<FrameworkElement, double>>();
		mMinimumExpectedTopBarWidth = 320.0;
		MB_MULTIPLIER = 1048576uL;
		((UserControl)this)._002Ector();
		InitializeComponent();
		if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: false);
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: false);
		}
		else
		{
			if (!FeatureManager.Instance.IsUserAccountBtnEnabled)
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: false);
			}
			if (!FeatureManager.Instance.IsWarningBtnEnabled)
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: false);
			}
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mHelpButton, FeatureManager.Instance.IsTopbarHelpEnabled);
		}
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((UIElement)mConfigButton).Visibility = (Visibility)2;
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: false);
			((UIElement)WindowHeaderGrid).Visibility = (Visibility)2;
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: false);
			((FrameworkElement)mWarningButton).ToolTip = null;
			((UIElement)mSidebarButton).Visibility = (Visibility)2;
		}
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: false);
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: false);
		}
		if (!string.Equals(mTitleIcon.ImageName, Strings.TitleBarIconImageName, StringComparison.InvariantCulture))
		{
			mTitleIcon.ImageName = Strings.TitleBarIconImageName;
		}
		if (Strings.TitleBarProductIconWidth.HasValue)
		{
			((FrameworkElement)mTitleIcon).Width = Strings.TitleBarProductIconWidth.Value;
		}
		if (Strings.TitleBarTextMaxWidth.HasValue)
		{
			((FrameworkElement)mTitleText).MaxWidth = Strings.TitleBarTextMaxWidth.Value;
		}
		mVersionText.Text = RegistryManager.Instance.ClientVersion;
	}

	private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.EngineInstanceRegistry.IsSidebarVisible && (int)((UIElement)this).Visibility == 0 && (int)((UIElement)ParentWindow.mSidebar).Visibility != 0 && !FeatureManager.Instance.IsCustomUIForDMM)
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mCommonHandler.FlipSidebarVisibility(mSidebarButton, null);
			}, new object[0]);
		}
	}

	internal void ChangeDownloadPercent(int percent)
	{
		PercentChanged?.Invoke(this, new PercentageChangedEventArgs
		{
			Percentage = percent
		});
	}

	internal void InitializeSnailButton()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsWarningBtnEnabled)
		{
			return;
		}
		string deviceCaps = RegistryManager.Instance.DeviceCaps;
		if (string.IsNullOrEmpty(deviceCaps))
		{
			mSnailMode = (PerformanceState)0;
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: false);
			return;
		}
		JObject deviceCapsData = JObject.Parse(deviceCaps);
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			if (((object)deviceCapsData["cpu_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && ((object)deviceCapsData["bios_hvm"]).ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
			{
				if (((object)deviceCapsData["engine_enabled"]).ToString().Equals(((object)(EngineState)1/*cast due to constrained. prefix*/).ToString(), StringComparison.OrdinalIgnoreCase))
				{
					mSnailMode = (PerformanceState)1;
					TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: true);
				}
			}
			else if (((object)deviceCapsData["cpu_hvm"]).ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
			{
				mSnailMode = (PerformanceState)(1082939783 + -1082939781);
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: true);
			}
			else if (((object)deviceCapsData["cpu_hvm"]).ToString().Equals("True", (StringComparison)(1277217769 + -1277217764)) && ((object)deviceCapsData["bios_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
			{
				mSnailMode = (PerformanceState)0;
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: false);
			}
			RefreshWarningButton();
		}, new object[0]);
	}

	internal void RefreshWarningButton()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!FeatureManager.Instance.IsCustomUIForDMMSandbox && FeatureManager.Instance.IsWarningBtnEnabled)
		{
			if ((int)mSnailMode != 0)
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: true);
				AddVtxNotification();
			}
			else
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mWarningButton, isVisible: false);
			}
		}
	}

	internal void AddVtxNotification()
	{
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Expected O, but got Unknown
				bool dontOverwrite = true;
				GenericNotificationItem val = new GenericNotificationItem
				{
					CreationTime = DateTime.Now,
					IsDeferred = false,
					Priority = (NotificationPriority)0,
					ShowRibbon = false,
					Id = "VtxNotification",
					NotificationMenuImageName = "SlowPerformance.png",
					Title = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT_TITLE", ""),
					Message = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT", "")
				};
				SerializableDictionary<string, string> val2 = new SerializableDictionary<string, string>();
				((Dictionary<string, string>)(object)val2).Add("click_generic_action", "UserBrowser");
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] array = new object[-629689905 - -629689907];
				array[0] = WebHelper.GetServerHost();
				array[1] = "help_articles";
				((Dictionary<string, string>)(object)val2).Add("click_action_value", WebHelper.GetUrlWithParams(string.Format(invariantCulture, "{0}/{1}", array)) + "&article=enable_virtualization");
				SerializableDictionary<string, string> val3 = val2;
				DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)val.ExtraPayload, (Dictionary<string, string>)(object)val3);
				GenericNotificationManager.AddNewNotification(val, dontOverwrite);
				RefreshNotificationCentreButton();
			}, new object[0]);
		}
	}

	internal void AddRamNotification()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			bool dontOverwrite = true;
			GenericNotificationItem val = new GenericNotificationItem
			{
				IsDeferred = false,
				Priority = (NotificationPriority)0,
				ShowRibbon = false,
				Id = "ramNotification",
				NotificationMenuImageName = "SlowPerformance.png",
				Title = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF_TITLE", ""),
				Message = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF", "")
			};
			SerializableDictionary<string, string> val2 = new SerializableDictionary<string, string>();
			((Dictionary<string, string>)(object)val2).Add("click_generic_action", "UserBrowser");
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] array = new object[0x23BEC7DD ^ 0x23BEC7DF];
			array[0] = WebHelper.GetServerHost();
			array[1] = "help_articles";
			((Dictionary<string, string>)(object)val2).Add("click_action_value", WebHelper.GetUrlWithParams(string.Format(invariantCulture, "{0}/{1}", array)) + "&article=bs3_nougat_min_requirements");
			SerializableDictionary<string, string> val3 = val2;
			DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)val.ExtraPayload, (Dictionary<string, string>)(object)val3);
			GenericNotificationManager.AddNewNotification(val, dontOverwrite);
			RefreshNotificationCentreButton();
		}, new object[0]);
	}

	private void UserAccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked account button");
		if (ParentWindow.mGuestBootCompleted && ParentWindow.mAppHandler.IsOneTimeSetupCompleted)
		{
			if (FeatureManager.Instance.IsOpenActivityFromAccountIcon)
			{
				mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sUserAccountPackageName, BlueStacksUIUtils.sUserAccountActivityName, "account_tab", isSwitch: true, isLaunch: true);
				return;
			}
			string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account");
			urlWithParams += "&email=";
			urlWithParams += RegistryManager.Instance.RegisteredEmail;
			urlWithParams += "&token=";
			urlWithParams += RegistryManager.Instance.Token;
			mAppTabButtons.AddWebTab(urlWithParams, "STRING_ACCOUNT", "account_tab", isSwitch: true, "account_tab");
		}
	}

	private void ConfigButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mPreferenceDropDownControl.LateInit();
		((Popup)mSettingsMenuPopup).IsOpen = true;
		((Popup)mSettingsMenuPopup).HorizontalOffset = 0.0 - (((FrameworkElement)mPreferenceDropDownBorder).ActualWidth - 40.0);
		mConfigButton.ImageName = "cfgmenu_hover";
	}

	private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked minimize button");
		ParentWindow.MinimizeWindow();
	}

	internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Clicked Maximize\\Restore button");
		if ((int)((Window)ParentWindow).WindowState == 0 && !ParentWindow.mIsDmmMaximised)
		{
			ParentWindow.MaximizeWindow();
		}
		else
		{
			ParentWindow.RestoreWindows();
		}
	}

	internal void SetConfigIndicator(string config)
	{
		((UIElement)mLocalConfigIndicator).Visibility = (Visibility)((!string.Equals(config, ".config_user.db", StringComparison.InvariantCultureIgnoreCase)) ? (0x6EFFE3BD ^ 0x6EFFE3BF) : 0);
	}

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked close Bluestacks button");
		Stats.SendCommonClientStatsAsync("notification_mode", "BlueStacks_close", ParentWindow.mVmName, "", "", "");
		if (RegistryManager.Instance.IsNotificationModeAlwaysOn && string.Compare("Android", ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) == 0)
		{
			if (ParentWindow.Utils.CheckQuitPopupLocal())
			{
				return;
			}
			Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", ParentWindow.mVmName, string.Empty, "on", "");
			ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
			ParentWindow.IsInNotificationMode = true;
			foreach (string key in ParentWindow.AppNotificationCountDictForEachVM.Keys)
			{
				Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", ParentWindow.mVmName, key, ParentWindow.AppNotificationCountDictForEachVM[key].ToString(CultureInfo.InvariantCulture), "NM_Off");
			}
			ParentWindow.AppNotificationCountDictForEachVM.Clear();
			ParentWindow.MinimizeWindowHandler();
		}
		else
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", ParentWindow.mVmName, string.Empty, "off", "");
			ParentWindow.CloseWindow();
		}
	}

	private void NotificationPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mConfigButton).IsEnabled = false;
	}

	private void NotificationPopup_Closed(object sender, EventArgs e)
	{
		((UIElement)mConfigButton).IsEnabled = true;
		mConfigButton.ImageName = "cfgmenu";
	}

	internal void ChangeUserPremiumButton(bool isPremium)
	{
		if (isPremium)
		{
			mUserAccountBtn.ImageName = BlueStacksUIUtils.sPremiumUserImageName;
		}
		else
		{
			mUserAccountBtn.ImageName = BlueStacksUIUtils.sLoggedInImageName;
		}
	}

	private void PreferenceDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void mWarningButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		Logger.Info("Clicked warning button for speed up Bluestacks ");
		mWarningButton.ImageName = "warning";
		SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
		if ((int)mSnailMode == (-1690726606 ^ -1690726608))
		{
			((UIElement)speedUpBlueStacks.mUpgradeComputer).Visibility = (Visibility)0;
		}
		else if ((int)mSnailMode == 1)
		{
			((UIElement)speedUpBlueStacks.mEnableVt).Visibility = (Visibility)0;
		}
		new ContainerWindow(ParentWindow, (UserControl)(object)speedUpBlueStacks, 640.0, 200.0);
	}

	private void mBtvButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked btv button");
		BTVManager.Instance.StartBlueStacksTV();
	}

	private void TopBar_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		if (FeatureManager.Instance.IsBTVEnabled && string.Equals(Strings.CurrentDefaultVmName, ParentWindow.mVmName, (StringComparison)(-50359377 ^ -50359379)))
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mBtvButton, isVisible: true);
		}
		RefreshNotificationCentreButton();
		if (!ParentWindow.mGuestBootCompleted)
		{
			ParentWindow.mCommonHandler.SetSidebarImageProperties(isVisible: false, mSidebarButton, null);
			ParentWindow.GuestBootCompleted += ParentWindow_GuestBootCompletedEvent;
		}
		ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += TopBar_ScreenRecordingStateChangedEvent;
		VideoRecordingStatus videoRecordingStatus = mVideoRecordStatusControl;
		videoRecordingStatus.RecordingStoppedEvent = (Action)Delegate.Combine(videoRecordingStatus.RecordingStoppedEvent, new Action(TopBar_RecordingStoppedEvent));
		if (ParentWindow.mVmName == "Android" && ((FrameworkElement)mTitleIcon).ToolTip.ToString().Equals(Strings.ProductTopBarDisplayName, StringComparison.OrdinalIgnoreCase))
		{
			((FrameworkElement)mTitleIcon).ToolTip = (object)new ToolTip
			{
				Content = (Strings.ProductDisplayName ?? "")
			};
		}
	}

	private void TopBar_RecordingStoppedEvent()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mVideoRecordStatusControl).Visibility = (Visibility)(-746467326 + (1174769606 << 333065611));
		}, new object[0]);
	}

	private void TopBar_ScreenRecordingStateChangedEvent(bool isRecording)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (isRecording)
			{
				if ((int)((UIElement)mVideoRecordStatusControl).Visibility != 0 && CommonHandlers.sIsRecordingVideo)
				{
					mVideoRecordStatusControl.Init(ParentWindow);
					((UIElement)mVideoRecordStatusControl).Visibility = (Visibility)0;
				}
			}
			else
			{
				mVideoRecordStatusControl.ResetTimer();
				((UIElement)mVideoRecordStatusControl).Visibility = (Visibility)(0x49 ^ 0x4B);
			}
		}, new object[0]);
	}

	public void mNotificationCentreButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked notification_centre button");
		((FrameworkElement)mNotificationDrawerControl).Width = 320.0;
		SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, ParentWindow.mVmName, (StringComparison)(-159019325 - -159019327)) || !x.IsAndroidNotification));
		mNotificationDrawerControl.Populate(notificationItems);
		ClientStats.SendMiscellaneousStatsAsync("NotificationBellIconClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, null);
		GenericNotificationManager.MarkNotification(((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Keys, delegate(GenericNotificationItem x)
		{
			if (x.IsReceivedStatSent && !x.IsDeleted && !x.IsShown && !x.IsAndroidNotification)
			{
				x.IsShown = true;
				ClientStats.SendMiscellaneousStatsAsync("notification_shown", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, x.Id, x.Title, ((Dictionary<string, string>)(object)x.ExtraPayload).ContainsKey("campaign_id") ? ((Dictionary<string, string>)(object)x.ExtraPayload)["campaign_id"] : "");
			}
		});
		mNotificationDrawerControl.UpdateNotificationCount();
		if (sender != null)
		{
			mNotificationCentreButton.ImageName = "notification";
			((UIElement)mNotificationCountBadge).Visibility = (Visibility)(-360621 + (1477112826 >> 1469876492));
		}
		else
		{
			NotificationDrawer.DrawerAnimationTimer.Start();
		}
		((Popup)mNotificationCentrePopup).IsOpen = true;
		mNotificationDrawerControl.mNotificationScroll.ScrollToTop();
		mNotificationCentreButton.ImageName = "notification_hover";
	}

	internal bool CheckForRam()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		try
		{
			num = (int)(ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) / MB_MULTIPLIER);
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
		}
		if (num < 4096)
		{
			return true;
		}
		return false;
	}

	internal void RefreshNotificationCentreButton()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		if (ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && FeatureManager.Instance.IsShowNotificationCentre && (int)RegistryManager.Instance.InstallationType != 1)
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: true);
			if (((Dictionary<string, GenericNotificationItem>)(object)GenericNotificationManager.GetNotificationItems(delegate(GenericNotificationItem x)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Invalid comparison between Unknown and I4
				return !x.IsRead && !x.IsDeleted && (int)x.Priority == 0;
			})).Count > 0)
			{
				mNotificationCentreButton.ImageName = "notification_crucial";
			}
			else
			{
				mNotificationCentreButton.ImageName = "notification";
			}
			mNotificationDrawerControl.UpdateNotificationCount();
		}
		else
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: false);
		}
	}

	internal void mNotificationCentreDropDownBorder_LayoutUpdated(object sender, EventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		RectangleGeometry val = new RectangleGeometry();
		Rect val2 = default(Rect);
		((Rect)(ref val2)).Height = ((FrameworkElement)mNotificationCentreDropDownBorder).ActualHeight;
		((Rect)(ref val2)).Width = ((FrameworkElement)mNotificationCentreDropDownBorder).ActualWidth;
		Rect rect = val2;
		BlueStacksUIBinding.BindCornerRadiusToDouble((DependencyObject)(object)val, RectangleGeometry.RadiusXProperty, "PreferenceDropDownRadius");
		BlueStacksUIBinding.BindCornerRadiusToDouble((DependencyObject)(object)val, RectangleGeometry.RadiusYProperty, "PreferenceDropDownRadius");
		val.Rect = rect;
		((UIElement)mNotificationCentreDropDownBorder).Clip = (Geometry)(object)val;
	}

	internal void ShowRecordingIcons()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		mMacroRecordControl.Init(ParentWindow);
		((UIElement)mMacroRecordControl).Visibility = (Visibility)0;
		mMacroRecordControl.StartTimer();
		if (!ParentWindow.mIsFullScreen)
		{
			((Popup)ParentWindow.mTopBar.mMacroRecorderToolTipPopup).IsOpen = true;
			((Popup)ParentWindow.mTopBar.mMacroRecorderToolTipPopup).StaysOpen = true;
			DispatcherTimer val = new DispatcherTimer();
			int seconds = ((839713741 > 1239831408) ? 6 : 5);
			val.Interval = new TimeSpan(0, 0, 0, seconds, 0);
			mMacroRecordingPopupTimer = val;
			mMacroRecordingPopupTimer.Tick += MacroRecordingPopupTimer_Tick;
			mMacroRecordingPopupTimer.Start();
		}
	}

	private void MacroRecordingPopupTimer_Tick(object sender, EventArgs e)
	{
		((Popup)ParentWindow.mTopBar.mMacroRecorderToolTipPopup).IsOpen = false;
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	internal void HideRecordingIcons()
	{
		((UIElement)mConfigButton).Visibility = (Visibility)0;
		if (ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
		{
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: true);
			TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: true);
		}
		((UIElement)mMacroRecordControl).Visibility = (Visibility)(677841269 - 1852795666 % 1174954399);
		((Popup)mMacroRecorderToolTipPopup).IsOpen = false;
	}

	internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			mMacroPlayControl.Init(ParentWindow, record);
			((UIElement)mMacroPlayControl).Visibility = (Visibility)0;
			if (!ParentWindow.mIsFullScreen)
			{
				((Popup)ParentWindow.mTopBar.mMacroRunningToolTipPopup).IsOpen = true;
				((Popup)ParentWindow.mTopBar.mMacroRunningToolTipPopup).StaysOpen = true;
				mMacroRunningPopupTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 8609710 - 71771193 % 31580744, 0)
				};
				mMacroRunningPopupTimer.Tick += MacroRunningPopupTimer_Tick;
				mMacroRunningPopupTimer.Start();
			}
		}
	}

	private void MacroRunningPopupTimer_Tick(object sender, EventArgs e)
	{
		((Popup)ParentWindow.mTopBar.mMacroRunningToolTipPopup).IsOpen = false;
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	internal void HideMacroPlaybackFromTopBar()
	{
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			((UIElement)mConfigButton).Visibility = (Visibility)0;
			if (ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: true);
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: true);
			}
			((UIElement)mMacroPlayControl).Visibility = (Visibility)(297533763 + -297533761);
		}
	}

	internal void UpdateMacroRecordingProgress()
	{
		if (ParentWindow.mIsMacroPlaying || ParentWindow.mIsMacroRecorderActive)
		{
			((UIElement)mConfigButton).Visibility = (Visibility)0;
			if (ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
			{
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mNotificationGrid, isVisible: true);
				TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mUserAccountBtn, isVisible: true);
			}
		}
	}

	internal void ShowSyncIcon()
	{
		TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mOperationsSyncGrid, isVisible: true);
	}

	internal void HideSyncIcon()
	{
		TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mOperationsSyncGrid, isVisible: false);
	}

	private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow?.mCommonHandler?.FlipSidebarVisibility((CustomPictureBox)((sender is CustomPictureBox) ? sender : null), null);
	}

	private void TopBar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			TopBarButtonsHandling();
		}
	}

	private void TopBarButtonsHandling()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		double num = ((FrameworkElement)this).ActualWidth - 180.0 - (double)(mAppTabButtons.mDictTabs.Count * 48);
		double num2 = ((FrameworkElement)mOptionsDockPanel).ActualWidth;
		if (num2 > num)
		{
			foreach (KeyValuePair<FrameworkElement, double> value2 in mOptionsPriorityPanel.Values)
			{
				if ((int)((UIElement)value2.Key).Visibility == 0)
				{
					((UIElement)value2.Key).Visibility = (Visibility)2;
					num2 -= value2.Value;
				}
				if (num2 < num)
				{
					break;
				}
			}
			return;
		}
		for (int num3 = mOptionsPriorityPanel.Count - 1; num3 >= 0; num3--)
		{
			KeyValuePair<FrameworkElement, double> value = mOptionsPriorityPanel.ElementAt(num3).Value;
			if ((int)((UIElement)value.Key).Visibility == 2)
			{
				if (!(num2 + value.Value < num))
				{
					break;
				}
				((UIElement)value.Key).Visibility = (Visibility)0;
				num2 += value.Value;
			}
		}
	}

	private bool ContainsKey(FrameworkElement element)
	{
		foreach (KeyValuePair<FrameworkElement, double> value in mOptionsPriorityPanel.Values)
		{
			if (value.Key == element)
			{
				return true;
			}
		}
		return false;
	}

	private void RemoveKey(FrameworkElement element)
	{
		foreach (KeyValuePair<int, KeyValuePair<FrameworkElement, double>> item in mOptionsPriorityPanel)
		{
			if (item.Value.Key == element)
			{
				mOptionsPriorityPanel.Remove(item.Key);
				break;
			}
		}
	}

	internal void TopBarOptionsPanelElementVisibility(FrameworkElement element, bool isVisible)
	{
		if (isVisible)
		{
			double num = ((FrameworkElement)this).ActualWidth - 180.0 - (double)(mAppTabButtons.mDictTabs.Count * (-993941575 ^ -993941623));
			if (((FrameworkElement)mOptionsDockPanel).ActualWidth + element.Width < num)
			{
				((UIElement)element).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)element).Visibility = (Visibility)(-1196031829 ^ -1196031831);
			}
			if (!ContainsKey(element))
			{
				mOptionsPriorityPanel.Add(int.Parse(element.Tag.ToString(), CultureInfo.InvariantCulture), new KeyValuePair<FrameworkElement, double>(element, element.Width));
			}
		}
		else
		{
			((UIElement)element).Visibility = (Visibility)(-538872378 ^ -538872380);
			if (ContainsKey(element))
			{
				RemoveKey(element);
			}
		}
	}

	void ITopBar.ShowSyncPanel(bool isSource)
	{
		((UIElement)mOperationsSyncGrid).Visibility = (Visibility)0;
		if (isSource)
		{
			mPlayPauseSyncButton.ImageName = "pause_title_bar";
			((UIElement)mPlayPauseSyncButton).Visibility = (Visibility)0;
			((UIElement)mStopSyncButton).Visibility = (Visibility)0;
		}
	}

	void ITopBar.HideSyncPanel()
	{
		((UIElement)mOperationsSyncGrid).Visibility = (Visibility)(487082946 + -487082944);
		((UIElement)mPlayPauseSyncButton).Visibility = (Visibility)(-469944010 ^ -469944012);
		CustomPictureBox obj = mStopSyncButton;
		int num = ((10544577 > 961901264) ? 2 : 2);
		((UIElement)obj).Visibility = (Visibility)num;
		((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
	}

	private void PlayPauseSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName.Equals("pause_title_bar", StringComparison.InvariantCultureIgnoreCase))
		{
			((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName = "play_title_bar";
			ParentWindow.mSynchronizerWindow.PauseAllSyncOperations();
		}
		else
		{
			((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName = "pause_title_bar";
			ParentWindow.mSynchronizerWindow.PlayAllSyncOperations();
		}
	}

	private void StopSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((ITopBar)this).HideSyncPanel();
		ParentWindow.mSynchronizerWindow.StopAllSyncOperations();
		if (RegistryManager.Instance.IsShowToastNotification)
		{
			ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STOPPED", ""));
		}
	}

	private void OperationsSyncGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		if (ParentWindow.mIsSynchronisationActive)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = true;
		}
	}

	private void OperationsSyncGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		if (ParentWindow.mIsSynchronisationActive && !((UIElement)mOperationsSyncGrid).IsMouseOver && !((UIElement)mSyncInstancesToolTipPopup).IsMouseOver)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
		}
	}

	private void SyncInstancesToolTip_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mOperationsSyncGrid).IsMouseOver && !((UIElement)mSyncInstancesToolTipPopup).IsMouseOver)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
		}
	}

	internal void ClosePopups()
	{
		if (((Popup)mMacroRecorderToolTipPopup).IsOpen)
		{
			((Popup)mMacroRecorderToolTipPopup).IsOpen = false;
		}
		if (((Popup)mMacroRunningToolTipPopup).IsOpen)
		{
			((Popup)mMacroRunningToolTipPopup).IsOpen = false;
		}
		if (((Popup)mNotificationCentrePopup).IsOpen)
		{
			((Popup)mNotificationCentrePopup).IsOpen = false;
		}
		if (((Popup)mSettingsMenuPopup).IsOpen)
		{
			((Popup)mSettingsMenuPopup).IsOpen = false;
		}
		if (((Popup)mSyncInstancesToolTipPopup).IsOpen)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
		}
	}

	private void HelpButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			BlueStacksUIUtils.OpenUrl(helpCenterUrl);
		}
		else
		{
			ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", isSwitch: true, "FEEDBACK_TEXT");
		}
	}

	private void mNotificationCentrePopup_Closed(object sender, EventArgs e)
	{
		GenericNotificationManager.MarkNotification(new List<string>(((Dictionary<string, GenericNotificationItem>)(object)GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, ParentWindow.mVmName, StringComparison.InvariantCulture))).Keys), delegate(GenericNotificationItem x)
		{
			x.IsRead = true;
		});
		mNotificationDrawerControl.UpdateNotificationCount();
		mNotificationCentreButton.ImageName = "notification";
		((UIElement)mNotificationCentreButton).IsEnabled = true;
	}

	private void mNotificationCentrePopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mNotificationCentreButton).IsEnabled = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/topbar.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Expected O, but got Unknown
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Expected O, but got Unknown
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Expected O, but got Unknown
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Expected O, but got Unknown
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Expected O, but got Unknown
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Expected O, but got Unknown
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Expected O, but got Unknown
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected O, but got Unknown
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Expected O, but got Unknown
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Expected O, but got Unknown
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Expected O, but got Unknown
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Expected O, but got Unknown
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Expected O, but got Unknown
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Expected O, but got Unknown
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Expected O, but got Unknown
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Expected O, but got Unknown
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Expected O, but got Unknown
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Expected O, but got Unknown
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Expected O, but got Unknown
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Expected O, but got Unknown
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Expected O, but got Unknown
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Expected O, but got Unknown
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Expected O, but got Unknown
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Expected O, but got Unknown
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(TopBar)target).Loaded += new RoutedEventHandler(TopBar_Loaded);
			((FrameworkElement)(TopBar)target).SizeChanged += new SizeChangedEventHandler(TopBar_SizeChanged);
			break;
		case 2:
			mMainGrid = (Grid)target;
			break;
		case 3:
			WindowHeaderGrid = (Grid)target;
			((FrameworkElement)WindowHeaderGrid).Margin = new Thickness(-40.0, 8.0, 0.0, -999.0);
			break;
		case 4:
			mTitleIcon = (CustomPictureBox)target;
			break;
		case 5:
			mTitleTextGrid = (Grid)target;
			break;
		case 6:
			mTitleText = (TextBlock)target;
			break;
		case 7:
			mVersionText = (TextBlock)target;
			((UIElement)mVersionText).Visibility = (Visibility)(1190420482 - (1704605603 << 1422335117));
			break;
		case 8:
			mOptionsDockPanel = (DockPanel)target;
			break;
		case 9:
			mSidebarButton = (CustomPictureBox)target;
			((UIElement)mSidebarButton).MouseLeftButtonUp += new MouseButtonEventHandler(MSidebarButton_MouseLeftButtonUp);
			break;
		case 10:
			mCloseButton = (CustomPictureBox)target;
			((UIElement)mCloseButton).MouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 11:
			mMaximizeButton = (CustomPictureBox)target;
			((UIElement)mMaximizeButton).MouseLeftButtonUp += new MouseButtonEventHandler(MaxmizeButton_MouseLeftButtonUp);
			break;
		case 12:
			mMinimizeButton = (CustomPictureBox)target;
			((UIElement)mMinimizeButton).MouseLeftButtonUp += new MouseButtonEventHandler(MinimizeButton_MouseLeftButtonUp);
			break;
		case 13:
			mConfigButton = (CustomPictureBox)target;
			((UIElement)mConfigButton).MouseLeftButtonUp += new MouseButtonEventHandler(ConfigButton_MouseLeftButtonUp);
			break;
		case 14:
			mSettingsBtnNotification = (Ellipse)target;
			break;
		case 15:
			mSettingsMenuPopup = (CustomPopUp)target;
			break;
		case 16:
			mPreferenceDropDownBorder = (Border)target;
			break;
		case 17:
			mGrid = (Grid)target;
			break;
		case 18:
			mMaskBorder = (Border)target;
			break;
		case 19:
			mPreferenceDropDownControl = (PreferenceDropDownControl)target;
			break;
		case 20:
			mHelpButton = (CustomPictureBox)target;
			((UIElement)mHelpButton).MouseLeftButtonUp += new MouseButtonEventHandler(HelpButton_MouseLeftButtonUp);
			break;
		case 21:
			mUserAccountBtn = (CustomPictureBox)target;
			((UIElement)mUserAccountBtn).MouseLeftButtonUp += new MouseButtonEventHandler(UserAccountButton_MouseLeftButtonUp);
			break;
		case 22:
			mNotificationGrid = (Grid)target;
			break;
		case 23:
			mNotificationCentreButton = (CustomPictureBox)target;
			((UIElement)mNotificationCentreButton).MouseLeftButtonUp += new MouseButtonEventHandler(mNotificationCentreButton_MouseLeftButtonUp);
			break;
		case 24:
			mNotificationCountBadge = (Canvas)target;
			break;
		case 25:
			mNotificationCentrePopup = (CustomPopUp)target;
			break;
		case 26:
			mNotificationCaret = (Path)target;
			break;
		case 27:
			mNotificationCentreDropDownBorder = (Border)target;
			((UIElement)mNotificationCentreDropDownBorder).LayoutUpdated += mNotificationCentreDropDownBorder_LayoutUpdated;
			break;
		case 28:
			mMaskBorder1 = (Border)target;
			break;
		case 29:
			mNotificationDrawerControl = (NotificationDrawer)target;
			break;
		case 30:
			mBtvButton = (CustomPictureBox)target;
			((UIElement)mBtvButton).MouseLeftButtonUp += new MouseButtonEventHandler(mBtvButton_MouseLeftButtonUp);
			break;
		case 31:
			mWarningButton = (CustomPictureBox)target;
			((UIElement)mWarningButton).MouseLeftButtonUp += new MouseButtonEventHandler(mWarningButton_MouseLeftButtonUp);
			break;
		case 32:
			mOperationsSyncGrid = (Grid)target;
			((UIElement)mOperationsSyncGrid).MouseEnter += new MouseEventHandler(OperationsSyncGrid_MouseEnter);
			((UIElement)mOperationsSyncGrid).MouseLeave += new MouseEventHandler(OperationsSyncGrid_MouseLeave);
			break;
		case 33:
			mSyncMaskBorder = (Border)target;
			break;
		case 34:
			mPlayPauseSyncButton = (CustomPictureBox)target;
			((UIElement)mPlayPauseSyncButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PlayPauseSyncButton_PreviewMouseLeftButtonUp);
			break;
		case 35:
			mStopSyncButton = (CustomPictureBox)target;
			((UIElement)mStopSyncButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopSyncButton_PreviewMouseLeftButtonUp);
			break;
		case 36:
			mSyncInstancesToolTipPopup = (CustomPopUp)target;
			break;
		case 37:
			mDummyGrid = (Grid)target;
			break;
		case 38:
			mMaskBorder2 = (Border)target;
			break;
		case 39:
			mUpwardArrow = (Path)target;
			break;
		case 40:
			mLocalConfigIndicator = (CustomPictureBox)target;
			break;
		case 41:
			mAppTabButtons = (AppTabButtons)target;
			break;
		case 42:
			mVideoRecordingStatusGrid = (Grid)target;
			break;
		case 43:
			mVideoRecordStatusControl = (VideoRecordingStatus)target;
			break;
		case 44:
			mMacroGrid = (Grid)target;
			break;
		case 45:
			mMacroRecordControl = (MacroTopBarRecordControl)target;
			break;
		case 46:
			mMacroPlayControl = (MacroTopBarPlayControl)target;
			break;
		case 47:
			mMacroRecorderToolTipPopup = (CustomPopUp)target;
			break;
		case 48:
			dummyGrid = (Grid)target;
			break;
		case 49:
			mMaskBorder3 = (Border)target;
			break;
		case 50:
			mMacroRecordingTooltip = (TextBlock)target;
			break;
		case 51:
			mUpArrow = (Path)target;
			break;
		case 52:
			mMacroRunningToolTipPopup = (CustomPopUp)target;
			break;
		case 53:
			grid = (Grid)target;
			break;
		case 54:
			mMaskBorder4 = (Border)target;
			break;
		case 55:
			mMacroRunningTooltip = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
