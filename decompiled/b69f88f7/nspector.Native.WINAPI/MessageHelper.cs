using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace nspector.Native.WINAPI;

internal class MessageHelper
{
	internal struct COPYDATASTRUCT
	{
		internal IntPtr dwData;

		internal int cbData;

		[MarshalAs(UnmanagedType.LPStr)]
		internal string lpData;
	}

	internal struct WINDOWPLACEMENT
	{
		internal int length;

		internal int flags;

		internal int showCmd;

		internal Point ptMinPosition;

		internal Point ptMaxPosition;

		internal Rectangle rcNormalPosition;
	}

	internal const int WM_NULL = 0;

	internal const int WM_CREATE = 1;

	internal const int WM_DESTROY = 2;

	internal const int WM_MOVE = 3;

	internal const int WM_SIZE = 5;

	internal const int WM_ACTIVATE = 6;

	internal const int WM_SETFOCUS = 7;

	internal const int WM_KILLFOCUS = 8;

	internal const int WM_ENABLE = 10;

	internal const int WM_SETREDRAW = 11;

	internal const int WM_SETTEXT = 12;

	internal const int WM_GETTEXT = 13;

	internal const int WM_GETTEXTLENGTH = 14;

	internal const int WM_PAINT = 15;

	internal const int WM_CLOSE = 16;

	internal const int WM_QUERYENDSESSION = 17;

	internal const int WM_QUIT = 18;

	internal const int WM_QUERYOPEN = 19;

	internal const int WM_ERASEBKGND = 20;

	internal const int WM_SYSCOLORCHANGE = 21;

	internal const int WM_ENDSESSION = 22;

	internal const int WM_SYSTEMERROR = 23;

	internal const int WM_SHOWWINDOW = 24;

	internal const int WM_CTLCOLOR = 25;

	internal const int WM_WININICHANGE = 26;

	internal const int WM_SETTINGCHANGE = 26;

	internal const int WM_DEVMODECHANGE = 27;

	internal const int WM_ACTIVATEAPP = 28;

	internal const int WM_FONTCHANGE = 29;

	internal const int WM_TIMECHANGE = 30;

	internal const int WM_CANCELMODE = 31;

	internal const int WM_SETCURSOR = 32;

	internal const int WM_MOUSEACTIVATE = 33;

	internal const int WM_CHILDACTIVATE = 34;

	internal const int WM_QUEUESYNC = 35;

	internal const int WM_GETMINMAXINFO = 36;

	internal const int WM_PAINTICON = 38;

	internal const int WM_ICONERASEBKGND = 39;

	internal const int WM_NEXTDLGCTL = 40;

	internal const int WM_SPOOLERSTATUS = 42;

	internal const int WM_DRAWITEM = 43;

	internal const int WM_MEASUREITEM = 44;

	internal const int WM_DELETEITEM = 45;

	internal const int WM_VKEYTOITEM = 46;

	internal const int WM_CHARTOITEM = 47;

	internal const int WM_SETFONT = 48;

	internal const int WM_GETFONT = 49;

	internal const int WM_SETHOTKEY = 50;

	internal const int WM_GETHOTKEY = 51;

	internal const int WM_QUERYDRAGICON = 55;

	internal const int WM_COMPAREITEM = 57;

	internal const int WM_COMPACTING = 65;

	internal const int WM_WINDOWPOSCHANGING = 70;

	internal const int WM_WINDOWPOSCHANGED = 71;

	internal const int WM_POWER = 72;

	internal const int WM_COPYDATA = 74;

	internal const int WM_CANCELJOURNAL = 75;

	internal const int WM_NOTIFY = 78;

	internal const int WM_INPUTLANGCHANGEREQUEST = 80;

	internal const int WM_INPUTLANGCHANGE = 81;

	internal const int WM_TCARD = 82;

	internal const int WM_HELP = 83;

	internal const int WM_USERCHANGED = 84;

	internal const int WM_NOTIFYFORMAT = 85;

	internal const int WM_CONTEXTMENU = 123;

	internal const int WM_STYLECHANGING = 124;

	internal const int WM_STYLECHANGED = 125;

	internal const int WM_DISPLAYCHANGE = 126;

	internal const int WM_GETICON = 127;

	internal const int WM_SETICON = 128;

	internal const int WM_NCCREATE = 129;

	internal const int WM_NCDESTROY = 130;

	internal const int WM_NCCALCSIZE = 131;

	internal const int WM_NCHITTEST = 132;

	internal const int WM_NCPAINT = 133;

	internal const int WM_NCACTIVATE = 134;

	internal const int WM_GETDLGCODE = 135;

	internal const int WM_NCMOUSEMOVE = 160;

	internal const int WM_NCLBUTTONDOWN = 161;

	internal const int WM_NCLBUTTONUP = 162;

	internal const int WM_NCLBUTTONDBLCLK = 163;

	internal const int WM_NCRBUTTONDOWN = 164;

	internal const int WM_NCRBUTTONUP = 165;

	internal const int WM_NCRBUTTONDBLCLK = 166;

