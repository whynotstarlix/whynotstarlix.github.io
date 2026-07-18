using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class NotificationsSettings : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private string mVmName = "Android";

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScroll;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNotificationModeSettingsSection;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mMinimzeOnCloseCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mReadMoreSection;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mCollapsedArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mExpandededArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mNotificationModeToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mNotifModeInfoGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mNotificationSoundToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRibbonHelp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mRibbonNotificationsToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mRibbonPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mToastHelp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mToastNotificationsToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mToastPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mAppSpecificNotificationsToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mInfoIcon;

	private bool _contentLoaded;

	public static NotificationsSettings Instance { get; set; }

	public NotificationsSettings(MainWindow window)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		Instance = this;
		InitializeComponent();
		ParentWindow = window;
		((UIElement)this).Visibility = (Visibility)1;
		mVmName = window?.mVmName;
		mNotificationModeToggleButton.BoolValue = RegistryManager.Instance.IsNotificationModeAlwaysOn;
		mNotificationSoundToggleButton.BoolValue = RegistryManager.Instance.IsNotificationSoundsActive;
		mRibbonNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowRibbonNotification;
		mToastNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowToastNotification;
		if (!string.Equals(mVmName, "Android", StringComparison.InvariantCultureIgnoreCase))
		{
			((UIElement)mNotificationModeSettingsSection).Visibility = (Visibility)2;
			((UIElement)mMinimzeOnCloseCheckBox).Visibility = (Visibility)2;
		}
		else
		{
			((UIElement)mExpandededArrow).Visibility = (Visibility)2;
			((UIElement)mNotifModeInfoGrid).Visibility = (Visibility)2;
			((UIElement)mCollapsedArrow).Visibility = (Visibility)0;
		}
		mScroll.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
		((ToggleButton)mMinimzeOnCloseCheckBox).IsChecked = !ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose;
	}

	private void NotificationSettings_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		NotificationManager.Instance.ReloadNotificationDetails();
		List<AppInfo> list = new JsonParser(mVmName).GetAppList().ToList();
		bool flag = true;
		foreach (AppInfo item in list)
		{
			bool flag2 = !AddNotificationToggleButton(item.Name, item.Img, item.Package);
			flag = flag && flag2;
		}
		if (list.Count > 0)
		{
			mAppSpecificNotificationsToggleButton.BoolValue = !flag;
		}
		else
		{
			mAppSpecificNotificationsToggleButton.BoolValue = true;
		}
		if (mAppSpecificNotificationsToggleButton.BoolValue)
		{
			((UIElement)mStackPanel).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mStackPanel).Visibility = (Visibility)2;
		}
	}

	private bool AddNotificationToggleButton(string name, string imageName, string package)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		string imagePath = Path.Combine(RegistryStrings.GadgetDir, imageName);
		AppNotificationsToggleButton appNotificationsToggleButton = new AppNotificationsToggleButton(ParentWindow, name, imagePath, package);
		((FrameworkElement)appNotificationsToggleButton).Margin = new Thickness(0.0, 0.0, 0.0, 12.0);
		AppNotificationsToggleButton appNotificationsToggleButton2 = appNotificationsToggleButton;
		((Panel)mStackPanel).Children.Add((UIElement)(object)appNotificationsToggleButton2);
		return appNotificationsToggleButton2.mAppNotificationStatus.BoolValue;
	}

	private void CheckBox_Click(object sender, RoutedEventArgs e)
	{
		ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = ((ToggleButton)mMinimzeOnCloseCheckBox).IsChecked != true;
		Stats.SendCommonClientStatsAsync("notification_mode", "donotshow_checkbox", "Android", string.Empty, (!ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose).ToString(CultureInfo.InvariantCulture), "");
	}

	private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("notification_mode", "readarticle", ParentWindow.mVmName, KMManager.sPackageName, "", "");
		Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=notification_mode_help");
		((RoutedEventArgs)e).Handled = true;
	}

	private void mReadMoreSection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)mCollapsedArrow).Visibility == 0)
		{
			((UIElement)mCollapsedArrow).Visibility = (Visibility)2;
			((UIElement)mExpandededArrow).Visibility = (Visibility)0;
			((UIElement)mNotifModeInfoGrid).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mExpandededArrow).Visibility = (Visibility)2;
			((UIElement)mNotifModeInfoGrid).Visibility = (Visibility)2;
			((UIElement)mCollapsedArrow).Visibility = (Visibility)0;
		}
	}

	private void mNotificationModeToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		RegistryManager.Instance.IsNotificationModeAlwaysOn = !mNotificationModeToggleButton.BoolValue;
		Stats.SendCommonClientStatsAsync("notification_mode", RegistryManager.Instance.IsNotificationModeAlwaysOn ? "toggle_on" : "toggle_off", "Android", "", "", "");
	}

	private void mNotificationSoundToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		RegistryManager.Instance.IsNotificationSoundsActive = !mNotificationSoundToggleButton.BoolValue;
		Stats.SendCommonClientStatsAsync("notification_mode", "notification_sound_toggle", "Android", string.Empty, RegistryManager.Instance.IsNotificationSoundsActive.ToString(CultureInfo.InvariantCulture), "");
	}

	private void mToastNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		RegistryManager.Instance.IsShowToastNotification = !mToastNotificationsToggleButton.BoolValue;
		Stats.SendCommonClientStatsAsync("notification_mode", "toast_notification_toggle", "Android", string.Empty, RegistryManager.Instance.IsShowToastNotification.ToString(CultureInfo.InvariantCulture), "");
	}

	private void mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		RegistryManager.Instance.IsShowRibbonNotification = !mRibbonNotificationsToggleButton.BoolValue;
		Stats.SendCommonClientStatsAsync("notification_mode", "ribbon_notification_toggle", "Android", string.Empty, RegistryManager.Instance.IsShowRibbonNotification.ToString(CultureInfo.InvariantCulture), "");
	}

	private void mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("notification_mode", "all_apps_notifications_muted_toggle", "Android", string.Empty, (!mAppSpecificNotificationsToggleButton.BoolValue).ToString(CultureInfo.InvariantCulture), "");
		if (!mAppSpecificNotificationsToggleButton.BoolValue)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)0, NotificationManager.Instance.ShowNotificationText);
			((UIElement)mStackPanel).Visibility = (Visibility)0;
		}
		else
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)5, NotificationManager.Instance.ShowNotificationText);
			((UIElement)mStackPanel).Visibility = (Visibility)2;
		}
		foreach (AppNotificationsToggleButton child in ((Panel)mStackPanel).Children)
		{
			child.mAppNotificationStatus.BoolValue = !mAppSpecificNotificationsToggleButton.BoolValue;
			if (child.mAppNotificationStatus.BoolValue)
			{
				NotificationManager.Instance.UpdateMuteState((MuteState)0, child.mAppTitle.Text);
			}
			else
			{
				NotificationManager.Instance.UpdateMuteState((MuteState)5, child.mAppTitle.Text);
			}
		}
	}

	private void mRibbonHelp_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mRibbonPopup).IsOpen = true;
	}

	private void mRibbonHelp_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mRibbonPopup).IsOpen = false;
	}

	private void mToastHelp_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mToastPopup).IsOpen = true;
	}

	private void mToastHelp_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mToastPopup).IsOpen = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/notificationssettings.xaml", UriKind.Relative);
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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(NotificationsSettings)target).Loaded += new RoutedEventHandler(NotificationSettings_Loaded);
			break;
		case 2:
			mScroll = (ScrollViewer)target;
			break;
		case 3:
			mNotificationModeSettingsSection = (Grid)target;
			break;
		case 4:
			mMinimzeOnCloseCheckBox = (CustomCheckbox)target;
			((ButtonBase)mMinimzeOnCloseCheckBox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 5:
			mReadMoreSection = (Label)target;
			((UIElement)mReadMoreSection).MouseLeftButtonUp += new MouseButtonEventHandler(mReadMoreSection_MouseLeftButtonUp);
			break;
		case 6:
			mCollapsedArrow = (Path)target;
			break;
		case 7:
			mExpandededArrow = (Path)target;
			break;
		case 8:
			mNotificationModeToggleButton = (CustomToggleButtonWithState)target;
			((UIElement)mNotificationModeToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mNotificationModeToggleButton_PreviewMouseLeftButtonUp);
			break;
		case 9:
			mNotifModeInfoGrid = (Border)target;
			break;
		case 10:
			((UIElement)(TextBlock)target).MouseLeftButtonDown += new MouseButtonEventHandler(ReadMoreLinkMouseLeftButtonUp);
			break;
		case 11:
			mNotificationSoundToggleButton = (CustomToggleButtonWithState)target;
			((UIElement)mNotificationSoundToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mNotificationSoundToggleButton_PreviewMouseLeftButtonUp);
			break;
		case 12:
			mRibbonHelp = (CustomPictureBox)target;
			((UIElement)mRibbonHelp).MouseEnter += new MouseEventHandler(mRibbonHelp_MouseEnter);
			((UIElement)mRibbonHelp).MouseLeave += new MouseEventHandler(mRibbonHelp_MouseLeave);
			break;
		case 13:
			mRibbonNotificationsToggleButton = (CustomToggleButtonWithState)target;
			((UIElement)mRibbonNotificationsToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp);
			break;
		case 14:
			mRibbonPopup = (CustomPopUp)target;
			break;
		case 15:
			mToastHelp = (CustomPictureBox)target;
			((UIElement)mToastHelp).MouseEnter += new MouseEventHandler(mToastHelp_MouseEnter);
			((UIElement)mToastHelp).MouseLeave += new MouseEventHandler(mToastHelp_MouseLeave);
			break;
		case 16:
			mToastNotificationsToggleButton = (CustomToggleButtonWithState)target;
			((UIElement)mToastNotificationsToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mToastNotificationsToggleButton_PreviewMouseLeftButtonUp);
			break;
		case 17:
			mToastPopup = (CustomPopUp)target;
			break;
		case 18:
			mAppSpecificNotificationsToggleButton = (CustomToggleButtonWithState)target;
			((UIElement)mAppSpecificNotificationsToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp);
			break;
		case 19:
			mStackPanel = (StackPanel)target;
			break;
		case 20:
			mInfoIcon = (CustomPictureBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
