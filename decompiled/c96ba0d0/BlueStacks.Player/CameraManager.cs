using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class CameraManager
{
	public delegate void fpStartStopCamera(int startStop, int unit, int width, int height, int framerate);

	private fpStartStopCamera s_fpStartStopCamera;

	private static Monitor s_Monitor;

	private static IntPtr s_IoHandle = IntPtr.Zero;

	private static object s_IoHandleLock = new object();

	private IntPtr overWrite;

	private Camera camera;

	private Camera.getFrameCB cb;

	private bool bShutDown;

	private int unit;

	private int framerate = 30;

	private int width = 640;

	private int height = 480;

	private int jpegQuality = 100;

	private int keyEnableCam;

	private bool cameraStoped = true;

	private SupportedColorFormat m_color;

	private IntPtr m_buffer = IntPtr.Zero;

	private int m_StartCount;

	public static Monitor Monitor
	{
		get
		{
			return s_Monitor;
		}
		set
		{
			s_Monitor = value;
		}
	}

	[DllImport("kernel32.dll")]
	private static extern bool CloseHandle(IntPtr handle);

	private void BstStartStopCamera(int startStop, int unit, int width, int height, int framerate)
	{
		lock (s_IoHandleLock)
		{
			if (this.unit != unit && startStop == 1)
			{
				camStop();
				m_StartCount = 0;
			}
			if (this.unit == unit && startStop == 0)
			{
				m_StartCount--;
				if (m_StartCount == 0)
				{
					camStop();
				}
			}
			if (startStop == 1)
			{
				m_StartCount++;
				camStart(unit, width, height, framerate);
				this.unit = unit;
			}
		}
	}

	public void InitCamera(string[] args)
	{
		if (args.Length != 1)
		{
			throw new SystemException("InitCamera: Should have vmName as one arg");
		}
		string text = args[0];
		keyEnableCam = RegistryManager.Instance.DefaultGuest.Camera;
		if (keyEnableCam != 1)
		{
			Logger.Info("Camera is Disabled");
			return;
		}
		uint num = MonitorLocator.Lookup(text);
		lock (s_IoHandleLock)
		{
			if (s_IoHandle != IntPtr.Zero)
			{
				throw new SystemException("I/O handle is already open");
			}
			Logger.Info("Attaching to monitor ID {0}", new object[1] { num });
			s_IoHandle = HDPlusModule.CameraIoAttach(num);
			if (s_IoHandle == IntPtr.Zero)
			{
				throw new SystemException("Cannot attach for I/O", new Win32Exception(Marshal.GetLastWin32Error()));
			}
		}
		s_fpStartStopCamera = BstStartStopCamera;
		HDPlusModule.SetStartStopCamerCB(s_fpStartStopCamera);
		Logger.Info("Waiting for Camera messages...");
		Thread thread = new Thread(() =>
		{
			while (!bShutDown)
			{
				int num2 = HDPlusModule.CameraIoProcessMessages(s_IoHandle);
				if (num2 != 0)
				{
					Logger.Error("Camera: Cannot process VM messages. Error: " + num2);
					Shutdown();
				}
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public void Shutdown()
	{
		lock (s_IoHandleLock)
		{
			if (keyEnableCam == 1)
			{
				bShutDown = true;
				if (camera != null || !cameraStoped)
				{
					camera.StopCamera();
				}
				camera = null;
				if (s_IoHandle != IntPtr.Zero)
				{
					CloseHandle(s_IoHandle);
					s_IoHandle = IntPtr.Zero;
				}
			}
		}
	}

	public void getFrame(IntPtr ip, int width, int height, int stride)
	{
		if (ip == IntPtr.Zero || camera == null || s_IoHandle == IntPtr.Zero || cameraStoped)
		{
			return;
		}
		IntPtr stream = ip;
		if (m_color == SupportedColorFormat.RGB24)
		{
			if (m_buffer == IntPtr.Zero)
			{
				m_buffer = Marshal.AllocCoTaskMem(width * height * 2);
			}
			HDPlusModule.convertRGB24toYUV422(ip, width, height, m_buffer);
			stream = m_buffer;
		}
		HDPlusModule.CameraSendCaptureStream(s_IoHandle, stream, width * height * 2, width, height, stride);
	}

	public void camStart(int unit, int w, int h, int f)
	{
		if (camera != null || keyEnableCam != 1 || !cameraStoped)
		{
			return;
		}
		if (w > 0)
		{
			width = w;
		}
		if (h > 0)
		{
			height = h;
		}
		if (f > 0)
		{
			framerate = f;
		}
		Logger.Info("Starting Camera {0}. Frame width: {1}, height: {2}, framerate: {3}", new object[4] { unit, width, height, framerate });
		cameraStoped = false;
		cb = getFrame;
		for (int i = 0; i < 2; i++)
		{
			if (camera != null)
			{
				break;
			}
			try
			{
				m_color = (SupportedColorFormat)i;
				camera = new Camera(unit, width, height, framerate, jpegQuality, m_color);
			}
			catch (ColorFormatNotSupported colorFormatNotSupported)
			{
				Logger.Info("Trying with other color." + colorFormatNotSupported.ToString());
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in to initialize the camera. Err : ", new object[1] { ex.ToString() });
			}
		}
		if (camera == null)
		{
			Logger.Error("Cannot start the host camera.");
			return;
		}
		camera.registerFrameCB(cb);
		camera.StartCamera();
	}

	public void camStop()
	{
		if (camera != null && !cameraStoped)
		{
			Logger.Info("Stoping Camera.");
			cameraStoped = true;
			camera.StopCamera();
			camera = null;
			if (m_buffer != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(m_buffer);
				m_buffer = IntPtr.Zero;
			}
		}
	}

	public void resumeCamera()
	{
		lock (s_IoHandleLock)
		{
			if (keyEnableCam == 1 && cameraStoped && camera != null)
			{
				Logger.Info("Resuming Camera");
				camStart(unit, width, height, framerate);
			}
		}
	}

	public void pauseCamera()
	{
		lock (s_IoHandleLock)
		{
			if (keyEnableCam == 1 && camera != null && !cameraStoped)
			{
				Logger.Info("Pausing Camera");
				camStop();
			}
		}
	}
}
