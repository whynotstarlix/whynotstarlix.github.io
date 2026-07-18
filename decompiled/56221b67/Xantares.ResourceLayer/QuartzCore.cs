using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Win32;

namespace Xantares.ResourceLayer;

public static class QuartzCore
{
	private static class Vault
	{
		private static readonly byte[] Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("xantares::pulse::vault"));

		internal static string Decode(string packed)
		{
			byte[] array = Convert.FromBase64String(packed);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] ^= Key[i % Key.Length];
			}
			return Encoding.UTF8.GetString(array);
		}
	}

	private static readonly string Endpoint = Vault.Decode("WW98uyL/UFNPQq606IjqBqoy/6+ipVhjbXRh3LXiz+keen2/OZoJTgVTqLA=");

	private static readonly string SealSeed = Vault.Decode("DUlbihqgBipKT7Wls87PEahp8ram+lNVeFpg79bP48lSSWOOAotHOk0QlozZxvQShUn7r7OJa1UiT1n5wtTz9Hc0PaMGlzsXcWWQobyitEvnatH0nbN0KWhnIojp7pj5RGEnsTO0NjNbaoyKwJO1Nf9MqKSnoWk7MCxYjv/5+aR3YSOFAqsIOx5HiJPCu60Jo3jnmpeNd2o7QlPe67rY514sYow4jCg+G0r5pOq6zxD1WvGBsYJmWU9RduXQtfz4fEIn5ACtEBlxVaiHvcPGJJR6zqaMo091YUdfyarIxvNrfSecB7YPHkxajJX/udspgTexreK0TiRIV1nI+MXFrh5rI4xp/E0IfGb3jL+EyDf1Xuunn7FUJnpte+moy8fRe0hPkiewM0xHcbqV34j4HalX//HslFUpWkxX7K610/5Db26dNY4qFmhg9Inqt+s0mmjohZK9aSdTdEfMw8qF+gZieYhnnDBFWG2J9+nDxVGLVMuGt55MUlt3cNvd1NnzY3VLsiOUET9EGvSN/abxSPlErIKfsUQ/eE13zdXl8+lpKEKCMK8wFHlVuILIw/dVuET5poTvRyM9ZUDL4ufo3GtvY/w56ikfbHmH8cmw6yaNZvyZsawVeDBzXNn33cHvYS1BsTzuPjhYSKiC5Z7lR40zyffsjVZRWXpR3PHm4/JJXW6HKfcvSE4WgaG5qOgqq1L/h7KwHT9EenHI9/nZow1ecLs+qxoSXx2BkcywvlGJZO6su6FPZDcpOu/IzeH4SE1ppySgQQ==");

	public static QuartzFrame Tick(string key)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		string text = CreateMarker();
		string text2 = CreateNode();
		try
		{
			HttpClient val = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(20.0)
			};
			try
			{
				FormUrlEncodedContent val2 = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string>
				{
					["key"] = key,
					["hwid"] = text2,
					["nonce"] = text,
					["app"] = "xantares",
					["protocol"] = "2"
				});
				try
				{
					HttpResponseMessage result = val.PostAsync(Endpoint, (HttpContent)(object)val2).GetAwaiter().GetResult();
					try
					{
						if (!result.IsSuccessStatusCode)
						{
							return QuartzFrame.Fail("network");
						}
						return Unpack(result.Content.ReadAsStringAsync().GetAwaiter().GetResult(), text2, text);
					}
					finally
					{
						((IDisposable)result)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch
		{
			return QuartzFrame.Fail("network");
		}
	}

	private static QuartzFrame Unpack(string response, string node, string marker)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(response))
		{
			return QuartzFrame.Fail("empty");
		}
		JavaScriptSerializer val = new JavaScriptSerializer();
		Dictionary<string, object> bag = val.DeserializeObject(response) as Dictionary<string, object>;
		string value = PickText(bag, "payload");
		string text = PickText(bag, "signature");
		if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(text))
		{
			return QuartzFrame.Fail("bad_response");
		}
		byte[] array = Base64UrlDecode(value);
		byte[] sealBytes = Convert.FromBase64String(text);
		if (!SealMatches(array, sealBytes))
		{
			return QuartzFrame.Fail("bad_signature");
		}
		Dictionary<string, object> bag2 = val.DeserializeObject(Encoding.UTF8.GetString(array)) as Dictionary<string, object>;
		if (!PickBool(bag2, "ok"))
		{
			return QuartzFrame.Fail(PickText(bag2, "message") ?? "denied");
		}
		if (!StringEquals(PickText(bag2, "hwid"), node) || !StringEquals(PickText(bag2, "nonce"), marker))
		{
			return QuartzFrame.Fail("mismatch");
		}
		DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(PickLong(bag2, "exp")).UtcDateTime;
		if (DateTime.UtcNow >= utcDateTime)
		{
			return QuartzFrame.Fail("expired");
		}
		return QuartzFrame.Ok(utcDateTime);
	}

	private static bool SealMatches(byte[] frameBytes, byte[] sealBytes)
	{
		using RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(3072);
		rSACryptoServiceProvider.PersistKeyInCsp = false;
		rSACryptoServiceProvider.FromXmlString(SealSeed);
		return rSACryptoServiceProvider.VerifyData(frameBytes, CryptoConfig.MapNameToOID("SHA256"), sealBytes);
	}

	private static string CreateNode()
	{
		string text = string.Empty;
		try
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			using RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
			text = (registryKey2?.GetValue("MachineGuid") as string) ?? string.Empty;
		}
		catch
		{
		}
		string s = Environment.MachineName + "|" + Environment.UserName + "|" + text;
		using SHA256 sHA = SHA256.Create();
		return ToHex(sHA.ComputeHash(Encoding.UTF8.GetBytes(s)));
	}

	private static string CreateMarker()
	{
		byte[] array = new byte[24];
		using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
		{
			randomNumberGenerator.GetBytes(array);
		}
		return Base64UrlEncode(array);
	}

	private static string Base64UrlEncode(byte[] value)
	{
		return Convert.ToBase64String(value).TrimEnd(new char[1] { '=' }).Replace('+', '-')
			.Replace('/', '_');
	}

	private static byte[] Base64UrlDecode(string value)
	{
		string text = value.Replace('-', '+').Replace('_', '/');
		switch (text.Length % 4)
		{
		case 2:
			text += "==";
			break;
		case 3:
			text += "=";
			break;
		}
		return Convert.FromBase64String(text);
	}

	private static string ToHex(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
		foreach (byte b in data)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private static string PickText(Dictionary<string, object> bag, string name)
	{
		if (bag == null || !bag.ContainsKey(name))
		{
			return null;
		}
		return (bag[name] as string) ?? Convert.ToString(bag[name]);
	}

	private static bool PickBool(Dictionary<string, object> bag, string name)
	{
		if (bag == null || !bag.ContainsKey(name))
		{
			return false;
		}
		object obj = bag[name];
		if (obj is bool)
		{
			return (bool)obj;
		}
		bool result;
		return bool.TryParse(Convert.ToString(obj), out result) && result;
	}

	private static long PickLong(Dictionary<string, object> bag, string name)
	{
		if (bag == null || !bag.ContainsKey(name))
		{
			return 0L;
		}
		object obj = bag[name];
		if (obj is long)
		{
			return (long)obj;
		}
		return Convert.ToInt64(obj);
	}

	private static bool StringEquals(string left, string right)
	{
		return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
	}
}
