using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class KeymapCanvasWindow : CustomWindow, IComponentConnector
{
	private int mCurrentTapElementDisplayRow;

	private int mCurrentTapElementDisplayCol;

	private int mTapYMargin = 5;

	private int mTapXMargin = 5;

	private int mMaxElementPerRow = 10;

	internal bool mIsShowWindow = true;

	private bool isNewElementAdded;

	private int mSidebarWidth = 260;

	internal AdvancedGameControlWindow SidebarWindow;

	private Point startPoint = new Point(-1.0, -1.0);

	private int zIndex;

	internal CanvasElement mCanvasElement;

	internal double mParentWindowHeight;

	internal double mParentWindowWidth;

	internal double mParentWindowTop;

	internal double mParentWindowLeft;

	internal Dictionary<IMAction, CanvasElement> dictCanvasElement = new Dictionary<IMAction, CanvasElement>();

	internal static bool IsDirty;

	internal static bool sWasMaximized;

	internal bool mIsExtraSettingsPopupOpened;

	private int mOldControlSchemeHashCode;

	private bool mIsInOverlayMode;

	internal bool mIsClosing;

	internal double SidebarWindowLeft = -1.0;

	internal double SidebarWindowTop = -1.0;

	internal double CanvasWindowLeft = -1.0;

	internal double CanvasWindowTop = -1.0;

	private Point mMousePointForNewTap;

	private IntPtr Handle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mCanvas;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCanvasImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Canvas mCanvas2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal OnboardingOverlayControl mOnboardingControl;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; }

	public static bool sIsDirty
	{
		get
		{
			return IsDirty;
		}
		set
		{
			IsDirty = value;
			if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
			{
				((UIElement)KMManager.CanvasWindow.SidebarWindow.mUndoBtn).IsEnabled = IsDirty;
				((UIElement)KMManager.CanvasWindow.SidebarWindow.mSaveBtn).IsEnabled = IsDirty;
			}
		}
	}

	internal bool IsInOverlayMode
	{
		get
		{
			return mIsInOverlayMode;
		}
		set
		{
			mIsInOverlayMode = value;
			((CustomWindow)this).IsShowGLWindow = value;
		}
	}

	internal KeymapCanvasWindow(MainWindow window)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow = window;
		InitializeComponent();
		mParentWindowHeight = ((FrameworkElement)ParentWindow).ActualHeight * MainWindow.sScalingFactor;
		mParentWindowWidth = ((FrameworkElement)ParentWindow).ActualWidth * MainWindow.sScalingFactor;
		mParentWindowTop = ((Window)ParentWindow).Top * MainWindow.sScalingFactor;
		mParentWindowLeft = ((Window)ParentWindow).Left * MainWindow.sScalingFactor;
	}

	internal void ClearWindow()
	{
		dictCanvasElement.Clear();
		KMManager.listCanvasElement.Clear();
		CanvasElement.dictPoints.Clear();
		((Panel)mCanvas).Children.Clear();
	}

	private void Canvas_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (KMManager.IsDragging)
		{
			isNewElementAdded = true;
			sIsDirty = true;
			List<IMAction> lstAction = KMManager.ClearElement();
			AddNewCanvasElement(lstAction);
			StartMoving(mCanvasElement, e.GetPosition((IInputElement)(object)this));
		}
	}

	public void AddNewCanvasElement(List<IMAction> lstAction, bool isTap = false)
	{
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		if (lstAction == null)
		{
			return;
		}
		mCanvasElement = new CanvasElement(this, ParentWindow);
		double value = (((FrameworkElement)this).ActualWidth - ((FrameworkElement)mCanvasElement).ActualWidth) * 100.0 / (((FrameworkElement)this).ActualWidth * 3.0) + (double)(mTapXMargin * mCurrentTapElementDisplayCol);
		double value2 = (((FrameworkElement)this).ActualHeight - ((FrameworkElement)mCanvasElement).ActualHeight) * 100.0 / (((FrameworkElement)this).ActualHeight * 3.0) + (double)(mTapYMargin * mCurrentTapElementDisplayRow);
		foreach (IMAction item in lstAction)
		{
			if (isTap)
			{
				item.PositionX = Math.Round(value, 2);
				item.PositionY = Math.Round(value2, 2);
			}
			mCanvasElement.AddAction(item);
			dictCanvasElement.Add(item, mCanvasElement);
			if (isTap && lstAction.First().Type != KeyActionType.EdgeScroll)
			{
				mCanvasElement.ShowTextBox(((Tuple<string, TextBox, TextBlock>)(object)mCanvasElement.dictTextElemets.First().Value).Item3);
			}
			if (!item.IsChildAction && item.Type != KeyActionType.MOBADpad)
			{
				ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(item);
			}
		}
		mCurrentTapElementDisplayCol++;
		if (mCurrentTapElementDisplayCol == mMaxElementPerRow)
		{
			mCurrentTapElementDisplayCol = 0;
			mCurrentTapElementDisplayRow++;
		}
		((UIElement)mCanvasElement).MouseLeftButtonDown += new MouseButtonEventHandler(MoveIcon_PreviewMouseDown);
		((UIElement)mCanvasElement.mResizeIcon).PreviewMouseDown += new MouseButtonEventHandler(ResizeIcon_PreviewMouseDown);
		((Panel)mCanvas).Children.Add((UIElement)(object)mCanvasElement);
	}

	private void ResizeIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		KMManager.CheckAndCreateNewScheme();
		mCanvasElement = WpfUtils.FindVisualParent<CanvasElement>((DependencyObject)((sender is DependencyObject) ? sender : null));
		((UIElement)mCanvasElement.mResizeIcon).Focus();
		startPoint = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
		((UIElement)mCanvas).PreviewMouseMove += new MouseEventHandler(CanvasResizeExistingElement_MouseMove);
		sIsDirty = true;
		((RoutedEventArgs)e).Handled = true;
		Mouse.Capture((IInputElement)(object)mCanvas);
	}

	private void CanvasResizeExistingElement_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).Cursor = Cursors.SizeNWSE;
		Point position = e.GetPosition((IInputElement)(object)this);
		double num = ((Point)(ref position)).X - ((Point)(ref startPoint)).X;
		double num2 = ((Point)(ref position)).Y - ((Point)(ref startPoint)).Y;
		double value = num2;
		if (Math.Abs(num) > Math.Abs(num2))
		{
			value = num;
		}
		value = Math.Round(value, 2);
		double num3 = ((FrameworkElement)mCanvasElement).ActualWidth + value;
		if (num3 < 40.0)
		{
			num3 = 40.0;
			value = num3 - ((FrameworkElement)mCanvasElement).ActualWidth;
		}
		if (num3 < 70.0)
		{
			double num4 = ((FrameworkElement)mCanvasElement).ActualHeight - 20.0;
			((FrameworkElement)mCanvasElement.mSkillImage).Margin = new Thickness(-50.0, num4, 10.0, 0.0);
		}
		if ((int)((UIElement)mCanvasElement.mSkillImage).Visibility == 0)
		{
			((UIElement)mCanvasElement.mActionIcon).Visibility = (Visibility)0;
		}
		double num5 = Canvas.GetTop((UIElement)(object)mCanvasElement);
		double num6 = Canvas.GetLeft((UIElement)(object)mCanvasElement);
		if (double.IsNaN(num5))
		{
			num5 = 0.0;
		}
		if (double.IsNaN(num6))
		{
			num6 = 0.0;
		}
		num5 -= value / 2.0;
		num6 -= value / 2.0;
		((FrameworkElement)mCanvasElement).Width = num3;
		((FrameworkElement)mCanvasElement).Height = num3;
		Canvas.SetLeft((UIElement)(object)mCanvasElement, num6);
		Canvas.SetTop((UIElement)(object)mCanvasElement, num5);
		mCanvasElement.UpdatePosition(num5, num6);
		startPoint = position;
	}

	internal void ReloadCanvasWindow()
	{
		mCurrentTapElementDisplayRow = 0;
		mCurrentTapElementDisplayCol = 0;
		if (ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null)
		{
			KMManager.LoadIMActions(ParentWindow, ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
		}
		((Panel)mCanvas).Children.Clear();
		Init();
	}

	private void MoveIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		CanvasElement canvasElement = sender as CanvasElement;
		if ((canvasElement.MOBASkillSettingsPopup != null && ((Popup)canvasElement.MOBASkillSettingsPopup).IsOpen) || ((Popup)canvasElement.MOBAOtherSettingsMoreInfoPopup).IsOpen || ((Popup)canvasElement.MOBASkillSettingsMoreInfoPopup).IsOpen)
		{
			((RoutedEventArgs)e).Handled = true;
			return;
		}
		canvasElement.TopOnClick = Canvas.GetTop((UIElement)(object)canvasElement);
		canvasElement.LeftOnClick = Canvas.GetLeft((UIElement)(object)canvasElement);
		Point position = ((MouseEventArgs)e).GetPosition((IInputElement)(object)this);
		if (((UIElement)canvasElement.mResizeIcon).IsMouseOver || ((UIElement)canvasElement.mCloseIcon).IsMouseOver)
		{
			return;
		}
		if (Keyboard.IsKeyDown((Key)118) || Keyboard.IsKeyDown((Key)119))
		{
			if (canvasElement.ListActionItem.First().Type == KeyActionType.Swipe)
			{
				bool isNewScheme = true;
				foreach (IMAction item in canvasElement.ListActionItem)
				{
					CreateGameControlCopy(item, ((MouseEventArgs)e).GetPosition((IInputElement)(object)this), isNewScheme);
					isNewScheme = false;
				}
			}
			else if (!canvasElement.ListActionItem.First().IsChildAction)
			{
				CreateGameControlCopy(canvasElement.ListActionItem.First(), ((MouseEventArgs)e).GetPosition((IInputElement)(object)this));
			}
		}
		else
		{
			StartMoving(canvasElement, position);
		}
		((RoutedEventArgs)e).Handled = true;
	}

	private void CreateGameControlCopy(IMAction originalAction, Point point, bool isNewScheme = true)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		IMAction iMAction = UsefulExtensionMethod.DeepCopy<IMAction>(originalAction);
		iMAction.PositionX = originalAction.PositionX + 1.0;
		List<CanvasElement> source = AddCanvasElementsForAction(iMAction);
		if (isNewScheme)
		{
			KMManager.CheckAndCreateNewScheme();
		}
		ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(iMAction);
		StartMoving(source.First(), point);
	}

	private void CanvasMoveExistingElement_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		KMManager.CheckAndCreateNewScheme();
		((UIElement)this).Focus();
		MoveElement(e.GetPosition((IInputElement)(object)this));
	}

	internal void StartMoving(CanvasElement element, Point p)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		if (mCanvasElement == null || mCanvasElement == element)
		{
			if (!((UIElement)element.mSkillImage).IsMouseOver)
			{
				sIsDirty = true;
			}
			mCanvasElement = element;
			startPoint = p;
			((UIElement)mCanvas).PreviewMouseMove -= new MouseEventHandler(CanvasMoveExistingElement_MouseMove);
			((UIElement)mCanvas).PreviewMouseMove += new MouseEventHandler(CanvasMoveExistingElement_MouseMove);
		}
	}

	internal void MoveElement(Point p1)
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (((FrameworkElement)mCanvasElement).IsLoaded)
		{
			((FrameworkElement)this).Cursor = Cursors.Hand;
			double num = Canvas.GetTop((UIElement)(object)mCanvasElement);
			double num2 = Canvas.GetLeft((UIElement)(object)mCanvasElement);
			if (double.IsNaN(num))
			{
				num = 0.0;
			}
			if (double.IsNaN(num2))
			{
				num2 = 0.0;
			}
			double num3 = num2 + ((FrameworkElement)mCanvasElement).ActualWidth / 2.0;
			double num4 = num + ((FrameworkElement)mCanvasElement).ActualHeight / 2.0;
			num3 += ((Point)(ref p1)).X - ((Point)(ref startPoint)).X;
			num4 += ((Point)(ref p1)).Y - ((Point)(ref startPoint)).Y;
			num3 = ((num3 < 0.0) ? 0.0 : num3);
			num4 = ((num4 < 0.0) ? 0.0 : num4);
			num3 = ((num3 > ((FrameworkElement)mCanvas).ActualWidth) ? ((FrameworkElement)mCanvas).ActualWidth : num3);
			num4 = ((num4 > ((FrameworkElement)mCanvas).ActualHeight) ? ((FrameworkElement)mCanvas).ActualHeight : num4);
			num2 = num3 - ((FrameworkElement)mCanvasElement).ActualWidth / 2.0;
			num = num4 - ((FrameworkElement)mCanvasElement).ActualHeight / 2.0;
			Canvas.SetLeft((UIElement)(object)mCanvasElement, num2);
			Canvas.SetTop((UIElement)(object)mCanvasElement, num);
			mCanvasElement.UpdatePosition(num, num2);
			startPoint = p1;
		}
	}

	internal void Init()
	{
		if (ParentWindow.SelectedConfig?.SelectedControlScheme == null)
		{
			return;
		}
		int value = (ParentWindow.SelectedConfig?.SelectedControlScheme.GetHashCode()).Value;
		if (mOldControlSchemeHashCode == value)
		{
			return;
		}
		mOldControlSchemeHashCode = value;
		ClearWindow();
		IMConfig selectedConfig = ParentWindow.SelectedConfig;
		if (selectedConfig == null || !(selectedConfig.SelectedControlScheme?.GameControls.Count > 0))
		{
			return;
		}
		foreach (IMAction gameControl in ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
		{
			if (!IsInOverlayMode || gameControl.IsVisibleInOverlay)
			{
				AddCanvasElementsForAction(gameControl, isLoadingFromFile: true);
			}
			else
			{
				if (gameControl.IsVisibleInOverlay)
				{
					continue;
				}
				List<CanvasElement> canvasElement = CanvasElement.GetCanvasElement(gameControl, this, ParentWindow);
				foreach (CanvasElement item in canvasElement)
				{
					((UIElement)item).Visibility = (Visibility)1;
				}
				KMManager.listCanvasElement.Add(canvasElement);
			}
		}
	}

	internal void InitLayout()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		MainWindow parentWindow = ParentWindow;
		Grid mFrontendGrid = parentWindow.mFrontendGrid;
		Point val = ((UIElement)mFrontendGrid).TranslatePoint(default(Point), (UIElement)(object)parentWindow);
		if (IsInOverlayMode)
		{
			((Control)this).Background = (Brush)(object)Brushes.Transparent;
			((Panel)mCanvas).Background = (Brush)(object)Brushes.Transparent;
			((Panel)mCanvas2).Background = (Brush)(object)Brushes.Transparent;
			((FrameworkElement)mCanvas).Margin = default(Thickness);
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				((UIElement)this).Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
			}
			else
			{
				string path = Path.Combine(RegistryManager.Instance.ClientInstallDir, "ImapImages");
				string path2 = ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName + ".png";
				string text = Path.Combine(path, path2);
				if (File.Exists(text))
				{
					mCanvasImage.ImageName = text;
					((UIElement)mCanvas).Opacity = 0.0;
				}
				else
				{
					((UIElement)mCanvas).Opacity = 1.0;
				}
			}
			Handle = WindowInteropHelperExtensions.EnsureHandle(new WindowInteropHelper((Window)(object)this));
			int num = 1207959552;
			InteropWindow.SetWindowLong(Handle, -16, num);
			return;
		}
		IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(ParentWindow.Handle, isWorkAreaRequired: true);
		double num2 = (double)fullscreenMonitorSize.Width - (double)mSidebarWidth * MainWindow.sScalingFactor;
		if (!ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
		{
			num2 -= ((FrameworkElement)ParentWindow.mSidebar).Width * MainWindow.sScalingFactor;
		}
		double num3 = ParentWindow.GetHeightFromWidth(num2, isScaled: true);
		if (num3 > (double)fullscreenMonitorSize.Height)
		{
			num3 = fullscreenMonitorSize.Height;
			num2 = ParentWindow.GetWidthFromHeight(num3, isScaled: true);
		}
		double top = ((!(((Window)ParentWindow).Top * MainWindow.sScalingFactor + num3 > (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height))) ? (((Window)ParentWindow).Top * MainWindow.sScalingFactor) : ((double)fullscreenMonitorSize.Y + ((double)fullscreenMonitorSize.Height - num3) / 2.0));
		double left = ((!(((Window)ParentWindow).Left * MainWindow.sScalingFactor + num2 + (double)mSidebarWidth * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width))) ? (((Window)ParentWindow).Left * MainWindow.sScalingFactor) : ((double)fullscreenMonitorSize.X + ((double)fullscreenMonitorSize.Width - num2 - (double)mSidebarWidth * MainWindow.sScalingFactor) / 2.0));
		ParentWindow.ChangeHeightWidthTopLeft(num2, num3, top, left);
		((FrameworkElement)this).Width = ((FrameworkElement)ParentWindow).ActualWidth;
		((FrameworkElement)this).Height = ((FrameworkElement)ParentWindow).ActualHeight;
		((Window)this).Top = ((Window)ParentWindow).Top;
		((Window)this).Left = ((Window)ParentWindow).Left;
		Point val2 = default(Point);
		((Point)(ref val2))._002Ector(((FrameworkElement)parentWindow).ActualWidth - (((FrameworkElement)mFrontendGrid).ActualWidth + ((Point)(ref val)).X), ((FrameworkElement)parentWindow).ActualHeight - (((FrameworkElement)mFrontendGrid).ActualHeight + ((Point)(ref val)).Y));
		((FrameworkElement)mCanvas).Margin = new Thickness(((Point)(ref val)).X, ((Point)(ref val)).Y, ((Point)(ref val2)).X, ((Point)(ref val2)).Y);
		((FrameworkElement)mCanvas).Width = ((FrameworkElement)mFrontendGrid).ActualWidth;
		((FrameworkElement)mCanvas).Height = ((FrameworkElement)mFrontendGrid).ActualHeight;
	}

	private List<CanvasElement> AddCanvasElementsForAction(IMAction item, bool isLoadingFromFile = false)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		List<CanvasElement> canvasElement = CanvasElement.GetCanvasElement(item, this, ParentWindow);
		foreach (CanvasElement item2 in canvasElement)
		{
			item2.mIsLoadingfromFile = isLoadingFromFile;
			foreach (IMAction item3 in item2.ListActionItem)
			{
				dictCanvasElement[item3] = item2;
			}
			if (((FrameworkElement)item2).Parent == null)
			{
				((Panel)mCanvas).Children.Add((UIElement)(object)item2);
				((UIElement)item2).MouseLeftButtonDown -= new MouseButtonEventHandler(MoveIcon_PreviewMouseDown);
				((UIElement)item2.mResizeIcon).PreviewMouseDown -= new MouseButtonEventHandler(ResizeIcon_PreviewMouseDown);
				((UIElement)item2).MouseLeftButtonDown += new MouseButtonEventHandler(MoveIcon_PreviewMouseDown);
				((UIElement)item2.mResizeIcon).PreviewMouseDown += new MouseButtonEventHandler(ResizeIcon_PreviewMouseDown);
			}
			if (SidebarWindow == null)
			{
				((UIElement)item2).Visibility = (Visibility)1;
			}
		}
		KMManager.listCanvasElement.Add(canvasElement);
		return canvasElement;
	}

	private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		((FrameworkElement)this).Cursor = Cursors.Arrow;
		if (mCanvasElement != null)
		{
			Panel.SetZIndex((UIElement)(object)mCanvasElement, zIndex++);
			mCanvasElement.ShowOtherIcons();
			if (((UIElement)mCanvasElement).IsMouseDirectlyOver)
			{
				((RoutedEventArgs)e).Handled = true;
			}
			if (isNewElementAdded && mCanvasElement.dictTextElemets.Count > 0)
			{
				isNewElementAdded = false;
				mCanvasElement.ShowTextBox(((Tuple<string, TextBox, TextBlock>)(object)mCanvasElement.dictTextElemets.First().Value).Item3);
			}
		}
		startPoint = new Point(-1.0, -1.0);
		((UIElement)mCanvas).PreviewMouseMove -= new MouseEventHandler(CanvasMoveExistingElement_MouseMove);
		((UIElement)mCanvas).PreviewMouseMove -= new MouseEventHandler(CanvasResizeExistingElement_MouseMove);
		Mouse.Capture((IInputElement)null);
		mCanvasElement = null;
	}

	private void KeymapCanvasWindow_Closing(object sender, CancelEventArgs e)
	{
		mIsClosing = true;
		((UIElement)ParentWindow).Focus();
	}

	private void CustomWindow_Closed(object sender, EventArgs e)
	{
		if (KMManager.dictOverlayWindow.ContainsKey(ParentWindow) && KMManager.dictOverlayWindow[ParentWindow] == this)
		{
			KMManager.dictOverlayWindow.Remove(ParentWindow);
		}
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		InteropWindow.RemoveWindowFromAltTabUI(new WindowInteropHelper((Window)(object)this).Handle);
		if (!IsInOverlayMode)
		{
			ShowSideBarWindow();
		}
		else
		{
			Handle = new WindowInteropHelper((Window)(object)this).Handle;
			int num = 1207959552;
			InteropWindow.SetWindowLong(Handle, -16, num);
			ParentWindow.mFrontendHandler.UpdateOverlaySizeStatus();
			((Window)ParentWindow).LocationChanged += ParentWindow_LocationChanged;
			((Window)ParentWindow).Activated += ParentWindow_Activated;
			((Window)ParentWindow).Deactivated += ParentWindow_Deactivated;
			UpdateSize();
		}
		Init();
	}

	private void ParentWindow_Deactivated(object sender, EventArgs e)
	{
		if (!mIsClosing && (KMManager.sGuidanceWindow == null || !((Window)KMManager.sGuidanceWindow).IsActive || KMManager.sGuidanceWindow.ParentWindow != ParentWindow))
		{
			((Window)this).Hide();
		}
	}

	private void ParentWindow_Activated(object sender, EventArgs e)
	{
		if (!mIsClosing)
		{
			((Window)this).Show();
		}
	}

	private void ParentWindow_LocationChanged(object sender, EventArgs e)
	{
		UpdateSize();
	}

	internal void UpdateSize()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (ParentWindow.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero && !mIsClosing)
		{
			if (mIsShowWindow)
			{
				mIsShowWindow = false;
				Logger.Debug("KMP KeymapCanvasWindow UpdateSize");
				ParentWindow.mFrontendHandler.DeactivateFrontend();
				((Window)this).Show();
				return;
			}
			RECT val = default(RECT);
			InteropWindow.GetWindowRect(ParentWindow.StaticComponents.mLastMappableWindowHandle, ref val);
			int left = ((RECT)(ref val)).Left;
			int top = ((RECT)(ref val)).Top;
			int num = ((RECT)(ref val)).Right - ((RECT)(ref val)).Left;
			int num2 = ((RECT)(ref val)).Bottom - ((RECT)(ref val)).Top;
			InteropWindow.SetWindowPos(Handle, (IntPtr)0, left, top, num, num2, 16448u);
			ParentWindow.mFrontendHandler.FocusFrontend();
			SetOnboardingControlPosition(0.0, 0.0);
		}
	}

	private Point GetCorrectCoordinateLocationForAndroid(Point p)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		double num = ((Point)(ref p)).X * 100.0 / ((FrameworkElement)ParentWindow).Width;
		double num2 = ((Point)(ref p)).Y * 100.0 / ((FrameworkElement)ParentWindow).Height;
		return new Point(num, num2);
	}

	private void ShowSideBarWindow()
	{
		if (SidebarWindow == null)
		{
			SidebarWindow = new AdvancedGameControlWindow(ParentWindow);
			SidebarWindow.Init(this);
			ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
			((Window)SidebarWindow).Owner = (Window)(object)this;
			((Window)SidebarWindow).Show();
			((Window)SidebarWindow).Activate();
		}
	}

	private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (CanvasElement.sFocusedTextBox != null)
		{
			object sFocusedTextBox = CanvasElement.sFocusedTextBox;
			WpfUtils.FindVisualParent<CanvasElement>((DependencyObject)((sFocusedTextBox is DependencyObject) ? sFocusedTextBox : null)).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
			return;
		}
		if (double.IsNaN(CanvasWindowLeft) && double.IsNaN(CanvasWindowTop))
		{
			CanvasWindowLeft = ((Window)this).Left;
			CanvasWindowTop = ((Window)this).Top;
			mMousePointForNewTap = Mouse.GetPosition((IInputElement)(object)mCanvas);
		}
		sIsDirty = true;
		try
		{
			((Window)this).DragMove();
		}
		catch (Exception)
		{
		}
		if (Math.Abs(CanvasWindowLeft - ((Window)this).Left) < 2.0 && Math.Abs(CanvasWindowTop - ((Window)this).Top) < 2.0)
		{
			if (KMManager.sIsInScriptEditingMode && mIsExtraSettingsPopupOpened)
			{
				return;
			}
			IMAction item = new Tap
			{
				Type = KeyActionType.Tap
			};
			if (ParentWindow.SelectedConfig.ControlSchemes.Count == 0 && CanvasElement.sFocusedTextBox != null)
			{
				object sFocusedTextBox2 = CanvasElement.sFocusedTextBox;
				WpfUtils.FindVisualParent<CanvasElement>((DependencyObject)((sFocusedTextBox2 is DependencyObject) ? sFocusedTextBox2 : null)).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
			}
			else
			{
				if (ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
				{
					KMManager.AddNewControlSchemeAndSelect(ParentWindow);
				}
				else if (ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
				{
					KMManager.CheckAndCreateNewScheme();
				}
				ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(item);
				List<CanvasElement> source = AddCanvasElementsForAction(item);
				source.First().SetMousePoint(mMousePointForNewTap);
				source.First().IsRemoveIfEmpty = true;
				source.First().ShowTextBox(((Tuple<string, TextBox, TextBlock>)(object)source.First().dictTextElemets.First().Value).Item3);
			}
		}
		CanvasWindowLeft = double.NaN;
		CanvasWindowTop = double.NaN;
	}

	private void CustomWindow_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mMousePointForNewTap = Mouse.GetPosition((IInputElement)(object)mCanvas);
		CanvasWindowLeft = ((Window)this).Left;
		CanvasWindowTop = ((Window)this).Top;
		try
		{
			((Window)this).DragMove();
		}
		catch (Exception)
		{
		}
	}

	private void CustomWindow_LocationChanged(object sender, EventArgs e)
	{
		if (!IsInOverlayMode)
		{
			((Window)ParentWindow).Top = ((Window)this).Top;
			((Window)ParentWindow).Left = ((Window)this).Left;
		}
	}

	private void CustomWindow_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		foreach (object child in ((Panel)mCanvas).Children)
		{
			if (IsInOverlayMode)
			{
				(child as CanvasElement).SetElementLayout(isLoaded: true, (child as CanvasElement).mXPosition, (child as CanvasElement).mYPosition);
				if ((child as CanvasElement).ListActionItem.First().Type == KeyActionType.Callback)
				{
					SetOnboardingControlPosition((child as CanvasElement).mXPosition, (child as CanvasElement).mYPosition);
				}
			}
			else
			{
				(child as CanvasElement).SetElementLayout();
			}
		}
	}

	private void MCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ToolTip val = new ToolTip();
		if (KMManager.sIsInScriptEditingMode)
		{
			Point correctCoordinateLocationForAndroid = GetCorrectCoordinateLocationForAndroid(Mouse.GetPosition((IInputElement)(object)mCanvas));
			if (val.IsOpen)
			{
				val.IsOpen = false;
			}
			((ContentControl)val).Content = $" X: {((Point)(ref correctCoordinateLocationForAndroid)).X} Y: {((Point)(ref correctCoordinateLocationForAndroid)).Y}";
			val.StaysOpen = true;
			val.Placement = (PlacementMode)7;
			val.IsOpen = true;
		}
	}

	internal void ShowOnboardingOverlayControl(double left, double top, bool isVisible = true)
	{
		if (!isVisible || !File.Exists(Path.Combine(CustomPictureBox.AssetsDir, "onboarding_step_" + KMManager.mOnboardingCounter.ToString(CultureInfo.InvariantCulture) + ".png")) || PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages?.IsPackageAvailable(KMManager.sPackageName) != true)
		{
			((UIElement)mOnboardingControl).Visibility = (Visibility)2;
			((UIElement)mGrid).Visibility = (Visibility)2;
			return;
		}
		((UIElement)mCanvas2).Opacity = 1.0;
		mOnboardingControl.mOnboardingImg.ImageName = "onboarding_step_" + KMManager.mOnboardingCounter.ToString(CultureInfo.InvariantCulture);
		((UIElement)mOnboardingControl).Visibility = (Visibility)0;
		((UIElement)mOnboardingControl.mOnboardingImg).Visibility = (Visibility)0;
		((UIElement)mGrid).Visibility = (Visibility)0;
		SetOnboardingControlPosition(left, top);
	}

	internal void SetOnboardingControlPosition(double left, double top)
	{
		if (left == 0.0 || top == 0.0)
		{
			left = 62.8;
			top = 15.6;
		}
		double num = left / 100.0 * ((FrameworkElement)mCanvas).ActualWidth;
		double num2 = top / 100.0 * ((FrameworkElement)mCanvas).ActualHeight;
		num = ((num < 0.0) ? 0.0 : num);
		num2 = ((num2 < 0.0) ? 0.0 : num2);
		num = ((num > ((FrameworkElement)ParentWindow).ActualWidth) ? ((FrameworkElement)ParentWindow).ActualWidth : num);
		num2 = ((num2 > ((FrameworkElement)ParentWindow).ActualHeight) ? ((FrameworkElement)ParentWindow).ActualHeight : num2);
		double num3 = 310.0;
		double num4 = 85.0;
		((FrameworkElement)mOnboardingControl.mOnboardingImg).Height = num4 / 100.0 * ((FrameworkElement)mCanvas).ActualHeight * 0.2;
		((FrameworkElement)mOnboardingControl.mOnboardingImg).Width = num3 / 100.0 * ((FrameworkElement)mCanvas).ActualWidth * 0.1;
		left = num - ((FrameworkElement)mOnboardingControl.mOnboardingImg).Width / 2.0;
		top = num2 - ((FrameworkElement)mOnboardingControl.mOnboardingImg).Height / 2.0;
		Canvas.SetLeft((UIElement)(object)mOnboardingControl, left);
		Canvas.SetTop((UIElement)(object)mOnboardingControl, top);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/keymapcanvaswindow.xaml", UriKind.Relative);
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
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((Window)(KeymapCanvasWindow)target).Closing += KeymapCanvasWindow_Closing;
			((Window)(KeymapCanvasWindow)target).Closed += CustomWindow_Closed;
			((FrameworkElement)(KeymapCanvasWindow)target).Loaded += new RoutedEventHandler(Window_Loaded);
			((Window)(KeymapCanvasWindow)target).LocationChanged += CustomWindow_LocationChanged;
			((UIElement)(KeymapCanvasWindow)target).MouseLeftButtonDown += new MouseButtonEventHandler(CustomWindow_MouseDown);
			((FrameworkElement)(KeymapCanvasWindow)target).SizeChanged += new SizeChangedEventHandler(CustomWindow_SizeChanged);
			break;
		case 2:
			((UIElement)(Grid)target).MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseDown);
			break;
		case 3:
			mCanvas = (Canvas)target;
			((UIElement)mCanvas).MouseEnter += new MouseEventHandler(Canvas_MouseEnter);
			((UIElement)mCanvas).PreviewMouseUp += new MouseButtonEventHandler(Canvas_MouseUp);
			((UIElement)mCanvas).MouseDown += new MouseButtonEventHandler(CustomWindow_MouseDown);
			break;
		case 4:
			mCanvasImage = (CustomPictureBox)target;
			break;
		case 5:
			mGrid = (Grid)target;
			break;
		case 6:
			mCanvas2 = (Canvas)target;
			break;
		case 7:
			mOnboardingControl = (OnboardingOverlayControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
