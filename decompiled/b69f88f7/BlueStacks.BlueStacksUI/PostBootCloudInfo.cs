using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI;

public class PostBootCloudInfo
{
	public NotificationModeInfo GameNotificationAppPackages { get; set; } = new NotificationModeInfo();

	public OnBoardingInfo OnBoardingInfo { get; set; } = new OnBoardingInfo();

	public AppSpecificCustomCursorInfo AppSpecificCustomCursorInfo { get; set; } = new AppSpecificCustomCursorInfo();

	public List<string> IgnoredActivitiesForTabs { get; } = new List<string>();

	public GameAwareOnboardingInfo GameAwareOnboardingInfo { get; set; } = new GameAwareOnboardingInfo();
}
