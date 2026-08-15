using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class Tap : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private string mKey;

	private string mKey_1 = string.Empty;

	internal bool mShowOnOverlay = true;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X
	{
		get
		{
			return mX;
		}
		set
		{
			mX = value;
		}
	}

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y
	{
		get
		{
			return mY;
		}
		set
		{
			mY = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key
	{
		get
		{
			return mKey;
		}
		set
		{
			mKey = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key_alt1
	{
		get
		{
			return mKey_1;
		}
		set
		{
			mKey_1 = value;
		}
	}

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
