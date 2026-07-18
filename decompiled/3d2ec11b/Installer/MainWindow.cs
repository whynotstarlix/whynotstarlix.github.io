using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Installer;

public class MainWindow : Window, IComponentConnector
{
	private sealed class AuthResult
	{
		internal bool Success { get; set; }

		internal string Message { get; set; }
	}

	private static class AuthClient
	{
		internal static AuthResult Exchange(string key)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			string text = CreateMarker();
			string text2 = CreateMachineHash();
			try
			{
				HttpClient val = new HttpClient
				{
					Timeout = TimeSpan.FromSeconds(20.0)
				};
				try
				{
					FormUrlEncodedContent val2 = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string>
					{
						["key"] = key,
						["hwid"] = text2,
						["nonce"] = text,
						["app"] = "xantares",
						["protocol"] = "2"
					});
					try
					{
						HttpResponseMessage result = val.PostAsync("https://dantezhxf.alwaysdata.net/auth_v2.php", (HttpContent)(object)val2).GetAwaiter().GetResult();
						try
						{
							if (!result.IsSuccessStatusCode)
							{
								return Fail("network");
							}
							return Unpack(result.Content.ReadAsStringAsync().GetAwaiter().GetResult(), text2, text);
						}
						finally
						{
							((IDisposable)result)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch
			{
				return Fail("network");
			}
		}

		private static AuthResult Unpack(string response, string node, string marker)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if (string.IsNullOrWhiteSpace(response))
			{
				return Fail("empty");
			}
			JavaScriptSerializer val = new JavaScriptSerializer();
			Dictionary<string, object> bag = val.DeserializeObject(response) as Dictionary<string, object>;
			string value = PickText(bag, "payload");
			string text = PickText(bag, "signature");
			if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(text))
			{
				return Fail("bad_response");
			}
			byte[] array = Base64UrlDecode(value);
			byte[] signature = Convert.FromBase64String(text);
			if (!VerifySignature(array, signature))
			{
				return Fail("bad_signature");
			}
			Dictionary<string, object> bag2 = val.DeserializeObject(Encoding.UTF8.GetString(array)) as Dictionary<string, object>;
			if (!PickBool(bag2, "ok"))
			{
				return Fail(PickText(bag2, "message") ?? "denied");
			}
			if (!StringEquals(PickText(bag2, "hwid"), node) || !StringEquals(PickText(bag2, "nonce"), marker))
			{
				return Fail("mismatch");
			}
			DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(PickLong(bag2, "exp")).UtcDateTime;
			if (DateTime.UtcNow >= utcDateTime)
			{
				return Fail("expired");
			}
			return new AuthResult
			{
				Success = true,
				Message = "ok"
			};
		}

