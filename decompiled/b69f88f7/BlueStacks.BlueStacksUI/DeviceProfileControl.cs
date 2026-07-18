using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class DeviceProfileControl : UserControl, IComponentConnector
{
	private JObject mCurrentDeviceProfileObject;

	private Dictionary<string, string> mPreDefinedProfilesList = new Dictionary<string, string>();

	private Dictionary<string, ComboBoxItem> mDeviceProfileComboBoxItems = new Dictionary<string, ComboBoxItem>();

	private Dictionary<string, string> mMobileOperatorsList = new Dictionary<string, string>();

	private Dictionary<string, ComboBoxItem> mMobileOperatorComboboxItems = new Dictionary<string, ComboBoxItem>();

	private CustomToastPopupControl mToastPopup;

	private MainWindow ParentWindow;

	private bool mGettingProfilesFromCloud;

	private bool mCurrentRootAccessStatus;

	private bool mIsProfileChanged;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mScrollBar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mProfileLoader;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mNoInternetWarning;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mChildGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mChooseProfile;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mPredefinedProfilesComboBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomRadioButton mCustomProfile;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mCustomProfileGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mManufacturerTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mBrandTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomTextBox mModelNumberTextBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mTryAgainBtnGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mMobileOperatorGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMobileOpertorText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock mMobileNetworkSetupText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomComboBox mMobileOperatorsCombobox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid mRootAccessGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mEnableRootAccessCheckBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mInfoIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mSaveChangesBtn;

	private bool _contentLoaded;

	public DeviceProfileControl(MainWindow window)
	{
		InitializeComponent();
		ParentWindow = window;
		Init();
	}

	public void Init()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		((UIElement)this).Visibility = (Visibility)1;
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(DeviceProfileControl_IsVisibleChanged);
		((TextBoxBase)mManufacturerTextBox).TextChanged += new TextChangedEventHandler(MManufacturerTextBox_TextChanged);
		((TextBoxBase)mModelNumberTextBox).TextChanged += new TextChangedEventHandler(MManufacturerTextBox_TextChanged);
		((TextBoxBase)mBrandTextBox).TextChanged += new TextChangedEventHandler(MManufacturerTextBox_TextChanged);
		if (PromotionObject.Instance.IsRootAccessEnabled || FeatureManager.Instance.IsCustomUIForNCSoft)
		{
			((UIElement)mRootAccessGrid).Visibility = (Visibility)0;
			mCurrentRootAccessStatus = GetRootAccessStatusFromAndroid(ParentWindow?.mVmName);
			((ToggleButton)mEnableRootAccessCheckBox).IsChecked = mCurrentRootAccessStatus;
		}
		mScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
		mGettingProfilesFromCloud = false;
	}

	private static bool GetRootAccessStatusFromAndroid(string vmname)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "d", "bst.config.bindmount" } };
			JObject val = JObject.Parse(HTTPUtils.SendRequestToGuest("getprop", dictionary, vmname, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64"));
			if (string.Equals(((object)val["result"]).ToString(), "ok", StringComparison.InvariantCulture))
			{
				if (string.Equals(((object)val["value"]).ToString(), "1", StringComparison.InvariantCulture))
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in Getting root status from android: " + ex.ToString());
			return false;
		}
	}

	private void ChangeLoadingGridVisibility(bool state)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (state)
			{
				((UIElement)mProfileLoader).Visibility = (Visibility)0;
				((UIElement)mNoInternetWarning).Visibility = (Visibility)2;
				((UIElement)mChildGrid).Visibility = (Visibility)2;
				((UIElement)mMobileOperatorGrid).Visibility = (Visibility)2;
				((UIElement)mTryAgainBtnGrid).Visibility = (Visibility)2;
			}
			else
			{
				((UIElement)mProfileLoader).Visibility = (Visibility)2;
				((UIElement)mNoInternetWarning).Visibility = (Visibility)2;
				((UIElement)mChildGrid).Visibility = (Visibility)0;
				if (RegistryManager.Instance.IsCacodeValid)
				{
					((UIElement)mMobileOperatorGrid).Visibility = (Visibility)0;
				}
				((UIElement)mTryAgainBtnGrid).Visibility = (Visibility)2;
			}
		}, new object[0]);
	}

	private void ChangeNoInternetGridVisibility(bool state)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			if (state)
			{
				((UIElement)mProfileLoader).Visibility = (Visibility)2;
				((UIElement)mNoInternetWarning).Visibility = (Visibility)0;
				((UIElement)mChildGrid).Visibility = (Visibility)2;
				((UIElement)mMobileOperatorGrid).Visibility = (Visibility)2;
				((UIElement)mTryAgainBtnGrid).Visibility = (Visibility)0;
			}
			else
			{
				((UIElement)mProfileLoader).Visibility = (Visibility)0;
				((UIElement)mNoInternetWarning).Visibility = (Visibility)2;
				((UIElement)mChildGrid).Visibility = (Visibility)2;
				((UIElement)mMobileOperatorGrid).Visibility = (Visibility)2;
				((UIElement)mTryAgainBtnGrid).Visibility = (Visibility)2;
			}
		}, new object[0]);
	}

	private void DeviceProfileControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
	{
		if (((UIElement)this).IsVisible)
		{
			if (!mGettingProfilesFromCloud)
			{
				mGettingProfilesFromCloud = true;
				ChangeLoadingGridVisibility(state: true);
				ChangeNoInternetGridVisibility(state: false);
				GetPreDefinedProfilesFromCloud();
			}
		}
		else
		{
			((UIElement)mSaveChangesBtn).IsEnabled = false;
			((ToggleButton)mEnableRootAccessCheckBox).IsChecked = mCurrentRootAccessStatus;
		}
	}

	private void SetUIAccordingToCurrentDeviceProfile()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		((Selector)mPredefinedProfilesComboBox).SelectionChanged -= new SelectionChangedEventHandler(mPredefinedProfilesComboBox_SelectionChanged);
		((Selector)mMobileOperatorsCombobox).SelectionChanged -= new SelectionChangedEventHandler(MobileOperatorsCombobox_SelectionChanged);
		if (mCurrentDeviceProfileObject == null)
		{
			((UIElement)mPredefinedProfilesComboBox).Visibility = (Visibility)0;
			((UIElement)mCustomProfileGrid).Visibility = (Visibility)2;
		}
		else
		{
			if (string.Equals(((object)mCurrentDeviceProfileObject["pcode"])?.ToString(), "custom", StringComparison.InvariantCulture))
			{
				((UIElement)mPredefinedProfilesComboBox).Visibility = (Visibility)2;
				((UIElement)mCustomProfileGrid).Visibility = (Visibility)0;
				((TextBox)mModelNumberTextBox).Text = ((object)mCurrentDeviceProfileObject["model"]).ToString();
				((TextBox)mBrandTextBox).Text = ((object)mCurrentDeviceProfileObject["brand"]).ToString();
				((TextBox)mManufacturerTextBox).Text = ((object)mCurrentDeviceProfileObject["manufacturer"]).ToString();
				((ToggleButton)mCustomProfile).IsChecked = true;
				((Selector)mPredefinedProfilesComboBox).SelectedItem = null;
			}
			else
			{
				((UIElement)mPredefinedProfilesComboBox).Visibility = (Visibility)0;
				((UIElement)mCustomProfileGrid).Visibility = (Visibility)2;
				if (mDeviceProfileComboBoxItems.ContainsKey(((object)mCurrentDeviceProfileObject["pcode"])?.ToString()))
				{
					((Selector)mPredefinedProfilesComboBox).SelectedItem = mDeviceProfileComboBoxItems[((object)mCurrentDeviceProfileObject["pcode"]).ToString()];
				}
				((ToggleButton)mChooseProfile).IsChecked = true;
				((TextBox)mModelNumberTextBox).Text = string.Empty;
				((TextBox)mBrandTextBox).Text = string.Empty;
				((TextBox)mManufacturerTextBox).Text = string.Empty;
			}
			if (mMobileOperatorComboboxItems.ContainsKey(((object)mCurrentDeviceProfileObject["caSelector"])?.ToString()))
			{
				((Selector)mMobileOperatorsCombobox).SelectedItem = mMobileOperatorComboboxItems[((object)mCurrentDeviceProfileObject["caSelector"]).ToString()];
			}
		}
		((Selector)mMobileOperatorsCombobox).SelectionChanged += new SelectionChangedEventHandler(MobileOperatorsCombobox_SelectionChanged);
		((Selector)mPredefinedProfilesComboBox).SelectionChanged += new SelectionChangedEventHandler(mPredefinedProfilesComboBox_SelectionChanged);
		ChangeLoadingGridVisibility(state: false);
	}

	private void GetPreDefinedProfilesFromCloud()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Expected O, but got Unknown
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Expected O, but got Unknown
			try
			{
				GetCurrentDeviceProfileFromAndroid(ParentWindow.mVmName);
				if (mPreDefinedProfilesList.Count == 0 || mMobileOperatorsList.Count == 0)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[3]
					{
						RegistryManager.Instance.Host,
						"bs4",
						"get_device_profile_list"
					});
					Dictionary<string, string> commonPOSTData = WebHelper.GetCommonPOSTData();
					commonPOSTData.Add("ca_code", Utils.GetValueInBootParams("caCode", ParentWindow.mVmName, "", "bgp64"));
					JObject val = JObject.Parse(BstHttpClient.Post(text, commonPOSTData, (Dictionary<string, string>)null, false, ParentWindow.mVmName, 0, 1, 0, false, "bgp64"));
					if (val != null && (bool)val["success"])
					{
						if (!JsonExtensions.IsNullOrEmptyBrackets(((object)val["device_profile_list"]).ToString()))
						{
							JToken[] array = ((IEnumerable<JToken>)val["device_profile_list"]).ToArray();
							for (int i = 0; i < array.Length; i++)
							{
								JObject val2 = (JObject)array[i];
								mPreDefinedProfilesList[((object)val2["pcode"]).ToString()] = ((object)val2["display_name"]).ToString();
							}
						}
						if (val.ContainsKey("ca_selector_list") && !JsonExtensions.IsNullOrEmptyBrackets(((object)val["ca_selector_list"]).ToString()))
						{
							JToken[] array = ((IEnumerable<JToken>)val["ca_selector_list"]).ToArray();
							for (int i = 0; i < array.Length; i++)
							{
								JObject val3 = (JObject)array[i];
								mMobileOperatorsList[((object)val3["ca_selector"]).ToString()] = ((object)val3["display_name"]).ToString();
							}
						}
						AddPreDefinedProfilesinComboBox();
					}
				}
				else
				{
					AddPreDefinedProfilesinComboBox();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting device profile from cloud : " + ex.ToString());
				ChangeNoInternetGridVisibility(state: true);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	internal void GetCurrentDeviceProfileFromAndroid(string vmName)
	{
		if (string.Equals(VmCmdHandler.SendRequest("currentdeviceprofile", (Dictionary<string, string>)null, vmName, ref mCurrentDeviceProfileObject), "ok", StringComparison.InvariantCulture))
		{
			mCurrentDeviceProfileObject.Remove("result");
		}
	}

	private void AddPreDefinedProfilesinComboBox()
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected O, but got Unknown
			foreach (string key in mPreDefinedProfilesList.Keys)
			{
				ComboBoxItem val = new ComboBoxItem
				{
					Content = mPreDefinedProfilesList[key]
				};
				((ItemsControl)mPredefinedProfilesComboBox).Items.Add((object)val);
				if (mDeviceProfileComboBoxItems.ContainsKey(key))
				{
					mDeviceProfileComboBoxItems[key] = val;
				}
				else
				{
					mDeviceProfileComboBoxItems.Add(key, val);
				}
			}
			foreach (string key2 in mMobileOperatorsList.Keys)
			{
				ComboBoxItem val2 = new ComboBoxItem
				{
					Content = mMobileOperatorsList[key2]
				};
				((ItemsControl)mMobileOperatorsCombobox).Items.Add((object)val2);
				mMobileOperatorComboboxItems[key2] = val2;
			}
			SetUIAccordingToCurrentDeviceProfile();
		}, new object[0]);
	}

	private void Profile_Checked(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mChooseProfile).IsChecked.Value)
		{
			((UIElement)mPredefinedProfilesComboBox).Visibility = (Visibility)0;
			((UIElement)mCustomProfileGrid).Visibility = (Visibility)2;
			mIsProfileChanged = (string.Equals(((object)mCurrentDeviceProfileObject["pcode"])?.ToString(), "custom", StringComparison.InvariantCulture) ? true : false);
		}
		else if (((ToggleButton)mCustomProfile).IsChecked.Value)
		{
			((UIElement)mPredefinedProfilesComboBox).Visibility = (Visibility)2;
			((UIElement)mCustomProfileGrid).Visibility = (Visibility)0;
			mIsProfileChanged = !string.Equals(((object)mCurrentDeviceProfileObject["pcode"])?.ToString(), "custom", StringComparison.InvariantCulture);
		}
	}

	private void mPredefinedProfilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		string jsonString;
		JObject changedDeviceProfileObject = GetChangedDeviceProfileObject(out jsonString);
		((UIElement)mSaveChangesBtn).IsEnabled = !JToken.DeepEquals((JToken)(object)mCurrentDeviceProfileObject, (JToken)(object)changedDeviceProfileObject) || ((ToggleButton)mEnableRootAccessCheckBox).IsChecked != mCurrentRootAccessStatus;
	}

	private void MManufacturerTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		string jsonString;
		JObject changedDeviceProfileObject = GetChangedDeviceProfileObject(out jsonString);
		((UIElement)mSaveChangesBtn).IsEnabled = !JToken.DeepEquals((JToken)(object)mCurrentDeviceProfileObject, (JToken)(object)changedDeviceProfileObject) || ((ToggleButton)mEnableRootAccessCheckBox).IsChecked != mCurrentRootAccessStatus;
	}

	private JObject GetChangedDeviceProfileObject(out string jsonString)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		jsonString = "{";
		JObject val = new JObject();
		string text = ((object)mCurrentDeviceProfileObject["pcode"]).ToString();
		string text2 = ((object)mCurrentDeviceProfileObject["caSelector"]).ToString();
		if (((ToggleButton)mChooseProfile).IsChecked.Value)
		{
			if (((Selector)mPredefinedProfilesComboBox).SelectedItem != null)
			{
				object selectedItem = ((Selector)mPredefinedProfilesComboBox).SelectedItem;
				string selectedDeviceProfile = ((ContentControl)((selectedItem is ComboBoxItem) ? selectedItem : null)).Content.ToString();
				text = mPreDefinedProfilesList.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedDeviceProfile).Key;
			}
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[1] { "false" });
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"pcode\":\"{0}\",", new object[1] { text });
			val["pcode"] = JToken.op_Implicit(text);
		}
		else
		{
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[1] { "true" });
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"model\":\"{0}\",", new object[1] { ((TextBox)mModelNumberTextBox).Text });
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", new object[1] { ((TextBox)mBrandTextBox).Text });
			jsonString += string.Format(CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\",", new object[1] { ((TextBox)mManufacturerTextBox).Text });
			val["pcode"] = JToken.op_Implicit("custom");
			val["model"] = JToken.op_Implicit(((TextBox)mModelNumberTextBox).Text);
			val["brand"] = JToken.op_Implicit(((TextBox)mBrandTextBox).Text);
			val["manufacturer"] = JToken.op_Implicit(((TextBox)mManufacturerTextBox).Text);
		}
		if (((Selector)mMobileOperatorsCombobox).SelectedItem != null)
		{
			object selectedItem2 = ((Selector)mMobileOperatorsCombobox).SelectedItem;
			string selectedMobileOperator = ((ContentControl)((selectedItem2 is ComboBoxItem) ? selectedItem2 : null)).Content.ToString();
			if (!string.IsNullOrEmpty(selectedMobileOperator))
			{
				text2 = mMobileOperatorsList.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedMobileOperator).Key;
			}
		}
		jsonString += string.Format(CultureInfo.InvariantCulture, "\"caSelector\":\"{0}\"", new object[1] { text2 });
		jsonString += "}";
		val.Add("caSelector", JToken.op_Implicit(text2));
		return val;
	}

	private void SaveChangesBtn_Click(object sender, RoutedEventArgs e)
	{
		((UIElement)mSaveChangesBtn).IsEnabled = false;
		mIsProfileChanged = false;
		string jsonString;
		JObject changedDeviceProfileObject = GetChangedDeviceProfileObject(out jsonString);
		SendDeviceProfileChangeToGuest(jsonString, changedDeviceProfileObject);
		if (((ToggleButton)mEnableRootAccessCheckBox).IsChecked == mCurrentRootAccessStatus)
		{
			return;
		}
		string res = null;
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				if (!mCurrentRootAccessStatus)
				{
					res = HTTPUtils.SendRequestToGuest("bindmount", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				else
				{
					res = HTTPUtils.SendRequestToGuest("unbindmount", (Dictionary<string, string>)null, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
				}
				if (string.Equals(((object)JObject.Parse(res)["result"]).ToString(), "ok", StringComparison.InvariantCulture))
				{
					AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
					mCurrentRootAccessStatus = !mCurrentRootAccessStatus;
					SendStatsOfRootAccessStatusAsync("success", mCurrentRootAccessStatus);
					if (((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(ParentWindow.mVmName) && mCurrentRootAccessStatus)
					{
						((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
					}
				}
				else
				{
					AddToastPopup(LocaleStrings.GetLocalizedString("STRING_ROOT_ACCESS_FAILURE", ""));
					((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						((ToggleButton)mEnableRootAccessCheckBox).IsChecked = mCurrentRootAccessStatus;
					}, new object[0]);
					SendStatsOfRootAccessStatusAsync("failed", mCurrentRootAccessStatus);
				}
				ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Advanced-Settings", "", null, ParentWindow.mVmName);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending mount unmount request to Android: " + ex.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void AddToastPopup(string message)
	{
		((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (mToastPopup == null)
			{
				mToastPopup = new CustomToastPopupControl((UserControl)(object)this);
			}
			Thickness value = default(Thickness);
			((Thickness)(ref value))._002Ector(0.0, 0.0, 0.0, 50.0);
			mToastPopup.Init((UserControl)(object)this, message, (Brush)null, (Brush)null, (HorizontalAlignment)1, (VerticalAlignment)2, (Thickness?)value, 12, (Thickness?)null, (Brush)null);
			mToastPopup.ShowPopup(1.3);
		}, new object[0]);
	}

	private static void SendStatsOfDeviceProfileChangeAsync(string successString, JObject newDeviceProfile, JObject oldDeviceProfile)
	{
		ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, successString, JsonConvert.SerializeObject((object)newDeviceProfile), JsonConvert.SerializeObject((object)oldDeviceProfile), RegistryManager.Instance.Version, "DeviceProfileSetting");
	}

	private void SendStatsOfRootAccessStatusAsync(string successString, bool rootedstatus)
	{
		string arg = (rootedstatus ? "Rooted" : "Unrooted");
		ClientStats.SendMiscellaneousStatsAsync("DeviceRootingStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, successString, arg, ParentWindow.mVmName);
	}

	private void TryAgainBtn_Click(object sender, RoutedEventArgs e)
	{
		ChangeNoInternetGridVisibility(state: false);
		ChangeLoadingGridVisibility(state: true);
		GetPreDefinedProfilesFromCloud();
	}

	private void mEnableRootAccessCheckBox_Click(object sender, RoutedEventArgs e)
	{
		string jsonString;
		JObject changedDeviceProfileObject = GetChangedDeviceProfileObject(out jsonString);
		((UIElement)mSaveChangesBtn).IsEnabled = !JToken.DeepEquals((JToken)(object)mCurrentDeviceProfileObject, (JToken)(object)changedDeviceProfileObject) || ((ToggleButton)mEnableRootAccessCheckBox).IsChecked != mCurrentRootAccessStatus;
	}

	private void SendDeviceProfileChangeToGuest(string json, JObject changedDeviceProfileObject)
	{
		if (!Utils.CheckIfDeviceProfileChanged(mCurrentDeviceProfileObject, changedDeviceProfileObject))
		{
			return;
		}
		string command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2] { "changeDeviceProfile", json });
		Logger.Info("Command for device profile change: " + command);
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				string text = VmCmdHandler.RunCommand(command, ParentWindow.mVmName);
				Logger.Info("Result for device profile change command: " + text);
				if (string.Equals(text, "ok", StringComparison.InvariantCulture))
				{
					AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
					SendStatsOfDeviceProfileChangeAsync("success", changedDeviceProfileObject, mCurrentDeviceProfileObject);
					mCurrentDeviceProfileObject = changedDeviceProfileObject;
					Utils.UpdateValueInBootParams("pcode", ((object)changedDeviceProfileObject["pcode"]).ToString(), ParentWindow.mVmName, false, "bgp64");
					Utils.UpdateValueInBootParams("caSelector", ((object)changedDeviceProfileObject["caSelector"]).ToString(), ParentWindow.mVmName, false, "bgp64");
					if (((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList).ContainsKey(ParentWindow.mVmName))
					{
						((Dictionary<string, SecurityMetrics>)(object)SecurityMetrics.SecurityMetricsInstanceList)[ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROFILE_CHANGED, string.Empty);
					}
				}
				else
				{
					AddToastPopup(LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""));
					SendStatsOfDeviceProfileChangeAsync("failed", changedDeviceProfileObject, mCurrentDeviceProfileObject);
				}
				((DispatcherObject)this).Dispatcher.Invoke((Delegate)(Action)delegate
				{
					SetUIAccordingToCurrentDeviceProfile();
				}, new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in change to predefined Pcode call to android: " + ex.ToString());
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void MobileOperatorsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		string jsonString;
		JObject changedDeviceProfileObject = GetChangedDeviceProfileObject(out jsonString);
		((UIElement)mSaveChangesBtn).IsEnabled = !JToken.DeepEquals((JToken)(object)mCurrentDeviceProfileObject, (JToken)(object)changedDeviceProfileObject) || ((ToggleButton)mEnableRootAccessCheckBox).IsChecked != mCurrentRootAccessStatus;
	}

	public bool IsDirty()
	{
		if (((UIElement)mSaveChangesBtn).IsEnabled || mIsProfileChanged)
		{
			return true;
		}
		return false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/deviceprofilecontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mScrollBar = (ScrollViewer)target;
			break;
		case 2:
			mProfileLoader = (Border)target;
			break;
		case 3:
			mNoInternetWarning = (Border)target;
			break;
		case 4:
			mChildGrid = (Grid)target;
			break;
		case 5:
			mChooseProfile = (CustomRadioButton)target;
			((ToggleButton)mChooseProfile).Checked += new RoutedEventHandler(Profile_Checked);
			break;
		case 6:
			mPredefinedProfilesComboBox = (CustomComboBox)target;
			((Selector)mPredefinedProfilesComboBox).SelectionChanged += new SelectionChangedEventHandler(mPredefinedProfilesComboBox_SelectionChanged);
			break;
		case 7:
			mCustomProfile = (CustomRadioButton)target;
			((ToggleButton)mCustomProfile).Checked += new RoutedEventHandler(Profile_Checked);
			break;
		case 8:
			mCustomProfileGrid = (Grid)target;
			break;
		case 9:
			mManufacturerTextBox = (CustomTextBox)target;
			break;
		case 10:
			mBrandTextBox = (CustomTextBox)target;
			break;
		case 11:
			mModelNumberTextBox = (CustomTextBox)target;
			break;
		case 12:
			mTryAgainBtnGrid = (Grid)target;
			break;
		case 13:
			((ButtonBase)(CustomButton)target).Click += new RoutedEventHandler(TryAgainBtn_Click);
			break;
		case 14:
			mMobileOperatorGrid = (Grid)target;
			break;
		case 15:
			mMobileOpertorText = (TextBlock)target;
			break;
		case 16:
			mMobileNetworkSetupText = (TextBlock)target;
			break;
		case 17:
			mMobileOperatorsCombobox = (CustomComboBox)target;
			((Selector)mMobileOperatorsCombobox).SelectionChanged += new SelectionChangedEventHandler(MobileOperatorsCombobox_SelectionChanged);
			break;
		case 18:
			mRootAccessGrid = (Grid)target;
			break;
		case 19:
			mEnableRootAccessCheckBox = (CustomCheckbox)target;
			((ButtonBase)mEnableRootAccessCheckBox).Click += new RoutedEventHandler(mEnableRootAccessCheckBox_Click);
			break;
		case 20:
			mInfoIcon = (CustomPictureBox)target;
			break;
		case 21:
			mSaveChangesBtn = (CustomButton)target;
			((ButtonBase)mSaveChangesBtn).Click += new RoutedEventHandler(SaveChangesBtn_Click);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
