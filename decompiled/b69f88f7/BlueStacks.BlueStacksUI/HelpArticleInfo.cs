using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class HelpArticleInfo
{
	[JsonProperty(PropertyName = "url")]
	public string HelpArticleUrl { get; set; }
}
