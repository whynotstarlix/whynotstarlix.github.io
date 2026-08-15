using System.Runtime.CompilerServices;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AppIconPromotionObject
{
	[CompilerGenerated]
	private GenericAction _003CAppPromotionAction_003Ek__BackingField;

	public string AppPromotionID { get; set; }

	public GenericAction AppPromotionAction
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CAppPromotionAction_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CAppPromotionAction_003Ek__BackingField = value;
		}
	}

	public string AppPromotionPackage { get; set; }

	public string AppPromotionName { get; set; }

	public string AppPromotionActionParam { get; set; }

	public string AppPromotionImagePath { get; set; }

	public AppIconPromotionObject()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		AppPromotionID = string.Empty;
		AppPromotionAction = (GenericAction)1;
		AppPromotionPackage = string.Empty;
		AppPromotionName = string.Empty;
		AppPromotionActionParam = string.Empty;
		AppPromotionImagePath = string.Empty;
		base._002Ector();
	}
}
