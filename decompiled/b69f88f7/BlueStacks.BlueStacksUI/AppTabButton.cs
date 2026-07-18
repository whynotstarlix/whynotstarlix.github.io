using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class AppTabButton : Button, IComponentConnector
{
	private MainWindow mMainWindow;

	internal const int IconModeMinWidth = 38;

	internal const int ParallelogramModeMinWidth = 48;

	private bool mIsPortraitModeTab;

	internal TabType mTabType;

	internal bool mRestartPubgTab;

	internal bool mRestartCODTab;

	internal bool mIsKeyMappingTipDisplayed;

	internal bool mIsOverlayTooltipDisplayed;

	internal bool mIsShootingModeToastDisplayed;

	internal bool mShootingModeToastIsOpen;

	internal bool mGuidanceWindowOpen;

	internal bool mShootingModeToastWhenGuidanceOpen;

	internal bool mIsAnyOperationPendingForTab;

	internal bool mIsSwitchedBackFromHomeTab;

	internal bool mIsNativeGamepadEnabledForApp;

	private bool mIsMoreTabsButton;

	private bool mIsDMMKeyMapEnabled;

	private string mActivityName;

	private bool mIsTabsSkewed;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ParallelogramGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mImageColumn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppTabIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mTabLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseTabButtonPortrait;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseTabButtonLandScape;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseTabButtonDropDown;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDownArrowGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path Arrow;

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

	public EventHandler<TabChangeEventArgs> EventOnTabChanged { get; set; }

	internal bool IsSelectedSchemeStatSent { get; set; }

	internal bool IsCursorClipped { get; set; }

	internal GameOnboardingControl OnboardingControl { get; set; }

	public bool IsPortraitModeTab
	{
		get
		{
			return mIsPortraitModeTab;
		}
		set
		{
			mIsPortraitModeTab = value;
			if (IsSelected && ParentWindow.IsUIInPortraitMode != mIsPortraitModeTab)
			{
				ParentWindow.SwitchToPortraitMode(mIsPortraitModeTab);
			}
		}
	}

	public bool IsMoreTabsButton
	{
		get
		{
			return mIsMoreTabsButton;
		}
		set
		{
			mIsMoreTabsButton = value;
			((UIElement)mAppTabIcon).IsEnabled = false;
		}
	}

	public bool IsButtonInDropDown { get; set; }

	public bool IsSelected { get; private set; }

	public bool IsShootingModeTooltipEnabled { get; set; }

	public string PackageName { get; set; }

	public string TabKey { get; set; }

	public string AppName { get; set; }

	public bool IsDMMKeymapEnabled
	{
		get
		{
			return mIsDMMKeyMapEnabled;
		}
		set
		{
			mIsDMMKeyMapEnabled = value;
			IsDMMKeymapUIVisible = value;
			ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
		}
	}

	public bool IsDMMKeymapUIVisible { get; set; }

	public string AppLabel => ((ContentControl)mTabLabel).Content.ToString();

	public bool IsLaunchOnSelection { get; set; }

	public Grid mControlGrid { get; set; }

	internal void Select(bool value, bool receivedFromImap = false)
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Invalid comparison between Unknown and I4
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Invalid comparison between Unknown and I4
		if (ParentWindow.StaticComponents.mSelectedTabButton == this && value)
		{
			return;
		}
		if (ParentWindow.StaticComponents.mSelectedTabButton != null)
		{
			if (!string.Equals(KMManager.sPackageName, PackageName, StringComparison.InvariantCulture))
			{
				KMManager.CloseWindows();
				if (KMManager.sGuidanceWindow != null)
				{
					return;
				}
			}
			AppTabButton mSelectedTabButton = ParentWindow.StaticComponents.mSelectedTabButton;
			if (mSelectedTabButton.mTabType == TabType.HomeTab)
			{
				mIsSwitchedBackFromHomeTab = true;
			}
			else
			{
				mIsSwitchedBackFromHomeTab = false;
			}
			ParentWindow.StaticComponents.mSelectedTabButton = null;
			mSelectedTabButton.Select(value: false);
			if (mSelectedTabButton.IsCursorClipped && mTabType == TabType.AppTab)
			{
				IsCursorClipped = true;
			}
			mSelectedTabButton.IsCursorClipped = false;
			ParentWindow.StaticComponents.mPreviousSelectedTabWeb = mSelectedTabButton.mTabType == TabType.WebTab;
		}
		ParentWindow.ToggleFullScreenToastVisibility(isFullScreen: false);
		IsSelected = value;
		AnimateTabSelection(value);
		if (IsSelected)
		{
			Publisher.PublishMessage((BrowserControlTags)4, ParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)PackageName)));
			ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.RemoveAll((string n) => n.Equals(TabKey, StringComparison.OrdinalIgnoreCase));
			ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.Add(TabKey);
			ParentWindow.StaticComponents.mSelectedTabButton = this;
			ParentWindow.Utils.ResetPendingUIOperations();
			ParentWindow.ShowControlGrid(mControlGrid);
			if (mTabType == TabType.AppTab || mTabType == TabType.HomeTab)
			{
				if (ParentWindow.AppForcedOrientationDict.ContainsKey(PackageName))
				{
					ParentWindow.ChangeOrientationFromClient(ParentWindow.AppForcedOrientationDict[PackageName]);
				}
				else
				{
					ParentWindow.ChangeOrientationFromClient(isPortrait: false, stopFurtherOrientation: false);
				}
			}
			if (mTabType == TabType.HomeTab)
			{
				ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(isHomeTabSelected: true);
				if (ParentWindow.mIsFullScreen)
				{
					ParentWindow.RestoreWindows();
				}
			}
			else if (!FeatureManager.Instance.IsCustomUIForDMM && mTabType == TabType.AppTab && !ParentWindow.mSidebar.mIsOverlayTooltipClosed && !mIsOverlayTooltipDisplayed && KMManager.KeyMappingFilesAvailable(PackageName))
			{
				mIsOverlayTooltipDisplayed = true;
				ParentWindow.mSidebar.ShowOverlayTooltip(isShow: true);
			}
			if (mTabType != TabType.HomeTab)
			{
				ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(isHomeTabSelected: false);
			}
			AppUsageTimer.StartTimer(ParentWindow.mVmName, TabKey);
			if (IsLaunchOnSelection)
			{
				LaunchApp();
			}
			else
			{
				IsLaunchOnSelection = true;
			}
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabLabel, Control.ForegroundProperty, "SelectedTabForegroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				if (mTabType == TabType.AppTab)
				{
					ParentWindow.mTopBar.mAppTabButtons.KillWebTabs();
					if (ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(PackageName)?.IsGamepadCompatible == true)
					{
						ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(visiblity: true);
					}
					else
					{
						ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(visiblity: false);
					}
					KMManager.LoadIMActions(ParentWindow, PackageName);
					ParentWindow.mCallbackEnabled = "False";
					Logger.Info("Callback: Select(): " + ParentWindow.mCallbackEnabled);
					KMManager.mOnboardingCounter = 1;
					if (!IsSelectedSchemeStatSent)
					{
						ClientStats.SendMiscellaneousStatsAsync("SelectedSchemeName", RegistryManager.Instance.UserGuid, PackageName, ParentWindow.SelectedConfig.SelectedControlScheme.Name, null, null);
						IsSelectedSchemeStatSent = true;
					}
					if (mTabType == TabType.AppTab && !mIsKeyMappingTipDisplayed && !ParentWindow.SendClientActions && !receivedFromImap && KMManager.KeyMappingFilesAvailable(PackageName))
					{
						mIsKeyMappingTipDisplayed = true;
						if (ParentWindow.SelectedConfig != null && ParentWindow.SelectedConfig.SelectedControlScheme != null && ParentWindow.SelectedConfig.SelectedControlScheme.GameControls != null && ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any((IMAction action) => action.Guidance.Any()))
						{
							ParentWindow.mSidebar.ShowKeyMapPopup(isShow: true);
						}
					}
					else if (mGuidanceWindowOpen)
					{
						KMManager.HandleInputMapperWindow(ParentWindow);
					}
					if (RegistryManager.Instance.ShowKeyControlsOverlay && !KMManager.CheckIfKeymappingWindowVisible())
					{
						KMManager.ShowOverlayWindow(ParentWindow, isShow: true, isreload: true);
					}
					if (mIsSwitchedBackFromHomeTab && KMManager.KeyMappingFilesAvailable(PackageName))
					{
						ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleLoadConfigOnTabSwitch", new Dictionary<string, string> { { "package", PackageName } });
					}
					ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
					ParentWindow.mCommonHandler.SetCustomCursorForApp(PackageName);
					mIsNativeGamepadEnabledForApp = (int)ParentWindow.EngineInstanceRegistry.NativeGamepadState != 1;
					if ((int)ParentWindow.EngineInstanceRegistry.NativeGamepadState == 2)
					{
						bool flag = (mIsNativeGamepadEnabledForApp = ParentWindow.mCommonHandler.CheckNativeGamepadState(PackageName));
						ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string> { 
						{
							"isEnabled",
							flag.ToString(CultureInfo.InvariantCulture)
						} });
					}
				}
				else
				{
					KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
					if (ParentWindow.mCommonHandler != null)
					{
						ParentWindow.mCommonHandler.ClipMouseCursorHandler(forceDisable: true);
					}
				}
				if (mTabType == TabType.HomeTab)
				{
					ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
				}
				else
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
				}
				List<GenericNotificationItem> list = new List<GenericNotificationItem>();
				foreach (GenericNotificationItem item in PromotionManager.sPassedDeferredNotificationsList.Where((GenericNotificationItem _) => string.Compare(_.DeferredApp, PackageName, StringComparison.OrdinalIgnoreCase) == 0))
				{
					BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(item);
					GenericNotificationManager.AddNewNotification(item);
					BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton();
					list.Add(item);
				}
				foreach (GenericNotificationItem item2 in list)
				{
					PromotionManager.sPassedDeferredNotificationsList.Remove(item2);
				}
				if (ParentWindow.SendClientActions && !receivedFromImap)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>
					{
						{ "EventAction", "TabSelected" },
						{ "tabKey", TabKey }
					};
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = (Formatting)0;
					dictionary.Add("operationData", JsonConvert.SerializeObject((object)dictionary2, serializerSettings));
					ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
				}
			}
			if (mTabType == TabType.AppTab && KMManager.KeyMappingFilesAvailable(PackageName) && ParentWindow.SelectedConfig.ControlSchemes != null && ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
			{
				ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(visiblity: true);
			}
			else
			{
				ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(visiblity: false);
			}
			ParentWindow.mTopBar.mAppTabButtons.EventOnTabChanged?.Invoke(null, null);
			if (mTabType == TabType.AppTab && File.Exists(Utils.GetInputmapperDefaultFilePath(PackageName)) && Oem.Instance.IsShowGameBlurb)
			{
				bool? flag2 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.OnBoardingInfo.OnBoardingAppPackages?.IsPackageAvailable(PackageName);
				bool? flag3 = flag2;
				if (flag3 == true && !AppConfigurationManager.Instance.CheckIfTrueInAnyVm(PackageName, (Predicate<AppSettings>)((AppSettings appSetting) => appSetting.IsAppOnboardingCompleted)))
				{
					OnboardingControl = new GameOnboardingControl(ParentWindow, PackageName, "applaunch");
					KMManager.sGuidanceWindow?.DimOverLayVisibility((Visibility)0);
					ParentWindow.ShowDimOverlay(OnboardingControl);
				}
				else
				{
					ParentWindow.ShowDimOverlay();
					if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>)((AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted)))
					{
						ShowDefaultBlurbOnboarding();
					}
					ParentWindow.HideDimOverlay();
				}
			}
			if ((int)((UIElement)ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton).Visibility == 0)
			{
				MoreTabsButtonHandling();
			}
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabLabel, Control.ForegroundProperty, "TabForegroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
		}
	}

	private void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
	{
		Logger.Info("Restarting Pubg/COD Tab.");
		Thread thread = new Thread((ThreadStart)delegate
		{
			ParentWindow.mTopBar.mAppTabButtons.RestartTab(PackageName);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public AppTabButton()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		IsShootingModeTooltipEnabled = true;
		PackageName = string.Empty;
		AppName = string.Empty;
		mActivityName = string.Empty;
		mIsTabsSkewed = true;
		InitializeComponent();
		UpdateTabAppearance(isSelected: false);
		mImageColumn.Width = new GridLength(0.0);
	}

	internal void Init(string appName, string packageName, string activityName, string imageName, Grid controlGrid, string tabKey)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(tabKey))
		{
			flag = true;
		}
		Init(appName, packageName, imageName, controlGrid, flag ? tabKey : packageName);
		mActivityName = activityName;
		mTabType = TabType.AppTab;
		if (string.Equals(packageName, "Home", StringComparison.InvariantCulture) || string.Equals(packageName, "Setup", StringComparison.InvariantCulture))
		{
			mTabType = TabType.HomeTab;
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)this, FrameworkElement.MarginProperty, "TabMarginLandScape");
		}
	}

	internal void Init(string title, string url, string imageName, Grid controlGrid, string tabKey)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		BlueStacksUIBinding.Bind((DependencyObject)(object)this, title, FrameworkElement.ToolTipProperty);
		BlueStacksUIBinding.Bind(mTabLabel, title);
		AppName = title;
		PackageName = url;
		TabKey = tabKey;
		mTabType = TabType.WebTab;
		mControlGrid = controlGrid;
		if (!IsSelected)
		{
			((UIElement)mControlGrid).Visibility = (Visibility)1;
		}
		if (string.IsNullOrEmpty(imageName))
		{
			mImageColumn.Width = new GridLength(0.0);
		}
		else
		{
			mAppTabIcon.ImageName = imageName;
		}
	}

	internal void ResizeButton(double tabWidth)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		if (ParentWindow.IsUIInPortraitMode)
		{
			MakeTabParallelogram(isSkewTab: false);
		}
		else
		{
			MakeTabParallelogram(isSkewTab: true);
		}
		if (tabWidth != ((FrameworkElement)this).ActualWidth)
		{
			DoubleAnimation val = new DoubleAnimation
			{
				From = ((FrameworkElement)this).ActualWidth,
				To = tabWidth,
				Duration = new Duration(TimeSpan.FromMilliseconds(200.0))
			};
			((UIElement)this).BeginAnimation(FrameworkElement.WidthProperty, (AnimationTimeline)(object)val);
		}
	}

	internal void MakeTabParallelogram(bool isSkewTab)
	{
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		if (isSkewTab)
		{
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)this, FrameworkElement.MarginProperty, "TabMarginLandScape");
		}
		else
		{
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)this, FrameworkElement.MarginProperty, "TabMarginPortrait");
		}
		if (isSkewTab != mIsTabsSkewed)
		{
			if (isSkewTab)
			{
				mIsTabsSkewed = true;
				((UIElement)CloseTabButtonPortrait).Visibility = (Visibility)1;
				((UIElement)ParallelogramGrid).RenderTransform = (Transform)new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
				DoubleAnimation val = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, Duration.op_Implicit(TimeSpan.FromMilliseconds(200.0)));
				DoubleAnimation val2 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, Duration.op_Implicit(TimeSpan.FromMilliseconds(200.0)));
				((Timeline)val2).Completed += SkewY_Completed;
				((Animatable)((UIElement)ParallelogramGrid).RenderTransform).BeginAnimation(SkewTransform.AngleXProperty, (AnimationTimeline)(object)val);
				((Animatable)((UIElement)ParallelogramGrid).RenderTransform).BeginAnimation(SkewTransform.AngleYProperty, (AnimationTimeline)(object)val2);
				BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)this, FrameworkElement.MarginProperty, "TabMarginLandScape");
			}
			else
			{
				mIsTabsSkewed = false;
				((UIElement)CloseTabButtonLandScape).Visibility = (Visibility)1;
				((UIElement)ParallelogramGrid).RenderTransform = (Transform)new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
				DoubleAnimation val3 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, Duration.op_Implicit(TimeSpan.FromMilliseconds(200.0)));
				DoubleAnimation val4 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, Duration.op_Implicit(TimeSpan.FromMilliseconds(200.0)));
				((Animatable)((UIElement)ParallelogramGrid).RenderTransform).BeginAnimation(SkewTransform.AngleXProperty, (AnimationTimeline)(object)val3);
				((Animatable)((UIElement)ParallelogramGrid).RenderTransform).BeginAnimation(SkewTransform.AngleYProperty, (AnimationTimeline)(object)val4);
				((Timeline)val4).Completed += SkewY_Completed;
				BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)this, FrameworkElement.MarginProperty, "TabMarginPortrait");
			}
			if (mIsMoreTabsButton)
			{
				((UIElement)mBorder).Visibility = (Visibility)0;
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			}
		}
	}

	private void SkewY_Completed(object sender, EventArgs e)
	{
		if (mIsTabsSkewed)
		{
			BlueStacksUIBinding.BindTransform((DependencyObject)(object)ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransform");
		}
		else
		{
			BlueStacksUIBinding.BindTransform((DependencyObject)(object)ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransformPortrait");
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		if (IsMoreTabsButton)
		{
			return;
		}
		bool num = sender.GetHashCode() == ((object)ParentWindow.StaticComponents.mSelectedTabButton).GetHashCode();
		bool flag = ((UIElement)CloseTabButtonLandScape).IsMouseOver || ((UIElement)CloseTabButtonPortrait).IsMouseOver || ((UIElement)CloseTabButtonDropDown).IsMouseOver;
		if (num)
		{
			if (flag)
			{
				if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed)
				{
					HandlePendingOperationsForTab("guidance");
				}
				if (KMManager.sGuidanceWindow == null)
				{
					ParentWindow.mTopBar.mAppTabButtons.CloseTab(TabKey, sendStopAppToAndroid: true);
				}
			}
		}
		else if (flag)
		{
			ParentWindow.mTopBar.mAppTabButtons.CloseTab(TabKey, sendStopAppToAndroid: true);
		}
		else
		{
			if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed)
			{
				HandlePendingOperationsForTab("guidance");
			}
			if (KMManager.sGuidanceWindow == null)
			{
				Select(value: true);
				Button_PreviewMouseUp(null, null);
				EventOnTabChanged?.Invoke(this, new TabChangeEventArgs(AppName, PackageName, mTabType));
			}
		}
	}

	private void HandlePendingOperationsForTab(string pendingOperation)
	{
		if (pendingOperation != null && pendingOperation == "guidance")
		{
			KMManager.CloseWindows();
		}
	}

	internal void UpdateUIForDropDown(bool isInDropDown)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (isInDropDown)
		{
			IsButtonInDropDown = true;
			MakeTabParallelogram(isSkewTab: false);
			((FrameworkElement)this).MinWidth = 150.0;
			((FrameworkElement)mTabLabel).Margin = new Thickness(3.0, 1.0, 3.0, 1.0);
			if (!IsSelected)
			{
				mBorder.Background = (Brush)(object)Brushes.Transparent;
			}
			mBorder.BorderThickness = new Thickness(0.0);
			return;
		}
		IsButtonInDropDown = false;
		mBorder.BorderThickness = new Thickness(1.0);
		((FrameworkElement)this).MinWidth = 0.0;
		((FrameworkElement)mTabLabel).Margin = new Thickness(3.0, 1.0, 24.0, 1.0);
		if (IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
		}
	}

	internal void LaunchApp()
	{
		if (!string.IsNullOrEmpty(PackageName) && mTabType == TabType.AppTab)
		{
			ParentWindow.mAppHandler.SendRunAppRequestAsync(PackageName, mActivityName);
		}
		else if ((mTabType == TabType.HomeTab || mTabType == TabType.WebTab) && RegistryManager.Instance.SwitchToAndroidHome)
		{
			ParentWindow.mAppHandler.GoHome();
		}
	}

	private void Button_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
		}
		if (IsButtonInDropDown)
		{
			if (mTabType != TabType.HomeTab)
			{
				((UIElement)CloseTabButtonDropDown).Visibility = (Visibility)0;
			}
		}
		else if (mTabType != TabType.HomeTab && !mIsMoreTabsButton)
		{
			((UIElement)CloseTabButtonLandScape).Visibility = (Visibility)0;
			if (!mIsTabsSkewed)
			{
				((UIElement)CloseTabButtonPortrait).Visibility = (Visibility)0;
			}
		}
		if (IsMoreTabsButton)
		{
			mAppTabIcon.SetHoverImage();
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
		}
	}

	private void Button_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
		}
		if (IsMoreTabsButton)
		{
			mAppTabIcon.SetDefaultImage();
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
		}
		((UIElement)CloseTabButtonLandScape).Visibility = (Visibility)1;
		((UIElement)CloseTabButtonPortrait).Visibility = (Visibility)1;
		((UIElement)CloseTabButtonDropDown).Visibility = (Visibility)1;
	}

	private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!IsButtonInDropDown)
		{
			if (IsMoreTabsButton)
			{
				mAppTabIcon.SetClickedImage();
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
			}
			else if (!IsSelected)
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
			}
		}
	}

	private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		if (IsButtonInDropDown)
		{
			return;
		}
		if (!IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
		}
		if (mIsMoreTabsButton)
		{
			if (((UIElement)this).IsMouseOver)
			{
				mAppTabIcon.SetHoverImage();
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
			}
			else
			{
				mAppTabIcon.SetDefaultImage();
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			}
		}
	}

	private void Button_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		if (((UIElement)this).IsEnabled)
		{
			((UIElement)this).Opacity = 1.0;
		}
		else
		{
			((UIElement)this).Opacity = 0.3;
		}
	}

	internal BrowserControl GetBrowserControl()
	{
		try
		{
			return ((Panel)mControlGrid).Children[0] as BrowserControl;
		}
		catch (Exception ex)
		{
			Logger.Warning("No BrowserControl associated with tabkey: " + TabKey + " Error: " + ex.ToString());
			return null;
		}
	}

	internal void EnableKeymapForDMM(bool enable)
	{
		mIsDMMKeyMapEnabled = enable;
	}

	internal void MoreTabsButtonHandling()
	{
		AppTabButton mMoreTabButton = ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton;
		((UIElement)mMoreTabButton.mTabLabel).Visibility = (Visibility)2;
		((UIElement)mMoreTabButton.mDownArrowGrid).Visibility = (Visibility)0;
		if (((Panel)ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons).Children.Contains((UIElement)(object)ParentWindow.StaticComponents.mSelectedTabButton))
		{
			mMoreTabButton.mAppTabIcon.ImageName = ParentWindow.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName;
		}
		else
		{
			mMoreTabButton.mAppTabIcon.ImageName = (((Panel)ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons).Children[0] as AppTabButton).mAppTabIcon.ImageName;
		}
	}

	internal void ShowBlurbOnboarding(JObject res)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected I4, but got Unknown
		if (!res["is_show_blurbs"].ToObject<bool>() || AppConfigurationManager.Instance.CheckIfTrueInAnyVm(PackageName, (Predicate<AppSettings>)((AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted)))
		{
			return;
		}
		KMManager.sGuidanceWindow?.DimOverLayVisibility((Visibility)2);
		if (res.ContainsKey("blurbs"))
		{
			JArray val = JArray.Parse(((object)res["blurbs"]).ToString());
			for (int num = 0; num < ((JContainer)val).Count; num++)
			{
				JObject val2 = JObject.Parse(((object)val[num]).ToString());
				if (!Enum.IsDefined(typeof(OnboardingBlurbTags), ((object)val2["tag"]).ToString()))
				{
					continue;
				}
				OnboardingBlurbTags val3 = (OnboardingBlurbTags)Enum.Parse(typeof(OnboardingBlurbTags), ((object)val2["tag"]).ToString());
				switch ((int)val3)
				{
				case 0:
				{
					IMConfig selectedConfig = ParentWindow.SelectedConfig;
					if (selectedConfig != null && selectedConfig.ControlSchemesDict?.Count > 1)
					{
						OnBoardingPopupWindow onBoardingPopupWindow4 = KMManager.sGuidanceWindow?.GuidanceSchemeOnboardingBlurb();
						if (onBoardingPopupWindow4 != null)
						{
							KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow4);
						}
					}
					break;
				}
				case 1:
				{
					OnBoardingPopupWindow onBoardingPopupWindow = KMManager.sGuidanceWindow?.GuidanceOnboardingBlurb();
					if (onBoardingPopupWindow != null)
					{
						KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow);
					}
					break;
				}
				case 2:
				{
					OnBoardingPopupWindow onBoardingPopupWindow3 = ParentWindow.mSidebar?.FullscreenOnboardingBlurb();
					if (onBoardingPopupWindow3 != null)
					{
						KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow3);
					}
					break;
				}
				case 3:
				{
					OnBoardingPopupWindow onBoardingPopupWindow2 = KMManager.sGuidanceWindow?.GuidanceVideoOnboardingBlurb();
					if (onBoardingPopupWindow2 != null)
					{
						KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow2);
					}
					break;
				}
				case 4:
					if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
					{
						AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsCloseGuidanceOnboardingCompleted = false;
					}
					break;
				}
			}
			StartBlurbOnboarding();
			return;
		}
		IMConfig selectedConfig2 = ParentWindow.SelectedConfig;
		if (selectedConfig2 != null && selectedConfig2.ControlSchemesDict?.Count > 1)
		{
			OnBoardingPopupWindow onBoardingPopupWindow5 = KMManager.sGuidanceWindow?.GuidanceSchemeOnboardingBlurb();
			if (onBoardingPopupWindow5 != null)
			{
				KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow5);
			}
		}
		ShowDefaultBlurbOnboarding();
	}

	internal void ShowDefaultBlurbOnboarding()
	{
		OnBoardingPopupWindow onBoardingPopupWindow = KMManager.sGuidanceWindow?.GuidanceVideoOnboardingBlurb();
		if (onBoardingPopupWindow != null)
		{
			KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow);
		}
		OnBoardingPopupWindow onBoardingPopupWindow2 = KMManager.sGuidanceWindow?.GuidanceOnboardingBlurb();
		if (onBoardingPopupWindow2 != null)
		{
			KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow2);
		}
		OnBoardingPopupWindow onBoardingPopupWindow3 = ParentWindow.mSidebar?.FullscreenOnboardingBlurb();
		if (onBoardingPopupWindow3 != null)
		{
			KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow3);
		}
		if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsCloseGuidanceOnboardingCompleted = false;
		}
		StartBlurbOnboarding();
	}

	internal void StartBlurbOnboarding()
	{
		foreach (OnBoardingPopupWindow item in KMManager.onBoardingPopupWindows.ToList())
		{
			KMManager.onBoardingPopupWindows.Remove(item);
			if (!item.IsBlurbRelatedToGuidance)
			{
				ParentWindow.HideDimOverlay();
			}
			if (KMManager.onBoardingPopupWindows.Count == 0)
			{
				item.IsLastPopup = true;
			}
			((Window)item).ShowDialog();
		}
		if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsGeneralAppOnBoardingCompleted = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/apptabbutton.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((ButtonBase)(AppTabButton)target).Click += new RoutedEventHandler(Button_Click);
			((UIElement)(AppTabButton)target).MouseEnter += new MouseEventHandler(Button_MouseEnter);
			((UIElement)(AppTabButton)target).MouseLeave += new MouseEventHandler(Button_MouseLeave);
			((UIElement)(AppTabButton)target).PreviewMouseDown += new MouseButtonEventHandler(Button_PreviewMouseDown);
			((UIElement)(AppTabButton)target).IsEnabledChanged += new DependencyPropertyChangedEventHandler(Button_IsEnabledChanged);
			break;
		case 2:
			ParallelogramGrid = (Grid)target;
			break;
		case 3:
			mBorder = (Border)target;
			((FrameworkElement)mBorder).Height = 40.0;
			break;
		case 4:
			mImageColumn = (ColumnDefinition)target;
			break;
		case 5:
			mAppTabIcon = (CustomPictureBox)target;
			((UIElement)mAppTabIcon).Opacity = 0.0;
			break;
		case 6:
			mTabLabel = (Label)target;
			((FrameworkElement)mTabLabel).Margin = new Thickness(0.0, 0.0, 0.0, 15.0);
			break;
		case 7:
			CloseTabButtonPortrait = (CustomPictureBox)target;
			break;
		case 8:
			CloseTabButtonLandScape = (CustomPictureBox)target;
			break;
		case 9:
			CloseTabButtonDropDown = (CustomPictureBox)target;
			break;
		case 10:
			mDownArrowGrid = (Grid)target;
			break;
		case 11:
			Arrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	internal void AnimateTabSelection(bool isSelected)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_01c0: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		if (mTabLabel != null && mBorder != null)
		{
			DoubleAnimation val = new DoubleAnimation
			{
				From = (isSelected ? 0.5 : 1.0),
				To = (isSelected ? 1.0 : 0.6),
				Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)),
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			((UIElement)mTabLabel).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
			if (!(((UIElement)mTabLabel).RenderTransform is ScaleTransform))
			{
				ScaleTransform renderTransform = new ScaleTransform(1.0, 1.0);
				((UIElement)mTabLabel).RenderTransformOrigin = new Point(0.5, 0.5);
				((UIElement)mTabLabel).RenderTransform = (Transform)(object)renderTransform;
			}
			new DoubleAnimation
			{
				From = (isSelected ? 1.0 : 1.1),
				To = (isSelected ? 1.1 : 1.0),
				Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)),
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			ColorAnimation val2 = new ColorAnimation
			{
				To = (isSelected ? Color.FromRgb((byte)55, (byte)55, (byte)55) : Color.FromRgb((byte)111, (byte)111, (byte)111)),
				Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(400.0)),
				AutoReverse = false,
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			Brush borderBrush = mBorder.BorderBrush;
			SolidColorBrush val3 = (SolidColorBrush)(object)((borderBrush is SolidColorBrush) ? borderBrush : null);
			if (val3 == null)
			{
				val3 = new SolidColorBrush(Colors.Transparent);
				mBorder.BorderBrush = (Brush)(object)val3;
			}
			((Animatable)val3).BeginAnimation(SolidColorBrush.ColorProperty, (AnimationTimeline)(object)val2);
		}
	}

	private void UpdateTabAppearance(bool isSelected)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius cornerRadius = default(CornerRadius);
		((CornerRadius)(ref cornerRadius))._002Ector(8.0);
		mBorder.CornerRadius = cornerRadius;
		mBorder.BorderThickness = new Thickness(1.0);
		((FrameworkElement)mBorder).Height = 34.0;
		((FrameworkElement)mBorder).VerticalAlignment = (VerticalAlignment)1;
		((FrameworkElement)mBorder).Margin = new Thickness(0.0);
		string text = (isSelected ? "SelectedTabBackgroundColor" : (((UIElement)this).IsMouseOver ? "TabBackgroundHoverColor" : "TabBackgroundColor"));
		string text2 = (isSelected ? "SelectedTabBorderColor" : "AppTabBorderBrush");
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Panel.BackgroundProperty, text);
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Border.BorderBrushProperty, text2);
	}
}
