using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SidebarElement : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SidebarElement mSidebarElement;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Ellipse mElementNotification;

	private bool _contentLoaded;

	public CustomPictureBox Image => mImage;

	public bool IsLastElementOfGroup { get; set; }

	public bool IsCurrentLastElementOfGroup { get; set; }

	public bool IsInMainSidebar { get; set; } = true;

	public string mSidebarElementTooltipKey { get; set; } = string.Empty;

	public SidebarElement()
	{
		InitializeComponent();
	}

	private void SidebarElement_Loaded(object sender, RoutedEventArgs e)
	{
		SetColor();
	}

	private void MImage_Loaded(object sender, RoutedEventArgs e)
	{
		mImage = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
	}

	private void SetColor(bool isPressed = false)
	{
		if (isPressed)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BorderBrushProperty, "SidebarElementClick");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BackgroundProperty, "SidebarElementClick");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.ForegroundProperty, "SidebarElementClick");
		}
		else if (((UIElement)this).IsMouseOver)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BorderBrushProperty, "SidebarElementHover");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BackgroundProperty, "SidebarElementHover");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.ForegroundProperty, "SidebarElementHover");
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BorderBrushProperty, "SidebarElementNormal");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BackgroundProperty, "SidebarElementNormal");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.ForegroundProperty, "SidebarElementNormal");
		}
	}

	private void SidebarElement_MouseEnter(object sender, MouseEventArgs e)
	{
		SetColor();
	}

	private void SidebarElement_MouseLeave(object sender, MouseEventArgs e)
	{
		SetColor();
	}

	private void SidebarElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		SetColor(isPressed: true);
	}

	private void SidebarElement_IsEnabledChanged(object _, DependencyPropertyChangedEventArgs e)
	{
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)this).Opacity = 1.0;
		}
		else
		{
			((UIElement)this).Opacity = 0.5;
		}
	}

	private void SidebarElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		SetColor();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebarelement.xaml", UriKind.Relative);
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
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mSidebarElement = (SidebarElement)target;
			((UIElement)mSidebarElement).MouseEnter += new MouseEventHandler(SidebarElement_MouseEnter);
			((UIElement)mSidebarElement).MouseLeave += new MouseEventHandler(SidebarElement_MouseLeave);
			((UIElement)mSidebarElement).PreviewMouseDown += new MouseButtonEventHandler(SidebarElement_PreviewMouseDown);
			((UIElement)mSidebarElement).PreviewMouseUp += new MouseButtonEventHandler(SidebarElement_PreviewMouseUp);
			((FrameworkElement)mSidebarElement).Loaded += new RoutedEventHandler(SidebarElement_Loaded);
			((UIElement)mSidebarElement).IsEnabledChanged += new DependencyPropertyChangedEventHandler(SidebarElement_IsEnabledChanged);
			break;
		case 2:
			mBorder = (Border)target;
			break;
		case 3:
			mGrid = (Grid)target;
			break;
		case 4:
			mImage = (CustomPictureBox)target;
			((FrameworkElement)mImage).Loaded += new RoutedEventHandler(MImage_Loaded);
			break;
		case 5:
			mElementNotification = (Ellipse)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
