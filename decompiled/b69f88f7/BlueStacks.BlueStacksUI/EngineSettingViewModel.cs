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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
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
				WindowStartupLocation = (WindowStartupLocation)2
			};
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
			val.AddButton((ButtonColors)4, "STRING_RESTART_NOW", (EventHandler)delegate
			{
				RestartInstanceHandler();
				BlueStacksUIUtils.RestartInstance(_VmName);
			}, (string)null, false, (object)null);
			val.AddButton((ButtonColors)2, "STRING_DISCARD_CHANGES", (EventHandler)delegate
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		if (((EngineSettingBaseViewModel)this).EngineData.ABISetting != ((EngineSettingBaseViewModel)this).ABISetting)
		{
			text = VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2]
			{
				"switchAbi",
				UsefulExtensionMethod.GetDescription((Enum)(object)((EngineSettingBaseViewModel)this).ABISetting)
			}), _VmName);
		}
		((EngineSettingBaseViewModel)this).SaveEngineSettings(text);
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)((EngineSettingBaseViewModel)this).ParentView);
	}
}
