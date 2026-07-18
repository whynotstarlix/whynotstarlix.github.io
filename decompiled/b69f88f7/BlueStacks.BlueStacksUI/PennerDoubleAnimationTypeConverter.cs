using System;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

public class PennerDoubleAnimationTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return (object)sourceType == typeof(string);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return (object)destinationType == typeof(Enum);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		foreach (int value2 in Enum.GetValues(typeof(PennerDoubleAnimation.Equations)))
		{
			if (Enum.GetName(typeof(PennerDoubleAnimation.Equations), value2) == value?.ToString())
			{
				return (PennerDoubleAnimation.Equations)value2;
			}
		}
		return null;
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value != null)
		{
			return ((PennerDoubleAnimation.Equations)value/*cast due to constrained. prefix*/).ToString();
		}
		return null;
	}
}
