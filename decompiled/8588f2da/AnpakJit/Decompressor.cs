using System;
using System.IO;
using System.Reflection;

namespace AnpakJit;

internal static class Decompressor
{
	public static byte[] Decompress(byte[] input, int extra)
	{
		Type type = LoadEngineType();
		if (type == null)
		{
			throw new InvalidOperationException("рядом должен лежать unpackjit.exe");
		}
		return (byte[])type.GetMethod("Decompress", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[2] { input, extra });
	}

	private static Type LoadEngineType()
	{
		string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "unpackjit.exe");
		if (!File.Exists(text))
		{
			text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unpackjit.exe");
		}
		if (!File.Exists(text))
		{
			return null;
		}
		return Assembly.LoadFrom(text).GetType("unpackjit.Decompressor");
	}
}
