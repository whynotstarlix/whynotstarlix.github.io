using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class ClientStats
{
	private static string sDevUrl = RegistryManager.Instance.BGPDevUrl;

	internal static Dictionary<string, string> GetCommonData
	{
		get
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"guid",
					RegistryManager.Instance.UserGuid
				},
				{
					"engine_guid",
					RegistryManager.Instance.UserGuid
				},
				{
					"engine_ver",
					RegistryManager.Instance.Version
				},
				{
					"client_ver",
					RegistryManager.Instance.ClientVersion
				},
				{
					"oem",
					Oem.Instance.OEM
				},
				{
					"campaign_md5",
					RegistryManager.Instance.CampaignMD5
				},
				{
					"partner",
					RegistryManager.Instance.Partner
				},
				{
					"lang",
					RegistryManager.Instance.UserSelectedLocale
				},
				{
					"email",
					RegistryManager.Instance.RegisteredEmail
				},
				{
					"engine_mode",
					RegistryManager.Instance.DeviceCaps
				}
			};
			string campaignJson = RegistryManager.Instance.CampaignJson;
			if (!string.IsNullOrEmpty(campaignJson))
			{
				try
				{
					JObject val = JObject.Parse(campaignJson);
					dictionary.Add("campaign_name", ((object)val["campaign_name"]).ToString());
				}
				catch
				{
					dictionary.Add("campaign_name", "");
				}
			}
			else
			{
				dictionary.Add("campaign_name", "");
			}
			if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
			{
				JObject val2 = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
				if (val2["campaign_id"] != null)
				{
					dictionary.Add("externalsource_campaignid", ((object)val2["campaign_id"]).ToString());
				}
				if (val2["source_version"] != null)
				{
					dictionary.Add("externalsource_version", ((object)val2["source_version"]).ToString());
				}
			}
			return dictionary;
		}
	}

	internal static void SendClientStatsAsync(string op, string status, string uri, string package = "", string errorCode = "", string vmName = "")
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				SendStatsSync(op, status, uri, package, errorCode, vmName);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send stats for uri : " + uri + ". Reason : " + ex.ToString());
			}
		});
	}

	internal static void SendFrontendClickStats(string eventType, string keyword, string app_loc, string app_pkg, string is_installed, string app_position, string app_rank, string apps_recommendation_obj)
	{
		Dictionary<string, string> getCommonData = GetCommonData;
		getCommonData.Add("event", eventType);
		getCommonData.Add("keyword", keyword);
		getCommonData.Add("app_loc", app_loc);
		getCommonData.Add("app_pkg", app_pkg);
		getCommonData.Add("is_installed", is_installed);
		getCommonData.Add("app_position", app_position);
		getCommonData.Add("app_rank", app_rank);
		getCommonData.Add("apps_recommendation_obj", apps_recommendation_obj);
		SendStatsAsync(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[2]
		{
			RegistryManager.Instance.Host,
			"frontend_click_stats"
		}), getCommonData);
	}

	internal static void SendCalendarStats(string eventType, string calendarstartdate, string calendarenddate, string calendarlink, string success = "", string rowsaffected = "")
	{
		Dictionary<string, string> getCommonData = GetCommonData;
		getCommonData.Add("event_type", eventType);
		getCommonData.Add("calendar_start_date", calendarstartdate);
		getCommonData.Add("calendar_end_date", calendarenddate);
		getCommonData.Add("calendar_link", calendarlink);
		getCommonData.Add("success", success);
		getCommonData.Add("rows_affected", rowsaffected);
		SendStatsAsync(RegistryManager.Instance.Host + "/bs4/stats/calendar_stats", getCommonData);
	}

	internal static void SendStatsSync(string op, string status, string uri, string package, string errorCode = "", string vmname = "")
	{
		Dictionary<string, string> data = GetCommonData;
		data.Add("op", op);
		data.Add("status", status);
		string value = ((!(uri != "engine_activity")) ? RegistryManager.Instance.Version : "4.220.0.4001");
		data.Add("version", value);
		if (uri == "emulator_activity")
		{
			Dictionary<string, string> resolutionData = BlueStacksUIUtils.GetResolutionData();
			try
			{
				resolutionData.ToList().ForEach(delegate(KeyValuePair<string, string> kvp)
				{
					data[kvp.Key] = kvp.Value;
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
			}
			try
			{
				resolutionData = BlueStacksUIUtils.GetEngineSettingsData(vmname);
				resolutionData.ToList().ForEach(delegate(KeyValuePair<string, string> kvp)
				{
					data[kvp.Key] = kvp.Value;
				});
			}
			catch (Exception ex2)
			{
				Logger.Error("Merge dictionary failed. Ex : " + ex2.ToString());
			}
		}
		if (!string.IsNullOrEmpty(errorCode))
		{
			data.Add("error_code", errorCode);
		}
		if (!string.IsNullOrEmpty(package))
		{
			data.Add("app_pkg", package);
		}
		SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[2]
		{
			string.IsNullOrEmpty(sDevUrl) ? RegistryManager.Instance.Host : sDevUrl,
			uri
		}), data, null, vmname);
	}

	internal static void SendGPlayClickStats(Dictionary<string, string> clientData)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Dictionary<string, string> getCommonData = GetCommonData;
				if (clientData != null)
				{
					foreach (KeyValuePair<string, string> clientDatum in clientData)
					{
						getCommonData.Add(clientDatum.Key, clientDatum.Value);
					}
				}
				SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/gplay_click_stats", new object[1] { string.IsNullOrEmpty(sDevUrl) ? RegistryManager.Instance.Host : sDevUrl }), getCommonData);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send gplay stats... Err : " + ex.ToString());
			}
		});
	}

	internal static void SendMiscellaneousStatsAsync(string tag, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6 = null, string arg7 = null, string arg8 = null)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Logger.Info("Sending miscellaneous Stats for tag : " + tag);
				Dictionary<string, string> data = new Dictionary<string, string>
				{
					{ "tag", tag },
					{ "arg1", arg1 },
					{ "arg2", arg2 },
					{ "arg3", arg3 },
					{ "arg4", arg4 },
					{ "arg5", arg5 },
					{ "arg6", arg6 },
					{ "arg7", arg7 },
					{ "arg8", arg8 }
				};
				SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
				{
					RegistryManager.Instance.Host,
					"/stats/miscellaneousstats"
				}), data);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
			}
		});
	}

	internal static void SendKeyMappingUIStatsAsync(string eventtype, string packageName, string extraInfo = "")
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Logger.Info("Sending KeyMappingUI Stats");
				Dictionary<string, string> data = new Dictionary<string, string>
				{
					{
						"guid",
						RegistryManager.Instance.UserGuid
					},
					{
						"prod_ver",
						RegistryManager.Instance.ClientVersion
					},
					{
						"oem",
						RegistryManager.Instance.Oem
					},
					{ "app_pkg", packageName },
					{ "event_type", eventtype },
					{
						"email",
						RegistryManager.Instance.RegisteredEmail
					},
					{ "extra_info", extraInfo },
					{
						"locale",
						RegistryManager.Instance.UserSelectedLocale
					}
				};
				SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
				{
					RegistryManager.Instance.Host,
					"/stats/keymappinguistats"
				}), data);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
			}
		});
	}

	internal static void SendLocalQuitPopupStatsAsync(string tag, string eventType)
	{
		Logger.Debug("Sending LocalQuitPopupStats for {0}", new object[1] { eventType });
		string userGuid = RegistryManager.Instance.UserGuid;
		string clientVersion = RegistryManager.Instance.ClientVersion;
		string campaignMD = RegistryManager.Instance.CampaignMD5;
		SendMiscellaneousStatsAsync(tag, eventType, userGuid, clientVersion, campaignMD, "");
	}

	internal static void SendBluestacksUpdaterUIStatsAsync(string eventName, string comment = "")
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Logger.Info("Sending Bluestacks Updater UI Stats");
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "event", eventName },
					{
						"install_id",
						RegistryManager.Instance.InstallID
					},
					{
						"engine_version",
						RegistryManager.Instance.Version
					},
					{
						"client_version",
						RegistryManager.Instance.ClientVersion
					},
					{
						"os",
						Profile.OS
					}
				};
				string value = InstallerArchitectures.AMD64;
				if (!SystemUtils.IsOs64Bit())
				{
					value = InstallerArchitectures.X86;
				}
				dictionary.Add("installer_arch", value);
				dictionary.Add("guid", RegistryManager.Instance.UserGuid);
				dictionary.Add("oem", Oem.Instance.OEM);
				dictionary.Add("campaign_hash", RegistryManager.Instance.CampaignMD5);
				dictionary.Add("campaign_name", RegistryManager.Instance.CampaignName);
				dictionary.Add("locale", RegistryManager.Instance.UserSelectedLocale);
				dictionary.Add("comment", comment);
				dictionary.Add("installation_type", ((object)RegistryManager.Instance.InstallationType/*cast due to constrained. prefix*/).ToString());
				dictionary.Add("gaming_pkg_name", RegistryManager.Instance.InstallerPkgName);
				SendStats(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[2]
				{
					RegistryManager.Instance.Host,
					"/bs3/stats/unified_install_stats"
				}), dictionary);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
			}
		});
	}

	internal static void SendPopupBrowserStatsInMiscASync(string eventType, string url)
	{
		SendMiscellaneousStatsAsync("PopupBrowser", RegistryManager.Instance.UserGuid, eventType, url, RegistryManager.Instance.RegisteredEmail, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version);
	}

	internal static void SendGeneralStats(string op, Dictionary<string, string> sourceData)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				Dictionary<string, string> getCommonData = GetCommonData;
				getCommonData.Add("op", op);
				if (sourceData != null)
				{
					foreach (KeyValuePair<string, string> sourceDatum in sourceData)
					{
						getCommonData.Add(sourceDatum.Key, sourceDatum.Value);
					}
				}
				getCommonData.Add("os_ver", string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
				{
					Environment.OSVersion.Version.Major,
					Environment.OSVersion.Version.Minor
				}));
				SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/general_json", new object[1] { string.IsNullOrEmpty(sDevUrl) ? RegistryManager.Instance.Host : sDevUrl }), getCommonData);
			}
			catch (Exception ex)
			{
				Logger.Info("Failed to send general stat for op : " + op + "...Err : " + ex.ToString());
			}
		});
	}

	internal static void SendStatsAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				SendStats(url, data, headers);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send stats for uri : " + url + ". Reason : " + ex.ToString());
			}
		});
	}

	internal static void SendPromotionAppClickStatsAsync(Dictionary<string, string> appData, string uri)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			Dictionary<string, string> getCommonData = GetCommonData;
			foreach (KeyValuePair<string, string> appDatum in appData)
			{
				getCommonData.Add(appDatum.Key, appDatum.Value);
			}
			SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[2]
			{
				string.IsNullOrEmpty(sDevUrl) ? RegistryManager.Instance.Host : sDevUrl,
				uri
			}), getCommonData);
		});
	}

	internal static void SendStats(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null, string vmname = "")
	{
		try
		{
			BstHttpClient.Post(url, data, headers, false, vmname, 0, 1, 0, false, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Info("Failed to send stats for : " + url + ". Reason : " + ex.ToString());
		}
	}
}
