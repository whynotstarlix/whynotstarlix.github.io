using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class QuitActionElement : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private QuitPopupControl ParentQuitPopup;

	private string mQuitActionValue = string.Empty;

	private string mCTAEventName = string.Empty;

	private QuitActionItemCTA mCallToAction;

	public static readonly DependencyProperty ActionElementProperty = DependencyProperty.Register("ActionElement", typeof(QuitActionItem), typeof(QuitActionElement), new PropertyMetadata((object)QuitActionItem.None, new PropertyChangedCallback(ActionElementPropertyChangedCallback)));

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border maskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mExternalLinkImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mMainImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mBodyTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mHyperlinkTextBlock;

	private bool _contentLoaded;

	public string ParentPopupTag { get; set; } = string.Empty;

	public QuitActionItem ActionElement
	{
		get
		{
			return (QuitActionItem)((DependencyObject)this).GetValue(ActionElementProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ActionElementProperty, (object)value);
		}
	}

	public QuitActionElement(MainWindow window, QuitPopupControl qpc)
	{
		ParentWindow = window;
		ParentQuitPopup = qpc;
		InitializeComponent();
	}

	private void SetProperties(QuitActionItem item)
	{
		string value = QuitActionCollection.Actions[item][QuitActionItemProperty.CallToAction];
		mBodyTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.BodyText];
		mMainImage.ImageName = QuitActionCollection.Actions[item][QuitActionItemProperty.ImageName];
		mHyperlinkTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionText];
		mQuitActionValue = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionValue];
		mCTAEventName = QuitActionCollection.Actions[item][QuitActionItemProperty.StatEventName];
		mCallToAction = (QuitActionItemCTA)Enum.Parse(typeof(QuitActionItemCTA), value, ignoreCase: true);
	}

	private static void ActionElementPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		QuitActionElement quitActionElement = d as QuitActionElement;
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)quitActionElement))
		{
			quitActionElement.SetProperties((QuitActionItem)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}

	private void QAE_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			switch (mCallToAction)
			{
			case QuitActionItemCTA.OpenLinkInBrowser:
				if (!string.IsNullOrEmpty(mQuitActionValue))
				{
					BlueStacksUIUtils.OpenUrl(mQuitActionValue);
				}
				SendCTAStat();
				break;
			case QuitActionItemCTA.OpenApplication:
				if (!string.IsNullOrEmpty(mQuitActionValue))
				{
					Process.Start(mQuitActionValue);
				}
				SendCTAStat();
				break;
			case QuitActionItemCTA.OpenAppCenter:
				OpenAppCenter();
				ParentQuitPopup.Close();
				SendCTAStat();
				break;
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Some error while CallToAction of QuitPopup. Ex: {0}", new object[1] { ex });
		}
	}

	private void OpenAppCenter()
	{
		try
		{
			ParentWindow?.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl(null), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter");
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't open app center. Ex: {0}", new object[1] { ex });
		}
	}

	private void SendCTAStat()
	{
		ClientStats.SendLocalQuitPopupStatsAsync(ParentPopupTag, mCTAEventName);
	}

	private void QAE_MouseEnter(object sender, MouseEventArgs e)
	{
		((UIElement)mExternalLinkImage).Visibility = (Visibility)1;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)maskBorder, Border.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		((FrameworkElement)this).Cursor = Cursors.Hand;
	}

	private void QAE_MouseLeave(object sender, MouseEventArgs e)
	{
		((UIElement)mExternalLinkImage).Visibility = (Visibility)1;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)maskBorder, Border.BackgroundProperty, "LightBandingColor");
		((FrameworkElement)this).Cursor = Cursors.Arrow;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitactionelement.xaml", UriKind.Relative);
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(QuitActionElement)target).MouseEnter += new MouseEventHandler(QAE_MouseEnter);
			((UIElement)(QuitActionElement)target).MouseLeave += new MouseEventHandler(QAE_MouseLeave);
			((UIElement)(QuitActionElement)target).PreviewMouseUp += new MouseButtonEventHandler(QAE_PreviewMouseUp);
			break;
		case 2:
			maskBorder = (Border)target;
			break;
		case 3:
			mParentGrid = (Grid)target;
			break;
		case 4:
			mExternalLinkImage = (CustomPictureBox)target;
			break;
		case 5:
			mMainImage = (CustomPictureBox)target;
			break;
		case 6:
			mBodyTextBlock = (TextBlock)target;
			break;
		case 7:
			mHyperlinkTextBlock = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
