namespace BlueStacks.BlueStacksUI;

internal interface IDimOverlayControl
{
	bool IsCloseOnOverLayClick { get; set; }

	bool ShowControlInSeparateWindow { get; set; }

	bool ShowTransparentWindow { get; set; }

	double Height { get; set; }

	double Width { get; set; }

	bool Close();

	bool Show();
}
