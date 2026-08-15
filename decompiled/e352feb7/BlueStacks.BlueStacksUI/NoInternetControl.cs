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

public class NoInternetControl : UserControl, IComponentConnector
{
	private BrowserControl AssociatedControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mFailureTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mErrorLine1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mErrorLine2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mBlueButton;

	private bool _contentLoaded;

	public NoInternetControl(BrowserControl browserControl)
	{
		InitializeComponent();
		AssociatedControl = browserControl;
		BlueStacksUIBinding.Bind(mFailureTextBox, "STRING_NAVIGATE_FAILED", "");
		BlueStacksUIBinding.Bind((Button)(object)mBlueButton, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
	}

	private void mBlueButton_Click(object sender, RoutedEventArgs e)
	{
		AssociatedControl.NavigateTo(AssociatedControl.mFailedUrl);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/nointernetcontrol.xaml", UriKind.Relative);
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
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mFailureTextBox = (TextBlock)target;
			break;
		case 2:
			mErrorLine1 = (TextBlock)target;
			break;
		case 3:
			mErrorLine2 = (TextBlock)target;
			break;
		case 4:
			mBlueButton = (CustomButton)target;
			((ButtonBase)mBlueButton).Click += new RoutedEventHandler(mBlueButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
