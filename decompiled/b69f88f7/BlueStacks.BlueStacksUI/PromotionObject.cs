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
	private static bool mIsPromotionLoading;

	internal static volatile bool mIsBootPromotionLoading;

	private const string sPromotionFilename = "bst_promotion";

	internal static PromotionObject Instance;

	internal static EventHandler BootPromotionHandler
	{
		get
		{
			return PromotionObject.mBootPromotionHandler;
		}
		set
		{
			PromotionObject.mBootPromotionHandler = value;
		}
	}

	internal static EventHandler BackgroundPromotionHandler
	{
		get
		{
			return PromotionObject.mBackgroundPromotionHandler;
		}
		set
		{
			PromotionObject.mBackgroundPromotionHandler = value;
			if (!mIsPromotionLoading)
			{
				PromotionObject.mBackgroundPromotionHandler(Instance, new EventArgs());
			}
		}
	}

	internal static EventHandler PromotionHandler
	{
		get
		{
			return PromotionObject.mPromotionHandler;
		}
		set
		{
			PromotionObject.mPromotionHandler = value;
			if (!mIsPromotionLoading)
			{
				PromotionObject.mPromotionHandler(Instance, new EventArgs());
			}
		}
	}

	internal static EventHandler AppSpecificRulesHandler
	{
		get
		{
			return PromotionObject.mAppSpecificRulesHandler;
		}
		set
		{
			PromotionObject.mAppSpecificRulesHandler = value;
			if (!mIsPromotionLoading)
			{
				PromotionObject.mAppSpecificRulesHandler(Instance, new EventArgs());
			}
		}
	}

	internal static Action<bool> AppSuggestionHandler
	{
		get
		{
			return PromotionObject.mAppSuggestionHandler;
		}
		set
		{
			PromotionObject.mAppSuggestionHandler = value;
		}
	}

	internal static Action<bool> AppRecommendationHandler
	{
		get
		{
			return PromotionObject.mAppRecommendationHandler;
		}
		set
		{
			PromotionObject.mAppRecommendationHandler = value;
		}
	}

	internal static Action QuestHandler
	{
		get
		{
			return PromotionObject.mQuestHandler;
		}
		set
		{
			PromotionObject.mQuestHandler = value;
		}
	}

	private static string FilePath => Path.Combine(RegistryStrings.PromotionDirectory, "");

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
		if (((Dictionary<string, int>)(object)MoreAppsDockOrder).Count == 0 || overwrite)
		{
			SerializableDictionary<string, int> moreAppsDockOrder = MoreAppsDockOrder;
			SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
			((Dictionary<string, int>)(object)obj).Add("com.android.chrome", 2);
			((Dictionary<string, int>)(object)obj).Add("com.android.camera2", 2);
			((Dictionary<string, int>)(object)obj).Add("com.bluestacks.settings", 3);
			((Dictionary<string, int>)(object)obj).Add("com.bluestacks.filemanager", 4);
			((Dictionary<string, int>)(object)obj).Add("instance_manager", 5);
			((Dictionary<string, int>)(object)obj).Add("help_center", 6);
			DictionaryExtensions.ClearAddRange<string, int>((Dictionary<string, int>)(object)moreAppsDockOrder, (Dictionary<string, int>)(object)obj);
		}
	}

	internal void SetDefaultDockOrder(bool overwrite = true)
	{
		if (((Dictionary<string, int>)(object)DockOrder).Count == 0 || overwrite)
		{
			SerializableDictionary<string, int> dockOrder = DockOrder;
			SerializableDictionary<string, int> obj = new SerializableDictionary<string, int>();
			((Dictionary<string, int>)(object)obj).Add("appcenter", 1);
			((Dictionary<string, int>)(object)obj).Add("pikaworld", 2);
			DictionaryExtensions.ClearAddRange<string, int>((Dictionary<string, int>)(object)dockOrder, (Dictionary<string, int>)(object)obj);
		}
	}

	internal void SetDefaultMyAppsOrder(bool overwrite = true)
	{
		if (((Dictionary<string, int>)(object)MyAppsOrder).Count == 0 || overwrite)
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

	private static void CacheOldBootPromotions()
	{
		DictionaryExtensions.ClearAddRange<string, BootPromotion>((Dictionary<string, BootPromotion>)(object)Instance.DictOldBootPromotions, (Dictionary<string, BootPromotion>)(object)Instance.DictBootPromotions);
	}

	internal static void Save()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!Directory.Exists(Directory.GetParent(FilePath).FullName))
			{
				Directory.CreateDirectory(Directory.GetParent(FilePath).FullName);
			}
			XmlTextWriter val = new XmlTextWriter(FilePath, Encoding.UTF8)
			{
				Formatting = (Formatting)1
			};
			try
			{
				new XmlSerializer(typeof(PromotionObject)).Serialize((XmlWriter)(object)val, (object)Instance);
				((XmlWriter)val).Flush();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
		}
	}

	internal void PromotionLoaded()
	{
		mIsPromotionLoading = false;
		PromotionObject.mBootPromotionHandler?.Invoke(this, new EventArgs());
		PromotionObject.mBackgroundPromotionHandler?.Invoke(this, new EventArgs());
		PromotionObject.mQuestHandler?.Invoke();
		PromotionObject.mAppSuggestionHandler?.Invoke(obj: true);
		PromotionObject.mAppRecommendationHandler?.Invoke(obj: true);
		PromotionObject.mPromotionHandler?.Invoke(this, new EventArgs());
		PromotionObject.mAppSpecificRulesHandler?.Invoke(this, new EventArgs());
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

	static PromotionObject()
	{
		mIsPromotionLoading = true;
		mIsBootPromotionLoading = true;
		Instance = null;
	}
}
