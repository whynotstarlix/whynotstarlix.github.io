using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PromotionControl : UserControl, IDisposable, IComponentConnector
{
	private bool isPerformActionOnClose;

	private bool mRunPromotion;

	private double mProgress;

	private bool mForceComplete;

	internal string mActionValue;

	internal string mTextOnActionBtn;

	internal bool mIsActionButtonToShow;

	internal PromotionControl PromoControl;

	private int mBootPromotionImageTimeout;

	internal BootPromotion currentBootPromotion;

	private Thread mSliderAnimationThread;

	private int mThreadId;

	private SerializableDictionary<string, BootPromotion> dictRunningPromotions;

	internal static Dictionary<BootPromotion, int> sBootPromotionDisplayed;

	private MainWindow mMainWindow;

	internal SerializableDictionary<string, string> mExtraPayloadClicked;

	private bool disposedValue;

	internal Grid mPromotionImageGrid;

	internal CustomPictureBox mPromotionImage;

	internal CustomButton mPromoButton;

	internal Border mPromotionInfoBorder;

	internal TextBlock mPromoInfoText;

	internal CustomPictureBox mCloseButton;

	internal TextBlock BootText;

	private bool _contentLoaded;

	private Grid rootGrid;

	private Image mLoadingImage;

	private TextBlock mLabel;

	private DispatcherTimer progressTimer;

	private DispatcherTimer textAnimationTimer;

	private string[] progressTexts;

	private int textIndex;

	private double _backgroundOpacity;

	private Border card;

	private Border trackBorder;

	private Border indicatorBorder;

	private TextBlock titleText;

	private TextBlock subtitleText;

	private StackPanel dotSpinner;

	private TextBlock _percentLabel;

	private ProgressBar mProgressBar;

	public MainWindow ParentWindow { get; set; }

	public double BackgroundOpacity
	{
		get
		{
			return _backgroundOpacity;
		}
		set
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			if (value >= 0.0 && value <= 1.0)
			{
				_backgroundOpacity = value;
				byte b = (byte)(value * 255.0);
				((Panel)rootGrid).Background = (Brush)new SolidColorBrush(Color.FromArgb(b, (byte)0, (byte)0, (byte)0));
			}
		}
	}

	public PromotionControl()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_backgroundOpacity = 0.75;
		mRunPromotion = true;
		mProgress = 0.1;
		progressTimer = new DispatcherTimer();
		mBootPromotionImageTimeout = 4000;
		dictRunningPromotions = new SerializableDictionary<string, BootPromotion>();
		mExtraPayloadClicked = new SerializableDictionary<string, string>();
		try
		{
			InitializeComponent();
		}
		catch
		{
		}
		PromoControl = this;
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			if (PromotionObject.Instance == null)
			{
				PromotionObject.LoadDataFromFile();
			}
			InitUI();
			UpdateLocalizedStrings();
			LocaleStrings.SourceUpdatedEvent += OnLocaleUpdated;
			InitProgressBarTimer();
			InitTextAnimation();
		}
		((ContentControl)this).Content = rootGrid;
	}

	private void ProgressTimer_Tick(object sender, EventArgs e)
	{
		try
		{
			if (ParentWindow != null)
			{
				try
				{
					ParentWindow.mWelcomeTab?.mHomeAppManager?.InitiateHtmlSidePanel();
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		if (mProgress >= 99.0 && !mForceComplete)
		{
			UpdateProgressVisual();
			return;
		}
		if (mProgress >= 95.0 && !mForceComplete)
		{
			UpdateProgressVisual();
			mProgress += 0.025;
			return;
		}
		UpdateProgressVisual();
		mProgress += 0.75;
		if (mProgress > 100.0)
		{
			mProgress = 100.0;
		}
		if (textIndex >= progressTexts.Length - 1 && textAnimationTimer != null)
		{
			textAnimationTimer.Stop();
		}
	}

	private void SetLoadingText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (mPromoButton != null)
			{
				try
				{
					((ContentControl)mPromoButton).Content = text;
					((UIElement)mPromoButton).Visibility = (Visibility)0;
				}
				catch
				{
					subtitleText.Text = text;
				}
			}
			else
			{
				subtitleText.Text = text;
			}
		}
		else
		{
			if (mPromoButton != null)
			{
				((UIElement)mPromoButton).Visibility = (Visibility)1;
			}
			subtitleText.Text = string.Empty;
		}
		isPerformActionOnClose = false;
		if (mPromotionInfoBorder != null)
		{
			((UIElement)mPromotionInfoBorder).Visibility = (Visibility)2;
		}
	}

	internal void Stop()
	{
		if (progressTimer != null)
		{
			progressTimer.Stop();
			progressTimer.Tick -= ProgressTimer_Tick;
			progressTimer = null;
		}
		if (textAnimationTimer != null)
		{
			textAnimationTimer.Stop();
			textAnimationTimer = null;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				Stop();
			}
			disposedValue = true;
		}
	}

	~PromotionControl()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/promotioncontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			break;
		case 2:
			mPromotionImageGrid = (Grid)target;
			break;
		case 3:
			mPromotionImage = (CustomPictureBox)target;
			break;
		case 4:
			mPromoButton = (CustomButton)target;
			break;
		case 5:
			mPromotionInfoBorder = (Border)target;
			break;
		case 6:
			mPromoInfoText = (TextBlock)target;
			break;
		case 7:
			mCloseButton = (CustomPictureBox)target;
			break;
		case 8:
			break;
		case 9:
			BootText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	private void InitUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Expected O, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Expected O, but got Unknown
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Expected O, but got Unknown
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Expected O, but got Unknown
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Expected O, but got Unknown
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Expected O, but got Unknown
		rootGrid = new Grid
		{
			Background = (Brush)new SolidColorBrush(Color.FromArgb((byte)(_backgroundOpacity * 255.0), (byte)0, (byte)0, (byte)0))
		};
		RenderOptions.SetBitmapScalingMode((DependencyObject)(object)rootGrid, (BitmapScalingMode)3);
		RenderOptions.SetClearTypeHint((DependencyObject)(object)rootGrid, (ClearTypeHint)1);
		TextOptions.SetTextFormattingMode((DependencyObject)(object)rootGrid, (TextFormattingMode)1);
		((FrameworkElement)rootGrid).UseLayoutRounding = true;
		card = new Border
		{
			Width = 640.0,
			Padding = new Thickness(28.0),
			CornerRadius = new CornerRadius(12.0),
			Background = (Brush)new SolidColorBrush(Color.FromRgb((byte)24, (byte)24, (byte)24)),
			BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)60, (byte)60, (byte)60)),
			BorderThickness = new Thickness(1.0),
			Effect = (Effect)new DropShadowEffect
			{
				Color = Colors.Black,
				BlurRadius = 18.0,
				ShadowDepth = 6.0,
				Opacity = 0.5
			},
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		StackPanel val = new StackPanel
		{
			Orientation = (Orientation)1,
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)1
		};
		titleText = new TextBlock
		{
			Text = "",
			FontSize = 22.0,
			FontWeight = FontWeights.SemiBold,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)236, (byte)236, (byte)236)),
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
			HorizontalAlignment = (HorizontalAlignment)1,
			TextAlignment = (TextAlignment)2
		};
		TextOptions.SetTextRenderingMode((DependencyObject)(object)titleText, (TextRenderingMode)3);
		subtitleText = new TextBlock
		{
			Text = string.Empty,
			FontSize = 13.0,
			FontWeight = FontWeights.Normal,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)190, (byte)190, (byte)190)),
			Opacity = 0.95,
			Margin = new Thickness(0.0, 0.0, 0.0, 18.0),
			HorizontalAlignment = (HorizontalAlignment)1,
			TextAlignment = (TextAlignment)2
		};
		TextOptions.SetTextRenderingMode((DependencyObject)(object)subtitleText, (TextRenderingMode)3);
		trackBorder = new Border
		{
			Height = 12.0,
			CornerRadius = new CornerRadius(8.0),
			Background = (Brush)new SolidColorBrush(Color.FromRgb((byte)32, (byte)32, (byte)32)),
			HorizontalAlignment = (HorizontalAlignment)3,
			SnapsToDevicePixels = true
		};
		indicatorBorder = new Border
		{
			Height = 12.0,
			CornerRadius = new CornerRadius(8.0),
			Background = CreateMonoAccentBrush(),
			HorizontalAlignment = (HorizontalAlignment)0,
			Width = 0.0
		};
		Grid val2 = new Grid
		{
			HorizontalAlignment = (HorizontalAlignment)3
		};
		((Panel)val2).Children.Add((UIElement)(object)trackBorder);
		((Panel)val2).Children.Add((UIElement)(object)indicatorBorder);
		TextBlock val3 = new TextBlock
		{
			Text = "0%",
			FontSize = 14.0,
			FontWeight = FontWeights.Normal,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 12.0, 0.0, 0.0)
		};
		TextOptions.SetTextRenderingMode((DependencyObject)(object)val3, (TextRenderingMode)3);
		_percentLabel = val3;
		((Panel)val).Children.Add((UIElement)(object)titleText);
		((Panel)val).Children.Add((UIElement)(object)subtitleText);
		((Panel)val).Children.Add((UIElement)(object)val2);
		((Panel)val).Children.Add((UIElement)(object)val3);
		((Decorator)card).Child = (UIElement)(object)val;
		((Panel)rootGrid).Children.Add((UIElement)(object)card);
		((FrameworkElement)trackBorder).SizeChanged += (SizeChangedEventHandler)delegate
		{
			UpdateProgressVisual(immediate: true);
		};
	}

	private void InitProgressBarTimer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		progressTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(120.0)
		};
		progressTimer.Tick += ProgressTimer_Tick;
		progressTimer.Start();
	}

	private void InitTextAnimation()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		if (progressTexts == null || progressTexts.Length == 0)
		{
			progressTexts = new string[1] { "Loading..." };
		}
		textIndex = 0;
		subtitleText.Text = progressTexts[textIndex];
		textAnimationTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(2.2)
		};
		textAnimationTimer.Tick += delegate
		{
			if (textIndex >= progressTexts.Length - 1)
			{
				textAnimationTimer.Stop();
			}
			else
			{
				textIndex++;
				if (textIndex >= progressTexts.Length)
				{
					textIndex = 0;
				}
				AnimateSubtitleText(progressTexts[textIndex]);
			}
		};
		textAnimationTimer.Start();
	}

	private Brush CreateMonoAccentBrush()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_009a: Expected O, but got Unknown
		LinearGradientBrush val = new LinearGradientBrush
		{
			StartPoint = new Point(0.0, 0.0),
			EndPoint = new Point(1.0, 0.0)
		};
		((GradientBrush)val).GradientStops.Add(new GradientStop(Color.FromRgb((byte)210, (byte)210, (byte)210), 0.0));
		((GradientBrush)val).GradientStops.Add(new GradientStop(Color.FromRgb((byte)245, (byte)245, (byte)245), 1.0));
		return (Brush)val;
	}

	private StackPanel CreateDotSpinner()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		StackPanel val = new StackPanel
		{
			Orientation = (Orientation)0,
			Width = 36.0,
			Height = 12.0
		};
		for (int i = 0; i < 3; i++)
		{
			Ellipse val2 = new Ellipse
			{
				Width = 6.0,
				Height = 6.0,
				Fill = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
				Margin = new Thickness(3.0, 0.0, 3.0, 0.0),
				RenderTransformOrigin = new Point(0.5, 0.5)
			};
			ScaleTransform val3 = (ScaleTransform)(object)(((UIElement)val2).RenderTransform = (Transform)new ScaleTransform(1.0, 1.0));
			DoubleAnimation val5 = new DoubleAnimation
			{
				From = 0.45,
				To = 1.0,
				Duration = Duration.op_Implicit(TimeSpan.FromSeconds(0.7)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever,
				BeginTime = TimeSpan.FromMilliseconds((double)(120 * i))
			};
			((Animatable)val3).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val5);
			((Animatable)val3).BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline)(object)val5);
			((Panel)val).Children.Add((UIElement)(object)val2);
		}
		return val;
	}

	private void AnimateSubtitleText(string newText)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		if (subtitleText != null)
		{
			DoubleAnimation val = new DoubleAnimation(1.0, 0.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(220.0)));
			((Timeline)val).Completed += delegate
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Expected O, but got Unknown
				subtitleText.Text = newText;
				DoubleAnimation val2 = new DoubleAnimation(0.0, 1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(260.0)));
				((UIElement)subtitleText).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val2);
			};
			((UIElement)subtitleText).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
		}
	}

	private void UpdateProgressVisual(bool immediate = false)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00af: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		if (indicatorBorder == null || trackBorder == null)
		{
			return;
		}
		double actualWidth = ((FrameworkElement)trackBorder).ActualWidth;
		if (double.IsNaN(actualWidth) || actualWidth <= 0.0)
		{
			return;
		}
		double num = actualWidth * Math.Max(0.0, Math.Min(1.0, mProgress / 100.0));
		if (immediate)
		{
			((FrameworkElement)indicatorBorder).Width = num;
		}
		else
		{
			DoubleAnimation val = new DoubleAnimation
			{
				To = num,
				Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)),
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			((UIElement)indicatorBorder).BeginAnimation(FrameworkElement.WidthProperty, (AnimationTimeline)(object)val);
		}
		if (_percentLabel != null)
		{
			_percentLabel.Text = $"{Math.Min(100, (int)Math.Round(mProgress))}%";
		}
		if (mProgress >= 100.0)
		{
			DispatcherTimer val2 = progressTimer;
			if (val2 != null)
			{
				val2.Stop();
			}
			DispatcherTimer val3 = textAnimationTimer;
			if (val3 != null)
			{
				val3.Stop();
			}
			subtitleText.Text = LocaleStrings.GetLocalizedString("STRING_READY", "Ready!");
			DoubleAnimation val4 = new DoubleAnimation(0.0, 0.7, Duration.op_Implicit(TimeSpan.FromMilliseconds(360.0)))
			{
				AutoReverse = true,
				RepeatBehavior = new RepeatBehavior(1.0)
			};
			((UIElement)indicatorBorder).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val4);
		}
	}

	private void OnLocaleUpdated(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			UpdateLocalizedStrings();
		});
	}

	private void UpdateLocalizedStrings()
	{
		if (titleText != null)
		{
			titleText.Text = LocaleStrings.GetLocalizedString("STRING_LAUNCHING_BLUESTER", "Launching Bluester");
		}
		progressTexts = new string[5]
		{
			LocaleStrings.GetLocalizedString("STRING_LOAD_COMPONENTS", "Loading components"),
			LocaleStrings.GetLocalizedString("STRING_IMPROVING_SENSITIVITY", "Improving sensitivity"),
			LocaleStrings.GetLocalizedString("STRING_INIT_ANDROID", "Initializing Android"),
			LocaleStrings.GetLocalizedString("STRING_OPTIMIZING_THREADS", "Optimizing threads"),
			LocaleStrings.GetLocalizedString("STRING_FINAL_SETUP", "Final setup")
		};
		if (subtitleText != null && progressTexts != null && textIndex < progressTexts.Length)
		{
			if (mProgress < 100.0)
			{
				subtitleText.Text = progressTexts[textIndex];
			}
			else
			{
				subtitleText.Text = LocaleStrings.GetLocalizedString("STRING_READY", "Ready!");
			}
		}
	}
}
