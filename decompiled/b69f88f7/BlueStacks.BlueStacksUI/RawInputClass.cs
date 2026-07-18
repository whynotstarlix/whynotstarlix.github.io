using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.BlueStacksUI;

internal class RawInputClass
{
	internal struct RAWINPUTDEVICE
	{
		public ushort usUsagePage;

		public ushort usUsage;

		public int dwFlags;

		public IntPtr hwndTarget;
	}

	internal struct RAWHID
	{
		public int dwSizHid;

		public int dwCount;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RawMouse
	{
		[FieldOffset(0)]
		public ushort usFlags;

		[FieldOffset(4)]
		public RawMouseButtons ButtonFlags;

		[FieldOffset(6)]
		public ushort usButtonData;

		[FieldOffset(8)]
		public uint ulRawButtons;

		[FieldOffset(12)]
		public int lLastX;

		[FieldOffset(16)]
		public int lLastY;

		[FieldOffset(20)]
		public uint ulExtraInformation;
	}

	internal struct RAWKEYBOARD
	{
		public ushort MakeCode;

		public ushort Flags;

		public ushort Reserved;

		public ushort VKey;

		public uint Message;

		public uint ExtraInformation;
	}

	public enum RawInputType
	{
		Mouse,
		Keyboard,
		HID
	}

	public struct RawInput
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

		public RawInputHeader Header;

		public Union Data;
	}

	internal struct RawInputHeader
	{
		public RawInputType Type;

		public int Size;

		public IntPtr Device;

		public IntPtr wParam;
	}

	[Flags]
	public enum RawMouseButtons : ushort
	{
		LeftDown = 1,
		LeftUp = 2,
		RightDown = 4,
		RightUp = 8,
		MiddleDown = 0x10,
		MiddleUp = 0x20,
		Button4Down = 0x40,
		Button4Up = 0x80,
		Button5Down = 0x100,
		Button5Up = 0x200,
		MouseWheel = 0x400
	}

	private const int RID_INPUT = 268435459;

	private const int RIDEV_INPUTSINK = 256;

	private const ushort HID_USAGE_PAGE_GENERIC = 1;

	private const ushort HID_USAGE_GENERIC_MOUSE = 2;

	private const ushort HID_USAGE_GENERIC_GAMEPAD = 5;

	private const ushort HID_USAGE_GENERIC_JOYSTICK = 4;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SecuritySafeCritical]
	internal unsafe static int GetDeviceID(IntPtr lParam)
	{
		try
		{
			uint pcbSize = 0u;
			uint cbSizeHeader = (uint)sizeof(RawInputHeader);
			GetRawInputData(lParam, 268435459u, IntPtr.Zero, ref pcbSize, cbSizeHeader);
			if (pcbSize == 0)
			{
				return -1;
			}
			byte* ptr = stackalloc byte[(int)pcbSize];
			if (GetRawInputData(lParam, 268435459u, (IntPtr)ptr, ref pcbSize, cbSizeHeader) == -1)
			{
				return -1;
			}
			RawInputHeader* ptr2 = (RawInputHeader*)ptr;
			if (ptr2->Type == RawInputType.Mouse)
			{
				RawInput* ptr3 = (RawInput*)ptr;
				RawMouseButtons buttonFlags = ptr3->Data.Mouse.ButtonFlags;
				if ((buttonFlags & RawMouseButtons.LeftDown) != 0 || (buttonFlags & RawMouseButtons.RightDown) != 0)
				{
					return (int)ptr2->Device;
				}
			}
		}
		catch
		{
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
			RegisterRawInputDevices(array, (uint)array.Length, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
		}
		catch
		{
		}
	}

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern int GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);
}
