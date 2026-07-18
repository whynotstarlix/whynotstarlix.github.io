using System;
using System.Text;

namespace BlueStacks.BlueStacksUI;

public static class Base64Helper
{
	public static string Encode(string plainText)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
	}

	public static string Decode(string base64Text)
	{
		byte[] bytes = Convert.FromBase64String(base64Text);
		return Encoding.UTF8.GetString(bytes);
	}
}
