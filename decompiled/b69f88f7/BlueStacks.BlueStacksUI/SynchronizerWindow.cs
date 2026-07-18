using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SynchronizerWindow : CustomWindow, IComponentConnector
{
	private MainWindow ParentWindow;

	private bool mIsActiveWindowPresent;

	private bool mStopEventFromPropagatingFurther;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTopGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mLineSeperator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mNoActiveWindowsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mActiveWindowsListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSelectAllCheckbox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mActiveWindowsPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBottomGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mLineSeperator1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mStartSyncBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mLaunchInstanceManagerBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mSyncHelp;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Hyperlink mHyperLink;

	private bool _contentLoaded;

	public SynchronizerWindow(MainWindow parent)
	{
		ParentWindow = parent;
		((Window)this).Owner = (Window)(object)parent;
		((CustomWindow)this).IsShowGLWindow = true;
		InitializeComponent();
		string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=";
		mHyperLink.NavigateUri = new Uri(text + "operation_synchronization");
		((TextElementCollection<Inline>)(object)((Span)mHyperLink).Inlines).Clear();
		((Span)mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
		BlueStacksUIBinding.Instance.PropertyChanged += Binding_PropertyChanged;
		if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)mSyncHelp).Visibility = (Visibility)2;
		}
	}

	private void Binding_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "LocaleModel")
		{
			((TextElementCollection<Inline>)(object)((Span)mHyperLink).Inlines).Clear();
			((Span)mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
		}
	}

	internal void Init()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		mIsActiveWindowPresent = false;
		((Panel)mActiveWindowsPanel).Children.Clear();
		foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
		{
			if (dictWindow.Key != ParentWindow.mVmName && (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(dictWindow.Key) || ParentWindow.mSelectedInstancesForSync.Contains(dictWindow.Key)))
			{
				CustomCheckbox val = new CustomCheckbox
				{
					Content = GetInstanceGameOrDisplayName(dictWindow.Key),
					Tag = dictWindow.Key
				};
				if (val.Image != null)
				{
					((FrameworkElement)val.Image).Height = 16.0;
					((FrameworkElement)val.Image).Width = 16.0;
				}
				((FrameworkElement)val).Height = 25.0;
				((Control)val).FontSize = 16.0;
				((FrameworkElement)val).Margin = new Thickness(12.0, 8.0, 0.0, 0.0);
				if (ParentWindow.mSelectedInstancesForSync.Contains(((FrameworkElement)val).Tag.ToString()))
				{
					((ToggleButton)val).IsChecked = true;
				}
				else
				{
					((ToggleButton)val).IsChecked = false;
				}
				((UIElement)val).MouseEnter += new MouseEventHandler(InstanceCheckbox_MouseEnter);
				((UIElement)val).MouseLeave += new MouseEventHandler(InstanceCheckbox_MouseLeave);
				((ToggleButton)val).Checked += new RoutedEventHandler(InstanceCheckbox_Checked);
				((ToggleButton)val).Unchecked += new RoutedEventHandler(InstanceCheckbox_Unchecked);
				((Panel)mActiveWindowsPanel).Children.Add((UIElement)(object)val);
				mIsActiveWindowPresent = true;
				((UIElement)mActiveWindowsListScrollbar).Visibility = (Visibility)0;
			}
		}
		if (mIsActiveWindowPresent)
		{
			((UIElement)mLaunchInstanceManagerBtn).Visibility = (Visibility)2;
			((UIElement)mNoActiveWindowsGrid).Visibility = (Visibility)2;
			((UIElement)mStartSyncBtn).Visibility = (Visibility)0;
			if (ParentWindow.mIsSynchronisationActive)
			{
				((UIElement)mStartSyncBtn).IsEnabled = false;
			}
			else
			{
				ToggleStartSyncButton();
			}
			ToggleSelectAllCheckboxSelection();
		}
		else if (FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			Close_MouseLeftButtonUp(null, null);
		}
		else
		{
			((UIElement)mActiveWindowsListScrollbar).Visibility = (Visibility)2;
			((UIElement)mNoActiveWindowsGrid).Visibility = (Visibility)0;
			((UIElement)mStartSyncBtn).Visibility = (Visibility)2;
			((UIElement)mLaunchInstanceManagerBtn).Visibility = (Visibility)0;
		}
	}

	private void InstanceCheckbox_Unchecked(object sender, RoutedEventArgs e)
	{
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		CustomCheckbox val = (CustomCheckbox)((sender is CustomCheckbox) ? sender : null);
		((ToggleButton)val).IsChecked = false;
		ParentWindow.mSelectedInstancesForSync.Remove(((FrameworkElement)val).Tag.ToString());
		ToggleSelectAllCheckboxSelection();
		if (ParentWindow.mIsSynchronisationActive)
		{
			HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>)null, ((FrameworkElement)val).Tag.ToString(), 0, (Dictionary<string, string>)null, false, 1, 0);
			BlueStacksUIUtils.DictWindows[((FrameworkElement)val).Tag.ToString()]._TopBar.HideSyncPanel();
			if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(((FrameworkElement)val).Tag.ToString()))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Remove(((FrameworkElement)val).Tag.ToString());
			}
			if (ParentWindow.mSelectedInstancesForSync.Count == 0)
			{
				ParentWindow.mIsSynchronisationActive = false;
				ParentWindow.mIsSyncMaster = false;
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(ParentWindow.mVmName);
				}
				ParentWindow._TopBar.HideSyncPanel();
				ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
			}
			UpdateOtherSyncWindows();
		}
		if (!ParentWindow.mIsSynchronisationActive)
		{
			ToggleStartSyncButton();
		}
		mStopEventFromPropagatingFurther = false;
	}

	private void InstanceCheckbox_Checked(object sender, RoutedEventArgs e)
	{
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		CustomCheckbox val = (CustomCheckbox)((sender is CustomCheckbox) ? sender : null);
		((ToggleButton)val).IsChecked = true;
		ParentWindow.mSelectedInstancesForSync.Add(((FrameworkElement)val).Tag.ToString());
		ToggleSelectAllCheckboxSelection();
		if (ParentWindow.mIsSynchronisationActive)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "instance", ParentWindow.mVmName } };
			HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", dictionary, BlueStacksUIUtils.DictWindows[((FrameworkElement)((sender is CustomCheckbox) ? sender : null)).Tag.ToString()].mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
			BlueStacksUIUtils.DictWindows[((FrameworkElement)val).Tag.ToString()]._TopBar.ShowSyncPanel();
			if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(((FrameworkElement)val).Tag.ToString()))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Add(((FrameworkElement)val).Tag.ToString());
			}
			UpdateOtherSyncWindows();
		}
		else
		{
			ToggleStartSyncButton();
		}
		mStopEventFromPropagatingFurther = false;
	}

	private void mSelectAll_Checked(object sender, RoutedEventArgs e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		foreach (CustomCheckbox child in ((Panel)mActiveWindowsPanel).Children)
		{
			CustomCheckbox val = child;
			((ToggleButton)val).IsChecked = true;
			if (ParentWindow.mSelectedInstancesForSync.Contains(((FrameworkElement)val).Tag.ToString()))
			{
				continue;
			}
			ParentWindow.mSelectedInstancesForSync.Add(((FrameworkElement)val).Tag.ToString());
			if (ParentWindow.mIsSynchronisationActive)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "instance", ParentWindow.mVmName } };
				HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", dictionary, ((FrameworkElement)val).Tag.ToString(), 0, (Dictionary<string, string>)null, false, 1, 0);
				BlueStacksUIUtils.DictWindows[((FrameworkElement)val).Tag.ToString()]._TopBar.ShowSyncPanel();
				if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(((FrameworkElement)val).Tag.ToString()))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Add(((FrameworkElement)val).Tag.ToString());
				}
				UpdateOtherSyncWindows();
			}
		}
		ToggleStartSyncButton();
		mStopEventFromPropagatingFurther = false;
	}

	private void mSelectAll_Unchecked(object sender, RoutedEventArgs e)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		foreach (CustomCheckbox child in ((Panel)mActiveWindowsPanel).Children)
		{
			CustomCheckbox val = child;
			((ToggleButton)val).IsChecked = false;
			if (!ParentWindow.mSelectedInstancesForSync.Contains(((FrameworkElement)val).Tag.ToString()))
			{
				continue;
			}
			ParentWindow.mSelectedInstancesForSync.Remove(((FrameworkElement)val).Tag.ToString());
			if (ParentWindow.mIsSynchronisationActive)
			{
				HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>)null, ((FrameworkElement)val).Tag.ToString(), 0, (Dictionary<string, string>)null, false, 1, 0);
				BlueStacksUIUtils.DictWindows[((FrameworkElement)val).Tag.ToString()]._TopBar.HideSyncPanel();
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(((FrameworkElement)val).Tag.ToString()))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(((FrameworkElement)val).Tag.ToString());
				}
			}
		}
		if (ParentWindow.mIsSynchronisationActive)
		{
			ParentWindow.mIsSynchronisationActive = false;
			ParentWindow.mIsSyncMaster = false;
			if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Remove(ParentWindow.mVmName);
			}
			ParentWindow._TopBar.HideSyncPanel();
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
			UpdateOtherSyncWindows();
		}
		ToggleStartSyncButton();
		mStopEventFromPropagatingFurther = false;
	}

	private void InstanceCheckbox_MouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is CustomCheckbox) ? sender : null), Control.BackgroundProperty, "SettingsWindowBackground");
	}

	private void InstanceCheckbox_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is CustomCheckbox) ? sender : null), Control.BackgroundProperty, "GameControlNavigationBackgroundColor");
	}

	private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
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

	private void mStartSyncBtn_Click(object sender, RoutedEventArgs e)
	{
		((UIElement)mStartSyncBtn).IsEnabled = false;
		ParentWindow._TopBar.ShowSyncPanel(show: true);
		ParentWindow.mIsSyncMaster = true;
		if (!RegistryManager.Instance.IsSynchronizerUsedStatSent)
		{
			ClientStats.SendMiscellaneousStatsAsync("MultipleInstancesSynced", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null);
			RegistryManager.Instance.IsSynchronizerUsedStatSent = true;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		IEnumerable<CustomCheckbox> source = from _ in ((IEnumerable)((Panel)mActiveWindowsPanel).Children).OfType<CustomCheckbox>()
			where ((ToggleButton)_).IsChecked == true
			select _;
		if (source.Any())
		{
			ParentWindow.mIsSynchronisationActive = true;
			dictionary.Add("instances", string.Join(",", source.Select((CustomCheckbox _) => ((FrameworkElement)_).Tag.ToString()).ToArray()));
			ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startOperationsSync", dictionary);
			source.ToList().ForEach(delegate(CustomCheckbox customCheckbox)
			{
				BlueStacksUIUtils.DictWindows[((FrameworkElement)customCheckbox).Tag.ToString()]._TopBar.ShowSyncPanel();
			});
		}
		foreach (CustomCheckbox item in source.ToList())
		{
			if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(((FrameworkElement)item).Tag.ToString()))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Add(((FrameworkElement)item).Tag.ToString());
			}
		}
		if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName))
		{
			BlueStacksUIUtils.sSyncInvolvedInstances.Add(ParentWindow.mVmName);
		}
		UpdateOtherSyncWindows();
		Close_MouseLeftButtonUp(null, null);
		if (RegistryManager.Instance.IsShowToastNotification)
		{
			ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STARTED", ""));
		}
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		((Window)this).Hide();
		((CustomWindow)this).ShowWithParentWindow = false;
		if (ParentWindow != null)
		{
			((UIElement)ParentWindow).Focus();
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

	private void mLaunchInstanceManagerBtn_Click(object sender, RoutedEventArgs e)
	{
		BlueStacksUIUtils.LaunchMultiInstanceManager();
		ClientStats.SendMiscellaneousStatsAsync("syncWindow", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem);
	}

	private void ToggleStartSyncButton()
	{
		if (ParentWindow.mSelectedInstancesForSync.Count > 0)
		{
			((UIElement)mStartSyncBtn).IsEnabled = true;
		}
		else
		{
			((UIElement)mStartSyncBtn).IsEnabled = false;
		}
	}

	private void ToggleSelectAllCheckboxSelection()
	{
		mStopEventFromPropagatingFurther = true;
		if (ParentWindow.mSelectedInstancesForSync.Count == ((Panel)mActiveWindowsPanel).Children.Count)
		{
			((ToggleButton)mSelectAllCheckbox).IsChecked = true;
		}
		else
		{
			((ToggleButton)mSelectAllCheckbox).IsChecked = false;
		}
		mStopEventFromPropagatingFurther = false;
	}

	private void SynchronizerWindow_Activated(object sender, EventArgs e)
	{
		if (((Panel)mActiveWindowsPanel).Children.Count == 0)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				Close_MouseLeftButtonUp(null, null);
			}
			else
			{
				mIsActiveWindowPresent = false;
				((UIElement)mActiveWindowsListScrollbar).Visibility = (Visibility)2;
				((UIElement)mStartSyncBtn).Visibility = (Visibility)2;
				((UIElement)mNoActiveWindowsGrid).Visibility = (Visibility)0;
				((UIElement)mLaunchInstanceManagerBtn).Visibility = (Visibility)0;
				((FrameworkElement)mNoActiveWindowsGrid).Height = double.NaN;
				((Window)this).SizeToContent = (SizeToContent)3;
			}
		}
		((Window)this).Left = ((Window)ParentWindow).Left + (((FrameworkElement)ParentWindow).Width - ((FrameworkElement)this).Width) / 2.0;
		((Window)this).Top = ((Window)ParentWindow).Top + (((FrameworkElement)ParentWindow).Height - ((FrameworkElement)this).Height) / 2.0;
	}

	internal void PauseAllSyncOperations()
	{
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		foreach (string item in ParentWindow.mSelectedInstancesForSync)
		{
			BlueStacksUIUtils.DictWindows[item]._TopBar.HideSyncPanel();
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "pause", "true" } };
		HTTPUtils.SendRequestToEngineAsync("playPauseSync", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
		mStopEventFromPropagatingFurther = false;
	}

	internal void StopAllSyncOperations()
	{
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		ParentWindow.mIsSynchronisationActive = false;
		ParentWindow.mIsSyncMaster = false;
		foreach (string item in ParentWindow.mSelectedInstancesForSync)
		{
			BlueStacksUIUtils.DictWindows[item]._TopBar.HideSyncPanel();
			if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(item))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Remove(item);
			}
		}
		if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(ParentWindow.mVmName))
		{
			BlueStacksUIUtils.sSyncInvolvedInstances.Remove(ParentWindow.mVmName);
		}
		UpdateOtherSyncWindows();
		ParentWindow.mSelectedInstancesForSync.Clear();
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
		Init();
		mStopEventFromPropagatingFurther = false;
	}

	internal void PlayAllSyncOperations()
	{
		if (mStopEventFromPropagatingFurther)
		{
			return;
		}
		mStopEventFromPropagatingFurther = true;
		foreach (string item in ParentWindow.mSelectedInstancesForSync)
		{
			BlueStacksUIUtils.DictWindows[item]._TopBar.ShowSyncPanel();
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string> { { "pause", "false" } };
		HTTPUtils.SendRequestToEngineAsync("playPauseSync", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0);
		mStopEventFromPropagatingFurther = false;
	}

	private void UpdateOtherSyncWindows()
	{
		try
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
				{
					if (dictWindow.Key != ParentWindow.mVmName && dictWindow.Value.mSynchronizerWindow != null && ((UIElement)dictWindow.Value.mSynchronizerWindow).IsVisible)
					{
						dictWindow.Value.mSynchronizerWindow.Init();
					}
				}
			}, new object[0]);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
		}
	}

	private static string GetInstanceGameOrDisplayName(string vmName)
	{
		string appName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.AppName;
		string characterName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.CharacterName;
		if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(characterName))
		{
			return appName + " " + characterName;
		}
		return Utils.GetDisplayName(vmName, "bgp64");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/synchronizerwindow.xaml", UriKind.Relative);
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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((Window)(SynchronizerWindow)target).Activated += SynchronizerWindow_Activated;
			break;
		case 2:
			mMaskBorder = (Border)target;
			break;
		case 3:
			mTopGrid = (Grid)target;
			((UIElement)mTopGrid).MouseDown += new MouseButtonEventHandler(Topbar_MouseDown);
			break;
		case 4:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 5:
			mLineSeperator = (Border)target;
			break;
		case 6:
			mNoActiveWindowsGrid = (Grid)target;
			break;
		case 7:
			mActiveWindowsListScrollbar = (ScrollViewer)target;
			break;
		case 8:
			mSelectAllCheckbox = (CustomCheckbox)target;
			((ToggleButton)mSelectAllCheckbox).Checked += new RoutedEventHandler(mSelectAll_Checked);
			((ToggleButton)mSelectAllCheckbox).Unchecked += new RoutedEventHandler(mSelectAll_Unchecked);
			break;
		case 9:
			mActiveWindowsPanel = (StackPanel)target;
			break;
		case 10:
			mBottomGrid = (Grid)target;
			break;
		case 11:
			mLineSeperator1 = (Border)target;
			break;
		case 12:
			mStartSyncBtn = (CustomButton)target;
			((ButtonBase)mStartSyncBtn).Click += new RoutedEventHandler(mStartSyncBtn_Click);
			break;
		case 13:
			mLaunchInstanceManagerBtn = (CustomButton)target;
			((ButtonBase)mLaunchInstanceManagerBtn).Click += new RoutedEventHandler(mLaunchInstanceManagerBtn_Click);
			break;
		case 14:
			mSyncHelp = (TextBlock)target;
			break;
		case 15:
			mHyperLink = (Hyperlink)target;
			mHyperLink.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
