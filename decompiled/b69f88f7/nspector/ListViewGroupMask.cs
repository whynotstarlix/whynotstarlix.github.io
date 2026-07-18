namespace nspector;

public enum ListViewGroupMask
{
	None = 0,
	Header = 1,
	Footer = 2,
	State = 4,
	Align = 8,
	GroupId = 0x10,
	SubTitle = 0x100,
	Task = 0x200,
	DescriptionTop = 0x400,
	DescriptionBottom = 0x800,
	TitleImage = 0x1000,
	ExtendedImage = 0x2000,
	Items = 0x4000,
	Subset = 0x8000,
	SubsetItems = 0x10000
}
