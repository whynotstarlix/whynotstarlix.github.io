using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MacroToAdd : UserControl, IComponentConnector
{
	private MergeMacroWindow mMergeMacroWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMacroName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAddMacro;

	private bool _contentLoaded;

	public MacroToAdd(MergeMacroWindow window, string macroName)
	{
		InitializeComponent();
		mMergeMacroWindow = window;
		((FrameworkElement)this).Tag = macroName;
		mMacroName.Text = macroName;
	}

	private void AddMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		MergedMacroConfiguration val = new MergedMacroConfiguration();
		val.MacrosToRun.Add(mMacroName.Text);
		val.Tag = mMergeMacroWindow.mAddedMacroTag++;
		if (mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations == null)
		{
			mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>();
		}
		mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Add(val);
	}

	private void MacroName_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		UsefulExtensionMethod.SetTextblockTooltip((TextBlock)((sender is TextBlock) ? sender : null));
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotoadd.xaml", UriKind.Relative);
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		if (connectionId != 1)
		{
			if (connectionId == (-1234242944 ^ -1234242942))
			{
				mAddMacro = (CustomPictureBox)target;
				((UIElement)mAddMacro).MouseLeftButtonUp += new MouseButtonEventHandler(AddMacro_MouseLeftButtonUp);
			}
			else
			{
				_contentLoaded = true;
			}
		}
		else
		{
			mMacroName = (TextBlock)target;
			((FrameworkElement)mMacroName).SizeChanged += new SizeChangedEventHandler(MacroName_SizeChanged);
		}
	}
}
