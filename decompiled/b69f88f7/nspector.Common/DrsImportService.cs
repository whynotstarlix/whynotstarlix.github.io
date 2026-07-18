using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nspector.Common.Helper;
using nspector.Common.Import;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class DrsImportService : DrsSettingsServiceBase
{
	private readonly DrsSettingsService _SettingService;

	private readonly DrsScannerService _ScannerService;

	private readonly DrsDecrypterService _DecrypterService;

	public DrsImportService(DrsSettingsMetaService metaService, DrsSettingsService settingService, DrsScannerService scannerService, DrsDecrypterService decrypterService)
		: base(metaService)
	{
		_SettingService = settingService;
		_ScannerService = scannerService;
		_DecrypterService = decrypterService;
	}

	internal void ExportAllProfilesToNvidiaTextFile(string filename)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			SaveSettingsFileEx(hSession, filename);
		});
	}

	internal void ImportAllProfilesFromNvidiaTextFile(string filename)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			LoadSettingsFileEx(hSession, filename);
			SaveSettings(hSession);
		}, forceNonGlobalSession: true, preventLoadSettings: true);
	}

	internal void ExportProfiles(List<string> profileNames, string filename, bool includePredefined)
	{
		Profiles exports = new Profiles();
		DrsSession(delegate(IntPtr hSession)
		{
			foreach (string profileName in profileNames)
			{
				Profile item = CreateProfileForExport(hSession, profileName, includePredefined);
				exports.Add(item);
			}
		});
		XMLHelper<Profiles>.SerializeToXmlFile(exports, filename, Encoding.Unicode, removeNamespace: true);
	}

	private Profile CreateProfileForExport(IntPtr hSession, string profileName, bool includePredefined)
	{
		Profile profile = new Profile();
		IntPtr profileHandle = GetProfileHandle(hSession, profileName);
		if (profileHandle != IntPtr.Zero)
		{
			profile.ProfileName = profileName;
			List<NVDRS_APPLICATION_V4> profileApplications = GetProfileApplications(hSession, profileHandle);
			foreach (NVDRS_APPLICATION_V4 item2 in profileApplications)
			{
				profile.Executeables.Add(item2.appName);
			}
			List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
			foreach (NVDRS_SETTING item3 in profileSettings)
			{
				bool flag = item3.isCurrentPredefined == 1;
				if (item3.settingLocation == NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION && (!flag || includePredefined))
				{
					NVDRS_SETTING setting = item3;
					_DecrypterService.DecryptSettingIfNeeded(profileName, ref setting);
					ProfileSetting item = ImportExportUitl.ConvertDrsSettingToProfileSetting(setting);
					profile.Settings.Add(item);
				}
			}
		}
		return profile;
	}

	internal string ImportProfiles(string filename)
	{
		StringBuilder sbFailedProfilesMessage = new StringBuilder();
		bool appInUseHint = false;
		Profiles profiles = XMLHelper<Profiles>.DeserializeFromXMLFile(filename);
		DrsSession(delegate(IntPtr hSession)
		{
			foreach (Profile item in profiles)
			{
				bool flag = false;
				IntPtr intPtr = GetProfileHandle(hSession, item.ProfileName);
				if (intPtr == IntPtr.Zero)
				{
					intPtr = CreateProfile(hSession, item.ProfileName);
					NvapiDrsWrapper.DRS_SaveSettings(hSession);
					flag = true;
				}
				if (intPtr != IntPtr.Zero)
				{
					bool removeFromModified = false;
					_SettingService.ResetProfile(item.ProfileName, out removeFromModified);
					try
					{
						UpdateApplications(hSession, intPtr, item);
						UpdateSettings(hSession, intPtr, item, item.ProfileName);
					}
					catch (NvapiException ex)
					{
						if (flag)
						{
							NvapiDrsWrapper.DRS_DeleteProfile(hSession, intPtr);
						}
						sbFailedProfilesMessage.AppendLine($"Failed to import profile '{item.ProfileName}'");
						if (ex is NvapiAddApplicationException ex2)
						{
							string arg = _ScannerService.FindProfilesUsingApplication(ex2.ApplicationName);
							sbFailedProfilesMessage.AppendLine($"- application '{ex2.ApplicationName}' is already in use by profile '{arg}'");
							appInUseHint = true;
						}
						else
						{
							sbFailedProfilesMessage.AppendLine($"- {ex.Message}");
						}
						sbFailedProfilesMessage.AppendLine("");
					}
					NvapiDrsWrapper.DRS_SaveSettings(hSession);
				}
			}
		});
		if (appInUseHint)
		{
			sbFailedProfilesMessage.AppendLine("Hint: If just the profile name has been changed by nvidia, consider to manually modify the profile name inside the import file using a text editor.");
		}
		return sbFailedProfilesMessage.ToString();
	}

	private bool ExistsImportApp(string appName, Profile importProfile)
	{
		return importProfile.Executeables.Any((string x) => x.Equals(appName));
	}

	private void UpdateApplications(IntPtr hSession, IntPtr hProfile, Profile importProfile)
	{
		HashSet<string> hashSet = new HashSet<string>();
		List<NVDRS_APPLICATION_V4> profileApplications = GetProfileApplications(hSession, hProfile);
		foreach (NVDRS_APPLICATION_V4 item in profileApplications)
		{
			if (ExistsImportApp(item.appName, importProfile) && !hashSet.Contains(item.appName))
			{
				hashSet.Add(item.appName);
			}
			else
			{
				NvapiDrsWrapper.DRS_DeleteApplication(hSession, hProfile, new StringBuilder(item.appName));
			}
		}
		foreach (string executeable in importProfile.Executeables)
		{
			if (!hashSet.Contains(executeable))
			{
				try
				{
					AddApplication(hSession, hProfile, executeable);
				}
				catch (NvapiException)
				{
					throw new NvapiAddApplicationException(executeable);
				}
			}
		}
	}

	private uint GetImportValue(uint settingId, Profile importProfile)
	{
		ProfileSetting profileSetting = importProfile.Settings.FirstOrDefault((ProfileSetting x) => x.SettingId.Equals(settingId));
		if (profileSetting != null)
		{
			return uint.Parse(profileSetting.SettingValue);
		}
		return 0u;
	}

	private ProfileSetting GetImportProfileSetting(uint settingId, Profile importProfile)
	{
		return importProfile.Settings.FirstOrDefault((ProfileSetting x) => x.SettingId.Equals(settingId));
	}

	private bool ExistsImportValue(uint settingId, Profile importProfile)
	{
		return importProfile.Settings.Any((ProfileSetting x) => x.SettingId.Equals(settingId));
	}

	private void UpdateSettings(IntPtr hSession, IntPtr hProfile, Profile importProfile, string profileName)
	{
		HashSet<uint> hashSet = new HashSet<uint>();
		List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, hProfile);
		foreach (NVDRS_SETTING item in profileSettings)
		{
			bool flag = item.settingLocation == NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION;
			bool flag2 = item.isCurrentPredefined == 1;
			if (flag)
			{
				bool flag3 = ExistsImportValue(item.settingId, importProfile);
				ProfileSetting importProfileSetting = GetImportProfileSetting(item.settingId, importProfile);
				NVDRS_SETTING setting = item;
				_DecrypterService.DecryptSettingIfNeeded(profileName, ref setting);
				if (flag2 && flag3 && ImportExportUitl.AreDrsSettingEqualToProfileSetting(setting, importProfileSetting))
				{
					hashSet.Add(item.settingId);
				}
				else if (flag3)
				{
					NVDRS_SETTING newSetting = ImportExportUitl.ConvertProfileSettingToDrsSetting(importProfileSetting);
					StoreSetting(hSession, hProfile, newSetting);
					hashSet.Add(item.settingId);
				}
				else if (!flag2)
				{
					NvapiDrsWrapper.DRS_DeleteProfileSetting(hSession, hProfile, item.settingId);
				}
			}
		}
		foreach (ProfileSetting setting2 in importProfile.Settings)
		{
			if (hashSet.Contains(setting2.SettingId))
			{
				continue;
			}
			NVDRS_SETTING newSetting2 = ImportExportUitl.ConvertProfileSettingToDrsSetting(setting2);
			try
			{
				StoreSetting(hSession, hProfile, newSetting2);
			}
			catch (NvapiException ex)
			{
				if (ex.Status != NvAPI_Status.NVAPI_SETTING_NOT_FOUND)
				{
					throw;
				}
			}
		}
	}
}
