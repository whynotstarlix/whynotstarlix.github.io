using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace BlueStacks.BlueStacksUI;

public class CustomAmination
{
	public unsafe static void ApplyRgbAnimation(TopBar topBar)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_023b: Expected O, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Expected O, but got Unknown
		//IL_02d5: Expected O, but got Unknown
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Expected O, but got Unknown
		//IL_0353: Expected O, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Expected O, but got Unknown
		//IL_03d1: Expected O, but got Unknown
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		if (topBar?.mVersionText == null)
		{
			return;
		}
		TextBlock mVersionText = topBar.mVersionText;
		SetupTextBlockProperties(mVersionText);
		((TextElementCollection<Inline>)(object)mVersionText.Inlines).Clear();
		string text = "BLUESTER";
		for (int i = 0; i < text.Length; i++)
		{
			LinearGradientBrush val = new LinearGradientBrush
			{
				MappingMode = (BrushMappingMode)1,
				StartPoint = new Point(0.0, 0.0),
				EndPoint = new Point(1.0, 1.0)
			};
			((GradientBrush)val).GradientStops.Add(new GradientStop(Colors.White, 0.0));
			((GradientBrush)val).GradientStops.Add(new GradientStop(Color.FromRgb((byte)240, (byte)240, (byte)240), 0.5));
			((GradientBrush)val).GradientStops.Add(new GradientStop(Color.FromRgb((byte)220, (byte)220, (byte)220), 1.0));
			DropShadowEffect val2 = new DropShadowEffect
			{
				Color = Colors.White,
				BlurRadius = 12.0,
				ShadowDepth = 0.0,
				Opacity = 0.4
			};
			TextBlock val3 = new TextBlock((Inline)new Run(text[i].ToString())
			{
				Foreground = (Brush)(object)val
			})
			{
				Effect = (Effect)(object)val2,
				FontSize = mVersionText.FontSize,
				FontWeight = mVersionText.FontWeight,
				FontFamily = mVersionText.FontFamily
			};
			((TextElementCollection<Inline>)(object)mVersionText.Inlines).Add((Inline)new InlineUIContainer((UIElement)(object)val3));
			double value = (double)i * 0.12;
			PointAnimation val4 = new PointAnimation
			{
				From = new Point(0.2, 0.2),
				To = new Point(0.8, 0.8),
				Duration = Duration.op_Implicit(TimeSpan.FromSeconds(4.0)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever,
				BeginTime = TimeSpan.FromSeconds(value),
				EasingFunction = (IEasingFunction)new SineEase
				{
					EasingMode = (EasingMode)2
				}
			};
			PointAnimation val5 = new PointAnimation
			{
				From = new Point(0.8, 0.8),
				To = new Point(0.2, 0.2),
				Duration = Duration.op_Implicit(TimeSpan.FromSeconds(4.5)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever,
				BeginTime = TimeSpan.FromSeconds(value),
				EasingFunction = (IEasingFunction)new SineEase
				{
					EasingMode = (EasingMode)2
				}
			};
			DoubleAnimation val6 = new DoubleAnimation
			{
				From = 0.0,
				To = 1.0,
				Duration = Duration.op_Implicit(TimeSpan.FromSeconds(3.5)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever,
				BeginTime = TimeSpan.FromSeconds(value),
				EasingFunction = (IEasingFunction)new SineEase
				{
					EasingMode = (EasingMode)2
				}
			};
			DoubleAnimation val7 = new DoubleAnimation
			{
				From = 0.3,
				To = 0.8,
				Duration = Duration.op_Implicit(TimeSpan.FromSeconds(2.0)),
				AutoReverse = true,
				RepeatBehavior = RepeatBehavior.Forever,
				BeginTime = TimeSpan.FromSeconds(value),
				EasingFunction = (IEasingFunction)new SineEase
				{
					EasingMode = (EasingMode)2
				}
			};
			((Animatable)val).BeginAnimation(LinearGradientBrush.StartPointProperty, (AnimationTimeline)(object)val4);
			((Animatable)val).BeginAnimation(LinearGradientBrush.EndPointProperty, (AnimationTimeline)(object)val5);
			Enumerator enumerator = ((GradientBrush)val).GradientStops.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					((Animatable)((Enumerator)(ref enumerator)).Current).BeginAnimation(GradientStop.OffsetProperty, (AnimationTimeline)(object)val6);
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
			((Animatable)val2).BeginAnimation(DropShadowEffect.OpacityProperty, (AnimationTimeline)(object)val7);
		}
	}

	private static void SetupTextBlockProperties(TextBlock textBlock)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		textBlock.Text = "";
		textBlock.FontFamily = new FontFamily("Segoe UI");
		textBlock.FontWeight = FontWeights.Bold;
		textBlock.FontSize = 18.0;
		textBlock.TextAlignment = (TextAlignment)2;
		((UIElement)textBlock).Opacity = 1.0;
	}
}
