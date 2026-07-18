using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class UpdatePrompt : UserControl, IComponentConnector
{
	private BlueStacksUpdateData mBstUpdateData;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label titleLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label bodyLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mLabelVersion;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mDetailedChangeLogs;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mDownloadNewButton;

	private bool _contentLoaded;

	internal UpdatePrompt(BlueStacksUpdateData bstUpdateData)
	{
		InitializeComponent();
		mBstUpdateData = bstUpdateData;
		if (string.IsNullOrEmpty(bstUpdateData.EngineVersion))
		{
			((ContentControl)mLabelVersion).Content = "";
			((UIElement)mLabelVersion).Visibility = (Visibility)2;
		}
		else
		{
			((ContentControl)mLabelVersion).Content = "v" + bstUpdateData.EngineVersion;
		}
		BlueStacksUIBinding.Bind(titleLabel, "STRING_BLUESTACKS_UPDATE_AVAILABLE");
		BlueStacksUIBinding.Bind(bodyLabel, "STRING_UPDATE_AVAILABLE");
		BlueStacksUIBinding.Bind((Button)(object)mDownloadNewButton, "STRING_DOWNLOAD_UPDATE");
		((UIElement)mCloseBtn).Visibility = (Visibility)0;
		mDetailedChangeLogs.NavigateUri = new Uri(bstUpdateData.DetailedChangeLogsUrl);
		((TextElementCollection<Inline>)(object)((Span)mDetailedChangeLogs).Inlines).Clear();
		((Span)mDetailedChangeLogs).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked UpdateNow Menu Close button");
		RegistryManager.Instance.LastUpdateSkippedVersion = mBstUpdateData.EngineVersion;
		ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupCross);
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	private void DownloadNowButton_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("Clicked Download_Now button");
		ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupDwnld);
		BlueStacksUpdater.DownloadNow(mBstUpdateData, hiddenMode: false);
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		((RoutedEventArgs)e).Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/updateprompt.xaml", UriKind.Relative);
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			titleLabel = (Label)target;
			break;
		case 2:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_MouseLeftButtonUp);
			break;
		case 3:
			bodyLabel = (Label)target;
			break;
		case 4:
			mLabelVersion = (Label)target;
			break;
		case 5:
			mDetailedChangeLogs = (Hyperlink)target;
			mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 6:
			mDownloadNewButton = (CustomButton)target;
			((ButtonBase)mDownloadNewButton).Click += new RoutedEventHandler(DownloadNowButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
