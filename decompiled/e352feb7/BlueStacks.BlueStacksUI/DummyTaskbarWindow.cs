using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using BlueStacks.Common;
using Vanara.PInvoke;

namespace BlueStacks.BlueStacksUI;

public class DummyTaskbarWindow : CustomWindow, IComponentConnector
{
	private const int WM_DWMSENDICONICTHUMBNAIL = 803;

	private static readonly object sync = new object();

	private static IntPtr sThisHandle = IntPtr.Zero;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal DummyTaskbarWindow mDummyTaskbarWindow;

	private bool _contentLoaded;

	public string TaskbarThumbnailPath { get; set; }

	public MainWindow ParentWindow { get; set; }

	public DummyTaskbarWindow(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
	}

	private void DummyTaskbarWindow_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		try
		{
			sThisHandle = new WindowInteropHelper((Window)(object)this).Handle;
			int num = Marshal.SizeOf((object)1);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.WriteInt32(intPtr, 0, 1);
			HWND val = new HWND(sThisHandle);
			DwmApi.DwmSetWindowAttribute(val, (DWMWINDOWATTRIBUTE)7, intPtr, num);
			DwmApi.DwmSetWindowAttribute(val, (DWMWINDOWATTRIBUTE)10, intPtr, num);
			Marshal.FreeHGlobal(intPtr);
			HwndSource.FromHwnd(sThisHandle).AddHook(new HwndSourceHook(WndProc));
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in setting window porperties for taskbar thumbnail : " + ex.ToString());
		}
	}

	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (msg == 803)
			{
				lock (sync)
				{
					int decodePixelWidth = (lParam.ToInt32() >> 16) & 0xFFFF;
					int decodePixelHeight = lParam.ToInt32() & 0xFFFF;
					BitmapImage val = new BitmapImage();
					val.BeginInit();
					val.UriSource = new Uri(TaskbarThumbnailPath);
					val.DecodePixelWidth = decodePixelWidth;
					val.DecodePixelHeight = decodePixelHeight;
					val.EndInit();
					Bitmap val2 = ImageUtils.BitmapImage2Bitmap(val);
					DwmApi.DwmSetIconicThumbnail(new HWND(sThisHandle), HBITMAP.op_Implicit(val2.GetHbitmap()), (DWM_SETICONICPREVIEW_Flags)0);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("Exception in setting taskbar thumbnail : " + ex.ToString());
		}
		return IntPtr.Zero;
	}

	private void DummyTaskbarWindow_Closing(object sender, CancelEventArgs e)
	{
		ParentWindow.DummyWindow = null;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/dummytaskbarwindow.xaml", UriKind.Relative);
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (connectionId == 1)
		{
			mDummyTaskbarWindow = (DummyTaskbarWindow)target;
			((FrameworkElement)mDummyTaskbarWindow).Loaded += new RoutedEventHandler(DummyTaskbarWindow_Loaded);
			((Window)mDummyTaskbarWindow).Closing += DummyTaskbarWindow_Closing;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
