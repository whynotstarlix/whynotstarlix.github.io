using System.Collections.Generic;
using System.Windows.Forms;

namespace nspector.Common.Helper;

public class ListViewGroupHeaderSorter : IComparer<ListViewGroup>
{
	private bool _ascending = true;

	public ListViewGroupHeaderSorter(bool ascending)
	{
		_ascending = ascending;
	}

	public int Compare(ListViewGroup x, ListViewGroup y)
	{
		if (_ascending)
		{
			return string.Compare(x.Header, y.Header);
		}
		return string.Compare(y.Header, x.Header);
	}
}
