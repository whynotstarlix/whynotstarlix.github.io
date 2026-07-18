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

	public MainWindow ParentWindow { get; set; }

	public SettingsWindow(MainWindow window, string startUpTab)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		SettingsWindow settingsWindow = this;
		mDuplicateShortcutsList = new List<string>();
		mSettingsButtons = new Dictionary<string, CustomSettingsButton>();
		ParentWindow = window;
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_DISPLAY_SETTINGS");
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_ENGINE_SETTING");
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_GRAPHICS");
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_CROSSHAIR");
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_SETTINGS");
		((SettingsWindowBase)this).SettingsControlNameList.Add("STRING_ABOUT_SETTING");
		UpdateSettingsListAndStartTabForCustomOEMs();
		((FrameworkElement)this).Loaded += (RoutedEventHandler)delegate
		{
			settingsWindow.SettingsWindow_Loaded(window);
		};
		if (!string.IsNullOrEmpty(startUpTab))
		{
			((SettingsWindowBase)this).StartUpTab = startUpTab;
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
			tab = "STRING_DISPLAY_SETTINGS";
		}
		((SettingsWindowBase)this).AddControlInGridAndDict(tab, userControl);
		((SettingsWindowBase)this).BringToFront(userControl);
		if (mSettingsButtons.TryGetValue(tab, out var value) && !value.IsSelected)
		{
			value.IsSelected = true;
			((UIElement)value).IsEnabled = true;
		}
	}

	public void UpdateSettingsListAndStartTabForCustomOEMs()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
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
		switch (controlName)
		{
		case "STRING_ENGINE_SETTING":
			return (UserControl)(object)GetEngineView(window);
		case "STRING_DISPLAY_SETTINGS":
			return (UserControl)(object)new DisplaySettingsControl(window);
		case "STRING_GRAPHICS":
			return (UserControl)(object)new GraphicsSettingControl(window);
		case "STRING_ABOUT_SETTING":
			return (UserControl)(object)new AboutSettingsControl(window, this);
		case "STRING_CROSSHAIR":
		{
			CrosshairsSettingsControl crosshairsSettingsControl = new CrosshairsSettingsControl(window);
			crosshairsSettingsControl.LoadConfigFromString();
			return (UserControl)(object)crosshairsSettingsControl;
		}
		case "STRING_SETTINGS":
			return (UserControl)(object)new Options(window);
		default:
			return null;
		}
	}

	private static GameSettingView GetGameSettingView(MainWindow window)
	{
		GameSettingViewModel gameSettingViewModel = new GameSettingViewModel(window);
		GameSettingView gameSettingView = new GameSettingView();
		((UIElement)gameSettingView).Visibility = (Visibility)2;
		((FrameworkElement)gameSettingView).DataContext = gameSettingViewModel;
		return gameSettingViewModel.View = gameSettingView;
	}

	private static EngineSettingBase GetEngineView(MainWindow window)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		EngineSettingBase val = new EngineSettingBase
		{
			Visibility = (Visibility)2
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
			string text = "Exception in SettingsWindowClosing. Exception: ";
			Logger.Error(text + ex);
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
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
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
						WindowStartupLocation = (WindowStartupLocation)2
					};
					BlueStacksUIBinding.Bind(val4.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
					BlueStacksUIBinding.Bind(val4.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
					val4.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
					{
						((RoutedEventArgs)args).Handled = true;
					}, (string)null, false, (object)null);
					val4.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
				WindowStartupLocation = (WindowStartupLocation)2
			};
			BlueStacksUIBinding.Bind(val5.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val5.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
			val5.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
			{
				((RoutedEventArgs)args).Handled = true;
			}, (string)null, false, (object)null);
			val5.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
					WindowStartupLocation = (WindowStartupLocation)2
				};
				BlueStacksUIBinding.Bind(val6.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(val6.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
				val6.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
				{
					((RoutedEventArgs)args).Handled = true;
				}, (string)null, false, (object)null);
				val6.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
				Owner = (Window)(object)ParentWindow,
				WindowStartupLocation = (WindowStartupLocation)2
			};
			BlueStacksUIBinding.Bind(val7.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
			BlueStacksUIBinding.Bind(val7.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
			val7.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
			{
				((RoutedEventArgs)args).Handled = true;
			}, (string)null, false, (object)null);
			val7.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
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
			}, (DispatcherPriority)7, new object[0]);
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
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
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
			val2.AddButton((ButtonColors)4, "STRING_NO", (EventHandler)delegate
			{
			}, (string)null, false, (object)null);
			val2.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
			val3.AddButton((ButtonColors)4, "STRING_SAVE_CHANGES", (EventHandler)delegate
			{
				ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
				mIsShortcutEdited = false;
			}, (string)null, false, (object)null);
			val3.AddButton((ButtonColors)2, "STRING_DISCARD", (EventHandler)delegate
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
