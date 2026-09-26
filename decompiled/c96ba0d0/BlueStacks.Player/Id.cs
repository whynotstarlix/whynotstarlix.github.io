using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32;

namespace BlueStacks.Player;

internal class Id
{
	private const uint HKEY_LOCAL_MACHINE = 2147483650u;

	private const uint KEY_READ = 131097u;

	private const uint KEY_WOW64_64KEY = 256u;

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, uint ulOptions, uint samDesired, ref UIntPtr phkResult);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, ref RegistryValueKind lpType, IntPtr lpData, ref int lpcbData);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern int RegCloseKey(UIntPtr hKey);

	public static string GenerateID()
	{
		string text = "";
		bool flag = true;
		try
		{
			byte[] bytes = new byte[4]
			{
				Convert.ToByte('w'),
				Convert.ToByte('m'),
				Convert.ToByte('i'),
				Convert.ToByte('c')
			};
			string text2 = Encoding.UTF8.GetString(bytes);
			byte[] bytes2 = new byte[18]
			{
				Convert.ToByte('c'),
				Convert.ToByte('s'),
				Convert.ToByte('p'),
				Convert.ToByte('r'),
				Convert.ToByte('o'),
				Convert.ToByte('d'),
				Convert.ToByte('u'),
				Convert.ToByte('c'),
				Convert.ToByte('t'),
				Convert.ToByte(' '),
				Convert.ToByte('g'),
				Convert.ToByte('e'),
				Convert.ToByte('t'),
				Convert.ToByte(' '),
				Convert.ToByte('U'),
				Convert.ToByte('U'),
				Convert.ToByte('I'),
				Convert.ToByte('D')
			};
			string text3 = Encoding.UTF8.GetString(bytes2);
			byte[] bytes3 = new byte[4]
			{
				Convert.ToByte('U'),
				Convert.ToByte('U'),
				Convert.ToByte('I'),
				Convert.ToByte('D')
			};
			string oldValue = Encoding.UTF8.GetString(bytes3);
			text = Utils.RunCmdNoLog(text2, text3, 3000);
			text = text.Replace(oldValue, "").Trim();
			text = text.Replace("\n", "");
			text = text.Replace("\r", "");
			text = text.Replace("\t", "");
			text = text.Replace(" ", "");
			string text4 = text;
			foreach (char c in text4)
			{
				if (c != 'F' && c != '-')
				{
					flag = false;
					break;
				}
			}
		}
		catch
		{
			Logger.Error("Unable to query intended string");
		}
		if (text != string.Empty && flag)
		{
			try
			{
				byte[] bytes4 = new byte[4]
				{
					Convert.ToByte('w'),
					Convert.ToByte('m'),
					Convert.ToByte('i'),
					Convert.ToByte('c')
				};
				string text5 = Encoding.UTF8.GetString(bytes4);
				byte[] bytes5 = new byte[21]
				{
					Convert.ToByte('b'),
					Convert.ToByte('i'),
					Convert.ToByte('o'),
					Convert.ToByte('s'),
					Convert.ToByte(' '),
					Convert.ToByte('g'),
					Convert.ToByte('e'),
					Convert.ToByte('t'),
					Convert.ToByte(' '),
					Convert.ToByte('s'),
					Convert.ToByte('e'),
					Convert.ToByte('r'),
					Convert.ToByte('i'),
					Convert.ToByte('a'),
					Convert.ToByte('l'),
					Convert.ToByte('n'),
					Convert.ToByte('u'),
					Convert.ToByte('m'),
					Convert.ToByte('b'),
					Convert.ToByte('e'),
					Convert.ToByte('r')
				};
				string text6 = Encoding.UTF8.GetString(bytes5);
				byte[] bytes6 = new byte[12]
				{
					Convert.ToByte('S'),
					Convert.ToByte('e'),
					Convert.ToByte('r'),
					Convert.ToByte('i'),
					Convert.ToByte('a'),
					Convert.ToByte('l'),
					Convert.ToByte('N'),
					Convert.ToByte('u'),
					Convert.ToByte('m'),
					Convert.ToByte('b'),
					Convert.ToByte('e'),
					Convert.ToByte('r')
				};
				string oldValue2 = Encoding.UTF8.GetString(bytes6);
				text = Utils.RunCmdNoLog(text5, text6, 3000);
				text = text.Replace(oldValue2, "").Trim();
				text = text.Replace("\n", "");
				text = text.Replace("\r", "");
				text = text.Replace("\t", "");
				text = text.Replace(" ", "");
			}
			catch
			{
				Logger.Error("Unable to query another intended string");
			}
		}
		if (text == string.Empty)
		{
			text = FallBackID();
		}
		return text;
	}

	private static string FallBackID()
	{
		IntPtr hKey = (IntPtr)(-2147483646);
		UIntPtr phkResult = UIntPtr.Zero;
		byte[] bytes = new byte[31]
		{
			Convert.ToByte('S'),
			Convert.ToByte('o'),
			Convert.ToByte('f'),
			Convert.ToByte('t'),
			Convert.ToByte('w'),
			Convert.ToByte('a'),
			Convert.ToByte('r'),
			Convert.ToByte('e'),
			Convert.ToByte('\\'),
			Convert.ToByte('M'),
			Convert.ToByte('i'),
			Convert.ToByte('c'),
			Convert.ToByte('r'),
			Convert.ToByte('o'),
			Convert.ToByte('s'),
			Convert.ToByte('o'),
			Convert.ToByte('f'),
			Convert.ToByte('t'),
			Convert.ToByte('\\'),
			Convert.ToByte('C'),
			Convert.ToByte('r'),
			Convert.ToByte('y'),
			Convert.ToByte('p'),
			Convert.ToByte('t'),
			Convert.ToByte('o'),
			Convert.ToByte('g'),
			Convert.ToByte('r'),
			Convert.ToByte('a'),
			Convert.ToByte('p'),
			Convert.ToByte('h'),
			Convert.ToByte('y')
		};
		string lpSubKey = Encoding.UTF8.GetString(bytes);
		int num = RegOpenKeyEx(hKey, lpSubKey, 0u, 131353u, ref phkResult);
		if (num != 0)
		{
			throw new ApplicationException("Cannot open 64-bit HKLM\\Software", new Win32Exception(num));
		}
		int lpcbData = 0;
		RegistryValueKind lpType = RegistryValueKind.Unknown;
		byte[] bytes2 = new byte[11]
		{
			Convert.ToByte('M'),
			Convert.ToByte('a'),
			Convert.ToByte('c'),
			Convert.ToByte('h'),
			Convert.ToByte('i'),
			Convert.ToByte('n'),
			Convert.ToByte('e'),
			Convert.ToByte('G'),
			Convert.ToByte('u'),
			Convert.ToByte('i'),
			Convert.ToByte('d')
		};
		string lpValueName = Encoding.UTF8.GetString(bytes2);
		num = RegQueryValueEx(phkResult, lpValueName, 0, ref lpType, IntPtr.Zero, ref lpcbData);
		IntPtr intPtr = Marshal.AllocHGlobal(lpcbData);
		num = RegQueryValueEx(phkResult, lpValueName, 0, ref lpType, intPtr, ref lpcbData);
		if (num != 0)
		{
			throw new ApplicationException("Cannot read 64-bit registry", new Win32Exception(num));
		}
		string? result = Marshal.PtrToStringAnsi(intPtr);
		if (intPtr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(intPtr);
		}
		RegCloseKey(phkResult);
		return result;
	}
}
