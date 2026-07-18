using System;

namespace nspector;

public struct NMHDR
{
	public IntPtr hwndFrom;

	public UIntPtr idFrom;

	public int code;
}
