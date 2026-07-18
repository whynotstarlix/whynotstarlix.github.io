using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ShortcutKeyControlElement : UserControl, IComponentConnector
{
	internal MainWindow ParentWindow;

	internal SettingsWindow ParentSettingsWindow;

	internal string mDefaultModifierForUI = string.Empty;

	internal string mDefaultModifierForFile = string.Empty;

	private bool mErrorMessageShown;

	internal bool mIsShortcutSameAsMacroShortcut;

	private CustomToastPopupControl mToastPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mShortcutNameTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mShortcutKeyTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mKeyInfoPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mKeyInfoText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mDownArrow;

	private bool _contentLoaded;

	internal List<ShortcutKeys> mUserDefinedConfigList { get; set; }

	public ShortcutKeyControlElement(MainWindow window, SettingsWindow settingsWindow)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		ParentWindow = window;
		ParentSettingsWindow = settingsWindow;
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mShortcutKeyTextBox, false);
		MainWindow parentWindow = ParentWindow;
		string[] array = ((parentWindow != null) ? parentWindow.mCommonHandler.mShortcutsConfigInstance.DefaultModifier.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null);
		foreach (string value in array)
		{
			Key val = (Key)Enum.Parse(typeof(Key), value);
			mDefaultModifierForUI = mDefaultModifierForUI + IMAPKeys.GetStringForUI(val) + " + ";
			mDefaultModifierForFile = mDefaultModifierForFile + IMAPKeys.GetStringForFile(val) + " + ";
		}
	}

	private static bool IsValid(Key key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if ((int)key == 120 || (int)key == 121 || (int)key == 116 || (int)key == 117 || (int)key == 118 || (int)key == 119 || (int)key == 0 || (int)key == 156)
		{
			return false;
		}
		return true;
	}

	private void ShortcutKeyTextBoxKeyUp(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if ((int)e.Key == 30 || (int)e.SystemKey == 30)
		{
			HandleShortcutKeyDown(e);
		}
	}

	private void ShortcutKeyTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		HandleShortcutKeyDown(e);
	}

	private void HandleShortcutKeyDown(KeyEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Invalid comparison between Unknown and I4
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Invalid comparison between Unknown and I4
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Invalid comparison between Unknown and I4
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Invalid comparison between Unknown and I4
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Invalid comparison between Unknown and I4
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Invalid comparison between Unknown and I4
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Invalid comparison between Unknown and I4
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		Logger.Debug("SHORTCUT: PrintKey............" + ((object)e.Key/*cast due to constrained. prefix*/).ToString());
		Logger.Debug("SHORTCUT: PrintSystemKey............" + ((object)e.SystemKey/*cast due to constrained. prefix*/).ToString());
		if (((!IMAPKeys.mDictKeys.ContainsKey(e.Key) && !IMAPKeys.mDictKeys.ContainsKey(e.SystemKey)) || (!IsValid(e.Key) && !IsValid(e.SystemKey))) && (int)e.Key != 2 && (int)e.Key != 32)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		((FrameworkElement)mShortcutKeyTextBox).Tag = string.Empty;
		if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers != 0)
		{
			if (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119))
			{
				text = IMAPKeys.GetStringForUI((Key)118) + " + ";
				((FrameworkElement)mShortcutKeyTextBox).Tag = IMAPKeys.GetStringForFile((Key)118) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121))
			{
				text2 = IMAPKeys.GetStringForUI((Key)120) + " + ";
				CustomTextBox obj = mShortcutKeyTextBox;
				((FrameworkElement)obj).Tag = ((FrameworkElement)obj).Tag?.ToString() + IMAPKeys.GetStringForFile((Key)120) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)116) || Keyboard.IsKeyDown((Key)117))
			{
				text3 = IMAPKeys.GetStringForUI((Key)116) + " + ";
				CustomTextBox obj2 = mShortcutKeyTextBox;
				((FrameworkElement)obj2).Tag = ((FrameworkElement)obj2).Tag?.ToString() + IMAPKeys.GetStringForFile((Key)116) + " + ";
			}
			if ((string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2)) || (int)e.SystemKey == 99)
			{
				((TextBox)mShortcutKeyTextBox).Text = text + text2 + text3 + IMAPKeys.GetStringForUI(e.SystemKey);
				CustomTextBox obj3 = mShortcutKeyTextBox;
				((FrameworkElement)obj3).Tag = ((FrameworkElement)obj3).Tag?.ToString() + IMAPKeys.GetStringForFile(e.SystemKey);
			}
			else
			{
				((TextBox)mShortcutKeyTextBox).Text = text + text2 + text3 + IMAPKeys.GetStringForUI(e.Key);
				CustomTextBox obj4 = mShortcutKeyTextBox;
				((FrameworkElement)obj4).Tag = ((FrameworkElement)obj4).Tag?.ToString() + IMAPKeys.GetStringForFile(e.Key);
			}
		}
		else if ((int)e.Key == 2 || (int)e.Key == 32)
		{
			((TextBox)mShortcutKeyTextBox).Text = string.Empty;
			((FrameworkElement)mShortcutKeyTextBox).Tag = string.Empty;
			if (ParentSettingsWindow.mDuplicateShortcutsList.Contains(mShortcutNameTextBlock.Text))
			{
				ParentSettingsWindow.mDuplicateShortcutsList.Remove(mShortcutNameTextBlock.Text);
			}
			SetSaveButtonState(ParentSettingsWindow.mIsShortcutEdited);
		}
		else if ((int)e.Key == 13)
		{
			if (string.Equals(mDefaultModifierForFile, "Shift + ", StringComparison.InvariantCulture))
			{
				((TextBox)mShortcutKeyTextBox).Text = mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
				((FrameworkElement)mShortcutKeyTextBox).Tag = mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key);
			}
		}
		else if (((int)e.Key == 34 || (int)e.SystemKey == 34) && string.Equals(mDefaultModifierForUI, "Ctrl + Shift + ", StringComparison.InvariantCulture))
		{
			((TextBox)mShortcutKeyTextBox).Text = string.Empty;
			((FrameworkElement)mShortcutKeyTextBox).Tag = string.Empty;
			AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
		}
		else if ((int)e.Key == 156)
		{
			((TextBox)mShortcutKeyTextBox).Text = mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.SystemKey);
			((FrameworkElement)mShortcutKeyTextBox).Tag = mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.SystemKey);
		}
		else
		{
			((TextBox)mShortcutKeyTextBox).Text = mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
			((FrameworkElement)mShortcutKeyTextBox).Tag = mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key);
		}
		((RoutedEventArgs)e).Handled = true;
		((TextBox)mShortcutKeyTextBox).CaretIndex = ((TextBox)mShortcutKeyTextBox).Text.Length;
		mIsShortcutSameAsMacroShortcut = false;
		if ((MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.Key)) || MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.SystemKey))) && (string.Equals(((TextBox)mShortcutKeyTextBox).Text, text + text2 + IMAPKeys.GetStringForUI(e.Key), StringComparison.InvariantCulture) || string.Equals(((TextBox)mShortcutKeyTextBox).Text, text + text2 + IMAPKeys.GetStringForUI(e.SystemKey), StringComparison.InvariantCulture)))
		{
			mIsShortcutSameAsMacroShortcut = true;
		}
		if (string.Equals(((TextBox)mShortcutKeyTextBox).Text, "Alt + F4", StringComparison.InvariantCulture))
		{
			((TextBox)mShortcutKeyTextBox).Text = string.Empty;
			((FrameworkElement)mShortcutKeyTextBox).Tag = string.Empty;
			AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
		}
		foreach (ShortcutKeys mUserDefinedConfig in mUserDefinedConfigList)
		{
			((XTextBox)mShortcutKeyTextBox).InputTextValidity = (TextValidityOptions)1;
			((Popup)mKeyInfoPopup).IsOpen = false;
			ParentSettingsWindow.mIsShortcutEdited = true;
			CheckIfShortcutAlreadyUsed();
			ParentWindow.mCommonHandler.OnShortcutKeysRefresh();
			if (string.Equals(LocaleStrings.GetLocalizedString(mUserDefinedConfig.ShortcutName, ""), mShortcutNameTextBlock.Text, StringComparison.InvariantCulture) && !string.Equals(mUserDefinedConfig.ShortcutKey, ((TextBox)mShortcutKeyTextBox).Text, StringComparison.InvariantCulture))
			{
				mUserDefinedConfig.ShortcutKey = ((FrameworkElement)mShortcutKeyTextBox).Tag.ToString();
				Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_edit", mShortcutNameTextBlock.Text, (string)null, (string)null, (string)null, (string)null, "Android", 0);
			}
		}
	}

	private void AddToastPopup(string message)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		try
		{
			if (mToastPopup == null)
			{
				mToastPopup = new CustomToastPopupControl((UserControl)(object)ParentSettingsWindow);
			}
			mToastPopup.Init((UserControl)(object)ParentSettingsWindow, message, (Brush)null, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)0, (Thickness?)null, 12, (Thickness?)null, (Brush)null);
			((FrameworkElement)mToastPopup).Margin = new Thickness(20.0, 30.0, 0.0, 0.0);
			mToastPopup.ShowPopup(1.3);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in showing toast popup: " + ex.ToString());
		}
	}

	private void CheckIfShortcutAlreadyUsed()
	{
		mErrorMessageShown = false;
		foreach (ShortcutKeys item in ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
		{
			if ((!string.IsNullOrEmpty(item.ShortcutKey) && string.Equals(item.ShortcutKey, ((FrameworkElement)mShortcutKeyTextBox).Tag.ToString(), StringComparison.InvariantCulture) && !string.Equals(LocaleStrings.GetLocalizedString(item.ShortcutName, ""), mShortcutNameTextBlock.Text, StringComparison.InvariantCulture)) || mIsShortcutSameAsMacroShortcut)
			{
				((Popup)mKeyInfoPopup).PlacementTarget = (UIElement)(object)mShortcutKeyTextBox;
				((XTextBox)mShortcutKeyTextBox).InputTextValidity = (TextValidityOptions)(-1);
				((Popup)mKeyInfoPopup).IsOpen = true;
				mErrorMessageShown = true;
				if (!ParentSettingsWindow.mDuplicateShortcutsList.Contains(mShortcutNameTextBlock.Text))
				{
					ParentSettingsWindow.mDuplicateShortcutsList.Add(mShortcutNameTextBlock.Text);
				}
			}
		}
		if (!mErrorMessageShown && ParentSettingsWindow.mDuplicateShortcutsList.Contains(mShortcutNameTextBlock.Text))
		{
			ParentSettingsWindow.mDuplicateShortcutsList.Remove(mShortcutNameTextBlock.Text);
		}
		SetSaveButtonState(ParentSettingsWindow.mIsShortcutEdited);
	}

	private void SetSaveButtonState(bool isEdited)
	{
		if (ParentSettingsWindow.mDuplicateShortcutsList.Count == 0 && isEdited)
		{
			ParentWindow.mCommonHandler.OnShortcutKeysChanged(isEnabled: true);
			ParentSettingsWindow.mIsShortcutSaveBtnEnabled = true;
		}
		else
		{
			ParentWindow.mCommonHandler.OnShortcutKeysChanged(isEnabled: false);
			ParentSettingsWindow.mIsShortcutSaveBtnEnabled = false;
		}
	}

	private void ShortcutKeyTextBoxMouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((XTextBox)mShortcutKeyTextBox).InputTextValidity == -1)
		{
			((Popup)mKeyInfoPopup).PlacementTarget = (UIElement)(object)mShortcutKeyTextBox;
			((Popup)mKeyInfoPopup).IsOpen = true;
			((Popup)mKeyInfoPopup).StaysOpen = true;
		}
		else
		{
			((Popup)mKeyInfoPopup).IsOpen = false;
		}
	}

	private void ShortcutKeyTextBoxMouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mKeyInfoPopup).IsOpen = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeycontrolelement.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mShortcutNameTextBlock = (TextBlock)target;
			break;
		case 2:
			mShortcutKeyTextBox = (CustomTextBox)target;
			((UIElement)mShortcutKeyTextBox).PreviewKeyDown += new KeyEventHandler(ShortcutKeyTextBoxKeyDown);
			((UIElement)mShortcutKeyTextBox).MouseEnter += new MouseEventHandler(ShortcutKeyTextBoxMouseEnter);
			((UIElement)mShortcutKeyTextBox).MouseLeave += new MouseEventHandler(ShortcutKeyTextBoxMouseLeave);
			((UIElement)mShortcutKeyTextBox).PreviewKeyUp += new KeyEventHandler(ShortcutKeyTextBoxKeyUp);
			break;
		case 3:
			mKeyInfoPopup = (CustomPopUp)target;
			break;
		case 4:
			mMaskBorder = (Border)target;
			break;
		case 5:
			mKeyInfoText = (TextBlock)target;
			break;
		case 6:
			mDownArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
