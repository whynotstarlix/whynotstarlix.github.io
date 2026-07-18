using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Newtonsoft.Json;

[Serializable]
[Description("Independent")]
public class Dpad : IMAction
{
	internal static List<Dpad> sListDpad = new List<Dpad>();

	internal MOBADpad mMOBADpad = new MOBADpad();

	private double mX = -1.0;

	private double mY = -1.0;

	internal bool mShowOnOverlay = true;

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
			if (IsMOBADpadEnabled)
			{
				mMOBADpad.X = value;
			}
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
			if (IsMOBADpadEnabled)
			{
				mMOBADpad.Y = value;
			}
		}
	}

	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 6.0;

	[JsonIgnore]
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string DpadTitle => (LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(KeyUp.ToString(CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(KeyLeft.ToString(CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(KeyDown.ToString(CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(KeyRight.ToString(CultureInfo.InvariantCulture)), "")).Trim();

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp { get; set; } = IMAPKeys.GetStringForFile((Key)66);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile((Key)44);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown { get; set; } = IMAPKeys.GetStringForFile((Key)62);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight { get; set; } = IMAPKeys.GetStringForFile((Key)47);

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1 { get; set; } = string.Empty;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string GamepadStick { get; set; } = "";

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier1 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier1_alt1 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius1 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier2 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier2_alt1 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius2 { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed { get; set; } = 200.0;

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ActivationTime { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double ActivationSpeed { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double DeadzoneRadius { get; set; }

	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsMOBADpadEnabled
	{
		get
		{
			if (mMOBADpad.OriginX != -1.0)
			{
				return mMOBADpad.OriginY != -1.0;
			}
			return false;
		}
	}

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

	public Dpad()
	{
		base.Type = KeyActionType.Dpad;
		sListDpad.Add(this);
	}
}
