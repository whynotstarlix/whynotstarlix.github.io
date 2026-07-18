using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class IMapTextBox : XTextBox, IComponentConnector
{
	private List<Key> mKeyList = new List<Key>();

	public static readonly DependencyProperty IsKeyBoardInFocusProperty = DependencyProperty.Register("IsKeyBoardInFocus", typeof(bool), typeof(IMapTextBox), new PropertyMetadata((object)false, new PropertyChangedCallback(OnKeyBoardInFocusChanged)));

	public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register("PropertyType", typeof(Type), typeof(IMapTextBox), new PropertyMetadata());

	public static readonly DependencyProperty ActionTypeProperty = DependencyProperty.Register("ActionType", typeof(KeyActionType), typeof(IMapTextBox), new PropertyMetadata());

	public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register("IMActionItems", typeof(ObservableCollection<IMActionItem>), typeof(IMapTextBox), new PropertyMetadata());

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal IMapTextBox mTextBox;

	private bool _contentLoaded;

	public bool IsKeyBoardInFocus
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsKeyBoardInFocusProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsKeyBoardInFocusProperty, (object)value);
		}
	}

	public Type PropertyType
	{
		get
		{
			return (Type)((DependencyObject)this).GetValue(PropertyTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PropertyTypeProperty, (object)value);
		}
	}

	public KeyActionType ActionType
	{
		get
		{
			return (KeyActionType)((DependencyObject)this).GetValue(ActionTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ActionTypeProperty, (object)value);
		}
	}

	public ObservableCollection<IMActionItem> IMActionItems
	{
		get
		{
			return (ObservableCollection<IMActionItem>)((DependencyObject)this).GetValue(IMActionItemsProperty);
		}
		set
		{
			if (value == null)
			{
				((DependencyObject)this).ClearValue(IMActionItemsProperty);
			}
			else
			{
				((DependencyObject)this).SetValue(IMActionItemsProperty, (object)value);
			}
		}
	}

	public IMapTextBox()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		InitializeComponent();
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)this, false);
		((DependencyObject)this).ClearValue(IMActionItemsProperty);
		((FrameworkElement)this).Loaded += new RoutedEventHandler(IMapTextBox_Loaded);
	}

	private static void OnKeyBoardInFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		if (sender is IMapTextBox mapTextBox && bool.TryParse(((DependencyPropertyChangedEventArgs)(ref args)).NewValue.ToString(), out var result))
		{
			KMManager.CurrentIMapTextBox = (result ? mapTextBox : null);
		}
	}

	private void IMapTextBox_Loaded(object sender, RoutedEventArgs e)
	{
		if (((XTextBox)this).TextBlock != null)
		{
			((XTextBox)this).TextBlock.TextTrimming = (TextTrimming)0;
			((XTextBox)this).TextBlock.TextWrapping = (TextWrapping)2;
		}
		if (string.IsNullOrEmpty(((FrameworkElement)this).Tag.ToString()))
		{
			return;
		}
		string[] array = ((FrameworkElement)this).Tag.ToString().Split(new char[1] { '+' });
		if (array.Length == 0)
		{
			return;
		}
		if (UsefulExtensionMethod.Contains(IMActionItems[0].ActionItem, "_alt1", StringComparison.InvariantCultureIgnoreCase) || UsefulExtensionMethod.Contains(IMActionItems[0].ActionItem, "Gamepad", StringComparison.InvariantCultureIgnoreCase))
		{
			((TextBox)this).Text = string.Join(" + ", (from x in array.ToList()
				select LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.CheckForGamepadSuffix(x.Trim())), "")).ToArray());
		}
		else
		{
			((TextBox)this).Text = string.Join(" + ", (from x in array.ToList()
				select LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(x.Trim()), "")).ToArray());
		}
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		((FrameworkElement)this).OnGotFocus(e);
		KMManager.CurrentIMapTextBox = this;
		KMManager.pressedGamepadKeyList.Clear();
		KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow);
		((TextBoxBase)this).TextChanged -= new TextChangedEventHandler(IMapTextBox_TextChanged);
		((TextBoxBase)this).TextChanged += new TextChangedEventHandler(IMapTextBox_TextChanged);
		((UIElement)this).PreviewMouseWheel -= new MouseWheelEventHandler(IMapTextBox_PreviewMouseWheel);
		((UIElement)this).PreviewMouseWheel += new MouseWheelEventHandler(IMapTextBox_PreviewMouseWheel);
		SetCaretIndex();
	}

	private void IMapTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
	{
		if (args != null && args.Delta != 0 && IMActionItems != null && IMActionItems.Any())
		{
			foreach (IMActionItem iMActionItem in IMActionItems)
			{
				if (iMActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
				{
					((FrameworkElement)this).Tag = ((args.Delta < 0) ? "MouseWheelDown" : "MouseWheelUp");
					SetValueHandling(iMActionItem);
					BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(((FrameworkElement)this).Tag.ToString()));
				}
				if (PropertyType.Equals(typeof(bool)))
				{
					bool flag = !Convert.ToBoolean(iMActionItem.IMAction[iMActionItem.ActionItem], CultureInfo.InvariantCulture);
					((FrameworkElement)this).Tag = flag;
					Setvalue(iMActionItem, flag.ToString(CultureInfo.InvariantCulture));
					BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + ((FrameworkElement)this).Tag);
					((RoutedEventArgs)args).Handled = true;
				}
			}
			((RoutedEventArgs)args).Handled = true;
		}
		SetCaretIndex();
	}

	private void IMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (IMActionItems != null && IMActionItems.Any())
		{
			foreach (IMActionItem iMActionItem in IMActionItems)
			{
				SetValueHandling(iMActionItem);
			}
			KMManager.CheckAndCreateNewScheme();
		}
		SetCaretIndex();
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		((TextBoxBase)this).TextChanged -= new TextChangedEventHandler(IMapTextBox_TextChanged);
		((UIElement)this).PreviewMouseWheel -= new MouseWheelEventHandler(IMapTextBox_PreviewMouseWheel);
		KMManager.CurrentIMapTextBox = null;
		((XTextBox)this).InputTextValidity = (TextValidityOptions)1;
		object toolTip = ((FrameworkElement)this).ToolTip;
		ToolTip val = (ToolTip)((toolTip is ToolTip) ? toolTip : null);
		if (val != null)
		{
			val.IsOpen = false;
		}
		KMManager.CurrentIMapTextBox = null;
		((TextBoxBase)this).OnLostFocus(e);
	}

	protected override void OnPreviewKeyDown(KeyEventArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Invalid comparison between Unknown and I4
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Invalid comparison between Unknown and I4
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Invalid comparison between Unknown and I4
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Invalid comparison between Unknown and I4
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Invalid comparison between Unknown and I4
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Invalid comparison between Unknown and I4
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Invalid comparison between Unknown and I4
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Invalid comparison between Unknown and I4
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (args != null && (int)args.Key != 13)
		{
			if (IMActionItems != null && IMActionItems.Any())
			{
				foreach (IMActionItem iMActionItem in IMActionItems)
				{
					if (iMActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
					{
						if (iMActionItem.IMAction.Type == KeyActionType.Tap || iMActionItem.IMAction.Type == KeyActionType.TapRepeat || iMActionItem.IMAction.Type == KeyActionType.Script)
						{
							if ((int)args.Key == 2 || (int)args.SystemKey == 2)
							{
								((FrameworkElement)this).Tag = string.Empty;
								BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + ((FrameworkElement)this).Tag);
							}
							else if (IMAPKeys.mDictKeys.ContainsKey(args.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(args.Key))
							{
								if ((int)args.SystemKey == 120 || (int)args.SystemKey == 121 || (int)args.SystemKey == 99)
								{
									UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, args.SystemKey);
								}
								else if ((int)((KeyboardEventArgs)args).KeyboardDevice.Modifiers != 0)
								{
									if ((int)((KeyboardEventArgs)args).KeyboardDevice.Modifiers == 1)
									{
										UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, args.SystemKey);
									}
									else if ((int)((KeyboardEventArgs)args).KeyboardDevice.Modifiers == 5)
									{
										UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, args.SystemKey);
									}
									else
									{
										UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, args.Key);
									}
								}
								else
								{
									UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, args.Key);
								}
							}
						}
						else
						{
							if ((int)args.Key == 156 && IMAPKeys.mDictKeys.ContainsKey(args.SystemKey))
							{
								((FrameworkElement)this).Tag = IMAPKeys.GetStringForFile(args.SystemKey);
								BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.SystemKey));
							}
							else if (IMAPKeys.mDictKeys.ContainsKey(args.Key))
							{
								((FrameworkElement)this).Tag = IMAPKeys.GetStringForFile(args.Key);
								BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.Key));
							}
							else if ((int)args.Key == 2)
							{
								((FrameworkElement)this).Tag = string.Empty;
								BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + string.Empty);
							}
							((RoutedEventArgs)args).Handled = true;
						}
					}
					if (string.Equals(iMActionItem.ActionItem, "GamepadStick", StringComparison.InvariantCulture))
					{
						if ((int)args.Key == 2 || (int)args.Key == 32)
						{
							((FrameworkElement)this).Tag = string.Empty;
							BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + string.Empty);
						}
						((RoutedEventArgs)args).Handled = true;
					}
					if (PropertyType.Equals(typeof(bool)))
					{
						bool flag = !Convert.ToBoolean(iMActionItem.IMAction[iMActionItem.ActionItem], CultureInfo.InvariantCulture);
						((FrameworkElement)this).Tag = flag;
						Setvalue(iMActionItem, flag.ToString(CultureInfo.InvariantCulture));
						BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + ((FrameworkElement)this).Tag);
						if (iMActionItem.IMAction.Type == KeyActionType.EdgeScroll && iMActionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
						{
							KMManager.AssignEdgeScrollMode(flag.ToString(CultureInfo.InvariantCulture), (TextBox)(object)this);
						}
						((RoutedEventArgs)args).Handled = true;
					}
				}
			}
			((UIElement)this).Focus();
			((RoutedEventArgs)args).Handled = true;
		}
		if (PropertyType.Equals(typeof(bool)))
		{
			KMManager.CheckAndCreateNewScheme();
		}
		SetCaretIndex();
		((UIElement)this).OnPreviewKeyDown(args);
	}

	protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
	{
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Invalid comparison between Unknown and I4
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Invalid comparison between Unknown and I4
		if (args != null)
		{
			if (IMActionItems != null && IMActionItems.Any())
			{
				foreach (IMActionItem iMActionItem in IMActionItems)
				{
					if (iMActionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
					{
						if ((int)((MouseEventArgs)args).MiddleButton == 1)
						{
							((RoutedEventArgs)args).Handled = true;
							((FrameworkElement)this).Tag = "MouseMButton";
							BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + "MouseMButton");
						}
						else if ((int)((MouseEventArgs)args).RightButton == 1)
						{
							((RoutedEventArgs)args).Handled = true;
							((FrameworkElement)this).Tag = "MouseRButton";
							BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + "MouseRButton");
						}
						else if ((int)((MouseEventArgs)args).XButton1 == 1)
						{
							((RoutedEventArgs)args).Handled = true;
							((FrameworkElement)this).Tag = "MouseXButton1";
							BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + "MouseXButton1");
						}
						else if ((int)((MouseEventArgs)args).XButton2 == 1)
						{
							((RoutedEventArgs)args).Handled = true;
							((FrameworkElement)this).Tag = "MouseXButton2";
							BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + "MouseXButton2");
						}
					}
					if (PropertyType.Equals(typeof(bool)))
					{
						bool flag = !Convert.ToBoolean(iMActionItem.IMAction[iMActionItem.ActionItem], CultureInfo.InvariantCulture);
						((FrameworkElement)this).Tag = flag;
						Setvalue(iMActionItem, flag.ToString(CultureInfo.InvariantCulture));
						BlueStacksUIBinding.Bind((TextBox)(object)this, Constants.ImapLocaleStringsConstant + ((FrameworkElement)this).Tag);
						if (iMActionItem.IMAction.Type == KeyActionType.EdgeScroll && iMActionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
						{
							KMManager.AssignEdgeScrollMode(flag.ToString(CultureInfo.InvariantCulture), (TextBox)(object)this);
						}
					}
				}
			}
			if ((int)((MouseEventArgs)args).LeftButton == 1 && ((UIElement)this).IsKeyboardFocusWithin)
			{
				((RoutedEventArgs)args).Handled = true;
			}
			((UIElement)this).Focus();
			((RoutedEventArgs)args).Handled = true;
		}
		if (PropertyType.Equals(typeof(bool)))
		{
			KMManager.CheckAndCreateNewScheme();
		}
		SetCaretIndex();
		((UIElement)this).OnPreviewMouseDown(args);
	}

	protected override void OnKeyUp(KeyEventArgs args)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (args != null)
		{
			if (IMActionItems != null && IMActionItems.Any())
			{
				foreach (IMActionItem iMActionItem in IMActionItems)
				{
					if (iMActionItem.IMAction.Type == KeyActionType.Tap || iMActionItem.IMAction.Type == KeyActionType.TapRepeat || iMActionItem.IMAction.Type == KeyActionType.Script)
					{
						if (mKeyList.Count >= 2)
						{
							string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 1));
							string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 1));
							((FrameworkElement)this).Tag = tag;
							((TextBox)this).Text = text;
							SetValueHandling(iMActionItem);
						}
						else if (mKeyList.Count == 1)
						{
							string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(0));
							string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(0));
							((FrameworkElement)this).Tag = tag;
							((TextBox)this).Text = text;
							SetValueHandling(iMActionItem);
						}
						mKeyList.Clear();
					}
				}
			}
			((RoutedEventArgs)args).Handled = true;
		}
		if (PropertyType.Equals(typeof(bool)))
		{
			KMManager.CheckAndCreateNewScheme();
		}
		SetCaretIndex();
		((TextBoxBase)this).OnKeyUp(args);
	}

	private void SetValueHandling(IMActionItem item)
	{
		string text = item.IMAction[item.ActionItem].ToString();
		if (((FrameworkElement)this).IsLoaded)
		{
			KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow);
		}
		int result2;
		if (PropertyType.Equals(typeof(double)))
		{
			if (double.TryParse(((TextBox)this).Text, out var _))
			{
				text = ((TextBox)this).Text;
			}
			else if (!string.IsNullOrEmpty(((TextBox)this).Text))
			{
				((TextBox)this).Text = text;
			}
		}
		else if (!PropertyType.Equals(typeof(int)))
		{
			text = ((!PropertyType.Equals(typeof(bool))) ? ((FrameworkElement)this).Tag.ToString() : ((FrameworkElement)this).Tag.ToString());
		}
		else if (int.TryParse(((TextBox)this).Text, out result2))
		{
			text = ((TextBox)this).Text;
		}
		else if (!string.IsNullOrEmpty(((TextBox)this).Text))
		{
			((TextBox)this).Text = text;
		}
		Setvalue(item, text);
	}

	internal static void Setvalue(IMActionItem item, string value)
	{
		if (!string.Equals(item.IMAction[item.ActionItem].ToString(), value, StringComparison.InvariantCulture))
		{
			item.IMAction[item.ActionItem] = value;
		}
		Logger.Debug("GUIDANCE: " + item.IMAction.Type);
	}

	private void SetCaretIndex()
	{
		if (!string.IsNullOrEmpty(((TextBox)this).Text))
		{
			((TextBox)this).CaretIndex = ((TextBox)this).Text.Length;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancemodels/imaptextbox.xaml", UriKind.Relative);
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
		if (connectionId == 1)
		{
			mTextBox = (IMapTextBox)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
