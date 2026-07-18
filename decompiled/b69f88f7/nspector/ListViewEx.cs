using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace nspector;

internal class ListViewEx : ListView
{
	private struct EmbeddedControl
	{
		internal Control Control;

		internal int Column;

		internal int Row;

		internal DockStyle Dock;

		internal ListViewItem Item;
	}

	private delegate void CallBackSetGroupState(ListViewGroup lstvwgrp, ListViewGroupState state);

	private delegate void CallbackSetGroupString(ListViewGroup lstvwgrp, string value);

	private const int LVM_FIRST = 4096;

	private const int LVM_GETCOLUMNORDERARRAY = 4155;

	private const int WM_PAINT = 15;

	private const int WM_VSCROLL = 277;

	private const int WM_HSCROLL = 276;

	private const int WM_MOUSEWHEEL = 522;

	private ArrayList _embeddedControls = new ArrayList();

	private const int WM_DROPFILES = 563;

	private bool _isUpdatingGroups = false;

	private const int WM_NOTIFY = 78;

	private const int WM_REFLECT_NOTIFY = 8270;

	private const int LVN_FIRST = -100;

	private const int LVN_GROUPINFO = -188;

	private const int LVM_SETGROUPINFO = 4243;

	private const int WM_LBUTTONUP = 514;

