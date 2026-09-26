using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class Toast : Form
{
	private const int WS_EX_TOOLWINDOW = 128;

	private const int WS_EX_NOACTIVATE = 134217728;

	private const int WS_CHILD = 1073741824;

	private Font font = new Font(Utils.GetSystemFontName(), 12f);

	private SizeF stringSize;

	private string toastText;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = ((Form)this).CreateParams;
			createParams.Style = 1073741824;
			createParams.ExStyle |= 0x8000080;
			return createParams;
		}
	}

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetProcessDPIAware();

	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteObject(IntPtr hObject);

	public Toast(Control parent, string toastText)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected Obj, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected Obj, but got Unknown
		this.toastText = toastText;
		Graphics val = ((Control)this).CreateGraphics();
		stringSize = val.MeasureString(this.toastText, font);
		((Form)this).StartPosition = (FormStartPosition)0;
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).ShowInTaskbar = false;
		((Control)this).Paint += ShowToast;
		((Control)this).Width = (int)stringSize.Width + 20;
		((Control)this).Height = (int)stringSize.Height + 20;
		int x = parent.Left + (parent.Width - ((Control)this).Width) / 2;
		int y = parent.Top + 5;
		((Form)this).Location = new Point(x, y);
		IntPtr intPtr = CreateRoundRectRgn(0, 0, ((Control)this).Width, ((Control)this).Height, 5, 5);
		((Control)this).Region = Region.FromHrgn(intPtr);
		DeleteObject(intPtr);
	}

	private void ShowToast(object sender, PaintEventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected Obj, but got Unknown
		RectangleF rectangleF = new RectangleF(0f, 0f, ((Control)this).Width, ((Control)this).Height);
		Pen val = new Pen(Color.Black);
		e.Graphics.DrawRectangle(val, 0, 0, ((Control)this).Width, ((Control)this).Height);
		SolidBrush val2 = new SolidBrush(Color.White);
		e.Graphics.FillRectangle((Brush)(object)val2, rectangleF);
		float x = ((float)((Control)this).Width - stringSize.Width) / 2f;
		float y = ((float)((Control)this).Height - stringSize.Height) / 2f;
		RectangleF rectangleF2 = new RectangleF(x, y, stringSize.Width, stringSize.Height);
		SolidBrush val3 = new SolidBrush(Color.Black);
		e.Graphics.DrawString(toastText, font, (Brush)(object)val3, rectangleF2);
	}
}
