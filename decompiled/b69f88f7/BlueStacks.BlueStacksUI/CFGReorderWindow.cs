using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class CFGReorderWindow : CustomWindow, IComponentConnector
{
	private class TreeItemDrop
	{
		public TreeViewItem Source { get; set; }

		public TreeViewItem Target { get; set; }

		public bool IsSourceCategory { get; set; }

		public bool IsTargetCategory { get; set; }

		public TreeViewItem SourceParent { get; set; }

		public TreeViewItem TargetParent { get; set; }

		public int SourceIndex { get; set; } = -1;

		public int TargetIndex { get; set; } = -1;

		public bool AreSourceAndTargetCategories { get; set; }

		public TreeItemDrop(TreeViewItem sourceItem, TreeViewItem targetItem, TreeView currentTree)
		{
			Source = sourceItem;
			Target = targetItem;
			ItemsControl selectedTreeViewItemParent = GetSelectedTreeViewItemParent(sourceItem);
			SourceParent = (TreeViewItem)(object)((selectedTreeViewItemParent is TreeViewItem) ? selectedTreeViewItemParent : null);
			if (SourceParent != null)
			{
				SourceIndex = ((CollectionView)((ItemsControl)SourceParent).Items).IndexOf((object)Source);
			}
			else
			{
				IsSourceCategory = true;
				SourceIndex = ((CollectionView)((ItemsControl)currentTree).Items).IndexOf((object)Source);
			}
			ItemsControl selectedTreeViewItemParent2 = GetSelectedTreeViewItemParent(targetItem);
			TargetParent = (TreeViewItem)(object)((selectedTreeViewItemParent2 is TreeViewItem) ? selectedTreeViewItemParent2 : null);
			if (TargetParent != null)
			{
				TargetIndex = ((CollectionView)((ItemsControl)TargetParent).Items).IndexOf((object)Target);
			}
			else
			{
				IsTargetCategory = true;
				TargetIndex = ((CollectionView)((ItemsControl)currentTree).Items).IndexOf((object)Target);
			}
			AreSourceAndTargetCategories = SourceParent == null;
		}
	}

	private IMConfig mCurrentlySelectedCFG;

	private IMControlScheme mCurrentlySelectedScheme;

	private Dictionary<string, IMConfig> mLoadedCFGDict = new Dictionary<string, IMConfig>();

	private IList<IMControlScheme> mSchemesList;

	private const string NO_GUIDANCE = "NO_GUIDANCE";

	private const string CFG_MODIFIED_SUFFIX = "* (modified)";

	private Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>> mSchemeTreeMapping = new Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>>();

	private static CFGReorderWindow mInstance;

	private Point _dragStartPoint;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ListView mLoadedCFGsListView;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mCurrentlyLoadedStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mCurrentlyLoadedCFGTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ListView mSchemesListView;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TreeView mIMActionsTreeView;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mActionJsonGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox mActionTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button mEditButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button mSaveButton;

	private bool _contentLoaded;

	public static CFGReorderWindow Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new CFGReorderWindow();
			}
			return mInstance;
		}
	}

	public CFGReorderWindow()
	{
		InitializeComponent();
		((Window)this).Closing += CFGReorderWindow_Closing;
		((Window)this).Owner = (Window)(object)BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName];
		((Window)this).WindowStartupLocation = (WindowStartupLocation)1;
	}

	private void ClearState()
	{
		mCurrentlySelectedCFG = null;
		mLoadedCFGDict.Clear();
		((ItemsControl)mLoadedCFGsListView).Items.Clear();
		mSchemeTreeMapping.Clear();
		ClearIMLists();
		((UIElement)mLoadedCFGsListView).Visibility = (Visibility)2;
		((UIElement)mSchemesListView).Visibility = (Visibility)2;
		((UIElement)mIMActionsTreeView).Visibility = (Visibility)2;
	}

	private void ClearIMLists()
	{
		mSchemesList = new ObservableCollection<IMControlScheme>();
		ClearIMActionsTree();
	}

	private void ClearIMActionsTree()
	{
		((ItemsControl)mIMActionsTreeView).Items.Clear();
	}

	private void CFGReorderWindow_Closing(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
		((Window)this).Hide();
		ClearState();
	}

	private void LoadCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		ClearState();
		List<string> list = new List<string>();
		string text = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
		if (!Directory.Exists(text) || Directory.GetFileSystemEntries(text).Length == 0)
		{
			text = RegistryStrings.InputMapperFolder;
		}
		OpenFileDialog val = new OpenFileDialog
		{
			Filter = "BlueStacks keyboard controls (*.cfg)|*.cfg",
			InitialDirectory = text,
			Multiselect = true,
			RestoreDirectory = true
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() != 1)
			{
				return;
			}
			string[] fileNames = ((FileDialog)val).FileNames;
			foreach (string text2 in fileNames)
			{
				if (!CheckValidCFGAndLoad(text2))
				{
					list.Add(Path.GetFileNameWithoutExtension(text2));
				}
			}
			if (list.Count > 0)
			{
				MessageBox.Show("The following CFG files could not be loaded.\n" + string.Join("\n", list.ToArray()));
			}
			if (mLoadedCFGDict.Count > 0)
			{
				InitCFGList();
				((UIElement)mLoadedCFGsListView).Visibility = (Visibility)0;
				((Selector)mLoadedCFGsListView).SelectedIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool CheckValidCFGAndLoad(string filePath)
	{
		bool result = false;
		try
		{
			IMConfig value = JsonConvert.DeserializeObject<IMConfig>(File.ReadAllText(filePath), Utils.GetSerializerSettings());
			result = true;
			mLoadedCFGDict.Add(filePath, value);
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to read cfg file... filepath: " + filePath + " Err : " + ex.Message);
		}
		return result;
	}

	private List<IMAction> GetFinalListOfActions(Dictionary<string, List<IMAction>> dict)
	{
		List<IMAction> list = new List<IMAction>();
		foreach (string key in dict.Keys)
		{
			foreach (IMAction item in dict[key])
			{
				item.GuidanceCategory = key;
				list.Add(item);
			}
		}
		return list;
	}

	private void SaveCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		foreach (ListViewItem item in (IEnumerable)((ItemsControl)mLoadedCFGsListView).Items)
		{
			ListViewItem val = item;
			if (!((ContentControl)val).Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
			{
				continue;
			}
			try
			{
				string text = ((FrameworkElement)val).Tag.ToString();
				IMConfig iMConfig = mLoadedCFGDict[text];
				List<IMControlScheme> list = new List<IMControlScheme>();
				foreach (IMControlScheme controlScheme in iMConfig.ControlSchemes)
				{
					IMControlScheme iMControlScheme = controlScheme.DeepCopy();
					iMControlScheme.SetGameControls(GetFinalListOfActions(mSchemeTreeMapping[iMConfig][controlScheme]));
					list.Add(iMControlScheme);
				}
				iMConfig.ControlSchemes = list;
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)1;
				WriteFile(text, JsonConvert.SerializeObject((object)iMConfig, serializerSettings));
				((ContentControl)val).Content = ((ContentControl)val).Content.ToString().TrimEnd("* (modified)".ToCharArray());
			}
			catch (Exception arg)
			{
				string text2 = $"Couldn't write to file: {((FrameworkElement)val).Tag.ToString()}, Ex: {arg}";
				Logger.Error(text2);
				MessageBox.Show(text2);
			}
		}
	}

	private void WriteFile(string fullFilePath, string output)
	{
		string text = fullFilePath + ".tmp";
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		File.WriteAllText(text, output);
		if (File.Exists(fullFilePath))
		{
			File.Delete(fullFilePath);
		}
		File.Move(text, fullFilePath);
	}

	private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
	{
		DependencyObject parent = VisualTreeHelper.GetParent(child);
		if (parent == null)
		{
			return default(T);
		}
		T val = (T)(object)((parent is T) ? parent : null);
		if (val != null)
		{
			return val;
		}
		return FindVisualParent<T>(parent);
	}

	private void MapTreeViewFromDict(Dictionary<string, List<IMAction>> dict)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		foreach (string key in dict.Keys)
		{
			TreeViewItem val = new TreeViewItem();
			((HeaderedItemsControl)val).Header = key;
			foreach (IMAction item in dict[key])
			{
				TreeViewItem val2 = new TreeViewItem();
				((HeaderedItemsControl)val2).Header = GetGuidanceFromIMAction(item.Guidance.Values);
				((FrameworkElement)val2).Tag = item;
				((ItemsControl)val).Items.Add((object)val2);
			}
			((ItemsControl)mIMActionsTreeView).Items.Add((object)val);
		}
	}

	private void BuildIMActionsTree()
	{
		if (mSchemeTreeMapping.ContainsKey(mCurrentlySelectedCFG) && mSchemeTreeMapping[mCurrentlySelectedCFG].ContainsKey(mCurrentlySelectedScheme))
		{
			MapTreeViewFromDict(mSchemeTreeMapping[mCurrentlySelectedCFG][mCurrentlySelectedScheme]);
			return;
		}
		foreach (IMControlScheme controlScheme in mCurrentlySelectedCFG.ControlSchemes)
		{
			Dictionary<string, List<IMAction>> dictionary = new Dictionary<string, List<IMAction>>();
			foreach (IMAction gameControl in controlScheme.GameControls)
			{
				if (!dictionary.ContainsKey(gameControl.GuidanceCategory))
				{
					dictionary[gameControl.GuidanceCategory] = new List<IMAction>();
				}
				dictionary[gameControl.GuidanceCategory].Add(gameControl);
			}
			if (!mSchemeTreeMapping.ContainsKey(mCurrentlySelectedCFG))
			{
				mSchemeTreeMapping[mCurrentlySelectedCFG] = new Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>();
			}
			mSchemeTreeMapping[mCurrentlySelectedCFG][controlScheme] = dictionary;
			if (controlScheme == mCurrentlySelectedScheme)
			{
				MapTreeViewFromDict(dictionary);
			}
		}
	}

	private static string GetGuidanceFromIMAction(Dictionary<string, string>.ValueCollection valuePairs)
	{
		if (valuePairs.Count == 0)
		{
			return "NO_GUIDANCE";
		}
		string text = "";
		foreach (string valuePair in valuePairs)
		{
			text = text + valuePair + " / ";
		}
		if (text.Length > 5)
		{
			text = text.Substring(0, text.Length - 3);
		}
		return text;
	}

	private void InitCFGList()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		foreach (string key in mLoadedCFGDict.Keys)
		{
			ListViewItem val = new ListViewItem();
			((ContentControl)val).Content = Path.GetFileNameWithoutExtension(key);
			((FrameworkElement)val).Tag = key;
			((ItemsControl)mLoadedCFGsListView).Items.Add((object)val);
		}
	}

	private void GenerateTreeView()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((UIElement)mIMActionsTreeView).PreviewMouseMove += new MouseEventHandler(TreeView_PreviewMouseMove);
		((ItemsControl)mIMActionsTreeView).ItemContainerStyle = GetIMActionsListStyle();
	}

	private void GenerateSchemesListView()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		((ItemsControl)mSchemesListView).DisplayMemberPath = "Name";
		((ItemsControl)mSchemesListView).ItemsSource = mSchemesList;
		((UIElement)mSchemesListView).PreviewMouseMove += new MouseEventHandler(ListView_PreviewMouseMove);
		((ItemsControl)mSchemesListView).ItemContainerStyle = GetSchemesListStyle();
	}

	private Style GetSchemesListStyle()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006d: Expected O, but got Unknown
		Style val = new Style(typeof(ListViewItem));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new Setter(UIElement.AllowDropProperty, (object)true));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(AnyItem_PreviewMouseLeftButtonDown)));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new EventSetter(UIElement.DropEvent, (Delegate)new DragEventHandler(SchemeItem_Drop)));
		return val;
	}

	private Style GetIMActionsListStyle()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006d: Expected O, but got Unknown
		Style val = new Style(typeof(TreeViewItem));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new Setter(UIElement.AllowDropProperty, (object)true));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(AnyItem_PreviewMouseLeftButtonDown)));
		((Collection<SetterBase>)(object)val.Setters).Add((SetterBase)new EventSetter(UIElement.DropEvent, (Delegate)new DragEventHandler(IMActionItem_Drop)));
		return val;
	}

	private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Point position = e.GetPosition((IInputElement)null);
		Vector val = _dragStartPoint - position;
		if ((int)e.LeftButton == 1 && (Math.Abs(((Vector)(ref val)).X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(((Vector)(ref val)).Y) > SystemParameters.MinimumVerticalDragDistance))
		{
			ListViewItem val2 = FindVisualParent<ListViewItem>((DependencyObject)((RoutedEventArgs)e).OriginalSource);
			if (val2 != null)
			{
				DragDrop.DoDragDrop((DependencyObject)(object)val2, ((FrameworkElement)val2).DataContext, (DragDropEffects)2);
			}
		}
	}

	private void TreeView_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Point position = e.GetPosition((IInputElement)null);
		Vector val = _dragStartPoint - position;
		if ((int)e.LeftButton == 1 && (Math.Abs(((Vector)(ref val)).X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(((Vector)(ref val)).Y) > SystemParameters.MinimumVerticalDragDistance))
		{
			TreeViewItem val2 = FindVisualParent<TreeViewItem>((DependencyObject)((RoutedEventArgs)e).OriginalSource);
			if (val2 != null)
			{
				DragDrop.DoDragDrop((DependencyObject)(object)val2, (object)val2, (DragDropEffects)2);
			}
		}
	}

	private void AnyItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_dragStartPoint = ((MouseEventArgs)e).GetPosition((IInputElement)null);
	}

	private void SchemeItem_Drop(object sender, DragEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IMControlScheme iMControlScheme = e.Data.GetData(typeof(IMControlScheme)) as IMControlScheme;
		IMControlScheme iMControlScheme2 = ((FrameworkElement)(ListViewItem)sender).DataContext as IMControlScheme;
		int num = ((CollectionView)((ItemsControl)mSchemesListView).Items).IndexOf((object)iMControlScheme);
		int num2 = ((CollectionView)((ItemsControl)mSchemesListView).Items).IndexOf((object)iMControlScheme2);
		if (num != -1 && num2 != -1)
		{
			MoveItem(iMControlScheme, num, num2);
			((CollectionView)((ItemsControl)mSchemesListView).Items).Refresh();
			MarkCurrentCFGModified();
		}
	}

	private void MarkCurrentCFGModified()
	{
		object selectedItem = ((Selector)mLoadedCFGsListView).SelectedItem;
		ListBoxItem val = (ListBoxItem)((selectedItem is ListBoxItem) ? selectedItem : null);
		if (!((ContentControl)val).Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
		{
			((ContentControl)val).Content = ((ContentControl)val).Content?.ToString() + "* (modified)";
		}
	}

	public static ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
	{
		DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)item);
		while (!(parent is TreeViewItem) && !(parent is TreeView))
		{
			parent = VisualTreeHelper.GetParent(parent);
		}
		return (ItemsControl)(object)((parent is ItemsControl) ? parent : null);
	}

	private void IMActionItem_Drop(object sender, DragEventArgs e)
	{
		object data = e.Data.GetData(typeof(TreeViewItem));
		object sourceItem = ((data is TreeViewItem) ? data : null);
		object source = ((RoutedEventArgs)e).Source;
		TreeViewItem targetItem = (TreeViewItem)((source is TreeViewItem) ? source : null);
		TreeItemDrop item = new TreeItemDrop((TreeViewItem)sourceItem, targetItem, mIMActionsTreeView);
		MoveItem2(item);
		((CollectionView)((ItemsControl)mIMActionsTreeView).Items).Refresh();
		UpdateTreeDictionary();
		MarkCurrentCFGModified();
	}

	private void UpdateTreeDictionary()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		Dictionary<string, List<IMAction>> dictionary = new Dictionary<string, List<IMAction>>();
		foreach (TreeViewItem item in (IEnumerable)((ItemsControl)mIMActionsTreeView).Items)
		{
			TreeViewItem val = item;
			List<IMAction> list = new List<IMAction>();
			foreach (TreeViewItem item2 in (IEnumerable)((ItemsControl)val).Items)
			{
				TreeViewItem val2 = item2;
				list.Add((IMAction)((FrameworkElement)val2).Tag);
			}
			dictionary[(string)((HeaderedItemsControl)val).Header] = list;
		}
		mSchemeTreeMapping[mCurrentlySelectedCFG][mCurrentlySelectedScheme] = dictionary;
	}

	private void MoveItem2(TreeItemDrop item)
	{
		if (item.IsTargetCategory != item.IsSourceCategory)
		{
			return;
		}
		if (item.AreSourceAndTargetCategories)
		{
			if (((CollectionView)((ItemsControl)mIMActionsTreeView).Items).Count > item.SourceIndex)
			{
				((ItemsControl)mIMActionsTreeView).Items.RemoveAt(item.SourceIndex);
				((ItemsControl)mIMActionsTreeView).Items.Insert(item.TargetIndex, (object)item.Source);
			}
		}
		else if (((CollectionView)((ItemsControl)item.SourceParent).Items).Count > item.SourceIndex && ((CollectionView)((ItemsControl)item.TargetParent).Items).Count > item.TargetIndex)
		{
			((ItemsControl)item.SourceParent).Items.RemoveAt(item.SourceIndex);
			((ItemsControl)item.TargetParent).Items.Insert(item.TargetIndex, (object)item.Source);
		}
	}

	private void MoveItem(IMControlScheme source, int sourceIndex, int targetIndex)
	{
		if (sourceIndex < targetIndex)
		{
			mSchemesList.Insert(targetIndex + 1, source);
			mSchemesList.RemoveAt(sourceIndex);
			return;
		}
		int num = sourceIndex + 1;
		if (mSchemesList.Count + 1 > num)
		{
			mSchemesList.Insert(targetIndex, source);
			mSchemesList.RemoveAt(num);
		}
	}

	private void LoadedCFGsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (((Selector)mLoadedCFGsListView).SelectedItem != null)
		{
			ClearIMLists();
			Dictionary<string, IMConfig> dictionary = mLoadedCFGDict;
			object selectedItem = ((Selector)mLoadedCFGsListView).SelectedItem;
			mCurrentlySelectedCFG = dictionary[(string)((FrameworkElement)((selectedItem is ListViewItem) ? selectedItem : null)).Tag];
			mSchemesList = mCurrentlySelectedCFG.ControlSchemes;
			GenerateSchemesListView();
			((ItemsControl)mIMActionsTreeView).Items.Clear();
			if (((CollectionView)((ItemsControl)mSchemesListView).Items).Count > 0)
			{
				((UIElement)mSchemesListView).Visibility = (Visibility)0;
			}
		}
	}

	private void mSchemesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		int selectedIndex = ((Selector)mSchemesListView).SelectedIndex;
		if (selectedIndex != -1)
		{
			mCurrentlySelectedScheme = mSchemesList[selectedIndex];
			ClearIMActionsTree();
			BuildIMActionsTree();
			GenerateTreeView();
			if (((CollectionView)((ItemsControl)mIMActionsTreeView).Items).Count > 0)
			{
				((UIElement)mIMActionsTreeView).Visibility = (Visibility)0;
			}
		}
	}

	private void mIMActionsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		((UIElement)mActionTextBox).IsEnabled = false;
		mActionTextBox.ScrollToLine(0);
		try
		{
			object selectedItem = mIMActionsTreeView.SelectedItem;
			IMAction iMAction = (IMAction)((FrameworkElement)((selectedItem is TreeViewItem) ? selectedItem : null)).Tag;
			if (iMAction != null)
			{
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)1;
				mActionTextBox.Text = JsonConvert.SerializeObject((object)iMAction, serializerSettings);
				((UIElement)mActionJsonGrid).Visibility = (Visibility)0;
			}
			else
			{
				mActionTextBox.Text = "";
				((UIElement)mActionJsonGrid).Visibility = (Visibility)2;
			}
		}
		catch
		{
			mActionTextBox.Text = "";
			((UIElement)mActionJsonGrid).Visibility = (Visibility)2;
		}
	}

	private void EditButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show("Not implemented");
		((UIElement)mActionTextBox).IsEnabled = true;
	}

	private void SaveButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show("Not implemented");
	}

	private void mLoadedCFGsListView_Scroll(object sender, ScrollEventArgs e)
	{
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/cfgreorderwindow.xaml", UriKind.Relative);
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
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mLoadedCFGsListView = (ListView)target;
			((Selector)mLoadedCFGsListView).SelectionChanged += new SelectionChangedEventHandler(LoadedCFGsListView_SelectionChanged);
			break;
		case 2:
			((UIElement)(Button)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LoadCFGButton_PreviewMouseLeftButtonUp);
			break;
		case 3:
			((UIElement)(Button)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SaveCFGButton_PreviewMouseLeftButtonUp);
			break;
		case 4:
			mCurrentlyLoadedStackPanel = (StackPanel)target;
			break;
		case 5:
			mCurrentlyLoadedCFGTextBlock = (TextBlock)target;
			break;
		case 6:
			mSchemesListView = (ListView)target;
			((Selector)mSchemesListView).SelectionChanged += new SelectionChangedEventHandler(mSchemesListView_SelectionChanged);
			break;
		case 7:
			mIMActionsTreeView = (TreeView)target;
			mIMActionsTreeView.SelectedItemChanged += mIMActionsTreeView_SelectedItemChanged;
			break;
		case 8:
			mActionJsonGrid = (Grid)target;
			break;
		case 9:
			mActionTextBox = (TextBox)target;
			break;
		case 10:
			mEditButton = (Button)target;
			((UIElement)mEditButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(EditButton_PreviewMouseLeftButtonUp);
			break;
		case 11:
			mSaveButton = (Button)target;
			((UIElement)mSaveButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(SaveButton_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
