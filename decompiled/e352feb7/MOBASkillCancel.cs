using System;
using System.ComponentModel;

[Serializable]
[Description("SubElementDependent")]
internal class MOBASkillCancel : IMAction
{
	internal MOBASkill mMOBASkill;

	public string Key
	{
		get
		{
			return mMOBASkill.KeyCancel;
		}
		set
		{
			mMOBASkill.KeyCancel = value;
		}
	}

	public string Key_alt1
	{
		get
		{
			return mMOBASkill.KeyCancel_alt1;
		}
		set
		{
			mMOBASkill.KeyCancel_alt1 = value;
		}
	}

	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return mMOBASkill.CancelX;
		}
		set
		{
			mMOBASkill.CancelX = value;
		}
	}

	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return mMOBASkill.CancelY;
		}
		set
		{
			mMOBASkill.CancelY = value;
		}
	}

	public string MOBASkillCancelXExpr
	{
		get
		{
			return mMOBASkill.CancelXExpr;
		}
		set
		{
			mMOBASkill.CancelXExpr = value;
		}
	}

	public string MOBASkillCancelYExpr
	{
		get
		{
			return mMOBASkill.CancelYExpr;
		}
		set
		{
			mMOBASkill.CancelYExpr = value;
		}
	}

	public string MOBASkillCancelOffsetX
	{
		get
		{
			return mMOBASkill.CancelXOverlayOffset;
		}
		set
		{
			mMOBASkill.CancelXOverlayOffset = value;
		}
	}

	public string MOBASkillCancelOffsetY
	{
		get
		{
			return mMOBASkill.CancelYOverlayOffset;
		}
		set
		{
			mMOBASkill.CancelYOverlayOffset = value;
		}
	}

	public string MOBASkillShowOnOverlayExpr
	{
		get
		{
			return mMOBASkill.CancelShowOnOverlayExpr;
		}
		set
		{
			mMOBASkill.CancelShowOnOverlayExpr = value;
		}
	}

	internal MOBASkillCancel(MOBASkill action)
	{
		IsChildAction = true;
		base.Type = KeyActionType.MOBASkillCancel;
		mMOBASkill = action;
		ParentAction = action;
	}
}
