using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Player;

public class Keyboard
{
	private static Keyboard mInstance;

	private Dictionary<Keys, bool> escapeSet;

	public static Keyboard Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new Keyboard();
			}
			return mInstance;
		}
	}

	[DllImport("user32.dll")]
	private static extern uint MapVirtualKey(uint code, uint mapType);

	public Keyboard()
	{
		escapeSet = new Dictionary<Keys, bool>
		{
			{
				(Keys)91,
				true
			},
			{
				(Keys)92,
				true
			},
			{
				(Keys)93,
				true
			},
			{
				(Keys)36,
				true
			},
			{
				(Keys)35,
				true
			},
			{
				(Keys)33,
				true
			},
			{
				(Keys)34,
				true
			},
			{
				(Keys)37,
				true
			},
			{
				(Keys)39,
				true
			},
			{
				(Keys)38,
				true
			},
			{
				(Keys)40,
				true
			}
		};
	}

	public uint NativeToScanCodes(Keys key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected I4, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		uint num = MapVirtualKey((uint)(key & 0xFFFF), 0u);
		if (!NeedEscape(key))
		{
			return num;
		}
		return 0xE000 | num;
	}

	private bool NeedEscape(Keys key)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return escapeSet.ContainsKey(key);
	}

	public bool IsAltDepressed()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		return (Control.ModifierKeys & 0x40000) == 262144;
	}
}
