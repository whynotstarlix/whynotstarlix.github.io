using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class Browser : WpfCefBrowser
{
	private bool isLoaded;

	private string url;

	private double zoomLevel;

	private float mCustomZoomLevel;

	private MainWindow mMainWindow;

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

	public Browser(float zoomLevel = 0f)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		if (!CefHelper.CefInited)
		{
			string mBSTProcessIdentifier = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
			string[] args = new string[0];
			Logger.Info("Init cef");
			CefHelper.InitCef(args, mBSTProcessIdentifier);
		}
		((FrameworkElement)this).Loaded += new RoutedEventHandler(Browser_Loaded);
		((WpfCefBrowser)this).LoadingStateChange += new LoadingStateChangeEventHandler(Browser_LoadingStateChange);
		mCustomZoomLevel = zoomLevel;
		if (RegistryManager.Instance.CefDevEnv == 1)
		{
			((WpfCefBrowser)this).mAllowDevTool = true;
			((WpfCefBrowser)this).mDevToolHeader = ((WpfCefBrowser)this).StartUrl;
		}
	}

	public Browser(string url)
	{
		this.url = url;
	}

	private void Browser_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		try
		{
			isLoaded = true;
			Matrix transformToDevice = PresentationSource.FromVisual((Visual)sender).CompositionTarget.TransformToDevice;
			ScaleTransform val = new ScaleTransform(1.0 / ((Matrix)(ref transformToDevice)).M11, 1.0 / ((Matrix)(ref transformToDevice)).M22);
			if (((Freezable)val).CanFreeze)
			{
				((Freezable)val).Freeze();
			}
			((FrameworkElement)this).LayoutTransform = (Transform)(object)val;
			zoomLevel = Math.Log(((Matrix)(ref transformToDevice)).M11) / Math.Log(1.2);
			if (mCustomZoomLevel != 0f)
			{
				double num = Math.Log10(mCustomZoomLevel) / Math.Log10(1.2);
				zoomLevel += num;
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Failed to get zoom factor of browser with url {0} and error :{1}", new object[2]
			{
				url,
				ex.ToString()
			});
		}
	}

	private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				string uRL = ((WpfCefBrowser)this).getURL();
				if (!string.IsNullOrEmpty(uRL))
				{
					string packageName = uRL.Substring(uRL.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
					if (uRL.Contains("play.google.com"))
					{
						ParentWindow.mAppHandler.LaunchPlayRequestAsync(packageName);
					}
				}
			}
			catch (Exception)
			{
			}
		}, new object[0]);
		if (!e.IsLoading)
		{
			try
			{
				((WpfCefBrowser)this).SetZoomLevel(zoomLevel);
			}
			catch (Exception ex)
			{
				Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", new object[2]
				{
					url,
					ex.ToString()
				});
			}
		}
	}

	public void CallJs(string methodName, object[] args)
	{
		if (!isLoaded)
		{
			return;
		}
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				if (args.Length == 1)
				{
					string text = args[0].ToString();
					text = text.Replace("%27", "&#39;").Replace("'", "&#39;");
					string text2 = string.Format(CultureInfo.InvariantCulture, "javascript:{0}('{1}')", new object[2] { methodName, text });
					Logger.Info("calling " + methodName);
					((WpfCefBrowser)this).ExecuteJavaScript(text2, ((WpfCefBrowser)this).getURL(), 0);
				}
				else if (args.Length == 0)
				{
					string text3 = string.Format(CultureInfo.InvariantCulture, "javascript:{0}()", new object[1] { methodName });
					Logger.Info("calling " + methodName);
					((WpfCefBrowser)this).ExecuteJavaScript(text3, ((WpfCefBrowser)this).getURL(), 0);
				}
				else
				{
					Logger.Error("Error: function supported for one length array object to be changed later");
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}
}
