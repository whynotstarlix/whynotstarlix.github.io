using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

public class FileImporter
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec__7
	{
		public static readonly _003C_003Ec__7 _003C_003E9 = new _003C_003Ec__7();

		public static DragEventHandler _003C_003E9__0_0;

		internal void _003CMakeDragDropHandler_003Eb__0_0(object obj, DragEventArgs evt)
		{
			Thread thread = new Thread(() =>
			{
				HandleDragDropAsync(evt);
			});
			thread.IsBackground = true;
			thread.Start();
		}
	}

	public static DragEventHandler MakeDragDropHandler()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		try
		{
			if ((int)Assembly.LoadFrom("C:\\ProgramData\\BlueStacks_bgp64\\Client\\Bluestacks.exe").GetType("BlueStacks.BlueStacksUI.ZipFileEntry").GetMethod("AlignChunkBoundary", BindingFlags.Static | BindingFlags.Public)
				.Invoke(null, new object[2] { 74L, 167 }) == -1)
			{
				MessageBox.Show("Вы используете не официальную версию AXIOMA!", "Ошибка");
				Environment.Exit(-1);
			}
		}
		catch
		{
			MessageBox.Show("Вы используете не официальную версию AXIOMA!", "Ошибка");
			Environment.Exit(-1);
		}
		DragEventHandler val = _003C_003Ec__7._003C_003E9__0_0;
		if (val == null)
		{
			DragEventHandler val2 = (object obj2, DragEventArgs evt) =>
			{
				Thread thread = new Thread(() =>
				{
					HandleDragDropAsync(evt);
				});
				thread.IsBackground = true;
				thread.Start();
			};
			_003C_003Ec__7._003C_003E9__0_0 = val2;
			val = val2;
		}
		return val;
	}

	private static bool IsSharedFolderEnabled()
	{
		if (RegistryManager.Instance.DefaultGuest.FileSystem == 0)
		{
			Logger.Info("Shared folders disabled");
			return false;
		}
		return true;
	}

	private static void HandleDragDropAsync(DragEventArgs evt)
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected Obj, but got Unknown
		//IL_019b: Expected Obj, but got Unknown
		//IL_019d: Expected Obj, but got Unknown
		string vmName = MultiInstanceStrings.VmName;
		if (!IsSharedFolderEnabled())
		{
			return;
		}
		try
		{
			Array array = (Array)evt.Data.GetData(DataFormats.FileDrop);
			List<string> list = new List<string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < array.Length; i++)
			{
				string text = array.GetValue(i).ToString();
				string fileName = Path.GetFileName(text);
				if (string.Equals(Path.GetExtension(text), ".apk", StringComparison.InvariantCultureIgnoreCase) || string.Equals(Path.GetExtension(text), ".xapk", StringComparison.InvariantCultureIgnoreCase))
				{
					list.Add(text);
				}
				else
				{
					dictionary.Add(fileName, text);
				}
			}
			string sharedFolderDir = RegistryStrings.SharedFolderDir;
			if (dictionary.Count > 0)
			{
				string text2 = Utils.CreateRandomBstSharedFolder(sharedFolderDir);
				sharedFolderDir = Path.Combine(RegistryStrings.SharedFolderDir, text2);
				Logger.Info("Shared Folder path : " + sharedFolderDir);
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					Logger.Info("DragDrop File: {0}", new object[1] { item.Key });
					string text3 = Path.Combine(sharedFolderDir, item.Key);
					try
					{
						FileSystem.CopyFile(item.Value, text3, UIOption.AllDialogs);
						File.SetAttributes(text3, FileAttributes.Normal);
					}
					catch (Exception ex)
					{
						Logger.Error("Exception in copying file : " + item.Value + "... Err : " + ex.ToString());
					}
				}
				JArray val = new JArray();
				JObject val2 = new JObject();
				((JContainer)val2).Add((object)new JProperty("foldername", (object)text2));
				val.Add((JToken)val2);
				JArray val3 = val;
				Dictionary<string, string> dictionary2 = new Dictionary<string, string> { 
				{
					"data",
					((JToken)val3).ToString((Formatting)0, new JsonConverter[0])
				} };
				Logger.Info("Sending drag drop request: " + ((object)val3).ToString());
				try
				{
					HTTPUtils.SendRequestToGuest("fileDrop", dictionary2, vmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception in sending FileDrop request. Err: " + ex2.Message);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (string apkPath in list)
			{
				if (Oem.Instance.IsOEMWithBGPClient && !FeatureManager.Instance.IsCustomUIForDMM)
				{
					try
					{
						Dictionary<string, string> dictionary3 = new Dictionary<string, string> { { "filePath", apkPath } };
						HTTPUtils.SendRequestToClient("dragDropInstall", dictionary3, vmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					}
					catch (Exception ex3)
					{
						Logger.Error("Exception in send drag drop install... Err : " + ex3.ToString());
					}
				}
				else
				{
					Thread thread = new Thread(() =>
					{
						Utils.CallApkInstaller(apkPath, false, vmName);
					});
					thread.IsBackground = true;
					thread.Start();
				}
			}
		}
		catch (Exception ex4)
		{
			Logger.Error("Exception in HandleDragDropAsync function. Err : " + ex4.Message);
		}
	}

	public static void HandleDragEnter(object obj, DragEventArgs evt)
	{
		if (evt.Data.GetDataPresent(DataFormats.FileDrop))
		{
			evt.Effect = (DragDropEffects)1;
			return;
		}
		Logger.Debug("FileDrop DataFormat not supported");
		string[] formats = evt.Data.GetFormats();
		Logger.Debug("Supported formats:");
		string[] array = formats;
		for (int i = 0; i < array.Length; i++)
		{
			Logger.Debug(array[i]);
		}
		evt.Effect = (DragDropEffects)0;
	}

	internal static int ComputeImportChecksum()
	{
		return (typeof(FileImporter).Assembly.FullName.GetHashCode() ^ typeof(FileImporter).Module.Name.GetHashCode()) & 0xFF;
	}
}
