using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;
using Bluester;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class Options : UserControl
{
	public static class OptionsSettingsManager
	{
		public class OptionsConfig
		{
			public bool AutolockEnabled { get; set; } = true;

			public bool LockEnabled { get; set; } = true;

			public string LockKeybind { get; set; } = "F6";

			public string LockValue { get; set; } = "1";

			public bool UnlockEnabled { get; set; } = true;

			public string UnlockKeybind { get; set; } = "F7";

			public string UnlockValue { get; set; } = "999";

			public string AutolockValue { get; set; } = "30";

			public bool FreezeEnabled { get; set; } = true;

			public string FreezeKeybind { get; set; } = "F8";

			public bool UnfreezeEnabled { get; set; } = true;

			public string UnfreezeKeybind { get; set; } = "F9";

			public bool KillEnabled { get; set; } = true;

			public string KillKeybind { get; set; } = "F10";

			public bool AutoAcceptEnabled { get; set; }
		}

		private static readonly string SettingsFilePath;

		public static event Action OnSettingsSaved;

		public static OptionsConfig Load()
		{
			try
			{
				if (!File.Exists(SettingsFilePath))
				{
					return new OptionsConfig();
				}
				return JsonConvert.DeserializeObject<OptionsConfig>(File.ReadAllText(SettingsFilePath)) ?? new OptionsConfig();
			}
			catch
			{
				return new OptionsConfig();
			}
		}

		public static void Save(OptionsConfig config)
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
				string contents = JsonConvert.SerializeObject((object)config, (Formatting)1);
				File.WriteAllText(SettingsFilePath, contents);
				OptionsSettingsManager.OnSettingsSaved?.Invoke();
			}
			catch
			{
			}
		}

		static OptionsSettingsManager()
		{
			SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bluester", "settings.json");
		}

		public static void EnsureCreated()
		{
			if (!File.Exists(SettingsFilePath))
			{
				Save(new OptionsConfig());
			}
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static TextCompositionEventHandler _003C_003E9__13_0;

		public static DataObjectPastingEventHandler _003C_003E9__13_2;

		internal void _003CSetupTextBox_003Eb__13_0(object s, TextCompositionEventArgs e)
		{
			((RoutedEventArgs)e).Handled = !char.IsDigit(e.Text, 0);
		}

		internal void _003CSetupTextBox_003Eb__13_2(object s, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetData(typeof(string)) is string s2 && !int.TryParse(s2, out var _))
			{
				((DataObjectEventArgs)e).CancelCommand();
			}
		}
	}

	private Grid adbGrid;

	private CustomCheckbox adbCheckbox;

	private Button _keybindButtonToChange;

	private CustomCheckbox autolockCheckbox;

	private CustomCheckbox lockCheckbox;

	private Button lockKeybindButton;

	private TextBox lockValueTextBox;

	private CustomCheckbox unlockCheckbox;

	private Button unlockKeybindButton;

	private TextBox unlockValueTextBox;

	private bool _isLoading;

	private string _oldKeybindValue;

	private TextBox autolockValueTextBox;

	public static bool IsBindingKey;

	private TextBox dpiValueTextBox;

	private CustomCheckbox freezeCheckbox;

	private Button freezeKeybindButton;

	private CustomCheckbox unfreezeCheckbox;

	private Button unfreezeKeybindButton;

	private CustomCheckbox killCheckbox;

	private Button killKeybindButton;

	private CustomCheckbox autoAcceptCheckbox;

	private Dictionary<string, ComboBoxItem> dictComboBoxItems;

	private bool _isRebuilding;

	private CustomComboBox mLanguageCombobox;

	public MainWindow ParentWindow { get; set; }

	public Options(MainWindow window)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		dictComboBoxItems = new Dictionary<string, ComboBoxItem>();
		ParentWindow = window;
		((FrameworkElement)this).Width = 530.0;
		((FrameworkElement)this).Height = 480.0;
		((UIElement)this).Visibility = (Visibility)1;
		((UIElement)this).Focusable = true;
		LocaleStrings.SourceUpdatedEvent += OnLocaleUpdated;
		RebuildUI();
		((UIElement)this).KeyDown += new KeyEventHandler(Options_KeyDown);
		((UIElement)this).KeyUp += new KeyEventHandler(Options_KeyUp);
	}

	private UIElement Layout()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		StackPanel val = new StackPanel
		{
			Orientation = (Orientation)1,
			Margin = new Thickness(10.0, 5.0, 10.0, 20.0)
		};
		Grid.SetIsSharedSizeScope((UIElement)val, true);
		((Panel)val).Children.Add((UIElement)(object)CreateSectionTitle(LocaleStrings.GetLocalizedString("STRING_OPTIONS", "Options")));
		((Panel)val).Children.Add(CreateAutolockRow());
		((Panel)val).Children.Add(CreateAutoAcceptRow());
		((Panel)val).Children.Add(CreateKeybindRow(LocaleStrings.GetLocalizedString("STRING_LOCK", "Lock"), "lock"));
		((Panel)val).Children.Add(CreateKeybindRow(LocaleStrings.GetLocalizedString("STRING_UNLOCK", "Unlock"), "unlock"));
		((Panel)val).Children.Add(CreateKeybindRow(LocaleStrings.GetLocalizedString("STRING_FREEZE", "Freeze game"), "freeze"));
		((Panel)val).Children.Add(CreateKeybindRow(LocaleStrings.GetLocalizedString("STRING_UNFREEZE", "Unfreeze game"), "unfreeze"));
		((Panel)val).Children.Add(CreateKeybindRow(LocaleStrings.GetLocalizedString("STRING_CLOSE_GAME", "Close game"), "kill"));
		((Panel)val).Children.Add((UIElement)new Separator
		{
			Opacity = 0.2,
			Margin = new Thickness(0.0, 10.0, 0.0, 10.0)
		});
		((Panel)val).Children.Add(CreateDpiRow());
		((Panel)val).Children.Add((UIElement)new Separator
		{
			Opacity = 0.2,
			Margin = new Thickness(0.0, 10.0, 0.0, 10.0)
		});
		((Panel)val).Children.Add((UIElement)(object)CreateSectionTitle(LocaleStrings.GetLocalizedString("STRING_ADVANCED_SETTINGS", "Add. Options")));
		((Panel)val).Children.Add(CreateAdbSection());
		return (UIElement)val;
	}

	private void InitializeAdbSettings()
	{
		if (!FeatureManager.Instance.AllowADBSettingToggle)
		{
			if (adbGrid != null)
			{
				((UIElement)adbGrid).Visibility = (Visibility)2;
			}
		}
		else if (ParentWindow.mGuestBootCompleted)
		{
			CheckIfAdbIsEnabled();
		}
		else
		{
			((UIElement)adbCheckbox).IsEnabled = false;
			string localizedString = LocaleStrings.GetLocalizedString("STRING_WAITING_FOR_BOOT", "Waiting for boot");
			((ContentControl)adbCheckbox).Content = "ADB (" + localizedString + ")";
			((UIElement)adbGrid).Visibility = (Visibility)0;
		}
	}

	private void CheckIfAdbIsEnabled()
	{
		((UIElement)adbCheckbox).IsEnabled = true;
		((ContentControl)adbCheckbox).Content = $"ADB (127.0.0.1:{ParentWindow.EngineInstanceRegistry.BstAdbPort})";
		JObject val = JsonConvert.DeserializeObject<JObject>(HTTPUtils.SendRequestToGuest("checkADBStatus", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
		bool value = string.Equals((val != null) ? ((JToken)val).Value<string>((object)"result") : null, "ok", StringComparison.OrdinalIgnoreCase);
		((ToggleButton)adbCheckbox).IsChecked = value;
		((UIElement)adbGrid).Visibility = (Visibility)0;
	}

	private void AdbCheckBox_Click(object sender, RoutedEventArgs e)
	{
		bool valueOrDefault = ((ToggleButton)adbCheckbox).IsChecked == true;
		HTTPUtils.SendRequestToGuestAsync(valueOrDefault ? "connectHost?d=permanent" : "disconnectHost?d=permanent", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
		if (valueOrDefault && ((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(ParentWindow.mVmName))
		{
			((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROBED, "");
		}
	}

	private void KeybindClick(object sender, RoutedEventArgs e)
	{
		if (_keybindButtonToChange != null)
		{
			((ContentControl)_keybindButtonToChange).Content = _oldKeybindValue;
		}
		_keybindButtonToChange = (Button)((sender is Button) ? sender : null);
		_oldKeybindValue = ((ContentControl)_keybindButtonToChange).Content.ToString();
		((ContentControl)_keybindButtonToChange).Content = "...";
		IsBindingKey = true;
		((UIElement)this).Focus();
	}

	private void Options_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_keybindButtonToChange == null)
		{
			return;
		}
		if ((int)e.Key == 13)
		{
			((ContentControl)_keybindButtonToChange).Content = _oldKeybindValue;
		}
		else
		{
			string text = ((object)e.Key/*cast due to constrained. prefix*/).ToString();
			Button[] array = (Button[])(object)new Button[5] { lockKeybindButton, unlockKeybindButton, freezeKeybindButton, unfreezeKeybindButton, killKeybindButton };
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					Button val = array[num];
					if (val != _keybindButtonToChange && string.Equals(((ContentControl)val).Content?.ToString(), text, StringComparison.OrdinalIgnoreCase))
					{
						((ContentControl)_keybindButtonToChange).Content = _oldKeybindValue;
						break;
					}
					num++;
					continue;
				}
				((ContentControl)_keybindButtonToChange).Content = text;
				break;
			}
		}
		_keybindButtonToChange = null;
		((RoutedEventArgs)e).Handled = true;
		SaveSettings();
	}

	private void LoadSettings()
	{
		OptionsSettingsManager.OptionsConfig optionsConfig = OptionsSettingsManager.Load();
		((ToggleButton)autolockCheckbox).IsChecked = optionsConfig.AutolockEnabled;
		autolockValueTextBox.Text = optionsConfig.AutolockValue;
		((ToggleButton)autoAcceptCheckbox).IsChecked = optionsConfig.AutoAcceptEnabled;
		((ToggleButton)lockCheckbox).IsChecked = optionsConfig.LockEnabled;
		((ContentControl)lockKeybindButton).Content = optionsConfig.LockKeybind;
		lockValueTextBox.Text = optionsConfig.LockValue;
		((ToggleButton)unlockCheckbox).IsChecked = optionsConfig.UnlockEnabled;
		((ContentControl)unlockKeybindButton).Content = optionsConfig.UnlockKeybind;
		unlockValueTextBox.Text = optionsConfig.UnlockValue;
		((ToggleButton)freezeCheckbox).IsChecked = optionsConfig.FreezeEnabled;
		((ContentControl)freezeKeybindButton).Content = optionsConfig.FreezeKeybind;
		((ToggleButton)unfreezeCheckbox).IsChecked = optionsConfig.UnfreezeEnabled;
		((ContentControl)unfreezeKeybindButton).Content = optionsConfig.UnfreezeKeybind;
		((ToggleButton)killCheckbox).IsChecked = optionsConfig.KillEnabled;
		((ContentControl)killKeybindButton).Content = optionsConfig.KillKeybind;
		try
		{
			string dpiFromBootParameters = Utils.GetDpiFromBootParameters(RegistryManager.Instance.Guest[ParentWindow.mVmName].BootParameters);
			dpiValueTextBox.Text = dpiFromBootParameters;
		}
		catch
		{
			dpiValueTextBox.Text = "240";
		}
	}

	private void SaveSettings()
	{
		if (!_isLoading)
		{
			OptionsSettingsManager.Save(new OptionsSettingsManager.OptionsConfig
			{
				AutolockEnabled = (((ToggleButton)autolockCheckbox).IsChecked == true),
				AutolockValue = autolockValueTextBox.Text,
				AutoAcceptEnabled = (((ToggleButton)autoAcceptCheckbox).IsChecked == true),
				LockEnabled = (((ToggleButton)lockCheckbox).IsChecked == true),
				LockKeybind = ((ContentControl)lockKeybindButton).Content.ToString(),
				LockValue = lockValueTextBox.Text,
				UnlockEnabled = (((ToggleButton)unlockCheckbox).IsChecked == true),
				UnlockKeybind = ((ContentControl)unlockKeybindButton).Content.ToString(),
				UnlockValue = unlockValueTextBox.Text,
				FreezeEnabled = (((ToggleButton)freezeCheckbox).IsChecked == true),
				FreezeKeybind = ((ContentControl)freezeKeybindButton).Content.ToString(),
				UnfreezeEnabled = (((ToggleButton)unfreezeCheckbox).IsChecked == true),
				UnfreezeKeybind = ((ContentControl)unfreezeKeybindButton).Content.ToString(),
				KillEnabled = (((ToggleButton)killCheckbox).IsChecked == true),
				KillKeybind = ((ContentControl)killKeybindButton).Content.ToString()
			});
		}
	}

	private void SetupTextBox(TextBox textBox)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		TextBox obj = textBox;
		object obj2 = _003C_003Ec._003C_003E9__13_0;
		if (obj2 == null)
		{
			TextCompositionEventHandler val = delegate(object s, TextCompositionEventArgs e)
			{
				((RoutedEventArgs)e).Handled = !char.IsDigit(e.Text, 0);
			};
			_003C_003Ec._003C_003E9__13_0 = val;
			obj2 = (object)val;
		}
		((UIElement)obj).PreviewTextInput += (TextCompositionEventHandler)obj2;
		((TextBoxBase)textBox).TextChanged += (TextChangedEventHandler)delegate
		{
			if (int.TryParse(textBox.Text, out var result))
			{
				if (result > 999)
				{
					textBox.Text = "999";
				}
			}
			else if (!string.IsNullOrEmpty(textBox.Text))
			{
				textBox.Text = "0";
			}
			if (textBox != dpiValueTextBox)
			{
				SaveSettings();
			}
			textBox.CaretIndex = textBox.Text.Length;
		};
		TextBox obj3 = textBox;
		object obj4 = _003C_003Ec._003C_003E9__13_2;
		if (obj4 == null)
		{
			DataObjectPastingEventHandler val2 = delegate(object s, DataObjectPastingEventArgs e)
			{
				if (e.DataObject.GetData(typeof(string)) is string s2 && !int.TryParse(s2, out var _))
				{
					((DataObjectEventArgs)e).CancelCommand();
				}
			};
			_003C_003Ec._003C_003E9__13_2 = val2;
			obj4 = (object)val2;
		}
		DataObject.AddPastingHandler((DependencyObject)(object)obj3, (DataObjectPastingEventHandler)obj4);
	}

	private UIElement CreateKeybindRow(string label, string type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		Grid val = new Grid();
		((FrameworkElement)val).Margin = new Thickness(0.0, 0.0, 0.0, 8.0);
		((FrameworkElement)val).HorizontalAlignment = (HorizontalAlignment)0;
		ConfigureOptionRowColumns(val);
		CustomCheckbox val2 = new CustomCheckbox
		{
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		((ButtonBase)val2).Click += (RoutedEventHandler)delegate
		{
			SaveSettings();
		};
		Grid.SetColumn((UIElement)(object)val2, 0);
		TextBlock val3 = new TextBlock
		{
			Text = label,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			FontSize = 17.0,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		Grid.SetColumn((UIElement)(object)val3, 1);
		TextBox val4 = null;
		if (type == "lock" || type == "unlock")
		{
			val4 = CreateTextBox();
			SetupTextBox(val4);
			Grid.SetColumn((UIElement)(object)val4, 2);
		}
		else
		{
			Canvas val5 = new Canvas
			{
				Width = 55.0,
				Height = 26.0,
				Margin = new Thickness(8.0, 0.0, 8.0, 0.0)
			};
			Grid.SetColumn((UIElement)(object)val5, 2);
			((Panel)val).Children.Add((UIElement)(object)val5);
		}
		Button val6 = CreateButton();
		((ButtonBase)val6).Click += new RoutedEventHandler(KeybindClick);
		switch (type)
		{
		case "freeze":
		case "unfreeze":
		case "kill":
			((FrameworkElement)val6).Width = 55.0;
			((FrameworkElement)val6).MinWidth = 0.0;
			break;
		}
		Grid.SetColumn((UIElement)(object)val6, 3);
		switch (type)
		{
		case "lock":
			lockCheckbox = val2;
			lockKeybindButton = val6;
			lockValueTextBox = val4;
			break;
		case "unlock":
			unlockCheckbox = val2;
			unlockKeybindButton = val6;
			unlockValueTextBox = val4;
			break;
		case "freeze":
			freezeCheckbox = val2;
			freezeKeybindButton = val6;
			break;
		case "unfreeze":
			unfreezeCheckbox = val2;
			unfreezeKeybindButton = val6;
			break;
		case "kill":
			killCheckbox = val2;
			killKeybindButton = val6;
			break;
		}
		((Panel)val).Children.Add((UIElement)(object)val2);
		((Panel)val).Children.Add((UIElement)(object)val3);
		if (val4 != null)
		{
			((Panel)val).Children.Add((UIElement)(object)val4);
		}
		((Panel)val).Children.Add((UIElement)(object)val6);
		return (UIElement)(object)val;
	}

	private TextBox CreateTextBox()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		return (TextBox)new CustomTextBox
		{
			Width = 55.0,
			Height = 26.0,
			Foreground = (Brush)(object)Brushes.White,
			VerticalContentAlignment = (VerticalAlignment)1,
			TextAlignment = (TextAlignment)2,
			Margin = new Thickness(8.0, 0.0, 8.0, 0.0),
			CaretBrush = (Brush)(object)Brushes.White,
			Padding = new Thickness(2.0, 0.0, 2.0, 0.0)
		};
	}

	private Button CreateButton()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0137: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0158: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_0179: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		Button val = new Button
		{
			Background = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222222")),
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)222, (byte)222, (byte)222)),
			BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)60, (byte)60, (byte)60)),
			BorderThickness = new Thickness(1.0),
			Margin = new Thickness(8.0, 0.0, 0.0, 0.0),
			MinWidth = 40.0,
			Height = 26.0,
			VerticalAlignment = (VerticalAlignment)1,
			Cursor = Cursors.Hand,
			Padding = new Thickness(4.0, 0.0, 4.0, 0.0)
		};
		ControlTemplate val2 = new ControlTemplate(typeof(Button));
		FrameworkElementFactory val3 = new FrameworkElementFactory(typeof(Border));
		val3.SetBinding(Border.BackgroundProperty, (BindingBase)new Binding("Background")
		{
			RelativeSource = new RelativeSource((RelativeSourceMode)1)
		});
		val3.SetBinding(Border.BorderBrushProperty, (BindingBase)new Binding("BorderBrush")
		{
			RelativeSource = new RelativeSource((RelativeSourceMode)1)
		});
		val3.SetBinding(Border.BorderThicknessProperty, (BindingBase)new Binding("BorderThickness")
		{
			RelativeSource = new RelativeSource((RelativeSourceMode)1)
		});
		FrameworkElementFactory val4 = new FrameworkElementFactory(typeof(ContentPresenter));
		val4.SetValue(FrameworkElement.HorizontalAlignmentProperty, (object)(HorizontalAlignment)1);
		val4.SetValue(FrameworkElement.VerticalAlignmentProperty, (object)(VerticalAlignment)1);
		val3.AppendChild(val4);
		((FrameworkTemplate)val2).VisualTree = val3;
		((Control)val).Template = val2;
		return val;
	}

	private UIElement CreateAutolockRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		Grid val = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
			HorizontalAlignment = (HorizontalAlignment)3
		};
		ConfigureOptionRowColumns(val);
		val.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = new GridLength(1.0, (GridUnitType)2)
		});
		val.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		val.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		autolockCheckbox = new CustomCheckbox
		{
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		((ButtonBase)autolockCheckbox).Click += (RoutedEventHandler)delegate
		{
			SaveSettings();
		};
		Grid.SetColumn((UIElement)(object)autolockCheckbox, 0);
		TextBlock val2 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_AUTOLOCK", "Autolock"),
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			FontSize = 17.0,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		Grid.SetColumn((UIElement)(object)val2, 1);
		autolockValueTextBox = CreateTextBox();
		SetupTextBox(autolockValueTextBox);
		Grid.SetColumn((UIElement)(object)autolockValueTextBox, 2);
		TextBlock val3 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_LANGUAGE_SETTING", "LANGUAGE"),
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			FontSize = 14.0,
			FontWeight = FontWeights.Bold,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(10.0, 0.0, 8.0, 0.0)
		};
		Grid.SetColumn((UIElement)(object)val3, 5);
		mLanguageCombobox = new CustomComboBox();
		((FrameworkElement)mLanguageCombobox).Width = 60.0;
		((FrameworkElement)mLanguageCombobox).Height = 26.0;
		((FrameworkElement)mLanguageCombobox).VerticalAlignment = (VerticalAlignment)1;
		((FrameworkElement)mLanguageCombobox).HorizontalAlignment = (HorizontalAlignment)2;
		((Control)mLanguageCombobox).Background = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222222"));
		Grid.SetColumn((UIElement)(object)mLanguageCombobox, 6);
		((Panel)val).Children.Add((UIElement)(object)autolockCheckbox);
		((Panel)val).Children.Add((UIElement)(object)val2);
		((Panel)val).Children.Add((UIElement)(object)autolockValueTextBox);
		((Panel)val).Children.Add((UIElement)(object)val3);
		((Panel)val).Children.Add((UIElement)(object)mLanguageCombobox);
		AddLanguages();
		return (UIElement)val;
	}

	private UIElement CreateAdbSection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		adbGrid = new Grid
		{
			Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
		};
		adbGrid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		adbGrid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		adbCheckbox = new CustomCheckbox
		{
			Content = "ADB",
			FontSize = 17.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 0.0)
		};
		((ButtonBase)adbCheckbox).Click += new RoutedEventHandler(AdbCheckBox_Click);
		Grid.SetRow((UIElement)(object)adbCheckbox, 0);
		((Panel)adbGrid).Children.Add((UIElement)(object)adbCheckbox);
		return (UIElement)(object)adbGrid;
	}

	private static void ConfigureOptionRowColumns(Grid grid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto,
			SharedSizeGroup = "OptCheck"
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto,
			SharedSizeGroup = "OptLabel"
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto,
			SharedSizeGroup = "OptValue"
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto,
			SharedSizeGroup = "OptKey"
		});
	}

	private void Options_KeyUp(object sender, KeyEventArgs e)
	{
		IsBindingKey = false;
	}

	private UIElement CreateDpiRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		Grid val = new Grid
		{
			Margin = new Thickness(0.0, 4.0, 0.0, 4.0),
			HorizontalAlignment = (HorizontalAlignment)0
		};
		ConfigureOptionRowColumns(val);
		Grid.SetColumn((UIElement)new Canvas(), 0);
		TextBlock val2 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_CUSTOM_DPI", "Custom DPI"),
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			FontSize = 17.0,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		Grid.SetColumn((UIElement)(object)val2, 1);
		dpiValueTextBox = CreateTextBox();
		SetupTextBox(dpiValueTextBox);
		Grid.SetColumn((UIElement)(object)dpiValueTextBox, 2);
		Button val3 = CreateButton();
		((ContentControl)val3).Content = "Set";
		((ButtonBase)val3).Click += new RoutedEventHandler(ApplyDpi_Click);
		Grid.SetColumn((UIElement)(object)val3, 3);
		((Panel)val).Children.Add((UIElement)(object)val2);
		((Panel)val).Children.Add((UIElement)(object)dpiValueTextBox);
		((Panel)val).Children.Add((UIElement)(object)val3);
		return (UIElement)val;
	}

	private void ApplyDpi_Click(object sender, RoutedEventArgs e)
	{
		if (int.TryParse(dpiValueTextBox.Text, out var result))
		{
			if (result < 80)
			{
				result = 80;
			}
			if (result > 999)
			{
				result = 999;
			}
			dpiValueTextBox.Text = result.ToString();
			try
			{
				string mVmName = ParentWindow.mVmName;
				string text = "bgp64";
				Utils.SetDPIInBootParameters(RegistryManager.Instance.Guest[mVmName].BootParameters, result.ToString(), mVmName, text);
				ShowNotification(LocaleStrings.GetLocalizedString("STRING_RESTART_EMULATOR", "Restart the emulator."));
			}
			catch (Exception ex)
			{
				ShowNotification("Error: " + ex.Message);
			}
		}
	}

	private void ShowNotification(string msg)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((Control)new NotificationForm(msg)).Show();
		}
		catch
		{
			MessageBox.Show(msg);
		}
	}

	private TextBlock CreateSectionTitle(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		return new TextBlock
		{
			Text = text,
			Foreground = (Brush)(object)Brushes.White,
			FontSize = 18.0,
			Margin = new Thickness(0.0, 5.0, 0.0, 10.0),
			HorizontalAlignment = (HorizontalAlignment)0
		};
	}

	private UIElement CreateAutoAcceptRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		Grid val = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
			HorizontalAlignment = (HorizontalAlignment)0
		};
		ConfigureOptionRowColumns(val);
		autoAcceptCheckbox = new CustomCheckbox
		{
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		((ButtonBase)autoAcceptCheckbox).Click += (RoutedEventHandler)delegate
		{
			SaveSettings();
		};
		Grid.SetColumn((UIElement)(object)autoAcceptCheckbox, 0);
		TextBlock val2 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_AUTO_ACCEPT", "Auto Accept"),
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)200, (byte)200, (byte)200)),
			FontSize = 17.0,
			VerticalAlignment = (VerticalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
		};
		Grid.SetColumn((UIElement)(object)val2, 1);
		((Panel)val).Children.Add((UIElement)(object)autoAcceptCheckbox);
		((Panel)val).Children.Add((UIElement)(object)val2);
		return (UIElement)val;
	}

	private void AddLanguages()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		if (Globalization.sSupportedLocales == null)
		{
			return;
		}
		dictComboBoxItems.Clear();
		List<string> list = new List<string> { "ru-RU", "en-US" };
		foreach (string key in Globalization.sSupportedLocales.Keys)
		{
			if (list.Contains(key))
			{
				string content = "EN";
				if (key.Equals("ru-RU", StringComparison.OrdinalIgnoreCase))
				{
					content = "RU";
				}
				ComboBoxItem val = new ComboBoxItem
				{
					Content = content,
					Tag = key,
					ToolTip = Globalization.sSupportedLocales[key].ToString(CultureInfo.InvariantCulture)
				};
				if (!dictComboBoxItems.ContainsKey(key))
				{
					dictComboBoxItems.Add(key, val);
				}
				((ItemsControl)mLanguageCombobox).Items.Add((object)val);
			}
		}
		SelectDefaultValue();
	}

	private void SelectDefaultValue()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		((Selector)mLanguageCombobox).SelectionChanged -= new SelectionChangedEventHandler(mLanguageCombobox_SelectionChanged);
		string value = RegistryManager.Instance.UserSelectedLocale;
		if (string.IsNullOrEmpty(value))
		{
			value = LocaleStrings.GetLocaleName("Android", false);
		}
		foreach (ComboBoxItem item in (IEnumerable)((ItemsControl)mLanguageCombobox).Items)
		{
			ComboBoxItem val = item;
			if (((FrameworkElement)val).Tag != null && ((FrameworkElement)val).Tag.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
			{
				((Selector)mLanguageCombobox).SelectedItem = val;
				break;
			}
		}
		((Selector)mLanguageCombobox).SelectionChanged += new SelectionChangedEventHandler(mLanguageCombobox_SelectionChanged);
	}

	private void mLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isRebuilding)
		{
			return;
		}
		try
		{
			object selectedItem = ((Selector)mLanguageCombobox).SelectedItem;
			ComboBoxItem val = (ComboBoxItem)((selectedItem is ComboBoxItem) ? selectedItem : null);
			if (val != null && ((FrameworkElement)val).Tag != null)
			{
				string text = ((FrameworkElement)val).Tag.ToString();
				if (!string.Equals(RegistryManager.Instance.UserSelectedLocale, text, StringComparison.InvariantCultureIgnoreCase))
				{
					RegistryManager.Instance.UserSelectedLocale = text;
					BlueStacksUIUtils.UpdateLocale(text);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Options: Exception in set locale " + ex.ToString());
		}
	}

	private void OnLocaleUpdated(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			RebuildUI();
		});
	}

	private void RebuildUI()
	{
		_isRebuilding = true;
		try
		{
			((ContentControl)this).Content = Layout();
			InitializeAdbSettings();
			_isLoading = true;
			LoadSettings();
			_isLoading = false;
		}
		finally
		{
			_isRebuilding = false;
		}
	}
}
