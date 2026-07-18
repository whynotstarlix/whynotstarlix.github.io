using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class DimOverlayControl : CustomWindow, IComponentConnector
{
	private IDimOverlayControl mControl;

	private ContainerWindow cw;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	private bool _contentLoaded;

	public MainWindow Owner
	{
		get
		{
			return (MainWindow)(object)((Window)this).Owner;
		}
		set
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (value != null)
			{
				if ((object)value != ((Window)this).Owner)
				{
					((Window)value).LocationChanged += ParentWindow_LocationChanged;
					((FrameworkElement)value).SizeChanged += new SizeChangedEventHandler(ParentWindow_SizeChanged);
				}
			}
			else if (((Window)this).Owner != null)
			{
				((Window)this).Owner.LocationChanged -= ParentWindow_LocationChanged;
				((FrameworkElement)((Window)this).Owner).SizeChanged -= new SizeChangedEventHandler(ParentWindow_SizeChanged);
			}
			((Window)this).Owner = (Window)(object)value;
		}
	}

	internal IDimOverlayControl Control
	{
		get
		{
			return mControl;
		}
		set
		{
			if (value != null)
			{
				AddControl(value);
			}
			mControl = value;
		}
	}

	public bool IsWindowVisible { get; set; }

	private void ParentWindow_LocationChanged(object sender, EventArgs e)
	{
		UpadteSizeLocation();
	}

	private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		UpadteSizeLocation();
	}

	public DimOverlayControl(MainWindow owner)
	{
		Owner = owner;
		InitializeComponent();
		((CustomWindow)this).IsShowGLWindow = true;
	}

	internal void AddControl(IDimOverlayControl el)
	{
		if (!el.ShowControlInSeparateWindow)
		{
			if (!((Panel)mGrid).Children.Contains((UIElement)((el is UIElement) ? el : null)))
			{
				((Panel)mGrid).Children.Add((UIElement)((el is UIElement) ? el : null));
			}
			else
			{
				((UIElement)((el is UIElement) ? el : null)).Visibility = (Visibility)0;
			}
		}
	}

	internal void RemoveControl()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		if (!Control.ShowControlInSeparateWindow)
		{
			UIElementCollection children = ((Panel)mGrid).Children;
			IDimOverlayControl control = Control;
			if (children.Contains((UIElement)((control is UIElement) ? control : null)))
			{
				UIElementCollection children2 = ((Panel)mGrid).Children;
				IDimOverlayControl control2 = Control;
				children2.Remove((UIElement)((control2 is UIElement) ? control2 : null));
				Control.Close();
			}
		}
		else
		{
			if (Control != null)
			{
				BlueStacksUIUtils.RemoveChildFromParent((UIElement)Control);
				Control.Close();
			}
			if (cw != null)
			{
				((Window)cw).Close();
			}
		}
	}

	internal void UpadteSizeLocation()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Owner != null && PresentationSource.FromVisual((Visual)(object)Owner) != null)
		{
			Point val = ((Visual)Owner).PointToScreen(new Point(0.0, 0.0));
			Matrix transformFromDevice = PresentationSource.FromVisual((Visual)(object)Owner).CompositionTarget.TransformFromDevice;
			Point val2 = ((Matrix)(ref transformFromDevice)).Transform(val);
			((Window)this).Left = ((Point)(ref val2)).X;
			((Window)this).Top = ((Point)(ref val2)).Y;
			((FrameworkElement)this).Width = ((FrameworkElement)Owner).ActualWidth;
			((FrameworkElement)this).Height = ((FrameworkElement)Owner).ActualHeight;
		}
	}

	internal void ShowWindow()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		if (IsWindowVisible)
		{
			return;
		}
		IsWindowVisible = true;
		((Window)this).Show();
		if (Control != null)
		{
			if (!Control.ShowControlInSeparateWindow)
			{
				Control.Show();
				return;
			}
			Control.Show();
			cw = new ContainerWindow(Owner, (UserControl)Control, Control.Width, Control.Height, autoHeight: false, isShow: false, Control.ShowTransparentWindow);
			((Window)cw).Show();
		}
	}

	internal void HideWindow(bool isFromOverlayClick)
	{
		if (!IsWindowVisible)
		{
			return;
		}
		if (Control != null)
		{
			if (isFromOverlayClick)
			{
				if (Control.IsCloseOnOverLayClick)
				{
					IsWindowVisible = false;
					RemoveControl();
					Hide();
				}
				else if (cw != null)
				{
					((UIElement)cw).Focus();
				}
			}
			else
			{
				IsWindowVisible = false;
				RemoveControl();
				Hide();
			}
		}
		else
		{
			IsWindowVisible = false;
			Hide();
		}
	}

	public void Hide()
	{
		if (!IsWindowVisible)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((Window)this).Hide();
			}, new object[0]);
		}
	}

	private void DimWindow_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		if ((int)e.Key == 156 && (int)e.SystemKey == 93 && Control != null && FeatureManager.Instance.IsCustomUIForNCSoft && (object)Control.GetType() == ((object)BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance).GetType() && ((UIElement)BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance).IsVisible)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
	{
		HideWindow(isFromOverlayClick: true);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dimoverlaycontrol.xaml", UriKind.Relative);
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(DimOverlayControl)target).KeyDown += new KeyEventHandler(DimWindow_KeyDown);
			break;
		case 2:
			mGrid = (Grid)target;
			break;
		case 3:
			((UIElement)(Grid)target).MouseUp += new MouseButtonEventHandler(Grid_MouseUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
