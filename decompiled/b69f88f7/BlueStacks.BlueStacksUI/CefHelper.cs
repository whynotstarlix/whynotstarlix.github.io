using System;
using System.IO;
using System.Windows;
using BlueStacks.Common;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI;

internal class CefHelper : CefApp
{
	private bool mDevToolEnable;

	internal static bool CefInited { get; set; }

	public CefHelper()
	{
		if (RegistryManager.Instance.CefDevEnv == 0)
		{
			mDevToolEnable = false;
		}
	}

	protected override CefRenderProcessHandler GetRenderProcessHandler()
	{
		return (CefRenderProcessHandler)(object)new RenderProcessHandler();
	}

	protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
	{
		if (string.IsNullOrEmpty(processType))
		{
			commandLine.AppendSwitch("disable-gpu");
			commandLine.AppendSwitch("disable-gpu-compositing");
			commandLine.AppendSwitch("disable-smooth-scrolling");
			commandLine.AppendSwitch("--enable-system-flash");
			commandLine.AppendSwitch("ppapi-flash-path", Path.Combine(RegistryManager.Instance.CefDataPath, "pepflashplayer.dll"));
			commandLine.AppendSwitch("plugin-policy", "allow");
			commandLine.AppendSwitch("enable-media-stream", "1");
			if (mDevToolEnable)
			{
				commandLine.AppendSwitch("enable-begin-frame-scheduling");
			}
		}
	}

	internal static bool InitCef(string[] args, string mBSTProcessIdentifier)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		try
		{
			Logger.Info("Install Boot: CefRuntime.Load");
			CefRuntime.Load(RegistryManager.Instance.CefDataPath);
		}
		catch (DllNotFoundException ex)
		{
			Logger.Info("Install Boot: DllNotFoundException");
			MessageBox.Show(ex.Message, "Error!", (MessageBoxButton)0, (MessageBoxImage)16);
			return false;
		}
		catch (CefRuntimeException ex2)
		{
			Logger.Info("Install Boot: CefRuntimeException");
			MessageBox.Show(((Exception)ex2).Message, "Error!", (MessageBoxButton)0, (MessageBoxImage)16);
			return false;
		}
		catch (Exception ex3)
		{
			Logger.Info("Install Boot: ex");
			MessageBox.Show(ex3.ToString(), "Error!", (MessageBoxButton)0, (MessageBoxImage)16);
			return false;
		}
		CefMainArgs val = new CefMainArgs(args);
		CefHelper cefHelper = new CefHelper();
		CefRuntime.EnableHighDpiSupport();
		if (CefRuntime.ExecuteProcess(val, (CefApp)(object)cefHelper, IntPtr.Zero) != -1)
		{
			return false;
		}
		string userAgent = "Mozilla/5.0(Windows NT 6.2; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
		if (!SystemUtils.IsOs64Bit())
		{
			userAgent = "Mozilla/5.0(Windows NT 6.2; WOW64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
		}
		CefSettings val2 = new CefSettings
		{
			SingleProcess = true,
			WindowlessRenderingEnabled = true,
			MultiThreadedMessageLoop = true,
			LogSeverity = (CefLogSeverity)1,
			BackgroundColor = new CefColor(byte.MaxValue, (byte)39, (byte)41, (byte)65),
			CachePath = Path.Combine(RegistryManager.Instance.CefDataPath, "Cache"),
			PersistSessionCookies = true,
			UserAgent = userAgent,
			Locale = RegistryManager.Instance.UserSelectedLocale
		};
		if (RegistryManager.Instance.CefDebugPort != 0)
		{
			val2.RemoteDebuggingPort = RegistryManager.Instance.CefDebugPort;
		}
		try
		{
			CefRuntime.Initialize(val, val2, (CefApp)(object)cefHelper, IntPtr.Zero);
			Logger.Info("Install Boot: cef Initialized");
		}
		catch (CefRuntimeException ex4)
		{
			MessageBox.Show(((object)ex4).ToString(), "Error!", (MessageBoxButton)0, (MessageBoxImage)16);
			return false;
		}
		CefInited = true;
		Logger.Info("Install Boot: cef Initialize completed");
		return true;
	}
}
