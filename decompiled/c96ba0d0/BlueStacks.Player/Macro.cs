using System;
using BlueStacks.Common;

namespace BlueStacks.Player;

[Serializable]
public class Macro
{
	private string mMacroName;

	private RepeatBehaviour mAndroidCommandRepeatMode;

	private RepeatBehaviour mRepeatBehaiour;

	private SerializableDictionary<int, MacroAction> mDictMacroActions = new SerializableDictionary<int, MacroAction>();

	private SerializableDictionary<string, SerializableDictionary<string, string>> mDictAndroidCommands = new SerializableDictionary<string, SerializableDictionary<string, string>>();

	public string MacroName
	{
		get
		{
			return mMacroName;
		}
		set
		{
			mMacroName = value;
		}
	}

	public RepeatBehaviour AndroidCommandRepeatMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAndroidCommandRepeatMode;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mAndroidCommandRepeatMode = value;
		}
	}

	public RepeatBehaviour RepeatBehaiour
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mRepeatBehaiour;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			mRepeatBehaiour = value;
		}
	}

	public SerializableDictionary<int, MacroAction> DictMacroActions
	{
		get
		{
			return mDictMacroActions;
		}
		set
		{
			mDictMacroActions = value;
		}
	}

	public SerializableDictionary<string, SerializableDictionary<string, string>> DictAndroidCommands
	{
		get
		{
			return mDictAndroidCommands;
		}
		set
		{
			mDictAndroidCommands = value;
		}
	}
}
