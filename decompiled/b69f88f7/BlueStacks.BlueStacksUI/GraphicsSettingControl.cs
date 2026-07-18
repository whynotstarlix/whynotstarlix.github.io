using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using BlueStacks.Common;
using nspector.Common;

namespace BlueStacks.BlueStacksUI;

public class GraphicsSettingControl : UserControl
{
	private Border _lowPanel;

	private Border _highPanel;

	private string _selectedMode;

	private MainWindow ParentWindow;

	private string _currentMode;

	private Button _saveButton;

	private readonly string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/selected_gfx.cfg");

	private Button _resetButton;

	private Border _potatoPanel;

	public GraphicsSettingControl(MainWindow window)
	{
		ParentWindow = window;
		((FrameworkElement)this).Width = 530.0;
		((FrameworkElement)this).Height = 440.0;
		((UIElement)this).Visibility = (Visibility)1;
		LocaleStrings.SourceUpdatedEvent += OnLocaleUpdated;
		RebuildUI();
	}

	private UIElement CreateLayout()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		StackPanel val = new StackPanel
		{
			Orientation = (Orientation)1,
			Margin = new Thickness(10.0),
			VerticalAlignment = (VerticalAlignment)1,
			HorizontalAlignment = (HorizontalAlignment)1
		};
		TextBlock val2 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_CHOOSE_GRAPHICS_MODE", "Choose graphics mode"),
			FontSize = 18.0,
			Foreground = (Brush)(object)Brushes.White,
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
		};
		((Panel)val).Children.Add((UIElement)(object)val2);
		TextBlock val3 = new TextBlock
		{
			Text = LocaleStrings.GetLocalizedString("STRING_NVIDIA_ONLY", "(Only for NVIDIA)"),
			FontSize = 12.0,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)100, (byte)100, (byte)100)),
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
		};
		((Panel)val).Children.Add((UIElement)(object)val3);
		StackPanel val4 = new StackPanel
		{
			Orientation = (Orientation)1,
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
		};
		string localizedString = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_POTATO", "Potato (Super Low)");
		string localizedString2 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_POTATO_DESC", "Max performance, no textures.");
		_potatoPanel = CreateGraphicsPanel(localizedString, localizedString2);
		string localizedString3 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_LOW", "Low graphics (Blur)");
		string localizedString4 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_LOW_DESC", "Simplified models, low res.");
		_lowPanel = CreateGraphicsPanel(localizedString3, localizedString4);
		string localizedString5 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_HIGH", "High graphics (4K)");
		string localizedString6 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_HIGH_DESC", "Detailed models, top quality.");
		_highPanel = CreateGraphicsPanel(localizedString5, localizedString6);
		((Panel)val4).Children.Add((UIElement)(object)_potatoPanel);
		((Panel)val4).Children.Add((UIElement)(object)_lowPanel);
		((Panel)val4).Children.Add((UIElement)(object)_highPanel);
		((Panel)val).Children.Add((UIElement)(object)val4);
		StackPanel val5 = new StackPanel
		{
			Orientation = (Orientation)0,
			HorizontalAlignment = (HorizontalAlignment)1
		};
		_saveButton = CreateStyledButton(LocaleStrings.GetLocalizedString("STRING_SAVE", "Save"), new RoutedEventHandler(SaveButton_Click));
		((UIElement)_saveButton).IsEnabled = false;
		_resetButton = CreateStyledButton(LocaleStrings.GetLocalizedString("STRING_RESET_DEFAULT", "Reset (Default)"), (RoutedEventHandler)delegate
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/def.nip")))
			{
				try
				{
					_selectedMode = "Default";
					ClearStatus(_potatoPanel);
					ClearStatus(_lowPanel);
					ClearStatus(_highPanel);
					_potatoPanel.BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)111, (byte)111, (byte)111));
					_lowPanel.BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)111, (byte)111, (byte)111));
					_highPanel.BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)111, (byte)111, (byte)111));
					UpdateSaveButtonState();
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_ERROR", "Error") + ": " + ex.Message);
					return;
				}
			}
			MessageBox.Show("def.nip not found.");
		});
		((Panel)val5).Children.Add((UIElement)(object)_saveButton);
		((Panel)val5).Children.Add((UIElement)(object)_resetButton);
		((Panel)val).Children.Add((UIElement)(object)val5);
		return (UIElement)val;
	}

	private void SelectPanel(Border selectedBorder, string mode, TextBlock statusBlock)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		Border[] array = (Border[])(object)new Border[3] { _potatoPanel, _lowPanel, _highPanel };
		foreach (Border val in array)
		{
			val.BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)111, (byte)111, (byte)111));
			ClearStatus(val);
		}
		selectedBorder.BorderBrush = (Brush)(object)Brushes.White;
		statusBlock.Text = LocaleStrings.GetLocalizedString("STRING_SELECTED", "✓ Selected");
		_selectedMode = mode;
		((UIElement)_resetButton).IsEnabled = true;
		UpdateSaveButtonState();
	}

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(_selectedMode))
		{
			MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_SELECT_MODE_FIRST", "Select a mode first."));
			return;
		}
		try
		{
			string text = null;
			string localizedString = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_HIGH", "High graphics (4K)");
			string localizedString2 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_LOW", "Low graphics (Blur)");
			string localizedString3 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_POTATO", "Potato (Super Low)");
			LocaleStrings.GetLocalizedString("STRING_RESET_DEFAULT", "Reset (Default)");
			if (_selectedMode == localizedString)
			{
				text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/4k.nip");
			}
			else if (_selectedMode == localizedString2)
			{
				text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/low.nip");
			}
			else if (_selectedMode == localizedString3)
			{
				text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/potato.nip");
			}
			else if (_selectedMode == "Default" || _selectedMode == "По умолчанию" || _selectedMode.Contains("Default") || _selectedMode.Contains("умолчанию"))
			{
				text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/def.nip");
			}
			if (string.IsNullOrEmpty(text) || !File.Exists(text))
			{
				MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_PROFILE_NOT_FOUND", "Profile file not found") + ": " + (text ?? "(unknown)"));
				return;
			}
			DrsServiceLocator.ImportService.ImportProfiles(text);
			_currentMode = ((_selectedMode.Contains("Default") || _selectedMode.Contains("умолчанию")) ? null : _selectedMode);
			SaveSelectedProfile(_currentMode);
			CustomMessageWindow val = new CustomMessageWindow
			{
				Owner = (Window)(object)ParentWindow,
				WindowStartupLocation = (WindowStartupLocation)2
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "Restart BlueStacks");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "Restart required to apply changes.");
			val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_RESTART_NOW", "Restart now"), (EventHandler)delegate
			{
				RestartInstanceHandler();
				BlueStacksUIUtils.RestartInstance(Strings.CurrentDefaultVmName);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_LATER_BUTTON", "Later"), (EventHandler)delegate
			{
				BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)ParentWindow);
			}, (string)null, false, (object)null);
			((Window)val).ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_ERROR", "Error") + ": " + ex.Message);
		}
		UpdateSaveButtonState();
	}

	public Button CreateStyledButton(string text, RoutedEventHandler onClick)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		CustomButton val = new CustomButton
		{
			Content = text,
			Margin = new Thickness(8.0),
			ButtonColor = (ButtonColors)4,
			Cursor = Cursors.Hand
		};
		((ButtonBase)val).Click += onClick;
		return (Button)val;
	}

	private void ClearStatus(Border border)
	{
		if (border == null)
		{
			return;
		}
		UIElement child = ((Decorator)border).Child;
		StackPanel val = (StackPanel)(object)((child is StackPanel) ? child : null);
		if (val == null)
		{
			return;
		}
		foreach (object child2 in ((Panel)val).Children)
		{
			TextBlock val2 = (TextBlock)((child2 is TextBlock) ? child2 : null);
			if (val2 != null && val2.FontSize == 11.0)
			{
				val2.Text = "";
			}
		}
	}

	private void RestartInstanceHandler()
	{
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)ParentWindow);
	}

	private void SaveSelectedProfile(string mode)
	{
		try
		{
			File.WriteAllText(configFile, mode ?? "");
		}
		catch
		{
		}
	}

	private void LoadSelectedProfile()
	{
		if (!File.Exists(configFile))
		{
			_currentMode = null;
			_selectedMode = null;
			((UIElement)_resetButton).IsEnabled = false;
			return;
		}
		try
		{
			_currentMode = File.ReadAllText(configFile).Trim();
		}
		catch
		{
			return;
		}
		_selectedMode = _currentMode;
		string localizedString = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_HIGH", "High graphics (4K)");
		string localizedString2 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_LOW", "Low graphics (Blur)");
		string localizedString3 = LocaleStrings.GetLocalizedString("STRING_GRAPHICS_POTATO", "Potato (Super Low)");
		if (_currentMode == localizedString)
		{
			SelectPanel(_highPanel, localizedString, GetStatusBlock(_highPanel));
			return;
		}
		if (_currentMode == localizedString2)
		{
			SelectPanel(_lowPanel, localizedString2, GetStatusBlock(_lowPanel));
			return;
		}
		if (_currentMode == localizedString3)
		{
			SelectPanel(_potatoPanel, localizedString3, GetStatusBlock(_potatoPanel));
			return;
		}
		((UIElement)_resetButton).IsEnabled = false;
		_currentMode = null;
		_selectedMode = null;
	}

	private TextBlock GetStatusBlock(Border border)
	{
		if (border == null || ((Decorator)border).Child == null)
		{
			return null;
		}
		UIElement child = ((Decorator)border).Child;
		foreach (object child2 in ((Panel)((child is StackPanel) ? child : null)).Children)
		{
			TextBlock val = (TextBlock)((child2 is TextBlock) ? child2 : null);
			if (val != null && val.FontSize == 11.0)
			{
				return val;
			}
		}
		return null;
	}

	private void UpdateSaveButtonState()
	{
		((UIElement)_saveButton).IsEnabled = !string.IsNullOrEmpty(_selectedMode) && _selectedMode != _currentMode;
	}

	private Border CreateGraphicsPanel(string title, string description)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00fa: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		Border border = new Border
		{
			Width = 400.0,
			Height = 80.0,
			BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)111, (byte)111, (byte)111)),
			BorderThickness = new Thickness(1.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0),
			Cursor = Cursors.Hand,
			Background = (Brush)new SolidColorBrush(Color.FromRgb((byte)17, (byte)17, (byte)17)),
			Effect = (Effect)new DropShadowEffect
			{
				BlurRadius = 8.0,
				ShadowDepth = 2.0,
				Opacity = 0.5,
				Color = Colors.Black
			}
		};
		StackPanel val = new StackPanel();
		((FrameworkElement)val).VerticalAlignment = (VerticalAlignment)1;
		TextBlock val2 = new TextBlock
		{
			Text = title,
			FontSize = 16.0,
			Foreground = (Brush)(object)Brushes.White,
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 5.0, 0.0, 2.0),
			FontWeight = FontWeights.Bold
		};
		((Panel)val).Children.Add((UIElement)(object)val2);
		TextBlock val3 = new TextBlock
		{
			Text = description,
			FontSize = 12.0,
			TextWrapping = (TextWrapping)2,
			Foreground = (Brush)new SolidColorBrush(Color.FromRgb((byte)190, (byte)190, (byte)190)),
			TextAlignment = (TextAlignment)2,
			Margin = new Thickness(6.0, 0.0, 6.0, 2.0)
		};
		((Panel)val).Children.Add((UIElement)(object)val3);
		TextBlock statusBlock = new TextBlock
		{
			Text = "",
			FontSize = 11.0,
			Foreground = (Brush)(object)Brushes.LimeGreen,
			HorizontalAlignment = (HorizontalAlignment)1,
			Margin = new Thickness(0.0, 0.0, 0.0, 5.0)
		};
		((Panel)val).Children.Add((UIElement)(object)statusBlock);
		((Decorator)border).Child = (UIElement)(object)val;
		((UIElement)border).MouseLeftButtonUp += (MouseButtonEventHandler)delegate
		{
			SelectPanel(border, title, statusBlock);
		};
		return border;
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
		((ContentControl)this).Content = CreateLayout();
		LoadSelectedProfile();
		UpdateSaveButtonState();
	}
}
