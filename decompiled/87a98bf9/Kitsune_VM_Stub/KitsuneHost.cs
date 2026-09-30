using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Kitsune_VM_Stub;

public static class KitsuneHost
{
	private static KitsuneExternalCallTable _externalCalls;

	private static KitsuneHeapRuntime _heapRuntime;

	private static KitsuneObjectHeap _persistentHeap;

	private static bool _initialized = false;

	private static byte[] _reversePerm = null;

	private static readonly object _initLock = new object();

	private static byte[] _masterKey;

	private static byte[][] _encryptedByLogical;

	private static byte[][] _decryptedChunks;

	private static object[] _chunkLocks;

	private const int MaxHotChunks = 16;

	private static int[] _chunkRefCount;

	private static LinkedList<int> _lruList;

	private static Dictionary<int, LinkedListNode<int>> _lruNodes;

	private static readonly object _lruLock = new object();

	private static uint[] _firstDecryptCRC;

	private static bool[] _chunkDecryptedOnce;

	private static int[] _methChunkId;

	private static int[] _methOffset;

	private static int[] _methLength;

	private static uint _wmBase = 0u;

	private static byte[] _vmCrcExpected = null;

	private static byte[] _bytecodeCrcExpected = null;

	private static byte _protectionFlags = 0;

	private static bool _vmCrcDone2 = false;

	private static bool _vmCrcDone3 = false;

	private static bool _bytecodeCrcDone2 = false;

	private static bool _headerCrcDone = false;

	private static byte[] _headerHashFromMemory = null;

	private static readonly string[] _wellKnownDlls = new string[24]
	{
		"System.dll", "System.Core.dll", "System.Net.dll", "System.Net.Http.dll", "System.Net.Sockets.dll", "System.Runtime.dll", "System.Collections.dll", "System.Linq.dll", "System.Xml.dll", "System.Data.dll",
		"System.Drawing.dll", "System.Windows.Forms.dll", "Microsoft.VisualBasic.dll", "System.Management.dll", "System.ServiceProcess.dll", "System.DirectoryServices.dll", "System.Runtime.Remoting.dll", "System.Web.dll", "System.Transactions.dll", "System.Numerics.dll",
		"PresentationCore.dll", "PresentationFramework.dll", "WindowsBase.dll", "System.Xaml.dll"
	};

	public static long RegisterArg(object arg)
	{
		if (_persistentHeap == null)
		{
			lock (_initLock)
			{
				if (_persistentHeap == null)
				{
					_persistentHeap = new KitsuneObjectHeap();
				}
			}
		}
		return _persistentHeap.Alloc(arg);
	}

	public static object RetrieveResult(long handle)
	{
		if (_persistentHeap == null)
		{
			return null;
		}
		return _persistentHeap.Get(handle);
	}

