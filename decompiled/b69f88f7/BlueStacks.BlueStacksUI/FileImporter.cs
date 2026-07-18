using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using BlueStacks.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public static class FileImporter
{
	internal static void Init(MainWindow window)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		((UIElement)window).AllowDrop = true;
		((UIElement)window).DragEnter += new DragEventHandler(HandleDragEnter);
		((UIElement)window).Drop += new DragEventHandler(HandleDragDrop);
	}

	private static void HandleDragDrop(object sender, DragEventArgs e)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			HandleDragDropAsync(e, sender as MainWindow);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static bool IsSharedFolderEnabled(int fileSystem)
	{
		if (fileSystem == 0)
		{
			Logger.Info("Shared folders disabled");
			return false;
		}
		return true;
	}

	private static void HandleDragDropAsync(DragEventArgs evt, MainWindow window)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_019d: Expected O, but got Unknown
		//IL_019f: Expected O, but got Unknown
		string mVmName = window.mVmName;
		if (!IsSharedFolderEnabled(window.EngineInstanceRegistry.FileSystem))
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
						FileSystem.CopyFile(item.Value, text3, (UIOption)3);
						File.SetAttributes(text3, FileAttributes.Normal);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to copy file : " + item.Value + "...Err : " + ex.ToString());
					}
				}
				JArray val = new JArray();
				JObject val2 = new JObject();
				((JContainer)val2).Add((object)new JProperty("foldername", (object)text2));
				val.Add((JToken)val2);
				JArray val3 = val;
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("data", ((JToken)val3).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
				Dictionary<string, string> dictionary3 = dictionary2;
				Logger.Info("Sending drag drop request: " + ((object)val3).ToString());
				try
				{
					HTTPUtils.SendRequestToGuest("fileDrop", dictionary3, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex2)
				{
					Logger.Error("Failed to send FileDrop request. err: " + ex2.ToString());
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (string item2 in list)
			{
				try
				{
					Dictionary<string, string> dictionary4 = new Dictionary<string, string> { { "filePath", item2 } };
					HTTPUtils.SendRequestToClient("dragDropInstall", dictionary4, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				catch (Exception ex3)
				{
					Logger.Warning("Failed to send drag drop install. Err: " + ex3.Message);
				}
			}
		}
		catch (Exception ex4)
		{
			Logger.Error("Error in DragDrop function: " + ex4.Message);
		}
	}

	public static void HandleDragEnter(object obj, DragEventArgs evt)
	{
		if (evt == null)
		{
			return;
		}
		if (evt.Data.GetDataPresent(DataFormats.FileDrop))
		{
			evt.Effects = (DragDropEffects)1;
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
		evt.Effects = (DragDropEffects)0;
	}

	public static string GetMimeFromFile(string filename)
	{
		string result = "";
		if (!File.Exists(filename))
		{
			return result;
		}
		byte[] array = new byte[256];
		using (FileStream fileStream = new FileStream(filename, FileMode.Open))
		{
			if (fileStream.Length >= 256)
			{
				fileStream.Read(array, 0, 256);
			}
			else
			{
				fileStream.Read(array, 0, (int)fileStream.Length);
			}
		}
		try
		{
			NativeMethods.FindMimeFromData(0u, null, array, 256u, null, 0u, out var ppwzMimeOut, 0u);
			IntPtr ptr = new IntPtr(ppwzMimeOut);
			result = Marshal.PtrToStringUni(ptr);
			Marshal.FreeCoTaskMem(ptr);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to get mime type. err: " + ex.Message);
		}
		return result;
	}
}
