using System;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

[Serializable]
public class MacroAction
{
	private double mDelayFromLastAction;

	private ActionType mActionType;

	private double mActionPointX;

	private double mActionPointY;

	private MouseButtons mMouseButton = (MouseButtons)1048576;

	private Keys mActionKey;

	public double DelayFromLastAction
	{
		get
		{
			return mDelayFromLastAction;
		}
		set
		{
			mDelayFromLastAction = value;
		}
	}

	public ActionType ActionType
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mActionType;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mActionType = value;
		}
	}

	public double ActionPointX
	{
		get
		{
			return mActionPointX;
		}
		set
		{
			mActionPointX = value;
		}
	}

	public double ActionPointY
	{
		get
		{
			return mActionPointY;
		}
		set
		{
			mActionPointY = value;
		}
	}

	public MouseButtons MouseButton
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mMouseButton;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mMouseButton = value;
		}
	}

	public Keys ActionKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mActionKey;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mActionKey = value;
		}
	}

	public MacroAction()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}
}
