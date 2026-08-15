using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("ParentElement")]
public class Pan : IMAction
{
	internal LookAround mLookAround;

	internal PanShoot mPanShoot;

	private double mX = -1.0;

	private double mY = -1.0;

	private string mKeyStartStop = IMAPKeys.GetStringForFile((Key)90);

	private string mKeyStartStop_1 = string.Empty;

	private string mKeySuspend = IMAPKeys.GetStringForFile((Key)120);

	private string mKeySuspend_1 = string.Empty;

	private double mLookAroundX = -1.0;

	private double mLookAroundY = -1.0;

	private string mKeyLookAround = IMAPKeys.GetStringForFile((Key)65);

	private double mLButtonX = -1.0;

	private double mLButtonY = -1.0;

	private string mKeyAction = "MouseLButton";

	private double mSensitivity = 1.0;

	private int mTweaks;

	private double mSensitivityRatioY = 1.0;

	internal bool mShowOnOverlay = true;

	private bool mMouseAcceleration;

	private string mGamepadStick = "";

	private double mGamepadSensitivity = 1.0;

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
	public string KeyStartStop
	{
		get
		{
			return mKeyStartStop;
		}
		set
		{
			mKeyStartStop = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyStartStop_alt1
	{
		get
		{
			return mKeyStartStop_1;
		}
		set
		{
			mKeyStartStop_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySuspend
	{
		get
		{
			return mKeySuspend;
		}
		set
		{
			mKeySuspend = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySuspend_alt1
	{
		get
		{
			return mKeySuspend_1;
		}
		set
		{
			mKeySuspend_1 = value;
		}
	}

	public double LookAroundX
	{
		get
		{
			return mLookAroundX;
		}
		set
		{
			mLookAroundX = value;
			CheckLookAround();
		}
	}

	public double LookAroundY
	{
		get
		{
			return mLookAroundY;
		}
		set
		{
			mLookAroundY = value;
			CheckLookAround();
		}
	}

	public string KeyLookAround
	{
		get
		{
			return mKeyLookAround;
		}
		set
		{
			mKeyLookAround = value;
		}
	}

	public double LButtonX
	{
		get
		{
			return mLButtonX;
		}
		set
		{
			mLButtonX = value;
			CheckShootOnClick();
		}
	}

	public double LButtonY
	{
		get
		{
			return mLButtonY;
		}
		set
		{
			mLButtonY = value;
			CheckShootOnClick();
		}
	}

	internal string KeyAction => mKeyAction;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Sensitivity
	{
		get
		{
			return mSensitivity;
		}
		set
		{
			mSensitivity = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Tweaks
	{
		get
		{
			return mTweaks;
		}
		set
		{
			mTweaks = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double SensitivityRatioY
	{
		get
		{
			return mSensitivityRatioY;
		}
		set
		{
			mSensitivityRatioY = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsLookAroundEnabled => mLookAround != null;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsShootOnClickEnabled => mPanShoot != null;

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
	public bool MouseAcceleration
	{
		get
		{
			return mMouseAcceleration;
		}
		set
		{
			mMouseAcceleration = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string GamepadStick
	{
		get
		{
			return mGamepadStick;
		}
		set
		{
			mGamepadStick = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double GamepadSensitivity
	{
		get
		{
			return mGamepadSensitivity;
		}
		set
		{
			mGamepadSensitivity = value;
		}
	}

	public string LButtonXExpr { get; set; }

	public string LButtonYExpr { get; set; }

	public string LButtonXOverlayOffset { get; set; }

	public string LButtonYOverlayOffset { get; set; }

	public string LookAroundXExpr { get; set; }

	public string LookAroundYExpr { get; set; }

	public string LookAroundXOverlayOffset { get; set; }

	public string LookAroundYOverlayOffset { get; set; }

	public string LButtonShowOnOverlayExpr { get; set; }

	public string LookAroundShowOnOverlayExpr { get; set; }

	private void CheckLookAround()
	{
		if (mLookAroundX == -1.0 && mLookAroundY == -1.0)
		{
			mLookAround = null;
		}
		else if (mLookAround == null)
		{
			mLookAround = new LookAround(this);
		}
	}

	private void CheckShootOnClick()
	{
		if (mLButtonX == -1.0 && mLButtonY == -1.0)
		{
			mPanShoot = null;
		}
		else if (mPanShoot == null)
		{
			mPanShoot = new PanShoot(this);
		}
	}
}
