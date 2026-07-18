using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class HTTPHandler
{
	private static object sLockObject = new object();

	internal static string lastPackage = string.Empty;

	private static Dictionary<string, string> dictFileNamesPackageName = new Dictionary<string, string>();

	private static bool mSendGamepadStats = false;

	private static object syncRoot = new object();

	private static string mPreviousActivityReported = "";

	private static void WriteSuccessJsonArray(HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0026: Expected O, but got Unknown
		JArray val = new JArray();
		JObject val2 = new JObject();
		((JContainer)val2).Add((object)new JProperty("success", (object)true));
		val.Add((JToken)val2);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
	}

	private static void WriteErrorJsonArray(string reason, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0037: Expected O, but got Unknown
		JArray val = new JArray();
		JObject val2 = new JObject();
		((JContainer)val2).Add((object)new JProperty("success", (object)false));
		((JContainer)val2).Add((object)new JProperty("reason", (object)reason));
		val.Add((JToken)val2);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
	}

	private static void WriteErrorJSONObjectWithoutReason(HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		JObject val = new JObject();
		val.Add("success", JToken.op_Implicit(false));
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
	}

	public static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		WriteSuccessJsonWithVmName(HTTPUtils.ParseRequest(req).RequestVmName, res);
	}

	internal static void EnableWndProcLogging(HttpListenerRequest _1, HttpListenerResponse _2)
	{
		try
		{
			WindowWndProcHandler.isLogWndProc = !WindowWndProcHandler.isLogWndProc;
			Logger.Info("Got request for EnableWndProcLogging" + WindowWndProcHandler.isLogWndProc);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set EnableWndProcLogging... Err : " + ex.ToString());
		}
	}

	internal static void EnableKeyboardHookLogging(HttpListenerRequest _1, HttpListenerResponse _2)
	{
		try
		{
			GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging = !GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set EnableKeyboardHookLogging... Err : " + ex.ToString());
		}
	}

	internal static void EnableDebugLogs(HttpListenerRequest _1, HttpListenerResponse res)
	{
		try
		{
			Logger.EnableDebugLogs();
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	public static void SendAppDisplayed(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			JObject val2 = new JObject();
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				val2.Add("success", JToken.op_Implicit(BlueStacksUIUtils.DictWindows[val.RequestVmName].mAppHandler.mAppDisplayedOccured));
			}
			HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server SendAppDisplayed. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void RestartFrontend(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].RestartFrontend();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server RestartFrontend. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void GCCollect(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData obj = HTTPUtils.ParseRequest(req);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			WriteSuccessJsonWithVmName(obj.RequestVmName, res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server GCCollect. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	public static void IsBlueStacksUIVisible(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			JObject val2 = new JObject();
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				val2.Add("success", JToken.op_Implicit(((UIElement)BlueStacksUIUtils.DictWindows[val.RequestVmName]).IsVisible));
			}
			HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server IsBlueStacksUIVisible. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void ToggleFarmMode(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData obj = HTTPUtils.ParseRequest(req);
			CommonHandlers.ToggleFarmMode(bool.Parse(obj.Data["state"]));
			WriteSuccessJsonWithVmName(obj.RequestVmName, res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ToggleFarmMode. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void ToggleStreamingMode(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			bool state = bool.Parse(val.Data["state"]);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				MainWindow mWindow = BlueStacksUIUtils.DictWindows[val.RequestVmName];
				((DispatcherObject)mWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					mWindow.mTopBar.mPreferenceDropDownControl.ToggleStreamingMode(state);
					mWindow.mFrontendHandler.ToggleStreamingMode(state);
				}, new object[0]);
			}
			WriteSuccessJsonWithVmName(val.RequestVmName, res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ToggleStreamingMode. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void GamepadGuidanceButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				MainWindow mWindow = BlueStacksUIUtils.DictWindows[val.RequestVmName];
				if (mWindow != BlueStacksUIUtils.LastActivatedWindow || (RegistryManager.Instance.GamepadDetectionEnabled && mWindow.IsGamepadConnected && mWindow.mTopBar.mAppTabButtons.SelectedTab.mIsNativeGamepadEnabledForApp))
				{
					return;
				}
				((DispatcherObject)mWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					if (KMManager.CheckIfKeymappingWindowVisible(checkForGuidanceWindow: true))
					{
						KMManager.CloseWindows();
						mWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
					}
					else if (!KeymapCanvasWindow.sIsDirty)
					{
						KMManager.HandleInputMapperWindow(mWindow, "gamepad");
					}
				}, new object[0]);
			}
			WriteSuccessJsonWithVmName(val.RequestVmName, res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GamepadGuidanceButtonHandler. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void SetCurrentVolumeFromAndroidHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData obj = HTTPUtils.ParseRequest(req);
			string requestVmName = obj.RequestVmName;
			int volumeLevelFromAndroid = Convert.ToInt32(obj.Data["volume"], CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				BlueStacksUIUtils.DictWindows[requestVmName].Utils.SetVolumeLevelFromAndroid(volumeLevelFromAndroid);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to set volume level. Er : " + ex.ToString());
		}
	}

	internal static void ReinitRegistry(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RegistryManager.ClearRegistryMangerInstance();
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to reinit registry. Err : " + ex.ToString());
		}
	}

	internal static void UpdateCrc(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string requestVmName = requestData.RequestVmName;
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				if (uint.TryParse(requestData.Data["Crc"], out var result) && float.TryParse(requestData.Data["X"], out var result2) && float.TryParse(requestData.Data["Y"], out var result3))
				{
					string text = string.Format(CultureInfo.InvariantCulture, "X: {0}   Y: {1}   Crc: {2}", new object[3]
					{
						result2.ToString(CultureInfo.InvariantCulture),
						result3.ToString(CultureInfo.InvariantCulture),
						result.ToString("X", CultureInfo.InvariantCulture)
					});
					Logger.Info("IMAGEPICKER: " + text);
					MessageBox.Show(text);
					Clipboard.SetText(text);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
		}
	}

	internal static void ConfigFileChanged(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			string config = val.Data["config"];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					window.mTopBar.SetConfigIndicator(config);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to set GameInfo err : " + ex.ToString());
		}
	}

	internal static void CheckCallbackEnabledStatus(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		JArray val = new JArray();
		JObject val2 = new JObject();
		try
		{
			string text = HTTPUtils.ParseRequest(req).Data["vmname"];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
			{
				MainWindow mainWindow = BlueStacksUIUtils.DictWindows[text];
				Logger.Info("Callback: vmname: " + text + " value: " + mainWindow.mCallbackEnabled);
				val2.Add("success", JToken.op_Implicit(true));
				val2.Add("Enabled", JToken.op_Implicit(mainWindow.mCallbackEnabled));
				val.Add((JToken)(object)val2);
				HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to get callback status. Err : " + ex.Message);
			val2.Add("success", JToken.op_Implicit(false));
			val2.Add("status", JToken.op_Implicit(ex.Message));
			val.Add((JToken)(object)val2);
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
	}

	internal static void AddNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.Data["vmname"];
			string text = val.Data["pkg"];
			string text2 = val.Data["app_name"];
			string message = val.Data["msg"];
			string id = val.Data["id"];
			string text3 = "bluestackslogo";
			string value = text3;
			JsonParser val2 = new JsonParser(vmName);
			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					AppInfo appInfoFromPackageName = val2.GetAppInfoFromPackageName(text);
					if (appInfoFromPackageName != null)
					{
						if (File.Exists(Path.Combine(RegistryStrings.GadgetDir, appInfoFromPackageName.Img)))
						{
							text3 = Path.Combine(RegistryStrings.GadgetDir, appInfoFromPackageName.Img);
							value = appInfoFromPackageName.Img;
						}
					}
					else
					{
						Logger.Info("GetAppInfoFromAppName returns false");
					}
				}
			}
			catch
			{
				Logger.Error("Error loading app icon file");
			}
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				_ = BlueStacksUIUtils.DictWindows[vmName];
				GenericNotificationItem val3 = new GenericNotificationItem
				{
					Title = text2,
					Message = message,
					ShowRibbon = true,
					NotificationMenuImageUrl = text3,
					NotificationMenuImageName = text3,
					IsAndroidNotification = true,
					Id = id,
					VmName = vmName,
					Package = text
				};
				if (text == null)
				{
					text = Strings.ProductDisplayName;
				}
				if (string.Equals(text2, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
				{
					if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(Strings.ProductDisplayName))
					{
						BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM[Strings.ProductDisplayName]++;
					}
					else
					{
						BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(Strings.ProductDisplayName, 1);
					}
				}
				else if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(text))
				{
					BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM[text]++;
				}
				else
				{
					BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(text, 1);
				}
				GenericNotificationManager.AddNewNotification(val3);
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_006f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0075: Invalid comparison between Unknown and I4
					BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
					SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
					BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
					if ((int)((Window)BlueStacksUIUtils.DictWindows[vmName]).WindowState == 1)
					{
						BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
					}
				}, new object[0]);
			}
			if (RegistryManager.Instance.IsNotificationSoundsActive && BlueStacksUIUtils.DictWindows[vmName].StaticComponents.mSelectedTabButton.mTabType != TabType.AppTab)
			{
				MediaPlayer val4 = new MediaPlayer();
				val4.Open(new Uri(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "NotificationSound.wav")));
				val4.Play();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
		}
	}

	internal static void MarkNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.Data["vmname"];
			string item = val.Data["id"];
			GenericNotificationManager.MarkNotification(new List<string> { item }, delegate(GenericNotificationItem x)
			{
				x.IsRead = true;
			});
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Invalid comparison between Unknown and I4
				BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
				SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
				BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
				if ((int)((Window)BlueStacksUIUtils.DictWindows[vmName]).WindowState == 1)
				{
					BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Error in marking notification read: " + ex);
		}
	}

	internal static void NCSetGameInfoOnTopBarHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			string gameName = val.Data["game"];
			string characterName = val.Data["character"];
			MainWindow mWindow = BlueStacksUIUtils.DictWindows[requestVmName];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				((DispatcherObject)mWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					mWindow.mNCTopBar.mAppName.Text = gameName;
					((FrameworkElement)mWindow.mNCTopBar.mAppName).ToolTip = gameName;
					((UIElement)mWindow.mNCTopBar.mGamenameSeparator).Visibility = (Visibility)0;
					mWindow.mNCTopBar.mCharacterName.Text = characterName;
					((FrameworkElement)mWindow.mNCTopBar.mCharacterName).ToolTip = characterName;
				}, new object[0]);
				WriteSuccessJsonWithVmName(requestVmName, res);
			}
			else
			{
				WriteErrorJsonArray("Client Instance not running", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to set GameInfo err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void OpenCFGReorderTool(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			((DispatcherObject)BlueStacksUIUtils.DictWindows.Values.ToList()[0]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((Window)CFGReorderWindow.Instance).Show();
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't open cfg reorder window. Ex: {0}", new object[1] { ex });
		}
	}

	internal static void OpenThemeEditor(HttpListenerRequest _1, HttpListenerResponse _2)
	{
		try
		{
			if (RegistryManager.Instance.OpenThemeEditor)
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows.Values.ToList()[0]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((Window)ThemeEditorWindow.Instance).Show();
				}, new object[0]);
			}
		}
		catch (Exception)
		{
		}
	}

	internal static void MuteAllInstancesHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		bool flag = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["muteInstance"], CultureInfo.InvariantCulture);
		foreach (MainWindow value in BlueStacksUIUtils.DictWindows.Values)
		{
			if (flag)
			{
				value.Utils.MuteApplication(allInstances: true);
			}
			else
			{
				value.Utils.UnmuteApplication(allInstances: true);
			}
		}
	}

	internal static void AccountSetupCompleted(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName) && FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				NCSoftUtils.Instance.SendGoogleLoginEventAsync(requestVmName);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AccountSetupCompleted Handler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void GetHeightWidth(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && BlueStacksUIUtils.DictWindows[vmName] != null)
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Expected O, but got Unknown
					//IL_0032: Expected O, but got Unknown
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					//IL_0052: Expected O, but got Unknown
					//IL_0052: Unknown result type (might be due to invalid IL or missing references)
					//IL_0063: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Expected O, but got Unknown
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Expected O, but got Unknown
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ad: Expected O, but got Unknown
					//IL_00ae: Expected O, but got Unknown
					//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cc: Expected O, but got Unknown
					//IL_00d1: Expected O, but got Unknown
					try
					{
						MainWindow mainWindow = BlueStacksUIUtils.DictWindows[vmName];
						JArray val2 = new JArray();
						JObject val3 = new JObject();
						((JContainer)val3).Add((object)new JProperty("success", (object)true));
						JObject val4 = val3;
						JObject val5 = new JObject();
						((JContainer)val5).Add((object)new JProperty("cHeight", (object)((FrameworkElement)mainWindow).ActualHeight));
						((JContainer)val5).Add((object)new JProperty("cWidth", (object)((FrameworkElement)mainWindow).ActualWidth));
						((JContainer)val5).Add((object)new JProperty("gHeight", (object)((FrameworkElement)mainWindow.mContentGrid).ActualHeight));
						((JContainer)val5).Add((object)new JProperty("gWidth", (object)((FrameworkElement)mainWindow.mContentGrid).ActualWidth));
						JObject val6 = val5;
						val2.Add((JToken)(object)val4);
						JObject val7 = new JObject();
						((JContainer)val7).Add((object)new JProperty("result", (object)val6));
						val2.Add((JToken)val7);
						HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
					}
					catch (Exception ex2)
					{
						Logger.Error("Some error in finding MainWindow instance err: " + ex2.ToString());
						WriteErrorJsonArray(ex2.Message, res);
					}
				}, new object[0]);
			}
			else
			{
				WriteErrorJsonArray("Client Instance not running", res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to GetHeightWidth err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void ScreenLock(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			bool lockScreen = Convert.ToBoolean(val.Data["lock"], CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					if (lockScreen)
					{
						BlueStacksUIUtils.DictWindows[vmName].ShowLockScreen();
					}
					else
					{
						BlueStacksUIUtils.DictWindows[vmName].HideLockScreen();
					}
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to lock screen err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void SetStreamingStatus(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			string status = val.Data["status"];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.SetNcSoftStreamingStatus(status);
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to SetStreamingStatus err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void PlayerScriptModifierKeyUp(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			double x = Convert.ToDouble(val.Data["X"], CultureInfo.InvariantCulture);
			double y = Convert.ToDouble(val.Data["Y"], CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.AddCoordinatesToScriptText(x, y);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to handle player script modifier key up: " + ex.ToString());
		}
	}

	internal static void LaunchPlay(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData obj = HTTPUtils.ParseRequest(req);
			string key = obj.Data["vmname"];
			string package = obj.Data["package"];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(key))
			{
				BlueStacksUIUtils.DictWindows[key].Utils.HandleLaunchPlay(package);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to launch play store err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void FullScreenSidebarHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			bool isVisible = Convert.ToBoolean(val.Data["visible"], CultureInfo.InvariantCulture);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				return;
			}
			MainWindow window = BlueStacksUIUtils.ActivatedWindow;
			if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
			{
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					window.mSidebar.ToggleSidebarVisibilityInFullscreen(isVisible);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in FullScreenSidebarHandler : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void HideTopSideBarHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
		if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
		{
			return;
		}
		MainWindow window = BlueStacksUIUtils.ActivatedWindow;
		if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
		{
			((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				window.mSidebar.HideSideBarInFullscreen();
				window.mTopbarOptions.HideTopBarInFullscreen();
			}, new object[0]);
		}
	}

	internal static void FullScreenTopbarButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			bool isVisible = Convert.ToBoolean(val.Data["visible"], CultureInfo.InvariantCulture);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				return;
			}
			MainWindow window = BlueStacksUIUtils.ActivatedWindow;
			if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
			{
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					window.mTopbarOptions.ToggleTopbarButtonVisibilityInFullscreen(isVisible);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in FullScreenTopbarButtonHandler : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void FullScreenSidebarButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			bool isVisible = Convert.ToBoolean(val.Data["visible"], CultureInfo.InvariantCulture);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				return;
			}
			MainWindow window = BlueStacksUIUtils.ActivatedWindow;
			if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
			{
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					window.mSidebar.ToggleSidebarButtonVisibilityInFullscreen(isVisible);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Errro in FullScreenSidebarButtonHandler : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void FullScreenTopbarHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			bool isVisible = Convert.ToBoolean(val.Data["visible"], CultureInfo.InvariantCulture);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				return;
			}
			MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
			if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
			{
				return;
			}
			((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (!((Popup)window.mTopBarPopup).IsOpen && isVisible)
				{
					((Popup)window.mTopBarPopup).IsOpen = true;
				}
				else
				{
					((Popup)window.mTopBarPopup).IsOpen = false;
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Error FullScreenTopbarHandler : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void HandleGamepadConnection(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			bool isGamepadConnected = bool.Parse(val.Data["status"]);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].IsGamepadConnected = isGamepadConnected;
				if (!mSendGamepadStats)
				{
					ClientStats.SendMiscellaneousStatsAsync("GamePadConnectedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, null);
					mSendGamepadStats = true;
				}
			}
			WriteSuccessJsonWithVmName(val.RequestVmName, res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleGamepadConnection. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void TileWindow(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			HTTPUtils.ParseRequest(req);
			CommonHandlers.ArrangeWindowInTiles();
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in tiling window. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void CascadeWindow(HttpListenerRequest _, HttpListenerResponse res)
	{
		try
		{
			CommonHandlers.ArrangeWindowInCascade();
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Cascading window. Err : " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void UpdateLocale(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got UpdateLocale {0} request from {1}", new object[2]
		{
			req.HttpMethod,
			req.RemoteEndPoint.ToString()
		});
		try
		{
			RequestData obj = HTTPUtils.ParseRequest(req);
			string requestVmName = obj.RequestVmName;
			string text = obj.Data["locale"].ToString(CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				RegistryManager.Instance.UserSelectedLocale = text;
				Utils.UpdateValueInBootParams("LANG", text, requestVmName, false, "bgp64");
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					LocaleStrings.InitLocalization((string)null, "Android", false);
				}, new object[0]);
				HTTPUtils.SendRequestToAgentAsync("reinitlocalization", (Dictionary<string, string>)null, "Android", 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UpdateLocale: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void ScreenshotCaptured(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got ScreenshotCaptured {0} request from {1}", new object[2]
		{
			req.HttpMethod,
			req.RemoteEndPoint.ToString()
		});
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			string path = val.Data["path"].ToString(CultureInfo.InvariantCulture);
			string text = val.Data["showSavedInfo"];
			if (text == null)
			{
				text = "false";
			}
			bool.TryParse(text, out var showSaved);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.PostScreenShotWork(path, showSaved);
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ScreenshotCaptured: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void ClientHotkeyHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			string value = val.Data["keyevent"].ToString(CultureInfo.InvariantCulture);
			ClientHotKeys clientHotKey = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), value);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					BlueStacksUIUtils.DictWindows[vmName].HandleClientHotKey(clientHotKey);
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ClientHotkeyHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void AndroidLocaleChanged(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			BlueStacksUIUtils.UpdateLocale(RegistryManager.Instance.UserSelectedLocale, val.RequestVmName);
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AndroidLocaleChanged. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void HandleClientOperation(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				string operationString = val.Data["data"];
				BlueStacksUIUtils.DictWindows[val.RequestVmName].mCommonHandler.HandleClientOperation(operationString);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleClientOperation. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void MacroPlaybackCompleteHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].SetMacroPlayBackEventHandle();
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in MacroPlaybackCompleteHandler. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void HandleClientGamepadButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				string text = requestData.Data["data"];
				if (bool.TryParse(requestData.Data["isDown"], out var result))
				{
					KMManager.UpdateUIForGamepadEvent(text, result);
				}
				else
				{
					Logger.Error("Error in HandleClientGamepadButtonHandler: Could not parse gamepad event isDown:'{0}'", new object[1] { requestData.Data["isDown"] });
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleClientGamepadButtonHandler. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void SaveComboEvents(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string events = val.Data["events"];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				if (BlueStacksUIUtils.DictWindows[val.RequestVmName].mIsMacroRecorderActive)
				{
					BlueStacksUIUtils.DictWindows[val.RequestVmName].MacroRecorderWindow.SaveOperation(events);
				}
				else
				{
					((DispatcherObject)BlueStacksUIUtils.DictWindows[val.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						KMManager.mComboEvents = events;
					}, new object[0]);
				}
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SaveComboEvents. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void MacroCompleted(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].MacroOverlayControl.ShowPromptAndHideOverlay();
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in MacroCompleted. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void ShowMaintenanceWarning(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			string message = val.Data["message"];
			WriteJSON(new Dictionary<string, string> { { "result_code", "0" } }, res);
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				CustomMessageWindow val2 = new CustomMessageWindow();
				val2.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("BlueStacks", "") + " " + LocaleStrings.GetLocalizedString("STRING_WARNING", "");
				val2.BodyTextBlock.Text = message;
				val2.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler)null, (string)null, false, (object)null);
				((Window)val2).Owner = (Window)(object)BlueStacksUIUtils.DictWindows[vmName];
				BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay();
				((Window)val2).ShowDialog();
				BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to ShowMaintenanceWarning app... Err : " + ex.ToString());
			WriteJSON(new Dictionary<string, string> { { "result_code", "-1" } }, res);
		}
	}

	internal static void WriteJSON(Dictionary<string, string> data, HttpListenerResponse res)
	{
		HTTPUtils.Write(JSONUtils.GetJSONArrayString(data), res);
	}

	internal static void LaunchDefaultWebApp(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string text = val.Data["action"];
			Logger.Info("Action : " + text);
			switch (text)
			{
			case "browser":
			{
				string text9 = val.Data["url"];
				Logger.Info("Url : " + text9);
				try
				{
					Process.Start(text9);
					WriteSuccessJsonArray(res);
					break;
				}
				catch
				{
					WriteErrorJsonArray("Invalid or empty url", res);
					break;
				}
			}
			case "email":
			{
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				string text7 = "";
				string text8 = "";
				try
				{
					text2 = val.Data["to"];
					text3 = val.Data["cc"];
					text4 = val.Data["bcc"];
					text5 = val.Data["message"];
					text6 = val.Data["subject"];
					text7 = val.Data["mailto"];
				}
				catch
				{
				}
				bool flag = false;
				if (!string.IsNullOrEmpty(text2))
				{
					flag = text2.Split(new char[1] { '@' }).Length > 1;
				}
				Logger.Info("to : " + text2 + ", cc : " + text3 + ", bcc : " + text4 + ", subject = " + text6);
				Logger.Info("mailto : " + text7);
				if (flag)
				{
					text8 = "mailto:" + text2 + "?cc=" + text3 + "&bcc=" + text4 + "&subject=" + text6 + "&body=" + text5;
				}
				else
				{
					if (string.IsNullOrEmpty(text7))
					{
						WriteErrorJsonArray("to and mailto field cannot be empty", res);
						break;
					}
					text8 = text7;
				}
				Logger.Info("mail to request : " + text8);
				Process.Start(text8);
				WriteSuccessJsonArray(res);
				break;
			}
			default:
				WriteErrorJsonArray("wrong or empty action", res);
				break;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to LaunchDefaultWebApp app... Err : " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	public static void GetRunningInstances(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			List<string> list = new List<string>(BlueStacksUIUtils.DictWindows.Keys);
			HTTPUtils.ParseRequest(req);
			JObject val = new JObject();
			string text = string.Join(",", list.ToArray());
			Logger.Info("Running instances: " + text);
			val.Add("success", JToken.op_Implicit(true));
			val.Add("vmname", JToken.op_Implicit(text));
			HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetRunningInstances");
			Logger.Error(ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void IsAnyAppRunning(HttpListenerRequest _1, HttpListenerResponse res)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		try
		{
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			JObject val2 = val;
			bool isAppRunning = false;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList()[0]];
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					isAppRunning = window.mTopBar.mAppTabButtons.IsAppRunning();
				}, new object[0]);
				val2.Add("isanyapprunning", JToken.op_Implicit(isAppRunning));
			}
			HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in IsAnyAppRunning: {0}", new object[1] { ex });
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void GetCurrentAppDetails(HttpListenerRequest _1, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		try
		{
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			JObject val2 = val;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList()[0]];
				string pkg = string.Empty;
				string appName = string.Empty;
				string tabType = string.Empty;
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					pkg = window.mTopBar.mAppTabButtons.SelectedTab.PackageName;
					appName = (string)((ContentControl)window.mTopBar.mAppTabButtons.SelectedTab.mTabLabel).Content;
					tabType = window.mTopBar.mAppTabButtons.SelectedTab.mTabType.ToString();
				}, new object[0]);
				val2.Add("pkgname", JToken.op_Implicit(pkg));
				val2.Add("appname", JToken.op_Implicit(appName));
				val2.Add("tabtype", JToken.op_Implicit(tabType));
			}
			HTTPUtils.Write(((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetCurrentAppDetails: {0}", new object[1] { ex });
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void ShowSettingWindow(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[val.RequestVmName];
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					MainWindow.OpenSettingsWindow(window, "STRING_NOTIFICATION");
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowSettingWindow: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void LaunchWebTab(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					window.mTopBar.mAppTabButtons.AddWebTab(requestData.Data["url"], requestData.Data["name"], requestData.Data["image"], isSwitch: true);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void OneTimeSetupCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			MainWindow mainWindow = BlueStacksUIUtils.DictWindows[val.RequestVmName];
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "OTS Completed", "OTS Completed", null, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
				mainWindow.mAppHandler.IsOneTimeSetupCompleted = true;
				ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "OTS Completed", "OTS Completed", RegistryManager.Instance.InstallID, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void AppJsonChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.InitIcons();
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AppjsonChangedHabdler " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void StartInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
			Logger.Info("start instance vm name :" + requestVmName);
			RegistryManager.ClearRegistryMangerInstance();
			BlueStacksUIUtils.RunInstance(requestVmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server StartInstanceHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void StopInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				BlueStacksUIUtils.DictWindows[requestVmName].ForceCloseWindow();
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server StopInstanceHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void MinimizeInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].MinimizeWindow();
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server MinimizeInstanceHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void HideBluestacksHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			HTTPUtils.ParseRequest(req);
			Logger.Info("Hide Bluestacks received");
			BlueStacksUIUtils.HideUnhideBlueStacks(isHide: true);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server HideBluestacksHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void OpenOrInstallPackageHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			string text = val.Data["json"].ToString(CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(text))
			{
				JObject val2 = JObject.Parse(text);
				if (val2 != null && val2["campaign_id"] != null)
				{
					RegistryManager.Instance.ClientLaunchParams = text;
				}
			}
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				ShowWindowHandler(req, res);
				if (!string.IsNullOrEmpty(text))
				{
					if (BlueStacksUIUtils.DictWindows[val.RequestVmName].mAppHandler.IsOneTimeSetupCompleted)
					{
						BlueStacksUIUtils.DictWindows[requestVmName].PublishForFlePopupToBrowser(text);
						new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).DownloadAndInstallAppFromJson(text);
					}
					else
					{
						Opt.Instance.Json = text;
					}
				}
			}
			else
			{
				Opt.Instance.Json = text;
				ShowWindowHandler(req, res);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in OpenOrInstallPackageHandler. Err : " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void GuestBootCompleted(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].mAppHandler.IsGuestReady = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in server GuestBootCompleted: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void AppDisplayedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					val.Data[text]
				});
			}
			string text2 = val.Data["packageName"];
			_ = val.Data["appDisplayed"];
			lock (sLockObject)
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[val.RequestVmName];
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						mainWindow.mAppHandler.HandleAppDisplayed(text2);
					}
					if (!mainWindow.EngineInstanceRegistry.IsOneTimeSetupDone && text2 != "com.bluestacks.appmart" && !mainWindow.mGuestBootCompleted)
					{
						int num = 20;
						while (!mainWindow.mAppHandler.IsGuestReady && num > 0)
						{
							num--;
							Thread.Sleep(1000);
						}
						if (text2 == mainWindow.mAppHandler.GetDefaultLauncher())
						{
							if (!FeatureManager.Instance.IsCustomUIForNCSoft)
							{
								Logger.Info("BOOT_STAGE: Calling guestboot_completed from AppDisplayedHandler");
								mainWindow.mAppHandler.IsGuestReady = true;
							}
							else
							{
								mainWindow.Utils.sBootCheckTimer.Enabled = false;
								mainWindow.mEnableLaunchPlayForNCSoft = true;
							}
						}
					}
				}
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppDisplayedHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void AppLaunchedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					val.Data[text]
				});
			}
			string text2 = val.Data["package"];
			string text3 = val.Data["activity"];
			string text4 = val.Data["callingPackage"];
			Logger.Info("Package: {0}, activity: {1}, callingPackage: {2}", new object[3] { text2, text3, text4 });
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				MainWindow mainWindow = BlueStacksUIUtils.DictWindows[val.RequestVmName];
				if (!RegistryManager.Instance.Guest[val.RequestVmName].IsGoogleSigninDone)
				{
					lock (syncRoot)
					{
						if (string.Compare(mPreviousActivityReported.Replace("/", ""), text3.Replace("/", ""), StringComparison.OrdinalIgnoreCase) != 0)
						{
							mPreviousActivityReported = text3;
							ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, text2, text3, RegistryManager.Instance.InstallID, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
						}
					}
				}
				if (FeatureManager.Instance.IsCustomUIForNCSoft && !mainWindow.mGuestBootCompleted && !text2.Equals("com.bluestacks.appmart", StringComparison.OrdinalIgnoreCase) && !text2.Equals("com.android.provision", StringComparison.OrdinalIgnoreCase))
				{
					mainWindow.mAppHandler.IsGuestReady = true;
				}
				if (mainWindow.mGuestBootCompleted && PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.IgnoredActivitiesForTabs.Contains<string>(text3, StringComparer.InvariantCultureIgnoreCase) != true && PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.IgnoredActivitiesForTabs.Contains<string>(StringExtensions.TrimStart(text3, text2 + "/"), StringComparer.InvariantCultureIgnoreCase) != true)
				{
					mainWindow.mAppHandler.AppLaunched(text2);
				}
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppLaunchedHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void AppCrashedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					val.Data[text]
				});
			}
			string vmName = val.RequestVmName;
			string package = val.Data["package"];
			Logger.Info("package: " + package);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.CloseTab("app:" + package, sendStopAppToAndroid: false, forceClose: false, dontCheckQuitPopup: true);
				}, new object[0]);
				if (FeatureManager.Instance.IsCustomUIForNCSoft && !NCSoftUtils.Instance.BlackListedApps.Any((string pkg) => package.StartsWith(pkg, StringComparison.InvariantCulture)))
				{
					NCSoftUtils.Instance.SendAppCrashEvent("check android logs", vmName);
				}
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppCrashedHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	internal static void AppInfoUpdated(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string package = val.Data["packageName"];
			if (!string.IsNullOrEmpty(val.Data["macro"]) && val.Data["macro"].Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				string[] allKeys = val.Data.AllKeys;
				foreach (string text in allKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[2]
					{
						text,
						val.Data[text]
					});
				}
				((DispatcherObject)BlueStacksUIUtils.DictWindows[val.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
					{
						if (dictWindow.Value.mWelcomeTab.mHomeAppManager.GetAppIcon(package) != null && dictWindow.Value.mWelcomeTab.mHomeAppManager.GetMacroAppIcon(package) == null)
						{
							dictWindow.Value.mWelcomeTab.mHomeAppManager.AddMacroAppIcon(package);
						}
					}
				}, new object[0]);
				WriteSuccessJsonArray(res);
			}
			if (!string.IsNullOrEmpty(val.Data["videoPresent"]) && BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				string text2 = "false";
				text2 = val.Data["videoPresent"].ToString(CultureInfo.InvariantCulture);
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "packageName", package },
					{ "videoPresent", text2 }
				};
				HTTPUtils.SendRequestToAgentAsync("appJsonUpdatedForVideo", dictionary, val.RequestVmName, 0, (Dictionary<string, string>)null, true, 1, 0, "bgp64");
			}
			KMManager.ControlSchemesHandlingWhileCfgUpdateFromCloud(package);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppInfoDownload: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void CloseTabHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string package = requestData.Data["package"];
			Logger.Info("package: " + package);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					try
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.CloseTab(package, sendStopAppToAndroid: false, forceClose: false, dontCheckQuitPopup: true);
					}
					catch (Exception ex2)
					{
						Logger.Error("Exception in closing tab. Err : ", new object[1] { ex2.ToString() });
					}
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server TabCloseHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void ShowAppHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string package = requestData.Data["package"];
			string activity = requestData.Data["activity"];
			string text2 = requestData.Data["title"];
			Logger.Info("package: " + package);
			Logger.Info("activity: " + activity);
			Logger.Info("title : " + text2);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (!string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(activity))
						{
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.SendRunAppRequestAsync(package, activity);
						}
					}, new object[0]);
				});
				thread.IsBackground = true;
				thread.Start();
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowAppHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void ShowWindowHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string text = val.RequestVmName;
			if (Enumerable.Contains<string>(val.Data.AllKeys, "showNotifications"))
			{
				MainWindow.sShowNotifications = Convert.ToBoolean(val.Data["showNotifications"], CultureInfo.InvariantCulture);
			}
			if (Enumerable.Contains<string>(val.Data.AllKeys, "all"))
			{
				foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
				{
					dictWindow.Value.ShowWindow();
				}
			}
			else
			{
				if (Enumerable.Contains<string>(val.Data.AllKeys, "vmname"))
				{
					text = val.Data["vmname"];
				}
				bool flag = val.Data["hidden"] != null && Convert.ToBoolean(val.Data["hidden"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					if (!flag)
					{
						BlueStacksUIUtils.DictWindows[text].ShowWindow();
					}
				}
				else
				{
					RegistryManager.ClearRegistryMangerInstance();
					BlueStacksUIUtils.RunInstance(text, flag);
				}
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowWindowHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void ShowWindowAndAppHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[val.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ShowWindowHandler(req, res);
					ShowAppHandler(req, res);
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowWindowAndAppHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void IsVisibleHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				if (((UIElement)BlueStacksUIUtils.DictWindows[val.RequestVmName]).IsVisible)
				{
					WriteSuccessJsonArray(res);
				}
				else
				{
					WriteErrorJsonArray("unused", res);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server IsVisibleHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void AppUninstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string package = requestData.Data["package"];
			string text2 = requestData.Data["name"];
			Logger.Info("package: " + package);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppUninstalled(package);
				}, new object[0]);
			}
			NotificationManager.Instance.RemoveNotificationItem(text2, package);
			Publisher.PublishMessage((BrowserControlTags)6, requestData.RequestVmName, new JObject((object)new JProperty("PackageName", (object)package)));
			ClientStats.SendClientStatsAsync("uninstall", "success", "app_install", package);
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppUninstalledHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void AppInstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Info("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string package = requestData.Data["package"];
			bool isUpdate = false;
			if (Enumerable.Contains<string>(requestData.Data.AllKeys, "isUpdate"))
			{
				isUpdate = string.Equals(requestData.Data["isUpdate"], "true", StringComparison.InvariantCultureIgnoreCase);
			}
			Logger.Info("package: " + package + ", isUpdate: " + isUpdate);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppInstalled(package, isUpdate);
				}, new object[0]);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server AppInstalledHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.ToString(), res);
		}
	}

	public static void ShowHomeTabHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		RequestData requestData = HTTPUtils.ParseRequest(req);
		if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
		{
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				Logger.Info("Switching to Welcome tab");
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.GoToTab("Home");
			}, new object[0]);
		}
	}

	public static void ShowWebPageHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string title = requestData.Data["title"].ToString(CultureInfo.InvariantCulture);
			string webUrl = requestData.Data["url"].ToString(CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ShowWindowHandler(req, res);
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab(webUrl, title, "cef_tab", isSwitch: true);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server ShowWebPageHandler : " + ex.ToString());
		}
	}

	public static void ForceQuitHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		Logger.Info("Quiting BlueStacksUI");
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			bool flag = false;
			try
			{
				flag = Convert.ToBoolean(val.Data["softclose"], CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				if (flag)
				{
					BlueStacksUIUtils.DictWindows[val.RequestVmName].CloseWindow();
				}
				((DispatcherObject)BlueStacksUIUtils.DictWindows[val.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					App.ExitApplication();
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ForceQuit... Err : " + ex.ToString());
		}
	}

	public static void OpenGoogleHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		RequestData requestData = HTTPUtils.ParseRequest(req);
		string tabName = "tab_" + (new Random().Next(100) + 1);
		if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
		{
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab("http://www.google.com", tabName, "cef_tab", isSwitch: true);
			}, new object[0]);
		}
	}

	private static void WriteSuccessJsonWithVmName(string vmName, HttpListenerResponse res)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0037: Expected O, but got Unknown
		JArray val = new JArray();
		JObject val2 = new JObject();
		((JContainer)val2).Add((object)new JProperty("success", (object)true));
		((JContainer)val2).Add((object)new JProperty("vmname", (object)vmName));
		val.Add((JToken)val2);
		HTTPUtils.Write(((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]), res);
	}

	public static void UnsupportedCPUError(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string reason = requestData.Data["PlusFailureReason"];
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Invalid comparison between Unknown and I4
				string localizedString = LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT_CAPTION", "");
				if ((int)MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT", ""), localizedString, (MessageBoxButtons)0) == 1)
				{
					Logger.Info("Quit BlueStacksUI End with reason {0}", new object[1] { reason });
					WriteSuccessJsonArray(res);
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].ForceCloseWindow();
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in QuitBlueStacksUI: " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	public static void UpdateUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string text = requestData.Data["result"].Trim();
			if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow != null)
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow.ChangeBasedonTokenReceived(text);
			}
			if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase) && BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
				}, new object[0]);
				PromotionManager.CheckIsUserPremium();
				PromotionObject.AppRecommendationHandler?.Invoke(obj: false);
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mLaunchStartupTabWhenTokenReceived && ((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab).Count > 0)
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab, "startup_action");
					}
				}, new object[0]);
				Publisher.PublishMessage((BrowserControlTags)16, requestData.RequestVmName, (JObject)null);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UpdateUserInfoHandler: " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void AppInstallStarted(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Info("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string apkPath = requestData.Data["filePath"];
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			string package = string.Empty;
			string appName = string.Empty;
			DownloadInstallApk downloader = new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]);
			if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
			{
				JToken val = Utils.ExtractInfoFromXapk(apkPath);
				if (val != null)
				{
					package = JsonExtensions.GetValue(val, "package_name");
					appName = JsonExtensions.GetValue(val, "name");
					Logger.Debug("Package name from manifest.json.." + package);
				}
			}
			else
			{
				AppInfoExtractor apkInfo = AppInfoExtractor.GetApkInfo(apkPath);
				appName = apkInfo.AppName;
				package = apkInfo.PackageName;
			}
			dictFileNamesPackageName[apkPath] = package;
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, downloader);
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(package, apkPath);
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetUserInfo: " + ex.ToString());
		}
	}

	public static void AppInstallFailed(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string[] allKeys = val.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Debug("Key: {0}, Value: {1}", new object[2]
				{
					text,
					val.Data[text]
				});
			}
			string apkPath = val.Data["filePath"];
			int errorCode = Convert.ToInt32(val.Data["errorCode"], CultureInfo.InvariantCulture);
			string vmName = val.RequestVmName;
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				try
				{
					BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.ApkInstallFailed(dictFileNamesPackageName[apkPath]);
					ShowErrorPromptIfNeeded(vmName, errorCode);
				}
				catch (Exception ex2)
				{
					Logger.Error("error in install failed http call: {0}", new object[1] { ex2 });
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in AppInstallFailed. Err : " + ex.ToString());
		}
	}

	private static void ShowErrorPromptIfNeeded(string vmName, int errorCode)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		empty = ((errorCode != 10) ? LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", "") : LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", ""));
		if (!string.IsNullOrEmpty(empty))
		{
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INSTALLATION_ERROR", "");
			val.BodyTextBlock.Text = empty;
			val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler)null, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)BlueStacksUIUtils.DictWindows[vmName];
			BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay();
			((Window)val).ShowDialog();
			BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
		}
	}

	public static void GooglePlayAppInstall(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string[] allKeys = requestData.Data.AllKeys;
			foreach (string text in allKeys)
			{
				Logger.Info("Key: {0}, Value: {1}", new object[2]
				{
					text,
					requestData.Data[text]
				});
			}
			string packageName = requestData.Data["packageName"];
			string appName = requestData.Data["appName"];
			string isAdditionalFile = requestData.Data["isAdditionalFile"];
			string status = requestData.Data["status"];
			if (string.IsNullOrEmpty(status) || !BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				AppIconModel appIcon = BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
				if (appIcon == null || !appIcon.mIsAppInstalled)
				{
					if (status.Equals("STARTED", StringComparison.InvariantCultureIgnoreCase))
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, string.Empty, null);
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, string.Empty);
					}
					if (status.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase) && isAdditionalFile.Equals("false", StringComparison.OrdinalIgnoreCase))
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
					}
					else if (status.Equals("CANCELED", StringComparison.InvariantCultureIgnoreCase))
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.RemoveAppIcon(packageName);
					}
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GooglePlayAppInstall: " + ex.ToString());
		}
	}

	internal static void ChangeTextOTSHandler(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					BlueStacksUIBinding.Bind(BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel, "STRING_WELCOME_TO_BLUESTACKS");
					Logger.Info("string set after change text OTS .." + ((ContentControl)BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel).Content);
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error in change ots text." + ex.ToString());
		}
	}

	internal static void ShootingModeChanged(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated = Convert.ToBoolean(requestData.Data["IsShootingModeActivated"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated)
				{
					((Popup)BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenSidebarPopup).IsOpen = false;
					((Popup)BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenTopbarPopup).IsOpen = false;
				}
				else
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mCommonHandler.ClipMouseCursorHandler(forceDisable: false, switchState: false);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Shooting Mode Changed: " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void ChangeOrientaionHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string packagename = val.Data["package"].ToString(CultureInfo.InvariantCulture);
			bool isPortrait = Convert.ToBoolean(val.Data["is_portrait"], CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].Frontend_OrientationChanged(packagename, isPortrait);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ChangeOrientaionHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void ShowGrmAndLaunchAppHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string packageName = val.Data["package"].ToString(CultureInfo.InvariantCulture);
			string vmName = val.RequestVmName;
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null)
				{
					BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(packageName);
				}
			}, new object[0]);
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ShowGrmAndLaunchAppHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void UpdateSizeOfOverlay(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				return;
			}
			((DispatcherObject)BlueStacksUIUtils.DictWindows[requestData.RequestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				try
				{
					IntPtr mLastMappableWindowHandle = new IntPtr(Convert.ToInt32(requestData.Data["handle"], CultureInfo.InvariantCulture));
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].StaticComponents.mLastMappableWindowHandle = mLastMappableWindowHandle;
					if (KMManager.dictOverlayWindow.ContainsKey(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]))
					{
						KMManager.dictOverlayWindow[BlueStacksUIUtils.DictWindows[requestData.RequestVmName]].UpdateSize();
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception in UpdateSizeOfOverlay: " + ex2.ToString());
					WriteErrorJsonArray(ex2.Message, res);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UpdateSizeOfOverlay: " + ex.ToString());
			WriteErrorJsonArray(ex.Message, res);
		}
	}

	internal static void BootFailedPopupHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].Utils.SendGuestBootFailureStats("com exception");
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in BootFailedPopupHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void DragDropInstallHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string requestVmName = val.RequestVmName;
			string apkPath = val.Data["filePath"].ToString(CultureInfo.InvariantCulture);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).InstallApk(apkPath, addToChooseApkList: true);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in DragDropInstallHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void DeviceProvisionedHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			Logger.Info("Device provisioned client");
			RequestData val = HTTPUtils.ParseRequest(req);
			_ = val.RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[val.RequestVmName].mAppHandler.IsOneTimeSetupCompleted = true;
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void GoogleSigninHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			_ = val.RequestVmName;
			string text = val.Data["email"].ToString(CultureInfo.InvariantCulture).Trim();
			if (BlueStacksUIUtils.DictWindows.ContainsKey(val.RequestVmName))
			{
				RegistryManager.Instance.Guest[val.RequestVmName].IsGoogleSigninDone = true;
				Stats.SendUnifiedInstallStatsAsync("google_login_completed", text);
				BlueStacksUIUtils.DictWindows[val.RequestVmName].PostGoogleSigninCompleteTask();
				Publisher.PublishMessage((BrowserControlTags)1, val.RequestVmName, (JObject)null);
			}
			WriteSuccessJsonArray(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GoogleSigninHandler: " + ex.ToString());
			WriteErrorJSONObjectWithoutReason(res);
		}
	}

	internal static void SetDMMKeymapping(HttpListenerRequest req, HttpListenerResponse _)
	{
		Logger.Info("Got SetKeymapping {0} request from {1}", new object[2]
		{
			req.HttpMethod,
			req.RemoteEndPoint.ToString()
		});
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			string vmName = val.RequestVmName;
			string package = val.Data["package"].ToString(CultureInfo.InvariantCulture);
			bool isKeymapEnabled = Convert.ToBoolean(val.Data["enablekeymap"], CultureInfo.InvariantCulture);
			Logger.Info("package : " + package + " enablekeymap : " + isKeymapEnabled);
			if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				return;
			}
			int retries = 3;
			while (retries > 0)
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					if ((int)((UIElement)BlueStacksUIUtils.DictWindows[vmName]).Visibility == 0 && BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs.ContainsKey(package))
					{
						retries = 0;
						BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs[package].IsDMMKeymapEnabled = isKeymapEnabled;
					}
				}, new object[0]);
				if (retries > 0)
				{
					retries--;
					Thread.Sleep(1000);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server SetKeymapping: " + ex.ToString());
		}
	}

	internal static void ReloadShortcuts(HttpListenerRequest req, HttpListenerResponse _)
	{
		try
		{
			string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				((DispatcherObject)BlueStacksUIUtils.DictWindows[requestVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					CommonHandlers.ReloadShortcutsForAllInstances();
				}, new object[0]);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReloadShortcuts: " + ex.ToString());
		}
	}

	internal static void ReloadPromotions(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				PromotionManager.ReloadPromotionsAsync();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReloadPromotions: " + ex.ToString());
		}
	}

	internal static void HandleOverlayControlsVisibility(HttpListenerRequest req, HttpListenerResponse res)
	{
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string vmName = requestData.RequestVmName;
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && RegistryManager.Instance.IsGameTvEnabled)
				{
					string data = requestData.Data["data"];
					KMManager.HandleCallbackControl(BlueStacksUIUtils.DictWindows[vmName], data);
				}
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && (((Window)BlueStacksUIUtils.DictWindows[vmName]).IsActive || (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.ParentWindow == BlueStacksUIUtils.DictWindows[vmName] && ((Window)KMManager.sGuidanceWindow).IsActive) || (KMManager.CanvasWindow != null && KMManager.CanvasWindow.ParentWindow == BlueStacksUIUtils.DictWindows[vmName] && KMManager.CanvasWindow.SidebarWindow != null && ((Window)KMManager.CanvasWindow.SidebarWindow).IsActive)))
				{
					string data2 = requestData.Data["data"];
					KMManager.ShowDynamicOverlay(BlueStacksUIUtils.DictWindows[vmName], isShow: true, isReload: false, data2);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleOverlayControlsVisibility: " + ex.ToString());
		}
	}
}
