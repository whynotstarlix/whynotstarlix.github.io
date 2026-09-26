using System;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Player;

public interface IGraphics
{
	void PgaLoggerInit(HdLoggerCallback cb);

	int PgaUtilsIsHotAttach();

	int PgaServerInit(IntPtr h, int x, int y, int width, int height, SafeWaitHandle evt, int glRenderMode, string engine);

	void PgaServerHandleCommand(int scancode);

	IntPtr PgaServerGetSubwindow();

	IntPtr PgaServerHandleOrientation(float hscale, float vscale, int orientation);

	IntPtr PgaServerHandleAppActivity(string package, string activity);

	int GetPgaServerInitStatus(StringBuilder glVendor, StringBuilder glRenderer, StringBuilder glVersion);

	int PgaIsGLES3();

	void PgaSetFps(int fps);

	void PgaShowFps(int enable);

	void ToggleFarmMode(bool enable);

	void ImgdUpdateScreenPoint(ref float xPos, ref float yPos, ref uint crc);

	void PgaSetAstcConfig(int config);

	int PgaIsHwAstcSupported();
}
