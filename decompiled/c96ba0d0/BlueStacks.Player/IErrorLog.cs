using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace BlueStacks.Player;

[ComImport]
[SuppressUnmanagedCodeSecurity]
[Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IErrorLog
{
	int AddError([In][MarshalAs(UnmanagedType.LPWStr)] string pszPropName, [In] EXCEPINFO pExcepInfo);
}
