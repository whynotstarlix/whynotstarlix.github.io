using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PreferenceDropDownControl : UserControl, IComponentConnector, IStyleConnector
{
	internal Grid EngineSettingGrid;

	internal CustomPictureBox mEngineSettingsButtonImage;

	internal Ellipse mSettingsBtnNotification;

	internal Grid mPinToTopGrid;

	internal CustomPictureBox mPinToTopImage;

	internal CustomPictureBox mPinToTopToggleButton;

	internal Grid mStreamingMode;

	internal CustomPictureBox mStreamingModeImage;

	internal CustomPictureBox mStreaminModeToggleButton;

	internal Grid mMultiInstanceSectionTag;

	internal Separator mMultiInstanceSectionBorderLine;

	internal Grid mMultiInstanceSection;

	internal Grid mSyncGrid;

	internal CustomPictureBox mSyncOperationsImage;

	internal Grid mAutoAlignGrid;

	internal CustomPictureBox mAutoAlignImage;

	internal Grid mUpgradeBluestacksStatus;

	internal CustomPictureBox mUpdateImage;

	internal TextBlock mUpgradeBluestacksStatusTextBlock;

	internal Label mUpdateDownloadProgressPercentage;

	internal Grid mUpgradeToFullBlueStacks;

	internal TextBlock mUpgradeToFullTextBlock;

	internal Grid mLogoutButtonGrid;

	internal Grid mCustomiseSectionTag;

	internal Separator mCustomiseSectionBorderLine;

	internal Grid mCustomiseSection;

	internal Grid mChangeSkinGrid;

	internal CustomPictureBox mChangeSkinImage;

	internal Grid mChangeWallpaperGrid;

	internal CustomPictureBox mChangeWallpaperImage;

	internal Grid mHelpandsupportSectionTag;

	internal Separator mHelpAndSupportSectionBorderLine;

	internal Grid mHelpandsupportSection;

	internal Grid ReportProblemGrid;

	internal Grid mHelpCenterGrid;

	internal CustomPictureBox mHelpCenterImage;

	internal Grid mSpeedUpBstGrid;

	internal CustomPictureBox mSpeedUpBstImage;

	internal CustomPopUp mWallpaperPopup;

	internal Grid mWallpaperPopupGrid;

	internal Grid dummyGridForSize;

	internal Border mWallpaperPopupBorder;

	internal Border mMaskBorder;

	internal TextBlock mTitleText;

	internal TextBlock mBodyText;

	internal Path RightArrow;

	internal CustomPopUp mChooseWallpaperPopup;

	internal Grid mChooseWallpaperPopupGrid;

	internal Grid dummyGridForSize2;

	internal Border mPopupGridBorder;

	internal Border mMaskBorder2;

	internal Grid mChooseNewGrid;

	internal Grid mSetDefaultGrid;

	internal TextBlock mRestoreDefaultText;

	internal Path mRightArrow;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; set; }

	private event EventHandler LogoutConfirmationResetAccountAcceptedHandler;

	private event EventHandler RestoreDefaultConfirmationClicked;

	public PreferenceDropDownControl()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
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
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
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
			((UIElement)mPinToTopGrid).Visibility = (Visibility)2;
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
			((UIElement)mSyncGrid).Visibility = (Visibility)2;
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
		((UIElement)mCustomiseSectionTag).Visibility = (Visibility)((!CheckSectionTagVisibility(mCustomiseSection)) ? 2 : 0);
		((UIElement)mHelpandsupportSectionTag).Visibility = (Visibility)((!CheckSectionTagVisibility(mHelpandsupportSection)) ? 2 : 0);
	}

	private static bool CheckSectionTagVisibility(Grid sectionGrid)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		IEnumerator enumerator = ((Panel)sectionGrid).Children.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			Grid val = (Grid)((current is Grid) ? current : null);
			if (val != null && (int)((UIElement)val).Visibility == 0)
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
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Clicked logout button");
		if (ParentWindow.mGuestBootCompleted)
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_LOGOUT_BLUESTACKS3", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_REMOVE_GOOGLE_ACCOUNT", "");
			val.AddButton((ButtonColors)0, "STRING_LOGOUT_BUTTON", this.LogoutConfirmationResetAccountAcceptedHandler, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
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
		int num = 504;
		int num2 = 652;
		new ContainerWindow(ParentWindow, (UserControl)(object)control, num2, num);
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_LBL_RESTORE_DEFAULT", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_RESTORE_DEFAULT_WALLPAPER", "");
			val.AddButton((ButtonColors)0, "STRING_RESTORE_BUTTON", this.RestoreDefaultConfirmationClicked, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
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
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		string localizedString = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "");
		string text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2]
		{
			LocaleStrings.GetLocalizedString("STRING_CONTINUING_WILL_UPGRADE_TO_STD_BST", ""),
			LocaleStrings.GetLocalizedString("STRING_LAUNCH_BLUESTACKS_FROM_DESK_SHORTCUT", "")
		});
		localizedString = localizedString.Replace(GameConfig.Instance.AppName, "BlueStacks");
		text = text.Replace(GameConfig.Instance.AppName, "BlueStacks");
		CustomMessageWindow val = new CustomMessageWindow();
		val.AddButton((ButtonColors)4, "STRING_YES", (EventHandler)UpgradeToFullBstHandler, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_NO", (EventHandler)null, (string)null, false, (object)null);
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
		}
		else if (mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOADING_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
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
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Expected O, but got Unknown
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Expected O, but got Unknown
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Expected O, but got Unknown
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected O, but got Unknown
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Expected O, but got Unknown
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Expected O, but got Unknown
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Expected O, but got Unknown
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Expected O, but got Unknown
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Expected O, but got Unknown
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Expected O, but got Unknown
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Expected O, but got Unknown
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Expected O, but got Unknown
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Expected O, but got Unknown
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Expected O, but got Unknown
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Expected O, but got Unknown
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Expected O, but got Unknown
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Expected O, but got Unknown
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Expected O, but got Unknown
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Expected O, but got Unknown
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Expected O, but got Unknown
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Expected O, but got Unknown
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Expected O, but got Unknown
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Expected O, but got Unknown
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Expected O, but got Unknown
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Expected O, but got Unknown
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Expected O, but got Unknown
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Expected O, but got Unknown
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Expected O, but got Unknown
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Expected O, but got Unknown
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Expected O, but got Unknown
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Expected O, but got Unknown
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Expected O, but got Unknown
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Expected O, but got Unknown
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Expected O, but got Unknown
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Expected O, but got Unknown
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Expected O, but got Unknown
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Expected O, but got Unknown
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Expected O, but got Unknown
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Expected O, but got Unknown
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Expected O, but got Unknown
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Expected O, but got Unknown
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Expected O, but got Unknown
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Expected O, but got Unknown
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Expected O, but got Unknown
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Expected O, but got Unknown
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Expected O, but got Unknown
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Expected O, but got Unknown
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Expected O, but got Unknown
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Expected O, but got Unknown
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Expected O, but got Unknown
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Expected O, but got Unknown
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Expected O, but got Unknown
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Expected O, but got Unknown
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Expected O, but got Unknown
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Expected O, but got Unknown
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Expected O, but got Unknown
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Expected O, but got Unknown
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Expected O, but got Unknown
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Expected O, but got Unknown
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Expected O, but got Unknown
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Expected O, but got Unknown
		//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Expected O, but got Unknown
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Expected O, but got Unknown
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f3: Expected O, but got Unknown
		switch (connectionId)
		{
		case 2:
			EngineSettingGrid = (Grid)target;
			((UIElement)EngineSettingGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)EngineSettingGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)EngineSettingGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(EngineSettingGrid_MouseLeftButtonUp);
			break;
		case 3:
			mEngineSettingsButtonImage = (CustomPictureBox)target;
			break;
		case 4:
			mSettingsBtnNotification = (Ellipse)target;
			((UIElement)mSettingsBtnNotification).Visibility = (Visibility)2;
			break;
		case 5:
			mPinToTopGrid = (Grid)target;
			((UIElement)mPinToTopGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mPinToTopGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mPinToTopGrid).Visibility = (Visibility)2;
			break;
		case 6:
			mPinToTopImage = (CustomPictureBox)target;
			((UIElement)mPinToTopImage).Visibility = (Visibility)2;
			break;
		case 7:
			mPinToTopToggleButton = (CustomPictureBox)target;
			((UIElement)mPinToTopToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PinToTop_MouseLeftButtonUp);
			((UIElement)mPinToTopToggleButton).Visibility = (Visibility)2;
			break;
		case 8:
			mStreamingMode = (Grid)target;
			((UIElement)mStreamingMode).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mStreamingMode).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 9:
			mStreamingModeImage = (CustomPictureBox)target;
			break;
		case 10:
			mStreaminModeToggleButton = (CustomPictureBox)target;
			((UIElement)mStreaminModeToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Streaming_MouseLeftButtonUp);
			break;
		case 11:
			mMultiInstanceSectionTag = (Grid)target;
			((UIElement)mMultiInstanceSectionTag).Visibility = (Visibility)2;
			break;
		case 12:
			mMultiInstanceSectionBorderLine = (Separator)target;
			((UIElement)mMultiInstanceSectionBorderLine).Visibility = (Visibility)2;
			break;
		case 13:
			mMultiInstanceSection = (Grid)target;
			((UIElement)mMultiInstanceSection).Visibility = (Visibility)2;
			break;
		case 14:
			mSyncGrid = (Grid)target;
			((UIElement)mSyncGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSyncGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SyncGrid_MouseLeftButtonUp);
			((UIElement)mSyncGrid).Visibility = (Visibility)2;
			break;
		case 15:
			mSyncOperationsImage = (CustomPictureBox)target;
			((UIElement)mSyncOperationsImage).Visibility = (Visibility)2;
			break;
		case 16:
			mAutoAlignGrid = (Grid)target;
			((UIElement)mAutoAlignGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mAutoAlignGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mAutoAlignGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(AutoAlign_MouseLeftButtonUp);
			((UIElement)mAutoAlignGrid).Visibility = (Visibility)2;
			break;
		case 17:
			mAutoAlignImage = (CustomPictureBox)target;
			((UIElement)mAutoAlignImage).Visibility = (Visibility)2;
			break;
		case 18:
			mUpgradeBluestacksStatus = (Grid)target;
			((UIElement)mUpgradeBluestacksStatus).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mUpgradeBluestacksStatus).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mUpgradeBluestacksStatus).MouseLeftButtonUp += new MouseButtonEventHandler(mUpgradeBluestacksStatus_MouseLeftButtonUp);
			((UIElement)mUpgradeBluestacksStatus).Visibility = (Visibility)2;
			break;
		case 19:
			mUpdateImage = (CustomPictureBox)target;
			((UIElement)mUpdateImage).Visibility = (Visibility)2;
			break;
		case 20:
			mUpgradeBluestacksStatusTextBlock = (TextBlock)target;
			((UIElement)mUpgradeBluestacksStatusTextBlock).Visibility = (Visibility)2;
			break;
		case 21:
			mUpdateDownloadProgressPercentage = (Label)target;
			((UIElement)mUpdateDownloadProgressPercentage).Visibility = (Visibility)2;
			break;
		case 22:
			mUpgradeToFullBlueStacks = (Grid)target;
			((UIElement)mUpgradeToFullBlueStacks).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mUpgradeToFullBlueStacks).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mUpgradeToFullBlueStacks).MouseLeftButtonUp += new MouseButtonEventHandler(mUpgradeToFullBlueStacks_MouseLeftButtonUp);
			((UIElement)mUpgradeToFullBlueStacks).Visibility = (Visibility)2;
			break;
		case 23:
			mUpgradeToFullTextBlock = (TextBlock)target;
			((UIElement)mUpgradeToFullTextBlock).Visibility = (Visibility)2;
			break;
		case 24:
			mLogoutButtonGrid = (Grid)target;
			((UIElement)mLogoutButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mLogoutButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mLogoutButtonGrid).MouseLeftButtonUp += new MouseButtonEventHandler(LogoutButtonGrid_MouseLeftButtonUp);
			((UIElement)mLogoutButtonGrid).Visibility = (Visibility)2;
			break;
		case 25:
			mCustomiseSectionTag = (Grid)target;
			((UIElement)mCustomiseSectionTag).Visibility = (Visibility)2;
			break;
		case 26:
			mCustomiseSectionBorderLine = (Separator)target;
			((UIElement)mCustomiseSectionBorderLine).Visibility = (Visibility)2;
			break;
		case 27:
			mCustomiseSection = (Grid)target;
			((UIElement)mCustomiseSection).Visibility = (Visibility)2;
			break;
		case 28:
			mChangeSkinGrid = (Grid)target;
			((UIElement)mChangeSkinGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mChangeSkinGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mChangeSkinGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mChangeSkinGrid_MouseLeftButtonUp);
			((UIElement)mChangeSkinGrid).Visibility = (Visibility)2;
			break;
		case 29:
			mChangeSkinImage = (CustomPictureBox)target;
			((UIElement)mChangeSkinImage).Visibility = (Visibility)2;
			break;
		case 30:
			mChangeWallpaperGrid = (Grid)target;
			((UIElement)mChangeWallpaperGrid).MouseEnter += new MouseEventHandler(mChangeWallpaperGrid_MouseEnter);
			((UIElement)mChangeWallpaperGrid).MouseLeave += new MouseEventHandler(mChangeWallpaperGrid_MouseLeave);
			((UIElement)mChangeWallpaperGrid).Visibility = (Visibility)2;
			break;
		case 31:
			mChangeWallpaperImage = (CustomPictureBox)target;
			((UIElement)mChangeWallpaperImage).Visibility = (Visibility)2;
			break;
		case 32:
			mHelpandsupportSectionTag = (Grid)target;
			((UIElement)mHelpandsupportSectionTag).Visibility = (Visibility)2;
			break;
		case 33:
			mHelpAndSupportSectionBorderLine = (Separator)target;
			((UIElement)mHelpAndSupportSectionBorderLine).Visibility = (Visibility)2;
			break;
		case 34:
			mHelpandsupportSection = (Grid)target;
			((UIElement)mHelpandsupportSection).Visibility = (Visibility)2;
			break;
		case 35:
			ReportProblemGrid = (Grid)target;
			((UIElement)ReportProblemGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)ReportProblemGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)ReportProblemGrid).MouseLeftButtonUp += new MouseButtonEventHandler(ReportProblemGrid_MouseLeftButtonUp);
			((UIElement)ReportProblemGrid).Visibility = (Visibility)2;
			break;
		case 36:
			mHelpCenterGrid = (Grid)target;
			((UIElement)mHelpCenterGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mHelpCenterGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mHelpCenterGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mHelpCenterGrid_MouseLeftButtonUp);
			((UIElement)mHelpCenterGrid).Visibility = (Visibility)2;
			break;
		case 37:
			mHelpCenterImage = (CustomPictureBox)target;
			((UIElement)mHelpCenterImage).Visibility = (Visibility)2;
			break;
		case 38:
			mSpeedUpBstGrid = (Grid)target;
			((UIElement)mSpeedUpBstGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSpeedUpBstGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSpeedUpBstGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SpeedUpBstGrid_MouseLeftButtonUp);
			((UIElement)mSpeedUpBstGrid).Visibility = (Visibility)2;
			break;
		case 39:
			mSpeedUpBstImage = (CustomPictureBox)target;
			((UIElement)mSpeedUpBstImage).Visibility = (Visibility)2;
			break;
		case 40:
			mWallpaperPopup = (CustomPopUp)target;
			((Popup)mWallpaperPopup).IsOpen = false;
			break;
		case 41:
			mWallpaperPopupGrid = (Grid)target;
			((UIElement)mWallpaperPopupGrid).Visibility = (Visibility)2;
			break;
		case 42:
			dummyGridForSize = (Grid)target;
			((UIElement)dummyGridForSize).Visibility = (Visibility)2;
			break;
		case 43:
			mWallpaperPopupBorder = (Border)target;
			((UIElement)mWallpaperPopupBorder).Visibility = (Visibility)2;
			break;
		case 44:
			mMaskBorder = (Border)target;
			((UIElement)mMaskBorder).Visibility = (Visibility)2;
			break;
		case 45:
			mTitleText = (TextBlock)target;
			((UIElement)mTitleText).Visibility = (Visibility)2;
			break;
		case 46:
			mBodyText = (TextBlock)target;
			((UIElement)mBodyText).Visibility = (Visibility)2;
			break;
		case 47:
			RightArrow = (Path)target;
			((UIElement)RightArrow).Visibility = (Visibility)2;
			break;
		case 48:
			mChooseWallpaperPopup = (CustomPopUp)target;
			((Popup)mChooseWallpaperPopup).IsOpen = false;
			break;
		case 49:
			mChooseWallpaperPopupGrid = (Grid)target;
			((UIElement)mChooseWallpaperPopupGrid).Visibility = (Visibility)2;
			break;
		case 50:
			dummyGridForSize2 = (Grid)target;
			((UIElement)dummyGridForSize2).Visibility = (Visibility)2;
			break;
		case 51:
			mPopupGridBorder = (Border)target;
			((UIElement)mPopupGridBorder).Visibility = (Visibility)2;
			break;
		case 52:
			mMaskBorder2 = (Border)target;
			((UIElement)mMaskBorder2).Visibility = (Visibility)2;
			break;
		case 53:
			mChooseNewGrid = (Grid)target;
			((UIElement)mChooseNewGrid).MouseEnter += new MouseEventHandler(ChooseNewGrid_MouseEnter);
			((UIElement)mChooseNewGrid).MouseLeave += new MouseEventHandler(ChooseNewGrid_MouseLeave);
			((UIElement)mChooseNewGrid).MouseLeftButtonUp += new MouseButtonEventHandler(ChooseNewGrid_MouseLeftButtonUp);
			((UIElement)mChooseNewGrid).Visibility = (Visibility)2;
			break;
		case 54:
			mSetDefaultGrid = (Grid)target;
			((UIElement)mSetDefaultGrid).MouseEnter += new MouseEventHandler(SetDefaultGrid_MouseEnter);
			((UIElement)mSetDefaultGrid).MouseLeave += new MouseEventHandler(SetDefaultGrid_MouseLeave);
			((UIElement)mSetDefaultGrid).MouseLeftButtonUp += new MouseButtonEventHandler(SetDefaultGrid_MouseLeftButtonUp);
			((UIElement)mSetDefaultGrid).Visibility = (Visibility)2;
			break;
		case 55:
			mRestoreDefaultText = (TextBlock)target;
			((UIElement)mRestoreDefaultText).Visibility = (Visibility)2;
			break;
		case 56:
			mRightArrow = (Path)target;
			((UIElement)mRightArrow).Visibility = (Visibility)2;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
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
