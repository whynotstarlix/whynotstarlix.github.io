using System;
using System.Runtime.Serialization;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class FractionException : Exception
{
	public FractionException()
	{
	}

	public FractionException(string Message)
		: base(Message)
	{
	}

	public FractionException(string Message, Exception InnerException)
		: base(Message, InnerException)
	{
	}

	protected FractionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}
}
