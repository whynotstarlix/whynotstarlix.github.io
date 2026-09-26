using System;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class FullScreenToast
{
	private Control mParent;

	private Toast mToast;

	private Timer mTimer;

	public FullScreenToast(Control parent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		mParent = parent;
		mTimer = new Timer
		{
			Interval = 5000
		};
		mTimer.Tick += Timeout;
	}

	public void Show()
	{
		Hide();
		mToast = new Toast(mParent, LocaleStrings.GetLocalizedString("STRING_FULL_SCREEN_TOAST", ""));
		int dwFlags = 262148;
		Animate.AnimateWindow(((Control)mToast).Handle, 500, dwFlags);
		((Control)mToast).Show();
		mTimer.Start();
	}

	public void Hide()
	{
		mTimer.Stop();
		if (mToast != null)
		{
			((Control)mToast).Hide();
			mToast = null;
		}
	}

	private void Timeout(object obj, EventArgs evt)
	{
		mTimer.Stop();
		int dwFlags = 327688;
		if (mToast != null)
		{
			Animate.AnimateWindow(((Control)mToast).Handle, 500, dwFlags);
			((Control)mToast).Hide();
		}
	}
}
