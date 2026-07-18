using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal class GuidanceCloudInfo
{
	public Dictionary<string, CustomThumbnail> CustomThumbnails { get; } = new Dictionary<string, CustomThumbnail>();

	public Dictionary<GuidanceVideoType, VideoThumbnailInfo> DefaultThumbnails { get; } = new Dictionary<GuidanceVideoType, VideoThumbnailInfo>();

	public Dictionary<string, HelpArticle> HelpArticles { get; } = new Dictionary<string, HelpArticle>();

	public List<GameSetting> GameSettings { get; } = new List<GameSetting>();
}