	[DefaultValue(/*Could not decode attribute arguments.*/)]
	internal View View
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return ((ListView)this).View;
		}
		set
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			foreach (EmbeddedControl embeddedControl in _embeddedControls)
			{
				embeddedControl.Control.Visible = (int)value == 1;
			}
			((ListView)this).View = value;
		}
	}

	public event DropFilesNativeHandler OnDropFilesNative;

	public event EventHandler<GroupStateChangedEventArgs> GroupStateChanged;

	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

	public ListViewEx()
	{
		((Control)this).SetStyle((ControlStyles)139264, true);
		((Control)this).SetStyle((ControlStyles)32768, true);
	}

	protected override void OnNotifyMessage(Message m)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((Message)(ref m)).Msg != 20)
		{
			((Control)this).OnNotifyMessage(m);
		}
	}

	protected int[] GetColumnOrder()
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * ((ListView)this).Columns.Count);
		if (SendMessage(((Control)this).Handle, 4155, new IntPtr(((ListView)this).Columns.Count), intPtr).ToInt32() == 0)
		{
			Marshal.FreeHGlobal(intPtr);
			return null;
		}
		int[] array = new int[((ListView)this).Columns.Count];
		Marshal.Copy(intPtr, array, 0, ((ListView)this).Columns.Count);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	protected Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
	{
		Rectangle empty = Rectangle.Empty;
		if (Item == null)
		{
			throw new ArgumentNullException("Item");
		}
		int[] columnOrder = GetColumnOrder();
		if (columnOrder == null)
		{
			return empty;
		}
		if (SubItem >= columnOrder.Length)
		{
			throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");
		}
		Rectangle bounds;
		try
		{
			bounds = Item.GetBounds((ItemBoundsPortion)0);
		}
		catch
		{
			return empty;
		}
		int num = bounds.Left;
		int i;
		for (i = 0; i < columnOrder.Length; i++)
		{
			ColumnHeader val = ((ListView)this).Columns[columnOrder[i]];
			if (val.Index == SubItem)
			{
				break;
			}
			num += val.Width;
		}
		return new Rectangle(num, bounds.Top - 1, ((ListView)this).Columns[columnOrder[i]].Width, bounds.Height);
	}

	internal void AddEmbeddedControl(Control c, int col, int row)
	{
		AddEmbeddedControl(c, col, row, (DockStyle)5);
	}

	internal void AddEmbeddedControl(Control c, int col, int row, DockStyle dock)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (c == null)
		{
			throw new ArgumentNullException();
		}
		if (col >= ((ListView)this).Columns.Count || row >= ((ListView)this).Items.Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		EmbeddedControl embeddedControl = default(EmbeddedControl);
		embeddedControl.Control = c;
		embeddedControl.Column = col;
		embeddedControl.Row = row;
		embeddedControl.Dock = dock;
		embeddedControl.Item = ((ListView)this).Items[row];
		_embeddedControls.Add(embeddedControl);
		c.Click += _embeddedControl_Click;
		((Control)this).Controls.Add(c);
	}

	internal void RemoveEmbeddedControl(Control c)
	{
		if (c == null)
		{
			throw new ArgumentNullException();
		}
		for (int i = 0; i < _embeddedControls.Count; i++)
		{
			if (((EmbeddedControl)_embeddedControls[i]).Control == c)
			{
				c.Click -= _embeddedControl_Click;
				((Control)this).Controls.Remove(c);
				_embeddedControls.RemoveAt(i);
				break;
			}
		}
	}

	internal Control GetEmbeddedControl(int col, int row)
	{
		foreach (EmbeddedControl embeddedControl in _embeddedControls)
		{
			if (embeddedControl.Row == row && embeddedControl.Column == col)
			{
				return embeddedControl.Control;
			}
		}
		return null;
	}

	[DllImport("shell32.dll", CharSet = CharSet.Auto)]
	public static extern int DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder lpszFile, int cch);

	protected override void WndProc(ref Message m)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected I4, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		if (((Message)(ref m)).Msg == 514)
		{
			((Control)this).DefWndProc(ref m);
			return;
		}
		switch (((Message)(ref m)).Msg)
		{
		case 15:
			if ((int)View != 1)
			{
				break;
			}
			foreach (EmbeddedControl embeddedControl in _embeddedControls)
			{
				Control control = embeddedControl.Control;
				ComboBox val3 = (ComboBox)(object)((control is ComboBox) ? control : null);
				if (val3 != null && val3.DroppedDown)
				{
					continue;
				}
				Rectangle subItemBounds = GetSubItemBounds(embeddedControl.Item, embeddedControl.Column);
				if ((int)((ListView)this).HeaderStyle != 0 && subItemBounds.Top < ((Control)this).Font.Height)
				{
					embeddedControl.Control.Visible = false;
					continue;
				}
				embeddedControl.Control.Visible = true;
				DockStyle dock = embeddedControl.Dock;
				DockStyle val4 = dock;
				switch ((int)val4)
				{
				case 1:
					subItemBounds.Height = embeddedControl.Control.Height;
					break;
				case 3:
					subItemBounds.Width = embeddedControl.Control.Width;
					break;
				case 2:
					subItemBounds.Offset(0, subItemBounds.Height - embeddedControl.Control.Height);
					subItemBounds.Height = embeddedControl.Control.Height;
					break;
				case 4:
					subItemBounds.Offset(subItemBounds.Width - embeddedControl.Control.Width, 0);
					subItemBounds.Width = embeddedControl.Control.Width;
					break;
				case 0:
					subItemBounds.Size = embeddedControl.Control.Size;
					break;
				}
				int x = subItemBounds.X;
				Padding margin = embeddedControl.Control.Margin;
				subItemBounds.X = x + ((Padding)(ref margin)).Left;
				int y = subItemBounds.Y;
				margin = embeddedControl.Control.Margin;
				subItemBounds.Y = y + ((Padding)(ref margin)).Top;
				int width = subItemBounds.Width;
				margin = embeddedControl.Control.Margin;
				subItemBounds.Width = width - ((Padding)(ref margin)).Right;
				int height = subItemBounds.Height;
				margin = embeddedControl.Control.Margin;
				subItemBounds.Height = height - ((Padding)(ref margin)).Bottom;
				embeddedControl.Control.Bounds = subItemBounds;
			}
			break;
		case 276:
		case 277:
		case 522:
			foreach (EmbeddedControl embeddedControl2 in _embeddedControls)
			{
				Control control2 = embeddedControl2.Control;
				ComboBox val5 = (ComboBox)(object)((control2 is ComboBox) ? control2 : null);
				if (val5 != null && val5.DroppedDown)
				{
					val5.DroppedDown = false;
				}
			}
			break;
		case 563:
			if (this.OnDropFilesNative != null)
			{
				int num = DragQueryFile(((Message)(ref m)).WParam, uint.MaxValue, null, 0);
				if (num > 0)
				{
					List<string> list = new List<string>();
					for (uint num2 = 0u; num2 < num; num2++)
					{
						int num3 = DragQueryFile(((Message)(ref m)).WParam, num2, null, 0);
						if (num3 > 0)
						{
							StringBuilder stringBuilder = new StringBuilder(num3 + 1);
							int num4 = DragQueryFile(((Message)(ref m)).WParam, num2, stringBuilder, num3 + 1);
							list.Add(stringBuilder.ToString());
						}
					}
					this.OnDropFilesNative(list.ToArray());
				}
			}
			((ListView)this).WndProc(ref m);
			break;
		case 78:
		case 8270:
		{
			if (((NMHDR)Marshal.PtrToStructure(((Message)(ref m)).LParam, typeof(NMHDR))).code != -188 || _isUpdatingGroups)
			{
				break;
			}
			NMLVGROUP nMLVGROUP = (NMLVGROUP)Marshal.PtrToStructure(((Message)(ref m)).LParam, typeof(NMLVGROUP));
			ListViewGroup val = null;
			foreach (ListViewGroup group in ((ListView)this).Groups)
			{
				ListViewGroup val2 = group;
				int? groupID = GetGroupID(val2);
				if (groupID.HasValue && groupID.Value == nMLVGROUP.iGroupId)
				{
					val = val2;
					break;
				}
			}
			if (val != null)
			{
				bool isCollapsed = (nMLVGROUP.uNewState & 1) != 0;
				this.GroupStateChanged?.Invoke(this, new GroupStateChangedEventArgs
				{
					Group = val,
					IsCollapsed = isCollapsed,
					NewState = (ListViewGroupState)nMLVGROUP.uNewState
				});
			}
			break;
		}
		}
		((ListView)this).WndProc(ref m);
	}

	private void _embeddedControl_Click(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between O and Unknown
		foreach (EmbeddedControl embeddedControl in _embeddedControls)
		{
			if ((object)embeddedControl.Control == (object)(Control)sender)
			{
				((ListView)this).SelectedItems.Clear();
				embeddedControl.Item.Selected = true;
			}
		}
	}

	[DllImport("user32.dll")]
	private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, LVGROUP lParam);

	private static int? GetGroupID(ListViewGroup lstvwgrp)
	{
		int? result = null;
		Type type = ((object)lstvwgrp).GetType();
		if (type != null)
		{
			PropertyInfo property = type.GetProperty("ID", BindingFlags.Instance | BindingFlags.NonPublic);
			if (property != null)
			{
				object value = property.GetValue(lstvwgrp, null);
				if (value != null)
				{
					return value as int?;
				}
			}
		}
		return result;
	}

	private static void setGrpState(ListViewGroup lstvwgrp, ListViewGroupState state)
	{
		if (Environment.OSVersion.Version.Major < 6 || lstvwgrp == null || lstvwgrp.ListView == null)
		{
			return;
		}
		if (((Control)lstvwgrp.ListView).InvokeRequired)
		{
			((Control)lstvwgrp.ListView).Invoke((Delegate)new CallBackSetGroupState(setGrpState), new object[2] { lstvwgrp, state });
			return;
		}
		int? groupID = GetGroupID(lstvwgrp);
		int num = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
		LVGROUP lVGROUP = default(LVGROUP);
		lVGROUP.CbSize = Marshal.SizeOf(lVGROUP);
		lVGROUP.State = state;
		lVGROUP.Mask = ListViewGroupMask.State;
		if (groupID.HasValue)
		{
			lVGROUP.IGroupId = groupID.Value;
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, groupID.Value, lVGROUP);
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, groupID.Value, lVGROUP);
		}
		else
		{
			lVGROUP.IGroupId = num;
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, num, lVGROUP);
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, num, lVGROUP);
		}
		((Control)lstvwgrp.ListView).Refresh();
	}

	private static void setGrpFooter(ListViewGroup lstvwgrp, string footer)
	{
		if (Environment.OSVersion.Version.Major < 6 || lstvwgrp == null || lstvwgrp.ListView == null)
		{
			return;
		}
		if (((Control)lstvwgrp.ListView).InvokeRequired)
		{
			((Control)lstvwgrp.ListView).Invoke((Delegate)new CallbackSetGroupString(setGrpFooter), new object[2] { lstvwgrp, footer });
			return;
		}
		int? groupID = GetGroupID(lstvwgrp);
		int num = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
		LVGROUP lVGROUP = default(LVGROUP);
		lVGROUP.CbSize = Marshal.SizeOf(lVGROUP);
		lVGROUP.PszFooter = footer;
		lVGROUP.Mask = ListViewGroupMask.Footer;
		if (groupID.HasValue)
		{
			lVGROUP.IGroupId = groupID.Value;
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, groupID.Value, lVGROUP);
		}
		else
		{
			lVGROUP.IGroupId = num;
			SendMessage(((Control)lstvwgrp.ListView).Handle, 4243, num, lVGROUP);
		}
	}

	public void SetGroupState(ListViewGroupState state)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		_isUpdatingGroups = true;
		foreach (ListViewGroup group in ((ListView)this).Groups)
		{
			ListViewGroup lstvwgrp = group;
			setGrpState(lstvwgrp, state);
		}
		_isUpdatingGroups = false;
	}

	public void SetGroupState(ListViewGroup group, ListViewGroupState state)
	{
		_isUpdatingGroups = true;
		setGrpState(group, state);
		_isUpdatingGroups = false;
	}

	public void SetGroupFooter(ListViewGroup lvg, string footerText)
	{
		setGrpFooter(lvg, footerText);
	}
}
