using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace Kitsune_VM_Stub;

internal static class KitsuneGuard
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate bool VirtualProtectFn(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpOldProtect);

	internal const byte FLAG_ANTIDEBUG = 1;

	internal const byte FLAG_ANTIDUMP = 2;

	internal const byte FLAG_ANTIVM = 4;

	internal const byte FLAG_VMCRC = 8;

	private static byte _config = byte.MaxValue;

	internal static void Initialize(byte config)
	{
		_config = config;
	}

	internal static void CheckDebugger()
	{
		if ((_config & 1) == 0)
		{
			return;
		}
		try
		{
			if (Debugger.IsAttached)
			{
				Corrupt("dbg-attached");
			}
		}
		catch
		{
		}
		string[] array = new string[16]
		{
			X(">4)*#"),
			X(">4)*#\""),
			X(">4)*#?\""),
			X("\"ln>8="),
			X("\"ih>8="),
			X("566#>8="),
			X("-34>8="),
			X("3>;+"),
			X("3>;+ln"),
			X("3>;"),
			X("3>;ln"),
			X(">?n>5."),
			X("36)*#"),
			X(">5.*??1"),
			X("0/).>?957*36?"),
			X("95>?9(;91?(")
		};
		try
		{
			Process[] procs = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					procs = Process.GetProcesses();
				}
				catch
				{
				}
			});
			thread.IsBackground = true;
			thread.Start();
			if (thread.Join(4000) && procs != null)
			{
				Process[] array2 = procs;
				foreach (Process process in array2)
				{
					string text;
					try
					{
						text = process.ProcessName.ToLowerInvariant();
					}
					catch
					{
						continue;
					}
					string[] array3 = array;
					foreach (string text2 in array3)
					{
						if (text.Contains(text2))
						{
							Corrupt("dbgproc:" + text2);
						}
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			long timestamp = Stopwatch.GetTimestamp();
			int num3 = 0;
			for (int num4 = 0; num4 < 1000; num4++)
			{
				num3 ^= num4 * 3;
			}
			if ((double)(Stopwatch.GetTimestamp() - timestamp) * 1000.0 / (double)Stopwatch.Frequency > 2000.0)
			{
				Corrupt("timing:" + num3);
			}
		}
		catch
		{
		}
	}

	internal static void CheckVM()
	{
		if ((_config & 4) == 0)
		{
			return;
		}
		string[] array = new string[10]
		{
			X("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\f\u0017-;(?vz\u001349t\u0006\f\u0017-;(?z\u000e556)"),
			X("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\u0015(;96?\u0006\f3(./;6\u00185\"z\u001d/?).z\u001b>>3.354)"),
			X("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\u001739(5)5<.\u0006\f3(./;6z\u0017;9234?\u0006\u001d/?).\u0006\n;(;7?.?()"),
			X("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006\f\u00185\"\u001d/?)."),
			X("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006,72=<)"),
			X("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006,775/)?"),
			X("\u0012\u001b\b\u001e\r\u001b\b\u001f\u0006\u001b\u0019\n\u0013\u0006\u001e\t\u001e\u000e\u0006\f\u0018\u0015\u0002\u0005\u0005"),
			X("\u0012\u001b\b\u001e\r\u001b\b\u001f\u0006\u001b\u0019\n\u0013\u0006\u001c\u001b\u001e\u000e\u0006\f\u0018\u0015\u0002\u0005\u0005"),
			X("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\n;(;66?6)\u0006\u001952?(?49?"),
			X("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\v\u001f\u0017\u000f")
		};
		foreach (string name in array)
		{
			try
			{
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
				if (registryKey != null)
				{
					Corrupt("vmreg");
				}
			}
			catch
			{
			}
		}
		string[] array2 = new string[9]
		{
			X(",7.556)>"),
			X(",7-;(?.(;#"),
			X(",7-;(?/)?("),
			X(",85\")?(,39?"),
			X(",85\".(;#"),
			X("*(6\u0005.556)"),
			X("*(6\u000599"),
			X("+?7/w=;"),
			X(",>;=?4.")
		};
		try
		{
			Process[] procs2 = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					procs2 = Process.GetProcesses();
				}
				catch
				{
				}
			});
			thread.IsBackground = true;
			thread.Start();
			if (!thread.Join(4000) || procs2 == null)
			{
				return;
			}
			Process[] array3 = procs2;
			foreach (Process process in array3)
			{
				string text;
				try
				{
					text = process.ProcessName.ToLowerInvariant();
				}
				catch
				{
					continue;
				}
				array = array2;
				foreach (string value in array)
				{
					if (text.Contains(value))
					{
						Corrupt("vmproc");
					}
				}
			}
		}
		catch
		{
		}
	}

	internal static void AntiDump()
	{
		if ((_config & 2) == 0)
		{
			return;
		}
		try
		{
			IntPtr hINSTANCE = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule);
			if (hINSTANCE == IntPtr.Zero || hINSTANCE.ToInt64() == -1)
			{
				return;
			}
			IntPtr intPtr = FindExportInKernel32("VirtualProtect");
			if (!(intPtr == IntPtr.Zero))
			{
				VirtualProtectFn virtualProtectFn = (VirtualProtectFn)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(VirtualProtectFn));
				if (virtualProtectFn(hINSTANCE, (IntPtr)60, 4u, out var lpOldProtect))
				{
					Marshal.WriteInt32(hINSTANCE + 4, 0);
					Marshal.WriteInt64(hINSTANCE + 8, 0L);
					Marshal.WriteInt64(hINSTANCE + 16, 0L);
					Marshal.WriteInt64(hINSTANCE + 24, 0L);
					Marshal.WriteInt64(hINSTANCE + 32, 0L);
					Marshal.WriteInt64(hINSTANCE + 40, 0L);
					Marshal.WriteInt64(hINSTANCE + 48, 0L);
					Marshal.WriteInt32(hINSTANCE + 56, 0);
					virtualProtectFn(hINSTANCE, (IntPtr)60, lpOldProtect, out lpOldProtect);
				}
			}
		}
		catch
		{
		}
	}

	private static IntPtr FindExportInKernel32(string funcName)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				string text;
				try
				{
					text = Path.GetFileName(module.FileName).ToLowerInvariant();
				}
				catch
				{
					continue;
				}
				if (text == "kernel32.dll")
				{
					intPtr = module.BaseAddress;
					break;
				}
			}
		}
		catch
		{
		}
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		try
		{
			int num = Marshal.ReadInt32(intPtr + 60);
			IntPtr intPtr2 = intPtr + num;
			int num2 = ((Marshal.ReadInt16(intPtr2 + 24) == 523) ? 136 : 120);
			int num3 = Marshal.ReadInt32(intPtr2 + num2);
			if (num3 == 0)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr3 = intPtr + num3;
			int num4 = Marshal.ReadInt32(intPtr3 + 24);
			int num5 = Marshal.ReadInt32(intPtr3 + 28);
			int num6 = Marshal.ReadInt32(intPtr3 + 32);
			int num7 = Marshal.ReadInt32(intPtr3 + 36);
			IntPtr intPtr4 = intPtr + num5;
			IntPtr intPtr5 = intPtr + num6;
			IntPtr intPtr6 = intPtr + num7;
			byte[] bytes = Encoding.ASCII.GetBytes(funcName);
			for (int i = 0; i < num4; i++)
			{
				IntPtr intPtr7 = intPtr + Marshal.ReadInt32(intPtr5 + i * 4);
				bool flag = true;
				for (int j = 0; (j < bytes.Length) & flag; j++)
				{
					if (Marshal.ReadByte(intPtr7 + j) != bytes[j])
					{
						flag = false;
					}
				}
				if (flag && Marshal.ReadByte(intPtr7 + bytes.Length) == 0)
				{
					ushort num8 = (ushort)Marshal.ReadInt16(intPtr6 + i * 2);
					int num9 = Marshal.ReadInt32(intPtr4 + num8 * 4);
					return intPtr + num9;
				}
			}
		}
		catch
		{
		}
		return IntPtr.Zero;
	}

	private static bool BytesEqual(byte[] a, byte[] b)
	{
		if (a.Length != b.Length)
		{
			return false;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	private static void Wipe(byte[][] chunks)
	{
		Random random = new Random();
		for (int i = 0; i < chunks.Length; i++)
		{
			if (chunks[i] != null)
			{
				random.NextBytes(chunks[i]);
				chunks[i] = null;
			}
		}
	}

	private static string X(string s)
	{
		char[] array = new char[s.Length];
		for (int i = 0; i < s.Length; i++)
		{
			array[i] = (char)(s[i] ^ 0x5A);
		}
		return new string(array);
	}

	internal static void Corrupt(string hint)
	{
		string[] array = new string[3];
		try
		{
			array[0] = Path.Combine(Path.GetTempPath(), "kitsune_corrupt.log");
		}
		catch
		{
		}
		try
		{
			array[1] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kitsune_corrupt.log");
		}
		catch
		{
		}
		try
		{
			array[2] = Path.Combine(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\").Substring(0, Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").LastIndexOfAny(new char[2] { '\\', '/' })), "kitsune_corrupt.log");
		}
		catch
		{
		}
		string contents = "Corrupt: " + hint + "\r\n" + new StackTrace().ToString();
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				if (text != null)
				{
					File.WriteAllText(text, contents);
					break;
				}
			}
			catch
			{
			}
		}
		try
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		catch
		{
		}
		Environment.Exit(-1);
	}
}
