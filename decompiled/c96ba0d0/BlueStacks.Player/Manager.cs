using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player;

public class Manager
{
	private IntPtr handle = IntPtr.Zero;

	[DllImport("kernel32.dll")]
	private static extern bool CloseHandle(IntPtr handle);

	private Manager(IntPtr handle)
	{
		this.handle = handle;
	}

	private Manager()
	{
	}

	public static Manager Open()
	{
		return new Manager();
	}

	public Monitor Attach(uint id, Monitor.ExitHandler exitHandler)
	{
		if (!HDPlusModule.ManagerAttach(handle, id))
		{
			CommonError.ThrowLastWin32Error("Cannot attach to monitor " + id);
		}
		return new Monitor(handle, id, exitHandler);
	}

	public Monitor Attach(uint id, bool verbose)
	{
		return new Monitor(id, verbose);
	}

	public Monitor Attach(uint id, bool verbose, bool isMonAttach)
	{
		if (isMonAttach)
		{
			return new Monitor(id, verbose);
		}
		return new Monitor(id);
	}
}
