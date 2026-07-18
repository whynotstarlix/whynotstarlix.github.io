using System;
using nspector.Native.NVAPI2;

namespace nspector.Common;

public class DrsSessionScope
{
	public static volatile IntPtr GlobalSession;

	public static volatile bool HoldSession = true;

	private static object _Sync = new object();

	public static T DrsSession<T>(Func<IntPtr, T> action, bool forceNonGlobalSession = false, bool preventLoadSettings = false)
	{
		lock (_Sync)
		{
			if (!HoldSession || forceNonGlobalSession)
			{
				return NonGlobalDrsSession(action, preventLoadSettings);
			}
			if (GlobalSession == IntPtr.Zero)
			{
				NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_CreateSession(ref GlobalSession);
				if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
				{
					throw new NvapiException("DRS_CreateSession", nvAPI_Status);
				}
				if (!preventLoadSettings)
				{
					NvAPI_Status nvAPI_Status2 = NvapiDrsWrapper.DRS_LoadSettings(GlobalSession);
					if (nvAPI_Status2 != NvAPI_Status.NVAPI_OK)
					{
						throw new NvapiException("DRS_LoadSettings", nvAPI_Status2);
					}
				}
			}
		}
		if (GlobalSession != IntPtr.Zero)
		{
			return action(GlobalSession);
		}
		throw new Exception("GlobalSession is Zero!");
	}

	public static void DestroyGlobalSession()
	{
		lock (_Sync)
		{
			if (GlobalSession != IntPtr.Zero)
			{
				NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_DestroySession(GlobalSession);
				GlobalSession = IntPtr.Zero;
			}
		}
	}

	private static T NonGlobalDrsSession<T>(Func<IntPtr, T> action, bool preventLoadSettings = false)
	{
		IntPtr phSession = IntPtr.Zero;
		NvAPI_Status nvAPI_Status = NvapiDrsWrapper.DRS_CreateSession(ref phSession);
		if (nvAPI_Status != NvAPI_Status.NVAPI_OK)
		{
			throw new NvapiException("DRS_CreateSession", nvAPI_Status);
		}
		try
		{
			if (!preventLoadSettings)
			{
				NvAPI_Status nvAPI_Status2 = NvapiDrsWrapper.DRS_LoadSettings(phSession);
				if (nvAPI_Status2 != NvAPI_Status.NVAPI_OK)
				{
					throw new NvapiException("DRS_LoadSettings", nvAPI_Status2);
				}
			}
			return action(phSession);
		}
		finally
		{
			NvAPI_Status nvAPI_Status3 = NvapiDrsWrapper.DRS_DestroySession(phSession);
			if (nvAPI_Status3 != NvAPI_Status.NVAPI_OK)
			{
				throw new NvapiException("DRS_DestroySession", nvAPI_Status3);
			}
		}
	}
}
