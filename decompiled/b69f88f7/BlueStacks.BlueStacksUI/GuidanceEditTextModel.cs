using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

[Serializable]
public class GuidanceEditTextModel : GuidanceEditModel
{
	private TextValidityOptions mTextValidityOption = (TextValidityOptions)1;

	public TextValidityOptions TextValidityOption
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mTextValidityOption;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((ViewModelBase)this).SetProperty<TextValidityOptions>(ref mTextValidityOption, value, (string)null);
		}
	}
}
