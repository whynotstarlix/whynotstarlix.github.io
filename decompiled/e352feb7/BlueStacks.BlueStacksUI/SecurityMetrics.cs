using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class SecurityMetrics : IDisposable
{
	private Dictionary<SecurityBreach, string> mSecurityBreachesList = new Dictionary<SecurityBreach, string>();

	private string mVmName;

	private System.Timers.Timer mTimer;

	private bool disposedValue;

	public static SerializableDictionary<string, SecurityMetrics> SecurityMetricsInstanceList { get; set; } = new SerializableDictionary<string, SecurityMetrics>();

	public SecurityMetrics(string vmName)
	{
		mVmName = vmName;
		mTimer = new System.Timers.Timer
		{
			Interval = 86400000.0
		};
		mTimer.Elapsed += OnTimedEvent;
		mTimer.AutoReset = true;
		mTimer.Enabled = true;
		Thread thread = new Thread((ThreadStart)delegate
		{
			CheckMd5HashOfRootVdi();
			CheckAppPlayerRootInfoFromAndroidBstk();
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void OnTimedEvent(object sender, ElapsedEventArgs e)
	{
		SendSecurityBreachesStatsToCloud();
	}

	internal static void Init(string vmName)
	{
		if (!((Dictionary<string, SecurityMetrics>)(object)SecurityMetricsInstanceList).ContainsKey(vmName))
		{
			((Dictionary<string, SecurityMetrics>)(object)SecurityMetricsInstanceList).Add(vmName, new SecurityMetrics(vmName));
		}
	}

	internal void SendSecurityBreachesStatsToCloud(bool isOnClose = false)
	{
		new Thread((ThreadStart)delegate
		{
			try
			{
				AddBlacklistedRunningApplicationsToSecurityBreaches();
				if (mSecurityBreachesList.Count > 0)
				{
					string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
					{
						RegistryManager.Instance.Host,
						"/bs4/security_metrics"
					}));
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"security_metric_data",
						GetSecurityMetricsData()
					} };
					BstHttpClient.Post(urlWithParams, dictionary, (Dictionary<string, string>)null, false, mVmName, 10000, 1, 0, false, "bgp64");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while sending security stats to cloud : {0}", new object[1] { ex.ToString() });
			}
			if (isOnClose)
			{
				((Dictionary<string, SecurityMetrics>)(object)SecurityMetricsInstanceList).Remove(mVmName);
			}
		}).Start();
	}

	private string GetSecurityMetricsData()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		using StringWriter stringWriter = new StringWriter(stringBuilder);
		JsonWriter val = (JsonWriter)new JsonTextWriter((TextWriter)stringWriter)
		{
			Formatting = (Formatting)1
		};
		try
		{
			val.WriteStartObject();
			foreach (SecurityBreach key in mSecurityBreachesList.Keys)
			{
				switch (key)
				{
				case SecurityBreach.DEVICE_PROBED:
				case SecurityBreach.DEVICE_ROOTED:
				case SecurityBreach.DEVICE_PROFILE_CHANGED:
				case SecurityBreach.SYNTHETIC_INPUT:
					val.WritePropertyName(key.ToString().ToLower(CultureInfo.InvariantCulture));
					val.WriteValue(mSecurityBreachesList[key]);
					break;
				case SecurityBreach.SCRIPT_TOOLS:
					val.WritePropertyName(key.ToString().ToLower(CultureInfo.InvariantCulture));
					val.WriteStartObject();
					val.WritePropertyName("running_blacklist_programs");
					val.WriteValue(mSecurityBreachesList[key]);
					val.WriteEndObject();
					break;
				}
			}
			val.WriteEndObject();
			empty = stringBuilder.ToString();
			Logger.Debug("security data " + empty);
			return empty;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void AddBlacklistedRunningApplicationsToSecurityBreaches()
	{
		List<string> blackListedApplicationsList = PromotionObject.Instance.BlackListedApplicationsList;
		List<string> list = new List<string>();
		foreach (string item in blackListedApplicationsList)
		{
			if (ProcessUtils.FindProcessByName(item))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			string data = JsonConvert.SerializeObject((object)list);
			AddSecurityBreach(SecurityBreach.SCRIPT_TOOLS, data);
		}
	}

	internal void AddSecurityBreach(SecurityBreach breach, string data)
	{
		try
		{
			if (!mSecurityBreachesList.ContainsKey(breach))
			{
				mSecurityBreachesList.Add(breach, data);
				Logger.Info("Security breach added for: {0}", new object[1] { breach });
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in adding security breach: {0}", new object[1] { ex.ToString() });
		}
	}

	internal void CheckMd5HashOfRootVdi()
	{
		try
		{
			string blockDevice0Path = RegistryManager.Instance.Guest["Android"].BlockDevice0Path;
			string rootVdiMd5Hash = RegistryManager.Instance.RootVdiMd5Hash;
			if (string.IsNullOrEmpty(rootVdiMd5Hash))
			{
				Utils.CreateMD5HashOfRootVdi();
				return;
			}
			string mD5HashFromFile = Utils.GetMD5HashFromFile(blockDevice0Path);
			if (!string.IsNullOrEmpty(mD5HashFromFile) && !string.Equals(mD5HashFromFile, rootVdiMd5Hash, StringComparison.OrdinalIgnoreCase))
			{
				AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in checking md5 hash of root vdi: {0}", new object[1] { ex });
		}
	}

	private void CheckAppPlayerRootInfoFromAndroidBstk()
	{
		try
		{
			JArray val = JArray.Parse(HTTPUtils.SendRequestToEngine("isAppPlayerRooted", (Dictionary<string, string>)null, mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, ""));
			if ((bool)val[0][(object)"success"] && (bool)val[0][(object)"isRooted"])
			{
				AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in checking root info from engine: {0}", new object[1] { ex.ToString() });
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (mTimer != null)
			{
				mTimer.Elapsed -= OnTimedEvent;
				mTimer.Dispose();
			}
			disposedValue = true;
		}
	}

	~SecurityMetrics()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
