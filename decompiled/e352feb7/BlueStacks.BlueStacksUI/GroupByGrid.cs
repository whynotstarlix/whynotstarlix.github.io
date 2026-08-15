using System.Collections.Generic;
using System.Windows.Forms;

namespace BlueStacks.BlueStacksUI;

public class GroupByGrid : DataGridView
{
	internal List<int> ColumnsToBeGrouped = new List<int>();

	protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
	{
		((DataGridView)this).OnCellFormatting(args);
		if (args != null && args.RowIndex != 0 && IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
		{
			((ConvertEventArgs)args).Value = string.Empty;
			args.FormattingApplied = true;
		}
	}

	private bool IsRepeatedCellValue(int rowIndex, int colIndex)
	{
		DataGridViewCell val = ((DataGridView)this).Rows[rowIndex].Cells[colIndex];
		DataGridViewCell val2 = ((DataGridView)this).Rows[rowIndex - 1].Cells[colIndex];
		if (!ColumnsToBeGrouped.Contains(colIndex))
		{
			return false;
		}
		if (val.Value == val2.Value || (val.Value != null && val2.Value != null && val.Value.ToString() == val2.Value.ToString()))
		{
			return true;
		}
		return false;
	}

	protected override void OnCellPainting(DataGridViewCellPaintingEventArgs args)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((DataGridView)this).OnCellPainting(args);
		if (args == null)
		{
			return;
		}
		args.AdvancedBorderStyle.Bottom = (DataGridViewAdvancedCellBorderStyle)1;
		if (args.RowIndex >= 1 && args.ColumnIndex >= 0)
		{
			if (IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
			{
				args.AdvancedBorderStyle.Top = (DataGridViewAdvancedCellBorderStyle)1;
			}
			else
			{
				args.AdvancedBorderStyle.Top = ((DataGridView)this).AdvancedCellBorderStyle.Top;
			}
		}
	}
}
