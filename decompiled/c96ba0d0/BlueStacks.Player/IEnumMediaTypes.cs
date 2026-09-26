using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player;

[ComImport]
[SuppressUnmanagedCodeSecurity]
[Guid("89c31040-846b-11ce-97d3-00aa0055595a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumMediaTypes
{
	int Next([In] int cMediaTypes, [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] AMMediaType[] ppMediaTypes, [In] IntPtr pcFetched);

	int Skip([In] int cMediaTypes);

	int Reset();

	int Clone(out IEnumMediaTypes ppEnum);
}
