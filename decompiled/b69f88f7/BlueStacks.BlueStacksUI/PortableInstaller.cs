using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class PortableInstaller
{
	internal static void CheckAndRunPortableInstaller()
	{
		try
		{
			if (Oem.Instance.IsPortableInstaller)
			{
				string value = (string)RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "Version", (object)"", (RegistryKeyKind)0);
				string fullName = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.Trim(new char[1] { '\\' })).FullName;
				Logger.InitLogAtPath(Path.Combine(fullName, "Logs\\PortableInstaller.log"), "PortableInstaller", true);
				if (string.IsNullOrEmpty(value) || (string)RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "InstallDir", (object)"", (RegistryKeyKind)0) != Path.Combine(fullName, "BlueStacksPF") || Opt.Instance.isForceInstall)
				{
					InstallPortableBlueStacks(AppDomain.CurrentDomain.BaseDirectory);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error in CheckAndRunPortableInstaller" + ex);
		}
	}

	private static void InstallPortableBlueStacks(string cwd)
	{
		try
		{
			string fullName = Directory.GetParent(cwd.Trim(new char[1] { '\\' })).FullName;
			string text = Path.Combine(fullName, "Engine");
			string text2 = Path.Combine(fullName, "BlueStacksPF");
			string path = Path.Combine(Path.Combine(text, "Android"), "Android.bstk");
			string path2 = Path.Combine(Path.Combine(text, "Manager"), "BstkGlobal.xml");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			if (File.Exists(path2))
			{
				File.Delete(path2);
			}
			CommonInstallUtils.ModifyDirectoryPermissionsForEveryone(fullName);
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.bat")))
			{
				if (RunInstallBat(text2, text) == 0)
				{
					FixRegistries(fullName, text2);
					DoComRegistration(text2);
					CommonInstallUtils.InstallVirtualBoxConfig(text, false);
					CommonInstallUtils.InstallVmConfig(text2, text);
				}
			}
			else
			{
				Logger.Error("Install.bat file missing");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in InstallPortableBlueStacks " + ex);
		}
	}

	private static void FixRegistries(string userDefinedDir, string installDir)
	{
		string text = Path.Combine(userDefinedDir, "Engine");
		string text2 = "Android";
		int num = 46;
		int num2 = 54;
		RegistryManager.Instance.SetAccessPermissions();
		RegistryManager.Instance.UserDefinedDir = userDefinedDir.Trim(new char[1] { '\\' });
		RegistryManager.Instance.DataDir = text.Trim(new char[1] { '\\' }) + "\\";
		RegistryManager.Instance.LogDir = Path.Combine(userDefinedDir, "Logs").Trim(new char[1] { '\\' }) + "\\";
		RegistryManager.Instance.InstallDir = installDir.Trim(new char[1] { '\\' }) + "\\";
		RegistryManager.Instance.EngineDataDir = Path.Combine(userDefinedDir, "Engine");
		RegistryManager.Instance.ClientInstallDir = Path.Combine(userDefinedDir, "Client");
		RegistryManager.Instance.CefDataPath = Path.Combine(userDefinedDir, "CefData");
		RegistryManager.Instance.SetupFolder = Path.Combine(Directory.GetParent(userDefinedDir).ToString(), "BlueStacksSetup");
		RegistryManager.Instance.PartnerExePath = Path.Combine(RegistryManager.Instance.ClientInstallDir, "BlueStacks.exe");
		RegistryManager.Instance.UserGuid = Guid.NewGuid().ToString();
		Utils.UpdateValueInBootParams("GUID", RegistryManager.Instance.UserGuid, text2, true, "bgp64");
		string text3 = Path.Combine(text, text2);
		text3 += "\\";
		RegistryManager.Instance.Guest[text2].BlockDevice0Name = "sda1";
		RegistryManager.Instance.Guest[text2].BlockDevice0Path = text3 + "Root.vdi";
		RegistryManager.Instance.Guest[text2].BlockDevice1Name = "sdb1";
		RegistryManager.Instance.Guest[text2].BlockDevice1Path = text3 + "Data.vdi";
		RegistryManager.Instance.Guest[text2].BlockDevice2Name = "sdc1";
		RegistryManager.Instance.Guest[text2].BlockDevice2Path = text3 + "SDCard.vdi";
		string sharedFolder0Path = Path.Combine(text, "UserData\\SharedFolder\\");
		RegistryManager.Instance.Guest[text2].SharedFolder0Name = "BstSharedFolder";
		RegistryManager.Instance.Guest[text2].SharedFolder0Path = sharedFolder0Path;
		RegistryManager.Instance.Guest[text2].SharedFolder0Writable = 1;
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		RegistryManager.Instance.Guest[text2].SharedFolder1Name = "Pictures";
		RegistryManager.Instance.Guest[text2].SharedFolder1Path = folderPath;
		RegistryManager.Instance.Guest[text2].SharedFolder1Writable = 1;
		string folderPath2 = CommonInstallUtils.GetFolderPath(num2);
		RegistryManager.Instance.Guest[text2].SharedFolder2Name = "PublicPictures";
		RegistryManager.Instance.Guest[text2].SharedFolder2Path = folderPath2;
		RegistryManager.Instance.Guest[text2].SharedFolder2Writable = 1;
		string folderPath3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		RegistryManager.Instance.Guest[text2].SharedFolder3Name = "Documents";
		RegistryManager.Instance.Guest[text2].SharedFolder3Path = folderPath3;
		RegistryManager.Instance.Guest[text2].SharedFolder3Writable = 1;
		string folderPath4 = CommonInstallUtils.GetFolderPath(num);
		RegistryManager.Instance.Guest[text2].SharedFolder4Name = "PublicDocuments";
		RegistryManager.Instance.Guest[text2].SharedFolder4Path = folderPath4;
		RegistryManager.Instance.Guest[text2].SharedFolder4Writable = 1;
		string sharedFolder5Path = Path.Combine(text, "UserData\\InputMapper");
		RegistryManager.Instance.Guest[text2].SharedFolder5Name = "InputMapper";
		RegistryManager.Instance.Guest[text2].SharedFolder5Path = sharedFolder5Path;
		RegistryManager.Instance.Guest[text2].SharedFolder5Writable = 1;
	}

	private static int RunInstallBat(string installDir, string dataDir)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		Process process = new Process();
		process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
		process.StartInfo.FileName = "install.bat";
		process.StartInfo.Arguments = "\"" + installDir + "\" \"" + dataDir + "\"";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		Countdown countDown = new Countdown(2);
		StringBuilder sb = new StringBuilder();
		process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
		{
			if (outLine.Data != null)
			{
				try
				{
					string data = outLine.Data;
					sb.AppendLine(data);
					Logger.Info(data);
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception in RunInstallBat");
					Console.WriteLine(ex.ToString());
					return;
				}
			}
			countDown.Signal();
		};
		process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
		{
			if (outLine.Data != null)
			{
				try
				{
					string data = outLine.Data;
					sb.AppendLine(data);
					Logger.Info(data);
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine("A crash occured in RunInstallBat");
					Console.WriteLine(ex.ToString());
					return;
				}
			}
			countDown.Signal();
		};
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		int milliseconds = 200000;
		process.WaitForExit(milliseconds);
		Logger.Info("Exit Code for InstallBat " + process.ExitCode);
		countDown.Wait();
		return process.ExitCode;
	}

	private static void DoComRegistration(string installDir)
	{
		string text = "HD-ComRegistrar.exe";
		try
		{
			Logger.Info("Starting registration of COM process with: {0}", new object[1] { text });
			Process process = new Process();
			process.StartInfo.FileName = Path.Combine(installDir, text);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.WaitForExit();
			Logger.Info("ExitCode: {0}", new object[1] { process.ExitCode });
		}
		catch (Exception ex)
		{
			Logger.Warning("Failed to execute process {0}. Err: {1}", new object[2]
			{
				text,
				ex.ToString()
			});
		}
	}
}
