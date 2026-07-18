using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class PostBootCloudInfoManager
{
	private static PostBootCloudInfoManager sInstance = null;

	private static readonly object sLock = new object();

	private const string sPostBootCloudInfoFilename = "bst_postboot";

	internal PostBootCloudInfo mPostBootCloudInfo;

	private string mUrl = string.Empty;

	private static string BstPostBootFilePath => Path.Combine(RegistryStrings.PromotionDirectory, "bst_postboot");

	internal static PostBootCloudInfoManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (sLock)
				{
					if (sInstance == null)
					{
						sInstance = new PostBootCloudInfoManager();
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

	internal string Url
	{
		get
		{
			if (string.IsNullOrEmpty(mUrl))
			{
				mUrl = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
				{
					RegistryManager.Instance.Host,
					"/bs4/post_boot"
				}));
			}
			return mUrl;
		}
		private set
		{
			mUrl = value;
		}
	}

	private PostBootCloudInfoManager()
	{
	}

	internal JToken GetPostBootData()
	{
		JToken result = null;
		try
		{
			string text = BstHttpClient.Get(Url, (Dictionary<string, string>)null, false, "Android", 0, 1, 0, false, "bgp64");
			Logger.Debug("Postboot data Url: " + Url);
			result = JToken.Parse(text);
		}
		catch (Exception ex)
		{
			Logger.Error("Error Getting Post Boot Data err: " + ex.ToString());
		}
		return result;
	}

	internal void GetPostBootDataAsync(MainWindow mainWindow)
	{
		if (mPostBootCloudInfo == null)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				mPostBootCloudInfo = new PostBootCloudInfo();
				if (File.Exists(BstPostBootFilePath))
				{
					mPostBootCloudInfo = JsonConvert.DeserializeObject<PostBootCloudInfo>(File.ReadAllText(BstPostBootFilePath));
				}
				JToken postBootData = GetPostBootData();
				if (postBootData != null)
				{
					PostBootCloudInfo postBootCloudInfo = new PostBootCloudInfo();
					SetMinimizeGameNotificationsPackages(postBootCloudInfo, postBootData);
					SetOnBoardingGamePackages(postBootCloudInfo, postBootData);
					SetGameAwareOnboardingPackages(postBootCloudInfo, postBootData);
					SetCustomCursorGamePackages(postBootCloudInfo, postBootData);
					SetIgnoreActivities(postBootCloudInfo, postBootData);
					SaveToFile(postBootCloudInfo);
					mPostBootCloudInfo = postBootCloudInfo;
					SendCustomCursorAppsListToPlayer(mainWindow);
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}
		else
		{
			SendCustomCursorAppsListToPlayer(mainWindow);
		}
	}

	private static void SendCustomCursorAppsListToPlayer(MainWindow mainWindow)
	{
		try
		{
			string text = string.Empty;
			foreach (string cloudPackage in Instance.mPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages.CloudPackageList)
			{
				text += string.Format(CultureInfo.InvariantCulture, "{0} ", new object[1] { cloudPackage });
			}
			mainWindow.mFrontendHandler.SendFrontendRequestAsync("sendCustomCursorEnabledApps", new Dictionary<string, string> { 
			{
				"packages",
				text.Trim()
			} });
			Logger.Debug("CURSOR: vmName:{0} packages: {1} ", new object[2]
			{
				mainWindow.mVmName,
				text.Trim()
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendCustomCursorAppsListToPlayer: " + ex.ToString());
		}
	}

	private static void SaveToFile(PostBootCloudInfo postBootCloudInfo)
	{
		try
		{
			string contents = JsonConvert.SerializeObject((object)postBootCloudInfo, (Formatting)1, Utils.GetSerializerSettings());
			if (!Directory.Exists(RegistryStrings.PromotionDirectory))
			{
				Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
			}
			File.WriteAllText(BstPostBootFilePath, contents);
		}
		catch (Exception)
		{
			Logger.Warning("Error in saving PostBootCloudInfo to file");
		}
	}

	private static void SetMinimizeGameNotificationsPackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
	{
		try
		{
			JToken val = JToken.Parse(JsonExtensions.GetValue(res, "minimize_game_notification_apps"));
			if (val[(object)"app_pkg_list"] != null)
			{
				JToken obj = val[(object)"app_pkg_list"];
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 != null)
				{
					currentPostBootCloudInfo.GameNotificationAppPackages.NotificationModeAppPackages = new AppPackageListObject(((JToken)val2).ToObject<List<string>>());
				}
			}
			if (val[(object)"consecutive_session_count_number"] != null && int.TryParse(((object)val[(object)"consecutive_session_count_number"]).ToString(), out var result))
			{
				currentPostBootCloudInfo.GameNotificationAppPackages.ExitPopupCount = result;
				RegistryManager.Instance.NotificationModeCounter = result;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in parsing game notification packages: " + ex.ToString());
		}
	}

	private static void SetOnBoardingGamePackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
	{
		try
		{
			JToken val = JToken.Parse(JsonExtensions.GetValue(res, "onboarding_tutorial_apps"));
			if (val[(object)"app_pkg_list"] != null)
			{
				JToken obj = val[(object)"app_pkg_list"];
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 != null)
				{
					currentPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages = new AppPackageListObject(((JToken)val2).ToObject<List<string>>());
				}
			}
			if (val[(object)"skip_button_timer"] != null)
			{
				currentPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer = int.Parse(((object)val[(object)"skip_button_timer"]).ToString(), CultureInfo.InvariantCulture);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in parsing onboarding packages: " + ex.ToString());
		}
	}

	private static void SetCustomCursorGamePackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
	{
		try
		{
			JToken val = JToken.Parse(JsonExtensions.GetValue(res, "custom_cursor_apps"));
			val = JToken.Parse(JsonExtensions.GetValue(val, "moba"));
			if (val[(object)"app_pkg_list"] != null)
			{
				JToken obj = val[(object)"app_pkg_list"];
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 != null)
				{
					currentPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages = new AppPackageListObject(((JToken)val2).ToObject<List<string>>());
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in parsing SetCustomCursorGamePackages: " + ex.ToString());
		}
	}

	private static void SetIgnoreActivities(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
	{
		try
		{
			if (!JsonExtensions.IsNullOrEmptyBrackets(JsonExtensions.GetValue(res, "ignore_activities_for_tab")))
			{
				ListExtensions.ClearSync<string>(currentPostBootCloudInfo.IgnoredActivitiesForTabs);
				{
					foreach (JToken item in JArray.Parse(((object)res[(object)"ignore_activities_for_tab"]).ToString()))
					{
						currentPostBootCloudInfo.IgnoredActivitiesForTabs.Add(((object)item).ToString());
					}
					return;
				}
			}
			ListExtensions.ClearSync<string>(currentPostBootCloudInfo.IgnoredActivitiesForTabs);
		}
		catch (Exception ex)
		{
			Logger.Error("Error while getting ignore activities for tab list: {0}", new object[1] { ex });
		}
	}

	private static void SetGameAwareOnboardingPackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
	{
		try
		{
			JToken val = JToken.Parse(JsonExtensions.GetValue(res, "game_aware_onboarding"));
			if (val[(object)"app_pkg_list"] != null)
			{
				JToken obj = val[(object)"app_pkg_list"];
				JArray val2 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val2 != null)
				{
					currentPostBootCloudInfo.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages = new AppPackageListObject(((JToken)val2).ToObject<List<string>>());
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in parsing game aware onboarding packages: " + ex.ToString());
		}
	}
}
