using System;
using System.IO;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class BootPromotion
{
	public int Order { get; set; }

	public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

	public string Id { get; set; }

	public string ButtonText { get; set; } = string.Empty;

	public string ImagePath { get; set; } = string.Empty;

	public string ImageUrl { get; set; } = string.Empty;

	public string ThemeEnabled { get; set; } = string.Empty;

	public string ThemeName { get; set; } = string.Empty;

	public string PromoBtnClickStatusText { get; set; } = string.Empty;

	internal void DeleteFile()
	{
		try
		{
			File.Delete(ImagePath);
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't delete bootpromo file: " + ImagePath);
			Logger.Error(ex.ToString());
		}
	}
}
