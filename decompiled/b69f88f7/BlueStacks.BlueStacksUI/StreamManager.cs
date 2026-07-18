using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI;

public class StreamManager : IDisposable
{
	private class ObsCommand
	{
		public string mRequest;

		public Dictionary<string, string> mData;

		public string mResponseCallback;

		public string mFailureCallback;

		public int mPauseTime;

		public ObsCommand(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime)
		{
			mRequest = request;
			mData = data;
			mResponseCallback = responseCallback;
			mFailureCallback = failureCallback;
			mPauseTime = pauseTime;
		}
	}

	private static Queue<ObsCommand> mObsCommandQueue;

	private object mObsCommandQueueObject = new object();

	private object mObsSendRequestObject = new object();

	private object mInitOBSLock = new object();

	private EventWaitHandle mObsCommandEventHandle;

	private bool mIsStreamStarted;

	private string mFailureReason = "";

	private static int mMicVolume;

	internal static string mSelectedCamera;

	internal static string mSelectedMic = string.Empty;

	internal static bool mIsWebcamDisabled = true;

	internal static bool mIsMicDisabled = true;

	internal static Dictionary<string, string> mDictCameraDetails = new Dictionary<string, string>();

	internal static Dictionary<string, string> mDictMicDetails = new Dictionary<string, string>();

	internal static Dictionary<string, string> mDictLastCameraPosition = new Dictionary<string, string>();

	private static int mSystemVolume;

	private string mAppHandle = "";

	private string mAppPid = "";

	internal int mStreamWidth;

	internal int mStreamHeight;

	private object stoppingOBSLock = new object();

	private Browser mBrowser;

	private int heightDiff;

	private int widthDiff = 14;

	internal Fraction mAspectRatio = new Fraction(16L, 9L);

	private MainWindow mWindow;

	private string mLastVideoFilePath;

	private int mObsRetryCount = 2;

	private bool disposedValue;

	public static string ObsServerBaseURL { get; set; } = "http://localhost";

	public static int ObsServerPort { get; set; } = 2851;

	public static string DEFAULT_NETWORK { get; set; } = "twitch";

	public static bool DEFAULT_ENABLE_FILTER { get; set; } = true;

	public static bool DEFAULT_SQUARE_THEME { get; set; } = false;

	public static string DEFAULT_LAYOUT_THEME { get; set; } = null;

	public static bool sStopInitOBSQueue { get; set; } = false;

	public string mCallbackStreamStatus { get; set; }

	public string mCallbackAppInfo { get; set; }

	public bool mIsObsRunning { get; set; }

	public bool mIsInitCalled { get; set; }

	public bool mIsStreaming { get; set; }

	public bool mStoppingOBS { get; set; }

	public bool mIsReconnecting { get; set; }

	public string mNetwork { get; set; } = DEFAULT_NETWORK;

	public bool mSquareTheme { get; set; } = DEFAULT_SQUARE_THEME;

	public string mLayoutTheme { get; set; } = DEFAULT_LAYOUT_THEME;

	public string mLastCameraLayoutTheme { get; set; } = DEFAULT_LAYOUT_THEME;

	public bool mAppViewLayout { get; set; }

	public bool mEnableFilter { get; set; } = DEFAULT_ENABLE_FILTER;

	public static string CamStatus { get; set; }

	public bool mReplayBufferEnabled { get; set; }

	public bool mCLRBrowserRunning { get; set; }

	public string mCurrentFilterAppPkg { get; set; }

	public static StreamManager Instance { get; set; } = null;

	public bool isWindowCaptureActive { get; set; }

	public event EventHandler<CustomVolumeEventArgs> EventGetSystemVolume;

	public event EventHandler<CustomVolumeEventArgs> EventGetMicVolume;

	public event EventHandler<CustomVolumeEventArgs> EventGetCameraDetails;

	public event EventHandler<CustomVolumeEventArgs> EventGetMicDetails;

	public StreamManager(Browser browser)
	{
		Instance = this;
		mBrowser = browser;
		mReplayBufferEnabled = RegistryManager.Instance.ReplayBufferEnabled == 1;
		if (RegistryManager.Instance.CamStatus == 1)
		{
			CamStatus = "true";
		}
		else
		{
			CamStatus = "false";
		}
		mSelectedCamera = RegistryManager.Instance.SelectedCam;
		mObsCommandEventHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
		MainWindow mainWindow = null;
		if (BlueStacksUIUtils.DictWindows.Count > 0)
		{
			mainWindow = BlueStacksUIUtils.DictWindows.Values.First();
		}
		mWindow = mainWindow;
		CopySceneConfigFile(mWindow);
	}

	public StreamManager(MainWindow window)
	{
		Instance = this;
		mReplayBufferEnabled = true;
		if (RegistryManager.Instance.CamStatus == 1)
		{
			CamStatus = "true";
		}
		else
		{
			CamStatus = "false";
		}
		mSelectedCamera = RegistryManager.Instance.SelectedCam;
		mObsCommandEventHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
		mWindow = window;
		CopySceneConfigFile(mWindow, !RegistryManager.Instance.IsGameCaptureSupportedInMachine);
	}

