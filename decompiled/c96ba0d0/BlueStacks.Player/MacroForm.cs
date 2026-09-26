using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;
using Microsoft.VisualBasic;

namespace BlueStacks.Player;

public class MacroForm : Form
{
	private string editComboText = "Editing-New-Combo";

	private static Thread t;

	internal static bool sIsRecording = false;

	internal static Macro mMacro = new Macro();

	internal static DateTime mBaseDateTime;

	internal static MacroForm mInstance = null;

	private IContainer components;

	private DataGridView dataGridView1;

	private SplitContainer splitContainer1;

	private Button bntStartRecord;

	private ComboBox cmbMacros;

	private Button btnStop;

	private Button btnPlay;

	private Button btnSave;

	private Button btnStopRecord;

	private Button btnAdd;

	private ComboBox cmbRepeatBehaviour;

	private Button btnReload;

	internal static MacroForm Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new MacroForm();
			}
			return mInstance;
		}
	}

	private MacroForm()
	{
		InitializeComponent();
		cmbRepeatBehaviour.DataSource = Enum.GetValues(typeof(RepeatBehaviour));
		UpdateUIForNewFile();
	}

	private void btnReload_Click(object sender, EventArgs e)
	{
		UpdateUIForNewFile();
	}

	private void UpdateUIForNewFile()
	{
		((Control)this).Text = "MacroForm - " + InputMapper.Instance.GetMacroFileName(isUserDirectoryRequested: false, InputMapper.Instance.GetPackage());
		List<string> list = ((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros).Keys.ToList();
		list.Add(editComboText);
		cmbMacros.DataSource = list;
	}

	private void btnPlay_Click(object sender, EventArgs e)
	{
		PlayMacro(((ListControl)cmbMacros).SelectedValue.ToString());
	}

	public static void PlayMacro(string MacroName)
	{
		Macro macro;
		if (string.IsNullOrEmpty(MacroName) || !((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros).ContainsKey(MacroName))
		{
			macro = ((IEnumerable<KeyValuePair<string, Macro>>)MacroData.Instance.DictMacros).FirstOrDefault().Value;
		}
		else
		{
			macro = ((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros)[MacroName];
		}
		int windowWidth = ((Control)VMWindow.Instance).Width;
		int windowHeight = ((Control)VMWindow.Instance).Height;
		if (t != null && t.IsAlive)
		{
			AbortReroll();
		}
		t = new Thread(() =>
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected I4, but got Unknown
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected Obj, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Invalid comparison between Unknown and I4
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Expected I4, but got Unknown
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Expected Obj, but got Unknown
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Expected Obj, but got Unknown
			try
			{
				DateTime dateTime = DateTime.Now;
				RepeatBehaviour repeatBehaiour = macro.RepeatBehaiour;
				switch ((int)repeatBehaiour)
				{
				case 0:
					dateTime = DateTime.Now.AddSeconds(2.0);
					break;
				case 1:
					dateTime = DateTime.Now.AddMinutes(5.0);
					break;
				case 2:
					dateTime = DateTime.Now.AddHours(1.0);
					break;
				case 3:
					dateTime = DateTime.Now.AddHours(8.0);
					break;
				case 4:
					dateTime = DateTime.Now.AddYears(5);
					break;
				}
				bool flag = true;
				while (DateTime.Now < dateTime)
				{
					if (flag)
					{
						flag = (int)macro.AndroidCommandRepeatMode > 0;
						foreach (KeyValuePair<string, SerializableDictionary<string, string>> item in (Dictionary<string, SerializableDictionary<string, string>>)(object)macro.DictAndroidCommands)
						{
							try
							{
								JsonParser val = new JsonParser(MultiInstanceStrings.VmName);
								if (item.Key.StartsWith("MacroSleep"))
								{
									Thread.Sleep(Convert.ToInt32(((Dictionary<string, string>)(object)item.Value).Keys.ElementAt(0)));
								}
								else if (item.Key.Equals("runex") && val.GetAppInfoFromPackageName(MacroData.Instance.mPackageName) != null)
								{
									string activity = val.GetAppInfoFromPackageName(MacroData.Instance.mPackageName).Activity;
									VmCmdHandler.RunCommand($"runex {MacroData.Instance.mPackageName}/{activity}", MultiInstanceStrings.VmName);
								}
								else
								{
									RunCommand(item);
								}
							}
							catch
							{
							}
						}
					}
					foreach (KeyValuePair<int, MacroAction> item2 in (Dictionary<int, MacroAction>)(object)macro.DictMacroActions)
					{
						MacroAction value = item2.Value;
						Thread.Sleep((int)value.DelayFromLastAction);
						ActionType actionType = value.ActionType;
						switch ((int)actionType)
						{
						case 2:
						{
							MouseEventArgs e2 = new MouseEventArgs(value.MouseButton, 0, (int)(value.ActionPointX * (double)windowWidth), (int)(value.ActionPointY * (double)windowHeight), 0);
							InputMapper.Instance.HandleMouseDown(null, e2);
							break;
						}
						case 3:
						{
							MouseEventArgs e = new MouseEventArgs(value.MouseButton, 0, (int)(value.ActionPointX * (double)windowWidth), (int)(value.ActionPointY * (double)windowHeight), 0);
							InputMapper.Instance.HandleMouseUp(null, e);
							break;
						}
						}
					}
				}
				t = null;
				HTTPUtils.SendRequestToClient("macroCompleted", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
			catch (ThreadAbortException ex)
			{
				Logger.Error("Error running reroll", new object[1] { ex.ToString() });
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in running reroll. Err : ", new object[1] { ex2.ToString() });
				HTTPUtils.SendRequestToClient("macroCompleted", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
			}
		})
		{
			IsBackground = true
		};
		t.Start();
	}

	internal static void RunCommand(KeyValuePair<string, SerializableDictionary<string, string>> item)
	{
		try
		{
			string key = item.Key;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item2 in (Dictionary<string, string>)(object)item.Value)
			{
				dictionary.Add(item2.Key, item2.Value);
			}
			string text = $"http://127.0.0.1:{Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName)}/{key}";
			Logger.Info("The url being hit is {0}", new object[1] { text });
			string text2 = BstHttpClient.Post(text, dictionary, (Dictionary<string, string>)null, false, MultiInstanceStrings.VmName, 500, 1, 0, false, "bgp64");
			Logger.Info("Resp: {0}", new object[1] { text2 });
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendKeymappingFiledownloadRequest. Err : " + ex.ToString());
		}
	}

	internal void btnStop_Click(object sender, EventArgs e)
	{
		AbortReroll();
	}

	internal static void AbortReroll()
	{
		try
		{
			if (t != null)
			{
				t.Abort();
			}
		}
		catch
		{
		}
	}

	private void bntStartRecord_Click(object sender, EventArgs e)
	{
		((Control)btnStopRecord).Enabled = true;
		((Control)btnAdd).Enabled = false;
		mMacro = new Macro();
		mBaseDateTime = DateTime.Now;
		sIsRecording = true;
		cmbMacros.SelectedItem = editComboText;
		((Control)cmbMacros).Enabled = false;
	}

	private void btnStopRecord_Click(object sender, EventArgs e)
	{
		((Control)cmbMacros).Enabled = true;
		((Control)btnStopRecord).Enabled = false;
		if (((Dictionary<int, MacroAction>)(object)mMacro.DictMacroActions).Count > 0)
		{
			((Control)btnAdd).Enabled = true;
		}
		sIsRecording = false;
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		string text = Interaction.InputBox("Please enter a name for macro ", $"{Strings.ProductDisplayName} Macro");
		if (((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros).ContainsKey(text) || string.IsNullOrEmpty(text))
		{
			MessageBox.Show("Macro name already exists");
			return;
		}
		((Control)btnAdd).Enabled = false;
		mMacro.RepeatBehaiour = (RepeatBehaviour)Enum.Parse(typeof(RepeatBehaviour), ((ListControl)cmbRepeatBehaviour).SelectedValue.ToString(), ignoreCase: true);
		((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros).Add(text, mMacro);
		UpdateUIForNewFile();
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		mMacro = new Macro();
		UpdateGrid();
		MacroData.Instance.SaveMacroData();
	}

	private void cmbMacros_SelectedValueChanged(object sender, EventArgs e)
	{
		UpdateGrid();
	}

	private void UpdateGrid()
	{
		if (((ListControl)cmbMacros).SelectedValue.ToString() == editComboText)
		{
			dataGridView1.DataSource = ((Dictionary<int, MacroAction>)(object)mMacro.DictMacroActions).Values.ToList();
		}
		else
		{
			dataGridView1.DataSource = ((Dictionary<int, MacroAction>)(object)((Dictionary<string, Macro>)(object)MacroData.Instance.DictMacros)[((ListControl)cmbMacros).SelectedValue.ToString()].DictMacroActions).Values.ToList();
		}
	}

	internal static void RecordMouse(double x, double y, double width, double height, MouseButtons button, ActionType type)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (sIsRecording)
		{
			double totalMilliseconds = (DateTime.Now - mBaseDateTime).TotalMilliseconds;
			mBaseDateTime = DateTime.Now;
			AddActionInDictionary(new MacroAction
			{
				ActionType = type,
				MouseButton = button,
				ActionPointX = x / width,
				ActionPointY = y / height,
				DelayFromLastAction = totalMilliseconds
			});
		}
	}

	internal static void RecordKeys(Keys keyCode, ActionType type)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (sIsRecording)
		{
			double totalMilliseconds = (DateTime.Now - mBaseDateTime).TotalMilliseconds;
			mBaseDateTime = DateTime.Now;
			AddActionInDictionary(new MacroAction
			{
				ActionType = type,
				ActionKey = keyCode,
				DelayFromLastAction = totalMilliseconds
			});
		}
	}

	private static void AddActionInDictionary(MacroAction ma)
	{
		((Dictionary<int, MacroAction>)(object)mMacro.DictMacroActions).Add(((Dictionary<int, MacroAction>)(object)mMacro.DictMacroActions).Count, ma);
		Instance.UpdateGrid();
	}

	private void MacroForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		((CancelEventArgs)(object)e).Cancel = true;
		((Control)this).Hide();
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected Obj, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected Obj, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected Obj, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected Obj, but got Unknown
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Expected Obj, but got Unknown
		dataGridView1 = new DataGridView();
		splitContainer1 = new SplitContainer();
		btnReload = new Button();
		btnAdd = new Button();
		cmbRepeatBehaviour = new ComboBox();
		btnSave = new Button();
		btnStopRecord = new Button();
		bntStartRecord = new Button();
		cmbMacros = new ComboBox();
		btnStop = new Button();
		btnPlay = new Button();
		((ISupportInitialize)dataGridView1).BeginInit();
		((Control)splitContainer1.Panel1).SuspendLayout();
		((Control)splitContainer1.Panel2).SuspendLayout();
		((Control)splitContainer1).SuspendLayout();
		((Control)this).SuspendLayout();
		dataGridView1.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
		((Control)dataGridView1).Dock = (DockStyle)5;
		((Control)dataGridView1).Location = new Point(0, 0);
		((Control)dataGridView1).Name = "dataGridView1";
		((Control)dataGridView1).Size = new Size(1152, 722);
		((Control)dataGridView1).TabIndex = 0;
		splitContainer1.Dock = (DockStyle)5;
		((Control)splitContainer1).Location = new Point(0, 0);
		((Control)splitContainer1).Name = "splitContainer1";
		splitContainer1.Orientation = (Orientation)0;
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnReload);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnAdd);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cmbRepeatBehaviour);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnSave);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnStopRecord);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)bntStartRecord);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)cmbMacros);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnStop);
		((Control)splitContainer1.Panel1).Controls.Add((Control)(object)btnPlay);
		((Control)splitContainer1.Panel2).Controls.Add((Control)(object)dataGridView1);
		((Control)splitContainer1).Size = new Size(1152, 790);
		splitContainer1.SplitterDistance = 64;
		((Control)splitContainer1).TabIndex = 1;
		((Control)btnReload).Location = new Point(1026, 17);
		((Control)btnReload).Name = "btnReload";
		((Control)btnReload).Size = new Size(92, 30);
		((Control)btnReload).TabIndex = 1;
		((Control)btnReload).Text = "Reload";
		((ButtonBase)btnReload).UseVisualStyleBackColor = true;
		((Control)btnReload).Click += btnReload_Click;
		((Control)btnAdd).Enabled = false;
		((Control)btnAdd).Location = new Point(734, 18);
		((Control)btnAdd).Name = "btnAdd";
		((Control)btnAdd).Size = new Size(92, 30);
		((Control)btnAdd).TabIndex = 7;
		((Control)btnAdd).Text = "Add";
		((ButtonBase)btnAdd).UseVisualStyleBackColor = true;
		((Control)btnAdd).Click += btnAdd_Click;
		((ListControl)cmbRepeatBehaviour).FormattingEnabled = true;
		((Control)cmbRepeatBehaviour).Location = new Point(607, 24);
		((Control)cmbRepeatBehaviour).Name = "cmbRepeatBehaviour";
		((Control)cmbRepeatBehaviour).Size = new Size(121, 21);
		((Control)cmbRepeatBehaviour).TabIndex = 6;
		((Control)btnSave).Location = new Point(928, 17);
		((Control)btnSave).Name = "btnSave";
		((Control)btnSave).Size = new Size(92, 30);
		((Control)btnSave).TabIndex = 5;
		((Control)btnSave).Text = "Save";
		((ButtonBase)btnSave).UseVisualStyleBackColor = true;
		((Control)btnSave).Click += btnSave_Click;
		((Control)btnStopRecord).Enabled = false;
		((Control)btnStopRecord).Location = new Point(509, 18);
		((Control)btnStopRecord).Name = "btnStopRecord";
		((Control)btnStopRecord).Size = new Size(92, 30);
		((Control)btnStopRecord).TabIndex = 4;
		((Control)btnStopRecord).Text = "Stop Record";
		((ButtonBase)btnStopRecord).UseVisualStyleBackColor = true;
		((Control)btnStopRecord).Click += btnStopRecord_Click;
		((Control)bntStartRecord).Location = new Point(411, 18);
		((Control)bntStartRecord).Name = "bntStartRecord";
		((Control)bntStartRecord).Size = new Size(92, 30);
		((Control)bntStartRecord).TabIndex = 3;
		((Control)bntStartRecord).Text = "StartRecord";
		((ButtonBase)bntStartRecord).UseVisualStyleBackColor = true;
		((Control)bntStartRecord).Click += bntStartRecord_Click;
		((ListControl)cmbMacros).FormattingEnabled = true;
		((Control)cmbMacros).Location = new Point(32, 23);
		((Control)cmbMacros).Name = "cmbMacros";
		((Control)cmbMacros).Size = new Size(121, 21);
		((Control)cmbMacros).TabIndex = 2;
		((ListControl)cmbMacros).SelectedValueChanged += cmbMacros_SelectedValueChanged;
		((Control)btnStop).Enabled = false;
		((Control)btnStop).Location = new Point(268, 17);
		((Control)btnStop).Name = "btnStop";
		((Control)btnStop).Size = new Size(92, 30);
		((Control)btnStop).TabIndex = 1;
		((Control)btnStop).Text = "Stop";
		((ButtonBase)btnStop).UseVisualStyleBackColor = true;
		((Control)btnStop).Click += btnStop_Click;
		((Control)btnPlay).Location = new Point(159, 17);
		((Control)btnPlay).Name = "btnPlay";
		((Control)btnPlay).Size = new Size(92, 30);
		((Control)btnPlay).TabIndex = 0;
		((Control)btnPlay).Text = "Play";
		((ButtonBase)btnPlay).UseVisualStyleBackColor = true;
		((Control)btnPlay).Click += btnPlay_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(1152, 790);
		((Control)this).Controls.Add((Control)(object)splitContainer1);
		((Control)this).Name = "MacroForm";
		((Control)this).Text = "MacroForm";
		((Form)this).FormClosing += MacroForm_FormClosing;
		((ISupportInitialize)dataGridView1).EndInit();
		((Control)splitContainer1.Panel1).ResumeLayout(false);
		((Control)splitContainer1.Panel2).ResumeLayout(false);
		((Control)splitContainer1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
