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
			{
				mValueColumn.Width = new GridLength(1.0, (GridUnitType)2);
				((Control)mKeyTextBox).HorizontalContentAlignment = (HorizontalAlignment)0;
				TextBox obj = mKeyPropertyName;
				int num = ((1240244486 > 246090016) ? 2 : 2);
				((UIElement)obj).Visibility = (Visibility)num;
				((FrameworkElement)mKeyTextBox).MaxWidth = double.PositiveInfinity;
				TextBox obj2 = mKeyPropertyNameTextBox;
				int num2 = ((1701293609 > 2066153302) ? 2 : 2);
				((UIElement)obj2).Visibility = (Visibility)num2;
				break;
			}
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
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Invalid comparison between Unknown and I4
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Invalid comparison between Unknown and I4
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Invalid comparison between Unknown and I4
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Invalid comparison between Unknown and I4
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Invalid comparison between Unknown and I4
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Invalid comparison between Unknown and I4
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Invalid comparison between Unknown and I4
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Invalid comparison between Unknown and I4
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Invalid comparison between Unknown and I4
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Invalid comparison between Unknown and I4
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Invalid comparison between Unknown and I4
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		KeymapCanvasWindow.sIsDirty = true;
		Key key = e.Key;
		int num = ((751166235 > 1889069307) ? 17 : 13);
		if ((int)key == num)
		{
			return;
		}
		if (ActionItemProperty.StartsWith("Key", (StringComparison)(-1802801302 - -1802801305)))
		{
			if (lstActionItem[0].Type == KeyActionType.Tap || lstActionItem[0].Type == KeyActionType.TapRepeat || lstActionItem[0].Type == KeyActionType.Script)
			{
				if ((int)e.Key == (0x3A7FFEAA ^ 0x3A7FFEA8) || (int)e.SystemKey == -103421 + (423622117 >> 1679909420))
				{
					((FrameworkElement)mKeyTextBox).Tag = string.Empty;
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
				}
				else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
				{
					if ((int)e.SystemKey == 896995408 - (448497644 << 1025289057) || (int)e.SystemKey == -1447848837 - ~1447848957 || (int)e.SystemKey == 600464596 + -600464497)
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
					}
					else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers != 0)
					{
						if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == 1)
						{
							UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
						}
						else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == -13115374 + 1083735007 % 535309814)
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
				if ((int)e.Key == 1405131586 + ~1405131429 && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
				{
					((FrameworkElement)mKeyTextBox).Tag = IMAPKeys.GetStringForFile(e.SystemKey);
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
				}
				else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
				{
					((FrameworkElement)mKeyTextBox).Tag = IMAPKeys.GetStringForFile(e.Key);
					BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
				}
				else
				{
					Key key2 = e.Key;
					int num2 = ((915276829 > 576964436) ? 2 : 2);
					if ((int)key2 == num2)
					{
						((FrameworkElement)mKeyTextBox).Tag = string.Empty;
						BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + string.Empty);
					}
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
			KeyActionType type = lstActionItem.First().Type;
			int num3 = ((495549638 > 1918500025) ? 26 : 20);
			if (type == (KeyActionType)num3 && ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
			{
				KMManager.AssignEdgeScrollMode(((FrameworkElement)mKeyTextBox).Tag.ToString(), mKeyTextBox);
			}
			((RoutedEventArgs)e).Handled = true;
		}
		if (PropertyType.Equals(typeof(int)) && lstActionItem.First().Type == KeyActionType.FreeLook && KMManager.CanvasWindow.mCanvasElement != null)
		{
			KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(lstActionItem.First());
		}
		if (string.Equals(ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase) && ((int)e.Key == 167772162 - (1907582474 << 566786680) || (int)e.SystemKey == 2130497976 - (0x3EA8C1A4 | 0x7CD45016)))
		{
			((FrameworkElement)mKeyTextBox).Tag = string.Empty;
			BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
		}
		if (ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) && (lstActionItem[0].Type == KeyActionType.Tap || lstActionItem[0].Type == KeyActionType.TapRepeat || lstActionItem[0].Type == KeyActionType.Script) && (int)e.Key == -1556830076 - -1556830079)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private void KeyTextBox_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (lstActionItem[0].Type != KeyActionType.Tap)
		{
			KeyActionType type = lstActionItem[0].Type;
			int num = ((2147220360 > 1844516765) ? 13 : 17);
			if (type != (KeyActionType)num && lstActionItem[0].Type != KeyActionType.Script)
			{
				return;
			}
		}
		if (mKeyList.Count >= 1602183267 + -1602183265)
		{
			string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - (-2134598849 - -2134598851))) + " + " + IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 1));
			string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - (0x7F2D668D ^ 0x7F2D668F))) + " + " + IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 1));
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
		if (!ActionItemProperty.Equals("EnableCondition", StringComparison.InvariantCultureIgnoreCase) && !ActionItemProperty.Equals("StartCondition", StringComparison.InvariantCultureIgnoreCase) && !ActionItemProperty.Equals("Note", (StringComparison)(-710978337 - -710978340)))
		{
			mKeyTextBox.CaretIndex = mKeyTextBox.Text.Length;
		}
		mKeyList.Clear();
	}

	private void KeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Invalid comparison between Unknown and I4
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
		if (!PropertyType.Equals(typeof(bool)))
		{
			return;
		}
		((FrameworkElement)mKeyTextBox).Tag = !Convert.ToBoolean(lstActionItem.First()[ActionItemProperty], CultureInfo.InvariantCulture);
		BlueStacksUIBinding.Bind(mKeyTextBox, Constants.ImapLocaleStringsConstant + ((FrameworkElement)mKeyTextBox).Tag);
		KeyActionType type = lstActionItem.First().Type;
		int num = ((1253212382 > 665050744) ? 20 : 26);
		if (type == (KeyActionType)num)
		{
			string actionItemProperty = ActionItemProperty;
			int comparisonType = ((1369048443 > 2038913438) ? 4 : 3);
			if (actionItemProperty.Equals("EdgeScrollEnabled", (StringComparison)comparisonType))
			{
				KMManager.AssignEdgeScrollMode(((FrameworkElement)mKeyTextBox).Tag.ToString(), mKeyTextBox);
			}
		}
	}

	internal bool AddActionItem(IMAction action)
	{
		PropertyType = IMAction.DictPropertyInfo[action.Type][ActionItemProperty].PropertyType;
		if (!PropertyType.Equals(typeof(string)) && !string.Equals(ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "GamepadSensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ActionItemProperty, "MouseAcceleration", (StringComparison)(-207175133 - -207175136)))
		{
			goto IL_0124;
		}
		if (action.Type == (KeyActionType)(-1545541639 - -1545541654))
		{
			string actionItemProperty = ActionItemProperty;
			int comparisonType = ((1057608839 > 688427638) ? 3 : 4);
			if (string.Equals(actionItemProperty, "Name", (StringComparison)comparisonType) || string.Equals(ActionItemProperty, "Model", StringComparison.InvariantCultureIgnoreCase))
			{
				goto IL_0124;
			}
		}
		((UIElement)mKeyPropertyNameTextBox).IsEnabled = true;
		string text = mActionItemProperty;
		int comparisonType2 = ((331687675 > 1180787028) ? 2 : 2);
		int num = text.IndexOf("_alt1", (StringComparison)comparisonType2);
		string origKey = mActionItemProperty;
		if (num > 0)
		{
			origKey = mActionItemProperty.Substring(0, num);
		}
		AssignGuidanceText(action, origKey);
		goto IL_0184;
		IL_0124:
		((UIElement)mKeyPropertyNameTextBox).IsEnabled = false;
		goto IL_0184;
		IL_0243:
		lstActionItem.Add(action);
		string text2 = action[ActionItemProperty].ToString();
		origKey = mActionItemProperty;
		if (mActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
		{
			string text3 = mActionItemProperty;
			int comparisonType3 = ((1354657339 > 1370148369) ? 2 : 2);
			int num2 = text3.IndexOf("_alt1", (StringComparison)comparisonType3);
			if (num2 > 0)
			{
				origKey = mActionItemProperty.Substring(0, num2);
			}
		}
		if (IsAddDirectionAttribute)
		{
			TextBox obj = mKeyPropertyName;
			string[] array = new string[-1876809401 + (0x2A94D216 | 0x45D984BE)];
			array[0] = Constants.ImapLocaleStringsConstant;
			array[1] = action.Type.ToString();
			array[-1891579886 ^ -1891579888] = "_";
			array[-110733058 - -110733061] = origKey;
			array[303598796 - 303598792 % 1050234258] = action.Direction.ToString();
			BlueStacksUIBinding.Bind(obj, string.Concat(array));
		}
		else
		{
			BlueStacksUIBinding.Bind(mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + origKey);
		}
		((FrameworkElement)mKeyTextBox).Tag = action[ActionItemProperty];
		if (ActionItemProperty.StartsWith("Key", StringComparison.CurrentCultureIgnoreCase))
		{
			BlueStacksUIBinding.Bind(mKeyTextBox, KMManager.GetStringsToShowInUI(text2));
		}
		else
		{
			mKeyTextBox.Text = text2;
		}
		if (lstActionItem.First().Type == KeyActionType.EdgeScroll && ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
		{
			KMManager.AssignEdgeScrollMode(text2, mKeyTextBox);
		}
		if (UsefulExtensionMethod.Contains(text2, "Gamepad", (StringComparison)(327646040 + -327646037)) || UsefulExtensionMethod.Contains(ActionItemProperty, "Gamepad", (StringComparison)(368161390 + -368161387)))
		{
			BlueStacksUIBinding.Bind(mKeyTextBox, KMManager.GetKeyUIValue(text2));
			((FrameworkElement)mKeyTextBox).ToolTip = mKeyTextBox.Text;
			return true;
		}
		return false;
		IL_0184:
		if (action.Type != KeyActionType.Zoom)
		{
			KeyActionType type = action.Type;
			int num3 = ((856385867 > 300856026) ? 18 : 24);
			if (type != (KeyActionType)num3)
			{
				goto IL_0243;
			}
		}
		if (!string.Equals(ActionItemProperty, "Speed", StringComparison.InvariantCultureIgnoreCase))
		{
			string actionItemProperty2 = ActionItemProperty;
			int comparisonType4 = ((920045288 > 2041265207) ? 4 : 3);
			if (!string.Equals(actionItemProperty2, "Acceleration", (StringComparison)comparisonType4) && !string.Equals(ActionItemProperty, "Amplitude", StringComparison.InvariantCultureIgnoreCase))
			{
				goto IL_0243;
			}
		}
		((UIElement)mKeyPropertyNameTextBox).IsEnabled = true;
		AssignGuidanceText(action, mActionItemProperty);
		goto IL_0243;
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
		else if (PropertyType.Equals(typeof(int)))
		{
			if (int.TryParse(mKeyTextBox.Text, out var _))
			{
				text = mKeyTextBox.Text;
			}
			else if (!string.IsNullOrEmpty(mKeyTextBox.Text))
			{
				mKeyTextBox.Text = text;
			}
		}
		else if (PropertyType.Equals(typeof(bool)))
		{
			text = ((FrameworkElement)mKeyTextBox).Tag.ToString();
		}
		else
		{
			if (!ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
			{
				string actionItemProperty = ActionItemProperty;
				int comparisonType = ((2018725432 > 155433818) ? 3 : 4);
				if (!actionItemProperty.StartsWith("Gamepad", (StringComparison)comparisonType))
				{
					text = mKeyTextBox.Text;
					goto IL_02c8;
				}
			}
			text = ((FrameworkElement)mKeyTextBox).Tag.ToString();
		}
		goto IL_02c8;
		IL_02c8:
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
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
