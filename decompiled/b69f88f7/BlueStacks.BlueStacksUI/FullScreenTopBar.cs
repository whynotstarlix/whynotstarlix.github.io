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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class FullScreenTopBar : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private double lastSliderValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mEscCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mGamePadButtonFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMacroRecorderFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mKeyMapSwitchFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mKeyMapButtonFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsButtonFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mFullScreenButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mLocationButtonFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mShakeButtonFullScreen;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mChangeTransparencyPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border borderSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsSliderButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider transSlider;

	private bool _contentLoaded;

	public FullScreenTopBar()
	{
		InitializeComponent();
	}

	internal void Init(MainWindow window)
	{
		ParentWindow = window;
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this) && !RegistryManager.Instance.UseEscapeToExitFullScreen)
		{
			mEscCheckbox.ImageName = "checkbox_new";
		}
		((RangeBase)transSlider).Value = RegistryManager.Instance.TranslucentControlsTransparency;
		if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
		{
			((UIElement)mKeyMapSwitchFullScreen).Visibility = (Visibility)2;
			((UIElement)mKeyMapButtonFullScreen).Visibility = (Visibility)2;
			((UIElement)mLocationButtonFullScreen).Visibility = (Visibility)2;
			((UIElement)mShakeButtonFullScreen).Visibility = (Visibility)2;
			((UIElement)mGamePadButtonFullScreen).Visibility = (Visibility)2;
			((UIElement)mTranslucentControlsButtonFullScreen).Visibility = (Visibility)2;
		}
		((UIElement)mMacroRecorderFullScreen).Visibility = (Visibility)2;
	}

	private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.BackButtonHandler();
	}

	private void HomeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.HomeButtonHandler();
	}

	private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("SwitchKeyMapClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "fullscreentopbar", null, null);
	}

	private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
		ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
	}

	private void FullScreenButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.FullScreenButtonHandler("fullScreenTopbar", "MouseClick");
	}

	private void LocationButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.LocationButtonHandler();
	}

	private void ScreenShotButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
		ParentWindow.mCommonHandler.ScreenShotButtonHandler();
	}

	private void ShakeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.ShakeButtonHandler();
	}

	private void mEscCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (RegistryManager.Instance.UseEscapeToExitFullScreen)
		{
			mEscCheckbox.ImageName = "checkbox_new";
			RegistryManager.Instance.UseEscapeToExitFullScreen = false;
		}
		else
		{
			mEscCheckbox.ImageName = "checkbox_new_checked";
			RegistryManager.Instance.UseEscapeToExitFullScreen = true;
		}
	}

	private void mMacroRecorderLandscape_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
	}

	private void GamePadButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
	}

	private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		RegistryManager.Instance.ShowKeyControlsOverlay = true;
		RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
		KMManager.ShowOverlayWindow(ParentWindow, isShow: true, isreload: true);
		((Popup)mChangeTransparencyPopup).PlacementTarget = (UIElement)(object)mTranslucentControlsButtonFullScreen;
		((Popup)mChangeTransparencyPopup).IsOpen = true;
	}

	private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		KMManager.ChangeTransparency(ParentWindow, ((RangeBase)transSlider).Value);
		if (((RangeBase)transSlider).Value == 0.0)
		{
			if (!RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
			}
			ParentWindow.mCommonHandler.OnOverlayStateChanged(isEnabled: false);
		}
		else
		{
			KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
			ParentWindow.mCommonHandler.OnOverlayStateChanged(isEnabled: true);
		}
		lastSliderValue = ((RangeBase)transSlider).Value;
	}

	private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((RangeBase)transSlider).Value == 0.0)
		{
			((RangeBase)transSlider).Value = lastSliderValue;
			return;
		}
		double value = ((RangeBase)transSlider).Value;
		((RangeBase)transSlider).Value = 0.0;
		lastSliderValue = value;
	}

	private void mChangeTransparencyPopup_Closed(object sender, EventArgs e)
	{
		if (!((UIElement)ParentWindow.mFullScreenTopBar).IsMouseOver)
		{
			((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/fullscreentopbar.xaml", UriKind.Relative);
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
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BackButton_PreviewMouseLeftButtonUp);
			break;
		case 2:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(HomeButton_PreviewMouseLeftButtonUp);
			break;
		case 3:
			mEscCheckbox = (CustomPictureBox)target;
			((UIElement)mEscCheckbox).MouseLeftButtonUp += new MouseButtonEventHandler(mEscCheckbox_MouseLeftButtonUp);
			break;
		case 4:
			mGamePadButtonFullScreen = (CustomPictureBox)target;
			((UIElement)mGamePadButtonFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(GamePadButton_PreviewMouseLeftButtonUp);
			break;
		case 5:
			mMacroRecorderFullScreen = (CustomPictureBox)target;
			((UIElement)mMacroRecorderFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mMacroRecorderLandscape_PreviewMouseLeftButtonUp);
			break;
		case 6:
			mKeyMapSwitchFullScreen = (CustomPictureBox)target;
			((UIElement)mKeyMapSwitchFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SwitchKeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 7:
			mKeyMapButtonFullScreen = (CustomPictureBox)target;
			((UIElement)mKeyMapButtonFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 8:
			mTranslucentControlsButtonFullScreen = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsButtonFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(TranslucentControlsButton_PreviewMouseLeftButtonUp);
			break;
		case 9:
			mFullScreenButton = (CustomPictureBox)target;
			((UIElement)mFullScreenButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(FullScreenButton_PreviewMouseLeftButtonUp);
			break;
		case 10:
			mLocationButtonFullScreen = (CustomPictureBox)target;
			((UIElement)mLocationButtonFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LocationButton_PreviewMouseLeftButtonUp);
			break;
		case 11:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScreenShotButton_PreviewMouseLeftButtonUp);
			break;
		case 12:
			mShakeButtonFullScreen = (CustomPictureBox)target;
			((UIElement)mShakeButtonFullScreen).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ShakeButton_PreviewMouseLeftButtonUp);
			break;
		case 13:
			mChangeTransparencyPopup = (CustomPopUp)target;
			break;
		case 14:
			borderSlider = (Border)target;
			break;
		case 15:
			mTranslucentControlsSliderButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsSliderButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
			break;
		case 16:
			transSlider = (Slider)target;
			((RangeBase)transSlider).ValueChanged += Slider_ValueChanged;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
