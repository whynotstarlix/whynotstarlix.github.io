using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI;

public class RecommendedAppsSection : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSectionHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mAppRecommendationsPanel;

	private bool _contentLoaded;

	public RecommendedAppsSection(string header)
	{
		InitializeComponent();
		mSectionHeader.Text = header;
	}

	internal void AddSuggestedApps(MainWindow ParentWindow, List<AppRecommendation> suggestedApps, int clientShowCount)
	{
		int num = 1;
		int num2 = 1;
		ParentWindow.mWelcomeTab.mHomeAppManager.ClearAppRecommendationPool();
		foreach (AppRecommendation suggestedApp in suggestedApps)
		{
			if (!ParentWindow.mAppHandler.IsAppInstalled(((Dictionary<string, string>)(object)suggestedApp.ExtraPayload)["click_action_packagename"]))
			{
				RecommendedApps recommendedApps = new RecommendedApps();
				recommendedApps.Populate(suggestedApp, num, num2);
				if (num <= clientShowCount)
				{
					((Panel)mAppRecommendationsPanel).Children.Add((UIElement)(object)recommendedApps);
					num++;
				}
				else
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.AddToAppRecommendationPool(recommendedApps);
				}
			}
			num2++;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedappssection.xaml", UriKind.Relative);
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mSectionHeader = (TextBlock)target;
			break;
		case 2:
			mAppRecommendationsPanel = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
