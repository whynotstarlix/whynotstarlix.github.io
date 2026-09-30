namespace Kitsune_VM;

internal static class KitsuneGuardFlags
{
	public const byte AntiDebug = 1;

	public const byte AntiDump = 2;

	public const byte AntiVM = 4;

	public const byte VmCRC = 8;

	public const byte All = 15;
}
