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
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class NCSoftTopBar : UserControl, ITopBar, IComponentConnector
{
	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTitleIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mWindowHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Line mGamenameSeparator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCharacterName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mStreamingTopbarGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNcTopBarControlGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMacroRecordGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroTopBarRecordControl mMacroRecordControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMacroPlayGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroTopBarPlayControl mMacroPlayControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mVideoRecordingStatusGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal VideoRecordingStatus mVideoRecordStatusControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOperationsSyncGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mSyncMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStopSyncButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mControlBtnPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSettingsButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSettingsButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSettingsButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMinimizeButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMinimizeButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMinimizeButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMaximizeButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMaximizeButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMaximizeButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCloseButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSidebarButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSidebarButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSidebarButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMacroRecorderToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroRecordingTooltip;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mUpArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMacroRunningToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid grid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroRunningTooltip;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mSettingsDropdownPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mSettingsDropdownBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SettingsWindowDropdown mSettingsDropDownControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mSyncInstancesToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mUpwardArrow;

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

	string ITopBar.AppName
	{
		get
		{
			return mAppName.Text;
		}
		set
		{
			mAppName.Text = value;
		}
	}

	string ITopBar.CharacterName
	{
		get
		{
			return mCharacterName.Text;
		}
		set
		{
			mCharacterName.Text = value;
		}
	}

	public NCSoftTopBar()
	{
		InitializeComponent();
	}

	public void ChangeTopBarColor(string color)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, color);
	}

	private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.EngineInstanceRegistry.IsSidebarVisible && (int)((UIElement)this).Visibility == 0 && (int)((UIElement)ParentWindow.mSidebar).Visibility != 0 && !FeatureManager.Instance.IsCustomUIForDMM)
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mCommonHandler.FlipSidebarVisibility(mSidebarButtonImage, mSidebarButtonText);
			}, new object[0]);
		}
	}

	private void NCSoftTopBar_Loaded(object sender, RoutedEventArgs e)
	{
		if (!ParentWindow.mGuestBootCompleted)
		{
			ParentWindow.mCommonHandler.SetSidebarImageProperties(isVisible: false, mSidebarButtonImage, mSidebarButtonText);
			ParentWindow.GuestBootCompleted += ParentWindow_GuestBootCompletedEvent;
		}
		ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += NCTopBar_ScreenRecordingStateChangedEvent;
		VideoRecordingStatus videoRecordingStatus = mVideoRecordStatusControl;
		videoRecordingStatus.RecordingStoppedEvent = (Action)Delegate.Combine(videoRecordingStatus.RecordingStoppedEvent, new Action(NCTopBar_RecordingStoppedEvent));
	}

	private void NCTopBar_ScreenRecordingStateChangedEvent(bool isRecording)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (isRecording)
			{
				if ((int)((UIElement)mVideoRecordingStatusGrid).Visibility != 0 && CommonHandlers.sIsRecordingVideo)
				{
					mVideoRecordStatusControl.Init(ParentWindow);
					((UIElement)mVideoRecordingStatusGrid).Visibility = (Visibility)0;
				}
			}
			else
			{
				mVideoRecordStatusControl.ResetTimer();
				((UIElement)mVideoRecordingStatusGrid).Visibility = (Visibility)2;
			}
		}, new object[0]);
	}

	private void NCTopBar_RecordingStoppedEvent()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mVideoRecordingStatusGrid).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked minimize button");
		ParentWindow.MinimizeWindow();
	}

	internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Clicked Maximize\\Restore button");
		if ((int)((Window)ParentWindow).WindowState == 0)
		{
			ParentWindow.MaximizeWindow();
		}
		else
		{
			ParentWindow.RestoreWindows();
		}
	}

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked close Bluestacks button");
		ParentWindow.CloseWindow();
	}

	private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Panel)((sender is Grid) ? sender : null)).Children[0].Visibility == 0)
		{
			mSettingsDropDownControl.LateInit();
			((Popup)mSettingsDropdownPopup).IsOpen = true;
			mSettingsButtonImage.ImageName = "cfgmenu_selected";
		}
		else
		{
			ParentWindow.mCommonHandler.LaunchSettingsWindow();
		}
	}

	private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (!ParentWindow.EngineInstanceRegistry.IsClientOnTop)
		{
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
			((Window)ParentWindow).Topmost = true;
		}
		else
		{
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			((Window)ParentWindow).Topmost = false;
		}
	}

	private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.FlipSidebarVisibility(mSidebarButtonImage, mSidebarButtonText);
	}

	internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
	{
		if (ParentWindow.IsUIInPortraitMode)
		{
			((UIElement)mSettingsButton).Visibility = (Visibility)2;
		}
		mMacroPlayControl.Init(ParentWindow, record);
		((UIElement)mMacroPlayGrid).Visibility = (Visibility)0;
	}

	internal void HideMacroPlaybackFromTopBar()
	{
		((UIElement)mSettingsButton).Visibility = (Visibility)0;
		((UIElement)mMacroPlayGrid).Visibility = (Visibility)2;
	}

	internal void ShowRecordingIcons()
	{
		if (ParentWindow.IsUIInPortraitMode)
		{
			((UIElement)mSettingsButton).Visibility = (Visibility)2;
		}
		mMacroRecordControl.Init(ParentWindow);
		((UIElement)mMacroRecordGrid).Visibility = (Visibility)0;
		mMacroRecordControl.StartTimer();
	}

	internal void HideRecordingIcons()
	{
		((UIElement)mSettingsButton).Visibility = (Visibility)0;
		((UIElement)mMacroRecordGrid).Visibility = (Visibility)2;
		((Popup)mMacroRecorderToolTipPopup).IsOpen = false;
	}

	private void NCSoftTopBar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this);
	}

	private void SettingsDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void SettingsPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mSettingsDropdownPopup).IsEnabled = false;
		mSettingsButtonImage.ImageName = "cfgmenu";
	}

	private void SettingsPopup_Closed(object sender, EventArgs e)
	{
		((UIElement)mSettingsDropdownPopup).IsEnabled = true;
		mSettingsButtonImage.ImageName = "cfgmenu";
	}

	void ITopBar.ShowSyncPanel(bool isSource)
	{
		((UIElement)mOperationsSyncGrid).Visibility = (Visibility)0;
		if (isSource)
		{
			((UIElement)mStopSyncButton).Visibility = (Visibility)0;
		}
	}

	void ITopBar.HideSyncPanel()
	{
		((UIElement)mOperationsSyncGrid).Visibility = (Visibility)2;
		((UIElement)mStopSyncButton).Visibility = (Visibility)2;
		((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
	}

	private void PlayPauseSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName.Equals("pause_title_bar", StringComparison.InvariantCultureIgnoreCase))
		{
			((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName = "play_title_bar";
			ParentWindow.mSynchronizerWindow.PauseAllSyncOperations();
		}
		else
		{
			((CustomPictureBox)((sender is CustomPictureBox) ? sender : null)).ImageName = "pause_title_bar";
			ParentWindow.mSynchronizerWindow.PlayAllSyncOperations();
		}
	}

	private void StopSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((ITopBar)this).HideSyncPanel();
		ParentWindow.mSynchronizerWindow.StopAllSyncOperations();
		if (RegistryManager.Instance.IsShowToastNotification)
		{
			ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STOPPED", ""));
		}
	}

	private void OperationsSyncGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		if (ParentWindow.mIsSynchronisationActive)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = true;
		}
	}

	private void OperationsSyncGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		if (ParentWindow.mIsSynchronisationActive && !((UIElement)mOperationsSyncGrid).IsMouseOver && !((UIElement)mSyncInstancesToolTipPopup).IsMouseOver)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
		}
	}

	private void SyncInstancesToolTip_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mOperationsSyncGrid).IsMouseOver && !((UIElement)mSyncInstancesToolTipPopup).IsMouseOver)
		{
			((Popup)mSyncInstancesToolTipPopup).IsOpen = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/ncsofttopbar.xaml", UriKind.Relative);
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Expected O, but got Unknown
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Expected O, but got Unknown
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Expected O, but got Unknown
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Expected O, but got Unknown
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Expected O, but got Unknown
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Expected O, but got Unknown
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Expected O, but got Unknown
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Expected O, but got Unknown
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Expected O, but got Unknown
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Expected O, but got Unknown
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Expected O, but got Unknown
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(NCSoftTopBar)target).Loaded += new RoutedEventHandler(NCSoftTopBar_Loaded);
			((FrameworkElement)(NCSoftTopBar)target).SizeChanged += new SizeChangedEventHandler(NCSoftTopBar_SizeChanged);
			break;
		case 2:
			mMainGrid = (Grid)target;
			break;
		case 3:
			mTitleIcon = (CustomPictureBox)target;
			break;
		case 4:
			mWindowHeaderGrid = (StackPanel)target;
			break;
		case 5:
			mAppName = (TextBlock)target;
			break;
		case 6:
			mGamenameSeparator = (Line)target;
			break;
		case 7:
			mCharacterName = (TextBlock)target;
			break;
		case 8:
			mStreamingTopbarGrid = (Grid)target;
			break;
		case 9:
			mBorder = (Border)target;
			break;
		case 10:
			mNcTopBarControlGrid = (Grid)target;
			break;
		case 11:
			mMacroRecordGrid = (Grid)target;
			break;
		case 12:
			mMacroRecordControl = (MacroTopBarRecordControl)target;
			break;
		case 13:
			mMacroPlayGrid = (Grid)target;
			break;
		case 14:
			mMacroPlayControl = (MacroTopBarPlayControl)target;
			break;
		case 15:
			mVideoRecordingStatusGrid = (Grid)target;
			break;
		case 16:
			mVideoRecordStatusControl = (VideoRecordingStatus)target;
			break;
		case 17:
			mOperationsSyncGrid = (Grid)target;
			((UIElement)mOperationsSyncGrid).MouseEnter += new MouseEventHandler(OperationsSyncGrid_MouseEnter);
			((UIElement)mOperationsSyncGrid).MouseLeave += new MouseEventHandler(OperationsSyncGrid_MouseLeave);
			break;
		case 18:
			mSyncMaskBorder = (Border)target;
			break;
		case 19:
			mStopSyncButton = (CustomPictureBox)target;
			((UIElement)mStopSyncButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopSyncButton_PreviewMouseLeftButtonUp);
			break;
		case 20:
			mControlBtnPanel = (StackPanel)target;
			break;
		case 21:
			mSettingsButton = (Grid)target;
			((UIElement)mSettingsButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SettingsButton_MouseLeftButtonUp);
			break;
		case 22:
			mSettingsButtonImage = (CustomPictureBox)target;
			break;
		case 23:
			mSettingsButtonText = (TextBlock)target;
			break;
		case 24:
			mMinimizeButton = (Grid)target;
			((UIElement)mMinimizeButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MinimizeButton_MouseLeftButtonUp);
			break;
		case 25:
			mMinimizeButtonImage = (CustomPictureBox)target;
			break;
		case 26:
			mMinimizeButtonText = (TextBlock)target;
			break;
		case 27:
			mMaximizeButton = (Grid)target;
			((UIElement)mMaximizeButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MaxmizeButton_MouseLeftButtonUp);
			break;
		case 28:
			mMaximizeButtonImage = (CustomPictureBox)target;
			break;
		case 29:
			mMaximizeButtonText = (TextBlock)target;
			break;
		case 30:
			mCloseButton = (Grid)target;
			((UIElement)mCloseButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 31:
			mCloseButtonImage = (CustomPictureBox)target;
			break;
		case 32:
			mCloseButtonText = (TextBlock)target;
			break;
		case 33:
			mSidebarButton = (Grid)target;
			((UIElement)mSidebarButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MSidebarButton_MouseLeftButtonUp);
			break;
		case 34:
			mSidebarButtonImage = (CustomPictureBox)target;
			break;
		case 35:
			mSidebarButtonText = (TextBlock)target;
			break;
		case 36:
			mMacroRecorderToolTipPopup = (CustomPopUp)target;
			break;
		case 37:
			dummyGrid = (Grid)target;
			break;
		case 38:
			mMacroRecordingTooltip = (TextBlock)target;
			break;
		case 39:
			mUpArrow = (Path)target;
			break;
		case 40:
			mMacroRunningToolTipPopup = (CustomPopUp)target;
			break;
		case 41:
			grid = (Grid)target;
			break;
		case 42:
			mMacroRunningTooltip = (TextBlock)target;
			break;
		case 43:
			mSettingsDropdownPopup = (CustomPopUp)target;
			((Popup)mSettingsDropdownPopup).Opened += SettingsPopup_Opened;
			((Popup)mSettingsDropdownPopup).Closed += SettingsPopup_Closed;
			break;
		case 44:
			mSettingsDropdownBorder = (Border)target;
			break;
		case 45:
			mGrid = (Grid)target;
			break;
		case 46:
			mMaskBorder = (Border)target;
			break;
		case 47:
			mSettingsDropDownControl = (SettingsWindowDropdown)target;
			break;
		case 48:
			mSyncInstancesToolTipPopup = (CustomPopUp)target;
			break;
		case 49:
			mDummyGrid = (Grid)target;
			break;
		case 50:
			mUpwardArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
