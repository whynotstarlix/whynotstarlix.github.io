using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class AdvancedGameControlWindow : CustomWindow, IComponentConnector
{
	private MainWindow ParentWindow;

	internal KeymapCanvasWindow CanvasWindow;

	private CustomToastPopupControl mToastPopup;

	internal ExportSchemesWindow mExportSchemesWindow;

	internal ImportSchemesWindow mImportSchemesWindow;

	internal double mLastSliderValue;

	internal double mLastSavedSliderValue;

	internal Dictionary<string, string> mScriptModeDictionary = new Dictionary<string, string>();

	internal List<IMAction> mLastScriptActionItem;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mAdvancedGameControlBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid PrimaryGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseSideBarWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mProfileHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mImport;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mExport;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mOpenFolder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SchemeComboBox mSchemeComboBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBrowserHelp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal WrapPanel mPrimitivesPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mTapPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mTapRepeatPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mDpadPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mPanPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mZoomPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mMOBASkillPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mSwipePrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mFreeLookPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mTiltPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mStatePrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mScriptPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mMouseZoomPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mRotatePrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mScrollPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AdvancedSettingsItemPanel mEdgeScrollPrimitive;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNCTransparencySlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mNCTransparencyLevel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mNCTranslucentControlsSliderButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider mNCTransSlider;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mButtonsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mRevertBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mUndoBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSaveBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOverlayGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid KeySequenceScriptGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mScriptHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mHeaderText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseScriptWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSubheadingText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mScriptText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mXYCurrentCoordinatesText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mShowHelpHyperlink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFooterGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mFooterText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mKeySeqDoneButton;

	private bool _contentLoaded;

	internal AdvancedGameControlWindow(MainWindow window)
	{
		ParentWindow = window;
		InitializeComponent();
		if (KMManager.sIsDeveloperModeOn)
		{
			((UIElement)mStatePrimitive).Visibility = (Visibility)0;
			((UIElement)mMouseZoomPrimitive).Visibility = (Visibility)0;
			((UIElement)mScrollPrimitive).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mStatePrimitive).Visibility = (Visibility)2;
			((UIElement)mMouseZoomPrimitive).Visibility = (Visibility)2;
			((UIElement)mScrollPrimitive).Visibility = (Visibility)2;
		}
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)mBrowserHelp).Visibility = (Visibility)2;
		}
		((FrameworkElement)this).Width = 0.0;
		((FrameworkElement)this).Height = 0.0;
		BlueStacksUIBinding.Bind(mShowHelpHyperlink, "STRING_SCRIPT_GUIDE", "");
		AdvancedSettingsItemPanel advancedSettingsItemPanel = mTapPrimitive;
		advancedSettingsItemPanel.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel2 = mTapRepeatPrimitive;
		advancedSettingsItemPanel2.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel2.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel3 = mDpadPrimitive;
		advancedSettingsItemPanel3.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel3.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel4 = mZoomPrimitive;
		advancedSettingsItemPanel4.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel4.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel5 = mFreeLookPrimitive;
		advancedSettingsItemPanel5.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel5.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel6 = mPanPrimitive;
		advancedSettingsItemPanel6.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel6.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel7 = mMOBASkillPrimitive;
		advancedSettingsItemPanel7.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel7.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel8 = mSwipePrimitive;
		advancedSettingsItemPanel8.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel8.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel9 = mTiltPrimitive;
		advancedSettingsItemPanel9.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel9.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel10 = mStatePrimitive;
		advancedSettingsItemPanel10.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel10.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel11 = mScriptPrimitive;
		advancedSettingsItemPanel11.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel11.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel12 = mMouseZoomPrimitive;
		advancedSettingsItemPanel12.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel12.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel13 = mRotatePrimitive;
		advancedSettingsItemPanel13.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel13.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel14 = mScrollPrimitive;
		advancedSettingsItemPanel14.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel14.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel15 = mEdgeScrollPrimitive;
		advancedSettingsItemPanel15.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel15.MouseDragStart, new EventHandler(AdvancedSettingsItemPanel_MouseDragStart));
		AdvancedSettingsItemPanel advancedSettingsItemPanel16 = mTapPrimitive;
		advancedSettingsItemPanel16.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel16.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel17 = mTapRepeatPrimitive;
		advancedSettingsItemPanel17.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel17.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel18 = mDpadPrimitive;
		advancedSettingsItemPanel18.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel18.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel19 = mZoomPrimitive;
		advancedSettingsItemPanel19.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel19.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel20 = mFreeLookPrimitive;
		advancedSettingsItemPanel20.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel20.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel21 = mPanPrimitive;
		advancedSettingsItemPanel21.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel21.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel22 = mMOBASkillPrimitive;
		advancedSettingsItemPanel22.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel22.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel23 = mSwipePrimitive;
		advancedSettingsItemPanel23.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel23.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel24 = mTiltPrimitive;
		advancedSettingsItemPanel24.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel24.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel25 = mStatePrimitive;
		advancedSettingsItemPanel25.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel25.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel26 = mScriptPrimitive;
		advancedSettingsItemPanel26.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel26.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel27 = mMouseZoomPrimitive;
		advancedSettingsItemPanel27.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel27.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel28 = mRotatePrimitive;
		advancedSettingsItemPanel28.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel28.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel29 = mScrollPrimitive;
		advancedSettingsItemPanel29.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel29.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
		AdvancedSettingsItemPanel advancedSettingsItemPanel30 = mEdgeScrollPrimitive;
		advancedSettingsItemPanel30.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel30.Tap, new EventHandler(AdvancedSettingsItemPanel_Tap));
	}

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		KMManager.sIsDeveloperModeOn = false;
		((Window)this).Close();
	}

	internal void ToggleAGCWindowVisiblity(bool isScriptModeWindow)
	{
		KMManager.sIsInScriptEditingMode = isScriptModeWindow;
		mScriptModeDictionary["isInScriptMode"] = isScriptModeWindow.ToString(CultureInfo.InvariantCulture);
		BindOrUnbindMouseEvents(isScriptModeWindow);
		if (isScriptModeWindow)
		{
			((UIElement)PrimaryGrid).Visibility = (Visibility)2;
			((UIElement)KeySequenceScriptGrid).Visibility = (Visibility)0;
			((Window)this).Owner = (Window)(object)ParentWindow;
			PopulateScriptTextBox();
			((Window)CanvasWindow).Hide();
			ParentWindow.Utils.ToggleTopBarSidebarEnabled(isEnabled: false);
		}
		else
		{
			((UIElement)PrimaryGrid).Visibility = (Visibility)0;
			((UIElement)KeySequenceScriptGrid).Visibility = (Visibility)2;
			((Window)CanvasWindow).Show();
			((Window)this).Owner = (Window)(object)CanvasWindow;
			((Window)this).Activate();
			ParentWindow.Utils.ToggleTopBarSidebarEnabled(isEnabled: true);
		}
		HTTPUtils.SendRequestToEngineAsync("scriptEditingModeEntered", mScriptModeDictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
	}

	private void BindOrUnbindMouseEvents(bool bind)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		((UIElement)this).MouseEnter -= new MouseEventHandler(AdvancedGameControlWindow_MouseEnter);
		((UIElement)this).MouseLeave -= new MouseEventHandler(AdvancedGameControlWindow_MouseLeave);
		if (bind)
		{
			((UIElement)this).MouseEnter += new MouseEventHandler(AdvancedGameControlWindow_MouseEnter);
			((UIElement)this).MouseLeave += new MouseEventHandler(AdvancedGameControlWindow_MouseLeave);
		}
	}

	private void AdvancedGameControlWindow_MouseLeave(object sender, MouseEventArgs e)
	{
		if (((Window)ParentWindow).IsActive)
		{
			ParentWindow.mFrontendHandler.ShowGLWindow();
		}
	}

	private void AdvancedGameControlWindow_MouseEnter(object sender, MouseEventArgs e)
	{
	}

	private void AdvancedGameControlWindow_Closing(object sender, CancelEventArgs evt)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (KeymapCanvasWindow.sIsDirty)
		{
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE", "");
			val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), (EventHandler)delegate
			{
				KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: false);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), (EventHandler)delegate
			{
				KMManager.LoadIMActions(ParentWindow, KMManager.sPackageName);
				KeymapCanvasWindow.sIsDirty = false;
			}, (string)null, false, (object)null);
			val.CloseButtonHandle((EventHandler)delegate
			{
				CanvasWindow.mIsClosing = false;
				evt.Cancel = true;
			}, (object)null);
			((Window)val).Owner = (Window)(object)CanvasWindow;
			((Window)val).ShowDialog();
		}
		CanvasWindow.SidebarWindowLeft = ((Window)this).Left;
		CanvasWindow.SidebarWindowTop = ((Window)this).Top;
		((Window)ParentWindow).Activate();
		ParentWindow.Utils.ToggleTopBarSidebarEnabled(isEnabled: true);
	}

	private void AdvancedGameControlWindow_Closed(object sender, EventArgs e)
	{
		CanvasWindow.SidebarWindow = null;
		if (KeymapCanvasWindow.sWasMaximized)
		{
			ParentWindow.MaximizeWindow();
		}
		else
		{
			ParentWindow.ChangeHeightWidthTopLeft(CanvasWindow.mParentWindowWidth, CanvasWindow.mParentWindowHeight, CanvasWindow.mParentWindowTop, CanvasWindow.mParentWindowLeft);
		}
		KeymapCanvasWindow.sWasMaximized = false;
		if (((FrameworkElement)CanvasWindow).IsLoaded && !CanvasWindow.mIsClosing)
		{
			((Window)CanvasWindow).Close();
		}
		if (RegistryManager.Instance.ShowKeyControlsOverlay)
		{
			KMManager.ShowOverlayWindow(ParentWindow, isShow: true, isreload: true);
		}
	}

	internal void Init(KeymapCanvasWindow window)
	{
		CanvasWindow = window;
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((RangeBase)mNCTransSlider).Value = RegistryManager.Instance.TranslucentControlsTransparency;
			mLastSavedSliderValue = ((RangeBase)mNCTransSlider).Value;
			((TextBox)mNCTransparencyLevel).Text = ((int)(((RangeBase)mNCTransSlider).Value * 100.0)).ToString(CultureInfo.InvariantCulture);
			if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
			{
				mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
			}
			ParentWindow.mCommonHandler.OverlayStateChangedEvent += ParentWindow_OverlayStateChangedEvent;
		}
		else
		{
			((UIElement)mNCTransparencySlider).Visibility = (Visibility)2;
		}
		FillProfileCombo();
		ProfileChanged();
		((UIElement)mSaveBtn).IsEnabled = false;
		((UIElement)mUndoBtn).IsEnabled = false;
	}

	internal void InsertXYInScript(double x, double y)
	{
		string text = " " + x.ToString("00.00", CultureInfo.InvariantCulture) + " " + y.ToString("00.00", CultureInfo.InvariantCulture);
		int selectionStart = ((TextBox)mScriptText).SelectionStart + text.Length;
		((TextBox)mScriptText).Text = ((TextBox)mScriptText).Text.Insert(((TextBox)mScriptText).SelectionStart, text);
		((TextBox)mScriptText).SelectionStart = selectionStart;
	}

	internal void OrderingControlSchemes()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		ParentWindow.SelectedConfig.ControlSchemes.Sort(CompareSchemesAlphabetically);
		foreach (IMControlScheme item in new List<IMControlScheme>(ParentWindow.SelectedConfig.ControlSchemes))
		{
			if (item.BuiltIn)
			{
				if (item.IsBookMarked)
				{
					ParentWindow.SelectedConfig.ControlSchemes.Remove(item);
					ParentWindow.SelectedConfig.ControlSchemes.Insert(num3, item);
					num3++;
					num2++;
					num++;
				}
				else
				{
					ParentWindow.SelectedConfig.ControlSchemes.Remove(item);
					ParentWindow.SelectedConfig.ControlSchemes.Insert(num2, item);
					num2++;
					num++;
				}
			}
			else if (item.IsBookMarked)
			{
				ParentWindow.SelectedConfig.ControlSchemes.Remove(item);
				ParentWindow.SelectedConfig.ControlSchemes.Insert(num, item);
				num++;
			}
		}
	}

	private int CompareSchemesAlphabetically(IMControlScheme x, IMControlScheme y)
	{
		string text = x.Name.ToLower(CultureInfo.InvariantCulture).Trim();
		string text2 = y.Name.ToLower(CultureInfo.InvariantCulture).Trim();
		if (text.Contains(text2))
		{
			return 1;
		}
		if (text2.Contains(text))
		{
			return -1;
		}
		if (string.CompareOrdinal(text, text2) < 0)
		{
			return -1;
		}
		return 1;
	}

	public void FillProfileCombo()
	{
		OrderingControlSchemes();
		ComboBoxSchemeControl comboBoxSchemeControl = null;
		((Panel)mSchemeComboBox.Items).Children.Clear();
		if (ParentWindow.SelectedConfig.ControlSchemes != null && ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
		{
			((UIElement)mProfileHeader).Visibility = (Visibility)0;
			foreach (IMControlScheme item in ParentWindow.SelectedConfig.ControlSchemesDict.Values)
			{
				ComboBoxSchemeControl comboBoxSchemeControl2 = new ComboBoxSchemeControl(CanvasWindow, ParentWindow);
				((TextBox)comboBoxSchemeControl2.mSchemeName).Text = item.Name;
				((UIElement)comboBoxSchemeControl2).IsEnabled = true;
				if (item.Selected)
				{
					comboBoxSchemeControl = comboBoxSchemeControl2;
					BlueStacksUIBinding.BindColor((DependencyObject)(object)comboBoxSchemeControl2, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
				}
				if (item.BuiltIn || ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, item.Name, StringComparison.InvariantCulture)) == 2)
				{
					((UIElement)comboBoxSchemeControl2.mEditImg).Visibility = (Visibility)1;
					((UIElement)comboBoxSchemeControl2.mDeleteImg).Visibility = (Visibility)1;
				}
				if (item.IsBookMarked)
				{
					comboBoxSchemeControl2.mBookmarkImg.ImageName = "bookmarked";
				}
				((Panel)mSchemeComboBox.Items).Children.Add((UIElement)(object)comboBoxSchemeControl2);
			}
			if (comboBoxSchemeControl == null)
			{
				comboBoxSchemeControl = ((Panel)mSchemeComboBox.Items).Children[0] as ComboBoxSchemeControl;
				ParentWindow.SelectedConfig.ControlSchemesDict[((TextBox)comboBoxSchemeControl.mSchemeName).Text].Selected = true;
			}
			else
			{
				mSchemeComboBox.SelectedItem = ((TextBox)comboBoxSchemeControl.mSchemeName).Text.ToString(CultureInfo.InvariantCulture);
				ParentWindow.SelectedConfig.SelectedControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[mSchemeComboBox.SelectedItem];
				mSchemeComboBox.mName.Text = mSchemeComboBox.SelectedItem;
			}
		}
		else
		{
			BlueStacksUIBinding.Bind(CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
		}
		if (ParentWindow.OriginalLoadedConfig.ControlSchemes != null && ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
		{
			((UIElement)mExport).IsEnabled = true;
		}
		else
		{
			((UIElement)mExport).IsEnabled = false;
		}
		((UIElement)mRevertBtn).IsEnabled = ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) == 2;
	}

	private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		try
		{
			((Window)this).DragMove();
		}
		catch
		{
		}
	}

	private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void UndoButton_Click(object sender, RoutedEventArgs e)
	{
		CloseWindow();
	}

	private void SaveButton_Click(object sender, RoutedEventArgs e)
	{
		KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: false);
		mLastSavedSliderValue = ((RangeBase)mNCTransSlider).Value;
		FillProfileCombo();
		AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
	}

	internal void AddToastPopup(string message)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		try
		{
			if (mToastPopup == null)
			{
				mToastPopup = new CustomToastPopupControl((Window)(object)this);
			}
			mToastPopup.Init((Window)(object)this, message, (Brush)null, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)2, (Thickness?)null, 12, (Thickness?)null, (Brush)null, false);
			mToastPopup.ShowPopup(1.3);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in showing toast popup: " + ex.ToString());
		}
	}

	private void AdvancedSettingsItemPanel_Tap(object sender, EventArgs e)
	{
		AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel, isTap: true);
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void AdvancedSettingsItemPanel_MouseDragStart(object sender, EventArgs e)
	{
		AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel);
		((FrameworkElement)this).Cursor = Cursors.Arrow;
	}

	private void AddAdvancedControlToCanvas(AdvancedSettingsItemPanel sender, bool isTap = false)
	{
		if (ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
		{
			KMManager.AddNewControlSchemeAndSelect(ParentWindow);
		}
		KMManager.CheckAndCreateNewScheme();
		if (!isTap)
		{
			((UIElement)this).Focus();
			((FrameworkElement)this).Cursor = Cursors.Hand;
		}
		KeyActionType actionType = sender.ActionType;
		IMAction action = Assembly.GetExecutingAssembly().CreateInstance(actionType.ToString()) as IMAction;
		KMManager.GetCanvasElement(ParentWindow, action, mCanvas, addToCanvas: false);
		if (isTap)
		{
			List<IMAction> lstAction = KMManager.ClearElement();
			CanvasWindow.AddNewCanvasElement(lstAction, isTap: true);
			KMManager.ClearElement();
		}
		sender.ReatchedMouseMove();
	}

	private void mCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		KMManager.RepositionCanvasElement();
	}

	private void mCanvas_MouseUp(object sender, MouseButtonEventArgs e)
	{
		((FrameworkElement)this).Cursor = Cursors.Arrow;
		KMManager.ClearElement();
	}

	private void mButtonsGrid_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).MaxWidth = ((((FrameworkElement)mButtonsGrid).ActualWidth < 320.0) ? 320.0 : ((FrameworkElement)mButtonsGrid).ActualWidth);
		((FrameworkElement)this).MinWidth = ((FrameworkElement)this).MaxWidth;
		((Window)this).Left = ((CanvasWindow.SidebarWindowLeft == -1.0) ? (((Window)ParentWindow).Left + ((FrameworkElement)ParentWindow).ActualWidth - (double)(ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? 60 : 0)) : CanvasWindow.SidebarWindowLeft);
		((Window)this).Top = ((CanvasWindow.SidebarWindowTop == -1.0) ? ((Window)ParentWindow).Top : CanvasWindow.SidebarWindowTop);
		((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow).ActualHeight;
		Screen val = Screen.FromHandle(new WindowInteropHelper((Window)(object)this).Handle);
		double sScalingFactor = MainWindow.sScalingFactor;
		Rectangle rectangle = new Rectangle((int)((double)val.WorkingArea.X / sScalingFactor), (int)((double)val.WorkingArea.Y / sScalingFactor), (int)((double)val.WorkingArea.Width / sScalingFactor), (int)((double)val.WorkingArea.Height / sScalingFactor));
		Rectangle rect = new Rectangle(new Point((int)((Window)this).Left, (int)((Window)this).Top), new Size((int)((FrameworkElement)this).ActualWidth, (int)((FrameworkElement)this).ActualHeight));
		if (!rectangle.Contains(rect))
		{
			((Window)this).Left = (double)rectangle.Width - ((FrameworkElement)this).Width;
		}
	}

	public void ProfileChanged()
	{
		if (mSchemeComboBox.SelectedItem == null)
		{
			return;
		}
		string selectedItem = mSchemeComboBox.SelectedItem;
		if (ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(selectedItem))
		{
			if (!ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem].Selected)
			{
				ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
				foreach (ComboBoxSchemeControl child in ((Panel)mSchemeComboBox.Items).Children)
				{
					if (((TextBox)child.mSchemeName).Text == ParentWindow.SelectedConfig.SelectedControlScheme.Name)
					{
						BlueStacksUIBinding.BindColor((DependencyObject)(object)child, Control.BackgroundProperty, "ComboBoxBackgroundColor");
						break;
					}
				}
				ParentWindow.SelectedConfig.SelectedControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem];
				ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
				KeymapCanvasWindow.sIsDirty = true;
			}
			CanvasWindow.Init();
		}
		((UIElement)mRevertBtn).IsEnabled = ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) == 2;
	}

	private void AdvancedGameControlWindow_Loaded(object sender, RoutedEventArgs e)
	{
		((Window)this).Activate();
	}

	private void AdvancedGameControlWindow_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		if ((int)e.Key != 0)
		{
			if (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119))
			{
				text = IMAPKeys.GetStringForFile((Key)118) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121))
			{
				text = text + IMAPKeys.GetStringForFile((Key)120) + " + ";
			}
			if (Keyboard.IsKeyDown((Key)116) || Keyboard.IsKeyDown((Key)117))
			{
				text = text + IMAPKeys.GetStringForFile((Key)116) + " + ";
			}
			text += IMAPKeys.GetStringForFile(e.Key);
		}
		Logger.Debug("SHORTCUT: KeyPressed.." + text);
		if (ParentWindow.mCommonHandler.mShortcutsConfigInstance == null)
		{
			return;
		}
		foreach (ShortcutKeys item in ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
		{
			if (string.Equals(item.ShortcutKey, text, StringComparison.InvariantCulture) && string.Equals(item.ShortcutName, "STRING_CONTROLS_EDITOR", StringComparison.InvariantCulture))
			{
				KMManager.CloseWindows();
			}
		}
	}

	private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			using Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			if (!Directory.Exists(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles")))
			{
				process.StartInfo.FileName = RegistryStrings.InputMapperFolder;
			}
			else
			{
				process.StartInfo.FileName = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
			}
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Some error in Open folder err: " + ex.ToString());
		}
	}

	private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
	{
		ClientStats.SendMiscellaneousStatsAsync("ExportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail);
		if (ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
		{
			((UIElement)mOverlayGrid).Visibility = (Visibility)0;
			if (mExportSchemesWindow == null)
			{
				ExportSchemesWindow exportSchemesWindow = new ExportSchemesWindow(CanvasWindow, ParentWindow);
				((Window)exportSchemesWindow).Owner = (Window)(object)this;
				mExportSchemesWindow = exportSchemesWindow;
				mExportSchemesWindow.Init();
				((Window)mExportSchemesWindow).Show();
			}
		}
		else
		{
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_AVAILABLE", ""));
		}
	}

	private void ImportBtn_Click(object sender, MouseButtonEventArgs e)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Invalid comparison between Unknown and I4
		ClientStats.SendMiscellaneousStatsAsync("ImportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail);
		((UIElement)mOverlayGrid).Visibility = (Visibility)0;
		OpenFileDialog val = new OpenFileDialog
		{
			Multiselect = true,
			Filter = "Cfg files (*.cfg)|*.cfg"
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() == 1)
			{
				ImportSchemesWindow importSchemesWindow = new ImportSchemesWindow(CanvasWindow, ParentWindow);
				((Window)importSchemesWindow).Owner = (Window)(object)this;
				mImportSchemesWindow = importSchemesWindow;
				mImportSchemesWindow.Init(((FileDialog)val).FileName);
				((Window)mImportSchemesWindow).Show();
			}
			else
			{
				((UIElement)mOverlayGrid).Visibility = (Visibility)1;
				mImportSchemesWindow = null;
				((UIElement)this).Focus();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void mCloseScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ToggleAGCWindowVisiblity(isScriptModeWindow: false);
		PopulateScriptTextBox();
		ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_close_click");
	}

	private void PopulateScriptTextBox()
	{
		if (mLastScriptActionItem != null)
		{
			IMAction iMAction = mLastScriptActionItem.First();
			if (iMAction.Type == KeyActionType.Script)
			{
				string text = string.Join(Environment.NewLine, (iMAction as Script).Commands.ToArray());
				((TextBox)mScriptText).Text = text;
				KeymapCanvasWindow.sIsDirty = true;
			}
		}
	}

	private void ShowHelpHyperlink_Click(object sender, RoutedEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/help_articles") + "&article=keymapping_script_faq");
	}

	private void mDoneScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (mLastScriptActionItem != null)
		{
			IMAction iMAction = mLastScriptActionItem.First();
			if (iMAction.Type == KeyActionType.Script)
			{
				string[] array = ((TextBox)mScriptText).Text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
				if (!CheckIfScriptValid(array))
				{
					AddToastPopup(LocaleStrings.GetLocalizedString("STRING_INVALID_SCRIPT_COMMANDS", ""));
					return;
				}
				ListExtensions.ClearAddRange<string>((iMAction as Script).Commands, array.ToList());
			}
		}
		ToggleAGCWindowVisiblity(isScriptModeWindow: false);
		ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_done_click");
	}

	private void NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (((RangeBase)mNCTransSlider).Value == 0.0)
		{
			((RangeBase)mNCTransSlider).Value = mLastSliderValue;
			((TextBox)mNCTransparencyLevel).Text = ((int)(((RangeBase)mNCTransSlider).Value * 100.0)).ToString(CultureInfo.InvariantCulture);
			if (mLastSliderValue > 0.0)
			{
				mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
			}
			RegistryManager.Instance.ShowKeyControlsOverlay = true;
		}
		else
		{
			mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
			double value = ((RangeBase)mNCTransSlider).Value;
			((RangeBase)mNCTransSlider).Value = 0.0;
			((TextBox)mNCTransparencyLevel).Text = ((int)(((RangeBase)mNCTransSlider).Value * 100.0)).ToString(CultureInfo.InvariantCulture);
			mLastSliderValue = value;
			RegistryManager.Instance.ShowKeyControlsOverlay = false;
		}
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void NCTransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		KMManager.ChangeTransparency(ParentWindow, ((RangeBase)mNCTransSlider).Value);
		if (((RangeBase)mNCTransSlider).Value == 0.0)
		{
			ParentWindow_OverlayStateChangedEvent(isEnabled: false);
		}
		else
		{
			ParentWindow_OverlayStateChangedEvent(isEnabled: true);
		}
		mLastSliderValue = ((RangeBase)mNCTransSlider).Value;
		((TextBox)mNCTransparencyLevel).Text = ((int)(((RangeBase)mNCTransSlider).Value * 100.0)).ToString(CultureInfo.InvariantCulture);
		if (((RangeBase)mNCTransSlider).Value != RegistryManager.Instance.TranslucentControlsTransparency)
		{
			KeymapCanvasWindow.sIsDirty = true;
		}
	}

	public void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
	{
		if (isEnabled)
		{
			mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
			if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0 && mLastSliderValue == 0.0)
			{
				RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
				((RangeBase)mNCTransSlider).Value = 0.5;
				((TextBox)mNCTransparencyLevel).Text = ((int)(((RangeBase)mNCTransSlider).Value * 100.0)).ToString(CultureInfo.InvariantCulture);
			}
			else if (((RangeBase)mNCTransSlider).Value == 0.0)
			{
				RegistryManager.Instance.TranslucentControlsTransparency = mLastSliderValue;
			}
			else
			{
				RegistryManager.Instance.TranslucentControlsTransparency = ((RangeBase)mNCTransSlider).Value;
			}
			RegistryManager.Instance.ShowKeyControlsOverlay = true;
		}
		else
		{
			mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
			RegistryManager.Instance.TranslucentControlsTransparency = 0.0;
			double value = ((RangeBase)mNCTransSlider).Value;
			((RangeBase)mNCTransSlider).Value = 0.0;
			((TextBox)mNCTransparencyLevel).Text = "0";
			mLastSliderValue = value;
			RegistryManager.Instance.ShowKeyControlsOverlay = false;
		}
	}

	private void BrowserHelp_MouseEnter(object sender, MouseEventArgs e)
	{
		((FrameworkElement)this).Cursor = Cursors.Hand;
	}

	private void BrowserHelp_MouseLeave(object sender, MouseEventArgs e)
	{
		((FrameworkElement)this).Cursor = Cursors.Arrow;
	}

	private void Export_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		if (((UIElement)mExport).IsEnabled)
		{
			((UIElement)mExport).Opacity = 1.0;
		}
		else
		{
			((UIElement)mExport).Opacity = 0.4;
		}
	}

	private void KeySequenceScriptGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)KeySequenceScriptGrid).Visibility == 0 && !((UIElement)mScriptText).IsMouseOver)
		{
			((UIElement)mAdvancedGameControlBorder).Focus();
		}
	}

	private void BrowserHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=advanced_game_control");
	}

	private void RevertBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageWindow val = new CustomMessageWindow
		{
			WindowStartupLocation = (WindowStartupLocation)1
		};
		val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
		val.AddButton((ButtonColors)0, "STRING_RESET", (EventHandler)delegate
		{
			string schemeName = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
			bool isBookMarked = ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
			ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
			IMControlScheme iMControlScheme = ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture)).FirstOrDefault();
			if (iMControlScheme != null)
			{
				iMControlScheme.Selected = true;
				ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme;
				ParentWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name] = iMControlScheme;
				iMControlScheme.IsBookMarked = isBookMarked;
				FillProfileCombo();
				ProfileChanged();
				((UIElement)mSaveBtn).IsEnabled = false;
				((UIElement)mUndoBtn).IsEnabled = false;
				KeymapCanvasWindow.sIsDirty = true;
				KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: false);
				ClientStats.SendKeyMappingUIStatsAsync("advancedcontrols_reset", KMManager.sPackageName);
			}
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)delegate
		{
		}, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
	}

	private bool CheckIfScriptValid(string[] scriptCmds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		bool result = false;
		try
		{
			JObject val = new JObject();
			val.Add("Commands", (JToken)(object)JArray.FromObject((object)scriptCmds.ToList()));
			JObject val2 = val;
			JToken obj = JToken.Parse(HTTPUtils.SendRequestToEngine("validateScriptCommands", new Dictionary<string, string> { 
			{
				"script",
				((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0])
			} }, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, ""));
			result = JObject.Parse(((object)((JArray)((obj is JArray) ? obj : null))[0]).ToString())["success"].ToObject<bool>();
		}
		catch
		{
		}
		return result;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/advancedgamecontrolwindow.xaml", UriKind.Relative);
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected O, but got Unknown
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Expected O, but got Unknown
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Expected O, but got Unknown
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Expected O, but got Unknown
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Expected O, but got Unknown
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Expected O, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Expected O, but got Unknown
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Expected O, but got Unknown
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Expected O, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Expected O, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected O, but got Unknown
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Expected O, but got Unknown
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Expected O, but got Unknown
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Expected O, but got Unknown
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Expected O, but got Unknown
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Expected O, but got Unknown
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Expected O, but got Unknown
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Expected O, but got Unknown
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Expected O, but got Unknown
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Expected O, but got Unknown
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Expected O, but got Unknown
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Expected O, but got Unknown
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Expected O, but got Unknown
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Expected O, but got Unknown
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Expected O, but got Unknown
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Expected O, but got Unknown
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Expected O, but got Unknown
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Expected O, but got Unknown
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Expected O, but got Unknown
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Expected O, but got Unknown
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Expected O, but got Unknown
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(AdvancedGameControlWindow)target).Loaded += new RoutedEventHandler(AdvancedGameControlWindow_Loaded);
			((Window)(AdvancedGameControlWindow)target).Closing += AdvancedGameControlWindow_Closing;
			((Window)(AdvancedGameControlWindow)target).Closed += AdvancedGameControlWindow_Closed;
			((UIElement)(AdvancedGameControlWindow)target).KeyDown += new KeyEventHandler(AdvancedGameControlWindow_KeyDown);
			break;
		case 2:
			mAdvancedGameControlBorder = (Border)target;
			((UIElement)mAdvancedGameControlBorder).PreviewMouseDown += new MouseButtonEventHandler(KeySequenceScriptGrid_PreviewMouseDown);
			break;
		case 3:
			PrimaryGrid = (Grid)target;
			break;
		case 4:
			((UIElement)(Grid)target).MouseLeftButtonDown += new MouseButtonEventHandler(TopBar_MouseLeftButtonDown);
			break;
		case 5:
			((UIElement)(TextBlock)target).MouseLeftButtonDown += new MouseButtonEventHandler(TopBar_MouseLeftButtonDown);
			break;
		case 6:
			mCloseSideBarWindow = (CustomPictureBox)target;
			((UIElement)mCloseSideBarWindow).MouseDown += new MouseButtonEventHandler(CustomPictureBox_MouseDown);
			((UIElement)mCloseSideBarWindow).MouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 7:
			mProfileHeader = (TextBlock)target;
			break;
		case 8:
			mImport = (CustomPictureBox)target;
			((UIElement)mImport).MouseLeftButtonUp += new MouseButtonEventHandler(ImportBtn_Click);
			break;
		case 9:
			mExport = (CustomPictureBox)target;
			((UIElement)mExport).IsEnabledChanged += new DependencyPropertyChangedEventHandler(Export_IsEnabledChanged);
			((UIElement)mExport).MouseLeftButtonUp += new MouseButtonEventHandler(ExportBtn_Click);
			break;
		case 10:
			mOpenFolder = (CustomPictureBox)target;
			((UIElement)mOpenFolder).MouseLeftButtonUp += new MouseButtonEventHandler(OpenFolder_MouseLeftButtonUp);
			break;
		case 11:
			mSchemeComboBox = (SchemeComboBox)target;
			break;
		case 12:
			mBrowserHelp = (CustomPictureBox)target;
			((UIElement)mBrowserHelp).MouseEnter += new MouseEventHandler(BrowserHelp_MouseEnter);
			((UIElement)mBrowserHelp).MouseLeave += new MouseEventHandler(BrowserHelp_MouseLeave);
			((UIElement)mBrowserHelp).MouseLeftButtonUp += new MouseButtonEventHandler(BrowserHelp_MouseLeftButtonUp);
			break;
		case 13:
			mPrimitivesPanel = (WrapPanel)target;
			break;
		case 14:
			mTapPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 15:
			mTapRepeatPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 16:
			mDpadPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 17:
			mPanPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 18:
			mZoomPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 19:
			mMOBASkillPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 20:
			mSwipePrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 21:
			mFreeLookPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 22:
			mTiltPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 23:
			mStatePrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 24:
			mScriptPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 25:
			mMouseZoomPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 26:
			mRotatePrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 27:
			mScrollPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 28:
			mEdgeScrollPrimitive = (AdvancedSettingsItemPanel)target;
			break;
		case 29:
			mNCTransparencySlider = (Grid)target;
			break;
		case 30:
			mNCTransparencyLevel = (CustomTextBox)target;
			break;
		case 31:
			mNCTranslucentControlsSliderButton = (CustomPictureBox)target;
			((UIElement)mNCTranslucentControlsSliderButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
			break;
		case 32:
			mNCTransSlider = (Slider)target;
			((RangeBase)mNCTransSlider).ValueChanged += NCTransparencySlider_ValueChanged;
			break;
		case 33:
			mButtonsGrid = (StackPanel)target;
			((FrameworkElement)mButtonsGrid).Loaded += new RoutedEventHandler(mButtonsGrid_Loaded);
			break;
		case 34:
			mRevertBtn = (CustomButton)target;
			((ButtonBase)mRevertBtn).Click += new RoutedEventHandler(RevertBtn_Click);
			break;
		case 35:
			mUndoBtn = (CustomButton)target;
			((ButtonBase)mUndoBtn).Click += new RoutedEventHandler(UndoButton_Click);
			break;
		case 36:
			mSaveBtn = (CustomButton)target;
			((ButtonBase)mSaveBtn).Click += new RoutedEventHandler(SaveButton_Click);
			break;
		case 37:
			mCanvas = (Canvas)target;
			((UIElement)mCanvas).PreviewMouseMove += new MouseEventHandler(mCanvas_PreviewMouseMove);
			((UIElement)mCanvas).PreviewMouseUp += new MouseButtonEventHandler(mCanvas_MouseUp);
			break;
		case 38:
			mOverlayGrid = (Grid)target;
			break;
		case 39:
			KeySequenceScriptGrid = (Grid)target;
			break;
		case 40:
			mScriptHeaderGrid = (Grid)target;
			((UIElement)mScriptHeaderGrid).MouseLeftButtonDown += new MouseButtonEventHandler(TopBar_MouseLeftButtonDown);
			break;
		case 41:
			mHeaderText = (TextBlock)target;
			break;
		case 42:
			mCloseScriptWindow = (CustomPictureBox)target;
			((UIElement)mCloseScriptWindow).MouseDown += new MouseButtonEventHandler(CustomPictureBox_MouseDown);
			((UIElement)mCloseScriptWindow).MouseLeftButtonUp += new MouseButtonEventHandler(mCloseScriptButton_MouseLeftButtonUp);
			break;
		case 43:
			mSubheadingText = (TextBlock)target;
			break;
		case 44:
			mScriptText = (CustomTextBox)target;
			break;
		case 45:
			mXYCurrentCoordinatesText = (TextBlock)target;
			break;
		case 46:
			((Hyperlink)target).Click += new RoutedEventHandler(ShowHelpHyperlink_Click);
			break;
		case 47:
			mShowHelpHyperlink = (TextBlock)target;
			break;
		case 48:
			mFooterGrid = (Grid)target;
			break;
		case 49:
			mFooterText = (TextBlock)target;
			break;
		case 50:
			mKeySeqDoneButton = (CustomButton)target;
			((UIElement)mKeySeqDoneButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mDoneScriptButton_MouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
