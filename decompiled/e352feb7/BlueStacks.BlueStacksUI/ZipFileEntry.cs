using System;

namespace BlueStacks.BlueStacksUI;

public struct ZipFileEntry : IEquatable<ZipFileEntry>
{
	public Compression Method { get; set; }

	public string FilenameInZip { get; set; }

	public uint FileSize { get; set; }

	public uint CompressedSize { get; set; }

	public uint HeaderOffset { get; set; }

	public uint FileOffset { get; set; }

	public uint HeaderSize { get; set; }

	public uint Crc32 { get; set; }

	public DateTime ModifyTime { get; set; }

	public string Comment { get; set; }

	public bool EncodeUTF8 { get; set; }

	public override bool Equals(object obj)
	{
		if (obj is ZipFileEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(ZipFileEntry other)
	{
		if (Method == other.Method && FilenameInZip == other.FilenameInZip && FileSize == other.FileSize && CompressedSize == other.CompressedSize && HeaderOffset == other.HeaderOffset && FileOffset == other.FileOffset && HeaderSize == other.HeaderSize && Crc32 == other.Crc32 && ModifyTime == other.ModifyTime && Comment == other.Comment)
		{
			return EncodeUTF8 == other.EncodeUTF8;
		}
		return false;
	}

	public override string ToString()
	{
		return FilenameInZip;
	}

	public static bool operator ==(ZipFileEntry left, ZipFileEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ZipFileEntry left, ZipFileEntry right)
	{
		return !(left == right);
	}

	public override int GetHashCode()
	{
		return Method.GetHashCode() ^ FilenameInZip.GetHashCode() ^ FileSize.GetHashCode() ^ CompressedSize.GetHashCode() ^ HeaderOffset.GetHashCode() ^ FileOffset.GetHashCode() ^ HeaderSize.GetHashCode() ^ Crc32.GetHashCode() ^ ModifyTime.GetHashCode() ^ Comment.GetHashCode() ^ EncodeUTF8.GetHashCode();
	}
}
