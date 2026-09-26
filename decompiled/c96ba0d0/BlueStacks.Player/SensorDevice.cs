using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class SensorDevice
{
	public enum Type
	{
		Accelerometer = 1
	}

	private class State
	{
		public bool Enabled;

		public uint Period;

		public bool HasPhysical;

		public int ControllerCount;
	}

	private class AccelerometerState : State
	{
		public float RawX;

		public float RawY;

		public float RawZ;

		public float SmoothedX;

		public float SmoothedY;

		public float SmoothedZ;

		public volatile bool HasNewData;
	}

	private delegate void LoggerCallback(string msg);

	private delegate void AccelerometerCallback(float x, float y, float z);

	private delegate void EnableHandler(Type sensor, bool enable);

	private delegate void SetDelayHandler(Type sensor, uint msec);

	private const string NATIVE_DLL = "HD-Sensor-Native.dll";

	private VmMonitor mMonitor;

	private LoggerCallback mLogger;

	private volatile bool mRunning;

	private Dictionary<Type, State> mStateMap;

	private Thread mAccelerometerThread;

	private AccelerometerCallback mAccelerometerCallback;

	private EnableHandler mEnableHandler;

	private SetDelayHandler mSetDelayHandler;

	private SerialWorkQueue mSerialQueue;

	private static SensorDevice mSensorDevice;

	private const float Sensitivity = 2.8f;

	private const float Deadzone = 0.003f;

	private const float AccelExponent = 1.15f;

	private const float MaxClamp = 100f;

	private const float MinClamp = -100f;

	private const float MinDeltaToSend = 0.0001f;

	internal static SensorDevice Instance
	{
		get
		{
			if (mSensorDevice == null)
			{
				mSensorDevice = new SensorDevice();
			}
			return mSensorDevice;
		}
	}

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void LoggerSetCallback(LoggerCallback cb);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void SensorMsgInit(EnableHandler enableHandler, SetDelayHandler setDelayHandler);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void SensorMsgHandleMessage(IntPtr msg);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void SensorMsgSendReattach(Type sensor, VmMonitor.SendMessage handler);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void SensorMsgSendAccelerometerEvent(float x, float y, float z, VmMonitor.SendMessage handler);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void SensorMsgSendStopReceiver(VmMonitor.SendMessage handler);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern bool HostInit();

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void HostSetOrientation(int orientation);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern bool HostSetupAccelerometer(AccelerometerCallback callback);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void HostEnableSensor(Type sensor, bool enable);

	[DllImport("HD-Sensor-Native.dll")]
	private static extern void HostSetSensorPeriod(Type sensor, uint msec);

	public SensorDevice()
	{
		mStateMap = new Dictionary<Type, State> { [Type.Accelerometer] = new AccelerometerState() };
	}

	public void StartThreads()
	{
	}

	public void Start(string vmName)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected Obj, but got Unknown
		mRunning = true;
		mLogger = (string msg) =>
		{
		};
		LoggerSetCallback(mLogger);
		mSerialQueue = new SerialWorkQueue();
		mSerialQueue.Start();
		SetupHostSensors();
		mEnableHandler = EnableHandlerImpl;
		mSetDelayHandler = SetDelayHandlerImpl;
		SensorMsgInit(mEnableHandler, mSetDelayHandler);
		mMonitor = VmMonitor.Connect(vmName, 0u);
		mMonitor.StartReceiver(SensorMsgHandleMessage);
		SensorMsgSendReattach(Type.Accelerometer, SendMessage);
	}

	public void Stop()
	{
		mRunning = false;
		try
		{
			SensorMsgSendStopReceiver(SendShutdownMessage);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending Stop receiver msg. Err : " + ex.ToString());
		}
		if (mMonitor != null)
		{
			mMonitor.StopReceiver();
			mMonitor.Close();
			mMonitor = null;
		}
	}

	private State LookupState(Type sensor)
	{
		if (mStateMap != null && mStateMap.ContainsKey(sensor))
		{
			return mStateMap[sensor];
		}
		return null;
	}

	private void SetupHostSensors()
	{
	}

	public void ControllerAttach(Type sensor)
	{
	}

	public void ControllerDetach(Type sensor)
	{
	}

	private void ControllerAttachDetach(Type sensor, bool attach)
	{
	}

	public void SetAccelerometerVector(float origX, float origY, float origZ)
	{
	}

	private void AccelerometerThreadEntry()
	{
	}

	private void SendAccelerometerVector(float x, float y, float z)
	{
	}

	private void EnableHandlerImpl(Type sensor, bool enable)
	{
		State state = LookupState(sensor);
		if (state != null)
		{
			state.Enabled = enable;
		}
	}

	private void SetDelayHandlerImpl(Type sensor, uint msec)
	{
		State state = LookupState(sensor);
		if (state != null)
		{
			state.Period = msec;
		}
	}

	private void SendMessage(IntPtr msg)
	{
		if (mMonitor != null)
		{
			mMonitor.Send(msg);
		}
	}

	private void SendShutdownMessage(IntPtr msg)
	{
		if (mMonitor != null)
		{
			mMonitor.SendShutdown(msg);
		}
	}

	private void UpdateOrientation(object obj, EventArgs evt)
	{
	}

	private static float ApplyDeadzoneAndCurve(float v)
	{
		if (Math.Abs(v) <= 0.003f)
		{
			return 0f;
		}
		float num = ((v >= 0f) ? 1f : (-1f));
		double num2 = Math.Pow(Math.Abs(v), 1.149999976158142);
		return num * (float)num2;
	}

	private static float Clamp(float v, float min, float max)
	{
		if (v < min)
		{
			return min;
		}
		if (v > max)
		{
			return max;
		}
		return v;
	}
}
