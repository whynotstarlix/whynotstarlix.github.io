using System;

namespace BlueStacks.Player;

public class Video
{
	public class Mode
	{
		public int width;

		public int height;

		public int depth;

		public int Width => width;

		public int Height => height;

		public int Depth => depth;

		public Mode(int width, int height, int depth)
		{
			this.width = width;
			this.height = height;
			this.depth = depth;
		}
	}

	private const uint OFFSET_MAGIC = 0u;

	private const uint OFFSET_LENGTH = 4u;

	private const uint OFFSET_OFFSET = 8u;

	private const uint OFFSET_MODE = 12u;

	private const uint OFFSET_STRIDE = 16u;

	private const uint OFFSET_DIRTY = 20u;

	private IntPtr addr;

	private unsafe byte* raw;

	private uint _cachedStride;

	private IntPtr _cachedBufferAddr = IntPtr.Zero;

	private IntPtr _cachedBufferEnd = IntPtr.Zero;

	private Mode _cachedMode;

	private bool _dirtyCached;

	public unsafe Video(IntPtr addr)
	{
		this.addr = addr;
		raw = (byte*)(void*)addr;
		PreCacheValues();
	}

	public void CheckMagic()
	{
		if (_cachedMode == null)
		{
			uint magic = 0u;
			if (!HDPlusModule.VideoCheckMagic(addr, ref magic))
			{
				throw new SystemException("Bad magic 0x" + magic.ToString("x"));
			}
		}
	}

	public Mode GetMode()
	{
		if (_cachedMode == null)
		{
			uint width = 0u;
			uint height = 0u;
			uint depth = 0u;
			HDPlusModule.VideoGetMode(addr, ref width, ref height, ref depth);
			_cachedMode = new Mode((int)width, (int)height, (int)depth);
		}
		return _cachedMode;
	}

	public bool GetAndClearDirty()
	{
		if (!_dirtyCached)
		{
			_dirtyCached = HDPlusModule.VideoGetAndClearDirty(addr);
		}
		bool dirtyCached = _dirtyCached;
		_dirtyCached = false;
		return dirtyCached;
	}

	public unsafe uint GetStride()
	{
		if (_cachedStride == 0)
		{
			ushort* ptr = (ushort*)raw + 8;
			_cachedStride = *ptr;
		}
		return _cachedStride;
	}

	public unsafe IntPtr GetBufferAddr()
	{
		if (!(_cachedBufferAddr != IntPtr.Zero))
		{
			return (IntPtr)(raw + (uint)((int*)raw)[2]);
		}
		return _cachedBufferAddr;
	}

	public unsafe IntPtr GetBufferEnd()
	{
		if (!(_cachedBufferEnd != IntPtr.Zero))
		{
			return (IntPtr)(raw + (uint)((int*)raw)[1]);
		}
		return _cachedBufferEnd;
	}

	public uint GetBufferSize()
	{
		long num = (long)GetBufferEnd() - (long)GetBufferAddr();
		if (num < 0)
		{
			throw new SystemException("Buffer size is negative");
		}
		return (uint)num;
	}

	private unsafe void PreCacheValues()
	{
		try
		{
			uint* ptr = (uint*)raw + 2;
			uint* ptr2 = (uint*)(raw + *ptr);
			_cachedBufferAddr = (IntPtr)ptr2;
			uint* ptr3 = (uint*)raw + 1;
			uint* ptr4 = (uint*)(raw + *ptr3);
			_cachedBufferEnd = (IntPtr)ptr4;
			ushort* ptr5 = (ushort*)raw + 8;
			_cachedStride = *ptr5;
		}
		catch
		{
		}
	}

	public void RefreshCache()
	{
		_cachedMode = null;
		_cachedStride = 0u;
		_cachedBufferAddr = IntPtr.Zero;
		_cachedBufferEnd = IntPtr.Zero;
		_dirtyCached = false;
		PreCacheValues();
	}
}
