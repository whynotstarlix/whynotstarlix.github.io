using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

internal class BootBanner
{
	[JsonProperty("frequency")]
	public string Frequency { get; set; }

	[JsonProperty("click_action_packagename")]
	public string ClickActionPackagename { get; set; }

	[JsonProperty("click_generic_action")]
	public string ClickGenericAction { get; set; }

	[JsonProperty("click_action_value")]
	public string ClickActionValue { get; set; }

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("button_text")]
	public string ButtonText { get; set; }

	[JsonProperty("order")]
	public string Order { get; set; }

	[JsonProperty("image_url")]
	public string ImageUrl { get; set; }

	[JsonProperty("hash_tags")]
	public string HashTags { get; set; }
}
