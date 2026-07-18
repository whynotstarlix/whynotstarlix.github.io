using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using nspector.Common.Helper;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class DrsScannerService(DrsSettingsMetaService metaService, DrsDecrypterService decrpterService) : DrsSettingsServiceBase(metaService, decrpterService)
{
	internal List<CachedSettings> CachedSettings = new List<CachedSettings>();

	internal List<string> ModifiedProfiles = new List<string>();

	internal HashSet<string> UserProfiles = new HashSet<string>();

	private readonly uint[] _commonSettingIds = new uint[18]
	{
		278257400u, 271834322u, 271830721u, 278073158u, 10512710u, 283958146u, 552327096u, 9821945u, 13983613u, 271834323u,
		271830722u, 544403510u, 6701881u, 2916165u, 546784961u, 7790948u, 553612435u, 541917051u
	};

	private bool CheckCommonSetting(IntPtr hSession, IntPtr hProfile, NVDRS_PROFILE profile, ref int checkedSettingsCount, uint checkSettingId, bool addToScanResult, ref List<uint> alreadyCheckedSettingIds)
	{
		if (checkedSettingsCount >= profile.numOfSettings)
		{
			return false;
		}
		NVDRS_SETTING pSetting = new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER
		};
		if (NvapiDrsWrapper.DRS_GetSetting(hSession, hProfile, checkSettingId, ref pSetting) != NvAPI_Status.NVAPI_OK)
		{
			return false;
		}
		if (pSetting.settingLocation != NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION)
		{
			return false;
		}
		if (!addToScanResult && pSetting.isCurrentPredefined == 1)
		{
			checkedSettingsCount++;
		}
		else
		{
			if (addToScanResult)
			{
				if (decrypter != null)
				{
					decrypter.DecryptSettingIfNeeded(profile.profileName, ref pSetting);
				}
				checkedSettingsCount++;
				AddScannedSettingToCache(profile, pSetting);
				alreadyCheckedSettingIds.Add(pSetting.settingId);
				return pSetting.isCurrentPredefined != 1;
			}
			if (pSetting.isCurrentPredefined != 1)
			{
				return true;
			}
		}
		return false;
	}

	private int CalcPercent(int current, int max)
	{
		return (current > 0) ? ((int)Math.Round((float)current * 100f / (float)max)) : 0;
	}

	public async Task ScanProfileSettingsAsync(bool justModified, IProgress<int> progress, CancellationToken token = default(CancellationToken))
	{
		await Task.Run(delegate
		{
			ModifiedProfiles = new List<string>();
			UserProfiles = new HashSet<string>();
			List<uint> knownPredefines = new List<uint>(_commonSettingIds);
			DrsSession(delegate(IntPtr hSession)
			{
				IntPtr profileHandle = GetProfileHandle(hSession, "");
				List<IntPtr> list = EnumProfileHandles(hSession);
				int count = list.Count;
				int num = 0;
				foreach (IntPtr item in list)
				{
					if (token.IsCancellationRequested)
					{
						break;
					}
					progress?.Report(CalcPercent(num++, count));
					NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, item);
					int checkedSettingsCount = 0;
					List<uint> alreadyCheckedSettingIds = new List<uint>();
					bool flag = false;
					if (profileInfo.isPredefined == 0)
					{
						ModifiedProfiles.Add(profileInfo.profileName);
						UserProfiles.Add(profileInfo.profileName);
						flag = true;
						if (justModified)
						{
							continue;
						}
					}
					foreach (uint item2 in knownPredefines)
					{
						if (CheckCommonSetting(hSession, item, profileInfo, ref checkedSettingsCount, item2, !justModified, ref alreadyCheckedSettingIds) && !flag)
						{
							flag = true;
							ModifiedProfiles.Add(profileInfo.profileName);
							if (justModified)
							{
								break;
							}
						}
					}
					if (!(flag && justModified) && checkedSettingsCount < profileInfo.numOfSettings)
					{
						List<NVDRS_SETTING> profileSettings = GetProfileSettings(hSession, item);
						foreach (NVDRS_SETTING item3 in profileSettings)
						{
							if (knownPredefines.IndexOf(item3.settingId) < 0)
							{
								knownPredefines.Add(item3.settingId);
							}
							if (!justModified && alreadyCheckedSettingIds.IndexOf(item3.settingId) < 0)
							{
								AddScannedSettingToCache(profileInfo, item3);
							}
							if (item3.isCurrentPredefined != 1 && !flag)
							{
								flag = true;
								ModifiedProfiles.Add(profileInfo.profileName);
								if (justModified)
								{
									break;
								}
							}
						}
					}
				}
			});
		});
	}

	private void AddScannedSettingToCache(NVDRS_PROFILE profile, NVDRS_SETTING setting)
	{
		bool flag = (setting.settingId & 0x70000000) != 1879048192;
		CachedSettings cachedSettings = CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId.Equals(setting.settingId));
		bool flag2 = true;
		if (cachedSettings == null)
		{
			flag2 = false;
			cachedSettings = new CachedSettings(setting.settingId, setting.settingType);
		}
		if (setting.isPredefinedValid != 1)
		{
			return;
		}
		if (flag)
		{
			if (setting.settingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
			{
				cachedSettings.AddStringValue(setting.predefinedValue.stringValue, profile.profileName);
			}
			else if (setting.settingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
			{
				cachedSettings.AddDwordValue(setting.predefinedValue.dwordValue, profile.profileName);
			}
			else if (setting.settingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
			{
				cachedSettings.AddBinaryValue(setting.predefinedValue.binaryValue, profile.profileName);
			}
		}
		else
		{
			cachedSettings.ProfileCount++;
		}
		if (!flag2)
		{
			CachedSettings.Add(cachedSettings);
		}
	}

	public string FindProfilesUsingApplication(string applicationName)
	{
		string text = applicationName.ToLowerInvariant();
		string tmpfile = TempFile.GetTempFileName();
		try
		{
			List<string> list = new List<string>();
			DrsSession(delegate(IntPtr hSession)
			{
				SaveSettingsFileEx(hSession, tmpfile);
			});
			if (File.Exists(tmpfile))
			{
				string input = File.ReadAllText(tmpfile);
				string pattern = "\\sProfile\\s\\\"(?<profile>.*?)\\\"(?<scope>.*?Executable.*?)EndProfile";
				foreach (Match item in Regex.Matches(input, pattern, RegexOptions.Singleline))
				{
					string input2 = item.Result("${scope}");
					foreach (Match item2 in Regex.Matches(input2, "Executable\\s\\\"(?<app>.*?)\\\"", RegexOptions.Singleline))
					{
						if (item2.Result("${app}").ToLowerInvariant() == text)
						{
							list.Add(item.Result("${profile}"));
						}
					}
				}
			}
			return string.Join(";", list);
		}
		finally
		{
			if (File.Exists(tmpfile))
			{
				File.Delete(tmpfile);
			}
		}
	}
}
