using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class StepperTextBox : XTextBox, IComponentConnector, IStyleConnector
{
	private Regex decimalRegex = new Regex("^[0-9]*(\\.)?[0-9]*$");

	public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register("PropertyType", typeof(Type), typeof(StepperTextBox), new PropertyMetadata());

	public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register("IMActionItems", typeof(ObservableCollection<IMActionItem>), typeof(StepperTextBox), new PropertyMetadata());

	private bool _contentLoaded;

	public Type PropertyType
	{
		get
		{
			return (Type)((DependencyObject)this).GetValue(PropertyTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PropertyTypeProperty, (object)value);
		}
	}

	public double MinValue { get; set; }

	public double MaxValue { get; set; }

	public ObservableCollection<IMActionItem> IMActionItems
	{
		get
		{
			return (ObservableCollection<IMActionItem>)((DependencyObject)this).GetValue(IMActionItemsProperty);
		}
		set
		{
			if (value == null)
			{
				((DependencyObject)this).ClearValue(IMActionItemsProperty);
			}
			else
			{
				((DependencyObject)this).SetValue(IMActionItemsProperty, (object)value);
			}
		}
	}

	public StepperTextBox()
	{
		InitializeComponent();
		((DependencyObject)this).ClearValue(IMActionItemsProperty);
		InputMethod.SetIsInputMethodEnabled((DependencyObject)(object)this, false);
	}

	protected override void OnPreviewTextInput(TextCompositionEventArgs args)
	{
		if (args != null)
		{
			string text;
			if (((TextBox)this).SelectionLength > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(((TextBox)this).Text);
				stringBuilder.Remove(((TextBox)this).SelectionStart, ((TextBox)this).SelectionLength);
				stringBuilder.Insert(((TextBox)this).SelectionStart, args.Text);
				text = stringBuilder.ToString();
			}
			else
			{
				text = ((TextBox)this).Text.Insert(((TextBox)this).SelectionStart, args.Text);
			}
			if ((object)PropertyType == typeof(int))
			{
				((RoutedEventArgs)args).Handled = int.TryParse(text, out var _);
			}
			else if ((object)PropertyType == typeof(double))
			{
				if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result2))
				{
					((RoutedEventArgs)args).Handled = !decimalRegex.IsMatch(text) || !(MinValue <= result2) || !(result2 <= MaxValue);
				}
				else
				{
					if (string.Equals(text, ".", StringComparison.InvariantCultureIgnoreCase))
					{
						((TextBox)this).Text = "0.";
						KMManager.CheckAndCreateNewScheme();
						if (IMActionItems != null && IMActionItems.Any())
						{
							foreach (IMActionItem iMActionItem in IMActionItems)
							{
								SetValueHandling(iMActionItem);
							}
						}
						((TextBox)this).CaretIndex = ((TextBox)this).Text.Length;
					}
					((RoutedEventArgs)args).Handled = true;
				}
			}
		}
		((UIElement)this).OnPreviewTextInput(args);
	}

	private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		((RoutedEventArgs)e).Handled = e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste;
	}

	private void OnIncrease(object sender, RoutedEventArgs e)
	{
		int result2;
		if ((object)PropertyType == typeof(double) && double.TryParse(((TextBox)this).Text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result))
		{
			if (CanIncrease(result, 0.05))
			{
				((TextBox)this).Text = (result + 0.05).ToString(CultureInfo.InvariantCulture);
				KMManager.CheckAndCreateNewScheme();
			}
		}
		else if ((object)PropertyType == typeof(int) && int.TryParse(((TextBox)this).Text, out result2) && CanIncrease(result2, 1.0))
		{
			((TextBox)this).Text = (result2 + 1).ToString(CultureInfo.InvariantCulture);
			KMManager.CheckAndCreateNewScheme();
		}
		foreach (IMActionItem iMActionItem in IMActionItems)
		{
			SetValueHandling(iMActionItem);
		}
	}

	private bool CanIncrease(double doubleVal, double val)
	{
		return doubleVal + val <= MaxValue;
	}

	private bool CanDecrease(double doubleVal, double val)
	{
		return doubleVal - val >= MinValue;
	}

	private void OnDecrease(object sender, RoutedEventArgs e)
	{
		int result2;
		if ((object)PropertyType == typeof(double) && double.TryParse(((TextBox)this).Text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var result) && result > 0.0)
		{
			if (CanDecrease(result, 0.05))
			{
				((TextBox)this).Text = (result - 0.05).ToString(CultureInfo.InvariantCulture);
				KMManager.CheckAndCreateNewScheme();
			}
		}
		else if ((object)PropertyType == typeof(int) && int.TryParse(((TextBox)this).Text, out result2) && result2 > 0 && CanDecrease(result2, 1.0))
		{
			((TextBox)this).Text = (result2 - 1).ToString(CultureInfo.InvariantCulture);
			KMManager.CheckAndCreateNewScheme();
		}
		foreach (IMActionItem iMActionItem in IMActionItems)
		{
			SetValueHandling(iMActionItem);
		}
	}

	protected override void OnPreviewKeyDown(KeyEventArgs args)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		if (args != null)
		{
			if ((int)args.Key == 18)
			{
				((RoutedEventArgs)args).Handled = true;
			}
			else
			{
				if ((int)args.Key == 13)
				{
					return;
				}
				((UIElement)this).Focus();
			}
		}
		((UIElement)this).OnPreviewKeyDown(args);
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		((TextBoxBase)this).TextChanged -= new TextChangedEventHandler(IMapTextBox_TextChanged);
		((TextBoxBase)this).TextChanged += new TextChangedEventHandler(IMapTextBox_TextChanged);
		((TextBox)this).CaretIndex = ((TextBox)this).Text.Length;
	}

	private void IMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (IMActionItems == null || !IMActionItems.Any())
		{
			return;
		}
		foreach (IMActionItem iMActionItem in IMActionItems)
		{
			SetValueHandling(iMActionItem);
		}
		KMManager.CheckAndCreateNewScheme();
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		((TextBoxBase)this).TextChanged -= new TextChangedEventHandler(IMapTextBox_TextChanged);
		((TextBoxBase)this).OnLostFocus(e);
	}

	private void SetValueHandling(IMActionItem item)
	{
		string text = item.IMAction[item.ActionItem].ToString();
		if (PropertyType.Equals(typeof(double)))
		{
			if (double.TryParse(((TextBox)this).Text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var _))
			{
				text = ((TextBox)this).Text;
			}
			else if (!string.IsNullOrEmpty(((TextBox)this).Text))
			{
				((TextBox)this).Text = text;
			}
		}
		else if (PropertyType.Equals(typeof(decimal)))
		{
			if (decimal.TryParse(((TextBox)this).Text, out var _))
			{
				text = ((TextBox)this).Text;
			}
			else if (!string.IsNullOrEmpty(((TextBox)this).Text))
			{
				((TextBox)this).Text = text;
			}
		}
		else if (PropertyType.Equals(typeof(int)))
		{
			if (int.TryParse(((TextBox)this).Text, out var _))
			{
				text = ((TextBox)this).Text;
			}
			else if (!string.IsNullOrEmpty(((TextBox)this).Text))
			{
				((TextBox)this).Text = text;
			}
		}
		Setvalue(item, text);
	}

	internal void Setvalue(IMActionItem item, string value)
	{
		if (!string.Equals(item.IMAction[item.ActionItem].ToString(), value, StringComparison.InvariantCulture))
		{
			item.IMAction[item.ActionItem] = value;
		}
		if (item.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
		{
			((TextBox)this).Text = ((TextBox)this).Text.ToUpper(CultureInfo.InvariantCulture);
		}
		if (UsefulExtensionMethod.Contains(item.ActionItem, "Gamepad", StringComparison.InvariantCultureIgnoreCase))
		{
			((TextBox)this).Text = ((TextBox)this).Text.ToUpper(CultureInfo.InvariantCulture);
		}
		Logger.Debug("GUIDANCE: " + item.IMAction.Type);
	}

	private void Path_MouseEnter(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Path) ? sender : null), Shape.FillProperty, "SettingsWindowTabMenuItemLegendForeground");
	}

	private void Path_MouseLeave(object sender, MouseEventArgs e)
	{
		BlueStacksUIBinding.BindColor((DependencyObject)((sender is Path) ? sender : null), Shape.FillProperty, "SettingsWindowForegroundDimColor");
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancemodels/steppertextbox.xaml", UriKind.Relative);
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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (connectionId == 1)
		{
			((UIElement)(StepperTextBox)target).AddHandler(CommandManager.PreviewExecutedEvent, (Delegate)new ExecutedRoutedEventHandler(OnPreviewExecuted));
		}
		else
		{
			_contentLoaded = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		switch (connectionId)
		{
		case 2:
			((ButtonBase)(RepeatButton)target).Click += new RoutedEventHandler(OnIncrease);
			break;
		case 3:
			((UIElement)(Path)target).MouseEnter += new MouseEventHandler(Path_MouseEnter);
			((UIElement)(Path)target).MouseLeave += new MouseEventHandler(Path_MouseLeave);
			break;
		case 4:
			((ButtonBase)(RepeatButton)target).Click += new RoutedEventHandler(OnDecrease);
			break;
		case 5:
			((UIElement)(Path)target).MouseEnter += new MouseEventHandler(Path_MouseEnter);
			((UIElement)(Path)target).MouseLeave += new MouseEventHandler(Path_MouseLeave);
			break;
		}
	}
}
