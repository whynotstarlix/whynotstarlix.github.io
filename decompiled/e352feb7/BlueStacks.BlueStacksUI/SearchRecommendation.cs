using System;
using System.IO;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SearchRecommendation
{
	public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

	public string IconId { get; set; }

	public string ImagePath { get; set; } = string.Empty;

	internal void DeleteFile()
	{
		try
		{
			File.Delete(ImagePath);
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't delete SearchRecommendation file: " + ImagePath);
			Logger.Error(ex.ToString());
		}
	}
}
