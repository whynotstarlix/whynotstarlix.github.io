using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32.SafeHandles;

namespace BlueStacks.Player;

internal class VMCommand
{
	public delegate void ChunkHandler(string chunk);

	public delegate void LineHandler(string line);

	private const string NATIVE_DLL = "HD-VMCommand-Native.dll";

	private Random random = new Random();

	private SafeFileHandle vmHandle;

	private uint unitId;

	private LineHandler userOutputHandler;

	private LineHandler userErrorHandler;

	private StringBuilder outputBuffer = new StringBuilder();

	private StringBuilder errorBuffer = new StringBuilder();

	[DllImport("HD-VMCommand-Native.dll", SetLastError = true)]
	private static extern SafeFileHandle CommandAttach(uint vmId, uint unitId);

	[DllImport("HD-VMCommand-Native.dll")]
	private static extern int CommandPing(SafeFileHandle vmHandle, uint unitId);

	[DllImport("HD-VMCommand-Native.dll")]
	private static extern int CommandRun(SafeFileHandle vmHandle, uint unitId, int argc, string[] argv, ChunkHandler outHandler, ChunkHandler errHandler, ref int exitCode);

	[DllImport("HD-VMCommand-Native.dll")]
	private static extern int CommandKill(SafeFileHandle vmHandle, uint unitId);

	public void Attach(string vmName)
	{
		uint vmId = MonitorLocator.Lookup(vmName);
		unitId = (uint)random.Next();
		vmHandle = CommandAttach(vmId, unitId);
		if (vmHandle.IsInvalid)
		{
			throw new ApplicationException("Cannot attach to monitor: " + Marshal.GetLastWin32Error());
		}
	}

	public void SetOutputHandler(LineHandler handler)
	{
		userOutputHandler = handler;
	}

	public void SetErrorHandler(LineHandler handler)
	{
		userErrorHandler = handler;
	}

	public int Run(string[] argv)
	{
		int exitCode = 0;
		int num = CommandPing(vmHandle, unitId);
		if (num != 0)
		{
			throw new ApplicationException("Cannot ping VM", new Win32Exception(num));
		}
		num = CommandRun(vmHandle, unitId, argv.Length, argv, OutputHandler, ErrorHandler, ref exitCode);
		if (num != 0)
		{
			throw new ApplicationException("Cannot run VM command", new Win32Exception(num));
		}
		if (outputBuffer.Length > 0 && userOutputHandler != null)
		{
			userOutputHandler(outputBuffer.ToString());
		}
		if (errorBuffer.Length > 0 && userErrorHandler != null)
		{
			userErrorHandler(errorBuffer.ToString());
		}
		return exitCode;
	}

	private void OutputHandler(string chunk)
	{
		CommonHandler(chunk, outputBuffer, userOutputHandler);
	}

	private void ErrorHandler(string chunk)
	{
		CommonHandler(chunk, errorBuffer, userErrorHandler);
	}

	private static void CommonHandler(string chunk, StringBuilder sb, LineHandler handler)
	{
		sb.Append(chunk);
		string[] array = sb.ToString().Split(new char[1] { '\n' });
		if (array.Length >= 2)
		{
			for (int i = 0; i < array.Length - 1; i++)
			{
				handler?.Invoke(array[i]);
			}
			sb.Remove(0, sb.Length);
			sb.Append(array[^1]);
		}
	}

	public void Kill()
	{
		CommandKill(vmHandle, unitId);
	}
}
