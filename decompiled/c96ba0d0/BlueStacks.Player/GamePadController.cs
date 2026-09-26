using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class GamePadController
{
	private delegate void LoggerCallback(string msg);

	private delegate void AttachCallback(int identity, int vendor, int product);

	private delegate void DetachCallback(int identity);

	private delegate void UpdateCallback(int identity, ref GamePad gamepad);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void GamePadSetup(LoggerCallback logger, AttachCallback attach, DetachCallback detach, UpdateCallback update, IntPtr windowHandle, IntPtr vmwHandle);

	private LoggerCallback mLoggerCallback;

	private AttachCallback mAttachCallback;

	private DetachCallback mDetachCallback;

	private UpdateCallback mUpdateCallback;

	[DllImport("kernel32.dll")]
	public static extern IntPtr LoadLibrary(string dllToLoad);

	[DllImport("kernel32.dll")]
	public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

	public void Setup(IntPtr windowHandle, IntPtr vmwHandle)
	{
		Logger.Info("GamePad.Setup()");
		Logger.Debug(windowHandle + " " + Environment.StackTrace);
		mLoggerCallback = (string msg) =>
		{
			Logger.Info("GamePad: " + msg);
		};
		mAttachCallback = (int identity, int vendor, int product) =>
		{
		};
		mDetachCallback = (int identity) =>
		{
		};
		mUpdateCallback = delegate
		{
		};
		string text = "HD-Frontend-Native.dll";
		IntPtr intPtr = LoadLibrary(text);
		if (intPtr == IntPtr.Zero)
		{
			Logger.Info("Failed to {0} dll", new object[1] { text });
			return;
		}
		IntPtr procAddress = GetProcAddress(intPtr, "GamePadSetup");
		if (procAddress == IntPtr.Zero)
		{
			Logger.Info("function pointer is null");
		}
		else
		{
			((GamePadSetup)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(GamePadSetup)))(mLoggerCallback, mAttachCallback, mDetachCallback, mUpdateCallback, windowHandle, vmwHandle);
		}
	}
}
