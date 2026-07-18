using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.BlueStacksUI;

public class GuidanceListToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is IEnumerable<string> enumerable)
		{
			List<string> list = new List<string>();
			foreach (string item in enumerable)
			{
				list.Add(KMManager.GetKeyUIValue(item));
			}
			return string.Join(" / ", list.ToArray());
		}
		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return Binding.DoNothing;
	}
}
