using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class GrmManager
{
	internal static void UpdateGrmAsync(IEnumerable<string> listOfPackages = null)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			if (AppRequirementsParser.Instance.Requirements == null)
			{
				AppRequirementsParser.Instance.PopulateRequirementsFromFile();
			}
			GetGrmFromCloud(listOfPackages);
		});
	}

	private static void GetGrmFromCloud(IEnumerable<string> listOfPackages = null)
	{
		try
		{
			if (listOfPackages != null && listOfPackages.Any())
			{
				List<string> list = new List<string>();
				string[] vmList = RegistryManager.Instance.VmList;
				foreach (string text in vmList)
				{
					list = list.Union(JsonParser.GetInstalledAppsList(text)).ToList();
				}
				if (!listOfPackages.Intersect(list).Any())
				{
					return;
				}
			}
			JObject val = JObject.Parse(HTTPUtils.SendRequestToCloud("grm/files", (Dictionary<string, string>)null, "Android", 0, (Dictionary<string, string>)null, false, 1, 0, false));
			if ((int)val["code"] == 200 && val["data"].Value<bool>((object)"success"))
			{
				string text2 = val["data"][(object)"files"].Value<string>((object)"translations_file");
				string text3 = BstHttpClient.Get(val["data"][(object)"files"].Value<string>((object)"config_file"), (Dictionary<string, string>)null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
				string text4 = BstHttpClient.Get(text2, (Dictionary<string, string>)null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
				AppRequirementsParser.Instance.UpdateOverwriteRequirements(text3, text4);
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error Getting Grm json " + ex.ToString());
		}
	}
}
