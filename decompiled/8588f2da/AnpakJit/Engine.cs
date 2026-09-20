using System;
using System.IO;

namespace AnpakJit;

internal static class Engine
{
	internal static string Unpack(string path, Action<string> log)
	{
		string text = Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path) + "_unpacked" + Path.GetExtension(path));
		MethodRestorer methodRestorer = new MethodRestorer(path, log);
		methodRestorer.Unpack();
		methodRestorer.Save(text);
		return text;
	}
}
