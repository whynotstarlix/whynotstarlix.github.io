using System;
using System.Reflection;

namespace Kitsune_VM_Stub;

public class KitsuneHeapRuntime
{
	public string[] Strings;

	public FieldInfo[] Fields;

	public Type[] Types;

	public ConstructorInfo[] Ctors;

	public KitsuneObjectHeap Heap;

	public static KitsuneHeapRuntime Empty()
	{
		return new KitsuneHeapRuntime
		{
			Strings = new string[0],
			Fields = new FieldInfo[0],
			Types = new Type[0],
			Ctors = new ConstructorInfo[0],
			Heap = new KitsuneObjectHeap()
		};
	}

	public KitsuneHeapRuntime WithFreshHeap()
	{
		return new KitsuneHeapRuntime
		{
			Strings = Strings,
			Fields = Fields,
			Types = Types,
			Ctors = Ctors,
			Heap = new KitsuneObjectHeap()
		};
	}

	public long LoadString(int id)
	{
		if (id < 0 || id >= Strings.Length)
		{
			throw new Exception("KitsuneHeapRuntime: invalid string id " + id);
		}
		return Heap.Alloc(Strings[id]);
	}

	public long CreateObject(int ctorId, long[] argValues)
	{
		if (ctorId < 0 || ctorId >= Ctors.Length)
		{
			throw new Exception("KitsuneHeapRuntime: invalid constructor id " + ctorId);
		}
		ConstructorInfo constructorInfo = Ctors[ctorId];
		ParameterInfo[] parameters = constructorInfo.GetParameters();
		object[] array = new object[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = KitsuneObjectHeap.UnboxFromLong(argValues[i], parameters[i].ParameterType, Heap);
		}
		object obj = constructorInfo.Invoke(array);
		return Heap.Alloc(obj);
	}
}
