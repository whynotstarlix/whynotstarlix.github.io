using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class VideoThumbnailInfo
{
	[CompilerGenerated]
	private GuidanceVideoType _003CThumbnailType_003Ek__BackingField;

	[JsonProperty(PropertyName = "thumbnail_id")]
	public string ThumbnailId { get; set; }

	[JsonProperty(PropertyName = "thumbnail_url")]
	public string ThumbnailUrl { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public GuidanceVideoType ThumbnailType
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CThumbnailType_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CThumbnailType_003Ek__BackingField = value;
		}
	}

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
