using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace nspector;

public class WatermarkTextBox : TextBox
{
	private const int WM_PAINT = 15;

	private string _watermarkText;

	[Category("Appearance")]
	public string WatermarkText
	{
		get
		{
			return _watermarkText;
		}
		set
		{
			_watermarkText = value;
			((Control)this).Invalidate();
		}
	}

	protected override void WndProc(ref Message m)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		((TextBox)this).WndProc(ref m);
		if (((Message)(ref m)).Msg != 15 || !string.IsNullOrEmpty(((Control)this).Text) || string.IsNullOrEmpty(_watermarkText))
		{
			return;
		}
		Graphics val = ((Control)this).CreateGraphics();
		try
		{
			Brush val2 = (Brush)new SolidBrush(SystemColors.GrayText);
			try
			{
				TextFormatFlags val3 = (TextFormatFlags)4;
				TextRenderer.DrawText((IDeviceContext)(object)val, _watermarkText, ((Control)this).Font, ((Control)this).ClientRectangle, SystemColors.GrayText, val3);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
