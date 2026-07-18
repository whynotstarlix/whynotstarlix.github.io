using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Installer;

public class MainWindow : Window, IComponentConnector
{
	private static class GateBridge
	{
		internal static GateOutcome Run(string key)
		{
			try
			{
				string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Xantares.ResourceLayer.dll");
				if (!File.Exists(text))
				{
					return GateOutcome.Fail("library_missing");
				}
				object obj = Assembly.LoadFrom(text).GetType("Xantares.ResourceLayer.QuartzCore", throwOnError: true).GetMethod("Tick", BindingFlags.Static | BindingFlags.Public)
					.Invoke(null, new object[1] { key });
				if (obj == null)
				{
					return GateOutcome.Fail("empty");
				}
				Type type = obj.GetType();
				bool success = (bool)type.GetProperty("Ready").GetValue(obj, null);
				string message = Convert.ToString(type.GetProperty("Note").GetValue(obj, null));
				return new GateOutcome(success, message);
			}
			catch
			{
				return GateOutcome.Fail("network");
			}
		}
	}

	private sealed class GateOutcome
	{
		internal bool Success { get; }

		internal string Message { get; }

		internal GateOutcome(bool success, string message)
		{
			Success = success;
			Message = message ?? string.Empty;
		}

		internal static GateOutcome Fail(string message)
		{
			return new GateOutcome(success: false, message);
		}
	}

	private static readonly string KeyFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Xantares", "key.dat");

	private const string CleanerUrl = "https://dantezhxf.alwaysdata.net/cleaner.exe";

	private const string EmulatorUrl = "https://dantezhxf.alwaysdata.net/XANTARES.exe";

	private const string HyperV11Url = "https://dantezhxf.alwaysdata.net/11FixHyperV.exe";

	private const string HyperV10Url = "https://dantezhxf.alwaysdata.net/10FixHyperV.exe";

	private const string RuntimeRegistryPath = "SOFTWARE\\XANTARES";

	private const string RuntimeRegistryValue = "RuntimeKey";

	private readonly DispatcherTimer _gateTimer;

	private bool _gateBusy;

	internal Border mainGrid;

	internal StackPanel BrandPanel;

	internal StackPanel LoginPanel;

	internal TextBox KeyTextInput;

	internal MenuView SecondPage;

	internal Grid ProgressPanel;

	internal TranslateTransform ProgressSkullMove;

	internal Grid ProgressJawRig;

	internal ScaleTransform ProgressTeethScale;

	internal TranslateTransform ProgressTeethBite;

	internal Path ProgressJawBone;

	internal Path ProgressJawMouthHole;

	internal Canvas ProgressTeethCanvas;

	internal Rectangle ProgressToothLeftCenter;

	internal Rectangle ProgressToothCenter;

	internal Rectangle ProgressToothRightCenter;

	internal Rectangle ProgressToothRightMid;

	internal TextBlock StatusText;

	internal ProgressBar DownloadProgress;

	private bool _contentLoaded;

