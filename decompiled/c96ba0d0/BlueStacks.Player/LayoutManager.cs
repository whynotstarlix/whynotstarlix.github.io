using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal static class LayoutManager
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action _003C_003E9__14_0;

		internal void _003COrientationHandler_003Eb__14_0()
		{
			try
			{
				Logger.Info("Orientation handler calling fixguest");
				if (Oem.Instance.IsResizeFrontendWindow)
				{
					ResizeFrontendWindow();
				}
				VMWindow.Instance.ResizeWindowOnStreamingMode();
				FixupGuestDisplay();
			}
			catch (Exception)
			{
			}
		}
	}

	internal static Size mConfiguredDisplaySize;

	internal static Size mConfiguredGuestSize;

	internal static Size mCurrentDisplaySize;

	internal static Rectangle mScaledDisplayArea;

	internal static bool mEmulatedPortraitMode;

	internal static int sCurrentOrientation;

	internal static bool mRotateGuest180;

	private static bool mUpdateGlWindowSize;

	private static Rectangle mLastGLRectangle;

	internal static bool mFullScreen;

	internal static bool UpdateGlWindowSize
	{
		get
		{
			return mUpdateGlWindowSize;
		}
		set
		{
			mUpdateGlWindowSize = value;
			if (value)
			{
				UpdateSizeToGM();
			}
		}
	}

	[DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
	private static extern void ImapSetGLWindowParams(int cx, int cy, int width, int height);

	internal static void OrientationHandler(int orientation)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected Obj, but got Unknown
		Logger.Info("Got orientation change notification for {0}", new object[1] { orientation });
		bool flag = ShouldEmulatePortraitMode();
		Logger.Info("ShouldEmulatePortraitMode => " + flag);
		if (sCurrentOrientation == orientation)
		{
			Logger.Info("Not doing anything as current orientation is same as orientation requested");
			return;
		}
		sCurrentOrientation = orientation;
		if (orientation == 2 || orientation == 3)
		{
			mRotateGuest180 = true;
		}
		else
		{
			mRotateGuest180 = false;
		}
		if (flag)
		{
			mEmulatedPortraitMode = orientation == 1 || orientation == 3;
			mRotateGuest180 = orientation == 2 || orientation == 3;
		}
		else
		{
			mEmulatedPortraitMode = false;
		}
		VMWindow instance = VMWindow.Instance;
		Action val = _003C_003Ec._003C_003E9__14_0;
		if (val == null)
		{
			Action val2 = () =>
			{
				try
				{
					Logger.Info("Orientation handler calling fixguest");
					if (Oem.Instance.IsResizeFrontendWindow)
					{
						ResizeFrontendWindow();
					}
					VMWindow.Instance.ResizeWindowOnStreamingMode();
					FixupGuestDisplay();
				}
				catch (Exception)
				{
				}
			};
			_003C_003Ec._003C_003E9__14_0 = val2;
			val = val2;
		}
		UIHelper.RunOnUIThread((Control)(object)instance, val);
	}

	internal static void FixupGuestDisplay()
	{
		((Control)VMWindow.Instance).Invalidate();
		FixupGuestDisplay_FixAspectRatio();
		FixupGuestDisplay_FixOpenGLSubwindow();
	}

	private static void FixupGuestDisplay_FixAspectRatio()
	{
		mScaledDisplayArea.X = 0;
		mScaledDisplayArea.Y = 0;
		mScaledDisplayArea.Width = ((Form)VMWindow.Instance).ClientSize.Width;
		mScaledDisplayArea.Height = ((Form)VMWindow.Instance).ClientSize.Height;
	}

	private static void FixupGuestDisplay_FixOpenGLSubwindow()
	{
		int orientation;
		if (IsPortrait())
		{
			orientation = sCurrentOrientation;
		}
		else
		{
			orientation = ((!mEmulatedPortraitMode) ? (mRotateGuest180 ? 2 : 0) : ((!mRotateGuest180) ? 1 : 3));
		}
		int width = mScaledDisplayArea.Width;
		int num = mScaledDisplayArea.Height;
		if (RegistryManager.Instance.IsImeDebuggingEnabled)
		{
			if (VMWindow.sIsWpfTextboxEnabled)
			{
				((Control)VMWindow.Instance.mCtrlHost).Location = new Point(0, 0);
				((Control)VMWindow.Instance.mCtrlHost).Size = new Size(200, 200);
			}
			else
			{
				((Control)VMWindow.Instance.mDummyInputKeyBoard).Location = new Point(0, 0);
				((Control)VMWindow.Instance.mDummyInputKeyBoard).Size = new Size(200, 200);
			}
			Opengl.ResizeSubWindow(200, 200, width - 200, num - 200);
		}
		else if (mLastGLRectangle.Width != width || mLastGLRectangle.Height != num || mLastGLRectangle.X != mScaledDisplayArea.X || mLastGLRectangle.Y != mScaledDisplayArea.Y)
		{
			mLastGLRectangle.Width = width;
			mLastGLRectangle.Height = num;
			if (VMWindow.Instance.IsFullscreen)
			{
				num--;
			}
			mLastGLRectangle.X = mScaledDisplayArea.X;
			mLastGLRectangle.Y = mScaledDisplayArea.Y;
			Opengl.ResizeSubWindow(mScaledDisplayArea.X, mScaledDisplayArea.Y, width, num);
			if (UpdateGlWindowSize)
			{
				UpdateSizeToGM();
			}
		}
		Opengl.HandleOrientation(1f, 1f, orientation);
		ImapSetGLWindowParams(mScaledDisplayArea.X, mScaledDisplayArea.Y, width, num);
	}

	private static void UpdateSizeToGM()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string> { 
		{
			"handle",
			Opengl.GetSubWindow().ToInt32().ToString()
		} };
		HTTPUtils.SendRequestToClientAsync("updateSizeOfOverlay", dictionary, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	internal static void InitScreen()
	{
		Logger.Info("InitScreen()");
		mConfiguredDisplaySize = GetConfiguredDisplaySize();
		mConfiguredGuestSize = GetConfiguredGuestSize();
	}

	internal static Size GetConfiguredDisplaySize()
	{
		int windowWidth = RegistryManager.Instance.DefaultGuest.WindowWidth;
		int windowHeight = RegistryManager.Instance.DefaultGuest.WindowHeight;
		return new Size(windowWidth, windowHeight);
	}

	internal static Size GetConfiguredGuestSize()
	{
		int guestWidth = RegistryManager.Instance.DefaultGuest.GuestWidth;
		int guestHeight = RegistryManager.Instance.DefaultGuest.GuestHeight;
		return new Size(guestWidth, guestHeight);
	}

	private static bool ShouldEmulatePortraitMode()
	{
		if (RegistryManager.Instance.DefaultGuest.EmulatePortraitMode == 1)
		{
			return true;
		}
		if (RegistryManager.Instance.DefaultGuest.EmulatePortraitMode == 0)
		{
			return false;
		}
		return IsDesktop();
	}

	internal static bool IsDesktop()
	{
		bool result;
		if (Features.IsFeatureEnabled(1073741824uL))
		{
			result = true;
		}
		else if (Utils.IsDesktopPC())
		{
			result = true;
		}
		else
		{
			try
			{
				List<DeviceEnumerator> list = DeviceEnumerator.ListDevices(Guids.VideoInputDeviceCategory);
				result = list.Count != 2;
				foreach (DeviceEnumerator item in list)
				{
					item.Dispose();
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Cannot enumerate camera devices: " + ex);
				result = false;
			}
		}
		return result;
	}

	private static int GetBorderWidth(int width, int height)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RECT val = default;
		val.Left = 0;
		val.Top = 0;
		val.Right = width;
		val.Bottom = height;
		int num = 13565952;
		if (!InteropWindow.AdjustWindowRect(ref val, num, false))
		{
			return 18;
		}
		return val.Right - val.Left - width;
	}

	internal static bool IsPortrait()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		ScreenOrientation screenOrientation = SystemInformation.ScreenOrientation;
		if ((int)screenOrientation != 1)
		{
			return (int)screenOrientation == 3;
		}
		return true;
	}

	internal static void HandleLayoutEvent()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)((Form)VMWindow.Instance).WindowState != 1)
		{
			VMWindow.Instance.HandleFrontendActivated();
		}
	}

	internal static void InitOpengl()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Opengl.Init({0}, {1}, {2}, {3}, {4})", new object[5]
		{
			((Control)VMWindow.Instance).Handle,
			mScaledDisplayArea.X,
			mScaledDisplayArea.Y,
			mConfiguredGuestSize.Width,
			mConfiguredGuestSize.Height
		});
		Opengl.Init(MultiInstanceStrings.VmName, ((Control)VMWindow.Instance).Handle, mScaledDisplayArea.X, mScaledDisplayArea.Y, mConfiguredGuestSize.Width, mConfiguredGuestSize.Height, GlInitSuccess, GlInitFailed);
		TimelineStatsSender.HandleEngineBootEvent(((object)(EngineStatsEvent)2/*cast due to constrained. prefix*/).ToString());
		Logger.Info("Done Opengl.Init");
	}

	internal static void GlInitSuccess()
	{
		Logger.Info("BOOT_STAGE: Gl Init success");
		FixupGuestDisplay();
		InputMapper.Instance.InputmapperInit();
	}

	internal static void GlInitFailed()
	{
		Logger.Error("Gl Init failed");
		AndroidBootUp.HandleBootError();
	}

	internal static int GetGuestX(int x, int y)
	{
		int num = 0;
		int landscapeGuestX = GetLandscapeGuestX(x, y);
		int portraitGuestX = GetPortraitGuestX(x, y);
		if (!IsPortrait() && !mEmulatedPortraitMode)
		{
			if (!mRotateGuest180)
			{
				return landscapeGuestX;
			}
			return 32768 - landscapeGuestX;
		}
		if (!mRotateGuest180)
		{
			return 32768 - portraitGuestX;
		}
		return portraitGuestX;
	}

	internal static int GetGuestY(int x, int y)
	{
		int num = 0;
		int landscapeGuestY = GetLandscapeGuestY(x, y);
		int portraitGuestY = GetPortraitGuestY(x, y);
		if (!IsPortrait() && !mEmulatedPortraitMode)
		{
			if (!mRotateGuest180)
			{
				return landscapeGuestY;
			}
			return 32768 - landscapeGuestY;
		}
		if (!mRotateGuest180)
		{
			return portraitGuestY;
		}
		return 32768 - portraitGuestY;
	}

	internal static int GetLandscapeGuestX(int x, int y)
	{
		int num = x - mScaledDisplayArea.X;
		if (mScaledDisplayArea.Width == 0)
		{
			return 0;
		}
		return (int)((float)num * 32768f / (float)mScaledDisplayArea.Width);
	}

	internal static int GetPortraitGuestX(int x, int y)
	{
		int num = y - mScaledDisplayArea.Y;
		if (mScaledDisplayArea.Height == 0)
		{
			return 0;
		}
		return (int)((float)num * 32768f / (float)mScaledDisplayArea.Height);
	}

	internal static int GetLandscapeGuestY(int x, int y)
	{
		int num = y - mScaledDisplayArea.Y;
		if (mScaledDisplayArea.Height == 0)
		{
			return 0;
		}
		return (int)((float)num * 32768f / (float)mScaledDisplayArea.Height);
	}

	internal static int GetPortraitGuestY(int x, int y)
	{
		int num = x - mScaledDisplayArea.X;
		if (mScaledDisplayArea.Width == 0)
		{
			return 0;
		}
		return (int)((float)num * 32768f / (float)mScaledDisplayArea.Width);
	}

	internal static void ToggleFullScreen()
	{
		if (!mFullScreen)
		{
			mFullScreen = true;
			ResizeFrontendWindow();
			if (Features.IsFeatureEnabled(17179869184uL))
			{
				VMWindow.Instance.mFullScreenToast.Show();
			}
		}
		else
		{
			mFullScreen = false;
			ResizeFrontendWindow();
			VMWindow.Instance.mFullScreenToast.Hide();
		}
	}

	internal static void ResizeFrontendWindow()
	{
		Logger.Info("ResizeFrontendWindow()");
		Logger.Info("Suspending Layout");
		((Control)VMWindow.Instance).SuspendLayout();
		if (mFullScreen)
		{
			ResizeFrontendWindow_FullScreen();
		}
		else
		{
			ResizeFrontendWindow_Windowed();
		}
		Logger.Info("Resuming Layout");
		((Control)VMWindow.Instance).ResumeLayout();
		FixupGuestDisplay();
		Logger.Info("ResizeFrontendWindow DONE");
	}

	internal static void ResizeFrontendWindow_FullScreen()
	{
		Logger.Info("ResizeFrontendWindow_FullScreen()");
		Logger.Info("Screen size is {0}x{1}", new object[2]
		{
			InteropWindow.ScreenWidth,
			InteropWindow.ScreenHeight
		});
		Logger.Info("Guest display area is {0}x{1}", new object[2] { mCurrentDisplaySize.Width, mCurrentDisplaySize.Height });
		((Form)VMWindow.Instance).FormBorderStyle = (FormBorderStyle)0;
		if (mEmulatedPortraitMode && Features.IsFeatureEnabled(2048uL))
		{
			float num = (float)mConfiguredGuestSize.Width / (float)mConfiguredGuestSize.Height;
			Size size = new Size
			{
				Height = Screen.PrimaryScreen.WorkingArea.Height
			};
			size.Height -= Oem.Instance.PartnerControlBarHeight;
			size.Width = (int)((float)size.Height / num);
			int num2 = InteropWindow.ScreenWidth - size.Width;
			int num3 = 0;
			int width = size.Width;
			int height = size.Height;
			InteropWindow.SetFullScreen(((Control)VMWindow.Instance).Handle, num2, num3, width, height);
		}
		else
		{
			InteropWindow.SetFullScreen(((Control)VMWindow.Instance).Handle);
		}
		Logger.Info("ResizeFrontendWindow_FullScreen DONE");
	}

	internal static void ResizeFrontendWindow_Windowed()
	{
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("ResizeFrontendWindow_Windowed()");
		Logger.Info("mEmulatedPortraitMode: " + mEmulatedPortraitMode);
		Size clientSize = default;
		int num = 20;
		int num2 = 20;
		if (Oem.Instance.IsFrontendFormLocation6)
		{
			num = 6;
			num2 = 6;
		}
		int height = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - GetBorderWidth(100, 100);
		if (mEmulatedPortraitMode)
		{
			clientSize.Height = height;
			clientSize.Height -= Oem.Instance.PartnerControlBarHeight;
			double num3 = (double)mConfiguredGuestSize.Width / (double)mConfiguredGuestSize.Height;
			double num4 = (double)((Form)VMWindow.Instance).ClientSize.Height / (double)((Form)VMWindow.Instance).ClientSize.Width;
			if (num3 == num4)
			{
				clientSize.Height = ((Form)VMWindow.Instance).ClientSize.Height;
			}
			clientSize.Width = (int)((double)clientSize.Height / num3);
			num = Screen.PrimaryScreen.WorkingArea.Width - clientSize.Width - GetBorderWidth(100, 100) / 2;
			num2 = GetBorderWidth(100, 100) / 2;
			Logger.Info("location: ({0}x{1})", new object[2] { num, num2 });
		}
		else if (!IsPortrait())
		{
			clientSize.Width = mConfiguredDisplaySize.Width;
			clientSize.Height = mConfiguredDisplaySize.Height;
		}
		else
		{
			clientSize.Width = mConfiguredDisplaySize.Height;
			clientSize.Height = mConfiguredDisplaySize.Width;
			clientSize.Height -= Oem.Instance.PartnerControlBarHeight;
		}
		mCurrentDisplaySize = clientSize;
		Logger.Info("Guest display area is {0}x{1}", new object[2] { mCurrentDisplaySize.Width, mCurrentDisplaySize.Height });
		Logger.Info("New window size is {0}x{1}", new object[2] { clientSize.Width, clientSize.Height });
		if (VMWindow.Instance.isStreamingModeEnabled)
		{
			((Form)VMWindow.Instance).MaximizeBox = true;
			((Form)VMWindow.Instance).WindowState = (FormWindowState)0;
			((Form)VMWindow.Instance).FormBorderStyle = (FormBorderStyle)4;
		}
		else if (!Oem.Instance.IsFrontendBorderHidden)
		{
			((Form)VMWindow.Instance).FormBorderStyle = VMWindow.Instance.mFormBorderStyle;
		}
		((Form)VMWindow.Instance).StartPosition = (FormStartPosition)0;
		((Form)VMWindow.Instance).Location = new Point(num, num2);
		((Form)VMWindow.Instance).ClientSize = clientSize;
		Logger.Info("New client size is {0}x{1}", new object[2]
		{
			((Form)VMWindow.Instance).ClientSize.Width,
			((Form)VMWindow.Instance).ClientSize.Height
		});
		Logger.Info("ResizeFrontendWindow_Windowed DONE");
	}

	static LayoutManager()
	{
	}
}
