using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

internal class DateTimeHelper
{
	internal static string GetReadableDateTimeString(DateTime yourDate)
	{
		if (yourDate.ToLocalTime().Date == DateTime.Now.Date)
		{
			return "Today at " + yourDate.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture);
		}
		if (yourDate.ToLocalTime().Date.Year == DateTime.Now.Date.Year)
		{
			return yourDate.ToLocalTime().ToString("%d MMM',' HH:mm", CultureInfo.InvariantCulture);
		}
		return yourDate.ToLocalTime().ToString("%d MMM yyyy',' HH:mm", CultureInfo.InvariantCulture);
	}
}
