using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class GuidanceVideoWindow : CustomWindow, IDisposable, IComponentConnector
{
	private BrowserControl mBrowser;

	internal MainWindow ParentWindow;

	private bool disposedValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GuidanceVideoWindow mWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainBrowserGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBrowserGrid;

	private bool _contentLoaded;

	public GuidanceVideoWindow(MainWindow parentWindow)
	{
		ParentWindow = parentWindow;
		InitializeComponent();
	}

	private void GuidanceVideoWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs eventArgs)
	{
		if (((UIElement)this).IsVisible)
		{
			ClientStats.SendKeyMappingUIStatsAsync("video_clicked", KMManager.sPackageName, ((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString());
			mBrowser = new BrowserControl();
			mBrowser.InitBaseControl(BlueStacksUIUtils.GetVideoTutorialUrl(KMManager.sPackageName, ((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString().ToLower(CultureInfo.InvariantCulture), ParentWindow?.SelectedConfig?.SelectedControlScheme?.Name));
			mBrowser.ParentWindow = ParentWindow;
			((UIElement)mBrowser).Visibility = (Visibility)0;
			((Panel)mBrowserGrid).Children.Add((UIElement)(object)mBrowser);
		}
		try
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref eventArgs)).NewValue)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					["allInstances"] = "False",
					["explicit"] = "False"
				};
				HTTPUtils.SendRequestToEngineAsync("mute", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
				ParentWindow.mCommonHandler.OnVolumeMuted(muted: true);
			}
			else if (!RegistryManager.Instance.AreAllInstancesMuted && !ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string> { ["allInstances"] = "False" };
				HTTPUtils.SendRequestToEngineAsync("unmute", dictionary2, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
				ParentWindow.mCommonHandler.OnVolumeMuted(muted: false);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
		}
	}

	internal void CloseWindow()
	{
		if (mBrowser != null)
		{
			mBrowser.DisposeBrowser();
			((Panel)mBrowserGrid).Children.Remove((UIElement)(object)mBrowser);
			mBrowser = null;
		}
	}

	private void CloseButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		((Window)this).Close();
	}

	private void mWindow_Closing(object sender, CancelEventArgs e)
	{
		CloseWindow();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			mBrowser?.Dispose();
			disposedValue = true;
		}
	}

	~GuidanceVideoWindow()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/guidancevideowindow.xaml", UriKind.Relative);
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
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mWindow = (GuidanceVideoWindow)target;
			((UIElement)mWindow).IsVisibleChanged += new DependencyPropertyChangedEventHandler(GuidanceVideoWindow_IsVisibleChanged);
			((Window)mWindow).Closing += mWindow_Closing;
			break;
		case 2:
			mMainBrowserGrid = (Grid)target;
			break;
		case 3:
			mMaskBorder = (Border)target;
			break;
		case 4:
			mBrowserGrid = (Grid)target;
			break;
		case 5:
			((UIElement)(CustomPictureBox)target).PreviewMouseUp += new MouseButtonEventHandler(CloseButton_PreviewMouseUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
