using System;
using System.IO;
using System.Xml.Serialization;
using BlueStacks.Common;

namespace BlueStacks.Player;

[Serializable]
public class MacroData
{
	public string mPackageName;

	private static MacroData sInstance;

	private SerializableDictionary<string, Macro> mDictMacros = new SerializableDictionary<string, Macro>();

	internal static MacroData Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new MacroData();
			}
			return sInstance;
		}
	}

	internal SerializableDictionary<string, Macro> DictMacros
	{
		get
		{
			return mDictMacros;
		}
		set
		{
			mDictMacros = value;
		}
	}

	private MacroData()
	{
	}

	internal void LoadMacroData(string packageName)
	{
		mPackageName = packageName;
		string macroFileName = InputMapper.Instance.GetMacroFileName(isUserDirectoryRequested: false, mPackageName);
		if (File.Exists(macroFileName))
		{
			StringReader textReader = new StringReader(File.ReadAllText(macroFileName));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializableDictionary<string, Macro>));
			mDictMacros = (SerializableDictionary<string, Macro>)xmlSerializer.Deserialize(textReader);
		}
		else
		{
			mDictMacros = new SerializableDictionary<string, Macro>();
		}
	}

	internal void SaveMacroData()
	{
		string macroFileName = InputMapper.Instance.GetMacroFileName(isUserDirectoryRequested: true, InputMapper.Instance.GetPackage());
		StringWriter stringWriter = new StringWriter();
		new XmlSerializer(typeof(SerializableDictionary<string, Macro>)).Serialize(stringWriter, mDictMacros);
		File.WriteAllText(macroFileName, stringWriter.ToString());
	}
}
