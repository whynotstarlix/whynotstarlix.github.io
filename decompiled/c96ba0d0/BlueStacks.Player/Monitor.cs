using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class Monitor
{
	public delegate void LoggerCallback(string msg);

	public delegate void ExitHandler();

	private delegate void ReadCallback();

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TouchPoint(int dummy = 0)
	{
		public int PosX = 65535;

		public int PosY = 65535;
	}

	public enum BstInputControlType
	{
		BST_INPUT_CONTROL_TYPE_NONE,
		BST_INPUT_CONTROL_TYPE_SHUTDOWN,
		BST_INPUT_CONTROL_TYPE_STOP,
		BST_INPUT_CONTROL_TYPE_START
	}

	private static LoggerCallback sLoggerCallback;

	private static LoggerCallback camLoggerCallback;

	private uint mId;

	private IntPtr handle;

	private uint id;

	public Monitor(IntPtr handle, uint id, ExitHandler exitHandler)
	{
		this.handle = handle;
		this.id = id;
		Thread thread = new Thread(() =>
		{
			while (ProcessUtils.IsProcessAlive(Convert.ToInt32(id)))
			{
				Thread.Sleep(1000);
			}
			exitHandler();
		});
		thread.IsBackground = true;
		thread.Start();
	}

	static Monitor()
	{
		sLoggerCallback = (string msg) =>
		{
			Logger.Info("Monitor: " + msg);
		};
		HDPlusModule.MonitorSetLogger(sLoggerCallback);
		camLoggerCallback = (string msg) =>
		{
			Logger.Info("Camera: " + msg);
		};
		HDPlusModule.CameraSetLogger(camLoggerCallback);
	}

	public Monitor(uint id, bool verbose)
	{
		mId = id;
	}

	public Monitor(uint id)
	{
		mId = id;
	}

	public void Close()
	{
	}

	public Video VideoAttach(bool verbose)
	{
		IntPtr zero = IntPtr.Zero;
		zero = HDPlusModule.MonitorVideoAttach(mId, verbose);
		if (zero == IntPtr.Zero)
		{
			CommonError.ThrowLastWin32Error($"FATAL ERROR: Cannot attach to monitor video: {Marshal.GetLastWin32Error()}");
		}
		Video video = new Video(zero);
		try
		{
			video.CheckMagic();
		}
		catch (Exception)
		{
			HDPlusModule.MonitorVideoDetach(zero);
			throw;
		}
		Logger.Info("Video Attached");
		return video;
	}

	public void SendScanCode(byte code)
	{
		if (!HDPlusModule.MonitorSendScanCode(code))
		{
			throw new IOException("Cannot send keyboard scan code");
		}
	}

	public void SendLocation(GPSManager.GpsLocation location)
	{
		if (!HDPlusModule.MonitorSendLocation(location))
		{
			CommonError.ThrowLastWin32Error("Cannot send GPS location update");
		}
	}

	public void SendMouseState(uint x, uint y, uint mask)
	{
		if (!HDPlusModule.MonitorSendMouseState(x, y, mask))
		{
			throw new IOException("Cannot send mouse state");
		}
	}

	public void SendControl(BstInputControlType type)
	{
		if (!HDPlusModule.MonitorSendControl(type))
		{
			throw new IOException("Cannot send control state state");
		}
	}

	public void SendTouchState(TouchPoint[] points)
	{
		if (points == null)
		{
			points = new TouchPoint[0];
		}
		if (!HDPlusModule.MonitorSendTouchState(points, points.Length))
		{
			throw new IOException("Cannot send touch state");
		}
	}

	internal void SendAndroidString(string formattedString)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(formattedString);
			HDPlusModule.MonitorSendImeMsg(bytes, bytes.Length);
		}
		catch
		{
		}
	}
}
