using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class PromotionControl : UserControl, IDisposable, IComponentConnector
{
	private bool isPerformActionOnClose;

	private bool mRunPromotion;

	private double mProgress;

	private Timer progressTimer;

	private bool mForceComplete;

	internal string mActionValue;

	internal string mTextOnActionBtn;

	internal bool mIsActionButtonToShow;

	internal PromotionControl PromoControl;

	private int mBootPromotionImageTimeout;

	internal BootPromotion currentBootPromotion;

	private Thread mSliderAnimationThread;

	private int mThreadId;

	private SerializableDictionary<string, BootPromotion> dictRunningPromotions;

	internal static Dictionary<BootPromotion, int> sBootPromotionDisplayed;

	private MainWindow mMainWindow;

	internal SerializableDictionary<string, string> mExtraPayloadClicked;

	private bool disposedValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPromotionImageGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPromotionImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mPromoButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mPromotionInfoBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mPromoInfoText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BlueProgressBar mProgressBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock BootText;

	private bool _contentLoaded;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	public PromotionControl()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		mRunPromotion = true;
		mProgress = 0.1;
		progressTimer = new Timer();
		mBootPromotionImageTimeout = 4000;
		dictRunningPromotions = new SerializableDictionary<string, BootPromotion>();
		mExtraPayloadClicked = new SerializableDictionary<string, string>();
		((UserControl)this)._002Ector();
		InitializeComponent();
		PromoControl = this;
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)PromoControl))
		{
			if (!string.IsNullOrEmpty(RegistryManager.Instance.PromotionId) || FeatureManager.Instance.IsPromotionFixed)
			{
				mPromotionImage.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promotions/promotion.jpg");
				((Panel)mPromotionImageGrid).Background = (Brush)new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte)0, (byte)0, (byte)0));
			}
			BlueStacksUIBinding.Bind(BootText, "STRING_BOOT_TIME", "");
			int num = RegistryManager.Instance.LastBootTime / 400;
			if (num <= 0)
			{
				RegistryManager.Instance.LastBootTime = 120000;
				RegistryManager.Instance.NoOfBootCompleted = 0;
				num = 1000;
			}
			progressTimer.Tick += ProgressTimer_Tick;
			progressTimer.Interval = num;
			progressTimer.Start();
			if (PromotionObject.Instance == null)
			{
				PromotionObject.LoadDataFromFile();
			}
			PromotionObject.BootPromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.BootPromotionHandler, new EventHandler(PromotionControl_BootPromotionHandler));
		}
	}

	private void PromotionControl_BootPromotionHandler(object sender, EventArgs e)
	{
		((DispatcherObject)PromoControl).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			Fraction fraction = new Fraction(RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth, RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight);
			if (App.defaultResolution == fraction && (!((Dictionary<string, BootPromotion>)(object)dictRunningPromotions).Keys.All(((IEnumerable<string>)((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions).Keys).Contains<string>) || ((Dictionary<string, BootPromotion>)(object)PromotionObject.Instance.DictBootPromotions).Count != ((Dictionary<string, BootPromotion>)(object)dictRunningPromotions).Count))
			{
				StopSlider();
				StartAnimation(PromotionObject.Instance.DictBootPromotions);
			}
		}, new object[0]);
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		Fraction fraction = new Fraction(RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth, RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight);
		if (App.defaultResolution != fraction)
		{
			StartAnimation(new SerializableDictionary<string, BootPromotion>());
		}
		else
		{
			StartAnimation(PromotionObject.Instance.DictBootPromotions);
		}
	}

	private void StartAnimation(SerializableDictionary<string, BootPromotion> dict)
	{
		((DispatcherObject)PromoControl).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected O, but got Unknown
			dictRunningPromotions = UsefulExtensionMethod.DeepCopy<SerializableDictionary<string, BootPromotion>>(dict);
			if (((Dictionary<string, BootPromotion>)(object)dict).Count > 0)
			{
				List<KeyValuePair<string, BootPromotion>> myList = ((IEnumerable<KeyValuePair<string, BootPromotion>>)dict).ToList();
				myList.Sort((KeyValuePair<string, BootPromotion> pair1, KeyValuePair<string, BootPromotion> pair2) => pair1.Value.Order.CompareTo(pair2.Value.Order));
				((Panel)mPromotionImageGrid).Background = (Brush)new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte)0, (byte)0, (byte)0));
				mRunPromotion = true;
				mSliderAnimationThread = new Thread((ThreadStart)delegate
				{
					mThreadId = mSliderAnimationThread.ManagedThreadId;
					Dictionary<BootPromotion, int> bootPromos = new Dictionary<BootPromotion, int>();
					while (mRunPromotion && mThreadId == Thread.CurrentThread.ManagedThreadId)
					{
						try
						{
							foreach (KeyValuePair<string, BootPromotion> item in myList)
							{
								if (!mRunPromotion || mThreadId != Thread.CurrentThread.ManagedThreadId)
								{
									break;
								}
								if (currentBootPromotion == null)
								{
									currentBootPromotion = item.Value;
								}
								((DispatcherObject)PromoControl).Dispatcher.Invoke((Delegate)(Action)delegate
								{
									HandleAnimation(item.Value);
									currentBootPromotion = item.Value;
									SetLoadingText(item.Value.ButtonText);
									if (bootPromos.ContainsKey(item.Value))
									{
										bootPromos[item.Value] = bootPromos[item.Value] + 1;
									}
									else
									{
										bootPromos.Add(item.Value, 1);
									}
									sBootPromotionDisplayed = bootPromos;
								}, new object[0]);
								mBootPromotionImageTimeout = PromotionObject.Instance.BootPromoDisplaytime;
								Thread.Sleep(mBootPromotionImageTimeout);
							}
						}
						catch (Exception ex)
						{
							Logger.Error(ex.ToString());
						}
					}
				})
				{
					IsBackground = true
				};
				mSliderAnimationThread.Start();
			}
		}, new object[0]);
	}

	private void ProgressTimer_Tick(object sender, EventArgs e)
	{
		try
		{
			ParentWindow.mWelcomeTab.mHomeAppManager.InitiateHtmlSidePanel();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while creating HTML sidepanel .Exception: " + ex.ToString());
		}
		if (mProgress >= 99.0 && !mForceComplete)
		{
			((RangeBase)mProgressBar).Value = mProgress;
			mProgress += 0.0;
		}
		else if (mProgress >= 95.0 && !mForceComplete)
		{
			progressTimer.Interval = progressTimer.Interval;
			((RangeBase)mProgressBar).Value = mProgress;
			mProgress += 0.025;
		}
		else
		{
			((RangeBase)mProgressBar).Value = mProgress;
			mProgress += 0.25;
		}
	}

	private void mPromoButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Invalid comparison between Unknown and I4
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Invalid comparison between Unknown and I4
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Invalid comparison between Unknown and I4
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Invalid comparison between Unknown and I4
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Invalid comparison between Unknown and I4
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		StopSlider();
		ClientStats.SendMiscellaneousStatsAsync("BootPromotion", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, JsonConvert.SerializeObject((object)currentBootPromotion.ExtraPayload), null, null);
		GenericAction val = (GenericAction)(348433303 + ~348367766);
		if (((Dictionary<string, string>)(object)currentBootPromotion.ExtraPayload).ContainsKey("click_generic_action"))
		{
			val = EnumHelper.Parse<GenericAction>(((Dictionary<string, string>)(object)currentBootPromotion.ExtraPayload)["click_generic_action"], (GenericAction)(0x19646898 ^ 0x19656898));
		}
		if ((int)val <= (0x72E79DE0 ^ 0x72E79DC0))
		{
			if ((int)val <= -901492303 + 901492307 % 2017264105)
			{
				if (val - 1 > 1 && (int)val == (-2129133564 ^ -2129133568))
				{
					goto IL_015b;
				}
			}
			else if ((int)val == 637534216 - (1509242444 << 2074429079) || (int)val == 2070111391 - (0x3363007E | 0x7A62646F))
			{
				goto IL_015b;
			}
		}
		else if ((int)val <= -896446596 + 896446724 % 1827241164)
		{
			if ((int)val == -43873523 - ~43873586)
			{
				goto IL_015b;
			}
			if ((int)val == (0x30 ^ 0xB0))
			{
			}
		}
		else if ((int)val != -1774377407 - -1774377663 && (int)val == -1373251296 - (2061655314 << 314341060))
		{
		}
		isPerformActionOnClose = true;
		goto IL_016b;
		IL_015b:
		isPerformActionOnClose = false;
		goto IL_016b;
		IL_016b:
		if (isPerformActionOnClose)
		{
			((UIElement)mPromotionInfoBorder).Visibility = (Visibility)0;
			((UIElement)mPromoButton).Visibility = (Visibility)1;
			mPromoInfoText.Text = currentBootPromotion.PromoBtnClickStatusText.ToString(CultureInfo.InvariantCulture);
			if (string.Equals(currentBootPromotion.ThemeEnabled, "true", StringComparison.InvariantCultureIgnoreCase))
			{
				ParentWindow.Utils.ApplyTheme(currentBootPromotion.ThemeName);
			}
		}
		else
		{
			mExtraPayloadClicked = currentBootPromotion.ExtraPayload;
			ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)currentBootPromotion.ExtraPayload, "boot_promo");
		}
	}

	private void StopSlider()
	{
		try
		{
			if (!mRunPromotion)
			{
				return;
			}
			mRunPromotion = false;
			if (sBootPromotionDisplayed != null && sBootPromotionDisplayed.Any())
			{
				Dictionary<BootPromotion, int> bootPromos = sBootPromotionDisplayed;
				sBootPromotionDisplayed = null;
				ThreadPool.QueueUserWorkItem(delegate
				{
					SendPromotionStats(bootPromos);
				});
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception aborting thread" + ex.ToString());
		}
	}

	private static void SendPromotionStats(Dictionary<BootPromotion, int> bootPromos)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"prod_ver",
					RegistryManager.Instance.ClientVersion
				},
				{
					"eng_ver",
					RegistryManager.Instance.Version
				},
				{
					"guid",
					RegistryManager.Instance.UserGuid
				},
				{
					"locale",
					RegistryManager.Instance.UserSelectedLocale
				},
				{
					"oem",
					RegistryManager.Instance.Oem
				},
				{
					"partner",
					RegistryManager.Instance.Partner
				},
				{
					"campaign_json",
					RegistryManager.Instance.CampaignJson
				}
			};
			List<BootBanner> list = new List<BootBanner>();
			foreach (KeyValuePair<BootPromotion, int> bootPromo in bootPromos)
			{
				list.Add(new BootBanner
				{
					Frequency = bootPromo.Value.ToString(CultureInfo.InvariantCulture),
					ClickActionPackagename = ((Dictionary<string, string>)(object)bootPromo.Key.ExtraPayload)["click_action_packagename"],
					ClickGenericAction = ((Dictionary<string, string>)(object)bootPromo.Key.ExtraPayload)["click_generic_action"],
					ClickActionValue = ((Dictionary<string, string>)(object)bootPromo.Key.ExtraPayload)["click_action_value"],
					Id = bootPromo.Key.Id,
					ButtonText = bootPromo.Key.ButtonText,
					Order = bootPromo.Key.Order.ToString(CultureInfo.InvariantCulture),
					ImageUrl = bootPromo.Key.ImageUrl,
					HashTags = ((Dictionary<string, string>)(object)bootPromo.Key.ExtraPayload)["hash_tags"]
				});
			}
			dictionary.Add("boot_banners", JsonConvert.SerializeObject((object)list));
			BstHttpClient.Post(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
			{
				RegistryManager.Instance.Host,
				"bs4/stats/client_boot_promotion_stats"
			})), dictionary, (Dictionary<string, string>)null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
		}
		catch (Exception ex)
		{
			Logger.Error("SendPromotionStats", new object[1] { ex });
		}
	}

	private void SetLoadingText(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			((ContentControl)mPromoButton).Content = text;
			((UIElement)mPromoButton).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mPromoButton).Visibility = (Visibility)1;
		}
		isPerformActionOnClose = false;
		Border obj = mPromotionInfoBorder;
		int num = ((2072810419 > 1070889170) ? 2 : 2);
		((UIElement)obj).Visibility = (Visibility)num;
	}

	internal void Stop()
	{
		progressTimer.Stop();
		((Component)(object)progressTimer).Dispose();
		StopSlider();
	}

	internal void HandlePromotionEventAfterBoot()
	{
		if (isPerformActionOnClose)
		{
			ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)currentBootPromotion.ExtraPayload, "boot_promo");
			isPerformActionOnClose = false;
		}
	}

	private void HandleAnimation(BootPromotion promo)
	{
		AnimateImage(mPromotionImage, promo.ImagePath);
		if (!string.IsNullOrEmpty(promo.ButtonText))
		{
			((UIElement)mPromoButton).Visibility = (Visibility)0;
			((ContentControl)mPromoButton).Content = promo.ButtonText;
		}
	}

	private static void AnimateImage(CustomPictureBox image, string imagePath)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		TimeSpan timeSpan = TimeSpan.FromSeconds(0.6);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(0.6);
		DoubleAnimation fadeInAnimation = new DoubleAnimation(1.0, Duration.op_Implicit(timeSpan));
		if (((Image)image).Source == null)
		{
			((UIElement)image).Opacity = 1.0;
			image.IsFullImagePath = true;
			image.ImageName = imagePath;
		}
		else if (image.ImageName != imagePath)
		{
			DoubleAnimation val = new DoubleAnimation(0.0, Duration.op_Implicit(timeSpan2));
			((Timeline)val).Completed += delegate
			{
				image.IsFullImagePath = true;
				image.ImageName = imagePath;
				((UIElement)image).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)fadeInAnimation);
			};
			((UIElement)image).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
		}
	}

	private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((UIElement)mPromotionInfoBorder).Visibility = (Visibility)(0x7FF6EBB9 ^ 0x7FF6EBBB);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (progressTimer != null)
			{
				progressTimer.Tick -= ProgressTimer_Tick;
				((Component)(object)progressTimer).Dispose();
			}
			disposedValue = true;
		}
	}

	~PromotionControl()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/promotioncontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(PromotionControl)target).Loaded += new RoutedEventHandler(UserControl_Loaded);
			break;
		case 2:
			mPromotionImageGrid = (Grid)target;
			break;
		case 3:
			mPromotionImage = (CustomPictureBox)target;
			break;
		case 4:
			mPromoButton = (CustomButton)target;
			((ButtonBase)mPromoButton).Click += new RoutedEventHandler(mPromoButton_Click);
			break;
		case 5:
			mPromotionInfoBorder = (Border)target;
			break;
		case 6:
			mPromoInfoText = (TextBlock)target;
			break;
		case 7:
			mCloseButton = (CustomPictureBox)target;
			((UIElement)mCloseButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_PreviewMouseLeftButtonUp);
			break;
		case 8:
			mProgressBar = (BlueProgressBar)target;
			break;
		case 9:
			BootText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
