using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class GuidanceCategoryEditModel : ViewModelBase
{
	private ObservableCollection<GuidanceEditModel> mGuidanceEditModels = new ObservableCollection<GuidanceEditModel>();

	private string mCategory;

	public ObservableCollection<GuidanceEditModel> GuidanceEditModels
	{
		get
		{
			return mGuidanceEditModels;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<GuidanceEditModel>>(ref mGuidanceEditModels, value, (string)null);
		}
	}

	public string Category
	{
		get
		{
			return mCategory;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mCategory, value, (string)null);
		}
	}
}
