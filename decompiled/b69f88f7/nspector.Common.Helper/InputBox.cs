using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using nspector.Properties;

namespace nspector.Common.Helper;

internal class InputBox
{
	internal static DialogResult Show(string title, string promptText, ref string value, List<string> invalidInputs, string mandatoryFormatRegExPattern, int maxLength, bool allowExeBrowse = false)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		Form val = new Form();
		Label val2 = new Label();
		TextBox textBox = new TextBox();
		Button buttonOk = new Button();
		Button val3 = new Button();
		Button val4 = new Button();
		PictureBox imageBox = new PictureBox();
		EventHandler eventHandler = delegate
		{
			bool flag = Regex.IsMatch(((Control)textBox).Text, mandatoryFormatRegExPattern);
			if (((Control)textBox).Text == "" || ((Control)textBox).Text.Length > maxLength || !flag)
			{
				imageBox.Image = (Image)(object)Resources.ieframe_1_18212;
				((Control)buttonOk).Enabled = false;
			}
			else
			{
				foreach (string invalidInput in invalidInputs)
				{
					if (((Control)textBox).Text.ToUpper() == invalidInput.ToUpper())
					{
						imageBox.Image = (Image)(object)Resources.ieframe_1_18212;
						((Control)buttonOk).Enabled = false;
						return;
					}
				}
				imageBox.Image = (Image)(object)Resources.ieframe_1_31073_002;
				((Control)buttonOk).Enabled = true;
			}
		};
		EventHandler eventHandler2 = delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			OpenFileDialog val5 = new OpenFileDialog();
			((FileDialog)val5).DefaultExt = "*.exe";
			((FileDialog)val5).Filter = "Application EXE Name|*.exe|Application Absolute Path|*.exe";
			if ((int)((CommonDialog)val5).ShowDialog() == 1)
			{
				string text = new FileInfo(((FileDialog)val5).FileName).Name;
				if (((FileDialog)val5).FilterIndex == 2)
				{
					text = ((FileDialog)val5).FileName;
				}
				((Control)textBox).Text = text;
			}
		};
		((Control)textBox).TextChanged += eventHandler;
		((Control)val).Text = title;
		((Control)val2).Text = promptText;
		((Control)textBox).Text = value;
		((TextBoxBase)textBox).MaxLength = maxLength;
		imageBox.Image = (Image)(object)Resources.ieframe_1_18212;
		((Control)buttonOk).Text = "OK";
		((Control)val3).Text = "Cancel";
		((Control)val4).Text = "Browse...";
		buttonOk.DialogResult = (DialogResult)1;
		val3.DialogResult = (DialogResult)2;
		((Control)buttonOk).Enabled = false;
		((Control)val2).SetBounds(Dpi(9), Dpi(20), Dpi(372), Dpi(13));
		((Control)textBox).SetBounds(Dpi(12), Dpi(44), Dpi(352), Dpi(20));
		((Control)buttonOk).SetBounds(Dpi(224), Dpi(72), Dpi(75), Dpi(23));
		((Control)val3).SetBounds(Dpi(305), Dpi(72), Dpi(75), Dpi(23));
		if (allowExeBrowse)
		{
			((Control)textBox).SetBounds(Dpi(12), Dpi(44), Dpi(286), Dpi(20));
			((Control)val4).SetBounds(Dpi(305), Dpi(39), Dpi(75), Dpi(23));
			((Control)val4).Click += eventHandler2;
		}
		((Control)imageBox).SetBounds(Dpi(368), Dpi(44), Dpi(16), Dpi(16));
		((Control)val2).AutoSize = true;
		((Control)val2).Anchor = (AnchorStyles)13;
		((Control)imageBox).Anchor = (AnchorStyles)9;
		((Control)textBox).Anchor = (AnchorStyles)13;
		((Control)buttonOk).Anchor = (AnchorStyles)10;
		((Control)val3).Anchor = (AnchorStyles)10;
		((Control)val4).Anchor = (AnchorStyles)10;
		val.ClientSize = new Size(Dpi(396), Dpi(107));
		val.ClientSize = new Size(Math.Max(Dpi(300), ((Control)val2).Right + Dpi(10)), val.ClientSize.Height);
		((Control)val).MinimumSize = val.Size;
		((Control)val).MaximumSize = new Size(((Control)val).MinimumSize.Width * 2, ((Control)val).MinimumSize.Height);
		((Control)val).Controls.AddRange((Control[])(object)new Control[4]
		{
			(Control)val2,
			(Control)textBox,
			(Control)buttonOk,
			(Control)val3
		});
		if (!allowExeBrowse)
		{
			((Control)val).Controls.Add((Control)(object)imageBox);
		}
		else
		{
			((Control)val).Controls.Add((Control)(object)val4);
		}
		val.ShowIcon = false;
		val.FormBorderStyle = (FormBorderStyle)4;
		val.StartPosition = (FormStartPosition)4;
		val.MinimizeBox = false;
		val.MaximizeBox = false;
		val.AcceptButton = (IButtonControl)(object)buttonOk;
		val.CancelButton = (IButtonControl)(object)val3;
		eventHandler(val, new EventArgs());
		DialogResult result = val.ShowDialog();
		value = ((Control)textBox).Text;
		return result;
	}

	private static int Dpi(int input)
	{
		return (int)Math.Round((double)input * frmDrvSettings.ScaleFactor);
	}
}
