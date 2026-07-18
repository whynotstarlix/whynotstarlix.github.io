using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class ImportMacroWindow : CustomWindow, IComponentConnector
{
	private MacroRecorderWindow mOperationWindow;

	private MainWindow ParentWindow;

	internal StackPanel mScriptsStackPanel;

	internal int mNumberOfFilesSelectedForImport;

	private Dictionary<ImportMacroScriptsControl, MacroRecording> mBoxToRecordingDict = new Dictionary<ImportMacroScriptsControl, MacroRecording>();

	private Dictionary<CustomTextBox, MacroRecording> mDependentRecordingDict = new Dictionary<CustomTextBox, MacroRecording>();

	private bool mInited;

	internal bool mIsInDependentFileFindingMode;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScriptsListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSelectAllBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mImportBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	private bool _contentLoaded;

	public ImportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
	{
		InitializeComponent();
		mOperationWindow = window;
		ParentWindow = mainWindow;
		object content = ((ContentControl)mScriptsListScrollbar).Content;
		mScriptsStackPanel = (StackPanel)((content is StackPanel) ? content : null);
	}

	internal void TextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (!mInited)
		{
			return;
		}
		ImportMacroScriptsControl scriptControlFromMacroItemGrandchild = GetScriptControlFromMacroItemGrandchild(((FrameworkElement)((sender is FrameworkElement) ? sender : null)).Parent);
		string text = ((TextBox)((sender is CustomTextBox) ? sender : null)).Text;
		foreach (UIElement child in ((Panel)scriptControlFromMacroItemGrandchild.mDependentScriptsPanel).Children)
		{
			CustomTextBox val2 = (CustomTextBox)((child is CustomTextBox) ? child : null);
			((TextBox)val2).Text = MacroRecorderWindow.GetDependentRecordingName(text, mDependentRecordingDict[val2].Name);
		}
	}

	private void ImportMacroWindow_Closing(object sender, CancelEventArgs e)
	{
		CloseWindow();
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Window)this).Close();
	}

	private void CloseWindow()
	{
		mOperationWindow.mImportMacroWindow = null;
		((UIElement)mOperationWindow.mOverlayGrid).Visibility = (Visibility)1;
		((UIElement)mOperationWindow).Focus();
	}

	private ImportMacroScriptsControl AddRecordingToStackPanelAndDict(MacroRecording record, bool isSingleRecording, out string suggestedName)
	{
		ImportMacroScriptsControl importMacroScriptsControl = new ImportMacroScriptsControl(this, ParentWindow);
		importMacroScriptsControl.Init(record.Name, isSingleRecording);
		suggestedName = ((!(from MacroRecording macro in MacroGraph.Instance.Vertices
			select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(record.Name.ToLower(CultureInfo.InvariantCulture).Trim())) ? record.Name : CommonHandlers.GetMacroName(record.Name));
		((TextBox)importMacroScriptsControl.mImportName).Text = ValidateSuggestedName(suggestedName);
		((Panel)mScriptsStackPanel).Children.Add((UIElement)(object)importMacroScriptsControl);
		mBoxToRecordingDict[importMacroScriptsControl] = record;
		return importMacroScriptsControl;
	}

	internal void Init()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		bool flag = ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Count == 1;
		try
		{
			mInited = false;
			((Panel)mScriptsStackPanel).Children.Clear();
			foreach (MacroRecording mRenamingMacros in ParentWindow.MacroRecorderWindow.mRenamingMacrosList)
			{
				string suggestedName;
				ImportMacroScriptsControl importMacroScriptsControl = AddRecordingToStackPanelAndDict(mRenamingMacros, flag, out suggestedName);
				if ((int)mRenamingMacros.RecordingType != 1 || false != mOperationWindow.mImportMultiMacroAsUnified)
				{
					continue;
				}
				((UIElement)importMacroScriptsControl.mDependentScriptsMsg).Visibility = (Visibility)0;
				((UIElement)importMacroScriptsControl.mDependentScriptsPanel).Visibility = (Visibility)0;
				((Panel)importMacroScriptsControl.mDependentScriptsPanel).Children.Clear();
				foreach (string sourceRecording in mRenamingMacros.SourceRecordings)
				{
					MacroRecording val = JsonConvert.DeserializeObject<MacroRecording>(sourceRecording, Utils.GetSerializerSettings());
					string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(suggestedName, val.Name);
					string suggestedName2 = ((!(from MacroRecording macro in MacroGraph.Instance.Vertices
						select macro.Name).Contains(dependentRecordingName.ToLower(CultureInfo.InvariantCulture).Trim())) ? dependentRecordingName : CommonHandlers.GetMacroName(dependentRecordingName));
					CustomTextBox val2 = new CustomTextBox
					{
						Height = 24.0,
						HorizontalAlignment = (HorizontalAlignment)0,
						Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
						Text = ValidateSuggestedName(suggestedName2),
						Visibility = (Visibility)0,
						IsEnabled = false
					};
					((Panel)importMacroScriptsControl.mDependentScriptsPanel).Children.Add((UIElement)(object)val2);
					mDependentRecordingDict[val2] = val;
				}
			}
			mNumberOfFilesSelectedForImport = 0;
		}
		catch (Exception ex)
		{
			Logger.Error("Error in import window init err: " + ex.ToString());
		}
		mInited = true;
		if (flag)
		{
			((UIElement)mSelectAllBtn).Visibility = (Visibility)1;
		}
		((ToggleButton)mSelectAllBtn).IsChecked = true;
		SelectAllBtn_Click(null, null);
	}

	private string ValidateSuggestedName(string suggestedName)
	{
		if (mBoxToRecordingDict.Keys.Any((ImportMacroScriptsControl box) => string.Equals(((TextBox)box.mImportName).Text.Trim(), suggestedName, StringComparison.InvariantCultureIgnoreCase)))
		{
			int num = suggestedName.LastIndexOf('(') + 1;
			int num2 = suggestedName.LastIndexOf(')');
			if (int.TryParse(suggestedName.Substring(num, num2 - num), out var result))
			{
				suggestedName = suggestedName.Remove(num, num2 - num).Insert(num, (result + 1).ToString(CultureInfo.InvariantCulture));
				return ValidateSuggestedName(suggestedName);
			}
			Logger.Error("Error in ValidateSuggestedName: Could not get integer part in suggested name '{0}'", new object[1] { suggestedName });
		}
		return suggestedName;
	}

	private bool CheckIfEditedMacroNameIsAllowed(string text, ImportMacroScriptsControl item)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(text.Trim()))
		{
			BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), "");
			return false;
		}
		foreach (MacroRecording vertex in MacroGraph.Instance.Vertices)
		{
			if (vertex.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				return false;
			}
		}
		foreach (ImportMacroScriptsControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if (item != child && ((ToggleButton)child.mContent).IsChecked == true && child.IsScriptInRenameMode() && ((TextBox)child.mImportName).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				return false;
			}
		}
		return true;
	}

	private bool IsMacroItemDependentOfParent(ImportMacroScriptsControl item, string name)
	{
		if (((FrameworkElement)item).Tag != null)
		{
			return ((FrameworkElement)item).Tag.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase);
		}
		return false;
	}

	private ImportMacroScriptsControl GetScriptControlFromMacroItemGrandchild(object grandchild)
	{
		while (grandchild != null)
		{
			DependencyObject parent = ((FrameworkElement)((grandchild is FrameworkElement) ? grandchild : null)).Parent;
			if (parent != null && parent is ImportMacroScriptsControl)
			{
				return parent as ImportMacroScriptsControl;
			}
			grandchild = parent;
		}
		return null;
	}

	internal void Box_Unchecked(object sender, RoutedEventArgs e)
	{
		if (!mIsInDependentFileFindingMode)
		{
			mIsInDependentFileFindingMode = true;
			mNumberOfFilesSelectedForImport--;
			if (mNumberOfFilesSelectedForImport == 0)
			{
				((UIElement)mImportBtn).IsEnabled = false;
			}
			if (mNumberOfFilesSelectedForImport < ((Panel)mScriptsStackPanel).Children.Count)
			{
				((ToggleButton)mSelectAllBtn).IsChecked = false;
			}
			mIsInDependentFileFindingMode = false;
		}
	}

	internal void Box_Checked(object sender, RoutedEventArgs e)
	{
		if (!mIsInDependentFileFindingMode)
		{
			mIsInDependentFileFindingMode = true;
			mNumberOfFilesSelectedForImport++;
			if (mNumberOfFilesSelectedForImport > 0)
			{
				((UIElement)mImportBtn).IsEnabled = true;
			}
			if (mNumberOfFilesSelectedForImport == ((Panel)mScriptsStackPanel).Children.Count)
			{
				((ToggleButton)mSelectAllBtn).IsChecked = true;
			}
			mIsInDependentFileFindingMode = false;
		}
	}

	private static List<MacroEvents> GetRecordingEventsFromSourceRecording(MacroRecording srcRecording, double acceleration, long initialTime, ref long elapsedTime)
	{
		if (srcRecording == null)
		{
			throw new Exception("Source recording now found in multiMacro");
		}
		List<MacroEvents> result = new List<MacroEvents>();
		foreach (MacroEvents @event in srcRecording.Events)
		{
			MacroEvents val = @event;
			val.Timestamp = (long)Math.Floor((double)@event.Timestamp / acceleration);
			val.Timestamp += initialTime;
			elapsedTime = val.Timestamp;
		}
		return result;
	}

	private ImportMacroScriptsControl GetMacroItemFromTag(string tag)
	{
		foreach (ImportMacroScriptsControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if (mBoxToRecordingDict[child].Name == tag)
			{
				return child;
			}
		}
		return null;
	}

	private void ImportBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Invalid comparison between Unknown and I4
		int num = 0;
		bool flag = false;
		bool flag2 = true;
		List<MacroRecording> list = new List<MacroRecording>();
		foreach (ImportMacroScriptsControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if (((ToggleButton)child.mContent).IsChecked == true)
			{
				if (((TextBox)child.mImportName).Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
					{
						LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
						Environment.NewLine,
						"\\ / : * ? \" < > |"
					});
					BlueStacksUIBinding.Bind(child.mWarningMsg, text, "");
					((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)(-1);
					if (((UIElement)child.mImportName).IsEnabled)
					{
						((UIElement)child.mWarningMsg).Visibility = (Visibility)0;
					}
					flag2 = false;
				}
				else if (Enumerable.Contains(Constants.ReservedFileNamesList, ((TextBox)child.mImportName).Text.Trim().ToLower(CultureInfo.InvariantCulture)))
				{
					BlueStacksUIBinding.Bind(child.mWarningMsg, "STRING_MACRO_FILE_NAME_ERROR", "");
					((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)(-1);
					if (((UIElement)child.mImportName).IsEnabled)
					{
						((UIElement)child.mWarningMsg).Visibility = (Visibility)0;
					}
					flag2 = false;
				}
				else if (!CheckIfEditedMacroNameIsAllowed(((TextBox)child.mImportName).Text, child) && child.IsScriptInRenameMode())
				{
					if (!string.IsNullOrEmpty(((TextBox)child.mImportName).Text.Trim()))
					{
						BlueStacksUIBinding.Bind(child.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", ""), "");
					}
					((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)(-1);
					if (((UIElement)child.mImportName).IsEnabled)
					{
						((UIElement)child.mWarningMsg).Visibility = (Visibility)0;
					}
					flag2 = false;
				}
				else if ((int)((UIElement)child.mDependentScriptsPanel).Visibility == 0 && ((Panel)child.mDependentScriptsPanel).Children.Count > 0)
				{
					string text2 = CheckIfDependentScriptsHaveInvalidName(child);
					if (text2 != "TEXT_VALID")
					{
						BlueStacksUIBinding.Bind(child.mWarningMsg, text2, "");
						((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)(-1);
						flag2 = false;
					}
					else
					{
						((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)1;
						((UIElement)child.mWarningMsg).Visibility = (Visibility)2;
					}
				}
				else
				{
					((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)1;
					((UIElement)child.mWarningMsg).Visibility = (Visibility)2;
				}
				flag = true;
			}
			num++;
		}
		if (!flag)
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_IMPORT_MACRO_SELECTED", ""), 4.0, isShowCloseImage: true);
		}
		else
		{
			if (!flag2)
			{
				return;
			}
			if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
			}
			foreach (ImportMacroScriptsControl child2 in ((Panel)mScriptsStackPanel).Children)
			{
				if (((ToggleButton)child2.mContent).IsChecked != true)
				{
					continue;
				}
				MacroRecording val = mBoxToRecordingDict[child2];
				string newScript = ((((ToggleButton)child2.mReplaceExistingBtn).IsChecked.HasValue && ((ToggleButton)child2.mReplaceExistingBtn).IsChecked.Value) ? ((ContentControl)child2.mContent).Content.ToString() : ((TextBox)child2.mImportName).Text.Trim());
				MacroRecording existingMacro = (from MacroRecording m in MacroGraph.Instance.Vertices
					where string.Equals(m.Name, newScript, StringComparison.InvariantCultureIgnoreCase)
					select m).FirstOrDefault();
				if (existingMacro != null)
				{
					if (((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents.Count > 0)
					{
						int index;
						for (index = ((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents.Count - 1; index >= 0; index--)
						{
							MacroRecording val2 = (from MacroRecording macro in MacroGraph.Instance.Vertices
								where ((object)macro).Equals((object?)((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents[index])
								select macro).FirstOrDefault();
							MacroRecorderWindow macroRecorderWindow = mOperationWindow;
							BiDirectionalVertex<MacroRecording> obj = ((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents[index];
							macroRecorderWindow.FlattenRecording((MacroRecording)(object)((obj is MacroRecording) ? obj : null));
							BiDirectionalVertex<MacroRecording> obj2 = ((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents[index];
							BiDirectionalVertex<MacroRecording> record = ((obj2 is MacroRecording) ? obj2 : null);
							BiDirectionalVertex<MacroRecording> obj3 = ((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents[index];
							CommonHandlers.SaveMacroJson((MacroRecording)(object)record, ((MacroRecording)((obj3 is MacroRecording) ? obj3 : null)).Name + ".json");
							foreach (SingleMacroControl child3 in ((Panel)mOperationWindow.mScriptsStackPanel).Children)
							{
								if (child3.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == val2.Name.ToLower(CultureInfo.InvariantCulture).Trim())
								{
									child3.mScriptSettingsImg.ImageName = "macro_settings";
								}
							}
							BiDirectionalGraph<MacroRecording> instance = MacroGraph.Instance;
							BiDirectionalVertex<MacroRecording> obj4 = ((BiDirectionalVertex<MacroRecording>)(object)existingMacro).Parents[index];
							instance.DeLinkMacroChild((obj4 is MacroRecording) ? obj4 : null);
						}
					}
					DeleteMacroScript(existingMacro);
				}
				val.Name = newScript;
				if ((int)val.RecordingType == 1)
				{
					mOperationWindow.ImportMultiMacro(val, mOperationWindow.mImportMultiMacroAsUnified.Value, list, GetDictionaryOfNewNamesForDependentRecordings(val.Name));
					continue;
				}
				CommonHandlers.SaveMacroJson(val, val.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json");
				MacroGraph.Instance.AddVertex((BiDirectionalVertex<MacroRecording>)(object)val);
				list.Add(val);
			}
			foreach (MacroRecording item in list)
			{
				MacroGraph.LinkMacroChilds(item);
			}
			mOperationWindow.mNewlyAddedMacrosList.AddRange(list);
			ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
			((Window)this).Close();
		}
	}

	private void DeleteMacroScript(MacroRecording mRecording)
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
		CommonHandlers.OnMacroDeleted(mRecording.Name);
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

	private string CheckIfDependentScriptsHaveInvalidName(ImportMacroScriptsControl scriptControl)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected O, but got Unknown
		string result = "TEXT_VALID";
		foreach (UIElement child in ((Panel)scriptControl.mDependentScriptsPanel).Children)
		{
			CustomTextBox val2 = (CustomTextBox)((child is CustomTextBox) ? child : null);
			string text = ((TextBox)val2).Text;
			if (text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				((XTextBox)val2).InputTextValidity = (TextValidityOptions)(-1);
				result = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
				{
					LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
					Environment.NewLine,
					"\\ / : * ? \" < > |"
				});
			}
			else if (Enumerable.Contains(Constants.ReservedFileNamesList, text.Trim().ToLower(CultureInfo.InvariantCulture)))
			{
				((XTextBox)val2).InputTextValidity = (TextValidityOptions)(-1);
				result = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
			}
			else
			{
				if (!scriptControl.IsScriptInRenameMode())
				{
					continue;
				}
				foreach (MacroRecording vertex in MacroGraph.Instance.Vertices)
				{
					if (vertex.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						((XTextBox)val2).InputTextValidity = (TextValidityOptions)(-1);
						return LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
					}
				}
				foreach (ImportMacroScriptsControl child2 in ((Panel)mScriptsStackPanel).Children)
				{
					if (child2 == scriptControl || !scriptControl.IsScriptInRenameMode() || ((ToggleButton)child2.mContent).IsChecked != true)
					{
						continue;
					}
					if (((TextBox)child2.mImportName).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						((XTextBox)val2).InputTextValidity = (TextValidityOptions)(-1);
						result = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
						continue;
					}
					foreach (UIElement child3 in ((Panel)child2.mDependentScriptsPanel).Children)
					{
						if (((TextBox)((child3 is CustomTextBox) ? child3 : null)).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
						{
							((XTextBox)val2).InputTextValidity = (TextValidityOptions)(-1);
							result = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
							break;
						}
					}
				}
			}
		}
		return result;
	}

	private Dictionary<string, string> GetDictionaryOfNewNamesForDependentRecordings(string parentMacroName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (ImportMacroScriptsControl child in ((Panel)mScriptsStackPanel).Children)
		{
			if ((((FrameworkElement)child).Tag != null && ((FrameworkElement)child).Tag.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase)) || (((ContentControl)child.mContent).Content.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase) && ((ToggleButton)child.mReplaceExistingBtn).IsChecked != true))
			{
				dictionary.Add(((ContentControl)child.mContent).Content.ToString(), ((TextBox)child.mImportName).Text);
			}
		}
		return dictionary;
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
				((UIElement)mLoadingGrid).Visibility = (Visibility)1;
			}
		}, new object[0]);
	}

	private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mSelectAllBtn).IsChecked.Value)
		{
			foreach (object child in ((Panel)mScriptsStackPanel).Children)
			{
				((ToggleButton)(child as ImportMacroScriptsControl).mContent).IsChecked = true;
			}
			return;
		}
		foreach (object child2 in ((Panel)mScriptsStackPanel).Children)
		{
			((ToggleButton)(child2 as ImportMacroScriptsControl).mContent).IsChecked = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/importmacrowindow.xaml", UriKind.Relative);
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((Window)(ImportMacroWindow)target).Closing += ImportMacroWindow_Closing;
			break;
		case 2:
			mMaskBorder = (Border)target;
			break;
		case 3:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 4:
			mScriptsListScrollbar = (ScrollViewer)target;
			break;
		case 5:
			mSelectAllBtn = (CustomCheckbox)target;
			((ButtonBase)mSelectAllBtn).Click += new RoutedEventHandler(SelectAllBtn_Click);
			break;
		case 6:
			mImportBtn = (CustomButton)target;
			((ButtonBase)mImportBtn).Click += new RoutedEventHandler(ImportBtn_Click);
			break;
		case 7:
			mLoadingGrid = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
