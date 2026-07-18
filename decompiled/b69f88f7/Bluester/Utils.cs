using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Bluester;

public static class Utils
{
	internal class Hardware
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint GetSystemFirmwareTable(uint providerSignature, uint tableID, IntPtr pFirmwareTableBuffer, uint bufferSize);

		private static byte[] GetSMBiosRaw()
		{
			uint systemFirmwareTable = GetSystemFirmwareTable(1381190978u, 0u, IntPtr.Zero, 0u);
			if (systemFirmwareTable == 0)
			{
				return null;
			}
			IntPtr intPtr = Marshal.AllocHGlobal((int)systemFirmwareTable);
			try
			{
				GetSystemFirmwareTable(1381190978u, 0u, intPtr, systemFirmwareTable);
				byte[] array = new byte[systemFirmwareTable];
				Marshal.Copy(intPtr, array, 0, (int)systemFirmwareTable);
				return array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private static string ParseSMBiosString(byte[] data, int offset, int index)
		{
			int i = offset;
			while (index > 1 && i < data.Length)
			{
				for (; i < data.Length && data[i] != 0; i++)
				{
				}
				i++;
				index--;
			}
			int num = i;
			for (; i < data.Length && data[i] != 0; i++)
			{
			}
			return Encoding.ASCII.GetString(data, num, i - num).Trim();
		}

		private static Dictionary<string, string> ParseSMBios(byte[] raw)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 8;
			while (num < raw.Length && num + 1 < raw.Length)
			{
				byte b = raw[num];
				byte b2 = raw[num + 1];
				if (b2 == 0 || num + b2 > raw.Length)
				{
					break;
				}
				if (b == 1 && b2 >= 25)
				{
					byte[] array = new byte[16];
					Array.Copy(raw, num + 8, array, 0, 16);
					Array.Reverse((Array)array, 0, 4);
					Array.Reverse((Array)array, 4, 2);
					Array.Reverse((Array)array, 6, 2);
					dictionary["UUID"] = new Guid(array).ToString();
				}
				else if (b == 2 && b2 >= 8)
				{
					byte index = raw[num + 7];
					string value = ParseSMBiosString(raw, num + b2, index);
					dictionary["BASEBOARD"] = value;
				}
				else if (b == 4 && b2 >= 16)
				{
					string value2 = $"{BitConverter.ToUInt32(raw, num + 8):X8}{BitConverter.ToUInt32(raw, num + 12):X8}";
					dictionary["CPU"] = value2;
				}
				int i;
				for (i = num + b2; i + 1 < raw.Length && (raw[i] != 0 || raw[i + 1] != 0); i++)
				{
				}
				num = i + 2;
			}
			return dictionary;
		}

		private static string GetValue(Dictionary<string, string> dict, string key)
		{
			if (!dict.ContainsKey(key))
			{
				return "N/A";
			}
			return dict[key];
		}

		internal static string GetHardware()
		{
			byte[] sMBiosRaw = GetSMBiosRaw();
			if (sMBiosRaw == null)
			{
				return "SMBIOS_UNAVAILABLE";
			}
			Dictionary<string, string> dict = ParseSMBios(sMBiosRaw);
			string value = GetValue(dict, "UUID");
			string value2 = GetValue(dict, "BASEBOARD");
			string s = value + "|" + value2;
			using SHA256 sHA = SHA256.Create();
			return BitConverter.ToString(sHA.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLower();
		}
	}

	private static readonly HttpClient http;

	public static void ApplyQuit()
	{
		try
		{
			Options.OptionsSettingsManager.OptionsConfig optionsConfig = Options.OptionsSettingsManager.Load();
			if (optionsConfig.AutolockEnabled && int.TryParse(optionsConfig.AutolockValue, out var result))
			{
				string vmname = Opt.Instance.vmname;
				if (RegistryManager.Instance.Guest.ContainsKey(vmname))
				{
					RegistryManager.Instance.Guest[vmname].EnableHighFPS = ((result > 165) ? 1 : 0);
					RegistryManager.Instance.Guest[vmname].FPS = result;
				}
				using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BlueStacks_bgp64\\Guests\\" + vmname, writable: true);
				if (registryKey != null)
				{
					object value = registryKey.GetValue("BootParameters");
					if (value != null)
					{
						string text = value.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							string value2 = Regex.Replace(text, "fps=\\d+", $"fps={result}");
							registryKey.SetValue("BootParameters", value2);
						}
					}
				}
			}
			string[] array = new string[4] { "HD-Player", "HD-Agent", "BstkSVC", "Bluestacks" };
			for (int i = 0; i < array.Length; i++)
			{
				Process[] processesByName = Process.GetProcessesByName(array[i]);
				foreach (Process process in processesByName)
				{
					try
					{
						process.Kill();
					}
					catch
					{
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Ошибка при выходе: " + ex.Message);
		}
	}

	private static void SecureWipe(byte[] data)
	{
		if (data != null)
		{
			Array.Clear(data, 0, data.Length);
		}
	}

	private static byte[] SecureStringToUtf8Bytes(SecureString secureString)
	{
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.SecureStringToBSTR(secureString);
			int num = Marshal.ReadInt32(intPtr, -4);
			byte[] array = new byte[num];
			Marshal.Copy(intPtr, array, 0, num);
			return Encoding.UTF8.GetBytes(Encoding.Unicode.GetString(array));
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.ZeroFreeBSTR(intPtr);
			}
		}
	}

	static Utils()
	{
		http = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			ServerCertificateCustomValidationCallback = (HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors) => cert != null && (errors == SslPolicyErrors.None || cert.Verify())
		})
		{
			Timeout = TimeSpan.FromSeconds(20.0)
		};
	}

	public static Tuple<bool, string> Authenticate(string key)
	{
		try
		{
			return Task.Run(() => AuthAsync(key)).GetAwaiter().GetResult();
		}
		catch (Exception ex)
		{
			return new Tuple<bool, string>(item1: false, "Failed to connect to the server: " + ex.Message);
		}
	}

	private static async Task<Tuple<bool, string>> AuthAsync(string key)
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		byte[] sessionKey = null;
		Tuple<bool, string> result;
		using (SecureString secureKey = new SecureString())
		{
			using SecureString secureHwid = new SecureString();
			string text = key;
			foreach (char c in text)
			{
				secureKey.AppendChar(c);
			}
			text = Hardware.GetHardware();
			foreach (char c2 in text)
			{
				secureHwid.AppendChar(c2);
			}
			secureKey.MakeReadOnly();
			secureHwid.MakeReadOnly();
			try
			{
				sessionKey = new byte[64];
				using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
				{
					rNGCryptoServiceProvider.GetBytes(sessionKey);
				}
				byte[] array;
				using (RSACryptoServiceProvider rSACryptoServiceProvider = ImportPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwWZWvkMcAlNTgXVl4YtZz56xVBYCQNVNMpnt0KAfuWiqTzfwBrA0tKnqfekYilHxOn2BCIC7X2sEp1Z6qLg/U1BwAbS+NQiomjEpwPWgcoyvscH5IaYhrQzg1xyddIq4zxN9YLG6Tql3nDpI0aI+eJw+DEEH6W8fzZcM0Lg042g7bAnGMvLX70lsdLXCWAzRKGLgDkL6SQ9UGqfqO/HIHKmYIgQWVe0GJTT+z5R24IHzqr4Gcpr6Nzv94RXyza/Au2FnYnbzDncqHEtFyy/Qphrji48OvnIeS/Z0OqoNpd1+/YKV/oDnSIgVyAQvlvI54aUhkFqKYD55qnULeVZQcQIDAQAB"))
				{
					array = rSACryptoServiceProvider.Encrypt(sessionKey, fOAEP: true);
				}
				byte[] array2 = SecureStringToUtf8Bytes(secureKey);
				byte[] array3 = SecureStringToUtf8Bytes(secureHwid);
				byte[] bytes = BitConverter.GetBytes((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse((Array)bytes);
				}
				byte[] data;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					memoryStream.Write(bytes, 0, bytes.Length);
					byte[] array4 = new byte[16];
					using (RNGCryptoServiceProvider rNGCryptoServiceProvider2 = new RNGCryptoServiceProvider())
					{
						rNGCryptoServiceProvider2.GetBytes(array4);
					}
					memoryStream.Write(array4, 0, array4.Length);
					memoryStream.WriteByte((byte)array2.Length);
					memoryStream.Write(array2, 0, array2.Length);
					memoryStream.WriteByte((byte)array3.Length);
					memoryStream.Write(array3, 0, array3.Length);
					data = memoryStream.ToArray();
				}
				byte[] array5 = EncryptAesCbcHmac(data, sessionKey);
				SecureWipe(array2);
				SecureWipe(array3);
				byte[] bytes2 = BitConverter.GetBytes(array.Length);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse((Array)bytes2);
				}
				byte[] content;
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					memoryStream2.Write(bytes2, 0, 4);
					memoryStream2.Write(array, 0, array.Length);
					memoryStream2.Write(array5, 0, array5.Length);
					content = memoryStream2.ToArray();
				}
				using ByteArrayContent content2 = new ByteArrayContent(content);
				content2.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				HttpResponseMessage httpResponseMessage = await http.PostAsync("https://bluester.up.railway.app/api/auth", content2);
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					result = new Tuple<bool, string>(item1: false, "Server returned an error: " + httpResponseMessage.ReasonPhrase);
				}
				else
				{
					byte[] bytes3 = DecryptAesCbcHmac(await httpResponseMessage.Content.ReadAsByteArrayAsync(), sessionKey);
					JObject obj = JObject.Parse(Encoding.UTF8.GetString(bytes3));
					string text2 = ((JToken)obj).Value<string>((object)"status");
					string item = ((JToken)obj).Value<string>((object)"message");
					result = new Tuple<bool, string>(text2 == "ok", item);
				}
			}
			catch (Exception)
			{
				result = new Tuple<bool, string>(item1: false, "");
			}
			finally
			{
				SecureWipe(sessionKey);
			}
		}
		return result;
	}

	private static RSACryptoServiceProvider ImportPublicKey(string pem)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		RsaKeyParameters val = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(pem.Replace("\n", "").Replace("\r", "")));
		RSAParameters parameters = new RSAParameters
		{
			Modulus = val.Modulus.ToByteArrayUnsigned(),
			Exponent = val.Exponent.ToByteArrayUnsigned()
		};
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.ImportParameters(parameters);
		return rSACryptoServiceProvider;
	}

	private static byte[] EncryptAesCbcHmac(byte[] data, byte[] key)
	{
		byte[] key2 = key.Take(32).ToArray();
		byte[] key3 = key.Skip(32).ToArray();
		byte[] iV;
		byte[] array;
		using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
		{
			aesCryptoServiceProvider.Key = key2;
			aesCryptoServiceProvider.Mode = CipherMode.CBC;
			aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			iV = aesCryptoServiceProvider.IV;
			using ICryptoTransform transform = aesCryptoServiceProvider.CreateEncryptor();
			using MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
			{
				cryptoStream.Write(data, 0, data.Length);
			}
			array = memoryStream.ToArray();
		}
		byte[] array2;
		using (HMACSHA256 hMACSHA = new HMACSHA256(key3))
		{
			using MemoryStream memoryStream2 = new MemoryStream();
			memoryStream2.Write(iV, 0, iV.Length);
			memoryStream2.Write(array, 0, array.Length);
			array2 = hMACSHA.ComputeHash(memoryStream2.ToArray());
		}
		byte[] array3 = new byte[iV.Length + array.Length + array2.Length];
		Buffer.BlockCopy(iV, 0, array3, 0, iV.Length);
		Buffer.BlockCopy(array, 0, array3, iV.Length, array.Length);
		Buffer.BlockCopy(array2, 0, array3, iV.Length + array.Length, array2.Length);
		return array3;
	}

	private static byte[] DecryptAesCbcHmac(byte[] encryptedBlob, byte[] key)
	{
		byte[] key2 = key.Take(32).ToArray();
		byte[] key3 = key.Skip(32).ToArray();
		byte[] array = encryptedBlob.Take(16).ToArray();
		byte[] first = encryptedBlob.Skip(encryptedBlob.Length - 32).ToArray();
		byte[] array2 = encryptedBlob.Skip(16).Take(encryptedBlob.Length - 16 - 32).ToArray();
		byte[] second;
		using (HMACSHA256 hMACSHA = new HMACSHA256(key3))
		{
			using MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(array, 0, array.Length);
			memoryStream.Write(array2, 0, array2.Length);
			second = hMACSHA.ComputeHash(memoryStream.ToArray());
		}
		if (!Enumerable.SequenceEqual(first, second))
		{
			throw new CryptographicException("Invalid MAC");
		}
		using AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		aesCryptoServiceProvider.Key = key2;
		aesCryptoServiceProvider.IV = array;
		aesCryptoServiceProvider.Mode = CipherMode.CBC;
		aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		using ICryptoTransform transform = aesCryptoServiceProvider.CreateDecryptor();
		using MemoryStream memoryStream2 = new MemoryStream();
		using (CryptoStream cryptoStream = new CryptoStream(memoryStream2, transform, CryptoStreamMode.Write))
		{
			cryptoStream.Write(array2, 0, array2.Length);
		}
		return memoryStream2.ToArray();
	}
}
