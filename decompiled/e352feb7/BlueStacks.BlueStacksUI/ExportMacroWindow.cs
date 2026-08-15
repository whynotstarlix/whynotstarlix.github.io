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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class ExportMacroWindow : CustomWindow, IComponentConnector
{
	private MacroRecorderWindow mOperationWindow;

	private MainWindow ParentWindow;

	internal StackPanel mScriptsStackPanel;

	internal int mNumberOfFilesSelectedForExport;

	private Dictionary<string, MacroRecording> mNameRecordingDict = new Dictionary<string, MacroRecording>();

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScriptsListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSelectAllBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mExportBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	private bool _contentLoaded;

	public ExportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
	{
		InitializeComponent();
		mOperationWindow = window;
		ParentWindow = mainWindow;
		object content = ((ContentControl)mScriptsListScrollbar).Content;
		mScriptsStackPanel = (StackPanel)((content is StackPanel) ? content : null);
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		((Window)this).Close();
		mOperationWindow.mExportMacroWindow = null;
		((UIElement)mOperationWindow.mOverlayGrid).Visibility = (Visibility)1;
		((UIElement)mOperationWindow).Focus();
	}

	internal void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (MacroRecording vertex in MacroGraph.Instance.Vertices)
			{
				MacroRecording val = vertex;
				ParentWindow.mIsScriptsPresent = true;
				if (!mNameRecordingDict.ContainsKey(val.Name.ToLower(CultureInfo.InvariantCulture)))
				{
					mNameRecordingDict.Add(val.Name.ToLower(CultureInfo.InvariantCulture), val);
					CustomCheckbox val2 = new CustomCheckbox
					{
						Content = val.Name,
						TextFontSize = 12.0,
						Margin = new Thickness(0.0, 6.0, 0.0, 6.0)
					};
					((ToggleButton)val2).Checked += new RoutedEventHandler(Box_Checked);
					((ToggleButton)val2).Unchecked += new RoutedEventHandler(Box_Unchecked);
					val2.ImageMargin = new Thickness(2.0);
					((FrameworkElement)val2).MaxHeight = 20.0;
					((Panel)mScriptsStackPanel).Children.Add((UIElement)(object)val2);
				}
			}
			mNumberOfFilesSelectedForExport = 0;
		}
		catch (Exception ex)
		{
			Logger.Error("Error in export window init err: " + ex.ToString());
		}
	}

	private void Box_Unchecked(object sender, RoutedEventArgs e)
	{
		mNumberOfFilesSelectedForExport--;
		if (mNumberOfFilesSelectedForExport == 0)
		{
			((UIElement)mExportBtn).IsEnabled = false;
		}
		if (mNumberOfFilesSelectedForExport == ((Panel)mScriptsStackPanel).Children.Count - 1)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = false;
		}
	}

	private void Box_Checked(object sender, RoutedEventArgs e)
	{
		mNumberOfFilesSelectedForExport++;
		if (mNumberOfFilesSelectedForExport == 1)
		{
			((UIElement)mExportBtn).IsEnabled = true;
		}
		if (mNumberOfFilesSelectedForExport == ((Panel)mScriptsStackPanel).Children.Count)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = true;
		}
	}

	private void ExportBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Invalid comparison between Unknown and I4
		try
		{
			int num = 0;
			List<MacroRecording> list = new List<MacroRecording>();
			foreach (object child in ((Panel)mScriptsStackPanel).Children)
			{
				if (((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked == true)
				{
					list.Add(mNameRecordingDict.ElementAt(num).Value);
				}
				num++;
			}
			if (list.Count != 0)
			{
				FolderBrowserDialog val = new FolderBrowserDialog();
				try
				{
					if ((int)((CommonDialog)val).ShowDialog() == 1)
					{
						using (BackgroundWorker backgroundWorker = new BackgroundWorker())
						{
							backgroundWorker.DoWork += BgExport_DoWork;
							backgroundWorker.RunWorkerCompleted += BgExport_RunWorkerCompleted;
							ShowLoadingGrid(isShow: true);
							backgroundWorker.RunWorkerAsync(new List<object> { val.SelectedPath, list });
							return;
						}
					}
					ToggleCheckBoxForExport();
					return;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_SELECTED", ""), 4.0, isShowCloseImage: true);
		}
		catch (Exception)
		{
			Logger.Error("Error while exporting script. err:" + ((object)e).ToString());
		}
	}

	private void BgExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		ShowLoadingGrid(isShow: false);
		ToggleCheckBoxForExport();
		CloseWindow();
	}

	private void ToggleCheckBoxForExport()
	{
		foreach (object child in ((Panel)mScriptsStackPanel).Children)
		{
			((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked = false;
		}
	}

	private void BgExport_DoWork(object sender, DoWorkEventArgs e)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		List<object> obj = e.Argument as List<object>;
		string path = obj[0] as string;
		foreach (MacroRecording item in obj[1] as List<MacroRecording>)
		{
			string name = item.Name;
			string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
			string text2 = Path.Combine(path, item.Name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
			if ((int)item.RecordingType == 0)
			{
				File.Copy(text, text2, overwrite: true);
				continue;
			}
			try
			{
				Logger.Info("Saving multi-macro");
				List<string> list = new List<string>();
				foreach (MacroRecording allChild in MacroGraph.Instance.GetAllChilds((BiDirectionalVertex<MacroRecording>)(object)item))
				{
					MacroRecording val = allChild;
					list.Add(File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, val.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
				}
				MacroRecording obj2 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text), Utils.GetSerializerSettings());
				obj2.SourceRecordings = list;
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)1;
				string contents = JsonConvert.SerializeObject((object)obj2, serializerSettings);
				File.WriteAllText(text2, contents);
			}
			catch (Exception ex)
			{
				Logger.Error("Coulnd't take backup of script {0}, Ex: {1}", new object[2] { name, ex });
			}
		}
	}

	private void ShowLoadingGrid(bool isShow)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (isShow)
			{
				((UIElement)mLoadingGrid).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mLoadingGrid).Visibility = (Visibility)1;
			}
		}, new object[0]);
	}

	private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mSelectAllBtn).IsChecked.Value)
		{
			foreach (object child in ((Panel)mScriptsStackPanel).Children)
			{
				((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked = true;
			}
			return;
		}
		foreach (object child2 in ((Panel)mScriptsStackPanel).Children)
		{
			((ToggleButton)((child2 is CustomCheckbox) ? child2 : null)).IsChecked = false;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/exportmacrowindow.xaml", UriKind.Relative);
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 3:
			mScriptsListScrollbar = (ScrollViewer)target;
			break;
		case 4:
			mSelectAllBtn = (CustomCheckbox)target;
			((ButtonBase)mSelectAllBtn).Click += new RoutedEventHandler(SelectAllBtn_Click);
			break;
		case 5:
			mExportBtn = (CustomButton)target;
			((ButtonBase)mExportBtn).Click += new RoutedEventHandler(ExportBtn_Click);
			break;
		case 6:
			mLoadingGrid = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
