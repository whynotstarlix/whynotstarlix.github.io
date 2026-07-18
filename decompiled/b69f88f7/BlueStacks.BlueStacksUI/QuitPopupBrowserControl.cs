using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class QuitPopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBrowserGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mCloseButton;

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

	public bool ShowTransparentWindow { get; set; }

	private MainWindow ParentWindow { get; set; }

	private BrowserControl mBrowser { get; set; }

	internal string PackageName { get; set; } = "";

	internal string QuitPopupUrl { get; set; } = string.Empty;

	internal bool IsForceReload { get; set; }

	internal bool ShowOnQuit { get; set; }

	bool IDimOverlayControl.Close()
	{
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	public QuitPopupBrowserControl(MainWindow mainWindow)
	{
		ParentWindow = mainWindow;
		InitializeComponent();
	}

	internal void SetQuitPopParams(string url, string package, bool isForceReload, bool showOnQuit)
	{
		QuitPopupUrl = url;
		IsForceReload = isForceReload;
		PackageName = package;
		ShowOnQuit = showOnQuit;
	}

	internal void Init(string appPackage)
	{
		((FrameworkElement)this).Width = 740.0;
		((FrameworkElement)this).Height = 490.0;
		PackageName = appPackage;
		((ContentControl)mCloseButton).Content = (string.IsNullOrEmpty(appPackage) ? LocaleStrings.GetLocalizedString("STRING_CLOSE_BLUESTACKS", "") : LocaleStrings.GetLocalizedString("STRING_CLOSE_GAME", ""));
		ClientStats.SendMiscellaneousStatsAsync("quitpopupdisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, QuitPopupUrl, PackageName, null);
		if (mBrowser == null)
		{
			LoadBrowser();
		}
		((Panel)ParentWindow.mQuitPopupBrowserLoadGrid).Children.Remove((UIElement)(object)mBrowser);
		((Panel)mBrowserGrid).Children.Add((UIElement)(object)mBrowser);
		((UIElement)mBrowser).Visibility = (Visibility)0;
		ParentWindow.ShowDimOverlay(this);
	}

	internal void LoadBrowser()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			DisposeBrowser();
			mBrowser = new BrowserControl();
			mBrowser.InitBaseControl(QuitPopupUrl);
			mBrowser.ParentWindow = ParentWindow;
			((Panel)ParentWindow.mQuitPopupBrowserLoadGrid).Children.Add((UIElement)(object)mBrowser);
			((UIElement)ParentWindow.mQuitPopupBrowserLoadGrid).Visibility = (Visibility)1;
			mBrowser.CreateNewBrowser();
		}, new object[0]);
	}

	internal void RefreshBrowserUrl(string url)
	{
		try
		{
			QuitPopupUrl = url;
			mBrowser.UpdateUrlAndRefresh(url);
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in refreshing quitpopup borwser url: " + ex.ToString());
		}
	}

	private void DisposeBrowser()
	{
		if (mBrowser != null)
		{
			mBrowser.DisposeBrowser();
			((Panel)mBrowserGrid).Children.Remove((UIElement)(object)mBrowser);
			((Panel)ParentWindow.mQuitPopupBrowserLoadGrid).Children.Remove((UIElement)(object)mBrowser);
			mBrowser = null;
		}
	}

	private void CloseButton_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	internal void Close()
	{
		try
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				DisposeBrowser();
				ParentWindow.mTopBar.mAppTabButtons.mLastPackageForQuitPopupDisplayed = "";
				((UIElement)this).Visibility = (Visibility)1;
				ClientStats.SendMiscellaneousStatsAsync("quitpopupclosed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, QuitPopupUrl, PackageName, null);
				ParentWindow.HideDimOverlay();
				if (string.IsNullOrEmpty(PackageName))
				{
					ParentWindow.ForceCloseWindow();
				}
				else
				{
					ParentWindow.mTopBar.mAppTabButtons.CloseTab(PackageName, sendStopAppToAndroid: true);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception when trying to close quit popup. " + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitpopupbrowsercontrol.xaml", UriKind.Relative);
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mGrid = (Grid)target;
			break;
		case 2:
			mBrowserGrid = (Grid)target;
			break;
		case 3:
			mCloseButton = (CustomButton)target;
			((ButtonBase)mCloseButton).Click += new RoutedEventHandler(CloseButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
