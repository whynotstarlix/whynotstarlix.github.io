using System;
using System.IO;
using System.Runtime.InteropServices;

namespace _3DViewerControls.Data;

public class PeHeaderReader
{
	public struct IMAGE_DOS_HEADER
	{
		public ushort e_magic;

		public ushort e_cblp;

		public ushort e_cp;

		public ushort e_crlc;

		public ushort e_cparhdr;

		public ushort e_minalloc;

		public ushort e_maxalloc;

		public ushort e_ss;

		public ushort e_sp;

		public ushort e_csum;

		public ushort e_ip;

		public ushort e_cs;

		public ushort e_lfarlc;

		public ushort e_ovno;

		public ushort e_res_0;

		public ushort e_res_1;

		public ushort e_res_2;

		public ushort e_res_3;

		public ushort e_oemid;

		public ushort e_oeminfo;

		public ushort e_res2_0;

		public ushort e_res2_1;

		public ushort e_res2_2;

		public ushort e_res2_3;

		public ushort e_res2_4;

		public ushort e_res2_5;

		public ushort e_res2_6;

		public ushort e_res2_7;

		public ushort e_res2_8;

		public ushort e_res2_9;

		public uint e_lfanew;
	}

	public struct IMAGE_DATA_DIRECTORY
	{
		public uint VirtualAddress;

