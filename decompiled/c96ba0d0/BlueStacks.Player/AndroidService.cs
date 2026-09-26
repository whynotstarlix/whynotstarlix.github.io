using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BlueStacks.Common;
using Microsoft.Win32;

namespace BlueStacks.Player;

public class AndroidService
{
	public class CmdResult
	{
		public string StdOut { get; set; } = string.Empty;

		public string StdErr { get; set; } = string.Empty;

		public int ExitCode { get; set; }
	}

	private static readonly Lazy<AndroidService> _instance = new Lazy<AndroidService>(() => new AndroidService());

	private static Thread _androidThread;

	private static EventWaitHandle _hdQuitEvent;

	private readonly StateMachine _stateMachine;

	private readonly string _vmName;

	private bool _disposed;

	private static readonly Dictionary<int, (string LoggerMessage, Action<int> UpdateAction)> _portMappings = new Dictionary<int, (string, Action<int>)>
	{
		{
			9999,
			("Bst Android Port Updated to {0}", (int port) =>
			{
				InstanceRegistry defaultGuest = RegistryManager.Instance.DefaultGuest;
				defaultGuest.BstAndroidPort = port;
				SetProperty(defaultGuest, "NetworkRedirectTcp9999", port);
			})
		},
		{
			5555,
			("Bst 5555 Port Updated to {0}", (int port) =>
			{
				InstanceRegistry defaultGuest = RegistryManager.Instance.DefaultGuest;
				defaultGuest.BstAdbPort = port;
				SetProperty(defaultGuest, "NetworkRedirectTcp5555", port);
			})
		},
		{
			6666,
			("Bst 6666 Port Updated to {0}", (int port) =>
			{
				SetProperty(RegistryManager.Instance.DefaultGuest, "NetworkRedirectTcp6666", port);
			})
		},
		{
			7777,
			("Bst 7777 Port Updated to {0}", (int port) =>
			{
				SetProperty(RegistryManager.Instance.DefaultGuest, "NetworkRedirectTcp7777", port);
			})
		}
	};

	public static AndroidService Instance => _instance.Value;

	public static void StartAsync()
	{
		_androidThread = new Thread(InitializeAndroidService)
		{
			IsBackground = true,
			Name = "AndroidServiceThread"
		};
		_androidThread.Start();
	}

