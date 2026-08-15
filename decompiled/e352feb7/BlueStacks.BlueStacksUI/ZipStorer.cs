using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlueStacks.BlueStacksUI;

public class ZipStorer : IDisposable
{
	private List<ZipFileEntry> Files = new List<ZipFileEntry>();

	private string FileName;

	private Stream ZipFileStream;

	private string Comment = "";

	private byte[] CentralDirImage;

	private ushort ExistingFiles;

	private FileAccess Access;

	private static uint[] CrcTable;

	private static Encoding DefaultEncoding;

	private bool disposedValue;

	public bool EncodeUTF8 { get; set; }

	public bool ForceDeflating { get; set; }

	static ZipStorer()
	{
		CrcTable = new uint[256];
		DefaultEncoding = Encoding.GetEncoding(437);
		for (int i = 0; i < CrcTable.Length; i++)
		{
			uint num = (uint)i;
			for (int j = 0; j < 8; j++)
			{
				num = (((num & 1) == 0) ? (num >> 1) : (0xEDB88320u ^ (num >> 1)));
			}
			CrcTable[i] = num;
		}
	}

	public static ZipStorer Create(string _filename, string _comment)
	{
		ZipStorer zipStorer = Create(new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite), _comment);
		zipStorer.Comment = _comment;
		zipStorer.FileName = _filename;
		return zipStorer;
	}

	public static ZipStorer Create(Stream _stream, string _comment)
	{
		return new ZipStorer
		{
			Comment = _comment,
			ZipFileStream = _stream,
			Access = FileAccess.Write
		};
	}

	public static ZipStorer Open(string _filename, FileAccess _access)
	{
		ZipStorer zipStorer = Open(new FileStream(_filename, FileMode.Open, (_access == FileAccess.Read) ? FileAccess.Read : FileAccess.ReadWrite), _access);
		zipStorer.FileName = _filename;
		return zipStorer;
	}

	public static ZipStorer Open(Stream _stream, FileAccess _access)
	{
		if (_stream != null)
		{
			if (!_stream.CanSeek && _access != FileAccess.Read)
			{
				throw new InvalidOperationException("Stream cannot seek");
			}
			ZipStorer zipStorer = new ZipStorer
			{
				ZipFileStream = _stream,
				Access = _access
			};
			if (zipStorer.ReadFileInfo())
			{
				return zipStorer;
			}
		}
		throw new InvalidDataException();
	}

	public void AddFile(Compression _method, string _pathname, string _filenameInZip, string _comment)
	{
		if (Access == FileAccess.Read)
		{
			throw new InvalidOperationException("Writing is not alowed");
		}
		FileStream fileStream = new FileStream(_pathname, FileMode.Open, FileAccess.Read);
		AddStream(_method, _filenameInZip, fileStream, File.GetLastWriteTime(_pathname), _comment);
		fileStream.Close();
	}

	public void AddStream(Compression _method, string _filenameInZip, Stream _source, DateTime _modTime, string _comment)
	{
		if (Access == FileAccess.Read || _source == null || string.IsNullOrEmpty(_filenameInZip))
		{
			throw new InvalidOperationException("Writing is not alowed");
		}
		ZipFileEntry _zfe = new ZipFileEntry
		{
			Method = _method,
			EncodeUTF8 = EncodeUTF8,
			FilenameInZip = NormalizedFilename(_filenameInZip),
			Comment = (_comment ?? ""),
			Crc32 = 0u,
			HeaderOffset = (uint)ZipFileStream.Position,
			ModifyTime = _modTime
		};
		WriteLocalHeader(ref _zfe);
		_zfe.FileOffset = (uint)ZipFileStream.Position;
		Store(ref _zfe, _source);
		_source.Close();
		UpdateCrcAndSizes(ref _zfe);
		Files.Add(_zfe);
	}

	public void Close()
	{
		if (Access != FileAccess.Read)
		{
			uint offset = (uint)ZipFileStream.Position;
			uint num = 0u;
			if (CentralDirImage != null)
			{
				ZipFileStream.Write(CentralDirImage, 0, CentralDirImage.Length);
			}
			for (int i = 0; i < Files.Count; i++)
			{
				long position = ZipFileStream.Position;
				WriteCentralDirRecord(Files[i]);
				num += (uint)(int)(ZipFileStream.Position - position);
			}
			if (CentralDirImage != null)
			{
				WriteEndRecord(num + (uint)CentralDirImage.Length, offset);
			}
			else
			{
				WriteEndRecord(num, offset);
			}
		}
		if (ZipFileStream != null)
		{
			ZipFileStream.Flush();
			ZipFileStream.Dispose();
			ZipFileStream = null;
		}
	}

	public List<ZipFileEntry> ReadCentralDir()
	{
		if (CentralDirImage == null)
		{
			throw new InvalidOperationException("Central directory currently does not exist");
		}
		List<ZipFileEntry> list = new List<ZipFileEntry>();
		ushort num4;
		ushort num5;
		ushort num6;
		for (int i = 0; i < CentralDirImage.Length && BitConverter.ToUInt32(CentralDirImage, i) == unchecked(-2113844400 + (523611131 << 2139454111)); i += -916178343 - ~916178388 + num4 + num5 + num6)
		{
			ushort num = BitConverter.ToUInt16(CentralDirImage, i + (-436207608 - (330503539 << 1883220601)));
			int num2 = ((365805270 > 413056551) ? 2730 : 2048);
			bool num3 = (num & num2) != 0;
			ushort method = BitConverter.ToUInt16(CentralDirImage, i + (-2147483638 - (807676242 << 1108208638)));
			uint crc = BitConverter.ToUInt32(CentralDirImage, i + (671088656 + (295583707 << 729461275)));
			uint compressedSize = BitConverter.ToUInt32(CentralDirImage, i + (-1482979014 - ~1482979033));
			uint fileSize = BitConverter.ToUInt32(CentralDirImage, i + (-1263223332 ^ -1263223356));
			num4 = BitConverter.ToUInt16(CentralDirImage, i + (438049180 - (1681143859 << 430610951)));
			num5 = BitConverter.ToUInt16(CentralDirImage, i + (-1087845209 + 1087845239 % 1928872814));
			num6 = BitConverter.ToUInt16(CentralDirImage, i + (1300302097 + ~1300302064));
			uint headerOffset = BitConverter.ToUInt32(CentralDirImage, i + (-1886232304 ^ -1886232262));
			int num7 = ((214184893 > 1354190696) ? 61 : 46);
			uint headerSize = (uint)(num7 + num4 + num5 + num6);
			Encoding encoding = (num3 ? Encoding.UTF8 : DefaultEncoding);
			ZipFileEntry zipFileEntry = new ZipFileEntry
			{
				Method = (Compression)method
			};
			byte[] centralDirImage = CentralDirImage;
			int num8 = i;
			int num9 = ((2024420782 > 2140030486) ? 61 : 46);
			zipFileEntry.FilenameInZip = encoding.GetString(centralDirImage, num8 + num9, num4);
			zipFileEntry.FileOffset = GetFileOffset(headerOffset);
			zipFileEntry.FileSize = fileSize;
			zipFileEntry.CompressedSize = compressedSize;
			zipFileEntry.HeaderOffset = headerOffset;
			zipFileEntry.HeaderSize = headerSize;
			zipFileEntry.Crc32 = crc;
			zipFileEntry.ModifyTime = DateTime.Now;
			ZipFileEntry item = zipFileEntry;
			if (num6 > 0)
			{
				item.Comment = encoding.GetString(CentralDirImage, i + (-1383052743 - ~1383052788) + num4 + num5, num6);
			}
			list.Add(item);
		}
		return list;
	}

	public bool ExtractFile(ZipFileEntry _zfe, string _filename)
	{
		string directoryName = Path.GetDirectoryName(_filename);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (Directory.Exists(_filename))
		{
			return true;
		}
		bool result;
		using (Stream stream = new FileStream(_filename, FileMode.Create, FileAccess.Write))
		{
			result = ExtractFile(_zfe, stream);
		}
		File.SetCreationTime(_filename, _zfe.ModifyTime);
		File.SetLastWriteTime(_filename, _zfe.ModifyTime);
		return result;
	}

	public bool ExtractFile(ZipFileEntry _zfe, Stream _stream)
	{
		if (_stream == null || !_stream.CanWrite)
		{
			throw new InvalidOperationException("Stream cannot be written");
		}
		byte[] array = new byte[304163762 - 1098250046 % 397043144];
		ZipFileStream.Seek(_zfe.HeaderOffset, SeekOrigin.Begin);
		ZipFileStream.Read(array, 0, 0x6E012F3 ^ 0x6E012F7);
		if (BitConverter.ToUInt32(array, 0) != (0x5261379 ^ 0x1255829))
		{
			return false;
		}
		Stream stream;
		if (_zfe.Method == Compression.Store)
		{
			stream = ZipFileStream;
		}
		else
		{
			if (_zfe.Method != Compression.Deflate)
			{
				return false;
			}
			stream = new DeflateStream(ZipFileStream, CompressionMode.Decompress, leaveOpen: true);
		}
		byte[] array2 = new byte[-2061398214 - ~2061414597];
		ZipFileStream.Seek(_zfe.FileOffset, SeekOrigin.Begin);
		uint num = _zfe.FileSize;
		while (num != 0)
		{
			int num2 = stream.Read(array2, 0, (int)Math.Min(num, array2.Length));
			_stream.Write(array2, 0, num2);
			num -= (uint)num2;
		}
		_stream.Flush();
		if (_zfe.Method == Compression.Deflate)
		{
			stream.Dispose();
		}
		return true;
	}

	public static bool RemoveEntries(ref ZipStorer _zip, List<ZipFileEntry> _zfes)
	{
		if (_zip == null || !(_zip.ZipFileStream is FileStream))
		{
			throw new InvalidOperationException("RemoveEntries is allowed just over streams of type FileStream");
		}
		List<ZipFileEntry> list = _zip.ReadCentralDir();
		string tempFileName = Path.GetTempFileName();
		string tempFileName2 = Path.GetTempFileName();
		try
		{
			ZipStorer zipStorer = Create(tempFileName, string.Empty);
			foreach (ZipFileEntry item in list)
			{
				if (_zfes != null && !_zfes.Contains(item) && _zip.ExtractFile(item, tempFileName2))
				{
					zipStorer.AddFile(item.Method, tempFileName2, item.FilenameInZip, item.Comment);
				}
			}
			_zip.Close();
			zipStorer.Close();
			File.Delete(_zip.FileName);
			File.Move(tempFileName, _zip.FileName);
			_zip = Open(_zip.FileName, _zip.Access);
		}
		catch
		{
			return false;
		}
		finally
		{
			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}
			if (File.Exists(tempFileName2))
			{
				File.Delete(tempFileName2);
			}
		}
		return true;
	}

	private uint GetFileOffset(uint _headerOffset)
	{
		byte[] array = new byte[1132770862 - 1132770860 % 1501045637];
		ZipFileStream.Seek((uint)((int)_headerOffset + (1714103602 + -1714103576)), SeekOrigin.Begin);
		ZipFileStream.Read(array, 0, 0x3BD3EA78 ^ 0x3BD3EA7A);
		ushort num = BitConverter.ToUInt16(array, 0);
		ZipFileStream.Read(array, 0, 933494786 - (1077779410 << 2109162001));
		ushort num2 = BitConverter.ToUInt16(array, 0);
		int num3 = ((44116850 > 1952797572) ? 40 : 30);
		return (uint)(num3 + num + num2 + _headerOffset);
	}

	private void WriteLocalHeader(ref ZipFileEntry _zfe)
	{
		long position = ZipFileStream.Position;
		byte[] bytes = (_zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding).GetBytes(_zfe.FilenameInZip);
		Stream zipFileStream = ZipFileStream;
		byte[] array = new byte[1278129666 + -1278129660];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		zipFileStream.Write(array, 0, -1935317722 - ~1935317727);
		Stream zipFileStream2 = ZipFileStream;
		int num;
		if (!_zfe.EncodeUTF8)
		{
			num = 0;
		}
		else
		{
			int num2 = ((1449828196 > 1751963982) ? 2730 : 2048);
			num = num2;
		}
		zipFileStream2.Write(BitConverter.GetBytes((ushort)num), 0, 3 - (1419491324 >> 1664502078));
		Stream zipFileStream3 = ZipFileStream;
		byte[] bytes2 = BitConverter.GetBytes((ushort)_zfe.Method);
		int count = ((1947274472 > 1613644200) ? 2 : 2);
		zipFileStream3.Write(bytes2, 0, count);
		ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, -253491447 + 1779865987 % 1526374536);
		ZipFileStream.Write(new byte[1071610683 - (0x3CDB1A2D | 0x2B8C7106)], 0, -1165003671 + 1165003683 % 1856096596);
		Stream zipFileStream4 = ZipFileStream;
		byte[] bytes3 = BitConverter.GetBytes((ushort)bytes.Length);
		int count2 = ((91183075 > 1798540145) ? 2 : 2);
		zipFileStream4.Write(bytes3, 0, count2);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 1862110621 + -1862110619);
		ZipFileStream.Write(bytes, 0, bytes.Length);
		_zfe.HeaderSize = (uint)(ZipFileStream.Position - position);
	}

	private void WriteCentralDirRecord(ZipFileEntry _zfe)
	{
		Encoding obj = (_zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding);
		byte[] bytes = obj.GetBytes(_zfe.FilenameInZip);
		byte[] bytes2 = obj.GetBytes(_zfe.Comment);
		Stream zipFileStream = ZipFileStream;
		byte[] array = new byte[770179080 - (1360127421 << 894028307)];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		int count = ((1600932511 > 288839465) ? 8 : 10);
		zipFileStream.Write(array, 0, count);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? (-570423296 ^ -570425344) : 0u)), 0, -922197626 + (0x16279E78 | 0x32F40664));
		ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 1609154302 - (0x5DC9BEB8 | 0x12A01C6C));
		ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 0x19233D41 ^ 0x19233D45);
		Stream zipFileStream2 = ZipFileStream;
		byte[] bytes3 = BitConverter.GetBytes(_zfe.Crc32);
		int count2 = ((596093853 > 1651545433) ? 5 : 4);
		zipFileStream2.Write(bytes3, 0, count2);
		ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 1981401826 + -1981401822);
		ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 1898137206 + -1898137202);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 1016113959 - 1016113957 % 1872075479);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 0x6D7CE402 ^ 0x6D7CE400);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 1233022242 + ~1233022239);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, -917003151 + (1834006306 >> 2019340129));
		ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, -2108027401 ^ -2108027403);
		Stream zipFileStream3 = ZipFileStream;
		byte[] bytes4 = BitConverter.GetBytes((ushort)0);
		int count3 = ((571257515 > 637227520) ? 2 : 2);
		zipFileStream3.Write(bytes4, 0, count3);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)33024), 0, 0x18F463E2 ^ 0x18F463E0);
		ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, -855638012 - (1978466714 << 1014528535));
		ZipFileStream.Write(bytes, 0, bytes.Length);
		ZipFileStream.Write(bytes2, 0, bytes2.Length);
	}

	private void WriteEndRecord(uint _size, uint _offset)
	{
		byte[] bytes = (EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding).GetBytes(Comment);
		Stream zipFileStream = ZipFileStream;
		byte[] array = new byte[-1336482541 - -1336482549];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		zipFileStream.Write(array, 0, -2076142259 + (0x22AB6028 | 0x791C6A9B));
		ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 0xB0 ^ 0xB2);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 2058795658 + ~2058795655);
		ZipFileStream.Write(BitConverter.GetBytes(_size), 0, -2130166015 ^ -2130166011);
		ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 0x43420724 ^ 0x43420720);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, -1103670961 - -1103670963);
		ZipFileStream.Write(bytes, 0, bytes.Length);
	}

	private void Store(ref ZipFileEntry _zfe, Stream _source)
	{
		int num = ((2071497772 > 1227098798) ? 16384 : 21845);
		byte[] array = new byte[num];
		uint num2 = 0u;
		Stream stream = null;
		long position = ZipFileStream.Position;
		long position2 = _source.Position;
		if (_zfe.Method == Compression.Store)
		{
			stream = ZipFileStream;
		}
		else if (_zfe.Method == Compression.Deflate)
		{
			stream = new DeflateStream(ZipFileStream, CompressionMode.Compress, leaveOpen: true);
		}
		_zfe.Crc32 = uint.MaxValue;
		int num3;
		do
		{
			num3 = _source.Read(array, 0, array.Length);
			num2 += (uint)num3;
			if (num3 > 0)
			{
				stream?.Write(array, 0, num3);
				for (uint num4 = 0u; num4 < num3; num4++)
				{
					_zfe.Crc32 = CrcTable[(_zfe.Crc32 ^ array[num4]) & 0xFF] ^ (_zfe.Crc32 >> (0x435 ^ 0x43D));
				}
			}
		}
		while (num3 == array.Length);
		stream.Flush();
		if (_zfe.Method == Compression.Deflate)
		{
			stream.Dispose();
		}
		_zfe.Crc32 ^= uint.MaxValue;
		_zfe.FileSize = num2;
		_zfe.CompressedSize = (uint)(ZipFileStream.Position - position);
		if (_zfe.Method == Compression.Deflate && !ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
		{
			_zfe.Method = Compression.Store;
			ZipFileStream.Position = position;
			ZipFileStream.SetLength(position);
			_source.Position = position2;
			Store(ref _zfe, _source);
		}
	}

	private static uint DateTimeToDosTime(DateTime _dt)
	{
		int num = (_dt.Second / (2036738880 + -2036738878)) | (_dt.Minute << (-1656942071 ^ -1656942068)) | (_dt.Hour << (0x32F4CC6C ^ 0x32F4CC67)) | (_dt.Day << 263 - (1040172811 >> 480692150));
		int month = _dt.Month;
		int num2 = ((512189504 > 1608820422) ? 28 : 21);
		return (uint)(num | (month << num2) | (_dt.Year - (-804972063 + (0x3FAAD03 | 0x2D5AE5D8)) << 104085467 + -104085442));
	}

	private void UpdateCrcAndSizes(ref ZipFileEntry _zfe)
	{
		long position = ZipFileStream.Position;
		ZipFileStream.Position = _zfe.HeaderOffset + (-1965354421 + 1965354429 % 2077199714);
		ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, -782589999 - -782590001);
		ZipFileStream.Position = _zfe.HeaderOffset + (-2063007087 + (0x2AB29D7C | 0x58E6692D));
		ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, -130110598 + 130110602 % 2058478179);
		ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 2 + (315605093 >> 1252138587));
		Stream zipFileStream = ZipFileStream;
		byte[] bytes = BitConverter.GetBytes(_zfe.FileSize);
		int count = ((1417683746 > 1990398353) ? 5 : 4);
		zipFileStream.Write(bytes, 0, count);
		ZipFileStream.Position = position;
	}

	private static string NormalizedFilename(string _filename)
	{
		int oldChar = ((922799006 > 1531634135) ? 122 : 92);
		int newChar = ((689417276 > 764295916) ? 62 : 47);
		string text = _filename.Replace((char)oldChar, (char)newChar);
		int num = text.IndexOf(':');
		if (num >= 0)
		{
			text = text.Remove(0, num + 1);
		}
		return text.Trim(new char[1] { '/' });
	}

	private bool ReadFileInfo()
	{
		if (ZipFileStream.Length < 22)
		{
			return false;
		}
		try
		{
			ZipFileStream.Seek(-17L, SeekOrigin.End);
			using BinaryReader binaryReader = new BinaryReader(ZipFileStream);
			do
			{
				ZipFileStream.Seek(-5L, SeekOrigin.Current);
				if (binaryReader.ReadUInt32() == 101010256)
				{
					ZipFileStream.Seek(6L, SeekOrigin.Current);
					ushort existingFiles = binaryReader.ReadUInt16();
					int num = binaryReader.ReadInt32();
					uint num2 = binaryReader.ReadUInt32();
					ushort num3 = binaryReader.ReadUInt16();
					if (ZipFileStream.Position + num3 != ZipFileStream.Length)
					{
						return false;
					}
					ExistingFiles = existingFiles;
					CentralDirImage = new byte[num];
					ZipFileStream.Seek(num2, SeekOrigin.Begin);
					ZipFileStream.Read(CentralDirImage, 0, num);
					ZipFileStream.Seek(num2, SeekOrigin.Begin);
					return true;
				}
			}
			while (ZipFileStream.Position > 0);
		}
		catch
		{
		}
		return false;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			Close();
			disposedValue = true;
		}
	}

	~ZipStorer()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
