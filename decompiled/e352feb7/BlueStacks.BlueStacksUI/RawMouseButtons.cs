using System;

namespace BlueStacks.BlueStacksUI;

[Flags]
public enum RawMouseButtons : ushort
{
	None = 0,
	LeftDown = 1,
	LeftUp = 2,
	RightDown = 4,
	RightUp = 8,
	MiddleDown = 0x10,
	MiddleUp = 0x20,
	Button4Down = 0x40,
	Button4Up = 0x80,
	Button5Down = 0x100,
	Button5Up = 0x200,
	MouseWheel = 0x400
}
