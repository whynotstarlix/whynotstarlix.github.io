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
				double value2 = mX + mRadius;
				int digits = ((927742770 > 1274072757) ? 2 : 2);
				mX2 = Math.Round(value2, digits);
				mX1 = Math.Round(mX - mRadius, -1693017904 + (1693017906 >> 723168480));
			}
			else if (Direction == Direction.Up)
			{
				mX1 = Math.Round(mX, 0x28189 ^ 0x2818B);
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
				mY1 = Math.Round(mY, 0x65FB5FCD ^ 0x65FB5FCF);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, 0x333AD7AF ^ 0x333AD7AD);
				mY1 = Math.Round(mY - mRadius, 1090856341 + ~1090856338);
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
			if (Direction == (Direction)(-251587789 ^ -251587792))
			{
				mX2 = Math.Round(mX + mRadius, 221734605 - (1773876830 >> 342230403));
				double value2 = mX - mRadius;
				int digits = ((106756997 > 1293640880) ? 2 : 2);
				mX1 = Math.Round(value2, digits);
				mY1 = Math.Round(mY, 0x61F1E1E ^ 0x61F1E1C);
				mY2 = Y1;
			}
			else if (Direction == Direction.Up)
			{
				mY2 = Math.Round(mY + mRadius, 1156389905 + -1156389903);
				mY1 = Math.Round(mY - mRadius, 0x7FAAFDD8 ^ 0x7FAAFDDA);
				mX1 = Math.Round(mX, 1106405122 - (142861518 << 1999454471));
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
			mRadius = Math.Round(Math.Abs(Y2 - Y1) / 2.0, -92821540 + 1320719806 % 1227898264);
		}
		else if (Y1 == Y2)
		{
			int direction = ((1193191272 > 1035463724) ? 3 : 4);
			Direction = (Direction)direction;
			mRadius = Math.Round(Math.Abs(X2 - X1) / 2.0, -575203547 ^ -575203545);
		}
	}
}
