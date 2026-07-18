using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MacroSettingsWindow : CustomWindow, IComponentConnector
{
	private MainWindow ParentWindow;

	private MacroRecorderWindow mMacroRecorderWindow;

	private MacroRecording mRecording;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSettingsHeaderText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mRepeactActionPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mRepeatActionInSession;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mRepeatActionInSessionGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopCountTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mErrorNamePopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mErrorText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mDownArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mRepeatActionTimePanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mRepeatActionTime;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mRepeatActionTimePanelGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopHours;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopMinutes;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopSeconds;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mRepeatSessionInfinitePanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mRepeatSessionInfinite;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopIntervalMinsTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mLoopIntervalSecondsTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mAccelerationCombobox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mPlayOnStartCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mRestartPlayerCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mRestartPlayerIntervalTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mRestartTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mDonotShowWindowOnFinishCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSaveButton;

	private bool _contentLoaded;

	internal MacroSettingsWindow(MainWindow window, MacroRecording record, MacroRecorderWindow singleMacroControl)
	{
		InitializeComponent();
		ParentWindow = window;
		((Window)this).Owner = (Window)(object)ParentWindow;
		mMacroRecorderWindow = singleMacroControl;
		mRecording = record;
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopCountTextBox, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopHours, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopMinutes, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopSeconds, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopIntervalMinsTextBox, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mRestartPlayerIntervalTextBox, false);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)mLoopIntervalSecondsTextBox, false);
		InitSettings();
	}

	private void InitSettings()
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Expected O, but got Unknown
		mSettingsHeaderText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2]
		{
			mRecording.Name,
			LocaleStrings.GetLocalizedString("STRING_SETTINGS", "").ToLower(CultureInfo.InvariantCulture)
		});
		((TextBox)mLoopCountTextBox).Text = mRecording.LoopNumber.ToString(CultureInfo.InvariantCulture);
		((TextBox)mLoopHours).Text = (mRecording.LoopTime / 3600).ToString(CultureInfo.InvariantCulture);
		((TextBox)mLoopMinutes).Text = (mRecording.LoopTime / 60 % 60).ToString(CultureInfo.InvariantCulture);
		((TextBox)mLoopSeconds).Text = (mRecording.LoopTime % 60).ToString(CultureInfo.InvariantCulture);
		((TextBox)mLoopIntervalMinsTextBox).Text = (mRecording.LoopInterval / 60).ToString(CultureInfo.InvariantCulture);
		((TextBox)mLoopIntervalSecondsTextBox).Text = (mRecording.LoopInterval % 60).ToString(CultureInfo.InvariantCulture);
		((ToggleButton)mRestartPlayerCheckBox).IsChecked = mRecording.RestartPlayer;
		((ToggleButton)mPlayOnStartCheckBox).IsChecked = mRecording.PlayOnStart;
		((ToggleButton)mDonotShowWindowOnFinishCheckBox).IsChecked = mRecording.DonotShowWindowOnFinish;
		((TextBox)mRestartPlayerIntervalTextBox).Text = mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture);
		((ItemsControl)mAccelerationCombobox).Items.Clear();
		for (int i = 0; i <= 8; i++)
		{
			ComboBoxItem val = new ComboBoxItem();
			((ContentControl)val).Content = ((double)(i + 2) * 0.5).ToString(CultureInfo.InvariantCulture) + "x";
			((ItemsControl)mAccelerationCombobox).Items.Add((object)val);
		}
		if (mRecording.Acceleration == 0.0)
		{
			((Selector)mAccelerationCombobox).SelectedIndex = 0;
		}
		else
		{
			((Selector)mAccelerationCombobox).SelectedIndex = (int)(mRecording.Acceleration / 0.5 - 2.0);
		}
		((FrameworkElement)mRestartTextBlock).ToolTip = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_AFTER", "") + " " + ((TextBox)mRestartPlayerIntervalTextBox).Text + " " + LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", ""), new object[0]);
		SelectRepeatExecutionSetting();
		((TextBoxBase)mLoopCountTextBox).TextChanged += new TextChangedEventHandler(LoopCountTextBox_TextChanged);
	}

	private void SelectRepeatExecutionSetting()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected I4, but got Unknown
		OperationsLoopType loopType = mRecording.LoopType;
		switch ((int)loopType)
		{
		case 0:
			((UIElement)mRepeatActionTimePanelGrid).IsEnabled = false;
			((ToggleButton)mRepeatActionInSession).IsChecked = true;
			break;
		case 1:
			((UIElement)mRepeatActionInSessionGrid).IsEnabled = false;
			((ToggleButton)mRepeatActionTime).IsChecked = true;
			break;
		case 2:
			((UIElement)mRepeatActionTimePanelGrid).IsEnabled = false;
			((UIElement)mRepeatActionInSessionGrid).IsEnabled = false;
			((ToggleButton)mRepeatSessionInfinite).IsChecked = true;
			break;
		}
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (IsMacroSettingsChanged())
		{
			((Window)GetUnsavedChangesWindow()).ShowDialog();
		}
		CloseWindow();
	}

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(((TextBox)mLoopHours).Text))
		{
			((TextBox)mLoopHours).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mLoopMinutes).Text))
		{
			((TextBox)mLoopMinutes).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mLoopSeconds).Text))
		{
			((TextBox)mLoopSeconds).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mLoopCountTextBox).Text))
		{
			((TextBox)mLoopCountTextBox).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mLoopIntervalMinsTextBox).Text))
		{
			((TextBox)mLoopIntervalMinsTextBox).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mLoopIntervalSecondsTextBox).Text))
		{
			((TextBox)mLoopIntervalSecondsTextBox).Text = "0";
		}
		if (string.IsNullOrEmpty(((TextBox)mRestartPlayerIntervalTextBox).Text))
		{
			((TextBox)mRestartPlayerIntervalTextBox).Text = "0";
		}
		bool flag = IsMacroSettingsChanged();
		if (!string.IsNullOrEmpty(((TextBox)mRestartPlayerIntervalTextBox).Text) && int.Parse(((TextBox)mRestartPlayerIntervalTextBox).Text, CultureInfo.InvariantCulture) > 0)
		{
			if (flag)
			{
				SaveScriptSettings();
				if (sender != null)
				{
					ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, isShowCloseImage: true);
				}
			}
			else
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_CHANGES_SAVE", ""), 4.0, isShowCloseImage: true);
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_MACRO_RESTART_INTERVAL_NULL", ""), 4.0, isShowCloseImage: true);
			((TextBox)mRestartPlayerIntervalTextBox).Text = mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture);
		}
	}

	private CustomMessageWindow GetUnsavedChangesWindow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		CustomMessageWindow val = new CustomMessageWindow();
		val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_TOOL", "");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE_WINDOW", "");
		val.IsWindowClosable = false;
		val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), (EventHandler)delegate
		{
			SaveAndCloseWindow();
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler)null, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow;
		return val;
	}

	protected override void OnClosed(EventArgs e)
	{
		((CustomWindow)this).OnClosed(e);
	}

	private void CloseWindow()
	{
		((Window)this).Close();
		((UIElement)mMacroRecorderWindow.mOverlayGrid).Visibility = (Visibility)1;
	}

	private void SaveAndCloseWindow()
	{
		SaveButton_Click(null, null);
		ParentWindow.mCommonHandler.AddToastPopup((Window)(object)mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, isShowCloseImage: true);
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		if (IsMacroSettingsChanged())
		{
			((Window)GetUnsavedChangesWindow()).ShowDialog();
		}
		CloseWindow();
	}

	private void SaveScriptSettings()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Invalid comparison between Unknown and I4
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			mRecording.LoopType = FindLoopType();
			if ((double)((Selector)mAccelerationCombobox).SelectedIndex < 0.0)
			{
				mRecording.Acceleration = 1.0;
			}
			else
			{
				mRecording.Acceleration = (double)(((Selector)mAccelerationCombobox).SelectedIndex + 2) * 0.5;
			}
			if (((ToggleButton)mPlayOnStartCheckBox).IsChecked == true)
			{
				if (ParentWindow.mAutoRunMacro != null)
				{
					ParentWindow.mAutoRunMacro.PlayOnStart = false;
					CommonHandlers.SaveMacroJson(ParentWindow.mAutoRunMacro, ParentWindow.mAutoRunMacro.Name + ".json");
				}
				foreach (SingleMacroControl child in ((Panel)ParentWindow.MacroRecorderWindow.mScriptsStackPanel).Children)
				{
					if (ParentWindow.mAutoRunMacro != null && ((TextBox)child.mScriptName).Text == ParentWindow.mAutoRunMacro.Name)
					{
						((UIElement)child.mAutorunImage).Visibility = (Visibility)1;
					}
					if (((TextBox)child.mScriptName).Text == mRecording.Name)
					{
						((UIElement)child.mAutorunImage).Visibility = (Visibility)0;
					}
				}
				ParentWindow.mAutoRunMacro = mRecording;
			}
			mRecording.LoopTime = Convert.ToInt32(((TextBox)mLoopHours).Text, CultureInfo.InvariantCulture) * 3600 + Convert.ToInt32(((TextBox)mLoopMinutes).Text, CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(((TextBox)mLoopSeconds).Text, CultureInfo.InvariantCulture);
			if ((int)((XTextBox)mLoopCountTextBox).InputTextValidity == 1)
			{
				mRecording.LoopNumber = Convert.ToInt32(((TextBox)mLoopCountTextBox).Text, CultureInfo.InvariantCulture);
			}
			mRecording.LoopInterval = Convert.ToInt32(((TextBox)mLoopIntervalMinsTextBox).Text, CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(((TextBox)mLoopIntervalSecondsTextBox).Text, CultureInfo.InvariantCulture);
			mRecording.PlayOnStart = Convert.ToBoolean(((ToggleButton)mPlayOnStartCheckBox).IsChecked, CultureInfo.InvariantCulture);
			mRecording.DonotShowWindowOnFinish = Convert.ToBoolean(((ToggleButton)mDonotShowWindowOnFinishCheckBox).IsChecked, CultureInfo.InvariantCulture);
			mRecording.RestartPlayer = Convert.ToBoolean(((ToggleButton)mRestartPlayerCheckBox).IsChecked, CultureInfo.InvariantCulture);
			mRecording.RestartPlayerAfterMinutes = Convert.ToInt32(((TextBox)mRestartPlayerIntervalTextBox).Text, CultureInfo.InvariantCulture);
			if ((int)mRecording.RecordingType == 0)
			{
				CommonHandlers.SaveMacroJson(mRecording, mRecording.Name + ".json");
			}
			CommonHandlers.RefreshAllMacroRecorderWindow();
			CommonHandlers.OnMacroSettingChanged(mRecording);
			InitSettings();
		}
		catch (Exception ex)
		{
			Logger.Error("Error in saving macro settings: " + ex.ToString());
		}
	}

	private OperationsLoopType FindLoopType()
	{
		if (!((ToggleButton)mRepeatActionInSession).IsChecked.Value)
		{
			if (!((ToggleButton)mRepeatActionTime).IsChecked.Value)
			{
				return (OperationsLoopType)2;
			}
			return (OperationsLoopType)1;
		}
		return (OperationsLoopType)0;
	}

	private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
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

	private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		((RoutedEventArgs)e).Handled = !IsTextAllowed(e.Text);
	}

	private bool IsTextAllowed(string text)
	{
		if (new Regex("^[0-9]+$").IsMatch(text))
		{
			return text.IndexOf(' ') == -1;
		}
		return false;
	}

	private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
	{
		if (e.DataObject.GetDataPresent(typeof(string)))
		{
			string text = (string)e.DataObject.GetData(typeof(string));
			if (!IsTextAllowed(text))
			{
				((DataObjectEventArgs)e).CancelCommand();
			}
		}
		else
		{
			((DataObjectEventArgs)e).CancelCommand();
		}
	}

	private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Key == 18)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private bool IsMacroSettingsChanged()
	{
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		if (((TextBox)mLoopHours).Text != (mRecording.LoopTime / 3600).ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((TextBox)mLoopMinutes).Text != (mRecording.LoopTime / 60 % 60).ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((TextBox)mLoopSeconds).Text != (mRecording.LoopTime % 60).ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((TextBox)mLoopCountTextBox).Text != mRecording.LoopNumber.ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((TextBox)mLoopIntervalMinsTextBox).Text != (mRecording.LoopInterval / 60).ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((TextBox)mLoopIntervalSecondsTextBox).Text != (mRecording.LoopInterval % 60).ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (((ToggleButton)mRestartPlayerCheckBox).IsChecked != mRecording.RestartPlayer)
		{
			return true;
		}
		if (((ToggleButton)mPlayOnStartCheckBox).IsChecked != mRecording.PlayOnStart)
		{
			return true;
		}
		if (((ToggleButton)mDonotShowWindowOnFinishCheckBox).IsChecked != mRecording.DonotShowWindowOnFinish)
		{
			return true;
		}
		if (((TextBox)mRestartPlayerIntervalTextBox).Text != mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture))
		{
			return true;
		}
		if (FindLoopType() != mRecording.LoopType)
		{
			return true;
		}
		if (((Selector)mAccelerationCombobox).SelectedIndex != 0 && mRecording.Acceleration == 0.0)
		{
			return true;
		}
		if (((Selector)mAccelerationCombobox).SelectedIndex != (int)(mRecording.Acceleration / 0.5 - 2.0))
		{
			return true;
		}
		return false;
	}

	private void RestartTextBlock_ToolTipOpening(object sender, ToolTipEventArgs e)
	{
		((FrameworkElement)mRestartTextBlock).ToolTip = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_AFTER", "") + " " + ((TextBox)mRestartPlayerIntervalTextBox).Text + " " + LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", ""), new object[0]);
	}

	private void RepeatAction_Checked(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected I4, but got Unknown
		OperationsLoopType val = FindLoopType();
		switch ((int)val)
		{
		case 0:
			((UIElement)mRepeatActionTimePanelGrid).IsEnabled = false;
			((UIElement)mRepeatActionInSessionGrid).IsEnabled = true;
			((ToggleButton)mRepeatActionInSession).IsChecked = true;
			break;
		case 1:
			((UIElement)mRepeatActionInSessionGrid).IsEnabled = false;
			((UIElement)mRepeatActionTimePanelGrid).IsEnabled = true;
			((ToggleButton)mRepeatActionTime).IsChecked = true;
			break;
		case 2:
			((UIElement)mRepeatActionTimePanelGrid).IsEnabled = false;
			((UIElement)mRepeatActionInSessionGrid).IsEnabled = false;
			((ToggleButton)mRepeatSessionInfinite).IsChecked = true;
			break;
		}
	}

	private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		CustomTextBox val = (CustomTextBox)((sender is CustomTextBox) ? sender : null);
		((XTextBox)val).InputTextValidity = (TextValidityOptions)((!string.IsNullOrEmpty(((TextBox)val).Text) && Convert.ToInt32(((TextBox)mLoopCountTextBox).Text, CultureInfo.InvariantCulture) != 0) ? 1 : (-1));
		((Popup)mErrorNamePopup).IsOpen = (int)((XTextBox)val).InputTextValidity == -1;
		((UIElement)mSaveButton).IsEnabled = (int)((XTextBox)val).InputTextValidity == 1;
	}

	private void LoopCountTextBox_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)((XTextBox)mLoopCountTextBox).InputTextValidity == -1)
		{
			((Popup)mErrorNamePopup).IsOpen = true;
			((Popup)mErrorNamePopup).StaysOpen = true;
		}
		else
		{
			((Popup)mErrorNamePopup).IsOpen = false;
		}
	}

	private void LoopCountTextBox_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mErrorNamePopup).IsOpen = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrosettingswindow.xaml", UriKind.Relative);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Expected O, but got Unknown
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Expected O, but got Unknown
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Expected O, but got Unknown
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Expected O, but got Unknown
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Expected O, but got Unknown
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Expected O, but got Unknown
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Expected O, but got Unknown
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Expected O, but got Unknown
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Expected O, but got Unknown
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Expected O, but got Unknown
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Expected O, but got Unknown
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Expected O, but got Unknown
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Expected O, but got Unknown
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Expected O, but got Unknown
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Expected O, but got Unknown
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			mSettingsHeaderText = (TextBlock)target;
			((UIElement)mSettingsHeaderText).MouseDown += new MouseButtonEventHandler(TopBar_MouseDown);
			break;
		case 3:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 4:
			mRepeactActionPanel = (StackPanel)target;
			break;
		case 5:
			mRepeatActionInSession = (CustomRadioButton)target;
			((ToggleButton)mRepeatActionInSession).Checked += new RoutedEventHandler(RepeatAction_Checked);
			break;
		case 6:
			mRepeatActionInSessionGrid = (Grid)target;
			break;
		case 7:
			mLoopCountTextBox = (CustomTextBox)target;
			((UIElement)mLoopCountTextBox).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopCountTextBox).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopCountTextBox).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			((UIElement)mLoopCountTextBox).MouseEnter += new MouseEventHandler(LoopCountTextBox_MouseEnter);
			((UIElement)mLoopCountTextBox).MouseLeave += new MouseEventHandler(LoopCountTextBox_MouseLeave);
			break;
		case 8:
			mErrorNamePopup = (CustomPopUp)target;
			break;
		case 9:
			mMaskBorder1 = (Border)target;
			break;
		case 10:
			mErrorText = (TextBlock)target;
			break;
		case 11:
			mDownArrow = (Path)target;
			break;
		case 12:
			mRepeatActionTimePanel = (StackPanel)target;
			break;
		case 13:
			mRepeatActionTime = (CustomRadioButton)target;
			((ToggleButton)mRepeatActionTime).Checked += new RoutedEventHandler(RepeatAction_Checked);
			break;
		case 14:
			mRepeatActionTimePanelGrid = (Grid)target;
			break;
		case 15:
			mLoopHours = (CustomTextBox)target;
			((UIElement)mLoopHours).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopHours).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopHours).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 16:
			mLoopMinutes = (CustomTextBox)target;
			((UIElement)mLoopMinutes).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopMinutes).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopMinutes).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 17:
			mLoopSeconds = (CustomTextBox)target;
			((UIElement)mLoopSeconds).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopSeconds).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopSeconds).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 18:
			mRepeatSessionInfinitePanel = (StackPanel)target;
			break;
		case 19:
			mRepeatSessionInfinite = (CustomRadioButton)target;
			((ToggleButton)mRepeatSessionInfinite).Checked += new RoutedEventHandler(RepeatAction_Checked);
			break;
		case 20:
			mLoopIntervalMinsTextBox = (CustomTextBox)target;
			((UIElement)mLoopIntervalMinsTextBox).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopIntervalMinsTextBox).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopIntervalMinsTextBox).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 21:
			mLoopIntervalSecondsTextBox = (CustomTextBox)target;
			((UIElement)mLoopIntervalSecondsTextBox).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mLoopIntervalSecondsTextBox).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mLoopIntervalSecondsTextBox).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 22:
			mAccelerationCombobox = (CustomComboBox)target;
			break;
		case 23:
			mPlayOnStartCheckBox = (CustomCheckbox)target;
			break;
		case 24:
			mRestartPlayerCheckBox = (CustomCheckbox)target;
			break;
		case 25:
			mRestartPlayerIntervalTextBox = (CustomTextBox)target;
			((UIElement)mRestartPlayerIntervalTextBox).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)mRestartPlayerIntervalTextBox).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)mRestartPlayerIntervalTextBox).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 26:
			mRestartTextBlock = (TextBlock)target;
			((FrameworkElement)mRestartTextBlock).ToolTipOpening += new ToolTipEventHandler(RestartTextBlock_ToolTipOpening);
			break;
		case 27:
			mDonotShowWindowOnFinishCheckBox = (CustomCheckbox)target;
			break;
		case 28:
			mSaveButton = (CustomButton)target;
			((ButtonBase)mSaveButton).Click += new RoutedEventHandler(SaveButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
