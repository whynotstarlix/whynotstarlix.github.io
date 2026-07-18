using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class ProgressBar : UserControl, IDimOverlayControl, IComponentConnector
{
	internal ProgressBar mProgressBar;

	internal CustomPictureBox mLoadingImage;

	internal TextBlock mLabel;

	private bool _contentLoaded;

	private bool _showTransparentWindow;

	public string ProgressText
	{
		get
		{
			return mLabel.Text;
		}
		set
		{
			BlueStacksUIBinding.Bind(mLabel, value, "");
			if (string.IsNullOrEmpty(mLabel.Text))
			{
				((UIElement)mLabel).Visibility = (Visibility)2;
			}
		}
	}

	public bool IsCloseOnOverLayClick
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowControlInSeparateWindow
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool ShowTransparentWindow
	{
		get
		{
			return _showTransparentWindow;
		}
		set
		{
			_showTransparentWindow = value;
		}
	}

	public ProgressBar()
	{
		InitializeComponent();
	}

	public bool Close()
	{
		((UIElement)this).Visibility = (Visibility)1;
		return true;
	}

	public bool Show()
	{
		((UIElement)this).Visibility = (Visibility)0;
		return true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/progressbar.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mProgressBar = (ProgressBar)target;
			break;
		case 2:
			mLoadingImage = (CustomPictureBox)target;
			break;
		case 3:
			mLabel = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
