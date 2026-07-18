using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ImageTranslateControl : UserControl, IDimOverlayControl, IComponentConnector
{
	private MainWindow ParentWindow;

	private Thread httpBackGroundThread;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTopBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mTitleLabel;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mFrontEndImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mLoadingImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mBootText;

	private bool _contentLoaded;

	public static ImageTranslateControl Instance { get; private set; }

	bool IDimOverlayControl.IsCloseOnOverLayClick
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow { get; set; } = true;

	public bool ShowTransparentWindow { get; set; } = true;

	public ImageTranslateControl(MainWindow parentWindow)
	{
		InitializeComponent();
		Instance = this;
		ParentWindow = parentWindow;
		if (ParentWindow != null)
		{
			((FrameworkElement)this).Width = ((FrameworkElement)parentWindow.FrontendParentGrid).ActualWidth;
			((FrameworkElement)this).Height = ((FrameworkElement)parentWindow.FrontendParentGrid).ActualHeight;
		}
		((UIElement)mLoadingImage).Visibility = (Visibility)0;
		((UIElement)mFrontEndImage).Visibility = (Visibility)2;
		((UIElement)mTopBar).Visibility = (Visibility)2;
		((UIElement)mBootText).Visibility = (Visibility)0;
		mBootText.Text = LocaleStrings.GetLocalizedString("STRING_LOADING_MESSAGE", "");
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((UIElement)Window.GetWindow((DependencyObject)(object)this)).KeyDown += new KeyEventHandler(UserControl_KeyDown);
	}

	public void GetTranslateImage(Bitmap bitmap)
	{
		if (bitmap == null)
		{
			return;
		}
		httpBackGroundThread = new Thread((ThreadStart)delegate
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			using MemoryStream memoryStream = new MemoryStream();
			try
			{
				((Image)bitmap).Save((Stream)memoryStream, ImageFormat.Jpeg);
				memoryStream.Position = 0L;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				string userSelectedLocale = RegistryManager.Instance.UserSelectedLocale;
				userSelectedLocale = userSelectedLocale.Substring(0, 2);
				if (string.Equals(userSelectedLocale, "zh-CN", StringComparison.InvariantCulture) || string.Equals(userSelectedLocale, "zh-TW", StringComparison.InvariantCulture))
				{
					userSelectedLocale = RegistryManager.Instance.UserSelectedLocale;
				}
				if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocale))
				{
					userSelectedLocale = RegistryManager.Instance.TargetLocale;
				}
				dictionary.Add("locale", userSelectedLocale);
				dictionary.Add("inputImage", (object)new FormFile
				{
					Name = "image.jpg",
					ContentType = "image/jpeg",
					Stream = memoryStream
				});
				dictionary.Add("oem", RegistryManager.Instance.Oem);
				dictionary.Add("guid", RegistryManager.Instance.UserGuid);
				dictionary.Add("prod_ver", RegistryManager.Instance.ClientVersion);
				string text = Convert.ToBase64String(memoryStream.ToArray());
				text = text + RegistryManager.Instance.UserGuid + "BstTranslate";
				_MD5 val = new _MD5
				{
					Value = text
				};
				dictionary.Add("token", val.FingerPrint);
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
				{
					RegistryManager.Instance.Host,
					"/translate/postimage"
				});
				if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocaleUrl))
				{
					text2 = RegistryManager.Instance.TargetLocaleUrl;
				}
				string empty = string.Empty;
				byte[] dataArray = null;
				try
				{
					empty = BstHttpClient.PostMultipart(text2, dictionary, ref dataArray);
				}
				catch (Exception ex)
				{
					Logger.Error("error while downloading translated image.." + ex.ToString());
					empty = "error";
				}
				if (empty.Contains("error"))
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						((UIElement)mLoadingImage).Visibility = (Visibility)2;
						mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
					}, new object[0]);
				}
				else
				{
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						BitmapImage source = ImageUtils.ByteArrayToImage(dataArray);
						((Image)mFrontEndImage).Source = (ImageSource)(object)source;
						mFrontEndImage.ReloadImages();
						((UIElement)mFrontEndImage).Visibility = (Visibility)0;
						((UIElement)mTopBar).Visibility = (Visibility)0;
						((UIElement)mLoadingImage).Visibility = (Visibility)2;
						((UIElement)mBootText).Visibility = (Visibility)2;
					}, new object[0]);
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Error in GetTranslateImage " + ex2);
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					((UIElement)mLoadingImage).Visibility = (Visibility)2;
					mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
				}, new object[0]);
			}
		})
		{
			IsBackground = true
		};
		httpBackGroundThread.Start();
	}

	public bool Close()
	{
		try
		{
			Instance = null;
			httpBackGroundThread?.Abort();
			ParentWindow?.HideDimOverlay();
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close imagetranslateontrol from dimoverlay " + ex.ToString());
		}
		return false;
	}

	private void UserControl_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Key == 13)
		{
			Close();
		}
	}

	private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Close();
	}

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

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/imagetranslatecontrol.xaml", UriKind.Relative);
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
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(ImageTranslateControl)target).Loaded += new RoutedEventHandler(UserControl_Loaded);
			break;
		case 2:
			mGrid = (Grid)target;
			break;
		case 3:
			mTopBar = (Grid)target;
			break;
		case 4:
			mTitleLabel = (Label)target;
			break;
		case 5:
			mCloseButton = (CustomPictureBox)target;
			((UIElement)mCloseButton).MouseLeftButtonUp += new MouseButtonEventHandler(CloseButton_MouseLeftButtonUp);
			break;
		case 6:
			mFrontEndImage = (CustomPictureBox)target;
			break;
		case 7:
			mLoadingImage = (CustomPictureBox)target;
			break;
		case 8:
			mBootText = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
