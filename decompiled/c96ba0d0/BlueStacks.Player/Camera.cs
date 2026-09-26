using System;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class Camera
{
	public delegate void getFrameCB(IntPtr ip, int width, int height, int stride);

	public IntPtr pFrame = IntPtr.Zero;

	protected Thread previewThread;

	private volatile bool m_bStop;

	private CaptureGraph VidCapture;

	private static getFrameCB s_sendFrame;

	private int m_Unit;

	private int m_Width;

	private int m_Height;

	private int m_Framerate;

	private int m_Quality;

	private SupportedColorFormat m_color;

	public bool registerFrameCB(getFrameCB cb)
	{
		if (cb == null)
		{
			return false;
		}
		s_sendFrame = cb.Invoke;
		return true;
	}

	public Camera(int unit, int width, int height, int framerate, int quality, SupportedColorFormat color)
	{
		m_bStop = true;
		m_Unit = unit;
		m_Width = width;
		m_Height = height;
		m_Framerate = framerate;
		m_Quality = quality;
		m_color = color;
		VidCapture = new CaptureGraph(m_Unit, m_Width, m_Height, m_Framerate, m_color);
	}

	public void StartCamera()
	{
		previewThread = new Thread(Run);
		previewThread.Start();
	}

	public void StopCamera()
	{
		if (previewThread != null)
		{
			m_bStop = true;
			previewThread.Join();
			if (VidCapture != null)
			{
				VidCapture.Dispose();
				VidCapture = null;
			}
			if (s_sendFrame != null)
			{
				s_sendFrame = null;
			}
		}
		previewThread = null;
	}

	protected void Run()
	{
		m_bStop = false;
		try
		{
			VidCapture.Run();
			do
			{
				IL_0015:
				try
				{
					pFrame = IntPtr.Zero;
					pFrame = VidCapture.getSignleFrame();
					if (s_sendFrame != null && pFrame != IntPtr.Zero)
					{
						s_sendFrame(pFrame, VidCapture.Width, VidCapture.Height, VidCapture.Stride);
					}
					if (m_bStop)
					{
						goto IL_00a5;
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in send frame callback. Err : {0}", new object[1] { ex.ToString() });
					throw;
				}
				goto IL_0015;
				IL_00a5:
				VidCapture.Pause();
			}
			while (!m_bStop);
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in Graph Run. Err : {0}", new object[1] { ex2.ToString() });
		}
		finally
		{
			if (VidCapture != null)
			{
				VidCapture.Dispose();
				VidCapture = null;
			}
		}
	}
}
