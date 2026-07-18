using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI;

internal sealed class RenderProcessHandler : CefRenderProcessHandler
{
	private MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();

	protected override void OnWebKitInitialized()
	{
		string text = "var gmApi = function(jsonArg) {\r\n                    native function MyNativeFunction(jsonArg);\r\n                    return MyNativeFunction(jsonArg);\r\n                };";
		CefRuntime.RegisterExtension("MessageEvent", text, (CefV8Handler)(object)myCefV8Handler);
		((CefRenderProcessHandler)this).OnWebKitInitialized();
	}

	protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		myCefV8Handler.OnProcessMessageReceived(message);
		return ((CefRenderProcessHandler)this).OnProcessMessageReceived(browser, sourceProcess, message);
	}
}
