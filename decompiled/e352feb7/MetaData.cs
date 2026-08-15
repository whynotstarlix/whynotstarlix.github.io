using System;
using BlueStacks.BlueStacksUI;

[Serializable]
public class MetaData
{
	public string ParserVersion { get; set; } = KMManager.ParserVersion;

	public string Comment { get; set; }
}
