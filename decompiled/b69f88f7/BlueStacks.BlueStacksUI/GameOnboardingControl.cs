using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class GameOnboardingControl : UserControl, IDimOverlayControl, IComponentConnector
{
	private DispatcherTimer dispatcherTimer;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBrowserGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSkipOnboardingButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBrowserGridTemp;

	private bool _contentLoaded;

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow { get; set; } = true;

	public bool ShowTransparentWindow { get; set; } = true;

	private BrowserControl mBrowser { get; set; }

	public string PackageName { get; set; }

	public MainWindow ParentWindow { get; set; }

	public Grid controlGrid { get; set; }

	public string InitiatedSource { get; set; }

	bool IDimOverlayControl.Close()
	{
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	public GameOnboardingControl(MainWindow mainWindow, string packageName, string source)
	{
		PackageName = packageName;
		ParentWindow = mainWindow;
		InitiatedSource = source;
		InitializeComponent();
	}

	private void Control_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_impression", ParentWindow.mVmName, PackageName, "", "");
		mBrowser = new BrowserControl();
		mBrowser.BrowserLoadCompleteEvent += BrowserLoadCompleteEvent;
		mBrowser.InitBaseControl(BlueStacksUIUtils.GetOnboardingUrl(PackageName, InitiatedSource));
		((UIElement)mBrowser).Visibility = (Visibility)0;
		mBrowser.ParentWindow = ParentWindow;
		((Panel)mBrowserGrid).Children.Add((UIElement)(object)mBrowser);
		controlGrid = AddBrowser();
		((UIElement)controlGrid).Visibility = (Visibility)0;
		((Panel)mBrowserGrid).Children.Add((UIElement)(object)controlGrid);
		dispatcherTimer = new DispatcherTimer();
		dispatcherTimer.Tick += DispatcherTimer_Tick;
		dispatcherTimer.Interval = new TimeSpan(0, 0, PostBootCloudInfoManager.Instance.mPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer);
		dispatcherTimer.Start();
	}

	internal Grid AddBrowser()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		Grid val = new Grid();
		CustomPictureBox val2 = new CustomPictureBox
		{
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1,
			Height = 30.0,
			Width = 30.0,
			ImageName = "loader",
			IsImageToBeRotated = true
		};
		((Panel)val).Children.Add((UIElement)(object)val2);
		((UIElement)val).Visibility = (Visibility)1;
		return val;
	}

	private void BrowserLoadCompleteEvent()
	{
		AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsAppOnboardingCompleted = true;
		((Panel)mBrowserGrid).Children.Remove((UIElement)(object)controlGrid);
	}

	private void DispatcherTimer_Tick(object _1, EventArgs _2)
	{
		((UIElement)mSkipOnboardingButton).Visibility = (Visibility)0;
		dispatcherTimer.Stop();
	}

	private void SkipOnboardingButton_Click(object sender, RoutedEventArgs e)
	{
		AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsAppOnboardingCompleted = true;
		Stats.SendCommonClientStatsAsync("onboarding-tutorial", "onboarding_skipped", ParentWindow.mVmName, PackageName, "", "");
		Close();
		KMManager.sGuidanceWindow?.DimOverLayVisibility((Visibility)2);
		if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>)((AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted)))
		{
			ParentWindow.StaticComponents.mSelectedTabButton.ShowDefaultBlurbOnboarding();
		}
		ParentWindow.HideDimOverlay();
	}

	private void Control_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		if ((int)e.Key == 156 && (int)e.SystemKey == 93)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	internal bool Close()
	{
		try
		{
			if (mBrowser != null)
			{
				mBrowser.DisposeBrowser();
				((Panel)mBrowserGrid).Children.Remove((UIElement)(object)mBrowser);
				mBrowser = null;
			}
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			((UIElement)this).Visibility = (Visibility)1;
			Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_closed", ParentWindow.mVmName, PackageName, "", "");
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close gameonboardingcontrol from dimoverlay " + ex.ToString());
		}
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/gameonboardingcontrol.xaml", UriKind.Relative);
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(GameOnboardingControl)target).Loaded += new RoutedEventHandler(Control_Loaded);
			((UIElement)(GameOnboardingControl)target).KeyDown += new KeyEventHandler(Control_KeyDown);
			break;
		case 2:
			mBrowserGrid = (Grid)target;
			break;
		case 3:
			mSkipOnboardingButton = (CustomButton)target;
			((ButtonBase)mSkipOnboardingButton).Click += new RoutedEventHandler(SkipOnboardingButton_Click);
			break;
		case 4:
			mBrowserGridTemp = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
