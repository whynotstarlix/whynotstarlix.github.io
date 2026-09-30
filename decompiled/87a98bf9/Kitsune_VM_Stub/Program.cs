using System;

namespace Kitsune_VM_Stub;

internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine("=== Kitsune VM — test ===");
		Console.WriteLine();
		Test_SimpleArithmetic();
		Test_Loop();
		Test_FunctionCall();
		Test_Conditionals();
		Test_ExternalCall();
		Console.WriteLine();
		Console.WriteLine("All tests passed successfully.");
		Console.ReadKey();
	}

	private static void Test_SimpleArithmetic()
	{
		KitsuneAssembler kitsuneAssembler = new KitsuneAssembler();
		kitsuneAssembler.MovImm(0, 5L);
		kitsuneAssembler.MovImm(1, 3L);
		kitsuneAssembler.Add(2, 0, 1);
		kitsuneAssembler.MovImm(3, 2L);
		kitsuneAssembler.Mul(0, 2, 3);
		kitsuneAssembler.RetVal(0);
		KitsuneDispatcher kitsuneDispatcher = new KitsuneDispatcher(kitsuneAssembler.Build());
		kitsuneDispatcher.Run();
		long num = kitsuneDispatcher.State.R[0];
		Console.WriteLine("Test 1 (arithmetic): (5+3)*2 = " + num + ((num == 16) ? " [OK]" : " [FAIL]"));
	}

	private static void Test_Loop()
	{
		KitsuneAssembler kitsuneAssembler = new KitsuneAssembler();
		kitsuneAssembler.MovImm(0, 0L);
		kitsuneAssembler.MovImm(1, 1L);
		kitsuneAssembler.MovImm(2, 5L);
		kitsuneAssembler.MovImm(3, 1L);
		int currentPosition = kitsuneAssembler.CurrentPosition;
		kitsuneAssembler.Cmp(1, 2);
		kitsuneAssembler.JgtFlag("loop_end");
		kitsuneAssembler.Add(0, 0, 1);
		kitsuneAssembler.Add(1, 1, 3);
		kitsuneAssembler.Jmp(currentPosition);
		kitsuneAssembler.Label("loop_end");
		kitsuneAssembler.RetVal(0);
		KitsuneDispatcher kitsuneDispatcher = new KitsuneDispatcher(kitsuneAssembler.Build());
		kitsuneDispatcher.Run();
		long num = kitsuneDispatcher.State.R[0];
		Console.WriteLine("Test 2 (loop 1..5):   sum = " + num + ((num == 15) ? " [OK]" : " [FAIL]"));
	}

	private static void Test_FunctionCall()
	{
		KitsuneAssembler kitsuneAssembler = new KitsuneAssembler();
		kitsuneAssembler.MovImm(0, 5L);
		kitsuneAssembler.Call("factorial");
		kitsuneAssembler.RetVal(0);
		kitsuneAssembler.Halt();
		kitsuneAssembler.Label("factorial");
		kitsuneAssembler.MovReg(1, 0);
		kitsuneAssembler.MovImm(0, 1L);
		kitsuneAssembler.MovImm(4, 1L);
		kitsuneAssembler.MovImm(5, 1L);
		int currentPosition = kitsuneAssembler.CurrentPosition;
		kitsuneAssembler.Cmp(1, 5);
		kitsuneAssembler.JleFlag("fact_end");
		kitsuneAssembler.Mul(0, 0, 1);
		kitsuneAssembler.Sub(1, 1, 4);
		kitsuneAssembler.Jmp(currentPosition);
		kitsuneAssembler.Label("fact_end");
		kitsuneAssembler.RetVal(0);
		KitsuneDispatcher kitsuneDispatcher = new KitsuneDispatcher(kitsuneAssembler.Build());
		kitsuneDispatcher.Run();
		long num = kitsuneDispatcher.State.R[0];
		Console.WriteLine("Test 3 (factorial 5!): " + num + ((num == 120) ? " [OK]" : " [FAIL]"));
	}

	private static void Test_Conditionals()
	{
		KitsuneAssembler kitsuneAssembler = new KitsuneAssembler();
		kitsuneAssembler.MovImm(0, 7L);
		kitsuneAssembler.MovImm(1, 3L);
		kitsuneAssembler.Cmp(0, 1);
		kitsuneAssembler.JgtFlag("take_r0");
		kitsuneAssembler.MovReg(0, 1);
		kitsuneAssembler.Label("take_r0");
		kitsuneAssembler.RetVal(0);
		KitsuneDispatcher kitsuneDispatcher = new KitsuneDispatcher(kitsuneAssembler.Build());
		kitsuneDispatcher.Run();
		long num = kitsuneDispatcher.State.R[0];
		Console.WriteLine("Test 4 (max(7,3)):     " + num + ((num == 7) ? " [OK]" : " [FAIL]"));
	}

	private static void Test_ExternalCall()
	{
		KitsuneExternalCallTable kitsuneExternalCallTable = new KitsuneExternalCallTable();
		kitsuneExternalCallTable.Register(1, (KitsuneState state, int[] argRegs, int retReg) =>
		{
			Console.Write((char)state.R[argRegs[0]]);
		});
		KitsuneAssembler kitsuneAssembler = new KitsuneAssembler();
		kitsuneAssembler.MovImm(0, 72L);
		kitsuneAssembler.ExternalCall(1, new int[1], -1);
		kitsuneAssembler.MovImm(0, 105L);
		kitsuneAssembler.ExternalCall(1, new int[1], -1);
		kitsuneAssembler.MovImm(0, 10L);
		kitsuneAssembler.ExternalCall(1, new int[1], -1);
		kitsuneAssembler.Halt();
		byte[] bytecode = kitsuneAssembler.Build();
		Console.Write("Test 5 (external call): VM printed: ");
		new KitsuneDispatcher(bytecode, kitsuneExternalCallTable).Run();
		Console.WriteLine("[OK]");
	}
}
