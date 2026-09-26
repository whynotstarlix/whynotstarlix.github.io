using System;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class GpsHelper
{
	[DllImport("HD-GpsLocator-Native.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern void HdLoggerInit(HdLoggerCallback cb);

	[DllImport("HD-GpsLocator-Native.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int LaunchGpsLocator();

	internal static void Start()
	{
		try
		{
			Logger.Info("Starting Gps Locator");
			Thread thread = new Thread(StartGpsLocator);
			thread.IsBackground = true;
			thread.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Error Occured, Err: {0}", new object[1] { ex.ToString() });
		}
	}

	private static void StartGpsLocator()
	{
		Logger.Info("Inside Start GpsLocator");
		try
		{
			try
			{
				Logger.Info("Checking if Gps Enabled");
				if (RegistryManager.Instance.DefaultGuest.GpsMode == 0)
				{
					Logger.Info("GpsMode is Disabled.");
					return;
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Error Occured, Err: {ex.ToString()}");
			}
			Version version = new Version(6, 2, 9200, 0);
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= version)
			{
				try
				{
					HdLoggerInit(Logger.GetHdLoggerCallback());
					LaunchGpsLocator();
					Logger.Info("Back from Native Call");
					return;
				}
				catch (Exception ex2)
				{
					Logger.Error($"Error Occured, Err: {ex2.ToString()}");
					return;
				}
			}
			Logger.Warning("Need Windows 8 or Higher for GpsLocator to work.");
		}
		catch (Exception ex3)
		{
			Logger.Error($"Exception Occured in StartGpsLocator. Err : {ex3.ToString()}");
		}
	}
}
