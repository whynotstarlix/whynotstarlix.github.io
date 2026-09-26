using System;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class MouseHWheel
{
	public delegate void MouseHWheelCallback(int x, int y, int keyState, int delta);

	private static MouseHWheelCallback s_MouseHWheelCallback;

	private int keyEnableSynaptic;

	public MouseHWheel()
	{
	}

	public MouseHWheel(MouseHWheelCallback cb)
	{
		setMousehWheelCallback(cb);
	}

	public bool setMousehWheelCallback(MouseHWheelCallback cb)
	{
		if (cb == null)
		{
			return false;
		}
		keyEnableSynaptic = RegistryManager.Instance.DefaultGuest.HScroll;
		if (keyEnableSynaptic != 1)
		{
			Logger.Info("Horizontal Mouse Wheel support is Disabled");
			return false;
		}
		s_MouseHWheelCallback = cb.Invoke;
		try
		{
			if (!HDPlusModule.SetMouseHWheelCallback(s_MouseHWheelCallback))
			{
				Logger.Info("Horizontal scrolling disabled, no synaptic device found");
			}
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("Continue with MouseHWheel error:");
			Logger.Error(ex.ToString());
		}
		return false;
	}
}
