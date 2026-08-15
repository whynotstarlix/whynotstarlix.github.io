namespace BlueStacks.BlueStacksUI;

internal interface IControlFordimOverLay
{
	bool IsCloseOnOverLayClick { get; set; }

	bool Close();

	bool Show();
}
