using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

internal class GlobalKeyBoardMouseHooks
{
	internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

	internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

	private enum MouseMessages
	{
		WM_LBUTTONDOWN = 513,
		WM_LBUTTONUP = 514,
		WM_MOUSEMOVE = 512,
		WM_MOUSEWHEEL = 522,
		WM_RBUTTONDOWN = 516,
		WM_RBUTTONUP = 517
	}

	private struct MSLLHOOKSTRUCT
	{
		public POINT pt;

		public uint mouseData;

		public uint flags;

		public uint time;

		public IntPtr dwExtraInfo;
	}

	private static bool mIsHidden = false;

	private const int WH_KEYBOARD_LL = 13;

	private const int WM_KEYDOWN = 256;

	private const int WM_KEYUP = 257;

	private const int WM_SYSKEYDOWN = 260;

	private static readonly LowLevelKeyboardProc mKeyboardProc = KeyboardHookCallback;

	private static IntPtr mKeyboardHookID = IntPtr.Zero;

	private static string sKey = null;

	private static bool sIsControlUsedInBossKey = false;

	private static bool sIsAltUsedInBossKey = false;

	private static bool sIsShiftUsedInBossKey = false;

	internal static bool sIsEnableKeyboardHookLogging = false;

	private static LowLevelMouseProc mMouseProc = MouseHookCallback;

	private static IntPtr mMouseHookId = IntPtr.Zero;

	private const int WH_MOUSE_LL = 14;

	private static IntPtr SetHook(LowLevelMouseProc proc)
	{
		using Process process = Process.GetCurrentProcess();
		using ProcessModule processModule = process.MainModule;
		return NativeMethods.SetWindowsHookEx(14, proc, NativeMethods.GetModuleHandle(processModule.ModuleName), 0u);
	}

	private static IntPtr SetHook(LowLevelKeyboardProc proc)
	{
		using Process process = Process.GetCurrentProcess();
		using ProcessModule processModule = process.MainModule;
		return NativeMethods.SetWindowsHookEx(13, proc, NativeMethods.GetModuleHandle(processModule.ModuleName), 0u);
	}

