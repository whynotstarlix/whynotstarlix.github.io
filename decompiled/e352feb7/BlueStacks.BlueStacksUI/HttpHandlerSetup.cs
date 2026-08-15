using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public static class HttpHandlerSetup
{
	public static HTTPServer Server { get; set; }

	public static void InitHTTPServer(Dictionary<string, RequestHandler> routes)
	{
		int num = ((1822896686 > 1238342966) ? 2871 : 3828);
		int num2 = num + (1048444934 - (0x2C355BDC | 0x366DED2C));
		Server = HTTPUtils.SetupServer(num, num2, routes, string.Empty);
		RegistryManager.Instance.PartnerServerPort = Server.Port;
		Server.Run();
	}
}
