using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class WelcomeTab : UserControl, IComponentConnector
{
	internal HomeAppManager mHomeAppManager;

	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mContentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBackground;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal FrontendPopupControl mFrontendPopupControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPromotionGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal PromotionControl mPromotionControl;

	private bool _contentLoaded;

	private BrowserControl mBrowser { get; set; }

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	internal bool IsPromotionVisible
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			return (int)((UIElement)mPromotionGrid).Visibility == 0;
		}
	}

	public WelcomeTab()
	{
		InitializeComponent();
	}

	internal void Init()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Invalid comparison between Unknown and I4
		HomeApp homeApp = null;
		if (FeatureManager.Instance.IsHtmlHome)
		{
			mBrowser = AddBrowser(ParentWindow.Utils.GetHtmlHomeUrl(), isFallbackUrlRequired: true);
		}
		else
		{
			homeApp = new HomeApp(ParentWindow);
			if (!((Panel)mContentGrid).Children.Contains((UIElement)(object)homeApp))
			{
				((Panel)mContentGrid).Children.Add((UIElement)(object)homeApp);
			}
		}
		mHomeAppManager = new HomeAppManager(homeApp, ParentWindow);
		if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			mHomeAppManager.ChangeHomeAppVisibility((Visibility)1);
			mBackground.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promo\\boot_promo_0.png");
			((UIElement)mBackground).Visibility = (Visibility)0;
		}
		if (FeatureManager.Instance.IsPromotionDisabled || Opt.Instance.hiddenBootMode)
		{
			RemovePromotionGrid();
			mHomeAppManager.ChangeHomeAppLoadingGridVisibility((Visibility)0);
		}
	}

	private void BrowserFallbackUrlEvent()
	{
		if (!string.IsNullOrEmpty(RegistryManager.Instance.OfflineHtmlHomeUrl))
		{
			mBrowser.UpdateUrlAndRefresh(RegistryManager.Instance.OfflineHtmlHomeUrl);
			Stats.SendCommonClientStatsAsync("html_home", "OfflineHtmlHome_loaded", ParentWindow.mVmName, "", "", "");
		}
	}

	private BrowserControl AddBrowser(string url, bool isFallbackUrlRequired = false)
	{
		BrowserControl browserControl = new BrowserControl();
		if (isFallbackUrlRequired)
		{
			browserControl.BrowserFallbackUrlEvent += BrowserFallbackUrlEvent;
		}
		browserControl.InitBaseControl(url);
		((UIElement)browserControl).Visibility = (Visibility)0;
		browserControl.ParentWindow = ParentWindow;
		((Panel)mContentGrid).Children.Add((UIElement)(object)browserControl);
		return browserControl;
	}

	internal void ReInitHtmlHome()
	{
		mBrowser.UpdateUrlAndRefresh(ParentWindow.Utils.GetHtmlHomeUrl());
	}

	internal void DisposeHtmHomeBrowser()
	{
		if (mBrowser != null)
		{
			mBrowser.DisposeBrowser();
			((Panel)mContentGrid).Children.Remove((UIElement)(object)mBrowser);
			mBrowser = null;
		}
	}

	internal void RemovePromotionGrid()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mPromotionGrid).Visibility = (Visibility)1;
			mPromotionControl.Stop();
			mHomeAppManager?.EnableSearchTextBox(isEnable: true);
		}, new object[0]);
	}

	internal void OpenFrontendAppTabControl(string packageName, PlayStoreAction action)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if ((int)action == 0 && ParentWindow.mAppHandler.IsAppInstalled(packageName) && !"com.android.vending".Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
			{
				AppIconModel appIcon = mHomeAppManager.GetAppIcon(packageName);
				if (appIcon != null)
				{
					if ((int)appIcon.AppIncompatType != 0)
					{
						GrmHandler.HandleCompatibility(appIcon.PackageName, ParentWindow.mVmName);
					}
					else
					{
						ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: true, isLaunch: true);
					}
				}
			}
			else if (!string.IsNullOrEmpty(packageName))
			{
				AppIconModel appIcon2 = mHomeAppManager.GetAppIcon("com.android.vending");
				if (appIcon2 != null)
				{
					if ((int)action == 0)
					{
						ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon2.AppName, appIcon2.PackageName, appIcon2.ActivityName, appIcon2.ImageName, isSwitch: false, isLaunch: true);
						ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
						ParentWindow.mAppHandler.LaunchPlayRequestAsync(packageName);
					}
					else if ((int)action == 1)
					{
						ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon2.AppName, appIcon2.PackageName, appIcon2.ActivityName, appIcon2.ImageName, isSwitch: false, isLaunch: true);
						ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
						ParentWindow.mAppHandler.SendSearchPlayRequestAsync(packageName);
					}
				}
			}
		}, new object[0]);
	}

	internal void ReloadHomeTabIME()
	{
		BrowserControl browserControl = mBrowser;
		if (browserControl != null)
		{
			Browser cefBrowser = browserControl.CefBrowser;
			if (cefBrowser != null)
			{
				((UIElement)cefBrowser).Focus();
			}
		}
		BrowserControl browserControl2 = mBrowser;
		if (browserControl2 != null)
		{
			Browser cefBrowser2 = browserControl2.CefBrowser;
			if (cefBrowser2 != null)
			{
				((WpfCefBrowser)cefBrowser2).ReloadIME();
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/welcometab.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mContentGrid = (Grid)target;
			break;
		case 2:
			mBackground = (CustomPictureBox)target;
			break;
		case 3:
			mFrontendPopupControl = (FrontendPopupControl)target;
			break;
		case 4:
			mPromotionGrid = (Grid)target;
			break;
		case 5:
			mPromotionControl = (PromotionControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
