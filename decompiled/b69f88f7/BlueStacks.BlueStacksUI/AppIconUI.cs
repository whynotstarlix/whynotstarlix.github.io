using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI;

public class AppIconUI : Button, IComponentConnector
{
	private Thread threadShowingUninstallButton;

	private ImageAnimationController mGifController;

	private MainWindow ParentWindow;

	private DownloadInstallApk mDownloader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMainGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mImageGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mAppImageBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mAppImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mProgressGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal BlueProgressBar CustomProgressBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mBusyGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mBusyImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mErrorGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mRetryGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mSuggestedAppPromotionImage;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mUnInstallTabButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mGl3ErrorIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mGl3InfoIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mRedDotNotifIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock AppNameTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mGamePadGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mGamepadIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPopUp mGamePadToolTipPopup;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mIconText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Path mUpArrow;

	private bool _contentLoaded;

	internal AppIconModel mAppIconModel { get; private set; }

	public AppIconUI(MainWindow window, AppIconModel model)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow = window;
		mAppIconModel = model;
		((FrameworkElement)this).DataContext = mAppIconModel;
		InitializeComponent();
		if (mAppIconModel != null)
		{
			SetAppIconLocation(mAppIconModel.IconLocation, mAppIconModel.IconHeight, mAppIconModel.IconWidth);
		}
	}

	internal void InitAppDownloader(DownloadInstallApk downloadInstallApk = null)
	{
		mDownloader = downloadInstallApk;
		mAppIconModel.mAppDownloadingEvent += DownloadingApp;
	}

	private void Button_Loaded(object sender, RoutedEventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		if (!DesignerProperties.GetIsInDesignMode((DependencyObject)(object)this))
		{
			((FrameworkElement)this).Loaded -= new RoutedEventHandler(Button_Loaded);
			ParentWindow.StaticComponents.ShowAllUninstallButtons += Button_MouseHoldAction;
			ParentWindow.StaticComponents.HideAllUninstallButtons += AppIcon_HideUninstallButton;
			if (mAppIconModel.IsGifIcon)
			{
				ParentWindow.StaticComponents.PlayAllGifs += GifAppIconPlay;
				ParentWindow.StaticComponents.PauseAllGifs += GifAppIconPause;
			}
			mAppIconModel.IsGamepadConnected = ParentWindow.IsGamepadConnected;
		}
	}

	private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (threadShowingUninstallButton != null)
		{
			return;
		}
		threadShowingUninstallButton = new Thread((ThreadStart)delegate
		{
			Thread.Sleep(1000);
			if (threadShowingUninstallButton != null)
			{
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					object obj = sender;
					((UIElement)((obj is UIElement) ? obj : null)).ReleaseMouseCapture();
					ParentWindow.StaticComponents.ShowUninstallButtons(isShow: true);
					threadShowingUninstallButton = null;
				}, new object[0]);
			}
		})
		{
			IsBackground = true
		};
		threadShowingUninstallButton.Start();
	}

	private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (threadShowingUninstallButton != null && threadShowingUninstallButton.IsAlive)
		{
			threadShowingUninstallButton = null;
		}
	}

	private void Button_MouseHoldAction(object sender, EventArgs e)
	{
		ShowAppUninstallButton(isShow: true);
	}

	private void AppIcon_HideUninstallButton(object sender, EventArgs e)
	{
		ShowAppUninstallButton(isShow: false);
	}

	private void ShowAppUninstallButton(bool isShow)
	{
		if (isShow && mAppIconModel.mIsAppRemovable && (!mAppIconModel.IsInstalling || mAppIconModel.IsInstallingFailed))
		{
			((UIElement)mUnInstallTabButton).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)mUnInstallTabButton).Visibility = (Visibility)1;
		}
	}

	private void GamepadIcon_MouseEnter(object sender, MouseEventArgs e)
	{
		mIconText.Text = (mAppIconModel.IsGamepadConnected ? LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", "") : LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", ""));
		mAppIconModel.AppNameTooltip = null;
		((Popup)mGamePadToolTipPopup).PlacementTarget = (UIElement)(object)mGamepadIcon;
		((Popup)mGamePadToolTipPopup).IsOpen = true;
		((Popup)mGamePadToolTipPopup).StaysOpen = true;
	}

	private void GamepadIcon_MouseLeave(object sender, MouseEventArgs e)
	{
		((Popup)mGamePadToolTipPopup).IsOpen = false;
		mAppIconModel.AppNameTooltip = mAppIconModel.AppName;
	}

	private void Button_MouseLeave(object sender, MouseEventArgs e)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01a7: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_020a: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Invalid comparison between Unknown and I4
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Invalid comparison between Unknown and I4
		if (!ParentWindow.StaticComponents.IsDeleteButtonVisible)
		{
			ShowAppUninstallButton(isShow: false);
		}
		((UIElement)mRetryGrid).Visibility = (Visibility)1;
		ScaleTransform val = new ScaleTransform(1.02, 1.02);
		((UIElement)mAppImage).RenderTransformOrigin = new Point(0.5, 0.5);
		((UIElement)mAppImage).RenderTransform = (Transform)(object)val;
		((UIElement)AppNameTextBox).RenderTransformOrigin = new Point(0.5, 0.5);
		((UIElement)AppNameTextBox).RenderTransform = (Transform)(object)val;
		DoubleAnimation val2 = new DoubleAnimation(1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
		{
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)1
			}
		};
		((Animatable)val).BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline)(object)val2);
		((Animatable)val).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val2);
		DropShadowEffect val3 = new DropShadowEffect
		{
			Color = Colors.White,
			Direction = 270.0,
			ShadowDepth = 1.0,
			BlurRadius = 20.0,
			Opacity = 1.0
		};
		((UIElement)mAppImageBorder).Effect = (Effect)(object)val3;
		DoubleAnimation val4 = new DoubleAnimation(6.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
		{
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)1
			}
		};
		DoubleAnimation val5 = new DoubleAnimation(0.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
		{
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)1
			}
		};
		((Animatable)val3).BeginAnimation(DropShadowEffect.BlurRadiusProperty, (AnimationTimeline)(object)val4);
		((Animatable)val3).BeginAnimation(DropShadowEffect.OpacityProperty, (AnimationTimeline)(object)val5);
		AppNameTextBox.Foreground = (Brush)new SolidColorBrush(Colors.White);
		DoubleAnimation val6 = new DoubleAnimation(1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
		{
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)1
			}
		};
		((UIElement)AppNameTextBox).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val6);
		if (mAppIconModel.IsAppSuggestionActive)
		{
			ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
		}
		if ((int)mAppIconModel.IconLocation == 1 || (int)mAppIconModel.IconLocation == 2)
		{
			ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, isOpen: false);
		}
	}

	private void Button_MouseEnter(object sender, MouseEventArgs e)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d5: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0109: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_01ca: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Invalid comparison between Unknown and I4
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Invalid comparison between Unknown and I4
		if (mAppIconModel.IsDownloading)
		{
			ShowAppUninstallButton(isShow: true);
		}
		if (mAppIconModel.IsDownLoadingFailed || mAppIconModel.IsInstallingFailed)
		{
			((UIElement)mRetryGrid).Visibility = (Visibility)0;
		}
		if ((int)((UIElement)mBusyGrid).Visibility != 0)
		{
			DropShadowEffect val = new DropShadowEffect
			{
				Color = Colors.White,
				Direction = 270.0,
				ShadowDepth = 1.0,
				BlurRadius = 0.0,
				Opacity = 0.0
			};
			((UIElement)mAppImageBorder).Effect = (Effect)(object)val;
			DoubleAnimation val2 = new DoubleAnimation(20.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
			{
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			DoubleAnimation val3 = new DoubleAnimation(1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
			{
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			((Animatable)val).BeginAnimation(DropShadowEffect.BlurRadiusProperty, (AnimationTimeline)(object)val2);
			((Animatable)val).BeginAnimation(DropShadowEffect.OpacityProperty, (AnimationTimeline)(object)val3);
			ScaleTransform val4 = new ScaleTransform(1.0, 1.0);
			((UIElement)mAppImage).RenderTransformOrigin = new Point(0.5, 0.5);
			((UIElement)mAppImage).RenderTransform = (Transform)(object)val4;
			((UIElement)AppNameTextBox).RenderTransformOrigin = new Point(0.5, 0.5);
			((UIElement)AppNameTextBox).RenderTransform = (Transform)(object)val4;
			DoubleAnimation val5 = new DoubleAnimation(1.02, Duration.op_Implicit(TimeSpan.FromMilliseconds(300.0)))
			{
				EasingFunction = (IEasingFunction)new QuadraticEase
				{
					EasingMode = (EasingMode)1
				}
			};
			((Animatable)val4).BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline)(object)val5);
			((Animatable)val4).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val5);
			if ((int)mAppIconModel.IconLocation == 1 || (int)mAppIconModel.IconLocation == 2)
			{
				ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, isOpen: true);
				val.BlurRadius = 12.0;
				((UIElement)mAppImageBorder).Effect = (Effect)(object)val;
			}
			if (mAppIconModel.IsAppSuggestionActive)
			{
				ParentWindow.mWelcomeTab.mHomeAppManager.OpenAppSuggestionPopup(mAppIconModel.AppSuggestionInfo, (UIElement)(object)AppNameTextBox);
			}
		}
	}

	private void UninstallButtonClicked()
	{
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (mAppIconModel.IsDownloading)
			{
				if (mDownloader != null)
				{
					mDownloader.AbortApkDownload(mAppIconModel.PackageName);
					ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(mAppIconModel.PackageName);
				}
				return;
			}
			if (mAppIconModel.IsInstalling)
			{
				if (mAppIconModel.IsInstallingFailed)
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(mAppIconModel.PackageName);
				}
				return;
			}
			if (mAppIconModel.IsAppSuggestionActive)
			{
				CustomMessageWindow val = new CustomMessageWindow();
				val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_REMOVE_ICON", "");
				val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ICON_REMOVE", "");
				val.AddButton((ButtonColors)0, LocaleStrings.GetLocalizedString("STRING_REMOVE", ""), (EventHandler)RemoveAppSuggestion, (string)null, false, (object)null);
				val.AddButton((ButtonColors)2, LocaleStrings.GetLocalizedString("STRING_KEEP", ""), (EventHandler)null, (string)null, false, (object)null);
				((Window)val).Owner = (Window)(object)ParentWindow;
				ParentWindow.ShowDimOverlay();
				((Window)val).ShowDialog();
				ParentWindow.HideDimOverlay();
				return;
			}
			CustomMessageWindow val2 = new CustomMessageWindow();
			val2.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0", ""), new object[1] { mAppIconModel.AppName });
			val2.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0_BS", ""), new object[1] { mAppIconModel.AppName });
			val2.AddButton((ButtonColors)0, "STRING_UNINSTALL", (EventHandler)delegate(object o, EventArgs e)
			{
				AppIcon_UninstallConfirmationClicked(o, e);
			}, (string)null, false, (object)null);
			val2.AddButton((ButtonColors)2, "STRING_NO", (EventHandler)null, (string)null, false, (object)null);
			ParentWindow.ShowDimOverlay();
			((Window)val2).Owner = (Window)(object)ParentWindow.mDimOverlay;
			((Window)val2).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in UninstallButtonClicked. Err : " + ex.ToString());
		}
	}

	private void RemoveAppSuggestion(object sender, EventArgs e)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(mAppIconModel.PackageName);
		ClientStats.SendMiscellaneousStatsAsync("cross_promotion_icon_removed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, mAppIconModel.PackageName, "bgp64");
		try
		{
			XmlWriterSettings val = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true
			};
			XmlSerializerNamespaces val2 = new XmlSerializerNamespaces((XmlQualifiedName[])(object)new XmlQualifiedName[1]
			{
				new XmlQualifiedName("", "")
			});
			string text = Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
			string text2 = "";
			if (File.Exists(text))
			{
				text2 = File.ReadAllText(text);
			}
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(text2))
			{
				list = DoDeserialize<List<string>>(text2);
			}
			if (!list.Contains(mAppIconModel.PackageName))
			{
				if (list.Count >= 20)
				{
					list.RemoveAt(0);
				}
				list.Add(mAppIconModel.PackageName);
			}
			XmlWriter val3 = XmlWriter.Create(text, val);
			try
			{
				new XmlSerializer(typeof(List<string>)).Serialize(val3, (object)list, val2);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error in writing removed suggested app icon package name in file " + ex.ToString());
		}
	}

	private static T DoDeserialize<T>(string data) where T : class
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		XmlReader val = XmlReader.Create((Stream)new MemoryStream(Encoding.UTF8.GetBytes(data)));
		try
		{
			return (T)new XmlSerializer(typeof(T)).Deserialize(val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void AppIcon_UninstallConfirmationClicked(object _1, EventArgs _2)
	{
		Logger.Info("Clicked app icon uninstall popup package name {0}", new object[1] { mAppIconModel.PackageName });
		ParentWindow.mAppInstaller.UninstallApp(mAppIconModel.PackageName);
		ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(mAppIconModel.PackageName);
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("Clicked app icon, package name {0}", new object[1] { mAppIconModel.PackageName });
		if (((UIElement)mUnInstallTabButton).IsMouseOver)
		{
			UninstallButtonClicked();
		}
		else if (((UIElement)mErrorGrid).IsVisible)
		{
			((UIElement)mErrorGrid).Visibility = (Visibility)1;
			if (mAppIconModel.IsDownLoadingFailed)
			{
				mDownloader?.DownloadApk(mAppIconModel.ApkUrl, mAppIconModel.PackageName, isLaunchAfterInstall: false, isDeleteApk: false);
			}
			else if (mAppIconModel.IsInstallingFailed)
			{
				mDownloader?.InstallApk(mAppIconModel.PackageName, mAppIconModel.ApkFilePath, isLaunchAfterInstall: false, isDeleteApk: false);
			}
		}
		else
		{
			if (mAppIconModel.IsDownloading)
			{
				return;
			}
			if (mAppIconModel.IsInstalling)
			{
				if (mDownloader == null)
				{
					ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(mAppIconModel.PackageName, mAppIconModel.AppName, (PlayStoreAction)0);
				}
			}
			else if (mAppIconModel.IsRerollIcon)
			{
				HandleRerollClick();
			}
			else if (mAppIconModel.IsAppSuggestionActive)
			{
				HandleAppSuggestionClick();
				if (mAppIconModel.IsRedDotVisible)
				{
					mAppIconModel.IsRedDotVisible = false;
					HomeAppManager.AddPackageInRedDotShownRegistry(mAppIconModel.PackageName);
				}
			}
			else if (!string.IsNullOrEmpty(mAppIconModel.PackageName))
			{
				if (string.Equals(mAppIconModel.PackageName, "help_center", StringComparison.InvariantCulture))
				{
					ParentWindow.mTopBar.mAppTabButtons.AddWebTab(BlueStacksUIUtils.GetHelpCenterUrl(), "STRING_FEEDBACK", "help_center", isSwitch: true, "STRING_FEEDBACK");
				}
				else if (string.Equals(mAppIconModel.PackageName, "instance_manager", StringComparison.InvariantCulture))
				{
					BlueStacksUIUtils.LaunchMultiInstanceManager();
				}
				else if (string.Equals(mAppIconModel.PackageName, "macro_recorder", StringComparison.InvariantCulture))
				{
					ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
				}
				else
				{
					ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(mAppIconModel.PackageName);
				}
			}
		}
	}

	private void HandleAppSuggestionClick()
	{
		ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>)(object)mAppIconModel.AppSuggestionInfo.ExtraPayload, "my_apps", mAppIconModel.ImageName);
		SendAppSuggestionIconClickStats();
	}

	private void SendAppSuggestionIconClickStats()
	{
		ClientStats.SendPromotionAppClickStatsAsync(new Dictionary<string, string>
		{
			{ "op", "init" },
			{ "status", "success" },
			{ "app_pkg", mAppIconModel.PackageName },
			{
				"extraPayload",
				JsonConvert.SerializeObject((object)mAppIconModel.AppSuggestionInfo.ExtraPayload)
			},
			{ "app_name", mAppIconModel.AppName },
			{ "app_promotion_id", mAppIconModel.mPromotionId },
			{ "promotion_type", "cross_promotion" }
		}, "app_activity");
	}

	private void HandleRerollClick()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		CustomMessageWindow val = new CustomMessageWindow();
		val.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_REROLL_0", ""), new object[1] { mAppIconModel.AppName });
		val.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_START_REROLL", ""), new object[1] { mAppIconModel.AppName });
		val.AddButton((ButtonColors)2, "STRING_CANCEL", (EventHandler)null, (string)null, false, (object)null);
		val.AddButton((ButtonColors)4, "STRING_REROLL_APP_PREFIX", (EventHandler)StartRerollAccepted, (string)null, false, (object)null);
		ParentWindow.ShowDimOverlay();
		((Window)val).Owner = (Window)(object)ParentWindow.mDimOverlay;
		((Window)val).ShowDialog();
		if ((int)val.ClickedButton != 2)
		{
			ParentWindow.HideDimOverlay();
		}
	}

	private void StartRerollAccepted(object sender, EventArgs e)
	{
		ParentWindow.ShowRerollOverlay();
		ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startReroll", new Dictionary<string, string>
		{
			{ "packageName", mAppIconModel.PackageName },
			{ "rerollName", "" }
		});
	}

	private void GifAppIconPlay()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		try
		{
			mGifController = ImageBehavior.GetAnimationController((Image)(object)mAppImage);
			if (mGifController != null)
			{
				mGifController.Play();
			}
			else if (mAppIconModel.ImageName != null)
			{
				ImageSource val = (ImageSource)new BitmapImage(new Uri(mAppIconModel.ImageName));
				ImageBehavior.SetAnimatedSource((Image)(object)mAppImage, val);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in animating appicon for package " + mAppIconModel.PackageName + Environment.NewLine + ex.ToString());
		}
	}

	private void GifAppIconPause()
	{
		try
		{
			mGifController = ImageBehavior.GetAnimationController((Image)(object)mAppImage);
			mGifController.Pause();
		}
		catch (Exception ex)
		{
			Logger.Warning("Failed to pause gif. Err : " + ex.Message);
		}
	}

	private void DownloadingApp(AppIconDownloadingPhases downloadPhase)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected I4, but got Unknown
			AppIconDownloadingPhases val = downloadPhase;
			switch ((int)val)
			{
			case 0:
				ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home");
				((UIElement)mErrorGrid).Visibility = (Visibility)1;
				((UIElement)mProgressGrid).Visibility = (Visibility)0;
				break;
			case 1:
				((UIElement)mErrorGrid).Visibility = (Visibility)0;
				break;
			case 2:
				((UIElement)mProgressGrid).Visibility = (Visibility)0;
				break;
			case 3:
				((UIElement)mProgressGrid).Visibility = (Visibility)1;
				((UIElement)mBusyGrid).Visibility = (Visibility)0;
				break;
			case 4:
				ShowAppUninstallButton(isShow: false);
				((UIElement)mErrorGrid).Visibility = (Visibility)1;
				((UIElement)mBusyGrid).Visibility = (Visibility)0;
				break;
			case 5:
				if (!mAppIconModel.mIsAppInstalled)
				{
					((UIElement)mBusyGrid).Visibility = (Visibility)1;
					((UIElement)mErrorGrid).Visibility = (Visibility)0;
				}
				break;
			case 6:
				((UIElement)mBusyGrid).Visibility = (Visibility)1;
				mDownloader = null;
				mAppIconModel.mAppDownloadingEvent -= DownloadingApp;
				break;
			}
		}, new object[0]);
	}

	private void SetAppIconLocation(AppIconLocation iconLocation, double height, double width)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if ((int)iconLocation != 1)
		{
			if ((int)iconLocation == 2)
			{
				if (width > 68.0)
				{
					mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
				}
				else
				{
					mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
				}
				if (height < 68.0)
				{
					mMainGrid.RowDefinitions[1].Height = new GridLength(height);
				}
				GridLength val = default(GridLength);
				((GridLength)(ref val))._002Ector(0.0);
				mMainGrid.ColumnDefinitions[1].Width = val;
				mMainGrid.ColumnDefinitions[4].Width = val;
				mMainGrid.RowDefinitions[3].Height = val;
				mMainGrid.RowDefinitions[5].Height = val;
				((FrameworkElement)this).Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
				((FrameworkElement)mAppImage).Height = height;
				((FrameworkElement)mAppImage).Width = width;
				RectangleGeometry clip = new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
				((UIElement)mAppImage).Clip = (Geometry)(object)clip;
				((UIElement)AppNameTextBox).Visibility = (Visibility)2;
				mAppIconModel.AppNameTooltip = null;
			}
		}
		else
		{
			if (width > 68.0)
			{
				mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
			}
			else
			{
				mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
			}
			if (height < 68.0)
			{
				mMainGrid.RowDefinitions[1].Height = new GridLength(height);
			}
			GridLength width2 = default(GridLength);
			((GridLength)(ref width2))._002Ector(0.0);
			mMainGrid.ColumnDefinitions[1].Width = width2;
			mMainGrid.ColumnDefinitions[4].Width = width2;
			((FrameworkElement)this).Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
			((FrameworkElement)mAppImage).Height = height;
			((FrameworkElement)mAppImage).Width = width;
			RectangleGeometry clip2 = new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
			((UIElement)mAppImage).Clip = (Geometry)(object)clip2;
			((UIElement)AppNameTextBox).Visibility = (Visibility)2;
			mAppIconModel.AppNameTooltip = null;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/appiconui.xaml", UriKind.Relative);
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
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((ButtonBase)(AppIconUI)target).Click += new RoutedEventHandler(Button_Click);
			((UIElement)(AppIconUI)target).MouseEnter += new MouseEventHandler(Button_MouseEnter);
			((UIElement)(AppIconUI)target).MouseLeave += new MouseEventHandler(Button_MouseLeave);
			((FrameworkElement)(AppIconUI)target).Loaded += new RoutedEventHandler(Button_Loaded);
			((UIElement)(AppIconUI)target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Button_PreviewMouseLeftButtonDown);
			((UIElement)(AppIconUI)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Button_PreviewMouseLeftButtonUp);
			break;
		case 2:
			mMainGrid = (Grid)target;
			break;
		case 3:
			mImageGrid = (Grid)target;
			break;
		case 4:
			mAppImageBorder = (Border)target;
			break;
		case 5:
			mAppImage = (CustomPictureBox)target;
			break;
		case 6:
			mProgressGrid = (Grid)target;
			break;
		case 7:
			CustomProgressBar = (BlueProgressBar)target;
			break;
		case 8:
			mBusyGrid = (Grid)target;
			break;
		case 9:
			mBusyImage = (CustomPictureBox)target;
			break;
		case 10:
			mErrorGrid = (Grid)target;
			break;
		case 11:
			mRetryGrid = (Grid)target;
			break;
		case 12:
			mSuggestedAppPromotionImage = (CustomPictureBox)target;
			break;
		case 13:
			mUnInstallTabButton = (CustomPictureBox)target;
			break;
		case 14:
			mGl3ErrorIcon = (CustomPictureBox)target;
			break;
		case 15:
			mGl3InfoIcon = (CustomPictureBox)target;
			break;
		case 16:
			mRedDotNotifIcon = (CustomPictureBox)target;
			break;
		case 17:
			AppNameTextBox = (TextBlock)target;
			((UIElement)AppNameTextBox).MouseEnter -= new MouseEventHandler(Button_MouseEnter);
			break;
		case 18:
			mGamePadGrid = (Grid)target;
			break;
		case 19:
			mGamepadIcon = (CustomPictureBox)target;
			((UIElement)mGamepadIcon).MouseEnter += new MouseEventHandler(GamepadIcon_MouseEnter);
			((UIElement)mGamepadIcon).MouseLeave += new MouseEventHandler(GamepadIcon_MouseLeave);
			break;
		case 20:
			mGamePadToolTipPopup = (CustomPopUp)target;
			break;
		case 21:
			mMaskBorder = (Border)target;
			break;
		case 22:
			mIconText = (TextBlock)target;
			break;
		case 23:
			mUpArrow = (Path)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
