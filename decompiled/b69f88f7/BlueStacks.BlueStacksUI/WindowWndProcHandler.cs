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

	public struct POINT
	{
		public int X;

		public int Y;

		public POINT(int x, int y)
		{
			X = x;
			Y = y;
		}

		public POINT(Point pt)
		{
			X = pt.X;
			Y = pt.Y;
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

		public IntPtr lParam;

		public RECT rc;
	}

	private enum TaskbarLocation
	{
		None,
		Left,
		Top,
		Right,
		Bottom
	}

	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
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
		mWindowInstance = window ?? throw new ArgumentNullException("window");
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
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		_hwndSource = (HwndSource)PresentationSource.FromVisual((Visual)(object)mWindowInstance);
		if (_hwndSource != null)
		{
			_hwndSource.AddHook(new HwndSourceHook(WndProc));
		}
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
		Rectangle val = (Rectangle)((sender is Rectangle) ? sender : null);
		if (val == null || string.IsNullOrEmpty(((FrameworkElement)val).Name))
		{
			return;
		}
		string name = ((FrameworkElement)val).Name;
		if (name == null)
		{
			return;
		}
		switch (name.Length)
		{
		default:
			return;
		case 3:
			if (!(name == "top"))
			{
				return;
			}
			goto IL_00d2;
		case 6:
			if (!(name == "bottom"))
			{
				return;
			}
			goto IL_00d2;
		case 4:
			if (!(name == "left"))
			{
				return;
			}
			goto IL_00e3;
		case 5:
			if (!(name == "right"))
			{
				return;
			}
			goto IL_00e3;
		case 7:
			if (!(name == "topLeft"))
			{
				return;
			}
			goto IL_00f4;
		case 11:
			if (!(name == "bottomRight"))
			{
				return;
			}
			goto IL_00f4;
		case 8:
			if (!(name == "topRight"))
			{
				return;
			}
			break;
		case 10:
			if (!(name == "bottomLeft"))
			{
				return;
			}
			break;
		case 9:
			return;
			IL_00f4:
			((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
			return;
			IL_00e3:
			((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
			return;
			IL_00d2:
			((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
			return;
		}
		((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
	}

	internal void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!IsResizingEnabled)
		{
			return;
		}
		Rectangle val = (Rectangle)((sender is Rectangle) ? sender : null);
		if (val != null && !string.IsNullOrEmpty(((FrameworkElement)val).Name))
		{
			((RoutedEventArgs)e).Handled = true;
			switch (((FrameworkElement)val).Name)
			{
			case "top":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
				mAdjustingWidth = false;
				ResizeWindow(ResizeDirection.Top);
				break;
			case "bottom":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNS;
				mAdjustingWidth = false;
				ResizeWindow(ResizeDirection.Bottom);
				break;
			case "left":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
				mAdjustingWidth = true;
				ResizeWindow(ResizeDirection.Left);
				break;
			case "right":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeWE;
				mAdjustingWidth = true;
				ResizeWindow(ResizeDirection.Right);
				break;
			case "topLeft":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
				ResizeWindow(ResizeDirection.TopLeft);
				break;
			case "topRight":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
				ResizeWindow(ResizeDirection.TopRight);
				break;
			case "bottomLeft":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNESW;
				ResizeWindow(ResizeDirection.BottomLeft);
				break;
			case "bottomRight":
				((FrameworkElement)mWindowInstance).Cursor = Cursors.SizeNWSE;
				ResizeWindow(ResizeDirection.BottomRight);
				break;
			default:
				((RoutedEventArgs)e).Handled = false;
				break;
			}
		}
	}

	internal void ResizeWindow(ResizeDirection direction)
	{
		mWindowInstance.ResizeBegin(mWindowInstance, EventArgs.Empty);
		NativeMethods.SendMessage(_hwndSource.Handle, 274u, (IntPtr)(int)(61440 + direction), IntPtr.Zero);
		mWindowInstance.ResizeEnd(mWindowInstance, EventArgs.Empty);
	}

	internal Point GetMousePosition()
	{
		NativeMethods.Win32Point pt = default(NativeMethods.Win32Point);
		NativeMethods.GetCursorPos(ref pt);
		return new Point(pt.X, pt.Y);
	}

	internal IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (isLogWndProc)
		{
			Logger.Info($"WndProcMessage: {msg}~~{wParam}~~{lParam}~~");
		}
		switch (msg)
		{
		case 7:
			((DispatcherObject)mWindowInstance).Dispatcher.BeginInvoke((DispatcherPriority)5, (Delegate)(Action)delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Expected O, but got Unknown
				try
				{
					bool flag2 = true;
					if (((Window)mWindowInstance).OwnedWindows != null)
					{
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
					}
					if (flag2 && !mWindowInstance.mIsFocusComeFromImap)
					{
						mWindowInstance.mFrontendHandler.ShowGLWindow();
					}
					mWindowInstance.mIsFocusComeFromImap = false;
				}
				catch (Exception ex)
				{
					Logger.Error("WndProc SETFOCUS Error: " + ex.Message);
				}
			});
			break;
		case 21:
		case 26:
		case 537:
		case 794:
			new Timer(delegate
			{
				SetMenuDropDownAlignment();
			}, null, TimeSpan.FromMilliseconds(2.0), TimeSpan.FromMilliseconds(-1.0));
			break;
		case 36:
			WmGetMinMaxInfo(hwnd, lParam);
			handled = true;
			break;
		case 70:
		{
			WINDOWPOS structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
			if ((structure.flags & 2) != 0)
			{
				return IntPtr.Zero;
			}
			HwndSource obj = HwndSource.FromHwnd(hwnd);
			if (!(((obj != null) ? ((PresentationSource)obj).RootVisual : null) is Window) || (int)((Window)mWindowInstance).WindowState != 0)
			{
				return IntPtr.Zero;
			}
			bool flag = true;
			if (mWindowInstance.MinWidthScaled > structure.cx)
			{
				structure.cx = mWindowInstance.MinWidthScaled;
				structure.cy = (int)mWindowInstance.GetHeightFromWidth(structure.cx, isScaled: true);
				flag = false;
			}
			else if (mWindowInstance.MinHeightScaled > structure.cy)
			{
				structure.cy = mWindowInstance.MinHeightScaled;
				structure.cx = (int)mWindowInstance.GetWidthFromHeight(structure.cy, isScaled: true);
				flag = false;
			}
			if (structure.cx > mWindowInstance.MaxWidthScaled || structure.cy > mWindowInstance.MaxHeightScaled)
			{
				structure.cx = mWindowInstance.MaxWidthScaled;
				structure.cy = mWindowInstance.MaxHeightScaled;
				flag = false;
			}
			if (flag)
			{
				if (mAdjustingWidth)
				{
					structure.cy = (int)mWindowInstance.GetHeightFromWidth(structure.cx, isScaled: true);
				}
				else
				{
					structure.cx = (int)mWindowInstance.GetWidthFromHeight(structure.cy, isScaled: true);
				}
			}
			Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
			handled = true;
			break;
		}
		case 274:
			if (wParam.ToInt64() == 61696)
			{
				handled = true;
			}
			break;
		case 255:
		case 529:
		{
			if (msg == 529)
			{
				handled = true;
				break;
			}
			int num = -1;
			if (mRawInput != null)
			{
				num = RawInputClass.GetDeviceID(lParam);
			}
			if (num == 0 && mWindowInstance.mVmName != null && ((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(mWindowInstance.mVmName))
			{
				((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[mWindowInstance.mVmName].AddSecurityBreach(SecurityBreach.SYNTHETIC_INPUT, string.Empty);
			}
			break;
		}
		}
		return IntPtr.Zero;
	}

	private static void SetMenuDropDownAlignment()
	{
		try
		{
			if (SystemParameters.MenuDropAlignment)
			{
				FieldInfo field = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.Static | BindingFlags.NonPublic);
				if (field != null)
				{
					field.SetValue(null, false);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("error setting _menuDropAlignment" + ex.ToString());
		}
	}

	private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
	{
		MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
		IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
		if (intPtr != IntPtr.Zero)
		{
			MONITORINFOEX mONITORINFOEX = new MONITORINFOEX
			{
				cbSize = Marshal.SizeOf(typeof(MONITORINFOEX))
			};
			NativeMethods.GetMonitorInfo(intPtr, mONITORINFOEX);
			IntereopRect rcWork = mONITORINFOEX.rcWork;
			IntereopRect rcMonitor = mONITORINFOEX.rcMonitor;
			TaskbarLocation taskbarPosition = GetTaskbarPosition();
			if (!mWindowInstance.mIsFullScreen)
			{
				structure.ptMaxPosition.X = Math.Abs(rcWork.Left - rcMonitor.Left);
				structure.ptMaxPosition.Y = Math.Abs(rcWork.Top - rcMonitor.Top);
				structure.ptMaxSize.X = Math.Abs(rcWork.Width);
				structure.ptMaxSize.Y = Math.Abs(rcWork.Height);
				if (rcWork.Left == rcMonitor.Left && rcWork.Top == rcMonitor.Top && rcWork.Right == rcMonitor.Right && rcWork.Bottom == rcMonitor.Bottom)
				{
					switch (taskbarPosition)
					{
					case TaskbarLocation.Left:
						structure.ptMaxPosition.X += 2;
						break;
					case TaskbarLocation.Top:
						structure.ptMaxPosition.Y += 2;
						break;
					case TaskbarLocation.Right:
						structure.ptMaxSize.X -= 2;
						break;
					case TaskbarLocation.Bottom:
						structure.ptMaxSize.Y -= 2;
						break;
					}
				}
			}
			else
			{
				structure.ptMaxPosition.X = 0;
				structure.ptMaxPosition.Y = 0;
				structure.ptMaxSize.X = Math.Abs(rcMonitor.Width);
				structure.ptMaxSize.Y = Math.Abs(rcMonitor.Height);
			}
			structure.ptMaxTrackSize.X = structure.ptMaxSize.X;
			structure.ptMaxTrackSize.Y = structure.ptMaxSize.Y;
		}
		Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
	}

	internal static IntereopRect GetFullscreenMonitorSize(IntPtr hwnd, bool isWorkAreaRequired = false)
	{
		IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
		if (intPtr == IntPtr.Zero)
		{
			return default(IntereopRect);
		}
		MONITORINFOEX mONITORINFOEX = new MONITORINFOEX();
		mONITORINFOEX.cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
		NativeMethods.GetMonitorInfo(intPtr, mONITORINFOEX);
		if (isWorkAreaRequired)
		{
			return mONITORINFOEX.rcWork;
		}
		return mONITORINFOEX.rcMonitor;
	}

	private static TaskbarLocation GetTaskbarPosition()
	{
		TaskbarLocation result = TaskbarLocation.None;
		APPBARDATA data = default(APPBARDATA);
		data.cbSize = Marshal.SizeOf(data);
		if (NativeMethods.SHAppBarMessage(5, ref data) == IntPtr.Zero)
		{
			return result;
		}
		if (data.rc.Left == data.rc.Top)
		{
			if (data.rc.Right < data.rc.Bottom)
			{
				result = TaskbarLocation.Left;
			}
			else if (data.rc.Right > data.rc.Bottom)
			{
				result = TaskbarLocation.Top;
			}
		}
		else if (data.rc.Left > data.rc.Top)
		{
			result = TaskbarLocation.Right;
		}
		else if (data.rc.Left < data.rc.Top)
		{
			result = TaskbarLocation.Bottom;
		}
		return result;
	}
}
