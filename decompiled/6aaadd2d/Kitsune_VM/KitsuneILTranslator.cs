using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace Kitsune_VM;

public class KitsuneILTranslator
{
	private List<byte> _code = new List<byte>();

	private Stack<int> _evalStack = new Stack<int>();

	private int _nextReg;

	private Dictionary<int, int> _argRegs = new Dictionary<int, int>();

	private Dictionary<int, int> _localRegs = new Dictionary<int, int>();

	private Dictionary<uint, int> _ilOffsetToVmOffset = new Dictionary<uint, int>();

	private List<KeyValuePair<int, uint>> _jumpPatches = new List<KeyValuePair<int, uint>>();

	private KitsuneExternalCallRegistry _extCallRegistry;

	private KitsuneStringRegistry _stringRegistry;

	private KitsuneFieldRegistry _fieldRegistry;

	private KitsuneTypeRegistry _typeRegistry;

	private HashSet<uint> _catchHandlerStarts = new HashSet<uint>();

	private HashSet<uint> _jumpTargets = new HashSet<uint>();

	private Dictionary<uint, Stack<int>> _stackSnapshots = new Dictionary<uint, Stack<int>>();

	private bool _unreachable;

	private byte[] _opcodePerm;

	public KitsuneILTranslator(KitsuneExternalCallRegistry extCallRegistry, KitsuneStringRegistry stringRegistry, KitsuneFieldRegistry fieldRegistry, KitsuneTypeRegistry typeRegistry, byte[] opcodePerm = null)
	{
		_extCallRegistry = extCallRegistry;
		_stringRegistry = stringRegistry;
		_fieldRegistry = fieldRegistry;
		_typeRegistry = typeRegistry;
		_opcodePerm = opcodePerm;
	}

	public KitsuneTranslationResult Translate(MethodDef method)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Invalid comparison between Unknown and I4
		_code.Clear();
		_evalStack.Clear();
		_nextReg = 0;
		_argRegs.Clear();
		_localRegs.Clear();
		_ilOffsetToVmOffset.Clear();
		_jumpPatches.Clear();
		_catchHandlerStarts.Clear();
		_jumpTargets.Clear();
		_stackSnapshots.Clear();
		_unreachable = false;
		CollectJumpTargets(method);
		Enumerator enumerator = method.Parameters.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Parameter current = enumerator.Current;
				_argRegs[current.Index] = _nextReg++;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (method.Body.HasVariables)
		{
			Enumerator<Local> enumerator2 = method.Body.Variables.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Local current2 = enumerator2.Current;
					_localRegs[current2.Index] = _nextReg++;
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (method.Body.HasExceptionHandlers)
		{
			foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
			{
				if (((int)exceptionHandler.HandlerType == 0 || (int)exceptionHandler.HandlerType == 1) && exceptionHandler.HandlerStart != null)
				{
					_catchHandlerStarts.Add(exceptionHandler.HandlerStart.Offset);
				}
			}
		}
		foreach (Instruction instruction in method.Body.Instructions)
		{
			_ilOffsetToVmOffset[instruction.Offset] = _code.Count;
			if (_jumpTargets.Contains(instruction.Offset))
			{
				_unreachable = false;
				if (_stackSnapshots.TryGetValue(instruction.Offset, out var value))
				{
					_evalStack = CloneStack(value);
				}
				else
				{
					_stackSnapshots[instruction.Offset] = CloneStack(_evalStack);
				}
			}
			if (!_unreachable)
			{
				if (_catchHandlerStarts.Contains(instruction.Offset))
				{
					int reg = AllocReg();
					EmitOpcode(KitsuneOpcodeRef.LOAD_EXCEPTION);
					EmitReg(reg);
					PushReg(reg);
				}
				TranslateInstruction(instruction, method);
			}
		}
		PatchJumps();
		EmitOpcode(KitsuneOpcodeRef.HALT);
		byte[] bytecode = _code.ToArray();
		List<KitsuneEHEntryRef> ehEntries = BuildEHTable(method);
		return new KitsuneTranslationResult(bytecode, ehEntries);
	}

