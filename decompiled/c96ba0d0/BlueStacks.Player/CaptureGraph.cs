using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class CaptureGraph : ISampleGrabberCB, IDisposable
{
	private int m_Unit;

	private int m_Width;

	private int m_Height;

	private int m_FrameRate;

	private int m_Stride;

	private int m_DroppedFrame;

	private IntPtr m_Buffer = IntPtr.Zero;

	private ManualResetEvent m_Evt;

	private bool m_bGraphRunning;

	private volatile bool m_bGrabFrame;

	private SupportedColorFormat m_color;

	private IFilterGraph2 m_FilterGraph;

	private IMediaControl m_mediaCtrl;

	public int Width => m_Width;

	public int Height => m_Height;

	public int Stride => m_Stride;

	[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
	private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

	public CaptureGraph(int unit, int width, int height, int framerate, SupportedColorFormat color)
	{
		m_Unit = unit;
		m_Width = width;
		m_Height = height;
		m_FrameRate = framerate;
		m_DroppedFrame = 0;
		m_color = color;
		m_Evt = new ManualResetEvent(initialState: false);
		m_bGraphRunning = false;
		Logger.Info("Building graph");
		try
		{
			BuildGraph();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in build graph. Err : {0}", new object[1] { ex.ToString() });
			Dispose();
			throw;
		}
	}

	~CaptureGraph()
	{
		if (m_Buffer != IntPtr.Zero)
		{
			Marshal.FreeCoTaskMem(m_Buffer);
			m_Buffer = IntPtr.Zero;
		}
		Dispose();
	}

	public void BuildGraph()
	{
		ICaptureGraphBuilder2 captureGraphBuilder = null;
		IBaseFilter ppFilter = null;
		ISampleGrabber sampleGrabber = null;
		List<DeviceEnumerator> list = null;
		DeviceEnumerator deviceEnumerator = null;
		try
		{
			Logger.Info("Creating List of devices");
			list = DeviceEnumerator.ListDevices(Guids.VideoInputDeviceCategory);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in finding Video device. Err : {0}", new object[1] { ex.ToString() });
		}
		if (list == null || list.Count == 0)
		{
			Logger.Info("CAMERA: Could not find a camera device!");
			return;
		}
		try
		{
			Logger.Info("found {0} Camera, Opening {1}", new object[2] { list.Count, m_Unit });
			deviceEnumerator = ((m_Unit >= list.Count) ? list[0] : list[m_Unit]);
			m_FilterGraph = (IFilterGraph2)new FilterGraph();
			m_mediaCtrl = m_FilterGraph as IMediaControl;
			captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
			sampleGrabber = (ISampleGrabber)new SampleGrabber();
			ErrorHandler errorHandler = captureGraphBuilder.SetFiltergraph(m_FilterGraph);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("SetFiltergraph failed with {0:X}..", new object[1] { errorHandler.GetError() });
			}
			errorHandler = m_FilterGraph.AddSourceFilterForMoniker(deviceEnumerator.Moniker, null, "Video input", out ppFilter);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("AddSourceFilterForMoniker failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			AMMediaType pmt = new AMMediaType
			{
				majorType = Guids.MediaTypeVideo
			};
			if (m_color == SupportedColorFormat.YUV2)
			{
				pmt.subType = Guids.MediaSubtypeYUY2;
			}
			else
			{
				if (m_color != SupportedColorFormat.RGB24)
				{
					throw new Exception("Unsupported color format");
				}
				pmt.subType = Guids.MediaSubtypeRGB24;
			}
			pmt.formatType = Guids.FormatTypesVideoInfo;
			errorHandler = sampleGrabber.SetMediaType(pmt);
			FreeAMMedia(pmt);
			errorHandler = sampleGrabber.SetCallback(this, 1);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("Grabber setcallback failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			IBaseFilter baseFilter = (IBaseFilter)sampleGrabber;
			errorHandler = m_FilterGraph.AddFilter(baseFilter, "FrameGrabber");
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("AddFilter failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			errorHandler = captureGraphBuilder.FindInterface(Guids.PinCategoryCapture, Guids.MediaTypeVideo, ppFilter, typeof(IAMStreamConfig).GUID, out var ppint);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("FindInterface failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			IAMStreamConfig obj = (ppint as IAMStreamConfig) ?? throw new Exception("Stream config Error");
			errorHandler = obj.GetFormat(out pmt);
			VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
			Marshal.PtrToStructure(pmt.pbFormat, (object)videoInfoHeader);
			videoInfoHeader.AvgTimePerFrame = 10000000 / m_FrameRate;
			videoInfoHeader.BmiHeader.Width = m_Width;
			videoInfoHeader.BmiHeader.Height = m_Height;
			Marshal.StructureToPtr((object)videoInfoHeader, pmt.pbFormat, false);
			errorHandler = obj.SetFormat(pmt);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("conf.setformat failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			FreeAMMedia(pmt);
			errorHandler = captureGraphBuilder.RenderStream(Guids.PinCategoryCapture, Guids.MediaTypeVideo, ppFilter, null, baseFilter);
			if (errorHandler.GetError() != 0)
			{
				Logger.Error("RenderStream failed with {0:X}", new object[1] { errorHandler.GetError() });
			}
			pmt = new AMMediaType();
			errorHandler = sampleGrabber.GetConnectedMediaType(pmt);
			if (pmt.formatType != Guids.FormatTypesVideoInfo)
			{
				throw new ColorFormatNotSupported("Not able to connect to Video Media");
			}
			if (pmt.pbFormat == IntPtr.Zero)
			{
				throw new Exception("Format Array is null");
			}
			videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(pmt.pbFormat, typeof(VideoInfoHeader));
			m_Width = videoInfoHeader.BmiHeader.Width;
			m_Height = videoInfoHeader.BmiHeader.Height;
			m_Stride = m_Width * (videoInfoHeader.BmiHeader.BitCount / 8);
			if (m_Buffer == IntPtr.Zero)
			{
				m_Buffer = Marshal.AllocCoTaskMem(m_Stride * m_Height);
			}
			FreeAMMedia(pmt);
		}
		catch
		{
			throw;
		}
		finally
		{
			if (ppFilter != null)
			{
				Marshal.ReleaseComObject(ppFilter);
				ppFilter = null;
			}
			if (sampleGrabber != null)
			{
				Marshal.ReleaseComObject(sampleGrabber);
				sampleGrabber = null;
			}
			if (captureGraphBuilder != null)
			{
				Marshal.ReleaseComObject(captureGraphBuilder);
				captureGraphBuilder = null;
			}
		}
	}

	public void Dispose()
	{
		TearDownCom();
		if (m_Evt != null)
		{
			m_Evt.Close();
			m_Evt = null;
		}
	}

	private void FreeAMMedia(AMMediaType m)
	{
		if (m != null)
		{
			if (m.cbFormat != 0)
			{
				Marshal.FreeCoTaskMem(m.pbFormat);
				m.cbFormat = 0;
				m.pbFormat = IntPtr.Zero;
			}
			if (m.pUnk != IntPtr.Zero)
			{
				Marshal.Release(m.pUnk);
				m.pUnk = IntPtr.Zero;
			}
		}
		m = null;
	}

	int ISampleGrabberCB.SampleCB(double time, IMediaSample pSample)
	{
		Marshal.ReleaseComObject(pSample);
		return 0;
	}

	int ISampleGrabberCB.BufferCB(double time, IntPtr pBuffer, int len)
	{
		if (m_bGrabFrame)
		{
			if (len <= m_Stride * m_Height)
			{
				CopyMemory(m_Buffer, pBuffer, len);
			}
			m_bGrabFrame = false;
			m_Evt.Set();
		}
		else
		{
			m_DroppedFrame++;
		}
		return 0;
	}

	public void Run()
	{
		if (!m_bGraphRunning && m_mediaCtrl != null)
		{
			_ = (ErrorHandler)m_mediaCtrl.Run();
			m_bGraphRunning = true;
		}
	}

	public void Pause()
	{
		if (m_bGraphRunning)
		{
			_ = (ErrorHandler)m_mediaCtrl.Pause();
			m_bGraphRunning = false;
		}
	}

	public IntPtr getSignleFrame()
	{
		try
		{
			m_Evt.Reset();
			m_bGrabFrame = true;
			Run();
			if (!m_Evt.WaitOne(5000, exitContext: false))
			{
				Logger.Info("GetSingleFrame Timed out");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in getting single frame. Err : " + ex.ToString());
			Marshal.FreeCoTaskMem(m_Buffer);
			m_Buffer = IntPtr.Zero;
		}
		return m_Buffer;
	}

	private void TearDownCom()
	{
		try
		{
			if (m_mediaCtrl != null && m_bGraphRunning)
			{
				_ = (ErrorHandler)m_mediaCtrl.Stop();
				m_bGraphRunning = false;
			}
			if (m_mediaCtrl != null)
			{
				Marshal.ReleaseComObject(m_mediaCtrl);
				m_mediaCtrl = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Stop Graph. Err : {0}", new object[1] { ex.ToString() });
		}
		if (m_FilterGraph != null)
		{
			Marshal.ReleaseComObject(m_FilterGraph);
			m_FilterGraph = null;
		}
	}
}
