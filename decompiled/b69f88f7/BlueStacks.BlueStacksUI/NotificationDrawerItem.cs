using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class NotificationDrawerItem : UserControl, IComponentConnector
{
	internal string Id;

	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox icon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock titleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock dateText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNotificationActions;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSnoozeBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMutePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mLbl1Hour;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mLbl1Day;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mLbl1Week;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mLblForever;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock messageText;

	private bool _contentLoaded;

	public string PackageName { get; set; }

	public NotificationDrawerItem()
	{
		InitializeComponent();
	}

	internal void InitFromGenricNotificationItem(GenericNotificationItem item, MainWindow parentWin)
	{
		ParentWindow = parentWin;
		Id = item.Id;
		PackageName = item.Package;
		titleText.Text = item.Title;
		messageText.Text = item.Message;
		if (!item.IsRead)
		{
			ChangeToUnreadBackground();
		}
		else
		{
			ChangeToReadBackground();
		}
		if (string.Equals(item.Title, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
		{
			((UIElement)mSnoozeBtn).IsEnabled = false;
			((UIElement)mSnoozeBtn).Opacity = 0.5;
		}
		if (!string.IsNullOrEmpty(item.NotificationMenuImageName) && !string.IsNullOrEmpty(item.NotificationMenuImageUrl) && !File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, item.NotificationMenuImageName)))
		{
			item.NotificationMenuImageName = Utils.TinyDownloader(item.NotificationMenuImageUrl, item.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
		}
		icon.ImageName = item.NotificationMenuImageName;
		dateText.Text = DateTimeHelper.GetReadableDateTimeString(item.CreationTime);
	}

	private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		string fileName = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
		GenericNotificationItem notificationItem = GenericNotificationManager.Instance.GetNotificationItem(Id);
		JsonParser val = new JsonParser(ParentWindow.mVmName);
		if (ParentWindow != null && ParentWindow.mGuestBootCompleted)
		{
			if (notificationItem == null)
			{
				return;
			}
			ClientStats.SendMiscellaneousStatsAsync("NotificationDrawerItemClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, notificationItem.Id, notificationItem.Title, JsonConvert.SerializeObject((object)notificationItem.ExtraPayload), ((Dictionary<string, string>)(object)notificationItem.ExtraPayload).ContainsKey("campaign_id") ? ((Dictionary<string, string>)(object)notificationItem.ExtraPayload)["campaign_id"] : "");
			GenericNotificationManager.MarkNotification(new List<string> { notificationItem.Id }, delegate(GenericNotificationItem x)
			{
				x.IsRead = true;
			});
			ChangeToReadBackground();
			ParentWindow.mTopBar.RefreshNotificationCentreButton();
			if (((Dictionary<string, string>)(object)notificationItem.ExtraPayload).Keys.Count <= 0)
			{
				try
				{
					if (string.Compare(notificationItem.Title, "Successfully copied files:", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(notificationItem.Title, "Cannot copy files:", StringComparison.OrdinalIgnoreCase) == 0)
					{
						NotificationPopup.LaunchExplorer(notificationItem.Message);
					}
					else
					{
						Logger.Info("launching " + notificationItem.Title);
						AppInfo appInfoFromPackageName = val.GetAppInfoFromPackageName(PackageName);
						if (appInfoFromPackageName != null)
						{
							JObject val2 = new JObject();
							val2.Add("app_icon_url", JToken.op_Implicit(""));
							val2.Add("app_name", JToken.op_Implicit(appInfoFromPackageName.Name));
							val2.Add("app_url", JToken.op_Implicit(""));
							val2.Add("app_pkg", JToken.op_Implicit(PackageName));
							JObject val3 = val2;
							string text = "-json \"" + ((JToken)val3).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]).Replace("\"", "\\\"") + "\"";
							Process.Start(fileName, string.Format(CultureInfo.InvariantCulture, "{0} -vmname {1}", new object[2] { text, ParentWindow.mVmName }));
						}
						else
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							dictionary.Add("click_generic_action", ((object)(GenericAction)1/*cast due to constrained. prefix*/).ToString());
							dictionary.Add("click_action_packagename", notificationItem.Package);
							ParentWindow.Utils.HandleGenericActionFromDictionary(dictionary, "notification_drawer");
						}
					}
					return;
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString());
					return;
				}
				finally
				{
					((Popup)ParentWindow.mTopBar.mNotificationCentrePopup).IsOpen = false;
				}
			}
			ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)notificationItem.ExtraPayload, "notification_drawer", notificationItem.NotificationMenuImageName);
		}
		else if (notificationItem != null)
		{
			ParentWindow.mPostBootNotificationAction = PackageName;
			((Popup)ParentWindow.mTopBar.mNotificationCentrePopup).IsOpen = false;
		}
	}

	internal void ChangeToUnreadBackground()
	{
		((Control)this).Background = (Brush)(object)Brushes.Transparent;
	}

	internal void ChangeToReadBackground()
	{
		((UIElement)this).Opacity = 0.5;
		((Control)this).Background = (Brush)(object)Brushes.Transparent;
	}

	private void UserControl_MouseEnter(object sender, MouseEventArgs e)
	{
		((UIElement)mNotificationActions).Visibility = (Visibility)0;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void UserControl_MouseLeave(object sender, MouseEventArgs e)
	{
		if (((Popup)mMutePopup).IsOpen)
		{
			return;
		}
		((UIElement)mNotificationActions).Visibility = (Visibility)2;
		if (GenericNotificationManager.Instance.GetNotificationItem(Id) != null)
		{
			if (!GenericNotificationManager.Instance.GetNotificationItem(Id).IsRead)
			{
				ChangeToUnreadBackground();
			}
			else
			{
				ChangeToReadBackground();
			}
		}
	}

	private void mCloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(Id);
		((RoutedEventArgs)e).Handled = true;
	}

	private void Grid_MouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundColor");
	}

	private void Grid_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void Lbl1Hour_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NotificationManager.Instance.UpdateMuteState((MuteState)2, titleText.Text);
		string text = default(string);
		string text2 = default(string);
		string text3 = default(string);
		new JsonParser(ParentWindow.mVmName).GetAppInfoFromAppName(titleText.Text, ref text, ref text2, ref text3);
		Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + ((TextBlock)((sender is TextBlock) ? sender : null)).Text, "");
		((Popup)mMutePopup).IsOpen = false;
		ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(Id);
		((UIElement)ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid).Visibility = (Visibility)0;
		ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[2]
		{
			titleText.Text,
			((TextBlock)((sender is TextBlock) ? sender : null)).Text
		});
		NotificationDrawer.SnoozeInfoGridTimer.Start();
		((RoutedEventArgs)e).Handled = true;
	}

	private void Lbl1Day_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NotificationManager.Instance.UpdateMuteState((MuteState)3, titleText.Text);
		string text = default(string);
		string text2 = default(string);
		string text3 = default(string);
		new JsonParser(ParentWindow.mVmName).GetAppInfoFromAppName(titleText.Text, ref text, ref text2, ref text3);
		Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + ((TextBlock)((sender is TextBlock) ? sender : null)).Text, "");
		((Popup)mMutePopup).IsOpen = false;
		ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(Id);
		((UIElement)ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid).Visibility = (Visibility)0;
		ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[2]
		{
			titleText.Text,
			((TextBlock)((sender is TextBlock) ? sender : null)).Text
		});
		NotificationDrawer.SnoozeInfoGridTimer.Start();
		((RoutedEventArgs)e).Handled = true;
	}

	private void Lbl1Week_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NotificationManager.Instance.UpdateMuteState((MuteState)4, titleText.Text);
		string text = default(string);
		string text2 = default(string);
		string text3 = default(string);
		new JsonParser(ParentWindow.mVmName).GetAppInfoFromAppName(titleText.Text, ref text, ref text2, ref text3);
		Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + ((TextBlock)((sender is TextBlock) ? sender : null)).Text, "");
		((Popup)mMutePopup).IsOpen = false;
		ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(Id);
		((UIElement)ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid).Visibility = (Visibility)0;
		ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[2]
		{
			titleText.Text,
			((TextBlock)((sender is TextBlock) ? sender : null)).Text
		});
		NotificationDrawer.SnoozeInfoGridTimer.Start();
		((RoutedEventArgs)e).Handled = true;
	}

	private void LblForever_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NotificationManager.Instance.UpdateMuteState((MuteState)5, titleText.Text);
		string text = default(string);
		string text2 = default(string);
		string text3 = default(string);
		new JsonParser(ParentWindow.mVmName).GetAppInfoFromAppName(titleText.Text, ref text, ref text2, ref text3);
		Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + ((TextBlock)((sender is TextBlock) ? sender : null)).Text, "");
		((Popup)mMutePopup).IsOpen = false;
		ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(Id);
		((UIElement)ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid).Visibility = (Visibility)0;
		ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[2]
		{
			titleText.Text,
			((TextBlock)((sender is TextBlock) ? sender : null)).Text
		});
		NotificationDrawer.SnoozeInfoGridTimer.Start();
		((RoutedEventArgs)e).Handled = true;
	}

	private void mSnoozeBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mMutePopup).IsOpen = !((Popup)mMutePopup).IsOpen;
		((RoutedEventArgs)e).Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/genericnotification/notificationdraweritem.xaml", UriKind.Relative);
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
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected O, but got Unknown
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(NotificationDrawerItem)target).MouseLeftButtonUp += new MouseButtonEventHandler(UserControl_MouseLeftButtonUp);
			((UIElement)(NotificationDrawerItem)target).MouseEnter += new MouseEventHandler(UserControl_MouseEnter);
			((UIElement)(NotificationDrawerItem)target).MouseLeave += new MouseEventHandler(UserControl_MouseLeave);
			break;
		case 2:
			icon = (CustomPictureBox)target;
			break;
		case 3:
			titleText = (TextBlock)target;
			break;
		case 4:
			dateText = (TextBlock)target;
			break;
		case 5:
			mNotificationActions = (Grid)target;
			break;
		case 6:
			mSnoozeBtn = (CustomPictureBox)target;
			((UIElement)mSnoozeBtn).MouseLeftButtonUp += new MouseButtonEventHandler(mSnoozeBtn_MouseLeftButtonUp);
			break;
		case 7:
			mMutePopup = (CustomPopUp)target;
			break;
		case 8:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 9:
			mLbl1Hour = (TextBlock)target;
			((UIElement)mLbl1Hour).MouseUp += new MouseButtonEventHandler(Lbl1Hour_MouseUp);
			break;
		case 10:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 11:
			mLbl1Day = (TextBlock)target;
			((UIElement)mLbl1Day).MouseUp += new MouseButtonEventHandler(Lbl1Day_MouseUp);
			break;
		case 12:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 13:
			mLbl1Week = (TextBlock)target;
			((UIElement)mLbl1Week).MouseUp += new MouseButtonEventHandler(Lbl1Week_MouseUp);
			break;
		case 14:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 15:
			mLblForever = (TextBlock)target;
			((UIElement)mLblForever).MouseUp += new MouseButtonEventHandler(LblForever_MouseUp);
			break;
		case 16:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(mCloseBtn_MouseLeftButtonUp);
			break;
		case 17:
			messageText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
