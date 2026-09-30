using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kitsune_VM_Stub;

public class KitsuneObjectHeap
{
	private List<object> _objects = new List<object>();

	public int Count => _objects.Count;

	public KitsuneObjectHeap()
	{
		_objects.Add(null);
	}

	public long Alloc(object obj)
	{
		if (obj == null)
		{
			return 0L;
		}
		int count = _objects.Count;
		_objects.Add(obj);
		return count;
	}

	public object Get(long handle)
	{
		if (handle == 0L)
		{
			return null;
		}
		if (handle < 0 || handle >= _objects.Count)
		{
			throw new Exception("KitsuneObjectHeap: invalid handle " + handle + ". The object may not have been allocated via Alloc().");
		}
		return _objects[(int)handle];
	}

	public void Set(long handle, object obj)
	{
		if (handle == 0L)
		{
			throw new Exception("KitsuneObjectHeap: attempt to write to the null handle (0).");
		}
		if (handle < 0 || handle >= _objects.Count)
		{
			throw new Exception("KitsuneObjectHeap: invalid handle " + handle);
		}
		_objects[(int)handle] = obj;
	}

	public bool IsNull(long handle)
	{
		return handle == 0;
	}

	public long GetField(long objHandle, FieldInfo field)
	{
		object obj = Get(objHandle);
		object value = field.GetValue(obj);
		return BoxToLong(value, field.FieldType);
	}

	public void SetField(long objHandle, FieldInfo field, long rawValue)
	{
		object obj = Get(objHandle);
		object value = UnboxFromLong(rawValue, field.FieldType, this);
		field.SetValue(obj, value);
	}

	public long GetStaticField(FieldInfo field)
	{
		object value = field.GetValue(null);
		return BoxToLong(value, field.FieldType);
	}

	public void SetStaticField(FieldInfo field, long rawValue)
	{
		object value = UnboxFromLong(rawValue, field.FieldType, this);
		field.SetValue(null, value);
	}

	public long BoxToLong(object value, Type type)
	{
		if (value == null)
		{
			return 0L;
		}
		if (type == typeof(int))
		{
			return (int)value;
		}
		if (type == typeof(long))
		{
			return (long)value;
		}
		if (type == typeof(bool))
		{
			return ((bool)value) ? 1 : 0;
		}
		if (type == typeof(byte))
		{
			return (byte)value;
		}
		if (type == typeof(short))
		{
			return (short)value;
		}
		if (type == typeof(char))
		{
			return (char)value;
		}
		if (type == typeof(uint))
		{
			return (uint)value;
		}
		if (type == typeof(ulong))
		{
			return (long)(ulong)value;
		}
		if (type == typeof(sbyte))
		{
			return (sbyte)value;
		}
		if (type == typeof(ushort))
		{
			return (ushort)value;
		}
		if (type == typeof(float))
		{
			return BitConverter.ToInt32(BitConverter.GetBytes((float)value), 0);
		}
		if (type == typeof(double))
		{
			return BitConverter.DoubleToInt64Bits((double)value);
		}
		if (type == typeof(IntPtr))
		{
			return ((IntPtr)value).ToInt64();
		}
		if (type == typeof(UIntPtr))
		{
			return (long)((UIntPtr)value).ToUInt64();
		}
		if (type != null && type.IsPointer)
		{
			if (value is IntPtr intPtr)
			{
				return intPtr.ToInt64();
			}
			if (value is UIntPtr uIntPtr)
			{
				return (long)uIntPtr.ToUInt64();
			}
			try
			{
				return Convert.ToInt64(value);
			}
			catch
			{
			}
			return Alloc(value);
		}
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (underlyingType != null)
		{
			if (value == null)
			{
				return 0L;
			}
			return BoxToLong(value, underlyingType);
		}
		return Alloc(value);
	}

	public static object UnboxFromLong(long raw, Type targetType, KitsuneObjectHeap heap)
	{
		if (targetType == typeof(int))
		{
			return (int)raw;
		}
		if (targetType == typeof(long))
		{
			return raw;
		}
		if (targetType == typeof(bool))
		{
			return raw != 0;
		}
		if (targetType == typeof(byte))
		{
			return (byte)raw;
		}
		if (targetType == typeof(short))
		{
			return (short)raw;
		}
		if (targetType == typeof(char))
		{
			return (char)raw;
		}
		if (targetType == typeof(uint))
		{
			return (uint)raw;
		}
		if (targetType == typeof(ulong))
		{
			return (ulong)raw;
		}
		if (targetType == typeof(sbyte))
		{
			return (sbyte)raw;
		}
		if (targetType == typeof(ushort))
		{
			return (ushort)raw;
		}
		if (targetType == typeof(float))
		{
			return BitConverter.ToSingle(BitConverter.GetBytes((int)raw), 0);
		}
		if (targetType == typeof(double))
		{
			return BitConverter.Int64BitsToDouble(raw);
		}
		if (targetType == typeof(IntPtr))
		{
			if (heap != null && raw > 0 && raw < heap.Count && heap.Get(raw) is IntPtr intPtr)
			{
				return intPtr;
			}
			return new IntPtr(raw);
		}
		if (targetType == typeof(UIntPtr))
		{
			if (heap != null && raw > 0 && raw < heap.Count && heap.Get(raw) is UIntPtr uIntPtr)
			{
				return uIntPtr;
			}
			return new UIntPtr((ulong)raw);
		}
		Type underlyingType = Nullable.GetUnderlyingType(targetType);
		if (underlyingType != null)
		{
			if (raw == 0L)
			{
				return null;
			}
			return UnboxFromLong(raw, underlyingType, heap);
		}
		if (targetType == typeof(RuntimeTypeHandle))
		{
			object obj = heap.Get(raw);
			if (obj is Type type)
			{
				return type.TypeHandle;
			}
			return (RuntimeTypeHandle)obj;
		}
		if (targetType.IsEnum)
		{
			return Enum.ToObject(targetType, raw);
		}
		if (!(targetType == typeof(string)))
		{
			_ = targetType.IsValueType;
		}
		return heap.Get(raw);
	}

	public long AllocArray(int length)
	{
		object[] obj = new object[length];
		return Alloc(obj);
	}

	public long AllocTypedArray(Type elementType, int length)
	{
		Array obj = Array.CreateInstance(elementType, length);
		return Alloc(obj);
	}

	public int GetArrayLength(long arrHandle)
	{
		return ((Array)Get(arrHandle)).Length;
	}

	public long GetArrayElement(long arrHandle, int index, Type elementType)
	{
		object value = ((Array)Get(arrHandle)).GetValue(index);
		return BoxToLong(value, elementType);
	}

	public void SetArrayElement(long arrHandle, int index, long rawValue, Type elementType)
	{
		Array array = (Array)Get(arrHandle);
		Type targetType = array.GetType().GetElementType() ?? elementType;
		object value = UnboxFromLong(rawValue, targetType, this);
		array.SetValue(value, index);
	}
}
