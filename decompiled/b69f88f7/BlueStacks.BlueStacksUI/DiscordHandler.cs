using System;
using DiscordRPC;

namespace BlueStacks.BlueStacksUI;

public class DiscordHandler
{
	private DiscordRpcClient client;

	public void Initialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0073: Expected O, but got Unknown
		client = new DiscordRpcClient("1299679085059899456");
		client.Initialize();
		client.SetPresence(new RichPresence
		{
			Details = "Эмулятор без ограничений",
			Timestamps = new Timestamps
			{
				Start = DateTime.UtcNow
			},
			Assets = new Assets
			{
				LargeImageKey = "bluestacks",
				LargeImageText = "t.me/BluesterBot"
			}
		});
	}
}
