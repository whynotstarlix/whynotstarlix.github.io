using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ShortcutKeysControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private SettingsWindow ParentSettingsWindow;

	private StackPanel mShortcutKeyPanel;

	private CustomToastPopupControl mToastPopup;

	private Dictionary<string, Tuple<GroupBox, List<ShortcutKeyControlElement>>> mShortcutUIElements = new Dictionary<string, Tuple<GroupBox, List<ShortcutKeyControlElement>>>();

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mShortcutKeyScrollBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mRevertBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSaveBtn;

	private bool _contentLoaded;

	public ShortcutKeysControl(MainWindow window, SettingsWindow settingsWindow)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		InitializeComponent();
		((UIElement)this).Visibility = (Visibility)1;
		ParentWindow = window;
		ParentSettingsWindow = settingsWindow;
		object content = ((ContentControl)mShortcutKeyScrollBar).Content;
		mShortcutKeyPanel = (StackPanel)((content is StackPanel) ? content : null);
		if (ParentWindow != null)
		{
			if (ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
			{
				AddShortcutKeyElements();
			}
			if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
			{
				((UIElement)mRevertBtn).IsEnabled = true;
			}
			ParentWindow.mCommonHandler.ShortcutKeysChangedEvent += ShortcutKeysChangedEvent;
			ParentWindow.mCommonHandler.ShortcutKeysRefreshEvent += ShortcutKeysRefreshEvent;
		}
		mShortcutKeyScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
	}

	private void ShortcutKeysRefreshEvent()
	{
		IEnumerable<ShortcutKeyControlElement> enumerable = from ele in (from ele in mShortcutUIElements.SelectMany((KeyValuePair<string, Tuple<GroupBox, List<ShortcutKeyControlElement>>> x) => x.Value.Item2)
				group ele by ((TextBox)ele.mShortcutKeyTextBox).Text into grp
				where grp.Count() == 1
				select grp).SelectMany((IGrouping<string, ShortcutKeyControlElement> grp) => grp)
			where !string.IsNullOrEmpty(((TextBox)ele.mShortcutKeyTextBox).Text)
			select ele;
		int num = 0;
		foreach (ShortcutKeyControlElement item in enumerable)
		{
			if (!item.mIsShortcutSameAsMacroShortcut)
			{
				((Popup)item.mKeyInfoPopup).IsOpen = false;
				((XTextBox)item.mShortcutKeyTextBox).InputTextValidity = (TextValidityOptions)1;
				num++;
			}
		}
		if (num == (from ele in mShortcutUIElements.SelectMany((KeyValuePair<string, Tuple<GroupBox, List<ShortcutKeyControlElement>>> x) => x.Value.Item2)
			where !string.IsNullOrEmpty(((TextBox)ele.mShortcutKeyTextBox).Text)
			select ele).Count())
		{
			ParentWindow.mCommonHandler.OnShortcutKeysChanged(isEnabled: true);
		}
		else
		{
			ParentWindow.mCommonHandler.OnShortcutKeysChanged(isEnabled: false);
		}
	}

	private void ShortcutKeysChangedEvent(bool isEnabled)
	{
		((UIElement)mSaveBtn).IsEnabled = isEnabled;
	}

	private void AddShortcutKeyElements()
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			new List<ShortcutKeys>();
			((Panel)mShortcutKeyPanel).Children.Clear();
			mShortcutUIElements.Clear();
			foreach (ShortcutKeys item in ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
			{
				CreateShortcutCategory(item.ShortcutCategory);
				AddElement(item);
			}
			foreach (KeyValuePair<string, Tuple<GroupBox, List<ShortcutKeyControlElement>>> mShortcutUIElement in mShortcutUIElements)
			{
				((Panel)mShortcutKeyPanel).Children.Add((UIElement)(object)((Tuple<GroupBox>)(object)mShortcutUIElement.Value).Item1);
				foreach (ShortcutKeyControlElement item2 in mShortcutUIElement.Value.Item2)
				{
					object content = ((ContentControl)((Tuple<GroupBox>)(object)mShortcutUIElement.Value).Item1).Content;
					((Panel)((content is StackPanel) ? content : null)).Children.Add((UIElement)(object)item2);
				}
			}
			((FrameworkElement)((Tuple<GroupBox>)(object)mShortcutUIElements.First().Value).Item1).Margin = new Thickness(0.0);
		}
		catch (Exception ex)
		{
			Logger.Error("Error in adding shortcut elements: " + ex.ToString());
		}
	}

	private void AddElement(ShortcutKeys ele)
	{
		ShortcutKeyControlElement shortcutKeyControlElement = new ShortcutKeyControlElement(ParentWindow, ParentSettingsWindow);
		BlueStacksUIBinding.Bind(shortcutKeyControlElement.mShortcutNameTextBlock, ele.ShortcutName, "");
		string[] array = ele.ShortcutKey.Split(new char[2] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		string text = string.Empty;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			text = text + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text2), "") + " + ";
		}
		mShortcutUIElements[ele.ShortcutCategory].Item2.Add(shortcutKeyControlElement);
		if (!string.IsNullOrEmpty(text))
		{
			((TextBox)shortcutKeyControlElement.mShortcutKeyTextBox).Text = text.Substring(0, text.Length - 3);
		}
		shortcutKeyControlElement.mUserDefinedConfigList = new List<ShortcutKeys> { ele };
		if (ele.ReadOnlyTextbox)
		{
			((UIElement)shortcutKeyControlElement.mShortcutKeyTextBox).IsEnabled = false;
		}
	}

	private void CreateShortcutCategory(string categoryName)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		if (!mShortcutUIElements.ContainsKey(categoryName))
		{
			string localizedString = LocaleStrings.GetLocalizedString(categoryName, "");
			GroupBox val = new GroupBox
			{
				Content = (object)new StackPanel(),
				Header = localizedString,
				Tag = categoryName,
				Margin = new Thickness(0.0, 20.0, 0.0, 0.0),
				FontSize = 16.0
			};
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val, Control.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
			((Control)val).BorderThickness = new Thickness(0.0);
			BlueStacksUIBinding.BindColor((DependencyObject)new TextBlock
			{
				Text = localizedString,
				Tag = categoryName,
				FontStretch = FontStretches.ExtraExpanded,
				HorizontalAlignment = (HorizontalAlignment)1,
				Margin = new Thickness(0.0, 0.0, 0.0, 10.0),
				TextWrapping = (TextWrapping)0
			}, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
			mShortcutUIElements.Add(categoryName, new Tuple<GroupBox, List<ShortcutKeyControlElement>>(val, new List<ShortcutKeyControlElement>()));
		}
	}

	private void SaveBtnClick(object sender, RoutedEventArgs e)
	{
		ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
		AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
		ParentSettingsWindow.mIsShortcutEdited = false;
		((UIElement)mSaveBtn).IsEnabled = false;
		((UIElement)mRevertBtn).IsEnabled = true;
		if (ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut != null)
		{
			RefreshShortcutConfigForUI();
		}
		ParentWindow.mTopbarOptions?.SetLabel();
		ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Shortcut-Settings", "", null, ParentWindow.mVmName);
	}

	private void RefreshShortcutConfigForUI()
	{
		foreach (ShortcutKeys item in ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
		{
			foreach (ShortcutKeyControlElement item2 in mShortcutUIElements[item.ShortcutCategory].Item2)
			{
				if (item2 is ShortcutKeyControlElement shortcutKeyControlElement && string.Equals(shortcutKeyControlElement.mShortcutNameTextBlock.Text, LocaleStrings.GetLocalizedString(item.ShortcutName, ""), StringComparison.InvariantCulture))
				{
					shortcutKeyControlElement.mUserDefinedConfigList = new List<ShortcutKeys> { item };
				}
			}
		}
	}

	private void RevertBtnClick(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageWindow val = new CustomMessageWindow();
		val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_DEFAULTS", "");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_SHORTCUTS", "");
		val.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_RESTORE_BUTTON", ""), (EventHandler)delegate
		{
			RestoreDefaultShortcuts();
			((UIElement)mRevertBtn).IsEnabled = false;
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler)delegate
		{
		}, (string)null, false, (object)null);
		val.CloseButtonHandle((Predicate<object>)null, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow;
		((Window)val).ShowDialog();
	}

	private void RestoreDefaultShortcuts()
	{
		RegistryManager.Instance.UserDefinedShortcuts = string.Empty;
		ParentSettingsWindow.mIsShortcutEdited = false;
		CommonHandlers.ReloadShortcutsForAllInstances();
		if (ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
		{
			AddShortcutKeyElements();
		}
		((UIElement)mSaveBtn).IsEnabled = false;
		ParentWindow.mTopbarOptions?.SetLabel();
		Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_restore_default", (string)null, (string)null, (string)null, (string)null, (string)null, "Android", 0);
	}

	private void AddToastPopup(string message)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		try
		{
			if (mToastPopup == null)
			{
				mToastPopup = new CustomToastPopupControl((UserControl)(object)this);
			}
			mToastPopup.Init((Window)(object)ParentWindow, message, (Brush)null, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)2, (Thickness?)null, 12, (Thickness?)null, (Brush)null, false);
			mToastPopup.ShowPopup(1.3);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in showing toast popup: " + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeyscontrol.xaml", UriKind.Relative);
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mShortcutKeyScrollBar = (ScrollViewer)target;
			break;
		case 2:
			mRevertBtn = (CustomButton)target;
			((ButtonBase)mRevertBtn).Click += new RoutedEventHandler(RevertBtnClick);
			break;
		case 3:
			mSaveBtn = (CustomButton)target;
			((ButtonBase)mSaveBtn).Click += new RoutedEventHandler(SaveBtnClick);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
