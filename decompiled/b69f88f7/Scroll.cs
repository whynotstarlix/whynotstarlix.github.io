using System;
using System.ComponentModel;

[Serializable]
[Description("Independent")]
public class Scroll : IMAction
{
	private double mX = -1.0;

	private double mY = -1.0;

	private double mSpeed = 100.0;

	private double mAmplitude = 20.0;

	private bool mOverride;

	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X
	{
		get
		{
			return mX;
		}
		set
		{
			mX = value;
		}
	}

	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y
	{
		get
		{
			return mY;
		}
		set
		{
			mY = value;
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
}
