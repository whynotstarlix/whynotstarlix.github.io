using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

public class InputMapper
{
	[Flags]
	public enum RawMouseFlags : ushort
	{
		MoveRelative = 0,
		MoveAbsolute = 1,
		VirtualDesktop = 2,
		AttributesChanged = 4
	}

	[Flags]
	public enum RawMouseButtons : ushort
	{
		None = 0,
		LeftDown = 1,
		LeftUp = 2,
		RightDown = 4,
		RightUp = 8,
		MiddleDown = 0x10,
		MiddleUp = 0x20,
		Button4Down = 0x40,
		Button4Up = 0x80,
		Button5Down = 0x100,
		Button5Up = 0x200,
		MouseWheel = 0x400
	}

	[Flags]
	public enum RawKeyboardFlags : ushort
	{
		KeyMake = 0,
		KeyBreak = 1,
		KeyE0 = 2,
		KeyE1 = 4,
		TerminalServerSetLED = 8,
		TerminalServerShadow = 0x10,
		TerminalServerVKPACKET = 0x20
	}

	internal struct RAWINPUTHEADER
	{
		[MarshalAs(UnmanagedType.U4)]
		public int dwType;

		[MarshalAs(UnmanagedType.U4)]
		public int dwSize;

		public IntPtr hDevice;

		[MarshalAs(UnmanagedType.U4)]
		public int wParam;
	}

	public struct RAWINPUTMOUSE
	{
		public RawMouseFlags Flags;

		public ushort ButtonData;

		public RawMouseButtons ButtonFlags;

		public ulong RawButtons;

		public long LastX;

		public long LastY;

		public ulong ExtraInformation;
	}

	public struct RAWINPUTKEYBOARD
	{
		public short MakeCode;

		public RawKeyboardFlags Flags;

		public short Reserved;

		public ushort VirtualKey;

		public uint Message;

		public int ExtraInformation;
	}

	public struct RAWINPUTHID
	{
		public int Size;

		public int Count;

		public IntPtr Data;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RAWINPUT32
	{
		[FieldOffset(0)]
		public RAWINPUTHEADER header;

		[FieldOffset(16)]
		public RAWINPUTMOUSE mouse;

		[FieldOffset(16)]
		public RAWINPUTKEYBOARD keyboard;

		[FieldOffset(16)]
		public RAWINPUTHID hid;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RAWINPUT64
	{
		[FieldOffset(0)]
		public RAWINPUTHEADER header;

		[FieldOffset(24)]
		public RAWINPUTMOUSE mouse;

		[FieldOffset(24)]
		public RAWINPUTKEYBOARD keyboard;

		[FieldOffset(24)]
		public RAWINPUTHID hid;
	}

	public struct TouchPoint
	{
		public float X;

		public float Y;

		public bool Down;
	}

	public struct BST_INPUT_TOUCH_POINT
	{
		public int X;

		public int Y;
	}

	public enum Direction
	{
		None,
		Up,
		Down,
		Left,
		Right
	}

	public enum GamepadEvent
	{
		None,
		Attach,
		Detach,
		GuidancePress,
		GuidanceRelease
	}

	public delegate void ModeHandlerNet(string mode);

	private delegate void KeyHandler(IntPtr context, byte code);

	private delegate void TouchHandler(IntPtr context, IntPtr list, int count, int offset);

	private delegate void TiltHandler(IntPtr context, float x, float y, float z);

	private delegate void ClientActionHandler([In][MarshalAs(UnmanagedType.LPStr)] string action);

	private delegate void GameControlStatusUpdate([In][MarshalAs(UnmanagedType.LPStr)] string values, int valueCount, string vmName);

	private delegate void ClientGetGamepadButtonHandler([In][MarshalAs(UnmanagedType.LPStr)] string gamepadButton, [In] int down);

	private delegate void SetGamepadStatusHandler(int state);

	private delegate void GamepadBackButtonPressedHandler();

	private delegate void PlaybackCompleteHandler();

	private delegate void RegisterRawInputMouseHandler(bool register);

	private delegate void ModeHandler(IntPtr context, string mode);

	private delegate void MoveHandler(IntPtr context, int identity, int x, int y);

	private delegate void ClickHandler(IntPtr context, int identity, int down);

	private delegate void SpecialHandler(IntPtr context, string cmd);

	private delegate void GamepadHandler(IntPtr context, int identity, GamepadEvent evt, string layout);

	private delegate void MouseHandler(IntPtr context);

	private delegate IntPtr ShootHandler(IntPtr context, int identity);

	private delegate void SetMouseCursorPos(int x, int y, bool clipInGuestWindow);

	private delegate void ShowMouseCursor(bool show, bool clippingEnabled);

	[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	private delegate void HandleGuestBootLogs(IntPtr bootLogs, int nLogs);

	private delegate void LogHandler(string msg);

	public struct RAWINPUTDEVICE
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort usUsagePage;

		[MarshalAs(UnmanagedType.U2)]
		public ushort usUsage;

		[MarshalAs(UnmanagedType.U4)]
		public int dwFlags;

		public IntPtr hwndTarget;
	}

	private delegate void ImapSendImageInfoHandler(IntPtr sceneArray, int nImages);

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Work _003C_003E9__68_0;

		public static Work _003C_003E9__69_0;

		internal void _003CDispatchControllerEvent_003Eb__68_0()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
			{
				AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				Logger.Debug("DispatchControllerEvent " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
			}
			if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
			{
				return;
			}
			if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
			{
				AppHandler.sAppPackagesCountClicks["*"]++;
			}
			if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
			{
				if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
				{
					AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
					AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				}
				else
				{
					AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				}
			}
		}

		internal void _003CDispatchGamePadUpdate_003Eb__69_0()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
			{
				AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				Logger.Debug("DispatchGamePadUpdate " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
			}
			if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
			{
				return;
			}
			if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
			{
				AppHandler.sAppPackagesCountClicks["*"]++;
			}
			if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
			{
				if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
				{
					AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
					AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				}
				else
				{
					AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				}
			}
		}
	}

