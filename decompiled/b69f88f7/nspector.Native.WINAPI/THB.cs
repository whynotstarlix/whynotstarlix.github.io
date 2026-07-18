using System;

namespace nspector.Native.WINAPI;

[Flags]
internal enum THB
{
	THB_BITMAP = 1,
	THB_ICON = 2,
	THB_TOOLTIP = 4,
	THB_FLAGS = 8,
	THBN_CLICKED = 0x1800
}
