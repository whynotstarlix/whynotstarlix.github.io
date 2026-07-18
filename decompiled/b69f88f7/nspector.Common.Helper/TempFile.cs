using System;
using System.IO;

namespace nspector.Common.Helper;

internal static class TempFile
{
	public static string GetTempFileName()
	{
		string text;
		do
		{
			text = GenerateTempFileName();
		}
		while (File.Exists(text));
		return text;
	}

	private static string GenerateTempFileName()
	{
		return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", ""));
	}
}
