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

public class DMMFullScreenTopBar : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mEscCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mKeyMapSwitch;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mKeyMapButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mScreenshotBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mVolumeBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mWindowedBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSettingsBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mVolumePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox volumeSliderImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider mVolumeSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mChangeTransparencyPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsSliderButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider transSlider;

	private bool _contentLoaded;

	public DMMFullScreenTopBar()
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
		mVolumeBtn.ImageName = "volume_small";
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

	private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
		ParentWindow.mCommonHandler.ScreenShotButtonHandler();
	}

	private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mVolumePopup).IsOpen = !((Popup)mVolumePopup).IsOpen;
	}

	private void WindowedBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.FullScreenButtonHandler("fullscreentopbarDmm", "MouseClick");
	}

	private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.LaunchSettingsWindow();
	}

	internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (ParentWindow != null)
		{
			ParentWindow.mDmmBottomBar.VolumeSlider_PreviewMouseLeftButtonUp(sender, e);
		}
	}

	private void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (ParentWindow != null)
		{
			ParentWindow.mDmmBottomBar.VolumeSliderImage_PreviewMouseLeftButtonUp(sender, e);
		}
	}

	private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
	}

	private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mTopBarPopup).IsOpen = false;
		if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName != null)
		{
			ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
		}
	}

	private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((RangeBase)transSlider).Value = RegistryManager.Instance.TranslucentControlsTransparency;
		((Popup)mChangeTransparencyPopup).PlacementTarget = (UIElement)(object)mTranslucentControlsButton;
		((Popup)mChangeTransparencyPopup).IsOpen = true;
	}

	internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(sender, e);
	}

	internal void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		ParentWindow.mDmmBottomBar.TransparencySlider_ValueChanged(sender, e);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dmmfullscreentopbar.xaml", UriKind.Relative);
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mEscCheckbox = (CustomPictureBox)target;
			((UIElement)mEscCheckbox).MouseLeftButtonUp += new MouseButtonEventHandler(mEscCheckbox_MouseLeftButtonUp);
			break;
		case 2:
			mKeyMapSwitch = (CustomPictureBox)target;
			((UIElement)mKeyMapSwitch).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SwitchKeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 3:
			mKeyMapButton = (CustomPictureBox)target;
			((UIElement)mKeyMapButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mTranslucentControlsButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(TranslucentControlsButton_PreviewMouseLeftButtonUp);
			break;
		case 5:
			mScreenshotBtn = (CustomPictureBox)target;
			((UIElement)mScreenshotBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScreenshotBtn_MouseUp);
			break;
		case 6:
			mVolumeBtn = (CustomPictureBox)target;
			((UIElement)mVolumeBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeBtn_MouseUp);
			break;
		case 7:
			mWindowedBtn = (CustomPictureBox)target;
			((UIElement)mWindowedBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(WindowedBtn_MouseUp);
			break;
		case 8:
			mSettingsBtn = (CustomPictureBox)target;
			((UIElement)mSettingsBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SettingsBtn_MouseUp);
			break;
		case 9:
			mVolumePopup = (CustomPopUp)target;
			break;
		case 10:
			volumeSliderImage = (CustomPictureBox)target;
			((UIElement)volumeSliderImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeSliderImage_PreviewMouseLeftButtonUp);
			break;
		case 11:
			mVolumeSlider = (Slider)target;
			((UIElement)mVolumeSlider).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeSlider_PreviewMouseLeftButtonUp);
			break;
		case 12:
			mChangeTransparencyPopup = (CustomPopUp)target;
			break;
		case 13:
			mTranslucentControlsSliderButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsSliderButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
			break;
		case 14:
			transSlider = (Slider)target;
			((RangeBase)transSlider).ValueChanged += TransparencySlider_ValueChanged;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
