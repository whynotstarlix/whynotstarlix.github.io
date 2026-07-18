using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class OtherAppGameSetting : ViewModelBase
{
	private string mMouseSenstivity;

	private Type mPropertyType;

	private ObservableCollection<IMActionItem> mIMActionItems = new ObservableCollection<IMActionItem>();

	private string mOldMouseSenstivity;

	private Visibility mShowSensitivity = (Visibility)2;

	private bool mPlayInLandscapeMode;

	private Visibility mPlayInLandscapeModeVisibility = (Visibility)2;

	private bool mPlayInPortraitMode;

	private Visibility mPlayInPortraitModeVisibility = (Visibility)2;

	public string AppName { get; }

	public string PackageName { get; }

	public MainWindow ParentWindow { get; }

	public string MouseSenstivity
	{
		get
		{
			return mMouseSenstivity;
		}
		set
		{
			((ViewModelBase)this).SetProperty<string>(ref mMouseSenstivity, value, (string)null);
		}
	}

	public Type SensitivityPropertyType
	{
		get
		{
			return mPropertyType;
		}
		set
		{
			((ViewModelBase)this).SetProperty<Type>(ref mPropertyType, value, (string)null);
		}
	}

	public ObservableCollection<IMActionItem> SensitivityIMActionItems
	{
		get
		{
			return mIMActionItems;
		}
		set
		{
			((ViewModelBase)this).SetProperty<ObservableCollection<IMActionItem>>(ref mIMActionItems, value, (string)null);
		}
	}

	public Visibility ShowSensitivity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mShowSensitivity;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<Visibility>(ref mShowSensitivity, value, (string)null);
		}
	}

	public bool PlayInLandscapeMode
	{
		get
		{
			return mPlayInLandscapeMode;
		}
		set
		{
			((ViewModelBase)this).SetProperty<bool>(ref mPlayInLandscapeMode, value, (string)null);
		}
	}

	public Visibility PlayInLandscapeModeVisibility
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mPlayInLandscapeModeVisibility;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<Visibility>(ref mPlayInLandscapeModeVisibility, value, (string)null);
		}
	}

	public bool PlayInPortraitMode
	{
		get
		{
			return mPlayInPortraitMode;
		}
		set
		{
			((ViewModelBase)this).SetProperty<bool>(ref mPlayInPortraitMode, value, (string)null);
		}
	}

	public Visibility PlayInPortraitModeVisibility
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mPlayInPortraitModeVisibility;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<Visibility>(ref mPlayInPortraitModeVisibility, value, (string)null);
		}
	}

	private void InitMouseSenstivity()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		IMAction iMAction = ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Where((IMAction item) => (object)item.GetType() == typeof(Pan)).FirstOrDefault();
		if (iMAction != null && iMAction is Pan pan && (pan.Tweaks & 0x20) == 0)
		{
			mShowSensitivity = (Visibility)0;
			SensitivityPropertyType = IMAction.DictPropertyInfo[iMAction.Type]["Sensitivity"].PropertyType;
			MouseSenstivity = iMAction["Sensitivity"].ToString();
			SensitivityIMActionItems.Add(new IMActionItem
			{
				ActionItem = "Sensitivity",
				IMAction = iMAction
			});
		}
		else
		{
			mShowSensitivity = (Visibility)2;
		}
	}

	public OtherAppGameSetting(MainWindow parentWindow, string appName, string packageName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		AppName = appName;
		PackageName = packageName;
		ParentWindow = parentWindow;
	}

	public virtual void Init()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		string cloudOrientationForPackage = GuidanceCloudInfoManager.GetCloudOrientationForPackage(PackageName);
		PlayInLandscapeModeVisibility = (Visibility)2;
		PlayInPortraitModeVisibility = (Visibility)2;
		if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName] = new AppSettings();
		}
		switch (cloudOrientationForPackage.ToLowerInvariant())
		{
		case "landscape":
			PlayInLandscapeModeVisibility = (Visibility)0;
			PlayInLandscapeMode = AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedLandscapeEnabled;
			break;
		case "portrait":
			PlayInPortraitModeVisibility = (Visibility)0;
			PlayInPortraitMode = AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedPortraitEnabled;
			break;
		}
		InitMouseSenstivity();
	}

	public virtual void LockOriginal()
	{
		mOldMouseSenstivity = MouseSenstivity;
	}

	public virtual bool HasChanged()
	{
		if (!HasLandscapeModeChanged() && !HasPortraitModeChanged())
		{
			return MouseSenstivity != mOldMouseSenstivity;
		}
		return true;
	}

	public bool HasLandscapeModeChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)PlayInLandscapeModeVisibility == 0 && AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			return AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedLandscapeEnabled != PlayInLandscapeMode;
		}
		return false;
	}

	public bool HasPortraitModeChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)PlayInPortraitModeVisibility == 0 && AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			return AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedPortraitEnabled != PlayInPortraitMode;
		}
		return false;
	}

	private bool HasMouseSenstivityChanged()
	{
		return mOldMouseSenstivity != MouseSenstivity;
	}

	public virtual bool Save(bool restartReq)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (!AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName].ContainsKey(PackageName))
		{
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName] = new AppSettings();
		}
		if (HasLandscapeModeChanged())
		{
			Utils.SetCustomAppSize(ParentWindow.mVmName, PackageName, (ScreenMode)((!PlayInLandscapeMode) ? 2 : 0));
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedLandscapeEnabled = PlayInLandscapeMode;
			KMManager.SelectSchemeIfPresent(ParentWindow, PlayInLandscapeMode ? "Landscape" : "Portrait", "gamesettings", forceSave: false);
			ClientStats.SendMiscellaneousStatsAsync("client_game_settings", RegistryManager.Instance.UserGuid, "landscapeMode", RegistryManager.Instance.ClientVersion, PackageName, PlayInLandscapeMode ? ((object)(ScreenMode)0/*cast due to constrained. prefix*/).ToString() : ((object)(ScreenMode)2/*cast due to constrained. prefix*/).ToString(), RegistryManager.Instance.Oem);
			restartReq = true;
		}
		if (HasPortraitModeChanged())
		{
			Utils.SetCustomAppSize(ParentWindow.mVmName, PackageName, (ScreenMode)(PlayInPortraitMode ? 1 : 2));
			AppConfigurationManager.Instance.VmAppConfig[ParentWindow.mVmName][PackageName].IsForcedPortraitEnabled = PlayInPortraitMode;
			KMManager.SelectSchemeIfPresent(ParentWindow, PlayInPortraitMode ? "Portrait" : "Landscape", "gamesettings", forceSave: false);
			ClientStats.SendMiscellaneousStatsAsync("client_game_settings", RegistryManager.Instance.UserGuid, "portraitMode", RegistryManager.Instance.ClientVersion, PackageName, PlayInPortraitMode ? ((object)(ScreenMode)1/*cast due to constrained. prefix*/).ToString() : ((object)(ScreenMode)2/*cast due to constrained. prefix*/).ToString(), RegistryManager.Instance.Oem);
			restartReq = true;
		}
		if (HasMouseSenstivityChanged())
		{
			KeymapCanvasWindow.sIsDirty = true;
			KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: true);
			if (KMManager.sGuidanceWindow != null && !((CustomWindow)KMManager.sGuidanceWindow).IsClosed && ((UIElement)KMManager.sGuidanceWindow).IsVisible)
			{
				KMManager.sGuidanceWindow.InitUI();
			}
			ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "mouseSenstivityChanged", string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, PackageName);
			InitMouseSenstivity();
		}
		return restartReq;
	}

	public void Reset()
	{
		string schemeName = ParentWindow.SelectedConfig.SelectedControlScheme.Name;
		IEnumerable<IMControlScheme> source = ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme_) => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase));
		if (!source.Any())
		{
			return;
		}
		IMControlScheme iMControlScheme = ((source.Count() == 1) ? source.First() : source.Where((IMControlScheme scheme_) => !scheme_.BuiltIn).First());
		if (iMControlScheme.BuiltIn)
		{
			ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
			IMControlScheme iMControlScheme2 = ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture)).FirstOrDefault();
			if (iMControlScheme2 != null)
			{
				iMControlScheme2.Selected = true;
				ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme2;
				ParentWindow.SelectedConfig.ControlSchemesDict[iMControlScheme2.Name] = iMControlScheme2;
			}
		}
		else
		{
			ParentWindow.SelectedConfig.ControlSchemes.Remove(ParentWindow.SelectedConfig.SelectedControlScheme);
			ParentWindow.SelectedConfig.SelectedControlScheme = iMControlScheme.DeepCopy();
			ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = ParentWindow.SelectedConfig.SelectedControlScheme;
			ParentWindow.SelectedConfig.ControlSchemes.Add(ParentWindow.SelectedConfig.SelectedControlScheme);
		}
	}
}
