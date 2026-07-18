using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI;

public class CrosshairStorage
{
	public static string LastCrosshairConfig
	{
		get
		{
			try
			{
				using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\BluesterCrosshair");
				if (registryKey != null)
				{
					return registryKey.GetValue("Config", "").ToString();
				}
			}
			catch
			{
			}
			return "";
		}
		set
		{
			try
			{
				using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\BluesterCrosshair");
				registryKey?.SetValue("Config", value ?? "");
			}
			catch
			{
			}
		}
	}

	static CrosshairStorage()
	{
	}
}
