using System;
using System.Drawing;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

public struct IntereopRect : IEquatable<IntereopRect>
{
	public int Left { get; set; }

	public int Top { get; set; }

	public int Right { get; set; }

	public int Bottom { get; set; }

	public int X
	{
		get
		{
			return Left;
		}
		set
		{
			Right -= Left - value;
			Left = value;
		}
	}

	public int Y
	{
		get
		{
			return Top;
		}
		set
		{
			Bottom -= Top - value;
			Top = value;
		}
	}

	public int Height
	{
		get
		{
			return Bottom - Top;
		}
		set
		{
			Bottom = value + Top;
		}
	}

	public int Width
	{
		get
		{
			return Right - Left;
		}
		set
		{
			Right = value + Left;
		}
	}

	public Point Location
	{
		get
		{
			return new Point(Left, Top);
		}
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	public Size Size
	{
		get
		{
			return new Size(Width, Height);
		}
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	public IntereopRect(int left, int top, int right, int bottom)
	{
		Left = left;
		Top = top;
		Right = right;
		Bottom = bottom;
	}

	public IntereopRect(Rectangle r)
		: this(r.Left, r.Top, r.Right, r.Bottom)
	{
	}

	public static implicit operator Rectangle(IntereopRect r)
	{
		return new Rectangle(r.Left, r.Top, r.Width, r.Height);
	}

	public static implicit operator IntereopRect(Rectangle r)
	{
		return new IntereopRect(r);
	}

	public static bool operator ==(IntereopRect r1, IntereopRect r2)
	{
		if (r1.Equals(default(IntereopRect)))
		{
			return r2.Equals(default(IntereopRect));
		}
		return r1.Equals(r2);
	}

	public static bool operator !=(IntereopRect r1, IntereopRect r2)
	{
		return !(r1 == r2);
	}

	public bool Equals(IntereopRect r)
	{
		if (r.Left == Left && r.Top == Top && r.Right == Right)
		{
			return r.Bottom == Bottom;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IntereopRect)
		{
			return Equals((IntereopRect)obj);
		}
		if (obj is Rectangle)
		{
			return Equals(new IntereopRect((Rectangle)obj));
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((Rectangle)this/*cast due to constrained. prefix*/).GetHashCode();
	}

	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		object[] array = new object[-922746739 + (0x36E58306 | 0x229EFD73)];
		array[0] = Left;
		array[1] = Top;
		array[-443254776 - ~443254777] = Right;
		array[-562954908 ^ -562954905] = Bottom;
		return string.Format(currentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", array);
	}

	public Rectangle ToRectangle()
	{
		return new Rectangle(Left, Top, Width, Height);
	}

	public IntereopRect ToIntereopRect()
	{
		return new IntereopRect(this);
	}
}
