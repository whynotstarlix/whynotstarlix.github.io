using System;
using System.Collections.Generic;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class MOBADpad : IMAction
{
	internal Dpad mDpad;

	internal static List<MOBADpad> sListMOBADpad = new List<MOBADpad>();

	public double X { get; set; } = -1.0;

	public double Y { get; set; } = -1.0;

	[Description("IMAP_CanvasElementY")]
	public double OriginX { get; set; } = -1.0;

	[Description("IMAP_CanvasElementX")]
	public double OriginY { get; set; }

	internal string KeyMove { get; } = "MouseRButton";

	public double XRadius
	{
		get
		{
			if (mDpad == null)
			{
				return -1.0;
			}
			return mDpad.XRadius;
		}
		set
		{
			if (mDpad != null)
			{
				mDpad.XRadius = value;
			}
		}
	}

	public double DpadSpeed
	{
		get
		{
			if (mDpad == null)
			{
				return -1.0;
			}
			return mDpad.Speed;
		}
		set
		{
			if (mDpad != null)
			{
				mDpad.Speed = value;
			}
		}
	}

	public double CharSpeed { get; set; } = 10.0;

	public string OriginXExpr { get; set; } = "";

	public string OriginYExpr { get; set; } = "";

	public MOBADpad()
	{
		IsChildAction = true;
		base.Type = KeyActionType.MOBADpad;
		sListMOBADpad.Add(this);
	}

	public MOBADpad(Dpad action)
	{
		IsChildAction = true;
		base.Type = KeyActionType.MOBADpad;
		sListMOBADpad.Add(this);
		mDpad = action;
		ParentAction = action;
	}
}
