using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class EdgeScroll : IMAction
{
	internal bool mShowOnOverlay;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XVelocity { get; set; } = 100.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double YVelocity { get; set; } = 100.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XActiveMargin { get; set; } = 3.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double YActiveMargin { get; set; } = 3.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ResetDelay { get; set; } = 150;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double SpeedUpFactor { get; set; } = 2.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int SpeedUpWaitTime { get; set; } = 200;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool EdgeScrollEnabled { get; set; } = true;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool ShowOnOverlay
	{
		get
		{
			return mShowOnOverlay;
		}
		set
		{
			mShowOnOverlay = value;
		}
	}
}
