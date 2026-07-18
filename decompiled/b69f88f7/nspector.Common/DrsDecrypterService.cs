using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using nspector.Common.Helper;
using nspector.Native.NVAPI2;

namespace nspector.Common;

internal class DrsDecrypterService : DrsSettingsServiceBase
{
	private static readonly byte[] _InternalSettingsKey = new byte[256]
	{
		47, 124, 79, 139, 32, 36, 82, 141, 38, 60,
		148, 119, 243, 124, 152, 165, 250, 113, 182, 128,
		221, 53, 132, 186, 253, 182, 166, 27, 57, 196,
		204, 176, 126, 149, 217, 238, 24, 75, 156, 245,
		45, 78, 208, 193, 85, 23, 223, 24, 30, 11,
		24, 139, 136, 88, 134, 90, 30, 3, 237, 86,
		251, 22, 254, 138, 1, 50, 156, 141, 242, 232,
		74, 230, 144, 142, 21, 104, 232, 45, 244, 64,
		55, 154, 114, 199, 2, 12, 209, 211, 88, 234,
		98, 209, 152, 54, 43, 178, 22, 213, 222, 147,
		241, 186, 116, 227, 50, 196, 159, 246, 18, 254,
		24, 192, 187, 53, 121, 156, 107, 122, 35, 127,
		43, 21, 155, 66, 7, 26, 255, 105, 251, 156,
		189, 35, 151, 168, 34, 99, 143, 50, 200, 233,
		155, 99, 28, 238, 44, 217, 237, 141, 58, 53,
		156, 177, 96, 174, 94, 245, 151, 107, 159, 32,
		140, 247, 152, 44, 67, 121, 149, 29, 205, 70,
		54, 108, 217, 103, 32, 171, 65, 34, 33, 229,
		85, 130, 245, 39, 32, 245, 8, 7, 63, 109,
		105, 217, 28, 75, 248, 38, 3, 110, 178, 63,
		30, 230, 202, 61, 97, 68, 176, 146, 175, 240,
		136, 202, 224, 95, 93, 244, 223, 198, 76, 164,
		224, 202, 176, 32, 93, 192, 250, 221, 154, 52,
		143, 80, 121, 90, 95, 124, 25, 158, 64, 112,
		113, 181, 69, 25, 184, 83, 252, 223, 36, 190,
		34, 28, 121, 191, 66, 137
	};

	private HashSet<string> _InternalSettings = new HashSet<string>();

	public DrsDecrypterService(DrsSettingsMetaService metaService)
		: base(metaService)
	{
		try
		{
			CreateInternalSettingMap();
		}
		catch
		{
		}
	}

	private uint GetDwordFromKey(uint offset)
	{
		return BitConverter.ToUInt32(new byte[4]
		{
			_InternalSettingsKey[offset % 256],
			_InternalSettingsKey[(offset + 1) % 256],
			_InternalSettingsKey[(offset + 2) % 256],
			_InternalSettingsKey[(offset + 3) % 256]
		}, 0);
	}

	public uint DecryptDwordValue(uint orgValue, uint settingId)
	{
		uint offset = settingId << 1;
		uint dwordFromKey = GetDwordFromKey(offset);
		return orgValue ^ dwordFromKey;
	}

	public string DecryptStringValue(byte[] rawData, uint settingId)
	{
		uint num = settingId << 1;
		for (uint num2 = 0u; num2 < (uint)rawData.Length; num2++)
		{
			rawData[num2] ^= _InternalSettingsKey[(num + num2) % 256];
		}
		return Encoding.Unicode.GetString(rawData).Trim(new char[1]);
	}

	public void DecryptSettingIfNeeded(string profileName, ref NVDRS_SETTING setting)
	{
		if (setting.isPredefinedValid != 1 || !IsInternalSetting(profileName, setting.settingId))
		{
			return;
		}
		if (setting.settingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
		{
			setting.predefinedValue.stringValue = DecryptStringValue(setting.predefinedValue.rawData, setting.settingId);
			if (setting.isCurrentPredefined == 1)
			{
				setting.currentValue.stringValue = DecryptStringValue(setting.currentValue.rawData, setting.settingId);
			}
		}
		else if (setting.settingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
		{
			setting.predefinedValue.dwordValue = DecryptDwordValue(setting.predefinedValue.dwordValue, setting.settingId);
			if (setting.isCurrentPredefined == 1)
			{
				setting.currentValue.dwordValue = DecryptDwordValue(setting.currentValue.dwordValue, setting.settingId);
			}
		}
	}

	private string FormatInternalSettingKey(string profileName, uint settingId)
	{
		return profileName + settingId.ToString("X8").ToLowerInvariant();
	}

	public bool IsInternalSetting(string profileName, uint settingId)
	{
		return _InternalSettings.Contains(FormatInternalSettingKey(profileName, settingId));
	}

	private void CreateInternalSettingMap()
	{
		string tmpfile = TempFile.GetTempFileName();
		try
		{
			DrsSession(delegate(IntPtr hSession)
			{
				SaveSettingsFileEx(hSession, tmpfile);
			});
			if (!File.Exists(tmpfile))
			{
				return;
			}
			string[] array = File.ReadAllLines(tmpfile);
			_InternalSettings = new HashSet<string>();
			string pattern = "Profile\\s\\\"(?<profileName>.*?)\\\"";
			Regex regex = new Regex(pattern, RegexOptions.Compiled);
			string pattern2 = "ID_0x(?<sid>[0-9a-fA-F]+)\\s\\=.*?InternalSettingFlag\\=V0";
			Regex regex2 = new Regex(pattern2, RegexOptions.Compiled);
			string text = "";
			for (int num = 0; num < array.Length; num++)
			{
				foreach (Match item in regex.Matches(array[num]))
				{
					text = item.Result("${profileName}");
				}
				foreach (Match item2 in regex2.Matches(array[num]))
				{
					_InternalSettings.Add(text + item2.Result("${sid}"));
				}
			}
		}
		finally
		{
			if (File.Exists(tmpfile))
			{
				File.Delete(tmpfile);
			}
		}
	}
}
