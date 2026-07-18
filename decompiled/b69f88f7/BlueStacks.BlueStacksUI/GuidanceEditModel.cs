using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public abstract class GuidanceEditModel : ViewModelBase
{
	private string mGuidanceText;

	private bool mIsEnabled = true;

	private string mOriginalGuidanceKey;

	private string mGuidanceKey;

	private Type mPropertyType;

	private KeyActionType mActionType;

	private ObservableCollection<IMActionItem> mIMActionItems = new ObservableCollection<IMActionItem>();

	public string GuidanceText
	{
		get
		{
			return mGuidanceText;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mGuidanceText, value, (string)null);
		}
	}

	public bool IsEnabled
	{
		get
		{
			return mIsEnabled;
		}
		set
		{
			((ViewModelBase)this).SetProperty<bool>(ref mIsEnabled, value, (string)null);
		}
	}

	public string OriginalGuidanceKey
	{
		get
		{
			return mOriginalGuidanceKey;
		}
		set
		{
			mOriginalGuidanceKey = value;
			GuidanceKey = mOriginalGuidanceKey;
		}
	}

	public string GuidanceKey
	{
		get
		{
			return mGuidanceKey;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mGuidanceKey, value, (string)null);
		}
	}

	public Type PropertyType
	{
		get
		{
			return mPropertyType;
		}
		set
		{
			((ViewModelBase)this).SetProperty<Type>(ref mPropertyType, value, (string)null);
		}
	}

	public KeyActionType ActionType
	{
		get
		{
			return mActionType;
		}
		set
		{
			((ViewModelBase)this).SetProperty<KeyActionType>(ref mActionType, value, (string)null);
		}
	}

	public ObservableCollection<IMActionItem> IMActionItems
	{
		get
		{
			return mIMActionItems;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<IMActionItem>>(ref mIMActionItems, value, (string)null);
		}
	}
}
