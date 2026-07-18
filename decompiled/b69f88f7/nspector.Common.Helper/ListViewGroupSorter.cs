using System.Collections.Generic;
using System.Windows.Forms;

namespace nspector.Common.Helper;

public class ListViewGroupSorter
{
	internal ListView _listview;

	public static bool operator ==(ListView listview, ListViewGroupSorter sorter)
	{
		return listview == sorter._listview;
	}

	public static bool operator !=(ListView listview, ListViewGroupSorter sorter)
	{
		return listview != sorter._listview;
	}

	public static implicit operator ListView(ListViewGroupSorter sorter)
	{
		return sorter._listview;
	}

	public static implicit operator ListViewGroupSorter(ListView listview)
	{
		return new ListViewGroupSorter(listview);
	}

	internal ListViewGroupSorter(ListView listview)
	{
		_listview = listview;
	}

	public void SortGroups(bool ascending)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		_listview.BeginUpdate();
		List<ListViewGroup> list = new List<ListViewGroup>();
		foreach (ListViewGroup group in _listview.Groups)
		{
			ListViewGroup item = group;
			list.Add(item);
		}
		_listview.Groups.Clear();
		list.Sort(new ListViewGroupHeaderSorter(ascending));
		_listview.Groups.AddRange(list.ToArray());
		_listview.EndUpdate();
	}

	public override bool Equals(object obj)
	{
		return ((object)_listview).Equals(obj);
	}

	public override int GetHashCode()
	{
		return ((object)_listview).GetHashCode();
	}

	public override string ToString()
	{
		return ((object)_listview).ToString();
	}
}
