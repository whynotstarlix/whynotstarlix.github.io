using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Installer;

public class MenuView : UserControl, IComponentConnector
{
	internal TranslateTransform SkullMove;

	internal Grid MenuJawRig;

	internal ScaleTransform TeethScale;

	internal TranslateTransform TeethBite;

	internal Path MenuJawBone;

	internal Path MenuJawMouthHole;

	internal Canvas MenuTeethCanvas;

	internal Rectangle MenuToothLeftCenter;

	internal Rectangle MenuToothCenter;

	internal Rectangle MenuToothRightCenter;

	internal Rectangle MenuToothRightMid;

	private bool _contentLoaded;

	public event EventHandler DisableHyperV11Requested;

	public event EventHandler DisableHyperV10Requested;

	public event EventHandler InstallRequested;

	public MenuView()
	{
		InitializeComponent();
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ca: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0135: Expected O, but got Unknown
		DoubleAnimation val = new DoubleAnimation
		{
			From = 0.0,
			To = -6.0,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(2.4)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new SineEase
			{
				EasingMode = (EasingMode)2
			}
		};
		DoubleAnimation val2 = new DoubleAnimation
		{
			From = 0.0,
			To = -4.0,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(0.85)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)2
			}
		};
		DoubleAnimation val3 = new DoubleAnimation
		{
			From = 1.0,
			To = 0.92,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(0.85)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)2
			}
		};
		((Animatable)SkullMove).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)(object)val);
		((Animatable)TeethBite).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)(object)val2);
		((Animatable)TeethScale).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val3);
	}

	private void DisableHyperV11_Click(object sender, RoutedEventArgs e)
	{
		this.DisableHyperV11Requested?.Invoke(this, EventArgs.Empty);
	}

	private void DisableHyperV10_Click(object sender, RoutedEventArgs e)
	{
		this.DisableHyperV10Requested?.Invoke(this, EventArgs.Empty);
	}

	private void InstallEmulator_Click(object sender, RoutedEventArgs e)
	{
		this.InstallRequested?.Invoke(this, EventArgs.Empty);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Installer;component/menuview.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(MenuView)target).Loaded += new RoutedEventHandler(UserControl_Loaded);
			break;
		case 2:
			SkullMove = (TranslateTransform)target;
			break;
		case 3:
			MenuJawRig = (Grid)target;
			break;
		case 4:
			TeethScale = (ScaleTransform)target;
			break;
		case 5:
			TeethBite = (TranslateTransform)target;
			break;
		case 6:
			MenuJawBone = (Path)target;
			break;
		case 7:
			MenuJawMouthHole = (Path)target;
			break;
		case 8:
			MenuTeethCanvas = (Canvas)target;
			break;
		case 9:
			MenuToothLeftCenter = (Rectangle)target;
			break;
		case 10:
			MenuToothCenter = (Rectangle)target;
			break;
		case 11:
			MenuToothRightCenter = (Rectangle)target;
			break;
		case 12:
			MenuToothRightMid = (Rectangle)target;
			break;
		case 13:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(DisableHyperV11_Click);
			break;
		case 14:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(DisableHyperV10_Click);
			break;
		case 15:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(InstallEmulator_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
