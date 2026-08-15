using System;
using System.ComponentModel;
using System.Windows.Input;

[Serializable]
[Description("Independent")]
public class Zoom : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private double mX1 = -1.0;

	private double mY1 = -1.0;

	private double mX2 = -1.0;

	private double mY2 = -1.0;

	private double mRadius = 20.0;

	private string mKeyIn = IMAPKeys.GetStringForFile((Key)141);

	private string mKeyIn_1 = string.Empty;

	private string mKeyOut = IMAPKeys.GetStringForFile((Key)143);

	private string mKeyOut_1 = string.Empty;

	private string mKeyModifier;

	private string mKeyModifier_1;

	private double mSpeed = 1.0;

	private double mAmplitude = 20.0;

	private double mAcceleration = 1.0;

	private int mMode;

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
			else
			{
				if (Direction != Direction.Left)
				{
					Direction direction = Direction;
					int num = ((1649259278 > 2013767988) ? 4 : 3);
					if (direction != (Direction)num)
					{
						mX = mX1;
						goto IL_0087;
					}
				}
				mX = mX1 + mRadius;
			}
			goto IL_0087;
			IL_0087:
			return mX;
		}
		set
		{
			mX = value;
			if (Direction == (Direction)(-739566257 ^ -739566260))
			{
				mX2 = Math.Round(mX + mRadius, -1953699064 - -1953699066);
				mX1 = Math.Round(mX - mRadius, -865589779 ^ -865589777);
			}
			else if (Direction == Direction.Up)
			{
				mX1 = Math.Round(mX, -2083333025 ^ -2083333027);
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
				double value2 = mY;
				int digits = ((747579745 > 1472392416) ? 2 : 2);
				mY1 = Math.Round(value2, digits);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, -2013050596 + (0x64F0B6E6 | 0x17BC96E6));
				mY1 = Math.Round(mY - mRadius, -533585918 - (1043267985 << 1981212205));
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
			Direction direction = Direction;
			int num = ((509330518 > 776841174) ? 4 : 3);
			if (direction == (Direction)num)
			{
				mX2 = Math.Round(mX + mRadius, -1269432318 - (1138809387 << 1171682897));
				double value2 = mX - mRadius;
				int digits = ((205507110 > 1183267700) ? 2 : 2);
				mX1 = Math.Round(value2, digits);
				mY1 = Math.Round(mY, 0x59D2767 ^ 0x59D2765);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, -264570848 + 2143032574 % 313076954);
				mY1 = Math.Round(mY - mRadius, 0x1EECDD6A ^ 0x1EECDD68);
				mX1 = Math.Round(mX, 0x17280002 ^ 0x17280000);
				mX2 = X1;
			}
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyIn
	{
		get
		{
			return mKeyIn;
		}
		set
		{
			mKeyIn = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyIn_alt1
	{
		get
		{
			return mKeyIn_1;
		}
		set
		{
			mKeyIn_1 = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyOut
	{
		get
		{
			return mKeyOut;
		}
		set
		{
			mKeyOut = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyOut_alt1
	{
		get
		{
			return mKeyOut_1;
		}
		set
		{
			mKeyOut_1 = value;
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
	public double Acceleration
	{
		get
		{
			return mAcceleration;
		}
		set
		{
			mAcceleration = value;
		}
	}

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Mode
	{
		get
		{
			return mMode;
		}
		set
		{
			mMode = value;
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
			mRadius = Math.Round(Math.Abs(Y2 - Y1) / 2.0, 0xCFB ^ 0xCF9);
		}
		else if (Y1 == Y2)
		{
			Direction = Direction.Right;
			mRadius = Math.Round(Math.Abs(X2 - X1) / 2.0, 2 + (525941592 >> 199815645));
		}
	}
}
