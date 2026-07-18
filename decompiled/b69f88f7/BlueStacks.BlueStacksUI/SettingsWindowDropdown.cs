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
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SettingsWindowDropdown : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mPinOnTopButtonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPinOnTopButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mPinOnTopButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPinOnTopToggleButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFullScreenButtonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mFullScreenImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mFullScreenButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSyncOperationsButtonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSyncOperationsButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSyncOperationsButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSortingGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSortingButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSortingButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mAccountGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAccountButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAccountButtonText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mSettingsButtonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSettingsButtonImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSettingsButtonText;

	private bool _contentLoaded;

	public SettingsWindowDropdown()
	{
		InitializeComponent();
	}

	internal void Init(MainWindow window)
	{
		ParentWindow = window;
	}

	private void Grid_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		foreach (UIElement child in ((Panel)((sender is Grid) ? sender : null)).Children)
		{
			UIElement val = child;
			if (val is CustomPictureBox)
			{
				((CustomPictureBox)((val is CustomPictureBox) ? val : null)).ImageName = ((CustomPictureBox)((val is CustomPictureBox) ? val : null)).ImageName + "_hover";
			}
			if (val is TextBlock)
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)((val is TextBlock) ? val : null), Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
			}
		}
	}

	private void Grid_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
		foreach (UIElement child in ((Panel)((sender is Grid) ? sender : null)).Children)
		{
			UIElement val = child;
			if (val is CustomPictureBox)
			{
				((CustomPictureBox)((val is CustomPictureBox) ? val : null)).ImageName = ((CustomPictureBox)((val is CustomPictureBox) ? val : null)).ImageName.Replace("_hover", "");
			}
			if (val is TextBlock)
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)((val is TextBlock) ? val : null), Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
			}
		}
	}

	private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mNCTopBar.mSettingsDropdownPopup).IsOpen = false;
		ParentWindow.mCommonHandler.LaunchSettingsWindow();
	}

	private void FullscreenButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mNCTopBar.mSettingsDropdownPopup).IsOpen = false;
		if (!ParentWindow.mResizeHandler.IsMinMaxEnabled)
		{
			return;
		}
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (FeatureManager.Instance.IsCustomUIForDMM || ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
			{
				if (ParentWindow.mIsFullScreen)
				{
					ParentWindow.RestoreWindows();
				}
				else
				{
					ParentWindow.FullScreenWindow();
				}
			}
		}, new object[0]);
	}

	private void SortingButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mNCTopBar.mSettingsDropdownPopup).IsOpen = false;
		CommonHandlers.ArrangeWindow();
	}

	private void AccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mNCTopBar.mSettingsDropdownPopup).IsOpen = false;
		if (ParentWindow.mGuestBootCompleted)
		{
			ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sAndroidSettingsPackageName, BlueStacksUIUtils.sAndroidAccountSettingsActivityName, "account_tab", isSwitch: true, isLaunch: true);
		}
	}

	private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val.ImageName.Contains("_off"))
		{
			val.ImageName = "toggle_on";
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
			((Window)ParentWindow).Topmost = true;
		}
		else
		{
			val.ImageName = "toggle_off";
			ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			((Window)ParentWindow).Topmost = false;
		}
	}

	internal void LateInit()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
		{
			((UIElement)mSyncOperationsButtonGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SyncOperationsButton_PreviewMouseLeftButtonUp);
			((UIElement)mSyncOperationsButtonGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncOperationsButtonGrid).Opacity = 0.5;
			((UIElement)mSortingGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SortingButton_MouseLeftButtonUp);
			((UIElement)mSortingGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSortingGrid).Opacity = 0.5;
		}
		else
		{
			((UIElement)mSortingGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SortingButton_MouseLeftButtonUp);
			((UIElement)mSortingGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SortingButton_MouseLeftButtonUp);
			((UIElement)mSortingGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSortingGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSortingGrid).Opacity = 1.0;
			if ((BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName) && ParentWindow.mIsSyncMaster) || (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName) && BlueStacksUIUtils.DictWindows.Keys.Count - BlueStacksUIUtils.sSyncInvolvedInstances.Count > 1))
			{
				((UIElement)mSyncOperationsButtonGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SyncOperationsButton_PreviewMouseLeftButtonUp);
				((UIElement)mSyncOperationsButtonGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SyncOperationsButton_PreviewMouseLeftButtonUp);
				((UIElement)mSyncOperationsButtonGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
				((UIElement)mSyncOperationsButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
				((UIElement)mSyncOperationsButtonGrid).Opacity = 1.0;
			}
			else
			{
				((UIElement)mSyncOperationsButtonGrid).PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(SyncOperationsButton_PreviewMouseLeftButtonUp);
				((UIElement)mSyncOperationsButtonGrid).MouseEnter -= new MouseEventHandler(Grid_MouseEnter);
				((UIElement)mSyncOperationsButtonGrid).Opacity = 0.5;
			}
		}
		if (ParentWindow.EngineInstanceRegistry.IsClientOnTop)
		{
			mPinOnTopToggleButton.ImageName = "toggle_on";
		}
		else
		{
			mPinOnTopToggleButton.ImageName = "toggle_off";
		}
	}

	private void SyncOperationsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Popup)ParentWindow.mNCTopBar.mSettingsDropdownPopup).IsOpen = false;
		ParentWindow.ShowSynchronizerWindow();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/ncsettingsdropdown.xaml", UriKind.Relative);
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
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Expected O, but got Unknown
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected O, but got Unknown
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Expected O, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mPinOnTopButtonGrid = (Grid)target;
			((UIElement)mPinOnTopButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mPinOnTopButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			break;
		case 2:
			mPinOnTopButtonImage = (CustomPictureBox)target;
			break;
		case 3:
			mPinOnTopButtonText = (TextBlock)target;
			break;
		case 4:
			mPinOnTopToggleButton = (CustomPictureBox)target;
			((UIElement)mPinOnTopToggleButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PinOnTop_MouseLeftButtonUp);
			break;
		case 5:
			mFullScreenButtonGrid = (Grid)target;
			((UIElement)mFullScreenButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mFullScreenButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mFullScreenButtonGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(FullscreenButton_MouseLeftButtonUp);
			break;
		case 6:
			mFullScreenImage = (CustomPictureBox)target;
			break;
		case 7:
			mFullScreenButtonText = (TextBlock)target;
			break;
		case 8:
			mSyncOperationsButtonGrid = (Grid)target;
			((UIElement)mSyncOperationsButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSyncOperationsButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSyncOperationsButtonGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SyncOperationsButton_PreviewMouseLeftButtonUp);
			break;
		case 9:
			mSyncOperationsButtonImage = (CustomPictureBox)target;
			break;
		case 10:
			mSyncOperationsButtonText = (TextBlock)target;
			break;
		case 11:
			mSortingGrid = (Grid)target;
			((UIElement)mSortingGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSortingGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSortingGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SortingButton_MouseLeftButtonUp);
			break;
		case 12:
			mSortingButtonImage = (CustomPictureBox)target;
			break;
		case 13:
			mSortingButtonText = (TextBlock)target;
			break;
		case 14:
			mAccountGrid = (Grid)target;
			((UIElement)mAccountGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mAccountGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mAccountGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(AccountButton_MouseLeftButtonUp);
			break;
		case 15:
			mAccountButtonImage = (CustomPictureBox)target;
			break;
		case 16:
			mAccountButtonText = (TextBlock)target;
			break;
		case 17:
			mSettingsButtonGrid = (Grid)target;
			((UIElement)mSettingsButtonGrid).MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			((UIElement)mSettingsButtonGrid).MouseLeave += new MouseEventHandler(Grid_MouseLeave);
			((UIElement)mSettingsButtonGrid).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SettingsButton_MouseLeftButtonUp);
			break;
		case 18:
			mSettingsButtonImage = (CustomPictureBox)target;
			break;
		case 19:
			mSettingsButtonText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