	internal static void SetMouseMoveHook()
	{
		try
		{
			if (mMouseHookId == IntPtr.Zero)
			{
				mMouseHookId = SetHook(mMouseProc);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception setting global mouse hook" + ex.ToString());
		}
	}

	internal static void SetBossKeyHook()
	{
		try
		{
			SetKey(RegistryManager.Instance.BossKey);
			if (mKeyboardHookID == IntPtr.Zero)
			{
				mKeyboardHookID = SetHook(mKeyboardProc);
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception setting global hook" + ex.ToString());
		}
	}

	internal static void SetKey(string key)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(key))
		{
			Logger.Warning("Cannot set an empty key");
			return;
		}
		string[] array = key.Split(new char[2] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		string bossKey = array[^1];
		sKey = ((object)IMAPKeys.mDictKeys.First((KeyValuePair<Key, string> x) => x.Value == bossKey).Key/*cast due to constrained. prefix*/).ToString();
		sIsControlUsedInBossKey = (sIsAltUsedInBossKey = (sIsShiftUsedInBossKey = false));
		string[] array2 = array;
		foreach (string a in array2)
		{
			if (string.Equals(a, "Ctrl", StringComparison.InvariantCulture))
			{
				sIsControlUsedInBossKey = true;
			}
			else if (string.Equals(a, "Alt", StringComparison.InvariantCulture))
			{
				sIsAltUsedInBossKey = true;
			}
			else if (string.Equals(a, "Shift", StringComparison.InvariantCulture))
			{
				sIsShiftUsedInBossKey = true;
			}
		}
	}

	internal static void UnsetKey()
	{
		sKey = string.Empty;
		sIsControlUsedInBossKey = false;
		sIsAltUsedInBossKey = false;
		sIsShiftUsedInBossKey = false;
	}

	internal static void UnHookGlobalHooks()
	{
		NativeMethods.UnhookWindowsHookEx(mKeyboardHookID);
		mKeyboardHookID = IntPtr.Zero;
		UnhookGlobalMouseHooks();
	}

	internal static void UnhookGlobalMouseHooks()
	{
		if (mMouseHookId != IntPtr.Zero)
		{
			NativeMethods.UnhookWindowsHookEx(mMouseHookId);
			mMouseHookId = IntPtr.Zero;
		}
	}

	private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Invalid comparison between I4 and Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (sIsEnableKeyboardHookLogging)
			{
				Logger.Info("Keyboard hook .." + nCode + ".." + wParam + ".." + lParam);
			}
			MainWindow window = BlueStacksUIUtils.ActivatedWindow;
			if (nCode >= 0 && (wParam == (IntPtr)256 || wParam == (IntPtr)260 || wParam == (IntPtr)257))
			{
				int num = Marshal.ReadInt32(lParam);
				Logger.Debug("Keyboard hook .." + num + ".." + sKey);
				if (wParam == (IntPtr)256 || wParam == (IntPtr)260)
				{
					if (!string.IsNullOrEmpty(sKey) && num == (int)(Keys)Enum.Parse(typeof(Keys), sKey, ignoreCase: false) && sIsControlUsedInBossKey == (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119)) && sIsAltUsedInBossKey == (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121)) && sIsShiftUsedInBossKey == (Keyboard.IsKeyDown((Key)116) || Keyboard.IsKeyDown((Key)117)))
					{
						ThreadPool.QueueUserWorkItem(delegate
						{
							if (BlueStacksUIUtils.DictWindows.Values.Count > 0)
							{
								MainWindow mainWindow = BlueStacksUIUtils.DictWindows.Values.ToList()[0];
								((DispatcherObject)mainWindow).Dispatcher.Invoke((Delegate)(Action)delegate
								{
									try
									{
										if (!((IEnumerable)((Window)mainWindow).OwnedWindows).OfType<OnBoardingPopupWindow>().Any() && !((IEnumerable)((Window)mainWindow).OwnedWindows).OfType<GameOnboardingControl>().Any())
										{
											mIsHidden = !mIsHidden;
											BlueStacksUIUtils.HideUnhideBlueStacks(mIsHidden);
										}
									}
									catch
									{
									}
								}, new object[0]);
							}
						});
						return (IntPtr)1;
					}
					if (window != null && (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119)) && (Keyboard.IsKeyDown((Key)120) || Keyboard.IsKeyDown((Key)121)))
					{
						if (num >= 96 && num <= 105)
						{
							num -= 48;
						}
						Key val = KeyInterop.KeyFromVirtualKey(num);
						string vkString = IMAPKeys.GetStringForFile(val);
						if (Enumerable.Contains(MainWindow.sMacroMapping.Keys, vkString))
						{
							ThreadPool.QueueUserWorkItem(delegate
							{
								try
								{
									((DispatcherObject)window).Dispatcher.Invoke((Delegate)(Action)delegate
									{
										//IL_002f: Unknown result type (might be due to invalid IL or missing references)
										//IL_0098: Unknown result type (might be due to invalid IL or missing references)
										//IL_009d: Unknown result type (might be due to invalid IL or missing references)
										//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
										//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
										//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
										//IL_0177: Unknown result type (might be due to invalid IL or missing references)
										//IL_017c: Unknown result type (might be due to invalid IL or missing references)
										if (window.mSidebar.GetElementFromTag("sidebar_macro") != null && (int)((UIElement)window.mSidebar.GetElementFromTag("sidebar_macro")).Visibility == 0 && ((UIElement)window.mSidebar.GetElementFromTag("sidebar_macro")).IsEnabled)
										{
											if (window.mIsMacroRecorderActive)
											{
												window.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""));
											}
											else
											{
												if (!window.mIsMacroPlaying)
												{
													try
													{
														string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, MainWindow.sMacroMapping[vkString] + ".json");
														if (File.Exists(path))
														{
															MacroRecording val2 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), Utils.GetSerializerSettings());
															val2.Name = MainWindow.sMacroMapping[vkString];
															window.mCommonHandler.FullMacroScriptPlayHandler(val2);
															ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "shortcut_keys", ((object)val2.RecordingType/*cast due to constrained. prefix*/).ToString(), string.IsNullOrEmpty(val2.MacroId) ? "local" : "community");
														}
														return;
													}
													catch (Exception ex)
													{
														Logger.Error("Exception in macro play with shortcut: " + ex.ToString());
														return;
													}
												}
												CustomMessageWindow val3 = new CustomMessageWindow();
												BlueStacksUIBinding.Bind(val3.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
												BlueStacksUIBinding.Bind(val3.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
												val3.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
												((Window)val3).Owner = (Window)(object)window;
												((Window)val3).ShowDialog();
											}
										}
										else
										{
											Logger.Info("Macro not enabled for the current package: " + window.StaticComponents.mSelectedTabButton.PackageName);
										}
									}, new object[0]);
								}
								catch
								{
								}
							});
						}
					}
				}
			}
		}
		catch
		{
		}
		return NativeMethods.CallNextHookEx(mKeyboardHookID, nCode, wParam, lParam);
	}

	private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		return NativeMethods.CallNextHookEx(mMouseHookId, nCode, wParam, lParam);
	}
}
