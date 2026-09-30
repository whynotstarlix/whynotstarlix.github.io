using System.Collections.Generic;

namespace Kitsune_VM_Stub;

public class KitsuneState
{
	public KitsuneRegisterFile R = new KitsuneRegisterFile(65536);

	public int IP;

	public bool ZF;

	public bool SF;

	public Stack<int> CallStack = new Stack<int>();

	public bool Running = true;

	public KitsuneObjectHeap Heap;

	public long CurrentExceptionHandle;

	public Stack<int> FinallyReturnStack = new Stack<int>();
}
