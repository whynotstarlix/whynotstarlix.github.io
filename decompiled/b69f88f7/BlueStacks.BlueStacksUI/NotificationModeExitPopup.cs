using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class NotificationModeExitPopup : UserControl, IDimOverlayControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBackground;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mClosebtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mIconBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button mYesBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCloseBluestacks;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mPreferenceCheckBox;

	private bool _contentLoaded;

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow { get; set; } = true;

	public bool ShowTransparentWindow { get; set; }

	public MainWindow ParentWindow { get; private set; }

	public string PackageName { get; private set; }

	public NotificationModeExitPopup(MainWindow window, string packageName)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		InitializeComponent();
		ParentWindow = window;
		PackageName = packageName;
		string uriString = (File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico") ? (Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico") : ((!File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png")) ? Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets\\BlueStacks.ico") : (Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png")));
		mIconBorder.Background = (Brush)new ImageBrush((ImageSource)new BitmapImage(new Uri(uriString)));
		Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_shown", ParentWindow?.mVmName, packageName, "", "");
		BitmapImage val = new BitmapImage(new Uri(uriString));
		CroppedBitmap source = new CroppedBitmap((BitmapSource)(object)val, new Int32Rect((int)((ImageSource)val).Width / 10, (int)((ImageSource)val).Height * 2 / 10, (int)((ImageSource)val).Width * 8 / 10, (int)((ImageSource)val).Height * 8 / 10));
		VisualBrush background = new VisualBrush((Visual)new Image
		{
			Source = (ImageSource)(object)source,
			Effect = (Effect)new BlurEffect
			{
				Radius = 24.0,
				KernelType = (KernelType)0
			},
			ClipToBounds = true
		})
		{
			Opacity = 0.4
		};
		mBackground.Background = (Brush)(object)background;
	}

	bool IDimOverlayControl.Close()
	{
		Close();
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	private void Close()
	{
		try
		{
			ParentWindow.HideDimOverlay();
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			((UIElement)this).Visibility = (Visibility)1;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
		}
	}

	private void mClosebtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_closed", ParentWindow.mVmName, "", "", "");
		Close();
	}

	private void mYesBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mYesBtn, Control.BackgroundProperty, "BlueMouseDownBorderBackground");
		Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_yes", ParentWindow.mVmName, "", "", "");
		ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
		RegistryManager.Instance.IsNotificationModeAlwaysOn = true;
		JsonParser val = new JsonParser(ParentWindow.mVmName);
		NotificationManager.Instance.UpdateMuteState((MuteState)0, val.GetAppNameFromPackage(PackageName));
		Close();
		ParentWindow.MinimizeWindowHandler();
	}

	private void mCloseBluestacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_no", ParentWindow.mVmName, "", "", "");
		InstanceRegistry engineInstanceRegistry = ParentWindow.EngineInstanceRegistry;
		int notificationModePopupShownCount = engineInstanceRegistry.NotificationModePopupShownCount;
		engineInstanceRegistry.NotificationModePopupShownCount = notificationModePopupShownCount + 1;
		ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
		RegistryManager.Instance.IsNotificationModeAlwaysOn = false;
		Close();
		ParentWindow.CloseWindowHandler();
	}

	private void mYesBtn_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mYesBtn, Control.BackgroundProperty, "BlueMouseInGridBackGround");
	}

	private void mYesBtn_MouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mYesBtn, Control.BackgroundProperty, "BlueMouseOutGridBackground");
	}

	private void mPreferenceCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = !(((ToggleButton)mPreferenceCheckBox).IsChecked ?? true);
		Stats.SendCommonClientStatsAsync("notification_mode", "exit_popup_preference", "Android", "", "", "");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/notificationmodeexitpopup.xaml", UriKind.Relative);
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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mBackground = (Border)target;
			break;
		case 2:
			mMainGrid = (Grid)target;
			break;
		case 3:
			mMaskBorder = (Border)target;
			break;
		case 4:
			mClosebtn = (CustomPictureBox)target;
			((UIElement)mClosebtn).MouseLeftButtonUp += new MouseButtonEventHandler(mClosebtn_MouseLeftButtonUp);
			break;
		case 5:
			mIconBorder = (Border)target;
			break;
		case 6:
			mYesBtn = (Button)target;
			((ButtonBase)mYesBtn).Click += new RoutedEventHandler(mYesBtn_Click);
			((UIElement)mYesBtn).MouseEnter += new MouseEventHandler(mYesBtn_MouseEnter);
			((UIElement)mYesBtn).MouseLeave += new MouseEventHandler(mYesBtn_MouseLeave);
			break;
		case 7:
			mCloseBluestacks = (TextBlock)target;
			((UIElement)mCloseBluestacks).MouseLeftButtonUp += new MouseButtonEventHandler(mCloseBluestacks_MouseLeftButtonUp);
			break;
		case 8:
			mPreferenceCheckBox = (CustomCheckbox)target;
			((ToggleButton)mPreferenceCheckBox).Checked += new RoutedEventHandler(mPreferenceCheckBox_Checked);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
