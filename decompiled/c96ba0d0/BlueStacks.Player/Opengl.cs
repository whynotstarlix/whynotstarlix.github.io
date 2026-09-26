using System;
using System.Text;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class Opengl
{
	public delegate void GlReadyHandler();

	public delegate void GlInitFailedHandler();

	internal static GlWindowAction glWindowAction = GlWindowAction.None;

	internal static bool userInteracted = false;

	private static int mGLMode = int.MinValue;

	private static bool initialized = false;

	private static EventWaitHandle glReadyEvent;

	private const int GL_MODE_SYS_SOFT = 0;

	private const int GL_MODE_SYS_PGA = 1;

	private const int GL_MODE_SYS_GGA = 2;

	private static object syncRoot = new object();

	private static IGraphics mGraphics = null;

	public static int GlMode
	{
		get
		{
			if (mGLMode == int.MinValue)
			{
				mGLMode = RegistryManager.Instance.DefaultGuest.GlMode;
			}
			return mGLMode;
		}
		set
		{
			mGLMode = value;
		}
	}

	public static IGraphics Graphics
	{
		get
		{
			if (mGraphics == null)
			{
				lock (syncRoot)
				{
					if (mGraphics == null)
					{
						if (GlMode == 2)
						{
							mGraphics = new SysGGA();
						}
						else if (GlMode == 1)
						{
							mGraphics = new SysPGA();
						}
					}
				}
			}
			return mGraphics;
		}
		set
		{
			mGraphics = value;
		}
	}

	public static bool Init(string vmName, IntPtr h, int x, int y, int width, int height, GlReadyHandler glReadyHandler, GlInitFailedHandler glInitFailedHandler)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected I4, but got Unknown
		if (GlMode == 0)
		{
			glReadyHandler();
			SignalGlReady(vmName);
			return true;
		}
		Graphics.PgaLoggerInit(Logger.GetHdLoggerCallback());
		EventWaitHandle eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.ManualReset);
		Graphics.PgaServerInit(h, 0, 0, width, height, eventWaitHandle.SafeWaitHandle, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].GlRenderMode, RegistryManager.Instance.CurrentEngine);
		eventWaitHandle.WaitOne();
		initialized = true;
		if (!GetPgaServerInitStatus())
		{
			glInitFailedHandler();
			return false;
		}
		Graphics.PgaSetAstcConfig((int)RegistryManager.Instance.Guest[vmName].ASTCOption);
		Thread thread = new Thread(() =>
		{
			try
			{
				int num = Graphics.PgaIsHwAstcSupported();
				RegistryManager.Instance.Guest[vmName].IsHardwareAstcSupported = num == 1;
			}
			catch (Exception ex)
			{
				Logger.Error("error while astc check.." + ex.ToString());
			}
			try
			{
				int num2 = Graphics.PgaIsGLES3();
				RegistryManager.Instance.GLES3 = num2 == 1;
			}
			catch (Exception ex2)
			{
				Logger.Error("error while gl3 check.." + ex2.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
		glReadyHandler();
		SignalGlReady(vmName);
		if (RegistryManager.Instance.Guest[vmName].ShowFPS == 1)
		{
			ShowFPS();
		}
		return true;
	}

	public static void SetAstcOption(int astcConfig = 1)
	{
		Graphics.PgaSetAstcConfig(astcConfig);
	}

	public static void ShowFPS(int isShowFPS = 1)
	{
		Graphics.PgaShowFps(isShowFPS);
	}

	public static void ToggleFarmMode(bool enable)
	{
		Graphics.ToggleFarmMode(enable);
	}

	public static void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc)
	{
		Graphics.ImgdUpdateScreenPoint(ref xPos, ref yPos, ref crc);
	}

	private static bool ToGenerateId()
	{
		return RegistryManager.Instance.DefaultGuest.UpdatedVersion != 0;
	}

	private static bool GetPgaServerInitStatus()
	{
		StringBuilder stringBuilder = new StringBuilder(512);
		StringBuilder stringBuilder2 = new StringBuilder(512);
		StringBuilder stringBuilder3 = new StringBuilder(512);
		int num = -1;
		try
		{
			num = Graphics.GetPgaServerInitStatus(stringBuilder, stringBuilder2, stringBuilder3);
		}
		catch (AccessViolationException ex)
		{
			Logger.Info("Error Occured" + ex.ToString());
		}
		if (num != 0)
		{
			return false;
		}
		Profile.GlVendor = stringBuilder.ToString();
		Profile.GlRenderer = stringBuilder2.ToString();
		Profile.GlVersion = stringBuilder3.ToString();
		RegistryManager.Instance.AvailableGPUDetails = Profile.GlRenderer ?? "";
		return true;
	}

	internal static IntPtr GetSubWindow()
	{
		if (!initialized)
		{
			return IntPtr.Zero;
		}
		return Graphics.PgaServerGetSubwindow();
	}

	public static bool ShowSubWindow()
	{
		IntPtr subWindow = GetSubWindow();
		if (subWindow == IntPtr.Zero)
		{
			return false;
		}
		InteropWindow.ShowWindow(subWindow, 8);
		return true;
	}

	public static bool HideSubWindow()
	{
		IntPtr subWindow = GetSubWindow();
		if (subWindow == IntPtr.Zero)
		{
			return false;
		}
		InteropWindow.ShowWindow(subWindow, 0);
		return true;
	}

	public static bool IsSubWindowVisible()
	{
		IntPtr subWindow = GetSubWindow();
		if (!(subWindow == IntPtr.Zero))
		{
			return InteropWindow.IsWindowVisible(subWindow);
		}
		return false;
	}

	public static bool ResizeSubWindow(int x, int y, int cx, int cy)
	{
		InteropWindow.SetWindowPos(GetSubWindow(), IntPtr.Zero, x, y, cx, cy, 16468u);
		return true;
	}

	public static void HandleOrientation(float hscale, float vscale, int orientation)
	{
		Graphics.PgaServerHandleOrientation(hscale, vscale, orientation);
	}

	public static void HandleCommand(int scancode)
	{
		Graphics.PgaServerHandleCommand(scancode);
	}

	public static void HandleAppActivity(string package, string activity)
	{
		Graphics.PgaServerHandleAppActivity(package, activity);
	}

	public static void StopZygote(string vmName)
	{
		int num = 0;
		while (num < 3)
		{
			num++;
			Monitor monitor = null;
			try
			{
				uint id = MonitorLocator.Lookup(vmName);
				monitor = Manager.Open().Attach(id, verbose: true);
			}
			catch (Exception)
			{
				Thread.Sleep(500);
				continue;
			}
			try
			{
				monitor.SendControl(Monitor.BstInputControlType.BST_INPUT_CONTROL_TYPE_STOP);
			}
			catch (Exception)
			{
				Thread.Sleep(500);
				continue;
			}
			monitor?.Close();
			break;
		}
	}

	private static void SetId(string vmName)
	{
		bool flag = true;
		string text = Id.GenerateID();
		do
		{
			Thread.Sleep(50);
			flag = false;
			try
			{
				VMCommand vMCommand = new VMCommand();
				vMCommand.Attach(vmName);
				if (vMCommand.Run(new string[2] { "iSetId", text }) != 0)
				{
					flag = true;
				}
			}
			catch
			{
				flag = true;
			}
		}
		while (flag);
	}

	public static void StartZygote(string vmName)
	{
		Monitor monitor;
		while (true)
		{
			uint id = MonitorLocator.Lookup(vmName);
			monitor = null;
			Manager manager = Manager.Open();
			try
			{
				monitor = manager.Attach(id, verbose: true);
			}
			catch (Exception)
			{
				Thread.Sleep(500);
				continue;
			}
			try
			{
				monitor.SendControl(Monitor.BstInputControlType.BST_INPUT_CONTROL_TYPE_START);
			}
			catch (Exception)
			{
				Thread.Sleep(500);
				continue;
			}
			break;
		}
		monitor?.Close();
	}

	private static void SignalGlReady(string vmName)
	{
		string name = $"Global\\BlueStacks_Frontend_Gl_Ready_{vmName}";
		if (glReadyEvent == null)
		{
			glReadyEvent = new EventWaitHandle(initialState: false, EventResetMode.ManualReset, name);
		}
		glReadyEvent.Set();
	}
}
