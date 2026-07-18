using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bluester;

public static class Updater
{
	private static readonly HttpClient _client = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(30.0)
	};

	public static async Task<bool> CheckAndApplyUpdate()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
		bool result = default(bool);
		object obj;
		int num;
		try
		{
			string text = await GetServerVersion();
			if (!string.IsNullOrEmpty(text) && !(text == "1.0.0.9"))
			{
				string text2 = await DownloadUpdate();
				if (string.IsNullOrEmpty(text2))
				{
					result = false;
					return result;
				}
				ApplyUpdateAndRestart(text2);
				result = true;
				return result;
			}
			await Assets.Check();
			result = false;
			return result;
		}
		catch (Exception ex)
		{
			obj = ex;
			num = 1;
		}
		if (num != 1)
		{
			return result;
		}
		Exception ex2 = (Exception)obj;
		Console.WriteLine("Update failed: " + ex2.Message);
		await Assets.Check();
		return false;
	}

	private static void ApplyUpdateAndRestart(string newFilePath)
	{
		string executablePath = Application.ExecutablePath;
		string directoryName = Path.GetDirectoryName(executablePath);
		string text = Path.Combine(Path.GetTempPath(), "update.bat");
		string contents = "\r\n@echo off\r\nchcp 65001 > nul\r\ntimeout /t 2 /nobreak > NUL\r\ntaskkill /f /im \"" + Path.GetFileName(executablePath) + "\" > NUL 2>&1\r\n\r\nfor %%f in (\"" + directoryName + "\\*.dll\") do del /f /q \"%%f\"\r\n\r\ndel /f /q \"" + executablePath + "\"\r\nmove /y \"" + newFilePath + "\" \"" + executablePath + "\"\r\n\r\nstart \"\" \"" + executablePath + "\"\r\ndel \"%~f0\"\r\n";
		File.WriteAllText(text, contents, Encoding.UTF8);
		Process.Start(new ProcessStartInfo
		{
			FileName = "cmd.exe",
			Arguments = "/c \"" + text + "\"",
			CreateNoWindow = true,
			UseShellExecute = false
		});
		Environment.Exit(0);
	}

	private static async Task<string> GetServerVersion()
	{
		try
		{
			return (await _client.GetStringAsync("https://bluester.up.railway.app/api/check_update")).Trim();
		}
		catch
		{
			return null;
		}
	}

	private static async Task<string> DownloadUpdate()
	{
		_ = 2;
		try
		{
			string tempPath = Path.Combine(Path.GetTempPath(), $"Bluestacks_update_{Guid.NewGuid()}.exe");
			using (HttpResponseMessage response = await _client.GetAsync("https://bluester.up.railway.app/api/download_update", HttpCompletionOption.ResponseHeadersRead))
			{
				response.EnsureSuccessStatusCode();
				using Stream contentStream = await response.Content.ReadAsStreamAsync();
				using FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true);
				await contentStream.CopyToAsync(fileStream);
			}
			return tempPath;
		}
		catch
		{
			return null;
		}
	}
}
