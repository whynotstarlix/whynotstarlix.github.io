using System;
using System.Windows.Forms;

namespace nspector;

public class GroupStateChangedEventArgs : EventArgs
{
	public ListViewGroup Group { get; set; }

	public bool IsCollapsed { get; set; }

	public ListViewGroupState NewState { get; set; }
}
