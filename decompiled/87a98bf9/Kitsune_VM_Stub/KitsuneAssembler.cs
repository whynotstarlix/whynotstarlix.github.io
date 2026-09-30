using System;
using System.Collections.Generic;

namespace Kitsune_VM_Stub;

public class KitsuneAssembler
{
	private List<byte> _code = new List<byte>();

	private Dictionary<string, int> _labels = new Dictionary<string, int>();

	private List<KeyValuePair<int, string>> _patches = new List<KeyValuePair<int, string>>();

	public int CurrentPosition => _code.Count;

	public KitsuneAssembler Label(string name)
	{
		if (_labels.ContainsKey(name))
		{
			throw new Exception("KitsuneAssembler: label '" + name + "' is already declared");
		}
		_labels[name] = _code.Count;
		return this;
	}

	public KitsuneAssembler MovImm(int dst, long value)
	{
		EmitOp(KitsuneOpcode.MOV_REG_IMM);
		EmitByte(dst);
		EmitLong(value);
		return this;
	}

	public KitsuneAssembler MovReg(int dst, int src)
	{
		EmitOp(KitsuneOpcode.MOV_REG_REG);
		EmitByte(dst);
		EmitByte(src);
		return this;
	}

	public KitsuneAssembler Add(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.ADD);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Sub(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.SUB);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Mul(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.MUL);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Div(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.DIV);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Mod(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.MOD);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Neg(int dst, int a)
	{
		EmitOp(KitsuneOpcode.NEG);
		EmitByte(dst);
		EmitByte(a);
		return this;
	}

	public KitsuneAssembler And(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.AND);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Or(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.OR);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Xor(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.XOR);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Not(int dst, int a)
	{
		EmitOp(KitsuneOpcode.NOT);
		EmitByte(dst);
		EmitByte(a);
		return this;
	}

	public KitsuneAssembler Shl(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.SHL);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Shr(int dst, int a, int b)
	{
		EmitOp(KitsuneOpcode.SHR);
		EmitByte(dst);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler Cmp(int a, int b)
	{
		EmitOp(KitsuneOpcode.CMP);
		EmitByte(a);
		EmitByte(b);
		return this;
	}

	public KitsuneAssembler SetEq(int dst)
	{
		EmitOp(KitsuneOpcode.SET_ZF);
		EmitByte(dst);
		return this;
	}

	public KitsuneAssembler SetLt(int dst)
	{
		EmitOp(KitsuneOpcode.SET_LT);
		EmitByte(dst);
		return this;
	}

	public KitsuneAssembler SetGt(int dst)
	{
		EmitOp(KitsuneOpcode.SET_GT);
		EmitByte(dst);
		return this;
	}

	public KitsuneAssembler Jmp(int absoluteAddress)
	{
		EmitOp(KitsuneOpcode.JMP);
		EmitInt(absoluteAddress);
		return this;
	}

	public KitsuneAssembler Jmp(string label)
	{
		EmitOp(KitsuneOpcode.JMP);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler Jz(int reg, string label)
	{
		EmitOp(KitsuneOpcode.JZ);
		EmitByte(reg);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler Jnz(int reg, string label)
	{
		EmitOp(KitsuneOpcode.JNZ);
		EmitByte(reg);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JzFlag(string label)
	{
		EmitOp(KitsuneOpcode.JZ_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JnzFlag(string label)
	{
		EmitOp(KitsuneOpcode.JNZ_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JltFlag(string label)
	{
		EmitOp(KitsuneOpcode.JLT_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JgtFlag(string label)
	{
		EmitOp(KitsuneOpcode.JGT_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JleFlag(string label)
	{
		EmitOp(KitsuneOpcode.JLE_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler JgeFlag(string label)
	{
		EmitOp(KitsuneOpcode.JGE_FLAG);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler Call(string label)
	{
		EmitOp(KitsuneOpcode.CALL);
		EmitLabelRef(label);
		return this;
	}

	public KitsuneAssembler RetVoid()
	{
		EmitOp(KitsuneOpcode.RET_VOID);
		return this;
	}

	public KitsuneAssembler RetVal(int reg)
	{
		EmitOp(KitsuneOpcode.RET_VAL);
		EmitByte(reg);
		return this;
	}

	public KitsuneAssembler ExternalCall(int id, int[] argRegs, int retReg)
	{
		EmitOp(KitsuneOpcode.EXTERNAL_CALL);
		EmitInt(id);
		EmitByte(argRegs.Length);
		foreach (int value in argRegs)
		{
			EmitByte(value);
		}
		EmitByte((retReg < 0) ? 255 : retReg);
		return this;
	}

	public KitsuneAssembler Halt()
	{
		EmitOp(KitsuneOpcode.HALT);
		return this;
	}

	public byte[] Build()
	{
		byte[] array = _code.ToArray();
		foreach (KeyValuePair<int, string> patch in _patches)
		{
			int key = patch.Key;
			string value = patch.Value;
			if (!_labels.ContainsKey(value))
			{
				throw new Exception("KitsuneAssembler: label '" + value + "' is not declared. Did you forget to call asm.Label(\"" + value + "\")?");
			}
			BitConverter.GetBytes(_labels[value]).CopyTo(array, key);
		}
		return array;
	}

	private void EmitOp(KitsuneOpcode op)
	{
		_code.Add((byte)op);
	}

	private void EmitByte(int value)
	{
		_code.Add((byte)value);
	}

	private void EmitInt(int value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		_code.AddRange(bytes);
	}

	private void EmitLong(long value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		_code.AddRange(bytes);
	}

	private void EmitLabelRef(string labelName)
	{
		if (_labels.ContainsKey(labelName))
		{
			EmitInt(_labels[labelName]);
			return;
		}
		_patches.Add(new KeyValuePair<int, string>(_code.Count, labelName));
		EmitInt(0);
	}
}
