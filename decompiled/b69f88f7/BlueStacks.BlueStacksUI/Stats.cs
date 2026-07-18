using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

internal static class Stats
{
	private static string sSessionId;

	private static string SessionId
	{
		get
		{
			if (sSessionId == null)
			{
				ResetSessionId();
			}
			return sSessionId;
		}
		set
		{
			sSessionId = value;
		}
	}

	private static string Timestamp
	{
		get
		{
			long num = DateTime.Now.Ticks - DateTime.Parse("01/01/1970 00:00:00", CultureInfo.InvariantCulture).Ticks;
			return (num / 10000000).ToString(CultureInfo.InvariantCulture);
		}
	}

	public static string GetSessionId()
	{
		return SessionId;
	}

	public static string ResetSessionId()
	{
		SessionId = Timestamp;
		return SessionId;
	}
}
