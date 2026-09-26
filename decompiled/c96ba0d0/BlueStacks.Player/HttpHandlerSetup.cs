using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class HttpHandlerSetup
{
	public static HTTPServer Server;

	public static void InitHTTPServer(Dictionary<string, RequestHandler> routes, string vmName)
	{
		int frontendServerPort = RegistryManager.Instance.Guest[vmName].FrontendServerPort;
		int num = frontendServerPort + 40;
		Server = HTTPUtils.SetupServer(frontendServerPort, num, routes, string.Empty);
		SetFrontendPortInBootParams(Server.Port, vmName);
		RegistryManager.Instance.Guest[vmName].FrontendServerPort = Server.Port;
		Server.Run();
	}

	private static void SetFrontendPortInBootParams(int frontendPort, string vmName)
	{
		string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
		string[] array = bootParameters.Split(new char[1] { ' ' });
		string text = "";
		string text2 = $"10.0.2.2:{frontendPort}";
		if (bootParameters.IndexOf("WINDOWSFRONTEND") == -1)
		{
			text = bootParameters + " WINDOWSFRONTEND=" + text2;
		}
		else
		{
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				if (text3.IndexOf("WINDOWSFRONTEND") != -1)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += " ";
					}
					text = text + "WINDOWSFRONTEND=" + text2;
				}
				else
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += " ";
					}
					text += text3;
				}
			}
		}
		RegistryManager.Instance.Guest[vmName].BootParameters = text;
	}
}
