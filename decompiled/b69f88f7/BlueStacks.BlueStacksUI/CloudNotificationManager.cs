using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal sealed class CloudNotificationManager
{
	private static volatile CloudNotificationManager sInstance;

	private static object syncRoot = new object();

	private static SerialWorkQueue mWorkQueue = null;

	public static CloudNotificationManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot)
				{
					if (sInstance == null)
					{
						sInstance = new CloudNotificationManager();
					}
				}
			}
			return sInstance;
		}
	}

	private static SerialWorkQueue WorkQueue
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			if (mWorkQueue == null)
			{
				lock (syncRoot)
				{
					if (mWorkQueue == null)
					{
						mWorkQueue = new SerialWorkQueue("androidCloudNotifications");
						if (BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mGuestBootCompleted)
						{
							mWorkQueue.Start();
						}
					}
				}
			}
			return mWorkQueue;
		}
	}

	private CloudNotificationManager()
	{
	}

	internal void HandleCloudNotification(string jsonReceived, string vmName)
	{
		try
		{
			Logger.Info("CloudFireBaseNotification response received: " + jsonReceived + " from vm: " + vmName);
			JObject val = JObject.Parse(jsonReceived);
			if (val["bluestacks_notification"] != null && val["bluestacks_notification"].ToObject<JObject>()["tag"] != null && !JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(val["bluestacks_notification"], "tag")))
			{
				HandleTagsInfo(val, jsonReceived);
			}
			if (val["bluestacks_notification"] != null && val["bluestacks_notification"].ToObject<JObject>()["type"] != null)
			{
				switch (((object)val["bluestacks_notification"][(object)"type"]).ToString().ToLower(CultureInfo.InvariantCulture))
				{
				case "genericnotification":
					HandleGenericNotification(val, vmName);
					break;
				case "genericreddotnotification":
					HandleGenericRedDotNotification(val, vmName);
					break;
				case "callmethod":
					HandleCallMethod(val, vmName);
					break;
				default:
					Logger.Warning("No notification type found in HandleCloudNotification. json: " + jsonReceived);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleCloudNotification. json: " + jsonReceived + " Error: " + ex.ToString());
		}
	}

	private static void HandleTagsInfo(JObject json, string jsonReceived)
	{
		try
		{
			foreach (string item in json["bluestacks_notification"][(object)"tag"].ToObject<List<string>>())
			{
				if (BrowserControl.mFirebaseTagsSubscribed.Contains(item))
				{
					SendNotifJsonToHtmlTag(item, jsonReceived);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in HandleTagsInfo: " + ex.ToString());
		}
	}

	private static void SendNotifJsonToHtmlTag(string _, string data)
	{
		try
		{
			object[] array = new object[1] { "" };
			if (!string.IsNullOrEmpty(data))
			{
				array[0] = data;
			}
			foreach (BrowserControl sAllBrowserControl in BrowserControl.sAllBrowserControls)
			{
				if (sAllBrowserControl != null && sAllBrowserControl.CefBrowser != null)
				{
					sAllBrowserControl.CefBrowser.CallJs(sAllBrowserControl.mFirebaseCallbackMethod, array);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending json to appcenter:" + ex.ToString());
		}
	}

	internal static SerializableDictionary<string, string> HandleExtraPayload(JObject json, NotificationPayloadType payloadType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SerializableDictionary<string, string> result = ((JToken)json).ToObject<SerializableDictionary<string, string>>();
		_ = 1;
		return result;
	}

	internal static void HandleGenericRedDotNotification(JObject resJson, string vmName)
	{
		JObject val = JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"][(object)"GenericRedDotNotificationItem"]).ToString());
		if (!JsonExtensions.IsNullOrEmptyBrackets(((object)val["myapps_cross_promotion"]).ToString()))
		{
			PromotionManager.AddNewMyAppsCrossPromotion((JToken)(object)val);
			PromotionObject.Save();
			string appPackage = ((object)val["myapps_cross_promotion"][(object)"app_pkg"]).ToString();
			((DispatcherObject)BlueStacksUIUtils.DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.AddIconWithRedDot(appPackage);
			}, new object[0]);
		}
	}

	internal static void HandleGenericNotification(JObject resJson, string vmName)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Expected O, but got Unknown
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		GenericNotificationItem genericItem = new GenericNotificationItem();
		try
		{
			JObject val = JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"][(object)"GenericNotificationItem"]).ToString());
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "id", (Action<string>)delegate(string x)
			{
				genericItem.Id = x;
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "priority", (Action<string>)delegate(string x)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				genericItem.Priority = EnumHelper.Parse<NotificationPriority>(x, (NotificationPriority)1);
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "title", (Action<string>)delegate(string x)
			{
				genericItem.Title = x;
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "message", (Action<string>)delegate(string x)
			{
				genericItem.Message = x;
			});
			JsonExtensions.AssignIfContains<bool>((JToken)(object)val, "showribbon", (Action<bool>)delegate(bool x)
			{
				genericItem.ShowRibbon = x;
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "menuimagename", (Action<string>)delegate(string x)
			{
				genericItem.NotificationMenuImageName = x;
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "menuimageurl", (Action<string>)delegate(string x)
			{
				genericItem.NotificationMenuImageUrl = x;
			});
			JsonExtensions.AssignIfContains<bool>((JToken)(object)val, "isread", (Action<bool>)delegate(bool x)
			{
				genericItem.IsRead = x;
			});
			JsonExtensions.AssignIfContains<bool>((JToken)(object)val, "isdeleted", (Action<bool>)delegate(bool x)
			{
				genericItem.IsDeleted = x;
			});
			JsonExtensions.AssignIfContains<bool>((JToken)(object)val, "deferred", (Action<bool>)delegate(bool x)
			{
				genericItem.IsDeferred = x;
			});
			JsonExtensions.AssignIfContains<string>((JToken)(object)val, "creationtime", (Action<string>)delegate(string x)
			{
				genericItem.CreationTime = DateTime.ParseExact(x, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
			});
			if (!string.IsNullOrEmpty(genericItem.NotificationMenuImageName) && !string.IsNullOrEmpty(genericItem.NotificationMenuImageUrl))
			{
				genericItem.NotificationMenuImageName = Utils.TinyDownloader(genericItem.NotificationMenuImageUrl, genericItem.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
			}
			if (val["ExtraPayload"] != null && !JsonExtensions.IsNullOrEmptyBrackets(((object)val.GetValue("ExtraPayload", StringComparison.InvariantCulture)).ToString()))
			{
				JsonExtensions.AssignIfContains<string>(val["ExtraPayload"], "payloadtype", (Action<string>)delegate(string x)
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					genericItem.PayloadType = EnumHelper.Parse<NotificationPayloadType>(x, (NotificationPayloadType)0);
				});
				SerializableDictionary<string, string> extraPayload = genericItem.ExtraPayload;
				if (extraPayload != null)
				{
					DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)extraPayload, (Dictionary<string, string>)(object)HandleExtraPayload(val.GetValue("ExtraPayload", StringComparison.InvariantCulture).ToObject<JObject>(), genericItem.PayloadType));
				}
			}
			ClientStats.SendMiscellaneousStatsAsync("notification_received", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, genericItem.Id, genericItem.Title, ((Dictionary<string, string>)(object)genericItem.ExtraPayload).ContainsKey("campaign_id") ? ((Dictionary<string, string>)(object)genericItem.ExtraPayload)["campaign_id"] : "");
			genericItem.IsReceivedStatSent = true;
			if (val["conditions"] != null && !JsonExtensions.IsNullOrEmptyBrackets(((object)val.GetValue("conditions", StringComparison.InvariantCulture)).ToString()))
			{
				JsonExtensions.AssignIfContains<string>(val["conditions"], "app_pkg_on_top", (Action<string>)delegate(string x)
				{
					genericItem.DeferredApp = x;
				});
				JsonExtensions.AssignIfContains<long>(val["conditions"], "app_usage_seconds", (Action<long>)delegate(long x)
				{
					genericItem.DeferredAppUsage = x;
				});
			}
			if (genericItem.ShowRibbon && resJson["bluestacks_notification"][(object)"payload"].ToObject<JObject>()["RibbonDesign"] != null && !JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(resJson["bluestacks_notification"][(object)"payload"], "RibbonDesign")))
			{
				genericItem.NotificationDesignItem = new GenericNotificationDesignItem();
				JObject val2 = JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"][(object)"RibbonDesign"]).ToString());
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "titleforegroundcolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.TitleForeGroundColor = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "messageforegroundcolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.MessageForeGroundColor = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "bordercolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.BorderColor = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "ribboncolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.Ribboncolor = x;
				});
				JsonExtensions.AssignIfContains<double>((JToken)(object)val2, "auto_hide_timer", (Action<double>)delegate(double x)
				{
					genericItem.NotificationDesignItem.AutoHideTime = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "hoverbordercolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.HoverBorderColor = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "hoverribboncolor", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.HoverRibboncolor = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "leftgifname", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.LeftGifName = x;
				});
				JsonExtensions.AssignIfContains<string>((JToken)(object)val2, "leftgifurl", (Action<string>)delegate(string x)
				{
					genericItem.NotificationDesignItem.LeftGifUrl = x;
				});
				if (!string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifName) && !string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifUrl))
				{
					Utils.TinyDownloader(genericItem.NotificationDesignItem.LeftGifUrl, genericItem.NotificationDesignItem.LeftGifName, RegistryStrings.PromotionDirectory, false);
				}
				if (val2["background_gradient"] != null)
				{
					foreach (JObject item in ((JToken)JArray.Parse(((object)val2["background_gradient"]).ToString())).ToObject<List<JObject>>())
					{
						genericItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>(((object)item["color"]).ToString(), item["offset"].ToObject<double>()));
					}
				}
				if (val2["hover_background_gradient"] != null)
				{
					foreach (JObject item2 in ((JToken)JArray.Parse(((object)val2["hover_background_gradient"]).ToString())).ToObject<List<JObject>>())
					{
						genericItem.NotificationDesignItem.HoverBackGroundGradient.Add(new SerializableKeyValuePair<string, double>(((object)item2["color"]).ToString(), item2["offset"].ToObject<double>()));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while parsing generic notification. Not showing notification and not adding in notification menu." + ex.ToString());
			return;
		}
		try
		{
			if (string.IsNullOrEmpty(genericItem.Title) && string.IsNullOrEmpty(genericItem.Message))
			{
				genericItem.IsDeleted = true;
			}
			if (!genericItem.IsDeferred)
			{
				GenericNotificationManager.AddNewNotification(genericItem);
			}
			if (genericItem.ShowRibbon && resJson["bluestacks_notification"][(object)"payload"].ToObject<JObject>()["RibbonDesign"] != null && !JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(resJson["bluestacks_notification"][(object)"payload"], "RibbonDesign")))
			{
				if (!genericItem.IsDeferred)
				{
					BlueStacksUIUtils.DictWindows[vmName].HandleGenericNotificationPopup(genericItem);
				}
				else
				{
					HandleDeferredNotification(genericItem);
				}
			}
			BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception when handling notification json. Id " + genericItem.Id + " Error: " + ex2.ToString());
		}
	}

	private static void HandleDeferredNotification(GenericNotificationItem genericItem)
	{
		PromotionManager.sDeferredNotificationsList.Add(genericItem, AppUsageTimer.GetTotalTimeForPackageAfterReset(genericItem.DeferredApp.ToLower(CultureInfo.InvariantCulture)));
	}

	internal static void HandleCallMethod(JObject resJson, string vmName)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Expected O, but got Unknown
		string text = "";
		JsonExtensions.AssignStringIfContains((JToken)(object)JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"]).ToString()), "methodName", ref text);
		switch (text.ToLower(CultureInfo.InvariantCulture))
		{
		case "appusagestats":
			HandleUsageNotification(resJson, vmName);
			break;
		case "updatepromotions":
			PromotionManager.ReloadPromotionsAsync();
			break;
		case "updatebstconfig":
			UpdateBstConfig();
			break;
		case "openquitpopup":
			OpenQuitPopup(resJson, vmName);
			break;
		case "updategrm":
			GrmManager.UpdateGrmAsync(JsonExtensions.ToIenumerableString(resJson["bluestacks_notification"][(object)"payload"][(object)"app_pkg_list"]));
			break;
		case "calendarentry":
			try
			{
				JObject androidPayload = (JObject)resJson["bluestacks_notification"][(object)"payload"][(object)"androidPayload"];
				ClientStats.SendCalendarStats("calendar_" + ((object)resJson["bluestacks_notification"][(object)"payload"][(object)"methodType"]).ToString() + "_firebase", androidPayload.ContainsKey("startDate") ? ((object)androidPayload["startDate"]).ToString() : "", androidPayload.ContainsKey("endDate") ? ((object)androidPayload["endDate"]).ToString() : "", ((object)androidPayload["location"]).ToString());
				string endpoint = ((object)resJson["bluestacks_notification"][(object)"payload"][(object)"methodType"]).ToString() switch
				{
					"add" => "addcalendarevent", 
					"update" => "updatecalendarevent", 
					"delete" => "deletecalendarevent", 
					_ => throw new Exception("could not identify the methodType "), 
				};
				JObject val = new JObject((object)new JProperty("event", (object)androidPayload));
				Dictionary<string, string> data = new Dictionary<string, string> { ["event"] = ((object)val).ToString() };
				WorkQueue.Enqueue((Work)delegate
				{
					try
					{
						string text2 = HTTPUtils.SendRequestToGuest(endpoint, data, vmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
						Logger.Info("Response for calendarEntry " + text2);
						JObject val2 = JObject.Parse(text2);
						ClientStats.SendCalendarStats("calendar_" + ((object)resJson["bluestacks_notification"][(object)"payload"][(object)"methodType"]).ToString() + "_android", androidPayload.ContainsKey("startDate") ? ((object)androidPayload["startDate"]).ToString() : "", androidPayload.ContainsKey("endDate") ? ((object)androidPayload["endDate"]).ToString() : "", ((object)androidPayload["location"]).ToString(), string.Equals(((object)val2["result"]).ToString(), "ok", StringComparison.InvariantCultureIgnoreCase).ToString(CultureInfo.InvariantCulture), val2.ContainsKey("rowsDeleted") ? ((object)val2["rowsDeleted"]).ToString() : (val2.ContainsKey("rowsUpdated") ? ((object)val2["rowsUpdated"]).ToString() : ""));
					}
					catch (Exception arg2)
					{
						Logger.Warning($"Guest not booted, error in sending Calendar entry event: {arg2}");
					}
				});
				break;
			}
			catch (Exception arg)
			{
				Logger.Warning($"Error in sending Calendar entry event data to android.. Json:{resJson} error:  {arg}");
				break;
			}
		default:
			Logger.Error("No method type found in HandleCallMethod json: " + (object)resJson);
			break;
		}
	}

	internal static void UpdateBstConfig()
	{
		RegistryManager.Instance.UpdateBstConfig = true;
		FeatureManager.Init(true);
	}

	internal static void OpenQuitPopup(JObject resJson, string vmName)
	{
		string text = "";
		string package = "";
		string value = "";
		string value2 = "";
		JObject val = JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"]).ToString());
		JsonExtensions.AssignStringIfContains((JToken)(object)val, "url", ref text);
		if (!string.IsNullOrEmpty(text))
		{
			JsonExtensions.AssignStringIfContains((JToken)(object)val, "app_pkg", ref package);
			JsonExtensions.AssignStringIfContains((JToken)(object)val, "force_reload", ref value);
			JsonExtensions.AssignStringIfContains((JToken)(object)val, "show_on_quit", ref value2);
			bool result;
			bool isForceReload = bool.TryParse(value, out result) && result;
			bool result2;
			bool showOnQuit = bool.TryParse(value2, out result2) && result2;
			text = WebHelper.GetUrlWithParams(text);
			BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = true;
			if (BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl == null)
			{
				BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl = new QuitPopupBrowserControl(BlueStacksUIUtils.DictWindows[vmName]);
			}
			BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.SetQuitPopParams(text, package, isForceReload, showOnQuit);
			BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.LoadBrowser();
		}
		else
		{
			Logger.Info("Quit Popup notification received without url");
			BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = false;
		}
	}

	internal static void HandleUsageNotification(JObject resJson, string vmName)
	{
		try
		{
			string text = "";
			string jSONObjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
			JsonExtensions.AssignStringIfContains((JToken)(object)JObject.Parse(((object)resJson["bluestacks_notification"][(object)"payload"]).ToString()), "handler", ref text);
			string text2 = WebHelper.GetServerHost() + "/v2/" + text;
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				["oem"] = "bgp64",
				["client_ver"] = RegistryManager.Instance.ClientVersion,
				["engine_ver"] = RegistryManager.Instance.Version,
				["guid"] = RegistryManager.Instance.UserGuid,
				["locale"] = RegistryManager.Instance.UserSelectedLocale,
				["partner"] = RegistryManager.Instance.Partner,
				["campaignMD5"] = RegistryManager.Instance.CampaignMD5
			};
			if (!string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
			{
				dictionary["email"] = RegistryManager.Instance.RegisteredEmail;
			}
			dictionary["usage_data"] = jSONObjectString;
			if (!dictionary.ContainsKey("current_app"))
			{
				dictionary.Add("current_app", BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName);
			}
			else
			{
				dictionary["current_app"] = BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName;
			}
			string text3 = BstHttpClient.Post(text2, dictionary, (Dictionary<string, string>)null, false, string.Empty, 0, 1, 0, false, "bgp64");
			Logger.Info("real time app usage response:" + text3);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in handling usage notification" + ex.ToString());
		}
	}

	internal static void PostBootCompleted()
	{
		if (mWorkQueue != null)
		{
			mWorkQueue.Start();
		}
	}
}
