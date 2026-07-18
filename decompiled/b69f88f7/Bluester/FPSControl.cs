using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Microsoft.Win32;

namespace Bluester;

public class FPSControl
{
	private bool isKeyPressed;

	private Thread workerThread;

	private readonly SynchronizationContext context;

	private bool isAlertShown;

	private Options.OptionsSettingsManager.OptionsConfig config;

	private Keys lockKey;

	private Keys unlockKey;

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern short GetAsyncKeyState(Keys vKey);

	public void Init()
	{
		Options.OptionsSettingsManager.OnSettingsSaved += ReloadSettings;
		ReloadSettings();
		workerThread = new Thread(CheckForBinds)
		{
			IsBackground = true
		};
		workerThread.Start();
	}

	public FPSControl()
	{
		SynchronizationContext current = SynchronizationContext.Current;
		context = current;
	}

	private void ShowNotification(string msg)
	{
		if (context != null)
		{
			context.Post(delegate
			{
				((Control)new NotificationForm(msg)).Show();
			}, null);
		}
		Thread.Sleep(500);
	}

	private void CheckForBinds()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		while (true)
		{
			if (Options.IsBindingKey)
			{
				Thread.Sleep(50);
				continue;
			}
			if (config.LockEnabled && (int)lockKey != 0 && (GetAsyncKeyState(lockKey) & 0x8000) != 0 && !isKeyPressed)
			{
				isKeyPressed = true;
				if (int.TryParse(config.LockValue, out var result))
				{
					SetFps(result);
					RegistryManager.Instance.Guest[Opt.Instance.vmname].EnableHighFPS = ((result > 30) ? 1 : 0);
					RegistryManager.Instance.Guest[Opt.Instance.vmname].FPS = result;
				}
				Thread.Sleep(500);
				isAlertShown = false;
			}
			else if (config.UnlockEnabled && (int)unlockKey != 0 && (GetAsyncKeyState(unlockKey) & 0x8000) != 0 && !isKeyPressed)
			{
				isKeyPressed = true;
				if (int.TryParse(config.UnlockValue, out var result2))
				{
					SetFps(result2);
					RegistryManager.Instance.Guest[Opt.Instance.vmname].EnableHighFPS = 1;
					RegistryManager.Instance.Guest[Opt.Instance.vmname].FPS = result2;
				}
				Thread.Sleep(500);
				isAlertShown = false;
			}
			else if ((GetAsyncKeyState(lockKey) & 0x8000) == 0 && (GetAsyncKeyState(unlockKey) & 0x8000) == 0)
			{
				isKeyPressed = false;
			}
			Thread.Sleep(50);
		}
	}

	private async void SetFps(int fps)
	{
		try
		{
			using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BlueStacks_bgp64\\Guests");
			if (registryKey == null)
			{
				return;
			}
			string[] subKeyNames = registryKey.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				if (!text.StartsWith("Android", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string name = "SOFTWARE\\BlueStacks_bgp64\\Guests\\" + text + "\\Config";
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name, writable: true))
				{
					registryKey2?.SetValue("FPS", fps, RegistryValueKind.DWord);
				}
				string name2 = "SOFTWARE\\BlueStacks_bgp64\\Guests\\" + text;
				using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey(name2, writable: true))
				{
					if (registryKey3 != null)
					{
						string text2 = registryKey3.GetValue("BootParameters") as string;
						if (!string.IsNullOrEmpty(text2))
						{
							string value = Regex.Replace(text2, "fps=\\d+", $"fps={fps}");
							registryKey3.SetValue("BootParameters", value, RegistryValueKind.String);
						}
					}
				}
				Utils.SendChangeFPSToInstanceASync(text, fps);
				if (!isAlertShown)
				{
					ShowNotification($"Ограничение ФПС: {fps}");
					isAlertShown = true;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Ошибка: " + ex.Message);
		}
	}

	private void ReloadSettings()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		config = Options.OptionsSettingsManager.Load();
		try
		{
			Enum.TryParse<Keys>(config.LockKeybind, true, out lockKey);
			Enum.TryParse<Keys>(config.UnlockKeybind, true, out unlockKey);
		}
		catch
		{
			lockKey = (Keys)0;
			unlockKey = (Keys)0;
		}
	}
}
