using System;
using System.CodeDom.Compiler;
using System.Collections;
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
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class HomeApp : UserControl, IComponentConnector
{
	private WrapPanel InstalledAppsDrawer;

	private static object syncRoot;

	private AppIconUI moreAppsIcon;

	private MainWindow mMainWindow;

	private SidePanelVisibility? mCurrentSidePanelVisibility;

	private bool mIsSidePanelContentLoadedOnce;

	internal List<RecommendedApps> sAppRecommendationsPool;

	private DispatcherTimer searchHoverTimer;

	private string defaultSearchBoxText;

	private bool mIsShowSearchRecommendations;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBackgroundImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mInstalledAppText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGridSeparator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppSettings;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mAppSettingsPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid dummyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mAppSettingsPopupBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path LeftArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer InstalledAppsDrawerScrollBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mAppRecommendationsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mDiscoverApps;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer appRecomScrollViewer;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mAppRecommendationSectionsPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mAppRecommendationsGenericMessages;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppRecommendationsGenericMessageImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppRecommendationsGenericMessageText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mSearchGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border searchTextBoxBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mSearchTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border Mask;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mSearchRecommendationBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel searchRecomItems;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMultiInstanceControlsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mDockGridBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDockGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mDockPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mDockAppIconToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mDockIconText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mDownArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mMoreAppsDockPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMoreAppsDockIconText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCustomMessageBoxCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMoreAppsDockPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mSuggestedAppPopUp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder4;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseAppSuggPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppSuggestionPopUp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path UpArrow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	private bool _contentLoaded;

	private MainWindow ParentWindow
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

	internal bool SideHtmlBrowserInited { get; set; }

	internal BrowserControl SideHtmlBrowser { get; set; }

	public HomeApp(MainWindow window)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		((UserControl)this)._002Ector();
		sAppRecommendationsPool = new List<RecommendedApps>();
		defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
		InitializeComponent();
		mMainWindow = window;
		SetWallpaper();
		object content = ((ContentControl)InstalledAppsDrawerScrollBar).Content;
		InstalledAppsDrawer = (WrapPanel)((content is WrapPanel) ? content : null);
		mLoadingGrid.ProgressText = "STRING_LOADING_ENGINE";
		if (InstalledAppsDrawer != null)
		{
			((FrameworkElement)InstalledAppsDrawer).Margin = new Thickness(-15.0, -40.0, 0.0, 0.0);
		}
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this) && PromotionObject.Instance != null)
		{
			PromotionObject.BackgroundPromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.BackgroundPromotionHandler, new EventHandler(HomeApp_BackgroundPromotionHandler));
		}
		if (!FeatureManager.Instance.IsMultiInstanceControlsGridVisible)
		{
			((UIElement)mMultiInstanceControlsGrid).Visibility = (Visibility)1;
		}
		if (!FeatureManager.Instance.IsAppSettingsAvailable)
		{
			((UIElement)mAppSettings).Visibility = (Visibility)1;
			((UIElement)mGridSeparator).Visibility = (Visibility)1;
		}
		searchHoverTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(700.0)
		};
		searchHoverTimer.Tick += delegate
		{
			OpenSearchSuggestions();
		};
		Border mask = Mask;
		CornerRadius cornerRadius = searchTextBoxBorder.CornerRadius;
		double topRight = ((CornerRadius)(ref cornerRadius)).TopRight;
		cornerRadius = searchTextBoxBorder.CornerRadius;
		mask.CornerRadius = new CornerRadius(0.0, topRight, ((CornerRadius)(ref cornerRadius)).BottomRight, 0.0);
		GetSearchTextFromCloud();
	}

	internal void AddDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
	{
		int num = 0;
		using (IEnumerator<AppIconUI> enumerator = ((IEnumerable)((Panel)mDockPanel).Children).OfType<AppIconUI>().GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
			{
				num++;
			}
		}
		AppIconUI newAppIconUI = GetNewAppIconUI(icon, downloadInstallApk);
		((Panel)mDockPanel).Children.Insert(num, (UIElement)(object)newAppIconUI);
		((Panel)mDockPanel).Children.Remove((UIElement)(object)moreAppsIcon);
		((Panel)mDockPanel).Children.Add((UIElement)(object)moreAppsIcon);
	}

	internal void AddMoreAppsDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
	{
		int num = 0;
		using (IEnumerator<AppIconUI> enumerator = ((IEnumerable)((Panel)mMoreAppsDockPanel).Children).OfType<AppIconUI>().GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
			{
				num++;
			}
		}
		AppIconUI newAppIconUI = GetNewAppIconUI(icon, downloadInstallApk);
		((Panel)mMoreAppsDockPanel).Children.Insert(num, (UIElement)(object)newAppIconUI);
	}

	internal void AddInstallDrawerIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
	{
		int num = 0;
		using (IEnumerator<AppIconUI> enumerator = ((IEnumerable)((Panel)InstalledAppsDrawer).Children).OfType<AppIconUI>().GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
			{
				num++;
			}
		}
		AppIconUI newAppIconUI = GetNewAppIconUI(icon, downloadInstallApk);
		((Panel)InstalledAppsDrawer).Children.Insert(num, (UIElement)(object)newAppIconUI);
	}

	internal void RemoveAppIconFromUI(AppIconModel appIcon)
	{
		((Panel)InstalledAppsDrawer).Children.Remove((UIElement)(object)(from AppIconUI s in (IEnumerable)((Panel)InstalledAppsDrawer).Children
			where s.mAppIconModel.PackageName == appIcon.PackageName
			select s).FirstOrDefault());
		((Panel)mDockPanel).Children.Remove((UIElement)(object)(from AppIconUI s in (IEnumerable)((Panel)mDockPanel).Children
			where s.mAppIconModel.PackageName == appIcon.PackageName
			select s).FirstOrDefault());
		((Panel)mMoreAppsDockPanel).Children.Remove((UIElement)(object)(from AppIconUI s in (IEnumerable)((Panel)mMoreAppsDockPanel).Children
			where s.mAppIconModel.PackageName == appIcon.PackageName
			select s).FirstOrDefault());
	}

	internal void InitUIAppPromotionEvents()
	{
		if (PromotionObject.Instance != null)
		{
			PromotionObject.AppRecommendationHandler = (Action<bool>)Delegate.Combine(PromotionObject.AppRecommendationHandler, new Action<bool>(ShowAppRecommendations));
		}
	}

	private void HomeApp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (((RoutedEventArgs)e).OriginalSource == InstalledAppsDrawerScrollBar)
		{
			ParentWindow.StaticComponents.ShowUninstallButtons(isShow: false);
		}
	}

	internal void InitMoreAppsIcon()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		AppIconModel appIconModel = new AppIconModel();
		appIconModel.AppName = LocaleStrings.GetLocalizedString("STRING_MORE_APPS", "");
		appIconModel.ImageName = "moreapps";
		appIconModel.AddToDock();
		moreAppsIcon = new AppIconUI(ParentWindow, appIconModel);
		((ButtonBase)moreAppsIcon).Click += new RoutedEventHandler(MoreAppsIcon_Click);
		((Panel)mDockPanel).Children.Add((UIElement)(object)moreAppsIcon);
	}

	private void MoreAppsIcon_Click(object sender, RoutedEventArgs e)
	{
		AppIconUI appIconUI = sender as AppIconUI;
		((Popup)mDockAppIconToolTipPopup).IsOpen = false;
		((Popup)mMoreAppsDockPopup).PlacementTarget = (UIElement)(object)appIconUI.mAppImage;
		((Popup)mMoreAppsDockPopup).IsOpen = true;
	}

	private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mMoreAppsDockPopup).IsOpen = false;
		((Popup)mMoreAppsDockPopup).StaysOpen = false;
	}

	private void InstalledAppsDrawerScrollBar_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		double verticalOffset = InstalledAppsDrawerScrollBar.VerticalOffset;
		if ((int)InstalledAppsDrawerScrollBar.ComputedVerticalScrollBarVisibility != 0)
		{
			((UIElement)InstalledAppsDrawerScrollBar).OpacityMask = null;
		}
		else if (verticalOffset > 1.0)
		{
			if (verticalOffset == InstalledAppsDrawerScrollBar.ScrollableHeight)
			{
				((UIElement)InstalledAppsDrawerScrollBar).OpacityMask = (Brush)(object)BluestacksUIColor.mBottomOpacityMask;
			}
			else
			{
				((UIElement)InstalledAppsDrawerScrollBar).OpacityMask = (Brush)(object)BluestacksUIColor.mScrolledOpacityMask;
			}
		}
		else
		{
			((UIElement)InstalledAppsDrawerScrollBar).OpacityMask = (Brush)(object)BluestacksUIColor.mTopOpacityMask;
		}
	}

	private void mCloseAppSuggPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)mSuggestedAppPopUp).IsOpen = false;
	}

	private AppIconUI GetNewAppIconUI(AppIconModel iconModel, DownloadInstallApk downloadInstallApk = null)
	{
		AppIconUI appIconUI = new AppIconUI(ParentWindow, iconModel);
		if (downloadInstallApk != null)
		{
			appIconUI.InitAppDownloader(downloadInstallApk);
		}
		return appIconUI;
	}

	internal void InitiateSideHtmlBrowser()
	{
		lock (syncRoot)
		{
			if (FeatureManager.Instance.IsHtmlSideBar && !SideHtmlBrowserInited && CefHelper.CefInited && !RegistryManager.Instance.IsPremium)
			{
				CreateSideHtmlBrowserControl();
			}
		}
	}

	private void CreateSideHtmlBrowserControl()
	{
		SideHtmlBrowserInited = true;
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			BrowserControl browserControl = new BrowserControl(BlueStacksUIUtils.GetHtmlSidePanelUrl());
			((UIElement)browserControl).Visibility = (Visibility)0;
			BrowserControl browserControl2 = browserControl;
			CustomPictureBox val = new CustomPictureBox
			{
				HorizontalAlignment = (HorizontalAlignment)1,
				VerticalAlignment = (VerticalAlignment)1,
				Height = 30.0,
				Width = 30.0,
				ImageName = "loader",
				IsImageToBeRotated = true
			};
			((Panel)mAppRecommendationsGrid).Children.Add((UIElement)(object)browserControl2);
			((Panel)mAppRecommendationsGrid).Children.Add((UIElement)(object)val);
			browserControl2.CreateNewBrowser();
			SideHtmlBrowser = browserControl2;
		}, new object[0]);
	}

	private void ChangeSideRecommendationsVisibility(bool isAppRecommendationsVisible, bool isSearchBarVisible)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			if (!isAppRecommendationsVisible)
			{
				if (!isAppRecommendationsVisible)
				{
					((UIElement)mAppRecommendationsGrid).Visibility = (Visibility)(-1595520869 - -1595520871);
					((DependencyObject)InstalledAppsDrawerScrollBar).SetValue(Grid.ColumnSpanProperty, (object)(-1481277438 - (345939670 << 189073102)));
					((DependencyObject)mMultiInstanceControlsGrid).SetValue(Grid.ColumnSpanProperty, (object)(1500525288 + ~1500525285));
					if (isSearchBarVisible)
					{
						Border obj = mSearchGrid;
						int num = ((654939765 > 294115132) ? 2 : 2);
						((UIElement)obj).Visibility = (Visibility)num;
						((FrameworkElement)mSearchGrid).Margin = new Thickness(0.0, 20.0, 20.0, 0.0);
						((FrameworkElement)mSearchGrid).Width = 350.0;
						mIsShowSearchRecommendations = true;
						((FrameworkElement)mSearchRecommendationBorder).Margin = new Thickness(0.0, 59.0, 20.0, 0.0);
						((FrameworkElement)mSearchRecommendationBorder).Width = 0.0;
					}
					else
					{
						((UIElement)mSearchGrid).Visibility = (Visibility)(0x15AE2 ^ 0x15AE0);
					}
				}
			}
			else
			{
				Grid obj2 = mAppRecommendationsGrid;
				int num2 = ((738771079 > 1154340525) ? 2 : 2);
				((UIElement)obj2).Visibility = (Visibility)num2;
				((DependencyObject)InstalledAppsDrawerScrollBar).SetValue(Grid.ColumnSpanProperty, (object)1);
				((DependencyObject)mMultiInstanceControlsGrid).SetValue(Grid.ColumnSpanProperty, (object)1);
				if (isSearchBarVisible)
				{
					((UIElement)mDiscoverApps).Visibility = (Visibility)(-397578238 + (1836153641 << 1422530892));
					ScrollViewer obj3 = appRecomScrollViewer;
					int num3 = ((1258089056 > 436496218) ? 2 : 2);
					((UIElement)obj3).Visibility = (Visibility)num3;
					((UIElement)mSearchGrid).Visibility = (Visibility)(-33208392 + (2125337252 >> 1477266118));
					((FrameworkElement)mSearchGrid).Margin = new Thickness(20.0, 54.0, 20.0, 0.0);
					((FrameworkElement)mSearchGrid).Width = 0.0;
					mIsShowSearchRecommendations = false;
					((FrameworkElement)mSearchRecommendationBorder).Margin = new Thickness(20.0, 88.0, 20.0, 0.0);
					((FrameworkElement)mSearchRecommendationBorder).Width = 0.0;
				}
				else
				{
					Border obj4 = mSearchGrid;
					int num4 = ((1641488020 > 511157651) ? 2 : 2);
					((UIElement)obj4).Visibility = (Visibility)num4;
					((UIElement)mDiscoverApps).Visibility = (Visibility)(-877345995 + 877345997 % 992224907);
					((UIElement)appRecomScrollViewer).Visibility = (Visibility)(-178158727 + (356317459 >> 443646465));
					((FrameworkElement)mAppRecommendationsGrid).Width = 0.0;
				}
			}
		}, new object[0]);
	}

	private void ShowAppRecommendations(bool isContentReloadRequired)
	{
		try
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Invalid comparison between Unknown and I4
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				if ((ParentWindow != null && ((FrameworkElement)ParentWindow).ActualWidth <= 700.0) || FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsSearchBarVisible || (int)RegistryManager.Instance.InstallationType == 1)
				{
					if (!mCurrentSidePanelVisibility.HasValue || mCurrentSidePanelVisibility != (SidePanelVisibility?)(1706706009 + ~1706706006))
					{
						ChangeSideRecommendationsVisibility(isAppRecommendationsVisible: false, isSearchBarVisible: false);
						mCurrentSidePanelVisibility = (SidePanelVisibility)2;
					}
				}
				else
				{
					if (!RegistryManager.Instance.IsPremium)
					{
						if (FeatureManager.Instance.IsHtmlSideBar)
						{
							goto IL_014b;
						}
						if (FeatureManager.Instance.IsShowAppRecommendations)
						{
							AppRecommendationSection appRecommendations = PromotionObject.Instance.AppRecommendations;
							if (appRecommendations == null || appRecommendations.AppSuggestions.Count != 0)
							{
								goto IL_014b;
							}
						}
					}
					if (!mCurrentSidePanelVisibility.HasValue || mCurrentSidePanelVisibility != (SidePanelVisibility?)1)
					{
						ChangeSideRecommendationsVisibility(isAppRecommendationsVisible: false, isSearchBarVisible: true);
						mCurrentSidePanelVisibility = (SidePanelVisibility)1;
					}
				}
				return;
				IL_014b:
				if (FeatureManager.Instance.IsHtmlSideBar)
				{
					if (!SideHtmlBrowserInited)
					{
						InitiateSideHtmlBrowser();
					}
					if (!RegistryManager.Instance.IsPremium && (!mCurrentSidePanelVisibility.HasValue || mCurrentSidePanelVisibility != (SidePanelVisibility?)(-1039290612 ^ -1039290609)))
					{
						ChangeSideRecommendationsVisibility(isAppRecommendationsVisible: true, isSearchBarVisible: false);
						mCurrentSidePanelVisibility = (SidePanelVisibility)3;
					}
				}
				else
				{
					if (isContentReloadRequired || !mIsSidePanelContentLoadedOnce)
					{
						((Panel)mAppRecommendationSectionsPanel).Children.Clear();
						AppRecommendationSection appRecommendations2 = PromotionObject.Instance.AppRecommendations;
						RecommendedAppsSection recommendedAppsSection = new RecommendedAppsSection(appRecommendations2.AppSuggestionHeader);
						recommendedAppsSection.AddSuggestedApps(ParentWindow, appRecommendations2.AppSuggestions, appRecommendations2.ClientShowCount);
						if (((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.Count != 0)
						{
							((Panel)mAppRecommendationSectionsPanel).Children.Add((UIElement)(object)recommendedAppsSection);
							((UIElement)mAppRecommendationSectionsPanel).Visibility = (Visibility)0;
							((UIElement)mAppRecommendationsGenericMessages).Visibility = (Visibility)(-319216202 ^ -319216204);
							SendAppRecommendationsImpressionStats();
						}
						mIsSidePanelContentLoadedOnce = true;
					}
					if (!mCurrentSidePanelVisibility.HasValue || mCurrentSidePanelVisibility != (SidePanelVisibility?)0)
					{
						ChangeSideRecommendationsVisibility(isAppRecommendationsVisible: true, isSearchBarVisible: true);
						mCurrentSidePanelVisibility = (SidePanelVisibility)0;
					}
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in showing app recommendations, " + ex.ToString());
		}
	}

	internal void UpdateRecommendedAppsInstallStatus(string package)
	{
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		if (((Panel)mAppRecommendationSectionsPanel).Children.Count <= 0)
		{
			return;
		}
		int num = ((511830840 > 564946123) ? (-1) : (-1));
		int num2 = num;
		RecommendedAppsSection recommendedAppsSection = ((Panel)mAppRecommendationSectionsPanel).Children[0] as RecommendedAppsSection;
		RecommendedApps recommendedApps = null;
		for (int i = 0; i < ((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.Count; i++)
		{
			RecommendedApps recommendedApps2 = ((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children[i] as RecommendedApps;
			if (recommendedApps2.AppRecomendation.AppPackage.Equals(package, StringComparison.InvariantCultureIgnoreCase))
			{
				num2 = i;
				recommendedApps = recommendedApps2;
				break;
			}
		}
		if (num2 == 1955642574 - (0x70103C4F | 0x74809CC2))
		{
			return;
		}
		((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.RemoveAt(num2);
		if (sAppRecommendationsPool.Count > 0)
		{
			int num3 = 1;
			for (int j = 0; j < sAppRecommendationsPool.Count; j++)
			{
				RecommendedApps recommendedApps3 = sAppRecommendationsPool[j];
				if (!ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)recommendedApps3.AppRecomendation.ExtraPayload)["click_action_packagename"]))
				{
					recommendedApps3.RecommendedAppPosition = recommendedApps.RecommendedAppPosition;
					((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.Insert(num2, (UIElement)(object)recommendedApps3);
					JArray val = new JArray();
					JObject val2 = new JObject();
					val2.Add("app_loc", JToken.op_Implicit((((Dictionary<string, string>)(object)recommendedApps3.AppRecomendation.ExtraPayload)["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"));
					val2.Add("app_pkg", JToken.op_Implicit(((Dictionary<string, string>)(object)recommendedApps3.AppRecomendation.ExtraPayload)["click_action_packagename"]));
					val2.Add("is_installed", JToken.op_Implicit(ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)recommendedApps3.AppRecomendation.ExtraPayload)["click_action_packagename"]) ? "true" : "false"));
					val2.Add("app_position", JToken.op_Implicit(recommendedApps3.RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)));
					val2.Add("app_rank", JToken.op_Implicit(recommendedApps3.RecommendedAppRank.ToString(CultureInfo.InvariantCulture)));
					JObject val3 = val2;
					val.Add((JToken)(object)val3);
					ClientStats.SendFrontendClickStats("apps_recommendation", "impression", null, null, null, null, null, ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
					break;
				}
				num3++;
			}
			sAppRecommendationsPool.RemoveRange(0, num3);
		}
		if (((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.Count == 0)
		{
			((Panel)mAppRecommendationSectionsPanel).Children.Remove((UIElement)(object)recommendedAppsSection);
		}
		if (((Panel)mAppRecommendationSectionsPanel).Children.Count == 0)
		{
			((UIElement)mAppRecommendationSectionsPanel).Visibility = (Visibility)(757679220 - 757679218 % 1936408421);
			((UIElement)mAppRecommendationsGenericMessages).Visibility = (Visibility)0;
		}
	}

	private void SendAppRecommendationsImpressionStats()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		JArray val = new JArray();
		RecommendedAppsSection recommendedAppsSection = ((Panel)mAppRecommendationSectionsPanel).Children[0] as RecommendedAppsSection;
		for (int i = 0; i < ((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children.Count; i++)
		{
			RecommendedApps recommendedApps = ((Panel)recommendedAppsSection.mAppRecommendationsPanel).Children[i] as RecommendedApps;
			JObject val2 = new JObject();
			val2.Add("app_loc", JToken.op_Implicit((((Dictionary<string, string>)(object)recommendedApps.AppRecomendation.ExtraPayload)["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"));
			val2.Add("app_pkg", JToken.op_Implicit(((Dictionary<string, string>)(object)recommendedApps.AppRecomendation.ExtraPayload)["click_action_packagename"]));
			val2.Add("is_installed", JToken.op_Implicit(ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)recommendedApps.AppRecomendation.ExtraPayload)["click_action_packagename"]) ? "true" : "false"));
			val2.Add("app_position", JToken.op_Implicit(recommendedApps.RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)));
			val2.Add("app_rank", JToken.op_Implicit(recommendedApps.RecommendedAppRank.ToString(CultureInfo.InvariantCulture)));
			JObject val3 = val2;
			val.Add((JToken)(object)val3);
		}
		ClientStats.SendFrontendClickStats("apps_recommendation", "impression", null, null, null, null, null, ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
	}

	private void Search_MouseEnter(object sender, MouseEventArgs e)
	{
		searchHoverTimer.Start();
	}

	private void search_MouseLeave(object sender, MouseEventArgs e)
	{
		searchHoverTimer.Stop();
		if (!((UIElement)mSearchTextBox).IsFocused && !((UIElement)mSearchRecommendationBorder).IsMouseOver && !((UIElement)mSearchGrid).IsMouseOver)
		{
			HideSearchSuggestions();
		}
	}

	private void OpenSearchSuggestions()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((int)((UIElement)mSearchRecommendationBorder).Visibility == 0 || (!string.IsNullOrEmpty(mSearchTextBox.Text) && !(mSearchTextBox.Text == defaultSearchBoxText)) || ((Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations).Count <= 0 || !mIsShowSearchRecommendations)
			{
				return;
			}
			((Panel)searchRecomItems).Children.Clear();
			Separator val = new Separator
			{
				Margin = new Thickness(0.0)
			};
			object obj = ((FrameworkElement)this).FindResource((object)ToolBar.SeparatorStyleKey);
			((FrameworkElement)val).Style = (Style)((obj is Style) ? obj : null);
			Separator val2 = val;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.BackgroundProperty, "VerticalSeparator");
			((Panel)searchRecomItems).Children.Add((UIElement)(object)val2);
			Label val3 = new Label
			{
				Content = LocaleStrings.GetLocalizedString("STRING_MOST_SEARCHED_APPS", "")
			};
			BlueStacksUIBinding.BindColor((DependencyObject)(object)val3, Control.ForegroundProperty, "SearchGridForegroundColor");
			((Control)val3).FontSize = 14.0;
			((Control)val3).Padding = new Thickness(10.0, 5.0, 5.0, 5.0);
			((Panel)searchRecomItems).Children.Add((UIElement)(object)val3);
			foreach (KeyValuePair<string, SearchRecommendation> item in (Dictionary<string, SearchRecommendation>)(object)PromotionObject.Instance.SearchRecommendations)
			{
				RecommendedAppItem recommendedAppItem = new RecommendedAppItem();
				recommendedAppItem.Populate(ParentWindow, item.Value);
				((Control)recommendedAppItem).Padding = new Thickness(5.0, 0.0, 0.0, 0.0);
				((Panel)searchRecomItems).Children.Add((UIElement)(object)recommendedAppItem);
			}
			Border obj2 = mSearchRecommendationBorder;
			CornerRadius cornerRadius = searchTextBoxBorder.CornerRadius;
			double bottomRight = ((CornerRadius)(ref cornerRadius)).BottomRight;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			obj2.CornerRadius = new CornerRadius(0.0, 0.0, bottomRight, ((CornerRadius)(ref cornerRadius)).BottomLeft);
			Border mask = Mask;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			mask.CornerRadius = new CornerRadius(0.0, ((CornerRadius)(ref cornerRadius)).TopRight, 0.0, 0.0);
			Border obj3 = searchTextBoxBorder;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			double topLeft = ((CornerRadius)(ref cornerRadius)).TopLeft;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			obj3.CornerRadius = new CornerRadius(topLeft, ((CornerRadius)(ref cornerRadius)).TopRight, 0.0, 0.0);
			((UIElement)mSearchRecommendationBorder).Visibility = (Visibility)0;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception when trying to open search recommendations. " + ex.ToString());
		}
	}

	private void HideSearchSuggestions()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((UIElement)mSearchRecommendationBorder).Visibility == 0)
		{
			Border obj = searchTextBoxBorder;
			CornerRadius cornerRadius = searchTextBoxBorder.CornerRadius;
			double topLeft = ((CornerRadius)(ref cornerRadius)).TopLeft;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			double topRight = ((CornerRadius)(ref cornerRadius)).TopRight;
			cornerRadius = mSearchRecommendationBorder.CornerRadius;
			double bottomRight = ((CornerRadius)(ref cornerRadius)).BottomRight;
			cornerRadius = mSearchRecommendationBorder.CornerRadius;
			obj.CornerRadius = new CornerRadius(topLeft, topRight, bottomRight, ((CornerRadius)(ref cornerRadius)).BottomLeft);
			Border mask = Mask;
			cornerRadius = searchTextBoxBorder.CornerRadius;
			double topRight2 = ((CornerRadius)(ref cornerRadius)).TopRight;
			cornerRadius = mSearchRecommendationBorder.CornerRadius;
			mask.CornerRadius = new CornerRadius(0.0, topRight2, ((CornerRadius)(ref cornerRadius)).BottomRight, 0.0);
			((UIElement)mSearchRecommendationBorder).Visibility = (Visibility)(226738178 - (1258843166 << 1432991085));
		}
	}

	private void SearchTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		if (!ParentWindow.mWelcomeTab.IsPromotionVisible)
		{
			if (mSearchTextBox.Text == defaultSearchBoxText)
			{
				mSearchTextBox.Text = string.Empty;
			}
			OpenSearchSuggestions();
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
		}
	}

	private void SearchTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		if (!((UIElement)mSearchRecommendationBorder).IsMouseOver)
		{
			HideSearchSuggestions();
		}
		if (!ParentWindow.mWelcomeTab.IsPromotionVisible)
		{
			if (string.IsNullOrEmpty(mSearchTextBox.Text))
			{
				mSearchTextBox.Text = defaultSearchBoxText;
			}
			if (string.Equals(mSearchTextBox.Text, defaultSearchBoxText, StringComparison.InvariantCulture))
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundColor");
			}
			else
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
			}
		}
	}

	private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
	{
		SearchApp();
	}

	private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		if ((int)((UIElement)mSearchRecommendationBorder).Visibility == 0)
		{
			HideSearchSuggestions();
		}
		if ((int)e.Key == (0x3DEDE1DD ^ 0x3DEDE1DB))
		{
			SearchApp();
		}
	}

	private void SearchApp()
	{
		if (!ParentWindow.mWelcomeTab.IsPromotionVisible && !string.IsNullOrEmpty(mSearchTextBox.Text))
		{
			ParentWindow.mCommonHandler.SearchAppCenter(mSearchTextBox.Text);
		}
	}

	private void GetSearchTextFromCloud()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
				string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/app_center_searchdefaultquery");
				Logger.Debug("url for search api :" + urlWithParams);
				string text = BstHttpClient.Get(urlWithParams, (Dictionary<string, string>)null, false, string.Empty, 0, 1, 0, false, "bgp64");
				Logger.Debug("result for app_center_searchdefaultquery : " + text);
				JObject val = JObject.Parse(text);
				if ((bool)val["success"])
				{
					string text2 = ((object)val["result"]).ToString().Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						defaultSearchBoxText = text2;
					}
					Logger.Debug("response from search text cloud api :" + text2);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to fetch text from cloud... Err : " + ex.ToString());
			}
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				mSearchTextBox.Text = defaultSearchBoxText;
			}, new object[0]);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void SetWallpaper()
	{
		if (File.Exists(HomeAppManager.BackgroundImagePath))
		{
			mBackgroundImage.IsFullImagePath = true;
			mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
		}
	}

	private void HomeApp_BackgroundPromotionHandler(object sender, EventArgs e)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (string.IsNullOrEmpty(PromotionObject.Instance.BackgroundPromotionImagePath))
			{
				SetWallpaper();
			}
			else
			{
				mBackgroundImage.IsFullImagePath = true;
				mBackgroundImage.ImageName = PromotionObject.Instance.BackgroundPromotionImagePath;
			}
		}, new object[0]);
	}

	internal void RestoreWallpaperImage()
	{
		mBackgroundImage.IsFullImagePath = false;
		mBackgroundImage.ImageName = "fancybg.jpg";
		try
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				File.Delete(HomeAppManager.BackgroundImagePath);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in deletion of image:" + ex.ToString());
		}
	}

	internal void ApplyWallpaperImage()
	{
		mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
		mBackgroundImage.ReloadImages();
	}

	private void mAppSettingsPopup_Opened(object sender, EventArgs e)
	{
		((UIElement)mAppSettings).IsEnabled = false;
	}

	private void mAppSettingsPopup_Closed(object sender, EventArgs e)
	{
		((UIElement)mAppSettings).IsEnabled = true;
	}

	private void mInstallApkGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		new DownloadInstallApk(ParentWindow).ChooseAndInstallApk();
	}

	private void mDeleteAppsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ParentWindow.StaticComponents.ShowUninstallButtons(isShow: true);
		((Popup)mAppSettingsPopup).IsOpen = false;
	}

	private void Grid_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void Grid_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
	}

	private void mAppSettings_MouseEnter(object sender, MouseEventArgs e)
	{
		((Popup)mAppSettingsPopup).IsOpen = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/homeapp.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Expected O, but got Unknown
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected O, but got Unknown
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Expected O, but got Unknown
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Expected O, but got Unknown
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Expected O, but got Unknown
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Expected O, but got Unknown
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Expected O, but got Unknown
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Expected O, but got Unknown
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Expected O, but got Unknown
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Expected O, but got Unknown
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Expected O, but got Unknown
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Expected O, but got Unknown
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Expected O, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Expected O, but got Unknown
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Expected O, but got Unknown
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Expected O, but got Unknown
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Expected O, but got Unknown
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Expected O, but got Unknown
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Expected O, but got Unknown
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Expected O, but got Unknown
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Expected O, but got Unknown
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Expected O, but got Unknown
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Expected O, but got Unknown
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Expected O, but got Unknown
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(HomeApp)target).PreviewMouseDown += new MouseButtonEventHandler(HomeApp_PreviewMouseDown);
			break;
		case 2:
			mBackgroundImage = (CustomPictureBox)target;
			break;
		case 3:
			mInstalledAppText = (Label)target;
			((UIElement)mInstalledAppText).Visibility = (Visibility)(-1508280702 - (692859629 << 808802503));
			break;
		case 4:
			mGridSeparator = (Grid)target;
			break;
		case 5:
			mAppSettings = (CustomPictureBox)target;
			((UIElement)mAppSettings).MouseEnter += new MouseEventHandler(mAppSettings_MouseEnter);
			break;
		case 6:
			mAppSettingsPopup = (CustomPopUp)target;
			break;
		case 7:
			dummyGrid = (Grid)target;
			break;
		case 8:
			mAppSettingsPopupBorder = (Border)target;
			break;
		case 9:
			mMaskBorder1 = (Border)target;
			break;
		case 10:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)(Grid)target).MouseLeftButtonUp += new MouseButtonEventHandler(mInstallApkGrid_MouseLeftButtonUp);
			break;
		case 11:
			((UIElement)(Grid)target).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)(Grid)target).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)(Grid)target).MouseLeftButtonUp += new MouseButtonEventHandler(mDeleteAppsGrid_MouseLeftButtonUp);
			break;
		case 12:
			LeftArrow = (Path)target;
			break;
		case 13:
			InstalledAppsDrawerScrollBar = (ScrollViewer)target;
			InstalledAppsDrawerScrollBar.ScrollChanged += new ScrollChangedEventHandler(InstalledAppsDrawerScrollBar_ScrollChanged);
			break;
		case 14:
			mAppRecommendationsGrid = (Grid)target;
			break;
		case 15:
			mDiscoverApps = (TextBlock)target;
			break;
		case 16:
			appRecomScrollViewer = (ScrollViewer)target;
			break;
		case 17:
			mAppRecommendationSectionsPanel = (StackPanel)target;
			break;
		case 18:
			mAppRecommendationsGenericMessages = (StackPanel)target;
			break;
		case 19:
			mAppRecommendationsGenericMessageImage = (CustomPictureBox)target;
			break;
		case 20:
			mAppRecommendationsGenericMessageText = (TextBlock)target;
			break;
		case 21:
			mSearchGrid = (Border)target;
			((UIElement)mSearchGrid).Visibility = (Visibility)(-15514350 + (248229645 >> 1094116420));
			((UIElement)mSearchGrid).MouseEnter += new MouseEventHandler(Search_MouseEnter);
			((UIElement)mSearchGrid).MouseLeave += new MouseEventHandler(search_MouseLeave);
			break;
		case 22:
		{
			searchTextBoxBorder = (Border)target;
			Border obj = searchTextBoxBorder;
			int num = ((869688860 > 1689622251) ? 2 : 2);
			((UIElement)obj).Visibility = (Visibility)num;
			break;
		}
		case 23:
			mSearchTextBox = (TextBox)target;
			((UIElement)mSearchTextBox).Visibility = (Visibility)(-235730621 ^ -235730623);
			((UIElement)mSearchTextBox).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(SearchTextBox_GotKeyboardFocus);
			((UIElement)mSearchTextBox).LostKeyboardFocus += new KeyboardFocusChangedEventHandler(SearchTextBox_LostKeyboardFocus);
			((UIElement)mSearchTextBox).KeyDown += new KeyEventHandler(SearchTextBox_KeyDown);
			break;
		case 24:
			Mask = (Border)target;
			break;
		case 25:
			((UIElement)(CustomPictureBox)target).MouseUp += new MouseButtonEventHandler(CustomPictureBox_MouseUp);
			break;
		case 26:
			mSearchRecommendationBorder = (Border)target;
			((UIElement)mSearchRecommendationBorder).Visibility = (Visibility)(368233387 - 1894157188 % 1525923803);
			((UIElement)mSearchRecommendationBorder).MouseLeave += new MouseEventHandler(search_MouseLeave);
			break;
		case 27:
			searchRecomItems = (StackPanel)target;
			((UIElement)searchRecomItems).Visibility = (Visibility)(10200112 + ~10200109);
			break;
		case 28:
			mMultiInstanceControlsGrid = (Grid)target;
			break;
		case 29:
			mDockGridBorder = (Border)target;
			break;
		case 30:
			mDockGrid = (Grid)target;
			break;
		case 31:
			mDockPanel = (StackPanel)target;
			break;
		case 32:
			mDockAppIconToolTipPopup = (CustomPopUp)target;
			break;
		case 33:
			mMaskBorder2 = (Border)target;
			break;
		case 34:
			mDockIconText = (TextBlock)target;
			break;
		case 35:
			mDownArrow = (Path)target;
			break;
		case 36:
			mMoreAppsDockPopup = (CustomPopUp)target;
			break;
		case 37:
			mMaskBorder3 = (Border)target;
			break;
		case 38:
			mMoreAppsDockIconText = (TextBlock)target;
			break;
		case 39:
			mCustomMessageBoxCloseButton = (CustomPictureBox)target;
			((UIElement)mCustomMessageBoxCloseButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Close_PreviewMouseLeftButtonUp);
			break;
		case 40:
			mMoreAppsDockPanel = (StackPanel)target;
			break;
		case 41:
			mSuggestedAppPopUp = (CustomPopUp)target;
			break;
		case 42:
			mMaskBorder4 = (Border)target;
			break;
		case 43:
			mCloseAppSuggPopup = (CustomPictureBox)target;
			((UIElement)mCloseAppSuggPopup).MouseLeftButtonUp += new MouseButtonEventHandler(mCloseAppSuggPopup_MouseLeftButtonUp);
			break;
		case 44:
			mAppSuggestionPopUp = (TextBlock)target;
			break;
		case 45:
			UpArrow = (Path)target;
			break;
		case 46:
			mLoadingGrid = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	static HomeApp()
	{
		syncRoot = new object();
	}
}
