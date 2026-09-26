using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

internal static class AndroidBootUp
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static ThreadStart _003C_003E9__11_0;

		public static Action _003C_003E9__12_0;

		public static WaitCallback _003C_003E9__13_0;

		public static WaitCallback _003C_003E9__13_1;

		public static WaitCallback _003C_003E9__13_2;

		public static Action _003C_003E9__13_3;

		public static WaitCallback _003C_003E9__13_4;

		public static Action _003C_003E9__13_5;

		public static WaitCallback _003C_003E9__13_6;

		public static ThreadStart _003C_003E9__14_0;

		public static Action _003C_003E9__23_0;

		internal void _003CStart_003Eb__11_0()
		{
			try
			{
				AttachMonitor();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in AndroidBootUp.Start. Err : ", new object[1] { ex.ToString() });
				HandleBootError();
				throw ex;
			}
		}

		internal void _003CGuestBootCompletedEvent_003Eb__12_0()
		{
			VMWindow instance = VMWindow.Instance;
			((Control)instance).Width = ((Control)instance).Width - 1;
			VMWindow instance2 = VMWindow.Instance;
			((Control)instance2).Width = ((Control)instance2).Width + 1;
		}

		internal void _003CPerformDeferredSetup_003Eb__13_0(object stateInfo)
		{
			VmCmdHandler.SyncConfig(InputMapper.GetKeyMappingParserVersion(), MultiInstanceStrings.VmName);
			VmCmdHandler.SetKeyboard(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName);
		}

		internal void _003CPerformDeferredSetup_003Eb__13_1(object stateInfo)
		{
			Logger.Info("Started fqdnSender thread for agent");
			VmCmdHandler.FqdnSend(0, "Agent", MultiInstanceStrings.VmName);
			Logger.Info("fqdnSender thread exiting");
		}

		internal void _003CPerformDeferredSetup_003Eb__13_2(object stateInfo)
		{
			Logger.Info("Started fqdnSender thread for frontend");
			VmCmdHandler.FqdnSend(0, "frontend", MultiInstanceStrings.VmName);
			Logger.Info("fqdnSender thread exiting");
		}

		internal void _003CPerformDeferredSetup_003Eb__13_3()
		{
			try
			{
				string text = Clipboard.GetText((TextDataFormat)0);
				Logger.Debug("sending clipboard data to android.." + text);
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "text", text } };
				HTTPUtils.SendRequestToGuestAsync("clipboard", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0);
			}
			catch (Exception ex)
			{
				Logger.Error(" error in sending clipboard data to android.." + ex);
			}
		}

		internal void _003CPerformDeferredSetup_003Eb__13_4(object stateInfo)
		{
			VmCmdHandler.SetMachineType(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName);
		}

		internal void _003CPerformDeferredSetup_003Eb__13_5()
		{
			foreach (int key in VMWindow.Instance.mControllerMap.Keys)
			{
				SensorDevice.Instance.ControllerAttach(SensorDevice.Type.Accelerometer);
				SendControllerEvent("attach", key, VMWindow.Instance.mControllerMap[key]);
			}
			VMWindow.Instance.mControllerMap.Clear();
		}

		internal void _003CPerformDeferredSetup_003Eb__13_6(object stateInfo)
		{
			Logger.Info("Checking for Black Screen Error");
			CheckBlackScreenAndRestartGMifOccurs();
		}

		internal void _003CGpsAttach_003Eb__14_0()
		{
			Logger.Info("GpsAttach");
			try
			{
				GPSManager.Init();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GpsAttach. Err : " + ex.ToString());
			}
		}

		internal void _003CAttachMonitor_003Eb__23_0()
		{
			if (mManager != null)
			{
				throw new SystemException("A connection to the manager is already open");
			}
			if (mMonitor != null)
			{
				throw new SystemException("Another monitor is already attached");
			}
			uint id = MonitorLocator.Lookup(MultiInstanceStrings.VmName);
			Manager manager = null;
			Monitor monitor = null;
			bool verbose = false;
			try
			{
				verbose = mFirstMonitorAttachAttempt;
				mFirstMonitorAttachAttempt = false;
				manager = Manager.Open();
				monitor = manager.Attach(id, verbose, isMonAttach: false);
			}
			catch (Exception ex)
			{
				if (!IsExceptionFileNotFound(ex))
				{
					Logger.Error(ex.ToString());
				}
				if (manager != null)
				{
					manager = null;
				}
			}
			if (monitor != null)
			{
				monitor = manager.Attach(id, verbose, isMonAttach: true);
				if (monitor == null)
				{
					Logger.Info("Could not Attach to a monitor");
				}
				forceVideoModeChange = true;
				mMonitor = monitor;
				mManager = manager;
				if (!HideBootProgress())
				{
					ShowConnectedView();
				}
			}
		}
	}

	internal static bool isAndroidBooted = false;

	internal static bool isAndroidReady = false;

	private static object sLogFailureLogRegLock = new object();

	private static bool mFirstMonitorAttachAttempt = true;

	internal static bool forceVideoModeChange = false;

	internal static Monitor mMonitor = null;

	internal static Manager mManager = null;

	internal static CameraManager camManager;

	private static object mSendingBootFailureLogsLock = new object();

	internal static bool sHasNotifiedClientForGuestBooted = false;

	private static readonly object sSendBootCompleteLockObject = new object();

	internal static void Start()
	{
		try
		{
			LayoutManager.InitOpengl();
			Thread thread = new Thread(() =>
			{
				try
				{
					AttachMonitor();
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception in AndroidBootUp.Start. Err : ", new object[1] { ex2.ToString() });
					HandleBootError();
					throw ex2;
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Error in Android bootup start: " + ex.ToString());
			HandleBootError();
			throw ex;
		}
	}

	public static void GuestBootCompletedEvent(object sender, EventArgs e)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected Obj, but got Unknown
		if (sHasNotifiedClientForGuestBooted)
		{
			return;
		}
		lock (sSendBootCompleteLockObject)
		{
			if (sHasNotifiedClientForGuestBooted)
			{
				return;
			}
			try
			{
				Logger.Info("BOOT_STAGE: Sending boot completed event");
				try
				{
					HTTPUtils.SendRequestToClientAsync("guestBootCompleted", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex)
				{
					Logger.Error("An exception in sending boot completed request to client: {0}", new object[1] { ex.ToString() });
				}
				if (LoadingScreen.mLoadingScreen != null)
				{
					LoadingScreen.RemoveLoadingScreen();
				}
				isAndroidBooted = true;
				StateMachine.mForceShutdownDueTime = 60000;
				Stats.SendFrontendStatusUpdate("frontend-ready", MultiInstanceStrings.VmName);
				SendSecurityMessageToAndroidOnBootFinish();
				if (HideBootProgress())
				{
					ShowConnectedView();
					PerformDeferredSetup();
					CheckVtxAndShowPopup();
				}
				VMWindow instance = VMWindow.Instance;
				Action val = _003C_003Ec._003C_003E9__12_0;
				if (val == null)
				{
					Action val2 = () =>
					{
						VMWindow instance2 = VMWindow.Instance;
						((Control)instance2).Width = ((Control)instance2).Width - 1;
						VMWindow instance3 = VMWindow.Instance;
						((Control)instance3).Width = ((Control)instance3).Width + 1;
					};
					_003C_003Ec._003C_003E9__12_0 = val2;
					val = val2;
				}
				UIHelper.RunOnUIThread((Control)(object)instance, val);
				Utils.SyncAppJson(MultiInstanceStrings.VmName);
				sHasNotifiedClientForGuestBooted = true;
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception on GuestBootCompletedEvent. Err : " + ex2.ToString());
			}
		}
	}

	private static void PerformDeferredSetup()
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected Obj, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected Obj, but got Unknown
		if (Opt.Instance.sysPrep)
		{
			return;
		}
		Logger.Info("Console PerformDeferredSetup");
		Stats.SendBootStats("frontend", true, false, MultiInstanceStrings.VmName);
		VMWindow.Instance.SendOrientationToGuest();
		if (RegistryManager.Instance.DefaultGuest.ConfigSynced == 0)
		{
			Logger.Info("Config not synced. Syncing now.");
			ThreadPool.QueueUserWorkItem((object stateInfo) =>
			{
				VmCmdHandler.SyncConfig(InputMapper.GetKeyMappingParserVersion(), MultiInstanceStrings.VmName);
				VmCmdHandler.SetKeyboard(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName);
			});
		}
		else
		{
			string currentKeyboardLayout = Utils.GetCurrentKeyboardLayout();
			VMWindow.Instance.SetKeyboardLayout(currentKeyboardLayout);
			Logger.Info("Config already synced.");
		}
		ThreadPool.QueueUserWorkItem((object stateInfo) =>
		{
			Logger.Info("Started fqdnSender thread for agent");
			VmCmdHandler.FqdnSend(0, "Agent", MultiInstanceStrings.VmName);
			Logger.Info("fqdnSender thread exiting");
		});
		ThreadPool.QueueUserWorkItem((object stateInfo) =>
		{
			Logger.Info("Started fqdnSender thread for frontend");
			VmCmdHandler.FqdnSend(0, "frontend", MultiInstanceStrings.VmName);
			Logger.Info("fqdnSender thread exiting");
		});
		VMWindow instance = VMWindow.Instance;
		Action val = _003C_003Ec._003C_003E9__13_3;
		if (val == null)
		{
			Action val2 = () =>
			{
				try
				{
					string text = Clipboard.GetText((TextDataFormat)0);
					Logger.Debug("sending clipboard data to android.." + text);
					Dictionary<string, string> dictionary = new Dictionary<string, string> { { "text", text } };
					HTTPUtils.SendRequestToGuestAsync("clipboard", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0);
				}
				catch (Exception ex)
				{
					Logger.Error(" error in sending clipboard data to android.." + ex);
				}
			};
			_003C_003Ec._003C_003E9__13_3 = val2;
			val = val2;
		}
		UIHelper.RunOnUIThread((Control)(object)instance, val);
		ThreadPool.QueueUserWorkItem((object stateInfo) =>
		{
			VmCmdHandler.SetMachineType(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName);
		});
		GpsAttach();
		SensorDevice.Instance.Start(MultiInstanceStrings.VmName);
		CameraAttach();
		Action val3 = _003C_003Ec._003C_003E9__13_5;
		if (val3 == null)
		{
			Action val4 = () =>
			{
				foreach (int key in VMWindow.Instance.mControllerMap.Keys)
				{
					SensorDevice.Instance.ControllerAttach(SensorDevice.Type.Accelerometer);
					SendControllerEvent("attach", key, VMWindow.Instance.mControllerMap[key]);
				}
				VMWindow.Instance.mControllerMap.Clear();
			};
			_003C_003Ec._003C_003E9__13_5 = val4;
			val3 = val4;
		}
		SendControllerEventInternal("controller_flush", val3);
		ThreadPool.QueueUserWorkItem((object stateInfo) =>
		{
			Logger.Info("Checking for Black Screen Error");
			CheckBlackScreenAndRestartGMifOccurs();
		});
	}

	private static void GpsAttach()
	{
		Thread thread = new Thread(() =>
		{
			Logger.Info("GpsAttach");
			try
			{
				GPSManager.Init();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GpsAttach. Err : " + ex.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal static void GpsDetach()
	{
		Logger.Info("GpsDetach");
		GPSManager.Shutdown();
	}

	private static void CameraAttach()
	{
		if (camManager != null)
		{
			Logger.Info("cam Manager is already attached");
			return;
		}
		camManager = new CameraManager();
		Logger.Info("CameraAttach");
		try
		{
			CameraManager.Monitor = mMonitor;
			camManager.InitCamera(new string[1] { MultiInstanceStrings.VmName });
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in CameraAttach. Err : " + ex.ToString());
		}
	}

	internal static void CameraDetach()
	{
		if (camManager == null)
		{
			Logger.Info("Cannot detach camera, which is not yet attached");
			return;
		}
		Logger.Info("CameraDetach");
		camManager.Shutdown();
		camManager = null;
	}

	private static void CheckBlackScreenAndRestartGMifOccurs()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		Logger.Info("In method CheckBlackScreenAndRestartGMifOccurs");
		int num = 0;
		while (VMWindow.Instance.CheckBlackScreen() && num < 300)
		{
			num++;
			Thread.Sleep(1000);
		}
		if (num >= 300)
		{
			Logger.Info("Black Screen occurs for 5 mins");
			if ((int)MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_BLACKSCREEN_FORM", ""), LocaleStrings.GetLocalizedString("STRING_TROUBLESHOOTER", ""), (MessageBoxButtons)1) == 1)
			{
				Logger.Info("User click Yes, Restartig GameManager");
				HTTPUtils.SendRequestToClient("restartFrontend", new Dictionary<string, string> { 
				{
					"vmname",
					MultiInstanceStrings.VmName
				} }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			else
			{
				Logger.Info("User clicked No");
			}
		}
		else
		{
			Logger.Info("Frontend launched Successfully");
			Stats.SendHomeScreenDisplayedStats(MultiInstanceStrings.VmName);
		}
	}

	private static void SendControllerEvent(string name, int identity, string type)
	{
		SendControllerEventInternal($"controller_{name} {identity} {type}", null);
	}

	private static void SendControllerEventInternal(string cmd, Action continuation)
	{
		Logger.Info("Sending controller event " + cmd);
		VmCmdHandler.RunCommandAsync(cmd, continuation, (Control)(object)VMWindow.Instance, MultiInstanceStrings.VmName);
	}

	private static void CheckVtxAndShowPopup()
	{
		Logger.Info("In CheckVtxAndShowPopup");
		int systemInfoStats = RegistryManager.Instance.SystemInfoStats2;
		string deviceCaps = RegistryManager.Instance.DeviceCaps;
		if (systemInfoStats == 1 && !deviceCaps.Equals(""))
		{
			Logger.Info("Sending DeviceCaps stats");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Logger.Info("DeviceCaps: " + deviceCaps);
			dictionary.Add("data", deviceCaps);
			dictionary.Add("install_id", RegistryManager.Instance.InstallID);
			try
			{
				BstHttpClient.Post(RegistryManager.Instance.Host + "/stats/systeminfostats2", dictionary, (Dictionary<string, string>)null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp64");
				RegistryManager.Instance.SystemInfoStats2 = 0;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Sending systeminfostats2. Err : " + ex.ToString());
			}
		}
		try
		{
			JObject val = JObject.Parse(deviceCaps);
			if (Oem.Instance.IsVTPopupEnabled && ((object)val["cpu_hvm"]).ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && ((object)val["bios_hvm"]).ToString().Equals("False", StringComparison.OrdinalIgnoreCase) && (((object)val["engine_enabled"]).ToString().Equals("legacy", StringComparison.OrdinalIgnoreCase) || ((object)val["engine_enabled"]).ToString().Equals("raw", StringComparison.OrdinalIgnoreCase)))
			{
				ShowVtxPopup();
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in showing vtxpopup {0}. Err : ", new object[1] { ex2.ToString() });
		}
	}

	private static void ShowVtxPopup()
	{
		Logger.Info("User shown vtx enable popup");
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "url", "http://bluestacks-cloud.appspot.com/performance_with_vt" },
			{ "title", "enablevt" }
		};
		HTTPUtils.SendRequestToClient("showenablevtpopup", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		try
		{
			Stats.SendMiscellaneousStatsSync("EnableVtx", (string)null, RegistryManager.Instance.UserGuid, "Enable vt popup shown", RegistryManager.Instance.Version, (string)null, (string)null, (string)null, (string)null, MultiInstanceStrings.VmName, 0);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending enablevtx stats. Err : {0}", new object[1] { ex.Message });
		}
	}

	private static void AttachMonitor()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		VMWindow instance = VMWindow.Instance;
		Action val = _003C_003Ec._003C_003E9__23_0;
		if (val == null)
		{
			Action val2 = () =>
			{
				if (mManager != null)
				{
					throw new SystemException("A connection to the manager is already open");
				}
				if (mMonitor != null)
				{
					throw new SystemException("Another monitor is already attached");
				}
				uint id = MonitorLocator.Lookup(MultiInstanceStrings.VmName);
				Manager manager = null;
				Monitor monitor = null;
				bool verbose = false;
				try
				{
					verbose = mFirstMonitorAttachAttempt;
					mFirstMonitorAttachAttempt = false;
					manager = Manager.Open();
					monitor = manager.Attach(id, verbose, isMonAttach: false);
				}
				catch (Exception ex)
				{
					if (!IsExceptionFileNotFound(ex))
					{
						Logger.Error(ex.ToString());
					}
					if (manager != null)
					{
						manager = null;
					}
				}
				if (monitor != null)
				{
					monitor = manager.Attach(id, verbose, isMonAttach: true);
					if (monitor == null)
					{
						Logger.Info("Could not Attach to a monitor");
					}
					forceVideoModeChange = true;
					mMonitor = monitor;
					mManager = manager;
					if (!HideBootProgress())
					{
						ShowConnectedView();
					}
				}
			};
			_003C_003Ec._003C_003E9__23_0 = val2;
			val = val2;
		}
		UIHelper.RunOnUIThread((Control)(object)instance, val);
	}

	private static void ShowConnectedView()
	{
		Logger.Info("ShowConnectedView");
		if (VMWindow.Instance.IsShownOnce)
		{
			MediaManager.UnmuteEngine();
		}
		else
		{
			MediaManager.MuteEngine();
		}
		InputMapper.Instance.SetMonitor(mMonitor);
		GPSManager.Instance().SetMonitor(mMonitor);
		Logger.Debug("Raising Layout event");
		Opengl.userInteracted = true;
		if (!Opengl.IsSubWindowVisible())
		{
			Logger.Info("showing window");
			Opengl.glWindowAction = GlWindowAction.Show;
			Opengl.userInteracted = false;
		}
		LayoutManager.FixupGuestDisplay();
		isAndroidReady = true;
		VMWindow.Instance.BootUpTasks();
	}

	private static void SendSecurityMessageToAndroidOnBootFinish()
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Regex regex = new Regex("[^a-zA-Z0-9]");
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			X509Certificate signerCertificate = executingAssembly.GetModules()[0].GetSignerCertificate();
			string arg = string.Empty;
			if (signerCertificate != null)
			{
				arg = BitConverter.ToString(signerCertificate.GetSerialNumber());
				arg = ((!string.IsNullOrEmpty(arg)) ? regex.Replace(arg, ".") : string.Empty);
			}
			string location = executingAssembly.Location;
			location = ((!string.IsNullOrEmpty(location)) ? regex.Replace(location, ".") : string.Empty);
			string baseKeyPath = RegistryManager.Instance.BaseKeyPath;
			baseKeyPath = ((!string.IsNullOrEmpty(baseKeyPath)) ? regex.Replace(baseKeyPath, ".") : string.Empty);
			string text = "{";
			text += $"\"regPath\":\"{baseKeyPath}\",";
			text += $"\"location\":\"{location}\",";
			text += $"\"serial\":\"{arg}\"";
			text += "}";
			VmCmdHandler.RunCommand(string.Format("{0} {1}", "setBlueStacksConfig", text), MultiInstanceStrings.VmName);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString());
		}
	}

	internal static bool HideBootProgress()
	{
		if (RegistryManager.Instance.DefaultGuest.HideBootProgress == 1)
		{
			return true;
		}
		return false;
	}

	private static bool IsExceptionFileNotFound(Exception exc)
	{
		Exception innerException = exc.InnerException;
		if (innerException == null || (object)innerException.GetType() != typeof(Win32Exception))
		{
			return false;
		}
		if (((Win32Exception)innerException).NativeErrorCode != 2)
		{
			return false;
		}
		return true;
	}

	internal static void HandleBootError()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		lock (mSendingBootFailureLogsLock)
		{
			Logger.Error("Handling Boot Error");
			Logger.Error(new StackTrace().ToString());
			SendBootFailureLogs();
			if (string.Equals(Oem.Instance.OEM, "dmm", StringComparison.InvariantCultureIgnoreCase))
			{
				HTTPUtils.SendRequestToAgent("guestBootFailed", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64", true);
			}
			if (Oem.Instance.IsOEMWithBGPClient)
			{
				HTTPUtils.SendRequestToClientAsync("bootFailedPopup", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			else
			{
				MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", ""));
			}
			Environment.Exit(-1);
		}
	}

	internal static void SendBootFailureLogs()
	{
		try
		{
			Logger.Info("In SendBootFailureLogs");
			Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe"), "-boot");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception occured in SendBootFailureLogs. Err : {0}", new object[1] { ex.ToString() });
		}
	}

	public static bool CheckIfErrorLogsAlreadySent(string category, int exitCode)
	{
		Logger.Info("Checking if logs sent for category: {0} with exitcode: {1}", new object[2] { category, exitCode });
		lock (sLogFailureLogRegLock)
		{
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.HostConfigKeyPath + "\\FailureLogsInfo\\");
			string text = (string)registryKey.GetValue(category, "");
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[1] { ',' });
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Compare(array[i], exitCode.ToString()) == 0)
					{
						Logger.Info("Logs already sent");
						return true;
					}
				}
				registryKey.SetValue(category, text + "," + exitCode);
				Logger.Info("Logs not sent, will send this time");
				return false;
			}
			registryKey.SetValue(category, exitCode.ToString());
			Logger.Info("Logs not sent, will send this time");
			return false;
		}
	}
}
