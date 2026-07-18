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

public class ExportSchemesWindow : CustomWindow, IComponentConnector
{
	private KeymapCanvasWindow CanvasWindow;

	private MainWindow ParentWindow;

	internal StackPanel mSchemesStackPanel;

	internal int mNumberOfSchemesSelectedForExport;

	private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mSchemesListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSelectAllBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mExportBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	private bool _contentLoaded;

	public ExportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
	{
		InitializeComponent();
		CanvasWindow = window;
		ParentWindow = mainWindow;
		object content = ((ContentControl)mSchemesListScrollbar).Content;
		mSchemesStackPanel = (StackPanel)((content is StackPanel) ? content : null);
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		((Window)this).Close();
		CanvasWindow.SidebarWindow.mExportSchemesWindow = null;
		((UIElement)CanvasWindow.SidebarWindow.mOverlayGrid).Visibility = (Visibility)1;
		((UIElement)CanvasWindow.SidebarWindow).Focus();
	}

	internal void Init()
	{
		try
		{
			mNumberOfSchemesSelectedForExport = 0;
			ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn).ToList().ForEach(delegate(IMControlScheme scheme)
			{
				AddSchemeToExportCheckbox(scheme);
			});
			ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn).ToList().ForEach(delegate(IMControlScheme scheme)
			{
				if (Enumerable.Contains(dict.Keys, scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
				{
					scheme.Name += " (Edited)";
					scheme.Name = KMManager.GetUniqueName(scheme.Name, ParentWindow.OriginalLoadedConfig.ControlSchemesDict.Keys);
				}
				AddSchemeToExportCheckbox(scheme);
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Error in export window init err: " + ex.ToString());
		}
		void AddSchemeToExportCheckbox(IMControlScheme scheme)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			dict.Add(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim(), scheme);
			CustomCheckbox val = new CustomCheckbox
			{
				Content = scheme.Name,
				TextFontSize = 14.0,
				ImageMargin = new Thickness(2.0),
				Margin = new Thickness(0.0, 1.0, 0.0, 1.0),
				MaxHeight = 20.0
			};
			((ToggleButton)val).Checked += new RoutedEventHandler(Box_Checked);
			((ToggleButton)val).Unchecked += new RoutedEventHandler(Box_Unchecked);
			((Panel)mSchemesStackPanel).Children.Add((UIElement)(object)val);
		}
	}

	private void Box_Unchecked(object sender, RoutedEventArgs e)
	{
		mNumberOfSchemesSelectedForExport--;
		if (mNumberOfSchemesSelectedForExport == ((Panel)mSchemesStackPanel).Children.Count - 1)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = false;
		}
		if (mNumberOfSchemesSelectedForExport == 0)
		{
			((UIElement)mExportBtn).IsEnabled = false;
		}
	}

	private void Box_Checked(object sender, RoutedEventArgs e)
	{
		mNumberOfSchemesSelectedForExport++;
		if (mNumberOfSchemesSelectedForExport == ((Panel)mSchemesStackPanel).Children.Count)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = true;
		}
		if (mNumberOfSchemesSelectedForExport == 1)
		{
			((UIElement)mExportBtn).IsEnabled = true;
		}
	}

	private void ExportBtn_Click(object sender, RoutedEventArgs e)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		try
		{
			int num = 0;
			List<IMControlScheme> list = new List<IMControlScheme>();
			foreach (object child in ((Panel)mSchemesStackPanel).Children)
			{
				if (((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked == true)
				{
					list.Add(dict.ElementAt(num).Value);
				}
				num++;
			}
			if (list.Count != 0)
			{
				SaveFileDialog val = new SaveFileDialog
				{
					AddExtension = true,
					DefaultExt = ".cfg",
					Filter = "Cfg files(*.cfg) | *.cfg",
					FileName = ParentWindow.StaticComponents.mSelectedTabButton.AppName
				};
				try
				{
					if ((int)((CommonDialog)val).ShowDialog() == 1)
					{
						using (BackgroundWorker backgroundWorker = new BackgroundWorker())
						{
							backgroundWorker.DoWork += BgExport_DoWork;
							backgroundWorker.RunWorkerCompleted += BgExport_RunWorkerCompleted;
							ShowLoadingGrid(isShow: true);
							backgroundWorker.RunWorkerAsync(new List<object>
							{
								((FileDialog)val).FileName,
								list
							});
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
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""));
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
		foreach (object child in ((Panel)mSchemesStackPanel).Children)
		{
			((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked = false;
		}
	}

	private void BgExport_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			List<object> obj = e.Argument as List<object>;
			string path = obj[0] as string;
			List<IMControlScheme> controlSchemes = obj[1] as List<IMControlScheme>;
			IMConfig obj2 = new IMConfig
			{
				Strings = UsefulExtensionMethod.DeepCopy<Dictionary<string, Dictionary<string, string>>>(ParentWindow.OriginalLoadedConfig.Strings),
				ControlSchemes = controlSchemes
			};
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = (Formatting)1;
			string contents = JsonConvert.SerializeObject((object)obj2, serializerSettings);
			File.WriteAllText(path, contents);
			ParentWindow.mCommonHandler.AddToastPopup((Window)(object)CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_EXPORTED", ""));
		}
		catch (Exception)
		{
			Logger.Error("Error in creating exported file " + e.ToString());
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
			foreach (object child in ((Panel)mSchemesStackPanel).Children)
			{
				((ToggleButton)((child is CustomCheckbox) ? child : null)).IsChecked = true;
			}
			return;
		}
		foreach (object child2 in ((Panel)mSchemesStackPanel).Children)
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
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/exportschemeswindow.xaml", UriKind.Relative);
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
			mSchemesListScrollbar = (ScrollViewer)target;
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
