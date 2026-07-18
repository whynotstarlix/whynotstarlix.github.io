using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class AdvancedSettingsItemPanel : UserControl, IComponentConnector
{
	private EventHandler mTap;

	private EventHandler mMouseDragStart;

	private KeyActionType mActionType;

	private Point? mousePressedPosition;

	private Point? mouseReleasedPosition;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mDragImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mActionHeader;

	private bool _contentLoaded;

	public EventHandler Tap
	{
		get
		{
			return mTap;
		}
		set
		{
			mTap = value;
		}
	}

	public EventHandler MouseDragStart
	{
		get
		{
			return mMouseDragStart;
		}
		set
		{
			mMouseDragStart = value;
		}
	}

	public KeyActionType ActionType
	{
		get
		{
			return mActionType;
		}
		set
		{
			mActionType = value;
			mImage.ImageName = mActionType.ToString() + "_sidebar";
			BlueStacksUIBinding.Bind(mActionHeader, Constants.ImapLocaleStringsConstant + mActionType.ToString() + "_Header_Edit_UI", "");
		}
	}

	public AdvancedSettingsItemPanel()
	{
		InitializeComponent();
	}

	private void Image_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		((FrameworkElement)this).Cursor = Cursors.Hand;
		((UIElement)mDragImage).Visibility = (Visibility)0;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BorderBrushProperty, "AdvancedGameControlHeaderBackgroundColor");
		((UIElement)mBorder).Effect = (Effect)new DropShadowEffect
		{
			Direction = 270.0,
			ShadowDepth = 3.0,
			BlurRadius = 12.0,
			Opacity = 0.75,
			Color = ((SolidColorBrush)mBorder.Background).Color
		};
	}

	private void Image_MouseLeave(object sender, MouseEventArgs e)
	{
		if (KMManager.sDragCanvasElement == null)
		{
			((FrameworkElement)this).Cursor = Cursors.Arrow;
		}
		((UIElement)mDragImage).Visibility = (Visibility)1;
		BlueStacksUIBinding.BindColor((DependencyObject)(object)mBorder, Control.BorderBrushProperty, "AdvancedSettingsItemPanelBorder");
		((UIElement)mBorder).Effect = null;
	}

	private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		mousePressedPosition = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
	}

	private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		mouseReleasedPosition = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
		if (mousePressedPosition.Equals(mouseReleasedPosition))
		{
			Tap?.Invoke(this, null);
		}
		else
		{
			KMManager.ClearElement();
		}
		ReatchedMouseMove();
	}

	private void OnTimedElapsed(object sender, ElapsedEventArgs e)
	{
		if (!mousePressedPosition.Equals(mouseReleasedPosition))
		{
			((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				MouseDragStart?.Invoke(this, null);
			}, new object[0]);
		}
	}

	private void Image_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		if (mousePressedPosition.HasValue && !mousePressedPosition.Equals(mouseReleasedPosition))
		{
			((UIElement)this).MouseMove -= new MouseEventHandler(Image_MouseMove);
			MouseDragStart?.Invoke(this, null);
		}
	}

	public void ReatchedMouseMove()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		mousePressedPosition = null;
		((UIElement)this).MouseMove -= new MouseEventHandler(Image_MouseMove);
		((UIElement)this).MouseMove += new MouseEventHandler(Image_MouseMove);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/advancedsettingsitempanel.xaml", UriKind.Relative);
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((UIElement)(AdvancedSettingsItemPanel)target).MouseEnter += new MouseEventHandler(Image_MouseEnter);
			((UIElement)(AdvancedSettingsItemPanel)target).MouseLeave += new MouseEventHandler(Image_MouseLeave);
			((UIElement)(AdvancedSettingsItemPanel)target).PreviewMouseDown += new MouseButtonEventHandler(Image_PreviewMouseDown);
			((UIElement)(AdvancedSettingsItemPanel)target).MouseMove += new MouseEventHandler(Image_MouseMove);
			((UIElement)(AdvancedSettingsItemPanel)target).PreviewMouseUp += new MouseButtonEventHandler(Image_PreviewMouseUp);
			break;
		case 2:
			mBorder = (Border)target;
			break;
		case 3:
			mDragImage = (CustomPictureBox)target;
			break;
		case 4:
			mImage = (CustomPictureBox)target;
			break;
		case 5:
			mActionHeader = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
