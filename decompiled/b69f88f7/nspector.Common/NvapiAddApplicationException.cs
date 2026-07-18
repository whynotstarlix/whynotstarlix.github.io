using nspector.Native.NVAPI2;

namespace nspector.Common;

public class NvapiAddApplicationException : NvapiException
{
	public readonly string ApplicationName;

	public NvapiAddApplicationException(string applicationName)
		: base("DRS_CreateApplication", NvAPI_Status.NVAPI_EXECUTABLE_ALREADY_IN_USE)
	{
		ApplicationName = applicationName;
	}
}
