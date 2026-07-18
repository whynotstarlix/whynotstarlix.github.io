using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class RecommendedAppItem : UserControl, IComponentConnector
{
	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox recomIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock appNameTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton installButton;

	private bool _contentLoaded;

	internal SearchRecommendation SearchRecomendation { get; set; }

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

	public RecommendedAppItem()
	{
		InitializeComponent();
	}

	internal void Populate(MainWindow parentWindow, SearchRecommendation recom)
	{
		mMainWindow = parentWindow;
		recomIcon.IsFullImagePath = true;
		recomIcon.ImageName = recom.ImagePath;
		installButton.ButtonColor = (ButtonColors)10;
		((ContentControl)installButton).Content = (ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)recom.ExtraPayload)["click_action_packagename"]) ? LocaleStrings.GetLocalizedString("STRING_PLAY", "") : LocaleStrings.GetLocalizedString("STRING_INSTALL", ""));
		appNameTextBlock.Text = ((Dictionary<string, string>)(object)recom.ExtraPayload)["click_action_title"];
		SearchRecomendation = recom;
	}

	private void Recommendation_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			ClientStats.SendFrontendClickStats("search_suggestion_click", "", (((Dictionary<string, string>)(object)SearchRecomendation.ExtraPayload)["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay", ((Dictionary<string, string>)(object)SearchRecomendation.ExtraPayload)["click_action_packagename"], ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)SearchRecomendation.ExtraPayload)["click_action_packagename"]) ? "true" : "false", null, null, null);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while sending stats to cloud for search_suggestion_click " + ex.ToString());
		}
		ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)SearchRecomendation.ExtraPayload, "search_suggestion");
	}

	private void UserControl_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)this, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
	}

	private void UserControl_MouseLeave(object sender, MouseEventArgs e)
	{
		((Control)this).Background = (Brush)(object)Brushes.Transparent;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedappitem.xaml", UriKind.Relative);
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
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(RecommendedAppItem)target).MouseUp += new MouseButtonEventHandler(Recommendation_Click);
			((UIElement)(RecommendedAppItem)target).MouseEnter += new MouseEventHandler(UserControl_MouseEnter);
			((UIElement)(RecommendedAppItem)target).MouseLeave += new MouseEventHandler(UserControl_MouseLeave);
			break;
		case 2:
			recomIcon = (CustomPictureBox)target;
			break;
		case 3:
			appNameTextBlock = (TextBlock)target;
			break;
		case 4:
			installButton = (CustomButton)target;
			((ButtonBase)installButton).Click += new RoutedEventHandler(Recommendation_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
