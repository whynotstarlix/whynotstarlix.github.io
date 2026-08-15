using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SettingsWindow : SettingsWindowBase
{
	internal CustomSettingsButton updateButton;

	internal CustomSettingsButton gameSettingsButton;

	internal bool mIsShortcutEdited;

	internal bool mIsShortcutSaveBtnEnabled;

	internal List<string> mDuplicateShortcutsList;

	internal Dictionary<string, CustomSettingsButton> mSettingsButtons;

	public MainWindow ParentWindow { get; }

	public SettingsWindow(MainWindow window, string startUpTab)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		SettingsWindow settingsWindow;
		do
		{
			mDuplicateShortcutsList = new List<string>();
			mSettingsButtons = new Dictionary<string, CustomSettingsButton>();
			((SettingsWindowBase)this)._002Ector();
			settingsWindow = this;
			ParentWindow = window;
			((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_DISPLAY_SETTINGS");
			((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_ENGINE_SETTING");
		}
		while (window == null);
		if (window.mGuestBootCompleted)
		{
		}
		if (window.mCommonHandler.mShortcutsConfigInstance != null)
		{
		}
		Logger.Warning("Not showing shortcuts settings as the config instance is null");
		UpdateSettingsListAndStartTabForCustomOEMs();
		((FrameworkElement)this).Loaded += (RoutedEventHandler)delegate
		{
			settingsWindow.SettingsWindow_Loaded(window);
		};
		if (!string.IsNullOrEmpty(startUpTab))
		{
		}
		CreateAllButtons(((SettingsWindowBase)this).StartUpTab);
		ChangeSettingsTab(window, ((SettingsWindowBase)this).StartUpTab);
	}

	public void ChangeSettingsTab(MainWindow window, string tab)
	{
		UserControl userControl = GetUserControl(tab, window);
		if (userControl == null)
		{
			userControl = GetUserControl("STRING_DISPLAY_SETTINGS", window);
		}
		((SettingsWindowBase)this).AddControlInGridAndDict(tab, userControl);
		((SettingsWindowBase)this).BringToFront(userControl);
		if (!mSettingsButtons[tab].IsSelected)
		{
			mSettingsButtons[tab].IsSelected = true;
			((UIElement)mSettingsButtons[tab]).IsEnabled = true;
		}
	}

	public void UpdateSettingsListAndStartTabForCustomOEMs()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((SettingsWindowBase)this).SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_SCREENSHOT" };
		}
		else if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
		{
			((SettingsWindowBase)this).SettingsControlNameList = new List<string> { "STRING_ABOUT_SETTING" };
			((SettingsWindowBase)this).StartUpTab = "STRING_ABOUT_SETTING";
		}
		else if (string.Equals(Oem.Instance.OEM, "yoozoo", StringComparison.InvariantCulture))
		{
			((SettingsWindowBase)this).SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_PREFERENCES" };
		}
		else if ((int)RegistryManager.Instance.InstallationType == 1)
		{
			((SettingsWindowBase)this).SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_ABOUT_SETTING" };
		}
		else if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((SettingsWindowBase)this).SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_PREFERENCES", "STRING_SHORTCUT_KEY_SETTINGS", "STRING_USER_DATA_SETTINGS" };
		}
	}

	private UserControl GetUserControl(string controlName, MainWindow window)
	{
		if (controlName != null)
		{
			uint num = global::_003CPrivateImplementationDetails_003E.ComputeStringHash(controlName);
			int num2 = ((1399651899 > 1668781152) ? (-1925123313) : (-1443842485));
			if (num <= (uint)num2)
			{
				if (num <= 1977647087 - 819113609 % 1388818194)
				{
					if (num != unchecked(-1749117068 - (0x689C35DC | 0x1CB42D00)))
					{
						if (num == 480269765 - ~678263712 && controlName == "STRING_NOTIFICATION")
						{
							return (UserControl)(object)new NotificationsSettings(window);
						}
					}
					else if (controlName == "STRING_ENGINE_SETTING")
					{
						return (UserControl)(object)GetEngineView(window);
					}
				}
				else if (num != 2256989329u)
				{
					int num3 = ((1307633318 > 980196294) ? (-1899214341) : 1762681508);
					if (num != (uint)num3)
					{
						if (num == 2851124811u && controlName == "STRING_DISPLAY_SETTINGS")
						{
							return (UserControl)(object)new DisplaySettingsControl(window);
						}
					}
					else if (controlName == "STRING_ADVANCED")
					{
						return (UserControl)(object)new DeviceProfileControl(window);
					}
				}
				else if (controlName == "STRING_SCREENSHOT")
				{
					return (UserControl)(object)new DMMScreenshotSettingControl(window);
				}
			}
			else if (num <= 3066349429u)
			{
				if (num != 2925263117u)
				{
					if (num == 3066349429u && controlName == "STRING_GAME_SETTINGS")
					{
						return (UserControl)(object)GetGameSettingView(window);
					}
				}
				else if (controlName == "STRING_USER_DATA_SETTINGS")
				{
					return (UserControl)(object)new BackupRestoreSettingsControl(window);
				}
			}
			else if (num != 3082043033u)
			{
				if (num != 3350936637u)
				{
					if (num == 3467783225u && controlName == "STRING_SHORTCUT_KEY_SETTINGS")
					{
						return (UserControl)(object)new ShortcutKeysControl(window, this);
					}
				}
				else if (controlName == "STRING_PREFERENCES")
				{
					return (UserControl)(object)new PreferencesSettingsControl(window);
				}
			}
			else if (controlName == "STRING_ABOUT_SETTING")
			{
				return (UserControl)(object)new AboutSettingsControl(window, this);
			}
		}
		return null;
	}

	private static GameSettingView GetGameSettingView(MainWindow window)
	{
		GameSettingViewModel gameSettingViewModel = new GameSettingViewModel(window);
		GameSettingView gameSettingView = new GameSettingView();
		((UIElement)gameSettingView).Visibility = (Visibility)(1995962257 - (0x20B6F18E | 0x7643E701));
		((FrameworkElement)gameSettingView).DataContext = gameSettingViewModel;
		return gameSettingViewModel.View = gameSettingView;
	}

	private static EngineSettingBase GetEngineView(MainWindow window)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		EngineSettingBase val = new EngineSettingBase
		{
			Visibility = (Visibility)(-8640617 + 8640619 % 1865763709)
		};
		EngineSettingViewModel dataContext = new EngineSettingViewModel(window, window.mVmName, val);
		((FrameworkElement)val).DataContext = dataContext;
		return val;
	}

	private void SettingsWindow_Loaded(MainWindow window)
	{
		Window.GetWindow((DependencyObject)(object)this).Closing += SettingWindow_Closing;
		Thread thread = new Thread((ThreadStart)delegate
		{
			Thread.Sleep(500);
			foreach (string settingName in ((SettingsWindowBase)this).SettingsControlNameList)
			{
				if (!string.Equals(settingName, ((SettingsWindowBase)this).StartUpTab, StringComparison.InvariantCulture))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						//IL_005f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0065: Expected O, but got Unknown
						UserControl userControl = GetUserControl(settingName, window);
						if (userControl != null)
						{
							((SettingsWindowBase)this).AddControlInGridAndDict(settingName, userControl);
							foreach (CustomSettingsButton child in ((Panel)((SettingsWindowBase)this).SettingsWindowStackPanel).Children)
							{
								CustomSettingsButton val = child;
								if (((FrameworkElement)val).Name == settingName)
								{
									((UIElement)val).IsEnabled = true;
								}
							}
						}
					}, new object[0]);
				}
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void SettingWindow_Closing(object sender, CancelEventArgs e)
	{
		try
		{
			MainWindow.CloseSettingsWindow(null);
			if (mIsShortcutEdited && mIsShortcutSaveBtnEnabled)
			{
				CommonHandlers.ReloadShortcutsForAllInstances();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SettingsWindowClosing. Exception: " + ex);
		}
	}

	private void CreateAllButtons(string mstartUpTab)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		foreach (string settingsControlName in ((SettingsWindowBase)this).SettingsControlNameList)
		{
			CustomSettingsButton val = new CustomSettingsButton
			{
				Name = settingsControlName,
				Group = "Settings"
			};
			mSettingsButtons.Add(settingsControlName, val);
			TextBlock val2 = new TextBlock
			{
				FontSize = 15.0,
				TextWrapping = (TextWrapping)2
			};
			BlueStacksUIBinding.Bind(val2, settingsControlName, "");
			((ContentControl)val).Content = val2;
			((FrameworkElement)val).MinHeight = 40.0;
			((Control)val).FontWeight = FontWeights.Normal;
			((Control)val).IsTabStop = false;
			((FrameworkElement)val).FocusVisualStyle = null;
			((UIElement)val).IsEnabled = false;
			((UIElement)val).PreviewMouseDown += new MouseButtonEventHandler(ValidateAndSwitchTab);
			((Panel)((SettingsWindowBase)this).SettingsWindowStackPanel).Children.Add((UIElement)(object)val);
			if (mstartUpTab == settingsControlName)
			{
				((UIElement)val).IsEnabled = true;
				val.IsSelected = true;
			}
		}
	}

	private void ValidateAndSwitchTab(object sender, MouseButtonEventArgs args)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		object obj = sender;
		CustomSettingsButton val = (CustomSettingsButton)((obj is CustomSettingsButton) ? obj : null);
		if ((object)((object)((SettingsWindowBase)this).SettingsWindowControlsDict[((FrameworkElement)val).Name]).GetType() == ((object)((SettingsWindowBase)this).visibleControl).GetType())
		{
			return;
		}
		UserControl visibleControl = ((SettingsWindowBase)this).visibleControl;
		EngineSettingBase val2 = (EngineSettingBase)(object)((visibleControl is EngineSettingBase) ? visibleControl : null);
		if (val2 != null)
		{
			object dataContext = ((FrameworkElement)val2).DataContext;
			EngineSettingBaseViewModel val3 = (EngineSettingBaseViewModel)((dataContext is EngineSettingBaseViewModel) ? dataContext : null);
			if (val3 != null)
			{
				if ((int)val3.Status == 1)
				{
					Logger.Info("Compatibility check is running");
				}
				else if (val3.IsDirty())
				{
					CustomMessageWindow val4 = new CustomMessageWindow
					{
						Owner = val3.Owner,
						WindowStartupLocation = (WindowStartupLocation)(1566427922 + ~1566427919)
					};
					BlueStacksUIBinding.Bind(val4.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
					BlueStacksUIBinding.Bind(val4.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
					val4.AddButton((ButtonColors)(1996 - (1044472964 >> 1546836275)), "STRING_NO", (EventHandler)delegate
					{
						((RoutedEventArgs)args).Handled = true;
					}, (string)null, false, (object)null);
					val4.AddButton((ButtonColors)(-1615894101 ^ -1615894103), "STRING_DISCARD_CHANGES", (EventHandler)delegate
					{
						((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
					}, (string)null, false, (object)null);
					((Window)val4).ShowDialog();
				}
				else
				{
					((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
				}
				return;
			}
		}
		UserControl visibleControl2 = ((SettingsWindowBase)this).visibleControl;
		DisplaySettingsControl displaySetting = visibleControl2 as DisplaySettingsControl;
		if (displaySetting != null && ((DisplaySettingsBase)displaySetting).IsDirty())
		{
			CustomMessageWindow val5 = new CustomMessageWindow
			{
				Owner = (Window)(object)displaySetting.ParentWindow,
				WindowStartupLocation = (WindowStartupLocation)(-1578634238 ^ -1578634240)
			};
			BlueStacksUIBinding.Bind(val5.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val5.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
			val5.AddButton((ButtonColors)(-1790836340 + (0x62091E58 | 0x28B4F438)), "STRING_NO", (EventHandler)delegate
			{
				((RoutedEventArgs)args).Handled = true;
			}, (string)null, false, (object)null);
			int num = ((798941729 > 2021557016) ? 2 : 2);
			val5.AddButton((ButtonColors)num, "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				((DisplaySettingsBase)displaySetting).DiscardCurrentChangingModel();
				((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
			}, (string)null, false, (object)null);
			((Window)val5).ShowDialog();
			return;
		}
		if (((SettingsWindowBase)this).visibleControl is GameSettingView gameSettingView)
		{
			object dataContext2 = ((FrameworkElement)gameSettingView).DataContext;
			GameSettingViewModel gameSettingViewModel = dataContext2 as GameSettingViewModel;
			if (gameSettingViewModel != null && gameSettingViewModel.IsDirty())
			{
				CustomMessageWindow val6 = new CustomMessageWindow
				{
					Owner = (Window)(object)ParentWindow,
					WindowStartupLocation = (WindowStartupLocation)(516886260 + ~516886257)
				};
				BlueStacksUIBinding.Bind(val6.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(val6.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
				int num2 = ((1288084203 > 852414022) ? 4 : 5);
				val6.AddButton((ButtonColors)num2, "STRING_NO", (EventHandler)delegate
				{
					((RoutedEventArgs)args).Handled = true;
				}, (string)null, false, (object)null);
				val6.AddButton((ButtonColors)(1021281273 + ~1021281270), "STRING_DISCARD_CHANGES", (EventHandler)delegate
				{
					gameSettingViewModel.Reset();
					gameSettingViewModel.Init();
					((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
				}, (string)null, false, (object)null);
				((Window)val6).ShowDialog();
				return;
			}
		}
		visibleControl2 = ((SettingsWindowBase)this).visibleControl;
		DeviceProfileControl deviceSetting = visibleControl2 as DeviceProfileControl;
		if (deviceSetting != null && deviceSetting.IsDirty())
		{
			CustomMessageWindow val7 = new CustomMessageWindow
			{
				Owner = (Window)(object)ParentWindow
			};
			int num3 = ((1866503415 > 1399929580) ? 2 : 2);
			((Window)val7).WindowStartupLocation = (WindowStartupLocation)num3;
			BlueStacksUIBinding.Bind(val7.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val7.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
			val7.AddButton((ButtonColors)(420595385 - 1121955562 % 701360181), "STRING_NO", (EventHandler)delegate
			{
				((RoutedEventArgs)args).Handled = true;
			}, (string)null, false, (object)null);
			val7.AddButton((ButtonColors)(-1431370456 ^ -1431370454), "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				deviceSetting.Init();
				((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
			}, (string)null, false, (object)null);
			((Window)val7).ShowDialog();
		}
		else
		{
			((SettingsWindowBase)this).SettingsBtn_Click(sender, (RoutedEventArgs)null);
		}
	}

	protected override void SetPopupOffset()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			Dispatcher dispatcher = ((DispatcherObject)this).Dispatcher;
			Action action = delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Invalid comparison between Unknown and I4
				if ((int)ParentWindow.mTopBar.mSnailMode == 1 && !((SettingsWindowBase)this).IsVtxLearned && ((SettingsWindowBase)this).CheckWidth())
				{
					((Popup)((SettingsWindowBase)this).EnableVTPopup).HorizontalOffset = ((FrameworkElement)((SettingsWindowBase)this).SettingsWindowStackPanel).ActualWidth;
					((FrameworkElement)((SettingsWindowBase)this).EnableVTPopup).Width = ((FrameworkElement)((SettingsWindowBase)this).SettingsWindowGrid).ActualWidth;
					((Popup)((SettingsWindowBase)this).EnableVTPopup).IsOpen = true;
					((Popup)((SettingsWindowBase)this).EnableVTPopup).StaysOpen = true;
				}
			};
			int num = ((1868932653 > 555165518) ? 7 : 9);
			dispatcher.Invoke((Delegate)action, (DispatcherPriority)num, new object[0]);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public override void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Expected O, but got Unknown
		Logger.Info("Clicked settings menu close button");
		bool staysOpen = false;
		bool hasChanges = false;
		UserControl visibleControl = ((SettingsWindowBase)this).visibleControl;
		EngineSettingBase val = (EngineSettingBase)(object)((visibleControl is EngineSettingBase) ? visibleControl : null);
		if (val == null)
		{
			if (!(visibleControl is DisplaySettingsControl displaySettingsControl))
			{
				if (!(visibleControl is GameSettingView gameSettingView))
				{
					if (visibleControl is DeviceProfileControl deviceProfileControl)
					{
						hasChanges = deviceProfileControl.IsDirty();
					}
				}
				else
				{
					GameSettingViewModel gameSettingViewModel = ((FrameworkElement)gameSettingView).DataContext as GameSettingViewModel;
					hasChanges = gameSettingViewModel.IsDirty();
				}
			}
			else
			{
				hasChanges = ((DisplaySettingsBase)displaySettingsControl).IsDirty();
			}
		}
		else
		{
			EngineSettingViewModel engineSettingViewModel = ((FrameworkElement)val).DataContext as EngineSettingViewModel;
			if ((int)((EngineSettingBaseViewModel)engineSettingViewModel).Status == 1)
			{
				Logger.Info("Compatibility check is running");
				return;
			}
			hasChanges = ((EngineSettingBaseViewModel)engineSettingViewModel).IsDirty();
		}
		if (hasChanges)
		{
			CustomMessageWindow val2 = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val2.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val2.BodyTextBlock, string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CLOSE_MESSAGE", ""), new object[1] { "bluestacks" }), "");
			val2.AddButton((ButtonColors)(0x61FE9813 ^ 0x61FE9817), "STRING_NO", (EventHandler)delegate
			{
			}, (string)null, false, (object)null);
			val2.AddButton((ButtonColors)(1035032485 + -1035032483), "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				if (((FrameworkElement)((SettingsWindowBase)this).visibleControl).DataContext is GameSettingViewModel gameSettingViewModel2)
				{
					gameSettingViewModel2.Reset();
				}
				hasChanges = false;
			}, (string)null, false, (object)null);
			((Window)val2).Owner = (Window)(object)ParentWindow;
			((Window)val2).ShowDialog();
		}
		if (hasChanges)
		{
			return;
		}
		GrmHandler.RequirementConfigUpdated(ParentWindow.mVmName);
		if (mIsShortcutEdited && mIsShortcutSaveBtnEnabled)
		{
			CustomMessageWindow val3 = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val3.TitleTextBlock, "STRING_SAVE_CHANGES_QUESTION", "");
			BlueStacksUIBinding.Bind(val3.BodyTextBlock, "STRING_UNSAVED_CHANGES", "");
			val3.AddButton((ButtonColors)(1384120324 + (477951670 << 1207693078)), "STRING_SAVE_CHANGES", (EventHandler)delegate
			{
				ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
				mIsShortcutEdited = false;
			}, (string)null, false, (object)null);
			val3.AddButton((ButtonColors)(-1634588452 - ~1634588453), "STRING_DISCARD", (EventHandler)delegate
			{
				CommonHandlers.ReloadShortcutsForAllInstances();
			}, (string)null, false, (object)null);
			((Window)val3).Owner = (Window)(object)ParentWindow;
			((UIElement)val3.CloseButton).PreviewMouseLeftButtonUp += (MouseButtonEventHandler)delegate
			{
				staysOpen = true;
			};
			((Window)val3).ShowDialog();
		}
		else if (mDuplicateShortcutsList.Count > 0)
		{
			CommonHandlers.ReloadShortcutsForAllInstances();
		}
		if (!staysOpen)
		{
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
		}
	}
}
