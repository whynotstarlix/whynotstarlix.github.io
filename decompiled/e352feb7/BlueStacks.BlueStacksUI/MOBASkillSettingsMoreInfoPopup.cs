using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MOBASkillSettingsMoreInfoPopup : CustomPopUp, IComponentConnector
{
	private CanvasElement mCanvasElement;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder4;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mHyperLink;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path LeftArrow;

	private bool _contentLoaded;

	public MOBASkillSettingsMoreInfoPopup(CanvasElement canvasElement)
	{
		mCanvasElement = canvasElement;
		InitializeComponent();
		((Popup)this).PlacementTarget = (UIElement)(object)mCanvasElement?.MOBASkillSettingsPopup.mHelpIcon;
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		try
		{
			Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
			mCanvasElement.SendMOBAStats("read_more_clicked");
			BlueStacksUIUtils.OpenUrl(e.Uri.AbsoluteUri);
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
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingsmoreinfopopup.xaml", UriKind.Relative);
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
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMOBASkillSettingsMoreInfoPopup = (MOBASkillSettingsMoreInfoPopup)target;
			break;
		case 2:
			mMaskBorder4 = (Border)target;
			break;
		case 3:
			mHyperLink = (Hyperlink)target;
			mHyperLink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 4:
			LeftArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
