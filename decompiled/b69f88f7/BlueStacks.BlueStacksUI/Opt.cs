using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public sealed class Opt : GetOpt
{
	private static volatile Opt instance;

	private static object syncRoot = new object();

	private string json = "";

	public string vmname { get; set; } = "Android";

	public bool h { get; set; }

	public bool mergeCfg { get; set; }

	public bool isForceInstall { get; set; }

	public string newPDPath { get; set; } = string.Empty;

	public bool isUpgradeFromImap13 { get; set; }

	public bool force { get; set; }

	public bool launchedFromSysTray { get; set; }

	public string Json
	{
		get
		{
			return json;
		}
		set
		{
			json = Uri.UnescapeDataString(value);
		}
	}

	public bool hiddenBootMode { get; set; }

	public static Opt Instance
	{
		get
		{
			if (instance == null)
			{
				lock (syncRoot)
				{
					if (instance == null)
					{
						instance = new Opt();
					}
				}
			}
			return instance;
		}
	}

	private Opt()
	{
	}
}
