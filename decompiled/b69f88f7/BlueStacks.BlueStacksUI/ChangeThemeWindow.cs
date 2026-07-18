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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ChangeThemeWindow : UserControl, IComponentConnector
{
	private WrapPanel ThemesDrawer;

	private MainWindow ParentWindow;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mChangeThemeWindowIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mLblBlueStacksChangeTheme;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCrossButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mThemesDrawerScrollBar;

	private bool _contentLoaded;

	public ChangeThemeWindow(MainWindow parentWindow)
	{
		InitializeComponent();
		ParentWindow = parentWindow;
		object content = ((ContentControl)mThemesDrawerScrollBar).Content;
		ThemesDrawer = (WrapPanel)((content is WrapPanel) ? content : null);
		AddSkinImages();
	}

	public void AddSkinImages()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((Panel)ThemesDrawer).Children.Clear();
			string[] directories = Directory.GetDirectories(RegistryManager.Instance.ClientInstallDir);
			foreach (string text in directories)
			{
				if (File.Exists(Path.Combine(text, "ThemeThumbnail.png")))
				{
					string themeName = BlueStacksUIColorManager.GetThemeName(text);
					SkinSelectorControl skinSelectorControl = new SkinSelectorControl();
					((UIElement)skinSelectorControl).Visibility = (Visibility)0;
					((FrameworkElement)skinSelectorControl).HorizontalAlignment = (HorizontalAlignment)1;
					((FrameworkElement)skinSelectorControl).VerticalAlignment = (VerticalAlignment)0;
					SkinSelectorControl skinSelectorControl2 = skinSelectorControl;
					((UIElement)skinSelectorControl2.mThemeImage).Visibility = (Visibility)0;
					((UIElement)skinSelectorControl2.mThemeName).Visibility = (Visibility)0;
					skinSelectorControl2.mThemeImage.IsFullImagePath = true;
					skinSelectorControl2.mThemeImage.ImageName = Path.Combine(text, "ThemeThumbnail.png");
					((FrameworkElement)skinSelectorControl2.mThemeCheckButton).Height = 30.0;
					((FrameworkElement)skinSelectorControl2.mThemeName).ToolTip = themeName;
					((FrameworkElement)skinSelectorControl2.mThemeName).Width = double.NaN;
					((FrameworkElement)skinSelectorControl2.mThemeCheckButton).Width = double.NaN;
					skinSelectorControl2.mThemeName.Text = themeName;
					BlueStacksUIBinding.BindColor((DependencyObject)(object)skinSelectorControl2.mThemeName, TextBlock.ForegroundProperty, "ContextMenuItemForegroundColor");
					((FrameworkElement)skinSelectorControl2.mThemeCheckButton).Tag = Path.GetFileName(text);
					((ButtonBase)skinSelectorControl2.mThemeCheckButton).Click += new RoutedEventHandler(ThemeApplyButton_Click);
					if (string.Compare(RegistryManager.ClientThemeName, Path.GetFileName(text), StringComparison.OrdinalIgnoreCase) == 0)
					{
						skinSelectorControl2.mThemeAppliedText.Text = LocaleStrings.GetLocalizedString("STRING_APPLIED", "");
						((UIElement)skinSelectorControl2.mThemeAppliedText).Visibility = (Visibility)0;
						((FrameworkElement)skinSelectorControl2.mThemeAppliedText).Margin = new Thickness(0.0, 3.0, 4.0, 0.0);
					}
					else
					{
						skinSelectorControl2.mThemeCheckButton.ButtonColor = (ButtonColors)4;
						((UIElement)skinSelectorControl2.mThemeCheckButton).IsEnabled = true;
						((ContentControl)skinSelectorControl2.mThemeCheckButton).Content = LocaleStrings.GetLocalizedString("STRING_APPLY", "");
						((UIElement)skinSelectorControl2.mThemeCheckButton).Visibility = (Visibility)0;
					}
					((Panel)ThemesDrawer).Children.Add((UIElement)(object)skinSelectorControl2);
					((UIElement)mThemesDrawerScrollBar).Visibility = (Visibility)0;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in populating themes in skin widget " + ex.ToString());
		}
	}

	private void ThemeApplyButton_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("Clicked theme apply button");
		string themeName = ((FrameworkElement)((sender is CustomButton) ? sender : null)).Tag.ToString();
		ParentWindow.Utils.ApplyTheme(themeName);
		AddSkinImages();
		ParentWindow.Utils.RestoreWallpaperImageForAllVms();
	}

	private void mCrossButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	private void mCrossButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		((RoutedEventArgs)e).Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/changethemewindow.xaml", UriKind.Relative);
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mGrid = (Grid)target;
			break;
		case 2:
			mChangeThemeWindowIcon = (CustomPictureBox)target;
			break;
		case 3:
			mLblBlueStacksChangeTheme = (TextBlock)target;
			break;
		case 4:
			mCrossButton = (CustomPictureBox)target;
			((UIElement)mCrossButton).MouseLeftButtonUp += new MouseButtonEventHandler(mCrossButton_MouseLeftButtonUp);
			((UIElement)mCrossButton).PreviewMouseDown += new MouseButtonEventHandler(mCrossButton_PreviewMouseDown);
			break;
		case 5:
			mThemesDrawerScrollBar = (ScrollViewer)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
