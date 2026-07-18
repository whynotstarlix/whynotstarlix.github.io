using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class MacroAddedDragControl : UserControl, IComponentConnector, IStyleConnector
{
	private MergeMacroWindow mMergeMacroWindow;

	private Point _dragStartPoint;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mNoMergeMacroGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ListBox mListBox;

	private bool _contentLoaded;

	public MergeMacroWindow MergeMacroWindow
	{
		get
		{
			if (mMergeMacroWindow == null)
			{
				mMergeMacroWindow = Window.GetWindow((DependencyObject)(object)this) as MergeMacroWindow;
			}
			return mMergeMacroWindow;
		}
	}

	public MacroAddedDragControl()
	{
		InitializeComponent();
	}

	internal void Init()
	{
		((FrameworkElement)mListBox).DataContext = MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
		MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= Items_CollectionChanged;
		MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += Items_CollectionChanged;
		Items_CollectionChanged(null, null);
	}

	private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Count > 0)
		{
			((UIElement)mNoMergeMacroGrid).Visibility = (Visibility)2;
			((UIElement)mListBox).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mNoMergeMacroGrid).Visibility = (Visibility)0;
			((UIElement)mListBox).Visibility = (Visibility)2;
		}
	}

	private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Point position = e.GetPosition((IInputElement)null);
		Vector val = _dragStartPoint - position;
		if ((int)e.LeftButton == 1 && (Math.Abs(((Vector)(ref val)).X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(((Vector)(ref val)).Y) > SystemParameters.MinimumVerticalDragDistance))
		{
			ListBoxItem val2 = WpfUtils.FindVisualParent<ListBoxItem>((DependencyObject)((RoutedEventArgs)e).OriginalSource);
			if (val2 != null)
			{
				DragDrop.DoDragDrop((DependencyObject)(object)val2, ((FrameworkElement)val2).DataContext, (DragDropEffects)2);
			}
		}
	}

	private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_dragStartPoint = ((MouseEventArgs)e).GetPosition((IInputElement)null);
	}

	private void ListBoxItem_Drop(object sender, DragEventArgs e)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		UnsetMarginDuringDrag();
		if (sender is ListBoxItem)
		{
			object data = e.Data.GetData(typeof(MergedMacroConfiguration));
			MergedMacroConfiguration val = (MergedMacroConfiguration)((data is MergedMacroConfiguration) ? data : null);
			object dataContext = ((FrameworkElement)(ListBoxItem)sender).DataContext;
			MergedMacroConfiguration val2 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
			int sourceIndex = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val);
			int targetIndex = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val2);
			Move(val, sourceIndex, targetIndex);
		}
	}

	private void ListBoxItem_DragOver(object sender, DragEventArgs e)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (sender is ListBoxItem)
		{
			object data = e.Data.GetData(typeof(MergedMacroConfiguration));
			MergedMacroConfiguration val = (MergedMacroConfiguration)((data is MergedMacroConfiguration) ? data : null);
			ListBoxItem val2 = (ListBoxItem)sender;
			object dataContext = ((FrameworkElement)(ListBoxItem)sender).DataContext;
			MergedMacroConfiguration val3 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
			int num = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val);
			int num2 = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val3);
			if (num2 < num)
			{
				object obj = ((FrameworkTemplate)((Control)val2).Template).FindName("mMainGrid", (FrameworkElement)(object)val2);
				((FrameworkElement)((obj is Grid) ? obj : null)).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			}
			else if (num2 > num)
			{
				object obj2 = ((FrameworkTemplate)((Control)val2).Template).FindName("mMainGrid", (FrameworkElement)(object)val2);
				((FrameworkElement)((obj2 is Grid) ? obj2 : null)).Margin = new Thickness(0.0, -1.0, 0.0, 10.0);
			}
			else
			{
				object obj3 = ((FrameworkTemplate)((Control)val2).Template).FindName("mMainGrid", (FrameworkElement)(object)val2);
				((FrameworkElement)((obj3 is Grid) ? obj3 : null)).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
			}
			UnsetMarginDuringDrag(val2);
		}
	}

	private void UnsetMarginDuringDrag(ListBoxItem neglectItem = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		foreach (object item in (IEnumerable)((ItemsControl)mListBox).Items)
		{
			DependencyObject obj = ((ItemsControl)mListBox).ItemContainerGenerator.ContainerFromItem(item);
			ListBoxItem val = (ListBoxItem)(object)((obj is ListBoxItem) ? obj : null);
			if (neglectItem == null || val != neglectItem)
			{
				object obj2 = ((FrameworkTemplate)((Control)val).Template).FindName("mMainGrid", (FrameworkElement)(object)val);
				((FrameworkElement)((obj2 is Grid) ? obj2 : null)).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
			}
		}
	}

	private void Move(MergedMacroConfiguration source, int sourceIndex, int targetIndex)
	{
		if (sourceIndex < targetIndex)
		{
			if (((FrameworkElement)mListBox).DataContext is ObservableCollection<MergedMacroConfiguration> observableCollection)
			{
				observableCollection.Insert(targetIndex + 1, source);
				observableCollection.RemoveAt(sourceIndex);
			}
		}
		else if (((FrameworkElement)mListBox).DataContext is ObservableCollection<MergedMacroConfiguration> observableCollection2)
		{
			int num = sourceIndex + 1;
			if (observableCollection2.Count + 1 > num)
			{
				observableCollection2.Insert(targetIndex, source);
				observableCollection2.RemoveAt(num);
			}
		}
	}

	private void ListBox_DragOver(object sender, DragEventArgs e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ListBox val = (ListBox)((sender is ListBox) ? sender : null);
		ScrollViewer val2 = WpfUtils.FindVisualChild<ScrollViewer>((DependencyObject)(object)val);
		double num = 15.0;
		Point position = e.GetPosition((IInputElement)(object)val);
		double y = ((Point)(ref position)).Y;
		double num2 = 10.0;
		if (y < num)
		{
			val2.ScrollToVerticalOffset(val2.VerticalOffset - num2);
		}
		else if (y > ((FrameworkElement)val).ActualHeight - num)
		{
			val2.ScrollToVerticalOffset(val2.VerticalOffset + num2);
		}
	}

	private void Group_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val != null)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_group", null, null);
			object dataContext = ((FrameworkElement)val).DataContext;
			MergedMacroConfiguration val2 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
			int num = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val2);
			Merge(num, num - 1);
		}
	}

	private void Merge(int sourceIndex, int targetIndex)
	{
		if (!(((FrameworkElement)mListBox).DataContext is ObservableCollection<MergedMacroConfiguration> observableCollection))
		{
			return;
		}
		foreach (string item in observableCollection[sourceIndex].MacrosToRun)
		{
			observableCollection[targetIndex].MacrosToRun.Add(item);
		}
		observableCollection.RemoveAt(sourceIndex);
		SetDefaultPropertiesForMergedMacroConfig(observableCollection[targetIndex]);
	}

	private void UnGroup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val != null)
		{
			object dataContext = ((FrameworkElement)val).DataContext;
			MergedMacroConfiguration val2 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
			int sourceIndex = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val2);
			UnMerge(val2, sourceIndex);
		}
	}

	private static void SetDefaultPropertiesForMergedMacroConfig(MergedMacroConfiguration config)
	{
		config.LoopCount = 1;
		config.LoopInterval = 0;
		config.Acceleration = 1.0;
		config.DelayNextScript = 0;
	}

	private void UnMerge(MergedMacroConfiguration source, int sourceIndex)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		if (((FrameworkElement)mListBox).DataContext is ObservableCollection<MergedMacroConfiguration> observableCollection)
		{
			SetDefaultPropertiesForMergedMacroConfig(source);
			for (int i = 0; i < source.MacrosToRun.Count; i++)
			{
				string item = source.MacrosToRun[i];
				MergedMacroConfiguration val = new MergedMacroConfiguration
				{
					Tag = MergeMacroWindow.mAddedMacroTag++
				};
				val.MacrosToRun.Add(item);
				observableCollection.Insert(sourceIndex + i + 1, val);
			}
			observableCollection.RemoveAt(sourceIndex);
		}
	}

	private void Remove_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val != null)
		{
			object dataContext = ((FrameworkElement)val).DataContext;
			MergedMacroConfiguration val2 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
			int index = ((CollectionView)((ItemsControl)mListBox).Items).IndexOf((object)val2);
			(((FrameworkElement)mListBox).DataContext as ObservableCollection<MergedMacroConfiguration>).RemoveAt(index);
			MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Remove(val2);
		}
	}

	private void Settings_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CustomPictureBox val = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		if (val == null)
		{
			return;
		}
		ListBoxItem val2 = WpfUtils.FindVisualParent<ListBoxItem>((DependencyObject)(object)val);
		object dataContext = ((FrameworkElement)val2).DataContext;
		MergedMacroConfiguration val3 = (MergedMacroConfiguration)((dataContext is MergedMacroConfiguration) ? dataContext : null);
		val3.IsSettingsVisible = !val3.IsSettingsVisible;
		object obj = ((FrameworkTemplate)((Control)val2).Template).FindName("mMacroSettingsImage", (FrameworkElement)(object)val2);
		((CustomPictureBox)((obj is CustomPictureBox) ? obj : null)).ImageName = (val3.IsSettingsVisible ? "outline_settings_collapse" : "outline_settings_expand");
		ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, val3.IsSettingsVisible ? "merge_dropdown_expand" : "merge_dropdown_collapse", null, null);
		foreach (object item in (IEnumerable)((ItemsControl)mListBox).Items)
		{
			DependencyObject obj2 = ((ItemsControl)mListBox).ItemContainerGenerator.ContainerFromItem(item);
			ListBoxItem val4 = (ListBoxItem)(object)((obj2 is ListBoxItem) ? obj2 : null);
			if (val4 != val2)
			{
				object dataContext2 = ((FrameworkElement)val4).DataContext;
				((MergedMacroConfiguration)((dataContext2 is MergedMacroConfiguration) ? dataContext2 : null)).IsSettingsVisible = false;
				object obj3 = ((FrameworkTemplate)((Control)val4).Template).FindName("mMacroSettingsImage", (FrameworkElement)(object)val4);
				((CustomPictureBox)((obj3 is CustomPictureBox) ? obj3 : null)).ImageName = "outline_settings_expand";
			}
		}
	}

	private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		((RoutedEventArgs)e).Handled = !IsTextAllowed(e.Text);
	}

	private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
	{
		if (e.DataObject.GetDataPresent(typeof(string)))
		{
			string text = (string)e.DataObject.GetData(typeof(string));
			if (!IsTextAllowed(text))
			{
				((DataObjectEventArgs)e).CancelCommand();
			}
		}
		else
		{
			((DataObjectEventArgs)e).CancelCommand();
		}
	}

	private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Key == 18)
		{
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private bool IsTextAllowed(string text)
	{
		if (new Regex("^[0-9]+$").IsMatch(text))
		{
			return text.IndexOf(' ') == -1;
		}
		return false;
	}

	private void MacroAddDragControl_Loaded(object sender, RoutedEventArgs e)
	{
		((FrameworkElement)mListBox).DataContext = MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
	}

	private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		CustomTextBox val = (CustomTextBox)((sender is CustomTextBox) ? sender : null);
		((XTextBox)val).InputTextValidity = (TextValidityOptions)((!string.IsNullOrEmpty(((TextBox)val).Text) && !(((TextBox)val).Text == "0")) ? 1 : (-1));
	}

	private void MacroName_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		UsefulExtensionMethod.SetTextblockTooltip((TextBlock)((sender is TextBlock) ? sender : null));
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macroaddeddragcontrol.xaml", UriKind.Relative);
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(MacroAddedDragControl)target).Loaded += new RoutedEventHandler(MacroAddDragControl_Loaded);
			break;
		case 12:
			mNoMergeMacroGrid = (Border)target;
			break;
		case 13:
			mListBox = (ListBox)target;
			((UIElement)mListBox).DragOver += new DragEventHandler(ListBox_DragOver);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		switch (connectionId)
		{
		case 2:
			((FrameworkElement)(TextBlock)target).SizeChanged += new SizeChangedEventHandler(MacroName_SizeChanged);
			break;
		case 3:
		{
			EventSetter val = new EventSetter();
			val.Event = UIElement.DragOverEvent;
			val.Handler = (Delegate)new DragEventHandler(ListBoxItem_DragOver);
			((Collection<SetterBase>)(object)((Style)target).Setters).Add((SetterBase)(object)val);
			val = new EventSetter();
			val.Event = UIElement.DropEvent;
			val.Handler = (Delegate)new DragEventHandler(ListBoxItem_Drop);
			((Collection<SetterBase>)(object)((Style)target).Setters).Add((SetterBase)(object)val);
			break;
		}
		case 4:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ListBoxItem_PreviewMouseLeftButtonDown);
			((UIElement)(CustomPictureBox)target).PreviewMouseMove += new MouseEventHandler(ListBox_PreviewMouseMove);
			break;
		case 5:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(UnGroup_PreviewMouseLeftButtonUp);
			break;
		case 6:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Settings_PreviewMouseLeftButtonUp);
			break;
		case 7:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Remove_PreviewMouseLeftButtonUp);
			break;
		case 8:
			((UIElement)(CustomTextBox)target).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)(CustomTextBox)target).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)(CustomTextBox)target).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			((TextBoxBase)(CustomTextBox)target).TextChanged += new TextChangedEventHandler(LoopCountTextBox_TextChanged);
			break;
		case 9:
			((UIElement)(CustomTextBox)target).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)(CustomTextBox)target).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)(CustomTextBox)target).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 10:
			((UIElement)(CustomTextBox)target).PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
			((UIElement)(CustomTextBox)target).AddHandler(DataObject.PastingEvent, (Delegate)new DataObjectPastingEventHandler(NumericTextBox_Pasting));
			((UIElement)(CustomTextBox)target).PreviewKeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
			break;
		case 11:
			((UIElement)(CustomPictureBox)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Group_PreviewMouseLeftButtonUp);
			break;
		}
	}
}
