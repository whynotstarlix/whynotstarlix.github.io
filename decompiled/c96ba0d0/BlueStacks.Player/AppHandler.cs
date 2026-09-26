using System;
using System.Collections.Generic;
using System.Net;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

internal class AppHandler
{
	internal static string sAppPackage;

	internal static string mCurrentAppPackage;

	internal static string sLastAppDisplayed;

	internal static string mCurrentAppActivity;

	internal static bool sAppLaunchedFromRunApp = false;

	internal static string sAppIconName = "";

	internal static bool appLaunched = false;

	internal static object sCurrentAppDisplayedLockObject = new object();

	internal static Dictionary<string, long> sAppPackagesCountClicks = new Dictionary<string, long>();

	internal static Dictionary<string, long> sDictCountClicks = new Dictionary<string, long>();

	internal static void SetPackagesForCountingInteractions(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("SetPackagesForCountingInteractions");
		try
		{
			Dictionary<string, long> dictionary = JsonExtensions.ToDictionary<long>((JToken)(object)JObject.Parse(HTTPUtils.ParseRequest(req).Data["data"])) as Dictionary<string, long>;
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, long> sAppPackagesCountClick in sAppPackagesCountClicks)
			{
				if (!dictionary.ContainsKey(sAppPackagesCountClick.Key))
				{
					list.Add(sAppPackagesCountClick.Key);
				}
			}
			foreach (string item in list)
			{
				sAppPackagesCountClicks.Remove(item);
			}
			foreach (KeyValuePair<string, long> item2 in dictionary)
			{
				if (!sAppPackagesCountClicks.ContainsKey(item2.Key.ToLower()))
				{
					sAppPackagesCountClicks.Add(item2.Key.ToLower(), 0L);
				}
			}
			foreach (KeyValuePair<string, long> sAppPackagesCountClick2 in sAppPackagesCountClicks)
			{
				Logger.Info("questpackage: " + sAppPackagesCountClick2.Key.ToString());
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server SetPackagesForCountingInteractions. Err : " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}

	internal static void SetCurrentAppData(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("In SetCurrentAppData");
		try
		{
			RequestData val = HTTPUtils.ParseRequest(req);
			mCurrentAppPackage = val.Data["package"];
			mCurrentAppActivity = val.Data["activity"];
			Logger.Info("SetCurrentAppData mCurrentAppPackage = " + mCurrentAppPackage);
			Logger.Info("SetCurrentAppData mCurrentAppActivity = " + mCurrentAppActivity);
			Logger.Info("Looking for: " + sAppPackage);
			if (sAppLaunchedFromRunApp || sAppIconName.Contains(mCurrentAppActivity))
			{
				appLaunched = true;
				sAppLaunchedFromRunApp = false;
			}
			if (Oem.Instance.IsSendGameManagerRequest)
			{
				string value = val.Data["callingPackage"];
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "package", mCurrentAppPackage },
					{ "activity", mCurrentAppActivity },
					{ "callingPackage", value }
				};
				HTTPUtils.SendRequestToClient("appLaunched", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			if (Features.ExitOnHome() && appLaunched && (mCurrentAppPackage == "com.bluestacks.gamepophome" || mCurrentAppPackage == "com.bluestacks.appmart"))
			{
				Logger.Info("Reached home app. Closing frontend.");
				Environment.Exit(0);
			}
			Opengl.HandleAppActivity(mCurrentAppPackage, mCurrentAppActivity);
			InputMapper.Instance.SetPackage(mCurrentAppPackage);
			HTTPHandler.WriteSuccessJson(res);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Server SetCurrentAppData. Err : " + ex.ToString());
			HTTPHandler.WriteErrorJson(ex.Message, res);
		}
	}
}
