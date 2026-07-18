using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class CustomPopUp : Popup
{
	public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register("IsTopmost", typeof(bool), typeof(CustomPopUp), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(OnIsTopmostChanged)));

	private bool? mAppliedTopMost;

	private bool mAlreadyLoaded;

	private Window mParentWindow;

	private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	private static readonly IntPtr HWND_TOP = new IntPtr(0);

	private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

	private const uint SWP_NOSIZE = 1u;

	private const uint SWP_NOMOVE = 2u;

	private const uint SWP_NOREDRAW = 8u;

	private const uint SWP_NOACTIVATE = 16u;

	private const uint SWP_NOOWNERZORDER = 512u;

	private const uint SWP_NOSENDCHANGING = 1024u;

	private const uint TOPMOST_FLAGS = 1563u;

	public bool IsFocusOnMouseClick { get; set; }

	public bool IsTopmost
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsTopmostProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsTopmostProperty, (object)value);
		}
	}

	private void CustomPopUp_Initialized(object sender, EventArgs e)
	{
		RenderHelper.ChangeRenderModeToSoftware(sender);
	}

	public CustomPopUp()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnPopupLoaded);
		((FrameworkElement)this).Unloaded += new RoutedEventHandler(OnPopupUnloaded);
		((Popup)this).Opened += CustomPopUp_Initialized;
		((UIElement)this).PreviewMouseDown += new MouseButtonEventHandler(CustomPopUp_PreviewMouseDown);
	}

	private void CustomPopUp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!IsFocusOnMouseClick)
		{
			return;
		}
		try
		{
			PresentationSource obj = PresentationSource.FromVisual((Visual)(object)((Popup)this).Child);
			HwndSource val = (HwndSource)(object)((obj is HwndSource) ? obj : null);
			if (val != null)
			{
				InteropWindow.SetForegroundWindow(val.Handle);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in setting popup as foreground window: {0}", new object[1] { ex });
		}
	}

	private void OnPopupLoaded(object sender, RoutedEventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		if (!mAlreadyLoaded)
		{
			mAlreadyLoaded = true;
			UIElement child = ((Popup)this).Child;
			if (child != null)
			{
				child.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);
			}
			mParentWindow = Window.GetWindow((DependencyObject)(object)this);
			if (mParentWindow != null)
			{
				mParentWindow.Activated += OnParentWindowActivated;
				mParentWindow.Deactivated += OnParentWindowDeactivated;
			}
		}
	}

	private void OnPopupUnloaded(object sender, RoutedEventArgs e)
	{
		if (mParentWindow != null)
		{
			mParentWindow.Activated -= OnParentWindowActivated;
			mParentWindow.Deactivated -= OnParentWindowDeactivated;
		}
	}

	private void OnParentWindowActivated(object sender, EventArgs e)
	{
		SetTopmostState(isTop: true);
	}

	private void OnParentWindowDeactivated(object sender, EventArgs e)
	{
		if (!IsTopmost)
		{
			SetTopmostState(IsTopmost);
		}
	}

	private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		Logger.Debug("Child Mouse Left Button Down");
		SetTopmostState(isTop: true);
		if (mParentWindow != null && !mParentWindow.IsActive && !IsTopmost)
		{
			mParentWindow.Activate();
			Logger.Debug("Activating Parent from child Left Button Down");
		}
	}

	private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs _)
	{
		CustomPopUp obj2 = (CustomPopUp)(object)obj;
		obj2.SetTopmostState(obj2.IsTopmost);
	}

	protected override void OnOpened(EventArgs e)
	{
		mParentWindow = Window.GetWindow((DependencyObject)(object)this);
		SetTopmostState(IsTopmost);
		((Popup)this).OnOpened(e);
	}

	private void SetTopmostState(bool isTop)
	{
		try
		{
			if (mParentWindow != null && !isTop && InteropWindow.GetTopmostOwnerWindow(mParentWindow).Topmost)
			{
				isTop = true;
			}
			if ((mAppliedTopMost.HasValue && mAppliedTopMost == isTop) || ((Popup)this).Child == null)
			{
				return;
			}
			PresentationSource obj = PresentationSource.FromVisual((Visual)(object)((Popup)this).Child);
			HwndSource val = (HwndSource)(object)((obj is HwndSource) ? obj : null);
			if (val == null)
			{
				return;
			}
			IntPtr handle = val.Handle;
			if (NativeMethods.GetWindowRect(handle, out var lpRect))
			{
				Logger.Debug("setting z-order " + isTop);
				if (isTop)
				{
					NativeMethods.SetWindowPos(handle, HWND_TOPMOST, ((RECT)(ref lpRect)).Left, ((RECT)(ref lpRect)).Top, (int)((FrameworkElement)this).Width, (int)((FrameworkElement)this).Height, 1563u);
				}
				else
				{
					NativeMethods.SetWindowPos(handle, HWND_BOTTOM, ((RECT)(ref lpRect)).Left, ((RECT)(ref lpRect)).Top, (int)((FrameworkElement)this).Width, (int)((FrameworkElement)this).Height, 1563u);
					NativeMethods.SetWindowPos(handle, HWND_TOP, ((RECT)(ref lpRect)).Left, ((RECT)(ref lpRect)).Top, (int)((FrameworkElement)this).Width, (int)((FrameworkElement)this).Height, 1563u);
					NativeMethods.SetWindowPos(handle, HWND_NOTOPMOST, ((RECT)(ref lpRect)).Left, ((RECT)(ref lpRect)).Top, (int)((FrameworkElement)this).Width, (int)((FrameworkElement)this).Height, 1563u);
				}
				mAppliedTopMost = isTop;
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in set topmost state in custom popup: {0}", new object[1] { ex });
		}
	}
}
