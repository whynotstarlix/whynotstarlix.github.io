using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AppNotificationsToggleButton : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppTitle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToggleButtonWithState mAppNotificationStatus;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; private set; }

	public string PackageName { get; private set; }

	public AppNotificationsToggleButton(MainWindow window, string name, string imagePath, string packageName)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Invalid comparison between Unknown and I4
		InitializeComponent();
		ParentWindow = window;
		PackageName = packageName;
		mAppTitle.Text = name;
		((Image)mAppIcon).Source = (ImageSource)(object)CustomPictureBox.GetBitmapImage(imagePath, "", true);
		if (Enumerable.Contains(((Dictionary<string, NotificationItem>)(object)NotificationManager.Instance.DictNotificationItems).Keys, name))
		{
			if ((int)((Dictionary<string, NotificationItem>)(object)NotificationManager.Instance.DictNotificationItems)[name].MuteState != 5)
			{
				mAppNotificationStatus.BoolValue = true;
			}
			else
			{
				mAppNotificationStatus.BoolValue = false;
			}
			return;
		}
		MuteState defaultState = NotificationManager.Instance.GetDefaultState(ParentWindow?.mVmName);
		((Dictionary<string, NotificationItem>)(object)NotificationManager.Instance.DictNotificationItems).Add(name, new NotificationItem(name, defaultState, DateTime.Now));
		if ((int)defaultState != 5)
		{
			mAppNotificationStatus.BoolValue = true;
		}
		else
		{
			mAppNotificationStatus.BoolValue = false;
		}
	}

	private void CustomToggleButtonWithState_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (mAppNotificationStatus.BoolValue)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)5, mAppTitle.Text);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", "Android", PackageName, "Mute", "");
		}
		else
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)0, mAppTitle.Text);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", "Android", PackageName, "UnMute", "");
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/appnotificationstogglebutton.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mAppIcon = (CustomPictureBox)target;
			break;
		case 2:
			mAppTitle = (TextBlock)target;
			break;
		case 3:
			mAppNotificationStatus = (CustomToggleButtonWithState)target;
			((UIElement)mAppNotificationStatus).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CustomToggleButtonWithState_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
