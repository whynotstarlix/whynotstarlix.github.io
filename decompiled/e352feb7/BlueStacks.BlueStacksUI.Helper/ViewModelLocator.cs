using GalaSoft.MvvmLight.Ioc;

namespace BlueStacks.BlueStacksUI.Helper;

public class ViewModelLocator
{
	static ViewModelLocator()
	{
		SimpleIoc.Default.Register<MinimizeBlueStacksOnCloseView>();
	}
}
