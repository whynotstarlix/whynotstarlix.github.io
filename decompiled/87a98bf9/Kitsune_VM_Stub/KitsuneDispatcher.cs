using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Kitsune_VM_Stub;

public class KitsuneDispatcher
{
	private byte[] _bytecode;

	private KitsuneExternalCallTable _externalCalls;

	private KitsuneHeapRuntime _heapRuntime;

	private KitsuneEHEntry[] _ehTable;

	private byte[] _reversePerm;

	internal Action[] _jt;

	private int _shadowCtr;

	private int _shadowStep;

	private long _shadowSeed;

	private uint _bytecodeSig;

	private int _integrityCheckCtr;

	private int _integrityCheckStep;

	private static readonly uint[] _crc32Table = BuildCrc32Table();

	public KitsuneState State { get; private set; }

	public KitsuneDispatcher(byte[] bytecode, KitsuneExternalCallTable externalCalls = null, KitsuneHeapRuntime heapRuntime = null, KitsuneEHEntry[] ehTable = null, byte[] reversePerm = null)
	{
		_bytecode = bytecode;
		State = new KitsuneState();
		_externalCalls = externalCalls ?? new KitsuneExternalCallTable();
		_heapRuntime = heapRuntime ?? KitsuneHeapRuntime.Empty();
		_ehTable = ehTable ?? new KitsuneEHEntry[0];
		_reversePerm = reversePerm;
		State.Heap = _heapRuntime.Heap;
		Random random = new Random();
		_shadowStep = 7 + random.Next(9);
		_shadowSeed = ((long)random.Next() << 32) | (uint)random.Next();
		int length = Math.Min(64, bytecode.Length);
		_bytecodeSig = ComputeCrc32Sig(_bytecode, 0, length);
		_integrityCheckStep = 200 + random.Next(200);
	}

	public void Run()
	{
		State.Running = true;
		while (State.Running)
		{
			if (State.IP >= _bytecode.Length)
			{
				throw new Exception("KitsuneVM: IP past end of bytecode (HALT missing?)");
			}
			byte b = ReadByte();
			KitsuneOpcode op = (KitsuneOpcode)((_reversePerm != null) ? _reversePerm[b] : b);
			Execute(op);
			if (++_shadowCtr >= _shadowStep)
			{
				_shadowCtr = 0;
				int i = 32768 + (int)((State.R[0] + State.IP) * 6364136223846793005L + 1442695040888963407L >>> 49);
				State.R[i] = State.IP ^ _shadowSeed ^ State.R[0];
			}
			if (++_integrityCheckCtr >= _integrityCheckStep)
			{
				_integrityCheckCtr = 0;
				int length = Math.Min(64, _bytecode.Length);
				if (ComputeCrc32Sig(_bytecode, 0, length) != _bytecodeSig)
				{
					throw new Exception("KitsuneVM: bytecode integrity check failed.");
				}
			}
		}
	}

