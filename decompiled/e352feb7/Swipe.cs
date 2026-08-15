using System;
using System.ComponentModel;
using BlueStacks.Common;

[Serializable]
[Description("Dependent")]
public class Swipe : IMAction
{
	private double mX1 = -1.0;

	private double mY1 = -1.0;

	private double mX2;

	private double mY2;

	private double mRadius = 10.0;

	private double mSpeed = 200.0;

	private bool mHold;

	private string mKey;

	private string mKey_1;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X1
	{
		get
		{
			return mX1;
		}
		set
		{
			mX1 = value;
			if (Direction == Direction.Up || Direction == Direction.Down)
			{
				mX2 = X1;
			}
			else if (Direction == Direction.Left)
			{
				mX2 = Math.Round(X1 - mRadius, -2063466413 + (0xA5DD5AF | 0x70E06B8B));
			}
			else if (Direction == Direction.Right)
			{
				mX2 = Math.Round(X1 + mRadius, -2131750775 + (0x60074141 | 0x7F0EAF38));
			}
		}
	}

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y1
	{
		get
		{
			return mY1;
		}
		set
		{
			mY1 = value;
			if (Direction == Direction.Left || Direction == (Direction)(1151558552 + -1151558549))
			{
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(Y1 - mRadius, 160074 - (1311313265 >> 844322221));
			}
			else if (Direction == Direction.Down)
			{
				mY2 = Math.Round(Y1 + mRadius, 3 ^ 1);
			}
		}
	}

	public double X2
	{
		get
		{
			return mX2;
		}
		set
		{
			mX2 = value;
			CheckDirection();
		}
	}

	public double Y2
	{
		get
		{
			return mY2;
		}
		set
		{
			mY2 = value;
			CheckDirection();
		}
	}

	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double Radius
	{
		get
		{
			return mRadius;
		}
		set
		{
			mRadius = value;
			if (Direction == Direction.Left)
			{
				Y2 = Y1;
				X2 = Math.Round(X1 - value, 2535136 - 281974334 % 11177568);
				Logger.Debug("SWIPE_L: X2: " + X2 + "...............Y2: " + Y2);
			}
			else if (Direction == Direction.Right)
			{
				Y2 = Y1;
				X2 = Math.Round(X1 + value, 29005 - (1900749673 >> 1248937808));
				Logger.Debug("SWIPE_R: X2: " + X2 + "...............Y2: " + Y2);
			}
			else if (Direction == Direction.Up)
			{
				X2 = X1;
				Y2 = Math.Round(Y1 - value, 2005272753 + -2005272751);
				Logger.Debug("SWIPE_U: X2: " + X2 + "...............Y2: " + Y2);
			}
			else if (Direction == Direction.Down)
			{
				X2 = X1;
				Y2 = Math.Round(Y1 + value, -1743715692 + (0x27AAF46E | 0x46CC5D4A));
				Logger.Debug("SWIPE_D: X2: " + X2 + "...............Y2: " + Y2);
			}
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
	public bool Hold
	{
		get
		{
			return mHold;
		}
		set
		{
			mHold = value;
		}
	}

	[Description("IMAP_PopupUIElementNotCommon")]
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

	[Description("IMAP_PopupUIElementNotCommon")]
	[Category("Fields")]
	public string Key_alt1
	{
		get
		{
			return mKey_1;
		}
		set
		{
			mKey_1 = value;
		}
	}

	private void CheckDirection()
	{
		if (X1 == X2)
		{
			if (Y1 > Y2)
			{
				Direction = Direction.Up;
				mRadius = Math.Round(Y1 - Y2, 325068729 - (1300274911 >> 1323881634));
				return;
			}
			Direction = Direction.Down;
			double value = Y2 - Y1;
			int digits = ((564141049 > 1552260547) ? 2 : 2);
			mRadius = Math.Round(value, digits);
		}
		else if (Y1 == Y2)
		{
			if (X1 > X2)
			{
				Direction = Direction.Left;
				mRadius = Math.Round(X1 - X2, 29605506 - (1894752266 >> 1903547814));
			}
			else
			{
				Direction = Direction.Right;
				mRadius = Math.Round(X2 - X1, 0x55680002 ^ 0x55680000);
			}
		}
	}
}
