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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class Gl3CustomWindow : CustomWindow, IComponentConnector
{
	private MainWindow mParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTextBlockGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCustomMessageBoxCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mTitleIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mBodyTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHintGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mHintTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHintGrid1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mHintTextBlock1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mButton;

	private bool _contentLoaded;

	public Gl3CustomWindow(MainWindow parentWindow)
	{
		mParentWindow = parentWindow;
		InitializeComponent();
	}

	private void mGetButton_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("Clicked Restart to opengl button");
		if (RegistryManager.Instance.GLES3 && mParentWindow.EngineInstanceRegistry.GlRenderMode != 1)
		{
			mParentWindow.EngineInstanceRegistry.GlRenderMode = 1;
			BlueStacksUIUtils.RestartInstance(mParentWindow.mVmName);
		}
		else
		{
			((Window)this).Close();
		}
	}

	private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked Gl3 custom window close button");
		((Window)this).Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/gl3customwindow.xaml", UriKind.Relative);
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
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			mParentGrid = (Grid)target;
			break;
		case 3:
			mTextBlockGrid = (Grid)target;
			break;
		case 4:
			mCustomMessageBoxCloseButton = (CustomPictureBox)target;
			((UIElement)mCustomMessageBoxCloseButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Close_PreviewMouseLeftButtonUp);
			break;
		case 5:
			mTitleText = (TextBlock)target;
			break;
		case 6:
			mTitleIcon = (CustomPictureBox)target;
			break;
		case 7:
			mBodyTextBlock = (TextBlock)target;
			break;
		case 8:
			mHintGrid = (Grid)target;
			break;
		case 9:
			mHintTextBlock = (TextBlock)target;
			break;
		case 10:
			mHintGrid1 = (Grid)target;
			break;
		case 11:
			mHintTextBlock1 = (TextBlock)target;
			break;
		case 12:
			mButton = (CustomButton)target;
			((ButtonBase)mButton).Click += new RoutedEventHandler(mGetButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
