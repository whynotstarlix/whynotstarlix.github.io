using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public struct IntereopRect : IEquatable<IntereopRect>
{
	private int _left;

	private int _top;

	private int _right;

	private int _bottom;

	public int Left
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_left = value;
		}
	}

	public int Top
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _top;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_top = value;
		}
	}

	public int Right
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _right;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_right = value;
		}
	}

	public int Bottom
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _bottom;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_bottom = value;
		}
	}

	public int X
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_right -= _left - value;
			_left = value;
		}
	}

	public int Y
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _top;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_bottom -= _top - value;
			_top = value;
		}
	}

	public int Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _bottom - _top;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_bottom = value + _top;
		}
	}

	public int Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _right - _left;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_right = value + _left;
		}
	}

	public Point Location
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Point(_left, _top);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	public Size Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Size(Width, Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntereopRect(int left, int top, int right, int bottom)
	{
		_left = left;
		_top = top;
		_right = right;
		_bottom = bottom;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntereopRect(Rectangle r)
	{
		_left = r.Left;
		_top = r.Top;
		_right = r.Right;
		_bottom = r.Bottom;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Rectangle(IntereopRect r)
	{
		return new Rectangle(r.Left, r.Top, r.Width, r.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator IntereopRect(Rectangle r)
	{
		return new IntereopRect(r);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(IntereopRect r1, IntereopRect r2)
	{
		if (r1._left == r2._left && r1._top == r2._top && r1._right == r2._right)
		{
			return r1._bottom == r2._bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(IntereopRect r1, IntereopRect r2)
	{
		return !(r1 == r2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(IntereopRect r)
	{
		if (r._left == _left && r._top == _top && r._right == _right)
		{
			return r._bottom == _bottom;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IntereopRect r)
		{
			return Equals(r);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return _left ^ _top ^ _right ^ _bottom;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", new object[4] { _left, _top, _right, _bottom });
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rectangle ToRectangle()
	{
		return new Rectangle(_left, _top, Width, Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntereopRect ToIntereopRect()
	{
		return this;
	}
}
