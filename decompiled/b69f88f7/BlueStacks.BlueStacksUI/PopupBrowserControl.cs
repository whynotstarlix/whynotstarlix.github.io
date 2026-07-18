using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mGridBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOuterGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BrowserControl mBrowser;

	private bool _contentLoaded;

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow { get; set; } = true;

	public bool ShowTransparentWindow { get; set; }

	bool IDimOverlayControl.Close()
	{
		((UIElement)this).Visibility = (Visibility)1;
		ClosePopupBrowser();
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	public PopupBrowserControl()
	{
		InitializeComponent();
	}

	public void Init(string url, string title, MainWindow window)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		mTitle.Text = title;
		mBrowser.mUrl = url;
		mBrowser.mGrid = new Grid();
		((ContentControl)mBrowser).Content = mBrowser.mGrid;
		mBrowser.CreateNewBrowser();
		if (window != null)
		{
			((FrameworkElement)window).SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
		}
		ParentWindow = window;
		mBrowser.ParentWindow = window;
		FixUpUILayout();
	}

	private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		FixUpUILayout();
	}

	private void FixUpUILayout()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if (ParentWindow.mIsFullScreen || (int)((Window)ParentWindow).WindowState == 2)
		{
			((FrameworkElement)this).Width = 880.0;
			((FrameworkElement)this).Height = 530.0;
		}
		else
		{
			((FrameworkElement)this).Width = 780.0;
			((FrameworkElement)this).Height = 480.0;
		}
	}

	public void ClosePopupBrowser()
	{
		ClientStats.SendPopupBrowserStatsInMiscASync("closed", mBrowser.mUrl);
		if (ParentWindow != null)
		{
			ParentWindow.HideDimOverlay();
		}
		if (mBrowser.CefBrowser != null)
		{
			mBrowser.DisposeBrowser();
		}
		((UIElement)this).Visibility = (Visibility)1;
	}

	private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ClosePopupBrowser();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/popupbrowsercontrol.xaml", UriKind.Relative);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mGridBorder = (Border)target;
			break;
		case 2:
			mOuterGrid = (Grid)target;
			break;
		case 3:
			mTitle = (TextBlock)target;
			break;
		case 4:
			CloseBtn = (CustomPictureBox)target;
			((UIElement)CloseBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_PreviewMouseLeftButtonUp);
			break;
		case 5:
			mGrid = (Grid)target;
			break;
		case 6:
			mBrowser = (BrowserControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
