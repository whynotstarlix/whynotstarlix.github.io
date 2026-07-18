using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("Independent")]
public class Rotate : IMAction
{
	internal bool mShowOnOverlay = true;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyClock { get; set; } = IMAPKeys.GetStringForFile((Key)67);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyAntiClock { get; set; } = IMAPKeys.GetStringForFile((Key)69);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyClock_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyAntiClock_alt1 { get; set; } = string.Empty;

	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 6.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Speed { get; set; } = 60;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ActivationTime { get; set; } = 100;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Tweaks { get; set; } = 1;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double StartingAngle { get; set; } = 90.0;

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
