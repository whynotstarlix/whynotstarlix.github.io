using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using nspector.Common;

namespace nspector;

internal class frmExportProfiles : Form
{
	private frmDrvSettings settingsOwner = null;

	private IContainer components = null;

	private ListView lvProfiles;

	private ColumnHeader chProfileName;

	private Label lProfiles;

	private Button btnExport;

	private Button btnCancel;

	private Button btnSelAll;

	private Button btnSelNone;

	private Button btnInvertSelection;

	private CheckBox cbIncludePredefined;

	internal frmExportProfiles()
	{
		InitializeComponent();
		((Form)this).Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		((Control)this).DoubleBuffered = true;
	}

	internal void ShowDialog(frmDrvSettings SettingsOwner)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		settingsOwner = SettingsOwner;
		((Control)this).Text = "Profile Export";
		updateProfileList();
		((Form)this).ShowDialog();
	}

	private void updateProfileList()
	{
		lvProfiles.Items.Clear();
		if (settingsOwner == null)
		{
			return;
		}
		foreach (string modifiedProfile in DrsServiceLocator.ScannerService.ModifiedProfiles)
		{
			lvProfiles.Items.Add(modifiedProfile);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		((Form)this).Close();
	}

	private void btnSelAll_Click(object sender, EventArgs e)
	{
		for (int i = 0; i < lvProfiles.Items.Count; i++)
		{
			lvProfiles.Items[i].Checked = true;
		}
	}

	private void btnSelNone_Click(object sender, EventArgs e)
	{
		for (int i = 0; i < lvProfiles.Items.Count; i++)
		{
			lvProfiles.Items[i].Checked = false;
		}
	}

	private void btnInvertSelection_Click(object sender, EventArgs e)
	{
		for (int i = 0; i < lvProfiles.Items.Count; i++)
		{
			lvProfiles.Items[i].Checked = !lvProfiles.Items[i].Checked;
		}
	}

	private void btnExport_Click(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		SaveFileDialog val = new SaveFileDialog();
		((FileDialog)val).DefaultExt = "*.nip";
		((FileDialog)val).Filter = Application.ProductName + " Profiles|*.nip";
		if ((int)((CommonDialog)val).ShowDialog() != 1)
		{
			return;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < lvProfiles.Items.Count; i++)
		{
			if (lvProfiles.Items[i].Checked)
			{
				list.Add(lvProfiles.Items[i].Text);
			}
		}
		DrsServiceLocator.ImportService.ExportProfiles(list, ((FileDialog)val).FileName, cbIncludePredefined.Checked);
		if (list.Count > 0)
		{
			if ((int)MessageBox.Show("Export succeeded.\r\n\r\nWould you like to continue exporting profiles?", "Profiles Export", (MessageBoxButtons)4, (MessageBoxIcon)32) == 7)
			{
				((Form)this).Close();
			}
		}
		else
		{
			MessageBox.Show("Nothing to export");
		}
	}

	private void lvProfiles_ItemChecked(object sender, ItemCheckedEventArgs e)
	{
		int num = 0;
		for (int i = 0; i < lvProfiles.Items.Count; i++)
		{
			if (lvProfiles.Items[i].Checked)
			{
				num++;
			}
		}
		if (num > 0)
		{
			((Control)btnExport).Enabled = true;
		}
		else
		{
			((Control)btnExport).Enabled = false;
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
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		lvProfiles = new ListView();
		chProfileName = new ColumnHeader();
		lProfiles = new Label();
		btnExport = new Button();
		btnCancel = new Button();
		btnSelAll = new Button();
		btnSelNone = new Button();
		btnInvertSelection = new Button();
		cbIncludePredefined = new CheckBox();
		((Control)this).SuspendLayout();
		((Control)lvProfiles).Anchor = (AnchorStyles)15;
		lvProfiles.CheckBoxes = true;
		lvProfiles.Columns.AddRange((ColumnHeader[])(object)new ColumnHeader[1] { chProfileName });
		lvProfiles.FullRowSelect = true;
		lvProfiles.GridLines = true;
		lvProfiles.HeaderStyle = (ColumnHeaderStyle)0;
		((Control)lvProfiles).Location = new Point(12, 40);
		lvProfiles.MultiSelect = false;
		((Control)lvProfiles).Name = "lvProfiles";
		((Control)lvProfiles).Size = new Size(492, 382);
		lvProfiles.Sorting = (SortOrder)1;
		((Control)lvProfiles).TabIndex = 0;
		lvProfiles.UseCompatibleStateImageBehavior = false;
		lvProfiles.View = (View)1;
		lvProfiles.ItemChecked += new ItemCheckedEventHandler(lvProfiles_ItemChecked);
		chProfileName.Text = "ProfileName";
		chProfileName.Width = 420;
		((Control)lProfiles).AutoSize = true;
		((Control)lProfiles).Location = new Point(9, 12);
		((Control)lProfiles).Name = "lProfiles";
		((Control)lProfiles).Size = new Size(184, 13);
		((Control)lProfiles).TabIndex = 1;
		((Control)lProfiles).Text = "Select the profiles you want to export:";
		((Control)btnExport).Anchor = (AnchorStyles)10;
		((Control)btnExport).Enabled = false;
		((Control)btnExport).Location = new Point(429, 451);
		((Control)btnExport).Name = "btnExport";
		((Control)btnExport).Size = new Size(75, 23);
		((Control)btnExport).TabIndex = 2;
		((Control)btnExport).Text = "Export";
		((ButtonBase)btnExport).UseVisualStyleBackColor = true;
		((Control)btnExport).Click += btnExport_Click;
		((Control)btnCancel).Anchor = (AnchorStyles)10;
		((Control)btnCancel).Location = new Point(348, 451);
		((Control)btnCancel).Name = "btnCancel";
		((Control)btnCancel).Size = new Size(75, 23);
		((Control)btnCancel).TabIndex = 3;
		((Control)btnCancel).Text = "Cancel";
		((ButtonBase)btnCancel).UseVisualStyleBackColor = true;
		((Control)btnCancel).Click += btnCancel_Click;
		((Control)btnSelAll).Anchor = (AnchorStyles)6;
		((Control)btnSelAll).Location = new Point(12, 451);
		((Control)btnSelAll).Name = "btnSelAll";
		((Control)btnSelAll).Size = new Size(75, 23);
		((Control)btnSelAll).TabIndex = 4;
		((Control)btnSelAll).Text = "Select All";
		((ButtonBase)btnSelAll).UseVisualStyleBackColor = true;
		((Control)btnSelAll).Click += btnSelAll_Click;
		((Control)btnSelNone).Anchor = (AnchorStyles)6;
		((Control)btnSelNone).Location = new Point(93, 451);
		((Control)btnSelNone).Name = "btnSelNone";
		((Control)btnSelNone).Size = new Size(75, 23);
		((Control)btnSelNone).TabIndex = 4;
		((Control)btnSelNone).Text = "Select None";
		((ButtonBase)btnSelNone).UseVisualStyleBackColor = true;
		((Control)btnSelNone).Click += btnSelNone_Click;
		((Control)btnInvertSelection).Anchor = (AnchorStyles)6;
		((Control)btnInvertSelection).Location = new Point(174, 451);
		((Control)btnInvertSelection).Name = "btnInvertSelection";
		((Control)btnInvertSelection).Size = new Size(100, 23);
		((Control)btnInvertSelection).TabIndex = 4;
		((Control)btnInvertSelection).Text = "Invert Selection";
		((ButtonBase)btnInvertSelection).UseVisualStyleBackColor = true;
		((Control)btnInvertSelection).Click += btnInvertSelection_Click;
		((Control)cbIncludePredefined).Anchor = (AnchorStyles)6;
		((Control)cbIncludePredefined).AutoSize = true;
		((Control)cbIncludePredefined).Location = new Point(12, 428);
		((Control)cbIncludePredefined).Name = "cbIncludePredefined";
		((Control)cbIncludePredefined).Size = new Size(153, 17);
		((Control)cbIncludePredefined).TabIndex = 5;
		((Control)cbIncludePredefined).Text = "Include predefined settings";
		((ButtonBase)cbIncludePredefined).UseVisualStyleBackColor = true;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Form)this).ClientSize = new Size(516, 487);
		((Control)this).Controls.Add((Control)(object)cbIncludePredefined);
		((Control)this).Controls.Add((Control)(object)btnInvertSelection);
		((Control)this).Controls.Add((Control)(object)btnSelNone);
		((Control)this).Controls.Add((Control)(object)btnSelAll);
		((Control)this).Controls.Add((Control)(object)btnCancel);
		((Control)this).Controls.Add((Control)(object)btnExport);
		((Control)this).Controls.Add((Control)(object)lProfiles);
		((Control)this).Controls.Add((Control)(object)lvProfiles);
		((Form)this).MaximizeBox = false;
		((Form)this).MinimizeBox = false;
		((Control)this).MinimumSize = new Size(464, 319);
		((Control)this).Name = "frmExportProfiles";
		((Form)this).StartPosition = (FormStartPosition)4;
		((Control)this).Text = "frmExportProfiles";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
