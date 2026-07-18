using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nspector.Native;

internal class NativeArrayHelper
{
	public static T GetArrayItemData<T>(IntPtr sourcePointer)
	{
		return (T)Marshal.PtrToStructure(sourcePointer, typeof(T));
	}

	public static T[] GetArrayData<T>(IntPtr sourcePointer, int itemCount)
	{
		List<T> list = new List<T>();
		if (sourcePointer != IntPtr.Zero && itemCount > 0)
		{
			int num = Marshal.SizeOf(typeof(T));
			for (int i = 0; i < itemCount; i++)
			{
				list.Add(GetArrayItemData<T>(sourcePointer + num * i));
			}
		}
		return list.ToArray();
	}

	public static void SetArrayData<T>(T[] items, out IntPtr targetPointer)
	{
		if (items != null && items.Length != 0)
		{
			int num = Marshal.SizeOf(typeof(T));
			targetPointer = Marshal.AllocHGlobal(num * items.Length);
			for (int i = 0; i < items.Length; i++)
			{
				Marshal.StructureToPtr(items[i], targetPointer + num * i, fDeleteOld: true);
			}
		}
		else
		{
			targetPointer = IntPtr.Zero;
		}
	}

	public static void SetArrayItemData<T>(T item, out IntPtr targetPointer)
	{
		int cb = Marshal.SizeOf(typeof(T));
		targetPointer = Marshal.AllocHGlobal(cb);
		Marshal.StructureToPtr(item, targetPointer, fDeleteOld: true);
	}
}
