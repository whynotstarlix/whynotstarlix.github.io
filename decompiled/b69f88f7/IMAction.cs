using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using BlueStacks.Common;

[Serializable]
public abstract class IMAction
{
	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> sDictDevModeUIElements;

	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPropertyInfo;

	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPopUpUIElements;

	private string mGuidanceCategory = "MISC";

	internal Direction Direction;

	internal IMAction ParentAction;

	private static Dictionary<KeyActionType, string> sPositionXPropertyName;

	private static Dictionary<KeyActionType, string> sPositionYPropertyName;

	internal static Dictionary<KeyActionType, string> sRadiusPropertyName;

	internal bool IsChildAction;

	public KeyActionType Type { get; set; }

	public Dictionary<string, string> Guidance { get; } = new Dictionary<string, string>();

	public string GuidanceCategory
	{
		get
		{
			string text = ((!IsChildAction) ? GetCurrentGuidanceCategory() : ((ParentAction == this) ? GetCurrentGuidanceCategory() : ParentAction.GuidanceCategory));
			Console.WriteLine("[LOG] Get GuidanceCategory: " + text);
			return text;
		}
		set
		{
			mGuidanceCategory = ((value == null || string.IsNullOrEmpty(value.Trim())) ? "MISC" : value.Trim());
			Console.WriteLine("[LOG] Set GuidanceCategory: " + mGuidanceCategory);
		}
	}

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool Exclusive { get; set; }

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int ExclusiveDelay { get; set; } = 200;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string XExpr { get; set; } = string.Empty;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string YExpr { get; set; } = string.Empty;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string XOverlayOffset { get; set; } = string.Empty;

	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string YOverlayOffset { get; set; } = string.Empty;

	public string EnableCondition { get; set; } = string.Empty;

	public string StartCondition { get; set; }

	public string Note { get; set; }

	public string Comment { get; set; }

	internal double PositionX
	{
		get
		{
			if (!double.TryParse(this[sPositionXPropertyName[Type]].ToString(), out var result))
			{
				return -1.0;
			}
			return result;
		}
		set
		{
			this[sPositionXPropertyName[Type]] = value;
		}
	}

	internal double PositionY
	{
		get
		{
			if (!double.TryParse(this[sPositionYPropertyName[Type]].ToString(), out var result))
			{
				return -1.0;
			}
			return result;
		}
		set
		{
			this[sPositionYPropertyName[Type]] = value;
		}
	}

	internal double RadiusProperty
	{
		get
		{
			if (!double.TryParse(this[sRadiusPropertyName[Type]].ToString(), out var result))
			{
				return -1.0;
			}
			return result;
		}
		set
		{
			this[sRadiusPropertyName[Type]] = value;
		}
	}

	public bool IsVisibleInOverlay
	{
		get
		{
			if (!bool.TryParse(this["ShowOnOverlay"].ToString(), out var result))
			{
				return false;
			}
			return result;
		}
		set
		{
			this["ShowOnOverlay"] = value;
		}
	}

	public object this[string propertyName]
	{
		get
		{
			object obj = null;
			if (GetPropertyInfo(propertyName) != null)
			{
				obj = GetPropertyInfo(propertyName).GetValue(this, null);
			}
			Console.WriteLine(string.Format("[LOG] Get property '{0}': {1}", propertyName, obj ?? "null"));
			return obj ?? string.Empty;
		}
		set
		{
			try
			{
				PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
				if (propertyInfo != null)
				{
					object obj = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
					propertyInfo.SetValue(this, obj, null);
					Console.WriteLine($"[LOG] Set property '{propertyName}' to '{obj}'");
				}
			}
			catch (Exception arg)
			{
				Console.WriteLine($"[ERROR] Error setting property '{propertyName}': {arg}");
			}
		}
	}

	public IMAction()
	{
		Console.WriteLine("[LOG] IMAction created: " + GetType().Name);
		GetPropertyInfo("Type");
		if (ParentAction == null)
		{
			ParentAction = this;
		}
	}

