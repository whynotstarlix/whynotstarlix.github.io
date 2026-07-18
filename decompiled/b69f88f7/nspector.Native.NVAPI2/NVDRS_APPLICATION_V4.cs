using System.Runtime.InteropServices;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
internal struct NVDRS_APPLICATION_V4
{
	public uint version;

	public uint isPredefined;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string appName;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string userFriendlyName;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string launcher;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string fileInFolder;

	private uint bitvector1;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
	public string commandLine;

	public bool isMetro
	{
		get
		{
			return (bitvector1 & 1) != 0;
		}
		set
		{
			if (value)
			{
				bitvector1 |= 1u;
			}
			else
			{
				bitvector1 &= 4294967294u;
			}
		}
	}

	public bool isCommandLine
	{
		get
		{
			return (bitvector1 & 2) != 0;
		}
		set
		{
			if (value)
			{
				bitvector1 |= 2u;
			}
			else
			{
				bitvector1 &= 4294967293u;
			}
		}
	}
}
