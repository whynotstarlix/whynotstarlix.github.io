using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using nspector.Common;
using nspector.Common.CustomSettings;
using nspector.Common.Helper;
using nspector.Common.Meta;
using nspector.Native.NVAPI2;
using nspector.Native.WINAPI;
using nspector.Properties;

namespace nspector;

internal class frmDrvSettings : Form
{
	private readonly DrsSettingsMetaService _meta = DrsServiceLocator.MetaService;

	private readonly DrsSettingsService _drs = DrsServiceLocator.SettingService;

	private readonly DrsScannerService _scanner = DrsServiceLocator.ScannerService;

	private readonly DrsImportService _import = DrsServiceLocator.ImportService;

	private List<SettingItem> _currentProfileSettingItems = new List<SettingItem>();

	private bool _alreadyScannedForPredefinedSettings = false;

	private IntPtr _taskbarParent = IntPtr.Zero;

	private bool _activated = false;

	private bool _isStartup = true;

	private bool _skipScan = false;

	private string _baseProfileName = "";

	private bool _isWin7TaskBar = false;

	private int _lastComboRowIndex = -1;

	private ITaskbarList3 _taskbarList;

	public string _CurrentProfile = "";

	private bool _isDevMode = false;

	private UserSettings _settings = null;

	public static double ScaleFactor = 1.0;

	private ToolTip appPathsTooltip = new ToolTip
	{
		InitialDelay = 250
	};

	private CancellationTokenSource _scannerCancelationTokenSource;

	private IContainer components = null;

	private ListViewEx lvSettings;

	private ColumnHeader chSettingID;

	private ColumnHeader chSettingValue;

	private ColumnHeader chSettingValueHex;

	private ImageList ilListView;

	private ComboBox cbValues;

	private Button btnResetValue;

	private ProgressBar pbMain;

	private ToolStrip tsMain;

	private ToolStripButton tsbRestoreProfile;

	private ToolStripButton tsbApplyProfile;

	private ToolStripButton tsbRefreshProfile;

	private ToolStripSeparator tsSep3;

	private ToolStripButton tsbBitValueEditor;

	private ToolStripSeparator tsSep6;

	private ToolStripButton tscbShowCustomSettingNamesOnly;

	private ToolStripSeparator tsSep5;

	private ToolStripButton tscbShowScannedUnknownSettings;

	private ToolStripLabel tslProfiles;

	private Label lblApplications;

	private ToolStripButton toolStripButton5;

	private ToolStripLabel toolStripLabel2;

	private ToolStripButton toolStripButton6;

	private ToolStripSeparator tsSep2;

	private ToolStripButton tsbDeleteProfile;

	private ToolStripButton tsbCreateProfile;

	private ToolStripButton tsbAddApplication;

	private ToolStripSplitButton tssbRemoveApplication;

	private ToolStripSeparator tsSep4;

	private ToolStripSplitButton tsbExportProfiles;

	private ToolStripMenuItem exportCurrentProfileOnlyToolStripMenuItem;

	private ToolStripMenuItem exportUserdefinedProfilesToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	public ToolStripComboBox cbProfiles;

	private ToolStripSplitButton tsbModifiedProfiles;

	private ImageList ilCombo;

	private ToolStripMenuItem exportAllProfilesNVIDIATextFormatToolStripMenuItem;

	private ToolStripSplitButton tsbImportProfiles;

	private ToolStripMenuItem importProfilesToolStripMenuItem;

	private ToolStripMenuItem importAllProfilesNVIDIATextFormatToolStripMenuItem;

	private Label lblWidth96;

	private Label lblWidth330;

	private Label lblWidth16;

	private Label lblWidth30;

	private ToolStripMenuItem exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem;

	private TextBox tbSettingDescription;

	private Panel pnlListview;

	private WatermarkTextBox txtFilter;

	protected override void WndProc(ref Message m)
	{
		int msg = ((Message)(ref m)).Msg;
		int num = msg;
		if (num == 74)
		{
			Type type = default(MessageHelper.COPYDATASTRUCT).GetType();
			if (((MessageHelper.COPYDATASTRUCT)((Message)(ref m)).GetLParam(type)).lpData.Equals("ProfilesImported"))
			{
				DrsSessionScope.DestroyGlobalSession();
				RefreshAll();
			}
		}
		((Form)this).WndProc(ref m);
	}

