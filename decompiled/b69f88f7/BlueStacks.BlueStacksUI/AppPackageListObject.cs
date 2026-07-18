using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class AppPackageListObject
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public List<string> CloudPackageList { get; set; } = new List<string>();

	public AppPackageListObject(List<string> packageList)
	{
		CloudPackageList = packageList;
	}

	public bool IsPackageAvailable(string appPackage)
	{
		foreach (string cloudPackage in CloudPackageList)
		{
			string text = cloudPackage;
			if (cloudPackage.EndsWith("*", StringComparison.InvariantCulture))
			{
				text = cloudPackage.TrimEnd(new char[1] { '*' });
			}
			if (text.StartsWith("~", StringComparison.InvariantCulture))
			{
				if (appPackage.StartsWith(text.Substring(1), StringComparison.InvariantCulture))
				{
					return false;
				}
			}
			else if (appPackage.StartsWith(text, StringComparison.InvariantCulture))
			{
				return true;
			}
		}
		return false;
	}
}
