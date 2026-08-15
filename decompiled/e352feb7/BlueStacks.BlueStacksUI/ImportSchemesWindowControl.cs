using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ImportSchemesWindowControl : UserControl, IComponentConnector
{
	internal ImportSchemesWindow mImportSchemesWindow;

	internal MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mContent;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mImportName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mWarningMsg;

	private bool _contentLoaded;

	public ImportSchemesWindowControl(ImportSchemesWindow importSchemesWindow, MainWindow window)
	{
		InitializeComponent();
		mImportSchemesWindow = importSchemesWindow;
		ParentWindow = window;
	}

	private void box_Checked(object sender, RoutedEventArgs e)
	{
		mImportSchemesWindow.Box_Checked(sender, e);
	}

	private void box_Unchecked(object sender, RoutedEventArgs e)
	{
		mImportSchemesWindow.Box_Unchecked(sender, e);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/importschemeswindowcontrol.xaml", UriKind.Relative);
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mContent = (CustomCheckbox)target;
			((ToggleButton)mContent).Checked += new RoutedEventHandler(box_Checked);
			((ToggleButton)mContent).Unchecked += new RoutedEventHandler(box_Unchecked);
			break;
		case 2:
			mBlock = (Grid)target;
			break;
		case 3:
			mImportName = (CustomTextBox)target;
			break;
		case 4:
			mWarningMsg = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
