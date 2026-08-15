using System;
using System.IO;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class AppRecommendation
{
	[JsonProperty(PropertyName = "extra_payload")]
	public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

	[JsonProperty(PropertyName = "app_icon_id")]
	public string IconId { get; set; }

	[JsonProperty(PropertyName = "app_icon")]
	public string Icon { get; set; }

	[JsonProperty(PropertyName = "game_genre")]
	public string GameGenre { get; set; }

	[JsonProperty(PropertyName = "app_pkg")]
	public string AppPackage { get; set; }

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
