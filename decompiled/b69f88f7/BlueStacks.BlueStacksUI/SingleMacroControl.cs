using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class SingleMacroControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	internal MacroRecorderWindow mMacroRecorderWindow;

	internal MacroRecording mRecording;

	internal MacroSettingsWindow mMacroSettingsWindow;

	private CustomMessageWindow mDeleteScriptMessageWindow;

	private bool mIsBookmarked;

	private string mLastScriptName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBookmarkImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mScriptNameGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mScriptName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mUserNameTextblock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mUserNameHyperlink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mEditNameImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTimestamp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mPrefix;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mMacroShortcutTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mScriptPlayPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAutorunImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCommunityMacroImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPlayScriptImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mScriptSettingsImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMergeScriptSettingsImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mDeleteScriptImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mScriptRunningPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mStopScriptImg;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRunning;

	private bool _contentLoaded;

	public bool IsBookmarked
	{
		get
		{
			return mIsBookmarked;
		}
		set
		{
			mIsBookmarked = value;
			ToggleBookmarkIcon(value);
		}
	}

	private void ToggleBookmarkIcon(bool isBookmarked)
	{
		if (isBookmarked)
		{
			mBookmarkImg.ImageName = "bookmarked";
		}
		else
		{
			mBookmarkImg.ImageName = "bookmark";
		}
	}

	internal SingleMacroControl(MainWindow parentWindow, MacroRecording record, MacroRecorderWindow recorderWindow)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Invalid comparison between Unknown and I4
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		mRecording = record;
		ParentWindow = parentWindow;
		mMacroRecorderWindow = recorderWindow;
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mMacroShortcutTextBox, false);
		mTimestamp.Text = DateTime.ParseExact(mRecording.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).ToString("yyyy.MM.dd HH.mm.ss", CultureInfo.InvariantCulture);
		((TextBox)mScriptName).Text = mRecording.Name;
		((TextBox)mMacroShortcutTextBox).Text = IMAPKeys.GetStringForUI(mRecording.Shortcut);
		((FrameworkElement)mScriptName).ToolTip = ((TextBox)mScriptName).Text;
		if ((int)record.RecordingType == 1)
		{
			((UIElement)mScriptSettingsImg).Visibility = (Visibility)2;
			((UIElement)mMergeScriptSettingsImg).Visibility = (Visibility)0;
		}
		if (!string.IsNullOrEmpty(mRecording.Shortcut))
		{
			Key key = IMAPKeys.mDictKeys.FirstOrDefault((KeyValuePair<Key, string> x) => x.Value == mRecording.Shortcut).Key;
			((FrameworkElement)mMacroShortcutTextBox).Tag = IMAPKeys.GetStringForFile(key);
			MainWindow.sMacroMapping[((FrameworkElement)mMacroShortcutTextBox).Tag.ToString()] = ((TextBox)mScriptName).Text;
		}
		else
		{
			((FrameworkElement)mMacroShortcutTextBox).Tag = "";
		}
		IsBookmarked = BlueStacksUIUtils.CheckIfMacroScriptBookmarked(mRecording.Name);
		if (record.PlayOnStart)
		{
			((UIElement)mAutorunImage).Visibility = (Visibility)0;
		}
		if (ParentWindow.mIsMacroPlaying && string.Equals(mRecording.Name, ParentWindow.mMacroPlaying, StringComparison.InvariantCulture))
		{
			ToggleScriptPlayPauseUi(isScriptRunning: true);
		}
		else
		{
			ToggleScriptPlayPauseUi(isScriptRunning: false);
		}
	}

	public void UpdateMacroRecordingObject(MacroRecording record)
	{
		mRecording = record;
	}

	public static bool DeleteScriptNameFromBookmarkedScriptListIfPresent(string fileName)
	{
		if (Enumerable.Contains(RegistryManager.Instance.BookmarkedScriptList, fileName))
		{
			List<string> list = new List<string>(RegistryManager.Instance.BookmarkedScriptList);
			list.Remove(fileName);
			RegistryManager.Instance.BookmarkedScriptList = list.ToArray();
			return true;
		}
		return false;
	}

	public bool AddScriptNameToBookmarkedScriptListIfNotPresent(string fileName)
	{
		if (!Enumerable.Contains(RegistryManager.Instance.BookmarkedScriptList, mRecording.Name))
		{
			List<string> list = new List<string>(RegistryManager.Instance.BookmarkedScriptList) { fileName };
			RegistryManager.Instance.BookmarkedScriptList = list.ToArray();
			return true;
		}
		return false;
	}

	private void UpdateMacroDeleteWindowSettings()
	{
		ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !((ToggleButton)mDeleteScriptMessageWindow.CheckBox).IsChecked.Value;
		mDeleteScriptMessageWindow = null;
	}

	private void DeleteMacroScript()
	{
		string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, mRecording.Name + ".json");
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (mRecording.Shortcut != null && MainWindow.sMacroMapping.ContainsKey(mRecording.Shortcut))
		{
			MainWindow.sMacroMapping.Remove(mRecording.Shortcut);
		}
		DeleteScriptNameFromBookmarkedScriptListIfPresent(mRecording.Name);
		MacroRecording val = (from MacroRecording macro in MacroGraph.Instance.Vertices
			where string.Equals(macro.Name, mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
			select macro).FirstOrDefault();
		MacroGraph.Instance.RemoveVertex((BiDirectionalVertex<MacroRecording>)(object)val);
		if (ParentWindow.mAutoRunMacro != null && ParentWindow.mAutoRunMacro.Name.ToLower(CultureInfo.InvariantCulture).Trim() == mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
		{
			ParentWindow.mAutoRunMacro = null;
		}
		CommonHandlers.RefreshAllMacroRecorderWindow();
		CommonHandlers.OnMacroDeleted(mRecording.Name);
	}

	private void SingleMacroControl_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mGrid, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		((UIElement)mEditNameImg).Visibility = (Visibility)0;
	}

	private void SingleMacroControl_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		((Panel)mGrid).Background = (Brush)new SolidColorBrush(Colors.Transparent);
		((UIElement)mEditNameImg).Visibility = (Visibility)1;
	}

	private void PauseScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
	}

	private void StopScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		ToggleScriptPlayPauseUi(isScriptRunning: false);
		ParentWindow.mCommonHandler.StopMacroScriptHandling();
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", null, ((object)mRecording.RecordingType/*cast due to constrained. prefix*/).ToString());
	}

	internal void ToggleScriptPlayPauseUi(bool isScriptRunning)
	{
		if (isScriptRunning)
		{
			((UIElement)mScriptPlayPanel).Visibility = (Visibility)2;
			((UIElement)mScriptRunningPanel).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mScriptPlayPanel).Visibility = (Visibility)0;
			((UIElement)mScriptRunningPanel).Visibility = (Visibility)2;
		}
	}

	private void ScriptSettingsImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mMacroRecorderWindow.mOverlayGrid).Visibility = (Visibility)0;
		MacroRecording val = (from MacroRecording macro in MacroGraph.Instance.Vertices
			where macro.Equals(mRecording)
			select macro).FirstOrDefault();
		MacroRecording obj = mRecording;
		if (obj != null && (int)obj.RecordingType == 1)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_edit", null, null);
			if (mMacroRecorderWindow.mMergeMacroWindow == null)
			{
				MacroRecorderWindow macroRecorderWindow = mMacroRecorderWindow;
				MergeMacroWindow mergeMacroWindow = new MergeMacroWindow(mMacroRecorderWindow, ParentWindow);
				((Window)mergeMacroWindow).Owner = (Window)(object)ParentWindow;
				macroRecorderWindow.mMergeMacroWindow = mergeMacroWindow;
			}
			mMacroRecorderWindow.mMergeMacroWindow.Init(val, this);
			((Window)mMacroRecorderWindow.mMergeMacroWindow).Show();
		}
		else
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_settings", null, ((object)mRecording.RecordingType/*cast due to constrained. prefix*/).ToString());
			if (mMacroSettingsWindow == null || ((CustomWindow)mMacroSettingsWindow).IsClosed)
			{
				mMacroSettingsWindow = new MacroSettingsWindow(ParentWindow, val, mMacroRecorderWindow);
			}
			((Window)mMacroSettingsWindow).ShowDialog();
		}
	}

	private void BookMarkScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (IsBookmarked)
		{
			IsBookmarked = false;
			DeleteScriptNameFromBookmarkedScriptListIfPresent(mRecording.Name);
			ParentWindow.mCommonHandler.OnMacroBookmarkChanged(mRecording.Name, wasBookmarked: false);
		}
		else if (RegistryManager.Instance.BookmarkedScriptList.Length < 5)
		{
			IsBookmarked = true;
			AddScriptNameToBookmarkedScriptListIfNotPresent(mRecording.Name);
			ParentWindow.mCommonHandler.OnMacroBookmarkChanged(mRecording.Name, wasBookmarked: true);
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_BOOKMARK_MACRO_WARNING", ""), 4.0, isShowCloseImage: true);
		}
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_bookmark", null, ((object)mRecording.RecordingType/*cast due to constrained. prefix*/).ToString());
	}

	private void DeleteScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		MacroRecording currentRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
			where string.Equals(macro.Name, mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
			select macro).FirstOrDefault();
		if (currentRecording == null)
		{
			return;
		}
		if (((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents.Count > 0)
		{
			mDeleteScriptMessageWindow = new CustomMessageWindow();
			mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_DEPENDENT_MACRO", "");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LocaleStrings.GetLocalizedString("STRING_MACRO_IN_USE_BY_OTHER_MACROS", ""));
			stringBuilder.Append(" ");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_MACRO_LOSE_CONFIGURABILITY", ""), new object[1] { mRecording.Name }));
			mDeleteScriptMessageWindow.BodyTextBlock.Text = stringBuilder.ToString();
			mDeleteScriptMessageWindow.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_DELETE_ANYWAY", ""), (EventHandler)delegate
			{
				int i;
				for (i = ((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents.Count - 1; i >= 0; i--)
				{
					MacroRecording val = (from MacroRecording macro in MacroGraph.Instance.Vertices
						where ((object)macro).Equals((object?)((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents[i])
						select macro).FirstOrDefault();
					MacroRecorderWindow macroRecorderWindow = mMacroRecorderWindow;
					BiDirectionalVertex<MacroRecording> obj = ((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents[i];
					macroRecorderWindow.FlattenRecording((MacroRecording)(object)((obj is MacroRecording) ? obj : null));
					BiDirectionalVertex<MacroRecording> obj2 = ((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents[i];
					BiDirectionalVertex<MacroRecording> record = ((obj2 is MacroRecording) ? obj2 : null);
					BiDirectionalVertex<MacroRecording> obj3 = ((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents[i];
					CommonHandlers.SaveMacroJson((MacroRecording)(object)record, ((MacroRecording)((obj3 is MacroRecording) ? obj3 : null)).Name + ".json");
					foreach (SingleMacroControl child in ((Panel)mMacroRecorderWindow.mScriptsStackPanel).Children)
					{
						if (child.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == val.Name.ToLower(CultureInfo.InvariantCulture).Trim())
						{
							child.mScriptSettingsImg.ImageName = "macro_settings";
						}
					}
					BiDirectionalGraph<MacroRecording> instance = MacroGraph.Instance;
					BiDirectionalVertex<MacroRecording> obj4 = ((BiDirectionalVertex<MacroRecording>)(object)currentRecording).Parents[i];
					instance.DeLinkMacroChild((obj4 is MacroRecording) ? obj4 : null);
				}
				DeleteMacroScript();
				CommonHandlers.RefreshAllMacroRecorderWindow();
			}, (string)null, false, (object)null);
			mDeleteScriptMessageWindow.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_DONT_DELETE", ""), (EventHandler)delegate
			{
			}, (string)null, false, (object)null);
			mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler)delegate
			{
			}, (object)null);
			((Window)mDeleteScriptMessageWindow).Owner = (Window)(object)ParentWindow;
			((Window)mDeleteScriptMessageWindow).ShowDialog();
		}
		else if (!ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup)
		{
			DeleteMacroScript();
		}
		else
		{
			mDeleteScriptMessageWindow = new CustomMessageWindow();
			mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_MACRO", "");
			mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCRIPT", "");
			((ContentControl)mDeleteScriptMessageWindow.CheckBox).Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
			((UIElement)mDeleteScriptMessageWindow.CheckBox).Visibility = (Visibility)0;
			((ToggleButton)mDeleteScriptMessageWindow.CheckBox).IsChecked = false;
			mDeleteScriptMessageWindow.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), (EventHandler)FlattenTargetMacrosAndDeleteSourceMacro, (string)null, false, (object)null);
			mDeleteScriptMessageWindow.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler)delegate
			{
				ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !((ToggleButton)mDeleteScriptMessageWindow.CheckBox).IsChecked.Value;
			}, (string)null, false, (object)null);
			mDeleteScriptMessageWindow.CloseButtonHandle((EventHandler)delegate
			{
			}, (object)null);
			((Window)mDeleteScriptMessageWindow).Owner = (Window)(object)ParentWindow;
			((Window)mDeleteScriptMessageWindow).ShowDialog();
		}
	}

	private void FlattenTargetMacrosAndDeleteSourceMacro(object sender, EventArgs e)
	{
		UpdateMacroDeleteWindowSettings();
		DeleteMacroScript();
	}

	private void PlayScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!ParentWindow.mIsMacroPlaying)
		{
			if (MacroGraph.CheckIfDependentMacrosAreAvailable(mRecording))
			{
				ToggleScriptPlayPauseUi(isScriptRunning: true);
				ParentWindow.mCommonHandler.PlayMacroScript(mRecording);
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "macro_popup", ((object)mRecording.RecordingType/*cast due to constrained. prefix*/).ToString(), string.IsNullOrEmpty(mRecording.MacroId) ? "local" : "community");
				ParentWindow.mCommonHandler.HideMacroRecorderWindow();
			}
			else
			{
				CustomMessageWindow val = new CustomMessageWindow
				{
					Owner = (Window)(object)mMacroRecorderWindow
				};
				BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_ERROR_IN_MERGE_MACRO", "");
				val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)delegate
				{
				}, (string)null, false, (object)null);
				((Window)val).ShowDialog();
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_STOP_THE_SCRIPT", ""), 4.0, isShowCloseImage: true);
		}
	}

	private void EditMacroName_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (string.Equals(mEditNameImg.ImageName, "edit_icon", StringComparison.InvariantCulture))
		{
			((RoutedEventArgs)e).Handled = true;
			((UIElement)mScriptName).IsEnabled = true;
			mEditNameImg.ImageName = "macro_name_save";
			((FrameworkElement)mScriptName).Width = ((FrameworkElement)mScriptNameGrid).ActualWidth - ((FrameworkElement)mEditNameImg).ActualWidth - ((FrameworkElement)mTimestamp).ActualWidth - ((FrameworkElement)mPrefix).ActualWidth - 30.0;
			mLastScriptName = ((TextBox)mScriptName).Text;
			((UIElement)mScriptName).Focusable = true;
			((TextBoxBase)mScriptName).IsReadOnly = false;
			((UIElement)mScriptName).Focus();
			BlueStacksUIBinding.Bind((Image)(object)mEditNameImg, "STRING_SAVE");
			((TextBox)mScriptName).CaretIndex = ((TextBox)mScriptName).Text.Length;
		}
		else
		{
			PerformSaveMacroNameOperations();
		}
	}

	private void PerformSaveMacroNameOperations()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mScriptName).IsEnabled = false;
		((UIElement)mScriptName).Focusable = false;
		((TextBoxBase)mScriptName).IsReadOnly = true;
		((Control)mScriptName).BorderThickness = new Thickness(0.0);
		mEditNameImg.ImageName = "edit_icon";
		BlueStacksUIBinding.Bind((Image)(object)mEditNameImg, "STRING_RENAME");
		SaveMacroName();
	}

	private void SaveMacroName()
	{
		if (!string.IsNullOrEmpty(((TextBox)mScriptName).Text.Trim()))
		{
			if (!(from MacroRecording macro in MacroGraph.Instance.Vertices
				select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(((TextBox)mScriptName).Text.ToLower(CultureInfo.InvariantCulture).Trim()) || ((TextBox)mScriptName).Text.ToLower(CultureInfo.InvariantCulture).Trim() == mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				if (((TextBox)mScriptName).Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
					{
						LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
						Environment.NewLine,
						"\\ / : * ? \" < > |"
					});
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, message, 4.0, isShowCloseImage: true);
					((TextBox)mScriptName).Text = mLastScriptName;
				}
				else if (Enumerable.Contains(Constants.ReservedFileNamesList, ((TextBox)mScriptName).Text.Trim().ToLower(CultureInfo.InvariantCulture)))
				{
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", ""), 4.0, isShowCloseImage: true);
					((TextBox)mScriptName).Text = mLastScriptName;
				}
				else
				{
					SaveScriptSettings();
				}
			}
			else
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", ""), 4.0, isShowCloseImage: true);
				((TextBox)mScriptName).Text = mLastScriptName;
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), 4.0, isShowCloseImage: true);
			((TextBox)mScriptName).Text = mLastScriptName;
		}
	}

	private void SaveScriptSettings()
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (string.Equals(mRecording.Shortcut, ((FrameworkElement)mMacroShortcutTextBox).Tag.ToString(), StringComparison.InvariantCulture) && string.Equals(mRecording.Name.Trim(), ((TextBox)mScriptName).Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)1;
			string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, mRecording.Name + ".json");
			if (mRecording.Shortcut != ((FrameworkElement)mMacroShortcutTextBox).Tag.ToString())
			{
				if (!string.IsNullOrEmpty(mRecording.Shortcut) && MainWindow.sMacroMapping.ContainsKey(mRecording.Shortcut))
				{
					MainWindow.sMacroMapping.Remove(mRecording.Shortcut);
				}
				if (((FrameworkElement)mMacroShortcutTextBox).Tag != null && !string.IsNullOrEmpty(((FrameworkElement)mMacroShortcutTextBox).Tag.ToString()))
				{
					MainWindow.sMacroMapping[((FrameworkElement)mMacroShortcutTextBox).Tag.ToString()] = ((TextBox)mScriptName).Text;
				}
				if (mRecording.Shortcut != null && ((FrameworkElement)mMacroShortcutTextBox).Tag != null && !string.Equals(mRecording.Shortcut, ((FrameworkElement)mMacroShortcutTextBox).Tag.ToString(), StringComparison.InvariantCulture))
				{
					ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_shortcutkey", null, ((object)mRecording.RecordingType/*cast due to constrained. prefix*/).ToString());
				}
				if (((FrameworkElement)mMacroShortcutTextBox).Tag != null)
				{
					mRecording.Shortcut = ((FrameworkElement)mMacroShortcutTextBox).Tag.ToString();
				}
				if (mRecording.PlayOnStart)
				{
					ParentWindow.mAutoRunMacro = mRecording;
				}
				string contents = JsonConvert.SerializeObject((object)mRecording, serializerSettings);
				File.WriteAllText(text, contents);
			}
			if (!string.Equals(mRecording.Name.Trim(), ((TextBox)mScriptName).Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
			{
				string oldMacroName = mRecording.Name;
				MacroRecording? obj = (from MacroRecording macro in MacroGraph.Instance.Vertices
					where string.Equals(macro.Name, mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
					select macro).FirstOrDefault();
				obj.Name = ((TextBox)mScriptName).Text.ToLower(CultureInfo.InvariantCulture).Trim();
				mRecording.Name = ((TextBox)mScriptName).Text.Trim();
				if (mRecording.PlayOnStart)
				{
					ParentWindow.mAutoRunMacro = mRecording;
				}
				string contents2 = JsonConvert.SerializeObject((object)mRecording, serializerSettings);
				File.WriteAllText(text, contents2);
				string destFileName = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, ((TextBox)mScriptName).Text.Trim() + ".json");
				File.Move(text, destFileName);
				foreach (MacroRecording item in ((BiDirectionalVertex<MacroRecording>)(object)obj).Parents.Cast<MacroRecording>())
				{
					foreach (MergedMacroConfiguration mergedMacroConfiguration in item.MergedMacroConfigurations)
					{
						List<string> list = new List<string>();
						foreach (string item2 in mergedMacroConfiguration.MacrosToRun.Select((string macroToRun) => (!string.Equals(oldMacroName, macroToRun, StringComparison.CurrentCultureIgnoreCase)) ? macroToRun : macroToRun.Replace(macroToRun, mRecording.Name)))
						{
							list.Add(item2);
						}
						mergedMacroConfiguration.MacrosToRun.Clear();
						foreach (string item3 in list)
						{
							mergedMacroConfiguration.MacrosToRun.Add(item3);
						}
					}
					CommonHandlers.SaveMacroJson(item, CommonHandlers.GetCompleteMacroRecordingPath(item.Name));
					CommonHandlers.OnMacroSettingChanged(item);
				}
				if (IsBookmarked)
				{
					DeleteScriptNameFromBookmarkedScriptListIfPresent(oldMacroName);
					AddScriptNameToBookmarkedScriptListIfNotPresent(mRecording.Name);
				}
			}
			CommonHandlers.OnMacroSettingChanged(mRecording);
			CommonHandlers.RefreshAllMacroRecorderWindow();
			CommonHandlers.ReloadMacroShortcutsForAllInstances();
		}
		catch (Exception ex)
		{
			Logger.Error("Error in saving macro settings: " + ex.ToString());
		}
	}

	private void NoSelection_MouseUp(object sender, MouseButtonEventArgs e)
	{
		((TextBox)mScriptName).SelectionLength = 0;
	}

	private bool Valid(Key key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)key == 13 || (int)key == 120 || (int)key == 121 || (int)key == 118 || (int)key == 119 || (int)key == 117 || (int)key == 116 || (int)key == 8 || (int)key == 6 || (int)key == 18 || (int)key == 32)
		{
			return false;
		}
		if (ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
		{
			string b = string.Format(CultureInfo.InvariantCulture, "{0} + {1} + {2}", new object[3]
			{
				IMAPKeys.GetStringForFile((Key)118),
				IMAPKeys.GetStringForFile((Key)120),
				IMAPKeys.GetStringForFile(key)
			});
			foreach (ShortcutKeys item in ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
			{
				if (string.Equals(item.ShortcutKey, b, StringComparison.InvariantCulture))
				{
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, isShowCloseImage: true);
					return false;
				}
			}
		}
		if (MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForFile(key)))
		{
			if (MainWindow.sMacroMapping[IMAPKeys.GetStringForFile(key)] != mRecording.Name)
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, isShowCloseImage: true);
				return false;
			}
			return true;
		}
		return true;
	}

	private void MacroShortcutPreviewKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		((TextBox)mMacroShortcutTextBox).Text = "";
		((FrameworkElement)mMacroShortcutTextBox).Tag = "";
		Key val = (((int)e.Key == 156) ? e.SystemKey : e.Key);
		if (IMAPKeys.mDictKeys.ContainsKey(val) && Valid(val))
		{
			((TextBox)mMacroShortcutTextBox).Text = IMAPKeys.GetStringForUI(val);
			((FrameworkElement)mMacroShortcutTextBox).Tag = IMAPKeys.GetStringForFile(val);
		}
		else
		{
			((TextBox)mMacroShortcutTextBox).Text = "";
			((FrameworkElement)mMacroShortcutTextBox).Tag = "";
		}
		((RoutedEventArgs)e).Handled = true;
		SaveScriptSettings();
	}

	private void ScriptName_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.Key == 6)
		{
			PerformSaveMacroNameOperations();
		}
	}

	private void ScriptName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		PerformSaveMacroNameOperations();
	}

	private void ScriptName_LostFocus(object sender, RoutedEventArgs e)
	{
		PerformSaveMacroNameOperations();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(mRecording.AuthorPageUrl.ToString());
		((RoutedEventArgs)e).Handled = true;
	}

	private void UserNameHyperlink_Loaded(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(mRecording.User))
		{
			((TextElementCollection<Inline>)(object)((Span)mUserNameHyperlink).Inlines).Clear();
			((Span)mUserNameHyperlink).Inlines.Add(mRecording.User);
			if (mRecording.AuthorPageUrl != null && !string.IsNullOrEmpty(mRecording.AuthorPageUrl.ToString()))
			{
				mUserNameHyperlink.NavigateUri = mRecording.AuthorPageUrl;
			}
			((Control)mScriptName).FontSize = 13.0;
			((UIElement)mUserNameTextblock).Visibility = (Visibility)0;
		}
	}

	private void CommunityMacroPage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(mRecording.MacroPageUrl.ToString());
	}

	private void ScriptControl_Loaded(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(mRecording.User))
		{
			((UIElement)mCommunityMacroImage).Visibility = (Visibility)0;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/singlemacrocontrol.xaml", UriKind.Relative);
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
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Expected O, but got Unknown
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Expected O, but got Unknown
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Expected O, but got Unknown
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(SingleMacroControl)target).MouseEnter += new MouseEventHandler(SingleMacroControl_MouseEnter);
			((UIElement)(SingleMacroControl)target).MouseLeave += new MouseEventHandler(SingleMacroControl_MouseLeave);
			((FrameworkElement)(SingleMacroControl)target).Loaded += new RoutedEventHandler(ScriptControl_Loaded);
			break;
		case 2:
			mGrid = (Grid)target;
			break;
		case 3:
			mBookmarkImg = (CustomPictureBox)target;
			((UIElement)mBookmarkImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BookMarkScriptImg_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mScriptNameGrid = (Grid)target;
			break;
		case 5:
			mScriptName = (CustomTextBox)target;
			((UIElement)mScriptName).PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(ScriptName_LostKeyboardFocus);
			((UIElement)mScriptName).LostFocus += new RoutedEventHandler(ScriptName_LostFocus);
			((UIElement)mScriptName).MouseLeftButtonUp += new MouseButtonEventHandler(NoSelection_MouseUp);
			((UIElement)mScriptName).KeyDown += new KeyEventHandler(ScriptName_KeyDown);
			break;
		case 6:
			mUserNameTextblock = (TextBlock)target;
			break;
		case 7:
			mUserNameHyperlink = (Hyperlink)target;
			mUserNameHyperlink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			((FrameworkContentElement)mUserNameHyperlink).Loaded += new RoutedEventHandler(UserNameHyperlink_Loaded);
			break;
		case 8:
			mEditNameImg = (CustomPictureBox)target;
			((UIElement)mEditNameImg).MouseLeftButtonDown += new MouseButtonEventHandler(EditMacroName_MouseDown);
			break;
		case 9:
			mTimestamp = (TextBlock)target;
			break;
		case 10:
			mPrefix = (TextBlock)target;
			break;
		case 11:
			mMacroShortcutTextBox = (CustomTextBox)target;
			((UIElement)mMacroShortcutTextBox).PreviewKeyDown += new KeyEventHandler(MacroShortcutPreviewKeyDown);
			break;
		case 12:
			mScriptPlayPanel = (StackPanel)target;
			break;
		case 13:
			mAutorunImage = (CustomPictureBox)target;
			((UIElement)mAutorunImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BookMarkScriptImg_PreviewMouseLeftButtonUp);
			break;
		case 14:
			mCommunityMacroImage = (CustomPictureBox)target;
			((UIElement)mCommunityMacroImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CommunityMacroPage_PreviewMouseLeftButtonUp);
			break;
		case 15:
			mPlayScriptImg = (CustomPictureBox)target;
			((UIElement)mPlayScriptImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PlayScriptImg_PreviewMouseLeftButtonUp);
			break;
		case 16:
			mScriptSettingsImg = (CustomPictureBox)target;
			((UIElement)mScriptSettingsImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScriptSettingsImg_PreviewMouseLeftButtonUp);
			break;
		case 17:
			mMergeScriptSettingsImg = (CustomPictureBox)target;
			((UIElement)mMergeScriptSettingsImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScriptSettingsImg_PreviewMouseLeftButtonUp);
			break;
		case 18:
			mDeleteScriptImg = (CustomPictureBox)target;
			((UIElement)mDeleteScriptImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DeleteScriptImg_PreviewMouseLeftButtonUp);
			break;
		case 19:
			mScriptRunningPanel = (StackPanel)target;
			break;
		case 20:
			mStopScriptImg = (CustomPictureBox)target;
			((UIElement)mStopScriptImg).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(StopScriptImg_PreviewMouseLeftButtonUp);
			break;
		case 21:
			mRunning = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
