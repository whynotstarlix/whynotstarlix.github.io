using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

internal class GameSetting
{
	[JsonProperty(PropertyName = "setting_type")]
	public string SettingType { get; set; }

	public List<Dictionary<string, object>> SettingsData { get; set; } = new List<Dictionary<string, object>>();
}
