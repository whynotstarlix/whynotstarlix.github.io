using System;

namespace Xantares.ResourceLayer;

public sealed class QuartzFrame
{
	public bool Ready { get; }

	public string Note { get; }

	public DateTime UntilUtc { get; }

	private QuartzFrame(bool ready, string note, DateTime untilUtc)
	{
		Ready = ready;
		Note = note ?? string.Empty;
		UntilUtc = untilUtc;
	}

	internal static QuartzFrame Ok(DateTime untilUtc)
	{
		return new QuartzFrame(ready: true, "ok", untilUtc);
	}

	internal static QuartzFrame Fail(string note)
	{
		return new QuartzFrame(ready: false, note, DateTime.MinValue);
	}
}