	internal const int WM_NCMBUTTONDOWN = 167;

	internal const int WM_NCMBUTTONUP = 168;

	internal const int WM_NCMBUTTONDBLCLK = 169;

	internal const int WM_KEYFIRST = 256;

	internal const int WM_KEYDOWN = 256;

	internal const int WM_KEYUP = 257;

	internal const int WM_CHAR = 258;

	internal const int WM_DEADCHAR = 259;

	internal const int WM_SYSKEYDOWN = 260;

	internal const int WM_SYSKEYUP = 261;

	internal const int WM_SYSCHAR = 262;

	internal const int WM_SYSDEADCHAR = 263;

	internal const int WM_KEYLAST = 264;

	internal const int WM_IME_STARTCOMPOSITION = 269;

	internal const int WM_IME_ENDCOMPOSITION = 270;

	internal const int WM_IME_COMPOSITION = 271;

	internal const int WM_IME_KEYLAST = 271;

	internal const int WM_INITDIALOG = 272;

	internal const int WM_COMMAND = 273;

	internal const int WM_SYSCOMMAND = 274;

	internal const int WM_TIMER = 275;

	internal const int WM_HSCROLL = 276;

	internal const int WM_VSCROLL = 277;

	internal const int WM_INITMENU = 278;

	internal const int WM_INITMENUPOPUP = 279;

	internal const int WM_MENUSELECT = 287;

	internal const int WM_MENUCHAR = 288;

	internal const int WM_ENTERIDLE = 289;

	internal const int WM_CTLCOLORMSGBOX = 306;

	internal const int WM_CTLCOLOREDIT = 307;

	internal const int WM_CTLCOLORLISTBOX = 308;

	internal const int WM_CTLCOLORBTN = 309;

	internal const int WM_CTLCOLORDLG = 310;

	internal const int WM_CTLCOLORSCROLLBAR = 311;

	internal const int WM_CTLCOLORSTATIC = 312;

	internal const int WM_MOUSEFIRST = 512;

	internal const int WM_MOUSEMOVE = 512;

	internal const int WM_LBUTTONDOWN = 513;

	internal const int WM_LBUTTONUP = 514;

	internal const int WM_LBUTTONDBLCLK = 515;

	internal const int WM_RBUTTONDOWN = 516;

	internal const int WM_RBUTTONUP = 517;

	internal const int WM_RBUTTONDBLCLK = 518;

	internal const int WM_MBUTTONDOWN = 519;

	internal const int WM_MBUTTONUP = 520;

	internal const int WM_MBUTTONDBLCLK = 521;

	internal const int WM_MOUSELAST = 522;

	internal const int WM_MOUSEWHEEL = 522;

	internal const int WM_PARENTNOTIFY = 528;

	internal const int WM_ENTERMENULOOP = 529;

	internal const int WM_EXITMENULOOP = 530;

	internal const int WM_NEXTMENU = 531;

	internal const int WM_SIZING = 532;

	internal const int WM_CAPTURECHANGED = 533;

	internal const int WM_MOVING = 534;

	internal const int WM_POWERBROADCAST = 536;

	internal const int WM_DEVICECHANGE = 537;

	internal const int WM_MDICREATE = 544;

	internal const int WM_MDIDESTROY = 545;

	internal const int WM_MDIACTIVATE = 546;

	internal const int WM_MDIRESTORE = 547;

	internal const int WM_MDINEXT = 548;

	internal const int WM_MDIMAXIMIZE = 549;

	internal const int WM_MDITILE = 550;

	internal const int WM_MDICASCADE = 551;

	internal const int WM_MDIICONARRANGE = 552;

	internal const int WM_MDIGETACTIVE = 553;

	internal const int WM_MDISETMENU = 560;

	internal const int WM_ENTERSIZEMOVE = 561;

	internal const int WM_EXITSIZEMOVE = 562;

	internal const int WM_DROPFILES = 563;

	internal const int WM_MDIREFRESHMENU = 564;

	internal const int WM_IME_SETCONTEXT = 641;

	internal const int WM_IME_NOTIFY = 642;

	internal const int WM_IME_CONTROL = 643;

	internal const int WM_IME_COMPOSITIONFULL = 644;

	internal const int WM_IME_SELECT = 645;

	internal const int WM_IME_CHAR = 646;

	internal const int WM_IME_KEYDOWN = 656;

	internal const int WM_IME_KEYUP = 657;

	internal const int WM_MOUSEHOVER = 673;

	internal const int WM_NCMOUSELEAVE = 674;

	internal const int WM_MOUSELEAVE = 675;

	internal const int WM_CUT = 768;

	internal const int WM_COPY = 769;

	internal const int WM_PASTE = 770;

	internal const int WM_CLEAR = 771;

	internal const int WM_UNDO = 772;

	internal const int WM_RENDERFORMAT = 773;

	internal const int WM_RENDERALLFORMATS = 774;

	internal const int WM_DESTROYCLIPBOARD = 775;

	internal const int WM_DRAWCLIPBOARD = 776;

