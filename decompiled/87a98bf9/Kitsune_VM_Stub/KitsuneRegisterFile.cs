using System;

namespace Kitsune_VM_Stub;

public sealed class KitsuneRegisterFile
{
	private readonly long[] _arr;

	private readonly long _mask;

	public long this[int i]
	{
		get
		{
			return _arr[i] ^ _mask;
		}
		set
		{
			_arr[i] = value ^ _mask;
		}
	}

	public KitsuneRegisterFile(int count)
	{
		_arr = new long[count];
		Random random = new Random();
		_mask = ((long)random.Next() << 32) | (uint)random.Next();
	}
}
