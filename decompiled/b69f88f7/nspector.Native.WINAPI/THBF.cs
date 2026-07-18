using System;

namespace nspector.Native.WINAPI;

[Flags]
internal enum THBF
{
	THBF_ENABLED = 0,
	THBF_DISABLED = 1,
	THBF_DISMISSONCLICK = 2,
	THBF_NOBACKGROUND = 4,
	THBF_HIDDEN = 8
}
