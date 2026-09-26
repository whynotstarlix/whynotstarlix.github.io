using System;

namespace BlueStacks.Player;

public class ColorFormatNotSupported : Exception
{
	public ColorFormatNotSupported()
	{
	}

	public ColorFormatNotSupported(string message)
		: base(message)
	{
	}

	public ColorFormatNotSupported(string message, Exception inner)
		: base(message, inner)
	{
	}
}
