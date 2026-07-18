using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SchemeBookmarkControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mSchemeName;

	private bool _contentLoaded;

	public SchemeBookmarkControl(IMControlScheme scheme, MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		((TextBox)mSchemeName).Text = scheme?.Name;
		if (scheme.Selected)
		{
			mCheckbox.ImageName = "radio_selected";
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
		}
	}

	private void UserControl_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void UserControl_MouseLeave(object sender, MouseEventArgs e)
	{
		if (ParentWindow.SelectedConfig.SelectedControlScheme != null && ((TextBox)mSchemeName).Text != ParentWindow.SelectedConfig.SelectedControlScheme.Name)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ComboBoxBackgroundColor");
		}
		else
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
		}
	}

	private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!(mCheckbox.ImageName == "radio_unselected"))
		{
			return;
		}
		foreach (SchemeBookmarkControl child in ((Panel)ParentWindow.mSidebar.mBookmarkedSchemesStackPanel).Children)
		{
			child.mCheckbox.ImageName = "radio_unselected";
			BlueStacksUIBinding.BindColor((DependencyObject)(object)child, Control.BackgroundProperty, "ComboBoxBackgroundColor");
		}
		mCheckbox.ImageName = "radio_selected";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "SelectedTabBackgroundColor");
		KMManager.SelectSchemeIfPresent(ParentWindow, ((TextBox)mSchemeName).Text, "bookmark", forceSave: true);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/schemebookmarkcontrol.xaml", UriKind.Relative);
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(SchemeBookmarkControl)target).MouseEnter += new MouseEventHandler(UserControl_MouseEnter);
			((UIElement)(SchemeBookmarkControl)target).MouseLeave += new MouseEventHandler(UserControl_MouseLeave);
			((UIElement)(SchemeBookmarkControl)target).MouseDown += new MouseButtonEventHandler(UserControl_PreviewMouseDown);
			break;
		case 2:
			mCheckbox = (CustomPictureBox)target;
			break;
		case 3:
			mSchemeName = (CustomTextBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
