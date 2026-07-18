using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class SidebarConfig
{
	private static volatile SidebarConfig sInstance;

	private static object syncRoot = new object();

	public static string sFilePath = Path.Combine(RegistryStrings.GadgetDir, string.Format(CultureInfo.InvariantCulture, "SidebarConfig_{0}.json", new object[1] { "Android" }));

	public List<List<string>> GroupElements { get; } = new List<List<string>>();

	public static SidebarConfig Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot)
				{
					if (sInstance == null)
					{
						sInstance = new SidebarConfig();
						sInstance.Init(sFilePath);
					}
				}
			}
			return sInstance;
		}
		set
		{
			sInstance = value;
		}
	}

	private void Init(string filePath)
	{
		InitFile(filePath);
		JObject obj = JObject.Parse(File.ReadAllText(filePath));
		int num = 0;
		foreach (JProperty item in from x in obj.Properties()
			orderby x.Name
			select x)
		{
			List<string> list = new List<string>();
			foreach (JProperty item2 in from x in item.Value.ToObject<JObject>().Properties()
				orderby x.Name
				select x)
			{
				list.Add(((object)item2.Value).ToString());
			}
			GroupElements.Add(list);
			num++;
		}
	}

	private static void InitFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			InitNewFile(filePath);
		}
	}

	public static void InitNewFile(string filePath)
	{
		File.Copy(Path.Combine(RegistryStrings.GadgetDir, "sidebar_config.json"), filePath);
	}
}
