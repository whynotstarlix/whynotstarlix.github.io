using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player;

[ComImport]
[SuppressUnmanagedCodeSecurity]
[Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ISampleGrabber
{
	int SetOneShot([In][MarshalAs(UnmanagedType.Bool)] bool OneShot);

	int SetMediaType([In][MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

	int GetConnectedMediaType([Out][MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

	int SetBufferSamples([In][MarshalAs(UnmanagedType.Bool)] bool BufferThem);

	int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);

	int GetCurrentSample(out IMediaSample ppSample);

	int SetCallback(ISampleGrabberCB pCallback, int WhichMethodToCallback);
}
