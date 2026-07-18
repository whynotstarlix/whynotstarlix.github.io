using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal sealed class BTVManager : IDisposable
{
	private static volatile BTVManager instance;

	private static object syncRoot = new object();

	public bool sStreaming;

	public static string sNetwork = "twitch";

	public bool sRecording;

	public bool sWasRecording;

	public bool sStopPingBTVThread;

	public object sPingBTVLock = new object();

	private CustomMessageWindow sWindow;

	private bool sDownloading;

	private LegacyDownloader sDownloader;

	private static string sBTvUrl = "https://cloud.bluestacks.com/bs4/btv/GetBTVFile";

	private bool disposedValue;

	public static BTVManager Instance
	{
		get
		{
			if (instance == null)
			{
				lock (syncRoot)
				{
					if (instance == null)
					{
						instance = new BTVManager();
					}
				}
			}
			return instance;
		}
	}

	public static bool sWritingToFile
	{
		set
		{
			HTTPServer.FileWriteComplete = !value;
		}
	}

	private BTVManager()
	{
	}

	public void StartBlueStacksTV()
	{
		using Process process = new Process();
		string installDir = RegistryStrings.InstallDir;
		process.StartInfo.FileName = Path.Combine(installDir, "BlueStacksTV.exe");
		process.StartInfo.Arguments = "-u";
		process.Start();
		Thread.Sleep(1000);
		Thread thread = new Thread(StartPingBTVThread);
		thread.IsBackground = true;
		thread.Start();
	}

	private void BtvWindow_Closing(object sender, CancelEventArgs e)
	{
	}

	internal static void BringToFront(CustomWindow win)
	{
		try
		{
			((DispatcherObject)win).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Invalid comparison between Unknown and I4
				if ((int)((Window)win).WindowState == 1)
				{
					((Window)win).WindowState = (WindowState)0;
				}
				((UIElement)win).Visibility = (Visibility)0;
				((Window)win).Show();
				((FrameworkElement)win).BringIntoView();
				if (!((Window)win).Topmost)
				{
					((Window)win).Topmost = true;
					((Window)win).Topmost = false;
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("An error was triggered in bringing BTv downloader to front", new object[1] { ex.Message });
		}
	}

	public static void ReportObsErrorHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got ReportObsErrorHandler");
		HTTPUtils.ParseRequest(req);
		try
		{
			StreamManager.Instance.ReportObsError("obs_error");
			StreamManager.Instance = null;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReportObsHandler");
			Logger.Error(ex.ToString());
		}
	}

	private void CancelBTvDownload(object sender, EventArgs e)
	{
		Logger.Info("User cancelled BTV download");
		sDownloading = false;
		if (sDownloader != null)
		{
			sDownloader.AbortDownload();
			if (IsBTVInstalled())
			{
				Directory.Delete(RegistryStrings.ObsDir, recursive: true);
			}
		}
	}

	private void CancelDownloadConfirmation(object sender, EventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		MainWindow owner = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			owner = BlueStacksUIUtils.DictWindows.Values.First();
		}
		CustomMessageWindow val = new CustomMessageWindow();
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_BTV_DOWNLOAD_CANCEL", "");
		val.AddButton((ButtonColors)0, "STRING_CANCEL", (EventHandler)CancelBTvDownload, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CONTINUE", (EventHandler)null, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)owner;
		((Window)val).ShowDialog();
	}

	internal static bool IsBTVInstalled()
	{
		if (!Directory.Exists(RegistryStrings.BtvDir) || !Directory.Exists(RegistryStrings.ObsDir))
		{
			return false;
		}
		return true;
	}

	internal static bool IsDirectXComponentsInstalled()
	{
		string systemDirectory = Environment.SystemDirectory;
		string[] array = new string[4] { "D3DX10_43.DLL", "D3D10_1.DLL", "DXGI.DLL", "D3DCompiler_43.dll" };
		foreach (string path in array)
		{
			if (!File.Exists(Path.Combine(systemDirectory, path)))
			{
				return false;
			}
		}
		return true;
	}

	public void MaybeDownloadAndLaunchBTv(MainWindow parentWindow)
	{
		if (!IsBTVInstalled())
		{
			if (sDownloading && sWindow != null)
			{
				BringToFront((CustomWindow)(object)sWindow);
				return;
			}
			ExtensionPopupControl btvExtPopup = new ExtensionPopupControl();
			btvExtPopup.LoadExtensionPopupFromFolder("BTVExtensionPopup");
			btvExtPopup.DownloadClicked += delegate
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Expected O, but got Unknown
				BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)btvExtPopup);
				sDownloading = true;
				sWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(sWindow.TitleTextBlock, "STRING_BTV_DOWNLOAD", "");
				BlueStacksUIBinding.Bind(sWindow.BodyTextBlock, "STRING_BTV_INSTALL_WAIT", "");
				BlueStacksUIBinding.Bind(sWindow.BodyWarningTextBlock, "STRING_BTV_WARNING", "");
				sWindow.AddButton((ButtonColors)4, "STRING_CANCEL", (EventHandler)CancelDownloadConfirmation, (string)null, false, (object)null);
				((UIElement)sWindow.BodyWarningTextBlock).Visibility = (Visibility)0;
				sWindow.ProgressBarEnabled = true;
				sWindow.IsWindowMinizable = true;
				sWindow.IsWindowClosable = false;
				sWindow.ImageName = "BTVTopBar";
				((Window)sWindow).ShowInTaskbar = true;
				((Window)sWindow).Owner = (Window)(object)parentWindow;
				((CustomWindow)sWindow).IsShowGLWindow = true;
				((Window)sWindow).Show();
				Thread thread = new Thread((ThreadStart)delegate
				{
					//IL_0076: Unknown result type (might be due to invalid IL or missing references)
					//IL_0080: Expected O, but got Unknown
					//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ca: Expected O, but got Unknown
					//IL_00ca: Expected O, but got Unknown
					//IL_009d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Expected O, but got Unknown
					//IL_00a9: Expected O, but got Unknown
					if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
					{
						sBTvUrl = RegistryManager.Instance.BtvDevServer;
					}
					string redirectedUrl = GetRedirectedUrl(sBTvUrl);
					if (redirectedUrl == null)
					{
						Logger.Error("The download url was null");
					}
					else
					{
						string fileName = Path.GetFileName(new Uri(redirectedUrl).LocalPath);
						string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
						sDownloader = new LegacyDownloader(3, redirectedUrl, downloadPath);
						LegacyDownloader obj = sDownloader;
						UpdateProgressCallback val = default(UpdateProgressCallback);
						UpdateProgressCallback obj2 = val;
						if (obj2 == null)
						{
							UpdateProgressCallback val2 = delegate(int percent)
							{
								((DispatcherObject)sWindow).Dispatcher.Invoke((Delegate)(Action)delegate
								{
									((RangeBase)sWindow.CustomProgressBar).Value = percent;
								}, new object[0]);
							};
							UpdateProgressCallback val3 = val2;
							val = val2;
							obj2 = val3;
						}
						obj.Download(obj2, (DownloadCompletedCallback)delegate
						{
							((DispatcherObject)sWindow).Dispatcher.Invoke((Delegate)(Action)delegate
							{
								((RangeBase)sWindow.CustomProgressBar).Value = 100.0;
								((Window)sWindow).Close();
							}, new object[0]);
							Logger.Info("Successfully downloaded BlueStacks TV");
							sDownloading = false;
							ExtractBTv(downloadPath);
							((DispatcherObject)parentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
							{
								parentWindow.mTopBar.mBtvButton.ImageName = "btv";
							}, new object[0]);
						}, (ExceptionCallback)delegate(Exception ex)
						{
							Logger.Error("Failed to download file: {0}. err: {1}", new object[2] { downloadPath, ex.Message });
						}, (ContentTypeCallback)null, (SizeDownloadedCallback)null, (PayloadInfoCallback)null);
					}
				});
				thread.IsBackground = true;
				thread.Start();
			};
			((FrameworkElement)btvExtPopup).Height = ((FrameworkElement)parentWindow).ActualHeight * 0.8;
			((FrameworkElement)btvExtPopup).Width = ((FrameworkElement)btvExtPopup).Height * 16.0 / 9.0;
			new ContainerWindow(parentWindow, (UserControl)(object)btvExtPopup, (int)((FrameworkElement)btvExtPopup).Width, (int)((FrameworkElement)btvExtPopup).Height);
		}
		else
		{
			StartBlueStacksTV();
		}
	}

	internal static void ReportOpenGLCaptureError(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got open gl CaptureError");
		try
		{
			StreamManager.Instance.ReportObsError("opengl_capture_error");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReportObsHandler");
			Logger.Error(ex.ToString());
		}
	}

	internal static void ReportCaptureError(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got ReportCaptureError");
		HTTPUtils.ParseRequest(req);
		try
		{
			StreamManager.Instance.ReportObsError("capture_error");
			StreamManager.Instance = null;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ReportObsHandler");
			Logger.Error(ex.ToString());
		}
	}

	internal static void ObsStatusHandler(HttpListenerRequest req, HttpListenerResponse res)
	{
		Logger.Info("Got ObsStatus {0} request from {1}", new object[2]
		{
			req.HttpMethod,
			req.RemoteEndPoint.ToString()
		});
		try
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (requestData.Data.Count <= 0 || !(requestData.Data.AllKeys[0] == "Error") || StreamManager.sStopInitOBSQueue)
			{
				return;
			}
			if (string.Equals(requestData.Data[0], "OBSAlreadyRunning", StringComparison.InvariantCulture))
			{
				StreamManager.sStopInitOBSQueue = true;
			}
			if (StreamManager.Instance != null)
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					StreamManager.Instance.ReportObsError(requestData.Data[0]);
				});
				thread.IsBackground = true;
				thread.Start();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in ObsStatus");
			Logger.Error(ex.ToString());
		}
	}

	internal static string GetRedirectedUrl(string url)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.AllowAutoRedirect = true;
		string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + text;
		httpWebRequest.Headers.Add("x_oem", RegistryManager.Instance.Oem);
		httpWebRequest.Headers.Set("x_email", RegistryManager.Instance.RegisteredEmail);
		httpWebRequest.Headers.Add("x_guid", RegistryManager.Instance.UserGuid);
		httpWebRequest.Headers.Add("x_prod_ver", RegistryManager.Instance.Version);
		httpWebRequest.Headers.Add("x_home_app_ver", RegistryManager.Instance.ClientVersion);
		try
		{
			using HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
			using Stream stream = httpWebResponse.GetResponseStream();
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			JObject val = JObject.Parse(streamReader.ReadToEnd());
			if (val["success"].ToObject<bool>())
			{
				return ((object)val["file_url"]).ToString();
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("Error in getting redirected url for BTV " + ex.ToString());
			return null;
		}
	}

	internal static bool ExtractBTv(string downloadPath)
	{
		try
		{
			if (File.Exists(downloadPath))
			{
				if (MiscUtils.Extract7Zip(downloadPath, RegistryManager.Instance.UserDefinedDir) == 0)
				{
					return true;
				}
				Logger.Error("Could not extract BTv zip file.");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Could not extract BTv zip file. Error: " + ex.ToString());
		}
		return false;
	}

	public void StartPingBTVThread()
	{
		lock (sPingBTVLock)
		{
			Logger.Info("Starting btv ping thread");
			while (true)
			{
				PingBTV();
				if (sStopPingBTVThread)
				{
					break;
				}
				Thread.Sleep(5000);
			}
		}
	}

	public void ShowStreamWindow()
	{
		if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
		{
			StartBlueStacksTV();
		}
		else
		{
			SendBTVAsyncRequest("showstreamwindow", null);
		}
	}

	public void HideStreamWindow()
	{
		if (ProcessUtils.FindProcessByName("BlueStacksTV"))
		{
			SendBTVAsyncRequest("hidestreamwindow", null);
		}
	}

	public void HideStreamWindowFromTaskbar()
	{
		SendBTVAsyncRequest("hidestreamwindowfromtaskbar", null);
	}

	public static void GetStreamDimensionInfo(out int startX, out int startY, out int width, out int height)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Point p = default(Point);
		MainWindow activatedWindow = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			activatedWindow = BlueStacksUIUtils.DictWindows.Values.First();
		}
		((DispatcherObject)activatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			p = ((UIElement)activatedWindow.mFrontendGrid).TranslatePoint(new Point(0.0, 0.0), (UIElement)(object)activatedWindow.mFrontendGrid);
		}, new object[0]);
		startX = Convert.ToInt32(((Point)(ref p)).X) * SystemUtils.GetDPI() / 96;
		startY = Convert.ToInt32(((Point)(ref p)).Y) * SystemUtils.GetDPI() / 96;
		width = (int)((FrameworkElement)activatedWindow.mFrontendGrid).ActualWidth * SystemUtils.GetDPI() / 96;
		height = (int)((FrameworkElement)activatedWindow.mFrontendGrid).ActualHeight * SystemUtils.GetDPI() / 96;
	}

	public void PingBTV()
	{
		bool flag = false;
		bool flag2 = false;
		try
		{
			string text = SendBTVRequest("ping", null);
			JArray.Parse(text);
			JObject val = JObject.Parse(text[0].ToString(CultureInfo.InvariantCulture));
			if (val["success"].ToObject<bool>())
			{
				flag = val["recording"].ToObject<bool>();
				flag2 = val["streaming"].ToObject<bool>();
			}
			Logger.Info("Ping BTV response recording: {0}, streaming: {1}", new object[2] { flag, flag2 });
			sStopPingBTVThread = false;
		}
		catch (Exception ex)
		{
			sStopPingBTVThread = true;
			Logger.Error("PingBTV : {0}", new object[1] { ex.Message });
		}
		sRecording = flag;
		sStreaming = flag2;
	}

	public void SetFrontendPosition(int width, int height, bool isPortrait)
	{
		if (ProcessUtils.FindProcessByName("BlueStacksTV"))
		{
			Dictionary<string, string> _ = new Dictionary<string, string>
			{
				{
					"width",
					width.ToString(CultureInfo.InvariantCulture)
				},
				{
					"height",
					height.ToString(CultureInfo.InvariantCulture)
				},
				{
					"isPortrait",
					isPortrait.ToString(CultureInfo.InvariantCulture)
				}
			};
			SendBTVRequest("setfrontendposition", _);
		}
	}

	public void WindowResized()
	{
		if (ProcessUtils.FindProcessByName("BlueStacksTV"))
		{
			try
			{
				SendBTVRequest("windowresized", null);
			}
			catch (Exception ex)
			{
				Logger.Error("{0}", new object[1] { ex });
			}
		}
	}

	public void StreamStarted()
	{
		sWritingToFile = true;
		sRecording = true;
		sStreaming = true;
	}

	public void StreamStopped()
	{
		sWritingToFile = false;
		sStreaming = false;
		sRecording = false;
		RestrictWindowResize(enable: false);
	}

	public void RecordStarted()
	{
		sWritingToFile = true;
		sRecording = true;
		sWasRecording = true;
	}

	public void SetConfig()
	{
		GetStreamDimensionInfo(out var startX, out var startY, out var width, out var height);
		Dictionary<string, string> _ = new Dictionary<string, string>
		{
			{
				"startX",
				startX.ToString(CultureInfo.InvariantCulture)
			},
			{
				"startY",
				startY.ToString(CultureInfo.InvariantCulture)
			},
			{
				"width",
				width.ToString(CultureInfo.InvariantCulture)
			},
			{
				"height",
				height.ToString(CultureInfo.InvariantCulture)
			}
		};
		SendBTVRequest("setconfig", _);
	}

	public void RecordStopped()
	{
		sWritingToFile = false;
		sRecording = false;
		RestrictWindowResize(enable: false);
	}

	public void SendTabChangeData(string[] tabChangedData)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			Dictionary<string, string> _ = new Dictionary<string, string>
			{
				{
					"type",
					tabChangedData[0]
				},
				{
					"name",
					tabChangedData[1]
				},
				{
					"data",
					tabChangedData[2]
				}
			};
			SendBTVRequest("tabchangeddata", _);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void ReplayBufferSaved()
	{
		MainWindow mainWindow = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			mainWindow = BlueStacksUIUtils.DictWindows.Values.First();
		}
		((DispatcherObject)mainWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Invalid comparison between Unknown and I4
			SaveFileDialog val = new SaveFileDialog
			{
				Filter = "Flash Video (*.flv)|*.flv",
				FilterIndex = 1,
				RestoreDirectory = true,
				FileName = "Replay"
			};
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				string fileName = ((FileDialog)val).FileName;
				string path = "replay.flv";
				File.Copy(Path.Combine(RegistryManager.Instance.ClientInstallDir, path), fileName);
			}
		}, new object[0]);
	}

	public void Stop()
	{
		if (sStreaming || sRecording)
		{
			SendBTVRequest("sessionswitch", null);
			sWasRecording = false;
		}
	}

	public void CloseBTV()
	{
		sWasRecording = false;
	}

	public void CheckNewFiltersAvailable()
	{
		SendBTVRequest("checknewfilters", null);
	}

	public static void SendBTVAsyncRequest(string request, Dictionary<string, string> data)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			Logger.Info("Sending btv async request");
			SendBTVRequest(request, data);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static string SendBTVRequest(string _1, Dictionary<string, string> _2)
	{
		return "";
	}

	public static void RestrictWindowResize(bool enable)
	{
		MainWindow activatedWindow = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			activatedWindow = BlueStacksUIUtils.DictWindows.Values.First();
		}
		((DispatcherObject)activatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			activatedWindow.RestrictWindowResize(enable);
		}, new object[0]);
	}

	public void RecordVideoOfApp()
	{
		if (StreamManager.Instance == null)
		{
			StreamManager.Instance = new StreamManager(BlueStacksUIUtils.DictWindows.Values.First());
		}
		StreamManager.GetStreamConfig(out var handle, out var pid);
		StreamManager.Instance.Init(handle, pid);
		StreamManager.Instance.SetHwnd(handle);
		StreamManager.Instance.EnableVideoRecording(enable: true);
		StreamManager.Instance.StartObs();
		StreamManager.Instance.StartRecordForVideo();
	}

	private void StopRecordVideo()
	{
		StreamManager.Instance.StopRecord();
	}

	internal void RecordStartedVideo()
	{
	}

	public void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
		}
	}

	~BTVManager()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
