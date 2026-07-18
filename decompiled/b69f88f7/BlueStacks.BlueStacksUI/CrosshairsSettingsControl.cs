using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;
using Bluester;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class CrosshairsSettingsControl : UserControl
{
	private MainWindow ParentWindow;

	private Canvas previewCanvas;

	private Rectangle topLine;

	private Rectangle bottomLine;

	private Rectangle leftLine;

	private Rectangle rightLine;

	private Rectangle centerDot;

	private Slider thicknessSlider;

	private Slider lengthSlider;

	private Slider gapSlider;

	private Slider opacitySlider;

	private CheckBox tShapeCheckBox;

	private Button colorButton;

	private Color crosshairColor;

	private CheckBox centerDotCheckBox;

	private Button applyButton;

	private CrosshairWindow crosshairWindow;

	private bool crosshairExists;

	private CheckBox crossCheckBox;

	private Button copyButton;

	private Button pasteButton;

	private CheckBox showCrosshairCheckBox;

	private CrosshairConfig currentConfig;

	private readonly SynchronizationContext context;

	public CrosshairsSettingsControl(MainWindow window)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		SynchronizationContext current = SynchronizationContext.Current;
		context = current;
		crosshairColor = Colors.LimeGreen;
		ParentWindow = window;
		((FrameworkElement)this).Width = 530.0;
		((FrameworkElement)this).Height = 500.0;
		((UIElement)this).Visibility = (Visibility)1;
		LocaleStrings.SourceUpdatedEvent += OnLocaleUpdated;
		RebuildUI();
	}

	private UIElement CreateLayout()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Expected O, but got Unknown
		//IL_029e: Expected O, but got Unknown
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Expected O, but got Unknown
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Expected O, but got Unknown
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Expected O, but got Unknown
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Expected O, but got Unknown
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Expected O, but got Unknown
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Expected O, but got Unknown
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Expected O, but got Unknown
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Expected O, but got Unknown
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Expected O, but got Unknown
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Expected O, but got Unknown
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Expected O, but got Unknown
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Expected O, but got Unknown
		Grid val = new Grid
		{
			Margin = new Thickness(20.0, 10.0, 20.0, 10.0)
		};
		val.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		val.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		val.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		val.RowDefinitions.Add(new RowDefinition
		{
			Height = new GridLength(95.0)
		});
		val.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		TextBlock val2 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_CROSSHAIR_SETTINGS", "Crosshair Settings"),
			FontSize = 20.0,
			Foreground = (Brush)(object)Brushes.White,
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 20.0, 0.0, 10.0)
		};
		((Panel)val).Children.Add((UIElement)(object)val2);
		Grid.SetRow((UIElement)(object)val2, 0);
		Grid val3 = new Grid();
		val3.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		val3.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, (GridUnitType)2)
		});
		StackPanel val4 = new StackPanel
		{
			Orientation = (Orientation)1,
			Margin = new Thickness(0.0, 0.0, 20.0, 0.0)
		};
		double num = 130.0;
		previewCanvas = new Canvas
		{
			Width = num,
			Height = num,
			Background = (Brush)new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte)10, (byte)10, (byte)10)),
			ClipToBounds = true
		};
		Border val5 = new Border
		{
			BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)60, (byte)60, (byte)60)),
			BorderThickness = new Thickness(1.0),
			CornerRadius = new CornerRadius(5.0),
			Child = (UIElement)(object)previewCanvas,
			Effect = (Effect)new DropShadowEffect
			{
				Color = Colors.Black,
				BlurRadius = 10.0,
				ShadowDepth = 2.0,
				Opacity = 0.4
			}
		};
		((Panel)val4).Children.Add((UIElement)(object)val5);
		colorButton = CreateStyledButtons(LocaleStrings.GetLocalizedString("STRING_COLOR", "Color"), new RoutedEventHandler(ColorButton_Click), num, 30.0, (Thickness?)new Thickness(0.0, 10.0, 0.0, 0.0));
		((Panel)val4).Children.Add((UIElement)(object)colorButton);
		((Panel)val3).Children.Add((UIElement)(object)val4);
		Grid.SetColumn((UIElement)(object)val4, 0);
		StackPanel val6 = new StackPanel
		{
			VerticalAlignment = (VerticalAlignment)0
		};
		thicknessSlider = CreateSliderBlock((Panel)(object)val6, LocaleStrings.GetLocalizedString("STRING_THICKNESS", "Thickness"), 1.0, 10.0, 2.5, 0.5);
		lengthSlider = CreateSliderBlock((Panel)(object)val6, LocaleStrings.GetLocalizedString("STRING_LENGTH", "Length"), 1.0, 50.0, 15.0, 1.0);
		gapSlider = CreateSliderBlock((Panel)(object)val6, LocaleStrings.GetLocalizedString("STRING_GAP", "Gap"), 0.0, 30.0, 5.0, 1.0);
		opacitySlider = CreateSliderBlock((Panel)(object)val6, LocaleStrings.GetLocalizedString("STRING_OPACITY", "Opacity"), 0.1, 1.0, 1.0, 0.05);
		((Panel)val3).Children.Add((UIElement)(object)val6);
		Grid.SetColumn((UIElement)(object)val6, 1);
		((Panel)val).Children.Add((UIElement)(object)val3);
		Grid.SetRow((UIElement)(object)val3, 1);
		UniformGrid val7 = new UniformGrid
		{
			Columns = 2,
			Margin = new Thickness(0.0, 10.0, 0.0, 0.0)
		};
		showCrosshairCheckBox = (CheckBox)(object)GenerateCheckBox(LocaleStrings.GetLocalizedString("STRING_ENABLE_CROSSHAIR", "Enable Crosshair"), isChecked: true);
		((Control)showCrosshairCheckBox).FontWeight = FontWeights.Bold;
		centerDotCheckBox = (CheckBox)(object)GenerateCheckBox(LocaleStrings.GetLocalizedString("STRING_CENTER_DOT", "Center Dot"), isChecked: true);
		crossCheckBox = (CheckBox)(object)GenerateCheckBox(LocaleStrings.GetLocalizedString("STRING_CROSS_LINES", "Cross Lines"), isChecked: true);
		tShapeCheckBox = (CheckBox)(object)GenerateCheckBox(LocaleStrings.GetLocalizedString("STRING_T_SHAPE", "T-Shape"), isChecked: false);
		((Panel)val7).Children.Add((UIElement)(object)showCrosshairCheckBox);
		((Panel)val7).Children.Add((UIElement)(object)centerDotCheckBox);
		((Panel)val7).Children.Add((UIElement)(object)crossCheckBox);
		((Panel)val7).Children.Add((UIElement)(object)tShapeCheckBox);
		((ToggleButton)showCrosshairCheckBox).Checked += new RoutedEventHandler(ShowCrosshairCheckBox_Changed);
		((ToggleButton)showCrosshairCheckBox).Unchecked += new RoutedEventHandler(ShowCrosshairCheckBox_Changed);
		((Panel)val).Children.Add((UIElement)(object)val7);
		Grid.SetRow((UIElement)(object)val7, 2);
		Grid val8 = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 0.0)
		};
		val8.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, (GridUnitType)2)
		});
		val8.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		StackPanel val9 = new StackPanel
		{
			Orientation = (Orientation)0
		};
		copyButton = CreateStyledButtons(LocaleStrings.GetLocalizedString("STRING_COPY", "Copy"), new RoutedEventHandler(CopyButton_Click), 130.0, 30.0, (Thickness?)new Thickness(0.0, 0.0, 10.0, 0.0));
		((Control)copyButton).Background = (Brush)new SolidColorBrush(Color.FromRgb((byte)45, (byte)45, (byte)48));
		pasteButton = CreateStyledButtons(LocaleStrings.GetLocalizedString("STRING_PASTE", "Paste"), new RoutedEventHandler(PasteButton_Click), 110.0, 30.0, (Thickness?)new Thickness(0.0, 0.0, 0.0, 0.0));
		((Control)pasteButton).Background = (Brush)new SolidColorBrush(Color.FromRgb((byte)45, (byte)45, (byte)48));
		((Panel)val9).Children.Add((UIElement)(object)copyButton);
		((Panel)val9).Children.Add((UIElement)(object)pasteButton);
		((Panel)val8).Children.Add((UIElement)(object)val9);
		Grid.SetColumn((UIElement)(object)val9, 0);
		string text = LocaleStrings.GetLocalizedString("STRING_APPLY", "APPLY").ToUpper();
		applyButton = CreateStyledButtons(text, new RoutedEventHandler(ApplyButton_Click), 140.0, 35.0, (Thickness?)new Thickness(0.0));
		((Control)applyButton).FontWeight = FontWeights.Bold;
		((Control)applyButton).FontSize = 14.0;
		((Panel)val8).Children.Add((UIElement)(object)applyButton);
		Grid.SetColumn((UIElement)(object)applyButton, 1);
		((Panel)val).Children.Add((UIElement)(object)val8);
		Grid.SetRow((UIElement)(object)val8, 4);
		InitializePreview();
		UpdatePreview();
		return (UIElement)val;
	}

	private void InitializePreview()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		topLine = new Rectangle();
		bottomLine = new Rectangle();
		leftLine = new Rectangle();
		rightLine = new Rectangle();
		centerDot = new Rectangle();
		((Panel)previewCanvas).Children.Add((UIElement)(object)topLine);
		((Panel)previewCanvas).Children.Add((UIElement)(object)bottomLine);
		((Panel)previewCanvas).Children.Add((UIElement)(object)leftLine);
		((Panel)previewCanvas).Children.Add((UIElement)(object)rightLine);
		((Panel)previewCanvas).Children.Add((UIElement)(object)centerDot);
	}

	private void UpdatePreview()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		if (previewCanvas != null)
		{
			double value = ((RangeBase)thicknessSlider).Value;
			double value2 = ((RangeBase)lengthSlider).Value;
			double value3 = ((RangeBase)gapSlider).Value;
			double value4 = ((RangeBase)opacitySlider).Value;
			bool valueOrDefault = ((ToggleButton)tShapeCheckBox).IsChecked == true;
			bool valueOrDefault2 = ((ToggleButton)centerDotCheckBox).IsChecked == true;
			bool valueOrDefault3 = ((ToggleButton)crossCheckBox).IsChecked == true;
			double num = ((FrameworkElement)previewCanvas).Width / 2.0;
			double num2 = ((FrameworkElement)previewCanvas).Height / 2.0;
			SolidColorBrush fill = new SolidColorBrush(crosshairColor)
			{
				Opacity = value4
			};
			if (valueOrDefault3)
			{
				((UIElement)topLine).Visibility = (Visibility)(valueOrDefault ? 2 : 0);
				((UIElement)bottomLine).Visibility = (Visibility)0;
				((UIElement)leftLine).Visibility = (Visibility)0;
				((UIElement)rightLine).Visibility = (Visibility)0;
				((FrameworkElement)topLine).Width = value;
				((FrameworkElement)topLine).Height = value2;
				((Shape)topLine).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)topLine, num - value / 2.0);
				Canvas.SetTop((UIElement)(object)topLine, num2 - value3 - value2);
				((FrameworkElement)bottomLine).Width = value;
				((FrameworkElement)bottomLine).Height = value2;
				((Shape)bottomLine).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)bottomLine, num - value / 2.0);
				Canvas.SetTop((UIElement)(object)bottomLine, num2 + value3);
				((FrameworkElement)leftLine).Width = value2;
				((FrameworkElement)leftLine).Height = value;
				((Shape)leftLine).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)leftLine, num - value3 - value2);
				Canvas.SetTop((UIElement)(object)leftLine, num2 - value / 2.0);
				((FrameworkElement)rightLine).Width = value2;
				((FrameworkElement)rightLine).Height = value;
				((Shape)rightLine).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)rightLine, num + value3);
				Canvas.SetTop((UIElement)(object)rightLine, num2 - value / 2.0);
				((UIElement)centerDot).Visibility = (Visibility)((!valueOrDefault2) ? 2 : 0);
				((FrameworkElement)centerDot).Width = value;
				((FrameworkElement)centerDot).Height = value;
				((Shape)centerDot).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)centerDot, num - value / 2.0);
				Canvas.SetTop((UIElement)(object)centerDot, num2 - value / 2.0);
			}
			else
			{
				((UIElement)topLine).Visibility = (Visibility)2;
				((UIElement)bottomLine).Visibility = (Visibility)2;
				((UIElement)leftLine).Visibility = (Visibility)2;
				((UIElement)rightLine).Visibility = (Visibility)2;
				((UIElement)centerDot).Visibility = (Visibility)0;
				((FrameworkElement)centerDot).Width = value;
				((FrameworkElement)centerDot).Height = value;
				((Shape)centerDot).Fill = (Brush)(object)fill;
				Canvas.SetLeft((UIElement)(object)centerDot, num - value / 2.0);
				Canvas.SetTop((UIElement)(object)centerDot, num2 - value / 2.0);
			}
		}
	}

	private void ColorButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ColorDialog val = new ColorDialog();
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			Color color = val.Color;
			crosshairColor = Color.FromArgb(color.A, color.R, color.G, color.B);
			UpdatePreview();
		}
	}

	private Slider CreateSliderBlock(Panel parent, string label, double min, double max, double value, double tick)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		Grid val = new Grid();
		((FrameworkElement)val).Margin = new Thickness(0.0, 8.0, 0.0, 8.0);
		val.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(100.0)
		});
		val.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, (GridUnitType)2)
		});
		TextBlock val2 = new TextBlock
		{
			Text = label,
			Foreground = (Brush)(object)Brushes.LightGray,
			FontSize = 13.0,
			VerticalAlignment = (VerticalAlignment)1
		};
		Slider val3 = new Slider
		{
			Minimum = min,
			Maximum = max,
			Value = value,
			TickFrequency = tick,
			IsSnapToTickEnabled = true,
			VerticalAlignment = (VerticalAlignment)1,
			Style = CreateModernSliderStyle()
		};
		((RangeBase)val3).ValueChanged += delegate
		{
			UpdatePreview();
		};
		((Panel)val).Children.Add((UIElement)(object)val2);
		Grid.SetColumn((UIElement)(object)val2, 0);
		((Panel)val).Children.Add((UIElement)(object)val3);
		Grid.SetColumn((UIElement)(object)val3, 1);
		parent.Children.Add((UIElement)(object)val);
		return val3;
	}

	private CheckBox CreateCheckBox(Panel parent, string content, bool isChecked = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		CheckBox val = new CheckBox
		{
			Content = content,
			Foreground = (Brush)(object)Brushes.LightGray,
			FontSize = 13.0,
			Margin = new Thickness(0.0, 8.0, 0.0, 8.0),
			IsChecked = isChecked,
			Style = CreateModernCheckBoxStyle()
		};
		((ToggleButton)val).Checked += (RoutedEventHandler)delegate
		{
			UpdatePreview();
		};
		((ToggleButton)val).Unchecked += (RoutedEventHandler)delegate
		{
			UpdatePreview();
		};
		parent.Children.Add((UIElement)(object)val);
		return val;
	}

	private Style CreateModernSliderStyle()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		Style val = new Style(typeof(Slider));
		string text = "\r\n        <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'\r\n                         TargetType='{x:Type Slider}'>\r\n            <Grid VerticalAlignment='Center'>\r\n                <Border x:Name='TrackBackground' \r\n                        Height='4' \r\n                        CornerRadius='2' \r\n                        Background='#555' />\r\n\r\n                <Border x:Name='PART_SelectionRange'\r\n                        Height='4'\r\n                        CornerRadius='2'\r\n                        Background='#007ACC'\r\n                        HorizontalAlignment='Left' />\r\n\r\n                <Track x:Name='PART_Track'>\r\n                    <Track.Thumb>\r\n                        <Thumb x:Name='Thumb'>\r\n                            <Thumb.Template>\r\n                                <ControlTemplate TargetType='{x:Type Thumb}'>\r\n                                    <Grid>\r\n                                        <Ellipse Width='16' \r\n                                                 Height='16' \r\n                                                 Fill='White' \r\n                                                 Stroke='#DDD' \r\n                                                 StrokeThickness='1' />\r\n                                    </Grid>\r\n                                </ControlTemplate>\r\n                            </Thumb.Template>\r\n                        </Thumb>\r\n                    </Track.Thumb>\r\n                </Track>\r\n            </Grid>\r\n        </ControlTemplate>";
		try
		{
			ControlTemplate val2 = (ControlTemplate)XamlReader.Parse(text);
			((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new Setter(Control.TemplateProperty, (object)val2));
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error parsing Slider template: " + ex.Message);
		}
		return val;
	}

	private Style CreateModernCheckBoxStyle()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected O, but got Unknown
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_02f6: Expected O, but got Unknown
		Style val = new Style(typeof(CheckBox));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new Setter(Control.ForegroundProperty, (object)Brushes.LightGray));
		ControlTemplate val2 = new ControlTemplate(typeof(CheckBox));
		FrameworkElementFactory val3 = new FrameworkElementFactory(typeof(StackPanel));
		val3.SetValue(StackPanel.OrientationProperty, (object)(Orientation)0);
		FrameworkElementFactory val4 = new FrameworkElementFactory(typeof(Border), "CheckBoxBorder");
		val4.SetValue(FrameworkElement.WidthProperty, (object)18.0);
		val4.SetValue(FrameworkElement.HeightProperty, (object)18.0);
		val4.SetValue(Border.BorderBrushProperty, (object)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555")));
		val4.SetValue(Border.BorderThicknessProperty, (object)new Thickness(2.0));
		val4.SetValue(Border.CornerRadiusProperty, (object)new CornerRadius(3.0));
		val4.SetValue(Border.BackgroundProperty, (object)Brushes.Transparent);
		FrameworkElementFactory val5 = new FrameworkElementFactory(typeof(Path), "CheckMark");
		val5.SetValue(Path.DataProperty, (object)Geometry.Parse("M 0 6 L 6 12 L 14 0"));
		val5.SetValue(Shape.StrokeProperty, (object)Brushes.White);
		val5.SetValue(Shape.StrokeThicknessProperty, (object)2.5);
		val5.SetValue(UIElement.VisibilityProperty, (object)(Visibility)2);
		val4.AppendChild(val5);
		val3.AppendChild(val4);
		FrameworkElementFactory val6 = new FrameworkElementFactory(typeof(ContentPresenter));
		val6.SetValue(FrameworkElement.MarginProperty, (object)new Thickness(8.0, 0.0, 0.0, 0.0));
		val6.SetValue(FrameworkElement.VerticalAlignmentProperty, (object)(VerticalAlignment)1);
		val3.AppendChild(val6);
		((FrameworkTemplate)val2).VisualTree = val3;
		Trigger val7 = new Trigger
		{
			Property = ToggleButton.IsCheckedProperty,
			Value = true
		};
		((Collection<SetterBase>)(object)val7.Setters).Add((SetterBase)new Setter(UIElement.VisibilityProperty, (object)(Visibility)0, "CheckMark"));
		((Collection<SetterBase>)(object)val7.Setters).Add((SetterBase)new Setter(Border.BackgroundProperty, (object)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")), "CheckBoxBorder"));
		((Collection<SetterBase>)(object)val7.Setters).Add((SetterBase)new Setter(Border.BorderBrushProperty, (object)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")), "CheckBoxBorder"));
		((Collection<TriggerBase>)(object)val2.Triggers).Add((TriggerBase)(object)val7);
		Trigger val8 = new Trigger
		{
			Property = UIElement.IsMouseOverProperty,
			Value = true
		};
		((Collection<SetterBase>)(object)val8.Setters).Add((SetterBase)new Setter(Border.BorderBrushProperty, (object)Brushes.White, "CheckBoxBorder"));
		((Collection<TriggerBase>)(object)val2.Triggers).Add((TriggerBase)(object)val8);
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new Setter(Control.TemplateProperty, (object)val2));
		return val;
	}

	private void ApplyButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		double value = ((RangeBase)thicknessSlider).Value;
		double value2 = ((RangeBase)lengthSlider).Value;
		double value3 = ((RangeBase)gapSlider).Value;
		double value4 = ((RangeBase)opacitySlider).Value;
		bool valueOrDefault = ((ToggleButton)tShapeCheckBox).IsChecked == true;
		bool valueOrDefault2 = ((ToggleButton)centerDotCheckBox).IsChecked == true;
		bool valueOrDefault3 = ((ToggleButton)crossCheckBox).IsChecked == true;
		bool valueOrDefault4 = ((ToggleButton)showCrosshairCheckBox).IsChecked == true;
		if (CrosshairManager.CurrentCrosshair == null)
		{
			if (valueOrDefault4)
			{
				CrosshairManager.ApplyCrosshair(crosshairColor, value, value2, value3, value4, valueOrDefault, valueOrDefault2, valueOrDefault3);
			}
		}
		else
		{
			CrosshairManager.CurrentCrosshair.UpdatePosition(value, value2, value3, valueOrDefault, valueOrDefault2, valueOrDefault3);
			CrosshairManager.CurrentCrosshair.UpdateColor(crosshairColor, value4);
		}
		if (CrosshairManager.CurrentCrosshair != null)
		{
			if (valueOrDefault4)
			{
				((Window)CrosshairManager.CurrentCrosshair).Show();
			}
			else
			{
				((Window)CrosshairManager.CurrentCrosshair).Hide();
			}
		}
		SaveCurrentConfigAsString();
	}

	private CustomCheckbox GenerateCheckBox(string name, bool isChecked)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		CustomCheckbox val = new CustomCheckbox();
		if (val.Image != null)
		{
			((FrameworkElement)val.Image).Height = 14.0;
			((FrameworkElement)val.Image).Width = 14.0;
		}
		((ContentControl)val).Content = name;
		((FrameworkElement)val).Height = 20.0;
		((FrameworkElement)val).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
		((ToggleButton)val).IsChecked = isChecked;
		((ToggleButton)val).Checked += (RoutedEventHandler)delegate
		{
			UpdatePreview();
		};
		((ToggleButton)val).Unchecked += (RoutedEventHandler)delegate
		{
			UpdatePreview();
		};
		return val;
	}

	private void CopyButton_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(Base64Helper.Encode(JsonConvert.SerializeObject((object)new CrosshairConfig
		{
			Color = ((object)Unsafe.As<Color, Color>(ref crosshairColor)/*cast due to constrained. prefix*/).ToString(),
			Thickness = ((RangeBase)thicknessSlider).Value,
			Length = ((RangeBase)lengthSlider).Value,
			Gap = ((RangeBase)gapSlider).Value,
			Opacity = ((RangeBase)opacitySlider).Value,
			TShape = (((ToggleButton)tShapeCheckBox).IsChecked == true),
			ShowCenterDot = (((ToggleButton)centerDotCheckBox).IsChecked == true),
			ShowCross = (((ToggleButton)crossCheckBox).IsChecked == true)
		})));
		ShowNotification(LocaleStrings.GetLocalizedString("STRING_CROSSHAIR_COPIED", "Crosshair copied!"));
	}

	private void PasteButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Clipboard.ContainsText())
			{
				CrosshairConfig crosshairConfig = JsonConvert.DeserializeObject<CrosshairConfig>(Base64Helper.Decode(Clipboard.GetText()));
				if (crosshairConfig != null)
				{
					crosshairColor = (Color)ColorConverter.ConvertFromString(crosshairConfig.Color);
					((RangeBase)thicknessSlider).Value = crosshairConfig.Thickness;
					((RangeBase)lengthSlider).Value = crosshairConfig.Length;
					((RangeBase)gapSlider).Value = crosshairConfig.Gap;
					((RangeBase)opacitySlider).Value = crosshairConfig.Opacity;
					((ToggleButton)tShapeCheckBox).IsChecked = crosshairConfig.TShape;
					((ToggleButton)centerDotCheckBox).IsChecked = crosshairConfig.ShowCenterDot;
					((ToggleButton)crossCheckBox).IsChecked = crosshairConfig.ShowCross;
					UpdatePreview();
					ShowNotification(LocaleStrings.GetLocalizedString("STRING_CROSSHAIR_PASTED", "Crosshair pasted!"));
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_CROSSHAIR_PASTE_ERROR", "Make sure you copied the crosshair code for our emulator.") + " " + ex.Message);
		}
	}

	public Button CreateStyledButtons(string text, RoutedEventHandler onClick, double width = 100.0, double height = 30.0, Thickness? margin = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		CustomButton val = new CustomButton
		{
			Content = text,
			Width = width,
			Height = height,
			Margin = (Thickness)(((_003F?)margin) ?? new Thickness(8.0)),
			ButtonColor = (ButtonColors)4,
			Cursor = Cursors.Hand
		};
		((ButtonBase)val).Click += onClick;
		return (Button)val;
	}

	private void ShowCrosshairCheckBox_Changed(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)showCrosshairCheckBox).IsChecked == true)
		{
			if (CrosshairManager.CurrentCrosshair != null)
			{
				((Window)CrosshairManager.CurrentCrosshair).Show();
			}
			else
			{
				ApplyButton_Click(null, null);
			}
			return;
		}
		CrosshairWindow currentCrosshair = CrosshairManager.CurrentCrosshair;
		if (currentCrosshair != null)
		{
			((Window)currentCrosshair).Hide();
		}
	}

	private void SaveCurrentConfigAsString()
	{
		if (currentConfig == null)
		{
			currentConfig = new CrosshairConfig();
		}
		currentConfig.Color = ((object)Unsafe.As<Color, Color>(ref crosshairColor)/*cast due to constrained. prefix*/).ToString();
		currentConfig.Thickness = ((RangeBase)thicknessSlider).Value;
		currentConfig.Length = ((RangeBase)lengthSlider).Value;
		currentConfig.Gap = ((RangeBase)gapSlider).Value;
		currentConfig.Opacity = ((RangeBase)opacitySlider).Value;
		currentConfig.TShape = ((ToggleButton)tShapeCheckBox).IsChecked == true;
		currentConfig.ShowCenterDot = ((ToggleButton)centerDotCheckBox).IsChecked == true;
		currentConfig.ShowCross = ((ToggleButton)crossCheckBox).IsChecked == true;
		CrosshairStorage.LastCrosshairConfig = Base64Helper.Encode(JsonConvert.SerializeObject((object)currentConfig));
	}

	public void LoadConfigFromString()
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		string lastCrosshairConfig = CrosshairStorage.LastCrosshairConfig;
		if (string.IsNullOrEmpty(lastCrosshairConfig))
		{
			return;
		}
		try
		{
			CrosshairConfig crosshairConfig = JsonConvert.DeserializeObject<CrosshairConfig>(Base64Helper.Decode(lastCrosshairConfig));
			if (crosshairConfig != null)
			{
				currentConfig = crosshairConfig;
				crosshairColor = (Color)ColorConverter.ConvertFromString(crosshairConfig.Color);
				((RangeBase)thicknessSlider).Value = crosshairConfig.Thickness;
				((RangeBase)lengthSlider).Value = crosshairConfig.Length;
				((RangeBase)gapSlider).Value = crosshairConfig.Gap;
				((RangeBase)opacitySlider).Value = crosshairConfig.Opacity;
				((ToggleButton)tShapeCheckBox).IsChecked = crosshairConfig.TShape;
				((ToggleButton)centerDotCheckBox).IsChecked = crosshairConfig.ShowCenterDot;
				((ToggleButton)crossCheckBox).IsChecked = crosshairConfig.ShowCross;
				UpdatePreview();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error loading crosshair config: " + ex.Message);
		}
	}

	private void ShowNotification(string msg)
	{
		context.Post(delegate
		{
			((Control)new NotificationForm(msg)).Show();
		}, null);
		Thread.Sleep(500);
	}

	private void OnLocaleUpdated(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			RebuildUI();
		});
	}

	private void RebuildUI()
	{
		((ContentControl)this).Content = CreateLayout();
		LoadConfigFromString();
	}
}