	public void CopySceneConfigFile(MainWindow activatedWindow, bool forceWindowCaptureMode = false)
	{
		Logger.Debug("In Scene config file copy method with glmode: {0}", new object[1] { (activatedWindow != null) ? new int?(activatedWindow.EngineInstanceRegistry.GlRenderMode) : ((int?)null) });
		string path = Path.Combine(RegistryStrings.ObsDir, "sceneCollection");
		try
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (activatedWindow.EngineInstanceRegistry.GlRenderMode != 1 || forceWindowCaptureMode)
			{
				string sourceFileName = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_window.xconfig");
				string destFileName = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
				string destFileName2 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
				File.Copy(sourceFileName, destFileName, overwrite: true);
				File.Copy(sourceFileName, destFileName2, overwrite: true);
				isWindowCaptureActive = true;
			}
			else
			{
				string sourceFileName2 = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_graphics.xconfig");
				string destFileName3 = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
				string destFileName4 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
				File.Copy(sourceFileName2, destFileName3, overwrite: true);
				File.Copy(sourceFileName2, destFileName4, overwrite: true);
				isWindowCaptureActive = false;
			}
			Logger.Debug("Is window capture active..: {0}", new object[1] { isWindowCaptureActive });
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in obs scene config file : {0}", new object[1] { ex });
		}
	}

	internal void OrientationChangeHandler()
	{
		try
		{
			if (isWindowCaptureActive)
			{
				SetCaptureSize();
			}
			RefreshCapture();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in OrientationChangeHandler : " + ex.ToString());
		}
	}

	private void RefreshCapture()
	{
		SendObsRequest("refreshCapture", null, null, null, 0, waitForInit: false);
	}

	public void SendCurrentAppInfoAtTabChange()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (mBrowser != null && !string.IsNullOrEmpty(((WpfCefBrowser)mBrowser).getURL()))
			{
				AppTabButton selectedTab = mWindow.mTopBar.mAppTabButtons.SelectedTab;
				if (selectedTab != null)
				{
					selectedTab.mTabType.ToString();
					string appName = selectedTab.AppName;
					string packageName = selectedTab.PackageName;
					JObject val = new JObject();
					val.Add("type", JToken.op_Implicit("app"));
					val.Add("name", JToken.op_Implicit(appName));
					val.Add("data", JToken.op_Implicit(packageName));
					string text = ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]);
					string text2 = "getCurrentAppInfo('" + text.Replace("'", "&#39;").Replace("%27", "&#39;") + "')";
					((WpfCefBrowser)mBrowser).ExecuteJavaScript(text2, ((WpfCefBrowser)mBrowser).getURL(), 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in send current app status on tab changed " + ex.ToString());
		}
	}

	public void StartObs()
	{
		if (!mIsInitCalled)
		{
			InitObs();
		}
	}

	private void InitObs()
	{
		mIsInitCalled = true;
		Utils.KillCurrentOemProcessByName("HD-OBS", (string)null);
		if (!ProcessUtils.FindProcessByName("HD-OBS") && !sStopInitOBSQueue)
		{
			StartOBS();
		}
		if (sStopInitOBSQueue)
		{
			return;
		}
		try
		{
			string text = SendObsRequestInternal("ping", null);
			Logger.Info("response for ping is {0}", new object[1] { text });
			mIsObsRunning = true;
		}
		catch (Exception ex)
		{
			if (sStopInitOBSQueue || Instance == null)
			{
				return;
			}
			Logger.Error("Exception in InitObs. err: " + ex.ToString());
			Thread.Sleep(100);
			if (mObsRetryCount <= 0)
			{
				ShutDownForcefully();
				throw new Exception("Could not start OBS.");
			}
			mObsRetryCount--;
			InitObs();
		}
		mObsRetryCount = 2;
		Thread thread = new Thread((ThreadStart)delegate
		{
			ProcessObsCommandQueue();
		});
		thread.IsBackground = true;
		thread.Start();
		if (mReplayBufferEnabled)
		{
			SetReplayBufferSavePath();
		}
		GetParametersFromOBS();
		EnableSource("BlueStacks");
	}

	private void SetBackGroundImagePath()
	{
		EnableSource("BackGroundImage");
		string value = Path.Combine(RegistryStrings.ObsDir, "backgrounds\\Background3.jpg");
		Dictionary<string, string> data = new Dictionary<string, string> { { "path", value } };
		SendObsRequest("setBackground", data, null, null, 0);
	}

	public void SetSceneConfiguration(string layoutTheme)
	{
		mAppViewLayout = false;
		mLayoutTheme = layoutTheme;
		if (layoutTheme == null)
		{
			SendObsRequest("resettooriginalscene", null, null, null, 0, waitForInit: false);
			return;
		}
		SetCaptureSize();
		DisableSource("CLR Browser");
		try
		{
			JObject val = JObject.Parse(layoutTheme);
			Logger.Info(layoutTheme);
			bool flag = IsPortraitApp();
			if (val["isPortrait"] != null)
			{
				flag = val["isPortrait"].ToObject<bool>();
			}
			JObject val2 = null;
			val2 = ((!flag) ? JObject.Parse(((object)val["landscape"]).ToString()) : JObject.Parse(((object)val["portrait"]).ToString()));
			Dictionary<string, string> dictionary = null;
			if (val2["BlueStacksWebcam"] != null)
			{
				if (bool.TryParse(((object)val2["BlueStacksWebcam"][(object)"enableWebCam"]).ToString(), out var result) && result)
				{
					SetCameraPosition(Convert.ToInt32(((object)val2["BlueStacksWebcam"][(object)"x"]).ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(((object)val2["BlueStacksWebcam"][(object)"y"]).ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(((object)val2["BlueStacksWebcam"][(object)"width"]).ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(((object)val2["BlueStacksWebcam"][(object)"height"]).ToString(), CultureInfo.InvariantCulture), result ? 1 : 0);
				}
				else
				{
					DisableWebcamAndClearDictionary();
				}
			}
			if (val2["BlueStacks"] != null)
			{
				string value = ((object)val2["BlueStacks"][(object)"width"]).ToString();
				string text = ((object)val2["BlueStacks"][(object)"height"]).ToString();
				string value2 = ((object)val2["BlueStacks"][(object)"x"]).ToString();
				string value3 = ((object)val2["BlueStacks"][(object)"y"]).ToString();
				if (!isWindowCaptureActive)
				{
					value = text;
					if (val["name"] != null)
					{
						string a = ((object)val["name"]).ToString();
						if (string.Equals(a, "layout_2", StringComparison.InvariantCulture) || string.Equals(a, "layout_3", StringComparison.InvariantCulture))
						{
							value2 = "22";
							if (flag && string.Equals(a, "layout_3", StringComparison.InvariantCulture))
							{
								value2 = "0";
							}
						}
						else if (string.Equals(a, "layout_1", StringComparison.InvariantCulture))
						{
							value2 = "47";
						}
					}
				}
				SetFrontendPosition(0, 0, Convert.ToInt32(value, CultureInfo.InvariantCulture), Convert.ToInt32(text, CultureInfo.InvariantCulture));
				SetPosition(Convert.ToInt32(value2, CultureInfo.InvariantCulture), Convert.ToInt32(value3, CultureInfo.InvariantCulture));
				EnableSource("BlueStacks");
			}
			else
			{
				DisableSource("BlueStacks");
			}
			string text2 = ((object)val["order"]).ToString();
			string value4 = ((object)val["logo"]).ToString();
			text2 += ",BackGroundImage";
			string value5 = "watermarkFB,watermark,watermarkGif";
			if (val["allLogo"] != null)
			{
				value5 = ((object)val["allLogo"]).ToString();
			}
			dictionary = new Dictionary<string, string>
			{
				{ "order", text2 },
				{ "logo", value4 },
				{ "allLogo", value5 }
			};
			SendObsRequest("setorderandlogo", dictionary, null, null, 0, waitForInit: false);
		}
		catch (Exception ex)
		{
			Logger.Error("SetSceneConfiguration: Error {0}", new object[1] { ex });
		}
	}

	public static bool IsPortraitApp()
	{
		int frontendWidth = RegistryManager.Instance.FrontendWidth;
		int frontendHeight = RegistryManager.Instance.FrontendHeight;
		if (frontendWidth > frontendHeight)
		{
			return false;
		}
		return true;
	}

	private static void StartOBS()
	{
		Logger.Info("starting obs");
		string obsBinaryPath = RegistryStrings.ObsBinaryPath;
		string text = "-port " + RegistryManager.Instance.PartnerServerPort;
		if (SystemUtils.IsOs64Bit())
		{
			text += " -64bit";
		}
		if (!string.IsNullOrEmpty(Strings.OEMTag))
		{
			text = text + " -oem " + Strings.OEMTag;
		}
		ProcessUtils.GetProcessObject(obsBinaryPath, text, false).Start();
		Logger.Info("OBS started");
		ObsServerPort = RegistryManager.Instance.OBSServerPort;
	}

	public void SetHwnd(string handle)
	{
		Dictionary<string, string> data = new Dictionary<string, string> { { "hwnd", handle } };
		SendObsRequest("sethwnd", data, null, null, 0, waitForInit: false);
	}

	public void SetSavePath(string path = null)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("savepath", mLastVideoFilePath = path ?? Path.Combine(RegistryStrings.BtvDir, "stream.flv"));
		SendObsRequest("setsavepath", dictionary, null, "SetSaveFailed", 0, waitForInit: false);
	}

	private void SetSaveFailed()
	{
		Logger.Error("Exception in SetSaveFailed");
	}

	private void SetReplayBufferSavePath()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string value = Path.Combine(RegistryStrings.BtvDir, "replay.flv");
		dictionary.Add("savepath", value);
		SendObsRequest("setreplaybuffersavepath", dictionary, null, null, 0, waitForInit: false);
	}

	public static void SetStreamDimension(out int startX, out int startY, out int width, out int height)
	{
		try
		{
			BTVManager.GetStreamDimensionInfo(out startX, out startY, out width, out height);
		}
		catch (Exception ex)
		{
			Logger.Error("Got Exception in getting stream dimension... Err : " + ex.ToString());
			startX = (startY = (width = (height = 0)));
		}
	}

	public void SetFrontendPosition()
	{
		SetStreamDimension(out var startX, out var startY, out var width, out var height);
		int num = Utils.GetInt(RegistryManager.Instance.FrontendHeight, height);
		int frontendWidth = ((!isWindowCaptureActive) ? ((int)GetWidthFromHeight(num)) : RegistryManager.Instance.FrontendWidth);
		SetFrontendPosition(frontendWidth, num, startX, startY, width, height);
	}

	public void SetFrontendPosition(int frontendWidth, int frontendHeight)
	{
		SetStreamDimension(out var startX, out var startY, out var width, out var height);
		SetFrontendPosition(frontendWidth, frontendHeight, startX, startY, width, height);
	}

	public void SetFrontendPosition(int frontendWidth, int frontendHeight, int startX, int startY, int width, int height)
	{
		startY += (height - frontendHeight) / 2;
		startX += (width - frontendWidth) / 2;
		if (mEnableFilter)
		{
			int num = frontendWidth * 100 / (frontendHeight * 16 / 9);
			int startX2 = (100 - num) / 2;
			SetFrontendPosition(startX2, 0, num, 100);
			SetPosition(startX2, 0);
		}
		else
		{
			SetFrontendPosition(0, 0, 100, 100);
			if (!mSquareTheme)
			{
				SetPosition(0, 0);
			}
		}
		SetCaptureSize(startX, startY, frontendWidth, frontendHeight);
	}

	public void SetFrontendPosition(int startX, int startY, int width, int height)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
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
				"y",
				startY.ToString(CultureInfo.InvariantCulture)
			},
			{
				"x",
				startX.ToString(CultureInfo.InvariantCulture)
			},
			{ "source", "BlueStacks" }
		};
		SendObsRequest("setsourceposition", data, null, null, 0, waitForInit: false);
	}

	public void SetPosition(int startX, int startY)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{
				"y",
				startY.ToString(CultureInfo.InvariantCulture)
			},
			{
				"x",
				startX.ToString(CultureInfo.InvariantCulture)
			},
			{ "source", "BlueStacks" }
		};
		SendObsRequest("setposition", data, null, null, 0, waitForInit: false);
	}

	public void SetFrontEndCaptureSize(int width, int height)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{
				"width",
				width.ToString(CultureInfo.InvariantCulture)
			},
			{
				"height",
				height.ToString(CultureInfo.InvariantCulture)
			},
			{ "source", "BlueStacks" }
		};
		SendObsRequest("SetFrontendCaptureSize", data, null, null, 0, waitForInit: false);
	}

	public void SetCaptureSize()
	{
		SetStreamDimension(out var startX, out var startY, out var width, out var height);
		int num = Utils.GetInt(RegistryManager.Instance.FrontendHeight, height);
		startY += (height - num) / 2;
		int num2 = (isWindowCaptureActive ? RegistryManager.Instance.FrontendWidth : ((int)GetWidthFromHeight(num)));
		startX += (width - num2) / 2;
		Logger.Info("frontendWidth for set capture size : " + num2);
		Logger.Info("frontendHeight for set capture size : " + num);
		SetCaptureSize(startX, startY, num2, num);
	}

	private void SetCaptureSize(int startX, int startY, int width, int height)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Logger.Info("width for set capture size : " + width);
		Logger.Info("height for set capture size : " + height);
		dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("x", startX.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("y", startY.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("source", "BlueStacks");
		SendObsRequest("setcapturesize", dictionary, null, null, 0);
		if (isWindowCaptureActive)
		{
			SetFrontEndCaptureSize(RegistryManager.Instance.FrontendWidth, RegistryManager.Instance.FrontendHeight);
			if (IsPortraitApp())
			{
				SetPosition(35, 0);
			}
			else
			{
				SetPosition(0, 0);
			}
		}
	}

	public void ResetCLRBrowser(bool isSetFrontEndPosition = true)
	{
		DisableSource("CLR Browser");
		if (isSetFrontEndPosition)
		{
			SetFrontendPosition();
		}
		if (string.Compare(CamStatus, "true", StringComparison.OrdinalIgnoreCase) == 0)
		{
			EnableWebcamInternal("320", "240", "3");
		}
		else
		{
			DisableWebcamAndClearDictionary();
		}
		mCLRBrowserRunning = false;
		mCurrentFilterAppPkg = null;
	}

	internal void EnableVideoRecording(bool enable)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (enable)
		{
			dictionary.Add("Enable", "1");
		}
		else
		{
			dictionary.Add("Enable", "0");
		}
		SendObsRequest("EnableVideoRecording", dictionary, null, null, 0);
	}

	private void SetCameraPosition(int x, int y, int width, int height, int render)
	{
		Logger.Info("camera position width is : " + width + " and height is :" + height);
		mIsWebcamDisabled = false;
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "source", "BlueStacksWebcam" },
			{
				"width",
				width.ToString(CultureInfo.InvariantCulture)
			},
			{
				"height",
				height.ToString(CultureInfo.InvariantCulture)
			},
			{
				"x",
				x.ToString(CultureInfo.InvariantCulture)
			},
			{
				"y",
				y.ToString(CultureInfo.InvariantCulture)
			},
			{
				"render",
				render.ToString(CultureInfo.InvariantCulture)
			}
		};
		if (mDictCameraDetails.ContainsKey(mSelectedCamera))
		{
			dictionary.Add("camera", mDictCameraDetails[mSelectedCamera]);
		}
		else
		{
			dictionary["camera"] = string.Empty;
		}
		mDictLastCameraPosition = dictionary;
		SendObsRequest("setcameraposition", dictionary, "WebcamConfigured", null, 0, waitForInit: false);
	}

	internal void UpdateCameraPosition(string camName)
	{
		DisableWebcam();
		if (!string.IsNullOrEmpty(camName))
		{
			mSelectedCamera = camName;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			["source"] = "BlueStacksWebcam",
			["width"] = "100",
			["height"] = "100",
			["x"] = "0",
			["y"] = "0",
			["render"] = "1"
		};
		if (mDictCameraDetails.ContainsKey(mSelectedCamera))
		{
			dictionary["camera"] = mDictCameraDetails[mSelectedCamera];
		}
		else
		{
			dictionary["camera"] = string.Empty;
		}
		SendObsRequest("setcameraposition", dictionary, "WebcamConfigured", null, 0, waitForInit: false);
	}

	internal void ChangeCamera()
	{
		DisableWebcam();
		if (!mIsWebcamDisabled)
		{
			if (mDictLastCameraPosition.Count == 0)
			{
				mDictLastCameraPosition["source"] = "BlueStacksWebcam";
				mDictLastCameraPosition["width"] = "17";
				mDictLastCameraPosition["height"] = "23";
				mDictLastCameraPosition["x"] = "0";
				mDictLastCameraPosition["y"] = "77";
				mDictLastCameraPosition["render"] = "1";
			}
			if (mDictCameraDetails.ContainsKey(mSelectedCamera))
			{
				mDictLastCameraPosition["camera"] = mDictCameraDetails[mSelectedCamera];
			}
			else
			{
				mDictLastCameraPosition["camera"] = string.Empty;
			}
			SendObsRequest("setcameraposition", mDictLastCameraPosition, "WebcamConfigured", null, 0, waitForInit: false);
		}
	}

	public void SetClrBrowserConfig(string width, string height, string url)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "width", width },
			{ "height", height },
			{ "url", url }
		};
		SendObsRequest("setclrbrowserconfig", data, null, null, 0);
	}

	public void ObsErrorStatus(string erroReason)
	{
		mIsStreaming = false;
		mFailureReason = "Error starting stream : " + erroReason;
		SendStreamStatus(streaming: false, reconnecting: false);
	}

	public void ReportObsError(string errorReason)
	{
		try
		{
			Logger.Info("error reason in obs :" + errorReason);
			string eventType = "stream_interrupted_error";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
			{
				if (!mIsStreamStarted)
				{
					mIsStreamStarted = true;
					eventType = "obs_connected";
				}
				else
				{
					eventType = "stream_resumed";
				}
			}
			else if (!mIsStreamStarted)
			{
				eventType = "went_live_error";
			}
			if (string.Equals(errorReason, "OBSAlreadyRunning", StringComparison.InvariantCultureIgnoreCase))
			{
				eventType = "obs_already_running";
				ReportStreamStatsToCloud(eventType, errorReason);
				dictionary.Add("reason", eventType);
				ReportObsErrorHandler(eventType);
			}
			else if (string.Equals(errorReason, "capture_error", StringComparison.InvariantCultureIgnoreCase))
			{
				eventType = "capture_error";
				ReportStreamStatsToCloud(eventType, errorReason);
				sStopInitOBSQueue = true;
				dictionary.Add("reason", eventType);
				ReportObsErrorHandler(eventType);
			}
			else if (string.Equals(errorReason, "opengl_capture_error", StringComparison.InvariantCultureIgnoreCase))
			{
				eventType = "opengl_capture_error";
				ReportStreamStatsToCloud(eventType, errorReason);
				sStopInitOBSQueue = true;
				dictionary.Add("reason", eventType);
				ReportObsErrorHandler(eventType);
			}
			else if (string.Equals(errorReason, "AccessDenied", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "ConnectServerError", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "obs_error", StringComparison.InvariantCultureIgnoreCase))
			{
				errorReason = "Error starting stream : " + errorReason;
				ReportStreamStatsToCloud(eventType, errorReason);
				dictionary.Add("reason", "obs_error");
				ReportObsErrorHandler("obs_error");
			}
			else if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
			{
				ReportStreamStatsToCloud(eventType, errorReason);
			}
			else
			{
				errorReason = "Error starting stream : " + errorReason;
				ReportStreamStatsToCloud(eventType, errorReason);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to report obs error.. Err : " + ex.ToString());
		}
	}

	public void RestartOBSInWindowCaptureMode()
	{
		try
		{
			ShutDownForcefully();
			CopySceneConfigFile(mWindow, forceWindowCaptureMode: true);
			Logger.Info("restarting obs in window capture mode");
			if (File.Exists(mLastVideoFilePath))
			{
				File.Delete(mLastVideoFilePath);
			}
			mWindow.mCommonHandler.RecordVideoOfApp();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in restart obs in window capture mode: {0}", new object[1] { ex.ToString() });
		}
	}

	private static void ReportObsErrorHandler(string reason)
	{
		Logger.Error("Obs reported an error. " + reason);
		if (string.Equals(reason, "opengl_capture_error", StringComparison.OrdinalIgnoreCase))
		{
			RegistryManager.Instance.IsGameCaptureSupportedInMachine = false;
			Instance.RestartOBSInWindowCaptureMode();
			return;
		}
		StreamManager instance = Instance;
		if (instance != null)
		{
			((DispatcherObject)instance.mWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				Instance.mWindow.mCommonHandler.ShowErrorRecordingVideoPopup();
			}, new object[0]);
		}
	}

	private void ReportStreamStatsToCloud(string eventType, string reason)
	{
		Logger.Info("StreamStats eventType: {0}, reason: {1}", new object[2] { eventType, reason });
		new Dictionary<string, string>
		{
			{ "event_type", eventType },
			{ "error_code", reason },
			{
				"guid",
				RegistryManager.Instance.UserGuid
			},
			{ "streaming_platform", mNetwork },
			{
				"session_id",
				Stats.GetSessionId()
			},
			{ "prod_ver", "4.220.0.4001" },
			{
				"created_at",
				DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture)
			}
		};
	}

	internal void GetParametersFromOBS()
	{
		SendObsRequest("getmicvolume", null, "SetMicVolumeLocal", null, 0, waitForInit: false);
		SendObsRequest("getsystemvolume", null, "SetSystemVolumeLocal", null, 0, waitForInit: false);
	}

	private void SetMicLocal(string response)
	{
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		try
		{
			List<string> list = response.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> list2 = response.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			mSelectedMic = response.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[2];
			mDictMicDetails.Clear();
			mSelectedMic = Regex.Replace(mSelectedMic, "[^\\u0000-\\u007F]+", string.Empty);
			if (list2.Count == 0)
			{
				mSelectedMic = string.Empty;
				mIsMicDisabled = true;
			}
			else
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (!string.Equals(list2[i], "Default", StringComparison.InvariantCulture) && !string.Equals(list2[i], "Disable", StringComparison.InvariantCulture))
					{
						mDictMicDetails.Add(Regex.Replace(list[i], "[^\\u0000-\\u007F]+", string.Empty), list2[i]);
					}
				}
				if (mDictMicDetails.Count == 0)
				{
					mSelectedMic = string.Empty;
					mIsMicDisabled = true;
				}
				else if (!mDictMicDetails.ContainsKey(mSelectedMic))
				{
					mSelectedMic = mDictMicDetails.Keys.ToList()[0];
				}
			}
			this.EventGetMicDetails?.Invoke(this, new CustomVolumeEventArgs(mDictMicDetails, mSelectedMic));
		}
		catch (Exception ex)
		{
			Logger.Error("Error in SetMicLocal. response: " + response);
			Logger.Error(ex.ToString());
		}
	}

	private void SetMicVolumeLocal(string volumeResponse)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		try
		{
			mMicVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
			this.EventGetMicVolume?.Invoke(this, new CustomVolumeEventArgs(mMicVolume));
		}
		catch (Exception ex)
		{
			Logger.Error("Error in SetMicVolumeLocal. response: " + volumeResponse);
			Logger.Error(ex.ToString());
		}
	}

	private void SetCameraLocal(string cameraResponse)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		try
		{
			List<string> list = cameraResponse.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> list2 = cameraResponse.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			mDictCameraDetails.Clear();
			if (list2.Count == 0)
			{
				mSelectedCamera = string.Empty;
				mIsWebcamDisabled = true;
			}
			else
			{
				for (int i = 0; i < list2.Count; i++)
				{
					mDictCameraDetails.Add(Regex.Replace(list[i], "[^\\u0000-\\u007F]+", string.Empty).Trim(), list2[i]);
				}
				if (!mDictCameraDetails.ContainsKey(mSelectedCamera))
				{
					mSelectedCamera = Regex.Replace(cameraResponse.Split(new string[1] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[2], "[^\\u0000-\\u007F]+", string.Empty);
				}
			}
			this.EventGetCameraDetails?.Invoke(this, new CustomVolumeEventArgs(mDictCameraDetails, mSelectedCamera));
		}
		catch (Exception ex)
		{
			Logger.Error("Error in SetCameraLocal. response: " + cameraResponse);
			Logger.Error(ex.ToString());
		}
	}

	private void SetSystemVolumeLocal(string volumeResponse)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		try
		{
			mSystemVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
			this.EventGetSystemVolume?.Invoke(this, new CustomVolumeEventArgs(mSystemVolume));
		}
		catch (Exception ex)
		{
			Logger.Error("Error in SetSystemVolumeLocal. response: " + volumeResponse);
			Logger.Error(ex.ToString());
		}
	}

	public static void EnableWebcam(string _1, string _2, string _3)
	{
	}

	public void DisableSource(string source)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "source", source },
			{ "render", "0" }
		};
		SendObsRequest("setrender", data, null, null, 0, waitForInit: false);
	}

	public void EnableSource(string source)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "source", source },
			{ "render", "1" }
		};
		SendObsRequest("setrender", data, null, null, 0, waitForInit: false);
	}

	public void EnableWebcamInternal(string widthStr, string heightStr, string position)
	{
		int num = Convert.ToInt32(widthStr, CultureInfo.InvariantCulture);
		int num2 = Convert.ToInt32(heightStr, CultureInfo.InvariantCulture);
		if (mStreamHeight != 0 && mStreamWidth != 0)
		{
			num = num * 100 / mStreamWidth;
			num2 = num2 * 100 / mStreamHeight;
			new Dictionary<string, string>();
			int x = 0;
			int y = 0;
			if (string.Equals(position, "2", StringComparison.InvariantCultureIgnoreCase))
			{
				x = 100 - num;
			}
			else if (string.Equals(position, "3", StringComparison.InvariantCultureIgnoreCase))
			{
				y = 100 - num2;
			}
			else if (string.Equals(position, "4", StringComparison.InvariantCultureIgnoreCase))
			{
				x = 100 - num;
				y = 100 - num;
			}
			SetCameraPosition(x, y, num, num2, 1);
		}
	}

	public void DisableWebcamV2(string _)
	{
	}

	public void DisableWebcamAndClearDictionary()
	{
		mDictLastCameraPosition.Clear();
		mIsWebcamDisabled = true;
		DisableWebcam();
	}

	internal void DisableWebcam()
	{
		DisableSource("BlueStacksWebcam");
	}

	private void WebcamConfigured(string response)
	{
		try
		{
			JObject val = JObject.Parse(response);
			if (val["success"].ToObject<bool>())
			{
				CamStatus = val["webcam"].ToObject<bool>().ToString(CultureInfo.InvariantCulture);
				if (Convert.ToBoolean(CamStatus, CultureInfo.InvariantCulture))
				{
					RegistryManager.Instance.CamStatus = 1;
				}
				else
				{
					RegistryManager.Instance.CamStatus = 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Setting WebCamRegistry. response: {0} err : {1}", new object[2]
			{
				response,
				ex.ToString()
			});
		}
	}

	public void ResetFlvStream()
	{
		SendObsRequest("resetflvstream", null, null, null, 0);
	}

	public void SetSquareConfig(int startX, int startY, int width, int height)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Logger.Info("Window size: ({0}, {1})", new object[2] { width, height });
		widthDiff = 0;
		MiscUtils.GetStreamWidthAndHeight(width, height, out var _, out var height2);
		int num = (int)GetWidthFromHeight(height2);
		Logger.Info("Stream size: ({0}, {1})", new object[2] { num, height2 });
		width = num;
		height = height2;
		height = width;
		string value;
		int num2;
		if (height2 <= 720)
		{
			value = "main";
			num2 = 2500;
		}
		else
		{
			value = "high";
			num2 = 3500;
		}
		float num3 = 1f;
		Logger.Info("x : " + startX);
		Logger.Info("y : " + startY);
		Logger.Info("width : " + width);
		Logger.Info("height : " + height);
		dictionary.Clear();
		dictionary.Add("startX", startX.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("startY", startY.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("x264Profile", value);
		dictionary.Add("maxBitrate", num2.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("downscale", num3.ToString(CultureInfo.InvariantCulture));
		mStreamWidth = width;
		mStreamHeight = height;
		SendObsRequest("setconfig", dictionary, null, null, 0, waitForInit: false);
	}

	public void SetConfig(int startX, int startY, int width, int height)
	{
		if (mSquareTheme)
		{
			SetSquareConfig(startX, startY, width, height);
		}
		else
		{
			SetDefaultConfig(startX, startY, width, height);
		}
	}

	public void RestartRecord()
	{
		StopRecord(immediate: true);
		StartRecord();
	}

	public void SetDefaultConfig(int startX, int startY, int width, int height)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Logger.Info("Window size: ({0}, {1})", new object[2] { width, height });
		widthDiff = 0;
		MiscUtils.GetStreamWidthAndHeight(width, height, out var _, out var height2);
		int num = (int)GetWidthFromHeight(height2);
		width = num;
		height = height2;
		_ = width * 9 / 16;
		Logger.Info("Stream size: ({0}, {1})", new object[2] { num, height2 });
		string value;
		int num2;
		switch (height2)
		{
		case 540:
			value = "main";
			num2 = 1200;
			break;
		case 720:
			value = "main";
			num2 = 2500;
			break;
		default:
			value = "high";
			num2 = 3500;
			break;
		}
		float num3 = (float)height / (float)height2;
		Logger.Info("x : " + startX);
		Logger.Info("y : " + startY);
		Logger.Info("width : " + width);
		Logger.Info("height : " + height);
		dictionary.Clear();
		dictionary.Add("startX", startX.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("startY", startY.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("x264Profile", value);
		dictionary.Add("maxBitrate", num2.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("downscale", num3.ToString(CultureInfo.InvariantCulture));
		mStreamWidth = width;
		mStreamHeight = height;
		SendObsRequest("setconfig", dictionary, null, null, 0, waitForInit: false);
	}

	public void StartStream(string key, string location, string callbackStreamStatus, string callbackAppInfo)
	{
		string service = "1";
		mCallbackStreamStatus = callbackStreamStatus;
		if (mCallbackAppInfo == null)
		{
			mCallbackAppInfo = callbackAppInfo;
		}
		SetStreamSettings(service, key, location);
		SendObsRequest("startstream", null, "StreamStarted", null, 0);
	}

	public void StartStream(string jsonString, string callbackStreamStatus, string callbackAppInfo)
	{
		Logger.Info(jsonString);
		JObject obj = JObject.Parse(jsonString);
		string playPath = ((object)obj["key"]).ToString();
		string service = ((object)obj["service"]).ToString();
		string url = ((object)obj["streamUrl"]).ToString();
		((object)obj["server"]).ToString();
		mCallbackStreamStatus = callbackStreamStatus;
		mCallbackAppInfo = callbackAppInfo;
		SetStreamSettings(service, playPath, url);
		SendObsRequest("startstream", null, "StreamStarted", null, 0);
	}

	public void StopStream()
	{
		SendObsRequest("stopstream", null, "StreamStopped", null, 0);
	}

	private void SendStatus(string path, Dictionary<string, string> data)
	{
		try
		{
			if (string.Equals(path, "streamstarted", StringComparison.InvariantCulture))
			{
				BTVManager.Instance.StreamStarted();
			}
			else if (string.Equals(path, "streamstopped", StringComparison.InvariantCulture))
			{
				BTVManager.Instance.StreamStopped();
			}
			else if (string.Equals(path, "recordstarted", StringComparison.InvariantCulture))
			{
				BTVManager.Instance.RecordStarted();
			}
			else if (string.Equals(path, "recordstopped", StringComparison.InvariantCulture))
			{
				BTVManager.Instance.RecordStopped();
				CommonHandlers.sIsRecordingVideo = false;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
				{
					BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStopped();
				}
				CommonHandlers.sRecordingInstance = "";
				ShutDownForcefully();
			}
			else if (string.Equals(path, "streamstatus", StringComparison.InvariantCulture))
			{
				if (string.Compare(data["isstreaming"], "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					BTVManager.Instance.sStreaming = true;
				}
				else
				{
					BTVManager.Instance.sStreaming = false;
				}
			}
			else if (string.Equals(path, "replaybuffersaved", StringComparison.InvariantCulture))
			{
				BTVManager.ReplayBufferSaved();
			}
			else if (string.Equals(path, "RecordStartedVideo", StringComparison.InvariantCulture) && BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
			{
				BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStarted();
			}
			Logger.Info("Successfully sent status for {0}", new object[1] { path });
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send post request for {0}... Err : {1}", new object[2]
			{
				path,
				ex.ToString()
			});
		}
	}

	public void StartRecordForVideo()
	{
		if (mIsObsRunning)
		{
			SetStreamDimension(out var startX, out var startY, out var width, out var height);
			SetConfig(startX, startY, width, height);
			SetSceneConfiguration(mLayoutTheme);
			ResetCLRBrowser();
		}
		SendObsRequest("startrecord", null, "RecordStartedVideo", null, 0);
	}

	public void StartRecord()
	{
		StartRecord(mNetwork, mEnableFilter, mSquareTheme, mLayoutTheme, mCallbackAppInfo);
	}

	public void StartRecordInit(string network, bool enableFilter, bool squareTheme, string layoutTheme, string callbackAppInfo)
	{
		mNetwork = network;
		mEnableFilter = enableFilter;
		mSquareTheme = squareTheme;
		mLayoutTheme = layoutTheme;
		mCallbackAppInfo = callbackAppInfo;
	}

	public void StartRecord(string network, bool enableFilter, bool squareTheme, string layoutTheme, string callbackAppInfo)
	{
		lock (stoppingOBSLock)
		{
			mEnableFilter = enableFilter;
			mSquareTheme = squareTheme;
			mCallbackAppInfo = callbackAppInfo;
			SendNetworkName(network);
			if (layoutTheme != null)
			{
				mLayoutTheme = Utils.GetString(RegistryManager.Instance.LayoutTheme, layoutTheme);
				mLastCameraLayoutTheme = RegistryManager.Instance.LastCameraLayoutTheme;
				mAppViewLayout = RegistryManager.Instance.AppViewLayout == 1;
				if (string.IsNullOrEmpty(mLastCameraLayoutTheme))
				{
					mLastCameraLayoutTheme = layoutTheme;
				}
			}
			else
			{
				mLayoutTheme = layoutTheme;
			}
			mNetwork = network;
			if (mIsObsRunning)
			{
				SetStreamDimension(out var startX, out var startY, out var width, out var height);
				SetConfig(startX, startY, width, height);
				SetSceneConfiguration(mLayoutTheme);
			}
			EnableVideoRecording(enable: false);
			SendObsRequest("startrecord", null, "RecordStarted", null, 0);
		}
	}

	public void StopRecord()
	{
		StopRecord(immediate: false);
	}

	public void StopRecord(bool immediate)
	{
		Dictionary<string, string> data = new Dictionary<string, string> { 
		{
			"immediate",
			immediate ? "1" : "0"
		} };
		SendObsRequest("stoprecord", data, "RecordStopped", null, 0);
	}

	public void SendAppInfo(string type, string name, string data)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		if (mCallbackAppInfo != null)
		{
			JObject val = new JObject();
			val.Add("type", JToken.op_Implicit(type));
			val.Add("name", JToken.op_Implicit(name));
			val.Add("data", JToken.op_Implicit(data));
			JObject val2 = val;
			object[] args = new object[1] { ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]) };
			mBrowser.CallJs(mCallbackAppInfo, args);
		}
	}

	public static string GetStreamConfig()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		string streamName = RegistryManager.Instance.StreamName;
		string serverLocation = RegistryManager.Instance.ServerLocation;
		JObject val = new JObject();
		val.Add("streamName", JToken.op_Implicit(streamName));
		val.Add("camStatus", JToken.op_Implicit(Convert.ToBoolean(CamStatus, CultureInfo.InvariantCulture)));
		val.Add("micVolume", JToken.op_Implicit(mMicVolume));
		val.Add("systemVolume", JToken.op_Implicit(mSystemVolume));
		val.Add("serverLocation", JToken.op_Implicit(serverLocation));
		JObject val2 = val;
		Logger.Info("GetStreamConfig: " + ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
		return ((object)val2).ToString();
	}

	private void StreamStarted(string _)
	{
		SendStatus("streamstarted", null);
		mIsStreaming = true;
	}

	private void StreamStopped(string _)
	{
		SendStatus("streamstopped", null);
		mIsStreaming = false;
		mIsStreamStarted = false;
		SendObsRequest("close", null, null, null, 0);
		Thread thread = new Thread((ThreadStart)delegate
		{
			KillOBS();
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void killOBSForcelly()
	{
		Utils.KillCurrentOemProcessByName("HD-OBS", (string)null);
	}

	public void KillOBS()
	{
		if (mStoppingOBS)
		{
			return;
		}
		lock (stoppingOBSLock)
		{
			mStoppingOBS = true;
			try
			{
				int num = 0;
				int num2 = 20;
				while (num < num2 && Process.GetProcessesByName("HD-OBS").Length != 0)
				{
					num++;
					if (num < num2)
					{
						Logger.Info("Waiting for HD-OBS to quit gracefully, retry: {0}", new object[1] { num });
						Thread.Sleep(200);
					}
				}
				if (num >= num2)
				{
					Utils.KillCurrentOemProcessByName("HD-OBS", (string)null);
				}
				StartOBS();
			}
			catch (Exception ex)
			{
				Logger.Info("Failed to kill HD-OBS.exe...Err : " + ex.ToString());
			}
			mStoppingOBS = false;
		}
	}

	private void RecordStarted(string _)
	{
		SendStatus("recordstarted", null);
	}

	private void RecordStopped(string _)
	{
		SendStatus("recordstopped", null);
	}

	private void RecordStartedVideo(string _)
	{
		SendStatus("RecordStartedVideo", null);
	}

	public void StartReplayBuffer()
	{
		SendObsRequest("startreplaybuffer", null, null, null, 0);
	}

	public void StopReplayBuffer()
	{
		SendObsRequest("stopreplaybuffer", null, null, null, 2000);
	}

	private void SetStreamSettings(string service, string playPath, string url)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "service", service },
			{ "playPath", playPath },
			{ "url", url }
		};
		SendObsRequest("setstreamsettings", data, null, null, 0);
	}

	public void SetSystemVolume(string level)
	{
		mSystemVolume = Convert.ToInt32(level, CultureInfo.InvariantCulture);
		Dictionary<string, string> data = new Dictionary<string, string> { { "volume", level } };
		SendObsRequest("setsystemvolume", data, null, null, 0);
	}

	internal void SetMic(string micName)
	{
		micName = mDictMicDetails[micName];
		Dictionary<string, string> data = new Dictionary<string, string> { { "micId", micName } };
		SendObsRequest("setmic", data, null, null, 0);
	}

	public void SetMicVolume(string level)
	{
		mMicVolume = Convert.ToInt32(level, CultureInfo.InvariantCulture);
		if (mMicVolume > 0)
		{
			mIsMicDisabled = false;
		}
		Dictionary<string, string> data = new Dictionary<string, string> { { "volume", level } };
		SendObsRequest("setmicvolume", data, null, null, 0);
	}

	private void StartPollingOBS()
	{
		while (mIsObsRunning)
		{
			try
			{
				JObject val = JObject.Parse(Regex.Replace(SendObsRequestInternal("getstatus", null), "\\r\\n?|\\n", ""));
				bool flag = val["streaming"].ToObject<bool>();
				bool reconnecting = val["reconnecting"].ToObject<bool>();
				if (!flag)
				{
					try
					{
						mFailureReason = ((object)val["reason"]).ToString();
					}
					catch
					{
					}
				}
				if (flag != mIsStreaming)
				{
					SendStreamStatus(flag, reconnecting);
				}
				mIsStreaming = flag;
				mIsReconnecting = reconnecting;
				Dictionary<string, string> data = new Dictionary<string, string> { 
				{
					"isstreaming",
					mIsStreaming.ToString(CultureInfo.InvariantCulture)
				} };
				SendStatus("streamstatus", data);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in StartPollingOBS err : " + ex.ToString());
				if (!ProcessUtils.FindProcessByName("HD-OBS"))
				{
					mIsObsRunning = false;
					mIsStreaming = false;
					mIsReconnecting = false;
					mCLRBrowserRunning = false;
					mIsStreamStarted = false;
					if (!mStoppingOBS)
					{
						UpdateFailureReason();
					}
					SendStreamStatus(streaming: false, reconnecting: false);
					InitObs();
					mStoppingOBS = false;
					break;
				}
			}
			Thread.Sleep(5000);
		}
	}

	private void UpdateFailureReason()
	{
		if (!string.IsNullOrEmpty(mFailureReason))
		{
			return;
		}
		string text = "";
		string format = "yyyy-MM-dd-HHmm-ss";
		DateTime dateTime = DateTime.MinValue;
		string[] files = Directory.GetFiles(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\"));
		for (int i = 0; i < files.Length; i++)
		{
			text = Path.GetFileNameWithoutExtension(files[i]);
			if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) && dateTime < result)
			{
				dateTime = result;
			}
		}
		if (!dateTime.Equals(DateTime.MinValue))
		{
			text = File.ReadAllLines(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\") + dateTime.ToString("yyyy-MM-dd-HHmm-ss", CultureInfo.InvariantCulture) + ".log").Last();
		}
		mFailureReason = "OBS crashed: " + text;
	}

	private static void SendNetworkName(string network)
	{
		try
		{
			BTVManager.sNetwork = network;
			RegistryManager.Instance.BtvNetwork = network;
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send network name... Err : " + ex.ToString());
		}
	}

	private void SendStreamStatus(bool streaming, bool reconnecting)
	{
		Logger.Info("Sending stream status with data :: streaming : " + streaming + ", reconnecting : " + reconnecting + ", obsRunning : " + mIsObsRunning + ", failureReason : " + mFailureReason);
		try
		{
			new Dictionary<string, string>
			{
				{
					"obs",
					mIsObsRunning.ToString(CultureInfo.InvariantCulture)
				},
				{
					"streaming",
					streaming.ToString(CultureInfo.InvariantCulture)
				},
				{
					"reconnecting",
					reconnecting.ToString(CultureInfo.InvariantCulture)
				},
				{
					"reason",
					mFailureReason.ToString(CultureInfo.InvariantCulture)
				}
			};
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send stream status... Err : " + ex.ToString());
		}
	}

	public void ResizeStream(string width, string height)
	{
		if (mObsCommandQueue != null)
		{
			Dictionary<string, string> data = new Dictionary<string, string>
			{
				{ "width", width },
				{ "height", height }
			};
			SendObsRequest("windowresized", data, null, null, 0);
		}
	}

	public void ShowObs()
	{
		SendObsRequest("show", null, null, null, 0);
	}

	public void HideObs()
	{
		SendObsRequest("hide", null, null, null, 0);
	}

	public void MoveWebcam(string horizontal, string vertical)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "horizontal", horizontal },
			{ "vertical", vertical }
		};
		SendObsRequest("movewebcam", data, null, null, 0);
	}

	public void Shutdown()
	{
		if (mIsStreaming)
		{
			StopStream();
		}
		if (mObsCommandQueue != null)
		{
			mIsObsRunning = false;
			mIsStreamStarted = false;
			SendObsRequest("close", null, "CloseSuccess", "CloseFailed", 0);
		}
	}

	public static void CloseSuccess(string _)
	{
		Instance = null;
	}

	public static void CloseFailed()
	{
		Utils.KillCurrentOemProcessByName("HD-OBS", (string)null);
		Instance = null;
	}

	public static void StopOBS()
	{
		Instance.mStoppingOBS = true;
		sStopInitOBSQueue = true;
		Instance.Shutdown();
		int num = 0;
		while (Process.GetProcessesByName("HD-OBS").Length != 0 && num < 20)
		{
			Thread.Sleep(500);
			num++;
		}
		if (num == 20)
		{
			Logger.Info("Killing hd-obs as normal close failed");
			CloseFailed();
		}
	}

	public void SaveReplayBuffer()
	{
		SendObsRequest("savereplaybuffer", null, null, null, 0);
	}

	public void ReplayBufferSaved()
	{
		SendStatus("replaybuffersaved", null);
	}

	public void SendObsRequest(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime)
	{
		SendObsRequest(request, data, responseCallback, failureCallback, pauseTime, waitForInit: true);
	}

	public void SendObsRequest(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime, bool waitForInit)
	{
		Logger.Info("got obs request: " + request);
		if (data != null && !data.ContainsKey("randomVal"))
		{
			data.Add("randomVal", "0");
		}
		Thread thread = new Thread((ThreadStart)delegate
		{
			ObsCommand item = new ObsCommand(request, data, responseCallback, failureCallback, pauseTime);
			lock (mInitOBSLock)
			{
				if (!mIsInitCalled)
				{
					InitObs();
				}
			}
			if (mObsCommandQueue == null)
			{
				mObsCommandQueue = new Queue<ObsCommand>();
			}
			if (waitForInit)
			{
				lock (mInitOBSLock)
				{
					lock (mObsCommandQueueObject)
					{
						mObsCommandQueue.Enqueue(item);
						mObsCommandEventHandle.Set();
						return;
					}
				}
			}
			lock (mObsCommandQueueObject)
			{
				mObsCommandQueue.Enqueue(item);
				mObsCommandEventHandle.Set();
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private string SendObsRequestInternal(string request, Dictionary<string, string> data)
	{
		Logger.Info("waiting to send request: " + request);
		lock (mObsSendRequestObject)
		{
			string result = string.Empty;
			if (mIsObsRunning)
			{
				result = BstHttpClient.Post(string.Format(CultureInfo.InvariantCulture, "{0}:{1}/{2}", new object[3] { ObsServerBaseURL, ObsServerPort, request }), data, (Dictionary<string, string>)null, false, "Android", 0, 1, 0, false, "bgp64");
			}
			return result;
		}
	}

	private void ProcessObsCommandQueue()
	{
		while (mIsObsRunning)
		{
			mObsCommandEventHandle.WaitOne();
			while (mObsCommandQueue.Count != 0)
			{
				ObsCommand obsCommand;
				lock (mObsCommandQueueObject)
				{
					if (mObsCommandQueue.Count == 0)
					{
						break;
					}
					obsCommand = mObsCommandQueue.Dequeue();
					goto IL_0048;
				}
				IL_0048:
				string empty = string.Empty;
				try
				{
					empty = SendObsRequestInternal(obsCommand.mRequest, obsCommand.mData);
					Logger.Info("Got response {0} for {1}", new object[2] { empty, obsCommand.mRequest });
					if (obsCommand.mResponseCallback != null)
					{
						GetType().GetMethod(obsCommand.mResponseCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(this, new object[1] { empty });
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception when sending " + obsCommand.mRequest);
					Logger.Error(ex.ToString());
					try
					{
						if (obsCommand.mFailureCallback != null)
						{
							GetType().GetMethod(obsCommand.mFailureCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(this, new object[0]);
						}
					}
					catch (Exception ex2)
					{
						Logger.Error("Error in failure call back for call {} and error {1}", new object[2] { obsCommand.mFailureCallback, ex2 });
					}
				}
				Thread.Sleep(obsCommand.mPauseTime);
			}
		}
	}

	public void Init(string appHandle, string pid)
	{
		Logger.Info("App Handle : {0} and Process Id : {1}", new object[2] { appHandle, pid });
		if (string.IsNullOrEmpty(mAppHandle) && string.IsNullOrEmpty(mAppPid))
		{
			mAppHandle = appHandle;
			mAppPid = pid;
		}
	}

	private double GetWidthFromHeight(double height)
	{
		return (height - (double)heightDiff) * mAspectRatio.DoubleValue + (double)widthDiff;
	}

	public static void GetStreamConfig(out string handle, out string pid)
	{
		try
		{
			MainWindow activatedWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				activatedWindow = BlueStacksUIUtils.DictWindows.Values.First();
			}
			handle = activatedWindow.mFrontendHandler.mFrontendHandle.ToString();
			((DispatcherObject)activatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				activatedWindow.RestrictWindowResize(enable: true);
			}, new object[0]);
			Process currentProcess = Process.GetCurrentProcess();
			pid = currentProcess.Id.ToString(CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to get window handle and process id... Err : " + ex.ToString());
			handle = (pid = null);
		}
	}

	public void ShutDownForcefully()
	{
		try
		{
			killOBSForcelly();
			mIsObsRunning = false;
			Instance.mStoppingOBS = true;
			mObsCommandQueue.Clear();
			Instance = null;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in shutdown obs : {0}", new object[1] { ex });
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			Browser browser = mBrowser;
			if (browser != null)
			{
				((WpfCefBrowser)browser).Dispose();
			}
			mObsCommandEventHandle?.Close();
			disposedValue = true;
		}
	}

	~StreamManager()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
