using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

public class 예냢趘檇때韤댼鸺具
{
	[椡퀰苬譶뤆鄅鑼틭묫剦堶첆播쓢뮃짯測鞴眽]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public MethodInfo capturedMethod;

		public bool isStatic;

		public bool hasReturnValue;

		internal void _003CRegisterFromReflection_003Eb__0(覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state, int[] argRegs, int retReg)
		{
			饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹 酁짷끜엯쵀柃懽農圁怡騾禶 = state.酁짷끜엯쵀柃懽農圁怡騾禶;
			ParameterInfo[] parameters = capturedMethod.GetParameters();
			int num = ((isStatic == false) ? 1 : 0);
			object[] array = new object[parameters.Length];
			bool[] array2 = new bool[parameters.Length];
			Type[] array3 = new Type[parameters.Length];
			for (int num2 = 0x20718641 ^ (22688 + -11913 + -6585) ^ (0x2071B3FA ^ (10353 + -652)); num2 < parameters.Length; num2 -= -((((1349898677 + -(0x74C3 ^ 0x7468)) ^ 0x13B8) + -(0x627D ^ 0x2420)) ^ (0x50757891 ^ ((-2053 ^ -1) + -(0x18DFABB5 ^ 0x18DFA88A)))))
			{
				Type parameterType = parameters[num2].ParameterType;
				long raw = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num - -num2]);
				if (parameterType.IsByRef)
				{
					array2[num2] = (byte)(0xE2381BC ^ (~-1877 + 1 + -1 + -548) ^ ((237210655 + -381) ^ 0xE2F)) != 0;
					array[num2] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, array3[num2] = parameterType.GetElementType(), 酁짷끜엯쵀柃懽農圁怡騾禶);
				}
				else
				{
					array[num2] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, parameterType, 酁짷끜엯쵀柃懽農圁怡騾禶);
				}
			}
			object obj = null;
			if (!isStatic)
			{
				long num3 = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[0x2F8ACAE ^ (0x2F8A683 ^ (3564 + -959))]);
				obj = ((!capturedMethod.DeclaringType.IsValueType) ? 酁짷끜엯쵀柃懽農圁怡騾禶.꼻쥥躍鵫峍캫瓷犉崗筩板詁反病(num3) : 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(num3, capturedMethod.DeclaringType, 酁짷끜엯쵀柃懽農圁怡騾禶));
			}
			object obj2 = capturedMethod.Invoke(obj, array);
			for (int num4 = 0x5E154586 ^ (0x5E15591A ^ (8160 + -836)); num4 < parameters.Length; num4 -= -(0x2E88589D ^ (((780701486 + -16275) & -1) ^ 0x1707)))
			{
				if (array2[num4])
				{
					long value = 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(array[num4], array3[num4]);
					long num5 = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num - -num4]);
					if (num5 != 0L && num5 < ((((89218644 + -(0x210F4971 ^ 0x210F4A63)) ^ 0x5DC) + -((104567 + -32085 + -8023) ^ 0x23C9)) ^ ((89268666 + -10773 + -23778 + -134) ^ 0x1CA1)))
					{
						酁짷끜엯쵀柃懽農圁怡騾禶.偂엗袹訵浕眰唘걪벹峔백튠끞륷托쳔皇鞈摋괂(num5, array[num4]);
					}
					else
					{
						state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(argRegs[num - -num4], value);
					}
				}
			}
			if (hasReturnValue && retReg >= ((((554365445 + -989) ^ 0x1BE3) + -(0x2C90 ^ 0x2537)) ^ 0x210AEC24))
			{
				long value2;
				if (obj2 is IntPtr intPtr)
				{
					value2 = intPtr.ToInt64();
				}
				else
				{
					value2 = ((!(obj2 is UIntPtr uIntPtr)) ? 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(obj2, capturedMethod.ReturnType) : ((long)uIntPtr.ToUInt64()));
				}
				state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(retReg, value2);
			}
		}

		private static bool LoadCache41()
		{
			int num = 28;
			do
			{
				num += 5;
			}
			while (num < 547);
			return false;
		}
	}

	private sealed class _003C_003Ec__DisplayClass4_1
	{
		public ConstructorInfo capturedCtor;

		public bool isValueType;

		public Type capturedType;

		internal void _003CRegisterFromReflection_003Eb__1(覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state, int[] argRegs, int retReg)
		{
			饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹 酁짷끜엯쵀柃懽農圁怡騾禶 = state.酁짷끜엯쵀柃懽農圁怡騾禶;
			ParameterInfo[] parameters = capturedCtor.GetParameters();
			int num = 0x7BCF272 ^ ((129825307 + -(34560 + -26619 + -7669)) ^ 0xB78);
			object[] array = new object[parameters.Length];
			for (int num2 = 0x3D06E8FE ^ 0x3D06E8FE; num2 < parameters.Length; num2 -= -(0x21102C5C ^ (5936 + -33) ^ ((1809948390 + -(0x10A654FA ^ 0x10A654B8)) ^ 0x2170) ^ (((0x3B455847 ^ 0x71B4F85F) + -409) ^ 0x1AF9)))
			{
				long raw = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num - -num2]);
				array[num2] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, parameters[num2].ParameterType, 酁짷끜엯쵀柃懽農圁怡騾禶);
			}
			object value = ((!isValueType || parameters.Length == 0) ? capturedCtor.Invoke(array) : Activator.CreateInstance(capturedType, array));
			state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(argRegs[0x2A9C47D0 ^ (0x3E15556 ^ (~-3806 + 1 + -1)) ^ 0x297D1C5B], 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(value, capturedType));
		}

		private static bool ProcessCache22()
		{
			int num = -1640531527;
			num = (num ^ 0x39FCE2CD) << 17;
			num = (num ^ 0x14E28104) << 7;
			num = (num ^ 0x6A7C2A08) << 24;
			num = (num ^ 0x38197624) << 25;
			return false;
		}

		private static bool InitContext68()
		{
			bool flag = true;
			if ((0x1DE4B49F & 0x17AB20E) == 0)
			{
				flag = false;
			}
			_ = 0;
			return false;
		}

		private static bool CheckToken34()
		{
			bool flag = true;
			if ((0x68C78EEA & 0x11AE612F) == 0)
			{
				flag = false;
			}
			_ = 0;
			return false;
		}
	}

	[咵싛卒釉仆鮲롚엯糔鰞룹铬钎]
	[鈷腦귮郟綯햽쬤뵋玪퍒鷃]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public 覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state;

		internal string _003CInvoke_003Eb__0(int r)
		{
			return state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(r).ToString();
		}

		private static int ComputeContext28()
		{
			bool flag = true;
			if ((0x799C3496 & 0x607DA39E) == 0)
			{
				flag = false;
			}
			_ = 0;
			return 0;
		}
	}

	private Dictionary<int, 쒩缽檠헅겄秕鉌棚툪퓶垈鬲핏딬蜣핯畿忊秽> 乡쫐햽觎쩺惐蒴灊먠熒렇谺늛朻閍 = new Dictionary<int, 쒩缽檠헅겄秕鉌棚툪퓶垈鬲핏딬蜣핯畿忊秽>();

	private Dictionary<int, MethodBase> 焢履脩愷亍萴贲쭇叠튒湋 = new Dictionary<int, MethodBase>();

	private static readonly bool 吠蚚嵦田쇨퉮덧왑쎩鏹퉱柌髖쪰꿶 = false;

	private Dictionary<int, string> 碤潭帒嘟嵒櫦죕镌溢贑鋖閦繤坧苞쾌쨨쫦椚 = new Dictionary<int, string>();

	private static readonly string[] 愝莞긟럐護댊莝듌憗晎釚宀 = new string[24]
	{
		"System.dll", "System.Core.dll", "System.Net.dll", "System.Net.Http.dll", "System.Net.Sockets.dll", "System.Runtime.dll", "System.Collections.dll", "System.Linq.dll", "System.Xml.dll", "System.Data.dll",
		"System.Drawing.dll", "System.Windows.Forms.dll", "Microsoft.VisualBasic.dll", "System.Management.dll", "System.ServiceProcess.dll", "System.DirectoryServices.dll", "System.Runtime.Remoting.dll", "System.Web.dll", "System.Transactions.dll", "System.Numerics.dll",
		"PresentationCore.dll", "PresentationFramework.dll", "WindowsBase.dll", "System.Xaml.dll"
	};

	public MethodBase 흷떇뿨玓풪顁룶섄돛針콚겅紖귂湻墬퇒즐뚃蜸(int id)
	{
		焢履脩愷亍萴贲쭇叠튒湋.TryGetValue(id, out var value);
		return value;
	}

	public void 傱눛狃녂몱棣汃渜丳鴙洺崹迈댈(int id, 쒩缽檠헅겄秕鉌棚툪퓶垈鬲핏딬蜣핯畿忊秽 handler)
	{
		乡쫐햽觎쩺惐蒴灊먠熒렇谺늛朻閍[id] = handler;
	}

	public void 穃츼包廹雖뫐랃輫(string typeName, string methodName, Type[] paramTypes, bool hasReturnValue, int id, [Optional] string[] methodGenericArgs)
	{
		_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals20 = new _003C_003Ec__DisplayClass4_0();
		CS_0024_003C_003E8__locals20.hasReturnValue = hasReturnValue;
		Type type = 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(typeName);
		if (methodName == ".ctor")
		{
			_003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass4_1();
			ConstructorInfo constructorInfo = null;
			if (paramTypes.Length != 0)
			{
				constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
			}
			if (constructorInfo == null)
			{
				ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (ConstructorInfo constructorInfo2 in constructors)
				{
					if (constructorInfo2.GetParameters().Length == paramTypes.Length)
					{
						constructorInfo = constructorInfo2;
						break;
					}
				}
			}
			if (constructorInfo == null)
			{
				throw new Exception("KitsuneExternalCallTable: method not found: " + typeName + "::.ctor");
			}
			焢履脩愷亍萴贲쭇叠튒湋[id] = constructorInfo;
			碤潭帒嘟嵒櫦죕镌溢贑鋖閦繤坧苞쾌쨨쫦椚[id] = typeName + "::.ctor";
			CS_0024_003C_003E8__locals12.capturedCtor = constructorInfo;
			CS_0024_003C_003E8__locals12.isValueType = type.IsValueType;
			CS_0024_003C_003E8__locals12.capturedType = type;
			乡쫐햽觎쩺惐蒴灊먠熒렇谺늛朻閍[id] = (覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state, int[] argRegs, int retReg) =>
			{
				饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹 酁짷끜엯쵀柃懽農圁怡騾禶 = state.酁짷끜엯쵀柃懽農圁怡騾禶;
				ParameterInfo[] parameters2 = CS_0024_003C_003E8__locals12.capturedCtor.GetParameters();
				int num3 = 0x7BCF272 ^ ((129825307 + -(34560 + -26619 + -7669)) ^ 0xB78);
				object[] array2 = new object[parameters2.Length];
				for (int num4 = 0x3D06E8FE ^ 0x3D06E8FE; num4 < parameters2.Length; num4 -= -(0x21102C5C ^ (5936 + -33) ^ ((1809948390 + -(0x10A654FA ^ 0x10A654B8)) ^ 0x2170) ^ (((0x3B455847 ^ 0x71B4F85F) + -409) ^ 0x1AF9)))
				{
					long raw = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num3 - -num4]);
					array2[num4] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, parameters2[num4].ParameterType, 酁짷끜엯쵀柃懽農圁怡騾禶);
				}
				object value2 = ((!CS_0024_003C_003E8__locals12.isValueType || parameters2.Length == 0) ? CS_0024_003C_003E8__locals12.capturedCtor.Invoke(array2) : Activator.CreateInstance(CS_0024_003C_003E8__locals12.capturedType, array2));
				state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(argRegs[0x2A9C47D0 ^ (0x3E15556 ^ (~-3806 + 1 + -1)) ^ 0x297D1C5B], 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(value2, CS_0024_003C_003E8__locals12.capturedType));
			};
			return;
		}
		MethodInfo methodInfo = null;
		try
		{
			methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
		}
		catch (AmbiguousMatchException)
		{
		}
		if (methodInfo == null)
		{
			bool flag = methodGenericArgs != null && methodGenericArgs.Length != 0;
			MethodInfo methodInfo2 = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo3 in methods)
			{
				if (methodInfo3.Name != methodName || (flag && !methodInfo3.IsGenericMethodDefinition) || (!flag && methodInfo3.IsGenericMethodDefinition))
				{
					continue;
				}
				ParameterInfo[] parameters = methodInfo3.GetParameters();
				if (parameters.Length != paramTypes.Length)
				{
					continue;
				}
				bool flag2 = true;
				for (int num = 0; num < parameters.Length; num++)
				{
					if (parameters[num].ParameterType != paramTypes[num])
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					if (!methodInfo3.ReturnType.IsPointer)
					{
						methodInfo = methodInfo3;
						break;
					}
					if (methodInfo2 == null)
					{
						methodInfo2 = methodInfo3;
					}
				}
			}
			if (methodInfo == null)
			{
				methodInfo = methodInfo2;
			}
			if (methodInfo == null)
			{
				MethodInfo methodInfo4 = null;
				methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo5 in methods)
				{
					if (!(methodInfo5.Name != methodName) && (!flag || methodInfo5.IsGenericMethodDefinition) && (flag || !methodInfo5.IsGenericMethodDefinition) && methodInfo5.GetParameters().Length == paramTypes.Length)
					{
						if (!methodInfo5.ReturnType.IsPointer)
						{
							methodInfo = methodInfo5;
							break;
						}
						if (methodInfo4 == null)
						{
							methodInfo4 = methodInfo5;
						}
					}
				}
				if (methodInfo == null)
				{
					methodInfo = methodInfo4;
				}
			}
		}
		if (methodInfo == null)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder("Methods on " + type.FullName + " (looking for '" + methodName + "' with " + paramTypes.Length + " params):\n");
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo6 in methods)
				{
					stringBuilder.AppendLine("  " + methodInfo6.Name + " params=" + methodInfo6.GetParameters().Length);
				}
				File.AppendAllText("kitsune_debug.log", stringBuilder.ToString());
			}
			catch
			{
			}
			throw new Exception("KitsuneExternalCallTable: method not found: " + typeName + "::" + methodName);
		}
		if (methodGenericArgs != null && methodGenericArgs.Length != 0)
		{
			if (!methodInfo.IsGenericMethodDefinition)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo7 in methods)
				{
					if (methodInfo7.Name == methodName && methodInfo7.IsGenericMethodDefinition && methodInfo7.GetGenericArguments().Length == methodGenericArgs.Length)
					{
						methodInfo = methodInfo7;
						break;
					}
				}
			}
			if (methodInfo.IsGenericMethodDefinition)
			{
				Type[] array = new Type[methodGenericArgs.Length];
				for (int num2 = 0; num2 < methodGenericArgs.Length; num2++)
				{
					array[num2] = 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(methodGenericArgs[num2]);
				}
				methodInfo = methodInfo.MakeGenericMethod(array);
			}
		}
		CS_0024_003C_003E8__locals20.isStatic = methodInfo.IsStatic;
		CS_0024_003C_003E8__locals20.capturedMethod = methodInfo;
		焢履脩愷亍萴贲쭇叠튒湋[id] = methodInfo;
		쒩缽檠헅겄秕鉌棚툪퓶垈鬲핏딬蜣핯畿忊秽 value = (覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state, int[] argRegs, int retReg) =>
		{
			饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹 酁짷끜엯쵀柃懽農圁怡騾禶 = state.酁짷끜엯쵀柃懽農圁怡騾禶;
			ParameterInfo[] parameters2 = CS_0024_003C_003E8__locals20.capturedMethod.GetParameters();
			int num3 = ((CS_0024_003C_003E8__locals20.isStatic == false) ? 1 : 0);
			object[] array2 = new object[parameters2.Length];
			bool[] array3 = new bool[parameters2.Length];
			Type[] array4 = new Type[parameters2.Length];
			for (int num4 = 0x20718641 ^ (22688 + -11913 + -6585) ^ (0x2071B3FA ^ (10353 + -652)); num4 < parameters2.Length; num4 -= -((((1349898677 + -(0x74C3 ^ 0x7468)) ^ 0x13B8) + -(0x627D ^ 0x2420)) ^ (0x50757891 ^ ((-2053 ^ -1) + -(0x18DFABB5 ^ 0x18DFA88A)))))
			{
				Type parameterType = parameters2[num4].ParameterType;
				long raw = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num3 - -num4]);
				if (parameterType.IsByRef)
				{
					array3[num4] = (byte)(0xE2381BC ^ (~-1877 + 1 + -1 + -548) ^ ((237210655 + -381) ^ 0xE2F)) != 0;
					array2[num4] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, array4[num4] = parameterType.GetElementType(), 酁짷끜엯쵀柃懽農圁怡騾禶);
				}
				else
				{
					array2[num4] = 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(raw, parameterType, 酁짷끜엯쵀柃懽農圁怡騾禶);
				}
			}
			object obj2 = null;
			if (!CS_0024_003C_003E8__locals20.isStatic)
			{
				long num5 = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[0x2F8ACAE ^ (0x2F8A683 ^ (3564 + -959))]);
				obj2 = ((!CS_0024_003C_003E8__locals20.capturedMethod.DeclaringType.IsValueType) ? 酁짷끜엯쵀柃懽農圁怡騾禶.꼻쥥躍鵫峍캫瓷犉崗筩板詁反病(num5) : 饶굟雀칡뚔긿磕化癧넹硯銜쮳焺煪鷓筹.櫞듙菑诳볚볇왐驸썱햃喓諍(num5, CS_0024_003C_003E8__locals20.capturedMethod.DeclaringType, 酁짷끜엯쵀柃懽農圁怡騾禶));
			}
			object obj3 = CS_0024_003C_003E8__locals20.capturedMethod.Invoke(obj2, array2);
			for (int num6 = 0x5E154586 ^ (0x5E15591A ^ (8160 + -836)); num6 < parameters2.Length; num6 -= -(0x2E88589D ^ (((780701486 + -16275) & -1) ^ 0x1707)))
			{
				if (array3[num6])
				{
					long value2 = 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(array2[num6], array4[num6]);
					long num7 = state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(argRegs[num3 - -num6]);
					if (num7 != 0L && num7 < ((((89218644 + -(0x210F4971 ^ 0x210F4A63)) ^ 0x5DC) + -((104567 + -32085 + -8023) ^ 0x23C9)) ^ ((89268666 + -10773 + -23778 + -134) ^ 0x1CA1)))
					{
						酁짷끜엯쵀柃懽農圁怡騾禶.偂엗袹訵浕眰唘걪벹峔백튠끞륷托쳔皇鞈摋괂(num7, array2[num6]);
					}
					else
					{
						state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(argRegs[num3 - -num6], value2);
					}
				}
			}
			if (CS_0024_003C_003E8__locals20.hasReturnValue && retReg >= ((((554365445 + -989) ^ 0x1BE3) + -(0x2C90 ^ 0x2537)) ^ 0x210AEC24))
			{
				long value3 = ((!(obj3 is IntPtr intPtr)) ? ((!(obj3 is UIntPtr uIntPtr)) ? 酁짷끜엯쵀柃懽農圁怡騾禶.牘왧妉퉴鸃候瑧萖鼺蝚瀐욌泺界끇糶縄(obj3, CS_0024_003C_003E8__locals20.capturedMethod.ReturnType) : ((long)uIntPtr.ToUInt64())) : intPtr.ToInt64());
				state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.set_Item(retReg, value3);
			}
		};
		乡쫐햽觎쩺惐蒴灊먠熒렇谺늛朻閍[id] = value;
		碤潭帒嘟嵒櫦죕镌溢贑鋖閦繤坧苞쾌쨨쫦椚[id] = typeName + "::" + methodName;
	}

	public void 晳璓쎣펯鵦鶵셐쮅閸蛤(int id, 覲棪舆븞颥쏟漱茕梹꽌弓珪鬂퀡鯞殌縕荆 state, int[] argRegs, int retReg)
	{
		_003C_003Ec__DisplayClass5_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass5_0();
		CS_0024_003C_003E8__locals4.state = state;
		if (!乡쫐햽觎쩺惐蒴灊먠熒렇谺늛朻閍.TryGetValue(id, out var value))
		{
			throw new Exception(鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("lU/xXf7W4EEXk3K1sfb34sXX3r0WUphEY+/MSZCswnw2OOp4Ye5+oMORoLFr9NghFLZyKsi31KDIAq2K0cNbi076EXw8XC7VxvA1gyLj2U9ml4PCNieyx5xWBGmRvKtgBRIHz2EQNT7WK+VhFYVn7oTvXABG+kTLtvenHkwJ9Uw/") + id);
		}
		if (吠蚚嵦田쇨퉮덧왑쎩鏹퉱柌髖쪰꿶)
		{
			碤潭帒嘟嵒櫦죕镌溢贑鋖閦繤坧苞쾌쨨쫦椚.TryGetValue(id, out var value2);
			TextWriter error = Console.Error;
			string[] array = new string[0x46CF1F54 ^ ((1187976954 + -((8761 + -8177) & -1)) ^ (3404 + -(0x6946 ^ 0x6818)))];
			array[0x3D4A2581 ^ (~-10215 + 1 + -1 + -413) ^ 0x515D3D4D ^ (0x6C172898 ^ ((26516 + -20855) & -1))] = 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("REcbA7WlnoW5Xsit5c4jaJqSZ0XtI0QDLwSioyzLchA08mndIrruLNglNdNlj+WD6gkrj4JuFcARHGUWpUPVbfCFwxq04XFqzhrEZgraiFo67VWxDR4DhPy4ZKWBK7mZFg==");
			array[(((~-1191152431 + 1 + -1 + -221) ^ 0x1CB0) + -((~-19957 + 1 + -1) ^ 0x265B)) ^ ((1191133015 + -683) ^ ((14170 + -8123) & -1))] = id.ToString();
			array[((0x52622516 ^ 0x23D0) + -(0xB615 ^ (9803 + -30))) ^ 0x5261768C] = 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("7xbNvGVWMSoGrLI89zHj5LpsVb8HgqHJ8scymEr0u2nf7Qn4g49LHgu0avC1r9qyGtehYcT6SbilT3246UXPbOlsffC4/T+YacBw2SNp83Sh");
			array[0x79F218DF ^ (1851 + -(0x1D29 ^ 0x1E62)) ^ 0x6FE56840 ^ ((370674088 + -25790 + -14557) ^ (8950 + -(0x1551F97B ^ 0x1551FAEE)))] = value2 ?? 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("EktR2UM9VaxBiiauLzLFixK1ZrWhIzGcYMrUMbUoWMEv42CoZUXHJKk2nPA0HxaDirdc4YOSYx6xrIzGubDCZGQun+gw30XU6QLOHbui2V1W");
			array[(((1316680038 + -6649 + -1748) ^ 0x1895) + -((40785 + -((32017 + -31529) & -1)) ^ 0x1861)) ^ 0x4E7A4300] = 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("wBs3IrggfY+O5qzoKXlxGw7vqjHOVM+gZ0nPQm4tGOU1VcFtCJusRmto/SvGRxWAmzGyejgLdG7iUUOMlMNQfHS//qLOhR7kRaJH8n1ofN0w");
			array[(463317931 + -(0x1D9CE98A ^ 0x1D9CEB36)) ^ (38290 + -17671 + -15618 + -290) ^ ((884868708 + -11380 + -16163 + -(-917 ^ -1)) ^ (6935 + -617)) ^ ((790636064 + -(0x47D8270C ^ 0x47D827A5)) ^ 0x106D)] = string.Join(鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("CkmfHVPyebAyxDAI/bffRERR77Xo5thR7duTQ1rCef6JSkt7wZzEitoX7X+MbO2a7krhurXWlVrsRPgnZ26D2uBJn1vekuxVmGUwBTYzRYjs"), Array.ConvertAll(argRegs, (int r) => CS_0024_003C_003E8__locals4.state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(r).ToString()));
			array[(179272215 + -674) ^ 0x1701 ^ 0xAAF6072] = 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("6VpwiwjIbCSioUQAikjLcnmZ1JJ6ujeSdI1NAKON3J5Fw4u7hjPcZR5/p3+eq4PkqTdgMsFyETYXnC6xcq62Mu4nnCy2sFYWjGQuNC8p/4Xj");
			array[(145532233 + -35) ^ (10333 + -(0x67D2A1C6 ^ 0x67D2A2E1)) ^ ((145557431 + -22607 + -2909 + -796) ^ (8531 + -91))] = retReg.ToString();
			error.WriteLine(string.Concat(array));
		}
		value(CS_0024_003C_003E8__locals4.state, argRegs, retReg);
		if (吠蚚嵦田쇨퉮덧왑쎩鏹퉱柌髖쪰꿶 && retReg >= (((0x29AF9F3D ^ (6598 + -303)) + -(0x7E09 ^ 0x1790)) ^ ((~-699340050 + 1 + -1 + -(41192 + -24555 + -15998)) ^ 0xC83)))
		{
			Console.Error.WriteLine(鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("cdXh002T3A5XDYISRuoF+dVlo+ELoXGgmOCZwMbPhgORmTyO487x/L4NMsUO2OEk28M1vGwkxRIEDYCOKulnK64iRpI6cY+N5ZfKFpmYR9Ex5OjGppgPY0HZvs1U2knOKQ==") + retReg + 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("72TWznkezDacadSP+B9INPntxSPNV6SuOWWsk43x9+gNjbBT4TX4MsVCOUCuKmlhF0U0T1C9DLiXPIJ94pyjaoqhSSr0bX+DFqcTO2b92o+Z") + CS_0024_003C_003E8__locals4.state.邳솭鴶嶔푍矑멺쇅慈볏鷯飌损览瞼乇뿕뛠剧濛.get_Item(retReg));
		}
	}

	private static Type 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(string fullName)
	{
		string text = fullName.Replace('/', '+');
		if (掶層讖蛥糣瀷鰰篸뾗祘鋺(text))
		{
			return typeof(object);
		}
		Type type = 뛶俾쏺甎쏀멍噌柨(text);
		if (type != null)
		{
			return type;
		}
		int num = text.LastIndexOf('+');
		if (num > 0)
		{
			string text2 = text.Substring(0, num);
			string text3 = text.Substring(num + 1);
			string text4 = text3;
			string text5 = null;
			int num2 = text3.IndexOf('[');
			if (num2 > 0 && text3.EndsWith("]"))
			{
				text4 = text3.Substring(0, num2);
				text5 = text3.Substring(num2 + 1, text3.Length - num2 - 2);
			}
			Type type2 = 뛶俾쏺甎쏀멍噌柨(text2);
			if (type2 == null)
			{
				type2 = 驨询긃蝅죪뢞쬦騞(text2);
			}
			if (type2 == null)
			{
				try
				{
					type2 = 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(text2);
				}
				catch
				{
				}
			}
			if (type2 != null)
			{
				type = type2.GetNestedType(text4, BindingFlags.Public | BindingFlags.NonPublic);
				if (type == null)
				{
					Type[] nestedTypes = type2.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
					foreach (Type type3 in nestedTypes)
					{
						if (type3.Name == text4)
						{
							type = type3;
							break;
						}
					}
				}
				if (type != null && text5 != null && type.IsGenericTypeDefinition)
				{
					string[] array = 珻놀뇍꿕쑋繸孏램휐樴켢睦姁癿풼밎桘擳즶댜(text5);
					Type[] array2 = new Type[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(array[j].Trim());
					}
					type = type.MakeGenericType(array2);
				}
				if (type != null)
				{
					return type;
				}
			}
		}
		int num3 = text.IndexOf('[');
		if (num3 > 0 && text.EndsWith("]"))
		{
			string clrName = text.Substring(0, num3);
			string s = text.Substring(num3 + 1, text.Length - num3 - 2);
			Type type4 = 뛶俾쏺甎쏀멍噌柨(clrName);
			if (type4 != null && type4.IsGenericTypeDefinition)
			{
				string[] array3 = 珻놀뇍꿕쑋繸孏램휐樴켢睦姁癿풼밎桘擳즶댜(s);
				Type[] array4 = new Type[array3.Length];
				for (int k = 0; k < array3.Length; k++)
				{
					array4[k] = 偝跬駴曩籏赺鎯늘챧꼑逓쓱퉝栁똍傇썻顐젆룥(array3[k].Trim());
				}
				return type4.MakeGenericType(array4);
			}
		}
		throw new Exception("KitsuneExternalCallTable: type not found: " + fullName);
	}

	private static bool 掶層讖蛥糣瀷鰰篸뾗祘鋺(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return (byte)((-1376016593 + -203) ^ (3770 + -(39608 + -21024 + -18119)) ^ -1 ^ 0x52045572) != 0;
		}
		char[] array = new char[((-1172417351 ^ 0x1E0FB1FC) + -569) ^ 0xC37 ^ -1 ^ 0x5BEE10C6];
		array[(((156052411 + -23899 + -10759 + -(0x9BC5AB2 ^ 0x9BC58A6)) ^ 0xFA8) + -(0x3546 ^ ((-9753 ^ -1) + -((20087 + -19823) & -1)))) ^ ((156013717 + -436) ^ 0xF76)] = (char)(0x22D35EF1 ^ ((0x5A80 ^ 0x4E42) + -106) ^ ((147372791 + -109) ^ 0x1BB2) ^ (0x2A1BCA6E ^ ((14487 + -5822) & -1)));
		array[(1824924766 + -24070 + -31668) ^ 0x22C2 ^ 0x6CC56867] = (char)(0x680A0E2E ^ (0x680A1350 ^ (7824 + -316)));
		string text = name.TrimEnd(array);
		char[] array2 = new char[0x44CBEE01 ^ (9045 + -(-328 ^ -1)) ^ 0x30694A8 ^ ((~-1204637685 + 1 + -1) ^ (33143 + -22133 + -5041))];
		array2[(1248173595 + -87) ^ 0x230C ^ 0x1EF8E259 ^ ((~-1419591944 + 1 + -1) ^ 0x1D96)] = (char)((-1860910552 + -706) ^ 0xF53 ^ -1 ^ ((1860918276 + -75) ^ (5237 + -71)));
		array2[((0x6C376A5 ^ ((0x41F8 ^ 0x4781) + -280)) + -(0x4142 ^ 0x24A4)) ^ 0x6C30DDF] = (char)(((0x17AC4186 ^ 0x1119) + -((~-29327 + 1 + -1) ^ ((0x7DBC5F84 ^ 0x7DBC48E4) + -338))) ^ 0x17ABEC44);
		string text2 = text.TrimEnd(array2);
		if (text2.StartsWith(鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("IO9JAxVtDcfONOoC6Ltv8zf6+39MYoOLaUnrQoMzomwBgfQiiN0YvU7cBsf9aitDcSgoRtg1CLI6ntY+wtP/a56qwMyvPMKkKgaMH59cy6qM")))
		{
			return (byte)(((((716167802 + -25330) & -1) ^ (1353 + -315)) + -(((24017 + -526) & -1) ^ 0x179D)) ^ 0x2AAF2729) != 0;
		}
		if (text2.IndexOf((char)((-1114126933 + -967) ^ (8147 + -402) ^ -1 ^ (((0x3FD5DD4 ^ 0x419569B1) + -112) ^ 0x1381))) < (0x71696343 ^ (0x71694032 ^ (9085 + -(0x3C85B620 ^ 0x3C85B62C)))) && text2.IndexOf((char)(((((-411774410 ^ -1) + -98) ^ ((-6441 ^ -1) + -(0x12E5A1A8 ^ 0x12E5A190))) + -((53472 + -934) ^ 0x21DF)) ^ 0x188A48D2)) < (0x2A3C64A9 ^ (((0x20D1752B ^ 0xAED08CD) + -928) ^ 0x1EEF)) && text2.IndexOf((char)(((0x59892CA4 ^ (5580 + -597)) + -(0xF3CD ^ 0x1B49)) ^ ((1502116200 + -789) ^ (8862 + -343)))) < ((2044459896 + -239) ^ 0x1E10 ^ 0x415B2819 ^ ((947963733 + -605) ^ 0x878)) && text2.Length >= (((0x4716E359 ^ (2956 + -(-337 ^ -1))) + -(0xC8B8 ^ 0xAE9)) ^ (0x47162C89 ^ (3139 + -(0x48C8 ^ 0x486F)))) && (text2.Length == ((~-1414694721 + 1 + -1) ^ 0x18CC ^ (0x5452832B ^ (7827 + -493))) || (text2.Length <= ((-1389480807 + -931) ^ 0xC2B ^ -1 ^ (0x52D1E14D ^ (9590 + -763))) && text2[(-1883957521 + -((23381 + -23086) & -1)) ^ 0x491 ^ -1 ^ ((1883956466 + -223) ^ (3496 + -(0x4686 ^ 0x4475)))] == (((0x70CA775A ^ 0x1FCA) + -((36303 + -271) ^ 0x1188)) ^ (0x70C9C5BF ^ ((0x130FDC24 ^ 0x130FD385) + -(-255 ^ -1)))) && char.IsUpper(text2[(1974425855 + -17526 + -8071) ^ ((-3035 ^ -1) + -(-603 ^ -1)) ^ ((68105123 + -20305 + -31587 + -(~-161 + 1 + -1)) ^ (10165 + -535)) ^ 0x71A0B152]))) && char.IsUpper(text2[0x3D723DC4 ^ 0x3D723DC4]))
		{
			if (text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("1XJ13bMlSosac67M+MpZXz2gSa9/lI9/s6xPAdgc0zMikW9E0i6SbTV+PUVebieEsCQNMoKq4/zn80k8HdquAa8+bqZw28S1rcgu/sYqCUGq") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("o5PfqoNi/t/uBkRA7ltr1urms6xGATmtlV4+JwGvlaPnqa78Nwu7sw2C7kSOTVruNHWCYvdjFUjECo8hxZ+3EpkclZOEXDhuGgJrXSzqB5A4") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("JYW5xpBDAWOtCVOy2oBgLw3qfBDmZkCMSR4zAvNTCfrhmA4MMzTw8Lu3trAHFCVFCZuzR7t3gK1kM9I21/SxOEW7p5zXKKh5IBZDEkIcIzjs") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("FjlVE+FZtC1MA+jNFl3NfGpVZHUdfUyCiop+RkJov4GL6QnUP2HF9xnSQyYSam1dKU/WFSk1c9D6squ051A18j31eoyjbZKtGEkI22lJULei") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("KLyKi+8CfHKogowFNGUMoaNF2hArtSQMJhs3u5FI8m4Q5Ic/r2QwMkE17kDhFBFIJE8YRsmaUd9Pvxl2H7IfJxLPe2gh4vQVXjRYOpwZNki+") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("f4tfi1edv8K3vTdRl/WZCxbe7GtWayJ8WfI/RMJG7s/rTW2Nb8n6ITSqhW9HVBT3qMrlaxFGOCMtOMr8vCm5fCfoTgyk36UxpHNYevhSoJjj") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("p6jSdjjsoE7nGzzLZ64QvQhFl4IP8MDgORbPJrPXgcBJWaLti91C1/nPVu3LJ5POeRBYbGRtxzIIEYil0FnoaLN2drIgd3aBGWAKqTbwmxJm") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("26KuAFZomV7D2y4qrakODj5NnlcTooHPiHOmtHqa3GZLXhobppDZuCP7N1dUbxvN2lCEbTy7FK91jG95d9N23wx0dXITdY6ED/Bzyojwtf32") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("7FlaeECQMPh9J++dshPb8ICZS0azfZ3N1cdhQEYINoExoL2fkpWGqb9aem90xeMXoyT1Nhy8V8BCmQ7LtKMny5gWj7jsdTLMF6DhxwwJHLFU") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("JqTCVGfs8KMjj1hFoGTcf/ytiATjoqIYlvPll+yKy15e+bUCwRoqMzs+k4Ez4UTSzJbKNQPgCNsJ5FXcny8iC83NZvRba94AtGAzGU/FEHAM") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("82+p5Hti/Mj69hM4UpScH2Ciil+4dd1vTiEMKn3/srmKc9HSfC4GMO0/jJoQ9dJKS4Rl9RmRBIK6aeFebNn3S917ZxV3z0cCC6AQjPSikvD6") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("EzCKrrcF1R160AkfDck/EjnEJFzZvnkOBJbYdD12dpe4FSB4rpv8RYHZWu/oA2uvyphHByNm8etH6xxEYdB2SXJkFDVnFdCzlWEBbWKe+U8E") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("yAWKHcKqJE3LZV6xU38qINaGqFodTGc0YISs7Ef3S9oPkFtS67YvAANMF4AOkeB0VGaEqC4JFHPNuaPFL7KGLsKXcOoA0nmNC0IxNVPA5B3/") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("Yp7K1GRthR/tVANyv05OICZEeIcAuAGW/0dIFoxPjtF1POmW4IJfqIviAU/JHuahVOWYgOfuTQfRkHVlrH8+bZ/niQxIIy+KBCRbEazOt0Pv") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("wGrG/Vm8KAOiifzvUPxtJPpaHujIQVlmYbXycsyeBW14Y3KfBAU6nsaMgcZTvtrUA0JP4Xj8OrkWLwigKaH4zCxu/yQ4GVU6Rhp40lhp9ubv") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("AeB6rORBqQByA38pKRLshFFVXYEa+tLiwTAg2xR39i9tOpkFL9yP8s8c2/UQAHmrZ1d+18TtGAocVAAw+Q+KQJV9K5RY6i/AiGapr6a8PgM2") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("mmYRfruEXG1qB4a+o2C6jFQqM8g4+fDU+aiJWUizQQJXfJNJeVIMRrTilEGmDD8jZi1CCGp7hCis6o/xgKILt8qTbdIJcpDdkGTlTKRwejPL") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("Y2YgUtTe8Pq0HjfmPfnbFwibCoxeIjTAs4ufujpKpZ3ATmNLbAxn1sVG5pqfu8APW8jSbFuNvdo6JPKbbYxgXYucP0+Feyr/zvoCkYpFkN/b") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("Og0Cg8iihTyWz6lJbMkkNG3x/8RaPAZu/QLRfKYzX1G8Qz5pYht2thSlEtmFh3/f8cRV2aLc9/LZDm3ZT38RfRTOd6t3Pw+O/by1c7JOValA") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("K2ud0v36ZSUaSjkNSdbdfZ7umjeY58lsH0vdaFvwwFey/3lZol4PA2M1qvtDzufBzVov2YRswcEUbZMu2Sq7RfeaVEG5x3DyQhG8v/hFFXGR") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("ri4kliG3khFJ5TxZYW3IW6OAuydR2gBoUVNoxi38Zsn6XdvKMAS9Lhvb0HkJ142SegRp8CVPJhTBiOO7KRf47CzTK5eyMxo1Kro78shJSG6t") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("hTN5SwLzwkqFp89TLVzEbRgg04W3G0mhdxEE1xzL3BR6/XuHEEZbSA0OOwIB5Lqm0Um6yGLF8C4UVSgIJc4MOP14J7zsT0YeMd/RzKWFWjpl") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("ygqYQy/EASD94+dDtlFkyAWf8QZeJk3kKmJRkuCkIbnU14KCaICQ674Z1YbWqKYFZ+lTcKcelu+5F9YL3bPBzWCeDFYg1cgdWkqdO2AZfXMr") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("Lnxhp6oxcm1JbA0nk8oSPr6eEEW+zIdMHC7xTGkvYC/0nyEe2GFnGHrCKINyshbe3Hnt3cxkCRwB4Pc6am8BjK4AlBPpIh3k06XJIPhHJMOy") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("X1XQBqeypPpv4Om3RcPe/Qwc4QVwj8cw/GNj7iDIk6HIc9ZZPmJy9TlGpNr22a7ne2GqNWuzkutAtJC5p6gA8KbSeBAYNGOSE7FkO/DLmoqw") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("mbk9XjLXOz6OmMTfUB/I66pF+Z1w8J0hKBVhNhQuPoN/hhPAv54i5Tx39AibLg7x7ejBp0graEKcvrB5MVhti3iscbQKSlVzblbkP2iQyolS") || text2 == 鈋춹鑀憆燧濩雂支簲拍쵁灎.푙춣뇒澕錅캎炐檽獚똾憌("BieG49wQsLY+9c+eq3zkC4V1XwOQJNm5t9mnQsr7awHz2iJAOyOluuISKvAMwnT8bQCABbINwG+e6z82bqLpg7DmpEemkIZCObZK+xwX8VYT"))
			{
				return (byte)(((-796209156 ^ 0x2C99F5A0) + -308) ^ 0x7FD ^ -1 ^ ((((65874429 + -21088) & -1) + -537) ^ 0x14AE)) != 0;
			}
			return (byte)((1428620722 + -15015 + -25417) ^ (40679 + -27629 + -10897) ^ (0x2A5D58D8 ^ (8101 + -122)) ^ (0x7F7B382D ^ ((32433 + -28221) & -1))) != 0;
		}
		return (byte)(((0x3DFB9A32 ^ ((0x29E1F10C ^ 0x29E1D5BE) + -960)) + -(0xD9F2 ^ 0xD05)) ^ (0x3DFAECF5 ^ (2612 + -(0x7052FC90 ^ 0x7052FC68)))) != 0;
	}

	private static Type 뛶俾쏺甎쏀멍噌柨(string clrName)
	{
		if (掶層讖蛥糣瀷鰰篸뾗祘鋺(clrName))
		{
			return null;
		}
		Type type = Type.GetType(clrName);
		if (type != null)
		{
			return type;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			type = assemblies[i].GetType(clrName);
			if (type != null)
			{
				return type;
			}
		}
		string directoryName = Path.GetDirectoryName(typeof(object).Assembly.Location);
		string[] array = 愝莞긟럐護댊莝듌憗晎釚宀;
		foreach (string path in array)
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
			array = Directory.GetFiles(directoryName, "*.dll");
			foreach (string assemblyFile in array)
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
		string directoryName2 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		if (!string.IsNullOrEmpty(directoryName2))
		{
			array = Directory.GetFiles(directoryName2, "*.dll");
			foreach (string assemblyFile2 in array)
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
		type = 驨询긃蝅죪뢞쬦騞(clrName);
		if (type != null)
		{
			return type;
		}
		return null;
	}

	private static Type 驨询긃蝅죪뢞쬦騞(string clrName)
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

	private static string[] 珻놀뇍꿕쑋繸孏램휐樴켢睦姁癿풼밎桘擳즶댜(string s)
	{
		List<string> list = new List<string>();
		int num = 0x7137E23C ^ 0x7137E23C;
		int num2 = ((0x6CE3B836 ^ (40145 + -28113 + -5637)) + -((~-51531 + 1 + -1 + -(-425 ^ -1)) ^ 0x1250)) ^ ((1826803542 + -((23571 + -23546) & -1)) ^ 0x5E6);
		for (int num3 = (((1021093275 + -470) ^ 0x1E1E) + -((33967 + -855) ^ (3717 + -750))) ^ ((1021057618 + -842) ^ (9545 + -325)); num3 < s.Length; num3 -= -(((0x389BB4E2 ^ -1) + -462) ^ 0x5E7 ^ -1 ^ (0x389BABFB ^ (6471 + -154))))
		{
			if (s[num3] == ((((71875375 + -(0x12F5 ^ 0x122C)) ^ 0x198B) + -(0xBCBA ^ 0x1D91)) ^ ((71880886 + -19637 + -29051) ^ ((29500 + -25293) & -1))))
			{
				num -= -(0x777412C9 ^ (0x77740122 ^ (6042 + -(0x5ADA ^ 0x596A))));
			}
			else if (s[num3] == (0x5A83A038 ^ (0x5A838133 ^ ((-8605 ^ -1) + -(0x5C1602B8 ^ 0x5C1602FE)))))
			{
				num += -(0x6D5649D6 ^ (~-5924 + 1 + -1) ^ ((~-1834374458 + 1 + -1) ^ (3074 + -53)));
			}
			else if (s[num3] == ((-180284997 + -668) ^ 0x1A20 ^ -1 ^ ((180288217 + -11) ^ 0x1022)) && num == 0)
			{
				list.Add(s.Substring(num2, num3 + -num2));
				num2 = num3 - -(((0x49E6A1A0 ^ 0xF35) + -(0x809B ^ 0x1A6B)) ^ (0x49E60455 ^ (((26303 + -19923) & -1) + -(0x6C5ACD13 ^ 0x6C5ACDE8))));
			}
		}
		list.Add(s.Substring(num2));
		return list.ToArray();
	}

	private static int 쇐뺪釄毧铇鯼셠흄讝輼듘륽쇂剎앇箫向捗虈暰()
	{
		int num = 29;
		do
		{
			num += 6;
		}
		while (num < 864);
		return 0;
	}

	private static bool 뫡쎼灩눙趘打냫꼄궂()
	{
		int num = 29;
		do
		{
			num += 7;
		}
		while (num < 969);
		return false;
	}
}
