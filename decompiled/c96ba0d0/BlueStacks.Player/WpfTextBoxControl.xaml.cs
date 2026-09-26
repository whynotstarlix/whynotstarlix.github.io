using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Player;

public partial class WpfTextBoxControl : UserControl, IComponentConnector
{
	internal TextBox mWpfTextBox;

	public WpfTextBoxControl()
	{
		InitializeComponent();
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		((FrameworkElement)this).OnGotFocus(e);
	}

	private void WpfTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		if (connectionId == 1)
		{
			mWpfTextBox = (TextBox)target;
			((UIElement)mWpfTextBox).AddHandler(CommandManager.PreviewExecutedEvent, (Delegate)new ExecutedRoutedEventHandler(WpfTextBox_PreviewExecuted));
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
