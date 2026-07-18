using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class CustomToggleButton : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mIconColDef;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mAppLableColDef;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ColumnDefinition mShowColDef;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mAppLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mMuteCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mAutoHideCheckBox;

	private bool _contentLoaded;

	public bool HideIcon
	{
		set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				mIconColDef.Width = new GridLength(0.0);
			}
		}
	}

	public string AppLabel
	{
		get
		{
			return mAppLabel.Text;
		}
		set
		{
			BlueStacksUIBinding.Bind(mAppLabel, value, "");
		}
	}

	public Orientation CheckBoxOrientation
	{
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			mMuteCheckBox.Orientation = value;
			mAutoHideCheckBox.Orientation = value;
			Grid.SetRow((UIElement)(object)mAppLabel, 1);
			Grid.SetRowSpan((UIElement)(object)mAppLabel, 1);
		}
	}

	public bool IsThreeStateCheckBox
	{
		set
		{
			((ToggleButton)mMuteCheckBox).IsThreeState = value;
			((ToggleButton)mAutoHideCheckBox).IsThreeState = value;
		}
	}

	public Visibility CheckBoxLabelVisibility
	{
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			mMuteCheckBox.LabelVisibility = value;
			mAutoHideCheckBox.LabelVisibility = value;
		}
	}

	public BitmapImage Image
	{
		set
		{
			((Image)mAppImage).Source = (ImageSource)(object)value;
			if (value != null)
			{
				mAppImage.ImageName = string.Empty;
			}
		}
	}

	public bool? IsMuted => ((ToggleButton)mMuteCheckBox).IsChecked;

	public bool? IsAutoHide => ((ToggleButton)mAutoHideCheckBox).IsChecked;

	public void HideAppLable()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		mAppLableColDef.Width = new GridLength(0.0);
		mIconColDef.Width = new GridLength(0.0);
	}

	public void HideShowButton()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		mShowColDef.Width = new GridLength(0.0);
		mIconColDef.Width = new GridLength(0.0);
	}

	public CustomToggleButton()
	{
		InitializeComponent();
		SetProperties();
	}

	private void SetProperties()
	{
		BlueStacksUIBinding.Bind((ToggleButton)(object)mMuteCheckBox, "STRING_SHOW");
		BlueStacksUIBinding.Bind((ToggleButton)(object)mAutoHideCheckBox, "STRING_AUTO_HIDE");
	}

	private void MuteButton_Checked(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mAutoHideCheckBox).IsChecked.HasValue && !((ToggleButton)mAutoHideCheckBox).IsChecked.Value && ((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)0, AppLabel);
		}
	}

	private void MuteButton_Unchecked(object sender, RoutedEventArgs e)
	{
		((ToggleButton)mAutoHideCheckBox).IsChecked = false;
		if (((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)5, AppLabel);
		}
	}

	private void MuteCheckBox_Indeterminate(object sender, RoutedEventArgs e)
	{
		if (((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)5, AppLabel);
		}
	}

	private void AutoHideButton_Checked(object sender, RoutedEventArgs e)
	{
		((ToggleButton)mMuteCheckBox).IsChecked = true;
		if (((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)1, AppLabel);
		}
	}

	private void AutoHideButton_Unchecked(object sender, RoutedEventArgs e)
	{
		if (((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)0, AppLabel);
		}
	}

	private void AutoHideCheckBox_Indeterminate(object sender, RoutedEventArgs e)
	{
		if (((FrameworkElement)this).IsLoaded)
		{
			NotificationManager.Instance.UpdateMuteState((MuteState)5, AppLabel);
		}
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)NotificationManager.Instance.IsNotificationMutedForKey(AppLabel, "Android") == 1)
		{
			((ToggleButton)mAutoHideCheckBox).IsChecked = true;
			return;
		}
		if ((int)NotificationManager.Instance.IsNotificationMutedForKey(AppLabel, "Android") == 0)
		{
			((ToggleButton)mMuteCheckBox).IsChecked = true;
			return;
		}
		((ToggleButton)mMuteCheckBox).IsChecked = false;
		((ToggleButton)mAutoHideCheckBox).IsChecked = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/customtogglebutton.xaml", UriKind.Relative);
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(CustomToggleButton)target).Loaded += new RoutedEventHandler(UserControl_Loaded);
			break;
		case 2:
			mIconColDef = (ColumnDefinition)target;
			break;
		case 3:
			mAppLableColDef = (ColumnDefinition)target;
			break;
		case 4:
			mShowColDef = (ColumnDefinition)target;
			break;
		case 5:
			mAppImage = (CustomPictureBox)target;
			break;
		case 6:
			mAppLabel = (TextBlock)target;
			break;
		case 7:
			mMuteCheckBox = (CustomCheckbox)target;
			((ToggleButton)mMuteCheckBox).Checked += new RoutedEventHandler(MuteButton_Checked);
			((ToggleButton)mMuteCheckBox).Unchecked += new RoutedEventHandler(MuteButton_Unchecked);
			((ToggleButton)mMuteCheckBox).Indeterminate += new RoutedEventHandler(MuteCheckBox_Indeterminate);
			break;
		case 8:
			mAutoHideCheckBox = (CustomCheckbox)target;
			((ToggleButton)mAutoHideCheckBox).Checked += new RoutedEventHandler(AutoHideButton_Checked);
			((ToggleButton)mAutoHideCheckBox).Unchecked += new RoutedEventHandler(AutoHideButton_Unchecked);
			((ToggleButton)mAutoHideCheckBox).Indeterminate += new RoutedEventHandler(AutoHideCheckBox_Indeterminate);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
