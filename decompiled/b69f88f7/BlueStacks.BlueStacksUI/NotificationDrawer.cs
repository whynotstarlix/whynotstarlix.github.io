using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class NotificationDrawer : UserControl, IComponentConnector
{
	private bool noNotification;

	private MainWindow mMainWindow;

	private static Timer snoozeInfoGridTimer = new Timer(2000.0);

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal NotificationDrawer mNotificationDrawer;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid grdImportantUpdates;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mImportantNotificationScroll;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid grdNormalUpdates;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mNotificationText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSettingsbtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSnoozeInfoGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSnoozeInfoBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mNotificationScroll;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid noNotifControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Rectangle mAnimationRect;

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

	public static Timer SnoozeInfoGridTimer
	{
		get
		{
			return snoozeInfoGridTimer;
		}
		set
		{
			snoozeInfoGridTimer = value;
		}
	}

	public static Timer DrawerAnimationTimer { get; set; } = new Timer(2000.0);

	public NotificationDrawer()
	{
		InitializeComponent();
	}

	internal void Populate(SerializableDictionary<string, GenericNotificationItem> items)
	{
		new List<NotificationDrawerItem>();
		new List<NotificationDrawerItem>();
		new List<string>();
		new List<string>();
		object content = ((ContentControl)mNotificationScroll).Content;
		StackPanel val = (StackPanel)((content is StackPanel) ? content : null);
		object content2 = ((ContentControl)mImportantNotificationScroll).Content;
		object obj = ((content2 is StackPanel) ? content2 : null);
		((Panel)val).Children.Clear();
		((Panel)obj).Children.Clear();
		((UIElement)mSnoozeInfoGrid).Visibility = (Visibility)2;
		SnoozeInfoGridTimer.Elapsed -= mSnoozeInfoGridTimer_Elapsed;
		SnoozeInfoGridTimer.Elapsed += mSnoozeInfoGridTimer_Elapsed;
		SnoozeInfoGridTimer.AutoReset = false;
		DrawerAnimationTimer.Elapsed -= DrawerAnimationTimer_Elapsed;
		DrawerAnimationTimer.Elapsed += DrawerAnimationTimer_Elapsed;
		DrawerAnimationTimer.AutoReset = false;
		foreach (KeyValuePair<string, GenericNotificationItem> item in ((IEnumerable<KeyValuePair<string, GenericNotificationItem>>)items).Where((KeyValuePair<string, GenericNotificationItem> _) => !_.Value.IsDeleted))
		{
			AddNotificationItem(item.Value);
		}
		HideUnhideNoNotification();
		UpdateNotificationCount();
	}

	private void DrawerAnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)ParentWindow.mTopBar.mNotificationCaret, Shape.FillProperty, "ContextMenuItemBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)ParentWindow.mTopBar.mNotificationCaret, Shape.StrokeProperty, "ContextMenuItemBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)ParentWindow.mTopBar.mNotificationCentreDropDownBorder, Control.BorderBrushProperty, "PopupBorderBrush");
			((UIElement)ParentWindow.mTopBar.mNotificationDrawerControl.mAnimationRect).Visibility = (Visibility)2;
			ParentWindow.mTopBar.mNotificationCentreButton.ImageName = "notification";
			((UIElement)ParentWindow.mTopBar.mNotificationCountBadge).Visibility = (Visibility)2;
			if (((Window)ParentWindow).IsActive)
			{
				((Popup)ParentWindow.mTopBar.mNotificationCentrePopup).IsOpen = true;
			}
		}, new object[0]);
	}

	private void mSnoozeInfoGridTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mSnoozeInfoGrid).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private void HideUnhideNoNotification()
	{
		object content = ((ContentControl)mNotificationScroll).Content;
		StackPanel val = (StackPanel)((content is StackPanel) ? content : null);
		object content2 = ((ContentControl)mImportantNotificationScroll).Content;
		StackPanel val2 = (StackPanel)((content2 is StackPanel) ? content2 : null);
		if (!((IEnumerable)((Panel)val2).Children).OfType<NotificationDrawerItem>().Any() && !((IEnumerable)((Panel)val).Children).OfType<NotificationDrawerItem>().Any())
		{
			((UIElement)grdImportantUpdates).Visibility = (Visibility)2;
			((UIElement)grdNormalUpdates).Visibility = (Visibility)0;
			((UIElement)noNotifControl).Visibility = (Visibility)0;
			((UIElement)mNotificationScroll).Visibility = (Visibility)2;
			ParentWindow.mTopBar.mNotificationCentreDropDownBorder_LayoutUpdated(null, null);
			noNotification = true;
			return;
		}
		if (!((IEnumerable)((Panel)val2).Children).OfType<NotificationDrawerItem>().Any())
		{
			((UIElement)grdImportantUpdates).Visibility = (Visibility)2;
		}
		if (!((IEnumerable)((Panel)val).Children).OfType<NotificationDrawerItem>().Any())
		{
			((UIElement)grdNormalUpdates).Visibility = (Visibility)2;
		}
		if (noNotification)
		{
			((UIElement)noNotifControl).Visibility = (Visibility)2;
			((UIElement)mNotificationScroll).Visibility = (Visibility)0;
		}
	}

	private void AddNotificationItem(GenericNotificationItem notifItem)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			NotificationDrawerItem notificationDrawerItem = new NotificationDrawerItem();
			notificationDrawerItem.InitFromGenricNotificationItem(notifItem, ParentWindow);
			if ((int)notifItem.Priority == 0)
			{
				object content = ((ContentControl)mImportantNotificationScroll).Content;
				StackPanel val = (StackPanel)((content is StackPanel) ? content : null);
				Separator val2 = new Separator();
				object obj = ((FrameworkElement)this).FindResource((object)ToolBar.SeparatorStyleKey);
				Style val3 = (Style)((obj is Style) ? obj : null);
				if (val3 != null)
				{
					((FrameworkElement)val2).Style = val3;
				}
				BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Panel.BackgroundProperty, "HorizontalSeparator");
				((FrameworkElement)val2).Margin = new Thickness(0.0);
				if (((IEnumerable)((Panel)val).Children).OfType<NotificationDrawerItem>().Any())
				{
					((Panel)val).Children.Insert(0, (UIElement)(object)val2);
				}
				((Panel)val).Children.Insert(0, (UIElement)(object)notificationDrawerItem);
				((UIElement)grdImportantUpdates).Visibility = (Visibility)0;
			}
			else
			{
				object content2 = ((ContentControl)mNotificationScroll).Content;
				StackPanel val4 = (StackPanel)((content2 is StackPanel) ? content2 : null);
				Separator val5 = new Separator();
				object obj2 = ((FrameworkElement)this).FindResource((object)ToolBar.SeparatorStyleKey);
				Style val6 = (Style)((obj2 is Style) ? obj2 : null);
				if (val6 != null)
				{
					((FrameworkElement)val5).Style = val6;
				}
				BlueStacksUIBinding.BindColor((DependencyObject)(object)val5, Panel.BackgroundProperty, "HorizontalSeparator");
				((FrameworkElement)val5).Margin = new Thickness(0.0);
				if (((IEnumerable)((Panel)val4).Children).OfType<NotificationDrawerItem>().Any())
				{
					((Panel)val4).Children.Insert(0, (UIElement)(object)val5);
				}
				((Panel)val4).Children.Insert(0, (UIElement)(object)notificationDrawerItem);
				((UIElement)grdNormalUpdates).Visibility = (Visibility)0;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Could not add notificationdraweritem. Id " + notifItem.Id + "Error:" + ex.ToString());
		}
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e)
	{
		RemoveAllNotificationItems();
		e.Handled = true;
	}

	public void RemoveAllNotificationItems()
	{
		object content = ((ContentControl)mNotificationScroll).Content;
		object obj = ((content is StackPanel) ? content : null);
		GenericNotificationManager.MarkNotification(from _ in ((IEnumerable)((Panel)obj).Children).OfType<NotificationDrawerItem>()
			select _.Id, delegate(GenericNotificationItem _)
		{
			_.IsDeleted = true;
		});
		((Panel)obj).Children.Clear();
		((UIElement)noNotifControl).Visibility = (Visibility)0;
		((UIElement)mNotificationScroll).Visibility = (Visibility)2;
		noNotification = true;
	}

	public void RemoveNotificationItem(string id)
	{
		object content = ((ContentControl)mNotificationScroll).Content;
		StackPanel val = (StackPanel)((content is StackPanel) ? content : null);
		foreach (NotificationDrawerItem item in ((IEnumerable)((Panel)val).Children).OfType<NotificationDrawerItem>())
		{
			if (string.Equals(item.Id, id, StringComparison.InvariantCultureIgnoreCase))
			{
				GenericNotificationManager.MarkNotification(new List<string> { id }, delegate(GenericNotificationItem x)
				{
					x.IsDeleted = true;
				});
				int num = ((Panel)val).Children.IndexOf((UIElement)(object)item);
				((Panel)val).Children.Remove((UIElement)(object)item);
				if (((Panel)val).Children.Count > num)
				{
					((Panel)val).Children.RemoveAt(num);
				}
				break;
			}
		}
		if (((Panel)val).Children.Count == 0)
		{
			((UIElement)noNotifControl).Visibility = (Visibility)0;
			((UIElement)mNotificationScroll).Visibility = (Visibility)2;
			noNotification = true;
		}
	}

	public void UpdateNotificationCount()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, ParentWindow.mVmName, StringComparison.InvariantCulture));
		if (((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count > 0 && !((Popup)ParentWindow.mTopBar.mNotificationCentrePopup).IsOpen)
		{
			Border val = new Border
			{
				VerticalAlignment = (VerticalAlignment)1,
				Height = 14.0,
				MaxWidth = 24.0
			};
			TextBlock val2 = new TextBlock
			{
				Text = ((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count.ToString(CultureInfo.InvariantCulture),
				FontSize = 10.0,
				MaxWidth = 24.0,
				FontWeight = FontWeights.Bold,
				VerticalAlignment = (VerticalAlignment)1,
				HorizontalAlignment = (HorizontalAlignment)1,
				Padding = new Thickness(3.0, 0.0, 3.0, 1.0)
			};
			if (((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count > 99)
			{
				val2.Text = "99+";
			}
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.ForegroundProperty, "SettingsWindowTitleBarForeGround");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val, Control.BackgroundProperty, "XPackPopupColor");
			val.CornerRadius = new CornerRadius(7.0);
			((Decorator)val).Child = (UIElement)(object)val2;
			Canvas.SetLeft((UIElement)(object)val, 20.0);
			Canvas.SetTop((UIElement)(object)val, 9.0);
			if (ParentWindow.mTopBar.mNotificationCountBadge != null)
			{
				if (((Dictionary<string, GenericNotificationItem>)(object)GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsRead && !x.IsDeleted && (int)x.Priority == 0)).Count > 0)
				{
					((UIElement)ParentWindow.mTopBar.mNotificationCountBadge).Visibility = (Visibility)2;
				}
				else
				{
					((UIElement)ParentWindow.mTopBar.mNotificationCountBadge).Visibility = (Visibility)0;
				}
				((Panel)ParentWindow.mTopBar.mNotificationCountBadge).Children.Clear();
				((Panel)ParentWindow.mTopBar.mNotificationCountBadge).Children.Add((UIElement)(object)val);
			}
		}
		else
		{
			((UIElement)ParentWindow.mTopBar.mNotificationCountBadge).Visibility = (Visibility)2;
		}
	}

	private void mSettingsbtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "bell_settings_clicked", ParentWindow.mVmName, "", "", "");
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				MainWindow.OpenSettingsWindow(ParentWindow, "STRING_NOTIFICATION");
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Info("Error in opening settings window" + ex);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/genericnotification/notificationdrawer.xaml", UriKind.Relative);
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
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mNotificationDrawer = (NotificationDrawer)target;
			break;
		case 2:
			grdImportantUpdates = (Grid)target;
			break;
		case 3:
			mImportantNotificationScroll = (ScrollViewer)target;
			break;
		case 4:
			grdNormalUpdates = (Grid)target;
			break;
		case 5:
			mNotificationText = (TextBlock)target;
			break;
		case 6:
			((ButtonBase)(CustomButton)target).Click += new RoutedEventHandler(ClearButton_Click);
			break;
		case 7:
			mSettingsbtn = (CustomPictureBox)target;
			((UIElement)mSettingsbtn).MouseLeftButtonUp += new MouseButtonEventHandler(mSettingsbtn_MouseLeftButtonUp);
			break;
		case 8:
			mSnoozeInfoGrid = (Grid)target;
			break;
		case 9:
			mSnoozeInfoBlock = (TextBlock)target;
			break;
		case 10:
			mNotificationScroll = (ScrollViewer)target;
			break;
		case 11:
			noNotifControl = (Grid)target;
			break;
		case 12:
			mAnimationRect = (Rectangle)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
