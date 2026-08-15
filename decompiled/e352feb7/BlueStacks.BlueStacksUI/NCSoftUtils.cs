using System;
using System.Collections.Generic;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class NCSoftUtils
{
	private static object sync = new object();

	internal List<string> BlackListedApps = new List<string> { "com.bluestacks", "com.google", "com.android", "com.uncube" };

	private int mNCSoftAgentPort = -1;

	private static NCSoftUtils mInstance = null;

	internal static NCSoftUtils Instance
	{
		get
		{
			if (mInstance == null)
			{
				lock (sync)
				{
					if (mInstance == null)
					{
						mInstance = new NCSoftUtils();
					}
				}
			}
			return mInstance;
		}
	}

	private int GetNCSoftAgentPort()
	{
		if (mNCSoftAgentPort != -1)
		{
			return mNCSoftAgentPort;
		}
		string text = "ngpmmf";
		uint num = 2u;
		try
		{
			mNCSoftAgentPort = MemoryMappedFile.GetNCSoftAgentPort(text, num);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to get ncsoft agent port");
			Logger.Error(ex.ToString());
		}
		return mNCSoftAgentPort;
	}

	internal void SendAppCrashEvent(string crashReason, string vmName)
	{
		try
		{
			int nCSoftAgentPort = GetNCSoftAgentPort();
			if (nCSoftAgentPort != -1)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "vm_name", vmName },
					{ "err_message", crashReason }
				};
				Logger.Info("Sending app crash event to NCSoft Agent for vm: " + vmName);
				Logger.Info("Reason: " + crashReason);
				string text = HTTPUtils.SendRequestToNCSoftAgent(nCSoftAgentPort, "error/crash", dictionary, vmName, 0, (Dictionary<string, string>)null, false, 1, 0);
				Logger.Info("app crash event resp:");
				Logger.Info(text);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to report app crash. Ex : " + ex.ToString());
		}
	}

	internal void SendGoogleLoginEventAsync(string vmName)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				int nCSoftAgentPort = GetNCSoftAgentPort();
				if (nCSoftAgentPort != -1)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "vm_name", vmName },
						{ "first", "true" }
					};
					Logger.Info("Sending google login event to NCSoft Agent for vm: " + vmName);
					string text = HTTPUtils.SendRequestToNCSoftAgent(nCSoftAgentPort, "account/google/login", dictionary, vmName, 0, (Dictionary<string, string>)null, false, 1, 0);
					Logger.Info("account google login event resp:");
					Logger.Info(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to report google login. Ex : " + ex.ToString());
			}
		});
	}

	internal void SendStreamingEvent(string vmName, string streamingStatus)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				int nCSoftAgentPort = GetNCSoftAgentPort();
				if (nCSoftAgentPort != -1)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "button", "streaming" },
						{ "state", streamingStatus },
						{ "vm_name", vmName }
					};
					Logger.Info("Sending streaming event to NCSoft Agent for vm: " + vmName);
					Logger.Info("Status : " + streamingStatus);
					string text = HTTPUtils.SendRequestToNCSoftAgent(nCSoftAgentPort, "action/button/streaming", dictionary, vmName, 0, (Dictionary<string, string>)null, false, 1, 0);
					Logger.Info("action button streaming event resp:");
					Logger.Info(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to report action button streaming. Ex : " + ex.ToString());
			}
		});
	}
}
