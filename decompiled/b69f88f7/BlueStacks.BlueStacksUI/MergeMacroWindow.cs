using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MergeMacroWindow : CustomWindow, IComponentConnector
{
	private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();

	private MacroRecorderWindow mMacroRecorderWindow;

	private MainWindow ParentWindow;

	private MacroRecording mOriginalMacroRecording;

	internal int mAddedMacroTag;

	private MacroSettingsWindow mMacroSettingsWindow;

	private SingleMacroControl mSingleMacroControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMergeMacroWindowHeading;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mUnifyButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mCurrentMacroScripts;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Line mLineSeperator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMergedMacrosHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHelpCenterImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroAddedDragControl mMacroDragControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMergedMacrosFooter;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMacroNameStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox MacroName;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mMacroSettings;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mMergeButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mErrorNamePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mErrorText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mDownArrow;

	private bool _contentLoaded;

	internal MacroRecording MergedMacroRecording { get; set; }

	public MergeMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		InitializeComponent();
		mMacroRecorderWindow = window;
		ParentWindow = mainWindow;
	}

	internal void Init(MacroRecording mergedMacro = null, SingleMacroControl singleMacroControl = null)
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		try
		{
			((UIElement)mMacroNameStackPanel).Visibility = (Visibility)((mergedMacro != null) ? 2 : 0);
			mOriginalMacroRecording = mergedMacro;
			int num = 0;
			foreach (MacroRecording record in (from MacroRecording o in MacroGraph.Instance.Vertices
				orderby DateTime.ParseExact(o.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
				select o).ToList())
			{
				ParentWindow.mIsScriptsPresent = true;
				if (!record.Equals(mergedMacro) && (mergedMacro == null || !MacroGraph.Instance.DoesParentExist(MacroGraph.Instance.Vertices.Where((BiDirectionalVertex<MacroRecording> macro) => ((object)macro).Equals((object?)mergedMacro)).FirstOrDefault(), MacroGraph.Instance.Vertices.Where((BiDirectionalVertex<MacroRecording> macro) => ((object)macro).Equals((object?)record)).FirstOrDefault())))
				{
					MacroToAdd macroToAdd = new MacroToAdd(this, record.Name);
					if (num % 2 == 0)
					{
						BlueStacksUIBinding.BindColor((DependencyObject)(object)macroToAdd, Control.BackgroundProperty, "DarkBandingColor");
					}
					else
					{
						BlueStacksUIBinding.BindColor((DependencyObject)(object)macroToAdd, Control.BackgroundProperty, "LightBandingColor");
					}
					((Panel)mCurrentMacroScripts).Children.Add((UIElement)(object)macroToAdd);
					num++;
				}
			}
			if (singleMacroControl != null)
			{
				mSingleMacroControl = singleMacroControl;
			}
			if (mergedMacro == null)
			{
				string timeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
				MergedMacroRecording = new MacroRecording
				{
					Name = CommonHandlers.GetMacroName(),
					TimeCreated = timeCreated,
					MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>()
				};
				((UIElement)mUnifyButton).Visibility = (Visibility)2;
				BlueStacksUIBinding.Bind((Button)(object)mMergeButton, "STRING_MERGE");
				BlueStacksUIBinding.Bind(mMergeMacroWindowHeading, "STRING_MERGE_MACROS", "");
			}
			else
			{
				MergedMacroRecording = UsefulExtensionMethod.DeepCopy<MacroRecording>(mergedMacro);
				BlueStacksUIBinding.Bind((Button)(object)mMergeButton, "STRING_UPDATE_SETTING");
				BlueStacksUIBinding.Bind(mMergeMacroWindowHeading, "STRING_EDIT_MERGED_MACRO", "");
				((UIElement)mUnifyButton).Visibility = (Visibility)0;
			}
			((TextBox)MacroName).Text = MergedMacroRecording.Name;
			mMacroDragControl.Init();
			MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= Items_CollectionChanged;
			MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += Items_CollectionChanged;
			Items_CollectionChanged(null, null);
			DataModificationTracker.Lock((object)mOriginalMacroRecording, new List<string> { "IsGroupButtonVisible", "IsUnGroupButtonVisible", "IsSettingsVisible", "IsFirstListBoxItem", "IsLastListBoxItem", "Parents", "Childs", "IsVisited" }, true);
			CheckIfCanSave();
		}
		catch (Exception ex)
		{
			Logger.Error("Error in export window init err: " + ex.ToString());
		}
	}

	private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		if (args != null)
		{
			if (args.OldItems != null)
			{
				foreach (INotifyPropertyChanged oldItem in args.OldItems)
				{
					oldItem.PropertyChanged -= Item_PropertyChanged;
				}
			}
			if (args.NewItems != null)
			{
				foreach (INotifyPropertyChanged newItem in args.NewItems)
				{
					newItem.PropertyChanged += Item_PropertyChanged;
				}
			}
		}
		else
		{
			foreach (MergedMacroConfiguration mergedMacroConfiguration in MergedMacroRecording.MergedMacroConfigurations)
			{
				mergedMacroConfiguration.PropertyChanged += Item_PropertyChanged;
			}
		}
		foreach (MergedMacroConfiguration mergedMacroConfiguration2 in MergedMacroRecording.MergedMacroConfigurations)
		{
			mergedMacroConfiguration2.IsGroupButtonVisible = true;
			mergedMacroConfiguration2.IsFirstListBoxItem = false;
			mergedMacroConfiguration2.IsLastListBoxItem = false;
			mergedMacroConfiguration2.IsUnGroupButtonVisible = mergedMacroConfiguration2.MacrosToRun.Count > 1;
		}
		if (MergedMacroRecording.MergedMacroConfigurations.Count > 0)
		{
			MergedMacroRecording.MergedMacroConfigurations[0].IsGroupButtonVisible = false;
			MergedMacroRecording.MergedMacroConfigurations[0].IsFirstListBoxItem = true;
			MergedMacroRecording.MergedMacroConfigurations[MergedMacroRecording.MergedMacroConfigurations.Count - 1].IsLastListBoxItem = true;
			MergedMacroRecording.MergedMacroConfigurations[MergedMacroRecording.MergedMacroConfigurations.Count - 1].DelayNextScript = 0;
			((UIElement)mMergedMacrosHeader).Visibility = (Visibility)0;
			((UIElement)mHelpCenterImage).Visibility = (Visibility)0;
			((UIElement)mMergedMacrosFooter).IsEnabled = true;
		}
		else
		{
			((UIElement)mMergedMacrosHeader).Visibility = (Visibility)2;
			((UIElement)mHelpCenterImage).Visibility = (Visibility)2;
			((UIElement)mMergedMacrosFooter).IsEnabled = false;
		}
		CheckIfCanSave();
	}

	private void Item_PropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		CheckIfCanSave();
	}

	private void CheckIfCanSave()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		bool flag = MergedMacroRecording.MergedMacroConfigurations.Count > 0 && (MergedMacroRecording.MergedMacroConfigurations.Count > 1 || MergedMacroRecording.MergedMacroConfigurations[0].MacrosToRun.Count > 1);
		((UIElement)mMergeButton).IsEnabled = ((int)((UIElement)mMacroNameStackPanel).Visibility == 2 || (int)((XTextBox)MacroName).InputTextValidity == 1) && flag && MergedMacroRecording.MergedMacroConfigurations.All((MergedMacroConfiguration macro) => macro.LoopCount > 0) && DataModificationTracker.HasChanged((object)MergedMacroRecording);
		((UIElement)mUnifyButton).IsEnabled = flag;
		((UIElement)mMacroSettings).IsEnabled = flag;
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_close", null, null);
		CloseWindow();
	}

	private void CloseWindow()
	{
		((Window)this).Close();
		mMacroRecorderWindow.mMergeMacroWindow = null;
		((UIElement)mMacroRecorderWindow.mOverlayGrid).Visibility = (Visibility)2;
	}

	private void MergeButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (mOriginalMacroRecording == null)
		{
			mOriginalMacroRecording = new MacroRecording();
		}
		mOriginalMacroRecording.CopyFrom(MergedMacroRecording);
		mMacroRecorderWindow.SaveMacroRecord(mOriginalMacroRecording);
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_success", null, null);
		CloseWindow();
	}

	private void MacroName_TextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Invalid comparison between Unknown and I4
		if ((int)((UIElement)mMacroNameStackPanel).Visibility == 0)
		{
			if (string.IsNullOrEmpty(((TextBox)MacroName).Text.Trim()))
			{
				mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", "");
				((XTextBox)MacroName).InputTextValidity = (TextValidityOptions)(-1);
			}
			else if (((TextBox)MacroName).Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				mErrorText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[3]
				{
					LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
					Environment.NewLine,
					"\\ / : * ? \" < > |"
				});
				((XTextBox)MacroName).InputTextValidity = (TextValidityOptions)(-1);
			}
			else if (Enumerable.Contains(Constants.ReservedFileNamesList, ((TextBox)MacroName).Text.Trim().ToLower(CultureInfo.InvariantCulture)))
			{
				mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
				((XTextBox)MacroName).InputTextValidity = (TextValidityOptions)(-1);
			}
			else if (MacroGraph.Instance.Vertices.Cast<MacroRecording>().Any((MacroRecording macro) => string.Equals(macro.Name, ((TextBox)MacroName).Text.Trim(), StringComparison.InvariantCultureIgnoreCase)))
			{
				mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", "");
				((XTextBox)MacroName).InputTextValidity = (TextValidityOptions)(-1);
			}
			else
			{
				((XTextBox)MacroName).InputTextValidity = (TextValidityOptions)1;
			}
			((Popup)mErrorNamePopup).IsOpen = (int)((XTextBox)MacroName).InputTextValidity == -1;
			MergedMacroRecording.Name = ((TextBox)MacroName).Text;
			CheckIfCanSave();
		}
	}

	private void MacroSettings_Click(object sender, RoutedEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_settings", null, null);
		if (mMacroSettingsWindow == null || ((CustomWindow)mMacroSettingsWindow).IsClosed)
		{
			mMacroSettingsWindow = new MacroSettingsWindow(ParentWindow, MergedMacroRecording, mMacroRecorderWindow);
			((Window)mMacroSettingsWindow).Closed += delegate
			{
				CheckIfCanSave();
			};
		}
		((Window)mMacroSettingsWindow).ShowDialog();
	}

	private void UnifyButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (mOriginalMacroRecording == null)
		{
			mOriginalMacroRecording = new MacroRecording();
		}
		mOriginalMacroRecording.CopyFrom(MergedMacroRecording);
		CustomMessageWindow val = new CustomMessageWindow();
		val.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNIFY_0", ""), new object[1] { mOriginalMacroRecording.Name });
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
		bool closeWindow = false;
		val.AddButton((ButtonColors)4, string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), new object[1] { "" }).Trim(), (EventHandler)delegate
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify", null, null);
			mMacroRecorderWindow.FlattenRecording(mOriginalMacroRecording);
			CommonHandlers.SaveMacroJson(mOriginalMacroRecording, mOriginalMacroRecording.Name + ".json");
			CommonHandlers.RefreshAllMacroRecorderWindow();
			closeWindow = true;
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)delegate
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", null, null);
		}, (string)null, false, (object)null);
		val.CloseButtonHandle((EventHandler)delegate
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", null, null);
		}, (object)null);
		((Window)val).Owner = (Window)(object)this;
		((Window)val).ShowDialog();
		if (closeWindow)
		{
			CloseWindow();
		}
	}

	private void MacroName_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((XTextBox)MacroName).InputTextValidity == -1)
		{
			((Popup)mErrorNamePopup).IsOpen = true;
			((Popup)mErrorNamePopup).StaysOpen = true;
		}
		else
		{
			((Popup)mErrorNamePopup).IsOpen = false;
		}
	}

	private void MacroName_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mErrorNamePopup).IsOpen = false;
	}

	private void mHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
	{
		Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=MergeMacro_Help");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/mergemacrowindow.xaml", UriKind.Relative);
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			mMergeMacroWindowHeading = (TextBlock)target;
			break;
		case 3:
			mUnifyButton = (CustomButton)target;
			((ButtonBase)mUnifyButton).Click += new RoutedEventHandler(UnifyButton_Click);
			break;
		case 4:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 5:
			mCurrentMacroScripts = (StackPanel)target;
			break;
		case 6:
			mLineSeperator = (Line)target;
			break;
		case 7:
			mMergedMacrosHeader = (TextBlock)target;
			break;
		case 8:
			mHelpCenterImage = (CustomPictureBox)target;
			((UIElement)mHelpCenterImage).MouseDown += new MouseButtonEventHandler(mHelpCenterImage_MouseDown);
			break;
		case 9:
			mMacroDragControl = (MacroAddedDragControl)target;
			break;
		case 10:
			mMergedMacrosFooter = (StackPanel)target;
			break;
		case 11:
			mMacroNameStackPanel = (StackPanel)target;
			break;
		case 12:
			MacroName = (CustomTextBox)target;
			((UIElement)MacroName).MouseEnter += new MouseEventHandler(MacroName_MouseEnter);
			((UIElement)MacroName).MouseLeave += new MouseEventHandler(MacroName_MouseLeave);
			((TextBoxBase)MacroName).TextChanged += new TextChangedEventHandler(MacroName_TextChanged);
			break;
		case 13:
			mMacroSettings = (CustomButton)target;
			((ButtonBase)mMacroSettings).Click += new RoutedEventHandler(MacroSettings_Click);
			break;
		case 14:
			mMergeButton = (CustomButton)target;
			((ButtonBase)mMergeButton).Click += new RoutedEventHandler(MergeButton_Click);
			break;
		case 15:
			mErrorNamePopup = (CustomPopUp)target;
			break;
		case 16:
			mMaskBorder1 = (Border)target;
			break;
		case 17:
			mErrorText = (TextBlock)target;
			break;
		case 18:
			mDownArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
