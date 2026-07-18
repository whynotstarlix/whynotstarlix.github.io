using System;
using System.IO;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class VideoThumbnailInfo
{
	[JsonProperty(PropertyName = "thumbnail_id")]
	public string ThumbnailId { get; set; }

	[JsonProperty(PropertyName = "thumbnail_url")]
	public string ThumbnailUrl { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public GuidanceVideoType ThumbnailType { get; set; }

	public string ImagePath { get; set; } = string.Empty;

	internal void DeleteFile()
	{
		try
		{
			File.Delete(ImagePath);
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't delete AppRecommendation file: " + ImagePath);
			Logger.Error(ex.ToString());
		}
	}
}
