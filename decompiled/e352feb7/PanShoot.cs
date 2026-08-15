using System;
using System.ComponentModel;

[Serializable]
[Description("SubElement")]
internal class PanShoot : IMAction
{
	private Pan mPan;

	public string Key => mPan.KeyAction;

	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return mPan.LButtonX;
		}
		set
		{
			mPan.LButtonX = value;
		}
	}

	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return mPan.LButtonY;
		}
		set
		{
			mPan.LButtonY = value;
		}
	}

	public string LButtonXExpr
	{
		get
		{
			return mPan.LButtonXExpr;
		}
		set
		{
			mPan.LButtonXExpr = value;
		}
	}

	public string LButtonYExpr
	{
		get
		{
			return mPan.LButtonYExpr;
		}
		set
		{
			mPan.LButtonYExpr = value;
		}
	}

	public string LButtonXOverlayOffset
	{
		get
		{
			return mPan.LButtonXOverlayOffset;
		}
		set
		{
			mPan.LButtonXOverlayOffset = value;
		}
	}

	public string LButtonYOverlayOffset
	{
		get
		{
			return mPan.LButtonYOverlayOffset;
		}
		set
		{
			mPan.LButtonYOverlayOffset = value;
		}
	}

	public string LButtonShowOnOverlayExpr
	{
		get
		{
			return mPan.LButtonShowOnOverlayExpr;
		}
		set
		{
			mPan.LButtonShowOnOverlayExpr = value;
		}
	}

	internal PanShoot(Pan action)
	{
		IsChildAction = true;
		base.Type = KeyActionType.PanShoot;
		mPan = action;
		ParentAction = action;
	}
}
