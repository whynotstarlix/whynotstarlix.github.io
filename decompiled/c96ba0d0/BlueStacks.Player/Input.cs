using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlueStacks.Player;

internal class Input
{
	private struct HookData
	{
		public uint vkCode;

		public uint scanCode;

		public uint flags;

		public uint time;

		public IntPtr dwExtraInfo;
	}

	public delegate bool KeyboardCallback(bool pressed, uint key);

	private delegate int HookProc(int code, uint wparam, IntPtr lparam);

	private const uint MOUSEEVENTF_FROMTOUCH = 4283518976u;

	private const uint MOUSEEVENTF_FROMPEN = 4283519232u;

	private const uint MOUSEEVENTF_MASK = 4294967040u;

	private const int WM_KEYDOWN = 256;

	private const int WM_KEYUP = 257;

	private const int WM_SYSKEYDOWN = 260;

	private const int WM_SYSKEYUP = 261;

	private const int WH_KEYBOARD_LL = 13;

	private const int HC_ACTION = 0;

	public const int VK_LWIN = 91;

	private static int sHookHandle;

	private static HookProc sHookProc;

	[DllImport("user32.dll")]
	private static extern uint GetMessageExtraInfo();

	public static bool IsEventFromTouch()
	{
		return (GetMessageExtraInfo() & 0xFFFFFF00u) == 4283518976u;
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern ushort GlobalAddAtom(string str);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetProp(IntPtr wind, string str, IntPtr data);

	public static void DisablePressAndHold(IntPtr hWnd)
	{
		string str = "MicrosoftTabletPenServiceProperty";
		if (GlobalAddAtom(str) == 0)
		{
			throw new SystemException("Cannot add global atom", new Win32Exception(Marshal.GetLastWin32Error()));
		}
		if (!SetProp(hWnd, str, (IntPtr)1))
		{
			throw new SystemException("Cannot set property", new Win32Exception(Marshal.GetLastWin32Error()));
		}
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern int SetWindowsHookEx(int type, HookProc callback, IntPtr module, uint threadId);

	[DllImport("user32.dll")]
	private static extern int CallNextHookEx(int handle, int code, uint wparam, IntPtr lparam);

	[DllImport("user32.dll")]
	private static extern bool UnhookWindowsHookEx(int handle);

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetModuleHandle(IntPtr name);

	public static void HookKeyboard(KeyboardCallback cb)
	{
		sHookProc = (int code, uint wparam, IntPtr lparam) =>
		{
			if (code < 0)
			{
				return CallNextHookEx(sHookHandle, code, wparam, lparam);
			}
			if (wparam == 260 || wparam == 261)
			{
				return CallNextHookEx(sHookHandle, code, wparam, lparam);
			}
			HookData hookData = (HookData)Marshal.PtrToStructure(lparam, typeof(HookData));
			bool pressed = wparam == 256;
			SensorKeyboardIntegration(hookData.vkCode, pressed);
			return (!cb(pressed, hookData.vkCode)) ? 1 : CallNextHookEx(sHookHandle, code, wparam, lparam);
		};
		if (sHookHandle != 0)
		{
			throw new SystemException("Keyboard hook is already set");
		}
		IntPtr moduleHandle = GetModuleHandle(IntPtr.Zero);
		sHookHandle = SetWindowsHookEx(13, sHookProc, moduleHandle, 0u);
		if (sHookHandle == 0)
		{
			throw new SystemException("Cannot set hooks", new Win32Exception(Marshal.GetLastWin32Error()));
		}
	}

	public static void UnhookKeyboard()
	{
		if (sHookHandle != 0)
		{
			UnhookWindowsHookEx(sHookHandle);
			sHookHandle = 0;
		}
	}

	public static bool IsFastTouchEvent()
	{
		uint messageExtraInfo = GetMessageExtraInfo();
		if ((messageExtraInfo & 0xFFFFFF00u) != 4283518976u)
		{
			return (messageExtraInfo & 0xFFFFFF00u) == 4283519232u;
		}
		return true;
	}

	private static void SensorKeyboardIntegration(uint key, bool pressed)
	{
		if (pressed && SensorDevice.Instance != null)
		{
			float origX = 0f;
			float origY = 0f;
			float origZ = 0f;
			switch (key)
			{
			case 87u:
				origY = 1f;
				break;
			case 83u:
				origY = -1f;
				break;
			case 65u:
				origX = -1f;
				break;
			case 68u:
				origX = 1f;
				break;
			case 32u:
				origZ = 1f;
				break;
			}
			SensorDevice.Instance.SetAccelerometerVector(origX, origY, origZ);
		}
	}
}
