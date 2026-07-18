using System;
using System.ComponentModel;

[Serializable]
[Description("SubElement")]
internal class LookAround : IMAction
{
	private Pan mPan;

	public string Key
	{
		get
		{
			return mPan.KeyLookAround;
		}
		set
		{
			mPan.KeyLookAround = value;
		}
	}

	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return mPan.LookAroundX;
		}
		set
		{
			mPan.LookAroundX = value;
		}
	}

	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return mPan.LookAroundY;
		}
		set
		{
			mPan.LookAroundY = value;
		}
	}

	public string LookAroundXExpr
	{
		get
		{
			return mPan.LookAroundXExpr;
		}
		set
		{
			mPan.LookAroundXExpr = value;
		}
	}

	public string LookAroundYExpr
	{
		get
		{
			return mPan.LookAroundYExpr;
		}
		set
		{
			mPan.LookAroundYExpr = value;
		}
	}

	public string LookAroundXOverlayOffset
	{
		get
		{
			return mPan.LookAroundXOverlayOffset;
		}
		set
		{
			mPan.LookAroundXOverlayOffset = value;
		}
	}

	public string LookAroundYOverlayOffset
	{
		get
		{
			return mPan.LookAroundYOverlayOffset;
		}
		set
		{
			mPan.LookAroundYOverlayOffset = value;
		}
	}

	public string LookAroundShowOnOverlayExpr
	{
		get
		{
			return mPan.LookAroundShowOnOverlayExpr;
		}
		set
		{
			mPan.LookAroundShowOnOverlayExpr = value;
		}
	}

	internal LookAround(Pan action)
	{
		IsChildAction = true;
		base.Type = KeyActionType.LookAround;
		mPan = action;
		ParentAction = action;
	}
}
