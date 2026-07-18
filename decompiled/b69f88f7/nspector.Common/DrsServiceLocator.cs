using System.IO;
using System.Reflection;
using nspector.Common.CustomSettings;
using nspector.Properties;

namespace nspector.Common;

internal class DrsServiceLocator
{
	private static readonly CustomSettingNames CustomSettings;

	public static readonly CustomSettingNames ReferenceSettings;

	public static readonly DrsSettingsMetaService MetaService;

	public static readonly DrsSettingsService SettingService;

	public static readonly DrsImportService ImportService;

	public static readonly DrsScannerService ScannerService;

	public static readonly DrsDecrypterService DecrypterService;

	public static bool IsExternalCustomSettings { get; private set; }

	static DrsServiceLocator()
	{
		IsExternalCustomSettings = false;
		CustomSettings = LoadCustomSettings();
		ReferenceSettings = LoadReferenceSettings();
		MetaService = new DrsSettingsMetaService(CustomSettings, ReferenceSettings);
		DecrypterService = new DrsDecrypterService(MetaService);
		ScannerService = new DrsScannerService(MetaService, DecrypterService);
		SettingService = new DrsSettingsService(MetaService, DecrypterService);
		ImportService = new DrsImportService(MetaService, SettingService, ScannerService, DecrypterService);
	}

	private static CustomSettingNames LoadCustomSettings()
	{
		string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\CustomSettingNames.xml";
		if (File.Exists(text))
		{
			try
			{
				CustomSettingNames result = CustomSettingNames.FactoryLoadFromFile(text);
				IsExternalCustomSettings = true;
				return result;
			}
			catch
			{
				return CustomSettingNames.FactoryLoadFromString(Resources.CustomSettingNames);
			}
		}
		return CustomSettingNames.FactoryLoadFromString(Resources.CustomSettingNames);
	}

	private static CustomSettingNames LoadReferenceSettings()
	{
		string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Reference.xml";
		try
		{
			if (File.Exists(text))
			{
				return CustomSettingNames.FactoryLoadFromFile(text);
			}
		}
		catch
		{
		}
		return null;
	}
}
