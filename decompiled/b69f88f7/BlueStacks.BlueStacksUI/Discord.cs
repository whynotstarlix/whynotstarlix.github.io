using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Threading;
using BlueStacks.Common;
using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Message;

namespace BlueStacks.BlueStacksUI;

public class Discord : IDisposable
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<AppSuggestionPromotion, bool> _003C_003E9__12_0;

		public static OnReadyEvent _003C_003E9__18_0;

		internal bool _003CSetSystemAppsAndClientID_003Eb__12_0(AppSuggestionPromotion x)
		{
			return string.Equals(x.AppLocation, "more_apps", StringComparison.InvariantCulture);
		}

		internal void _003CInitDiscordClient_003Eb__18_0(object sender, ReadyMessage msg)
		{
			Logger.Info("Connected to discord with user {0}", new object[1] { msg.User.Username });
		}
	}

	private DiscordRpcClient mDiscordClient;

	private Timer mDiscordClientInvokeTimer;

	private string mPreviousAppPackage;

	private List<AppSuggestionPromotion> mSystemApps = new List<AppSuggestionPromotion>();

	private string mDiscordClientID = string.Empty;

	private Dictionary<string, Timestamps> mAppStartTimestamp = new Dictionary<string, Timestamps>();

	private MainWindow ParentWindow;

	private bool mIsDiscordConnected;

	private bool disposedValue;

	public Discord(MainWindow window)
	{
		ParentWindow = window;
	}

	internal void Init()
	{
	}

	private void AssignTabChangeEventOnOpenedTabs()
	{
		if (ParentWindow == null)
		{
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			foreach (AppTabButton item in ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList())
			{
				if (item.EventOnTabChanged == null)
				{
					Logger.Info("discord attaching tab change event on tab.." + item.PackageName);
					AssignTabChangeEvent(item);
					if (item.IsSelected)
					{
						Tab_ChangeOrCreateEvent(null, new TabChangeEventArgs(item.AppName, item.PackageName, item.mTabType));
					}
				}
				else if (item.IsSelected)
				{
					Tab_ChangeOrCreateEvent(null, new TabChangeEventArgs(item.AppName, item.PackageName, item.mTabType));
				}
			}
		}, new object[0]);
	}

	private void RemoveTabChangeEventFromOpenedTabs()
	{
		if (ParentWindow == null)
		{
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			foreach (AppTabButton item in ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList())
			{
				if (item.EventOnTabChanged != null)
				{
					item.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Remove(item.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(Tab_ChangeOrCreateEvent));
				}
			}
		}, new object[0]);
	}

	private void SetSystemAppsAndClientID()
	{
		if (PromotionObject.Instance != null)
		{
			mDiscordClientID = PromotionObject.Instance.DiscordClientID;
			mSystemApps = PromotionObject.Instance.AppSuggestionList.Where((AppSuggestionPromotion x) => string.Equals(x.AppLocation, "more_apps", StringComparison.InvariantCulture)).ToList();
		}
	}

	internal void RemoveAppFromTimestampList(string package)
	{
		if (mAppStartTimestamp.ContainsKey(package))
		{
			mAppStartTimestamp.Remove(package);
		}
	}

	internal bool IsDiscordClientReady()
	{
		if (mDiscordClient != null && mDiscordClient.IsInitialized)
		{
			return mIsDiscordConnected;
		}
		return false;
	}

	private void Tab_ChangeOrCreateEvent(object sender, TabChangeEventArgs e)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		try
		{
			if (string.Equals(mPreviousAppPackage, e.PackageName, StringComparison.InvariantCulture) || !IsDiscordClientReady())
			{
				return;
			}
			Logger.Info("Discord tab changed event. PkgName: {0}, AppName: {1}", new object[2] { e.PackageName, e.AppName });
			RichPresence val = new RichPresence();
			switch (e.TabType)
			{
			case TabType.WebTab:
			case TabType.HomeTab:
				if (e.PackageName.Contains("bluestacks") && e.PackageName.Contains("appcenter"))
				{
					val.State = "Searching";
					val.Details = "Google Play Store";
					val.Assets = new Assets
					{
						LargeImageKey = "bstk-logo",
						LargeImageText = "BlueStacks",
						SmallImageKey = "com_android_vending",
						SmallImageText = "Google Play"
					};
				}
				else
				{
					val.State = "In Lobby";
					val.Details = "About to start a game";
					val.Assets = new Assets
					{
						LargeImageKey = "bstk-logo",
						LargeImageText = "BlueStacks",
						SmallImageKey = "",
						SmallImageText = ""
					};
				}
				break;
			case TabType.AppTab:
				if (mSystemApps.Any((AppSuggestionPromotion _) => object.Equals(_.AppPackage == e.PackageName, StringComparison.InvariantCulture)))
				{
					val.State = "In Lobby";
					val.Details = "About to start a game";
					val.Assets = new Assets
					{
						LargeImageKey = "bstk-logo",
						LargeImageText = "BlueStacks",
						SmallImageKey = "",
						SmallImageText = ""
					};
					break;
				}
				if (e.PackageName.Contains("android.vending"))
				{
					val.State = "Searching";
					val.Details = "Google Play Store";
					val.Assets = new Assets
					{
						LargeImageKey = "bstk-logo",
						LargeImageText = "BlueStacks",
						SmallImageKey = "com_android_vending",
						SmallImageText = "Google Play"
					};
					break;
				}
				if (mAppStartTimestamp.ContainsKey(e.PackageName))
				{
					val.Timestamps = mAppStartTimestamp[e.PackageName];
				}
				else
				{
					val.Timestamps = Timestamps.Now;
					mAppStartTimestamp.Add(e.PackageName, Timestamps.Now);
				}
				val.State = "Playing";
				val.Details = e.AppName;
				val.Assets = new Assets
				{
					LargeImageKey = GetMD5HashFromPackageName(e.PackageName),
					LargeImageText = e.AppName,
					SmallImageKey = "bstk-logo",
					SmallImageText = "BlueStacks"
				};
				break;
			}
			SetPresence(val);
			mPreviousAppPackage = e.PackageName;
		}
		catch (Exception ex)
		{
			Logger.Error("Error while setting presence in discord with exception : {0}", new object[1] { ex.ToString() });
		}
	}

	private string GetMD5HashFromPackageName(string package)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		string text = new _MD5
		{
			Value = package
		}.FingerPrint.ToLower(CultureInfo.InvariantCulture);
		Logger.Info("Md5 hash for package name: {0}..is {1}", new object[2] { package, text });
		return text;
	}

	internal void AssignTabChangeEvent(AppTabButton button)
	{
		if (button.EventOnTabChanged == null)
		{
			button.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Combine(button.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(Tab_ChangeOrCreateEvent));
		}
	}

	private void InitDiscordClient()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		try
		{
			if (mDiscordClient != null)
			{
				return;
			}
			Logger.Info("Initing discord");
			mDiscordClient = new DiscordRpcClient(mDiscordClientID);
			DiscordRpcClient obj = mDiscordClient;
			object obj2 = _003C_003Ec._003C_003E9__18_0;
			if (obj2 == null)
			{
				OnReadyEvent val = delegate(object sender, ReadyMessage msg)
				{
					Logger.Info("Connected to discord with user {0}", new object[1] { msg.User.Username });
				};
				_003C_003Ec._003C_003E9__18_0 = val;
				obj2 = (object)val;
			}
			obj.OnReady += (OnReadyEvent)obj2;
			mDiscordClient.OnPresenceUpdate += new OnPresenceUpdateEvent(Client_OnPresenceUpdate);
			mDiscordClient.OnError += new OnErrorEvent(Client_OnError);
			mDiscordClient.OnConnectionFailed += new OnConnectionFailedEvent(Client_OnConnectionFailed);
			mDiscordClient.OnConnectionEstablished += new OnConnectionEstablishedEvent(Client_OnConnectionEstablished);
			mDiscordClientInvokeTimer = new Timer(150.0);
			mDiscordClientInvokeTimer.Elapsed += delegate
			{
				mDiscordClient.Invoke();
			};
			mDiscordClientInvokeTimer.Start();
			bool flag = mDiscordClient.Initialize();
			Logger.Info("Discord client init: {0}", new object[1] { flag });
		}
		catch (Exception ex)
		{
			Logger.Info("Exception in Discord init. ex:  " + ex.ToString());
		}
	}

	private void Client_OnPresenceUpdate(object sender, PresenceMessage args)
	{
		object obj;
		if (args == null)
		{
			obj = null;
		}
		else
		{
			RichPresence presence = args.Presence;
			obj = ((presence != null) ? presence.Details : null);
		}
		Logger.Info("Discord presence has been updated with details." + (string?)obj);
		if (args.Presence.Assets.LargeImageKey == null)
		{
			RichPresence val = args.Presence.Clone();
			val.Assets.LargeImageKey = "bstk-logo";
			val.Assets.SmallImageKey = "";
			val.Assets.SmallImageText = "";
			SetPresence(val);
		}
	}

	private void SetPresence(RichPresence presence)
	{
		if (mDiscordClient != null && mDiscordClient.IsInitialized)
		{
			mDiscordClient.SetPresence(presence);
		}
		else
		{
			Logger.Warning("SetPresence called without a client being inited");
		}
	}

	private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
	{
		Logger.Info("Discord connection Established");
		mIsDiscordConnected = true;
		AssignTabChangeEventOnOpenedTabs();
		ClientStats.SendMiscellaneousStatsAsync("DiscordConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, null);
	}

	private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Discord connection failed. ErrorCode: {0}", new object[1] { ((IMessage)args).Type });
		mIsDiscordConnected = false;
		Dispose();
		ClientStats.SendMiscellaneousStatsAsync("DiscordNotConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, null);
	}

	private void Client_OnError(object sender, ErrorMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Discord client error. ErrorCode: {0}, Message: {1}", new object[2] { args.Code, args.Message });
	}

	internal void ToggleDiscordState(bool state)
	{
		if (state)
		{
			if (mDiscordClient == null)
			{
				Init();
			}
		}
		else
		{
			Dispose();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		if (disposedValue)
		{
			return;
		}
		if (mDiscordClient != null)
		{
			mDiscordClient.OnPresenceUpdate -= new OnPresenceUpdateEvent(Client_OnPresenceUpdate);
			mDiscordClient.OnError -= new OnErrorEvent(Client_OnError);
			mDiscordClient.OnConnectionFailed -= new OnConnectionFailedEvent(Client_OnConnectionFailed);
			mDiscordClient.OnConnectionEstablished -= new OnConnectionEstablishedEvent(Client_OnConnectionEstablished);
			mDiscordClient.Dispose();
			RemoveTabChangeEventFromOpenedTabs();
		}
		if (mDiscordClientInvokeTimer != null)
		{
			mDiscordClientInvokeTimer.Elapsed -= delegate
			{
				mDiscordClient.Invoke();
			};
			mDiscordClientInvokeTimer.Dispose();
		}
		disposedValue = true;
	}

	~Discord()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
