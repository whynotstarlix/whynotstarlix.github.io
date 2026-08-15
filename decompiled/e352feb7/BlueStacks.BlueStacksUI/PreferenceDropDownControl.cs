using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using BlueStacks.BlueStacksUI.Controls;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PreferenceDropDownControl : UserControl, IComponentConnector, IStyleConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid EngineSettingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mEngineSettingsButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Ellipse mSettingsBtnNotification;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPinToTopGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPinToTopImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPinToTopToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mStreamingMode;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStreamingModeImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStreaminModeToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMultiInstanceSectionTag;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Separator mMultiInstanceSectionBorderLine;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMultiInstanceSection;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSyncGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSyncOperationsImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mAutoAlignGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAutoAlignImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mUpgradeBluestacksStatus;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mUpdateImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mUpgradeBluestacksStatusTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mUpdateDownloadProgressPercentage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mUpgradeToFullBlueStacks;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mUpgradeToFullTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mLogoutButtonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCustomiseSectionTag;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Separator mCustomiseSectionBorderLine;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCustomiseSection;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChangeSkinGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mChangeSkinImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChangeWallpaperGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mChangeWallpaperImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHelpandsupportSectionTag;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Separator mHelpAndSupportSectionBorderLine;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHelpandsupportSection;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ReportProblemGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHelpCenterGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHelpCenterImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSpeedUpBstGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSpeedUpBstImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mWallpaperPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mWallpaperPopupGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGridForSize;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mWallpaperPopupBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mBodyText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path RightArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mChooseWallpaperPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChooseWallpaperPopupGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGridForSize2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mPopupGridBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChooseNewGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSetDefaultGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRestoreDefaultText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mRightArrow;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; set; }

	private event EventHandler LogoutConfirmationResetAccountAcceptedHandler;

	private event EventHandler RestoreDefaultConfirmationClicked;

	public PreferenceDropDownControl()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		((UserControl)this)._002Ector();
		InitializeComponent();
		LogoutConfirmationResetAccountAcceptedHandler += PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler;
		RestoreDefaultConfirmationClicked += PreferenceDropDownControl_RestoreDefaultConfirmationClicked;
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			((UIElement)mSpeedUpBstGrid).Visibility = (Visibility)2;
			((UIElement)mUpgradeToFullBlueStacks).Visibility = (Visibility)0;
		}
		if (!FeatureManager.Instance.IsShowSpeedUpTips)
		{
			((UIElement)mSpeedUpBstGrid).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsShowHelpCenter)
		{
			((UIElement)mHelpCenterGrid).Visibility = (Visibility)2;
		}
	}

	private void PreferenceDropDownControl_RestoreDefaultConfirmationClicked(object sender, EventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "RestoreDefaultWallpaper", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium");
		((Popup)mChooseWallpaperPopup).IsOpen = false;
		ParentWindow.Utils.RestoreWallpaperImageForAllVms();
	}

	internal void Init(MainWindow parentWindow)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		ParentWindow = parentWindow;
		if (Oem.Instance.IsRemoveAccountOnExit)
		{
			((UIElement)mLogoutButtonGrid).Visibility = (Visibility)0;
		}
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			mUpgradeToFullTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "").Replace(GameConfig.Instance.AppName, "BlueStacks");
		}
	}

	internal void LateInit()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		if (FeatureManager.Instance.ShowClientOnTopPreference)
		{
			if (ParentWindow.EngineInstanceRegistry.IsClientOnTop)
			{
				mPinToTopToggleButton.ImageName = mPinToTopToggleButton.ImageName.Replace("_off", "_on");
			}
			else
			{
				mPinToTopToggleButton.ImageName = mPinToTopToggleButton.ImageName.Replace("_on", "_off");
			}
		}
		else
		{
			Grid obj = mPinToTopGrid;
			int num = ((871872188 > 1966980290) ? 2 : 2);
			((UIElement)obj).Visibility = (Visibility)num;
		}
		if (FeatureManager.Instance.IsThemeEnabled && (int)RegistryManager.Instance.InstallationType != 1)
		{
			((UIElement)mChangeSkinGrid).Visibility = (Visibility)0;
		}
		if (ParentWindow != null && ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && !FeatureManager.Instance.IsWallpaperChangeDisabled && (int)RegistryManager.Instance.InstallationType != 1 && !FeatureManager.Instance.IsHtmlHome)
		{
			((UIElement)mChangeWallpaperGrid).Visibility = (Visibility)0;
		}
		((UIElement)mAutoAlignGrid).MouseLeftButtonUp += new MouseButtonEventHandler(AutoAlign_MouseLeftButtonUp);
		((UIElement)mAutoAlignGrid).Opacity = 1.0;
		if (!FeatureManager.Instance.IsOperationsSyncEnabled)
		{
			((UIElement)mSyncGrid).Visibility = (Visibility)(-1208688087 ^ -1208688085);
		}
		else if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName) && !ParentWindow.mIsSyncMaster)
		{
			((UIElement)mSyncGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SyncGrid_MouseLeftButtonUp);
			((UIElement)mSyncGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncGrid).Opacity = 0.5;
		}
		else
		{
			((UIElement)mSyncGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SyncGrid_MouseLeftButtonUp);
			((UIElement)mSyncGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SyncGrid_MouseLeftButtonUp);
			((UIElement)mSyncGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncGrid).Opacity = 1.0;
		}
		SectionsTagVisibilityToggling();
	}

	internal void SectionsTagVisibilityToggling()
	{
		((UIElement)mCustomiseSectionTag).Visibility = (Visibility)((!CheckSectionTagVisibility(mCustomiseSection)) ? (-2040671927 - -2040671929) : 0);
		((UIElement)mHelpandsupportSectionTag).Visibility = (Visibility)((!CheckSectionTagVisibility(mHelpandsupportSection)) ? (0x79DEFDFA ^ 0x79DEFDF8) : 0);
	}

	private static bool CheckSectionTagVisibility(Grid sectionGrid)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (UIElement child in ((Panel)sectionGrid).Children)
		{
			if ((int)((child is Grid) ? child : null).Visibility == 0)
			{
				return true;
			}
		}
		return false;
	}

	private void Grid_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void Grid_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
	}

	private void EngineSettingGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked settings button");
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		string tabName = string.Empty;
		if (ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
		{
			tabName = "STRING_GAME_SETTINGS";
		}
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		ParentWindow.mCommonHandler.LaunchSettingsWindow(tabName);
	}

	private void ReportProblemGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked report problem button");
		using (Process process = new Process())
		{
			process.StartInfo.Arguments = "-vmname:" + ParentWindow.mVmName;
			process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
			process.Start();
		}
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ReportProblem", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void LogoutButtonGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Clicked logout button");
		if (ParentWindow.mGuestBootCompleted)
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_LOGOUT_BLUESTACKS3", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_REMOVE_GOOGLE_ACCOUNT", "");
			val.AddButton((ButtonColors)0, "STRING_LOGOUT_BUTTON", LogoutConfirmationResetAccountAcceptedHandler, (string)null, false, (object)null);
			val.AddButton((ButtonColors)(-1156390407 ^ -1156390405), "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Logout", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
	}

	private void PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler(object sender, EventArgs e)
	{
		ParentWindow.mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync();
	}

	private void SpeedUpBstGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		Logger.Info("Clicked SpeedUp BlueStacks button");
		SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
		if ((int)ParentWindow.mTopBar.mSnailMode == 1)
		{
			((UIElement)speedUpBlueStacks.mEnableVt).Visibility = (Visibility)0;
		}
		((UIElement)speedUpBlueStacks.mUpgradeComputer).Visibility = (Visibility)0;
		((UIElement)speedUpBlueStacks.mPowerPlan).Visibility = (Visibility)0;
		((UIElement)speedUpBlueStacks.mConfigureAntivirus).Visibility = (Visibility)0;
		new ContainerWindow(ParentWindow, (UserControl)(object)speedUpBlueStacks, 640.0, 440.0);
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "SpeedUpBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void mHelpCenterGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "HelpCentre", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			BlueStacksUIUtils.OpenUrl(helpCenterUrl);
		}
		else
		{
			ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", isSwitch: true, "FEEDBACK_TEXT");
		}
	}

	private void mChangeSkinGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ChangeThemeWindow control = new ChangeThemeWindow(ParentWindow);
		int num = 0x1FC ^ 4;
		int num2 = ((631966773 > 323009514) ? 652 : 869);
		int num3 = num2;
		new ContainerWindow(ParentWindow, (UserControl)(object)control, num3, num);
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeSkin", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void NotificationPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)dummyGridForSize2).Visibility = (Visibility)0;
	}

	private void NotificationPopup_Closed(object sender, EventArgs e)
	{
		((Panel)mChangeWallpaperGrid).Background = (Brush)(object)Brushes.Transparent;
	}

	private void ChooseNewGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		if (RegistryManager.Instance.IsPremium)
		{
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium");
			ParentWindow.Utils.ChooseWallpaper();
			return;
		}
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "NonPremium");
		string text = "/bluestacks_account?extra=section:plans";
		string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + text);
		urlWithParams += "&email=";
		urlWithParams += RegistryManager.Instance.RegisteredEmail;
		urlWithParams += "&token=";
		urlWithParams += RegistryManager.Instance.Token;
		ParentWindow.mTopBar.mAppTabButtons.AddWebTab(urlWithParams, "STRING_ACCOUNT", "account_tab", isSwitch: true, "account_tab");
	}

	private void SetDefaultGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_LBL_RESTORE_DEFAULT", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_RESTORE_DEFAULT_WALLPAPER", "");
			val.AddButton((ButtonColors)0, "STRING_RESTORE_BUTTON", RestoreDefaultConfirmationClicked, (string)null, false, (object)null);
			val.AddButton((ButtonColors)(-595786475 ^ -595786473), "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
	}

	private void mChangeWallpaperGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			((UIElement)mChangeWallpaperGrid).MouseLeftButtonUp -= new MouseButtonEventHandler(ChooseNewGrid_MouseLeftButtonUp);
			((Popup)mWallpaperPopup).PlacementTarget = (UIElement)(object)mChooseNewGrid;
			((Popup)mChooseWallpaperPopup).IsOpen = false;
			((Popup)mChooseWallpaperPopup).IsOpen = true;
		}
		else
		{
			if (!RegistryManager.Instance.IsPremium)
			{
				((Popup)mWallpaperPopup).PlacementTarget = (UIElement)(object)mChangeWallpaperGrid;
				((Popup)mWallpaperPopup).IsOpen = false;
				((Popup)mWallpaperPopup).IsOpen = true;
			}
			((UIElement)mChangeWallpaperGrid).MouseLeftButtonUp -= new MouseButtonEventHandler(ChooseNewGrid_MouseLeftButtonUp);
			((UIElement)mChangeWallpaperGrid).MouseLeftButtonUp += new MouseButtonEventHandler(ChooseNewGrid_MouseLeftButtonUp);
		}
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void mChangeWallpaperGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
		if (!((UIElement)mChangeWallpaperGrid).IsMouseOver && !((UIElement)mChooseWallpaperPopupGrid).IsMouseOver && !((UIElement)mWallpaperPopupGrid).IsMouseOver)
		{
			((Popup)mChooseWallpaperPopup).IsOpen = false;
			((Popup)mWallpaperPopup).IsOpen = false;
		}
	}

	private void ChooseNewGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!RegistryManager.Instance.IsPremium)
		{
			((Popup)mWallpaperPopup).IsOpen = true;
		}
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void ChooseNewGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mChooseNewGrid).IsMouseOver && !((UIElement)mWallpaperPopupGrid).IsMouseOver)
		{
			((Popup)mWallpaperPopup).IsOpen = false;
		}
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
	}

	private void SetDefaultGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
		}
	}

	private void SetDefaultGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}
	}

	private void mWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mChooseNewGrid).IsMouseOver)
		{
			((Popup)mWallpaperPopup).IsOpen = false;
		}
	}

	private void mChooseWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mChangeWallpaperGrid).IsMouseOver && !((UIElement)mChooseWallpaperPopupGrid).IsMouseOver && !((UIElement)mWallpaperPopupGrid).IsMouseOver)
		{
			((Popup)mChooseWallpaperPopup).IsOpen = false;
			((Popup)mWallpaperPopup).IsOpen = false;
		}
	}

	private void mUpgradeToFullBlueStacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		string localizedString = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "");
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		object[] array = new object[-24338 + (797601469 >> 2084505903)];
		array[0] = LocaleStrings.GetLocalizedString("STRING_CONTINUING_WILL_UPGRADE_TO_STD_BST", "");
		array[1] = LocaleStrings.GetLocalizedString("STRING_LAUNCH_BLUESTACKS_FROM_DESK_SHORTCUT", "");
		string text = string.Format(invariantCulture, "{0} {1}", array);
		localizedString = localizedString.Replace(GameConfig.Instance.AppName, "BlueStacks");
		text = text.Replace(GameConfig.Instance.AppName, "BlueStacks");
		CustomMessageWindow val = new CustomMessageWindow();
		val.AddButton((ButtonColors)(-891939801 + (0x3429E1D4 | 0x2121EB4D)), "STRING_YES", (EventHandler)UpgradeToFullBstHandler, (string)null, false, (object)null);
		val.AddButton((ButtonColors)(-787720236 + 787720238 % 888558925), "STRING_NO", (EventHandler)null, (string)null, false, (object)null);
		BlueStacksUIBinding.Bind(val.TitleTextBlock, localizedString, "");
		BlueStacksUIBinding.Bind(val.BodyTextBlock, text, "");
		ParentWindow.ShowDimOverlay();
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
		ParentWindow.HideDimOverlay();
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "UpgradeBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void UpgradeToFullBstHandler(object sender, EventArgs e)
	{
		((UIElement)ParentWindow.mWelcomeTab.mBackground).Visibility = (Visibility)0;
		ParentWindow.ShowDimOverlayForUpgrade();
		using BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += MBWUpdateToFullVersion_DoWork;
		backgroundWorker.RunWorkerCompleted += MBWUpdateToFullVersion_RunWorkerCompleted;
		backgroundWorker.RunWorkerAsync();
	}

	private void MBWUpdateToFullVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler(null, null);
	}

	private void MBWUpdateToFullVersion_DoWork(object sender, DoWorkEventArgs e)
	{
		Utils.UpgradeToFullVersionAndCreateBstShortcut(true);
	}

	private void SyncGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		ParentWindow.ShowSynchronizerWindow();
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void mUpgradeBluestacksStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
		{
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingsGearDwnld);
			UpdatePrompt updatePrompt = new UpdatePrompt(BlueStacksUpdater.sBstUpdateData);
			((FrameworkElement)updatePrompt).Height = 215.0;
			((FrameworkElement)updatePrompt).Width = 400.0;
			UpdatePrompt updatePrompt2 = updatePrompt;
			new ContainerWindow(ParentWindow, (UserControl)(object)updatePrompt2, (int)((FrameworkElement)updatePrompt2).Width, (int)((FrameworkElement)updatePrompt2).Height);
			return;
		}
		string text = mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture);
		string localizedString = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING_UPDATE", "");
		int comparisonType = ((2046239614 > 414654850) ? 5 : 6);
		if (text.Equals(localizedString, (StringComparison)comparisonType))
		{
			((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
			BlueStacksUpdater.ShowDownloadProgress();
		}
		else if (mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
		{
			ParentWindow.ShowInstallPopup();
		}
	}

	internal void ToggleStreamingMode(bool enable)
	{
		if (enable)
		{
			mStreaminModeToggleButton.ImageName = mStreaminModeToggleButton.ImageName.Replace("_off", "_on");
		}
		else
		{
			mStreaminModeToggleButton.ImageName = mStreaminModeToggleButton.ImageName.Replace("_on", "_off");
		}
	}

	private void AutoAlign_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		CommonHandlers.ArrangeWindow();
		ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "AutoAlign", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void PinToTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val.ImageName.Contains("_off"))
		{
			val.ImageName = "toggle_on";
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
			((Window)ParentWindow).Topmost = true;
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		else
		{
			val.ImageName = "toggle_off";
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			((Window)ParentWindow).Topmost = false;
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
	}

	private void Streaming_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val.ImageName.Contains("_off"))
		{
			val.ImageName = "toggle_on";
			ParentWindow.mFrontendHandler.ToggleStreamingMode(state: true);
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStart", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		else
		{
			val.ImageName = "toggle_off";
			ParentWindow.mFrontendHandler.ToggleStreamingMode(state: false);
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStop", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
	}

	private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (sender != null)
		{
			UsefulExtensionMethod.SetTextblockTooltip((TextBlock)((sender is TextBlock) ? sender : null));
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/preferencedropdowncontrol.xaml", UriKind.Relative);
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
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Expected O, but got Unknown
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Expected O, but got Unknown
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Expected O, but got Unknown
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Expected O, but got Unknown
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Expected O, but got Unknown
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Expected O, but got Unknown
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Expected O, but got Unknown
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected O, but got Unknown
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Expected O, but got Unknown
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Expected O, but got Unknown
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Expected O, but got Unknown
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Expected O, but got Unknown
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Expected O, but got Unknown
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Expected O, but got Unknown
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Expected O, but got Unknown
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Expected O, but got Unknown
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Expected O, but got Unknown
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Expected O, but got Unknown
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Expected O, but got Unknown
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Expected O, but got Unknown
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Expected O, but got Unknown
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Expected O, but got Unknown
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Expected O, but got Unknown
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Expected O, but got Unknown
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Expected O, but got Unknown
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Expected O, but got Unknown
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Expected O, but got Unknown
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Expected O, but got Unknown
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Expected O, but got Unknown
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Expected O, but got Unknown
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Expected O, but got Unknown
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Expected O, but got Unknown
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Expected O, but got Unknown
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Expected O, but got Unknown
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Expected O, but got Unknown
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Expected O, but got Unknown
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Expected O, but got Unknown
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Expected O, but got Unknown
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Expected O, but got Unknown
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Expected O, but got Unknown
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Expected O, but got Unknown
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Expected O, but got Unknown
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Expected O, but got Unknown
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Expected O, but got Unknown
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Expected O, but got Unknown
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Expected O, but got Unknown
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Expected O, but got Unknown
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Expected O, but got Unknown
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Expected O, but got Unknown
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Expected O, but got Unknown
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Expected O, but got Unknown
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Expected O, but got Unknown
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Expected O, but got Unknown
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Expected O, but got Unknown
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Expected O, but got Unknown
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Expected O, but got Unknown
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Expected O, but got Unknown
		switch (connectionId - (-477441424 ^ -477441422))
		{
		case 0:
			EngineSettingGrid = (Grid)target;
			((UIElement)EngineSettingGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)EngineSettingGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)EngineSettingGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(EngineSettingGrid_MouseLeftButtonUp);
			break;
		case 1:
			mEngineSettingsButtonImage = (CustomPictureBox)target;
			break;
		case 2:
			mSettingsBtnNotification = (Ellipse)target;
			break;
		case 3:
			mPinToTopGrid = (Grid)target;
			((UIElement)mPinToTopGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mPinToTopGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 4:
			mPinToTopImage = (CustomPictureBox)target;
			break;
		case 5:
			mPinToTopToggleButton = (CustomPictureBox)target;
			((UIElement)mPinToTopToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PinToTop_MouseLeftButtonUp);
			break;
		case 6:
			mStreamingMode = (Grid)target;
			((UIElement)mStreamingMode).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mStreamingMode).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 7:
			mStreamingModeImage = (CustomPictureBox)target;
			break;
		case 8:
			mStreaminModeToggleButton = (CustomPictureBox)target;
			((UIElement)mStreaminModeToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Streaming_MouseLeftButtonUp);
			break;
		case 9:
			mMultiInstanceSectionTag = (Grid)target;
			break;
		case 10:
			mMultiInstanceSectionBorderLine = (Separator)target;
			break;
		case 11:
			mMultiInstanceSection = (Grid)target;
			break;
		case 12:
			mSyncGrid = (Grid)target;
			((UIElement)mSyncGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSyncGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SyncGrid_MouseLeftButtonUp);
			break;
		case 13:
			mSyncOperationsImage = (CustomPictureBox)target;
			break;
		case 14:
			mAutoAlignGrid = (Grid)target;
			((UIElement)mAutoAlignGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mAutoAlignGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mAutoAlignGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(AutoAlign_MouseLeftButtonUp);
			break;
		case 15:
			mAutoAlignImage = (CustomPictureBox)target;
			break;
		case 16:
			mUpgradeBluestacksStatus = (Grid)target;
			((UIElement)mUpgradeBluestacksStatus).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mUpgradeBluestacksStatus).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mUpgradeBluestacksStatus).MouseLeftButtonUp += new MouseButtonEventHandler(mUpgradeBluestacksStatus_MouseLeftButtonUp);
			break;
		case 17:
			mUpdateImage = (CustomPictureBox)target;
			break;
		case 18:
			mUpgradeBluestacksStatusTextBlock = (TextBlock)target;
			break;
		case 19:
			mUpdateDownloadProgressPercentage = (Label)target;
			break;
		case 20:
			mUpgradeToFullBlueStacks = (Grid)target;
			((UIElement)mUpgradeToFullBlueStacks).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mUpgradeToFullBlueStacks).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mUpgradeToFullBlueStacks).MouseLeftButtonUp += new MouseButtonEventHandler(mUpgradeToFullBlueStacks_MouseLeftButtonUp);
			break;
		case 21:
			mUpgradeToFullTextBlock = (TextBlock)target;
			break;
		case 22:
			mLogoutButtonGrid = (Grid)target;
			((UIElement)mLogoutButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mLogoutButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mLogoutButtonGrid).MouseLeftButtonUp += new MouseButtonEventHandler(LogoutButtonGrid_MouseLeftButtonUp);
			break;
		case 23:
			mCustomiseSectionTag = (Grid)target;
			break;
		case 24:
			mCustomiseSectionBorderLine = (Separator)target;
			break;
		case 25:
			mCustomiseSection = (Grid)target;
			break;
		case 26:
			mChangeSkinGrid = (Grid)target;
			((UIElement)mChangeSkinGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mChangeSkinGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mChangeSkinGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mChangeSkinGrid_MouseLeftButtonUp);
			break;
		case 27:
			mChangeSkinImage = (CustomPictureBox)target;
			break;
		case 28:
			mChangeWallpaperGrid = (Grid)target;
			((UIElement)mChangeWallpaperGrid).MouseEnter += new MouseEventHandler(mChangeWallpaperGrid_MouseEnter);
			((UIElement)mChangeWallpaperGrid).MouseLeave += new MouseEventHandler(mChangeWallpaperGrid_MouseLeave);
			break;
		case 29:
			mChangeWallpaperImage = (CustomPictureBox)target;
			break;
		case 30:
			mHelpandsupportSectionTag = (Grid)target;
			break;
		case 31:
			mHelpAndSupportSectionBorderLine = (Separator)target;
			break;
		case 32:
			mHelpandsupportSection = (Grid)target;
			break;
		case 33:
			ReportProblemGrid = (Grid)target;
			((UIElement)ReportProblemGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)ReportProblemGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)ReportProblemGrid).MouseLeftButtonUp += new MouseButtonEventHandler(ReportProblemGrid_MouseLeftButtonUp);
			break;
		case 34:
			mHelpCenterGrid = (Grid)target;
			((UIElement)mHelpCenterGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mHelpCenterGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mHelpCenterGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mHelpCenterGrid_MouseLeftButtonUp);
			break;
		case 35:
			mHelpCenterImage = (CustomPictureBox)target;
			break;
		case 36:
			mSpeedUpBstGrid = (Grid)target;
			((UIElement)mSpeedUpBstGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSpeedUpBstGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSpeedUpBstGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SpeedUpBstGrid_MouseLeftButtonUp);
			break;
		case 37:
			mSpeedUpBstImage = (CustomPictureBox)target;
			break;
		case 38:
			mWallpaperPopup = (CustomPopUp)target;
			break;
		case 39:
			mWallpaperPopupGrid = (Grid)target;
			break;
		case 40:
			dummyGridForSize = (Grid)target;
			break;
		case 41:
			mWallpaperPopupBorder = (Border)target;
			break;
		case 42:
			mMaskBorder = (Border)target;
			break;
		case 43:
			mTitleText = (TextBlock)target;
			break;
		case 44:
			mBodyText = (TextBlock)target;
			break;
		case 45:
			RightArrow = (Path)target;
			break;
		case 46:
			mChooseWallpaperPopup = (CustomPopUp)target;
			break;
		case 47:
			mChooseWallpaperPopupGrid = (Grid)target;
			break;
		case 48:
			dummyGridForSize2 = (Grid)target;
			break;
		case 49:
			mPopupGridBorder = (Border)target;
			break;
		case 50:
			mMaskBorder2 = (Border)target;
			break;
		case 51:
			mChooseNewGrid = (Grid)target;
			((UIElement)mChooseNewGrid).MouseEnter += new MouseEventHandler(ChooseNewGrid_MouseEnter);
			((UIElement)mChooseNewGrid).MouseLeave += new MouseEventHandler(ChooseNewGrid_MouseLeave);
			((UIElement)mChooseNewGrid).MouseLeftButtonUp += new MouseButtonEventHandler(ChooseNewGrid_MouseLeftButtonUp);
			break;
		case 52:
			mSetDefaultGrid = (Grid)target;
			((UIElement)mSetDefaultGrid).MouseEnter += new MouseEventHandler(SetDefaultGrid_MouseEnter);
			((UIElement)mSetDefaultGrid).MouseLeave += new MouseEventHandler(SetDefaultGrid_MouseLeave);
			((UIElement)mSetDefaultGrid).MouseLeftButtonUp += new MouseButtonEventHandler(SetDefaultGrid_MouseLeftButtonUp);
			break;
		case 53:
			mRestoreDefaultText = (TextBlock)target;
			break;
		case 54:
			mRightArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (connectionId == 1)
		{
			EventSetter val = new EventSetter();
			val.Event = FrameworkElement.SizeChangedEvent;
			val.Handler = (Delegate)new SizeChangedEventHandler(TextBlock_SizeChanged);
			((Collection<SetterBase>)(object)((Style)target).Setters).Add((SetterBase)(object)val);
		}
	}
}
