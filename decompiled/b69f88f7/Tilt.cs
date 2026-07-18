using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("Independent")]
public class Tilt : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private double mRadius = 10.0;

	private string mKeyUp = IMAPKeys.GetStringForFile((Key)24);

	private string mKeyUp_1 = string.Empty;

	private string mKeyDown = IMAPKeys.GetStringForFile((Key)26);

	private string mKeyDown_1 = string.Empty;

	private string mKeyLeft = IMAPKeys.GetStringForFile((Key)23);

	private string mKeyLeft_1 = string.Empty;

	private string mKeyRight = IMAPKeys.GetStringForFile((Key)25);

	private string mKeyRight_1 = string.Empty;

	private double mMaxAngle = 20.0;

	private double mSpeed = 90.0;

	private bool mAutoReset = true;

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

	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Radius
	{
		get
		{
			return mRadius;
		}
		set
		{
			mRadius = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp
	{
		get
		{
			return mKeyUp;
		}
		set
		{
			mKeyUp = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1
	{
		get
		{
			return mKeyUp_1;
		}
		set
		{
			mKeyUp_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown
	{
		get
		{
			return mKeyDown;
		}
		set
		{
			mKeyDown = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1
	{
		get
		{
			return mKeyDown_1;
		}
		set
		{
			mKeyDown_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft
	{
		get
		{
			return mKeyLeft;
		}
		set
		{
			mKeyLeft = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1
	{
		get
		{
			return mKeyLeft_1;
		}
		set
		{
			mKeyLeft_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight
	{
		get
		{
			return mKeyRight;
		}
		set
		{
			mKeyRight = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1
	{
		get
		{
			return mKeyRight_1;
		}
		set
		{
			mKeyRight_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double MaxAngle
	{
		get
		{
			return mMaxAngle;
		}
		set
		{
			mMaxAngle = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed
	{
		get
		{
			return mSpeed;
		}
		set
		{
			mSpeed = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool AutoReset
	{
		get
		{
			return mAutoReset;
		}
		set
		{
			mAutoReset = value;
		}
	}
}
