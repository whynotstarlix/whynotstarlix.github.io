using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nspector.Native.WINAPI;

[ComImport]
[Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ITaskbarList3
{
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void HrInit();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void AddTab([In] IntPtr hwnd);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void DeleteTab([In] IntPtr hwnd);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void ActivateTab([In] IntPtr hwnd);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetActiveAlt([In] IntPtr hwnd);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void MarkFullscreenWindow([In] IntPtr hwnd, [In][MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetProgressValue([In] IntPtr hwnd, [In] ulong ullCompleted, [In] ulong ullTotal);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetProgressState([In] IntPtr hwnd, [In] TBPFLAG tbpFlags);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void RegisterTab([In] IntPtr hwndTab, [In] IntPtr hwndMDI);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void UnregisterTab([In] IntPtr hwndTab);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetTabOrder([In] IntPtr hwndTab, [In] IntPtr hwndInsertBefore);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetTabActive([In] IntPtr hwndTab, [In] IntPtr hwndMDI, [In] TBATFLAG tbatFlags);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void ThumbBarAddButtons([In] IntPtr hwnd, [In] uint cButtons, [In] IntPtr pButton);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void ThumbBarUpdateButtons([In] IntPtr hwnd, [In] uint cButtons, [In] IntPtr pButton);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void ThumbBarSetImageList([In] IntPtr hwnd, [In] IntPtr himl);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetOverlayIcon([In] IntPtr hwnd, [In] IntPtr hIcon, [In][MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetThumbnailTooltip([In] IntPtr hwnd, [In][MarshalAs(UnmanagedType.LPWStr)] string pszTip);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	void SetThumbnailClip([In] IntPtr hwnd, [In] IntPtr prcClip);
}
