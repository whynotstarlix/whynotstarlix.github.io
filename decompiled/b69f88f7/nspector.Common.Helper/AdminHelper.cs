using System.Security.Principal;

namespace nspector.Common.Helper;

public static class AdminHelper
{
	private static bool isAdmin;

	public static bool IsAdmin => isAdmin;

	static AdminHelper()
	{
		isAdmin = false;
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		isAdmin = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
	}
}
