using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI;

public static class Animator
{
	public static AnimationClock AnimatePenner(DependencyObject element, DependencyProperty prop, PennerDoubleAnimation.Equations type, double to, int durationMS, EventHandler callbackFunc)
	{
		return AnimatePenner(element, prop, type, null, to, durationMS, callbackFunc);
	}

	public static AnimationClock AnimatePenner(DependencyObject element, DependencyProperty prop, PennerDoubleAnimation.Equations type, double? from, double to, int durationMS, EventHandler callbackFunc)
	{
		double num = (double.IsNaN((double)((element != null) ? element.GetValue(prop) : null)) ? 0.0 : ((double)element.GetValue(prop)));
		PennerDoubleAnimation anim = new PennerDoubleAnimation(type, from ?? num, to);
		return Animate(element, prop, (AnimationTimeline)(object)anim, durationMS, null, null, callbackFunc);
	}

	public static AnimationClock AnimateDouble(DependencyObject element, DependencyProperty prop, double? from, double to, int durationMS, double? accel, double? decel, EventHandler callbackFunc)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		double num = (double.IsNaN((double)((element != null) ? element.GetValue(prop) : null)) ? 0.0 : ((double)element.GetValue(prop)));
		DoubleAnimation anim = new DoubleAnimation
		{
			From = (from ?? num),
			To = to
		};
		return Animate(element, prop, (AnimationTimeline)(object)anim, durationMS, accel, decel, callbackFunc);
	}

	public static void ClearAnimation(DependencyObject animatable, DependencyProperty property)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (animatable != null)
		{
			animatable.SetValue(property, animatable.GetValue(property));
			((IAnimatable)animatable).ApplyAnimationClock(property, (AnimationClock)null);
		}
	}

	private static AnimationClock Animate(DependencyObject animatable, DependencyProperty prop, AnimationTimeline anim, int duration, double? accel, double? decel, EventHandler func)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		((Timeline)anim).AccelerationRatio = accel ?? 0.0;
		((Timeline)anim).DecelerationRatio = decel ?? 0.0;
		((Timeline)anim).Duration = Duration.op_Implicit(TimeSpan.FromMilliseconds((double)duration));
		((Freezable)anim).Freeze();
		AnimationClock animClock = anim.CreateClock();
		((Clock)animClock).Completed += animClock_Completed;
		if (func != null)
		{
			((Clock)animClock).Completed += func;
		}
		((Clock)animClock).Controller.Begin();
		ClearAnimation(animatable, prop);
		((IAnimatable)animatable).ApplyAnimationClock(prop, animClock);
		return animClock;
		void animClock_Completed(object sender, EventArgs e)
		{
			ClearAnimation(animatable, prop);
			((Clock)animClock).Completed -= animClock_Completed;
		}
	}
}
