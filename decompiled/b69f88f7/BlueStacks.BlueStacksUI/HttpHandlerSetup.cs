using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public static class HttpHandlerSetup
{
	public static HTTPServer Server { get; set; }

	public static void InitHTTPServer(Dictionary<string, RequestHandler> routes)
	{
		int num = 2871 + 10;
		Server = HTTPUtils.SetupServer(2871, num, routes, string.Empty);
		RegistryManager.Instance.PartnerServerPort = Server.Port;
		Server.Run();
	}
}
