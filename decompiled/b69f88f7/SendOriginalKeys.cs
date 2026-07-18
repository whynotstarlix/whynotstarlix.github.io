using System;

[Serializable]
public class SendOriginalKeys : IMAction
{
	private string mComments;

	public string Comments
	{
		get
		{
			return mComments;
		}
		set
		{
			mComments = value;
		}
	}
}
