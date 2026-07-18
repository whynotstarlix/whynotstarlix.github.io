using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class RecommendedApps : UserControl, IComponentConnector
{
	private MainWindow mMainWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox recomIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock appNameTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock appGenreTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton installButton;

	private bool _contentLoaded;

	internal AppRecommendation AppRecomendation { get; set; }

	internal int RecommendedAppPosition { get; set; }

	internal int RecommendedAppRank { get; set; }

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

	public RecommendedApps()
	{
		InitializeComponent();
	}

	internal void Populate(AppRecommendation recom, int appPosition, int appRank)
	{
		AppRecomendation = recom;
		recomIcon.IsFullImagePath = true;
		recomIcon.ImageName = recom.ImagePath;
		appNameTextBlock.Text = ((Dictionary<string, string>)(object)recom.ExtraPayload)["click_action_title"];
		appGenreTextBlock.Text = recom.GameGenre;
		RecommendedAppPosition = appPosition;
		RecommendedAppRank = appRank;
	}

	private void Recommendation_Click(object sender, MouseButtonEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		try
		{
			JArray val = new JArray();
			JObject val2 = new JObject();
			val2.Add("app_loc", JToken.op_Implicit((((Dictionary<string, string>)(object)AppRecomendation.ExtraPayload)["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"));
			val2.Add("app_pkg", JToken.op_Implicit(((Dictionary<string, string>)(object)AppRecomendation.ExtraPayload)["click_action_packagename"]));
			val2.Add("is_installed", JToken.op_Implicit(ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)AppRecomendation.ExtraPayload)["click_action_packagename"]) ? "true" : "false"));
			val2.Add("app_position", JToken.op_Implicit(RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)));
			val2.Add("app_rank", JToken.op_Implicit(RecommendedAppRank.ToString(CultureInfo.InvariantCulture)));
			JObject val3 = val2;
			val.Add((JToken)(object)val3);
			ClientStats.SendFrontendClickStats("apps_recommendation", "click", null, ((Dictionary<string, string>)(object)AppRecomendation.ExtraPayload)["click_action_packagename"], null, null, null, ((JToken)val).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while sending stats to cloud for apps_recommendation_click " + ex.ToString());
		}
		ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)AppRecomendation.ExtraPayload, "search_suggestion");
	}

	private void UserControl_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mMainGrid, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
	}

	private void UserControl_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)mMainGrid).Background = (Brush)(object)Brushes.Transparent;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedapps.xaml", UriKind.Relative);
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMainGrid = (Grid)target;
			((UIElement)mMainGrid).MouseEnter += new MouseEventHandler(UserControl_MouseEnter);
			((UIElement)mMainGrid).MouseLeave += new MouseEventHandler(UserControl_MouseLeave);
			((UIElement)mMainGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Recommendation_Click);
			break;
		case 2:
			recomIcon = (CustomPictureBox)target;
			break;
		case 3:
			appNameTextBlock = (TextBlock)target;
			break;
		case 4:
			appGenreTextBlock = (TextBlock)target;
			break;
		case 5:
			installButton = (CustomButton)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
