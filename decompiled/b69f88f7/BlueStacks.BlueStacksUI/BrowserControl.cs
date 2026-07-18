using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.Common;
using BlueStacks.Common.Grm;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows7.Multitouch;
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch.WPF;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class BrowserControl : UserControl, IDisposable
{
	private string SOURCE_APPCENTER = "BSAppCenter";

	private float customZoomLevel;

	private MainWindow mMainWindow;

	private NoInternetControl mNoInternetControl;

	private Browser mBrowser;

	internal string mUrl;

	private double zoomLevel = 1.0;

	[CompilerGenerated]
	private ProcessMessageEventHandler m_ProcessMessageRecieved;

	internal static List<BrowserControl> sAllBrowserControls = new List<BrowserControl>();

	internal static List<string> mFirebaseTagsSubscribed = new List<string>();

	internal string mFirebaseCallbackMethod = string.Empty;

	internal string mFailedUrl = string.Empty;

	private CefBrowserHost mBrowserHost;

	private bool disposedValue;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
				}, new object[0]);
			}
			return mMainWindow;
		}
		set
		{
			mMainWindow = value;
		}
	}

	public NoInternetControl NoInternetControl
	{
		get
		{
			if (mNoInternetControl == null)
			{
				mNoInternetControl = new NoInternetControl(this);
			}
			return mNoInternetControl;
		}
		set
		{
			mNoInternetControl = value;
		}
	}

	public Grid mGrid { get; set; }

	public Dictionary<BrowserControlTags, JObject> TagsSubscribedDict { get; } = new Dictionary<BrowserControlTags, JObject>();

	public BrowserSubscriber mSubscriber { get; set; }

	internal Browser CefBrowser
	{
		get
		{
			return mBrowser;
		}
		set
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			mBrowser = value;
			if (mBrowser != null)
			{
				return;
			}
			foreach (BrowserControlTags key in TagsSubscribedDict.Keys)
			{
				mSubscriber?.UnsubscribeTag(key);
			}
		}
	}

	public event ProcessMessageEventHandler ProcessMessageRecieved
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			ProcessMessageEventHandler val = this.m_ProcessMessageRecieved;
			ProcessMessageEventHandler val2;
			do
			{
				val2 = val;
				ProcessMessageEventHandler value2 = (ProcessMessageEventHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ProcessMessageRecieved, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			ProcessMessageEventHandler val = this.m_ProcessMessageRecieved;
			ProcessMessageEventHandler val2;
			do
			{
				val2 = val;
				ProcessMessageEventHandler value2 = (ProcessMessageEventHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ProcessMessageRecieved, value2, val2);
			}
			while (val != val2);
		}
	}

	public event Action BrowserLoadCompleteEvent;

	public event Action BrowserFallbackUrlEvent;

	public BrowserControl()
	{
	}

	public void UpdateUrlAndRefresh(string newUrl)
	{
		mUrl = newUrl;
		if (CefBrowser != null)
		{
			((WpfCefBrowser)CefBrowser).StartUrl = mUrl;
			SetVisibilityOfLoader((Visibility)0);
			((WpfCefBrowser)CefBrowser).NavigateTo(mUrl);
		}
	}

	internal void NavigateTo(string url)
	{
		if (CefBrowser != null)
		{
			SetVisibilityOfLoader((Visibility)0);
			((WpfCefBrowser)CefBrowser).NavigateTo(url);
		}
	}

	public void RefreshBrowser()
	{
		if (CefBrowser != null)
		{
			((WpfCefBrowser)CefBrowser).Refresh();
		}
	}

	public BrowserControl(string url)
	{
		InitBaseControl(url);
		mSubscriber = new BrowserSubscriber(this);
	}

	public void CallJsForMaps(string methodName, string appName, string packageName)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		object[] array = new object[1] { "" };
		if (!string.IsNullOrEmpty(appName) || !string.IsNullOrEmpty(packageName))
		{
			JObject val = new JObject();
			val.Add("name", JToken.op_Implicit(appName));
			val.Add("pkg", JToken.op_Implicit(packageName));
			JObject val2 = val;
			array[0] = ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
		}
		if (CefBrowser != null)
		{
			CefBrowser.CallJs(methodName, array);
		}
	}

	internal void InitBaseControl(string url, float zoomLevel = 0f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		customZoomLevel = zoomLevel;
		mUrl = url;
		((UIElement)this).Visibility = (Visibility)1;
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(BrowserControl_IsVisibleChanged);
		mGrid = new Grid();
		((ContentControl)this).Content = mGrid;
		if (FeatureManager.Instance.IsCreateBrowserOnStart)
		{
			CreateNewBrowser();
		}
	}

	public void DisposeBrowser()
	{
		sAllBrowserControls.Remove(this);
		if (CefBrowser != null)
		{
			((Panel)mGrid).Children.Remove((UIElement)(object)CefBrowser);
			((WpfCefBrowser)CefBrowser).Dispose();
			mBrowserHost = null;
			CefBrowser = null;
		}
	}

	private void BrowserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		if (((UIElement)this).IsVisible)
		{
			Logger.Info("Install Boot: BrowserControl_IsVisibleChanged");
			CreateNewBrowser();
		}
	}

	internal void WelcomeTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs _)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (FeatureManager.Instance.IsBrowserKilledOnTabSwitch && (int)((UIElement)sender).Visibility != 0)
		{
			DisposeBrowser();
		}
	}

	internal void CreateNewBrowser()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		if (CefBrowser == null && !string.IsNullOrEmpty(mUrl))
		{
			Logger.Info("Install Boot: CreateNewBrowser");
			CefBrowser = new Browser(customZoomLevel);
			sAllBrowserControls.Add(this);
			((WpfCefBrowser)CefBrowser).StartUrl = mUrl;
			((Panel)mGrid).Children.Add((UIElement)(object)CefBrowser);
			((WpfCefBrowser)CefBrowser).LoadEnd += new LoadEndEventHandler(MBrowser_LoadEnd);
			((WpfCefBrowser)CefBrowser).ProcessMessageRecieved += new ProcessMessageEventHandler(Browser_ProcessMessageRecieved);
			((FrameworkElement)CefBrowser).Loaded += new RoutedEventHandler(Browser_Loaded);
			((WpfCefBrowser)CefBrowser).LoadError += new LoadErrorEventHandler(Browser_LoadError);
			((WpfCefBrowser)CefBrowser).LoadingStateChange += new LoadingStateChangeEventHandler(Browser_LoadingStateChange);
			((WpfCefBrowser)CefBrowser).OnBeforePopup += CefBrowser_OnBeforePopup;
			((WpfCefBrowser)CefBrowser).mWPFCefBrowserExceptionHandler += new ExceptionHandler(Browser_WPFCefBrowserExceptionHandler);
			if (RegistryManager.Instance.CefDevEnv == 1)
			{
				((WpfCefBrowser)CefBrowser).mAllowDevTool = true;
				((WpfCefBrowser)CefBrowser).mDevToolHeader = mUrl;
			}
			Logger.Info("Install Boot: CreateNewBrowser complete");
			try
			{
				AddTouchHandler();
			}
			catch (Exception ex)
			{
				Logger.Info("Install Boot: CreateNewBrowser error");
				Logger.Error("exception adding touch handler: {0}", new object[1] { ex });
			}
		}
	}

	private bool CefBrowser_OnBeforePopup(string url)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected I4, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				BlueStacksUIUtils.OpenUrl(url);
			}
			else
			{
				OpenExternalBrowserLinks val = (OpenExternalBrowserLinks)0;
				if (Enum.IsDefined(typeof(OpenExternalBrowserLinks), RegistryManager.Instance.OpenExternalLink))
				{
					val = (OpenExternalBrowserLinks)Enum.Parse(typeof(OpenExternalBrowserLinks), RegistryManager.Instance.OpenExternalLink);
				}
				switch ((int)val)
				{
				case 0:
					BlueStacksUIUtils.OpenUrl(url);
					break;
				case 1:
					UpdateUrlAndRefresh(url);
					break;
				case 2:
					ParentWindow?.Utils.AppendUrlWithCommonParamsAndOpenTab(url, "Browser", "cef_tab");
					break;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logger.Warning("Error in opening external links from the cef browser: " + ex.ToString());
			return false;
		}
	}

	private void SendMessageToBrowserRenderProcess(CefProcessMessage message)
	{
		try
		{
			((WpfCefBrowser)CefBrowser).GetHost().GetBrowser().SendProcessMessage((CefProcessId)1, message);
		}
		catch (Exception ex)
		{
			Logger.Error("exception in sending IPC message to cef render process.." + ex);
		}
	}

	private void MBrowser_LoadEnd(object sender, LoadEndEventArgs e)
	{
		try
		{
			SetVisibilityOfLoader((Visibility)1);
			if (ParentWindow != null)
			{
				CefProcessMessage val = CefProcessMessage.Create("SetVmName");
				try
				{
					val.Arguments.SetString(0, ParentWindow.mVmName);
					SendMessageToBrowserRenderProcess(val);
					return;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in browser_loadend " + ex);
		}
	}

	private void Browser_LoadError(object sender, LoadErrorEventArgs e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		if (CefBrowser == null)
		{
			return;
		}
		Logger.Warning("Cef error code: {0}, error text: {1}", new object[2] { e.ErrorCode, e.ErrorText });
		if ((int)e.ErrorCode != -106 && (int)e.ErrorCode != -111 && (int)e.ErrorCode != -101 && (int)e.ErrorCode != -21 && (int)e.ErrorCode != -130 && ((int)e.ErrorCode != -105 || Utils.CheckForInternetConnection()))
		{
			return;
		}
		mFailedUrl = e.FailedUrl;
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (this.BrowserFallbackUrlEvent != null)
			{
				this.BrowserFallbackUrlEvent();
			}
			else if (!((Panel)mGrid).Children.Contains((UIElement)(object)NoInternetControl))
			{
				((Panel)mGrid).Children.Add((UIElement)(object)NoInternetControl);
			}
		}, new object[0]);
	}

	private void SetVisibilityOfLoader(Visibility visibility)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				DependencyObject parent = ((FrameworkElement)this).Parent;
				Grid val = (Grid)(object)((parent is Grid) ? parent : null);
				if (val != null)
				{
					IEnumerable<CustomPictureBox> enumerable = ((IEnumerable)((Panel)val).Children).OfType<CustomPictureBox>();
					if (enumerable != null && enumerable.Any())
					{
						((UIElement)enumerable.First()).Visibility = visibility;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set visibility of web page loader : " + ex.ToString());
			}
		}, new object[0]);
	}

	private void Browser_WPFCefBrowserExceptionHandler(object sender, Exception e)
	{
		Logger.Error("Handle Error in wpf cef browser.." + e.ToString());
	}

	private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
	{
		try
		{
			if (customZoomLevel == 0f)
			{
				((WpfCefBrowser)CefBrowser).SetZoomLevel(zoomLevel);
			}
			if (!e.IsLoading)
			{
				return;
			}
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (((Panel)mGrid).Children.Contains((UIElement)(object)NoInternetControl))
				{
					((Panel)mGrid).Children.Remove((UIElement)(object)NoInternetControl);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", new object[2]
			{
				mUrl,
				ex.ToString()
			});
		}
	}

	private void Browser_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		try
		{
			if (customZoomLevel == 0f)
			{
				Matrix transformToDevice = PresentationSource.FromVisual((Visual)sender).CompositionTarget.TransformToDevice;
				ScaleTransform val = new ScaleTransform(1.0 / ((Matrix)(ref transformToDevice)).M11, 1.0 / ((Matrix)(ref transformToDevice)).M22);
				if (((Freezable)val).CanFreeze)
				{
					((Freezable)val).Freeze();
				}
				((FrameworkElement)CefBrowser).LayoutTransform = (Transform)(object)val;
				zoomLevel = Math.Log(((Matrix)(ref transformToDevice)).M11) / Math.Log(1.2);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while getting zoom of browser with url {0} and error :{1}", new object[2]
			{
				mUrl,
				ex.ToString()
			});
		}
	}

	private void AddTouchHandler()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		try
		{
			if (DigitizerCapabilities.IsMultiTouchReady)
			{
				Logger.Info("adding touch handler");
				ManipulationProcessor mManipulationProcessor = new ManipulationProcessor((ProcessorManipulations)2);
				mBrowserHost = ((WpfCefBrowser)CefBrowser).GetHost();
				Factory.EnableStylusEvents((Window)(object)ParentWindow);
				((UIElement)this).StylusDown += (StylusDownEventHandler)delegate(object s, StylusDownEventArgs e)
				{
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					mManipulationProcessor.ProcessDown((uint)((StylusEventArgs)e).StylusDevice.Id, PointUtil.ToDrawingPointF(((StylusEventArgs)e).GetPosition((IInputElement)(object)this)));
				};
				((UIElement)this).StylusUp += (StylusEventHandler)delegate(object s, StylusEventArgs e)
				{
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					mManipulationProcessor.ProcessUp((uint)e.StylusDevice.Id, PointUtil.ToDrawingPointF(e.GetPosition((IInputElement)(object)this)));
				};
				((UIElement)this).StylusMove += (StylusEventHandler)delegate(object s, StylusEventArgs e)
				{
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					mManipulationProcessor.ProcessMove((uint)e.StylusDevice.Id, PointUtil.ToDrawingPointF(e.GetPosition((IInputElement)(object)this)));
				};
				mManipulationProcessor.ManipulationDelta += ProcessManipulationDelta;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("exception in adding touch handler: {0}", new object[1] { ex });
		}
	}

	private void ProcessManipulationDelta(object sender, ManipulationDeltaEventArgs e)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Logger.Debug("ProcessManipulationDelta.." + e.TranslationDelta.Height + "..." + ((ManipulationStartedEventArgs)e).Location);
		if (mBrowserHost == null)
		{
			mBrowserHost = ((WpfCefBrowser)CefBrowser).GetHost();
		}
		CefMouseEvent val = default(CefMouseEvent);
		((CefMouseEvent)(ref val)).X = (int)((ManipulationStartedEventArgs)e).Location.X;
		((CefMouseEvent)(ref val)).Y = (int)((ManipulationStartedEventArgs)e).Location.Y;
		CefMouseEvent val2 = val;
		mBrowserHost.SendMouseWheelEvent(val2, 0, (int)e.TranslationDelta.Height);
		mBrowserHost.SendMouseMoveEvent(new CefMouseEvent(0, 0, (CefEventFlags)0), false);
	}

	private void Browser_ProcessMessageRecieved(object sender, ProcessMessageEventArgs e)
	{
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Invalid comparison between Unknown and I4
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Expected O, but got Unknown
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Expected O, but got Unknown
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Expected O, but got Unknown
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Invalid comparison between Unknown and I4
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Expected O, but got Unknown
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Expected O, but got Unknown
		Logger.Info("Browser to client web call :" + e.Message.Name);
		if (string.Equals(e.Message.Name, "InstallApp", StringComparison.InvariantCulture))
		{
			CefListValue arguments = e.Message.Arguments;
			string iconUrl = arguments.GetString(0);
			string appName = arguments.GetString(1);
			string apkUrl = arguments.GetString(2);
			string text = arguments.GetString(3);
			InstallApp(iconUrl, appName, apkUrl, text);
			ParentWindow.Utils.SendMessageToAndroidForAffiliate(text, SOURCE_APPCENTER);
		}
		else if (string.Equals(e.Message.Name, "InstallAppVersion", StringComparison.InvariantCulture))
		{
			CefListValue arguments2 = e.Message.Arguments;
			string iconUrl2 = arguments2.GetString(0);
			string appName2 = arguments2.GetString(1);
			string apkUrl2 = arguments2.GetString(2);
			string text2 = arguments2.GetString(3);
			string timestamp = arguments2.GetString(4);
			InstallApp(iconUrl2, appName2, apkUrl2, text2, timestamp);
			ParentWindow.Utils.SendMessageToAndroidForAffiliate(text2, SOURCE_APPCENTER);
		}
		else if (string.Equals(e.Message.Name, "InstallAppGooglePlay", StringComparison.InvariantCulture))
		{
			CefListValue arguments3 = e.Message.Arguments;
			arguments3.GetString(0);
			arguments3.GetString(1);
			arguments3.GetString(2);
			string text3 = arguments3.GetString(3);
			ShowAppInPlayStore(text3);
			ParentWindow.Utils.SendMessageToAndroidForAffiliate(text3, SOURCE_APPCENTER);
		}
		else if (string.Equals(e.Message.Name, "InstallAppGooglePlayPopup", StringComparison.InvariantCulture))
		{
			CefListValue arguments4 = e.Message.Arguments;
			arguments4.GetString(0);
			string appName3 = arguments4.GetString(1);
			arguments4.GetString(2);
			string text4 = arguments4.GetString(3);
			ShowAppInPlayStorePopup(text4, appName3);
			ParentWindow.Utils.SendMessageToAndroidForAffiliate(text4, SOURCE_APPCENTER);
		}
		else if (string.Equals(e.Message.Name, "DownloadInstallOem", StringComparison.InvariantCulture))
		{
			CefListValue arguments5 = e.Message.Arguments;
			string oem = arguments5.GetString(0);
			string abiValue = arguments5.GetString(1);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (!string.IsNullOrEmpty(oem))
				{
					AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel(oem, abiValue);
					if (appPlayerModel != null)
					{
						new DownloadInstallOem(ParentWindow).DownloadOem(appPlayerModel);
					}
				}
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "CancelOemDownload", StringComparison.InvariantCulture))
		{
			CefListValue arguments6 = e.Message.Arguments;
			string oem2 = arguments6.GetString(0);
			string abiValue2 = arguments6.GetString(1);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (!string.IsNullOrEmpty(oem2))
				{
					AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel(oem2, abiValue2);
					if (appPlayerModel != null)
					{
						appPlayerModel.CancelOemDownload();
					}
				}
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "LaunchAppInDifferentOem", StringComparison.InvariantCulture))
		{
			CefListValue arguments7 = e.Message.Arguments;
			string oem3 = arguments7.GetString(0);
			string abiValue3 = arguments7.GetString(1);
			string vmname = arguments7.GetString(2);
			string packageName = arguments7.GetString(3);
			string actionWithRemainingInstances = arguments7.GetString(4);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (!string.IsNullOrEmpty(oem3))
				{
					InstalledOem.LaunchOemInstance(oem3, abiValue3, vmname, packageName, actionWithRemainingInstances);
				}
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "UninstallApp", StringComparison.InvariantCulture))
		{
			CefListValue arguments8 = e.Message.Arguments;
			string packageName2 = arguments8.GetString(0);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mAppInstaller?.UninstallApp(packageName2);
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "GetUpdatedGrm", StringComparison.InvariantCulture))
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				GrmHandler.SendUpdateGrmPackagesToBrowser(ParentWindow.mVmName);
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "RetryApkInstall", StringComparison.InvariantCulture))
		{
			CefListValue arguments9 = e.Message.Arguments;
			string apkFilePath = arguments9.GetString(0);
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (File.Exists(apkFilePath))
				{
					new DownloadInstallApk(ParentWindow).InstallApk(apkFilePath);
				}
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "ChooseAndInstallApk", StringComparison.InvariantCulture))
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				new DownloadInstallApk(ParentWindow).ChooseAndInstallApk();
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "GoogleSearch", StringComparison.InvariantCulture))
		{
			string text5 = e.Message.Arguments.GetString(0);
			SearchAppInPlayStore(text5);
			ClientStats.SendGPlayClickStats(new Dictionary<string, string>
			{
				{ "query", text5 },
				{ "source", "bs3_appsearch" }
			});
		}
		else if (string.Equals(e.Message.Name, "CloseSearch", StringComparison.InvariantCulture))
		{
			CloseSearch();
		}
		else if (string.Equals(e.Message.Name, "RefreshSearch", StringComparison.InvariantCulture))
		{
			CefListValue arguments10 = e.Message.Arguments;
			string _ = string.Empty;
			if (arguments10.Count > 0)
			{
				_ = arguments10.GetString(0);
			}
			RefreshSearch(_);
		}
		else if (string.Equals(e.Message.Name, "OfflineHtmlHomeUrl", StringComparison.InvariantCulture))
		{
			string offlineHtmlHomeUrl = e.Message.Arguments.GetString(0);
			RegistryManager.Instance.OfflineHtmlHomeUrl = offlineHtmlHomeUrl;
		}
		else if (string.Equals(e.Message.Name, "RefreshHomeHtml", StringComparison.InvariantCulture))
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.Utils.RefreshHtmlHomeUrl();
			}, new object[0]);
		}
		else if (string.Equals(e.Message.Name, "SetWebAppVersion", StringComparison.InvariantCulture))
		{
			string webAppVersion = e.Message.Arguments.GetString(0);
			RegistryManager.Instance.WebAppVersion = webAppVersion;
		}
		else if (string.Equals(e.Message.Name, "ShowWebPage", StringComparison.InvariantCulture))
		{
			CefListValue arguments11 = e.Message.Arguments;
			string title = arguments11.GetString(0);
			string webUrl = arguments11.GetString(1);
			ShowWebPage(title, webUrl);
		}
		else
		{
			if (string.Equals(e.Message.Name, "CloseBlockerAd", StringComparison.InvariantCulture))
			{
				return;
			}
			if (string.Equals(e.Message.Name, "CheckIfPremium", StringComparison.InvariantCulture))
			{
				string isPremium = e.Message.Arguments.GetString(0);
				CheckIfPremium(isPremium);
			}
			else
			{
				if (string.Equals(e.Message.Name, "GetImpressionId", StringComparison.InvariantCulture))
				{
					return;
				}
				if (string.Equals(e.Message.Name, "sendFirebaseNotification", StringComparison.InvariantCulture))
				{
					string json = e.Message.Arguments.GetString(0);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						CloudNotificationManager.Instance.HandleCloudNotification(json, ParentWindow.mVmName);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "PikaWorldProfileAdded", StringComparison.InvariantCulture))
				{
					string pikaWorldId = e.Message.Arguments.GetString(0);
					RegistryManager.Instance.PikaWorldId = pikaWorldId;
					return;
				}
				if (string.Equals(e.Message.Name, "subscribeModule", StringComparison.InvariantCulture))
				{
					string text6 = e.Message.Arguments.GetString(0);
					char[] separator = new char[1] { ',' };
					string[] array = text6.Split(separator, StringSplitOptions.None);
					PopulateTagsInfo(array, array[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "unsubscribeModule", StringComparison.InvariantCulture))
				{
					string text7 = e.Message.Arguments.GetString(0);
					char[] separator2 = new char[1] { ',' };
					string[] tagsList = text7.Split(separator2, StringSplitOptions.None);
					RemoveTagsInfo(tagsList);
					return;
				}
				if (string.Equals(e.Message.Name, "subscribeVmSpecificClientTags", StringComparison.InvariantCulture))
				{
					Dictionary<string, List<string>> dictionary = JToken.Parse(e.Message.Arguments.GetString(0)).ToObject<Dictionary<string, List<string>>>();
					if (mSubscriber == null)
					{
						mSubscriber = new BrowserSubscriber(this);
					}
					{
						foreach (KeyValuePair<string, List<string>> item in dictionary)
						{
							foreach (string item2 in item.Value)
							{
								if (Enum.IsDefined(typeof(BrowserControlTags), item2))
								{
									BrowserControlTags val = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), item2);
									if (!TagsSubscribedDict.ContainsKey(val))
									{
										JObject value = new JObject
										{
											["ClientTag"] = JToken.op_Implicit(item2),
											["CallbackFunction"] = JToken.op_Implicit(item.Key),
											["IsReceiveFromAllVm"] = JToken.op_Implicit(false)
										};
										TagsSubscribedDict.Add(val, value);
									}
									mSubscriber?.SubscribeTag(val);
									if ((int)val == 15)
									{
										Publisher.PublishMessage((BrowserControlTags)15, ParentWindow.mVmName, new JObject((object)new JProperty("VmId", (object)Utils.GetVmIdFromVmName(ParentWindow.mVmName))));
									}
									if ((int)val == 0 && ParentWindow.mGuestBootCompleted)
									{
										Logger.Info("Sending boot complete to browser immediately");
										Publisher.PublishMessage((BrowserControlTags)0, ParentWindow.mVmName, (JObject)null);
									}
								}
							}
						}
						return;
					}
				}
				if (string.Equals(e.Message.Name, "subscribeClientTags", StringComparison.InvariantCulture))
				{
					JArray val2 = JArray.Parse(e.Message.Arguments.GetString(0));
					if (mSubscriber == null)
					{
						mSubscriber = new BrowserSubscriber(this);
					}
					for (int num = 0; num < ((JContainer)val2).Count; num++)
					{
						JObject val3 = JObject.Parse(((object)val2[num]).ToString());
						if (Enum.IsDefined(typeof(BrowserControlTags), ((object)val3["ClientTag"]).ToString()))
						{
							BrowserControlTags val4 = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), ((object)val3["ClientTag"]).ToString());
							if (!TagsSubscribedDict.ContainsKey(val4))
							{
								TagsSubscribedDict.Add(val4, val3);
							}
							mSubscriber?.SubscribeTag(val4);
							if ((int)val4 == 15)
							{
								Publisher.PublishMessage((BrowserControlTags)15, ParentWindow.mVmName, new JObject((object)new JProperty("VmId", (object)Utils.GetVmIdFromVmName(ParentWindow.mVmName))));
							}
							if ((int)val4 == 0 && ParentWindow.mGuestBootCompleted)
							{
								Logger.Info("Sending boot complete to browser immediately");
								Publisher.PublishMessage((BrowserControlTags)0, ParentWindow.mVmName, (JObject)null);
							}
						}
					}
					return;
				}
				if (string.Equals(e.Message.Name, "unsubscribeClientTags", StringComparison.InvariantCulture))
				{
					foreach (string item3 in ((JToken)JArray.Parse(e.Message.Arguments.GetString(0))).ToObject<List<string>>())
					{
						if (Enum.IsDefined(typeof(BrowserControlTags), item3))
						{
							BrowserControlTags val5 = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), item3);
							if (TagsSubscribedDict.ContainsKey(val5))
							{
								TagsSubscribedDict.Remove(val5);
								mSubscriber?.UnsubscribeTag(val5);
							}
						}
					}
					return;
				}
				if (string.Equals(e.Message.Name, "ApplyThemeName", StringComparison.InvariantCulture))
				{
					string themeName = e.Message.Arguments.GetString(0);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.Utils.ApplyTheme(themeName);
						ParentWindow.Utils.RestoreWallpaperImageForAllVms();
						BlueStacksUIColorManager.ApplyTheme(themeName);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "GoToMapsTab", StringComparison.InvariantCulture))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						GoToMapsTab();
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "HandleClick", StringComparison.InvariantCulture))
				{
					string text8 = "";
					try
					{
						text8 = e.Message.Arguments.GetString(0);
						JToken res = JToken.Parse(text8);
						((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							ParentWindow.HideDimOverlay();
							ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)JsonExtensions.ToSerializableDictionary<string>(res), "handle_browser_click");
						}, new object[0]);
						return;
					}
					catch (Exception ex)
					{
						Logger.Error("Error when processing click action received from gmapi. JsonString: " + text8 + Environment.NewLine + "Error: " + ex.ToString());
						return;
					}
				}
				if (string.Equals(e.Message.Name, "UpdateQuestRules", StringComparison.InvariantCulture))
				{
					string text9 = "";
					try
					{
						text9 = e.Message.Arguments.GetString(0);
						PromotionManager.ReadQuests(JToken.Parse(text9), writePromo: true);
						return;
					}
					catch (Exception ex2)
					{
						Logger.Error("Error when processing UpdateQuestRules. JsonString: " + text9 + Environment.NewLine + "Error: " + ex2.ToString());
						return;
					}
				}
				if (string.Equals(e.Message.Name, "GetGamepadConnectionStatus", StringComparison.InvariantCulture))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (ParentWindow != null)
						{
							BlueStacksUIUtils.SendGamepadStatusToBrowsers(ParentWindow.IsGamepadConnected);
						}
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "CloseAnyPopup", StringComparison.InvariantCulture))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						if (ParentWindow != null)
						{
							ParentWindow.HideDimOverlay();
						}
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "SearchAppcenter", StringComparison.OrdinalIgnoreCase))
				{
					string searchText = e.Message.Arguments.GetString(0);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.mCommonHandler.SearchAppCenter(searchText);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "DownloadMacro", StringComparison.OrdinalIgnoreCase))
				{
					string macroData = e.Message.Arguments.GetString(0);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.Utils.DownloadAndUpdateMacro(macroData);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "ChangeControlScheme", StringComparison.OrdinalIgnoreCase))
				{
					string schemeSelected = e.Message.Arguments.GetString(0);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						KMManager.sGuidanceWindow?.SelectControlScheme(schemeSelected);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "CloseOnBoarding", StringComparison.OrdinalIgnoreCase))
				{
					string text10 = e.Message.Arguments.GetString(0);
					Logger.Info("CloseOnBoarding response from browser : " + text10);
					JObject res2 = JObject.Parse(text10);
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl?.Close();
						ParentWindow.StaticComponents.mSelectedTabButton.ShowBlurbOnboarding(res2);
						ParentWindow.HideDimOverlay();
						KMManager.sGuidanceWindow?.DimOverLayVisibility((Visibility)2);
					}, new object[0]);
					return;
				}
				if (string.Equals(e.Message.Name, "BrowserLoaded", StringComparison.OrdinalIgnoreCase))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						this.BrowserLoadCompleteEvent?.Invoke();
					}, new object[0]);
					return;
				}
				try
				{
					object[] array2 = null;
					if (e.Message.Arguments.Count > 0)
					{
						array2 = new object[e.Message.Arguments.Count];
						for (int num2 = 0; num2 < e.Message.Arguments.Count; num2++)
						{
							if (e.Message.Arguments.GetString(num2) != null)
							{
								array2[num2] = e.Message.Arguments.GetString(num2).ToString(CultureInfo.InvariantCulture);
								Logger.Info("web api call.." + e.Message.Name + "..with args.." + e.Message.Arguments.GetString(num2).ToString(CultureInfo.InvariantCulture));
							}
							else
							{
								array2[num2] = string.Empty;
							}
						}
					}
					((object)this).GetType().GetMethod(e.Message.Name).Invoke(this, array2);
				}
				catch (Exception ex3)
				{
					Logger.Error("Error in executing gmapi " + e.Message.Name + ": " + ex3.ToString());
				}
				ProcessMessageEventHandler obj = this.ProcessMessageRecieved;
				if (obj != null)
				{
					obj.Invoke(sender, e);
				}
			}
		}
	}

	internal void RemoveTagsInfo(string[] tagsList)
	{
		foreach (string item in tagsList)
		{
			if (mFirebaseTagsSubscribed.Contains(item))
			{
				mFirebaseTagsSubscribed.Remove(item);
			}
		}
	}

	public void PopulateTagsInfo(string[] tagsList, string methodName)
	{
		if (tagsList != null)
		{
			foreach (string text in tagsList)
			{
				if (!string.Equals(text, methodName, StringComparison.InvariantCultureIgnoreCase))
				{
					mFirebaseTagsSubscribed.Add(text);
				}
			}
		}
		mFirebaseCallbackMethod = methodName;
	}

	public void GoToMapsTab()
	{
		if (ParentWindow != null)
		{
			ParentWindow.mTopBar.mAppTabButtons.GoToTab("pikaworld");
		}
	}

	private void CheckIfPremium(string isPremium)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (isPremium.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				RegistryManager.Instance.IsPremium = true;
				ParentWindow.mTopBar.ChangeUserPremiumButton(isPremium: true);
			}
			else
			{
				RegistryManager.Instance.IsPremium = false;
				ParentWindow.mTopBar.ChangeUserPremiumButton(isPremium: false);
			}
			PromotionObject.AppRecommendationHandler?.Invoke(obj: true);
		}, new object[0]);
	}

	private void InstallApp(string iconUrl, string appName, string apkUrl, string package, string timestamp = "")
	{
		if (!string.IsNullOrEmpty(package))
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				new DownloadInstallApk(ParentWindow).DownloadAndInstallApp(iconUrl, appName, apkUrl, package, isLaunchAfterInstall: false, isDeleteApk: true, timestamp);
			}, new object[0]);
		}
	}

	private void ShowAppInPlayStore(string packageName)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(packageName, (PlayStoreAction)0);
		}, new object[0]);
	}

	private void ShowAppInPlayStorePopup(string packageName, string appName)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(packageName, appName, (PlayStoreAction)0);
			((UIElement)ParentWindow.mWelcomeTab.mFrontendPopupControl).Visibility = (Visibility)0;
		}, new object[0]);
	}

	private void SearchAppInPlayStore(string searchString)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (searchString != null)
			{
				ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(searchString, (PlayStoreAction)1);
			}
		}, new object[0]);
	}

	private void CloseSearch()
	{
		if (CefBrowser != null)
		{
			((WpfCefBrowser)CefBrowser).NavigateTo(mUrl);
		}
	}

	private void RefreshSearch(string _)
	{
	}

	public void ShowWebPage(string title, string webUrl)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (title == null)
			{
				title = "";
			}
			if (ParentWindow != null)
			{
				ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab");
			}
			else
			{
				MainWindow activatedWindow = null;
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					activatedWindow = BlueStacksUIUtils.DictWindows.Values.First();
				}
				((DispatcherObject)activatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					activatedWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab");
				}, new object[0]);
			}
		}, new object[0]);
	}

	public void CloseSelf()
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			AppTabButton selectedTab = ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			if (selectedTab != null)
			{
				ParentWindow.mTopBar.mAppTabButtons.CloseTab(selectedTab.TabKey);
			}
		}, new object[0]);
	}

	public void CloseBrowserQuitPopup()
	{
		ParentWindow.CloseBrowserQuitPopup();
	}

	internal void ReInitBrowser(string url)
	{
		((WpfCefBrowser)CefBrowser).Dispose();
		CefBrowser = null;
		mUrl = url;
		CreateNewBrowser();
	}

	public static void DownloadBTV()
	{
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList()[0]];
			((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				BTVManager.Instance.MaybeDownloadAndLaunchBTv(window);
			}, new object[0]);
		}
	}

	public static void DownloadDirectX()
	{
		if (BlueStacksUIUtils.DictWindows.Count <= 0)
		{
			return;
		}
		MainWindow activatedWindow = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList()[0]];
		((DispatcherObject)activatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
			CustomMessageWindow window = new CustomMessageWindow();
			window.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
			window.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
			window.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), (RequestNavigateEventHandler)delegate(object o, RequestNavigateEventArgs arg)
			{
				BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
				window.CloseWindow();
			});
			window.AddButton((ButtonColors)4, "STRING_DOWNLOAD_NOW", (EventHandler)delegate
			{
				BlueStacksUIUtils.OpenUrl(directXDownloadURL);
			}, (string)null, false, (object)null);
			window.AddButton((ButtonColors)2, "STRING_NO", (EventHandler)null, (string)null, false, (object)null);
			((Window)window).Owner = (Window)(object)activatedWindow;
			((Window)window).ShowDialog();
			((FrameworkElement)activatedWindow).BringIntoView();
		}, new object[0]);
	}

	public static void SetSystemVolume(string level)
	{
		StreamManager.Instance.SetSystemVolume(level);
	}

	public static void SetMicVolume(string level)
	{
		if (string.Equals(level?.Trim(), "0", StringComparison.InvariantCultureIgnoreCase))
		{
			StreamManager.mIsMicDisabled = true;
		}
		StreamManager.Instance.SetMicVolume(level);
	}

	public static void EnableWebcam(string width, string height, string position)
	{
		StreamManager.EnableWebcam(width, height, position);
	}

	public static void DisableWebcamV2(string jsonString)
	{
		StreamManager.Instance.DisableWebcamV2(jsonString);
	}

	public static void MoveWebcam(string horizontal, string vertical)
	{
		StreamManager.Instance.MoveWebcam(horizontal, vertical);
	}

	public static void StopRecord()
	{
		if (StreamManager.Instance != null)
		{
			Logger.Info("Got StopRecord");
			StreamManager.Instance.StopRecord(immediate: true);
		}
	}

	public static void StopStream()
	{
		StreamManager.Instance.StopStream();
	}

	public static void ShowPreview()
	{
	}

	public static void HidePreview()
	{
	}

	public void StartObs(string _)
	{
		InitStreamManager();
		StreamManager.Instance.StartObs();
	}

	public void GetRealtimeAppUsage(string callBackFunction)
	{
		try
		{
			Dictionary<string, Dictionary<string, long>> realtimeDictionary = AppUsageTimer.GetRealtimeDictionary();
			if (!string.IsNullOrEmpty(callBackFunction))
			{
				CallBackToHtml(callBackFunction, JSONUtils.GetJSONObjectString<long>(realtimeDictionary[ParentWindow.mVmName]));
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while sending realtime dictionary to gmapi" + ex.ToString());
		}
	}

	public void GetInstalledAppsForAllOems(string callBackFunction)
	{
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		string b = "Android";
		if (!string.IsNullOrEmpty(ParentWindow?.mVmName))
		{
			b = ParentWindow.mVmName;
		}
		JArray val = new JArray();
		foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
		{
			JArray val2 = new JArray();
			string[] vmList = RegistryManager.RegistryManagers[installedCoexistingOem].VmList;
			foreach (string text in vmList)
			{
				if (string.Equals(installedCoexistingOem, "bgp64", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(text, b, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				string path = Path.Combine(RegistryManager.RegistryManagers[installedCoexistingOem].DataDir, "UserData\\Gadget\\apps_" + text + ".json");
				string text2 = "[]";
				using (Mutex mutex = new Mutex(initiallyOwned: false, "BlueStacks_AppJsonUpdate"))
				{
					if (mutex.WaitOne())
					{
						try
						{
							StreamReader streamReader = new StreamReader(path);
							text2 = streamReader.ReadToEnd();
							streamReader.Close();
						}
						catch (Exception ex)
						{
							Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
						}
						finally
						{
							mutex.ReleaseMutex();
						}
					}
				}
				string suffix = InstalledOem.GetAppPlayerModel(installedCoexistingOem, Utils.GetValueInBootParams("abivalue", text, string.Empty, installedCoexistingOem)).Suffix;
				if (string.IsNullOrEmpty(RegistryManager.RegistryManagers[installedCoexistingOem].Guest[text].DisplayName))
				{
					string text3 = "";
					text3 = ((!string.Equals(text, "Android", StringComparison.InvariantCultureIgnoreCase)) ? (Strings.ProductDisplayName + " " + Utils.GetVmIdFromVmName(text) + " " + suffix) : (Strings.ProductDisplayName + " " + suffix));
					RegistryManager.RegistryManagers[installedCoexistingOem].Guest[text].DisplayName = text3.Trim();
				}
				JObject val3 = new JObject();
				val3.Add("vmname", JToken.op_Implicit(text));
				val3.Add("vmdisplayname", JToken.op_Implicit(RegistryManager.RegistryManagers[installedCoexistingOem].Guest[text].DisplayName));
				val3.Add("abiValue", JToken.op_Implicit(Utils.GetValueInBootParams("abivalue", text, string.Empty, installedCoexistingOem)));
				val3.Add("oemSuffix", JToken.op_Implicit(string.IsNullOrEmpty(suffix) ? "N-32" : suffix));
				val3.Add("filedata", (JToken)(object)JArray.Parse(text2));
				val2.Add((JToken)val3);
			}
			JObject val4 = new JObject();
			val4.Add("oem", JToken.op_Implicit(installedCoexistingOem));
			val4.Add("vmlist", (JToken)(object)val2);
			val.Add((JToken)val4);
		}
		if (!string.IsNullOrEmpty(callBackFunction))
		{
			string text4 = ((JToken)new JObject((object)new JProperty("oemlist", (object)((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0])))).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
			text4 = text4.Replace("\n", "");
			text4 = text4.Replace("\r", "");
			text4 = Regex.Replace(text4, "\\s+", " ", RegexOptions.Multiline);
			CallBackToHtml(callBackFunction, text4);
		}
	}

	public unsafe void GetSystemInfo(string callbackFunction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		try
		{
			num = (int)(new ComputerInfo().TotalPhysicalMemory / 1048576);
		}
		catch (Exception arg)
		{
			Logger.Error($"Exception when finding ram {arg}");
		}
		bool flag = false;
		string text = "";
		try
		{
			if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers["bgp64"].AvailableGPUDetails))
			{
				flag = true;
				text = RegistryManager.RegistryManagers["bgp64"].AvailableGPUDetails;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in getting gpu details " + ex);
		}
		try
		{
			GlMode glModeForVm = Utils.GetGlModeForVm(ParentWindow.mVmName);
			EngineState val = (EngineState)0;
			if (RegistryManager.Instance.CurrentEngine == "raw")
			{
				val = (EngineState)1;
			}
			int guestWidth = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth;
			int guestHeight = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight;
			string text2 = guestWidth.ToString(CultureInfo.InvariantCulture) + "x" + guestHeight.ToString(CultureInfo.InvariantCulture);
			JObject val2 = new JObject();
			val2.Add("physicalCpu", JToken.op_Implicit(Environment.ProcessorCount));
			val2.Add("physicalRam", JToken.op_Implicit(num));
			val2.Add("isGpuAvailable", JToken.op_Implicit(flag));
			val2.Add("gpuText", JToken.op_Implicit(text));
			val2.Add("engineMode", JToken.op_Implicit(((object)(*(EngineState*)(&val))/*cast due to constrained. prefix*/).ToString()));
			val2.Add("glMode", JToken.op_Implicit(((object)(*(GlMode*)(&glModeForVm))/*cast due to constrained. prefix*/).ToString()));
			val2.Add("ram", JToken.op_Implicit(RegistryManager.Instance.Guest[ParentWindow.mVmName].Memory));
			val2.Add("cpuAllocated", JToken.op_Implicit(RegistryManager.Instance.Guest[ParentWindow.mVmName].VCPUs));
			val2.Add("dpi", JToken.op_Implicit(Utils.GetDpiFromBootParameters(RegistryManager.Instance.Guest[ParentWindow.mVmName].BootParameters)));
			val2.Add("fps", JToken.op_Implicit(RegistryManager.Instance.Guest[ParentWindow.mVmName].FPS));
			val2.Add("res", JToken.op_Implicit(text2));
			val2.Add("installedOems", JToken.op_Implicit(string.Join(",", InstalledOem.AllInstalledOemList.ToArray())));
			val2.Add("pcode", JToken.op_Implicit(Utils.GetValueInBootParams("pcode", ParentWindow.mVmName, "", "bgp64")));
			val2.Add("astcOption", JToken.op_Implicit(((object)RegistryManager.Instance.Guest[ParentWindow.mVmName].ASTCOption/*cast due to constrained. prefix*/).ToString()));
			val2.Add("abiValue", JToken.op_Implicit(Utils.GetValueInBootParams("abivalue", ParentWindow.mVmName, "", "bgp64")));
			JObject val3 = val2;
			if (!string.IsNullOrEmpty(callbackFunction))
			{
				CallBackToHtml(callbackFunction, ((JToken)val3).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in getting system info details " + ex2);
		}
	}

	public void GetInstalledAppsJsonforJS(string callBackFunction)
	{
		bool flag = false;
		string text = "Android";
		if (!string.IsNullOrEmpty(ParentWindow?.mVmName))
		{
			text = ParentWindow.mVmName;
		}
		string path = Path.Combine(RegistryStrings.GadgetDir, "apps_" + text + ".json");
		string text2 = "[" + Environment.NewLine + "]";
		using (Mutex mutex = new Mutex(initiallyOwned: false, "BlueStacks_AppJsonUpdate"))
		{
			if (mutex.WaitOne())
			{
				try
				{
					StreamReader streamReader = new StreamReader(path);
					text2 = streamReader.ReadToEnd();
					streamReader.Close();
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}
		if (flag)
		{
			text2 = text2.Replace("\"", "\\\"");
		}
		text2 = text2.Replace("\n", "");
		text2 = text2.Replace("\r", "");
		text2 = Regex.Replace(text2, "\\s+", " ", RegexOptions.Multiline);
		if (!string.IsNullOrEmpty(callBackFunction))
		{
			CallBackToHtml(callBackFunction, text2);
		}
	}

	public void PerformOTS(string callbackFunction)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			AppIconModel appIcon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
			if (appIcon != null)
			{
				ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: true, isLaunch: true);
			}
		}, new object[0]);
		if (!string.IsNullOrEmpty(callbackFunction))
		{
			ParentWindow.mBrowserCallbackFunctionName = callbackFunction;
			ParentWindow.BrowserOTSCompletedCallback += ParentWindow_BrowserOTSCompletedCallback;
		}
	}

	private void ParentWindow_BrowserOTSCompletedCallback(object sender, MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args)
	{
		string data = RegistryManager.Instance.Token + "@@" + RegistryManager.Instance.RegisteredEmail;
		CallBackToHtml(args.CallbackFunction, data);
		string communityWebTabKey = LocaleStrings.GetLocalizedString("STRING_MACRO_COMMUNITY", "");
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(communityWebTabKey) && ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey != communityWebTabKey)
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.mTopBar.mAppTabButtons.GoToTab(communityWebTabKey);
				}, new object[0]);
			}
		}, new object[0]);
	}

	public string GetCurrentAppInfo(string callBackFunction)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		MainWindow mainWindow = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			mainWindow = BlueStacksUIUtils.DictWindows.Values.First();
		}
		AppTabButton selectedTab = mainWindow.mTopBar.mAppTabButtons.SelectedTab;
		if (selectedTab == null)
		{
			return "{}";
		}
		string appName = selectedTab.AppName;
		string packageName = selectedTab.PackageName;
		JObject val = new JObject();
		val.Add("type", JToken.op_Implicit("app"));
		val.Add("name", JToken.op_Implicit(appName));
		val.Add("data", JToken.op_Implicit(packageName));
		JObject val2 = val;
		if (!string.IsNullOrEmpty(callBackFunction))
		{
			CallBackToHtml(callBackFunction, ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
		}
		return ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
	}

	public void StartStreamV2(string jsonString, string callbackStreamStatus, string callbackTabChanged)
	{
		Logger.Info("Got StartStreamV2");
		InitStreamManager();
		if (StreamManager.Instance.mReplayBufferEnabled)
		{
			StreamManager.Instance.StartReplayBuffer();
		}
		Logger.Info("Got StartStream");
		StreamManager.Instance.StartStream(jsonString, callbackStreamStatus, callbackTabChanged);
	}

	private StreamManager InitStreamManager()
	{
		if (StreamManager.Instance == null)
		{
			StreamManager.Instance = new StreamManager(CefBrowser);
		}
		else
		{
			StreamManager.GetStreamConfig(out var handle, out var _);
			StreamManager.Instance.SetHwnd(handle);
		}
		return StreamManager.Instance;
	}

	public void makeWebCall(string url, string scriptToInvoke)
	{
		HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
		httpWebRequest.Method = "GET";
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
		string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + text;
		new Uri(url);
		try
		{
			Logger.Info("making a webcall at url=" + url);
			string text2 = null;
			using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
			{
				using Stream stream = httpWebResponse.GetResponseStream();
				using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
				text2 = streamReader.ReadToEnd();
			}
			object[] args = new object[1] { text2 };
			CefBrowser.CallJs(scriptToInvoke, args);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in MakeWebCall. err : " + ex.ToString());
			string text3 = "";
			object[] args2 = new object[1] { text3 };
			CefBrowser.CallJs(scriptToInvoke, args2);
		}
	}

	public static void LaunchDialog(string jsonString)
	{
		try
		{
			JObject val = JObject.Parse(jsonString);
			if (val["parameter"] != null)
			{
				((object)val["parameter"]).ToString();
			}
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				BlueStacksUIUtils.DictWindows.Values.First();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in launchDialog gmApi : " + ex.ToString());
		}
	}

	public static void CloseFilterWindow(string _)
	{
	}

	public void CallBackToHtml(string callBackFunction, string data)
	{
		if (data != null)
		{
			data = data?.Replace("\\", "\\\\");
			string text = callBackFunction + "('" + data?.Replace("'", "&#39;").Replace("%27", "&#39;") + "')";
			Browser cefBrowser = CefBrowser;
			if (cefBrowser != null)
			{
				Browser cefBrowser2 = CefBrowser;
				((WpfCefBrowser)cefBrowser).ExecuteJavaScript(text, (cefBrowser2 != null) ? ((WpfCefBrowser)cefBrowser2).getURL() : null, 0);
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (disposedValue)
		{
			return;
		}
		if (CefBrowser != null)
		{
			((WpfCefBrowser)CefBrowser).LoadEnd -= new LoadEndEventHandler(MBrowser_LoadEnd);
			((WpfCefBrowser)CefBrowser).ProcessMessageRecieved -= new ProcessMessageEventHandler(Browser_ProcessMessageRecieved);
			((FrameworkElement)CefBrowser).Loaded -= new RoutedEventHandler(Browser_Loaded);
			((WpfCefBrowser)CefBrowser).LoadError -= new LoadErrorEventHandler(Browser_LoadError);
			((WpfCefBrowser)CefBrowser).LoadingStateChange -= new LoadingStateChangeEventHandler(Browser_LoadingStateChange);
			((WpfCefBrowser)CefBrowser).mWPFCefBrowserExceptionHandler -= new ExceptionHandler(Browser_WPFCefBrowserExceptionHandler);
			((WpfCefBrowser)CefBrowser).Dispose();
			CefBrowserHost obj = mBrowserHost;
			if (obj != null)
			{
				obj.Dispose();
			}
			foreach (BrowserControlTags key in TagsSubscribedDict.Keys)
			{
				mSubscriber?.UnsubscribeTag(key);
			}
		}
		disposedValue = true;
	}

	~BrowserControl()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
