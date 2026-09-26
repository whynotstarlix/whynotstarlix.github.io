using System;
using System.Runtime.InteropServices;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Player;

public class SysPGA : IGraphics
{
	private const string OpenGL_Native_DLL = "HD-OpenGl-Native.dll";

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void PgaSetAstcConfig(int config);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern int PgaIsHwAstcSupported();

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void PgaLoggerInit(HdLoggerCallback cb);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern int PgaUtilsIsHotAttach();

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern int PgaServerInit(IntPtr h, int x, int y, int width, int height, SafeWaitHandle evt, int glRenderMode, string engine);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void PgaServerHandleCommand(int scancode);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern IntPtr PgaServerGetSubwindow();

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern IntPtr PgaServerHandleOrientation(float hscale, float vscale, int orientation);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern IntPtr PgaServerHandleAppActivity(string package, string activity);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern int GetPgaServerInitStatus(StringBuilder glVendor, StringBuilder glRenderer, StringBuilder glVersion);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern int PgaIsGLES3();

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void PgaSetFps(int fps);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void PgaShowFps(int enable);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void ToggleFarmMode(bool enable);

	[DllImport("HD-OpenGl-Native.dll")]
	private static extern void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc);

	void IGraphics.PgaSetAstcConfig(int config)
	{
		PgaSetAstcConfig(config);
	}

	int IGraphics.PgaIsHwAstcSupported()
	{
		return PgaIsHwAstcSupported();
	}

	int IGraphics.GetPgaServerInitStatus(StringBuilder glVendor, StringBuilder glRenderer, StringBuilder glVersion)
	{
		return GetPgaServerInitStatus(glVendor, glRenderer, glVersion);
	}

	void IGraphics.PgaLoggerInit(HdLoggerCallback cb)
	{
		PgaLoggerInit(cb);
	}

	IntPtr IGraphics.PgaServerGetSubwindow()
	{
		return PgaServerGetSubwindow();
	}

	IntPtr IGraphics.PgaServerHandleAppActivity(string package, string activity)
	{
		return PgaServerHandleAppActivity(package, activity);
	}

	void IGraphics.PgaServerHandleCommand(int scancode)
	{
		PgaServerHandleCommand(scancode);
	}

	IntPtr IGraphics.PgaServerHandleOrientation(float hscale, float vscale, int orientation)
	{
		return PgaServerHandleOrientation(hscale, vscale, orientation);
	}

	int IGraphics.PgaServerInit(IntPtr h, int x, int y, int width, int height, SafeWaitHandle evt, int glRenderMode, string engine)
	{
		return PgaServerInit(h, x, y, width, height, evt, glRenderMode, engine);
	}

	int IGraphics.PgaUtilsIsHotAttach()
	{
		return PgaUtilsIsHotAttach();
	}

	int IGraphics.PgaIsGLES3()
	{
		return PgaIsGLES3();
	}

	void IGraphics.PgaSetFps(int fps)
	{
		PgaSetFps(fps);
	}

	void IGraphics.PgaShowFps(int enable)
	{
		PgaShowFps(enable);
	}

	void IGraphics.ToggleFarmMode(bool enable)
	{
		ToggleFarmMode(enable);
	}

	void IGraphics.ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc)
	{
		ImgdUpdateScreenPoint(ref xPos, ref yPos, ref crc);
	}
}
