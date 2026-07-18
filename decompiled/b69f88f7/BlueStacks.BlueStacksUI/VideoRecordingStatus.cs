using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class VideoRecordingStatus : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private DispatcherTimer mBlinkPlayingIconTimer;

	private DispatcherTimer mTimer;

	private bool mToggleBlinkImage = true;

	internal Action RecordingStoppedEvent;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRecordingImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mDescriptionPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRunningVideo;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTimerDisplay;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStopVideoRecordImg;

	private bool _contentLoaded;

	public DateTime mStartTime { get; set; }

	public VideoRecordingStatus()
	{
		InitializeComponent();
	}

	private void StopRecord_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ResetTimer();
		RecordingStoppedEvent?.Invoke();
		ParentWindow.mCommonHandler.StopRecordVideo();
	}

	private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
	{
		ToggleRecordingIcon();
	}

	internal void StopTimer()
	{
		DispatcherTimer obj = mBlinkPlayingIconTimer;
		if (obj != null && obj.IsEnabled)
		{
			DispatcherTimer obj2 = mBlinkPlayingIconTimer;
			if (obj2 != null)
			{
				obj2.Stop();
			}
		}
		DispatcherTimer obj3 = mTimer;
		if (obj3 != null && obj3.IsEnabled)
		{
			DispatcherTimer obj4 = mTimer;
			if (obj4 != null)
			{
				obj4.Stop();
			}
		}
	}

	internal void StartTimer()
	{
		mBlinkPlayingIconTimer.Start();
	}

	private void ToggleRecordingIcon()
	{
		if (mToggleBlinkImage)
		{
			mRecordingImage.ImageName = "sidebar_video_capture";
		}
		else
		{
			mRecordingImage.ImageName = "sidebar_video_capture_active";
		}
		if (FeatureManager.Instance.IsCustomUIForNCSoft && ParentWindow.mSidebar != null)
		{
			ParentWindow.mSidebar.ChangeVideoRecordingImage(mRecordingImage.ImageName);
		}
		mToggleBlinkImage = !mToggleBlinkImage;
	}

	internal void Init(MainWindow parentWindow)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		ParentWindow = parentWindow;
		if (mBlinkPlayingIconTimer == null)
		{
			mBlinkPlayingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), (DispatcherPriority)7, (EventHandler)BlinkPlayingIcon_Tick, Dispatcher.CurrentDispatcher);
			mStartTime = DateTime.Now;
			mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), (DispatcherPriority)7, (EventHandler)T_Tick, Dispatcher.CurrentDispatcher);
			StartTimer();
		}
	}

	private void T_Tick(object sender, EventArgs e)
	{
		TimeSpan timeSpan = DateTime.Now - mStartTime;
		string text = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", new object[3]
		{
			timeSpan.Minutes,
			timeSpan.Seconds,
			timeSpan.Milliseconds / 10
		});
		mTimerDisplay.Text = text;
	}

	internal void ResetTimer()
	{
		StopTimer();
		mBlinkPlayingIconTimer = null;
		mTimer = null;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/videorecordingstatus.xaml", UriKind.Relative);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			mRecordingImage = (CustomPictureBox)target;
			break;
		case 3:
			mDescriptionPanel = (StackPanel)target;
			break;
		case 4:
			mRunningVideo = (TextBlock)target;
			break;
		case 5:
			mTimerDisplay = (TextBlock)target;
			break;
		case 6:
			mStopVideoRecordImg = (CustomPictureBox)target;
			((UIElement)mStopVideoRecordImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopRecord_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
