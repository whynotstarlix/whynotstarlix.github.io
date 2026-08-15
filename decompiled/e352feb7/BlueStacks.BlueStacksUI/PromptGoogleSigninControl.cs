using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PromptGoogleSigninControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton SigninLaterBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton SigninBtn;

	private bool _contentLoaded;

	public PromptGoogleSigninControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
	}

	private void CloseBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		try
		{
			ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClose", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID);
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in CloseBtn_MouseLeftButtonUp. Exception: " + ex);
		}
	}

	private void SigninBtn_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			AppIconModel appIcon = ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
			if (appIcon != null)
			{
				ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, isSwitch: true, isLaunch: true);
			}
			ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClick", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID);
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SigninBtn_Click. Exception: " + ex);
		}
	}

	private void SigninLaterBtn_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			ClientStats.SendMiscellaneousStatsAsync("GoogleSigninLater", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID);
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SigninLaterBtn_Click. Exception: " + ex);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/promptgooglesignincontrol.xaml", UriKind.Relative);
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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			CloseBtn = (CustomPictureBox)target;
			((UIElement)CloseBtn).PreviewMouseDown += new MouseButtonEventHandler(CloseBtn_PreviewMouseDown);
			((UIElement)CloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_MouseLeftButtonUp);
			break;
		case 2:
			SigninLaterBtn = (CustomButton)target;
			((ButtonBase)SigninLaterBtn).Click += new RoutedEventHandler(SigninLaterBtn_Click);
			break;
		case 3:
			SigninBtn = (CustomButton)target;
			((ButtonBase)SigninBtn).Click += new RoutedEventHandler(SigninBtn_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
