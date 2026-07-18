using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal sealed class GuidanceCloudInfoManager
{
	private static GuidanceCloudInfoManager sInstance = null;

	private static readonly object sLock = new object();

	private const string sGuidanceCloudInfoFilename = "bst_guidance";

	internal GuidanceCloudInfo mGuidanceCloudInfo = new GuidanceCloudInfo();

	private static string BstGuidanceFilePath => Path.Combine(RegistryStrings.PromotionDirectory, "bst_guidance");

	public static GuidanceCloudInfoManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (sLock)
				{
					if (sInstance == null)
					{
						sInstance = new GuidanceCloudInfoManager();
					}
				}
			}
			return sInstance;
		}
		set
		{
			sInstance = value;
		}
	}

	private GuidanceCloudInfoManager()
	{
	}

	private static JToken GetGuidanceCloudInfoData()
	{
		JToken result = null;
		try
		{
			string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
			{
				RegistryManager.Instance.Host,
				"bs4",
				"guidance_window"
			}));
			string text = BstHttpClient.Post(urlWithParams, new Dictionary<string, string> { 
			{
				"app_pkgs",
				GetInstalledAppDataFromAllVms()
			} }, (Dictionary<string, string>)null, false, "Android", 0, 1, 0, false, "bgp64");
			Logger.Debug("Guidance Cloud Info Url: " + urlWithParams);
			Logger.Debug("Guidance Cloud Info Response: " + text);
			result = JToken.Parse(text);
		}
		catch (Exception ex)
		{
			Logger.Warning("Error Getting GetGuidanceCloudInfoData " + ex.ToString());
		}
		return result;
	}

	private static string GetInstalledAppDataFromAllVms()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		string[] vmList = RegistryManager.Instance.VmList;
		JArray val = new JArray();
		try
		{
			string[] array = vmList;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (AppInfo item in new JsonParser(array[i]).GetAppList().ToList())
				{
					string package = item.Package;
					val.Add(JToken.op_Implicit(package));
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in getting all installed apps from all Vms: {0}", new object[1] { ex.ToString() });
		}
		return ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
	}

	internal void AppsGuidanceCloudInfoRefresh()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			if (File.Exists(BstGuidanceFilePath))
			{
				mGuidanceCloudInfo = JsonConvert.DeserializeObject<GuidanceCloudInfo>(File.ReadAllText(BstGuidanceFilePath), Utils.GetSerializerSettings());
			}
			JToken guidanceCloudInfoData = GetGuidanceCloudInfoData();
			if (guidanceCloudInfoData != null)
			{
				GuidanceCloudInfo guidanceCloudInfo = new GuidanceCloudInfo();
				SetAppsVideoThumbnail(guidanceCloudInfo, guidanceCloudInfoData);
				SetAppsReadArticle(guidanceCloudInfo, guidanceCloudInfoData);
				SetGameSettings(guidanceCloudInfo, guidanceCloudInfoData);
				SaveToFile(guidanceCloudInfo);
				mGuidanceCloudInfo = guidanceCloudInfo;
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void SaveToFile(GuidanceCloudInfo guidanceCloudInfo)
	{
		try
		{
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)1;
			string contents = JsonConvert.SerializeObject((object)guidanceCloudInfo, serializerSettings);
			if (!Directory.Exists(RegistryStrings.PromotionDirectory))
			{
				Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
			}
			File.WriteAllText(BstGuidanceFilePath, contents);
		}
		catch (Exception)
		{
			Logger.Warning("Error in saving GuidanceCloudInfo to file");
		}
	}

	private static void SetAppsVideoThumbnail(GuidanceCloudInfo currentAppsGuidanceCloudInfo, JToken res)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (JToken item in JArray.Parse(JsonExtensions.GetValue(res, "custom_thumbnails").ToString(CultureInfo.InvariantCulture)))
			{
				CustomThumbnail customThumbnail = JsonConvert.DeserializeObject<CustomThumbnail>(((object)item).ToString(), Utils.GetSerializerSettings());
				foreach (GuidanceVideoType value2 in Enum.GetValues(typeof(GuidanceVideoType)))
				{
					if ((int)value2 == 5)
					{
						foreach (KeyValuePair<string, VideoThumbnailInfo> item2 in (Dictionary<string, VideoThumbnailInfo>)customThumbnail[((object)value2/*cast due to constrained. prefix*/).ToString()])
						{
							VideoThumbnailInfo value = item2.Value;
							value.ThumbnailType = value2;
							value.ImagePath = Utils.TinyDownloader(value.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + value.ThumbnailId, RegistryStrings.PromotionDirectory, false);
						}
					}
					else if (customThumbnail[((object)value2/*cast due to constrained. prefix*/).ToString()] != null)
					{
						VideoThumbnailInfo videoThumbnailInfo = (VideoThumbnailInfo)customThumbnail[((object)value2/*cast due to constrained. prefix*/).ToString()];
						videoThumbnailInfo.ThumbnailType = value2;
						videoThumbnailInfo.ImagePath = Utils.TinyDownloader(videoThumbnailInfo.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + videoThumbnailInfo.ThumbnailId, RegistryStrings.PromotionDirectory, false);
					}
				}
				currentAppsGuidanceCloudInfo.CustomThumbnails[customThumbnail.Package] = customThumbnail;
			}
			foreach (JToken item3 in JArray.Parse(JsonExtensions.GetValue(res, "default_thumbnails").ToString(CultureInfo.InvariantCulture)))
			{
				VideoThumbnailInfo videoThumbnailInfo2 = JsonConvert.DeserializeObject<VideoThumbnailInfo>(((object)item3).ToString(), Utils.GetSerializerSettings());
				videoThumbnailInfo2.ImagePath = Utils.TinyDownloader(videoThumbnailInfo2.ThumbnailUrl, "VideoThumbnail_DefaultPackage_" + videoThumbnailInfo2.ThumbnailId, RegistryStrings.PromotionDirectory, false);
				currentAppsGuidanceCloudInfo.DefaultThumbnails[videoThumbnailInfo2.ThumbnailType] = videoThumbnailInfo2;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Error Loading Apps VideoThumbnail" + ex.ToString());
		}
	}

	private static void SetAppsReadArticle(GuidanceCloudInfo currentAppsGuidanceCloudInfo, JToken res)
	{
		try
		{
			foreach (JToken item in JArray.Parse(JsonExtensions.GetValue(res, "help_article").ToString(CultureInfo.InvariantCulture)))
			{
				HelpArticle helpArticle = JsonConvert.DeserializeObject<HelpArticle>(((object)item).ToString(), Utils.GetSerializerSettings());
				currentAppsGuidanceCloudInfo.HelpArticles[helpArticle.Package] = helpArticle;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Error Loading Apps ReadArticle" + ex.ToString());
		}
	}

	private static void SetGameSettings(GuidanceCloudInfo guidanceCloudInfoDict, JToken res)
	{
		try
		{
			JToken obj = res[(object)"game_settings"];
			JArray val = (JArray)(object)((obj is JArray) ? obj : null);
			if (val == null)
			{
				return;
			}
			foreach (JToken item in val)
			{
				try
				{
					GameSetting gameSetting = new GameSetting
					{
						SettingType = Extensions.Value<string>((IEnumerable<JToken>)item[(object)"setting_type"])
					};
					JToken obj2 = item[(object)"setting_data"];
					JArray val2 = (JArray)(object)((obj2 is JArray) ? obj2 : null);
					if (val2 == null)
					{
						continue;
					}
					foreach (JToken item2 in val2)
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						JToken obj3 = item2[(object)"app_pkg_list"];
						JArray val3 = (JArray)(object)((obj3 is JArray) ? obj3 : null);
						if (val3 != null)
						{
							dictionary.Add("app_pkg_list", new AppPackageListObject(((JToken)val3).ToObject<List<string>>()));
						}
						string settingType = gameSetting.SettingType;
						if (settingType != null && settingType == "OrientationMode")
						{
							dictionary.Add("mode", Extensions.Value<string>((IEnumerable<JToken>)item2[(object)"mode"]));
							gameSetting.SettingsData.Add(dictionary);
						}
					}
					guidanceCloudInfoDict.GameSettings.Add(gameSetting);
				}
				catch (Exception ex)
				{
					Logger.Warning("Error while loading game settings from cloud data " + ex);
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Warning("Error while loading game settings from cloud data " + ex2);
		}
	}

	internal static string GetCloudOrientationForPackage(string package)
	{
		string result = string.Empty;
		if (Instance.mGuidanceCloudInfo != null && Instance.mGuidanceCloudInfo.GameSettings.Any())
		{
			GameSetting gameSetting = Instance.mGuidanceCloudInfo.GameSettings.Where((GameSetting setting) => string.Equals(setting.SettingType, "OrientationMode", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
			if (gameSetting != null)
			{
				foreach (Dictionary<string, object> settingsDatum in gameSetting.SettingsData)
				{
					if (settingsDatum.ContainsKey("mode") && settingsDatum.ContainsKey("app_pkg_list") && settingsDatum["app_pkg_list"] is AppPackageListObject appPackageListObject && appPackageListObject.IsPackageAvailable(package))
					{
						result = settingsDatum["mode"].ToString().ToLower(CultureInfo.InvariantCulture);
						break;
					}
				}
			}
		}
		return result;
	}
}
