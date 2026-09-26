using BlueStacks.Common;

namespace BlueStacks.Player;

public sealed class Opt : GetOpt
{
	public bool h;

	public string vmname = "Android";

	public bool help;

	public bool w;

	public bool sysPrep;

	private static volatile Opt instance;

	private static object syncRoot = new object();

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
