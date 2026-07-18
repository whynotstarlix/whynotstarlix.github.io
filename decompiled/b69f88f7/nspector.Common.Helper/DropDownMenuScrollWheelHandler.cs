using System;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace nspector.Common.Helper;

public class DropDownMenuScrollWheelHandler : IMessageFilter
{
	private static DropDownMenuScrollWheelHandler Instance;

	private IntPtr activeHwnd;

	private ToolStripDropDown activeMenu;

	private static readonly Action<ToolStrip, int> ScrollInternal = (Action<ToolStrip, int>)Delegate.CreateDelegate(typeof(Action<ToolStrip, int>), typeof(ToolStrip).GetMethod("ScrollInternal", BindingFlags.Instance | BindingFlags.NonPublic));

	public static void Enable(bool enabled)
	{
		if (enabled)
		{
			if (Instance == null)
			{
				Instance = new DropDownMenuScrollWheelHandler();
				Application.AddMessageFilter((IMessageFilter)(object)Instance);
			}
		}
		else if (Instance != null)
		{
			Application.RemoveMessageFilter((IMessageFilter)(object)Instance);
			Instance = null;
		}
	}

	public bool PreFilterMessage(ref Message m)
	{
		if (((Message)(ref m)).Msg == 512 && activeHwnd != ((Message)(ref m)).HWnd)
		{
			activeHwnd = ((Message)(ref m)).HWnd;
			Control obj = Control.FromHandle(((Message)(ref m)).HWnd);
			activeMenu = (ToolStripDropDown)(object)((obj is ToolStripDropDown) ? obj : null);
		}
		else if (((Message)(ref m)).Msg == 522 && activeMenu != null)
		{
			int delta = (short)(ushort)((uint)(long)((Message)(ref m)).WParam >> 16);
			HandleDelta(activeMenu, delta);
			return true;
		}
		return false;
	}

	private void HandleDelta(ToolStripDropDown ts, int delta)
	{
		if (((ArrangedElementCollection)((ToolStrip)ts).Items).Count == 0)
		{
			return;
		}
		ToolStripItem val = ((ToolStrip)ts).Items[0];
		ToolStripItem val2 = ((ToolStrip)ts).Items[((ArrangedElementCollection)((ToolStrip)ts).Items).Count - 1];
		if (val2.Bounds.Bottom >= ((Control)ts).Height || val.Bounds.Top <= 0)
		{
			delta /= -4;
			if (delta < 0 && val.Bounds.Top - delta > 9)
			{
				delta = val.Bounds.Top - 9;
			}
			else if (delta > 0 && delta > val2.Bounds.Bottom - ((Control)ts).Height + 9)
			{
				delta = val2.Bounds.Bottom - ((Control)ts).Height + 9;
			}
			if (delta != 0)
			{
				ScrollInternal((ToolStrip)(object)ts, delta);
			}
		}
	}
}
