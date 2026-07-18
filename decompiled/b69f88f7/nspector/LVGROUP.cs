using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace nspector;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[Description("LVGROUP StructureUsed to set and retrieve groups.")]
public struct LVGROUP
{
	[Description("Size of this structure, in bytes.")]
	public int CbSize;

	[Description("Mask that specifies which members of the structure are valid input. One or more of the following values:LVGF_NONE No other items are valid.")]
	public ListViewGroupMask Mask;

	[MarshalAs(UnmanagedType.LPWStr)]
	[Description("Pointer to a null-terminated string that contains the header text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the header text.")]
	public string PszHeader;

	[Description("Size in TCHARs of the buffer pointed to by the pszHeader member. If the structure is not receiving information about a group, this member is ignored.")]
	public int CchHeader;

	[MarshalAs(UnmanagedType.LPWStr)]
	[Description("Pointer to a null-terminated string that contains the footer text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the footer text.")]
	public string PszFooter;

	[Description("Size in TCHARs of the buffer pointed to by the pszFooter member. If the structure is not receiving information about a group, this member is ignored.")]
	public int CchFooter;

	[Description("ID of the group.")]
	public int IGroupId;

	[Description("Mask used with LVM_GETGROUPINFO (Microsoft Windows XP and Windows Vista) and LVM_SETGROUPINFO (Windows Vista only) to specify which flags in the state value are being retrieved or set.")]
	public int StateMask;

	[Description("Flag that can have one of the following values:LVGS_NORMAL Groups are expanded, the group name is displayed, and all items in the group are displayed.")]
	public ListViewGroupState State;

	[Description("Indicates the alignment of the header or footer text for the group. It can have one or more of the following values. Use one of the header flags. Footer flags are optional. Windows XP: Footer flags are reserved.LVGA_FOOTER_CENTERReserved.")]
	public uint UAlign;

	[Description("Windows Vista. Pointer to a null-terminated string that contains the subtitle text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subtitle text. This element is drawn under the header text.")]
	public IntPtr PszSubtitle;

	[Description("Windows Vista. Size, in TCHARs, of the buffer pointed to by the pszSubtitle member. If the structure is not receiving information about a group, this member is ignored.")]
	public uint CchSubtitle;

	[MarshalAs(UnmanagedType.LPWStr)]
	[Description("Windows Vista. Pointer to a null-terminated string that contains the text for a task link when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the task text. This item is drawn right-aligned opposite the header text. When clicked by the user, the task link generates an LVN_LINKCLICK notification.")]
	public string PszTask;

	[Description("Windows Vista. Size in TCHARs of the buffer pointed to by the pszTask member. If the structure is not receiving information about a group, this member is ignored.")]
	public uint CchTask;

	[MarshalAs(UnmanagedType.LPWStr)]
	[Description("Windows Vista. Pointer to a null-terminated string that contains the top description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the top description text. This item is drawn opposite the title image when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.")]
	public string PszDescriptionTop;

	[Description("Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionTop member. If the structure is not receiving information about a group, this member is ignored.")]
	public uint CchDescriptionTop;

	[MarshalAs(UnmanagedType.LPWStr)]
	[Description("Windows Vista. Pointer to a null-terminated string that contains the bottom description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the bottom description text. This item is drawn under the top description text when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.")]
	public string PszDescriptionBottom;

	[Description("Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionBottom member. If the structure is not receiving information about a group, this member is ignored.")]
	public uint CchDescriptionBottom;

	[Description("Windows Vista. Index of the title image in the control imagelist.")]
	public int ITitleImage;

	[Description("Windows Vista. Index of the extended image in the control imagelist.")]
	public int IExtendedImage;

	[Description("Windows Vista. Read-only.")]
	public int IFirstItem;

	[Description("Windows Vista. Read-only in non-owner data mode.")]
	public IntPtr CItems;

	[Description("Windows Vista. NULL if group is not a subset. Pointer to a null-terminated string that contains the subset title text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subset title text.")]
	public IntPtr PszSubsetTitle;

	[Description("Windows Vista. Size in TCHARs of the buffer pointed to by the pszSubsetTitle member. If the structure is not receiving information about a group, this member is ignored.")]
	public IntPtr CchSubsetTitle;
}