	public const int CURSOR_SLOTS = 4;

	private const int GAMEPAD_AXIS_MAX = 1000;

	private const int CURSOR_MOVE_FACTOR = 10;

	private const string TEMPLATE = "TEMPLATE.cfg";

	private const string DEFAULT = "DEFAULT.cfg";

	private const int MAX_X = 32767;

	private const int MAX_Y = 32767;

	private const int MOUSE_SLOT = 13;

	public const int RIDEV_INPUTSINK = 256;

	public const int RIDEV_NOLEGACY = 48;

	public const int RIDEV_REMOVE = 1;

	public const int RID_INPUT = 268435459;

	public const int RIM_TYPEMOUSE = 0;

	private static object syncRoot;

	private static object syncRoot1;

	private Monitor.TouchPoint[] touchPoints = new Monitor.TouchPoint[16];

	private static InputMapper sInstance;

	private string mUserFolder;

	private Monitor mMonitor;

	private Monitor.TouchPoint[] mTouchPoints = new Monitor.TouchPoint[16];

	private string mCurrentPackage;

	private SerialWorkQueue mSerialQueue;

	private object mCursorLock = new object();

	private Point[] mCursorDeltas;

	private Point mLastMouseMoveLocation = new Point(0, 0);

	private TiltHandler mTiltHandler;

	private ClientActionHandler mClientActionHandler;

	private ClientGetGamepadButtonHandler mClientGetGamepadButtonHandler;

	private SetGamepadStatusHandler mSetGamepadStatusHandler;

	private GamepadBackButtonPressedHandler mGamepadBackButtonPressedHandler;

	private PlaybackCompleteHandler mPlaybackCompleteHandler;

	private GameControlStatusUpdate mGameControlStatusUpdateHandler;

	private RegisterRawInputMouseHandler mRegisterRawInputMouseHandler;

	private SetMouseCursorPos mSetMouseCursorPos;

	private ShowMouseCursor mShowMouseCursor;

	private static HandleGuestBootLogs mHandleGuestBootLogs;

	private ImapSendImageInfoHandler mImapSendImageInfoHandler;

	public float mSoftControlBarHeightLandscape;

	public float mSoftControlBarHeightPortrait;

	public bool mSoftControlEnabled;

	private static uint[] sMapableKeyArray;

	private static Dictionary<uint, int> sMapableKeySet;

	internal static bool s_UserKeyMappingEnabled;

	internal static bool mIsVMWindowsActivated;

	internal static bool mIsGamepadConnected;

	internal static bool isSendSensorDeviceData;

	internal static bool IsMacroPlaying;

