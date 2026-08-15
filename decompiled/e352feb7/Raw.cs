using System;

[Serializable]
public class Raw : IMAction
{
	private string mKey;

	private object mSequence;

	public string Key
	{
		get
		{
			return mKey;
		}
		set
		{
			mKey = value;
		}
	}

	public object Sequence
	{
		get
		{
			return mSequence;
		}
		set
		{
			mSequence = value;
		}
	}
}
