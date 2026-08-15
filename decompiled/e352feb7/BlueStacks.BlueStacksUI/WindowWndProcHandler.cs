using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class WindowWndProcHandler
{
	internal enum ResizeDirection
	{
		Left = 1,
		Right,
		Top,
		TopLeft,
		TopRight,
		Bottom,
		BottomLeft,
		BottomRight
	}

	private struct WINDOWPOS
	{
		public IntPtr hwnd;

		public IntPtr hwndInsertAfter;

		public int x;

		public int y;

		public int cx;

		public int cy;

		public int flags;
	}

	private enum SWP
	{
		NOMOVE = 2
	}

	private enum WM
	{
		SYSCOMMAND = 274,
		ENTERMENULOOP = 529,
		WINDOWPOSCHANGING = 70,
		NCCALCSIZE = 131,
		EXITSIZEMOVE = 562,
		GETMINMAXINFO = 36,
		WININICHANGE = 26,
		DEVICECHANGE = 537,
		DISPLAYCHANGE = 126,
		THEMECHANGED = 794,
		SYSCOLORCHANGE = 21,
		INPUT = 255,
		SETFOCUS = 7,
		ACTIVATE = 6
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
	public class MONITORINFOEX
	{
		public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));

		public IntereopRect rcMonitor;

		public IntereopRect rcWork;

		public int dwFlags;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public char[] szDevice = new char[32];
	}

	internal struct MINMAXINFO
	{
		public POINT ptReserved;

		public POINT ptMaxSize;

		public POINT ptMaxPosition;

		public POINT ptMinTrackSize;

		public POINT ptMaxTrackSize;
	}

	public struct POINT(int x, int y)
	{
		public int X = x;

		public int Y = y;

		public POINT(Point pt)
			: this(pt.X, pt.Y)
		{
		}

		public static implicit operator Point(POINT p)
		{
			return new Point(p.X, p.Y);
		}

		public static implicit operator POINT(Point p)
		{
			return new POINT(p.X, p.Y);
		}
	}

	internal struct APPBARDATA
	{
		public int cbSize;

		public IntPtr hWnd;

		public int uCallbackMessage;

		public int uEdge;

		public RECT rc;

		public IntPtr lParam;
	}

	private enum TaskbarLocation
	{
		None,
		Left,
		Top,
		Right,
		Bottom
	}

	private const int ABM_GETTASKBARPOS = 5;

	internal bool IsResizingEnabled = true;

	internal bool IsMinMaxEnabled = true;

	internal bool mAdjustingWidth;

	private MainWindow mWindowInstance;

	private RawInputClass mRawInput;

	private HwndSource _hwndSource;

	internal static bool isLogWndProc;

	private const int MONITOR_DEFAULTTOPRIMARY = 1;

	internal WindowWndProcHandler(MainWindow window)
	{
		mWindowInstance = window;
		MainWindow mainWindow = mWindowInstance;
		mainWindow.ResizeBegin = (EventHandler)Delegate.Combine(mainWindow.ResizeBegin, new EventHandler(mWindowInstance.MainWindow_ResizeBegin));
		MainWindow mainWindow2 = mWindowInstance;
		mainWindow2.ResizeEnd = (EventHandler)Delegate.Combine(mainWindow2.ResizeEnd, new EventHandler(mWindowInstance.MainWindow_ResizeEnd));
		((Window)mWindowInstance).SourceInitialized += Instance_SourceInitialized;
		SetMenuDropDownAlignment();
	}

	private void Instance_SourceInitialized(object sender, EventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		_hwndSource = (HwndSource)PresentationSource.FromVisual((Visual)(object)mWindowInstance);
		_hwndSource.AddHook(new HwndSourceHook(WndProc));
	}

	internal void AddRawInputHandler()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		try
		{
			if (PromotionObject.Instance != null && PromotionObject.Instance.IsSecurityMetricsEnable)
			{
				WindowInteropHelper val = new WindowInteropHelper((Window)(object)mWindowInstance);
				mRawInput = new RawInputClass(val.Handle);
				Logger.Info("Adding raw input handle");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while adding raw input handle: {0}", new object[1] { ex.ToString() });
		}
	}

	internal void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
	{
		if (!IsResizingEnabled)
		{
			return;
		}
		string name = ((FrameworkElement)((sender is Rectangle) ? sender : null)).Name;
		if (name == null)
		{
			return;
		}
		uint num = global::_003CPrivateImplementationDetails_003E.ComputeStringHash(name);
		if (num <= unchecked(-1786320536 + ~1189051965))
		{
			if (num <= (-2127775098 ^ -1570475328))
			{
				if (num != 306900081 - (1455666557 >> 910049342))
				{
					if (num == -168928378 + 1717897260 % 956996460 && name == "topRight")
					{
						((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
					}
				}
				else if (name == "left")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
				}
			}
			else if (num != 873742264 + (940373845 >> 1900426910))
			{
				if (num == 44526378 + (1160437587 << 691466010) && name == "bottom")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
				}
			}
			else if (name == "bottomRight")
			{
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
			}
		}
		else if (num <= -87757688 + (0x2AFD36E3 | 0x77CE867E))
		{
			if (num != (0x5E7623BA ^ 0x26950E5F))
			{
				if (num == unchecked(-906989102 - 1328270923 % 1572089921) && name == "bottomLeft")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
				}
			}
			else if (name == "right")
			{
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
			}
		}
		else if (num != 2387400333u)
		{
			if (num == 2802900028u && name == "top")
			{
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
			}
		}
		else if (name == "topLeft")
		{
			((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
		}
	}

	internal void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!IsResizingEnabled)
		{
			return;
		}
		((RoutedEventArgs)e).Handled = true;
		string name = ((FrameworkElement)((sender is Rectangle) ? sender : null)).Name;
		if (name != null)
		{
			uint num = global::_003CPrivateImplementationDetails_003E.ComputeStringHash(name);
			if (num <= (0x4B04BC55 ^ 0x5A3D77F))
			{
				if (num <= 594524175 - (1306497734 >> 1163624073))
				{
					if (num != (0x179E4D8D ^ 0x5D4A1FD))
					{
						if (num == -126903161 - ~718875582 && name == "topRight")
						{
							((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
							ResizeWindow((ResizeDirection)(-1805719141 - -1805719146));
							return;
						}
					}
					else if (name == "left")
					{
						((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
						mAdjustingWidth = true;
						ResizeWindow(ResizeDirection.Left);
						return;
					}
				}
				else if (num != (-684218850 ^ -484206170))
				{
					int num2 = ((732612591 > 621178709) ? 1319594794 : 1759459725);
					if (num == (uint)num2 && name == "bottom")
					{
						((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
						mAdjustingWidth = false;
						int direction = ((34694955 > 1362351988) ? 8 : 6);
						ResizeWindow((ResizeDirection)direction);
						return;
					}
				}
				else if (name == "bottomRight")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
					int direction2 = ((549690751 > 1775305349) ? 10 : 8);
					ResizeWindow((ResizeDirection)direction2);
					return;
				}
			}
			else if (num <= (uint)(-692433771 + -1542826254))
			{
				int num3 = ((32425960 > 580453976) ? (-1590761508) : 2028154341);
				if (num != (uint)num3)
				{
					int num4 = ((171002457 > 297554953) ? (-1548690935) : 2059707271);
					if (num == (uint)num4 && name == "bottomLeft")
					{
						((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
						ResizeWindow(ResizeDirection.BottomLeft);
						return;
					}
				}
				else if (name == "right")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
					mAdjustingWidth = true;
					ResizeWindow(ResizeDirection.Right);
					return;
				}
			}
			else if (num != 2387400333u)
			{
				if (num == 2802900028u && name == "top")
				{
					((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
					mAdjustingWidth = false;
					ResizeWindow(ResizeDirection.Top);
					return;
				}
			}
			else if (name == "topLeft")
			{
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
				ResizeWindow(ResizeDirection.TopLeft);
				return;
			}
		}
		((RoutedEventArgs)e).Handled = false;
	}

	internal void ResizeWindow(ResizeDirection direction)
	{
		mWindowInstance.ResizeBegin(mWindowInstance, new EventArgs());
		NativeMethods.SendMessage(_hwndSource.Handle, (uint)(-802067757 ^ -802067519), (IntPtr)(int)(466202289 + ~466140848 + direction), IntPtr.Zero);
		mWindowInstance.ResizeEnd(mWindowInstance, new EventArgs());
	}

	internal Point GetMousePosition()
	{
		NativeMethods.Win32Point pt = default(NativeMethods.Win32Point);
		NativeMethods.GetCursorPos(ref pt);
		return new Point(pt.X, pt.Y);
	}

	internal IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		if (isLogWndProc)
		{
			Logger.Info("WndProcMessage: " + msg + "~~" + wParam + "~~" + lParam + "~~");
		}
		switch ((WM)msg)
		{
		case WM.SYSCOMMAND:
			if (wParam == (IntPtr)61696)
			{
				handled = true;
			}
			break;
		case WM.ENTERMENULOOP:
			handled = true;
			break;
		case WM.SYSCOLORCHANGE:
		case WM.WININICHANGE:
		case WM.DISPLAYCHANGE:
		case WM.DEVICECHANGE:
		case WM.THEMECHANGED:
			using (new Timer(delegate
			{
				SetMenuDropDownAlignment();
			}, null, TimeSpan.FromMilliseconds(2.0), TimeSpan.FromMilliseconds(-1.0)))
			{
			}
			break;
		case WM.GETMINMAXINFO:
			WmGetMinMaxInfo(hwnd, lParam);
			handled = true;
			break;
		case WM.WINDOWPOSCHANGING:
		{
			WINDOWPOS wINDOWPOS = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
			if ((wINDOWPOS.flags & 2) != 0)
			{
				return IntPtr.Zero;
			}
			if ((int)(Window)((PresentationSource)HwndSource.FromHwnd(hwnd)).RootVisual == 0)
			{
				return IntPtr.Zero;
			}
			if ((int)((Window)mWindowInstance).WindowState != 0)
			{
				return IntPtr.Zero;
			}
			bool flag = true;
			if (mWindowInstance.MinWidthScaled > wINDOWPOS.cx)
			{
				wINDOWPOS.cx = mWindowInstance.MinWidthScaled;
				wINDOWPOS.cy = (int)mWindowInstance.GetHeightFromWidth(wINDOWPOS.cx, isScaled: true);
				flag = false;
			}
			else if (mWindowInstance.MinHeightScaled > wINDOWPOS.cy)
			{
				wINDOWPOS.cy = mWindowInstance.MinHeightScaled;
				wINDOWPOS.cx = (int)mWindowInstance.GetWidthFromHeight(wINDOWPOS.cy, isScaled: true);
				flag = false;
			}
			if (wINDOWPOS.cx > mWindowInstance.MaxWidthScaled || wINDOWPOS.cy > mWindowInstance.MaxHeightScaled)
			{
				wINDOWPOS.cx = mWindowInstance.MaxWidthScaled;
				wINDOWPOS.cy = mWindowInstance.MaxHeightScaled;
				flag = false;
			}
			if (flag)
			{
				if (mAdjustingWidth)
				{
					wINDOWPOS.cy = (int)mWindowInstance.GetHeightFromWidth(wINDOWPOS.cx, isScaled: true);
				}
				else
				{
					wINDOWPOS.cx = (int)mWindowInstance.GetWidthFromHeight(wINDOWPOS.cy, isScaled: true);
				}
			}
			Marshal.StructureToPtr((object)wINDOWPOS, lParam, true);
			handled = true;
			break;
		}
		case WM.INPUT:
		{
			int num = -1;
			if (mRawInput != null)
			{
				num = RawInputClass.GetDeviceID(lParam);
			}
			if (num == 0 && ((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(mWindowInstance.mVmName))
			{
				((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[mWindowInstance.mVmName].AddSecurityBreach(SecurityBreach.SYNTHETIC_INPUT, string.Empty);
			}
			break;
		}
		case WM.SETFOCUS:
			ThreadPool.QueueUserWorkItem(delegate
			{
				((DispatcherObject)mWindowInstance).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Expected O, but got Unknown
					try
					{
						bool flag2 = true;
						foreach (Window ownedWindow in ((Window)mWindowInstance).OwnedWindows)
						{
							Window val = ownedWindow;
							CustomWindow val2 = (CustomWindow)(object)((val is CustomWindow) ? val : null);
							if (val2 != null)
							{
								if (!val2.IsShowGLWindow && !KMManager.sIsInScriptEditingMode)
								{
									flag2 = false;
									Logger.Debug("OnFocusChanged window IsShowGLWindow false: " + ((FrameworkElement)val2).Name);
								}
							}
							else
							{
								Logger.Debug("OnFocusChanged Non Custom window found! " + ((FrameworkElement)val).Name);
							}
						}
						if (flag2 && !mWindowInstance.mIsFocusComeFromImap)
						{
							mWindowInstance.mFrontendHandler.ShowGLWindow();
						}
						mWindowInstance.mIsFocusComeFromImap = false;
					}
					catch
					{
					}
				}, new object[0]);
			});
			break;
		}
		return IntPtr.Zero;
	}

	private static void SetMenuDropDownAlignment()
	{
		try
		{
			if (SystemParameters.MenuDropAlignment)
			{
				typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
				_ = SystemParameters.MenuDropAlignment;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error setting _menuDropAlignment" + ex.ToString());
		}
	}

	private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
	{
		MINMAXINFO mINMAXINFO = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
		IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
		if (intPtr != IntPtr.Zero)
		{
			MONITORINFOEX mONITORINFOEX = new MONITORINFOEX
			{
				cbSize = Marshal.SizeOf(typeof(MONITORINFO))
			};
			NativeMethods.GetMonitorInfo(intPtr, mONITORINFOEX);
			IntereopRect rcWork = mONITORINFOEX.rcWork;
			IntereopRect rcMonitor = mONITORINFOEX.rcMonitor;
			TaskbarLocation taskbarPosition = GetTaskbarPosition();
			if (!mWindowInstance.mIsFullScreen)
			{
				mINMAXINFO.ptMaxPosition.X = Math.Abs(rcWork.Left - rcMonitor.Left);
				mINMAXINFO.ptMaxPosition.Y = Math.Abs(rcWork.Top - rcMonitor.Top);
				mINMAXINFO.ptMaxSize.X = Math.Abs(rcWork.Width);
				mINMAXINFO.ptMaxSize.Y = Math.Abs(rcWork.Height);
				if (rcWork == rcMonitor)
				{
					switch (taskbarPosition)
					{
					case TaskbarLocation.Left:
						mINMAXINFO.ptMaxPosition.X += 733756604 - (733756602 << 885675584);
						break;
					case TaskbarLocation.Top:
						mINMAXINFO.ptMaxPosition.Y += 0x30D19 ^ 0x30D1B;
						break;
					case TaskbarLocation.Right:
						mINMAXINFO.ptMaxSize.X -= -761030429 ^ -761030431;
						break;
					case TaskbarLocation.Bottom:
						mINMAXINFO.ptMaxSize.Y -= -223485292 + 223485294 % 1894547283;
						break;
					}
				}
			}
			else
			{
				mINMAXINFO.ptMaxPosition.X = 0;
				mINMAXINFO.ptMaxPosition.Y = 0;
				mINMAXINFO.ptMaxSize.X = Math.Abs(rcMonitor.Width);
				mINMAXINFO.ptMaxSize.Y = Math.Abs(rcMonitor.Height);
			}
			mINMAXINFO.ptMaxTrackSize.X = mINMAXINFO.ptMaxSize.X;
			mINMAXINFO.ptMaxTrackSize.Y = mINMAXINFO.ptMaxSize.Y;
		}
		Marshal.StructureToPtr((object)mINMAXINFO, lParam, true);
	}

	internal static IntereopRect GetFullscreenMonitorSize(IntPtr hwnd, bool isWorkAreaRequired = false)
	{
		IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
		if (intPtr != IntPtr.Zero)
		{
			MONITORINFOEX mONITORINFOEX = new MONITORINFOEX();
			NativeMethods.GetMonitorInfo(intPtr, mONITORINFOEX);
			if (isWorkAreaRequired)
			{
				return mONITORINFOEX.rcWork;
			}
			return mONITORINFOEX.rcMonitor;
		}
		return default(IntereopRect);
	}

	private static TaskbarLocation GetTaskbarPosition()
	{
		TaskbarLocation result = TaskbarLocation.None;
		APPBARDATA data = default(APPBARDATA);
		data.cbSize = Marshal.SizeOf((object)data);
		if (NativeMethods.SHAppBarMessage(1679102737 + ~1679102731, ref data) == IntPtr.Zero)
		{
			return result;
		}
		if (((RECT)(ref data.rc)).Left == ((RECT)(ref data.rc)).Top)
		{
			if (((RECT)(ref data.rc)).Right < ((RECT)(ref data.rc)).Bottom)
			{
				result = TaskbarLocation.Left;
			}
			if (((RECT)(ref data.rc)).Right > ((RECT)(ref data.rc)).Bottom)
			{
				result = TaskbarLocation.Top;
			}
		}
		if (((RECT)(ref data.rc)).Left > ((RECT)(ref data.rc)).Top)
		{
			result = TaskbarLocation.Right;
		}
		if (((RECT)(ref data.rc)).Left < ((RECT)(ref data.rc)).Top)
		{
			result = TaskbarLocation.Bottom;
		}
		return result;
	}
}
