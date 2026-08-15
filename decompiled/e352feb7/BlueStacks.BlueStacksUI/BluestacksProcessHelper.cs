using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class BluestacksProcessHelper
{
	internal static int StartFrontend(string vmName)
	{
		try
		{
			string installDir = RegistryStrings.InstallDir;
			string fileName = Path.Combine(installDir, "HD-Player.exe");
			string text = (FeatureManager.Instance.IsUseWpfTextbox ? " -w" : "");
			string text2 = " -h";
			if (RegistryManager.Instance.DevEnv == 1)
			{
				text2 = "";
			}
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = vmName + text2 + text;
			process.StartInfo.WorkingDirectory = installDir;
			Logger.Info("Starting Frontend for vm: {0} with args: {1}", new object[2]
			{
				vmName,
				process.StartInfo.Arguments
			});
			process.Start();
			process.WaitForExit();
			return process.ExitCode;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in starting frontend. Err : " + ex.ToString());
		}
		return 0;
	}

	public static void RunUpdateInstaller(string filePath, string arg, bool isAdmin = false)
	{
		Logger.Info("RunUpdateInstaller start");
		try
		{
			using Process process = new Process();
			process.StartInfo.FileName = filePath;
			process.StartInfo.Arguments = arg;
			if (isAdmin)
			{
				process.StartInfo.Verb = "runas";
			}
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in running update installer " + ex.ToString());
		}
	}

	public static Process StartBluestacks(string vmName)
	{
		string fileName = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.FileName = fileName;
		process.StartInfo.Arguments = "-vmname:" + vmName + " -h";
		Logger.Info("Sending RunApp for vm calling {0}", new object[1] { vmName });
		Logger.Info("Utils: Starting hidden Frontend");
		process.Start();
		return process;
	}

	public static int RunApkInstaller(string apkPath, bool isSilentInstall, string vmName)
	{
		Logger.Info("Installing apk :{0} vmname: {1} ", new object[2] { apkPath, vmName });
		if (vmName == null)
		{
			vmName = "Android";
		}
		int result = -1;
		try
		{
			string installDir = RegistryStrings.InstallDir;
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				WorkingDirectory = installDir
			};
			if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
			{
				processStartInfo.FileName = Path.Combine(installDir, "HD-XapkHandler.exe");
				if (isSilentInstall)
				{
					processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -s -vmname {1}", new object[2] { apkPath, vmName });
				}
				else
				{
					processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -vmname {1}", new object[2] { apkPath, vmName });
				}
			}
			else
			{
				processStartInfo.FileName = Path.Combine(installDir, "HD-ApkHandler.exe");
				if (isSilentInstall)
				{
					processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -s -vmname {1}", new object[2] { apkPath, vmName });
				}
				else
				{
					processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -vmname {1}", new object[2] { apkPath, vmName });
				}
			}
			processStartInfo.UseShellExecute = false;
			processStartInfo.CreateNoWindow = true;
			Logger.Info("Console: installer path {0}", new object[1] { processStartInfo.FileName });
			Process process = Process.Start(processStartInfo);
			process.WaitForExit();
			result = process.ExitCode;
			Logger.Info("Console: apk installer exit code: {0}", new object[1] { process.ExitCode });
		}
		catch (Exception ex)
		{
			Logger.Info("Error Installing Apk : " + ex.ToString());
		}
		return result;
	}

	internal static bool TakeLock(string lockBane)
	{
		Mutex mutex = default(Mutex);
		return ProcessUtils.CheckAlreadyRunningAndTakeLock(lockBane, ref mutex);
	}
}
