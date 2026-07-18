using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class FrontendOTSControl : UserControl, IComponentConnector
{
	private MainWindow mMainWindow;

	private EventHandler<EventArgs> OneTimeSetupCompletedEventHandle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DimControlWithProgresBar mBaseControl;

	private bool _contentLoaded;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	public FrontendOTSControl()
	{
		InitializeComponent();
		OneTimeSetupCompletedEventHandle = (EventHandler<EventArgs>)Delegate.Combine(OneTimeSetupCompletedEventHandle, new EventHandler<EventArgs>(OneTimeSetup_Completed));
	}

	private void UserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0)
		{
			BlueStacksUIBinding.Bind(mBaseControl.mTitleLabel, "STRING_GOOGLE_LOGIN_MESSAGE");
			mBaseControl.Init((Control)(object)this, (Panel)(object)ParentWindow.mFrontendGrid, isWindowForced: true, _: true);
			mBaseControl.ShowContent();
			ParentWindow.mAppHandler.EventOnOneTimeSetupCompleted = OneTimeSetupCompletedEventHandle;
		}
	}

	private void OneTimeSetup_Completed(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mBaseControl.HideWindow();
		}, new object[0]);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/frontendotscontrol.xaml", UriKind.Relative);
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(FrontendOTSControl)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(UserControl_IsVisibleChanged);
			break;
		case 2:
			mBaseControl = (DimControlWithProgresBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
