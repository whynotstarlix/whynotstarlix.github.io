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

namespace BlueStacks.BlueStacksUI;

public class ComboBoxSchemeControl : UserControl, IComponentConnector
{
	private KeymapCanvasWindow CanvasWindow;

	private MainWindow ParentWindow;

	private CustomMessageWindow mDeleteScriptMessageWindow;

	internal string mOldSchemeName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSchemeControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBookmarkImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mSchemeName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mEditImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSaveImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCopyImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mDeleteImg;

	private bool _contentLoaded;

	public ComboBoxSchemeControl(KeymapCanvasWindow window, MainWindow mainWindow)
	{
		CanvasWindow = window;
		ParentWindow = mainWindow;
		InitializeComponent();
	}

	private void Bookmark_img_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!((TextBoxBase)mSchemeName).IsReadOnly)
		{
			HandleNameEdit(this);
		}
		if (ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(((TextBox)mSchemeName).Text))
		{
			IMControlScheme iMControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text];
			if (iMControlScheme.IsBookMarked)
			{
				iMControlScheme.IsBookMarked = false;
				mBookmarkImg.ImageName = "bookmark";
			}
			else
			{
				List<IMControlScheme> controlSchemes = ParentWindow.SelectedConfig.ControlSchemes;
				if (controlSchemes != null && controlSchemes.Count((IMControlScheme scheme) => scheme.IsBookMarked) < 5)
				{
					iMControlScheme.IsBookMarked = true;
					mBookmarkImg.ImageName = "bookmarked";
				}
				else
				{
					CanvasWindow.SidebarWindow.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_BOOKMARK_SCHEMES_WARNING", ""));
				}
			}
			CanvasWindow.SidebarWindow.FillProfileCombo();
			KeymapCanvasWindow.sIsDirty = true;
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void EditImg_MouseDown(object sender, MouseButtonEventArgs e)
	{
		((UIElement)mEditImg).Visibility = (Visibility)2;
		((UIElement)mSaveImg).Visibility = (Visibility)0;
		mOldSchemeName = ((TextBox)mSchemeName).Text;
		((UIElement)mSchemeName).Focusable = true;
		((TextBoxBase)mSchemeName).IsReadOnly = false;
		((TextBox)mSchemeName).CaretIndex = ((TextBox)mSchemeName).Text.Length;
		((UIElement)mSchemeName).Focus();
		((RoutedEventArgs)e).Handled = true;
	}

	private void SaveImg_MouseDown(object sender, MouseButtonEventArgs e)
	{
		HandleNameEdit(this);
		((RoutedEventArgs)e).Handled = true;
	}

	private bool EditedNameIsAllowed(string text, ComboBoxSchemeControl toBeRenamedControl)
	{
		if (string.IsNullOrEmpty(text.Trim()))
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""));
			return false;
		}
		foreach (ComboBoxSchemeControl child in ((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children)
		{
			if (((TextBox)child.mSchemeName).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && child != toBeRenamedControl)
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""));
				return false;
			}
			if (((TextBox)child.mSchemeName).Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
				{
					LocaleStrings.GetLocalizedString("STRING_SCHEME_INVALID_CHARACTERS", ""),
					Environment.NewLine,
					"\\ / : * ? \" < > |"
				});
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)CanvasWindow.SidebarWindow, message, 3.0);
				return false;
			}
		}
		return true;
	}

	private void CopyImg_MouseDown(object sender, MouseButtonEventArgs e)
	{
		bool flag = false;
		foreach (ComboBoxSchemeControl child in ((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children)
		{
			if (!((TextBoxBase)child.mSchemeName).IsReadOnly)
			{
				HandleNameEdit(child);
				flag = true;
				((RoutedEventArgs)e).Handled = true;
				break;
			}
		}
		if (!flag)
		{
			KMManager.AddNewControlSchemeAndSelect(ParentWindow, ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text], isCopyOrNew: true);
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void DeleteImg_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		if (!((TextBoxBase)mSchemeName).IsReadOnly)
		{
			HandleNameEdit(this);
		}
		if (!ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup)
		{
			DeleteControlScheme();
			((RoutedEventArgs)e).Handled = true;
			return;
		}
		mDeleteScriptMessageWindow = new CustomMessageWindow();
		mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME", "");
		mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME_CONFIRMATION", "");
		((ContentControl)mDeleteScriptMessageWindow.CheckBox).Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
		((UIElement)mDeleteScriptMessageWindow.CheckBox).Visibility = (Visibility)0;
		((ToggleButton)mDeleteScriptMessageWindow.CheckBox).IsChecked = false;
		mDeleteScriptMessageWindow.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), (EventHandler)UpdateSettingsAndDeleteScheme, (string)null, false, (object)null);
		mDeleteScriptMessageWindow.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler)delegate
		{
			KeymapCanvasWindow.sIsDirty = false;
			GuidanceWindow.sIsDirty = false;
		}, (string)null, false, (object)null);
		mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler)delegate
		{
		}, (object)null);
		((Window)mDeleteScriptMessageWindow).Owner = (Window)(object)CanvasWindow;
		((Window)mDeleteScriptMessageWindow).ShowDialog();
		((RoutedEventArgs)e).Handled = true;
	}

	private void UpdateSettingsAndDeleteScheme(object sender, EventArgs e)
	{
		ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup = !((ToggleButton)mDeleteScriptMessageWindow.CheckBox).IsChecked.Value;
		mDeleteScriptMessageWindow = null;
		DeleteControlScheme();
	}

	private void DeleteControlScheme()
	{
		if (!ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(((TextBox)mSchemeName).Text) || ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text].BuiltIn)
		{
			return;
		}
		if (ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text].Selected)
		{
			ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text].Selected = false;
			if (ParentWindow.SelectedConfig.ControlSchemes.Count > 1)
			{
				if (CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == ((TextBox)(((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children[0] as ComboBoxSchemeControl).mSchemeName).Text.ToString(CultureInfo.InvariantCulture))
				{
					CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = ((TextBox)(((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children[1] as ComboBoxSchemeControl).mSchemeName).Text.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = ((TextBox)(((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children[0] as ComboBoxSchemeControl).mSchemeName).Text.ToString(CultureInfo.InvariantCulture);
				}
				ParentWindow.SelectedConfig.ControlSchemesDict[CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = true;
				CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem;
				ParentWindow.SelectedConfig.SelectedControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem];
				CanvasWindow.SidebarWindow.ProfileChanged();
			}
			else
			{
				CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = null;
				BlueStacksUIBinding.Bind(CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
			}
		}
		ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text]);
		ParentWindow.SelectedConfig.ControlSchemesDict.Remove(((TextBox)mSchemeName).Text);
		ComboBoxSchemeControl comboBoxSchemeControlFromName = KMManager.GetComboBoxSchemeControlFromName(((TextBox)mSchemeName).Text);
		if (comboBoxSchemeControlFromName != null)
		{
			((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children.Remove((UIElement)(object)comboBoxSchemeControlFromName);
		}
		KeymapCanvasWindow.sIsDirty = true;
		CanvasWindow.SidebarWindow.FillProfileCombo();
		if (ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
		{
			CanvasWindow.ClearWindow();
		}
	}

	private void ComboBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
	{
		bool flag = false;
		foreach (ComboBoxSchemeControl child in ((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children)
		{
			if (!((TextBoxBase)child.mSchemeName).IsReadOnly)
			{
				HandleNameEdit(child);
				flag = true;
				((RoutedEventArgs)e).Handled = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		if (CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == ((TextBox)mSchemeName).Text)
		{
			((Popup)CanvasWindow.SidebarWindow.mSchemeComboBox.mItems).IsOpen = false;
			return;
		}
		if (CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem != null)
		{
			ParentWindow.SelectedConfig.ControlSchemesDict[CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = false;
		}
		ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text].Selected = true;
		ParentWindow.SelectedConfig.ControlSchemesDict[ParentWindow.SelectedConfig.SelectedControlScheme.Name].Selected = false;
		ParentWindow.SelectedConfig.SelectedControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)mSchemeName).Text];
		CanvasWindow.SidebarWindow.FillProfileCombo();
		CanvasWindow.SidebarWindow.ProfileChanged();
		((Popup)CanvasWindow.SidebarWindow.mSchemeComboBox.mItems).IsOpen = false;
		KeymapCanvasWindow.sIsDirty = true;
		KMManager.SendSchemeChangedStats(ParentWindow, "controls_editor");
	}

	private void ComboBoxItem_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void ComboBoxItem_MouseLeave(object sender, MouseEventArgs e)
	{
		if (((TextBox)mSchemeName).Text != CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ComboBoxBackgroundColor");
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
		}
	}

	private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
	{
		if (((UIElement)mSchemeName).Focusable)
		{
			HandleNameEdit(this);
		}
		e.Handled = true;
	}

	private void HandleNameEdit(ComboBoxSchemeControl control)
	{
		((UIElement)control.mEditImg).Visibility = (Visibility)0;
		((UIElement)control.mSaveImg).Visibility = (Visibility)2;
		if (EditedNameIsAllowed(((TextBox)control.mSchemeName).Text, control))
		{
			if (ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(control.mOldSchemeName))
			{
				IMControlScheme iMControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[control.mOldSchemeName];
				iMControlScheme.Name = ((TextBox)control.mSchemeName).Text.Trim();
				ParentWindow.SelectedConfig.ControlSchemesDict.Remove(control.mOldSchemeName);
				ParentWindow.SelectedConfig.ControlSchemesDict.Add(iMControlScheme.Name, iMControlScheme);
				CanvasWindow.SidebarWindow.FillProfileCombo();
				KeymapCanvasWindow.sIsDirty = true;
			}
		}
		else
		{
			((TextBox)control.mSchemeName).Text = control.mOldSchemeName;
		}
		((UIElement)control.mSchemeName).Focusable = false;
		((TextBoxBase)control.mSchemeName).IsReadOnly = true;
	}

	private void MSchemeName_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.Key == 6)
		{
			HandleNameEdit(this);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/comboboxschemecontrol.xaml", UriKind.Relative);
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
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(ComboBoxSchemeControl)target).MouseDown += new MouseButtonEventHandler(ComboBoxItem_MouseDown);
			((UIElement)(ComboBoxSchemeControl)target).MouseEnter += new MouseEventHandler(ComboBoxItem_MouseEnter);
			((UIElement)(ComboBoxSchemeControl)target).MouseLeave += new MouseEventHandler(ComboBoxItem_MouseLeave);
			((UIElement)(ComboBoxSchemeControl)target).LostFocus += new RoutedEventHandler(ComboBoxItem_LostFocus);
			break;
		case 2:
			mSchemeControl = (Grid)target;
			break;
		case 3:
			mBookmarkImg = (CustomPictureBox)target;
			((UIElement)mBookmarkImg).MouseDown += new MouseButtonEventHandler(Bookmark_img_MouseDown);
			break;
		case 4:
			mSchemeName = (CustomTextBox)target;
			((UIElement)mSchemeName).KeyUp += new KeyEventHandler(MSchemeName_KeyUp);
			break;
		case 5:
			mEditImg = (CustomPictureBox)target;
			((UIElement)mEditImg).MouseDown += new MouseButtonEventHandler(EditImg_MouseDown);
			break;
		case 6:
			mSaveImg = (CustomPictureBox)target;
			((UIElement)mSaveImg).MouseDown += new MouseButtonEventHandler(SaveImg_MouseDown);
			break;
		case 7:
			mCopyImg = (CustomPictureBox)target;
			((UIElement)mCopyImg).MouseDown += new MouseButtonEventHandler(CopyImg_MouseDown);
			break;
		case 8:
			mDeleteImg = (CustomPictureBox)target;
			((UIElement)mDeleteImg).MouseDown += new MouseButtonEventHandler(DeleteImg_MouseDown);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
