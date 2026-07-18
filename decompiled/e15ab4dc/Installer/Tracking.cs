using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Installer;

public static class Tracking
{
	public static readonly DependencyProperty CharacterSpacingProperty = DependencyProperty.RegisterAttached("CharacterSpacing", typeof(int), typeof(Tracking), new PropertyMetadata((object)0, new PropertyChangedCallback(OnCharacterSpacingChanged)));

	private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(Tracking), new PropertyMetadata((object)false));

	private static readonly DependencyProperty OriginalTextProperty = DependencyProperty.RegisterAttached("OriginalText", typeof(string), typeof(Tracking), new PropertyMetadata((PropertyChangedCallback)null));

	public static void SetCharacterSpacing(DependencyObject element, int value)
	{
		element.SetValue(CharacterSpacingProperty, (object)value);
	}

	public static int GetCharacterSpacing(DependencyObject element)
	{
		return (int)element.GetValue(CharacterSpacingProperty);
	}

	private static bool GetIsUpdating(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsUpdatingProperty);
	}

	private static void SetIsUpdating(DependencyObject obj, bool value)
	{
		obj.SetValue(IsUpdatingProperty, (object)value);
	}

	private static string GetOriginalText(DependencyObject obj)
	{
		return (string)obj.GetValue(OriginalTextProperty);
	}

	private static void SetOriginalText(DependencyObject obj, string value)
	{
		obj.SetValue(OriginalTextProperty, (object)value);
	}

	private static void OnCharacterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		TextBlock val = (TextBlock)(object)((d is TextBlock) ? d : null);
		if (val != null)
		{
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(TbOnLoaded);
			((FrameworkElement)val).Loaded += new RoutedEventHandler(TbOnLoaded);
			Apply(val);
		}
	}

	private static void TbOnLoaded(object sender, RoutedEventArgs e)
	{
		TextBlock val = (TextBlock)((sender is TextBlock) ? sender : null);
		if (val != null)
		{
			Apply(val);
		}
	}

	private static void Apply(TextBlock tb)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		if (GetIsUpdating((DependencyObject)(object)tb))
		{
			return;
		}
		int characterSpacing = GetCharacterSpacing((DependencyObject)(object)tb);
		if (characterSpacing == 0)
		{
			string originalText = GetOriginalText((DependencyObject)(object)tb);
			if (originalText != null)
			{
				SetIsUpdating((DependencyObject)(object)tb, value: true);
				((TextElementCollection<Inline>)(object)tb.Inlines).Clear();
				tb.Text = originalText;
				SetIsUpdating((DependencyObject)(object)tb, value: false);
			}
			return;
		}
		string originalText2 = GetOriginalText((DependencyObject)(object)tb);
		string text = originalText2 ?? tb.Text;
		if (originalText2 == null)
		{
			SetOriginalText((DependencyObject)(object)tb, text);
		}
		double num = (double)characterSpacing / 1000.0 * tb.FontSize;
		SetIsUpdating((DependencyObject)(object)tb, value: true);
		tb.Text = string.Empty;
		((TextElementCollection<Inline>)(object)tb.Inlines).Clear();
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			((TextElementCollection<Inline>)(object)tb.Inlines).Add((Inline)new Run(c.ToString(CultureInfo.InvariantCulture)));
			if (i < text.Length - 1)
			{
				((TextElementCollection<Inline>)(object)tb.Inlines).Add((Inline)new Run("\u200a")
				{
					FontSize = tb.FontSize + num
				});
			}
		}
		SetIsUpdating((DependencyObject)(object)tb, value: false);
	}
}
