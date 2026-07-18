using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class State : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private string mName = string.Empty;

	private string mKey;

	private string mKey_alt1;

	private string mModel = string.Empty;

	private int mDelay;

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
	public string Name
	{
		get
		{
			return mName;
		}
		set
		{
			mName = value;
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
			return mKey_alt1;
		}
		set
		{
			mKey_alt1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Model
	{
		get
		{
			return mModel;
		}
		set
		{
			mModel = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Delay
	{
		get
		{
			return mDelay;
		}
		set
		{
			mDelay = value;
		}
	}
}
