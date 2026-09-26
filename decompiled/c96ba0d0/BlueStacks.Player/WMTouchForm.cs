using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class WMTouchForm : Form
{
	public class TouchPoint
	{
		private int x;

		private int y;

		private int id;

		private int slot;

		public int X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public int Slot => slot;

		public TouchPoint(int slot)
		{
			Clear();
			this.slot = slot;
		}

		public void Clear()
		{
			x = -1;
			y = -1;
			id = -1;
		}
	}

	public class WMTouchEventArgs : EventArgs
	{
		private WMTouchForm form;

		public int GetPointCount()
		{
			return form.touchPointArray.Length;
		}

		public TouchPoint GetPoint(int ndx)
		{
			return form.touchPointArray[ndx];
		}

		public WMTouchEventArgs(WMTouchForm form)
		{
			this.form = form;
		}
	}

	private struct TOUCHINPUT
	{
		public int x;

		public int y;

		public IntPtr hSource;

		public int dwID;

		public int dwFlags;

		public int dwMask;

		public int dwTime;

		public IntPtr dwExtraInfo;

		public int cxContact;

		public int cyContact;
	}

	private struct POINTS
	{
		public short x;

		public short y;
	}

	private const int WM_TOUCHMOVE = 576;

	private const int WM_TOUCHDOWN = 577;

	private const int WM_TOUCHUP = 578;

	private const int TOUCHEVENTF_MOVE = 1;

	private const int TOUCHEVENTF_DOWN = 2;

	private const int TOUCHEVENTF_UP = 4;

	private const int TOUCHEVENTF_INRANGE = 8;

	private const int TOUCHEVENTF_PRIMARY = 16;

	private const int TOUCHEVENTF_NOCOALESCE = 32;

	private const int TOUCHEVENTF_PEN = 64;

	private const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 1;

	private const int TOUCHINPUTMASKF_EXTRAINFO = 2;

	private const int TOUCHINPUTMASKF_CONTACTAREA = 4;

	private const int TWF_FINETOUCH = 1;

	private const int TWF_WANTPALM = 2;

	private TOUCHINPUT[] touchInputArray;

	private TouchPoint[] touchPointArray;

	private WMTouchEventArgs touchEventArgs;

	private int touchInputSize;

	protected event EventHandler<WMTouchEventArgs> TouchEvent;

	[SecurityPermission(SecurityAction.Demand)]
	public WMTouchForm()
	{
		try
		{
			((Form)this).Load += OnLoadHandler;
		}
		catch (Exception ex)
		{
			Logger.Info("Touch: ERROR: Could not add form load handler");
			Logger.Info("Touch: " + ex.ToString());
		}
		touchInputArray = new TOUCHINPUT[16];
		for (int i = 0; i < 16; i++)
		{
			touchInputArray[i] = default;
		}
		touchPointArray = new TouchPoint[16];
		for (int j = 0; j < 16; j++)
		{
			touchPointArray[j] = new TouchPoint(j);
		}
		touchEventArgs = new WMTouchEventArgs(this);
		touchInputSize = Marshal.SizeOf((object)default(TOUCHINPUT));
	}

	[DllImport("user32")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool RegisterTouchWindow(IntPtr hWnd, ulong ulFlags);

	[DllImport("user32")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In][Out] TOUCHINPUT[] pInputs, int cbSize);

	[DllImport("user32")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern void CloseTouchInputHandle(IntPtr lParam);

	private void OnLoadHandler(object sender, EventArgs e)
	{
		ulong ulFlags = 2uL;
		try
		{
			if (!RegisterTouchWindow(((Control)this).Handle, ulFlags))
			{
				Logger.Info("Touch: ERROR: Could not register window for touch");
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Touch: ERROR: RegisterTouchWindow API not available");
			Logger.Info("Touch: " + ex.ToString());
		}
	}

	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	protected override void WndProc(ref Message m)
	{
		int msg = m.Msg;
		bool flag = (uint)(msg - 576) <= 2u && DecodeTouch(ref m);
		((Form)this).WndProc(ref m);
		if (flag)
		{
			try
			{
				m.Result = new IntPtr(1);
			}
			catch (Exception ex)
			{
				Logger.Info("Touch: ERROR: Could not allocate result ptr");
				Logger.Info("Touch: " + ex.ToString());
			}
		}
	}

	private static int LoWord(int number)
	{
		return number & 0xFFFF;
	}

	private bool DecodeTouch(ref Message m)
	{
		if (TouchEvent == null)
		{
			return false;
		}
		int num = LoWord(m.WParam.ToInt32());
		if (num > touchInputArray.Length)
		{
			num = touchInputArray.Length;
		}
		if (!GetTouchInputInfo(m.LParam, num, touchInputArray, touchInputSize))
		{
			return false;
		}
		for (int i = 0; i < touchPointArray.Length; i++)
		{
			touchPointArray[i].Clear();
		}
		for (int j = 0; j < num; j++)
		{
			TOUCHINPUT tOUCHINPUT = touchInputArray[j];
			TouchPoint touchPoint = touchPointArray[j];
			if ((tOUCHINPUT.dwFlags & 2) != 0 || (tOUCHINPUT.dwFlags & 1) != 0)
			{
				Point point = ((Control)this).PointToClient(new Point(tOUCHINPUT.x / 100, tOUCHINPUT.y / 100));
				touchPoint.Id = tOUCHINPUT.dwID;
				touchPoint.X = point.X;
				touchPoint.Y = point.Y;
			}
		}
		TouchEvent(this, touchEventArgs);
		CloseTouchInputHandle(m.LParam);
		return true;
	}
}
