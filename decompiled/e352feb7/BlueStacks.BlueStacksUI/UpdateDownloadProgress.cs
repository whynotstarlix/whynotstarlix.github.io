using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class UpdateDownloadProgress : CustomWindow, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal UpdateDownloadProgress mUpdateDownloadProgressUserControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock titleLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mDetailedChangeLogs;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BlueProgressBar mUpdateDownloadProgressBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mUpdateDownloadProgressPercentage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mHideBtn;

	private bool _contentLoaded;

	public UpdateDownloadProgress()
	{
		InitializeComponent();
		((CustomWindow)this).IsShowGLWindow = true;
		((TextElementCollection<Inline>)(object)((Span)mDetailedChangeLogs).Inlines).Clear();
		((Span)mDetailedChangeLogs).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
		mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
	}

	private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (!((RoutedEventArgs)e).OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
		{
			try
			{
				((Window)this).DragMove();
			}
			catch
			{
			}
		}
	}

	private void HideBtn_Click(object sender, RoutedEventArgs e)
	{
		((Window)this).Hide();
	}

	private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Window)this).Hide();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		((RoutedEventArgs)e).Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/updatedownloadprogress.xaml", UriKind.Relative);
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mUpdateDownloadProgressUserControl = (UpdateDownloadProgress)target;
			break;
		case 2:
			mMaskBorder = (Border)target;
			break;
		case 3:
			((UIElement)(Grid)target).MouseLeftButtonDown += new MouseButtonEventHandler(Grid_MouseLeftButtonDown);
			break;
		case 4:
			titleLabel = (TextBlock)target;
			break;
		case 5:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(mCloseBtn_PreviewMouseLeftButtonUp);
			break;
		case 6:
			mDetailedChangeLogs = (Hyperlink)target;
			mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		case 7:
			mUpdateDownloadProgressBar = (BlueProgressBar)target;
			break;
		case 8:
			mUpdateDownloadProgressPercentage = (Label)target;
			break;
		case 9:
			mHideBtn = (CustomButton)target;
			((ButtonBase)mHideBtn).Click += new RoutedEventHandler(HideBtn_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
