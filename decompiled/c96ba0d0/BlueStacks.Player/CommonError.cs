using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlueStacks.Player;

public class CommonError
{
	public static void ThrowLastWin32Error(string msg)
	{
		throw new SystemException(msg, new Win32Exception(Marshal.GetLastWin32Error()));
	}
}
