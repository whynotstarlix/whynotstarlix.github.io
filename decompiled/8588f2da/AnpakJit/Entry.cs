using System;
using System.IO;

namespace AnpakJit;

internal static class Entry
{
	public static void Main(string[] args)
	{
		Console.Title = "анпак джит by cultreverse";
		Console.WriteLine("анпак джит by cultreverse");
		if (args.Length < 1)
		{
			Console.WriteLine("Usage: АнпакДжит.exe <path_to_obfuscated_file>");
			return;
		}
		string path = args[0];
		if (!File.Exists(path))
		{
			Console.WriteLine("Error: File not found.");
			return;
		}
		try
		{
			Engine.Unpack(path, Console.WriteLine);
		}
		catch (Exception ex)
		{
			Console.WriteLine((ex.InnerException ?? ex).Message);
			Console.WriteLine((ex.InnerException ?? ex).StackTrace);
		}
		Console.WriteLine();
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey(intercept: true);
	}
}
