using System;

namespace BlueStacks.BlueStacksUI;

internal class MainWindowsStaticComponents
{
	internal AppTabButton mSelectedTabButton;

	internal bool mPreviousSelectedTabWeb;

	internal HomeAppTabButton mSelectedHomeAppTabButton;

	internal bool IsDeleteButtonVisible;

	internal IntPtr mLastMappableWindowHandle = IntPtr.Zero;

	internal event EventHandler ShowAllUninstallButtons;

	internal event EventHandler HideAllUninstallButtons;

	internal event Action PlayAllGifs;

	internal event Action PauseAllGifs;

	internal void ShowUninstallButtons(bool isShow)
	{
		IsDeleteButtonVisible = isShow;
		if (isShow)
		{
			this.ShowAllUninstallButtons?.Invoke(null, new EventArgs());
		}
		else
		{
			this.HideAllUninstallButtons?.Invoke(null, new EventArgs());
		}
	}

	internal void PlayPauseGifs(bool isPlay)
	{
		if (isPlay)
		{
			this.PlayAllGifs?.Invoke();
		}
		else
		{
			this.PauseAllGifs?.Invoke();
		}
	}
}
