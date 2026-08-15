using System;

namespace BlueStacks.BlueStacksUI;

internal class MainWindowEventArgs
{
	internal class CursorLockChangedEventArgs : EventArgs
	{
		public bool IsLocked { get; set; }
	}

	internal class FullScreenChangedEventArgs : EventArgs
	{
		public bool IsFullscreen { get; set; }
	}

	internal class FrontendGridVisibilityChangedEventArgs : EventArgs
	{
		public bool IsVisible { get; set; }
	}

	internal class BrowserOTSCompletedCallbackEventArgs : EventArgs
	{
		internal string CallbackFunction { get; set; }
	}
}
