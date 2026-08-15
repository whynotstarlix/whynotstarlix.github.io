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

public class MacroTopBarPlayControl : UserControl, IComponentConnector
{
	internal delegate void ScriptPlayDelegate(string tag);

	internal delegate void ScriptStopDelegate(string tag);

	private MainWindow ParentWindow;

	internal MacroRecording mOperationsRecord;

	private DispatcherTimer mBlinkPlayingIconTimer;

	private DispatcherTimer mTimer;

	private bool mShowPlayingIcon = true;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox RecordingImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mDescriptionPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRunningScript;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRunningIterations;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTimerDisplay;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox StopMacroImg;

	private bool _contentLoaded;

	public DateTime mStartTime { get; set; }

	internal event ScriptPlayDelegate ScriptPlayEvent;

	internal event ScriptStopDelegate ScriptStopEvent;

	public MacroTopBarPlayControl()
	{
		InitializeComponent();
	}

	internal void OnScriptPlayEvent(string tag)
	{
		ScriptPlayEvent?.Invoke(tag);
	}

	private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
	{
		ToggleRecordingIcon();
	}

	internal void StopTimer()
	{
		mBlinkPlayingIconTimer.Stop();
		mTimer.Stop();
	}

	internal void StartTimer()
	{
		mBlinkPlayingIconTimer.Start();
		mTimer.Start();
	}

	private void ToggleRecordingIcon()
	{
		if (mShowPlayingIcon)
		{
			RecordingImage.ImageName = "recording_macro_title_play";
			mShowPlayingIcon = false;
		}
		else
		{
			RecordingImage.ImageName = "recording_macro";
			mShowPlayingIcon = true;
		}
	}

	internal void OnScriptStopEvent(string tag)
	{
		ScriptStopEvent?.Invoke(tag);
	}

	internal void Init(MainWindow parentWindow, MacroRecording record)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		ParentWindow = parentWindow;
		mOperationsRecord = record;
		mRunningScript.Text = mOperationsRecord.Name;
		((UIElement)mRunningIterations).Visibility = (Visibility)0;
		TextBlock obj = mRunningScript;
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		object[] array = new object[0x459 ^ 0x45B];
		array[0] = LocaleStrings.GetLocalizedString("STRING_PLAYING", "");
		array[1] = mRunningScript.Text;
		((FrameworkElement)obj).ToolTip = string.Format(invariantCulture, "{0} {1}", array);
		if (mBlinkPlayingIconTimer == null)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 500);
			int num = ((1196899812 > 326318922) ? 7 : 9);
			mBlinkPlayingIconTimer = new DispatcherTimer(timeSpan, (DispatcherPriority)num, (EventHandler)BlinkPlayingIcon_Tick, Dispatcher.CurrentDispatcher);
			mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), (DispatcherPriority)(-2067441782 ^ -2067441779), (EventHandler)T_Tick, Dispatcher.CurrentDispatcher);
			StartTimer();
		}
	}

	private void T_Tick(object sender, EventArgs e)
	{
		TimeSpan timeSpan = DateTime.Now - mStartTime;
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		int num = ((1023453543 > 1036677266) ? 4 : 3);
		object[] array = new object[num];
		array[0] = timeSpan.Minutes;
		array[1] = timeSpan.Seconds;
		int num2 = ((390485118 > 385763861) ? 2 : 2);
		array[num2] = timeSpan.Milliseconds / (115063969 - (0x65BB487 | 0x90A891));
		string text = string.Format(invariantCulture, "{0:00}:{1:00}:{2:00}", array);
		mTimerDisplay.Text = text;
	}

	internal void IncreaseIteration(int iteration)
	{
		mRunningIterations.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), new object[1] { Strings.AddOrdinal(iteration) });
	}

	private void PauseMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
	}

	private void PlayMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		OnScriptPlayEvent(mOperationsRecord.Name);
	}

	private void StopMacro_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		StopMacro();
	}

	public void StopMacro()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		StopTimer();
		mBlinkPlayingIconTimer = null;
		ParentWindow.mCommonHandler.StopMacroScriptHandling();
		ScriptStopEvent?.Invoke(mOperationsRecord.Name);
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", null, ((object)mOperationsRecord.RecordingType/*cast due to constrained. prefix*/).ToString());
	}

	internal void UpdateUiForIterationTillTime()
	{
		((UIElement)mRunningIterations).Visibility = (Visibility)(0x32E51ADA ^ 0x32E51AD8);
		((UIElement)mTimerDisplay).Visibility = (Visibility)0;
		TextBlock obj = mRunningScript;
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		int num = ((2031001236 > 1232594309) ? 2 : 2);
		object[] array = new object[num];
		array[0] = mOperationsRecord.Name;
		array[1] = mOperationsRecord.LoopTime;
		((FrameworkElement)obj).ToolTip = string.Format(invariantCulture, "{0}-{1}sec", array);
	}

	internal void UpdateUiMacroPlaybackForInfiniteTime(int iteration)
	{
		((UIElement)mTimerDisplay).Visibility = (Visibility)(179610247 + ~179610244);
		((UIElement)mRunningIterations).Visibility = (Visibility)0;
		mRunningIterations.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), new object[1] { Strings.AddOrdinal(iteration) });
		((FrameworkElement)mRunningScript).ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", new object[1] { mOperationsRecord.Name });
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotopbarplaycontrol.xaml", UriKind.Relative);
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			RecordingImage = (CustomPictureBox)target;
			break;
		case 3:
			mDescriptionPanel = (StackPanel)target;
			break;
		case 4:
			mRunningScript = (TextBlock)target;
			break;
		case 5:
			mRunningIterations = (TextBlock)target;
			break;
		case 6:
			mTimerDisplay = (TextBlock)target;
			break;
		case 7:
			StopMacroImg = (CustomPictureBox)target;
			((UIElement)StopMacroImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopMacro_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
