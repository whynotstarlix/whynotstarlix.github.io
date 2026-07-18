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
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class BlueStacksAdvancedExit : UserControl, IDimOverlayControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private string mCurrentGlobalDefault = RegistryManager.Instance.QuitDefaultOption;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCrossButtonPictureBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mParentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTitleGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTitleText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mOptionsGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal StackPanel mOptionsStackPanel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mFooterGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mNoButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mYesButton;

	private bool _contentLoaded;

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow { get; set; } = true;

	public bool ShowTransparentWindow { get; set; }

	public CustomButton YesButton => mYesButton;

	public CustomButton NoButton => mNoButton;

	public CustomPictureBox CrossButton => mCrossButtonPictureBox;

	bool IDimOverlayControl.Close()
	{
		Close();
		return true;
	}

	bool IDimOverlayControl.Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	public BlueStacksAdvancedExit(MainWindow window)
	{
		ParentWindow = window;
		InitializeComponent();
		AddOptions();
	}

	private void AddOptions()
	{
		GenerateOptions("STRING_QUIT_BLUESTACKS", LocaleStringsConstants.ExitOptions);
		AddLineSeperator();
		GenerateOptions("STRING_RESTART", LocaleStringsConstants.RestartOptions);
		AddLineSeperator();
		GenerateCheckBox();
	}

	private void AddLineSeperator()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		Border val = new Border
		{
			Opacity = 0.5,
			Height = 1.0,
			Margin = new Thickness(0.0, 10.0, 0.0, 0.0)
		};
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val, Border.BackgroundProperty, "SettingsWindowTabMenuItemForeground");
		((Panel)mOptionsStackPanel).Children.Add((UIElement)(object)val);
	}

	private void GenerateCheckBox()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		CustomCheckbox val = new CustomCheckbox();
		BlueStacksUIBinding.Bind((ToggleButton)(object)val, "STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04");
		if (val.Image != null)
		{
			((FrameworkElement)val.Image).Height = 14.0;
			((FrameworkElement)val.Image).Width = 14.0;
		}
		((FrameworkElement)val).Height = 20.0;
		((FrameworkElement)val).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
		((ToggleButton)val).IsChecked = false;
		((ToggleButton)val).Checked += new RoutedEventHandler(DontShowAgainCB_Checked);
		((ToggleButton)val).Unchecked += new RoutedEventHandler(DontShowAgainCB_Unchecked);
		((Panel)mOptionsStackPanel).Children.Add((UIElement)(object)val);
	}

	private void DontShowAgainCB_Checked(object sender, RoutedEventArgs e)
	{
		RegistryManager.Instance.IsQuitOptionSaved = true;
	}

	private void DontShowAgainCB_Unchecked(object sender, RoutedEventArgs e)
	{
		RegistryManager.Instance.IsQuitOptionSaved = false;
	}

	private void GenerateOptions(string title, string[] childrenKeys)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		TextBlock val = new TextBlock();
		BlueStacksUIBinding.Bind(val, title, "");
		val.Padding = new Thickness(0.0);
		val.FontSize = 16.0;
		((FrameworkElement)val).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
		BlueStacksUIBinding.BindColor((DependencyObject)(object)val, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
		val.FontWeight = FontWeights.Normal;
		((FrameworkElement)val).HorizontalAlignment = (HorizontalAlignment)0;
		((FrameworkElement)val).VerticalAlignment = (VerticalAlignment)1;
		((Panel)mOptionsStackPanel).Children.Add((UIElement)(object)val);
		foreach (string text in childrenKeys)
		{
			CustomRadioButton val2 = new CustomRadioButton();
			((ToggleButton)val2).Checked += new RoutedEventHandler(Btn_Checked);
			((FrameworkElement)val2).HorizontalAlignment = (HorizontalAlignment)0;
			BlueStacksUIBinding.Bind(val2, text);
			((FrameworkElement)val2).Tag = text;
			((FrameworkElement)val2).Margin = new Thickness(0.0, 10.0, 0.0, 5.0);
			((Panel)mOptionsStackPanel).Children.Add((UIElement)(object)val2);
			if (text == mCurrentGlobalDefault)
			{
				((ToggleButton)val2).IsChecked = true;
			}
		}
	}

	private void Btn_Checked(object sender, RoutedEventArgs e)
	{
		RegistryManager.Instance.QuitDefaultOption = ((FrameworkElement)((sender is CustomRadioButton) ? sender : null)).Tag.ToString();
	}

	internal bool Close()
	{
		try
		{
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			ParentWindow.HideDimOverlay();
			((UIElement)this).Visibility = (Visibility)1;
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close the advanced exit from dimoverlay " + ex.ToString());
		}
		return false;
	}

	private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
	}

	private void MYesButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
	}

	private void MNoButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/bluestacksadvancedexit.xaml", UriKind.Relative);
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mCrossButtonPictureBox = (CustomPictureBox)target;
			((UIElement)mCrossButtonPictureBox).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Close_PreviewMouseLeftButtonUp);
			break;
		case 2:
			mParentGrid = (Grid)target;
			break;
		case 3:
			mTitleGrid = (Grid)target;
			break;
		case 4:
			mTitleText = (TextBlock)target;
			break;
		case 5:
			mOptionsGrid = (Grid)target;
			break;
		case 6:
			mOptionsStackPanel = (StackPanel)target;
			break;
		case 7:
			mFooterGrid = (Grid)target;
			break;
		case 8:
			mNoButton = (CustomButton)target;
			((UIElement)mNoButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MNoButton_PreviewMouseLeftButtonUp);
			break;
		case 9:
			mYesButton = (CustomButton)target;
			((UIElement)mYesButton).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MYesButton_PreviewMouseLeftButtonUp);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
