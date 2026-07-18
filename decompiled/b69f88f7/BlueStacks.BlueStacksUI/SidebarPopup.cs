using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI;

public class SidebarPopup : UserControl, IComponentConnector
{
	private const int NumElementsPerRow = 3;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mMainStackPanel;

	private bool _contentLoaded;

	private int NumColumns => ((Panel)mMainStackPanel).Children.Count;

	public SidebarPopup()
	{
		InitializeComponent();
	}

	public void AddElement(SidebarElement element)
	{
		if (element == null)
		{
			return;
		}
		RemoveParentIfExists(element);
		if (NumColumns == 0)
		{
			AddToNewPanel(element);
			return;
		}
		UIElement obj = ((Panel)mMainStackPanel).Children[NumColumns - 1];
		StackPanel val = (StackPanel)(object)((obj is StackPanel) ? obj : null);
		if (((Panel)val).Children.Count == 3)
		{
			AddToNewPanel(element);
		}
		else
		{
			((Panel)val).Children.Add((UIElement)(object)element);
		}
	}

	private static void RemoveParentIfExists(SidebarElement element)
	{
		DependencyObject parent = ((FrameworkElement)element).Parent;
		StackPanel val = (StackPanel)(object)((parent is StackPanel) ? parent : null);
		if (val != null)
		{
			((Panel)val).Children.Remove((UIElement)(object)element);
		}
	}

	private void AddToNewPanel(SidebarElement element)
	{
		((Panel)CreateStackPanel()).Children.Add((UIElement)(object)element);
	}

	public SidebarElement PopElement()
	{
		UIElement obj = ((Panel)mMainStackPanel).Children[NumColumns - 1];
		StackPanel val = (StackPanel)(object)((obj is StackPanel) ? obj : null);
		SidebarElement sidebarElement = ((Panel)val).Children[((Panel)val).Children.Count - 1] as SidebarElement;
		((Panel)val).Children.Remove((UIElement)(object)sidebarElement);
		if (((Panel)val).Children.Count == 0)
		{
			((Panel)mMainStackPanel).Children.Remove((UIElement)(object)val);
		}
		return sidebarElement;
	}

	private StackPanel CreateStackPanel()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		StackPanel val = new StackPanel
		{
			Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
			Orientation = (Orientation)1
		};
		((Panel)mMainStackPanel).Children.Add((UIElement)(object)val);
		return val;
	}

	private void SidebarPopup_Loaded(object sender, RoutedEventArgs e)
	{
	}

	internal void InitAllElements(IEnumerable<SidebarElement> listOfHiddenElements)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (SidebarElement listOfHiddenElement in listOfHiddenElements)
		{
			if ((int)((UIElement)listOfHiddenElement).Visibility == 0)
			{
				((FrameworkElement)listOfHiddenElement).Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
				AddElement(listOfHiddenElement);
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebarpopup.xaml", UriKind.Relative);
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(SidebarPopup)target).Loaded += new RoutedEventHandler(SidebarPopup_Loaded);
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
