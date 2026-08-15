using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class Callback : IMAction
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
	public string Id { get; set; } = "";

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Action { get; set; } = "";

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
