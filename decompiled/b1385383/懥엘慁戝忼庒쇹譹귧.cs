using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[닫企퍞俾칋諥慬탛と뼖듯唰叿럊횔퍥]
internal static class 懥엘慁戝忼庒쇹譹귧
{
	private const uint 쨫껫瞟径崸侀協뎶 = 1263817577u;

	private const uint 겖둹敵鼊攣騃ج矤퉓櫆馮끱 = 1263817572u;

	internal static void 묉饂偔邴澝졀葫狧羲譫莑쇩(out byte[] reversePerm, out byte[] masterKey, out int totalChunks, out int[] methChunkId, out int[] methOffset, out int[] methLength, out byte[][] encryptedByLogical, out byte[] crcExpected, out byte protectionFlags, out byte[] vmCrcExpected)
	{
		string text = Assembly.GetExecutingAssembly().Location;
		if (string.IsNullOrEmpty(text))
		{
			text = Process.GetCurrentProcess().MainModule.FileName;
		}
		byte[] array = File.ReadAllBytes(text);
		int num = BitConverter.ToInt32(array, 0x7DC9DBFC ^ 0x7DC9DBC0) - -(0x31A7C4D2 ^ (0x31A7DF49 ^ (~-7072 + 1 + -1)));
		int num2 = BitConverter.ToUInt16(array, num - -((((~-2044705506 + 1 + -1) ^ ((0x5826CDE1 ^ 0x5826EAAE) + -236)) + -(0x320C ^ 0x2010)) ^ 0x79DF8A64));
		int num3 = BitConverter.ToUInt16(array, num - -((~-1768086246 + 1 + -1 + -355) ^ ((-8194 ^ -1) + -571) ^ 0x6962C454));
		int num4 = num - -(0x5F99B928 ^ ((((1603910472 + -1876) & -1) + -152) ^ 0xA60)) - -num3;
		byte[] array2 = null;
		int num5 = ((-765852261 ^ (2215 + -(0x4F42 ^ 0x4FF8))) + -(0x84FF ^ 0x20DB)) ^ (0x2DA6BDDD ^ (7563 + -283));
		int num6 = 0x27DE0D46 ^ (0x27DE1039 ^ ((23627 + -16076) & -1));
		byte[][] array3 = new byte[num2][];
		for (int num7 = ((0x2659E0A6 ^ 0x853) + -(0xF05B ^ (~-4443 + 1 + -1))) ^ (0x265902A5 ^ (~-1522 + 1 + -1 + -160)); num7 < num2; num7 -= -(0x2FC1E750 ^ 0x2FC1E751))
		{
			int num8 = num4 - -(num7 * (0x3215F85 ^ (17237 + -6660 + -4150) ^ (0x3214867 ^ (25140 + -20649 + -698))));
			uint num9 = BitConverter.ToUInt32(array, num8 - -((((1559268515 + -126) ^ (7186 + -338)) + -(0xD850 ^ ((-1766 ^ -1) + -(-188 ^ -1)))) ^ 0x5CEFB87F));
			uint num10 = BitConverter.ToUInt32(array, num8 - -(0x68B8C6A ^ (((0x68BB706 ^ 0x326D) + -678) ^ (~-4287 + 1 + -1 + -511))));
			if (num9 != 0 && num10 >= (uint)(((0x4EC93070 ^ 0x84E) + -((55867 + -86) ^ (9268 + -325))) ^ 0x4EC83D38))
			{
				uint num11 = BitConverter.ToUInt32(array, (int)num9);
				if (num11 == (uint)(((0x686262D4 ^ (3019 + -(~-946 + 1 + -1))) + -(0x7DB1 ^ 0x16D2)) ^ 0x2335AC02) || num11 == (0x3990040E ^ 0x72C4576A))
				{
					int num12 = BitConverter.ToInt32(array, (int)num9 - -(0x62CA8F9 ^ (0x13A8AD96 ^ ((0x5724AF62 ^ 0x5724BB21) + -605)) ^ (((0x77A9369F ^ 0x622D035A) + -37) ^ 0x212D)));
					int num13 = BitConverter.ToInt32(array, (int)num9 - -((806475327 + -152) ^ 0xAB1 ^ 0x3011DF1E));
					int num14 = (int)num9 - -((-158928736 + -14924 + -21145) ^ 0x2685 ^ -1 ^ ((158971549 + -318) ^ 0xF92));
					if (num12 > ((147535988 + -949) ^ (49209 + -29877 + -17242) ^ (0x8CB22DC ^ (8120 + -(-368 ^ -1)))) && num14 - -num12 <= array.Length)
					{
						byte[] array4 = new byte[num12];
						Buffer.BlockCopy(array, num14, array4, (621152982 + -(0x432485EF ^ 0x432484D3)) ^ (((25228 + -23115) & -1) + -973) ^ 0x6943E5A5 ^ ((~-1279653734 + 1 + -1 + -39) ^ (8247 + -194)), num12);
						if (num11 == (uint)(0x6A56178D ^ (2666 + -290) ^ (0x4C17404B ^ (66334 + -30026 + -30238)) ^ (0x6D15058E ^ (~-9140 + 1 + -1 + -980))))
						{
							array2 = array4;
							num5 = (int)num9;
							num6 = num12;
						}
						else if (num13 >= ((-304673322 + -(-108 ^ -1)) ^ ((0x126F48B1 ^ 0x126F694A) + -(0x5C44B1DA ^ 0x5C44B0F4)) ^ -1 ^ (0x1228F735 ^ (9843 + -263))) && num13 < array3.Length)
						{
							array3[num13] = array4;
						}
					}
				}
			}
		}
		vmCrcExpected = new byte[(((-1250032734 + -25056) & -1) + -(0x52BC ^ 0x510E)) ^ 0xC88 ^ -1 ^ 0x4A826D47];
		if (num5 >= ((~-1322515824 + 1 + -1) ^ 0x8E3 ^ ((1452937088 + -(25538 + -18069 + -7405)) ^ 0x1D0C) ^ (((0x1849C477 ^ 0x30FD) + -925) ^ 0x172D)))
		{
			int num15 = num5 - -(((0x8360B91 ^ (8001 + -949)) + -(0x6F21 ^ 0x1BAA)) ^ 0x8359B9E) - -num6;
			if (num15 - -((~-121103130 + 1 + -1) ^ 0x20CF ^ ((121128764 + -22604 + -4163) ^ 0x1D5B)) <= array.Length)
			{
				Buffer.BlockCopy(array, num15, vmCrcExpected, 0x673B8718 ^ (4083 + -880) ^ 0x29E83E37 ^ (0x4ED3BEEB ^ (3185 + -(~-299 + 1 + -1))), ((0x5F4E80C5 ^ 0xFFF) + -((42572 + -248) ^ 0x6B7)) ^ 0x5F4DEB77);
			}
		}
		if (array2 == null)
		{
			throw new Exception(鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("wBXmQPCxWkp9n2woHCDFFAvzfW1GbxHS4g4UMqJEC7bGe6asyJVtWHlqWjBN3qUlv4iBKcqGZkVuBNqQyiLoOAgOWtakwzttYRg4lT8u8WvvGW+uMsr50QgPOkPEmU5QAxAY/286UmUASbvjJdza0iw="));
		}
		byte[] array5 = 說翦堞浚욼奍쥞犒湸쪦旄쪖뭱뭧霾덆粵瞚.舕絠閾卸権醲嘈甮즱諧(array2, 說翦堞浚욼奍쥞犒湸쪦旄쪖뭱뭧霾덆粵瞚.get_BootstrapKey(), (1535888438 + -4247 + -13906) ^ (4183 + -(-216 ^ -1)) ^ ((1535870384 + -(56748 + -32474 + -24093)) ^ 0xE36));
		int num16 = (1081040886 + -424) ^ 0xA91 ^ (0x406F4837 ^ ((0x2B819C0 ^ 0x2B839EE) + -(0x41713383 ^ 0x417130C5)));
		totalChunks = BitConverter.ToInt32(array5, num16);
		num16 -= -(0x714C3A6 ^ (6612 + -(~-47 + 1 + -1)) ^ 0x79A78BE8 ^ 0x7EB351EC);
		byte[] array6 = new byte[(~1090342516 + 1 + -1) ^ 0xEF1 ^ -1 ^ ((1090347674 + -(0xF1FC9 ^ 0xF1E1F)) ^ ((-8857 ^ -1) + -343))];
		Buffer.BlockCopy(array5, num16, array6, 0x71AF8586 ^ (0x3A45B277 ^ (5927 + -206)) ^ (0x4BEA07BD ^ (10249 + -500)), ((0x7B36D210 ^ 0x6B46B3F0) + -425) ^ (4654 + -782) ^ (0x10706618 ^ (2721 + -(~-659 + 1 + -1))));
		num16 -= -(0x110CD8A9 ^ ((((286073395 + -24114) & -1) + -549) ^ 0x1875));
		reversePerm = 說翦堞浚욼奍쥞犒湸쪦旄쪖뭱뭧霾덆粵瞚.蹣콬券葪単쮾쁨쟴괥귑螔闣굴诋游얉酙퀨돲밍(array6);
		masterKey = new byte[0x2CEC587 ^ ((0x673A0B46 ^ 0x673A1B13) + -119) ^ (0x78B00A95 ^ (2274 + -(0x2AAEF9D3 ^ 0x2AAEFA4A))) ^ 0x7A7EC5A5];
		Buffer.BlockCopy(array5, num16, masterKey, 0x3D0EAA23 ^ ((1024374046 + -852) ^ 0x1BE9), ((((826882404 + -15170) & -1) ^ (3860 + -301)) + -(0x598C ^ 0x1634)) ^ ((826851406 + -(-348 ^ -1)) ^ 0x1ADE));
		num16 -= -(0x69D3143E ^ 0x69D3141E);
		encryptedByLogical = new byte[totalChunks][];
		for (int num17 = ((0x57068C07 ^ 0x11DF) + -(0x1D1E ^ 0x15AE)) ^ ((1460052018 + -920) ^ 0x9B2); num17 < totalChunks; num17 -= -(-2097141463 ^ (25708 + -13385 + -7132) ^ -1 ^ 0x7CFFC290))
		{
			int num18 = BitConverter.ToInt32(array5, num16);
			num16 -= -(0x5FFC82C0 ^ (0x5FFC9199 ^ (5875 + -918)));
			if (num18 >= (0x7A2583CC ^ 0x7A2583CC) && num18 < array3.Length)
			{
				encryptedByLogical[num17] = array3[num18];
			}
		}
		int num19 = BitConverter.ToInt32(array5, num16);
		num16 -= -(0x41E85F45 ^ 0x41E85F41);
		methChunkId = new int[num19];
		methOffset = new int[num19];
		methLength = new int[num19];
		for (int num20 = ((0x4D378EA3 ^ 0xC96) + -((40149 + -510) ^ 0xD49)) ^ 0x4D36EA97; num20 < num19; num20 -= -(((0x332655C9 ^ 0x1524) + -(0x718C ^ 0x91B)) ^ (0x3325D4F3 ^ ((-7477 ^ -1) + -144))))
		{
			methChunkId[num20] = BitConverter.ToInt32(array5, num16);
			num16 -= -(0x159CD0C3 ^ ((362640643 + -2158 + -30990 + -363) ^ (~-9124 + 1 + -1 + -200)));
		}
		for (int num21 = (1924021628 + -(47978 + -27621 + -19974)) ^ 0x139F ^ ((255207134 + -686) ^ 0x1CA7) ^ (0x7D980213 ^ ((0x3B9A ^ 0x2F39) + -((2179 + -1734) & -1))); num21 < num19; num21 -= -(((((0x35413531 ^ 0x24D4DE8) + -918) ^ (((21262 + -18771) & -1) + -((11185 + -10682) & -1))) + -((22054 + -355) ^ (10627 + -914))) ^ 0x370C0154))
		{
			methOffset[num21] = BitConverter.ToInt32(array5, num16);
			num16 -= -(0x20ADC984 ^ (18442 + -8078 + -2770 + -653) ^ ((1989097616 + -26019 + -28773) ^ 0x15DA) ^ ((1445179158 + -749) ^ 0x9E6));
		}
		for (int num22 = 0x5E515E00 ^ 0x5E515E00; num22 < num19; num22 -= -((178773368 + -(0x35C1A0CF ^ 0x35C1A363)) ^ (5425 + -824) ^ (((0x29047673 ^ 0x26D01111) + -(-954 ^ -1)) ^ ((0x12B6 ^ 0x430) + -(0x3D7218FD ^ 0x3D721ADE))) ^ (0x573B77B ^ (2891 + -710))))
		{
			methLength[num22] = BitConverter.ToInt32(array5, num16);
			num16 -= -(0x1893115B ^ 0x1893115F);
		}
		crcExpected = new byte[((((0x2C931CC1 ^ 0x10365B88) + -381) ^ 0x2053) + -(((0x73A00079 ^ 0x73A0302C) + -51) ^ 0x1B76)) ^ (0x3CA53082 ^ (64589 + -29909 + -31887))];
		if (num16 - -(0x5784022 ^ ((~-91776137 + 1 + -1 + -(0x280F904C ^ 0x280F914E)) ^ (9340 + -(~-249 + 1 + -1)))) <= array5.Length)
		{
			Buffer.BlockCopy(array5, num16, crcExpected, 0x3790A49B ^ 0x3790A49B, 0x688FEB1 ^ (((0x127B8F80 ^ 0x14F35060) + -397) ^ (9338 + -(0x736BE90B ^ 0x736BEAB3))));
			num16 -= -(((0x3A21E53 ^ 0x5F5) + -((18622 + -875) ^ ((0x7D8831F6 ^ 0x7D8810C4) + -(~-368 + 1 + -1)))) ^ 0x3A1C136);
		}
		protectionFlags = (byte)((num16 < array5.Length) ? array5[num16] : ((-1285740115 + -213) ^ 0x89F ^ -1 ^ 0x4CA2DF47));
	}

	private static bool 葬푋豬턒梼謳益粱鄊휸()
	{
		int num = 10;
		do
		{
			num += 4;
		}
		while (num < 610);
		return false;
	}

	private static bool 뾯鳴辬안쉟京鴫좁胛佴뢳嫓()
	{
		bool flag = true;
		if ((0x7409BC2C & 0x792E4DC0) == 0)
		{
			flag = false;
		}
		_ = 0;
		return false;
	}
}
