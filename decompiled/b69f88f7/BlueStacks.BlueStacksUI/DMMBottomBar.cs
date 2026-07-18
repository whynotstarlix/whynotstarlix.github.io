using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class DMMBottomBar : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private static double sCurrentTransparency = 0.0;

	private static double sPreviousTransparency = 0.0;

	private static int sCurrentVolume = 33;

	public static readonly DependencyProperty VolumeImageNameProperty = DependencyProperty.Register("VolumeImageName", typeof(string), typeof(DMMBottomBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)"volume_small", new PropertyChangedCallback(OnVolumeImageNameChanged)));

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid DMMBottomGrid;

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
	internal CustomPictureBox mFullscreenBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSettingsBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRecommendedWindowBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mVolumePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox volumesSliderImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider mVolumeSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mKeyMapPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mKeyMappingPopUp1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mKeyMappingPopUp3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mDoNotPromptChkBx;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mKeyMappingDontShowPopUp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path DownArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mChangeTransparencyPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsSliderButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider transSlider;

	private bool _contentLoaded;

	internal double CurrentTransparency
	{
		get
		{
			return sCurrentTransparency;
		}
		set
		{
			sCurrentTransparency = value;
			((RangeBase)transSlider).ValueChanged -= TransparencySlider_ValueChanged;
			if (ParentWindow.mDMMFST != null)
			{
				((RangeBase)ParentWindow.mDMMFST.transSlider).ValueChanged -= ParentWindow.mDMMFST.TransparencySlider_ValueChanged;
			}
			((RangeBase)transSlider).Value = sCurrentTransparency;
			if (ParentWindow.mDMMFST != null)
			{
				((RangeBase)ParentWindow.mDMMFST.transSlider).Value = sCurrentTransparency;
			}
			((RangeBase)transSlider).ValueChanged += TransparencySlider_ValueChanged;
			if (ParentWindow.mDMMFST != null)
			{
				((RangeBase)ParentWindow.mDMMFST.transSlider).ValueChanged += ParentWindow.mDMMFST.TransparencySlider_ValueChanged;
			}
		}
	}

	internal int CurrentVolume
	{
		get
		{
			return sCurrentVolume;
		}
		set
		{
			sCurrentVolume = value;
			((RangeBase)mVolumeSlider).Value = sCurrentVolume;
			if (ParentWindow.mDMMFST != null && ParentWindow.mDMMFST.mVolumeSlider != null)
			{
				((RangeBase)ParentWindow.mDMMFST.mVolumeSlider).Value = sCurrentVolume;
			}
			if (sCurrentVolume < 1)
			{
				VolumeImageName = "volume_mute";
			}
			else if (sCurrentVolume <= 50)
			{
				VolumeImageName = "volume_small";
			}
			else
			{
				VolumeImageName = "volume_large";
			}
		}
	}

	public string VolumeImageName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(VolumeImageNameProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(VolumeImageNameProperty, (object)value);
		}
	}

	private static void OnVolumeImageNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as DMMBottomBar).ParentWindow.mDmmBottomBar.mVolumeBtn.ImageName = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue.ToString();
		(d as DMMBottomBar).ParentWindow.mDmmBottomBar.volumesSliderImage.ImageName = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue.ToString();
		(d as DMMBottomBar).ParentWindow.mDMMFST.mVolumeBtn.ImageName = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue.ToString();
		(d as DMMBottomBar).ParentWindow.mDMMFST.volumeSliderImage.ImageName = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue.ToString();
	}

	public DMMBottomBar()
	{
		InitializeComponent();
	}

	public void Init(MainWindow window)
	{
		ParentWindow = window;
		VolumeImageName = "volume_small";
		mVolumeBtn.ImageName = "volume_small";
		CurrentTransparency = (sPreviousTransparency = RegistryManager.Instance.TranslucentControlsTransparency);
		if (ParentWindow != null)
		{
			ParentWindow.mCommonHandler.VolumeChangedEvent += DMMBottomBar_VolumeChangedEvent;
			ParentWindow.mCommonHandler.VolumeMutedEvent += DMMBottomBar_VolumeMutedEvent;
		}
	}

	private void DMMBottomBar_VolumeMutedEvent(bool muted)
	{
		if (muted)
		{
			VolumeImageName = "volume_mute";
		}
		else
		{
			CurrentVolume = (int)((RangeBase)mVolumeSlider).Value;
		}
	}

	private void DMMBottomBar_VolumeChangedEvent(int volumeLevel)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			CurrentVolume = volumeLevel;
		}, new object[0]);
	}

	internal void Tab_Changed(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
	}

	private void FullScreenBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.FullScreenButtonHandler("bottombarDmm", "MouseClick");
	}

	private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.ScreenShotButtonHandler();
	}

	private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mVolumePopup).IsOpen = !((Popup)mVolumePopup).IsOpen;
	}

	private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.LaunchSettingsWindow();
	}

	private void RecommendedWindowBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Window)ParentWindow).WindowState != 0)
		{
			return;
		}
		if (ParentWindow.mDMMRecommendedWindow == null)
		{
			ParentWindow.mDMMRecommendedWindow = new DMMRecommendedWindow(ParentWindow);
			ParentWindow.mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
		}
		if ((int)((UIElement)ParentWindow.mDMMRecommendedWindow).Visibility != 0)
		{
			((UIElement)ParentWindow.mDMMRecommendedWindow).Visibility = (Visibility)0;
			ParentWindow.mIsDMMRecommendedWindowOpen = true;
		}
		else
		{
			((UIElement)ParentWindow.mDMMRecommendedWindow).Visibility = (Visibility)1;
			ParentWindow.mIsDMMRecommendedWindowOpen = false;
		}
		ThreadPool.QueueUserWorkItem(delegate
		{
			Thread.Sleep(500);
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((Window)ParentWindow).Activate();
			}, new object[0]);
		});
	}

	private void DoNotPromptManageGP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (string.Equals(mDoNotPromptChkBx.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
		{
			mDoNotPromptChkBx.ImageName = "bgpcheckbox_checked";
			RegistryManager.Instance.KeyMappingAvailablePromptEnabled = false;
		}
		else
		{
			mDoNotPromptChkBx.ImageName = "bgpcheckbox";
			RegistryManager.Instance.KeyMappingAvailablePromptEnabled = true;
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mKeyMapPopup).IsOpen = false;
		((RoutedEventArgs)e).Handled = true;
	}

	private void KeyMapPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mKeyMapPopup).IsOpen = false;
		ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombarpopup");
	}

	private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
	}

	private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName != null)
		{
			ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombar");
		}
	}

	private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((RangeBase)transSlider).Value = RegistryManager.Instance.TranslucentControlsTransparency;
		((Popup)mChangeTransparencyPopup).PlacementTarget = (UIElement)(object)mTranslucentControlsButton;
		((Popup)mChangeTransparencyPopup).IsOpen = true;
	}

	private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			UpdateLayoutAndBounds();
		}
	}

	internal void UpdateLayoutAndBounds()
	{
		if (((Popup)mKeyMapPopup).IsOpen)
		{
			ShowKeyMapPopup(isShow: true);
		}
	}

	internal void ShowKeyMapPopup(bool isShow)
	{
		if (isShow)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				Thread.Sleep(500);
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((Popup)mKeyMapPopup).IsOpen = false;
					((Popup)mKeyMapPopup).PlacementTarget = (UIElement)(object)mKeyMapButton;
					if (!Array.Exists(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == ParentWindow.StaticComponents.mSelectedTabButton.PackageName) && RegistryManager.Instance.IsAutoShowGuidance && !ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsKeyMappingTipDisplayed)
					{
						ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsKeyMappingTipDisplayed = true;
						KMManager.HandleInputMapperWindow(ParentWindow);
					}
					else if (RegistryManager.Instance.KeyMappingAvailablePromptEnabled)
					{
						((Popup)mKeyMapPopup).IsOpen = true;
					}
				}, new object[0]);
			});
			thread.IsBackground = true;
			thread.Start();
		}
		else
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((Popup)mKeyMapPopup).IsOpen = false;
			}, new object[0]);
		}
	}

	internal void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		CurrentTransparency = e.NewValue;
		sPreviousTransparency = e.NewValue;
		ChangeTransparency();
	}

	private void ChangeTransparency()
	{
		KMManager.ChangeTransparency(ParentWindow, CurrentTransparency);
		if (CurrentTransparency == 0.0)
		{
			KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
			ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye_off");
		}
		else
		{
			KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
			ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye");
		}
	}

	internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Slider val = (Slider)sender;
		if (ParentWindow != null)
		{
			ParentWindow.Utils.SetVolumeInFrontendAsync((int)((RangeBase)val).Value);
		}
	}

	internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CurrentTransparency = ((CurrentTransparency == 0.0) ? sPreviousTransparency : 0.0);
		ChangeTransparency();
	}

	internal void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (ParentWindow != null)
		{
			if (ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				ParentWindow.Utils.UnmuteApplication(allInstances: false);
			}
			else
			{
				ParentWindow.Utils.MuteApplication(allInstances: false);
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dmmbottombar.xaml", UriKind.Relative);
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
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Expected O, but got Unknown
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(DMMBottomBar)target).SizeChanged += new SizeChangedEventHandler(UserControl_SizeChanged);
			break;
		case 2:
			DMMBottomGrid = (Grid)target;
			break;
		case 3:
			mKeyMapSwitch = (CustomPictureBox)target;
			((UIElement)mKeyMapSwitch).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SwitchKeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mKeyMapButton = (CustomPictureBox)target;
			((UIElement)mKeyMapButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyMapButton_PreviewMouseLeftButtonUp);
			break;
		case 5:
			mTranslucentControlsButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(TranslucentControlsButton_PreviewMouseLeftButtonUp);
			break;
		case 6:
			mScreenshotBtn = (CustomPictureBox)target;
			((UIElement)mScreenshotBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScreenshotBtn_MouseUp);
			break;
		case 7:
			mVolumeBtn = (CustomPictureBox)target;
			((UIElement)mVolumeBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeBtn_MouseUp);
			break;
		case 8:
			mFullscreenBtn = (CustomPictureBox)target;
			((UIElement)mFullscreenBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(FullScreenBtn_MouseUp);
			break;
		case 9:
			mSettingsBtn = (CustomPictureBox)target;
			((UIElement)mSettingsBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SettingsBtn_MouseUp);
			break;
		case 10:
			mRecommendedWindowBtn = (CustomPictureBox)target;
			((UIElement)mRecommendedWindowBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(RecommendedWindowBtn_PreviewMouseLeftButtonUp);
			break;
		case 11:
			mVolumePopup = (CustomPopUp)target;
			break;
		case 12:
			volumesSliderImage = (CustomPictureBox)target;
			((UIElement)volumesSliderImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeSliderImage_PreviewMouseLeftButtonUp);
			break;
		case 13:
			mVolumeSlider = (Slider)target;
			((UIElement)mVolumeSlider).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeSlider_PreviewMouseLeftButtonUp);
			break;
		case 14:
			mKeyMapPopup = (CustomPopUp)target;
			break;
		case 15:
			((UIElement)(Border)target).MouseLeftButtonUp += new MouseButtonEventHandler(KeyMapPopup_PreviewMouseLeftButtonUp);
			break;
		case 16:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(ClosePopup_MouseLeftButtonUp);
			break;
		case 17:
			mKeyMappingPopUp1 = (TextBlock)target;
			break;
		case 18:
			mKeyMappingPopUp3 = (TextBlock)target;
			break;
		case 19:
			mDoNotPromptChkBx = (CustomPictureBox)target;
			((UIElement)mDoNotPromptChkBx).MouseLeftButtonUp += new MouseButtonEventHandler(DoNotPromptManageGP_MouseLeftButtonUp);
			break;
		case 20:
			mKeyMappingDontShowPopUp = (TextBlock)target;
			((UIElement)mKeyMappingDontShowPopUp).MouseLeftButtonUp += new MouseButtonEventHandler(DoNotPromptManageGP_MouseLeftButtonUp);
			break;
		case 21:
			DownArrow = (Path)target;
			break;
		case 22:
			mChangeTransparencyPopup = (CustomPopUp)target;
			break;
		case 23:
			mTranslucentControlsSliderButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsSliderButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
			break;
		case 24:
			transSlider = (Slider)target;
			((RangeBase)transSlider).ValueChanged += TransparencySlider_ValueChanged;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
