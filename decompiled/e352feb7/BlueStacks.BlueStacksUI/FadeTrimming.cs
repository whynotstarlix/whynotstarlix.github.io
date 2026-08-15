using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI;

public static class FadeTrimming
{
	private class Fader
	{
		private readonly TextBlock _textBlock;

		private bool _isAttached;

		private LinearGradientBrush _brush;

		private Brush _opacityMask;

		private bool _isClipped;

		private bool _verticalFadingEnabled;

		public Fader(TextBlock textBlock)
		{
			_textBlock = textBlock;
		}

		public void Attach()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Invalid comparison between Unknown and I4
			DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)_textBlock);
			FrameworkElement val = (FrameworkElement)(object)((parent is FrameworkElement) ? parent : null);
			if (val != null && !_isAttached)
			{
				val.SizeChanged += new SizeChangedEventHandler(UpdateForegroundBrush);
				((FrameworkElement)_textBlock).SizeChanged += new SizeChangedEventHandler(UpdateForegroundBrush);
				_opacityMask = ((UIElement)_textBlock).OpacityMask;
				if (_verticalFadingEnabled || (int)_textBlock.TextWrapping == 1)
				{
					_textBlock.TextTrimming = (TextTrimming)0;
				}
				UpdateForegroundBrush(_textBlock, EventArgs.Empty);
				_isAttached = true;
			}
		}

		public void Detach()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			((FrameworkElement)_textBlock).SizeChanged -= new SizeChangedEventHandler(UpdateForegroundBrush);
			DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)_textBlock);
			FrameworkElement val = (FrameworkElement)(object)((parent is FrameworkElement) ? parent : null);
			if (val != null)
			{
				val.SizeChanged -= new SizeChangedEventHandler(UpdateForegroundBrush);
			}
			((UIElement)_textBlock).OpacityMask = _opacityMask;
			_isAttached = false;
		}

		public void ToggleVerticalFading(bool newValue)
		{
			_verticalFadingEnabled = newValue;
			UpdateForegroundBrush(_textBlock, EventArgs.Empty);
		}

		private void UpdateForegroundBrush(object sender, EventArgs e)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Invalid comparison between Unknown and I4
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Invalid comparison between Unknown and I4
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			Geometry layoutClip = LayoutInformation.GetLayoutClip((FrameworkElement)(object)_textBlock);
			Rect bounds;
			int num;
			if (layoutClip != null)
			{
				if ((int)_textBlock.TextWrapping == 1)
				{
					bounds = layoutClip.Bounds;
					if (((Rect)(ref bounds)).Width > 0.0)
					{
						bounds = layoutClip.Bounds;
						if (((Rect)(ref bounds)).Width < ((FrameworkElement)_textBlock).ActualWidth)
						{
							num = 1;
							goto IL_00b7;
						}
					}
				}
				if (_verticalFadingEnabled && (int)_textBlock.TextWrapping == (0x7149 ^ 0x714B))
				{
					bounds = layoutClip.Bounds;
					if (((Rect)(ref bounds)).Height > 0.0)
					{
						bounds = layoutClip.Bounds;
						num = ((((Rect)(ref bounds)).Height < ((FrameworkElement)_textBlock).ActualHeight) ? 1 : 0);
						goto IL_00b7;
					}
				}
				num = 0;
			}
			else
			{
				num = 0;
			}
			goto IL_00b7;
			IL_00b7:
			bool flag = (byte)num != 0;
			if (_isClipped && !flag)
			{
				((UIElement)_textBlock).OpacityMask = _opacityMask;
				_brush = null;
				_isClipped = false;
			}
			if (flag)
			{
				bounds = layoutClip.Bounds;
				double width = ((Rect)(ref bounds)).Width;
				bounds = layoutClip.Bounds;
				double height = ((Rect)(ref bounds)).Height;
				bool flag2 = (int)_textBlock.TextWrapping == -337496293 + 1969962175 % 1632465880;
				if (_brush == null)
				{
					_brush = (flag2 ? GetVerticalClipBrush(height) : GetHorizontalClipBrush(width));
					((UIElement)_textBlock).OpacityMask = (Brush)(object)_brush;
				}
				else if (flag2 && VerticalBrushNeedsUpdating(_brush, height))
				{
					_brush.EndPoint = new Point(0.0, height);
					((GradientBrush)_brush).GradientStops[1].Offset = (height - 20.0) / height;
				}
				else if (!flag2 && HorizontalBrushNeedsUpdating(_brush, width))
				{
					_brush.EndPoint = new Point(width, 0.0);
					((GradientBrush)_brush).GradientStops[1].Offset = (width - 10.0) / width;
				}
				_isClipped = true;
			}
		}

		private LinearGradientBrush GetHorizontalClipBrush(double visibleWidth)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			//IL_00c1: Expected O, but got Unknown
			LinearGradientBrush val = new LinearGradientBrush
			{
				MappingMode = (BrushMappingMode)0,
				StartPoint = new Point(0.0, 0.0),
				EndPoint = new Point(visibleWidth, 0.0)
			};
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Black,
				Offset = 0.0
			});
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Black,
				Offset = (visibleWidth - 10.0) / visibleWidth
			});
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 1.0
			});
			return val;
		}

		private LinearGradientBrush GetVerticalClipBrush(double visibleHeight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			//IL_00c1: Expected O, but got Unknown
			LinearGradientBrush val = new LinearGradientBrush
			{
				MappingMode = (BrushMappingMode)0,
				StartPoint = new Point(0.0, 0.0),
				EndPoint = new Point(0.0, visibleHeight)
			};
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Black,
				Offset = 0.0
			});
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Black,
				Offset = (visibleHeight - 20.0) / visibleHeight
			});
			((GradientBrush)val).GradientStops.Add(new GradientStop
			{
				Color = Colors.Transparent,
				Offset = 1.0
			});
			return val;
		}
	}

	private const double Epsilon = 1E-05;

	private const double FadeWidth = 10.0;

	private const double FadeHeight = 20.0;

	public static readonly DependencyProperty IsEnabledProperty;

	private static readonly DependencyProperty FaderProperty;

	public static readonly DependencyProperty IsVerticalFadingEnabledProperty;

	public static bool GetIsEnabled(DependencyObject obj)
	{
		return (bool)((obj != null) ? obj.GetValue(IsEnabledProperty) : null);
	}

	public static void SetIsEnabled(DependencyObject obj, bool value)
	{
		if (obj != null)
		{
			obj.SetValue(IsEnabledProperty, (object)value);
		}
	}

	public static void SetIsVerticalFadingEnabled(DependencyObject obj, bool value)
	{
		if (obj != null)
		{
			obj.SetValue(IsVerticalFadingEnabledProperty, (object)value);
		}
	}

	private static Fader GetFader(DependencyObject obj)
	{
		return (Fader)obj.GetValue(FaderProperty);
	}

	private static void SetFader(DependencyObject obj, Fader value)
	{
		obj.SetValue(FaderProperty, (object)value);
	}

	private static void HandleVerticalFadingEnabled(DependencyObject source, DependencyPropertyChangedEventArgs e)
	{
		TextBlock val = (TextBlock)(object)((source is TextBlock) ? source : null);
		if (val != null)
		{
			GetFader((DependencyObject)(object)val)?.ToggleVerticalFading((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}

	private static void HandleIsEnabledChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		TextBlock val = (TextBlock)(object)((source is TextBlock) ? source : null);
		if (val == null)
		{
			return;
		}
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue)
		{
			Fader fader = GetFader((DependencyObject)(object)val);
			if (fader != null)
			{
				fader.Detach();
				SetFader((DependencyObject)(object)val, null);
			}
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(HandleTextBlockLoaded);
			((FrameworkElement)val).Unloaded -= new RoutedEventHandler(HandleTextBlockUnloaded);
		}
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((FrameworkElement)val).Loaded += new RoutedEventHandler(HandleTextBlockLoaded);
			((FrameworkElement)val).Unloaded += new RoutedEventHandler(HandleTextBlockUnloaded);
			Fader fader2 = new Fader(val);
			SetFader((DependencyObject)(object)val, fader2);
			fader2.Attach();
		}
	}

	private static void HandleTextBlockUnloaded(object sender, RoutedEventArgs e)
	{
		DependencyObject val = (DependencyObject)((sender is DependencyObject) ? sender : null);
		if (val != null)
		{
			GetFader(val)?.Detach();
		}
	}

	private static void HandleTextBlockLoaded(object sender, RoutedEventArgs e)
	{
		DependencyObject val = (DependencyObject)((sender is DependencyObject) ? sender : null);
		if (val != null)
		{
			GetFader(val)?.Attach();
		}
	}

	private static bool HorizontalBrushNeedsUpdating(LinearGradientBrush brush, double visibleWidth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Point endPoint = brush.EndPoint;
		if (!(((Point)(ref endPoint)).X < visibleWidth - 1E-05))
		{
			endPoint = brush.EndPoint;
			return ((Point)(ref endPoint)).X > visibleWidth + 1E-05;
		}
		return true;
	}

	private static bool VerticalBrushNeedsUpdating(LinearGradientBrush brush, double visibleHeight)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Point endPoint = brush.EndPoint;
		if (!(((Point)(ref endPoint)).Y < visibleHeight - 1E-05))
		{
			endPoint = brush.EndPoint;
			return ((Point)(ref endPoint)).Y > visibleHeight + 1E-05;
		}
		return true;
	}

	static FadeTrimming()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FadeTrimming), new PropertyMetadata((object)false, new PropertyChangedCallback(HandleIsEnabledChanged)));
		FaderProperty = DependencyProperty.RegisterAttached("Fader", typeof(Fader), typeof(FadeTrimming), new PropertyMetadata((PropertyChangedCallback)null));
		IsVerticalFadingEnabledProperty = DependencyProperty.RegisterAttached("IsVerticalFadingEnabledProperty", typeof(bool), typeof(FadeTrimming), new PropertyMetadata((object)false, new PropertyChangedCallback(HandleVerticalFadingEnabled)));
	}
}
