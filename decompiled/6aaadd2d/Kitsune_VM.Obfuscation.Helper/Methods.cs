using System;
using System.Linq;

namespace Kitsune_VM.Obfuscation.Helper;

internal class Methods
{
	private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	[ThreadStatic]
	private static Random _localRandom;

	private static Random SafeRandom => _localRandom ?? (_localRandom = new Random(Guid.NewGuid().GetHashCode()));

	public static string GenerateString()
	{
		return GenerateString((byte)SafeRandom.Next(10, 30));
	}

	public static string GenerateString(byte length)
	{
		char c = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[SafeRandom.Next(52)];
		char[] value = (from s in Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", length - 1)
			select s[SafeRandom.Next(s.Length)]).ToArray();
		return c + new string(value);
	}

	public static bool GenerateBool()
	{
		return SafeRandom.Next(2) == 1;
	}

	public static bool GenerateBool(byte probability)
	{
		return SafeRandom.Next(probability) == 1;
	}
}
