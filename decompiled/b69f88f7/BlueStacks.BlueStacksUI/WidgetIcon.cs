using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class WidgetIcon : Button, IComponentConnector, IStyleConnector
{
	private CustomPictureBox mImage;

	private CustomPictureBox mBusyImage;

	private string mBusyImageNamePostFix = "_busy";

	private Storyboard mBusyIconStoryBoard;

	private string mImageName;

	public static readonly DependencyProperty MyFooterTextProperty = DependencyProperty.Register("FooterText", typeof(string), typeof(WidgetIcon));

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal WidgetIcon mWidgetIcon;

	private bool _contentLoaded;

	public string ImageName
	{
		get
		{
			return mImageName;
		}
		set
		{
			mImageName = value;
			if (mImage != null)
			{
				mImage.ImageName = mImageName;
			}
			if (mBusyImage != null)
			{
				mBusyImage.ImageName = mImageName + mBusyImageNamePostFix;
			}
		}
	}

	public string FooterText
	{
		get
		{
			return ((DependencyObject)this).GetValue(MyFooterTextProperty) as string;
		}
		set
		{
			((DependencyObject)this).SetValue(MyFooterTextProperty, (object)value);
		}
	}

	public WidgetIcon()
	{
		InitializeComponent();
	}

	internal void ShowBusyIcon(Visibility visibility)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((UIElement)mBusyImage).Visibility = visibility;
	}

	private void Image_Initialized(object sender, EventArgs e)
	{
		if (mImage == null)
		{
			mImage = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		}
		if (!string.IsNullOrEmpty(mImageName))
		{
			mImage.ImageName = mImageName;
		}
	}

	private void BusyImage_Initialized(object sender, EventArgs e)
	{
		if (mBusyImage == null)
		{
			mBusyImage = (CustomPictureBox)((sender is CustomPictureBox) ? sender : null);
		}
		if (!string.IsNullOrEmpty(mImageName))
		{
			mBusyImage.ImageName = mImageName + mBusyImageNamePostFix;
		}
	}

	private void CustomPictureBox_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		if (((UIElement)mBusyImage).IsVisible)
		{
			if (mBusyIconStoryBoard == null)
			{
				mBusyIconStoryBoard = new Storyboard();
				DoubleAnimation val = new DoubleAnimation
				{
					From = 0.0,
					To = 360.0,
					RepeatBehavior = RepeatBehavior.Forever,
					Duration = new Duration(new TimeSpan(0, 0, 1))
				};
				Storyboard.SetTarget((DependencyObject)(object)val, (DependencyObject)(object)mBusyImage);
				Storyboard.SetTargetProperty((DependencyObject)(object)val, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)", new object[0]));
				((TimelineGroup)mBusyIconStoryBoard).Children.Add((Timeline)(object)val);
			}
			mBusyIconStoryBoard.Begin();
		}
		else
		{
			mBusyIconStoryBoard.Pause();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/widgeticon.xaml", UriKind.Relative);
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
		if (connectionId == 1)
		{
			mWidgetIcon = (WidgetIcon)target;
		}
		else
		{
			_contentLoaded = true;
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
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		switch (connectionId)
		{
		case 2:
			((FrameworkElement)(CustomPictureBox)target).Initialized += Image_Initialized;
			break;
		case 3:
			((FrameworkElement)(CustomPictureBox)target).Initialized += BusyImage_Initialized;
			((UIElement)(CustomPictureBox)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(CustomPictureBox_IsVisibleChanged);
			break;
		}
	}
}