		private static bool VerifySignature(byte[] payloadBytes, byte[] signature)
		{
			using RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(3072);
			rSACryptoServiceProvider.PersistKeyInCsp = false;
			rSACryptoServiceProvider.FromXmlString("<RSAKeyValue><Modulus>rEqOuRMCITcRkESN8Ff3VLT4vlIUelfMJE+ZLDYXYiF/5hWRDkZFPa1P65+vO7HwU9ar75rb2duz/zbqIOpILJMa7K3P6greH+99M3duS9Fz+NSnwG5dHSOI/wodyYBIVz2WFcp6rzo7jGiIWB0i9dgHMn9FoBdFGIFDcXK9VeMY//QhoeZvhG01DZXfPeYgnehRJt1DlnZf/WVspbgyLUrKYWM+/n7po4ABLucIo3/p+G892tWE7L2vJI9BudJuu6sxnT3GmLJSGYvuL0lRzURzzceKa29Pt9SYBQ59ycrtfVdKUjCC4IgEiJVtvFGyH7ZaRqXF/g7yqC6YO9sNI7d1G/GHUEbZmBRbefFXsnRnCyrQnCo94MpTs65X2AJue/qXbpNiYtX3JIajOhRvxBE1u+tXgeQ+f34pUvykBAZtk7h/VcGZG1DBiXAzbZdh4h9fIdlQkrP6Izm+ADskhBhlg9A/W49IwAPoDajjIoxFfLx2P4e5Aa4ZjTgNaDgt</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
			return rSACryptoServiceProvider.VerifyData(payloadBytes, CryptoConfig.MapNameToOID("SHA256"), signature);
		}

		private static string CreateMachineHash()
		{
			string text = string.Empty;
			try
			{
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
				text = (registryKey?.GetValue("MachineGuid") as string) ?? string.Empty;
			}
			catch
			{
			}
			string s = Environment.MachineName + "|" + Environment.UserName + "|" + text;
			using SHA256 sHA = SHA256.Create();
			return ToHex(sHA.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}

		private static string CreateMarker()
		{
			byte[] array = new byte[24];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
			{
				randomNumberGenerator.GetBytes(array);
			}
			return Base64UrlEncode(array);
		}

		private static string Base64UrlEncode(byte[] value)
		{
			return Convert.ToBase64String(value).TrimEnd(new char[1] { '=' }).Replace('+', '-')
				.Replace('/', '_');
		}

		private static byte[] Base64UrlDecode(string value)
		{
			string text = value.Replace('-', '+').Replace('_', '/');
			switch (text.Length % 4)
			{
			case 2:
				text += "==";
				break;
			case 3:
				text += "=";
				break;
			}
			return Convert.FromBase64String(text);
		}

		private static string ToHex(byte[] data)
		{
			StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
			foreach (byte b in data)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private static string PickText(Dictionary<string, object> bag, string name)
		{
			if (bag == null || !bag.ContainsKey(name))
			{
				return null;
			}
			return (bag[name] as string) ?? Convert.ToString(bag[name]);
		}

		private static bool PickBool(Dictionary<string, object> bag, string name)
		{
			if (bag == null || !bag.ContainsKey(name))
			{
				return false;
			}
			object obj = bag[name];
			if (obj is bool)
			{
				return (bool)obj;
			}
			bool result;
			return bool.TryParse(Convert.ToString(obj), out result) && result;
		}

		private static long PickLong(Dictionary<string, object> bag, string name)
		{
			if (bag == null || !bag.ContainsKey(name))
			{
				return 0L;
			}
			object obj = bag[name];
			if (obj is long)
			{
				return (long)obj;
			}
			return Convert.ToInt64(obj);
		}

		private static bool StringEquals(string left, string right)
		{
			return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
		}

		private static AuthResult Fail(string message)
		{
			return new AuthResult
			{
				Success = false,
				Message = (message ?? string.Empty)
			};
		}
	}

	private const string KeyFilePath = "key.txt";

	private const string AuthUrl = "https://dantezhxf.alwaysdata.net/auth_v2.php";

	private const string CleanerUrl = "https://dantezhxf.alwaysdata.net/cleaner.exe";

	private const string EmulatorUrl = "https://dantezhxf.alwaysdata.net/XANTARES.exe";

	private const string HyperV11Url = "https://dantezhxf.alwaysdata.net/11FixHyperV.exe";

	private const string HyperV10Url = "https://dantezhxf.alwaysdata.net/10FixHyperV.exe";

	private const string PublicKeyXml = "<RSAKeyValue><Modulus>rEqOuRMCITcRkESN8Ff3VLT4vlIUelfMJE+ZLDYXYiF/5hWRDkZFPa1P65+vO7HwU9ar75rb2duz/zbqIOpILJMa7K3P6greH+99M3duS9Fz+NSnwG5dHSOI/wodyYBIVz2WFcp6rzo7jGiIWB0i9dgHMn9FoBdFGIFDcXK9VeMY//QhoeZvhG01DZXfPeYgnehRJt1DlnZf/WVspbgyLUrKYWM+/n7po4ABLucIo3/p+G892tWE7L2vJI9BudJuu6sxnT3GmLJSGYvuL0lRzURzzceKa29Pt9SYBQ59ycrtfVdKUjCC4IgEiJVtvFGyH7ZaRqXF/g7yqC6YO9sNI7d1G/GHUEbZmBRbefFXsnRnCyrQnCo94MpTs65X2AJue/qXbpNiYtX3JIajOhRvxBE1u+tXgeQ+f34pUvykBAZtk7h/VcGZG1DBiXAzbZdh4h9fIdlQkrP6Izm+ADskhBhlg9A/W49IwAPoDajjIoxFfLx2P4e5Aa4ZjTgNaDgt</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

	private readonly DispatcherTimer _authTimer;

	private bool _authInProgress;

	private string _acceptedKey;

	internal Border mainGrid;

	internal StackPanel LoginPanel;

	internal TextBox KeyTextInput;

	internal StackPanel MenuPanel;

	internal StackPanel ProgressPanel;

	internal TextBlock StatusText;

	internal ProgressBar DownloadProgress;

	private bool _contentLoaded;

	public MainWindow()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		InitializeComponent();
		_authTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(700.0)
		};
		_authTimer.Tick += async delegate
		{
			_authTimer.Stop();
			await TryAuthorizeAsync(showError: false);
		};
	}

	private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	{
		((UIElement)this).Opacity = 0.0;
		AnimateWindowIn();
		LoadKeyFromFile();
	}

