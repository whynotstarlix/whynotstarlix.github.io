using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class GPSManager
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GpsLocation
	{
		public double latitude;

		public double longitude;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string country;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string city;
	}

	private const string NATIVE_DLL = "HD-Gps-Native.dll";

	private static IntPtr s_IoHandle = IntPtr.Zero;

	private static object s_IoHandleLock = new object();

	public static GpsLocation location;

	private static GPSManager sInstance = new GPSManager();

	private static Monitor sMonitor;

	[DllImport("kernel32.dll")]
	private static extern bool CloseHandle(IntPtr handle);

	[DllImport("HD-Gps-Native.dll", SetLastError = true)]
	private static extern IntPtr GpsIoAttach(uint vmId);

	[DllImport("HD-Gps-Native.dll", SetLastError = true)]
	private static extern int GpsIoProcessMessages(IntPtr ioHandle);

	public void SetMonitor(Monitor monitor)
	{
		sMonitor = monitor;
	}

	public static GPSManager Instance()
	{
		return sInstance;
	}

	public static void Init()
	{
		Logger.Debug("Waiting for Gps messages...");
		Thread thread = new Thread(() =>
		{
			while (true)
			{
				try
				{
					location.latitude = Convert.ToDouble(RegistryManager.Instance.DefaultGuest.GpsLatitude, CultureInfo.InvariantCulture);
					location.longitude = Convert.ToDouble(RegistryManager.Instance.DefaultGuest.GpsLongitude, CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString());
					Logger.Error("GPS: Exiting thread.");
					break;
				}
				Logger.Debug("Sending GPS location...");
				sMonitor.SendLocation(location);
				Thread.Sleep(60000);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void Shutdown()
	{
		lock (s_IoHandleLock)
		{
			if (s_IoHandle != IntPtr.Zero)
			{
				Logger.Debug("Shutting down gps...\n");
				CloseHandle(s_IoHandle);
				s_IoHandle = IntPtr.Zero;
			}
		}
	}
}
