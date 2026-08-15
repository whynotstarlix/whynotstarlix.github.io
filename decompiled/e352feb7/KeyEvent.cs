using System;

[Serializable]
public class KeyEvent : IMAction
{
	private string mKey;

	private int mHoldTime;

	private object mKeyDownEvents;

	private object mKeyUpEvents;

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

	public int HoldTime
	{
		get
		{
			return mHoldTime;
		}
		set
		{
			mHoldTime = value;
		}
	}

	public object KeyDownEvents
	{
		get
		{
			return mKeyDownEvents;
		}
		set
		{
			mKeyDownEvents = value;
		}
	}

	public object KeyUpEvents
	{
		get
		{
			return mKeyUpEvents;
		}
		set
		{
			mKeyUpEvents = value;
		}
	}
}
