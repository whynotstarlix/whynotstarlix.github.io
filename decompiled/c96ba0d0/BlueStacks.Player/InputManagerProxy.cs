using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class InputManagerProxy : IDisposable
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void HyperVLog(Monitor.LoggerCallback logger);

	private int mId;

	private const int CONTROL_TYPE_SHUTDOWN = 1;

	private static Monitor.LoggerCallback mLoggerCallback;

	[DllImport("HD-Plus-Service-Native.dll", SetLastError = true)]
	private static extern bool MonitorSendControl(int control);

	[DllImport("HD-Plus-Service-Native.dll", SetLastError = true)]
	internal static extern int IsHyperVEnabled();

	[DllImport("kernel32.dll")]
	public static extern IntPtr LoadLibrary(string dllToLoad);

	[DllImport("kernel32.dll")]
	public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

	public InputManagerProxy(int id)
	{
		mId = id;
	}

	internal static void SetUp()
	{
		mLoggerCallback = (string msg) =>
		{
			Logger.Info("HyperV: " + msg);
		};
		string text = "HD-Plus-Service-Native.dll";
		IntPtr intPtr = LoadLibrary(text);
		if (intPtr == IntPtr.Zero)
		{
			Logger.Info("Failed to {0} dll", new object[1] { text });
			return;
		}
		IntPtr procAddress = GetProcAddress(intPtr, "HyperVLog");
		if (procAddress == IntPtr.Zero)
		{
			Logger.Info("function pointer is null");
		}
		else
		{
			((HyperVLog)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(HyperVLog)))(mLoggerCallback);
		}
	}

	public void SendControlShutdown()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		if (!MonitorSendControl(1))
		{
			ThrowLastWin32Error("Cannot send shutdown control");
		}
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	private static void ThrowLastWin32Error(string msg)
	{
		throw new SystemException(msg, new Win32Exception(Marshal.GetLastWin32Error()));
	}
}
