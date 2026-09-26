using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace BlueStacks.Player;

[ComImport]
[SuppressUnmanagedCodeSecurity]
[Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateDevEnum
{
	int CreateClassEnumerator([In][MarshalAs(UnmanagedType.LPStruct)] Guid pType, out IEnumMoniker ppEnumMoniker, [In][MarshalAs(UnmanagedType.I4)] int dwFlags);
}
