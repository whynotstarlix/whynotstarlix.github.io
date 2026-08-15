using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class GuidanceViewModel : ViewModelBase
{
	private ObservableCollection<string> mGuidanceTexts = new ObservableCollection<string>();

	private ObservableCollection<string> mGuidanceKeys = new ObservableCollection<string>();

	public Type PropertyType { get; set; }

	public ObservableCollection<string> GuidanceTexts
	{
		get
		{
			return mGuidanceTexts;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<string>>(ref mGuidanceTexts, value, (string)null);
		}
	}

	public ObservableCollection<string> GuidanceKeys
	{
		get
		{
			return mGuidanceKeys;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<string>>(ref mGuidanceKeys, value, (string)null);
		}
	}
}