	private ListViewGroup FindOrCreateGroup(string groupName)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		if (string.IsNullOrEmpty(groupName))
		{
			groupName = "Unknown";
		}
		foreach (ListViewGroup group in ((ListView)lvSettings).Groups)
		{
			ListViewGroup val = group;
			if (val.Header == groupName)
			{
				return val;
			}
		}
		ListViewGroup val2 = new ListViewGroup(groupName);
		((ListView)lvSettings).Groups.Insert(0, val2);
		return val2;
	}

	private ListViewItem CreateListViewItem(SettingItem setting)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		ListViewGroup val = FindOrCreateGroup(setting.GroupName);
		string text = (_isDevMode ? $"0x{setting.SettingId:X8} {setting.SettingText}" : setting.SettingText);
		if (setting.IsSettingHidden)
		{
			text = "[H] " + text;
		}
		ListViewItem val2 = new ListViewItem(text);
		val2.Tag = setting.SettingId;
		val2.Group = val;
		val2.SubItems.Add(setting.ValueText);
		val2.SubItems.Add(setting.ValueRaw);
		switch (setting.State)
		{
		default:
			val2.ImageIndex = 1;
			val2.ForeColor = SystemColors.GrayText;
			break;
		case SettingState.NvidiaSetting:
			val2.ImageIndex = 2;
			break;
		case SettingState.GlobalSetting:
			val2.ImageIndex = 3;
			val2.ForeColor = SystemColors.GrayText;
			break;
		case SettingState.UserdefinedSetting:
			val2.ImageIndex = 0;
			break;
		}
		return val2;
	}

	private void RefreshApplicationsCombosAndText(Dictionary<string, string> applications)
	{
		((Control)lblApplications).Text = "";
		((ToolStripDropDownItem)tssbRemoveApplication).DropDownItems.Clear();
		((Control)lblApplications).Text = " " + string.Join(", ", applications.Select((KeyValuePair<string, string> x) => x.Value));
		foreach (KeyValuePair<string, string> application in applications)
		{
			ToolStripItem val = ((ToolStripDropDownItem)tssbRemoveApplication).DropDownItems.Add(application.Value, (Image)(object)Resources.ieframe_1_18212);
			val.Tag = application.Key;
		}
		((ToolStripItem)tssbRemoveApplication).Enabled = ((ArrangedElementCollection)((ToolStripDropDownItem)tssbRemoveApplication).DropDownItems).Count > 0;
	}

	private SettingViewMode GetSettingViewMode()
	{
		if (tscbShowCustomSettingNamesOnly.Checked)
		{
			return SettingViewMode.CustomSettingsOnly;
		}
		if (tscbShowScannedUnknownSettings.Checked)
		{
			return SettingViewMode.IncludeScannedSetttings;
		}
		return SettingViewMode.Normal;
	}

	private void RefreshCurrentProfile()
	{
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		string text = "";
		if (((ListView)lvSettings).SelectedItems.Count > 0)
		{
			text = ((ListView)lvSettings).SelectedItems[0].Text;
		}
		((ListView)lvSettings).BeginUpdate();
		try
		{
			((ListView)lvSettings).Items.Clear();
			((ListView)lvSettings).Groups.Clear();
			Dictionary<string, string> applications = new Dictionary<string, string>();
			_currentProfileSettingItems = _drs.GetSettingsForProfile(_CurrentProfile, GetSettingViewMode(), ref applications);
			RefreshApplicationsCombosAndText(applications);
			string value = ((Control)txtFilter).Text.Trim();
			foreach (SettingItem currentProfileSettingItem in _currentProfileSettingItems)
			{
				if (currentProfileSettingItem.IsSettingHidden && !_isDevMode)
				{
					continue;
				}
				ListViewItem val = CreateListViewItem(currentProfileSettingItem);
				if (string.IsNullOrEmpty(value) || val.Text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0 || (currentProfileSettingItem.AlternateNames != null && currentProfileSettingItem.AlternateNames.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0))
				{
					((ListView)lvSettings).Items.Add(val);
					if (Debugger.IsAttached && !currentProfileSettingItem.IsApiExposed)
					{
						val.ForeColor = Color.LightCoral;
					}
				}
			}
			((Control)btnResetValue).Enabled = false;
			try
			{
				lvSettings.RemoveEmbeddedControl((Control)(object)cbValues);
				lvSettings.RemoveEmbeddedControl((Control)(object)btnResetValue);
			}
			catch
			{
			}
		}
		finally
		{
			((ListViewGroupSorter)(ListView)(object)lvSettings).SortGroups(ascending: true);
			foreach (ListViewGroup group in ((ListView)lvSettings).Groups)
			{
				ListViewGroup val2 = group;
				if (_settings.HiddenSettingGroups.Contains(val2.Header))
				{
					lvSettings.SetGroupState(val2, (ListViewGroupState)9);
				}
				else
				{
					lvSettings.SetGroupState(val2, ListViewGroupState.Collapsible);
				}
			}
			((ListView)lvSettings).EndUpdate();
			GC.Collect();
			for (int i = 0; i < ((ListView)lvSettings).Items.Count; i++)
			{
				if (((ListView)lvSettings).Items[i].Text == text)
				{
					((ListView)lvSettings).Items[i].Selected = true;
					((ListView)lvSettings).Items[i].EnsureVisible();
					if (!((ToolStripControlHost)cbProfiles).Focused)
					{
						((Control)lvSettings).Select();
						((Control)cbValues).Text = ((ListView)lvSettings).Items[i].SubItems[1].Text;
					}
					break;
				}
			}
		}
	}

	private void RefreshProfilesCombo()
	{
		cbProfiles.Items.Clear();
		List<string> profileNames = _drs.GetProfileNames(ref _baseProfileName);
		cbProfiles.Items.AddRange(profileNames.Cast<object>().ToArray());
		cbProfiles.Sorted = true;
	}

	private void MoveComboToItemAndFill()
	{
		if (((ListView)lvSettings).SelectedItems.Count > 0)
		{
			if (((Control)cbValues).ContainsFocus || _lastComboRowIndex == ((ListView)lvSettings).SelectedItems[0].Index)
			{
				return;
			}
			((Control)btnResetValue).Enabled = true;
			cbValues.BeginUpdate();
			((ToolStripItem)tsbBitValueEditor).Enabled = false;
			cbValues.Items.Clear();
			((Control)cbValues).Tag = ((ListView)lvSettings).SelectedItems[0].Tag;
			uint settingid = (uint)((ListView)lvSettings).SelectedItems[0].Tag;
			SettingMeta settingMeta = _meta.GetSettingMeta(settingid, GetSettingViewMode());
			if (settingMeta != null)
			{
				if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE && settingMeta.DwordValues != null)
				{
					List<string> list = settingMeta.DwordValues.Select((SettingValue<uint> x) => x.ValueName).ToList();
					foreach (string item in list)
					{
						string text = "";
						text = ((item.Length <= 4000) ? item : (item.Substring(0, 4000) + " ..."));
						cbValues.Items.Add((object)text);
					}
					((ToolStripItem)tsbBitValueEditor).Enabled = list.Count > 0;
				}
				if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE && settingMeta.StringValues != null)
				{
					List<string> list2 = settingMeta.StringValues.Select((SettingValue<string> x) => x.ValueName).ToList();
					foreach (string item2 in list2)
					{
						cbValues.Items.Add((object)item2);
					}
				}
				if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE && settingMeta.BinaryValues != null)
				{
					List<string> list3 = settingMeta.BinaryValues.Select((SettingValue<byte[]> x) => x.ValueName).ToList();
					foreach (string item3 in list3)
					{
						cbValues.Items.Add((object)item3);
					}
				}
			}
			if (cbValues.Items.Count < 1)
			{
				cbValues.Items.Add((object)"");
				cbValues.Items.RemoveAt(0);
			}
			CustomSetting customSetting = DrsServiceLocator.ReferenceSettings?.Settings.FirstOrDefault((CustomSetting s) => s.SettingId == settingid);
			string text2 = DlssHelper.ReplaceDlssVersions(settingMeta.Description);
			if (!string.IsNullOrEmpty(settingMeta.AlternateNames))
			{
				text2 = "Alternate names: " + settingMeta.AlternateNames + "\r\n" + text2;
			}
			if (string.IsNullOrEmpty(text2) && !(customSetting?.HasConstraints ?? false))
			{
				((Control)tbSettingDescription).Text = "";
				((Control)tbSettingDescription).Visible = false;
				((Control)tbSettingDescription).BackColor = SystemColors.Control;
			}
			else
			{
				((Control)tbSettingDescription).Text = text2.Replace("\\r\\n", "\r\n");
				((Control)tbSettingDescription).Visible = true;
				((Control)tbSettingDescription).BackColor = ((customSetting != null && customSetting.HasConstraints) ? Color.LightCoral : SystemColors.Control);
			}
			((Control)cbValues).Text = ((ListView)lvSettings).SelectedItems[0].SubItems[1].Text;
			cbValues.EndUpdate();
			lvSettings.AddEmbeddedControl((Control)(object)cbValues, 1, ((ListView)lvSettings).SelectedItems[0].Index);
			if (((ListView)lvSettings).SelectedItems[0].ImageIndex == 0)
			{
				lvSettings.AddEmbeddedControl((Control)(object)btnResetValue, 2, ((ListView)lvSettings).SelectedItems[0].Index, (DockStyle)4);
			}
			_lastComboRowIndex = ((ListView)lvSettings).SelectedItems[0].Index;
			((Control)cbValues).Visible = true;
			return;
		}
		_lastComboRowIndex = -1;
		if (!((Control)cbValues).ContainsFocus)
		{
			try
			{
				lvSettings.RemoveEmbeddedControl((Control)(object)cbValues);
				lvSettings.RemoveEmbeddedControl((Control)(object)btnResetValue);
			}
			catch
			{
			}
			((Control)btnResetValue).Enabled = false;
			((Control)cbValues).Visible = false;
			((ToolStripItem)tsbBitValueEditor).Enabled = false;
		}
	}

	private int GetListViewIndexOfSetting(uint settingId)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		int num = 0;
		foreach (ListViewItem item in ((ListView)lvSettings).Items)
		{
			ListViewItem val = item;
			if (settingId == (uint)val.Tag)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private void UpdateItemByComboValue()
	{
		uint settingId = (uint)((Control)cbValues).Tag;
		int[] source = new int[2] { 0, 2 };
		int listViewIndexOfSetting = GetListViewIndexOfSetting(settingId);
		if (listViewIndexOfSetting != -1)
		{
			ListViewItem val = ((ListView)lvSettings).Items[listViewIndexOfSetting];
			SettingMeta settingMeta = _meta.GetSettingMeta(settingId, GetSettingViewMode());
			SettingItem settingItem = _currentProfileSettingItems.First((SettingItem x) => x.SettingId.Equals(settingId));
			string text = ((Control)cbValues).Text.Trim();
			bool flag = settingItem.ValueText != text;
			if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
			{
				string text2 = DrsUtil.ParseStringSettingValue(settingMeta, text);
				flag = settingItem.ValueRaw != text2;
			}
			if (flag || Enumerable.Contains(source, val.ImageIndex))
			{
				val.ForeColor = SystemColors.ControlText;
			}
			else
			{
				val.ForeColor = SystemColors.GrayText;
			}
			if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
			{
				val.SubItems[2].Text = DrsUtil.GetDwordString(DrsUtil.ParseDwordSettingValue(settingMeta, text));
				val.SubItems[1].Text = text;
			}
			else if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_WSTRING_TYPE)
			{
				val.SubItems[2].Text = DrsUtil.ParseStringSettingValue(settingMeta, text);
				val.SubItems[1].Text = text;
			}
			else if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
			{
				val.SubItems[2].Text = DrsUtil.GetBinaryString(DrsUtil.ParseBinarySettingValue(settingMeta, text));
				val.SubItems[1].Text = text;
			}
		}
	}

	private void StoreChangesOfProfileToDriver()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		List<KeyValuePair<uint, string>> list = new List<KeyValuePair<uint, string>>();
		foreach (ListViewItem item in ((ListView)lvSettings).Items)
		{
			ListViewItem lvi = item;
			SettingItem settingItem = _currentProfileSettingItems.First((SettingItem x) => x.SettingId.Equals((uint)lvi.Tag));
			string text = lvi.SubItems[1].Text;
			bool flag = string.IsNullOrEmpty(text);
			bool flag2 = string.IsNullOrEmpty(settingItem.ValueText);
			if (settingItem.ValueText != text && !(flag && flag2))
			{
				list.Add(new KeyValuePair<uint, string>((uint)lvi.Tag, text));
			}
		}
		if (list.Count > 0)
		{
			_drs.StoreSettingsToProfile(_CurrentProfile, list);
			AddToModifiedProfiles(_CurrentProfile);
		}
		RefreshCurrentProfile();
	}

	private void ResetCurrentProfile()
	{
		bool removeFromModified = false;
		_drs.ResetProfile(_CurrentProfile, out removeFromModified);
		if (removeFromModified)
		{
			RemoveFromModifiedProfiles(_CurrentProfile);
		}
		RefreshCurrentProfile();
	}

	private void ResetSelectedValue()
	{
		if (((ListView)lvSettings).SelectedItems != null && ((ListView)lvSettings).SelectedItems.Count > 0)
		{
			uint settingId = (uint)((ListView)lvSettings).SelectedItems[0].Tag;
			_drs.ResetValue(_CurrentProfile, settingId, out var removeFromModified);
			if (removeFromModified)
			{
				RemoveFromModifiedProfiles(_CurrentProfile);
			}
			RefreshCurrentProfile();
		}
	}

	private void DeleteSelectedValue()
	{
		if (((ListView)lvSettings).SelectedItems != null && ((ListView)lvSettings).SelectedItems.Count > 0)
		{
			uint settingId = (uint)((ListView)lvSettings).SelectedItems[0].Tag;
			_drs.DeleteValue(_CurrentProfile, settingId, out var removeFromModified);
			if (removeFromModified)
			{
				RemoveFromModifiedProfiles(_CurrentProfile);
			}
			RefreshCurrentProfile();
		}
	}

	private void InitTaskbarList()
	{
		if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
		{
			try
			{
				_taskbarList = (ITaskbarList3)new TaskbarList();
				_taskbarList.HrInit();
				_taskbarParent = ((Control)this).Handle;
				_isWin7TaskBar = true;
			}
			catch
			{
				_taskbarList = null;
				_taskbarParent = IntPtr.Zero;
				_isWin7TaskBar = false;
			}
		}
	}

	private void SetTaskbarIcon()
	{
		if (_taskbarList != null && _isWin7TaskBar && AdminHelper.IsAdmin)
		{
			try
			{
				_taskbarList.SetOverlayIcon(_taskbarParent, Resources.shield16.Handle, "Elevated");
			}
			catch
			{
			}
		}
	}

	private void SetTitleVersion(bool isUpdateAvailable = false)
	{
		NumberFormatInfo numberFormatInfo = new NumberFormatInfo
		{
			NumberDecimalSeparator = "."
		};
		string text = Assembly.GetExecutingAssembly().GetName().Version.ToString() + (isUpdateAvailable ? " (update available on GitHub)" : "");
		FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
		string text2 = (DrsServiceLocator.IsExternalCustomSettings ? " - CSN OVERRIDE!" : "");
		string[] obj = new string[8]
		{
			Application.ProductName,
			" ",
			text,
			" - Geforce ",
			null,
			null,
			null,
			null
		};
		float driverVersion = DrsSettingsServiceBase.DriverVersion;
		obj[4] = driverVersion.ToString("#.00", numberFormatInfo);
		obj[5] = " - Profile Settings - ";
		obj[6] = versionInfo.LegalCopyright;
		obj[7] = text2;
		((Control)this).Text = string.Concat(obj);
	}

	private static void InitMessageFilter(IntPtr handle)
	{
		if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
		{
			DragAcceptNativeHelper.ChangeWindowMessageFilterEx(handle, 563, 1, IntPtr.Zero);
			DragAcceptNativeHelper.ChangeWindowMessageFilterEx(handle, 74, 1, IntPtr.Zero);
			DragAcceptNativeHelper.ChangeWindowMessageFilterEx(handle, 73, 1, IntPtr.Zero);
		}
		else if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 0)
		{
			DragAcceptNativeHelper.ChangeWindowMessageFilter(563, 1);
			DragAcceptNativeHelper.ChangeWindowMessageFilter(74, 1);
			DragAcceptNativeHelper.ChangeWindowMessageFilter(73, 1);
		}
	}

	internal frmDrvSettings()
		: this(showCsnOnly: false, skipScan: false)
	{
	}

	internal frmDrvSettings(bool showCsnOnly, bool skipScan)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		_skipScan = skipScan;
		InitializeComponent();
		((Control)lblApplications).Text = "";
		InitTaskbarList();
		SetupDropFilesNative();
		SetupToolbar();
		SetupDpiAdjustments();
		tscbShowCustomSettingNamesOnly.Checked = showCsnOnly;
		((Form)this).Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		((ToolStripControlHost)cbProfiles).Control.KeyUp += new KeyEventHandler(cbProfiles_KeyUp);
	}

	private void SetupDpiAdjustments()
	{
		ScaleFactor = ((Control)lblWidth330).Width / 330;
		Graphics val = ((Control)this).CreateGraphics();
		try
		{
			ScaleFactor = Math.Max(ScaleFactor, Math.Max(val.DpiX / 96f, val.DpiY / 96f));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		chSettingID.Width = ((Control)lblWidth330).Width;
		chSettingValueHex.Width = ((Control)lblWidth96).Width;
	}

	private void SetupToolbar()
	{
		tsMain.Renderer = (ToolStripRenderer)(object)new NoBorderRenderer();
		tsMain.ImageScalingSize = new Size(((Control)lblWidth16).Width, ((Control)lblWidth16).Width);
	}

	private void SetupDropFilesNative()
	{
		lvSettings.OnDropFilesNative += lvSettings_OnDropFilesNative;
		DragAcceptNativeHelper.DragAcceptFiles(((Control)this).Handle, fAccept: true);
		DragAcceptNativeHelper.DragAcceptFiles(((Control)lvSettings).Handle, fAccept: true);
		InitMessageFilter(((Control)lvSettings).Handle);
	}

	private void SetupLayout()
	{
		if (Screen.GetWorkingArea((Control)(object)this).Height < ((Control)this).Height + 10)
		{
			((Control)this).Height = Screen.GetWorkingArea((Control)(object)this).Height - 20;
		}
	}

	private void RefreshModifiesProfilesDropDown()
	{
		((ToolStripDropDownItem)tsbModifiedProfiles).DropDownItems.Clear();
		_scanner.ModifiedProfiles.Sort();
		foreach (string modifiedProfile in _scanner.ModifiedProfiles)
		{
			if (modifiedProfile != _baseProfileName)
			{
				ToolStripItem val = ((ToolStripDropDownItem)tsbModifiedProfiles).DropDownItems.Add(modifiedProfile);
				if (!_scanner.UserProfiles.Contains(modifiedProfile))
				{
					val.Image = ((ToolStripItem)tsbRestoreProfile).Image;
				}
			}
		}
		if (((ArrangedElementCollection)((ToolStripDropDownItem)tsbModifiedProfiles).DropDownItems).Count > 0)
		{
			((ToolStripItem)tsbModifiedProfiles).Enabled = true;
		}
	}

	private async void frmDrvSettings_Load(object sender, EventArgs e)
	{
		SetupLayout();
		SetTitleVersion();
		LoadSettings();
		RefreshProfilesCombo();
		((ToolStripItem)cbProfiles).Text = GetBaseProfileName();
		((ToolStripItem)tsbBitValueEditor).Enabled = false;
		((ToolStripItem)tsbDeleteProfile).Enabled = false;
		((ToolStripItem)tsbAddApplication).Enabled = false;
		((ToolStripItem)tssbRemoveApplication).Enabled = false;
		InitResetValueTooltip();
		await CheckForUpdatesAsync();
	}

	private async Task CheckForUpdatesAsync()
	{
		if (_settings.DisableUpdateCheck || File.Exists(Path.Combine(AppContext.BaseDirectory, "DisableUpdateCheck.txt")))
		{
			return;
		}
		try
		{
			bool updateAvailable = await GithubVersionHelper.IsUpdateAvailableAsync();
			if (updateAvailable)
			{
				SetTitleVersion(updateAvailable);
			}
		}
		catch
		{
		}
	}

	private void lvSettings_GroupStateChanged(object sender, GroupStateChangedEventArgs e)
	{
		if (e.IsCollapsed && !_settings.HiddenSettingGroups.Contains(e.Group.Header))
		{
			_settings.HiddenSettingGroups.Add(e.Group.Header);
		}
		else if (!e.IsCollapsed && _settings.HiddenSettingGroups.Contains(e.Group.Header))
		{
			_settings.HiddenSettingGroups.Remove(e.Group.Header);
		}
		SaveSettings();
	}

	private void InitResetValueTooltip()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ToolTip val = new ToolTip();
		val.SetToolTip((Control)(object)btnResetValue, "Restore this value to NVIDIA defaults.");
	}

	private void lvSettings_SelectedIndexChanged(object sender, EventArgs e)
	{
		MoveComboToItemAndFill();
	}

	private void cbValues_SelectedValueChanged(object sender, EventArgs e)
	{
		UpdateItemByComboValue();
	}

	private void cbValues_Leave(object sender, EventArgs e)
	{
		UpdateItemByComboValue();
	}

	private void btnResetValue_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Control.ModifierKeys == 131072)
		{
			DeleteSelectedValue();
		}
		else
		{
			ResetSelectedValue();
		}
	}

	private void ChangeCurrentProfile(string profileName)
	{
		if (profileName == GetBaseProfileName() || profileName == _baseProfileName)
		{
			_CurrentProfile = _baseProfileName;
			((ToolStripItem)cbProfiles).Text = GetBaseProfileName();
			((ToolStripItem)tsbDeleteProfile).Enabled = false;
			((ToolStripItem)tsbAddApplication).Enabled = false;
			((ToolStripItem)tssbRemoveApplication).Enabled = false;
			appPathsTooltip.SetToolTip((Control)(object)lblApplications, "");
		}
		else
		{
			_CurrentProfile = profileName;
			((ToolStripItem)tsbDeleteProfile).Enabled = true;
			((ToolStripItem)tsbAddApplication).Enabled = true;
			((ToolStripItem)tssbRemoveApplication).Enabled = true;
			appPathsTooltip.SetToolTip((Control)(object)lblApplications, "Double-click to add application");
		}
		((Control)txtFilter).Text = "";
		RefreshCurrentProfile();
	}

	private void cbProfiles_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cbProfiles.SelectedIndex > -1)
		{
			ChangeCurrentProfile(((ToolStripItem)cbProfiles).Text);
		}
		((Control)lvSettings).Focus();
	}

	private void cbProfiles_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if ((int)e.KeyCode != 13)
		{
			return;
		}
		try
		{
			string profileNameByExeName = _drs.GetProfileNameByExeName(((ToolStripItem)cbProfiles).Text);
			if (!string.IsNullOrEmpty(profileNameByExeName))
			{
				((ToolStripItem)cbProfiles).Text = profileNameByExeName;
				ChangeCurrentProfile(profileNameByExeName);
			}
		}
		catch
		{
		}
	}

	private void SetTaskbarProgress(int progress)
	{
		if (!_isWin7TaskBar)
		{
			return;
		}
		try
		{
			if (progress == 0)
			{
				_taskbarList.SetProgressState(_taskbarParent, TBPFLAG.TBPF_NOPROGRESS);
				return;
			}
			_taskbarList.SetProgressState(_taskbarParent, TBPFLAG.TBPF_NORMAL);
			_taskbarList.SetProgressValue(_taskbarParent, (ulong)progress, 100uL);
		}
		catch
		{
		}
	}

	private void AddToModifiedProfiles(string profileName, bool userProfile = false)
	{
		if (!_scanner.UserProfiles.Contains(profileName) && profileName != _baseProfileName && userProfile)
		{
			_scanner.UserProfiles.Add(profileName);
		}
		if (!_scanner.ModifiedProfiles.Contains(profileName) && profileName != _baseProfileName)
		{
			_scanner.ModifiedProfiles.Add(profileName);
			RefreshModifiesProfilesDropDown();
		}
	}

	private void RemoveFromModifiedProfiles(string profileName)
	{
		if (_scanner.UserProfiles.Contains(profileName))
		{
			_scanner.UserProfiles.Remove(profileName);
		}
		if (_scanner.ModifiedProfiles.Contains(profileName))
		{
			_scanner.ModifiedProfiles.Remove(profileName);
			RefreshModifiesProfilesDropDown();
		}
	}

	private void ShowExportProfiles()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (_scanner.ModifiedProfiles.Count > 0)
		{
			frmExportProfiles frmExportProfiles2 = new frmExportProfiles();
			frmExportProfiles2.ShowDialog(this);
		}
		else
		{
			MessageBox.Show("No user modified profiles found! Nothing to export.", "Userprofile Search", (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
	}

	private async Task ScanProfilesSilentAsync(bool scanPredefined, bool showProfileDialog)
	{
		if (_skipScan)
		{
			if (scanPredefined && !_alreadyScannedForPredefinedSettings)
			{
				_alreadyScannedForPredefinedSettings = true;
				_meta.ResetMetaCache();
				((ToolStripItem)tsbModifiedProfiles).Enabled = true;
				((ToolStripItem)exportUserdefinedProfilesToolStripMenuItem).Enabled = false;
				RefreshCurrentProfile();
			}
			return;
		}
		((ToolStripItem)tsbModifiedProfiles).Enabled = false;
		((ToolStripItem)tsbRefreshProfile).Enabled = false;
		pbMain.Minimum = 0;
		pbMain.Maximum = 100;
		_scannerCancelationTokenSource = new CancellationTokenSource();
		Progress<int> progressHandler = new Progress<int>(delegate(int value)
		{
			pbMain.Value = value;
			SetTaskbarProgress(value);
		});
		if (!scanPredefined || _alreadyScannedForPredefinedSettings)
		{
			await _scanner.ScanProfileSettingsAsync(justModified: true, progressHandler, _scannerCancelationTokenSource.Token);
		}
		else
		{
			_alreadyScannedForPredefinedSettings = true;
			await _scanner.ScanProfileSettingsAsync(justModified: false, progressHandler, _scannerCancelationTokenSource.Token);
			_meta.ResetMetaCache();
			((ToolStripItem)tscbShowScannedUnknownSettings).Enabled = true;
		}
		RefreshModifiesProfilesDropDown();
		((ToolStripItem)tsbModifiedProfiles).Enabled = true;
		pbMain.Value = 0;
		((Control)pbMain).Enabled = false;
		SetTaskbarProgress(0);
		if (showProfileDialog)
		{
			ShowExportProfiles();
		}
		RefreshCurrentProfile();
		((ToolStripItem)tsbRefreshProfile).Enabled = true;
	}

	private void cbCustomSettingsOnly_CheckedChanged(object sender, EventArgs e)
	{
		RefreshCurrentProfile();
	}

	internal void SetSelectedDwordValue(uint dwordValue)
	{
		if ((((ListView)lvSettings).SelectedItems != null) & (((ListView)lvSettings).SelectedItems.Count > 0))
		{
			((Control)cbValues).Text = DrsUtil.GetDwordString(dwordValue);
			UpdateItemByComboValue();
		}
	}

	private async void tsbRestoreProfile_Click(object sender, EventArgs e)
	{
		if ((int)Control.ModifierKeys == 131072)
		{
			if ((int)MessageBox.Show((IWin32Window)(object)this, "Restore all profiles to NVIDIA driver defaults?", "Restore all profiles", (MessageBoxButtons)4, (MessageBoxIcon)32) == 6)
			{
				_drs.ResetAllProfilesInternal();
				RefreshProfilesCombo();
				RefreshCurrentProfile();
				await ScanProfilesSilentAsync(scanPredefined: true, showProfileDialog: false);
				((ToolStripItem)cbProfiles).Text = GetBaseProfileName();
			}
		}
		else if ((int)MessageBox.Show((IWin32Window)(object)this, "Restore profile to NVIDIA driver defaults?", "Restore profile", (MessageBoxButtons)4, (MessageBoxIcon)32) == 6)
		{
			ResetCurrentProfile();
		}
	}

	private void tsbRefreshProfile_Click(object sender, EventArgs e)
	{
		DrsSessionScope.DestroyGlobalSession();
		RefreshAll();
	}

	private void tsbApplyProfile_Click(object sender, EventArgs e)
	{
		try
		{
			UpdateItemByComboValue();
		}
		catch
		{
		}
		StoreChangesOfProfileToDriver();
	}

	private void tsbBitValueEditor_Click(object sender, EventArgs e)
	{
		if ((((ListView)lvSettings).SelectedItems != null) & (((ListView)lvSettings).SelectedItems.Count > 0))
		{
			frmBitEditor frmBitEditor2 = new frmBitEditor();
			frmBitEditor2.ShowDialog(this, (uint)((ListView)lvSettings).SelectedItems[0].Tag, uint.Parse(((ListView)lvSettings).SelectedItems[0].SubItems[2].Text.Substring(2), NumberStyles.AllowHexSpecifier), ((ListView)lvSettings).SelectedItems[0].Text);
		}
	}

	private void tscbShowScannedUnknownSettings_Click(object sender, EventArgs e)
	{
		RefreshCurrentProfile();
	}

	private void lvSettings_Resize(object sender, EventArgs e)
	{
		ResizeColumn();
	}

	private void ResizeColumn()
	{
		((ListView)lvSettings).Columns[1].Width = ((Control)lvSettings).Width - (((ListView)lvSettings).Columns[0].Width + ((ListView)lvSettings).Columns[2].Width + ((Control)lblWidth30).Width);
	}

	private void lvSettings_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
	{
		if (e.ColumnIndex != 1)
		{
			if (e.ColumnIndex == 0 && e.NewWidth < 260)
			{
				e.NewWidth = 260;
				((CancelEventArgs)(object)e).Cancel = true;
			}
			else if (e.ColumnIndex == 2 && e.NewWidth < 96)
			{
				((CancelEventArgs)(object)e).Cancel = true;
				e.NewWidth = 96;
			}
			ResizeColumn();
		}
	}

	private async void frmDrvSettings_Shown(object sender, EventArgs e)
	{
		if (_isStartup)
		{
			new Thread(SetTaskbarIcon).Start();
			await ScanProfilesSilentAsync(scanPredefined: true, showProfileDialog: false);
			if (_scannerCancelationTokenSource != null && !_scannerCancelationTokenSource.Token.IsCancellationRequested && (int)((Form)this).WindowState != 2)
			{
				new MessageHelper().bringAppToFront((int)((Control)this).Handle);
			}
			_isStartup = false;
		}
	}

	private void tsbDeleteProfile_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Control.ModifierKeys == 131072)
		{
			if ((int)MessageBox.Show((IWin32Window)(object)this, "Really delete all profiles?", "Delete all profiles", (MessageBoxButtons)4, (MessageBoxIcon)32) == 6)
			{
				_drs.DeleteAllProfilesHard();
				ChangeCurrentProfile(_baseProfileName);
				DrsSessionScope.DestroyGlobalSession();
				RefreshAll();
			}
		}
		else if ((int)MessageBox.Show((IWin32Window)(object)this, "Really delete this profile?\r\n\r\nNote: NVIDIA predefined profiles can not be restored until next driver installation!", "Delete Profile", (MessageBoxButtons)4, (MessageBoxIcon)32) == 6)
		{
			if (DrsSettingsServiceBase.DriverVersion > 280f && DrsSettingsServiceBase.DriverVersion < 310f)
			{
				_drs.DeleteProfileHard(_CurrentProfile);
			}
			else
			{
				_drs.DeleteProfile(_CurrentProfile);
			}
			RemoveFromModifiedProfiles(_CurrentProfile);
			MessageBox.Show((IWin32Window)(object)this, $"Profile '{_CurrentProfile}' has been deleted.", "Info", (MessageBoxButtons)0, (MessageBoxIcon)64);
			RefreshProfilesCombo();
			ChangeCurrentProfile(_baseProfileName);
		}
	}

	private void tsbAddApplication_Click(object sender, EventArgs e)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (_CurrentProfile == GetBaseProfileName() || _CurrentProfile == _baseProfileName)
		{
			return;
		}
		Dictionary<string, string> applications = new Dictionary<string, string>();
		_currentProfileSettingItems = _drs.GetSettingsForProfile(_CurrentProfile, GetSettingViewMode(), ref applications);
		HashSet<string> hashSet = new HashSet<string>(applications.Values, StringComparer.OrdinalIgnoreCase);
		string value = "";
		if ((int)InputBox.Show("Add Application", "Enter an application path/filename/UWP ID to add to the profile:", ref value, new List<string>(), "", 2048, allowExeBrowse: true) != 1)
		{
			return;
		}
		if (hashSet.Contains(value))
		{
			MessageBox.Show("This application is already assigned to this profile!", "Error adding Application", (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		try
		{
			_drs.AddApplication(_CurrentProfile, value);
		}
		catch (NvapiException ex)
		{
			if (ex.Status != NvAPI_Status.NVAPI_EXECUTABLE_ALREADY_IN_USE && ex.Status != NvAPI_Status.NVAPI_ERROR)
			{
				throw;
			}
			if (((Control)lblApplications).Text.ToUpper().IndexOf(" " + value.ToUpper() + ",") != -1)
			{
				MessageBox.Show("This application is already assigned to this profile!", "Error adding Application", (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			else
			{
				string text = _scanner.FindProfilesUsingApplication(value);
				if (text == "")
				{
					MessageBox.Show("This application might already be assigned to another profile!", "Error adding Application", (MessageBoxButtons)0, (MessageBoxIcon)16);
				}
				else
				{
					MessageBox.Show("This application is already assigned to the following profiles: " + text, "Error adding Application", (MessageBoxButtons)0, (MessageBoxIcon)16);
				}
			}
		}
		RefreshCurrentProfile();
	}

	private void tssbRemoveApplication_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		_drs.RemoveApplication(_CurrentProfile, e.ClickedItem.Tag.ToString());
		RefreshCurrentProfile();
	}

	private void tsbCreateProfile_Click(object sender, EventArgs e)
	{
		ShowCreateProfileDialog("");
	}

	private void ShowCreateProfileDialog(string nameProposal, string applicationName = null)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		List<string> invalidInputs = ((IEnumerable)cbProfiles.Items).Cast<string>().ToList();
		string value = nameProposal;
		if ((int)InputBox.Show("Create Profile", "Please enter profile name:", ref value, invalidInputs, "", 2048) == 1)
		{
			try
			{
				_drs.CreateProfile(value, applicationName);
				RefreshProfilesCombo();
				cbProfiles.SelectedIndex = cbProfiles.Items.IndexOf((object)value);
				AddToModifiedProfiles(value, userProfile: true);
			}
			catch (NvapiException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}

	private void tsbExportProfiles_Click(object sender, EventArgs e)
	{
		((ToolStripDropDownItem)tsbExportProfiles).ShowDropDown();
	}

	private void tsbImportProfiles_Click(object sender, EventArgs e)
	{
		((ToolStripDropDownItem)tsbImportProfiles).ShowDropDown();
	}

	private async void exportUserdefinedProfilesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await ScanProfilesSilentAsync(scanPredefined: false, showProfileDialog: true);
	}

	private void ExportCurrentProfile(bool includePredefined)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).DefaultExt = "*.nip";
		((FileDialog)val).Filter = Application.ProductName + " Profiles|*.nip";
		((FileDialog)val).FileName = _CurrentProfile + ".nip";
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			List<string> profileNames = new string[1] { _CurrentProfile }.ToList();
			_import.ExportProfiles(profileNames, ((FileDialog)val).FileName, includePredefined);
		}
	}

	private void exportCurrentProfileOnlyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ExportCurrentProfile(includePredefined: false);
	}

	private void exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ExportCurrentProfile(includePredefined: true);
	}

	private void tssbRemoveApplication_Click(object sender, EventArgs e)
	{
		if (((ArrangedElementCollection)((ToolStrip)((ToolStripDropDownItem)tssbRemoveApplication).DropDown).Items).Count > 0)
		{
			((ToolStripDropDownItem)tssbRemoveApplication).ShowDropDown();
		}
	}

	private void tsbModifiedProfiles_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
	{
		cbProfiles.SelectedIndex = cbProfiles.FindStringExact(e.ClickedItem.Text);
	}

	private string GetBaseProfileName()
	{
		return $"_GLOBAL_DRIVER_PROFILE ({_baseProfileName})";
	}

	private void tsbModifiedProfiles_ButtonClick(object sender, EventArgs e)
	{
		ChangeCurrentProfile(GetBaseProfileName());
	}

	private void frmDrvSettings_Activated(object sender, EventArgs e)
	{
		if (!_activated)
		{
			_activated = true;
		}
	}

	private void exportAllProfilesNVIDIATextFormatToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).DefaultExt = "*.txt";
		((FileDialog)val).Filter = "Profiles (NVIDIA Text Format)|*.txt";
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			_import.ExportAllProfilesToNvidiaTextFile(((FileDialog)val).FileName);
		}
	}

	private async void RefreshAll()
	{
		((Control)txtFilter).Text = "";
		RefreshProfilesCombo();
		await ScanProfilesSilentAsync(scanPredefined: true, showProfileDialog: false);
		int idx = cbProfiles.Items.IndexOf((object)_CurrentProfile);
		if (idx == -1 || _CurrentProfile == _baseProfileName)
		{
			((ToolStripItem)cbProfiles).Text = GetBaseProfileName();
		}
		else
		{
			cbProfiles.SelectedIndex = idx;
		}
		RefreshCurrentProfile();
	}

	private void importAllProfilesNVIDIATextFormatToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		OpenFileDialog val = new OpenFileDialog();
		((FileDialog)val).DefaultExt = "*.txt";
		((FileDialog)val).Filter = "Profiles (NVIDIA Text Format)|*.txt";
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			try
			{
				_import.ImportAllProfilesFromNvidiaTextFile(((FileDialog)val).FileName);
				MessageBox.Show("Profile(s) successfully imported!", Application.ProductName, (MessageBoxButtons)0, (MessageBoxIcon)64);
				DrsSessionScope.DestroyGlobalSession();
				RefreshAll();
			}
			catch (NvapiException)
			{
				MessageBox.Show("Profile(s) could not imported!", Application.ProductName, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}
	}

	private void importProfilesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog();
		((FileDialog)val).DefaultExt = "*.nip";
		((FileDialog)val).Filter = Application.ProductName + " Profiles|*.nip";
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			ImportProfiles(((FileDialog)val).FileName);
		}
	}

	private void cbProfiles_TextChanged(object sender, EventArgs e)
	{
		if (cbProfiles.DroppedDown)
		{
			string text = ((ToolStripItem)cbProfiles).Text;
			cbProfiles.DroppedDown = false;
			((ToolStripItem)cbProfiles).Text = text;
			cbProfiles.Select(((ToolStripItem)cbProfiles).Text.Length, 0);
		}
	}

	public static void ShowImportDoneMessage(string importReport)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(importReport))
		{
			MessageBox.Show("Profile(s) successfully imported!", Application.ProductName, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		else
		{
			MessageBox.Show("Some profile(s) could not imported!\r\n\r\n" + importReport, Application.ProductName, (MessageBoxButtons)0, (MessageBoxIcon)48);
		}
	}

	private void ImportProfiles(string nipFileName)
	{
		string importReport = _import.ImportProfiles(nipFileName);
		RefreshAll();
		ShowImportDoneMessage(importReport);
	}

	private void lvSettings_OnDropFilesNative(string[] files)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I4
		if (files.Length != 1)
		{
			return;
		}
		FileInfo fileInfo = new FileInfo(files[0]);
		if (fileInfo.Extension.ToLowerInvariant().Equals(".nip"))
		{
			ImportProfiles(fileInfo.FullName);
			return;
		}
		string profileName = "";
		string text = ShortcutResolver.ResolveExecuteable(files[0], out profileName);
		if (!(text != ""))
		{
			return;
		}
		string text2 = _scanner.FindProfilesUsingApplication(text);
		if (text2 != "")
		{
			string text3 = text2.Split(new char[1] { ';' })[0];
			int num = cbProfiles.Items.IndexOf((object)text3);
			if (num > -1)
			{
				cbProfiles.SelectedIndex = num;
			}
		}
		else
		{
			DialogResult val = MessageBox.Show("Would you like to create a new profile for this application?", "Profile not found!", (MessageBoxButtons)4);
			if ((int)val == 6)
			{
				ShowCreateProfileDialog(profileName, text);
			}
		}
	}

	private void lvSettings_DoubleClick(object sender, EventArgs e)
	{
		if (_isDevMode && ((ListView)lvSettings).SelectedItems != null && ((ListView)lvSettings).SelectedItems.Count == 1)
		{
			uint num = (uint)((ListView)lvSettings).SelectedItems[0].Tag;
			string text = ((ListView)lvSettings).SelectedItems[0].Text;
			Clipboard.SetText(string.Format(text ?? "", Array.Empty<object>()));
		}
	}

	private void HandleScreenConstraints()
	{
		Rectangle workingArea = Screen.GetWorkingArea((Control)(object)this);
		if (((Control)this).Left < workingArea.X)
		{
			((Control)this).Left = workingArea.X;
		}
		if (((Control)this).Top < workingArea.Y)
		{
			((Control)this).Top = workingArea.Y;
		}
		if (((Control)this).Left + ((Control)this).Width > workingArea.X + workingArea.Width)
		{
			((Control)this).Left = workingArea.X + workingArea.Width - ((Control)this).Width;
		}
		if (((Control)this).Top + ((Control)this).Height > workingArea.Y + workingArea.Height)
		{
			((Control)this).Top = workingArea.Y + workingArea.Height - ((Control)this).Height;
		}
	}

	private void SaveSettings()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (_settings == null)
		{
			_settings = UserSettings.LoadSettings();
		}
		if ((int)((Form)this).WindowState == 0)
		{
			_settings.WindowTop = ((Control)this).Top;
			_settings.WindowLeft = ((Control)this).Left;
			_settings.WindowHeight = ((Control)this).Height;
			_settings.WindowWidth = ((Control)this).Width;
		}
		else
		{
			_settings.WindowTop = ((Form)this).RestoreBounds.Top;
			_settings.WindowLeft = ((Form)this).RestoreBounds.Left;
			_settings.WindowHeight = ((Form)this).RestoreBounds.Height;
			_settings.WindowWidth = ((Form)this).RestoreBounds.Width;
		}
		_settings.WindowState = ((Form)this).WindowState;
		_settings.ShowCustomizedSettingNamesOnly = tscbShowCustomSettingNamesOnly.Checked;
		_settings.ShowScannedUnknownSettings = tscbShowScannedUnknownSettings.Checked;
		_settings.SaveSettings();
	}

	private void LoadSettings()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		_settings = UserSettings.LoadSettings();
		((Control)this).SetBounds(_settings.WindowLeft, _settings.WindowTop, _settings.WindowWidth, _settings.WindowHeight);
		((Form)this).WindowState = (FormWindowState)(((int)_settings.WindowState != 1) ? ((int)_settings.WindowState) : 0);
		HandleScreenConstraints();
		tscbShowCustomSettingNamesOnly.Checked = _settings.ShowCustomizedSettingNamesOnly;
		tscbShowScannedUnknownSettings.Checked = !_skipScan && _settings.ShowScannedUnknownSettings;
	}

	private void frmDrvSettings_FormClosed(object sender, FormClosedEventArgs e)
	{
		_scannerCancelationTokenSource?.Cancel();
		SaveSettings();
	}

	private void lvSettings_KeyDown(object sender, KeyEventArgs e)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Invalid comparison between Unknown and I4
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Invalid comparison between Unknown and I4
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Invalid comparison between Unknown and I4
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Invalid comparison between Unknown and I4
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Invalid comparison between Unknown and I4
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Invalid comparison between Unknown and I4
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Invalid comparison between Unknown and I4
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Invalid comparison between Unknown and I4
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Invalid comparison between Unknown and I4
		if (e.Control && (int)e.KeyCode == 67)
		{
			CopyModifiedSettingsToClipBoard();
		}
		else if (e.Control && e.Alt && (int)e.KeyCode == 68)
		{
			ToggleDevMode();
		}
		else if (Debugger.IsAttached && e.Control && (int)e.KeyCode == 84)
		{
			TestStoreSettings();
		}
		else if (e.Control && (int)e.KeyCode == 70)
		{
			((Control)txtFilter).Focus();
		}
		else if ((int)e.KeyCode == 27)
		{
			((Control)txtFilter).Text = "";
			RefreshCurrentProfile();
		}
		else
		{
			if (e.Control || (((int)e.KeyCode < 65 || (int)e.KeyCode > 90) && ((int)e.KeyCode < 48 || (int)e.KeyCode > 57) && ((int)e.KeyCode < 96 || (int)e.KeyCode > 105) && (int)e.KeyCode != 32 && (int)e.KeyCode != 190 && (int)e.KeyCode != 8))
			{
				return;
			}
			((Control)txtFilter).Focus();
			if ((int)e.KeyCode == 8)
			{
				if (((Control)txtFilter).Text.Length > 0)
				{
					((Control)txtFilter).Text = ((Control)txtFilter).Text.Substring(0, ((Control)txtFilter).Text.Length - 1);
				}
			}
			else
			{
				WatermarkTextBox watermarkTextBox = txtFilter;
				((Control)watermarkTextBox).Text = ((Control)watermarkTextBox).Text + (e.Shift ? ((object)e.KeyCode/*cast due to constrained. prefix*/).ToString() : ((object)e.KeyCode/*cast due to constrained. prefix*/).ToString().ToLower());
			}
			((TextBoxBase)txtFilter).SelectionStart = ((Control)txtFilter).Text.Length;
			e.SuppressKeyPress = true;
			e.Handled = true;
		}
	}

	private async void txtFilter_TextChanged(object sender, EventArgs e)
	{
		RefreshCurrentProfile();
		if (!string.IsNullOrEmpty(((Control)txtFilter).Text))
		{
			((Control)txtFilter).Focus();
		}
	}

	private void ToggleDevMode()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		_isDevMode = !_isDevMode;
		if (_isDevMode)
		{
			((Control)lvSettings).Font = new Font("Consolas", 9f);
			((Control)cbValues).Font = new Font("Consolas", 9f);
			((ListView)lvSettings).HeaderStyle = (ColumnHeaderStyle)1;
		}
		else
		{
			((Control)lvSettings).Font = null;
			((Control)cbValues).Font = null;
			((ListView)lvSettings).HeaderStyle = (ColumnHeaderStyle)0;
		}
		RefreshCurrentProfile();
	}

	private void TestStoreSettings()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("{0,-40} {1}\r\n", "### Inspector Store Failed ###", _CurrentProfile);
		pbMain.Minimum = 0;
		pbMain.Maximum = ((ListView)lvSettings).Items.Count;
		int num = 0;
		foreach (ListViewGroup group in ((ListView)lvSettings).Groups)
		{
			ListViewGroup val = group;
			bool flag = false;
			foreach (ListViewItem item in val.Items)
			{
				ListViewItem val2 = item;
				try
				{
					pbMain.Value = num++;
					uint settingId = (uint)val2.Tag;
					SettingMeta settingMeta = _meta.GetSettingMeta(settingId);
					if (settingMeta.SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
					{
						if (Enumerable.Contains(new int[3] { 1, 2, 3 }, val2.ImageIndex))
						{
							_drs.SetDwordValueToProfile(_CurrentProfile, settingId, 0u);
							_drs.ResetValue(_CurrentProfile, settingId, out var _);
						}
						else
						{
							uint dwordValueFromProfile = _drs.GetDwordValueFromProfile(_CurrentProfile, settingId);
							_drs.SetDwordValueToProfile(_CurrentProfile, settingId, 0u);
							_drs.SetDwordValueToProfile(_CurrentProfile, settingId, dwordValueFromProfile);
						}
					}
				}
				catch (NvapiException ex)
				{
					if (!flag)
					{
						stringBuilder.AppendFormat("\r\n[{0}]\r\n", val.Header);
						flag = true;
					}
					stringBuilder.AppendFormat("{0,-40} SettingId: {1} Failed: {2}\r\n", val2.Text, DrsUtil.GetDwordString((uint)val2.Tag), ex.Status);
				}
			}
		}
		pbMain.Value = 0;
		Clipboard.SetText(stringBuilder.ToString());
		MessageBox.Show("Failed Settings Stored to Clipboard");
	}

	private void CopyModifiedSettingsToClipBoard()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("{0,-40} {1}\r\n", "### NVIDIA Profile Inspector ###", _CurrentProfile);
		foreach (ListViewGroup group in ((ListView)lvSettings).Groups)
		{
			ListViewGroup val = group;
			bool flag = false;
			foreach (ListViewItem item in val.Items)
			{
				ListViewItem val2 = item;
				if (val2.ImageIndex == 0)
				{
					if (!flag)
					{
						stringBuilder.AppendFormat("\r\n[{0}]\r\n", val.Header);
						flag = true;
					}
					stringBuilder.AppendFormat("{0,-40} {1}\r\n", val2.Text, val2.SubItems[1].Text);
				}
			}
		}
		Clipboard.SetText(stringBuilder.ToString());
	}

	private void txtFilter_KeyUp(object sender, KeyEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		if ((int)e.KeyCode == 27)
		{
			((Control)txtFilter).Text = "";
		}
		else if (e.Control && e.Alt && (int)e.KeyCode == 68)
		{
			ToggleDevMode();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Expected O, but got Unknown
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Expected O, but got Unknown
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Expected O, but got Unknown
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Expected O, but got Unknown
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Expected O, but got Unknown
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Expected O, but got Unknown
		//IL_10ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1195: Unknown result type (might be due to invalid IL or missing references)
		//IL_121e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1228: Expected O, but got Unknown
		//IL_12c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d0: Expected O, but got Unknown
		//IL_138f: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1696: Unknown result type (might be due to invalid IL or missing references)
		//IL_1745: Unknown result type (might be due to invalid IL or missing references)
		//IL_174f: Expected O, but got Unknown
		//IL_178d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1797: Expected O, but got Unknown
		//IL_1971: Unknown result type (might be due to invalid IL or missing references)
		//IL_197b: Expected O, but got Unknown
		//IL_1999: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a39: Expected O, but got Unknown
		//IL_1b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8a: Expected O, but got Unknown
		components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmDrvSettings));
		ilListView = new ImageList(components);
		pbMain = new ProgressBar();
		tsMain = new ToolStrip();
		tslProfiles = new ToolStripLabel();
		cbProfiles = new ToolStripComboBox();
		tsbModifiedProfiles = new ToolStripSplitButton();
		toolStripSeparator1 = new ToolStripSeparator();
		tsbRefreshProfile = new ToolStripButton();
		tsbRestoreProfile = new ToolStripButton();
		tsbCreateProfile = new ToolStripButton();
		tsbDeleteProfile = new ToolStripButton();
		tsSep2 = new ToolStripSeparator();
		tsbAddApplication = new ToolStripButton();
		tssbRemoveApplication = new ToolStripSplitButton();
		tsSep3 = new ToolStripSeparator();
		tsbExportProfiles = new ToolStripSplitButton();
		exportCurrentProfileOnlyToolStripMenuItem = new ToolStripMenuItem();
		exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem = new ToolStripMenuItem();
		exportUserdefinedProfilesToolStripMenuItem = new ToolStripMenuItem();
		exportAllProfilesNVIDIATextFormatToolStripMenuItem = new ToolStripMenuItem();
		tsbImportProfiles = new ToolStripSplitButton();
		importProfilesToolStripMenuItem = new ToolStripMenuItem();
		importAllProfilesNVIDIATextFormatToolStripMenuItem = new ToolStripMenuItem();
		tsSep4 = new ToolStripSeparator();
		tscbShowCustomSettingNamesOnly = new ToolStripButton();
		tsSep5 = new ToolStripSeparator();
		tscbShowScannedUnknownSettings = new ToolStripButton();
		tsbBitValueEditor = new ToolStripButton();
		tsSep6 = new ToolStripSeparator();
		tsbApplyProfile = new ToolStripButton();
		btnResetValue = new Button();
		lblApplications = new Label();
		toolStripButton5 = new ToolStripButton();
		toolStripLabel2 = new ToolStripLabel();
		toolStripButton6 = new ToolStripButton();
		ilCombo = new ImageList(components);
		cbValues = new ComboBox();
		lblWidth96 = new Label();
		lblWidth330 = new Label();
		lblWidth16 = new Label();
		lblWidth30 = new Label();
		lvSettings = new ListViewEx();
		chSettingID = new ColumnHeader();
		chSettingValue = new ColumnHeader();
		chSettingValueHex = new ColumnHeader();
		tbSettingDescription = new TextBox();
		pnlListview = new Panel();
		txtFilter = new WatermarkTextBox();
		((Control)tsMain).SuspendLayout();
		((Control)pnlListview).SuspendLayout();
		((Control)this).SuspendLayout();
		ilListView.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("ilListView.ImageStream");
		ilListView.TransparentColor = Color.Transparent;
		ilListView.Images.SetKeyName(0, "0_gear2.png");
		ilListView.Images.SetKeyName(1, "1_gear2_2.png");
		ilListView.Images.SetKeyName(2, "4_gear_nv2.png");
		ilListView.Images.SetKeyName(3, "6_gear_inherit.png");
		((Control)pbMain).Anchor = (AnchorStyles)14;
		((Control)pbMain).Location = new Point(12, 475);
		((Control)pbMain).Margin = new Padding(4);
		((Control)pbMain).Name = "pbMain";
		((Control)pbMain).Size = new Size(840, 9);
		((Control)pbMain).TabIndex = 19;
		tsMain.AllowMerge = false;
		((Control)tsMain).Anchor = (AnchorStyles)13;
		((Control)tsMain).AutoSize = false;
		((Control)tsMain).BackgroundImage = (Image)(object)Resources.transparent16;
		tsMain.CanOverflow = false;
		((Control)tsMain).Dock = (DockStyle)0;
		tsMain.GripMargin = new Padding(0);
		tsMain.GripStyle = (ToolStripGripStyle)0;
		tsMain.ImageScalingSize = new Size(20, 20);
		tsMain.Items.AddRange((ToolStripItem[])(object)new ToolStripItem[21]
		{
			(ToolStripItem)tslProfiles,
			(ToolStripItem)cbProfiles,
			(ToolStripItem)tsbModifiedProfiles,
			(ToolStripItem)toolStripSeparator1,
			(ToolStripItem)tsbRefreshProfile,
			(ToolStripItem)tsbRestoreProfile,
			(ToolStripItem)tsbCreateProfile,
			(ToolStripItem)tsbDeleteProfile,
			(ToolStripItem)tsSep2,
			(ToolStripItem)tsbAddApplication,
			(ToolStripItem)tssbRemoveApplication,
			(ToolStripItem)tsSep3,
			(ToolStripItem)tsbExportProfiles,
			(ToolStripItem)tsbImportProfiles,
			(ToolStripItem)tsSep4,
			(ToolStripItem)tscbShowCustomSettingNamesOnly,
			(ToolStripItem)tsSep5,
			(ToolStripItem)tscbShowScannedUnknownSettings,
			(ToolStripItem)tsbBitValueEditor,
			(ToolStripItem)tsSep6,
			(ToolStripItem)tsbApplyProfile
		});
		tsMain.LayoutStyle = (ToolStripLayoutStyle)1;
		((Control)tsMain).Location = new Point(12, 4);
		((Control)tsMain).Name = "tsMain";
		tsMain.RenderMode = (ToolStripRenderMode)2;
		((Control)tsMain).Size = new Size(840, 25);
		((Control)tsMain).TabIndex = 24;
		((Control)tsMain).Text = "toolStrip1";
		((ToolStripItem)tslProfiles).ImageScaling = (ToolStripItemImageScaling)0;
		((ToolStripItem)tslProfiles).Margin = new Padding(0, 5, 10, 2);
		((ToolStripItem)tslProfiles).Name = "tslProfiles";
		((ToolStripItem)tslProfiles).Size = new Size(49, 18);
		((ToolStripItem)tslProfiles).Text = "Profiles:";
		cbProfiles.AutoCompleteMode = (AutoCompleteMode)1;
		cbProfiles.AutoCompleteSource = (AutoCompleteSource)256;
		((ToolStripItem)cbProfiles).AutoSize = false;
		cbProfiles.DropDownWidth = 290;
		((ToolStripItem)cbProfiles).Margin = new Padding(1);
		cbProfiles.MaxDropDownItems = 50;
		((ToolStripItem)cbProfiles).Name = "cbProfiles";
		((ToolStripItem)cbProfiles).Size = new Size(290, 23);
		cbProfiles.SelectedIndexChanged += cbProfiles_SelectedIndexChanged;
		((ToolStripItem)cbProfiles).TextChanged += cbProfiles_TextChanged;
		((ToolStripItem)tsbModifiedProfiles).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbModifiedProfiles).Enabled = false;
		((ToolStripItem)tsbModifiedProfiles).Image = (Image)(object)Resources.home_sm;
		((ToolStripItem)tsbModifiedProfiles).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbModifiedProfiles).Name = "tsbModifiedProfiles";
		((ToolStripItem)tsbModifiedProfiles).Size = new Size(36, 22);
		((ToolStripItem)tsbModifiedProfiles).TextImageRelation = (TextImageRelation)0;
		((ToolStripItem)tsbModifiedProfiles).ToolTipText = "Back to global profile (Home) / User modified profiles";
		tsbModifiedProfiles.ButtonClick += tsbModifiedProfiles_ButtonClick;
		((ToolStripDropDownItem)tsbModifiedProfiles).DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbModifiedProfiles_DropDownItemClicked);
		((ToolStripItem)toolStripSeparator1).Name = "toolStripSeparator1";
		((ToolStripItem)toolStripSeparator1).Size = new Size(6, 25);
		((ToolStripItem)tsbRefreshProfile).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbRefreshProfile).Image = (Image)componentResourceManager.GetObject("tsbRefreshProfile.Image");
		((ToolStripItem)tsbRefreshProfile).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbRefreshProfile).Name = "tsbRefreshProfile";
		((ToolStripItem)tsbRefreshProfile).Size = new Size(24, 22);
		((ToolStripItem)tsbRefreshProfile).Text = "Refresh current profile.";
		((ToolStripItem)tsbRefreshProfile).Click += tsbRefreshProfile_Click;
		((ToolStripItem)tsbRestoreProfile).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbRestoreProfile).Image = (Image)componentResourceManager.GetObject("tsbRestoreProfile.Image");
		((ToolStripItem)tsbRestoreProfile).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbRestoreProfile).Name = "tsbRestoreProfile";
		((ToolStripItem)tsbRestoreProfile).Size = new Size(24, 22);
		((ToolStripItem)tsbRestoreProfile).Text = "Restore current profile to NVIDIA defaults.";
		((ToolStripItem)tsbRestoreProfile).Click += tsbRestoreProfile_Click;
		((ToolStripItem)tsbCreateProfile).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbCreateProfile).Image = (Image)componentResourceManager.GetObject("tsbCreateProfile.Image");
		((ToolStripItem)tsbCreateProfile).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbCreateProfile).Name = "tsbCreateProfile";
		((ToolStripItem)tsbCreateProfile).Size = new Size(24, 22);
		((ToolStripItem)tsbCreateProfile).Text = "Create new profile";
		((ToolStripItem)tsbCreateProfile).Click += tsbCreateProfile_Click;
		((ToolStripItem)tsbDeleteProfile).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbDeleteProfile).Image = (Image)(object)Resources.ieframe_1_18212;
		((ToolStripItem)tsbDeleteProfile).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbDeleteProfile).Name = "tsbDeleteProfile";
		((ToolStripItem)tsbDeleteProfile).Size = new Size(24, 22);
		((ToolStripItem)tsbDeleteProfile).Text = "Delete current Profile";
		((ToolStripItem)tsbDeleteProfile).Click += tsbDeleteProfile_Click;
		((ToolStripItem)tsSep2).Name = "tsSep2";
		((ToolStripItem)tsSep2).Size = new Size(6, 25);
		((ToolStripItem)tsbAddApplication).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbAddApplication).Image = (Image)(object)Resources.window_application_add;
		((ToolStripItem)tsbAddApplication).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbAddApplication).Name = "tsbAddApplication";
		((ToolStripItem)tsbAddApplication).Size = new Size(24, 22);
		((ToolStripItem)tsbAddApplication).Text = "Add application to current profile.";
		((ToolStripItem)tsbAddApplication).Click += tsbAddApplication_Click;
		((ToolStripItem)tssbRemoveApplication).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tssbRemoveApplication).Image = (Image)(object)Resources.window_application_delete;
		((ToolStripItem)tssbRemoveApplication).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tssbRemoveApplication).Name = "tssbRemoveApplication";
		((ToolStripItem)tssbRemoveApplication).Size = new Size(36, 22);
		((ToolStripItem)tssbRemoveApplication).Text = "Remove application from current profile";
		((ToolStripDropDownItem)tssbRemoveApplication).DropDownItemClicked += new ToolStripItemClickedEventHandler(tssbRemoveApplication_DropDownItemClicked);
		((ToolStripItem)tssbRemoveApplication).Click += tssbRemoveApplication_Click;
		((ToolStripItem)tsSep3).Name = "tsSep3";
		((ToolStripItem)tsSep3).Size = new Size(6, 25);
		((ToolStripItem)tsbExportProfiles).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripDropDownItem)tsbExportProfiles).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[4]
		{
			(ToolStripItem)exportCurrentProfileOnlyToolStripMenuItem,
			(ToolStripItem)exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem,
			(ToolStripItem)exportUserdefinedProfilesToolStripMenuItem,
			(ToolStripItem)exportAllProfilesNVIDIATextFormatToolStripMenuItem
		});
		((ToolStripItem)tsbExportProfiles).Image = (Image)(object)Resources.export1;
		((ToolStripItem)tsbExportProfiles).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbExportProfiles).Name = "tsbExportProfiles";
		((ToolStripItem)tsbExportProfiles).Size = new Size(36, 22);
		((ToolStripItem)tsbExportProfiles).Text = "Export user defined profiles";
		((ToolStripItem)tsbExportProfiles).Click += tsbExportProfiles_Click;
		((ToolStripItem)exportCurrentProfileOnlyToolStripMenuItem).Name = "exportCurrentProfileOnlyToolStripMenuItem";
		((ToolStripItem)exportCurrentProfileOnlyToolStripMenuItem).Size = new Size(343, 22);
		((ToolStripItem)exportCurrentProfileOnlyToolStripMenuItem).Text = "Export current profile only";
		((ToolStripItem)exportCurrentProfileOnlyToolStripMenuItem).Click += exportCurrentProfileOnlyToolStripMenuItem_Click;
		((ToolStripItem)exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem).Name = "exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem";
		((ToolStripItem)exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem).Size = new Size(343, 22);
		((ToolStripItem)exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem).Text = "Export current profile including predefined settings";
		((ToolStripItem)exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem).Click += exportCurrentProfileIncludingPredefinedSettingsToolStripMenuItem_Click;
		((ToolStripItem)exportUserdefinedProfilesToolStripMenuItem).Name = "exportUserdefinedProfilesToolStripMenuItem";
		((ToolStripItem)exportUserdefinedProfilesToolStripMenuItem).Size = new Size(343, 22);
		((ToolStripItem)exportUserdefinedProfilesToolStripMenuItem).Text = "Export all customized profiles";
		((ToolStripItem)exportUserdefinedProfilesToolStripMenuItem).Click += exportUserdefinedProfilesToolStripMenuItem_Click;
		((ToolStripItem)exportAllProfilesNVIDIATextFormatToolStripMenuItem).Name = "exportAllProfilesNVIDIATextFormatToolStripMenuItem";
		((ToolStripItem)exportAllProfilesNVIDIATextFormatToolStripMenuItem).Size = new Size(343, 22);
		((ToolStripItem)exportAllProfilesNVIDIATextFormatToolStripMenuItem).Text = "Export all driver profiles (NVIDIA Text Format)";
		((ToolStripItem)exportAllProfilesNVIDIATextFormatToolStripMenuItem).Click += exportAllProfilesNVIDIATextFormatToolStripMenuItem_Click;
		((ToolStripItem)tsbImportProfiles).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripDropDownItem)tsbImportProfiles).DropDownItems.AddRange((ToolStripItem[])(object)new ToolStripItem[2]
		{
			(ToolStripItem)importProfilesToolStripMenuItem,
			(ToolStripItem)importAllProfilesNVIDIATextFormatToolStripMenuItem
		});
		((ToolStripItem)tsbImportProfiles).Image = (Image)(object)Resources.import1;
		((ToolStripItem)tsbImportProfiles).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbImportProfiles).Name = "tsbImportProfiles";
		((ToolStripItem)tsbImportProfiles).Size = new Size(36, 22);
		((ToolStripItem)tsbImportProfiles).Text = "Import user defined profiles";
		((ToolStripItem)tsbImportProfiles).Click += tsbImportProfiles_Click;
		((ToolStripItem)importProfilesToolStripMenuItem).Name = "importProfilesToolStripMenuItem";
		((ToolStripItem)importProfilesToolStripMenuItem).Size = new Size(363, 22);
		((ToolStripItem)importProfilesToolStripMenuItem).Text = "Import profile(s)";
		((ToolStripItem)importProfilesToolStripMenuItem).Click += importProfilesToolStripMenuItem_Click;
		((ToolStripItem)importAllProfilesNVIDIATextFormatToolStripMenuItem).Name = "importAllProfilesNVIDIATextFormatToolStripMenuItem";
		((ToolStripItem)importAllProfilesNVIDIATextFormatToolStripMenuItem).Size = new Size(363, 22);
		((ToolStripItem)importAllProfilesNVIDIATextFormatToolStripMenuItem).Text = "Import (replace) all driver profiles (NVIDIA Text Format)";
		((ToolStripItem)importAllProfilesNVIDIATextFormatToolStripMenuItem).Click += importAllProfilesNVIDIATextFormatToolStripMenuItem_Click;
		((ToolStripItem)tsSep4).Name = "tsSep4";
		((ToolStripItem)tsSep4).Size = new Size(6, 25);
		tscbShowCustomSettingNamesOnly.CheckOnClick = true;
		((ToolStripItem)tscbShowCustomSettingNamesOnly).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tscbShowCustomSettingNamesOnly).Image = (Image)(object)Resources.filter_user;
		((ToolStripItem)tscbShowCustomSettingNamesOnly).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tscbShowCustomSettingNamesOnly).Name = "tscbShowCustomSettingNamesOnly";
		((ToolStripItem)tscbShowCustomSettingNamesOnly).Size = new Size(24, 22);
		((ToolStripItem)tscbShowCustomSettingNamesOnly).Text = "Show the settings and values from CustomSettingNames file only.";
		tscbShowCustomSettingNamesOnly.CheckedChanged += cbCustomSettingsOnly_CheckedChanged;
		((ToolStripItem)tsSep5).Name = "tsSep5";
		((ToolStripItem)tsSep5).Size = new Size(6, 25);
		tscbShowScannedUnknownSettings.CheckOnClick = true;
		((ToolStripItem)tscbShowScannedUnknownSettings).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tscbShowScannedUnknownSettings).Enabled = false;
		((ToolStripItem)tscbShowScannedUnknownSettings).Image = (Image)(object)Resources.find_set2;
		((ToolStripItem)tscbShowScannedUnknownSettings).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tscbShowScannedUnknownSettings).Name = "tscbShowScannedUnknownSettings";
		((ToolStripItem)tscbShowScannedUnknownSettings).Size = new Size(24, 22);
		((ToolStripItem)tscbShowScannedUnknownSettings).Text = "Show unknown settings from NVIDIA predefined profiles";
		((ToolStripItem)tscbShowScannedUnknownSettings).Click += tscbShowScannedUnknownSettings_Click;
		((ToolStripItem)tsbBitValueEditor).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)tsbBitValueEditor).Image = (Image)(object)Resources.text_binary;
		((ToolStripItem)tsbBitValueEditor).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbBitValueEditor).Name = "tsbBitValueEditor";
		((ToolStripItem)tsbBitValueEditor).Size = new Size(24, 22);
		((ToolStripItem)tsbBitValueEditor).Text = "Show bit value editor.";
		((ToolStripItem)tsbBitValueEditor).Click += tsbBitValueEditor_Click;
		((ToolStripItem)tsSep6).Name = "tsSep6";
		((ToolStripItem)tsSep6).Size = new Size(6, 25);
		((ToolStripItem)tsbApplyProfile).Alignment = (ToolStripItemAlignment)1;
		((ToolStripItem)tsbApplyProfile).Image = (Image)(object)Resources.apply;
		((ToolStripItem)tsbApplyProfile).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)tsbApplyProfile).Name = "tsbApplyProfile";
		((ToolStripItem)tsbApplyProfile).Overflow = (ToolStripItemOverflow)0;
		((ToolStripItem)tsbApplyProfile).Size = new Size(109, 22);
		((ToolStripItem)tsbApplyProfile).Text = "Apply changes";
		((ToolStripItem)tsbApplyProfile).TextAlign = (ContentAlignment)64;
		((ToolStripItem)tsbApplyProfile).Click += tsbApplyProfile_Click;
		((Control)btnResetValue).Anchor = (AnchorStyles)9;
		((Control)btnResetValue).Enabled = false;
		((ButtonBase)btnResetValue).Image = (Image)(object)Resources.nv_btn;
		((Control)btnResetValue).Location = new Point(732, 175);
		((Control)btnResetValue).Margin = new Padding(0, 1, 0, 0);
		((Control)btnResetValue).Name = "btnResetValue";
		((Control)btnResetValue).Size = new Size(25, 19);
		((Control)btnResetValue).TabIndex = 7;
		((ButtonBase)btnResetValue).UseVisualStyleBackColor = true;
		((Control)btnResetValue).Click += btnResetValue_Click;
		((Control)lblApplications).Anchor = (AnchorStyles)13;
		((Control)lblApplications).BackColor = Color.FromArgb(118, 185, 0);
		lblApplications.BorderStyle = (BorderStyle)2;
		((Control)lblApplications).ForeColor = Color.White;
		((Control)lblApplications).Location = new Point(12, 32);
		((Control)lblApplications).Margin = new Padding(4, 0, 4, 0);
		((Control)lblApplications).Name = "lblApplications";
		((Control)lblApplications).Size = new Size(840, 17);
		((Control)lblApplications).TabIndex = 25;
		((Control)lblApplications).Text = "fsagame.exe, bond.exe, herozero.exe";
		((Control)lblApplications).DoubleClick += tsbAddApplication_Click;
		((ToolStripItem)toolStripButton5).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)toolStripButton5).Image = (Image)componentResourceManager.GetObject("toolStripButton5.Image");
		((ToolStripItem)toolStripButton5).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripButton5).Name = "toolStripButton5";
		((ToolStripItem)toolStripButton5).Size = new Size(23, 22);
		((ToolStripItem)toolStripButton5).Text = "toolStripButton5";
		((ToolStripItem)toolStripLabel2).Name = "toolStripLabel2";
		((ToolStripItem)toolStripLabel2).Size = new Size(86, 22);
		((ToolStripItem)toolStripLabel2).Text = "toolStripLabel2";
		((ToolStripItem)toolStripButton6).DisplayStyle = (ToolStripItemDisplayStyle)2;
		((ToolStripItem)toolStripButton6).Image = (Image)componentResourceManager.GetObject("toolStripButton6.Image");
		((ToolStripItem)toolStripButton6).ImageTransparentColor = Color.Magenta;
		((ToolStripItem)toolStripButton6).Name = "toolStripButton6";
		((ToolStripItem)toolStripButton6).Size = new Size(23, 22);
		((ToolStripItem)toolStripButton6).Text = "toolStripButton6";
		ilCombo.ColorDepth = (ColorDepth)8;
		ilCombo.ImageSize = new Size(16, 16);
		ilCombo.TransparentColor = Color.Transparent;
		((Control)cbValues).BackColor = SystemColors.Window;
		((ListControl)cbValues).FormattingEnabled = true;
		((Control)cbValues).Location = new Point(524, 175);
		((Control)cbValues).Margin = new Padding(4, 0, 4, 0);
		((Control)cbValues).Name = "cbValues";
		((Control)cbValues).Size = new Size(72, 21);
		((Control)cbValues).TabIndex = 5;
		((Control)cbValues).Visible = false;
		((ListControl)cbValues).SelectedValueChanged += cbValues_SelectedValueChanged;
		((Control)cbValues).Leave += cbValues_Leave;
		((Control)lblWidth96).Location = new Point(77, 233);
		((Control)lblWidth96).Margin = new Padding(4, 0, 4, 0);
		((Control)lblWidth96).Name = "lblWidth96";
		((Control)lblWidth96).Size = new Size(96, 18);
		((Control)lblWidth96).TabIndex = 77;
		((Control)lblWidth96).Text = "96";
		((Control)lblWidth96).Visible = false;
		((Control)lblWidth330).Location = new Point(77, 210);
		((Control)lblWidth330).Margin = new Padding(4, 0, 4, 0);
		((Control)lblWidth330).Name = "lblWidth330";
		((Control)lblWidth330).Size = new Size(330, 22);
		((Control)lblWidth330).TabIndex = 78;
		((Control)lblWidth330).Text = "330 (Helper Labels for DPI Scaling)";
		((Control)lblWidth330).Visible = false;
		((Control)lblWidth16).Location = new Point(77, 269);
		((Control)lblWidth16).Margin = new Padding(4, 0, 4, 0);
		((Control)lblWidth16).Name = "lblWidth16";
		((Control)lblWidth16).Size = new Size(16, 18);
		((Control)lblWidth16).TabIndex = 79;
		((Control)lblWidth16).Text = "16";
		((Control)lblWidth16).Visible = false;
		((Control)lblWidth30).Location = new Point(77, 251);
		((Control)lblWidth30).Margin = new Padding(4, 0, 4, 0);
		((Control)lblWidth30).Name = "lblWidth30";
		((Control)lblWidth30).Size = new Size(30, 18);
		((Control)lblWidth30).TabIndex = 80;
		((Control)lblWidth30).Text = "30";
		((Control)lblWidth30).Visible = false;
		((ListView)lvSettings).Columns.AddRange((ColumnHeader[])(object)new ColumnHeader[3] { chSettingID, chSettingValue, chSettingValueHex });
		((Control)lvSettings).Dock = (DockStyle)5;
		((ListView)lvSettings).FullRowSelect = true;
		((ListView)lvSettings).GridLines = true;
		((ListView)lvSettings).HeaderStyle = (ColumnHeaderStyle)0;
		((ListView)lvSettings).HideSelection = false;
		((Control)lvSettings).Location = new Point(0, 0);
		((Control)lvSettings).Margin = new Padding(4);
		((ListView)lvSettings).MultiSelect = false;
		((Control)lvSettings).Name = "lvSettings";
		((ListView)lvSettings).ShowItemToolTips = true;
		((Control)lvSettings).Size = new Size(840, 372);
		((ListView)lvSettings).SmallImageList = ilListView;
		((Control)lvSettings).TabIndex = 2;
		((ListView)lvSettings).UseCompatibleStateImageBehavior = false;
		lvSettings.View = (View)1;
		lvSettings.GroupStateChanged += lvSettings_GroupStateChanged;
		((ListView)lvSettings).ColumnWidthChanging += new ColumnWidthChangingEventHandler(lvSettings_ColumnWidthChanging);
		((ListView)lvSettings).SelectedIndexChanged += lvSettings_SelectedIndexChanged;
		((Control)lvSettings).DoubleClick += lvSettings_DoubleClick;
		((Control)lvSettings).KeyDown += new KeyEventHandler(lvSettings_KeyDown);
		((Control)lvSettings).Resize += lvSettings_Resize;
		chSettingID.Text = "SettingID";
		chSettingID.Width = 330;
		chSettingValue.Text = "SettingValue";
		chSettingValue.Width = 340;
		chSettingValueHex.Text = "SettingValueHex";
		chSettingValueHex.Width = 96;
		((Control)tbSettingDescription).Dock = (DockStyle)2;
		((Control)tbSettingDescription).Location = new Point(0, 372);
		((TextBoxBase)tbSettingDescription).Multiline = true;
		((Control)tbSettingDescription).Name = "tbSettingDescription";
		((TextBoxBase)tbSettingDescription).ReadOnly = true;
		tbSettingDescription.ScrollBars = (ScrollBars)2;
		((Control)tbSettingDescription).Size = new Size(840, 44);
		((Control)tbSettingDescription).TabIndex = 81;
		((Control)tbSettingDescription).Visible = false;
		((Control)pnlListview).Anchor = (AnchorStyles)15;
		((Control)pnlListview).Controls.Add((Control)(object)lvSettings);
		((Control)pnlListview).Controls.Add((Control)(object)txtFilter);
		((Control)pnlListview).Controls.Add((Control)(object)tbSettingDescription);
		((Control)pnlListview).Location = new Point(12, 52);
		((Control)pnlListview).Name = "pnlListview";
		((Control)pnlListview).Size = new Size(840, 416);
		((Control)pnlListview).TabIndex = 82;
		((TextBoxBase)txtFilter).BorderStyle = (BorderStyle)1;
		((Control)txtFilter).Dock = (DockStyle)1;
		((Control)txtFilter).Font = new Font("Segoe UI", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)txtFilter).Location = new Point(0, 0);
		((Control)txtFilter).Margin = new Padding(4, 3, 4, 3);
		((Control)txtFilter).Name = "txtFilter";
		((TextBox)txtFilter).AutoCompleteMode = (AutoCompleteMode)0;
		((TextBox)txtFilter).AutoCompleteSource = (AutoCompleteSource)128;
		((Control)txtFilter).Size = new Size(2118, 35);
		((Control)txtFilter).TabIndex = 82;
		txtFilter.WatermarkText = "Search for setting...";
		((Control)txtFilter).TextChanged += txtFilter_TextChanged;
		((Control)txtFilter).KeyUp += new KeyEventHandler(txtFilter_KeyUp);
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(864, 492);
		((Control)this).Controls.Add((Control)(object)pnlListview);
		((Control)this).Controls.Add((Control)(object)lblWidth30);
		((Control)this).Controls.Add((Control)(object)lblWidth16);
		((Control)this).Controls.Add((Control)(object)lblWidth330);
		((Control)this).Controls.Add((Control)(object)lblWidth96);
		((Control)this).Controls.Add((Control)(object)lblApplications);
		((Control)this).Controls.Add((Control)(object)tsMain);
		((Control)this).Controls.Add((Control)(object)pbMain);
		((Control)this).Controls.Add((Control)(object)btnResetValue);
		((Control)this).Controls.Add((Control)(object)cbValues);
		((Form)this).Margin = new Padding(4);
		((Control)this).MinimumSize = new Size(879, 346);
		((Control)this).Name = "frmDrvSettings";
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "nSpector - Driver Profile Settings";
		((Form)this).Activated += frmDrvSettings_Activated;
		((Form)this).FormClosed += new FormClosedEventHandler(frmDrvSettings_FormClosed);
		((Form)this).Load += frmDrvSettings_Load;
		((Form)this).Shown += frmDrvSettings_Shown;
		((Control)tsMain).ResumeLayout(false);
		((Control)tsMain).PerformLayout();
		((Control)pnlListview).ResumeLayout(false);
		((Control)pnlListview).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
