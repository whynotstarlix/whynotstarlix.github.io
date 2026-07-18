using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SpeedUpBluestacksUserControl : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mBodyText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mHyperLink;

	private bool _contentLoaded;

	public SpeedUpBluestacksUserControl()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		InitializeComponent();
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((TextElement)mHyperLink).Foreground = (Brush)(SolidColorBrush)((TypeConverter)new BrushConverter()).ConvertFrom((object)"#FF328CF2");
		}
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		try
		{
			Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
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
			Uri uri = new Uri("/Bluestacks;component/controls/speedupbluestacksusercontrol.xaml", UriKind.Relative);
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mTitleText = (TextBlock)target;
			break;
		case 2:
			mBodyText = (TextBlock)target;
			break;
		case 3:
			mImage = (CustomPictureBox)target;
			break;
		case 4:
			mHyperLink = (Hyperlink)target;
			mHyperLink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
