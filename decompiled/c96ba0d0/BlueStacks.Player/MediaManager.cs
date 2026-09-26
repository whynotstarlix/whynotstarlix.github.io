using System;
using System.Collections.Generic;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Player;

internal static class MediaManager
{
	internal static volatile bool mIsMutedExplicitly = RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].IsMuted || RegistryManager.Instance.AreAllInstancesMuted;

	private static object syncRoot = new object();

	public static int GetGuestVolume(string mediaType = null)
	{
		int result = -1;
		try
		{
			lock (syncRoot)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (string.IsNullOrEmpty(mediaType))
				{
					dictionary.Add("mediatype", "");
				}
				else
				{
					dictionary.Add("mediatype", mediaType);
				}
				JObject val = JObject.Parse(HTTPUtils.SendRequestToGuest("getVolume", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
				if (((object)val["result"]).ToString() == "ok")
				{
					return Convert.ToInt32(((object)val["volume"]).ToString());
				}
				Logger.Error("Couldn't get volume");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("An error occured while getting the volume from guest: {0}", new object[1] { ex });
		}
		return result;
	}

	public static bool SetGuestVolume(int volume)
	{
		try
		{
			lock (syncRoot)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"vol",
					volume.ToString()
				} };
				Logger.Info("Sending request to set volume {0}", new object[1] { volume });
				string text = HTTPUtils.SendRequestToGuest("setVolume", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				Logger.Info("The response for SetVolume is {0}", new object[1] { text });
				return ((object)JObject.Parse(text)["result"]).ToString() == "ok";
			}
		}
		catch (Exception ex)
		{
			Logger.Error("An error occured while setting guest volume: {0}", new object[1] { ex });
		}
		return false;
	}

	internal static void MuteEngine(bool isMutedExplicitly = false, bool updateRegistry = true)
	{
		ThreadPool.QueueUserWorkItem((object obj) =>
		{
			Logger.Info("Mute engine start with muteFromUser {0}", new object[1] { isMutedExplicitly.ToString() });
			SendMuteEventToGuest(mute: true);
			if (isMutedExplicitly)
			{
				mIsMutedExplicitly = isMutedExplicitly;
				if (updateRegistry)
				{
					RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].IsMuted = true;
				}
			}
		});
	}

	internal static void UnmuteEngine(bool updateRegistry = true)
	{
		ThreadPool.QueueUserWorkItem((object obj) =>
		{
			Logger.Info("Unmute Engine Start");
			mIsMutedExplicitly = false;
			SendMuteEventToGuest(mute: false);
			if (updateRegistry)
			{
				RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].IsMuted = false;
			}
		});
	}

	private static void SendMuteEventToGuest(bool mute)
	{
		lock (syncRoot)
		{
			string empty = string.Empty;
			try
			{
				empty = ((!mute) ? HTTPUtils.SendRequestToGuest("unmuteAppPlayer", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64") : HTTPUtils.SendRequestToGuest("muteAppPlayer", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
				Logger.Info("The result for mute: {0} is {1}", new object[2] { mute, empty });
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in SendMuteEventToGuest. Err: " + ex.ToString());
			}
		}
	}
}
