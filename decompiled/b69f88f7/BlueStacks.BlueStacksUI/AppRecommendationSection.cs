using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class AppRecommendationSection
{
	[JsonProperty(PropertyName = "section_header")]
	public string AppSuggestionHeader { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	[DefaultValue(3)]
	public int ClientShowCount { get; set; } = 3;

	[JsonProperty(PropertyName = "suggested_apps")]
	public List<AppRecommendation> AppSuggestions { get; } = new List<AppRecommendation>();
}
