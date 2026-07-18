using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class GuidanceData
{
	private ObservableCollection<GuidanceCategoryEditModel> mKeymapEditGuidanceCloned;

	private ObservableCollection<GuidanceCategoryEditModel> mGamepadEditGuidanceCloned;

	private Dictionary<string, Dictionary<Type, Dictionary<string, string>>> mViewIgnoreList = new Dictionary<string, Dictionary<Type, Dictionary<string, string>>>
	{
		{
			"KeyMap",
			new Dictionary<Type, Dictionary<string, string>> { 
			{
				typeof(Dpad),
				new Dictionary<string, string>
				{
					{ "KeyUp", "DpadTitle" },
					{ "KeyLeft", "DpadTitle" },
					{ "KeyDown", "DpadTitle" },
					{ "KeyRight", "DpadTitle" }
				}
			} }
		},
		{
			"GamePad",
			new Dictionary<Type, Dictionary<string, string>>()
		}
	};

	private Dictionary<string, Dictionary<Type, List<string>>> mEditIgnoreList = new Dictionary<string, Dictionary<Type, List<string>>>
	{
		{
			"KeyMap",
			new Dictionary<Type, List<string>> { 
			{
				typeof(Dpad),
				new List<string> { "DpadTitle" }
			} }
		},
		{
			"GamePad",
			new Dictionary<Type, List<string>>()
		}
	};

	public ObservableCollection<GuidanceCategoryViewModel> KeymapViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

	public ObservableCollection<GuidanceCategoryViewModel> GamepadViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

	public ObservableCollection<GuidanceCategoryEditModel> KeymapEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

	public ObservableCollection<GuidanceCategoryEditModel> GamepadEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

	public void Clear()
	{
		KeymapViewGuidance.Clear();
		GamepadViewGuidance.Clear();
		KeymapEditGuidance.Clear();
		GamepadEditGuidance.Clear();
		mKeymapEditGuidanceCloned = null;
		mGamepadEditGuidanceCloned = null;
	}

	public void SaveOriginalData()
	{
		mKeymapEditGuidanceCloned = UsefulExtensionMethod.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>(KeymapEditGuidance);
		mGamepadEditGuidanceCloned = UsefulExtensionMethod.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>(GamepadEditGuidance);
	}

	public void Reset()
	{
		KeymapEditGuidance = UsefulExtensionMethod.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>(mKeymapEditGuidanceCloned);
		GamepadEditGuidance = UsefulExtensionMethod.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>(mGamepadEditGuidanceCloned);
	}

	public void AddGuidance(bool isGamePad, string category, string guidanceText, string guidanceKey, string actionItem, IMAction imAction)
	{
		if (imAction == null || string.IsNullOrEmpty(guidanceKey) || string.IsNullOrEmpty(actionItem))
		{
			return;
		}
		Type propertyType = IMAction.DictPropertyInfo[imAction.Type][actionItem].PropertyType;
		if ((object)propertyType == typeof(double) && double.TryParse(guidanceKey, out var result))
		{
			guidanceKey = result.ToString(CultureInfo.InvariantCulture);
		}
		if (imAction is EdgeScroll && actionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
		{
			guidanceKey = (Convert.ToBoolean(guidanceKey, CultureInfo.InvariantCulture) ? "ON" : "OFF");
		}
		string key = (isGamePad ? "GamePad" : "KeyMap");
		if (!mViewIgnoreList[key].ContainsKey(imAction.GetType()) || !mViewIgnoreList[key][imAction.GetType()].ContainsKey(actionItem) || !imAction.Guidance.ContainsKey(mViewIgnoreList[key][imAction.GetType()][actionItem]))
		{
			ObservableCollection<GuidanceCategoryViewModel> observableCollection = (isGamePad ? GamepadViewGuidance : KeymapViewGuidance);
			GuidanceCategoryViewModel guidanceCategoryViewModel = observableCollection.Where((GuidanceCategoryViewModel guide) => string.Equals(guide.Category, category, StringComparison.InvariantCulture)).FirstOrDefault();
			if (guidanceCategoryViewModel == null)
			{
				GuidanceViewModel guidanceViewModel = new GuidanceViewModel
				{
					PropertyType = propertyType
				};
				guidanceViewModel.GuidanceTexts.Add(guidanceText);
				guidanceViewModel.GuidanceKeys.Add(guidanceKey);
				guidanceCategoryViewModel = new GuidanceCategoryViewModel
				{
					Category = category
				};
				guidanceCategoryViewModel.GuidanceViewModels.Add(guidanceViewModel);
				observableCollection.Add(guidanceCategoryViewModel);
			}
			else
			{
				if ((object)propertyType != typeof(double))
				{
					GuidanceViewModel guidanceViewModel2 = guidanceCategoryViewModel.GuidanceViewModels.Where((GuidanceViewModel guide) => (object)guide.PropertyType != typeof(double) && guide.GuidanceTexts.Count == 1 && guide.GuidanceTexts.Contains(guidanceText)).FirstOrDefault();
					if (guidanceViewModel2 != null)
					{
						UsefulExtensionMethod.AddIfNotContain<string>((IList<string>)guidanceViewModel2.GuidanceKeys, guidanceKey);
						goto IL_0352;
					}
				}
				if ((object)propertyType != typeof(double))
				{
					GuidanceViewModel guidanceViewModel3 = guidanceCategoryViewModel.GuidanceViewModels.Where((GuidanceViewModel guide) => (object)guide.PropertyType != typeof(double) && guide.GuidanceKeys.Count == 1 && guide.GuidanceKeys.Contains(guidanceKey)).FirstOrDefault();
					if (guidanceViewModel3 != null)
					{
						UsefulExtensionMethod.AddIfNotContain<string>((IList<string>)guidanceViewModel3.GuidanceTexts, guidanceText);
						goto IL_0352;
					}
				}
				GuidanceViewModel guidanceViewModel4 = new GuidanceViewModel
				{
					PropertyType = propertyType
				};
				guidanceViewModel4.GuidanceTexts.Add(guidanceText);
				guidanceViewModel4.GuidanceKeys.Add(guidanceKey);
				guidanceCategoryViewModel.GuidanceViewModels.Add(guidanceViewModel4);
			}
		}
		goto IL_0352;
		IL_0352:
		if (mEditIgnoreList[key].ContainsKey(imAction.GetType()) && mEditIgnoreList[key][imAction.GetType()].Contains(actionItem))
		{
			return;
		}
		ObservableCollection<GuidanceCategoryEditModel> observableCollection2 = (isGamePad ? GamepadEditGuidance : KeymapEditGuidance);
		GuidanceCategoryEditModel guidanceCategoryEditModel = observableCollection2.Where((GuidanceCategoryEditModel guide) => string.Equals(guide.Category, category, StringComparison.InvariantCulture)).FirstOrDefault();
		if (guidanceCategoryEditModel == null)
		{
			guidanceCategoryEditModel = new GuidanceCategoryEditModel
			{
				Category = category
			};
			observableCollection2.Add(guidanceCategoryEditModel);
		}
		GuidanceEditModel guidanceEditModel = null;
		if ((object)propertyType == typeof(string))
		{
			guidanceEditModel = guidanceCategoryEditModel.GuidanceEditModels.Where((GuidanceEditModel gem) => gem.ActionType == imAction.Type && (object)gem.PropertyType == propertyType && string.Equals(gem.GuidanceText, guidanceText, StringComparison.InvariantCultureIgnoreCase) && string.Equals(gem.GuidanceKey, guidanceKey, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
		}
		if (guidanceEditModel == null)
		{
			guidanceEditModel = (((object)propertyType == typeof(string) || (object)propertyType == typeof(bool)) ? ((GuidanceEditModel)new GuidanceEditTextModel()) : ((GuidanceEditModel)new GuidanceEditDecimalModel()));
			guidanceEditModel.GuidanceText = guidanceText;
			guidanceEditModel.OriginalGuidanceKey = guidanceKey;
			guidanceEditModel.ActionType = imAction.Type;
			guidanceEditModel.PropertyType = propertyType;
			guidanceEditModel.IsEnabled = !string.Equals(actionItem, "KeyAction", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(actionItem, "KeyMove", StringComparison.InvariantCultureIgnoreCase);
			guidanceCategoryEditModel.GuidanceEditModels.Add(guidanceEditModel);
		}
		guidanceEditModel.IMActionItems.Add(new IMActionItem
		{
			ActionItem = actionItem,
			IMAction = imAction
		});
	}
}
