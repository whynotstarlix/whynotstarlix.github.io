using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal static class KMManager
{
	internal static KeymapCanvasWindow CanvasWindow = null;

	internal static GuidanceWindow sGuidanceWindow = null;

	internal static string mComboEvents = string.Empty;

	internal static bool sIsCancelComboClicked = false;

	internal static bool sIsSaveComboClicked = false;

	internal static bool sIsComboRecordingOn = false;

	internal static DualTextBlockControl sGamepadDualTextbox = null;

	internal static IMapTextBox CurrentIMapTextBox = null;

	internal static Dictionary<string, bool> dictGamepadEligibility = new Dictionary<string, bool>();

	internal static string sShootingModeKey = "F1";

	public static Dictionary<string, bool> pressedGamepadKeyList = new Dictionary<string, bool>();

	public static string sGameControlsEnabledDisabledArray = string.Empty;

	public static string sOldGameControlsEnabledDisabledArray = string.Empty;

	public static List<List<CanvasElement>> listCanvasElement = new List<List<CanvasElement>>();

	internal static bool sIsInScriptEditingMode = false;

	internal static Dictionary<MainWindow, KeymapCanvasWindow> dictOverlayWindow = new Dictionary<MainWindow, KeymapCanvasWindow>();

	internal static CanvasElement sDragCanvasElement;

	public static string ParserVersion = "17";

	internal static string sPackageName = string.Empty;

	internal static GuidanceVideoType sVideoMode = (GuidanceVideoType)0;

	internal static bool sIsDeveloperModeOn = false;

	internal static int mOnboardingCounter = 1;

	internal static bool mIsEnabledStateChanged = false;

	internal static List<OnBoardingPopupWindow> onBoardingPopupWindows = new List<OnBoardingPopupWindow>();

	public static bool IsDragging => sDragCanvasElement != null;

	internal static void GetCurrentParserVersion(MainWindow window)
	{
		try
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					JObject val = JObject.Parse(window.mFrontendHandler.SendFrontendRequest("getkeymappingparserversion"));
					if (val["success"].ToObject<bool>())
					{
						ParserVersion = ((object)val["parserversion"]).ToString();
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("Failed to get/parse result for getkeymappingparserversion");
					Logger.Error(ex2.ToString());
				}
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in KMManager init: " + ex.ToString());
		}
	}

	internal static void CheckAndCreateNewScheme()
	{
		if (BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
		{
			bool isBookMarked = BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
			IMControlScheme iMControlScheme = BlueStacksUIUtils.LastActivatedWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn && string.Equals(scheme.Name, BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)).FirstOrDefault();
			if (iMControlScheme != null)
			{
				AddNewControlSchemeAndSelectImap(BlueStacksUIUtils.LastActivatedWindow, iMControlScheme);
				BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked = isBookMarked;
			}
		}
	}

	internal static void UpdateUIForGamepadEvent(string text, bool isDown)
	{
		if (UsefulExtensionMethod.Contains(text, "GamepadStart", StringComparison.InvariantCultureIgnoreCase) || UsefulExtensionMethod.Contains(text, "GamepadBack", StringComparison.InvariantCultureIgnoreCase))
		{
			return;
		}
		string text2 = string.Empty;
		string value = ".";
		if (text.Contains(value))
		{
			text2 = text.Substring(text.IndexOf(value, StringComparison.InvariantCultureIgnoreCase));
			text = text.Substring(0, text.IndexOf(value, StringComparison.InvariantCultureIgnoreCase));
		}
		if (CanvasWindow != null && ((UIElement)CanvasWindow).IsVisible && sGamepadDualTextbox != null)
		{
			if (string.Equals(sGamepadDualTextbox.ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase))
			{
				text = CheckForAnalogEvent(text);
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.Tap || sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.TapRepeat || sGamepadDualTextbox.LstActionItem[0].Type == KeyActionType.Script)
			{
				CheckItemToAddInList(text, isDown);
				if (pressedGamepadKeyList.Count > 2)
				{
					pressedGamepadKeyList.Clear();
					sGamepadDualTextbox.mKeyTextBox.Text = string.Empty;
					sGamepadDualTextbox.Setvalue(string.Empty);
					((FrameworkElement)sGamepadDualTextbox.mKeyTextBox).ToolTip = string.Empty;
				}
				else if (pressedGamepadKeyList.Count == 2)
				{
					string text3 = IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0)) + " + " + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(1));
					sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(1)), "");
					sGamepadDualTextbox.Setvalue(text3 + text2);
					((FrameworkElement)sGamepadDualTextbox.mKeyTextBox).ToolTip = sGamepadDualTextbox.mKeyTextBox.Text;
					pressedGamepadKeyList.Clear();
				}
				else if (pressedGamepadKeyList.Count == 1)
				{
					string stringForUI = IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0));
					sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUI, "");
					sGamepadDualTextbox.Setvalue(stringForUI + text2);
					((FrameworkElement)sGamepadDualTextbox.mKeyTextBox).ToolTip = sGamepadDualTextbox.mKeyTextBox.Text;
				}
			}
			else
			{
				sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + text, "");
				sGamepadDualTextbox.Setvalue(text + text2);
				((FrameworkElement)sGamepadDualTextbox.mKeyTextBox).ToolTip = sGamepadDualTextbox.mKeyTextBox.Text;
			}
		}
		else
		{
			if (sGuidanceWindow == null || !((UIElement)sGuidanceWindow).IsVisible || CurrentIMapTextBox == null || CurrentIMapTextBox.IMActionItems == null || !CurrentIMapTextBox.IMActionItems.Any())
			{
				return;
			}
			CheckAndCreateNewScheme();
			GuidanceWindow.sIsDirty = true;
			if (CurrentIMapTextBox.IMActionItems != null && CurrentIMapTextBox.IMActionItems.Any((IMActionItem item) => string.Equals(item.ActionItem, "GamepadStick", StringComparison.InvariantCultureIgnoreCase)))
			{
				text = CheckForAnalogEvent(text);
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (CurrentIMapTextBox.ActionType != KeyActionType.Tap && CurrentIMapTextBox.ActionType != KeyActionType.TapRepeat && CurrentIMapTextBox.ActionType != KeyActionType.Script)
			{
				((FrameworkElement)CurrentIMapTextBox).Tag = text + text2;
				((TextBox)CurrentIMapTextBox).Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text), "");
				{
					foreach (IMActionItem iMActionItem in CurrentIMapTextBox.IMActionItems)
					{
						IMapTextBox.Setvalue(iMActionItem, ((FrameworkElement)CurrentIMapTextBox).Tag.ToString());
					}
					return;
				}
			}
			CheckItemToAddInList(text, isDown);
			if (pressedGamepadKeyList.Count > 2)
			{
				pressedGamepadKeyList.Clear();
				((FrameworkElement)CurrentIMapTextBox).Tag = string.Empty;
				((TextBox)CurrentIMapTextBox).Text = string.Empty;
				{
					foreach (IMActionItem iMActionItem2 in CurrentIMapTextBox.IMActionItems)
					{
						IMapTextBox.Setvalue(iMActionItem2, string.Empty);
					}
					return;
				}
			}
			if (pressedGamepadKeyList.Count == 2)
			{
				string text4 = IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0)) + " + " + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(1));
				((FrameworkElement)CurrentIMapTextBox).Tag = text4 + text2;
				((TextBox)CurrentIMapTextBox).Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(1)), "");
				foreach (IMActionItem iMActionItem3 in CurrentIMapTextBox.IMActionItems)
				{
					IMapTextBox.Setvalue(iMActionItem3, ((FrameworkElement)CurrentIMapTextBox).Tag.ToString());
				}
				pressedGamepadKeyList.Clear();
			}
			else
			{
				if (pressedGamepadKeyList.Count != 1)
				{
					return;
				}
				string stringForUI2 = IMAPKeys.GetStringForUI(pressedGamepadKeyList.Keys.ElementAt(0));
				((FrameworkElement)CurrentIMapTextBox).Tag = stringForUI2 + text2;
				((TextBox)CurrentIMapTextBox).Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUI2, "");
				foreach (IMActionItem iMActionItem4 in CurrentIMapTextBox.IMActionItems)
				{
					IMapTextBox.Setvalue(iMActionItem4, ((FrameworkElement)CurrentIMapTextBox).Tag.ToString());
				}
			}
		}
	}

	private static void CheckItemToAddInList(string text, bool isDown)
	{
		if (pressedGamepadKeyList.ContainsKey(text) && pressedGamepadKeyList[text] && !isDown)
		{
			pressedGamepadKeyList.Remove(text);
		}
		if (!pressedGamepadKeyList.ContainsKey(text) && isDown)
		{
			pressedGamepadKeyList.Add(text, isDown);
		}
	}

	private static string CheckForAnalogEvent(string text)
	{
		string result = string.Empty;
		if (string.Equals(text, "GamepadLStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickDown", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickRight", StringComparison.InvariantCultureIgnoreCase))
		{
			result = "LeftStick";
		}
		else if (string.Equals(text, "GamepadRStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickDown", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickRight", StringComparison.InvariantCultureIgnoreCase))
		{
			result = "RightStick";
		}
		return result;
	}

	internal static bool KeyMappingFilesAvailable(string packageName)
	{
		return !string.IsNullOrEmpty(Utils.GetInputmapperFile(packageName));
	}

	internal static bool IsSelectedSchemeSmart(MainWindow mainWindow)
	{
		if (mainWindow == null)
		{
			return false;
		}
		return mainWindow.SelectedConfig?.SelectedControlScheme?.Images?.Count > 0;
	}

	internal static bool IsShowShootingModeTooltip(MainWindow mainWindow)
	{
		bool result = false;
		foreach (IMAction gameControl in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
		{
			if (gameControl is Pan pan)
			{
				if ((pan.Tweaks & 0x20) != 0)
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}

	internal static void HandleInputMapperWindow(MainWindow mainWindow, string isSelectedTab = "")
	{
	}

	internal static void ResizeMainWindow(MainWindow window)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Screen val = Screen.FromHandle(new WindowInteropHelper((Window)(object)window).Handle);
		double sScalingFactor = MainWindow.sScalingFactor;
		Rectangle rectangle = new Rectangle((int)((double)val.WorkingArea.X / sScalingFactor), (int)((double)val.WorkingArea.Y / sScalingFactor), (int)((double)val.WorkingArea.Width / sScalingFactor), (int)((double)val.WorkingArea.Height / sScalingFactor));
		if (((Window)window).Top + ((FrameworkElement)window).ActualHeight > (double)rectangle.Height)
		{
			((Window)window).Top = ((double)rectangle.Height - ((FrameworkElement)window).ActualHeight) / 2.0;
		}
		if (((Window)window).Left < 0.0 || ((Window)window).Left + ((FrameworkElement)window).ActualWidth > (double)rectangle.Width)
		{
			((Window)window).Left = ((double)rectangle.Width - ((FrameworkElement)window).ActualWidth) / 2.0;
		}
	}

	internal static void ShowAdvancedSettings(MainWindow mainWindow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Window)mainWindow).WindowState != 0)
		{
			mainWindow.RestoreWindows();
			KeymapCanvasWindow.sWasMaximized = true;
		}
		CloseWindows();
		if (sGuidanceWindow == null)
		{
			KeymapCanvasWindow keymapCanvasWindow = new KeymapCanvasWindow(mainWindow);
			((Window)keymapCanvasWindow).Owner = (Window)(object)mainWindow;
			CanvasWindow = keymapCanvasWindow;
			CanvasWindow.InitLayout();
			ShowOverlayWindow(mainWindow, isShow: false);
			((Window)CanvasWindow).ShowDialog();
			if (RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				ShowOverlayWindow(mainWindow, isShow: true);
			}
		}
	}

	internal static void ShowDynamicOverlay(MainWindow mainWindow, bool isShow, bool isReload = false, string data = "")
	{
		DynamicOverlayConfigControls.Init(data);
		ShowOverlayWindow(mainWindow, isShow, isReload);
	}

	internal static void HandleCallbackControl(MainWindow mainWindow, string data = "")
	{
		DynamicOverlayConfigControls.Init(data);
		if ((!sIsInScriptEditingMode && CanvasWindow?.SidebarWindow != null) || listCanvasElement == null || DynamicOverlayConfigControls.Instance.GameControls == null || listCanvasElement.Count != DynamicOverlayConfigControls.Instance.GameControls.Count)
		{
			return;
		}
		try
		{
			for (int i = 0; i < DynamicOverlayConfigControls.Instance.GameControls.Count; i++)
			{
				DynamicOverlayConfig val = DynamicOverlayConfigControls.Instance.GameControls[i];
				CanvasElement canvasElement = listCanvasElement[i][0];
				if (canvasElement != null && canvasElement.ListActionItem.First().Type == KeyActionType.Callback)
				{
					Logger.Info("Callback: IsEnabled1 : " + val.Enabled);
					IMAction iMAction = canvasElement.ListActionItem.First();
					HandleCallbackPrimitive(mainWindow, val, (iMAction as Callback).Action, (iMAction as Callback).Id);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("ERROR : GameControl not found in canvas elements. " + ex);
		}
	}

	internal static void ShowOverlayWindow(MainWindow mainWindow, bool isShow, bool isreload = false)
	{
		if (mainWindow == null)
		{
			return;
		}
		if (isShow && ((UIElement)mainWindow).IsVisible)
		{
			if (!dictOverlayWindow.ContainsKey(mainWindow) && mainWindow != null && mainWindow.mTopBar?.mAppTabButtons?.SelectedTab?.PackageName != null && mainWindow != null && mainWindow.mTopBar?.mAppTabButtons?.SelectedTab.mTabType == TabType.AppTab)
			{
				if (FeatureManager.Instance.IsCustomUIForNCSoft && mainWindow.mDimOverlay != null && mainWindow.mDimOverlay.Control != null && (object)mainWindow.mDimOverlay.Control.GetType() == ((object)mainWindow.ScreenLockInstance).GetType() && ((UIElement)mainWindow.ScreenLockInstance).IsVisible)
				{
					return;
				}
				KeymapCanvasWindow keymapCanvasWindow = new KeymapCanvasWindow(mainWindow);
				dictOverlayWindow[mainWindow] = keymapCanvasWindow;
				keymapCanvasWindow.IsInOverlayMode = true;
				((Window)keymapCanvasWindow).Owner = (Window)(object)mainWindow;
				keymapCanvasWindow.InitLayout();
				if (mainWindow.mFrontendHandler.mFrontendHandle == IntPtr.Zero)
				{
					mainWindow.mFrontendHandler.ReparentingCompletedAction = ShowOverlayWindowAfterReparenting;
				}
				else
				{
					ShowOverlayWindowAfterReparenting(mainWindow);
				}
			}
			else if (mainWindow != null && dictOverlayWindow.ContainsKey(mainWindow))
			{
				if (isreload)
				{
					dictOverlayWindow[mainWindow].ReloadCanvasWindow();
				}
				dictOverlayWindow[mainWindow].UpdateSize();
			}
			if (dictOverlayWindow.ContainsKey(mainWindow))
			{
				dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(0.0, 0.0, isVisible: false);
			}
		}
		else if (dictOverlayWindow.ContainsKey(mainWindow) && !dictOverlayWindow[mainWindow].mIsClosing)
		{
			((Window)dictOverlayWindow[mainWindow]).Close();
		}
		ToggleOverlayVisibility(mainWindow);
	}

	private static void ToggleOverlayVisibility(MainWindow mainWindow)
	{
		if ((CanvasWindow?.SidebarWindow != null && !sIsInScriptEditingMode) || listCanvasElement == null || DynamicOverlayConfigControls.Instance.GameControls == null || (listCanvasElement.Count != DynamicOverlayConfigControls.Instance.GameControls.Count && !sIsInScriptEditingMode))
		{
			return;
		}
		try
		{
			for (int i = 0; i < DynamicOverlayConfigControls.Instance.GameControls.Count; i++)
			{
				if (mainWindow.SelectedConfig.SelectedControlScheme.GameControls[i].IsVisibleInOverlay)
				{
					DynamicOverlayConfig val = DynamicOverlayConfigControls.Instance.GameControls[i];
					if (!val.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
					{
						listCanvasElement[i].ForEach(delegate(CanvasElement element)
						{
							((UIElement)element).Visibility = (Visibility)1;
						});
						continue;
					}
					foreach (CanvasElement item in listCanvasElement[i])
					{
						IMAction iMAction = item.ListActionItem.First();
						switch (iMAction.Type)
						{
						case KeyActionType.Pan:
							if (((Pan)iMAction).IsLookAroundEnabled || ((Pan)iMAction).IsShootOnClickEnabled)
							{
								((UIElement)item).Visibility = (Visibility)1;
								continue;
							}
							break;
						case KeyActionType.PanShoot:
							Logger.Info("Position: " + val.Type + " " + val.LButtonX + " " + val.LButtonY);
							if (val.LButtonShowOnOverlay && iMAction is PanShoot panShoot)
							{
								double num5 = (string.IsNullOrEmpty(panShoot.LButtonXOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonXOverlayOffset, CultureInfo.InvariantCulture));
								double num6 = (string.IsNullOrEmpty(panShoot.LButtonYOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonYOverlayOffset, CultureInfo.InvariantCulture));
								item.SetElementLayout(isLoaded: true, val.LButtonX + num5, val.LButtonY + num6);
								item.mXPosition = val.LButtonX + num5;
								item.mYPosition = val.LButtonY + num6;
								((UIElement)item).Visibility = (Visibility)0;
							}
							else
							{
								((UIElement)item).Visibility = (Visibility)1;
							}
							continue;
						case KeyActionType.LookAround:
							Logger.Info("Position: " + val.Type + " " + val.LookAroundX + " " + val.LookAroundY);
							if (val.LookAroundShowOnOverlay && iMAction is LookAround lookAround)
							{
								double num3 = (string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, CultureInfo.InvariantCulture));
								double num4 = (string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, CultureInfo.InvariantCulture));
								item.SetElementLayout(isLoaded: true, val.LookAroundX + num3, val.LookAroundY + num4);
								item.mXPosition = val.LookAroundX + num3;
								item.mYPosition = val.LookAroundY + num4;
								((UIElement)item).Visibility = (Visibility)0;
							}
							else
							{
								((UIElement)item).Visibility = (Visibility)1;
							}
							continue;
						case KeyActionType.MOBASkillCancel:
							Logger.Info("Position: " + val.Type + " " + val.CancelX + " " + val.CancelY);
							if (val.CancelShowOnOverlay && iMAction is MOBASkillCancel mOBASkillCancel)
							{
								double num7 = (string.IsNullOrEmpty(mOBASkillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mOBASkillCancel.MOBASkillCancelOffsetX, CultureInfo.InvariantCulture));
								double num8 = (string.IsNullOrEmpty(mOBASkillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mOBASkillCancel.MOBASkillCancelOffsetX, CultureInfo.InvariantCulture));
								item.SetElementLayout(isLoaded: true, val.CancelX + num7, val.CancelY + num8);
								item.mXPosition = val.CancelX + num7;
								item.mYPosition = val.CancelY + num8;
								((UIElement)item).Visibility = (Visibility)0;
							}
							else
							{
								((UIElement)item).Visibility = (Visibility)1;
							}
							continue;
						case KeyActionType.Dpad:
							if (iMAction is Dpad { IsMOBADpadEnabled: not false } dpad)
							{
								Logger.Info("Position: " + val.Type + " " + val.X + " " + val.Y);
								double num = (string.IsNullOrEmpty(dpad.mMOBADpad.XOverlayOffset) ? 0.0 : Convert.ToDouble(dpad.mMOBADpad.XOverlayOffset, CultureInfo.InvariantCulture));
								double num2 = (string.IsNullOrEmpty(dpad.mMOBADpad.YOverlayOffset) ? 0.0 : Convert.ToDouble(dpad.mMOBADpad.YOverlayOffset, CultureInfo.InvariantCulture));
								item.SetElementLayout(isLoaded: true, val.X + num, val.Y + num2);
								item.mXPosition = val.X + num;
								item.mYPosition = val.Y + num2;
								((UIElement)item).Visibility = (Visibility)0;
								continue;
							}
							break;
						case KeyActionType.Callback:
							Logger.Info("Callback: IsEnabled2 : " + val.Enabled);
							HandleCallbackPrimitive(mainWindow, val, (iMAction as Callback).Action, (iMAction as Callback).Id);
							continue;
						}
						Logger.Info("Position: " + val.Type + " " + val.X + " " + val.Y);
						IMAction iMAction2 = iMAction;
						double num9 = (string.IsNullOrEmpty(iMAction2.XOverlayOffset) ? 0.0 : Convert.ToDouble(iMAction2.XOverlayOffset, CultureInfo.InvariantCulture));
						double num10 = (string.IsNullOrEmpty(iMAction2.YOverlayOffset) ? 0.0 : Convert.ToDouble(iMAction2.YOverlayOffset, CultureInfo.InvariantCulture));
						item.SetElementLayout(isLoaded: true, val.X + num9, val.Y + num10);
						item.mXPosition = val.X + num9;
						item.mYPosition = val.Y + num10;
						((UIElement)item).Visibility = (Visibility)0;
					}
				}
				else
				{
					DynamicOverlayConfig val2 = DynamicOverlayConfigControls.Instance.GameControls[i];
					CanvasElement canvasElement = listCanvasElement[i][0];
					if (canvasElement != null && canvasElement.ListActionItem.First().Type == KeyActionType.Callback)
					{
						Logger.Info("Callback: IsEnabled3 : " + val2.Enabled);
						IMAction iMAction3 = canvasElement.ListActionItem.First();
						canvasElement.mXPosition = val2.X;
						canvasElement.mYPosition = val2.Y;
						HandleCallbackPrimitive(mainWindow, val2, (iMAction3 as Callback).Action, (iMAction3 as Callback).Id);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("ERROR : GameControl not found in canvas elements. " + ex);
		}
	}

	private static void HandleCallbackPrimitive(MainWindow mainWindow, DynamicOverlayConfig item, string action, string id)
	{
		switch (action)
		{
		case "Api":
			mainWindow.mCallbackEnabled = item.Enabled.ToString(CultureInfo.InvariantCulture);
			Logger.Info("Callback: HandleCallbackPrimitive(): " + mainWindow.mCallbackEnabled);
			break;
		case "Onboarding":
			if (item.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				if (!mIsEnabledStateChanged && id.Equals("Step2", StringComparison.InvariantCultureIgnoreCase))
				{
					mOnboardingCounter++;
					mIsEnabledStateChanged = true;
				}
				dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(item.X, item.Y);
			}
			else if (!id.Equals("Step1", StringComparison.InvariantCultureIgnoreCase) && mOnboardingCounter > 1)
			{
				mIsEnabledStateChanged = false;
				dictOverlayWindow[mainWindow]?.ShowOnboardingOverlayControl(item.X, item.Y, isVisible: false);
			}
			break;
		}
	}

	private static void ShowOverlayWindowAfterReparenting(MainWindow window)
	{
		((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (window != null)
			{
				window.mFrontendHandler.ShowGLWindow();
				if (dictOverlayWindow.ContainsKey(window))
				{
					KeymapCanvasWindow keymapCanvasWindow = dictOverlayWindow[window];
					if (keymapCanvasWindow != null && !keymapCanvasWindow.mIsClosing)
					{
						((UIElement)keymapCanvasWindow.mCanvas).Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
						if (window.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero)
						{
							((Window)keymapCanvasWindow).Show();
							if (sIsInScriptEditingMode && CanvasWindow.SidebarWindow != null)
							{
								((Window)CanvasWindow.SidebarWindow).Owner = (Window)(object)keymapCanvasWindow;
								((Window)CanvasWindow.SidebarWindow).Activate();
							}
						}
					}
				}
			}
		}, new object[0]);
	}

	internal static void ChangeTransparency(MainWindow window, double value)
	{
		if (window != null && dictOverlayWindow.ContainsKey(window))
		{
			((UIElement)dictOverlayWindow[window].mCanvas).Opacity = value;
		}
		RegistryManager.Instance.TranslucentControlsTransparency = value;
	}

	internal static void CloseWindows()
	{
		if (sGuidanceWindow != null && !((CustomWindow)sGuidanceWindow).IsClosed)
		{
			try
			{
				((Window)sGuidanceWindow).Close();
			}
			catch (Exception ex)
			{
				Logger.Error("exception closing GameControlWindow " + ex.ToString());
			}
		}
		if (CanvasWindow != null && !CanvasWindow.mIsClosing)
		{
			try
			{
				((Window)CanvasWindow.SidebarWindow).Close();
			}
			catch (Exception ex2)
			{
				Logger.Error("exception closing GameControlWindow " + ex2.ToString());
			}
		}
	}

	internal static void LoadIMActions(MainWindow mainWindow, string packageName)
	{
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		Logger.Debug("Extralog: LoadImAction called. vmName:" + mainWindow.mVmName + "..." + Environment.StackTrace);
		sPackageName = packageName;
		string inputmapperFile = Utils.GetInputmapperFile(sPackageName);
		try
		{
			ClearConfig(mainWindow);
			if (File.Exists(inputmapperFile))
			{
				mainWindow.SelectedConfig = GetDeserializedIMConfigObject(inputmapperFile);
				if (mainWindow.SelectedConfig.ControlSchemes.Any())
				{
					foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
					{
						if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(controlScheme.Name))
						{
							if (mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name].BuiltIn)
							{
								mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name] = controlScheme;
							}
						}
						else
						{
							mainWindow.SelectedConfig.ControlSchemesDict[controlScheme.Name] = controlScheme;
						}
					}
					IMControlScheme iMControlScheme = mainWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.Selected).FirstOrDefault();
					mainWindow.SelectedConfig.SelectedControlScheme = iMControlScheme ?? mainWindow.SelectedConfig.ControlSchemes[0];
				}
				mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
			}
			CheckForShootingModeTooltip(mainWindow);
			if (!AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName].ContainsKey(packageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName] = new AppSettings();
			}
			if (!AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded)
			{
				ClientStats.SendMiscellaneousStatsAsync("DefaultScheme", RegistryManager.Instance.UserGuid, packageName, mainWindow.SelectedConfig?.SelectedControlScheme?.Name, null, null);
				AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded = true;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error parsing file " + inputmapperFile + ex.ToString());
		}
	}

	internal static void SendSchemeChangedStats(MainWindow window, string source = "")
	{
		ClientStats.SendMiscellaneousStatsAsync("SchemeChanged", RegistryManager.Instance.UserGuid, sPackageName, window.SelectedConfig.SelectedControlScheme.Name, source, null);
	}

	internal static void PopulateMOBADpadAsChildOfDpad(IMConfig imConfigObj)
	{
		foreach (IMControlScheme controlScheme in imConfigObj.ControlSchemes)
		{
			List<Dpad> list = new List<Dpad>();
			List<MOBADpad> list2 = new List<MOBADpad>();
			foreach (IMAction gameControl in controlScheme.GameControls)
			{
				if (gameControl.Type == KeyActionType.Dpad)
				{
					Dpad item = gameControl as Dpad;
					list.Add(item);
				}
				else if (gameControl.Type == KeyActionType.MOBADpad)
				{
					MOBADpad item2 = gameControl as MOBADpad;
					list2.Add(item2);
				}
			}
			foreach (MOBADpad item3 in list2)
			{
				foreach (Dpad item4 in list)
				{
					if (item3.X.Equals(item4.X) && item3.Y.Equals(item4.Y))
					{
						item4.mMOBADpad = item3;
						item3.mDpad = item4;
						item3.ParentAction = item4;
						break;
					}
				}
			}
		}
	}

	internal static MOBADpad GetMOBADPad(MainWindow mainWindow)
	{
		foreach (IMAction gameControl in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
		{
			if (gameControl.Type == KeyActionType.MOBADpad)
			{
				MOBADpad mOBADpad = gameControl as MOBADpad;
				if (mOBADpad.OriginX != -1.0 && mOBADpad.OriginY != -1.0)
				{
					return mOBADpad;
				}
			}
		}
		return null;
	}

	internal static IMConfig GetDeserializedIMConfigObject(string fileName, bool isFileNameUsed = true)
	{
		IMConfig iMConfig = null;
		if (!isFileNameUsed)
		{
			iMConfig = JsonConvert.DeserializeObject<IMConfig>(fileName, Utils.GetSerializerSettings());
		}
		else
		{
			bool flag = false;
			string text = "";
			using (Mutex mutex = new Mutex(initiallyOwned: false, "BlueStacks_CfgAccess"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						text = File.ReadAllText(fileName);
						flag = true;
					}
					catch (Exception arg)
					{
						Logger.Error($"Failed to read cfg file... filepath: {fileName} Err : {arg}");
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			if (!flag)
			{
				throw new Exception("Could not read file " + fileName);
			}
			iMConfig = JsonConvert.DeserializeObject<IMConfig>(text, Utils.GetSerializerSettings());
		}
		PopulateMOBADpadAsChildOfDpad(iMConfig);
		return iMConfig;
	}

	internal static IMControlScheme GetNewControlSchemes(string name)
	{
		return new IMControlScheme
		{
			Name = name
		};
	}

	internal static ComboBoxSchemeControl GetComboBoxSchemeControlFromName(string schemeName)
	{
		foreach (ComboBoxSchemeControl child in ((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children)
		{
			if (((TextBox)child.mSchemeName).Text == schemeName)
			{
				return child;
			}
		}
		return null;
	}

	internal static void AddNewControlSchemeAndSelect(MainWindow mainWindow, IMControlScheme toCopyFromScheme = null, bool isCopyOrNew = false)
	{
		bool flag = false;
		if (toCopyFromScheme != null && toCopyFromScheme.Selected && string.Equals(mainWindow.SelectedConfig?.SelectedControlScheme.Name, toCopyFromScheme.Name, StringComparison.InvariantCulture))
		{
			flag = true;
		}
		IMControlScheme iMControlScheme;
		if (toCopyFromScheme != null)
		{
			iMControlScheme = toCopyFromScheme.DeepCopy();
			if (flag)
			{
				List<IMAction> gameControls = toCopyFromScheme.GameControls;
				toCopyFromScheme.SetGameControls(iMControlScheme.GameControls);
				iMControlScheme.SetGameControls(gameControls);
			}
		}
		else
		{
			iMControlScheme = new IMControlScheme();
		}
		iMControlScheme.Name = GetNewSchemeName(mainWindow, toCopyFromScheme, isCopyOrNew);
		iMControlScheme.Selected = true;
		iMControlScheme.BuiltIn = false;
		mainWindow.SelectedConfig.ControlSchemes.Add(iMControlScheme);
		bool isBookMarked = false;
		if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(iMControlScheme.Name))
		{
			if (mainWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name].BuiltIn)
			{
				isBookMarked = mainWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name].IsBookMarked;
				mainWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name] = iMControlScheme;
			}
		}
		else
		{
			mainWindow.SelectedConfig.ControlSchemesDict.Add(iMControlScheme.Name, iMControlScheme);
		}
		iMControlScheme.IsBookMarked = isBookMarked;
		if (isCopyOrNew && CanvasWindow != null && CanvasWindow.SidebarWindow != null)
		{
			CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = iMControlScheme.Name;
			ComboBoxSchemeControl comboBoxSchemeControl = new ComboBoxSchemeControl(CanvasWindow, mainWindow);
			((TextBox)comboBoxSchemeControl.mSchemeName).Text = LocaleStrings.GetLocalizedString(iMControlScheme.Name, "");
			((UIElement)comboBoxSchemeControl).IsEnabled = true;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)comboBoxSchemeControl, Control.BackgroundProperty, "AdvancedGameControlButtonGridBackground");
			((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children.Add((UIElement)(object)comboBoxSchemeControl);
		}
		if (mainWindow.SelectedConfig.SelectedControlScheme != null)
		{
			if (CanvasWindow != null && CanvasWindow.SidebarWindow != null)
			{
				ComboBoxSchemeControl comboBoxSchemeControlFromName = GetComboBoxSchemeControlFromName(mainWindow.SelectedConfig.SelectedControlScheme.Name);
				if (comboBoxSchemeControlFromName != null)
				{
					BlueStacksUIBinding.BindColor((DependencyObject)(object)comboBoxSchemeControlFromName, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				}
			}
			mainWindow.SelectedConfig.SelectedControlScheme.Selected = false;
		}
		mainWindow.SelectedConfig.SelectedControlScheme = iMControlScheme;
		KeymapCanvasWindow.sIsDirty = true;
		if (!flag && CanvasWindow != null)
		{
			CanvasWindow.Init();
		}
		if (isCopyOrNew && CanvasWindow != null && CanvasWindow.SidebarWindow != null)
		{
			CanvasWindow.SidebarWindow.FillProfileCombo();
		}
	}

	private static void AddNewControlSchemeAndSelectImap(MainWindow mainWindow, IMControlScheme builtInScheme)
	{
		IMControlScheme iMControlScheme = mainWindow.SelectedConfig?.SelectedControlScheme;
		int? num = mainWindow?.SelectedConfig?.ControlSchemes.IndexOf(iMControlScheme);
		if (num.HasValue && num > -1)
		{
			mainWindow.SelectedConfig.ControlSchemes[num.Value] = builtInScheme.DeepCopy();
			mainWindow.SelectedConfig.ControlSchemes[num.Value].IsBookMarked = false;
		}
		iMControlScheme.BuiltIn = false;
		foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
		{
			controlScheme.Selected = false;
		}
		mainWindow.SelectedConfig.ControlSchemes.Add(iMControlScheme);
		if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(iMControlScheme.Name))
		{
			mainWindow.SelectedConfig.ControlSchemesDict[iMControlScheme.Name] = iMControlScheme;
		}
		mainWindow.SelectedConfig.SelectedControlScheme = iMControlScheme;
		iMControlScheme.Selected = true;
	}

	internal static string GetNewSchemeName(MainWindow mainWindow, IMControlScheme builtInScheme, bool isCopyOrNew)
	{
		string text;
		if (builtInScheme == null)
		{
			text = "Custom";
		}
		else
		{
			text = builtInScheme.Name;
			if (builtInScheme.BuiltIn && !isCopyOrNew)
			{
				return text;
			}
		}
		List<string> list = new List<string>();
		foreach (IMControlScheme controlScheme in mainWindow.SelectedConfig.ControlSchemes)
		{
			list.Add(controlScheme.Name);
		}
		return GetUniqueName(text, list);
	}

	internal static string GetUniqueName(string baseName, IEnumerable<string> nameCollection)
	{
		int length = baseName.Length;
		int num = 0;
		bool flag = false;
		foreach (string item in nameCollection)
		{
			if (string.Compare(baseName, 0, item, 0, length, StringComparison.OrdinalIgnoreCase) != 0)
			{
				continue;
			}
			flag = true;
			if (item.Length > length + 3 && item[length] == ' ' && item[length + 1] == '(' && item[item.Length - 1] == ')')
			{
				int num2;
				try
				{
					num2 = int.Parse(item.Substring(length + 2, item.Length - length - 3), CultureInfo.InvariantCulture);
				}
				catch (Exception)
				{
					continue;
				}
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		if (!flag)
		{
			return baseName;
		}
		return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[2]
		{
			baseName,
			num + 1
		});
	}

	internal static bool IsValidCfg(string fileName)
	{
		try
		{
			if (JsonConvert.DeserializeObject(File.ReadAllText(fileName)) == null)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			Logger.Error("invalid cfg file: {0}", new object[1] { fileName });
			return false;
		}
	}

	private static void CheckForShootingModeTooltip(MainWindow window)
	{
		try
		{
			foreach (IMAction gameControl in window.SelectedConfig.SelectedControlScheme.GameControls)
			{
				if (gameControl.Type == KeyActionType.Pan)
				{
					sShootingModeKey = ((Pan)gameControl).KeyStartStop.ToString(CultureInfo.InvariantCulture);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in showing shooting mode tooltip: " + ex.ToString());
		}
	}

	internal static void ClearConfig(MainWindow mainWindow)
	{
		MOBADpad.sListMOBADpad.Clear();
		Dpad.sListDpad.Clear();
		mainWindow.SelectedConfig = null;
		mainWindow.OriginalLoadedConfig = null;
	}

	internal static void GetCanvasElement(MainWindow mainWindow, IMAction action, Canvas canvas, bool addToCanvas = true)
	{
		sDragCanvasElement = new CanvasElement(CanvasWindow, mainWindow);
		sDragCanvasElement.AddAction(action);
		((UIElement)sDragCanvasElement).Opacity = 0.1;
		if (addToCanvas)
		{
			((Panel)canvas).Children.Add((UIElement)(object)sDragCanvasElement);
		}
		if (action.Type != KeyActionType.Swipe)
		{
			return;
		}
		AssignSwapValues(action);
		List<Direction> list = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
		list.Remove(action.Direction);
		foreach (Direction item in list)
		{
			IMAction iMAction = UsefulExtensionMethod.DeepCopy<IMAction>(action);
			iMAction.Direction = item;
			iMAction.RadiusProperty = action.RadiusProperty;
			AssignSwapValues(iMAction);
			sDragCanvasElement.AddAction(iMAction);
		}
		action.RadiusProperty = action.RadiusProperty;
	}

	private static void AssignSwapValues(IMAction action)
	{
		if (action.Direction == Direction.Up)
		{
			(action as Swipe).Key = IMAPKeys.GetStringForUI((Key)24);
		}
		else if (action.Direction == Direction.Down)
		{
			(action as Swipe).Key = IMAPKeys.GetStringForUI((Key)26);
		}
		else if (action.Direction == Direction.Left)
		{
			(action as Swipe).Key = IMAPKeys.GetStringForUI((Key)23);
		}
		else
		{
			(action as Swipe).Key = IMAPKeys.GetStringForUI((Key)25);
		}
	}

	internal static List<IMAction> ClearElement()
	{
		List<IMAction> result = null;
		if (sDragCanvasElement != null)
		{
			result = sDragCanvasElement.ListActionItem;
			DependencyObject parent = ((FrameworkElement)sDragCanvasElement).Parent;
			DependencyObject obj = ((parent is Panel) ? parent : null);
			if (obj != null)
			{
				((Panel)obj).Children.Remove((UIElement)(object)sDragCanvasElement);
			}
			sDragCanvasElement = null;
		}
		return result;
	}

	internal static void RepositionCanvasElement()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (sDragCanvasElement != null)
		{
			DependencyObject parent = ((FrameworkElement)sDragCanvasElement).Parent;
			Point position = Mouse.GetPosition((IInputElement)(object)((parent is IInputElement) ? parent : null));
			Canvas.SetTop((UIElement)(object)sDragCanvasElement, ((Point)(ref position)).Y - ((FrameworkElement)sDragCanvasElement).ActualHeight / 2.0);
			Canvas.SetLeft((UIElement)(object)sDragCanvasElement, ((Point)(ref position)).X - ((FrameworkElement)sDragCanvasElement).ActualWidth / 2.0);
		}
	}

	internal static void SaveIMActions(MainWindow mainWindow, bool isSavedFromGameControlWindow, bool isdDeleteIfEmpty = false)
	{
		Logger.Debug($"ExtraLog:Calling SaveIMActions, VmName:{mainWindow.mVmName}, Scheme:{mainWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{mainWindow.SelectedConfig.ControlSchemes.Count}");
		if (!KeymapCanvasWindow.sIsDirty && !GuidanceWindow.sIsDirty && !isdDeleteIfEmpty)
		{
			Logger.Info("No changes were made in config file. Not saving");
			return;
		}
		sPackageName = mainWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
		KeymapCanvasWindow.sIsDirty = false;
		GuidanceWindow.sIsDirty = false;
		sGamepadDualTextbox = null;
		string inputmapperUserFilePath = Utils.GetInputmapperUserFilePath(sPackageName);
		CheckForShootingModeTooltip(mainWindow);
		try
		{
			string directoryName = Path.GetDirectoryName(inputmapperUserFilePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			CleanupGuidanceAccordingToSchemes(mainWindow.SelectedConfig.ControlSchemes, mainWindow.SelectedConfig.Strings);
			SaveAndUpdateKeymapUI(mainWindow, isSavedFromGameControlWindow, inputmapperUserFilePath);
		}
		catch (Exception ex)
		{
			Logger.Error("Error saving file  for " + inputmapperUserFilePath + Environment.NewLine + ex.ToString());
		}
	}

	private static void SaveAndUpdateKeymapUI(MainWindow mainWindow, bool isSavedFromGameControlWindow, string path)
	{
		Logger.Debug($"ExtraLog:Calling SaveAndUpdateKeymapUI, VmName:{mainWindow.mVmName}, Scheme:{mainWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{mainWindow.SelectedConfig.ControlSchemes.Count}");
		try
		{
			mainWindow.SelectedConfig.MetaData.ParserVersion = ParserVersion;
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)1;
			string contents = JsonConvert.SerializeObject((object)mainWindow.SelectedConfig, serializerSettings);
			bool callUpdateGrm = false;
			if (!File.Exists(path))
			{
				callUpdateGrm = true;
			}
			using (Mutex mutex = new Mutex(initiallyOwned: false, "BlueStacks_CfgAccess"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						Logger.Debug($"ExtraLog:Calling WriteAllText, VmName:{mainWindow.mVmName}, Scheme:{mainWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{mainWindow.SelectedConfig.ControlSchemes.Count}");
						File.WriteAllText(path, contents);
					}
					catch (Exception arg)
					{
						Logger.Error($"Failed to write cfg path: {path} Err : {arg}");
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			Logger.Debug($"ExtraLog:Updating Original Config, VmName:{mainWindow.mVmName}, Scheme:{mainWindow.SelectedConfig.SelectedControlScheme.Name}, SchemeCount:{mainWindow.SelectedConfig.ControlSchemes.Count}");
			mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
			bool isEnabled = false;
			if (mainWindow.OriginalLoadedConfig.ControlSchemes != null && mainWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
			{
				isEnabled = true;
			}
			else
			{
				isEnabled = false;
			}
			((DispatcherObject)mainWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (CanvasWindow != null && CanvasWindow.SidebarWindow != null)
				{
					((UIElement)CanvasWindow.SidebarWindow.mExport).IsEnabled = isEnabled;
				}
				mainWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(isEnabled);
				ClientStats.SendKeyMappingUIStatsAsync("cfg_saved", sPackageName, isSavedFromGameControlWindow ? "edit_keys" : "advanced");
				if (callUpdateGrm)
				{
					GrmHandler.RequirementConfigUpdated(mainWindow.mVmName);
				}
				BlueStacksUIUtils.RefreshKeyMap(sPackageName);
				if (CanvasWindow != null && !CanvasWindow.IsInOverlayMode)
				{
					CanvasWindow.Init();
				}
				if (dictGamepadEligibility.ContainsKey(sPackageName))
				{
					dictGamepadEligibility.Remove(sPackageName);
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SaveAndUpdateKeymapUI.." + ex.ToString());
		}
	}

	internal static void SaveConfigToFile(string path, IMConfig config)
	{
		JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
		serializerSettings.Formatting = (Formatting)1;
		string contents = JsonConvert.SerializeObject((object)config, serializerSettings);
		File.WriteAllText(path, contents);
	}

	internal static bool CheckIfKeymappingWindowVisible(bool checkForGuidanceWindow = false)
	{
		bool isVisible = false;
		bool guidanceWindowVisible = false;
		try
		{
			((DispatcherObject)BlueStacksUIUtils.LastActivatedWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				if (sGuidanceWindow != null && (((Window)sGuidanceWindow).IsActive || ((UIElement)sGuidanceWindow).IsVisible))
				{
					guidanceWindowVisible = true;
				}
				if (CanvasWindow != null && ((Window)CanvasWindow).IsActive)
				{
					isVisible = true;
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Info("Exception in checkifkeymappingwindowvisible: " + ex.ToString());
		}
		if (checkForGuidanceWindow)
		{
			return guidanceWindowVisible;
		}
		return isVisible;
	}

	internal static void CallGamepadHandler(MainWindow mainWindow, string isEnable = "true")
	{
		Dictionary<string, string> data = new Dictionary<string, string> { { "enable", isEnable } };
		mainWindow.mFrontendHandler.SendFrontendRequestAsync("toggleGamepadButton", data);
	}

	internal static string GetStringsToShowInUI(string text)
	{
		string[] array = text.ToString(CultureInfo.InvariantCulture).Split(new char[1] { '+' }, StringSplitOptions.RemoveEmptyEntries);
		string result = string.Empty;
		if (array.Length == 2)
		{
			string stringForUI = IMAPKeys.GetStringForUI(array[0].Trim());
			string stringForUI2 = IMAPKeys.GetStringForUI(array[1].Trim());
			result = Constants.ImapLocaleStringsConstant + stringForUI + " + " + stringForUI2;
		}
		else if (array.Length == 1)
		{
			string stringForUI = IMAPKeys.GetStringForUI(array[0].Trim());
			result = Constants.ImapLocaleStringsConstant + stringForUI;
		}
		return result;
	}

	internal static Dictionary<string, Dictionary<string, string>> CleanupGuidanceAccordingToSchemes(List<IMControlScheme> schemes, Dictionary<string, Dictionary<string, string>> locales)
	{
		HashSet<string> guidanceInUse = new HashSet<string>();
		foreach (IMControlScheme scheme in schemes)
		{
			foreach (IMAction gameControl in scheme.GameControls)
			{
				guidanceInUse.UnionWith(gameControl.Guidance.Values);
				guidanceInUse.Add(gameControl.GuidanceCategory);
			}
		}
		foreach (string key in locales.Keys)
		{
			foreach (KeyValuePair<string, string> item in locales[key].Where((KeyValuePair<string, string> kv) => !guidanceInUse.Contains(kv.Key)).ToList())
			{
				locales[key].Remove(item.Key);
			}
		}
		return locales;
	}

	public static string GetPackageFromCfgFile(string cfgFileName)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(cfgFileName))
		{
			result = Path.GetFileNameWithoutExtension(cfgFileName);
		}
		return result;
	}

	public static void MergeConfig(string pdPath)
	{
		Logger.Info("In MergeConfig");
		try
		{
			string text = Path.Combine(pdPath, "Engine\\UserData\\InputMapper\\UserFiles");
			string[] files = Directory.GetFiles(text);
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = new FileInfo(files[i]);
				string path = Path.Combine(pdPath, "Engine\\UserData\\InputMapper");
				string text2 = Path.Combine(path, fileInfo.Name);
				string text3 = Path.Combine(text, fileInfo.Name);
				IMConfig deserializedIMConfigObject = GetDeserializedIMConfigObject(text3);
				if (deserializedIMConfigObject.ControlSchemes.Count == 1)
				{
					deserializedIMConfigObject.ControlSchemes[0].Selected = true;
				}
				if (!File.Exists(text2))
				{
					SaveConfigToFile(Path.Combine(path, "UserFiles\\" + fileInfo.Name), deserializedIMConfigObject);
				}
				else
				{
					ControlSchemesHandling(GetPackageFromCfgFile(fileInfo.Name), text3, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in merging cfg. err: " + ex.ToString());
		}
	}

	internal static void MergeConflictingGuidanceStrings(IMConfig newConfig, List<IMControlScheme> toCopyFromSchemes, Dictionary<string, Dictionary<string, string>> stringsToImport)
	{
		HashSet<string> hashSet = new HashSet<string>();
		HashSet<string> hashSet2 = new HashSet<string>();
		foreach (string key in stringsToImport.Keys)
		{
			hashSet2.UnionWith(stringsToImport[key].Keys);
			if (!Enumerable.Contains(newConfig.Strings.Keys, key))
			{
				continue;
			}
			hashSet2.UnionWith(newConfig.Strings[key].Keys);
			foreach (string key2 in stringsToImport[key].Keys)
			{
				if (Enumerable.Contains(newConfig.Strings[key].Keys, key2) && stringsToImport[key][key2] != newConfig.Strings[key][key2])
				{
					hashSet.Add(key2);
				}
			}
		}
		foreach (string item in hashSet)
		{
			string uniqueName = GetUniqueName(item, hashSet2);
			foreach (IMControlScheme toCopyFromScheme in toCopyFromSchemes)
			{
				foreach (IMAction gameControl in toCopyFromScheme.GameControls)
				{
					if (gameControl.GuidanceCategory == item)
					{
						gameControl.GuidanceCategory = uniqueName;
					}
					foreach (string key3 in gameControl.Guidance.Keys)
					{
						if (gameControl.Guidance[key3] == item)
						{
							gameControl.Guidance[key3] = uniqueName;
							break;
						}
					}
				}
			}
			foreach (Dictionary<string, string> value in stringsToImport.Values)
			{
				if (value.ContainsKey(item))
				{
					value[uniqueName] = value[item];
					value.Remove(item);
				}
			}
		}
		foreach (KeyValuePair<string, Dictionary<string, string>> item2 in stringsToImport)
		{
			if (newConfig.Strings.ContainsKey(item2.Key))
			{
				foreach (KeyValuePair<string, string> item3 in item2.Value)
				{
					newConfig.Strings[item2.Key][item3.Key] = item3.Value;
				}
			}
			else
			{
				newConfig.Strings[item2.Key] = item2.Value;
			}
		}
	}

	public static void ControlSchemesHandlingWhileCfgUpdateFromCloud(string package)
	{
		string path = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
		string userFilesCfgPath = string.Format(CultureInfo.InvariantCulture, Path.Combine(path, package) + ".cfg", new object[0]);
		string inputMapperCfgPath = string.Format(CultureInfo.InvariantCulture, Path.Combine(RegistryStrings.InputMapperFolder, package) + ".cfg", new object[0]);
		ControlSchemesHandlingFromCloud(package, userFilesCfgPath, inputMapperCfgPath);
	}

	private static void ControlSchemesHandling(string package, string userFilesCfgPath, string inputMapperCfgPath)
	{
		try
		{
			if (!File.Exists(userFilesCfgPath))
			{
				return;
			}
			IMConfig deserializedIMConfigObject = GetDeserializedIMConfigObject(inputMapperCfgPath);
			IMConfig deserializedIMConfigObject2 = GetDeserializedIMConfigObject(userFilesCfgPath);
			MergeConflictingGuidanceStrings(deserializedIMConfigObject, deserializedIMConfigObject2.ControlSchemes, deserializedIMConfigObject2.Strings);
			deserializedIMConfigObject2.Strings = deserializedIMConfigObject.Strings;
			List<IMControlScheme> list = new List<IMControlScheme>();
			foreach (IMControlScheme controlScheme in deserializedIMConfigObject.ControlSchemes)
			{
				if (controlScheme.BuiltIn)
				{
					list.Add(controlScheme);
				}
			}
			IMControlScheme selectedScheme;
			bool flag = IsBuiltInSchemeSelected(deserializedIMConfigObject2, out selectedScheme);
			List<IMControlScheme> list2 = new List<IMControlScheme>();
			foreach (IMControlScheme controlScheme2 in deserializedIMConfigObject2.ControlSchemes)
			{
				if (controlScheme2.BuiltIn)
				{
					list2.Add(controlScheme2);
				}
			}
			foreach (IMControlScheme item in list2)
			{
				deserializedIMConfigObject2.ControlSchemes.Remove(item);
			}
			List<string> list3 = list.Select((IMControlScheme scheme) => scheme.Name).ToList();
			List<string> list4 = deserializedIMConfigObject2.ControlSchemes.Select((IMControlScheme scheme) => scheme.Name).ToList();
			list4.AddRange(list3);
			string text = " (Custom)";
			foreach (IMControlScheme controlScheme3 in deserializedIMConfigObject2.ControlSchemes)
			{
				if (list3.Contains(controlScheme3.Name))
				{
					controlScheme3.Name = GetUniqueName(controlScheme3.Name + text, list4);
					list4.Add(controlScheme3.Name);
				}
			}
			foreach (IMControlScheme item2 in list)
			{
				deserializedIMConfigObject2.ControlSchemes.Add(item2);
			}
			if (!flag)
			{
				foreach (IMControlScheme controlScheme4 in deserializedIMConfigObject2.ControlSchemes)
				{
					if (controlScheme4.BuiltIn)
					{
						if (!Opt.Instance.isUpgradeFromImap13 || !string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
						{
							controlScheme4.Selected = false;
						}
					}
					else if (Opt.Instance.isUpgradeFromImap13 && string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
					{
						controlScheme4.Selected = false;
					}
				}
			}
			else if (selectedScheme != null)
			{
				IMControlScheme iMControlScheme = null;
				bool flag2 = false;
				foreach (IMControlScheme controlScheme5 in deserializedIMConfigObject2.ControlSchemes)
				{
					if (controlScheme5.BuiltIn)
					{
						if (controlScheme5.Name == selectedScheme.Name)
						{
							controlScheme5.Selected = true;
							flag2 = true;
						}
						else if (controlScheme5.Selected)
						{
							iMControlScheme = controlScheme5;
						}
					}
				}
				if (iMControlScheme != null && flag2)
				{
					iMControlScheme.Selected = false;
				}
			}
			if (string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || ThirdParty.AllCallOfDutyPackageNames.Any((string codPckg) => string.Equals(codPckg, package, StringComparison.InvariantCultureIgnoreCase)))
			{
				Dictionary<string, IMControlScheme> dictionary = new Dictionary<string, IMControlScheme>();
				foreach (IMControlScheme item3 in list2)
				{
					if (item3.Images != null && item3.Images.Count > 0)
					{
						string key = JsonConvert.SerializeObject((object)item3.Images, Utils.GetSerializerSettings());
						dictionary.Add(key, item3);
					}
				}
				foreach (IMControlScheme controlScheme6 in deserializedIMConfigObject2.ControlSchemes)
				{
					if (controlScheme6.Images == null || controlScheme6.BuiltIn || controlScheme6.Images.Count <= 0)
					{
						continue;
					}
					string images = JsonConvert.SerializeObject((object)controlScheme6.Images, Utils.GetSerializerSettings());
					IEnumerable<KeyValuePair<string, IMControlScheme>> prevSchemesMatchingImages = dictionary.Where((KeyValuePair<string, IMControlScheme> kvp) => string.Compare(kvp.Key, images, StringComparison.InvariantCultureIgnoreCase) == 0);
					if (prevSchemesMatchingImages.Any())
					{
						IEnumerable<IMControlScheme> source = list.Where((IMControlScheme newScheme) => string.Compare(newScheme.Name, prevSchemesMatchingImages.First().Value.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
						if (source.Any() && source.First().Images != null && source.First().Images.Any())
						{
							controlScheme6.SetImages(source.First().Images);
						}
					}
				}
			}
			SaveConfigToFile(userFilesCfgPath, deserializedIMConfigObject2);
		}
		catch (Exception ex)
		{
			Logger.Error("Error in updating control schemes err: " + ex.ToString());
		}
	}

	internal static void SelectSchemeIfPresent(MainWindow window, string schemeNameToSelect, string statSource, bool forceSave)
	{
		IEnumerable<IMControlScheme> source = window.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeNameToSelect, StringComparison.InvariantCulture));
		IMControlScheme iMControlScheme = null;
		bool flag = true;
		if (source.Any())
		{
			iMControlScheme = ((source.Count() != 1) ? source.Where((IMControlScheme scheme) => !scheme.BuiltIn).FirstOrDefault() : source.FirstOrDefault());
			if (iMControlScheme == null || (iMControlScheme.Name == window.SelectedConfig.SelectedControlScheme.Name && !forceSave))
			{
				flag = false;
			}
			else
			{
				window.SelectedConfig.SelectedControlScheme.Selected = false;
				iMControlScheme.Selected = true;
				window.SelectedConfig.SelectedControlScheme = iMControlScheme;
			}
		}
		if (flag)
		{
			KeymapCanvasWindow.sIsDirty = true;
			SaveIMActions(window, isSavedFromGameControlWindow: false);
			if (dictOverlayWindow.ContainsKey(window) && dictOverlayWindow[window] != null && RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				ShowOverlayWindow(window, isShow: true, isreload: true);
			}
			BlueStacksUIUtils.RefreshKeyMap(sPackageName);
			SendSchemeChangedStats(window, statSource);
		}
	}

	private static bool IsBuiltInSchemeSelected(IMConfig prevConfig, out IMControlScheme selectedScheme)
	{
		selectedScheme = null;
		foreach (IMControlScheme controlScheme in prevConfig.ControlSchemes)
		{
			if (controlScheme.Selected && controlScheme.BuiltIn)
			{
				selectedScheme = controlScheme;
				return true;
			}
		}
		return false;
	}

	private static void ControlSchemesHandlingFromCloud(string package, string userFilesCfgPath, string inputMapperCfgPath)
	{
		try
		{
			if (!File.Exists(userFilesCfgPath))
			{
				return;
			}
			IMConfig deserializedIMConfigObject = GetDeserializedIMConfigObject(inputMapperCfgPath);
			IMConfig deserializedIMConfigObject2 = GetDeserializedIMConfigObject(userFilesCfgPath);
			MergeConflictingGuidanceStrings(deserializedIMConfigObject, deserializedIMConfigObject2.ControlSchemes, deserializedIMConfigObject2.Strings);
			IEnumerable<IMControlScheme> enumerable = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn);
			if (enumerable.Any())
			{
				foreach (IMControlScheme item in enumerable)
				{
					deserializedIMConfigObject.ControlSchemes.Add(item);
				}
			}
			string selectedSchemeName = string.Empty;
			IMControlScheme iMControlScheme = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => scheme.Selected).FirstOrDefault();
			if (iMControlScheme != null)
			{
				selectedSchemeName = iMControlScheme.Name;
			}
			deserializedIMConfigObject.ControlSchemes.ForEach(delegate(IMControlScheme scheme)
			{
				scheme.Selected = false;
			});
			if (iMControlScheme != null)
			{
				List<IMControlScheme> list = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, selectedSchemeName, StringComparison.InvariantCultureIgnoreCase)).ToList();
				if (list.Count == 1)
				{
					list[0].Selected = true;
				}
				else if (list.Count == 2)
				{
					IMControlScheme iMControlScheme2 = list.Where((IMControlScheme scheme) => !scheme.BuiltIn).FirstOrDefault();
					if (iMControlScheme2 != null)
					{
						iMControlScheme2.Selected = true;
					}
				}
			}
			IEnumerable<IMControlScheme> enumerable2 = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn);
			if (enumerable2.Any())
			{
				foreach (IMControlScheme userScheme in enumerable2)
				{
					IMControlScheme iMControlScheme3 = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn && string.Equals(scheme.Name, userScheme.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
					if (iMControlScheme3 != null)
					{
						iMControlScheme3.IsBookMarked = userScheme.IsBookMarked;
					}
				}
				if (string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || ThirdParty.AllCallOfDutyPackageNames.Any((string codPckg) => string.Equals(codPckg, package, StringComparison.InvariantCultureIgnoreCase)))
				{
					Dictionary<string, IMControlScheme> dictionary = new Dictionary<string, IMControlScheme>();
					foreach (IMControlScheme item2 in enumerable2)
					{
						if (item2.Images != null && item2.Images.Count > 0)
						{
							string key = JsonConvert.SerializeObject((object)item2.Images, Utils.GetSerializerSettings());
							dictionary.Add(key, item2);
						}
					}
					foreach (IMControlScheme item3 in deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn))
					{
						if (item3.Images == null || item3.Images.Count <= 0)
						{
							continue;
						}
						string key2 = JsonConvert.SerializeObject((object)item3.Images, Utils.GetSerializerSettings());
						if (dictionary.ContainsKey(key2))
						{
							IMControlScheme userSchemeMatchingBuiltInImage = dictionary[key2];
							IMControlScheme iMControlScheme4 = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme cloudScheme) => cloudScheme.BuiltIn && string.Equals(cloudScheme.Name, userSchemeMatchingBuiltInImage.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
							if (iMControlScheme4 != null && iMControlScheme4.Images != null && iMControlScheme4.Images.Any())
							{
								item3.SetImages(iMControlScheme4.Images);
							}
						}
					}
				}
			}
			SaveConfigToFile(userFilesCfgPath, deserializedIMConfigObject);
		}
		catch (Exception ex)
		{
			Logger.Error("Error in updating control schemes err: " + ex.ToString());
		}
	}

	internal static string CheckForGamepadSuffix(string text)
	{
		if (UsefulExtensionMethod.Contains(text, "Gamepad", StringComparison.InvariantCultureIgnoreCase) || UsefulExtensionMethod.Contains(text, "LeftStick", StringComparison.InvariantCultureIgnoreCase) || UsefulExtensionMethod.Contains(text, "RightStick", StringComparison.InvariantCultureIgnoreCase))
		{
			string value = ".";
			if (text.Contains(value))
			{
				text = text.Substring(0, text.IndexOf(value, StringComparison.InvariantCultureIgnoreCase));
			}
		}
		return text;
	}

	internal static string GetKeyUIValue(string text)
	{
		return string.Join(" + ", (from singleItem in text.Split(new char[1] { '+' }).ToList()
			select LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(CheckForGamepadSuffix(singleItem.Trim())), "")).ToArray());
	}

	internal static void ShowShootingModeTooltip(MainWindow mainWindow, string package)
	{
		if (!CheckIfKeymappingWindowVisible() && KeyMappingFilesAvailable(package) && !mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed && !IsSelectedSchemeSmart(mainWindow) && IsShowShootingModeTooltip(mainWindow))
		{
			string[] array = LocaleStrings.GetLocalizedString("STRING_PRESS_TO_AIM_AND_SHOOT", "").Split(new char[2] { '{', '}' });
			mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = true;
			mainWindow.ToggleFullScreenToastVisibility(isFullScreen: true, array[0], IMAPKeys.GetStringForUI(sShootingModeKey), array[2]);
			mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed = true;
			mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = false;
		}
	}

	internal static void AssignEdgeScrollMode(string keyValue, TextBox keyTextBox)
	{
		string text = (Convert.ToBoolean(keyValue, CultureInfo.InvariantCulture) ? "ON" : "OFF");
		BlueStacksUIBinding.Bind(keyTextBox, Constants.ImapLocaleStringsConstant + text);
	}
}