	private string GetCurrentGuidanceCategory()
	{
		if (string.IsNullOrEmpty(mGuidanceCategory))
		{
			mGuidanceCategory = "MISC";
		}
		return mGuidanceCategory;
	}

	private PropertyInfo GetPropertyInfo(string propertyName)
	{
		PropertyInfo propertyInfo = null;
		KeyActionType key = (Type = EnumHelper.Parse<KeyActionType>(GetType().Name, KeyActionType.Alias));
		if (!DictPropertyInfo.ContainsKey(key))
		{
			DictPropertyInfo[key] = new Dictionary<string, PropertyInfo>();
			DictPopUpUIElements[key] = new Dictionary<string, PropertyInfo>();
			sDictDevModeUIElements[key] = new Dictionary<string, PropertyInfo>();
			sPositionXPropertyName[key] = string.Empty;
			sPositionYPropertyName[key] = string.Empty;
			sRadiusPropertyName[key] = string.Empty;
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo2 in properties)
			{
				DictPropertyInfo[key].Add(propertyInfo2.Name, propertyInfo2);
				object[] customAttributes = propertyInfo2.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
				if (customAttributes.Length != 0)
				{
					DescriptionAttribute obj = customAttributes[0] as DescriptionAttribute;
					if (obj.Description.Contains("IMAP_CanvasElementY"))
					{
						sPositionXPropertyName[key] = propertyInfo2.Name;
					}
					if (obj.Description.Contains("IMAP_CanvasElementX"))
					{
						sPositionYPropertyName[key] = propertyInfo2.Name;
					}
					if (obj.Description.Contains("IMAP_CanvasElementRadius"))
					{
						sRadiusPropertyName[key] = propertyInfo2.Name;
					}
					if (obj.Description.Contains("IMAP_PopupUIElement"))
					{
						DictPopUpUIElements[key].Add(propertyInfo2.Name, propertyInfo2);
					}
					if (obj.Description.Contains("IMAP_DeveloperModeUIElemnt"))
					{
						sDictDevModeUIElements[key].Add(propertyInfo2.Name, propertyInfo2);
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(propertyName) && DictPropertyInfo[key].ContainsKey(propertyName))
		{
			propertyInfo = DictPropertyInfo[key][propertyName];
		}
		Console.WriteLine("[LOG] GetPropertyInfo('" + propertyName + "') => " + ((propertyInfo != null) ? propertyInfo.Name : "null"));
		return propertyInfo;
	}

	internal List<Tuple<string, IMAction>> GetListGuidanceElements()
	{
		List<string> list = new List<string>();
		List<Tuple<string, IMAction>> list2 = new List<Tuple<string, IMAction>>();
		foreach (KeyValuePair<string, string> item in Guidance)
		{
			if (item.Key.StartsWith("Key", StringComparison.InvariantCulture))
			{
				list2.Add(new Tuple<string, IMAction>(item.Key, this));
				list.Add(item.Key);
			}
		}
		foreach (KeyValuePair<string, PropertyInfo> item2 in DictPropertyInfo[Type])
		{
			if (!list.Contains(item2.Key) && (item2.Key.StartsWith("Key", StringComparison.InvariantCulture) || item2.Key.StartsWith("Sensitivity", StringComparison.InvariantCulture) || item2.Key.StartsWith("MouseAcceleration", StringComparison.InvariantCulture) || item2.Key.StartsWith("EdgeScrollEnabled", StringComparison.InvariantCulture)))
			{
				list2.Add(new Tuple<string, IMAction>(item2.Key, this));
			}
		}
		return list2;
	}

	static IMAction()
	{
		sDictDevModeUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
		DictPropertyInfo = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
		DictPopUpUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();
		sPositionXPropertyName = new Dictionary<KeyActionType, string>();
		sPositionYPropertyName = new Dictionary<KeyActionType, string>();
		sRadiusPropertyName = new Dictionary<KeyActionType, string>();
	}
}
