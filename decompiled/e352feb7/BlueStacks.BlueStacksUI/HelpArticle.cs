using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class HelpArticle
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public HelpArticleInfo Gamepad { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public HelpArticleInfo Moba { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public HelpArticleInfo Pan { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public HelpArticleInfo Special { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public HelpArticleInfo Default { get; set; }

	[JsonProperty(PropertyName = "package")]
	public string Package { get; set; }

	[JsonProperty(PropertyName = "schemespecific")]
	public Dictionary<string, HelpArticleInfo> SchemeSpecific { get; set; } = new Dictionary<string, HelpArticleInfo>();

	public object this[string propertyName] => typeof(HelpArticle).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(this, null);
}
