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

public class DMMScreenshotSettingControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mChooseFolderTextBlock;

	private bool _contentLoaded;

	public DMMScreenshotSettingControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		((UIElement)this).Visibility = (Visibility)1;
		mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
	}

	private void ChooseScreenshotFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.DMMScreenshotHandler();
		mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/dmmscreenshotsettingcontrol.xaml", UriKind.Relative);
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (connectionId != 1)
		{
			if (connectionId == 1328313112 - 1328313110 % 2099097974)
			{
				((UIElement)(Grid)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ChooseScreenshotFolder_MouseLeftButtonUp);
			}
			else
			{
				_contentLoaded = true;
			}
		}
		else
		{
			mChooseFolderTextBlock = (TextBox)target;
		}
	}
}
