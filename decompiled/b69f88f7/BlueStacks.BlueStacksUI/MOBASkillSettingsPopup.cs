using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MOBASkillSettingsPopup : CustomPopUp, IComponentConnector
{
	private CanvasElement mCanvasElement;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MOBASkillSettingsPopup mMOBASkillSettingsPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHelpIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mQuickSkill;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mAutoSkill;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mManualSkill;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOtherSettingsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mOtherSettingsHelpIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mStopMovementCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path DownArrow;

	private bool _contentLoaded;

	public MOBASkillSettingsPopup(CanvasElement canvasElement)
	{
		mCanvasElement = canvasElement;
		InitializeComponent();
		((Popup)this).PlacementTarget = (UIElement)(object)mCanvasElement?.mSkillImage;
	}

	private void MobaSkillsRadioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		string text = "";
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen = false;
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen = false;
		object obj = ((sender is CustomRadioButton) ? sender : null);
		((ToggleButton)obj).IsChecked = true;
		KeymapCanvasWindow.sIsDirty = true;
		switch (((FrameworkElement)obj).Name)
		{
		case "mManualSkill":
			mCanvasElement.AssignMobaSkill(advancedMode: false, autoCastEnabled: false);
			text = "ManualSkill";
			break;
		case "mAutoSkill":
			mCanvasElement.AssignMobaSkill(advancedMode: true, autoCastEnabled: false);
			text = "AutoSkill";
			break;
		case "mQuickSkill":
			mCanvasElement.AssignMobaSkill(advancedMode: true, autoCastEnabled: true);
			text = "QuickSkill";
			break;
		default:
			mCanvasElement.AssignMobaSkill(advancedMode: true, autoCastEnabled: true);
			text = "QuickSkill";
			break;
		}
		mCanvasElement.SendMOBAStats("moba_skill_changed", text);
	}

	private void MOBASkillSettingsPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mCanvasElement.mSkillImage).IsEnabled = false;
	}

	private void MOBASkillSettingsPopup_Closed(object sender, EventArgs e)
	{
		((UIElement)mCanvasElement.mSkillImage).IsEnabled = true;
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen = false;
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen = false;
	}

	private void HelpIcon_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen = false;
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen = true;
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).StaysOpen = true;
	}

	private void OtherSettingsHelpIcon_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen = false;
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen = true;
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).StaysOpen = true;
	}

	private void StopMovementCheckbox_Checked(object sender, RoutedEventArgs e)
	{
		SetStopMobaDpadValue(isChecked: true);
		mCanvasElement.SendMOBAStats("stop_moba_dpad_checked");
	}

	private void SetStopMobaDpadValue(bool isChecked)
	{
		((Popup)mCanvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen = false;
		((Popup)mCanvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen = false;
		KMManager.CheckAndCreateNewScheme();
		((ToggleButton)mStopMovementCheckbox).IsChecked = isChecked;
		if (mCanvasElement.ListActionItem.First().Type == KeyActionType.MOBASkill)
		{
			((MOBASkill)mCanvasElement.ListActionItem.First()).StopMOBADpad = isChecked;
		}
		KeymapCanvasWindow.sIsDirty = true;
	}

	private void StopMovementCheckbox_Unchecked(object sender, RoutedEventArgs e)
	{
		SetStopMobaDpadValue(isChecked: false);
		mCanvasElement.SendMOBAStats("stop_moba_dpad_unchecked");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingspopup.xaml", UriKind.Relative);
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
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMOBASkillSettingsPopup = (MOBASkillSettingsPopup)target;
			break;
		case 2:
			mBorder = (Border)target;
			break;
		case 3:
			mMaskBorder3 = (Border)target;
			break;
		case 4:
			mHeaderGrid = (Grid)target;
			break;
		case 5:
			mHelpIcon = (CustomPictureBox)target;
			((UIElement)mHelpIcon).MouseEnter += new MouseEventHandler(HelpIcon_MouseEnter);
			break;
		case 6:
			mQuickSkill = (CustomRadioButton)target;
			((UIElement)mQuickSkill).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
			break;
		case 7:
			mAutoSkill = (CustomRadioButton)target;
			((UIElement)mAutoSkill).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
			break;
		case 8:
			mManualSkill = (CustomRadioButton)target;
			((UIElement)mManualSkill).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
			break;
		case 9:
			mOtherSettingsGrid = (Grid)target;
			break;
		case 10:
			mOtherSettingsHelpIcon = (CustomPictureBox)target;
			((UIElement)mOtherSettingsHelpIcon).MouseEnter += new MouseEventHandler(OtherSettingsHelpIcon_MouseEnter);
			break;
		case 11:
			mStopMovementCheckbox = (CustomCheckbox)target;
			((ToggleButton)mStopMovementCheckbox).Checked += new RoutedEventHandler(StopMovementCheckbox_Checked);
			((ToggleButton)mStopMovementCheckbox).Unchecked += new RoutedEventHandler(StopMovementCheckbox_Unchecked);
			break;
		case 12:
			DownArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
