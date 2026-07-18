using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Installer;

public class NotificationWindow : Window, IComponentConnector
{
	public enum _200E_200F_200F_200E
	{
		_200F_200F_200F_200E,
		_2060_200F_200F_200E,
		_2061_200F_200F_200E,
		_2062_200F_200F_200E
	}

	private delegate void _2063_200F_200F_200E(string P_0, _200E_200F_200F_200E P_1, Window P_2);

	private delegate void _2064_200F_200F_200E(NotificationWindow P_0, string P_1, _200E_200F_200F_200E P_2, Window P_3);

	private delegate void _FEFF_200F_200F_200E(NotificationWindow P_0, object P_1, RoutedEventArgs P_2);

	private delegate void _200B_2060_200F_200E(NotificationWindow P_0);

	private delegate void _200C_2060_200F_200E(NotificationWindow P_0);

	private delegate void _200D_2060_200F_200E(NotificationWindow P_0, _200E_200F_200F_200E P_1);

	private delegate void _200E_2060_200F_200E(NotificationWindow P_0, object P_1, EventArgs P_2);

	private delegate void _200F_2060_200F_200E(NotificationWindow P_0);

	private delegate void _2060_2060_200F_200E(NotificationWindow P_0);

	private delegate void _2061_2060_200F_200E(NotificationWindow P_0, object P_1, EventArgs P_2);

	private delegate void _2062_2060_200F_200E(NotificationWindow P_0);

	private delegate void _2063_2060_200F_200E(NotificationWindow P_0, int P_1, object P_2);

	private delegate void _2064_2060_200F_200E();

	private static double _2061_200E_200F_200E;

	private static double _2062_200E_200F_200E;

	private static double _2063_200E_200F_200E;

	private static double _2064_200E_200F_200E;

	private static double _FEFF_200E_200F_200E;

	private static double _200B_200F_200F_200E;

	private readonly Window _200C_200F_200F_200E;

	private readonly DispatcherTimer _200D_200F_200F_200E;

	internal Grid RootGrid;

	internal TranslateTransform RootTranslate;

	internal Border RootBorder;

	internal TextBlock MessageText;

	private bool _contentLoaded;

	public unsafe static void _200B_200E_200F_200E(string P_0, _200E_200F_200F_200E P_1, Window P_2)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		_ = ~/*Error near IL_004d: Stack underflow*/ - /*Error near IL_004d: ldloc 115 (out-of-bounds)*/;
		/*Error near IL_0050: Unknown opcode: 0xE9*/;
	}

	private unsafe NotificationWindow(string P_0, _200E_200F_200F_200E P_1, Window P_2)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		/*OpCode not supported: DebugBreak*/;
		/*Error near IL_004d: Unknown opcode: 0xA7*/;
	}

	private unsafe void OnLoaded(object P_0, RoutedEventArgs P_1)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		//IL_004d: Invalid comparison between Unknown and I8
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		if ((long)/*Error near IL_004f: Stack underflow*/ < (long)checked((ulong)/*Error near IL_004d: Stack underflow*/))
		{
			checked
			{
				_ = (byte)/*Error near IL_0050: Stack underflow*/;
				/*Error near IL_0051: Unknown opcode: 0x78*/;
			}
		}
		/*Error near IL_00a3: Invalid metadata token*/;
	}

	private unsafe void _200C_200E_200F_200E()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		if (/*Error near IL_004e: Stack underflow*/ < /*Error near IL_004e: Stack underflow*/)
		{
			*(_003F*)(nint)/*Error near IL_004f: Stack underflow*/ = /*Error near IL_004f: Stack underflow*/;
			checked
			{
				_ = (byte)/*Error near IL_0050: Stack underflow*/;
				/*Error near IL_0051: Unknown opcode: 0x78*/;
			}
		}
		_ = (short)(*(ushort*)1);
		/*Error near IL_0065: Read out of bounds.*/;
	}

	private unsafe void _200D_200E_200F_200E()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		/*Error near IL_004c: stloc 22 (out-of-bounds)*/;
		*(_003F*)(nint)/*Error near IL_004f: Stack underflow*/ = /*Error near IL_004f: Stack underflow*/;
		checked
		{
			_ = (byte)/*Error near IL_0050: Stack underflow*/;
			/*Error near IL_0051: Unknown opcode: 0x78*/;
		}
	}

	private unsafe void _200E_200E_200F_200E(_200E_200F_200F_200E P_0)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		_ = *(sbyte*)(nint)/*Error near IL_004d: Stack underflow*/;
		/*Error near IL_004d: Unknown opcode: 0xE7*/;
	}

	private unsafe void OnCloseTimerTick(object P_0, EventArgs P_1)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		int num = default(int);
		_ = (double)(int)(*(ushort*)num);
		_ = ((short[])(*(float*)5))[(object)this];
		/*Error near IL_0053: Unknown opcode: 0xF0*/;
	}

	private unsafe void _200F_200E_200F_200E()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		/*Error near IL_004c: Unknown opcode: 0xF7*/;
	}

	private unsafe void _2060_200E_200F_200E()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		/*Error near IL_004c: Unknown opcode: 0xF7*/;
	}

	private unsafe void OnDisappearCompleted(object P_0, EventArgs P_1)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		_ = (float)(sbyte)/*Error near IL_004d: Stack underflow*/;
		/*Error near IL_004e: Not a type handle*/;
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public unsafe void InitializeComponent()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		//IL_004d: Expected native int or pointer, but got O
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		byte[] array = default(byte[]);
		*(int*)(nint)/*Error near IL_004f: Stack underflow*/ = *(sbyte*)(nint)array;
		_ = *(uint*)(nint)(/*Error near IL_0050: Stack underflow*/ * /*Error near IL_0050: Stack underflow*/);
		/*Error near IL_0051: starg 56 (out-of-bounds)*/;
		if (/*Error near IL_0058: Stack underflow*/ > /*Error near IL_0058: Stack underflow*/)
		{
			/*Error: Invalid branch target*/;
		}
		_ = (float)/*Error near IL_0059: Stack underflow*/;
		/*Error near IL_0059: Metadata token must be either a methoddef, memberref or methodspec*/;
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	unsafe void IComponentConnector.Connect(int P_0, object P_1)
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		_ = (float)(sbyte)/*Error near IL_004d: Stack underflow*/;
		/*Error near IL_004e: Not a type handle*/;
	}

	unsafe static NotificationWindow()
	{
		//IL_0001: Invalid comparison between Unknown and I4
		if ((int)/*Error near IL_0003: Stack underflow*/ == *(short*)(nint)/*Error near IL_0001: Stack underflow*/)
		{
			/*Error near IL_0003: Invalid branch target*/;
		}
		/*Error near IL_004c: Unknown opcode: 0x24*/;
	}
}
