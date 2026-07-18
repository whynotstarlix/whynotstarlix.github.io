using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class PubgGameSettingViewModel : OtherAppGameSetting
{
	private InGameResolution mOldInGameResolution;

	private GraphicsQuality mOldGraphicsQuality;

	private InGameResolution mInGameResolution;

	private GraphicsQuality mGraphicsQuality;

	public InGameResolution InGameResolution
	{
		get
		{
			return mInGameResolution;
		}
		set
		{
			((ViewModelBase)this).SetProperty<InGameResolution>(ref mInGameResolution, value, (string)null);
		}
	}

	public GraphicsQuality GraphicsQuality
	{
		get
		{
			return mGraphicsQuality;
		}
		set
		{
			((ViewModelBase)this).SetProperty<GraphicsQuality>(ref mGraphicsQuality, value, (string)null);
		}
	}

	public PubgGameSettingViewModel(MainWindow parentWindow, string appName, string packageName)
		: base(parentWindow, appName, packageName)
	{
	}

	public override void Init()
	{
		base.Init();
		if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "1", StringComparison.InvariantCultureIgnoreCase))
		{
			InGameResolution = InGameResolution.HD_720p;
		}
		else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "1.5", StringComparison.InvariantCultureIgnoreCase))
		{
			InGameResolution = InGameResolution.FHD_1080p;
		}
		else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg, "2", StringComparison.InvariantCultureIgnoreCase))
		{
			InGameResolution = InGameResolution.QHD_1440p;
		}
		if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "-1", StringComparison.InvariantCultureIgnoreCase))
		{
			GraphicsQuality = GraphicsQuality.Auto;
		}
		else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "0", StringComparison.InvariantCultureIgnoreCase))
		{
			GraphicsQuality = GraphicsQuality.Smooth;
		}
		else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "1", StringComparison.InvariantCultureIgnoreCase))
		{
			GraphicsQuality = GraphicsQuality.Balanced;
		}
		else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg, "2", StringComparison.InvariantCultureIgnoreCase))
		{
			GraphicsQuality = GraphicsQuality.HD;
		}
	}

	public override void LockOriginal()
	{
		base.LockOriginal();
		mOldInGameResolution = InGameResolution;
		mOldGraphicsQuality = GraphicsQuality;
	}

	public override bool HasChanged()
	{
		if (!base.HasChanged() && InGameResolution == mOldInGameResolution)
		{
			return GraphicsQuality != mOldGraphicsQuality;
		}
		return true;
	}

	public override bool Save(bool restartReq)
	{
		restartReq = base.Save(restartReq);
		if (InGameResolution != mOldInGameResolution)
		{
			restartReq = true;
			switch (InGameResolution)
			{
			case InGameResolution.HD_720p:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "1";
				GameSettingViewModel.SendGameSettingsStat("pubg_res_720");
				break;
			case InGameResolution.FHD_1080p:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "1.5";
				GameSettingViewModel.SendGameSettingsStat("pubg_res_1080");
				break;
			case InGameResolution.QHD_1440p:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionPubg = "2";
				GameSettingViewModel.SendGameSettingsStat("pubg_res_1440");
				break;
			}
		}
		if (GraphicsQuality != mOldGraphicsQuality)
		{
			restartReq = true;
			switch (GraphicsQuality)
			{
			case GraphicsQuality.Auto:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "-1";
				GameSettingViewModel.SendGameSettingsStat("pubg_gfx_auto");
				break;
			case GraphicsQuality.Smooth:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "0";
				GameSettingViewModel.SendGameSettingsStat("pubg_gfx_smooth");
				break;
			case GraphicsQuality.Balanced:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "1";
				GameSettingViewModel.SendGameSettingsStat("pubg_gfx_balanced");
				break;
			case GraphicsQuality.HD:
				RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityPubg = "2";
				GameSettingViewModel.SendGameSettingsStat("pubg_gfx_hd");
				break;
			}
		}
		return restartReq;
	}
}
