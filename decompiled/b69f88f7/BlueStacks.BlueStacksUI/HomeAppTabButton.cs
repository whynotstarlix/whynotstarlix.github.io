using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class HomeAppTabButton : Button, IComponentConnector
{
	private int animationTime = 150;

	private MainWindow mMainWindow;

	private static bool mIsSwipeDirectonLeft = true;

	private TranslateTransform mTranslateTransform = new TranslateTransform();

	private BrowserControl mAssociatedUserControl;

	private bool mIsSelected;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid tabGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox ImageBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mTabHeader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mAppTabNotificationCountBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppTabNotificationCount;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBottomGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGridHighlighterBox;

	private bool _contentLoaded;

	public string Key { get; set; } = string.Empty;

	public MainWindow ParentWindow
	{
		get
		{
			if (mMainWindow == null)
			{
				mMainWindow = Window.GetWindow((DependencyObject)(object)this) as MainWindow;
			}
			return mMainWindow;
		}
	}

	public string Text
	{
		get
		{
			return mTabHeader.Text;
		}
		set
		{
			Key = value + "-Normal";
			ImageBox.ImageName = Key;
			BlueStacksUIBinding.Bind(mTabHeader, value, "");
		}
	}

	public int Column { get; internal set; }

	public BrowserControl AssociatedUserControl
	{
		get
		{
			return mAssociatedUserControl;
		}
		set
		{
			mAssociatedUserControl = value;
			if (mAssociatedUserControl != null)
			{
				((UIElement)mAssociatedUserControl).RenderTransform = (Transform)(object)mTranslateTransform;
			}
		}
	}

	public bool IsSelected
	{
		get
		{
			return mIsSelected;
		}
		set
		{
			if (ParentWindow.StaticComponents.mSelectedHomeAppTabButton == this && value)
			{
				return;
			}
			mIsSelected = value;
			if (ParentWindow.StaticComponents.mSelectedHomeAppTabButton != null)
			{
				HomeAppTabButton mSelectedHomeAppTabButton = ParentWindow.StaticComponents.mSelectedHomeAppTabButton;
				if (Column > mSelectedHomeAppTabButton.Column)
				{
					mIsSwipeDirectonLeft = true;
				}
				else
				{
					mIsSwipeDirectonLeft = false;
				}
				ParentWindow.StaticComponents.mSelectedHomeAppTabButton = null;
				if (mAssociatedUserControl == mSelectedHomeAppTabButton.mAssociatedUserControl)
				{
					mSelectedHomeAppTabButton.IsAnimationIgnored = true;
					IsAnimationIgnored = true;
				}
				mSelectedHomeAppTabButton.IsSelected = false;
			}
			if (mIsSelected)
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabHeader, TextBlock.ForegroundProperty, "SelectedHomeAppTabForegroundColor");
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
				ParentWindow.StaticComponents.mSelectedHomeAppTabButton = this;
				ParentWindow.Utils.ResetPendingUIOperations();
				((UIElement)mGridHighlighterBox).Visibility = (Visibility)0;
				AnimateAssociatedGrid(show: true);
				ImageBox.ImageName = Key.Replace("Normal", "Active");
				if (mAssociatedUserControl.CefBrowser != null)
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.SetSearchTextBoxFocus(animationTime + 100);
					MiscUtils.SetFocusAsync((UIElement)(object)mAssociatedUserControl.CefBrowser);
				}
			}
			else
			{
				BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
				((UIElement)mGridHighlighterBox).Visibility = (Visibility)1;
				AnimateAssociatedGrid(show: false);
				ImageBox.ImageName = Key.Replace("Active", "Normal");
			}
		}
	}

	public bool IsAnimationIgnored { get; set; }

	private void AssociatedGrid_LayoutUpdated(object sender, EventArgs e)
	{
		try
		{
			if (!IsSelected && Math.Abs(Math.Abs(mTranslateTransform.X) - ((FrameworkElement)mAssociatedUserControl).ActualWidth) <= 10.0)
			{
				((UIElement)mAssociatedUserControl).Visibility = (Visibility)1;
				((UIElement)mAssociatedUserControl).LayoutUpdated -= AssociatedGrid_LayoutUpdated;
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Error updating " + ex.ToString());
		}
	}

	private void AnimateAssociatedGrid(bool show)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		if (IsAnimationIgnored)
		{
			IsAnimationIgnored = false;
			return;
		}
		((UIElement)mAssociatedUserControl).LayoutUpdated += AssociatedGrid_LayoutUpdated;
		DoubleAnimation val;
		if (!show)
		{
			val = ((!mIsSwipeDirectonLeft) ? new DoubleAnimation(0.0, ((FrameworkElement)mAssociatedUserControl).ActualWidth, Duration.op_Implicit(TimeSpan.FromMilliseconds((double)animationTime))) : new DoubleAnimation(0.0, -1.0 * ((FrameworkElement)mAssociatedUserControl).ActualWidth, Duration.op_Implicit(TimeSpan.FromMilliseconds((double)animationTime))));
		}
		else
		{
			((UIElement)mAssociatedUserControl).Visibility = (Visibility)0;
			val = ((!mIsSwipeDirectonLeft) ? new DoubleAnimation(-1.0 * ((FrameworkElement)mAssociatedUserControl).ActualWidth, 0.0, Duration.op_Implicit(TimeSpan.FromMilliseconds((double)animationTime))) : new DoubleAnimation(((FrameworkElement)mAssociatedUserControl).ActualWidth, 0.0, Duration.op_Implicit(TimeSpan.FromMilliseconds((double)animationTime))));
		}
		((Animatable)mTranslateTransform).BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline)(object)val);
	}

	public HomeAppTabButton()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		InitializeComponent();
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		IsSelected = true;
	}

	private void Button_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundHoverColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundHoverColor");
		}
	}

	private void Button_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!IsSelected)
		{
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
			BlueStacksUIBinding.BindColor((DependencyObject)(object)mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
		}
	}

	private void Button_Loaded(object sender, RoutedEventArgs e)
	{
		SetMaxWidth();
	}

	internal void SetMaxWidth(double extraWidth = 0.0)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		Typeface val = new Typeface(mTabHeader.FontFamily, mTabHeader.FontStyle, mTabHeader.FontWeight, mTabHeader.FontStretch);
		FormattedText val2 = new FormattedText(mTabHeader.Text, Thread.CurrentThread.CurrentCulture, ((FrameworkElement)mTabHeader).FlowDirection, val, mTabHeader.FontSize, mTabHeader.Foreground);
		num += ((FrameworkElement)mAppTabNotificationCountBorder).ActualWidth;
		num += val2.WidthIncludingTrailingWhitespace;
		num += ((FrameworkElement)tabGrid).ActualHeight;
		num += 30.0;
		GridLength width;
		if (extraWidth == double.MaxValue)
		{
			width = tabGrid.ColumnDefinitions[0].Width;
			if (((GridLength)(ref width)).IsStar)
			{
				num += 50.0;
			}
		}
		else
		{
			num += extraWidth;
		}
		int column = Grid.GetColumn((UIElement)(object)mTabHeader);
		if (!(extraWidth > 0.0) || !(extraWidth < double.MaxValue))
		{
			width = tabGrid.ColumnDefinitions[0].Width;
			if (!((GridLength)(ref width)).IsStar || extraWidth != double.MaxValue)
			{
				tabGrid.ColumnDefinitions[column].Width = new GridLength(1.0, (GridUnitType)2);
				tabGrid.ColumnDefinitions[0].Width = new GridLength(0.0, (GridUnitType)1);
				tabGrid.ColumnDefinitions[5].Width = new GridLength(0.0, (GridUnitType)1);
				goto IL_021a;
			}
		}
		tabGrid.ColumnDefinitions[column].Width = new GridLength(1.0, (GridUnitType)0);
		tabGrid.ColumnDefinitions[0].Width = new GridLength(1.0, (GridUnitType)2);
		tabGrid.ColumnDefinitions[5].Width = new GridLength(1.0, (GridUnitType)2);
		goto IL_021a;
		IL_021a:
		column = Grid.GetColumn((UIElement)(object)this);
		((Grid)((FrameworkElement)this).Parent).ColumnDefinitions[column].MaxWidth = num;
	}

	private void mAppTabNotificationCountBorder_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		SetMaxWidth();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/homeapptabbutton.xaml", UriKind.Relative);
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
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((ButtonBase)(HomeAppTabButton)target).Click += new RoutedEventHandler(Button_Click);
			((UIElement)(HomeAppTabButton)target).MouseEnter += new MouseEventHandler(Button_MouseEnter);
			((UIElement)(HomeAppTabButton)target).MouseLeave += new MouseEventHandler(Button_MouseLeave);
			((FrameworkElement)(HomeAppTabButton)target).Loaded += new RoutedEventHandler(Button_Loaded);
			break;
		case 2:
			tabGrid = (Grid)target;
			break;
		case 3:
			ImageBox = (CustomPictureBox)target;
			break;
		case 4:
			mTabHeader = (TextBlock)target;
			break;
		case 5:
			mAppTabNotificationCountBorder = (Border)target;
			((FrameworkElement)mAppTabNotificationCountBorder).SizeChanged += new SizeChangedEventHandler(mAppTabNotificationCountBorder_SizeChanged);
			break;
		case 6:
			mAppTabNotificationCount = (TextBlock)target;
			break;
		case 7:
			mBottomGrid = (Grid)target;
			break;
		case 8:
			mGridHighlighterBox = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
