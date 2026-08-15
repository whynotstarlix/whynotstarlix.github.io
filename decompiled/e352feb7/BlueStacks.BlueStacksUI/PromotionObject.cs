using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PromotionObject
{
	private static bool mIsPromotionLoading = true;

	internal static volatile bool mIsBootPromotionLoading = true;

	private const string sPromotionFilename = "bst_promotion";

	internal static PromotionObject Instance = null;

	internal static EventHandler BootPromotionHandler
	{
		get
		{
			return mBootPromotionHandler;
		}
		set
		{
			mBootPromotionHandler = value;
		}
	}

	internal static EventHandler BackgroundPromotionHandler
	{
		get
		{
			return mBackgroundPromotionHandler;
		}
		set
		{
			mBackgroundPromotionHandler = value;
			if (!mIsPromotionLoading)
			{
				mBackgroundPromotionHandler(Instance, new EventArgs());
			}
		}
	}

	internal static EventHandler PromotionHandler
	{
		get
		{
			return mPromotionHandler;
		}
		set
		{
			mPromotionHandler = value;
			if (!mIsPromotionLoading)
			{
				mPromotionHandler(Instance, new EventArgs());
			}
		}
	}

	internal static EventHandler AppSpecificRulesHandler
	{
		get
		{
			return mAppSpecificRulesHandler;
		}
		set
		{
			mAppSpecificRulesHandler = value;
			if (!mIsPromotionLoading)
			{
				mAppSpecificRulesHandler(Instance, new EventArgs());
			}
		}
	}

	internal static Action<bool> AppSuggestionHandler
	{
		get
		{
			return mAppSuggestionHandler;
		}
		set
		{
			mAppSuggestionHandler = value;
		}
	}

	internal static Action<bool> AppRecommendationHandler
	{
		get
		{
			return mAppRecommendationHandler;
		}
		set
		{
			mAppRecommendationHandler = value;
		}
	}

	internal static Action QuestHandler
	{
		get
		{
			return mQuestHandler;
		}
		set
		{
			mQuestHandler = value;
		}
	}

	private static string FilePath => Path.Combine(RegistryStrings.PromotionDirectory, "bst_promotion");

	[XmlIgnore]
	public List<string> AppSpecificRulesList { get; } = new List<string>();

	public List<string> CustomCursorExcludedAppsList { get; } = new List<string> { "com.android.vending" };

	[XmlIgnore]
	public bool IsRootAccessEnabled { get; set; }

	public string MyAppsPromotionID { get; set; } = string.Empty;

	public string MyAppsCrossPromotionID { get; set; } = string.Empty;

	public string BackgroundPromotionID { get; set; } = string.Empty;

	public string BackgroundPromotionImagePath { get; set; } = string.Empty;

	public SerializableDictionary<string, AppIconPromotionObject> DictAppsPromotions { get; set; } = new SerializableDictionary<string, AppIconPromotionObject>();

	public string QuestName { get; set; }

	public string QuestActionType { get; set; }

	public List<QuestRule> QuestRules { get; } = new List<QuestRule>();

	public SerializableDictionary<string, long[]> ResetQuestRules { get; set; } = new SerializableDictionary<string, long[]>();

	public SerializableDictionary<string, long> QuestHdPlayerRules { get; set; } = new SerializableDictionary<string, long>();

	public SerializableDictionary<string, int> MyAppsOrder { get; set; } = new SerializableDictionary<string, int>();

	public SerializableDictionary<string, int> DockOrder { get; set; }

	public SerializableDictionary<string, int> MoreAppsDockOrder { get; set; }

	internal SerializableDictionary<string, BootPromotion> DictOldBootPromotions { get; set; }

	public int BootPromoDisplaytime { get; set; }

	public SerializableDictionary<string, BootPromotion> DictBootPromotions { get; set; }

	public SerializableDictionary<string, SearchRecommendation> SearchRecommendations { get; set; }

	public AppRecommendationSection AppRecommendations { get; set; }

	public List<AppSuggestionPromotion> AppSuggestionList { get; }

	public List<string> BlackListedApplicationsList { get; }

	public SerializableDictionary<string, string> StartupTab { get; set; }

	public bool IsShowOtsFeedback { get; set; }

	public string DiscordClientID { get; set; }

	public bool IsSecurityMetricsEnable { get; set; }

	private static event EventHandler mBootPromotionHandler;

	private static event EventHandler mBackgroundPromotionHandler;

	private static event EventHandler mPromotionHandler;

	private static event EventHandler mAppSpecificRulesHandler;

	private static event Action<bool> mAppSuggestionHandler;

	private static event Action<bool> mAppRecommendationHandler;

	private static event Action mQuestHandler;

	internal void SetDefaultMoreAppsOrder(bool overwrite = true)
	{
		if ((((Dictionary<string, int>)(object)MoreAppsDockOrder).Count == 0) | overwrite)
		{
			SerializableDictionary<string, int> moreAppsDockOrder = MoreAppsDockOrder;
			SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
			((Dictionary<string, int>)(object)obj).Add("com.android.chrome", 49956284 - 1612165195 % 1562208913);
			((Dictionary<string, int>)(object)obj).Add("com.android.camera2", -1997529918 + (62422810 << 1015092613));
			((Dictionary<string, int>)(object)obj).Add("com.bluestacks.settings", -1834480540 + (0x6D16411F | 0x2145B29B));
			((Dictionary<string, int>)(object)obj).Add("com.bluestacks.filemanager", 1336343600 + -1336343596);
			((Dictionary<string, int>)(object)obj).Add("instance_manager", 2092826628 - (0x8B4FEFA | 0x7CA9A3CD));
			((Dictionary<string, int>)(object)obj).Add("help_center", 775421958 + (120339001 << 748770675));
			DictionaryExtensions.ClearAddRange<string, int>((Dictionary<string, int>)(object)moreAppsDockOrder, (Dictionary<string, int>)(object)obj);
		}
	}

	internal void SetDefaultDockOrder(bool overwrite = true)
	{
		if ((((Dictionary<string, int>)(object)DockOrder).Count == 0) | overwrite)
		{
			SerializableDictionary<string, int> dockOrder = DockOrder;
			SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
			((Dictionary<string, int>)(object)obj).Add("appcenter", 1);
			((Dictionary<string, int>)(object)obj).Add("pikaworld", 0x96061A7 ^ 0x96061A5);
			DictionaryExtensions.ClearAddRange<string, int>((Dictionary<string, int>)(object)dockOrder, (Dictionary<string, int>)(object)obj);
		}
	}

	internal void SetDefaultMyAppsOrder(bool overwrite = true)
	{
		if ((((Dictionary<string, int>)(object)MyAppsOrder).Count == 0) | overwrite)
		{
			SerializableDictionary<string, int> myAppsOrder = MyAppsOrder;
			SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
			((Dictionary<string, int>)(object)obj).Add("com.android.vending", 1);
			DictionaryExtensions.ClearAddRange<string, int>((Dictionary<string, int>)(object)myAppsOrder, (Dictionary<string, int>)(object)obj);
		}
	}

	internal void SetDefaultOrder(bool overwrite = true)
	{
		SetDefaultMyAppsOrder(overwrite);
		SetDefaultDockOrder(overwrite);
		SetDefaultMoreAppsOrder(overwrite);
	}

	internal static void LoadDataFromFile()
	{
		try
		{
			if (File.Exists(FilePath))
			{
				using (XmlReader xmlReader = XmlReader.Create(FilePath, new XmlReaderSettings
				{
					ProhibitDtd = true
				}))
				{
					Logger.Info("vikramTest: Loading PromotionObject Settings from " + FilePath);
					Instance = (PromotionObject)new XmlSerializer(typeof(PromotionObject)).Deserialize(xmlReader);
					Logger.Info("vikramTest: Done loading promotionObject.");
					DictionaryExtensions.ClearSync<string, long>((Dictionary<string, long>)(object)Instance.QuestHdPlayerRules);
					ListExtensions.ClearSync<QuestRule>(Instance.QuestRules);
					DictionaryExtensions.ClearSync<string, long[]>((Dictionary<string, long[]>)(object)Instance.ResetQuestRules);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error Loading PromotionObject Settings " + ex.ToString());
		}
		finally
		{
			if (Instance == null)
			{
				Instance = new PromotionObject();
			}
			if (((Dictionary<string, int>)(object)Instance.DockOrder).Count == 0)
			{
				Instance.SetDefaultDockOrder();
			}
			CacheOldBootPromotions();
		}
	}

	private static void CacheOldBootPromotions()
	{
		DictionaryExtensions.ClearAddRange<string, BootPromotion>((Dictionary<string, BootPromotion>)(object)Instance.DictOldBootPromotions, (Dictionary<string, BootPromotion>)(object)Instance.DictBootPromotions);
	}

	internal static void Save()
	{
		try
		{
			if (!Directory.Exists(Directory.GetParent(FilePath).FullName))
			{
				Directory.CreateDirectory(Directory.GetParent(FilePath).FullName);
			}
			using XmlTextWriter xmlTextWriter = new XmlTextWriter(FilePath, Encoding.UTF8)
			{
				Formatting = Formatting.Indented
			};
			new XmlSerializer(typeof(PromotionObject)).Serialize(xmlTextWriter, Instance);
			xmlTextWriter.Flush();
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
		}
	}

	internal void PromotionLoaded()
	{
		mIsPromotionLoading = false;
		mBootPromotionHandler?.Invoke(this, new EventArgs());
		mBackgroundPromotionHandler?.Invoke(this, new EventArgs());
		mQuestHandler?.Invoke();
		mAppSuggestionHandler?.Invoke(obj: true);
		mAppRecommendationHandler?.Invoke(obj: true);
		mPromotionHandler?.Invoke(this, new EventArgs());
		mAppSpecificRulesHandler?.Invoke(this, new EventArgs());
	}

	public PromotionObject()
	{
		SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
		((Dictionary<string, int>)(object)obj).Add("appcenter", 1);
		((Dictionary<string, int>)(object)obj).Add("com.android.vending", 2);
		((Dictionary<string, int>)(object)obj).Add("pikaworld", 3);
		((Dictionary<string, int>)(object)obj).Add("macro_recorder", 4);
		((Dictionary<string, int>)(object)obj).Add("instance_manager", 5);
		((Dictionary<string, int>)(object)obj).Add("help_center", 6);
		DockOrder = obj;
		SerializableDictionary<string, int> obj2 = new SerializableDictionary<string, int>();
		((Dictionary<string, int>)(object)obj2).Add("appcenter", 1);
		((Dictionary<string, int>)(object)obj2).Add("com.android.vending", 2);
		((Dictionary<string, int>)(object)obj2).Add("pikaworld", 3);
		((Dictionary<string, int>)(object)obj2).Add("macro_recorder", 4);
		((Dictionary<string, int>)(object)obj2).Add("instance_manager", 5);
		((Dictionary<string, int>)(object)obj2).Add("help_center", 6);
		MoreAppsDockOrder = obj2;
		DictOldBootPromotions = new SerializableDictionary<string, BootPromotion>();
		BootPromoDisplaytime = 4000;
		DictBootPromotions = new SerializableDictionary<string, BootPromotion>();
		SearchRecommendations = new SerializableDictionary<string, SearchRecommendation>();
		AppRecommendations = new AppRecommendationSection();
		AppSuggestionList = new List<AppSuggestionPromotion>();
		BlackListedApplicationsList = new List<string>();
		StartupTab = new SerializableDictionary<string, string>();
		base._002Ector();
	}
}
