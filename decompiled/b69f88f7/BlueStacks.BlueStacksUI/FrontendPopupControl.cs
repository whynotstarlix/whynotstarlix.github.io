using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class FrontendPopupControl : UserControl, IComponentConnector
{
	private MainWindow mMainWindow;

	private EventHandler<EventArgs> RequestedAppDisplayed;

	private string mGooglePlayStoreArg;

	private bool mIsWindowForcedTillLoaded;

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

	public PlayStoreAction mAction { get; set; }

	public FrontendPopupControl()
	{
		InitializeComponent();
		RequestedAppDisplayed = (EventHandler<EventArgs>)Delegate.Combine(RequestedAppDisplayed, new EventHandler<EventArgs>(RequestedApp_Displayed));
	}

	private void ProcessArgs(string googlePlayStoreArg, bool isWindowForcedTillLoaded)
	{
		mGooglePlayStoreArg = googlePlayStoreArg;
		mIsWindowForcedTillLoaded = isWindowForcedTillLoaded;
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Invalid comparison between Unknown and I4
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Invalid comparison between Unknown and I4
			mBaseControl.Init((Control)(object)this, (Panel)(object)ParentWindow.mFrontendGrid, isWindowForced: false, isWindowForcedTillLoaded);
			mBaseControl.DimBackground();
			ParentWindow.mCommonHandler.SetCustomCursorForApp("com.android.vending");
			if ((int)mAction == 0)
			{
				if (ParentWindow.mAppHandler.IsAppInstalled(googlePlayStoreArg))
				{
					ParentWindow.mAppHandler.SendRunAppRequestAsync(googlePlayStoreArg);
				}
				else
				{
					AppHandler.EventOnAppDisplayed = RequestedAppDisplayed;
					ParentWindow.mAppHandler.LaunchPlayRequestAsync(googlePlayStoreArg);
				}
				ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
			}
			else if ((int)mAction == 1)
			{
				AppHandler.EventOnAppDisplayed = RequestedAppDisplayed;
				ParentWindow.mAppHandler.SendSearchPlayRequestAsync(googlePlayStoreArg);
				ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
			}
			else if ((int)mAction == 2)
			{
				AppHandler.EventOnAppDisplayed = RequestedAppDisplayed;
				ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
				Dictionary<string, string> data = new Dictionary<string, string> { { "action", googlePlayStoreArg } };
				ParentWindow.mAppHandler.StartCustomActivity(data);
			}
		}, new object[0]);
	}

	internal void Reload()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0 && !string.IsNullOrEmpty(mGooglePlayStoreArg))
		{
			ProcessArgs(mGooglePlayStoreArg, mIsWindowForcedTillLoaded);
		}
	}

	internal void RequestedApp_Displayed(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mBaseControl.ShowContent();
		}, new object[0]);
	}

	internal void Init(string args, string appName, PlayStoreAction action, bool isWindowForcedTillLoaded = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			if (!ParentWindow.mGuestBootCompleted)
			{
				CustomMessageWindow val = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE", "");
				BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_GUEST_NOT_BOOTED", "");
				val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
				((Window)val).Owner = (Window)(object)ParentWindow;
				((Window)val).ShowDialog();
			}
			else if ((int)action == 0 && ParentWindow.mAppHandler.IsAppInstalled(args) && !"com.android.vending".Equals(args, StringComparison.InvariantCultureIgnoreCase))
			{
				AppIconModel appIcon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(args);
				if (appIcon != null)
				{
					if ((int)appIcon.AppIncompatType != 0)
					{
						GrmHandler.HandleCompatibility(appIcon.PackageName, ParentWindow.mVmName);
					}
					else
					{
						ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: true, isLaunch: true);
					}
				}
			}
			else if (!string.IsNullOrEmpty(args))
			{
				if (!((UIElement)ParentWindow.WelcomeTabParentGrid).IsVisible)
				{
					ParentWindow.mCommonHandler.HomeButtonHandler(isLaunch: false);
				}
				((ContentControl)mBaseControl.mTitleLabel).Content = appName;
				mAction = action;
				((UIElement)this).Visibility = (Visibility)0;
				ParentWindow.ChangeOrientationFromClient(isPortrait: false, stopFurtherOrientation: false);
				ProcessArgs(args, isWindowForcedTillLoaded);
			}
		}, new object[0]);
	}

	internal void HideWindow()
	{
		mBaseControl.HideWindow();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/frontendpopupcontrol.xaml", UriKind.Relative);
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
		if (connectionId == 1)
		{
			mBaseControl = (DimControlWithProgresBar)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
