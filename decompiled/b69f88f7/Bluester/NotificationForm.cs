using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlueStacks.Common;
using Guna.UI2.WinForms;

namespace Bluester;

public class NotificationForm : Form
{
	private Guna2BorderlessForm guna2BorderlessForm1;

	private Guna2HtmlLabel guna2HtmlLabel1;

	private IContainer components;

	private Guna2PictureBox guna2PictureBox1;

	private Timer progressTimer;

	private float progress;

	private DateTime animationStartTime;

	private readonly Color _backColor = Color.FromArgb(24, 24, 24);

	private readonly Color _borderColor = Color.FromArgb(60, 60, 60);

	private readonly Color _textColor = Color.FromArgb(236, 236, 236);

	private readonly Color _progressStart = Color.FromArgb(210, 210, 210);

	private readonly Color _progressEnd = Color.FromArgb(245, 245, 245);

	private const int CornerRadius = 12;

	protected override bool ShowWithoutActivation => true;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = ((Form)this).CreateParams;
			createParams.ExStyle |= 8;
			return createParams;
		}
	}

	public NotificationForm(string message)
	{
		InitializeComponent();
		SetupNotification(message);
		((Control)this).DoubleBuffered = true;
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).BackColor = _backColor;
		if (!((Control)this).IsHandleCreated)
		{
			((Control)this).HandleCreated += delegate
			{
				InitializeTimers();
			};
		}
		else
		{
			InitializeTimers();
		}
		ApplyRoundedCorners(12);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Expected O, but got Unknown
		components = new Container();
		guna2BorderlessForm1 = new Guna2BorderlessForm(components);
		guna2HtmlLabel1 = new Guna2HtmlLabel();
		guna2PictureBox1 = new Guna2PictureBox();
		((ISupportInitialize)guna2PictureBox1).BeginInit();
		((Control)this).SuspendLayout();
		guna2BorderlessForm1.BorderRadius = 12;
		guna2BorderlessForm1.ContainerControl = (ContainerControl)(object)this;
		guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.5;
		guna2BorderlessForm1.DragForm = false;
		guna2BorderlessForm1.HasFormShadow = true;
		guna2BorderlessForm1.ShadowColor = Color.Black;
		((Control)guna2HtmlLabel1).BackColor = Color.Transparent;
		((Control)guna2HtmlLabel1).Font = new Font("Bahnschrift", 12f, (FontStyle)1);
		((Control)guna2HtmlLabel1).ForeColor = _textColor;
		guna2HtmlLabel1.IsSelectionEnabled = false;
		((Control)guna2HtmlLabel1).AutoSize = false;
		guna2HtmlLabel1.TextAlignment = (ContentAlignment)16;
		((Control)guna2HtmlLabel1).Location = new Point(55, 0);
		((Control)guna2HtmlLabel1).Name = "guna2HtmlLabel1";
		((Control)guna2HtmlLabel1).Size = new Size(295, 60);
		((Control)guna2HtmlLabel1).TabIndex = 0;
		((Control)guna2HtmlLabel1).Text = "Notification Message";
		((Control)guna2PictureBox1).BackColor = Color.Transparent;
		guna2PictureBox1.ImageRotate = 0f;
		((PictureBox)guna2PictureBox1).InitialImage = null;
		((Control)guna2PictureBox1).Location = new Point(15, 15);
		((Control)guna2PictureBox1).Name = "guna2PictureBox1";
		string text = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client", "Assets", "ProductLogo.png");
		if (File.Exists(text))
		{
			((PictureBox)guna2PictureBox1).Image = Image.FromFile(text);
		}
		((Control)guna2PictureBox1).Size = new Size(30, 30);
		((PictureBox)guna2PictureBox1).SizeMode = (PictureBoxSizeMode)1;
		((PictureBox)guna2PictureBox1).TabIndex = 1;
		((PictureBox)guna2PictureBox1).TabStop = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = _backColor;
		((Form)this).ClientSize = new Size(360, 60);
		((Control)this).Controls.Add((Control)(object)guna2PictureBox1);
		((Control)this).Controls.Add((Control)(object)guna2HtmlLabel1);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).Name = "NotificationForm";
		((Form)this).Opacity = 0.0;
		((Control)this).Text = "Notification";
		((Control)this).Paint += new PaintEventHandler(NotificationForm_Paint);
		((ISupportInitialize)guna2PictureBox1).EndInit();
		((Control)this).ResumeLayout(false);
	}

	private void SetupNotification(string message)
	{
		((Control)guna2HtmlLabel1).Text = message;
		((Form)this).StartPosition = (FormStartPosition)0;
		((Form)this).Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - ((Control)this).Width - 15, Screen.PrimaryScreen.WorkingArea.Height - ((Control)this).Height - 15);
		((Form)this).TopMost = true;
		((Form)this).ShowInTaskbar = false;
	}

	private void InitializeTimers()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		progressTimer = new Timer
		{
			Interval = 15
		};
		progressTimer.Tick += ProgressTimer_Tick;
		((Control)this).BeginInvoke((Delegate)(Action)async delegate
		{
			await FadeIn();
			animationStartTime = DateTime.Now;
			progressTimer.Start();
		});
	}

	protected override void WndProc(ref Message m)
	{
		if (((Message)(ref m)).Msg == 132)
		{
			((Message)(ref m)).Result = (IntPtr)1;
		}
		else
		{
			((Form)this).WndProc(ref m);
		}
	}

	private async Task FadeIn()
	{
		int steps = 15;
		for (int i = 0; i <= steps; i++)
		{
			((Form)this).Opacity = 1f * ((float)i / (float)steps);
			await Task.Delay(20);
		}
	}

	private async Task FadeOut()
	{
		int steps = 15;
		for (int i = steps; i >= 0; i--)
		{
			((Form)this).Opacity = 1f * ((float)i / (float)steps);
			await Task.Delay(20);
		}
	}

	private async void ProgressTimer_Tick(object sender, EventArgs e)
	{
		float num = (float)(DateTime.Now - animationStartTime).TotalSeconds;
		float num2 = 3.5f;
		progress = num / num2;
		if (progress >= 1f)
		{
			progressTimer.Stop();
			await FadeOut();
			((Form)this).Close();
			((Component)this).Dispose();
		}
		else
		{
			((Control)this).Invalidate(new Rectangle(0, ((Control)this).Height - 5, ((Control)this).Width, 5));
		}
	}

	private void NotificationForm_Paint(object sender, PaintEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		e.Graphics.SmoothingMode = (SmoothingMode)4;
		Pen val = new Pen(_borderColor, 1f);
		try
		{
			Rectangle rect = new Rectangle(0, 0, ((Control)this).Width - 1, ((Control)this).Height - 1);
			GraphicsPath roundedPath = GetRoundedPath(rect, 12);
			try
			{
				e.Graphics.DrawPath(val, roundedPath);
			}
			finally
			{
				((IDisposable)roundedPath)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		int num = 4;
		int num2 = ((Control)this).Height - num;
		float num3 = (float)((Control)this).Width * (1f - progress);
		if (!(num3 > 0f))
		{
			return;
		}
		RectangleF rectangleF = new RectangleF(0f, num2, num3, num);
		LinearGradientBrush val2 = new LinearGradientBrush(rectangleF, _progressStart, _progressEnd, 0f);
		try
		{
			GraphicsPath roundedPath2 = GetRoundedPath(new Rectangle(0, 0, ((Control)this).Width, ((Control)this).Height), 12);
			try
			{
				e.Graphics.SetClip(roundedPath2);
				e.Graphics.FillRectangle((Brush)(object)val2, rectangleF);
				e.Graphics.ResetClip();
			}
			finally
			{
				((IDisposable)roundedPath2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void ApplyRoundedCorners(int radius)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		((Control)this).Region = new Region(GetRoundedPath(new Rectangle(0, 0, ((Control)this).Width, ((Control)this).Height), radius));
	}

	protected override void OnResize(EventArgs e)
	{
		((Form)this).OnResize(e);
		ApplyRoundedCorners(12);
		((Control)this).Invalidate();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		GraphicsPath val = new GraphicsPath();
		int num = radius * 2;
		val.AddArc(rect.X, rect.Y, num, num, 180f, 90f);
		val.AddArc(rect.Right - num, rect.Y, num, num, 270f, 90f);
		val.AddArc(rect.Right - num, rect.Bottom - num, num, num, 0f, 90f);
		val.AddArc(rect.X, rect.Bottom - num, num, num, 90f, 90f);
		val.CloseFigure();
		return val;
	}
}
