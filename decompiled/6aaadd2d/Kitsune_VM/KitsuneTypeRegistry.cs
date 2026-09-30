using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kitsune_VM;

public class KitsuneTypeRegistry
{
	public class TypeEntry
	{
		public int Id;

		public string TypeName;

		public string[] CtorArgTypeNames;

		public bool IsConstructor => CtorArgTypeNames != null;
	}

	private List<TypeEntry> _entries = new List<TypeEntry>();

	private Dictionary<string, int> _lookup = new Dictionary<string, int>();

	public int Count => _entries.Count;

	public int GetOrRegisterType(string typeName)
	{
		string key = "T:" + typeName;
		if (_lookup.TryGetValue(key, out var value))
		{
			return value;
		}
		value = _entries.Count;
		_entries.Add(new TypeEntry
		{
			Id = value,
			TypeName = typeName,
			CtorArgTypeNames = null
		});
		_lookup[key] = value;
		return value;
	}

	public int GetOrRegisterConstructor(string typeName, string[] argTypeNames)
	{
		string key = "C:" + typeName + "(" + string.Join(",", argTypeNames) + ")";
		if (_lookup.TryGetValue(key, out var value))
		{
			return value;
		}
		value = _entries.Count;
		_entries.Add(new TypeEntry
		{
			Id = value,
			TypeName = typeName,
			CtorArgTypeNames = argTypeNames
		});
		_lookup[key] = value;
		return value;
	}

	public List<TypeEntry> GetAllEntries()
	{
		return _entries;
	}

	public byte[] Serialize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
		binaryWriter.Write(_entries.Count);
		foreach (TypeEntry entry in _entries)
		{
			binaryWriter.Write(entry.TypeName);
			binaryWriter.Write(entry.IsConstructor);
			if (entry.IsConstructor)
			{
				binaryWriter.Write(entry.CtorArgTypeNames.Length);
				string[] ctorArgTypeNames = entry.CtorArgTypeNames;
				foreach (string value in ctorArgTypeNames)
				{
					binaryWriter.Write(value);
				}
			}
		}
		return memoryStream.ToArray();
	}
}
