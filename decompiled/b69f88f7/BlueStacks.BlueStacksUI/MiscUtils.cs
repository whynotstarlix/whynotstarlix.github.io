using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using BlueStacks.Common;
using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI;

public static class MiscUtils
{
	private const int TextBoxFoxusAttemts = 10;

	private static SerialWorkQueue sFocusWorker;

	private static SerialWorkQueue FocusWorker
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			if (sFocusWorker == null)
			{
				sFocusWorker = new SerialWorkQueue();
				sFocusWorker.Start();
			}
			return sFocusWorker;
		}
	}

	public static void SetFocusAsync(UIElement control, int delay = 0)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		FocusWorker.Enqueue((Work)delegate
		{
			try
			{
				int i = 0;
				if (delay > 0)
				{
					Thread.Sleep(delay);
				}
				while (10 > i)
				{
					((DispatcherObject)control).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (control.Focus())
						{
							i = 11;
						}
					}, new object[0]);
					i++;
					Thread.Sleep(10);
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error setting focus on control" + ex.ToString());
			}
		});
	}

	public static void GetWindowWidthAndHeight(out int width, out int height)
	{
		int width2 = Screen.PrimaryScreen.Bounds.Width;
		int height2 = Screen.PrimaryScreen.Bounds.Height;
		if (width2 > 2560 && height2 > 1440)
		{
			width = 2560;
			height = 1440;
		}
		else if (width2 > 1920 && height2 > 1080)
		{
			width = 1920;
			height = 1080;
		}
		else if (width2 > 1600 && height2 > 900)
		{
			width = 1600;
			height = 900;
		}
		else if (width2 > 1280 && height2 > 720)
		{
			width = 1280;
			height = 720;
		}
		else
		{
			width = 960;
			height = 540;
		}
	}

	private static bool IsParametersValid(Window window)
	{
		try
		{
			if (window.Left < 0.0 || window.Left > SystemParameters.VirtualScreenWidth || window.Top < 0.0 || window.Top > SystemParameters.VirtualScreenHeight)
			{
				return false;
			}
			if (SystemParameters.VirtualScreenWidth - window.Left < ((FrameworkElement)window).Width / 10.0 || SystemParameters.VirtualScreenHeight - window.Top < ((FrameworkElement)window).Height / 10.0)
			{
				return false;
			}
			int screenWidth = RegistryManager.Instance.ScreenWidth;
			int screenHeight = RegistryManager.Instance.ScreenHeight;
			if (Math.Abs((double)screenWidth - SystemParameters.VirtualScreenWidth) > 100.0 || Math.Abs((double)screenHeight - SystemParameters.VirtualScreenHeight) > 100.0)
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Exception calculating size" + ex.ToString());
			return false;
		}
		return true;
	}

	private static void SaveControlSize(double width, double height, string prefix)
	{
		RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
		registryKey.SetValue(prefix + "Width", width, RegistryValueKind.DWord);
		registryKey.SetValue(prefix + "Height", height, RegistryValueKind.DWord);
		registryKey.Close();
	}

	public static void SetWindowSizeAndLocation(Window window, string prefix, bool isGMWindow = false)
	{
		if (window == null)
		{
			return;
		}
		try
		{
			double num = 1.7777777777777777;
			bool flag = true;
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
			if ((int)registryKey.GetValue(prefix + "Width", int.MinValue) != int.MinValue)
			{
				try
				{
					((FrameworkElement)window).Width = (int)registryKey.GetValue(prefix + "Width");
					((FrameworkElement)window).Height = (int)registryKey.GetValue(prefix + "Height");
					RegistryKey registryKey2 = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
					window.Left = (int)registryKey2.GetValue(prefix + "Left");
					window.Top = (int)registryKey2.GetValue(prefix + "Top");
					flag = false;
					if (!IsParametersValid(window))
					{
						flag = true;
					}
				}
				catch (Exception ex)
				{
					Logger.Info("Exception in geting value from reg" + ex.ToString());
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			double num2 = default(double);
			double num3 = default(double);
			double num4 = default(double);
			WpfUtils.GetDefaultSize(ref num2, ref num3, ref num4, num, isGMWindow);
			double left = num4 + num2;
			double top = (int)(SystemParameters.PrimaryScreenHeight - num3) / 2;
			if (isGMWindow)
			{
				window.Left = num4;
				window.Top = top;
				((FrameworkElement)window).Height = num3;
				((FrameworkElement)window).Width = num2;
				SaveControlSize(num2, num3, "DefaultGM");
				return;
			}
			window.Left = left;
			window.Top = top;
			((FrameworkElement)window).Height = num3;
			((FrameworkElement)window).Width = (((FrameworkElement)window).Height - 33.0) / 27.0 * 16.0;
			if (window.Left + ((FrameworkElement)window).Width > SystemParameters.PrimaryScreenWidth)
			{
				window.Left = SystemParameters.PrimaryScreenWidth - ((FrameworkElement)window).Width - 20.0;
			}
		}
		catch (Exception ex2)
		{
			Logger.Info("Exception getting size" + ex2.ToString());
		}
	}

	public static int Extract7Zip(string zipFilePath, string extractDirectory)
	{
		string text = Path.Combine(RegistryStrings.InstallDir, "7zr.exe");
		if (!Directory.Exists(extractDirectory))
		{
			Directory.CreateDirectory(extractDirectory);
		}
		string text2 = string.Format(CultureInfo.InvariantCulture, "x \"{0}\" -o\"{1}\" -aoa", new object[2] { zipFilePath, extractDirectory });
		return RunCommand.RunCmd(text, text2, false, true, false, 0).ExitCode;
	}

	public static void GetStreamWidthAndHeight(int sWidth, int sHeight, out int width, out int height)
	{
		height = Utils.GetInt(RegistryManager.Instance.FrontendHeight, sHeight);
		width = Utils.GetInt(RegistryManager.Instance.FrontendWidth, sWidth);
	}
}
