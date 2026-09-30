using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kitsune_VM;

public class KitsuneStringRegistry
{
	private List<string> _strings = new List<string>();

	private Dictionary<string, int> _lookup = new Dictionary<string, int>();

	public int Count => _strings.Count;

	public int GetOrAdd(string value)
	{
		if (_lookup.TryGetValue(value, out var value2))
		{
			return value2;
		}
		value2 = _strings.Count;
		_strings.Add(value);
		_lookup[value] = value2;
		return value2;
	}

	public byte[] Serialize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
		binaryWriter.Write(_strings.Count);
		foreach (string @string in _strings)
		{
			binaryWriter.Write(@string ?? "");
		}
		return memoryStream.ToArray();
	}
}
