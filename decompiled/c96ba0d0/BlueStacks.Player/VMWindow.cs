using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using SystemInfo;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

public class VMWindow : WMTouchForm
{
	internal enum InputHandlingState
	{
		IMAP_STATE_NOFOCUS = 1,
		IMAP_STATE_MAPPING,
		IMAP_STATE_TEXT,
		IMAP_STATE_RAW
	}

	public enum tagINPUT_MESSAGE_ORIGIN_ID
	{
		IMO_UNAVAILABLE = 0,
		IMO_HARDWARE = 1,
		IMO_INJECTED = 2,
		IMO_SYSTEM = 4
	}

	public enum tagINPUT_MESSAGE_DEVICE_TYPE
	{
		IMDT_UNAVAILABLE = 0,
		IMDT_KEYBOARD = 1,
		IMDT_MOUSE = 2,
		IMDT_TOUCH = 4,
		IMDT_PEN = 8
	}

	public struct tagINPUT_MESSAGE_SOURCE
	{
		public tagINPUT_MESSAGE_DEVICE_TYPE deviceType;

		public tagINPUT_MESSAGE_ORIGIN_ID originId;
	}

	public struct CursorCoordinate
	{
		public float XPerc;

		public float YPerc;
	}

	internal class CustomTextBox : TextBox
	{
		public CustomTextBox()
		{
			((Control)this).SetStyle((ControlStyles)141314, true);
			((Control)this).BackColor = Color.Transparent;
			((Control)this).ForeColor = Color.Transparent;
			InteropWindow.SendMessage(((Control)this).Handle, 11, false, 0);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			((TextBox)this).OnGotFocus(e);
			InteropWindow.HideCaret(((Control)this).Handle);
			InteropWindow.SendMessage(((Control)this).Handle, 11, false, 0);
			InteropWindow.SetFocus(((Control)this).Handle);
		}

		internal void Focus()
		{
			InteropWindow.SetFocus(((Control)this).Handle);
		}
	}

	internal class CustomElementHost : ElementHost
	{
		protected override void OnGotFocus(EventArgs e)
		{
			((ElementHost)this).OnGotFocus(e);
			InteropWindow.SendMessage(((Control)this).Handle, 11, false, 0);
		}

		internal void Focus()
		{
			InteropWindow.SetFocus(((Control)this).Handle);
		}
	}

	internal static VMWindow Instance;

	internal Toast snapshotErrorToast;

	internal bool snapshotErrorShown;

	internal IntPtr m_hImc;

	internal FullScreenToast mFullScreenToast;

	internal FormBorderStyle mFormBorderStyle;

	internal ShortcutConfig mShortcutConfig;

	internal bool IsShownOnce;

	internal Dictionary<string, string> AppOrientationDict = new Dictionary<string, string>();

	internal bool mLastCallFromAndroid;

	internal string mLlastPackageName = "";

	internal int mLlastOrientationFromAndroid;

	private bool mIsFullscreen;

	private int mVMWindowMouseX;

	internal Dictionary<string, string> mMacroShortcutsDict = new Dictionary<string, string>();

	internal CSysInfo cSysInfo;

	internal bool mIsKBVibrationDllLoaded;

	public bool mIsInImagePickerMode;

	internal List<string> mCustomCursorAppsList = new List<string>();

	internal bool mIsCustomCursorEnabled;

	private bool mIsChineseSimplifiedLangSelected;

	public bool mIsInScriptMode;

	private InputHandlingState mInputHandlingState = InputHandlingState.IMAP_STATE_NOFOCUS;

	private bool mSendBootCheckStats = true;

	private long lastLWinTimestamp;

	internal static int sLastTouchTime;

	private const int SPI_GETMOUSESPEED = 112;

	private const int SPI_SETMOUSESPEED = 113;

	internal ToolTip sKeyMapToolTip = new ToolTip
	{
		ForeColor = Color.White,
		BackColor = Color.Black
	};

	private string mInstallDir = string.Empty;

	private static Dictionary<Keys, int> sKeyStateSet;

	private Monitor.TouchPoint[] touchPoints = new Monitor.TouchPoint[16];

	internal DateTime mFrontendLaunchTime;

	private Thread displayTimeOutThread;

	internal bool s_KeyMapTeachMode;

	internal static bool isUsePcImeWorkflow;

	internal bool mIsTextInputBoxInFocus;

	private bool mWasImeEnabled;

	internal CustomTextBox mDummyInputKeyBoard = new CustomTextBox();

	internal static bool sIsWpfTextboxEnabled;

	internal CustomElementHost mCtrlHost = new CustomElementHost();

	internal WpfTextBoxControl mTextBoxControl = new WpfTextBoxControl();

	internal bool isStreamingModeEnabled;

	private double widthRatio = 16.0;

	private double heightRatio = 9.0;

	public double widthDiff = 26.0;

	public double heightDiff = 71.0;

	private DateTime mLastFrontendStatusUpdateTime;

	private Keys mLastSentKeyToClient;

	private const int WM_SIZING = 532;

	private const int WMSZ_LEFT = 1;

	private const int WMSZ_RIGHT = 2;

	private const int WMSZ_TOP = 3;

	private const int WMSZ_BOTTOM = 6;

	internal string mLastKeyBoardString = string.Empty;

	private bool isKeyDown;

	private string lastCompositeString = string.Empty;

	internal Dictionary<int, string> mControllerMap = new Dictionary<int, string>();

	private SerialWorkQueue mSerialQueue;

	private bool mIsSideBarVisible;

	private bool mIsTopBarVisible;

	internal static bool isLogWndProc;

	private IContainer components;

	public bool IsFullscreen
	{
		get
		{
			return mIsFullscreen;
		}
		set
		{
			mIsFullscreen = value;
			FullScreenStateChanged(mIsFullscreen);
		}
	}

	internal InputHandlingState InputMapperHandlingState
	{
		get
		{
			return mInputHandlingState;
		}
		set
		{
			mInputHandlingState = value;
			Logger.Debug("KMP " + value);
			if ((!OperationsSyncManager.mIsBroadcasting && !InputMapper.IsMacroPlaying && !OperationsSyncManager.mIsReceiving) || value != InputHandlingState.IMAP_STATE_NOFOCUS)
			{
				InputMapper.SetInputMapperState((int)value);
			}
		}
	}

	internal string InstallDir
	{
		get
		{
			if (string.IsNullOrEmpty(mInstallDir))
			{
				mInstallDir = RegistryStrings.InstallDir;
			}
			return mInstallDir;
		}
	}

	internal bool WasImeEnabled
	{
		get
		{
			return mWasImeEnabled;
		}
		set
		{
			Logger.Debug("KMP WasImeEnabled " + value);
			mWasImeEnabled = value;
		}
	}

