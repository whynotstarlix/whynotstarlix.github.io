using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM.Obfuscation.Rename;

internal class Renamer
{
	private static readonly Random _rnd = new Random();

	private static readonly int[] _pool = BuildPool();

	private static bool ShouldSkipType(TypeDef type)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (type.IsSerializable)
		{
			return true;
		}
		if (type.IsGlobalModuleType)
		{
			return true;
		}
		if (IsResource(type))
		{
			return true;
		}
		UTF8String name = type.Name;
		if ((((name != null) ? name.String : null) ?? "").IndexOfAny(new char[4] { '<', '>', '$', '`' }) >= 0)
		{
			return true;
		}
		Enumerator<CustomAttribute> enumerator = ((LazyList<CustomAttribute>)(object)type.CustomAttributes).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string typeFullName = enumerator.Current.TypeFullName;
				if (typeFullName.Contains("SerializableAttribute"))
				{
					return true;
				}
				if (typeFullName.Contains("ComVisible"))
				{
					return true;
				}
				if (typeFullName.Contains("Guid"))
				{
					return true;
				}
				if (typeFullName.Contains("WrapperName"))
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}

	public static void Execute(ModuleDef module, HashSet<TypeDef> excluded = null, HashSet<TypeDef> noRenameTypes = null, Dictionary<TypeDef, HashSet<string>> noRenameMembers = null)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<(TypeDef, string), string> dictionary = new Dictionary<(TypeDef, string), string>();
		foreach (TypeDef type in module.GetTypes())
		{
			if ((excluded != null && excluded.Contains(type)) || ShouldSkipType(type))
			{
				continue;
			}
			if (noRenameTypes == null || !noRenameTypes.Contains(type))
			{
				Rename(type);
			}
			HashSet<string> value = null;
			noRenameMembers?.TryGetValue(type, out value);
			foreach (MethodDef method in type.Methods)
			{
				if (!method.IsConstructor && !method.IsStaticConstructor && !method.IsRuntimeSpecialName && !method.IsSpecialName && (method.ImplAttributes & 3) == 0 && method != module.EntryPoint && (value == null || !value.Contains(UTF8String.op_Implicit(method.Name))))
				{
					if (method.IsStatic && method.IsPinvokeImpl && method.ImplMap != null)
					{
						FixPinvokeEntryPoint(method);
					}
					string item = UTF8String.op_Implicit(method.Name);
					Rename(method);
					dictionary[(type, item)] = UTF8String.op_Implicit(method.Name);
				}
			}
			foreach (FieldDef field in type.Fields)
			{
				if (!field.IsRuntimeSpecialName && !field.IsSpecialName && (value == null || !value.Contains(UTF8String.op_Implicit(field.Name))))
				{
					string item2 = UTF8String.op_Implicit(field.Name);
					Rename(field);
					dictionary[(type, item2)] = UTF8String.op_Implicit(field.Name);
				}
			}
		}
		FixMemberRefs(module, dictionary);
	}

	private static void FixMemberRefs(ModuleDef module, Dictionary<(TypeDef, string), string> renameMap)
	{
		if (renameMap.Count == 0)
		{
			return;
		}
		foreach (TypeDef type in module.GetTypes())
		{
			foreach (MethodDef method in type.Methods)
			{
				if (!method.HasBody)
				{
					continue;
				}
				foreach (Instruction instruction in method.Body.Instructions)
				{
					object operand = instruction.Operand;
					MemberRef val = (MemberRef)((operand is MemberRef) ? operand : null);
					if (val == null)
					{
						continue;
					}
					try
					{
						TypeDef val2 = null;
						IMemberRefParent val3 = val.Class;
						TypeDef val4 = (TypeDef)(object)((val3 is TypeDef) ? val3 : null);
						if (val4 != null)
						{
							val2 = val4;
						}
						else
						{
							IMemberRefParent val5 = val.Class;
							TypeRef val6 = (TypeRef)(object)((val5 is TypeRef) ? val5 : null);
							if (val6 != null)
							{
								val2 = Extensions.ResolveTypeDef((ITypeDefOrRef)(object)val6);
							}
						}
						if (val2 != null && renameMap.TryGetValue((val2, UTF8String.op_Implicit(val.Name)), out var value))
						{
							val.Name = UTF8String.op_Implicit(value);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	private static void FixPinvokeEntryPoint(MethodDef method)
	{
		try
		{
			ImplMap implMap = method.ImplMap;
			if (implMap != null && string.IsNullOrEmpty(UTF8String.op_Implicit(implMap.Name)))
			{
				implMap.Name = method.Name;
			}
		}
		catch
		{
		}
	}

	private static bool IsResource(TypeDef type)
	{
		return false;
	}

	private static void Rename(TypeDef type)
	{
		type.Name = UTF8String.op_Implicit(RandomName());
		type.Namespace = UTF8String.op_Implicit("");
	}

	private static void Rename(MethodDef method)
	{
		method.Name = UTF8String.op_Implicit(RandomName());
	}

	private static void Rename(FieldDef field)
	{
		field.Name = UTF8String.op_Implicit(RandomName());
	}

	private static int[] BuildPool()
	{
		List<int> list = new List<int>();
		AddRange(1040, 1103);
		AddRange(1575, 1610);
		AddRange(12353, 12438);
		AddRange(12449, 12534);
		AddRange(19968, 40959);
		AddRange(44032, 55203);
		return list.ToArray();
		void AddRange(int from, int to)
		{
			for (int i = from; i <= to; i++)
			{
				if (i < 55296 || i > 57343)
				{
					UnicodeCategory unicodeCategory = char.GetUnicodeCategory((char)i);
					if ((uint)unicodeCategory <= 2u || unicodeCategory == UnicodeCategory.OtherLetter || unicodeCategory == UnicodeCategory.LetterNumber)
					{
						list.Add(i);
					}
				}
			}
		}
	}

	private static string RandomName()
	{
		int num = _rnd.Next(8, 21);
		StringBuilder stringBuilder = new StringBuilder(num);
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append((char)_pool[_rnd.Next(_pool.Length)]);
		}
		return stringBuilder.ToString();
	}
}
