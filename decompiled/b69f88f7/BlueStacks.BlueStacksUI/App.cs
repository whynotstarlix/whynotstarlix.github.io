using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using BlueStacks.Common;
using Bluester;
using Microsoft.Win32;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI;

public class App : Application, IComponentConnector
{
	private static Mutex mBluestacksUILock;

	internal static Fraction defaultResolution;

	private bool _contentLoaded;

	public static StartupEventHandler Application_StartupHandler;

	public static EventHandler HandleDisplaySettingsChangedHandler;

	public static ThreadExceptionEventHandler Application_ThreadExceptionHandler;

	public static UnhandledExceptionEventHandler CurrentDomain_UnhandledExceptionHandler;

	private static DiscordHandler discordHandler;

	public static Mutex BlueStacksUILock
	{
		get
		{
			return mBluestacksUILock;
		}
		set
		{
			mBluestacksUILock = value;
		}
	}

	internal static bool IsApplicationActive { get; set; }

	[STAThread]
	public static void Main(string[] args)
	{
		Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		InitExceptionAndLogging();
		ProcessUtils.LogParentProcessDetails();
		if (args != null)
		{
			ParseWebMagnetArgs(ref args);
			((GetOpt)Opt.Instance).Parse(args);
		}
		Oem.CurrentOemFilePath = Path.Combine(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.Trim(new char[1] { '\\' })).FullName, "Engine"), "Oem.cfg");
		PortableInstaller.CheckAndRunPortableInstaller();
		if (!RegistryManager.Instance.Guest.ContainsKey(Opt.Instance.vmname))
		{
			Opt.Instance.vmname = "Android";
		}
		Strings.CurrentDefaultVmName = Opt.Instance.vmname;
		if (Opt.Instance.mergeCfg)
		{
			KMManager.MergeConfig(Opt.Instance.newPDPath);
			Environment.Exit(0);
		}
		string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
		if (!string.Join(string.Empty, args).Contains(text))
		{
			Logger.Info("BOOT_STAGE: Client starting");
			RegistryManager.ClientThemeName = RegistryManager.Instance.GetClientThemeNameFromRegistry();
			if (FeatureManager.Instance.IsCustomUIForDMM || !BlueStacksUpdater.CheckIfDownloadedFileExist())
			{
				discordHandler = new DiscordHandler();
				discordHandler.Initialize();
				RunMain();
			}
			BlueStacksUpdater.HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile);
		}
		else
		{
			CefHelper.InitCef(args, text);
		}
		AppUsageTimer.DetachSessionEventHandler();
		CefRuntime.Shutdown();
		ExitApplication();
	}

	private static void HandleDisplaySettingsChanged(object sender, EventArgs e)
	{
		try
		{
			foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
			{
				if (item != null && !item.mClosed)
				{
					item.HandleDisplaySettingsChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
		}
	}

	private static void ParseWebMagnetArgs(ref string[] args)
	{
		if (args.Length != 0 && args[0].StartsWith("bluestacksgp:", StringComparison.InvariantCultureIgnoreCase))
		{
			Logger.Info("Handling web uri: " + args[0]);
			string[] array = args[0].Split(new char[1] { ':' }, 2);
			string[] array2 = new string[args.Length + 1];
			string[] array3 = Uri.UnescapeDataString(array[1]).TrimStart(new char[0]).Split(new char[1] { ' ' }, 2);
			if (array3.Length > 1)
			{
				Array.Copy(array3, 0, array2, 0, 2);
				Array.Copy(args, 1, array2, 2, args.Length - 1);
				args = array2;
			}
			else
			{
				args[0] = array3[0];
			}
		}
	}

	private static void InitExceptionAndLogging()
	{
		Logger.InitLog("BlueStacksUI", "BlueStacksUI", true);
		Application.SetUnhandledExceptionMode((UnhandledExceptionMode)2);
		ThreadExceptionEventHandler threadExceptionEventHandler;
		if ((threadExceptionEventHandler = Application_ThreadExceptionHandler) == null)
		{
			threadExceptionEventHandler = (Application_ThreadExceptionHandler = Application_ThreadException);
		}
		Application.ThreadException += threadExceptionEventHandler;
		AppDomain currentDomain = AppDomain.CurrentDomain;
		UnhandledExceptionEventHandler value;
		if ((value = CurrentDomain_UnhandledExceptionHandler) == null)
		{
			value = (CurrentDomain_UnhandledExceptionHandler = CurrentDomain_UnhandledException);
		}
		currentDomain.UnhandledException += value;
	}

	private static void Application_Startup(object sender, StartupEventArgs e)
	{
		Logger.Info("In Application_Startup");
		new FPSControl().Init();
		new ProcessControl().Init();
		new AutoAcceptMatch().Init();
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValidateRemoteCertificate));
		ServicePointManager.DefaultConnectionLimit = 1000;
	}

	private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
	{
		return true;
	}

	private static void CheckIfAlreadyRunning()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp64"))
			{
				Logger.Info("Disk compaction is running in background");
				foreach (string item in GetProcessExecutionPath.GetApplicationPath(Process.GetProcessesByName("DiskCompactionTool")))
				{
					if (item.Equals(Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe"), StringComparison.InvariantCultureIgnoreCase))
					{
						CustomMessageWindow val = new CustomMessageWindow
						{
							ImageName = "ProductLogo"
						};
						val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_HEADING", "");
						val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_MESSAGE", "");
						val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
						val.CloseButtonHandle((Predicate<object>)null, (object)null);
						((Window)val).ShowDialog();
						Logger.Info("Disk compaction running for this instance. Exiting this instance");
						ExitApplication();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to check if disk compaction is running: " + ex.Message);
		}
		string text = default(string);
		if (!Opt.Instance.force && ProcessUtils.IsAnyInstallerProcesRunning(ref text) && !string.IsNullOrEmpty(text))
		{
			Logger.Info(text + " process is running. Exiting BlueStacks");
			ExitApplication();
		}
		if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_BlueStacksUI_Lockbgp64", ref mBluestacksUILock))
		{
			try
			{
				Logger.Info("Relaunching client for vm : " + Opt.Instance.vmname);
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"vmname",
						Opt.Instance.vmname
					},
					{
						"hidden",
						Opt.Instance.h.ToString(CultureInfo.InvariantCulture)
					}
				};
				if (Opt.Instance.launchedFromSysTray)
				{
					dictionary.Add("all", "True");
				}
				if (!string.IsNullOrEmpty(Opt.Instance.Json))
				{
					dictionary.Add("json", Opt.Instance.Json);
					string text2 = HTTPUtils.SendRequestToClient("openPackage", dictionary, Opt.Instance.vmname, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					Logger.Debug("OpenPackage result: " + text2);
				}
				else
				{
					string text3 = HTTPUtils.SendRequestToClient("showWindow", dictionary, Opt.Instance.vmname, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					Logger.Debug("ShowWindow result: " + text3);
				}
			}
			catch (Exception ex2)
			{
				Logger.Error(ex2.ToString());
			}
			Logger.Info("BlueStacksUI already running. Exiting this instance");
			ExitApplication();
			return;
		}
		try
		{
			Logger.Debug("Checking for existing process not exited");
			List<Process> list = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).ToList();
			if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp64"))
			{
				return;
			}
			foreach (Process item2 in list)
			{
				if (item2.Id != Process.GetCurrentProcess().Id)
				{
					item2.Kill();
				}
			}
		}
		catch (Exception ex3)
		{
			Logger.Warning("Ignoring error closing previous instances" + ex3.ToString());
		}
	}

	private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (CheckForIgnoredExceptions(e.Exception.ToString()))
		{
			Logger.Error("Unhandled Thread Exception:");
			Logger.Error(e.Exception.ToString());
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				MessageBox.Show("BlueStacks App Player.\nError: " + e.Exception.ToString());
			}
			ExitApplication();
		}
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (CheckForIgnoredExceptions(((Exception)e.ExceptionObject).ToString()))
		{
			CheckForIgnoredExceptions(((Exception)e.ExceptionObject).ToString());
			Logger.Error("Unhandled Application Exception.");
			Logger.Error("Err: " + e.ExceptionObject.ToString());
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				MessageBox.Show("BlueStacks App Player.\nError: " + ((Exception)e.ExceptionObject).ToString());
			}
			ExitApplication();
		}
	}

	private static bool CheckForIgnoredExceptions(string s)
	{
		if (s.Contains("GetFocusedElementFromWinEvent"))
		{
			Logger.Warning("Ignoring Unhandled Application Exception: " + s);
			return false;
		}
		return true;
	}

	internal static void ExitApplication()
	{
		foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
		{
			if (item != null && !item.mClosed)
			{
				item.ForceCloseWindow();
			}
		}
		UnwindEvents();
		ReleaseLock();
		Process.GetCurrentProcess().Kill();
	}

	internal static void UnwindEvents()
	{
		try
		{
			EventHandler eventHandler;
			if ((eventHandler = HandleDisplaySettingsChangedHandler) == null)
			{
				eventHandler = (HandleDisplaySettingsChangedHandler = HandleDisplaySettingsChanged);
			}
			SystemEvents.DisplaySettingsChanged -= eventHandler;
		}
		catch (Exception ex)
		{
			string text = "Couldn't unwind events properly; ";
			Logger.Error(text + ex);
		}
	}

	internal static void ReleaseLock()
	{
		try
		{
			BluestacksProcessHelper.TakeLock("Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp64");
			if (BlueStacksUILock != null)
			{
				BlueStacksUILock.Close();
				BlueStacksUILock = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Ignoring Exception while releasing lock. Err : " + ex.ToString());
		}
	}

	private void Application_Activated(object sender, EventArgs e)
	{
		IsApplicationActive = true;
		foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
		{
			item.SendTempGamepadState(enable: true);
		}
	}

	private void Application_Deactivated(object sender, EventArgs e)
	{
		IsApplicationActive = false;
		foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
		{
			if (item.mStreamingModeEnabled)
			{
				item.SendTempGamepadState(enable: true);
			}
			else
			{
				item.SendTempGamepadState(enable: false);
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		((Application)this).Activated += Application_Activated;
		((Application)this).Deactivated += Application_Deactivated;
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/app.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		_contentLoaded = true;
	}

	public static void RunMain()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0021: Expected O, but got Unknown
		App app = new App();
		StartupEventHandler val;
		if ((val = Application_StartupHandler) == null)
		{
			StartupEventHandler val2 = Application_Startup;
			Application_StartupHandler = val2;
			val = val2;
		}
		((Application)app).Startup += val;
		((Application)app).ShutdownMode = (ShutdownMode)2;
		app.InitializeComponent();
		CheckIfAlreadyRunning();
		RegistryManager.Instance.ClientLaunchParams = Opt.Instance.Json;
		defaultResolution = new Fraction(RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth, RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight);
		EventHandler eventHandler;
		if ((eventHandler = HandleDisplaySettingsChangedHandler) == null)
		{
			eventHandler = (HandleDisplaySettingsChangedHandler = HandleDisplaySettingsChanged);
		}
		SystemEvents.DisplaySettingsChanged += eventHandler;
		BGPHelper.InitHttpServerAsync();
		BlueStacksUIUtils.RunInstance(Strings.CurrentDefaultVmName, Opt.Instance.h);
		RegistryManager.Instance.CurrentFarmModeStatus = false;
		AppUsageTimer.SessionEventHandler();
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			GrmManager.UpdateGrmAsync();
			GuidanceCloudInfoManager.Instance.AppsGuidanceCloudInfoRefresh();
		}
		if (!FeatureManager.Instance.IsHtmlHome)
		{
			BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].CreateFirebaseBrowserControl();
		}
		MemoryManager.TrimMemory(true);
		((Application)app).Run();
	}
}
