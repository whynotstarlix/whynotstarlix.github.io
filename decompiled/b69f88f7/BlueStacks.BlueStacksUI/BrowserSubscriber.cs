using System;
using System.Collections.Generic;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class BrowserSubscriber : ISubscriber
{
	private BrowserControl mControl;

	private Dictionary<BrowserControlTags, object> mTokens = new Dictionary<BrowserControlTags, object>();

	public BrowserSubscriber(BrowserControl control)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		mControl = control;
		foreach (BrowserControlTags item in control?.TagsSubscribedDict.Keys)
		{
			SubscribeTag(item);
		}
	}

	public void SubscribeTag(BrowserControlTags args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected I4, but got Unknown
		switch ((int)args)
		{
		case 0:
			mTokens[(BrowserControlTags)0] = EventAggregator.Subscribe<BootCompleteEventArgs>((Action<BootCompleteEventArgs>)Message);
			break;
		case 1:
			mTokens[(BrowserControlTags)1] = EventAggregator.Subscribe<GoogleSignInCompleteEventArgs>((Action<GoogleSignInCompleteEventArgs>)Message);
			break;
		case 2:
			mTokens[(BrowserControlTags)2] = EventAggregator.Subscribe<AppPlayerClosingEventArgs>((Action<AppPlayerClosingEventArgs>)Message);
			break;
		case 3:
			mTokens[(BrowserControlTags)3] = EventAggregator.Subscribe<TabClosingEventArgs>((Action<TabClosingEventArgs>)Message);
			break;
		case 4:
			mTokens[(BrowserControlTags)4] = EventAggregator.Subscribe<TabSwitchedEventArgs>((Action<TabSwitchedEventArgs>)Message);
			break;
		case 5:
			mTokens[(BrowserControlTags)5] = EventAggregator.Subscribe<AppInstalledEventArgs>((Action<AppInstalledEventArgs>)Message);
			break;
		case 6:
			mTokens[(BrowserControlTags)6] = EventAggregator.Subscribe<AppUninstalledEventArgs>((Action<AppUninstalledEventArgs>)Message);
			break;
		case 7:
			mTokens[(BrowserControlTags)7] = EventAggregator.Subscribe<GrmAppListUpdateEventArgs>((Action<GrmAppListUpdateEventArgs>)Message);
			break;
		case 8:
			mTokens[(BrowserControlTags)8] = EventAggregator.Subscribe<ApkDownloadStartedEventArgs>((Action<ApkDownloadStartedEventArgs>)Message);
			break;
		case 9:
			mTokens[(BrowserControlTags)9] = EventAggregator.Subscribe<ApkDownloadFailedEventArgs>((Action<ApkDownloadFailedEventArgs>)Message);
			break;
		case 11:
			mTokens[(BrowserControlTags)11] = EventAggregator.Subscribe<ApkDownloadCompletedEventArgs>((Action<ApkDownloadCompletedEventArgs>)Message);
			break;
		case 10:
			mTokens[(BrowserControlTags)10] = EventAggregator.Subscribe<ApkDownloadCurrentProgressEventArgs>((Action<ApkDownloadCurrentProgressEventArgs>)Message);
			break;
		case 12:
			mTokens[(BrowserControlTags)12] = EventAggregator.Subscribe<ApkInstallStartedEventArgs>((Action<ApkInstallStartedEventArgs>)Message);
			break;
		case 13:
			mTokens[(BrowserControlTags)13] = EventAggregator.Subscribe<ApkInstallFailedEventArgs>((Action<ApkInstallFailedEventArgs>)Message);
			break;
		case 14:
			mTokens[(BrowserControlTags)14] = EventAggregator.Subscribe<ApkInstallCompletedEventArgs>((Action<ApkInstallCompletedEventArgs>)Message);
			break;
		case 15:
			mTokens[(BrowserControlTags)15] = EventAggregator.Subscribe<GetVmInfoEventArgs>((Action<GetVmInfoEventArgs>)Message);
			break;
		case 16:
			mTokens[(BrowserControlTags)16] = EventAggregator.Subscribe<UserInfoUpdatedEventArgs>((Action<UserInfoUpdatedEventArgs>)Message);
			break;
		case 17:
			mTokens[(BrowserControlTags)17] = EventAggregator.Subscribe<ThemeChangeEventArgs>((Action<ThemeChangeEventArgs>)Message);
			break;
		case 18:
			mTokens[(BrowserControlTags)18] = EventAggregator.Subscribe<OemDownloadStartedEventArgs>((Action<OemDownloadStartedEventArgs>)Message);
			break;
		case 19:
			mTokens[(BrowserControlTags)19] = EventAggregator.Subscribe<OemDownloadFailedEventArgs>((Action<OemDownloadFailedEventArgs>)Message);
			break;
		case 21:
			mTokens[(BrowserControlTags)21] = EventAggregator.Subscribe<OemDownloadCompletedEventArgs>((Action<OemDownloadCompletedEventArgs>)Message);
			break;
		case 20:
			mTokens[(BrowserControlTags)20] = EventAggregator.Subscribe<OemDownloadCurrentProgressEventArgs>((Action<OemDownloadCurrentProgressEventArgs>)Message);
			break;
		case 22:
			mTokens[(BrowserControlTags)22] = EventAggregator.Subscribe<OemInstallStartedEventArgs>((Action<OemInstallStartedEventArgs>)Message);
			break;
		case 23:
			mTokens[(BrowserControlTags)23] = EventAggregator.Subscribe<OemInstallFailedEventArgs>((Action<OemInstallFailedEventArgs>)Message);
			break;
		case 24:
			mTokens[(BrowserControlTags)24] = EventAggregator.Subscribe<OemInstallCompletedEventArgs>((Action<OemInstallCompletedEventArgs>)Message);
			break;
		case 25:
			mTokens[(BrowserControlTags)25] = EventAggregator.Subscribe<ShowFlePopupEventArgs>((Action<ShowFlePopupEventArgs>)Message);
			break;
		}
	}

	public void UnsubscribeTag(BrowserControlTags args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected I4, but got Unknown
		switch ((int)args)
		{
		case 0:
			EventAggregator.Unsubscribe<BootCompleteEventArgs>((Subscription<BootCompleteEventArgs>)mTokens[(BrowserControlTags)0]);
			break;
		case 1:
			EventAggregator.Unsubscribe<GoogleSignInCompleteEventArgs>((Subscription<GoogleSignInCompleteEventArgs>)mTokens[(BrowserControlTags)1]);
			break;
		case 2:
			EventAggregator.Unsubscribe<AppPlayerClosingEventArgs>((Subscription<AppPlayerClosingEventArgs>)mTokens[(BrowserControlTags)2]);
			break;
		case 3:
			EventAggregator.Unsubscribe<TabClosingEventArgs>((Subscription<TabClosingEventArgs>)mTokens[(BrowserControlTags)3]);
			break;
		case 4:
			EventAggregator.Unsubscribe<TabSwitchedEventArgs>((Subscription<TabSwitchedEventArgs>)mTokens[(BrowserControlTags)4]);
			break;
		case 5:
			EventAggregator.Unsubscribe<AppInstalledEventArgs>((Subscription<AppInstalledEventArgs>)mTokens[(BrowserControlTags)5]);
			break;
		case 6:
			EventAggregator.Unsubscribe<AppUninstalledEventArgs>((Subscription<AppUninstalledEventArgs>)mTokens[(BrowserControlTags)6]);
			break;
		case 7:
			EventAggregator.Unsubscribe<GrmAppListUpdateEventArgs>((Subscription<GrmAppListUpdateEventArgs>)mTokens[(BrowserControlTags)7]);
			break;
		case 8:
			EventAggregator.Unsubscribe<ApkDownloadStartedEventArgs>((Subscription<ApkDownloadStartedEventArgs>)mTokens[(BrowserControlTags)8]);
			break;
		case 9:
			EventAggregator.Unsubscribe<ApkDownloadFailedEventArgs>((Subscription<ApkDownloadFailedEventArgs>)mTokens[(BrowserControlTags)9]);
			break;
		case 11:
			EventAggregator.Unsubscribe<ApkDownloadCompletedEventArgs>((Subscription<ApkDownloadCompletedEventArgs>)mTokens[(BrowserControlTags)11]);
			break;
		case 10:
			EventAggregator.Unsubscribe<ApkDownloadCurrentProgressEventArgs>((Subscription<ApkDownloadCurrentProgressEventArgs>)mTokens[(BrowserControlTags)10]);
			break;
		case 12:
			EventAggregator.Unsubscribe<ApkInstallStartedEventArgs>((Subscription<ApkInstallStartedEventArgs>)mTokens[(BrowserControlTags)12]);
			break;
		case 13:
			EventAggregator.Unsubscribe<ApkInstallFailedEventArgs>((Subscription<ApkInstallFailedEventArgs>)mTokens[(BrowserControlTags)13]);
			break;
		case 14:
			EventAggregator.Unsubscribe<ApkInstallCompletedEventArgs>((Subscription<ApkInstallCompletedEventArgs>)mTokens[(BrowserControlTags)14]);
			break;
		case 15:
			EventAggregator.Unsubscribe<GetVmInfoEventArgs>((Subscription<GetVmInfoEventArgs>)mTokens[(BrowserControlTags)15]);
			break;
		case 16:
			EventAggregator.Unsubscribe<UserInfoUpdatedEventArgs>((Subscription<UserInfoUpdatedEventArgs>)mTokens[(BrowserControlTags)16]);
			break;
		case 17:
			EventAggregator.Unsubscribe<ThemeChangeEventArgs>((Subscription<ThemeChangeEventArgs>)mTokens[(BrowserControlTags)17]);
			break;
		case 18:
			EventAggregator.Unsubscribe<OemDownloadStartedEventArgs>((Subscription<OemDownloadStartedEventArgs>)mTokens[(BrowserControlTags)18]);
			break;
		case 19:
			EventAggregator.Unsubscribe<OemDownloadFailedEventArgs>((Subscription<OemDownloadFailedEventArgs>)mTokens[(BrowserControlTags)19]);
			break;
		case 21:
			EventAggregator.Unsubscribe<OemDownloadCompletedEventArgs>((Subscription<OemDownloadCompletedEventArgs>)mTokens[(BrowserControlTags)21]);
			break;
		case 20:
			EventAggregator.Unsubscribe<OemDownloadCurrentProgressEventArgs>((Subscription<OemDownloadCurrentProgressEventArgs>)mTokens[(BrowserControlTags)20]);
			break;
		case 22:
			EventAggregator.Unsubscribe<OemInstallStartedEventArgs>((Subscription<OemInstallStartedEventArgs>)mTokens[(BrowserControlTags)22]);
			break;
		case 23:
			EventAggregator.Unsubscribe<OemInstallFailedEventArgs>((Subscription<OemInstallFailedEventArgs>)mTokens[(BrowserControlTags)23]);
			break;
		case 24:
			EventAggregator.Unsubscribe<OemInstallCompletedEventArgs>((Subscription<OemInstallCompletedEventArgs>)mTokens[(BrowserControlTags)24]);
			break;
		case 25:
			EventAggregator.Unsubscribe<ShowFlePopupEventArgs>((Subscription<ShowFlePopupEventArgs>)mTokens[(BrowserControlTags)25]);
			break;
		}
	}

	public void Message(EventArgs eventArgs)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		BrowserEventArgs e = (BrowserEventArgs)(object)((eventArgs is BrowserEventArgs) ? eventArgs : null);
		if (e != null && (string.Equals(mControl.ParentWindow.mVmName, e.mVmName, StringComparison.InvariantCultureIgnoreCase) || (mControl.TagsSubscribedDict[e.ClientTag].ContainsKey("IsReceiveFromAllVm") && mControl.TagsSubscribedDict[e.ClientTag]["IsReceiveFromAllVm"].ToObject<bool>())))
		{
			JObject val = new JObject();
			val["eventRaised"] = JToken.op_Implicit(((object)e.ClientTag/*cast due to constrained. prefix*/).ToString());
			val["vmName"] = JToken.op_Implicit(e.mVmName);
			if (e.ExtraData != null)
			{
				val["extraData"] = (JToken)(object)e.ExtraData;
			}
			mControl.CallBackToHtml(((object)mControl.TagsSubscribedDict[e.ClientTag]["CallbackFunction"]).ToString(), ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
		}
	}
}
