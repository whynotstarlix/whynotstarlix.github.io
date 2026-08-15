using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

[JsonObject(/*Could not decode attribute arguments.*/)]
public class QuestRule
{
	[JsonProperty("rule_id")]
	public string RuleId { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string AppPackage { get; set; } = string.Empty;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int AppUsageTime { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int MinUserInteraction { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public bool IsRecurring { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public int RecurringCount { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string CloudHandler { get; set; }
}
