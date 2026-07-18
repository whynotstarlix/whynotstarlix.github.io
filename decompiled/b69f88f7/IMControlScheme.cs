using System;
using System.Collections.Generic;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

[Serializable]
public class IMControlScheme
{
	public List<IMAction> GameControls { get; private set; } = new List<IMAction>();

	public List<JObject> Images { get; private set; } = new List<JObject>();

	public string Name { get; set; }

	public bool BuiltIn { get; set; }

	public bool Selected { get; set; }

	public bool IsBookMarked { get; set; }

	public bool IsCategoryVisible { get; set; } = true;

	public string KeyboardLayout { get; set; } = InteropWindow.MapLayoutName((string)null);

	public IMControlScheme DeepCopy()
	{
		IMControlScheme obj = (IMControlScheme)MemberwiseClone();
		List<IMAction> gameControls = GameControls;
		obj.SetGameControls((gameControls != null) ? UsefulExtensionMethod.DeepCopy<List<IMAction>>(gameControls) : null);
		obj.SetImages(Images.ConvertAll((Converter<JObject, JObject>)((JObject jt) => (JObject)((jt != null) ? ((JToken)jt).DeepClone() : null))));
		return obj;
	}

	public void SetGameControls(List<IMAction> gameControls)
	{
		GameControls = gameControls;
	}

	public void SetImages(List<JObject> images)
	{
		Images = images?.ConvertAll((Converter<JObject, JObject>)((JObject jt) => (JObject)((jt != null) ? ((JToken)jt).DeepClone() : null)));
	}
}
