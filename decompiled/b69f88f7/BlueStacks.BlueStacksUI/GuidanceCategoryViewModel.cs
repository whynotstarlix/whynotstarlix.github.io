using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class GuidanceCategoryViewModel : ViewModelBase
{
	private ObservableCollection<GuidanceViewModel> sGuidanceViewModels = new ObservableCollection<GuidanceViewModel>();

	private string mCategory;

	public ObservableCollection<GuidanceViewModel> GuidanceViewModels
	{
		get
		{
			return sGuidanceViewModels;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<GuidanceViewModel>>(ref sGuidanceViewModels, value, (string)null);
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
