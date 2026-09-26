using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Player;

public class VmMonitor
{
	public delegate void SendMessage(IntPtr msg);

	public delegate void ReceiverCallback(IntPtr msg);

	private SafeFileHandle mHandle;

	private IntPtr mListener;

	private UdpClient mUdpClient;

	private ReceiverCallback mReceiverCallback;

	private Thread mReceiverThread;

	private EventWaitHandle mReceiverWakeup;

	public static int lPort;

	public static string data;

	private VmMonitor(UdpClient c)
	{
		Logger.Warning("{0} Monitor constructor initing UDP socket", new object[1] { MethodBase.GetCurrentMethod().Name });
		mUdpClient = c;
		mListener = mUdpClient.Client.Handle;
	}

	private VmMonitor(SafeFileHandle handle)
	{
		mHandle = handle;
	}

	public static VmMonitor Connect(string vmName, uint cls)
	{
		lPort = 2921;
		int num = lPort + 40;
		UdpClient c = null;
		for (int i = lPort; i <= num; i++)
		{
			try
			{
				c = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), i));
				lPort = i;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while setting up UDPServer on port. {0}, Err : ", new object[2]
				{
					i,
					ex.ToString()
				});
				if (i == num)
				{
					throw;
				}
				continue;
			}
			break;
		}
		RegistryManager.Instance.DefaultGuest.HostSensorPort = lPort;
		Logger.Warning("Host sensor port is {0}", new object[1] { lPort });
		return new VmMonitor(c);
	}

	public void Close()
	{
		mUdpClient.Close();
	}

	public void Send(IntPtr msg)
	{
		if (!HDPlusModule.SensorSendMsg(msg))
		{
			CommonError.ThrowLastWin32Error("Cannot send message to guest");
		}
	}

	public void SendShutdown(IntPtr msg)
	{
		if (!HDPlusModule.SensorSendMsg(msg))
		{
			CommonError.ThrowLastWin32Error("Cannot send message to guest");
		}
	}

	public void StartReceiver(ReceiverCallback callback)
	{
		mReceiverCallback = callback;
		mReceiverWakeup = new ManualResetEvent(initialState: false);
		mReceiverThread = new Thread(() =>
		{
			try
			{
				if (!HDPlusModule.SensorRecvMsg(mReceiverCallback))
				{
					CommonError.ThrowLastWin32Error("Cannot receive monitor message");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception, receiver thread died. Err : " + ex.ToString());
			}
		})
		{
			IsBackground = true
		};
		mReceiverThread.Start();
	}

	public void StopReceiver()
	{
		Logger.Warning("{0} NOP for VBox", new object[1] { MethodBase.GetCurrentMethod().Name });
	}
}
