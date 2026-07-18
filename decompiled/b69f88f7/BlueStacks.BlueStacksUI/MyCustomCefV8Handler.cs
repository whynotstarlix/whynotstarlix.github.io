using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI;

internal class MyCustomCefV8Handler : CefV8Handler
{
	private static object sLock = new object();

	private static string vmName = "";

	private string mCallBackFunction;

	protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
	{
		returnValue = CefV8Value.CreateString("");
		ReceiveJsFunctionCall(arguments, ref returnValue);
		exception = null;
		return true;
	}

	private void ReceiveJsFunctionCall(CefV8Value[] arguments, ref CefV8Value returnValue)
	{
		JObject val = JObject.Parse(arguments[0].GetStringValue());
		string text = ((object)val["data"]).ToString();
		if (string.IsNullOrEmpty(text) || text.Equals("null", StringComparison.InvariantCultureIgnoreCase))
		{
			text = "[]";
		}
		JArray val2 = JArray.Parse(text);
		object[] array = null;
		if (!JsonExtensions.IsNullOrEmpty((JToken)(object)val2))
		{
			array = new object[((JContainer)val2).Count];
			for (int i = 0; i < ((JContainer)val2).Count; i++)
			{
				array[i] = ((object)val2[i]).ToString();
			}
		}
		try
		{
			try
			{
				if (val.ContainsKey("callbackFunction"))
				{
					mCallBackFunction = ((object)val["callbackFunction"]).ToString();
				}
			}
			catch
			{
				Logger.Info("Error in callback function name.");
			}
			if (((object)val["calledFunction"]).ToString().IndexOf("LogInfo", StringComparison.InvariantCulture) == -1)
			{
				Logger.Debug("Calling function from GM API.." + ((object)val["calledFunction"]).ToString());
			}
			((object)val["calledFunction"]).ToString();
			object obj2;
			try
			{
				Type[] types = Type.EmptyTypes;
				if (array != null)
				{
					types = Type.GetTypeArray(array);
				}
				obj2 = ((object)this).GetType().GetMethod(((object)val["calledFunction"]).ToString(), types).Invoke(this, array);
			}
			catch (Exception)
			{
				obj2 = ((object)this).GetType().GetMethod(((object)val["calledFunction"]).ToString()).Invoke(this, array);
			}
			if (obj2 != null)
			{
				returnValue = CefV8Value.CreateString((string)obj2);
			}
		}
		catch (Exception ex2)
		{
			Logger.Info("Error in ReceiveJSFunctionCall: " + ex2.ToString());
		}
	}

	internal void OnProcessMessageReceived(CefProcessMessage message)
	{
		Logger.Info("message received in render process." + message.Name);
		string name = message.Name;
		if (name != null && name == "SetVmName")
		{
			vmName = message.Arguments.GetString(0);
		}
	}

