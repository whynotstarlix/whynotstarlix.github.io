using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.BlueStacksUI;

namespace Bluester;

public class AutoAcceptMatch
{
	private struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	private readonly Color TargetColor = Color.FromArgb(248, 175, 59);

	private const int MOUSEEVENTF_LEFTDOWN = 2;

	private const int MOUSEEVENTF_LEFTUP = 4;

	private Thread workerThread;

	private Options.OptionsSettingsManager.OptionsConfig config;

	[DllImport("user32.dll")]
	private static extern IntPtr GetDC(IntPtr hwnd);

	[DllImport("user32.dll")]
	private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

	[DllImport("gdi32.dll")]
	private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

	[DllImport("user32.dll")]
	private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

	[DllImport("user32.dll")]
	private static extern bool SetCursorPos(int X, int Y);

	[DllImport("user32.dll")]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	public void Init()
	{
		Options.OptionsSettingsManager.OnSettingsSaved += ReloadSettings;
		ReloadSettings();
		workerThread = new Thread(WorkLoop)
		{
			IsBackground = true
		};
		workerThread.Start();
	}

	private void ReloadSettings()
	{
		config = Options.OptionsSettingsManager.Load();
	}

	private void WorkLoop()
	{
		while (true)
		{
			try
			{
				if (config == null || !config.AutoAcceptEnabled)
				{
					Thread.Sleep(1000);
					continue;
				}
				ProcessLogic();
			}
			catch
			{
			}
			Thread.Sleep(500);
		}
	}

	private void ProcessLogic()
	{
		IntPtr intPtr = FindBlueStacksWindow();
		if (!(intPtr != IntPtr.Zero) || !GetWindowRect(intPtr, out var lpRect))
		{
			return;
		}
		int num = lpRect.Right - lpRect.Left;
		int num2 = lpRect.Bottom - lpRect.Top;
		if (num > 0 && num2 > 0)
		{
			int x = lpRect.Left + num / 2;
			int y = lpRect.Top + num2 / 2;
			Color pixelColor = GetPixelColor(x, y);
			if (IsColorMatch(pixelColor, TargetColor))
			{
				DoMouseClick(x, y);
				Thread.Sleep(3000);
			}
		}
	}

	private IntPtr FindBlueStacksWindow()
	{
		Process[] processesByName = Process.GetProcessesByName("BlueStacks");
		if (processesByName.Length != 0)
		{
			return processesByName[0].MainWindowHandle;
		}
		processesByName = Process.GetProcessesByName("BlueStacks");
		if (processesByName.Length != 0)
		{
			return processesByName[0].MainWindowHandle;
		}
		return IntPtr.Zero;
	}

	private bool IsColorMatch(Color c1, Color c2)
	{
		if (c1.R == c2.R && c1.G == c2.G)
		{
			return c1.B == c2.B;
		}
		return false;
	}

	private Color GetPixelColor(int x, int y)
	{
		IntPtr dC = GetDC(IntPtr.Zero);
		uint pixel = GetPixel(dC, x, y);
		ReleaseDC(IntPtr.Zero, dC);
		return Color.FromArgb((int)(pixel & 0xFF), (int)(pixel & 0xFF00) >> 8, (int)(pixel & 0xFF0000) >> 16);
	}

	private void DoMouseClick(int x, int y)
	{
		SetCursorPos(x, y);
		mouse_event(2, x, y, 0, 0);
		Thread.Sleep(50);
		mouse_event(4, x, y, 0, 0);
	}
}
