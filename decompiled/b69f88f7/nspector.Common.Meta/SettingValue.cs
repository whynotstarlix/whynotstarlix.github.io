namespace nspector.Common.Meta;

internal class SettingValue<T>
{
	public SettingMetaSource ValueSource;

	public int ValuePos { get; set; }

	public string ValueName { get; set; }

	public T Value { get; set; }

	public SettingValue(SettingMetaSource source)
	{
		ValueSource = source;
	}

	public override string ToString()
	{
		if (typeof(T) == typeof(uint))
		{
			return $"Value=0x{Value:X8}; ValueName={ValueName}; Source={ValueSource};";
		}
		return $"Value={Value}; ValueName={ValueName};";
	}
}
