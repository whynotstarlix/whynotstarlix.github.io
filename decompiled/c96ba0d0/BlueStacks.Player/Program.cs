using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

public static class Program
{
	public static Mutex sFrontendLock;

	[DllImport("HD-Plus-Devices.dll")]
	private static extern int BstPlusDevicesInit();

	[DllImport("HD-Plus-Devices.dll")]
	private static extern int BstSetComputedGuid(string systemGuid);

	[DllImport("HD-Audio-Native.dll")]
	private static extern void InitAudioLogger(HdLoggerCallback cb);

	[STAThread]
	public static void Main(string[] args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		ProcessUtils.LogParentProcessDetails();
		Strings.CurrentDefaultVmName = args[0];
		MultiInstanceStrings.VmName = args[0];
		TimelineStatsSender.Init(MultiInstanceStrings.VmName);
		TimelineStatsSender.HandleEngineBootEvent(((object)(EngineStatsEvent)0/*cast due to constrained. prefix*/).ToString());
		BstPlusDevicesInit();
		((GetOpt)Opt.Instance).Parse(args);
		if (Opt.Instance.help)
		{
			Usage();
		}
		Stats.SendFrontendStatusUpdate("frontend-launched", MultiInstanceStrings.VmName);
		if (!MultiInstanceUtils.VerifyVmId(MultiInstanceStrings.VmName))
		{
			Logger.Error("VmName {0} , not part of VmList {1} , Exiting Process", new object[2]
			{
				MultiInstanceStrings.VmName,
				RegistryManager.Instance.VmList.ToString()
			});
			Environment.Exit(1);
		}
		Logger.InitVmInstanceName(MultiInstanceStrings.VmName);
		InputManagerProxy.SetUp();
		int num = InputManagerProxy.IsHyperVEnabled();
		Logger.Info("IsHyperVEnabled: {0}", new object[1] { num });
		switch (num)
		{
		case 1:
			Environment.Exit(-5);
			return;
		default:
			Logger.Info("Non-microsoft Hyper-V may be active, continuing");
			break;
		case 0:
			break;
		}
		CheckIfAlreadyRunning(Opt.Instance.h);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		if (!VMWindow.CheckAndroidFilesIntegrity())
		{
			Logger.Error("Android File Integrity check failed");
			Environment.Exit(-2);
		}
		TimelineStatsSender.HandleEngineBootEvent(((object)(EngineStatsEvent)1/*cast due to constrained. prefix*/).ToString());
		InputMapper.RegisterGuestBootLogsHandler();
		AndroidService.StartAsync();
		HTTPHandler.StartServer();
		InitAudioLogger(Logger.GetHdLoggerCallback());
		new VMWindow(Opt.Instance.h, Opt.Instance.w);
		Application.Run();
	}

	private static void CheckIfAlreadyRunning(bool hideMode)
	{
		if (!ProcessUtils.CheckAlreadyRunningAndTakeLock(Strings.GetPlayerLockName(MultiInstanceStrings.VmName, "bgp64"), ref sFrontendLock))
		{
			return;
		}
		Logger.Info("Frontend already running");
		IntPtr zero = IntPtr.Zero;
		if (!hideMode)
		{
			zero = BringToFront(MultiInstanceStrings.VmName);
			if (zero != IntPtr.Zero)
			{
				Logger.Info("Sending WM_USER_SHOW_WINDOW to Frontend handle {0}", new object[1] { zero });
				InteropWindow.SendMessage(zero, 1025u, IntPtr.Zero, IntPtr.Zero);
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				try
				{
					string text = HTTPUtils.SendRequestToEngine("showWindow", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "");
					Logger.Info("ShowWindow result: " + text);
				}
				catch (Exception ex)
				{
					Logger.Warning("Exception in ShowWindow request failed. Err: {0}", new object[1] { ex.Message });
				}
			}
		}
		Environment.Exit(0);
	}

	private static IntPtr BringToFront(string vmname)
	{
		Logger.Info($"Starting BlueStacks {vmname} Frontend");
		string text = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
		IntPtr result = IntPtr.Zero;
		try
		{
			result = InteropWindow.BringWindowToFront(text, false, false);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in bringing existing frontend window for VM {0} to the foreground", new object[1] { vmname + " Err : " + ex.ToString() });
		}
		return result;
	}

	private static void Usage()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		string processName = Process.GetCurrentProcess().ProcessName;
		string text = $"{Strings.ProductDisplayName} Frontend";
		string text2 = "";
		text2 += string.Format("Usage:\n", new object[0]);
		text2 += $"    {processName} [OPTION] \n";
		text2 += string.Format("  -vmname:vmname   Specify the vmId for which to run HD-Player, default value is Android", new object[0]);
		if (Oem.Instance.IsMessageBoxToBeDisplayed)
		{
			Logger.Info("Displaying Message box");
			MessageBox.Show(text2, text);
			Logger.Info("Displayed Message box");
		}
		Environment.Exit(1);
	}

	private static void InitLog()
	{
		Logger.InitLog("Player", "Player", true);
		Logger.Info("BOOT_STAGE: Player starting");
		Console.SetOut(Logger.GetWriter());
		Console.SetError(Logger.GetWriter());
		AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
		Application.ThreadException += Application_ThreadException;
		Application.SetUnhandledExceptionMode((UnhandledExceptionMode)2);
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
	}

	private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
	{
		Logger.Info("Exiting frontend PID {0}", new object[1] { Process.GetCurrentProcess().Id });
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Logger.Error("Unhandled Exception:");
		Logger.Error(e.ExceptionObject.ToString());
		Environment.Exit(-3);
	}

	private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
	{
		Logger.Error("Unhandled Exception:");
		Logger.Error(e.Exception.ToString());
		Environment.Exit(-3);
	}
}
