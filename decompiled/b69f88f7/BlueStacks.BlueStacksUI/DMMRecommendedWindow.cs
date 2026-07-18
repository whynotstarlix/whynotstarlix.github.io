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
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class DMMRecommendedWindow : CustomWindow, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BrowserControl mRecommendedBrowserControl;

	private bool _contentLoaded;

	public DMMRecommendedWindow(MainWindow window)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		InitializeComponent();
		((CustomWindow)this).IsShowGLWindow = true;
		ParentWindow = window;
		((Window)this).Owner = (Window)(object)ParentWindow;
		((Window)this).Topmost = false;
		((Window)this).Left = ((ParentWindow != null) ? ((Window)ParentWindow).Left : 0.0) + ((ParentWindow != null) ? ((FrameworkElement)ParentWindow).ActualWidth : 0.0);
		((Window)this).Top = ((ParentWindow != null) ? ((Window)ParentWindow).Top : 0.0);
		((FrameworkElement)this).Height = ((ParentWindow != null) ? ((FrameworkElement)ParentWindow).Height : 0.0);
		((FrameworkElement)this).Width = (((FrameworkElement)this).Height - (double)(((ParentWindow != null) ? ParentWindow.ParentWindowHeightDiff : 0) * 9)) / 16.0 + (double)((ParentWindow != null) ? ParentWindow.ParentWindowWidthDiff : 0);
		((Window)this).Closing += RecommendedWindow_Closing;
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(RecommendedWindow_IsVisibleChanged);
	}

	private void RecommendedWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0)
		{
			ParentWindow.mDmmBottomBar.mRecommendedWindowBtn.ImageName = "recommend_click";
		}
		else
		{
			ParentWindow.mDmmBottomBar.mRecommendedWindowBtn.ImageName = "recommend";
		}
		UpdateSize();
	}

	private void RecommendedWindow_Closing(object sender, CancelEventArgs e)
	{
		ParentWindow.mDMMRecommendedWindow.mRecommendedBrowserControl.DisposeBrowser();
		ParentWindow.mDMMRecommendedWindow = null;
	}

	public void Init(string url)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		mRecommendedBrowserControl.mUrl = url;
		mRecommendedBrowserControl.mGrid = new Grid();
		((ContentControl)mRecommendedBrowserControl).Content = mRecommendedBrowserControl.mGrid;
		mRecommendedBrowserControl.CreateNewBrowser();
		mRecommendedBrowserControl.ProcessMessageRecieved += new ProcessMessageEventHandler(MRecommendedBrowserControl_ProcessMessageRecieved);
	}

	public void UpdateSize()
	{
		((Window)this).Top = ((Window)ParentWindow).Top;
		((Window)this).Left = ((Window)ParentWindow).Left + ((FrameworkElement)ParentWindow).Width;
		((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow).Height;
		((FrameworkElement)this).Width = (((FrameworkElement)ParentWindow).Height - (double)ParentWindow.ParentWindowHeightDiff) * 9.0 / 16.0 + (double)ParentWindow.ParentWindowWidthDiff;
	}

	public void UpdateLocation()
	{
		((Window)this).Top = ((Window)ParentWindow).Top;
		((Window)this).Left = ((Window)ParentWindow).Left + ((FrameworkElement)ParentWindow).Width;
	}

	private void MRecommendedBrowserControl_ProcessMessageRecieved(object sender, ProcessMessageEventArgs e)
	{
	}

	private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mIsDMMRecommendedWindowOpen = false;
		((Window)this).Hide();
		InteropWindow.ShowWindow(ParentWindow.Handle, 9);
		if (!((Window)ParentWindow).Topmost)
		{
			((Window)ParentWindow).Topmost = true;
			((Window)ParentWindow).Topmost = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/dmmrecommendedwindow.xaml", UriKind.Relative);
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mCloseBtn_PreviewMouseLeftButtonUp);
			break;
		case 2:
			mRecommendedBrowserControl = (BrowserControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
