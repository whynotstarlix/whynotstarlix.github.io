using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AboutSettingsControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPoweredByBSGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBSIconAndNameGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mProductTextGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mVersionLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mUpdateInfoGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label bodyLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mLabelVersion;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mDetailedChangeLogs;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mCheckUpdateBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mStatusLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCheckingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ContactInfoGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mWebsiteLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mSupportLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mSupportMailLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mSupportEMailHyperlink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTermsOfUse;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mTermsOfUseLink;

	private bool _contentLoaded;

	public AboutSettingsControl(MainWindow window, SettingsWindow _)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		((UserControl)this)._002Ector();
		ParentWindow = window;
		InitializeComponent();
		((UIElement)this).Visibility = (Visibility)1;
		((FrameworkElement)this).Loaded += new RoutedEventHandler(AboutSettingsControl_Loaded);
		AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel("bgp64", Utils.GetValueInBootParams("abivalue", "Android", string.Empty, "bgp64"));
		((ContentControl)mVersionLabel).Content = "v" + RegistryManager.Instance.Version;
		BlueStacksUIBinding.Bind(mSupportLabel, "STRING_SUPPORT");
		((ContentControl)mSupportLabel).ContentStringFormat = "{0} - ";
		BlueStacksUIBinding.Bind(mWebsiteLabel, "STRING_WEBSITE");
		((ContentControl)mWebsiteLabel).ContentStringFormat = "{0} - ";
		BlueStacksUIBinding.Bind(mSupportMailLabel, "STRING_SUPPORT_EMAIL");
		((ContentControl)mSupportMailLabel).ContentStringFormat = "{0} - ";
		if (Oem.Instance.IsProductBeta)
		{
			Label obj = mVersionLabel;
			((ContentControl)obj).Content = ((ContentControl)obj).Content?.ToString() + LocaleStrings.GetLocalizedString("STRING_BETA", "");
		}
		if (appPlayerModel != null && appPlayerModel.AppPlayerOsArch != null)
		{
			Label obj2 = mVersionLabel;
			((ContentControl)obj2).Content = ((ContentControl)obj2).Content?.ToString() + ", " + appPlayerModel.AppPlayerOsArch;
		}
		else if (Oem.Instance.IsAndroid64Bit)
		{
			Label obj3 = mVersionLabel;
			((ContentControl)obj3).Content = ((ContentControl)obj3).Content?.ToString() + ", " + LocaleStrings.GetLocalizedString("STRING_64BIT_ANDROID", "");
		}
		else
		{
			Label obj4 = mVersionLabel;
			((ContentControl)obj4).Content = ((ContentControl)obj4).Content?.ToString() + ", " + LocaleStrings.GetLocalizedString("STRING_32BIT_ANDROID", "");
		}
		string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=";
		mTermsOfUseLink.NavigateUri = new Uri(text + "terms_of_use");
		((TextElementCollection<Inline>)(object)((Span)mDetailedChangeLogs).Inlines).Clear();
		((Span)mDetailedChangeLogs).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
	}

	private void AboutSettingsControl_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
		{
			((UIElement)ContactInfoGrid).Visibility = (Visibility)1;
		}
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			((UIElement)mPoweredByBSGrid).Visibility = (Visibility)0;
			((UIElement)mBSIconAndNameGrid).Visibility = (Visibility)1;
		}
		if (Oem.Instance.IsProductBeta)
		{
			string text = "beta-support@bluestacks.com";
			((TextElementCollection<Inline>)(object)((Span)mSupportEMailHyperlink).Inlines).Clear();
			((Span)mSupportEMailHyperlink).Inlines.Add(text);
			mSupportEMailHyperlink.NavigateUri = new Uri("mailto:" + text);
		}
		HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
		BlueStacksUpdater.StateChanged -= BlueStacksUpdater_StateChanged;
		BlueStacksUpdater.StateChanged += BlueStacksUpdater_StateChanged;
	}

	private void BlueStacksUpdater_StateChanged()
	{
		HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
	}

	private void HandleUpdateStateGridVisibility(BlueStacksUpdater.UpdateState state)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			switch (state)
			{
			case BlueStacksUpdater.UpdateState.NO_UPDATE:
				((UIElement)mUpdateInfoGrid).Visibility = (Visibility)(-1256580389 ^ -1256580391);
				((FrameworkElement)mCheckUpdateBtn).HorizontalAlignment = (HorizontalAlignment)0;
				((UIElement)mCheckUpdateBtn).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind((Button)(object)mCheckUpdateBtn, "STRING_CHECK_UPDATES");
				((UIElement)mStatusLabel).Visibility = (Visibility)(-289928846 + 1745945794 % 728008473);
				((UIElement)mCheckingGrid).Visibility = (Visibility)(182321921 + ~182321918);
				break;
			case BlueStacksUpdater.UpdateState.UPDATE_AVAILABLE:
				((UIElement)mUpdateInfoGrid).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind(bodyLabel, "STRING_UPDATE_AVAILABLE");
				mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
				((ContentControl)mLabelVersion).Content = "v" + BlueStacksUpdater.sBstUpdateData.EngineVersion;
				((FrameworkElement)mCheckUpdateBtn).HorizontalAlignment = (HorizontalAlignment)(0x36EEFB6F ^ 0x36EEFB6D);
				((UIElement)mCheckUpdateBtn).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind((Button)(object)mCheckUpdateBtn, "STRING_DOWNLOAD_UPDATE");
				((UIElement)mStatusLabel).Visibility = (Visibility)(-1537632328 ^ -1537632326);
				((UIElement)mCheckingGrid).Visibility = (Visibility)(976932997 - 976932995 % 2007618386);
				break;
			case BlueStacksUpdater.UpdateState.DOWNLOADING:
			{
				((UIElement)mUpdateInfoGrid).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind(bodyLabel, "STRING_DOWNLOADING_UPDATE");
				mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
				((ContentControl)mLabelVersion).Content = "v" + BlueStacksUpdater.sBstUpdateData.EngineVersion;
				CustomButton obj = mCheckUpdateBtn;
				int num = ((627912792 > 2122169588) ? 2 : 2);
				((UIElement)obj).Visibility = (Visibility)num;
				((UIElement)mStatusLabel).Visibility = (Visibility)(0xB24 ^ 0xB26);
				((UIElement)mCheckingGrid).Visibility = (Visibility)(-648567757 - ~648567758);
				break;
			}
			case BlueStacksUpdater.UpdateState.DOWNLOADED:
				((UIElement)mUpdateInfoGrid).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind(bodyLabel, "STRING_UPDATES_READY_TO_INSTALL");
				mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
				((ContentControl)mLabelVersion).Content = "v" + BlueStacksUpdater.sBstUpdateData.EngineVersion;
				((FrameworkElement)mCheckUpdateBtn).HorizontalAlignment = (HorizontalAlignment)(259051944 + -259051942);
				((UIElement)mCheckUpdateBtn).Visibility = (Visibility)0;
				BlueStacksUIBinding.Bind((Button)(object)mCheckUpdateBtn, "STRING_INSTALL_UPDATE");
				((UIElement)mStatusLabel).Visibility = (Visibility)(-2077882834 ^ -2077882836);
				((UIElement)mCheckingGrid).Visibility = (Visibility)(0x5EE37F4D ^ 0x5EE37F4F);
				break;
			}
		}, new object[0]);
	}

	private void ShowCheckingForUpdateGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)(1553133491 + -1553133489);
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)(2 ^ 0);
			((UIElement)mStatusLabel).Visibility = (Visibility)(1041162161 - (0x3A0ED222 | 0xC0C0FAF));
			((UIElement)mCheckingGrid).Visibility = (Visibility)0;
		}, new object[0]);
	}

	private void ShowLatestVersionGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)(1708269517 + -1708269515);
			CustomButton obj = mCheckUpdateBtn;
			int num = ((980917683 > 1183404499) ? 2 : 2);
			((UIElement)obj).Visibility = (Visibility)num;
			((UIElement)mStatusLabel).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind(mStatusLabel, "STRING_LATEST_VERSION", "");
			((UIElement)mCheckingGrid).Visibility = (Visibility)(283891 - (1162812758 >> 1875345676));
		}, new object[0]);
	}

	private void ShowInternetConnectionErrorGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)(-1680340187 ^ -1680340185);
			((FrameworkElement)mCheckUpdateBtn).HorizontalAlignment = (HorizontalAlignment)(-20499849 + 1541325115 % 95051579);
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind((Button)(object)mCheckUpdateBtn, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
			((UIElement)mStatusLabel).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind(mStatusLabel, "STRING_POST_OTS_FAILED_WARNING_MESSAGE", "");
			((UIElement)mCheckingGrid).Visibility = (Visibility)(-1801809191 ^ -1801809189);
		}, new object[0]);
	}

	private void mCheckUpdateBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.Equals(((ContentControl)mCheckUpdateBtn).Content.ToString(), LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_UPDATE", ""), StringComparison.InvariantCulture))
		{
			BlueStacksUpdater.DownloadNow(BlueStacksUpdater.sBstUpdateData, hiddenMode: false);
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingDownloadUpdate);
		}
		else if (string.Equals(((ContentControl)mCheckUpdateBtn).Content.ToString(), LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATE", ""), (StringComparison)(514588251 + -514588249)))
		{
			ParentWindow.ShowInstallPopup();
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingInstallUpdate);
		}
		else if (string.Equals(((ContentControl)mCheckUpdateBtn).Content.ToString(), LocaleStrings.GetLocalizedString("STRING_CHECK_UPDATES", ""), StringComparison.InvariantCulture) || string.Equals(((ContentControl)mCheckUpdateBtn).Content.ToString(), LocaleStrings.GetLocalizedString("STRING_RETRY_CONNECTION_ISSUE_TEXT1", ""), StringComparison.InvariantCulture))
		{
			ShowCheckingForUpdateGrid();
			BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted -= HandleCheckForUpdateResult;
			BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted += HandleCheckForUpdateResult;
			BlueStacksUpdater.SetupBlueStacksUpdater(ParentWindow, isStartup: false);
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingCheckUpdate);
		}
	}

	private void HandleCheckForUpdateResult(object sender, RunWorkerCompletedEventArgs e)
	{
		if (BlueStacksUpdater.sBstUpdateData.IsUpdateAvailble)
		{
			HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingUpdateAvailable);
		}
		else if (BlueStacksUpdater.sBstUpdateData.IsTryAgain)
		{
			ShowInternetConnectionErrorGrid();
		}
		else
		{
			ShowLatestVersionGrid();
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingUpdateNotAvailable);
		}
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		((RoutedEventArgs)e).Handled = true;
	}

	private void mTermsOfUseLink_Loaded(object sender, RoutedEventArgs e)
	{
		((TextElementCollection<Inline>)(object)((Span)mTermsOfUseLink).Inlines).Clear();
		((Span)mTermsOfUseLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_TERMS_OF_USE_LINK", ""));
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/aboutsettingscontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mPoweredByBSGrid = (Grid)target;
			break;
		case 2:
			mBSIconAndNameGrid = (Grid)target;
			break;
		case 3:
			mProductTextGrid = (Grid)target;
			break;
		case 4:
			mVersionLabel = (Label)target;
			break;
		case 5:
			mUpdateInfoGrid = (Grid)target;
			break;
		case 6:
			bodyLabel = (Label)target;
			break;
		case 7:
			mLabelVersion = (Label)target;
			break;
		case 8:
			mDetailedChangeLogs = (Hyperlink)target;
			mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 9:
			mCheckUpdateBtn = (CustomButton)target;
			((ButtonBase)mCheckUpdateBtn).Click += new RoutedEventHandler(mCheckUpdateBtn_Click);
			break;
		case 10:
			mStatusLabel = (TextBlock)target;
			break;
		case 11:
			mCheckingGrid = (Grid)target;
			break;
		case 12:
			ContactInfoGrid = (Grid)target;
			break;
		case 13:
			mWebsiteLabel = (Label)target;
			break;
		case 14:
			((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 15:
			mSupportLabel = (Label)target;
			break;
		case 16:
			((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 17:
			mSupportMailLabel = (Label)target;
			break;
		case 18:
			mSupportEMailHyperlink = (Hyperlink)target;
			mSupportEMailHyperlink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 19:
			mTermsOfUse = (TextBlock)target;
			break;
		case 20:
			mTermsOfUseLink = (Hyperlink)target;
			mTermsOfUseLink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			((FrameworkContentElement)mTermsOfUseLink).Loaded += new RoutedEventHandler(mTermsOfUseLink_Loaded);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
