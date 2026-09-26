using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.Player;

public static class KeyEventExts
{
	public static KeyEventArgs ToWinforms(this KeyEventArgs keyEventArgs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected Obj, but got Unknown
		Key val = (((int)keyEventArgs.Key == 156) ? keyEventArgs.SystemKey : keyEventArgs.Key);
		Keys val2 = ((KeyboardEventArgs)keyEventArgs).KeyboardDevice.Modifiers.ToWinforms();
		return new KeyEventArgs((Keys)(KeyInterop.VirtualKeyFromKey(val) | val2));
	}

	private static Keys ToWinforms(this ModifierKeys modifier)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Keys val = (Keys)0;
		if ((int)modifier == 1)
		{
			val = (Keys)(val | 0x40000);
		}
		if ((int)modifier == 2)
		{
			val = (Keys)(val | 0x20000);
		}
		if ((int)modifier == 4)
		{
			val = (Keys)(val | 0x10000);
		}
		return val;
	}
}
