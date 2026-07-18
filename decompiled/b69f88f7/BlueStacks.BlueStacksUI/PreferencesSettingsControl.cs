using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class PreferencesSettingsControl : UserControl, IComponentConnector
{
	private Dictionary<string, ComboBoxItem> dictComboBoxItems = new Dictionary<string, ComboBoxItem>();

	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScrollBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mLanguageSettingsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mLanguageCombobox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mLanguagePreferencePaddingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPerformancePreference;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mPerformanceSettingsLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSwitchToHome;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSwitchKillWebTab;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableMemoryTrim;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mEnableMemoryTrimWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGameControlPreferencePaddingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGameControlsSettingsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mGameControlSettingsLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mGameControlsStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableGamePadCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHelpIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mEnableNativeGamepad;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mForcedOnMode;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mForcedOffMode;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mAutoMode;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNativeGamepadInfo;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mShowSchemeDeleteWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPerformancePreferencePaddingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mPlatformStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mAddDesktopShortcuts;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mShowGamingSummary;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mShowMacroDeleteWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mDiscordCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableAdbCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mEnableAdbWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mInputGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableTouchPointsCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableTouchCoordinatesCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mEnableDevSettingsWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mQuitOptionsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mQuitOptionsComboBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mShowOnExitCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mScreenshotGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mScreenShotPathLable;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mChangePathBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChangeLocaleGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mInfoIconLocale;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChangePrefGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mInfoIcon;

	private bool _contentLoaded;

	public PreferencesSettingsControl(MainWindow window)
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		InitializeComponent();
		((UIElement)mChangePrefGrid).Visibility = (Visibility)0;
		((UIElement)mChangeLocaleGrid).Visibility = (Visibility)2;
		ParentWindow = window;
		((UIElement)this).Visibility = (Visibility)1;
		InitSettings();
		AddLanguages();
		AddQuitOptions();
		if (!FeatureManager.Instance.IsShowLanguagePreference)
		{
			((UIElement)mLanguageSettingsGrid).Visibility = (Visibility)2;
			((UIElement)mLanguagePreferencePaddingGrid).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsShowDesktopShortcutPreference)
		{
			((UIElement)mAddDesktopShortcuts).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsShowGamingSummaryPreference)
		{
			((UIElement)mShowGamingSummary).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsShowPerformancePreference)
		{
			((UIElement)mPerformancePreference).Visibility = (Visibility)2;
			((UIElement)mPerformancePreferencePaddingGrid).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsShowDiscordPreference)
		{
			((UIElement)mDiscordCheckBox).Visibility = (Visibility)2;
		}
		if (!FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)mQuitOptionsGrid).Visibility = (Visibility)2;
		}
		mScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
	}

	private void AddQuitOptions()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		ComboBoxItem val = null;
		string[] exitOptions = LocaleStringsConstants.ExitOptions;
		foreach (string text in exitOptions)
		{
			ComboBoxItem val2 = new ComboBoxItem
			{
				Content = LocaleStrings.GetLocalizedString(text, ""),
				Tag = text
			};
			((ItemsControl)mQuitOptionsComboBox).Items.Add((object)val2);
			if (text == RegistryManager.Instance.QuitDefaultOption)
			{
				val = val2;
			}
		}
		exitOptions = LocaleStringsConstants.RestartOptions;
		foreach (string text2 in exitOptions)
		{
			ComboBoxItem val3 = new ComboBoxItem
			{
				Content = LocaleStrings.GetLocalizedString(text2, "")
			};
			((ItemsControl)mQuitOptionsComboBox).Items.Add((object)val3);
			((FrameworkElement)val3).Tag = text2;
			if (text2 == RegistryManager.Instance.QuitDefaultOption)
			{
				val = val3;
			}
		}
		if (val == null)
		{
			((Selector)mQuitOptionsComboBox).SelectedIndex = 0;
		}
		else
		{
			((Selector)mQuitOptionsComboBox).SelectedItem = val;
		}
	}

	private void AddLanguages()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		foreach (string key in Globalization.sSupportedLocales.Keys)
		{
			ComboBoxItem val = new ComboBoxItem
			{
				Content = Globalization.sSupportedLocales[key].ToString(CultureInfo.InvariantCulture)
			};
			dictComboBoxItems.Add(((ContentControl)val).Content.ToString(), val);
			((ItemsControl)mLanguageCombobox).Items.Add((object)val);
		}
		SelectDefaultValue();
	}

	private void SelectDefaultValue()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		((Selector)mLanguageCombobox).SelectionChanged -= new SelectionChangedEventHandler(mLanguageCombobox_SelectionChanged);
		string text = RegistryManager.Instance.UserSelectedLocale;
		if (string.IsNullOrEmpty(text))
		{
			text = LocaleStrings.GetLocaleName("Android", false);
			RegistryManager.Instance.UserSelectedLocale = text;
		}
		else if (!Globalization.sSupportedLocales.ContainsKey(text))
		{
			string locale = text;
			text = "en-US";
			string text2 = Globalization.sSupportedLocales.Keys.FirstOrDefault((string x) => x.StartsWith(locale.Substring(0, 2), StringComparison.InvariantCulture));
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		((Selector)mLanguageCombobox).SelectedItem = dictComboBoxItems[Globalization.sSupportedLocales[text].ToString(CultureInfo.InvariantCulture)];
		((Selector)mLanguageCombobox).SelectionChanged += new SelectionChangedEventHandler(mLanguageCombobox_SelectionChanged);
	}

	private void InitSettings()
	{
		if (!ParentWindow.IsDefaultVM)
		{
			((UIElement)mAddDesktopShortcuts).Visibility = (Visibility)2;
		}
		if (RegistryManager.Instance.AddDesktopShortcuts)
		{
			((ToggleButton)mAddDesktopShortcuts).IsChecked = true;
		}
		if (RegistryManager.Instance.SwitchToAndroidHome)
		{
			((ToggleButton)mSwitchToHome).IsChecked = true;
		}
		else
		{
			((ToggleButton)mSwitchToHome).IsChecked = false;
		}
		if (RegistryManager.Instance.SwitchKillWebTab)
		{
			((ToggleButton)mSwitchKillWebTab).IsChecked = true;
		}
		else
		{
			((ToggleButton)mSwitchKillWebTab).IsChecked = false;
		}
		((ToggleButton)mEnableMemoryTrim).IsChecked = RegistryManager.Instance.EnableMemoryTrim;
		if (RegistryManager.Instance.ShowGamingSummary)
		{
			((ToggleButton)mShowGamingSummary).IsChecked = true;
		}
		else
		{
			((ToggleButton)mShowGamingSummary).IsChecked = false;
		}
		if (FeatureManager.Instance.IsMacroRecorderEnabled)
		{
			((ToggleButton)mShowMacroDeleteWarning).IsChecked = ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup;
		}
		else
		{
			((UIElement)mShowMacroDeleteWarning).Visibility = (Visibility)2;
		}
		((ToggleButton)mShowSchemeDeleteWarning).IsChecked = ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup;
		if (PromotionObject.Instance != null && !string.IsNullOrEmpty(PromotionObject.Instance.DiscordClientID) && ParentWindow.IsDefaultVM)
		{
			((UIElement)mDiscordCheckBox).Visibility = (Visibility)0;
			if (RegistryManager.Instance.DiscordEnabled)
			{
				((ToggleButton)mDiscordCheckBox).IsChecked = true;
			}
			else
			{
				((ToggleButton)mDiscordCheckBox).IsChecked = false;
			}
		}
		else
		{
			((UIElement)mDiscordCheckBox).Visibility = (Visibility)2;
		}
		((ToggleButton)mEnableGamePadCheckbox).IsChecked = RegistryManager.Instance.GamepadDetectionEnabled;
		if (((ToggleButton)mEnableGamePadCheckbox).IsChecked == true)
		{
			((UIElement)mEnableNativeGamepad).IsEnabled = true;
		}
		else
		{
			((UIElement)mEnableNativeGamepad).IsEnabled = false;
		}
		InitNativeGamepadSettings();
		if (FeatureManager.Instance.AllowADBSettingToggle)
		{
			try
			{
				if (ParentWindow.mGuestBootCompleted)
				{
					CheckIfAdbIsEnabled();
				}
				else
				{
					((UIElement)mEnableAdbCheckBox).Visibility = (Visibility)2;
					((UIElement)mEnableAdbWarning).Visibility = (Visibility)2;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when initialising adb checkbox " + ex.ToString());
				((UIElement)mEnableAdbCheckBox).Visibility = (Visibility)2;
				((UIElement)mEnableAdbWarning).Visibility = (Visibility)2;
			}
		}
		else
		{
			((UIElement)mEnableAdbCheckBox).Visibility = (Visibility)2;
			((UIElement)mEnableAdbWarning).Visibility = (Visibility)2;
		}
		if (FeatureManager.Instance.IsShowAndroidInputDebugSetting)
		{
			try
			{
				if (ParentWindow.mGuestBootCompleted)
				{
					CheckIfAndroidTouchPointsEnabled();
				}
				else
				{
					((UIElement)mInputGrid).Visibility = (Visibility)2;
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception when initialising android input debugging checkbox " + ex2.ToString());
				((UIElement)mInputGrid).Visibility = (Visibility)2;
			}
		}
		else
		{
			((UIElement)mInputGrid).Visibility = (Visibility)2;
		}
		if (StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
		{
			mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
		}
		else
		{
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Strings.ProductTopBarDisplayName);
				Logger.Info("Path to save screenshots and recording is ", new object[1] { text });
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				RegistryManager.Instance.ScreenShotsPath = text;
				mScreenShotPathLable.Text = text;
			}
			catch (Exception ex3)
			{
				Logger.Error("Exception while creating picutres directory: " + ex3.ToString());
			}
		}
		if (!RegistryManager.Instance.Guest[ParentWindow.mVmName].IsGoogleSigninDone)
		{
			((UIElement)mLanguageSettingsGrid).Visibility = (Visibility)2;
			((UIElement)mLanguagePreferencePaddingGrid).Visibility = (Visibility)2;
		}
		((ToggleButton)mShowOnExitCheckbox).IsChecked = !RegistryManager.Instance.IsQuitOptionSaved;
	}

	private void InitNativeGamepadSettings(bool isUpdate = false)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected I4, but got Unknown
		string value = "false";
		NativeGamepadState nativeGamepadState = ParentWindow.EngineInstanceRegistry.NativeGamepadState;
		switch ((int)nativeGamepadState)
		{
		case 0:
			((ToggleButton)mForcedOnMode).IsChecked = true;
			value = "true";
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)0;
			break;
		case 1:
			((ToggleButton)mForcedOffMode).IsChecked = true;
			value = "false";
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)2;
			break;
		case 2:
			((ToggleButton)mAutoMode).IsChecked = true;
			if (ParentWindow.mGuestBootCompleted)
			{
				value = ParentWindow.mCommonHandler.CheckNativeGamepadState(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName).ToString(CultureInfo.InvariantCulture);
			}
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)2;
			break;
		}
		if (isUpdate)
		{
			Dictionary<string, string> data = new Dictionary<string, string> { { "isEnabled", value } };
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", data);
		}
	}

	private void CheckIfAdbIsEnabled()
	{
		string text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ADB_CONNECTED_PORT_0", ""), new object[1] { ParentWindow.EngineInstanceRegistry.BstAdbPort });
		mEnableAdbWarning.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2]
		{
			LocaleStrings.GetLocalizedString("STRING_ENABLE_ADB_WARNING", ""),
			text
		});
		object obj = JsonConvert.DeserializeObject(HTTPUtils.SendRequestToGuest("checkADBStatus", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"), Utils.GetSerializerSettings());
		JObject val = (JObject)((obj is JObject) ? obj : null);
		if (string.Compare("ok", Extensions.Value<string>((IEnumerable<JToken>)val["result"]), StringComparison.OrdinalIgnoreCase) == 0)
		{
			((ToggleButton)mEnableAdbCheckBox).IsChecked = true;
		}
		else
		{
			((ToggleButton)mEnableAdbCheckBox).IsChecked = false;
		}
		((UIElement)mEnableAdbCheckBox).Visibility = (Visibility)0;
		((UIElement)mEnableAdbWarning).Visibility = (Visibility)0;
	}

	private void CheckIfAndroidTouchPointsEnabled()
	{
		JObject obj = JObject.Parse(HTTPUtils.SendRequestToGuest("checkTouchPointState", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
		string b = ((object)obj["touchPoint"]).ToString().Trim();
		string b2 = ((object)obj["pointerLocation"]).ToString().Trim();
		((ToggleButton)mEnableTouchPointsCheckBox).IsChecked = string.Equals("enabled", b, StringComparison.InvariantCultureIgnoreCase);
		((ToggleButton)mEnableTouchCoordinatesCheckbox).IsChecked = string.Equals("enabled", b2, StringComparison.InvariantCultureIgnoreCase);
		((UIElement)mInputGrid).Visibility = (Visibility)0;
	}

	private void CheckBox_Click(object sender, RoutedEventArgs e)
	{
		((UIElement)mChangeLocaleGrid).Visibility = (Visibility)2;
		((UIElement)mChangePrefGrid).Visibility = (Visibility)0;
		CustomCheckbox val = (CustomCheckbox)((sender is CustomCheckbox) ? sender : null);
		if (val == mAddDesktopShortcuts)
		{
			if (true == ((ToggleButton)mAddDesktopShortcuts).IsChecked)
			{
				RegistryManager.Instance.AddDesktopShortcuts = true;
			}
			else
			{
				RegistryManager.Instance.AddDesktopShortcuts = false;
			}
		}
		else if (val == mShowGamingSummary)
		{
			ClientStats.SendMiscellaneousStatsAsync("gamingSummaryCheckboxClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "checked" + ((ToggleButton)mShowGamingSummary).IsChecked, null, null);
			if (((ToggleButton)mShowGamingSummary).IsChecked == true)
			{
				RegistryManager.Instance.ShowGamingSummary = true;
			}
			else
			{
				RegistryManager.Instance.ShowGamingSummary = false;
			}
		}
		else if (val == mDiscordCheckBox)
		{
			if (((ToggleButton)mDiscordCheckBox).IsChecked == true)
			{
				RegistryManager.Instance.DiscordEnabled = true;
				if (ParentWindow.mAppHandler.IsOneTimeSetupCompleted && ParentWindow.mGuestBootCompleted)
				{
					if (ParentWindow.mDiscordhandler == null)
					{
						ParentWindow.InitDiscord();
					}
					else
					{
						ParentWindow.mDiscordhandler.ToggleDiscordState(state: true);
					}
				}
			}
			else
			{
				RegistryManager.Instance.DiscordEnabled = false;
				if (ParentWindow.mDiscordhandler != null)
				{
					ParentWindow.mDiscordhandler.ToggleDiscordState(state: false);
				}
				ParentWindow.mDiscordhandler = null;
			}
		}
		else if (val == mEnableAdbCheckBox)
		{
			HTTPUtils.SendRequestToGuestAsync((((ToggleButton)mEnableAdbCheckBox).IsChecked == true) ? "connectHost?d=permanent" : "disconnectHost?d=permanent", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
			if (((ToggleButton)mEnableAdbCheckBox).IsChecked == true && ((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(ParentWindow.mVmName))
			{
				((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROBED, "");
			}
		}
		else if (val == mEnableTouchPointsCheckBox)
		{
			string text = ((((ToggleButton)mEnableTouchPointsCheckBox).IsChecked == true) ? "enable" : "disable");
			string text2 = ((((ToggleButton)mEnableTouchCoordinatesCheckbox).IsChecked == true) ? "enable" : "disable");
			string text3 = "{";
			text3 += string.Format(CultureInfo.InvariantCulture, "\"touchPoint\":\"{0}\",", new object[1] { text });
			text3 += string.Format(CultureInfo.InvariantCulture, "\"pointerLocation\":\"{0}\"", new object[1] { text2 });
			text3 += "}";
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "data", text3 } };
			HTTPUtils.SendRequestToGuest("showTouchPoints", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		else if (val == mEnableTouchCoordinatesCheckbox)
		{
			string text4 = ((((ToggleButton)mEnableTouchPointsCheckBox).IsChecked == true) ? "enable" : "disable");
			string text5 = ((((ToggleButton)mEnableTouchCoordinatesCheckbox).IsChecked == true) ? "enable" : "disable");
			string text6 = "{";
			text6 += string.Format(CultureInfo.InvariantCulture, "\"touchPoint\":\"{0}\",", new object[1] { text4 });
			text6 += string.Format(CultureInfo.InvariantCulture, "\"pointerLocation\":\"{0}\"", new object[1] { text5 });
			text6 += "}";
			Dictionary<string, string> dictionary2 = new Dictionary<string, string> { { "data", text6 } };
			HTTPUtils.SendRequestToGuest("showTouchPoints", dictionary2, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		else if (val == mShowMacroDeleteWarning)
		{
			ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = ((ToggleButton)mShowMacroDeleteWarning).IsChecked.Value;
		}
		else if (val == mShowSchemeDeleteWarning)
		{
			ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup = ((ToggleButton)mShowSchemeDeleteWarning).IsChecked.Value;
		}
		else if (val == mEnableGamePadCheckbox)
		{
			if (((ToggleButton)mEnableGamePadCheckbox).IsChecked == true)
			{
				RegistryManager.Instance.GamepadDetectionEnabled = true;
				((UIElement)mEnableNativeGamepad).IsEnabled = true;
				InitNativeGamepadSettings(isUpdate: true);
			}
			else
			{
				RegistryManager.Instance.GamepadDetectionEnabled = false;
				Dictionary<string, string> data = new Dictionary<string, string> { 
				{
					"isEnabled",
					RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture)
				} };
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", data);
				((UIElement)mEnableNativeGamepad).IsEnabled = false;
			}
			Dictionary<string, string> data2 = new Dictionary<string, string> { 
			{
				"enable",
				RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture)
			} };
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", data2);
		}
	}

	private void mLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			object selectedItem = ((Selector)mLanguageCombobox).SelectedItem;
			string selectedLocale = ((ContentControl)((selectedItem is ComboBoxItem) ? selectedItem : null)).Content.ToString();
			if (selectedLocale != null)
			{
				((UIElement)mChangePrefGrid).Visibility = (Visibility)0;
				string key = Globalization.sSupportedLocales.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedLocale).Key;
				if (!string.Equals(RegistryManager.Instance.UserSelectedLocale, key, StringComparison.InvariantCultureIgnoreCase))
				{
					RegistryManager.Instance.UserSelectedLocale = key;
					BlueStacksUIUtils.UpdateLocale(key);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set locale" + ex.ToString());
		}
	}

	private void mSwitchToHome_Click(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mSwitchToHome).IsChecked == true)
		{
			RegistryManager.Instance.SwitchToAndroidHome = true;
		}
		else
		{
			RegistryManager.Instance.SwitchToAndroidHome = false;
		}
	}

	private void SwitchKillWebTab_Click(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mSwitchKillWebTab).IsChecked == true)
		{
			RegistryManager.Instance.SwitchKillWebTab = true;
		}
		else
		{
			RegistryManager.Instance.SwitchKillWebTab = false;
		}
	}

	private void mChangePathBtn_Click(object sender, RoutedEventArgs e)
	{
		string text = mScreenShotPathLable.Text;
		ParentWindow.mCommonHandler.ShowFolderBrowserDialog(text);
		mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
		ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPreferences", text, RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void MQuitOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		RegistryManager instance = RegistryManager.Instance;
		object selectedItem = ((Selector)mQuitOptionsComboBox).SelectedItem;
		instance.QuitDefaultOption = ((FrameworkElement)((selectedItem is ComboBoxItem) ? selectedItem : null)).Tag.ToString();
	}

	private void MShowOnExitCheckbox_Click(object sender, RoutedEventArgs e)
	{
		CustomCheckbox val = (CustomCheckbox)((sender is CustomCheckbox) ? sender : null);
		RegistryManager.Instance.IsQuitOptionSaved = !((ToggleButton)val).IsChecked.Value;
	}

	private void HelpIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=native_gamepad_help");
	}

	private void NativeGamepadMode_Click(object sender, RoutedEventArgs e)
	{
		string value = string.Empty;
		switch (((FrameworkElement)((sender is CustomRadioButton) ? sender : null)).Name)
		{
		case "mForcedOnMode":
			value = RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture);
			ParentWindow.EngineInstanceRegistry.NativeGamepadState = (NativeGamepadState)0;
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)0;
			ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "ForcedOn", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			break;
		case "mForcedOffMode":
			value = "false";
			ParentWindow.EngineInstanceRegistry.NativeGamepadState = (NativeGamepadState)1;
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)2;
			ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "ForcedOff", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			break;
		case "mAutoMode":
			if (ParentWindow.mGuestBootCompleted)
			{
				value = ParentWindow.mCommonHandler.CheckNativeGamepadState(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName).ToString(CultureInfo.InvariantCulture);
			}
			ParentWindow.EngineInstanceRegistry.NativeGamepadState = (NativeGamepadState)2;
			((UIElement)mNativeGamepadInfo).Visibility = (Visibility)2;
			ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "Auto", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
			break;
		}
		Dictionary<string, string> data = new Dictionary<string, string> { { "isEnabled", value } };
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", data);
	}

	private void EnableMemoryTrim_Click(object sender, RoutedEventArgs e)
	{
		RegistryManager.Instance.EnableMemoryTrim = ((ToggleButton)mEnableMemoryTrim).IsChecked == true;
		if (((ToggleButton)mEnableMemoryTrim).IsChecked != true)
		{
			return;
		}
		foreach (string item in BlueStacksUIUtils.DictWindows.Keys.ToList())
		{
			HTTPUtils.SendRequestToEngineAsync("enableMemoryTrim", (Dictionary<string, string>)null, item, 0, (Dictionary<string, string>)null, false, 1, 0);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/preferencessettingscontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Expected O, but got Unknown
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Expected O, but got Unknown
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Expected O, but got Unknown
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Expected O, but got Unknown
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Expected O, but got Unknown
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Expected O, but got Unknown
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Expected O, but got Unknown
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Expected O, but got Unknown
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Expected O, but got Unknown
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Expected O, but got Unknown
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Expected O, but got Unknown
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Expected O, but got Unknown
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Expected O, but got Unknown
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Expected O, but got Unknown
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Expected O, but got Unknown
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Expected O, but got Unknown
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Expected O, but got Unknown
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Expected O, but got Unknown
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Expected O, but got Unknown
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Expected O, but got Unknown
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Expected O, but got Unknown
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mScrollBar = (ScrollViewer)target;
			break;
		case 2:
			mMainGrid = (Grid)target;
			break;
		case 3:
			mLanguageSettingsGrid = (Grid)target;
			((UIElement)mLanguageSettingsGrid).Visibility = (Visibility)2;
			break;
		case 4:
			mLanguageCombobox = (CustomComboBox)target;
			((Selector)mLanguageCombobox).SelectionChanged += new SelectionChangedEventHandler(mLanguageCombobox_SelectionChanged);
			((UIElement)mLanguageCombobox).Visibility = (Visibility)2;
			break;
		case 5:
			mLanguagePreferencePaddingGrid = (Grid)target;
			((UIElement)mLanguagePreferencePaddingGrid).Visibility = (Visibility)2;
			break;
		case 6:
			mPerformancePreference = (Grid)target;
			((UIElement)mPerformancePreference).Visibility = (Visibility)2;
			break;
		case 7:
			mPerformanceSettingsLabel = (Label)target;
			((UIElement)mPerformanceSettingsLabel).Visibility = (Visibility)2;
			break;
		case 8:
			mSwitchToHome = (CustomCheckbox)target;
			((ButtonBase)mSwitchToHome).Click += new RoutedEventHandler(mSwitchToHome_Click);
			((UIElement)mSwitchToHome).Visibility = (Visibility)2;
			break;
		case 9:
			mSwitchKillWebTab = (CustomCheckbox)target;
			((ButtonBase)mSwitchKillWebTab).Click += new RoutedEventHandler(SwitchKillWebTab_Click);
			((UIElement)mSwitchKillWebTab).Visibility = (Visibility)2;
			break;
		case 10:
			mEnableMemoryTrim = (CustomCheckbox)target;
			((ButtonBase)mEnableMemoryTrim).Click += new RoutedEventHandler(EnableMemoryTrim_Click);
			((UIElement)mEnableMemoryTrim).Visibility = (Visibility)2;
			break;
		case 11:
			mEnableMemoryTrimWarning = (TextBlock)target;
			((UIElement)mEnableMemoryTrimWarning).Visibility = (Visibility)2;
			break;
		case 12:
			mGameControlPreferencePaddingGrid = (Grid)target;
			break;
		case 13:
			mGameControlsSettingsGrid = (Grid)target;
			break;
		case 14:
			mGameControlSettingsLabel = (Label)target;
			break;
		case 15:
			mGameControlsStackPanel = (StackPanel)target;
			break;
		case 16:
			mEnableGamePadCheckbox = (CustomCheckbox)target;
			((ButtonBase)mEnableGamePadCheckbox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 17:
			mHelpIcon = (CustomPictureBox)target;
			((UIElement)mHelpIcon).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(HelpIconPreviewMouseLeftButtonUp);
			break;
		case 18:
			mEnableNativeGamepad = (Grid)target;
			break;
		case 19:
			mForcedOnMode = (CustomRadioButton)target;
			((ButtonBase)mForcedOnMode).Click += new RoutedEventHandler(NativeGamepadMode_Click);
			break;
		case 20:
			mForcedOffMode = (CustomRadioButton)target;
			((ButtonBase)mForcedOffMode).Click += new RoutedEventHandler(NativeGamepadMode_Click);
			break;
		case 21:
			mAutoMode = (CustomRadioButton)target;
			((ButtonBase)mAutoMode).Click += new RoutedEventHandler(NativeGamepadMode_Click);
			break;
		case 22:
			mNativeGamepadInfo = (Grid)target;
			break;
		case 23:
			mShowSchemeDeleteWarning = (CustomCheckbox)target;
			((ButtonBase)mShowSchemeDeleteWarning).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 24:
			mPerformancePreferencePaddingGrid = (Grid)target;
			break;
		case 25:
			mPlatformStackPanel = (StackPanel)target;
			break;
		case 26:
			mAddDesktopShortcuts = (CustomCheckbox)target;
			((ButtonBase)mAddDesktopShortcuts).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 27:
			mShowGamingSummary = (CustomCheckbox)target;
			((ButtonBase)mShowGamingSummary).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 28:
			mShowMacroDeleteWarning = (CustomCheckbox)target;
			((ButtonBase)mShowMacroDeleteWarning).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 29:
			mDiscordCheckBox = (CustomCheckbox)target;
			((ButtonBase)mDiscordCheckBox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 30:
			mEnableAdbCheckBox = (CustomCheckbox)target;
			((ButtonBase)mEnableAdbCheckBox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 31:
			mEnableAdbWarning = (TextBlock)target;
			break;
		case 32:
			mInputGrid = (Grid)target;
			break;
		case 33:
			mEnableTouchPointsCheckBox = (CustomCheckbox)target;
			((ButtonBase)mEnableTouchPointsCheckBox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 34:
			mEnableTouchCoordinatesCheckbox = (CustomCheckbox)target;
			((ButtonBase)mEnableTouchCoordinatesCheckbox).Click += new RoutedEventHandler(CheckBox_Click);
			break;
		case 35:
			mEnableDevSettingsWarning = (TextBlock)target;
			break;
		case 36:
			mQuitOptionsGrid = (Grid)target;
			break;
		case 37:
			mQuitOptionsComboBox = (CustomComboBox)target;
			((Selector)mQuitOptionsComboBox).SelectionChanged += new SelectionChangedEventHandler(MQuitOptionsComboBox_SelectionChanged);
			break;
		case 38:
			mShowOnExitCheckbox = (CustomCheckbox)target;
			((ButtonBase)mShowOnExitCheckbox).Click += new RoutedEventHandler(MShowOnExitCheckbox_Click);
			break;
		case 39:
			mScreenshotGrid = (Grid)target;
			break;
		case 40:
			mScreenShotPathLable = (TextBlock)target;
			break;
		case 41:
			mChangePathBtn = (CustomButton)target;
			((ButtonBase)mChangePathBtn).Click += new RoutedEventHandler(mChangePathBtn_Click);
			break;
		case 42:
			mChangeLocaleGrid = (Grid)target;
			break;
		case 43:
			mInfoIconLocale = (CustomPictureBox)target;
			break;
		case 44:
			mChangePrefGrid = (Grid)target;
			break;
		case 45:
			mInfoIcon = (CustomPictureBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