	public static InputMapper Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot1)
				{
					if (sInstance == null)
					{
						sInstance = new InputMapper();
					}
				}
			}
			return sInstance;
		}
	}

	[DllImport("User32.dll")]
	private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

	[DllImport("user32.dll")]
	public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SystemParametersInfo(int uiAction, int uiParam, uint pvParam, int fWinIni);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SystemParametersInfo(int uiAction, int uiParam, ref uint pvParam, int fWinIni);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

	[DllImport("kernel32.dll")]
	private static extern bool FreeLibrary(int hModule);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapShake();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapSetState(int state);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapStartRecording();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
	private static extern int ImapValidateScriptCommands(StringBuilder commands, ref int errIndex, ref bool isValid);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
	private static extern int ImapGetInteractionStats(ref int numInteractions, ref int numMappedInteractions, ref int numTextInteractions, ref int nativeGamepadUsed, StringBuilder interactionHist);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapStopRecording(StringBuilder events, ref int size);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
	private static extern void ImapPauseRecording();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapGetParserVersion();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapLoadConfig([MarshalAs(UnmanagedType.LPWStr)] string pkg, [MarshalAs(UnmanagedType.LPWStr)] string activity, ref bool SendSensorDataToGuest);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapInitPlayback([MarshalAs(UnmanagedType.LPWStr)] string filePath, PlaybackCompleteHandler mPlaybackCompleteHandler);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapStopPlayback();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	internal static extern int ImapRunMacroUnit([MarshalAs(UnmanagedType.LPWStr)] string macroName, double acceleration);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapStartSync(bool stopAllThreads);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapStopSync(bool stopAllThreads);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapStartSyncConsumer([MarshalAs(UnmanagedType.LPWStr)] string sourceVmName);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapStopSyncConsumer();

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapHandleConfigChange([MarshalAs(UnmanagedType.LPWStr)] string pkg, [MarshalAs(UnmanagedType.LPWStr)] string activity);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapInit(int guestWidth, int guestHeight, [MarshalAs(UnmanagedType.LPWStr)] string configFolder, [MarshalAs(UnmanagedType.LPWStr)] string userConfigFolder, HdLoggerCallback cb, SetMouseCursorPos mSetMouseCursorPos, ShowMouseCursor mShowMouseCursor, IntPtr monitorSendTouchState, IntPtr monitorSendScanCode, IntPtr monitorSendGamepadState, IntPtr monitorSendImeMsg, TiltHandler mTiltHandler, RegisterRawInputMouseHandler mRegisterRawInputMouseHandler, ClientActionHandler mClientActionHandler, ClientGetGamepadButtonHandler mClientGetGamepadButtonHandler, [MarshalAs(UnmanagedType.LPWStr)] string vmName, GameControlStatusUpdate mGameControlStatusUpdateHandler);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapRegisterGamepadConnectedCb(SetGamepadStatusHandler mSetGamepadStatusHandler, GamepadBackButtonPressedHandler mGamepadBackButtonPressedHandler);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapHandleTouch([In] Monitor.TouchPoint[] tps, int nTouchPoints);

	[DllImport("HD-PgaSocketHgcm.dll", CharSet = CharSet.Ansi)]
	private static extern void BstGetGuestBootLogs(HandleGuestBootLogs guestBootLogsCb);

	[DllImport("HD-Plus-Frontend-Native.dll")]
	private static extern bool MonitorSendTouchState(BST_INPUT_TOUCH_POINT[] points, int count);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	public static extern int ImapHandleIme([MarshalAs(UnmanagedType.LPWStr)] string buf);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	public static extern int ImapHandleClient([MarshalAs(UnmanagedType.LPWStr)] string buf, bool needCb);

	[DllImport("HD-Imap-Native.dll")]
	private static extern int ImapHandleKey(uint keyCode, int down);

	[DllImport("HD-Imap-Native.dll")]
	private static extern int ImapHandleRawInput(IntPtr buffer);

	[DllImport("HD-Imap-Native.dll")]
	private static extern int ImapHandleMouse(int evt, int x, int y, bool leftButtonPressed, bool rightButtonPressed, bool middleButtonPressed, bool xButton1Pressed, bool xButton2Pressed, int delta);

	[DllImport("User32.dll")]
	private static extern uint MapVirtualKey(uint code, uint mapType);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapEnableGamepadButtonCb(bool enable);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapGamepadEnable(bool enable);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapSetGamepadMode(int mode);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern int ImapSwitchBackFromHomeCallback([MarshalAs(UnmanagedType.LPWStr)] string pkg);

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapEnableScrollOnEdge(bool enable);

	public static bool IsMapableKey(uint scanCode)
	{
		lock (syncRoot)
		{
			if (sMapableKeySet == null)
			{
				sMapableKeySet = new Dictionary<uint, int>();
				for (int i = 0; i < sMapableKeyArray.Length; i++)
				{
					sMapableKeySet.Add(sMapableKeyArray[i], 1);
				}
			}
			return sMapableKeySet.ContainsKey(scanCode);
		}
	}

	public static string GetKeyMappingParserVersion()
	{
		int num = ImapGetParserVersion();
		Logger.Info("the parserVersion returned is {0}", new object[1] { num });
		return num.ToString();
	}

	public InputMapper()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		mSerialQueue = new SerialWorkQueue();
		mSerialQueue.Start();
	}

	internal static void RegisterGuestBootLogsHandler()
	{
		try
		{
			mHandleGuestBootLogs = HandleGuestBootLogsImpl;
			BstGetGuestBootLogs(mHandleGuestBootLogs);
		}
		catch (Exception ex)
		{
			string text = "Error in RegisterGuestBootLogsHandler ";
			Logger.Error(text + ex);
		}
	}

	internal void InputmapperInit()
	{
		mUserFolder = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
		mCursorDeltas = new Point[4];
		mTiltHandler = TiltHandlerImpl;
		mClientActionHandler = ClientActionHandlerImpl;
		mClientGetGamepadButtonHandler = ClientGetGamepadButtonImpl;
		mSetGamepadStatusHandler = SetGamepadStatusHandlerImpl;
		mGamepadBackButtonPressedHandler = GamepadBackButtonPressedHandlerImpl;
		mPlaybackCompleteHandler = MacroManager.Instance.PlaybackCompleteHandlerImpl;
		mGameControlStatusUpdateHandler = GameControlsStatusUpdateHandlerImpl;
		mRegisterRawInputMouseHandler = RegisterRawInputMouseImpl;
		mSetMouseCursorPos = SetMouseCursorPosImpl;
		mShowMouseCursor = ShowMouseCursorImpl;
		int guestWidth = RegistryManager.Instance.DefaultGuest.GuestWidth;
		int guestHeight = RegistryManager.Instance.DefaultGuest.GuestHeight;
		IntPtr intPtr = LoadLibrary("HD-Plus-Frontend-Native.dll");
		if (intPtr == IntPtr.Zero)
		{
			Debugger.Break();
			return;
		}
		IntPtr procAddress = GetProcAddress(intPtr, "MonitorSendTouchState");
		IntPtr procAddress2 = GetProcAddress(intPtr, "MonitorSendScanCode");
		IntPtr procAddress3 = GetProcAddress(intPtr, "MonitorSendGamepadState");
		IntPtr procAddress4 = GetProcAddress(intPtr, "MonitorSendImeMsg");
		ImapGamepadEnable(RegistryManager.Instance.GamepadDetectionEnabled);
		ImapInit(guestWidth, guestHeight, RegistryStrings.InputMapperFolder, mUserFolder, Logger.GetHdLoggerCallback(), mSetMouseCursorPos, mShowMouseCursor, procAddress, procAddress2, procAddress3, procAddress4, mTiltHandler, mRegisterRawInputMouseHandler, mClientActionHandler, mClientGetGamepadButtonHandler, MultiInstanceStrings.VmName, mGameControlStatusUpdateHandler);
		ImapRegisterGamepadConnectedCb(mSetGamepadStatusHandler, mGamepadBackButtonPressedHandler);
	}

	internal static void SoftControlBarVisible(bool visible)
	{
		float landscape = 0f;
		float portrait = 0f;
		Instance.mSoftControlEnabled = visible;
		if (visible)
		{
			float num = RegistryManager.Instance.DefaultGuest.SoftControlBarHeightLandscape;
			float num2 = RegistryManager.Instance.DefaultGuest.SoftControlBarHeightPortrait;
			landscape = num / (float)LayoutManager.mConfiguredDisplaySize.Height;
			portrait = num2 / (float)LayoutManager.mConfiguredDisplaySize.Width;
		}
		Instance.SetSoftControlBarHeight(landscape, portrait);
	}

	internal void SetupSoftControlBar()
	{
		if (!Utils.IsAndroidFeatureBitEnabled(1u, MultiInstanceStrings.VmName))
		{
			Logger.Info("Soft Control Bar Enabled");
			SoftControlBarVisible(visible: true);
		}
	}

	internal void SetSoftControlBarHeight(float landscape, float portrait)
	{
		Logger.Info("SetSoftControlBarHeight({0}, {1})", new object[2] { landscape, portrait });
		mSoftControlBarHeightLandscape = landscape;
		mSoftControlBarHeightPortrait = portrait;
	}

	public void SetMonitor(Monitor monitor)
	{
		mMonitor = monitor;
	}

	internal string GetMacroFileName(bool isUserDirectoryRequested, string PackageName)
	{
		string path = PackageName + "_macro.cfg";
		string result = Path.Combine(RegistryStrings.InputMapperFolder, path);
		string text = Path.Combine(mUserFolder, path);
		if (File.Exists(text) | isUserDirectoryRequested)
		{
			return text;
		}
		return result;
	}

	public string GetPackage()
	{
		return mCurrentPackage;
	}

	public void SetPackage(string package)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		mCurrentPackage = package;
		MacroData.Instance.LoadMacroData(mCurrentPackage);
		mSerialQueue.Enqueue((Work)(() =>
		{
			Logger.Info("Package name is {0}", new object[1] { package });
			ImapLoadConfig(package, "", ref isSendSensorDeviceData);
		}));
	}

	public void InitMacroPlayback(string filePath)
	{
		ImapInitPlayback(filePath, mPlaybackCompleteHandler);
		IsMacroPlaying = true;
	}

	public void StopMacroPlayback()
	{
		ImapStopPlayback();
		IsMacroPlaying = false;
	}

	public void RunMacroUnit(string macroName, double acceleration)
	{
		ImapRunMacroUnit(macroName, acceleration);
	}

	internal void StartOperationsSync()
	{
		ImapStartSync(stopAllThreads: true);
	}

	internal void StopOperationsSync()
	{
		ImapStopSync(stopAllThreads: true);
	}

	internal void PlayPauseOperationsSync(bool isPause)
	{
		if (isPause)
		{
			ImapStopSync(stopAllThreads: false);
		}
		else
		{
			ImapStartSync(stopAllThreads: false);
		}
	}

	internal void StartSyncConsumer(string fromVmName)
	{
		ImapStartSyncConsumer(fromVmName);
	}

	internal void StopSyncConsumer()
	{
		ImapStopSyncConsumer();
	}

	public void RefreshKeymapping(string package)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected Obj, but got Unknown
		mCurrentPackage = package;
		MacroData.Instance.LoadMacroData(mCurrentPackage);
		mSerialQueue.Enqueue((Work)(() =>
		{
			Logger.Info("Package name is {0}", new object[1] { package });
			SetPackage(package);
		}));
	}

	public void HandleLoadConfigAfterHomeSwitch(string package)
	{
		ImapSwitchBackFromHomeCallback(package);
	}

	public void ShowConfigDialog()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Form)new InputMapperForm(mCurrentPackage, EditHandler, ManageHandler)).ShowDialog();
	}

	private void EditHandler(string package)
	{
		string path = package + ".cfg";
		string sourceFileName = Path.Combine(RegistryStrings.InputMapperFolder, "TEMPLATE.cfg");
		string text = Path.Combine(RegistryStrings.InputMapperFolder, path);
		string text2 = Path.Combine(mUserFolder, path);
		if (!File.Exists(text2) && File.Exists(text))
		{
			File.Copy(text, text2);
		}
		text = text2;
		Logger.Info("Editing input mapper file '{0}'", new object[1] { text });
		try
		{
			if (!File.Exists(text))
			{
				File.Copy(sourceFileName, text);
			}
			Process process = new Process();
			process.StartInfo.FileName = "notepad.exe";
			process.StartInfo.Arguments = "\"" + text + "\"";
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in edit input mapper file Err : " + ex.ToString());
		}
	}

	private void ManageHandler(string package)
	{
		string path = package + ".cfg";
		string path2 = Path.Combine(mUserFolder, path);
		string inputMapperFolder = RegistryStrings.InputMapperFolder;
		if (File.Exists(path2))
		{
			inputMapperFolder = mUserFolder;
		}
		try
		{
			Process process = new Process();
			process.StartInfo.FileName = inputMapperFolder;
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in open input mapper folder. Err : " + ex.ToString());
		}
	}

	public void DispatchKeyboardEvent(uint keyCode, bool down)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected Obj, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(AppHandler.mCurrentAppPackage))
		{
			return;
		}
		VMWindow.Instance.SerialQueue.Enqueue((Work)(() =>
		{
			ImapHandleKey(keyCode, down ? 1 : 0);
		}));
		if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()) & down)
		{
			AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
			Logger.Debug("DispatchKeyboardEvent " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
		}
		if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
		{
			return;
		}
		if (AppHandler.sAppPackagesCountClicks.ContainsKey("*") & down)
		{
			AppHandler.sAppPackagesCountClicks["*"]++;
		}
		if (AppHandler.sAppPackagesCountClicks.ContainsKey("?") & down)
		{
			if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
			{
				AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
				AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
			}
			else
			{
				AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
			}
		}
	}

	public void DispatchControllerEvent(int identity, uint button, int down)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected Obj, but got Unknown
		SerialWorkQueue val = mSerialQueue;
		Work val2 = _003C_003Ec._003C_003E9__68_0;
		if (val2 == null)
		{
			Work val3 = () =>
			{
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
				{
					AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
					Logger.Debug("DispatchControllerEvent " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
				}
				if (!new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) && !AppHandler.mCurrentAppPackage.Equals("Home"))
				{
					if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
					{
						AppHandler.sAppPackagesCountClicks["*"]++;
					}
					if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
					{
						if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
						{
							AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
							AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
						}
						else
						{
							AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
						}
					}
				}
			};
			_003C_003Ec._003C_003E9__68_0 = val3;
			val2 = val3;
		}
		val.Enqueue(val2);
	}

	public void DispatchGamePadUpdate(int identity, GamePad gamepad)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected Obj, but got Unknown
		SerialWorkQueue val = mSerialQueue;
		Work val2 = _003C_003Ec._003C_003E9__69_0;
		if (val2 == null)
		{
			Work val3 = () =>
			{
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
				{
					AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
					Logger.Debug("DispatchGamePadUpdate " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
				}
				if (!new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) && !AppHandler.mCurrentAppPackage.Equals("Home"))
				{
					if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
					{
						AppHandler.sAppPackagesCountClicks["*"]++;
					}
					if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
					{
						if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
						{
							AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
							AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
						}
						else
						{
							AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
						}
					}
				}
			};
			_003C_003Ec._003C_003E9__69_0 = val3;
			val2 = val3;
		}
		val.Enqueue(val2);
	}

	internal void SendAndroidString(string formattedString)
	{
		ImapHandleIme(formattedString);
	}

	internal void SendClientString(string formattedString)
	{
		ImapHandleClient(formattedString, needCb: false);
	}

	public void TouchHandlerImpl(IntPtr array, int count, int offset)
	{
		TouchHandlerImpl(array, count, offset, adjustForControlBar: true);
	}

	public void TouchHandlerImpl(IntPtr array, int count, int offset, bool adjustForControlBar)
	{
		try
		{
			TouchHandlerImplInternal(array, count, offset, adjustForControlBar);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending mapped touch points: " + ex.ToString());
		}
	}

	public void TouchHandlerImplInternal(IntPtr array, int count, int offset, bool adjustForControlBar)
	{
		TouchPoint[] array2 = new TouchPoint[count];
		int num = Marshal.SizeOf(typeof(TouchPoint));
		for (int i = 0; i < count; i++)
		{
			IntPtr ptr = new IntPtr(array.ToInt64() + i * num);
			array2[i] = (TouchPoint)Marshal.PtrToStructure(ptr, typeof(TouchPoint));
		}
		TouchHandlerImpl(array2, offset, adjustForControlBar);
	}

	public void TouchHandlerImpl(TouchPoint[] points, int offset, bool adjustForControlBar)
	{
		for (int i = 0; i + offset < mTouchPoints.Length && i < points.Length; i++)
		{
			TouchPoint touchPoint = points[i];
			if (!touchPoint.Down)
			{
				mTouchPoints[i + offset].PosX = int.MaxValue;
				mTouchPoints[i + offset].PosY = int.MaxValue;
				continue;
			}
			int num = (int)(touchPoint.X * 32767f);
			int num2 = (int)(touchPoint.Y * 32767f);
			float num3 = 0f;
			if (!LayoutManager.mEmulatedPortraitMode)
			{
				if (adjustForControlBar)
				{
					num3 = mSoftControlBarHeightLandscape;
				}
				if (i + offset != 13)
				{
					num2 = (int)((float)num2 * (1f - num3));
				}
				if (!LayoutManager.mRotateGuest180)
				{
					mTouchPoints[i + offset].PosX = num;
					mTouchPoints[i + offset].PosY = num2;
				}
				else
				{
					mTouchPoints[i + offset].PosX = 32767 - num;
					mTouchPoints[i + offset].PosY = 32767 - num2;
				}
			}
			else
			{
				if (adjustForControlBar)
				{
					num3 = mSoftControlBarHeightPortrait;
				}
				if (i + offset != 13)
				{
					num2 = (int)((float)num2 * (1f - num3));
				}
				if (!LayoutManager.mRotateGuest180)
				{
					mTouchPoints[i + offset].PosX = 32767 - num2;
					mTouchPoints[i + offset].PosY = num;
				}
				else
				{
					mTouchPoints[i + offset].PosX = num2;
					mTouchPoints[i + offset].PosY = 32767 - num;
				}
			}
		}
		if (mMonitor != null)
		{
			mMonitor.SendTouchState(mTouchPoints);
		}
	}

	public void UpdateMouseStatus(int guestX, int guestY, int msg, MouseButtons button, int delta = 0)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Invalid comparison between Unknown and I4
		int x = (int)((double)guestX * (double)LayoutManager.mScaledDisplayArea.Width / 32768.0);
		int y = (int)((double)guestY * (double)LayoutManager.mScaledDisplayArea.Height / 32768.0);
		ImapHandleMouse(msg, x, y, (button & 0x100000) == 1048576, (button & 0x200000) == 2097152, (button & 0x400000) == 4194304, (button & 0x800000) == 8388608, (button & 0x1000000) == 16777216, delta);
	}

	private void TiltHandlerImpl(IntPtr context, float x, float y, float z)
	{
		SensorDevice.Instance.SetAccelerometerVector(x, y, z);
	}

	private void PlaybackCompleteHandlerImpl()
	{
		HTTPUtils.SendRequestToClientAsync("macroPlaybackComplete", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void ClientActionHandlerImpl([In][MarshalAs(UnmanagedType.LPStr)] string action)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "data", action } };
		HTTPUtils.SendRequestToClientAsync("handleClientOperation", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void GameControlsStatusUpdateHandlerImpl([In][MarshalAs(UnmanagedType.LPStr)] string values, [In] int valueCount, string vmname)
	{
		Logger.Info("Got callback Gamecontrolstatus...");
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "data", values } };
		Logger.Info("Vmname from Imap: " + vmname);
		HTTPUtils.SendRequestToClientAsync("overlayControlsVisibility", dictionary, vmname, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void ClientGetGamepadButtonImpl([In][MarshalAs(UnmanagedType.LPStr)] string gamepadButton, [In] int down)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "data", gamepadButton },
			{
				"isDown",
				(down == 1) ? "true" : "false"
			}
		};
		HTTPUtils.SendRequestToClientAsync("handleClientGamepadButton", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void SetGamepadStatusHandlerImpl(int gamepadStatus)
	{
		if (!mIsGamepadConnected ^ (gamepadStatus == 0))
		{
			mIsGamepadConnected = gamepadStatus != 0;
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"status",
				mIsGamepadConnected.ToString()
			} };
			HTTPUtils.SendRequestToClientAsync("handleGamepadConnection", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 5, 1000, "bgp64");
		}
	}

	private void GamepadBackButtonPressedHandlerImpl()
	{
		HTTPUtils.SendRequestToClientAsync("handleGamepadGuidanceButton", new Dictionary<string, string>(), MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private void RegisterRawInputMouseImpl(bool register)
	{
		RAWINPUTDEVICE[] array = new RAWINPUTDEVICE[1];
		array[0].usUsagePage = 1;
		array[0].usUsage = 2;
		if (register)
		{
			array[0].hwndTarget = ((Control)VMWindow.Instance).Handle;
			array[0].dwFlags = 304;
		}
		else
		{
			array[0].hwndTarget = IntPtr.Zero;
			array[0].dwFlags = 1;
		}
		RegisterRawInputDevices(array, 1, Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
	}

	private void SetMouseCursorPosImpl(int cx, int cy, bool clipInGuestWindow)
	{
		if (VMWindow.Instance != null)
		{
			VMWindow.Instance.SetMouseCursorPos(cx, cy, clipInGuestWindow);
		}
	}

	private static void HandleGuestBootLogsImpl(IntPtr bootLogs, int nLogs)
	{
		ThreadPool.QueueUserWorkItem((object _) =>
		{
			try
			{
				IntPtr[] array = new IntPtr[nLogs];
				Marshal.Copy(bootLogs, array, 0, nLogs);
				if (Marshal.PtrToStringAnsi(array[0]).Equals("timeline", StringComparison.OrdinalIgnoreCase))
				{
					TimelineStatsSender.HandleEngineBootEvent(Marshal.PtrToStringAnsi(array[1]));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to parse boot event");
				Logger.Error(ex.ToString());
			}
		});
	}

	private void ShowMouseCursorImpl(bool show, bool clippingEnabled = true)
	{
		if (VMWindow.Instance != null)
		{
			VMWindow.Instance.ShowMouseCursor(show, clippingEnabled);
		}
	}

	internal void RefreshKeyMapping(string appPkg)
	{
		Logger.Info("Refresh Keymapping");
		if (string.IsNullOrEmpty(appPkg) && !string.IsNullOrEmpty(AppHandler.mCurrentAppPackage))
		{
			RefreshKeymapping(AppHandler.mCurrentAppPackage);
		}
		else
		{
			RefreshKeymapping(appPkg);
		}
	}

	internal void LaunchBlueStacksKeyMapper()
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "action", "com.bluestacks.appguidance.GuidanceScreen" },
				{ "extras", "{\"event\":\"show_guidance_app_player\"}" }
			};
			string text = HTTPUtils.SendRequestToGuest("customStartService", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, true, 10, 500, "bgp64");
			if (((object)JObject.Parse(text)["result"]).ToString().Trim() == "ok")
			{
				Logger.Info("The key mapping tool launched successfully");
				return;
			}
			Logger.Error("The key mapping tool could not be launched, Response: {0}", new object[1] { text });
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception occured in trying to launch key mapping tool. Err : {ex.ToString()}");
		}
	}

	internal void HandleRawInput(IntPtr lParam)
	{
		uint pcbSize = 0u;
		GetRawInputData(lParam, 268435459u, IntPtr.Zero, ref pcbSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
		IntPtr intPtr = Marshal.AllocHGlobal((int)pcbSize);
		if (GetRawInputData(lParam, 268435459u, intPtr, ref pcbSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) != pcbSize)
		{
			Logger.Error("GetRawInputData does not return correct size\n");
			return;
		}
		ImapHandleRawInput(intPtr);
		Marshal.FreeHGlobal(intPtr);
	}

	internal void HandleTouchEvent(object sender, WMTouchForm.WMTouchEventArgs e)
	{
		VMWindow.Instance.UpdateUserActivityStatus();
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			WMTouchForm.TouchPoint point = e.GetPoint(i);
			if (point.Id != -1)
			{
				int landscapeGuestX = LayoutManager.GetLandscapeGuestX(point.X, point.Y);
				int landscapeGuestY = LayoutManager.GetLandscapeGuestY(point.X, point.Y);
				touchPoints[i].PosX = (int)((double)landscapeGuestX * (double)LayoutManager.mScaledDisplayArea.Width / 32768.0);
				touchPoints[i].PosY = (int)((double)landscapeGuestY * (double)LayoutManager.mScaledDisplayArea.Height / 32768.0);
				num++;
			}
			else
			{
				touchPoints[i].PosX = int.MaxValue;
				touchPoints[i].PosY = int.MaxValue;
			}
		}
		ImapHandleTouch(touchPoints, num);
	}

	internal void HandleMouseWheel(object sender, MouseEventArgs e)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		VMWindow.Instance.UpdateUserActivityStatus();
		if (!Input.IsEventFromTouch())
		{
			UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(e.X, e.Y), LayoutManager.GetLandscapeGuestY(e.X, e.Y), 522, e.Button, e.Delta);
		}
	}

	internal void HandleMouseUp(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Button == 2097152)
		{
			if (VMWindow.Instance.mIsInScriptMode)
			{
				return;
			}
			if (VMWindow.Instance.mIsCustomCursorEnabled)
			{
				VMWindow.Instance.ChangeCursorStyle(Constants.CustomCursorPath);
			}
			else
			{
				VMWindow.Instance.ChangeCursorStyle("");
			}
		}
		VMWindow.Instance.UpdateUserActivityStatus();
		MacroForm.RecordMouse(e.X, e.Y, ((Form)VMWindow.Instance).ClientSize.Width, ((Form)VMWindow.Instance).ClientSize.Height, e.Button, (ActionType)3);
		if (AppHandler.mCurrentAppPackage != null)
		{
			if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
			{
				AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
				Logger.Debug("HandleMouseUp " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]);
			}
			if (!new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) && !AppHandler.mCurrentAppPackage.Equals("Home"))
			{
				if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
				{
					AppHandler.sAppPackagesCountClicks["*"]++;
				}
				if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
				{
					if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
					{
						AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
						AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
					}
					else
					{
						AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
					}
				}
			}
		}
		HandleMouseButton(e.X, e.Y, e.Button, pressed: false, force: false);
	}

	internal void HandleMouseDown(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Button == 2097152)
		{
			if (VMWindow.Instance.mIsInScriptMode)
			{
				return;
			}
			if (RegistryManager.Instance.CustomCursorEnabled && VMWindow.Instance.IsPackageAvailableForCustomCursor(AppHandler.mCurrentAppPackage))
			{
				VMWindow.Instance.ChangeCursorStyle(Constants.MOBACursorPath, isMOBACursor: true);
			}
		}
		VMWindow.Instance.UpdateUserActivityStatus();
		MacroForm.RecordMouse(e.X, e.Y, ((Form)VMWindow.Instance).ClientSize.Width, ((Form)VMWindow.Instance).ClientSize.Height, e.Button, (ActionType)2);
		if ((int)e.Button == 1048576)
		{
			Logger.Debug("left button");
			Logger.Debug("{0},{1}", new object[2] { e.X, e.Y });
			Logger.Debug("{0},{1}", new object[2]
			{
				((Form)VMWindow.Instance).ClientSize.Width,
				((Form)VMWindow.Instance).ClientSize.Height
			});
		}
		HandleMouseButton(e.X, e.Y, e.Button, pressed: true, force: false);
	}

	internal void HandleMouseMove(object sender, MouseEventArgs e)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		_ = mLastMouseMoveLocation;
		if (mLastMouseMoveLocation != e.Location)
		{
			VMWindow.Instance.UpdateUserActivityStatus();
		}
		mLastMouseMoveLocation = e.Location;
		if (Math.Abs(Environment.TickCount - VMWindow.sLastTouchTime) > 1000 && !Input.IsEventFromTouch() && AndroidBootUp.mMonitor != null)
		{
			int x = e.X;
			int y = e.Y;
			UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), 512, e.Button);
			if (VMWindow.Instance.s_KeyMapTeachMode || VMWindow.Instance.mIsInScriptMode)
			{
				GetMousePercentLocationFromPointToClient(out var x2, out var y2);
				string text = ((!VMWindow.Instance.s_KeyMapTeachMode) ? $" X: {Math.Round(x2, 2)}, Y: {Math.Round(y2, 2)}" : $"  [ x={Math.Round(x2, 2)}%, y={Math.Round(y2, 2)}% - {AppHandler.mCurrentAppPackage}]");
				VMWindow.Instance.sKeyMapToolTip.Show(text, (IWin32Window)(object)VMWindow.Instance, x + 10, y + 15, 100000);
			}
			else
			{
				VMWindow.Instance.sKeyMapToolTip.Hide((IWin32Window)(object)VMWindow.Instance);
			}
		}
	}

	internal void GetMousePercentLocationFromPointToClient(out double x1, out double y1)
	{
		x1 = (y1 = 0.0);
		Point point = ((Control)VMWindow.Instance).PointToClient(Cursor.Position);
		int x2 = point.X;
		int y2 = point.Y;
		x1 = 100.0 * (double)LayoutManager.GetGuestX(x2, y2) / 32768.0;
		y1 = 100.0 * (double)LayoutManager.GetGuestY(x2, y2) / 32768.0;
		if (LayoutManager.mEmulatedPortraitMode)
		{
			double num = x1;
			x1 *= 1f - mSoftControlBarHeightPortrait;
			if (!LayoutManager.mRotateGuest180)
			{
				x1 = y1;
				y1 = 100.0 - num;
			}
			else
			{
				x1 = 100.0 - y1;
				y1 = num;
			}
		}
		if (mSoftControlEnabled)
		{
			double num2 = y1 * (double)LayoutManager.mScaledDisplayArea.Height / 100.0;
			double num3 = (float)LayoutManager.mScaledDisplayArea.Height - mSoftControlBarHeightLandscape * (float)LayoutManager.mConfiguredDisplaySize.Height;
			y1 = num2 / num3 * 100.0;
			if (y1 > 100.0 || y1 < 0.0)
			{
				VMWindow.Instance.sKeyMapToolTip.Hide((IWin32Window)(object)VMWindow.Instance);
			}
		}
	}

	internal void HandleMouseButton(int x, int y, MouseButtons button, bool pressed, bool force)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		VMWindow.Instance.UpdateUserActivityStatus();
		if (!Input.IsEventFromTouch() && AndroidBootUp.mMonitor != null)
		{
			Logger.Debug("{3} Mouse {2} at {0}, {1}", new object[4]
			{
				((Form)VMWindow.Instance).ClientSize.Width,
				((Form)VMWindow.Instance).ClientSize.Height,
				pressed ? "down" : "up",
				((int)button == 1048576) ? "left" : "right"
			});
			if ((int)button == 2097152)
			{
				UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 516 : 517, button);
				return;
			}
			if ((int)button == 1048576)
			{
				UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 513 : 514, button);
				return;
			}
			if ((int)button == 4194304)
			{
				UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 519 : 520, button);
				return;
			}
			if ((int)button == 8388608 || (int)button == 16777216)
			{
				UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 523 : 524, button);
				return;
			}
			Mouse.Instance.UpdateButton((uint)LayoutManager.GetGuestX(x, y), (uint)LayoutManager.GetGuestY(x, y), button, pressed);
			AndroidBootUp.mMonitor.SendMouseState(Mouse.Instance.X, Mouse.Instance.Y, Mouse.Instance.Mask);
		}
	}

	internal static void Shake()
	{
		isSendSensorDeviceData = true;
		ImapShake();
	}

	internal static void StartRecording()
	{
		ImapStartRecording();
	}

	internal static void StopRecording(StringBuilder events, ref int size)
	{
		ImapStopRecording(events, ref size);
	}

	internal static void PauseRecording()
	{
		ImapPauseRecording();
	}

	internal static void SetInputMapperState(int state)
	{
		ImapSetState(state);
	}

	internal static void ToggleGamepadButton(bool isEnable)
	{
		ImapEnableGamepadButtonCb(isEnable);
	}

	internal static void GetInteractionStats(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			int numInteractions = 0;
			int numMappedInteractions = 0;
			int numTextInteractions = 0;
			int nativeGamepadUsed = 0;
			StringBuilder stringBuilder = new StringBuilder(2400);
			ImapGetInteractionStats(ref numInteractions, ref numMappedInteractions, ref numTextInteractions, ref nativeGamepadUsed, stringBuilder);
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			val.Add("s1", JToken.op_Implicit(numInteractions));
			val.Add("s2", JToken.op_Implicit(numMappedInteractions));
			val.Add("s3", JToken.op_Implicit(numTextInteractions));
			val.Add("s4", JToken.op_Implicit(stringBuilder.ToString()));
			val.Add("s5", JToken.op_Implicit(nativeGamepadUsed));
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetInteractionStats. Err : " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	internal static void EnableGamepad(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			ImapGamepadEnable(Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["enable"].ToString()));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableGamepad: " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	internal static void EnableNativeGamepadControls(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			bool mode = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["isEnabled"].ToString());
			Logger.Debug("NATIVE_GAMEPAD: IsEnabled= " + mode);
			ImapSetGamepadMode(mode ? 1 : 0);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableNativeGamepadControls: " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	internal static void ExportCfgFile(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["path"].ToString();
			if (!string.IsNullOrEmpty(Instance.mCurrentPackage))
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				File.Copy(Instance.GetInputmapperFile(Instance.mCurrentPackage), text, overwrite: true);
				HTTPHandler.WriteSuccessJson(res);
			}
			else
			{
				HTTPHandler.WriteErrorJson("App not running", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ExportCfgFile: " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	private string GetInputmapperFile(string packageName)
	{
		string result = string.Empty;
		try
		{
			string text = Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), packageName + ".cfg");
			string text2 = Path.Combine(RegistryStrings.InputMapperFolder, packageName + ".cfg");
			if (File.Exists(text))
			{
				result = text;
			}
			else if (File.Exists(text2))
			{
				result = text2;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Excpetion in GetInputMapper: " + ex.ToString());
		}
		return result;
	}

	internal static void ImportCfgFile(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["path"].ToString();
			if (!string.IsNullOrEmpty(Instance.mCurrentPackage))
			{
				Logger.Info("CFg file Selected : " + text);
				if (!Instance.IsValidCfg(text))
				{
					HTTPHandler.WriteErrorJson("Cfg not valid", res);
					return;
				}
				string destFileName = Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), Instance.mCurrentPackage + ".cfg");
				File.Copy(text, destFileName, overwrite: true);
				Instance.RefreshKeymapping(Instance.mCurrentPackage);
				HTTPHandler.WriteSuccessJson(res);
			}
			else
			{
				HTTPHandler.WriteErrorJson("App not running", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Import CfgFile: " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	internal bool IsValidCfg(string fileName)
	{
		try
		{
			if (JsonConvert.DeserializeObject(File.ReadAllText(fileName)) == null)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			Logger.Error("invalid cfg file: {0}", new object[1] { fileName });
			return false;
		}
	}

	internal bool IsScriptCommandsValid(string commandObj)
	{
		bool isValid = false;
		int errIndex = 0;
		int num = ImapValidateScriptCommands(new StringBuilder(commandObj), ref errIndex, ref isValid);
		Logger.Info($"Return value for ImapValidateScriptCommands : {num}");
		if (isValid)
		{
			Logger.Error($"Failed to parse script command : {commandObj}. Error Index : {errIndex}");
		}
		return num == 0;
	}

	internal static void EnableScrollOnEdgeFeature(bool enable)
	{
		try
		{
			Logger.Info("SCROLL_ON_EDGE: IsEnabled= " + enable);
			ImapEnableScrollOnEdge(enable);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableScrollOnEdgeFeature: " + ex.ToString());
		}
	}

	static InputMapper()
	{
		syncRoot = new object();
		syncRoot1 = new object();
		sInstance = null;
		sMapableKeyArray = new uint[42]
		{
			2u, 3u, 4u, 5u, 6u, 7u, 8u, 9u, 10u, 11u,
			16u, 17u, 18u, 19u, 20u, 21u, 22u, 23u, 24u, 25u,
			30u, 31u, 32u, 33u, 34u, 35u, 36u, 37u, 38u, 44u,
			45u, 46u, 47u, 48u, 49u, 50u, 57u, 28u, 57416u, 57424u,
			57419u, 57421u
		};
		sMapableKeySet = null;
		s_UserKeyMappingEnabled = true;
		mIsVMWindowsActivated = false;
		mIsGamepadConnected = false;
		isSendSensorDeviceData = false;
		IsMacroPlaying = false;
	}
}
