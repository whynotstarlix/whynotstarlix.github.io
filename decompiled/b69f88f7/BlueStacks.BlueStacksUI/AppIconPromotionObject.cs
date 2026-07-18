using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AppIconPromotionObject
{
	public string AppPromotionID { get; set; } = string.Empty;

	public GenericAction AppPromotionAction { get; set; } = (GenericAction)1;

	public string AppPromotionPackage { get; set; } = string.Empty;

	public string AppPromotionName { get; set; } = string.Empty;

	public string AppPromotionActionParam { get; set; } = string.Empty;

	public string AppPromotionImagePath { get; set; } = string.Empty;
}
