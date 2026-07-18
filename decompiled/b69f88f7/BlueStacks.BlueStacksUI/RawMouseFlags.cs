using System;

namespace BlueStacks.BlueStacksUI;

[Flags]
public enum RawMouseFlags : ushort
{
	MoveRelative = 0,
	MoveAbsolute = 1,
	VirtualDesktop = 2,
	AttributesChanged = 4
}
