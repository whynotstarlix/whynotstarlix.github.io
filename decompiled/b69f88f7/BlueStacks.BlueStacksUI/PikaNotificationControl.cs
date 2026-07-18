using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class PikaNotificationControl : UserControl, IComponentConnector
{
	internal MainWindow ParentWindow;

	private GenericNotificationItem mNotificationItem;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNotificationGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path ribbonBack;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path ribbonStroke;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel backgroundPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox pikaGif;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock titleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock messageText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border notificationBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	private bool _contentLoaded;

	public event EventHandler CloseClicked;

	public PikaNotificationControl()
	{
		InitializeComponent();
	}

	private void pikanotificationcontrol_MouseUp(object sender, MouseButtonEventArgs e)
	{
		if (ParentWindow != null && ParentWindow.mGuestBootCompleted)
		{
			ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)mNotificationItem.ExtraPayload, "notification_ribbon", mNotificationItem.NotificationMenuImageName);
			ClientStats.SendMiscellaneousStatsAsync("RibbonClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, mNotificationItem.Id, mNotificationItem.Title, JsonConvert.SerializeObject((object)mNotificationItem.ExtraPayload));
			GenericNotificationManager.MarkNotification(new List<string> { mNotificationItem.Id }, delegate(GenericNotificationItem x)
			{
				x.IsRead = true;
			});
			object content = ((ContentControl)ParentWindow.mTopBar.mNotificationDrawerControl.mNotificationScroll).Content;
			IEnumerable<NotificationDrawerItem> source = from _ in ((IEnumerable)((Panel)((content is StackPanel) ? content : null)).Children).OfType<NotificationDrawerItem>()
				where _.Id == mNotificationItem.Id
				select _;
			if (source.Any())
			{
				source.First().ChangeToReadBackground();
			}
			ParentWindow.mTopBar.RefreshNotificationCentreButton();
			this.CloseClicked(sender, (EventArgs)(object)e);
		}
	}

	private void ApplyHoverColors(bool hover)
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		if (hover)
		{
			if (string.IsNullOrEmpty(mNotificationItem.NotificationDesignItem.HoverBorderColor))
			{
				mNotificationItem.NotificationDesignItem.HoverBorderColor = mNotificationItem.NotificationDesignItem.BorderColor;
			}
			notificationBorder.BorderBrush = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.HoverBorderColor));
			if (string.IsNullOrEmpty(mNotificationItem.NotificationDesignItem.HoverRibboncolor))
			{
				mNotificationItem.NotificationDesignItem.HoverRibboncolor = mNotificationItem.NotificationDesignItem.Ribboncolor;
			}
			((Shape)ribbonStroke).Stroke = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.HoverBorderColor));
			((Shape)ribbonBack).Fill = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.HoverRibboncolor));
			if (mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.Count == 0)
			{
				ListExtensions.ClearAddRange<SerializableKeyValuePair<string, double>>(mNotificationItem.NotificationDesignItem.HoverBackGroundGradient, mNotificationItem.NotificationDesignItem.BackgroundGradient);
			}
			((Panel)backgroundPanel).Background = (Brush)new LinearGradientBrush(new GradientStopCollection(((IEnumerable<SerializableKeyValuePair<string, double>>)mNotificationItem.NotificationDesignItem.HoverBackGroundGradient).Select((Func<SerializableKeyValuePair<string, double>, GradientStop>)((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value)))));
		}
		else
		{
			notificationBorder.BorderBrush = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.BorderColor));
			((Shape)ribbonStroke).Stroke = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.BorderColor));
			((Shape)ribbonBack).Fill = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(mNotificationItem.NotificationDesignItem.Ribboncolor));
			((Panel)backgroundPanel).Background = (Brush)new LinearGradientBrush(new GradientStopCollection(((IEnumerable<SerializableKeyValuePair<string, double>>)mNotificationItem.NotificationDesignItem.BackgroundGradient).Select((Func<SerializableKeyValuePair<string, double>, GradientStop>)((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value)))));
		}
	}

	internal void Init(GenericNotificationItem notifItem)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		mNotificationItem = notifItem;
		titleText.Text = notifItem.Title;
		titleText.Foreground = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.TitleForeGroundColor));
		messageText.Text = notifItem.Message;
		messageText.Foreground = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.MessageForeGroundColor));
		notificationBorder.BorderBrush = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
		((Shape)ribbonStroke).Stroke = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
		if (notifItem.NotificationDesignItem.BackgroundGradient.Count == 0)
		{
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF350", 0.0));
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF8AF", 0.3));
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE940", 0.6));
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FCE74E", 0.8));
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FDF09C", 0.9));
			notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE227", 1.0));
		}
		((Panel)backgroundPanel).Background = (Brush)new LinearGradientBrush(new GradientStopCollection(((IEnumerable<SerializableKeyValuePair<string, double>>)notifItem.NotificationDesignItem.BackgroundGradient).Select((Func<SerializableKeyValuePair<string, double>, GradientStop>)((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value)))));
		if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.Ribboncolor))
		{
			((Shape)ribbonBack).Fill = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF350"));
		}
		else
		{
			((Shape)ribbonBack).Fill = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.Ribboncolor));
		}
		if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.LeftGifName))
		{
			((UIElement)pikaGif).Visibility = (Visibility)2;
		}
		else
		{
			((UIElement)pikaGif).Visibility = (Visibility)0;
			pikaGif.ImageName = notifItem.NotificationDesignItem.LeftGifName;
		}
		Canvas.SetLeft((UIElement)(object)this, 0.0);
	}

	private void PikaNotificationControl_MouseLeave(object sender, MouseEventArgs e)
	{
		((UIElement)mCloseBtn).Visibility = (Visibility)1;
		ApplyHoverColors(hover: false);
	}

	private void PikaNotificationControl_MouseEnter(object sender, MouseEventArgs e)
	{
		((UIElement)mCloseBtn).Visibility = (Visibility)0;
		ApplyHoverColors(hover: true);
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Pika notification close button clicked");
		this.CloseClicked(sender, (EventArgs)(object)e);
		((RoutedEventArgs)e).Handled = true;
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		try
		{
			if (File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, pikaGif.ImageName)))
			{
				ImageSource val = (ImageSource)new BitmapImage(new Uri(Path.Combine(RegistryStrings.PromotionDirectory, pikaGif.ImageName)));
				ImageBehavior.SetAnimatedSource((Image)(object)pikaGif, val);
			}
			else if (File.Exists(Path.Combine(CustomPictureBox.AssetsDir, pikaGif.ImageName)))
			{
				ImageSource val2 = (ImageSource)new BitmapImage(new Uri(Path.Combine(CustomPictureBox.AssetsDir, pikaGif.ImageName)));
				ImageBehavior.SetAnimatedSource((Image)(object)pikaGif, val2);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while loading pika notification. " + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/pikanotificationcontrol.xaml", UriKind.Relative);
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
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
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(PikaNotificationControl)target).AddHandler(Mouse.MouseUpEvent, (Delegate)new MouseButtonEventHandler(pikanotificationcontrol_MouseUp));
			((UIElement)(PikaNotificationControl)target).MouseEnter += new MouseEventHandler(PikaNotificationControl_MouseEnter);
			((UIElement)(PikaNotificationControl)target).MouseLeave += new MouseEventHandler(PikaNotificationControl_MouseLeave);
			((FrameworkElement)(PikaNotificationControl)target).Loaded += new RoutedEventHandler(UserControl_Loaded);
			break;
		case 2:
			mNotificationGrid = (Grid)target;
			break;
		case 3:
			ribbonBack = (Path)target;
			break;
		case 4:
			ribbonStroke = (Path)target;
			break;
		case 5:
			backgroundPanel = (StackPanel)target;
			break;
		case 6:
			pikaGif = (CustomPictureBox)target;
			break;
		case 7:
			titleText = (TextBlock)target;
			break;
		case 8:
			messageText = (TextBlock)target;
			break;
		case 9:
			notificationBorder = (Border)target;
			break;
		case 10:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_MouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