	private void RunDebug()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		ConsoleControl.SetHandler((Handler)HandleConsoleControl);
		OnStart();
		Logger.Info("Waiting for signal from user");
		_stateMachine.WaitForTermination();
		OnStop();
	}

	private void OnStart()
	{
		Logger.Info("Starting {0} VM", new object[1] { _vmName });
		try
		{
			Utils.SetCurrentEngineStateAndGlTransportValue((EngineState)0, MultiInstanceStrings.VmName);
			StartInternal();
		}
		catch (Exception arg)
		{
			Logger.Error($"Exception in OnStart(): {arg}");
			throw;
		}
	}

	private void OnStop()
	{
		if (_disposed)
		{
			return;
		}
		Logger.Info("Stopping {0} VM", new object[1] { _vmName });
		try
		{
			StopInternal();
			Logger.Info("{0} VM is stopped", new object[1] { _vmName });
		}
		catch (Exception arg)
		{
			Logger.Error($"Exception in OnStop(): {arg}");
			throw;
		}
		finally
		{
			_disposed = true;
		}
	}

	private void UpdateNetworkChangePortRedirectInfo(int guestPort, int hostPort)
	{
		if (_portMappings.TryGetValue(guestPort, out (string, Action<int>) value))
		{
			Logger.Info(value.Item1, new object[1] { hostPort });
			value.Item2(hostPort);
		}
	}

	private AndroidService()
	{
		_vmName = Strings.CurrentDefaultVmName;
		_stateMachine = new StateMachine(_vmName);
	}

	private static void InitializeAndroidService()
	{
		try
		{
			EnsureAndroidConfiguration();
			StartCoreServices();
			InitializeAndroidServiceInstance();
			StartQuitMonitor();
			Instance.RunDebug();
		}
		catch (Exception arg)
		{
			Logger.Error($"Failed to initialize Android service: {arg}");
			throw;
		}
	}

	private static void EnsureAndroidConfiguration()
	{
		if (!Utils.CheckIfAndroidBstkExistAndValid(Strings.CurrentDefaultVmName))
		{
			Utils.CreateBstkFileFromPrev(MultiInstanceStrings.VmName);
		}
	}

	private static void StartCoreServices()
	{
		ServiceManager.StartService(Strings.BlueStacksDriverName, true);
		ServiceHelper.FindAndSyncConfig();
		MonitorLocator.Publish(Strings.CurrentDefaultVmName, (uint)Process.GetCurrentProcess().Id);
	}

	private static void InitializeAndroidServiceInstance()
	{
		Logger.Info("Android service instance initialized");
	}

	private static void StartQuitMonitor()
	{
		ThreadPool.QueueUserWorkItem((object _) =>
		{
			_hdQuitEvent = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, $"HD-Plus-{Process.GetCurrentProcess().Id}");
			_hdQuitEvent.WaitOne();
			Instance.OnStop();
		});
	}

	public static CmdResult RunCmd(string command, string arguments, bool throwOnNonZero = false)
	{
		Logger.Info("Command Runner: " + command + " " + arguments);
		CmdResult result = new CmdResult();
		Process process = CreateProcess(command, arguments);
		try
		{
			process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				HandleOutputData(e.Data, result, process.Id, "OUT");
			};
			process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
			{
				HandleOutputData(e.Data, result, process.Id, "ERR");
			};
			try
			{
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				result.ExitCode = process.ExitCode;
				Logger.Info($"{process.Id} EXIT: {process.ExitCode}");
				ValidateExitCode(process.ExitCode, throwOnNonZero);
			}
			catch (Exception arg)
			{
				Logger.Error($"Command execution failed: {arg}");
				throw;
			}
		}
		finally
		{
			if (process != null)
			{
				((IDisposable)process).Dispose();
			}
		}
		return result;
	}

	public static async Task<CmdResult> RunCmdAsync(string command, string arguments, bool throwOnNonZero = false)
	{
		return await Task.Run(() => RunCmd(command, arguments, throwOnNonZero));
	}

	private static Process CreateProcess(string command, string arguments)
	{
		return new Process
		{
			StartInfo = 
			{
				FileName = command,
				Arguments = arguments,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			}
		};
	}

	private static void HandleOutputData(string data, CmdResult result, int processId, string type)
	{
		if (!string.IsNullOrWhiteSpace(data))
		{
			string text = data.Trim();
			Logger.Info($"{processId} {type}: {text}");
			if (type == "OUT")
			{
				result.StdOut = result.StdOut + text + "\n";
			}
			else
			{
				result.StdErr = result.StdErr + text + "\n";
			}
		}
	}

	private static void ValidateExitCode(int exitCode, bool throwOnNonZero)
	{
		if (throwOnNonZero && exitCode != 0 && ShouldIgnoreErrors())
		{
			throw new ApplicationException($"Command returned exit code {exitCode}");
		}
	}

	private static bool ShouldIgnoreErrors()
	{
		using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
		return (int)(registryKey?.GetValue("IgnoreError", 0) ?? ((object)0)) == 0;
	}

	private static void SetProperty(object obj, string propertyName, object value)
	{
		PropertyInfo property = obj.GetType().GetProperty(propertyName);
		if (property != null && property.CanWrite)
		{
			property.SetValue(obj, value, null);
		}
		else
		{
			Logger.Warning("Property " + propertyName + " not found or not writable");
		}
	}

	private bool HandleConsoleControl(CtrlType ctrlType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if ((int)ctrlType == 0)
		{
			_stateMachine.RequestTermination();
			return true;
		}
		return false;
	}

	private void StartInternal()
	{
		TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
		_stateMachine.RunningCallback = ApplyNetworkRedirects;
		_stateMachine.Start((bool success) =>
		{
			Logger.Info("State machine has started -> {0}", new object[1] { success });
			completionSource.SetResult(success);
		});
		if (!completionSource.Task.Result)
		{
			throw new ApplicationException("Cannot start state machine");
		}
	}

	private void StopInternal()
	{
		_stateMachine.RequestTermination();
		_stateMachine.WaitForTermination();
	}

	private void ApplyNetworkRedirects()
	{
		string[] networkInboundRules = RegistryManager.Instance.DefaultGuest.NetworkInboundRules;
		if (networkInboundRules == null)
		{
			Logger.Error("Cannot get network inbound rules");
			return;
		}
		Logger.Info("Applying network rules:");
		string[] array = networkInboundRules;
		foreach (string rule in array)
		{
			ApplyNetworkRule(rule);
		}
	}

	private bool ApplyNetworkRule(string rule)
	{
		string[] array = rule.Split(new char[1] { ':' });
		if (array.Length != 3)
		{
			Logger.Error("Cannot parse rule: " + rule);
			return false;
		}
		if (!int.TryParse(array[1], out var result) || !int.TryParse(array[2], out var result2))
		{
			Logger.Error("Invalid port numbers in rule: " + rule);
			return false;
		}
		bool isTcp = array[0] == "tcp";
		int? num = FindAvailablePort(result2, result2 + 10);
		if (num.HasValue)
		{
			VBoxBridgeService.Instance.AddNetworkRedirect(isTcp, result, num.Value);
			UpdateNetworkChangePortRedirectInfo(result, num.Value);
			return true;
		}
		Logger.Error("No available port found for rule: " + rule);
		return false;
	}

	private int? FindAvailablePort(int startPort, int endPort)
	{
		HashSet<int> usedPorts = GetUsedPorts();
		for (int i = startPort; i <= endPort; i++)
		{
			if (!usedPorts.Contains(i))
			{
				return i;
			}
		}
		return null;
	}

	private HashSet<int> GetUsedPorts()
	{
		return new HashSet<int>(from endpoint in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
			select endpoint.Port);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			OnStop();
			_hdQuitEvent?.Dispose();
			_disposed = true;
		}
	}
}
