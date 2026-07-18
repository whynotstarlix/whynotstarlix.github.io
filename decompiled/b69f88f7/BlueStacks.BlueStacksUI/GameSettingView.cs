using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class GameSettingView : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mGuideBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mKnowMoreLink;

	private bool _contentLoaded;

	public GameSettingView()
	{
		InitializeComponent();
		((TextElementCollection<Inline>)(object)((Span)mKnowMoreLink).Inlines).Clear();
		((Span)mKnowMoreLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_KNOW_MORE", ""));
	}

	private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		BluestacksUIColor.ScrollBarScrollChanged(sender, e);
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		try
		{
			Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
			Utils.OpenUrl(e.Uri.AbsoluteUri);
			((RoutedEventArgs)e).Handled = true;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in opening url" + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/gamesettingview.xaml", UriKind.Relative);
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
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((ScrollViewer)target).ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
			break;
		case 2:
			mGuideBtn = (CustomButton)target;
			break;
		case 3:
			mKnowMoreLink = (Hyperlink)target;
			mKnowMoreLink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
