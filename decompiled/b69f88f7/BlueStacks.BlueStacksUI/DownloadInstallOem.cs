using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class DownloadInstallOem
{
	private MainWindow ParentWindow;

	private AppPlayerModel currentDownloadingOem;

	private long mSizeInBytes;

	private int mLastPercentSend;

	public DownloadInstallOem(MainWindow mainWindow)
	{
		ParentWindow = mainWindow;
	}

	public void DownloadOem(AppPlayerModel appPlayerModel)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00ae: Expected O, but got Unknown
		//IL_00ae: Expected O, but got Unknown
		//IL_00ae: Expected O, but got Unknown
		//IL_00ae: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		currentDownloadingOem = appPlayerModel;
		if (InstalledOem.InstalledCoexistingOemList.Contains(appPlayerModel.AppPlayerOem))
		{
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += BGCreateNewInstance_DoWork;
				backgroundWorker.RunWorkerCompleted += BGCreateNewInstance_RunWorkerCompleted;
				backgroundWorker.RunWorkerAsync();
				return;
			}
		}
		Publisher.PublishMessage((BrowserControlTags)18, ParentWindow.mVmName, (JObject)null);
		if (!appPlayerModel.DownLoadOem((DownloadExceptionEventHandler)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			JObject val2 = new JObject();
			val2["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""));
			val2["MessageBody"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", ""));
			val2["ActionType"] = JToken.op_Implicit("failed");
			Publisher.PublishMessage((BrowserControlTags)19, ParentWindow.mVmName, val2);
		}, (DownloadProgressChangedEventHandler)delegate(long size)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			decimal num = decimal.Divide(size, mSizeInBytes) * 100m;
			if (mLastPercentSend + 4 < (int)num)
			{
				JObject val2 = new JObject();
				val2["DownloadPercent"] = JToken.op_Implicit(num);
				Publisher.PublishMessage((BrowserControlTags)20, ParentWindow.mVmName, val2);
				mLastPercentSend = (int)num;
			}
		}, (DownloadFileCompletedEventHandler)delegate
		{
			Publisher.PublishMessage((BrowserControlTags)21, ParentWindow.mVmName, (JObject)null);
			InstallOemOperation();
		}, (FilePayloadInfoReceivedHandler)delegate(long fileSize)
		{
			mSizeInBytes = fileSize;
		}, (UnsupportedResumeEventHandler)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			JObject val2 = new JObject();
			val2["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""));
			val2["MessageBody"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_FAILED_DOWNLOAD_RETRY", ""));
			val2["ActionType"] = JToken.op_Implicit("retry");
			Publisher.PublishMessage((BrowserControlTags)19, ParentWindow.mVmName, val2);
		}, false))
		{
			JObject val = new JObject();
			val["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", ""));
			val["MessageBody"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", ""));
			val["ActionType"] = JToken.op_Implicit("failed");
			Publisher.PublishMessage((BrowserControlTags)19, ParentWindow.mVmName, val);
		}
	}

	private void BGCreateNewInstance_DoWork(object sender, DoWorkEventArgs e)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		Publisher.PublishMessage((BrowserControlTags)18, ParentWindow.mVmName, (JObject)null);
		Publisher.PublishMessage((BrowserControlTags)21, ParentWindow.mVmName, (JObject)null);
		Publisher.PublishMessage((BrowserControlTags)22, ParentWindow.mVmName, (JObject)null);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			JObject val = new JObject();
			val.Add("cpu", JToken.op_Implicit(2));
			val.Add("ram", JToken.op_Implicit(2048));
			val.Add("dpi", JToken.op_Implicit(240));
			val.Add("abi", JToken.op_Implicit(currentDownloadingOem.AbiValue));
			val.Add("resolutionwidth", JToken.op_Implicit(1920));
			val.Add("resolutionheight", JToken.op_Implicit(1080));
			JObject val2 = val;
			dictionary2["settings"] = ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
			dictionary2["vmtype"] = "fresh";
			dictionary2["vmname"] = string.Format(CultureInfo.InvariantCulture, "Android_{0}", new object[1] { Utils.GetVmIdToCreate(currentDownloadingOem.AppPlayerOem) });
			JObject val3 = JObject.Parse(HTTPUtils.SendRequestToAgent("createInstance", dictionary2, "Android", 240000, (Dictionary<string, string>)null, false, 1, 0, currentDownloadingOem.AppPlayerOem, true));
			if (val3["success"].ToObject<bool>())
			{
				string value = ((object)JObject.Parse(((object)val3["vmconfig"]).ToString().Trim())["vmname"]).ToString().Trim();
				dictionary["vmname"] = value;
				dictionary["status"] = "success";
			}
			else
			{
				dictionary["status"] = "fail";
				dictionary["reason"] = ((object)val3["reason"]).ToString().Trim();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error in creating new instance" + ex.ToString());
			dictionary["status"] = "fail";
			dictionary["reason"] = "UnknownException";
		}
		finally
		{
			e.Result = dictionary;
		}
	}

	private void BGCreateNewInstance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		if (e.Result is Dictionary<string, string> dictionary)
		{
			if (dictionary["status"].Equals("success", StringComparison.InvariantCultureIgnoreCase))
			{
				InstalledOem.SetInstalledCoexistingOems();
				RegistryManager.RegistryManagers[currentDownloadingOem.AppPlayerOem].Guest[dictionary["vmname"]].DisplayName = Strings.ProductDisplayName + " " + Utils.GetVmIdFromVmName(dictionary["vmname"]) + " " + currentDownloadingOem.Suffix;
				Publisher.PublishMessage((BrowserControlTags)24, ParentWindow.mVmName, (JObject)null);
			}
			else if (dictionary["status"].Equals("fail", StringComparison.InvariantCultureIgnoreCase))
			{
				JObject val = new JObject();
				val["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""));
				val["MessageBody"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", ""));
				val["ActionType"] = JToken.op_Implicit("failed");
				Publisher.PublishMessage((BrowserControlTags)23, ParentWindow.mVmName, val);
			}
		}
	}

	private void InstallOemOperation()
	{
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		try
		{
			Publisher.PublishMessage((BrowserControlTags)22, ParentWindow.mVmName, (JObject)null);
			int num = currentDownloadingOem.InstallOem();
			if (num != 0 || !RegistryManager.CheckOemInRegistry(currentDownloadingOem.AppPlayerOem, "Android"))
			{
				Logger.Warning("Installation failed: " + num);
				string text = ((num == 0) ? LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "") : InstallerErrorHandling.AssignErrorStringForInstallerExitCodes(num, "STRING_INSTALLATION_FAILED"));
				JObject val = new JObject();
				val["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""));
				val["MessageBody"] = JToken.op_Implicit(text);
				val["ActionType"] = JToken.op_Implicit("failed");
				Publisher.PublishMessage((BrowserControlTags)23, ParentWindow.mVmName, val);
			}
			else
			{
				InstalledOem.SetInstalledCoexistingOems();
				if (currentDownloadingOem.AppPlayerOem.Contains("bgp64"))
				{
					Utils.UpdateValueInBootParams("abivalue", currentDownloadingOem.AbiValue.ToString(CultureInfo.InvariantCulture), "Android", true, currentDownloadingOem.AppPlayerOem);
				}
				RegistryManager.RegistryManagers[currentDownloadingOem.AppPlayerOem].Guest["Android"].DisplayName = Strings.ProductDisplayName + " " + currentDownloadingOem.Suffix;
				Publisher.PublishMessage((BrowserControlTags)24, ParentWindow.mVmName, (JObject)null);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed after running installer process: " + ex);
			JObject val2 = new JObject();
			val2["MessageTitle"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""));
			val2["MessageBody"] = JToken.op_Implicit(LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", ""));
			val2["ActionType"] = JToken.op_Implicit("failed");
			Publisher.PublishMessage((BrowserControlTags)23, ParentWindow.mVmName, val2);
		}
	}
}
