using System;

[Serializable]
public class KeyInput : IMAction
{
	private string mKeyIn;

	private string mKeyOut;

	public string KeyIn
	{
		get
		{
			return mKeyIn;
		}
		set
		{
			mKeyIn = value;
		}
	}

	public string KeyOut
	{
		get
		{
			return mKeyOut;
		}
		set
		{
			mKeyOut = value;
		}
	}
}
