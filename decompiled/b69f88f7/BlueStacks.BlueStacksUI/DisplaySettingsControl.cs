using System;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class DisplaySettingsControl : DisplaySettingsBase
{
	public MainWindow ParentWindow { get; private set; }

	public DisplaySettingsControl(MainWindow window)
		: base((Window)(object)window, window?.mVmName, "")
	{
		ParentWindow = window;
	}

	protected override void Save(object param)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			((DisplaySettingsBase)this).SaveDisplaySetting();
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
			((Window)ParentWindow).Close();
		}
		else
		{
			if (!((DisplaySettingsBase)this).IsDirty())
			{
				return;
			}
			CustomMessageWindow val = new CustomMessageWindow
			{
				Owner = (Window)(object)ParentWindow,
				WindowStartupLocation = (WindowStartupLocation)2
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
			val.AddButton((ButtonColors)4, "STRING_RESTART_NOW", (EventHandler)delegate
			{
				((DisplaySettingsBase)this).SaveDisplaySetting();
				if (BlueStacksUIUtils.DictWindows.Count == 1)
				{
					App.defaultResolution = new Fraction(RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth, RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight);
					PromotionManager.ReloadPromotionsAsync();
				}
				BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
				BlueStacksUIUtils.RestartInstance(((DisplaySettingsBase)this).VmName);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				((DisplaySettingsBase)this).DiscardCurrentChangingModel();
			}, (string)null, false, (object)null);
			((Window)val).ShowDialog();
		}
	}
}
