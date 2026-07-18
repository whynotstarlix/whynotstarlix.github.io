using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class MacroBookmarksPopup : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal MacroBookmarksPopup mMacroBookmarksPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMainStackPanel;

	private bool _contentLoaded;

	private MainWindow ParentWindow { get; set; }

	public MacroBookmarksPopup()
	{
		InitializeComponent();
		InitList();
	}

	public void SetParentWindowAndBindEvents(MainWindow window)
	{
		ParentWindow = window;
		if (ParentWindow != null)
		{
			ParentWindow.mCommonHandler.MacroBookmarkChangedEvent += ParentWindow_MacroBookmarkChanged;
			ParentWindow.mCommonHandler.MacroSettingChangedEvent += ParentWindow_MacroSettingChangedEvent;
			ParentWindow.mCommonHandler.MacroDeletedEvent += ParentWindow_MacroDeletedEvent;
		}
	}

	private void ParentWindow_MacroDeletedEvent(string fileName)
	{
		Grid gridByTag = GetGridByTag(fileName);
		if (gridByTag != null)
		{
			((Panel)mMainStackPanel).Children.Remove((UIElement)(object)gridByTag);
		}
	}

	private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
	{
		try
		{
			((Panel)mMainStackPanel).Children.Clear();
			InitList();
		}
		catch (Exception ex)
		{
			Logger.Error("Couldn't update name: {0}", new object[1] { ex.Message });
		}
	}

	private void ParentWindow_MacroBookmarkChanged(string fileName, bool wasBookmarked)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (wasBookmarked)
		{
			((Panel)mMainStackPanel).Children.Add((UIElement)(object)CreateGrid(fileName));
			return;
		}
		foreach (Grid child in ((Panel)mMainStackPanel).Children)
		{
			Grid val = child;
			if ((string)((FrameworkElement)val).Tag == fileName)
			{
				((Panel)mMainStackPanel).Children.Remove((UIElement)(object)val);
				break;
			}
		}
	}

	private Grid GetGridByTag(string tag)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		foreach (Grid child in ((Panel)mMainStackPanel).Children)
		{
			Grid val = child;
			if ((string)((FrameworkElement)val).Tag == tag)
			{
				return val;
			}
		}
		return null;
	}

	private void InitList()
	{
		string[] bookmarkedScriptList = RegistryManager.Instance.BookmarkedScriptList;
		foreach (string text in bookmarkedScriptList)
		{
			if (!string.IsNullOrEmpty(text))
			{
				Grid val = CreateGrid(text);
				((Panel)mMainStackPanel).Children.Add((UIElement)(object)val);
			}
		}
	}

	private void MMacroBookmarksPopup_Loaded(object sender, RoutedEventArgs e)
	{
	}

	private Grid CreateGrid(string fileName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		Grid val = new Grid();
		((UIElement)val).MouseEnter += new MouseEventHandler(GridElement_MouseEnter);
		((UIElement)val).MouseLeave += new MouseEventHandler(GridElement_MouseLeave);
		((UIElement)val).MouseLeftButtonUp += new MouseButtonEventHandler(GridElement_MouseLeftButtonUp);
		((Panel)val).Background = (Brush)(object)Brushes.Transparent;
		((FrameworkElement)val).Tag = fileName;
		TextBlock val2 = new TextBlock
		{
			FontSize = 12.0,
			TextTrimming = (TextTrimming)1,
			Margin = new Thickness(10.0, 5.0, 10.0, 5.0)
		};
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, TextBlock.ForegroundProperty, "GuidanceTextColorForeground");
		string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, fileName);
		if (File.Exists(path))
		{
			try
			{
				MacroRecording val3 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(path), Utils.GetSerializerSettings());
				val2.Text = val3.Name;
				((FrameworkElement)val2).ToolTip = val3.Name;
			}
			catch
			{
			}
		}
		else
		{
			val2.Text = fileName;
			((FrameworkElement)val2).ToolTip = fileName;
		}
		((Panel)val).Children.Add((UIElement)(object)val2);
		return val;
	}

	private void GridElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.mIsMacroRecorderActive)
		{
			return;
		}
		string macroFileName = (string)((FrameworkElement)((sender is Grid) ? sender : null)).Tag;
		MacroRecording val = (from MacroRecording macro in MacroGraph.Instance.Vertices
			where string.Equals(macro.Name, macroFileName, StringComparison.InvariantCultureIgnoreCase)
			select macro).FirstOrDefault();
		if (val == null)
		{
			((Panel)mMainStackPanel).Children.Remove((UIElement)((sender is Grid) ? sender : null));
			return;
		}
		try
		{
			if (!ParentWindow.mIsMacroPlaying)
			{
				ParentWindow.mCommonHandler.FullMacroScriptPlayHandler(val);
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "bookmark", ((object)val.RecordingType/*cast due to constrained. prefix*/).ToString(), string.IsNullOrEmpty(val.MacroId) ? "local" : "community");
			}
			else
			{
				CustomMessageWindow val2 = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(val2.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
				BlueStacksUIBinding.Bind(val2.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
				val2.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
				((Window)val2).Owner = (Window)(object)ParentWindow;
				((Window)val2).ShowDialog();
			}
			if (ParentWindow.mSidebar != null)
			{
				((Popup)ParentWindow.mSidebar.mMacroButtonPopup).IsOpen = false;
				ParentWindow.mSidebar.ToggleSidebarVisibilityInFullscreen(isVisible: false);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString());
		}
	}

	private void GridElement_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Grid) ? sender : null), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
	}

	private void GridElement_MouseLeave(object sender, MouseEventArgs e)
	{
		((Panel)((sender is Grid) ? sender : null)).Background = (Brush)(object)Brushes.Transparent;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrobookmarkspopup.xaml", UriKind.Relative);
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
		switch (connectionId)
		{
		case 1:
			mMacroBookmarksPopup = (MacroBookmarksPopup)target;
			((FrameworkElement)mMacroBookmarksPopup).Loaded += new RoutedEventHandler(MMacroBookmarksPopup_Loaded);
			break;
		case 2:
			mGrid = (Grid)target;
			break;
		case 3:
			mMainStackPanel = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
