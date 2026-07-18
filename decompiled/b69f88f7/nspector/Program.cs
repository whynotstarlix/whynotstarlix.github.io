using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nspector.Common;
using nspector.Common.Helper;
using nspector.Native.WINAPI;
using nspector.Properties;

namespace nspector;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SafeNativeMethods.DeleteFile(Application.ExecutablePath + ":Zone.Identifier");
		}
		catch
		{
		}
		try
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			DropDownMenuScrollWheelHandler.Enable(enabled: true);
			int num = ArgFileIndex(args);
			if (num != -1)
			{
				if (!(new FileInfo(args[num]).Extension.ToLowerInvariant() == ".nip"))
				{
					return;
				}
				try
				{
					DrsImportService importService = DrsServiceLocator.ImportService;
					string text = importService.ImportProfiles(args[num]);
					GC.Collect();
					Process currentProcess = Process.GetCurrentProcess();
					Process[] processesByName = Process.GetProcessesByName(currentProcess.ProcessName.Replace(".vshost", ""));
					foreach (Process process in processesByName)
					{
						if (process.Id != currentProcess.Id && process.MainWindowTitle.Contains("Settings"))
						{
							MessageHelper messageHelper = new MessageHelper();
							messageHelper.sendWindowsStringMessage((int)process.MainWindowHandle, 0, "ProfilesImported");
						}
					}
					if (string.IsNullOrEmpty(text) && !ArgExists(args, "-silentImport") && !ArgExists(args, "-silent"))
					{
						frmDrvSettings.ShowImportDoneMessage(text);
					}
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Import Error: " + ex.Message, Application.ProductName + " Error", (MessageBoxButtons)0, (MessageBoxIcon)16);
					return;
				}
			}
			if (ArgExists(args, "-createCSN"))
			{
				File.WriteAllText("CustomSettingNames.xml", Resources.CustomSettingNames);
				return;
			}
			bool createdNew = true;
			using (new Mutex(initiallyOwned: true, Application.ProductName, out createdNew))
			{
				if (createdNew)
				{
					Application.Run((Form)(object)new frmDrvSettings(ArgExists(args, "-showOnlyCSN"), ArgExists(args, "-disableScan")));
					return;
				}
				Process currentProcess2 = Process.GetCurrentProcess();
				Process[] processesByName2 = Process.GetProcessesByName(currentProcess2.ProcessName.Replace(".vshost", ""));
				foreach (Process process2 in processesByName2)
				{
					if (process2.Id != currentProcess2.Id && process2.MainWindowTitle.Contains("Settings"))
					{
						MessageHelper messageHelper2 = new MessageHelper();
						messageHelper2.bringAppToFront((int)process2.MainWindowHandle);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message + "\r\n\r\n" + ex2.StackTrace, "Error", (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
	}

	private static bool ArgExists(string[] args, string arg)
	{
		foreach (string text in args)
		{
			if (text.ToUpper() == arg.ToUpper())
			{
				return true;
			}
		}
		return false;
	}

	private static int ArgFileIndex(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (File.Exists(args[i]))
			{
				return i;
			}
		}
		return -1;
	}
}
