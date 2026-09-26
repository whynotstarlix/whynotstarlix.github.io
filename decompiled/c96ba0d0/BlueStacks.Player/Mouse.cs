using System.Windows.Forms;

namespace BlueStacks.Player;

public class Mouse
{
	private static Mouse mInstance;

	private uint x;

	private uint y;

	private bool b0;

	private bool b1;

	private bool b2;

	public static Mouse Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new Mouse();
			}
			return mInstance;
		}
	}

	public uint X => x;

	public uint Y => y;

	public uint Mask
	{
		get
		{
			uint num = 0u;
			if (b0)
			{
				num |= 1;
			}
			if (b1)
			{
				num |= 2;
			}
			if (b2)
			{
				num |= 4;
			}
			return num;
		}
	}

	public Mouse()
	{
		x = 0u;
		y = 0u;
		b0 = false;
		b1 = false;
		b2 = false;
	}

	public void UpdateCursor(uint x, uint y)
	{
		this.x = x;
		this.y = y;
	}

	public void UpdateButton(uint x, uint y, MouseButtons button, bool pressed)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		this.x = x;
		this.y = y;
		if ((int)button != 1048576)
		{
			if ((int)button != 2097152)
			{
				if ((int)button == 4194304)
				{
					b2 = pressed;
				}
			}
			else
			{
				b1 = pressed;
			}
		}
		else
		{
			b0 = pressed;
		}
	}
}
