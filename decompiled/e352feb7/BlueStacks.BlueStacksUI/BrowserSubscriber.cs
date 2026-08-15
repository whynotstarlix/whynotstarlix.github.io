using System;
using System.Collections.Generic;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class BrowserSubscriber : ISubscriber
{
	private BrowserControl mControl;

	private Dictionary<BrowserControlTags, object> mTokens;

	public BrowserSubscriber(BrowserControl control)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		mTokens = new Dictionary<BrowserControlTags, object>();
		base._002Ector();
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
			mTokens[(BrowserControlTags)(0x2BBEFF2D ^ 0x2BBEFF2F)] = EventAggregator.Subscribe<AppPlayerClosingEventArgs>((Action<AppPlayerClosingEventArgs>)Message);
			break;
		case 3:
			mTokens[(BrowserControlTags)(881338646 + -881338643)] = EventAggregator.Subscribe<TabClosingEventArgs>((Action<TabClosingEventArgs>)Message);
			break;
		case 4:
			mTokens[(BrowserControlTags)(0x5EC9749B ^ 0x5EC9749F)] = EventAggregator.Subscribe<TabSwitchedEventArgs>((Action<TabSwitchedEventArgs>)Message);
			break;
		case 5:
			mTokens[(BrowserControlTags)(-1822558640 ^ -1822558635)] = EventAggregator.Subscribe<AppInstalledEventArgs>((Action<AppInstalledEventArgs>)Message);
			break;
		case 6:
			mTokens[(BrowserControlTags)(0x704C0006 ^ 0x704C0000)] = EventAggregator.Subscribe<AppUninstalledEventArgs>((Action<AppUninstalledEventArgs>)Message);
			break;
		case 7:
			mTokens[(BrowserControlTags)(7 + (1688665258 << 1077069503))] = EventAggregator.Subscribe<GrmAppListUpdateEventArgs>((Action<GrmAppListUpdateEventArgs>)Message);
			break;
		case 8:
			mTokens[(BrowserControlTags)(0x1AA2A3A ^ 0x1AA2A32)] = EventAggregator.Subscribe<ApkDownloadStartedEventArgs>((Action<ApkDownloadStartedEventArgs>)Message);
			break;
		case 9:
			mTokens[(BrowserControlTags)(0x683 ^ 0x68A)] = EventAggregator.Subscribe<ApkDownloadFailedEventArgs>((Action<ApkDownloadFailedEventArgs>)Message);
			break;
		case 11:
			mTokens[(BrowserControlTags)(2128600998 - (0x56DED719 | 0x784FCD8A))] = EventAggregator.Subscribe<ApkDownloadCompletedEventArgs>((Action<ApkDownloadCompletedEventArgs>)Message);
			break;
		case 10:
			mTokens[(BrowserControlTags)(318674 - (163156045 >> 1134338665))] = EventAggregator.Subscribe<ApkDownloadCurrentProgressEventArgs>((Action<ApkDownloadCurrentProgressEventArgs>)Message);
			break;
		case 12:
			mTokens[(BrowserControlTags)(1547697277 - (0xC39F831 | 0x5C37D461))] = EventAggregator.Subscribe<ApkInstallStartedEventArgs>((Action<ApkInstallStartedEventArgs>)Message);
			break;
		case 13:
			mTokens[(BrowserControlTags)(-1803550482 + (0x687ECC1F | 0x6355B31E))] = EventAggregator.Subscribe<ApkInstallFailedEventArgs>((Action<ApkInstallFailedEventArgs>)Message);
			break;
		case 14:
			mTokens[(BrowserControlTags)(60342996 - 1081132250 % 255197317)] = EventAggregator.Subscribe<ApkInstallCompletedEventArgs>((Action<ApkInstallCompletedEventArgs>)Message);
			break;
		case 15:
			mTokens[(BrowserControlTags)(-32392 + (2123863011 >> 1167194512))] = EventAggregator.Subscribe<GetVmInfoEventArgs>((Action<GetVmInfoEventArgs>)Message);
			break;
		case 16:
			mTokens[(BrowserControlTags)(208225 - (1705649657 >> 144242605))] = EventAggregator.Subscribe<UserInfoUpdatedEventArgs>((Action<UserInfoUpdatedEventArgs>)Message);
			break;
		case 17:
			mTokens[(BrowserControlTags)(0x135 ^ 0x124)] = EventAggregator.Subscribe<ThemeChangeEventArgs>((Action<ThemeChangeEventArgs>)Message);
			break;
		case 18:
			mTokens[(BrowserControlTags)(-789152343 ^ -789152325)] = EventAggregator.Subscribe<OemDownloadStartedEventArgs>((Action<OemDownloadStartedEventArgs>)Message);
			break;
		case 19:
			mTokens[(BrowserControlTags)(0x47DF7C13 ^ 0x47DF7C00)] = EventAggregator.Subscribe<OemDownloadFailedEventArgs>((Action<OemDownloadFailedEventArgs>)Message);
			break;
		case 21:
			mTokens[(BrowserControlTags)(-475098542 ^ -475098553)] = EventAggregator.Subscribe<OemDownloadCompletedEventArgs>((Action<OemDownloadCompletedEventArgs>)Message);
			break;
		case 20:
			mTokens[(BrowserControlTags)(0x5EFD5DA ^ 0x5EFD5CE)] = EventAggregator.Subscribe<OemDownloadCurrentProgressEventArgs>((Action<OemDownloadCurrentProgressEventArgs>)Message);
			break;
		case 22:
			mTokens[(BrowserControlTags)(0x5DC18016 ^ 0x5DC18000)] = EventAggregator.Subscribe<OemInstallStartedEventArgs>((Action<OemInstallStartedEventArgs>)Message);
			break;
		case 23:
			mTokens[(BrowserControlTags)(0x333B36CC ^ 0x333B36DB)] = EventAggregator.Subscribe<OemInstallFailedEventArgs>((Action<OemInstallFailedEventArgs>)Message);
			break;
		case 24:
		{
			Dictionary<BrowserControlTags, object> dictionary = mTokens;
			int num = ((1929986216 > 628662618) ? 24 : 32);
			dictionary[(BrowserControlTags)num] = EventAggregator.Subscribe<OemInstallCompletedEventArgs>((Action<OemInstallCompletedEventArgs>)Message);
			break;
		}
		case 25:
			mTokens[(BrowserControlTags)(0x382B6 ^ 0x382AF)] = EventAggregator.Subscribe<ShowFlePopupEventArgs>((Action<ShowFlePopupEventArgs>)Message);
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
			EventAggregator.Unsubscribe<AppPlayerClosingEventArgs>((Subscription<AppPlayerClosingEventArgs>)mTokens[(BrowserControlTags)(-2129543163 + (0x50EC3F79 | 0x7E2E2C95))]);
			break;
		case 3:
		{
			Dictionary<BrowserControlTags, object> dictionary2 = mTokens;
			int num2 = ((1605013623 > 449611261) ? 3 : 4);
			EventAggregator.Unsubscribe<TabClosingEventArgs>((Subscription<TabClosingEventArgs>)dictionary2[(BrowserControlTags)num2]);
			break;
		}
		case 4:
			EventAggregator.Unsubscribe<TabSwitchedEventArgs>((Subscription<TabSwitchedEventArgs>)mTokens[(BrowserControlTags)(0x300004 ^ 0x300000)]);
			break;
		case 5:
			EventAggregator.Unsubscribe<AppInstalledEventArgs>((Subscription<AppInstalledEventArgs>)mTokens[(BrowserControlTags)(-1186837915 ^ -1186837920)]);
			break;
		case 6:
			EventAggregator.Unsubscribe<AppUninstalledEventArgs>((Subscription<AppUninstalledEventArgs>)mTokens[(BrowserControlTags)(2093744134 - (913538252 << 836460208))]);
			break;
		case 7:
			EventAggregator.Unsubscribe<GrmAppListUpdateEventArgs>((Subscription<GrmAppListUpdateEventArgs>)mTokens[(BrowserControlTags)(1459088846 - (0x4657C4C5 | 0x16B3AD42))]);
			break;
		case 8:
			EventAggregator.Unsubscribe<ApkDownloadStartedEventArgs>((Subscription<ApkDownloadStartedEventArgs>)mTokens[(BrowserControlTags)(-2144829608 ^ -2144829616)]);
			break;
		case 9:
		{
			Dictionary<BrowserControlTags, object> dictionary = mTokens;
			int num = ((468154240 > 914069894) ? 12 : 9);
			EventAggregator.Unsubscribe<ApkDownloadFailedEventArgs>((Subscription<ApkDownloadFailedEventArgs>)dictionary[(BrowserControlTags)num]);
			break;
		}
		case 11:
			EventAggregator.Unsubscribe<ApkDownloadCompletedEventArgs>((Subscription<ApkDownloadCompletedEventArgs>)mTokens[(BrowserControlTags)(2062516051 - (0x7AE86F08 | 0x402F7E40))]);
			break;
		case 10:
			EventAggregator.Unsubscribe<ApkDownloadCurrentProgressEventArgs>((Subscription<ApkDownloadCurrentProgressEventArgs>)mTokens[(BrowserControlTags)(720833269 + ~720833258)]);
			break;
		case 12:
			EventAggregator.Unsubscribe<ApkInstallStartedEventArgs>((Subscription<ApkInstallStartedEventArgs>)mTokens[(BrowserControlTags)(-2112242195 ^ -2112242207)]);
			break;
		case 13:
			EventAggregator.Unsubscribe<ApkInstallFailedEventArgs>((Subscription<ApkInstallFailedEventArgs>)mTokens[(BrowserControlTags)(1398475005 + -1398474992)]);
			break;
		case 14:
			EventAggregator.Unsubscribe<ApkInstallCompletedEventArgs>((Subscription<ApkInstallCompletedEventArgs>)mTokens[(BrowserControlTags)(-1766686706 + (382945900 << 665219341))]);
			break;
		case 15:
			EventAggregator.Unsubscribe<GetVmInfoEventArgs>((Subscription<GetVmInfoEventArgs>)mTokens[(BrowserControlTags)(0xE ^ 1)]);
			break;
		case 16:
			EventAggregator.Unsubscribe<UserInfoUpdatedEventArgs>((Subscription<UserInfoUpdatedEventArgs>)mTokens[(BrowserControlTags)(-1527268852 ^ -1527268836)]);
			break;
		case 17:
			EventAggregator.Unsubscribe<ThemeChangeEventArgs>((Subscription<ThemeChangeEventArgs>)mTokens[(BrowserControlTags)(-1476373997 + (0x56FF8DEC | 0x111FA1DA))]);
			break;
		case 18:
			EventAggregator.Unsubscribe<OemDownloadStartedEventArgs>((Subscription<OemDownloadStartedEventArgs>)mTokens[(BrowserControlTags)(0x16120012 ^ 0x16120000)]);
			break;
		case 19:
			EventAggregator.Unsubscribe<OemDownloadFailedEventArgs>((Subscription<OemDownloadFailedEventArgs>)mTokens[(BrowserControlTags)(-2056500250 - ~2056500268)]);
			break;
		case 21:
			EventAggregator.Unsubscribe<OemDownloadCompletedEventArgs>((Subscription<OemDownloadCompletedEventArgs>)mTokens[(BrowserControlTags)(-1323380194 ^ -1323380213)]);
			break;
		case 20:
			EventAggregator.Unsubscribe<OemDownloadCurrentProgressEventArgs>((Subscription<OemDownloadCurrentProgressEventArgs>)mTokens[(BrowserControlTags)(1188762166 + -1188762146)]);
			break;
		case 22:
			EventAggregator.Unsubscribe<OemInstallStartedEventArgs>((Subscription<OemInstallStartedEventArgs>)mTokens[(BrowserControlTags)(690792849 - 1625171751 % 934378924)]);
			break;
		case 23:
			EventAggregator.Unsubscribe<OemInstallFailedEventArgs>((Subscription<OemInstallFailedEventArgs>)mTokens[(BrowserControlTags)(20326323 + ~20326299)]);
			break;
		case 24:
			EventAggregator.Unsubscribe<OemInstallCompletedEventArgs>((Subscription<OemInstallCompletedEventArgs>)mTokens[(BrowserControlTags)(0x2CE7DF61 ^ 0x2CE7DF79)]);
			break;
		case 25:
			EventAggregator.Unsubscribe<ShowFlePopupEventArgs>((Subscription<ShowFlePopupEventArgs>)mTokens[(BrowserControlTags)(-1678196512 - ~1678196536)]);
			break;
		}
	}

	public void Message(EventArgs eventArgs)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
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
