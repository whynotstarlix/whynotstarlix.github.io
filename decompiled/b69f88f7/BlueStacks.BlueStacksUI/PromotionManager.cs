using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class PromotionManager
{
	internal static Dictionary<string, long> combinedPackages = new Dictionary<string, long>();

	private static System.Timers.Timer mQuestTimer = new System.Timers.Timer(15000.0);

	private static Dictionary<string, int> mDictRecurringCount = new Dictionary<string, int>();

	private static List<string> mRuleIdAlreadyPassed = new List<string>();

	internal static Dictionary<GenericNotificationItem, long> sDeferredNotificationsList = new Dictionary<GenericNotificationItem, long>();

	internal static List<GenericNotificationItem> sPassedDeferredNotificationsList = new List<GenericNotificationItem>();

	internal static BootPromotion AddBootPromotion(JToken promoImage)
	{
		BootPromotion bootPromotion = new BootPromotion();
		string value = JsonExtensions.GetValue(promoImage, "image_url");
		bootPromotion.ImageUrl = JsonExtensions.GetValue(promoImage, "image_url");
		bootPromotion.Id = JsonExtensions.GetValue(promoImage, "id");
		string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[2] { "BootPromo", bootPromotion.Id });
		bootPromotion.Order = int.Parse(JsonExtensions.GetValue(promoImage, "order"), CultureInfo.InvariantCulture);
		if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(promoImage, "extra_payload")))
		{
			DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)bootPromotion.ExtraPayload, (Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(promoImage[(object)"extra_payload"]));
			PopulateAndDownloadFavicon((IDictionary<string, string>)bootPromotion.ExtraPayload, text + "_" + bootPromotion.Id);
		}
		bootPromotion.ButtonText = JsonExtensions.GetValue(promoImage, "button_text");
		bootPromotion.ThemeEnabled = JsonExtensions.GetValue(promoImage, "theme_enabled");
		bootPromotion.ThemeName = JsonExtensions.GetValue(promoImage, "theme_name");
		bootPromotion.ImagePath = Utils.TinyDownloader(value, text, RegistryStrings.PromotionDirectory, false);
		bootPromotion.PromoBtnClickStatusText = JsonExtensions.GetValue(promoImage, "promo_button_click_status_text");
		return bootPromotion;
	}

	internal static SearchRecommendation AddSearchRecommendation(JToken searchItem)
	{
		SearchRecommendation searchRecommendation = new SearchRecommendation
		{
			IconId = JsonExtensions.GetValue(searchItem, "app_icon_id")
		};
		string value = JsonExtensions.GetValue(searchItem, "app_icon");
		string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[2] { "recommendation", searchRecommendation.IconId });
		searchRecommendation.ImagePath = Utils.TinyDownloader(value, text, RegistryStrings.PromotionDirectory, false);
		if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(searchItem, "extra_payload")))
		{
			DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)searchRecommendation.ExtraPayload, (Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(searchItem[(object)"extra_payload"]));
		}
		return searchRecommendation;
	}

	internal static void SendAppUsageStats()
	{
		string urlWithParams = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs3/stats/v4/usage");
		string value = AppUsageTimer.DecryptString(RegistryManager.Instance.AInfo);
		if (!string.IsNullOrEmpty(value))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "usage", value } };
			try
			{
				BstHttpClient.Post(urlWithParams, dictionary, (Dictionary<string, string>)null, false, string.Empty, 0, 1, 0, false, "bgp64");
				RegistryManager.Instance.AInfo = string.Empty;
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
				Logger.Error("Post failed. url = {0}", new object[1] { urlWithParams });
			}
		}
	}

	internal static string AddDiscordClientVersionInUrl(string url)
	{
		string text = string.Empty;
		string text2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Discord";
		try
		{
			text = (string)Utils.GetRegistryHKCUValue(text2, "DisplayVersion", (object)string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				text = (string)Utils.GetRegistryHKLMValue(text2, "DisplayVersion", (object)string.Empty);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("exception in getting discord client version.." + ex.ToString());
		}
		url += "&discord_version=";
		url += text;
		return url;
	}

	private static Dictionary<string, string> GetPromotionCallData()
	{
		Dictionary<string, string> data = GetInstalledAppsData();
		Dictionary<string, string> resolutionData = BlueStacksUIUtils.GetResolutionData();
		try
		{
			resolutionData.ToList().ForEach(delegate(KeyValuePair<string, string> kvp)
			{
				data[kvp.Key] = kvp.Value;
			});
			Logger.Info("RESOLUTION : " + data["resolution"]);
			Logger.Info("RESOLUTION TYPE : " + data["resolution_type"]);
		}
		catch (Exception ex)
		{
			Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
		}
		return data;
	}

	internal static Dictionary<string, string> GetInstalledAppsData()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		List<AppInfo> list = new JsonParser("Android").GetAppList().ToList();
		JArray val = new JArray();
		foreach (AppInfo item in list)
		{
			JObject val2 = new JObject();
			string package = item.Package;
			string name = item.Name;
			val2.Add(package, JToken.op_Implicit(name));
			val.Add((JToken)(object)val2);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("installed_apps", ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
		Dictionary<string, string> dictionary2 = dictionary;
		dictionary2.Add("all_installed_apps", Utils.GetInstalledAppDataFromAllVms());
		dictionary2.Add("campaign_json", RegistryManager.Instance.CampaignJson);
		dictionary2.Add("email", RegistryManager.Instance.RegisteredEmail);
		if (!string.IsNullOrEmpty(Opt.Instance.Json))
		{
			JObject val3 = JObject.Parse(Opt.Instance.Json);
			if (val3["fle_pkg"] != null)
			{
				dictionary2.Add("fle_packagename", ((object)val3["fle_pkg"]).ToString().Trim());
			}
		}
		if (RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			if (RegistryManager.Instance.IsClientUpgraded)
			{
				dictionary2.Add("first_boot_update", bool.TrueString);
			}
			else
			{
				dictionary2.Add("first_boot", bool.TrueString);
			}
		}
		try
		{
			string path = Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
			if (File.Exists(path))
			{
				string text = File.ReadAllText(path);
				List<string> list2 = new List<string>();
				if (!string.IsNullOrEmpty(text))
				{
					list2 = DoDeserialize<List<string>>(text);
				}
				val = new JArray();
				foreach (string item2 in list2)
				{
					val.Add(JToken.op_Implicit(item2));
				}
				dictionary2.Add("cross_promotion_closed_apps_list", ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Error in adding cross promotion closed app list " + ex.ToString());
			if (!dictionary2.ContainsKey("cross_promotion_closed_apps_list"))
			{
				dictionary2.Add("cross_promotion_closed_apps_list", "[]");
			}
		}
		return dictionary2;
	}

	internal static void ReloadPromotionsAsync()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			if (PromotionObject.Instance == null)
			{
				PromotionObject.LoadDataFromFile();
			}
			try
			{
				SendAppUsageStats();
				CheckIsUserPremium();
				JToken promotionData = GetPromotionData();
				if (promotionData != null)
				{
					SetBootPromotion(promotionData);
					SetDiscordId(promotionData);
					SetFeatures(promotionData);
					SetMyAppsCrossPromotion(promotionData);
					SetMyAppsBackgroundPromotion(promotionData);
					SetSearchRecommendations(promotionData);
					SetAppRecommendations(promotionData);
					SetStartupTab(promotionData);
					SetIconOrder(promotionData);
					ReadQuests(promotionData, writePromo: false);
					PopulateAppSpecificRules(promotionData);
					SetSecurityMetrics(promotionData);
					SetCustomCursorRuleForApp(promotionData);
				}
				PromotionObject.Save();
				PromotionObject.Instance.PromotionLoaded();
			}
			catch (Exception ex)
			{
				Logger.Info("Error Loading Promotions" + ex.ToString());
			}
		});
	}

	private static JToken GetPromotionData()
	{
		JToken result = null;
		try
		{
			string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
			{
				WebHelper.GetServerHost(),
				"promotions"
			}));
			urlWithParams = AddDiscordClientVersionInUrl(urlWithParams);
			urlWithParams = AddSamsungStoreParamsIfPresent(urlWithParams);
			string text = BstHttpClient.Post(urlWithParams, GetPromotionCallData(), (Dictionary<string, string>)null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
			Logger.Debug("Promotion Url: " + urlWithParams);
			Logger.Debug("Promotion Response: " + text);
			result = JToken.Parse(text);
		}
		catch (Exception ex)
		{
			Logger.Info("Error Getting PromotionData " + ex.ToString());
		}
		return result;
	}

	private static void PopulateAppSpecificRules(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "macro_rules")))
			{
				return;
			}
			foreach (JToken item in JArray.Parse(((object)res[(object)"macro_rules"]).ToString()))
			{
				PromotionObject.Instance.AppSpecificRulesList.Add(((object)item).ToString());
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in PopulateAppSpecificRules: " + ex.ToString());
		}
	}

	private static void SetSearchRecommendations(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "search_recommendations")))
			{
				foreach (KeyValuePair<string, SearchRecommendation> item in (Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations)
				{
					item.Value.DeleteFile();
				}
				DictionaryExtensions.ClearSync<string, SearchRecommendation>((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations);
				return;
			}
			SerializableDictionary<string, SearchRecommendation> tempDict = new SerializableDictionary<string, SearchRecommendation>();
			foreach (JToken item2 in ((JToken)JArray.Parse(((object)res[(object)"search_recommendations"]).ToString())).ToObject<List<JToken>>())
			{
				string value = JsonExtensions.GetValue(item2, "app_icon_id");
				if (!JsonExtensions.IsNullOrEmptyBrackets(value))
				{
					SearchRecommendation searchRecommendation = ((!((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations).ContainsKey(value)) ? AddSearchRecommendation(item2) : ((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations)[value]);
					if (searchRecommendation != null)
					{
						((Dictionary<string, SearchRecommendation>)(object)tempDict)[searchRecommendation.IconId] = searchRecommendation;
					}
				}
			}
			foreach (string item3 in from _ in ((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations).Values
				select _.ImagePath into _
				where !((Dictionary<string, SearchRecommendation>)(object)tempDict).Values.Select((SearchRecommendation x) => x.ImagePath).Contains(_)
				select _)
			{
				try
				{
					File.Delete(item3);
				}
				catch (Exception)
				{
				}
			}
			DictionaryExtensions.ClearAddRange<string, SearchRecommendation>((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations, (Dictionary<string, SearchRecommendation>)(object)tempDict);
		}
		catch (Exception ex2)
		{
			DictionaryExtensions.ClearSync<string, SearchRecommendation>((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations);
			Logger.Info("Error Loading Search Recommendations" + ex2.ToString());
		}
	}

	private static void SetAppRecommendations(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "app_recommendations")))
			{
				foreach (AppRecommendation appSuggestion in PromotionObject.Instance.AppRecommendations.AppSuggestions)
				{
					appSuggestion.DeleteFile();
				}
				PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
				return;
			}
			List<AppRecommendationSection> list = JsonConvert.DeserializeObject<List<AppRecommendationSection>>(((object)res[(object)"app_recommendations"]).ToString(), Utils.GetSerializerSettings());
			if (list != null)
			{
				foreach (AppRecommendation appSuggestion2 in list[0].AppSuggestions)
				{
					if (!JsonExtensions.IsNullOrEmptyBrackets(appSuggestion2.IconId))
					{
						string icon = appSuggestion2.Icon;
						string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[2] { "AppRecommendation", appSuggestion2.IconId });
						appSuggestion2.ImagePath = Utils.TinyDownloader(icon, text, RegistryStrings.PromotionDirectory, false);
					}
				}
			}
			PromotionObject.Instance.AppRecommendations = list[0];
		}
		catch (Exception ex)
		{
			PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
			Logger.Info("Error Loading App Recommendations" + ex.ToString());
		}
	}

	private static void SetStartupTab(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "startup_tab")))
			{
				DictionaryExtensions.ClearSync<string, string>((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab);
			}
			else if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res[(object)"startup_tab"], "extra_payload")))
			{
				DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab, (Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(res[(object)"startup_tab"][(object)"extra_payload"]));
				PopulateAndDownloadFavicon((IDictionary<string, string>)PromotionObject.Instance.StartupTab, "startup_favicon");
			}
		}
		catch (Exception ex)
		{
			DictionaryExtensions.ClearSync<string, string>((Dictionary<string, string>)(object)PromotionObject.Instance.StartupTab);
			Logger.Error("Exception while setting the startup tab. " + ex.ToString());
		}
	}

	public static void PopulateAndDownloadFavicon(IDictionary<string, string> payload, string id, bool redownload = false)
	{
		if (payload.ContainsKey("click_action_app_icon_id"))
		{
			id += payload["click_action_app_icon_id"];
		}
		if (payload.ContainsKey("click_action_app_icon_url"))
		{
			string value = Utils.TinyDownloader(payload["click_action_app_icon_url"], id, RegistryStrings.PromotionDirectory, redownload);
			if (!string.IsNullOrEmpty(value))
			{
				payload["icon_path"] = value;
			}
		}
	}

	private static void SetIconOrder(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "order")))
			{
				PromotionObject.Instance.SetDefaultOrder();
				return;
			}
			SetMyAppsOrder(res[(object)"order"]);
			SetDockOrder(res[(object)"order"]);
			SetMoreAppsOrder(res[(object)"order"]);
		}
		catch (Exception ex)
		{
			PromotionObject.Instance.SetDefaultOrder(overwrite: false);
			Logger.Info("Error Loading icon order" + ex.ToString());
		}
	}

	private static void SetMoreAppsOrder(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "more_apps_order")))
			{
				PromotionObject.Instance.SetDefaultMoreAppsOrder();
				return;
			}
			DictionaryExtensions.ClearSync<string, int>((Dictionary<string, int>)(object)PromotionObject.Instance.MoreAppsDockOrder);
			foreach (KeyValuePair<string, int> item in (Dictionary<string, int>)(object)JsonExtensions.ToSerializableDictionary<int>(res[(object)"more_apps_order"]))
			{
				((Dictionary<string, int>)(object)PromotionObject.Instance.MoreAppsDockOrder)[item.Key] = item.Value;
			}
		}
		catch (Exception ex)
		{
			PromotionObject.Instance.SetDefaultMoreAppsOrder();
			Logger.Info("Error Loading more_apps_order" + ex.ToString());
		}
	}

	private static void SetMyAppsOrder(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "myapps_order")))
			{
				PromotionObject.Instance.SetDefaultMyAppsOrder();
				return;
			}
			DictionaryExtensions.ClearSync<string, int>((Dictionary<string, int>)(object)PromotionObject.Instance.MyAppsOrder);
			foreach (KeyValuePair<string, int> item in (Dictionary<string, int>)(object)JsonExtensions.ToSerializableDictionary<int>(res[(object)"myapps_order"]))
			{
				((Dictionary<string, int>)(object)PromotionObject.Instance.MyAppsOrder)[item.Key] = item.Value;
			}
		}
		catch (Exception ex)
		{
			PromotionObject.Instance.SetDefaultMyAppsOrder();
			Logger.Info("Error Loading My apps order" + ex.ToString());
		}
	}

	private static void SetDiscordId(JToken res)
	{
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "discord_client_id")))
			{
				PromotionObject.Instance.DiscordClientID = JsonExtensions.GetValue(res, "discord_client_id");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while getting discord id : {0}", new object[1] { ex.ToString() });
		}
	}

	private static void SetFeatures(JToken res)
	{
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "is_root_access_enabled")))
			{
				PromotionObject.Instance.IsRootAccessEnabled = res[(object)"is_root_access_enabled"].ToObject<bool>();
			}
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "is_timeline_stats4_enabled")))
			{
				RegistryManager.Instance.IsTimelineStats4Enabled = res[(object)"is_timeline_stats4_enabled"].ToObject<bool>();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in SetFeatures: {0}", new object[1] { ex });
		}
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "geo")))
			{
				string text = ((object)res[(object)"geo"]).ToString();
				if (!string.IsNullOrEmpty(text))
				{
					RegistryManager.Instance.Geo = text;
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Error while getting geo feature: {0}", new object[1] { ex2 });
		}
	}

	private static void SetDockOrder(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "dock_order")))
			{
				PromotionObject.Instance.SetDefaultDockOrder();
				return;
			}
			DictionaryExtensions.ClearSync<string, int>((Dictionary<string, int>)(object)PromotionObject.Instance.DockOrder);
			JToken obj = res[(object)"dock_order"];
			SerializableDictionary<string, int> val = ((obj != null) ? JsonExtensions.ToSerializableDictionary<int>(obj) : null);
			if (val != null && ((Dictionary<string, int>)(object)val).Count > 0)
			{
				foreach (KeyValuePair<string, int> item in (Dictionary<string, int>)(object)val)
				{
					((Dictionary<string, int>)(object)PromotionObject.Instance.DockOrder)[item.Key] = item.Value;
				}
				return;
			}
			PromotionObject.Instance.SetDefaultDockOrder();
		}
		catch (Exception ex)
		{
			PromotionObject.Instance.SetDefaultDockOrder();
			Logger.Info("Error Loading dock order" + ex.ToString());
		}
	}

	private static void SetBootPromotion(JToken res)
	{
		try
		{
			if (JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "boot_promotion_obj")))
			{
				foreach (KeyValuePair<string, BootPromotion> item in (Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions)
				{
					item.Value.DeleteFile();
				}
				DictionaryExtensions.ClearSync<string, BootPromotion>((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions);
			}
			else
			{
				JToken val = JToken.Parse(JsonExtensions.GetValue(res, "boot_promotion_obj"));
				if (val[(object)"boot_promotion_display_time"] != null)
				{
					PromotionObject.Instance.BootPromoDisplaytime = val[(object)"boot_promotion_display_time"].ToObject<int>();
				}
				SerializableDictionary<string, BootPromotion> val2 = new SerializableDictionary<string, BootPromotion>();
				foreach (JToken item2 in JArray.Parse(((object)val[(object)"boot_promotion_images"]).ToString()))
				{
					string value = JsonExtensions.GetValue(item2, "id");
					if (!JsonExtensions.IsNullOrEmptyBrackets(value))
					{
						BootPromotion bootPromotion = ((!((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions).ContainsKey(value)) ? AddBootPromotion(item2) : ((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions)[value]);
						if (bootPromotion != null)
						{
							((Dictionary<string, BootPromotion>)(object)val2)[bootPromotion.Id] = bootPromotion;
						}
					}
				}
				DictionaryExtensions.ClearAddRange<string, BootPromotion>((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions, (Dictionary<string, BootPromotion>)(object)val2);
			}
		}
		catch (Exception ex)
		{
			DictionaryExtensions.ClearSync<string, BootPromotion>((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions);
			Logger.Info("Error Loading Boot Promotions" + ex.ToString());
		}
		PromotionObject.mIsBootPromotionLoading = false;
		PromotionObject.BootPromotionHandler?.Invoke(PromotionObject.Instance, new EventArgs());
		try
		{
			foreach (KeyValuePair<string, BootPromotion> item3 in (Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictOldBootPromotions)
			{
				if (!((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions).ContainsKey(item3.Key))
				{
					item3.Value.DeleteFile();
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Warning("Error Loading myapp cross Promotions" + ex2.ToString());
		}
	}

	private static void SetMyAppsBackgroundPromotion(JToken res)
	{
		bool flag = false;
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "myapps_background_id")))
			{
				if (!string.Equals(PromotionObject.Instance.BackgroundPromotionID, JsonExtensions.GetValue(res, "myapps_background_id"), StringComparison.InvariantCulture))
				{
					PromotionObject.Instance.BackgroundPromotionID = JsonExtensions.GetValue(res, "myapps_background_id");
					PromotionObject.Instance.BackgroundPromotionImagePath = Utils.TinyDownloader(JsonExtensions.GetValue(res, "myapps_background_url"), "BackPromo", RegistryStrings.PromotionDirectory, true);
				}
			}
			else
			{
				flag = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Error Loading myapp background Promotions" + ex.ToString());
			flag = true;
		}
		if (flag)
		{
			PromotionObject.Instance.BackgroundPromotionID = "";
			PromotionObject.Instance.BackgroundPromotionImagePath = "";
			IOUtils.DeleteIfExists((IEnumerable<string>)new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "BackPromo") });
		}
	}

	internal static void SetMyAppsCrossPromotion(JToken res)
	{
		bool flag = false;
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "myapps_cross_promotion")))
			{
				List<AppSuggestionPromotion> list = res[(object)"myapps_cross_promotion"].ToObject<IEnumerable<AppSuggestionPromotion>>().ToList();
				if (list == null)
				{
					list = new List<AppSuggestionPromotion>();
				}
				else
				{
					foreach (JToken x in JArray.Parse(((object)res[(object)"myapps_cross_promotion"]).ToString()))
					{
						if (x[(object)"extra_payload"] != null && x[(object)"app_icon_id"] != null)
						{
							DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)list.Where((AppSuggestionPromotion _) => _.AppIconId == ((object)x[(object)"app_icon_id"]).ToString()).First().ExtraPayload, (Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(x[(object)"extra_payload"]));
						}
					}
				}
				lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
				{
					foreach (AppSuggestionPromotion item in PromotionObject.Instance.AppSuggestionList)
					{
						if (!list.Any((AppSuggestionPromotion _) => string.Equals(_.AppIconId, item.AppIconId, StringComparison.InvariantCulture)))
						{
							IOUtils.DeleteIfExists((IEnumerable<string>)new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + item.AppIconId) });
							DeleteFavicon((IDictionary<string, string>)item.ExtraPayload);
						}
						if (!list.Any((AppSuggestionPromotion _) => string.Equals(_.IconBorderId, item.IconBorderId, StringComparison.InvariantCulture)))
						{
							IOUtils.DeleteIfExists((IEnumerable<string>)new List<string>
							{
								Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border.png"),
								Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_hover.png"),
								Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_click.png")
							});
						}
					}
					ListExtensions.ClearAddRange<AppSuggestionPromotion>(PromotionObject.Instance.AppSuggestionList, list);
					foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
					{
						appSuggestion.AppIconPath = Utils.TinyDownloader(appSuggestion.AppIcon, "AppSuggestion" + appSuggestion.AppIconId, RegistryStrings.PromotionDirectory, false);
						if (!string.IsNullOrEmpty(appSuggestion.IconBorderId) && appSuggestion.IsIconBorder)
						{
							Utils.TinyDownloader(appSuggestion.IconBorderUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
							Utils.TinyDownloader(appSuggestion.IconBorderHoverUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
							Utils.TinyDownloader(appSuggestion.IconBorderClickUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
						}
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error Loading myapp cross Promotions" + ex.ToString());
			flag = true;
		}
		if (flag)
		{
			lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
			{
				ListExtensions.ClearSync<AppSuggestionPromotion>(PromotionObject.Instance.AppSuggestionList);
			}
			flag = false;
		}
	}

	private static void DeleteFavicon(IDictionary<string, string> payload)
	{
		if (payload.ContainsKey("favicon_path"))
		{
			IOUtils.DeleteIfExists((IEnumerable<string>)new List<string> { payload["favicon_path"] });
		}
	}

	internal static void ReadQuests(JToken res, bool writePromo)
	{
		bool flag = false;
		SerializableDictionary<string, long[]> val = new SerializableDictionary<string, long[]>();
		SerializableDictionary<string, long> val2 = new SerializableDictionary<string, long>();
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "quest")))
			{
				PromotionObject.Instance.QuestName = JsonExtensions.GetValue(res[(object)"quest"], "quest_name");
				PromotionObject.Instance.QuestActionType = JsonExtensions.GetValue(res[(object)"quest"], "action_type");
				List<QuestRule> list = res[(object)"quest"][(object)"details"].ToObject<IEnumerable<QuestRule>>().ToList();
				foreach (QuestRule rule in list)
				{
					if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, rule.RuleId, StringComparison.InvariantCulture)))
					{
						if (!((Dictionary<string, long>)(object)val2).ContainsKey(rule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
						{
							((Dictionary<string, long>)(object)val2)[rule.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
						}
						if (((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules).ContainsKey(rule.RuleId))
						{
							((Dictionary<string, long[]>)(object)val).Add(rule.RuleId, ((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[rule.RuleId]);
						}
						continue;
					}
					((Dictionary<string, long>)(object)val2)[rule.AppPackage] = 0L;
					long totalTimeForPackageAcrossInstances = AppUsageTimer.GetTotalTimeForPackageAcrossInstances(rule.AppPackage);
					long num = 0L;
					if (combinedPackages.ContainsKey(rule.AppPackage))
					{
						num = combinedPackages[rule.AppPackage];
					}
					((Dictionary<string, long[]>)(object)val).Add(rule.RuleId, new long[2] { num, totalTimeForPackageAcrossInstances });
				}
				ListExtensions.ClearAddRange<QuestRule>(PromotionObject.Instance.QuestRules, list);
				DictionaryExtensions.ClearAddRange<string, long[]>((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules, (Dictionary<string, long[]>)(object)val);
				DictionaryExtensions.ClearAddRange<string, long>((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules, (Dictionary<string, long>)(object)val2);
			}
			else
			{
				flag = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error Loading promotion quests" + ex.ToString());
			flag = true;
		}
		if (flag)
		{
			PromotionObject.Instance.QuestName = "";
			PromotionObject.Instance.QuestActionType = "";
			ListExtensions.ClearSync<QuestRule>(PromotionObject.Instance.QuestRules);
			DictionaryExtensions.ClearSync<string, long>((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules);
			flag = false;
		}
		if (writePromo)
		{
			PromotionObject.Save();
			PromotionObject.QuestHandler?.Invoke();
		}
	}

	private static T DoDeserialize<T>(string data) where T : class
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		XmlReader val = XmlReader.Create((Stream)new MemoryStream(Encoding.UTF8.GetBytes(data)));
		try
		{
			return (T)new XmlSerializer(typeof(T)).Deserialize(val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal static void AddNewMyAppsCrossPromotion(JToken res)
	{
		try
		{
			AppSuggestionPromotion appSuggestionPromotion = res[(object)"myapps_cross_promotion"].ToObject<AppSuggestionPromotion>();
			if (appSuggestionPromotion != null && res[(object)"myapps_cross_promotion"][(object)"extra_payload"] != null && res[(object)"myapps_cross_promotion"][(object)"app_icon_id"] != null)
			{
				DictionaryExtensions.ClearAddRange<string, string>((Dictionary<string, string>)(object)appSuggestionPromotion.ExtraPayload, (Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(res[(object)"myapps_cross_promotion"][(object)"extra_payload"]));
				PopulateAndDownloadFavicon((IDictionary<string, string>)appSuggestionPromotion.ExtraPayload, "AppSuggestion");
			}
			List<AppSuggestionPromotion> list = new List<AppSuggestionPromotion>();
			lock (((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot)
			{
				foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
				{
					if (string.Equals(appSuggestionPromotion.AppIconId, appSuggestion.AppIconId, StringComparison.InvariantCulture))
					{
						list.Add(appSuggestion);
						IOUtils.DeleteIfExists((IEnumerable<string>)new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + appSuggestion.AppIconId) });
					}
				}
				foreach (AppSuggestionPromotion item in list)
				{
					PromotionObject.Instance.AppSuggestionList.Remove(item);
				}
				appSuggestionPromotion.AppIconPath = Utils.TinyDownloader(appSuggestionPromotion.AppIcon, "AppSuggestion" + appSuggestionPromotion.AppIconId, RegistryStrings.PromotionDirectory, false);
				if (!string.IsNullOrEmpty(appSuggestionPromotion.IconBorderId) && appSuggestionPromotion.IsIconBorder)
				{
					Utils.TinyDownloader(appSuggestionPromotion.IconBorderUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
					Utils.TinyDownloader(appSuggestionPromotion.IconBorderHoverUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
					Utils.TinyDownloader(appSuggestionPromotion.IconBorderClickUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
				}
				PromotionObject.Instance.AppSuggestionList.Add(appSuggestionPromotion);
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error Loading myapp cross Promotions by notification: " + ex.ToString());
		}
	}

	internal static void CheckIsUserPremium()
	{
		string registeredEmail = RegistryManager.Instance.RegisteredEmail;
		string token = RegistryManager.Instance.Token;
		string userGuid = RegistryManager.Instance.UserGuid;
		string version = RegistryManager.Instance.Version;
		string clientVersion = RegistryManager.Instance.ClientVersion;
		string text = "bgp64";
		if (string.IsNullOrEmpty(registeredEmail) || string.IsNullOrEmpty(token))
		{
			return;
		}
		string text2 = string.Format(CultureInfo.InvariantCulture, "{0}/bs-accounts/getuser?email={1}&guid={2}&token={3}&eng_ver={4}&client_ver={5}&oem={6}", new object[7]
		{
			RegistryManager.Instance.Host,
			registeredEmail,
			userGuid,
			token,
			version,
			clientVersion,
			text
		});
		string text3;
		while (true)
		{
			try
			{
				text3 = BstHttpClient.Get(text2, (Dictionary<string, string>)null, false, "", 0, 1, 0, false, "bgp64");
			}
			catch
			{
				Thread.Sleep(20000);
				continue;
			}
			break;
		}
		Logger.Debug("Response string from cloud for bs-accounts/getuser : " + text3);
		try
		{
			JObject val = JObject.Parse(text3);
			if (string.Equals(((object)val["status"]).ToString().Trim(), "success", StringComparison.InvariantCulture))
			{
				RegistryManager.Instance.RegisteredEmail = ((object)val["message"][(object)"email"]).ToString().Trim();
				if (string.Compare(((object)val["message"][(object)"subscription_status"]).ToString().Trim(), "PAID", StringComparison.OrdinalIgnoreCase) == 0)
				{
					RegistryManager.Instance.IsPremium = true;
				}
				else
				{
					RegistryManager.Instance.IsPremium = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to parse string received from cloud... Err : " + ex.ToString());
		}
	}

	internal static void StartQuestRulesProcessor()
	{
		foreach (QuestRule questRule in PromotionObject.Instance.QuestRules)
		{
			if (questRule.IsRecurring)
			{
				if (!mDictRecurringCount.ContainsKey(questRule.RuleId))
				{
					mDictRecurringCount.Add(questRule.RuleId, questRule.RecurringCount);
				}
				else
				{
					mDictRecurringCount[questRule.RuleId] = questRule.RecurringCount;
				}
			}
		}
		if (mQuestTimer.Enabled)
		{
			if (((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules).Count == 0)
			{
				mQuestTimer.Stop();
			}
		}
		else if (((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules).Count > 0)
		{
			mQuestTimer.Elapsed -= QuestTimer_Elapsed;
			mQuestTimer.Elapsed += QuestTimer_Elapsed;
			mQuestTimer.Start();
		}
	}

	private static void QuestTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		try
		{
			combinedPackages.Clear();
			foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
			{
				string text = item.mFrontendHandler.SendFrontendRequest("getInteractionForPackage");
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				Logger.Debug("Package interaction Json received from frontend: " + text);
				foreach (KeyValuePair<string, long> item2 in JsonExtensions.ToDictionary<long>(JToken.Parse(text)) as Dictionary<string, long>)
				{
					if (combinedPackages.ContainsKey(item2.Key))
					{
						combinedPackages[item2.Key] += item2.Value;
					}
					else
					{
						combinedPackages.Add(item2.Key, item2.Value);
					}
				}
			}
			List<QuestRuleState> list = new List<QuestRuleState>();
			Dictionary<string, long> dictionary = new Dictionary<string, long>();
			List<QuestRule> list2 = new List<QuestRule>();
			string packageName = string.Empty;
			foreach (QuestRule item3 in PromotionObject.Instance.QuestRules.Where((QuestRule _) => !mRuleIdAlreadyPassed.Contains(_.RuleId)))
			{
				if (!combinedPackages.ContainsKey(item3.AppPackage.ToLower(CultureInfo.InvariantCulture)))
				{
					continue;
				}
				if (combinedPackages.ContainsKey("?"))
				{
					foreach (KeyValuePair<string, long> combinedPackage in combinedPackages)
					{
						if (!combinedPackage.Key.ToString(CultureInfo.InvariantCulture).Equals("?", StringComparison.OrdinalIgnoreCase))
						{
							packageName = combinedPackage.Key.ToString(CultureInfo.InvariantCulture);
						}
					}
				}
				if (item3.MinUserInteraction <= combinedPackages[item3.AppPackage.ToLower(CultureInfo.InvariantCulture)] - ((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item3.RuleId][0])
				{
					list2.Add(item3);
					if (dictionary.ContainsKey(item3.AppPackage))
					{
						dictionary[item3.AppPackage] = combinedPackages[item3.AppPackage.ToLower(CultureInfo.InvariantCulture)];
					}
					else
					{
						dictionary.Add(item3.AppPackage, combinedPackages[item3.AppPackage.ToLower(CultureInfo.InvariantCulture)]);
					}
					Logger.Debug("Interaction rule passed for package " + item3.AppPackage + combinedPackages[item3.AppPackage.ToLower(CultureInfo.InvariantCulture)]);
				}
			}
			foreach (QuestRule item4 in list2.Where((QuestRule _) => !mRuleIdAlreadyPassed.Contains(_.RuleId)))
			{
				QuestRuleState questRuleState = new QuestRuleState();
				if (string.Equals(item4.AppPackage, "*", StringComparison.InvariantCulture))
				{
					long totalTimeForAllPackages = AppUsageTimer.GetTotalTimeForAllPackages();
					if (item4.AppUsageTime <= totalTimeForAllPackages - ((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1])
					{
						questRuleState.TotalTime = totalTimeForAllPackages;
						questRuleState.QuestRules = item4;
						questRuleState.Interaction = dictionary[item4.AppPackage];
						list.Add(questRuleState);
						if (combinedPackages.ContainsKey(item4.AppPackage.ToLower(CultureInfo.InvariantCulture)))
						{
							((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][0] = combinedPackages[item4.AppPackage.ToLower(CultureInfo.InvariantCulture)];
							((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1] = questRuleState.TotalTime;
						}
					}
					continue;
				}
				if (string.Equals(item4.AppPackage, "?", StringComparison.InvariantCulture))
				{
					long totalTimeForPackageAfterReset = AppUsageTimer.GetTotalTimeForPackageAfterReset(packageName);
					if (item4.AppUsageTime <= totalTimeForPackageAfterReset - ((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1])
					{
						questRuleState.TotalTime = totalTimeForPackageAfterReset;
						questRuleState.QuestRules = item4;
						questRuleState.Interaction = dictionary[item4.AppPackage];
						list.Add(questRuleState);
						if (combinedPackages.ContainsKey(item4.AppPackage.ToLower(CultureInfo.InvariantCulture)))
						{
							((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][0] = combinedPackages[item4.AppPackage.ToLower(CultureInfo.InvariantCulture)];
							((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1] = questRuleState.TotalTime;
						}
					}
					continue;
				}
				long totalTimeForPackageAfterReset2 = AppUsageTimer.GetTotalTimeForPackageAfterReset(item4.AppPackage.ToLower(CultureInfo.InvariantCulture));
				if (item4.AppUsageTime <= totalTimeForPackageAfterReset2 - ((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1])
				{
					questRuleState.TotalTime = totalTimeForPackageAfterReset2;
					questRuleState.QuestRules = item4;
					questRuleState.Interaction = dictionary[item4.AppPackage];
					list.Add(questRuleState);
					if (combinedPackages.ContainsKey(item4.AppPackage.ToLower(CultureInfo.InvariantCulture)))
					{
						((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][0] = combinedPackages[item4.AppPackage.ToLower(CultureInfo.InvariantCulture)];
						((Dictionary<string, long[]>)(object)PromotionObject.Instance.ResetQuestRules)[item4.RuleId][1] = questRuleState.TotalTime;
					}
				}
			}
			if (list.Count > 0)
			{
				SerializableDictionary<string, long> val = new SerializableDictionary<string, long>();
				bool flag = false;
				foreach (QuestRule questRule in PromotionObject.Instance.QuestRules)
				{
					((Dictionary<string, long>)(object)val)[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
				}
				foreach (QuestRuleState ruleState in list)
				{
					string text2 = ruleState.QuestRules.CloudHandler;
					string jSONObjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
					if (string.IsNullOrEmpty(text2))
					{
						text2 = "/pika_points/quest_rule_accomplished";
					}
					string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[2]
					{
						WebHelper.GetServerHost(),
						text2
					}));
					urlWithParams += string.Format(CultureInfo.InvariantCulture, "&email={5}&quest_name={0}&rule_id={1}&app_pkg={2}&usage_time={3}&user_interactions={4}&usage_data={6}", new object[7]
					{
						PromotionObject.Instance.QuestName,
						ruleState.QuestRules.RuleId,
						ruleState.QuestRules.AppPackage,
						ruleState.TotalTime,
						ruleState.Interaction,
						RegistryManager.Instance.RegisteredEmail,
						jSONObjectString
					});
					int num = 3;
					string text3 = "";
					while (num > 0)
					{
						try
						{
							text3 = BstHttpClient.Get(urlWithParams, (Dictionary<string, string>)null, false, string.Empty, 5000, 1, 0, false, "bgp64");
							Logger.Info("Quest rule passed response from cloud " + text3 + " ruleId: " + ruleState.QuestRules.RuleId);
						}
						catch (Exception ex)
						{
							Logger.Warning("Exception while calling cloud for quest rule passed, RETRYING " + num + Environment.NewLine + ex.ToString());
							num--;
							Thread.Sleep(1000);
							continue;
						}
						break;
					}
					if (num == 0)
					{
						Logger.Error("Could not send quest rule passed, to cloud after retries.");
					}
					if (!ruleState.QuestRules.IsRecurring)
					{
						mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
						continue;
					}
					if (mDictRecurringCount[ruleState.QuestRules.RuleId] == -1)
					{
						if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
						{
							AppUsageTimer.GetTotalTimeForAllPackages();
						}
						else
						{
							AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
						}
						if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture)) && ((Dictionary<string, long>)(object)val).ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
						{
							if (ruleState.QuestRules.RecurringCount != -1)
							{
								ruleState.QuestRules.RecurringCount--;
							}
							((Dictionary<string, long>)(object)val)[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
							flag = true;
						}
						continue;
					}
					mDictRecurringCount[ruleState.QuestRules.RuleId] = mDictRecurringCount[ruleState.QuestRules.RuleId] - 1;
					if (mDictRecurringCount[ruleState.QuestRules.RuleId] == 0)
					{
						mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
					}
					else
					{
						if (mDictRecurringCount[ruleState.QuestRules.RuleId] <= 0)
						{
							continue;
						}
						if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
						{
							long totalTimeForAllPackages2 = AppUsageTimer.GetTotalTimeForAllPackages();
							AppUsageTimer.AddPackageForReset("*", totalTimeForAllPackages2);
						}
						else
						{
							long totalTimeForPackageAfterReset3 = AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
							AppUsageTimer.AddPackageForReset(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture), totalTimeForPackageAfterReset3);
						}
						if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture)) && ((Dictionary<string, long>)(object)val).ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
						{
							if (ruleState.QuestRules.RecurringCount != -1)
							{
								ruleState.QuestRules.RecurringCount--;
							}
							((Dictionary<string, long>)(object)val)[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
							flag = true;
						}
					}
				}
				if (flag)
				{
					DictionaryExtensions.ClearAddRange<string, long>((Dictionary<string, long>)(object)PromotionObject.Instance.QuestHdPlayerRules, (Dictionary<string, long>)(object)val);
					PromotionObject.Save();
					PromotionObject.QuestHandler?.Invoke();
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in QuestTimer_Elapsed " + ex2.ToString());
		}
		try
		{
			List<GenericNotificationItem> list3 = new List<GenericNotificationItem>();
			foreach (KeyValuePair<GenericNotificationItem, long> sDeferredNotifications in sDeferredNotificationsList)
			{
				if (AppUsageTimer.GetTotalTimeForPackageAfterReset(sDeferredNotifications.Key.DeferredApp.ToLower(CultureInfo.InvariantCulture)) - sDeferredNotifications.Value < sDeferredNotifications.Key.DeferredAppUsage)
				{
					continue;
				}
				if (string.Equals(BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.mAppTabButtons.SelectedTab.PackageName, sDeferredNotifications.Key.DeferredApp, StringComparison.InvariantCulture))
				{
					BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(sDeferredNotifications.Key);
					GenericNotificationManager.AddNewNotification(sDeferredNotifications.Key);
					((DispatcherObject)BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName]).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton();
					}, new object[0]);
					list3.Add(sDeferredNotifications.Key);
				}
				else
				{
					sPassedDeferredNotificationsList.Add(sDeferredNotifications.Key);
				}
			}
			foreach (GenericNotificationItem item5 in list3)
			{
				sDeferredNotificationsList.Remove(item5);
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("Exception in checking deferred notification: " + ex3.ToString());
		}
	}

	private static void SetSecurityMetrics(JToken res)
	{
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "security_metrics_enable_user")))
			{
				PromotionObject.Instance.IsSecurityMetricsEnable = res[(object)"security_metrics_enable_user"].ToObject<bool>();
			}
			else
			{
				PromotionObject.Instance.IsSecurityMetricsEnable = false;
			}
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "security_metrics_blacklisted_apps")))
			{
				ListExtensions.ClearSync<string>(PromotionObject.Instance.BlackListedApplicationsList);
				{
					foreach (JToken item in JArray.Parse(((object)res[(object)"security_metrics_blacklisted_apps"]).ToString()))
					{
						PromotionObject.Instance.BlackListedApplicationsList.Add(((object)item).ToString());
					}
					return;
				}
			}
			ListExtensions.ClearSync<string>(PromotionObject.Instance.BlackListedApplicationsList);
		}
		catch (Exception ex)
		{
			Logger.Error("Error while getting security metrics info: {0}", new object[1] { ex.ToString() });
			PromotionObject.Instance.IsSecurityMetricsEnable = false;
			ListExtensions.ClearSync<string>(PromotionObject.Instance.BlackListedApplicationsList);
		}
	}

	private static void SetCustomCursorRuleForApp(JToken res)
	{
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "exclude_custom_cursor")))
			{
				ListExtensions.ClearSync<string>(PromotionObject.Instance.CustomCursorExcludedAppsList);
				{
					foreach (JToken item in JArray.Parse(((object)res[(object)"exclude_custom_cursor"]).ToString()))
					{
						PromotionObject.Instance.CustomCursorExcludedAppsList.Add(((object)item).ToString());
					}
					return;
				}
			}
			ListExtensions.ClearSync<string>(PromotionObject.Instance.CustomCursorExcludedAppsList);
		}
		catch (Exception ex)
		{
			Logger.Error("Error while getting custom cursor exclude list of apps: {0}", new object[1] { ex });
		}
	}

	private static string AddSamsungStoreParamsIfPresent(string url)
	{
		try
		{
			url = url + "&samsung_store_present=" + RegistryManager.Instance.IsSamsungStorePresent.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to add samsung store parameter. Ex : " + ex.ToString());
		}
		return url;
	}
}
