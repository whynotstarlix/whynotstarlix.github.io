using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class DMMScreenshotSettingControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

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
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mChooseFolderTextBlock = (TextBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		case 2:
			((UIElement)(Grid)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ChooseScreenshotFolder_MouseLeftButtonUp);
			break;
		}
	}
}
