using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace nspector.Native.WINAPI;

internal class ShellLink : IDisposable
{
	[ComImport]
	[Guid("0000010C-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IPersist
	{
		[PreserveSig]
		void GetClassID(out Guid pClassID);
	}

	[ComImport]
	[Guid("0000010B-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IPersistFile
	{
		[PreserveSig]
		void GetClassID(out Guid pClassID);

		void IsDirty();

		void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

		void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

		void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

		void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
	}

	[ComImport]
	[Guid("000214EE-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IShellLinkA
	{
		void GetPath([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxPath, ref _WIN32_FIND_DATAA pfd, uint fFlags);

		void GetIDList(out IntPtr ppidl);

		void SetIDList(IntPtr pidl);

		void GetDescription([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cchMaxName);

		void SetDescription([MarshalAs(UnmanagedType.LPStr)] string pszName);

		void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir, int cchMaxPath);

		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string pszDir);

		void GetArguments([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, int cchMaxPath);

		void SetArguments([MarshalAs(UnmanagedType.LPStr)] string pszArgs);

		void GetHotkey(out short pwHotkey);

		void SetHotkey(short pwHotkey);

		void GetShowCmd(out uint piShowCmd);

		void SetShowCmd(uint piShowCmd);

		void GetIconLocation([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPStr)] string pszIconPath, int iIcon);

		void SetRelativePath([MarshalAs(UnmanagedType.LPStr)] string pszPathRel, uint dwReserved);

		void Resolve(IntPtr hWnd, uint fFlags);

		void SetPath([MarshalAs(UnmanagedType.LPStr)] string pszFile);
	}

	[ComImport]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	private interface IShellLinkW
	{
		void GetPath([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, ref _WIN32_FIND_DATAW pfd, uint fFlags);

		void GetIDList(out IntPtr ppidl);

		void SetIDList(IntPtr pidl);

		void GetDescription([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);

		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

		void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

		void GetArguments([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

		void GetHotkey(out short pwHotkey);

		void SetHotkey(short pwHotkey);

		void GetShowCmd(out uint piShowCmd);

		void SetShowCmd(uint piShowCmd);

		void GetIconLocation([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

		void Resolve(IntPtr hWnd, uint fFlags);

		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	[ClassInterface(ClassInterfaceType.None)]
	private class CShellLink
	{
	}

	private enum EShellLinkGP : uint
	{
		SLGP_SHORTPATH = 1u,
		SLGP_UNCPRIORITY
	}

	[Flags]
	private enum EShowWindowFlags : uint
	{
		SW_HIDE = 0u,
		SW_SHOWNORMAL = 1u,
		SW_NORMAL = 1u,
		SW_SHOWMINIMIZED = 2u,
		SW_SHOWMAXIMIZED = 3u,
		SW_MAXIMIZE = 3u,
		SW_SHOWNOACTIVATE = 4u,
		SW_SHOW = 5u,
		SW_MINIMIZE = 6u,
		SW_SHOWMINNOACTIVE = 7u,
		SW_SHOWNA = 8u,
		SW_RESTORE = 9u,
		SW_SHOWDEFAULT = 0xAu,
		SW_MAX = 0xAu
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
	private struct _WIN32_FIND_DATAW
	{
		internal uint dwFileAttributes;

		internal _FILETIME ftCreationTime;

		internal _FILETIME ftLastAccessTime;

		internal _FILETIME ftLastWriteTime;

		internal uint nFileSizeHigh;

		internal uint nFileSizeLow;

		internal uint dwReserved0;

		internal uint dwReserved1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string cFileName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		internal string cAlternateFileName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct _WIN32_FIND_DATAA
	{
		internal uint dwFileAttributes;

		internal _FILETIME ftCreationTime;

		internal _FILETIME ftLastAccessTime;

		internal _FILETIME ftLastWriteTime;

		internal uint nFileSizeHigh;

		internal uint nFileSizeLow;

		internal uint dwReserved0;

		internal uint dwReserved1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		internal string cFileName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		internal string cAlternateFileName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct _FILETIME
	{
		internal uint dwLowDateTime;

		internal uint dwHighDateTime;
	}

	private class NativeMethods
	{
		[DllImport("Shell32", CharSet = CharSet.Auto)]
		internal static extern int ExtractIconEx([MarshalAs(UnmanagedType.LPTStr)] string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);

		[DllImport("user32")]
		internal static extern int DestroyIcon(IntPtr hIcon);
	}

	[Flags]
	internal enum EShellLinkResolveFlags : uint
	{
		SLR_ANY_MATCH = 2u,
		SLR_INVOKE_MSI = 0x80u,
		SLR_NOLINKINFO = 0x40u,
		SLR_NO_UI = 1u,
		SLR_NO_UI_WITH_MSG_PUMP = 0x101u,
		SLR_NOUPDATE = 8u,
		SLR_NOSEARCH = 0x10u,
		SLR_NOTRACK = 0x20u,
		SLR_UPDATE = 4u
	}

	internal enum LinkDisplayMode : uint
	{
		edmNormal = 1u,
		edmMinimized = 7u,
		edmMaximized = 3u
	}

	private IShellLinkW linkW;

	private IShellLinkA linkA;

	private string shortcutFile = "";

	internal string ShortCutFile
	{
		get
		{
			return shortcutFile;
		}
		set
		{
			shortcutFile = value;
		}
	}

	internal string IconPath
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			return stringBuilder.ToString();
		}
		set
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			if (linkA == null)
			{
				linkW.SetIconLocation(value, piIcon);
			}
			else
			{
				linkA.SetIconLocation(value, piIcon);
			}
		}
	}

	internal int IconIndex
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			return piIcon;
		}
		set
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			int piIcon = 0;
			if (linkA == null)
			{
				linkW.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			else
			{
				linkA.GetIconLocation(stringBuilder, stringBuilder.Capacity, out piIcon);
			}
			if (linkA == null)
			{
				linkW.SetIconLocation(stringBuilder.ToString(), value);
			}
			else
			{
				linkA.SetIconLocation(stringBuilder.ToString(), value);
			}
		}
	}

	internal string Target
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				_WIN32_FIND_DATAW pfd = default(_WIN32_FIND_DATAW);
				linkW.GetPath(stringBuilder, stringBuilder.Capacity, ref pfd, 2u);
			}
			else
			{
				_WIN32_FIND_DATAA pfd2 = default(_WIN32_FIND_DATAA);
				linkA.GetPath(stringBuilder, stringBuilder.Capacity, ref pfd2, 2u);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetPath(value);
			}
			else
			{
				linkA.SetPath(value);
			}
		}
	}

	internal string WorkingDirectory
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				linkW.GetWorkingDirectory(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetWorkingDirectory(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetWorkingDirectory(value);
			}
			else
			{
				linkA.SetWorkingDirectory(value);
			}
		}
	}

	internal string Description
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(1024, 1024);
			if (linkA == null)
			{
				linkW.GetDescription(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetDescription(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetDescription(value);
			}
			else
			{
				linkA.SetDescription(value);
			}
		}
	}

	internal string Arguments
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260, 260);
			if (linkA == null)
			{
				linkW.GetArguments(stringBuilder, stringBuilder.Capacity);
			}
			else
			{
				linkA.GetArguments(stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetArguments(value);
			}
			else
			{
				linkA.SetArguments(value);
			}
		}
	}

	internal LinkDisplayMode DisplayMode
	{
		get
		{
			uint piShowCmd = 0u;
			if (linkA == null)
			{
				linkW.GetShowCmd(out piShowCmd);
			}
			else
			{
				linkA.GetShowCmd(out piShowCmd);
			}
			return (LinkDisplayMode)piShowCmd;
		}
		set
		{
			if (linkA == null)
			{
				linkW.SetShowCmd((uint)value);
			}
			else
			{
				linkA.SetShowCmd((uint)value);
			}
		}
	}

	internal Keys HotKey
	{
		get
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			short pwHotkey = 0;
			if (linkA == null)
			{
				linkW.GetHotkey(out pwHotkey);
			}
			else
			{
				linkA.GetHotkey(out pwHotkey);
			}
			return (Keys)pwHotkey;
		}
		set
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (linkA == null)
			{
				linkW.SetHotkey((short)value);
			}
			else
			{
				linkA.SetHotkey((short)value);
			}
		}
	}

	internal ShellLink()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			linkW = (IShellLinkW)new CShellLink();
		}
		else
		{
			linkA = (IShellLinkA)new CShellLink();
		}
	}

	internal ShellLink(string linkFile)
		: this()
	{
		Open(linkFile);
	}

	~ShellLink()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (linkW != null)
		{
			Marshal.ReleaseComObject(linkW);
			linkW = null;
		}
		if (linkA != null)
		{
			Marshal.ReleaseComObject(linkA);
			linkA = null;
		}
	}

