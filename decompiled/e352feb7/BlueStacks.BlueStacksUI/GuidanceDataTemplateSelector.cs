using System.Windows;
using System.Windows.Controls;

namespace BlueStacks.BlueStacksUI;

public class GuidanceDataTemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		FrameworkElement val = (FrameworkElement)(object)((container is FrameworkElement) ? container : null);
		if (val != null && item != null)
		{
			if (item is GuidanceViewModel)
			{
				object obj = val.FindResource((object)"GuidanceViewModelTemplate");
				return (DataTemplate)((obj is DataTemplate) ? obj : null);
			}
			if (item is GuidanceEditTextModel)
			{
				object obj2 = val.FindResource((object)"GuidanceEditTextModelTemplate");
				return (DataTemplate)((obj2 is DataTemplate) ? obj2 : null);
			}
			if (item is GuidanceEditDecimalModel)
			{
				object obj3 = val.FindResource((object)"GuidanceEditDecimalModelTemplate");
				return (DataTemplate)((obj3 is DataTemplate) ? obj3 : null);
			}
			if (item is GuidanceCategoryViewModel)
			{
				object obj4 = val.FindResource((object)"GuidanceCategoryViewModelTemplate");
				return (DataTemplate)((obj4 is DataTemplate) ? obj4 : null);
			}
			if (item is GuidanceCategoryEditModel)
			{
				object obj5 = val.FindResource((object)"GuidanceCategoryEditModelTemplate");
				return (DataTemplate)((obj5 is DataTemplate) ? obj5 : null);
			}
		}
		return null;
	}
}
