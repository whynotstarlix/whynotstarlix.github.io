namespace BlueStacks.BlueStacksUI;

internal interface ITopBar
{
	string AppName { get; set; }

	string CharacterName { get; set; }

	void ShowSyncPanel(bool show = false);

	void HideSyncPanel();
}