	internal void Save()
	{
		Save(shortcutFile);
	}

	internal void Save(string linkFile)
	{
		if (linkA == null)
		{
			((IPersistFile)linkW).Save(linkFile, fRemember: true);
			shortcutFile = linkFile;
		}
		else
		{
			((IPersistFile)linkA).Save(linkFile, fRemember: true);
			shortcutFile = linkFile;
		}
	}

	internal void Open(string linkFile)
	{
		Open(linkFile, IntPtr.Zero, EShellLinkResolveFlags.SLR_ANY_MATCH | EShellLinkResolveFlags.SLR_NO_UI, 1);
	}

	internal void Open(string linkFile, IntPtr hWnd, EShellLinkResolveFlags resolveFlags)
	{
		Open(linkFile, hWnd, resolveFlags, 1);
	}

	internal void Open(string linkFile, IntPtr hWnd, EShellLinkResolveFlags resolveFlags, ushort timeOut)
	{
		uint fFlags = (((resolveFlags & EShellLinkResolveFlags.SLR_NO_UI) != EShellLinkResolveFlags.SLR_NO_UI) ? ((uint)resolveFlags) : ((uint)resolveFlags | (uint)(timeOut << 16)));
		if (linkA == null)
		{
			((IPersistFile)linkW).Load(linkFile, 0u);
			linkW.Resolve(hWnd, fFlags);
			shortcutFile = linkFile;
		}
		else
		{
			((IPersistFile)linkA).Load(linkFile, 0u);
			linkA.Resolve(hWnd, fFlags);
			shortcutFile = linkFile;
		}
	}
}
