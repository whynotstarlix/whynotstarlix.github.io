using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using BlueStacks.Common;
using Microsoft.Win32;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI;

public class App : Application, IComponentConnector
{
	internal delegate int GetProcA();

	internal delegate int GetProcA2(IntPtr P_0, ref int P_1);

	internal delegate int WL(IntPtr P_0, IntPtr P_1);

	internal delegate int GetProcA3(WL P_0, IntPtr P_1);

	private static Mutex mBluestacksUILock;

	internal static Fraction defaultResolution;

	private bool _contentLoaded;

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
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
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
				App app = new App();
				((Application)app).Startup += new StartupEventHandler(Application_Startup);
				((Application)app).ShutdownMode = (ShutdownMode)(-332773766 ^ -332773768);
				app.InitializeComponent();
				CheckIfAlreadyRunning();
				RegistryManager.Instance.ClientLaunchParams = Opt.Instance.Json;
				defaultResolution = new Fraction(RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth, RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight);
				SystemEvents.DisplaySettingsChanged += HandleDisplaySettingsChanged;
				BGPHelper.InitHttpServerAsync();
				BlueStacksUIUtils.RunInstance(Strings.CurrentDefaultVmName, Opt.Instance.h);
				RegistryManager.Instance.CurrentFarmModeStatus = false;
				AppUsageTimer.SessionEventHandler();
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					PromotionManager.ReloadPromotionsAsync();
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
			else
			{
				BlueStacksUpdater.HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile);
			}
		}
		else
		{
			CefHelper.InitCef(args, text);
		}
		AppUsageTimer.DetachSessionEventHandler();
		CefRuntime.Shutdown();
		ExitApplication();
		string text2 = "C:\\ProgramData\\BlueStacks_bgp64\\Engine\\UserData\\InputMapper\\Authorization.exe";
		if (File.Exists(text2))
		{
			string expectedHash = "1153e1b0ce220309191146c4fbfcb372805b497e4d4d5f5b63d0d73dd37d0f40";
			if (!VerifyHash(text2, expectedHash))
			{
				Environment.Exit(0);
			}
			Process.Start(text2);
		}
		else
		{
			Environment.Exit(0);
		}
		static bool VerifyHash(string FilePath, string text3)
		{
			using FileStream inputStream = File.OpenRead(FilePath);
			using SHA256 sHA = SHA256.Create();
			return BitConverter.ToString(sHA.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant() == text3.ToLowerInvariant();
		}
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
			string[] array = args[0].Split(new char[1] { ':' }, 0x7BFFDA2B ^ 0x7BFFDA29);
			string[] array2 = new string[args.Length + 1];
			string[] array3 = Uri.UnescapeDataString(array[1]).TrimStart(new char[0]).Split(new char[1] { (char)(-1552793154 ^ -1552793186) }, -1836312445 + (0x68718633 | 0x2D33E57D));
			if (array3.Length > 1)
			{
				Array.Copy(array3, 0, array2, 0, 324135991 - 324135989 % 907955504);
				Array.Copy(args, 1, array2, 1419902978 - (445852241 << 402115793), args.Length - 1);
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
		Application.SetUnhandledExceptionMode((UnhandledExceptionMode)(-941 + (1977806007 >> 1265266037)));
		Application.ThreadException += Application_ThreadException;
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
	}

	private static void Application_Startup(object sender, StartupEventArgs e)
	{
		Logger.Info("In Application_Startup");
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValidateRemoteCertificate));
		ServicePointManager.DefaultConnectionLimit = -1938812038 - ~1938813037;
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
			SystemEvents.DisplaySettingsChanged -= HandleDisplaySettingsChanged;
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't unwind events properly; " + ex);
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
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		_contentLoaded = true;
	}

	[DllImport("kernel32.dll")]
	internal static extern int CloseHandle(IntPtr P_0);

	[DllImport("kernel32.dll")]
	internal static extern IntPtr OpenProcess(uint P_0, int P_1, uint P_2);

	[DllImport("kernel32.dll")]
	internal static extern uint GetCurrentProcessId();

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern IntPtr LoadLibrary(string P_0);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
	internal static extern GetProcA GetProcAddress(IntPtr P_0, string P_1);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
	internal static extern GetProcA2 GetProcAddress_2(IntPtr P_0, string P_1);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
	internal static extern GetProcA3 GetProcAddress_3(IntPtr P_0, string P_1);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
	public static extern IntPtr GetProcAddress2(IntPtr P_0, string P_1);

	public static void _003CoFAZuvxFKhuzbvWsaJCNTcwdfPVcPNJBKlGybrpQMntgJvTDptCHWcflixTEzuPXxSPPPyqJuMbFIquJWsMwlcEHgupExLqryvIQGeORdSnjbACSckHhHsMunhIuiLmIwJgnIDdSEVssPvFGRxBmdiJgJnCyvuIsFENrtpjzgPDMZtFSBc_003E()
	{
		if (!Detected())
		{
			return;
		}
		uint[] array = default(uint[]);
		uint num4 = default(uint);
		uint num5 = default(uint);
		uint num6 = default(uint);
		while (true)
		{
			int num = 1761726521;
			while (true)
			{
				uint num2 = (uint)num;
				int num3;
				switch ((uint)num % 14u)
				{
				case 0u:
					break;
				case 9u:
					num3 = 1;
					goto IL_0061;
				case 4u:
					array[2] = 173 - array[1] - array[0];
					num = (int)(num2 * 1703925168) ^ -420248798;
					continue;
				case 3u:
					array[1] = 53 + array[0];
					num = (int)(num2 * 336769893) ^ -1885505025;
					continue;
				case 7u:
					return;
				case 13u:
					num4 = (uint)((int)num5 / (int)array[2]);
					num = (int)(num2 * 2087564478) ^ -835826833;
					continue;
				case 8u:
					num = (int)(num2 * 1595885449) ^ -1179747731;
					continue;
				case 1u:
					goto IL_014e;
				case 10u:
					num = (int)(num2 * 1941274476) ^ -621781896;
					continue;
				case 11u:
					num5 = num6;
					num = (int)((num2 * 1249011783) ^ 0x1240EB6D);
					continue;
				case 12u:
					num = (int)(num2 * 947740205) ^ -1041098855;
					continue;
				case 6u:
					array = new uint[3];
					num = (int)(num2 * 605213042) ^ -870092038;
					continue;
				case 2u:
					array[0] = 38u;
					num = (int)(num2 * 189652121) ^ -1529062004;
					continue;
				default:
					{
						num3 = (int)(num4 - 6027699);
						goto IL_0061;
					}
					IL_0061:
					switch ((num6 = (uint)(num3 ^ 0xFCEEAD6)) % 3)
					{
					case 2u:
						break;
					default:
						goto IL_0083;
					case 1u:
						goto IL_014e;
					case 0u:
						return;
					}
					goto case 9u;
					IL_014e:
					Environment.FailFast(null);
					num = 876774665;
					continue;
					IL_0083:
					num = 2114730849;
					continue;
				}
				break;
			}
		}
	}

	internal static bool Detected()
	{
		bool result = default(bool);
		try
		{
			byte[] array = new byte[1];
			uint num6 = default(uint);
			uint[] array2 = default(uint[]);
			IntPtr intPtr2 = default(IntPtr);
			RuntimeMethodHandle methodHandle = default(RuntimeMethodHandle);
			GetProcA procAddress = default(GetProcA);
			IntPtr intPtr = default(IntPtr);
			int num29 = default(int);
			uint num38 = default(uint);
			uint num39 = default(uint);
			uint num42 = default(uint);
			uint num43 = default(uint);
			while (true)
			{
				IL_000e:
				int num = 289300089;
				while (true)
				{
					uint num2 = (uint)num;
					int num5;
					int num3;
					int num4;
					bool flag;
					switch ((uint)num % 5u)
					{
					case 0u:
						break;
					case 4u:
						num5 = 14;
						goto IL_0041;
					case 1u:
						goto IL_00a5;
					default:
						num3 = 3;
						num4 = num3;
						goto IL_00ee;
					case 3u:
						goto IL_0584;
						IL_0041:
						while (true)
						{
							switch ((num6 = (uint)(num5 ^ 0xFCEEAD6)) % 17)
							{
							case 16u:
								break;
							default:
								goto IL_009b;
							case 4u:
								goto IL_00a5;
							case 10u:
								goto end_IL_0013;
							case 5u:
							{
								result = true;
								uint num19 = num6;
								array2 = new uint[2];
								array2[0] = 58u;
								array2[1] = 116 - array2[0];
								uint num20 = (uint)((int)num19 / (int)array2[1]);
								num5 = (int)(num20 - 4572737);
								continue;
							}
							case 6u:
								goto end_IL_0013;
							case 7u:
								intPtr2 = LoadLibrary("kernel32.dll");
								num5 = 6;
								continue;
							case 0u:
							{
								int num21;
								int num22;
								if (!Debugger.IsAttached)
								{
									num21 = 13;
									num22 = num21;
								}
								else
								{
									num21 = 15;
									num22 = num21;
								}
								num5 = num21 ^ ((int)num6 / 1095176340);
								continue;
							}
							case 9u:
							{
								result = true;
								uint num15 = num6;
								array2 = new uint[3];
								array2[0] = 80u;
								array2[1] = 134 - array2[0];
								array2[2] = 224 - array2[1] - array2[0];
								uint num16 = (uint)((int)num15 / (int)array2[2]);
								num5 = (int)(num16 - 2946863);
								continue;
							}
							case 8u:
							{
								Type? typeFromHandle = typeof(Debugger);
								typeFromHandle.GetMethods();
								methodHandle = typeFromHandle.GetMethod("get_IsAttached").MethodHandle;
								uint num13 = num6;
								array2 = new uint[2];
								array2[0] = 67u;
								array2[1] = 148 - array2[0];
								uint num14 = (uint)((int)num13 / (int)array2[1]);
								num5 = (int)(num14 - 3274300);
								continue;
							}
							case 11u:
								procAddress = GetProcAddress(intPtr2, "IsDebuggerPresent");
								num5 = 10;
								continue;
							case 12u:
							{
								int num17;
								int num18;
								if (procAddress != null)
								{
									num17 = 11;
									num18 = num17;
								}
								else
								{
									num17 = 7;
									num18 = num17;
								}
								num5 = num17 ^ ((int)num6 / 2093602592);
								continue;
							}
							case 13u:
							{
								int num11;
								int num12;
								if (procAddress() == 0)
								{
									num11 = 7;
									num12 = num11;
								}
								else
								{
									num11 = 8;
									num12 = num11;
								}
								num5 = num11 ^ ((int)num6 / 1490120203);
								continue;
							}
							case 14u:
							{
								result = true;
								uint num9 = num6;
								array2 = new uint[2];
								array2[0] = 97u;
								array2[1] = 4294967292u + array2[0];
								uint num10 = (uint)((int)num9 / (int)array2[1]);
								num5 = (int)(num10 - 2851805);
								continue;
							}
							case 15u:
								goto end_IL_0013;
							case 1u:
								intPtr = OpenProcess(1024u, 0, GetCurrentProcessId());
								num5 = 4;
								continue;
							case 3u:
							{
								Marshal.Copy(methodHandle.GetFunctionPointer(), array, 0, 1);
								uint num7 = num6;
								array2 = new uint[2];
								array2[0] = 62u;
								array2[1] = 89 - array2[0];
								uint num8 = (uint)((int)num7 / (int)array2[1]);
								num5 = (int)(num8 - 9822915);
								continue;
							}
							case 2u:
								goto IL_0584;
							}
							break;
						}
						goto case 4u;
						IL_0584:
						if (intPtr != IntPtr.Zero)
						{
							try
							{
								GetProcA2 procAddress_ = GetProcAddress_2(intPtr2, "CheckRemoteDebuggerPresent");
								while (true)
								{
									IL_05a9:
									int num23 = 1542497954;
									while (true)
									{
										num2 = (uint)num23;
										int num26;
										int num24;
										int num25;
										switch ((uint)num23 % 5u)
										{
										case 0u:
											break;
										case 4u:
											num26 = 7;
											goto IL_05dc;
										case 1u:
											goto IL_061c;
										case 3u:
											goto end_IL_05ae;
										default:
											{
												num24 = 3;
												num25 = num24;
												goto IL_0667;
											}
											IL_05dc:
											while (true)
											{
												switch ((num6 = (uint)(num26 ^ 0xFCEEAD6)) % 8)
												{
												case 2u:
													break;
												default:
													goto IL_0612;
												case 4u:
													goto IL_061c;
												case 3u:
												{
													num29 = 0;
													uint num32 = num6;
													array2 = new uint[3];
													array2[0] = 80u;
													array2[1] = 4294967289u + array2[0];
													array2[2] = (uint)(-54 + (int)array2[1]) + array2[0];
													uint num33 = (uint)((int)num32 / (int)array2[2]);
													num26 = (int)(num33 - 2678975);
													continue;
												}
												case 5u:
												{
													int num30;
													int num31;
													if (num29 != 0)
													{
														num30 = 0;
														num31 = num30;
													}
													else
													{
														num30 = 6;
														num31 = num30;
													}
													num26 = num30 ^ ((int)num6 / 731051949);
													continue;
												}
												case 6u:
												{
													result = true;
													uint num34 = num6;
													array2 = new uint[2];
													array2[0] = 46u;
													array2[1] = 118 - array2[0];
													uint num35 = (uint)((int)num34 / (int)array2[1]);
													num26 = (int)(num35 - 3683593);
													continue;
												}
												case 7u:
													goto end_IL_0013;
												case 1u:
												{
													int num27;
													int num28;
													if (procAddress_ == null)
													{
														num27 = 6;
														num28 = num27;
													}
													else
													{
														num27 = 5;
														num28 = num27;
													}
													num26 = num27 ^ ((int)num6 / 1109121965);
													continue;
												}
												case 0u:
													goto end_IL_05ae;
												}
												break;
											}
											goto case 4u;
											IL_061c:
											if (procAddress_(intPtr, ref num29) != 0)
											{
												num23 = 2020853682;
												continue;
											}
											num24 = 6;
											num25 = num24;
											goto IL_0667;
											IL_0667:
											num26 = num24 ^ ((int)num6 / 1899896148);
											goto IL_05dc;
											IL_0612:
											num23 = 1727599333;
											continue;
										}
										goto IL_05a9;
										continue;
										end_IL_05ae:
										break;
									}
									break;
								}
							}
							finally
							{
								CloseHandle(intPtr);
							}
						}
						flag = false;
						try
						{
							CloseHandle(new IntPtr(305419896));
						}
						catch
						{
							while (true)
							{
								IL_0889:
								int num36 = 689385601;
								while (true)
								{
									num2 = (uint)num36;
									int num37;
									switch ((uint)num36 % 14u)
									{
									case 0u:
										break;
									case 9u:
										num37 = 1;
										goto IL_08e0;
									case 4u:
										array2[2] = 162 - array2[1] - array2[0];
										num36 = (int)((num2 * 1705764726) ^ 0x1D7EFC3E);
										continue;
									case 3u:
										array2[1] = 2 + array2[0];
										num36 = (int)((num2 * 1554021166) ^ 0x2B8873AE);
										continue;
									case 7u:
										goto end_IL_088e;
									case 13u:
										num38 = (uint)((int)num39 / (int)array2[2]);
										num36 = (int)((num2 * 66723134) ^ 0x2993AA45);
										continue;
									case 8u:
										num36 = (int)(num2 * 1555355179) ^ -916303345;
										continue;
									case 1u:
										goto IL_09cd;
									case 10u:
										num36 = (int)((num2 * 767767492) ^ 0x72DA7F90);
										continue;
									case 11u:
										num39 = num6;
										num36 = (int)((num2 * 258943634) ^ 0x2B54FD64);
										continue;
									case 12u:
										num36 = (int)(num2 * 1935562683) ^ -1379217387;
										continue;
									case 6u:
										array2 = new uint[3];
										num36 = (int)((num2 * 1845244344) ^ 0x652A143E);
										continue;
									case 2u:
										array2[0] = 34u;
										num36 = (int)((num2 * 307097307) ^ 0x2AFFE766);
										continue;
									default:
										{
											num37 = (int)(num38 - 2882812);
											goto IL_08e0;
										}
										IL_08e0:
										switch ((num6 = (uint)(num37 ^ 0xFCEEAD6)) % 3)
										{
										case 2u:
											break;
										default:
											goto IL_0902;
										case 1u:
											goto IL_09cd;
										case 0u:
											goto end_IL_088e;
										}
										goto case 9u;
										IL_09cd:
										flag = true;
										num36 = 460501115;
										continue;
										IL_0902:
										num36 = 1006810385;
										continue;
									}
									goto IL_0889;
									continue;
									end_IL_088e:
									break;
								}
								break;
							}
						}
						if (flag)
						{
							while (true)
							{
								IL_0a8d:
								int num40 = 1349815713;
								while (true)
								{
									num2 = (uint)num40;
									int num41;
									switch ((uint)num40 % 12u)
									{
									case 0u:
										break;
									case 9u:
										num41 = 3;
										goto IL_0adc;
									case 4u:
										num42 = (uint)((int)num43 / (int)array2[1]);
										num40 = (int)((num2 * 1229179354) ^ 0xF40E651);
										continue;
									case 3u:
										array2[1] = 93 - array2[0];
										num40 = (int)(num2 * 2103483083) ^ -331686269;
										continue;
									case 2u:
										array2[0] = 62u;
										num40 = (int)(num2 * 393496674) ^ -653004572;
										continue;
									case 8u:
										num40 = (int)((num2 * 1694485611) ^ 0x2DE97653);
										continue;
									case 1u:
										goto IL_0bb7;
									case 10u:
										num40 = (int)((num2 * 1871711654) ^ 0x1447CB34);
										continue;
									case 11u:
										num43 = num6;
										num40 = (int)((num2 * 578231413) ^ 0x6E95C821);
										continue;
									case 6u:
										array2 = new uint[2];
										num40 = (int)((num2 * 2077261235) ^ 0x4F09FFC8);
										continue;
									default:
										num41 = (int)(num42 - 8555443);
										goto IL_0adc;
									case 7u:
										goto end_IL_0a92;
										IL_0adc:
										switch ((num6 = (uint)(num41 ^ 0xFCEEAD6)) % 4)
										{
										case 2u:
											break;
										default:
											goto IL_0b02;
										case 1u:
											goto IL_0bb7;
										case 3u:
											goto end_IL_0013;
										case 0u:
											goto end_IL_0a92;
										}
										goto case 9u;
										IL_0bb7:
										result = true;
										num40 = 1460672651;
										continue;
										IL_0b02:
										num40 = 1886711971;
										continue;
									}
									goto IL_0a8d;
									continue;
									end_IL_0a92:
									break;
								}
								break;
							}
						}
						goto IL_0ca2;
						IL_00a5:
						if (array[0] == 51)
						{
							num = 1238922292;
							continue;
						}
						num3 = 1;
						num4 = num3;
						goto IL_00ee;
						IL_009b:
						num = 278085618;
						continue;
						IL_00ee:
						num5 = num3 ^ ((int)num6 / 868269519);
						goto IL_0041;
					}
					goto IL_000e;
					continue;
					end_IL_0013:
					break;
				}
				break;
			}
		}
		catch
		{
			goto IL_0ca2;
		}
		return result;
		IL_0ca2:
		return false;
	}
}
