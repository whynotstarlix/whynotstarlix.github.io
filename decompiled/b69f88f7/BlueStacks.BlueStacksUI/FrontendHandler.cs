using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class FrontendHandler
{
	private int frontendRestartAttempts;

	internal MainWindow ParentWindow;

	private string mVmName;

	internal string mWindowTitle;

	private bool sIsfrontendAlreadyVisible;

	internal IntPtr mFrontendHandle;

	internal DateTime mFrontendStartTime = DateTime.Now;

	internal bool mIsSufficientRAMAvailable = true;

	internal bool IsShootingModeActivated;

	private object mLockObject = new object();

	internal Action<MainWindow> ReparentingCompletedAction;

	internal bool IsRestartFrontendWhenClosed { get; set; }

	internal event EventHandler mEventOnFrontendClosed;

	public FrontendHandler(string vmName)
	{
		mVmName = vmName;
		mWindowTitle = Oem.Instance.CommonAppTitleText + vmName;
		StartFrontend();
	}

	internal void FrontendHandler_ShowLowRAMMessage()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		MainWindow parentWindow = ParentWindow;
		if (((parentWindow != null) ? new bool?(((FrameworkElement)parentWindow).IsLoaded) : ((bool?)null)) == true)
		{
			CustomMessageWindow cmw = new CustomMessageWindow();
			cmw.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_PERF_WARNING", "");
			cmw.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TITLE", ""), "message_error");
			cmw.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT1", "") + Environment.NewLine + LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT2", "");
			cmw.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_CONTINUE_ANYWAY", ""), (EventHandler)delegate
			{
				((Window)cmw).Close();
			}, (string)null, false, (object)null);
			cmw.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CLOSE_BLUESTACKS", ""), (EventHandler)delegate
			{
				((Window)ParentWindow).Close();
			}, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)cmw).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)cmw).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
	}

	internal void StartFrontend()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			Logger.Info("BOOT_STAGE: Starting player");
			if (ProcessUtils.IsLockInUse(Strings.GetPlayerLockName(mVmName, "bgp64")))
			{
				KillFrontend(isWaitForPlayerClosing: true);
			}
			this.mEventOnFrontendClosed = null;
			mIsSufficientRAMAvailable = true;
			IsRestartFrontendWhenClosed = true;
			mFrontendStartTime = DateTime.Now;
			int num = BluestacksProcessHelper.StartFrontend(mVmName);
			if (ParentWindow == null)
			{
				WaitForParentWindowInit();
			}
			if (ParentWindow != null)
			{
				switch (num)
				{
				case -5:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_000a: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0024: Unknown result type (might be due to invalid IL or missing references)
						//IL_003e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0053: Unknown result type (might be due to invalid IL or missing references)
						Logger.Error("Hyper v enabled on this machine");
						CustomMessageWindow val = new CustomMessageWindow();
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
						val.AddWarning(LocaleStrings.GetLocalizedString("STRING_HYPERV_ENABLED_WARNING", ""), "message_error");
						BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_HYPERV_ENABLED_MESSAGE", "");
						val.AddButton((ButtonColors)4, "STRING_CHECK_FAQ", (EventHandler)delegate
						{
							BlueStacksUIUtils.OpenUrl("https://t.me/BluesterBot");
						}, (string)null, false, (object)null);
						((Window)val).ShowDialog();
						App.ExitApplication();
					}, new object[0]);
					break;
				case -2:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_000a: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0024: Unknown result type (might be due to invalid IL or missing references)
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						Logger.Error("Android File Integrity check failed");
						CustomMessageWindow val = new CustomMessageWindow();
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_CORRUPT_INSTALLATION", "");
						BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_CORRUPT_INSTALLATION_MESSAGE", "");
						val.AddButton((ButtonColors)4, "STRING_EXIT", (EventHandler)null, (string)null, false, (object)null);
						((Window)val).ShowDialog();
						App.ExitApplication();
					}, new object[0]);
					break;
				case -7:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_000a: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0024: Unknown result type (might be due to invalid IL or missing references)
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						Logger.Error("VBox couldn't detect driver");
						CustomMessageWindow val = new CustomMessageWindow();
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
						BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_COULDNT_BOOT_TRY_RESTART", "");
						val.AddButton((ButtonColors)4, "STRING_RESTART_PC", (EventHandler)RestartPCEvent, (string)null, false, (object)null);
						val.AddButton((ButtonColors)2, "STRING_EXIT", (EventHandler)null, (string)null, false, (object)null);
						((Window)val).ShowDialog();
						App.ExitApplication();
					}, new object[0]);
					break;
				case -6:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_000a: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0044: Unknown result type (might be due to invalid IL or missing references)
						//IL_0050: Unknown result type (might be due to invalid IL or missing references)
						//IL_0065: Unknown result type (might be due to invalid IL or missing references)
						Logger.Error("Unable to initialise audio on this machine");
						CustomMessageWindow val = new CustomMessageWindow
						{
							ImageName = "sound_error"
						};
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_AUDIO_SERVICE_FAILURE", "");
						BlueStacksUIBinding.Bind(val.BodyTextBlockTitle, "STRING_AUDIO_SERVICE_FAILUE_FIX", "");
						((UIElement)val.BodyTextBlockTitle).Visibility = (Visibility)0;
						BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_AUDIO_SERVICE_FAILURE_ALTERNATE_FIX", "");
						val.AddButton((ButtonColors)4, "STRING_READ_MORE", (EventHandler)delegate
						{
							BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
							{
								WebHelper.GetServerHost(),
								"help_articles"
							})) + "&article=audio_service_issue");
						}, "external_link", true, (object)null);
						((Window)val).ShowDialog();
						App.ExitApplication();
					}, new object[0]);
					break;
				case -10:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_006f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0074: Unknown result type (might be due to invalid IL or missing references)
						//IL_0089: Unknown result type (might be due to invalid IL or missing references)
						//IL_009e: Unknown result type (might be due to invalid IL or missing references)
						//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
						string url = null;
						url = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						}));
						url = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[2] { url, "enable_virtualization" });
						string text = "STRING_VTX_DISABLED_ENABLEIT_BODY";
						CustomMessageWindow val = new CustomMessageWindow();
						BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
						val.AddAboveBodyWarning(LocaleStrings.GetLocalizedString("STRING_VTX_DISABLED_WARNING", ""));
						((UIElement)val.AboveBodyWarningTextBlock).Visibility = (Visibility)0;
						((FrameworkElement)val.MessageIcon).VerticalAlignment = (VerticalAlignment)1;
						BlueStacksUIBinding.Bind(val.BodyTextBlock, text, "");
						val.AddButton((ButtonColors)4, "STRING_CHECK_FAQ", (EventHandler)delegate
						{
							BlueStacksUIUtils.OpenUrl(url);
						}, (string)null, false, (object)null);
						val.AddButton((ButtonColors)2, "STRING_EXIT", (EventHandler)null, (string)null, false, (object)null);
						((Window)val).ShowDialog();
						App.ExitApplication();
					}, new object[0]);
					break;
				default:
					if (IsRestartFrontendWhenClosed)
					{
						((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							if (frontendRestartAttempts < 2)
							{
								frontendRestartAttempts++;
								ParentWindow.RestartFrontend();
							}
						}, new object[0]);
					}
					break;
				}
			}
			else
			{
				Logger.Error("parent window is null for vmName: {0} and frontend Exit code: {1}", new object[2] { mVmName, num });
			}
		});
	}

	private void WaitForParentWindowInit()
	{
		Logger.Info("In method WaitForParentWindowInit for vmName: " + mVmName);
		int num = 20;
		while (num > 0)
		{
			num--;
			try
			{
				if (ParentWindow != null && BlueStacksUIUtils.DictWindows.ContainsKey(mVmName))
				{
					Logger.Info("parent window init for vmName: " + mVmName);
					return;
				}
				Thread.Sleep(200);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in wait for mainwindow init: " + ex.ToString());
				Thread.Sleep(200);
			}
		}
		Logger.Error("Parent window not init after {0} retries", new object[1] { num });
	}

	private void RestartPCEvent(object sender, EventArgs e)
	{
		Process.Start("shutdown.exe", "/r /t 0");
	}

	internal void KillFrontendAsync()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			KillFrontend(isWaitForPlayerClosing: true);
		});
	}

	internal void KillFrontend(bool isWaitForPlayerClosing = false)
	{
		try
		{
			IsRestartFrontendWhenClosed = false;
			Utils.StopFrontend(mVmName, isWaitForPlayerClosing);
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in killing frontend: " + ex.ToString());
		}
		finally
		{
			this.mEventOnFrontendClosed?.Invoke(mVmName, null);
		}
	}

	internal void EnableKeyMapping(bool isEnabled)
	{
		try
		{
			SendFrontendRequestAsync("setKeymappingState", new Dictionary<string, string> { 
			{
				"keymapping",
				isEnabled.ToString(CultureInfo.InvariantCulture)
			} });
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send EnableKeyMapping to frontend... Err : " + ex.ToString());
		}
	}

	internal void GetScreenShot(string filePath)
	{
		Dictionary<string, string> data = new Dictionary<string, string>
		{
			{ "path", filePath },
			{
				"showSavedInfo",
				true.ToString(CultureInfo.InvariantCulture)
			}
		};
		SendFrontendRequestAsync("getScreenshot", data);
	}

	internal void FrontendVisibleChanged(bool value)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"new_value",
				Convert.ToString(value, CultureInfo.InvariantCulture)
			} };
			if (RegistryManager.Instance.AreAllInstancesMuted || ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				dictionary.Add("is_mute", Convert.ToString(value: true, CultureInfo.InvariantCulture));
			}
			else
			{
				dictionary.Add("is_mute", Convert.ToString(value: false, CultureInfo.InvariantCulture));
			}
			SendFrontendRequestAsync("frontendVisibleChanged", dictionary);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
		}
	}

	internal void RefreshKeyMap(string packageName)
	{
		try
		{
			Dictionary<string, string> data = new Dictionary<string, string> { { "package", packageName } };
			SendFrontendRequestAsync("refreshKeymap", data);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
		}
	}

	internal void DeactivateFrontend()
	{
		try
		{
			if (mFrontendHandle != IntPtr.Zero)
			{
				Logger.Debug("KMP deactivateFrontend");
				SendFrontendRequestAsync("deactivateFrontend");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send deactivate to frontend.. Err : " + ex.ToString());
		}
	}

	internal void ToggleStreamingMode(bool state)
	{
		((Popup)ParentWindow.mTopBar.mSettingsMenuPopup).IsOpen = false;
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			Logger.Info("Streaming mode toggle called with state.." + state);
			ParentWindow.mStreamingModeEnabled = state;
			string value = ParentWindow.Handle.ToString();
			if (state)
			{
				value = "0";
			}
			Rectangle windowRectangle = GetWindowRectangle();
			if (windowRectangle.Width == 0 && windowRectangle.Height == 0)
			{
				windowRectangle.Width = (int)((FrameworkElement)ParentWindow).Width;
				windowRectangle.Height = (int)((FrameworkElement)ParentWindow).Height;
			}
			Dictionary<string, string> dict = new Dictionary<string, string>
			{
				{ "ParentHandle", value },
				{
					"X",
					windowRectangle.X.ToString(CultureInfo.InvariantCulture)
				},
				{
					"Y",
					windowRectangle.Y.ToString(CultureInfo.InvariantCulture)
				},
				{
					"Width",
					windowRectangle.Width.ToString(CultureInfo.InvariantCulture)
				},
				{
					"Height",
					windowRectangle.Height.ToString(CultureInfo.InvariantCulture)
				}
			};
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					JObject val = JObject.Parse(((object)JArray.Parse(SendFrontendRequest("setparent", dict))[0]).ToString());
					if (val["success"].ToObject<bool>())
					{
						mFrontendHandle = new IntPtr(val["frontendhandle"].ToObject<int>());
						((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							ShowGLWindow();
						}, new object[0]);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
				}
			});
		}, new object[0]);
	}

	internal void ShowGLWindow()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Invalid comparison between Unknown and I4
		if (CanfrontendBeResizedAndFocused())
		{
			ResizeWindow();
			return;
		}
		if (((UIElement)ParentWindow.mFrontendGrid).IsVisible)
		{
			if (ParentWindow.Handle.ToString().Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			sIsfrontendAlreadyVisible = true;
			if (mFrontendHandle == IntPtr.Zero)
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
						{
							Rectangle windowRectangle = GetWindowRectangle();
							Dictionary<string, string> dict = new Dictionary<string, string>
							{
								{
									"ParentHandle",
									(!ParentWindow.mStreamingModeEnabled) ? ParentWindow.Handle.ToString() : "0"
								},
								{
									"X",
									windowRectangle.X.ToString(CultureInfo.InvariantCulture)
								},
								{
									"Y",
									windowRectangle.Y.ToString(CultureInfo.InvariantCulture)
								},
								{
									"Width",
									windowRectangle.Width.ToString(CultureInfo.InvariantCulture)
								},
								{
									"Height",
									windowRectangle.Height.ToString(CultureInfo.InvariantCulture)
								}
							};
							if (windowRectangle.Width == 0 || windowRectangle.Height == 0)
							{
								sIsfrontendAlreadyVisible = false;
							}
							else
							{
								ThreadPool.QueueUserWorkItem(delegate
								{
									try
									{
										lock (mLockObject)
										{
											if (mFrontendHandle == IntPtr.Zero)
											{
												JObject val = JObject.Parse(((object)JArray.Parse(SendFrontendRequest("setParent", dict))[0]).ToString());
												if (val["success"].ToObject<bool>())
												{
													mFrontendHandle = new IntPtr(val["frontendhandle"].ToObject<int>());
												}
												ReparentingCompletedAction?.Invoke(ParentWindow);
												Logger.Debug("Set parent call completed. handle: " + mFrontendHandle);
											}
										}
									}
									catch (Exception ex2)
									{
										Logger.Error("Failed to send Show event to engine... err : " + ex2.ToString());
									}
								});
							}
						}, new object[0]);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
					}
				});
			}
			else
			{
				ResizeWindow();
			}
			return;
		}
		sIsfrontendAlreadyVisible = false;
		if (mFrontendHandle != IntPtr.Zero)
		{
			InteropWindow.ShowWindow(mFrontendHandle, 0);
			if (KMManager.dictOverlayWindow.ContainsKey(ParentWindow) && (int)((Window)ParentWindow).WindowState != 2)
			{
				KMManager.ShowOverlayWindow(ParentWindow, isShow: false);
			}
		}
	}

	private bool CanfrontendBeResizedAndFocused()
	{
		if (ParentWindow.mDimOverlay == null || !ParentWindow.mDimOverlay.IsWindowVisible)
		{
			if (((UIElement)ParentWindow.mFrontendGrid).IsVisible)
			{
				return sIsfrontendAlreadyVisible;
			}
			return false;
		}
		return false;
	}

	internal void ResizeWindow()
	{
		Rectangle windowRectangle = GetWindowRectangle();
		if (ParentWindow.mStreamingModeEnabled)
		{
			InteropWindow.ShowWindow(mFrontendHandle, 5);
		}
		else
		{
			InteropWindow.SetWindowPos(mFrontendHandle, (IntPtr)0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448u);
		}
		if (KMManager.dictOverlayWindow.ContainsKey(ParentWindow))
		{
			if (ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
			{
				ParentWindow.StaticComponents.mLastMappableWindowHandle = mFrontendHandle;
			}
			KMManager.dictOverlayWindow[ParentWindow].UpdateSize();
		}
		FocusFrontend();
		RegistryManager.Instance.FrontendHeight = windowRectangle.Height;
		RegistryManager.Instance.FrontendWidth = windowRectangle.Width;
	}

	internal Rectangle GetWindowRectangle()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Grid mFrontendGrid = ParentWindow.mFrontendGrid;
		Point val = ((UIElement)mFrontendGrid).TranslatePoint(new Point(0.0, 0.0), (UIElement)(object)ParentWindow);
		Point location = new Point((int)(MainWindow.sScalingFactor * ((Point)(ref val)).X), (int)(MainWindow.sScalingFactor * ((Point)(ref val)).Y));
		Size size = new Size((int)(((FrameworkElement)mFrontendGrid).ActualWidth * MainWindow.sScalingFactor), (int)(((FrameworkElement)mFrontendGrid).ActualHeight * MainWindow.sScalingFactor));
		return new Rectangle(location, size);
	}

	internal void ChangeFrontendToPortraitMode()
	{
		Rectangle windowRectangle = GetWindowRectangle();
		InteropWindow.SetWindowPos(mFrontendHandle, (IntPtr)0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448u);
	}

	internal void FocusFrontend()
	{
		if (CanfrontendBeResizedAndFocused() && !ParentWindow.mStreamingModeEnabled && !ParentWindow.mIsFocusComeFromImap && ((Window)ParentWindow).IsActive)
		{
			InteropWindow.SetFocus(mFrontendHandle);
			Logger.Debug("KMP REFRESH Frontend...." + Environment.StackTrace);
			SendFrontendRequestAsync("refreshWindow");
		}
		else
		{
			Logger.Debug("KMP CanfrontendBeResizedAndFocused false " + ((UIElement)ParentWindow.mFrontendGrid).IsVisible + sIsfrontendAlreadyVisible);
		}
	}

	internal void SendFrontendRequestAsync(string path, Dictionary<string, string> data = null)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			SendFrontendRequest(path, data);
		});
	}

	internal string SendFrontendRequest(string path, Dictionary<string, string> data = null)
	{
		string result = string.Empty;
		try
		{
			result = HTTPUtils.SendRequestToEngine(path, data, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "");
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendFrontendRequest: " + ex.ToString());
		}
		return result;
	}

	internal static void UpdateBootTimeInregistry(DateTime time)
	{
		try
		{
			int lastBootTime = (int)(DateTime.Now - time).TotalSeconds * 1000;
			int noOfBootCompleted = RegistryManager.Instance.NoOfBootCompleted;
			RegistryManager.Instance.LastBootTime = lastBootTime;
			RegistryManager.Instance.NoOfBootCompleted = noOfBootCompleted + 1;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UpdateBootTimeInregistry: " + ex.ToString());
		}
	}

	internal void UpdateOverlaySizeStatus()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		SendFrontendRequestAsync("sendGlWindowSize", new Dictionary<string, string> { 
		{
			"updateSize",
			((int)((Window)ParentWindow).WindowState == 2).ToString(CultureInfo.InvariantCulture)
		} });
	}
}
