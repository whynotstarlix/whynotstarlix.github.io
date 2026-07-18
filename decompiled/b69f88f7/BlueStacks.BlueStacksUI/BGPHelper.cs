using System;
using System.Collections.Generic;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class BGPHelper
{
	internal static void InitHttpServerAsync()
	{
		Thread thread = new Thread(SetupHTTPServer);
		thread.IsBackground = true;
		thread.Start();
	}

	internal static void SetupHTTPServer()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected O, but got Unknown
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected O, but got Unknown
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Expected O, but got Unknown
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Expected O, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Expected O, but got Unknown
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected O, but got Unknown
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Expected O, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Expected O, but got Unknown
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Expected O, but got Unknown
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Expected O, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected O, but got Unknown
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Expected O, but got Unknown
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Expected O, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Expected O, but got Unknown
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Expected O, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected O, but got Unknown
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Expected O, but got Unknown
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Expected O, but got Unknown
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected O, but got Unknown
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Expected O, but got Unknown
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Expected O, but got Unknown
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Expected O, but got Unknown
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Expected O, but got Unknown
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Expected O, but got Unknown
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Expected O, but got Unknown
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Expected O, but got Unknown
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Expected O, but got Unknown
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Expected O, but got Unknown
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Expected O, but got Unknown
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Expected O, but got Unknown
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Expected O, but got Unknown
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Expected O, but got Unknown
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Expected O, but got Unknown
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Expected O, but got Unknown
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Expected O, but got Unknown
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Expected O, but got Unknown
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Expected O, but got Unknown
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Expected O, but got Unknown
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Expected O, but got Unknown
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Expected O, but got Unknown
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Expected O, but got Unknown
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Expected O, but got Unknown
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Expected O, but got Unknown
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Expected O, but got Unknown
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Expected O, but got Unknown
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Expected O, but got Unknown
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Expected O, but got Unknown
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Expected O, but got Unknown
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Expected O, but got Unknown
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Expected O, but got Unknown
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Expected O, but got Unknown
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Expected O, but got Unknown
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Expected O, but got Unknown
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Expected O, but got Unknown
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Expected O, but got Unknown
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Expected O, but got Unknown
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Expected O, but got Unknown
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Expected O, but got Unknown
		HttpHandlerSetup.InitHTTPServer(new Dictionary<string, RequestHandler>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"/ping",
				new RequestHandler(HTTPHandler.PingHandler)
			},
			{
				"/appDisplayed",
				new RequestHandler(HTTPHandler.AppDisplayedHandler)
			},
			{
				"/appLaunched",
				new RequestHandler(HTTPHandler.AppLaunchedHandler)
			},
			{
				"/showApp",
				new RequestHandler(HTTPHandler.ShowAppHandler)
			},
			{
				"/showWindow",
				new RequestHandler(HTTPHandler.ShowWindowHandler)
			},
			{
				"/isVisible",
				new RequestHandler(HTTPHandler.IsVisibleHandler)
			},
			{
				"/appUninstalled",
				new RequestHandler(HTTPHandler.AppUninstalledHandler)
			},
			{
				"/appInstalled",
				new RequestHandler(HTTPHandler.AppInstalledHandler)
			},
			{
				"/enableWndProcLogging",
				new RequestHandler(HTTPHandler.EnableWndProcLogging)
			},
			{
				"/quit",
				new RequestHandler(HTTPHandler.ForceQuitHandler)
			},
			{
				"/google",
				new RequestHandler(HTTPHandler.OpenGoogleHandler)
			},
			{
				"/closeCrashedAppTab",
				new RequestHandler(HTTPHandler.AppCrashedHandler)
			},
			{
				"/showWebPage",
				new RequestHandler(HTTPHandler.ShowWebPageHandler)
			},
			{
				"/showHomeTab",
				new RequestHandler(HTTPHandler.ShowHomeTabHandler)
			},
			{
				"/closeTab",
				new RequestHandler(HTTPHandler.CloseTabHandler)
			},
			{
				"/updateUserInfo",
				new RequestHandler(HTTPHandler.UpdateUserInfoHandler)
			},
			{
				"/oneTimeSetupCompleted",
				new RequestHandler(HTTPHandler.OneTimeSetupCompletedHandler)
			},
			{
				"/appInstallStarted",
				new RequestHandler(HTTPHandler.AppInstallStarted)
			},
			{
				"/appInstallFailed",
				new RequestHandler(HTTPHandler.AppInstallFailed)
			},
			{
				"/googlePlayAppInstall",
				new RequestHandler(HTTPHandler.GooglePlayAppInstall)
			},
			{
				"/bootFailedPopup",
				new RequestHandler(HTTPHandler.BootFailedPopupHandler)
			},
			{
				"/dragDropInstall",
				new RequestHandler(HTTPHandler.DragDropInstallHandler)
			},
			{
				"/openPackage",
				new RequestHandler(HTTPHandler.OpenOrInstallPackageHandler)
			},
			{
				"/stopInstance",
				new RequestHandler(HTTPHandler.StopInstanceHandler)
			},
			{
				"/minimizeInstance",
				new RequestHandler(HTTPHandler.MinimizeInstanceHandler)
			},
			{
				"/startInstance",
				new RequestHandler(HTTPHandler.StartInstanceHandler)
			},
			{
				"/hideBluestacks",
				new RequestHandler(HTTPHandler.HideBluestacksHandler)
			},
			{
				"/tileWindow",
				new RequestHandler(HTTPHandler.TileWindow)
			},
			{
				"/cascadeWindow",
				new RequestHandler(HTTPHandler.CascadeWindow)
			},
			{
				"/launchWebTab",
				new RequestHandler(HTTPHandler.LaunchWebTab)
			},
			{
				"/openNotificationSettings",
				new RequestHandler(HTTPHandler.ShowSettingWindow)
			},
			{
				"/isAnyAppRunning",
				new RequestHandler(HTTPHandler.IsAnyAppRunning)
			},
			{
				"/launchDefaultWebApp",
				new RequestHandler(HTTPHandler.LaunchDefaultWebApp)
			},
			{
				"/toggleFarmMode",
				new RequestHandler(HTTPHandler.ToggleFarmMode)
			},
			{
				"/changeTextOTS",
				new RequestHandler(HTTPHandler.ChangeTextOTSHandler)
			},
			{
				"/macroCompleted",
				new RequestHandler(HTTPHandler.MacroCompleted)
			},
			{
				"/appInfoUpdated",
				new RequestHandler(HTTPHandler.AppInfoUpdated)
			},
			{
				"/sendAppDisplayed",
				new RequestHandler(HTTPHandler.SendAppDisplayed)
			},
			{
				"/static",
				new RequestHandler(HTTPHandler.IsBlueStacksUIVisible)
			},
			{
				"/restartFrontend",
				new RequestHandler(HTTPHandler.RestartFrontend)
			},
			{
				"/gcCollect",
				new RequestHandler(HTTPHandler.GCCollect)
			},
			{
				"/showWindowAndApp",
				new RequestHandler(HTTPHandler.ShowWindowAndAppHandler)
			},
			{
				"/unsupportedCpuError",
				new RequestHandler(HTTPHandler.UnsupportedCPUError)
			},
			{
				"/changeOrientaion",
				new RequestHandler(HTTPHandler.ChangeOrientaionHandler)
			},
			{
				"/shootingModeChanged",
				new RequestHandler(HTTPHandler.ShootingModeChanged)
			},
			{
				"/guestBootCompleted",
				new RequestHandler(HTTPHandler.GuestBootCompleted)
			},
			{
				"/getRunningInstances",
				new RequestHandler(HTTPHandler.GetRunningInstances)
			},
			{
				"/appJsonChanged",
				new RequestHandler(HTTPHandler.AppJsonChangedHandler)
			},
			{
				"/getCurrentAppDetails",
				new RequestHandler(HTTPHandler.GetCurrentAppDetails)
			},
			{
				"/maintenanceWarning",
				new RequestHandler(HTTPHandler.ShowMaintenanceWarning)
			},
			{
				"/updateSizeOfOverlay",
				new RequestHandler(HTTPHandler.UpdateSizeOfOverlay)
			},
			{
				"/androidLocaleChanged",
				new RequestHandler(HTTPHandler.AndroidLocaleChanged)
			},
			{
				"/saveComboEvents",
				new RequestHandler(HTTPHandler.SaveComboEvents)
			},
			{
				"/handleClientOperation",
				new RequestHandler(HTTPHandler.HandleClientOperation)
			},
			{
				"/handleGamepadConnection",
				new RequestHandler(HTTPHandler.HandleGamepadConnection)
			},
			{
				"/macroPlaybackComplete",
				new RequestHandler(HTTPHandler.MacroPlaybackCompleteHandler)
			},
			{
				"/toggleStreamingMode",
				new RequestHandler(HTTPHandler.ToggleStreamingMode)
			},
			{
				"/handleClientGamepadButton",
				new RequestHandler(HTTPHandler.HandleClientGamepadButtonHandler)
			},
			{
				"/handleGamepadGuidanceButton",
				new RequestHandler(HTTPHandler.GamepadGuidanceButtonHandler)
			},
			{
				"/deviceProvisioned",
				new RequestHandler(HTTPHandler.DeviceProvisionedHandler)
			},
			{
				"/googleSignin",
				new RequestHandler(HTTPHandler.GoogleSigninHandler)
			},
			{
				"/hideTopSidebar",
				new RequestHandler(HTTPHandler.HideTopSideBarHandler)
			},
			{
				"/showFullscreenSidebarButton",
				new RequestHandler(HTTPHandler.FullScreenSidebarButtonHandler)
			},
			{
				"/showFullscreenTopbarButton",
				new RequestHandler(HTTPHandler.FullScreenTopbarButtonHandler)
			},
			{
				"/showFullscreenSidebar",
				new RequestHandler(HTTPHandler.FullScreenSidebarHandler)
			},
			{
				"/setCurrentVolumeFromAndroid",
				new RequestHandler(HTTPHandler.SetCurrentVolumeFromAndroidHandler)
			},
			{
				"/enableDebugLogs",
				new RequestHandler(HTTPHandler.EnableDebugLogs)
			},
			{
				"/setDMMKeymapping",
				new RequestHandler(HTTPHandler.SetDMMKeymapping)
			},
			{
				"/ncSetGameInfoOnTopBar",
				new RequestHandler(HTTPHandler.NCSetGameInfoOnTopBarHandler)
			},
			{
				"/updateLocale",
				new RequestHandler(HTTPHandler.UpdateLocale)
			},
			{
				"/screenshotCaptured",
				new RequestHandler(HTTPHandler.ScreenshotCaptured)
			},
			{
				"/hotKeyEvents",
				new RequestHandler(HTTPHandler.ClientHotkeyHandler)
			},
			{
				"/launchPlay",
				new RequestHandler(HTTPHandler.LaunchPlay)
			},
			{
				"/enableKeyboardHookLogging",
				new RequestHandler(HTTPHandler.EnableKeyboardHookLogging)
			},
			{
				"/muteAllInstances",
				new RequestHandler(HTTPHandler.MuteAllInstancesHandler)
			},
			{
				"/screenLock",
				new RequestHandler(HTTPHandler.ScreenLock)
			},
			{
				"/getHeightWidth",
				new RequestHandler(HTTPHandler.GetHeightWidth)
			},
			{
				"/accountSetupCompleted",
				new RequestHandler(HTTPHandler.AccountSetupCompleted)
			},
			{
				"/openThemeEditor",
				new RequestHandler(HTTPHandler.OpenThemeEditor)
			},
			{
				"/setStreamingStatus",
				new RequestHandler(HTTPHandler.SetStreamingStatus)
			},
			{
				"/playerScriptModifierClick",
				new RequestHandler(HTTPHandler.PlayerScriptModifierKeyUp)
			},
			{
				"/reloadShortcuts",
				new RequestHandler(HTTPHandler.ReloadShortcuts)
			},
			{
				"/reloadPromotions",
				new RequestHandler(HTTPHandler.ReloadPromotions)
			},
			{
				"/overlayControlsVisibility",
				new RequestHandler(HTTPHandler.HandleOverlayControlsVisibility)
			},
			{
				"/showGrmAndLaunchApp",
				new RequestHandler(HTTPHandler.ShowGrmAndLaunchAppHandler)
			},
			{
				"/reinitRegistry",
				new RequestHandler(HTTPHandler.ReinitRegistry)
			},
			{
				"/openCFGReorderTool",
				new RequestHandler(HTTPHandler.OpenCFGReorderTool)
			},
			{
				"/updateCrc",
				new RequestHandler(HTTPHandler.UpdateCrc)
			},
			{
				"/configFileChanged",
				new RequestHandler(HTTPHandler.ConfigFileChanged)
			},
			{
				"/addNotificationInDrawer",
				new RequestHandler(HTTPHandler.AddNotificationInDrawer)
			},
			{
				"/markNotificationInDrawer",
				new RequestHandler(HTTPHandler.MarkNotificationInDrawer)
			},
			{
				"/checkCallbackEnabledStatus",
				new RequestHandler(HTTPHandler.CheckCallbackEnabledStatus)
			},
			{
				"/obsStatus",
				new RequestHandler(BTVManager.ObsStatusHandler)
			},
			{
				"/reportObsError",
				new RequestHandler(BTVManager.ReportObsErrorHandler)
			},
			{
				"/capturingError",
				new RequestHandler(BTVManager.ReportCaptureError)
			},
			{
				"/openGLCapturingError",
				new RequestHandler(BTVManager.ReportOpenGLCaptureError)
			}
		});
	}
}
