using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kitsune_VM_Stub;

public class KitsuneExternalCallTable
{
	private Dictionary<int, ExternalCallHandler> _table = new Dictionary<int, ExternalCallHandler>();

	private Dictionary<int, MethodBase> _methodBases = new Dictionary<int, MethodBase>();

	private static readonly bool _debugMode = false;

	private Dictionary<int, string> _idToLabel = new Dictionary<int, string>();

	private static readonly string[] _wellKnownDlls = new string[24]
	{
		"System.dll", "System.Core.dll", "System.Net.dll", "System.Net.Http.dll", "System.Net.Sockets.dll", "System.Runtime.dll", "System.Collections.dll", "System.Linq.dll", "System.Xml.dll", "System.Data.dll",
		"System.Drawing.dll", "System.Windows.Forms.dll", "Microsoft.VisualBasic.dll", "System.Management.dll", "System.ServiceProcess.dll", "System.DirectoryServices.dll", "System.Runtime.Remoting.dll", "System.Web.dll", "System.Transactions.dll", "System.Numerics.dll",
		"PresentationCore.dll", "PresentationFramework.dll", "WindowsBase.dll", "System.Xaml.dll"
	};

	public MethodBase GetMethodBase(int id)
	{
		_methodBases.TryGetValue(id, out var value);
		return value;
	}

	public void Register(int id, ExternalCallHandler handler)
	{
		_table[id] = handler;
	}

