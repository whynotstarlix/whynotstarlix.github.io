using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class TapRepeat : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private string mKey;

	private string mKey_alt1;

	private int mCount = 5;

	private int mDelay = 100;

	internal bool mShowOnOverlay = true;

	private bool mRepeatUntilKeyUp = true;

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
			return mKey_alt1;
		}
		set
		{
			mKey_alt1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Count
	{
		get
		{
			return mCount;
		}
		set
		{
			mCount = value;
		}
	}

	public int Delay
	{
		get
		{
			return mDelay;
		}
		set
		{
			mDelay = 1000 / (2 * Count);
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

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool RepeatUntilKeyUp
	{
		get
		{
			return mRepeatUntilKeyUp;
		}
		set
		{
			mRepeatUntilKeyUp = value;
		}
	}
}
