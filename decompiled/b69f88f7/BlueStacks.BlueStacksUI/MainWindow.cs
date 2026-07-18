using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;
using Bluester;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class MainWindow : CustomWindow, IDisposable, IComponentConnector, IStyleConnector
{
	public delegate void GuestBootCompletedEventHandler(object sender, EventArgs args);

	internal delegate void CursorLockChangedEventHandler(object sender, MainWindowEventArgs.CursorLockChangedEventArgs args);

	internal delegate void FullScreenChangedEventHandler(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args);

	internal delegate void FrontendGridVisibilityChangedEventHandler(object sender, MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args);

	internal delegate void BrowserOTSCompletedCallbackEventHandler(object sender, MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args);

	private Mutex mBlueStacksClientInstanceLock;

	private int heightDiffScaled = 42;

	private int widthDiffScaled = 2;

	internal Fraction mAspectRatio = new Fraction(16L, 9L);

	private const long OneGB = 1073741824L;

	internal int MinWidthScaled;

	internal int MinHeightScaled;

	internal int MaxHeightScaled = 10000;

	internal int MaxWidthScaled = 10000;

	internal bool mIsDmmMaximised;

	internal bool mIsDMMMaximizedFromPortrait;

	internal bool mIsDMMRecommendedWindowOpen = true;

	internal Rect DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);

	internal DMMFullScreenTopBar mDMMFST;

	internal DMMRecommendedWindow mDMMRecommendedWindow;

	private bool mIsWindowResizedOnce;

	internal bool mIsFullScreenFromMaximized;

	internal bool mIsMinimizedThroughCloseButton;

	internal bool mIsStreaming;

	private bool isSetupDone;

	private double? mPreviousWidth;

	private double? mPreviousHeight;

	internal bool IsUIInPortraitMode;

	internal bool IsUIInPortraitModeWhenMaximized;

	private Grid mLastVisibleGrid;

	internal QuitPopupBrowserControl mQuitPopupBrowserControl;

	internal bool mIsFullScreen;

	internal static double sScalingFactor;

	internal static bool sIsClosingForBackupRestore;

	internal static bool sShowNotifications;

	internal bool mIsFocusComeFromImap;

	private IMConfig mSelectedConfig;

	private IMConfig mOriginalLoadedConfig;

	internal bool mClosed;

	private bool mIsGamepadConnected;

	internal Dictionary<string, bool> AppForcedOrientationDict = new Dictionary<string, bool>();

	private bool mSkipNextGamepadStatus;

	internal string mCallbackEnabled = "False";

	private Grid mResizeGrid;

	internal bool mIsResizing;

	internal EventHandler ResizeBegin;

	internal EventHandler ResizeEnd;

	private bool mClosing;

	internal bool mGuestBootCompleted;

	internal bool mEnableLaunchPlayForNCSoft;

	internal volatile bool mIsWindowInFocus;

	internal string mBrowserCallbackFunctionName;

	internal DateTime mBootStartTime = DateTime.Now;

	internal bool IsQuitPopupNotficationReceived;

	private Grid mFirebaseBrowserControlGrid;

	internal static Dictionary<string, string> sMacroMapping;

	internal MacroRecording mAutoRunMacro;

	private ScreenLockControl mScreenLock;

	private MacroOverlay mMacroOverlay;

	internal CommonHandlers mCommonHandler;

	internal FrontendHandler mFrontendHandler;

	internal DownloadInstallApk mAppInstaller;

	internal AppHandler mAppHandler;

	internal bool mStreamingModeEnabled;

	internal PostOtsWelcomeWindowControl mPostOtsWelcomeWindow;

	private MacroRecorderWindow mMacroRecorderWindow;

	internal SynchronizerWindow mSynchronizerWindow;

	internal List<string> mSelectedInstancesForSync = new List<string>();

	internal bool mIsMacroPlaying;

	internal string mMacroPlaying = string.Empty;

	internal bool mIsScriptsPresent;

	internal System.Timers.Timer mMacroTimer;

	internal bool mIsSyncMaster;

	private BlueStacksUIUtils mUtils;

	internal WindowWndProcHandler mResizeHandler;

	private MainWindowsStaticComponents mStaticComponents;

	internal string mVmName = Strings.CurrentDefaultVmName;

	private bool mIsTokenAvailable;

	private readonly bool mIsWindowLoadedOnce;

	internal DimOverlayControl mDimOverlay;

	internal IntPtr Handle;

	internal bool mIsRestart;

	private Storyboard mStoryBoard;

	internal bool mIsMacroRecorderActive;

	internal bool mIsSynchronisationActive;

	internal bool mStartupTabLaunched;

	internal bool mLaunchStartupTabWhenTokenReceived;

	private readonly SerialWorkQueue pikaNotificationWorkQueue = new SerialWorkQueue("pikaNotificationWorkQueue");

	private bool isPikaPopOpen;

	private readonly DispatcherTimer pikaNotificationTimer = new DispatcherTimer();

	private readonly DispatcherTimer toastTimer = new DispatcherTimer();

	private readonly DispatcherTimer mFullScreenToastTimer = new DispatcherTimer();

	private bool mIsLockScreenActionPending;

	private bool disposedValue;

	private bool mIsSideButtonDragging;

	private Point mSideButtonOldPosition;

	private Thickness mOldSideButtonMargin;

	private bool mIsTopButtonDragging;

	private Point mTopButtonOldPosition;

	private Thickness mOldTopButtonMargin;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border OuterBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid MainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp pikaPop;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas pikaCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal PikaNotificationControl pikaPopControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp toastPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas toastCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToastPopupControl toastControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mFullScreenToastPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mFullScreenToastCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal FullScreenToastPopupControl mFullScreenToastControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mGeneraltoast;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mGeneraltoastCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomToastPopupControl mGeneraltoastControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mShootingModePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mShootingModePopupCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPersistentToastPopupControl mToastControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mTopBarPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal FullScreenTopBar mFullScreenTopBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mFullscreenSidebarPopupButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFullscreenSidebarPopupButtonInnerGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button mFullscreenSidebarButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mFullscreenSidebarPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFullscreenSidebarPopupInnerGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mFullscreenTopbarPopupButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFullscreenTopbarPopupButtonInnerGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button mFullscreenTopbarButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mFullscreenTopbarPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFullscreenTopbarPopupInnerGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TopbarOptions mTopbarOptions;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainWindowTopGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TopBar mTopBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal NCSoftTopBar mNCTopBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal FrontendOTSControl mFrontendOTSControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyPika;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mContentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid WelcomeTabParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal WelcomeTab mWelcomeTab;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid FrontendParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DMMProgressControl mDmmProgressControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFrontendGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DMMBottomBar mDmmBottomBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Sidebar mSidebar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyToast;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyTooltip;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mExitProgressGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mQuitPopupBrowserLoadGrid;

	private bool _contentLoaded;

	internal int ParentWindowHeightDiff { get; set; } = 42;

	internal int ParentWindowWidthDiff { get; set; } = 2;

	public UserControl TopBar
	{
		get
		{
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				return (UserControl)(object)mTopBar;
			}
			return (UserControl)(object)mNCTopBar;
		}
	}

	internal ITopBar _TopBar
	{
		get
		{
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				return mTopBar;
			}
			return mNCTopBar;
		}
	}

	internal IMConfig SelectedConfig
	{
		get
		{
			if (mSelectedConfig == null)
			{
				mSelectedConfig = new IMConfig();
			}
			return mSelectedConfig;
		}
		set
		{
			mSelectedConfig = value;
		}
	}

	internal IMConfig OriginalLoadedConfig
	{
		get
		{
			if (mOriginalLoadedConfig == null)
			{
				mOriginalLoadedConfig = new IMConfig();
			}
			return mOriginalLoadedConfig;
		}
		set
		{
			mOriginalLoadedConfig = value;
		}
	}

	internal Dictionary<string, int> AppNotificationCountDictForEachVM { get; set; } = new Dictionary<string, int>();

	internal bool SkipNextGamepadStatus
	{
		get
		{
			return mSkipNextGamepadStatus;
		}
		set
		{
			mSkipNextGamepadStatus = value;
			if (mSkipNextGamepadStatus)
			{
				WasGamepadStatusSkipped = value;
			}
		}
	}

	internal bool WasGamepadStatusSkipped { get; set; }

	internal bool IsGamepadConnected
	{
		get
		{
			return mIsGamepadConnected;
		}
		set
		{
			mIsGamepadConnected = value;
			if (RegistryManager.Instance.IsShowToastNotification && !SkipNextGamepadStatus)
			{
				ShowGamepadToast(value);
			}
			SkipNextGamepadStatus = false;
			BlueStacksUIUtils.SendGamepadStatusToBrowsers(value);
			mWelcomeTab.mHomeAppManager.UpdateGamepadIcons(value);
		}
	}

	public DummyTaskbarWindow DummyWindow { get; set; }

	internal Grid FirebaseBrowserControlGrid
	{
		get
		{
			if (mFirebaseBrowserControlGrid == null)
			{
				mFirebaseBrowserControlGrid = AddBrowser(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/notification"));
			}
			return mFirebaseBrowserControlGrid;
		}
	}

	internal ScreenLockControl ScreenLockInstance
	{
		get
		{
			if (mScreenLock == null)
			{
				mScreenLock = new ScreenLockControl();
			}
			return mScreenLock;
		}
	}

	internal MacroOverlay MacroOverlayControl
	{
		get
		{
			if (mMacroOverlay == null)
			{
				mMacroOverlay = new MacroOverlay(this);
			}
			return mMacroOverlay;
		}
	}

	internal InstanceRegistry EngineInstanceRegistry => RegistryManager.Instance.Guest[mVmName];

	internal MacroRecorderWindow MacroRecorderWindow
	{
		get
		{
			if (mMacroRecorderWindow == null)
			{
				MacroRecorderWindow macroRecorderWindow = new MacroRecorderWindow(this);
				((Window)macroRecorderWindow).Owner = (Window)(object)this;
				mMacroRecorderWindow = macroRecorderWindow;
			}
			return mMacroRecorderWindow;
		}
	}

	internal BlueStacksUIUtils Utils
	{
		get
		{
			if (mUtils == null)
			{
				mUtils = new BlueStacksUIUtils(this);
			}
			return mUtils;
		}
	}

	internal MainWindowsStaticComponents StaticComponents
	{
		get
		{
			if (mStaticComponents == null)
			{
				mStaticComponents = new MainWindowsStaticComponents();
			}
			return mStaticComponents;
		}
	}

	internal bool IsDefaultVM => string.Equals(mVmName, Strings.CurrentDefaultVmName, StringComparison.InvariantCulture);

	internal Storyboard StoryBoard
	{
		get
		{
			if (mStoryBoard == null)
			{
				object obj = ((FrameworkElement)this).FindResource((object)"mStoryBoard");
				mStoryBoard = (Storyboard)((obj is Storyboard) ? obj : null);
			}
			return mStoryBoard;
		}
	}

	public Discord mDiscordhandler { get; set; }

	internal bool SendClientActions
	{
		get
		{
			if (!mIsMacroRecorderActive)
			{
				return mIsSynchronisationActive;
			}
			return true;
		}
	}

	public static SettingsWindow SettingsWindow { get; set; }

	public bool IsInNotificationMode { get; set; }

	public string mPostBootNotificationAction { get; set; }

	private event EventHandler CloseWindowConfirmationAcceptedHandler;

	private event EventHandler CloseWindowConfirmationResetAccountAcceptedHandler;

	public event GuestBootCompletedEventHandler GuestBootCompleted;

	internal event CursorLockChangedEventHandler CursorLockChangedEvent;

	internal event FullScreenChangedEventHandler FullScreenChanged;

	internal event FrontendGridVisibilityChangedEventHandler FrontendGridVisibilityChanged;

	private event EventHandler mEventOnAllWindowClosed;

	private event EventHandler mEventOnInstanceClosed;

	internal event EventHandler RestartEngineConfirmationAcceptedHandler;

	internal event EventHandler RestartPcConfirmationAcceptedHandler;

	internal event BrowserOTSCompletedCallbackEventHandler BrowserOTSCompletedCallback;

	public static void OpenSettingsWindow(MainWindow window, string startTab)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (window != null)
		{
			if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed && !KMManager.sGuidanceWindow.IsViewState)
			{
				CustomMessageWindow val = new CustomMessageWindow
				{
					Owner = (Window)(object)window.mDimOverlay
				};
				val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
				val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANNOT_OPEN_SETTING", "");
				val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler)delegate
				{
				}, (string)null, false, (object)null);
				window.ShowDimOverlay();
				((Window)val).ShowDialog();
				window.HideDimOverlay();
			}
			else if (SettingsWindow == null)
			{
				SettingsWindow = new SettingsWindow(window, startTab);
				int num = 500;
				int num2 = 750;
				new ContainerWindow(window, (UserControl)(object)SettingsWindow, num2, num);
			}
			else
			{
				SettingsWindow.ChangeSettingsTab(window, startTab);
			}
		}
	}

	public static void CloseSettingsWindow(SettingsWindow window)
	{
		SettingsWindow = window;
		if (SettingsWindow != null)
		{
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)SettingsWindow);
		}
	}

	private void ShowGamepadToast(bool state)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Expected O, but got Unknown
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Expected O, but got Unknown
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (mIsWindowInFocus)
				{
					if (((Popup)toastPopup).IsOpen)
					{
						toastTimer.Stop();
						((Popup)toastPopup).IsOpen = false;
					}
					double num = ((10.0 + ((FrameworkElement)mSidebar).ActualWidth > 0.0) ? ((FrameworkElement)mSidebar).ActualWidth : (0.0 + mWelcomeTab.mHomeAppManager.GetAppRecommendationsGridWidth()));
					if (state)
					{
						toastControl.Init((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", ""), (Brush)null, (Brush)new SolidColorBrush(Color.FromArgb((byte)85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), (HorizontalAlignment)2, (VerticalAlignment)2, (Thickness?)new Thickness(0.0, 0.0, num, 20.0), 5, (Thickness?)null, (Brush)null, false);
						toastControl.AddImage("gamepad_connected", 16.0, 24.0, (Thickness?)new Thickness(0.0, 5.0, 10.0, 5.0));
					}
					else
					{
						toastControl.Init((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", ""), (Brush)null, (Brush)new SolidColorBrush(Color.FromArgb((byte)85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), (HorizontalAlignment)2, (VerticalAlignment)2, (Thickness?)new Thickness(0.0, 0.0, num, 20.0), 5, (Thickness?)null, (Brush)null, false);
						toastControl.AddImage("gamepad_disconnected", 19.0, 24.0, (Thickness?)new Thickness(0.0, 5.0, 10.0, 5.0));
					}
					((FrameworkElement)dummyToast).HorizontalAlignment = (HorizontalAlignment)2;
					((FrameworkElement)dummyToast).VerticalAlignment = (VerticalAlignment)2;
					((UIElement)toastControl).Visibility = (Visibility)0;
					((Popup)toastPopup).IsOpen = true;
					((FrameworkElement)toastCanvas).Width = ((FrameworkElement)toastControl).ActualWidth;
					((FrameworkElement)toastCanvas).Height = ((FrameworkElement)toastControl).ActualHeight;
					((Popup)toastPopup).VerticalOffset = -1.0 * ((FrameworkElement)toastControl).ActualHeight - 50.0;
					((Popup)toastPopup).HorizontalOffset = -20.0;
					toastTimer.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup for gamepad : " + ex.ToString());
			}
		}, new object[0]);
	}

	internal void ShowGeneralToast(string message)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			try
			{
				if (mIsWindowInFocus)
				{
					if (((Popup)mGeneraltoast).IsOpen)
					{
						toastTimer.Stop();
						((Popup)mGeneraltoast).IsOpen = false;
					}
					mGeneraltoastControl.Init((Window)(object)this, message, (Brush)(object)Brushes.Black, (Brush)new SolidColorBrush(Color.FromArgb((byte)85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), (HorizontalAlignment)1, (VerticalAlignment)2, (Thickness?)null, 5, (Thickness?)null, (Brush)null, false);
					((FrameworkElement)dummyToast).HorizontalAlignment = (HorizontalAlignment)1;
					((FrameworkElement)dummyToast).VerticalAlignment = (VerticalAlignment)2;
					((UIElement)mGeneraltoastControl).Visibility = (Visibility)0;
					((Popup)mGeneraltoast).IsOpen = true;
					((FrameworkElement)mGeneraltoastCanvas).Height = ((FrameworkElement)mGeneraltoastControl).ActualHeight;
					((Popup)mGeneraltoast).VerticalOffset = -1.0 * ((FrameworkElement)mGeneraltoastControl).ActualHeight - 50.0;
					((FrameworkElement)mGeneraltoast).HorizontalAlignment = (HorizontalAlignment)1;
					toastTimer.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing general toast popup : " + ex.ToString());
			}
		}, new object[0]);
	}

	private void ToastTimer_Tick(object sender, EventArgs e)
	{
		toastTimer.Stop();
		((Popup)toastPopup).IsOpen = false;
		((Popup)mGeneraltoast).IsOpen = false;
	}

	internal void CloseFullScreenToastAndStopTimer()
	{
		mFullScreenToastTimer.Stop();
		((Popup)mFullScreenToastPopup).IsOpen = false;
	}

	private void FullScreenToastTimer_Tick(object sender, EventArgs e)
	{
		CloseFullScreenToastAndStopTimer();
	}

	private void GetMacroShortcutKeyMappingsWithRestrictedKeysandNames()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		foreach (MacroRecording vertex in MacroGraph.Instance.Vertices)
		{
			MacroRecording val = vertex;
			if (val.Shortcut.Length == 1 && !sMacroMapping.ContainsKey(val.Shortcut))
			{
				sMacroMapping.Add(val.Shortcut, val.Name);
			}
			if (val.PlayOnStart)
			{
				if (mAutoRunMacro == null)
				{
					mAutoRunMacro = val;
					continue;
				}
				val.PlayOnStart = false;
				CommonHandlers.SaveMacroJson(val, val.Name + ".json");
			}
		}
		HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", sMacroMapping, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
	}

	public MainWindow(string vmName, FrontendHandler frontendHandler)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Expected O, but got Unknown
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Expected O, but got Unknown
		Logger.Info("main window init");
		mVmName = vmName;
		GetLockOfCurrentInstance();
		SetMultiInstanceEventWaitHandle();
		if (frontendHandler != null)
		{
			mFrontendHandler = frontendHandler;
			frontendHandler.ParentWindow = this;
		}
		mCommonHandler = new CommonHandlers(this);
		InitializeComponent();
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			mWelcomeTab.Init();
			((UIElement)mFrontendGrid).Visibility = (Visibility)0;
		}
		else
		{
			ParentWindowHeightDiff = (heightDiffScaled = 94);
			((UIElement)WelcomeTabParentGrid).Visibility = (Visibility)1;
			mWelcomeTab.Init();
			((UIElement)mWelcomeTab).Visibility = (Visibility)1;
			((UIElement)mWelcomeTab.mPromotionGrid).Visibility = (Visibility)1;
			((UIElement)mWelcomeTab.mPromotionControl).IsEnabled = false;
			((UIElement)FrontendParentGrid).Visibility = (Visibility)0;
			((UIElement)mDmmProgressControl).Visibility = (Visibility)0;
			((UIElement)mFrontendGrid).Visibility = (Visibility)1;
			((FrameworkElement)mFrontendGrid).Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
			((UIElement)mDmmBottomBar).Visibility = (Visibility)0;
			mDMMFST = new DMMFullScreenTopBar();
			mDmmBottomBar.Init(this);
			((Popup)mTopBarPopup).Child = (UIElement)(object)mDMMFST;
			mDMMFST.Init(this);
			((UIElement)mDMMFST).MouseLeave += new MouseEventHandler(TopBarPopup_MouseLeave);
		}
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
		((Window)this).LocationChanged += MainWindow_LocationChanged;
		SetupInitialSize();
		SetWindowTitle(vmName);
		mResizeHandler = new WindowWndProcHandler(this);
		mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
		mAppHandler = new AppHandler(this);
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)mTopBar).Visibility = (Visibility)2;
			((UIElement)mNCTopBar).Visibility = (Visibility)0;
		}
		if (EngineInstanceRegistry.IsClientOnTop)
		{
			((Window)this).Topmost = true;
		}
		mCommonHandler.InitShortcuts();
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			mSidebar.InitElements();
		}
		if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
		{
			mIsTokenAvailable = true;
		}
		if (IsDefaultVM && mAppHandler.IsOneTimeSetupCompleted)
		{
			PromotionObject.PromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.PromotionHandler, new EventHandler(MainWindow_PromotionHandler));
		}
		AppRequirementsParser.Instance.RequirementConfigUpdated += MainWindow_RequirementConfigUpdated;
		((UIElement)this).PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);
		((UIElement)this).PreviewKeyUp += new KeyEventHandler(MainWindow_PreviewKeyUp);
		RegistryManager.Instance.BossKey = mCommonHandler.GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", isBossKey: true);
		try
		{
			if (!AppConfigurationManager.Instance.VmAppConfig.ContainsKey(mVmName))
			{
				AppConfigurationManager.Instance.VmAppConfig[mVmName] = new Dictionary<string, AppSettings>();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error {0}", new object[1] { ex });
		}
		mIsWindowLoadedOnce = true;
	}

	private void GetLockOfCurrentInstance()
	{
		Logger.Debug("Getting lock of instance.." + mVmName);
		ProcessUtils.CheckAlreadyRunningAndTakeLock(Strings.GetClientInstanceLockName(mVmName, "bgp64"), ref mBlueStacksClientInstanceLock);
		if (mBlueStacksClientInstanceLock == null)
		{
			Logger.Error("Client lock is not created for vmName: {0}", new object[1] { mVmName });
		}
	}

	private void SetMultiInstanceEventWaitHandle()
	{
		try
		{
			using EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(Utils.GetMultiInstanceEventName(mVmName));
			eventWaitHandle.Set();
		}
		catch (Exception ex)
		{
			Logger.Error("Error while setting event wait handle for vmName: {0} ex: {1}", new object[2] { mVmName, ex });
		}
	}

	private void MainWindow_RequirementConfigUpdated(object sender, EventArgs args)
	{
		GrmHandler.RequirementConfigUpdated(mVmName);
	}

	private void MainWindow_PromotionHandler(object sender, EventArgs e)
	{
		if (IsDefaultVM && mAppHandler.IsOneTimeSetupCompleted && !mGuestBootCompleted)
		{
			HandleFLEorAppPopupBeforeBoot();
		}
	}

	private void SetTaskbarProperties()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		((Window)this).Icon = (ImageSource)new BitmapImage(new Uri(Path.Combine(RegistryStrings.InstallDir, "app_icon.ico")));
		((Window)this).Title = GameConfig.Instance.AppName;
	}

	internal void RestartFrontend()
	{
		mFrontendHandler.mEventOnFrontendClosed -= FrontendHandler_StartFrontend;
		mFrontendHandler.mEventOnFrontendClosed += FrontendHandler_StartFrontend;
		CloseFrontend();
	}

	private void FrontendHandler_StartFrontend(object sender, EventArgs e)
	{
		mFrontendHandler.StartFrontend();
		if (!FeatureManager.Instance.IsCustomUIForDMM && !Utils.IsRequiredFreeRAMAvailable())
		{
			mFrontendHandler.mIsSufficientRAMAvailable = false;
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mFrontendHandler.FrontendHandler_ShowLowRAMMessage();
			}, new object[0]);
		}
	}

	internal void CloseFrontend()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ShowLoadingGrid(isShow: true);
			mTopBar.mAppTabButtons.GoToTab("Home");
			if (mWelcomeTab != null)
			{
				mWelcomeTab.mFrontendPopupControl.HideWindow();
			}
			if (mAppHandler != null)
			{
				mAppHandler.IsGuestReady = false;
				mAppHandler.mGuestReadyCheckStarted = false;
			}
			mFrontendHandler.mFrontendHandle = IntPtr.Zero;
		}, new object[0]);
		mFrontendHandler.KillFrontend(isWaitForPlayerClosing: true);
	}

	internal void SwitchToPortraitMode(bool isSwitchForPortraitMode)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if ((int)((Window)this).WindowState == 0)
				{
					mPreviousWidth = ((FrameworkElement)this).Width;
					mPreviousHeight = ((FrameworkElement)this).Height;
				}
				bool flag = false;
				if (isSwitchForPortraitMode && (int)((Window)this).WindowState != 2)
				{
					if (isSwitchForPortraitMode != IsUIInPortraitMode)
					{
						flag = true;
						IsUIInPortraitMode = true;
						mTopBar.RefreshNotificationCentreButton();
						mTopBar.UpdateMacroRecordingProgress();
					}
				}
				else
				{
					if (isSwitchForPortraitMode && FeatureManager.Instance.IsCustomUIForDMM)
					{
						IsUIInPortraitMode = true;
						((Window)this).WindowState = (WindowState)0;
						SetSizeForDMMPortraitMaximisedWindow();
						mTopBar.RefreshNotificationCentreButton();
						mTopBar.RefreshWarningButton();
						return;
					}
					if (isSwitchForPortraitMode != IsUIInPortraitMode)
					{
						flag = true;
						IsUIInPortraitMode = false;
						if (mIsDmmMaximised)
						{
							((Window)this).WindowState = (WindowState)2;
						}
						mTopBar.UpdateMacroRecordingProgress();
						mTopBar.RefreshNotificationCentreButton();
					}
				}
				if ((int)((Window)this).WindowState == 0)
				{
					if (FeatureManager.Instance.IsCustomUIForDMM && mIsDmmMaximised && ((Rect)(ref DmmRestoreWindowRectangle)).Height != 0.0)
					{
						SetDMMSizeOnRestoreWindow();
					}
					else
					{
						ChangeHeightWidthAndPosition(GetWidthFromHeight((int)((FrameworkElement)this).Height), (int)((FrameworkElement)this).Height, flag || (IsUIInPortraitMode ^ IsUIInPortraitModeWhenMaximized));
					}
				}
				mTopBar.RefreshWarningButton();
				UIChangesOnMainWindowSizeChanged();
				if (mStreamingModeEnabled)
				{
					mFrontendHandler.ChangeFrontendToPortraitMode();
				}
				if (StreamManager.Instance != null)
				{
					StreamManager.Instance.OrientationChangeHandler();
				}
				if (KMManager.sGuidanceWindow != null && !mIsFullScreen)
				{
					KMManager.sGuidanceWindow.ResizeGuidanceWindow();
				}
			}
			catch (Exception ex)
			{
				SetupInitialSize();
				Logger.Info("Error occured setting size." + ex.ToString());
			}
		}, new object[0]);
	}

	private void SetDMMSizeOnRestoreWindow()
	{
		ChangeHeightWidthAndPosition(GetWidthFromHeight((int)((Rect)(ref DmmRestoreWindowRectangle)).Height), (int)((Rect)(ref DmmRestoreWindowRectangle)).Height, changePosition: false);
		if (mIsDMMMaximizedFromPortrait != IsUIInPortraitMode)
		{
			if (IsUIInPortraitMode)
			{
				((Window)this).Left = ((Rect)(ref DmmRestoreWindowRectangle)).Left + (((Rect)(ref DmmRestoreWindowRectangle)).Width - ((FrameworkElement)this).Width) / 2.0;
			}
			else
			{
				((Window)this).Left = ((Rect)(ref DmmRestoreWindowRectangle)).Left - (((FrameworkElement)this).Width - ((Rect)(ref DmmRestoreWindowRectangle)).Width) / 2.0;
			}
		}
		else
		{
			((Window)this).Left = ((Rect)(ref DmmRestoreWindowRectangle)).Left;
		}
		((Window)this).Top = ((Rect)(ref DmmRestoreWindowRectangle)).Top;
	}

	private void UIChangesOnMainWindowSizeChanged()
	{
		CustomPopUp customPopUp = pikaPop;
		((Popup)customPopUp).HorizontalOffset = ((Popup)customPopUp).HorizontalOffset + 1.0;
		CustomPopUp customPopUp2 = pikaPop;
		((Popup)customPopUp2).HorizontalOffset = ((Popup)customPopUp2).HorizontalOffset - 1.0;
		CustomPopUp customPopUp3 = toastPopup;
		((Popup)customPopUp3).HorizontalOffset = ((Popup)customPopUp3).HorizontalOffset + 1.0;
		CustomPopUp customPopUp4 = toastPopup;
		((Popup)customPopUp4).HorizontalOffset = ((Popup)customPopUp4).HorizontalOffset - 1.0;
		mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
		SetMaxSizeOfWindow();
	}

	private void SetMaxSizeOfWindow()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Size maxWidthAndHeightOfMonitor = WindowPlacement.GetMaxWidthAndHeightOfMonitor(new WindowInteropHelper((Window)(object)this).Handle);
		MaxHeightScaled = (int)((Size)(ref maxWidthAndHeightOfMonitor)).Height;
		MaxWidthScaled = (int)GetWidthFromHeight(MaxHeightScaled);
		if ((double)MaxWidthScaled > ((Size)(ref maxWidthAndHeightOfMonitor)).Width)
		{
			MaxWidthScaled = (int)((Size)(ref maxWidthAndHeightOfMonitor)).Width;
			MaxHeightScaled = (int)GetHeightFromWidth(MaxWidthScaled);
		}
	}

	private void ChangeHeightWidthAndPosition(double width, double height, bool changePosition)
	{
		try
		{
			((FrameworkElement)this).Height = height;
			((FrameworkElement)this).Width = width;
			if (FeatureManager.Instance.IsCustomUIForDMM && !mIsWindowResizedOnce)
			{
				double num = (((FrameworkElement)this).Height - (double)ParentWindowHeightDiff) * 9.0 / 16.0 + (double)ParentWindowWidthDiff;
				((Window)this).Left = (SystemParameters.MaximizedPrimaryScreenWidth - ((FrameworkElement)this).Width - num) / 2.0;
				((Window)this).Top = (SystemParameters.MaximizedPrimaryScreenHeight - ((FrameworkElement)this).Height) / 2.0;
				mIsWindowResizedOnce = true;
			}
			else if (changePosition)
			{
				if (IsUIInPortraitMode)
				{
					((Window)this).Left = ((Window)this).Left + (mPreviousWidth.Value - ((FrameworkElement)this).Width) / 2.0;
				}
				else
				{
					((Window)this).Left = ((Window)this).Left - (((FrameworkElement)this).Width - mPreviousWidth.Value) / 2.0;
				}
			}
			mPreviousWidth = ((FrameworkElement)this).Width;
			mPreviousHeight = ((FrameworkElement)this).Height;
		}
		catch (Exception ex)
		{
			Logger.Info("Error occured setting size." + ex.ToString());
		}
	}

	internal void ChangeHeightWidthTopLeft(double width, double height, double top, double left)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		try
		{
			if ((int)((Window)this).WindowState == 2)
			{
				RestoreWindows();
			}
			((FrameworkElement)this).Height = height / sScalingFactor;
			((FrameworkElement)this).Width = width / sScalingFactor;
			((Window)this).Top = top / sScalingFactor;
			((Window)this).Left = left / sScalingFactor;
			mSidebar?.ArrangeAllSidebarElements();
		}
		catch (Exception ex)
		{
			Logger.Error("Error occured setting size of the window. err:" + ex.ToString());
		}
	}

	private void SetWindowTitle(string vmName)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		((Window)this).Title = Utils.GetDisplayName(vmName, "bgp64");
		mTopBar.mTitleText.Text = ((Window)this).Title;
		_ = FeatureManager.Instance.IsCustomUIForNCSoft;
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			mTopBar.mTitleText.Text = GameConfig.Instance.AppName;
			mTopBar.mTitleIcon.ImageName = Path.Combine(RegistryStrings.InstallDir, "app_icon.ico");
			SetTaskbarProperties();
		}
	}

	internal void ShowRerollOverlay()
	{
		ShowDimOverlay(MacroOverlayControl);
	}

	internal void HandleGenericNotificationPopup(GenericNotificationItem notifItem)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		GenericNotificationDesignItem designItem = notifItem.NotificationDesignItem;
		if (!RegistryManager.Instance.IsShowRibbonNotification || (int)RegistryManager.Instance.InstallationType == 1)
		{
			return;
		}
		pikaNotificationWorkQueue.Enqueue((Work)delegate
		{
			while (!mIsWindowInFocus || isPikaPopOpen)
			{
				Thread.Sleep(2000);
			}
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				isPikaPopOpen = true;
				pikaPopControl.Init(notifItem);
				Canvas.SetLeft((UIElement)(object)pikaPopControl, 0.0);
				((Popup)pikaPop).IsOpen = true;
				new Storyboard();
				((FrameworkElement)pikaCanvas).Width = ((FrameworkElement)pikaPopControl).ActualWidth;
				((Popup)pikaPop).HorizontalOffset = ((FrameworkElement)pikaPopControl).ActualWidth * -0.5;
				PennerDoubleAnimation.Equations type = PennerDoubleAnimation.Equations.QuadEaseInOut;
				double actualWidth = ((FrameworkElement)pikaPopControl).ActualWidth;
				double to = 0.0;
				int durationMS = 700;
				Animator.AnimatePenner((DependencyObject)(object)pikaPopControl, Canvas.LeftProperty, type, actualWidth, to, durationMS, null);
				string arg = "Home";
				if (mTopBar.mAppTabButtons.SelectedTab != null)
				{
					arg = mTopBar.mAppTabButtons.SelectedTab.AppLabel;
				}
				ClientStats.SendMiscellaneousStatsAsync("RibbonShown", RegistryManager.Instance.UserGuid, JsonConvert.SerializeObject((object)notifItem.ExtraPayload), arg, RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, notifItem.Id, notifItem.Title);
			}, new object[0]);
			pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(designItem.AutoHideTime);
			pikaNotificationTimer.Start();
		});
	}

	private void PikaNotificationTimer_Tick(object sender, EventArgs e)
	{
		pikaNotificationTimer.Stop();
		if (!isPikaPopOpen)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			PennerDoubleAnimation.Equations type = PennerDoubleAnimation.Equations.QuadEaseInOut;
			double value = 0.0;
			double actualWidth = ((FrameworkElement)pikaPopControl).ActualWidth;
			int durationMS = 400;
			Animator.AnimatePenner((DependencyObject)(object)pikaPopControl, Canvas.LeftProperty, type, value, actualWidth, durationMS, delegate
			{
				((Popup)pikaPop).IsOpen = false;
				isPikaPopOpen = false;
			});
		}, new object[0]);
	}

	internal void ShowDimOverlay(IDimOverlayControl el = null)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ShowDimOverlayUIThread(el);
		}, new object[0]);
	}

	private void ShowDimOverlayUIThread(IDimOverlayControl el = null)
	{
		try
		{
			Logger.Debug("showing dim overlay");
			if (mDimOverlay == null || ((CustomWindow)mDimOverlay).IsClosed)
			{
				mDimOverlay = new DimOverlayControl(this);
			}
			if (PresentationSource.FromVisual((Visual)(object)this) != null)
			{
				mDimOverlay.Owner = this;
				mDimOverlay.Control = el;
				mDimOverlay.UpadteSizeLocation();
				mDimOverlay.ShowWindow();
				mFrontendHandler.ShowGLWindow();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while showing dimoverlay control. " + ex.ToString());
		}
	}

	internal void HideDimOverlay()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				Logger.Debug("Hide dim overlay");
				if (mDimOverlay != null)
				{
					if (mIsLockScreenActionPending)
					{
						ShowDimOverlay(ScreenLockInstance);
					}
					else
					{
						mDimOverlay.HideWindow(isFromOverlayClick: false);
						mDimOverlay.Control = null;
						mFrontendHandler.ShowGLWindow();
					}
				}
			}
			catch (Exception)
			{
			}
			((UIElement)this).Focus();
		}, new object[0]);
	}

	private void MainWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		foreach (Window ownedWindow in ((Window)this).OwnedWindows)
		{
			Window val = ownedWindow;
			if (val == null)
			{
				continue;
			}
			try
			{
				CustomWindow val2 = (CustomWindow)val;
				if (val2 == null || val2.ShowWithParentWindow)
				{
					((UIElement)val).Visibility = ((UIElement)this).Visibility;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing child windows: {0}", new object[1] { ex.ToString() });
			}
		}
	}

	public void MainWindow_StateChanged(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Window)this).WindowState != 1)
		{
			SendTempGamepadState(enable: true);
			try
			{
				Uri uri = new Uri(RegistryStrings.ProductIconCompletePath);
				((Window)this).Icon = (ImageSource)(object)BitmapFrame.Create(uri);
				SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
				((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count.ToString(CultureInfo.InvariantCulture);
				if (IsInNotificationMode)
				{
					foreach (string key in AppNotificationCountDictForEachVM.Keys)
					{
						Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", mVmName, key, AppNotificationCountDictForEachVM[key].ToString(CultureInfo.InvariantCulture), "NM_On");
					}
					AppNotificationCountDictForEachVM.Clear();
					DummyTaskbarWindow dummyWindow = DummyWindow;
					if (dummyWindow != null)
					{
						((Window)dummyWindow).Close();
					}
					HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string> { { "override", "False" } }, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					mIsMinimizedThroughCloseButton = false;
					if (((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count > 0 && sShowNotifications)
					{
						Thread thread = new Thread((ThreadStart)delegate
						{
							((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
							{
								BlueStacksUIBinding.BindColor((DependencyObject)(object)mTopBar.mNotificationCaret, Shape.FillProperty, "SliderButtonColor");
								BlueStacksUIBinding.BindColor((DependencyObject)(object)mTopBar.mNotificationCaret, Shape.StrokeProperty, "SliderButtonColor");
								BlueStacksUIBinding.BindColor((DependencyObject)(object)mTopBar.mNotificationCentreDropDownBorder, Control.BorderBrushProperty, "SliderButtonColor");
								((UIElement)mTopBar.mNotificationDrawerControl.mAnimationRect).Visibility = (Visibility)0;
								mTopBar.mNotificationCentreButton_MouseLeftButtonUp(null, null);
							}, new object[0]);
						});
						thread.IsBackground = true;
						thread.Start();
					}
				}
				sShowNotifications = true;
				IsInNotificationMode = false;
			}
			catch (Exception ex)
			{
				Logger.Error("Error in setting window's icon: " + ex);
			}
		}
		else
		{
			Logger.Debug("KMP MainWindow_StateChanged " + mVmName);
			if (BlueStacksUIUtils.ActivatedWindow == this)
			{
				BlueStacksUIUtils.ActivatedWindow = null;
			}
			AppUsageTimer.StopTimer();
			mFrontendHandler.DeactivateFrontend();
			mCommonHandler.ClipMouseCursorHandler(forceDisable: true);
			mIsWindowInFocus = false;
			if (!IsInNotificationMode)
			{
				BlueStacksUIUtils.SetWindowTaskbarIcon(this);
			}
		}
		BlueStacksUIUtils.LastActivatedWindow.mFrontendHandler.UpdateOverlaySizeStatus();
		OnResizeMainWindow();
	}

	internal void SendTempGamepadState(bool enable)
	{
		if (!RegistryManager.Instance.GamepadDetectionEnabled)
		{
			return;
		}
		if (enable)
		{
			if (!IsGamepadConnected)
			{
				if (WasGamepadStatusSkipped)
				{
					SkipNextGamepadStatus = true;
					WasGamepadStatusSkipped = false;
				}
				mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string> { { "enable", "true" } });
			}
		}
		else
		{
			SkipNextGamepadStatus = true;
			mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string> { { "enable", "false" } });
		}
	}

	private void MainWindow_Deactivated(object sender, EventArgs e)
	{
		Logger.Debug("KMP MainWindow_Deactivated " + mVmName);
		if (BlueStacksUIUtils.ActivatedWindow == this)
		{
			BlueStacksUIUtils.ActivatedWindow = null;
		}
		ClosePopUps();
		mFrontendHandler.DeactivateFrontend();
		mCommonHandler.ClipMouseCursorHandler(forceDisable: true);
		mIsWindowInFocus = false;
	}

	private void MainWindow_Activated(object sender, EventArgs e)
	{
		Logger.Debug("In MainWindow_Activated");
		BlueStacksUIUtils.LastActivatedWindow = this;
		BlueStacksUIUtils.ActivatedWindow = this;
		App.IsApplicationActive = true;
		mIsWindowInFocus = true;
		if (!string.IsNullOrEmpty(mVmName) && mTopBar != null && mTopBar.mAppTabButtons != null && mTopBar.mAppTabButtons.SelectedTab != null && !string.IsNullOrEmpty(mTopBar.mAppTabButtons.SelectedTab.TabKey))
		{
			AppUsageTimer.StartTimer(mVmName, mTopBar.mAppTabButtons.SelectedTab.TabKey);
		}
		if (((UIElement)mFrontendGrid).IsVisible)
		{
			Logger.Debug("KMP MainWindow_Activated focusfrontend " + mVmName);
			if (!mTopBar.mAppTabButtons.SelectedTab.mGuidanceWindowOpen || mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen)
			{
				KMManager.ShowShootingModeTooltip(this, mTopBar.mAppTabButtons.SelectedTab.PackageName);
			}
			else
			{
				mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen = true;
			}
		}
		else
		{
			Logger.Debug("KMP MainWindow_Activated DeactivateFrontend " + mVmName);
			mFrontendHandler.DeactivateFrontend();
		}
		SendTempGamepadState(enable: true);
	}

	private void MainWindow_SourceInitialized(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Handle = ((HwndSource)PresentationSource.FromVisual((Visual)(object)this)).Handle;
	}

	internal void MainWindow_ResizeBegin(object sender, EventArgs e)
	{
		mIsResizing = true;
	}

	private void OnResizeMainWindow()
	{
		mSidebar?.SetHeight();
		PromotionObject.AppRecommendationHandler?.Invoke(obj: false);
		CustomPopUp customPopUp = mShootingModePopup;
		((Popup)customPopUp).HorizontalOffset = ((Popup)customPopUp).HorizontalOffset + 1.0;
		CustomPopUp customPopUp2 = mShootingModePopup;
		((Popup)customPopUp2).HorizontalOffset = ((Popup)customPopUp2).HorizontalOffset - 1.0;
		CustomPopUp customPopUp3 = mFullScreenToastPopup;
		((Popup)customPopUp3).HorizontalOffset = ((Popup)customPopUp3).HorizontalOffset + 1.0;
		CustomPopUp customPopUp4 = mFullScreenToastPopup;
		((Popup)customPopUp4).HorizontalOffset = ((Popup)customPopUp4).HorizontalOffset - 1.0;
	}

	internal void MainWindow_ResizeEnd(object sender, EventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		mIsResizing = false;
		if ((int)((Window)this).WindowState == 0)
		{
			try
			{
				EngineInstanceRegistry.WindowPlacement = UsefulExtensionMethod.GetPlacement((Window)(object)this);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in MainWindow_ResizeEnd. Exception: " + ex.ToString());
			}
		}
		UIChangesOnMainWindowSizeChanged();
		mFrontendHandler.ShowGLWindow();
	}

	internal void ChangeWindowOrientaion(object sender, ChangeOrientationEventArgs e)
	{
		SwitchToPortraitMode(e.IsPotrait);
	}

	private void SetupInitialSize()
	{
		mAspectRatio = new Fraction(EngineInstanceRegistry.GuestWidth, EngineInstanceRegistry.GuestHeight);
		mPreviousWidth = ((FrameworkElement)this).Width;
		mPreviousHeight = ((FrameworkElement)this).Height;
		ChangeHeightWidthAndPosition(GetWidthFromHeight(BlueStacksUIUtils.GetDefaultHeight()), BlueStacksUIUtils.GetDefaultHeight(), changePosition: true);
	}

	internal void ChangeOrientationFromClient(bool isPortrait, bool stopFurtherOrientation = true)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			if (Utils.IsGuestBooted(mVmName, "bgp64"))
			{
				SwitchOrientationFromClient(isPortrait, stopFurtherOrientation);
				SendOrientationChangeToAndroid(isPortrait);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void SendOrientationChangeToAndroid(bool isPortrait)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"d",
				isPortrait ? "1" : "0"
			} };
			HTTPUtils.SendRequestToGuest("guestorientation", dictionary, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending GuestOrientation to android: " + ex.ToString());
		}
	}

	private void SwitchOrientationFromClient(bool orientation, bool stopFurtherOrientation)
	{
		try
		{
			string text = (orientation ? "1" : "0");
			string text2 = (stopFurtherOrientation ? "1" : "0");
			string text3 = (StaticComponents.mPreviousSelectedTabWeb ? "1" : "0");
			string text4 = "orientation=" + text + "&package=" + mTopBar.mAppTabButtons.SelectedTab.PackageName + "&stopFurtherOrientationChange=" + text2 + "&isPreviousSelectedTabWeb=" + text3;
			string text5 = string.Format(CultureInfo.InvariantCulture, "{0}?{1}", new object[2] { "switchOrientation", text4 });
			BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
			{
				string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
				{
					"http://127.0.0.1",
					RegistryManager.Instance.Guest[mVmName].FrontendServerPort
				}),
				text5
			}), (Dictionary<string, string>)null, false, mVmName, 0, 1, 0, false, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending switch orientation from client: " + ex.ToString());
		}
	}

	internal void HandleDisplaySettingsChanged()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (PresentationSource.FromVisual((Visual)(object)this) != null)
			{
				Matrix transformToDevice = PresentationSource.FromVisual((Visual)(object)this).CompositionTarget.TransformToDevice;
				sScalingFactor = ((Matrix)(ref transformToDevice)).M11;
			}
			MinWidthScaled = (int)(((FrameworkElement)this).MinWidth * sScalingFactor);
			MinHeightScaled = (int)(((FrameworkElement)this).MinHeight * sScalingFactor);
			heightDiffScaled = (int)((double)ParentWindowHeightDiff * sScalingFactor);
			widthDiffScaled = (int)((double)ParentWindowWidthDiff * sScalingFactor);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
		}
	}

	internal void ShowWindow(bool updateBootStartTime = false)
	{
		if (!mIsWindowLoadedOnce)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				KMManager.ShowOverlayWindow(this, isShow: false);
			}
			((Window)this).ShowInTaskbar = true;
			((UIElement)this).Visibility = (Visibility)0;
			((Window)this).Show();
			((FrameworkElement)this).BringIntoView();
			if ((int)((Window)this).WindowState == 1)
			{
				InteropWindow.ShowWindow(Handle, 9);
			}
			if (FeatureManager.Instance.IsCustomUIForDMM && mDMMRecommendedWindow == null)
			{
				mDMMRecommendedWindow = new DMMRecommendedWindow(this);
				mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
				((UIElement)mDMMRecommendedWindow).Visibility = (Visibility)0;
			}
			if (!((Window)this).Topmost)
			{
				((Window)this).Topmost = true;
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						((Window)this).Topmost = false;
					}, new object[0]);
				});
			}
			if (updateBootStartTime)
			{
				mBootStartTime = DateTime.Now;
			}
		}, new object[0]);
	}

	internal double GetWidthFromHeight(double height, bool isScaled = false, bool isIgnoreMinWidth = false)
	{
		if (IsUIInPortraitMode)
		{
			if (isScaled)
			{
				try
				{
					return Math.Max((height - (double)heightDiffScaled) / mAspectRatio.DoubleValue + (double)widthDiffScaled, (!isIgnoreMinWidth) ? MinWidthScaled : 0);
				}
				catch
				{
				}
			}
			return Math.Max((height - (double)ParentWindowHeightDiff) / mAspectRatio.DoubleValue + (double)ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : ((FrameworkElement)this).MinWidth);
		}
		if (isScaled)
		{
			try
			{
				return Math.Max((height - (double)heightDiffScaled) * mAspectRatio.DoubleValue + (double)widthDiffScaled, (!isIgnoreMinWidth) ? MinWidthScaled : 0);
			}
			catch
			{
			}
		}
		return Math.Max((height - (double)ParentWindowHeightDiff) * mAspectRatio.DoubleValue + (double)ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : ((FrameworkElement)this).MinWidth);
	}

	internal double GetHeightFromWidth(double width, bool isScaled = false, bool isIgnoreMinWidth = false)
	{
		if (IsUIInPortraitMode)
		{
			if (isScaled)
			{
				try
				{
					return Math.Max((width - (double)widthDiffScaled) * mAspectRatio.DoubleValue + (double)heightDiffScaled, (!isIgnoreMinWidth) ? MinHeightScaled : 0);
				}
				catch
				{
				}
			}
			return Math.Max((width - (double)ParentWindowWidthDiff) * mAspectRatio.DoubleValue + (double)ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : ((FrameworkElement)this).MinHeight);
		}
		if (isScaled)
		{
			try
			{
				return Math.Max((width - (double)widthDiffScaled) / mAspectRatio.DoubleValue + (double)heightDiffScaled, (!isIgnoreMinWidth) ? MinHeightScaled : 0);
			}
			catch
			{
			}
		}
		return Math.Max((width - (double)ParentWindowWidthDiff) / mAspectRatio.DoubleValue + (double)ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : ((FrameworkElement)this).MinHeight);
	}

	private void MainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		if (!mIsResizing)
		{
			((FrameworkElement)this).Cursor = Cursors.Arrow;
		}
	}

	private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		mDMMRecommendedWindow?.UpdateSize();
		OnResizeMainWindow();
		KMManager.sGuidanceWindow?.UpdateSize();
		mTopBar.ClosePopups();
	}

	private void MainWindow_LocationChanged(object sender, EventArgs e)
	{
		mDMMRecommendedWindow?.UpdateLocation();
		OnResizeMainWindow();
		KMManager.sGuidanceWindow?.UpdateSize();
	}

	internal void RestartInstanceAndPerform(EventHandler action = null)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (action != null)
			{
				mFrontendHandler.mEventOnFrontendClosed -= action;
				mFrontendHandler.mEventOnFrontendClosed += action;
			}
			mFrontendHandler.mEventOnFrontendClosed -= FrontendHandler_RunInstance;
			mFrontendHandler.mEventOnFrontendClosed += FrontendHandler_RunInstance;
			CloseCurrentInstanceForRestart();
		}, new object[0]);
	}

	internal void CloseAllWindowAndPerform(EventHandler action = null)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (action != null)
			{
				mEventOnAllWindowClosed -= action;
				mEventOnAllWindowClosed += action;
			}
			ForceCloseWindow(isWaitForPlayerClosing: true);
		}, new object[0]);
	}

	internal void FrontendHandler_RunInstance(object sender, EventArgs e)
	{
		CloseMainWindow();
		BlueStacksUIUtils.RunInstance(mVmName);
	}

	internal void CloseMainWindow()
	{
		HTTPUtils.SendRequestToAgentAsync("instanceStopped", (Dictionary<string, string>)null, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
			{
				Publisher.PublishMessage((BrowserControlTags)2, BlueStacksUIUtils.DictWindows.First().Key, (JObject)null);
			}
			mGuestBootCompleted = false;
			mClosing = true;
			((Window)this).Close();
		}, new object[0]);
	}

	internal void CloseCurrentInstanceForRestart()
	{
		mIsRestart = true;
		Opt.Instance.Json = "";
		ForceCloseWindow();
	}

	internal void ForceCloseWindow(bool isWaitForPlayerClosing = false)
	{
		try
		{
			CloseWindowHandler(isWaitForPlayerClosing);
		}
		catch (Exception ex)
		{
			Logger.Error("Error occured in ForceClose" + ex.ToString());
		}
	}

	internal void CloseWindow()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (mClosed)
		{
			return;
		}
		if (Oem.Instance.IsRemoveAccountOnExit)
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_CLOSING_BLUESTACKS", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_REMOVE_ACCOUNT_ON_EXIT", "");
			val.AddButton((ButtonColors)0, "STRING_REMOVE_CLOSE", this.CloseWindowConfirmationResetAccountAcceptedHandler, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_CLOSE", this.CloseWindowConfirmationAcceptedHandler, (string)null, false, (object)null);
			ShowDimOverlay();
			((Window)val).Owner = (Window)(object)mDimOverlay;
			((Window)val).ShowDialog();
			HideDimOverlay();
			return;
		}
		ProgressBar obj = new ProgressBar
		{
			ProgressText = "STRING_LOADING_MESSAGE"
		};
		((UIElement)obj).Visibility = (Visibility)1;
		ProgressBar el = obj;
		ShowDimOverlay(el);
		Thread thread = new Thread((ThreadStart)delegate
		{
			if (!FeatureManager.Instance.IsCheckForQuitPopup || FeatureManager.Instance.IsCustomUIForDMM || (!Utils.CheckQuitPopupFromCloud() && !Utils.CheckQuitPopupLocal()))
			{
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_009e: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Expected O, but got Unknown
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Expected O, but got Unknown
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Expected O, but got Unknown
					//IL_006c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0076: Expected O, but got Unknown
					HideDimOverlay();
					if (FeatureManager.Instance.IsShowAdvanceExitOption)
					{
						if (RegistryManager.Instance.IsQuitOptionSaved)
						{
							BlueStacksAdvancedExitAcceptedHandler(null, null);
						}
						else
						{
							BlueStacksAdvancedExit blueStacksAdvancedExit = new BlueStacksAdvancedExit(this);
							((UIElement)blueStacksAdvancedExit.YesButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BlueStacksAdvancedExitAcceptedHandler);
							((UIElement)blueStacksAdvancedExit.NoButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BlueStacksAdvancedExitDeclinedHandler);
							((UIElement)blueStacksAdvancedExit.CrossButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BlueStacksAdvancedExitDeclinedHandler);
							new ContainerWindow(this, (UserControl)(object)blueStacksAdvancedExit, 440.0, 400.0);
						}
					}
					else
					{
						CustomMessageWindow val2 = new CustomMessageWindow();
						if (BlueStacksUIUtils.DictWindows.Where((KeyValuePair<string, MainWindow> _) => !_.Value.mClosed).Count() == 1)
						{
							BlueStacksUIBinding.Bind(val2.TitleTextBlock, "STRING_CLOSE_BLUESTACKS", "");
							BlueStacksUIBinding.Bind(val2.BodyTextBlock, "STRING_EXIT_BLUESTACKS", "");
							if (FeatureManager.Instance.IsCustomUIForDMM)
							{
								val2.ContentMaxWidth = 400.0;
							}
						}
						else
						{
							BlueStacksUIBinding.Bind(val2.TitleTextBlock, "STRING_INSTANCE_CLOSE_TITLE", "");
							BlueStacksUIBinding.Bind(val2.BodyTextBlock, "STRING_EXIT_INSTANCE", "");
							if (FeatureManager.Instance.IsCustomUIForDMM)
							{
								val2.ContentMaxWidth = 400.0;
							}
						}
						val2.AddButton((ButtonColors)0, "STRING_CLOSE", this.CloseWindowConfirmationAcceptedHandler, (string)null, false, (object)null);
						val2.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)CloseWindowConfirmationDeniedHandler, (string)null, false, (object)null);
						ShowDimOverlayUIThread();
						((Window)val2).Owner = (Window)(object)mDimOverlay;
						((Window)val2).ShowDialog();
						if (mDimOverlay != null && !((IEnumerable)((Window)mDimOverlay).OwnedWindows).OfType<ContainerWindow>().Any())
						{
							HideDimOverlay();
						}
					}
				}, new object[0]);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void BlueStacksAdvancedExitAcceptedHandler(object sender, MouseButtonEventArgs e)
	{
		Bluester.Utils.ApplyQuit();
	}

	private void BlueStacksAdvancedExitDeclinedHandler(object sender, MouseButtonEventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)RegistryManager.Instance.InstallationType == 1 && mGuestBootCompleted)
		{
			mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
		}
	}

	private void CloseWindowConfirmationDeniedHandler(object sender, EventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)RegistryManager.Instance.InstallationType == 1 && mGuestBootCompleted)
		{
			mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
		}
	}

	internal void MainWindow_CloseWindowConfirmationAcceptedHandler(object sender, EventArgs e)
	{
		Bluester.Utils.ApplyQuit();
	}

	private void MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0)
		{
			((UIElement)mFrontendGrid).Visibility = (Visibility)1;
			if (mIsRestart)
			{
				mExitProgressGrid.ProgressText = "STRING_RESTARTING";
			}
			else
			{
				mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
			}
			((UIElement)mExitProgressGrid).Visibility = (Visibility)0;
		}
		mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync(closeWindow: true);
	}

	internal void ShowDimOverlayForUpgrade()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			CloseChildOwnedWindows();
			GotoHomeTab();
			mWelcomeTab.mFrontendPopupControl.HideWindow();
			mExitProgressGrid.ProgressText = "STRING_UPGRADING_TEXT";
			((UIElement)mExitProgressGrid).Visibility = (Visibility)0;
			HideDimOverlay();
		}, new object[0]);
	}

	private void CloseChildOwnedWindows()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		foreach (Window ownedWindow in ((Window)this).OwnedWindows)
		{
			Window val = ownedWindow;
			if (val == null)
			{
				continue;
			}
			foreach (Window ownedWindow2 in val.OwnedWindows)
			{
				Window val2 = ownedWindow2;
				if (val2 != null)
				{
					val2.Close();
				}
			}
			val.Close();
		}
	}

	private void GotoHomeTab()
	{
		if (!FeatureManager.Instance.IsCustomUIForDMM && !mTopBar.mAppTabButtons.GoToTab("Home"))
		{
			Logger.Info("Test logs: GotoHomeTab()");
			mTopBar.mAppTabButtons.AddHomeTab();
			mTopBar.mAppTabButtons.CloseTab("Setup", sendStopAppToAndroid: false, forceClose: true);
		}
	}

	internal void MinimizeWindowHandler()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				IsInNotificationMode = true;
				mIsMinimizedThroughCloseButton = true;
				HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string> { { "override", "True" } }, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Dictionary<string, AppTabButton> dictionary = new Dictionary<string, AppTabButton>();
				foreach (KeyValuePair<string, AppTabButton> mDictTab in mTopBar.mAppTabButtons.mDictTabs)
				{
					dictionary.Add(mDictTab.Key, mDictTab.Value);
				}
				foreach (KeyValuePair<string, AppTabButton> item in dictionary)
				{
					mTopBar.mAppTabButtons.CloseTabAfterQuitPopup(item.Key, sendStopAppToAndroid: true, forceClose: false);
				}
				BlueStacksUIUtils.HideUnhideParentWindow(isHide: true, this);
			}
			catch (Exception ex)
			{
				Logger.Error("Error occured in MinimizeWindowHandler " + ex.ToString());
			}
		}, new object[0]);
	}

	internal void CloseWindowHandler(bool isWaitForPlayerClosing = false)
	{
		if (mClosed)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Invalid comparison between Unknown and I4
			try
			{
				if (!mClosed)
				{
					HTTPUtils.SendRequestToAgentAsync("notificationStatsOnClosing", (Dictionary<string, string>)null, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					CloseChildOwnedWindows();
					if (CommonHandlers.sIsRecordingVideo && string.Equals(CommonHandlers.sRecordingInstance, mVmName, StringComparison.InvariantCulture))
					{
						mCommonHandler.StopRecordVideo();
					}
					GotoHomeTab();
					if ((int)RegistryManager.Instance.InstallationType == 1)
					{
						((UIElement)mWelcomeTab.mBackground).Visibility = (Visibility)2;
					}
					mWelcomeTab.mFrontendPopupControl.HideWindow();
					if (mIsRestart)
					{
						mExitProgressGrid.ProgressText = "STRING_RESTARTING";
					}
					else
					{
						mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
					}
					((UIElement)mExitProgressGrid).Visibility = (Visibility)0;
					HideDimOverlay();
					mClosed = true;
					if (!mIsRestart)
					{
						mFrontendHandler.mEventOnFrontendClosed -= FrontendHandler_CloseMainWindow;
						mFrontendHandler.mEventOnFrontendClosed += FrontendHandler_CloseMainWindow;
					}
					if (isWaitForPlayerClosing)
					{
						mFrontendHandler.KillFrontendAsync();
					}
					else
					{
						mFrontendHandler.KillFrontend();
					}
					if (mDiscordhandler != null)
					{
						mDiscordhandler.Dispose();
						mDiscordhandler = null;
					}
					if (((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(mVmName))
					{
						((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[mVmName].SendSecurityBreachesStatsToCloud(isOnClose: true);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error occured in CloseWindowHandler " + ex.ToString());
			}
		}, new object[0]);
	}

	private void FrontendHandler_CloseMainWindow(object sender, EventArgs e)
	{
		CloseMainWindow();
	}

	private void UpdateSynchronizerInstancesList()
	{
		try
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
				{
					if (dictWindow.Value.mSynchronizerWindow != null && ((UIElement)dictWindow.Value.mSynchronizerWindow).IsVisible)
					{
						dictWindow.Value.mSynchronizerWindow.Init();
					}
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
		}
	}

	private void MainWindow_Closing(object sender, CancelEventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (mClosing)
		{
			mClosing = false;
			if (!Opt.Instance.h && (int)((Window)this).WindowState == 0)
			{
				try
				{
					EngineInstanceRegistry.WindowPlacement = UsefulExtensionMethod.GetPlacement((Window)(object)this);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in MainWindow_Closing. Exception: " + ex);
				}
			}
			BlueStacksUIUtils.DictWindows[mVmName].mWelcomeTab.mHomeAppManager.DisposeHtmlSidePanel();
			BlueStacksUIUtils.DictWindows[mVmName].mWelcomeTab.DisposeHtmHomeBrowser();
			BlueStacksUIUtils.DictWindows.Remove(mVmName);
			UpdateSynchronizationState();
			UpdateSynchronizerInstancesList();
			if (mVmName == "Android")
			{
				BlueStacksUpdater.DownloadCompleted -= BlueStacksUpdater_DownloadCompleted;
			}
			this.mEventOnInstanceClosed?.Invoke(mVmName, null);
			ReleaseClientGlobalLock();
			if (BlueStacksUIUtils.DictWindows.Count == 0 && !mIsRestart)
			{
				AppUsageTimer.SaveData();
				GlobalKeyBoardMouseHooks.UnHookGlobalHooks();
				App.UnwindEvents();
				App.ReleaseLock();
				this.mEventOnAllWindowClosed?.Invoke(mVmName, null);
				if (HttpHandlerSetup.Server != null)
				{
					HttpHandlerSetup.Server.Stop();
				}
				BlueStacksUIUtils.sStopStatSendingThread = true;
				if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_MultiInstanceManager_Lockbgp64"))
				{
					if (sIsClosingForBackupRestore)
					{
						Utils.RunHDQuit(false, true, false);
					}
					else
					{
						Utils.RunHDQuit(false, true, true);
					}
				}
				Application.Current.Shutdown();
			}
			mIsRestart = false;
		}
		else
		{
			e.Cancel = true;
			CloseWindow();
		}
	}

	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		HandleDisplaySettingsChanged();
		if (string.IsNullOrEmpty(EngineInstanceRegistry.WindowPlacement) || !RegistryManager.Instance.IsRememberWindowPositionEnabled)
		{
			((Window)this).Left = (SystemParameters.MaximizedPrimaryScreenWidth - ((FrameworkElement)this).Width) / 2.0;
			((Window)this).Top = (SystemParameters.MaximizedPrimaryScreenHeight - ((FrameworkElement)this).Height) / 2.0;
		}
		else
		{
			UsefulExtensionMethod.SetPlacement((Window)(object)this, EngineInstanceRegistry.WindowPlacement);
		}
		bool flag = false;
		IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(Handle, isWorkAreaRequired: true);
		if ((((Window)this).Left + ((FrameworkElement)this).Width + (double)(EngineInstanceRegistry.IsSidebarVisible ? 62 : 0)) * sScalingFactor > (double)(fullscreenMonitorSize.Left + fullscreenMonitorSize.Width))
		{
			((Window)this).Left = (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width) / sScalingFactor - ((FrameworkElement)this).Width - 62.0;
			if (((Window)this).Left < 0.0)
			{
				((Window)this).Left = 0.0;
			}
			flag = true;
		}
		if ((((Window)this).Top + ((FrameworkElement)this).Height) * sScalingFactor > (double)(fullscreenMonitorSize.Top + fullscreenMonitorSize.Height))
		{
			((Window)this).Top = (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) / sScalingFactor - ((FrameworkElement)this).Height;
			if (((Window)this).Top < 0.0)
			{
				((Window)this).Top = 0.0;
			}
			flag = true;
		}
		if (flag)
		{
			mSidebar?.ArrangeAllSidebarElements();
		}
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			double num = (FeatureManager.Instance.IsCustomUIForDMM ? ((((FrameworkElement)this).Height - (double)ParentWindowHeightDiff) * 9.0 / 16.0 + (double)ParentWindowWidthDiff) : 0.0);
			((Window)this).Left = ((Window)this).Left - num / 2.0;
		}
		mTopBar.mPreferenceDropDownControl.Init(this);
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			mNCTopBar.mSettingsDropDownControl.Init(this);
		}
		mFullScreenTopBar.Init(this);
		CloseWindowConfirmationAcceptedHandler += MainWindow_CloseWindowConfirmationAcceptedHandler;
		CloseWindowConfirmationResetAccountAcceptedHandler += MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler;
		RestartEngineConfirmationAcceptedHandler += MainWindow_RestartEngineConfirmationAcceptedHandler;
		RestartPcConfirmationAcceptedHandler += MainWindow_RestartPcConfirmationHandler;
		GlobalKeyBoardMouseHooks.SetBossKeyHook();
		mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
		if (IsDefaultVM)
		{
			pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(3500.0);
			pikaNotificationTimer.Tick += PikaNotificationTimer_Tick;
			pikaPopControl.ParentWindow = this;
			pikaNotificationWorkQueue.Start();
		}
		ClientLaunchedStats();
		toastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
		toastTimer.Tick += ToastTimer_Tick;
		mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(5000.0);
		mFullScreenToastTimer.Tick += FullScreenToastTimer_Tick;
		SetMaxSizeOfWindow();
	}

	private void ClientLaunchedStats()
	{
		if (RegistryManager.Instance.IsClientUpgraded && RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			ClientStats.SendClientStatsAsync("update_init", "success", "emulator_activity", "", "", mVmName);
		}
		else if (RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			ClientStats.SendClientStatsAsync("first_init", "success", "emulator_activity", "", "", mVmName);
		}
		else
		{
			ClientStats.SendClientStatsAsync("init", "success", "emulator_activity", "", "", mVmName);
		}
	}

	internal void CreateFirebaseBrowserControl()
	{
		Logger.Info("In CreateFirebaseBrowserControl");
		(((Panel)FirebaseBrowserControlGrid).Children[0] as BrowserControl).CreateNewBrowser();
	}

	private void MainWindow_ContentRendered(object sender, EventArgs e)
	{
		if (isSetupDone)
		{
			return;
		}
		isSetupDone = true;
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			if (!Utils.IsRequiredFreeRAMAvailable())
			{
				mFrontendHandler.mIsSufficientRAMAvailable = false;
				mFrontendHandler.FrontendHandler_ShowLowRAMMessage();
			}
			else
			{
				Utils.CheckGuestFailedAsync();
			}
			if (mVmName == Strings.CurrentDefaultVmName)
			{
				BlueStacksUpdater.DownloadCompleted += BlueStacksUpdater_DownloadCompleted;
				BlueStacksUpdater.SetupBlueStacksUpdater(this, isStartup: true);
			}
		}
		((Window)this).ContentRendered -= MainWindow_ContentRendered;
	}

	private void BlueStacksUpdater_DownloadCompleted(Tuple<BlueStacksUpdateData, bool> result)
	{
		if (((Tuple<BlueStacksUpdateData>)(object)result).Item1.IsUpdateAvailble && ((Tuple<BlueStacksUpdateData>)(object)result).Item1.IsFullInstaller)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ShowInstallPopup();
			}, new object[0]);
		}
	}

	public void ShowInstallPopup()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		ShowDimOverlay();
		CustomMessageWindow val = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_UPDATE_AVAILABLE", "");
		val.ImageName = "update_icon";
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_NEW_UPDATE_READY", "");
		((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
		BlueStacksUIBinding.Bind(val.BodyWarningTextBlock, "STRING_NEW_UPDATE_READY_WARNING", "");
		val.BodyWarningTextBlock.Foreground = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F09200"));
		((UIElement)val.UrlTextBlock).Visibility = (Visibility)0;
		((Span)val.UrlLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
		val.UrlLink.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
		val.UrlLink.RequestNavigate += new RequestNavigateEventHandler(OpenRecentChangelogs);
		val.CloseButtonHandle((EventHandler)delegate
		{
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupCross);
		}, (object)null);
		val.AddButton((ButtonColors)4, "STRING_INSTALL_NOW", (EventHandler)delegate
		{
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupNow);
			BlueStacksUpdater.CheckDownloadedUpdateFileAndUpdate();
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_INSTALL_NEXT_BOOT", (EventHandler)delegate
		{
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupLater);
		}, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)mDimOverlay;
		((Window)val).ShowDialog();
		HideDimOverlay();
	}

	private void OpenRecentChangelogs(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		((RoutedEventArgs)e).Handled = true;
	}

	private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Key == 30 || (int)e.Key == 58)
		{
			HandleKeyDown(e.Key);
		}
		if ((int)e.SystemKey == 30)
		{
			HandleKeyDown(e.SystemKey);
		}
	}

	private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Key == 156)
		{
			HandleKeyDown(e.SystemKey);
		}
		else
		{
			HandleKeyDown(e.Key);
		}
	}

	internal static string GetShortcutKey(Key key)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		if ((int)key != 0)
		{
			if (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119))
			{
				text = IMAPKeys.GetStringForFile((Key)118) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121))
			{
				text = text + IMAPKeys.GetStringForFile((Key)120) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)116) || Keyboard.IsKeyDown((Key)117))
			{
				text = text + IMAPKeys.GetStringForFile((Key)116) + " + ";
			}
			text += IMAPKeys.GetStringForFile(key);
		}
		return text;
	}

	internal void HandleKeyDown(Key key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		string shortcutKey = GetShortcutKey(key);
		Logger.Debug("SHORTCUT: KeyPressed.." + shortcutKey);
		if (mCommonHandler.mShortcutsConfigInstance == null)
		{
			return;
		}
		foreach (ShortcutKeys item in mCommonHandler.mShortcutsConfigInstance.Shortcut)
		{
			if (string.Equals(item.ShortcutKey, shortcutKey, StringComparison.InvariantCulture))
			{
				ClientHotKeys clienthotKey = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), item.ShortcutName);
				HandleClientHotKey(clienthotKey);
				Logger.Debug("SHORTCUT: Shortcut Name.." + item.ShortcutName);
			}
		}
	}

	internal void HandleClientHotKey(ClientHotKeys clienthotKey)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected I4, but got Unknown
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Expected O, but got Unknown
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Expected O, but got Unknown
		try
		{
			switch ((int)clienthotKey)
			{
			case 31:
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							if (mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
							{
								mCommonHandler.KeyMapButtonHandler("shortcut", "sidebar");
							}
						}, new object[0]);
					}
					catch
					{
					}
				});
				break;
			case 1:
				if (!FeatureManager.Instance.IsFarmingModeDisabled)
				{
					CommonHandlers.ToggleFarmMode(!RegistryManager.Instance.CurrentFarmModeStatus);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "FarmMode", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 2:
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					break;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							mTopBar.mAppTabButtons.AddWebTab("https://www.google.com/", "Google", string.Empty, isSwitch: true, DateTime.Now.ToString(CultureInfo.InvariantCulture), forceRefresh: true);
						}, new object[0]);
					}
					catch (Exception ex4)
					{
						Logger.Error("Exception while ading web tab using key shortcut:{0}", new object[1] { ex4 });
					}
				});
				break;
			case 3:
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (FeatureManager.Instance.IsCustomUIForDMM)
						{
							mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(null, null);
						}
						else if (RegistryManager.Instance.TranslucentControlsTransparency != 0.0)
						{
							ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOff");
							KMManager.ShowOverlayWindow(this, isShow: false);
							mCommonHandler.OnOverlayStateChanged(isEnabled: false);
						}
						else if (!KMManager.CheckIfKeymappingWindowVisible() && mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
						{
							ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOn");
							mCommonHandler.OnOverlayStateChanged(isEnabled: true);
							KMManager.ShowOverlayWindow(this, isShow: true);
						}
					}, new object[0]);
				});
				break;
			case 4:
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (mCommonHandler != null && !mStreamingModeEnabled)
						{
							mCommonHandler.FullScreenButtonHandler("sidebar", "shortcut");
						}
					}, new object[0]);
				});
				break;
			case 5:
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					break;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (mCommonHandler != null)
						{
							mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: true, "shortcut", "sidebar");
						}
					}, new object[0]);
				});
				break;
			case 6:
				if (!mIsFullScreen || !RegistryManager.Instance.UseEscapeToExitFullScreen)
				{
					break;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						RestoreWindows();
					}, new object[0]);
				});
				break;
			case 7:
				if (mTopBar.mAppTabButtons.SelectedTab == null || mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
				{
					break;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (FeatureManager.Instance.IsCustomUIForDMM)
						{
							mCommonHandler.DMMSwitchKeyMapButtonHandler();
						}
						else
						{
							mSidebar.KeyMapSwitchButtonHandler(null);
						}
					}, new object[0]);
				});
				break;
			case 8:
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					break;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						try
						{
							if (mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
							{
								mCommonHandler.ImageTranslationHandler();
								ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "translatorTool", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
							}
						}
						catch (Exception ex4)
						{
							Logger.Error("error while calling image translation function.." + ex4.ToString());
						}
					}, new object[0]);
				});
				break;
			case 9:
				EngineInstanceRegistry.IsClientOnTop = !EngineInstanceRegistry.IsClientOnTop;
				((Window)this).Topmost = EngineInstanceRegistry.IsClientOnTop;
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, EngineInstanceRegistry.IsClientOnTop ? "PinToTopOn" : "PinToTopOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 11:
				if (mSidebar != null && mSidebar.GetElementFromTag("sidebar_volume") != null && (int)((UIElement)mSidebar.GetElementFromTag("sidebar_volume")).Visibility == 0 && ((UIElement)mSidebar.GetElementFromTag("sidebar_volume")).IsEnabled)
				{
					if (mSidebar.mVolumeSliderPopupTimer == null)
					{
						mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer
						{
							Interval = new TimeSpan(0, 0, 2)
						};
						mSidebar.mVolumeSliderPopupTimer.Tick += mSidebar.VolumeSliderPopupTimer_Tick;
					}
					else
					{
						mSidebar.mVolumeSliderPopupTimer.Stop();
					}
					mSidebar.mVolumeSliderPopupTimer.Start();
					((Popup)mSidebar.mVolumeSliderPopup).IsOpen = true;
					Utils.VolumeDownHandler();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeDown", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 10:
				if (mSidebar != null && mSidebar.GetElementFromTag("sidebar_volume") != null && (int)((UIElement)mSidebar.GetElementFromTag("sidebar_volume")).Visibility == 0 && ((UIElement)mSidebar.GetElementFromTag("sidebar_volume")).IsEnabled)
				{
					if (mSidebar.mVolumeSliderPopupTimer == null)
					{
						mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer
						{
							Interval = new TimeSpan(0, 0, 2)
						};
						mSidebar.mVolumeSliderPopupTimer.Tick += mSidebar.VolumeSliderPopupTimer_Tick;
					}
					else
					{
						mSidebar.mVolumeSliderPopupTimer.Stop();
					}
					mSidebar.mVolumeSliderPopupTimer.Start();
					((Popup)mSidebar.mVolumeSliderPopup).IsOpen = true;
					Utils.VolumeUpHandler();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeUp", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 12:
				if (mSidebar != null)
				{
					mCommonHandler.MuteUnmuteButtonHanlder();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, EngineInstanceRegistry.IsMuted ? "VolumeOn" : "VolumeOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 13:
				if (mSidebar != null && mSidebar.GetElementFromTag("sidebar_screenshot") != null && (int)((UIElement)mSidebar.GetElementFromTag("sidebar_screenshot")).Visibility == 0 && ((UIElement)mSidebar.GetElementFromTag("sidebar_screenshot")).IsEnabled)
				{
					mCommonHandler.ScreenShotButtonHandler();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 14:
				if (mSidebar != null && mSidebar.GetElementFromTag("sidebar_macro") != null && (int)((UIElement)mSidebar.GetElementFromTag("sidebar_macro")).Visibility == 0 && ((UIElement)mSidebar.GetElementFromTag("sidebar_macro")).IsEnabled && (FeatureManager.Instance.IsMacroRecorderEnabled || FeatureManager.Instance.IsCustomUIForNCSoft))
				{
					if (mIsMacroRecorderActive)
					{
						ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""));
					}
					else
					{
						mCommonHandler.ShowMacroRecorderWindow();
					}
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 15:
				if (FeatureManager.Instance.IsOperationsSyncEnabled)
				{
					if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(mVmName) || mIsSyncMaster)
					{
						ShowSynchronizerWindow();
					}
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				}
				break;
			case 17:
				if (mSidebar != null && mSidebar.GetElementFromTag("sidebar_video_capture") != null && (int)((UIElement)mSidebar.GetElementFromTag("sidebar_video_capture")).Visibility == 0 && ((UIElement)mSidebar.GetElementFromTag("sidebar_video_capture")).IsEnabled)
				{
					mCommonHandler.DownloadAndLaunchRecording("sidebar", "shortcut");
				}
				break;
			case 18:
				mCommonHandler.HomeButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Home", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 19:
				mCommonHandler.BackButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 20:
				mCommonHandler.ShakeButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Shake", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 21:
				if (mSidebar != null)
				{
					mSidebar.RotateButtonHandler("shortcut");
				}
				break;
			case 22:
				CommonHandlers.OpenMediaFolder();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 23:
				try
				{
					if (!FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"));
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
					}
					break;
				}
				catch (Exception ex2)
				{
					Logger.Error("Couldn't launch MI Manager. Ex: {0}", new object[1] { ex2.Message });
					break;
				}
			case 25:
				mCommonHandler.LocationButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 27:
				MinimizeWindow();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Minimize", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 28:
				CommonHandlers.ArrangeWindow();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ArrangeWindow", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			case 29:
			{
				bool flag = mIsStreaming;
				NCSoftUtils.Instance.SendStreamingEvent(mVmName, mIsStreaming ? "off" : "on");
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, flag ? "StreamVideoOff" : "StreamVideoOn", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				break;
			}
			case 26:
				if (mTopBar.mAppTabButtons.SelectedTab != null && mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab && mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppIconModel _) => _.IsGamepadCompatible) && !mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
				{
					KMManager.HandleInputMapperWindow(this, "gamepad");
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamePad", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, mTopBar.mAppTabButtons.SelectedTab?.PackageName);
				}
				break;
			case 30:
			{
				((Popup)mTopBar.mSettingsMenuPopup).IsOpen = false;
				string empty = string.Empty;
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Setting", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
				mCommonHandler.LaunchSettingsWindow(empty);
				break;
			}
			case 0:
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							if (mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
							{
								mCommonHandler.GameGuideButtonHandler("shortcut", "sidebar");
							}
						}, new object[0]);
					}
					catch
					{
					}
				});
				break;
			case 32:
				try
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string> { { "isInImagePickerMode", "true" } };
					HTTPUtils.SendRequestToEngineAsync("toggleImagePickerMode", dictionary, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
					break;
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in image picker mode: " + ex.ToString());
					break;
				}
			case 16:
			case 24:
				break;
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("Exception in executing shortcut: " + ex3.ToString());
		}
	}

	internal void ResizeMainWindowForKeyMapSidebar()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			bool flag = false;
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(Handle);
			double num = fullscreenMonitorSize.Width;
			if ((int)((Window)this).WindowState == 2 || mIsFullScreen)
			{
				RestoreWindows();
				flag = true;
			}
			if (flag || ((FrameworkElement)this).ActualWidth * sScalingFactor > num - sScalingFactor * 241.0)
			{
				double num2 = num - 241.0 * sScalingFactor;
				double heightFromWidth = GetHeightFromWidth(num2, isScaled: true);
				int num3 = Convert.ToInt32(Math.Floor(((double)fullscreenMonitorSize.Height - heightFromWidth) / 2.0));
				InteropWindow.SetWindowPos(Handle, (IntPtr)0, 0, num3, Convert.ToInt32(Math.Floor(num2)), Convert.ToInt32(Math.Floor(heightFromWidth)), 80u);
			}
			else if (((FrameworkElement)this).ActualWidth * sScalingFactor + ((Window)this).Left * sScalingFactor > num - sScalingFactor * 241.0)
			{
				InteropWindow.SetWindowPos(Handle, (IntPtr)0, Convert.ToInt32(Math.Floor(num - sScalingFactor * 241.0 - ((FrameworkElement)this).ActualWidth * sScalingFactor)), Convert.ToInt32(Math.Floor(((Window)this).Top * sScalingFactor)), Convert.ToInt32(Math.Floor(((FrameworkElement)this).ActualWidth * sScalingFactor)), Convert.ToInt32(Math.Floor(((FrameworkElement)this).ActualHeight * sScalingFactor)), 80u);
			}
		}, new object[0]);
	}

	internal Grid AddBrowser(string url)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		Grid val = new Grid();
		BrowserControl browserControl = new BrowserControl(url);
		((UIElement)browserControl).Visibility = (Visibility)0;
		BrowserControl browserControl2 = browserControl;
		CustomPictureBox val2 = new CustomPictureBox
		{
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Height = 30.0,
			Width = 30.0,
			ImageName = "loader",
			IsImageToBeRotated = true
		};
		((Panel)val).Children.Add((UIElement)(object)browserControl2);
		((Panel)val).Children.Add((UIElement)(object)val2);
		((UIElement)val).Visibility = (Visibility)1;
		((Panel)mContentGrid).Children.Add((UIElement)(object)val);
		return val;
	}

	internal void Frontend_OrientationChanged(string packagename, bool isPortrait)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (!string.IsNullOrEmpty(packagename))
			{
				AppTabButton tab = mTopBar.mAppTabButtons.GetTab(packagename);
				if (tab != null)
				{
					tab.IsPortraitModeTab = isPortrait;
				}
				if (AppForcedOrientationDict.ContainsKey(packagename))
				{
					AppForcedOrientationDict[packagename] = isPortrait;
				}
			}
			mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
		}, new object[0]);
	}

	internal void GuestBoot_Completed()
	{
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Invalid comparison between Unknown and I4
		Logger.Info("BOOT_STAGE: In Guestboot_completed ");
		ShowLoadingGrid(isShow: false);
		if (!mGuestBootCompleted)
		{
			mGuestBootCompleted = true;
			Publisher.PublishMessage((BrowserControlTags)0, mVmName, (JObject)null);
			OnGuestBootCompleted();
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((UIElement)FrontendParentGrid).Visibility = (Visibility)0;
				}, new object[0]);
			}
			HideQuitPopupIfShown();
			mWelcomeTab.mHomeAppManager.InitAppPromotionEvents();
			if ((int)RegistryManager.Instance.InstallationType == 1)
			{
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((UIElement)mWelcomeTab.mBackground).Visibility = (Visibility)2;
				}, new object[0]);
			}
			AppUsageTimer.StartTimer(mVmName, "Home");
			KMManager.GetCurrentParserVersion(this);
			Utils.GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState();
			BlueStacksUIUtils.InvokeMIManagerEvents(mVmName);
			EngineInstanceRegistry.LastBootDate = DateTime.Now.Date.ToShortDateString();
			Utils.sBootCheckTimer.Enabled = false;
			mTopBar.InitializeSnailButton();
			if (!Opt.Instance.hiddenBootMode)
			{
				CheckIfVtxDisabledOrUnavailableAndShowPopup();
			}
			FrontendHandler.UpdateBootTimeInregistry(mBootStartTime);
			GrmHandler.RequirementConfigUpdated(mVmName);
			if (!FeatureManager.Instance.IsPromotionDisabled)
			{
				mWelcomeTab.RemovePromotionGrid();
			}
			if (EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted)
			{
				Utils.MuteApplication(allInstances: false);
			}
			else
			{
				Utils.UnmuteApplication(allInstances: false);
			}
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				if ((int)RegistryManager.Instance.InstallationType == 1)
				{
					if (mTopBar.CheckForRam())
					{
						mTopBar.AddRamNotification();
					}
					bool flag = false;
					if (!string.IsNullOrEmpty(Opt.Instance.Json))
					{
						JObject val = JObject.Parse(Opt.Instance.Json);
						if (val["app_pkg"] != null && !string.IsNullOrEmpty(((object)val["app_pkg"]).ToString().Trim().Trim()))
						{
							mAppHandler.PerformGamingAction(((object)val["app_pkg"]).ToString().Trim());
							flag = true;
						}
					}
					if (!flag)
					{
						mAppHandler.PerformGamingAction();
					}
				}
				else
				{
					PerformPendingRegistryActionIfAny();
					if (EngineInstanceRegistry.IsGoogleSigninDone)
					{
						PostGoogleSigninCompleteTask();
					}
					else
					{
						if (mTopBar.CheckForRam())
						{
							mTopBar.AddRamNotification();
						}
						HandleSslConnectionError();
						PromotionObject.AppSuggestionHandler?.Invoke(obj: false);
					}
					PostBootCompleteTask();
				}
			}
			BootCompletedStats();
		}
		if ((int)EngineInstanceRegistry.NativeGamepadState == 0)
		{
			Dictionary<string, string> data = new Dictionary<string, string> { { "isEnabled", "true" } };
			mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", data);
		}
	}

	private void OnFullScreenChanged(bool isFullScreen)
	{
		this.FullScreenChanged?.Invoke(this, new MainWindowEventArgs.FullScreenChangedEventArgs
		{
			IsFullscreen = isFullScreen
		});
	}

	private void OnGuestBootCompleted()
	{
		this.GuestBootCompleted?.Invoke(this, new EventArgs());
	}

	internal void OnCursorLockChanged(bool locked)
	{
		this.CursorLockChangedEvent?.Invoke(this, new MainWindowEventArgs.CursorLockChangedEventArgs
		{
			IsLocked = locked
		});
	}

	private QuitPopupControl GetQuitPopupFromDimOverlay()
	{
		if (mDimOverlay != null)
		{
			return mDimOverlay.Control as QuitPopupControl;
		}
		return null;
	}

	private void HideQuitPopupIfShown()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				QuitPopupControl quitPopupFromDimOverlay = GetQuitPopupFromDimOverlay();
				if (quitPopupFromDimOverlay != null)
				{
					quitPopupFromDimOverlay.Close();
					ClientStats.SendLocalQuitPopupStatsAsync(quitPopupFromDimOverlay.CurrentPopupTag, "popup_auto_hidden");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't notify QuitPopup for boot complete. Ex: {0}", new object[1] { ex });
			}
		}, new object[0]);
	}

	internal void InitDiscord()
	{
		try
		{
			if (RegistryManager.Instance.DiscordEnabled && IsDefaultVM)
			{
				if (mDiscordhandler == null)
				{
					mDiscordhandler = new Discord(this);
				}
				mDiscordhandler.Init();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in init discord: {0}", new object[1] { ex.ToString() });
		}
	}

	private void HandleSslConnectionError()
	{
	}

	private void PerformPendingRegistryActionIfAny()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		string pendingAction = RegistryManager.Instance.PendingLaunchAction;
		if (string.IsNullOrEmpty(pendingAction))
		{
			return;
		}
		GenericAction action = (GenericAction)Enum.Parse(typeof(GenericAction), pendingAction.Split(new char[1] { ',' })[0].Trim(), ignoreCase: true);
		string actionValue = pendingAction.Split(new char[1] { ',' })[1].Trim();
		if ((int)action != 65536)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Invalid comparison between Unknown and I4
				Logger.Info("Performing pending registry action: {0}", new object[1] { pendingAction });
				if (mAppHandler.IsAppInstalled(actionValue))
				{
					mAppHandler.SendRunAppRequestAsync(actionValue);
				}
				else if ((int)action == 1)
				{
					mAppHandler.LaunchPlayRequestAsync(actionValue);
				}
			}, new object[0]);
		}
		RegistryManager.Instance.PendingLaunchAction = string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[2]
		{
			(object)(GenericAction)65536,
			string.Empty
		});
	}

	internal void CheckIfVtxDisabledOrUnavailableAndShowPopup()
	{
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			return;
		}
		Logger.Info("In CheckIfVtxDisabledOrUnavailableAndShowPopup");
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			string deviceCaps = RegistryManager.Instance.DeviceCaps;
			if (!string.IsNullOrEmpty(deviceCaps))
			{
				string text = null;
				JObject val = JObject.Parse(deviceCaps);
				if (((object)val["cpu_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && ((object)val["bios_hvm"]).ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
				{
					text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
					{
						WebHelper.GetServerHost(),
						"help_articles"
					}));
					text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[2] { text, "enable_virtualization" });
					ShowImprovePerformanceWarningPopup(text, "STRING_VTX_DISABLED_WARNING_MESSAGE");
				}
				else if (((object)val["cpu_hvm"]).ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
				{
					text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
					{
						WebHelper.GetServerHost(),
						"help_articles"
					}));
					text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[2] { text, "vtx_unavailable" });
					ShowImprovePerformanceWarningPopup(text, "STRING_VTX_UNAVAILABLE_WARNING_MESSAGE");
				}
				else if (((object)val["cpu_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && ((object)val["bios_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && ((object)val["engine_enabled"]).ToString().Equals(((object)(EngineState)1/*cast due to constrained. prefix*/).ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
					{
						WebHelper.GetServerHost(),
						"help_articles"
					}));
					text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[2] { text, "disable_antivirus" });
					ShowImprovePerformanceWarningPopup(text, "STRING_VTX_DISABLED_WARNING_MESSAGE");
				}
			}
		}, new object[0]);
	}

	private void ShowImprovePerformanceWarningPopup(string url, string bodyTextKeyValue)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		CustomMessageWindow window = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(window.TitleTextBlock, "STRING_IMPROVE_PERFORMANCE", "");
		window.AddWarning(LocaleStrings.GetLocalizedString("STRING_IMPROVE_PERFORMANCE_WARNING", ""), "message_error");
		((Window)window).Owner = (Window)(object)this;
		BlueStacksUIBinding.Bind(window.BodyTextBlock, bodyTextKeyValue, "");
		window.AddButton((ButtonColors)4, "STRING_CHECK_FAQ", (EventHandler)delegate
		{
			BlueStacksUIUtils.OpenUrl(url);
		}, (string)null, false, (object)null);
		window.AddButton((ButtonColors)2, "STRING_CONTINUE_ANYWAY", (EventHandler)delegate
		{
			((Window)window).Close();
		}, (string)null, false, (object)null);
		((Window)window).Show();
	}

	internal void CloseBrowserQuitPopup()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (mQuitPopupBrowserControl != null)
			{
				mQuitPopupBrowserControl.Close();
			}
		}, new object[0]);
	}

	private void BootCompletedStats()
	{
		if (RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			if (RegistryManager.Instance.IsEngineUpgraded == 1)
			{
				ClientStats.SendClientStatsAsync("update_init", "success", "engine_activity");
			}
			else
			{
				ClientStats.SendClientStatsAsync("first_init", "success", "engine_activity");
			}
			RegistryManager.Instance.IsClientFirstLaunch = 0;
			NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
			HTTPUtils.SendRequestToAgentAsync("downloadInstalledAppsCfg", (Dictionary<string, string>)null, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		else
		{
			ClientStats.SendClientStatsAsync("init", "success", "engine_activity");
		}
	}

	private void PostBootCompleteTask()
	{
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		GetMacroShortcutKeyMappingsWithRestrictedKeysandNames();
		PromotionObject.AppRecommendationHandler?.Invoke(obj: false);
		PostBootCloudInfoManager.Instance.GetPostBootDataAsync(this);
		CheckUserPremiumAsync();
		if (RegistryManager.Instance.RequirementConfigUpdateRequired)
		{
			HTTPUtils.SendRequestToGuest("getConfigList", (Dictionary<string, string>)null, mVmName, 1000, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			RegistryManager.Instance.RequirementConfigUpdateRequired = false;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
		}, new object[0]);
		PromotionObject.QuestHandler = (Action)Delegate.Remove(PromotionObject.QuestHandler, new Action(HandleQuestForFrontend));
		PromotionObject.QuestHandler = (Action)Delegate.Combine(PromotionObject.QuestHandler, new Action(HandleQuestForFrontend));
		HandleQuestForFrontend();
		mAppHandler.UpdateDefaultLauncher();
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mAppInstaller = new DownloadInstallApk(this);
			if (Oem.Instance.IsDragDropEnabled)
			{
				FileImporter.Init(this);
			}
		}, new object[0]);
		try
		{
			DownloadInstallApk.SerialWorkQueueInstaller(mVmName).Start();
		}
		catch (ThreadStateException ex)
		{
			Logger.Info("Thread Already Started" + ex.ToString());
		}
		if (mStartupTabLaunched)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mTopBar.mAppTabButtons.GoToTab(1);
			}, new object[0]);
		}
		else if (mPostBootNotificationAction != null)
		{
			AppInfo appInfoFromPackageName = new JsonParser(mVmName).GetAppInfoFromPackageName(mPostBootNotificationAction);
			if (appInfoFromPackageName != null)
			{
				string fileName = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
				JObject val = new JObject();
				val.Add("app_icon_url", JToken.op_Implicit(""));
				val.Add("app_name", JToken.op_Implicit(appInfoFromPackageName.Name));
				val.Add("app_url", JToken.op_Implicit(""));
				val.Add("app_pkg", JToken.op_Implicit(mPostBootNotificationAction));
				JObject val2 = val;
				string text = "-json \"" + ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]).Replace("\"", "\\\"") + "\"";
				Process.Start(fileName, string.Format(CultureInfo.InvariantCulture, "{0} -vmname {1}", new object[2] { text, mVmName }));
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("click_generic_action", ((object)(GenericAction)1/*cast due to constrained. prefix*/).ToString());
				dictionary.Add("click_action_packagename", mPostBootNotificationAction);
				Utils.HandleGenericActionFromDictionary(dictionary, "notification_drawer");
			}
			mPostBootNotificationAction = null;
		}
		HandleFLEorAppPopupPostBoot();
		mCommonHandler.CheckForMacroScriptOnRestart();
		UpdateSynchronizerInstancesList();
		InitDiscord();
		Utils.SetGoogleAdIdAndAndroidIdFromAndroid(mVmName);
		if (PromotionObject.Instance != null && PromotionObject.Instance.IsSecurityMetricsEnable)
		{
			SecurityMetrics.Init(mVmName);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mResizeHandler.AddRawInputHandler();
			}, new object[0]);
		}
		RegistryManager.Instance.Guest[mVmName].IsGoogleSigninPopupShown = true;
		if (new DriveInfo(Path.GetPathRoot(RegistryManager.Instance.UserDefinedDir)).AvailableFreeSpace < 1073741824)
		{
			ShowLowDiskSpaceWarning();
		}
		if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1 && VersionCheckForSmartControl())
		{
			ShowAppPopupAfterUpgrade("com.dts.freefireth");
		}
		if (string.Compare(mVmName, Strings.CurrentDefaultVmName, StringComparison.OrdinalIgnoreCase) == 0)
		{
			CloudNotificationManager.PostBootCompleted();
		}
	}

	private void ShowAppPopupAfterUpgrade(string packageName)
	{
		try
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Expected O, but got Unknown
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Expected O, but got Unknown
				if (mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null && File.Exists(Utils.GetInputmapperUserFilePath(packageName)))
				{
					CustomMessageWindow val = new CustomMessageWindow();
					val.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SMART_CONTROLS_ENABLED_0", ""), new object[1] { "Garena Free Fire" });
					val.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_MESSAGE", ""), new object[1] { "Garena Free Fire" });
					((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
					BlueStacksUIBinding.Bind(val.BodyWarningTextBlock, "STRING_FREEFIRE_NOTIFICATION_DETAIL", "");
					val.BodyWarningTextBlock.FontWeight = FontWeights.Light;
					BlueStacksUIBinding.BindColor((DependencyObject)(object)val.BodyWarningTextBlock, Control.ForegroundProperty, "SettingsWindowForegroundDimDimColor");
					val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
					((UIElement)val.UrlTextBlock).Visibility = (Visibility)0;
					((Span)val.UrlLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_LINK", ""));
					string uriString = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
					{
						WebHelper.GetServerHost(),
						"help_articles"
					})) + "&article=smart_control";
					val.UrlLink.NavigateUri = new Uri(uriString);
					val.UrlLink.RequestNavigate += new RequestNavigateEventHandler(OpenSmartControlHelp);
					ShowDimOverlay();
					((Window)val).Owner = (Window)(object)mDimOverlay;
					((Window)val).ShowDialog();
					if (mDimOverlay != null && ((IEnumerable)((Window)mDimOverlay).OwnedWindows).OfType<ContainerWindow>().Any())
					{
						HideDimOverlay();
					}
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in showing app notifications after upgrade: " + ex.ToString());
		}
	}

	private void OpenSmartControlHelp(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
	}

	private static bool VersionCheckForSmartControl()
	{
		if (RegistryManager.Instance.UpgradeVersionList.Length != 0)
		{
			Version version = new Version("4.140.00.0000");
			return new Version(RegistryManager.Instance.UpgradeVersionList.Last()) < version;
		}
		return false;
	}

	private void ShowLowDiskSpaceWarning()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_MESSAGE", "");
			val.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_WARNING", ""), "");
			((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
			val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
			ShowDimOverlay();
			((Window)val).Owner = (Window)(object)mDimOverlay;
			((Window)val).ShowDialog();
			HideDimOverlay();
		}, new object[0]);
	}

	internal void PostGoogleSigninCompleteTask()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		if (!mIsTokenAvailable && (int)RegistryManager.Instance.InstallationType != 1)
		{
			BlueStacksUIUtils.SendBluestacksLoginRequest(mVmName);
		}
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mTopBar.TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mTopBar.mUserAccountBtn, isVisible: true);
				mTopBar.TopBarOptionsPanelElementVisibility((FrameworkElement)(object)mTopBar.mNotificationGrid, isVisible: true);
			}, new object[0]);
			PromotionObject.AppSuggestionHandler?.Invoke(obj: false);
		}
		this.BrowserOTSCompletedCallback?.Invoke(this, new MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs
		{
			CallbackFunction = mBrowserCallbackFunctionName
		});
	}

	internal static void CheckUserPremiumAsync()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				PromotionManager.CheckIsUserPremium();
			}
			catch (Exception ex)
			{
				Logger.Error("PostOTSBootComplete: call for premium failed" + ex.ToString());
			}
		});
	}

	private void HandleQuestForFrontend()
	{
		if (((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules).Count > 0)
		{
			Dictionary<string, string> data = new Dictionary<string, string> { 
			{
				"data",
				JsonConvert.SerializeObject((object)PromotionObject.Instance.QuestHdPlayerRules, (Formatting)0)
			} };
			mFrontendHandler.SendFrontendRequest("setPackagesForInteraction", data);
			PromotionManager.StartQuestRulesProcessor();
		}
	}

	private void HandleFLEorAppPopupPostBoot()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Invalid comparison between Unknown and I4
		GetFleCampaignJson();
		if (!RegistryManager.Instance.Guest[mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[mVmName].IsGoogleSigninDone)
		{
			return;
		}
		if (!string.IsNullOrEmpty(Opt.Instance.Json) && string.Equals("Android", mVmName, StringComparison.InvariantCulture))
		{
			JObject val = JObject.Parse(Opt.Instance.Json);
			if (val["app_pkg"] != null && !string.IsNullOrEmpty(((object)val["app_pkg"]).ToString().Trim().Trim()))
			{
				new DownloadInstallApk(this).DownloadAndInstallAppFromJson(Opt.Instance.Json);
				return;
			}
		}
		if (mStartupTabLaunched || ((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab).Count <= 0)
		{
			return;
		}
		if (((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab).ContainsKey("click_generic_action") && (int)EnumHelper.Parse<GenericAction>(((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab)["click_generic_action"], (GenericAction)65536) != 65536 && (string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail.Trim()) || string.IsNullOrEmpty(RegistryManager.Instance.Token.Trim())))
		{
			mLaunchStartupTabWhenTokenReceived = true;
		}
		if (!mLaunchStartupTabWhenTokenReceived)
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mStartupTabLaunched = true;
				Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab, "startup_tab");
			}, new object[0]);
		}
	}

	public void PublishForFlePopupToBrowser(string json)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (!string.IsNullOrEmpty(json))
		{
			JObject val = JObject.Parse(json);
			if (val["fle_pkg"] != null)
			{
				string text = ((object)val["fle_pkg"]).ToString().Trim();
				Publisher.PublishMessage((BrowserControlTags)25, mVmName, new JObject((object)new JProperty("PackageName", (object)text)));
			}
		}
	}

	private void GetFleCampaignJson()
	{
		string fLECampaignMD = RegistryManager.Instance.FLECampaignMD5;
		if (!string.IsNullOrEmpty(fLECampaignMD))
		{
			try
			{
				string campaignJson = BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/getcampaigninfo?md5_hash={1}", new object[2]
				{
					RegistryManager.Instance.Host,
					fLECampaignMD
				}), (Dictionary<string, string>)null, false, mVmName, 0, 1, 0, false, "bgp64");
				RegistryManager.Instance.DeleteFLECampaignMD5();
				RegistryManager.Instance.CampaignJson = campaignJson;
			}
			catch
			{
				Logger.Info("Error fetching campaign json");
			}
		}
	}

	private void HandleFLEorAppPopupBeforeBoot()
	{
		GetFleCampaignJson();
		if (!RegistryManager.Instance.Guest[mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[mVmName].IsGoogleSigninDone)
		{
			return;
		}
		if (!string.IsNullOrEmpty(Opt.Instance.Json) && string.Equals("Android", mVmName, StringComparison.InvariantCulture))
		{
			JObject val = JObject.Parse(Opt.Instance.Json);
			if (val["app_pkg"] != null && !string.IsNullOrEmpty(((object)val["app_pkg"]).ToString().Trim().Trim()))
			{
				return;
			}
		}
		if (((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab).Count <= 0)
		{
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab).ContainsKey("click_generic_action") && (EnumHelper.Parse<GenericAction>(((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab)["click_generic_action"], (GenericAction)65536) & 0x24) != 0)
			{
				mStartupTabLaunched = true;
				Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab, "startup_tab");
			}
		}, new object[0]);
	}

	internal void ShowLoadingGrid(bool isShow)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				if (isShow)
				{
					mTopBar.mAppTabButtons.EnableAppTabs(isEnableTab: false);
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility((Visibility)0);
					}
				}
				else
				{
					mTopBar.mAppTabButtons.EnableAppTabs(isEnableTab: true);
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility((Visibility)1);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ShowLoadingGrid. " + ex.ToString());
			}
			Logger.Info("BOOT_STAGE: Removing progress bar");
		}, new object[0]);
	}

	internal void ShowControlGrid(Grid controlGrid)
	{
		if (mLastVisibleGrid != null && controlGrid != mLastVisibleGrid)
		{
			((UIElement)mLastVisibleGrid).Visibility = (Visibility)1;
		}
		mLastVisibleGrid = controlGrid;
		((UIElement)controlGrid).Visibility = (Visibility)0;
	}

	private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(CustomPictureBox)) || ((UIElement)mTopBar.WindowHeaderGrid).IsMouseOver)
		{
			try
			{
				((Window)this).DragMove();
			}
			catch
			{
			}
			UIChangesOnMainWindowSizeChanged();
		}
	}

	private void TopBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		if (!((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
		{
			if ((int)((Window)this).WindowState == 2)
			{
				RestoreWindows();
			}
			else
			{
				MaximizeWindow();
			}
		}
	}

	internal void RestoreWindows(bool isReArrange = false)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		if (!mResizeHandler.IsMinMaxEnabled)
		{
			return;
		}
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((Popup)mTopBarPopup).IsOpen = false;
		}
		if (((Popup)mGeneraltoast).IsOpen)
		{
			toastTimer.Stop();
			((Popup)mGeneraltoast).IsOpen = false;
		}
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			OnFullScreenChanged(isFullScreen: false);
			ToggleFullScreenToastVisibility(isFullScreen: false);
		}
		((UIElement)TopBar).Visibility = (Visibility)0;
		OuterBorder.BorderThickness = new Thickness(1.0);
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((UIElement)mDmmBottomBar).Visibility = (Visibility)0;
		}
		if (!isReArrange && mIsFullScreenFromMaximized && mIsFullScreen)
		{
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(Handle, isWorkAreaRequired: true);
			InteropWindow.SetWindowPos(Handle, (IntPtr)0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80u);
			UIChangesOnMainWindowSizeChanged();
		}
		else
		{
			mIsFullScreenFromMaximized = false;
			if (FeatureManager.Instance.IsCustomUIForDMM && mDMMRecommendedWindow != null && mIsDMMRecommendedWindowOpen)
			{
				((UIElement)mDMMRecommendedWindow).Visibility = (Visibility)0;
			}
			mResizeHandler.mAdjustingWidth = false;
			if (mTopBar.mAppTabButtons.SelectedTab != null)
			{
				IsUIInPortraitMode = mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab;
			}
			((UIElement)mResizeGrid).Visibility = (Visibility)0;
			((FrameworkElement)FrontendParentGrid).Margin = new Thickness(1.0);
			((Window)this).WindowState = (WindowState)0;
			ChangeHeightWidthAndPosition(GetWidthFromHeight(mPreviousHeight.Value), mPreviousHeight.Value, changePosition: true);
			SwitchToPortraitMode(IsUIInPortraitMode);
			mIsDmmMaximised = false;
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);
			}
			else if (IsUIInPortraitMode)
			{
				mTopBar.RefreshNotificationCentreButton();
			}
			mResizeHandler.IsResizingEnabled = true;
			mTopBar.mMaximizeButton.ImageName = "maximize";
			BlueStacksUIBinding.Bind((Image)(object)mTopBar.mMaximizeButton, "STRING_MAXIMIZE_TOOLTIP");
			mNCTopBar.mMaximizeButtonImage.ImageName = "maximize";
			BlueStacksUIBinding.Bind((Image)(object)mNCTopBar.mMaximizeButtonImage, "STRING_MAXIMIZE_TOOLTIP");
			mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
			if (KMManager.sGuidanceWindow != null)
			{
				((UIElement)KMManager.sGuidanceWindow).Visibility = (Visibility)0;
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((UIElement)this).Focus();
				}, new object[0]);
			}
		}
		mTopBar.UpdateMacroRecordingProgress();
		mIsFullScreen = false;
		HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string> { { "isFullscreen", "false" } }, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
		mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
	}

	internal void MinimizeWindow()
	{
		((Window)this).WindowState = (WindowState)1;
	}

	internal void MaximizeWindow()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!mResizeHandler.IsMinMaxEnabled)
		{
			return;
		}
		mIsDMMMaximizedFromPortrait = IsUIInPortraitMode;
		if (FeatureManager.Instance.IsCustomUIForDMM && mDMMRecommendedWindow != null)
		{
			((UIElement)mDMMRecommendedWindow).Visibility = (Visibility)1;
		}
		if ((int)((Window)this).WindowState == 0)
		{
			mPreviousWidth = ((FrameworkElement)this).Width;
			mPreviousHeight = ((FrameworkElement)this).Height;
		}
		mIsDmmMaximised = true;
		if (FeatureManager.Instance.IsCustomUIForDMM && IsUIInPortraitMode && !mIsFullScreen)
		{
			SetDMMRestoreWindowSizeAndPosition();
			SetSizeForDMMPortraitMaximisedWindow();
		}
		else
		{
			if (FeatureManager.Instance.IsCustomUIForDMM && !mIsFullScreen)
			{
				SetDMMRestoreWindowSizeAndPosition();
			}
			IsUIInPortraitModeWhenMaximized = IsUIInPortraitMode;
			IsUIInPortraitMode = !(mAspectRatio > 1L);
			((Window)this).WindowState = (WindowState)2;
		}
		mResizeHandler.IsResizingEnabled = false;
		mTopBar.mMaximizeButton.ImageName = "restore";
		mTopBar.RefreshNotificationCentreButton();
		mTopBar.UpdateMacroRecordingProgress();
		BlueStacksUIBinding.Bind((Image)(object)mTopBar.mMaximizeButton, "STRING_RESTORE_BUTTON");
		mNCTopBar.mMaximizeButtonImage.ImageName = "restore";
		BlueStacksUIBinding.Bind((Image)(object)mNCTopBar.mMaximizeButtonImage, "STRING_RESTORE_BUTTON");
		mTopBar.RefreshWarningButton();
		UIChangesOnMainWindowSizeChanged();
		if (KMManager.sGuidanceWindow != null && string.Equals(KMManager.sGuidanceWindow.ParentWindow?.mVmName, mVmName, StringComparison.InvariantCultureIgnoreCase))
		{
			((UIElement)KMManager.sGuidanceWindow).Visibility = (Visibility)2;
			if (AppConfigurationManager.Instance.VmAppConfig[mVmName].ContainsKey(mTopBar.mAppTabButtons.SelectedTab.PackageName) && !AppConfigurationManager.Instance.CheckIfTrueInAnyVm(mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>)((AppSettings appSettings) => appSettings.IsCloseGuidanceOnboardingCompleted)) && !mIsFullScreen)
			{
				mSidebar?.ShowViewGuidancePopup();
				AppConfigurationManager.Instance.VmAppConfig[mVmName][mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
			}
		}
	}

	internal void RestrictWindowResize(bool enable)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mResizeHandler.IsMinMaxEnabled = !enable;
			mResizeHandler.IsResizingEnabled = !enable;
			((UIElement)mTopBar.mMaximizeButton).IsEnabled = !enable;
			((UIElement)mNCTopBar.mMaximizeButtonImage).IsEnabled = !enable;
			if (enable)
			{
				mTopBar.mMaximizeButton.SetDisabledState();
				mNCTopBar.mMaximizeButtonImage.SetDisabledState();
			}
			else
			{
				mTopBar.mMaximizeButton.SetNormalState();
				mNCTopBar.mMaximizeButtonImage.SetNormalState();
			}
		}, new object[0]);
	}

	internal void FullScreenWindow()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Invalid comparison between Unknown and I4
		if (!FeatureManager.Instance.IsCustomUIForDMM && mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
		{
			return;
		}
		if ((int)((Window)this).WindowState == 0)
		{
			mPreviousWidth = ((FrameworkElement)this).Width;
			mPreviousHeight = ((FrameworkElement)this).Height;
		}
		mIsFullScreen = true;
		OuterBorder.BorderThickness = new Thickness(0.0);
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((UIElement)mDmmBottomBar).Visibility = (Visibility)2;
			mDmmBottomBar.ShowKeyMapPopup(isShow: false);
		}
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			OnFullScreenChanged(isFullScreen: true);
		}
		((UIElement)TopBar).Visibility = (Visibility)2;
		((UIElement)mResizeGrid).Visibility = (Visibility)2;
		((FrameworkElement)FrontendParentGrid).Margin = new Thickness(0.0);
		if ((int)((Window)this).WindowState == 2)
		{
			mIsFullScreenFromMaximized = true;
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(Handle);
			InteropWindow.SetWindowPos(Handle, (IntPtr)0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80u);
		}
		else
		{
			if (FeatureManager.Instance.IsCustomUIForDMM && (int)((Window)this).WindowState != 2 && !mIsDmmMaximised)
			{
				SetDMMRestoreWindowSizeAndPosition();
			}
			MaximizeWindow();
		}
		Cursor.Clip = Rectangle.Empty;
		HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string> { { "isFullscreen", "true" } }, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
		Thread thread = new Thread((ThreadStart)delegate
		{
			Thread.Sleep(1000);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (FeatureManager.Instance.IsCustomUIForDMM && !((UIElement)mDMMFST).IsMouseOver && !((Popup)mDMMFST.mVolumePopup).IsOpen && !((Popup)mDMMFST.mChangeTransparencyPopup).IsOpen)
				{
					((Popup)mTopBarPopup).IsOpen = false;
				}
			}, new object[0]);
		});
		thread.IsBackground = true;
		thread.Start();
		UIChangesOnMainWindowSizeChanged();
		if (KMManager.sGuidanceWindow != null)
		{
			((UIElement)KMManager.sGuidanceWindow).Visibility = (Visibility)2;
		}
	}

	public void ToggleFullScreenToastVisibility(bool isFullScreen, string tip = "", string key = "", string info = "")
	{
		if (isFullScreen)
		{
			ShowToast(tip, key, info, isForced: true);
		}
		else
		{
			CloseFullScreenToastAndStopTimer();
		}
	}

	internal void ShowToast(string tip, string key = "", string info = "", bool isForced = false)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				if (isForced || (mIsWindowInFocus && !FeatureManager.Instance.IsCustomUIForDMM))
				{
					if (((Popup)mFullScreenToastPopup).IsOpen)
					{
						mFullScreenToastTimer.Stop();
						((Popup)mFullScreenToastPopup).IsOpen = false;
					}
					if (isForced)
					{
						if (string.IsNullOrEmpty(key))
						{
							return;
						}
						mFullScreenToastControl.Init(this, tip, key, info);
					}
					else
					{
						mFullScreenToastControl.Init(this, tip);
					}
					((FrameworkElement)dummyToast).HorizontalAlignment = (HorizontalAlignment)1;
					((FrameworkElement)dummyToast).VerticalAlignment = (VerticalAlignment)0;
					((UIElement)mFullScreenToastControl).Visibility = (Visibility)0;
					((Popup)mFullScreenToastPopup).IsOpen = true;
					((FrameworkElement)mFullScreenToastCanvas).Height = ((FrameworkElement)mFullScreenToastControl).ActualHeight;
					((Popup)mFullScreenToastPopup).VerticalOffset = ((FrameworkElement)mFullScreenToastControl).ActualHeight + 20.0;
					((FrameworkElement)mFullScreenToastPopup).HorizontalAlignment = (HorizontalAlignment)1;
					if (mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen)
					{
						mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
					}
					else
					{
						mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(5000.0);
					}
					mFullScreenToastTimer.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing fullscreen toast : " + ex.ToString());
			}
		}, new object[0]);
	}

	private void SetDMMRestoreWindowSizeAndPosition()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DmmRestoreWindowRectangle = new Rect(((Window)this).Left, ((Window)this).Top, ((FrameworkElement)this).Width, ((FrameworkElement)this).Height);
	}

	private void SetSizeForDMMPortraitMaximisedWindow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Rect workArea = SystemParameters.WorkArea;
		double num = ((Rect)(ref workArea)).Height;
		double num2 = GetWidthFromHeight(num);
		double num3 = num2;
		workArea = SystemParameters.WorkArea;
		if (num3 > ((Rect)(ref workArea)).Width / sScalingFactor)
		{
			workArea = SystemParameters.WorkArea;
			num2 = ((Rect)(ref workArea)).Width / sScalingFactor;
			num = GetHeightFromWidth(num2);
		}
		if (num2 < ((FrameworkElement)this).MinWidth || num < ((FrameworkElement)this).MinHeight)
		{
			num2 = ((FrameworkElement)this).MinWidth;
			num = ((FrameworkElement)this).MinHeight;
		}
		((FrameworkElement)this).Height = num;
		((FrameworkElement)this).Width = num2;
		workArea = SystemParameters.WorkArea;
		((Window)this).Left = (((Rect)(ref workArea)).Width - ((FrameworkElement)this).Width) / 2.0;
		((Window)this).Top = 0.0;
	}

	private void BottomBar_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
		{
			try
			{
				((Window)this).DragMove();
			}
			catch
			{
			}
		}
	}

	private void ResizeGrid_Loaded(object sender, RoutedEventArgs e)
	{
		if (mResizeGrid == null)
		{
			mResizeGrid = (Grid)((sender is Grid) ? sender : null);
			WireSizingEvents();
		}
	}

	private void WireSizingEvents()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		foreach (UIElement child in ((Panel)mResizeGrid).Children)
		{
			Rectangle val2 = (Rectangle)((child is Rectangle) ? child : null);
			if (val2 != null)
			{
				((UIElement)val2).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(mResizeHandler.ResizeRectangle_PreviewMouseDown);
				((UIElement)val2).MouseMove += new MouseEventHandler(mResizeHandler.ResizeRectangle_MouseMove);
			}
		}
	}

	private void FrontendGrid_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
	{
		mFrontendHandler.FrontendVisibleChanged((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		Logger.Debug("KMP FrontendGrid_IsVisibleChanged " + ((DependencyPropertyChangedEventArgs)(ref e)).NewValue?.ToString() + mVmName);
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			OnFrontendGridVisible();
		}
		else
		{
			OnFrontendGridHidden();
		}
		mFrontendHandler.ShowGLWindow();
	}

	private void OnFrontendGridHidden()
	{
		mFrontendHandler.DeactivateFrontend();
		this.FrontendGridVisibilityChanged?.Invoke(this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs
		{
			IsVisible = false
		});
	}

	private void OnFrontendGridVisible()
	{
		this.FrontendGridVisibilityChanged?.Invoke(this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs
		{
			IsVisible = true
		});
	}

	private void FrontendGrid_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		mFrontendHandler.ShowGLWindow();
	}

	private void FrontendParentGrid_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)FrontendParentGrid).Visibility != 0)
		{
			return;
		}
		if (!((Panel)FrontendParentGrid).Children.Contains((UIElement)(object)mFrontendGrid))
		{
			if (((FrameworkElement)mFrontendGrid).Parent != null)
			{
				DependencyObject parent = ((FrameworkElement)mFrontendGrid).Parent;
				((Panel)((parent is Grid) ? parent : null)).Children.Remove((UIElement)(object)mFrontendGrid);
			}
			((Panel)FrontendParentGrid).Children.Add((UIElement)(object)mFrontendGrid);
		}
		if (mGuestBootCompleted && FeatureManager.Instance.IsCustomUIForDMM)
		{
			((UIElement)mDmmProgressControl).Visibility = (Visibility)1;
			((UIElement)mFrontendGrid).Visibility = (Visibility)0;
		}
	}

	internal void HandleRestartPopup()
	{
		Logger.Info("Showing restart option to the user");
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CustomMessageWindow val = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
				BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_ENGINE_RESTART", "");
				val.AddButton((ButtonColors)4, "STRING_RESTART_ENGINE", this.RestartEngineConfirmationAcceptedHandler, (string)null, false, (object)null);
				val.AddButton((ButtonColors)2, "STRING_RESTART_PC", this.RestartPcConfirmationAcceptedHandler, (string)null, false, (object)null);
				ShowDimOverlay();
				((Window)val).Owner = (Window)(object)mDimOverlay;
				((Window)val).ShowDialog();
				HideDimOverlay();
			}
			catch (Exception ex)
			{
				Logger.Error("Error window probably closed");
				Logger.Error(ex.ToString());
			}
		}, new object[0]);
	}

	internal void MainWindow_RestartEngineConfirmationAcceptedHandler(object sender, EventArgs e)
	{
		BlueStacksUIUtils.RestartInstance(mVmName);
	}

	private void MainWindow_RestartPcConfirmationHandler(object sender, EventArgs e)
	{
		Process.Start("shutdown.exe", "-r -t 0");
	}

	private void WelcomeTabParentGrid_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mWelcomeTab).Visibility = ((UIElement)WelcomeTabParentGrid).Visibility;
	}

	private void PikaPopControl_CloseClicked(object sender, EventArgs e)
	{
		((Popup)pikaPop).IsOpen = false;
		isPikaPopOpen = false;
	}

	internal void ClosePopUps()
	{
		PikaPopControl_CloseClicked(this, null);
		mWelcomeTab.mHomeAppManager.CloseHomeAppPopups();
		((Popup)toastPopup).IsOpen = false;
		((Popup)mShootingModePopup).IsOpen = false;
		((Popup)mFullScreenToastPopup).IsOpen = false;
		((Popup)mFullscreenTopbarPopup).IsOpen = false;
		((Popup)mFullscreenTopbarPopupButton).IsOpen = false;
		((Popup)mFullscreenSidebarPopup).IsOpen = false;
		((Popup)mFullscreenSidebarPopupButton).IsOpen = false;
		if (mSidebar.mListPopups == null)
		{
			return;
		}
		foreach (CustomPopUp mListPopup in mSidebar.mListPopups)
		{
			((Popup)mListPopup).IsOpen = false;
		}
	}

	private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked back button setup bottombar ");
		mCommonHandler.BackButtonHandler();
	}

	private void TopBarPopup_MouseLeave(object sender, MouseEventArgs e)
	{
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			if (!((UIElement)mDMMFST).IsMouseOver && !((Popup)mDMMFST.mVolumePopup).IsOpen && !((Popup)mDMMFST.mChangeTransparencyPopup).IsOpen)
			{
				((Popup)mTopBarPopup).IsOpen = false;
			}
		}
		else if (!((Popup)mFullScreenTopBar.mChangeTransparencyPopup).IsOpen)
		{
			((Popup)mTopBarPopup).IsOpen = false;
		}
	}

	internal void SetMacroPlayBackEventHandle()
	{
		try
		{
			using EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(BlueStacksUIUtils.GetMacroPlaybackEventName(mVmName));
			eventWaitHandle.Set();
		}
		catch (Exception ex)
		{
			Logger.Warning("Unable to set macro playback event err:" + ex.ToString());
		}
	}

	internal void StartTimerForAppPlayerRestart(int interval)
	{
		mMacroTimer = new System.Timers.Timer(interval * 60 * 1000);
		mMacroTimer.Elapsed -= MacroTimer_Elapsed;
		mMacroTimer.Elapsed += MacroTimer_Elapsed;
		mMacroTimer.Start();
	}

	private void Fullscreentopbar_opened(object sender, EventArgs e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (((Popup)mTopBarPopup).IsOpen)
		{
			((UIElement)this).MouseMove -= new MouseEventHandler(MainWindow_MouseMove);
			((UIElement)this).MouseMove += new MouseEventHandler(MainWindow_MouseMove);
		}
		else
		{
			((UIElement)this).MouseMove -= new MouseEventHandler(MainWindow_MouseMove);
		}
	}

	private void MacroTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (!mMacroTimer.Enabled)
		{
			return;
		}
		mMacroTimer.Enabled = false;
		mMacroTimer.AutoReset = false;
		mMacroTimer.Dispose();
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mTopBar.HideMacroPlaybackFromTopBar();
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				mNCTopBar.HideMacroPlaybackFromTopBar();
			}
			mIsMacroPlaying = false;
			mMacroPlaying = string.Empty;
			BlueStacksUIUtils.RestartInstance(mVmName);
		}, new object[0]);
	}

	internal void ShowSynchronizerWindow()
	{
		((Popup)mTopBar.mSettingsMenuPopup).IsOpen = false;
		if (mSynchronizerWindow == null)
		{
			mSynchronizerWindow = new SynchronizerWindow(this);
		}
		mSynchronizerWindow.Init();
		((Window)mSynchronizerWindow).Show();
		((CustomWindow)mSynchronizerWindow).ShowWithParentWindow = true;
	}

	private void ReleaseClientGlobalLock()
	{
		try
		{
			if (mBlueStacksClientInstanceLock != null)
			{
				mBlueStacksClientInstanceLock.ReleaseMutex();
				mBlueStacksClientInstanceLock.Close();
				mBlueStacksClientInstanceLock = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in releasing client global lock.." + ex);
		}
	}

	private void MainWindow_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (mIsFullScreen && ((Popup)mTopBarPopup).IsOpen)
			{
				Point position = e.GetPosition((IInputElement)(object)mDMMFST);
				if (((Point)(ref position)).Y > 80.0 && !((Popup)mDMMFST.mChangeTransparencyPopup).IsOpen)
				{
					((Popup)mTopBarPopup).IsOpen = false;
				}
			}
		}
		catch
		{
		}
	}

	internal void ShowLockScreen()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (mIsLockScreenActionPending)
		{
			return;
		}
		if (EngineInstanceRegistry.IsClientOnTop)
		{
			((Window)this).Topmost = false;
			EngineInstanceRegistry.IsClientOnTop = false;
		}
		if (mDimOverlay != null && ((Window)mDimOverlay).OwnedWindows.Count > 0)
		{
			foreach (Window ownedWindow in ((Window)mDimOverlay).OwnedWindows)
			{
				ownedWindow.Close();
			}
		}
		else if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null && (int)((UIElement)KMManager.CanvasWindow.SidebarWindow).Visibility == 0)
		{
			((Window)KMManager.CanvasWindow.SidebarWindow).Close();
		}
		else if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed && (int)((UIElement)KMManager.sGuidanceWindow).Visibility == 0)
		{
			((Window)KMManager.sGuidanceWindow).Close();
		}
		KMManager.ShowOverlayWindow(this, isShow: false);
		if (mMacroRecorderWindow != null)
		{
			mCommonHandler.HideMacroRecorderWindow();
		}
		mIsLockScreenActionPending = true;
		ShowDimOverlay(ScreenLockInstance);
	}

	internal void HideLockScreen()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (mDimOverlay != null && (int)((UIElement)ScreenLockInstance).Visibility == 0)
		{
			mIsLockScreenActionPending = false;
			HideDimOverlay();
			ShowWindow();
			((Window)this).Activate();
			if (RegistryManager.Instance.ShowKeyControlsOverlay && !KMManager.CheckIfKeymappingWindowVisible())
			{
				KMManager.ShowOverlayWindow(this, isShow: true);
			}
		}
	}

	private void UpdateSynchronizationState()
	{
		_TopBar.HideSyncPanel();
		if (mIsSyncMaster)
		{
			mSynchronizerWindow.StopAllSyncOperations();
			return;
		}
		if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(mVmName))
		{
			HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>)null, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
			BlueStacksUIUtils.sSyncInvolvedInstances.Remove(mVmName);
		}
		foreach (string key in BlueStacksUIUtils.DictWindows.Keys)
		{
			if (!(key != mVmName) || !BlueStacksUIUtils.DictWindows[key].mSelectedInstancesForSync.Contains(mVmName))
			{
				continue;
			}
			MainWindow mainWindow = BlueStacksUIUtils.DictWindows[key];
			mainWindow.mSelectedInstancesForSync.Remove(mVmName);
			if (mainWindow.mSelectedInstancesForSync.Count == 0)
			{
				mainWindow.mIsSynchronisationActive = false;
				mainWindow.mIsSyncMaster = false;
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(mainWindow.mVmName))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(mainWindow.mVmName);
				}
				mainWindow._TopBar.HideSyncPanel();
				mainWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			ReleaseClientGlobalLock();
			if (mMacroTimer != null)
			{
				mMacroTimer.Elapsed -= MacroTimer_Elapsed;
				mMacroTimer.Dispose();
				mPostOtsWelcomeWindow.Dispose();
			}
			mDiscordhandler?.Dispose();
			mCommonHandler?.Dispose();
			mMacroRecorderWindow?.Dispose();
			mUtils?.Dispose();
			disposedValue = true;
		}
	}

	~MainWindow()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void mFullscreenSidebarButton_Click(object sender, RoutedEventArgs e)
	{
		mSidebar.ToggleSidebarVisibilityInFullscreen(isVisible: true);
	}

	private void SidebarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		mIsSideButtonDragging = true;
		mOldSideButtonMargin = ((FrameworkElement)((sender is Button) ? sender : null)).Margin;
		mSideButtonOldPosition = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
	}

	private void SidebarButton_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Point position = e.GetPosition((IInputElement)(object)this);
		if (mIsSideButtonDragging && ((Point)(ref position)).Y > 0.0 && ((Point)(ref position)).Y < ((FrameworkElement)mFullscreenSidebarPopupButtonInnerGrid).ActualHeight)
		{
			((FrameworkElement)mFullscreenSidebarButton).Margin = new Thickness(0.0, ((Thickness)(ref mOldSideButtonMargin)).Top + 2.0 * (((Point)(ref position)).Y - ((Point)(ref mSideButtonOldPosition)).Y), 0.0, 0.0);
		}
	}

	private void SidebarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (mIsSideButtonDragging)
		{
			mIsSideButtonDragging = false;
			mSideButtonOldPosition = default(Point);
		}
	}

	private void mFullscreenTopbarButton_Click(object sender, RoutedEventArgs e)
	{
		mTopbarOptions.ToggleTopbarVisibilityInFullscreen(isVisible: true);
	}

	private void TopbarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		mIsTopButtonDragging = true;
		mOldTopButtonMargin = ((FrameworkElement)((sender is Button) ? sender : null)).Margin;
		mTopButtonOldPosition = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
	}

	private void TopbarButton_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Point position = e.GetPosition((IInputElement)(object)this);
		if (mIsTopButtonDragging && ((Point)(ref position)).X > 0.0 && ((Point)(ref position)).X < ((FrameworkElement)mFullscreenTopbarPopupButtonInnerGrid).ActualWidth)
		{
			((FrameworkElement)mFullscreenTopbarButton).Margin = new Thickness(((Thickness)(ref mOldTopButtonMargin)).Left + 2.0 * (((Point)(ref position)).X - ((Point)(ref mTopButtonOldPosition)).X), 0.0, 0.0, 0.0);
		}
	}

	private void TopbarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (mIsTopButtonDragging)
		{
			mIsTopButtonDragging = false;
			mTopButtonOldPosition = default(Point);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/mainwindow.xaml", UriKind.Relative);
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
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Expected O, but got Unknown
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Expected O, but got Unknown
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Expected O, but got Unknown
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Expected O, but got Unknown
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Expected O, but got Unknown
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Expected O, but got Unknown
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Expected O, but got Unknown
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Expected O, but got Unknown
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Expected O, but got Unknown
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Expected O, but got Unknown
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Expected O, but got Unknown
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Expected O, but got Unknown
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Expected O, but got Unknown
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Expected O, but got Unknown
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Expected O, but got Unknown
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Expected O, but got Unknown
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMainWindow = (MainWindow)target;
			((UIElement)mMainWindow).IsVisibleChanged += new DependencyPropertyChangedEventHandler(MainWindow_IsVisibleChanged);
			((Window)mMainWindow).StateChanged += MainWindow_StateChanged;
			((Window)mMainWindow).Activated += MainWindow_Activated;
			((Window)mMainWindow).Deactivated += MainWindow_Deactivated;
			((UIElement)mMainWindow).PreviewMouseMove += new MouseEventHandler(MainWindow_PreviewMouseMove);
			((Window)mMainWindow).ContentRendered += MainWindow_ContentRendered;
			((FrameworkElement)mMainWindow).Loaded += new RoutedEventHandler(MainWindow_Loaded);
			((Window)mMainWindow).Closing += MainWindow_Closing;
			((Window)mMainWindow).SourceInitialized += MainWindow_SourceInitialized;
			break;
		case 3:
			OuterBorder = (Border)target;
			break;
		case 4:
			MainGrid = (Grid)target;
			break;
		case 5:
			pikaPop = (CustomPopUp)target;
			break;
		case 6:
			pikaCanvas = (Canvas)target;
			break;
		case 7:
			pikaPopControl = (PikaNotificationControl)target;
			break;
		case 8:
			toastPopup = (CustomPopUp)target;
			break;
		case 9:
			toastCanvas = (Canvas)target;
			break;
		case 10:
			toastControl = (CustomToastPopupControl)target;
			break;
		case 11:
			mFullScreenToastPopup = (CustomPopUp)target;
			break;
		case 12:
			mFullScreenToastCanvas = (Canvas)target;
			break;
		case 13:
			mFullScreenToastControl = (FullScreenToastPopupControl)target;
			break;
		case 14:
			mGeneraltoast = (CustomPopUp)target;
			break;
		case 15:
			mGeneraltoastCanvas = (Canvas)target;
			break;
		case 16:
			mGeneraltoastControl = (CustomToastPopupControl)target;
			break;
		case 17:
			mShootingModePopup = (CustomPopUp)target;
			break;
		case 18:
			mShootingModePopupCanvas = (Canvas)target;
			break;
		case 19:
			mToastControl = (CustomPersistentToastPopupControl)target;
			break;
		case 20:
			mTopBarPopup = (CustomPopUp)target;
			((UIElement)mTopBarPopup).Visibility = (Visibility)2;
			break;
		case 21:
			mFullScreenTopBar = (FullScreenTopBar)target;
			break;
		case 22:
			mFullscreenSidebarPopupButton = (CustomPopUp)target;
			break;
		case 23:
			mFullscreenSidebarPopupButtonInnerGrid = (Grid)target;
			break;
		case 24:
			mFullscreenSidebarButton = (Button)target;
			((UIElement)mFullscreenSidebarButton).Visibility = (Visibility)2;
			((ButtonBase)mFullscreenSidebarButton).Click += new RoutedEventHandler(mFullscreenSidebarButton_Click);
			((UIElement)mFullscreenSidebarButton).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SidebarButton_PreviewMouseLeftButtonDown);
			((UIElement)mFullscreenSidebarButton).PreviewMouseMove += new MouseEventHandler(SidebarButton_MouseMove);
			((UIElement)mFullscreenSidebarButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SidebarButton_PreviewMouseLeftButtonUp);
			break;
		case 25:
			mFullscreenSidebarPopup = (CustomPopUp)target;
			break;
		case 26:
			mFullscreenSidebarPopupInnerGrid = (Grid)target;
			break;
		case 27:
			mFullscreenTopbarPopupButton = (CustomPopUp)target;
			break;
		case 28:
			mFullscreenTopbarPopupButtonInnerGrid = (Grid)target;
			break;
		case 29:
			mFullscreenTopbarButton = (Button)target;
			((UIElement)mFullscreenTopbarButton).Visibility = (Visibility)2;
			((ButtonBase)mFullscreenTopbarButton).Click += new RoutedEventHandler(mFullscreenTopbarButton_Click);
			((UIElement)mFullscreenTopbarButton).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TopbarButton_PreviewMouseLeftButtonDown);
			((UIElement)mFullscreenTopbarButton).PreviewMouseMove += new MouseEventHandler(TopbarButton_MouseMove);
			((UIElement)mFullscreenTopbarButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(TopbarButton_PreviewMouseLeftButtonUp);
			break;
		case 30:
			mFullscreenTopbarPopup = (CustomPopUp)target;
			break;
		case 31:
			mFullscreenTopbarPopupInnerGrid = (Grid)target;
			break;
		case 32:
			mTopbarOptions = (TopbarOptions)target;
			break;
		case 33:
			mMainWindowTopGrid = (Grid)target;
			break;
		case 34:
			mTopBar = (TopBar)target;
			break;
		case 35:
			mNCTopBar = (NCSoftTopBar)target;
			break;
		case 36:
			mFrontendOTSControl = (FrontendOTSControl)target;
			break;
		case 37:
			dummyPika = (Grid)target;
			break;
		case 38:
			mContentGrid = (Grid)target;
			break;
		case 39:
			WelcomeTabParentGrid = (Grid)target;
			((UIElement)WelcomeTabParentGrid).IsVisibleChanged += new DependencyPropertyChangedEventHandler(WelcomeTabParentGrid_IsVisibleChanged);
			break;
		case 40:
			mWelcomeTab = (WelcomeTab)target;
			break;
		case 41:
			FrontendParentGrid = (Grid)target;
			((UIElement)FrontendParentGrid).IsVisibleChanged += new DependencyPropertyChangedEventHandler(FrontendParentGrid_IsVisibleChanged);
			break;
		case 42:
			mDmmProgressControl = (DMMProgressControl)target;
			break;
		case 43:
			mFrontendGrid = (Grid)target;
			((UIElement)mFrontendGrid).IsVisibleChanged += new DependencyPropertyChangedEventHandler(FrontendGrid_IsVisibleChanged);
			((FrameworkElement)mFrontendGrid).SizeChanged += new SizeChangedEventHandler(FrontendGrid_SizeChanged);
			break;
		case 44:
			mDmmBottomBar = (DMMBottomBar)target;
			break;
		case 45:
			mSidebar = (Sidebar)target;
			break;
		case 46:
			dummyToast = (Grid)target;
			break;
		case 47:
			dummyTooltip = (Grid)target;
			break;
		case 48:
			mExitProgressGrid = (ProgressBar)target;
			break;
		case 49:
			mQuitPopupBrowserLoadGrid = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (connectionId == 2)
		{
			((FrameworkElement)(Grid)target).Loaded += new RoutedEventHandler(ResizeGrid_Loaded);
		}
	}

	static MainWindow()
	{
		sScalingFactor = 1.0;
		sIsClosingForBackupRestore = false;
		sShowNotifications = true;
		sMacroMapping = new Dictionary<string, string>();
	}
}