	public SerialWorkQueue SerialQueue
	{
		get
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected Obj, but got Unknown
			if (mSerialQueue == null)
			{
				mSerialQueue = new SerialWorkQueue();
				mSerialQueue.Start();
			}
			return mSerialQueue;
		}
	}

	private void FullScreenStateChanged(bool mIsFullscreen)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		try
		{
			UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
			{
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Expected Obj, but got Unknown
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected Obj, but got Unknown
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Expected Obj, but got Unknown
				if (mIsFullscreen)
				{
					if (RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].ShowSidebarInFullScreen)
					{
						((Control)this).MouseMove -= VMWindow_MouseMove;
						((Control)this).MouseMove += VMWindow_MouseMove;
					}
					InputMapper.EnableScrollOnEdgeFeature(enable: true);
				}
				else
				{
					((Control)this).MouseMove -= VMWindow_MouseMove;
					InputMapper.EnableScrollOnEdgeFeature(enable: false);
				}
			}));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in FullScreenStateChanged: " + ex.ToString());
		}
	}

	internal void ToggleScrollOnEdgeState(bool isEnable)
	{
		if (isEnable || mIsFullscreen)
		{
			InputMapper.EnableScrollOnEdgeFeature(enable: true);
		}
		else
		{
			InputMapper.EnableScrollOnEdgeFeature(enable: false);
		}
	}

	internal void ClientScriptModeStateChanged(bool isInScriptMode)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected Obj, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected Obj, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected Obj, but got Unknown
			mIsInScriptMode = isInScriptMode;
			if (isInScriptMode)
			{
				((Control)this).MouseUp -= VMWindow_MouseUp;
				((Control)this).MouseLeave -= VMWindow_MouseLeave;
				((Control)this).MouseUp += VMWindow_MouseUp;
				((Control)this).MouseLeave += VMWindow_MouseLeave;
			}
			else
			{
				((Control)this).MouseUp -= VMWindow_MouseUp;
				((Control)this).MouseLeave -= VMWindow_MouseLeave;
			}
		}));
	}

	internal void ImagePickerModeStateChanged(bool isInImagePickerMode)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected Obj, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected Obj, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected Obj, but got Unknown
			mIsInImagePickerMode = isInImagePickerMode;
			if (isInImagePickerMode)
			{
				((Control)this).MouseUp -= VMWindow_MouseUp;
				((Control)this).MouseUp += VMWindow_MouseUp;
			}
			else
			{
				((Control)this).MouseUp -= VMWindow_MouseUp;
			}
		}));
	}

	private void VMWindow_MouseLeave(object sender, EventArgs e)
	{
		sKeyMapToolTip.Hide((IWin32Window)(object)Instance);
	}

	private void VMWindow_MouseUp(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		try
		{
			if ((int)e.Button == 2097152)
			{
				InputMapper.Instance.GetMousePercentLocationFromPointToClient(out var x, out var y);
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					["X"] = x.ToString(),
					["Y"] = y.ToString()
				};
				if (mIsInImagePickerMode)
				{
					HTTPUtils.SendRequestToEngineAsync("sendImagePickerCoordinates", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0);
					mIsInImagePickerMode = false;
					ImagePickerModeStateChanged(mIsInImagePickerMode);
				}
				else
				{
					HTTPUtils.SendRequestToClientAsync("playerScriptModifierClick", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in VMWindow_MouseUp: " + ex.ToString());
		}
	}

	public VMWindow()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected Obj, but got Unknown
		Instance = this;
		InitializeComponent();
		((Control)this).Click += VMWindow_Click;
	}

	private void VMWindow_Click(object sender, EventArgs e)
	{
		if (IsFullscreen)
		{
			HTTPUtils.SendRequestToClientAsync("hideTopSidebar", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
	}

	private void KeyMapToolTip_Draw(object sender, DrawToolTipEventArgs e)
	{
		e.DrawBackground();
		e.DrawBorder();
		e.DrawText();
	}

	public VMWindow(bool hidden, bool useWpfTextbox)
		: this()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected Obj, but got Unknown
		((Form)this).Icon = Utils.GetApplicationIcon();
		((Control)this).Text = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
		((Form)this).ClientSize = LayoutManager.GetConfiguredDisplaySize();
		m_hImc = InteropWindow.ImmGetContext(((Control)this).Handle);
		if (Oem.Instance.IsFormBorderStyleFixedSingle)
		{
			((Form)this).FormBorderStyle = (FormBorderStyle)1;
		}
		if ((int)((Form)this).FormBorderStyle != 0)
		{
			((Form)this).MaximizeBox = false;
		}
		mFormBorderStyle = ((Form)this).FormBorderStyle;
		LocaleStrings.InitLocalization((string)null, MultiInstanceStrings.VmName, false);
		ServicePointManager.DefaultConnectionLimit = 10;
		Strings.AppTitle = Oem.Instance.CommonAppTitleText;
		mFullScreenToast = new FullScreenToast((Control)(object)this);
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValidateRemoteCertificate));
			if (RegistryManager.Instance.SystemStats == 0)
			{
				Stats.SendSystemInfoStats(MultiInstanceStrings.VmName);
			}
		}
		catch (Exception ex)
		{
			Logger.Fatal("Exception while setting up VMWindow. Exiting");
			Logger.Fatal(ex.ToString());
			Environment.Exit(-4);
		}
		mFrontendLaunchTime = DateTime.Now;
		mLastFrontendStatusUpdateTime = DateTime.Now;
		Input.DisablePressAndHold(((Control)this).Handle);
		try
		{
			Input.HookKeyboard(HandleKeyboardHook);
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception while keyboard hook. Exception: " + ex2.ToString());
		}
		StartAgent();
		AddDummyTextBox(useWpfTextbox);
		MemoryManager.CheckAndTrimAndroidMemory();
		LayoutManager.InitScreen();
		InputMapper.Instance.SetupSoftControlBar();
		SensorDevice.Instance.StartThreads();
		GpsHelper.Start();
		SystemEvents.DisplaySettingsChanged += HandleDisplaySettingsChanged;
		MemoryManager.TrimMemory(false);
		ThreadPool.QueueUserWorkItem((object obj) =>
		{
			PrintingGraphicsInfo();
		});
		if (!hidden)
		{
			((Control)this).Show();
		}
		Logger.Info("BOOT_STAGE: Starting Android boot now");
		AndroidBootUp.Start();
		mShortcutConfig = ShortcutConfig.LoadShortcutsConfig();
		sKeyMapToolTip.Draw += KeyMapToolTip_Draw;
		if (SystemUtils.IsAdministrator() && Oem.Instance.OEM.Equals("msi2", StringComparison.OrdinalIgnoreCase))
		{
			mIsKBVibrationDllLoaded = MsiVibration.Init();
			if (mIsKBVibrationDllLoaded)
			{
				Logger.Info("Msi vibration dll loaded successfully");
			}
			else
			{
				Logger.Info("Failed to load msi vibration dll");
			}
		}
	}

	internal bool CheckBlackScreen()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		try
		{
			Logger.Debug("Inside CheckBlackScreen");
			Size size = new Size((int)((double)((Form)this).ClientSize.Width * 0.993), (int)((double)((Form)this).ClientSize.Height * 0.957));
			double num = 0.0035;
			double num2 = 0.0425;
			Bitmap val = new Bitmap(((Form)Instance).ClientSize.Width, ((Form)this).ClientSize.Height);
			Graphics val2 = Graphics.FromImage((Image)(object)val);
			try
			{
				val2.CopyFromScreen(new Point((int)((double)((Control)this).Left + (double)((Control)this).Width * num), (int)((double)((Control)this).Top + (double)((Control)this).Height * num2)), Point.Empty, size);
				for (int i = 0; i < size.Width; i++)
				{
					for (int j = 0; j < size.Height; j++)
					{
						Color pixel = val.GetPixel(i, j);
						if (pixel.A != Color.Black.A || pixel.R != Color.Black.R || pixel.G != Color.Black.G || pixel.B != Color.Black.B)
						{
							Logger.Info($"Pixel {i},{j} is not black");
							((Image)val).Dispose();
							return false;
						}
					}
				}
				((Image)val).Dispose();
				Logger.Error("Black Screen Detected");
				return true;
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception occured in CheckBlackScreen. Err : {0}", new object[1] { ex.ToString() });
			return false;
		}
	}

	private void PrintingGraphicsInfo()
	{
		Logger.Info("In PrintingGraphicsInfo");
		try
		{
			Dictionary<string, string> dictionary = Profile.InfoForGraphicsDriverCheck();
			dictionary.Add("guid", RegistryManager.Instance.UserGuid);
			Logger.Info("data being posted: ");
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				Logger.Info("Key: " + item.Key + " Value: " + item.Value);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in print graphics info... Err : " + ex.ToString());
		}
	}

	internal void BootUpTasks()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected Obj, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected Obj, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected Obj, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected Obj, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected Obj, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected Obj, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected Obj, but got Unknown
			((Control)this).OnLayout(new LayoutEventArgs((Control)(object)this, ""));
			displayTimeOutThread = new Thread(() =>
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected Obj, but got Unknown
				while (true)
				{
					UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
					{
						HandleDisplayTimeout();
					}));
				}
			})
			{
				IsBackground = true
			};
			displayTimeOutThread.Start();
			((Control)this).MouseMove += InputMapper.Instance.HandleMouseMove;
			((Control)this).MouseDown += InputMapper.Instance.HandleMouseDown;
			((Control)this).MouseUp += InputMapper.Instance.HandleMouseUp;
			((Control)this).MouseWheel += InputMapper.Instance.HandleMouseWheel;
			TouchEvent += HandleTouchEvent;
			((Control)this).KeyDown += HandleKeyDown;
			((Control)this).KeyUp += HandleKeyUp;
			SendLockedKeys();
		}));
	}

	private void HandleTouchEvent(object sender, WMTouchEventArgs e)
	{
		Logger.Info("In HandleTouchEvent in VMWindow");
		sLastTouchTime = Environment.TickCount;
		InputMapper.Instance.HandleTouchEvent(sender, e);
	}

	private void HandleDisplayTimeout()
	{
		if (Opengl.glWindowAction == GlWindowAction.Show)
		{
			Logger.Info("Showing subwindow");
			if (Opengl.ShowSubWindow())
			{
				Opengl.glWindowAction = GlWindowAction.None;
			}
		}
		else if (Opengl.glWindowAction == GlWindowAction.Hide)
		{
			Logger.Info("Hiding subwindow");
			if (Opengl.HideSubWindow())
			{
				Opengl.glWindowAction = GlWindowAction.None;
			}
		}
	}

	private void SendLockedKeys()
	{
		Thread thread = new Thread(() =>
		{
			Logger.Info("in sendLockedKeys -- mCapsLocked - {0} and mNumLocked - {1}", new object[2]
			{
				Control.IsKeyLocked((Keys)20),
				Control.IsKeyLocked((Keys)144)
			});
			Logger.Info("in sendLockedKeys - guest booted");
			if (Control.IsKeyLocked((Keys)20))
			{
				HandleKeyEvent((Keys)20, pressed: true);
				Logger.Info("in sendLockedKeys - sleeping for 100 ms for capslock toggle");
				Thread.Sleep(100);
				HandleKeyEvent((Keys)20, pressed: false);
			}
			if (Control.IsKeyLocked((Keys)144))
			{
				HandleKeyEvent((Keys)144, pressed: true);
				Logger.Info("in sendLockedKeys - sleeping for 100 ms for numLock toggle");
				Thread.Sleep(100);
				HandleKeyEvent((Keys)144, pressed: false);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void HandleDisplaySettingsChanged(object sender, EventArgs e)
	{
		Logger.Info("HandleDisplaySettingsChanged()");
		SendOrientationToGuest();
	}

	internal void SendOrientationToGuest()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Sending screen orientation to guest: {0}", new object[1] { SystemInformation.ScreenOrientation });
		Dictionary<string, string> dictionary = new Dictionary<string, string> { 
		{
			"data",
			((object)SystemInformation.ScreenOrientation/*cast due to constrained. prefix*/).ToString()
		} };
		HTTPUtils.SendRequestToGuestAsync("hostOrientation", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0);
	}

	public void UpdateUserActivityStatus()
	{
		try
		{
			if (TimeSpan.Compare(DateTime.Now.Subtract(mLastFrontendStatusUpdateTime), new TimeSpan(0, 0, 10)) > 0)
			{
				Stats.SendFrontendStatusUpdate("user-active", MultiInstanceStrings.VmName);
				mLastFrontendStatusUpdateTime = DateTime.Now;
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Error Occured, Err : {ex.ToString()}");
		}
	}

	private void AddDummyTextBox(bool useWpfTextbox)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected Obj, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected Obj, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected Obj, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected Obj, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected Obj, but got Unknown
		if (useWpfTextbox)
		{
			((Control)mCtrlHost).Dock = (DockStyle)0;
			((Control)mCtrlHost).Location = new Point(-500, -500);
			((Control)mCtrlHost).Size = new Size(300, 300);
			((Control)this).Controls.Add((Control)(object)mCtrlHost);
			((ElementHost)mCtrlHost).Child = (UIElement)(object)mTextBoxControl;
			((UIElement)mTextBoxControl.mWpfTextBox).PreviewKeyDown += (object sender, KeyEventArgs e) =>
			{
				DummyInputKeyBoard_KeyDown(sender, e.ToWinforms());
			};
			((UIElement)mTextBoxControl.mWpfTextBox).PreviewKeyUp += (object sender, KeyEventArgs e) =>
			{
				DummyInputKeyBoard_KeyUp(sender, e.ToWinforms());
			};
			((TextBoxBase)mTextBoxControl.mWpfTextBox).TextChanged += DummyInputKeyBoard_TextChanged;
			sIsWpfTextboxEnabled = true;
		}
		else
		{
			((TextBoxBase)mDummyInputKeyBoard).Multiline = true;
			((Control)mDummyInputKeyBoard).TabStop = false;
			((TextBoxBase)mDummyInputKeyBoard).AcceptsTab = true;
			((Control)mDummyInputKeyBoard).Size = new Size(150, 100);
			((Control)mDummyInputKeyBoard).Location = new Point(-500, -500);
			((Control)mDummyInputKeyBoard).KeyDown += DummyInputKeyBoard_KeyDown;
			((Control)mDummyInputKeyBoard).KeyUp += DummyInputKeyBoard_KeyUp;
			((Control)this).Controls.Add((Control)(object)mDummyInputKeyBoard);
		}
	}

	private void DummyInputKeyBoard_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Invalid comparison between Unknown and I4
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and I4
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Invalid comparison between Unknown and I4
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Invalid comparison between Unknown and I4
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Invalid comparison between Unknown and I4
		isKeyDown = true;
		int length = ((Control)mDummyInputKeyBoard).Text.Length;
		int selectionLength = ((TextBoxBase)mDummyInputKeyBoard).SelectionLength;
		int selectionStart = ((TextBoxBase)mDummyInputKeyBoard).SelectionStart;
		if (sIsWpfTextboxEnabled)
		{
			length = mTextBoxControl.mWpfTextBox.Text.Length;
			selectionLength = mTextBoxControl.mWpfTextBox.SelectionLength;
			selectionStart = mTextBoxControl.mWpfTextBox.SelectionStart;
		}
		if ((int)e.KeyData == 131158 && isUsePcImeWorkflow)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
		}
		Keys keyData = e.KeyData;
		if ((int)keyData == 9 || (int)keyData == 65545 || (int)keyData == 65545)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
		}
		if (mIsTextInputBoxInFocus && isUsePcImeWorkflow)
		{
			if ((int)keyData == 8 && length != 0 && selectionStart == length && selectionLength == 0 && mIsTextInputBoxInFocus)
			{
				return;
			}
			if ((int)keyData == 13 || (int)keyData == 65549 || (int)keyData == 131085)
			{
				if (mIsChineseSimplifiedLangSelected)
				{
					CleanUpTextBox();
				}
				return;
			}
			if ((int)keyData == 8)
			{
				CleanUpTextBox();
			}
			else if (selectionStart != length && (selectionLength != 1 || selectionStart != length - 1))
			{
				CleanUpTextBox();
			}
		}
		HandleKeyDown(null, e);
	}

	private void DummyInputKeyBoard_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		if (isKeyDown && !mIsChineseSimplifiedLangSelected && ((Control)mDummyInputKeyBoard).Text.Equals(mLastKeyBoardString))
		{
			CheckForImeString(mLastKeyBoardString);
		}
		if ((int)e.KeyData != 131158 || !isUsePcImeWorkflow)
		{
			HandleKeyUp(null, e);
		}
	}

	private void CleanUpTextBox()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected Obj, but got Unknown
		if (sIsWpfTextboxEnabled)
		{
			((TextBoxBase)mTextBoxControl.mWpfTextBox).TextChanged -= DummyInputKeyBoard_TextChanged;
			mTextBoxControl.mWpfTextBox.Text = string.Empty;
			((TextBoxBase)mTextBoxControl.mWpfTextBox).TextChanged += DummyInputKeyBoard_TextChanged;
		}
		else
		{
			((Control)mDummyInputKeyBoard).TextChanged -= DummyInputKeyBoard_TextChanged;
			((Control)mDummyInputKeyBoard).Text = string.Empty;
			((Control)mDummyInputKeyBoard).TextChanged += DummyInputKeyBoard_TextChanged;
		}
		mLastKeyBoardString = string.Empty;
	}

	private void DummyInputKeyBoard_TextChanged(object sender, EventArgs e)
	{
		isKeyDown = false;
		string text = mLastKeyBoardString;
		string text2 = "";
		text2 = ((!sIsWpfTextboxEnabled) ? ((Control)mDummyInputKeyBoard).Text : mTextBoxControl.mWpfTextBox.Text);
		int length = text.Length;
		if (text.Length > text2.Length)
		{
			length = text2.Length;
		}
		int i;
		for (i = 0; i < length && text[i] == text2[i]; i++)
		{
		}
		string text3 = text2.Substring(i);
		if (i == length && text2.Length > text.Length)
		{
			if (text3 == Environment.NewLine)
			{
				SendStringToAndroid(string.Empty, 0, 1);
			}
			else if (string.IsNullOrEmpty(text))
			{
				SendStringToAndroid(text2, 0);
			}
			else
			{
				SendStringToAndroid(text2.Substring(length), 0);
			}
		}
		else
		{
			int num = text.Length - i;
			if (text.Substring(i) == Environment.NewLine)
			{
				num = 1;
			}
			if (num > 0 || !string.IsNullOrEmpty(text3))
			{
				SendStringToAndroid(text3, num);
			}
		}
		mLastKeyBoardString = text2;
	}

	private void SendStringToAndroid(string c, int passCharWithBackSpace, int passNewLine = 0)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		if (!mIsTextInputBoxInFocus || !isUsePcImeWorkflow)
		{
			return;
		}
		try
		{
			string text = "";
			text = "start_" + c + "_end";
			string formattedString = text + " del=" + (passCharWithBackSpace + lastCompositeString.Length) + " enter=" + passNewLine;
			lastCompositeString = string.Empty;
			Logger.Debug("the inputcharstring is through textbox");
			SerialQueue.Enqueue((Work)(() =>
			{
				try
				{
					InputMapper.Instance.SendAndroidString(formattedString);
				}
				catch
				{
				}
			}));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception when sending char to android. Err : {0}", new object[1] { ex.ToString() });
		}
	}

	private void CheckForImeString(string str = "")
	{
		string text = InteropWindow.CurrentCompStr(((Control)mDummyInputKeyBoard).Handle);
		if (!string.IsNullOrEmpty(text) && !str.Equals(text))
		{
			SendStringToAndroid(text, 0);
			lastCompositeString = text;
		}
	}

	public void HandleKeyDown(object obj, KeyEventArgs evt)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Invalid comparison between Unknown and I4
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Invalid comparison between Unknown and I4
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Invalid comparison between Unknown and I4
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Invalid comparison between Unknown and I4
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Invalid comparison between Unknown and I4
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Invalid comparison between Unknown and I4
		bool flag = false;
		if (Oem.Instance.IsOEMWithBGPClient)
		{
			flag = HandleClientHotKeys(evt);
		}
		UpdateUserActivityStatus();
		if (flag)
		{
			evt.Handled = true;
			return;
		}
		MacroForm.RecordKeys(evt.KeyCode, (ActionType)0);
		if (evt.Alt && evt.Control)
		{
			if (mMacroShortcutsDict.ContainsKey(((object)evt.KeyCode/*cast due to constrained. prefix*/).ToString()))
			{
				Logger.Debug("MACRO: Shortcut pressed: " + ((object)evt.KeyCode/*cast due to constrained. prefix*/).ToString());
				return;
			}
			if ((int)evt.KeyCode == 86 || (int)evt.KeyCode == 79 || (int)evt.KeyCode == 71 || (int)evt.KeyCode == 84 || (int)evt.KeyCode == 70)
			{
				Opengl.HandleCommand((int)Keyboard.Instance.NativeToScanCodes(evt.KeyCode));
			}
			else if ((int)evt.KeyCode == 75)
			{
				s_KeyMapTeachMode = !s_KeyMapTeachMode;
			}
			else
			{
				if ((int)evt.KeyCode == 73 && Features.IsFeatureEnabled(16777216uL))
				{
					InputMapper.Instance.ShowConfigDialog();
					return;
				}
				if ((int)evt.KeyCode == 77 && Features.IsFeatureEnabled(4294967296uL))
				{
					InputMapper.Instance.LaunchBlueStacksKeyMapper();
					return;
				}
			}
		}
		if (IgnoreKey(evt) || !LockdownIsKeyAllowed(evt.KeyCode))
		{
			return;
		}
		if (AppHandler.mCurrentAppPackage == null)
		{
			Logger.Info("current app package name is null");
		}
		if ((int)evt.KeyCode == 122 && Features.IsFullScreenToggleEnabled() && !Oem.Instance.IsOEMWithBGPClient)
		{
			LayoutManager.ToggleFullScreen();
		}
		if ((int)evt.KeyCode == 27)
		{
			if (AppHandler.mCurrentAppPackage.Equals("com.uncube.account"))
			{
				return;
			}
			if (IsFullscreen)
			{
				if (RegistryManager.Instance.UseEscapeToExitFullScreen)
				{
					SendHotKeyEventToClient("RestoreWindow");
				}
				else
				{
					ThreadPool.QueueUserWorkItem((object obj2) =>
					{
						try
						{
							VmCmdHandler.RunCommand("back", MultiInstanceStrings.VmName);
						}
						catch (Exception ex)
						{
							Logger.Error("Exception when sending back to android. Exception: " + ex);
						}
					});
				}
			}
		}
		HandleKeyEvent(evt.KeyCode, pressed: true);
		if ((int)evt.KeyCode == 20)
		{
			Logger.Info("caps lock pressed while in frontend");
		}
		if ((int)evt.KeyCode == 144)
		{
			Logger.Info("numlock pressed while in frontend");
		}
	}

	private bool HandleClientHotKeys(KeyEventArgs evt)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected I4, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (mLastSentKeyToClient == evt.KeyCode)
		{
			return false;
		}
		mLastSentKeyToClient = evt.KeyCode;
		string text = string.Empty;
		if ((int)evt.KeyCode != 0)
		{
			Key val = KeyInterop.KeyFromVirtualKey((int)evt.KeyCode);
			if (evt.Control)
			{
				text = IMAPKeys.GetStringForFile((Key)118) + " + ";
			}
			if (evt.Alt)
			{
				text = text + IMAPKeys.GetStringForFile((Key)120) + " + ";
			}
			if (evt.Shift)
			{
				text = text + IMAPKeys.GetStringForFile((Key)116) + " + ";
			}
			text += IMAPKeys.GetStringForFile(val);
		}
		Logger.Debug("SHORTCUT: KeyPressed.." + text);
		if (mShortcutConfig != null)
		{
			foreach (ShortcutKeys item in mShortcutConfig.Shortcut)
			{
				if (!item.ShortcutKey.Equals(text))
				{
					continue;
				}
				result = true;
				SendHotKeyEventToClient(item.ShortcutName);
				if (item.ShortcutName.Equals("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP"))
				{
					if (isStreamingModeEnabled)
					{
						LayoutManager.ToggleFullScreen();
					}
					else
					{
						IsFullscreen = !IsFullscreen;
					}
				}
				Logger.Debug("SHORTCUT: Shortcut Name.." + item.ShortcutName);
			}
		}
		return result;
	}

	private void SendHotKeyEventToClient(string clientShortcut)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string> { 
		{
			"keyevent",
			clientShortcut.ToString()
		} };
		HTTPUtils.SendRequestToClientAsync("hotKeyEvents", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void HandleKeyUp(object obj, KeyEventArgs evt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((int)evt.KeyCode != 44 && (int)evt.KeyCode != 79) || !Oem.Instance.IsOEMWithBGPClient || !HandleClientHotKeys(evt))
		{
			mLastSentKeyToClient = (Keys)0;
			UpdateUserActivityStatus();
			MacroForm.RecordKeys(evt.KeyCode, (ActionType)1);
			if (!IgnoreKey(evt))
			{
				HandleKeyEvent(evt.KeyCode, pressed: false);
			}
		}
	}

	private bool LockdownIsKeyAllowed(Keys key)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		if (!Keyboard.Instance.IsAltDepressed())
		{
			return true;
		}
		if ((int)key == 37 || (int)key == 39 || (int)key == 112 || (int)key == 113 || (int)key == 114 || (int)key == 115)
		{
			return false;
		}
		return true;
	}

	private bool IgnoreKey(KeyEventArgs evt)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		bool result = false;
		if ((int)evt.KeyCode == 174 || (int)evt.KeyCode == 175 || (int)evt.KeyCode == 173)
		{
			result = true;
		}
		return result;
	}

	private bool IsPrintingKey(Keys key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		if ((int)key >= 65 && (int)key <= 90)
		{
			return true;
		}
		if ((int)key >= 48 && (int)key <= 57)
		{
			return true;
		}
		if ((int)key >= 96 && (int)key <= 105)
		{
			return true;
		}
		if ((int)key >= 186 && (int)key <= 192)
		{
			return true;
		}
		if ((int)key >= 219 && (int)key <= 222)
		{
			return true;
		}
		if ((int)key >= 106 && (int)key <= 107)
		{
			return true;
		}
		if ((int)key >= 109 && (int)key <= 111)
		{
			return true;
		}
		if ((int)key == 32)
		{
			return true;
		}
		return false;
	}

	public void HandleKeyEvent(Keys key, bool pressed)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected I4, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		if (AndroidBootUp.isAndroidBooted && (InputMapperHandlingState != InputHandlingState.IMAP_STATE_NOFOCUS || ((int)key != 160 && (int)key != 20 && (int)key != 144)) && (!InputMapper.IsMapableKey(Keyboard.Instance.NativeToScanCodes(key)) || UpdateKeyState(key, pressed)))
		{
			InputMapper.Instance.DispatchKeyboardEvent((uint)(key & 0xFFFF), pressed);
		}
	}

	private bool UpdateKeyState(Keys key, bool pressed)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		if (pressed && !sKeyStateSet.ContainsKey(key))
		{
			sKeyStateSet.Add(key, 1);
		}
		else if (!pressed && sKeyStateSet.ContainsKey(key))
		{
			sKeyStateSet.Remove(key);
		}
		else if (!pressed && !sKeyStateSet.ContainsKey(key))
		{
			result = false;
		}
		return result;
	}

	private void VMWindow_Resize(object sender, EventArgs e)
	{
		LayoutManager.FixupGuestDisplay();
	}

	private void StartAgent()
	{
		Logger.Info("Launching agent");
		Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-Agent.exe"));
	}

	private bool HandleKeyboardHook(bool pressed, uint key)
	{
		try
		{
			if (!((Control)this).Focused)
			{
				switch (key)
				{
				case 20u:
					HandleKeyEvent((Keys)20, pressed);
					return true;
				case 144u:
					HandleKeyEvent((Keys)144, pressed);
					return true;
				case 160u:
					HandleKeyEvent((Keys)160, pressed);
					return true;
				}
			}
			if (!AndroidBootUp.isAndroidBooted || !((Control)this).Focused)
			{
				return true;
			}
			if (RegistryManager.Instance.DefaultGuest.GrabKeyboard != 0 && (key == 91 || key == 92))
			{
				lastLWinTimestamp = DateTime.Now.Ticks;
				HandleKeyEvent((Keys)91, pressed);
				return false;
			}
			switch (key)
			{
			case 68u:
				if (DateTime.Now.Ticks - lastLWinTimestamp < 1000000)
				{
					return false;
				}
				return true;
			case 166u:
				HandleKeyEvent((Keys)27, pressed);
				return false;
			case 172u:
				HandleKeyEvent((Keys)119, pressed);
				return false;
			case 255u:
				HandleKeyEvent((Keys)93, pressed);
				return false;
			default:
				return true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleKeyboardHook. Exception: " + ex.ToString());
		}
		return true;
	}

	internal static bool CheckAndroidFilesIntegrity()
	{
		Logger.Info("BOOT_STAGE: Inside CheckAndroidFilesIntegrity check");
		string bstAndroidDir = RegistryStrings.GetBstAndroidDir(MultiInstanceStrings.VmName);
		string bstManagerDir = RegistryStrings.BstManagerDir;
		string text = Path.Combine(bstAndroidDir, "Root.vdi");
		string text2 = Path.Combine(bstAndroidDir, "Fastboot.vdi");
		string text3 = Path.Combine(bstAndroidDir, "Data.vdi");
		string text4 = Path.Combine(bstAndroidDir, $"{MultiInstanceStrings.VmName}.bstk");
		string text5 = Path.Combine(bstAndroidDir, $"{MultiInstanceStrings.VmName}.bstk-prev");
		string text6 = Path.Combine(bstManagerDir, "BstkGlobal.xml");
		try
		{
			if (MultiInstanceStrings.VmName == "Android" && (Utils.IsFileNullOrMissing(text) || Utils.IsFileNullOrMissing(text2) || Utils.IsFileNullOrMissing(text3) || Utils.IsFileNullOrMissing(text6) || (Utils.IsFileNullOrMissing(text4) && Utils.IsFileNullOrMissing(text5))))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in checking AndroidFilesIntegrity. Err : " + ex.ToString());
		}
		return true;
	}

	private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
	{
		return true;
	}

	private void VMWindow_Activated(object sender, EventArgs e)
	{
		if (!Oem.Instance.IsOEMWithBGPClient || isStreamingModeEnabled)
		{
			Logger.Debug("KMP VMWindow_Activated HandleFrontendActivated");
			HandleFrontendActivated();
		}
	}

	private void VMWindow_Deactivate(object sender, EventArgs e)
	{
		if (!Oem.Instance.IsOEMWithBGPClient || isStreamingModeEnabled)
		{
			Logger.Debug("KMP VMWindow_Deactivate HandleFrontendDeactivated");
			HandleFrontendDeactivated();
		}
	}

	private void VMWindow_GotFocus(object sender, EventArgs e)
	{
		if (!Oem.Instance.IsOEMWithBGPClient || isStreamingModeEnabled)
		{
			Logger.Debug("KMP VMWindow_GotFocus HandleFrontendActivated");
			HandleFrontendActivated();
		}
	}

	private void VMWindow_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (isStreamingModeEnabled)
		{
			((CancelEventArgs)(object)e).Cancel = true;
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "state", "false" } };
			HTTPUtils.SendRequestToClientAsync("toggleStreamingMode", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			return;
		}
		if (mIsKBVibrationDllLoaded)
		{
			MsiVibration.Release();
		}
		Stats.SendFrontendStatusUpdate("frontend-closed", MultiInstanceStrings.VmName);
		CloseWindow(e);
	}

	private void VMWindow_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
	{
		FrontendInputLanguageChanged(e.InputLanguage.Culture.Name);
	}

	private void FrontendInputLanguageChanged(string inputLang)
	{
		Logger.Info("The inputlanguage changed to : " + inputLang);
		if (InputLanguageManager.Current.CurrentInputLanguage.KeyboardLayoutId == 2052)
		{
			mIsChineseSimplifiedLangSelected = true;
		}
		else
		{
			mIsChineseSimplifiedLangSelected = false;
		}
		if (WasImeEnabled)
		{
			if (inputLang.Equals("zh-tw", StringComparison.InvariantCultureIgnoreCase))
			{
				((Control)mDummyInputKeyBoard).Location = ((Control)this).PointToClient(new Point(-500, -500));
				((Control)mDummyInputKeyBoard).Enabled = true;
			}
			else
			{
				((Control)mDummyInputKeyBoard).Location = ((Control)this).PointToClient(new Point(Cursor.Position.X + 20, Cursor.Position.Y + 20));
				((Control)mDummyInputKeyBoard).Enabled = true;
			}
		}
		SetKeyboardLayout(inputLang);
	}

	private void VMWindow_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				if (e.Y <= 40)
				{
					HTTPUtils.SendRequestToClientAsync("showFullscreenTopbar", new Dictionary<string, string> { { "visible", "true" } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				return;
			}
			if (e.X >= ((Control)this).Width - 40)
			{
				HTTPUtils.SendRequestToClientAsync("showFullscreenSidebarButton", new Dictionary<string, string> { { "visible", "true" } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				mIsSideBarVisible = true;
			}
			else if (mIsSideBarVisible)
			{
				HTTPUtils.SendRequestToClientAsync("showFullscreenSidebarButton", new Dictionary<string, string> { { "visible", "false" } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				mIsSideBarVisible = false;
			}
			if (e.Y <= 40)
			{
				HTTPUtils.SendRequestToClientAsync("showFullscreenTopbarButton", new Dictionary<string, string> { { "visible", "true" } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				mIsTopBarVisible = true;
			}
			else if (mIsTopBarVisible)
			{
				HTTPUtils.SendRequestToClientAsync("showFullscreenTopbarButton", new Dictionary<string, string> { { "visible", "false" } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				mIsTopBarVisible = false;
			}
		}
		catch (Exception ex)
		{
			Logger.Fatal(ex.ToString());
		}
	}

	public void SetKeyboardLayout(string keyboardLayout)
	{
		Thread thread = new Thread(() =>
		{
			try
			{
				string text = "setkeyboardlayout";
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "keyboardlayout", keyboardLayout } };
				Logger.Info("Sending request for " + text + " with data : ");
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					Logger.Info("key : " + item.Key + " value : " + item.Value);
				}
				JObject val = default;
				string text2 = VmCmdHandler.SendRequest(text, dictionary, MultiInstanceStrings.VmName, ref val);
				if (text2 == null || text2.Contains("error"))
				{
					Logger.Error("Failed to set keyboard layout in syn config...checking current IME");
					if (Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
					{
						SetPcImeWorkflow(isSet: true);
					}
					else if (Oem.Instance.IsSendGameManagerRequest)
					{
						HTTPUtils.SendRequestToClient("showIMESwitchPrompt", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					}
				}
				else if (!Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
				{
					SetPcImeWorkflow(isSet: false);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set keyboard layout... Err : " + ex.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public void SetPcImeWorkflow(bool isSet)
	{
		isUsePcImeWorkflow = isSet;
	}

	protected override void WndProc(ref Message m)
	{
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		if (isLogWndProc)
		{
			Logger.Info("WndProcMessage: " + m.Msg + "~~" + m.WParam + "~~" + m.LParam + "~~");
		}
		bool flag = false;
		switch (m.Msg)
		{
		case 255:
			InputMapper.Instance.HandleRawInput(m.LParam);
			break;
		case 1063:
			Logger.Info("Received message WM_USER_HIDE_WINDOW");
			HandleUserHideWindow();
			break;
		case 1025:
			Logger.Info("Received message WM_USER_SHOW_WINDOW");
			HandleUserShowWindow();
			break;
		case 1062:
			Logger.Info("Received message WM_USER_ACTIVATE");
			HandleFrontendActivated();
			break;
		case 1065:
			Logger.Info("Received message WM_USER_DEACTIVATE");
			HandleFrontendDeactivated();
			break;
		case 1059:
			Logger.Info("Received message WM_USER_AUDIO_MUTE");
			HandleFrontendMute();
			break;
		case 1060:
			Logger.Info("Received message WM_USER_AUDIO_UNMUTE");
			HandleFrontendUnMute();
			break;
		case 1058:
			Logger.Info("Received message WM_USER_SHOW_GUIDANCE");
			VmCmdHandler.RunCommand("controller_guidance_pressed", MultiInstanceStrings.VmName);
			break;
		case 130:
			Logger.Error("----------------------------> Got WM_NCDESTROY");
			Stats.SendFrontendStatusUpdate("frontend-closed", MultiInstanceStrings.VmName);
			break;
		case 274:
		{
			Logger.Info("Received message WM_SYSCOMMAND");
			int command = m.WParam.ToInt32();
			if (!HandleWMSysCommand(command))
			{
				return;
			}
			break;
		}
		case 74:
		{
			COPYGAMEPADDATASTRUCT val2 = (COPYGAMEPADDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYGAMEPADDATASTRUCT));
			byte[] array = new byte[val2.size];
			Marshal.Copy(val2.lpData, array, 0, array.Length);
			int[] array2 = new int[array.Length / 4];
			try
			{
				for (int i = 0; i < array.Length; i += 4)
				{
					array2[i / 4] = BitConverter.ToInt32(array, i);
				}
			}
			catch
			{
			}
			switch (m.WParam.ToInt32())
			{
			case 2:
			{
				GamePad val3 = default;
				val3.X = array2[1];
				val3.Y = array2[2];
				val3.Z = array2[3];
				val3.Rx = array2[4];
				val3.Ry = array2[5];
				val3.Rz = array2[6];
				val3.Hat = array2[7];
				val3.Mask = (uint)array2[8];
				GamePad gamepad = val3;
				InputMapper.Instance.DispatchGamePadUpdate(array2[0], gamepad);
				break;
			}
			default:
				Logger.Info("Recieved CopyData wParam: {0}", new object[1] { m.WParam });
				break;
			case 0:
			case 1:
				break;
			}
			break;
		}
		case 532:
		{
			if (!isStreamingModeEnabled)
			{
				return;
			}
			Logger.Info("size changed window message.." + m.Msg);
			SetAspectRationAndMinMaxOfForm();
			RECT val = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
			switch (m.WParam.ToInt32())
			{
			case 1:
			case 2:
				val.Bottom = val.Top + GetHeightFromWidth(((Control)this).Width);
				break;
			case 3:
			case 6:
				val.Right = val.Left + GetWidthFromHeight(((Control)this).Height);
				break;
			case 8:
				val.Bottom = val.Top + GetHeightFromWidth(((Control)this).Width);
				break;
			case 4:
				val.Left = val.Right - GetWidthFromHeight(((Control)this).Height);
				break;
			}
			Logger.Info("form width: {0} height : {1}", new object[2]
			{
				val.Right - val.Left,
				val.Bottom - val.Top
			});
			Marshal.StructureToPtr(val, m.LParam, fDeleteOld: true);
			flag = true;
			break;
		}
		default:
			flag = false;
			break;
		}
		base.WndProc(ref m);
		if (flag)
		{
			try
			{
				m.Result = new IntPtr(1);
			}
			catch (Exception)
			{
			}
		}
	}

	public void SetAspectRationAndMinMaxOfForm()
	{
		if (LayoutManager.mEmulatedPortraitMode)
		{
			widthRatio = 9.0;
			heightRatio = 16.0;
		}
		else
		{
			widthRatio = 16.0;
			heightRatio = 9.0;
		}
	}

	public static int GetWindowsScaling()
	{
		return (int)((double)Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);
	}

	public int GetWidthFromHeight(int height)
	{
		return (int)(widthRatio * ((double)height - heightDiff) / heightRatio) + (int)widthDiff;
	}

	public int GetHeightFromWidth(int width)
	{
		return (int)(heightRatio * ((double)width - widthDiff) / widthRatio) + (int)heightDiff;
	}

	public void ResizeWindowOnStreamingMode()
	{
		if (isStreamingModeEnabled)
		{
			SetAspectRationAndMinMaxOfForm();
			if (LayoutManager.mEmulatedPortraitMode)
			{
				HandleUserShowWindow(GetWidthFromHeight(((Control)this).Height), ((Control)this).Height);
			}
			else
			{
				HandleUserShowWindow(((Control)this).Width, GetHeightFromWidth(((Control)this).Width));
			}
		}
	}

	private bool HandleWMSysCommand(int command)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		if (command == 61488 || command == 61490 || command == 61728 || command == 61730)
		{
			Logger.Info("Received MAXIMIZE/RESTORE command");
			if ((int)((Form)this).WindowState == 1)
			{
				return true;
			}
		}
		if (command == 61696)
		{
			return false;
		}
		return true;
	}

	internal void ChangeCursorStyle(string path, bool isMOBACursor = false)
	{
		try
		{
			if (RegistryManager.Instance.CustomCursorEnabled && !string.IsNullOrEmpty(path) && AppHandler.mCurrentAppPackage.Equals("com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
			{
				path = ((!isMOBACursor) ? Constants.BrawlStarsCustomCursorPath : Constants.BrawlStarsMOBACursorPath);
			}
			if (File.Exists(path))
			{
				Logger.Info("CURSOR Changing to custom cursor " + path);
				((Control)this).Cursor = InteropWindow.LoadCustomCursor(path);
				if (!isMOBACursor)
				{
					mIsCustomCursorEnabled = true;
				}
			}
			else
			{
				Logger.Info("CURSOR Changing to default cursor");
				((Control)this).Cursor = Cursors.Default;
				mIsCustomCursorEnabled = false;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("CURSOR Error in changing cusor style: {0}", new object[1] { ex });
			((Control)this).Cursor = Cursors.Default;
		}
	}

	internal void HandleUserHideWindow()
	{
		Logger.Info("UserHideWindow start");
		((Control)this).Hide();
		((Form)this).WindowState = (FormWindowState)1;
		MediaManager.MuteEngine();
	}

	internal void HandleUserShowWindow(int width = 0, int height = 0)
	{
		Logger.Info("UserShowWindow start");
		ShowVmWindow(width, height);
		PostVmWindowShowTasks();
	}

	internal void ShowVmWindow(int width, int height)
	{
		if (width != 0)
		{
			((Control)this).Width = width;
		}
		if (height != 0)
		{
			((Control)this).Height = height;
		}
		((Control)this).Show();
		IsShownOnce = true;
		((Control)this).BringToFront();
		((Form)this).WindowState = (FormWindowState)0;
	}

	internal void PostVmWindowShowTasks()
	{
		HandleFrontendActivated();
		if (!MediaManager.mIsMutedExplicitly)
		{
			Logger.Info("Unmuting Engine");
			MediaManager.UnmuteEngine();
		}
		else
		{
			Logger.Info("Not unmuting Engine, since it is muted explicitly");
		}
	}

	internal bool IsFrontendReparented()
	{
		if (InteropWindow.GetParent(((Control)this).Handle) == IntPtr.Zero)
		{
			return false;
		}
		return true;
	}

	internal void HandleFrontendActivated()
	{
		Logger.Debug("KMP HandleFrontendActivated");
		if (!InputMapper.mIsVMWindowsActivated)
		{
			Stats.SendFrontendStatusUpdate("frontend-activated", MultiInstanceStrings.VmName);
		}
		InputMapper.mIsVMWindowsActivated = true;
		((Control)this).Focus();
		LayoutManager.FixupGuestDisplay();
		if (WasImeEnabled)
		{
			ChangeImeMode(enableIme: true, fromAndroid: false);
		}
		if (mIsTextInputBoxInFocus)
		{
			Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_TEXT");
			InputMapperHandlingState = InputHandlingState.IMAP_STATE_TEXT;
			if (sIsWpfTextboxEnabled)
			{
				mCtrlHost.Focus();
			}
			else
			{
				mDummyInputKeyBoard.Focus();
				((ContainerControl)this).ActiveControl = (Control)(object)mDummyInputKeyBoard;
			}
		}
		else if (InputMapper.s_UserKeyMappingEnabled)
		{
			Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_MAPPING");
			InputMapperHandlingState = InputHandlingState.IMAP_STATE_MAPPING;
		}
		else
		{
			Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_RAW");
			InputMapperHandlingState = InputHandlingState.IMAP_STATE_RAW;
		}
		if (AndroidBootUp.camManager != null)
		{
			AndroidBootUp.camManager.resumeCamera();
		}
		BstCursor.Instance.RaiseFocusChange();
	}

	internal void HandleFrontendDeactivated()
	{
		Logger.Debug("KMP HandleFrontendDeactivated");
		if (InputMapper.mIsVMWindowsActivated)
		{
			Stats.SendFrontendStatusUpdate("frontend-deactivated", MultiInstanceStrings.VmName);
		}
		InputMapper.mIsVMWindowsActivated = false;
		sKeyStateSet.Clear();
		BstCursor.Instance.RaiseFocusChange();
		InputMapperHandlingState = InputHandlingState.IMAP_STATE_NOFOCUS;
		if (WasImeEnabled)
		{
			ChangeImeMode(enableIme: false, fromAndroid: false);
		}
	}

	private void HandleFrontendMute()
	{
		MediaManager.MuteEngine(isMutedExplicitly: true);
	}

	private void HandleFrontendUnMute()
	{
		MediaManager.UnmuteEngine();
	}

	private void CloseService()
	{
		if (AndroidBootUp.isAndroidBooted)
		{
			/*Error near IL_0007: Invalid metadata token*/;
		}
	}

	internal void CloseWindow(FormClosingEventArgs e)
	{
		try
		{
			Logger.Info("Changing the frontend state to quitting");
			Utils.KillCurrentOemProcessByName("HD-RunApp", (string)null);
			CloseService();
			try
			{
				if (AndroidBootUp.camManager != null)
				{
					AndroidBootUp.camManager.pauseCamera();
				}
				StateExitConnected();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in stateexitconnected. Err : {0}", new object[1] { ex.ToString() });
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in closing form. Err : " + ex2.ToString());
		}
	}

	private void StateExitConnected()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected Obj, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected Obj, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected Obj, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected Obj, but got Unknown
		Logger.Info("Exiting state Connected");
		Opengl.glWindowAction = GlWindowAction.Hide;
		AndroidBootUp.GpsDetach();
		try
		{
			SensorDevice.Instance.Stop();
		}
		catch (Exception ex)
		{
			Logger.Info(ex.Message);
		}
		try
		{
			AndroidBootUp.CameraDetach();
		}
		catch (Exception ex2)
		{
			Logger.Info(ex2.Message);
		}
		((Control)this).MouseMove -= InputMapper.Instance.HandleMouseMove;
		((Control)this).MouseDown -= InputMapper.Instance.HandleMouseDown;
		((Control)this).MouseUp -= InputMapper.Instance.HandleMouseUp;
		((Control)this).MouseWheel -= InputMapper.Instance.HandleMouseWheel;
		TouchEvent -= HandleTouchEvent;
		((Control)this).KeyDown -= HandleKeyDown;
		((Control)this).KeyUp -= HandleKeyUp;
		try
		{
			displayTimeOutThread.Abort();
		}
		catch (Exception ex3)
		{
			Logger.Info(ex3.Message);
		}
		try
		{
			SystemEvents.DisplaySettingsChanged -= HandleDisplaySettingsChanged;
		}
		catch (Exception ex4)
		{
			Logger.Warning("Exception while unwinding DisplaySettingsChangedEvent. " + ex4.Message);
		}
		AndroidBootUp.mMonitor.Close();
		AndroidBootUp.mMonitor = null;
		AndroidBootUp.mManager = null;
		((Control)this).Invalidate();
	}

	public void UpdateMouse()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)this, (Action)(() =>
		{
			if (AndroidBootUp.isAndroidBooted)
			{
				Point point = ((Control)this).PointToClient(Control.MousePosition);
				int x = point.X;
				int y = point.Y;
				Mouse.Instance.UpdateCursor((uint)LayoutManager.GetGuestX(x, y), (uint)LayoutManager.GetGuestY(x, y));
				AndroidBootUp.mMonitor.SendMouseState(Mouse.Instance.X, Mouse.Instance.Y, Mouse.Instance.Mask);
			}
		}));
	}

	private void ClipCursorInGuestWindow()
	{
		Point point = ((Control)this).PointToScreen(default(Point));
		int x = point.X;
		int num = point.Y + 15;
		int width = ((Form)this).Size.Width;
		int num2 = ((Form)this).ClientSize.Height - 15;
		Logger.Info("cursor cli: {0} {1} {2} {3} {4}", new object[5]
		{
			((Form)this).Location.X,
			x,
			num,
			width,
			num2
		});
		Cursor.Clip = new Rectangle(new Point(x, num), new Size(width, num2));
	}

	public void SetMouseCursorPos(int cx, int cy, bool clipInGuestWindow)
	{
		Action val2 = default;
		ThreadPool.QueueUserWorkItem((object obj) =>
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected Obj, but got Unknown
			//IL_0024: Expected Obj, but got Unknown
			VMWindow vMWindow = this;
			Action val = val2;
			if (val == null)
			{
				Action val3 = () =>
				{
					Cursor.Position = ((Control)this).PointToScreen(new Point(cx, cy));
					if (clipInGuestWindow)
					{
						ClipCursorInGuestWindow();
					}
					else
					{
						Logger.Info("CURSOR clipping removed");
						Cursor.Clip = default;
					}
				};
				Action val4 = val3;
				val2 = val3;
				val = val4;
			}
			UIHelper.RunOnUIThread((Control)(object)vMWindow, val);
		});
	}

	public void ShowMouseCursor(bool show, bool clippingEnabled = true)
	{
		Logger.Info($"CURSOR ShowMouseCursor called {show}");
		Action val2 = default;
		ThreadPool.QueueUserWorkItem((object obj) =>
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected Obj, but got Unknown
			//IL_0024: Expected Obj, but got Unknown
			VMWindow vMWindow = this;
			Action val = val2;
			if (val == null)
			{
				Action val3 = () =>
				{
					if (show)
					{
						Cursor.Show();
						Logger.Info("CURSOR clipping removed and shown: " + Cursor.Position);
						if (clippingEnabled)
						{
							Cursor.Clip = default;
						}
					}
					else
					{
						Cursor.Hide();
						Logger.Info("CURSOR clipping added and hidden" + Cursor.Position);
						if (clippingEnabled)
						{
							ClipCursorInGuestWindow();
						}
					}
					Logger.Info("CURSOR: Clipping enabled: " + clippingEnabled);
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"IsShootingModeActivated",
						(!show).ToString()
					} };
					HTTPUtils.SendRequestToClientAsync("shootingModeChanged", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				};
				Action val4 = val3;
				val2 = val3;
				val = val4;
			}
			UIHelper.RunOnUIThread((Control)(object)vMWindow, val);
		});
	}

	internal void SaveScreenShot(string path, bool showSaved)
	{
		((Image)CaptureScreenShot()).Save(path, ImageFormat.Jpeg);
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "path", path },
			{
				"showSavedInfo",
				showSaved.ToString()
			}
		};
		HTTPUtils.SendRequestToClientAsync("screenshotCaptured", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private Bitmap CaptureScreenShot()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected Obj, but got Unknown
		Point point = ((Control)this).PointToScreen(new Point(0, 0));
		Point point2 = new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
		Point point3 = ((Control)this).PointToScreen(new Point(((Form)this).ClientSize.Width, ((Form)this).ClientSize.Height));
		Size size = new Size(Convert.ToInt32(point3.X - point2.X), Convert.ToInt32(point3.Y - point2.Y));
		Bitmap val = new Bitmap(size.Width, size.Height);
		Graphics val2 = Graphics.FromImage((Image)(object)val);
		try
		{
			val2.CopyFromScreen(point2, Point.Empty, size);
			return val;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	internal void HandleShareButtonClicked()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected Obj, but got Unknown
		int num = (((Control)this).Width - ((Form)this).ClientSize.Width) / 2;
		int num2 = ((Control)this).Height - ((Form)this).ClientSize.Height - 2 * num;
		int num3 = ((Control)this).Width + 2 * num;
		int num4 = ((Control)this).Height + 2 * num + num2;
		Bitmap val = new Bitmap(num3, num4);
		Graphics.FromImage((Image)(object)val).CopyFromScreen(new Point(((Control)this).Left, ((Control)this).Top), Point.Empty, new Size(((Control)this).Width, ((Control)this).Height));
		int num5 = new Random().Next(0, 100000);
		string text = $"bstSnapshot_{num5}.jpg";
		string text2 = $"final_{text}";
		if (RegistryManager.Instance.DefaultGuest.FileSystem == 0)
		{
			Logger.Info("Shared folders disabled");
			return;
		}
		string sharedFolderDir = RegistryStrings.SharedFolderDir;
		string text3 = "BstSharedFolder";
		string text4 = Path.Combine(sharedFolderDir, text);
		string text5 = Path.Combine(sharedFolderDir, text2);
		((Image)val).Save(text4, ImageFormat.Jpeg);
		try
		{
			Utils.AddUploadTextToImage(text4, text5);
			File.Delete(text4);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in adding upload text to snapshot. Err : " + ex.ToString());
			text2 = text;
			text5 = text4;
		}
		string text6 = string.Format("http://127.0.0.1:{0}/{1}", MultiInstanceStrings.BstServerPort, "sharepic");
		string text7 = "/mnt/sdcard/windows/" + text3 + "/" + Path.GetFileName(text2);
		Logger.Info("androidPath: " + text7);
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "data", text7 } };
		Logger.Info("Sending snapshot upload request.");
		string text8 = "";
		try
		{
			text8 = BstHttpClient.Post(text6, dictionary, (Dictionary<string, string>)null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp64");
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in sending post request. url = {0}, data = {1}. Err : {2}", new object[3]
			{
				text6,
				dictionary,
				ex2.ToString()
			});
		}
		if (text8.Contains("error") && !snapshotErrorShown)
		{
			snapshotErrorShown = true;
			if (snapshotErrorToast == null)
			{
				snapshotErrorToast = new Toast((Control)(object)this, LocaleStrings.GetLocalizedString("STRING_SNAPSHOT_ERROR_TOAST", ""));
			}
			Animate.AnimateWindow(((Control)snapshotErrorToast).Handle, 500, 262148);
			((Control)snapshotErrorToast).Show();
			Thread thread = new Thread(() =>
			{
				Thread.Sleep(3000);
				Animate.AnimateWindow(((Control)snapshotErrorToast).Handle, 500, 327688);
				snapshotErrorShown = false;
			});
			thread.IsBackground = true;
			thread.Start();
		}
	}

	internal IntPtr GetHandle()
	{
		if (((Control)this).Handle == IntPtr.Zero)
		{
			((Control)this).CreateHandle();
		}
		return ((Control)this).Handle;
	}

	private void SendControllerEvent(string name, int identity, string type)
	{
		string cmd = $"controller_{name} {identity} {type}";
		SendControllerEventInternal(cmd, null);
	}

	private void SendControllerEventInternal(string cmd, Action continuation)
	{
		Logger.Info("Sending controller event " + cmd);
		VmCmdHandler.RunCommandAsync(cmd, continuation, (Control)(object)this, MultiInstanceStrings.VmName);
	}

	internal void ChangeImeMode(bool enableIme, bool fromAndroid = true)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected Obj, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected Obj, but got Unknown
		Logger.Debug("KMP change enable ime called " + enableIme + "," + mIsTextInputBoxInFocus);
		if (enableIme && Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
		{
			Logger.Info("ime mode enabled");
			UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
			{
				mIsTextInputBoxInFocus = true;
				WasImeEnabled = true;
				InputMapperHandlingState = InputHandlingState.IMAP_STATE_TEXT;
				((Control)this).SuspendLayout();
				if (sIsWpfTextboxEnabled)
				{
					((Control)mCtrlHost).Location = ((Control)this).PointToClient(new Point(Cursor.Position.X + 20, Cursor.Position.Y + 20));
					((UIElement)mTextBoxControl.mWpfTextBox).IsEnabled = true;
				}
				else if (InputLanguageManager.Current.CurrentInputLanguage.Name.Equals("zh-tw", StringComparison.InvariantCultureIgnoreCase))
				{
					((Control)mDummyInputKeyBoard).Location = ((Control)this).PointToClient(new Point(-500, -500));
					((Control)mDummyInputKeyBoard).Enabled = true;
				}
				else
				{
					((Control)mDummyInputKeyBoard).Location = ((Control)this).PointToClient(new Point(Cursor.Position.X + 20, Cursor.Position.Y + 20));
					((Control)mDummyInputKeyBoard).Enabled = true;
				}
				((Control)this).ResumeLayout(false);
				((Control)this).PerformLayout();
				CleanUpTextBox();
				if (!OperationsSyncManager.mIsReceiving || InputMapper.mIsVMWindowsActivated)
				{
					if (sIsWpfTextboxEnabled)
					{
						mCtrlHost.Focus();
					}
					else
					{
						mDummyInputKeyBoard.Focus();
					}
				}
				InteropWindow.ImmSetOpenStatus(m_hImc, true);
			}));
		}
		else
		{
			Logger.Info("ime mode disabled");
			UIHelper.RunOnUIThread((Control)(object)Instance, (Action)(() =>
			{
				if (fromAndroid)
				{
					WasImeEnabled = false;
					if (InputMapperHandlingState != InputHandlingState.IMAP_STATE_NOFOCUS)
					{
						if (InputMapper.s_UserKeyMappingEnabled)
						{
							InputMapperHandlingState = InputHandlingState.IMAP_STATE_MAPPING;
						}
						else
						{
							InputMapperHandlingState = InputHandlingState.IMAP_STATE_RAW;
						}
					}
				}
				((Control)this).SuspendLayout();
				if (sIsWpfTextboxEnabled)
				{
					((UIElement)mTextBoxControl.mWpfTextBox).IsEnabled = false;
				}
				else
				{
					((Control)mDummyInputKeyBoard).Enabled = false;
				}
				((Control)this).ResumeLayout(false);
				((Control)this).PerformLayout();
				mIsTextInputBoxInFocus = false;
				InteropWindow.ImmSetOpenStatus(m_hImc, false);
				if (fromAndroid && (!OperationsSyncManager.mIsReceiving || InputMapper.mIsVMWindowsActivated))
				{
					((Control)this).Focus();
				}
			}));
		}
		InteropWindow.ImmSetOpenStatus(m_hImc, mIsTextInputBoxInFocus);
	}

	internal bool IsPackageAvailableForCustomCursor(string appPackage)
	{
		foreach (string mCustomCursorApps in mCustomCursorAppsList)
		{
			string text = mCustomCursorApps;
			if (mCustomCursorApps.EndsWith("*", StringComparison.InvariantCulture))
			{
				text = mCustomCursorApps.TrimEnd(new char[1] { '*' });
			}
			if (text.StartsWith("~", StringComparison.InvariantCulture))
			{
				if (appPackage.StartsWith(text.Substring(1), StringComparison.InvariantCulture))
				{
					return false;
				}
			}
			else if (appPackage.StartsWith(text, StringComparison.InvariantCulture))
			{
				return true;
			}
		}
		return false;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected Obj, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected Obj, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected Obj, but got Unknown
		((Control)this).SuspendLayout();
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.Black;
		((Form)this).ClientSize = new Size(1053, 675);
		((Control)this).DoubleBuffered = true;
		((Control)this).ForeColor = Color.LightGray;
		((Control)this).Name = "VMWindow";
		((Control)this).Text = "VMWindow";
		((Control)this).AllowDrop = true;
		if (Oem.Instance.IsDragDropEnabled)
		{
			((Control)this).DragEnter += FileImporter.HandleDragEnter;
			((Control)this).DragDrop += FileImporter.MakeDragDropHandler();
		}
		((Control)this).Resize += VMWindow_Resize;
		((Form)this).Activated += VMWindow_Activated;
		((Form)this).Deactivate += VMWindow_Deactivate;
		((Control)this).GotFocus += VMWindow_GotFocus;
		((Form)this).FormClosing += VMWindow_FormClosing;
		((Form)this).InputLanguageChanged += VMWindow_InputLanguageChanged;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}

	static VMWindow()
	{
		Instance = null;
		sLastTouchTime = 0;
		sKeyStateSet = new Dictionary<Keys, int>();
		isUsePcImeWorkflow = true;
		sIsWpfTextboxEnabled = false;
		isLogWndProc = false;
	}
}
