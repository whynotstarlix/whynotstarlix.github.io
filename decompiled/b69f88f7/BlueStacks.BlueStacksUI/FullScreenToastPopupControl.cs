using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class FullScreenToastPopupControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mToastPopupBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DockPanel mToastPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTipTextblock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mKeyBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mKeyTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mInfoTextblock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mToastIcon;

	private bool _contentLoaded;

	public FullScreenToastPopupControl()
	{
		InitializeComponent();
	}

	public FullScreenToastPopupControl(MainWindow window)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		InitializeComponent();
		if (window != null)
		{
			ParentWindow = window;
			Grid val = new Grid();
			object content = ((ContentControl)window).Content;
			((ContentControl)window).Content = val;
			((Panel)val).Children.Add((UIElement)((content is UIElement) ? content : null));
			((Panel)val).Children.Add((UIElement)(object)this);
		}
	}

	public void Init(MainWindow window, string text)
	{
		if (window != null)
		{
			ParentWindow = window;
			((FrameworkElement)mToastPanel).MaxWidth = ((FrameworkElement)ParentWindow).ActualWidth - 15.0;
		}
		mTipTextblock.Text = text;
		((UIElement)mInfoTextblock).Visibility = (Visibility)2;
		((UIElement)mKeyBorder).Visibility = (Visibility)2;
	}

	public void Init(MainWindow window, string tip, string key, string info)
	{
		if (window != null)
		{
			ParentWindow = window;
			((FrameworkElement)mToastPanel).MaxWidth = ((FrameworkElement)ParentWindow).ActualWidth - 15.0;
		}
		mTipTextblock.Text = tip;
		mKeyTextBlock.Text = key;
		mInfoTextblock.Text = info;
		((UIElement)mInfoTextblock).Visibility = (Visibility)0;
		((UIElement)mKeyBorder).Visibility = (Visibility)0;
	}

	public void ShowPopup(double seconds = 4.0)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)this).Visibility = (Visibility)0;
		((UIElement)this).Opacity = 0.0;
		DoubleAnimation val = new DoubleAnimation
		{
			From = 0.0,
			To = seconds,
			Duration = new Duration(TimeSpan.FromSeconds(0.3))
		};
		Storyboard val2 = new Storyboard();
		((TimelineGroup)val2).Children.Add((Timeline)(object)val);
		Storyboard.SetTarget((DependencyObject)(object)val, (DependencyObject)(object)this);
		Storyboard.SetTargetProperty((DependencyObject)(object)val, new PropertyPath((object)UIElement.OpacityProperty));
		((Timeline)val2).Completed += delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			((UIElement)this).Visibility = (Visibility)0;
			DoubleAnimation val3 = new DoubleAnimation
			{
				From = seconds,
				To = 0.0,
				FillBehavior = (FillBehavior)1,
				BeginTime = TimeSpan.FromSeconds(seconds),
				Duration = new Duration(TimeSpan.FromSeconds(seconds / 2.0))
			};
			Storyboard val4 = new Storyboard();
			((TimelineGroup)val4).Children.Add((Timeline)(object)val3);
			Storyboard.SetTarget((DependencyObject)(object)val3, (DependencyObject)(object)this);
			Storyboard.SetTargetProperty((DependencyObject)(object)val3, new PropertyPath((object)UIElement.OpacityProperty));
			((Timeline)val4).Completed += delegate
			{
				((UIElement)this).Visibility = (Visibility)2;
			};
			val4.Begin();
		};
		val2.Begin();
	}

	private void ToastIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.CloseFullScreenToastAndStopTimer();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/fullscreentoastpopupcontrol.xaml", UriKind.Relative);
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mToastPopupBorder = (Border)target;
			break;
		case 2:
			mMaskBorder = (Border)target;
			break;
		case 3:
			mToastPanel = (DockPanel)target;
			break;
		case 4:
			mTipTextblock = (TextBlock)target;
			break;
		case 5:
			mKeyBorder = (Border)target;
			break;
		case 6:
			mKeyTextBlock = (TextBlock)target;
			break;
		case 7:
			mInfoTextblock = (TextBlock)target;
			break;
		case 8:
			mToastIcon = (CustomPictureBox)target;
			((UIElement)mToastIcon).MouseLeftButtonUp += new MouseButtonEventHandler(ToastIcon_MouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
