using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class OtsFeedbackControl : UserControl, IComponentConnector
{
	private static List<string> m_Phone_Patterns = new List<string> { "^[\\d\\s-\\+]{5,15}$" };

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox txtDescIssue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border txtEmailBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox txtEmail;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border txtPhoneBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox txtPhone;

	private bool _contentLoaded;

	public MainWindow ParentWindow { get; set; }

	public OtsFeedbackControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	private void SubmitButton_Click(object sender, RoutedEventArgs e)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!TestEmail(txtEmail.Text) || !TestPhone(txtPhone.Text))
			{
				return;
			}
			ClientStats.SendMiscellaneousStatsAsync("OTSFeedback", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, txtDescIssue.Text, txtEmail.Text, txtPhone.Text);
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					Process process = new Process();
					process.StartInfo.Arguments = "-silent";
					process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
					process.Start();
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception in starting HD-logCollector.exe: " + ex2.ToString());
				}
			});
			thread.IsBackground = true;
			thread.Start();
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			CustomMessageWindow val = new CustomMessageWindow
			{
				ImageName = "help"
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_THANK_YOU", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_APPRECIATE_FEEDBACK", "");
			val.AddButton((ButtonColors)4, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler)null, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)ParentWindow;
			ParentWindow.ShowDimOverlay();
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Submitting ots feedback " + ex.ToString());
		}
	}

	private bool TestEmail(string text)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)txtEmailBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
		if (!Regex.IsMatch(text, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase))
		{
			txtEmailBorder.BorderBrush = (Brush)(object)Brushes.Red;
			return false;
		}
		return true;
	}

	private bool TestPhone(string text)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)(object)txtPhoneBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
		if (!Regex.IsMatch(text, MakeCombinedPattern(m_Phone_Patterns), RegexOptions.IgnoreCase))
		{
			txtPhoneBorder.BorderBrush = (Brush)(object)Brushes.Red;
			return false;
		}
		return true;
	}

	private static string MakeCombinedPattern(IEnumerable<string> patterns)
	{
		return string.Join("|", patterns.Select((string item) => "(" + item + ")").ToArray());
	}

	private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
	{
		TestEmail(txtEmail.Text);
	}

	private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
	{
		TestPhone(txtPhone.Text);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/otsfeedbackcontrol.xaml", UriKind.Relative);
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
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_MouseLeftButtonUp);
			break;
		case 2:
			txtDescIssue = (TextBox)target;
			break;
		case 3:
			txtEmailBorder = (Border)target;
			break;
		case 4:
			txtEmail = (TextBox)target;
			((TextBoxBase)txtEmail).TextChanged += new TextChangedEventHandler(txtEmail_TextChanged);
			break;
		case 5:
			txtPhoneBorder = (Border)target;
			break;
		case 6:
			txtPhone = (TextBox)target;
			((TextBoxBase)txtPhone).TextChanged += new TextChangedEventHandler(txtPhone_TextChanged);
			break;
		case 7:
			((ButtonBase)(CustomButton)target).Click += new RoutedEventHandler(SubmitButton_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
