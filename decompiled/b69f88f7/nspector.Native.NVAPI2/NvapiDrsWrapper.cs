using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace nspector.Native.NVAPI2;

internal class NvapiDrsWrapper
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate IntPtr nvapi_QueryInterfaceDelegate(uint id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status NvAPI_InitializeDelegate();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status InitializeDelegate();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status UnloadDelegate();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status GetErrorMessageDelegate(NvAPI_Status nr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder szDesc);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status GetInterfaceVersionStringDelegate([MarshalAs(UnmanagedType.LPStr)] StringBuilder szDesc);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status SYS_GetDriverAndBranchVersionDelegate(ref uint pDriverVersion, [MarshalAs(UnmanagedType.LPStr)] StringBuilder szBuildBranchString);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_CreateSessionDelegate(ref IntPtr phSession);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_DestroySessionDelegate(IntPtr hSession);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_LoadSettingsDelegate(IntPtr hSession);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SaveSettingsDelegate(IntPtr hSession);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_LoadSettingsFromFileDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder fileName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SaveSettingsToFileDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder fileName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_LoadSettingsFromFileExDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder fileName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SaveSettingsToFileExDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder fileName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_CreateProfileDelegate(IntPtr hSession, ref NVDRS_PROFILE pProfileInfo, ref IntPtr phProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_DeleteProfileDelegate(IntPtr hSession, IntPtr hProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SetCurrentGlobalProfileDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder wszGlobalProfileName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetCurrentGlobalProfileDelegate(IntPtr hSession, ref IntPtr phProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetProfileInfoDelegate(IntPtr hSession, IntPtr hProfile, ref NVDRS_PROFILE pProfileInfo);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SetProfileInfoDelegate(IntPtr hSession, IntPtr hProfile, ref NVDRS_PROFILE pProfileInfo);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_FindProfileByNameDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder profileName, ref IntPtr phProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_EnumProfilesDelegate(IntPtr hSession, uint index, ref IntPtr phProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetNumProfilesDelegate(IntPtr hSession, ref uint numProfiles);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_CreateApplicationDelegate(IntPtr hSession, IntPtr hProfile, ref NVDRS_APPLICATION_V4 pApplication);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_DeleteApplicationExDelegate(IntPtr hSession, IntPtr hProfile, ref NVDRS_APPLICATION_V4 pApp);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_DeleteApplicationDelegate(IntPtr hSession, IntPtr hProfile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder appName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetApplicationInfoDelegate(IntPtr hSession, IntPtr hProfile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder appName, ref NVDRS_APPLICATION_V4 pApplication);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate NvAPI_Status DRS_EnumApplicationsDelegate(IntPtr hSession, IntPtr hProfile, uint startIndex, ref uint appCount, IntPtr pApplication);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_FindApplicationByNameDelegate(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder appName, ref IntPtr phProfile, ref NVDRS_APPLICATION_V4 pApplication);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_SetSettingDelegate(IntPtr hSession, IntPtr hProfile, ref NVDRS_SETTING pSetting, uint x, uint y);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetSettingDelegate(IntPtr hSession, IntPtr hProfile, uint settingId, ref NVDRS_SETTING pSetting, ref uint x);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate NvAPI_Status DRS_EnumSettingsDelegate(IntPtr hSession, IntPtr hProfile, uint startIndex, ref uint settingsCount, IntPtr pSetting);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_EnumAvailableSettingIdsDelegate(IntPtr pSettingIds, ref uint pMaxCount);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate NvAPI_Status DRS_EnumAvailableSettingValuesDelegate(uint settingId, ref uint pMaxNumValues, IntPtr pSettingValues);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetSettingIdFromNameDelegate([MarshalAs(UnmanagedType.LPWStr)] StringBuilder settingName, ref uint pSettingId);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetSettingNameFromIdDelegate(uint settingId, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pSettingName);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_DeleteProfileSettingDelegate(IntPtr hSession, IntPtr hProfile, uint settingId);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_RestoreAllDefaultsDelegate(IntPtr hSession);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_RestoreProfileDefaultDelegate(IntPtr hSession, IntPtr hProfile);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_RestoreProfileDefaultSettingDelegate(IntPtr hSession, IntPtr hProfile, uint settingId);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NvAPI_Status DRS_GetBaseProfileDelegate(IntPtr hSession, ref IntPtr phProfile);

	public const uint NVAPI_GENERIC_STRING_MAX = 4096u;

	public const uint NVAPI_LONG_STRING_MAX = 256u;

	public const uint NVAPI_SHORT_STRING_MAX = 64u;

	public const uint NVAPI_MAX_PHYSICAL_GPUS = 64u;

	public const uint NVAPI_UNICODE_STRING_MAX = 2048u;

	public const uint NVAPI_BINARY_DATA_MAX = 4096u;

	public const uint NVAPI_SETTING_MAX_VALUES = 100u;

	public static uint NVDRS_SETTING_VALUES_VER;

	public static uint NVDRS_SETTING_VER;

	public static uint NVDRS_APPLICATION_VER_V1;

	public static uint NVDRS_APPLICATION_VER_V2;

	public static uint NVDRS_APPLICATION_VER_V3;

	public static uint NVDRS_APPLICATION_VER_V4;

	public static uint NVDRS_APPLICATION_VER;

	public static uint NVDRS_PROFILE_VER;

	public const uint OGL_IMPLICIT_GPU_AFFINITY_NUM_VALUES = 1u;

	public const uint CUDA_EXCLUDED_GPUS_NUM_VALUES = 1u;

	public const string D3DOGL_GPU_MAX_POWER_DEFAULTPOWER = "0";

	public const uint D3DOGL_GPU_MAX_POWER_NUM_VALUES = 1u;

	public const string D3DOGL_GPU_MAX_POWER_DEFAULT = "0";

	private static readonly nvapi_QueryInterfaceDelegate nvapi_QueryInterface;

	public static readonly NvAPI_InitializeDelegate NvAPI_Initialize;

	public static readonly InitializeDelegate Initialize;

	public static readonly UnloadDelegate Unload;

	public static readonly GetErrorMessageDelegate GetErrorMessage;

	public static readonly GetInterfaceVersionStringDelegate GetInterfaceVersionString;

	public static readonly SYS_GetDriverAndBranchVersionDelegate SYS_GetDriverAndBranchVersion;

	public static readonly DRS_CreateSessionDelegate DRS_CreateSession;

	public static readonly DRS_DestroySessionDelegate DRS_DestroySession;

	public static readonly DRS_LoadSettingsDelegate DRS_LoadSettings;

	public static readonly DRS_SaveSettingsDelegate DRS_SaveSettings;

	public static readonly DRS_LoadSettingsFromFileDelegate DRS_LoadSettingsFromFile;

	public static readonly DRS_SaveSettingsToFileDelegate DRS_SaveSettingsToFile;

	public static readonly DRS_LoadSettingsFromFileExDelegate DRS_LoadSettingsFromFileEx;

	public static readonly DRS_SaveSettingsToFileExDelegate DRS_SaveSettingsToFileEx;

	public static readonly DRS_CreateProfileDelegate DRS_CreateProfile;

	public static readonly DRS_DeleteProfileDelegate DRS_DeleteProfile;

	public static readonly DRS_SetCurrentGlobalProfileDelegate DRS_SetCurrentGlobalProfile;

	public static readonly DRS_GetCurrentGlobalProfileDelegate DRS_GetCurrentGlobalProfile;

	public static readonly DRS_GetProfileInfoDelegate DRS_GetProfileInfo;

	public static readonly DRS_SetProfileInfoDelegate DRS_SetProfileInfo;

	public static readonly DRS_FindProfileByNameDelegate DRS_FindProfileByName;

	public static readonly DRS_EnumProfilesDelegate DRS_EnumProfiles;

	public static readonly DRS_GetNumProfilesDelegate DRS_GetNumProfiles;

	public static readonly DRS_CreateApplicationDelegate DRS_CreateApplication;

	public static readonly DRS_DeleteApplicationExDelegate DRS_DeleteApplicationEx;

	public static readonly DRS_DeleteApplicationDelegate DRS_DeleteApplication;

	public static readonly DRS_GetApplicationInfoDelegate DRS_GetApplicationInfo;

	private static readonly DRS_EnumApplicationsDelegate DRS_EnumApplicationsInternal;

	public static readonly DRS_FindApplicationByNameDelegate DRS_FindApplicationByName;

	private static readonly DRS_SetSettingDelegate _DRS_SetSetting;

	private static readonly DRS_GetSettingDelegate _DRS_GetSetting;

	private static readonly DRS_EnumSettingsDelegate DRS_EnumSettingsInternal;

	public static readonly DRS_EnumAvailableSettingIdsDelegate DRS_EnumAvailableSettingIdsInternal;

	private static readonly DRS_EnumAvailableSettingValuesDelegate DRS_EnumAvailableSettingValuesInternal;

	public static readonly DRS_GetSettingIdFromNameDelegate DRS_GetSettingIdFromName;

	public static readonly DRS_GetSettingNameFromIdDelegate DRS_GetSettingNameFromId;

	public static readonly DRS_DeleteProfileSettingDelegate DRS_DeleteProfileSetting;

	public static readonly DRS_RestoreAllDefaultsDelegate DRS_RestoreAllDefaults;

	public static readonly DRS_RestoreProfileDefaultDelegate DRS_RestoreProfileDefault;

	public static readonly DRS_RestoreProfileDefaultSettingDelegate DRS_RestoreProfileDefaultSetting;

	public static readonly DRS_GetBaseProfileDelegate DRS_GetBaseProfile;

	private NvapiDrsWrapper()
	{
	}

	[DllImport("kernel32.dll")]
	private static extern IntPtr LoadLibrary(string dllname);

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

	private static uint MAKE_NVAPI_VERSION<T>(int version)
	{
		return (uint)(Marshal.SizeOf(typeof(T)) | (version << 16));
	}

	private static string GetDllName()
	{
		if (IntPtr.Size == 4)
		{
			return "nvapi.dll";
		}
		return "nvapi64.dll";
	}

	private static void GetDelegate<T>(uint id, out T newDelegate, uint? fallbackId = null) where T : class
	{
		IntPtr intPtr = nvapi_QueryInterface(id);
		if (intPtr != IntPtr.Zero)
		{
			newDelegate = Marshal.GetDelegateForFunctionPointer(intPtr, typeof(T)) as T;
		}
		else if (fallbackId.HasValue)
		{
			GetDelegate<T>(fallbackId.Value, out newDelegate);
		}
		else
		{
			newDelegate = null;
		}
	}

	private static T GetDelegateOfFunction<T>(IntPtr pLib, string signature)
	{
		T result = default(T);
		IntPtr procAddress = GetProcAddress(pLib, signature);
		if (procAddress != IntPtr.Zero)
		{
			return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T));
		}
		return result;
	}

	public static NvAPI_Status DRS_EnumApplications<TDrsAppVersion>(IntPtr hSession, IntPtr hProfile, uint startIndex, ref uint appCount, ref TDrsAppVersion[] apps)
	{
		NativeArrayHelper.SetArrayData(apps, out var targetPointer);
		NvAPI_Status result;
		try
		{
			result = DRS_EnumApplicationsInternal(hSession, hProfile, startIndex, ref appCount, targetPointer);
			apps = NativeArrayHelper.GetArrayData<TDrsAppVersion>(targetPointer, (int)appCount);
		}
		finally
		{
			Marshal.FreeHGlobal(targetPointer);
		}
		return result;
	}

	public static NvAPI_Status DRS_SetSetting(IntPtr hSession, IntPtr hProfile, ref NVDRS_SETTING pSetting)
	{
		return _DRS_SetSetting(hSession, hProfile, ref pSetting, 0u, 0u);
	}

	public static NvAPI_Status DRS_GetSetting(IntPtr hSession, IntPtr hProfile, uint settingId, ref NVDRS_SETTING pSetting)
	{
		uint x = 0u;
		return _DRS_GetSetting(hSession, hProfile, settingId, ref pSetting, ref x);
	}

	public static NvAPI_Status DRS_EnumSettings(IntPtr hSession, IntPtr hProfile, uint startIndex, ref uint settingsCount, ref NVDRS_SETTING[] settings)
	{
		NativeArrayHelper.SetArrayData(settings, out var targetPointer);
		NvAPI_Status result;
		try
		{
			result = DRS_EnumSettingsInternal(hSession, hProfile, startIndex, ref settingsCount, targetPointer);
			settings = NativeArrayHelper.GetArrayData<NVDRS_SETTING>(targetPointer, (int)settingsCount);
		}
		finally
		{
			Marshal.FreeHGlobal(targetPointer);
		}
		return result;
	}

	public static NvAPI_Status DRS_EnumAvailableSettingIds(out List<uint> settingIds, uint maxCount)
	{
		uint[] items = new uint[maxCount];
		IntPtr targetPointer = IntPtr.Zero;
		NativeArrayHelper.SetArrayData(items, out targetPointer);
		NvAPI_Status result;
		try
		{
			result = DRS_EnumAvailableSettingIdsInternal(targetPointer, ref maxCount);
			items = NativeArrayHelper.GetArrayData<uint>(targetPointer, (int)maxCount);
			settingIds = items.ToList();
		}
		finally
		{
			Marshal.FreeHGlobal(targetPointer);
		}
		return result;
	}

	public static NvAPI_Status DRS_EnumAvailableSettingValues(uint settingId, ref uint pMaxNumValues, ref NVDRS_SETTING_VALUES settingValues)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NVDRS_SETTING_VALUES)));
		NvAPI_Status result;
		try
		{
			settingValues.settingValues = new NVDRS_SETTING_UNION[100];
			Marshal.StructureToPtr(settingValues, intPtr, fDeleteOld: true);
			result = DRS_EnumAvailableSettingValuesInternal(settingId, ref pMaxNumValues, intPtr);
			settingValues = (NVDRS_SETTING_VALUES)Marshal.PtrToStructure(intPtr, typeof(NVDRS_SETTING_VALUES));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	static NvapiDrsWrapper()
	{
		NVDRS_SETTING_VALUES_VER = MAKE_NVAPI_VERSION<NVDRS_SETTING_VALUES>(1);
		NVDRS_SETTING_VER = MAKE_NVAPI_VERSION<NVDRS_SETTING>(1);
		NVDRS_APPLICATION_VER_V1 = MAKE_NVAPI_VERSION<NVDRS_APPLICATION_V1>(1);
		NVDRS_APPLICATION_VER_V2 = MAKE_NVAPI_VERSION<NVDRS_APPLICATION_V2>(2);
		NVDRS_APPLICATION_VER_V3 = MAKE_NVAPI_VERSION<NVDRS_APPLICATION_V3>(3);
		NVDRS_APPLICATION_VER_V4 = MAKE_NVAPI_VERSION<NVDRS_APPLICATION_V4>(4);
		NVDRS_APPLICATION_VER = NVDRS_APPLICATION_VER_V4;
		NVDRS_PROFILE_VER = MAKE_NVAPI_VERSION<NVDRS_PROFILE>(1);
		IntPtr intPtr = LoadLibrary(GetDllName());
		if (!(intPtr != IntPtr.Zero))
		{
			return;
		}
		nvapi_QueryInterface = GetDelegateOfFunction<nvapi_QueryInterfaceDelegate>(intPtr, "nvapi_QueryInterface");
		if (nvapi_QueryInterface != null)
		{
			GetDelegate<NvAPI_InitializeDelegate>(22079528u, out NvAPI_Initialize);
			if (NvAPI_Initialize() == NvAPI_Status.NVAPI_OK)
			{
				GetDelegate<InitializeDelegate>(22079528u, out Initialize);
				GetDelegate<UnloadDelegate>(3526090110u, out Unload);
				GetDelegate<GetErrorMessageDelegate>(1814889612u, out GetErrorMessage);
				GetDelegate<GetInterfaceVersionStringDelegate>(17121189u, out GetInterfaceVersionString);
				GetDelegate<SYS_GetDriverAndBranchVersionDelegate>(690399917u, out SYS_GetDriverAndBranchVersion);
				GetDelegate<DRS_CreateSessionDelegate>(110417198u, out DRS_CreateSession);
				GetDelegate<DRS_DestroySessionDelegate>(3671707640u, out DRS_DestroySession);
				GetDelegate<DRS_LoadSettingsDelegate>(928890219u, out DRS_LoadSettings);
				GetDelegate<DRS_SaveSettingsDelegate>(4240211476u, out DRS_SaveSettings);
				GetDelegate<DRS_LoadSettingsFromFileDelegate>(3555584137u, out DRS_LoadSettingsFromFile);
				GetDelegate<DRS_SaveSettingsToFileDelegate>(736255480u, out DRS_SaveSettingsToFile);
				GetDelegate<DRS_LoadSettingsFromFileExDelegate>(3325822043u, out DRS_LoadSettingsFromFileEx);
				GetDelegate<DRS_SaveSettingsToFileExDelegate>(308773262u, out DRS_SaveSettingsToFileEx);
				GetDelegate<DRS_CreateProfileDelegate>(3424084072u, out DRS_CreateProfile);
				GetDelegate<DRS_DeleteProfileDelegate>(386478598u, out DRS_DeleteProfile);
				GetDelegate<DRS_SetCurrentGlobalProfileDelegate>(478791135u, out DRS_SetCurrentGlobalProfile);
				GetDelegate<DRS_GetCurrentGlobalProfileDelegate>(1635516319u, out DRS_GetCurrentGlobalProfile);
				GetDelegate<DRS_GetProfileInfoDelegate>(1640853462u, out DRS_GetProfileInfo);
				GetDelegate<DRS_SetProfileInfoDelegate>(380359593u, out DRS_SetProfileInfo);
				GetDelegate<DRS_FindProfileByNameDelegate>(2118818315u, out DRS_FindProfileByName);
				GetDelegate<DRS_EnumProfilesDelegate>(3157728992u, out DRS_EnumProfiles);
				GetDelegate<DRS_GetNumProfilesDelegate>(497962940u, out DRS_GetNumProfiles);
				GetDelegate<DRS_CreateApplicationDelegate>(1128770014u, out DRS_CreateApplication);
				GetDelegate<DRS_DeleteApplicationExDelegate>(3320481185u, out DRS_DeleteApplicationEx);
				GetDelegate<DRS_DeleteApplicationDelegate>(745098182u, out DRS_DeleteApplication);
				GetDelegate<DRS_GetApplicationInfoDelegate>(3978267753u, out DRS_GetApplicationInfo);
				GetDelegate<DRS_EnumApplicationsDelegate>(2141329210u, out DRS_EnumApplicationsInternal);
				GetDelegate<DRS_FindApplicationByNameDelegate>(4008011442u, out DRS_FindApplicationByName);
				GetDelegate<DRS_SetSettingDelegate>(2318202357u, out _DRS_SetSetting, 1467863554u);
				GetDelegate<DRS_GetSettingDelegate>(3935914381u, out _DRS_GetSetting, 1941930808u);
				GetDelegate<DRS_EnumSettingsDelegate>(3486947390u, out DRS_EnumSettingsInternal, 2922396122u);
				GetDelegate<DRS_EnumAvailableSettingIdsDelegate>(3856550117u, out DRS_EnumAvailableSettingIdsInternal, 4028653898u);
				GetDelegate<DRS_EnumAvailableSettingValuesDelegate>(784572304u, out DRS_EnumAvailableSettingValuesInternal);
				GetDelegate<DRS_GetSettingIdFromNameDelegate>(3413313997u, out DRS_GetSettingIdFromName);
				GetDelegate<DRS_GetSettingNameFromIdDelegate>(514930577u, out DRS_GetSettingNameFromId, 3592207982u);
				GetDelegate<DRS_DeleteProfileSettingDelegate>(3524078047u, out DRS_DeleteProfileSetting, 3835847522u);
				GetDelegate<DRS_RestoreAllDefaultsDelegate>(1495773332u, out DRS_RestoreAllDefaults);
				GetDelegate<DRS_RestoreProfileDefaultDelegate>(4200554804u, out DRS_RestoreProfileDefault);
				GetDelegate<DRS_RestoreProfileDefaultSettingDelegate>(2111156833u, out DRS_RestoreProfileDefaultSetting, 1408251934u);
				GetDelegate<DRS_GetBaseProfileDelegate>(3666110112u, out DRS_GetBaseProfile);
			}
		}
	}
}
