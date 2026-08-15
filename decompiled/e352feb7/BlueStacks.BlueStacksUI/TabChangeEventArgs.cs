using System;

namespace BlueStacks.BlueStacksUI;

public class TabChangeEventArgs : EventArgs
{
	public string AppName { get; set; } = string.Empty;

	public string PackageName { get; set; } = string.Empty;

	public TabType TabType { get; set; }

	public TabChangeEventArgs(string appName, string packageName, TabType tabType)
	{
		AppName = appName;
		PackageName = packageName;
		TabType = tabType;
	}
}
