using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI;

public class CrosshairWindow : Window
{
	private Rectangle topLine;

	private Rectangle bottomLine;

	private Rectangle leftLine;

	private Rectangle rightLine;

	private Rectangle centerDot;

	public void UpdateColor(Color color, double opacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		SolidColorBrush fill = new SolidColorBrush(color)
		{
			Opacity = opacity
		};
		((Shape)topLine).Fill = (Brush)(object)fill;
		((Shape)bottomLine).Fill = (Brush)(object)fill;
		((Shape)leftLine).Fill = (Brush)(object)fill;
		((Shape)rightLine).Fill = (Brush)(object)fill;
		((Shape)centerDot).Fill = (Brush)(object)fill;
	}

	public CrosshairWindow(Color color, double thickness, double length, double gap, double opacity, bool tShape, bool showCenterDot, bool showCross)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		((FrameworkElement)this).Width = SystemParameters.PrimaryScreenWidth;
		((FrameworkElement)this).Height = SystemParameters.PrimaryScreenHeight;
		((Window)this).Left = 0.0;
		((Window)this).Top = 0.0;
		((Window)this).WindowStartupLocation = (WindowStartupLocation)0;
		((Window)this).WindowStyle = (WindowStyle)0;
		((Window)this).AllowsTransparency = true;
		((Control)this).Background = (Brush)(object)Brushes.Transparent;
		((Window)this).Topmost = true;
		((Window)this).ShowInTaskbar = false;
		((UIElement)this).Focusable = false;
		Canvas val = (Canvas)(((ContentControl)this).Content = (object)new Canvas());
		SolidColorBrush fill = new SolidColorBrush(color)
		{
			Opacity = opacity
		};
		topLine = new Rectangle
		{
			Fill = (Brush)(object)fill
		};
		bottomLine = new Rectangle
		{
			Fill = (Brush)(object)fill
		};
		leftLine = new Rectangle
		{
			Fill = (Brush)(object)fill
		};
		rightLine = new Rectangle
		{
			Fill = (Brush)(object)fill
		};
		centerDot = new Rectangle
		{
			Fill = (Brush)(object)fill
		};
		((Panel)val).Children.Add((UIElement)(object)topLine);
		((Panel)val).Children.Add((UIElement)(object)bottomLine);
		((Panel)val).Children.Add((UIElement)(object)leftLine);
		((Panel)val).Children.Add((UIElement)(object)rightLine);
		((Panel)val).Children.Add((UIElement)(object)centerDot);
		UpdatePosition(thickness, length, gap, tShape, showCenterDot, showCross);
	}

	public void UpdatePosition(double thickness, double length, double gap, bool tShape, bool showCenterDot, bool showCross)
	{
		double num = SystemParameters.PrimaryScreenWidth / 2.0;
		double num2 = SystemParameters.PrimaryScreenHeight / 2.0;
		if (!showCross)
		{
			((UIElement)topLine).Visibility = (Visibility)2;
			((UIElement)bottomLine).Visibility = (Visibility)2;
			((UIElement)leftLine).Visibility = (Visibility)2;
			((UIElement)rightLine).Visibility = (Visibility)2;
		}
		else
		{
			((UIElement)topLine).Visibility = (Visibility)(tShape ? 2 : 0);
			((FrameworkElement)topLine).Width = thickness;
			((FrameworkElement)topLine).Height = length;
			Canvas.SetLeft((UIElement)(object)topLine, num - thickness / 2.0);
			Canvas.SetTop((UIElement)(object)topLine, num2 - gap - length);
			((FrameworkElement)bottomLine).Width = thickness;
			((FrameworkElement)bottomLine).Height = length;
			Canvas.SetLeft((UIElement)(object)bottomLine, num - thickness / 2.0);
			Canvas.SetTop((UIElement)(object)bottomLine, num2 + gap);
			((FrameworkElement)leftLine).Width = length;
			((FrameworkElement)leftLine).Height = thickness;
			Canvas.SetLeft((UIElement)(object)leftLine, num - gap - length);
			Canvas.SetTop((UIElement)(object)leftLine, num2 - thickness / 2.0);
			((FrameworkElement)rightLine).Width = length;
			((FrameworkElement)rightLine).Height = thickness;
			Canvas.SetLeft((UIElement)(object)rightLine, num + gap);
			Canvas.SetTop((UIElement)(object)rightLine, num2 - thickness / 2.0);
		}
		((FrameworkElement)centerDot).Width = thickness;
		((FrameworkElement)centerDot).Height = thickness;
		Canvas.SetLeft((UIElement)(object)centerDot, num - thickness / 2.0);
		Canvas.SetTop((UIElement)(object)centerDot, num2 - thickness / 2.0);
		((UIElement)centerDot).Visibility = (Visibility)((!showCenterDot) ? 2 : 0);
	}
}