	public string isBTVInstalled()
	{
		if (!BTVManager.IsBTVInstalled())
		{
			CefProcessMessage val = CefProcessMessage.Create("DownloadBTV");
			try
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
				return "false";
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (!BTVManager.IsDirectXComponentsInstalled())
		{
			CefProcessMessage val2 = CefProcessMessage.Create("DownloadDirectX");
			try
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val2);
				return "false";
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		return "true";
	}

	public void sendFirebaseNotification(string data)
	{
		Logger.Debug("Got call for sendFirebaseNotification");
		CefProcessMessage val = CefProcessMessage.Create("sendFirebaseNotification");
		try
		{
			val.Arguments.SetString(0, data);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void subscribeModule(string tag)
	{
		Logger.Info("Subscribe html module");
		CefProcessMessage val = CefProcessMessage.Create("subscribeModule");
		try
		{
			val.Arguments.SetString(0, tag);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UnsubscribeModule(string tag)
	{
		Logger.Info("Unsubscribe html module");
		CefProcessMessage val = CefProcessMessage.Create("unsubscribeModule");
		try
		{
			val.Arguments.SetString(0, tag);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void subscribeToClientTags(string json)
	{
		Logger.Info("Subscribe to client tags");
		CefProcessMessage val = CefProcessMessage.Create("subscribeClientTags");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void subscribeToVmSpecificClientTags(string json)
	{
		Logger.Info("Subscribe to vm specific client tags");
		CefProcessMessage val = CefProcessMessage.Create("subscribeVmSpecificClientTags");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UnsubscribeToClientTags(string json)
	{
		Logger.Info("Unsubscribe to client tags");
		CefProcessMessage val = CefProcessMessage.Create("unsubscribeClientTags");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void HandleClick(string json)
	{
		CefProcessMessage val = CefProcessMessage.Create("HandleClick");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateQuestRules(string json)
	{
		CefProcessMessage val = CefProcessMessage.Create("UpdateQuestRules");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void pikaworldprofileadded(string profileId)
	{
		Logger.Debug("Got call for PikaWorldProfileAdded");
		CefProcessMessage val = CefProcessMessage.Create("PikaWorldProfileAdded");
		try
		{
			val.Arguments.SetString(0, profileId);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string getPikaWorldUserId()
	{
		return RegistryManager.Instance.PikaWorldId;
	}

	public string getBootTime()
	{
		return RegistryManager.Instance.LastBootTime.ToString(CultureInfo.InvariantCulture);
	}

	public void getGamepadConnectionStatus()
	{
		Logger.Debug("Got call for getGamepadConnectionStatus");
		CefProcessMessage val = CefProcessMessage.Create("GetGamepadConnectionStatus");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string IsAnyAppRunning()
	{
		string text = "isAnyAppRunning";
		try
		{
			return HTTPUtils.SendRequestToClient(text, (Dictionary<string, string>)null, "Android", 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64").ToLower(CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Logger.Error("An unexpected error occured in {0}. Err: {1}", new object[2]
			{
				text,
				ex.ToString()
			});
			return null;
		}
	}

	public void goToMapsTab()
	{
		CefProcessMessage val = CefProcessMessage.Create("GoToMapsTab");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void clearmapsnotification()
	{
		Logger.Info("Got call from browser for maps clear notification");
		CefProcessMessage val = CefProcessMessage.Create("ClearMapsNotification");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void installapp(string appIcon, string appName, string apkUrl, string packageName)
	{
		Logger.Info("Get Call from browser of Install App :" + appName);
		CefProcessMessage val = CefProcessMessage.Create("InstallApp");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, appIcon);
			arguments.SetString(1, appName);
			arguments.SetString(2, apkUrl);
			arguments.SetString(3, packageName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string installapp(string appIcon, string appName, string apkUrl, string packageName, string timestamp)
	{
		Logger.Info("Get Call from browser of Install App with version :" + appName);
		CefProcessMessage val = CefProcessMessage.Create("InstallAppVersion");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, appIcon);
			arguments.SetString(1, appName);
			arguments.SetString(2, apkUrl);
			arguments.SetString(3, packageName);
			arguments.SetString(4, timestamp);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			return "true";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void installapp_google(string appIcon, string appName, string apkUrl, string packageName)
	{
		Logger.Info("Get Call from browser of Install App from googleplay :" + appName);
		CefProcessMessage val = CefProcessMessage.Create("InstallAppGooglePlay");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, appIcon);
			arguments.SetString(1, appName);
			arguments.SetString(2, apkUrl);
			arguments.SetString(3, packageName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void installapp_google_popup(string appIcon, string appName, string apkUrl, string packageName)
	{
		Logger.Info("Get Call from browser of Install App from googleplay in popup :" + appName);
		CefProcessMessage val = CefProcessMessage.Create("InstallAppGooglePlayPopup");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, appIcon);
			arguments.SetString(1, appName);
			arguments.SetString(2, apkUrl);
			arguments.SetString(3, packageName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void downloadinstalloem(string oem, string abiValue)
	{
		Logger.Info("Get Call from browser of downloadoem oem: " + oem + ", abiValue: " + abiValue);
		CefProcessMessage val = CefProcessMessage.Create("DownloadInstallOem");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, oem);
			arguments.SetString(1, abiValue);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void canceloemdownload(string oem, string abiValue)
	{
		Logger.Info("Get Call from browser of canceloemdownload oem: " + oem + ", abiValue: " + abiValue);
		CefProcessMessage val = CefProcessMessage.Create("CancelOemDownload");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, oem);
			arguments.SetString(1, abiValue);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void launchappindifferentoem(string oem, string abiValue, string vmname, string packageName, string actionWithRemainingInstances)
	{
		Logger.Info("Get Call from browser of launchappindifferentoem oem: " + oem + ", abiValue: " + abiValue + ", packageName: " + packageName + ", actionWithRemainingInstances: " + actionWithRemainingInstances);
		CefProcessMessage val = CefProcessMessage.Create("LaunchAppInDifferentOem");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, oem);
			arguments.SetString(1, abiValue);
			arguments.SetString(2, vmname);
			arguments.SetString(3, packageName);
			arguments.SetString(4, actionWithRemainingInstances);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void uninstallapp(string packageName)
	{
		Logger.Info("Get Call from browser of Uninstall App for packagename :" + packageName);
		CefProcessMessage val = CefProcessMessage.Create("UninstallApp");
		try
		{
			val.Arguments.SetString(0, packageName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void getupdatedgrm()
	{
		Logger.Info("Got call from browser to get updated grm");
		CefProcessMessage val = CefProcessMessage.Create("GetUpdatedGrm");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void retryapkinstall(string apkFilePath)
	{
		Logger.Info("Get Call from browser of RetryApkInstall :" + apkFilePath);
		CefProcessMessage val = CefProcessMessage.Create("RetryApkInstall");
		try
		{
			val.Arguments.SetString(0, apkFilePath);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void chooseandinstallapk()
	{
		Logger.Info("Get Call from browser of ChooseAndInstallApk :");
		CefProcessMessage val = CefProcessMessage.Create("ChooseAndInstallApk");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void checkifpremium(string isPremium)
	{
		Logger.Info("Got call from blocker ad browser of premium subscription");
		CefProcessMessage val = CefProcessMessage.Create("CheckIfPremium");
		try
		{
			val.Arguments.SetString(0, isPremium);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void applyTheme(string themeName)
	{
		CefProcessMessage val = CefProcessMessage.Create("ApplyThemeName");
		try
		{
			val.Arguments.SetString(0, themeName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public List<string> getsupportedactiontypes()
	{
		return Enum.GetNames(typeof(GenericAction)).ToList();
	}

	public void getimpressionid(string impressionId)
	{
		Logger.Info("Get call from browser of impression_id :" + impressionId);
		CefProcessMessage val = CefProcessMessage.Create("GetImpressionId");
		try
		{
			val.Arguments.SetString(0, impressionId);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			val.Dispose();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string installedapps()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		List<AppInfo> list = new JsonParser("Android").GetAppList().ToList();
		string text = string.Empty;
		foreach (AppInfo item in list)
		{
			text = text + item.Package + ",";
		}
		return text;
	}

	public string installedappsforvm(string vmName)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(vmName))
		{
			vmName = "Android";
		}
		List<AppInfo> list = new JsonParser(vmName).GetAppList().ToList();
		string text = string.Empty;
		foreach (AppInfo item in list)
		{
			text = text + item.Package + ",";
		}
		return text;
	}

	public void openapp(string appIcon, string appName, string apkUrl, string packageName)
	{
		Logger.Info("Get Call from browser of open App :" + appName);
		CefProcessMessage val = CefProcessMessage.Create("InstallApp");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, appIcon);
			arguments.SetString(1, appName);
			arguments.SetString(2, apkUrl);
			arguments.SetString(3, packageName);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void showdetails(string url)
	{
		CefBrowser browser = CefV8Context.GetCurrentContext().GetBrowser();
		try
		{
			CefProcessMessage val = CefProcessMessage.Create(url);
			browser.SendProcessMessage((CefProcessId)0, val);
			val.Dispose();
		}
		finally
		{
			((IDisposable)browser)?.Dispose();
		}
	}

	public void searchappcenter(string searchString)
	{
		CefProcessMessage val = CefProcessMessage.Create("SearchAppcenter");
		try
		{
			val.Arguments.SetString(0, searchString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void browser2Client()
	{
	}

	public string getguid()
	{
		return RegistryManager.Instance.UserGuid;
	}

	public string GetUserGUID()
	{
		return RegistryManager.Instance.UserGuid;
	}

	public void feedback(string email, string appName, string description, string downloadURl)
	{
		string clientVersion = RegistryManager.Instance.ClientVersion;
		description = description.Replace("&", " ");
		description += "\n From Client VER=";
		description += clientVersion;
		string text = "-startwithparam \"" + email + "&Others&" + appName + "&" + description + "&" + downloadURl + "\"";
		using Process process = new Process();
		process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
		Logger.Info("The arguments being passed to log collector is :{0}", new object[1] { text });
		process.StartInfo.Arguments = text;
		process.Start();
	}

	public static void openLogCollector(string data = "")
	{
		if (!string.IsNullOrEmpty(data))
		{
			string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
			if (!string.IsNullOrEmpty(clientInstallDir))
			{
				File.WriteAllText(Path.Combine(clientInstallDir, "logCollectorSourceData.txt"), data);
			}
		}
		string installDir = RegistryStrings.InstallDir;
		using Process process = new Process();
		process.StartInfo.FileName = Path.Combine(installDir, "HD-LogCollector.exe");
		if (!string.IsNullOrEmpty(vmName))
		{
			process.StartInfo.Arguments = "-Vmname=" + vmName;
		}
		Logger.Info("Open log collector through gmApi from dir: " + installDir);
		process.Start();
	}

	public void openLogCollector()
	{
		openLogCollector(string.Empty);
	}

	public void closesearch()
	{
		CefProcessMessage val = CefProcessMessage.Create("CloseSearch");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void refresh_search()
	{
		CefProcessMessage val = CefProcessMessage.Create("RefreshSearch");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void refresh_search(string searchString)
	{
		CefProcessMessage val = CefProcessMessage.Create("RefreshSearch");
		try
		{
			val.Arguments.SetString(0, searchString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void offlinehtmlhomeurl(string url)
	{
		CefProcessMessage val = CefProcessMessage.Create("OfflineHtmlHomeUrl");
		try
		{
			val.Arguments.SetString(0, url);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void refreshhomehtml()
	{
		Logger.Info("Got call from browser to refresh home html");
		CefProcessMessage val = CefProcessMessage.Create("RefreshHomeHtml");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void setwebappversion(string version)
	{
		CefProcessMessage val = CefProcessMessage.Create("SetWebAppVersion");
		try
		{
			val.Arguments.SetString(0, version);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string getwebappversion()
	{
		return RegistryManager.Instance.WebAppVersion;
	}

	public void google_search(string searchString)
	{
		CefProcessMessage val = CefProcessMessage.Create("GoogleSearch");
		try
		{
			val.Arguments.SetString(0, searchString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string getusertoken()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		JObject val = new JObject();
		val.Add("email", JToken.op_Implicit(RegistryManager.Instance.RegisteredEmail));
		val.Add("token", JToken.op_Implicit(RegistryManager.Instance.Token));
		return ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
	}

	public void preinstallapp()
	{
	}

	public void openurl(string url)
	{
		Process.Start(url);
	}

	public string prod_ver()
	{
		return RegistryManager.Instance.Version;
	}

	public string getengineguid()
	{
		return RegistryManager.Instance.Version;
	}

	public string EngineVersion()
	{
		return RegistryManager.Instance.Version;
	}

	public string ClientVersion()
	{
		return RegistryManager.Instance.ClientVersion;
	}

	public string InstallID()
	{
		return RegistryManager.Instance.InstallID;
	}

	public string IsPremiumUser()
	{
		return RegistryManager.Instance.IsPremium.ToString(CultureInfo.InvariantCulture);
	}

	public string IsGoogleSigninDone(string vmName)
	{
		if (string.IsNullOrEmpty(vmName))
		{
			vmName = "Android";
		}
		if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
		{
			return RegistryManager.Instance.Guest["Android"].IsGoogleSigninDone.ToString(CultureInfo.InvariantCulture);
		}
		return RegistryManager.Instance.Guest[vmName].IsGoogleSigninDone.ToString(CultureInfo.InvariantCulture);
	}

	public string IsOemAlreadyInstalled(string oem, string abi)
	{
		return InstalledOem.CheckIfOemInstancePresent(oem, abi).ToString(CultureInfo.InvariantCulture);
	}

	public string CampaignName()
	{
		try
		{
			string campaignJson = RegistryManager.Instance.CampaignJson;
			if (!string.IsNullOrEmpty(campaignJson))
			{
				JObject val = JObject.Parse(campaignJson);
				if (val["campaign_name"] != null)
				{
					return ((object)val["campaign_name"]).ToString();
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logger.Error("error while sending campaign name in gm api: " + ex.ToString());
			return "";
		}
	}

	public string CampaignJson()
	{
		return RegistryManager.Instance.CampaignJson;
	}

	public string get_oem()
	{
		return RegistryManager.Instance.Oem;
	}

	public string isAutomationEnabled()
	{
		return RegistryManager.Instance.EnableAutomation.ToString(CultureInfo.InvariantCulture);
	}

	public string bgp_uuid()
	{
		return RegistryManager.Instance.UserGuid;
	}

	public string BGPDevUrl()
	{
		return RegistryManager.Instance.BGPDevUrl;
	}

	public string DevCloudUrl()
	{
		return RegistryManager.Instance.Host;
	}

	public string GetMachineId()
	{
		return GuidUtils.GetBlueStacksMachineId();
	}

	public string GetVersionId()
	{
		return GuidUtils.GetBlueStacksVersionId();
	}

	public string SetFirebaseHost(string hostName)
	{
		lock (sLock)
		{
			if (!string.IsNullOrEmpty(RegistryManager.Instance.CurrentFirebaseHost))
			{
				return "false";
			}
			RegistryManager.Instance.CurrentFirebaseHost = hostName;
			return "true";
		}
	}

	public void closeAnyPopup()
	{
		Logger.Info("Got call from browser of closeAnyPopup");
		CefProcessMessage val = CefProcessMessage.Create("CloseAnyPopup");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void closeself()
	{
		Logger.Info("Got call from browser of closeself");
		CefProcessMessage val = CefProcessMessage.Create("CloseSelf");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void closequitpopup()
	{
		Logger.Info("Got call from browser of closequitpopup");
		CefProcessMessage val = CefProcessMessage.Create("CloseBrowserQuitPopup");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void downloadMacro(string macroData)
	{
		Logger.Info("Got call from browser of downloadmacro");
		CefProcessMessage val = CefProcessMessage.Create("DownloadMacro");
		try
		{
			val.Arguments.SetString(0, macroData);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string getCurrentMacros()
	{
		Logger.Info("Got call from browser of getcurrentmacros");
		return string.Join("|", BlueStacksUIUtils.GetMacroList().ToArray());
	}

	public string uploadLocalMacro(string macroName)
	{
		Logger.Info("Got call from browser of uploadlocalmacro");
		return BlueStacksUIUtils.GetBase64MacroData(macroName);
	}

	public void performOTS()
	{
		Logger.Info("Got call from browser of performOTS");
		CefProcessMessage val = CefProcessMessage.Create("PerformOTS");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void changeControlScheme(string schemeSelected)
	{
		Logger.Info("Got call from browser of changeControlScheme");
		CefProcessMessage val = CefProcessMessage.Create("ChangeControlScheme");
		try
		{
			val.Arguments.SetString(0, schemeSelected);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void closeOnBoarding(string json)
	{
		Logger.Info("Got call from browser of closeOnBoarding");
		CefProcessMessage val = CefProcessMessage.Create("CloseOnBoarding");
		try
		{
			val.Arguments.SetString(0, json);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void browserLoaded()
	{
		Logger.Info("Got call from browser of browserLoaded");
		CefProcessMessage val = CefProcessMessage.Create("BrowserLoaded");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void getSystemInfo()
	{
		Logger.Info("Got call from browser of getSystemInfo");
		CefProcessMessage val = CefProcessMessage.Create("GetSystemInfo");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void LogInfo(string info)
	{
		Logger.Info("HtmlLog: " + info);
	}

	public string GetSystemRAM()
	{
		return Profile.RAM;
	}

	public string GetLocale()
	{
		return RegistryManager.Instance.UserSelectedLocale;
	}

	public string GetSystemCPU()
	{
		return Profile.CPU;
	}

	public string GetSystemGPU()
	{
		return Profile.GPU;
	}

	public string GetSystemOS()
	{
		return Profile.OS;
	}

	public string GetCurrentSessionId()
	{
		Logger.Info("In GetCurrentSessionId");
		return Stats.GetSessionId();
	}

	public void showWebPage(string title, string webUrl)
	{
		CefProcessMessage val = CefProcessMessage.Create("ShowWebPage");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, title);
			arguments.SetString(1, webUrl);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void HidePreview()
	{
		CefProcessMessage val = CefProcessMessage.Create("HidePreview");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ShowPreview()
	{
		CefProcessMessage val = CefProcessMessage.Create("ShowPreview");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StartObs(string callbackFunction)
	{
		CefProcessMessage val = CefProcessMessage.Create("StartObs");
		try
		{
			val.Arguments.SetString(0, callbackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StartStreamViewStatsRecorder(string label, string jsonString)
	{
		CefProcessMessage val = CefProcessMessage.Create("StartStreamViewStatsRecorder");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, label);
			arguments.SetString(1, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetStreamConfig()
	{
		Logger.Info("In GetStreamConfig");
		return StreamManager.GetStreamConfig();
	}

	public void LaunchDialog(string jsonString)
	{
		CefProcessMessage val = CefProcessMessage.Create("LaunchDialog");
		try
		{
			val.Arguments.SetString(0, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StartStreamV2(string jsonString, string callbackStreamStatus, string callbackTabChanged)
	{
		Logger.Info("Got StartStreamV2");
		CefProcessMessage val = CefProcessMessage.Create("StartStreamV2");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, jsonString);
			arguments.SetString(1, callbackStreamStatus);
			arguments.SetString(2, callbackTabChanged);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StopStream()
	{
		Logger.Info("Got StopStream");
		CefProcessMessage val = CefProcessMessage.Create("StopStream");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StartRecord()
	{
		StartRecordV2("{}");
	}

	public static void StartRecordV2(string jsonString)
	{
		Logger.Info("Got StartRecordV2");
		CefProcessMessage val = CefProcessMessage.Create("StartRecordV2");
		try
		{
			val.Arguments.SetString(0, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StopRecord()
	{
		Logger.Info("Got StopStream");
		CefProcessMessage val = CefProcessMessage.Create("StopRecord");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SetSystemVolume(string level)
	{
		CefProcessMessage val = CefProcessMessage.Create("SetSystemVolume");
		try
		{
			val.Arguments.SetString(0, level);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SetMicVolume(string level)
	{
		CefProcessMessage val = CefProcessMessage.Create("SetMicVolume");
		try
		{
			val.Arguments.SetString(0, level);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void EnableWebcam(string width, string height, string position)
	{
		CefProcessMessage val = CefProcessMessage.Create("EnableWebcam");
		try
		{
			Logger.Info("Got EnableWebcam");
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, width);
			arguments.SetString(1, height);
			arguments.SetString(2, position);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void DisableWebcamV2(string jsonString)
	{
		CefProcessMessage val = CefProcessMessage.Create("DisableWebcamV2");
		try
		{
			Logger.Info("Got DisableWebcamV2");
			val.Arguments.SetString(0, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void MoveWebcam(string horizontal, string vertical)
	{
		CefProcessMessage val = CefProcessMessage.Create("MoveWebcam");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, horizontal);
			arguments.SetString(1, vertical);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SetStreamName(string name)
	{
		Logger.Info("Got SetStreamName: " + name);
		RegistryManager.Instance.StreamName = name;
	}

	public void SetServerLocation(string location)
	{
		Logger.Info("Got SetServerLocation: " + location);
		RegistryManager.Instance.ServerLocation = location;
	}

	public void SetChannelName(string channelName)
	{
		RegistryManager.Instance.ChannelName = channelName;
	}

	public string GetRealtimeAppUsage()
	{
		CefProcessMessage val = CefProcessMessage.Create("GetRealtimeAppUsage");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			return "";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetInstalledAppsJsonforJS()
	{
		CefProcessMessage val = CefProcessMessage.Create("GetInstalledAppsJsonforJS");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			return "";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetInstalledAppsForAllOems()
	{
		CefProcessMessage val = CefProcessMessage.Create("GetInstalledAppsForAllOems");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			return "";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetCurrentAppInfo()
	{
		CefProcessMessage val = CefProcessMessage.Create("GetCurrentAppInfo");
		try
		{
			val.Arguments.SetString(0, mCallBackFunction);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
			return "";
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetGMPort()
	{
		Logger.Info("In GetGMPort");
		return RegistryManager.Instance.PartnerServerPort.ToString(CultureInfo.InvariantCulture);
	}

	public string ResetSessionId()
	{
		Logger.Info("In ResetSessionId");
		return Stats.ResetSessionId();
	}

	public void makeWebCall(string url, string scriptToInvoke)
	{
		CefProcessMessage val = CefProcessMessage.Create("makeWebCall");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, url);
			arguments.SetString(1, scriptToInvoke);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ShowWebPageInBrowser(string url)
	{
		Logger.Info("Showing " + url + " in default browser");
		BlueStacksUIUtils.OpenUrl(url);
	}

	public void DialogClickHandler(string jsonString)
	{
		CefProcessMessage val = CefProcessMessage.Create("DialogClickHandler");
		try
		{
			val.Arguments.SetString(0, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void CloseDialog(string jsonString)
	{
		CefProcessMessage val = CefProcessMessage.Create("CloseDialog");
		try
		{
			val.Arguments.SetString(0, jsonString);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ShowAdvancedSettings()
	{
		CefProcessMessage val = CefProcessMessage.Create("ShowAdvancedSettings");
		try
		{
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void LaunchFilterWindow(string channel, string sessionId)
	{
		CefProcessMessage val = CefProcessMessage.Create("LaunchFilterWindow");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, channel);
			arguments.SetString(1, sessionId);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ChangeFilterTheme(string theme)
	{
		CefProcessMessage val = CefProcessMessage.Create("ChangeFilterTheme");
		try
		{
			val.Arguments.SetString(0, theme);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateThemeSettings(string currentTheme, string settingsJson)
	{
		CefProcessMessage val = CefProcessMessage.Create("UpdateThemeSettings");
		try
		{
			CefListValue arguments = val.Arguments;
			arguments.SetString(0, currentTheme);
			arguments.SetString(1, settingsJson);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void CloseFilterWindow(string jsonArray)
	{
		CefProcessMessage val = CefProcessMessage.Create("CloseFilterWindow");
		try
		{
			val.Arguments.SetString(0, jsonArray);
			CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage((CefProcessId)0, val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string IsFacebook()
	{
		if (string.Equals(RegistryManager.Instance.BtvNetwork, "facebook", StringComparison.InvariantCulture))
		{
			return "true";
		}
		return "false";
	}
}
