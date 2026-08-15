using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class OnBoardingPopupWindow : CustomWindow, INotifyPropertyChanged, IComponentConnector
{
	private string mHeaderContent;

	private string mBodyContent;

	private bool mIsLastPopup;

	private PopupArrowAlignment mPopArrowAlignment;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid ContentGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock headerTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock bodyTextBlock;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BlurbMessageControl bodyContentBlurbControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton OkayButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path RightArrow;

	private bool _contentLoaded;

	public string HeaderContent
	{
		get
		{
			return mHeaderContent;
		}
		set
		{
			if (mHeaderContent != value)
			{
				mHeaderContent = value;
				OnPropertyChanged("HeaderContent");
			}
		}
	}

	public string BodyContent
	{
		get
		{
			return mBodyContent;
		}
		set
		{
			if (mBodyContent != value)
			{
				mBodyContent = value;
				OnPropertyChanged("BodyContent");
			}
		}
	}

	public bool IsLastPopup
	{
		get
		{
			return mIsLastPopup;
		}
		set
		{
			if (mIsLastPopup != value)
			{
				mIsLastPopup = value;
				OnPropertyChanged("IsLastPopup");
			}
		}
	}

	public PopupArrowAlignment PopArrowAlignment
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mPopArrowAlignment;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (mPopArrowAlignment != value)
			{
				mPopArrowAlignment = value;
				OnPropertyChanged("PopArrowAlignment");
			}
		}
	}

	public UIElement PlacementTarget { get; set; }

	public int LeftMargin { get; set; }

	public int TopMargin { get; set; }

	public bool IsBlurbRelatedToGuidance { get; set; }

	public string PackageName { get; set; }

	public MainWindow ParentWindow { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public event Action Startevent;

	public event Action Endevent;

	protected void OnPropertyChanged(string property)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
	}

	public OnBoardingPopupWindow(MainWindow mainWindow, string packageName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		mPopArrowAlignment = (PopupArrowAlignment)1;
		((CustomWindow)this)._002Ector();
		PackageName = packageName;
		ParentWindow = mainWindow;
		InitializeComponent();
		if (ParentWindow != null)
		{
			((FrameworkElement)ParentWindow).SizeChanged -= new SizeChangedEventHandler(Owner_SizeChanged);
			((Window)ParentWindow).LocationChanged -= Owner_SizeChanged;
			((Window)ParentWindow).StateChanged -= Owner_SizeChanged;
			((FrameworkElement)ParentWindow).SizeChanged += new SizeChangedEventHandler(Owner_SizeChanged);
			((Window)ParentWindow).LocationChanged += Owner_SizeChanged;
			((Window)ParentWindow).StateChanged += Owner_SizeChanged;
		}
	}

	private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
	{
		Startevent?.Invoke();
	}

	private void Owner_SizeChanged(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (PlacementTarget != null && PlacementTarget.IsVisible)
		{
			DependencyProperty leftProperty = Window.LeftProperty;
			Point val = ((Visual)PlacementTarget).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)this).SetValue(leftProperty, (object)(((Point)(ref val)).X / MainWindow.sScalingFactor - (double)LeftMargin));
			DependencyProperty topProperty = Window.TopProperty;
			val = ((Visual)PlacementTarget).PointToScreen(new Point(0.0, 0.0));
			((DependencyObject)this).SetValue(topProperty, (object)(((Point)(ref val)).Y / MainWindow.sScalingFactor - (double)TopMargin));
		}
	}

	public void CloseWindow()
	{
		Endevent?.Invoke();
		((Window)this).Close();
	}

	private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
	{
		Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", ParentWindow.mVmName, PackageName, ((Window)this).Title, "");
		CloseWindow();
	}

	private void CustomWindow_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Key == (0x65710EDE ^ 0x65710E42) && (int)e.SystemKey == 1610217372 - (0x4F494636 | 0x53F1B50F))
		{
			((RoutedEventArgs)e).Handled = true;
			return;
		}
		string keyPressed = string.Empty;
		if ((int)e.Key == 618598374 + (1838184539 << 248265089))
		{
			keyPressed = MainWindow.GetShortcutKey(e.SystemKey);
		}
		else
		{
			keyPressed = MainWindow.GetShortcutKey(e.Key);
		}
		MainWindow mainWindow = (MainWindow)(object)((Window)this).Owner;
		string a = keyPressed;
		string shortcutKeyFromName = mainWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP");
		int comparisonType = ((2017092061 > 363535781) ? 2 : 2);
		if (string.Equals(a, shortcutKeyFromName, (StringComparison)comparisonType) && ((Window)this).Title == "FullScreenBlurb")
		{
			ShortcutKeys val = mainWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut.Where(delegate(ShortcutKeys val2)
			{
				string shortcutKey = val2.ShortcutKey;
				string value = keyPressed;
				int comparisonType2 = ((1042515968 > 155432960) ? 2 : 2);
				return shortcutKey.Equals(value, (StringComparison)comparisonType2);
			}).First();
			ClientHotKeys clienthotKey = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), val.ShortcutName);
			mainWindow.HandleClientHotKey(clienthotKey);
			CloseWindow();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/onboardingpopupwindow.xaml", UriKind.Relative);
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(OnBoardingPopupWindow)target).Loaded += new RoutedEventHandler(CustomWindow_Loaded);
			((UIElement)(OnBoardingPopupWindow)target).KeyDown += new KeyEventHandler(CustomWindow_KeyDown);
			break;
		case 2:
			mMaskBorder = (Border)target;
			break;
		case 3:
			ContentGrid = (Grid)target;
			break;
		case 4:
			headerTextBlock = (TextBlock)target;
			break;
		case 5:
			bodyTextBlock = (TextBlock)target;
			break;
		case 6:
			bodyContentBlurbControl = (BlurbMessageControl)target;
			break;
		case 7:
			OkayButton = (CustomButton)target;
			((ButtonBase)OkayButton).Click += new RoutedEventHandler(OnBoardingPopupNext_Click);
			break;
		case 8:
			((ButtonBase)(CustomButton)target).Click += new RoutedEventHandler(OnBoardingPopupNext_Click);
			break;
		case 9:
			RightArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
