using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class BackupRestoreSettingsControl : UserControl, IComponentConnector
{
	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mDiskCleanupGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mDiskCleanupBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Line mLineSeperator;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBackupRestoreGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mRestoreBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mBackupBtn;

	private bool _contentLoaded;

	public BackupRestoreSettingsControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		if (ParentWindow != null && !ParentWindow.IsDefaultVM)
		{
			((UIElement)mBackupRestoreGrid).Visibility = (Visibility)2;
			((UIElement)mLineSeperator).Visibility = (Visibility)2;
		}
		((UIElement)this).Visibility = (Visibility)1;
	}

	private void RestoreBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageWindow val = new CustomMessageWindow
		{
			ImageName = "backup_restore_popup_window"
		};
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_RESTORE_BACKUP", "");
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_MAKE_SURE_LATEST_WARNING", "");
		val.AddButton((ButtonColors)4, "STRING_RESTORE_BUTTON", (EventHandler)delegate
		{
			LaunchDataManager("restore");
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow;
		((Window)val).ShowDialog();
	}

	private void BackupBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		CustomMessageWindow val = new CustomMessageWindow
		{
			ImageName = "backup_restore_popup_window"
		};
		BlueStacksUIBinding.Bind(val.TitleTextBlock, "STRING_BACKUP_WARNING", "");
		BlueStacksUIBinding.Bind(val.BodyTextBlock, "STRING_BLUESTACKS_BACKUP_PROMPT", "");
		val.AddButton((ButtonColors)4, "STRING_BACKUP", (EventHandler)delegate
		{
			LaunchDataManager("backup");
		}, (string)null, false, (object)null);
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
		((Window)val).Owner = (Window)(object)ParentWindow;
		((Window)val).ShowDialog();
	}

	private void DiskCleanupBtn_Click(object sender, RoutedEventArgs e)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp64"))
			{
				CustomMessageWindow val = new CustomMessageWindow
				{
					ImageName = "disk_cleanup_popup_window"
				};
				val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_HEADING", "");
				val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_MESSAGE", "");
				val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
				val.CloseButtonHandle((Predicate<object>)null, (object)null);
				((Window)val).Owner = (Window)(object)ParentWindow;
				((Window)val).ShowDialog();
			}
			else
			{
				CustomMessageWindow val2 = new CustomMessageWindow
				{
					ImageName = "disk_cleanup_popup_window"
				};
				val2.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP", "");
				val2.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MESSAGE", "");
				((UIElement)val2.BodyTextBlockTitle).Visibility = (Visibility)0;
				val2.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
				val2.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CONTINUE_CONFIRMATION", "");
				val2.AddButton((ButtonColors)2, "STRING_CLOSE", (EventHandler)null, (string)null, false, (object)null);
				val2.AddButton((ButtonColors)4, "STRING_CONTINUE", (EventHandler)delegate
				{
					LaunchDiskCompaction(sender, null);
				}, (string)null, false, (object)null);
				val2.CloseButtonHandle((Predicate<object>)null, (object)null);
				((Window)val2).Owner = (Window)(object)ParentWindow;
				((Window)val2).ShowDialog();
			}
		}, new object[0]);
	}

	private void LaunchDataManager(string argument)
	{
		foreach (MainWindow item in BlueStacksUIUtils.DictWindows.Values.ToList())
		{
			MainWindow.sIsClosingForBackupRestore = true;
			if (argument == "backup")
			{
				item.CloseAllWindowAndPerform(Closing_WindowHandlerForBackup);
			}
			else if (argument == "restore")
			{
				item.CloseAllWindowAndPerform(Closing_WindowHandlerForRestore);
			}
		}
	}

	private void LaunchDiskCompaction(object sender, MouseButtonEventArgs e)
	{
		try
		{
			ParentWindow.mFrontendHandler.IsRestartFrontendWhenClosed = false;
			BlueStacksUIUtils.HideUnhideBlueStacks(isHide: true);
			using Process process = new Process();
			process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe");
			process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-vmname:{0} -relaunch", new object[1] { ParentWindow.mVmName });
			process.Start();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in starting disk compaction" + ex.ToString());
		}
	}

	internal void Closing_WindowHandlerForBackup(object sender, EventArgs e)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "relaunch", "true" },
				{ "sendResponseImmediately", "true" }
			};
			HTTPUtils.SendRequestToAgent("backup", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64", true);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in closing window handler for backup" + ex.ToString());
		}
	}

	internal void Closing_WindowHandlerForRestore(object sender, EventArgs e)
	{
		try
		{
			Utils.KillCurrentOemProcessByName("HD-MultiInstanceManager", (string)null);
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "relaunch", "true" },
				{ "sendResponseImmediately", "true" }
			};
			HTTPUtils.SendRequestToAgent("restore", dictionary, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64", true);
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in closing window handler for restore" + ex.ToString());
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/backuprestoresettingscontrol.xaml", UriKind.Relative);
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
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
		switch (connectionId)
		{
		case 1:
			mDiskCleanupGrid = (Grid)target;
			break;
		case 2:
			mDiskCleanupBtn = (CustomButton)target;
			((ButtonBase)mDiskCleanupBtn).Click += new RoutedEventHandler(DiskCleanupBtn_Click);
			break;
		case 3:
			mLineSeperator = (Line)target;
			break;
		case 4:
			mBackupRestoreGrid = (Grid)target;
			break;
		case 5:
			mRestoreBtn = (CustomButton)target;
			((ButtonBase)mRestoreBtn).Click += new RoutedEventHandler(RestoreBtn_Click);
			break;
		case 6:
			mBackupBtn = (CustomButton)target;
			((ButtonBase)mBackupBtn).Click += new RoutedEventHandler(BackupBtn_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