	private static void Initialize()
	{
		lock (_initLock)
		{
			if (_initialized)
			{
				return;
			}
			_persistentHeap = new KitsuneObjectHeap();
			KitsunePEReader.ReadIndex(out var reversePerm, out var masterKey, out var totalChunks, out var methChunkId, out var methOffset, out var methLength, out var encryptedByLogical, out var crcExpected, out var protectionFlags, out var vmCrcExpected);
			KitsuneGuard.Initialize(protectionFlags);
			_protectionFlags = protectionFlags;
			KitsuneGuard.CheckDebugger();
			KitsuneGuard.CheckVM();
			if (crcExpected != null && crcExpected.Length == 32)
			{
				byte[] array;
				using (SHA256 sHA = SHA256.Create())
				{
					using MemoryStream memoryStream = new MemoryStream();
					for (int i = 0; i < encryptedByLogical.Length; i++)
					{
						if (encryptedByLogical[i] != null)
						{
							memoryStream.Write(encryptedByLogical[i], 0, encryptedByLogical[i].Length);
						}
					}
					array = sHA.ComputeHash(memoryStream.ToArray());
				}
				bool flag = array.Length == crcExpected.Length;
				for (int j = 0; (j < array.Length) & flag; j++)
				{
					if (array[j] != crcExpected[j])
					{
						flag = false;
					}
				}
				if (!flag)
				{
					Random random = new Random();
					for (int k = 0; k < encryptedByLogical.Length; k++)
					{
						if (encryptedByLogical[k] != null)
						{
							random.NextBytes(encryptedByLogical[k]);
							encryptedByLogical[k] = null;
						}
					}
					KitsuneGuard.Corrupt("bc-crc1");
				}
			}
			if ((protectionFlags & 8) != 0 && vmCrcExpected != null && vmCrcExpected.Length == 32)
			{
				try
				{
					string location = Assembly.GetExecutingAssembly().Location;
					if (string.IsNullOrEmpty(location))
					{
						location = Assembly.GetExecutingAssembly().Location;
					}
					byte[] array2 = File.ReadAllBytes(location);
					int num = BitConverter.ToInt32(array2, 60) + 4;
					int num2 = BitConverter.ToUInt16(array2, num + 2);
					int num3 = BitConverter.ToUInt16(array2, num + 16);
					int num4 = num + 20 + num3;
					int num5 = int.MaxValue;
					for (int l = 0; l < num2; l++)
					{
						int num6 = num4 + l * 40;
						uint num7 = BitConverter.ToUInt32(array2, num6 + 20);
						if (num7 != 0 && num7 + 4 <= (uint)array2.Length)
						{
							uint num8 = BitConverter.ToUInt32(array2, (int)num7);
							if ((num8 == 1263817577 || num8 == 1263817572) && (int)num7 < num5)
							{
								num5 = (int)num7;
							}
						}
					}
					if (num5 > 0 && num5 != int.MaxValue)
					{
						byte[] array3 = new byte[num5];
						Buffer.BlockCopy(array2, 0, array3, 0, num5);
						byte[] array4;
						using (SHA256 sHA2 = SHA256.Create())
						{
							array4 = sHA2.ComputeHash(array3);
						}
						bool flag2 = array4.Length == vmCrcExpected.Length;
						for (int m = 0; (m < array4.Length) & flag2; m++)
						{
							if (array4[m] != vmCrcExpected[m])
							{
								flag2 = false;
							}
						}
						if (!flag2)
						{
							KitsuneGuard.Corrupt("vm-crc1");
						}
					}
				}
				catch
				{
				}
			}
			KitsuneGuard.AntiDump();
			_vmCrcExpected = vmCrcExpected;
			_bytecodeCrcExpected = crcExpected;
			uint num9 = uint.MaxValue;
			for (int n = 0; n < masterKey.Length; n++)
			{
				num9 ^= masterKey[n];
				for (int num10 = 0; num10 < 8; num10++)
				{
					num9 = (((num9 & 1) != 0) ? ((num9 >> 1) ^ 0xEDB88320u) : (num9 >> 1));
				}
			}
			_wmBase = num9 ^ 0xFFFFFFFFu;
			try
			{
				IntPtr hINSTANCE = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule);
				if (hINSTANCE != IntPtr.Zero && hINSTANCE.ToInt64() != -1)
				{
					byte[] array5 = new byte[4032];
					for (int num11 = 0; num11 < array5.Length; num11++)
					{
						array5[num11] = Marshal.ReadByte(hINSTANCE + 64 + num11);
					}
					using SHA256 sHA3 = SHA256.Create();
					_headerHashFromMemory = sHA3.ComputeHash(array5);
				}
			}
			catch
			{
			}
			_reversePerm = reversePerm;
			_masterKey = masterKey;
			_methChunkId = methChunkId;
			_methOffset = methOffset;
			_methLength = methLength;
			_encryptedByLogical = encryptedByLogical;
			_decryptedChunks = new byte[totalChunks][];
			_chunkLocks = new object[totalChunks];
			for (int num12 = 0; num12 < totalChunks; num12++)
			{
				_chunkLocks[num12] = new object();
			}
			_chunkRefCount = new int[totalChunks];
			_firstDecryptCRC = new uint[totalChunks];
			_chunkDecryptedOnce = new bool[totalChunks];
			_lruList = new LinkedList<int>();
			_lruNodes = new Dictionary<int, LinkedListNode<int>>();
			using (MemoryStream memoryStream2 = new MemoryStream(GetOrDecryptChunk(0)))
			{
				using BinaryReader binaryReader = new BinaryReader(memoryStream2, Encoding.UTF8);
				int num13 = binaryReader.ReadInt32();
				for (int num14 = 0; num14 < num13; num14++)
				{
					string assemblyString = binaryReader.ReadString();
					try
					{
						Assembly.Load(assemblyString);
					}
					catch
					{
					}
				}
				_externalCalls = new KitsuneExternalCallTable();
				int num15 = binaryReader.ReadInt32();
				for (int num16 = 0; num16 < num15; num16++)
				{
					int id = binaryReader.ReadInt32();
					string text = binaryReader.ReadString();
					string text2 = binaryReader.ReadString();
					bool hasReturnValue = binaryReader.ReadBoolean();
					int num17 = binaryReader.ReadInt32();
					Type[] array6 = new Type[num17];
					bool flag3 = false;
					for (int num18 = 0; num18 < num17; num18++)
					{
						string text3 = binaryReader.ReadString();
						if (IsGenericParamRef(text3))
						{
							flag3 = true;
							array6[num18] = null;
							continue;
						}
						try
						{
							array6[num18] = ResolveType(text3);
						}
						catch (Exception ex)
						{
							try
							{
								File.AppendAllText("kitsune_debug.log", "ResolveType FAIL id=" + id + " type=" + text + " method=" + text2 + " param=" + text3 + "\n" + ex.Message + "\n");
							}
							catch
							{
							}
							throw;
						}
					}
					if (flag3)
					{
						array6 = new Type[0];
					}
					int num19 = binaryReader.ReadInt32();
					string[] array7 = new string[num19];
					for (int num20 = 0; num20 < num19; num20++)
					{
						array7[num20] = binaryReader.ReadString();
					}
					try
					{
						_externalCalls.RegisterFromReflection(text, text2, array6, hasReturnValue, id, (num19 > 0) ? array7 : null);
					}
					catch (Exception ex2)
					{
						try
						{
							File.AppendAllText("kitsune_debug.log", "RegisterFromReflection FAIL id=" + id + " type=" + text + " method=" + text2 + "\n" + ex2.Message + "\n");
						}
						catch
						{
						}
						throw;
					}
				}
				_heapRuntime = ((memoryStream2.Position < memoryStream2.Length) ? LoadHeapRuntime(binaryReader) : KitsuneHeapRuntime.Empty());
				_heapRuntime.Heap = _persistentHeap;
			}
			_initialized = true;
		}
	}

	private static void RunVmCrcSite3()
	{
		if ((_protectionFlags & 8) == 0 || _vmCrcExpected == null || _vmCrcExpected.Length != 32)
		{
			return;
		}
		try
		{
			byte[] array = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
			int num = BitConverter.ToInt32(array, 60) + 4;
			int num2 = BitConverter.ToUInt16(array, num + 2);
			int num3 = BitConverter.ToUInt16(array, num + 16);
			int num4 = num + 20 + num3;
			int num5 = int.MaxValue;
			for (int i = 0; i < num2; i++)
			{
				int num6 = num4 + i * 40;
				uint num7 = BitConverter.ToUInt32(array, num6 + 20);
				if (num7 != 0 && num7 + 4 <= (uint)array.Length)
				{
					uint num8 = BitConverter.ToUInt32(array, (int)num7);
					if ((num8 == 1263817577 || num8 == 1263817572) && (int)num7 < num5)
					{
						num5 = (int)num7;
					}
				}
			}
			if (num5 <= 0 || num5 == int.MaxValue)
			{
				return;
			}
			byte[] array2 = new byte[num5];
			Buffer.BlockCopy(array, 0, array2, 0, num5);
			byte[] array3;
			using (SHA256 sHA = SHA256.Create())
			{
				array3 = sHA.ComputeHash(array2);
			}
			uint[] array4 = new uint[256];
			for (int j = 0; j < 256; j++)
			{
				uint num9 = (uint)j;
				for (int k = 0; k < 8; k++)
				{
					num9 = (((num9 & 1) != 0) ? ((num9 >> 1) ^ 0x82F63B78u) : (num9 >> 1));
				}
				array4[j] = num9;
			}
			uint num10 = uint.MaxValue;
			for (int l = 0; l < array3.Length; l++)
			{
				num10 = array4[(num10 ^ array3[l]) & 0xFF] ^ (num10 >> 8);
			}
			num10 ^= 0xFFFFFFFFu;
			uint num11 = uint.MaxValue;
			for (int m = 0; m < _vmCrcExpected.Length; m++)
			{
				num11 = array4[(num11 ^ _vmCrcExpected[m]) & 0xFF] ^ (num11 >> 8);
			}
			num11 ^= 0xFFFFFFFFu;
			if (num10 != num11)
			{
				KitsuneGuard.Corrupt("vm-crc3");
			}
		}
		catch
		{
		}
	}

	private static byte[] GetOrDecryptChunk(int logicalChunk)
	{
		if (_decryptedChunks[logicalChunk] != null)
		{
			return _decryptedChunks[logicalChunk];
		}
		lock (_chunkLocks[logicalChunk])
		{
			if (_decryptedChunks[logicalChunk] != null)
			{
				return _decryptedChunks[logicalChunk];
			}
			byte[] packet = _encryptedByLogical[logicalChunk] ?? throw new Exception("KitsuneHost: encrypted bytes for chunk #" + logicalChunk + " are missing.");
			if (!_vmCrcDone3)
			{
				_vmCrcDone3 = true;
				RunVmCrcSite3();
			}
			byte[] array = KitsuneCrypto.Decrypt(packet, _masterKey, logicalChunk);
			uint num = KitsuneCrypto.Crc32(array, 0, array.Length);
			if (_chunkDecryptedOnce[logicalChunk])
			{
				if (num != _firstDecryptCRC[logicalChunk])
				{
					KitsuneGuard.Corrupt("redecrypt-crc");
				}
			}
			else
			{
				_firstDecryptCRC[logicalChunk] = num;
				_chunkDecryptedOnce[logicalChunk] = true;
			}
			_decryptedChunks[logicalChunk] = array;
			TouchLru(logicalChunk);
			TryEvict();
			return _decryptedChunks[logicalChunk];
		}
	}

	private static byte[] AcquireChunk(int logicalChunk)
	{
		byte[] orDecryptChunk = GetOrDecryptChunk(logicalChunk);
		Interlocked.Increment(ref _chunkRefCount[logicalChunk]);
		TouchLru(logicalChunk);
		return orDecryptChunk;
	}

	private static void ReleaseChunk(int logicalChunk)
	{
		Interlocked.Decrement(ref _chunkRefCount[logicalChunk]);
	}

	private static void TouchLru(int logicalChunk)
	{
		lock (_lruLock)
		{
			if (_lruNodes.TryGetValue(logicalChunk, out var value))
			{
				_lruList.Remove(value);
				_lruList.AddFirst(value);
			}
			else
			{
				value = _lruList.AddFirst(logicalChunk);
				_lruNodes[logicalChunk] = value;
			}
		}
	}

	private static void TryEvict()
	{
		lock (_lruLock)
		{
			while (_lruList.Count > 16)
			{
				LinkedListNode<int> last = _lruList.Last;
				if (last == null)
				{
					break;
				}
				int value = last.Value;
				if (value == 0 || Volatile.Read(in _chunkRefCount[value]) > 0)
				{
					break;
				}
				_lruList.RemoveLast();
				_lruNodes.Remove(value);
				_decryptedChunks[value] = null;
			}
		}
	}

	private static void ReadMethodFromChunk(int idx, out byte[] bc, out KitsuneEHEntry[] eh)
	{
		int logicalChunk = _methChunkId[idx];
		int index = _methOffset[idx];
		int count = _methLength[idx];
		byte[] buffer = AcquireChunk(logicalChunk);
		try
		{
			using MemoryStream input = new MemoryStream(buffer, index, count);
			using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8);
			int count2 = binaryReader.ReadInt32();
			bc = binaryReader.ReadBytes(count2);
			eh = ReadEHTable(binaryReader);
		}
		finally
		{
			ReleaseChunk(logicalChunk);
		}
	}

	public static long Execute(int bytecodeIndex, long[] args)
	{
		if (!_initialized)
		{
			Initialize();
		}
		if (!_vmCrcDone2)
		{
			_vmCrcDone2 = true;
			if ((_protectionFlags & 8) != 0 && _vmCrcExpected != null && _vmCrcExpected.Length == 32)
			{
				try
				{
					byte[] array = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
					int num = BitConverter.ToInt32(array, 60) + 4;
					int num2 = BitConverter.ToUInt16(array, num + 2);
					int num3 = BitConverter.ToUInt16(array, num + 16);
					int num4 = num + 20 + num3;
					int num5 = int.MaxValue;
					for (int i = 0; i < num2; i++)
					{
						int num6 = num4 + i * 40;
						uint num7 = BitConverter.ToUInt32(array, num6 + 20);
						if (num7 != 0 && num7 + 4 <= (uint)array.Length)
						{
							uint num8 = BitConverter.ToUInt32(array, (int)num7);
							if ((num8 == 1263817577 || num8 == 1263817572) && (int)num7 < num5)
							{
								num5 = (int)num7;
							}
						}
					}
					if (num5 > 0 && num5 != int.MaxValue)
					{
						byte[] array2 = new byte[num5];
						Buffer.BlockCopy(array, 0, array2, 0, num5);
						byte[] array3;
						using (SHA256 sHA = SHA256.Create())
						{
							array3 = sHA.ComputeHash(array2);
						}
						ulong num9 = 14695981039346656037uL;
						for (int j = 0; j < array3.Length; j++)
						{
							num9 = (num9 ^ array3[j]) * 1099511628211L;
						}
						ulong num10 = 14695981039346656037uL;
						for (int k = 0; k < _vmCrcExpected.Length; k++)
						{
							num10 = (num10 ^ _vmCrcExpected[k]) * 1099511628211L;
						}
						if (num9 != num10)
						{
							KitsuneGuard.Corrupt("vm-crc2");
						}
					}
				}
				catch
				{
				}
			}
		}
		if (!_bytecodeCrcDone2)
		{
			_bytecodeCrcDone2 = true;
			if (_bytecodeCrcExpected != null && _bytecodeCrcExpected.Length == 32)
			{
				uint[] array4 = new uint[256];
				for (int l = 0; l < 256; l++)
				{
					uint num11 = (uint)l;
					for (int m = 0; m < 8; m++)
					{
						num11 = (((num11 & 1) != 0) ? ((num11 >> 1) ^ 0xEDB88320u) : (num11 >> 1));
					}
					array4[l] = num11;
				}
				uint num12 = uint.MaxValue;
				for (int n = 0; n < _bytecodeCrcExpected.Length; n++)
				{
					num12 = array4[(num12 ^ _bytecodeCrcExpected[n]) & 0xFF] ^ (num12 >> 8);
				}
				num12 ^= 0xFFFFFFFFu;
				uint num13 = uint.MaxValue;
				for (int num14 = 0; num14 < _bytecodeCrcExpected.Length; num14++)
				{
					num13 = array4[(num13 ^ _bytecodeCrcExpected[num14]) & 0xFF] ^ (num13 >> 8);
				}
				num13 ^= 0xFFFFFFFFu;
				if (num12 != num13)
				{
					KitsuneGuard.Corrupt("bc-crc2");
				}
			}
		}
		if (!_headerCrcDone)
		{
			_headerCrcDone = true;
			if (_headerHashFromMemory != null)
			{
				try
				{
					IntPtr hINSTANCE = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule);
					if (hINSTANCE != IntPtr.Zero && hINSTANCE.ToInt64() != -1)
					{
						byte[] array5 = new byte[4032];
						for (int num15 = 0; num15 < array5.Length; num15++)
						{
							array5[num15] = Marshal.ReadByte(hINSTANCE + 64 + num15);
						}
						byte[] array6;
						using (SHA256 sHA2 = SHA256.Create())
						{
							array6 = sHA2.ComputeHash(array5);
						}
						bool flag = array6.Length == _headerHashFromMemory.Length;
						for (int num16 = 0; (num16 < array6.Length) & flag; num16++)
						{
							if (array6[num16] != _headerHashFromMemory[num16])
							{
								flag = false;
							}
						}
						if (!flag)
						{
							KitsuneGuard.Corrupt("mem-hdr");
						}
					}
				}
				catch
				{
				}
			}
		}
		ReadMethodFromChunk(bytecodeIndex, out var bc, out var eh);
		if (bc.Length >= 5)
		{
			if (_wmBase != 0)
			{
				uint num17 = BitConverter.ToUInt32(bc, 1);
				uint num18 = _wmBase ^ (uint)bytecodeIndex;
				if (num17 != num18)
				{
					KitsuneGuard.Corrupt("wm");
				}
			}
			byte[] array7 = new byte[bc.Length - 5];
			Buffer.BlockCopy(bc, 5, array7, 0, array7.Length);
			bc = array7;
		}
		KitsuneHeapRuntime heapRuntime = _heapRuntime;
		KitsuneDispatcher kitsuneDispatcher = new KitsuneDispatcher(bc, _externalCalls, heapRuntime, eh, _reversePerm);
		if (args != null)
		{
			for (int num19 = 0; num19 < args.Length; num19++)
			{
				kitsuneDispatcher.State.R[num19] = args[num19];
			}
		}
		kitsuneDispatcher.Run();
		return kitsuneDispatcher.State.R[0];
	}

	public static long Execute(int bytecodeIndex)
	{
		return Execute(bytecodeIndex, null);
	}

	private static KitsuneEHEntry[] ReadEHTable(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		KitsuneEHEntry[] array = new KitsuneEHEntry[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new KitsuneEHEntry
			{
				VmTryStart = reader.ReadInt32(),
				VmTryEnd = reader.ReadInt32(),
				VmHandlerStart = reader.ReadInt32(),
				VmHandlerEnd = reader.ReadInt32(),
				HandlerType = (KitsuneHandlerType)reader.ReadByte(),
				CatchTypeId = reader.ReadInt32()
			};
		}
		return array;
	}

	private static KitsuneHeapRuntime LoadHeapRuntime(BinaryReader reader)
	{
		KitsuneHeapRuntime kitsuneHeapRuntime = new KitsuneHeapRuntime();
		int num = reader.ReadInt32();
		kitsuneHeapRuntime.Strings = new string[num];
		for (int i = 0; i < num; i++)
		{
			kitsuneHeapRuntime.Strings[i] = reader.ReadString();
		}
		int num2 = reader.ReadInt32();
		kitsuneHeapRuntime.Fields = new FieldInfo[num2];
		for (int j = 0; j < num2; j++)
		{
			string text = reader.ReadString();
			string text2 = reader.ReadString();
			Type type = ResolveType(text);
			kitsuneHeapRuntime.Fields[j] = type.GetField(text2, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (kitsuneHeapRuntime.Fields[j] == null)
			{
				throw new Exception($"KitsuneHost: field '{text}.{text2}' not found via Reflection.");
			}
		}
		int num3 = reader.ReadInt32();
		List<Type> list = new List<Type>();
		List<ConstructorInfo> list2 = new List<ConstructorInfo>();
		for (int k = 0; k < num3; k++)
		{
			string text3 = reader.ReadString();
			bool flag = reader.ReadBoolean();
			Type type2 = ResolveType(text3);
			if (flag)
			{
				int num4 = reader.ReadInt32();
				Type[] array = new Type[num4];
				bool flag2 = false;
				for (int l = 0; l < num4; l++)
				{
					string text4 = reader.ReadString();
					if (IsGenericParamRef(text4))
					{
						flag2 = true;
						array[l] = null;
					}
					else
					{
						array[l] = ResolveType(text4);
					}
				}
				if (flag2)
				{
					array = new Type[0];
				}
				ConstructorInfo constructor = type2.GetConstructor(array);
				if (constructor == null)
				{
					throw new Exception(string.Format("KitsuneHost: constructor '{0}({1})' not found.", text3, string.Join(", ", Array.ConvertAll(array, (Type t) => t.Name))));
				}
				list2.Add(constructor);
				list.Add(type2);
			}
			else
			{
				list.Add(type2);
				list2.Add(null);
			}
		}
		kitsuneHeapRuntime.Types = list.ToArray();
		kitsuneHeapRuntime.Ctors = list2.ToArray();
		kitsuneHeapRuntime.Heap = new KitsuneObjectHeap();
		return kitsuneHeapRuntime;
	}

	private static Type ResolveType(string fullName)
	{
		string text = fullName.Replace('/', '+');
		Type type = TryFindType(text);
		if (type != null)
		{
			return type;
		}
		int num = text.LastIndexOf('+');
		if (num > 0)
		{
			string clrName = text.Substring(0, num);
			string text2 = text.Substring(num + 1);
			Type type2 = TryFindType(clrName);
			if (type2 == null)
			{
				type2 = ScanAllTypesForName(clrName);
			}
			if (type2 != null)
			{
				type = type2.GetNestedType(text2, BindingFlags.Public | BindingFlags.NonPublic);
				if (type != null)
				{
					return type;
				}
				Type[] nestedTypes = type2.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
				foreach (Type type3 in nestedTypes)
				{
					if (type3.Name == text2)
					{
						return type3;
					}
				}
			}
		}
		int num2 = text.IndexOf('[');
		if (num2 > 0 && text.EndsWith("]"))
		{
			string clrName2 = text.Substring(0, num2);
			string s = text.Substring(num2 + 1, text.Length - num2 - 2);
			Type type4 = TryFindType(clrName2);
			if (type4 != null && type4.IsGenericTypeDefinition)
			{
				string[] array = SplitGenericArgs(s);
				Type[] array2 = new Type[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = ResolveType(array[j].Trim());
				}
				return type4.MakeGenericType(array2);
			}
		}
		throw new Exception("KitsuneHost: failed to find type: " + fullName);
	}

	private static bool IsGenericParamRef(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		string text = name.TrimEnd(new char[2] { '&', '*' }).TrimEnd(new char[2] { ']', '[' });
		if (!text.StartsWith("!"))
		{
			return text.StartsWith("!!");
		}
		return true;
	}

	private static Type TryFindType(string clrName)
	{
		if (IsGenericParamRef(clrName))
		{
			return null;
		}
		Type type = null;
		try
		{
			type = Type.GetType(clrName);
		}
		catch
		{
		}
		if (type != null)
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			try
			{
				type = assembly.GetType(clrName);
			}
			catch
			{
			}
			if (type != null)
			{
				return type;
			}
		}
		string directoryName = Path.GetDirectoryName(typeof(object).Assembly.Location);
		string[] wellKnownDlls = _wellKnownDlls;
		foreach (string path in wellKnownDlls)
		{
			string text = Path.Combine(directoryName, path);
			if (!File.Exists(text))
			{
				continue;
			}
			try
			{
				type = Assembly.LoadFrom(text).GetType(clrName);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
		}
		try
		{
			wellKnownDlls = Directory.GetFiles(directoryName, "*.dll");
			foreach (string assemblyFile in wellKnownDlls)
			{
				try
				{
					type = Assembly.LoadFrom(assemblyFile).GetType(clrName);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		string directoryName2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		if (!string.IsNullOrEmpty(directoryName2))
		{
			wellKnownDlls = Directory.GetFiles(directoryName2, "*.dll");
			foreach (string assemblyFile2 in wellKnownDlls)
			{
				try
				{
					type = Assembly.LoadFrom(assemblyFile2).GetType(clrName);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
		}
		type = ScanAllTypesForName(clrName);
		if (type != null)
		{
			return type;
		}
		return null;
	}

	private static Type ScanAllTypesForName(string clrName)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] array = null;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				array = ex.Types;
			}
			catch
			{
				continue;
			}
			if (array == null)
			{
				continue;
			}
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				if (type != null && type.FullName == clrName)
				{
					return type;
				}
			}
		}
		return null;
	}

	private static string[] SplitGenericArgs(string s)
	{
		List<string> list = new List<string>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == '[')
			{
				num++;
			}
			else if (s[i] == ']')
			{
				num--;
			}
			else if (s[i] == ',' && num == 0)
			{
				list.Add(s.Substring(num2, i - num2));
				num2 = i + 1;
			}
		}
		list.Add(s.Substring(num2));
		return list.ToArray();
	}
}