	private void Execute(KitsuneOpcode op)
	{
		switch (op)
		{
		case KitsuneOpcode.MOV_REG_IMM:
			H_MOV_REG_IMM();
			break;
		case KitsuneOpcode.MOV_REG_REG:
			H_MOV_REG_REG();
			break;
		case KitsuneOpcode.ADD:
			H_ADD();
			break;
		case KitsuneOpcode.SUB:
			H_SUB();
			break;
		case KitsuneOpcode.MUL:
			H_MUL();
			break;
		case KitsuneOpcode.DIV:
			H_DIV();
			break;
		case KitsuneOpcode.MOD:
			H_MOD();
			break;
		case KitsuneOpcode.NEG:
			H_NEG();
			break;
		case KitsuneOpcode.AND:
			H_AND();
			break;
		case KitsuneOpcode.OR:
			H_OR();
			break;
		case KitsuneOpcode.XOR:
			H_XOR();
			break;
		case KitsuneOpcode.NOT:
			H_NOT();
			break;
		case KitsuneOpcode.SHL:
			H_SHL();
			break;
		case KitsuneOpcode.SHR:
			H_SHR();
			break;
		case KitsuneOpcode.CMP:
			H_CMP();
			break;
		case KitsuneOpcode.SET_ZF:
			H_SET_ZF();
			break;
		case KitsuneOpcode.SET_LT:
			H_SET_LT();
			break;
		case KitsuneOpcode.SET_GT:
			H_SET_GT();
			break;
		case KitsuneOpcode.JMP:
			H_JMP();
			break;
		case KitsuneOpcode.JZ:
			H_JZ();
			break;
		case KitsuneOpcode.JNZ:
			H_JNZ();
			break;
		case KitsuneOpcode.JZ_FLAG:
			H_JZ_FLAG();
			break;
		case KitsuneOpcode.JNZ_FLAG:
			H_JNZ_FLAG();
			break;
		case KitsuneOpcode.JLT_FLAG:
			H_JLT_FLAG();
			break;
		case KitsuneOpcode.JGT_FLAG:
			H_JGT_FLAG();
			break;
		case KitsuneOpcode.JLE_FLAG:
			H_JLE_FLAG();
			break;
		case KitsuneOpcode.JGE_FLAG:
			H_JGE_FLAG();
			break;
		case KitsuneOpcode.CALL:
			H_CALL();
			break;
		case KitsuneOpcode.RET_VOID:
			H_RET_VOID();
			break;
		case KitsuneOpcode.RET_VAL:
			H_RET_VAL();
			break;
		case KitsuneOpcode.EXTERNAL_CALL:
			H_EXTERNAL_CALL();
			break;
		case KitsuneOpcode.HALT:
			H_HALT();
			break;
		case KitsuneOpcode.LDSTR:
			H_LDSTR();
			break;
		case KitsuneOpcode.NEWOBJ:
			H_NEWOBJ();
			break;
		case KitsuneOpcode.LDFLD:
			H_LDFLD();
			break;
		case KitsuneOpcode.STFLD:
			H_STFLD();
			break;
		case KitsuneOpcode.LDSFLD:
			H_LDSFLD();
			break;
		case KitsuneOpcode.STSFLD:
			H_STSFLD();
			break;
		case KitsuneOpcode.NEWARR:
			H_NEWARR();
			break;
		case KitsuneOpcode.LDLEN:
			H_LDLEN();
			break;
		case KitsuneOpcode.LDELEM:
			H_LDELEM();
			break;
		case KitsuneOpcode.STELEM:
			H_STELEM();
			break;
		case KitsuneOpcode.ISINST:
			H_ISINST();
			break;
		case KitsuneOpcode.CASTCLASS:
			H_CASTCLASS();
			break;
		case KitsuneOpcode.BOX:
			H_BOX();
			break;
		case KitsuneOpcode.UNBOX_ANY:
			H_UNBOX_ANY();
			break;
		case KitsuneOpcode.THROW:
			H_THROW();
			break;
		case KitsuneOpcode.RETHROW:
			H_RETHROW();
			break;
		case KitsuneOpcode.LEAVE:
			H_LEAVE();
			break;
		case KitsuneOpcode.END_FINALLY:
			H_END_FINALLY();
			break;
		case KitsuneOpcode.LOAD_EXCEPTION:
			H_LOAD_EXCEPTION();
			break;
		case KitsuneOpcode.LDTOKEN:
			H_LDTOKEN();
			break;
		case KitsuneOpcode.CONV_U1:
			H_CONV_U1();
			break;
		case KitsuneOpcode.CONV_I1:
			H_CONV_I1();
			break;
		case KitsuneOpcode.CONV_U2:
			H_CONV_U2();
			break;
		case KitsuneOpcode.CONV_I2:
			H_CONV_I2();
			break;
		case KitsuneOpcode.LDFTN:
			H_LDFTN();
			break;
		case KitsuneOpcode.LDIND:
			H_LDIND();
			break;
		case KitsuneOpcode.STIND:
			H_STIND();
			break;
		case KitsuneOpcode.NOP_A:
			H_NOP_A();
			break;
		case KitsuneOpcode.NOP_B:
			H_NOP_B();
			break;
		default:
			throw new Exception($"KitsuneVM: unknown opcode 0x{(byte)op:X2} at IP={State.IP - 1}");
		}
	}

