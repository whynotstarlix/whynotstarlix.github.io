using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class CanvasElement : UserControl, IComponentConnector
{
	internal bool mIsLoadingfromFile;

	internal Point Center;

	private Point? mMousePointForTap;

	private KeymapCanvasWindow mParentWindow;

	private MainWindow ParentWindow;

	private List<Key> mKeyList = new List<Key>();

	internal Dictionary<Positions, Tuple<string, TextBox, TextBlock, List<IMAction>>> dictTextElemets = new Dictionary<Positions, Tuple<string, TextBox, TextBlock, List<IMAction>>>();

	internal static Dictionary<string, CanvasElement> dictPoints = new Dictionary<string, CanvasElement>();

	private KeyActionType mType;

	internal double TopOnClick;

	internal double LeftOnClick;

	internal static object sFocusedTextBox;

	internal double mXPosition;

	internal double mYPosition;

	private MOBASkillSettingsPopup mMOBASkillSettingsPopup;

	private MOBAOtherSettingsMoreInfoPopup mMOBAOtherSettingsMoreInfoPopup;

	private SkillIconToolTipPopup mSkillIconToolTipPopup;

	private MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CanvasElement mCanvasElement;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mToggleModeGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mToggleMode1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mToggleImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mToggleMode2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCanvasGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mKeyRepeatGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCountText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mActionIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mActionIcon2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mResizeIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSkillImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mColumn0;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mColumn1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mColumn2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mColumn3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mColumn4;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RowDefinition mRow0;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RowDefinition mRow1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RowDefinition mRow2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RowDefinition mRow3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RowDefinition mRow4;

	private bool _contentLoaded;

	public List<IMAction> ListActionItem { get; } = new List<IMAction>();

	public KeyActionType ActionType
	{
		get
		{
			return mType;
		}
		set
		{
			mType = value;
			SetActiveImage(isActive: false);
		}
	}

	internal MOBASkillSettingsPopup MOBASkillSettingsPopup
	{
		get
		{
			if (mMOBASkillSettingsPopup == null)
			{
				mMOBASkillSettingsPopup = new MOBASkillSettingsPopup(this);
			}
			return mMOBASkillSettingsPopup;
		}
	}

	internal MOBAOtherSettingsMoreInfoPopup MOBAOtherSettingsMoreInfoPopup
	{
		get
		{
			if (mMOBAOtherSettingsMoreInfoPopup == null)
			{
				mMOBAOtherSettingsMoreInfoPopup = new MOBAOtherSettingsMoreInfoPopup(this);
			}
			return mMOBAOtherSettingsMoreInfoPopup;
		}
	}

	private SkillIconToolTipPopup SkillIconToolTipPopup
	{
		get
		{
			if (mSkillIconToolTipPopup == null)
			{
				mSkillIconToolTipPopup = new SkillIconToolTipPopup(this);
			}
			return mSkillIconToolTipPopup;
		}
	}

	internal MOBASkillSettingsMoreInfoPopup MOBASkillSettingsMoreInfoPopup
	{
		get
		{
			if (mMOBASkillSettingsMoreInfoPopup == null)
			{
				mMOBASkillSettingsMoreInfoPopup = new MOBASkillSettingsMoreInfoPopup(this);
			}
			return mMOBASkillSettingsMoreInfoPopup;
		}
	}

	public bool IsRemoveIfEmpty { get; internal set; }

	public CanvasElement(KeymapCanvasWindow window, MainWindow parentWindow)
	{
		mParentWindow = window;
		ParentWindow = parentWindow;
		InitializeComponent();
	}

	internal void AddAction(IMAction action)
	{
		ListActionItem.Add(action);
		ActionType = (KeyActionType)Enum.Parse(typeof(KeyActionType), action.GetType().ToString());
		SetKeysForActions(ListActionItem);
		SetSize(action);
		SetElementLayout(isLoaded: true);
	}

	private void SetKeysForActions(List<IMAction> lst)
	{
		foreach (KeyValuePair<Positions, Tuple<string, TextBox, TextBlock, List<IMAction>>> dictTextElemet in dictTextElemets)
		{
			((UIElement)((Tuple<string, TextBox, TextBlock>)(object)dictTextElemet.Value).Item3).Visibility = (Visibility)2;
		}
		foreach (IMAction item in lst)
		{
			SetKeysForAction(item);
		}
	}

	private void SetKeysForAction(IMAction action)
	{
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		switch (action.Type)
		{
		case KeyActionType.Dpad:
			mColumn0.Width = new GridLength(10.0, (GridUnitType)2);
			mColumn1.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn2.Width = new GridLength(20.0, (GridUnitType)2);
			mColumn3.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn4.Width = new GridLength(10.0, (GridUnitType)2);
			mRow0.Height = new GridLength(15.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(30.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(15.0, (GridUnitType)2);
			SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
			break;
		case KeyActionType.MOBADpad:
			SetKeys(action, "", "", "", "", "");
			break;
		case KeyActionType.MOBASkill:
		{
			MOBASkill mOBASkill = action as MOBASkill;
			SetKeys(action, "KeyActivate", "", "", "", "");
			if (mParentWindow.dictCanvasElement.ContainsKey(action) && mOBASkill.IsCancelSkillEnabled && mParentWindow.dictCanvasElement.ContainsKey(mOBASkill.mMOBASkillCancel))
			{
				mParentWindow.dictCanvasElement[mOBASkill.mMOBASkillCancel].SetKeysForAction(mOBASkill.mMOBASkillCancel);
			}
			break;
		}
		case KeyActionType.Pan:
		{
			Pan pan = action as Pan;
			SetKeys(action, "KeyStartStop", "", "", "", "");
			if (mParentWindow.dictCanvasElement.ContainsKey(action) && pan.IsLookAroundEnabled && mParentWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
			{
				mParentWindow.dictCanvasElement[pan.mLookAround].SetKeysForAction(pan.mLookAround);
			}
			break;
		}
		case KeyActionType.Swipe:
			mColumn0.Width = new GridLength(0.0, (GridUnitType)2);
			mColumn1.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn2.Width = new GridLength(40.0, (GridUnitType)2);
			mColumn3.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn4.Width = new GridLength(0.0, (GridUnitType)2);
			mRow0.Height = new GridLength(5.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(50.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(5.0, (GridUnitType)2);
			if (action.Direction == Direction.Left)
			{
				SetKeys(action, "", "Key", "", "", "");
			}
			else if (action.Direction == Direction.Right)
			{
				SetKeys(action, "", "", "", "Key", "");
			}
			else if (action.Direction == Direction.Up)
			{
				SetKeys(action, "", "", "Key", "", "");
			}
			else if (action.Direction == Direction.Down)
			{
				SetKeys(action, "", "", "", "", "Key");
			}
			break;
		case KeyActionType.Tap:
			SetKeys(action, "Key", "", "", "", "");
			break;
		case KeyActionType.State:
			if (KMManager.sIsDeveloperModeOn)
			{
				SetKeys(action, "Key", "", "", "", "");
			}
			break;
		case KeyActionType.TapRepeat:
			mRow1.Height = new GridLength(0.0);
			mRow2.Height = new GridLength(30.0, (GridUnitType)2);
			mRow3.Height = new GridLength(4.0, (GridUnitType)2);
			mCountText.Text = ((TapRepeat)action).Count.ToString(CultureInfo.InvariantCulture);
			((UIElement)mKeyRepeatGrid).Visibility = (Visibility)0;
			SetToggleModeValues(action);
			SetKeys(action, "Key", "", "", "", "");
			break;
		case KeyActionType.Tilt:
			mColumn0.Width = new GridLength(0.0, (GridUnitType)2);
			mColumn1.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn2.Width = new GridLength(40.0, (GridUnitType)2);
			mColumn3.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn4.Width = new GridLength(0.0, (GridUnitType)2);
			mRow0.Height = new GridLength(5.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(50.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(5.0, (GridUnitType)2);
			SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
			break;
		case KeyActionType.Zoom:
			mColumn0.Width = new GridLength(6.0, (GridUnitType)2);
			mColumn1.Width = new GridLength(72.0);
			mColumn2.Width = new GridLength(70.0, (GridUnitType)2);
			mColumn3.Width = new GridLength(30.0);
			mColumn4.Width = new GridLength(6.0, (GridUnitType)2);
			mRow0.Height = new GridLength(80.0, (GridUnitType)2);
			mRow1.Height = new GridLength(35.0);
			mRow2.Height = new GridLength(35.0);
			mRow3.Height = new GridLength(0.0);
			mRow4.Height = new GridLength(80.0, (GridUnitType)2);
			SetKeys(action, "", "KeyOut", "KeyIn", "", "");
			break;
		case KeyActionType.LookAround:
			SetKeys(action, "Key", "", "", "", "");
			break;
		case KeyActionType.PanShoot:
			if (mParentWindow.IsInOverlayMode)
			{
				SetKeys(action, "Key", "", "", "", "");
			}
			break;
		case KeyActionType.MOBASkillCancel:
			SetKeys(action, "Key", "", "", "", "");
			break;
		case KeyActionType.FreeLook:
			SetToggleModeValues(action);
			break;
		case KeyActionType.Script:
			SetKeys(action, "Key", "", "", "", "");
			break;
		case KeyActionType.MouseZoom:
			if (KMManager.sIsDeveloperModeOn)
			{
				mColumn0.Width = new GridLength(6.0, (GridUnitType)2);
				mColumn1.Width = new GridLength(72.0);
				mColumn2.Width = new GridLength(70.0, (GridUnitType)2);
				mColumn3.Width = new GridLength(30.0);
				mColumn4.Width = new GridLength(6.0, (GridUnitType)2);
				mRow0.Height = new GridLength(80.0, (GridUnitType)2);
				mRow1.Height = new GridLength(35.0);
				mRow2.Height = new GridLength(35.0);
				mRow3.Height = new GridLength(0.0);
				mRow4.Height = new GridLength(80.0, (GridUnitType)2);
				SetKeys(action, "Key", "", "", "", "");
			}
			break;
		case KeyActionType.Rotate:
			mColumn0.Width = new GridLength(0.0);
			mColumn1.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn2.Width = new GridLength(40.0, (GridUnitType)2);
			mColumn3.Width = new GridLength(30.0, (GridUnitType)2);
			mColumn4.Width = new GridLength(0.0);
			mRow0.Height = new GridLength(15.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(30.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(15.0, (GridUnitType)2);
			SetKeys(action, "", "KeyAntiClock", "", "KeyClock", "");
			break;
		case KeyActionType.EdgeScroll:
			mRow0.Height = new GridLength(15.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(30.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(15.0, (GridUnitType)2);
			((FrameworkElement)mActionIcon).MinHeight = 60.0;
			((FrameworkElement)mActionIcon).MinWidth = 60.0;
			SetKeys(action, "", "", "", "", "");
			break;
		case KeyActionType.Callback:
			mRow0.Height = new GridLength(15.0, (GridUnitType)2);
			mRow1.Height = new GridLength(20.0, (GridUnitType)2);
			mRow2.Height = new GridLength(30.0, (GridUnitType)2);
			mRow3.Height = new GridLength(20.0, (GridUnitType)2);
			mRow4.Height = new GridLength(15.0, (GridUnitType)2);
			((FrameworkElement)mActionIcon).MinHeight = 60.0;
			((FrameworkElement)mActionIcon).MinWidth = 60.0;
			SetKeys(action, "Id", "", "", "", "");
			break;
		}
	}

	internal void SetToggleModeValues(IMAction action)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mToggleModeGrid).Visibility = (Visibility)0;
		switch (action.Type)
		{
		case KeyActionType.TapRepeat:
			((FrameworkElement)this).MinHeight = 92.0;
			BlueStacksUIBinding.Bind(mToggleMode1, "STRING_TAP_MODE", "");
			BlueStacksUIBinding.Bind(mToggleMode2, "STRING_LONG_PRESS_MODE", "");
			if (((TapRepeat)action).RepeatUntilKeyUp)
			{
				mToggleImage.ImageName = "right_switch";
			}
			else
			{
				mToggleImage.ImageName = "left_switch";
			}
			break;
		case KeyActionType.FreeLook:
			BlueStacksUIBinding.Bind(mToggleMode1, "STRING_KEYBOARD_MODE", "");
			BlueStacksUIBinding.Bind(mToggleMode2, "STRING_MOUSE_MODE", "");
			if (((FreeLook)action).DeviceType == 0)
			{
				((FrameworkElement)this).MinHeight = 182.0;
				mToggleImage.ImageName = "left_switch";
				mColumn0.Width = new GridLength(0.0);
				mColumn1.Width = new GridLength(25.0, (GridUnitType)2);
				mColumn2.Width = new GridLength(40.0, (GridUnitType)2);
				mColumn3.Width = new GridLength(25.0, (GridUnitType)2);
				mColumn4.Width = new GridLength(0.0);
				mRow0.Height = new GridLength(10.0, (GridUnitType)2);
				mRow1.Height = new GridLength(25.0, (GridUnitType)2);
				mRow2.Height = new GridLength(30.0, (GridUnitType)2);
				mRow3.Height = new GridLength(25.0, (GridUnitType)2);
				mRow4.Height = new GridLength(10.0, (GridUnitType)2);
				SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
			}
			else if (((FreeLook)action).DeviceType == 1)
			{
				((FrameworkElement)this).MinHeight = 117.0;
				mToggleImage.ImageName = "right_switch";
				SetKeys(action, "Key", "", "", "", "");
			}
			SetActiveImage();
			break;
		}
	}

	private void InsertScriptSettingsClickGrid()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		Grid val = new Grid
		{
			Height = 19.0,
			Width = 19.0,
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)2,
			Margin = new Thickness(30.0, 0.0, 0.0, 3.0),
			Background = (Brush)(object)Brushes.Black,
			Opacity = 0.0001
		};
		Grid.SetRow((UIElement)(object)val, 3);
		Grid.SetRowSpan((UIElement)(object)val, 2);
		Grid.SetColumn((UIElement)(object)val, 3);
		((UIElement)val).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ScriptSettingsGrid_MouseLeftButtonUp);
		((UIElement)val).MouseEnter += new MouseEventHandler(ScriptSettingsGrid_MouseEnter);
		((UIElement)val).MouseLeave += new MouseEventHandler(ScriptSettingsGrid_MouseLeave);
		((Panel)mGrid).Children.Add((UIElement)(object)val);
	}

	private void ScriptSettingsGrid_MouseLeave(object sender, MouseEventArgs e)
	{
		string text = ActionType.ToString();
		mActionIcon.ImageName = text + "_canvas";
	}

	private void ScriptSettingsGrid_MouseEnter(object sender, MouseEventArgs e)
	{
		string text = ActionType.ToString();
		mActionIcon.ImageName = text + "_canvas_hover";
	}

	private static bool CheckForOffsetValueInGameControl(IMAction action)
	{
		bool result = true;
		switch (action.Type)
		{
		case KeyActionType.PanShoot:
			if (string.IsNullOrEmpty(((PanShoot)action).LButtonXOverlayOffset) && string.IsNullOrEmpty(((PanShoot)action).LButtonXOverlayOffset))
			{
				result = false;
			}
			break;
		case KeyActionType.LookAround:
			if (string.IsNullOrEmpty(((LookAround)action).LookAroundXOverlayOffset) && string.IsNullOrEmpty(((LookAround)action).LookAroundYOverlayOffset))
			{
				result = false;
			}
			break;
		case KeyActionType.MOBASkillCancel:
			if (string.IsNullOrEmpty(((MOBASkillCancel)action).MOBASkillCancelOffsetX) && string.IsNullOrEmpty(((MOBASkillCancel)action).MOBASkillCancelOffsetY))
			{
				result = false;
			}
			break;
		default:
			if (string.IsNullOrEmpty(action.XOverlayOffset) && string.IsNullOrEmpty(action.YOverlayOffset))
			{
				return false;
			}
			break;
		}
		return result;
	}

	private void SetKeys(IMAction action, string center, string left, string up, string right, string down)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		if (mParentWindow.IsInOverlayMode && action.Type == KeyActionType.FreeLook)
		{
			ShowOverlayKeysOnImage(center, action, 0, 0);
		}
		if (!mParentWindow.IsInOverlayMode && action.Type == KeyActionType.Script)
		{
			mColumn4.Width = new GridLength(5.0, (GridUnitType)2);
			mRow4.Height = new GridLength(5.0, (GridUnitType)2);
			mColumn0.Width = new GridLength(2.0, (GridUnitType)2);
			InsertScriptSettingsClickGrid();
		}
		string text = string.Empty;
		if (!string.IsNullOrEmpty(center))
		{
			if (!string.IsNullOrEmpty(action[center].ToString()))
			{
				text = KMManager.GetKeyUIValue(action[center].ToString());
			}
			if (mParentWindow.IsInOverlayMode)
			{
				((FrameworkElement)this).MinHeight = 50.0;
				((FrameworkElement)this).MinWidth = 50.0;
				if (!string.IsNullOrEmpty(action[center].ToString()) && !Enumerable.Contains(Constants.ImapGameControlsHiddenInOverlayList, action.Type.ToString()))
				{
					if (CheckForOffsetValueInGameControl(action))
					{
						if (action.Type == KeyActionType.Script)
						{
							GetLabelsForOverlay(Positions.Center, text, action, 3, 3, center);
						}
						else
						{
							GetLabelsForOverlay(Positions.Center, text, action, 2, 2, center);
						}
					}
					else
					{
						if (action.Type == KeyActionType.Tap)
						{
							mColumn4.Width = new GridLength(5.0, (GridUnitType)2);
							mRow4.Height = new GridLength(5.0, (GridUnitType)2);
							mColumn0.Width = new GridLength(5.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.TapRepeat)
						{
							((FrameworkElement)this).MinWidth = 50.0;
							((FrameworkElement)this).MinHeight = 50.0;
							mColumn4.Width = new GridLength(5.0, (GridUnitType)2);
							mRow4.Height = new GridLength(5.0, (GridUnitType)2);
							mRow1.Height = new GridLength(1.0, (GridUnitType)0);
							mRow2.Height = new GridLength(3.0, (GridUnitType)2);
							mRow3.Height = new GridLength(1.0, (GridUnitType)0);
							mColumn0.Width = new GridLength(5.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.MOBASkill)
						{
							mColumn4.Width = new GridLength(5.0, (GridUnitType)2);
							mRow4.Height = new GridLength(5.0, (GridUnitType)2);
							mColumn0.Width = new GridLength(5.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.MOBASkillCancel || action.Type == KeyActionType.Script)
						{
							mColumn4.Width = new GridLength(6.0, (GridUnitType)2);
							mRow4.Height = new GridLength(5.0, (GridUnitType)2);
							mColumn0.Width = new GridLength(6.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.Pan || action.Type == KeyActionType.PanShoot || action.Type == KeyActionType.LookAround)
						{
							mColumn4.Width = new GridLength(5.0, (GridUnitType)2);
							mRow4.Height = new GridLength(5.0, (GridUnitType)2);
							mColumn0.Width = new GridLength(5.0, (GridUnitType)2);
						}
						if (action.Type != KeyActionType.FreeLook)
						{
							GetLabelsForOverlay(Positions.Center, text, action, 4, 4, center);
						}
					}
				}
			}
			else
			{
				TextBlock newTextBlock = GetNewTextBlock(Positions.Center, center, action);
				if (action.Type == KeyActionType.Script)
				{
					Grid.SetColumn((UIElement)(object)newTextBlock, 3);
					Grid.SetRow((UIElement)(object)newTextBlock, 3);
				}
				else
				{
					Grid.SetColumn((UIElement)(object)newTextBlock, 2);
					Grid.SetRow((UIElement)(object)newTextBlock, 2);
				}
				BlueStacksUIBinding.Bind(newTextBlock, KMManager.GetStringsToShowInUI(text.ToString(CultureInfo.InvariantCulture)), "");
				if (action.Type != KeyActionType.MouseZoom && action.Type != KeyActionType.Callback)
				{
					((UIElement)newTextBlock).Visibility = (Visibility)0;
					((FrameworkElement)newTextBlock).ToolTip = newTextBlock.Text;
				}
				else
				{
					((UIElement)newTextBlock).Visibility = (Visibility)2;
				}
			}
		}
		if (!string.IsNullOrEmpty(left))
		{
			if (!string.IsNullOrEmpty(action[left].ToString()))
			{
				text = KMManager.GetKeyUIValue(action[left].ToString());
			}
			if (mParentWindow.IsInOverlayMode)
			{
				if (!string.IsNullOrEmpty(action[left].ToString()) && !Enumerable.Contains(Constants.ImapGameControlsHiddenInOverlayList, action.Type.ToString()))
				{
					if (CheckForOffsetValueInGameControl(action))
					{
						GetLabelsForOverlay(Positions.Center, text, action, 2, 1, left);
					}
					else
					{
						if (action.Type == KeyActionType.Dpad)
						{
							mColumn1.Width = new GridLength(50.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.Rotate)
						{
							mColumn1.Width = new GridLength(60.0, (GridUnitType)2);
							ShowRotateControlOverlay(2, 1, 16.0, "overlay_left_arrow");
						}
						if (action.Type == KeyActionType.FreeLook)
						{
							mColumn2.Width = new GridLength(30.0, (GridUnitType)2);
							mColumn3.Width = new GridLength(40.0, (GridUnitType)2);
							GetLabelsForOverlay(Positions.Left, text, action, 2, 2, left);
						}
						else
						{
							GetLabelsForOverlay(Positions.Left, text, action, 2, 1, left);
						}
					}
				}
			}
			else
			{
				TextBlock newTextBlock2 = GetNewTextBlock(Positions.Left, left, action);
				if (action.Type == KeyActionType.Zoom)
				{
					Grid.SetColumn((UIElement)(object)newTextBlock2, 2);
					Grid.SetRow((UIElement)(object)newTextBlock2, 2);
				}
				else
				{
					Grid.SetColumn((UIElement)(object)newTextBlock2, 1);
					Grid.SetRow((UIElement)(object)newTextBlock2, 2);
				}
				BlueStacksUIBinding.Bind(newTextBlock2, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[left].ToString()), "");
				((UIElement)newTextBlock2).Visibility = (Visibility)0;
				((FrameworkElement)newTextBlock2).ToolTip = newTextBlock2.Text;
				if (action.Type == KeyActionType.Zoom)
				{
					BlueStacksUIBinding.BindColor((DependencyObject)(object)newTextBlock2, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
				}
			}
		}
		if (!string.IsNullOrEmpty(up))
		{
			if (!string.IsNullOrEmpty(action[up].ToString()))
			{
				text = KMManager.GetKeyUIValue(action[up].ToString());
			}
			if (mParentWindow.IsInOverlayMode)
			{
				if (!string.IsNullOrEmpty(action[up].ToString()) && !Enumerable.Contains(Constants.ImapGameControlsHiddenInOverlayList, action.Type.ToString()))
				{
					if (CheckForOffsetValueInGameControl(action))
					{
						GetLabelsForOverlay(Positions.Center, text, action, 1, 2, up);
					}
					else
					{
						if (action.Type == KeyActionType.Dpad)
						{
							mColumn2.Width = new GridLength(50.0, (GridUnitType)2);
						}
						if (action.Type == KeyActionType.FreeLook)
						{
							GetLabelsForOverlay(Positions.Up, text, action, 1, 3, up);
						}
						else
						{
							GetLabelsForOverlay(Positions.Up, text, action, 1, 2, up);
						}
					}
				}
			}
			else
			{
				TextBlock newTextBlock3 = GetNewTextBlock(Positions.Up, up, action);
				Grid.SetColumn((UIElement)(object)newTextBlock3, 2);
				Grid.SetRow((UIElement)(object)newTextBlock3, 1);
				BlueStacksUIBinding.Bind(newTextBlock3, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[up].ToString()), "");
				((UIElement)newTextBlock3).Visibility = (Visibility)0;
				((FrameworkElement)newTextBlock3).ToolTip = newTextBlock3.Text;
				if (action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom)
				{
					BlueStacksUIBinding.BindColor((DependencyObject)(object)newTextBlock3, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
				}
			}
		}
		if (!string.IsNullOrEmpty(right))
		{
			if (!string.IsNullOrEmpty(action[right].ToString()))
			{
				text = KMManager.GetKeyUIValue(action[right].ToString());
			}
			if (mParentWindow.IsInOverlayMode)
			{
				if (!string.IsNullOrEmpty(action[right].ToString()) && !Enumerable.Contains(Constants.ImapGameControlsHiddenInOverlayList, action.Type.ToString()))
				{
					if (CheckForOffsetValueInGameControl(action))
					{
						GetLabelsForOverlay(Positions.Center, text, action, 2, 3, right);
					}
					else
					{
						if (action.Type == KeyActionType.Dpad)
						{
							mColumn3.Width = new GridLength(50.0, (GridUnitType)2);
						}
						else if (action.Type == KeyActionType.Rotate)
						{
							mColumn3.Width = new GridLength(60.0, (GridUnitType)2);
							ShowRotateControlOverlay(2, 3, -16.0, "overlay_right_arrow");
						}
						if (action.Type == KeyActionType.FreeLook)
						{
							mColumn3.Width = new GridLength(70.0);
							mColumn4.Width = new GridLength(30.0, (GridUnitType)2);
							GetLabelsForOverlay(Positions.Right, text, action, 2, 4, right);
						}
						else
						{
							GetLabelsForOverlay(Positions.Right, text, action, 2, 3, right);
						}
					}
				}
			}
			else
			{
				TextBlock newTextBlock4 = GetNewTextBlock(Positions.Right, right, action);
				Grid.SetColumn((UIElement)(object)newTextBlock4, 3);
				Grid.SetRow((UIElement)(object)newTextBlock4, 2);
				BlueStacksUIBinding.Bind(newTextBlock4, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[right].ToString()), "");
				((FrameworkElement)newTextBlock4).ToolTip = newTextBlock4.Text;
				((UIElement)newTextBlock4).Visibility = (Visibility)0;
			}
		}
		if (!string.IsNullOrEmpty(down))
		{
			if (!string.IsNullOrEmpty(action[down].ToString()))
			{
				text = KMManager.GetKeyUIValue(action[down].ToString());
			}
			if (mParentWindow.IsInOverlayMode)
			{
				if (!string.IsNullOrEmpty(action[down].ToString()) && !Enumerable.Contains(Constants.ImapGameControlsHiddenInOverlayList, action.Type.ToString()))
				{
					if (CheckForOffsetValueInGameControl(action))
					{
						GetLabelsForOverlay(Positions.Center, text, action, 3, 2, down);
					}
					else
					{
						if (action.Type == KeyActionType.Dpad)
						{
							mColumn2.Width = new GridLength(50.0, (GridUnitType)2);
						}
						if (action.Type == KeyActionType.FreeLook)
						{
							GetLabelsForOverlay(Positions.Down, text, action, 3, 3, down);
						}
						else
						{
							GetLabelsForOverlay(Positions.Down, text, action, 3, 2, down);
						}
					}
				}
			}
			else
			{
				TextBlock newTextBlock5 = GetNewTextBlock(Positions.Down, down, action);
				Grid.SetColumn((UIElement)(object)newTextBlock5, 2);
				Grid.SetRow((UIElement)(object)newTextBlock5, 3);
				BlueStacksUIBinding.Bind(newTextBlock5, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[down].ToString()), "");
				((FrameworkElement)newTextBlock5).ToolTip = newTextBlock5.Text;
				((UIElement)newTextBlock5).Visibility = (Visibility)0;
			}
		}
		if (action.Type == KeyActionType.Dpad && (action as Dpad).IsMOBADpadEnabled && mParentWindow.IsInOverlayMode)
		{
			((UIElement)mGrid).Visibility = (Visibility)0;
			mColumn2.Width = new GridLength(70.0, (GridUnitType)2);
			mRow2.Height = new GridLength(70.0, (GridUnitType)2);
			GetLabelsForOverlay(Positions.Center, string.Empty, action, 2, 2, center);
		}
		if (mParentWindow.IsInOverlayMode)
		{
			((UIElement)mCanvasGrid).Visibility = (Visibility)2;
			((UIElement)mToggleModeGrid).Visibility = (Visibility)2;
			return;
		}
		((UIElement)mCanvasGrid).Visibility = (Visibility)0;
		if (action.Type == KeyActionType.TapRepeat || action.Type == KeyActionType.FreeLook)
		{
			((UIElement)mToggleModeGrid).Visibility = (Visibility)0;
		}
	}

	private void ShowRotateControlOverlay(int row, int column, double margin, string imageName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Grid val = new Grid
		{
			MinHeight = 50.0,
			MinWidth = 50.0,
			Visibility = (Visibility)0,
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)1
		};
		CustomPictureBox val2 = new CustomPictureBox
		{
			Visibility = (Visibility)0,
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		val2.ImageName = imageName;
		((Panel)val).Children.Add((UIElement)(object)val2);
		((FrameworkElement)val).Margin = new Thickness(margin, 0.0, 0.0, 0.0);
		Grid.SetRow((UIElement)(object)val, row);
		Grid.SetColumn((UIElement)(object)val, column);
		((Panel)mGrid).Children.Add((UIElement)(object)val);
	}

	private void ShowOverlayKeysOnImage(string s, IMAction action, int row, int column)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Grid val = new Grid
		{
			MinHeight = 50.0,
			MinWidth = 60.0,
			Visibility = (Visibility)0,
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)1
		};
		CustomPictureBox val2 = new CustomPictureBox
		{
			Visibility = (Visibility)0,
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		if (((FreeLook)action).DeviceType == 0)
		{
			((FrameworkElement)val2).Height = 196.0;
			((FrameworkElement)val2).Width = 98.0;
			val2.ImageName = "overlay_keyboard";
			((FrameworkElement)val2).Margin = new Thickness(0.0, 6.0, 0.0, 0.0);
			((Panel)val).Children.Add((UIElement)(object)val2);
			Grid.SetRow((UIElement)(object)val, row);
			Grid.SetColumn((UIElement)(object)val, 1);
			Grid.SetRowSpan((UIElement)(object)val, 5);
			Grid.SetColumnSpan((UIElement)(object)val, 5);
		}
		else
		{
			val2.ImageName = "overlay_mouse";
			((FrameworkElement)val2).HorizontalAlignment = (HorizontalAlignment)3;
			Label val3 = new Label();
			BlueStacksUIBinding.Bind(val3, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[s].ToString()));
			((FrameworkElement)val3).Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
			((Control)val3).Padding = new Thickness(2.0);
			((Control)val3).FontSize = 11.0;
			((Control)val3).Foreground = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
			((Control)val3).FontWeight = FontWeights.DemiBold;
			((FrameworkElement)val3).HorizontalAlignment = (HorizontalAlignment)1;
			((FrameworkElement)val3).VerticalAlignment = (VerticalAlignment)1;
			Typeface val4 = new Typeface(((Control)val3).FontFamily, ((Control)val3).FontStyle, ((Control)val3).FontWeight, ((Control)val3).FontStretch);
			FormattedText val5 = new FormattedText(((ContentControl)val3).Content.ToString(), Thread.CurrentThread.CurrentCulture, ((FrameworkElement)val3).FlowDirection, val4, ((Control)val3).FontSize, ((Control)val3).Foreground);
			((FrameworkElement)val2).Width = ((val5.WidthIncludingTrailingWhitespace + 10.0 > 40.0) ? (val5.WidthIncludingTrailingWhitespace + 10.0) : 40.0);
			((FrameworkElement)val2).Height = ((((FrameworkElement)val2).Width > 50.0) ? 50.0 : ((FrameworkElement)val2).Width);
			((Panel)val).Children.Add((UIElement)(object)val2);
			((Panel)val).Children.Add((UIElement)(object)val3);
			Grid.SetRow((UIElement)(object)val, 2);
			Grid.SetRowSpan((UIElement)(object)val, 4);
			Grid.SetColumn((UIElement)(object)val, column);
		}
		((Panel)mGrid).Children.Add((UIElement)(object)val);
	}

	private void GetLabelsForOverlay(Positions p, string text, IMAction action, int row, int column, string key)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		if (ShowImagesOnOverlay.ListShowImagesForKeys.Contains(action[key].ToString()))
		{
			Grid val = new Grid
			{
				MinHeight = 29.0,
				MinWidth = 29.0,
				Visibility = (Visibility)0,
				HorizontalAlignment = (HorizontalAlignment)1,
				VerticalAlignment = (VerticalAlignment)1
			};
			CustomPictureBox val2 = new CustomPictureBox
			{
				Height = 27.0,
				Width = 27.0,
				Visibility = (Visibility)0,
				HorizontalAlignment = (HorizontalAlignment)1,
				VerticalAlignment = (VerticalAlignment)1,
				ImageName = "Imap_" + action[key].ToString()
			};
			((Panel)val).Children.Add((UIElement)(object)val2);
			((Panel)mGrid).Children.Add((UIElement)(object)val);
			Grid.SetRow((UIElement)(object)val, row);
			Grid.SetColumn((UIElement)(object)val, column);
		}
		else if (action.Type == KeyActionType.Dpad || action.Type == KeyActionType.MOBASkill || action.Type == KeyActionType.FreeLook || action.Type == KeyActionType.Rotate)
		{
			if (action.Type == KeyActionType.Dpad)
			{
				if ((action as Dpad).IsMOBADpadEnabled)
				{
					Grid val3 = new Grid
					{
						MinHeight = 67.0,
						MinWidth = 67.0,
						Visibility = (Visibility)0,
						HorizontalAlignment = (HorizontalAlignment)1,
						VerticalAlignment = (VerticalAlignment)1
					};
					CustomPictureBox val4 = new CustomPictureBox
					{
						Height = 67.0,
						Width = ((FrameworkElement)val3).Width,
						Visibility = (Visibility)0,
						HorizontalAlignment = (HorizontalAlignment)1,
						VerticalAlignment = (VerticalAlignment)1,
						ImageName = "Imap_MOBADpad"
					};
					((Panel)val3).Children.Add((UIElement)(object)val4);
					((Panel)mGrid).Children.Add((UIElement)(object)val3);
					Grid.SetRow((UIElement)(object)val3, row);
					Grid.SetColumn((UIElement)(object)val3, column);
				}
				else
				{
					Grid labelGrid = GetLabelGrid(text);
					Grid.SetRow((UIElement)(object)labelGrid, row);
					Grid.SetColumn((UIElement)(object)labelGrid, column);
				}
			}
			else
			{
				Grid labelGrid2 = GetLabelGrid(text);
				Grid.SetRow((UIElement)(object)labelGrid2, row);
				Grid.SetColumn((UIElement)(object)labelGrid2, column);
				if (p.Equals(Positions.Up))
				{
					((FrameworkElement)labelGrid2).Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
				}
				if (p.Equals(Positions.Down))
				{
					((FrameworkElement)labelGrid2).Margin = new Thickness(0.0, -20.0, 0.0, 0.0);
				}
			}
		}
		else
		{
			OverlayLabelControl overlayLabelControl = new OverlayLabelControl();
			BlueStacksUIBinding.Bind(overlayLabelControl.lbl, KMManager.GetStringsToShowInUI(text));
			((FrameworkElement)overlayLabelControl).MinHeight = 27.0;
			((FrameworkElement)overlayLabelControl).MinWidth = 27.0;
			((FrameworkElement)overlayLabelControl.lbl).HorizontalAlignment = (HorizontalAlignment)1;
			((FrameworkElement)overlayLabelControl.lbl).VerticalAlignment = (VerticalAlignment)1;
			((FrameworkElement)overlayLabelControl.lbl).Margin = new Thickness(3.0, 0.0, 3.0, 1.0);
			((Control)overlayLabelControl.lbl).Padding = new Thickness(5.0);
			((Control)overlayLabelControl.lbl).FontSize = 11.0;
			((Panel)mGrid).Children.Add((UIElement)(object)overlayLabelControl);
			Grid.SetRow((UIElement)(object)overlayLabelControl, row);
			Grid.SetColumn((UIElement)(object)overlayLabelControl, column);
		}
	}

	private Grid GetLabelGrid(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		Grid val = new Grid
		{
			MinHeight = 28.0,
			MinWidth = 28.0,
			Visibility = (Visibility)0,
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		Border val2 = new Border();
		Label val3 = new Label();
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Border.BorderBrushProperty, "OverlayLabelBorderColor");
		RenderOptions.SetEdgeMode((DependencyObject)(object)val2, (EdgeMode)0);
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Border.BackgroundProperty, "OverlayLabelBackgroundColor");
		((UIElement)val2).SnapsToDevicePixels = false;
		((UIElement)val2).ClipToBounds = false;
		val2.CornerRadius = new CornerRadius(14.0);
		val2.BorderThickness = new Thickness(1.5);
		BlueStacksUIBinding.Bind(val3, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text));
		((FrameworkElement)val3).Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
		((Control)val3).Padding = new Thickness(1.0);
		((Control)val3).FontSize = 11.0;
		((Control)val3).Foreground = (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
		((Control)val3).FontWeight = FontWeights.Bold;
		((FrameworkElement)val3).HorizontalAlignment = (HorizontalAlignment)1;
		((FrameworkElement)val3).VerticalAlignment = (VerticalAlignment)1;
		((Panel)val).Children.Add((UIElement)(object)val2);
		((Panel)val).Children.Add((UIElement)(object)val3);
		((Panel)mGrid).Children.Add((UIElement)(object)val);
		return val;
	}

	private TextBlock GetNewTextBlock(Positions p, string s, IMAction action)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		TextBlock val;
		if (dictTextElemets.ContainsKey(p))
		{
			dictTextElemets[p].Item4.Add(action);
			val = ((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[p]).Item3;
		}
		else
		{
			val = new TextBlock
			{
				FontSize = 14.0,
				FontWeight = FontWeights.Bold,
				Background = (Brush)(object)Brushes.Transparent,
				TextTrimming = (TextTrimming)1,
				Padding = new Thickness(5.0, 2.0, 5.0, 2.0),
				Foreground = (Brush)(object)Brushes.Black,
				HorizontalAlignment = (HorizontalAlignment)1,
				VerticalAlignment = (VerticalAlignment)1
			};
			((UIElement)val).PreviewMouseUp += new MouseButtonEventHandler(TxtBlock_PreviewMouseUp);
			((FrameworkElement)val).Name = p.ToString();
			TextBox val2 = new TextBox
			{
				FontSize = 14.0,
				FontWeight = FontWeights.Bold
			};
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.BackgroundProperty, "ComboBoxBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.BorderBrushProperty, "ComboBoxBorderColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.ForegroundProperty, "ComboBoxForegroundColor");
			((Control)val2).Padding = new Thickness(0.0, 1.0, 0.0, 1.0);
			((FrameworkElement)val2).HorizontalAlignment = (HorizontalAlignment)1;
			((FrameworkElement)val2).VerticalAlignment = (VerticalAlignment)1;
			val2.TextAlignment = (TextAlignment)2;
			((UIElement)val2).LostFocus += new RoutedEventHandler(TxtBox_LostFocus);
			((UIElement)val2).GotFocus += new RoutedEventHandler(TxtBox_GotFocus);
			((FrameworkElement)val2).MinWidth = 24.0;
			val2.TextWrapping = (TextWrapping)2;
			InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)val2, false);
			((FrameworkElement)val2).Name = p.ToString();
			((UIElement)val2).Visibility = (Visibility)2;
			((UIElement)val2).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectivelyIgnoreMouseButton);
			((UIElement)val2).PreviewMouseDown += new MouseButtonEventHandler(TxtBox_PreviewMouseDown);
			((UIElement)val2).PreviewKeyDown += new KeyEventHandler(TxtBox_PreviewKeyDown);
			((UIElement)val2).KeyUp += new KeyEventHandler(TxtBox_KeyUp);
			((TextBoxBase)val2).TextChanged += new TextChangedEventHandler(TxtBox_TextChanged);
			((UIElement)val2).PreviewMouseWheel += new MouseWheelEventHandler(TxtBox_PreviewMouseWheel);
			((Panel)mGrid).Children.Add((UIElement)(object)val);
			((Panel)mGrid).Children.Add((UIElement)(object)val2);
			dictTextElemets.Add(p, new Tuple<string, TextBox, TextBlock, List<IMAction>>(s, val2, val, new List<IMAction> { action }));
		}
		return val;
	}

	private void TxtBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if (e.Delta > 0)
		{
			((RoutedEventArgs)e).Handled = true;
			((FrameworkElement)val).Tag = "MouseWheelUp";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
		}
		else if (e.Delta < 0)
		{
			((RoutedEventArgs)e).Handled = true;
			((FrameworkElement)val).Tag = "MouseWheelDown";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
		}
	}

	private void TxtBox_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if (ActionType == KeyActionType.Tap || ListActionItem[0].Type == KeyActionType.TapRepeat || ListActionItem[0].Type == KeyActionType.Script)
		{
			if (mKeyList.Count >= 2)
			{
				string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(mKeyList.ElementAt(mKeyList.Count - 1));
				string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(mKeyList.ElementAt(mKeyList.Count - 1));
				val.Text = text;
				((FrameworkElement)val).Tag = tag;
				SetValueHandling(sender);
			}
			else if (mKeyList.Count == 1)
			{
				string text = IMAPKeys.GetStringForUI(mKeyList.ElementAt(0));
				string tag = IMAPKeys.GetStringForFile(mKeyList.ElementAt(0));
				val.Text = text;
				((FrameworkElement)val).Tag = tag;
				SetValueHandling(sender);
			}
			val.CaretIndex = val.Text.Length;
			mKeyList.Clear();
		}
		else
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	internal void SetMousePoint(Point mousePoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		mMousePointForTap = mousePoint;
	}

	private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		SetValueHandling(sender);
	}

	private void SetValueHandling(object sender)
	{
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		Positions key = EnumHelper.Parse<Positions>(((FrameworkElement)val).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
		string item = ((Tuple<string>)(object)dictTextElemets[key]).Item1;
		string value = dictTextElemets[key].Item4[0][item].ToString();
		if (((FrameworkElement)val).Tag != null)
		{
			value = ((FrameworkElement)val).Tag.ToString();
		}
		Setvalue(val, value);
	}

	private void Setvalue(TextBox mKeyTextBox, string value)
	{
		Positions key = EnumHelper.Parse<Positions>(((FrameworkElement)mKeyTextBox).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
		string item = ((Tuple<string>)(object)dictTextElemets[key]).Item1;
		foreach (IMAction item2 in dictTextElemets[key].Item4)
		{
			if (!string.Equals(item2[item].ToString(), value, StringComparison.InvariantCulture))
			{
				item2[item] = value;
				KeymapCanvasWindow.sIsDirty = true;
			}
		}
		if (item.StartsWith("Key", StringComparison.InvariantCulture))
		{
			mKeyTextBox.Text = mKeyTextBox.Text;
		}
	}

	private void TxtBox_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Invalid comparison between Unknown and I4
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Invalid comparison between Unknown and I4
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Invalid comparison between Unknown and I4
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Key == 13)
		{
			return;
		}
		KMManager.CheckAndCreateNewScheme();
		if (IsRemoveIfEmpty)
		{
			IsRemoveIfEmpty = false;
			UpdatePosition(Canvas.GetTop((UIElement)(object)this), Canvas.GetLeft((UIElement)(object)this));
		}
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if (ActionType == KeyActionType.Tap || ActionType == KeyActionType.TapRepeat || ActionType == KeyActionType.Script)
		{
			if ((int)e.Key == 2 || (int)e.SystemKey == 2)
			{
				((FrameworkElement)val).Tag = string.Empty;
				BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + ((FrameworkElement)val).Tag);
			}
			else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
			{
				if ((int)e.SystemKey == 120 || (int)e.SystemKey == 121 || (int)e.SystemKey == 99)
				{
					UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
				}
				else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers != 0)
				{
					if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == 1)
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
					}
					else if ((int)((KeyboardEventArgs)e).KeyboardDevice.Modifiers == 5)
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.SystemKey);
					}
					else
					{
						UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.Key);
					}
				}
				else
				{
					UsefulExtensionMethod.AddIfNotContain<Key>((IList<Key>)mKeyList, e.Key);
				}
			}
		}
		else if ((int)e.Key == 156 && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
		{
			((FrameworkElement)val).Tag = IMAPKeys.GetStringForFile(e.SystemKey);
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
		}
		else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
		{
			((FrameworkElement)val).Tag = IMAPKeys.GetStringForFile(e.Key);
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
		}
		else if ((int)e.Key == 2)
		{
			((FrameworkElement)val).Tag = string.Empty;
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + ((FrameworkElement)val).Tag);
		}
		((RoutedEventArgs)e).Handled = true;
		((UIElement)val).Focus();
		((TextBoxBase)val).SelectAll();
	}

	private void TxtBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Invalid comparison between Unknown and I4
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Invalid comparison between Unknown and I4
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Invalid comparison between Unknown and I4
		if (IsRemoveIfEmpty)
		{
			IsRemoveIfEmpty = false;
			UpdatePosition(Canvas.GetTop((UIElement)(object)this), Canvas.GetLeft((UIElement)(object)this));
		}
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if ((int)((MouseEventArgs)e).MiddleButton == 1)
		{
			((RoutedEventArgs)e).Handled = true;
			KMManager.CheckAndCreateNewScheme();
			((FrameworkElement)val).Tag = "MouseMButton";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseMButton");
		}
		else if ((int)((MouseEventArgs)e).RightButton == 1)
		{
			((RoutedEventArgs)e).Handled = true;
			KMManager.CheckAndCreateNewScheme();
			((FrameworkElement)val).Tag = "MouseRButton";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseRButton");
		}
		else if ((int)((MouseEventArgs)e).XButton1 == 1)
		{
			((RoutedEventArgs)e).Handled = true;
			KMManager.CheckAndCreateNewScheme();
			((FrameworkElement)val).Tag = "MouseXButton1";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseXButton1");
		}
		else if ((int)((MouseEventArgs)e).XButton2 == 1)
		{
			((RoutedEventArgs)e).Handled = true;
			KMManager.CheckAndCreateNewScheme();
			((FrameworkElement)val).Tag = "MouseXButton2";
			BlueStacksUIBinding.Bind(val, Constants.ImapLocaleStringsConstant + "MouseXButton2");
		}
		if ((int)((MouseEventArgs)e).LeftButton == 1 && ((UIElement)val).IsKeyboardFocusWithin)
		{
			((RoutedEventArgs)e).Handled = true;
		}
		((UIElement)val).Focus();
		((TextBoxBase)val).SelectAll();
	}

	private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
	{
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if (val != null && !((UIElement)val).IsKeyboardFocusWithin)
		{
			((RoutedEventArgs)e).Handled = true;
			((UIElement)val).Focus();
		}
	}

	private void TxtBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		if (Math.Abs(TopOnClick - Canvas.GetTop((UIElement)(object)this)) < 2.0 && Math.Abs(LeftOnClick - Canvas.GetLeft((UIElement)(object)this)) < 2.0)
		{
			ShowTextBox(sender);
		}
	}

	internal void ShowTextBox(object sender)
	{
		IMAction iMAction = ListActionItem.FirstOrDefault();
		Positions key = EnumHelper.Parse<Positions>(((FrameworkElement)((sender is TextBlock) ? sender : null)).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
		if (iMAction != null && iMAction.Type != KeyActionType.MouseZoom)
		{
			((UIElement)((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2).Visibility = (Visibility)0;
		}
		((UIElement)((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3).Visibility = (Visibility)2;
		((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2.Text = ((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3.Text;
		Grid.SetColumn((UIElement)(object)((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2, Grid.GetColumn((UIElement)(object)((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3));
		Grid.SetRow((UIElement)(object)((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2, Grid.GetRow((UIElement)(object)((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3));
		MiscUtils.SetFocusAsync((UIElement)(object)((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2, 100);
		if (sFocusedTextBox == null)
		{
			sFocusedTextBox = ((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2;
		}
		if (iMAction != null && iMAction.Type == KeyActionType.MOBASkill)
		{
			((UIElement)mActionIcon).Visibility = (Visibility)0;
		}
	}

	private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
	{
		sFocusedTextBox = sender;
		SetActiveImage();
	}

	internal void TxtBox_LostFocus(object sender, RoutedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)((sender is TextBox) ? sender : null)).Visibility == 0)
		{
			sFocusedTextBox = null;
			Positions key = EnumHelper.Parse<Positions>(((FrameworkElement)((sender is TextBox) ? sender : null)).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
			((UIElement)((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3).Visibility = (Visibility)0;
			((UIElement)((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2).Visibility = (Visibility)2;
			((Tuple<string, TextBox, TextBlock>)(object)dictTextElemets[key]).Item3.Text = ((Tuple<string, TextBox>)(object)dictTextElemets[key]).Item2.Text;
			SetActiveImage(isActive: false);
			if (IsRemoveIfEmpty && !mParentWindow.mIsExtraSettingsPopupOpened)
			{
				Logger.Debug("DELETE_TAP: Empty canvas element deleted");
				DeleteElement();
			}
			if (ListActionItem.First().Type == KeyActionType.MOBASkill)
			{
				((UIElement)mActionIcon).Visibility = (Visibility)2;
			}
		}
	}

	internal void ShowOtherIcons(bool isShow = true)
	{
		IMAction iMAction = ListActionItem.FirstOrDefault();
		if (isShow)
		{
			if (iMAction != null && iMAction.RadiusProperty != -1.0)
			{
				if (KMManager.sIsDeveloperModeOn)
				{
					((UIElement)mResizeIcon).Visibility = (Visibility)0;
				}
				else if (iMAction.Type != KeyActionType.MouseZoom)
				{
					((UIElement)mResizeIcon).Visibility = (Visibility)0;
				}
			}
		}
		else
		{
			((UIElement)mSkillImage).Visibility = (Visibility)1;
			((UIElement)mResizeIcon).Visibility = (Visibility)1;
		}
	}

	private void SetMOBASkillSettingsContent()
	{
		if (!((MOBASkill)ListActionItem.First()).AdvancedMode && !((MOBASkill)ListActionItem.First()).AutocastEnabled)
		{
			((ToggleButton)MOBASkillSettingsPopup.mManualSkill).IsChecked = true;
		}
		else if (((MOBASkill)ListActionItem.First()).AdvancedMode && !((MOBASkill)ListActionItem.First()).AutocastEnabled)
		{
			((ToggleButton)MOBASkillSettingsPopup.mAutoSkill).IsChecked = true;
		}
		else if (((MOBASkill)ListActionItem.First()).AdvancedMode && ((MOBASkill)ListActionItem.First()).AutocastEnabled)
		{
			((ToggleButton)MOBASkillSettingsPopup.mQuickSkill).IsChecked = true;
		}
		((ToggleButton)MOBASkillSettingsPopup.mStopMovementCheckbox).IsChecked = ((MOBASkill)ListActionItem.First()).StopMOBADpad;
		((TextElementCollection<Inline>)(object)((Span)MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink).Inlines).Clear();
		((Span)MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
		string uriString = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=moba_stop_movement_help";
		MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.NavigateUri = new Uri(uriString);
		((TextElementCollection<Inline>)(object)((Span)MOBASkillSettingsMoreInfoPopup.mHyperLink).Inlines).Clear();
		((Span)MOBASkillSettingsMoreInfoPopup.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
		string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=";
		MOBASkillSettingsMoreInfoPopup.mHyperLink.NavigateUri = new Uri(text + "moba_skill_settings_help");
	}

	private void SetSize(IMAction action)
	{
		if (string.IsNullOrEmpty(IMAction.sRadiusPropertyName[action.Type]))
		{
			mActionIcon.IsAlwaysHalfSize = true;
			((FrameworkElement)this).MaxHeight = ((FrameworkElement)mActionIcon).MaxHeight;
		}
	}

	internal void UpdatePosition(double top, double left)
	{
		Canvas mCanvas = mParentWindow.mCanvas;
		double value = (left + ((FrameworkElement)this).ActualWidth / 2.0) / ((FrameworkElement)mCanvas).ActualWidth * 100.0;
		double value2 = (top + ((FrameworkElement)this).ActualHeight / 2.0) / ((FrameworkElement)mCanvas).ActualHeight * 100.0;
		double value3 = ((FrameworkElement)this).ActualWidth / ((FrameworkElement)mCanvas).ActualWidth * 50.0;
		foreach (IMAction item in ListActionItem)
		{
			item.PositionX = Math.Round(value, 2);
			item.PositionY = Math.Round(value2, 2);
			item.RadiusProperty = Math.Round(value3, 2);
		}
	}

	internal static List<CanvasElement> GetCanvasElement(IMAction action, KeymapCanvasWindow window, MainWindow mainWindow)
	{
		List<CanvasElement> list = new List<CanvasElement>();
		object[] customAttributes = action.GetType().GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
		if (customAttributes.Length != 0 && customAttributes[0] is DescriptionAttribute descriptionAttribute)
		{
			if (descriptionAttribute.Description.Contains("Dependent"))
			{
				if (dictPoints.ContainsKey(action.PositionX + "~" + action.PositionY))
				{
					CanvasElement canvasElement = dictPoints[action.PositionX + "~" + action.PositionY];
					canvasElement.AddAction(action);
					list.Add(canvasElement);
				}
				else
				{
					CanvasElement canvasElement2 = new CanvasElement(window, mainWindow);
					canvasElement2.AddAction(action);
					canvasElement2.ShowOtherIcons();
					dictPoints.Add(action.PositionX + "~" + action.PositionY, canvasElement2);
					list.Add(canvasElement2);
				}
			}
			else if (descriptionAttribute.Description.Contains("ParentElement"))
			{
				CanvasElement canvasElement3 = new CanvasElement(window, mainWindow);
				canvasElement3.AddAction(action);
				canvasElement3.ShowOtherIcons();
				list.Add(canvasElement3);
				if (action is Pan)
				{
					Pan pan = action as Pan;
					if (pan.mLookAround != null)
					{
						list.Add(GetCanvasElement(pan.mLookAround, window, mainWindow).First());
					}
					if (pan.mPanShoot != null)
					{
						list.Add(GetCanvasElement(pan.mPanShoot, window, mainWindow).First());
					}
				}
				else if (action is MOBASkill)
				{
					MOBASkill mOBASkill = action as MOBASkill;
					if (mOBASkill.mMOBASkillCancel != null)
					{
						list.Add(GetCanvasElement(mOBASkill.mMOBASkillCancel, window, mainWindow).First());
					}
				}
			}
			else
			{
				CanvasElement canvasElement4 = new CanvasElement(window, mainWindow);
				canvasElement4.AddAction(action);
				canvasElement4.ShowOtherIcons();
				list.Add(canvasElement4);
			}
		}
		return list;
	}

	internal void RemoveAction(string actionItemProperty = "")
	{
		if (ListActionItem.Count > 1)
		{
			ListActionItem.RemoveAt(0);
		}
		else if (((FrameworkElement)this).Parent != null)
		{
			DependencyObject parent = ((FrameworkElement)this).Parent;
			((Panel)((parent is Canvas) ? parent : null)).Children.Remove((UIElement)(object)this);
			IMAction parentAction = ListActionItem.First().ParentAction;
			if (parentAction.Guidance.ContainsKey(actionItemProperty))
			{
				parentAction.Guidance.Remove(actionItemProperty);
			}
		}
	}

	private void CanvasElement_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (!((Popup)MOBASkillSettingsPopup).IsOpen && !((Popup)MOBAOtherSettingsMoreInfoPopup).IsOpen && !((Popup)MOBASkillSettingsMoreInfoPopup).IsOpen)
		{
			OpenPopup();
		}
	}

	internal void OpenPopup()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		List<IMAction> list = new List<IMAction>();
		if (ListActionItem.First().IsChildAction)
		{
			list = new List<IMAction> { ListActionItem.First().ParentAction };
			SetActiveImage();
		}
		else
		{
			list = ListActionItem;
		}
		if (list.Count != 0)
		{
			KeymapExtraSettingWindow keymapExtraSettingWindow = new KeymapExtraSettingWindow(ParentWindow);
			ListExtensions.ClearAddRange<IMAction>(keymapExtraSettingWindow.ListAction, list);
			keymapExtraSettingWindow.mCanvasElement = this;
			((Popup)keymapExtraSettingWindow).Placement = (PlacementMode)1;
			((Popup)keymapExtraSettingWindow).PlacementTarget = (UIElement)(object)this;
			((Popup)keymapExtraSettingWindow).StaysOpen = false;
			keymapExtraSettingWindow.Init();
			mParentWindow.mIsExtraSettingsPopupOpened = true;
			SetActiveImage(isActive: false);
			keymapExtraSettingWindow.IsTopmost = true;
			((Popup)keymapExtraSettingWindow).IsOpen = true;
			Point position = Mouse.GetPosition((IInputElement)(object)this);
			((Popup)keymapExtraSettingWindow).HorizontalOffset = ((Point)(ref position)).X;
			((Popup)keymapExtraSettingWindow).VerticalOffset = ((Point)(ref position)).Y;
			((Popup)keymapExtraSettingWindow).Closed += ExtraSettingPopup_Closed;
		}
	}

	private void SetActiveImage(bool isActive = true)
	{
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		string text = mType.ToString();
		if (mType == KeyActionType.MOBASkill && mParentWindow.IsInOverlayMode)
		{
			text = KeyActionType.Tap.ToString();
		}
		if (mType == KeyActionType.Dpad)
		{
			if (ListActionItem.Count != 0 && (ListActionItem.First() as Dpad).IsMOBADpadEnabled && !mParentWindow.IsInOverlayMode)
			{
				text = "moba_" + text;
				((UIElement)mGrid).Visibility = (Visibility)2;
			}
			else
			{
				((UIElement)mGrid).Visibility = (Visibility)0;
			}
		}
		if (mType == KeyActionType.FreeLook)
		{
			text = (((ListActionItem.First() as FreeLook).DeviceType != 0) ? (text + "_mouse") : (text + "_keyboard"));
		}
		if (isActive)
		{
			mActionIcon.ImageName = text + "_canvas_active";
			mActionIcon2.ImageName = mActionIcon.ImageName + "_2";
		}
		else
		{
			mActionIcon.ImageName = text + "_canvas";
			mActionIcon2.ImageName = mActionIcon.ImageName + "_2";
		}
		((UIElement)mActionIcon).Visibility = (Visibility)0;
		((UIElement)mActionIcon2).Visibility = (Visibility)0;
		if ((mType == KeyActionType.State || mType == KeyActionType.MouseZoom || mType == KeyActionType.Callback) && !KMManager.sIsDeveloperModeOn)
		{
			((UIElement)mCloseIcon).Visibility = (Visibility)2;
			((UIElement)mResizeIcon).Visibility = (Visibility)2;
			((UIElement)mActionIcon).Visibility = (Visibility)2;
			((UIElement)mActionIcon2).Visibility = (Visibility)2;
		}
		if (mType == KeyActionType.Zoom || mType == KeyActionType.MouseZoom)
		{
			((FrameworkElement)mCloseIcon).Margin = new Thickness(-20.0, 20.0, 20.0, -20.0);
			((FrameworkElement)mActionIcon2).Margin = new Thickness(-55.0, 0.0, 0.0, 0.0);
			((FrameworkElement)mResizeIcon).Margin = new Thickness(-20.0, -20.0, 20.0, 20.0);
		}
		if (mType == KeyActionType.MOBASkill)
		{
			((UIElement)mSkillImage).Visibility = (Visibility)0;
			if (((FrameworkElement)this).ActualWidth < 70.0)
			{
				((FrameworkElement)mSkillImage).Margin = new Thickness(-50.0, 30.0, 10.0, 0.0);
			}
			if (sFocusedTextBox == null)
			{
				((UIElement)mActionIcon).Visibility = (Visibility)2;
			}
		}
		if (ListActionItem.Count != 0 && ListActionItem.First().IsChildAction && mParentWindow.dictCanvasElement.ContainsKey(ListActionItem.First().ParentAction))
		{
			mParentWindow.dictCanvasElement[ListActionItem.First().ParentAction].SetActiveImage(isActive);
		}
	}

	internal void ExtraSettingPopup_Closed(object sender, EventArgs e)
	{
		if (mParentWindow.mIsClosing)
		{
			return;
		}
		KeymapExtraSettingWindow obj = sender as KeymapExtraSettingWindow;
		string text = ((ComboBox)obj.mGuidanceCategoryComboBox.mAutoComboBox).Text;
		foreach (IMAction item in obj.ListAction)
		{
			if (!string.Equals(item.GuidanceCategory, text, StringComparison.InvariantCulture))
			{
				item.GuidanceCategory = text;
				KeymapCanvasWindow.sIsDirty = true;
				ParentWindow.SelectedConfig.AddString(item.GuidanceCategory);
			}
		}
		SetKeysForActions(ListActionItem);
		SetActiveImage(isActive: false);
		if (ListActionItem.First().Type == KeyActionType.Zoom || ListActionItem.First().Type == KeyActionType.MouseZoom)
		{
			ListActionItem.First().RadiusProperty = ListActionItem.First().RadiusProperty;
		}
		if (ParentWindow.SelectedConfig.ControlSchemes == null)
		{
			ParentWindow.SelectedConfig.ControlSchemes = new List<IMControlScheme>();
		}
		KMManager.sGamepadDualTextbox = null;
		KMManager.CallGamepadHandler(ParentWindow, "false");
		SetElementLayout();
		mParentWindow.mIsExtraSettingsPopupOpened = false;
	}

	private void Grid_Loaded(object sender, RoutedEventArgs e)
	{
		if (ListActionItem.Count != 0 && mParentWindow != null)
		{
			if (!mParentWindow.IsInOverlayMode)
			{
				SetElementLayout(isLoaded: true);
			}
			else
			{
				SetElementLayout(isLoaded: true, mXPosition, mYPosition);
				if (ListActionItem.First().Type == KeyActionType.Callback)
				{
					KMManager.CanvasWindow.SetOnboardingControlPosition(mXPosition, mYPosition);
				}
			}
		}
		mIsLoadingfromFile = false;
		mMousePointForTap = null;
	}

	internal void SetElementLayout(bool isLoaded = false, double xPos = 0.0, double yPos = 0.0)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		IMAction iMAction = ListActionItem.First();
		if (xPos <= 0.0)
		{
			xPos = iMAction.PositionX;
		}
		if (yPos <= 0.0)
		{
			yPos = iMAction.PositionY;
		}
		double num = (((FrameworkElement)this).Height = ((iMAction.RadiusProperty == -1.0 || (mParentWindow.IsInOverlayMode && iMAction.Type == KeyActionType.MOBASkill)) ? ((FrameworkElement)this).ActualWidth : (((FrameworkElement)this).Width = iMAction.RadiusProperty * 2.0 / 100.0 * ((FrameworkElement)mParentWindow.mCanvas).ActualWidth)));
		if (iMAction.PositionX == -1.0)
		{
			Point val;
			if (mMousePointForTap.HasValue)
			{
				val = mMousePointForTap.Value;
				if (iMAction.Type == KeyActionType.Tap)
				{
					UpdatePosition(((Point)(ref val)).Y, ((Point)(ref val)).X);
				}
			}
			else
			{
				DependencyObject parent = ((FrameworkElement)this).Parent;
				val = Mouse.GetPosition((IInputElement)(object)((parent is IInputElement) ? parent : null));
			}
			if (!isLoaded && (iMAction.Type == KeyActionType.Tilt || iMAction.Type == KeyActionType.State || iMAction.Type == KeyActionType.MouseZoom))
			{
				return;
			}
			if (isLoaded && mIsLoadingfromFile)
			{
				if (iMAction.Type == KeyActionType.Tilt || ((iMAction.Type == KeyActionType.State || iMAction.Type == KeyActionType.MouseZoom) && KMManager.sIsDeveloperModeOn))
				{
					Canvas.SetTop((UIElement)(object)this, 0.0);
					Canvas.SetLeft((UIElement)(object)this, 0.0);
				}
			}
			else
			{
				Canvas.SetTop((UIElement)(object)this, ((Point)(ref val)).Y - num / 2.0);
				Canvas.SetLeft((UIElement)(object)this, ((Point)(ref val)).X - num / 2.0);
			}
			return;
		}
		double num4 = xPos / 100.0 * ((FrameworkElement)mParentWindow.mCanvas).ActualWidth;
		double num5 = yPos / 100.0 * ((FrameworkElement)mParentWindow.mCanvas).ActualHeight;
		num4 = ((num4 < 0.0) ? 0.0 : num4);
		num5 = ((num5 < 0.0) ? 0.0 : num5);
		double num6 = 0.0;
		double num7 = 0.0;
		if (mParentWindow.IsInOverlayMode)
		{
			double num8 = 0.0;
			num8 = ((iMAction.Type == KeyActionType.Dpad) ? (((FrameworkElement)this).ActualWidth - ((FrameworkElement)this).ActualWidth * 0.4) : ((iMAction.Type != KeyActionType.MOBADpad) ? 30.0 : (((FrameworkElement)this).ActualWidth - ((FrameworkElement)this).ActualWidth * 0.5)));
			if (num4 > ((FrameworkElement)mParentWindow.mCanvas).ActualWidth - num8)
			{
				num4 = ((FrameworkElement)mParentWindow.mCanvas).ActualWidth - ((FrameworkElement)this).ActualWidth;
				num6 = num4;
			}
			else
			{
				num6 = num4 - num / 2.0;
			}
			if (num5 > ((FrameworkElement)mParentWindow.mCanvas).ActualHeight - num8)
			{
				num5 = ((FrameworkElement)mParentWindow.mCanvas).ActualHeight - ((FrameworkElement)this).ActualHeight;
				num7 = num5;
			}
			else
			{
				num7 = num5 - num / 2.0;
			}
			Canvas.SetLeft((UIElement)(object)this, num6);
			Canvas.SetTop((UIElement)(object)this, num7);
		}
		else
		{
			num4 = ((num4 > ((FrameworkElement)mParentWindow.mCanvas).ActualWidth) ? ((FrameworkElement)mParentWindow.mCanvas).ActualWidth : num4);
			num5 = ((num5 > ((FrameworkElement)mParentWindow.mCanvas).ActualHeight) ? ((FrameworkElement)mParentWindow.mCanvas).ActualHeight : num5);
			num6 = num4 - num / 2.0;
			num7 = num5 - num / 2.0;
			Canvas.SetLeft((UIElement)(object)this, num6);
			Canvas.SetTop((UIElement)(object)this, num7);
		}
	}

	private void MoveIcon_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!((UIElement)mResizeIcon).IsMouseOver)
		{
			((FrameworkElement)this).Cursor = Cursors.Hand;
		}
	}

	private void MoveIcon_MouseLeave(object sender, MouseEventArgs e)
	{
		if (mParentWindow.mCanvasElement == null)
		{
			((FrameworkElement)this).Cursor = Cursors.Arrow;
		}
		if (ListActionItem.First().Type == KeyActionType.MOBASkill)
		{
			((UIElement)mActionIcon).Visibility = (Visibility)2;
		}
	}

	private void ResizeIcon_MouseEnter(object sender, MouseEventArgs e)
	{
		((FrameworkElement)this).Cursor = Cursors.SizeNWSE;
		((RoutedEventArgs)e).Handled = true;
	}

	private void ResizeIcon_MouseLeave(object sender, MouseEventArgs e)
	{
		if (mParentWindow.mCanvasElement == null)
		{
			((FrameworkElement)this).Cursor = Cursors.Arrow;
			((RoutedEventArgs)e).Handled = true;
			if (((UIElement)this).IsMouseOver)
			{
				((FrameworkElement)this).Cursor = Cursors.Hand;
			}
		}
	}

	private void DeleteIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
		KeymapCanvasWindow.sIsDirty = true;
		KMManager.CheckAndCreateNewScheme();
		DeleteElement();
	}

	private void DeleteElement()
	{
		switch (ListActionItem.First().Type)
		{
		case KeyActionType.MOBASkillCancel:
		{
			if (mParentWindow.dictCanvasElement.ContainsKey(ListActionItem.First()))
			{
				mParentWindow.dictCanvasElement[ListActionItem.First()].RemoveAction("KeyCancel");
				mParentWindow.dictCanvasElement.Remove(ListActionItem.First());
			}
			MOBASkill obj = (ListActionItem.First() as MOBASkillCancel).ParentAction as MOBASkill;
			double cancelX = (obj.CancelY = -1.0);
			obj.CancelX = cancelX;
			return;
		}
		case KeyActionType.LookAround:
		{
			if (mParentWindow.dictCanvasElement.ContainsKey(ListActionItem.First()))
			{
				mParentWindow.dictCanvasElement[ListActionItem.First()].RemoveAction("KeyLookAround");
				mParentWindow.dictCanvasElement.Remove(ListActionItem.First());
			}
			Pan obj3 = (ListActionItem.First() as LookAround).ParentAction as Pan;
			double cancelX = (obj3.LookAroundY = -1.0);
			obj3.LookAroundX = cancelX;
			return;
		}
		case KeyActionType.PanShoot:
		{
			if (mParentWindow.dictCanvasElement.ContainsKey(ListActionItem.First()))
			{
				mParentWindow.dictCanvasElement[ListActionItem.First()].RemoveAction("KeyAction");
				mParentWindow.dictCanvasElement.Remove(ListActionItem.First());
			}
			Pan obj2 = (ListActionItem.First() as PanShoot).ParentAction as Pan;
			double cancelX = (obj2.LButtonY = -1.0);
			obj2.LButtonX = cancelX;
			return;
		}
		case KeyActionType.MOBADpad:
		{
			if (mParentWindow.dictCanvasElement.ContainsKey(ListActionItem.First()))
			{
				mParentWindow.dictCanvasElement[ListActionItem.First()].RemoveAction();
				mParentWindow.dictCanvasElement.Remove(ListActionItem.First());
			}
			ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(ListActionItem.First());
			Dpad dpad = (ListActionItem.First() as MOBADpad).ParentAction as Dpad;
			MOBADpad mMOBADpad = dpad.mMOBADpad;
			double cancelX = (dpad.mMOBADpad.OriginY = -1.0);
			mMOBADpad.OriginX = cancelX;
			mParentWindow.dictCanvasElement[dpad].SetActiveImage(isActive: false);
			return;
		}
		}
		foreach (IMAction item in ListActionItem)
		{
			ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(item);
		}
		if (((FrameworkElement)this).Parent == null)
		{
			return;
		}
		DependencyObject parent = ((FrameworkElement)this).Parent;
		((Panel)((parent is Canvas) ? parent : null)).Children.Remove((UIElement)(object)this);
		foreach (KeyValuePair<IMAction, CanvasElement> item2 in mParentWindow.dictCanvasElement)
		{
			if (item2.Key.ParentAction == ListActionItem.First())
			{
				item2.Value.RemoveAction();
				ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(item2.Value.ListActionItem.First());
			}
		}
	}

	private void UpArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		int num = Convert.ToInt32(mCountText.Text, CultureInfo.InvariantCulture);
		num++;
		mCountText.Text = num.ToString(CultureInfo.InvariantCulture);
		if (ListActionItem.First().Type == KeyActionType.TapRepeat)
		{
			((TapRepeat)ListActionItem.First()).Count = num;
			((TapRepeat)ListActionItem.First()).Delay = 1000 / (2 * num);
		}
	}

	private void DownArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		int num = Convert.ToInt32(mCountText.Text, CultureInfo.InvariantCulture);
		num--;
		if (num <= 1)
		{
			mCountText.Text = "1";
		}
		else
		{
			mCountText.Text = num.ToString(CultureInfo.InvariantCulture);
		}
		if (ListActionItem.First().Type == KeyActionType.TapRepeat)
		{
			((TapRepeat)ListActionItem.First()).Count = Convert.ToInt32(mCountText.Text, CultureInfo.InvariantCulture);
			((TapRepeat)ListActionItem.First()).Delay = 1000 / (2 * ((TapRepeat)ListActionItem.First()).Count);
		}
	}

	private void mToggleImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		try
		{
			if (sFocusedTextBox != null)
			{
				object obj = sFocusedTextBox;
				WpfUtils.FindVisualParent<CanvasElement>((DependencyObject)((obj is DependencyObject) ? obj : null)).TxtBox_LostFocus(sFocusedTextBox, new RoutedEventArgs());
			}
			if (string.Equals(mToggleImage.ImageName, "right_switch", StringComparison.InvariantCulture))
			{
				mToggleImage.ImageName = "left_switch";
				if (ListActionItem.First().Type == KeyActionType.TapRepeat)
				{
					((TapRepeat)ListActionItem.First()).RepeatUntilKeyUp = false;
				}
				else if (ListActionItem.First().Type == KeyActionType.FreeLook)
				{
					((FreeLook)ListActionItem.First()).DeviceType = 0;
					SetKeysForActions(ListActionItem);
				}
			}
			else
			{
				mToggleImage.ImageName = "right_switch";
				if (ListActionItem.First().Type == KeyActionType.TapRepeat)
				{
					((TapRepeat)ListActionItem.First()).RepeatUntilKeyUp = true;
				}
				else if (ListActionItem.First().Type == KeyActionType.FreeLook)
				{
					((FreeLook)ListActionItem.First()).DeviceType = 1;
					SetKeysForActions(ListActionItem);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in toggleMode: " + ex.ToString());
		}
	}

	private void MSkillImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		SetMOBASkillSettingsContent();
		((Popup)SkillIconToolTipPopup).IsOpen = false;
		((Popup)MOBASkillSettingsPopup).IsOpen = true;
		((Popup)MOBAOtherSettingsMoreInfoPopup).IsOpen = false;
		((Popup)MOBASkillSettingsMoreInfoPopup).IsOpen = false;
		ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, "moba_skill_settings_clicked", null, null);
	}

	private void MSkillImage_MouseEnter(object sender, MouseEventArgs e)
	{
		if (ListActionItem.First().Type == KeyActionType.MOBASkill && ((UIElement)mSkillImage).IsEnabled)
		{
			((Popup)SkillIconToolTipPopup).IsOpen = true;
			((Popup)SkillIconToolTipPopup).StaysOpen = true;
		}
	}

	private void MSkillImage_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)SkillIconToolTipPopup).IsOpen = false;
	}

	private void ScriptSettingsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mParentWindow.SidebarWindow.mLastScriptActionItem = ListActionItem;
		mParentWindow.SidebarWindow.ToggleAGCWindowVisiblity(isScriptModeWindow: true);
		ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_edit");
	}

	internal void SendMOBAStats(string action, string skillName = "")
	{
		try
		{
			string item = ((Tuple<string>)(object)dictTextElemets[Positions.Center]).Item1;
			string arg = "";
			if (ListActionItem.First().Guidance.ContainsKey(item))
			{
				arg = ListActionItem.First().Guidance[item];
			}
			ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, action, arg, skillName);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in sending MOBA stats: " + ex.ToString());
		}
	}

	internal void AssignMobaSkill(bool advancedMode, bool autoCastEnabled)
	{
		KMManager.CheckAndCreateNewScheme();
		if (ListActionItem.First().Type == KeyActionType.MOBASkill)
		{
			((MOBASkill)ListActionItem.First()).AdvancedMode = advancedMode;
			((MOBASkill)ListActionItem.First()).AutocastEnabled = autoCastEnabled;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/canvaselement.xaml", UriKind.Relative);
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
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Expected O, but got Unknown
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Expected O, but got Unknown
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Expected O, but got Unknown
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mCanvasElement = (CanvasElement)target;
			((UIElement)mCanvasElement).MouseEnter += new MouseEventHandler(MoveIcon_MouseEnter);
			((UIElement)mCanvasElement).MouseLeave += new MouseEventHandler(MoveIcon_MouseLeave);
			((UIElement)mCanvasElement).PreviewMouseRightButtonUp += new MouseButtonEventHandler(CanvasElement_PreviewMouseRightButtonUp);
			break;
		case 2:
			((FrameworkElement)(Grid)target).Loaded += new RoutedEventHandler(Grid_Loaded);
			break;
		case 3:
			mToggleModeGrid = (Grid)target;
			break;
		case 4:
			mToggleMode1 = (TextBlock)target;
			break;
		case 5:
			mToggleImage = (CustomPictureBox)target;
			((UIElement)mToggleImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mToggleImage_PreviewMouseLeftButtonUp);
			break;
		case 6:
			mToggleMode2 = (TextBlock)target;
			break;
		case 7:
			mCanvasGrid = (Grid)target;
			break;
		case 8:
			mKeyRepeatGrid = (Grid)target;
			break;
		case 9:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(UpArrow_PreviewMouseDown);
			break;
		case 10:
			mCountText = (TextBlock)target;
			break;
		case 11:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DownArrow_PreviewMouseDown);
			break;
		case 12:
			mActionIcon = (CustomPictureBox)target;
			break;
		case 13:
			mActionIcon2 = (CustomPictureBox)target;
			break;
		case 14:
			mCloseIcon = (CustomPictureBox)target;
			((UIElement)mCloseIcon).PreviewMouseDown += new MouseButtonEventHandler(DeleteIcon_PreviewMouseDown);
			break;
		case 15:
			mResizeIcon = (CustomPictureBox)target;
			((UIElement)mResizeIcon).MouseEnter += new MouseEventHandler(ResizeIcon_MouseEnter);
			((UIElement)mResizeIcon).MouseLeave += new MouseEventHandler(ResizeIcon_MouseLeave);
			break;
		case 16:
			mSkillImage = (CustomPictureBox)target;
			((UIElement)mSkillImage).MouseLeftButtonUp += new MouseButtonEventHandler(MSkillImage_MouseLeftButtonUp);
			((UIElement)mSkillImage).MouseEnter += new MouseEventHandler(MSkillImage_MouseEnter);
			((UIElement)mSkillImage).MouseLeave += new MouseEventHandler(MSkillImage_MouseLeave);
			break;
		case 17:
			mGrid = (Grid)target;
			break;
		case 18:
			mColumn0 = (ColumnDefinition)target;
			break;
		case 19:
			mColumn1 = (ColumnDefinition)target;
			break;
		case 20:
			mColumn2 = (ColumnDefinition)target;
			break;
		case 21:
			mColumn3 = (ColumnDefinition)target;
			break;
		case 22:
			mColumn4 = (ColumnDefinition)target;
			break;
		case 23:
			mRow0 = (RowDefinition)target;
			break;
		case 24:
			mRow1 = (RowDefinition)target;
			break;
		case 25:
			mRow2 = (RowDefinition)target;
			break;
		case 26:
			mRow3 = (RowDefinition)target;
			break;
		case 27:
			mRow4 = (RowDefinition)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
