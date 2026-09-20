using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using dnlib.IO;

namespace AnpakJit;

internal sealed class MethodRestorer
{
	private readonly ModuleDefMD _module;

	private readonly Action<string> _log;

	public MethodRestorer(string path, Action<string> log)
	{
		_log = log;
		_module = ModuleDefMD.Load(path, (ModuleCreationOptions)null);
	}

	public void Unpack()
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		TypeDef globalType = ((ModuleDef)_module).GlobalType;
		MethodDef val = null;
		foreach (Instruction instruction in globalType.FindOrCreateStaticConstructor().Body.Instructions)
		{
			if (instruction.OpCode == OpCodes.Call)
			{
				object operand = instruction.Operand;
				MethodDef val2 = (MethodDef)((operand is MethodDef) ? operand : null);
				if (val2 != null && val2.DeclaringType == globalType)
				{
					val = val2;
					break;
				}
			}
		}
		List<EmbeddedResource> list = new List<EmbeddedResource>();
		foreach (EmbeddedResource item in ((IEnumerable)((ModuleDef)_module).Resources).OfType<EmbeddedResource>())
		{
			if (((Resource)item).Name.Length > 20)
			{
				list.Add(item);
			}
		}
		bool flag = false;
		foreach (EmbeddedResource item2 in list)
		{
			if (flag)
			{
				break;
			}
			try
			{
				DataReader val3 = item2.CreateReader();
				byte[] array = Decompressor.Decompress(((DataReader)(ref val3)).ToArray(), 3);
				if (array == null || array.Length < 8)
				{
					continue;
				}
				int num = BitConverter.ToInt32(array, 0);
				if (num > 0 && num <= 50000 && BitConverter.ToUInt32(array, 4) >> 24 == 6)
				{
					_log("[+] Found method data: " + num + " methods");
					int num2 = RestoreMethods(array);
					if (num2 > 0)
					{
						_log("[+] Restored " + num2 + "/" + num + " methods");
						flag = true;
					}
				}
			}
			catch
			{
			}
		}
		RemoveNoInlining();
		if (val != null)
		{
			Cleanup(val);
		}
	}

	private int RestoreMethods(byte[] data)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(data)))
		{
			int num2 = binaryReader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				try
				{
					if (binaryReader.BaseStream.Position + 4 > binaryReader.BaseStream.Length)
					{
						break;
					}
					int num3 = binaryReader.ReadInt32();
					byte[] ilBytes;
					try
					{
						ilBytes = Convert.FromBase64String(binaryReader.ReadString());
					}
					catch
					{
						continue;
					}
					uint num4 = (uint)(num3 & 0xFFFFFF);
					MethodDef val = _module.ResolveMethod(num4);
					if (val != null)
					{
						byte[] array = BuildFullMethodBody(ilBytes, val);
						GenericParamContext val2 = GenericParamContext.Create(val);
						DataReader val3 = ByteArrayDataReaderFactory.CreateReader(array);
						val.Body = MethodBodyReader.CreateCilBody((IInstructionOperandResolver)(object)_module, val3, val, val2);
						val.Body.KeepOldMaxStack = true;
						num++;
					}
					continue;
				}
				catch
				{
					continue;
				}
			}
		}
		return num;
	}

	private byte[] BuildFullMethodBody(byte[] ilBytes, MethodDef method)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			DataReader pe = _module.Metadata.PEImage.CreateReader(method.RVA);
			byte b = ((DataReader)(ref pe)).ReadByte();
			int num = b & 3;
			if (num == 2 || (b & 7) == 6)
			{
				if (ilBytes.Length <= 63)
				{
					return TinyBody(ilBytes);
				}
				return BuildFatBody(ilBytes, 8, 0u, null);
			}
			if (num == 3)
			{
				ushort num2 = (ushort)((((DataReader)(ref pe)).ReadByte() << 8) | b);
				ushort maxStack = ((DataReader)(ref pe)).ReadUInt16();
				uint num3 = ((DataReader)(ref pe)).ReadUInt32();
				uint localVarSigTok = ((DataReader)(ref pe)).ReadUInt32();
				bool num4 = (num2 & 8) != 0;
				byte[] exceptionSections = null;
				if (num4)
				{
					uint position = (((DataReader)(ref pe)).Position + num3 + 3) & 0xFFFFFFFCu;
					((DataReader)(ref pe)).Position = position;
					exceptionSections = ReadExceptionSections(ref pe);
				}
				return BuildFatBody(ilBytes, maxStack, localVarSigTok, exceptionSections);
			}
		}
		catch
		{
		}
		if (ilBytes.Length <= 63)
		{
			return TinyBody(ilBytes);
		}
		return BuildFatBody(ilBytes, 8, 0u, null);
	}

	private static byte[] TinyBody(byte[] ilBytes)
	{
		byte[] array = new byte[ilBytes.Length + 1];
		array[0] = (byte)((ilBytes.Length << 2) | 2);
		Buffer.BlockCopy(ilBytes, 0, array, 1, ilBytes.Length);
		return array;
	}

	private static byte[] ReadExceptionSections(ref DataReader pe)
	{
		MemoryStream memoryStream = new MemoryStream();
		try
		{
			bool flag = true;
			while (flag && ((DataReader)(ref pe)).Position < ((DataReader)(ref pe)).Length)
			{
				byte b = ((DataReader)(ref pe)).ReadByte();
				flag = (b & 0x80) != 0;
				if ((b & 0x40) != 0)
				{
					byte b2 = ((DataReader)(ref pe)).ReadByte();
					byte b3 = ((DataReader)(ref pe)).ReadByte();
					byte b4 = ((DataReader)(ref pe)).ReadByte();
					int num = b2 | (b3 << 8) | (b4 << 16);
					memoryStream.WriteByte(b);
					memoryStream.WriteByte(b2);
					memoryStream.WriteByte(b3);
					memoryStream.WriteByte(b4);
					int num2 = num - 4;
					if (num2 > 0)
					{
						byte[] array = new byte[num2];
						((DataReader)(ref pe)).ReadBytes(array, 0, num2);
						memoryStream.Write(array, 0, num2);
					}
				}
				else
				{
					byte b5 = ((DataReader)(ref pe)).ReadByte();
					byte value = ((DataReader)(ref pe)).ReadByte();
					byte value2 = ((DataReader)(ref pe)).ReadByte();
					memoryStream.WriteByte(b);
					memoryStream.WriteByte(b5);
					memoryStream.WriteByte(value);
					memoryStream.WriteByte(value2);
					int num3 = b5 - 4;
					if (num3 > 0)
					{
						byte[] array2 = new byte[num3];
						((DataReader)(ref pe)).ReadBytes(array2, 0, num3);
						memoryStream.Write(array2, 0, num3);
					}
				}
			}
		}
		catch
		{
		}
		return memoryStream.ToArray();
	}

	private static byte[] BuildFatBody(byte[] ilBytes, ushort maxStack, uint localVarSigTok, byte[] exceptionSections)
	{
		bool flag = exceptionSections != null && exceptionSections.Length != 0;
		int num = 0;
		if (flag)
		{
			int num2 = 12 + ilBytes.Length;
			num = ((num2 + 3) & -4) - num2;
		}
		byte[] array = new byte[12 + ilBytes.Length + num + (flag ? exceptionSections.Length : 0)];
		ushort num3 = 12291;
		if (flag)
		{
			num3 |= 8;
		}
		array[0] = (byte)(num3 & 0xFF);
		array[1] = (byte)((num3 >> 8) & 0xFF);
		array[2] = (byte)(maxStack & 0xFF);
		array[3] = (byte)((maxStack >> 8) & 0xFF);
		int num4 = ilBytes.Length;
		array[4] = (byte)(num4 & 0xFF);
		array[5] = (byte)((num4 >> 8) & 0xFF);
		array[6] = (byte)((num4 >> 16) & 0xFF);
		array[7] = (byte)((num4 >> 24) & 0xFF);
		array[8] = (byte)(localVarSigTok & 0xFF);
		array[9] = (byte)((localVarSigTok >> 8) & 0xFF);
		array[10] = (byte)((localVarSigTok >> 16) & 0xFF);
		array[11] = (byte)((localVarSigTok >> 24) & 0xFF);
		Buffer.BlockCopy(ilBytes, 0, array, 12, ilBytes.Length);
		if (flag)
		{
			Buffer.BlockCopy(exceptionSections, 0, array, 12 + ilBytes.Length + num, exceptionSections.Length);
		}
		return array;
	}

	private void RemoveNoInlining()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (TypeDef type in ((ModuleDef)_module).GetTypes())
		{
			foreach (MethodDef method in type.Methods)
			{
				if ((method.ImplAttributes & 8) != 0)
				{
					method.ImplAttributes = (MethodImplAttributes)(method.ImplAttributes & 0xFFF7);
					num++;
				}
			}
		}
		_log("[*] Removed NoInlining from " + num + " methods");
	}

	private void Cleanup(MethodDef initMethod)
	{
		MethodDef val = ((ModuleDef)_module).GlobalType.FindOrCreateStaticConstructor();
		if (val == null || !val.HasBody)
		{
			return;
		}
		IList<Instruction> instructions = val.Body.Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			Instruction val2 = instructions[i];
			if (val2.OpCode == OpCodes.Call && val2.Operand == initMethod)
			{
				instructions.RemoveAt(i);
				_log("[*] Removed JIT hook from .cctor");
				break;
			}
		}
	}

	public void Save(string output)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		ModuleWriterOptions val = new ModuleWriterOptions((ModuleDef)(object)_module);
		MetadataOptions metadataOptions = ((ModuleWriterOptionsBase)val).MetadataOptions;
		metadataOptions.Flags = (MetadataFlags)(metadataOptions.Flags | 0x7FFF);
		MetadataOptions metadataOptions2 = ((ModuleWriterOptionsBase)val).MetadataOptions;
		metadataOptions2.Flags = (MetadataFlags)(metadataOptions2.Flags | 0x8000);
		((ModuleWriterOptionsBase)val).Logger = (ILogger)(object)DummyLogger.NoThrowInstance;
		((ModuleDef)_module).Write(output, val);
		_log("[SUCCESS] Saved to: " + output);
	}
}
