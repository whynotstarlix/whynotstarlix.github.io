using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal abstract class DrsSettingsServiceBase
{
	public static readonly float DriverVersion = GetDriverVersionInternal();

	protected DrsSettingsMetaService meta;

	protected DrsDecrypterService decrypter;

	public DrsSettingsServiceBase(DrsSettingsMetaService metaService, DrsDecrypterService decrpterService = null)
	{
		meta = metaService;
		decrypter = decrpterService;
	}

	private static float GetDriverVersionInternal()
	{
		float result = 0f;
		uint pDriverVersion = 0u;
		StringBuilder szBuildBranchString = new StringBuilder(64);
		if (NvapiDrsWrapper.SYS_GetDriverAndBranchVersion(ref pDriverVersion, szBuildBranchString) == NvAPI_Status.NVAPI_OK)
		{
			try
			{
				result = (float)pDriverVersion / 100f;
			}
			catch
			{
			}
		}
		return result;
	}

	protected void DrsSession(Action<IntPtr> action, bool forceNonGlobalSession = false, bool preventLoadSettings = false)
	{
		DrsSessionScope.DrsSession(delegate(IntPtr hSession)
		{
			action(hSession);
			return true;
		}, forceNonGlobalSession, preventLoadSettings);
	}

	protected T DrsSession<T>(Func<IntPtr, T> action, bool forceDedicatedScope = false)
	{
		return DrsSessionScope.DrsSession(action, forceDedicatedScope);
	}

	protected IntPtr GetProfileHandle(IntPtr hSession, string profileName)
	{
		IntPtr phProfile = IntPtr.Zero;
		if (string.IsNullOrEmpty(profileName))
		{
			NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_GetCurrentGlobalProfile(hSession, ref phProfile);
			if (phProfile == IntPtr.Zero)
			{
				throw new NvapiException("DRS_GetCurrentGlobalProfile", NvAPI_Status.NVAPI_PROFILE_NOT_FOUND);
			}
			if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
			{
				throw new NvapiException("DRS_GetCurrentGlobalProfile", nvAPI_Status);
			}
		}
		else
		{
			NvAPI_Status nvAPI_Status2 = NvapiDrsWrapper.DRS_FindProfileByName(hSession, new StringBuilder(profileName), ref phProfile);
			switch (nvAPI_Status2)
			{
			case NvAPI_Status.NVAPI_PROFILE_NOT_FOUND:
				return IntPtr.Zero;
			default:
				throw new NvapiException("DRS_FindProfileByName", nvAPI_Status2);
			case NvAPI_Status.NVAPI_OK:
				break;
			}
		}
		return phProfile;
	}

	protected IntPtr CreateProfile(IntPtr hSession, string profileName)
	{
		if (string.IsNullOrEmpty(profileName))
		{
			throw new ArgumentNullException("profileName");
		}
		IntPtr phProfile = IntPtr.Zero;
		NVDRS_PROFILE pProfileInfo = new NVDRS_PROFILE
		{
			version = NvapiDrsWrapper.NVDRS_PROFILE_VER,
			profileName = profileName
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_CreateProfile(hSession, ref pProfileInfo, ref phProfile);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_CreateProfile", nvAPI_Status);
		}
		return phProfile;
	}

	protected NVDRS_PROFILE GetProfileInfo(IntPtr hSession, IntPtr hProfile)
	{
		NVDRS_PROFILE pProfileInfo = new NVDRS_PROFILE
		{
			version = NvapiDrsWrapper.NVDRS_PROFILE_VER
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_GetProfileInfo(hSession, hProfile, ref pProfileInfo);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_GetProfileInfo", nvAPI_Status);
		}
		return pProfileInfo;
	}

	protected void StoreSetting(IntPtr hSession, IntPtr hProfile, NVDRS_SETTING newSetting)
	{
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SetSetting(hSession, hProfile, ref newSetting);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SetSetting", nvAPI_Status);
		}
	}

	protected void StoreDwordValue(IntPtr hSession, IntPtr hProfile, uint settingId, uint dwordValue)
	{
		NVDRS_SETTING pSetting = new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER,
			settingId = settingId,
			settingType = NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE,
			settingLocation = NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION,
			currentValue = new NVDRS_SETTING_UNION
			{
				dwordValue = dwordValue
			}
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SetSetting(hSession, hProfile, ref pSetting);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SetSetting", nvAPI_Status);
		}
	}

	protected void StoreStringValue(IntPtr hSession, IntPtr hProfile, uint settingId, string stringValue)
	{
		NVDRS_SETTING pSetting = new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER,
			settingId = settingId,
			settingType = NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE,
			settingLocation = NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION,
			currentValue = new NVDRS_SETTING_UNION
			{
				stringValue = stringValue
			}
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SetSetting(hSession, hProfile, ref pSetting);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SetSetting", nvAPI_Status);
		}
	}

	protected void StoreBinaryValue(IntPtr hSession, IntPtr hProfile, uint settingId, byte[] binValue)
	{
		NVDRS_SETTING pSetting = new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER,
			settingId = settingId,
			settingType = NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE,
			settingLocation = NVDRS_SETTING_LOCATION.NVDRS_CURRENT_PROFILE_LOCATION,
			currentValue = new NVDRS_SETTING_UNION
			{
				binaryValue = binValue
			}
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SetSetting(hSession, hProfile, ref pSetting);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SetSetting", nvAPI_Status);
		}
	}

	protected NVDRS_SETTING? ReadSetting(IntPtr hSession, IntPtr hProfile, uint settingId)
	{
		NVDRS_SETTING pSetting = new NVDRS_SETTING
		{
			version = NvapiDrsWrapper.NVDRS_SETTING_VER
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_GetSetting(hSession, hProfile, settingId, ref pSetting);
		switch (nvAPI_Status)
		{
		case NvAPI_Status.NVAPI_SETTING_NOT_FOUND:
			return null;
		default:
			throw new NvapiException("DRS_GetSetting", nvAPI_Status);
		case NvAPI_Status.NVAPI_OK:
			if (decrypter != null)
			{
				NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, hProfile);
				decrypter.DecryptSettingIfNeeded(profileInfo.profileName, ref pSetting);
			}
			return pSetting;
		}
	}

	protected uint? ReadDwordValue(IntPtr hSession, IntPtr hProfile, uint settingId)
	{
		NVDRS_SETTING? nVDRS_SETTING = ReadSetting(hSession, hProfile, settingId);
		if (!nVDRS_SETTING.HasValue)
		{
			return null;
		}
		return nVDRS_SETTING.Value.currentValue.dwordValue;
	}

	protected void AddApplication(IntPtr hSession, IntPtr hProfile, string applicationName)
	{
		NVDRS_APPLICATION_V4 pApplication = new NVDRS_APPLICATION_V4
		{
			version = NvapiDrsWrapper.NVDRS_APPLICATION_VER_V4,
			appName = applicationName
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_CreateApplication(hSession, hProfile, ref pApplication);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_CreateApplication", nvAPI_Status);
		}
	}

	protected void DeleteApplication(IntPtr hSession, IntPtr hProfile, NVDRS_APPLICATION_V4 application)
	{
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_DeleteApplicationEx(hSession, hProfile, ref application);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_DeleteApplication", nvAPI_Status);
		}
	}

	protected List<IntPtr> EnumProfileHandles(IntPtr hSession)
	{
		List<IntPtr> list = new List<IntPtr>();
		IntPtr phProfile = IntPtr.Zero;
		uint num = 0u;
		while (true)
		{
			NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_EnumProfiles(hSession, num, ref phProfile);
			if (nvAPI_Status == NvAPI_Status.NVAPI_OK)
			{
				list.Add(phProfile);
			}
			num++;
			switch (nvAPI_Status)
			{
			case NvAPI_Status.NVAPI_OK:
				break;
			default:
				throw new NvapiException("DRS_EnumProfiles", nvAPI_Status);
			case NvAPI_Status.NVAPI_END_ENUMERATION:
				return list;
			}
		}
	}

	protected List<NVDRS_SETTING> GetProfileSettings(IntPtr hSession, IntPtr hProfile)
	{
		uint settingsCount = 512u;
		NVDRS_SETTING[] settings = new NVDRS_SETTING[settingsCount];
		settings[0].version = NvapiDrsWrapper.NVDRS_SETTING_VER;
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_EnumSettings(hSession, hProfile, 0u, ref settingsCount, ref settings);
		switch (nvAPI_Status)
		{
		case NvAPI_Status.NVAPI_END_ENUMERATION:
			return new List<NVDRS_SETTING>();
		default:
			throw new NvapiException("DRS_EnumSettings", nvAPI_Status);
		case NvAPI_Status.NVAPI_OK:
			if (decrypter != null)
			{
				NVDRS_PROFILE profileInfo = GetProfileInfo(hSession, hProfile);
				for (int i = 0; i < settingsCount; i++)
				{
					decrypter.DecryptSettingIfNeeded(profileInfo.profileName, ref settings[i]);
				}
			}
			return settings.ToList();
		}
	}

	protected List<NVDRS_APPLICATION_V4> GetProfileApplications(IntPtr hSession, IntPtr hProfile)
	{
		uint appCount = 512u;
		NVDRS_APPLICATION_V4[] apps = new NVDRS_APPLICATION_V4[512];
		apps[0].version = NvapiDrsWrapper.NVDRS_APPLICATION_VER_V4;
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_EnumApplications(hSession, hProfile, 0u, ref appCount, ref apps);
		return nvAPI_Status switch
		{
			NvAPI_Status.NVAPI_END_ENUMERATION => new List<NVDRS_APPLICATION_V4>(), 
			NvAPI_Status.NVAPI_OK => apps.ToList(), 
			_ => throw new NvapiException("DRS_EnumApplications", nvAPI_Status), 
		};
	}

	protected IntPtr FindApplicationByName(IntPtr hSession, string appName)
	{
		IntPtr phProfile = IntPtr.Zero;
		NVDRS_APPLICATION_V4 pApplication = new NVDRS_APPLICATION_V4
		{
			version = NvapiDrsWrapper.NVDRS_APPLICATION_VER_V4
		};
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_FindApplicationByName(hSession, new StringBuilder(appName), ref phProfile, ref pApplication);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_FindApplicationByName", nvAPI_Status);
		}
		return phProfile;
	}

	protected void SaveSettings(IntPtr hSession)
	{
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SaveSettings(hSession);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SaveSettings", nvAPI_Status);
		}
	}

	protected void LoadSettingsFileEx(IntPtr hSession, string filename)
	{
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_LoadSettingsFromFileEx(hSession, new StringBuilder(filename));
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_LoadSettingsFromFileEx", nvAPI_Status);
		}
	}

	protected void SaveSettingsFileEx(IntPtr hSession, string filename)
	{
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_SaveSettingsToFileEx(hSession, new StringBuilder(filename));
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_SaveSettingsToFileEx", nvAPI_Status);
		}
	}
}
