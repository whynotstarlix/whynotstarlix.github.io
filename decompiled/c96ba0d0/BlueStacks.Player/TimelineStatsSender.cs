using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

internal class TimelineStatsSender
{
	private class TimelineEvent
	{
		public DateTime Time;

		public long Ticks;

		public string Event;

		public string S1;

		public string S2;

		public string S3;

		public TimelineEvent(string evt, string s1, string s2, string s3)
		{
			Time = DateTime.UtcNow;
			Ticks = TicksInSeconds();
			Event = evt;
			S1 = s1;
			S2 = s2;
			S3 = s3;
		}
	}

	private static Queue<TimelineEvent> sEventQueue = new Queue<TimelineEvent>();

	private static Mutex sEventQueueMutex = new Mutex();

	private static long sSequenceNumber = 1000000 * UtcToUnixTimestampSecs(DateTime.UtcNow);

	[DllImport("kernel32.dll")]
	private static extern long GetTickCount64();

	private static long UtcToUnixTimestampSecs(DateTime value)
	{
		return (long)(value - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
	}

	private static long TicksInSeconds()
	{
		return GetTickCount64() / 1000;
	}

	internal static void Init(string vmName)
	{
		if (SystemUtils.IsOSWinXP())
		{
			Logger.Warning("TimelineStats: Not supported for WindowsXP");
			return;
		}
		if (!RegistryManager.Instance.IsTimelineStats4Enabled)
		{
			Logger.Warning("TimelineStats are disabled.");
			return;
		}
		Logger.Info("TimelineStats: Initalizing: Staring thread.");
		Thread thread = new Thread(() =>
		{
			StatsSenderThread(vmName);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public static void HandleEngineBootEvent(string eventName)
	{
		if (!SystemUtils.IsOSWinXP() && RegistryManager.Instance.IsTimelineStats4Enabled)
		{
			Logger.Info("TimelineStats: UpdateEngineBootState: " + eventName);
			TimelineEvent item = new TimelineEvent("engine-boot", eventName, string.Empty, string.Empty);
			sEventQueue.Enqueue(item);
		}
	}

	internal static void SendTimelineStats(DateTime timestamp, string evt, long duration, string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, DateTime fromTimestamp, DateTime toTimestamp, long fromTicks, long toTicks, string vmName)
	{
		string timezone = TimeZone.CurrentTimeZone.DaylightName;
		string locale = Thread.CurrentThread.CurrentCulture.Name;
		Thread thread = new Thread(() =>
		{
			sEventQueueMutex.WaitOne();
			Stats.SendTimelineStats(UtcToUnixTimestampSecs(timestamp), sSequenceNumber++, evt, duration, s1, s2, s3, s4, s5, s6, s7, s8, timezone, locale, UtcToUnixTimestampSecs(fromTimestamp), UtcToUnixTimestampSecs(toTimestamp), fromTicks, toTicks, vmName);
			sEventQueueMutex.ReleaseMutex();
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void StatsSenderThread(string vmName)
	{
		DateTime utcNow = DateTime.UtcNow;
		long num = TicksInSeconds();
		while (true)
		{
			try
			{
				sEventQueueMutex.WaitOne();
				if (sEventQueue.Count <= 0)
				{
					sEventQueueMutex.ReleaseMutex();
					Thread.Sleep(1000);
				}
				else
				{
					TimelineEvent timelineEvent = sEventQueue.Dequeue();
					sEventQueueMutex.ReleaseMutex();
					SendTimelineStats(timelineEvent.Time, timelineEvent.S1, timelineEvent.Ticks - num, timelineEvent.S2, timelineEvent.S3, "", "", "", "", "", "", utcNow, timelineEvent.Time, num, timelineEvent.Ticks, vmName);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in sending timeline stats: " + ex.ToString());
				Thread.Sleep(1000);
			}
		}
	}
}