	internal const int WM_PAINTCLIPBOARD = 777;

	internal const int WM_VSCROLLCLIPBOARD = 778;

	internal const int WM_SIZECLIPBOARD = 779;

	internal const int WM_ASKCBFORMATNAME = 780;

	internal const int WM_CHANGECBCHAIN = 781;

	internal const int WM_HSCROLLCLIPBOARD = 782;

	internal const int WM_QUERYNEWPALETTE = 783;

	internal const int WM_PALETTEISCHANGING = 784;

	internal const int WM_PALETTECHANGED = 785;

	internal const int WM_HOTKEY = 786;

	internal const int WM_PRINT = 791;

	internal const int WM_PRINTCLIENT = 792;

	internal const int WM_HANDHELDFIRST = 856;

	internal const int WM_HANDHELDLAST = 863;

	internal const int WM_PENWINFIRST = 896;

	internal const int WM_PENWINLAST = 911;

	internal const int WM_COALESCE_FIRST = 912;

	internal const int WM_COALESCE_LAST = 927;

	internal const int WM_DDE_FIRST = 992;

	internal const int WM_DDE_INITIATE = 992;

	internal const int WM_DDE_TERMINATE = 993;

	internal const int WM_DDE_ADVISE = 994;

	internal const int WM_DDE_UNADVISE = 995;

	internal const int WM_DDE_ACK = 996;

	internal const int WM_DDE_DATA = 997;

	internal const int WM_DDE_REQUEST = 998;

	internal const int WM_DDE_POKE = 999;

	internal const int WM_DDE_EXECUTE = 1000;

	internal const int WM_DDE_LAST = 1000;

	internal const int WM_USER = 1024;

	internal const int WM_APP = 32768;

	internal const int SW_HIDE = 0;

	internal const int SW_SHOWNORMAL = 1;

	internal const int SW_NORMAL = 1;

	internal const int SW_SHOWMINIMIZED = 2;

	internal const int SW_SHOWMAXIMIZED = 3;

	internal const int SW_MAXIMIZE = 3;

	internal const int SW_SHOWNOACTIVATE = 4;

	internal const int SW_SHOW = 5;

	internal const int SW_MINIMIZE = 6;

	internal const int SW_SHOWMINNOACTIVE = 7;

	internal const int SW_SHOWNA = 8;

	internal const int SW_RESTORE = 9;

	[DllImport("User32.dll")]
	private static extern int RegisterWindowMessage(string lpString);

	[DllImport("User32.dll")]
	internal static extern int FindWindow(string lpClassName, string lpWindowName);

	[DllImport("User32.dll")]
	internal static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

	[DllImport("User32.dll")]
	internal static extern int PostMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

	[DllImport("User32.dll")]
	internal static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

	[DllImport("User32.dll")]
	internal static extern int PostMessage(int hWnd, int Msg, int wParam, int lParam);

	[DllImport("User32.dll")]
	internal static extern bool SetForegroundWindow(int hWnd);

	[DllImport("User32.dll")]
	private static extern bool SetWindowPlacement(int hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

	[DllImport("User32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

	internal bool bringAppToFront(int hWnd)
	{
		WINDOWPLACEMENT lpwndpl = default(WINDOWPLACEMENT);
		if (GetWindowPlacement(hWnd, ref lpwndpl) && lpwndpl.showCmd != 1)
		{
			lpwndpl.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
			lpwndpl.showCmd = 1;
			SetWindowPlacement(hWnd, ref lpwndpl);
		}
		return SetForegroundWindow(hWnd);
	}

	internal int sendWindowsStringMessage(int hWnd, int wParam, string msg)
	{
		int result = 0;
		if (hWnd > 0)
		{
			byte[] bytes = Encoding.Default.GetBytes(msg);
			int num = bytes.Length;
			COPYDATASTRUCT lParam = default(COPYDATASTRUCT);
			lParam.dwData = (IntPtr)100;
			lParam.lpData = msg;
			lParam.cbData = num + 1;
			result = SendMessage(hWnd, 74, wParam, ref lParam);
		}
		return result;
	}

	internal int sendWindowsMessage(int hWnd, int Msg, int wParam, int lParam)
	{
		int result = 0;
		if (hWnd > 0)
		{
			result = SendMessage(hWnd, Msg, wParam, lParam);
		}
		return result;
	}

	internal int getWindowId(string className, string windowName)
	{
		return FindWindow(className, windowName);
	}

	internal int checkIfProcessRunning(string process)
	{
		Process[] processesByName = Process.GetProcessesByName(process);
		return processesByName.Length;
	}

	internal void closeProcesses(string process, bool force)
	{
		Process[] processesByName = Process.GetProcessesByName(process);
		for (int i = 0; i < processesByName.Length; i++)
		{
			processesByName[i].CloseMainWindow();
			if (force)
			{
				processesByName[i].Kill();
			}
			else
			{
				processesByName[i].WaitForExit();
			}
		}
	}

	internal void startProcess(string processName)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.FileName = processName;
		process.Start();
	}
}
