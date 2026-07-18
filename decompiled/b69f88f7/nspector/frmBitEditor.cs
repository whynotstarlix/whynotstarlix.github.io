using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using nspector.Common;
using nspector.Common.CustomSettings;

namespace nspector;

internal class frmBitEditor : Form
{
	private uint _Settingid = 0u;

	private frmDrvSettings _SettingsOwner = null;

	private uint _InitValue = 0u;

	private uint _CurrentValue = 0u;

	private IContainer components = null;

	private Button btnClose;

	private Label lValue;

	private Label lFilter;

	private TextBox tbFilter;

	private TextBox textBox1;

	private Button btnDirectApplyStart;

	private GroupBox gbDirectTest;

	private Button btnBrowseGame;

	private TextBox tbGamePath;

	private Label lblGamePath;

	private ListViewEx clbBits;

	private ColumnHeader chBit;

	private ColumnHeader chProfileCount;

	private ColumnHeader chName;

	private ColumnHeader chProfileNames;

	internal frmBitEditor()
	{
		InitializeComponent();
		((Form)this).Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		((Control)this).DoubleBuffered = true;
	}

	internal void ShowDialog(frmDrvSettings SettingsOwner, uint SettingId, uint InitValue, string SettingName)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_Settingid = SettingId;
		_SettingsOwner = SettingsOwner;
		_InitValue = InitValue;
		((Control)this).Text = $"Bit Value Editor - {SettingName}";
		((Form)this).ShowDialog((IWin32Window)(object)SettingsOwner);
	}

	private void frmBitEditor_Load(object sender, EventArgs e)
	{
		SplitBitsFromUnknownSettings();
		((ListView)clbBits).AutoResizeColumns((ColumnHeaderAutoResizeStyle)2);
		SetValue(_InitValue);
	}

	private void SplitBitsFromUnknownSettings()
	{
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Expected O, but got Unknown
		uint num = 0u;
		num = _CurrentValue;
		string[] array = ((Control)tbFilter).Text.Split(new char[1] { ',' });
		((ListView)clbBits).Items.Clear();
		CustomSetting customSetting = DrsServiceLocator.ReferenceSettings?.Settings.FirstOrDefault((CustomSetting s) => s.SettingId == _Settingid);
		CachedSettings cachedSettings = DrsServiceLocator.ScannerService.CachedSettings.FirstOrDefault((CachedSettings x) => x.SettingId == _Settingid);
		for (int num2 = 0; num2 < 32; num2++)
		{
			string text = "";
			uint num3 = 0u;
			if (cachedSettings != null)
			{
				for (int num4 = 0; num4 < cachedSettings.SettingValues.Count; num4++)
				{
					if (((cachedSettings.SettingValues[num4].Value >> num2) & 1) != 1)
					{
						continue;
					}
					if (array.Length == 0)
					{
						text = text + cachedSettings.SettingValues[num4].ProfileNames?.ToString() + ",";
					}
					else
					{
						string[] array2 = cachedSettings.SettingValues[num4].ProfileNames.ToString().Split(new char[1] { ',' });
						for (int num5 = 0; num5 < array2.Length; num5++)
						{
							for (int num6 = 0; num6 < array.Length; num6++)
							{
								if (array2[num5].ToLowerInvariant().Contains(array[num6].ToLower()))
								{
									text = text + array2[num5] + ",";
								}
							}
						}
					}
					num3 += cachedSettings.SettingValues[num4].ValueProfileCount;
				}
			}
			uint mask = (uint)(1 << num2);
			string text2 = "";
			if (customSetting != null)
			{
				CustomSettingValue customSettingValue = customSetting.SettingValues.FirstOrDefault((CustomSettingValue v) => v.SettingValue == mask);
				if (customSettingValue != null)
				{
					text2 = customSettingValue.UserfriendlyName;
					if (text2.Contains("("))
					{
						text2 = text2.Substring(0, text2.IndexOf("(") - 1);
					}
				}
			}
			((ListView)clbBits).Items.Add(new ListViewItem(new string[4]
			{
				$"#{num2:00}",
				text2,
				num3.ToString(),
				text
			}));
		}
		SetValue(num);
	}

	private void updateValue(bool changeState, int changedIndex)
	{
		uint num = 0u;
		for (int i = 0; i < ((ListView)clbBits).Items.Count; i++)
		{
			if ((((ListView)clbBits).Items[i].Checked && changedIndex != i) || (changeState && changedIndex == i))
			{
				num |= (uint)(1 << i);
			}
		}
		UpdateCurrent(num);
	}

	private void UpdateValue()
	{
		uint num = 0u;
		for (int i = 0; i < ((ListView)clbBits).Items.Count; i++)
		{
			if (((ListView)clbBits).Items[i].Checked)
			{
				num |= (uint)(1 << i);
			}
		}
		UpdateCurrent(num);
	}

	private void SetValue(uint val)
	{
		for (int i = 0; i < ((ListView)clbBits).Items.Count; i++)
		{
			if (((val >> i) & 1) == 1)
			{
				((ListView)clbBits).Items[i].Checked = true;
			}
			else
			{
				((ListView)clbBits).Items[i].Checked = false;
			}
		}
		UpdateValue();
	}

	private void UpdateCurrent(uint val)
	{
		_CurrentValue = val;
		((Control)textBox1).Text = "0x" + val.ToString("X8");
	}

	private void UpdateCurrent(string text)
	{
		uint num = DrsUtil.ParseDwordByInputSafe(text);
		UpdateCurrent(num);
		SetValue(num);
	}

	private void clbBits_ItemCheck(object sender, ItemCheckEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		updateValue((int)e.NewValue == 1, e.Index);
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		_SettingsOwner.SetSelectedDwordValue(_CurrentValue);
		((Form)this).Close();
	}

	private void tbFilter_TextChanged(object sender, EventArgs e)
	{
		SplitBitsFromUnknownSettings();
	}

	private void numericUpDown1_ValueChanged(object sender, EventArgs e)
	{
		SplitBitsFromUnknownSettings();
	}

	private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyValue == 13)
		{
			UpdateCurrent(((Control)textBox1).Text);
		}
	}

	private void textBox1_Leave(object sender, EventArgs e)
	{
		UpdateCurrent(((Control)textBox1).Text);
	}

	private void ApplyValueToProfile(uint val)
	{
		DrsServiceLocator.SettingService.SetDwordValueToProfile(_SettingsOwner._CurrentProfile, _Settingid, val);
	}

	private async void btnDirectApply_Click(object sender, EventArgs e)
	{
		ApplyValueToProfile(_CurrentValue);
		await CheckIfSettingIsStored();
		if (File.Exists(((Control)tbGamePath).Text))
		{
			Process.Start(((Control)tbGamePath).Text);
		}
	}

	private async Task CheckIfSettingIsStored()
	{
		await Task.Run(async delegate
		{
			while (_CurrentValue != DrsServiceLocator.SettingService.GetDwordValueFromProfile(_SettingsOwner._CurrentProfile, _Settingid, returnDefaultValue: false, forceDedicatedScope: true))
			{
				await Task.Delay(50);
			}
		});
	}

	private void btnBrowseGame_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		OpenFileDialog val = new OpenFileDialog();
		((FileDialog)val).DefaultExt = "*.exe";
		((FileDialog)val).Filter = "Applications|*.exe";
		((FileDialog)val).DereferenceLinks = false;
		if ((int)((CommonDialog)val).ShowDialog() == 1)
		{
			((Control)tbGamePath).Text = ((FileDialog)val).FileName;
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
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
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Expected O, but got Unknown
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Expected O, but got Unknown
		btnClose = new Button();
		lValue = new Label();
		lFilter = new Label();
		tbFilter = new TextBox();
		textBox1 = new TextBox();
		btnDirectApplyStart = new Button();
		gbDirectTest = new GroupBox();
		btnBrowseGame = new Button();
		tbGamePath = new TextBox();
		lblGamePath = new Label();
		clbBits = new ListViewEx();
		chBit = new ColumnHeader();
		chName = new ColumnHeader();
		chProfileCount = new ColumnHeader();
		chProfileNames = new ColumnHeader();
		((Control)gbDirectTest).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)btnClose).Anchor = (AnchorStyles)10;
		btnClose.DialogResult = (DialogResult)2;
		((Control)btnClose).Location = new Point(731, 645);
		((Control)btnClose).Name = "btnClose";
		((Control)btnClose).Size = new Size(106, 23);
		((Control)btnClose).TabIndex = 1;
		((Control)btnClose).Text = "Apply && Close";
		((ButtonBase)btnClose).UseVisualStyleBackColor = true;
		((Control)btnClose).Click += btnClose_Click;
		((Control)lValue).Anchor = (AnchorStyles)6;
		((Control)lValue).AutoSize = true;
		((Control)lValue).Location = new Point(17, 650);
		((Control)lValue).Name = "lValue";
		((Control)lValue).Size = new Size(37, 13);
		((Control)lValue).TabIndex = 2;
		((Control)lValue).Text = "Value:";
		((Control)lFilter).Anchor = (AnchorStyles)6;
		((Control)lFilter).AutoSize = true;
		((Control)lFilter).Location = new Point(150, 650);
		((Control)lFilter).Name = "lFilter";
		((Control)lFilter).Size = new Size(64, 13);
		((Control)lFilter).TabIndex = 23;
		((Control)lFilter).Text = "Profile Filter:";
		((Control)tbFilter).Anchor = (AnchorStyles)14;
		((Control)tbFilter).Location = new Point(219, 647);
		((Control)tbFilter).Name = "tbFilter";
		((Control)tbFilter).Size = new Size(506, 20);
		((Control)tbFilter).TabIndex = 24;
		((Control)tbFilter).TextChanged += tbFilter_TextChanged;
		((Control)textBox1).Anchor = (AnchorStyles)6;
		((Control)textBox1).Location = new Point(59, 647);
		((Control)textBox1).Name = "textBox1";
		((Control)textBox1).Size = new Size(70, 20);
		((Control)textBox1).TabIndex = 31;
		((Control)textBox1).Text = "0x00FF00FF";
		((Control)textBox1).Leave += textBox1_Leave;
		((Control)textBox1).PreviewKeyDown += new PreviewKeyDownEventHandler(textBox1_PreviewKeyDown);
		((Control)btnDirectApplyStart).Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)btnDirectApplyStart).Location = new Point(5, 15);
		((Control)btnDirectApplyStart).Name = "btnDirectApplyStart";
		((Control)btnDirectApplyStart).Size = new Size(84, 34);
		((Control)btnDirectApplyStart).TabIndex = 32;
		((Control)btnDirectApplyStart).Text = "GO!";
		((ButtonBase)btnDirectApplyStart).UseVisualStyleBackColor = true;
		((Control)btnDirectApplyStart).Click += btnDirectApply_Click;
		((Control)gbDirectTest).Anchor = (AnchorStyles)14;
		((Control)gbDirectTest).Controls.Add((Control)(object)btnBrowseGame);
		((Control)gbDirectTest).Controls.Add((Control)(object)tbGamePath);
		((Control)gbDirectTest).Controls.Add((Control)(object)lblGamePath);
		((Control)gbDirectTest).Controls.Add((Control)(object)btnDirectApplyStart);
		((Control)gbDirectTest).Location = new Point(14, 586);
		((Control)gbDirectTest).Name = "gbDirectTest";
		((Control)gbDirectTest).Size = new Size(823, 53);
		((Control)gbDirectTest).TabIndex = 33;
		gbDirectTest.TabStop = false;
		((Control)gbDirectTest).Text = "Quick Bit Value Tester (stores this setting value to the current profile and immediately starts the game when successful)";
		((Control)btnBrowseGame).Anchor = (AnchorStyles)9;
		((Control)btnBrowseGame).Location = new Point(777, 19);
		((Control)btnBrowseGame).Name = "btnBrowseGame";
		((Control)btnBrowseGame).Size = new Size(33, 23);
		((Control)btnBrowseGame).TabIndex = 35;
		((Control)btnBrowseGame).Text = "...";
		((ButtonBase)btnBrowseGame).UseVisualStyleBackColor = true;
		((Control)btnBrowseGame).Click += btnBrowseGame_Click;
		((Control)tbGamePath).Anchor = (AnchorStyles)13;
		((Control)tbGamePath).Location = new Point(174, 21);
		((Control)tbGamePath).Name = "tbGamePath";
		((Control)tbGamePath).Size = new Size(597, 20);
		((Control)tbGamePath).TabIndex = 34;
		((Control)lblGamePath).AutoSize = true;
		((Control)lblGamePath).Location = new Point(95, 23);
		((Control)lblGamePath).Name = "lblGamePath";
		((Control)lblGamePath).Size = new Size(73, 13);
		((Control)lblGamePath).TabIndex = 33;
		((Control)lblGamePath).Text = "Game to start:";
		((Control)clbBits).Anchor = (AnchorStyles)15;
		((ListView)clbBits).CheckBoxes = true;
		((ListView)clbBits).Columns.AddRange((ColumnHeader[])(object)new ColumnHeader[4] { chBit, chName, chProfileCount, chProfileNames });
		((ListView)clbBits).FullRowSelect = true;
		((ListView)clbBits).GridLines = true;
		((ListView)clbBits).HeaderStyle = (ColumnHeaderStyle)1;
		((ListView)clbBits).HideSelection = false;
		((Control)clbBits).Location = new Point(10, 10);
		((Control)clbBits).Margin = new Padding(2, 2, 2, 2);
		((ListView)clbBits).MultiSelect = false;
		((Control)clbBits).Name = "clbBits";
		((ListView)clbBits).ShowGroups = false;
		((Control)clbBits).Size = new Size(829, 572);
		((Control)clbBits).TabIndex = 34;
		((ListView)clbBits).UseCompatibleStateImageBehavior = false;
		clbBits.View = (View)1;
		((ListView)clbBits).ItemCheck += new ItemCheckEventHandler(clbBits_ItemCheck);
		chBit.Text = "Bit";
		chName.Text = "Name";
		chName.Width = 200;
		chProfileCount.Text = "Count";
		chProfileNames.Text = "Profiles";
		chProfileNames.Width = 4000;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Form)this).ClientSize = new Size(847, 678);
		((Control)this).Controls.Add((Control)(object)clbBits);
		((Control)this).Controls.Add((Control)(object)gbDirectTest);
		((Control)this).Controls.Add((Control)(object)textBox1);
		((Control)this).Controls.Add((Control)(object)tbFilter);
		((Control)this).Controls.Add((Control)(object)lFilter);
		((Control)this).Controls.Add((Control)(object)lValue);
		((Control)this).Controls.Add((Control)(object)btnClose);
		((Form)this).FormBorderStyle = (FormBorderStyle)6;
		((Control)this).MinimumSize = new Size(686, 495);
		((Control)this).Name = "frmBitEditor";
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "Bit Value Editor";
		((Form)this).Load += frmBitEditor_Load;
		((Control)gbDirectTest).ResumeLayout(false);
		((Control)gbDirectTest).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
