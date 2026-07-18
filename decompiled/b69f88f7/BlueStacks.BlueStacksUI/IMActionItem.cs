using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class IMActionItem : ViewModelBase
{
	private string mActionItem;

	private IMAction mIMAction;

	public string ActionItem
	{
		get
		{
			return mActionItem;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mActionItem, value, (string)null);
		}
	}

	public IMAction IMAction
	{
		get
		{
			return mIMAction;
		}
		set
		{
			((ViewModelBase)this).SetProperty<IMAction>(ref mIMAction, value, (string)null);
		}
	}
}
