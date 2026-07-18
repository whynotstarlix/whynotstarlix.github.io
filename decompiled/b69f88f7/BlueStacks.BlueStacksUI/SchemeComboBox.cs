using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SchemeComboBox : UserControl, IComponentConnector
{
	public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(ComboBoxSchemeControl), (PropertyMetadata)new UIPropertyMetadata((object)string.Empty));

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SchemeComboBox _this;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ToggleButton TogglePopupButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path Arrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mItems;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mSchemesListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel Items;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid NewProfile;

	private bool _contentLoaded;

	public string mSelectedItem { get; set; }

	public string SelectedItem
	{
		get
		{
			return mSelectedItem;
		}
		set
		{
			mSelectedItem = value;
		}
	}

	public SchemeComboBox()
	{
		InitializeComponent();
	}

	private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
	{
		if (!Keyboard.IsKeyDown((Key)26) && !Keyboard.IsKeyDown((Key)24))
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private void ComboBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
	{
		((UIElement)((ComboBoxSchemeControl)sender).mBookmarkImg).Visibility = (Visibility)2;
	}

	private void NewProfile_MouseDown(object sender, MouseButtonEventArgs e)
	{
		KMManager.AddNewControlSchemeAndSelect(BlueStacksUIUtils.LastActivatedWindow, null, isCopyOrNew: true);
		KMManager.CanvasWindow.ClearWindow();
	}

	private void NewProfile_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)NewProfile, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void NewProfile_MouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)NewProfile, Panel.BackgroundProperty, "ComboBoxBackgroundColor");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/schemecombobox.xaml", UriKind.Relative);
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			_this = (SchemeComboBox)target;
			break;
		case 2:
			mGrid = (Grid)target;
			break;
		case 3:
			TogglePopupButton = (ToggleButton)target;
			break;
		case 4:
			mName = (TextBlock)target;
			break;
		case 5:
			Arrow = (Path)target;
			break;
		case 6:
			mItems = (CustomPopUp)target;
			break;
		case 7:
			mSchemesListScrollbar = (ScrollViewer)target;
			break;
		case 8:
			Items = (StackPanel)target;
			break;
		case 9:
			NewProfile = (Grid)target;
			((UIElement)NewProfile).MouseDown += new MouseButtonEventHandler(NewProfile_MouseDown);
			((UIElement)NewProfile).MouseEnter += new MouseEventHandler(NewProfile_MouseEnter);
			((UIElement)NewProfile).MouseLeave += new MouseEventHandler(NewProfile_MouseLeave);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
