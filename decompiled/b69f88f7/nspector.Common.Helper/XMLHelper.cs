using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace nspector.Common.Helper;

internal static class XMLHelper<T> where T : new()
{
	private static XmlSerializer xmlSerializer;

	static XMLHelper()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		xmlSerializer = new XmlSerializer(typeof(T));
	}

	internal static string SerializeToXmlString(T xmlObject, Encoding encoding, bool removeNamespace)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		MemoryStream memoryStream = new MemoryStream();
		XmlTextWriter val = new XmlTextWriter((Stream)memoryStream, encoding)
		{
			Formatting = (Formatting)1
		};
		if (removeNamespace)
		{
			XmlSerializerNamespaces val2 = new XmlSerializerNamespaces();
			val2.Add("", "");
			xmlSerializer.Serialize((XmlWriter)(object)val, (object)xmlObject, val2);
		}
		else
		{
			xmlSerializer.Serialize((XmlWriter)(object)val, (object)xmlObject);
		}
		return encoding.GetString(memoryStream.ToArray());
	}

	internal static void SerializeToXmlFile(T xmlObject, string filename, Encoding encoding, bool removeNamespace)
	{
		File.WriteAllText(filename, SerializeToXmlString(xmlObject, encoding, removeNamespace));
	}

	internal static T DeserializeFromXmlString(string xml)
	{
		StringReader stringReader = new StringReader(xml);
		return (T)xmlSerializer.Deserialize((TextReader)stringReader);
	}

	internal static T DeserializeFromXMLFile(string filename)
	{
		return DeserializeFromXmlString(File.ReadAllText(filename));
	}
}