	private void TranslateInstruction(Instruction instr, MethodDef method)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Expected I4, but got Unknown
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Expected Obj, but got Unknown
		//IL_1264: Unknown result type (might be due to invalid IL or missing references)
		//IL_126b: Expected Obj, but got Unknown
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Expected Obj, but got Unknown
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Expected Obj, but got Unknown
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Expected Obj, but got Unknown
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Expected Obj, but got Unknown
		//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Expected Obj, but got Unknown
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Expected Obj, but got Unknown
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Expected Obj, but got Unknown
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Expected Obj, but got Unknown
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Expected Obj, but got Unknown
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Expected Obj, but got Unknown
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Expected Obj, but got Unknown
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Expected Obj, but got Unknown
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Expected Obj, but got Unknown
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Expected Obj, but got Unknown
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca2: Expected Obj, but got Unknown
		//IL_112c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Expected Obj, but got Unknown
		//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4: Expected Obj, but got Unknown
		//IL_11ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1: Expected Obj, but got Unknown
		//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6b: Expected Obj, but got Unknown
		//IL_1409: Unknown result type (might be due to invalid IL or missing references)
		//IL_1410: Expected Obj, but got Unknown
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddb: Expected Obj, but got Unknown
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e43: Expected Obj, but got Unknown
		//IL_1479: Unknown result type (might be due to invalid IL or missing references)
		//IL_1480: Expected Obj, but got Unknown
		//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea3: Expected Obj, but got Unknown
		//IL_118b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1192: Expected Obj, but got Unknown
		//IL_0ef4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efb: Expected Obj, but got Unknown
		//IL_105e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_163e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1645: Expected Obj, but got Unknown
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Expected I4, but got Unknown
		//IL_12aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b1: Expected Obj, but got Unknown
		Code code = instr.OpCode.Code;
		Parameter val19;
		Local val20;
		Local val21;
		int src2;
		Parameter val22;
		int src3;
		Local val24;
		switch ((int)code)
		{
		default:
			switch (code - 65025)
			{
			case 8:
				break;
			case 10:
				goto IL_0552;
			case 11:
				goto IL_05ce;
			case 13:
				goto IL_0668;
			case 12:
				goto IL_0698;
			case 0:
			{
				int reg43 = PopReg();
				int reg44 = PopReg();
				int reg45 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.CMP);
				EmitReg(reg44);
				EmitReg(reg43);
				EmitOpcode(KitsuneOpcodeRef.SET_ZF);
				EmitReg(reg45);
				PushReg(reg45);
				return;
			}
			case 1:
			case 2:
			{
				int reg46 = PopReg();
				int reg47 = PopReg();
				int reg48 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.CMP);
				EmitReg(reg47);
				EmitReg(reg46);
				EmitOpcode(KitsuneOpcodeRef.SET_GT);
				EmitReg(reg48);
				PushReg(reg48);
				return;
			}
			case 3:
			case 4:
			{
				int reg49 = PopReg();
				int reg50 = PopReg();
				int reg51 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.CMP);
				EmitReg(reg50);
				EmitReg(reg49);
				EmitOpcode(KitsuneOpcodeRef.SET_LT);
				EmitReg(reg51);
				PushReg(reg51);
				return;
			}
			case 9:
				goto IL_125e;
			case 20:
			{
				int reg52 = PopReg();
				EmitOpcode(KitsuneOpcodeRef.MOV_REG_IMM);
				EmitReg(reg52);
				EmitLong(0L);
				return;
			}
			case 5:
			{
				IMethod val15 = (IMethod)instr.Operand;
				string[] array4 = new string[0];
				MethodSpec val16 = (MethodSpec)(object)((val15 is MethodSpec) ? val15 : null);
				if (val16 != null)
				{
					if (val16.GenericInstMethodSig != null)
					{
						array4 = GetGenericInstArgNames(val16.GenericInstMethodSig.GenericArguments);
					}
					val15 = (IMethod)(object)val16.Method;
				}
				string typeName3 = NormalizeTypeName(((IFullName)((IMemberRef)val15).DeclaringType).FullName);
				bool flag = ((MethodBaseSig)val15.MethodSig).RetType.FullName != "System.Void";
				MethodDef val17 = (MethodDef)(object)((val15 is MethodDef) ? val15 : null);
				if (val17 == null)
				{
					MemberRef val18 = (MemberRef)(object)((val15 is MemberRef) ? val15 : null);
					if (val18 != null)
					{
						val17 = val18.ResolveMethod();
					}
				}
				int orRegister7;
				if (val17 == null)
				{
					KitsuneExternalCallRegistry extCallRegistry = _extCallRegistry;
					string methodName = UTF8String.op_Implicit(((IFullName)val15).Name);
					bool hasReturnValue = flag;
					IMethod method2 = val15;
					object extraMethodGenArgs;
					if (val16 == null)
					{
						extraMethodGenArgs = null;
					}
					else
					{
						GenericInstMethodSig genericInstMethodSig = val16.GenericInstMethodSig;
						extraMethodGenArgs = ((genericInstMethodSig != null) ? genericInstMethodSig.GenericArguments : null);
					}
					orRegister7 = extCallRegistry.GetOrRegister(typeName3, methodName, hasReturnValue, GetParamTypeNames(method2, (IList<TypeSig>)extraMethodGenArgs), (array4.Length != 0) ? array4 : null);
				}
				else
				{
					KitsuneExternalCallRegistry extCallRegistry2 = _extCallRegistry;
					string methodName2 = UTF8String.op_Implicit(((IFullName)val15).Name);
					bool hasReturnValue2 = flag;
					IMethod method3 = val15;
					object extraMethodGenArgs2;
					if (val16 == null)
					{
						extraMethodGenArgs2 = null;
					}
					else
					{
						GenericInstMethodSig genericInstMethodSig2 = val16.GenericInstMethodSig;
						extraMethodGenArgs2 = ((genericInstMethodSig2 != null) ? genericInstMethodSig2.GenericArguments : null);
					}
					orRegister7 = extCallRegistry2.GetOrRegister(typeName3, methodName2, hasReturnValue2, GetParamTypeNames(method3, (IList<TypeSig>)extraMethodGenArgs2), (array4.Length != 0) ? array4 : null, val17);
				}
				int v = orRegister7;
				int reg42 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.LDFTN);
				EmitInt(v);
				EmitReg(reg42);
				PushReg(reg42);
				return;
			}
			case 25:
				EmitOpcode(KitsuneOpcodeRef.RETHROW);
				_unreachable = true;
				return;
			case 16:
				throw new NotSupportedException("KitsuneILTranslator: endfilter not supported. Method: " + UTF8String.op_Implicit(method.Name));
			default:
				goto end_IL_000d;
			case 21:
				return;
			}
			goto case 14;
		case 32:
			PushImmediate(Convert.ToInt64(instr.Operand));
			return;
		case 31:
			PushImmediate((sbyte)instr.Operand);
			return;
		case 21:
			PushImmediate(-1L);
			return;
		case 22:
			PushImmediate(0L);
			return;
		case 23:
			PushImmediate(1L);
			return;
		case 24:
			PushImmediate(2L);
			return;
		case 25:
			PushImmediate(3L);
			return;
		case 26:
			PushImmediate(4L);
			return;
		case 27:
			PushImmediate(5L);
			return;
		case 28:
			PushImmediate(6L);
			return;
		case 29:
			PushImmediate(7L);
			return;
		case 30:
			PushImmediate(8L);
			return;
		case 33:
			PushImmediate((long)instr.Operand);
			return;
		case 34:
		{
			float value3 = (float)instr.Operand;
			PushImmediate(BitConverter.ToInt32(BitConverter.GetBytes(value3), 0));
			return;
		}
		case 35:
		{
			double value2 = (double)instr.Operand;
			PushImmediate(BitConverter.DoubleToInt64Bits(value2));
			return;
		}
		case 20:
			PushImmediate(0L);
			return;
		case 2:
			PushReg(_argRegs[0]);
			return;
		case 3:
			PushReg(_argRegs[1]);
			return;
		case 4:
			PushReg(_argRegs[2]);
			return;
		case 5:
			PushReg(_argRegs[3]);
			return;
		case 14:
		{
			Parameter val23 = (Parameter)instr.Operand;
			PushReg(_argRegs[val23.Index]);
			return;
		}
		case 16:
			goto IL_0552;
		case 6:
			PushReg(_localRegs[0]);
			return;
		case 7:
			PushReg(_localRegs[1]);
			return;
		case 8:
			PushReg(_localRegs[2]);
			return;
		case 9:
			PushReg(_localRegs[3]);
			return;
		case 17:
			goto IL_05ce;
		case 10:
		{
			int src8 = PopReg();
			EmitMoveReg(_localRegs[0], src8);
			return;
		}
		case 11:
		{
			int src7 = PopReg();
			EmitMoveReg(_localRegs[1], src7);
			return;
		}
		case 12:
		{
			int src6 = PopReg();
			EmitMoveReg(_localRegs[2], src6);
			return;
		}
		case 13:
		{
			int src5 = PopReg();
			EmitMoveReg(_localRegs[3], src5);
			return;
		}
		case 19:
			goto IL_0668;
		case 18:
			goto IL_0698;
		case 88:
		case 214:
		case 215:
			EmitBinaryOp(KitsuneOpcodeRef.ADD);
			return;
		case 89:
		case 218:
		case 219:
			EmitBinaryOp(KitsuneOpcodeRef.SUB);
			return;
		case 90:
		case 216:
		case 217:
			EmitBinaryOp(KitsuneOpcodeRef.MUL);
			return;
		case 91:
		case 92:
			EmitBinaryOp(KitsuneOpcodeRef.DIV);
			return;
		case 93:
		case 94:
			EmitBinaryOp(KitsuneOpcodeRef.MOD);
			return;
		case 101:
			EmitUnaryOp(KitsuneOpcodeRef.NEG);
			return;
		case 95:
			EmitBinaryOp(KitsuneOpcodeRef.AND);
			return;
		case 96:
			EmitBinaryOp(KitsuneOpcodeRef.OR);
			return;
		case 97:
			EmitBinaryOp(KitsuneOpcodeRef.XOR);
			return;
		case 102:
			EmitUnaryOp(KitsuneOpcodeRef.NOT);
			return;
		case 98:
			EmitBinaryOp(KitsuneOpcodeRef.SHL);
			return;
		case 99:
		case 100:
			EmitBinaryOp(KitsuneOpcodeRef.SHR);
			return;
		case 43:
		case 56:
		{
			Instruction val33 = (Instruction)instr.Operand;
			MergeStackSnapshot(val33.Offset, _evalStack);
			EmitOpcode(KitsuneOpcodeRef.JMP);
			EmitJumpPatch(val33.Offset);
			_unreachable = true;
			return;
		}
		case 44:
		case 57:
		{
			Instruction val32 = (Instruction)instr.Operand;
			MergeStackSnapshot(val32.Offset, _evalStack);
			int reg76 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.JZ);
			EmitReg(reg76);
			EmitJumpPatch(val32.Offset);
			return;
		}
		case 45:
		case 58:
		{
			Instruction val31 = (Instruction)instr.Operand;
			MergeStackSnapshot(val31.Offset, _evalStack);
			int reg75 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.JNZ);
			EmitReg(reg75);
			EmitJumpPatch(val31.Offset);
			return;
		}
		case 46:
		case 59:
		{
			Instruction val30 = (Instruction)instr.Operand;
			MergeStackSnapshot(val30.Offset, _evalStack);
			int reg73 = PopReg();
			int reg74 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg74);
			EmitReg(reg73);
			EmitOpcode(KitsuneOpcodeRef.JZ_FLAG);
			EmitJumpPatch(val30.Offset);
			return;
		}
		case 51:
		case 64:
		{
			Instruction val29 = (Instruction)instr.Operand;
			MergeStackSnapshot(val29.Offset, _evalStack);
			int reg71 = PopReg();
			int reg72 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg72);
			EmitReg(reg71);
			EmitOpcode(KitsuneOpcodeRef.JNZ_FLAG);
			EmitJumpPatch(val29.Offset);
			return;
		}
		case 48:
		case 53:
		case 61:
		case 66:
		{
			Instruction val28 = (Instruction)instr.Operand;
			MergeStackSnapshot(val28.Offset, _evalStack);
			int reg69 = PopReg();
			int reg70 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg70);
			EmitReg(reg69);
			EmitOpcode(KitsuneOpcodeRef.JGT_FLAG);
			EmitJumpPatch(val28.Offset);
			return;
		}
		case 50:
		case 55:
		case 63:
		case 68:
		{
			Instruction val27 = (Instruction)instr.Operand;
			MergeStackSnapshot(val27.Offset, _evalStack);
			int reg67 = PopReg();
			int reg68 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg68);
			EmitReg(reg67);
			EmitOpcode(KitsuneOpcodeRef.JLT_FLAG);
			EmitJumpPatch(val27.Offset);
			return;
		}
		case 47:
		case 52:
		case 60:
		case 65:
		{
			Instruction val26 = (Instruction)instr.Operand;
			MergeStackSnapshot(val26.Offset, _evalStack);
			int reg65 = PopReg();
			int reg66 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg66);
			EmitReg(reg65);
			EmitOpcode(KitsuneOpcodeRef.JGE_FLAG);
			EmitJumpPatch(val26.Offset);
			return;
		}
		case 49:
		case 54:
		case 62:
		case 67:
		{
			Instruction val25 = (Instruction)instr.Operand;
			MergeStackSnapshot(val25.Offset, _evalStack);
			int reg63 = PopReg();
			int reg64 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.CMP);
			EmitReg(reg64);
			EmitReg(reg63);
			EmitOpcode(KitsuneOpcodeRef.JLE_FLAG);
			EmitJumpPatch(val25.Offset);
			return;
		}
		case 37:
		{
			int src4 = _evalStack.Peek();
			int num2 = AllocReg();
			EmitMoveReg(num2, src4);
			PushReg(num2);
			return;
		}
		case 38:
			PopReg();
			return;
		case 42:
			if (_evalStack.Count > 0)
			{
				int reg62 = PopReg();
				EmitOpcode(KitsuneOpcodeRef.RET_VAL);
				EmitReg(reg62);
			}
			else
			{
				EmitOpcode(KitsuneOpcodeRef.RET_VOID);
			}
			_unreachable = true;
			return;
		case 40:
		case 111:
		{
			IMethod calledMethod = (IMethod)instr.Operand;
			TranslateCall(calledMethod);
			return;
		}
		case 210:
		{
			int reg60 = PopReg();
			int reg61 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.CONV_U1);
			EmitReg(reg61);
			EmitReg(reg60);
			PushReg(reg61);
			return;
		}
		case 103:
		{
			int reg58 = PopReg();
			int reg59 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.CONV_I1);
			EmitReg(reg59);
			EmitReg(reg58);
			PushReg(reg59);
			return;
		}
		case 209:
		{
			int reg56 = PopReg();
			int reg57 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.CONV_U2);
			EmitReg(reg57);
			EmitReg(reg56);
			PushReg(reg57);
			return;
		}
		case 104:
		{
			int reg54 = PopReg();
			int reg55 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.CONV_I2);
			EmitReg(reg55);
			EmitReg(reg54);
			PushReg(reg55);
			return;
		}
		case 114:
		{
			string value = (string)instr.Operand;
			int orAdd = _stringRegistry.GetOrAdd(value);
			int reg53 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDSTR);
			EmitInt(orAdd);
			EmitReg(reg53);
			PushReg(reg53);
			return;
		}
		case 115:
		{
			IMethod val14 = (IMethod)instr.Operand;
			int count = ((MethodBaseSig)val14.MethodSig).Params.Count;
			string[] paramTypeNames = GetParamTypeNames(val14);
			int orRegisterConstructor = _typeRegistry.GetOrRegisterConstructor(NormalizeTypeName(((IFullName)((IMemberRef)val14).DeclaringType).FullName), paramTypeNames);
			int[] array2 = new int[count];
			for (int num = count - 1; num >= 0; num--)
			{
				array2[num] = PopReg();
			}
			int reg40 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.NEWOBJ);
			EmitInt(orRegisterConstructor);
			EmitByte(count);
			int[] array3 = array2;
			foreach (int reg41 in array3)
			{
				EmitReg(reg41);
			}
			EmitReg(reg40);
			PushReg(reg40);
			return;
		}
		case 123:
		{
			IField val13 = (IField)instr.Operand;
			int orRegister6 = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val13).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val13).Name));
			int reg38 = PopReg();
			int reg39 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDFLD);
			EmitInt(orRegister6);
			EmitReg(reg38);
			EmitReg(reg39);
			PushReg(reg39);
			return;
		}
		case 125:
		{
			IField val12 = (IField)instr.Operand;
			int orRegister5 = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val12).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val12).Name));
			int reg36 = PopReg();
			int reg37 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.STFLD);
			EmitInt(orRegister5);
			EmitReg(reg37);
			EmitReg(reg36);
			return;
		}
		case 126:
		{
			IField val11 = (IField)instr.Operand;
			int orRegister4 = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val11).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val11).Name));
			int reg35 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDSFLD);
			EmitInt(orRegister4);
			EmitReg(reg35);
			PushReg(reg35);
			return;
		}
		case 128:
		{
			IField val10 = (IField)instr.Operand;
			int orRegister3 = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val10).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val10).Name));
			int reg34 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.STSFLD);
			EmitInt(orRegister3);
			EmitReg(reg34);
			return;
		}
		case 141:
		{
			ITypeDefOrRef val9 = (ITypeDefOrRef)instr.Operand;
			int orRegisterType9 = _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)val9).FullName));
			int reg32 = PopReg();
			int reg33 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.NEWARR);
			EmitInt(orRegisterType9);
			EmitReg(reg32);
			EmitReg(reg33);
			PushReg(reg33);
			return;
		}
		case 142:
		{
			int reg30 = PopReg();
			int reg31 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDLEN);
			EmitReg(reg30);
			EmitReg(reg31);
			PushReg(reg31);
			return;
		}
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 149:
		case 150:
		case 151:
		case 152:
		case 153:
		case 154:
		case 163:
		{
			string ldelemTypeName = GetLdelemTypeName(instr.OpCode.Code, instr.Operand);
			int orRegisterType8 = _typeRegistry.GetOrRegisterType(ldelemTypeName);
			int reg27 = PopReg();
			int reg28 = PopReg();
			int reg29 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDELEM);
			EmitReg(reg28);
			EmitReg(reg27);
			EmitInt(orRegisterType8);
			EmitReg(reg29);
			PushReg(reg29);
			return;
		}
		case 155:
		case 156:
		case 157:
		case 158:
		case 159:
		case 160:
		case 161:
		case 162:
		case 164:
		{
			string stelemTypeName = GetStelemTypeName(instr.OpCode.Code, instr.Operand);
			int orRegisterType7 = _typeRegistry.GetOrRegisterType(stelemTypeName);
			int reg24 = PopReg();
			int reg25 = PopReg();
			int reg26 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.STELEM);
			EmitReg(reg26);
			EmitReg(reg25);
			EmitInt(orRegisterType7);
			EmitReg(reg24);
			return;
		}
		case 143:
		{
			string typeName2 = NormalizeTypeName(((IFullName)(ITypeDefOrRef)instr.Operand).FullName);
			int orRegisterType6 = _typeRegistry.GetOrRegisterType(typeName2);
			int reg21 = PopReg();
			int reg22 = PopReg();
			int reg23 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDELEM);
			EmitReg(reg22);
			EmitReg(reg21);
			EmitInt(orRegisterType6);
			EmitReg(reg23);
			PushReg(reg23);
			return;
		}
		case 117:
		{
			ITypeDefOrRef val8 = (ITypeDefOrRef)instr.Operand;
			int orRegisterType5 = _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)val8).FullName));
			int reg19 = PopReg();
			int reg20 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.ISINST);
			EmitInt(orRegisterType5);
			EmitReg(reg19);
			EmitReg(reg20);
			PushReg(reg20);
			return;
		}
		case 116:
		{
			ITypeDefOrRef val7 = (ITypeDefOrRef)instr.Operand;
			int orRegisterType4 = _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)val7).FullName));
			int reg17 = PopReg();
			int reg18 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.CASTCLASS);
			EmitInt(orRegisterType4);
			EmitReg(reg17);
			EmitReg(reg18);
			PushReg(reg18);
			return;
		}
		case 140:
		{
			ITypeDefOrRef val6 = (ITypeDefOrRef)instr.Operand;
			int orRegisterType3 = _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)val6).FullName));
			int reg15 = PopReg();
			int reg16 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.BOX);
			EmitInt(orRegisterType3);
			EmitReg(reg15);
			EmitReg(reg16);
			PushReg(reg16);
			return;
		}
		case 121:
		case 165:
		{
			ITypeDefOrRef val5 = (ITypeDefOrRef)instr.Operand;
			int orRegisterType2 = _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)val5).FullName));
			int reg13 = PopReg();
			int reg14 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.UNBOX_ANY);
			EmitInt(orRegisterType2);
			EmitReg(reg13);
			EmitReg(reg14);
			PushReg(reg14);
			return;
		}
		case 129:
		{
			int src = PopReg();
			int dst = PopReg();
			EmitMoveReg(dst, src);
			return;
		}
		case 15:
			goto IL_125e;
		case 124:
		{
			IField val4 = (IField)instr.Operand;
			int orRegister2 = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val4).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val4).Name));
			int reg11 = PopReg();
			int reg12 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDFLD);
			EmitInt(orRegister2);
			EmitReg(reg11);
			EmitReg(reg12);
			PushReg(reg12);
			return;
		}
		case 127:
		{
			IField val3 = (IField)instr.Operand;
			int orRegister = _fieldRegistry.GetOrRegister(NormalizeTypeName(((IFullName)((IMemberRef)val3).DeclaringType).FullName), UTF8String.op_Implicit(((IFullName)val3).Name));
			int reg10 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDSFLD);
			EmitInt(orRegister);
			EmitReg(reg10);
			PushReg(reg10);
			return;
		}
		case 208:
		{
			object operand = instr.Operand;
			ITypeDefOrRef val2 = (ITypeDefOrRef)((operand is ITypeDefOrRef) ? operand : null);
			if (val2 != null)
			{
				string typeName = NormalizeTypeName(((IFullName)val2).FullName);
				int orRegisterType = _typeRegistry.GetOrRegisterType(typeName);
				int reg8 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.LDTOKEN);
				EmitInt(orRegisterType);
				EmitReg(reg8);
				PushReg(reg8);
			}
			else
			{
				int reg9 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.MOV_REG_IMM);
				EmitReg(reg9);
				EmitLong(0L);
				PushReg(reg9);
			}
			return;
		}
		case 122:
		{
			int reg7 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.THROW);
			EmitReg(reg7);
			_unreachable = true;
			return;
		}
		case 69:
		{
			Instruction[] array = (Instruction[])instr.Operand;
			int reg5 = PopReg();
			for (int i = 0; i < array.Length; i++)
			{
				MergeStackSnapshot(array[i].Offset, _evalStack);
			}
			for (int j = 0; j < array.Length; j++)
			{
				int reg6 = AllocReg();
				EmitOpcode(KitsuneOpcodeRef.MOV_REG_IMM);
				EmitReg(reg6);
				EmitLong(j);
				EmitOpcode(KitsuneOpcodeRef.CMP);
				EmitReg(reg5);
				EmitReg(reg6);
				EmitOpcode(KitsuneOpcodeRef.JZ_FLAG);
				EmitJumpPatch(array[j].Offset);
			}
			return;
		}
		case 221:
		case 222:
		{
			_evalStack.Clear();
			Instruction val = (Instruction)instr.Operand;
			MergeStackSnapshot(val.Offset, _evalStack);
			EmitOpcode(KitsuneOpcodeRef.LEAVE);
			EmitJumpPatch(val.Offset);
			_unreachable = true;
			return;
		}
		case 220:
			EmitOpcode(KitsuneOpcodeRef.END_FINALLY);
			return;
		case 70:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 78:
		case 79:
		case 80:
		{
			int reg3 = PopReg();
			int reg4 = AllocReg();
			EmitOpcode(KitsuneOpcodeRef.LDIND);
			EmitReg(reg3);
			EmitReg(reg4);
			PushReg(reg4);
			return;
		}
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
		case 87:
		case 223:
		{
			int reg = PopReg();
			int reg2 = PopReg();
			EmitOpcode(KitsuneOpcodeRef.STIND);
			EmitReg(reg2);
			EmitReg(reg);
			return;
		}
		case 1:
		case 36:
		case 39:
		case 41:
		case 112:
		case 118:
		case 119:
		case 120:
		case 130:
		case 131:
		case 134:
		case 135:
		case 136:
		case 137:
		case 138:
		case 139:
		case 166:
		case 167:
		case 168:
		case 169:
		case 170:
		case 171:
		case 172:
		case 173:
		case 174:
		case 175:
		case 176:
		case 177:
		case 178:
		case 179:
		case 180:
		case 181:
		case 182:
		case 184:
		case 186:
		case 187:
		case 188:
		case 189:
		case 190:
		case 191:
		case 192:
		case 193:
		case 194:
		case 195:
		case 196:
		case 197:
		case 198:
		case 199:
		case 200:
		case 201:
		case 202:
		case 203:
		case 204:
		case 205:
		case 206:
		case 207:
			break;
		case 0:
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 113:
		case 132:
		case 133:
		case 183:
		case 185:
		case 211:
		case 212:
		case 213:
		case 224:
			return;
			IL_125e:
			val19 = (Parameter)instr.Operand;
			PushReg(_argRegs[val19.Index]);
			return;
			IL_0698:
			val20 = (Local)instr.Operand;
			PushReg(_localRegs[val20.Index]);
			return;
			IL_0668:
			val21 = (Local)instr.Operand;
			src2 = PopReg();
			EmitMoveReg(_localRegs[val21.Index], src2);
			return;
			IL_0552:
			val22 = (Parameter)instr.Operand;
			src3 = PopReg();
			EmitMoveReg(_argRegs[val22.Index], src3);
			return;
			IL_05ce:
			val24 = (Local)instr.Operand;
			PushReg(_localRegs[val24.Index]);
			return;
			end_IL_000d:
			break;
		}
		throw new NotSupportedException("KitsuneILTranslator: unsupported opcode: " + instr.OpCode.Name + " @ IL_" + instr.Offset.ToString("X4") + " in method " + UTF8String.op_Implicit(method.Name) + ". Add handling in TranslateInstruction().");
	}

	private void TranslateCall(IMethod calledMethod)
	{
		string[] array = new string[0];
		MethodSpec val = (MethodSpec)(object)((calledMethod is MethodSpec) ? calledMethod : null);
		if (val != null)
		{
			if (val.GenericInstMethodSig != null)
			{
				array = GetGenericInstArgNames(val.GenericInstMethodSig.GenericArguments);
			}
			calledMethod = (IMethod)(object)val.Method;
		}
		int num = ((MethodBaseSig)calledMethod.MethodSig).Params.Count;
		if (((CallingConventionSig)calledMethod.MethodSig).HasThis)
		{
			num++;
		}
		bool flag = ((MethodBaseSig)calledMethod.MethodSig).RetType.FullName != "System.Void";
		string typeName = NormalizeTypeName(((IFullName)((IMemberRef)calledMethod).DeclaringType).FullName);
		object obj;
		if (val == null)
		{
			obj = null;
		}
		else
		{
			GenericInstMethodSig genericInstMethodSig = val.GenericInstMethodSig;
			obj = ((genericInstMethodSig != null) ? genericInstMethodSig.GenericArguments : null);
		}
		IList<TypeSig> extraMethodGenArgs = (IList<TypeSig>)obj;
		MethodDef val2 = (MethodDef)(object)((calledMethod is MethodDef) ? calledMethod : null);
		if (val2 == null)
		{
			MemberRef val3 = (MemberRef)(object)((calledMethod is MemberRef) ? calledMethod : null);
			if (val3 != null)
			{
				val2 = val3.ResolveMethod();
			}
		}
		int v = ((val2 != null) ? _extCallRegistry.GetOrRegister(typeName, UTF8String.op_Implicit(((IFullName)calledMethod).Name), flag, GetParamTypeNames(calledMethod, extraMethodGenArgs), (array.Length != 0) ? array : null, val2) : _extCallRegistry.GetOrRegister(typeName, UTF8String.op_Implicit(((IFullName)calledMethod).Name), flag, GetParamTypeNames(calledMethod, extraMethodGenArgs), (array.Length != 0) ? array : null));
		int[] array2 = new int[num];
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			array2[num2] = PopReg();
		}
		EmitOpcode(KitsuneOpcodeRef.EXTERNAL_CALL);
		EmitInt(v);
		EmitByte(num);
		int[] array3 = array2;
		foreach (int reg in array3)
		{
			EmitReg(reg);
		}
		if (flag)
		{
			int reg2 = AllocReg();
			EmitReg(reg2);
			PushReg(reg2);
		}
		else
		{
			EmitReg(65535);
		}
	}

	private string[] GetGenericInstArgNames(IList<TypeSig> genericArguments)
	{
		string[] array = new string[genericArguments.Count];
		for (int i = 0; i < genericArguments.Count; i++)
		{
			array[i] = NormalizeTypeName(genericArguments[i].FullName);
		}
		return array;
	}

	private string[] GetParamTypeNames(IMethod method, IList<TypeSig> extraMethodGenArgs = null)
	{
		IList<TypeSig> typeArgs = null;
		MemberRef val = (MemberRef)(object)((method is MemberRef) ? method : null);
		if (val != null)
		{
			IMemberRefParent val2 = val.Class;
			TypeSpec val3 = (TypeSpec)(object)((val2 is TypeSpec) ? val2 : null);
			if (val3 != null)
			{
				TypeSig typeSig = val3.TypeSig;
				GenericInstSig val4 = (GenericInstSig)(object)((typeSig is GenericInstSig) ? typeSig : null);
				if (val4 != null)
				{
					typeArgs = val4.GenericArguments;
				}
			}
		}
		IList<TypeSig> list = extraMethodGenArgs;
		if (list == null)
		{
			MethodSpec val5 = (MethodSpec)(object)((method is MethodSpec) ? method : null);
			if (val5 != null && val5.GenericInstMethodSig != null)
			{
				list = val5.GenericInstMethodSig.GenericArguments;
			}
		}
		List<string> list2 = new List<string>();
		foreach (TypeSig item in ((MethodBaseSig)method.MethodSig).Params)
		{
			list2.Add(ResolveTypeSigName(item, typeArgs, list));
		}
		return list2.ToArray();
	}

	private string ResolveTypeSigName(TypeSig sig, IList<TypeSig> typeArgs, IList<TypeSig> methodArgs)
	{
		if (sig == null)
		{
			return "";
		}
		ByRefSig val = (ByRefSig)(object)((sig is ByRefSig) ? sig : null);
		if (val != null)
		{
			return ResolveTypeSigName(((TypeSig)val).Next, typeArgs, methodArgs) + "&";
		}
		PtrSig val2 = (PtrSig)(object)((sig is PtrSig) ? sig : null);
		if (val2 != null)
		{
			return ResolveTypeSigName(((TypeSig)val2).Next, typeArgs, methodArgs) + "*";
		}
		SZArraySig val3 = (SZArraySig)(object)((sig is SZArraySig) ? sig : null);
		if (val3 != null)
		{
			return ResolveTypeSigName(((TypeSig)val3).Next, typeArgs, methodArgs) + "[]";
		}
		ArraySig val4 = (ArraySig)(object)((sig is ArraySig) ? sig : null);
		if (val4 != null)
		{
			return ResolveTypeSigName(((TypeSig)val4).Next, typeArgs, methodArgs) + "[]";
		}
		GenericVar val5 = (GenericVar)(object)((sig is GenericVar) ? sig : null);
		if (val5 != null && typeArgs != null && (int)((GenericSig)val5).Number < typeArgs.Count)
		{
			return ResolveTypeSigName(typeArgs[(int)((GenericSig)val5).Number], null, methodArgs);
		}
		GenericMVar val6 = (GenericMVar)(object)((sig is GenericMVar) ? sig : null);
		if (val6 != null && methodArgs != null && (int)((GenericSig)val6).Number < methodArgs.Count)
		{
			return ResolveTypeSigName(methodArgs[(int)((GenericSig)val6).Number], typeArgs, null);
		}
		GenericInstSig val7 = (GenericInstSig)(object)((sig is GenericInstSig) ? sig : null);
		if (val7 != null)
		{
			string text = NormalizeTypeName(((TypeSig)val7.GenericType).FullName);
			string[] array = new string[val7.GenericArguments.Count];
			for (int i = 0; i < val7.GenericArguments.Count; i++)
			{
				array[i] = ResolveTypeSigName(val7.GenericArguments[i], typeArgs, methodArgs);
			}
			return text + "[" + string.Join(",", array) + "]";
		}
		return NormalizeTypeName(sig.FullName);
	}

	private static string NormalizeTypeName(string name)
	{
		return KitsuneExternalCallRegistry.Normalize(name);
	}

	private string GetLdelemTypeName(Code code, object operand)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected I4, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		return (code - 144) switch
		{
			7 => "System.IntPtr", 
			0 => "System.SByte", 
			2 => "System.Int16", 
			4 => "System.Int32", 
			6 => "System.Int64", 
			1 => "System.Byte", 
			3 => "System.UInt16", 
			5 => "System.UInt32", 
			8 => "System.Single", 
			9 => "System.Double", 
			10 => "System.Object", 
			19 => NormalizeTypeName(((IFullName)(ITypeDefOrRef)operand).FullName), 
			_ => "System.Object", 
		};
	}

	private string GetStelemTypeName(Code code, object operand)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected I4, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		return (code - 155) switch
		{
			0 => "System.IntPtr", 
			1 => "System.SByte", 
			2 => "System.Int16", 
			3 => "System.Int32", 
			4 => "System.Int64", 
			5 => "System.Single", 
			6 => "System.Double", 
			7 => "System.Object", 
			9 => NormalizeTypeName(((IFullName)(ITypeDefOrRef)operand).FullName), 
			_ => "System.Object", 
		};
	}

	private List<KitsuneEHEntryRef> BuildEHTable(MethodDef method)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected I4, but got Unknown
		List<KitsuneEHEntryRef> list = new List<KitsuneEHEntryRef>();
		if (!method.Body.HasExceptionHandlers)
		{
			return list;
		}
		foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
		{
			if ((int)exceptionHandler.HandlerType != 1)
			{
				KitsuneEHEntryRef kitsuneEHEntryRef = new KitsuneEHEntryRef();
				kitsuneEHEntryRef.VmTryStart = ILOffsetToVM(exceptionHandler.TryStart);
				kitsuneEHEntryRef.VmTryEnd = ILOffsetToVM(exceptionHandler.TryEnd);
				kitsuneEHEntryRef.VmHandlerStart = ILOffsetToVM(exceptionHandler.HandlerStart);
				kitsuneEHEntryRef.VmHandlerEnd = ILOffsetToVM(exceptionHandler.HandlerEnd);
				ExceptionHandlerType handlerType = exceptionHandler.HandlerType;
				switch ((int)handlerType)
				{
				case 0:
					kitsuneEHEntryRef.HandlerType = KitsuneHandlerTypeRef.Catch;
					kitsuneEHEntryRef.CatchTypeId = ((exceptionHandler.CatchType != null) ? _typeRegistry.GetOrRegisterType(NormalizeTypeName(((IFullName)exceptionHandler.CatchType).FullName)) : (-1));
					break;
				case 2:
					kitsuneEHEntryRef.HandlerType = KitsuneHandlerTypeRef.Finally;
					kitsuneEHEntryRef.CatchTypeId = -1;
					break;
				case 4:
					kitsuneEHEntryRef.HandlerType = KitsuneHandlerTypeRef.Fault;
					kitsuneEHEntryRef.CatchTypeId = -1;
					break;
				}
				list.Add(kitsuneEHEntryRef);
			}
		}
		return list;
	}

	private int ILOffsetToVM(Instruction ilInstr)
	{
		if (ilInstr == null)
		{
			return _code.Count;
		}
		if (_ilOffsetToVmOffset.TryGetValue(ilInstr.Offset, out var value))
		{
			return value;
		}
		return _code.Count;
	}

	private void PatchJumps()
	{
		byte[] array = _code.ToArray();
		foreach (KeyValuePair<int, uint> jumpPatch in _jumpPatches)
		{
			int key = jumpPatch.Key;
			uint value = jumpPatch.Value;
			if (!_ilOffsetToVmOffset.ContainsKey(value))
			{
				throw new Exception("KitsuneILTranslator: no VM address for IL_" + value.ToString("X4") + ". Make sure the target instruction was translated.");
			}
			BitConverter.GetBytes(_ilOffsetToVmOffset[value]).CopyTo(array, key);
		}
		_code.Clear();
		_code.AddRange(array);
	}

	private void CollectJumpTargets(MethodDef method)
	{
		foreach (Instruction instruction in method.Body.Instructions)
		{
			object operand = instruction.Operand;
			Instruction val = (Instruction)((operand is Instruction) ? operand : null);
			if (val != null)
			{
				_jumpTargets.Add(val.Offset);
			}
			else if (instruction.Operand is Instruction[] array)
			{
				Instruction[] array2 = array;
				foreach (Instruction val2 in array2)
				{
					_jumpTargets.Add(val2.Offset);
				}
			}
		}
		if (!method.Body.HasExceptionHandlers)
		{
			return;
		}
		foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
		{
			if (exceptionHandler.TryStart != null)
			{
				_jumpTargets.Add(exceptionHandler.TryStart.Offset);
			}
			if (exceptionHandler.HandlerStart != null)
			{
				_jumpTargets.Add(exceptionHandler.HandlerStart.Offset);
			}
		}
	}

	private void MergeStackSnapshot(uint ilOffset, Stack<int> currentStack)
	{
		if (!_stackSnapshots.ContainsKey(ilOffset))
		{
			_stackSnapshots[ilOffset] = CloneStack(currentStack);
		}
	}

	private static Stack<int> CloneStack(Stack<int> stack)
	{
		int[] array = stack.ToArray();
		Stack<int> stack2 = new Stack<int>();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			stack2.Push(array[num]);
		}
		return stack2;
	}

	private int AllocReg()
	{
		if (_nextReg >= 65536)
		{
			throw new Exception("KitsuneILTranslator: ran out of registers (max 65536). The method is too complex or requires register spilling.");
		}
		return _nextReg++;
	}

	private void PushReg(int reg)
	{
		_evalStack.Push(reg);
	}

	private int PopReg()
	{
		if (_evalStack.Count == 0)
		{
			throw new Exception("KitsuneILTranslator: attempted Pop on an empty stack. Most likely a bad translation of a preceding instruction.");
		}
		return _evalStack.Pop();
	}

	private void PushImmediate(long value)
	{
		int reg = AllocReg();
		EmitOpcode(KitsuneOpcodeRef.MOV_REG_IMM);
		EmitReg(reg);
		EmitLong(value);
		PushReg(reg);
	}

	private void EmitBinaryOp(KitsuneOpcodeRef op)
	{
		int reg = PopReg();
		int reg2 = PopReg();
		int reg3 = AllocReg();
		EmitOpcode(op);
		EmitReg(reg3);
		EmitReg(reg2);
		EmitReg(reg);
		PushReg(reg3);
	}

	private void EmitUnaryOp(KitsuneOpcodeRef op)
	{
		int reg = PopReg();
		int reg2 = AllocReg();
		EmitOpcode(op);
		EmitReg(reg2);
		EmitReg(reg);
		PushReg(reg2);
	}

	private void EmitMoveReg(int dst, int src)
	{
		EmitOpcode(KitsuneOpcodeRef.MOV_REG_REG);
		EmitReg(dst);
		EmitReg(src);
	}

	private void EmitJumpPatch(uint ilTargetOffset)
	{
		_jumpPatches.Add(new KeyValuePair<int, uint>(_code.Count, ilTargetOffset));
		EmitInt(0);
	}

	private void EmitByte(int b)
	{
		_code.Add((byte)b);
	}

	private void EmitReg(int reg)
	{
		_code.Add((byte)(reg & 0xFF));
		_code.Add((byte)(reg >> 8));
	}

	private void EmitInt(int v)
	{
		_code.AddRange(BitConverter.GetBytes(v));
	}

	private void EmitLong(long v)
	{
		_code.AddRange(BitConverter.GetBytes(v));
	}

	private void EmitOpcode(KitsuneOpcodeRef op)
	{
		byte b = (byte)op;
		_code.Add((_opcodePerm != null) ? _opcodePerm[b] : b);
	}
}
