using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class TopbarOptions : UserControl, IComponentConnector
{
	private bool mIsLoadedOnce;

	private bool mIsInFullscreenMode;

	private readonly object mSyncRoot = new object();

	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid TopMenu;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition GameGuideColumn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mFullScreenTextBlock;

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

	public TopbarOptions()
	{
		InitializeComponent();
	}

	private void Topbar_Loaded(object sender, RoutedEventArgs e)
	{
		if (!mIsLoadedOnce)
		{
			mIsLoadedOnce = true;
			BindEvents();
		}
		SetLabel();
	}

	public void SetLabel()
	{
		mFullScreenTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_FULL_SCREEN", "") + " (" + ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP") + ")";
	}

	internal void BindEvents()
	{
		ParentWindow.FullScreenChanged += ParentWindow_FullScreenChangedEvent;
	}

	private void ParentWindow_FullScreenChangedEvent(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		lock (mSyncRoot)
		{
			mIsInFullscreenMode = args.IsFullscreen;
			if (!mIsInFullscreenMode)
			{
				((Popup)ParentWindow.mFullscreenTopbarPopupButton).IsOpen = false;
				((Popup)ParentWindow.mFullscreenTopbarPopup).IsOpen = false;
			}
			else
			{
				GameGuideColumn.Width = ((ParentWindow.SelectedConfig != null && ParentWindow.SelectedConfig.SelectedControlScheme != null && ParentWindow.SelectedConfig.SelectedControlScheme.GameControls != null && ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any((IMAction action) => action.Guidance.Any())) ? new GridLength(1.0, (GridUnitType)2) : new GridLength(0.0, (GridUnitType)2));
			}
		}
	}

	internal void HideTopBarInFullscreen()
	{
		((Popup)ParentWindow.mFullscreenTopbarPopupButton).IsOpen = false;
		((Popup)ParentWindow.mFullscreenTopbarPopup).IsOpen = false;
	}

	internal void ToggleTopbarButtonVisibilityInFullscreen(bool isVisible)
	{
		if (isVisible && !((Popup)ParentWindow.mFullscreenTopbarPopup).IsOpen && !((Popup)ParentWindow.mFullscreenSidebarPopup).IsOpen)
		{
			((FrameworkElement)ParentWindow.mFullscreenTopbarPopupButtonInnerGrid).Width = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth;
			((FrameworkElement)ParentWindow.mFullscreenTopbarPopupButton).Width = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth;
			((Popup)ParentWindow.mFullscreenTopbarPopupButton).VerticalOffset = 0.0 - ((FrameworkElement)ParentWindow.MainGrid).ActualHeight / 2.0;
			((Popup)ParentWindow.mFullscreenTopbarPopupButton).IsOpen = mIsInFullscreenMode;
		}
		else if (!isVisible)
		{
			((Popup)ParentWindow.mFullscreenTopbarPopupButton).IsOpen = false;
		}
	}

	internal void ToggleTopbarVisibilityInFullscreen(bool isVisible)
	{
		if (isVisible)
		{
			((FrameworkElement)ParentWindow.mFullscreenTopbarPopup).Width = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth;
			((Popup)ParentWindow.mFullscreenTopbarPopup).VerticalOffset = 0.0 - ((FrameworkElement)ParentWindow.MainGrid).ActualHeight / 2.0;
			((FrameworkElement)ParentWindow.mFullscreenTopbarPopupInnerGrid).Width = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth;
			ClientStats.SendMiscellaneousStatsAsync("fullscreen", RegistryManager.Instance.UserGuid, "topBarButton", "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM);
		}
		((Popup)ParentWindow.mFullscreenTopbarPopup).IsOpen = isVisible;
		((Popup)ParentWindow.mFullscreenTopbarPopupButton).IsOpen = false;
	}

	private void Label_MouseEnter(object sender, MouseEventArgs e)
	{
		Label val = (Label)((sender is Label) ? sender : null);
		if (val != null)
		{
			object obj = ((FrameworkElement)this).TryFindResource((object)"LabelMouseHoverBackground");
			SolidColorBrush val2 = (SolidColorBrush)((obj is SolidColorBrush) ? obj : null);
			if (val2 != null)
			{
				((Control)val).Background = (Brush)(object)val2;
			}
		}
	}

	private void Label_MouseLeave(object sender, MouseEventArgs e)
	{
		Label val = (Label)((sender is Label) ? sender : null);
		if (val != null)
		{
			object obj = ((FrameworkElement)this).TryFindResource((object)"LabelBackground");
			SolidColorBrush val2 = (SolidColorBrush)((obj is SolidColorBrush) ? obj : null);
			if (val2 != null)
			{
				((Control)val).Background = (Brush)(object)val2;
			}
		}
	}

	private void FullScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (!ParentWindow.mStreamingModeEnabled)
		{
			ParentWindow.mCommonHandler.FullScreenButtonHandler("topbar", "MouseClick");
		}
	}

	private void GameGuide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (!ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
		{
			KMManager.HandleInputMapperWindow(ParentWindow, "gamepad");
		}
		ClientStats.SendMiscellaneousStatsAsync("topbar", RegistryManager.Instance.UserGuid, "gameGuide", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName);
	}

	private void Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		string empty = string.Empty;
		ClientStats.SendMiscellaneousStatsAsync("topbar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		ParentWindow.mCommonHandler.LaunchSettingsWindow(empty);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/topbaroptions.xaml", UriKind.Relative);
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(TopbarOptions)target).Loaded += new RoutedEventHandler(Topbar_Loaded);
			break;
		case 2:
			TopMenu = (Grid)target;
			break;
		case 3:
			GameGuideColumn = (ColumnDefinition)target;
			break;
		case 4:
			((UIElement)(Label)target).MouseEnter += new MouseEventHandler(Label_MouseEnter);
			((UIElement)(Label)target).MouseLeave += new MouseEventHandler(Label_MouseLeave);
			((UIElement)(Label)target).MouseLeftButtonDown += new MouseButtonEventHandler(FullScreen_MouseLeftButtonDown);
			break;
		case 5:
			mFullScreenTextBlock = (TextBlock)target;
			break;
		case 6:
			((UIElement)(Label)target).MouseEnter += new MouseEventHandler(Label_MouseEnter);
			((UIElement)(Label)target).MouseLeave += new MouseEventHandler(Label_MouseLeave);
			((UIElement)(Label)target).MouseLeftButtonDown += new MouseButtonEventHandler(GameGuide_MouseLeftButtonDown);
			break;
		case 7:
			((UIElement)(Label)target).MouseEnter += new MouseEventHandler(Label_MouseEnter);
			((UIElement)(Label)target).MouseLeave += new MouseEventHandler(Label_MouseLeave);
			((UIElement)(Label)target).MouseLeftButtonDown += new MouseButtonEventHandler(Setting_MouseLeftButtonDown);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
