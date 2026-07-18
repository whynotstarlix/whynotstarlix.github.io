using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;

namespace Bluester;

public class ProcessControl
{
	private bool isKeyPressed;

	private Thread workerThread;

	private readonly SynchronizationContext context;

	private bool isAlertShown;

	private Options.OptionsSettingsManager.OptionsConfig config;

	private Keys freezeKey;

	private Keys unfreezeKey;

	private Keys killKey;

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

	public ProcessControl()
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		while (true)
		{
			if (Options.IsBindingKey)
			{
				Thread.Sleep(50);
				continue;
			}
			string vmname = Opt.Instance.vmname;
			string windowTitle = RegistryManager.Instance.Guest[vmname].DisplayName ?? vmname;
			if (config.FreezeEnabled && (int)freezeKey != 0 && (GetAsyncKeyState(freezeKey) & 0x8000) != 0 && !isKeyPressed)
			{
				isKeyPressed = true;
				ShowNotification("Игра заморожена");
				SuspendInstanceProcesses(vmname, windowTitle);
				Thread.Sleep(500);
			}
			else if (config.UnfreezeEnabled && (int)unfreezeKey != 0 && (GetAsyncKeyState(unfreezeKey) & 0x8000) != 0 && !isKeyPressed)
			{
				isKeyPressed = true;
				ResumeInstanceProcesses(vmname, windowTitle);
				if (!isAlertShown)
				{
					ShowNotification("Игра разморожена");
					isAlertShown = true;
				}
				Thread.Sleep(500);
			}
			else if (config.KillEnabled && (int)killKey != 0 && (GetAsyncKeyState(killKey) & 0x8000) != 0 && !isKeyPressed)
			{
				isKeyPressed = true;
				KillInstanceProcesses(vmname, windowTitle);
				if (!isAlertShown)
				{
					ShowNotification("Игра закрыта");
					isAlertShown = true;
				}
				Thread.Sleep(500);
			}
			else if ((GetAsyncKeyState(freezeKey) & 0x8000) == 0 && (GetAsyncKeyState(unfreezeKey) & 0x8000) == 0 && (GetAsyncKeyState(killKey) & 0x8000) == 0)
			{
				isKeyPressed = false;
				isAlertShown = false;
			}
			Thread.Sleep(50);
		}
	}

	private void SuspendInstanceProcesses(string vmName, string windowTitle)
	{
		Process[] processesByName = Process.GetProcessesByName("HD-Player");
		foreach (Process process in processesByName)
		{
			if (process.MainWindowTitle.Contains(windowTitle))
			{
				NtSuspendProcess(process.Handle);
			}
		}
	}

	private void ResumeInstanceProcesses(string vmName, string windowTitle)
	{
		Process[] processesByName = Process.GetProcessesByName("HD-Player");
		foreach (Process process in processesByName)
		{
			if (process.MainWindowTitle.Contains(windowTitle))
			{
				NtResumeProcess(process.Handle);
			}
		}
	}

	private void KillInstanceProcesses(string vmName, string windowTitle)
	{
		Process[] processesByName = Process.GetProcessesByName("HD-Player");
		foreach (Process process in processesByName)
		{
			if (process.MainWindowTitle.Contains(windowTitle))
			{
				process.Kill();
			}
		}
	}

	[DllImport("ntdll.dll")]
	private static extern int NtSuspendProcess(IntPtr processHandle);

	[DllImport("ntdll.dll")]
	private static extern int NtResumeProcess(IntPtr processHandle);

	private void ReloadSettings()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		config = Options.OptionsSettingsManager.Load();
		try
		{
			Enum.TryParse<Keys>(config.FreezeKeybind, true, out freezeKey);
			Enum.TryParse<Keys>(config.UnfreezeKeybind, true, out unfreezeKey);
			Enum.TryParse<Keys>(config.KillKeybind, true, out killKey);
		}
		catch
		{
			freezeKey = (Keys)0;
			unfreezeKey = (Keys)0;
			killKey = (Keys)0;
		}
	}
}