	public void RegisterFromReflection(string typeName, string methodName, Type[] paramTypes, bool hasReturnValue, int id, string[] methodGenericArgs = null)
	{
		Type type = ResolveType(typeName);
		if (methodName == ".ctor")
		{
			ConstructorInfo constructorInfo = null;
			if (paramTypes.Length != 0)
			{
				constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
			}
			if (constructorInfo == null)
			{
				ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (ConstructorInfo constructorInfo2 in constructors)
				{
					if (constructorInfo2.GetParameters().Length == paramTypes.Length)
					{
						constructorInfo = constructorInfo2;
						break;
					}
				}
			}
			if (constructorInfo == null)
			{
				throw new Exception("KitsuneExternalCallTable: method not found: " + typeName + "::.ctor");
			}
			_methodBases[id] = constructorInfo;
			_idToLabel[id] = typeName + "::.ctor";
			ConstructorInfo capturedCtor = constructorInfo;
			bool isValueType = type.IsValueType;
			Type capturedType = type;
			_table[id] = (KitsuneState state, int[] argRegs, int retReg) =>
			{
				KitsuneObjectHeap heap = state.Heap;
				ParameterInfo[] parameters2 = capturedCtor.GetParameters();
				int num3 = 1;
				object[] array2 = new object[parameters2.Length];
				for (int j = 0; j < parameters2.Length; j++)
				{
					long raw = state.R[argRegs[num3 + j]];
					array2[j] = KitsuneObjectHeap.UnboxFromLong(raw, parameters2[j].ParameterType, heap);
				}
				object value2 = ((!isValueType || parameters2.Length == 0) ? capturedCtor.Invoke(array2) : Activator.CreateInstance(capturedType, array2));
				state.R[argRegs[0]] = heap.BoxToLong(value2, capturedType);
			};
			return;
		}
		MethodInfo methodInfo = null;
		try
		{
			methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
		}
		catch (AmbiguousMatchException)
		{
		}
		if (methodInfo == null)
		{
			bool flag = methodGenericArgs != null && methodGenericArgs.Length != 0;
			MethodInfo methodInfo2 = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo3 in methods)
			{
				if (methodInfo3.Name != methodName || (flag && !methodInfo3.IsGenericMethodDefinition) || (!flag && methodInfo3.IsGenericMethodDefinition))
				{
					continue;
				}
				ParameterInfo[] parameters = methodInfo3.GetParameters();
				if (parameters.Length != paramTypes.Length)
				{
					continue;
				}
				bool flag2 = true;
				for (int num = 0; num < parameters.Length; num++)
				{
					if (parameters[num].ParameterType != paramTypes[num])
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					if (!methodInfo3.ReturnType.IsPointer)
					{
						methodInfo = methodInfo3;
						break;
					}
					if (methodInfo2 == null)
					{
						methodInfo2 = methodInfo3;
					}
				}
			}
			if (methodInfo == null)
			{
				methodInfo = methodInfo2;
			}
			if (methodInfo == null)
			{
				MethodInfo methodInfo4 = null;
				methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo5 in methods)
				{
					if (!(methodInfo5.Name != methodName) && (!flag || methodInfo5.IsGenericMethodDefinition) && (flag || !methodInfo5.IsGenericMethodDefinition) && methodInfo5.GetParameters().Length == paramTypes.Length)
					{
						if (!methodInfo5.ReturnType.IsPointer)
						{
							methodInfo = methodInfo5;
							break;
						}
						if (methodInfo4 == null)
						{
							methodInfo4 = methodInfo5;
						}
					}
				}
				if (methodInfo == null)
				{
					methodInfo = methodInfo4;
				}
			}
		}
		if (methodInfo == null)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder("Methods on " + type.FullName + " (looking for '" + methodName + "' with " + paramTypes.Length + " params):\n");
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo6 in methods)
				{
					stringBuilder.AppendLine("  " + methodInfo6.Name + " params=" + methodInfo6.GetParameters().Length);
				}
				File.AppendAllText("kitsune_debug.log", stringBuilder.ToString());
			}
			catch
			{
			}
			throw new Exception("KitsuneExternalCallTable: method not found: " + typeName + "::" + methodName);
		}
		if (methodGenericArgs != null && methodGenericArgs.Length != 0)
		{
			if (!methodInfo.IsGenericMethodDefinition)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo7 in methods)
				{
					if (methodInfo7.Name == methodName && methodInfo7.IsGenericMethodDefinition && methodInfo7.GetGenericArguments().Length == methodGenericArgs.Length)
					{
						methodInfo = methodInfo7;
						break;
					}
				}
			}
			if (methodInfo.IsGenericMethodDefinition)
			{
				Type[] array = new Type[methodGenericArgs.Length];
				for (int num2 = 0; num2 < methodGenericArgs.Length; num2++)
				{
					array[num2] = ResolveType(methodGenericArgs[num2]);
				}
				methodInfo = methodInfo.MakeGenericMethod(array);
			}
		}
		bool isStatic = methodInfo.IsStatic;
		MethodInfo capturedMethod = methodInfo;
		_methodBases[id] = methodInfo;
		ExternalCallHandler value = (KitsuneState state, int[] argRegs, int retReg) =>
		{
			KitsuneObjectHeap heap = state.Heap;
			ParameterInfo[] parameters2 = capturedMethod.GetParameters();
			int num3 = ((!isStatic) ? 1 : 0);
			object[] array2 = new object[parameters2.Length];
			bool[] array3 = new bool[parameters2.Length];
			Type[] array4 = new Type[parameters2.Length];
			for (int j = 0; j < parameters2.Length; j++)
			{
				Type parameterType = parameters2[j].ParameterType;
				long raw = state.R[argRegs[num3 + j]];
				if (parameterType.IsByRef)
				{
					array3[j] = true;
					array2[j] = KitsuneObjectHeap.UnboxFromLong(raw, array4[j] = parameterType.GetElementType(), heap);
				}
				else
				{
					array2[j] = KitsuneObjectHeap.UnboxFromLong(raw, parameterType, heap);
				}
			}
			object obj2 = null;
			if (!isStatic)
			{
				long num4 = state.R[argRegs[0]];
				obj2 = ((!capturedMethod.DeclaringType.IsValueType) ? heap.Get(num4) : KitsuneObjectHeap.UnboxFromLong(num4, capturedMethod.DeclaringType, heap));
			}
			object obj3 = capturedMethod.Invoke(obj2, array2);
			for (int k = 0; k < parameters2.Length; k++)
			{
				if (array3[k])
				{
					long value2 = heap.BoxToLong(array2[k], array4[k]);
					long num5 = state.R[argRegs[num3 + k]];
					if (num5 != 0L && num5 < 65536)
					{
						heap.Set(num5, array2[k]);
					}
					else
					{
						state.R[argRegs[num3 + k]] = value2;
					}
				}
			}
			if (hasReturnValue && retReg >= 0)
			{
				long value3 = ((!(obj3 is IntPtr intPtr)) ? ((!(obj3 is UIntPtr uIntPtr)) ? heap.BoxToLong(obj3, capturedMethod.ReturnType) : ((long)uIntPtr.ToUInt64())) : intPtr.ToInt64());
				state.R[retReg] = value3;
			}
		};
		_table[id] = value;
		_idToLabel[id] = typeName + "::" + methodName;
	}

	public void Invoke(int id, KitsuneState state, int[] argRegs, int retReg)
	{
		if (!_table.TryGetValue(id, out var value))
		{
			throw new Exception("KitsuneExternalCallTable: no handler registered for ID " + id);
		}
		if (_debugMode)
		{
			_idToLabel.TryGetValue(id, out var value2);
			Console.Error.WriteLine("[KITSUNE] CALL id=" + id + " " + (value2 ?? "?") + " args=[" + string.Join(",", Array.ConvertAll(argRegs, (int r) => state.R[r].ToString())) + "] retReg=" + retReg);
		}
		value(state, argRegs, retReg);
		if (_debugMode && retReg >= 0)
		{
			Console.Error.WriteLine("[KITSUNE]      → R" + retReg + "=" + state.R[retReg]);
		}
	}

	private static Type ResolveType(string fullName)
	{
		string text = fullName.Replace('/', '+');
		if (IsGenericParamRef(text))
		{
			return typeof(object);
		}
		Type type = TryFindType(text);
		if (type != null)
		{
			return type;
		}
		int num = text.LastIndexOf('+');
		if (num > 0)
		{
			string text2 = text.Substring(0, num);
			string text3 = text.Substring(num + 1);
			string text4 = text3;
			string text5 = null;
			int num2 = text3.IndexOf('[');
			if (num2 > 0 && text3.EndsWith("]"))
			{
				text4 = text3.Substring(0, num2);
				text5 = text3.Substring(num2 + 1, text3.Length - num2 - 2);
			}
			Type type2 = TryFindType(text2);
			if (type2 == null)
			{
				type2 = ScanAllTypesForName(text2);
			}
			if (type2 == null)
			{
				try
				{
					type2 = ResolveType(text2);
				}
				catch
				{
				}
			}
			if (type2 != null)
			{
				type = type2.GetNestedType(text4, BindingFlags.Public | BindingFlags.NonPublic);
				if (type == null)
				{
					Type[] nestedTypes = type2.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
					foreach (Type type3 in nestedTypes)
					{
						if (type3.Name == text4)
						{
							type = type3;
							break;
						}
					}
				}
				if (type != null && text5 != null && type.IsGenericTypeDefinition)
				{
					string[] array = SplitGenericArgs(text5);
					Type[] array2 = new Type[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = ResolveType(array[j].Trim());
					}
					type = type.MakeGenericType(array2);
				}
				if (type != null)
				{
					return type;
				}
			}
		}
		int num3 = text.IndexOf('[');
		if (num3 > 0 && text.EndsWith("]"))
		{
			string clrName = text.Substring(0, num3);
			string s = text.Substring(num3 + 1, text.Length - num3 - 2);
			Type type4 = TryFindType(clrName);
			if (type4 != null && type4.IsGenericTypeDefinition)
			{
				string[] array3 = SplitGenericArgs(s);
				Type[] array4 = new Type[array3.Length];
				for (int k = 0; k < array3.Length; k++)
				{
					array4[k] = ResolveType(array3[k].Trim());
				}
				return type4.MakeGenericType(array4);
			}
		}
		throw new Exception("KitsuneExternalCallTable: type not found: " + fullName);
	}

	private static bool IsGenericParamRef(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		string text = name.TrimEnd(new char[2] { '&', '*' }).TrimEnd(new char[2] { ']', '[' });
		if (text.StartsWith("!"))
		{
			return true;
		}
		if (text.IndexOf('.') < 0 && text.IndexOf('`') < 0 && text.IndexOf('[') < 0 && text.Length >= 1 && (text.Length == 1 || (text.Length <= 20 && text[0] == 'T' && char.IsUpper(text[1]))) && char.IsUpper(text[0]))
		{
			switch (text)
			{
			case "Object":
			case "String":
			case "Boolean":
			case "Byte":
			case "Int16":
			case "Int32":
			case "Int64":
			case "UInt32":
			case "UInt64":
			case "Single":
			case "Double":
			case "Void":
			case "IntPtr":
			case "UIntPtr":
			case "Char":
			case "Decimal":
			case "DateTime":
			case "Guid":
			case "TimeSpan":
			case "Stream":
			case "Type":
			case "Exception":
			case "Array":
			case "Enum":
			case "Delegate":
			case "Attribute":
			case "ValueType":
				return false;
			default:
				return true;
			}
		}
		return false;
	}

	private static Type TryFindType(string clrName)
	{
		if (IsGenericParamRef(clrName))
		{
			return null;
		}
		Type type = Type.GetType(clrName);
		if (type != null)
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType(clrName);
			if (type != null)
			{
				return type;
			}
		}
		string directoryName = Path.GetDirectoryName(typeof(object).Assembly.Location);
		string[] wellKnownDlls = _wellKnownDlls;
		foreach (string path in wellKnownDlls)
		{
			string text = Path.Combine(directoryName, path);
			if (!File.Exists(text))
			{
				continue;
			}
			try
			{
				type = Assembly.LoadFrom(text).GetType(clrName);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
		}
		try
		{
			wellKnownDlls = Directory.GetFiles(directoryName, "*.dll");
			foreach (string assemblyFile in wellKnownDlls)
			{
				try
				{
					type = Assembly.LoadFrom(assemblyFile).GetType(clrName);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		string directoryName2 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		if (!string.IsNullOrEmpty(directoryName2))
		{
			wellKnownDlls = Directory.GetFiles(directoryName2, "*.dll");
			foreach (string assemblyFile2 in wellKnownDlls)
			{
				try
				{
					type = Assembly.LoadFrom(assemblyFile2).GetType(clrName);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
		}
		type = ScanAllTypesForName(clrName);
		if (type != null)
		{
			return type;
		}
		return null;
	}

	private static Type ScanAllTypesForName(string clrName)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] array = null;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				array = ex.Types;
			}
			catch
			{
				continue;
			}
			if (array == null)
			{
				continue;
			}
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				if (type != null && type.FullName == clrName)
				{
					return type;
				}
			}
		}
		return null;
	}

	private static string[] SplitGenericArgs(string s)
	{
		List<string> list = new List<string>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == '[')
			{
				num++;
			}
			else if (s[i] == ']')
			{
				num--;
			}
			else if (s[i] == ',' && num == 0)
			{
				list.Add(s.Substring(num2, i - num2));
				num2 = i + 1;
			}
		}
		list.Add(s.Substring(num2));
		return list.ToArray();
	}
}
