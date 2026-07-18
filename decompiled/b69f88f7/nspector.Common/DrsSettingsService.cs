using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using nspector.Common.Helper;
using nspector.Common.Meta;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class DrsSettingsService : DrsSettingsServiceBase
{
	private readonly List<uint> _baseProfileSettingIds;

	public DrsSettingsService(DrsSettingsMetaService metaService, DrsDecrypterService decrpterService)
		: base(metaService, decrpterService)
	{
		_baseProfileSettingIds = InitBaseProfileSettingIds();
	}

	private List<uint> InitBaseProfileSettingIds()
	{
		return DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, "");
			List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
			return profileSettings.Select((NVDRS_SETTING x) => x.settingId).ToList();
		});
	}

	private string GetDrsProgramPath()
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation\\Installer2");
		IEnumerable<string> enumerable = Directory.EnumerateDirectories(path, "Display.Driver.*");
		foreach (string item in enumerable)
		{
			FileInfo fileInfo = new FileInfo(Path.Combine(item, "dbInstaller.exe"));
			if (fileInfo.Exists)
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
				string text = versionInfo.FileVersion.Replace(".", "");
				float driverVersion = DrsSettingsServiceBase.DriverVersion;
				string value = driverVersion.ToString().Replace(",", "").Replace(".", "");
				if (text.EndsWith(value))
				{
					return fileInfo.DirectoryName;
				}
			}
		}
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NVIDIA Corporation\\Drs");
	}

	private void RunDrsInitProcess()
	{
		string drsProgramPath = GetDrsProgramPath();
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.UseShellExecute = true;
		processStartInfo.WorkingDirectory = drsProgramPath;
		processStartInfo.Arguments = "-init";
		processStartInfo.FileName = Path.Combine(drsProgramPath, "dbInstaller.exe");
		if (!AdminHelper.IsAdmin)
		{
			processStartInfo.Verb = "runas";
		}
		Process process = Process.Start(processStartInfo);
		process.WaitForExit();
	}

	public void DeleteAllProfilesHard()
	{
		string tmpFile = TempFile.GetTempFileName();
		try
		{
			File.WriteAllText(tmpFile, "BaseProfile \"Base Profile\"\r\nSelectedGlobalProfile \"Base Profile\"\r\nProfile \"Base Profile\"\r\nShowOn All\r\nProfileType Global\r\nEndProfile\r\n");
			DrsSession(delegate(IntPtr hSession)
			{
				LoadSettingsFileEx(hSession, tmpFile);
				SaveSettings(hSession);
			}, forceNonGlobalSession: true, preventLoadSettings: true);
		}
		finally
		{
			if (File.Exists(tmpFile))
			{
				File.Delete(tmpFile);
			}
		}
	}

	public void DeleteProfileHard(string profileName)
	{
		string tmpFileName = TempFile.GetTempFileName();
		try
		{
			string tmpFileContent = "";
			DrsSession(delegate(IntPtr hSession)
			{
				SaveSettingsFileEx(hSession, tmpFileName);
				tmpFileContent = File.ReadAllText(tmpFileName);
				string pattern = "(?<rpl>\nProfile\\s\"" + Regex.Escape(profileName) + "\".*?EndProfile.*?\n)";
				tmpFileContent = Regex.Replace(tmpFileContent, pattern, "", RegexOptions.Singleline);
				File.WriteAllText(tmpFileName, tmpFileContent);
			});
			if (tmpFileContent != "")
			{
				DrsSession(delegate(IntPtr hSession)
				{
					LoadSettingsFileEx(hSession, tmpFileName);
					SaveSettings(hSession);
				});
			}
		}
		finally
		{
			if (File.Exists(tmpFileName))
			{
				File.Delete(tmpFileName);
			}
		}
	}

	public void DeleteProfile(string profileName)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			if (profileHandle != IntPtr.Zero)
			{
				NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_DeleteProfile(hSession, profileHandle);
				if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
				{
					throw new NvapiException("DRS_DeleteProfile", nvAPI_Status);
				}
				SaveSettings(hSession);
			}
		});
	}

	public string GetProfileNameByExeName(string appName)
	{
		string profileName = string.Empty;
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr intPtr = FindApplicationByName(hSession, appName);
			if (intPtr != IntPtr.Zero)
			{
				NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, intPtr);
				if (profileInfo.isPredefined == 0 || profileInfo.numOfApps != 0)
				{
					profileName = profileInfo.profileName;
				}
			}
		});
		return profileName;
	}

	public List<string> GetProfileNames(ref string baseProfileName)
	{
		List<string> lstResult = new List<string>();
		string tmpBaseProfileName = baseProfileName;
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, null);
			tmpBaseProfileName = GetProfileInfo(hSession, profileHandle).profileName;
			lstResult.Add("_GLOBAL_DRIVER_PROFILE (" + tmpBaseProfileName + ")");
			List<IntPtr> list = EnumProfileHandles(hSession);
			foreach (IntPtr item in list)
			{
				NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, item);
				if (profileInfo.isPredefined == 0 || profileInfo.numOfApps != 0)
				{
					lstResult.Add(profileInfo.profileName);
				}
			}
		});
		baseProfileName = tmpBaseProfileName;
		return lstResult;
	}

	public void CreateProfile(string profileName, string applicationName = null)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr hProfile = CreateProfile(hSession, profileName);
			if (applicationName != null)
			{
				AddApplication(hSession, hProfile, applicationName);
			}
			SaveSettings(hSession);
		});
	}

	public void ResetAllProfilesInternal()
	{
		RunDrsInitProcess();
		DrsSession(delegate(IntPtr hSession)
		{
			NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_RestoreAllDefaults(hSession);
			if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
			{
				throw new NvapiException("DRS_RestoreAllDefaults", nvAPI_Status);
			}
			SaveSettings(hSession);
		});
	}

	public void ResetProfile(string profileName, out bool removeFromModified)
	{
		bool tmpRemoveFromModified = false;
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, profileHandle);
			if (profileInfo.isPredefined == 1)
			{
				NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_RestoreProfileDefault(hSession, profileHandle);
				if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
				{
					throw new NvapiException("DRS_RestoreProfileDefault", nvAPI_Status);
				}
				SaveSettings(hSession);
				tmpRemoveFromModified = true;
			}
			else if (profileInfo.numOfSettings != 0)
			{
				int num = 0;
				List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
				foreach (NVDRS_SETTING item in profileSettings)
				{
					if (item.settingLocation == NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION && NvapiDrsWrapper.DRS_DeleteProfileSetting(hSession, profileHandle, item.settingId) == NvAPI_Status.NVAPI_OK)
					{
						num++;
					}
				}
				if (num > 0)
				{
					SaveSettings(hSession);
				}
			}
		});
		removeFromModified = tmpRemoveFromModified;
	}

	public void ResetValue(string profileName, uint settingId, out bool removeFromModified)
	{
		bool tmpRemoveFromModified = false;
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			if (profileHandle != IntPtr.Zero)
			{
				NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_RestoreProfileDefaultSetting(hSession, profileHandle, settingId);
				if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
				{
					throw new NvapiException("DRS_RestoreProfileDefaultSetting", nvAPI_Status);
				}
				SaveSettings(hSession);
				int num = 0;
				List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
				foreach (NVDRS_SETTING item in profileSettings)
				{
					if (item.isCurrentPredefined == 0 && item.settingLocation == NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION)
					{
						num++;
					}
				}
				tmpRemoveFromModified = num == 0;
			}
		});
		removeFromModified = tmpRemoveFromModified;
	}

	public void DeleteValue(string profileName, uint settingId, out bool removeFromModified)
	{
		bool tmpRemoveFromModified = false;
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			if (profileHandle != IntPtr.Zero)
			{
				int num = 0;
				List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
				foreach (NVDRS_SETTING item in profileSettings)
				{
					if (item.settingId == settingId && item.settingLocation == NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION && NvapiDrsWrapper.DRS_DeleteProfileSetting(hSession, profileHandle, item.settingId) == NvAPI_Status.NVAPI_OK)
					{
						num++;
						break;
					}
				}
				tmpRemoveFromModified = num == 0;
				SaveSettings(hSession);
			}
		});
		removeFromModified = tmpRemoveFromModified;
	}

	public uint GetDwordValueFromProfile(string profileName, uint settingId, bool returnDefaultValue = false, bool forceDedicatedScope = false)
	{
		return DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			uint? num = ReadDwordValue(hSession, profileHandle, settingId);
			if (num.HasValue)
			{
				return num.Value;
			}
			if (!returnDefaultValue)
			{
				throw new NvapiException("DRS_GetSetting", NvAPI_Status.NVAPI_SETTING_NOT_FOUND);
			}
			return meta.GetSettingMeta(settingId).DefaultDwordValue;
		});
	}

	public void SetDwordValueToProfile(string profileName, uint settingId, uint dwordValue)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			StoreDwordValue(hSession, profileHandle, settingId, dwordValue);
			SaveSettings(hSession);
		});
	}

	public int StoreSettingsToProfile(string profileName, List<KeyValuePair<uint, string>> settings)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			foreach (KeyValuePair<uint, string> setting in settings)
			{
				SettingMeta settingMeta = meta.GetSettingMeta(setting.Key);
				NVDRS_SETTING_TYPE? settingType = settingMeta.SettingType;
				if (settingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
				{
					uint dwordValue = DrsUtil.ParseDwordSettingValue(settingMeta, setting.Value);
					StoreDwordValue(hSession, profileHandle, setting.Key, dwordValue);
				}
				else if (settingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
				{
					string stringValue = DrsUtil.ParseStringSettingValue(settingMeta, setting.Value);
					StoreStringValue(hSession, profileHandle, setting.Key, stringValue);
				}
				else if (settingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
				{
					byte[] binValue = DrsUtil.ParseBinarySettingValue(settingMeta, setting.Value);
					StoreBinaryValue(hSession, profileHandle, setting.Key, binValue);
				}
			}
			SaveSettings(hSession);
		});
		return 0;
	}

	private SettingItem CreateSettingItem(NVDRS_SETTING setting, bool useDefault = false)
	{
		SettingMeta settingMeta = meta.GetSettingMeta(setting.settingId);
		if (settingMeta.DwordValues == null)
		{
			settingMeta.DwordValues = new List<SettingValue<uint>>();
		}
		if (settingMeta.StringValues == null)
		{
			settingMeta.StringValues = new List<SettingValue<string>>();
		}
		if (settingMeta.BinaryValues == null)
		{
			settingMeta.BinaryValues = new List<SettingValue<byte[]>>();
		}
		SettingState state = SettingState.NotAssiged;
		string valueRaw = "";
		string valueText = "";
		if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
		{
			if (useDefault)
			{
				valueRaw = DrsUtil.GetDwordString(settingMeta.DefaultDwordValue);
				valueText = DrsUtil.GetDwordSettingValueName(settingMeta, settingMeta.DefaultDwordValue);
			}
			else if (setting.isCurrentPredefined == 1 && setting.isPredefinedValid == 1)
			{
				valueRaw = DrsUtil.GetDwordString(setting.predefinedValue.dwordValue);
				valueText = DrsUtil.GetDwordSettingValueName(settingMeta, setting.predefinedValue.dwordValue);
				state = ((setting.settingLocation != NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION) ? SettingState.GlobalSetting : SettingState.NvidiaSetting);
			}
			else
			{
				valueRaw = DrsUtil.GetDwordString(setting.currentValue.dwordValue);
				valueText = DrsUtil.GetDwordSettingValueName(settingMeta, setting.currentValue.dwordValue);
				state = ((setting.settingLocation != NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION) ? SettingState.GlobalSetting : SettingState.UserdefinedSetting);
			}
		}
		if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
		{
			if (useDefault)
			{
				valueRaw = settingMeta.DefaultStringValue;
				valueText = DrsUtil.GetStringSettingValueName(settingMeta, settingMeta.DefaultStringValue);
			}
			else if (setting.isCurrentPredefined == 1 && setting.isPredefinedValid == 1)
			{
				valueRaw = setting.predefinedValue.stringValue;
				valueText = DrsUtil.GetStringSettingValueName(settingMeta, setting.predefinedValue.stringValue);
				state = SettingState.NvidiaSetting;
			}
			else
			{
				valueRaw = setting.currentValue.stringValue;
				valueText = DrsUtil.GetStringSettingValueName(settingMeta, setting.currentValue.stringValue);
				state = ((setting.settingLocation != NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION) ? SettingState.GlobalSetting : SettingState.UserdefinedSetting);
			}
		}
		if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
		{
			if (useDefault)
			{
				valueRaw = DrsUtil.GetBinaryString(settingMeta.DefaultBinaryValue);
				valueText = DrsUtil.GetBinarySettingValueName(settingMeta, settingMeta.DefaultBinaryValue);
			}
			else if (setting.isCurrentPredefined == 1 && setting.isPredefinedValid == 1)
			{
				valueRaw = DrsUtil.GetBinaryString(setting.predefinedValue.binaryValue);
				valueText = DrsUtil.GetBinarySettingValueName(settingMeta, setting.predefinedValue.binaryValue);
				state = SettingState.NvidiaSetting;
			}
			else
			{
				valueRaw = DrsUtil.GetBinaryString(setting.currentValue.binaryValue);
				valueText = DrsUtil.GetBinarySettingValueName(settingMeta, setting.currentValue.binaryValue);
				state = ((setting.settingLocation != NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION) ? SettingState.GlobalSetting : SettingState.UserdefinedSetting);
			}
		}
		return new SettingItem
		{
			SettingId = setting.settingId,
			SettingText = settingMeta.SettingName,
			GroupName = settingMeta.GroupName,
			AlternateNames = settingMeta.AlternateNames,
			ValueRaw = valueRaw,
			ValueText = valueText,
			State = state,
			IsStringValue = (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE),
			IsApiExposed = settingMeta.IsApiExposed,
			IsSettingHidden = settingMeta.IsSettingHidden
		};
	}

	public List<SettingItem> GetSettingsForProfile(string profileName, SettingViewMode viewMode, ref Dictionary<string, string> applications)
	{
		List<SettingItem> result = new List<SettingItem>();
		List<uint> settingIds = meta.GetSettingIds(viewMode);
		settingIds.AddRange(_baseProfileSettingIds);
		settingIds = settingIds.Distinct().ToList();
		applications = DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, profileHandle);
			foreach (NVDRS_SETTING item in profileSettings)
			{
				result.Add(CreateSettingItem(item));
				if (settingIds.Contains(item.settingId))
				{
					settingIds.Remove(item.settingId);
				}
			}
			foreach (uint item2 in settingIds)
			{
				if (item2 != 0)
				{
					NVDRS_SETTING? nVDRS_SETTING = ReadSetting(hSession, profileHandle, item2);
					if (nVDRS_SETTING.HasValue)
					{
						result.Add(CreateSettingItem(nVDRS_SETTING.Value));
					}
					else
					{
						NVDRS_SETTING setting = new NVDRS_SETTING
						{
							settingId = item2
						};
						result.Add(CreateSettingItem(setting, useDefault: true));
					}
				}
			}
			return (from x in GetProfileApplications(hSession, profileHandle)
				select Tuple.Create(x.appName, GetApplicationFingerprint(x))).ToDictionary((Tuple<string, string> x) => x.Item2, (Tuple<string, string> x) => x.Item1);
		});
		return (from x in result
			orderby x.SettingText, x.GroupName
			select x).ToList();
	}

	public void AddApplication(string profileName, string applicationName)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			AddApplication(hSession, profileHandle, applicationName);
			SaveSettings(hSession);
		});
	}

	public void RemoveApplication(string profileName, string applicationFingerprint)
	{
		DrsSession(delegate(IntPtr hSession)
		{
			IntPtr profileHandle = GetProfileHandle(hSession, profileName);
			List<NVDRS_APPLICATION_V4> profileApplications = GetProfileApplications(hSession, profileHandle);
			foreach (NVDRS_APPLICATION_V4 item in profileApplications)
			{
				if (!(GetApplicationFingerprint(item) != applicationFingerprint))
				{
					DeleteApplication(hSession, profileHandle, item);
					break;
				}
			}
			SaveSettings(hSession);
		});
	}

	private string GetApplicationFingerprint(NVDRS_APPLICATION_V4 application)
	{
		return application.appName + "|" + application.fileInFolder + "|" + application.userFriendlyName + "|" + application.launcher;
	}
}
