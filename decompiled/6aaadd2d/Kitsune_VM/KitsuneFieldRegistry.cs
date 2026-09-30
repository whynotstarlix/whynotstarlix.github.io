using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kitsune_VM;

public class KitsuneFieldRegistry
{
	public class FieldEntry
	{
		public int Id;

		public string TypeName;

		public string FieldName;
	}

	private List<FieldEntry> _entries = new List<FieldEntry>();

	private Dictionary<string, int> _lookup = new Dictionary<string, int>();

	public int Count => _entries.Count;

	public int GetOrRegister(string typeName, string fieldName)
	{
		string key = typeName + "::" + fieldName;
		if (_lookup.TryGetValue(key, out var value))
		{
			return value;
		}
		value = _entries.Count;
		_entries.Add(new FieldEntry
		{
			Id = value,
			TypeName = typeName,
			FieldName = fieldName
		});
		_lookup[key] = value;
		return value;
	}

	public List<FieldEntry> GetAllEntries()
	{
		return _entries;
	}

	public byte[] Serialize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
		binaryWriter.Write(_entries.Count);
		foreach (FieldEntry entry in _entries)
		{
			binaryWriter.Write(entry.TypeName);
			binaryWriter.Write(entry.FieldName);
		}
		return memoryStream.ToArray();
	}
}
