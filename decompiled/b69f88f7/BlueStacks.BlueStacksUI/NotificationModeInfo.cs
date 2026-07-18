using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class NotificationModeInfo
{
	public AppPackageListObject NotificationModeAppPackages { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int ExitPopupCount { get; set; } = 3;
}
