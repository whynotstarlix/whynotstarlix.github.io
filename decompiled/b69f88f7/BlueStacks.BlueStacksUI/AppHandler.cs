using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class AppHandler
{
	private MainWindow ParentWindow;

	private Thread mOtsCheckThread;

	private object mOtsCheckLock = new object();

	private int oneSecond = 1000;

	private bool mIsOneTimeSetupCompleted;

	private bool mIsGuestReady;

	internal bool mGuestReadyCheckStarted;

	internal string mDefaultLauncher = "com.bluestacks.appmart";

	private object sLockObject = new object();

	private object sOTSLock = new object();

	private string mSwitchWhenPackageNameRecieved = string.Empty;

	public SerializableDictionary<string, DateTime> CdnAppdict { get; set; } = new SerializableDictionary<string, DateTime>();

	public string mLastAppDisplayed { get; set; } = string.Empty;

	public string mLastRunAppSentForSynced { get; set; } = string.Empty;

	public string mAppDisplayedOccured { get; set; } = string.Empty;

	public static List<string> ListIgnoredApps { get; } = new List<string>
	{
		"tv.gamepop.home", "com.pop.store", "com.pop.store51", "com.bluestacks.s2p5105", "com.bluestacks.help", "mpi.v23", "com.google.android.gms", "com.google.android.gsf.login", "com.android.deskclock", "me.onemobile.android",
		"me.onemobile.lite.android", "android.rk.RockVideoPlayer.RockVideoPlayer", "com.bluestacks.chartapp", "com.bluestacks.setupapp", "com.android.gallery3d", "com.bluestacks.keymappingtool", "com.baidu.appsearch", "com.bluestacks.s2p", "com.bluestacks.windowsfilemanager", "com.android.quicksearchbox",
		"com.bluestacks.setup", "com.bluestacks.appsettings", "mpi.v23", "com.bluestacks.setup", "com.bluestacks.gamepophome", "com.bluestacks.appfinder", "com.android.providers.downloads.ui", "com.google.android.instantapps.supervisor"
	};

	public bool IsOneTimeSetupCompleted
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			if ((int)RegistryManager.Instance.InstallationType == 1 && (int)GameConfig.Instance.AppGenericAction == 2)
			{
				return true;
			}
			if (!mIsOneTimeSetupCompleted)
			{
				StartOTSCheckThread();
			}
			return mIsOneTimeSetupCompleted;
		}
		set
		{
			mIsOneTimeSetupCompleted = value;
			ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone = value;
			Logger.Info("One time setup completed. Will perform tasks now");
			lock (sOTSLock)
			{
				Logger.Info("Performing OTS completed tasks");
				if (value && EventOnOneTimeSetupCompleted != null)
				{
					EventOnOneTimeSetupCompleted(ParentWindow, new EventArgs());
					EventOnOneTimeSetupCompleted = null;
				}
			}
		}
	}

	public bool IsGuestReady
	{
		get
		{
			return mIsGuestReady;
		}
		set
		{
			mIsGuestReady = value;
			if (mIsGuestReady)
			{
				SignalGuestReady();
			}
		}
	}

	public string SwitchWhenPackageNameRecieved
	{
		get
		{
			return mSwitchWhenPackageNameRecieved;
		}
		set
		{
			mSwitchWhenPackageNameRecieved = value;
			if (!string.IsNullOrEmpty(mSwitchWhenPackageNameRecieved) && mSwitchWhenPackageNameRecieved.Equals(mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
			{
				AppLaunched(mSwitchWhenPackageNameRecieved, forced: true);
			}
		}
	}

	public EventHandler<EventArgs> EventOnOneTimeSetupCompleted { get; set; }

	public static EventHandler<EventArgs> EventOnAppDisplayed { get; set; }

	private void StartOTSCheckThread()
	{
		if (mOtsCheckThread != null)
		{
			return;
		}
		lock (mOtsCheckLock)
		{
			if (mOtsCheckThread != null)
			{
				return;
			}
			try
			{
				mOtsCheckThread = new Thread((ThreadStart)delegate
				{
					Logger.Info("Checking for if OTS completed");
					while (!mIsOneTimeSetupCompleted)
					{
						CheckingOneTimeSetupCompleted();
						Thread.Sleep(2 * oneSecond);
					}
				})
				{
					IsBackground = true
				};
				mOtsCheckThread.Start();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to create ots check thread.");
				Logger.Error(ex.ToString());
			}
		}
	}

	private void SignalGuestReady()
	{
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			Logger.Info("Boot install: Signal Guest Ready");
			ParentWindow.GuestBoot_Completed();
		}
		else
		{
			ParentWindow.Utils.sBootCheckTimer.Enabled = false;
			ParentWindow.mEnableLaunchPlayForNCSoft = true;
		}
	}

	private void CheckingOneTimeSetupCompleted()
	{
		try
		{
			string text = ((object)JObject.Parse(HTTPUtils.SendRequestToGuest("isOTSCompleted", (Dictionary<string, string>)null, ParentWindow.mVmName, 1000, (Dictionary<string, string>)null, false, 1, 0, "bgp64"))["result"]).ToString();
			if (text.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
			{
				Logger.Info("OTS result: {0}", new object[1] { text });
				IsOneTimeSetupCompleted = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in checking OneTimeSetupCompleted with vmName {0}. Err: {1}", new object[2] { ParentWindow.mVmName, ex.Message });
		}
	}

	internal AppHandler(MainWindow window)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		ParentWindow = window;
		string cDNAppsTimeStamp = RegistryManager.Instance.CDNAppsTimeStamp;
		if (!string.IsNullOrEmpty(cDNAppsTimeStamp))
		{
			XmlReader val = XmlReader.Create((TextReader)new StringReader(cDNAppsTimeStamp));
			try
			{
				XmlSerializer val2 = new XmlSerializer(typeof(SerializableDictionary<string, DateTime>));
				CdnAppdict = (SerializableDictionary<string, DateTime>)val2.Deserialize(val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		mIsOneTimeSetupCompleted = ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone;
	}

	public bool IsAppInstalled(string package)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		string text = default(string);
		if (new JsonParser(ParentWindow.mVmName).IsAppInstalled(package, ref text))
		{
			result = true;
		}
		return result;
	}

	public bool IsAppInstalled(string package, out string version)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (new JsonParser(ParentWindow.mVmName).IsAppInstalled(package, ref version))
		{
			result = true;
		}
		return result;
	}

	public void AppLaunched(string packageName, bool forced = false)
	{
		lock (sLockObject)
		{
			if (ParentWindow.mClosed)
			{
				return;
			}
			if ((packageName == BlueStacksUIUtils.sUserAccountPackageName || packageName == "com.android.vending") && mSwitchWhenPackageNameRecieved == "com.android.vending")
			{
				packageName = mSwitchWhenPackageNameRecieved;
				if (string.Compare(mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					mSwitchWhenPackageNameRecieved = "";
				}
			}
			if (!forced && string.Equals(packageName, mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			if (!mIsOneTimeSetupCompleted)
			{
				if (!string.IsNullOrEmpty(packageName) && (packageName.StartsWith("com.google.android.gms", StringComparison.InvariantCultureIgnoreCase) || packageName.Equals("com.google.android.setupwizard", StringComparison.InvariantCultureIgnoreCase)))
				{
					StartOTSCheckThread();
				}
				return;
			}
			Logger.Info("SwitchWhenPackageNameRecieved: {0}", new object[1] { mSwitchWhenPackageNameRecieved });
			ParentWindow.ShowLoadingGrid(isShow: false);
			bool receivedFromImap = string.Compare(mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0;
			if (receivedFromImap)
			{
				mLastRunAppSentForSynced = "";
			}
			if (!string.IsNullOrEmpty(mSwitchWhenPackageNameRecieved) && string.Equals(packageName, mSwitchWhenPackageNameRecieved, StringComparison.OrdinalIgnoreCase))
			{
				mSwitchWhenPackageNameRecieved = string.Empty;
				if (EventOnAppDisplayed == null)
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_0058: Unknown result type (might be due to invalid IL or missing references)
						//IL_0062: Expected O, but got Unknown
						//IL_005d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0067: Expected O, but got Unknown
						ParentWindow.mTopBar.mAppTabButtons.GoToTab(packageName, receivedFromImap);
						Publisher.PublishMessage((BrowserControlTags)4, ParentWindow.mVmName, new JObject((object)new JProperty("PackageName", (object)packageName)));
					}, new object[0]);
				}
				else
				{
					EventHandler<EventArgs> eventOnAppDisplayed = EventOnAppDisplayed;
					EventOnAppDisplayed = null;
					eventOnAppDisplayed(ParentWindow, new EventArgs());
				}
			}
			else if (mDefaultLauncher.Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
			{
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					Logger.Info("Assuming app is crashed/exited going to last tab");
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
						//IL_00cf: Invalid comparison between Unknown and I4
						if (ParentWindow.mFrontendGrid != null)
						{
							DependencyObject parent = ((FrameworkElement)ParentWindow.mFrontendGrid).Parent;
							if ((object)((parent is Grid) ? parent : null) == ParentWindow.FrontendParentGrid)
							{
								if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
								{
									ParentWindow.mTopBar.mAppTabButtons.CloseTab(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, sendStopAppToAndroid: false, forceClose: false, dontCheckQuitPopup: true, receivedFromImap: false, packageName);
								}
								if ((int)RegistryManager.Instance.InstallationType == 1)
								{
									PerformGamingAction();
								}
							}
							else
							{
								ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
							}
						}
					}, new object[0]);
				}
			}
			else
			{
				AppIconModel icon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
				if (icon != null)
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.mTopBar.mAppTabButtons.AddAppTab(icon.AppName, icon.PackageName, icon.ActivityName, icon.ImageName, isSwitch: true, isLaunch: false, receivedFromImap);
					}, new object[0]);
				}
			}
			mLastAppDisplayed = packageName;
		}
	}

	public void HandleAppDisplayed(string packageName)
	{
		if (!string.Equals(packageName, mDefaultLauncher, StringComparison.InvariantCultureIgnoreCase))
		{
			return;
		}
		Logger.Info("Home app is displayed...closing tab");
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (ParentWindow.mFrontendGrid != null)
			{
				DependencyObject parent = ((FrameworkElement)ParentWindow.mFrontendGrid).Parent;
				if ((object)((parent is Grid) ? parent : null) == ParentWindow.FrontendParentGrid && ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
				{
					ParentWindow.mTopBar.mAppTabButtons.CloseTab(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, sendStopAppToAndroid: false, forceClose: false, dontCheckQuitPopup: true);
				}
			}
		}, new object[0]);
	}

	internal void GoHome()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			VmCmdHandler.RunCommand("home", ParentWindow.mVmName);
		});
	}

	public string GetDefaultLauncher()
	{
		string result = "com.bluestacks.appmart";
		try
		{
			string text = HTTPUtils.SendRequestToGuest("getDefaultLauncher", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			Logger.Info("GetDefaultLauncher response = " + text);
			JObject val = JObject.Parse(text);
			string text2 = ((object)val["result"]).ToString().Trim();
			if (text2 == "ok")
			{
				result = ((object)val["defaultLauncher"]).ToString().Trim();
			}
			else if (text2 == "error" && ((object)val["reason"]).ToString().Trim() == "no default launcher")
			{
				result = "none";
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in GetDefauntLauncher. Err." + ex.ToString());
		}
		return result;
	}

	internal void StartCustomActivity(Dictionary<string, string> data)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Logger.Info("Starting a custom activity");
				foreach (KeyValuePair<string, string> datum in data)
				{
					Logger.Debug("Data = {0} , {1}", new object[2] { datum.Key, datum.Value });
				}
				HTTPUtils.SendRequestToGuest("customStartActivity", data, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in launching custom activity. Err: " + ex.Message);
			}
		});
	}

	internal void SetDefaultLauncher(string launcherName)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "d", launcherName } };
				string text = HTTPUtils.SendRequestToGuest("setDefaultLauncher", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("Setlauncher res: {0}", new object[1] { text });
				dictionary = new Dictionary<string, string> { { "arg", "" } };
				text = HTTPUtils.SendRequestToGuest("home", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("the response for home command is {0}", new object[1] { text });
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SetDefaultLauncher. Err:{0}", new object[1] { ex.ToString() });
			}
		});
	}

	internal void AppUninstalled(string package)
	{
		ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppAfterUninstall(package);
		ParentWindow.mTopBar.mAppTabButtons.CloseTab(package, sendStopAppToAndroid: false, forceClose: false, dontCheckQuitPopup: true);
		if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(package))
		{
			if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsForcedLandscapeEnabled)
			{
				Utils.SetCustomAppSize(ParentWindow.mVmName, package, (ScreenMode)2);
				KMManager.SelectSchemeIfPresent(ParentWindow, "Portrait", "appuninstalled", forceSave: false);
				AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsForcedLandscapeEnabled = false;
			}
			if (AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsForcedPortraitEnabled)
			{
				Utils.SetCustomAppSize(ParentWindow.mVmName, package, (ScreenMode)2);
				KMManager.SelectSchemeIfPresent(ParentWindow, "Landscape", "appuninstalled", forceSave: false);
				AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsForcedPortraitEnabled = false;
			}
		}
	}

	internal void AppInstalled(string package, bool isUpdate)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		AppInfo val = ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package);
		JObject val2 = new JObject
		{
			["PackageName"] = JToken.op_Implicit(package),
			["AppName"] = JToken.op_Implicit((val != null) ? val.Name : null),
			["IsGamepadCompatible"] = JToken.op_Implicit((val != null) ? new bool?(val.IsGamepadCompatible) : ((bool?)null))
		};
		Publisher.PublishMessage((BrowserControlTags)5, ParentWindow.mVmName, val2);
		if (FeatureManager.Instance.IsShowAppRecommendations || !RegistryManager.Instance.IsPremium)
		{
			ParentWindow.mWelcomeTab.mHomeAppManager.UpdateRecommendedAppsInstallStatus(package);
		}
		GrmHandler.RefreshGrmIndication(package, ParentWindow.mVmName);
		GrmHandler.SendUpdateGrmPackagesToAndroid(ParentWindow.mVmName);
		GrmHandler.SendUpdateGrmPackagesToBrowser(ParentWindow.mVmName);
		GuidanceCloudInfoManager.Instance.AppsGuidanceCloudInfoRefresh();
		if ((int)RegistryManager.Instance.FirstAppLaunchState == 1)
		{
			RegistryManager.Instance.FirstAppLaunchState = (AppLaunchState)2;
		}
		if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(package))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package] = new AppSettings();
		}
		if (!isUpdate)
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsAppOnboardingCompleted = false;
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][package].IsGeneralAppOnBoardingCompleted = false;
		}
	}

	internal void UpdateDefaultLauncher()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			string text = GetDefaultLauncher();
			Logger.Info("DefaultLauncher " + text);
			if (text.Equals("none", StringComparison.InvariantCultureIgnoreCase))
			{
				text = "com.bluestacks.appmart";
				SetDefaultLauncher(text);
			}
			if (text.Equals("com.android.provision", StringComparison.InvariantCultureIgnoreCase))
			{
				text = "com.bluestacks.appmart";
			}
			mDefaultLauncher = text;
		});
	}

	internal void SendSearchPlayRequestAsync(string searchQuery)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			if (searchQuery.Contains("search::"))
			{
				searchQuery = searchQuery.Remove(0, 8);
			}
			VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "searchplay {0}", new object[1] { searchQuery }), ParentWindow.mVmName);
		});
	}

	internal void LaunchPlayRequestAsync(string packageName)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "launchplay?pkgname={0}", new object[1] { packageName }), ParentWindow.mVmName);
		});
	}

	public void SendRunAppRequestAsync(string package, string activity = "", bool receivedFromImap = false)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Expected O, but got Unknown
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Expected O, but got Unknown
			if (ParentWindow.SendClientActions && !receivedFromImap)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>
				{
					{ "EventAction", "RunApp" },
					{ "Package", package },
					{ "Activity", activity }
				};
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)0;
				dictionary.Add("operationData", JsonConvert.SerializeObject((object)dictionary2, serializerSettings));
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
			}
			if (receivedFromImap)
			{
				mLastRunAppSentForSynced = package;
				if (package == "com.android.vending")
				{
					mSwitchWhenPackageNameRecieved = package;
				}
			}
			if (string.IsNullOrEmpty(activity))
			{
				AppIconModel appIcon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(package);
				if (appIcon != null)
				{
					activity = appIcon.ActivityName;
				}
				if (string.IsNullOrEmpty(activity))
				{
					activity = ".Main";
					Logger.Info("Empty activity name ovveriding .Main for package: " + package);
				}
			}
			if (ThirdParty.AllPUBGPackageNames.Contains(package))
			{
				string displayQualityPubg = RegistryManager.Instance.Guest[ParentWindow.mVmName].DisplayQualityPubg;
				string gamingResolutionPubg = RegistryManager.Instance.Guest[ParentWindow.mVmName].GamingResolutionPubg;
				if (string.Equals(displayQualityPubg, "-1", StringComparison.InvariantCulture) && string.Equals(gamingResolutionPubg, "1", StringComparison.InvariantCulture))
				{
					SendRunex(package, activity);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					JsonWriter val = (JsonWriter)new JsonTextWriter((TextWriter)new StringWriter(stringBuilder));
					try
					{
						val.WriteStartObject();
						if (string.Equals(RegistryManager.Instance.Guest[ParentWindow.mVmName].DisplayQualityPubg, "-1", StringComparison.InvariantCulture))
						{
							val.WritePropertyName("renderqualitylevel");
							val.WriteValue("0");
						}
						else
						{
							val.WritePropertyName("renderqualitylevel");
							val.WriteValue(RegistryManager.Instance.Guest[ParentWindow.mVmName].DisplayQualityPubg);
						}
						val.WritePropertyName("contentscale");
						val.WriteValue(RegistryManager.Instance.Guest[ParentWindow.mVmName].GamingResolutionPubg);
						val.WriteEndObject();
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
					Dictionary<string, string> dictionary3 = new Dictionary<string, string>
					{
						{
							"component",
							package + "/" + activity
						},
						{
							"extras",
							stringBuilder.ToString()
						}
					};
					string text = HTTPUtils.SendRequestToGuest("customStartActivity", dictionary3, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					Logger.Info("The response we get is: " + text);
				}
			}
			else if (ThirdParty.AllCallOfDutyPackageNames.Contains(package))
			{
				int num = int.Parse(RegistryManager.Instance.Guest[ParentWindow.mVmName].DisplayQualityCOD, CultureInfo.InvariantCulture);
				int num2 = int.Parse(RegistryManager.Instance.Guest[ParentWindow.mVmName].GamingResolutionCOD, CultureInfo.InvariantCulture);
				int num3 = int.Parse("1", CultureInfo.InvariantCulture);
				StringBuilder stringBuilder2 = new StringBuilder();
				JsonWriter val2 = (JsonWriter)new JsonTextWriter((TextWriter)new StringWriter(stringBuilder2));
				try
				{
					val2.WriteStartObject();
					if (string.Equals(num.ToString(CultureInfo.InvariantCulture), "-1", StringComparison.InvariantCulture))
					{
						val2.WritePropertyName("QualityLevel");
						val2.WriteValue(int.Parse("0", CultureInfo.InvariantCulture));
					}
					else
					{
						val2.WritePropertyName("QualityLevel");
						val2.WriteValue(num);
					}
					val2.WritePropertyName("ResolutionHeight");
					val2.WriteValue(num2);
					val2.WritePropertyName("FrameRateLevel");
					val2.WriteValue(num3);
					val2.WriteEndObject();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				Dictionary<string, string> dictionary4 = new Dictionary<string, string>
				{
					{
						"component",
						package + "/" + activity
					},
					{
						"extras",
						stringBuilder2.ToString()
					}
				};
				string text2 = HTTPUtils.SendRequestToGuest("customStartActivity", dictionary4, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("The response we get is: " + text2);
			}
			else if ("com.android.chrome".Equals(package, StringComparison.InvariantCultureIgnoreCase))
			{
				HTTPUtils.SendRequestToGuest("launchchrome", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			else
			{
				SendRunex(package, activity);
			}
		});
	}

	internal void SendRunex(string package, string activity)
	{
		VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "runex {0}/{1}", new object[2] { package, activity }), ParentWindow.mVmName);
	}

	internal void StopAppRequest(string packageName)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Logger.Info("Will send stop {0} request", new object[1] { packageName });
				Dictionary<string, string> data = new Dictionary<string, string> { { "appPackage", packageName } };
				string text = ParentWindow.mFrontendHandler.SendFrontendRequest("stopAppInfo", data);
				Logger.Info("the response we get is {0}", new object[1] { text });
				Logger.Info(VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "StopApp {0}", new object[1] { packageName }), ParentWindow.mVmName));
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in StopAppRequest. Err : " + ex.ToString());
			}
		});
	}

	internal void SendRequestToRemoveAccountAndCloseWindowASync(bool closeWindow = false)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				string text = HTTPUtils.SendRequestToGuest("removeAccountsInfo", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("Account removed response: " + text);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in removing account, Ex: " + ex.Message);
			}
			if (closeWindow)
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.ForceCloseWindow();
				}, new object[0]);
			}
		});
	}

	internal void WriteXMl(bool isAppInstall, string packageName, DateTime timestamp)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (isAppInstall)
		{
			((Dictionary<string, DateTime>)(object)CdnAppdict)[packageName] = timestamp;
			using StringWriter stringWriter = new StringWriter();
			new XmlSerializer(typeof(SerializableDictionary<string, DateTime>)).Serialize((TextWriter)stringWriter, (object)CdnAppdict);
			RegistryManager.Instance.CDNAppsTimeStamp = stringWriter.ToString();
			return;
		}
		if (((Dictionary<string, DateTime>)(object)CdnAppdict).ContainsKey(packageName))
		{
			((Dictionary<string, DateTime>)(object)CdnAppdict).Remove(packageName);
			using StringWriter stringWriter2 = new StringWriter();
			new XmlSerializer(typeof(SerializableDictionary<string, DateTime>)).Serialize((TextWriter)stringWriter2, (object)CdnAppdict);
			RegistryManager.Instance.CDNAppsTimeStamp = stringWriter2.ToString();
		}
	}

	internal void PerformGamingAction(string pkgName = "", string activityName = "")
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GenericAction action;
		if (string.IsNullOrEmpty(pkgName))
		{
			pkgName = GameConfig.Instance.PkgName;
			activityName = GameConfig.Instance.ActivityName;
			action = GameConfig.Instance.AppGenericAction;
		}
		else
		{
			action = (GenericAction)1;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			if (IsAppInstalled(pkgName))
			{
				SendRunAppRequestAsync(pkgName);
			}
			else if ((int)action == 1)
			{
				LaunchPlayRequestAsync(pkgName);
			}
		}, new object[0]);
	}
}
