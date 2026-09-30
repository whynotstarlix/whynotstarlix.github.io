using System.Collections.Generic;
using System.IO;
using System.Text;
using dnlib.DotNet;

namespace Kitsune_VM;

public class KitsuneExternalCallRegistry
{
	private List<ExternalCallEntry> _entries = new List<ExternalCallEntry>();

	private Dictionary<string, int> _keyToId = new Dictionary<string, int>();

	private int _nextId;

	public int GetOrRegister(string typeName, string methodName, bool hasReturnValue, string[] paramTypeNames, string[] methodGenericArgs, MethodDef sourceDef)
	{
		int orRegister = GetOrRegister(typeName, methodName, hasReturnValue, paramTypeNames, methodGenericArgs);
		ExternalCallEntry externalCallEntry = _entries[orRegister];
		if (externalCallEntry.SourceDef == null)
		{
			externalCallEntry.SourceDef = sourceDef;
		}
		return orRegister;
	}

	public int GetOrRegister(string typeName, string methodName, bool hasReturnValue, string[] paramTypeNames, string[] methodGenericArgs = null)
	{
		typeName = Normalize(typeName);
		string[] array = NormalizeAll(paramTypeNames);
		string[] array2 = NormalizeAll(methodGenericArgs);
		string key = typeName + "::" + methodName + "(" + string.Join(",", array) + ")" + ((array2.Length != 0) ? ("<" + string.Join(",", array2) + ">") : "");
		if (_keyToId.ContainsKey(key))
		{
			return _keyToId[key];
		}
		int num = _nextId++;
		_keyToId[key] = num;
		_entries.Add(new ExternalCallEntry
		{
			Id = num,
			TypeName = typeName,
			MethodName = methodName,
			HasReturnValue = hasReturnValue,
			ParamTypeNames = array,
			MethodGenericArgs = array2
		});
		return num;
	}

	public byte[] Serialize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
		binaryWriter.Write(_entries.Count);
		foreach (ExternalCallEntry entry in _entries)
		{
			binaryWriter.Write(entry.Id);
			binaryWriter.Write(entry.TypeName);
			binaryWriter.Write(entry.MethodName);
			binaryWriter.Write(entry.HasReturnValue);
			binaryWriter.Write(entry.ParamTypeNames.Length);
			string[] paramTypeNames = entry.ParamTypeNames;
			foreach (string value in paramTypeNames)
			{
				binaryWriter.Write(value);
			}
			binaryWriter.Write(entry.MethodGenericArgs.Length);
			paramTypeNames = entry.MethodGenericArgs;
			foreach (string value2 in paramTypeNames)
			{
				binaryWriter.Write(value2);
			}
		}
		return memoryStream.ToArray();
	}

	public List<ExternalCallEntry> GetAllEntries()
	{
		return _entries;
	}

	public void UpdateNamesAfterRename()
	{
		foreach (ExternalCallEntry entry in _entries)
		{
			if (entry.SourceDef != null)
			{
				MethodDef sourceDef = entry.SourceDef;
				string typeName = Normalize(sourceDef.DeclaringType.FullName);
				entry.TypeName = typeName;
				entry.MethodName = UTF8String.op_Implicit(sourceDef.Name);
			}
		}
	}

	public static string Normalize(string name)
	{
		if (name == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder(name.Length + 4);
		Stack<bool> stack = new Stack<bool>();
		for (int i = 0; i < name.Length; i++)
		{
			char c = name[i];
			switch (c)
			{
			case '/':
				stringBuilder.Append('+');
				break;
			case '<':
			{
				bool flag2 = i >= 1 && (char.IsLetterOrDigit(name[i - 1]) || name[i - 1] == '_');
				stack.Push(flag2);
				stringBuilder.Append(flag2 ? '[' : '<');
				break;
			}
			case '>':
			{
				bool flag = stack.Count > 0 && stack.Pop();
				stringBuilder.Append(flag ? ']' : '>');
				break;
			}
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	private static string[] NormalizeAll(string[] names)
	{
		if (names == null || names.Length == 0)
		{
			return new string[0];
		}
		string[] array = new string[names.Length];
		for (int i = 0; i < names.Length; i++)
		{
			array[i] = Normalize(names[i]);
		}
		return array;
	}
}
