using System.Text;

namespace nspector.Common;

internal class CachedSettingValue
{
	internal string ValueStr = "";

	internal uint Value = 0u;

	internal byte[] ValueBin = new byte[0];

	internal StringBuilder ProfileNames;

	internal uint ValueProfileCount;

	internal CachedSettingValue()
	{
	}

	internal CachedSettingValue(uint Value, string ProfileNames)
	{
		this.Value = Value;
		this.ProfileNames = new StringBuilder(ProfileNames);
		ValueProfileCount = 1u;
	}

	internal CachedSettingValue(string ValueStr, string ProfileNames)
	{
		this.ValueStr = ValueStr;
		this.ProfileNames = new StringBuilder(ProfileNames);
		ValueProfileCount = 1u;
	}

	internal CachedSettingValue(byte[] ValueBin, string ProfileNames)
	{
		this.ValueBin = ValueBin;
		this.ProfileNames = new StringBuilder(ProfileNames);
		ValueProfileCount = 1u;
	}
}
