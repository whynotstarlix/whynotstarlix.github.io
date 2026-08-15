using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class CustomThumbnail
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public VideoThumbnailInfo Pan { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public VideoThumbnailInfo Moba { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public VideoThumbnailInfo Gamepad { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public VideoThumbnailInfo Special { get; set; }

	[JsonProperty(PropertyName = "package")]
	public string Package { get; set; }

	[JsonProperty(PropertyName = "schemespecific")]
	public Dictionary<string, VideoThumbnailInfo> SchemeSpecific { get; set; } = new Dictionary<string, VideoThumbnailInfo>();

	public object this[string propertyName] => typeof(CustomThumbnail).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)?.GetValue(this, null);
}
