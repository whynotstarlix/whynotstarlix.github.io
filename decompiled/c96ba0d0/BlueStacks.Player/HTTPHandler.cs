using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SystemInfo;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

internal class HTTPHandler
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static ThreadStart _003C_003E9__1_0;

		public static Action _003C_003E9__27_0;

		public static Action _003C_003E9__55_0;

		public static Action _003C_003E9__55_1;

		public static Action _003C_003E9__85_0;

		public static Action _003C_003E9__93_0;

		internal void _003CStartServer_003Eb__1_0()
		{
			HttpHandlerSetup.InitHTTPServer(GetRoutes(), MultiInstanceStrings.VmName);
		}

		internal void _003COpenMacroWindow_003Eb__27_0()
		{
			((Control)MacroForm.Instance).Show();
		}

		internal void _003CSetFrontendVisibility_003Eb__55_0()
		{
			Logger.Info("Hiding frontend");
			VMWindow.Instance.HandleUserHideWindow();
		}

		internal void _003CSetFrontendVisibility_003Eb__55_1()
		{
			Logger.Info("Showing frontend");
			VMWindow.Instance.HandleUserShowWindow();
		}

		internal void _003CShowWindow_003Eb__85_0()
		{
			VMWindow.Instance.HandleUserShowWindow();
		}

		internal void _003CToggleScreen_003Eb__93_0()
		{
			LayoutManager.ToggleFullScreen();
		}
	}

	private static readonly object sPlayerShuttingDownLockObject = new object();

	private static bool mIsPlayerShuttingDown = false;

	private static bool IsDeviceProvisionReceived = false;

	[DllImport("HD-Imap-Native.dll")]
	private static extern int ImapHandleOrientation(int Orientation);

	internal static void StartServer()
	{
		Thread thread = new Thread(() =>
		{
			HttpHandlerSetup.InitHTTPServer(GetRoutes(), MultiInstanceStrings.VmName);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal static Dictionary<string, RequestHandler> GetRoutes()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected Obj, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected Obj, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected Obj, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected Obj, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected Obj, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected Obj, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected Obj, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected Obj, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected Obj, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected Obj, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected Obj, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected Obj, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected Obj, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected Obj, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected Obj, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected Obj, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected Obj, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected Obj, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected Obj, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected Obj, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected Obj, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected Obj, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected Obj, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected Obj, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected Obj, but got Unknown
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected Obj, but got Unknown
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected Obj, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Expected Obj, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected Obj, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Expected Obj, but got Unknown
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Expected Obj, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected Obj, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Expected Obj, but got Unknown
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Expected Obj, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected Obj, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected Obj, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected Obj, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected Obj, but got Unknown
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Expected Obj, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Expected Obj, but got Unknown
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Expected Obj, but got Unknown
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Expected Obj, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected Obj, but got Unknown
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Expected Obj, but got Unknown
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Expected Obj, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Expected Obj, but got Unknown
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Expected Obj, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected Obj, but got Unknown
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Expected Obj, but got Unknown
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Expected Obj, but got Unknown
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected Obj, but got Unknown
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Expected Obj, but got Unknown
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Expected Obj, but got Unknown
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Expected Obj, but got Unknown
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Expected Obj, but got Unknown
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Expected Obj, but got Unknown
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Expected Obj, but got Unknown
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Expected Obj, but got Unknown
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Expected Obj, but got Unknown
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Expected Obj, but got Unknown
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Expected Obj, but got Unknown
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Expected Obj, but got Unknown
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Expected Obj, but got Unknown
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Expected Obj, but got Unknown
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Expected Obj, but got Unknown
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Expected Obj, but got Unknown
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Expected Obj, but got Unknown
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Expected Obj, but got Unknown
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Expected Obj, but got Unknown
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected Obj, but got Unknown
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Expected Obj, but got Unknown
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Expected Obj, but got Unknown
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Expected Obj, but got Unknown
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Expected Obj, but got Unknown
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Expected Obj, but got Unknown
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Expected Obj, but got Unknown
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Expected Obj, but got Unknown
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Expected Obj, but got Unknown
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Expected Obj, but got Unknown
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Expected Obj, but got Unknown
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Expected Obj, but got Unknown
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Expected Obj, but got Unknown
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Expected Obj, but got Unknown
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Expected Obj, but got Unknown
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Expected Obj, but got Unknown
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Expected Obj, but got Unknown
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Expected Obj, but got Unknown
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Expected Obj, but got Unknown
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Expected Obj, but got Unknown
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Expected Obj, but got Unknown
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Expected Obj, but got Unknown
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Expected Obj, but got Unknown
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Expected Obj, but got Unknown
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Expected Obj, but got Unknown
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Expected Obj, but got Unknown
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Expected Obj, but got Unknown
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Expected Obj, but got Unknown
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Expected Obj, but got Unknown
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Expected Obj, but got Unknown
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Expected Obj, but got Unknown
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Expected Obj, but got Unknown
		return new Dictionary<string, RequestHandler>(StringComparer.OrdinalIgnoreCase)
		{
			{ "/ping", PingHandler },
			{ "/refreshKeymap", RefreshKeyMappingHandler },
			{ "/shutdown", Shutdown },
			{ "/switchOrientation", SwitchOrientation },
			{ "/showWindow", ShowWindow },
			{ "/refreshWindow", RefreshWindow },
			{ "/setParent", SetParent },
			{ "/shareScreenshot", ShareScreenshot },
			{ "/goBack", GoBack },
			{ "/closeScreen", CloseScreen },
			{ "/softControlBarEvent", HandleSoftControlBarEvent },
			{ "/inputMapperFilesDownloaded", InputMapperFilesDownloaded },
			{ "/enableWndProcLogging", EnableWndProcLogging },
			{ "/pingVm", PingVMHandler },
			{ "/copyFiles", SendFilesToWindows },
			{ "/getWindowsFiles", PickFilesFromWindows },
			{ "/gpsCoordinates", UpdateGpsCoordinates },
			{ "/initGamepad", InitGamePad },
			{ "/getVolume", GetProductVolume },
			{ "/setVolume", SetProductVolume },
			{ "/topDisplayedActivityInfo", TopDisplayedActivityInfo },
			{ "/appDisplayed", GetAppDisplayedInfo },
			{ "/goHome", GoHome },
			{ "/isKeyboardEnabled", IsKeyboardEnabled },
			{ "/setKeymappingState", SetKeyMappingState },
			{ "/keymap", KeyMappingHandler },
			{ "/setFrontendVisibility", SetFrontendVisibility },
			{ "/getFeSize", GetFESize },
			{ "/mute", MuteHandler },
			{ "/unmute", UnmuteHandler },
			{ "/getCurrentKeymappingStatus", IsKeyMappingEnabled },
			{ "/shake", ShakeHandler },
			{ "/isKeyNameFocussed", IsKeyNameFocussed },
			{ "/androidImeSelected", AndroidImeSelected },
			{ "/isGpsSupported", IsGPSSupported },
			{ "/installApk", InstallApk },
			{ "/injectCopy", InjectCopyHandler },
			{ "/injectPaste", InjectPasteHandler },
			{ "/stopZygote", StopZygote },
			{ "/startZygote", StartZygote },
			{ "/getKeyMappingParserVersion", KeyMappingParserVersion },
			{ "/vibrateHostWindow", VibrateHostWindowHandler },
			{ "/localeChanged", LocaleChangedHandler },
			{ "/getScreenshot", GetScreenShot },
			{ "/setPcImeWorkflow", SetPcImeWorkflow },
			{ "/setUserInfo", SetUserInfoHandler },
			{ "/getUserInfo", GetUserInfoHandler },
			{ "/getPremium", GetPremiumHandler },
			{ "/setCursorStyle", SetCursorStyle },
			{ "/openMacroWindow", OpenMacroWindow },
			{ "/startReroll", StartReroll },
			{ "/abortReroll", AbortReroll },
			{
				"/setPackagesForInteraction",
				AppHandler.SetPackagesForCountingInteractions
			},
			{ "/getInteractionForPackage", GetInteractionCountForPackages },
			{ "/toggleScreen", ToggleScreen },
			{ "/sendGlWindowSize", SendGLWindowSize },
			{ "/deactivateFrontend", DeactivateFrontend },
			{ "/startRecordingCombo", StartRecordingCombo },
			{ "/stopRecordingCombo", StopRecordingCombo },
			{ "/pauseRecordingCombo", PauseRecordingCombo },
			{ "/handleClientOperation", HandleClientOperation },
			{ "/initMacroPlayback", InitMacroPlayback },
			{ "/stopMacroPlayback", StopMacroPlayback },
			{ "/runMacroUnit", RunMacroUnit },
			{ "/farmModeHandler", FarmModeHandler },
			{ "/startOperationsSync", StartOperationsSync },
			{ "/stopOperationsSync", StopOperationsSync },
			{ "/startSyncConsumer", StartSyncConsumer },
			{ "/stopSyncConsumer", StopSyncConsumer },
			{ "/showFPS", ShowFPS },
			{ "/frontendVisibleChanged", FrontendVisibleChangedHandler },
			{ "/oneTimeSetupCompleted", OTSCompletedHandler },
			{ "/closeCrashedAppTab", CloseCrashedTabHandler },
			{ "/appDataFeUrl", SetCurrentAppData },
			{ "/runAppInfo", RunAppInfo },
			{ "/stopAppInfo", StopAppInfo },
			{ "/quitFrontend", QuitFrontend },
			{ "/toggleGamepadButton", ToggleGamepadButton },
			{ "/deviceProvisioned", DeviceProvisionedHandler },
			{ "/deviceProvisionedReceived", DeviceProvisionedReceived },
			{ "/googleSignin", GoogleSigninHandler },
			{ "/isAppPlayerRooted", IsAppPlayerRooted },
			{ "/setIsFullscreen", SetFullScreenState },
			{
				"/getInteractionStats",
				InputMapper.GetInteractionStats
			},
			{
				"/enableGamepad",
				InputMapper.EnableGamepad
			},
			{
				"/exportCfgFile",
				InputMapper.ExportCfgFile
			},
			{
				"/importCfgFile",
				InputMapper.ImportCfgFile
			},
			{ "/enableDebugLogs", EnableDebugLogs },
			{ "/reloadShortcutsConfig", ReloadShortcutsConfig },
			{ "/accountSetupCompleted", AccountSetupCompleted },
			{ "/scriptEditingModeEntered", ScriptEditingModeStateChanged },
			{ "/playPauseSync", PlayPauseSyncHandler },
			{ "/reinitGuestRegistry", ReinitGuestRegistry },
			{ "/updateMacroShortcutsDict", UpdateMacroShortcutsDict },
			{ "/setAstcOption", SetAstcOption },
			{ "/validateScriptCommands", ValidateSciptCommands },
			{ "/changeimei", ChangeImei },
			{
				"/enableNativeGamepad",
				InputMapper.EnableNativeGamepadControls
			},
			{ "/sendImagePickerCoordinates", SendImagePickerCoordinates },
			{ "/toggleImagePickerMode", ImagePickerModeStateChanged },
			{ "/handleLoadConfigOnTabSwitch", HandleLoadConfigAfterHomeSwitch },
			{ "/sendCustomCursorEnabledApps", SendCustomCursorEnabledApps },
			{ "/toggleScrollOnEdgeFeature", EnableScrollOnEdgeMode },
			{ "/forceShutdown", ForceExit },
			{ "/bootcompleted", BootCompletedHandler },
			{ "/enableMemoryTrim", EnableMemoryTrim }
		};
	}

	private static void ValidateSciptCommands(HttpListenerRequest req, HttpListenerResponse res)
	{
		bool flag = false;
		try
		{
			string commandObj = HTTPUtils.ParseRequest(req).Data["script"];
			flag = InputMapper.Instance.IsScriptCommandsValid(commandObj);
		}
		catch (Exception arg)
		{
			Logger.Error($"Failed to validate script commands. Er : {arg}");
		}
		if (flag)
		{
			WriteSuccessJson(res);
		}
		else
		{
			WriteErrorJson("invalid script", res);
		}
	}

	private static void SendImagePickerCoordinates(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			float.TryParse(val.Data["X"], out var result);
			float.TryParse(val.Data["Y"], out var result2);
			uint crc = 0u;
			Logger.Info("ImagePicker: X: " + result + "  Y:" + result2);
			Opengl.ImgdUpdateScreenPoint(ref result, ref result2, ref crc);
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"X",
					result.ToString(CultureInfo.InvariantCulture)
				},
				{
					"Y",
					result2.ToString(CultureInfo.InvariantCulture)
				},
				{
					"Crc",
					crc.ToString(CultureInfo.InvariantCulture)
				}
			};
			HTTPUtils.SendRequestToClient("updateCrc", dictionary, "Android", 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendImagePickerCoordinates. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void ImagePickerModeStateChanged(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string value = HTTPUtils.ParseRequest(req).Data["isInImagePickerMode"];
			VMWindow.Instance.ImagePickerModeStateChanged(bool.Parse(value));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ImagePickerModeStateChanged. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void HandleLoadConfigAfterHomeSwitch(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string package = HTTPUtils.ParseRequest(req).Data["package"];
			InputMapper.Instance.HandleLoadConfigAfterHomeSwitch(package);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleLoadConfigAfterHomeSwitch. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SendCustomCursorEnabledApps(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["packages"];
			if (!string.IsNullOrEmpty(text))
			{
				VMWindow.Instance.mCustomCursorAppsList = text.Split(new char[1] { ' ' }).ToList();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendCustomCursorEnabledApps. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void EnableScrollOnEdgeMode(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			bool isEnable = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["isEnabled"].ToString());
			VMWindow.Instance.ToggleScrollOnEdgeState(isEnable);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableScrollOnEdgeMode. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void EnableMemoryTrim(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			MemoryManager.CheckAndTrimAndroidMemory();
			MemoryManager.TrimMemory(false);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableMemoryTrim. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetAstcOption(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Opengl.SetAstcOption(Convert.ToInt32(HTTPUtils.ParseRequest(req).Data["AstcOption"]));
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Change Astc. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void ReinitGuestRegistry(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RegistryManager.ClearRegistryMangerInstance();
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to reinit registry. Err : " + ex.ToString());
		}
	}

	private static void PlayPauseSyncHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			bool isPause = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["pause"]);
			Logger.Info("isPause : " + isPause);
			OperationsSyncManager.Instance.PlayPauseOperationsSync(isPause);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to play pause sync. Err : " + ex.ToString());
		}
	}

	private static void ScriptEditingModeStateChanged(HttpListenerRequest req, HttpListenerResponse res)
	{
		string value = HTTPUtils.ParseRequest(req).Data["isInScriptMode"];
		VMWindow.Instance.ClientScriptModeStateChanged(bool.Parse(value));
	}

	private static void AccountSetupCompleted(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			HTTPUtils.SendRequestToClient("accountSetupCompleted", (Dictionary<string, string>)null, val.RequestVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AccountSetupCompleted Handler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StopOperationsSync(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			HTTPUtils.ParseRequest(req);
			OperationsSyncManager.Instance.StopOperationsSync();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopOperationsSync. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StartOperationsSync(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string[] instances = HTTPUtils.ParseRequest(req).Data["instances"].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			OperationsSyncManager.Instance.StartSyncToInstances(instances);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StartOperationsSync. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StartSyncConsumer(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string fromInstance = HTTPUtils.ParseRequest(req).Data["instance"];
			OperationsSyncManager.Instance.StartSyncConsumer(fromInstance);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StartSyncConsumer. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StopSyncConsumer(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			OperationsSyncManager.Instance.StopSyncConsumer();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopSyncConsumer. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void ShowFPS(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Opengl.ShowFPS(int.Parse(HTTPUtils.ParseRequest(req).Data["isshowfps"]));
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Change FPS. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void HandleClientOperation(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string formattedString = HTTPUtils.ParseRequest(req).Data["operationData"];
			InputMapper.Instance.SendClientString(formattedString);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleClientOperation. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void GetInteractionCountForPackages(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
			{
				string key = string.Empty;
				long num = long.MinValue;
				foreach (KeyValuePair<string, long> sDictCountClick in AppHandler.sDictCountClicks)
				{
					if (sDictCountClick.Value > num)
					{
						num = sDictCountClick.Value;
						key = sDictCountClick.Key.ToString();
					}
				}
				if (!AppHandler.sAppPackagesCountClicks.ContainsKey(key))
				{
					AppHandler.sAppPackagesCountClicks.Add(key, num);
				}
				else
				{
					AppHandler.sAppPackagesCountClicks[key] = num;
				}
				if (AppHandler.sDictCountClicks.Count == 0)
				{
					AppHandler.sAppPackagesCountClicks["?"] = 0L;
				}
				else
				{
					AppHandler.sAppPackagesCountClicks["?"] = num;
				}
			}
			HTTPUtils.Write(JsonConvert.SerializeObject((object)AppHandler.sAppPackagesCountClicks, (Formatting)0).ToString(), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetInteractionCountForPackages... Err : " + ex.ToString());
		}
	}

	private static void EnableWndProcLogging(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.isLogWndProc = !VMWindow.isLogWndProc;
			Logger.Debug("Got request for EnableWndProcLogging" + VMWindow.isLogWndProc);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set EnableWndProcLogging... Err : " + ex.ToString());
		}
	}

	private static void EnableDebugLogs(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Logger.EnableDebugLogs();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void ReloadShortcutsConfig(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.Instance.mShortcutConfig = ShortcutConfig.LoadShortcutsConfig();
			if (VMWindow.Instance.mShortcutConfig != null)
			{
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("ShortcutConfig is null, see inner exception", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReloadShortcutsConfig... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void SetPcImeWorkflow(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.Instance.SetPcImeWorkflow(isSet: true);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set PC Ime workflow... Err : " + ex.ToString());
		}
	}

	private static void SetCursorStyle(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected Obj, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string path = val.Data["path"];
			if (path != null)
			{
				UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
				{
					VMWindow.Instance.ChangeCursorStyle(path);
				}));
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("Path is invalid", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SetCursorStyle. Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void OpenMacroWindow(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		try
		{
			VMWindow instance = VMWindow.Instance;
			Action val = _003C_003Ec._003C_003E9__27_0;
			if (val == null)
			{
				Action val2 = () =>
				{
					((Control)MacroForm.Instance).Show();
				};
				_003C_003Ec._003C_003E9__27_0 = val2;
				val = val2;
			}
			UIHelper.RunOnUIThread((Control)(object)instance, val);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in OpenMacroWindow: " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void StartReroll(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		try
		{
			UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
			{
				RequestData val = HTTPUtils.ParseRequest(req);
				string packageName = val.Data["packageName"];
				string? macroName = val.Data["rerollName"];
				MacroData.Instance.LoadMacroData(packageName);
				MacroForm.PlayMacro(macroName);
			}));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in OpenMacroWindow. Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void AbortReroll(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			MacroForm.AbortReroll();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AbortReroll. Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void GetPremiumHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			bool isPremium = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["value"]);
			Logger.Info("Is User Premium : " + isPremium);
			RegistryManager.Instance.IsPremium = isPremium;
			WriteSuccessJson(res);
			string[] vmList = RegistryManager.Instance.VmList;
			foreach (string text in vmList)
			{
				try
				{
					HTTPUtils.SendRequestToClient("updateUserInfo", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in validating acc icon for vm. Err : " + text + "... Reason : " + ex.ToString());
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in GetPremiumHandler. Err : " + ex2.ToString());
			WriteErrorJson(ex2.ToString(), res);
		}
	}

	private static void GetUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		try
		{
			JObject val = new JObject();
			val.Add("email", JToken.op_Implicit(RegistryManager.Instance.RegisteredEmail));
			val.Add("token", JToken.op_Implicit(RegistryManager.Instance.Token));
			val.Add("state", JToken.op_Implicit(RegistryManager.Instance.IsPremium ? "PAID" : "AD-SUPPORTED"));
			val.Add("success", JToken.op_Implicit(true));
			JObject val2 = val;
			Logger.Info("sending json : " + ((JToken)val2).ToString((Formatting)0, new JsonConverter[0]));
			HTTPUtils.Write(((object)val2).ToString(), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetUserInfoHandler... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void SetUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = val.Data["result"].Trim();
			string text2 = val.Data["error_msg"];
			dictionary.Add("result", text);
			if (text.Equals("false", StringComparison.InvariantCultureIgnoreCase))
			{
				Logger.Info("Error message in receiving token from android: " + text2);
			}
			if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				string text3 = val.Data["user_info"];
				JObject val2 = JObject.Parse(text3);
				Logger.Info("User info : " + text3);
				string text4 = ((object)val2["email"]).ToString().Trim();
				if (!string.Equals(RegistryManager.Instance.RegisteredEmail, text4, StringComparison.InvariantCultureIgnoreCase))
				{
					RegistryManager.Instance.RegisteredEmail = text4;
					RegistryManager.Instance.Token = ((object)val2["token"]).ToString().Trim();
					RegistryManager.Instance.IsPremium = ((object)val2["state"]).ToString().Equals("PAID", StringComparison.InvariantCultureIgnoreCase);
					Stats.SendUnifiedInstallStatsAsync("bluestacks_login_completed", text4);
				}
			}
			HTTPUtils.SendRequestToClient("updateUserInfo", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SetUserInfoHandler... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void CloseCrashedTabHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					val.Data[text]
				});
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"package",
				val.Data["package"]
			} };
			HTTPUtils.SendRequestToClient("closeCrashedAppTab", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in CloseCrashedTabHandler... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void OTSCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			HTTPUtils.SendRequestToClient("oneTimeSetupCompleted", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in OTSCompletedHandler... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void FrontendVisibleChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			foreach (string key in val.Data.Keys)
			{
				Logger.Warning("{0} {1}", new object[2]
				{
					key,
					val.Data[key]
				});
			}
			bool flag = Convert.ToBoolean(val.Data["new_value"]);
			bool flag2 = Convert.ToBoolean(val.Data["is_mute"]);
			if (flag)
			{
				if (!flag2)
				{
					MediaManager.UnmuteEngine();
				}
			}
			else
			{
				MediaManager.MuteEngine();
			}
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in FrontendVisibleChangedHandler... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void GetScreenShot(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected Obj, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string path = val.Data["path"];
			string text = val.Data["showSavedInfo"];
			if (text == null)
			{
				text = "true";
			}
			if (!bool.TryParse(text, out var showSaved))
			{
				showSaved = true;
			}
			string extension = Path.GetExtension(path);
			if (path != null && (extension == ".jpeg" || extension == ".jpg"))
			{
				UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
				{
					VMWindow.Instance.SaveScreenShot(path, showSaved);
				}));
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("Path is invalid", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetScreenShot. Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void LocaleChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			if (Features.IsFeatureEnabled(4194304uL))
			{
				WriteSuccessJson(res);
				return;
			}
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestedLocale = val.Data["result"];
			if (Globalization.sSupportedLocales.Keys.ToList().FindIndex((string x) => x.Equals(requestedLocale, StringComparison.InvariantCultureIgnoreCase)) == -1)
			{
				Logger.Warning("We do not support {0}, finding closest match", new object[1] { requestedLocale });
				requestedLocale = Globalization.FindClosestMatchingLocale(requestedLocale);
				string text = HTTPUtils.SendRequestToGuest("setLocale", new Dictionary<string, string> { { "arg", requestedLocale } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("Response for setlocale from guest : " + text);
			}
			Logger.Info("Setting locale: {0}", new object[1] { requestedLocale });
			RegistryManager.Instance.UserSelectedLocale = requestedLocale;
			RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].Locale = requestedLocale;
			Utils.UpdateValueInBootParams("LANG", requestedLocale, MultiInstanceStrings.VmName, false, "bgp64");
			HTTPUtils.SendRequestToClientAsync("androidLocaleChanged", new Dictionary<string, string> { { "locale", requestedLocale } }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AndroidLocalechanged. Err : " + ex.ToString());
		}
	}

	private static void VibrateHostWindowHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected Obj, but got Unknown
		try
		{
			new Dictionary<string, string>();
			int num = JObject.Parse(HTTPUtils.ParseRequest(req).Data["result"])["duration"].ToObject<int>();
			if (VMWindow.Instance.cSysInfo == null)
			{
				VMWindow.Instance.cSysInfo = new CSysInfo();
			}
			if (VMWindow.Instance.cSysInfo.JoystickIsConnected() == 1)
			{
				VMWindow.Instance.cSysInfo.SetJoystickVibration(num);
			}
			if (VMWindow.Instance.mIsKBVibrationDllLoaded)
			{
				MsiVibration.SetVibration(num);
			}
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception when sending vibration to MSI. Err : {0}", new object[1] { ex.ToString() });
		}
	}

	private static void KeyMappingParserVersion(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string keyMappingParserVersion = InputMapper.GetKeyMappingParserVersion();
			JObject val = new JObject();
			val.Add("parserversion", JToken.op_Implicit(keyMappingParserVersion));
			val.Add("success", JToken.op_Implicit(true));
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server KeyMappingParserVersion. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StartZygote(HttpListenerRequest req, HttpListenerResponse res)
	{
		RequestData val = HTTPUtils.ParseRequest(req);
		Logger.Info("Got request for startzygote for vm : " + val.Data["vmName"]);
		string vmName = val.Data["vmName"];
		try
		{
			Opengl.StartZygote(vmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StartZygote. Err : " + ex.ToString());
		}
	}

	private static void StopZygote(HttpListenerRequest req, HttpListenerResponse res)
	{
		RequestData val = HTTPUtils.ParseRequest(req);
		Logger.Info("Got request for stopzygote for vm : " + val.Data["vmName"]);
		string vmName = val.Data["vmName"];
		try
		{
			Opengl.StopZygote(vmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopZygote. Err : " + ex.ToString());
		}
	}

	private static void InjectPasteHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			SendKeys.SendWait("^V");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending ctrl + v. Err : " + ex.ToString());
		}
	}

	private static void InjectCopyHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			SendKeys.SendWait("^C");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending ctrl + c. Err : " + ex.ToString());
		}
	}

	private static void InstallApk(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected Obj, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			try
			{
				OpenFileDialog val = new OpenFileDialog
				{
					Filter = "Android Files (*.apk, *.xapk) | *.apk; *.xapk"
				};
				if ((int)((CommonDialog)val).ShowDialog((IWin32Window)(object)VMWindow.Instance) == 1)
				{
					Logger.Info("File Selected : " + ((FileDialog)val).FileName);
					string apkPath = ((FileDialog)val).FileName;
					Logger.Info("Installing apk: {0}", new object[1] { apkPath });
					ThreadPool.QueueUserWorkItem((object obj) =>
					{
						Utils.CallApkInstaller(apkPath, false);
					});
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in getting install apk. Err : " + ex.ToString());
				WriteErrorJson(ex.ToString(), res);
			}
		}));
	}

	private static void StopAppInfo(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["appPackage"];
			Logger.Info("Received stop app package = {0}", new object[1] { text });
			if (!string.IsNullOrEmpty(text) && AppHandler.sLastAppDisplayed.Contains(text))
			{
				AppHandler.sLastAppDisplayed = "";
				Logger.Info("assigned empty value to sLastAppDisplayed");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopAppInfo. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void RunAppInfo(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["appPackage"];
			Logger.Info("Received appPackage = {0}", new object[1] { text });
			if (!string.IsNullOrEmpty(text))
			{
				AppHandler.sAppPackage = text;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in RunAppInfo. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void IsGPSSupported(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			if (SystemUtils.IsOSWinXP() || SystemUtils.IsOSWin7() || SystemUtils.IsOSVista())
			{
				WriteErrorJson("not supported", res);
			}
			else if (RegistryManager.Instance.DefaultGuest.GPSAvailable == 1)
			{
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("not supported", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in getting GPS device status. Err : " + ex.ToString());
			WriteErrorJson("not supported", res);
		}
	}

	private static void AndroidImeSelected(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string? text = HTTPUtils.ParseRequest(req).Data["result"];
			if (!text.Equals("com.android.inputmethod.latin/.LatinIME"))
			{
				Logger.Info("Android Ime Selected in not latinIme");
				VMWindow.Instance.ChangeImeMode(enableIme: false);
			}
			Utils.SetImeSelectedInReg(text, MultiInstanceStrings.VmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AndroidImeSelected. Err : " + ex.ToString());
		}
	}

	private static void IsKeyNameFocussed(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["state"].Trim();
			Logger.Info("The focussed state is " + text);
			if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				VMWindow.Instance.ChangeImeMode(enableIme: false);
			}
			else
			{
				VMWindow.Instance.ChangeImeMode(enableIme: true);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception Occured in IsKeyNameFocussed. Err : " + ex.ToString());
		}
	}

	private static void ShakeHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			InputMapper.Shake();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured in ShakeHandler. Err : {ex.ToString()}");
			WriteErrorJson("Error in api", res);
		}
	}

	private static void IsKeyMappingEnabled(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			bool flag = !VMWindow.Instance.mIsTextInputBoxInFocus && InputMapper.s_UserKeyMappingEnabled;
			val.Add("keymapping", JToken.op_Implicit(flag));
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured IsKeyMappingEnabled. Err : {ex.ToString()}");
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void UnmuteHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		RequestData val = HTTPUtils.ParseRequest(req);
		if (!string.IsNullOrEmpty(val.Data["allInstances"]))
		{
			MediaManager.UnmuteEngine(!Convert.ToBoolean(val.Data["allInstances"]));
		}
	}

	private static void MuteHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		RequestData val = HTTPUtils.ParseRequest(req);
		string? value = val.Data["allInstances"];
		string value2 = val.Data["explicit"];
		bool result = true;
		if (!string.IsNullOrEmpty(value2) && !bool.TryParse(value2, out result))
		{
			result = true;
		}
		if (!string.IsNullOrEmpty(value))
		{
			MediaManager.MuteEngine(result, !Convert.ToBoolean(val.Data["allInstances"]));
		}
	}

	private static void GetFESize(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			JArray val = new JArray();
			JObject val2 = new JObject();
			val2.Add("success", JToken.op_Implicit(true));
			val2.Add("Height", JToken.op_Implicit(((Form)VMWindow.Instance).Size.Height));
			val2.Add("Width", JToken.op_Implicit(((Form)VMWindow.Instance).Size.Width));
			val2.Add("ClientHeight", JToken.op_Implicit(((Form)VMWindow.Instance).ClientSize.Height));
			val2.Add("ClientWidth", JToken.op_Implicit(((Form)VMWindow.Instance).ClientSize.Width));
			JObject val3 = val2;
			val.Add((JToken)(object)val3);
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server GetFESize. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetFrontendVisibility(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected Obj, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected Obj, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string text = val.Data["visible"];
			string text2 = val.Data["appPackage"];
			Logger.Info("Received visible = {0} and appPackage = {1}", new object[2] { text, text2 });
			if (!string.IsNullOrEmpty(text2))
			{
				AppHandler.sAppPackage = text2;
			}
			if (string.Compare(text, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				VMWindow instance = VMWindow.Instance;
				Action val2 = _003C_003Ec._003C_003E9__55_0;
				if (val2 == null)
				{
					Action val3 = () =>
					{
						Logger.Info("Hiding frontend");
						VMWindow.Instance.HandleUserHideWindow();
					};
					_003C_003Ec._003C_003E9__55_0 = val3;
					val2 = val3;
				}
				UIHelper.RunOnUIThread((Control)(object)instance, val2);
			}
			else
			{
				VMWindow instance2 = VMWindow.Instance;
				Action val4 = _003C_003Ec._003C_003E9__55_1;
				if (val4 == null)
				{
					Action val5 = () =>
					{
						Logger.Info("Showing frontend");
						VMWindow.Instance.HandleUserShowWindow();
					};
					_003C_003Ec._003C_003E9__55_1 = val5;
					val4 = val5;
				}
				UIHelper.RunOnUIThread((Control)(object)instance2, val4);
			}
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SetFrontendVisibility. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void RefreshKeyMappingHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string appPkg = null;
			try
			{
				appPkg = HTTPUtils.ParseRequest(req).Data["package"].ToString();
			}
			catch
			{
			}
			InputMapper.Instance.RefreshKeyMapping(appPkg);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured in RefreshKeyMappingHandler. Err : {ex.ToString()}");
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void KeyMappingHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			InputMapper.Instance.LaunchBlueStacksKeyMapper();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured in KeyMappingHandler. Err : {ex.ToString()}");
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetKeyMappingState(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (string.Compare(HTTPUtils.ParseRequest(req).Data["keymapping"], "true", ignoreCase: true) == 0)
			{
				InputMapper.s_UserKeyMappingEnabled = true;
				VMWindow.Instance.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_MAPPING;
			}
			else
			{
				InputMapper.s_UserKeyMappingEnabled = false;
				VMWindow.Instance.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_RAW;
			}
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured in SetKeyMappingState. Err : {ex.ToString()}");
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void IsKeyboardEnabled(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected Obj, but got Unknown
		try
		{
			if (string.Compare(HTTPUtils.ParseRequest(req).Data["isinput"], "true", ignoreCase: true) == 0)
			{
				Logger.Info("calling change ime with true");
				VMWindow.Instance.ChangeImeMode(enableIme: true);
			}
			else
			{
				Logger.Info("calling change ime with false");
				VMWindow.Instance.ChangeImeMode(enableIme: false);
			}
			if (!InputMapper.s_UserKeyMappingEnabled)
			{
				WriteErrorJson("Cannot entertain this request as the user/keymappingtool has disabled keymapping, Will force this value when user enables keymapping", res);
				return;
			}
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			HTTPUtils.Write(((object)val).ToString(), res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured IsKeyboardEnabled. Err : {ex.ToString()}");
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void GoHome(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VmCmdHandler.RunCommand("home", MultiInstanceStrings.VmName);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error($"Exception Occured in GoHome. Err: {ex.ToString()}");
			WriteErrorJson("unable to go home", res);
		}
	}

	private static void GetAppDisplayedInfo(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		JArray val = new JArray();
		JObject val2 = new JObject();
		val2.Add("success", JToken.op_Implicit(true));
		val2.Add("LastAppDisplayed", JToken.op_Implicit(AppHandler.sLastAppDisplayed));
		JObject val3 = val2;
		val.Add((JToken)(object)val3);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
	}

	private static void TopDisplayedActivityInfo(HttpListenerRequest req, HttpListenerResponse res)
	{
		string text = "";
		string text2 = "";
		try
		{
			text = HTTPUtils.ParseRequest(req).Data["appToken"];
			Logger.Info("appToken = " + text);
			if (text.IndexOf("com.bluestacks.appmart") != -1)
			{
				Logger.Info("BOOT_STAGE: Sending boot completed event received from TopDisplayedActivityInfo");
				AndroidBootUp.GuestBootCompletedEvent(null, new EventArgs());
			}
			if (text.IndexOf("com.bluestacks.keymappingtool") == -1)
			{
				string[] separator = new string[1] { "ActivityRecord" };
				text2 = text.Split(separator, StringSplitOptions.None)[1];
				separator = new string[1] { "u0 " };
				text2 = text2.Split(separator, StringSplitOptions.None)[1].Replace("}", "");
				text2 = text2.Split(new char[1] { ' ' })[0];
				string value = text2;
				string text3 = text2.Split(new char[1] { '/' })[0];
				InputMapper.Instance.SetPackage(text3);
				AppHandler.mCurrentAppPackage = text3;
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "token", text },
					{ "packageName", text3 },
					{ "appDisplayed", value }
				};
				AppHandler.mCurrentAppPackage = text3;
				HTTPUtils.SendRequestToClient("appDisplayed", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			if (!string.IsNullOrEmpty(text2) && string.Compare(text2, AppHandler.sLastAppDisplayed, ignoreCase: true) != 0)
			{
				Logger.Info("appDisplayed = {0}, s_Console.sLastAppDisplayed= {1}", new object[2]
				{
					text2,
					AppHandler.sLastAppDisplayed
				});
				lock (AppHandler.sCurrentAppDisplayedLockObject)
				{
					AppHandler.sLastAppDisplayed = text2;
				}
			}
			HTTPUtils.Write("true", res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server TopDisplayedActivityInfo appToken = {0}. Err : {1}", new object[2]
			{
				text,
				ex.ToString()
			});
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetCurrentAppData(HttpListenerRequest req, HttpListenerResponse res)
	{
		AppHandler.SetCurrentAppData(req, res);
	}

	private static void SetProductVolume(HttpListenerRequest req, HttpListenerResponse res)
	{
		if (MediaManager.SetGuestVolume(Convert.ToInt32(HTTPUtils.ParseRequest(req).Data["vol"])))
		{
			WriteSuccessJson(res);
		}
		else
		{
			WriteErrorJson("", res);
		}
	}

	private static void GetProductVolume(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		int guestVolume = MediaManager.GetGuestVolume(HTTPUtils.ParseRequest(req).Data["mediatype"]);
		if (guestVolume != -1)
		{
			JArray val = new JArray();
			JObject val2 = new JObject();
			val2.Add("success", JToken.op_Implicit(true));
			val2.Add("volume", JToken.op_Implicit(guestVolume));
			JObject val3 = val2;
			val.Add((JToken)(object)val3);
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		else
		{
			WriteErrorJson("", res);
		}
	}

	private static void InitGamePad(HttpListenerRequest req, HttpListenerResponse res)
	{
	}

	private static void UpdateGpsCoordinates(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			int gpsMode = RegistryManager.Instance.DefaultGuest.GpsMode;
			int gpsSource = RegistryManager.Instance.DefaultGuest.GpsSource;
			string gpsLatitude = RegistryManager.Instance.DefaultGuest.GpsLatitude;
			string gpsLongitude = RegistryManager.Instance.DefaultGuest.GpsLongitude;
			if (gpsMode == 0 || gpsSource == 0 || (gpsSource != 8 && IsWindows7AndBelow()))
			{
				Logger.Info($"Stopping Gps Service, gpsMode = {gpsMode}, gpsSource = {gpsSource}, IsWindows7AndBelow() = {IsWindows7AndBelow()}");
				Logger.Info("No Coordinates Available so far");
				HTTPUtils.Write("", res);
			}
			else
			{
				HTTPUtils.Write(gpsLatitude + "," + gpsLongitude, res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error(string.Format("Exception Occured in UpdateGpsCoordinates. Err : ", ex.ToString()));
			HTTPUtils.Write("exception", res);
		}
	}

	private static bool IsWindows7AndBelow()
	{
		Version version = new Version(6, 2, 9200, 0);
		if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= version)
		{
			return false;
		}
		return true;
	}

	private static void PickFilesFromWindows(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		RequestData requestData = HTTPUtils.ParseRequest(req);
		bool? isFileListEmpty = null;
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected Obj, but got Unknown
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected Obj, but got Unknown
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string[] allKeys = requestData.Data.AllKeys;
				foreach (string text in allKeys)
				{
					Logger.Info($"Key = {text}, Value = {requestData.Data[text]}");
				}
				string bstSharedFolder = RegistryStrings.SharedFolderDir;
				string text2 = "";
				OpenFileDialog val = new OpenFileDialog();
				if (string.Compare(requestData.Data["filesNo"].ToUpper(), "MULTIPLE") == 0)
				{
					val.Multiselect = true;
				}
				((FileDialog)val).InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				if (requestData.Data["mimeType"].ToUpper().Contains("VIDEO") || requestData.Data["mimeType"].ToUpper().Contains("AUDIO"))
				{
					text2 = "Video & Audio Files | *.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;*.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v;*.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4;*.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg;*.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec;*.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.mp3";
				}
				else if (requestData.Data["mimeType"].ToUpper().Contains("IMAGE"))
				{
					text2 = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
					((FileDialog)val).InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				}
				else if (text2.Length < 1)
				{
					text2 = "All Files|*.*";
				}
				((FileDialog)val).Filter = text2;
				JObject json = new JObject();
				Dictionary<string, string> data = new Dictionary<string, string> { { "action", "com.bluestacks.FILES_FROM_WINDOWS_SERVICE" } };
				string folderName = Utils.CreateRandomBstSharedFolder(bstSharedFolder);
				bstSharedFolder = Path.Combine(bstSharedFolder, folderName);
				DialogResult result = ((CommonDialog)val).ShowDialog((IWin32Window)(object)VMWindow.Instance);
				string[] str = ((FileDialog)val).FileNames;
				ThreadPool.QueueUserWorkItem((object obj3) =>
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Invalid comparison between Unknown and I4
					try
					{
						if ((int)result == 1)
						{
							string text3 = "";
							for (int j = 0; j < str.Length; j++)
							{
								text3 = Path.GetFileName(str[j]);
								Logger.Info($"Copying : {str[j]}  to {Path.Combine(bstSharedFolder, text3)}");
								File.Copy(str[j], Path.Combine(bstSharedFolder, text3));
								File.SetAttributes(Path.Combine(bstSharedFolder, text3), FileAttributes.Normal);
							}
							json.Add("success", JToken.op_Implicit(true.ToString()));
							json.Add("files", JToken.op_Implicit(folderName));
							isFileListEmpty = false;
						}
						else
						{
							json.Add("success", JToken.op_Implicit(false));
							try
							{
								Directory.Delete(bstSharedFolder, recursive: true);
							}
							catch
							{
							}
							isFileListEmpty = true;
						}
						data.Add("extras", ((object)json).ToString());
						HTTPUtils.SendRequestToGuest("customStartService", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 10, 500, "bgp64");
					}
					catch (Exception ex2)
					{
						isFileListEmpty = true;
						Logger.Error($"Error Occured while calling to service, Err: {ex2.ToString()}");
					}
				});
			}
			catch (Exception ex)
			{
				isFileListEmpty = true;
				Logger.Error($"Error Occured while calling to service, Err: {ex.ToString()}");
			}
		}));
		try
		{
			int num = 300;
			while (num >= 0 && !isFileListEmpty.HasValue)
			{
				Thread.Sleep(2000);
				num--;
			}
			if (!isFileListEmpty.HasValue || isFileListEmpty.Value)
			{
				HTTPUtils.Write("false", res);
			}
			else
			{
				HTTPUtils.Write("true", res);
			}
		}
		catch
		{
		}
	}

	private static void SendFilesToWindows(HttpListenerRequest req, HttpListenerResponse res)
	{
		RequestData val = HTTPUtils.ParseRequest(req);
		try
		{
			HTTPUtils.Write("true", res);
		}
		catch
		{
		}
		if (val.Data.Count > 1)
		{
			SendMultipleFilesToWindows(val, res);
		}
		else
		{
			SendSingleFileToWindows(val, res);
		}
	}

	public static void SendMultipleFilesToWindows(RequestData requestData, HttpListenerResponse res)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected Obj, but got Unknown
		string responseStringSuccess = null;
		string responseStringFailure = null;
		string bstSharedFolder = RegistryStrings.SharedFolderDir;
		DialogResult result;
		string userCopyDir;
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			FolderBrowserDialog val = new FolderBrowserDialog
			{
				ShowNewFolderButton = true,
				Description = "Choose folder to copy files",
				RootFolder = Environment.SpecialFolder.MyComputer
			};
			result = ((CommonDialog)val).ShowDialog((IWin32Window)(object)VMWindow.Instance);
			userCopyDir = val.SelectedPath;
			ThreadPool.QueueUserWorkItem((object obj) =>
			{
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0194: Invalid comparison between Unknown and I4
				if (((object)result/*cast due to constrained. prefix*/).Equals((object)(DialogResult)1))
				{
					Logger.Debug($"User Select {userCopyDir} Directory");
					string[] allKeys = requestData.Data.AllKeys;
					foreach (string text in allKeys)
					{
						try
						{
							Logger.Info("Key: {0}, Value: {1}", new object[2]
							{
								text,
								requestData.Data[text]
							});
							if (File.Exists(Path.Combine(bstSharedFolder, requestData.Data[text].Trim())))
							{
								if (string.Compare(userCopyDir, bstSharedFolder, ignoreCase: false) == 0)
								{
									goto IL_025b;
								}
								if (!File.Exists(Path.Combine(userCopyDir, requestData.Data[text].Trim())))
								{
									goto IL_0212;
								}
								result = MessageBox.Show((IWin32Window)(object)VMWindow.Instance, $"Overwrite {requestData.Data[text].Trim()}?", "File already exists", (MessageBoxButtons)4, (MessageBoxIcon)64);
								if ((int)result != 7)
								{
									if (File.Exists(Path.Combine(userCopyDir, requestData.Data[text].Trim())))
									{
										File.Delete(Path.Combine(userCopyDir, requestData.Data[text].Trim()));
									}
									goto IL_0212;
								}
								File.Delete(Path.Combine(bstSharedFolder, requestData.Data[text].Trim()));
							}
							else
							{
								if (responseStringFailure == null)
								{
									responseStringFailure = Path.Combine(userCopyDir, requestData.Data[text].Trim());
								}
								else
								{
									responseStringFailure = responseStringFailure + "\n" + Path.Combine(userCopyDir, requestData.Data[text].Trim());
								}
								Logger.Error($"{requestData.Data[text]} does not exist in sharedfolder");
							}
							goto end_IL_00bb;
							IL_025b:
							if (responseStringSuccess == null)
							{
								responseStringSuccess = Path.Combine(userCopyDir, requestData.Data[text].Trim());
							}
							else
							{
								responseStringSuccess = responseStringSuccess + "\n" + Path.Combine(userCopyDir, requestData.Data[text].Trim());
							}
							goto end_IL_00bb;
							IL_0212:
							File.Move(Path.Combine(bstSharedFolder, requestData.Data[text].Trim()), Path.Combine(userCopyDir, requestData.Data[text].Trim()));
							goto IL_025b;
							end_IL_00bb:;
						}
						catch (Exception ex)
						{
							Logger.Error($"Error Occured, Err: {ex.ToString()}");
							if (responseStringFailure == null)
							{
								responseStringFailure = Path.Combine(userCopyDir, requestData.Data[text].Trim());
							}
							else
							{
								responseStringFailure = responseStringFailure + "\n" + Path.Combine(userCopyDir, requestData.Data[text].Trim());
							}
						}
					}
					if (responseStringSuccess != null)
					{
						SendSysTrayNotification("Successfully copied files:", "success", responseStringSuccess);
					}
					if (responseStringFailure != null)
					{
						SendSysTrayNotification("Cannot copy files:", "error", responseStringFailure);
					}
				}
				else
				{
					Logger.Info("User cancelled browser dialog");
					string[] allKeys = requestData.Data.AllKeys;
					foreach (string name in allKeys)
					{
						try
						{
							File.Delete(Path.Combine(bstSharedFolder, requestData.Data[name].Trim()));
						}
						catch (Exception ex2)
						{
							Logger.Error($"Error Occured, Err : {ex2.ToString()}");
						}
					}
				}
			});
		}));
	}

	public static void SendSingleFileToWindows(RequestData requestData, HttpListenerResponse res)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected Obj, but got Unknown
		DialogResult result = (DialogResult)2;
		string fileKey = null;
		string[] allKeys = requestData.Data.AllKeys;
		int num = 0;
		if (num < allKeys.Length)
		{
			string text = allKeys[num];
			fileKey = text;
		}
		string responseStringSuccess = null;
		string responseStringFailure = null;
		string bstSharedFolder = RegistryStrings.SharedFolderDir;
		string fileName = null;
		if (fileKey == null)
		{
			return;
		}
		string ext = Path.GetExtension(requestData.Data[fileKey]).Replace(".", "");
		SaveFileDialog fileSaver;
		string userDefinedName;
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected Obj, but got Unknown
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			Logger.Debug($"File Extension = {ext}");
			SaveFileDialog val = new SaveFileDialog();
			((FileDialog)val).Filter = ext + " files (*." + ext + ")| *." + ext;
			((FileDialog)val).AddExtension = true;
			((FileDialog)val).Title = "Save File";
			((FileDialog)val).AutoUpgradeEnabled = true;
			((FileDialog)val).CheckPathExists = true;
			((FileDialog)val).DefaultExt = ext;
			((FileDialog)val).InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			((FileDialog)val).FileName = requestData.Data[fileKey];
			val.OverwritePrompt = true;
			((FileDialog)val).ValidateNames = true;
			fileSaver = val;
			result = ((CommonDialog)fileSaver).ShowDialog((IWin32Window)(object)VMWindow.Instance);
			fileName = ((FileDialog)fileSaver).FileName;
			ThreadPool.QueueUserWorkItem((object obj) =>
			{
				if (!((object)result/*cast due to constrained. prefix*/).Equals((object)(DialogResult)1))
				{
					Logger.Info("User cancelled save file dialog");
					try
					{
						File.Delete(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim()));
						return;
					}
					catch (Exception ex)
					{
						Logger.Error($"Error Occured, Err : {ex.ToString()}");
						return;
					}
				}
				userDefinedName = fileName;
				Logger.Info($"User Selected {userDefinedName} Path");
				try
				{
					Logger.Info("Key: {0}, Value: {1}", new object[2]
					{
						fileKey,
						requestData.Data[fileKey]
					});
					if (File.Exists(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim())))
					{
						if (string.Compare(userDefinedName, Path.Combine(bstSharedFolder, requestData.Data[fileKey]), ignoreCase: false) != 0)
						{
							if (File.Exists(userDefinedName))
							{
								File.Delete(userDefinedName);
							}
							File.Move(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim()), userDefinedName);
						}
						responseStringSuccess = userDefinedName;
					}
					else
					{
						responseStringFailure = userDefinedName;
						Logger.Error($"{requestData.Data[fileKey]} does not exist in sharedfolder");
					}
				}
				catch (Exception ex2)
				{
					Logger.Error($"Error Occured, Err: {ex2.ToString()}");
					responseStringFailure = userDefinedName;
				}
				if (responseStringSuccess != null)
				{
					SendSysTrayNotification("Successfully copied files:", "success", responseStringSuccess);
				}
				else if (responseStringFailure != null)
				{
					SendSysTrayNotification("Cannot copy files:", "error", responseStringFailure);
				}
			});
		}));
	}

	public static void SendSysTrayNotification(string title, string status, string message)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "message", message },
			{ "title", title },
			{ "status", status }
		};
		HTTPUtils.SendRequestToAgentAsync("showTrayNotification", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	private static void PingVMHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		JArray val = new JArray();
		JObject val2 = new JObject();
		try
		{
			if (HTTPUtils.ParseRequest(req).RequestVmName.Equals(MultiInstanceStrings.VmName))
			{
				bool flag = false;
				try
				{
					/*Error near IL_0028: Invalid metadata token*/;
				}
				catch (AbandonedMutexException)
				{
					flag = true;
				}
				catch
				{
				}
				if (flag)
				{
					val2.Add("success", JToken.op_Implicit(false));
					val2.Add("status", JToken.op_Implicit("stopped"));
					val.Add((JToken)(object)val2);
					HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
				}
				else if (Utils.CheckIfGuestReady(MultiInstanceStrings.VmName, 1))
				{
					val2.Add("success", JToken.op_Implicit(true));
					val2.Add("status", JToken.op_Implicit("ready"));
					val.Add((JToken)(object)val2);
					HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
				}
				else
				{
					val2.Add("success", JToken.op_Implicit(false));
					val2.Add("status", JToken.op_Implicit("started"));
					val.Add((JToken)(object)val2);
					HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
				}
			}
			else
			{
				val2.Add("success", JToken.op_Implicit(false));
				val2.Add("status", JToken.op_Implicit("invalid vmname"));
				val.Add((JToken)(object)val2);
				HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in Server PingVM. Err : " + ex2.ToString());
			Logger.Error(ex2.ToString());
			val2.Add("success", JToken.op_Implicit(false));
			val2.Add("status", JToken.op_Implicit(ex2.Message));
			val.Add((JToken)(object)val2);
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
	}

	private static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			WriteSuccessJsonWithVmName(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server Ping. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void InputMapperFilesDownloaded(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			JObject val = JObject.Parse(req.QueryString["data"]);
			string value = ((object)val["pkg_name"]).ToString();
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "packageName", value } };
			JObject val2 = JObject.Parse(((object)val["response"]).ToString().Trim());
			if (val2.Property("macro") != null && val2["macro"].ToObject<bool>())
			{
				dictionary["macro"] = true.ToString();
			}
			if (val2.Property("is_video_present") != null)
			{
				dictionary["videoPresent"] = ((object)val2["is_video_present"]).ToString();
			}
			HTTPUtils.SendRequestToClient("appInfoUpdated", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJsonWithVmName(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in InputMapperFilesDownloaded. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void HandleSoftControlBarEvent(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = req.QueryString["visible"];
			if (text != null)
			{
				InputMapper.SoftControlBarVisible(text != "0");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleSoftControlBarEvent. Err : " + ex.ToString());
		}
	}

	private static void CloseScreen(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in CloseScreen. Err : " + ex.ToString());
		}
	}

	private static void GoBack(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VmCmdHandler.RunCommand("back", MultiInstanceStrings.VmName);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server BackPress. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void ShareScreenshot(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.Instance.HandleShareButtonClicked();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShareScreenshot. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetParent(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Expected Obj, but got Unknown
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				RequestData val = HTTPUtils.ParseRequest(req);
				IntPtr mParentHanlde = new IntPtr(Convert.ToInt32(val.Data["ParentHandle"]));
				Rectangle rect = new Rectangle
				{
					X = Convert.ToInt32(val.Data["X"]),
					Y = Convert.ToInt32(val.Data["Y"]),
					Width = Convert.ToInt32(val.Data["Width"]),
					Height = Convert.ToInt32(val.Data["Height"])
				};
				ReparentWindow(mParentHanlde, rect);
				InitGamePad(req, res);
				JArray val2 = new JArray();
				JObject val3 = new JObject();
				val3.Add("success", JToken.op_Implicit(true));
				val3.Add("frontendhandle", JToken.op_Implicit(VMWindow.Instance.GetHandle().ToInt32()));
				JObject val4 = val3;
				val2.Add((JToken)(object)val4);
				HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server SetParent. Err : " + ex.ToString());
				WriteErrorJson(ex.Message, res);
			}
		}));
	}

	private static void ReparentWindow(IntPtr mParentHanlde, Rectangle rect)
	{
		IntPtr handle = VMWindow.Instance.GetHandle();
		if (mParentHanlde != IntPtr.Zero)
		{
			VMWindow.Instance.isStreamingModeEnabled = false;
			int windowLong = InteropWindow.GetWindowLong(handle, -16);
			windowLong = (int)((long)(windowLong & -13565953) | 0x40000000L);
			InteropWindow.SetWindowLong(handle, -16, windowLong);
			InteropWindow.SetParent(handle, mParentHanlde);
			VMWindow.Instance.ShowVmWindow(rect.Width, rect.Height);
			InteropWindow.SetWindowPos(handle, IntPtr.Zero, rect.X, rect.Y, rect.Width, rect.Height, 16384u);
			((Control)VMWindow.Instance).Text = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
		}
		else
		{
			VMWindow.Instance.isStreamingModeEnabled = true;
			int windowLong2 = InteropWindow.GetWindowLong(handle, -16);
			windowLong2 = (int)((windowLong2 & 0xBFFFFFFFu) | 0xCF0000);
			InteropWindow.SetWindowLong(handle, -16, windowLong2);
			InteropWindow.SetParent(handle, mParentHanlde);
			VMWindow.Instance.SetAspectRationAndMinMaxOfForm();
			VMWindow.Instance.ShowVmWindow(rect.Width + (int)VMWindow.Instance.widthDiff, rect.Height + (int)VMWindow.Instance.heightDiff);
			InteropWindow.SetWindowPos(handle, IntPtr.Zero, rect.X, rect.Y, rect.Width, rect.Height, 16448u);
			((Control)VMWindow.Instance).Text = LocaleStrings.GetLocalizedString("STRING_STREAMING_WINDOW_TITLE", "");
		}
		VMWindow.Instance.PostVmWindowShowTasks();
	}

	private static void RefreshWindow(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.Instance.HandleFrontendActivated();
		}
		catch (Exception ex)
		{
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void DeactivateFrontend(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			VMWindow.Instance.HandleFrontendDeactivated();
		}
		catch (Exception ex)
		{
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void ShowWindow(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		try
		{
			VMWindow instance = VMWindow.Instance;
			Action val = _003C_003Ec._003C_003E9__85_0;
			if (val == null)
			{
				Action val2 = () =>
				{
					VMWindow.Instance.HandleUserShowWindow();
				};
				_003C_003Ec._003C_003E9__85_0 = val2;
				val = val2;
			}
			UIHelper.RunOnUIThread((Control)(object)instance, val);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowWindow. Err : " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SwitchOrientation(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = req.QueryString["orientation"];
			string text2 = string.Empty;
			int num = int.Parse(text);
			try
			{
				text2 = req.QueryString["package"];
			}
			catch
			{
			}
			if (req.QueryString["stopFurtherOrientationChange"] != null)
			{
				if (VMWindow.Instance.AppOrientationDict.ContainsKey(text2))
				{
					VMWindow.Instance.AppOrientationDict[text2] = text;
				}
				else if (string.Equals(req.QueryString["stopFurtherOrientationChange"], "1"))
				{
					VMWindow.Instance.AppOrientationDict.Add(text2, text);
				}
				else
				{
					if (text2 == VMWindow.Instance.mLlastPackageName && VMWindow.Instance.mLastCallFromAndroid && string.Equals(req.QueryString["isPreviousSelectedTabWeb"], "0"))
					{
						return;
					}
					if (VMWindow.Instance.mLlastOrientationFromAndroid != num && string.Compare(text2, "Home", ignoreCase: true) != 0)
					{
						num = VMWindow.Instance.mLlastOrientationFromAndroid;
					}
				}
				VMWindow.Instance.mLastCallFromAndroid = false;
			}
			else
			{
				VMWindow.Instance.mLastCallFromAndroid = true;
				VMWindow.Instance.mLlastOrientationFromAndroid = num;
				if (VMWindow.Instance.AppOrientationDict.ContainsKey(text2))
				{
					return;
				}
			}
			VMWindow.Instance.mLlastPackageName = text2;
			try
			{
				Logger.Info("Changing orientation to {0}", new object[1] { num });
				LayoutManager.OrientationHandler(num);
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"is_portrait",
						Convert.ToString(LayoutManager.mEmulatedPortraitMode)
					},
					{ "package", text2 }
				};
				try
				{
					HTTPUtils.SendRequestToClient("changeOrientaion", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send orientaion change event to client... err : " + ex.ToString());
				}
				ImapHandleOrientation(num);
			}
			catch (Exception ex2)
			{
				Logger.Info("Got exec in orientation change");
				Logger.Info(ex2.ToString());
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("Exception in SwitchOrientation. Err : " + ex3.ToString());
		}
	}

	private static void QuitFrontend(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in QuitFrontend. Err : " + ex.ToString());
		}
	}

	private static void Shutdown(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Shutdown. Err : " + ex.ToString());
		}
	}

	private static void ShutdownPlayer(string vmName)
	{
		if (mIsPlayerShuttingDown)
		{
			return;
		}
		lock (sPlayerShuttingDownLockObject)
		{
			if (mIsPlayerShuttingDown)
			{
				return;
			}
			mIsPlayerShuttingDown = true;
			try
			{
				bool createdNew;
				using Mutex mutex = new Mutex(initiallyOwned: true, "Global\\BlueStacks_PlayerClosing_Lockbgp64", out createdNew);
				if (!createdNew)
				{
					try
					{
						Logger.Info("Shutdown waiting " + vmName);
						mutex.WaitOne(-1);
					}
					catch (Exception ex)
					{
						Logger.Error("Shutdown mutex wait exited: " + ex.Message);
					}
				}
				Logger.Info("Shutdown started " + vmName);
				((Form)VMWindow.Instance).Close();
				Logger.Info("Shutdown ended " + vmName);
				mutex.ReleaseMutex();
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in Shutdown player. Err : " + ex2.ToString());
			}
			finally
			{
				if (Program.sFrontendLock != null)
				{
					Logger.Info("Releasing frontend lock");
					Program.sFrontendLock.Close();
					Program.sFrontendLock = null;
				}
				Environment.Exit(0);
			}
		}
	}

	internal static void SendSetFrontendPositionRequest(int width, int height)
	{
		Thread thread = new Thread(() =>
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"width",
						width.ToString()
					},
					{
						"height",
						height.ToString()
					}
				};
				HTTPUtils.SendRequestToClient("setfrontendposition", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending request setfrontendposition to gamemanager. Err : {0}", new object[1] { ex.ToString() });
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void ToggleScreen(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		try
		{
			VMWindow instance = VMWindow.Instance;
			Action val = _003C_003Ec._003C_003E9__93_0;
			if (val == null)
			{
				Action val2 = () =>
				{
					LayoutManager.ToggleFullScreen();
				};
				_003C_003Ec._003C_003E9__93_0 = val2;
				val = val2;
			}
			UIHelper.RunOnUIThread((Control)(object)instance, val);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ToggleScreen: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StartRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			InputMapper.StartRecording();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StartRecordingCombo: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void PauseRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			InputMapper.PauseRecording();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in PauseRecordingCombo. err:" + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StopRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			int size = 0;
			StringBuilder sb = null;
			InputMapper.StopRecording(null, ref size);
			sb = new StringBuilder(size);
			InputMapper.StopRecording(sb, ref size);
			Thread thread = new Thread(() =>
			{
				try
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"events",
						sb.ToString()
					} };
					HTTPUtils.SendRequestToClient("saveComboEvents", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception in sending request setfrontendposition to gamemanager. Err : {0}", new object[1] { ex2.ToString() });
				}
			});
			thread.IsBackground = true;
			thread.Start();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopRecordingCombo: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SendGLWindowSize(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			LayoutManager.UpdateGlWindowSize = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["updateSize"]);
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendGLWindowSize: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void InitMacroPlayback(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			MacroManager.Instance.InitMacroPlayback(val.Data["scriptFilePath"].ToString());
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in InitMacroPlayback: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void RunMacroUnit(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			MacroManager.Instance.RunMacroUnit();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in RunMacroUnit: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void StopMacroPlayback(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			MacroManager.Instance.StopMacroPlayback();
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in StopMacroPlayback: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void FarmModeHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			bool flag = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["enable"]);
			Opengl.ToggleFarmMode(flag);
			if (flag)
			{
				Utils.SendChangeFPSToInstanceASync(MultiInstanceStrings.VmName, 1);
			}
			else
			{
				Utils.SendChangeFPSToInstanceASync(MultiInstanceStrings.VmName, int.MaxValue);
			}
			if (flag)
			{
				MediaManager.MuteEngine();
			}
			else if (!MediaManager.mIsMutedExplicitly)
			{
				MediaManager.UnmuteEngine();
			}
			int bstCommandProcessorPort = Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName);
			BstHttpClient.Get(string.Format("http://127.0.0.1:{0}/farmmodevalue?d={1}", bstCommandProcessorPort, flag ? "1" : "0"), (Dictionary<string, string>)null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in FarmModeHandler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void ToggleGamepadButton(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			InputMapper.ToggleGamepadButton(Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["enable"].ToString()));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ToggleGamepadButton: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void IsAppPlayerRooted(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected Obj, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			HTTPUtils.ParseRequest(req);
			bool flag = VBoxBridgeService.Instance.CheckIfAppPlayerRooted();
			Logger.Info("IsRooted = " + flag);
			JArray val = new JArray();
			JObject val2 = new JObject();
			val2.Add("success", JToken.op_Implicit(true));
			val2.Add("isRooted", JToken.op_Implicit(flag.ToString()));
			val2.Add("vmname", JToken.op_Implicit(MultiInstanceStrings.VmName));
			JObject val3 = val2;
			val.Add((JToken)(object)val3);
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in IsAppPlayerRooted err: {0}", new object[1] { ex });
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void DeviceProvisionedReceived(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			HTTPUtils.ParseRequest(req);
			if (IsDeviceProvisionReceived)
			{
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("Not provisioned", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void DeviceProvisionedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Logger.Info("Device provisioned player");
			RequestData val = HTTPUtils.ParseRequest(req);
			Stats.SendUnifiedInstallStats("device_provisioned", "");
			IsDeviceProvisionReceived = true;
			HTTPUtils.SendRequestToClient("deviceProvisioned", (Dictionary<string, string>)null, val.RequestVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void GoogleSigninHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string value = req.QueryString["email"];
			HTTPUtils.SendRequestToClient("googleSignin", new Dictionary<string, string> { { "email", value } }, val.RequestVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GoogleSigninHandler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	private static void SetFullScreenState(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string value = HTTPUtils.ParseRequest(req).Data["isFullscreen"];
			VMWindow.Instance.IsFullscreen = bool.Parse(value);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in setting Fullscreen state... Err : " + ex.ToString());
		}
	}

	private static void UpdateMacroShortcutsDict(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			VMWindow.Instance.mMacroShortcutsDict.Clear();
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				VMWindow.Instance.mMacroShortcutsDict.Add(text, val.Data[text]);
			}
			if (VMWindow.Instance.mMacroShortcutsDict != null)
			{
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson("Macroshortcuts dict is null, see inner exception", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UpdateMacroShortcutsDict... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void ChangeImei(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["imei"];
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"imei",
				text.ToString()
			} };
			string text2 = HTTPUtils.SendRequestToGuest("changeimei", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			Logger.Info("resp from android.." + text2);
			JObject val = JObject.Parse(text2);
			if (((object)val["result"]).ToString() == "ok")
			{
				WriteSuccessJson(res);
			}
			else
			{
				WriteErrorJson(((object)val["value"]).ToString(), res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending imei to android... Err : " + ex.ToString());
			WriteErrorJson(ex.ToString(), res);
		}
	}

	private static void ForceExit(HttpListenerRequest req, HttpListenerResponse res)
	{
		if (Program.sFrontendLock != null)
		{
			Logger.Info("Releasing frontend lock");
			Program.sFrontendLock.Close();
			Program.sFrontendLock = null;
		}
		Environment.Exit(-9);
	}

	private static void BootCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Logger.Info("BOOT_STAGE: Sending boot completed event received from BootCompleteHandler");
			AndroidBootUp.GuestBootCompletedEvent(null, new EventArgs());
			WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in BootCompleteHandler: " + ex.ToString());
			WriteErrorJson(ex.Message, res);
		}
	}

	public static void WriteErrorJson(string reason, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		JArray val = new JArray();
		JObject val2 = new JObject();
		val2.Add("success", JToken.op_Implicit(false));
		val2.Add("reason", JToken.op_Implicit(reason));
		JObject val3 = val2;
		val.Add((JToken)(object)val3);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
	}

	private static void WriteSuccessJsonWithVmName(HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		JArray val = new JArray();
		JObject val2 = new JObject();
		val2.Add("success", JToken.op_Implicit(true));
		val2.Add("vmname", JToken.op_Implicit(MultiInstanceStrings.VmName));
		JObject val3 = val2;
		val.Add((JToken)(object)val3);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
	}

	public static void WriteSuccessJson(HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected Obj, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		JArray val = new JArray();
		JObject val2 = new JObject();
		val2.Add("success", JToken.op_Implicit(true));
		JObject val3 = val2;
		val.Add((JToken)(object)val3);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, new JsonConverter[0]), res);
	}
}