		public uint Size;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER32
	{
		public ushort Magic;

		public byte MajorLinkerVersion;

		public byte MinorLinkerVersion;

		public uint SizeOfCode;

		public uint SizeOfInitializedData;

		public uint SizeOfUninitializedData;

		public uint AddressOfEntryPoint;

		public uint BaseOfCode;

		public uint BaseOfData;

		public uint ImageBase;

		public uint SectionAlignment;

		public uint FileAlignment;

		public ushort MajorOperatingSystemVersion;

		public ushort MinorOperatingSystemVersion;

		public ushort MajorImageVersion;

		public ushort MinorImageVersion;

		public ushort MajorSubsystemVersion;

		public ushort MinorSubsystemVersion;

		public uint Win32VersionValue;

		public uint SizeOfImage;

		public uint SizeOfHeaders;

		public uint CheckSum;

		public ushort Subsystem;

		public ushort DllCharacteristics;

		public uint SizeOfStackReserve;

		public uint SizeOfStackCommit;

		public uint SizeOfHeapReserve;

		public uint SizeOfHeapCommit;

		public uint LoaderFlags;

		public uint NumberOfRvaAndSizes;

		public IMAGE_DATA_DIRECTORY ExportTable;

		public IMAGE_DATA_DIRECTORY ImportTable;

		public IMAGE_DATA_DIRECTORY ResourceTable;

		public IMAGE_DATA_DIRECTORY ExceptionTable;

		public IMAGE_DATA_DIRECTORY CertificateTable;

		public IMAGE_DATA_DIRECTORY BaseRelocationTable;

		public IMAGE_DATA_DIRECTORY Debug;

		public IMAGE_DATA_DIRECTORY Architecture;

		public IMAGE_DATA_DIRECTORY GlobalPtr;

		public IMAGE_DATA_DIRECTORY TLSTable;

		public IMAGE_DATA_DIRECTORY LoadConfigTable;

		public IMAGE_DATA_DIRECTORY BoundImport;

		public IMAGE_DATA_DIRECTORY IAT;

		public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

		public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

		public IMAGE_DATA_DIRECTORY Reserved;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER64
	{
		public ushort Magic;

		public byte MajorLinkerVersion;

		public byte MinorLinkerVersion;

		public uint SizeOfCode;

		public uint SizeOfInitializedData;

		public uint SizeOfUninitializedData;

		public uint AddressOfEntryPoint;

		public uint BaseOfCode;

		public ulong ImageBase;

		public uint SectionAlignment;

		public uint FileAlignment;

		public ushort MajorOperatingSystemVersion;

		public ushort MinorOperatingSystemVersion;

		public ushort MajorImageVersion;

		public ushort MinorImageVersion;

		public ushort MajorSubsystemVersion;

		public ushort MinorSubsystemVersion;

		public uint Win32VersionValue;

		public uint SizeOfImage;

		public uint SizeOfHeaders;

		public uint CheckSum;

		public ushort Subsystem;

		public ushort DllCharacteristics;

		public ulong SizeOfStackReserve;

		public ulong SizeOfStackCommit;

		public ulong SizeOfHeapReserve;

		public ulong SizeOfHeapCommit;

		public uint LoaderFlags;

		public uint NumberOfRvaAndSizes;

		public IMAGE_DATA_DIRECTORY ExportTable;

		public IMAGE_DATA_DIRECTORY ImportTable;

		public IMAGE_DATA_DIRECTORY ResourceTable;

		public IMAGE_DATA_DIRECTORY ExceptionTable;

		public IMAGE_DATA_DIRECTORY CertificateTable;

		public IMAGE_DATA_DIRECTORY BaseRelocationTable;

		public IMAGE_DATA_DIRECTORY Debug;

		public IMAGE_DATA_DIRECTORY Architecture;

		public IMAGE_DATA_DIRECTORY GlobalPtr;

		public IMAGE_DATA_DIRECTORY TLSTable;

		public IMAGE_DATA_DIRECTORY LoadConfigTable;

		public IMAGE_DATA_DIRECTORY BoundImport;

		public IMAGE_DATA_DIRECTORY IAT;

		public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

		public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

		public IMAGE_DATA_DIRECTORY Reserved;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_FILE_HEADER
	{
		public ushort Machine;

		public ushort NumberOfSections;

		public uint TimeDateStamp;

		public uint PointerToSymbolTable;

		public uint NumberOfSymbols;

		public ushort SizeOfOptionalHeader;

		public ushort Characteristics;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct IMAGE_SECTION_HEADER
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public char[] Name;

		[FieldOffset(8)]
		public uint VirtualSize;

		[FieldOffset(12)]
		public uint VirtualAddress;

		[FieldOffset(16)]
		public uint SizeOfRawData;

		[FieldOffset(20)]
		public uint PointerToRawData;

		[FieldOffset(24)]
		public uint PointerToRelocations;

		[FieldOffset(28)]
		public uint PointerToLinenumbers;

		[FieldOffset(32)]
		public ushort NumberOfRelocations;

		[FieldOffset(34)]
		public ushort NumberOfLinenumbers;

		[FieldOffset(36)]
		public DataSectionFlags Characteristics;

		public string Section => new string(Name);
	}

	[Flags]
	public enum DataSectionFlags : uint
	{
		TypeReg = 0u,
		TypeDsect = 1u,
		TypeNoLoad = 2u,
		TypeGroup = 4u,
		TypeNoPadded = 8u,
		TypeCopy = 0x10u,
		ContentCode = 0x20u,
		ContentInitializedData = 0x40u,
		ContentUninitializedData = 0x80u,
		LinkOther = 0x100u,
		LinkInfo = 0x200u,
		TypeOver = 0x400u,
		LinkRemove = 0x800u,
		LinkComDat = 0x1000u,
		NoDeferSpecExceptions = 0x4000u,
		RelativeGP = 0x8000u,
		MemPurgeable = 0x20000u,
		Memory16Bit = MemPurgeable,
		MemoryLocked = 0x40000u,
		MemoryPreload = 0x80000u,
		Align1Bytes = 0x100000u,
		Align2Bytes = 0x200000u,
		Align4Bytes = Align1Bytes | Align2Bytes,
		Align8Bytes = 0x400000u,
		Align16Bytes = Align1Bytes | Align8Bytes,
		Align32Bytes = Align2Bytes | Align8Bytes,
		Align64Bytes = Align4Bytes | Align8Bytes,
		Align128Bytes = 0x800000u,
		Align256Bytes = Align1Bytes | Align128Bytes,
		Align512Bytes = Align2Bytes | Align128Bytes,
		Align1024Bytes = Align4Bytes | Align128Bytes,
		Align2048Bytes = Align8Bytes | Align128Bytes,
		Align4096Bytes = Align16Bytes | Align128Bytes,
		Align8192Bytes = Align32Bytes | Align128Bytes,
		LinkExtendedRelocationOverflow = 0x1000000u,
		MemoryDiscardable = 0x2000000u,
		MemoryNotCached = 0x4000000u,
		MemoryNotPaged = 0x8000000u,
		MemoryShared = 0x10000000u,
		MemoryExecute = 0x20000000u,
		MemoryRead = 0x40000000u,
		MemoryWrite = 0x80000000u
	}

	private IMAGE_DOS_HEADER dosHeader;

	private IMAGE_FILE_HEADER fileHeader;

	private IMAGE_OPTIONAL_HEADER32 optionalHeader32;

	private IMAGE_OPTIONAL_HEADER64 optionalHeader64;

	private IMAGE_SECTION_HEADER[] imageSectionHeaders;

	public bool Is32BitHeader
	{
		get
		{
			ushort num = 256;
			return (num & FileHeader.Characteristics) == num;
		}
	}

	public IMAGE_FILE_HEADER FileHeader => fileHeader;

	public IMAGE_OPTIONAL_HEADER32 OptionalHeader32 => optionalHeader32;

	public IMAGE_OPTIONAL_HEADER64 OptionalHeader64 => optionalHeader64;

	public IMAGE_SECTION_HEADER[] ImageSectionHeaders => imageSectionHeaders;

	public DateTime TimeStamp
	{
		get
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(fileHeader.TimeDateStamp);
			return dateTime + TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
		}
	}

	public PeHeaderReader(string filePath)
	{
		using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		dosHeader = FromBinaryReader<IMAGE_DOS_HEADER>(binaryReader);
		fileStream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);
		binaryReader.ReadUInt32();
		fileHeader = FromBinaryReader<IMAGE_FILE_HEADER>(binaryReader);
		if (Is32BitHeader)
		{
			optionalHeader32 = FromBinaryReader<IMAGE_OPTIONAL_HEADER32>(binaryReader);
		}
		else
		{
			optionalHeader64 = FromBinaryReader<IMAGE_OPTIONAL_HEADER64>(binaryReader);
		}
		imageSectionHeaders = new IMAGE_SECTION_HEADER[fileHeader.NumberOfSections];
		for (int i = 0; i < imageSectionHeaders.Length; i++)
		{
			imageSectionHeaders[i] = FromBinaryReader<IMAGE_SECTION_HEADER>(binaryReader);
		}
	}

	public static T FromBinaryReader<T>(BinaryReader reader)
	{
		GCHandle gCHandle = GCHandle.Alloc(reader.ReadBytes(Marshal.SizeOf(typeof(T))), GCHandleType.Pinned);
		T result = (T)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T));
		gCHandle.Free();
		return result;
	}
}
