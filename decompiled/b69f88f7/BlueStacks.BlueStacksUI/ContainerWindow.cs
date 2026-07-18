using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ContainerWindow : CustomWindow, IComponentConnector
{
	public static readonly DependencyProperty CustomCornerRadiusProperty = DependencyProperty.Register("CustomCornerRadius", typeof(CornerRadius), typeof(CustomWindow), new PropertyMetadata((object)new CornerRadius(-1.0)));

	private bool IsCustomCornerRadius;

	public static readonly DependencyProperty CustomBorderBrushProperty = DependencyProperty.Register("CustomBorderBrush", typeof(Brush), typeof(Border), new PropertyMetadata((object)Brushes.Transparent));

	private bool IsCustomBorderBrush;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mShadowBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mOuterBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ContentGrid;

	private bool _contentLoaded;

	public CornerRadius CustomCornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CustomCornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CustomCornerRadiusProperty, (object)value);
			IsCustomCornerRadius = true;
		}
	}

	public Brush CustomBorderBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(CustomBorderBrushProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CustomBorderBrushProperty, (object)value);
			IsCustomBorderBrush = true;
		}
	}

	public ContainerWindow(MainWindow window, UserControl control, double width, double height, bool autoHeight = false, bool isShow = true, bool isWindowTransparent = false, double radius = -1.0, Brush brush = null)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Invalid comparison between Unknown and I4
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		ContainerWindow containerWindow = this;
		InitializeComponent();
		((Window)this).Closing += delegate(object o, CancelEventArgs e)
		{
			containerWindow.ClosingContainerWindow(o, e, control);
		};
		if (radius != -1.0)
		{
			CustomCornerRadius = new CornerRadius(radius);
		}
		if (brush != null)
		{
			CustomBorderBrush = brush;
		}
		if (!isWindowTransparent)
		{
			SetShadowBorder();
			SetOuterBorder();
			SetMaskGrid();
		}
		if (autoHeight)
		{
			((FrameworkElement)this).Width = width + (double)((!isWindowTransparent) ? 64 : 0);
			((Window)this).SizeToContent = (SizeToContent)2;
		}
		else
		{
			((FrameworkElement)this).Width = width + (double)((!isWindowTransparent) ? 64 : 0);
			((FrameworkElement)this).Height = height + (double)((!isWindowTransparent) ? 64 : 0);
		}
		((Window)this).Owner = (Window)(object)window;
		if (window == null)
		{
			return;
		}
		if (window.mDMMRecommendedWindow != null && (int)((UIElement)window.mDMMRecommendedWindow).Visibility == 0 && window.IsUIInPortraitMode)
		{
			double num = (((FrameworkElement)window).Width + ((FrameworkElement)window.mDMMRecommendedWindow).Width - ((FrameworkElement)this).Width) / 2.0 + ((Window)window).Left;
			double num2 = (((FrameworkElement)window).Height - ((FrameworkElement)this).Height) / 2.0 + ((Window)window).Top;
			double num3 = num + ((FrameworkElement)this).Width;
			double num4 = num2 + ((FrameworkElement)this).Height;
			if (num > 0.0 && num3 < SystemParameters.PrimaryScreenWidth && num2 > 0.0 && num4 < SystemParameters.PrimaryScreenHeight)
			{
				((Window)this).Left = num;
				((Window)this).Top = num2;
			}
			else
			{
				((Window)this).WindowStartupLocation = (WindowStartupLocation)2;
			}
		}
		else if ((int)((Window)window).WindowState == 1)
		{
			((Window)this).WindowStartupLocation = (WindowStartupLocation)1;
		}
		else
		{
			((Window)this).WindowStartupLocation = (WindowStartupLocation)2;
		}
		((Panel)ContentGrid).Children.Add((UIElement)(object)control);
		if (isShow)
		{
			if (window != null)
			{
				window.ShowDimOverlay();
				((Window)this).Owner = (Window)(object)window.mDimOverlay;
			}
			((Window)this).ShowDialog();
			window?.HideDimOverlay();
		}
	}

	private void ClosingContainerWindow(object _1, CancelEventArgs _2, UserControl control)
	{
		if (control is IDimOverlayControl dimOverlayControl)
		{
			dimOverlayControl.Close();
		}
		if (control != null)
		{
			((Panel)ContentGrid).Children.Remove((UIElement)(object)control);
		}
	}

	private void SetShadowBorder()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		((UIElement)mShadowBorder).SnapsToDevicePixels = true;
		mShadowBorder.BorderThickness = new Thickness(1.0);
		((FrameworkElement)mShadowBorder).Margin = new Thickness(30.0);
		((DependencyObject)mShadowBorder).SetValue(RenderOptions.EdgeModeProperty, (object)(EdgeMode)1);
		if (IsCustomCornerRadius)
		{
			mShadowBorder.CornerRadius = CustomCornerRadius;
		}
		else
		{
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)mShadowBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
		}
		DropShadowEffect val = new DropShadowEffect();
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val, DropShadowEffect.ColorProperty, "PopupShadowColor");
		val.Direction = 270.0;
		val.ShadowDepth = 0.0;
		val.BlurRadius = 15.0;
		val.Opacity = 0.7;
		((UIElement)mShadowBorder).Effect = (Effect)(object)val;
	}

	private void SetOuterBorder()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		mOuterBorder.BorderThickness = new Thickness(1.0);
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mOuterBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
		if (IsCustomBorderBrush)
		{
			mOuterBorder.BorderBrush = CustomBorderBrush;
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mOuterBorder, Control.BorderBrushProperty, "PopupBorderBrush");
		}
		if (IsCustomCornerRadius)
		{
			mOuterBorder.CornerRadius = CustomCornerRadius;
		}
		else
		{
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)mOuterBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
		}
	}

	private void SetMaskGrid()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		((UIElement)mMaskBorder).SnapsToDevicePixels = true;
		((DependencyObject)mMaskBorder).SetValue(RenderOptions.EdgeModeProperty, (object)(EdgeMode)1);
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mMaskBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
		if (IsCustomCornerRadius)
		{
			mMaskBorder.CornerRadius = CustomCornerRadius;
		}
		else
		{
			BlueStacksUIBinding.BindCornerRadius((DependencyObject)(object)mMaskBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
		}
		VisualBrush opacityMask = new VisualBrush((Visual)(object)mMaskBorder)
		{
			Stretch = (Stretch)0
		};
		((UIElement)mMainGrid).OpacityMask = (Brush)(object)opacityMask;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/containerwindow.xaml", UriKind.Relative);
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mShadowBorder = (Border)target;
			break;
		case 2:
			mOuterBorder = (Border)target;
			break;
		case 3:
			mMainGrid = (Grid)target;
			break;
		case 4:
			mMaskBorder = (Border)target;
			break;
		case 5:
			ContentGrid = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