	private void LoadKeyFromFile()
	{
		try
		{
			if (File.Exists("key.txt"))
			{
				KeyTextInput.Text = File.ReadAllText("key.txt").Trim();
				KeyTextInput.CaretIndex = KeyTextInput.Text.Length;
			}
		}
		catch
		{
		}
	}

	private async void TextBox_Input(object sender, TextChangedEventArgs e)
	{
		if (!_authInProgress && (int)((UIElement)MenuPanel).Visibility != 0)
		{
			_authTimer.Stop();
			if ((KeyTextInput.Text ?? string.Empty).Trim().Length >= 8)
			{
				_authTimer.Start();
			}
			await Task.CompletedTask;
		}
	}

	private async void KeyTextInput_KeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 6)
		{
			((RoutedEventArgs)e).Handled = true;
			_authTimer.Stop();
			await TryAuthorizeAsync(showError: true);
		}
	}

	private async Task TryAuthorizeAsync(bool showError)
	{
		string key = (KeyTextInput.Text ?? string.Empty).Trim();
		if (key.Length >= 4 && !_authInProgress)
		{
			_authInProgress = true;
			((UIElement)KeyTextInput).IsEnabled = false;
			Status("Проверяю ключ...");
			AuthResult authResult = await Task.Run(() => AuthClient.Exchange(key));
			_authInProgress = false;
			((UIElement)KeyTextInput).IsEnabled = true;
			if (authResult.Success)
			{
				_acceptedKey = key;
				SaveKeyToFile(key);
				ShowMenu();
				Status("");
			}
			else if (showError)
			{
				Status((authResult.Message == "hwid_mismatch") ? "Ключ уже привязан к другому ПК" : "Ключ не найден");
			}
			else
			{
				Status("");
			}
		}
	}

	private void ShowMenu()
	{
		((UIElement)LoginPanel).Visibility = (Visibility)2;
		((UIElement)ProgressPanel).Visibility = (Visibility)2;
		((UIElement)MenuPanel).Visibility = (Visibility)0;
	}

	private void ShowProgress(string message)
	{
		((UIElement)MenuPanel).Visibility = (Visibility)2;
		((UIElement)LoginPanel).Visibility = (Visibility)2;
		((UIElement)ProgressPanel).Visibility = (Visibility)0;
		((RangeBase)DownloadProgress).Value = 0.0;
		Status(message);
	}

	private async void DisableHyperV11_Click(object sender, RoutedEventArgs e)
	{
		await DownloadAndRunToolAsync("https://dantezhxf.alwaysdata.net/11FixHyperV.exe", "11FixHyperV.exe", "Отключаю Hyper-V...");
	}

	private async void DisableHyperV10_Click(object sender, RoutedEventArgs e)
	{
		await DownloadAndRunToolAsync("https://dantezhxf.alwaysdata.net/10FixHyperV.exe", "10FixHyperV.exe", "Отключаю Hyper-V...");
	}

	private async void InstallEmulator_Click(object sender, RoutedEventArgs e)
	{
		await InstallEmulatorAsync();
	}

	private async Task DownloadAndRunToolAsync(string url, string fileName, string message)
	{
		try
		{
			ShowProgress(message);
			string path = Path.Combine(Path.GetTempPath(), fileName);
			await DownloadFileAsync(url, path);
			Status("Запускаю...");
			RunElevated(path, wait: true);
			Status("Готово");
			await Task.Delay(1000);
			ShowMenu();
		}
		catch (Exception ex)
		{
			Status("Ошибка: " + ex.Message);
			await Task.Delay(1800);
			ShowMenu();
		}
	}

	private async Task InstallEmulatorAsync()
	{
		try
		{
			ShowProgress("Удаляю старый эмулятор...");
			string temp = Path.GetTempPath();
			string cleaner = Path.Combine(temp, "cleaner.exe");
			string emulator = Path.Combine(temp, "XANTARES.exe");
			await DownloadFileAsync("https://dantezhxf.alwaysdata.net/cleaner.exe", cleaner);
			Status("Удаляю старый эмулятор...");
			RunElevated(cleaner, wait: true);
			Status("Очищаю TEMP...");
			CleanTempFolders();
			ShowProgress("Скачиваю XANTARES...");
			await DownloadFileAsync("https://dantezhxf.alwaysdata.net/XANTARES.exe", emulator);
			Status("Устанавливаю эмулятор...");
			RunElevated(emulator, wait: true);
			Status("Очищаю временные файлы...");
			CleanupInstallationFiles(temp);
			Status("Приятной игры");
			await Task.Delay(1800);
			((Window)this).Close();
		}
		catch (Exception ex)
		{
			Status("Ошибка: " + ex.Message);
			await Task.Delay(2200);
			ShowMenu();
		}
	}

	private async Task DownloadFileAsync(string url, string destinationPath)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
		HttpClient client = new HttpClient
		{
			Timeout = TimeSpan.FromHours(2.0)
		};
		try
		{
			HttpResponseMessage response = await client.GetAsync(url, (HttpCompletionOption)1);
			try
			{
				response.EnsureSuccessStatusCode();
				long total = response.Content.Headers.ContentLength.GetValueOrDefault();
				using Stream input = await response.Content.ReadAsStreamAsync();
				using FileStream output = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
				byte[] buffer = new byte[81920];
				long readTotal = 0L;
				while (true)
				{
					int num;
					int read = (num = await input.ReadAsync(buffer, 0, buffer.Length));
					if (num <= 0)
					{
						break;
					}
					await output.WriteAsync(buffer, 0, read);
					readTotal += read;
					if (total > 0)
					{
						((RangeBase)DownloadProgress).Value = Math.Min(100.0, (double)readTotal * 100.0 / (double)total);
					}
				}
			}
			finally
			{
				((IDisposable)response)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
		((RangeBase)DownloadProgress).Value = 100.0;
	}

	private static void RunElevated(string path, bool wait)
	{
		using Process process = Process.Start(new ProcessStartInfo(path)
		{
			UseShellExecute = true,
			Verb = "runas"
		});
		if (wait)
		{
			process?.WaitForExit();
		}
	}

	private void Status(string text)
	{
		StatusText.Text = text ?? string.Empty;
	}

	private void SaveKeyToFile(string key)
	{
		try
		{
			File.WriteAllText("key.txt", key ?? string.Empty);
		}
		catch
		{
		}
	}

	private static void CleanTempFolders()
	{
		CleanDirectorySafe(Path.GetTempPath());
		CleanDirectorySafe(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));
	}

	private static void CleanDirectorySafe(string directory)
	{
		if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
		{
			return;
		}
		string[] files = Directory.GetFiles(directory);
		foreach (string path in files)
		{
			try
			{
				File.Delete(path);
			}
			catch
			{
			}
		}
		files = Directory.GetDirectories(directory);
		foreach (string path2 in files)
		{
			try
			{
				Directory.Delete(path2, recursive: true);
			}
			catch
			{
			}
		}
	}

	private static void CleanupInstallationFiles(string tempDirectory)
	{
		try
		{
			string[] array = new string[4] { "cleaner.exe", "XANTARES.exe", "11FixHyperV.exe", "10FixHyperV.exe" };
			foreach (string path in array)
			{
				string path2 = Path.Combine(tempDirectory, path);
				try
				{
					if (File.Exists(path2))
					{
						File.Delete(path2);
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
	}

	private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.ChangedButton == 0)
		{
			((Window)this).DragMove();
		}
	}

	private void Minimize_Window_Click(object sender, RoutedEventArgs e)
	{
		((Window)this).WindowState = (WindowState)1;
	}

	private void Close_Button(object sender, RoutedEventArgs e)
	{
		AnimateWindowOut((Action)base.Close);
	}

	private void AnimateWindowIn()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		DoubleAnimation val = new DoubleAnimation(0.0, 1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(350.0)));
		((UIElement)this).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
	}

	private void AnimateWindowOut(Action onComplete)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		DoubleAnimation val = new DoubleAnimation(1.0, 0.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(250.0)));
		((Timeline)val).Completed += delegate
		{
			onComplete?.Invoke();
		};
		((UIElement)this).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Installer;component/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			((FrameworkElement)(MainWindow)target).Loaded += new RoutedEventHandler(MainWindow_OnLoaded);
			break;
		case 2:
			mainGrid = (Border)target;
			((UIElement)mainGrid).MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
			break;
		case 3:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(Minimize_Window_Click);
			break;
		case 4:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(Close_Button);
			break;
		case 5:
			LoginPanel = (StackPanel)target;
			break;
		case 6:
			KeyTextInput = (TextBox)target;
			((TextBoxBase)KeyTextInput).TextChanged += new TextChangedEventHandler(TextBox_Input);
			((UIElement)KeyTextInput).KeyDown += new KeyEventHandler(KeyTextInput_KeyDown);
			break;
		case 7:
			MenuPanel = (StackPanel)target;
			break;
		case 8:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(DisableHyperV11_Click);
			break;
		case 9:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(DisableHyperV10_Click);
			break;
		case 10:
			((ButtonBase)(Button)target).Click += new RoutedEventHandler(InstallEmulator_Click);
			break;
		case 11:
			ProgressPanel = (StackPanel)target;
			break;
		case 12:
			StatusText = (TextBlock)target;
			break;
		case 13:
			DownloadProgress = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
