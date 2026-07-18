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

public class MacroOverlay : UserControl, IDimOverlayControl, IComponentConnector
{
	private bool mIsCloseOnOverLayClick;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; set; }

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return mIsCloseOnOverLayClick;
		}
		set
		{
			mIsCloseOnOverLayClick = value;
		}
	}

	public bool ShowControlInSeparateWindow { get; set; }

	public bool ShowTransparentWindow { get; set; }

	public MacroOverlay()
	{
		InitializeComponent();
	}

	public MacroOverlay(MainWindow mainWindow)
	{
		ParentWindow = mainWindow;
	}

	private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		HideOverlay();
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("abortReroll");
	}

	private void HideOverlay()
	{
		ParentWindow.HideDimOverlay();
	}

	internal void ShowPromptAndHideOverlay()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0)
		{
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_REROLL_COMPLETED", "");
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_REROLL_COMPLETED_SUCCESS", "");
			val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
			HideOverlay();
		}
	}

	bool IDimOverlayControl.Close()
	{
		((UIElement)this).Visibility = (Visibility)1;
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrooverlay.xaml", UriKind.Relative);
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (connectionId == 1)
		{
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_PreviewMouseLeftButtonUp);
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
