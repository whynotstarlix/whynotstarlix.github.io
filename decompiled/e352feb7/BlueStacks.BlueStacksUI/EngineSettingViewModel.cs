using System;
using System.Globalization;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class EngineSettingViewModel : EngineSettingBaseViewModel
{
	private string _VmName;

	private MainWindow ParentWindow;

	public EngineSettingViewModel(MainWindow owner, string vmName, EngineSettingBase engineSettingBase)
		: base((Window)(object)owner, vmName, engineSettingBase, false, "")
	{
		ParentWindow = owner;
		_VmName = vmName;
	}

	protected override void Save(object param)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((EngineSettingBaseViewModel)this).Status == 1)
		{
			Logger.Info("Compatibility check is running");
		}
		else if (((EngineSettingBaseViewModel)this).IsRestartRequired())
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				RestartInstanceHandler();
				((Window)ParentWindow).Close();
				return;
			}
			CustomMessageWindow val = new CustomMessageWindow
			{
				Owner = ((EngineSettingBaseViewModel)this).Owner,
				WindowStartupLocation = (WindowStartupLocation)(-1785093438 ^ -1785093440)
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
			val.AddButton((ButtonColors)(-1419027243 ^ -1419027247), "STRING_RESTART_NOW", (EventHandler)delegate
			{
				RestartInstanceHandler();
				BlueStacksUIUtils.RestartInstance(_VmName);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)(-1715256640 - -1715256642), "STRING_DISCARD_CHANGES", (EventHandler)delegate
			{
				((EngineSettingBaseViewModel)this).Init();
			}, (string)null, false, (object)null);
			((Window)val).ShowDialog();
		}
		else
		{
			((EngineSettingBaseViewModel)this).SaveEngineSettings("");
			((EngineSettingBaseViewModel)this).AddToastPopupUserControl(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
		}
	}

	private void RestartInstanceHandler()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		if (((EngineSettingBaseViewModel)this).EngineData.ABISetting != ((EngineSettingBaseViewModel)this).ABISetting)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] array = new object[-1054600946 ^ -1054600948];
			array[0] = "switchAbi";
			array[1] = UsefulExtensionMethod.GetDescription((Enum)(object)((EngineSettingBaseViewModel)this).ABISetting);
			text = VmCmdHandler.RunCommand(string.Format(invariantCulture, "{0} {1}", array), _VmName);
		}
		((EngineSettingBaseViewModel)this).SaveEngineSettings(text);
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)((EngineSettingBaseViewModel)this).ParentView);
	}
}
