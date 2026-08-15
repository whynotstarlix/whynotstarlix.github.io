using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

internal sealed class GenericNotificationManager
{
	private static volatile GenericNotificationManager sInstance;

	private static object syncRoot = new object();

	private static object syncNotificationsReadWrite = new object();

	public static GenericNotificationManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot)
				{
					if (sInstance == null)
					{
						sInstance = new GenericNotificationManager();
					}
				}
			}
			return sInstance;
		}
	}

	internal static string GenericNotificationFilePath => Path.Combine(RegistryStrings.PromotionDirectory, "bst_genericNotification");

	private GenericNotificationManager()
	{
	}

	public static void AddNewNotification(GenericNotificationItem notificationItem, bool dontOverwrite = false)
	{
		lock (syncNotificationsReadWrite)
		{
			try
			{
				SerializableDictionary<string, GenericNotificationItem> savedNotifications = GetSavedNotifications();
				if (!dontOverwrite)
				{
					((Dictionary<string, GenericNotificationItem>)(object)savedNotifications)[notificationItem.Id] = notificationItem;
					SaveNotifications(savedNotifications);
				}
				else if (!((Dictionary<string, GenericNotificationItem>)(object)savedNotifications).ContainsKey(notificationItem.Id))
				{
					((Dictionary<string, GenericNotificationItem>)(object)savedNotifications)[notificationItem.Id] = notificationItem;
					SaveNotifications(savedNotifications);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to add notification id : {0} titled : {1} and msg : {2}... Err : {3}", new object[4]
				{
					notificationItem.Id,
					notificationItem.Title,
					notificationItem.Message,
					ex.ToString()
				});
			}
		}
	}

	private static void SaveNotifications(SerializableDictionary<string, GenericNotificationItem> lstItem)
	{
		using XmlTextWriter xmlTextWriter = new XmlTextWriter(GenericNotificationFilePath, Encoding.UTF8)
		{
			Formatting = Formatting.Indented
		};
		SerializableDictionary<string, GenericNotificationItem> val = new SerializableDictionary<string, GenericNotificationItem>();
		foreach (KeyValuePair<string, GenericNotificationItem> item in (Dictionary<string, GenericNotificationItem>)(object)lstItem)
		{
			if (!item.Value.IsDeleted)
			{
				((Dictionary<string, GenericNotificationItem>)(object)val).Add(item.Key, item.Value);
			}
		}
		new XmlSerializer(typeof(SerializableDictionary<string, GenericNotificationItem>)).Serialize(xmlTextWriter, val);
		xmlTextWriter.Flush();
	}

	private static SerializableDictionary<string, GenericNotificationItem> GetSavedNotifications()
	{
		SerializableDictionary<string, GenericNotificationItem> result = new SerializableDictionary<string, GenericNotificationItem>();
		if (File.Exists(GenericNotificationFilePath))
		{
			int num = 3;
			while (num > 0)
			{
				num--;
				try
				{
					using XmlReader xmlReader = XmlReader.Create(GenericNotificationFilePath, new XmlReaderSettings
					{
						ProhibitDtd = true
					});
					result = (SerializableDictionary<string, GenericNotificationItem>)new XmlSerializer(typeof(SerializableDictionary<string, GenericNotificationItem>)).Deserialize(xmlReader);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception when reading saved notifications." + ex.ToString());
					continue;
				}
				break;
			}
		}
		return result;
	}

	internal GenericNotificationItem GetNotificationItem(string id)
	{
		return ((IEnumerable<KeyValuePair<string, GenericNotificationItem>>)GetNotificationItems((GenericNotificationItem _) => _.Id == id)).FirstOrDefault().Value;
	}

	public static SerializableDictionary<string, GenericNotificationItem> MarkNotification(IEnumerable<string> ids, Action<GenericNotificationItem> setter)
	{
		lock (syncNotificationsReadWrite)
		{
			SerializableDictionary<string, GenericNotificationItem> lstItem = new SerializableDictionary<string, GenericNotificationItem>();
			try
			{
				lstItem = GetSavedNotifications();
				foreach (string item in ids.Where((string id) => id != null && ((Dictionary<string, GenericNotificationItem>)(object)lstItem).ContainsKey(id)))
				{
					setter(((Dictionary<string, GenericNotificationItem>)(object)lstItem)[item]);
				}
				SaveNotifications(lstItem);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to mark notification... Err : " + ex.ToString());
			}
			return lstItem;
		}
	}

	public static SerializableDictionary<string, GenericNotificationItem> GetNotificationItems(Predicate<GenericNotificationItem> getter)
	{
		lock (syncNotificationsReadWrite)
		{
			SerializableDictionary<string, GenericNotificationItem> savedNotifications = GetSavedNotifications();
			SerializableDictionary<string, GenericNotificationItem> val = new SerializableDictionary<string, GenericNotificationItem>();
			foreach (KeyValuePair<string, GenericNotificationItem> item in ((IEnumerable<KeyValuePair<string, GenericNotificationItem>>)savedNotifications).Where((KeyValuePair<string, GenericNotificationItem> item) => getter(item.Value)))
			{
				((Dictionary<string, GenericNotificationItem>)(object)val).Add(item.Key, item.Value);
			}
			return val;
		}
	}
}
