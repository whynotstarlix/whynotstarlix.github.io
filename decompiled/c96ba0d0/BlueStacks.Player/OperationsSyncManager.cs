using System.Collections.Generic;

namespace BlueStacks.Player;

internal sealed class OperationsSyncManager
{
	private static volatile OperationsSyncManager sInstance;

	private static object syncRoot = new object();

	internal static bool mIsBroadcasting = false;

	internal static bool mIsReceiving = false;

	public static OperationsSyncManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot)
				{
					if (sInstance == null)
					{
						sInstance = new OperationsSyncManager();
					}
				}
			}
			return sInstance;
		}
	}

	private OperationsSyncManager()
	{
	}

	private string GetPipeName(string fromVM, string toVM)
	{
		return "Sync" + fromVM + toVM;
	}

	internal void StartSyncToInstances(IEnumerable<string> instances)
	{
	}

	internal void StopOperationsSync()
	{
		mIsBroadcasting = false;
	}

	internal void StartSyncConsumer(string fromInstance)
	{
		mIsReceiving = true;
	}

	internal void StopSyncConsumer()
	{
		mIsReceiving = false;
	}

	internal void PlayPauseOperationsSync(bool isPause)
	{
		if (isPause)
		{
			mIsBroadcasting = false;
		}
		else
		{
			mIsBroadcasting = true;
		}
	}
}
