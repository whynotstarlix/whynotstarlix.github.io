using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class KeymapExtraSettingWindow : CustomPopUp, IComponentConnector
{
	private Dictionary<string, GroupBox> mDictGroupBox = new Dictionary<string, GroupBox>();

	private Dictionary<string, DualTextBlockControl> mDictDualTextBox = new Dictionary<string, DualTextBlockControl>();

	private List<string> mListSuggestions = new List<string>();

	internal CanvasElement mCanvasElement;

	private StackPanel mStackPanel;

	private MainWindow ParentWindow;

	private DualTextBlockControl mMOBADpadOriginXDTB;

	private DualTextBlockControl mMOBADpadOriginYDTB;

	private DualTextBlockControl mMOBADpadCharSpeedDTB;

	private DualTextBlockControl mMOBADpadKeyDTB;

	private DualTextBlockControl mMOBADpadXExprDTB;

	private DualTextBlockControl mMOBADpadYExprDTB;

	private DualTextBlockControl mMOBADpadXOverlayOffsetDTB;

	private DualTextBlockControl mMOBADpadYOverlayOffsetDTB;

	private DualTextBlockControl mMOBADpadHeroOriginXExprDTB;

	private DualTextBlockControl mMOBADpadHeroOriginYExprDTB;

	private DualTextBlockControl mLookAroundXDTB;

	private DualTextBlockControl mLookAroundYDTB;

	private DualTextBlockControl mLookAroundDTB;

	private DualTextBlockControl mLookAroundXExprDTB;

	private DualTextBlockControl mLookAroundYExprDTB;

	private DualTextBlockControl mLookAroundXOffsetDTB;

	private DualTextBlockControl mLookAroundYOffsetDTB;

	private DualTextBlockControl mLookAroundShowOnOverlayDTB;

	private DualTextBlockControl mShootXDTB;

	private DualTextBlockControl mShootYDTB;

	private DualTextBlockControl mShootDTB;

	private DualTextBlockControl mShootXExprDTB;

	private DualTextBlockControl mShootYExprDTB;

	private DualTextBlockControl mShootXOffsetDTB;

	private DualTextBlockControl mShootYOffsetDTB;

	private DualTextBlockControl mShootShowOnOverlayDTB;

	private DualTextBlockControl mMOBASkillCancelDTB;

	private DualTextBlockControl mMOBASkillCancelXExprDTB;

	private DualTextBlockControl mMOBASkillCancelYExprDTB;

	private DualTextBlockControl mMOBASkillCancelXOffsetDTB;

	private DualTextBlockControl mMOBASkillCancelYOffsetDTB;

	private DualTextBlockControl mMOBASkillCancelShowOnOverlayDTB;

	private DualTextBlockControl mEnableConditionTB;

	private DualTextBlockControl mStartConditionTB;

	private DualTextBlockControl mNoteTB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomScrollViewer mScrollBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mDeleteButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mMOBAGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMOBAPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mMOBACB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMOBAPB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mGuidanceCategory;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal AutoCompleteComboBox mGuidanceCategoryComboBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mTabsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mKeyboardTabBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock keyboardBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mGamepadTabBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock gamepadBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mMOBASkillCancelGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMOBASkillCancelGBPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mMOBASkillCancelCB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMOBASkillCancelPB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mLookAroundGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mLookAroundPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mLookAroundCB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mLookAroundPB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mShootGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mShootGBPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mShootCB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mShootPB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mSchemesGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mEnableConditionGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mNoteGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mStartConditionGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox mOverlayGB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mOverlayCB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mCanvas;

	private bool _contentLoaded;

	public List<IMAction> ListAction { get; } = new List<IMAction>();

	public KeymapExtraSettingWindow(MainWindow window)
	{
		InitializeComponent();
		base.IsFocusOnMouseClick = true;
		ParentWindow = window;
		object content = ((ContentControl)mScrollBar).Content;
		mStackPanel = (StackPanel)((content is StackPanel) ? content : null);
		AddGuidanceCategories();
		AddDualTextBlockControl();
		SetPopupDraggableProperty();
	}

	private void AddDualTextBlockControl()
	{
		AddDualTextBlockControlToMOBAPanel();
		AddDualTextBlockControlToLookAroundPanel();
		AddDualTextBlockControlToShootGBPanel();
		AddDualTextBlockControlToMOBASkillCancelGBPanel();
		AddDualTextBlockControlToGroupBox();
	}

	private void AddDualTextBlockControlToGroupBox()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		((FrameworkElement)dualTextBlockControl).VerticalAlignment = (VerticalAlignment)0;
		((FrameworkElement)dualTextBlockControl).HorizontalAlignment = (HorizontalAlignment)3;
		((FrameworkElement)dualTextBlockControl).Height = 32.0;
		mEnableConditionTB = dualTextBlockControl;
		((ContentControl)mEnableConditionGB).Content = mEnableConditionTB;
		DualTextBlockControl dualTextBlockControl2 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl2).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		((FrameworkElement)dualTextBlockControl2).VerticalAlignment = (VerticalAlignment)0;
		((FrameworkElement)dualTextBlockControl2).HorizontalAlignment = (HorizontalAlignment)3;
		((FrameworkElement)dualTextBlockControl2).Height = 32.0;
		mNoteTB = dualTextBlockControl2;
		((ContentControl)mNoteGB).Content = mNoteTB;
		DualTextBlockControl dualTextBlockControl3 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl3).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		((FrameworkElement)dualTextBlockControl3).VerticalAlignment = (VerticalAlignment)0;
		((FrameworkElement)dualTextBlockControl3).HorizontalAlignment = (HorizontalAlignment)3;
		((FrameworkElement)dualTextBlockControl3).Height = 32.0;
		mStartConditionTB = dualTextBlockControl3;
		((ContentControl)mStartConditionGB).Content = mStartConditionTB;
	}

	internal void AddDualTextBlockControlToMOBASkillCancelGBPanel()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mMOBASkillCancelDTB = dualTextBlockControl;
		((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelDTB);
		if (KMManager.sIsDeveloperModeOn)
		{
			DualTextBlockControl dualTextBlockControl2 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl2).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBASkillCancelXExprDTB = dualTextBlockControl2;
			DualTextBlockControl dualTextBlockControl3 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl3).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBASkillCancelYExprDTB = dualTextBlockControl3;
			DualTextBlockControl dualTextBlockControl4 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl4).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBASkillCancelXOffsetDTB = dualTextBlockControl4;
			DualTextBlockControl dualTextBlockControl5 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl5).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBASkillCancelYOffsetDTB = dualTextBlockControl5;
			DualTextBlockControl dualTextBlockControl6 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl6).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBASkillCancelShowOnOverlayDTB = dualTextBlockControl6;
			((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelXExprDTB);
			((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelYExprDTB);
			((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelXOffsetDTB);
			((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelYOffsetDTB);
			((Panel)mMOBASkillCancelGBPanel).Children.Add((UIElement)(object)mMOBASkillCancelShowOnOverlayDTB);
		}
	}

	private void AddDualTextBlockControlToShootGBPanel()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mShootXDTB = dualTextBlockControl;
		DualTextBlockControl dualTextBlockControl2 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl2).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mShootYDTB = dualTextBlockControl2;
		DualTextBlockControl dualTextBlockControl3 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl3).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mShootDTB = dualTextBlockControl3;
		((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootXDTB);
		((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootYDTB);
		((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootDTB);
		if (KMManager.sIsDeveloperModeOn)
		{
			DualTextBlockControl dualTextBlockControl4 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl4).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mShootXExprDTB = dualTextBlockControl4;
			DualTextBlockControl dualTextBlockControl5 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl5).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mShootYExprDTB = dualTextBlockControl5;
			DualTextBlockControl dualTextBlockControl6 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl6).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mShootXOffsetDTB = dualTextBlockControl6;
			DualTextBlockControl dualTextBlockControl7 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl7).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mShootYOffsetDTB = dualTextBlockControl7;
			DualTextBlockControl dualTextBlockControl8 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl8).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mShootShowOnOverlayDTB = dualTextBlockControl8;
			((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootXExprDTB);
			((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootYExprDTB);
			((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootXOffsetDTB);
			((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootYOffsetDTB);
			((Panel)mShootGBPanel).Children.Add((UIElement)(object)mShootShowOnOverlayDTB);
		}
	}

	private void AddDualTextBlockControlToLookAroundPanel()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mLookAroundXDTB = dualTextBlockControl;
		DualTextBlockControl dualTextBlockControl2 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl2).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mLookAroundYDTB = dualTextBlockControl2;
		DualTextBlockControl dualTextBlockControl3 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl3).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mLookAroundDTB = dualTextBlockControl3;
		((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundXDTB);
		((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundYDTB);
		((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundDTB);
		if (KMManager.sIsDeveloperModeOn)
		{
			DualTextBlockControl dualTextBlockControl4 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl4).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mLookAroundXExprDTB = dualTextBlockControl4;
			DualTextBlockControl dualTextBlockControl5 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl5).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mLookAroundYExprDTB = dualTextBlockControl5;
			DualTextBlockControl dualTextBlockControl6 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl6).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mLookAroundXOffsetDTB = dualTextBlockControl6;
			DualTextBlockControl dualTextBlockControl7 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl7).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mLookAroundYOffsetDTB = dualTextBlockControl7;
			DualTextBlockControl dualTextBlockControl8 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl8).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mLookAroundShowOnOverlayDTB = dualTextBlockControl8;
			((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundXExprDTB);
			((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundYExprDTB);
			((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundXOffsetDTB);
			((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundYOffsetDTB);
			((Panel)mLookAroundPanel).Children.Add((UIElement)(object)mLookAroundShowOnOverlayDTB);
		}
	}

	private void AddDualTextBlockControlToMOBAPanel()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mMOBADpadOriginXDTB = dualTextBlockControl;
		DualTextBlockControl dualTextBlockControl2 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl2).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mMOBADpadOriginYDTB = dualTextBlockControl2;
		DualTextBlockControl dualTextBlockControl3 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl3).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mMOBADpadCharSpeedDTB = dualTextBlockControl3;
		DualTextBlockControl dualTextBlockControl4 = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl4).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		mMOBADpadKeyDTB = dualTextBlockControl4;
		((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadOriginXDTB);
		((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadOriginYDTB);
		((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadCharSpeedDTB);
		((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadKeyDTB);
		if (KMManager.sIsDeveloperModeOn)
		{
			DualTextBlockControl dualTextBlockControl5 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl5).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadXExprDTB = dualTextBlockControl5;
			DualTextBlockControl dualTextBlockControl6 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl6).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadYExprDTB = dualTextBlockControl6;
			DualTextBlockControl dualTextBlockControl7 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl7).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadXOverlayOffsetDTB = dualTextBlockControl7;
			DualTextBlockControl dualTextBlockControl8 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl8).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadYOverlayOffsetDTB = dualTextBlockControl8;
			DualTextBlockControl dualTextBlockControl9 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl9).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadHeroOriginXExprDTB = dualTextBlockControl9;
			DualTextBlockControl dualTextBlockControl10 = new DualTextBlockControl(ParentWindow);
			((FrameworkElement)dualTextBlockControl10).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			mMOBADpadHeroOriginYExprDTB = dualTextBlockControl10;
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadXExprDTB);
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadYExprDTB);
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadXOverlayOffsetDTB);
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadYOverlayOffsetDTB);
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadHeroOriginXExprDTB);
			((Panel)mMOBAPanel).Children.Add((UIElement)(object)mMOBADpadHeroOriginYExprDTB);
		}
	}

	private void AddControls(DualTextBlockControl control, StackPanel panel)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow);
		((FrameworkElement)dualTextBlockControl).Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		control = dualTextBlockControl;
		((Panel)panel).Children.Add((UIElement)(object)control);
	}

	private void AddGuidanceCategories()
	{
		mListSuggestions.Clear();
		foreach (IMAction gameControl in ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
		{
			string item = ((!string.Equals(gameControl.GuidanceCategory, "MISC", StringComparison.InvariantCulture)) ? ParentWindow.SelectedConfig.GetUIString(gameControl.GuidanceCategory) : LocaleStrings.GetLocalizedString("STRING_" + gameControl.GuidanceCategory, ""));
			if (!mListSuggestions.Contains(item))
			{
				mListSuggestions.Add(item);
			}
		}
	}

	private void AddListOfSuggestions()
	{
		mGuidanceCategoryComboBox.AddSuggestions(mListSuggestions);
	}

	private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		((Popup)this).IsOpen = false;
	}

	private void DeleteButton_Click(object sender, RoutedEventArgs e)
	{
		KMManager.CheckAndCreateNewScheme();
		KeymapCanvasWindow.sIsDirty = true;
		foreach (IMAction item in ListAction)
		{
			ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(item);
		}
		DependencyObject parent = ((FrameworkElement)mCanvasElement).Parent;
		((Panel)((parent is Canvas) ? parent : null)).Children.Remove((UIElement)(object)mCanvasElement);
		((Popup)this).IsOpen = false;
		foreach (KeyValuePair<IMAction, CanvasElement> item2 in KMManager.CanvasWindow.dictCanvasElement)
		{
			if (item2.Key.ParentAction == ListAction.First())
			{
				item2.Value.RemoveAction();
				ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(item2.Value.ListActionItem.First());
			}
		}
	}

	private void SetPopupDraggableProperty()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		try
		{
			Thumb Thumb = new Thumb
			{
				Width = 0.0,
				Height = 0.0
			};
			((Panel)mHeaderGrid).Children.Add((UIElement)(object)Thumb);
			((UIElement)mHeaderGrid).MouseLeftButtonDown -= new MouseButtonEventHandler(mouseDownHandler);
			((UIElement)mHeaderGrid).MouseLeftButtonDown += new MouseButtonEventHandler(mouseDownHandler);
			Thumb.DragDelta -= new DragDeltaEventHandler(deltaEventHandler);
			Thumb.DragDelta += new DragDeltaEventHandler(deltaEventHandler);
			void mouseDownHandler(object o, MouseButtonEventArgs e)
			{
				((UIElement)Thumb).RaiseEvent((RoutedEventArgs)(object)e);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in draggable popup: " + ex.ToString());
		}
		void deltaEventHandler(object o, DragDeltaEventArgs e)
		{
			((Popup)this).HorizontalOffset = ((Popup)this).HorizontalOffset + e.HorizontalChange;
			((Popup)this).VerticalOffset = ((Popup)this).VerticalOffset + e.VerticalChange;
		}
	}

	internal void Init(bool isGamepadTabSelected = false)
	{
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		bool sIsDirty = KeymapCanvasWindow.sIsDirty;
		mDictGroupBox.Clear();
		((Panel)mDummyGrid).Children.Clear();
		((Panel)mStackPanel).Children.Clear();
		mDictDualTextBox.Clear();
		BlueStacksUIBinding.Bind(mHeader, Constants.ImapLocaleStringsConstant + ListAction.First().Type.ToString() + "_Settings", "");
		if (KMManager.sIsDeveloperModeOn)
		{
			((UIElement)mEnableConditionTB).Visibility = (Visibility)0;
			((UIElement)mEnableConditionGB).Visibility = (Visibility)0;
			mEnableConditionTB.ActionItemProperty = "EnableCondition";
			((UIElement)mStartConditionTB).Visibility = (Visibility)0;
			((UIElement)mStartConditionGB).Visibility = (Visibility)0;
			mStartConditionTB.ActionItemProperty = "StartCondition";
			((UIElement)mNoteTB).Visibility = (Visibility)0;
			((UIElement)mNoteGB).Visibility = (Visibility)0;
			mNoteTB.ActionItemProperty = "Note";
		}
		AddListOfSuggestions();
		((Panel)mStackPanel).Children.Add((UIElement)(object)mGuidanceCategory);
		((Panel)mStackPanel).Children.Add((UIElement)(object)mTabsGrid);
		if (isGamepadTabSelected)
		{
			mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
			mKeyboardTabBorder.Background = (Brush)(object)Brushes.Transparent;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
			mGamepadTabBorder.BorderThickness = new Thickness(0.0);
		}
		else
		{
			mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
			mGamepadTabBorder.Background = (Brush)(object)Brushes.Transparent;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
			mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
		}
		foreach (IMAction item in ListAction)
		{
			if (string.Equals(item.GuidanceCategory, "MISC", StringComparison.InvariantCulture))
			{
				((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text = LocaleStrings.GetLocalizedString("STRING_" + item.GuidanceCategory, "");
			}
			else
			{
				((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text = ParentWindow.SelectedConfig.GetUIString(item.GuidanceCategory);
			}
			if (KMManager.sIsDeveloperModeOn)
			{
				((UIElement)mEnableConditionGB).Visibility = (Visibility)0;
				((UIElement)mEnableConditionTB).Visibility = (Visibility)0;
				mEnableConditionTB.AddActionItem(item);
				((UIElement)mNoteGB).Visibility = (Visibility)0;
				((UIElement)mNoteTB).Visibility = (Visibility)0;
				mNoteTB.AddActionItem(item);
				((UIElement)mStartConditionGB).Visibility = (Visibility)0;
				((UIElement)mStartConditionTB).Visibility = (Visibility)0;
				mStartConditionTB.AddActionItem(item);
			}
			else
			{
				((UIElement)mEnableConditionGB).Visibility = (Visibility)2;
				((UIElement)mStartConditionGB).Visibility = (Visibility)2;
				((UIElement)mNoteGB).Visibility = (Visibility)2;
			}
			foreach (KeyValuePair<string, PropertyInfo> item2 in IMAction.DictPopUpUIElements[item.Type])
			{
				if (string.Equals(item2.Key, "IsMOBADpadEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					((Panel)mStackPanel).Children.Add((UIElement)(object)mMOBAGB);
					((ToggleButton)mMOBACB).IsChecked = Convert.ToBoolean(item[item2.Key], CultureInfo.InvariantCulture);
					bool value = ((ToggleButton)mMOBACB).IsChecked.Value;
					((UIElement)mMOBADpadCharSpeedDTB).IsEnabled = value;
					((UIElement)mMOBADpadCharSpeedDTB.mKeyPropertyNameTextBox).IsEnabled = false;
					mMOBADpadCharSpeedDTB.ActionItemProperty = "CharSpeed";
					mMOBADpadCharSpeedDTB.AddActionItem((item as Dpad).mMOBADpad);
					((UIElement)mMOBADpadOriginXDTB).IsEnabled = value;
					((UIElement)mMOBADpadOriginXDTB.mKeyPropertyNameTextBox).IsEnabled = false;
					mMOBADpadOriginXDTB.ActionItemProperty = "OriginX";
					mMOBADpadOriginXDTB.AddActionItem((item as Dpad).mMOBADpad);
					((UIElement)mMOBADpadOriginYDTB).IsEnabled = value;
					((UIElement)mMOBADpadOriginYDTB.mKeyPropertyNameTextBox).IsEnabled = false;
					mMOBADpadOriginYDTB.ActionItemProperty = "OriginY";
					mMOBADpadOriginYDTB.AddActionItem((item as Dpad).mMOBADpad);
					((UIElement)mMOBADpadKeyDTB).IsEnabled = value;
					((UIElement)mMOBADpadKeyDTB.mKeyPropertyNameTextBox).IsEnabled = false;
					((UIElement)mMOBADpadKeyDTB.mKeyTextBox).IsEnabled = false;
					mMOBADpadKeyDTB.ActionItemProperty = "KeyMove";
					mMOBADpadKeyDTB.AddActionItem((item as Dpad).mMOBADpad);
					if (KMManager.sIsDeveloperModeOn)
					{
						((UIElement)mMOBADpadXExprDTB).IsEnabled = value;
						mMOBADpadXExprDTB.ActionItemProperty = "XExpr";
						mMOBADpadXExprDTB.AddActionItem((item as Dpad).mMOBADpad);
						((UIElement)mMOBADpadYExprDTB).IsEnabled = value;
						mMOBADpadYExprDTB.ActionItemProperty = "YExpr";
						mMOBADpadYExprDTB.AddActionItem((item as Dpad).mMOBADpad);
						((UIElement)mMOBADpadXOverlayOffsetDTB).IsEnabled = value;
						mMOBADpadXOverlayOffsetDTB.ActionItemProperty = "XOverlayOffset";
						mMOBADpadXOverlayOffsetDTB.AddActionItem((item as Dpad).mMOBADpad);
						((UIElement)mMOBADpadYOverlayOffsetDTB).IsEnabled = value;
						mMOBADpadYOverlayOffsetDTB.ActionItemProperty = "YOverlayOffset";
						mMOBADpadYOverlayOffsetDTB.AddActionItem((item as Dpad).mMOBADpad);
						((UIElement)mMOBADpadHeroOriginXExprDTB).IsEnabled = value;
						mMOBADpadHeroOriginXExprDTB.ActionItemProperty = "OriginXExpr";
						mMOBADpadHeroOriginXExprDTB.AddActionItem((item as Dpad).mMOBADpad);
						((UIElement)mMOBADpadHeroOriginYExprDTB).IsEnabled = value;
						mMOBADpadHeroOriginYExprDTB.ActionItemProperty = "OriginYExpr";
						mMOBADpadHeroOriginYExprDTB.AddActionItem((item as Dpad).mMOBADpad);
					}
				}
				else if (string.Equals(item2.Key, "IsCancelSkillEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					((Panel)mStackPanel).Children.Add((UIElement)(object)mMOBASkillCancelGB);
					((ToggleButton)mMOBASkillCancelCB).IsChecked = Convert.ToBoolean(item[item2.Key], CultureInfo.InvariantCulture);
					bool value2 = ((ToggleButton)mMOBASkillCancelCB).IsChecked.Value;
					((UIElement)mMOBASkillCancelDTB).IsEnabled = value2;
					if (isGamepadTabSelected)
					{
						mMOBASkillCancelDTB.ActionItemProperty = "KeyCancel_alt1";
					}
					else
					{
						mMOBASkillCancelDTB.ActionItemProperty = "KeyCancel";
					}
					mMOBASkillCancelDTB.AddActionItem(item);
					if (KMManager.sIsDeveloperModeOn)
					{
						((UIElement)mMOBASkillCancelXExprDTB).IsEnabled = value2;
						mMOBASkillCancelXExprDTB.ActionItemProperty = "CancelXExpr";
						mMOBASkillCancelXExprDTB.AddActionItem(item);
						((UIElement)mMOBASkillCancelYExprDTB).IsEnabled = value2;
						mMOBASkillCancelYExprDTB.ActionItemProperty = "CancelYExpr";
						mMOBASkillCancelYExprDTB.AddActionItem(item);
						((UIElement)mMOBASkillCancelXOffsetDTB).IsEnabled = value2;
						mMOBASkillCancelXOffsetDTB.ActionItemProperty = "CancelXOverlayOffset";
						mMOBASkillCancelXOffsetDTB.AddActionItem(item);
						((UIElement)mMOBASkillCancelYOffsetDTB).IsEnabled = value2;
						mMOBASkillCancelYOffsetDTB.ActionItemProperty = "CancelYOverlayOffset";
						mMOBASkillCancelYOffsetDTB.AddActionItem(item);
						((UIElement)mMOBASkillCancelShowOnOverlayDTB).IsEnabled = value2;
						mMOBASkillCancelShowOnOverlayDTB.ActionItemProperty = "CancelShowOnOverlayExpr";
						mMOBASkillCancelShowOnOverlayDTB.AddActionItem(item);
					}
				}
				else if (string.Equals(item2.Key, "IsLookAroundEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					((Panel)mStackPanel).Children.Add((UIElement)(object)mLookAroundGB);
					((ToggleButton)mLookAroundCB).IsChecked = Convert.ToBoolean(item[item2.Key], CultureInfo.InvariantCulture);
					bool value3 = ((ToggleButton)mLookAroundCB).IsChecked.Value;
					SetChildControlsValues(item, mLookAroundDTB, value3, "KeyLookAround");
					SetChildControlsValues(item, mLookAroundXDTB, value3, "LookAroundX");
					SetChildControlsValues(item, mLookAroundYDTB, value3, "LookAroundY");
					if (KMManager.sIsDeveloperModeOn)
					{
						SetChildControlsValues(item, mLookAroundXExprDTB, value3, "LookAroundXExpr");
						SetChildControlsValues(item, mLookAroundYExprDTB, value3, "LookAroundYExpr");
						SetChildControlsValues(item, mLookAroundXOffsetDTB, value3, "LookAroundXOverlayOffset");
						SetChildControlsValues(item, mLookAroundYOffsetDTB, value3, "LookAroundYOverlayOffset");
						SetChildControlsValues(item, mLookAroundShowOnOverlayDTB, value3, "LookAroundShowOnOverlayExpr");
					}
				}
				else if (string.Equals(item2.Key, "IsShootOnClickEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					((Panel)mStackPanel).Children.Add((UIElement)(object)mShootGB);
					((ToggleButton)mShootCB).IsChecked = Convert.ToBoolean(item[item2.Key], CultureInfo.InvariantCulture);
					bool value4 = ((ToggleButton)mShootCB).IsChecked.Value;
					SetChildControlsValues(item, mShootDTB, value4, "KeyAction");
					SetChildControlsValues(item, mShootXDTB, value4, "LButtonX");
					SetChildControlsValues(item, mShootYDTB, value4, "LButtonY");
					if (KMManager.sIsDeveloperModeOn)
					{
						SetChildControlsValues(item, mShootXExprDTB, value4, "LButtonXExpr");
						SetChildControlsValues(item, mShootYExprDTB, value4, "LButtonYExpr");
						SetChildControlsValues(item, mShootXOffsetDTB, value4, "LButtonXOverlayOffset");
						SetChildControlsValues(item, mShootYOffsetDTB, value4, "LButtonYOverlayOffset");
						SetChildControlsValues(item, mShootShowOnOverlayDTB, value4, "LButtonShowOnOverlayExpr");
					}
				}
				else if (string.Equals(item2.Key, "ShowOnOverlay", StringComparison.InvariantCultureIgnoreCase))
				{
					((ToggleButton)mOverlayCB).IsChecked = Convert.ToBoolean(item[item2.Key], CultureInfo.InvariantCulture);
					((FrameworkElement)mOverlayCB).Tag = item2.Key;
					if (!((Panel)mStackPanel).Children.Contains((UIElement)(object)mOverlayGB))
					{
						((Panel)mStackPanel).Children.Add((UIElement)(object)mOverlayGB);
					}
				}
				else if (item.Type == KeyActionType.FreeLook && (string.Equals(item2.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(item2.Key, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(item2.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase)))
				{
					if (((FreeLook)item).DeviceType == 0)
					{
						if (string.Equals(item2.Key, "Speed", StringComparison.InvariantCultureIgnoreCase))
						{
							AddFields(item2, item);
						}
					}
					else if (string.Equals(item2.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(item2.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase))
					{
						AddFields(item2, item);
					}
				}
				else if (isGamepadTabSelected)
				{
					if (item2.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !item2.Key.ToString(CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture))
					{
						AddFields(item2, item);
					}
				}
				else if (!item2.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
				{
					AddFields(item2, item);
				}
			}
			if (!KMManager.sIsDeveloperModeOn)
			{
				continue;
			}
			foreach (KeyValuePair<string, PropertyInfo> item3 in IMAction.sDictDevModeUIElements[item.Type])
			{
				if (isGamepadTabSelected)
				{
					if (item3.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !item3.Key.ToString(CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture))
					{
						AddFields(item3, item);
					}
				}
				else if (!item3.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
				{
					AddFields(item3, item);
				}
			}
		}
		if (KMManager.sIsDeveloperModeOn)
		{
			((Panel)mStackPanel).Children.Add((UIElement)(object)mEnableConditionGB);
			((Panel)mStackPanel).Children.Add((UIElement)(object)mNoteGB);
			((Panel)mStackPanel).Children.Add((UIElement)(object)mStartConditionGB);
		}
		UpdateFieldsForMOBADpad();
		KeymapCanvasWindow.sIsDirty = sIsDirty;
	}

	private static void SetChildControlsValues(IMAction action, DualTextBlockControl control, bool isEnabled, string actionItemProperty)
	{
		((UIElement)control.mKeyPropertyNameTextBox).IsEnabled = isEnabled;
		control.ActionItemProperty = actionItemProperty;
		control.AddActionItem(action);
	}

	private void AddFields(KeyValuePair<string, PropertyInfo> item, IMAction action)
	{
		bool isAddDirectionAttribute = false;
		object[] customAttributes = item.Value.GetCustomAttributes(typeof(CategoryAttribute), inherit: true);
		CategoryAttribute obj = customAttributes[0] as CategoryAttribute;
		string category = obj.Category;
		string text = obj.Category + "~" + item.Key;
		customAttributes = item.Value.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
		if (customAttributes.Length != 0 && (customAttributes[0] as DescriptionAttribute).Description.Contains("NotCommon"))
		{
			text = text + "~" + action.Direction;
			isAddDirectionAttribute = true;
		}
		GroupBox groupBox = GetGroupBox(category);
		if (mDictDualTextBox.ContainsKey(text))
		{
			mDictDualTextBox[text].AddActionItem(action);
			return;
		}
		DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(ParentWindow)
		{
			IsAddDirectionAttribute = isAddDirectionAttribute,
			ActionItemProperty = item.Key
		};
		dualTextBlockControl.AddActionItem(action);
		if (string.Equals(item.Key, "GuidanceCategory", StringComparison.InvariantCultureIgnoreCase))
		{
			((UIElement)dualTextBlockControl.mKeyPropertyNameTextBox).IsEnabled = false;
			((Panel)mStackPanel).Children.Remove((UIElement)(object)groupBox);
			((Panel)mStackPanel).Children.Insert(0, (UIElement)(object)groupBox);
		}
		object content = ((ContentControl)groupBox).Content;
		((Panel)((content is StackPanel) ? content : null)).Children.Add((UIElement)(object)dualTextBlockControl);
		mDictDualTextBox[text] = dualTextBlockControl;
	}

	private GroupBox GetGroupBox(string category)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		GroupBox val;
		if (mDictGroupBox.ContainsKey(category))
		{
			val = mDictGroupBox[category];
		}
		else
		{
			val = new GroupBox();
			mDictGroupBox.Add(category, val);
			((HeaderedContentControl)val).Header = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + category, "");
			((Panel)mStackPanel).Children.Add((UIElement)(object)val);
			((ContentControl)val).Content = (object)new StackPanel();
		}
		return val;
	}

	private void CustomPictureBox_MouseEnter(object sender, MouseEventArgs e)
	{
		((FrameworkElement)this).Cursor = Cursors.Hand;
	}

	private void CustomPictureBox_MouseLeave(object sender, MouseEventArgs e)
	{
		if (KMManager.CanvasWindow.mCanvasElement == null)
		{
			((FrameworkElement)this).Cursor = Cursors.Arrow;
		}
	}

	private void MOBAHeroCB_CheckedChanged(object sender, RoutedEventArgs e)
	{
		if (ListAction.Count > 0)
		{
			KMManager.CheckAndCreateNewScheme();
			DualTextBlockControl dualTextBlockControl = mMOBADpadCharSpeedDTB;
			DualTextBlockControl dualTextBlockControl2 = mMOBADpadOriginXDTB;
			DualTextBlockControl dualTextBlockControl3 = mMOBADpadOriginYDTB;
			bool flag = (((UIElement)mMOBADpadKeyDTB).IsEnabled = ((ToggleButton)mMOBACB).IsChecked.Value);
			bool flag2 = (((UIElement)dualTextBlockControl3).IsEnabled = flag);
			bool isEnabled = (((UIElement)dualTextBlockControl2).IsEnabled = flag2);
			((UIElement)dualTextBlockControl).IsEnabled = isEnabled;
			if (KMManager.sIsDeveloperModeOn)
			{
				DualTextBlockControl dualTextBlockControl4 = mMOBADpadXExprDTB;
				DualTextBlockControl dualTextBlockControl5 = mMOBADpadYExprDTB;
				DualTextBlockControl dualTextBlockControl6 = mMOBADpadXOverlayOffsetDTB;
				DualTextBlockControl dualTextBlockControl7 = mMOBADpadYOverlayOffsetDTB;
				DualTextBlockControl dualTextBlockControl8 = mMOBADpadHeroOriginXExprDTB;
				bool flag5 = (((UIElement)mMOBADpadHeroOriginYExprDTB).IsEnabled = ((ToggleButton)mMOBACB).IsChecked.Value);
				bool flag6 = (((UIElement)dualTextBlockControl8).IsEnabled = flag5);
				flag = (((UIElement)dualTextBlockControl7).IsEnabled = flag6);
				flag2 = (((UIElement)dualTextBlockControl6).IsEnabled = flag);
				isEnabled = (((UIElement)dualTextBlockControl5).IsEnabled = flag2);
				((UIElement)dualTextBlockControl4).IsEnabled = isEnabled;
			}
			((UIElement)mMOBADpadKeyDTB.mKeyTextBox).IsEnabled = false;
			Dpad dpad = ListAction.First() as Dpad;
			if (((ToggleButton)mMOBACB).IsChecked.Value)
			{
				dpad.mMOBADpad.mDpad = dpad;
				dpad.mMOBADpad.ParentAction = dpad;
			}
			else if (dpad.IsMOBADpadEnabled)
			{
				MOBADpad.sListMOBADpad.Remove(dpad.mMOBADpad);
				if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(dpad.mMOBADpad))
				{
					KMManager.CanvasWindow.dictCanvasElement[dpad.mMOBADpad].RemoveAction();
					KMManager.CanvasWindow.dictCanvasElement.Remove(dpad.mMOBADpad);
				}
				ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(dpad.mMOBADpad);
				MOBADpad mMOBADpad = dpad.mMOBADpad;
				double originX = (dpad.mMOBADpad.OriginY = -1.0);
				mMOBADpad.OriginX = originX;
			}
			UpdateFieldsForMOBADpad();
			KeymapCanvasWindow.sIsDirty = true;
		}
		if (mMOBAPB != null)
		{
			((UIElement)mMOBAPB).IsEnabled = ((ToggleButton)mMOBACB).IsChecked.Value;
		}
	}

	private void MOBAHeroPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		((Popup)this).IsOpen = false;
		Dpad dpad = ListAction.First() as Dpad;
		if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(dpad.mMOBADpad))
		{
			CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[dpad.mMOBADpad];
			KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft((UIElement)(object)canvasElement), Canvas.GetTop((UIElement)(object)canvasElement)));
			return;
		}
		dpad.mMOBADpad.X = dpad.X;
		dpad.mMOBADpad.Y = dpad.Y;
		KMManager.CheckAndCreateNewScheme();
		ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(dpad.mMOBADpad);
		AddUIInCanvas(dpad.mMOBADpad);
	}

	private void UpdateFieldsForMOBADpad()
	{
		if (ListAction.First().Type != KeyActionType.Dpad || !((ToggleButton)mMOBACB).IsChecked.HasValue)
		{
			return;
		}
		foreach (KeyValuePair<string, DualTextBlockControl> item in mDictDualTextBox)
		{
			if (item.Key.Contains("~Key"))
			{
				((UIElement)item.Value.mKeyTextBox).IsEnabled = !((ToggleButton)mMOBACB).IsChecked.Value;
				((UIElement)item.Value.mKeyPropertyNameTextBox).IsEnabled = !((ToggleButton)mMOBACB).IsChecked.Value;
				if (!((UIElement)item.Value.mKeyTextBox).IsEnabled)
				{
					item.Value.mKeyPropertyNameTextBox.Text = string.Empty;
					((FrameworkElement)item.Value.mKeyTextBox).Tag = string.Empty;
					item.Value.mKeyTextBox.Text = string.Empty;
				}
			}
		}
	}

	private void MOBASkillCancelCB_CheckedChanged(object sender, RoutedEventArgs e)
	{
		if (ListAction.Count > 0)
		{
			KMManager.CheckAndCreateNewScheme();
			((UIElement)mMOBASkillCancelDTB).IsEnabled = ((ToggleButton)mMOBASkillCancelCB).IsChecked.Value;
			if (KMManager.sIsDeveloperModeOn)
			{
				DualTextBlockControl dualTextBlockControl = mMOBASkillCancelXExprDTB;
				DualTextBlockControl dualTextBlockControl2 = mMOBASkillCancelYExprDTB;
				DualTextBlockControl dualTextBlockControl3 = mMOBASkillCancelYExprDTB;
				DualTextBlockControl dualTextBlockControl4 = mMOBASkillCancelYOffsetDTB;
				bool flag = (((UIElement)mMOBASkillCancelShowOnOverlayDTB).IsEnabled = ((ToggleButton)mMOBASkillCancelCB).IsChecked.Value);
				bool flag2 = (((UIElement)dualTextBlockControl4).IsEnabled = flag);
				bool flag4 = (((UIElement)dualTextBlockControl3).IsEnabled = flag2);
				bool isEnabled = (((UIElement)dualTextBlockControl2).IsEnabled = flag4);
				((UIElement)dualTextBlockControl).IsEnabled = isEnabled;
			}
			MOBASkill mOBASkill = ListAction.First() as MOBASkill;
			if (((ToggleButton)mMOBASkillCancelCB).IsChecked.Value)
			{
				mOBASkill.mMOBASkillCancel = new MOBASkillCancel(mOBASkill);
			}
			else if (mOBASkill.IsCancelSkillEnabled)
			{
				if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(mOBASkill.mMOBASkillCancel))
				{
					KMManager.CanvasWindow.dictCanvasElement[mOBASkill.mMOBASkillCancel].RemoveAction("KeyCancel");
					KMManager.CanvasWindow.dictCanvasElement.Remove(mOBASkill.mMOBASkillCancel);
					mMOBASkillCancelDTB.mKeyPropertyNameTextBox.Text = string.Empty;
				}
				double cancelX = (mOBASkill.CancelY = -1.0);
				mOBASkill.CancelX = cancelX;
			}
			KeymapCanvasWindow.sIsDirty = true;
		}
		if (mMOBASkillCancelPB != null)
		{
			((UIElement)mMOBASkillCancelPB).IsEnabled = ((ToggleButton)mMOBASkillCancelCB).IsChecked.Value;
		}
	}

	private void MOBASkillCancelPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		((Popup)this).IsOpen = false;
		MOBASkill mOBASkill = ListAction.First() as MOBASkill;
		if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(mOBASkill.mMOBASkillCancel))
		{
			CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[mOBASkill.mMOBASkillCancel];
			KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft((UIElement)(object)canvasElement), Canvas.GetTop((UIElement)(object)canvasElement)));
		}
		else
		{
			AddUIInCanvas(mOBASkill.mMOBASkillCancel);
		}
	}

	private void LookAroundCB_CheckedChanged(object sender, RoutedEventArgs e)
	{
		if (ListAction.Count > 0)
		{
			KMManager.CheckAndCreateNewScheme();
			DualTextBlockControl dualTextBlockControl = mLookAroundXDTB;
			DualTextBlockControl dualTextBlockControl2 = mLookAroundYDTB;
			bool flag = (((UIElement)mLookAroundDTB).IsEnabled = ((ToggleButton)mLookAroundCB).IsChecked.Value);
			bool isEnabled = (((UIElement)dualTextBlockControl2).IsEnabled = flag);
			((UIElement)dualTextBlockControl).IsEnabled = isEnabled;
			if (KMManager.sIsDeveloperModeOn)
			{
				DualTextBlockControl dualTextBlockControl3 = mLookAroundXExprDTB;
				DualTextBlockControl dualTextBlockControl4 = mLookAroundYExprDTB;
				DualTextBlockControl dualTextBlockControl5 = mLookAroundXOffsetDTB;
				DualTextBlockControl dualTextBlockControl6 = mLookAroundYOffsetDTB;
				bool flag3 = (((UIElement)mLookAroundShowOnOverlayDTB).IsEnabled = ((ToggleButton)mLookAroundCB).IsChecked.Value);
				bool flag4 = (((UIElement)dualTextBlockControl6).IsEnabled = flag3);
				flag = (((UIElement)dualTextBlockControl5).IsEnabled = flag4);
				isEnabled = (((UIElement)dualTextBlockControl4).IsEnabled = flag);
				((UIElement)dualTextBlockControl3).IsEnabled = isEnabled;
			}
			Pan pan = ListAction.First() as Pan;
			if (((ToggleButton)mLookAroundCB).IsChecked.Value)
			{
				pan.mLookAround = new LookAround(pan);
			}
			else if (pan.IsLookAroundEnabled)
			{
				if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
				{
					KMManager.CanvasWindow.dictCanvasElement[pan.mLookAround].RemoveAction("KeyLookAround");
					KMManager.CanvasWindow.dictCanvasElement.Remove(pan.mLookAround);
					mLookAroundDTB.mKeyPropertyNameTextBox.Text = string.Empty;
				}
				double lookAroundX = (pan.LookAroundY = -1.0);
				pan.LookAroundX = lookAroundX;
			}
			KeymapCanvasWindow.sIsDirty = true;
		}
		if (mLookAroundPB != null)
		{
			((UIElement)mLookAroundPB).IsEnabled = ((ToggleButton)mLookAroundCB).IsChecked.Value;
		}
	}

	private void LookAroundPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		((Popup)this).IsOpen = false;
		Pan pan = ListAction.First() as Pan;
		if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
		{
			CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[pan.mLookAround];
			KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft((UIElement)(object)canvasElement), Canvas.GetTop((UIElement)(object)canvasElement)));
		}
		else
		{
			AddUIInCanvas(pan.mLookAround);
		}
	}

	private void ShootCB_CheckedChanged(object sender, RoutedEventArgs e)
	{
		if (ListAction.Count > 0)
		{
			KMManager.CheckAndCreateNewScheme();
			DualTextBlockControl dualTextBlockControl = mShootXDTB;
			DualTextBlockControl dualTextBlockControl2 = mShootYDTB;
			bool flag = (((UIElement)mShootDTB).IsEnabled = ((ToggleButton)mLookAroundCB).IsChecked.Value);
			bool isEnabled = (((UIElement)dualTextBlockControl2).IsEnabled = flag);
			((UIElement)dualTextBlockControl).IsEnabled = isEnabled;
			if (KMManager.sIsDeveloperModeOn)
			{
				DualTextBlockControl dualTextBlockControl3 = mShootXExprDTB;
				DualTextBlockControl dualTextBlockControl4 = mShootYExprDTB;
				DualTextBlockControl dualTextBlockControl5 = mShootXOffsetDTB;
				DualTextBlockControl dualTextBlockControl6 = mShootYOffsetDTB;
				bool flag3 = (((UIElement)mShootShowOnOverlayDTB).IsEnabled = ((ToggleButton)mShootCB).IsChecked.Value);
				bool flag4 = (((UIElement)dualTextBlockControl6).IsEnabled = flag3);
				flag = (((UIElement)dualTextBlockControl5).IsEnabled = flag4);
				isEnabled = (((UIElement)dualTextBlockControl4).IsEnabled = flag);
				((UIElement)dualTextBlockControl3).IsEnabled = isEnabled;
			}
			Pan pan = ListAction.First() as Pan;
			if (((ToggleButton)mShootCB).IsChecked.Value)
			{
				pan.mPanShoot = new PanShoot(pan);
			}
			else if (pan.IsShootOnClickEnabled)
			{
				if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mPanShoot))
				{
					KMManager.CanvasWindow.dictCanvasElement[pan.mPanShoot].RemoveAction("KeyAction");
					KMManager.CanvasWindow.dictCanvasElement.Remove(pan.mPanShoot);
					mShootDTB.mKeyPropertyNameTextBox.Text = string.Empty;
				}
				double lButtonX = (pan.LButtonY = -1.0);
				pan.LButtonX = lButtonX;
			}
			KeymapCanvasWindow.sIsDirty = true;
		}
		if (mShootPB != null)
		{
			((UIElement)mShootPB).IsEnabled = ((ToggleButton)mShootCB).IsChecked.Value;
		}
	}

	private void ShootPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		((Popup)this).IsOpen = false;
		Pan pan = ListAction.First() as Pan;
		if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mPanShoot))
		{
			CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[pan.mPanShoot];
			KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft((UIElement)(object)canvasElement), Canvas.GetTop((UIElement)(object)canvasElement)));
		}
		else
		{
			AddUIInCanvas(pan.mPanShoot);
		}
	}

	private void mOverlayCB_Checked(object sender, RoutedEventArgs e)
	{
		if (ListAction.Count <= 0)
		{
			return;
		}
		KMManager.CheckAndCreateNewScheme();
		foreach (IMAction item in ListAction)
		{
			item.IsVisibleInOverlay = true;
		}
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void mOverlayCB_Unchecked(object sender, RoutedEventArgs e)
	{
		KMManager.CheckAndCreateNewScheme();
		if (ListAction.Count > 0)
		{
			foreach (IMAction item in ListAction)
			{
				item.IsVisibleInOverlay = false;
			}
		}
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void AddUIInCanvas(IMAction hero)
	{
		((FrameworkElement)this).Cursor = Cursors.Hand;
		KMManager.GetCanvasElement(ParentWindow, hero, mCanvas);
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

	private void mGamepadTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			KMManager.CheckAndCreateNewScheme();
			foreach (IMAction item in ListAction)
			{
				if (!string.Equals(item.GuidanceCategory, ((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text, StringComparison.InvariantCulture))
				{
					item.GuidanceCategory = ((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text;
					KeymapCanvasWindow.sIsDirty = true;
					ParentWindow.SelectedConfig.AddString(item.GuidanceCategory);
				}
			}
			mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
			mGamepadTabBorder.Background = (Brush)(object)Brushes.Transparent;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
			mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
			AddGuidanceCategories();
			Init(isGamepadTabSelected: true);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in switching to Gamepad tab: " + ex.ToString());
		}
	}

	private void mKeyboardTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			KMManager.CheckAndCreateNewScheme();
			foreach (IMAction item in ListAction)
			{
				if (!string.Equals(item.GuidanceCategory, ((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text, StringComparison.InvariantCulture))
				{
					item.GuidanceCategory = ((ComboBox)mGuidanceCategoryComboBox.mAutoComboBox).Text;
					KeymapCanvasWindow.sIsDirty = true;
					ParentWindow.SelectedConfig.AddString(item.GuidanceCategory);
				}
			}
			AddGuidanceCategories();
			mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
			mKeyboardTabBorder.Background = (Brush)(object)Brushes.Transparent;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
			mGamepadTabBorder.BorderThickness = new Thickness(0.0);
			Init();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in switching to Keyboard tab: " + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/keymapextrasettingwindow.xaml", UriKind.Relative);
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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Expected O, but got Unknown
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Expected O, but got Unknown
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Expected O, but got Unknown
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Expected O, but got Unknown
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Expected O, but got Unknown
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Expected O, but got Unknown
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Expected O, but got Unknown
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Expected O, but got Unknown
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Expected O, but got Unknown
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Expected O, but got Unknown
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Expected O, but got Unknown
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Expected O, but got Unknown
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Expected O, but got Unknown
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Expected O, but got Unknown
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Expected O, but got Unknown
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Expected O, but got Unknown
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Expected O, but got Unknown
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Expected O, but got Unknown
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Expected O, but got Unknown
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Expected O, but got Unknown
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Expected O, but got Unknown
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Expected O, but got Unknown
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Expected O, but got Unknown
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Expected O, but got Unknown
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mHeaderGrid = (Grid)target;
			break;
		case 2:
			mHeader = (TextBlock)target;
			break;
		case 3:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(CloseButton_MouseLeftButtonDown);
			break;
		case 4:
			mScrollBar = (CustomScrollViewer)target;
			break;
		case 5:
			mDeleteButton = (CustomButton)target;
			((ButtonBase)mDeleteButton).Click += new RoutedEventHandler(DeleteButton_Click);
			break;
		case 6:
			mDummyGrid = (Grid)target;
			break;
		case 7:
			mMOBAGB = (GroupBox)target;
			break;
		case 8:
			mMOBAPanel = (StackPanel)target;
			break;
		case 9:
			mMOBACB = (CustomCheckbox)target;
			((ToggleButton)mMOBACB).Checked += new RoutedEventHandler(MOBAHeroCB_CheckedChanged);
			((ToggleButton)mMOBACB).Unchecked += new RoutedEventHandler(MOBAHeroCB_CheckedChanged);
			break;
		case 10:
			mMOBAPB = (CustomPictureBox)target;
			((UIElement)mMOBAPB).MouseEnter += new MouseEventHandler(CustomPictureBox_MouseEnter);
			((UIElement)mMOBAPB).MouseLeave += new MouseEventHandler(CustomPictureBox_MouseLeave);
			((UIElement)mMOBAPB).MouseDown += new MouseButtonEventHandler(MOBAHeroPictureBox_MouseDown);
			break;
		case 11:
			mGuidanceCategory = (GroupBox)target;
			break;
		case 12:
			mGuidanceCategoryComboBox = (AutoCompleteComboBox)target;
			break;
		case 13:
			mTabsGrid = (GroupBox)target;
			break;
		case 14:
			mKeyboardTabBorder = (Border)target;
			((UIElement)mKeyboardTabBorder).MouseLeftButtonUp += new MouseButtonEventHandler(mKeyboardTabBorder_MouseLeftButtonUp);
			break;
		case 15:
			keyboardBtn = (TextBlock)target;
			break;
		case 16:
			mGamepadTabBorder = (Border)target;
			((UIElement)mGamepadTabBorder).MouseLeftButtonUp += new MouseButtonEventHandler(mGamepadTabBorder_MouseLeftButtonUp);
			break;
		case 17:
			gamepadBtn = (TextBlock)target;
			break;
		case 18:
			mMOBASkillCancelGB = (GroupBox)target;
			break;
		case 19:
			mMOBASkillCancelGBPanel = (StackPanel)target;
			break;
		case 20:
			mMOBASkillCancelCB = (CustomCheckbox)target;
			((ToggleButton)mMOBASkillCancelCB).Checked += new RoutedEventHandler(MOBASkillCancelCB_CheckedChanged);
			((ToggleButton)mMOBASkillCancelCB).Unchecked += new RoutedEventHandler(MOBASkillCancelCB_CheckedChanged);
			break;
		case 21:
			mMOBASkillCancelPB = (CustomPictureBox)target;
			((UIElement)mMOBASkillCancelPB).MouseEnter += new MouseEventHandler(CustomPictureBox_MouseEnter);
			((UIElement)mMOBASkillCancelPB).MouseLeave += new MouseEventHandler(CustomPictureBox_MouseLeave);
			((UIElement)mMOBASkillCancelPB).MouseDown += new MouseButtonEventHandler(MOBASkillCancelPictureBox_MouseDown);
			break;
		case 22:
			mLookAroundGB = (GroupBox)target;
			break;
		case 23:
			mLookAroundPanel = (StackPanel)target;
			break;
		case 24:
			mLookAroundCB = (CustomCheckbox)target;
			((ToggleButton)mLookAroundCB).Checked += new RoutedEventHandler(LookAroundCB_CheckedChanged);
			((ToggleButton)mLookAroundCB).Unchecked += new RoutedEventHandler(LookAroundCB_CheckedChanged);
			break;
		case 25:
			mLookAroundPB = (CustomPictureBox)target;
			((UIElement)mLookAroundPB).MouseEnter += new MouseEventHandler(CustomPictureBox_MouseEnter);
			((UIElement)mLookAroundPB).MouseLeave += new MouseEventHandler(CustomPictureBox_MouseLeave);
			((UIElement)mLookAroundPB).MouseDown += new MouseButtonEventHandler(LookAroundPictureBox_MouseDown);
			break;
		case 26:
			mShootGB = (GroupBox)target;
			break;
		case 27:
			mShootGBPanel = (StackPanel)target;
			break;
		case 28:
			mShootCB = (CustomCheckbox)target;
			((ToggleButton)mShootCB).Checked += new RoutedEventHandler(ShootCB_CheckedChanged);
			((ToggleButton)mShootCB).Unchecked += new RoutedEventHandler(ShootCB_CheckedChanged);
			break;
		case 29:
			mShootPB = (CustomPictureBox)target;
			((UIElement)mShootPB).MouseEnter += new MouseEventHandler(CustomPictureBox_MouseEnter);
			((UIElement)mShootPB).MouseLeave += new MouseEventHandler(CustomPictureBox_MouseLeave);
			((UIElement)mShootPB).MouseDown += new MouseButtonEventHandler(ShootPictureBox_MouseDown);
			break;
		case 30:
			mSchemesGB = (GroupBox)target;
			break;
		case 31:
			mEnableConditionGB = (GroupBox)target;
			break;
		case 32:
			mNoteGB = (GroupBox)target;
			break;
		case 33:
			mStartConditionGB = (GroupBox)target;
			break;
		case 34:
			mOverlayGB = (GroupBox)target;
			break;
		case 35:
			mOverlayCB = (CustomCheckbox)target;
			((ToggleButton)mOverlayCB).Checked += new RoutedEventHandler(mOverlayCB_Checked);
			((ToggleButton)mOverlayCB).Unchecked += new RoutedEventHandler(mOverlayCB_Unchecked);
			break;
		case 36:
			mCanvas = (Canvas)target;
			((UIElement)mCanvas).PreviewMouseMove += new MouseEventHandler(mCanvas_PreviewMouseMove);
			((UIElement)mCanvas).MouseUp += new MouseButtonEventHandler(mCanvas_MouseUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
