using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("Independent")]
public class FreeLook : IMAction
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
	public string Key { get; set; } = IMAPKeys.GetStringForFile((Key)65);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile((Key)23);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight { get; set; } = IMAPKeys.GetStringForFile((Key)25);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp { get; set; } = IMAPKeys.GetStringForFile((Key)24);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown { get; set; } = IMAPKeys.GetStringForFile((Key)26);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int DeviceType { get; set; }

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

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Sensitivity { get; set; } = 1.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed { get; set; } = 20.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool MouseAcceleration { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Delay { get; set; } = 50;
}
