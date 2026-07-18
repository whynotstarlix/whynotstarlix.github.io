using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class AppTabButtons : UserControl, IComponentConnector
{
	private enum TabMode
	{
		ParallelogramMode,
		IconMode
	}

	private MainWindow mMainWindow;

	internal AppTabButton mHomeAppTabButton;

	private int mTabMinWidth = 48;

	private DateTime mLastTimeOfSizeChangeEventRecieved = DateTime.Now;

	private int mSizeChangedEventCountInLast2Seconds = 1;

	internal Dictionary<string, AppTabButton> mDictTabs = new Dictionary<string, AppTabButton>(StringComparer.OrdinalIgnoreCase);

	internal string mLastPackageForQuitPopupDisplayed = "";

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AppTabButton mMoreTabButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mHiddenButtons;

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

	public EventHandler<EventArgs> EventOnTabChanged { get; set; }

	public List<string> ListTabHistory { get; } = new List<string>();

	public int AreaForTABS
	{
		get
		{
			int num = (int)(((FrameworkElement)this).ActualWidth - 20.0);
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
	}

	internal AppTabButton SelectedTab => ParentWindow.StaticComponents.mSelectedTabButton;

	public AppTabButtons()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		InitializeComponent();
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
			((FrameworkElement)this).Loaded += new RoutedEventHandler(AppTabButtons_Loaded);
		}
	}

	private void AppTabButtons_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		((FrameworkElement)this).Loaded -= new RoutedEventHandler(AppTabButtons_Loaded);
		if (!FeatureManager.Instance.IsCustomUIForDMM && (int)RegistryManager.Instance.InstallationType == 2)
		{
			Logger.Info("Test logs: AppTabButtons_Loaded()");
			AddHomeTab();
		}
	}

	internal void AddHomeTab()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		Logger.Info("Test logs: AddHomeTab()");
		AppTabButton appTabButton = (mHomeAppTabButton = new AppTabButton());
		((Panel)mPanel).Children.Insert(0, (UIElement)(object)appTabButton);
		appTabButton.Init("STRING_HOME", "Home", string.Empty, "home", ParentWindow.WelcomeTabParentGrid, "Home");
		BlueStacksUIBinding.Bind(appTabButton.mTabLabel, "STRING_HOME");
		((UIElement)appTabButton).MouseUp += new MouseButtonEventHandler(AppTabButton_MouseUp);
		mDictTabs[appTabButton.PackageName] = appTabButton;
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			((UIElement)appTabButton).Visibility = (Visibility)2;
		}
		ResizeTabs();
		GoToTab("Home", isLaunch: false);
	}

	internal void AddHiddenAppTabAndLaunch(string packageName, string activityName)
	{
		AddAppTab("", packageName, activityName, "", isSwitch: true, isLaunch: true);
		((UIElement)ParentWindow.StaticComponents.mSelectedTabButton).Visibility = (Visibility)2;
	}

	internal void AddAppTab(string appName, string packageName, string activityName, string imageName, bool isSwitch, bool isLaunch, bool receivedFromImap = false)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Invalid comparison between Unknown and I4
		DoExtraHandlingForApp(packageName);
		if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(packageName) == true)
		{
			ParentWindow.EngineInstanceRegistry.LastNotificationEnabledAppLaunched = packageName;
		}
		if (mDictTabs.ContainsKey(packageName))
		{
			GoToTab(packageName, isLaunch, receivedFromImap);
			return;
		}
		AppTabButton mSelectedTabButton = ParentWindow.StaticComponents.mSelectedTabButton;
		AppTabButton appTabButton = new AppTabButton();
		appTabButton.Init(appName, packageName, activityName, imageName, ParentWindow.FrontendParentGrid, packageName);
		((UIElement)appTabButton).MouseUp += new MouseButtonEventHandler(AppTabButton_MouseUp);
		if (ParentWindow.mDiscordhandler != null)
		{
			ParentWindow.mDiscordhandler.AssignTabChangeEvent(appTabButton);
		}
		if (FeatureManager.Instance.IsCustomUIForDMM && ParentWindow.mDmmBottomBar != null)
		{
			appTabButton.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Combine(appTabButton.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(ParentWindow.mDmmBottomBar.Tab_Changed));
		}
		mDictTabs.Add(packageName, appTabButton);
		((Panel)mPanel).Children.Add((UIElement)(object)appTabButton);
		if (Oem.Instance.SendAppClickStatsFromClient)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				AppInfo appInfoFromPackageName = new JsonParser(ParentWindow.mVmName).GetAppInfoFromPackageName(packageName);
				string text = string.Empty;
				string text2 = string.Empty;
				if (appInfoFromPackageName != null)
				{
					if (!string.IsNullOrEmpty(appInfoFromPackageName.Version))
					{
						text = appInfoFromPackageName.Version;
					}
					if (!string.IsNullOrEmpty(appInfoFromPackageName.VersionName))
					{
						text2 = appInfoFromPackageName.VersionName;
					}
				}
				Stats.SendAppStats(appName, packageName, text, "HomeVersionNotKnown", (AppType)0, ParentWindow.mVmName, text2);
			});
		}
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			((UIElement)appTabButton).Visibility = (Visibility)2;
		}
		else if (mSelectedTabButton != null && mSelectedTabButton.IsPortraitModeTab && mSelectedTabButton.mTabType == TabType.AppTab)
		{
			appTabButton.IsPortraitModeTab = true;
		}
		ResizeTabs();
		if (isSwitch)
		{
			GoToTab(packageName, isLaunch, receivedFromImap);
		}
	}

	private void DoExtraHandlingForApp(string packageName)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		if ((int)RegistryManager.Instance.FirstAppLaunchState == 2 && JsonParser.GetInstalledAppsList(ParentWindow.mVmName).Contains(packageName))
		{
			RegistryManager.Instance.FirstAppLaunchState = (AppLaunchState)3;
		}
		if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(packageName))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][packageName] = new AppSettings();
		}
	}

	internal void AddWebTab(string url, string tabName, string imageName, bool isSwitch, string tabKey = "", bool forceRefresh = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			return;
		}
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			Process.Start(url);
			return;
		}
		bool flag = false;
		if (!string.IsNullOrEmpty(tabKey))
		{
			flag = true;
		}
		if (mDictTabs.ContainsKey(flag ? tabKey : url))
		{
			BrowserControl browserControl = mDictTabs[flag ? tabKey : url].GetBrowserControl();
			if (browserControl == null)
			{
				mDictTabs[tabKey].mControlGrid = ParentWindow.AddBrowser(url);
				mDictTabs[tabKey].Init(tabName, url, imageName, mDictTabs[tabKey].mControlGrid, tabKey);
			}
			if (flag && string.Compare(url, mDictTabs[tabKey].PackageName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				browserControl = mDictTabs[tabKey].GetBrowserControl();
				mDictTabs[tabKey].Init(tabName, url, imageName, mDictTabs[tabKey].mControlGrid, tabKey);
				browserControl?.UpdateUrlAndRefresh(url);
			}
			else if (forceRefresh)
			{
				browserControl = mDictTabs[flag ? tabKey : url].GetBrowserControl();
				browserControl.UpdateUrlAndRefresh(browserControl.mUrl);
			}
			GoToTab(flag ? tabKey : url);
			return;
		}
		AppTabButton appTabButton = new AppTabButton();
		Grid val = ParentWindow.AddBrowser(url);
		((UIElement)val).Visibility = (Visibility)0;
		appTabButton.Init(tabName, url, imageName, val, flag ? tabKey : url);
		((UIElement)appTabButton).MouseUp += new MouseButtonEventHandler(AppTabButton_MouseUp);
		if (ParentWindow.mDiscordhandler != null)
		{
			ParentWindow.mDiscordhandler.AssignTabChangeEvent(appTabButton);
		}
		mDictTabs.Add(flag ? tabKey : url, appTabButton);
		((Panel)mPanel).Children.Add((UIElement)(object)appTabButton);
		ResizeTabs();
		if (isSwitch)
		{
			GoToTab(flag ? tabKey : url);
		}
		ClientStats.SendMiscellaneousStatsAsync("WebTabLaunched", RegistryManager.Instance.UserGuid, url, appTabButton.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM);
	}

	internal void KillWebTabs()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!RegistryManager.Instance.SwitchKillWebTab)
		{
			return;
		}
		foreach (KeyValuePair<string, AppTabButton> mDictTab in mDictTabs)
		{
			if (mDictTab.Value.mTabType != TabType.WebTab)
			{
				continue;
			}
			BrowserControl browserControl = null;
			foreach (object child in ((Panel)mDictTab.Value.mControlGrid).Children)
			{
				if (!(child is BrowserControl { CefBrowser: not null } browserControl2))
				{
					continue;
				}
				foreach (BrowserControlTags key in browserControl2.TagsSubscribedDict.Keys)
				{
					browserControl2.mSubscriber?.UnsubscribeTag(key);
				}
				((WpfCefBrowser)browserControl2.CefBrowser).Dispose();
				browserControl2.CefBrowser = null;
			}
		}
	}

	private void AppTabButton_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.ChangedButton == 1)
		{
			string tabKey = (sender as AppTabButton).TabKey;
			if (!string.IsNullOrEmpty(tabKey))
			{
				CloseTab(tabKey, sendStopAppToAndroid: true);
			}
		}
	}

	private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (!ParentWindow.mIsFullScreen)
		{
			RefreshUI();
		}
	}

	private void RefreshUI()
	{
		if ((DateTime.Now - mLastTimeOfSizeChangeEventRecieved).TotalSeconds > 2.0)
		{
			mLastTimeOfSizeChangeEventRecieved = DateTime.Now;
			mSizeChangedEventCountInLast2Seconds = 1;
		}
		else
		{
			mSizeChangedEventCountInLast2Seconds++;
		}
		if (mSizeChangedEventCountInLast2Seconds <= 500)
		{
			if (ParentWindow.IsUIInPortraitMode)
			{
				SwitchToIconMode(isSwitchToIconMode: true);
			}
			else
			{
				SwitchToIconMode(isSwitchToIconMode: false);
			}
			ResizeTabs();
		}
	}

	private void SwitchToIconMode(bool isSwitchToIconMode)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if (isSwitchToIconMode)
		{
			mTabMinWidth = 38;
			if ((int)RegistryManager.Instance.InstallationType == 1)
			{
				((UIElement)mMoreTabButton).Visibility = (Visibility)1;
				((UIElement)ParentWindow.mTopBar.mTitleText).Visibility = (Visibility)2;
			}
			else if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				((UIElement)ParentWindow.mTopBar.mTitleTextGrid).Visibility = (Visibility)2;
			}
			mMoreTabButton.MakeTabParallelogram(isSkewTab: false);
		}
		else
		{
			if ((int)RegistryManager.Instance.InstallationType == 1)
			{
				((UIElement)ParentWindow.mTopBar.mTitleText).Visibility = (Visibility)0;
			}
			mTabMinWidth = 48;
			mMoreTabButton.MakeTabParallelogram(isSkewTab: true);
		}
		ParentWindow.mTopBar.RefreshWarningButton();
	}

	internal void ResizeTabs()
	{
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.mIsFullScreen)
		{
			return;
		}
		double num = MacroGridHandling();
		num += VideoRecordingGridHandling();
		if (((FrameworkElement)ParentWindow.mTopBar).ActualWidth > ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num + 40.0)
		{
			((UIElement)ParentWindow.mTopBar.mTitleIcon).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)ParentWindow.mTopBar.mTitleIcon).Visibility = (Visibility)2;
		}
		if (((FrameworkElement)ParentWindow.mTopBar).ActualWidth > ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + 140.0 + num + (double)(mDictTabs.Count * 48))
		{
			((UIElement)ParentWindow.mTopBar.mTitleTextGrid).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)ParentWindow.mTopBar.mTitleTextGrid).Visibility = (Visibility)2;
		}
		int num2 = ((Panel)mPanel).Children.Count + ((Panel)mHiddenButtons).Children.Count;
		if (num2 > 0)
		{
			double num3 = mTabMinWidth;
			if (AreaForTABS >= num2 * mTabMinWidth)
			{
				num3 = AreaForTABS / num2;
			}
			for (int i = 0; i < ((Panel)mPanel).Children.Count; i++)
			{
				(((Panel)mPanel).Children[i] as AppTabButton).ResizeButton(num3);
			}
			if ((double)AreaForTABS >= num3 * (double)num2)
			{
				if (((Panel)mHiddenButtons).Children.Count > 0)
				{
					ShowXTabs(((Panel)mHiddenButtons).Children.Count, num3);
				}
			}
			else
			{
				int num4 = AreaForTABS / mTabMinWidth - 1;
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					CornerRadius val = BlueStacksUIBinding.Instance.CornerRadiusModel["TabMarginPortrait"];
					int num5 = (int)Math.Floor(((CornerRadius)(ref val)).TopLeft);
					int num6 = (int)Math.Floor(((FrameworkElement)mMoreTabButton).ActualWidth) + num5;
					num4 = (AreaForTABS - num6) / (mTabMinWidth + num5);
				}
				if (num4 > num2)
				{
					num4 = num2;
				}
				if (num4 > ((Panel)mPanel).Children.Count || num2 == 1)
				{
					ShowXTabs(num4 - ((Panel)mPanel).Children.Count, num3);
				}
				else if (num4 < ((Panel)mPanel).Children.Count)
				{
					HideXTabs(((Panel)mPanel).Children.Count - num4);
				}
			}
		}
		if (((Panel)mHiddenButtons).Children.Count > 0)
		{
			((UIElement)mMoreTabButton).Visibility = (Visibility)0;
			mMoreTabButton.MoreTabsButtonHandling();
		}
		else
		{
			((UIElement)mMoreTabButton).Visibility = (Visibility)1;
		}
	}

	private double MacroGridHandling()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		if ((int)((UIElement)ParentWindow.mTopBar.mMacroRecordControl).Visibility == 0)
		{
			num = ((FrameworkElement)ParentWindow.mTopBar.mMacroRecordControl).MaxWidth;
		}
		else if ((int)((UIElement)ParentWindow.mTopBar.mMacroPlayControl).Visibility == 0)
		{
			num = ((FrameworkElement)ParentWindow.mTopBar.mMacroPlayControl).MaxWidth;
		}
		if (num > 0.0)
		{
			if (((FrameworkElement)ParentWindow.mTopBar).ActualWidth > ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
			{
				((UIElement)ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay).Visibility = (Visibility)0;
				((UIElement)ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay).Visibility = (Visibility)2;
				((UIElement)ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel).Visibility = (Visibility)2;
			}
		}
		return num;
	}

	private double VideoRecordingGridHandling()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		if ((int)((UIElement)ParentWindow.mTopBar.mVideoRecordStatusControl).Visibility == 0)
		{
			num = ((FrameworkElement)ParentWindow.mTopBar.mVideoRecordStatusControl).MaxWidth;
		}
		if (num > 0.0)
		{
			if (((FrameworkElement)ParentWindow.mTopBar).ActualWidth > ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
			{
				((UIElement)ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel).Visibility = (Visibility)2;
			}
		}
		return num;
	}

	private void ShowXTabs(int x, double tabWidth)
	{
		for (int i = 0; i < x; i++)
		{
			AppTabButton appTabButton = mDictTabs.Values.First();
			foreach (AppTabButton value in mDictTabs.Values)
			{
				if (((Panel)mHiddenButtons).Children.Contains((UIElement)(object)value))
				{
					appTabButton = value;
					break;
				}
			}
			appTabButton.ResizeButton(tabWidth);
			appTabButton.UpdateUIForDropDown(isInDropDown: false);
			if (!((Panel)mPanel).Children.Contains((UIElement)(object)appTabButton))
			{
				((Panel)mHiddenButtons).Children.Remove((UIElement)(object)appTabButton);
				if (appTabButton.mTabType == TabType.HomeTab)
				{
					((Panel)mPanel).Children.Insert(0, (UIElement)(object)appTabButton);
				}
				else
				{
					((Panel)mPanel).Children.Add((UIElement)(object)appTabButton);
				}
			}
		}
	}

	private void HideXTabs(int x)
	{
		for (int i = 0; i < x; i++)
		{
			AppTabButton appTabButton = mDictTabs.Values.Last();
			for (int num = mDictTabs.Count - 1; num >= 0; num--)
			{
				AppTabButton value = mDictTabs.ElementAt(num).Value;
				if (((Panel)mPanel).Children.Contains((UIElement)(object)value))
				{
					appTabButton = value;
					break;
				}
			}
			appTabButton.UpdateUIForDropDown(isInDropDown: true);
			if (!((Panel)mHiddenButtons).Children.Contains((UIElement)(object)appTabButton))
			{
				((Panel)mPanel).Children.Remove((UIElement)(object)appTabButton);
				((Panel)mHiddenButtons).Children.Add((UIElement)(object)appTabButton);
			}
		}
	}

	internal void CloseTab(string tabKey, bool sendStopAppToAndroid = false, bool forceClose = false, bool dontCheckQuitPopup = false, bool receivedFromImap = false, string topActivityPackageName = "")
	{
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		if (!mDictTabs.ContainsKey(tabKey))
		{
			return;
		}
		if (ParentWindow.SendClientActions && !receivedFromImap)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>
			{
				{ "EventAction", "TabClosed" },
				{ "tabKey", tabKey },
				{
					"sendStopAppToAndroid",
					sendStopAppToAndroid.ToString(CultureInfo.InvariantCulture)
				},
				{
					"forceClose",
					forceClose.ToString(CultureInfo.InvariantCulture)
				}
			};
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)0;
			dictionary.Add("operationData", JsonConvert.SerializeObject((object)dictionary2, serializerSettings));
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
		}
		AppTabButton appTabButton = mDictTabs[tabKey];
		if (appTabButton.mTabType == TabType.WebTab)
		{
			BrowserControl browserControl = null;
			foreach (object child in ((Panel)appTabButton.mControlGrid).Children)
			{
				browserControl = child as BrowserControl;
				if (browserControl != null)
				{
					break;
				}
			}
			string arg = string.Empty;
			if (browserControl != null)
			{
				arg = browserControl.mUrl;
				((Panel)appTabButton.mControlGrid).Children.Remove((UIElement)(object)browserControl);
				if (browserControl.CefBrowser != null)
				{
					foreach (BrowserControlTags key in browserControl.TagsSubscribedDict.Keys)
					{
						browserControl.mSubscriber?.UnsubscribeTag(key);
					}
					((WpfCefBrowser)browserControl.CefBrowser).Dispose();
				}
			}
			ClientStats.SendMiscellaneousStatsAsync("WebTabClosed", RegistryManager.Instance.UserGuid, arg, appTabButton.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM);
		}
		if (FeatureManager.Instance.IsCheckForQuitPopup && !RegistryManager.Instance.Guest[ParentWindow.mVmName].IsGoogleSigninDone && appTabButton.mTabType == TabType.AppTab && appTabButton.PackageName == "com.android.vending")
		{
			QuitPopupControl quitPopupControl = new QuitPopupControl(ParentWindow);
			string tag = (quitPopupControl.CurrentPopupTag = "exit_popup_ots");
			BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
			BlueStacksUIBinding.Bind((Button)(object)quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_TAB");
			quitPopupControl.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
			quitPopupControl.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
			quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
			((UIElement)quitPopupControl.CloseBlueStacksButton).PreviewMouseUp += (MouseButtonEventHandler)delegate
			{
				CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
			};
			((UIElement)quitPopupControl.CrossButtonPictureBox).PreviewMouseUp += (MouseButtonEventHandler)delegate
			{
				if (string.Equals(topActivityPackageName, "com.bluestacks.appmart", StringComparison.InvariantCulture))
				{
					CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
				}
			};
			ParentWindow.HideDimOverlay();
			ParentWindow.ShowDimOverlay(quitPopupControl);
			ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
		}
		else if (!FeatureManager.Instance.IsCustomUIForDMM && !dontCheckQuitPopup && appTabButton.mTabType == TabType.AppTab && tabKey != mLastPackageForQuitPopupDisplayed && !ParentWindow.SendClientActions && !receivedFromImap && ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (AppIconModel _) => _.IsInstalledApp) && ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (AppIconModel _) => !_.IsAppSuggestionActive))
		{
			ProgressBar obj = new ProgressBar
			{
				ProgressText = "STRING_LOADING_MESSAGE"
			};
			((UIElement)obj).Visibility = (Visibility)1;
			ProgressBar el = obj;
			ParentWindow.ShowDimOverlay(el);
			mLastPackageForQuitPopupDisplayed = tabKey;
			Thread thread = new Thread((ThreadStart)delegate
			{
				if (!ParentWindow.Utils.CheckQuitPopupFromCloud(tabKey))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
					}, new object[0]);
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}
		else
		{
			CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
		}
	}

	internal void CloseTabAfterQuitPopup(string tabKey, bool sendStopAppToAndroid, bool forceClose)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		if (!mDictTabs.ContainsKey(tabKey))
		{
			return;
		}
		AppTabButton appTabButton = mDictTabs[tabKey];
		if (appTabButton.mTabType != TabType.HomeTab && ParentWindow.mDimOverlay != null && ParentWindow.mDimOverlay.Control != null && ((FeatureManager.Instance.IsCustomUIForNCSoft && (object)ParentWindow.mDimOverlay.Control.GetType() == ((object)ParentWindow.ScreenLockInstance).GetType()) || !FeatureManager.Instance.IsCustomUIForNCSoft))
		{
			ParentWindow.HideDimOverlay();
			((Popup)mPopup).IsOpen = false;
		}
		mLastPackageForQuitPopupDisplayed = "";
		if (!(appTabButton.mTabType != TabType.HomeTab || forceClose))
		{
			return;
		}
		Publisher.PublishMessage((BrowserControlTags)3, ParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)appTabButton.PackageName)));
		DependencyObject parent = ((FrameworkElement)appTabButton).Parent;
		((Panel)((parent is Panel) ? parent : null)).Children.Remove((UIElement)(object)appTabButton);
		mDictTabs.Remove(tabKey);
		if (appTabButton.mTabType == TabType.AppTab || appTabButton.mTabType == TabType.HomeTab)
		{
			ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
		}
		if (sendStopAppToAndroid && appTabButton.mTabType == TabType.AppTab)
		{
			ParentWindow.mAppHandler.StopAppRequest(appTabButton.PackageName);
		}
		ListTabHistory.RemoveAll((string n) => n.Equals(tabKey, StringComparison.OrdinalIgnoreCase));
		if (ParentWindow.mDiscordhandler != null)
		{
			ParentWindow.mDiscordhandler.RemoveAppFromTimestampList(tabKey);
		}
		if (FeatureManager.Instance.IsCustomUIForDMM && ListTabHistory.Count == 0)
		{
			((Window)ParentWindow).Hide();
			ParentWindow.RestoreWindows();
			if (ParentWindow.mDMMRecommendedWindow != null)
			{
				((UIElement)ParentWindow.mDMMRecommendedWindow).Visibility = (Visibility)1;
			}
			ParentWindow.StaticComponents.mSelectedTabButton.IsPortraitModeTab = false;
		}
		else if (appTabButton.IsSelected)
		{
			if (ListTabHistory.Count != 0)
			{
				GoToTab(ListTabHistory[ListTabHistory.Count - 1]);
			}
			else
			{
				Logger.Fatal("No tab to go back to! Ignoring");
			}
		}
		ResizeTabs();
	}

	internal bool GoToTab(string key, bool isLaunch = true, bool receivedFromImap = false)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Test logs: GoToTab() key: " + key + ", isPresentInmDict: " + mDictTabs.ContainsKey(key));
		bool result = false;
		if (InteropWindow.GetForegroundWindow() != ParentWindow.Handle)
		{
			ParentWindow.mIsFocusComeFromImap = true;
		}
		if (mDictTabs.ContainsKey(key))
		{
			if (FeatureManager.Instance.IsCustomUIForDMM && (int)((UIElement)ParentWindow.mFrontendGrid).Visibility != 0)
			{
				((UIElement)ParentWindow.mFrontendGrid).Visibility = (Visibility)0;
				((UIElement)ParentWindow.mDmmProgressControl).Visibility = (Visibility)1;
			}
			AppTabButton appTabButton = mDictTabs[key];
			if (!appTabButton.IsSelected)
			{
				appTabButton.IsLaunchOnSelection = isLaunch;
				if (KMManager.sGuidanceWindow != null && GuidanceWindow.sIsDirty)
				{
					appTabButton.mIsAnyOperationPendingForTab = true;
				}
				else
				{
					appTabButton.mIsAnyOperationPendingForTab = false;
				}
				appTabButton.Select(value: true, receivedFromImap);
				result = true;
				appTabButton.EventOnTabChanged?.Invoke(null, new TabChangeEventArgs(appTabButton.AppName, appTabButton.PackageName, appTabButton.mTabType));
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	internal bool GoToTab(int index)
	{
		if (mDictTabs.Count > index)
		{
			return GoToTab(((IEnumerable)((Panel)mPanel).Children).OfType<AppTabButton>().Last().TabKey);
		}
		return false;
	}

	internal AppTabButton GetTab(string packageName)
	{
		if (mDictTabs.ContainsKey(packageName))
		{
			return mDictTabs[packageName];
		}
		return null;
	}

	private void MoreTabButton_Click(object sender, RoutedEventArgs e)
	{
		((Popup)mPopup).IsOpen = true;
	}

	private void NotificationPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mMoreTabButton).IsEnabled = false;
	}

	private void NotificationPopup_Closed(object sender, EventArgs e)
	{
		((UIElement)mMoreTabButton).IsEnabled = true;
	}

	private void NotificaitonPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			Thread.Sleep(100);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((Popup)mPopup).IsOpen = false;
			}, new object[0]);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal void EnableAppTabs(bool isEnableTab)
	{
		foreach (KeyValuePair<string, AppTabButton> mDictTab in mDictTabs)
		{
			if (mDictTab.Value.mTabType == TabType.AppTab)
			{
				((UIElement)mDictTab.Value).IsEnabled = isEnableTab;
			}
		}
	}

	internal bool IsAppRunning()
	{
		foreach (KeyValuePair<string, AppTabButton> mDictTab in mDictTabs)
		{
			if (mDictTab.Value.mTabType == TabType.AppTab)
			{
				return true;
			}
		}
		return false;
	}

	internal void RestartTab(string package)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			CloseTab(package, sendStopAppToAndroid: true, forceClose: true, dontCheckQuitPopup: true);
		}, new object[0]);
		Thread.Sleep(1000);
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, isCheckForGrm: false);
		}, new object[0]);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/apptabbuttons.xaml", UriKind.Relative);
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
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mPanel = (StackPanel)target;
			break;
		case 2:
			mMoreTabButton = (AppTabButton)target;
			break;
		case 3:
			mPopup = (CustomPopUp)target;
			break;
		case 4:
			mMaskBorder = (Border)target;
			break;
		case 5:
			mHiddenButtons = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
