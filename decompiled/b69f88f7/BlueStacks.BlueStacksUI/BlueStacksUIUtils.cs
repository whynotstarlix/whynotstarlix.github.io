using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class BlueStacksUIUtils : IDisposable
{
	private MainWindow ParentWindow;

	private int mCurrentVolumeLevel = 33;

	internal static object mLaunchPlaySyncObj;

	internal static string sLoggedInImageName;

	internal static string sPremiumUserImageName;

	internal static string sUserAccountPackageName;

	internal static string sUserAccountActivityName;

	internal static string sAndroidSettingsPackageName;

	internal static string sAndroidAccountSettingsActivityName;

	internal static bool sStopStatSendingThread;

	internal System.Timers.Timer sBootCheckTimer = new System.Timers.Timer(360000.0);

	internal static List<string> lstCreatingWindows;

	internal static Dictionary<string, MainWindow> DictWindows;

	internal static bool sIsSynchronizationActive;

	internal static List<string> sSelectedInstancesForSync;

	public static MainWindow LastActivatedWindow;

	public static MainWindow ActivatedWindow;

	public static Dictionary<string, List<EventHandler>> BootEventsForMIManager;

	public static List<string> sSyncInvolvedInstances;

	private static bool? isOglSupported;

	private bool disposedValue;

	public int CurrentVolumeLevel
	{
		get
		{
			return mCurrentVolumeLevel;
		}
		private set
		{
			if (value <= 0)
			{
				mCurrentVolumeLevel = 0;
			}
			else if (value >= 100)
			{
				mCurrentVolumeLevel = 100;
			}
			else
			{
				mCurrentVolumeLevel = value;
			}
			ParentWindow.EngineInstanceRegistry.Volume = mCurrentVolumeLevel;
			ParentWindow.mCommonHandler.OnVolumeChanged(mCurrentVolumeLevel);
		}
	}

	internal static bool IsAlphabet(char c)
	{
		if (c < 'A' || c > 'Z')
		{
			if (c >= 'a')
			{
				return c <= 'z';
			}
			return false;
		}
		return true;
	}

	internal BlueStacksUIUtils(MainWindow window)
	{
		ParentWindow = window;
		mCurrentVolumeLevel = ParentWindow.EngineInstanceRegistry.Volume;
	}

	internal static void CloseContainerWindow(FrameworkElement control)
	{
		FrameworkElement val = control;
		while (val != null && !(val is ContainerWindow))
		{
			DependencyObject parent = val.Parent;
			val = (FrameworkElement)(object)((parent is FrameworkElement) ? parent : null);
		}
		if (val != null)
		{
			((Window)(val as ContainerWindow)).Close();
		}
	}

	internal static void RefreshKeyMap(string packageName)
	{
		foreach (KeyValuePair<string, MainWindow> item in DictWindows)
		{
			try
			{
				if (!string.Equals(item.Value.mTopBar.mAppTabButtons.SelectedTab.PackageName, packageName, StringComparison.InvariantCulture))
				{
					continue;
				}
				item.Value.mFrontendHandler.RefreshKeyMap(packageName);
				if (!RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					continue;
				}
				KMManager.LoadIMActions(item.Value, packageName);
				if (KMManager.CanvasWindow != null && ((UIElement)KMManager.CanvasWindow).IsVisible && ((object)((Window)KMManager.CanvasWindow).Owner).GetHashCode() == ((object)item.Value).GetHashCode())
				{
					continue;
				}
				Dispatcher.CurrentDispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					if (KMManager.dictOverlayWindow.ContainsKey(item.Value) && LastActivatedWindow != item.Value)
					{
						KMManager.dictOverlayWindow[item.Value].Init();
					}
				}, new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString() + Environment.NewLine + "Exception refreshing mapping of package : " + packageName + " for instance : " + item.Value.mVmName);
			}
		}
	}

	public static bool IsModal(Window window)
	{
		return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
	}

	private static void CloseWindows(Window win)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		for (int num = win.OwnedWindows.Count - 1; num >= 0; num--)
		{
			CloseWindows(win.OwnedWindows[num]);
			if (win.OwnedWindows[num] != null && ((FrameworkElement)win.OwnedWindows[num]).IsLoaded && (int)((UIElement)win.OwnedWindows[num]).Visibility == 0)
			{
				if (IsModal(win.OwnedWindows[num]))
				{
					win.OwnedWindows[num].Close();
				}
				else
				{
					((UIElement)win.OwnedWindows[num]).Visibility = (Visibility)1;
				}
			}
		}
	}

	internal static void HideUnhideBlueStacks(bool isHide)
	{
		foreach (MainWindow value in DictWindows.Values)
		{
			if (!value.mIsMinimizedThroughCloseButton)
			{
				HideUnhideParentWindow(isHide, value);
			}
		}
	}

	internal static void HideUnhideParentWindow(bool isHide, MainWindow window)
	{
		if (isHide)
		{
			((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				CloseWindows((Window)(object)window);
				((Window)window).WindowState = (WindowState)1;
				((Window)window).Hide();
				((Window)window).ShowInTaskbar = false;
			}, new object[0]);
			return;
		}
		((Window)window).ShowInTaskbar = true;
		((Window)window).ShowActivated = true;
		((Window)window).Show();
		if (window.mIsFullScreen)
		{
			((Window)window).WindowState = (WindowState)2;
		}
		else
		{
			((Window)window).WindowState = (WindowState)0;
		}
		if (!((Window)window).Topmost)
		{
			((Window)window).Topmost = true;
			ThreadPool.QueueUserWorkItem(delegate
			{
				((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((Window)window).Topmost = false;
				}, new object[0]);
			});
		}
		if (RegistryManager.Instance.ShowKeyControlsOverlay)
		{
			KMManager.ShowOverlayWindow(window, isShow: true);
		}
	}

	public static void SetWindowTaskbarIcon(MainWindow window)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		try
		{
			Bitmap val = new Bitmap(RegistryStrings.ProductIconCompletePath);
			try
			{
				new Uri(RegistryStrings.ProductIconCompletePath);
				if (((Dictionary<string, GenericNotificationItem>)(object)GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, window.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification))).Count > 0 && window.IsInNotificationMode)
				{
					if (window.DummyWindow == null)
					{
						MainWindow mainWindow = window;
						DummyTaskbarWindow dummyTaskbarWindow = new DummyTaskbarWindow(window);
						((Window)dummyTaskbarWindow).Icon = (ImageSource)new BitmapImage(new Uri(Path.Combine(RegistryManager.Instance.ClientInstallDir, Path.Combine("Assets", "ProductLogo.ico"))));
						((Window)dummyTaskbarWindow).Title = Strings.ProductDisplayName;
						dummyTaskbarWindow.TaskbarThumbnailPath = Path.Combine(CustomPictureBox.AssetsDir, "PreviewThumbnail.png");
						((Window)dummyTaskbarWindow).WindowState = (WindowState)1;
						mainWindow.DummyWindow = dummyTaskbarWindow;
						((Window)window.DummyWindow).StateChanged -= DummyWindow_StateChanged;
						((Window)window.DummyWindow).StateChanged += DummyWindow_StateChanged;
						((Window)window.DummyWindow).Show();
					}
					AddIconOverlay((Window)(object)window.DummyWindow, val, window.mVmName);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while setting taskbar icon " + ex);
		}
	}

	private static void DummyWindow_StateChanged(object sender, EventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		DummyTaskbarWindow obj = sender as DummyTaskbarWindow;
		if (obj == null || (int)((Window)obj).WindowState != 1)
		{
			HideUnhideParentWindow(isHide: false, (sender as DummyTaskbarWindow).ParentWindow);
			Stats.SendCommonClientStatsAsync("notification_mode", "taskbar_bluestacksicon_clicked", "Android", "", "", "");
			DummyTaskbarWindow obj2 = sender as DummyTaskbarWindow;
			if (obj2 != null)
			{
				((Window)obj2).Close();
			}
		}
	}

	public static void AddIconOverlay(Window window, Bitmap originalIcon, string vmName)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Expected O, but got Unknown
		try
		{
			SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
			string text = ((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count.ToString(CultureInfo.InvariantCulture);
			if (((Dictionary<string, GenericNotificationItem>)(object)notificationItems).Count > 99)
			{
				text = "99+";
			}
			Bitmap val = new Bitmap(256, 256);
			try
			{
				Graphics val2 = Graphics.FromImage((Image)(object)val);
				try
				{
					val2.SmoothingMode = (SmoothingMode)4;
					int num = (text.Length + 1) * 45 + 10;
					int num2 = 256 - num + 5;
					if (num < 120)
					{
						num2 = 206 - num / 2;
						num = 120;
					}
					Rectangle rectangle = new Rectangle(256 - num, 111, num, 120);
					int num3 = 120;
					Size size = new Size(num3, num3);
					Rectangle rectangle2 = new Rectangle(rectangle.Location, size);
					GraphicsPath val3 = new GraphicsPath();
					try
					{
						val3.AddArc(rectangle2, 180f, 90f);
						rectangle2.X = rectangle.Right - num3;
						val3.AddArc(rectangle2, 270f, 90f);
						rectangle2.Y = rectangle.Bottom - num3;
						val3.AddArc(rectangle2, 0f, 90f);
						rectangle2.X = rectangle.Left;
						val3.AddArc(rectangle2, 90f, 90f);
						val3.CloseFigure();
						val2.FillPath(Brushes.OrangeRed, val3);
						Image val4 = (Image)(object)val;
						Bitmap val5 = new Bitmap(256, 256);
						try
						{
							Graphics val6 = Graphics.FromImage((Image)(object)val5);
							Rectangle rectangle3 = new Rectangle(0, 0, 256, 256);
							val6.DrawImage((Image)(object)originalIcon, rectangle3);
							val6.DrawImage(val4, new Point(0, 0));
							Font val7 = new Font("Arial", (float)(70.0 / MainWindow.sScalingFactor), (FontStyle)0);
							try
							{
								val6.DrawString(text, val7, Brushes.White, (float)num2, 117f);
								val6.Save();
								window.Icon = (ImageSource)(object)Imaging.CreateBitmapSourceFromHIcon(val5.GetHicon(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
								Bitmap val8 = new Bitmap(256, 256);
								try
								{
									Graphics val9 = Graphics.FromImage((Image)(object)val8);
									try
									{
										val9.SmoothingMode = (SmoothingMode)4;
										Rectangle rectangle4 = new Rectangle(0, 0, 256, 256);
										int num4 = 256;
										Size size2 = new Size(num4, num4);
										Rectangle rectangle5 = new Rectangle(rectangle4.Location, size2);
										GraphicsPath val10 = new GraphicsPath();
										try
										{
											val10.AddArc(rectangle5, 180f, 90f);
											rectangle5.X = rectangle4.Right - num4;
											val10.AddArc(rectangle5, 270f, 90f);
											rectangle5.Y = rectangle4.Bottom - num4;
											val10.AddArc(rectangle5, 0f, 90f);
											rectangle5.X = rectangle4.Left;
											val10.AddArc(rectangle5, 90f, 90f);
											val10.CloseFigure();
											val9.FillPath(Brushes.OrangeRed, val10);
											int num5 = 175 - (text.Length - 1) * 35;
											int num6 = 10 + (text.Length - 1) * 22;
											int num7 = -5;
											if (text.Length == 1)
											{
												num7 = 35;
											}
											if (num5 > 150)
											{
												num5 = 150;
												num6 += 14;
											}
											Font val11 = new Font("Arial", (float)((double)num5 / MainWindow.sScalingFactor), (FontStyle)0);
											try
											{
												val9.DrawString(text, val11, Brushes.White, (float)num7, (float)num6);
												TaskbarManager.Instance.SetOverlayIcon(window, Icon.FromHandle(val8.GetHicon()), text);
											}
											finally
											{
												((IDisposable)val11)?.Dispose();
											}
										}
										finally
										{
											((IDisposable)val10)?.Dispose();
										}
									}
									finally
									{
										((IDisposable)val9)?.Dispose();
									}
								}
								finally
								{
									((IDisposable)val8)?.Dispose();
								}
							}
							finally
							{
								((IDisposable)val7)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Info("error" + ex);
		}
	}

	internal void MuteApplication(bool allInstances)
	{
		NativeMethods.waveOutSetVolume(IntPtr.Zero, 0u);
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				["allInstances"] = allInstances.ToString(CultureInfo.InvariantCulture),
				["explicit"] = "true"
			};
			HTTPUtils.SendRequestToEngine("mute", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "");
			ParentWindow.mCommonHandler.OnVolumeMuted(muted: true);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
		}
	}

	internal void UnmuteApplication(bool allInstances)
	{
		if (!FeatureManager.Instance.IsCustomUIForDMM)
		{
			if (allInstances && ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				ParentWindow.mSidebar.UpdateMuteAllInstancesCheckbox();
				return;
			}
			if (!allInstances && RegistryManager.Instance.AreAllInstancesMuted)
			{
				RegistryManager.Instance.AreAllInstancesMuted = false;
				foreach (MainWindow value in DictWindows.Values)
				{
					value.mSidebar.UpdateMuteAllInstancesCheckbox();
				}
			}
		}
		NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { ["allInstances"] = allInstances.ToString(CultureInfo.InvariantCulture) };
			HTTPUtils.SendRequestToEngine("unmute", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "");
			ParentWindow.mCommonHandler.OnVolumeMuted(muted: false);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
		}
	}

	internal void SetCurrentVolumeForDMM(int previousVolume, int newVolume)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				ParentWindow.Utils.SetVolumeInFrontendAsync(newVolume);
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume;
				}, new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set volume... Err : " + ex.ToString());
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume;
				}, new object[0]);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal void SetVolumeLevelFromAndroid(int volume)
	{
		CurrentVolumeLevel = volume;
	}

	internal void SetVolumeInFrontendAsync(int newVolume)
	{
		_ = CurrentVolumeLevel;
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				if (ParentWindow.mGuestBootCompleted)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"vol",
						newVolume.ToString(CultureInfo.InvariantCulture)
					} };
					if (Convert.ToBoolean(JArray.Parse(HTTPUtils.SendRequestToEngine("setVolume", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, ""))[0][(object)"success"], CultureInfo.InvariantCulture))
					{
						CurrentVolumeLevel = newVolume;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set volume. Ex: " + ex.ToString());
			}
			if (CurrentVolumeLevel == 0)
			{
				MuteApplication(allInstances: false);
			}
			if ((ParentWindow.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted) && CurrentVolumeLevel != 0)
			{
				UnmuteApplication(allInstances: false);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal void GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				int millisecondsTimeout = 1000;
				int volume = mCurrentVolumeLevel;
				int num = 60;
				while (num > 0)
				{
					num--;
					try
					{
						JObject val = JObject.Parse(HTTPUtils.SendRequestToGuest("getVolume", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, true, 1, 0, "bgp64"));
						if (((object)val["result"]).ToString() != "ok")
						{
							Thread.Sleep(millisecondsTimeout);
							continue;
						}
						volume = Convert.ToInt32(((object)val["volume"]).ToString(), CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						Logger.Warning("Failed to get volume from guest: {0}", new object[1] { ex.Message });
						Thread.Sleep(millisecondsTimeout);
						continue;
					}
					break;
				}
				CurrentVolumeLevel = volume;
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					((DispatcherObject)ParentWindow.mDmmBottomBar).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						ParentWindow.mDmmBottomBar.CurrentVolume = volume;
					}, new object[0]);
				}
				if (RegistryManager.Instance.AreAllInstancesMuted)
				{
					MuteApplication(allInstances: true);
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Failed to get volume: " + ex2.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal static void RestartInstance(string vmName)
	{
		RegistryManager.Instance.Guest[Opt.Instance.vmname].EnableHighFPS = 0;
		RegistryManager.Instance.Guest[Opt.Instance.vmname].FPS = 30;
		using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BlueStacks_bgp64\\Guests\\" + vmName, writable: true))
		{
			if (registryKey != null)
			{
				string[] array = registryKey.GetValue("BootParameters").ToString().Split(new char[1] { ' ' });
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].StartsWith("fps="))
					{
						array[i] = "fps=30";
						break;
					}
				}
				registryKey.SetValue("BootParameters", string.Join(" ", array));
			}
		}
		if (DictWindows.ContainsKey(vmName))
		{
			DictWindows[vmName].RestartInstanceAndPerform();
		}
	}

	internal static void SwitchAndRestartInstanceInAgl(string vmName)
	{
		if (DictWindows.ContainsKey(vmName))
		{
			DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 1;
			Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp64");
			DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
			RestartInstance(vmName);
		}
	}

	internal static void SwitchAndRestartInstanceInOglAfterRunningGlCheck(string vmName, Action openApp)
	{
		if (!DictWindows.ContainsKey(vmName))
		{
			return;
		}
		if (!isOglSupported.HasValue)
		{
			DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_RUNNING_CHECKS";
			((UIElement)DictWindows[vmName].mExitProgressGrid).Visibility = (Visibility)0;
			using BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += Bgw_DoWork;
			backgroundWorker.RunWorkerCompleted += Bgw_RunWorkerCompleted;
			backgroundWorker.RunWorkerAsync(new object[2] { vmName, openApp });
			return;
		}
		Bgw_RunWorkerCompleted(new object(), new RunWorkerCompletedEventArgs(new object[2] { vmName, openApp }, null, cancelled: false));
	}

	private static void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (e.Error != null)
		{
			return;
		}
		string text = ((object[])e.Result)[0].ToString();
		Action openApp = (Action)((object[])e.Result)[1];
		DictWindows[text].mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
		((UIElement)DictWindows[text].mExitProgressGrid).Visibility = (Visibility)1;
		if (isOglSupported == true)
		{
			DictWindows[text].EngineInstanceRegistry.GlRenderMode = 1;
			Utils.UpdateValueInBootParams("GlMode", "1", text, true, "bgp64");
			DictWindows[text].EngineInstanceRegistry.GlMode = 1;
			RestartInstance(text);
			return;
		}
		CustomMessageWindow val = new CustomMessageWindow();
		val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOT_SUPPORTED", "");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOTSUPPORTED_BODY", "");
		val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), (EventHandler)delegate
		{
			openApp();
		}, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)DictWindows[text];
		DictWindows[text].ShowDimOverlay();
		((Window)val).ShowDialog();
		DictWindows[text].HideDimOverlay();
	}

	private static void Bgw_DoWork(object sender, DoWorkEventArgs e)
	{
		int num = default(int);
		string text = default(string);
		string text2 = default(string);
		string text3 = default(string);
		isOglSupported = Utils.CheckOpenGlSupport(ref num, ref text, ref text2, ref text3, RegistryStrings.InstallDir);
		e.Result = e.Argument;
	}

	internal static void SwitchAndRestartInstanceInDx(string vmName)
	{
		if (DictWindows.ContainsKey(vmName))
		{
			DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
			Utils.UpdateValueInBootParams("GlMode", "1", vmName, true, "bgp64");
			DictWindows[vmName].EngineInstanceRegistry.GlMode = 1;
			RestartInstance(vmName);
		}
	}

	internal static void SwitchAndRestartInstanceInAdx(string vmName)
	{
		if (DictWindows.ContainsKey(vmName))
		{
			DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
			Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp64");
			DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
			RestartInstance(vmName);
		}
	}

	internal void RunAppOrCreateTabButton(string packageName)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (ParentWindow.mTopBar.mAppTabButtons.mHomeAppTabButton.IsSelected)
			{
				ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName);
			}
			else
			{
				AppIconModel appIcon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
				if (appIcon != null)
				{
					ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: false, isLaunch: false);
				}
			}
		}, new object[0]);
	}

	internal void ResetPendingUIOperations()
	{
		try
		{
			if (ParentWindow.mGuestBootCompleted)
			{
				ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = string.Empty;
				AppHandler.EventOnAppDisplayed = null;
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					ParentWindow.mDmmBottomBar.ShowKeyMapPopup(isShow: false);
				}
				else
				{
					ParentWindow.mSidebar.ShowKeyMapPopup(isShow: false);
					ParentWindow.mSidebar.ShowOverlayTooltip(isShow: false);
				}
				ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
				ParentWindow.StaticComponents.ShowUninstallButtons(isShow: false);
				ParentWindow.ClosePopUps();
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error in ResetPendingUIOperations " + ex.ToString());
		}
	}

	internal static void AddBootEventHandler(string vmName, EventHandler bootedEvennt)
	{
		if (BootEventsForMIManager.ContainsKey(vmName))
		{
			BootEventsForMIManager[vmName].Add(bootedEvennt);
			return;
		}
		BootEventsForMIManager.Add(vmName, new List<EventHandler> { bootedEvennt });
	}

	internal static void InvokeMIManagerEvents(string VmName)
	{
		if (!BootEventsForMIManager.ContainsKey(VmName))
		{
			return;
		}
		foreach (EventHandler item in BootEventsForMIManager[VmName])
		{
			item(null, null);
		}
	}

	internal void ShakeWindow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((Window)ParentWindow).WindowState == 2)
		{
			ParentWindow.StoryBoard.Begin();
			return;
		}
		int num = 10;
		int num2 = 5;
		int num3 = 0;
		int num4 = 0;
		while (num > 0)
		{
			switch (num4)
			{
			case 0:
				num3 = num2;
				break;
			case 1:
				num3 = num2 * -1;
				break;
			case 2:
				num3 = num2 * -1;
				break;
			case 3:
				num3 = num2;
				break;
			}
			num4++;
			if (num4 == 4)
			{
				num4 = 0;
				num--;
			}
			MainWindow parentWindow = ParentWindow;
			((Window)parentWindow).Left = ((Window)parentWindow).Left + (double)num3;
			Thread.Sleep(30);
		}
	}

	internal static void RunInstance(string vmName, bool hiddenMode = false)
	{
		if (lstCreatingWindows.Contains(vmName))
		{
			return;
		}
		if (DictWindows.ContainsKey(vmName) && !hiddenMode)
		{
			((DispatcherObject)DictWindows[vmName]).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				DictWindows[vmName].ShowWindow();
			}, new object[0]);
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			lstCreatingWindows.Add(vmName);
			FrontendHandler frontendHandler = new FrontendHandler(vmName);
			MainWindow mainWindow = new MainWindow(vmName, frontendHandler);
			DictWindows[vmName] = mainWindow;
			lstCreatingWindows.Remove(vmName);
			if (!hiddenMode)
			{
				mainWindow.ShowWindow(updateBootStartTime: true);
			}
		}, new object[0]);
	}

	internal void CheckGuestFailedAsync()
	{
		sBootCheckTimer.Elapsed += SBootCheckTimer_Elapsed;
		sBootCheckTimer.Enabled = true;
	}

	internal static void HideAllBlueStacks()
	{
		foreach (MainWindow item in DictWindows.Values.ToList())
		{
			CloseWindows((Window)(object)item);
			((Window)item).ShowInTaskbar = false;
			((Window)item).Hide();
		}
	}

	private void SBootCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		(sender as System.Timers.Timer).Enabled = false;
		if (!ParentWindow.mGuestBootCompleted)
		{
			SendGuestBootFailureStats("boot timeout exception");
		}
	}

	public static string GetFinalRedirectedUrl(string url)
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
			string result = null;
			using (WebResponse webResponse = httpWebRequest.GetResponse())
			{
				result = webResponse.ResponseUri.ToString();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logger.Error("Error in getting redirected url" + ex.ToString());
			return null;
		}
	}

	internal void SendGuestBootFailureStats(string errorString)
	{
		if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			ClientStats.SendClientStatsAsync("update_init", "fail", "engine_activity", "", errorString);
		}
		else if (RegistryManager.Instance.IsClientFirstLaunch == 1)
		{
			ClientStats.SendClientStatsAsync("first_init", "fail", "engine_activity", "", errorString);
		}
		else
		{
			ClientStats.SendClientStatsAsync("init", "fail", "engine_activity", "", errorString);
		}
		ParentWindow.HandleRestartPopup();
	}

	internal static bool CheckForMacrAvailable(string packageName)
	{
		string path = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper"), packageName + "_macro.cfg");
		string path2 = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles"), packageName + "_macro.cfg");
		if (File.Exists(path) || File.Exists(path2))
		{
			return true;
		}
		return false;
	}

	internal static string GetVideoTutorialUrl(string packageName, string videoMode, string selectedSchemeName)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		string serverHost = WebHelper.GetServerHost();
		serverHost = serverHost.Substring(0, serverHost.Length - 4);
		string text = ((!((object)(GuidanceVideoType)5/*cast due to constrained. prefix*/).ToString().ToLower(CultureInfo.InvariantCulture).Equals(videoMode, StringComparison.InvariantCulture)) ? WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}", new object[4] { serverHost, "videoTutorial", packageName, videoMode })) : WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}&scheme={4}", new object[5] { serverHost, "videoTutorial", packageName, videoMode, selectedSchemeName })));
		if (!RegistryManager.Instance.IgnoreAutoPlayPackageList.Contains(packageName))
		{
			text = string.Format(CultureInfo.InvariantCulture, "{0}&autoplay=1", new object[1] { text });
			List<string> ignoreAutoPlayPackageList = RegistryManager.Instance.IgnoreAutoPlayPackageList;
			ignoreAutoPlayPackageList.Add(packageName);
			RegistryManager.Instance.IgnoreAutoPlayPackageList = ignoreAutoPlayPackageList;
		}
		else
		{
			text = string.Format(CultureInfo.InvariantCulture, "{0}&autoplay=0", new object[1] { text });
		}
		if (!string.IsNullOrEmpty(RegistryManager.Instance.Partner))
		{
			text += string.Format(CultureInfo.InvariantCulture, "&partner={0}", new object[1] { RegistryManager.Instance.Partner });
		}
		return text;
	}

	internal static string GetOnboardingUrl(string packageName, string source)
	{
		return WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&source={3}", new object[4]
		{
			RegistryManager.Instance.Host,
			"bs3/page/onboarding-tutorial",
			packageName,
			source
		}));
	}

	internal void SetKeyMapping(string packageName, string source)
	{
		string path = Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles");
		string destFileName = Path.Combine(path, string.Format(CultureInfo.InvariantCulture, "{0}.cfg", new object[1] { packageName }));
		string sourceFileName = Path.Combine(path, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.cfg", new object[2] { packageName, source }));
		try
		{
			File.Copy(sourceFileName, destFileName, overwrite: true);
		}
		catch (Exception ex)
		{
			Logger.Error("Faield to copy cfgs... Err : " + ex.ToString());
			return;
		}
		ParentWindow.mFrontendHandler.RefreshKeyMap(packageName);
	}

	internal static void OpenUrl(string url)
	{
		try
		{
			Process.Start(url);
		}
		catch (Win32Exception)
		{
			try
			{
				Process.Start("IExplore.exe", url);
			}
			catch (Exception ex2)
			{
				Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex2.ToString());
			}
		}
		catch (Exception ex3)
		{
			Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex3.ToString());
		}
	}

	internal bool IsSufficientRAMAvailable()
	{
		Logger.Info("Checking for physical memory...");
		long num = long.Parse(SystemUtils.GetSysInfo("Select TotalPhysicalMemory from Win32_ComputerSystem"), CultureInfo.InvariantCulture);
		long num2 = 1073741824L;
		if (num < num2)
		{
			return false;
		}
		return true;
	}

	public void SendMessageToAndroidForAffiliate(string pkgName, string source)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		try
		{
			Logger.Info("Sending message to Android for affiliate");
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "action", "com.bluestacks.home.AFFILIATE_HANDLER_HTML" } };
			JObject val = new JObject();
			val.Add("success", JToken.op_Implicit(true));
			val.Add("app_pkg", JToken.op_Implicit(pkgName));
			val.Add("WINDOWS_SOURCE", JToken.op_Implicit(source));
			JObject val2 = val;
			dictionary.Add("extras", ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
			HTTPUtils.SendRequestToGuest("customStartService", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't send message to Adnroid: " + ex.ToString());
		}
	}

	public void AppendUrlWithCommonParamsAndOpenTab(string url, string title, string imagePath, string tabKey = "")
	{
		try
		{
			url = WebHelper.GetUrlWithParams(url);
			if (UsefulExtensionMethod.Contains(new Uri(url).Host, "bluestacks", StringComparison.InvariantCultureIgnoreCase))
			{
				string registeredEmail = RegistryManager.Instance.RegisteredEmail;
				string token = RegistryManager.Instance.Token;
				if (string.IsNullOrEmpty(registeredEmail))
				{
					Logger.Warning("User email not found. Not opening webpage.");
				}
				if (string.IsNullOrEmpty(token))
				{
					Logger.Warning("User token not found. Not opening webpage.");
				}
			}
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mTopBar.mAppTabButtons.AddWebTab(url, title, imagePath, isSwitch: true, tabKey);
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception when parsing uri for opening in webtab " + ex.ToString());
		}
	}

	public void ApplyTheme(string themeName)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		BlueStacksUIColorManager.ReloadAppliedTheme(themeName);
		Publisher.PublishMessage((BrowserControlTags)17, ParentWindow.mVmName, new JObject((object)new JProperty("Theme", (object)themeName)));
		RefreshAppCenterUrl();
		RefreshHtmlSidePanelUrl();
		ParentWindow.mCommonHandler.SetCustomCursorForApp(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		ClientStats.SendMiscellaneousStatsAsync("SkinChangedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.ClientThemeName, null);
	}

	public void RestoreWallpaperImageForAllVms()
	{
		foreach (MainWindow item in DictWindows.Values.ToList())
		{
			item.mWelcomeTab.mHomeAppManager.RestoreWallpaper();
		}
	}

	public void ChooseWallpaper()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			OpenFileDialog val = new OpenFileDialog
			{
				Title = LocaleStrings.GetLocalizedString("STRING_CHANGE_WALLPAPER", ""),
				RestoreDirectory = true,
				DefaultExt = ".jpg",
				Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
			};
			try
			{
				if ((int)((CommonDialog)val).ShowDialog() != 1)
				{
					return;
				}
				Bitmap val2 = new Bitmap(((FileDialog)val).FileName);
				((Image)val2).Save(HomeAppManager.BackgroundImagePath);
				((Image)val2).Dispose();
				foreach (MainWindow item in DictWindows.Values.ToList())
				{
					item.mWelcomeTab.mHomeAppManager.ApplyWallpaper();
				}
				ClientStats.SendMiscellaneousStatsAsync("WallPaperStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Premium", "Changed_Wallpaper", null);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in changing wallpaper:" + ex.ToString());
			MessageBox.Show("Cannot change wallpaper.Please try again.", "Error");
		}
	}

	internal static string GetAppCenterUrl(string tabId)
	{
		string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/appcenter-v2");
		urlWithParams += "&theme=";
		urlWithParams += RegistryManager.ClientThemeName;
		urlWithParams += "&naked=1";
		if (!string.IsNullOrEmpty(tabId))
		{
			urlWithParams += "&tabid=";
			urlWithParams += tabId;
		}
		return urlWithParams;
	}

	internal static string GetHtmlSidePanelUrl()
	{
		return string.Concat(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/myapps-sidepanel") + "&theme=", RegistryManager.ClientThemeName);
	}

	internal string GetHtmlHomeUrl()
	{
		string text = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/bgp-home-html") + "&theme=" + RegistryManager.ClientThemeName + "&vmId=" + Utils.GetVmIdFromVmName(ParentWindow.mVmName) + "&vmName=" + ParentWindow.mVmName + "&firstLaunchedVmName=" + Strings.CurrentDefaultVmName + "&oem=" + RegistryManager.Instance.Oem;
		if (!string.IsNullOrEmpty(Opt.Instance.Json))
		{
			JObject val = JObject.Parse(Opt.Instance.Json);
			if (val["fle_pkg"] != null)
			{
				text = text + "&flePackageName=" + ((object)val["fle_pkg"]).ToString().Trim();
			}
			if (val["campaign_id"] != null)
			{
				text = text + "&campaignId=" + ((object)val["campaign_id"]).ToString().Trim();
			}
			if (val["source"] != null)
			{
				text = text + "&source=" + ((object)val["source"]).ToString().Trim();
			}
		}
		return text;
	}

	internal void RefreshHtmlHomeUrl()
	{
		try
		{
			ParentWindow.mWelcomeTab.ReInitHtmlHome();
		}
		catch (Exception ex)
		{
			Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", new object[2] { ParentWindow.mVmName, ex });
		}
	}

	internal static string GetGiftTabUrl()
	{
		return string.Concat(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/gift") + "&theme=", RegistryManager.ClientThemeName);
	}

	internal static string GetPikaWorldUrl()
	{
		return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/pikaworld") + "&naked=1";
	}

	internal void SwitchProfile(string vmName, string pcode)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		try
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			JObject val = new JObject();
			val.Add("pcode", JToken.op_Implicit(pcode));
			val.Add("createcustomprofile", JToken.op_Implicit("false"));
			JObject val2 = val;
			data.Add("data", ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
			DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_SWITCHING_PROFILE";
			((UIElement)DictWindows[vmName].mExitProgressGrid).Visibility = (Visibility)0;
			ThreadPool.QueueUserWorkItem(delegate
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Expected O, but got Unknown
				JObject val3 = new JObject { ["pcode"] = JToken.op_Implicit(Utils.GetValueInBootParams("pcode", ParentWindow.mVmName, "", "bgp64")) };
				string text = HTTPUtils.SendRequestToGuest("changeDeviceProfile", data, vmName, 0, (Dictionary<string, string>)null, false, 3, 60000, "bgp64");
				Logger.Info("Response for ChangeDeviceProfile: " + text);
				JObject val4 = JObject.Parse(text);
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
					((UIElement)ParentWindow.mExitProgressGrid).Visibility = (Visibility)1;
				}, new object[0]);
				JObject val5 = new JObject { ["pcode"] = JToken.op_Implicit(pcode) };
				if (((object)val4["result"]).ToString() == "ok")
				{
					Logger.Info("Successfully updated Device Profile.");
					Utils.UpdateValueInBootParams("pcode", pcode, ParentWindow.mVmName, false, "bgp64");
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_UPDATED", ""));
					ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "success", JsonConvert.SerializeObject((object)val5), JsonConvert.SerializeObject((object)val3), RegistryManager.Instance.Version, "GRM");
				}
				else
				{
					Logger.Warning("DeviceProfile Update failed in android");
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""));
					ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "failed", JsonConvert.SerializeObject((object)val5), JsonConvert.SerializeObject((object)val3), RegistryManager.Instance.Version, "GRM");
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SwitchProfileAndRestart: {0}", new object[1] { ex });
		}
	}

	internal static string GetHelpCenterUrl()
	{
		return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/feedback");
	}

	internal static void RefreshHtmlSidePanelUrl()
	{
		foreach (MainWindow value in DictWindows.Values)
		{
			try
			{
				value.mWelcomeTab.mHomeAppManager.ReinitHtmlSidePanel();
			}
			catch (Exception ex)
			{
				Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", new object[2] { value.mVmName, ex });
			}
		}
	}

	internal static void RefreshAppCenterUrl()
	{
		if (DictWindows.ContainsKey(Strings.CurrentDefaultVmName))
		{
			MainWindow mainWindow = DictWindows[Strings.CurrentDefaultVmName];
			AppTabButton tab = mainWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
			if (tab != null && tab.GetBrowserControl() != null)
			{
				tab.GetBrowserControl().NavigateTo(GetAppCenterUrl(""));
			}
			AppTabButton tab2 = mainWindow.mTopBar.mAppTabButtons.GetTab("gift");
			if (tab2 != null && tab2.GetBrowserControl() != null)
			{
				tab2.GetBrowserControl().NavigateTo(GetGiftTabUrl());
			}
		}
	}

	internal static string GetMacroCommunityUrl(string currentAppPackage)
	{
		return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/macro-share") + "&pkg=" + currentAppPackage;
	}

	internal bool IsRequiredFreeRAMAvailable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ulong num = 1048576uL;
			ulong availablePhysicalMemory = new ComputerInfo().AvailablePhysicalMemory;
			int num2 = ParentWindow.EngineInstanceRegistry.Memory + 100;
			if (num2 > 2148)
			{
				num2 = 2148;
			}
			ulong num3 = (ulong)num2 * num;
			if (availablePhysicalMemory < num3)
			{
				Logger.Warning("Available physical memory is less than required. {0} < {1}", new object[2]
				{
					availablePhysicalMemory / num,
					num2
				});
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("An error occurred while finding free RAM");
			Logger.Error(ex.ToString());
		}
		return true;
	}

	public bool CheckQuitPopupLocal()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		try
		{
			if (!ParentWindow.mGuestBootCompleted)
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0065: Unknown result type (might be due to invalid IL or missing references)
					//IL_006f: Expected O, but got Unknown
					QuitPopupControl quitPopupControl = new QuitPopupControl(ParentWindow);
					string tag = (quitPopupControl.CurrentPopupTag = "exit_popup_boot");
					BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_TROUBLE_STARTING_BLUESTACKS", "");
					BlueStacksUIBinding.Bind((Button)(object)quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
					quitPopupControl.AddQuitActionItem(QuitActionItem.StuckAtBoot);
					quitPopupControl.AddQuitActionItem(QuitActionItem.SlowPerformance);
					quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
					((UIElement)quitPopupControl.CloseBlueStacksButton).PreviewMouseUp += new MouseButtonEventHandler(ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
					ParentWindow.HideDimOverlay();
					ParentWindow.ShowDimOverlay(quitPopupControl);
					ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
				}, new object[0]);
				return true;
			}
			if (!RegistryManager.Instance.Guest[ParentWindow.mVmName].IsGoogleSigninDone && string.Equals(ParentWindow.StaticComponents.mSelectedTabButton.PackageName, "com.android.vending", StringComparison.InvariantCultureIgnoreCase))
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0065: Unknown result type (might be due to invalid IL or missing references)
					//IL_006f: Expected O, but got Unknown
					QuitPopupControl quitPopupControl = new QuitPopupControl(ParentWindow);
					string tag = (quitPopupControl.CurrentPopupTag = "exit_popup_ots");
					BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
					BlueStacksUIBinding.Bind((Button)(object)quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
					quitPopupControl.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
					quitPopupControl.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
					quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
					((UIElement)quitPopupControl.CloseBlueStacksButton).PreviewMouseUp += new MouseButtonEventHandler(ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
					ParentWindow.HideDimOverlay();
					ParentWindow.ShowDimOverlay(quitPopupControl);
					ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
				}, new object[0]);
				return true;
			}
			if (ParentWindow.mVmName == "Android" && (int)RegistryManager.Instance.FirstAppLaunchState != 3)
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_0075: Unknown result type (might be due to invalid IL or missing references)
					//IL_007f: Expected O, but got Unknown
					QuitPopupControl quitPopupControl = new QuitPopupControl(ParentWindow);
					string tag = (quitPopupControl.CurrentPopupTag = "exit_popup_no_app");
					BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_HAVING_TROUBLE_STARTING_GAME", "");
					BlueStacksUIBinding.Bind((Button)(object)quitPopupControl.ReturnBlueStacksButton, "STRING_RETURN_BLUESTACKS");
					BlueStacksUIBinding.Bind((Button)(object)quitPopupControl.CloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
					quitPopupControl.AddQuitActionItem(QuitActionItem.UnsureWhereStart);
					quitPopupControl.AddQuitActionItem(QuitActionItem.IssueInstallingGame);
					quitPopupControl.AddQuitActionItem(QuitActionItem.FacingOtherTroubles);
					((UIElement)quitPopupControl.CloseBlueStacksButton).PreviewMouseUp += new MouseButtonEventHandler(ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
					ParentWindow.HideDimOverlay();
					ParentWindow.ShowDimOverlay(quitPopupControl);
					ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
				}, new object[0]);
				return true;
			}
			if (!RegistryManager.Instance.IsNotificationModeAlwaysOn && ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose && ParentWindow.EngineInstanceRegistry.NotificationModePopupShownCount < RegistryManager.Instance.NotificationModeCounter && CheckIfNotificationModePopupToBeShown(ParentWindow, out var package) && string.Compare("Android", ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_005a: Expected O, but got Unknown
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0064: Expected O, but got Unknown
					ParentWindow.HideDimOverlay();
					NotificationModeExitPopup notificationModeExitPopup = new NotificationModeExitPopup(ParentWindow, package);
					new ContainerWindow(ParentWindow, (UserControl)(object)notificationModeExitPopup, ((FrameworkElement)notificationModeExitPopup).Width, ((FrameworkElement)notificationModeExitPopup).Height, autoHeight: false, isShow: true, isWindowTransparent: false, 12.0, (Brush)(SolidColorBrush)((TypeConverter)new BrushConverter()).ConvertFrom((object)"#4CFFFFFF"));
				}, new object[0]);
				return true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to show local quit popup. " + ex.ToString());
		}
		return false;
	}

	private bool CheckIfNotificationModePopupToBeShown(MainWindow window, out string packageName)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		packageName = window.StaticComponents.mSelectedTabButton.PackageName;
		new JsonParser(window.mVmName);
		if (window.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
		{
			bool flag = true;
			bool flag2 = true;
			foreach (string key in window.mTopBar.mAppTabButtons.mDictTabs.Keys)
			{
				bool value = (PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(key)).Value;
				flag = flag && value;
				flag2 = flag2 && !value;
			}
			if (flag)
			{
				return true;
			}
			if (flag2)
			{
				return false;
			}
			if ((PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(packageName)).Value)
			{
				return true;
			}
			return false;
		}
		bool flag3 = true;
		List<string> list = Utils.GetInstalledPackagesFromAppsJSon(ParentWindow.mVmName).Split(new char[1] { ',' }).ToList();
		foreach (string item in list)
		{
			if (!string.IsNullOrEmpty(item))
			{
				bool value2 = (PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(item)).Value;
				flag3 = flag3 && !value2;
			}
		}
		if (flag3)
		{
			return false;
		}
		packageName = window.EngineInstanceRegistry.LastNotificationEnabledAppLaunched;
		if (string.IsNullOrEmpty(packageName) || !list.Contains(packageName))
		{
			foreach (string item2 in list)
			{
				if ((PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(item2)).Value)
				{
					packageName = item2;
					break;
				}
			}
		}
		return true;
	}

	public void OpenPikaAccountPage()
	{
		if (ParentWindow.mAppHandler.IsOneTimeSetupCompleted && !string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
		{
			string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account?extra=section:pika");
			urlWithParams += "&email=";
			urlWithParams += RegistryManager.Instance.RegisteredEmail;
			urlWithParams += "&token=";
			urlWithParams += RegistryManager.Instance.Token;
			ParentWindow.mTopBar.mAppTabButtons.AddWebTab(urlWithParams, "STRING_ACCOUNT", "account_tab", isSwitch: true, "account_tab", forceRefresh: true);
		}
	}

	internal void HandleApplicationBrowserClick(string clickActionValue, string title, string key, bool paramsOnlyActionValue = false, string customImageName = "")
	{
		title = title.Trim();
		string imagePath = "cef_tab";
		switch (key)
		{
		case "appcenter":
		case "APP_CENTER_TEXT":
			clickActionValue = HTTPUtils.MergeQueryParams(GetAppCenterUrl(""), clickActionValue, paramsOnlyActionValue);
			if (string.IsNullOrEmpty(title))
			{
				title = LocaleStrings.GetLocalizedString("STRING_APP_CENTER", "");
			}
			key = "appcenter";
			imagePath = "appcenter";
			break;
		case "GIFT_TEXT":
		case "gift":
			clickActionValue = HTTPUtils.MergeQueryParams(GetGiftTabUrl(), clickActionValue, paramsOnlyActionValue);
			if (string.IsNullOrEmpty(title))
			{
				title = LocaleStrings.GetLocalizedString("STRING_GIFT", "");
			}
			key = "gift";
			imagePath = "gift";
			break;
		case "pikaworld":
		case "MAPS_TEXT":
			clickActionValue = HTTPUtils.MergeQueryParams(GetPikaWorldUrl(), clickActionValue, paramsOnlyActionValue);
			if (string.IsNullOrEmpty(title))
			{
				title = LocaleStrings.GetLocalizedString("STRING_MAPS", "");
			}
			key = "pikaworld";
			imagePath = "pikaworld";
			break;
		case "preregistration":
			if (string.IsNullOrEmpty(title))
			{
				title = LocaleStrings.GetLocalizedString("STRING_PREREGISTER", "");
			}
			imagePath = "preregistration";
			break;
		case "FEEDBACK_TEXT":
			clickActionValue = HTTPUtils.MergeQueryParams(GetHelpCenterUrl(), clickActionValue, paramsOnlyActionValue);
			Process.Start(clickActionValue);
			return;
		}
		if (!string.IsNullOrEmpty(customImageName))
		{
			imagePath = customImageName;
		}
		ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(clickActionValue, title, imagePath, key);
	}

	private static string GetImagePath(Dictionary<string, string> payload, string customImageName = "")
	{
		if (string.IsNullOrEmpty(customImageName))
		{
			if (payload.ContainsKey("icon_path"))
			{
				return payload["icon_path"];
			}
			if (payload.ContainsKey("click_action_app_icon_id") && File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + payload["click_action_app_icon_id"])))
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + payload["click_action_app_icon_id"]);
			}
			return "";
		}
		return customImageName;
	}

	public void HandleGenericActionFromDictionary(Dictionary<string, string> payload, string source, string customImageName = "")
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected I4, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Invalid comparison between Unknown and I4
		try
		{
			if (!payload.ContainsKey("click_generic_action"))
			{
				return;
			}
			GenericAction val = EnumHelper.Parse<GenericAction>(payload["click_generic_action"], (GenericAction)65536);
			if ((int)val <= 64)
			{
				if ((int)val <= 8)
				{
					switch (val - 1)
					{
					default:
						if ((int)val != 8)
						{
							break;
						}
						OpenUrl(payload["click_action_value"]);
						return;
					case 3:
						HandleApplicationBrowserClick(payload["click_action_value"], payload["click_action_title"], payload.ContainsKey("click_action_key") ? payload["click_action_key"] : "", paramsOnlyActionValue: false, GetImagePath(payload, customImageName));
						return;
					case 1:
						if (!ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
						{
							ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
						}
						ParentWindow.mAppInstaller.DownloadAndInstallApp(string.Empty, payload["click_action_title"], payload["click_action_value"], payload["click_action_packagename"], isLaunchAfterInstall: false, isDeleteApk: true);
						return;
					case 0:
						if (!ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
						{
							ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
						}
						ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(payload["click_action_packagename"], (PlayStoreAction)0);
						return;
					case 2:
						break;
					}
				}
				else
				{
					if ((int)val == 32)
					{
						ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home");
						if (string.Compare(payload["click_action_value"], "my_app_text", StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (payload.ContainsKey("query_params") && !string.IsNullOrEmpty(payload["query_params"].Trim()))
							{
								HandleApplicationBrowserClick(payload["query_params"], "", payload["click_action_value"], paramsOnlyActionValue: true, customImageName);
							}
							else
							{
								HandleApplicationBrowserClick("", "", payload["click_action_value"], paramsOnlyActionValue: true, customImageName);
							}
						}
						return;
					}
					if ((int)val == 64)
					{
						((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							MainWindow.OpenSettingsWindow(ParentWindow, payload["click_action_value"]);
						}, new object[0]);
						return;
					}
				}
			}
			else if ((int)val <= 256)
			{
				if ((int)val == 128)
				{
					switch (payload["click_action_key"].Trim().ToLower(CultureInfo.InvariantCulture))
					{
					case "instance_manager":
						LaunchMultiInstanceManager();
						break;
					case "macro_recorder":
						ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
						break;
					}
					return;
				}
				if ((int)val == 256)
				{
					ParentWindow.mTopBar.mAppTabButtons.AddAppTab(payload["click_action_title"], payload["click_action_packagename"], payload["click_action_app_activity"], GetImagePath(payload, customImageName), isSwitch: false, isLaunch: true);
					ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = payload["click_action_packagename"];
					ParentWindow.mAppHandler.SendRunAppRequestAsync(payload["click_action_packagename"], payload["click_action_app_activity"]);
					return;
				}
			}
			else
			{
				if ((int)val == 512)
				{
					ParentWindow.mCommonHandler.OpenBrowserInPopup(payload);
					return;
				}
				if ((int)val == 1024)
				{
					LaunchAllInstancesAndArrange();
					return;
				}
				if ((int)val == 2048)
				{
					if (!ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
					{
						ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
					}
					ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(payload["click_action_packagename"], payload["click_action_title"], (PlayStoreAction)0, isWindowForcedTillLoaded: true);
					return;
				}
			}
			Logger.Warning("Unknown case {0}", new object[1] { payload["click_generic_action"] });
		}
		catch (Exception ex)
		{
			Logger.Error("Exception on handling click event for payload " + UsefulExtensionMethod.ToDebugString<string, string>((IDictionary<string, string>)payload) + Environment.NewLine + "Exception: " + ex.ToString());
		}
	}

	internal bool CheckQuitPopupFromCloud(string appPackage = "")
	{
		try
		{
			Logger.Info("IsQuitPopupNotificationReceived status: " + ParentWindow.IsQuitPopupNotficationReceived);
			if (!RegistryManager.Instance.ShowGamingSummary || !ParentWindow.IsQuitPopupNotficationReceived || (string.IsNullOrEmpty(appPackage) && !ParentWindow.mQuitPopupBrowserControl.ShowOnQuit) || (!string.Equals(ParentWindow.mQuitPopupBrowserControl.PackageName, appPackage, StringComparison.InvariantCulture) && !string.Equals(ParentWindow.mQuitPopupBrowserControl.PackageName, "*", StringComparison.InvariantCulture) && !string.IsNullOrEmpty(appPackage)))
			{
				return false;
			}
			ParentWindow.IsQuitPopupNotficationReceived = false;
			if (ParentWindow.mQuitPopupBrowserControl.IsForceReload)
			{
				string quitPopupUrl = ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl;
				string text = JsonConvert.SerializeObject((object)AppUsageTimer.GetRealtimeDictionary()[ParentWindow.mVmName], (Formatting)0);
				string text2 = "usage_data=" + text;
				quitPopupUrl = HTTPUtils.MergeQueryParams(quitPopupUrl, text2, true);
				quitPopupUrl = WebHelper.GetUrlWithParams(quitPopupUrl);
				ParentWindow.mQuitPopupBrowserControl.RefreshBrowserUrl(quitPopupUrl);
			}
			if (!string.IsNullOrEmpty(ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl))
			{
				((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					ParentWindow.HideDimOverlay();
					ParentWindow.mQuitPopupBrowserControl.Init(appPackage);
				}, new object[0]);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to show quit popup. " + ex.ToString());
			return false;
		}
	}

	internal static void LaunchMultiInstanceManager()
	{
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			ProcessUtils.GetProcessObject(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), (string)null, false).Start();
		}
	}

	internal static void RemoveChildFromParent(UIElement child)
	{
		DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)child);
		Panel val = (Panel)(object)((parent is Panel) ? parent : null);
		if (val != null)
		{
			val.Children.Remove(child);
		}
		ContentControl val2 = (ContentControl)(object)((parent is ContentControl) ? parent : null);
		if (val2 != null)
		{
			val2.Content = null;
		}
		Decorator val3 = (Decorator)(object)((parent is Decorator) ? parent : null);
		if (val3 != null)
		{
			val3.Child = null;
		}
	}

	public static void UpdateLocale(string locale, string vmToIgnore = "")
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		new List<string>();
		foreach (string vmName in RegistryManager.Instance.VmList.ToList())
		{
			try
			{
				if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
				{
					InstanceRegistry value = new InstanceRegistry(vmName, "bgp64");
					RegistryManager.Instance.Guest.Add(vmName, value);
				}
				if (!(RegistryManager.Instance.UserSelectedLocale != RegistryManager.Instance.Guest[vmName].Locale))
				{
					continue;
				}
				RegistryManager.Instance.Guest[vmName].Locale = locale;
				Utils.UpdateValueInBootParams("LANG", locale, vmName, false, "bgp64");
				if (!DictWindows.ContainsKey(vmName) || string.Compare(vmName, vmToIgnore.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
				{
					continue;
				}
				string cmd = string.Format(CultureInfo.InvariantCulture, "setlocale {0}", new object[1] { locale });
				Thread thread = new Thread((ThreadStart)delegate
				{
					if (VmCmdHandler.RunCommand(cmd, vmName) == null)
					{
						Logger.Error("Set locale did not work for vm " + vmName);
					}
				});
				thread.IsBackground = true;
				thread.Start();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to change locale for vm : " + vmName);
				Logger.Error(ex.ToString());
			}
		}
		HTTPUtils.SendRequestToAgentAsync("reinitlocalization", (Dictionary<string, string>)null, "Android", 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		LocaleStrings.InitLocalization((string)null, "Android", false);
	}

	internal static bool SendBluestacksLoginRequest(string vmName)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		bool result = false;
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "action", "com.bluestacks.account.RETRY_BLUESTACKS_LOGIN" } };
			JObject val = new JObject();
			val.Add("windows", JToken.op_Implicit("true"));
			JObject val2 = val;
			dictionary.Add("extras", ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
			Logger.Info("Sending bluestacks login request");
			HTTPUtils.SendRequestToGuest("customStartService".ToLower(CultureInfo.InvariantCulture), dictionary, vmName, 500, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			result = true;
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't send request to guest for login. Ex: {0}", new object[1] { ex.Message });
		}
		return result;
	}

	internal static bool CheckIfMacroScriptBookmarked(string fileName)
	{
		if (Enumerable.Contains(RegistryManager.Instance.BookmarkedScriptList, fileName))
		{
			return true;
		}
		return false;
	}

	internal static string GetMacroPlaybackEventName(string vmname)
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[2] { "MacroPlayBack", vmname });
	}

	internal void HandleLaunchPlay(string package)
	{
		lock (mLaunchPlaySyncObj)
		{
			int num = 180000;
			while (num > 0)
			{
				num--;
				if (ParentWindow.mEnableLaunchPlayForNCSoft || (!FeatureManager.Instance.IsCustomUIForNCSoft && ParentWindow.mGuestBootCompleted))
				{
					break;
				}
				Thread.Sleep(1000);
			}
			if (num > 0)
			{
				HTTPUtils.SendRequestToGuest(string.Format(CultureInfo.InvariantCulture, "launchplay?pkgname={0}", new object[1] { package }), (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
		}
	}

	internal void VolumeDownHandler()
	{
		if (CurrentVolumeLevel != 0)
		{
			int num = CurrentVolumeLevel - 7;
			if (num <= 0)
			{
				num = 0;
			}
			SetVolumeInFrontendAsync(num);
		}
	}

	internal void VolumeUpHandler()
	{
		if (CurrentVolumeLevel < 100)
		{
			int num = CurrentVolumeLevel + 7;
			if (num >= 100)
			{
				num = 100;
			}
			SetVolumeInFrontendAsync(num);
		}
	}

	internal void ToggleTopBarSidebarEnabled(bool isEnabled)
	{
		((UIElement)ParentWindow.TopBar).IsEnabled = isEnabled;
		((UIElement)ParentWindow.mSidebar).IsEnabled = isEnabled;
	}

	internal static void SendGamepadStatusToBrowsers(bool status)
	{
		try
		{
			object[] array = new object[1] { "" };
			array[0] = status.ToString(CultureInfo.InvariantCulture);
			foreach (BrowserControl sAllBrowserControl in BrowserControl.sAllBrowserControls)
			{
				try
				{
					if (sAllBrowserControl != null && sAllBrowserControl.CefBrowser != null)
					{
						sAllBrowserControl.CefBrowser.CallJs("toggleGamePadSupport", array);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in sending gamepad status to browser:" + sAllBrowserControl.mUrl + Environment.NewLine + ex.ToString());
				}
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in sending gamepad status to browser:" + ex2.ToString());
		}
	}

	internal static double GetDefaultHeight()
	{
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			return SystemParameters.MaximizedPrimaryScreenHeight * 0.6 + 94.0;
		}
		return SystemParameters.MaximizedPrimaryScreenHeight * 0.75 + 94.0;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (sBootCheckTimer != null)
			{
				sBootCheckTimer.Elapsed -= SBootCheckTimer_Elapsed;
				sBootCheckTimer.Dispose();
			}
			disposedValue = true;
		}
	}

	~BlueStacksUIUtils()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal static Dictionary<string, string> GetEngineSettingsData(string vmName)
	{
		return new Dictionary<string, string>
		{
			{
				"cpu",
				RegistryManager.Instance.Guest[vmName].VCPUs.ToString(CultureInfo.InvariantCulture)
			},
			{
				"ram",
				RegistryManager.Instance.Guest[vmName].Memory.ToString(CultureInfo.InvariantCulture)
			},
			{
				"glMode",
				RegistryManager.Instance.Guest[vmName].GlMode.ToString(CultureInfo.InvariantCulture)
			},
			{
				"glRenderMode",
				RegistryManager.Instance.Guest[vmName].GlRenderMode.ToString(CultureInfo.InvariantCulture)
			},
			{
				"gpu",
				RegistryManager.Instance.AvailableGPUDetails
			}
		};
	}

	internal static Dictionary<string, string> GetResolutionData()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int guestWidth = RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth;
		int guestHeight = RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight;
		string value = Convert.ToString(guestWidth, CultureInfo.InvariantCulture) + "x" + Convert.ToString(guestHeight, CultureInfo.InvariantCulture);
		dictionary.Add("resolution", value);
		double num = (double)guestWidth / (double)guestHeight;
		string value2 = "landscape";
		if (num < 1.0)
		{
			value2 = "portrait";
		}
		dictionary.Add("resolution_type", value2);
		return dictionary;
	}

	internal void DownloadAndUpdateMacro(string macroData)
	{
		try
		{
			JObject val = JObject.Parse(macroData);
			string? text = ((object)val["macro_name"]).ToString();
			string url = ((object)val["download_link"]).ToString();
			string userName = ((object)val["nickname"]).ToString();
			string authorPageUrl = ((object)val["author_url"]).ToString();
			string macroId = ((object)val["macro_id"]).ToString();
			string macroPageUrl = ((object)val["macro_url"]).ToString();
			string invalidCharsFreeName = default(string);
			if (!StringExtensions.GetValidFileName(text, ref invalidCharsFreeName))
			{
				return;
			}
			string path = string.Format(CultureInfo.InvariantCulture, "{0}.json", new object[1] { invalidCharsFreeName });
			string filePath = Path.Combine(Path.GetTempPath(), path);
			Thread thread = new Thread((ThreadStart)delegate
			{
				using WebClient webClient = new WebClient();
				try
				{
					webClient.DownloadFile(url, filePath);
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to download macro at path : " + filePath + ". Ex : " + ex.ToString());
				}
				finally
				{
					webClient?.Dispose();
				}
				if (File.Exists(filePath))
				{
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						try
						{
							MacroRecording val2 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(filePath), Utils.GetSerializerSettings());
							int num;
							try
							{
								if (!string.IsNullOrEmpty(userName))
								{
									val2.User = userName;
								}
								if (!string.IsNullOrEmpty(authorPageUrl) && Uri.TryCreate(authorPageUrl, UriKind.RelativeOrAbsolute, out Uri result))
								{
									val2.AuthorPageUrl = result;
								}
								if (!string.IsNullOrEmpty(macroId))
								{
									val2.MacroId = macroId;
								}
								if (!string.IsNullOrEmpty(macroPageUrl) && Uri.TryCreate(macroPageUrl, UriKind.RelativeOrAbsolute, out Uri result2))
								{
									val2.MacroPageUrl = result2;
								}
								val2.Name = invalidCharsFreeName;
								if (string.IsNullOrEmpty(val2.TimeCreated))
								{
									val2.TimeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
								}
								bool isShowRenameWizard = false;
								ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
								ParentWindow.MacroRecorderWindow.mImportMultiMacroAsUnified = true;
								num = ParentWindow.MacroRecorderWindow.ImportMacroRecordings(new List<MacroRecording> { val2 }, ref isShowRenameWizard);
								if (isShowRenameWizard)
								{
									num = 3;
								}
							}
							catch (Exception ex2)
							{
								Logger.Error("Failed to import macro recording.");
								Logger.Error(ex2.ToString());
								num = 2;
							}
							if (num == 0)
							{
								foreach (KeyValuePair<string, MainWindow> dictWindow in DictWindows)
								{
									if (dictWindow.Value.MacroRecorderWindow != null)
									{
										((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children.Clear();
										dictWindow.Value.MacroRecorderWindow.Init();
									}
								}
							}
							ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
							ParentWindow.MacroRecorderWindow.ValidateReturnCode(num);
						}
						catch (Exception ex3)
						{
							Logger.Error("Failed to deserialize downloaded macro.");
							Logger.Error(ex3.ToString());
						}
					}, new object[0]);
					try
					{
						File.Delete(filePath);
						return;
					}
					catch
					{
						return;
					}
				}
			});
			thread.IsBackground = true;
			thread.Start();
		}
		catch (Exception arg)
		{
			Logger.Error($"Invalid data in DowloadMacro api : {arg}");
		}
	}

	internal static List<string> GetMacroList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		List<string> list = new List<string>();
		try
		{
			MacroGraph.ReCreateMacroGraphInstance();
			foreach (MacroRecording vertex in MacroGraph.Instance.Vertices)
			{
				MacroRecording val = vertex;
				if (!string.IsNullOrEmpty(val.Name) && string.IsNullOrEmpty(val.MacroId))
				{
					list.Add(val.Name);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("Failed to get macro list. Ex : " + ex.ToString());
		}
		return list;
	}

	internal static string GetBase64MacroData(string macroName)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		string result = string.Empty;
		try
		{
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
			MacroRecording val = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where string.Equals(macroName, macro.Name, StringComparison.InvariantCultureIgnoreCase)
				select macro).FirstOrDefault();
			if ((int)val.RecordingType == 0)
			{
				Logger.Info("Uploading single recording macro");
				result = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)val, serializerSettings)));
			}
			else
			{
				List<string> list = new List<string>();
				foreach (MacroRecording allChild in MacroGraph.Instance.GetAllChilds((BiDirectionalVertex<MacroRecording>)(object)val))
				{
					MacroRecording val2 = allChild;
					list.Add(File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, val2.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
				}
				MacroRecording val3 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), serializerSettings);
				val3.SourceRecordings = list;
				result = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)val3, serializerSettings)));
				Logger.Info("Uploading merged macro");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Coulnd't upload macro recording {0}, Ex: {1}", new object[2] { macroName, ex });
		}
		return result;
	}

	private static void LaunchAllInstancesAndArrange()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			RegistryManager.ClearRegistryMangerInstance();
			string[] vmList = RegistryManager.Instance.VmList;
			foreach (string text in vmList)
			{
				if (!DictWindows.ContainsKey(text))
				{
					RunInstance(text);
					int num = RegistryManager.Instance.BatchInstanceStartInterval;
					if (num <= 0)
					{
						num = 2;
					}
					Thread.Sleep(num * 1000);
				}
			}
		});
	}

	static BlueStacksUIUtils()
	{
		mLaunchPlaySyncObj = new object();
		sLoggedInImageName = "loggedin";
		sPremiumUserImageName = "premiumuser";
		sUserAccountPackageName = "com.uncube.account";
		sUserAccountActivityName = "com.bluestacks.account.activities.AccountActivity_";
		sAndroidSettingsPackageName = "com.android.settings";
		sAndroidAccountSettingsActivityName = "com.android.settings.BstAccountsSettings";
		sStopStatSendingThread = false;
		lstCreatingWindows = new List<string>();
		DictWindows = new Dictionary<string, MainWindow>();
		sIsSynchronizationActive = false;
		sSelectedInstancesForSync = new List<string>();
		BootEventsForMIManager = new Dictionary<string, List<EventHandler>>();
		sSyncInvolvedInstances = new List<string>();
		isOglSupported = null;
	}
}
