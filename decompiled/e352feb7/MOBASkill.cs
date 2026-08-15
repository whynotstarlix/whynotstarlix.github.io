using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("ParentElement")]
public class MOBASkill : IMAction
{
	internal MOBASkillCancel mMOBASkillCancel;

	private double mCancelX = -1.0;

	private double mCancelY = -1.0;

	internal bool mShowOnOverlay = true;

	internal static MOBADpad MOBADpad
	{
		get
		{
			foreach (MOBADpad item in MOBADpad.sListMOBADpad)
			{
				if (item.OriginX != -1.0 && item.OriginY != -1.0)
				{
					return item;
				}
			}
			return null;
		}
	}

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyActivate { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyActivate_alt1 { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string KeyAutocastToggle { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string KeyAutocastToggle_alt1 { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double YAxisRatio { get; set; }

	public string KeyCancel { get; set; } = IMAPKeys.GetStringForFile((Key)18);

	public string KeyCancel_alt1 { get; set; } = string.Empty;

	public double CancelX
	{
		get
		{
			return mCancelX;
		}
		set
		{
			mCancelX = value;
			CheckSkillCancel();
		}
	}

	public double CancelY
	{
		get
		{
			return mCancelY;
		}
		set
		{
			mCancelY = value;
			CheckSkillCancel();
		}
	}

	public static double OriginX
	{
		get
		{
			if (MOBADpad == null)
			{
				return -1.0;
			}
			return MOBADpad.OriginX;
		}
		set
		{
			if (MOBADpad != null)
			{
				MOBADpad.OriginX = value;
			}
		}
	}

	public static double OriginY
	{
		get
		{
			if (MOBADpad == null)
			{
				return -1.0;
			}
			return MOBADpad.OriginY;
		}
		set
		{
			if (MOBADpad != null)
			{
				MOBADpad.OriginY = value;
			}
		}
	}

	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 5.0;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double DeadZoneRadius { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double CancelSpeed { get; set; } = 500.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsCancelSkillEnabled => mMOBASkillCancel != null;

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

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int NoCancelOnSwitch { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int NoCancelTime { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AutoAttack { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool StopMOBADpad { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AdvancedMode { get; set; } = true;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AutocastEnabled { get; set; } = true;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int MinSkillTime { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double MinSwipeRadius { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int MinSkillHoldTime { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double Speed { get; set; } = 200.0;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string OriginXExpr { get; set; } = "";

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string OriginYExpr { get; set; } = "";

	public string CancelXExpr { get; set; } = "";

	public string CancelYExpr { get; set; } = "";

	public string CancelXOverlayOffset { get; set; } = "";

	public string CancelYOverlayOffset { get; set; } = "";

	public string CancelShowOnOverlayExpr { get; set; }

	private void CheckSkillCancel()
	{
		if (mCancelX == -1.0 && mCancelY == -1.0)
		{
			mMOBASkillCancel = null;
		}
		else if (mMOBASkillCancel == null)
		{
			mMOBASkillCancel = new MOBASkillCancel(this);
		}
	}
}
