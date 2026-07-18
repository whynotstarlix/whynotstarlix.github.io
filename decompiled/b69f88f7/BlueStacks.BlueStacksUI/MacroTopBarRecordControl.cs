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

public class MacroTopBarRecordControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private DispatcherTimer mTimer;

	private DispatcherTimer mBlinkRecordingIconTimer;

	private DateTime mStartTime;

	private DateTime mPauseTime;

	private bool mShowRecordingIcon = true;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRecordingImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock TimerDisplay;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPauseMacroImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPlayMacroImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStopMacroImg;

	private bool _contentLoaded;

	public MacroTopBarRecordControl()
	{
		InitializeComponent();
	}

	internal void Init(MainWindow window)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		ParentWindow = window;
		mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), (DispatcherPriority)7, (EventHandler)T_Tick, Dispatcher.CurrentDispatcher);
		((UIElement)mPlayMacroImg).Visibility = (Visibility)2;
		((UIElement)mPauseMacroImg).Visibility = (Visibility)0;
		mBlinkRecordingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), (DispatcherPriority)7, (EventHandler)BlinkRecordingIcon_Tick, Dispatcher.CurrentDispatcher);
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			BlueStacksUIBinding.Bind(ParentWindow.mNCTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
		}
		else
		{
			BlueStacksUIBinding.Bind(ParentWindow.mTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
		}
	}

	private void BlinkRecordingIcon_Tick(object sender, EventArgs e)
	{
		ToggleRecordingIcon();
	}

	private void PauseMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("pauseRecordingCombo");
		PauseTimer();
		((UIElement)mPauseMacroImg).Visibility = (Visibility)2;
		((UIElement)mPlayMacroImg).Visibility = (Visibility)0;
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_pause", null, ((object)(RecordingTypes)0/*cast due to constrained. prefix*/).ToString());
	}

	private void ResumeMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo");
		ResumeTimer();
		((UIElement)mPlayMacroImg).Visibility = (Visibility)2;
		((UIElement)mPauseMacroImg).Visibility = (Visibility)0;
	}

	private void StopMacroRecording_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow.mCommonHandler.StopMacroRecording();
		mBlinkRecordingIconTimer.Stop();
		mRecordingImage.ImageName = "recording_macro_title_bar";
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", null, ((object)(RecordingTypes)0/*cast due to constrained. prefix*/).ToString());
	}

	internal void StopTimer()
	{
		mTimer.Stop();
		mBlinkRecordingIconTimer.Stop();
	}

	internal void StartTimer()
	{
		mTimer.Start();
		mStartTime = DateTime.Now;
		mBlinkRecordingIconTimer.Start();
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
		TimerDisplay.Text = text;
	}

	private void ToggleRecordingIcon()
	{
		if (mShowRecordingIcon)
		{
			mRecordingImage.ImageName = "recording_macro_active";
			mShowRecordingIcon = false;
		}
		else
		{
			mRecordingImage.ImageName = "recording_macro";
			mShowRecordingIcon = true;
		}
	}

	internal void PauseTimer()
	{
		mTimer.IsEnabled = false;
		mTimer.Stop();
		mPauseTime = DateTime.Now;
		mBlinkRecordingIconTimer.Stop();
		mShowRecordingIcon = true;
		ToggleRecordingIcon();
	}

	internal void ResumeTimer()
	{
		TimeSpan timeSpan = DateTime.Now - mPauseTime;
		mStartTime += timeSpan;
		mTimer.IsEnabled = true;
		mTimer.Start();
		mBlinkRecordingIconTimer.Start();
		mShowRecordingIcon = true;
		ToggleRecordingIcon();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotopbarrecordcontrol.xaml", UriKind.Relative);
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			mRecordingImage = (CustomPictureBox)target;
			break;
		case 3:
			TimerDisplay = (TextBlock)target;
			break;
		case 4:
			mPauseMacroImg = (CustomPictureBox)target;
			((UIElement)mPauseMacroImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PauseMacroRecording_MouseLeftButtonUp);
			break;
		case 5:
			mPlayMacroImg = (CustomPictureBox)target;
			((UIElement)mPlayMacroImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ResumeMacroRecording_MouseLeftButtonUp);
			break;
		case 6:
			mStopMacroImg = (CustomPictureBox)target;
			((UIElement)mStopMacroImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopMacroRecording_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
