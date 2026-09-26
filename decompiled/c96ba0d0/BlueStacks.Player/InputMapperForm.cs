using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlueStacks.Player;

public class InputMapperForm : Form
{
	public delegate void EditHandler(string package);

	public delegate void ManageHandler(string package);

	private const int WIDTH = 400;

	private const int HEIGHT = 250;

	private const int PADDING = 10;

	private const int TEXT_HEIGHT = 50;

	private string mPackage;

	private EditHandler mEditHandler;

	private ManageHandler mManageHandler;

	public InputMapperForm(string package, EditHandler editHandler, ManageHandler manageHandler)
	{
		mPackage = package;
		mEditHandler = editHandler;
		mManageHandler = manageHandler;
		((Control)this).Text = "Input Mapper Tool";
		CreateLayout();
	}

	private void CreateLayout()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected Obj, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected Obj, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected Obj, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected Obj, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected Obj, but got Unknown
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected Obj, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected Obj, but got Unknown
		((Form)this).Size = new Size(400, 250);
		((Form)this).FormBorderStyle = (FormBorderStyle)1;
		((Form)this).MinimizeBox = false;
		((Form)this).MaximizeBox = false;
		Label val = new Label
		{
			Text = "Current app: " + ((mPackage != null) ? mPackage : "none"),
			Location = new Point(10, 10),
			Width = ((Form)this).ClientSize.Width - 10
		};
		Button val2 = new Button
		{
			Text = "Edit",
			Location = new Point(10, ((Control)val).Bottom + 10)
		};
		if (mPackage == null)
		{
			((Control)val2).Enabled = false;
		}
		((Control)val2).Click += (object obj, EventArgs evt) =>
		{
			mEditHandler(mPackage);
			((Form)this).Close();
		};
		Label val3 = new Label
		{
			Text = "Edit the input mapper configuration for the current app.  If the current app does not yet have a configuration file, then create one from a template.",
			Location = new Point(((Control)val2).Right + 10, ((Control)val2).Top)
		};
		((Control)val3).Size = new Size(((Form)this).ClientSize.Width - ((Control)val3).Left - 10, 50);
		Button val4 = new Button
		{
			Text = "Manage",
			Location = new Point(10, ((Control)val3).Bottom + 10)
		};
		((Control)val4).Click += (object obj, EventArgs evt) =>
		{
			mManageHandler(mPackage);
			((Form)this).Close();
		};
		Label val5 = new Label
		{
			Text = "Manage all the existing input mapper configurations.  Opens the input mapper folder in Windows Explorer.",
			Location = new Point(((Control)val4).Right + 10, ((Control)val4).Top)
		};
		((Control)val5).Size = new Size(((Form)this).ClientSize.Width - ((Control)val5).Left - 10, 50);
		Button val6 = new Button
		{
			Text = "Cancel",
			Location = new Point(10, ((Control)val5).Bottom + 10)
		};
		((Control)val6).Click += (object obj, EventArgs evt) =>
		{
			((Form)this).Close();
		};
		Label val7 = new Label
		{
			Text = "Close this window.",
			Location = new Point(((Control)val6).Right + 10, ((Control)val6).Top)
		};
		((Control)val7).Size = new Size(((Form)this).ClientSize.Width - ((Control)val7).Left - 10, 50);
		((Control)this).Controls.AddRange(new Control[7]
		{
			(Control)val,
			(Control)val2,
			(Control)val3,
			(Control)val4,
			(Control)val5,
			(Control)val6,
			(Control)val7
		});
	}
}
