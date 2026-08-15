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

public class DimControlWithProgresBar : UserControl, IComponentConnector
{
	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mControlGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTopBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBackButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mTitleLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mControlParentGrid;

	private bool _contentLoaded;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	public Control ParentControl { get; set; }

	public Panel ChildControl { get; set; }

	public DimControlWithProgresBar()
	{
		InitializeComponent();
	}

	internal void Init(Control parentControl, Panel childControl, bool isWindowForced, bool _)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ParentControl = parentControl;
		ChildControl = childControl;
		FixUpUILayout();
		if (isWindowForced)
		{
			((UIElement)mBackButton).Visibility = (Visibility)1;
			((UIElement)mCloseButton).Visibility = (Visibility)1;
		}
		((FrameworkElement)ParentWindow).SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
	}

	private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		FixUpUILayout();
	}

	private void FixUpUILayout()
	{
		((FrameworkElement)mControlGrid).Height = (int)(((FrameworkElement)ParentWindow.mWelcomeTab).ActualHeight * 0.8 / (double)ParentWindow.mAspectRatio.Denominator) * ParentWindow.mAspectRatio.Denominator;
		if (((FrameworkElement)ParentWindow.mWelcomeTab).ActualHeight * 0.9 - ((FrameworkElement)mControlGrid).Height > 10.0)
		{
			((FrameworkElement)mControlGrid).Height = ((FrameworkElement)ParentWindow.mWelcomeTab).ActualHeight * 0.8;
		}
		Grid obj = mControlGrid;
		((FrameworkElement)obj).Height = ((FrameworkElement)obj).Height + 50.0;
		((FrameworkElement)mControlGrid).Width = (((FrameworkElement)mControlGrid).Height - 50.0) * ParentWindow.mAspectRatio.DoubleValue + 10.0;
	}

	internal void ShowContent()
	{
		DimBackground();
		if (ChildControl != null)
		{
			if (((FrameworkElement)ChildControl).Parent != null)
			{
				DependencyObject parent = ((FrameworkElement)ChildControl).Parent;
				((Panel)((parent is Panel) ? parent : null)).Children.Remove((UIElement)(object)ChildControl);
			}
			((Panel)mControlParentGrid).Children.Add((UIElement)(object)ChildControl);
		}
		((UIElement)mControlGrid).Visibility = (Visibility)0;
	}

	private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Cicked DimControl close button");
		HideWindow();
	}

	internal void DimBackground()
	{
		Logger.Info("Diming popup window");
		if (ParentControl != null)
		{
			((UIElement)ParentControl).Visibility = (Visibility)0;
		}
		((UIElement)this).Visibility = (Visibility)0;
	}

	private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked Back Button");
		ParentWindow.mCommonHandler.BackButtonHandler();
	}

	internal void HideWindow()
	{
		Logger.Debug("Hiding popup window");
		((UIElement)this).Visibility = (Visibility)1;
		((UIElement)mControlGrid).Visibility = (Visibility)1;
		if (ParentControl != null)
		{
			((UIElement)ParentControl).Visibility = (Visibility)1;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dimcontrolwithprogresbar.xaml", UriKind.Relative);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mControlGrid = (Grid)target;
			break;
		case 2:
			mTopBar = (Grid)target;
			break;
		case 3:
			mBackButton = (CustomPictureBox)target;
			((UIElement)mBackButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BackButton_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mTitleLabel = (Label)target;
			break;
		case 5:
			mCloseButton = (CustomPictureBox)target;
			((UIElement)mCloseButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_PreviewMouseLeftButtonUp);
			break;
		case 6:
			mControlParentGrid = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
