using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class GuidanceWindow : CustomWindow, IComponentConnector
{
	internal MainWindow ParentWindow;

	private bool mIsGuidanceVideoPresent;

	private CustomToastPopupControl mToastPopup;

	private List<string> lstPanTags = new List<string>();

	private List<string> lstMOBATags = new List<string>();

	internal bool mIsGamePadTabSelected;

	internal bool mIsOnboardingPopupToBeShownOnGuidanceClose;

	private int mSidebarWidth = 220;

	internal double mGuidanceWindowLeft = -1.0;

	internal double mGuidanceWindowTop = -1.0;

	internal bool mGuidanceHasBeenMoved;

	internal static bool IsDirty;

	private GuidanceData mGuidanceData = new GuidanceData();

	private string mHelpArticleUrl;

	private bool isViewState = true;

	private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGuidanceMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mGameControlBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DockPanel mHeaderGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mControlsTab;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mControlsTabTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mEditKeysGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mEditKeysGridTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseSideBarWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mControlsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mSchemePanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSchemeTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mSchemesComboBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mVideoBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mVideoThumbnail;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mHowToPlayGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mHowToPlayCollapseExpand;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mQuickLearnBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mVideoTutorialBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mReadArticleBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DockPanel mKeysIconGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mKeyboardIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mKeyboardIconImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mKeyboardIconSeparator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGamepadIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mGamepadIconImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGamepadIconSeparator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mReadArticlePanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid separator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mGuidanceKeyBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGuidanceKeysGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ListBox mGuidanceListBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel noGameGuidePanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DockPanel mViewDock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mEditBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mRevertBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mEditDock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mDiscardBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSaveBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOverlayGrid;

	private bool _contentLoaded;

	private bool IsVideoTutorialAvailable { get; set; }

	public bool IsViewState
	{
		get
		{
			return isViewState;
		}
		set
		{
			isViewState = value;
			((UIElement)mSchemesComboBox).IsEnabled = isViewState;
			if (isViewState)
			{
				((UIElement)mEditKeysGrid).Visibility = (Visibility)2;
				((UIElement)mControlsTab).Visibility = (Visibility)0;
				((UIElement)mSchemeTextBlock).Visibility = (Visibility)0;
				((UIElement)mSchemesComboBox).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mEditKeysGrid).Visibility = (Visibility)0;
				((UIElement)mControlsTab).Visibility = (Visibility)2;
				((UIElement)mSchemeTextBlock).Visibility = (Visibility)2;
				((UIElement)mSchemesComboBox).Visibility = (Visibility)2;
				((UIElement)mVideoBorder).Visibility = (Visibility)2;
				((UIElement)mReadArticlePanel).Visibility = (Visibility)2;
				((UIElement)mHowToPlayGrid).Visibility = (Visibility)2;
			}
		}
	}

	public static bool sIsDirty
	{
		get
		{
			return IsDirty;
		}
		set
		{
			IsDirty = value;
		}
	}

	internal GuidanceWindow(MainWindow window)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
		ParentWindow = window;
		((Window)this).Owner = (Window)(object)window;
		((CustomWindow)this).IsShowGLWindow = true;
		mIsOnboardingPopupToBeShownOnGuidanceClose = false;
		((CustomWindow)this).ShowWithParentWindow = true;
		if ((int)((Window)window).WindowState != 0)
		{
			window.RestoreWindows();
		}
		KMManager.CloseWindows();
		ResizeGuidanceWindow();
		mGuidanceHasBeenMoved = false;
		ResetGuidanceTab();
		Init();
		HideOnNextLaunch(updatedFlag: false);
	}

	internal void Init()
	{
		FillProfileComboBox();
	}

	internal void InitUI()
	{
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		IsDirty = false;
		mGuidanceData.Clear();
		foreach (IMAction gameControl in ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
		{
			if (gameControl.Type == KeyActionType.Pan)
			{
				lstPanTags.Add(ParentWindow.SelectedConfig.SelectedControlScheme.Name);
			}
			else if (gameControl.Type == KeyActionType.MOBADpad)
			{
				lstMOBATags.Add(ParentWindow.SelectedConfig.SelectedControlScheme.Name);
			}
			string category = string.Empty;
			if (ParentWindow.SelectedConfig.SelectedControlScheme.IsCategoryVisible)
			{
				category = (string.Equals(gameControl.GuidanceCategory.Trim(), "MISC", StringComparison.InvariantCulture) ? LocaleStrings.GetLocalizedString("STRING_" + gameControl.GuidanceCategory.Trim(), "") : ParentWindow.SelectedConfig.GetUIString(gameControl.GuidanceCategory.Trim()));
			}
			Dictionary<string, string> dictionary = UsefulExtensionMethod.DeepCopy<Dictionary<string, string>>(gameControl.Guidance);
			if (gameControl.Type == KeyActionType.Dpad)
			{
				dictionary.Clear();
				foreach (DPadControls value in Enum.GetValues(typeof(DPadControls)))
				{
					if (gameControl.Guidance.ContainsKey(((object)value/*cast due to constrained. prefix*/).ToString()))
					{
						dictionary.Add(((object)value/*cast due to constrained. prefix*/).ToString(), gameControl.Guidance[((object)value/*cast due to constrained. prefix*/).ToString()]);
					}
				}
			}
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				string text = string.Empty;
				if (gameControl is MOBASkill mobaSkill && item.Key.Contains("KeyActivate"))
				{
					text = AppendMOBASkillModeInGuidance(mobaSkill);
				}
				bool num = UsefulExtensionMethod.Contains(item.Key, "Gamepad", StringComparison.InvariantCultureIgnoreCase);
				if (!num && IMAction.DictPropertyInfo[gameControl.Type].ContainsKey(item.Key))
				{
					mGuidanceData.AddGuidance(isGamePad: false, category, ParentWindow.SelectedConfig.GetUIString(gameControl.Guidance[item.Key]) + text, gameControl[item.Key].ToString(), item.Key, gameControl);
				}
				string text2 = (num ? item.Key : (item.Key + "_alt1"));
				if (IMAction.DictPropertyInfo[gameControl.Type].ContainsKey(text2))
				{
					mGuidanceData.AddGuidance(isGamePad: true, category, ParentWindow.SelectedConfig.GetUIString(gameControl.Guidance[item.Key]) + text, gameControl[text2].ToString(), text2, gameControl);
				}
			}
		}
		((UIElement)mGamepadIcon).Visibility = (Visibility)((mGuidanceData.GamepadViewGuidance.Count <= 0) ? 2 : 0);
		mGuidanceData.SaveOriginalData();
		if (mGuidanceData.GamepadViewGuidance.Count > 0 || mGuidanceData.KeymapViewGuidance.Count > 0)
		{
			((UIElement)mKeyboardIcon).Visibility = (Visibility)0;
			((UIElement)separator).Visibility = (Visibility)0;
			((UIElement)noGameGuidePanel).Visibility = (Visibility)2;
			AddVideoElementInUI();
		}
		else
		{
			((UIElement)mKeyboardIcon).Visibility = (Visibility)2;
			((UIElement)separator).Visibility = (Visibility)2;
			((UIElement)noGameGuidePanel).Visibility = (Visibility)0;
		}
		ShowGuidance();
	}

	private static string AppendMOBASkillModeInGuidance(MOBASkill mobaSkill)
	{
		string result = string.Empty;
		if (!mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
		{
			result = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_MANUAL_MODE", "") + ")", new object[0]);
		}
		else if (mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
		{
			result = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_AUTOCAST", "") + ")", new object[0]);
		}
		else if (mobaSkill.AdvancedMode && mobaSkill.AutocastEnabled)
		{
			result = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_QUICK_CAST", "") + ")", new object[0]);
		}
		return result;
	}

	private void ResetGuidanceTab()
	{
		mIsGamePadTabSelected = false;
		mGamepadIconImage.ImageName = "guidance_gamepad";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
		mKeyboardIconImage.ImageName = "guidance_controls_hover";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
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

	internal void FillProfileComboBox()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		OrderingControlSchemes();
		((ItemsControl)mSchemesComboBox).Items.Clear();
		if (ParentWindow.SelectedConfig.ControlSchemes == null || ParentWindow.SelectedConfig.ControlSchemes.Count <= 0)
		{
			return;
		}
		bool flag = false;
		foreach (IMControlScheme value in ParentWindow.SelectedConfig.ControlSchemesDict.Values)
		{
			ComboBoxItem val = new ComboBoxItem
			{
				Content = value.Name
			};
			if (string.Equals(value.Name, ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))
			{
				((ListBoxItem)val).IsSelected = true;
				flag = true;
			}
			((FrameworkElement)val).ToolTip = ((ContentControl)val).Content;
			((ItemsControl)mSchemesComboBox).Items.Add((object)val);
		}
		if (!flag)
		{
			((ListBoxItem)(ComboBoxItem)((ItemsControl)mSchemesComboBox).Items[0]).IsSelected = true;
		}
		((UIElement)mSchemePanel).Visibility = (Visibility)((ParentWindow.SelectedConfig.ControlSchemesDict.Count == 1) ? 2 : 0);
		InitUI();
	}

	private void AddVideoElementInUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		foreach (AppInfo item in new JsonParser(Strings.CurrentDefaultVmName).GetAppList().ToList())
		{
			if (string.Equals(item.Package, KMManager.sPackageName, StringComparison.InvariantCulture))
			{
				mIsGuidanceVideoPresent = item.VideoPresent;
			}
		}
		UpdateVideoElement(mIsGamePadTabSelected);
	}

	private void UpdateVideoElement(bool isGamepadTabSelected = false)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (isGamepadTabSelected)
		{
			KMManager.sVideoMode = (GuidanceVideoType)3;
		}
		else if (lstMOBATags.Contains(ParentWindow.SelectedConfig.SelectedControlScheme.Name) || lstMOBATags.Contains("GlobalValidTag"))
		{
			KMManager.sVideoMode = (GuidanceVideoType)2;
		}
		else if (lstPanTags.Contains(ParentWindow.SelectedConfig.SelectedControlScheme.Name) || lstPanTags.Contains("GlobalValidTag"))
		{
			KMManager.sVideoMode = (GuidanceVideoType)1;
		}
		else
		{
			KMManager.sVideoMode = (GuidanceVideoType)0;
		}
		UpdateTutorialGrid();
		UpdateReadArticleGrid();
		GuidanceVisualInfoVisibility();
	}

	private void UpdateTutorialGrid()
	{
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		try
		{
			Dictionary<string, CustomThumbnail> customThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.CustomThumbnails;
			Dictionary<GuidanceVideoType, VideoThumbnailInfo> defaultThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.DefaultThumbnails;
			if (customThumbnails.ContainsKey(KMManager.sPackageName))
			{
				if ((int)KMManager.sVideoMode == 3 && customThumbnails[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()] != null)
				{
					text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()]).ImagePath;
				}
				else if (customThumbnails[KMManager.sPackageName][((object)(GuidanceVideoType)5/*cast due to constrained. prefix*/).ToString()] != null && ((Dictionary<string, VideoThumbnailInfo>)customThumbnails[KMManager.sPackageName][((object)(GuidanceVideoType)5/*cast due to constrained. prefix*/).ToString()]).ContainsKey(ParentWindow.SelectedConfig.SelectedControlScheme.Name))
				{
					string name = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
					text = ((Dictionary<string, VideoThumbnailInfo>)customThumbnails[KMManager.sPackageName][((object)(GuidanceVideoType)5/*cast due to constrained. prefix*/).ToString()])[name].ImagePath;
					KMManager.sVideoMode = (GuidanceVideoType)5;
				}
				else if (customThumbnails[KMManager.sPackageName][((object)(GuidanceVideoType)4/*cast due to constrained. prefix*/).ToString()] != null)
				{
					text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][((object)(GuidanceVideoType)4/*cast due to constrained. prefix*/).ToString()]).ImagePath;
					KMManager.sVideoMode = (GuidanceVideoType)4;
				}
				else if (customThumbnails[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()] != null)
				{
					text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()]).ImagePath;
				}
			}
			else if (defaultThumbnails.ContainsKey(KMManager.sVideoMode))
			{
				text = defaultThumbnails[KMManager.sVideoMode].ImagePath;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in evaluating tutorial grid : " + ex.ToString());
		}
		mVideoThumbnail.ImageName = text;
		IsVideoTutorialAvailable = !string.IsNullOrEmpty(text);
	}

	private void UpdateReadArticleGrid()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		mHelpArticleUrl = null;
		try
		{
			Dictionary<string, HelpArticle> helpArticles = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.HelpArticles;
			if (helpArticles.ContainsKey(KMManager.sPackageName) && helpArticles[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()] != null)
			{
				string name = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
				Dictionary<string, HelpArticleInfo> dictionary = (Dictionary<string, HelpArticleInfo>)helpArticles[KMManager.sPackageName][((object)(GuidanceVideoType)5/*cast due to constrained. prefix*/).ToString()];
				if ((int)KMManager.sVideoMode == 5 && dictionary.ContainsKey(name))
				{
					mHelpArticleUrl = dictionary[name].HelpArticleUrl;
				}
				else if ((int)KMManager.sVideoMode != 5)
				{
					mHelpArticleUrl = ((HelpArticleInfo)helpArticles[KMManager.sPackageName][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()]).HelpArticleUrl;
				}
			}
			else if (helpArticles.ContainsKey("default") && helpArticles["default"][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()] != null)
			{
				mHelpArticleUrl = ((HelpArticleInfo)helpArticles["default"][((object)Unsafe.As<GuidanceVideoType, GuidanceVideoType>(ref KMManager.sVideoMode)/*cast due to constrained. prefix*/).ToString()]).HelpArticleUrl;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in evaluating read article : " + ex.ToString());
		}
	}

	private void GuidanceWindow_Loaded(object sender, RoutedEventArgs e)
	{
		((Window)this).Activate();
	}

	internal void ResizeGuidanceWindow()
	{
		bool flag = false;
		IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(ParentWindow.Handle, isWorkAreaRequired: true);
		double num = ((FrameworkElement)ParentWindow).Width * MainWindow.sScalingFactor;
		double num2 = ((FrameworkElement)ParentWindow).Height * MainWindow.sScalingFactor;
		if (num + (double)mSidebarWidth * MainWindow.sScalingFactor + ((FrameworkElement)ParentWindow.mSidebar).Width * MainWindow.sScalingFactor > (double)fullscreenMonitorSize.Width)
		{
			num = (double)fullscreenMonitorSize.Width - (double)mSidebarWidth * MainWindow.sScalingFactor - ((FrameworkElement)ParentWindow.mSidebar).Width * MainWindow.sScalingFactor;
			num2 = ParentWindow.GetHeightFromWidth(num, isScaled: true);
			flag = true;
		}
		if (num2 > (double)fullscreenMonitorSize.Height)
		{
			num2 = fullscreenMonitorSize.Height;
			num = ParentWindow.GetWidthFromHeight(num2, isScaled: true);
			flag = true;
		}
		double top;
		if (((Window)ParentWindow).Top * MainWindow.sScalingFactor + num2 > (double)(fullscreenMonitorSize.Height + fullscreenMonitorSize.Y))
		{
			top = (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) - num2;
			flag = true;
		}
		else
		{
			top = ((Window)ParentWindow).Top * MainWindow.sScalingFactor;
		}
		double left;
		if (((Window)ParentWindow).Left * MainWindow.sScalingFactor + num + ((double)mSidebarWidth + ((FrameworkElement)ParentWindow.mSidebar).Width) * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.Width + fullscreenMonitorSize.X))
		{
			left = (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width) - num - ((double)mSidebarWidth + ((FrameworkElement)ParentWindow.mSidebar).Width) * MainWindow.sScalingFactor;
			flag = true;
		}
		else
		{
			left = ((Window)ParentWindow).Left * MainWindow.sScalingFactor;
		}
		if (flag)
		{
			ParentWindow.ChangeHeightWidthTopLeft(num, num2, top, left);
		}
		((Window)this).Left = ((mGuidanceWindowLeft == -1.0) ? (((Window)ParentWindow).Left + ((FrameworkElement)ParentWindow).ActualWidth) : mGuidanceWindowLeft);
		((Window)this).Top = ((mGuidanceWindowTop == -1.0) ? ((Window)ParentWindow).Top : mGuidanceWindowTop);
		((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow).ActualHeight;
	}

	internal void DimOverLayVisibility(Visibility visible)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mOverlayGrid).Visibility = visible;
	}

	private void GuidanceWindow_Closing(object sender, CancelEventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (!IsViewState && (sIsDirty || DataModificationTracker.HasChanged((object)mGuidanceData)))
		{
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANCEL_CONFIG_CHANGES", "");
			val.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), (EventHandler)delegate
			{
				KMManager.LoadIMActions(ParentWindow, KMManager.sPackageName);
				sIsDirty = false;
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), (EventHandler)delegate
			{
				e.Cancel = true;
			}, (string)null, false, (object)null);
			val.CloseButtonHandle((EventHandler)delegate
			{
				e.Cancel = true;
			}, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
		mGuidanceWindowLeft = ((Window)this).Left;
		mGuidanceWindowTop = ((Window)this).Top;
	}

	private void GuidanceWindow_Closed(object sender, EventArgs e)
	{
		if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>)((AppSettings appSettings) => appSettings.IsCloseGuidanceOnboardingCompleted)) && mIsOnboardingPopupToBeShownOnGuidanceClose)
		{
			ParentWindow.mSidebar?.ShowViewGuidancePopup();
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
		}
		KMManager.sGuidanceWindow = null;
		ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
		ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsAnyOperationPendingForTab = false;
	}

	private void ProfileComboBox_ProfileChanged(object sender, SelectionChangedEventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((ComboBox)mSchemesComboBox).IsDropDownOpen)
		{
			return;
		}
		((ComboBox)mSchemesComboBox).IsDropDownOpen = false;
		if (((Selector)mSchemesComboBox).SelectedItem != null)
		{
			string text = ((ContentControl)(ComboBoxItem)((Selector)mSchemesComboBox).SelectedItem).Content.ToString();
			if (SelectControlScheme(text))
			{
				AddToastPopup(LocaleStrings.GetLocalizedString("STRING_USING_SCHEME", "") + " : " + text);
				KMManager.SendSchemeChangedStats(ParentWindow, "game_guide");
				KMManager.ShowShootingModeTooltip(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			}
		}
	}

	internal bool SelectControlScheme(string schemeSelected)
	{
		if (ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(schemeSelected))
		{
			if (!ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected].Selected)
			{
				ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
				ParentWindow.SelectedConfig.SelectedControlScheme = ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected];
				ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
				if (!sIsDirty)
				{
					sIsDirty = true;
					SaveGuidanceChanges();
				}
				else
				{
					sIsDirty = false;
				}
				((Selector)mSchemesComboBox).SelectedValue = schemeSelected;
				return true;
			}
			InitUI();
		}
		return false;
	}

	private void AddToastPopup(string message)
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

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mIsOnboardingPopupToBeShownOnGuidanceClose = true;
		((CustomWindow)this).ShowWithParentWindow = false;
		((Window)this).Close();
		HideOnNextLaunch(updatedFlag: true);
		ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
		if (ParentWindow != null)
		{
			((UIElement)ParentWindow).Focus();
		}
	}

	internal void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
	{
		Logger.Info("Restarting Game Tab.");
		Thread thread = new Thread((ThreadStart)delegate
		{
			ParentWindow.mTopBar.mAppTabButtons.RestartTab(ParentWindow.StaticComponents.mSelectedTabButton.PackageName);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void HideOnNextLaunch(bool updatedFlag)
	{
		List<string> list = new List<string>(RegistryManager.Instance.DisabledGuidancePackages);
		if (updatedFlag)
		{
			UsefulExtensionMethod.AddIfNotContain<string>((IList<string>)list, KMManager.sPackageName);
		}
		else if (list.Contains(KMManager.sPackageName))
		{
			list.Remove(KMManager.sPackageName);
		}
		RegistryManager.Instance.DisabledGuidancePackages = list.ToArray();
	}

	internal OnBoardingPopupWindow GuidanceSchemeOnboardingBlurb()
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		((Window)onBoardingPopupWindow).Owner = (Window)(object)ParentWindow;
		((Window)onBoardingPopupWindow).Title = "SelectedGameSchemeBlurb";
		onBoardingPopupWindow.PlacementTarget = (UIElement)(object)mSchemesComboBox;
		onBoardingPopupWindow.LeftMargin = 50;
		onBoardingPopupWindow.TopMargin = 4;
		onBoardingPopupWindow.Startevent += delegate
		{
			mSchemesComboBox.Highlight = true;
		};
		onBoardingPopupWindow.Endevent += delegate
		{
			mSchemesComboBox.Highlight = false;
		};
		onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
		onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE", "");
		onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE_MESSAGE", ""), new object[1] { ParentWindow.SelectedConfig.SelectedControlScheme.Name });
		Point val = ((Visual)mSchemesComboBox).PointToScreen(new Point(0.0, 0.0));
		((Window)onBoardingPopupWindow).Left = ((Point)(ref val)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin;
		val = ((Visual)mSchemesComboBox).PointToScreen(new Point(0.0, 0.0));
		((Window)onBoardingPopupWindow).Top = ((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin;
		return onBoardingPopupWindow;
	}

	internal OnBoardingPopupWindow GuidanceOnboardingBlurb()
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if (((FrameworkElement)mGuidanceKeysGrid).ActualHeight < 1.0)
		{
			return null;
		}
		OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		((Window)onBoardingPopupWindow).Owner = (Window)(object)ParentWindow;
		onBoardingPopupWindow.PlacementTarget = (UIElement)(object)mGuidanceKeysGrid;
		((Window)onBoardingPopupWindow).Title = "GameControlBlurb";
		onBoardingPopupWindow.LeftMargin = 320;
		onBoardingPopupWindow.TopMargin = (230 - (int)((FrameworkElement)mGuidanceKeysGrid).ActualHeight) / 2;
		onBoardingPopupWindow.Startevent += delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			mGuidanceKeyBorder.BorderThickness = new Thickness(2.0);
		};
		onBoardingPopupWindow.Endevent += delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			mGuidanceKeyBorder.BorderThickness = new Thickness(0.0);
		};
		onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
		onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_HEADER", "");
		onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_MESSAGE", ""), new object[0]);
		onBoardingPopupWindow.PopArrowAlignment = (PopupArrowAlignment)2;
		DependencyProperty leftProperty = Window.LeftProperty;
		Point val = ((Visual)mGuidanceKeysGrid).PointToScreen(new Point(0.0, 0.0));
		((DependencyObject)onBoardingPopupWindow).SetValue(leftProperty, (object)(((Point)(ref val)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin));
		DependencyProperty topProperty = Window.TopProperty;
		val = ((Visual)mGuidanceKeysGrid).PointToScreen(new Point(0.0, 0.0));
		((DependencyObject)onBoardingPopupWindow).SetValue(topProperty, (object)(((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin));
		return onBoardingPopupWindow;
	}

	internal OnBoardingPopupWindow GuidanceVideoOnboardingBlurb()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)mVideoBorder).Visibility != 0 && (int)((UIElement)mQuickLearnBorder).Visibility != 0)
		{
			return null;
		}
		OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		((Window)onBoardingPopupWindow).Owner = (Window)(object)ParentWindow;
		((Window)onBoardingPopupWindow).Title = "GuidanceVideoBlurb";
		onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
		onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_HEADER", "");
		onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_MESSAGE", ""), new object[0]);
		onBoardingPopupWindow.PopArrowAlignment = (PopupArrowAlignment)2;
		onBoardingPopupWindow.LeftMargin = 320;
		Point val;
		if ((int)((UIElement)mVideoBorder).Visibility == 0)
		{
			onBoardingPopupWindow.PlacementTarget = (UIElement)(object)mVideoBorder;
			onBoardingPopupWindow.TopMargin = ((int)((FrameworkElement)mVideoBorder).ActualHeight - 80) / 2;
			onBoardingPopupWindow.Startevent += delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				mVideoBorder.BorderThickness = new Thickness(2.0);
			};
			onBoardingPopupWindow.Endevent += delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				mVideoBorder.BorderThickness = new Thickness(0.0);
			};
			DependencyProperty leftProperty = Window.LeftProperty;
			val = ((Visual)mVideoBorder).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)onBoardingPopupWindow).SetValue(leftProperty, (object)(((Point)(ref val)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin));
			DependencyProperty topProperty = Window.TopProperty;
			val = ((Visual)mVideoBorder).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)onBoardingPopupWindow).SetValue(topProperty, (object)(((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin));
		}
		else
		{
			onBoardingPopupWindow.PlacementTarget = (UIElement)(object)mQuickLearnBorder;
			onBoardingPopupWindow.TopMargin = ((int)((FrameworkElement)mQuickLearnBorder).ActualHeight + 160) / 2;
			onBoardingPopupWindow.Startevent += delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				mQuickLearnBorder.BorderThickness = new Thickness(2.0);
			};
			onBoardingPopupWindow.Endevent += delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				mQuickLearnBorder.BorderThickness = new Thickness(0.0);
			};
			DependencyProperty leftProperty2 = Window.LeftProperty;
			val = ((Visual)mQuickLearnBorder).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)onBoardingPopupWindow).SetValue(leftProperty2, (object)(((Point)(ref val)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin));
			DependencyProperty topProperty2 = Window.TopProperty;
			val = ((Visual)mQuickLearnBorder).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)onBoardingPopupWindow).SetValue(topProperty2, (object)(((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin));
		}
		return onBoardingPopupWindow;
	}

	private void ControlsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((UIElement)mControlsGrid).Visibility = (Visibility)0;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
	}

	private void SettingsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((UIElement)mControlsGrid).Visibility = (Visibility)2;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowForegroundDimColor");
	}

	private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		Stats.SendCommonClientStatsAsync("guidance-howtoplay", "watchvideo", ParentWindow.mVmName, KMManager.sPackageName, "", "");
		using GuidanceVideoWindow guidanceVideoWindow = new GuidanceVideoWindow(ParentWindow);
		((Window)guidanceVideoWindow).Owner = (Window)(object)ParentWindow;
		((FrameworkElement)guidanceVideoWindow).Width = Math.Max(((FrameworkElement)ParentWindow).ActualWidth * 0.7, 700.0);
		((FrameworkElement)guidanceVideoWindow).Height = Math.Max(((FrameworkElement)ParentWindow).ActualHeight * 0.7, 450.0);
		((FrameworkElement)guidanceVideoWindow).Loaded += new RoutedEventHandler(Window_Loaded);
		((Window)guidanceVideoWindow).ShowDialog();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		try
		{
			if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.mGuidanceHasBeenMoved)
			{
				CustomWindow val = (CustomWindow)((sender is CustomWindow) ? sender : null);
				((Window)val).Left = ((Window)ParentWindow).Left + (((FrameworkElement)ParentWindow).Width + ((FrameworkElement)KMManager.sGuidanceWindow).ActualWidth - ((FrameworkElement)val).ActualWidth) / 2.0;
				((Window)val).Top = ((Window)ParentWindow).Top + (((FrameworkElement)ParentWindow).Height - ((FrameworkElement)val).ActualHeight) / 2.0;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in setting position guidance video window: " + ex.ToString());
		}
	}

	private void GamepadIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mKeyboardIconImage.ImageName = "guidance_controls";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
		mGamepadIconImage.ImageName = "guidance_gamepad_hover";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
		mIsGamePadTabSelected = true;
		ShowGuidance();
	}

	private void KeyboardIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		mGamepadIconImage.ImageName = "guidance_gamepad";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
		mKeyboardIconImage.ImageName = "guidance_controls_hover";
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
		mIsGamePadTabSelected = false;
		ShowGuidance();
	}

	private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("guidance-howtoplay", "readarticle", ParentWindow.mVmName, KMManager.sPackageName, "", "");
		if (mHelpArticleUrl != null)
		{
			Utils.OpenUrl(mHelpArticleUrl);
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		try
		{
			if (((UIElement)mHeaderGrid).IsMouseOver && !((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(TextBlock)) && !((UIElement)mControlsTab).IsMouseOver)
			{
				((Window)this).DragMove();
				mGuidanceHasBeenMoved = true;
				((Window)this).ResizeMode = (ResizeMode)3;
			}
		}
		catch
		{
		}
	}

	internal void GuidanceWindowTabSelected(string mSelectedTab)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		mSelectedTab = ((!string.IsNullOrEmpty(mSelectedTab)) ? mSelectedTab : ((mGuidanceData.GamepadViewGuidance.Count > 0 && ParentWindow.IsGamepadConnected) ? "gamepad" : "default"));
		if (mSelectedTab == "gamepad")
		{
			if ((int)((UIElement)mGamepadIcon).Visibility == 0)
			{
				GamepadIconPreviewMouseLeftButtonUp(null, null);
			}
		}
		else if ((int)((UIElement)mKeyboardIcon).Visibility == 0)
		{
			KeyboardIconPreviewMouseLeftButtonUp(null, null);
		}
	}

	private void GuidanceWindow_KeyDown(object sender, KeyEventArgs e)
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
			if (string.Equals(item.ShortcutKey, text, StringComparison.InvariantCulture))
			{
				if (string.Equals(item.ShortcutName, "STRING_TOGGLE_KEYMAP_WINDOW", StringComparison.InvariantCulture))
				{
					ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("default");
				}
				else if (string.Equals(item.ShortcutName, "STRING_GAMEPAD_CONTROLS", StringComparison.InvariantCulture))
				{
					ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad");
				}
			}
		}
	}

	internal void UpdateSize()
	{
		if (!mGuidanceHasBeenMoved)
		{
			((Window)this).Left = ((mGuidanceWindowLeft == -1.0) ? (((Window)ParentWindow).Left + ((FrameworkElement)ParentWindow).ActualWidth) : mGuidanceWindowLeft);
			((Window)this).Top = ((mGuidanceWindowTop == -1.0) ? ((Window)ParentWindow).Top : mGuidanceWindowTop);
			((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow).ActualHeight;
		}
	}

	private void CustomWindow_StateChanged(object sender, EventArgs e)
	{
		((Window)this).WindowState = (WindowState)0;
	}

	private void GuidanceWindow_Activated(object sender, EventArgs e)
	{
		try
		{
			if (RegistryManager.Instance.ShowKeyControlsOverlay && KMManager.dictOverlayWindow.ContainsKey(ParentWindow))
			{
				KeymapCanvasWindow keymapCanvasWindow = KMManager.dictOverlayWindow[ParentWindow];
				if (keymapCanvasWindow != null)
				{
					((Window)keymapCanvasWindow).Show();
				}
			}
		}
		catch (Exception arg)
		{
			Logger.Error($"Exception in GuidanceWindow_Activated {arg}");
		}
	}

	private void GuidanceWindow_Deactivated(object sender, EventArgs e)
	{
		try
		{
			if (!((Window)ParentWindow).IsActive && KMManager.dictOverlayWindow.ContainsKey(ParentWindow))
			{
				((Window)KMManager.dictOverlayWindow[ParentWindow]).Hide();
			}
		}
		catch (Exception arg)
		{
			Logger.Error($"Exception in GuidanceWindow_Deactivated {arg}");
		}
	}

	private void GuidanceKeyTextChanged(object sender, TextChangedEventArgs e)
	{
		IEnumerable<GuidanceCategoryEditModel> source = mGuidanceData.KeymapEditGuidance.Union(mGuidanceData.GamepadEditGuidance);
		source.SelectMany((GuidanceCategoryEditModel cgem) => cgem.GuidanceEditModels).OfType<GuidanceEditTextModel>().ToList()
			.ForEach(delegate(GuidanceEditTextModel gem)
			{
				gem.TextValidityOption = (TextValidityOptions)1;
			});
		if (sender is IMapTextBox mapTextBox)
		{
			object dataContext = ((FrameworkElement)mapTextBox).DataContext;
			GuidanceEditTextModel guidanceEditTextModel = dataContext as GuidanceEditTextModel;
			if (guidanceEditTextModel != null && !string.Equals(guidanceEditTextModel.OriginalGuidanceKey, guidanceEditTextModel.GuidanceKey, StringComparison.OrdinalIgnoreCase))
			{
				sIsDirty = true;
				object toolTip = ((FrameworkElement)mapTextBox).ToolTip;
				ToolTip val = (ToolTip)((toolTip is ToolTip) ? toolTip : null);
				if (val != null)
				{
					val.PlacementTarget = (UIElement)(object)mapTextBox;
				}
				if ((from gem in source.SelectMany((GuidanceCategoryEditModel cgem) => cgem.GuidanceEditModels).OfType<GuidanceEditTextModel>()
					where !string.IsNullOrEmpty(gem.GuidanceKey) && (object)gem.PropertyType == typeof(string) && string.Equals(guidanceEditTextModel.GuidanceKey, gem.GuidanceKey, StringComparison.OrdinalIgnoreCase)
					select gem).Count() > 1)
				{
					((XTextBox)mapTextBox).InputTextValidity = (TextValidityOptions)0;
					if (val != null)
					{
						val.IsOpen = true;
					}
				}
			}
		}
		((UIElement)mSaveBtn).IsEnabled = sIsDirty || DataModificationTracker.HasChanged((object)mGuidanceData);
	}

	private void StepperTextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is StepperTextBox stepperTextBox && ((FrameworkElement)stepperTextBox).DataContext is GuidanceEditDecimalModel guidanceEditDecimalModel && !string.Equals(guidanceEditDecimalModel.OriginalGuidanceKey, guidanceEditDecimalModel.GuidanceKey, StringComparison.OrdinalIgnoreCase))
		{
			sIsDirty = true;
		}
		((UIElement)mSaveBtn).IsEnabled = sIsDirty || DataModificationTracker.HasChanged((object)mGuidanceData);
	}

	private void EditBtn_Click(object sender, RoutedEventArgs e)
	{
		ShowEditGuidance();
		DataModificationTracker.Lock((object)UsefulExtensionMethod.DeepCopy<GuidanceData>(mGuidanceData), new List<string> { "KeymapViewGuidance", "GamepadViewGuidance", "Item", "TextValidityOption", "GuidanceText", "OriginalGuidanceKey", "IMActionItems", "PropertyType", "ActionType" }, true);
		ClientStats.SendKeyMappingUIStatsAsync("guide_edit", KMManager.sPackageName);
	}

	private void SaveBtn_Click(object sender, RoutedEventArgs e)
	{
		SaveGuidanceChanges();
		AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
		ClientStats.SendKeyMappingUIStatsAsync("guide_save", KMManager.sPackageName);
	}

	private void SaveGuidanceChanges()
	{
		Logger.Debug($"ExtraLog: SaveGuidanceChanges, VmName:{ParentWindow.mVmName}, Scheme:{ParentWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{ParentWindow.SelectedConfig.ControlSchemes.Count}");
		bool flag = false;
		if (ParentWindow.OriginalLoadedConfig.ControlSchemes.Count != ParentWindow.SelectedConfig.ControlSchemes.Count)
		{
			flag = true;
		}
		if (sIsDirty || DataModificationTracker.HasChanged((object)mGuidanceData))
		{
			sIsDirty = true;
			KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: true);
		}
		if (flag)
		{
			FillProfileComboBox();
		}
		else
		{
			InitUI();
		}
		ShowViewGuidance();
		if (KMManager.dictOverlayWindow.ContainsKey(ParentWindow) && KMManager.dictOverlayWindow[ParentWindow] != null)
		{
			KMManager.dictOverlayWindow[ParentWindow].Init();
			if (RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				KMManager.ShowOverlayWindow(ParentWindow, isShow: true);
			}
		}
	}

	private void DiscardBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (sIsDirty || DataModificationTracker.HasChanged((object)mGuidanceData))
		{
			CustomMessageWindow val = new CustomMessageWindow
			{
				WindowStartupLocation = (WindowStartupLocation)1
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_CHANGES", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_GUIDANCE_CHANGES", "");
			val.AddButton((ButtonColors)4, "STRING_DISCARD", (EventHandler)delegate
			{
				string schemeName = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
				IEnumerable<IMControlScheme> source = ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme_) => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase));
				if (source.Any())
				{
					mGuidanceData.Reset();
					IMControlScheme iMControlScheme = ((source.Count() == 1) ? source.First() : source.Where((IMControlScheme scheme_) => !scheme_.BuiltIn).First());
					if (iMControlScheme.BuiltIn)
					{
						ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
						IMControlScheme iMControlScheme2 = ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture)).FirstOrDefault();
						if (iMControlScheme2 != null)
						{
							iMControlScheme2.Selected = true;
							ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme2;
							ParentWindow.SelectedConfig.ControlSchemesDict[iMControlScheme2.Name] = iMControlScheme2;
							sIsDirty = true;
						}
					}
					else
					{
						ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
						ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme.DeepCopy();
						ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = ParentWindow.SelectedConfig.SelectedControlScheme;
						ParentWindow.SelectedConfig.ControlSchemes.Add(ParentWindow.SelectedConfig.SelectedControlScheme);
						InitUI();
					}
					ShowViewGuidance();
				}
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)delegate
			{
			}, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
		else
		{
			ShowViewGuidance();
		}
	}

	private void ShowGuidance()
	{
		if (IsViewState)
		{
			ShowViewGuidance();
		}
		else
		{
			ShowEditGuidance();
		}
	}

	private void ShowViewGuidance()
	{
		IsViewState = true;
		((UIElement)mViewDock).Visibility = (Visibility)0;
		((UIElement)mEditDock).Visibility = (Visibility)2;
		if (mGuidanceData.GamepadViewGuidance.Count == 0)
		{
			mIsGamePadTabSelected = false;
		}
		ObservableCollection<GuidanceCategoryViewModel> observableCollection = (mIsGamePadTabSelected ? mGuidanceData.GamepadViewGuidance : mGuidanceData.KeymapViewGuidance);
		if (observableCollection != null)
		{
			if (observableCollection.Count == 1)
			{
				((FrameworkElement)mGuidanceListBox).DataContext = observableCollection[0].GuidanceViewModels;
				((ItemsControl)mGuidanceListBox).AlternationCount = 2;
			}
			else
			{
				((FrameworkElement)mGuidanceListBox).DataContext = observableCollection;
				((ItemsControl)mGuidanceListBox).AlternationCount = 0;
			}
		}
		((UIElement)mEditBtn).Visibility = (Visibility)((ParentWindow.SelectedConfig.SelectedControlScheme == null || !ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any() || !ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany((IMAction action) => action.Guidance).Any()) ? 2 : 0);
		((UIElement)mRevertBtn).Visibility = (Visibility)((ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) != 2) ? 2 : 0);
		UpdateVideoElement(mIsGamePadTabSelected);
		sIsDirty = false;
		((UIElement)mSaveBtn).IsEnabled = false;
	}

	private void ShowEditGuidance()
	{
		IsViewState = false;
		((UIElement)mViewDock).Visibility = (Visibility)2;
		((UIElement)mEditDock).Visibility = (Visibility)0;
		ObservableCollection<GuidanceCategoryEditModel> observableCollection = (mIsGamePadTabSelected ? mGuidanceData.GamepadEditGuidance : mGuidanceData.KeymapEditGuidance);
		if (observableCollection != null)
		{
			if (observableCollection.Count == 1)
			{
				((FrameworkElement)mGuidanceListBox).DataContext = observableCollection[0].GuidanceEditModels;
				((ItemsControl)mGuidanceListBox).AlternationCount = 2;
			}
			else
			{
				((FrameworkElement)mGuidanceListBox).DataContext = observableCollection;
				((ItemsControl)mGuidanceListBox).AlternationCount = 0;
			}
		}
	}

	private void RevertBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
		{
			return;
		}
		CustomMessageWindow val = new CustomMessageWindow
		{
			WindowStartupLocation = (WindowStartupLocation)1
		};
		val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
		val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
		val.AddButton((ButtonColors)0, "STRING_RESET", (EventHandler)delegate
		{
			Logger.Debug($"ExtraLog: Revert Clicked, VmName:{ParentWindow.mVmName}, Scheme:{ParentWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{ParentWindow.SelectedConfig.ControlSchemes.Count}");
			string schemeName = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
			bool isBookMarked = ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
			ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
			IMControlScheme iMControlScheme = ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture) && scheme.BuiltIn).FirstOrDefault();
			if (iMControlScheme != null)
			{
				Logger.Debug($"ExtraLog: Updating scheme dictionary, VmName:{ParentWindow.mVmName}, Scheme:{ParentWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{ParentWindow.SelectedConfig.ControlSchemes.Count}");
				iMControlScheme.Selected = true;
				iMControlScheme.IsBookMarked = isBookMarked;
				ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme;
				ParentWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name] = iMControlScheme;
				sIsDirty = true;
				SaveGuidanceChanges();
				ClientStats.SendKeyMappingUIStatsAsync("guide_reset", KMManager.sPackageName);
			}
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)delegate
		{
		}, (string)null, false, (object)null);
		ParentWindow.ShowDimOverlay();
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
		ParentWindow.HideDimOverlay();
	}

	public void Highlight()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_010b: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_0142: Expected O, but got Unknown
		Brush obj = BlueStacksUIBinding.Instance.ColorModel["BlueMouseDownBorderBackground"];
		SolidColorBrush val = (SolidColorBrush)(object)((obj is SolidColorBrush) ? obj : null);
		if (val == null)
		{
			return;
		}
		Border val2 = new Border
		{
			BorderThickness = new Thickness(((FrameworkElement)this).ActualWidth / 2.0, ((FrameworkElement)this).ActualHeight / 2.0, ((FrameworkElement)this).ActualWidth / 2.0, ((FrameworkElement)this).ActualHeight / 2.0)
		};
		GradientStopCollection val3 = new GradientStopCollection();
		val3.Add(new GradientStop
		{
			Color = Colors.Transparent,
			Offset = 0.0
		});
		GradientStop val4 = new GradientStop
		{
			Offset = 1.0
		};
		Color color = val.Color;
		byte a = ((Color)(ref color)).A;
		color = val.Color;
		byte r = ((Color)(ref color)).R;
		color = val.Color;
		byte g = ((Color)(ref color)).G;
		color = val.Color;
		val4.Color = Color.FromArgb(a, r, g, ((Color)(ref color)).B);
		val3.Add(val4);
		val2.BorderBrush = (Brush)new RadialGradientBrush(val3)
		{
			RadiusX = 1.0,
			RadiusY = 1.0,
			Opacity = 0.5
		};
		Border border = val2;
		((Panel)mGuidanceMainGrid).Children.Add((UIElement)(object)border);
		new Thread((ThreadStart)delegate
		{
			Thread.Sleep(500);
			((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				((Panel)mGuidanceMainGrid).Children.Remove((UIElement)(object)border);
			}, new object[0]);
		}).Start();
	}

	private void GuidanceWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)this).Visibility == 0)
		{
			ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide_active");
		}
		else
		{
			ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
		}
	}

	private void QuickLearnBorder_MouseUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("guidance-howtoplay", "quicklearn", ParentWindow.mVmName, KMManager.sPackageName, "", "");
		if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.OnBoardingInfo.OnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName) == true)
		{
			ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl = new GameOnboardingControl(ParentWindow, KMManager.sPackageName, "guidancewindow");
			KMManager.sGuidanceWindow?.DimOverLayVisibility((Visibility)0);
			ParentWindow.ShowDimOverlay(ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl);
		}
	}

	private void HowToPlay_MouseUp(object sender, MouseButtonEventArgs e)
	{
		mHowToPlayCollapseExpand.ImageName = (string.Equals(mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? "outline_settings_expand" : "outline_settings_collapse");
		GuidanceVisualInfoVisibility();
	}

	private void GuidanceVisualInfoVisibility()
	{
		if (string.Equals(KMManager.sPackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
		{
			((UIElement)mHowToPlayGrid).Visibility = (Visibility)0;
			((UIElement)mQuickLearnBorder).Visibility = (Visibility)((PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.OnBoardingInfo.OnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName) != true || !string.Equals(mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? 2 : 0);
			((UIElement)mVideoTutorialBorder).Visibility = (Visibility)((!IsVideoTutorialAvailable || !string.Equals(mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? 2 : 0);
			((UIElement)mReadArticleBorder).Visibility = (Visibility)((string.IsNullOrEmpty(mHelpArticleUrl) || !string.Equals(mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? 2 : 0);
		}
		else
		{
			((UIElement)mVideoBorder).Visibility = (Visibility)((!IsVideoTutorialAvailable) ? 2 : 0);
			((UIElement)mReadArticlePanel).Visibility = (Visibility)(string.IsNullOrEmpty(mHelpArticleUrl) ? 2 : 0);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancewindow.xaml", UriKind.Relative);
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
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
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
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Expected O, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Expected O, but got Unknown
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
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
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Expected O, but got Unknown
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Expected O, but got Unknown
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Expected O, but got Unknown
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Expected O, but got Unknown
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Expected O, but got Unknown
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Expected O, but got Unknown
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Expected O, but got Unknown
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Expected O, but got Unknown
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Expected O, but got Unknown
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Expected O, but got Unknown
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Expected O, but got Unknown
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Expected O, but got Unknown
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Expected O, but got Unknown
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Expected O, but got Unknown
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Expected O, but got Unknown
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Expected O, but got Unknown
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Expected O, but got Unknown
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Expected O, but got Unknown
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Expected O, but got Unknown
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((Window)(GuidanceWindow)target).StateChanged += CustomWindow_StateChanged;
			((Window)(GuidanceWindow)target).Closing += GuidanceWindow_Closing;
			((Window)(GuidanceWindow)target).Closed += GuidanceWindow_Closed;
			((FrameworkElement)(GuidanceWindow)target).Loaded += new RoutedEventHandler(GuidanceWindow_Loaded);
			((UIElement)(GuidanceWindow)target).KeyDown += new KeyEventHandler(GuidanceWindow_KeyDown);
			((UIElement)(GuidanceWindow)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(GuidanceWindow_IsVisibleChanged);
			((Window)(GuidanceWindow)target).Activated += GuidanceWindow_Activated;
			((Window)(GuidanceWindow)target).Deactivated += GuidanceWindow_Deactivated;
			break;
		case 2:
			mGuidanceMainGrid = (Grid)target;
			break;
		case 3:
			mGameControlBorder = (Border)target;
			break;
		case 4:
			mHeaderGrid = (DockPanel)target;
			((UIElement)mHeaderGrid).MouseLeftButtonDown += new MouseButtonEventHandler(Grid_MouseLeftButtonDown);
			break;
		case 5:
			mControlsTab = (Grid)target;
			((UIElement)mControlsTab).MouseLeftButtonUp += new MouseButtonEventHandler(ControlsTabMouseLeftButtonUp);
			break;
		case 6:
			mControlsTabTextBlock = (TextBlock)target;
			break;
		case 7:
			mEditKeysGrid = (Grid)target;
			break;
		case 8:
			mEditKeysGridTextBlock = (TextBlock)target;
			break;
		case 9:
			mCloseSideBarWindow = (CustomPictureBox)target;
			((UIElement)mCloseSideBarWindow).MouseDown += new MouseButtonEventHandler(CustomPictureBox_MouseDown);
			((UIElement)mCloseSideBarWindow).MouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 10:
			mControlsGrid = (Grid)target;
			break;
		case 11:
			mSchemePanel = (StackPanel)target;
			break;
		case 12:
			mSchemeTextBlock = (TextBlock)target;
			break;
		case 13:
			mSchemesComboBox = (CustomComboBox)target;
			((Selector)mSchemesComboBox).SelectionChanged += new SelectionChangedEventHandler(ProfileComboBox_ProfileChanged);
			break;
		case 14:
			mVideoBorder = (Border)target;
			break;
		case 15:
			((UIElement)(Grid)target).MouseUp += new MouseButtonEventHandler(CustomPictureBox_MouseUp);
			break;
		case 16:
			mVideoThumbnail = (CustomPictureBox)target;
			break;
		case 17:
			mHowToPlayGrid = (Border)target;
			break;
		case 18:
			((UIElement)(Grid)target).MouseUp += new MouseButtonEventHandler(HowToPlay_MouseUp);
			break;
		case 19:
			mHowToPlayCollapseExpand = (CustomPictureBox)target;
			break;
		case 20:
			mQuickLearnBorder = (Border)target;
			((UIElement)mQuickLearnBorder).MouseUp += new MouseButtonEventHandler(QuickLearnBorder_MouseUp);
			break;
		case 21:
			mVideoTutorialBorder = (Border)target;
			((UIElement)mVideoTutorialBorder).MouseUp += new MouseButtonEventHandler(CustomPictureBox_MouseUp);
			break;
		case 22:
			mReadArticleBorder = (Border)target;
			((UIElement)mReadArticleBorder).MouseUp += new MouseButtonEventHandler(ReadMoreLinkMouseLeftButtonUp);
			break;
		case 23:
			mKeysIconGrid = (DockPanel)target;
			break;
		case 24:
			mKeyboardIcon = (Grid)target;
			break;
		case 25:
			mKeyboardIconImage = (CustomPictureBox)target;
			((UIElement)mKeyboardIconImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(KeyboardIconPreviewMouseLeftButtonUp);
			break;
		case 26:
			mKeyboardIconSeparator = (Grid)target;
			break;
		case 27:
			mGamepadIcon = (Grid)target;
			break;
		case 28:
			mGamepadIconImage = (CustomPictureBox)target;
			((UIElement)mGamepadIconImage).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(GamepadIconPreviewMouseLeftButtonUp);
			break;
		case 29:
			mGamepadIconSeparator = (Grid)target;
			break;
		case 30:
			mReadArticlePanel = (StackPanel)target;
			break;
		case 31:
			((UIElement)(TextBlock)target).MouseLeftButtonDown += new MouseButtonEventHandler(ReadMoreLinkMouseLeftButtonUp);
			break;
		case 32:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonDown += new MouseButtonEventHandler(ReadMoreLinkMouseLeftButtonUp);
			break;
		case 33:
			separator = (Grid)target;
			break;
		case 34:
			mGuidanceKeyBorder = (Border)target;
			break;
		case 35:
			mGuidanceKeysGrid = (Grid)target;
			break;
		case 36:
			mGuidanceListBox = (ListBox)target;
			break;
		case 37:
			noGameGuidePanel = (StackPanel)target;
			break;
		case 38:
			mViewDock = (DockPanel)target;
			break;
		case 39:
			mEditBtn = (CustomButton)target;
			((ButtonBase)mEditBtn).Click += new RoutedEventHandler(EditBtn_Click);
			break;
		case 40:
			mRevertBtn = (CustomButton)target;
			((ButtonBase)mRevertBtn).Click += new RoutedEventHandler(RevertBtn_Click);
			break;
		case 41:
			mEditDock = (Grid)target;
			break;
		case 42:
			mDiscardBtn = (CustomButton)target;
			((ButtonBase)mDiscardBtn).Click += new RoutedEventHandler(DiscardBtn_Click);
			break;
		case 43:
			mSaveBtn = (CustomButton)target;
			((ButtonBase)mSaveBtn).Click += new RoutedEventHandler(SaveBtn_Click);
			break;
		case 44:
			mOverlayGrid = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
