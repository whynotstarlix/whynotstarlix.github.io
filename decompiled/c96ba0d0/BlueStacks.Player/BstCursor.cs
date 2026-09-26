using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class BstCursor
{
	private class State
	{
		public int SlotId;

		public Bitmap PrimaryImage;

		public Bitmap SecondaryImage;

		public Pointer Pointer;

		public Point Position;

		public bool Clicked;

		public State(int slotId, Bitmap primaryImage, Bitmap secondaryImage)
		{
			SlotId = slotId;
			PrimaryImage = primaryImage;
			SecondaryImage = secondaryImage;
		}
	}

	private class Pointer : Form
	{
		private struct Win32Point(int x, int y)
		{
			public int X = x;

			public int Y = y;
		}

		private struct Win32Size(int width, int height)
		{
			public int Width = width;

			public int Height = height;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct BLENDFUNCTION
		{
			public byte BlendOp;

			public byte BlendFlags;

			public byte SourceConstantAlpha;

			public byte AlphaFormat;
		}

		private const int WS_EX_TRANSPARENT = 32;

		private const int WS_EX_TOOLWINDOW = 128;

		private const int WS_EX_LAYERED = 524288;

		private const byte AC_SRC_OVER = 0;

		private const byte AC_SRC_ALPHA = 1;

		private const int ULW_ALPHA = 2;

		private Bitmap mBitmap;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = ((Form)this).CreateParams;
				createParams.ExStyle |= 0x20;
				createParams.ExStyle |= 0x80;
				createParams.ExStyle |= 0x80000;
				return createParams;
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Win32Point pptDst, ref Win32Size psize, IntPtr hdcSrc, ref Win32Point pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		public Pointer()
		{
			((Control)this).SuspendLayout();
			((Form)this).ShowInTaskbar = false;
			((Form)this).FormBorderStyle = (FormBorderStyle)0;
			((Form)this).TopMost = true;
			((Control)this).ResumeLayout();
		}

		public void SetBitmap(Bitmap bitmap)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			if ((int)((Image)bitmap).PixelFormat != 2498570)
			{
				throw new ApplicationException("Bad bitmap");
			}
			mBitmap = bitmap;
		}

		public Bitmap GetBitmap()
		{
			return mBitmap;
		}

		public void Update(int x, int y)
		{
			IntPtr dC = GetDC(IntPtr.Zero);
			IntPtr intPtr = CreateCompatibleDC(dC);
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr hObject = IntPtr.Zero;
			try
			{
				intPtr2 = mBitmap.GetHbitmap(Color.FromArgb(0));
				hObject = SelectObject(intPtr, intPtr2);
				Win32Size psize = new Win32Size(((Image)mBitmap).Width, ((Image)mBitmap).Height);
				Win32Point pprSrc = new Win32Point(0, 0);
				Win32Point pptDst = new Win32Point(x, y);
				BLENDFUNCTION pblend = new BLENDFUNCTION
				{
					BlendOp = 0,
					BlendFlags = 0,
					SourceConstantAlpha = byte.MaxValue,
					AlphaFormat = 1
				};
				if (!UpdateLayeredWindow(((Control)this).Handle, dC, ref pptDst, ref psize, intPtr, ref pprSrc, 0, ref pblend, 2))
				{
					CommonError.ThrowLastWin32Error("Cannot update layered window");
				}
			}
			finally
			{
				ReleaseDC(IntPtr.Zero, dC);
				if (intPtr2 != IntPtr.Zero)
				{
					SelectObject(intPtr, hObject);
					DeleteObject(intPtr2);
				}
				DeleteDC(intPtr);
			}
		}
	}

	private const int COUNT_MAX = 4;

	private const int INITIAL_X = 128;

	private const int INITIAL_Y = 128;

	private State[] mCursors;

	private static BstCursor mBstCursor;

	internal static BstCursor Instance
	{
		get
		{
			if (mBstCursor == null)
			{
				mBstCursor = new BstCursor();
			}
			return mBstCursor;
		}
	}

	public BstCursor()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected Obj, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected Obj, but got Unknown
		mCursors = new State[4];
		for (int i = 0; i < 4; i++)
		{
			Bitmap primaryImage = new Bitmap($"{VMWindow.Instance.InstallDir}\\CursorPrimary.png");
			Bitmap secondaryImage = new Bitmap($"{VMWindow.Instance.InstallDir}\\CursorSecondary.png");
			mCursors[i] = new State(i, primaryImage, secondaryImage);
		}
	}

	public void Attach(int identity)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			try
			{
				InternalAttach(identity);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}));
	}

	public void Detach(int identity)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			try
			{
				InternalDetach(identity);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}));
	}

	public void Move(int identity, float x, float y, bool absolute)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			try
			{
				InternalMove(identity, x, y, absolute);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}));
	}

	public void Click(int identity, bool down)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
		{
			try
			{
				InternalClick(identity, down);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}));
	}

	public void RaiseFocusChange()
	{
		bool flag = false;
		if (Utils.IsForegroundApplication())
		{
			flag = true;
		}
		for (int i = 0; i < 4; i++)
		{
			State state = mCursors[i];
			if (state.Pointer != null)
			{
				if (flag)
				{
					((Control)VMWindow.Instance).Focus();
					((Control)state.Pointer).Show();
				}
				else
				{
					((Control)state.Pointer).Hide();
				}
			}
		}
	}

	public void GetNormalizedPosition(int identity, out float x, out float y)
	{
		State state = LookupCursor(identity);
		if (state == null)
		{
			x = 0f;
			y = 0f;
		}
		else
		{
			Rectangle mScaledDisplayArea = LayoutManager.mScaledDisplayArea;
			x = (float)state.Position.X / (float)mScaledDisplayArea.Width;
			y = (float)state.Position.Y / (float)mScaledDisplayArea.Height;
		}
	}

	private void InternalAttach(int identity)
	{
		Logger.Info("Cursor.Attach({0})", new object[1] { identity });
		State state = LookupCursor(identity);
		if (state == null)
		{
			Logger.Warning("Cannot find cursor slot for identity {0}", new object[1] { identity });
			return;
		}
		if (state.Pointer != null)
		{
			Logger.Warning("Cursor slot ID %d already has a pointer", new object[1] { state.SlotId });
			return;
		}
		Logger.Info("Cursor using slot {0}", new object[1] { state.SlotId });
		state.Position.X = 128;
		state.Position.Y = 128;
		state.Clicked = false;
		state.Pointer = new Pointer();
		state.Pointer.SetBitmap(state.PrimaryImage);
		InternalMove(identity, 0f, 0f, absolute: false);
		((Control)VMWindow.Instance).Focus();
	}

	private void InternalDetach(int identity)
	{
		Logger.Info("Cursor.Detach({0})", new object[1] { identity });
		State state = LookupCursor(identity);
		if (state == null)
		{
			Logger.Warning("Cannot find cursor slot for identity {0}", new object[1] { identity });
			return;
		}
		((Form)state.Pointer).Close();
		state.Pointer = null;
		state.Position.X = 0;
		state.Position.Y = 0;
		state.Clicked = false;
	}

	private void InternalMove(int identity, float x, float y, bool absolute)
	{
		if (!Utils.IsForegroundApplication())
		{
			return;
		}
		State state = LookupCursor(identity);
		if (state == null)
		{
			Logger.Warning("Cannot find cursor slot for identity {0}", new object[1] { identity });
			return;
		}
		Rectangle mScaledDisplayArea = LayoutManager.mScaledDisplayArea;
		state.Position.X += (int)x;
		if (state.Position.X < 0)
		{
			state.Position.X = 0;
		}
		else if (state.Position.X > mScaledDisplayArea.Width)
		{
			state.Position.X = mScaledDisplayArea.Width;
		}
		state.Position.Y += (int)y;
		if (state.Position.Y < 0)
		{
			state.Position.Y = 0;
		}
		else if (state.Position.Y > mScaledDisplayArea.Height)
		{
			state.Position.Y = mScaledDisplayArea.Height;
		}
		if (((Control)VMWindow.Instance).Visible)
		{
			Rectangle rectangle = ((Control)VMWindow.Instance).RectangleToScreen(mScaledDisplayArea);
			int x2 = state.Position.X + rectangle.Left - ((Image)state.Pointer.GetBitmap()).Width / 2;
			int y2 = state.Position.Y + rectangle.Top - ((Image)state.Pointer.GetBitmap()).Height / 2;
			state.Pointer.Update(x2, y2);
			((Control)state.Pointer).Show();
		}
		else
		{
			((Control)state.Pointer).Hide();
		}
		InputMapper.TouchPoint[] array = new InputMapper.TouchPoint[1];
		array[0].X = (float)state.Position.X / (float)mScaledDisplayArea.Width;
		array[0].Y = (float)state.Position.Y / (float)mScaledDisplayArea.Height;
		array[0].Down = state.Clicked;
		InputMapper.Instance.TouchHandlerImpl(array, state.SlotId * 4, adjustForControlBar: false);
	}

	private void InternalClick(int identity, bool down)
	{
		if (!Utils.IsForegroundApplication())
		{
			return;
		}
		State state = LookupCursor(identity);
		if (state == null)
		{
			Logger.Warning("Cannot find cursor slot for identity {0}", new object[1] { identity });
			return;
		}
		state.Clicked = down;
		if (!down)
		{
			state.Pointer.SetBitmap(state.PrimaryImage);
		}
		else
		{
			state.Pointer.SetBitmap(state.SecondaryImage);
		}
		InternalMove(identity, 0f, 0f, absolute: false);
	}

	private State LookupCursor(int identity)
	{
		int num = -1;
		if (identity >= 0 && identity < 4)
		{
			num = 3 - identity;
		}
		else if (identity >= 16)
		{
			num = identity - 16;
		}
		if (num >= 0 && num < 4)
		{
			return mCursors[num];
		}
		return null;
	}
}
