using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace nspector.Common.Helper;

public class UserSettings
{
	public int WindowTop { get; set; }

	public int WindowLeft { get; set; }

	public int WindowWidth { get; set; }

	public int WindowHeight { get; set; }

	public FormWindowState WindowState { get; set; }

	public bool ShowCustomizedSettingNamesOnly { get; set; } = false;

	public bool ShowScannedUnknownSettings { get; set; } = false;

	public List<string> HiddenSettingGroups { get; set; } = new List<string>();

	public bool DisableUpdateCheck { get; set; } = false;

	private static string GetSettingsFilename()
	{
		FileInfo fileInfo = new FileInfo("settings.xml");
		if (fileInfo.Exists)
		{
			return fileInfo.FullName;
		}
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return Path.Combine(text, "settings.xml");
	}

	public void SaveSettings()
	{
		XMLHelper<UserSettings>.SerializeToXmlFile(this, GetSettingsFilename(), Encoding.Unicode, removeNamespace: true);
	}

	public static UserSettings LoadSettings()
	{
		string settingsFilename = GetSettingsFilename();
		if (!File.Exists(settingsFilename))
		{
			return new UserSettings();
		}
		try
		{
			return XMLHelper<UserSettings>.DeserializeFromXMLFile(GetSettingsFilename());
		}
		catch
		{
			return new UserSettings();
		}
	}
}
