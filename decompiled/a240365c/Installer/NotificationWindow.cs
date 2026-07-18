using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Installer;

public class NotificationWindow : Window, IComponentConnector
{
	public enum AlertType
	{
		Minimal,
		Success,
		Warning,
		Error
	}

	private static double SlideOffset = 50.0;

	private static double AppearSeconds = 0.35;

	private static double DisappearSeconds = 0.25;

	private static double AutoCloseSeconds = 4.0;

	private static double MarginLeft = 20.0;

	private static double MarginBottom = 20.0;

	private readonly Window _ownerWindow;

	private readonly DispatcherTimer _closeTimer;

	internal Grid RootGrid;

	internal TranslateTransform RootTranslate;

	internal Border RootBorder;

	internal TextBlock MessageText;

	private bool _contentLoaded;

	public static void Present(string message, AlertType type, Window owner)
	{
		((Window)new NotificationWindow(message, type, owner)).Show();
	}

	private NotificationWindow(string message, AlertType type, Window ownerWindow)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		InitializeComponent();
		_ownerWindow = ownerWindow ?? throw new ArgumentNullException("ownerWindow");
		((Window)this).Owner = ownerWindow;
		((UIElement)this).Opacity = 1.0;
		((UIElement)RootGrid).Opacity = 0.0;
		RootTranslate.Y = SlideOffset;
		ApplyAlertStyle(type);
		MessageText.Text = message;
		_closeTimer = new DispatcherTimer();
		_closeTimer.Interval = TimeSpan.FromSeconds(AutoCloseSeconds);
		_closeTimer.Tick += OnCloseTimerTick;
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((DispatcherPriority)7, (Delegate)new Action(OnReadyToShow));
	}

	private void OnReadyToShow()
	{
		PositionWindow();
		PlayAppearAnimation();
		_closeTimer.Start();
	}

	private void PositionWindow()
	{
		((Window)this).Left = _ownerWindow.Left + MarginLeft;
		((Window)this).Top = _ownerWindow.Top + ((FrameworkElement)_ownerWindow).ActualHeight - ((FrameworkElement)this).ActualHeight - MarginBottom;
	}

	private void ApplyAlertStyle(AlertType type)
	{
		string text = type switch
		{
			AlertType.Success => "Alert.Success", 
			AlertType.Warning => "Alert.Warning", 
			AlertType.Error => "Alert.Error", 
			_ => "Alert.Minimal", 
		};
		object obj = ((FrameworkElement)this).Resources[(object)text];
		Style val = (Style)((obj is Style) ? obj : null);
		if (val != null)
		{
			((FrameworkElement)RootBorder).Style = val;
		}
		object obj2 = ((FrameworkElement)this).Resources[(object)(text + ".Text")];
		Style val2 = (Style)((obj2 is Style) ? obj2 : null);
		if (val2 != null)
		{
			((FrameworkElement)MessageText).Style = val2;
		}
	}

	private void OnCloseTimerTick(object sender, EventArgs e)
	{
		_closeTimer.Stop();
		PlayDisappearAnimation();
	}

	private void PlayAppearAnimation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		CubicEase val = new CubicEase();
		((EasingFunctionBase)val).EasingMode = (EasingMode)1;
		Duration duration = default(Duration);
		((Duration)(ref duration))._002Ector(TimeSpan.FromSeconds(AppearSeconds));
		DoubleAnimation val2 = new DoubleAnimation();
		val2.From = 0.0;
		val2.To = 1.0;
		((Timeline)val2).Duration = duration;
		val2.EasingFunction = (IEasingFunction)(object)val;
		DoubleAnimation val3 = new DoubleAnimation();
		val3.From = SlideOffset;
		val3.To = 0.0;
		((Timeline)val3).Duration = duration;
		val3.EasingFunction = (IEasingFunction)(object)val;
		Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)RootGrid);
		Storyboard.SetTargetProperty((DependencyObject)(object)val2, new PropertyPath((object)UIElement.OpacityProperty));
		Storyboard.SetTarget((DependencyObject)(object)val3, (DependencyObject)(object)RootTranslate);
		Storyboard.SetTargetProperty((DependencyObject)(object)val3, new PropertyPath((object)TranslateTransform.YProperty));
		Storyboard val4 = new Storyboard();
		((TimelineGroup)val4).Children.Add((Timeline)(object)val2);
		((TimelineGroup)val4).Children.Add((Timeline)(object)val3);
		val4.Begin();
	}

	private void PlayDisappearAnimation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		CubicEase val = new CubicEase();
		((EasingFunctionBase)val).EasingMode = (EasingMode)0;
		Duration duration = default(Duration);
		((Duration)(ref duration))._002Ector(TimeSpan.FromSeconds(DisappearSeconds));
		DoubleAnimation val2 = new DoubleAnimation();
		val2.From = 1.0;
		val2.To = 0.0;
		((Timeline)val2).Duration = duration;
		val2.EasingFunction = (IEasingFunction)(object)val;
		DoubleAnimation val3 = new DoubleAnimation();
		val3.From = 0.0;
		val3.To = SlideOffset;
		((Timeline)val3).Duration = duration;
		val3.EasingFunction = (IEasingFunction)(object)val;
		Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)RootGrid);
		Storyboard.SetTargetProperty((DependencyObject)(object)val2, new PropertyPath((object)UIElement.OpacityProperty));
		Storyboard.SetTarget((DependencyObject)(object)val3, (DependencyObject)(object)RootTranslate);
		Storyboard.SetTargetProperty((DependencyObject)(object)val3, new PropertyPath((object)TranslateTransform.YProperty));
		Storyboard val4 = new Storyboard();
		((TimelineGroup)val4).Children.Add((Timeline)(object)val2);
		((TimelineGroup)val4).Children.Add((Timeline)(object)val3);
		((Timeline)val4).Completed += OnDisappearCompleted;
		val4.Begin();
	}

	private void OnDisappearCompleted(object sender, EventArgs e)
	{
		((Window)this).Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Installer;component/notificationwindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			RootGrid = (Grid)target;
			break;
		case 2:
			RootTranslate = (TranslateTransform)target;
			break;
		case 3:
			RootBorder = (Border)target;
			break;
		case 4:
			MessageText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
