using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class DualTextBlockControl : UserControl, IComponentConnector
{
	private Regex decimalRegex = new Regex("^[0-9]*(\\.)?[0-9]*$");

	private List<IMAction> lstActionItem = new List<IMAction>();

	private MainWindow ParentWindow;

	private List<Key> mKeyList = new List<Key>();

	private string mActionItemProperty;

	private Type PropertyType;

	internal bool IsAddDirectionAttribute;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mValueColumn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mKeyPropertyName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mKeyTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mKeyPropertyNameTextBox;

	private bool _contentLoaded;

	internal List<IMAction> LstActionItem => lstActionItem;

	internal string ActionItemProperty
	{
		get
		{
			return mActionItemProperty;
		}
		set
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			mActionItemProperty = value;
			switch (value)
			{
			case "Tags":
			case "EnableCondition":
			case "StartCondition":
			case "Note":
				mValueColumn.Width = new GridLength(1.0, (GridUnitType)2);
				((Control)mKeyTextBox).HorizontalContentAlignment = (HorizontalAlignment)0;
				((UIElement)mKeyPropertyName).Visibility = (Visibility)2;
				((FrameworkElement)mKeyTextBox).MaxWidth = double.PositiveInfinity;
				((UIElement)mKeyPropertyNameTextBox).Visibility = (Visibility)2;
				break;
			}
			if (value == "DpadTitle")
			{
				((UIElement)mKeyTextBox).IsEnabled = false;
			}
		}
	}

	public DualTextBlockControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mKeyTextBox, false);
	}

	private void KeyPropertyNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		KMManager.CheckAndCreateNewScheme();
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void KeyTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Invalid comparison between Unknown and I4
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Invalid comparison between Unknown and I4
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Invalid comparison between Unknown and I4
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Invalid comparison between Unknown and I4
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Invalid comparison between Unknown and I4
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		KeymapCanvasWindow.sIsDirty = true;
		if ((int)e.Key == 13)
		{
			return;
		}
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
		{
			if (lstActionItem[0].Type == KeyActionType.Tap || lstActionItem[0].Type == KeyActionType.TapRepeat || lstActionItem[0].Type == KeyActionType.Script)
			{
				if ((int)e.Key == 2 || (int)e.SystemKey == 2)
				{
					((FrameworkElement)mKeyTextBox).Tag = string.Empty;
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
				}
				else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
				{
					if ((int)e.SystemKey == 120 || (int)e.SystemKey == 121 || (int)e.SystemKey == 99)
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
					}
					else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers != 0)
					{
						if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == 1)
						{
							UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
						}
						else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == 5)
						{
							UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
						}
						else
						{
							UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.Key);
						}
					}
					else
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.Key);
					}
				}
			}
			else
			{
				if ((int)e.Key == 156 && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
				{
					((FrameworkElement)mKeyTextBox).Tag = IMAPKeys.GetStringForFile(e.SystemKey);
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
				}
				else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
				{
					((FrameworkElement)mKeyTextBox).Tag = IMAPKeys.GetStringForFile(e.Key);
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
				}
				else if ((int)e.Key == 2)
				{
					((FrameworkElement)mKeyTextBox).Tag = string.Empty;
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + string.Empty);
				}
				((RoutedEventArgs)e).Handled = true;
			}
		}
		if (PropertyType.Equals(typeof(bool)))
		{
			((FrameworkElement)mKeyTextBox).Tag = !Convert.ToBoolean(lstActionItem.First()[ActionItemProperty], CultureInfo.InvariantCulture);
			BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
			if (lstActionItem.First().Type == KeyActionType.TapRepeat && KMManager.CanvasWindow.mCanvasElement != null)
			{
				KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(lstActionItem.First());
			}
			if (lstActionItem.First().Type == KeyActionType.EdgeScroll && ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
			{
				KMManager.AssignEdgeScrollMode(((FrameworkElement)mKeyTextBox).Tag.ToString(), mKeyTextBox);
			}
			((RoutedEventArgs)e).Handled = true;
		}
		if (PropertyType.Equals(typeof(int)) && lstActionItem.First().Type == KeyActionType.FreeLook && KMManager.CanvasWindow.mCanvasElement != null)
		{
			KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(lstActionItem.First());
		}
		if (string.Equals(ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase) && ((int)e.Key == 2 || (int)e.SystemKey == 2))
		{
			((FrameworkElement)mKeyTextBox).Tag = string.Empty;
			BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
		}
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) && (lstActionItem[0].Type == KeyActionType.Tap || lstActionItem[0].Type == KeyActionType.TapRepeat || lstActionItem[0].Type == KeyActionType.Script) && (int)e.Key == 3)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private void KeyTextBox_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (lstActionItem[0].Type == KeyActionType.Tap || lstActionItem[0].Type == KeyActionType.TapRepeat || lstActionItem[0].Type == KeyActionType.Script)
		{
			if (mKeyList.Count >= 2)
			{
				string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 1));
				string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 1));
				mKeyTextBox.Text = text;
				((FrameworkElement)mKeyTextBox).Tag = tag;
				SetValueHandling();
			}
			else if (mKeyList.Count == 1)
			{
				string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(0));
				string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(0));
				mKeyTextBox.Text = text;
				((FrameworkElement)mKeyTextBox).Tag = tag;
				SetValueHandling();
			}
			if (!ActionItemProperty.Equals("EnableCondition", StringComparison.InvariantCultureIgnoreCase) && !ActionItemProperty.Equals("StartCondition", StringComparison.InvariantCultureIgnoreCase) && !ActionItemProperty.Equals("Note", StringComparison.InvariantCultureIgnoreCase))
			{
				mKeyTextBox.CaretIndex = mKeyTextBox.Text.Length;
			}
			mKeyList.Clear();
		}
	}

	private void KeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Invalid comparison between Unknown and I4
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
		{
			if ((int)((MouseEventArgs)e).MiddleButton == 1)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseMButton";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseMButton");
			}
			else if ((int)((MouseEventArgs)e).RightButton == 1)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseRButton";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseRButton");
			}
			else if ((int)((MouseEventArgs)e).XButton1 == 1)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseXButton1";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton1");
			}
			else if ((int)((MouseEventArgs)e).XButton2 == 1)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseXButton2";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton2");
			}
		}
		if (PropertyType.Equals(typeof(bool)))
		{
			((FrameworkElement)mKeyTextBox).Tag = !Convert.ToBoolean(lstActionItem.First()[ActionItemProperty], CultureInfo.InvariantCulture);
			BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
			if (lstActionItem.First().Type == KeyActionType.EdgeScroll && ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
			{
				KMManager.AssignEdgeScrollMode(((FrameworkElement)mKeyTextBox).Tag.ToString(), mKeyTextBox);
			}
		}
	}

	internal bool AddActionItem(IMAction action)
	{
		PropertyType = IMAction.DictPropertyInfo[action.Type][ActionItemProperty].PropertyType;
		string origKey;
		if ((!PropertyType.Equals(typeof(string)) && !string.Equals(ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "GamepadSensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase)) || (action.Type == KeyActionType.State && (string.Equals(ActionItemProperty, "Name", StringComparison.InvariantCultureIgnoreCase) || string.Equals(ActionItemProperty, "Model", StringComparison.InvariantCultureIgnoreCase))))
		{
			((UIElement)mKeyPropertyNameTextBox).IsEnabled = false;
		}
		else
		{
			((UIElement)mKeyPropertyNameTextBox).IsEnabled = true;
			int num = mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
			origKey = mActionItemProperty;
			if (num > 0)
			{
				origKey = mActionItemProperty.Substring(0, num);
			}
			AssignGuidanceText(action, origKey);
		}
		if ((action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom) && (string.Equals(ActionItemProperty, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(ActionItemProperty, "Acceleration", StringComparison.InvariantCultureIgnoreCase) || string.Equals(ActionItemProperty, "Amplitude", StringComparison.InvariantCultureIgnoreCase)))
		{
			((UIElement)mKeyPropertyNameTextBox).IsEnabled = true;
			AssignGuidanceText(action, mActionItemProperty);
		}
		lstActionItem.Add(action);
		string text = action[ActionItemProperty].ToString();
		origKey = mActionItemProperty;
		if (mActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
		{
			int num2 = mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
			if (num2 > 0)
			{
				origKey = mActionItemProperty.Substring(0, num2);
			}
		}
		if (IsAddDirectionAttribute)
		{
			BlueStacksUIBinding.Bind(mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + origKey + action.Direction);
		}
		else
		{
			BlueStacksUIBinding.Bind(mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + origKey);
		}
		((FrameworkElement)mKeyTextBox).Tag = action[ActionItemProperty];
		if (ActionItemProperty.StartsWith("Key", StringComparison.CurrentCultureIgnoreCase))
		{
			BlueStacksUIBinding.Bind(mKeyTextBox, KMManager.GetStringsToShowInUI(text));
		}
		else
		{
			mKeyTextBox.Text = text;
		}
		if (lstActionItem.First().Type == KeyActionType.EdgeScroll && ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
		{
			KMManager.AssignEdgeScrollMode(text, mKeyTextBox);
		}
		if (UsefulExtensionMethod.Contains(text, "Gamepad", StringComparison.InvariantCultureIgnoreCase) || UsefulExtensionMethod.Contains(ActionItemProperty, "Gamepad", StringComparison.InvariantCultureIgnoreCase))
		{
			BlueStacksUIBinding.Bind(mKeyTextBox, KMManager.GetKeyUIValue(text));
			((FrameworkElement)mKeyTextBox).ToolTip = mKeyTextBox.Text;
			return true;
		}
		return false;
	}

	private void AssignGuidanceText(IMAction action, string origKey)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (action.Guidance.ContainsKey(origKey) && !string.IsNullOrEmpty(action.Guidance[origKey]))
		{
			mKeyPropertyNameTextBox.Text = ParentWindow.SelectedConfig.GetUIString(action.Guidance[origKey]);
		}
		else if (action.Guidance.ContainsKey(mActionItemProperty) && !string.IsNullOrEmpty(action.Guidance[mActionItemProperty]))
		{
			mKeyPropertyNameTextBox.Text = ParentWindow.SelectedConfig.GetUIString(action.Guidance[mActionItemProperty]);
			if (!action.Guidance.ContainsKey(origKey) && !string.IsNullOrEmpty(mKeyPropertyNameTextBox.Text.Trim()))
			{
				action.Guidance.Add(origKey, mKeyPropertyNameTextBox.Text.ToString(CultureInfo.InvariantCulture));
			}
		}
		else
		{
			BlueStacksUIBinding.Bind(mKeyPropertyNameTextBox, "STRING_ENTER_GUIDANCE_TEXT");
			((Control)mKeyPropertyNameTextBox).FontStyle = FontStyles.Italic;
			((Control)mKeyPropertyNameTextBox).FontWeight = FontWeights.ExtraLight;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
		}
	}

	private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		SetValueHandling();
	}

	private void SetValueHandling()
	{
		string text = lstActionItem[0][ActionItemProperty].ToString();
		int result3;
		if (PropertyType.Equals(typeof(double)))
		{
			if (double.TryParse(text, out var result))
			{
				text = result.ToString(CultureInfo.InvariantCulture);
			}
			if (double.TryParse(mKeyTextBox.Text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result2))
			{
				if (!string.Equals(ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase))
				{
					text = mKeyTextBox.Text;
				}
				else if (decimalRegex.IsMatch(mKeyTextBox.Text) && 0.0 <= result2 && result2 <= 10.0)
				{
					text = result2.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					mKeyTextBox.Text = text;
				}
			}
			else if (string.Equals(mKeyTextBox.Text, ".", StringComparison.InvariantCultureIgnoreCase))
			{
				mKeyTextBox.Text = "0.";
				text = "0";
				mKeyTextBox.CaretIndex = mKeyTextBox.Text.Length;
			}
			else if (string.IsNullOrEmpty(mKeyTextBox.Text))
			{
				mKeyTextBox.Text = "0";
				text = "0";
				mKeyTextBox.CaretIndex = mKeyTextBox.Text.Length;
			}
			else if (!string.IsNullOrEmpty(mKeyTextBox.Text))
			{
				mKeyTextBox.Text = text.ToString(CultureInfo.InvariantCulture);
			}
		}
		else if (!PropertyType.Equals(typeof(int)))
		{
			text = (PropertyType.Equals(typeof(bool)) ? ((FrameworkElement)mKeyTextBox).Tag.ToString() : ((!ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) && !ActionItemProperty.StartsWith("Gamepad", StringComparison.InvariantCultureIgnoreCase)) ? mKeyTextBox.Text : ((FrameworkElement)mKeyTextBox).Tag.ToString()));
		}
		else if (int.TryParse(mKeyTextBox.Text, out result3))
		{
			text = mKeyTextBox.Text;
		}
		else if (!string.IsNullOrEmpty(mKeyTextBox.Text))
		{
			mKeyTextBox.Text = text;
		}
		Setvalue(text);
	}

	internal void Setvalue(string value)
	{
		foreach (IMAction item in lstActionItem)
		{
			if (UsefulExtensionMethod.Contains(item[ActionItemProperty].ToString(), "Gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				((FrameworkElement)mKeyTextBox).ToolTip = mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
			}
			if (!string.Equals(item[ActionItemProperty].ToString(), value, StringComparison.InvariantCultureIgnoreCase))
			{
				item[ActionItemProperty] = value;
				KeymapCanvasWindow.sIsDirty = true;
			}
		}
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
		{
			mKeyTextBox.Text = mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
		}
		if (UsefulExtensionMethod.Contains(ActionItemProperty, "Gamepad", StringComparison.InvariantCultureIgnoreCase))
		{
			mKeyTextBox.Text = mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
			((FrameworkElement)mKeyTextBox).ToolTip = mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
		}
	}

	private void KeyPropertyNameTextBox_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		if (((UIElement)mKeyPropertyNameTextBox).IsVisible)
		{
			return;
		}
		string key = ActionItemProperty;
		if (ActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
		{
			int num = ActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
			if (num > 0)
			{
				key = ActionItemProperty.Substring(0, num);
			}
		}
		if (string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), mKeyPropertyNameTextBox.Text, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(mKeyPropertyNameTextBox.Text.Trim()))
		{
			foreach (IMAction item in lstActionItem)
			{
				item.Guidance.Remove(key);
			}
			return;
		}
		KeymapCanvasWindow.sIsDirty = true;
		foreach (IMAction item2 in lstActionItem)
		{
			item2.Guidance[key] = mKeyPropertyNameTextBox.Text;
		}
		ParentWindow.SelectedConfig.AddString(mKeyPropertyNameTextBox.Text);
	}

	private void KeyTextBox_LostFocus(object sender, RoutedEventArgs e)
	{
		if (PropertyType.Equals(typeof(double)) && !double.TryParse(mKeyTextBox.Text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var _))
		{
			Setvalue("0");
			mKeyTextBox.Text = "0";
		}
		if (PropertyType.Equals(typeof(int)) && !int.TryParse(mKeyTextBox.Text, out var _))
		{
			Setvalue("0");
			mKeyTextBox.Text = "0";
		}
	}

	private void KeyPropertyNameTextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), mKeyPropertyNameTextBox.Text, StringComparison.InvariantCulture))
		{
			mKeyPropertyNameTextBox.Text = "";
			((Control)mKeyPropertyNameTextBox).FontStyle = FontStyles.Normal;
			((Control)mKeyPropertyNameTextBox).FontWeight = FontWeights.Normal;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockForeground");
		}
	}

	private void KeyPropertyNameTextBox_LostFocus(object sender, RoutedEventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(mKeyPropertyNameTextBox.Text))
		{
			mKeyPropertyNameTextBox.Text = LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", "");
			((Control)mKeyPropertyNameTextBox).FontStyle = FontStyles.Italic;
			((Control)mKeyPropertyNameTextBox).FontWeight = FontWeights.ExtraLight;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
		}
	}

	private void mKeyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (((FrameworkElement)this).IsLoaded)
		{
			KMManager.sGamepadDualTextbox = this;
			KMManager.pressedGamepadKeyList.Clear();
			KMManager.CallGamepadHandler(ParentWindow);
		}
	}

	private void KeyTextBoxPreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
		{
			if (e.Delta > 0)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseWheelUp";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
			}
			else if (e.Delta < 0)
			{
				((RoutedEventArgs)e).Handled = true;
				((FrameworkElement)mKeyTextBox).Tag = "MouseWheelDown";
				BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/dualtextblockcontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mValueColumn = (ColumnDefinition)target;
			break;
		case 2:
			mKeyPropertyName = (TextBox)target;
			((TextBoxBase)mKeyPropertyName).TextChanged += new TextChangedEventHandler(KeyPropertyNameTextBox_TextChanged);
			((UIElement)mKeyPropertyName).IsVisibleChanged += new DependencyPropertyChangedEventHandler(KeyPropertyNameTextBox_IsVisibleChanged);
			break;
		case 3:
			mKeyTextBox = (TextBox)target;
			((UIElement)mKeyTextBox).PreviewMouseDown += new MouseButtonEventHandler(KeyTextBox_MouseDown);
			((UIElement)mKeyTextBox).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(mKeyTextBox_PreviewMouseLeftButtonDown);
			((TextBoxBase)mKeyTextBox).TextChanged += new TextChangedEventHandler(KeyTextBox_TextChanged);
			((UIElement)mKeyTextBox).PreviewKeyDown += new KeyEventHandler(KeyTextBox_KeyDown);
			((UIElement)mKeyTextBox).KeyUp += new KeyEventHandler(KeyTextBox_KeyUp);
			((UIElement)mKeyTextBox).LostFocus += new RoutedEventHandler(KeyTextBox_LostFocus);
			((UIElement)mKeyTextBox).PreviewMouseWheel += new MouseWheelEventHandler(KeyTextBoxPreviewMouseWheel);
			break;
		case 4:
			mKeyPropertyNameTextBox = (TextBox)target;
			((UIElement)mKeyPropertyNameTextBox).GotFocus += new RoutedEventHandler(KeyPropertyNameTextBox_GotFocus);
			((UIElement)mKeyPropertyNameTextBox).LostFocus += new RoutedEventHandler(KeyPropertyNameTextBox_LostFocus);
			((TextBoxBase)mKeyPropertyNameTextBox).TextChanged += new TextChangedEventHandler(KeyPropertyNameTextBox_TextChanged);
			((UIElement)mKeyPropertyNameTextBox).IsVisibleChanged += new DependencyPropertyChangedEventHandler(KeyPropertyNameTextBox_IsVisibleChanged);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
