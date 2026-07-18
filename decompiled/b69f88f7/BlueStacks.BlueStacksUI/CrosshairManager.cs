using System.Windows;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI;

public static class CrosshairManager
{
	public static CrosshairWindow CurrentCrosshair { get; private set; }

	public static void ApplyCrosshair(Color color, double thickness, double length, double gap, double opacity, bool tShape, bool showCenterDot, bool showCross)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (CurrentCrosshair != null)
		{
			((Window)CurrentCrosshair).Close();
			CurrentCrosshair = null;
		}
		CurrentCrosshair = new CrosshairWindow(color, thickness, length, gap, opacity, tShape, showCenterDot, showCross);
		((Window)CurrentCrosshair).Show();
	}
}
