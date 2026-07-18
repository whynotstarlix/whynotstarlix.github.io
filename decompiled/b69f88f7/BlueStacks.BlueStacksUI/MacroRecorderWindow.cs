using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class MacroRecorderWindow : CustomWindow, IDisposable, IComponentConnector
{
	private MainWindow ParentWindow;

	internal string mMacroOnRestart;

	internal StackPanel mScriptsStackPanel;

	private System.Timers.Timer mMacroLoopTimer;

	internal ExportMacroWindow mExportMacroWindow;

	internal MergeMacroWindow mMergeMacroWindow;

	internal ImportMacroWindow mImportMacroWindow;

	internal List<MacroRecording> mRenamingMacrosList = new List<MacroRecording>();

	internal List<MacroRecording> mNewlyAddedMacrosList = new List<MacroRecording>();

	internal bool? mImportMultiMacroAsUnified;

	private bool mAlternateBackgroundColor;

	internal BackgroundWorker mBGMacroPlaybackWorker;

	private bool disposedValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mOperationRecorderBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMerge;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mImport;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mExport;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mOpenFolder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mStartMacroRecordingBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mStopMacroRecordingBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mGetMacroBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mNoScriptsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mScriptsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScriptsListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mOpenCommunityBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOverlayGrid;

	private bool _contentLoaded;

	public MacroRecorderWindow(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		((Window)this).Owner = (Window)(object)ParentWindow;
		((CustomWindow)this).IsShowGLWindow = true;
		object content = ((ContentControl)mScriptsListScrollbar).Content;
		mScriptsStackPanel = (StackPanel)((content is StackPanel) ? content : null);
		((Window)this).WindowStartupLocation = (WindowStartupLocation)2;
		if (window != null)
		{
			window.mCommonHandler.MacroSettingChangedEvent += ParentWindow_MacroSettingChangedEvent;
		}
		Init();
		if (ParentWindow != null)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent -= ParentWindow_ScriptPlayEvent;
				ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent -= MacroPlayControl_ScriptStopEvent;
				ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent += ParentWindow_ScriptPlayEvent;
				ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent += MacroPlayControl_ScriptStopEvent;
			}
			else
			{
				ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent -= ParentWindow_ScriptPlayEvent;
				ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent -= MacroPlayControl_ScriptStopEvent;
				ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent += ParentWindow_ScriptPlayEvent;
				ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent += MacroPlayControl_ScriptStopEvent;
			}
		}
	}

	public void ShowAtCenter()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		((Window)this).Show();
		NativeMethods.GetWindowRect((((Window)this).Owner as MainWindow).Handle, out var lpRect);
		NativeMethods.GetWindowRect(new WindowInteropHelper((Window)(object)this).Handle, out var lpRect2);
		RECT val = default(RECT);
		((RECT)(ref val)).Left = (((RECT)(ref lpRect)).Right - ((RECT)(ref lpRect)).Left - ((RECT)(ref lpRect2)).Right + ((RECT)(ref lpRect2)).Left) / 2 + ((RECT)(ref lpRect)).Left;
		((RECT)(ref val)).Top = (((RECT)(ref lpRect)).Bottom - ((RECT)(ref lpRect)).Top - ((RECT)(ref lpRect2)).Bottom + ((RECT)(ref lpRect2)).Top) / 2 + ((RECT)(ref lpRect)).Top;
		RECT val2 = val;
		((RECT)(ref val2)).Right = ((RECT)(ref val2)).Left + ((RECT)(ref lpRect2)).Right - ((RECT)(ref lpRect2)).Left;
		((RECT)(ref val2)).Bottom = ((RECT)(ref val2)).Top + ((RECT)(ref lpRect2)).Bottom - ((RECT)(ref lpRect2)).Top;
		WindowPlacement.SetPlacement(new WindowInteropHelper((Window)(object)this).Handle, val2);
	}

	private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
	{
		if (!record.PlayOnStart)
		{
			return;
		}
		foreach (SingleMacroControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if (child.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == record.Name.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				ChangeAutorunImageVisibility(child.mAutorunImage, (Visibility)0);
				continue;
			}
			ChangeAutorunImageVisibility(child.mAutorunImage, (Visibility)1);
			if (child.mRecording.PlayOnStart)
			{
				child.mRecording.PlayOnStart = false;
				if (child.mMacroSettingsWindow != null)
				{
					((ToggleButton)child.mMacroSettingsWindow.mPlayOnStartCheckBox).IsChecked = false;
				}
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)1;
				string contents = JsonConvert.SerializeObject((object)child.mRecording, serializerSettings);
				File.WriteAllText(CommonHandlers.GetCompleteMacroRecordingPath(child.mRecording.Name), contents);
			}
		}
	}

	private void ChangeAutorunImageVisibility(CustomPictureBox cpb, Visibility visibility)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((UIElement)cpb).Visibility = visibility;
		}, new object[0]);
	}

	private void MacroPlayControl_ScriptStopEvent(string tag)
	{
		SingleMacroControl controlFromTag = GetControlFromTag(tag);
		controlFromTag?.ToggleScriptPlayPauseUi(isScriptRunning: false);
		if ((controlFromTag == null || !controlFromTag.mRecording.DonotShowWindowOnFinish) && !((CustomWindow)ParentWindow).IsClosed)
		{
			ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
		}
	}

	private void ParentWindow_ScriptPlayEvent(string tag)
	{
		GetControlFromTag(tag)?.ToggleScriptPlayPauseUi(isScriptRunning: true);
	}

	public void Init()
	{
		ParentWindow.mIsScriptsPresent = false;
		mAlternateBackgroundColor = false;
		AddScriptsToStackPanel();
		if (!ParentWindow.mIsScriptsPresent)
		{
			((UIElement)mNoScriptsGrid).Visibility = (Visibility)0;
			((UIElement)mExport).IsEnabled = false;
			((UIElement)mExport).Opacity = 0.4;
		}
		else
		{
			((UIElement)mNoScriptsGrid).Visibility = (Visibility)2;
			((UIElement)mExport).IsEnabled = true;
			((UIElement)mExport).Opacity = 1.0;
		}
		if (ParentWindow.mIsMacroRecorderActive)
		{
			((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)2;
			((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)0;
			((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)2;
		}
		ToggleUI(ParentWindow.mIsMacroRecorderActive);
		if (ParentWindow.mIsMacroPlaying)
		{
			((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)1;
			((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)1;
		}
		ShowLoadingGrid(isShow: false);
	}

	private void AddScriptsToStackPanel()
	{
		foreach (MacroRecording item in from MacroRecording macro in MacroGraph.Instance.Vertices
			orderby DateTime.ParseExact(macro.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
			select macro)
		{
			if (item == null || string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.TimeCreated))
			{
				continue;
			}
			if (item.Events == null)
			{
				ObservableCollection<MergedMacroConfiguration> mergedMacroConfigurations = item.MergedMacroConfigurations;
				if (mergedMacroConfigurations == null || mergedMacroConfigurations.Count <= 0)
				{
					continue;
				}
			}
			ParentWindow.mIsScriptsPresent = true;
			SingleMacroControl singleMacroControl = new SingleMacroControl(ParentWindow, item, this);
			((FrameworkElement)singleMacroControl).Tag = item.Name;
			SingleMacroControl singleMacroControl2 = singleMacroControl;
			if (ParentWindow.mIsMacroPlaying && !string.Equals(ParentWindow.mMacroPlaying, item.Name, StringComparison.InvariantCulture))
			{
				CommonHandlers.DisableScriptControl(singleMacroControl2);
			}
			else if (ParentWindow.mIsMacroPlaying)
			{
				((UIElement)singleMacroControl2.mEditNameImg).IsEnabled = false;
			}
			else if (ParentWindow.mIsMacroRecorderActive)
			{
				CommonHandlers.DisableScriptControl(singleMacroControl2);
			}
			if (item.PlayOnStart)
			{
				((UIElement)singleMacroControl2.mAutorunImage).Visibility = (Visibility)0;
			}
			if (mAlternateBackgroundColor)
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)singleMacroControl2, Control.BackgroundProperty, "DarkBandingColor");
			}
			else
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)singleMacroControl2, Control.BackgroundProperty, "LightBandingColor");
			}
			mAlternateBackgroundColor = !mAlternateBackgroundColor;
			((Panel)mScriptsStackPanel).Children.Add((UIElement)(object)singleMacroControl2);
		}
	}

	private SingleMacroControl GetControlFromTag(string tag)
	{
		foreach (SingleMacroControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if ((string)((FrameworkElement)child).Tag == tag)
			{
				return child;
			}
		}
		return null;
	}

	private void OpenScriptFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
		{
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
				process.Start();
			}
		}
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.mCommonHandler.HideMacroRecorderWindow();
	}

	private void mStartMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow.mCommonHandler.StartMacroRecording();
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "new_macro_record", null, ((object)(RecordingTypes)0/*cast due to constrained. prefix*/).ToString());
	}

	private void mStopMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow.mCommonHandler.StopMacroRecording();
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", null, ((object)(RecordingTypes)0/*cast due to constrained. prefix*/).ToString());
	}

	internal void PerformStopMacroAfterSave()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mTopBar.HideRecordingIcons();
			ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
			((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)0;
			((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)2;
			ParentWindow.mIsMacroRecorderActive = false;
		}, new object[0]);
	}

	internal void SaveOperation(string events)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		try
		{
			if (!string.Equals(events, "[]", StringComparison.InvariantCulture))
			{
				_ = RegistryStrings.MacroRecordingsFolderPath;
				MacroRecording val = new MacroRecording();
				string timeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
				val.TimeCreated = timeCreated;
				val.Name = CommonHandlers.GetMacroName();
				val.Events = JsonConvert.DeserializeObject<List<MacroEvents>>(events, Utils.GetSerializerSettings());
				SaveMacroRecord(val);
			}
			else
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_OPERATION_MESSAGE", ""), 4.0, isShowCloseImage: true);
			}
			PerformStopMacroAfterSave();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SaveOperations. Exception: " + ex.ToString());
		}
	}

	internal void SaveMacroRecord(MacroRecording record)
	{
		CommonHandlers.SaveMacroJson(record, record.Name + ".json");
		MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>)(object)record);
		MacroGraph.LinkMacroChilds(record);
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mIsMacroRecorderActive = false;
			foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
			{
				if (dictWindow.Value.MacroRecorderWindow != null)
				{
					((FrameworkElement)new SingleMacroControl(dictWindow.Value, record, this)).Tag = record.Name;
					((UIElement)dictWindow.Value.MacroRecorderWindow.mNoScriptsGrid).Visibility = (Visibility)2;
					((UIElement)mExport).IsEnabled = true;
					((UIElement)mExport).Opacity = 1.0;
					if (!dictWindow.Value.mIsScriptsPresent)
					{
						dictWindow.Value.mIsScriptsPresent = true;
					}
					((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children.Clear();
					dictWindow.Value.MacroRecorderWindow.Init();
					dictWindow.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
					int num = ((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children.Count - 1;
					SingleMacroControl obj = ((Panel)dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel).Children[num] as SingleMacroControl;
					BlueStacksUIBinding.BindColor((DependencyObject)(object)obj.mGrid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
					BlueStacksUIBinding.BindColor((DependencyObject)(object)obj.mScriptName, TextBlock.ForegroundProperty, "WhiteMouseOutBorderBackground");
					BlueStacksUIBinding.BindColor((DependencyObject)(object)obj.mMacroShortcutTextBox, TextBlock.ForegroundProperty, "DualTextBlockForeground");
				}
			}
		}, new object[0]);
	}

	private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
		{
			try
			{
				((Window)this).DragMove();
			}
			catch
			{
			}
		}
	}

	internal void ToggleUI(bool isRecording)
	{
		if (isRecording)
		{
			((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)0;
			((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)2;
			((UIElement)mNoScriptsGrid).Visibility = (Visibility)2;
			((UIElement)mScriptsGrid).Visibility = (Visibility)0;
			return;
		}
		((UIElement)mStopMacroRecordingBtn).Visibility = (Visibility)2;
		((UIElement)mStartMacroRecordingBtn).Visibility = (Visibility)0;
		if (ParentWindow.mIsScriptsPresent)
		{
			((UIElement)mNoScriptsGrid).Visibility = (Visibility)2;
			((UIElement)mScriptsGrid).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mNoScriptsGrid).Visibility = (Visibility)0;
			((UIElement)mScriptsGrid).Visibility = (Visibility)2;
		}
	}

	private void ShowLoadingGrid(bool isShow)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (isShow)
			{
				((UIElement)mLoadingGrid).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mLoadingGrid).Visibility = (Visibility)2;
			}
		}, new object[0]);
	}

	private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_export", null, null);
		if (ParentWindow.mIsScriptsPresent)
		{
			((UIElement)mOverlayGrid).Visibility = (Visibility)0;
			if (mExportMacroWindow == null)
			{
				ExportMacroWindow exportMacroWindow = new ExportMacroWindow(this, ParentWindow);
				((Window)exportMacroWindow).Owner = (Window)(object)this;
				mExportMacroWindow = exportMacroWindow;
				mExportMacroWindow.Init();
				((Window)mExportMacroWindow).ShowDialog();
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, isShowCloseImage: true);
		}
	}

	private void MergeMacroBtn_Click(object sender, MouseButtonEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_icon", null, null);
		if (ParentWindow.mIsScriptsPresent)
		{
			((UIElement)mOverlayGrid).Visibility = (Visibility)0;
			if (mMergeMacroWindow == null)
			{
				MergeMacroWindow mergeMacroWindow = new MergeMacroWindow(this, ParentWindow);
				((Window)mergeMacroWindow).Owner = (Window)(object)ParentWindow;
				mMergeMacroWindow = mergeMacroWindow;
				mMergeMacroWindow.Init();
				((Window)mMergeMacroWindow).Show();
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, isShowCloseImage: true);
		}
	}

	private void ImportBtn_Click(object sender, MouseButtonEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (ParentWindow.mIsMacroPlaying)
			{
				CustomMessageWindow val = new CustomMessageWindow();
				val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_IMPORT_MACRO_WARNING", "");
				val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_WARNING", "");
				val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_STOP_IMPORT", ""), (EventHandler)delegate
				{
					ParentWindow.mTopBar.mMacroPlayControl.StopMacro();
					ImportMacro();
				}, (string)null, false, (object)null);
				((Window)val).Owner = (Window)(object)this;
				((Window)val).ShowDialog();
			}
			else
			{
				ImportMacro();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Importing file. err: " + ex.ToString());
			ShowLoadingGrid(isShow: false);
		}
	}

	private void ImportMacro()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_import", null, null);
		OpenFileDialog val = new OpenFileDialog
		{
			Multiselect = true,
			Filter = "Json files (*.json)|*.json"
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() == 1 && ((FileDialog)val).FileNames.Length != 0)
			{
				if (string.Equals(Path.GetDirectoryName(((FileDialog)val).FileNames[0]), RegistryStrings.MacroRecordingsFolderPath, StringComparison.InvariantCultureIgnoreCase))
				{
					CustomMessageWindow val2 = new CustomMessageWindow
					{
						Owner = (Window)(object)this
					};
					BlueStacksUIBinding.Bind(val2.BodyTextBlock, "STRING_SAME_MACRO_EXISTS", "");
					val2.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)delegate
					{
					}, (string)null, false, (object)null);
					((Window)val2).ShowDialog();
				}
				else
				{
					RunImportMacroScriptBacgroundWorker(((FileDialog)val).FileNames.ToList());
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void RunImportMacroScriptBacgroundWorker(List<string> fileNames)
	{
		using BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += BgImport_DoWork;
		backgroundWorker.RunWorkerCompleted += BgImport_RunWorkerCompleted;
		ShowLoadingGrid(isShow: true);
		backgroundWorker.RunWorkerAsync(fileNames);
	}

	private void BgImport_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			e.Result = CopyMacroScriptIfFileFormatSupported(e.Argument as List<string>);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Importing file. err: " + ex.ToString());
			e.Result = true;
		}
	}

	private void BgImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		ValidateReturnCode((int)e.Result);
		if ((int)e.Result == 0)
		{
			CommonHandlers.RefreshAllMacroWindowWithScroll();
		}
	}

	internal void ValidateReturnCode(int retCode)
	{
		ShowLoadingGrid(isShow: false);
		switch (retCode)
		{
		case 2:
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_FILE_FORMAT_NOT_SUPPORTED", ""), 4.0, isShowCloseImage: true);
			break;
		case 3:
			ShowMacroImportWizard();
			if (mNewlyAddedMacrosList.Count > 0)
			{
				CommonHandlers.RefreshAllMacroWindowWithScroll();
				ShowMacroImportSuccessPopup();
			}
			break;
		case 1:
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_IMPORTING_CANCELLED", ""), 4.0, isShowCloseImage: true);
			break;
		case 0:
			if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
			}
			ShowMacroImportSuccessPopup();
			break;
		}
	}

	internal int CopyMacroScriptIfFileFormatSupported(List<string> selectedFileNames)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		try
		{
			mRenamingMacrosList.Clear();
			bool isShowRenameWizard = false;
			List<string> list = new List<string>();
			List<MacroRecording> list2 = new List<MacroRecording>();
			foreach (string selectedFileName in selectedFileNames)
			{
				MacroRecording val = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(selectedFileName), Utils.GetSerializerSettings());
				if (val == null || string.IsNullOrEmpty(val.Name) || string.IsNullOrEmpty(val.TimeCreated) || ((int)val.RecordingType == 1 && (val.SourceRecordings == null || !val.SourceRecordings.Any())))
				{
					list.Add(Path.GetFileNameWithoutExtension(selectedFileName));
				}
				else
				{
					list2.Add(val);
				}
			}
			if (list2.Any((MacroRecording x) => (int)x.RecordingType == 1))
			{
				AskUserHowToImportMultiMacro();
				if (!mImportMultiMacroAsUnified.HasValue)
				{
					return 1;
				}
			}
			int num = ImportMacroRecordings(list2, ref isShowRenameWizard);
			if (num != 0)
			{
				return num;
			}
			if (list.Count > 0)
			{
				string message = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_INVALID_FILES_LIST", ""), new object[1] { string.Join(", ", list.ToArray()) });
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, message, 4.0, isShowCloseImage: true);
				if (list2.Count <= 0)
				{
					return 4;
				}
			}
			if (isShowRenameWizard)
			{
				return 3;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logger.Error("Wrong file format wont import. err:" + ex.ToString());
			return 2;
		}
	}

	internal int ImportMacroRecordings(List<MacroRecording> recordingsToImport, ref bool isShowRenameWizard)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			mNewlyAddedMacrosList.Clear();
			foreach (MacroRecording item in recordingsToImport)
			{
				item.Shortcut = string.Empty;
				item.PlayOnStart = false;
				if (CheckIfDuplicateMacroInImport(item.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
				{
					isShowRenameWizard = true;
					mRenamingMacrosList.Add(item);
				}
				if ((int)item.RecordingType == 1 && false == mImportMultiMacroAsUnified && !mRenamingMacrosList.Contains(item))
				{
					new List<string>();
					if (CheckIfDuplicateMacroInImport(item.Name, item.MergedMacroConfigurations.SelectMany((MergedMacroConfiguration macro) => macro.MacrosToRun)))
					{
						isShowRenameWizard = true;
						UsefulExtensionMethod.AddIfNotContain<MacroRecording>((IList<MacroRecording>)mRenamingMacrosList, item);
					}
				}
				if (!mRenamingMacrosList.Contains(item))
				{
					item.Name = item.Name.Trim();
					if ((int)item.RecordingType == 0)
					{
						MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>)(object)item);
						mNewlyAddedMacrosList.Add(item);
						CommonHandlers.SaveMacroJson(item, item.Name + ".json");
					}
					else
					{
						ImportMultiMacro(item, mImportMultiMacroAsUnified.Value, mNewlyAddedMacrosList);
					}
				}
			}
			foreach (MacroRecording mNewlyAddedMacros in mNewlyAddedMacrosList)
			{
				MacroGraph.LinkMacroChilds(mNewlyAddedMacros);
			}
		}
		catch
		{
			throw;
		}
		return 0;
	}

	private void AskUserHowToImportMultiMacro()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			CustomMessageWindow val = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_IMPORT_DEPENDENT_MACRO", "");
			val.AddButton((ButtonColors)4, "STRING_IMPORT_ALL_MACROS", (EventHandler)delegate
			{
				mImportMultiMacroAsUnified = false;
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_all", null, null);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_IMPORT_UNIFIED", (EventHandler)delegate
			{
				mImportMultiMacroAsUnified = true;
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_unify", null, null);
			}, (string)null, false, (object)null);
			BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_IMPORT_DEPENDENT_MACRO_UNIFIED", "");
			((UIElement)val.BodyWarningTextBlock).Visibility = (Visibility)0;
			BlueStacksUIBinding.Bind(val.BodyWarningTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
			val.CloseButtonHandle((EventHandler)delegate
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_cancel", null, null);
				mImportMultiMacroAsUnified = null;
			}, (object)null);
			((Window)val).Owner = (Window)(object)this;
			((Window)val).ShowDialog();
		}, new object[0]);
	}

	private List<MacroEvents> GetFlattenedEventsFromMultiRecording(MacroRecording srcRecording, long initialTime, out long elapsedTime, bool isExternalMacro = false)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		List<MacroEvents> list = new List<MacroEvents>();
		elapsedTime = initialTime;
		List<MacroRecording> list2 = MacroGraph.Instance.Vertices.Cast<MacroRecording>().ToList();
		if (isExternalMacro)
		{
			list2.Clear();
			foreach (string sourceRecording in srcRecording.SourceRecordings)
			{
				MacroRecording item = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
				list2.Add(item);
			}
		}
		try
		{
			foreach (MergedMacroConfiguration mergedMacroConfiguration in srcRecording.MergedMacroConfigurations)
			{
				for (int i = 0; i < mergedMacroConfiguration.LoopCount; i++)
				{
					foreach (string gMacro in mergedMacroConfiguration.MacrosToRun)
					{
						MacroRecording val = list2.Where((MacroRecording macro) => string.Equals(gMacro, macro.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
						if ((int)val.RecordingType == 0)
						{
							list.AddRange(GetRecordingEventsFromSourceRecording(val, mergedMacroConfiguration.Acceleration, elapsedTime, ref elapsedTime));
						}
						else
						{
							list.AddRange(GetFlattenedEventsFromMultiRecording(val, elapsedTime, out elapsedTime));
						}
						elapsedTime += mergedMacroConfiguration.LoopInterval * 1000;
					}
					elapsedTime += mergedMacroConfiguration.DelayNextScript * 1000;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't get flattened events. Ex: {0}", new object[1] { ex });
		}
		return list;
	}

	internal void FlattenRecording(MacroRecording srcRecording, bool isExternalMacro = false)
	{
		Logger.Info("Will attempt to flatten {0}", new object[1] { srcRecording.Name });
		srcRecording.Events = GetFlattenedEventsFromMultiRecording(srcRecording, 0L, out var _, isExternalMacro);
		srcRecording.SourceRecordings = null;
		srcRecording.MergedMacroConfigurations = null;
	}

	private static MacroRecording GetFixedMultiMacroSourceRecording(MacroRecording record, string baseChildRecordingName)
	{
		try
		{
			MacroRecording val = UsefulExtensionMethod.DeepCopy<MacroRecording>(record);
			val.Name = GetDependentRecordingName(baseChildRecordingName, val.Name);
			val.MergedMacroConfigurations.Clear();
			foreach (MergedMacroConfiguration mergedMacroConfiguration in record.MergedMacroConfigurations)
			{
				MergedMacroConfiguration val2 = UsefulExtensionMethod.DeepCopy<MergedMacroConfiguration>(mergedMacroConfiguration);
				val2.MacrosToRun.Clear();
				foreach (string item in mergedMacroConfiguration.MacrosToRun)
				{
					val2.MacrosToRun.Add(GetDependentRecordingName(baseChildRecordingName, item));
				}
				val.MergedMacroConfigurations.Add(val2);
			}
			record = val;
			record.SourceRecordings = null;
		}
		catch (Exception ex)
		{
			Logger.Error("Some error occured while fixing dependent source multi macro: Ex: {0}", new object[1] { ex });
		}
		return record;
	}

	internal void ImportMultiMacro(MacroRecording record, bool flatten, List<MacroRecording> newlyAddedMacro, Dictionary<string, string> dependentMacroNewNamesDict = null)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		if (flatten)
		{
			FlattenRecording(record, isExternalMacro: true);
			if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
			{
				record.Name = dependentMacroNewNamesDict[record.Name];
			}
		}
		else
		{
			try
			{
				MacroRecording val = UsefulExtensionMethod.DeepCopy<MacroRecording>(record);
				string text = record.Name;
				if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
				{
					text = (val.Name = dependentMacroNewNamesDict[record.Name]);
				}
				foreach (string sourceRecording in record.SourceRecordings)
				{
					MacroRecording val2 = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
					if ((int)val2.RecordingType == 1)
					{
						val2 = GetFixedMultiMacroSourceRecording(val2, text);
					}
					else
					{
						string name = val2.Name;
						string name2 = GetDependentRecordingName(text, name);
						if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(name))
						{
							name2 = dependentMacroNewNamesDict[name];
						}
						val2.Name = name2;
					}
					MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>)(object)val2);
					newlyAddedMacro.Add(val2);
					CommonHandlers.SaveMacroJson(val2, val2.Name + ".json");
				}
				val.MergedMacroConfigurations.Clear();
				foreach (MergedMacroConfiguration mergedMacroConfiguration in record.MergedMacroConfigurations)
				{
					MergedMacroConfiguration val3 = UsefulExtensionMethod.DeepCopy<MergedMacroConfiguration>(mergedMacroConfiguration);
					val3.MacrosToRun.Clear();
					new ObservableCollection<string>();
					foreach (string item2 in mergedMacroConfiguration.MacrosToRun)
					{
						string item = GetDependentRecordingName(text, item2);
						if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(item2))
						{
							item = dependentMacroNewNamesDict[item2];
						}
						val3.MacrosToRun.Add(item);
					}
					val.MergedMacroConfigurations.Add(val3);
				}
				record = val;
			}
			catch (Exception ex)
			{
				Logger.Error("Some error occured: Ex: {0}", new object[1] { ex });
			}
			record.SourceRecordings = null;
		}
		MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>)(object)record);
		newlyAddedMacro.Add(record);
		CommonHandlers.SaveMacroJson(record, record.Name + ".json");
	}

	private static List<MacroEvents> GetRecordingEventsFromSourceRecording(MacroRecording srcRecording, double acceleration, long initialTime, ref long elapsedTime)
	{
		if (srcRecording == null)
		{
			throw new Exception("Source recording now found in multiMacro");
		}
		List<MacroEvents> list = new List<MacroEvents>();
		foreach (MacroEvents @event in srcRecording.Events)
		{
			MacroEvents val = UsefulExtensionMethod.DeepCopy<MacroEvents>(@event);
			val.Timestamp = (long)Math.Floor((double)@event.Timestamp / acceleration);
			val.Timestamp += initialTime;
			elapsedTime = val.Timestamp;
			list.Add(val);
		}
		return list;
	}

	private static bool CheckIfDuplicateMacroInImport(string origMacroName, IEnumerable<string> lsMacros)
	{
		foreach (string lsMacro in lsMacros)
		{
			string dependentRecordingName = GetDependentRecordingName(lsMacro, origMacroName);
			if ((from MacroRecording macro in MacroGraph.Instance.Vertices
				select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(dependentRecordingName.ToLower(CultureInfo.InvariantCulture)))
			{
				return true;
			}
		}
		return false;
	}

	internal static string GetDependentRecordingName(string originalMacroName, string dependentMacroName)
	{
		return originalMacroName + "-" + dependentMacroName;
	}

	private static bool CheckIfDuplicateMacroInImport(string macroName)
	{
		if ((from MacroRecording macro in MacroGraph.Instance.Vertices
			select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(macroName.ToLower(CultureInfo.InvariantCulture)))
		{
			return true;
		}
		return false;
	}

	private void ShowMacroImportWizard()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			((UIElement)mOverlayGrid).Visibility = (Visibility)0;
			if (mImportMacroWindow == null)
			{
				ImportMacroWindow importMacroWindow = new ImportMacroWindow(this, ParentWindow);
				((Window)importMacroWindow).Owner = (Window)(object)this;
				mImportMacroWindow = importMacroWindow;
				mImportMacroWindow.Init();
				((Window)mImportMacroWindow).ShowDialog();
			}
		}, new object[0]);
	}

	private void MImportBtn_Click(object sender, RoutedEventArgs e)
	{
		CommonHandlers.RefreshAllMacroWindowWithScroll();
	}

	private void mBGMacroPlaybackWorker_DoWork(BackgroundWorker bg, MacroRecording record)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected I4, but got Unknown
		if (File.Exists(CommonHandlers.GetCompleteMacroRecordingPath(record.Name)) && !ParentWindow.mIsMacroPlaying)
		{
			Logger.Debug("Macro Playback started");
			ParentWindow.mIsMacroPlaying = true;
			ParentWindow.mFrontendHandler.SendFrontendRequest("initMacroPlayback", new Dictionary<string, string> { 
			{
				"scriptFilePath",
				CommonHandlers.GetCompleteMacroRecordingPath(record.Name)
			} });
			OperationsLoopType loopType = record.LoopType;
			switch ((int)loopType)
			{
			case 0:
				HandleMacroPlaybackTillLoopNumber(bg, record);
				break;
			case 1:
				HandleMacroPlaybackTillTime(bg, record);
				break;
			case 2:
				HandleMacroPlaybackUntillStopped(bg, record);
				break;
			}
		}
	}

	internal void RunMacroOperation(MacroRecording record)
	{
		BackgroundWorker bg = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		bg.DoWork += delegate
		{
			mBGMacroPlaybackWorker_DoWork(bg, record);
		};
		mBGMacroPlaybackWorker = bg;
		bg.RunWorkerAsync();
	}

	private void HandleMacroPlaybackUntillStopped(BackgroundWorker bg, MacroRecording record)
	{
		try
		{
			EventWaitHandle eventWaitHandle = null;
			string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(ParentWindow.mVmName);
			ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
			int num = 1;
			UpdateMacroPlayBackUI(num, record);
			while (ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
			{
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit");
				if (eventWaitHandle == null)
				{
					eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, macroPlaybackEventName);
				}
				UpdateMacroPlayBackUI(num, record);
				num++;
				eventWaitHandle.WaitOne();
				Thread.Sleep(record.LoopInterval * 1000);
			}
			eventWaitHandle.Close();
		}
		catch (Exception ex)
		{
			Logger.Error("Error in macroplaybackuntil stopped. err:" + ex.ToString());
		}
	}

	private void HandleMacroPlaybackTillTime(BackgroundWorker bg, MacroRecording record)
	{
		try
		{
			if (record.LoopTime > 0)
			{
				EventWaitHandle eventWaitHandle = null;
				string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(ParentWindow.mVmName);
				mMacroLoopTimer = new System.Timers.Timer(record.LoopTime)
				{
					Interval = record.LoopTime * 1000
				};
				mMacroLoopTimer.Elapsed += delegate
				{
					MacroLoopTimer_Elapsed(record.Name);
				};
				DateTime now = DateTime.Now;
				TimeSpan timeSpan = DateTime.Now - now;
				ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
				mMacroLoopTimer.Enabled = true;
				int i = 1;
				UpdateMacroPlayBackUI(i, record);
				while (timeSpan.TotalSeconds < (double)record.LoopTime && ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
				{
					ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit");
					if (eventWaitHandle == null)
					{
						eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, macroPlaybackEventName);
					}
					eventWaitHandle.WaitOne();
					Thread.Sleep(record.LoopInterval * 1000);
					timeSpan = DateTime.Now - now;
				}
				eventWaitHandle.Close();
			}
			else
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_TIMER_SET", ""), 4.0, isShowCloseImage: true);
				Logger.Debug("Macro timer set to ZERO");
				SendStopMacroEventsAndUpdateUi(record.Name);
			}
			if (mMacroLoopTimer != null)
			{
				mMacroLoopTimer.Enabled = false;
				mMacroLoopTimer = null;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in MacroPlaybacktillTime. err:" + ex.ToString());
		}
	}

	private void MacroLoopTimer_Elapsed(string fileName)
	{
		Logger.Debug("Macro timer finished");
		SendStopMacroEventsAndUpdateUi(fileName);
	}

	private void SendStopMacroEventsAndUpdateUi(string fileName)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			ParentWindow.mCommonHandler.StopMacroScriptHandling();
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
			}
			else
			{
				ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
			}
		}, new object[0]);
	}

	private void HandleMacroPlaybackTillLoopNumber(BackgroundWorker bg, MacroRecording record)
	{
		try
		{
			string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(ParentWindow.mVmName);
			if (record.LoopNumber >= 1)
			{
				ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
			}
			else
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_LOOP_ITERATION_SET", ""), 4.0, isShowCloseImage: true);
				Logger.Debug("Macro loop iterations set to ZERO");
			}
			EventWaitHandle eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, macroPlaybackEventName);
			for (int i = 1; i <= record.LoopNumber; i++)
			{
				if (!ParentWindow.mIsMacroPlaying)
				{
					break;
				}
				if (bg.CancellationPending)
				{
					break;
				}
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit");
				UpdateMacroPlayBackUI(i, record);
				eventWaitHandle.WaitOne();
				if (i != record.LoopNumber)
				{
					Thread.Sleep(record.LoopInterval * 1000);
				}
			}
			eventWaitHandle.Close();
			if (bg.CancellationPending || !ParentWindow.mIsMacroPlaying)
			{
				return;
			}
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				ParentWindow.mCommonHandler.StopMacroPlaybackOperation();
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
				}
				else
				{
					ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception err: " + ex.ToString());
		}
	}

	private void UpdateMacroPlayBackUI(int i, MacroRecording record)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Invalid comparison between Unknown and I4
			if ((int)record.LoopType == 0)
			{
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					((UIElement)ParentWindow.mNCTopBar.mMacroPlayControl.mTimerDisplay).Visibility = (Visibility)2;
					((UIElement)ParentWindow.mNCTopBar.mMacroPlayControl.mRunningIterations).Visibility = (Visibility)0;
					ParentWindow.mNCTopBar.mMacroPlayControl.IncreaseIteration(i);
				}
				else
				{
					((UIElement)ParentWindow.mTopBar.mMacroPlayControl.mTimerDisplay).Visibility = (Visibility)2;
					((UIElement)ParentWindow.mTopBar.mMacroPlayControl.mRunningIterations).Visibility = (Visibility)0;
					ParentWindow.mTopBar.mMacroPlayControl.IncreaseIteration(i);
				}
			}
			else if ((int)record.LoopType == 1)
			{
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
				}
				else
				{
					ParentWindow.mTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
				}
			}
			else if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
			}
			else
			{
				ParentWindow.mTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
			}
		}, new object[0]);
	}

	private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
			}
			using Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Some error in Open folder err: " + ex.ToString());
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			mBGMacroPlaybackWorker?.Dispose();
			mMacroLoopTimer?.Dispose();
			disposedValue = true;
		}
	}

	~MacroRecorderWindow()
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

	private void OpenCommunityBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.Equals(((FrameworkElement)((sender is CustomButton) ? sender : null)).Name, "mOpenCommunityBtn", StringComparison.InvariantCultureIgnoreCase))
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Open_Community_Button", null);
		}
		else
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Get_Macro_Button", null);
		}
		OpenCommunityAndCloseMacroWindow(BlueStacksUIUtils.GetMacroCommunityUrl(ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName));
	}

	internal void OpenCommunityAndCloseMacroWindow(string url)
	{
		ParentWindow.mTopBar.mAppTabButtons.AddWebTab(url ?? BlueStacksUIUtils.GetMacroCommunityUrl(ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName), "STRING_MACRO_COMMUNITY", "community_big", isSwitch: true, "STRING_MACRO_COMMUNITY");
		ParentWindow.mCommonHandler.HideMacroRecorderWindow();
		((UIElement)ParentWindow).Focus();
	}

	internal void ShowMacroImportSuccessPopup()
	{
		ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_MACRO_IMPORT_SUCCESS", ""), 2.0, isShowCloseImage: true);
	}

	private void CustomWindow_Closing(object sender, CancelEventArgs e)
	{
		((UIElement)this).Visibility = (Visibility)1;
		e.Cancel = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrorecorderwindow.xaml", UriKind.Relative);
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
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((Window)(MacroRecorderWindow)target).Closing += CustomWindow_Closing;
			break;
		case 2:
			mOperationRecorderBorder = (Border)target;
			break;
		case 3:
			mMaskBorder = (Border)target;
			break;
		case 4:
			((UIElement)(Grid)target).MouseDown += new MouseButtonEventHandler(Topbar_MouseDown);
			break;
		case 5:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 6:
			mMerge = (CustomPictureBox)target;
			((UIElement)mMerge).MouseLeftButtonUp += new MouseButtonEventHandler(MergeMacroBtn_Click);
			break;
		case 7:
			mImport = (CustomPictureBox)target;
			((UIElement)mImport).MouseLeftButtonUp += new MouseButtonEventHandler(ImportBtn_Click);
			break;
		case 8:
			mExport = (CustomPictureBox)target;
			((UIElement)mExport).MouseLeftButtonUp += new MouseButtonEventHandler(ExportBtn_Click);
			break;
		case 9:
			mOpenFolder = (CustomPictureBox)target;
			((UIElement)mOpenFolder).MouseLeftButtonUp += new MouseButtonEventHandler(OpenFolder_MouseLeftButtonUp);
			break;
		case 10:
			mStartMacroRecordingBtn = (CustomButton)target;
			((ButtonBase)mStartMacroRecordingBtn).Click += new RoutedEventHandler(mStartMacroRecordingBtn_Click);
			break;
		case 11:
			mStopMacroRecordingBtn = (CustomButton)target;
			((ButtonBase)mStopMacroRecordingBtn).Click += new RoutedEventHandler(mStopMacroRecordingBtn_Click);
			break;
		case 12:
			mGetMacroBtn = (CustomButton)target;
			((ButtonBase)mGetMacroBtn).Click += new RoutedEventHandler(OpenCommunityBtn_Click);
			break;
		case 13:
			mNoScriptsGrid = (Border)target;
			break;
		case 14:
			mScriptsGrid = (Grid)target;
			break;
		case 15:
			mScriptsListScrollbar = (ScrollViewer)target;
			break;
		case 16:
			mOpenCommunityBtn = (CustomButton)target;
			((ButtonBase)mOpenCommunityBtn).Click += new RoutedEventHandler(OpenCommunityBtn_Click);
			break;
		case 17:
			mLoadingGrid = (ProgressBar)target;
			break;
		case 18:
			mOverlayGrid = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
