using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class RawInputClass
{
	internal struct RAWINPUTDEVICE
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort usUsagePage;

		[MarshalAs(UnmanagedType.U2)]
		public ushort usUsage;

		[MarshalAs(UnmanagedType.U4)]
		public int dwFlags;

		public IntPtr hwndTarget;
	}

	internal struct RAWHID
	{
		[MarshalAs(UnmanagedType.U4)]
		public int dwSizHid;

		[MarshalAs(UnmanagedType.U4)]
		public int dwCount;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RawMouse
	{
		[FieldOffset(0)]
		public RawMouseFlags Flags;

		[FieldOffset(4)]
		public RawMouseButtons ButtonFlags;

		[FieldOffset(6)]
		public ushort ButtonData;

		[FieldOffset(8)]
		public uint RawButtons;

		[FieldOffset(12)]
		public int LastX;

		[FieldOffset(16)]
		public int LastY;

		[FieldOffset(20)]
		public uint ExtraInformation;
	}

	internal struct RAWKEYBOARD
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort MakeCode;

		[MarshalAs(UnmanagedType.U2)]
		public ushort Flags;

		[MarshalAs(UnmanagedType.U2)]
		public ushort Reserved;

		[MarshalAs(UnmanagedType.U2)]
		public ushort VKey;

		[MarshalAs(UnmanagedType.U4)]
		public uint Message;

		[MarshalAs(UnmanagedType.U4)]
		public uint ExtraInformation;
	}

	public enum RawInputType
	{
		Mouse,
		Keyboard,
		HID
	}

	public struct RawInput(RawInputHeader _header, RawInput.Union _data)
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct Union
		{
			[FieldOffset(0)]
			public RawMouse Mouse;

			[FieldOffset(0)]
			public RAWKEYBOARD Keyboard;

			[FieldOffset(0)]
			public RAWHID HID;
		}

		public RawInputHeader Header = _header;

		public Union Data = _data;
	}

	internal struct RawInputHeader
	{
		public RawInputType Type;

		public int Size;

		public IntPtr Device;

		public IntPtr wParam;
	}

	private const int RID_INPUT = 268435459;

	private const int RIDEV_INPUTSINK = 256;

	internal static int GetDeviceID(IntPtr lParam)
	{
		try
		{
			uint pcbSize = 0u;
			NativeMethods.GetRawInputData(lParam, 268435459u, IntPtr.Zero, ref pcbSize, (uint)Marshal.SizeOf(typeof(RawInputHeader)));
			IntPtr intPtr = Marshal.AllocHGlobal((int)pcbSize);
			NativeMethods.GetRawInputData(lParam, 268435459u, intPtr, ref pcbSize, (uint)Marshal.SizeOf(typeof(RawInputHeader)));
			RawInput rawInput = (RawInput)Marshal.PtrToStructure(intPtr, typeof(RawInput));
			Marshal.FreeHGlobal(intPtr);
			if (rawInput.Data.Mouse.ButtonFlags == RawMouseButtons.LeftDown || rawInput.Data.Mouse.ButtonFlags == RawMouseButtons.RightDown)
			{
				return (int)rawInput.Header.Device;
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logger.Info("Exception in raw input constructor : {0}", new object[1] { ex.ToString() });
		}
		return -1;
	}

	public RawInputClass(IntPtr hwnd)
	{
		try
		{
			RAWINPUTDEVICE[] array = new RAWINPUTDEVICE[3];
			array[0].usUsagePage = 1;
			array[0].usUsage = 2;
			array[0].dwFlags = 256;
			array[0].hwndTarget = hwnd;
			array[1].usUsagePage = 1;
			array[1].usUsage = 5;
			array[1].dwFlags = 256;
			array[1].hwndTarget = hwnd;
			array[2].usUsagePage = 1;
			array[2].usUsage = 4;
			array[2].dwFlags = 256;
			array[2].hwndTarget = hwnd;
			if (!NativeMethods.RegisterRawInputDevices(array, (uint)array.Length, (uint)Marshal.SizeOf((object)array[0])))
			{
				Logger.Info("Failed to register raw input device(s).");
			}
			else
			{
				Logger.Info("Successfully registered raw input device(s).");
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Exception in raw input constructor : {0}", new object[1] { ex.ToString() });
		}
	}
}
