using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BlueStacks.Common;
using GalaSoft.MvvmLight.Command;

namespace BlueStacks.BlueStacksUI;

public class MinimizeBlueStacksOnCloseViewModel : INotifyPropertyChanged
{
	private bool mIsDoNotShowAgainChkBoxChecked;

	private bool mIsQuitBluestacksChecked;

	private bool mIsMinimizeBlueStacksRadioBtnChecked = true;

	public MainWindow ParentWindow { get; set; }

	public bool IsDoNotShowAgainChkBoxChecked
	{
		get
		{
			return mIsDoNotShowAgainChkBoxChecked;
		}
		set
		{
			mIsDoNotShowAgainChkBoxChecked = value;
			NotifyPropertyChanged("IsDoNotShowAgainChkBoxChecked");
		}
	}

	public bool IsQuitBluestacksChecked
	{
		get
		{
			return mIsQuitBluestacksChecked;
		}
		set
		{
			mIsQuitBluestacksChecked = value;
			NotifyPropertyChanged("IsQuitBluestacksChecked");
		}
	}

	public bool IsMinimizeBlueStacksRadioBtnChecked
	{
		get
		{
			return mIsMinimizeBlueStacksRadioBtnChecked;
		}
		set
		{
			mIsMinimizeBlueStacksRadioBtnChecked = value;
			NotifyPropertyChanged("IsMinimizeBlueStacksRadioBtnChecked");
		}
	}

	public static Dictionary<string, string> LocaleModel => BlueStacksUIBinding.Instance.LocaleModel;

	public static Dictionary<string, Brush> ColorModel => BlueStacksUIBinding.Instance.ColorModel;

	public ICommand CloseControlCommand { get; set; }

	public ICommand MinimizeCommand { get; set; }

	public ICommand QuitCommand { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	private void NotifyPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public MinimizeBlueStacksOnCloseViewModel(MainWindow window)
	{
		ParentWindow = window;
		Init();
	}

	private void Init()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		CloseControlCommand = (ICommand)new RelayCommand<UserControl>((Action<UserControl>)Close, false);
		MinimizeCommand = (ICommand)new RelayCommand<UserControl>((Action<UserControl>)MinimizeBluestacksHandler, false);
		QuitCommand = (ICommand)new RelayCommand((Action)QuitBlueStacks, false);
		if (ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup)
		{
			IsMinimizeBlueStacksRadioBtnChecked = true;
		}
		else
		{
			IsQuitBluestacksChecked = true;
		}
	}

	private void DoNotShowAgainPromptHandler()
	{
		if (IsDoNotShowAgainChkBoxChecked)
		{
			ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = false;
		}
	}

	private void QuitBlueStacks()
	{
		Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_exit_popup", ParentWindow.mVmName, "", "", "");
		ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
		DoNotShowAgainPromptHandler();
		ParentWindow.CloseWindowHandler();
	}

	private void MinimizeBluestacksHandler(UserControl control)
	{
		Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_minimize_popup", ParentWindow.mVmName, "", "", "");
		ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
		DoNotShowAgainPromptHandler();
		Close(control);
		ParentWindow.MinimizeWindowHandler();
	}

	private void Close(UserControl control)
	{
		try
		{
			BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)control);
			ParentWindow.HideDimOverlay();
			((UIElement)control).Visibility = (Visibility)1;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
		}
	}
}
