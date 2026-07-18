using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("Independent")]
public class MouseZoom : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private double mX1 = -1.0;

	private double mY1 = -1.0;

	private double mX2 = -1.0;

	private double mY2 = -1.0;

	private double mRadius = 20.0;

	private string mKey;

	private string mKey_1 = string.Empty;

	private string mKeyModifier = IMAPKeys.GetStringForFile((Key)118);

	private string mKeyModifier_1;

	private double mSpeed = 40.0;

	private double mAmplitude = 25.0;

	private bool mOverride = true;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double X
	{
		get
		{
			if (mX1 == -1.0 && mX2 == -1.0)
			{
				mX = -1.0;
			}
			else if (Direction == Direction.Left || Direction == Direction.Right)
			{
				mX = mX1 + mRadius;
			}
			else
			{
				mX = mX1;
			}
			return mX;
		}
		set
		{
			mX = value;
			if (Direction == Direction.Right)
			{
				mX2 = Math.Round(mX + mRadius, 2);
				mX1 = Math.Round(mX - mRadius, 2);
			}
			else if (Direction == Direction.Up)
			{
				mX1 = Math.Round(mX, 2);
				mX2 = X1;
			}
		}
	}

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double Y
	{
		get
		{
			if (mY1 == -1.0 && mY2 == -1.0)
			{
				mY = -1.0;
			}
			else if (Direction == Direction.Up || Direction == Direction.Down)
			{
				mY = mY1 + mRadius;
			}
			else
			{
				mY = mY1;
			}
			return mY;
		}
		set
		{
			mY = value;
			if (Direction == Direction.Right)
			{
				mY1 = Math.Round(mY, 2);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, 2);
				mY1 = Math.Round(mY - mRadius, 2);
			}
		}
	}

	public double X1
	{
		get
		{
			return mX1;
		}
		set
		{
			mX1 = value;
			CheckDirection();
			if (Direction == Direction.Up || Direction == Direction.Down)
			{
				mX2 = X1;
			}
		}
	}

	public double Y1
	{
		get
		{
			return mY1;
		}
		set
		{
			mY1 = value;
			CheckDirection();
			if (Direction == Direction.Left || Direction == Direction.Right)
			{
				mY2 = Y1;
			}
		}
	}

	[Category("Fields")]
	internal double Size
	{
		get
		{
			return Radius * 2.0;
		}
		set
		{
			Radius = value / 2.0;
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

	[Description("IMAP_CanvasElementRadius")]
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
			if (Direction == Direction.Right)
			{
				mX2 = Math.Round(mX + mRadius, 2);
				mX1 = Math.Round(mX - mRadius, 2);
				mY1 = Math.Round(mY, 2);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, 2);
				mY1 = Math.Round(mY - mRadius, 2);
				mX1 = Math.Round(mX, 2);
				mX2 = X1;
			}
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
			return mKey_1;
		}
		set
		{
			mKey_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyModifier
	{
		get
		{
			return mKeyModifier;
		}
		set
		{
			mKeyModifier = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyModifier_alt1
	{
		get
		{
			return mKeyModifier_1;
		}
		set
		{
			mKeyModifier_1 = value;
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
	public double Amplitude
	{
		get
		{
			return mAmplitude;
		}
		set
		{
			mAmplitude = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool Override
	{
		get
		{
			return mOverride;
		}
		set
		{
			mOverride = value;
		}
	}

	private void CheckDirection()
	{
		if (X1 == X2)
		{
			Direction = Direction.Up;
			mRadius = Math.Round(Math.Abs(Y2 - Y1) / 2.0, 2);
		}
		else if (Y1 == Y2)
		{
			Direction = Direction.Right;
			mRadius = Math.Round(Math.Abs(X2 - X1) / 2.0, 2);
		}
	}
}
