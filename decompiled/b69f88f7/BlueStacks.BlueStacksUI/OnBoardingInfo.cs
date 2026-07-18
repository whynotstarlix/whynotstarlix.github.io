using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class OnBoardingInfo
{
	public AppPackageListObject OnBoardingAppPackages { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int OnBoardingSkipTimer { get; set; } = 5;
}
