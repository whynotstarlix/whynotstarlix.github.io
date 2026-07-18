namespace nspector;

public enum ListViewGroupState
{
	Normal = 0,
	Collapsed = 1,
	Hidden = 2,
	NoHeader = 4,
	Collapsible = 8,
	Focused = 0x10,
	Selected = 0x20,
	SubSeted = 0x40,
	SubSetLinkFocused = 0x80
}