	public MainWindow()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		InitializeComponent();
		SecondPage.DisableHyperV11Requested += async delegate
		{
			await DownloadAndRunToolAsync("https://dantezhxf.alwaysdata.net/11FixHyperV.exe", "11FixHyperV.exe", "Отключаю Hyper-V...");
		};
		SecondPage.DisableHyperV10Requested += async delegate
		{
			await DownloadAndRunToolAsync("https://dantezhxf.alwaysdata.net/10FixHyperV.exe", "10FixHyperV.exe", "Отключаю Hyper-V...");
		};
		SecondPage.InstallRequested += async delegate
		{
			await InstallEmulatorAsync();
		};
		_gateTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(700.0)
		};
		_gateTimer.Tick += async delegate
		{
			_gateTimer.Stop();
			await TryUnlockAsync(showError: false);
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
			Directory.CreateDirectory(Path.GetDirectoryName(KeyFilePath));
			string text = LoadKeyFromRegistry();
			if (string.IsNullOrWhiteSpace(text))
			{
				string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt");
				if (File.Exists(path))
				{
					text = File.ReadAllText(path).Trim();
					try
					{
						File.Delete(path);
					}
					catch
					{
					}
				}
				else if (File.Exists(KeyFilePath))
				{
					text = File.ReadAllText(KeyFilePath).Trim();
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				KeyTextInput.Text = text;
				KeyTextInput.CaretIndex = KeyTextInput.Text.Length;
			}
		}
		catch
		{
		}
	}

	private void TextBox_Input(object sender, TextChangedEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!_gateBusy && (int)((UIElement)SecondPage).Visibility != 0)
		{
			_gateTimer.Stop();
			if ((KeyTextInput.Text ?? string.Empty).Trim().Length >= 8)
			{
				_gateTimer.Start();
			}
		}
	}

	private async void KeyTextInput_KeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key == 6)
		{
			((RoutedEventArgs)e).Handled = true;
			_gateTimer.Stop();
			await TryUnlockAsync(showError: true);
		}
	}

	private async Task TryUnlockAsync(bool showError)
	{
		string key = (KeyTextInput.Text ?? string.Empty).Trim();
		if (key.Length >= 4 && !_gateBusy)
		{
			_gateBusy = true;
			((UIElement)KeyTextInput).IsEnabled = false;
			Status("Проверяю ключ...");
			GateOutcome gateOutcome = await Task.Run(() => GateBridge.Run(key));
			_gateBusy = false;
			((UIElement)KeyTextInput).IsEnabled = true;
			if (gateOutcome.Success)
			{
				SaveKeyToFile(key);
				SaveKeyToRegistry(key);
				ShowMenu();
				Status(string.Empty);
			}
			else
			{
				Status(showError ? FormatGateError(gateOutcome.Message) : string.Empty);
			}
		}
	}

	private static string FormatGateError(string message)
	{
		return (message ?? string.Empty) switch
		{
			"hwid_mismatch" => "Ключ привязан к другому устройству", 
			"expired" => "Срок ключа закончился", 
			"network" => "Нет связи с сервером", 
			_ => "Ключ не найден", 
		};
	}

	private void ShowMenu()
	{
		((UIElement)BrandPanel).Visibility = (Visibility)2;
		((UIElement)LoginPanel).Visibility = (Visibility)2;
		((UIElement)ProgressPanel).Visibility = (Visibility)2;
		((UIElement)SecondPage).Visibility = (Visibility)0;
	}

	private void ShowProgress(string message)
	{
		((UIElement)BrandPanel).Visibility = (Visibility)2;
		((UIElement)LoginPanel).Visibility = (Visibility)2;
		((UIElement)SecondPage).Visibility = (Visibility)2;
		((UIElement)ProgressPanel).Visibility = (Visibility)0;
		((RangeBase)DownloadProgress).Value = 0.0;
		Status(message);
		StartProgressAnimation();
	}

	private void StartProgressAnimation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ca: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0135: Expected O, but got Unknown
		DoubleAnimation val = new DoubleAnimation
		{
			From = 0.0,
			To = -6.0,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(2.4)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new SineEase
			{
				EasingMode = (EasingMode)2
			}
		};
		DoubleAnimation val2 = new DoubleAnimation
		{
			From = 0.0,
			To = -4.0,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(0.85)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)2
			}
		};
		DoubleAnimation val3 = new DoubleAnimation
		{
			From = 1.0,
			To = 0.92,
			Duration = Duration.op_Implicit(TimeSpan.FromSeconds(0.85)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever,
			EasingFunction = (IEasingFunction)new QuadraticEase
			{
				EasingMode = (EasingMode)2
			}
		};
		((Animatable)ProgressSkullMove).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)(object)val);
		((Animatable)ProgressTeethBite).BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)(object)val2);
		((Animatable)ProgressTeethScale).BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline)(object)val3);
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

	private static void SaveKeyToFile(string key)
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(KeyFilePath));
			File.WriteAllText(KeyFilePath, key ?? string.Empty);
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.txt");
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
					return;
				}
				catch
				{
					return;
				}
			}
		}
		catch
		{
		}
	}

	private static void SaveKeyToRegistry(string key)
	{
		try
		{
			byte[] inArray = ProtectedData.Protect(Encoding.UTF8.GetBytes(key ?? string.Empty), (byte[])null, (DataProtectionScope)0);
			using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\XANTARES");
			registryKey?.SetValue("RuntimeKey", Convert.ToBase64String(inArray), RegistryValueKind.String);
			registryKey?.DeleteValue("Key", throwOnMissingValue: false);
		}
		catch
		{
		}
	}

	private static string LoadKeyFromRegistry()
	{
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\XANTARES");
			string text = registryKey?.GetValue("RuntimeKey") as string;
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}
			byte[] bytes = ProtectedData.Unprotect(Convert.FromBase64String(text), (byte[])null, (DataProtectionScope)0);
			return Encoding.UTF8.GetString(bytes).Trim();
		}
		catch
		{
			return string.Empty;
		}
	}

	private static void CleanTempFolders()
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		AddTempFolder(hashSet, Path.GetTempPath());
		AddTempFolder(hashSet, Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.User));
		AddTempFolder(hashSet, Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.User));
		AddTempFolder(hashSet, Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine));
		AddTempFolder(hashSet, Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Machine));
		AddTempFolder(hashSet, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));
		foreach (string item in hashSet)
		{
			CleanDirectorySafe(item);
		}
	}

	private static void AddTempFolder(ICollection<string> folders, string folder)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(folder))
			{
				string text = Path.GetFullPath(Environment.ExpandEnvironmentVariables(folder)).TrimEnd(new char[1] { '\\' });
				if (Directory.Exists(text))
				{
					folders.Add(text);
				}
			}
		}
		catch
		{
		}
	}

	private static void CleanDirectorySafe(string directory)
	{
		if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
		{
			return;
		}
		foreach (string item in EnumerateFilesSafe(directory))
		{
			try
			{
				File.SetAttributes(item, FileAttributes.Normal);
				File.Delete(item);
			}
			catch
			{
			}
		}
		List<string> list = new List<string>(EnumerateDirectoriesSafe(directory));
		list.Sort((string left, string right) => right.Length.CompareTo(left.Length));
		foreach (string item2 in list)
		{
			try
			{
				Directory.Delete(item2, recursive: false);
			}
			catch
			{
			}
		}
	}

	private static IEnumerable<string> EnumerateFilesSafe(string directory)
	{
		string[] array;
		try
		{
			array = Directory.GetFiles(directory);
		}
		catch
		{
			array = new string[0];
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			yield return array2[i];
		}
		foreach (string item in EnumerateDirectoriesSafe(directory))
		{
			foreach (string item2 in EnumerateFilesSafe(item))
			{
				yield return item2;
			}
		}
	}

	private static IEnumerable<string> EnumerateDirectoriesSafe(string directory)
	{
		string[] directories;
		try
		{
			directories = Directory.GetDirectories(directory);
		}
		catch
		{
			yield break;
		}
		string[] array = directories;
		foreach (string child in array)
		{
			yield return child;
			foreach (string item in EnumerateDirectoriesSafe(child))
			{
				yield return item;
			}
		}
	}

	private static void CleanupInstallationFiles(string tempDirectory)
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		((UIElement)this).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)new DoubleAnimation(0.0, 1.0, Duration.op_Implicit(TimeSpan.FromMilliseconds(350.0))));
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
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
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
			BrandPanel = (StackPanel)target;
			break;
		case 6:
			LoginPanel = (StackPanel)target;
			break;
		case 7:
			KeyTextInput = (TextBox)target;
			((TextBoxBase)KeyTextInput).TextChanged += new TextChangedEventHandler(TextBox_Input);
			((UIElement)KeyTextInput).KeyDown += new KeyEventHandler(KeyTextInput_KeyDown);
			break;
		case 8:
			SecondPage = (MenuView)target;
			break;
		case 9:
			ProgressPanel = (Grid)target;
			break;
		case 10:
			ProgressSkullMove = (TranslateTransform)target;
			break;
		case 11:
			ProgressJawRig = (Grid)target;
			break;
		case 12:
			ProgressTeethScale = (ScaleTransform)target;
			break;
		case 13:
			ProgressTeethBite = (TranslateTransform)target;
			break;
		case 14:
			ProgressJawBone = (Path)target;
			break;
		case 15:
			ProgressJawMouthHole = (Path)target;
			break;
		case 16:
			ProgressTeethCanvas = (Canvas)target;
			break;
		case 17:
			ProgressToothLeftCenter = (Rectangle)target;
			break;
		case 18:
			ProgressToothCenter = (Rectangle)target;
			break;
		case 19:
			ProgressToothRightCenter = (Rectangle)target;
			break;
		case 20:
			ProgressToothRightMid = (Rectangle)target;
			break;
		case 21:
			StatusText = (TextBlock)target;
			break;
		case 22:
			DownloadProgress = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
