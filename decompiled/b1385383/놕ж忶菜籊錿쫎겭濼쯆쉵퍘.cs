using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

[쩍飄갚픆뷓딾潽뻮]
internal static class 놕ж忶菜籊錿쫎겭濼쯆쉵퍘
{
	private delegate bool 찖牋舥塿鏋듵鬗咜(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpOldProtect);

	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public Process[] procs;

		internal void _003CCheckDebugger_003Eb__0()
		{
			try
			{
				procs = Process.GetProcesses();
			}
			catch
			{
			}
		}

		private static int GetBuffer39()
		{
			int num = 7;
			do
			{
				num++;
			}
			while (num < 421);
			return 0;
		}

		private static int GetEntry61()
		{
			int num = 18;
			do
			{
				num += 7;
			}
			while (num < 803);
			return 0;
		}

		private static bool UpdateBlock33()
		{
			int num = -1640531527;
			num = (num ^ 0x476BE8A0) << 16;
			num = (num ^ 0x580138DB) << 17;
			num = (num ^ 0x881A771) << 12;
			num = (num ^ 0x79D267BC) << 5;
			return false;
		}
	}

	[글빩嘘谕睵箏救썵鎧奊醁꼬迒嶭쁺獎꺭쭒永]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public Process[] procs2;

		internal void _003CCheckVM_003Eb__0()
		{
			try
			{
				procs2 = Process.GetProcesses();
			}
			catch
			{
			}
		}

		private static void ValidateHandle51()
		{
			int num = -1640531527;
			num = (num ^ 0x47FA5300) << 8;
			num = (num ^ 0x1955A0C9) << 2;
			num = (num ^ 0x6FC51C0) << 30;
			num = (num ^ 0x111C1FA9) << 9;
		}
	}

	internal const byte 瑼쁘衏昪洺燗修睐鱯爍뛌쫘 = 1;

	internal const byte 従밷蔅햅縊邲儗靕困 = 2;

	internal const byte 玠帧췕虁煼泈哙쩪纥蘜굉쏤滨傏爀쫷凁 = 4;

	internal const byte 軁늆킝뭄娚亯惧웻愺貇扥묲繰뒌橽唲憟哴眉쌹 = 8;

	private static byte 坸钇罆磮薃訟獒踖腧끶誶询勰善簎뼝退 = byte.MaxValue;

	internal static void 녖웁걯綪蜫쨊笅묡儴庅唭榏阦愉(byte config)
	{
		坸钇罆磮薃訟獒踖腧끶誶询勰善簎뼝退 = config;
	}

	internal static void 螅횸鮾쿣셸둢艐韞繲氖刀()
	{
		if ((坸钇罆磮薃訟獒踖腧끶誶询勰善簎뼝退 & 1) == 0)
		{
			return;
		}
		try
		{
			if (Debugger.IsAttached)
			{
				瞾矾匷팗擼捇랋蔞묡岶獠("dbg-attached");
			}
		}
		catch
		{
		}
		string[] array = new string[16]
		{
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(">4)*#"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(">4)*#\""),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(">4)*#?\""),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\"ln>8="),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\"ih>8="),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("566#>8="),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("-34>8="),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("3>;+"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("3>;+ln"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("3>;"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("3>;ln"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(">?n>5."),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("36)*#"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(">5.*??1"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("0/).>?957*36?"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("95>?9(;91?(")
		};
		try
		{
			_003C_003Ec__DisplayClass6_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass6_0();
			CS_0024_003C_003E8__locals4.procs = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					CS_0024_003C_003E8__locals4.procs = Process.GetProcesses();
				}
				catch
				{
				}
			});
			thread.IsBackground = true;
			thread.Start();
			if (thread.Join(4000) && CS_0024_003C_003E8__locals4.procs != null)
			{
				Process[] procs = CS_0024_003C_003E8__locals4.procs;
				foreach (Process process in procs)
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
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (text.Contains(text2))
						{
							瞾矾匷팗擼捇랋蔞묡岶獠("dbgproc:" + text2);
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
				瞾矾匷팗擼捇랋蔞묡岶獠("timing:" + num3);
			}
		}
		catch
		{
		}
	}

	internal static void 屸屽浯唾뾤跭慷똟깦뭨낿狖섫()
	{
		if ((坸钇罆磮薃訟獒踖腧끶誶询勰善簎뼝退 & 4) == 0)
		{
			return;
		}
		string[] array = new string[10]
		{
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\f\u0017-;(?vz\u001349t\u0006\f\u0017-;(?z\u000e556)"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\u0015(;96?\u0006\f3(./;6\u00185\"z\u001d/?).z\u001b>>3.354)"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\u001739(5)5<.\u0006\f3(./;6z\u0017;9234?\u0006\u001d/?).\u0006\n;(;7?.?()"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006\f\u00185\"\u001d/?)."),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006,72=<)"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0003\t\u000e\u001f\u0017\u0006\u0019/((?4.\u001954.(56\t?.\u0006\t?(,39?)\u0006,775/)?"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\u0012\u001b\b\u001e\r\u001b\b\u001f\u0006\u001b\u0019\n\u0013\u0006\u001e\t\u001e\u000e\u0006\f\u0018\u0015\u0002\u0005\u0005"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\u0012\u001b\b\u001e\r\u001b\b\u001f\u0006\u001b\u0019\n\u0013\u0006\u001c\u001b\u001e\u000e\u0006\f\u0018\u0015\u0002\u0005\u0005"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\n;(;66?6)\u0006\u001952?(?49?"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("\t\u0015\u001c\u000e\r\u001b\b\u001f\u0006\v\u001f\u0017\u000f")
		};
		foreach (string name in array)
		{
			try
			{
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
				if (registryKey != null)
				{
					瞾矾匷팗擼捇랋蔞묡岶獠("vmreg");
				}
			}
			catch
			{
			}
		}
		string[] array2 = new string[9]
		{
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",7.556)>"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",7-;(?.(;#"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",7-;(?/)?("),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",85\")?(,39?"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",85\".(;#"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("*(6\u0005.556)"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("*(6\u000599"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡("+?7/w=;"),
			햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(",>;=?4.")
		};
		try
		{
			_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass7_0();
			CS_0024_003C_003E8__locals4.procs2 = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					CS_0024_003C_003E8__locals4.procs2 = Process.GetProcesses();
				}
				catch
				{
				}
			});
			thread.IsBackground = true;
			thread.Start();
			if (!thread.Join(4000) || CS_0024_003C_003E8__locals4.procs2 == null)
			{
				return;
			}
			Process[] procs = CS_0024_003C_003E8__locals4.procs2;
			foreach (Process process in procs)
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
						瞾矾匷팗擼捇랋蔞묡岶獠("vmproc");
					}
				}
			}
		}
		catch
		{
		}
	}

	internal static void 邚굒옵垺朽뷇缦鵂旯烧烏쨶廐뷁濌澩滃庅舜觇()
	{
		if ((坸钇罆磮薃訟獒踖腧끶誶询勰善簎뼝退 & 2) == 0)
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
			IntPtr intPtr = 齪뭩嘷洬덅膠芎鞏꿫狩痘("VirtualProtect");
			if (!(intPtr == IntPtr.Zero))
			{
				찖牋舥塿鏋듵鬗咜 찖牋舥塿鏋듵鬗咜2 = (찖牋舥塿鏋듵鬗咜)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(찖牋舥塿鏋듵鬗咜));
				if (찖牋舥塿鏋듵鬗咜2(hINSTANCE, (IntPtr)60, 4u, out var lpOldProtect))
				{
					Marshal.WriteInt32(hINSTANCE + 4, 0);
					Marshal.WriteInt64(hINSTANCE + 8, 0L);
					Marshal.WriteInt64(hINSTANCE + 16, 0L);
					Marshal.WriteInt64(hINSTANCE + 24, 0L);
					Marshal.WriteInt64(hINSTANCE + 32, 0L);
					Marshal.WriteInt64(hINSTANCE + 40, 0L);
					Marshal.WriteInt64(hINSTANCE + 48, 0L);
					Marshal.WriteInt32(hINSTANCE + 56, 0);
					찖牋舥塿鏋듵鬗咜2(hINSTANCE, (IntPtr)60, lpOldProtect, out lpOldProtect);
				}
			}
		}
		catch
		{
		}
	}

	private static IntPtr 齪뭩嘷洬덅膠芎鞏꿫狩痘(string funcName)
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

	private static bool 씪撏료땶玩凉毝仕誯(byte[] a, byte[] b)
	{
		if (a.Length != b.Length)
		{
			return (byte)(0x245DB96 ^ (~-5821 + 1 + -1) ^ 0x2B2B0CA8 ^ ((695132425 + -(0x7F6D113B ^ 0x7F6D1077)) ^ 0x1E3F)) != 0;
		}
		for (int num = 0x7687F9CD ^ 0x7687F9CD; num < a.Length; num -= -(0x3AF0022D ^ ((0x10F45AF6 ^ 0x10F45DEC) + -((13454 + -13003) & -1)) ^ ((2059258264 + -574) ^ (~-6271 + 1 + -1)) ^ (0x404DCDC8 ^ ((0x2ACC35A5 ^ 0x2ACC2CA8) + -886))))
		{
			if (a[num] != b[num])
			{
				return (byte)(((0x51B6CA79 ^ 0x9AE) + -(0x4D47 ^ 0x1BB2)) ^ 0x51B66CE2) != 0;
			}
		}
		return true;
	}

	private static void 퐫밓촒簎筙蜚羃赫錁졞砫땤죓즍볮숱讛糮퐼劣(byte[][] chunks)
	{
		Random random = new Random();
		for (int num = 0x7923A19F ^ (((2032404995 + -19614) & -1) ^ 0x1CFA); num < chunks.Length; num -= -(0x3027AA28 ^ 0x3027AA29))
		{
			if (chunks[num] != null)
			{
				random.NextBytes(chunks[num]);
				chunks[num] = null;
			}
		}
	}

	private static string 햢튚뭾煉駹躣釖튾妬挰蟣롲딆뻡(string s)
	{
		char[] array = new char[s.Length];
		for (int num = 0x40F4FD26 ^ (40373 + -23604 + -9760) ^ 0x40F4E647; num < s.Length; num -= -(0x1BC33E91 ^ (((0x7708C6CF ^ 0x6CCBE23D) + -(0x1B2CA6F5 ^ 0x1B2CA7C8)) ^ 0x1D25)))
		{
			array[num] = (char)(s[num] ^ (0x1BA13885 ^ (8373 + -(-763 ^ -1)) ^ (((1352528655 + -1482) & -1) ^ 0xE13) ^ 0x4B3CC632));
		}
		return new string(array);
	}

	internal static void 瞾矾匷팗擼捇랋蔞묡岶獠(string hint)
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

	private static void 퀫褛醧욓刼蓼腑혭서庄垵뤄퀚앐鉑虅눟쟬풕()
	{
		bool flag = true;
		if ((0x703B3595 & 0x341AD0D7) == 0)
		{
			flag = false;
		}
		_ = 0;
	}

	private static bool 垜语謺悎皧丕않睑欇蹃()
	{
		int num = -1640531527;
		num = (num ^ 0x1F854188) << 2;
		num = (num ^ 0x2E211C8C) << 23;
		num = (num ^ 0xB022805) << 8;
		num = (num ^ 0x442B9677) << 10;
		num = (num ^ 0x7570BCF6) << 9;
		return false;
	}

	private static bool 憃玊齤饏詩鍿瑀芗()
	{
		int num = 27;
		do
		{
			num += 5;
		}
		while (num < 344);
		return false;
	}
}
