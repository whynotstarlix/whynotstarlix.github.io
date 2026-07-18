using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

[JsonObject(/*Could not decode attribute arguments.*/)]
public class AppSuggestionPromotion
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppPackage { get; set; } = string.Empty;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppActivity { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public bool IsShowRedDot { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppName { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppIcon { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppIconId { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string ToolTip { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string CrossPromotionPackage { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppLocation { get; set; } = string.Empty;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public bool IsEmailRequired { get; set; }

	public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public bool IsAnimation { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int AnimationTime { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public bool IsIconBorder { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string IconBorderUrl { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string IconBorderClickUrl { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string IconBorderId { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string IconBorderHoverUrl { get; set; }

	public string AppIconPath { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public double IconWidth { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public double IconHeight { get; set; }
}
