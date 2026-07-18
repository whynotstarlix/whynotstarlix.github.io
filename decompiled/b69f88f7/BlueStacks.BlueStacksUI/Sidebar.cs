using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class Sidebar : UserControl, IComponentConnector
{
	private Dictionary<SidebarElement, EventHandler> mDictActions = new Dictionary<SidebarElement, EventHandler>();

	internal List<SidebarElement> mListSidebarElements = new List<SidebarElement>();

	private int mTotalVisibleElements;

	private double mSidebarElementApproxHeight;

	internal bool mIsUIInPortraitModeBeforeChange;

	internal bool mIsOverlayTooltipClosed;

	private bool mIsPendingShowOverlayTooltip;

	internal double mLastSliderValue;

	private bool mIsLoadedOnce;

	private bool mIsInFullscreenMode;

	private DispatcherTimer mMacroBookmarkTimer;

	private DispatcherTimer mGameControlBookmarkTimer;

	private DispatcherTimer mChangeTransparencyPopupTimer;

	internal DispatcherTimer mVolumeSliderPopupTimer;

	private DispatcherTimer mSidebarPopupTimer;

	private string currentScreenshotSavedPath;

	private readonly object mSyncRoot = new object();

	private MainWindow mMainWindow;

	private bool mIsOneSidebarElementLoadedBinded;

	internal List<CustomPopUp> mListPopups;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Sidebar mSidebar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTopGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mElementsStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SidebarElement mMoreButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mChangeTransparencyPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTranslucentControlsSliderButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider transSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mVolumeSliderPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mVolumeMuteUnmuteImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider mVolumeSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCurrentVolumeValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMuteInstancesCheckboxImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMuteAllInstancesTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mOverlayTooltip;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder4;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mOverlayPopUpTitle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mOverlayPopUpMessage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mOverlayDoNotShowCheckboxImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mOverlayDontShowPopUp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMacroButtonPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder5;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroBookmarksPopup mMacroBookmarkPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCustomiseSectionTag;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Separator mCustomiseSectionBorderLine;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mOpenMacroTextbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mGameControlButtonPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder6;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mBookmarkedSchemesStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mOpenGameControlTextbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mRecordScreenPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder7;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRecordScreenClose;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock RecordScreenPopupHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock RecordScreenPopupBody;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock RecordScreenPopupHyperlink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRecorderClickLink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mScreenshotPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder8;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mScreenshotClose;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock ScreenshotPopupHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock ScreenshotPopupHyperlink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mScreenshotClickLink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mGameControlsBlurbPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder10;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ContentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock headerTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock bodyTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton OkayButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path RightArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMoreElements;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPopupGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SidebarPopup mSidebarPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBottomGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mStaticButtonsStackPanel;

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

	public Sidebar()
	{
		InitializeComponent();
		mMoreButton.Image.ImageName = "sidebar_options_close";
		if (mListPopups == null)
		{
			mListPopups = new List<CustomPopUp>(8) { mChangeTransparencyPopup, mVolumeSliderPopup, mOverlayTooltip, mMacroButtonPopup, mGameControlButtonPopup, mRecordScreenPopup, mScreenshotPopup, mMoreElements };
		}
		BlueStacksUIBinding.Instance.PropertyChanged += BlueStacksUIBinding_PropertyChanged;
	}

	private void BlueStacksUIBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "LocaleModel")
		{
			ParentWindow.mCommonHandler.ReloadTooltips();
		}
	}

	internal void BindEvents()
	{
		ParentWindow.CursorLockChangedEvent += ParentWindow_CursorLockChangedEvent;
		ParentWindow.FullScreenChanged += ParentWindow_FullScreenChangedEvent;
		ParentWindow.FrontendGridVisibilityChanged += ParentWindow_FrontendGridVisibleChangedEvent;
		ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += ParentWindow_ScreenRecordingStateChangedEvent;
		ParentWindow.mCommonHandler.OverlayStateChangedEvent += ParentWindow_OverlayStateChangedEvent;
		ParentWindow.mCommonHandler.MacroButtonVisibilityChangedEvent += ParentWindow_MacroButtonVisibilityChangedEvent;
		ParentWindow.mCommonHandler.OperationSyncButtonVisibilityChangedEvent += ParentWindow_OperationSyncButtonVisibilityChangedEvent;
		ParentWindow.mCommonHandler.ScreenRecorderStateTransitioningEvent += ParentWindow_ScreenRecordingInitingEvent;
		ParentWindow.mCommonHandler.OBSResponseTimeoutEvent += ParentWindow_OBSResponseTimeoutEvent;
		ParentWindow.mCommonHandler.BTvDownloaderMinimizedEvent += ParentWindow_BTvDownloaderMinimizedEvent;
		ParentWindow.mCommonHandler.GamepadButtonVisibilityChangedEvent += ParentWindow_GamepadButtonVisibilityChangedEvent;
		ParentWindow.mCommonHandler.VolumeChangedEvent += ParentWindow_VolumeChangedEvent;
		ParentWindow.mCommonHandler.VolumeMutedEvent += ParentWindow_VolumeMutedEvent;
		PromotionObject.AppSpecificRulesHandler = (EventHandler)Delegate.Combine(PromotionObject.AppSpecificRulesHandler, new EventHandler(PromotionUpdated));
		ParentWindow.mCommonHandler.GameGuideButtonVisibilityChangedEvent += ParentWindow_GameGuideButtonVisibilityChangedEvent;
		if (ParentWindow.mGuestBootCompleted)
		{
			ToggleBootCompletedState();
		}
		else
		{
			ParentWindow.GuestBootCompleted += ParentWindow_GuestBootCompletedEvent;
		}
	}

	private void ParentWindow_GameGuideButtonVisibilityChangedEvent(bool visibility)
	{
		ChangeElementState("sidebar_gameguide", visibility);
	}

	private void PromotionUpdated(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
	}

	public void UpdateMuteAllInstancesCheckbox()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (RegistryManager.Instance.AreAllInstancesMuted)
			{
				mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
			}
			else
			{
				mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
			}
		}, new object[0]);
	}

	private void ParentWindow_VolumeMutedEvent(bool muted)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (muted)
			{
				mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_muted_popup";
				UpdateImage("sidebar_volume", "sidebar_volume_muted");
			}
			else
			{
				mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_popup";
				UpdateToDefaultImage("sidebar_volume");
			}
			UpdateMuteAllInstancesCheckbox();
		}, new object[0]);
	}

	private void ParentWindow_VolumeChangedEvent(int volumeLevel)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((RangeBase)mVolumeSlider).Value = volumeLevel;
			mCurrentVolumeValue.Text = volumeLevel.ToString(CultureInfo.InvariantCulture);
		}, new object[0]);
	}

	private void ParentWindow_GamepadButtonVisibilityChangedEvent(bool visibility)
	{
		ChangeElementState("sidebar_gamepad", visibility);
	}

	private void ParentWindow_BTvDownloaderMinimizedEvent()
	{
		((UIElement)RecordScreenPopupHeader).Visibility = (Visibility)2;
		((UIElement)RecordScreenPopupBody).Visibility = (Visibility)0;
		((UIElement)RecordScreenPopupHyperlink).Visibility = (Visibility)2;
		BlueStacksUIBinding.Bind(RecordScreenPopupBody, "STRING_DOWNLOAD_BACKGROUND", "");
		((Popup)mRecordScreenPopup).StaysOpen = false;
		((Popup)mRecordScreenPopup).IsOpen = true;
	}

	private void ParentWindow_OBSResponseTimeoutEvent()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
			elementFromTag.Image.IsImageToBeRotated = false;
			UpdateToDefaultImage(elementFromTag);
		}, new object[0]);
	}

	private void ParentWindow_ScreenRecordingInitingEvent()
	{
		SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
		UpdateImage(elementFromTag, "sidebar_video_loading");
		((UIElement)elementFromTag.Image).Visibility = (Visibility)1;
		elementFromTag.Image.IsImageToBeRotated = true;
		((UIElement)elementFromTag.Image).Visibility = (Visibility)0;
		((UIElement)RecordScreenPopupHyperlink).Visibility = (Visibility)2;
		((UIElement)RecordScreenPopupBody).Visibility = (Visibility)2;
		((UIElement)mRecordScreenClose).Visibility = (Visibility)2;
	}

	private void ParentWindow_OperationSyncButtonVisibilityChangedEvent(bool isVisible)
	{
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ToggleElementVisibilty("sidebar_operation", isVisible);
		}
	}

	private void ToggleElementVisibilty(SidebarElement ele, bool isVisible)
	{
		if (ele == null)
		{
			return;
		}
		if (isVisible)
		{
			((UIElement)ele).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)ele).Visibility = (Visibility)2;
		}
		if (ele.IsInMainSidebar)
		{
			int num = ((Panel)mElementsStackPanel).Children.IndexOf((UIElement)(object)ele);
			int num2 = mListSidebarElements.IndexOf(ele);
			if (num != -1 && num != num2)
			{
				((Panel)mElementsStackPanel).Children.RemoveAt(num);
				int count = ((Panel)mElementsStackPanel).Children.Count;
				if (num2 >= count)
				{
					ele.IsInMainSidebar = false;
				}
				else
				{
					((Panel)mElementsStackPanel).Children.Insert(num2 + 1, (UIElement)(object)ele);
				}
			}
		}
		FixMarginOfSurroundingElement(ele);
		UpdateTotalVisibleElementCount();
		ArrangeAllSidebarElements();
	}

	private SidebarElement GetPreviousVisibleSidebarElement(SidebarElement ele)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SidebarElement previousSidebarElement = GetPreviousSidebarElement(ele);
		if ((int)((UIElement)previousSidebarElement).Visibility != 0)
		{
			return GetPreviousSidebarElement(previousSidebarElement);
		}
		return previousSidebarElement;
	}

	private SidebarElement GetPreviousSidebarElement(SidebarElement ele)
	{
		int num = mListSidebarElements.IndexOf(ele);
		if (num != 0)
		{
			return mListSidebarElements[num - 1];
		}
		return ele;
	}

	private void FixMarginOfSurroundingElement(SidebarElement currentElement)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (currentElement != null && (int)((UIElement)currentElement).Visibility == 0)
		{
			if (currentElement.IsLastElementOfGroup && !currentElement.IsCurrentLastElementOfGroup)
			{
				currentElement.IsCurrentLastElementOfGroup = true;
				IncreaseElementBottomMarginIfLast(currentElement);
				SidebarElement previousVisibleSidebarElement = GetPreviousVisibleSidebarElement(currentElement);
				if (previousVisibleSidebarElement != currentElement)
				{
					previousVisibleSidebarElement.IsCurrentLastElementOfGroup = false;
					DecreaseElementBottomMargin(previousVisibleSidebarElement);
					Thickness margin = ((FrameworkElement)previousVisibleSidebarElement).Margin;
					((Thickness)(ref margin)).Bottom = 2.0;
					((FrameworkElement)previousVisibleSidebarElement).Margin = margin;
				}
			}
		}
		else if (currentElement.IsCurrentLastElementOfGroup)
		{
			currentElement.IsCurrentLastElementOfGroup = false;
			SidebarElement previousVisibleSidebarElement2 = GetPreviousVisibleSidebarElement(currentElement);
			if (previousVisibleSidebarElement2 != currentElement)
			{
				previousVisibleSidebarElement2.IsCurrentLastElementOfGroup = true;
				IncreaseElementBottomMarginIfLast(previousVisibleSidebarElement2);
			}
		}
	}

	private void ToggleElementVisibilty(string elementKey, bool isVisible)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ToggleElementVisibilty(GetElementFromTag(elementKey), isVisible);
		}, new object[0]);
	}

	private void ParentWindow_MacroButtonVisibilityChangedEvent(bool isVisible)
	{
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ToggleElementVisibilty("sidebar_macro", isVisible);
		}
	}

	private void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
	{
		SidebarElement elementFromTag = GetElementFromTag("sidebar_overlay");
		if (isEnabled)
		{
			UpdateToDefaultImage(elementFromTag);
			if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
			{
				if (mLastSliderValue == 0.0)
				{
					RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
					((RangeBase)transSlider).Value = 0.5;
				}
				else
				{
					RegistryManager.Instance.TranslucentControlsTransparency = mLastSliderValue;
					((RangeBase)transSlider).Value = mLastSliderValue;
				}
			}
		}
		else
		{
			UpdateImage(elementFromTag, "sidebar_overlay_inactive");
			double value = ((RangeBase)transSlider).Value;
			((RangeBase)transSlider).Value = 0.0;
			mLastSliderValue = value;
		}
		KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
	}

	private void MSidebarPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((Popup)mMoreElements).IsOpen)
		{
			((Popup)mMoreElements).IsOpen = false;
		}
	}

	private void ParentWindow_ScreenRecordingStateChangedEvent(bool isRecording)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
			elementFromTag.Image.IsImageToBeRotated = false;
			if (isRecording)
			{
				UpdateImage(elementFromTag, "sidebar_video_capture_active");
				ChangeElementState("sidebar_fullscreen", isEnabled: false);
				BlueStacksUIBinding.Bind(RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
				((UIElement)RecordScreenPopupHeader).Visibility = (Visibility)0;
				((UIElement)RecordScreenPopupHyperlink).Visibility = (Visibility)2;
				((UIElement)RecordScreenPopupBody).Visibility = (Visibility)2;
				((UIElement)mRecordScreenClose).Visibility = (Visibility)2;
			}
			else
			{
				UpdateToDefaultImage(elementFromTag);
				((UIElement)RecordScreenPopupBody).Visibility = (Visibility)0;
				((UIElement)RecordScreenPopupHeader).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind(RecordScreenPopupHeader, "STRING_RECORDING_SAVED", "");
				BlueStacksUIBinding.Bind(RecordScreenPopupBody, "STRING_CLICK_TO_SEE_VIDEO", "");
				((UIElement)RecordScreenPopupBody).Visibility = (Visibility)2;
				((UIElement)RecordScreenPopupHyperlink).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind(mRecorderClickLink, "STRING_CLICK_TO_SEE_VIDEO", "");
				((UIElement)RecordScreenPopupBody).Visibility = (Visibility)0;
				((UIElement)mRecordScreenClose).Visibility = (Visibility)0;
				if (ParentWindow.mIsWindowInFocus && elementFromTag.IsInMainSidebar)
				{
					((Popup)mRecordScreenPopup).PlacementTarget = (UIElement)(object)elementFromTag;
					((Popup)mRecordScreenPopup).StaysOpen = false;
					((Popup)mRecordScreenPopup).IsOpen = true;
				}
				if (RegistryManager.Instance.IsShowToastNotification)
				{
					ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_SAVED", ""));
				}
				if (((UIElement)ParentWindow.mFrontendGrid).IsVisible)
				{
					ChangeElementState("sidebar_fullscreen", isEnabled: true);
				}
			}
			SetVideoRecordingTooltip(isRecording);
		}, new object[0]);
	}

	internal void ShowScreenshotSavedPopup(string screenshotPath)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			SidebarElement elementFromTag = GetElementFromTag("sidebar_screenshot");
			SetSidebarElementTooltip(elementFromTag, "STRING_TOOLBAR_CAMERA");
			if (ParentWindow.mIsWindowInFocus && elementFromTag.IsInMainSidebar)
			{
				((Popup)mScreenshotPopup).PlacementTarget = (UIElement)(object)elementFromTag;
				((Popup)mScreenshotPopup).StaysOpen = false;
				((Popup)mScreenshotPopup).IsOpen = true;
				currentScreenshotSavedPath = screenshotPath;
			}
		}, new object[0]);
	}

	private void ParentWindow_FrontendGridVisibleChangedEvent(object sender, MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args)
	{
		ChangeElementState("sidebar_lock_cursor", args.IsVisible);
		if (!CommonHandlers.sIsRecordingVideo)
		{
			ChangeElementState("sidebar_fullscreen", args.IsVisible);
		}
		ChangeElementState("sidebar_toggle", args.IsVisible);
		ChangeElementState("sidebar_controls", args.IsVisible);
		ChangeElementState("sidebar_overlay", args.IsVisible);
		ChangeElementState("sidebar_back", args.IsVisible);
		ChangeElementState("sidebar_home", args.IsVisible);
		ChangeElementState("sidebar_screenshot", args.IsVisible);
		ChangeElementState("sidebar_video_capture", args.IsVisible);
		if (!args.IsVisible)
		{
			ChangeElementState("sidebar_gamepad", args.IsVisible);
			ChangeElementState("sidebar_gameguide", args.IsVisible);
		}
	}

	private void InitDefaultSettings()
	{
		if (ParentWindow.mIsFullScreen)
		{
			UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
		}
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ToggleElementVisibilty("sidebar_macro", isVisible: false);
			ToggleElementVisibilty("sidebar_operation", isVisible: false);
			if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
			{
				UpdateImage("sidebar_overlay", "sidebar_overlay_inactive");
				mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
			}
		}
		else
		{
			ToggleElementVisibilty("sidebar_overlay", isVisible: false);
			ToggleElementVisibilty("sidebar_overlay_inactive", isVisible: false);
		}
		SetupVolumeInitState();
		((RangeBase)transSlider).Value = RegistryManager.Instance.TranslucentControlsTransparency;
		mOverlayPopUpMessage.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ON_SCREEN_CONTROLS_BODY", ""), new object[1] { ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_OVERLAY") });
	}

	private void SetupVolumeInitState()
	{
		if (ParentWindow.EngineInstanceRegistry.IsMuted)
		{
			UpdateImage("sidebar_volume", "sidebar_volume_muted");
			mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_muted_popup";
		}
		UpdateMuteAllInstancesCheckbox();
		((RangeBase)mVolumeSlider).Value = ParentWindow.Utils.CurrentVolumeLevel;
		mCurrentVolumeValue.Text = ParentWindow.Utils.CurrentVolumeLevel.ToString(CultureInfo.InvariantCulture);
	}

	internal void HideSideBarInFullscreen()
	{
		((Popup)ParentWindow.mFullscreenSidebarPopupButton).IsOpen = false;
		((Popup)ParentWindow.mFullscreenSidebarPopup).IsOpen = false;
	}

	internal void ToggleSidebarVisibilityInFullscreen(bool isVisible)
	{
		if (isVisible)
		{
			((FrameworkElement)ParentWindow.mFullscreenSidebarPopup).Height = ((FrameworkElement)ParentWindow.MainGrid).ActualHeight;
			((Popup)ParentWindow.mFullscreenSidebarPopup).HorizontalOffset = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth / 2.0;
			((FrameworkElement)ParentWindow.mFullscreenSidebarPopupInnerGrid).Height = ((FrameworkElement)ParentWindow.MainGrid).ActualHeight;
			ClientStats.SendMiscellaneousStatsAsync("fullscreen", RegistryManager.Instance.UserGuid, "sideBarButton", "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM);
		}
		else
		{
			mListPopups.All((CustomPopUp x) => ((Popup)x).IsOpen = false);
		}
		((Popup)ParentWindow.mFullscreenSidebarPopup).IsOpen = isVisible;
		((Popup)ParentWindow.mFullscreenSidebarPopupButton).IsOpen = false;
	}

	internal void ToggleSidebarButtonVisibilityInFullscreen(bool isVisible)
	{
		if (isVisible && !((Popup)ParentWindow.mFullscreenTopbarPopup).IsOpen && !((Popup)ParentWindow.mFullscreenSidebarPopup).IsOpen)
		{
			((FrameworkElement)ParentWindow.mFullscreenSidebarPopupButtonInnerGrid).Height = ((FrameworkElement)ParentWindow.MainGrid).ActualHeight;
			((FrameworkElement)ParentWindow.mFullscreenSidebarPopupButton).Height = ((FrameworkElement)ParentWindow.MainGrid).ActualHeight;
			((Popup)ParentWindow.mFullscreenSidebarPopupButton).HorizontalOffset = ((FrameworkElement)ParentWindow.MainGrid).ActualWidth / 2.0;
			((Popup)ParentWindow.mFullscreenSidebarPopupButton).IsOpen = true;
		}
		else if (!isVisible)
		{
			mListPopups.All((CustomPopUp x) => ((Popup)x).IsOpen = false);
			((Popup)ParentWindow.mFullscreenSidebarPopupButton).IsOpen = false;
		}
	}

	private void ParentWindow_FullScreenChangedEvent(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args)
	{
		lock (mSyncRoot)
		{
			mIsInFullscreenMode = args.IsFullscreen;
			SetupSidebarForFullscreen(mIsInFullscreenMode);
			if (mIsInFullscreenMode)
			{
				UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
			}
			else
			{
				UpdateImage("sidebar_fullscreen", "sidebar_fullscreen");
				((Popup)ParentWindow.mFullscreenSidebarPopup).IsOpen = false;
				((Popup)ParentWindow.mFullscreenSidebarPopupButton).IsOpen = false;
			}
			ArrangeAllSidebarElements();
		}
	}

	private void SetupSidebarForFullscreen(bool isFullScreen)
	{
		if (isFullScreen)
		{
			if (((Panel)ParentWindow.mMainWindowTopGrid).Children.Contains((UIElement)(object)this))
			{
				((Panel)ParentWindow.mMainWindowTopGrid).Children.Remove((UIElement)(object)this);
				((Panel)ParentWindow.mFullscreenSidebarPopupInnerGrid).Children.Add((UIElement)(object)this);
			}
			((UIElement)this).Visibility = (Visibility)0;
			return;
		}
		if (((Panel)ParentWindow.mFullscreenSidebarPopupInnerGrid).Children.Contains((UIElement)(object)this))
		{
			((Panel)ParentWindow.mFullscreenSidebarPopupInnerGrid).Children.Remove((UIElement)(object)this);
			((Panel)ParentWindow.mMainWindowTopGrid).Children.Add((UIElement)(object)this);
		}
		if (ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
		{
			((UIElement)this).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)this).Visibility = (Visibility)2;
		}
	}

	private void ParentWindow_CursorLockChangedEvent(object sender, MainWindowEventArgs.CursorLockChangedEventArgs args)
	{
		if (args.IsLocked)
		{
			UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor_active");
		}
		else
		{
			UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor");
		}
	}

	private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
	{
		ToggleBootCompletedState();
	}

	private void ToggleBootCompletedState()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ChangeElementState("sidebar_stream_video", isEnabled: true);
			ChangeElementState("sidebar_volume", isEnabled: true);
			ChangeElementState("sidebar_macro", isEnabled: true);
			ChangeElementState("sidebar_operation", isEnabled: true);
			ChangeElementState("sidebar_location", isEnabled: true);
			ChangeElementState("sidebar_rotate", isEnabled: true);
		}, new object[0]);
	}

	private static void ChangeElementState(SidebarElement ele, bool isEnabled)
	{
		if (ele != null)
		{
			((UIElement)ele).IsEnabled = isEnabled;
		}
	}

	private void ChangeElementState(string elementTag, bool isEnabled)
	{
		ChangeElementState(GetElementFromTag(elementTag), isEnabled);
	}

	private void Sidebar_Loaded(object sender, RoutedEventArgs e)
	{
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			if (!mIsLoadedOnce)
			{
				mIsLoadedOnce = true;
				BindEvents();
				SetPlacementTargets();
				InitDefaultSettings();
				mMacroBookmarkPopup.SetParentWindowAndBindEvents(ParentWindow);
				ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
			}
			ParentWindow.mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
			SetVideoRecordingTooltipForNCSoft();
		}
	}

	private void MMacroButtonAndPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (((Popup)mMacroButtonPopup).IsOpen)
		{
			if (mMacroBookmarkTimer == null)
			{
				mMacroBookmarkTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 0, 500)
				};
				mMacroBookmarkTimer.Tick += MMacroBookmarkTimer_Tick;
			}
			else
			{
				mMacroBookmarkTimer.Stop();
			}
			mMacroBookmarkTimer.Start();
		}
	}

	private void MMacroBookmarkTimer_Tick(object sender, EventArgs e)
	{
		if (!((UIElement)mMacroButtonPopup).IsMouseOver && !((UIElement)GetElementFromTag("sidebar_macro")).IsMouseOver)
		{
			((Popup)mMacroButtonPopup).IsOpen = false;
			if (mIsInFullscreenMode && !((UIElement)this).IsMouseOver)
			{
				ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	private void MacroButtonHandler(object sender, EventArgs e)
	{
		if (ParentWindow.mIsMacroRecorderActive)
		{
			ParentWindow.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""));
			return;
		}
		if (RegistryManager.Instance.BookmarkedScriptList.Length != 0 && !((Popup)mMoreElements).IsOpen)
		{
			((Popup)mMacroButtonPopup).IsOpen = true;
			return;
		}
		ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
		((Popup)mMacroButtonPopup).IsOpen = false;
		ToggleSidebarVisibilityInFullscreen(isVisible: false);
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void OperationSyncHandler(object sender, EventArgs e)
	{
		ParentWindow.ShowSynchronizerWindow();
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void KeymapToggleHandler(object sender, EventArgs e)
	{
		KeyMapSwitchButtonHandler(sender as SidebarElement);
	}

	private void KeyMapControlsButton_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Right Click on keymap control UI button ");
		try
		{
			KMManager.sIsDeveloperModeOn = (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119)) && (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121));
			KMManager.LoadIMActions(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			KMManager.ShowAdvancedSettings(ParentWindow);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception on right click on keymap button: " + ex.ToString());
		}
	}

	internal void KeyMapSwitchButtonHandler(SidebarElement ele)
	{
		bool fromSideBar = true;
		if (ele == null)
		{
			ele = GetElementFromTag("sidebar_toggle");
			fromSideBar = false;
		}
		if (ele == null)
		{
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (!KMManager.sIsComboRecordingOn)
			{
				if (string.Equals(ele.Image.ImageName, "sidebar_toggle_off", StringComparison.InvariantCulture))
				{
					UpdateToDefaultImage(ele);
					ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: true);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOn", fromSideBar ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				else
				{
					UpdateImage(ele, "sidebar_toggle_off");
					ParentWindow.mFrontendHandler.EnableKeyMapping(isEnabled: false);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOff", fromSideBar ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
			}
		}, new object[0]);
	}

	private void SetPlacementTargets()
	{
		((Popup)mChangeTransparencyPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_overlay");
		((Popup)mVolumeSliderPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_volume");
		((Popup)mOverlayTooltip).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_overlay");
		((Popup)mRecordScreenPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_video_capture");
		((Popup)mScreenshotPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_screenshot");
		((Popup)mMacroButtonPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_macro");
		((Popup)mGameControlButtonPopup).PlacementTarget = (UIElement)(object)GetElementFromTag("sidebar_controls");
	}

	internal void InitElements()
	{
		SidebarConfig.sFilePath = Path.Combine(RegistryStrings.GadgetDir, string.Format(CultureInfo.InvariantCulture, "SidebarConfig_{0}.json", new object[1] { ParentWindow.mVmName }));
		foreach (List<string> groupElement in SidebarConfig.Instance.GroupElements)
		{
			CreateAndAddElementsToStackPanel(groupElement);
		}
		InitStaticElements();
		UpdateTotalVisibleElementCount();
	}

	private void UpdateTotalVisibleElementCount()
	{
		mTotalVisibleElements = mListSidebarElements.Where((SidebarElement item) => (int)((UIElement)item).Visibility == 0).Count();
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			mTotalVisibleElements--;
		}
		else
		{
			mTotalVisibleElements -= 2;
		}
	}

	private void InitStaticElements()
	{
		SidebarElement ele = CreateElement("sidebar_settings", "STRING_SETTINGS", GoSettingsHandler);
		AddElement(ele, isStaticElement: true);
		ele = CreateElement("sidebar_back", "STRING_BACK", GoBackHandler);
		AddElement(ele, isStaticElement: true);
		ChangeElementState(ele, isEnabled: false);
	}

	private void CreateAndAddElementsToStackPanel(List<string> ls)
	{
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Expected O, but got Unknown
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Expected O, but got Unknown
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Expected O, but got Unknown
		SidebarElement sidebarElement = null;
		foreach (string l in ls)
		{
			switch (l)
			{
			case "sidebar_stream_video":
				sidebarElement = CreateElement("sidebar_stream_video", "STRING_START_STREAMING", StreamingHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_volume":
				sidebarElement = CreateElement("sidebar_volume", "STRING_CHANGE_VOLUME", VolumeButtonHandler);
				AddElement(sidebarElement);
				((UIElement)sidebarElement).MouseLeave += new MouseEventHandler(VolumeSliderPopup_MouseLeave);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_toggle":
				sidebarElement = CreateElement("sidebar_toggle", "STRING_TOGGLE_KEYMAPPING_STATE", KeymapToggleHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_controls":
				sidebarElement = CreateElement("sidebar_controls", "STRING_CONTROLS_EDITOR", GameControlButtonHandler);
				((UIElement)sidebarElement).PreviewMouseRightButtonUp += new MouseButtonEventHandler(KeyMapControlsButton_PreviewMouseRightButtonUp);
				((UIElement)sidebarElement).MouseLeave += new MouseEventHandler(GameControlButtonPopup_MouseLeave);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_overlay":
				sidebarElement = CreateElement("sidebar_overlay", "STRING_TOGGLE_OVERLAY", KeymappingControlsTransparencyButtonHandler);
				AddElement(sidebarElement);
				((UIElement)sidebarElement).MouseLeave += new MouseEventHandler(ChangeTransparencyPopup_MouseLeave);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_screenshot":
				sidebarElement = CreateElement("sidebar_screenshot", "STRING_TOOLBAR_CAMERA", ScreenshotHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_video_capture":
				sidebarElement = CreateElement("sidebar_video_capture", "STRING_RECORD_SCREEN", ScreenRecorderButtonHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_macro":
				sidebarElement = CreateElement("sidebar_macro", "STRING_MACRO_RECORDER", MacroButtonHandler);
				((UIElement)sidebarElement).MouseLeave += new MouseEventHandler(MMacroButtonAndPopup_MouseLeave);
				AddElement(sidebarElement);
				break;
			case "sidebar_operation":
				sidebarElement = CreateElement("sidebar_operation", "STRING_SYNCHRONISER", OperationSyncHandler);
				AddElement(sidebarElement);
				break;
			case "sidebar_lock_cursor":
				sidebarElement = CreateElement("sidebar_lock_cursor", "STRING_TOGGLE_LOCK_CURSOR", LockCursorHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_location":
				sidebarElement = CreateElement("sidebar_location", "STRING_SET_LOCATION", LocationHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_shake":
				sidebarElement = CreateElement("sidebar_shake", "STRING_SHAKE", ShakeHandler);
				AddElement(sidebarElement);
				break;
			case "sidebar_rotate":
				sidebarElement = CreateElement("sidebar_rotate", "STRING_ROTATE", RotateHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_fullscreen":
				sidebarElement = CreateElement("sidebar_fullscreen", "STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", FullScreenHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			case "sidebar_mm":
				sidebarElement = CreateElement("sidebar_mm", "STRING_TOGGLE_MULTIINSTANCE_WINDOW", MIManagerHandler);
				AddElement(sidebarElement);
				break;
			case "sidebar_media_folder":
				sidebarElement = CreateElement("sidebar_media_folder", "STRING_OPEN_MEDIA_FOLDER", MediaFolderHandler);
				AddElement(sidebarElement);
				break;
			case "sidebar_gamepad":
				sidebarElement = CreateElement("sidebar_gamepad", "STRING_GAMEPAD_CONTROLS", GamepadControlsWindowHandler);
				AddElement(sidebarElement);
				ChangeElementState(sidebarElement, isEnabled: false);
				break;
			default:
				Logger.Warning("Unhandled sidebar element found: {0}", new object[1] { l });
				break;
			}
			if (l == ls.Last())
			{
				sidebarElement.IsLastElementOfGroup = true;
				sidebarElement.IsCurrentLastElementOfGroup = true;
				IncreaseElementBottomMarginIfLast(sidebarElement);
			}
		}
	}

	private void StreamingHandler(object sender, EventArgs e)
	{
		bool mIsStreaming = ParentWindow.mIsStreaming;
		NCSoftUtils.Instance.SendStreamingEvent(ParentWindow.mVmName, mIsStreaming ? "off" : "on");
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, mIsStreaming ? "StreamVideoOff" : "StreamVideoOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void GamepadControlsWindowHandler(object sender, EventArgs e)
	{
		if (!ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
		{
			KMManager.HandleInputMapperWindow(ParentWindow, "gamepad");
		}
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamePad", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName);
	}

	private void MediaFolderHandler(object sender, EventArgs e)
	{
		CommonHandlers.OpenMediaFolder();
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void GoBackHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.BackButtonHandler();
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void GoHomeHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.HomeButtonHandler();
	}

	private void GoSettingsHandler(object sender, EventArgs e)
	{
		string empty = string.Empty;
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		ParentWindow.mCommonHandler.LaunchSettingsWindow(empty);
	}

	private void MIManagerHandler(object sender, EventArgs e)
	{
		try
		{
			Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), "-IsMIMLaunchedFromClient");
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't launch MI Manager. Ex: {0}", new object[1] { ex.Message });
		}
	}

	private void RotateHandler(object sender, EventArgs e)
	{
		RotateButtonHandler("MouseClick");
	}

	internal void RotateButtonHandler(string action)
	{
		mIsUIInPortraitModeBeforeChange = (ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab ? true : false);
		ParentWindow.AppForcedOrientationDict[ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName] = !mIsUIInPortraitModeBeforeChange;
		ParentWindow.ChangeOrientationFromClient(!mIsUIInPortraitModeBeforeChange);
		string arg = (mIsUIInPortraitModeBeforeChange ? "landscape" : "portrait");
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Rotate", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, arg, ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName);
	}

	private void ShakeHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.ShakeButtonHandler();
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Shake", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName);
	}

	private void LocationHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.LocationButtonHandler();
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void LockCursorHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: true, "MouseClick", "sidebar");
	}

	private void ScreenshotHandler(object sender, EventArgs e)
	{
		ToolTipService.SetToolTip((DependencyObject)(object)(sender as SidebarElement), (object)null);
		((Popup)mScreenshotPopup).IsOpen = false;
		ThreadPool.QueueUserWorkItem(delegate
		{
			Thread.Sleep(100);
			ParentWindow.mCommonHandler.ScreenShotButtonHandler();
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		});
	}

	private void VolumeButtonHandler(object sender, EventArgs e)
	{
		if (!GetElementFromTag("sidebar_volume").IsInMainSidebar)
		{
			((Popup)mVolumeSliderPopup).StaysOpen = false;
		}
		else
		{
			((Popup)mVolumeSliderPopup).StaysOpen = true;
		}
		if (((Popup)mVolumeSliderPopup).IsOpen)
		{
			((Popup)mVolumeSliderPopup).IsOpen = false;
		}
		else
		{
			((Popup)mVolumeSliderPopup).IsOpen = true;
		}
	}

	private void FullScreenHandler(object sender, EventArgs e)
	{
		if (!ParentWindow.mStreamingModeEnabled)
		{
			ParentWindow.mCommonHandler.FullScreenButtonHandler("sidebar", "MouseClick");
		}
	}

	internal SidebarElement GetElementFromTag(string tag)
	{
		if (mListSidebarElements.Count >= 1)
		{
			return mListSidebarElements.Where((SidebarElement item) => (string)((FrameworkElement)item).Tag == tag).FirstOrDefault();
		}
		return null;
	}

	public void AddElement(SidebarElement ele, bool isStaticElement = false)
	{
		if (isStaticElement)
		{
			((Panel)mStaticButtonsStackPanel).Children.Add((UIElement)(object)ele);
		}
		else
		{
			((Panel)mElementsStackPanel).Children.Add((UIElement)(object)ele);
		}
	}

	public void UpdateToDefaultImage(string tag)
	{
		UpdateToDefaultImage(GetElementFromTag(tag));
	}

	public void UpdateImage(string tag, string newImage)
	{
		UpdateImage(GetElementFromTag(tag), newImage);
	}

	public static void UpdateToDefaultImage(SidebarElement ele)
	{
		if (ele != null)
		{
			ele.Image.ImageName = (string)((FrameworkElement)ele).Tag;
		}
	}

	public static void UpdateImage(SidebarElement ele, string newImage)
	{
		if (ele != null)
		{
			ele.Image.ImageName = newImage;
		}
	}

	private static void DecreaseElementBottomMargin(SidebarElement ele)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Thickness margin = ((FrameworkElement)ele).Margin;
		((Thickness)(ref margin)).Bottom = 2.0;
		((FrameworkElement)ele).Margin = margin;
	}

	private static void IncreaseElementBottomMarginIfLast(SidebarElement ele)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (ele.IsCurrentLastElementOfGroup)
		{
			Thickness margin = ((FrameworkElement)ele).Margin;
			((Thickness)(ref margin)).Bottom = 10.0;
			((FrameworkElement)ele).Margin = margin;
		}
	}

	private SidebarElement CreateElement(string imageName, string toolTipKey, EventHandler evt)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		SidebarElement sidebarElement = new SidebarElement();
		((FrameworkElement)sidebarElement).Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
		((UIElement)sidebarElement).Visibility = (Visibility)0;
		sidebarElement.mSidebarElementTooltipKey = toolTipKey;
		SidebarElement sidebarElement2 = sidebarElement;
		sidebarElement2.Image.ImageName = imageName;
		((FrameworkElement)sidebarElement2).Tag = imageName;
		((UIElement)sidebarElement2).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SidebarElement_PreviewMouseLeftButtonUp);
		((UIElement)sidebarElement2).IsVisibleChanged += new DependencyPropertyChangedEventHandler(SidebarElement_IsVisibleChanged);
		SetSidebarElementTooltip(sidebarElement2, toolTipKey);
		mDictActions.Add(sidebarElement2, evt);
		mListSidebarElements.Add(sidebarElement2);
		return sidebarElement2;
	}

	internal void SetSidebarElementTooltip(SidebarElement ele, string toolTipKey)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			string text = null;
			if (((FrameworkElement)ele).Tag.ToString() == "sidebar_volume")
			{
				text = GetTooltip(LocaleStrings.GetLocalizedString("STRING_INCREASE_VOLUME", ""), ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_INCREASE_VOLUME"), " ");
				text = text + "\n" + GetTooltip(LocaleStrings.GetLocalizedString("STRING_DECREASE_VOLUME", ""), ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_DECREASE_VOLUME"), " ");
				((FrameworkElement)mVolumeMuteUnmuteImage).ToolTip = GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_MUTE_STATE", ""), ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_MUTE_STATE"));
			}
			else
			{
				text = GetTooltip(LocaleStrings.GetLocalizedString(toolTipKey, ""), ParentWindow.mCommonHandler.GetShortcutKeyFromName(toolTipKey));
			}
			((FrameworkElement)ele).ToolTip = (object)new ToolTip
			{
				Content = text
			};
		}, new object[0]);
	}

	private static string GetTooltip(string text, string shortcut, string delimiter = "\n")
	{
		if (!string.IsNullOrEmpty(shortcut))
		{
			return string.Format(CultureInfo.InvariantCulture, text + delimiter + "(" + shortcut + ")", new object[0]);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return null;
	}

	private void SidebarElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!mIsOneSidebarElementLoadedBinded && (bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			mIsOneSidebarElementLoadedBinded = true;
			if (sender is SidebarElement sidebarElement && mSidebarElementApproxHeight == 0.0)
			{
				int num = (int)((FrameworkElement)sidebarElement).Height;
				Thickness margin = ((FrameworkElement)sidebarElement).Margin;
				int num2 = num + 2 * (int)((Thickness)(ref margin)).Top;
				int num3 = mListSidebarElements.Where((SidebarElement item) => item.IsLastElementOfGroup).Count();
				int num4 = num2 * ((Panel)mElementsStackPanel).Children.Count + (num3 - 1) * 8;
				mSidebarElementApproxHeight = num4 / ((Panel)mElementsStackPanel).Children.Count + 2;
				Logger.Info("Aprrox: {0}", new object[1] { mSidebarElementApproxHeight });
			}
		}
		UpdateTotalVisibleElementCount();
	}

	private void SidebarElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		SidebarElement sidebarElement = sender as SidebarElement;
		if (mDictActions.ContainsKey(sidebarElement))
		{
			mDictActions[sidebarElement]?.Invoke(sidebarElement, new EventArgs());
		}
	}

	private void MSidebar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		ArrangeAllSidebarElements();
	}

	internal void ArrangeAllSidebarElements()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((int)((UIElement)this).Visibility != 0)
			{
				return;
			}
			int num = Math.Min((int)(Math.Max(0.0, ((FrameworkElement)this).ActualHeight - ((FrameworkElement)mBottomGrid).ActualHeight - 54.0) / mSidebarElementApproxHeight), mTotalVisibleElements);
			List<SidebarElement> list = (from item in ((IEnumerable)((Panel)mElementsStackPanel).Children).OfType<SidebarElement>()
				where (int)((UIElement)item).Visibility == 0
				select item).ToList();
			int count = list.Count;
			if (count > num)
			{
				int num2 = count - num;
				for (int num3 = 1; num3 <= num2; num3++)
				{
					SidebarElement sidebarElement = list[count - num3];
					((Panel)mElementsStackPanel).Children.Remove((UIElement)(object)sidebarElement);
					sidebarElement.IsInMainSidebar = false;
				}
			}
			else if (count < num)
			{
				int num4 = num - count;
				SidebarElement[] array = mListSidebarElements.Where((SidebarElement item) => !item.IsInMainSidebar).ToArray();
				for (int num5 = 0; num5 < num4; num5++)
				{
					if (array.Length > num5)
					{
						SidebarElement sidebarElement2 = array[num5];
						sidebarElement2.IsInMainSidebar = true;
						AddToVisibleElementsPanel(sidebarElement2);
					}
				}
			}
			if (mListSidebarElements.Any((SidebarElement x) => !x.IsInMainSidebar))
			{
				((UIElement)mMoreButton).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mMoreButton).Visibility = (Visibility)2;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("XXX SR: An error occured while rearranging elements. Ex: {0}", new object[1] { ex });
		}
	}

	private void AddToVisibleElementsPanel(SidebarElement ele)
	{
		DependencyObject parent = ((FrameworkElement)ele).Parent;
		StackPanel val = (StackPanel)(object)((parent is StackPanel) ? parent : null);
		if (val != null)
		{
			((Panel)val).Children.Remove((UIElement)(object)ele);
		}
		((Panel)mElementsStackPanel).Children.Add((UIElement)(object)ele);
		IncreaseElementBottomMarginIfLast(ele);
	}

	public void SetHeight()
	{
		((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow.mContentGrid).ActualHeight;
	}

	private void MMoreElements_Opened(object sender, EventArgs e)
	{
		SidebarPopupContentClear();
		mSidebarPopup.InitAllElements(mListSidebarElements.Where((SidebarElement x) => !x.IsInMainSidebar));
		UpdateImage(mMoreButton, "sidebar_options_open");
		BlueStacksUIBinding.Bind((UserControl)(object)mMoreButton, "STRING_CLOSE");
	}

	private void MMoreElements_Closed(object sender, EventArgs e)
	{
		SidebarPopupContentClear();
		UpdateImage(mMoreButton, "sidebar_options_close");
		BlueStacksUIBinding.Bind((UserControl)(object)mMoreButton, "STRING_MORE_BUTTON");
	}

	private void SidebarPopupContentClear()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (StackPanel child in ((Panel)mSidebarPopup.mMainStackPanel).Children)
		{
			((Panel)child).Children.Clear();
		}
		((Panel)mSidebarPopup.mMainStackPanel).Children.Clear();
	}

	private void MSidebarPopup_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void MMoreButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mMoreElements).IsOpen = !((Popup)mMoreElements).IsOpen;
	}

	private void MMoreButton_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		if (!((UIElement)mMoreButton).IsMouseOver && mListPopups.All((CustomPopUp x) => !((UIElement)x).IsMouseOver) && ((Popup)mMoreElements).IsOpen)
		{
			if (mSidebarPopupTimer == null)
			{
				mSidebarPopupTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 0, 500)
				};
				mSidebarPopupTimer.Tick += SidebarPopupTimer_Tick;
			}
			else
			{
				mSidebarPopupTimer.Stop();
			}
			mSidebarPopupTimer.Start();
		}
	}

	private void SidebarPopupTimer_Tick(object sender, EventArgs e)
	{
		if (!((UIElement)mMoreButton).IsMouseOver && mListPopups.All((CustomPopUp x) => !((UIElement)x).IsMouseOver))
		{
			mListPopups.Select(delegate(CustomPopUp c)
			{
				((Popup)c).IsOpen = false;
				return c;
			}).ToList();
			if (mIsInFullscreenMode && !((UIElement)this).IsMouseOver)
			{
				ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((RangeBase)transSlider).Value == 0.0)
		{
			((RangeBase)transSlider).Value = mLastSliderValue;
			mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_popup";
			return;
		}
		double value = ((RangeBase)transSlider).Value;
		((RangeBase)transSlider).Value = 0.0;
		mLastSliderValue = value;
		mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
	}

	private void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		KMManager.ChangeTransparency(ParentWindow, ((RangeBase)transSlider).Value);
		if (((RangeBase)transSlider).Value == 0.0)
		{
			mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
			if (!RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
			}
			ParentWindow_OverlayStateChangedEvent(isEnabled: false);
		}
		else
		{
			mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_popup";
			KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
			ParentWindow_OverlayStateChangedEvent(isEnabled: true);
		}
		mLastSliderValue = ((RangeBase)transSlider).Value;
	}

	private void OverlayTooltipCPB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mOverlayTooltip).IsOpen = false;
		mIsOverlayTooltipClosed = true;
		((RoutedEventArgs)e).Handled = true;
	}

	private void OverlayDoNotShowCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (string.Equals(mOverlayDoNotShowCheckboxImage.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
		{
			mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox_checked";
			RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
		}
		else
		{
			mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox";
			RegistryManager.Instance.OverlayAvailablePromptEnabled = true;
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void ScreenRecorderButtonHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.DownloadAndLaunchRecording("sidebar", "MouseClick");
	}

	private void RecordScreenPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mRecordScreenPopup).IsOpen = false;
	}

	private void RecordScreenPopup_Closed(object sender, EventArgs e)
	{
		if (CommonHandlers.sIsRecordingVideo)
		{
			BlueStacksUIBinding.Bind(RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
			((UIElement)RecordScreenPopupHeader).Visibility = (Visibility)0;
			((UIElement)RecordScreenPopupBody).Visibility = (Visibility)2;
			((UIElement)mRecordScreenClose).Visibility = (Visibility)2;
		}
		else
		{
			BlueStacksUIBinding.Bind(RecordScreenPopupHeader, "STRING_RECORD_SCREEN", "");
			BlueStacksUIBinding.Bind(RecordScreenPopupBody, "STRING_RECORD_SCREEN_PLAYING", "");
			((UIElement)RecordScreenPopupHeader).Visibility = (Visibility)0;
			((UIElement)RecordScreenPopupBody).Visibility = (Visibility)0;
			((UIElement)mRecordScreenClose).Visibility = (Visibility)2;
		}
		((Popup)mRecordScreenPopup).StaysOpen = true;
		((UIElement)RecordScreenPopupHyperlink).Visibility = (Visibility)2;
	}

	private void KeymappingControlsTransparencyButtonHandler(object sender, EventArgs e)
	{
		RegistryManager.Instance.ShowKeyControlsOverlay = true;
		RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
		KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
		if (!GetElementFromTag("sidebar_overlay").IsInMainSidebar)
		{
			((Popup)mChangeTransparencyPopup).StaysOpen = false;
		}
		else
		{
			((Popup)mChangeTransparencyPopup).StaysOpen = true;
		}
		((Popup)mChangeTransparencyPopup).IsOpen = true;
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void RecordScreenPopupHyperlink_Click(object sender, RoutedEventArgs e)
	{
		CommonHandlers.OpenMediaFolderWithFileSelected(CommonHandlers.mSavedVideoRecordingFilePath);
		((Popup)mRecordScreenPopup).IsOpen = false;
	}

	private void RecordScreenClose_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((FrameworkElement)RecordScreenPopupHeader).Margin = new Thickness(0.0, 0.0, 20.0, 0.0);
		}
		else
		{
			((FrameworkElement)RecordScreenPopupHeader).Margin = new Thickness(0.0);
		}
	}

	internal void SidebarVisiblityChanged(Visibility currentVisibility)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!mIsInFullscreenMode && ((FrameworkElement)this).IsLoaded)
		{
			if ((int)currentVisibility == 0)
			{
				ParentWindow.ParentWindowWidthDiff = 62;
				((FrameworkElement)ParentWindow).Width = ((FrameworkElement)ParentWindow).ActualWidth + ((FrameworkElement)this).Width;
				ArrangeAllSidebarElements();
			}
			else
			{
				ParentWindow.ParentWindowWidthDiff = 2;
				((FrameworkElement)ParentWindow).Width = Math.Max(((FrameworkElement)ParentWindow).ActualWidth - ((FrameworkElement)this).Width, ((FrameworkElement)ParentWindow).MinWidth);
			}
			ParentWindow.HandleDisplaySettingsChanged();
			((FrameworkElement)ParentWindow).Height = ParentWindow.GetHeightFromWidth(((FrameworkElement)ParentWindow).Width);
		}
	}

	private void MMacroButtonPopup_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mMacroButtonPopup).IsOpen = true;
	}

	internal void ShowOverlayTooltip(bool isShow, bool force = false)
	{
		if (GetElementFromTag("sidebar_overlay") == null || !RegistryManager.Instance.OverlayAvailablePromptEnabled)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (isShow)
			{
				mIsPendingShowOverlayTooltip = true;
				ActualOverlayTooltip(force);
			}
			else
			{
				((Popup)mOverlayTooltip).IsOpen = false;
			}
		}, new object[0]);
	}

	private void ActualOverlayTooltip(bool force = false)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (RegistryManager.Instance.OverlayAvailablePromptEnabled && !mIsOverlayTooltipClosed && mIsPendingShowOverlayTooltip && (!RegistryManager.Instance.IsAutoShowGuidance || Array.Exists(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == ParentWindow.StaticComponents.mSelectedTabButton.PackageName) || force) && !mIsInFullscreenMode && !FeatureManager.Instance.IsCustomUIForNCSoft && (int)((UIElement)this).Visibility == 0)
		{
			mIsPendingShowOverlayTooltip = false;
			((Popup)mOverlayTooltip).IsOpen = true;
		}
	}

	private void ActualKeymapPopup()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		if (!RegistryManager.Instance.OverlayAvailablePromptEnabled || mIsOverlayTooltipClosed)
		{
			string packageName = ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
			if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(packageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][packageName] = new AppSettings();
			}
			if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][packageName].IsKeymappingTooltipShown)
			{
				AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][packageName].IsKeymappingTooltipShown = true;
			}
		}
	}

	internal void ShowKeyMapPopup(bool isShow)
	{
		if (GetElementFromTag("sidebar_controls") != null && isShow)
		{
			if (!Array.Exists(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == ParentWindow.StaticComponents.mSelectedTabButton.PackageName) && RegistryManager.Instance.IsAutoShowGuidance)
			{
				KMManager.HandleInputMapperWindow(ParentWindow);
			}
			else
			{
				ActualKeymapPopup();
			}
		}
	}

	private void OpenMacroGridPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
		((Popup)mMacroButtonPopup).IsOpen = false;
		ToggleSidebarVisibilityInFullscreen(isVisible: false);
		ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "MacroBookmarkPopup");
	}

	private void OpenMacroGridMouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void OpenMacroGridMouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "SidebarBackground");
	}

	private void VolumeImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.MuteUnmuteButtonHanlder();
		if (ParentWindow.EngineInstanceRegistry.IsMuted)
		{
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		else
		{
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
	}

	private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		mCurrentVolumeValue.Text = Math.Round(e.NewValue).ToString(CultureInfo.InvariantCulture);
	}

	private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		int volumeInFrontendAsync = Convert.ToInt32(((RangeBase)mVolumeSlider).Value);
		ParentWindow.Utils.SetVolumeInFrontendAsync(volumeInFrontendAsync);
	}

	private void MuteInstancesCheckboxImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (mMuteInstancesCheckboxImage.ImageName.Equals("bgpcheckbox", StringComparison.OrdinalIgnoreCase))
		{
			mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
			RegistryManager.Instance.AreAllInstancesMuted = true;
			SendMuteUnmuteRequestToAllInstances(isMute: true);
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOffAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		else
		{
			mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
			RegistryManager.Instance.AreAllInstancesMuted = false;
			SendMuteUnmuteRequestToAllInstances(isMute: false);
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOnAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void SendMuteUnmuteRequestToAllInstances(bool isMute)
	{
		string[] vmList = RegistryManager.Instance.VmList;
		foreach (string text in vmList)
		{
			if (isMute)
			{
				if (Enumerable.Contains(BlueStacksUIUtils.DictWindows.Keys, text))
				{
					BlueStacksUIUtils.DictWindows[text].Utils.MuteApplication(allInstances: true);
				}
			}
			else if (Enumerable.Contains(BlueStacksUIUtils.DictWindows.Keys, text))
			{
				BlueStacksUIUtils.DictWindows[text].Utils.UnmuteApplication(allInstances: true);
			}
		}
	}

	private void ChangeTransparencyPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (((Popup)mChangeTransparencyPopup).IsOpen)
		{
			if (mChangeTransparencyPopupTimer == null)
			{
				mChangeTransparencyPopupTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 0, 500)
				};
				mChangeTransparencyPopupTimer.Tick += ChangeTransparencyPopupTimer_Tick;
			}
			else
			{
				mChangeTransparencyPopupTimer.Stop();
			}
			mChangeTransparencyPopupTimer.Start();
		}
	}

	private void ChangeTransparencyPopupTimer_Tick(object sender, EventArgs e)
	{
		if (!((UIElement)mChangeTransparencyPopup).IsMouseOver && !((UIElement)GetElementFromTag("sidebar_overlay")).IsMouseOver)
		{
			((Popup)mChangeTransparencyPopup).IsOpen = false;
			if (mIsInFullscreenMode && !((UIElement)this).IsMouseOver)
			{
				ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	private void ChangeTransparencyPopup_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!GetElementFromTag("sidebar_overlay").IsInMainSidebar)
		{
			((Popup)mChangeTransparencyPopup).StaysOpen = false;
		}
		else
		{
			((Popup)mChangeTransparencyPopup).StaysOpen = true;
		}
		((Popup)mChangeTransparencyPopup).IsOpen = true;
	}

	private void VolumeSliderPopup_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mVolumeSliderPopup).IsOpen = true;
	}

	private void VolumeSliderPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if (((Popup)mVolumeSliderPopup).IsOpen)
		{
			if (mVolumeSliderPopupTimer == null)
			{
				mVolumeSliderPopupTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 1)
				};
				mVolumeSliderPopupTimer.Tick += VolumeSliderPopupTimer_Tick;
			}
			else
			{
				mVolumeSliderPopupTimer.Stop();
			}
			mVolumeSliderPopupTimer.Start();
		}
	}

	internal void VolumeSliderPopupTimer_Tick(object sender, EventArgs e)
	{
		if (!((UIElement)mVolumeSliderPopup).IsMouseOver && !((UIElement)GetElementFromTag("sidebar_volume")).IsMouseOver)
		{
			((Popup)mVolumeSliderPopup).IsOpen = false;
			if (mIsInFullscreenMode && !((UIElement)this).IsMouseOver)
			{
				ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	private void ScreenshotPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mScreenshotPopup).IsOpen = false;
	}

	private void ScreenshotPopupHyperlink_Click(object sender, RoutedEventArgs e)
	{
		CommonHandlers.OpenMediaFolderWithFileSelected(currentScreenshotSavedPath);
		((Popup)mScreenshotPopup).IsOpen = false;
	}

	private void GameControlButtonPopup_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!GetElementFromTag("sidebar_controls").IsInMainSidebar)
		{
			((Popup)mGameControlButtonPopup).StaysOpen = false;
		}
		else
		{
			((Popup)mGameControlButtonPopup).StaysOpen = true;
		}
		((Popup)mGameControlButtonPopup).IsOpen = true;
	}

	private void GameControlButtonPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (((Popup)mGameControlButtonPopup).IsOpen)
		{
			if (mGameControlBookmarkTimer == null)
			{
				mGameControlBookmarkTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 0, 500)
				};
				mGameControlBookmarkTimer.Tick += GameControlBookmarkTimer_Tick;
			}
			else
			{
				mGameControlBookmarkTimer.Stop();
			}
			mGameControlBookmarkTimer.Start();
		}
	}

	private void GameControlBookmarkTimer_Tick(object sender, EventArgs e)
	{
		if (!((UIElement)mGameControlButtonPopup).IsMouseOver && !((UIElement)GetElementFromTag("sidebar_controls")).IsMouseOver)
		{
			((Popup)mGameControlButtonPopup).IsOpen = false;
			if (ParentWindow.mIsFullScreen)
			{
				ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		((DispatcherTimer)((sender is DispatcherTimer) ? sender : null)).Stop();
	}

	private void GameControlButtonHandler(object sender, EventArgs e)
	{
		bool flag = true;
		((Panel)mBookmarkedSchemesStackPanel).Children.Clear();
		foreach (IMControlScheme controlScheme in ParentWindow.SelectedConfig.ControlSchemes)
		{
			if (controlScheme.IsBookMarked)
			{
				SchemeBookmarkControl schemeBookmarkControl = new SchemeBookmarkControl(controlScheme, ParentWindow);
				((Panel)mBookmarkedSchemesStackPanel).Children.Add((UIElement)(object)schemeBookmarkControl);
				flag = false;
			}
		}
		if (!flag)
		{
			if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed)
			{
				((Window)KMManager.sGuidanceWindow).Close();
			}
			if (!GetElementFromTag("sidebar_controls").IsInMainSidebar)
			{
				((Popup)mGameControlButtonPopup).StaysOpen = false;
			}
			else
			{
				((Popup)mGameControlButtonPopup).StaysOpen = true;
			}
			((Popup)mGameControlButtonPopup).IsOpen = true;
		}
		else
		{
			ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "sidebar");
			((Popup)mGameControlButtonPopup).IsOpen = false;
			ToggleSidebarVisibilityInFullscreen(isVisible: false);
		}
	}

	private void OpenGameControlPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "sidebar");
		((Popup)mGameControlButtonPopup).IsOpen = false;
		ToggleSidebarVisibilityInFullscreen(isVisible: false);
	}

	private void OpenGameControlMouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void OpenGameControlMouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ComboBoxBackgroundColor");
	}

	internal void ChangeVideoRecordingImage(string imageName)
	{
		SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
		if (elementFromTag != null)
		{
			elementFromTag.Image.IsImageToBeRotated = false;
			UpdateImage(elementFromTag, imageName);
		}
	}

	private void SetVideoRecordingTooltipForNCSoft()
	{
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
			if (elementFromTag != null)
			{
				object toolTip = ((FrameworkElement)elementFromTag).ToolTip;
				object obj = ((toolTip is ToolTip) ? toolTip : null);
				((ContentControl)obj).Content = Convert.ToString(((ContentControl)obj).Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
			}
		}
	}

	internal void SetVideoRecordingTooltip(bool isRecording)
	{
		SidebarElement elementFromTag = GetElementFromTag("sidebar_video_capture");
		if (elementFromTag == null)
		{
			return;
		}
		object toolTip = ((FrameworkElement)elementFromTag).ToolTip;
		ToolTip val = (ToolTip)((toolTip is ToolTip) ? toolTip : null);
		if (isRecording)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				((ContentControl)val).Content = Convert.ToString(((ContentControl)val).Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
			}
			else
			{
				((ContentControl)val).Content = Convert.ToString(((ContentControl)val).Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
			}
		}
		else if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((ContentControl)val).Content = Convert.ToString(((ContentControl)val).Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
		}
		else
		{
			((ContentControl)val).Content = Convert.ToString(((ContentControl)val).Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""));
		}
	}

	private void VolumeSliderPopup_Closed(object sender, EventArgs e)
	{
		if (!GetElementFromTag("sidebar_volume").IsInMainSidebar)
		{
			MMoreButton_MouseLeave(null, null);
		}
	}

	private void GameControlButtonPopup_Closed(object sender, EventArgs e)
	{
		if (!GetElementFromTag("sidebar_controls").IsInMainSidebar)
		{
			MMoreButton_MouseLeave(null, null);
		}
	}

	private void ChangeTransparencyPopup_Closed(object sender, EventArgs e)
	{
		if (!GetElementFromTag("sidebar_overlay").IsInMainSidebar)
		{
			MMoreButton_MouseLeave(null, null);
		}
	}

	private void OpenGameGuideButtonHandler(object sender, EventArgs e)
	{
		ParentWindow.mCommonHandler.GameGuideButtonHandler("MouseClick", "sidebar");
	}

	public OnBoardingPopupWindow FullscreenOnboardingBlurb()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)ParentWindow.mSidebar).Visibility == 2 || string.IsNullOrEmpty(ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP")))
		{
			return null;
		}
		SidebarElement sidebarElement = (from SidebarElement s in (IEnumerable)((Panel)ParentWindow.mSidebar.mElementsStackPanel).Children
			where s.Image.ImageName == "sidebar_fullscreen"
			select s).FirstOrDefault();
		if (sidebarElement == null)
		{
			return null;
		}
		OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		((Window)onBoardingPopupWindow).Owner = (Window)(object)ParentWindow;
		onBoardingPopupWindow.PlacementTarget = (UIElement)(object)sidebarElement;
		((Window)onBoardingPopupWindow).Title = "FullScreenBlurb";
		onBoardingPopupWindow.LeftMargin = 310;
		onBoardingPopupWindow.TopMargin = 45;
		onBoardingPopupWindow.IsBlurbRelatedToGuidance = false;
		onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_HEADER", "");
		OnBoardingPopupWindow onBoardingPopupWindow2 = onBoardingPopupWindow;
		((UIElement)onBoardingPopupWindow2.bodyTextBlock).Visibility = (Visibility)2;
		((UIElement)onBoardingPopupWindow2.bodyContentBlurbControl).Visibility = (Visibility)0;
		onBoardingPopupWindow2.bodyContentBlurbControl.FirstMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_MESSAGE", "");
		onBoardingPopupWindow2.bodyContentBlurbControl.KeyMessage.Text = ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP");
		onBoardingPopupWindow2.bodyContentBlurbControl.SecondMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_FULL_SCREEN_MESSAGE", "");
		onBoardingPopupWindow2.PopArrowAlignment = (PopupArrowAlignment)2;
		DependencyProperty leftProperty = Window.LeftProperty;
		Point val = ((Visual)sidebarElement).PointToScreen(new Point(0.0, 0.0));
		((DependencyObject)onBoardingPopupWindow2).SetValue(leftProperty, (object)(((Point)(ref val)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow2.LeftMargin));
		DependencyProperty topProperty = Window.TopProperty;
		val = ((Visual)sidebarElement).PointToScreen(new Point(0.0, 0.0));
		((DependencyObject)onBoardingPopupWindow2).SetValue(topProperty, (object)(((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow2.TopMargin));
		((FrameworkElement)onBoardingPopupWindow2.RightArrow).Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
		((FrameworkElement)onBoardingPopupWindow2.RightArrow).VerticalAlignment = (VerticalAlignment)0;
		return onBoardingPopupWindow2;
	}

	public void ShowViewGuidancePopup()
	{
		SidebarElement sidebarElement = (from SidebarElement s in (IEnumerable)((Panel)ParentWindow.mSidebar.mStaticButtonsStackPanel).Children
			where s.Image.ImageName == "sidebar_gameguide" || s.Image.ImageName == "sidebar_gameguide_active"
			select s).FirstOrDefault();
		if (sidebarElement != null)
		{
			((Popup)mGameControlsBlurbPopup).PlacementTarget = (UIElement)(object)sidebarElement;
			((Popup)mGameControlsBlurbPopup).IsOpen = true;
			mGameControlsBlurbPopup.IsTopmost = true;
		}
	}

	private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", ParentWindow.mVmName, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, "ViewControlBlurb", "");
		((Popup)mGameControlsBlurbPopup).IsOpen = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebar.xaml", UriKind.Relative);
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
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected O, but got Unknown
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected O, but got Unknown
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Expected O, but got Unknown
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Expected O, but got Unknown
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Expected O, but got Unknown
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Expected O, but got Unknown
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Expected O, but got Unknown
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Expected O, but got Unknown
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Expected O, but got Unknown
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Expected O, but got Unknown
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected O, but got Unknown
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Expected O, but got Unknown
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Expected O, but got Unknown
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Expected O, but got Unknown
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Expected O, but got Unknown
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Expected O, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected O, but got Unknown
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Expected O, but got Unknown
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Expected O, but got Unknown
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Expected O, but got Unknown
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Expected O, but got Unknown
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Expected O, but got Unknown
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Expected O, but got Unknown
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Expected O, but got Unknown
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Expected O, but got Unknown
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Expected O, but got Unknown
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Expected O, but got Unknown
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Expected O, but got Unknown
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Expected O, but got Unknown
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Expected O, but got Unknown
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Expected O, but got Unknown
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Expected O, but got Unknown
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Expected O, but got Unknown
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Expected O, but got Unknown
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Expected O, but got Unknown
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Expected O, but got Unknown
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Expected O, but got Unknown
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Expected O, but got Unknown
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Expected O, but got Unknown
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Expected O, but got Unknown
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mSidebar = (Sidebar)target;
			((FrameworkElement)mSidebar).SizeChanged += new SizeChangedEventHandler(MSidebar_SizeChanged);
			((FrameworkElement)mSidebar).Loaded += new RoutedEventHandler(Sidebar_Loaded);
			break;
		case 2:
			mBorder = (Border)target;
			break;
		case 3:
			mGrid = (Grid)target;
			break;
		case 4:
			mTopGrid = (Grid)target;
			break;
		case 5:
			mElementsStackPanel = (StackPanel)target;
			break;
		case 6:
			mMoreButton = (SidebarElement)target;
			break;
		case 7:
			mChangeTransparencyPopup = (CustomPopUp)target;
			break;
		case 8:
			mMaskBorder2 = (Border)target;
			break;
		case 9:
			mTranslucentControlsSliderButton = (CustomPictureBox)target;
			((UIElement)mTranslucentControlsSliderButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
			break;
		case 10:
			transSlider = (Slider)target;
			((RangeBase)transSlider).ValueChanged += TransparencySlider_ValueChanged;
			break;
		case 11:
			mVolumeSliderPopup = (CustomPopUp)target;
			break;
		case 12:
			mMaskBorder3 = (Border)target;
			break;
		case 13:
			mVolumeMuteUnmuteImage = (CustomPictureBox)target;
			((UIElement)mVolumeMuteUnmuteImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeImage_PreviewMouseLeftButtonUp);
			break;
		case 14:
			mVolumeSlider = (Slider)target;
			((UIElement)mVolumeSlider).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(VolumeSlider_PreviewMouseLeftButtonUp);
			((RangeBase)mVolumeSlider).ValueChanged += VolumeSlider_ValueChanged;
			break;
		case 15:
			mCurrentVolumeValue = (TextBlock)target;
			break;
		case 16:
			mMuteInstancesCheckboxImage = (CustomPictureBox)target;
			((UIElement)mMuteInstancesCheckboxImage).MouseLeftButtonUp += new MouseButtonEventHandler(MuteInstancesCheckboxImage_MouseLeftButtonUp);
			break;
		case 17:
			mMuteAllInstancesTextBlock = (TextBlock)target;
			((UIElement)mMuteAllInstancesTextBlock).MouseLeftButtonUp += new MouseButtonEventHandler(MuteInstancesCheckboxImage_MouseLeftButtonUp);
			break;
		case 18:
			mOverlayTooltip = (CustomPopUp)target;
			break;
		case 19:
			mMaskBorder4 = (Border)target;
			break;
		case 20:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(OverlayTooltipCPB_MouseLeftButtonUp);
			break;
		case 21:
			mOverlayPopUpTitle = (TextBlock)target;
			break;
		case 22:
			mOverlayPopUpMessage = (TextBlock)target;
			break;
		case 23:
			mOverlayDoNotShowCheckboxImage = (CustomPictureBox)target;
			((UIElement)mOverlayDoNotShowCheckboxImage).MouseLeftButtonUp += new MouseButtonEventHandler(OverlayDoNotShowCheckbox_MouseLeftButtonUp);
			break;
		case 24:
			mOverlayDontShowPopUp = (TextBlock)target;
			((UIElement)mOverlayDontShowPopUp).MouseLeftButtonUp += new MouseButtonEventHandler(OverlayDoNotShowCheckbox_MouseLeftButtonUp);
			break;
		case 25:
			mMacroButtonPopup = (CustomPopUp)target;
			break;
		case 26:
			mMaskBorder5 = (Border)target;
			break;
		case 27:
			mMacroBookmarkPopup = (MacroBookmarksPopup)target;
			break;
		case 28:
			mCustomiseSectionTag = (Grid)target;
			break;
		case 29:
			mCustomiseSectionBorderLine = (Separator)target;
			break;
		case 30:
			((UIElement)(Grid)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OpenMacroGridPreviewMouseLeftButtonUp);
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(OpenMacroGridMouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(OpenMacroGridMouseLeave);
			break;
		case 31:
			mOpenMacroTextbox = (TextBlock)target;
			break;
		case 32:
			mGameControlButtonPopup = (CustomPopUp)target;
			break;
		case 33:
			mMaskBorder6 = (Border)target;
			break;
		case 34:
			mBookmarkedSchemesStackPanel = (StackPanel)target;
			break;
		case 35:
			((UIElement)(Grid)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OpenGameControlPreviewMouseLeftButtonUp);
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(OpenGameControlMouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(OpenGameControlMouseLeave);
			break;
		case 36:
			mOpenGameControlTextbox = (TextBlock)target;
			break;
		case 37:
			mRecordScreenPopup = (CustomPopUp)target;
			break;
		case 38:
			mMaskBorder7 = (Border)target;
			break;
		case 39:
			mRecordScreenClose = (CustomPictureBox)target;
			((UIElement)mRecordScreenClose).IsVisibleChanged += new DependencyPropertyChangedEventHandler(RecordScreenClose_IsVisibleChanged);
			((UIElement)mRecordScreenClose).MouseLeftButtonUp += new MouseButtonEventHandler(RecordScreenPopupClose_MouseLeftButtonUp);
			break;
		case 40:
			RecordScreenPopupHeader = (TextBlock)target;
			break;
		case 41:
			RecordScreenPopupBody = (TextBlock)target;
			break;
		case 42:
			RecordScreenPopupHyperlink = (TextBlock)target;
			break;
		case 43:
			((Hyperlink)target).Click += new RoutedEventHandler(RecordScreenPopupHyperlink_Click);
			break;
		case 44:
			mRecorderClickLink = (TextBlock)target;
			break;
		case 45:
			mScreenshotPopup = (CustomPopUp)target;
			break;
		case 46:
			mMaskBorder8 = (Border)target;
			break;
		case 47:
			mScreenshotClose = (CustomPictureBox)target;
			((UIElement)mScreenshotClose).MouseLeftButtonUp += new MouseButtonEventHandler(ScreenshotPopupClose_MouseLeftButtonUp);
			break;
		case 48:
			ScreenshotPopupHeader = (TextBlock)target;
			break;
		case 49:
			ScreenshotPopupHyperlink = (TextBlock)target;
			break;
		case 50:
			((Hyperlink)target).Click += new RoutedEventHandler(ScreenshotPopupHyperlink_Click);
			break;
		case 51:
			mScreenshotClickLink = (TextBlock)target;
			break;
		case 52:
			mGameControlsBlurbPopup = (CustomPopUp)target;
			break;
		case 53:
			mMaskBorder10 = (Border)target;
			break;
		case 54:
			ContentGrid = (Grid)target;
			break;
		case 55:
			headerTextBlock = (TextBlock)target;
			break;
		case 56:
			bodyTextBlock = (TextBlock)target;
			break;
		case 57:
			OkayButton = (CustomButton)target;
			((ButtonBase)OkayButton).Click += new RoutedEventHandler(OnBoardingPopupNext_Click);
			break;
		case 58:
			RightArrow = (Path)target;
			break;
		case 59:
			mMoreElements = (CustomPopUp)target;
			break;
		case 60:
			mPopupGrid = (Grid)target;
			break;
		case 61:
			mMaskBorder = (Border)target;
			break;
		case 62:
			mSidebarPopup = (SidebarPopup)target;
			break;
		case 63:
			mBottomGrid = (Grid)target;
			break;
		case 64:
			mStaticButtonsStackPanel = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
