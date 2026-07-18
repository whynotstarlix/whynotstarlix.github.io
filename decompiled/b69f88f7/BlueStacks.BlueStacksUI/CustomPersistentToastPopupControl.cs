using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class CustomPersistentToastPopupControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mPersistentToastPopupBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mPersistentToastTextblock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mCloseSettingsPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mCloseSettingsPopupBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder1;

	private bool _contentLoaded;

	public CustomPersistentToastPopupControl()
	{
		InitializeComponent();
	}

	public CustomPersistentToastPopupControl(Window window)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		InitializeComponent();
		if (window != null)
		{
			Grid val = new Grid();
			object content = ((ContentControl)window).Content;
			((ContentControl)window).Content = val;
			((Panel)val).Children.Add((UIElement)((content is UIElement) ? content : null));
			((Panel)val).Children.Add((UIElement)(object)this);
		}
	}

	public bool Init(MainWindow window, string text)
	{
		ParentWindow = window;
		if (ParentWindow != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled && RegistryManager.Instance.IsShootingModeTooltipVisible)
		{
			((UIElement)this).Visibility = (Visibility)0;
			mPersistentToastTextblock.Text = text;
			((FrameworkElement)mPersistentToastPopupBorder).HorizontalAlignment = (HorizontalAlignment)1;
			((FrameworkElement)mPersistentToastPopupBorder).VerticalAlignment = (VerticalAlignment)1;
			((UIElement)this).UpdateLayout();
			return true;
		}
		return false;
	}

	private void MCloseIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mCloseSettingsPopup).IsOpen = true;
	}

	private void Grid_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FFFFFF"));
	}

	private void Grid_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
	}

	private void mNeverShowAgain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mCloseSettingsPopup).IsOpen = false;
		((UIElement)this).Visibility = (Visibility)2;
		RegistryManager.Instance.IsShootingModeTooltipVisible = false;
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mCloseSettingsPopup).IsOpen = false;
		((UIElement)this).Visibility = (Visibility)2;
		ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/custompersistenttoastpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mPersistentToastPopupBorder = (Border)target;
			break;
		case 2:
			mCloseIcon = (CustomPictureBox)target;
			((UIElement)mCloseIcon).MouseLeftButtonUp += new MouseButtonEventHandler(MCloseIcon_MouseLeftButtonUp);
			break;
		case 3:
			mPersistentToastTextblock = (TextBlock)target;
			break;
		case 4:
			mCloseSettingsPopup = (CustomPopUp)target;
			break;
		case 5:
			dummyGrid = (Grid)target;
			break;
		case 6:
			mCloseSettingsPopupBorder = (Border)target;
			break;
		case 7:
			mMaskBorder1 = (Border)target;
			break;
		case 8:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)(Grid)target).MouseLeftButtonUp += new MouseButtonEventHandler(mNeverShowAgain_MouseLeftButtonUp);
			break;
		case 9:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)(Grid)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