	private void H_MOV_REG_IMM()
	{
		int i = ReadReg();
		long value = ReadInt64();
		State.R[i] = value;
	}

	private void H_MOV_REG_REG()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i] = State.R[i2];
	}

	private void H_ADD()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] + State.R[i3];
	}

	private void H_SUB()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] - State.R[i3];
	}

	private void H_MUL()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] * State.R[i3];
	}

	private void H_DIV()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		if (State.R[i3] == 0L)
		{
			throw new DivideByZeroException("KitsuneVM DIV: division by zero");
		}
		State.R[i] = State.R[i2] / State.R[i3];
	}

	private void H_MOD()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		if (State.R[i3] == 0L)
		{
			throw new DivideByZeroException("KitsuneVM MOD: modulo by zero");
		}
		State.R[i] = State.R[i2] % State.R[i3];
	}

	private void H_NEG()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i] = -State.R[i2];
	}

	private void H_AND()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] & State.R[i3];
	}

	private void H_OR()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] | State.R[i3];
	}

	private void H_XOR()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] ^ State.R[i3];
	}

	private void H_NOT()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i] = ~State.R[i2];
	}

	private void H_SHL()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] << (int)State.R[i3];
	}

	private void H_SHR()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int i3 = ReadReg();
		State.R[i] = State.R[i2] >> (int)State.R[i3];
	}

	private void H_CMP()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		long num = State.R[i];
		long num2 = State.R[i2];
		State.ZF = num == num2;
		State.SF = num < num2;
	}

	private void H_SET_ZF()
	{
		int i = ReadReg();
		State.R[i] = (State.ZF ? 1 : 0);
	}

	private void H_SET_LT()
	{
		int i = ReadReg();
		State.R[i] = (State.SF ? 1 : 0);
	}

	private void H_SET_GT()
	{
		int i = ReadReg();
		State.R[i] = ((!State.ZF && !State.SF) ? 1 : 0);
	}

	private void H_JMP()
	{
		State.IP = ReadInt32();
	}

	private void H_JZ()
	{
		int i = ReadReg();
		int iP = ReadInt32();
		if (State.R[i] == 0L)
		{
			State.IP = iP;
		}
	}

	private void H_JNZ()
	{
		int i = ReadReg();
		int iP = ReadInt32();
		if (State.R[i] != 0L)
		{
			State.IP = iP;
		}
	}

	private void H_JZ_FLAG()
	{
		int iP = ReadInt32();
		if (State.ZF)
		{
			State.IP = iP;
		}
	}

	private void H_JNZ_FLAG()
	{
		int iP = ReadInt32();
		if (!State.ZF)
		{
			State.IP = iP;
		}
	}

	private void H_JLT_FLAG()
	{
		int iP = ReadInt32();
		if (State.SF)
		{
			State.IP = iP;
		}
	}

	private void H_JGT_FLAG()
	{
		int iP = ReadInt32();
		if (!State.ZF && !State.SF)
		{
			State.IP = iP;
		}
	}

	private void H_JLE_FLAG()
	{
		int iP = ReadInt32();
		if (State.ZF || State.SF)
		{
			State.IP = iP;
		}
	}

	private void H_JGE_FLAG()
	{
		int iP = ReadInt32();
		if (!State.SF)
		{
			State.IP = iP;
		}
	}

	private void H_CALL()
	{
		int iP = ReadInt32();
		State.CallStack.Push(State.IP);
		State.IP = iP;
	}

	private void H_RET_VOID()
	{
		if (State.CallStack.Count == 0)
		{
			State.Running = false;
		}
		else
		{
			State.IP = State.CallStack.Pop();
		}
	}

	private void H_RET_VAL()
	{
		int i = ReadReg();
		long value = State.R[i];
		if (State.CallStack.Count == 0)
		{
			State.R[0] = value;
			State.Running = false;
		}
		else
		{
			State.IP = State.CallStack.Pop();
			State.R[0] = value;
		}
	}

	private void H_EXTERNAL_CALL()
	{
		int id = ReadInt32();
		int num = ReadByte();
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadReg();
		}
		int num2 = ReadReg();
		if (num2 == 65535)
		{
			num2 = -1;
		}
		try
		{
			_externalCalls.Invoke(id, State, array, num2);
		}
		catch (Exception ex)
		{
			int iP = State.IP;
			if (!DispatchException(ex, iP))
			{
				throw;
			}
		}
	}

	private void H_HALT()
	{
		State.Running = false;
	}

	private void H_LDSTR()
	{
		int id = ReadInt32();
		int i = ReadReg();
		State.R[i] = _heapRuntime.LoadString(id);
	}

	private void H_NEWOBJ()
	{
		int ctorId = ReadInt32();
		int num = ReadByte();
		long[] array = new long[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = State.R[ReadReg()];
		}
		int i2 = ReadReg();
		State.R[i2] = _heapRuntime.CreateObject(ctorId, array);
	}

	private void H_LDFLD()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		FieldInfo fieldInfo = _heapRuntime.Fields[num];
		long num2 = State.R[i];
		if (num2 == 0L && fieldInfo.DeclaringType != null && fieldInfo.DeclaringType.IsValueType)
		{
			State.R[i2] = 0L;
		}
		else
		{
			State.R[i2] = _heapRuntime.Heap.GetField(num2, fieldInfo);
		}
	}

	private void H_STFLD()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		FieldInfo fieldInfo = _heapRuntime.Fields[num];
		long num2 = State.R[i];
		if (num2 == 0L && fieldInfo.DeclaringType != null && fieldInfo.DeclaringType.IsValueType)
		{
			object obj = Activator.CreateInstance(fieldInfo.DeclaringType);
			object value = KitsuneObjectHeap.UnboxFromLong(State.R[i2], fieldInfo.FieldType, _heapRuntime.Heap);
			fieldInfo.SetValue(obj, value);
			State.R[i] = _heapRuntime.Heap.BoxToLong(obj, fieldInfo.DeclaringType);
		}
		else
		{
			_heapRuntime.Heap.SetField(num2, fieldInfo, State.R[i2]);
		}
	}

	private void H_LDSFLD()
	{
		int num = ReadInt32();
		int i = ReadReg();
		FieldInfo field = _heapRuntime.Fields[num];
		State.R[i] = _heapRuntime.Heap.GetStaticField(field);
	}

	private void H_STSFLD()
	{
		int num = ReadInt32();
		int i = ReadReg();
		FieldInfo field = _heapRuntime.Fields[num];
		_heapRuntime.Heap.SetStaticField(field, State.R[i]);
	}

	private void H_NEWARR()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		Type elementType = _heapRuntime.Types[num];
		State.R[i2] = _heapRuntime.Heap.AllocTypedArray(elementType, (int)State.R[i]);
	}

	private void H_LDLEN()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i2] = _heapRuntime.Heap.GetArrayLength(State.R[i]);
	}

	private void H_LDELEM()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int num = ReadInt32();
		int i3 = ReadReg();
		Type elementType = _heapRuntime.Types[num];
		State.R[i3] = _heapRuntime.Heap.GetArrayElement(State.R[i], (int)State.R[i2], elementType);
	}

	private void H_STELEM()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		int num = ReadInt32();
		int i3 = ReadReg();
		Type elementType = _heapRuntime.Types[num];
		_heapRuntime.Heap.SetArrayElement(State.R[i], (int)State.R[i2], State.R[i3], elementType);
	}

	private void H_ISINST()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		long num2 = State.R[i];
		object obj = _heapRuntime.Heap.Get(num2);
		Type type = _heapRuntime.Types[num];
		State.R[i2] = ((obj != null && type.IsInstanceOfType(obj)) ? num2 : 0);
	}

	private void H_CASTCLASS()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		long num2 = State.R[i];
		object obj = _heapRuntime.Heap.Get(num2);
		Type type = _heapRuntime.Types[num];
		if (obj != null && !type.IsInstanceOfType(obj))
		{
			throw new InvalidCastException($"KitsuneVM CASTCLASS: cannot cast {obj.GetType().FullName} to {type.FullName}");
		}
		State.R[i2] = num2;
	}

	private void H_BOX()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		Type targetType = _heapRuntime.Types[num];
		object obj = KitsuneObjectHeap.UnboxFromLong(State.R[i], targetType, _heapRuntime.Heap);
		State.R[i2] = _heapRuntime.Heap.Alloc(obj);
	}

	private void H_UNBOX_ANY()
	{
		int num = ReadInt32();
		int i = ReadReg();
		int i2 = ReadReg();
		Type type = _heapRuntime.Types[num];
		object value = _heapRuntime.Heap.Get(State.R[i]);
		State.R[i2] = _heapRuntime.Heap.BoxToLong(value, type);
	}

	private void H_THROW()
	{
		int i = ReadReg();
		long handle = State.R[i];
		object obj = _heapRuntime.Heap.Get(handle);
		Exception ex = (obj as Exception) ?? new Exception("KitsuneVM THROW: object is not an Exception: " + (obj?.GetType().FullName ?? "null"));
		int iP = State.IP;
		if (!DispatchException(ex, iP))
		{
			throw ex;
		}
	}

	private void H_RETHROW()
	{
		long currentExceptionHandle = State.CurrentExceptionHandle;
		if (currentExceptionHandle == 0L)
		{
			throw new Exception("KitsuneVM RETHROW: no active exception.");
		}
		Exception ex = (_heapRuntime.Heap.Get(currentExceptionHandle) as Exception) ?? new Exception("KitsuneVM RETHROW: object is not an Exception");
		int iP = State.IP;
		if (!DispatchException(ex, iP))
		{
			throw ex;
		}
	}

	private void H_LEAVE()
	{
		int num = ReadInt32();
		int iP = State.IP;
		List<int> list = FindFinallyHandlers(iP);
		if (list.Count == 0)
		{
			State.IP = num;
			return;
		}
		State.FinallyReturnStack.Push(num);
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			State.FinallyReturnStack.Push(list[num2]);
		}
		State.IP = State.FinallyReturnStack.Pop();
	}

	private void H_END_FINALLY()
	{
		if (State.FinallyReturnStack.Count == 0)
		{
			State.Running = false;
		}
		else
		{
			State.IP = State.FinallyReturnStack.Pop();
		}
	}

	private void H_LOAD_EXCEPTION()
	{
		int i = ReadReg();
		State.R[i] = State.CurrentExceptionHandle;
	}

	private void H_LDTOKEN()
	{
		int num = ReadInt32();
		int i = ReadReg();
		Type obj = _heapRuntime.Types[num];
		State.R[i] = _heapRuntime.Heap.Alloc(obj);
	}

	private void H_CONV_U1()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i] = State.R[i2] & 0xFF;
	}

	private void H_CONV_I1()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		long num = State.R[i2] & 0xFF;
		State.R[i] = (sbyte)(byte)num;
	}

	private void H_CONV_U2()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		State.R[i] = State.R[i2] & 0xFFFF;
	}

	private void H_CONV_I2()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		long num = State.R[i2] & 0xFFFF;
		State.R[i] = (short)(ushort)num;
	}

	private void H_LDFTN()
	{
		int id = ReadInt32();
		int i = ReadReg();
		MethodBase methodBase = _externalCalls.GetMethodBase(id);
		if (methodBase != null)
		{
			RuntimeHelpers.PrepareMethod(methodBase.MethodHandle);
			State.R[i] = methodBase.MethodHandle.GetFunctionPointer().ToInt64();
		}
		else
		{
			State.R[i] = 0L;
		}
	}

	private void H_LDIND()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		long num = State.R[i];
		if (num == 0L)
		{
			State.R[i2] = 0L;
			return;
		}
		try
		{
			State.R[i2] = Marshal.ReadInt64(new IntPtr(num));
		}
		catch
		{
			State.R[i2] = 0L;
		}
	}

	private void H_STIND()
	{
		int i = ReadReg();
		int i2 = ReadReg();
		long num = State.R[i];
		if (num == 0L)
		{
			return;
		}
		try
		{
			Marshal.WriteInt64(new IntPtr(num), State.R[i2]);
		}
		catch
		{
		}
	}

	private void H_WATERMARK()
	{
		ReadInt32();
	}

	private void H_NOP_A()
	{
	}

	private void H_NOP_B()
	{
	}

	private byte ReadByte()
	{
		return _bytecode[State.IP++];
	}

	private int ReadReg()
	{
		int result = _bytecode[State.IP] | (_bytecode[State.IP + 1] << 8);
		State.IP += 2;
		return result;
	}

	private int ReadInt32()
	{
		int result = BitConverter.ToInt32(_bytecode, State.IP);
		State.IP += 4;
		return result;
	}

	private long ReadInt64()
	{
		long result = BitConverter.ToInt64(_bytecode, State.IP);
		State.IP += 8;
		return result;
	}

	private bool DispatchException(Exception ex, int throwIP)
	{
		if (_ehTable == null || _ehTable.Length == 0)
		{
			return false;
		}
		State.CurrentExceptionHandle = ((State.Heap != null) ? State.Heap.Alloc(ex) : 0);
		KitsuneEHEntry[] array = SortEHByInnermost(_ehTable);
		foreach (KitsuneEHEntry kitsuneEHEntry in array)
		{
			if (throwIP > kitsuneEHEntry.VmTryStart && throwIP <= kitsuneEHEntry.VmTryEnd && kitsuneEHEntry.HandlerType == KitsuneHandlerType.Catch && (kitsuneEHEntry.CatchTypeId < 0 || _heapRuntime.Types == null || kitsuneEHEntry.CatchTypeId >= _heapRuntime.Types.Length || _heapRuntime.Types[kitsuneEHEntry.CatchTypeId].IsInstanceOfType(ex)))
			{
				State.IP = kitsuneEHEntry.VmHandlerStart;
				return true;
			}
		}
		return false;
	}

	private List<int> FindFinallyHandlers(int ip)
	{
		List<KitsuneEHEntry> list = new List<KitsuneEHEntry>();
		KitsuneEHEntry[] ehTable = _ehTable;
		foreach (KitsuneEHEntry kitsuneEHEntry in ehTable)
		{
			if (kitsuneEHEntry.HandlerType == KitsuneHandlerType.Finally && ip > kitsuneEHEntry.VmTryStart && ip <= kitsuneEHEntry.VmTryEnd)
			{
				list.Add(kitsuneEHEntry);
			}
		}
		list.Sort((KitsuneEHEntry a, KitsuneEHEntry b) => b.VmTryStart.CompareTo(a.VmTryStart));
		List<int> list2 = new List<int>();
		foreach (KitsuneEHEntry item in list)
		{
			list2.Add(item.VmHandlerStart);
		}
		return list2;
	}

	private KitsuneEHEntry[] SortEHByInnermost(KitsuneEHEntry[] table)
	{
		KitsuneEHEntry[] array = new KitsuneEHEntry[table.Length];
		Array.Copy(table, array, table.Length);
		Array.Sort(array, (KitsuneEHEntry a, KitsuneEHEntry b) => b.VmTryStart.CompareTo(a.VmTryStart));
		return array;
	}

	private static uint[] BuildCrc32Table()
	{
		uint[] array = new uint[256];
		for (uint num = 0u; num < 256; num++)
		{
			uint num2 = num;
			for (int i = 0; i < 8; i++)
			{
				num2 = (((num2 & 1) != 0) ? (0xEDB88320u ^ (num2 >> 1)) : (num2 >> 1));
			}
			array[num] = num2;
		}
		return array;
	}

	private static uint ComputeCrc32Sig(byte[] data, int offset, int length)
	{
		uint num = uint.MaxValue;
		for (int i = offset; i < offset + length; i++)
		{
			num = (num >> 8) ^ _crc32Table[(num ^ data[i]) & 0xFF];
		}
		return num ^ 0xFFFFFFFFu;
	}
}
