using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nspector.Native.NVAPI2;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8, Size = 4100)]
internal struct NVDRS_SETTING_UNION
{
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4100)]
	public byte[] rawData;

	public byte[] binaryValue
	{
		get
		{
			uint num = BitConverter.ToUInt32(rawData, 0);
			byte[] array = new byte[num];
			Buffer.BlockCopy(rawData, 4, array, 0, (int)num);
			return array;
		}
		set
		{
			rawData = new byte[4100];
			if (value != null)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(value.Length), 0, rawData, 0, 4);
				Buffer.BlockCopy(value, 0, rawData, 4, value.Length);
			}
		}
	}

	public uint dwordValue
	{
		get
		{
			return BitConverter.ToUInt32(rawData, 0);
		}
		set
		{
			rawData = new byte[4100];
			Buffer.BlockCopy(BitConverter.GetBytes(value), 0, rawData, 0, 4);
		}
	}

	public string stringValue
	{
		get
		{
			return Encoding.Unicode.GetString(rawData).Split(new char[1], 2)[0];
		}
		set
		{
			rawData = new byte[4100];
			byte[] bytes = Encoding.Unicode.GetBytes(value);
			Buffer.BlockCopy(bytes, 0, rawData, 0, bytes.Length);
		}
	}

	public string ansiStringValue
	{
		get
		{
			return Encoding.Default.GetString(rawData).Split(new char[1], 2)[0];
		}
		set
		{
			rawData = new byte[4100];
			byte[] bytes = Encoding.Default.GetBytes(value);
			Buffer.BlockCopy(bytes, 0, rawData, 0, bytes.Length);
		}
	}
}
