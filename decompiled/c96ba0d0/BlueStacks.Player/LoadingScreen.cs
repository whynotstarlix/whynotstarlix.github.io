using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class LoadingScreen : UserControl
{
	public class NewProgressBar : ProgressBar
	{
		internal enum BarType
		{
			Progress,
			Marquee
		}

		internal BarType barType;

		private SolidBrush baseBrush;

		private SolidBrush backBrush;

		private SolidBrush foreBrush;

		private int marqueeStart;

		public NewProgressBar(string type)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected Obj, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected Obj, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected Obj, but got Unknown
			((Control)this).SetStyle((ControlStyles)2, true);
			((Control)this).SetStyle((ControlStyles)65536, true);
			((Control)this).SetStyle((ControlStyles)8192, true);
			barType = (BarType)Enum.Parse(typeof(BarType), type, ignoreCase: true);
			Color color = Color.FromArgb(35, 147, 213);
			baseBrush = new SolidBrush(color);
			Color color2 = Color.FromArgb(195, 195, 193);
			backBrush = new SolidBrush(color2);
			Color color3 = Color.FromArgb(21, 83, 120);
			foreBrush = new SolidBrush(color3);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle clipRectangle = e.ClipRectangle;
			FillRectangle(e, (Brush)(object)baseBrush, 0, 0, clipRectangle.Width, 1);
			FillRectangle(e, (Brush)(object)backBrush, 0, 1, clipRectangle.Width, clipRectangle.Height - 2);
			FillRectangle(e, (Brush)(object)baseBrush, 0, clipRectangle.Height - 1, clipRectangle.Width, 1);
			switch (barType)
			{
			case BarType.Progress:
			{
				int width = (int)((double)clipRectangle.Width * ((double)((ProgressBar)this).Value / (double)((ProgressBar)this).Maximum));
				FillRectangle(e, (Brush)(object)foreBrush, 0, 0, width, clipRectangle.Height);
				break;
			}
			case BarType.Marquee:
				FillRectangle(e, (Brush)(object)foreBrush, marqueeStart, 0, 96, clipRectangle.Height);
				marqueeStart += 10;
				if (marqueeStart >= clipRectangle.Width - 20)
				{
					marqueeStart = 0;
				}
				break;
			}
		}

		private void FillRectangle(PaintEventArgs e, Brush brush, int x, int y, int width, int height)
		{
			e.Graphics.FillRectangle(brush, x, y, width, height);
		}
	}

	public class AppNameText : Label
	{
		public AppNameText()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected Obj, but got Unknown
			((Control)this).Font = new Font(Utils.GetSystemFontName(), 18f, (FontStyle)1);
			((Control)this).Height = 36;
		}

		protected override void OnPaint(PaintEventArgs evt)
		{
			evt.Graphics.TextRenderingHint = (TextRenderingHint)4;
			((Label)this).OnPaint(evt);
		}
	}

	public class StatusText : Label
	{
		public StatusText()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected Obj, but got Unknown
			((Control)this).Font = new Font(Utils.GetSystemFontName(), 12f, (FontStyle)0);
			((Control)this).Height = 24;
		}

		protected override void OnPaint(PaintEventArgs evt)
		{
			evt.Graphics.TextRenderingHint = (TextRenderingHint)4;
			((Label)this).OnPaint(evt);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static EventHandler _003C_003E9__17_1;

		public static Action _003C_003E9__21_0;

		internal void _003C_002Ector_003Eb__17_1(object obj, EventArgs evt)
		{
			((Form)VMWindow.Instance).Close();
		}

		internal void _003CRemoveLoadingScreen_003Eb__21_0()
		{
			((Control)VMWindow.Instance).SuspendLayout();
			mLoadingScreen.progressTimer.Stop();
			((Control)mLoadingScreen).Hide();
			while (((ArrangedElementCollection)((Control)mLoadingScreen).Controls).Count > 0)
			{
				((Component)(object)((Control)mLoadingScreen).Controls[0]).Dispose();
			}
			((Control)VMWindow.Instance).Controls.Remove((Control)(object)mLoadingScreen);
			((Component)(object)mLoadingScreen).Dispose();
			mLoadingScreen = null;
			((Control)VMWindow.Instance).ResumeLayout();
		}
	}

	private Image splashLogoImage;

	private Image whiteLogoImage;

	private Image closeButtonImage;

	private Image fullScreenButtonImage;

	private NewProgressBar progressBar;

	private Label statusText;

	private Label closeButton;

	private Label fullScreenButton;

	private Label appLogo;

	private Label whiteLogo;

	private string installDir;

	private string imageDir;

	private Label appNameText = (Label)(object)new AppNameText();

	private Timer progressTimer;

	private bool isFullScreen;

	private List<string> mLstDynamicString;

	internal static LoadingScreen mLoadingScreen;

	public LoadingScreen(Point loadingScreenLocation, Size loadingScreenSize, string barType, bool isFullScreenButtonVisible)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected Obj, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Expected Obj, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected Obj, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Expected Obj, but got Unknown
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Expected Obj, but got Unknown
		Logger.Info("LoadingScreen({0})", new object[1] { loadingScreenSize });
		SetImageDir();
		LoadImages();
		((Control)this).SetStyle((ControlStyles)139264, true);
		((Control)this).Location = loadingScreenLocation;
		((Control)this).Size = loadingScreenSize;
		((Control)this).BackColor = Color.FromArgb(35, 147, 213);
		appLogo = new Label();
		Logger.Info("Using splash logo");
		((Control)appLogo).BackgroundImage = splashLogoImage;
		((Control)appLogo).Width = ((Control)appLogo).BackgroundImage.Width;
		((Control)appLogo).Height = ((Control)appLogo).BackgroundImage.Height;
		((Control)appLogo).BackColor = Color.Transparent;
		installDir = RegistryStrings.InstallDir;
		string text = "LoadingScreenAppTitle";
		if (text.StartsWith("DynamicText"))
		{
			mLstDynamicString = new List<string>(text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries));
			if (mLstDynamicString.Count > 0)
			{
				mLstDynamicString.Remove("DynamicText");
				Timer timer = new Timer(5000.0);
				timer.Elapsed += TimerElapsed;
				text = mLstDynamicString[0];
				timer.Start();
			}
		}
		((Control)appNameText).Text = text;
		appNameText.TextAlign = (ContentAlignment)32;
		((Control)appNameText).Width = loadingScreenSize.Width;
		((Control)appNameText).Height = 50;
		appNameText.UseMnemonic = false;
		((Control)appNameText).ForeColor = Color.White;
		((Control)appNameText).BackColor = Color.Transparent;
		StatusText statusText = new StatusText();
		((Label)statusText).TextAlign = (ContentAlignment)32;
		((Control)statusText).Width = loadingScreenSize.Width;
		((Control)statusText).Height = 40;
		((Label)statusText).UseMnemonic = false;
		((Control)statusText).ForeColor = Color.White;
		((Control)statusText).BackColor = Color.Transparent;
		this.statusText = (Label)(object)statusText;
		NewProgressBar newProgressBar = new NewProgressBar(barType);
		((Control)newProgressBar).Width = 336;
		((Control)newProgressBar).Height = 10;
		((ProgressBar)newProgressBar).Value = 0;
		progressBar = newProgressBar;
		if (barType == "Marquee")
		{
			progressTimer = new Timer
			{
				Interval = 50
			};
			progressTimer.Tick += (object obj, EventArgs evt) =>
			{
				((Control)progressBar).Invalidate();
			};
			progressTimer.Start();
		}
		whiteLogo = new Label
		{
			BackgroundImage = whiteLogoImage,
			BackgroundImageLayout = (ImageLayout)3,
			Width = 48,
			Height = 44,
			BackColor = Color.Transparent
		};
		closeButton = new Label
		{
			BackgroundImage = closeButtonImage,
			BackgroundImageLayout = (ImageLayout)3,
			Width = 24,
			Height = 24,
			BackColor = Color.Transparent
		};
		((Control)closeButton).Click += (object obj, EventArgs evt) =>
		{
			((Form)VMWindow.Instance).Close();
		};
		((Control)closeButton).Visible = false;
		fullScreenButton = new Label
		{
			BackgroundImage = fullScreenButtonImage,
			BackgroundImageLayout = (ImageLayout)3,
			Width = 24,
			Height = 24,
			BackColor = Color.Transparent
		};
		((Control)fullScreenButton).Click += (object obj, EventArgs evt) =>
		{
			LayoutManager.ToggleFullScreen();
			FullScreenToggled();
		};
		if (!isFullScreenButtonVisible)
		{
			((Control)fullScreenButton).Visible = false;
		}
		SetLocations();
		((Control)this).Controls.Add((Control)(object)appLogo);
		((Control)this).Controls.Add((Control)(object)appNameText);
		((Control)this).Controls.Add((Control)(object)progressBar);
		((Control)this).Controls.Add((Control)(object)this.statusText);
		((Control)this).Controls.Add((Control)(object)whiteLogo);
		((Control)this).Controls.Add((Control)(object)closeButton);
		((Control)this).Controls.Add((Control)(object)fullScreenButton);
		((Control)whiteLogo).Visible = false;
	}

	private void SetLocations()
	{
		int num = ((Control)this).Size.Width / 2;
		int num2 = ((Control)this).Size.Height / 2;
		Logger.Info("centerX: {0}, centerY: {1}", new object[2] { num, num2 });
		int num3 = ((Control)appLogo).Height + 30 + ((Control)appNameText).Height + 50 + ((Control)progressBar).Height + 20 + ((Control)statusText).Height;
		int y = num2 - num3 / 2;
		((Control)appLogo).Location = new Point(num - ((Control)appLogo).Width / 2, y);
		((Control)appNameText).Location = new Point(0, ((Control)appLogo).Bottom + 30);
		((Control)progressBar).Location = new Point(num - ((Control)progressBar).Width / 2, ((Control)appNameText).Bottom + 50);
		((Control)statusText).Location = new Point(0, ((Control)progressBar).Bottom + 20);
		((Control)whiteLogo).Location = new Point(num - ((Control)whiteLogo).Width / 2, ((Control)this).Height - ((Control)whiteLogo).Height - 20);
		((Control)closeButton).Location = new Point(((Control)this).Width - ((Control)closeButton).Width - 30, 30);
		((Control)fullScreenButton).Location = new Point(((Control)closeButton).Left - 10 - ((Control)fullScreenButton).Width, 30);
	}

	private void FullScreenToggled()
	{
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected Obj, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Expected Obj, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected Obj, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected Obj, but got Unknown
		isFullScreen = !isFullScreen;
		((Control)closeButton).Visible = !((Control)closeButton).Visible;
		((Control)this).Size = ((Form)VMWindow.Instance).ClientSize;
		if (isFullScreen)
		{
			double num = (double)((Form)VMWindow.Instance).ClientSize.Width / (double)LayoutManager.mConfiguredDisplaySize.Width;
			double num2 = (double)((Form)VMWindow.Instance).ClientSize.Height / (double)LayoutManager.mConfiguredDisplaySize.Height;
			((Control)appLogo).Width = (int)((double)((Control)appLogo).Width * num);
			((Control)appLogo).Height = (int)((double)((Control)appLogo).Height * num);
			splashLogoImage = (Image)new Bitmap((Image)new Bitmap(RegistryStrings.ProductImageCompletePath), new Size(((Control)appLogo).Width, ((Control)appLogo).Height));
			((Control)appLogo).BackgroundImage = splashLogoImage;
			((Control)appNameText).Width = ((Control)mLoadingScreen).Width;
			((Control)appNameText).Height = (int)((double)((Control)appNameText).Height * num2);
			((Control)statusText).Width = ((Control)mLoadingScreen).Width;
			((Control)statusText).Height = (int)((double)((Control)statusText).Height * num2);
			((Control)progressBar).Width = (int)((double)((Control)progressBar).Width * num);
			((Control)progressBar).Height = (int)((double)((Control)progressBar).Height * num2);
			((Control)whiteLogo).Width = (int)((double)((Control)whiteLogo).Width * num);
			((Control)whiteLogo).Height = (int)((double)((Control)whiteLogo).Height * num2);
			((Control)fullScreenButton).Width = (int)((double)((Control)fullScreenButton).Width * num);
			((Control)fullScreenButton).Height = (int)((double)((Control)fullScreenButton).Height * num2);
			((Control)closeButton).Width = (int)((double)((Control)closeButton).Width * num);
			((Control)closeButton).Height = (int)((double)((Control)closeButton).Height * num2);
		}
		else
		{
			splashLogoImage = (Image)new Bitmap((Image)new Bitmap(RegistryStrings.ProductImageCompletePath), new Size(128, 128));
			((Control)appLogo).BackgroundImage = splashLogoImage;
			((Control)appLogo).Width = splashLogoImage.Width;
			((Control)appLogo).Height = splashLogoImage.Height;
			((Control)appNameText).Width = ((Control)mLoadingScreen).Width;
			((Control)appNameText).Height = 50;
			((Control)statusText).Width = ((Control)mLoadingScreen).Width;
			((Control)statusText).Height = 40;
			((Control)progressBar).Width = 336;
			((Control)progressBar).Height = 10;
			((Control)whiteLogo).Width = 48;
			((Control)whiteLogo).Height = 44;
			((Control)fullScreenButton).Width = 24;
			((Control)fullScreenButton).Height = 24;
			((Control)closeButton).Width = 24;
			((Control)closeButton).Height = 24;
		}
		SetLocations();
	}

	internal static void AddLoadingScreen(string barType = "Marquee")
	{
		Logger.Info("AddLoadingScreen: " + barType);
		if (mLoadingScreen != null && ((Control)VMWindow.Instance).Controls.Contains((Control)(object)mLoadingScreen))
		{
			Logger.Info("Already added");
			return;
		}
		Logger.Info("In Loading Screen");
		Size loadingScreenSize = new Size
		{
			Height = ((Form)VMWindow.Instance).ClientSize.Height,
			Width = ((Form)VMWindow.Instance).ClientSize.Width
		};
		mLoadingScreen = new LoadingScreen(new Point(0, 0), loadingScreenSize, barType, isFullScreenButtonVisible: false);
		((Control)VMWindow.Instance).SuspendLayout();
		((Control)VMWindow.Instance).Controls.Add((Control)(object)mLoadingScreen);
		mLoadingScreen.SetStatusText(LocaleStrings.GetLocalizedString("STRING_INITIALIZING", ""));
		((Control)VMWindow.Instance).ResumeLayout();
	}

	internal static void RemoveLoadingScreen()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		VMWindow instance = VMWindow.Instance;
		Action val = _003C_003Ec._003C_003E9__21_0;
		if (val == null)
		{
			Action val2 = () =>
			{
				((Control)VMWindow.Instance).SuspendLayout();
				mLoadingScreen.progressTimer.Stop();
				((Control)mLoadingScreen).Hide();
				while (((ArrangedElementCollection)((Control)mLoadingScreen).Controls).Count > 0)
				{
					((Component)(object)((Control)mLoadingScreen).Controls[0]).Dispose();
				}
				((Control)VMWindow.Instance).Controls.Remove((Control)(object)mLoadingScreen);
				((Component)(object)mLoadingScreen).Dispose();
				mLoadingScreen = null;
				((Control)VMWindow.Instance).ResumeLayout();
			};
			_003C_003Ec._003C_003E9__21_0 = val2;
			val = val2;
		}
		UIHelper.RunOnUIThread((Control)(object)instance, val);
	}

	private void TimerElapsed(object sender, ElapsedEventArgs e)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		try
		{
			UIHelper.RunOnUIThread((Control)(object)VMWindow.Instance, (Action)(() =>
			{
				if (mLstDynamicString != null && mLstDynamicString.Count > 0)
				{
					int num = 0;
					if (mLstDynamicString.Contains(((Control)appNameText).Text))
					{
						num = mLstDynamicString.IndexOf(((Control)appNameText).Text) + 1;
					}
					if (num == mLstDynamicString.Count)
					{
						num = 0;
					}
					((Control)appNameText).Text = mLstDynamicString[num];
				}
			}));
		}
		catch (Exception ex)
		{
			Logger.Info(ex.Message);
		}
	}

	public void SetStatusText(string text)
	{
		((Control)statusText).Text = text;
	}

	private void SetImageDir()
	{
		imageDir = RegistryStrings.InstallDir;
	}

	private void LoadImages()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected Obj, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected Obj, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected Obj, but got Unknown
		Logger.Info("imageDir = " + imageDir);
		Image val = (Image)new Bitmap(RegistryStrings.ProductImageCompletePath);
		splashLogoImage = (Image)new Bitmap(val, new Size(128, 128));
		whiteLogoImage = (Image)new Bitmap(Path.Combine(imageDir, "WhiteLogo.png"));
		closeButtonImage = (Image)new Bitmap(Path.Combine(imageDir, "XButton.png"));
		fullScreenButtonImage = (Image)new Bitmap(Path.Combine(imageDir, "WhiteFullScreen.png"));
	}

	public void UpdateProgressBar(int val)
	{
		((ProgressBar)progressBar).Value = val;
	}
}
