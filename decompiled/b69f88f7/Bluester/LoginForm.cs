using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Guna.UI2.WinForms;
using Microsoft.Win32;

namespace Bluester;

public class LoginForm : Form
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static FormClosingEventHandler _003C_003E9__5_0;

		internal void _003C_002Ector_003Eb__5_0(object s, FormClosingEventArgs e)
		{
			Environment.Exit(0);
		}
	}

	private Guna2BorderlessForm guna2BorderlessForm1;

	private IContainer components;

	private Guna2AnimateWindow guna2AnimateWindow1;

	private Guna2ShadowForm guna2ShadowForm1;

	private Guna2Panel guna2Panel1;

	private Label lblTitle;

	private Label lblSubtitle;

	private Guna2TextBox txtAuthKey;

	private Guna2Button btnLogin;

	private readonly SynchronizationContext context;

	private InstanceRegistry _instanceRegistry;

	private readonly Color _backColor = Color.FromArgb(24, 24, 24);

	private readonly Color _inputColor = Color.FromArgb(32, 32, 32);

	private readonly Color _borderColor = Color.FromArgb(60, 60, 60);

	private readonly Color _accentColor = Color.FromArgb(236, 236, 236);

	private readonly Color _textColor = Color.FromArgb(200, 200, 200);

	public LoginForm()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		SynchronizationContext current = SynchronizationContext.Current;
		context = current;
		_instanceRegistry = new InstanceRegistry("Android", "bgp64");
		InitializeComponent();
		LocalizeTexts();
		((Control)txtAuthKey).Text = LoadKeyFromRegistry();
		((Form)this).Shown += LoginForm_Shown;
		object obj = _003C_003Ec._003C_003E9__5_0;
		if (obj == null)
		{
			FormClosingEventHandler val = delegate
			{
				Environment.Exit(0);
			};
			_003C_003Ec._003C_003E9__5_0 = val;
			obj = (object)val;
		}
		((Form)this).FormClosing += (FormClosingEventHandler)obj;
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Expected O, but got Unknown
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Expected O, but got Unknown
		components = new Container();
		guna2BorderlessForm1 = new Guna2BorderlessForm(components);
		guna2AnimateWindow1 = new Guna2AnimateWindow(components);
		guna2ShadowForm1 = new Guna2ShadowForm(components);
		guna2Panel1 = new Guna2Panel();
		btnLogin = new Guna2Button();
		txtAuthKey = new Guna2TextBox();
		lblSubtitle = new Label();
		lblTitle = new Label();
		((Control)guna2Panel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)this).BackColor = _backColor;
		((Form)this).ClientSize = new Size(330, 240);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Form)this).ShowIcon = false;
		((Control)this).Name = "LoginForm";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "BLUESTER";
		guna2BorderlessForm1.BorderRadius = 12;
		guna2BorderlessForm1.ContainerControl = (ContainerControl)(object)this;
		guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6;
		guna2BorderlessForm1.ResizeForm = false;
		guna2BorderlessForm1.TransparentWhileDrag = true;
		guna2BorderlessForm1.ShadowColor = Color.Black;
		guna2AnimateWindow1.AnimationType = (AnimateWindowType)524288;
		guna2AnimateWindow1.Interval = 200;
		guna2AnimateWindow1.TargetForm = (Form)(object)this;
		guna2ShadowForm1.BorderRadius = 12;
		guna2ShadowForm1.TargetForm = (Form)(object)this;
		((Control)guna2Panel1).Controls.Add((Control)(object)btnLogin);
		((Control)guna2Panel1).Controls.Add((Control)(object)txtAuthKey);
		((Control)guna2Panel1).Controls.Add((Control)(object)lblSubtitle);
		((Control)guna2Panel1).Controls.Add((Control)(object)lblTitle);
		((Control)guna2Panel1).Location = new Point(0, 24);
		((Control)guna2Panel1).Name = "guna2Panel1";
		((Control)guna2Panel1).Size = new Size(330, 216);
		((Control)guna2Panel1).TabIndex = 1;
		((Control)lblTitle).ForeColor = _accentColor;
		((Control)lblTitle).Font = new Font("Bahnschrift", 28f, (FontStyle)1);
		((Control)lblTitle).Location = new Point(0, 0);
		((Control)lblTitle).Name = "lblTitle";
		((Control)lblTitle).Size = new Size(330, 50);
		((Control)lblTitle).TabIndex = 0;
		((Control)lblTitle).Text = "BLUESTER";
		lblTitle.TextAlign = (ContentAlignment)32;
		((Control)lblSubtitle).Font = new Font("Bahnschrift", 9.5f, (FontStyle)0);
		((Control)lblSubtitle).ForeColor = Color.FromArgb(120, 120, 120);
		((Control)lblSubtitle).Location = new Point(0, 50);
		((Control)lblSubtitle).Name = "lblSubtitle";
		((Control)lblSubtitle).Size = new Size(330, 20);
		((Control)lblSubtitle).TabIndex = 1;
		((Control)lblSubtitle).Text = "Authentication";
		lblSubtitle.TextAlign = (ContentAlignment)32;
		txtAuthKey.Animated = true;
		txtAuthKey.BorderColor = _borderColor;
		txtAuthKey.BorderRadius = 8;
		((Control)txtAuthKey).Cursor = Cursors.IBeam;
		txtAuthKey.DefaultText = "";
		txtAuthKey.FillColor = _inputColor;
		txtAuthKey.FocusedState.BorderColor = _accentColor;
		((Control)txtAuthKey).Font = new Font("Bahnschrift", 10.5f);
		((Control)txtAuthKey).ForeColor = Color.White;
		txtAuthKey.HoverState.BorderColor = Color.Gray;
		((Control)txtAuthKey).Location = new Point(30, 90);
		((Control)txtAuthKey).Name = "txtAuthKey";
		txtAuthKey.PlaceholderForeColor = Color.Gray;
		txtAuthKey.PlaceholderText = "Key";
		txtAuthKey.SelectedText = "";
		((Control)txtAuthKey).Size = new Size(270, 42);
		((Control)txtAuthKey).TabIndex = 2;
		txtAuthKey.TextAlign = (HorizontalAlignment)2;
		txtAuthKey.TextOffset = new Point(0, 1);
		((Control)txtAuthKey).KeyDown += new KeyEventHandler(txtAuthKey_KeyDown);
		btnLogin.Animated = true;
		btnLogin.BorderRadius = 8;
		((Control)btnLogin).Cursor = Cursors.Hand;
		btnLogin.FillColor = _accentColor;
		((Control)btnLogin).Font = new Font("Bahnschrift", 10.5f, (FontStyle)1);
		((Control)btnLogin).ForeColor = Color.Black;
		((Control)btnLogin).Location = new Point(30, 150);
		((Control)btnLogin).Name = "btnLogin";
		btnLogin.HoverState.FillColor = Color.White;
		btnLogin.PressedColor = Color.Gainsboro;
		((Control)btnLogin).Size = new Size(270, 42);
		((Control)btnLogin).TabIndex = 3;
		((Control)btnLogin).Text = "Login";
		((Control)btnLogin).Click += btnLogin_Click;
		((Control)this).Controls.Add((Control)(object)guna2Panel1);
		((Control)guna2Panel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void ExecuteAction()
	{
		string text = ((Control)txtAuthKey).Text;
		if (string.IsNullOrWhiteSpace(text))
		{
			string msg = (IsRussian() ? "Пожалуйста, введите ключ!" : "Please enter your key!");
			ShowNotification(msg);
			return;
		}
		((Control)btnLogin).Enabled = false;
		((Control)btnLogin).Text = (IsRussian() ? "Проверка..." : "Checking...");
		Tuple<bool, string> tuple = Utils.Authenticate(text);
		if (tuple.Item1)
		{
			SaveKeyToRegistry(text);
			string msg2 = (IsRussian() ? "Успешный вход!" : "Login successful!");
			ShowNotification(msg2);
			((Control)this).Hide();
			App.RunMain();
			return;
		}
		string text2 = tuple.Item2;
		bool flag = IsRussian();
		if (text2.StartsWith("Failed to connect") || text2.Contains("Не удалось подключиться") || text2.Contains("Server returned an error") || string.IsNullOrEmpty(text2))
		{
			text2 = (flag ? "Нет соединения с сервером." : "Connection failed.");
		}
		else if (text2.Contains("Неверный HWID"))
		{
			text2 = (flag ? "Ключ привязан к другому ПК." : "Key bound to another PC.");
		}
		else if (text2.Contains("Ключ не найден"))
		{
			text2 = (flag ? "Неверный ключ." : "Invalid key.");
		}
		ShowNotification(text2);
		((Control)btnLogin).Enabled = true;
		((Control)btnLogin).Text = (flag ? "Авторизоваться" : "Login");
	}

	private void btnLogin_Click(object sender, EventArgs e)
	{
		ExecuteAction();
	}

	private void txtAuthKey_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.KeyCode == 13)
		{
			ExecuteAction();
			e.Handled = true;
			e.SuppressKeyPress = true;
		}
	}

	private string LoadKeyFromRegistry()
	{
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\BluesterAuth");
			if (registryKey != null)
			{
				return registryKey.GetValue("Key", "").ToString();
			}
		}
		catch
		{
		}
		return "";
	}

	private void SaveKeyToRegistry(string keyToSave)
	{
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\BLUESTERAuth");
			registryKey?.SetValue("Key", keyToSave);
		}
		catch
		{
		}
	}

	private void ShowNotification(string msg)
	{
		context.Post(delegate
		{
			((Control)new NotificationForm(msg)).Show();
		}, null);
	}

	private async void LoginForm_Shown(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(((Control)txtAuthKey).Text))
		{
			await Task.Delay(300);
			ExecuteAction();
		}
	}

	private void LocalizeTexts()
	{
		string text = _instanceRegistry.Locale ?? "";
		bool flag = text.Equals("ru", StringComparison.OrdinalIgnoreCase) || text.Equals("ru-RU", StringComparison.OrdinalIgnoreCase);
		((Control)lblTitle).Text = "BLUESTER";
		((Control)lblSubtitle).Text = (flag ? "Введите ключ для продолжения" : "Enter your key to continue");
		((Control)btnLogin).Text = (flag ? "Авторизоваться" : "Login");
		txtAuthKey.PlaceholderText = (flag ? "Ваш ключ" : "Your License Key");
	}

	private bool IsRussian()
	{
		return (_instanceRegistry.Locale ?? "").Equals("ru-RU", StringComparison.OrdinalIgnoreCase);
	}
}
