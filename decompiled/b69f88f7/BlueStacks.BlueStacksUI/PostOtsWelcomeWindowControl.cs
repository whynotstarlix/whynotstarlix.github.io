using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PostOtsWelcomeWindowControl : UserControl, IDisposable, IComponentConnector
{
	private bool? mSuccess;

	private Timer loginSyncTimer;

	private MainWindow ParentWindow;

	private bool disposedValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPostOtsImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mLoadingImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mPostOtsLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mPostOtsWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mPostOtsButton;

	private bool _contentLoaded;

	public PostOtsWelcomeWindowControl(MainWindow ParentWindow)
	{
		InitializeComponent();
		this.ParentWindow = ParentWindow;
	}

	private void PostOtsWelcome_Loaded(object sender, RoutedEventArgs e)
	{
		Logger.Info("PostOtsWelcome window loaded");
		loginSyncTimer = new Timer(10000.0);
		loginSyncTimer.Elapsed += OnLoginSyncTimeout;
		loginSyncTimer.AutoReset = false;
		if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
		{
			ChangeBasedonTokenReceived("true");
		}
		else
		{
			StartingTimer();
		}
	}

	public void ChangeBasedonTokenReceived(string status)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			try
			{
				Logger.Info("In ChangeBasedonTokenReceived");
				((UIElement)mLoadingImage).Visibility = (Visibility)2;
				if (status.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					mPostOtsImage.ImageName = "success_ots_icon";
					((UIElement)mPostOtsWarning).Visibility = (Visibility)2;
					((UIElement)mCloseButton).Visibility = (Visibility)2;
					BlueStacksUIBinding.Bind(mPostOtsLabel, "STRING_POST_OTS_SUCCESS_MESSAGE");
					BlueStacksUIBinding.Bind((Button)(object)mPostOtsButton, "STRING_POST_OTS_SUCCESS_BUTTON_MESSAGE");
					mSuccess = true;
				}
				else
				{
					mPostOtsImage.ImageName = "failure_ots_icon";
					((UIElement)mPostOtsWarning).Visibility = (Visibility)0;
					((UIElement)mCloseButton).Visibility = (Visibility)0;
					BlueStacksUIBinding.Bind(mPostOtsLabel, "STRING_POST_OTS_FAILED_MESSAGE");
					BlueStacksUIBinding.Bind((Button)(object)mPostOtsButton, "STRING_POST_OTS_FAILED_BUTTON_MESSAGE");
					mSuccess = false;
				}
				if (loginSyncTimer != null)
				{
					loginSyncTimer.Stop();
				}
				((UIElement)mPostOtsButton).IsEnabled = true;
			}
			catch (Exception ex)
			{
				Logger.Error(" Exception in ChangeBasedOnTokenReceived Status: " + status + Environment.NewLine + "Error: " + ex.ToString());
			}
		}, new object[0]);
	}

	private void StartingTimer()
	{
		Logger.Info("Starting Timer");
		loginSyncTimer.Stop();
		loginSyncTimer.Start();
		loginSyncTimer.Enabled = true;
	}

	private void OnLoginSyncTimeout(object source, ElapsedEventArgs e)
	{
		try
		{
			Logger.Error("Login Sync timed out.");
			if (!mSuccess.HasValue)
			{
				ChangeBasedonTokenReceived("false");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in login sync timer timeout " + ex.ToString());
		}
	}

	private void mPostOtsButton_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("mPostOtsButton clicked");
		if (!mSuccess.HasValue)
		{
			return;
		}
		if (mSuccess.Value)
		{
			loginSyncTimer.Dispose();
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			return;
		}
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			mPostOtsImage.ImageName = "syncing_ots_icon";
			((UIElement)mLoadingImage).Visibility = (Visibility)0;
			((UIElement)mPostOtsWarning).Visibility = (Visibility)2;
			((UIElement)mCloseButton).Visibility = (Visibility)2;
			BlueStacksUIBinding.Bind(mPostOtsLabel, "STRING_POST_OTS_SYNCING_MESSAGE");
			BlueStacksUIBinding.Bind((Button)(object)mPostOtsButton, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE");
			((UIElement)mPostOtsButton).IsEnabled = false;
		}, new object[0]);
		SendRetryBluestacksLoginRequest(ParentWindow.mVmName);
	}

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			Logger.Info("Clicked postotswelcome window close button");
			ParentWindow.CloseWindow();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in closing bluestacks from postotswelcome window, " + ex.ToString());
		}
	}

	private void SendRetryBluestacksLoginRequest(string vmName)
	{
		try
		{
			Logger.Info("Sending retry call for token to android, since token is not received successfully");
			mSuccess = null;
			StartingTimer();
			BlueStacksUIUtils.SendBluestacksLoginRequest(vmName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendRetryBluestacksLoginRequest: " + ex.ToString());
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (loginSyncTimer != null)
			{
				loginSyncTimer.Elapsed -= OnLoginSyncTimeout;
				loginSyncTimer.Dispose();
			}
			disposedValue = true;
		}
	}

	~PostOtsWelcomeWindowControl()
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
			Uri uri = new Uri("/Bluestacks;component/controls/postotswelcomewindowcontrol.xaml", UriKind.Relative);
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(PostOtsWelcomeWindowControl)target).Loaded += new RoutedEventHandler(PostOtsWelcome_Loaded);
			break;
		case 2:
			mCloseButton = (CustomPictureBox)target;
			((UIElement)mCloseButton).MouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 3:
			mPostOtsImage = (CustomPictureBox)target;
			break;
		case 4:
			mLoadingImage = (CustomPictureBox)target;
			break;
		case 5:
			mPostOtsLabel = (Label)target;
			break;
		case 6:
			mPostOtsWarning = (TextBlock)target;
			break;
		case 7:
			mPostOtsButton = (CustomButton)target;
			((ButtonBase)mPostOtsButton).Click += new RoutedEventHandler(mPostOtsButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
