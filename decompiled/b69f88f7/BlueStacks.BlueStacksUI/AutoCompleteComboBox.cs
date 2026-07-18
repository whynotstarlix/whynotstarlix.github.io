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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AutoCompleteComboBox : UserControl, IComponentConnector
{
	private List<string> mListData = new List<string>();

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mAutoComboBox;

	private bool _contentLoaded;

	public AutoCompleteComboBox()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		InitializeComponent();
		((ComboBox)mAutoComboBox).IsDropDownOpen = false;
		((FrameworkElement)mAutoComboBox).Loaded += (RoutedEventHandler)delegate
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			object obj = ((FrameworkTemplate)((Control)mAutoComboBox).Template).FindName("PART_EditableTextBox", (FrameworkElement)(object)mAutoComboBox);
			TextBox val = (TextBox)((obj is TextBox) ? obj : null);
			if (val != null)
			{
				((TextBoxBase)val).TextChanged += new TextChangedEventHandler(EditTextBox_TextChanged);
			}
		};
		((ComboBox)mAutoComboBox).DropDownOpened += MAutoComboBox_DropDownOpened;
		EventManager.RegisterClassHandler(typeof(TextBox), UIElement.KeyUpEvent, (Delegate)new RoutedEventHandler(DeselectText));
	}

	private void DeselectText(object sender, RoutedEventArgs e)
	{
		object originalSource = e.OriginalSource;
		TextBox val = (TextBox)((originalSource is TextBox) ? originalSource : null);
		if (val != null && val.Text.Length < 2)
		{
			val.SelectionLength = 0;
			val.SelectionStart = 1;
		}
	}

	private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (Keyboard.IsKeyDown((Key)26))
		{
			((RoutedEventArgs)e).Handled = true;
			return;
		}
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		mAutoComboBox_TextChanged(val.Text);
	}

	private void MAutoComboBox_DropDownOpened(object sender, EventArgs e)
	{
		((Selector)mAutoComboBox).SelectedItem = null;
	}

	public void AddItems(string key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		ComboBoxItem val = new ComboBoxItem
		{
			Content = key
		};
		((ItemsControl)mAutoComboBox).Items.Add((object)val);
	}

	public void AddSuggestions(List<string> listOfSuggestions)
	{
		mListData.Clear();
		mListData = listOfSuggestions;
	}

	private void mAutoComboBox_TextChanged(string msg)
	{
		bool flag = false;
		if (string.IsNullOrEmpty(msg))
		{
			((ComboBox)mAutoComboBox).IsDropDownOpen = false;
		}
		((ItemsControl)mAutoComboBox).Items.Clear();
		foreach (string mListDatum in mListData)
		{
			if (mListDatum.StartsWith(msg, StringComparison.InvariantCultureIgnoreCase))
			{
				AddItems(mListDatum);
				flag = true;
			}
		}
		if (flag)
		{
			((ComboBox)mAutoComboBox).IsDropDownOpen = true;
		}
		else
		{
			((ComboBox)mAutoComboBox).IsDropDownOpen = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/autocompletecombobox.xaml", UriKind.Relative);
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		if (connectionId == 1)
		{
			mAutoComboBox = (CustomComboBox)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
