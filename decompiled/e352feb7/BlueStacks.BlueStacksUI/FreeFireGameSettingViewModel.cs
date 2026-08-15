using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class FreeFireGameSettingViewModel : OtherAppGameSetting
{
	private readonly MainWindow mParentWindow;

	private bool mOldOptimizeInGameSetting;

	private bool mOptimizeInGameSetting;

	public bool OptimizeInGameSetting
	{
		get
		{
			return mOptimizeInGameSetting;
		}
		set
		{
			((ViewModelBase)this).SetProperty<bool>(ref mOptimizeInGameSetting, value, (string)null);
		}
	}

	public FreeFireGameSettingViewModel(MainWindow parentWindow, string appName, string packageName)
		: base(parentWindow, appName, packageName)
	{
		mParentWindow = parentWindow;
	}

	public override void Init()
	{
		base.Init();
		OptimizeInGameSetting = mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized;
	}

	public override void LockOriginal()
	{
		base.LockOriginal();
		mOldOptimizeInGameSetting = OptimizeInGameSetting;
	}

	public override bool HasChanged()
	{
		if (!base.HasChanged())
		{
			return OptimizeInGameSetting != mOldOptimizeInGameSetting;
		}
		return true;
	}

	public override bool Save(bool restartReq)
	{
		restartReq = base.Save(restartReq);
		if (OptimizeInGameSetting != mOldOptimizeInGameSetting)
		{
			mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized = OptimizeInGameSetting;
			GameSettingViewModel.SendGameSettingsEnabledToGuest(mParentWindow, OptimizeInGameSetting);
			GameSettingViewModel.SendGameSettingsStat(OptimizeInGameSetting ? "freefire_optimizegame_enabled" : "freefire_optimizegame_disabled");
		}
		return restartReq;
	}
}
