using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AboutSettingsControl : UserControl, IComponentConnector
{
	internal Grid mPoweredByBSGrid;

	internal Grid mProductTextGrid;

	internal Label mVersionLabel;

	internal Grid mUpdateInfoGrid;

	internal Label bodyLabel;

	internal Label mLabelVersion;

	internal Hyperlink mDetailedChangeLogs;

	internal CustomButton mCheckUpdateBtn;

	internal TextBlock mStatusLabel;

	internal Grid mCheckingGrid;

	internal Grid ContactInfoGrid;

	internal Label mWebsiteLabel;

	internal Label mSupportLabel;

	internal Label mSupportMailLabel;

	internal Hyperlink mSupportEMailHyperlink;

	internal TextBlock mTermsOfUse;

	internal Hyperlink mTermsOfUseLink;

	private bool _contentLoaded;

	internal Grid grid_0;

	internal Hyperlink mlink1;

	internal Hyperlink mlink2;

	public MainWindow ParentWindow { get; set; }

	public AboutSettingsControl(MainWindow window, SettingsWindow _)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		ParentWindow = window;
		InitializeComponent();
		((UIElement)this).Visibility = (Visibility)1;
		((FrameworkElement)this).Loaded += new RoutedEventHandler(AboutSettingsControl_Loaded);
		LocaleStrings.SourceUpdatedEvent += OnLocaleUpdated;
		ApplyLocalization();
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
			((UIElement)grid_0).Visibility = (Visibility)1;
		}
		((ContentControl)mVersionLabel).Content = "";
		((TextElementCollection<Inline>)(object)((Span)mSupportEMailHyperlink).Inlines).Clear();
		((TextElementCollection<Inline>)(object)((Span)mlink1).Inlines).Clear();
		((Span)mlink1).Inlines.Add("t.me/BluesterChannel");
		mlink1.NavigateUri = new Uri("https://t.me/BluesterChannel");
		((TextElementCollection<Inline>)(object)((Span)mlink2).Inlines).Clear();
		((Span)mlink2).Inlines.Add("t.me/BluesterSupportBot");
		mlink2.NavigateUri = new Uri("https://t.me/BluesterSupportBot");
		HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
		BlueStacksUpdater.StateChanged -= BlueStacksUpdater_StateChanged;
		BlueStacksUpdater.StateChanged += BlueStacksUpdater_StateChanged;
		VisualTreeChecker((DependencyObject)(object)this);
		ApplyLocalization();
	}

	private void BlueStacksUpdater_StateChanged()
	{
		HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
	}

	private void HandleUpdateStateGridVisibility(BlueStacksUpdater.UpdateState state)
	{
	}

	private void ShowCheckingForUpdateGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)2;
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)2;
			((UIElement)mStatusLabel).Visibility = (Visibility)2;
			((UIElement)mCheckingGrid).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private void ShowLatestVersionGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)2;
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)2;
			((UIElement)mStatusLabel).Visibility = (Visibility)2;
			BlueStacksUIBinding.Bind(mStatusLabel, "STRING_LATEST_VERSION", "");
			((UIElement)mCheckingGrid).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private void ShowInternetConnectionErrorGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)2;
			((FrameworkElement)mCheckUpdateBtn).HorizontalAlignment = (HorizontalAlignment)2;
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)2;
			BlueStacksUIBinding.Bind((Button)(object)mCheckUpdateBtn, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
			((UIElement)mStatusLabel).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind(mStatusLabel, "STRING_POST_OTS_FAILED_WARNING_MESSAGE", "");
			((UIElement)mCheckingGrid).Visibility = (Visibility)2;
		}, new object[0]);
	}

	private void HandleCheckForUpdateResult(object sender, RunWorkerCompletedEventArgs e)
	{
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		((RoutedEventArgs)e).Handled = true;
	}

	private void mTermsOfUseLink_Loaded(object sender, RoutedEventArgs e)
	{
		((TextElementCollection<Inline>)(object)((Span)mTermsOfUseLink).Inlines).Clear();
	}

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

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mPoweredByBSGrid = (Grid)target;
			((UIElement)mPoweredByBSGrid).Visibility = (Visibility)2;
			break;
		case 2:
		case 3:
			mProductTextGrid = (Grid)target;
			break;
		case 4:
			mVersionLabel = (Label)target;
			((UIElement)mVersionLabel).Visibility = (Visibility)2;
			break;
		case 5:
			mUpdateInfoGrid = (Grid)target;
			((UIElement)mUpdateInfoGrid).Visibility = (Visibility)2;
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
			((UIElement)mCheckUpdateBtn).Visibility = (Visibility)2;
			break;
		case 10:
			mStatusLabel = (TextBlock)target;
			((UIElement)mStatusLabel).Visibility = (Visibility)2;
			break;
		case 11:
			mCheckingGrid = (Grid)target;
			((UIElement)mCheckingGrid).Visibility = (Visibility)2;
			break;
		case 12:
			ContactInfoGrid = (Grid)target;
			((FrameworkElement)ContactInfoGrid).Margin = new Thickness(250.0, -140.0, 0.0, 0.0);
			break;
		case 13:
			mWebsiteLabel = (Label)target;
			break;
		case 14:
			((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			((TextElementCollection<Inline>)(object)((Span)(Hyperlink)target).Inlines).Clear();
			((Span)(Hyperlink)target).Inlines.Add("text 2");
			mlink1 = (Hyperlink)target;
			break;
		case 15:
			mSupportLabel = (Label)target;
			break;
		case 16:
			((Hyperlink)target).RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			((TextElementCollection<Inline>)(object)((Span)(Hyperlink)target).Inlines).Clear();
			((Span)(Hyperlink)target).Inlines.Add("text 1");
			mlink2 = (Hyperlink)target;
			break;
		case 17:
			mSupportMailLabel = (Label)target;
			((UIElement)mSupportMailLabel).Visibility = (Visibility)2;
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

	private void VisualTreeChecker(DependencyObject parent)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, i);
			Line val = (Line)(object)((child is Line) ? child : null);
			if (val != null)
			{
				Binding binding = BindingOperations.GetBinding((DependencyObject)(object)val, Shape.StrokeProperty);
				if (binding != null && binding.Path.Path.Contains("HorizontalSeparator"))
				{
					((UIElement)val).Visibility = (Visibility)1;
				}
			}
			TextBlock val2 = (TextBlock)(object)((child is TextBlock) ? child : null);
			if (val2 != null && val2.Text.Contains("Bluester"))
			{
				val2.FontFamily = new FontFamily(AppDomain.CurrentDomain.BaseDirectory + "Assets/#Intro Regular");
			}
			VisualTreeChecker(child);
		}
	}

	private void OnLocaleUpdated(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			ApplyLocalization();
		});
	}

	private void ApplyLocalization()
	{
		((ContentControl)mSupportLabel).Content = "TELEGRAM:";
		((ContentControl)mWebsiteLabel).Content = "TELEGRAM:";
		mTermsOfUse.Text = string.Empty;
		((UIElement)mTermsOfUse).Visibility = (Visibility)2;
	}
}
