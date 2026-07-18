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

public class ImportMacroScriptsControl : UserControl, IComponentConnector
{
	internal ImportMacroWindow mImportMacroWindow;

	internal MainWindow ParentWindow;

	private static int mIdCount;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mContent;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSingleMacroRecordTextblock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroImportedAsTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mConflictingMacroOptionsPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mReplaceExistingBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mRenameBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mImportName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mWarningMsg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mDependentScriptsMsg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mDependentScriptsPanel;

	private bool _contentLoaded;

	public ImportMacroScriptsControl(ImportMacroWindow importMacroWindow, MainWindow window)
	{
		InitializeComponent();
		mImportMacroWindow = importMacroWindow;
		ParentWindow = window;
		mIdCount++;
	}

	private void Box_Checked(object sender, RoutedEventArgs e)
	{
		mImportMacroWindow.Box_Checked(sender, e);
	}

	private void Box_Unchecked(object sender, RoutedEventArgs e)
	{
		mImportMacroWindow.Box_Unchecked(sender, e);
	}

	private void ImportName_TextChanged(object sender, TextChangedEventArgs e)
	{
		mImportMacroWindow.TextChanged(sender, e);
	}

	internal void Init(string macroName, bool isSingleRecording)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)mRenameBtn).ApplyTemplate();
		((FrameworkElement)mReplaceExistingBtn).ApplyTemplate();
		((FrameworkElement)mRenameBtn.RadioBtnImage).Width = 14.0;
		((FrameworkElement)mRenameBtn.RadioBtnImage).Height = 14.0;
		((RadioButton)mRenameBtn).GroupName = $"MacroConflictAction_{macroName}{mIdCount}";
		((FrameworkElement)mReplaceExistingBtn.RadioBtnImage).Width = 14.0;
		((FrameworkElement)mReplaceExistingBtn.RadioBtnImage).Height = 14.0;
		((RadioButton)mReplaceExistingBtn).GroupName = $"MacroConflictAction_{macroName}{mIdCount}";
		((ToggleButton)mReplaceExistingBtn).IsChecked = true;
		((ToggleButton)mReplaceExistingBtn).Checked += new RoutedEventHandler(ConflictingMacroHandlingRadioBtn_Checked);
		((ContentControl)mContent).Content = macroName;
		if (isSingleRecording)
		{
			((UIElement)mContent).Visibility = (Visibility)2;
			((FrameworkElement)mBlock).Margin = new Thickness(0.0);
			((FrameworkElement)mMainGrid).Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
			((UIElement)mSingleMacroRecordTextblock).Visibility = (Visibility)0;
			mSingleMacroRecordTextblock.Text = macroName;
			((FrameworkElement)mWarningMsg).Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
		}
		else
		{
			((FrameworkElement)mMainGrid).Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
		}
	}

	private void ConflictingMacroHandlingRadioBtn_Checked(object sender, RoutedEventArgs e)
	{
		CustomRadioButton val = (CustomRadioButton)((sender is CustomRadioButton) ? sender : null);
		if (val != null && mImportName != null)
		{
			if (val == mRenameBtn)
			{
				((UIElement)mImportName).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mImportName).Visibility = (Visibility)2;
			}
		}
	}

	internal bool IsScriptInRenameMode()
	{
		if (((ToggleButton)mRenameBtn).IsChecked.HasValue && ((ToggleButton)mRenameBtn).IsChecked.Value)
		{
			return true;
		}
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/importmacroscriptscontrol.xaml", UriKind.Relative);
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
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMainGrid = (Grid)target;
			break;
		case 2:
			mContent = (CustomCheckbox)target;
			((ToggleButton)mContent).Checked += new RoutedEventHandler(Box_Checked);
			((ToggleButton)mContent).Unchecked += new RoutedEventHandler(Box_Unchecked);
			break;
		case 3:
			mSingleMacroRecordTextblock = (TextBlock)target;
			break;
		case 4:
			mBlock = (Grid)target;
			break;
		case 5:
			mMacroImportedAsTextBlock = (TextBlock)target;
			break;
		case 6:
			mConflictingMacroOptionsPanel = (StackPanel)target;
			break;
		case 7:
			mReplaceExistingBtn = (CustomRadioButton)target;
			break;
		case 8:
			mRenameBtn = (CustomRadioButton)target;
			((ToggleButton)mRenameBtn).Checked += new RoutedEventHandler(ConflictingMacroHandlingRadioBtn_Checked);
			break;
		case 9:
			mImportName = (CustomTextBox)target;
			((TextBoxBase)mImportName).TextChanged += new TextChangedEventHandler(ImportName_TextChanged);
			break;
		case 10:
			mWarningMsg = (TextBlock)target;
			break;
		case 11:
			mDependentScriptsMsg = (TextBlock)target;
			break;
		case 12:
			mDependentScriptsPanel = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
