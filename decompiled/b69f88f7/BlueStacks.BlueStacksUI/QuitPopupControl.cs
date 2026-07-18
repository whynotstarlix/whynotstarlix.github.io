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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class QuitPopupControl : UserControl, IDimOverlayControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private bool mHasSuccessfulEventOccured;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTitleGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCrossButtonPictureBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOptionsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mQuitElementStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFooterGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mReturnBlueStacksButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mCloseBlueStacksButton;

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

	public TextBlock TitleTextBlock => mTitleText;

	public CustomButton CloseBlueStacksButton => mCloseBlueStacksButton;

	public CustomButton ReturnBlueStacksButton => mReturnBlueStacksButton;

	public CustomPictureBox CrossButtonPictureBox => mCrossButtonPictureBox;

	public bool HasSuccessfulEventOccured
	{
		get
		{
			return mHasSuccessfulEventOccured;
		}
		set
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			if (value)
			{
				mHasSuccessfulEventOccured = value;
				((Panel)mTitleGrid).Background = (Brush)(SolidColorBrush)((TypeConverter)new BrushConverter()).ConvertFrom((object)"#0BA200");
			}
		}
	}

	public string CurrentPopupTag { get; set; } = string.Empty;

	bool IDimOverlayControl.Close()
	{
		Close();
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	public QuitPopupControl(MainWindow window)
	{
		ParentWindow = window;
		InitializeComponent();
	}

	private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
		ClientStats.SendLocalQuitPopupStatsAsync(CurrentPopupTag, "click_action_close");
	}

	private void CloseBlueStacksButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
		if (HasSuccessfulEventOccured)
		{
			ClientStats.SendLocalQuitPopupStatsAsync(CurrentPopupTag, "click_action_continue_bluestacks");
		}
		else
		{
			ClientStats.SendLocalQuitPopupStatsAsync(CurrentPopupTag, "popup_closed");
		}
	}

	public void AddQuitActionItem(QuitActionItem item)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		bool num = ((Panel)mQuitElementStackPanel).Children.Count != 0;
		QuitActionElement quitActionElement = new QuitActionElement(ParentWindow, this);
		((FrameworkElement)quitActionElement).Width = 210.0;
		quitActionElement.ActionElement = item;
		quitActionElement.ParentPopupTag = CurrentPopupTag;
		QuitActionElement quitActionElement2 = quitActionElement;
		if (num)
		{
			((FrameworkElement)quitActionElement2).Margin = new Thickness(32.0, 0.0, 0.0, 0.0);
		}
		((Panel)mQuitElementStackPanel).Children.Add((UIElement)(object)quitActionElement2);
	}

	internal bool Close()
	{
		try
		{
			ParentWindow.HideDimOverlay();
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close quitpopup from dimoverlay " + ex.ToString());
		}
		return false;
	}

	private void ReturnBlueStacksButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
		ClientStats.SendLocalQuitPopupStatsAsync(CurrentPopupTag, "click_action_return_bluestacks");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitpopupcontrol.xaml", UriKind.Relative);
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mParentGrid = (Grid)target;
			break;
		case 2:
			mTitleGrid = (Grid)target;
			break;
		case 3:
			mCrossButtonPictureBox = (CustomPictureBox)target;
			((UIElement)mCrossButtonPictureBox).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Close_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mTitleText = (TextBlock)target;
			break;
		case 5:
			mOptionsGrid = (Grid)target;
			break;
		case 6:
			mQuitElementStackPanel = (StackPanel)target;
			break;
		case 7:
			mFooterGrid = (Grid)target;
			break;
		case 8:
			mReturnBlueStacksButton = (CustomButton)target;
			((UIElement)mReturnBlueStacksButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ReturnBlueStacksButton_PreviewMouseLeftButtonUp);
			break;
		case 9:
			mCloseBlueStacksButton = (CustomButton)target;
			((UIElement)mCloseBlueStacksButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseBlueStacksButton_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
