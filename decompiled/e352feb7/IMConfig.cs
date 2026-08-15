using System;
using System.Collections.Generic;
using System.Linq;
using BlueStacks.Common;
using Newtonsoft.Json;

[Serializable]
internal class IMConfig
{
	public MetaData MetaData { get; set; } = new MetaData();

	public List<IMControlScheme> ControlSchemes { get; set; } = new List<IMControlScheme>();

	[JsonIgnore]
	public Dictionary<string, IMControlScheme> ControlSchemesDict { get; private set; } = new Dictionary<string, IMControlScheme>();

	public Dictionary<string, Dictionary<string, string>> Strings { get; set; } = new Dictionary<string, Dictionary<string, string>>();

	[JsonIgnore]
	public IMControlScheme SelectedControlScheme { get; set; } = new IMControlScheme();

	internal string GetUIString(string key)
	{
		string result = key;
		if (Strings.ContainsKey(LocaleStrings.Locale) && Strings[LocaleStrings.Locale].ContainsKey(key))
		{
			result = Strings[LocaleStrings.Locale][key];
		}
		else if (Strings.ContainsKey("en-US") && Strings["en-US"].ContainsKey(key))
		{
			result = Strings["en-US"][key];
		}
		else if (Strings.ContainsKey("User-Defined") && Strings["User-Defined"].ContainsKey(key))
		{
			result = Strings["User-Defined"][key];
		}
		return result;
	}

	internal void AddString(string key)
	{
		if (!Strings.ContainsKey("User-Defined"))
		{
			Strings.Add("User-Defined", new Dictionary<string, string>());
		}
		Strings["User-Defined"][key] = key;
	}

	public IMConfig DeepCopy()
	{
		IMConfig obj = (IMConfig)MemberwiseClone();
		MetaData metaData = MetaData;
		obj.MetaData = ((metaData != null) ? UsefulExtensionMethod.DeepCopy<MetaData>(metaData) : null);
		obj.ControlSchemes = ControlSchemes?.ConvertAll((IMControlScheme cs) => cs?.DeepCopy());
		obj.ControlSchemesDict = ControlSchemesDict?.ToDictionary((KeyValuePair<string, IMControlScheme> kvp) => kvp.Key, (KeyValuePair<string, IMControlScheme> kvp) => kvp.Value?.DeepCopy());
		obj.Strings = Strings?.ToDictionary((KeyValuePair<string, Dictionary<string, string>> kvp) => kvp.Key, (KeyValuePair<string, Dictionary<string, string>> kvp) => kvp.Value);
		obj.SelectedControlScheme = SelectedControlScheme?.DeepCopy();
		return obj;
	}
}
