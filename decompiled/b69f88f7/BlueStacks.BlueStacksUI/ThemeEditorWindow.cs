using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlueStacks.Common;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI;

public class ThemeEditorWindow : CustomWindow, IComponentConnector
{
	internal const string THUMBNAIL_ICON = "ThemeThumbnail.png";

	private string selectedItem = string.Empty;

	private bool isCreateDraftDirectory = true;

	private bool ignore = true;

	private static ThemeEditorWindow mInstance;

	private const string DraftFolderName = "Drafts";

	private string DraftDirectory = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Drafts");

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderR;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderG;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderA;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelA;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelR;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelG;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelB;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderX;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider sliderY;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelX;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label labelY;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label AppIcon;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider tabangleX;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label AngleX;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider tabangleY;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label AngleY;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider topleftCornerRadius;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label top;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider toprightcornerradius;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label left;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider bottomleftCornerRadius;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label right;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Slider bottomrightcornerradius;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label bottom;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupBox groupBox1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RadioButton SearchTextBoxCurvature;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RadioButton TabTransFormPortrait;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal RadioButton TabTransFormLandscape;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBox textBox;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid gridColor;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button btnLoad;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Button btnSave;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupByGrid dataGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal GroupByGrid dataGrid1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ListBox ListView2;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Image pictureBox;

	private bool _contentLoaded;

	public static ThemeEditorWindow Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new ThemeEditorWindow();
			}
			return mInstance;
		}
		set
		{
			mInstance = value;
		}
	}

	public ThemeEditorWindow()
	{
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Expected O, but got Unknown
		InitializeComponent();
		((Window)this).Closing += ThemeEditorWindow_Closing;
		((Window)this).Activated += ThemeEditorWindow_Activated;
		((RangeBase)sliderX).Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX;
		((RangeBase)sliderY).Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY;
		((ToggleButton)TabTransFormLandscape).IsChecked = true;
		((ItemsControl)ListView2).ItemsSource = BlueStacksUIBinding.Instance.ImageModel.Keys.ToList();
		using (DataTable dataTable = new DataTable())
		{
			dataTable.Columns.Add(new DataColumn("Category", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Brush", typeof(Brush)));
			foreach (KeyValuePair<string, Brush> item in (Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Name"] = item.Key;
				dataRow["Brush"] = item.Value;
				if (((Dictionary<string, string>)(object)BlueStacksUIColorManager.AppliedTheme.DictCategory).ContainsKey(item.Key))
				{
					dataRow["Category"] = ((Dictionary<string, string>)(object)BlueStacksUIColorManager.AppliedTheme.DictCategory)[item.Key];
				}
				dataTable.Rows.Add(dataRow);
			}
			DataView defaultView = dataTable.DefaultView;
			defaultView.Sort = "Category asc";
			DataTable dataSource = defaultView.ToTable();
			dataGrid.ColumnsToBeGrouped.Add(0);
			((DataGridView)dataGrid).DataSource = dataSource;
			((DataGridView)dataGrid).CellClick += new DataGridViewCellEventHandler(DataGrid_CellClick);
			((DataGridView)dataGrid).CellValueChanged += new DataGridViewCellEventHandler(DataGrid_CellValueChanged);
			((DataGridView)dataGrid).AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)6;
			((DataGridViewBand)((DataGridView)dataGrid).Columns["Brush"]).Visible = false;
		}
		using (DataTable dataTable2 = new DataTable())
		{
			dataTable2.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("CornerRadius", typeof(CornerRadius)));
			foreach (KeyValuePair<string, CornerRadius> item2 in (Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["Name"] = item2.Key;
				dataRow2["CornerRadius"] = item2.Value;
				dataTable2.Rows.Add(dataRow2);
			}
			((DataGridView)dataGrid1).DataSource = dataTable2;
			((DataGridView)dataGrid1).CellClick += new DataGridViewCellEventHandler(DataGrid1_CellClick);
			((DataGridView)dataGrid1).AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)6;
		}
		ignore = false;
	}

	private void DataGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		ignore = true;
		try
		{
			Slider obj = topleftCornerRadius;
			CornerRadius val = ((Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)[((DataGridView)dataGrid1).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			((RangeBase)obj).Value = ((CornerRadius)(ref val)).TopLeft;
			Slider obj2 = toprightcornerradius;
			val = ((Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)[((DataGridView)dataGrid1).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			((RangeBase)obj2).Value = ((CornerRadius)(ref val)).TopRight;
			Slider obj3 = bottomleftCornerRadius;
			val = ((Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)[((DataGridView)dataGrid1).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			((RangeBase)obj3).Value = ((CornerRadius)(ref val)).BottomLeft;
			Slider obj4 = bottomrightcornerradius;
			val = ((Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)[((DataGridView)dataGrid1).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			((RangeBase)obj4).Value = ((CornerRadius)(ref val)).BottomRight;
		}
		catch (Exception ex)
		{
			Console.WriteLine("exception:" + ex.ToString());
		}
		ignore = false;
	}

	private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
	{
		if (((DataGridView)dataGrid).Columns[e.ColumnIndex].Name == "Category")
		{
			((Dictionary<string, string>)(object)BlueStacksUIColorManager.AppliedTheme.DictCategory)[((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Name"].Value.ToString()] = ((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Category"].Value.ToString();
			((DataGridView)dataGrid).Sort(((DataGridView)dataGrid).Columns[e.ColumnIndex], ListSortDirection.Ascending);
		}
	}

	private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		ignore = true;
		try
		{
			Slider obj = sliderA;
			Brush obj2 = ((Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)[((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			Color color = ((SolidColorBrush)((obj2 is SolidColorBrush) ? obj2 : null)).Color;
			((RangeBase)obj).Value = (int)((Color)(ref color)).A;
			Slider obj3 = sliderR;
			Brush obj4 = ((Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)[((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			color = ((SolidColorBrush)((obj4 is SolidColorBrush) ? obj4 : null)).Color;
			((RangeBase)obj3).Value = (int)((Color)(ref color)).R;
			Slider obj5 = sliderG;
			Brush obj6 = ((Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)[((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			color = ((SolidColorBrush)((obj6 is SolidColorBrush) ? obj6 : null)).Color;
			((RangeBase)obj5).Value = (int)((Color)(ref color)).G;
			Slider obj7 = sliderB;
			Brush obj8 = ((Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)[((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Name"].Value.ToString()];
			color = ((SolidColorBrush)((obj8 is SolidColorBrush) ? obj8 : null)).Color;
			((RangeBase)obj7).Value = (int)((Color)(ref color)).B;
			textBox.Text = ((DataGridView)dataGrid).Rows[e.RowIndex].Cells["Brush"].Value.ToString();
		}
		catch (Exception ex)
		{
			Console.WriteLine("exception:" + ex.ToString());
		}
		ignore = false;
	}

	private void ThemeEditorWindow_Activated(object sender, EventArgs e)
	{
		if (isCreateDraftDirectory)
		{
			((ItemsControl)ListView2).ItemsSource = BlueStacksUIBinding.Instance.ImageModel.Keys.ToList();
			isCreateDraftDirectory = false;
			CopyEverything(CustomPictureBox.AssetsDir, DraftDirectory);
			File.Delete(Path.Combine(DraftDirectory, "ThemeThumbnail.png"));
		}
	}

	private void ThemeEditorWindow_Closing(object sender, CancelEventArgs e)
	{
		isCreateDraftDirectory = true;
		e.Cancel = true;
		((Window)this).Hide();
	}

	private void Color_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		try
		{
			byte num = (byte)((RangeBase)sliderA).Value;
			byte b = (byte)((RangeBase)sliderR).Value;
			byte b2 = (byte)((RangeBase)sliderG).Value;
			byte b3 = (byte)((RangeBase)sliderB).Value;
			Color val = Color.FromArgb(num, b, b2, b3);
			Brush background = (Brush)new SolidColorBrush(val);
			((Panel)gridColor).Background = background;
			if (!ignore)
			{
				textBox.Text = ((Color)(ref val)).ToString((IFormatProvider)CultureInfo.InvariantCulture);
				((Dictionary<string, Brush>)(object)BlueStacksUIColorManager.AppliedTheme.DictBrush)[((DataGridView)dataGrid).CurrentRow.Cells["Name"].Value.ToString()] = (Brush)new SolidColorBrush(new ColorUtils(val).WPFColor);
				if (((DataGridView)dataGrid).CurrentRow.Cells["Category"].Value.ToString().Equals("*MainColors*", StringComparison.OrdinalIgnoreCase))
				{
					BlueStacksUIColorManager.AppliedTheme.CalculateAndNotify(true);
				}
				else
				{
					BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void textBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!ignore)
			{
				SolidColorBrush val = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textBox.Text));
				Slider obj = sliderA;
				Color color = val.Color;
				((RangeBase)obj).Value = (int)((Color)(ref color)).A;
				Slider obj2 = sliderR;
				color = val.Color;
				((RangeBase)obj2).Value = (int)((Color)(ref color)).R;
				Slider obj3 = sliderG;
				color = val.Color;
				((RangeBase)obj3).Value = (int)((Color)(ref color)).G;
				Slider obj4 = sliderB;
				color = val.Color;
				((RangeBase)obj4).Value = (int)((Color)(ref color)).B;
			}
		}
		catch
		{
		}
	}

	private void Curve_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (!ignore)
		{
			BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX = ((RangeBase)sliderX).Value;
			BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY = ((RangeBase)sliderY).Value;
			BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
		}
	}

	private void Load_Click(object sender, RoutedEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		FolderBrowserDialog val = new FolderBrowserDialog
		{
			SelectedPath = RegistryManager.Instance.ClientInstallDir
		};
		try
		{
			if ((int)((CommonDialog)val).ShowDialog() != 1 || string.IsNullOrEmpty(val.SelectedPath))
			{
				return;
			}
			_ = val.SelectedPath;
			if (File.Exists(Path.Combine(val.SelectedPath, "ThemeFile")))
			{
				string fileName = Path.GetFileName(val.SelectedPath);
				if (!val.SelectedPath.Contains(RegistryManager.Instance.ClientInstallDir))
				{
					string text = Path.Combine(RegistryManager.Instance.ClientInstallDir, fileName);
					if (Directory.Exists(text))
					{
						MessageBox.Show("Theme with this name already exists. Please rename the folder an try again");
					}
					else
					{
						Directory.CreateDirectory(text);
						CopyEverything(val.SelectedPath, text);
					}
				}
				BlueStacksUIColorManager.ReloadAppliedTheme(fileName);
			}
			else
			{
				MessageBox.Show("Please select theme folder");
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		string text = Interaction.InputBox("Theme name", "BlueStacks Theme Editor Tool", string.Format(CultureInfo.CurrentCulture, "{0:F}", new object[1] { DateTime.Now }), 0, 0);
		if (Directory.Exists(Path.Combine(RegistryManager.Instance.ClientInstallDir, text)))
		{
			MessageBox.Show("Already Exists. Please retry");
			return;
		}
		Directory.CreateDirectory(Path.Combine(RegistryManager.Instance.ClientInstallDir, text));
		CopyEverything(DraftDirectory, Path.Combine(RegistryManager.Instance.ClientInstallDir, text));
		RegistryManager.Instance.SetClientThemeNameInRegistry(text);
		Window w = (Window)(object)BlueStacksUIUtils.DictWindows.Values.ToList()[0];
		((DispatcherObject)w).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			RenderTargetBitmap val = new RenderTargetBitmap((int)((FrameworkElement)w).ActualWidth, (int)((FrameworkElement)w).ActualHeight, 0.0, 0.0, PixelFormats.Pbgra32);
			val.Render((Visual)(object)BlueStacksUIUtils.DictWindows.Values.ToList()[0]);
			PngBitmapEncoder val2 = new PngBitmapEncoder();
			((BitmapEncoder)val2).Frames.Add(BitmapFrame.Create((BitmapSource)(object)val));
			using Stream stream = File.Create(Path.Combine(CustomPictureBox.AssetsDir, "ThemeThumbnail.png"));
			((BitmapEncoder)val2).Save(stream);
		}, new object[0]);
		BlueStacksUIColorManager.AppliedTheme.Save(BlueStacksUIColorManager.GetThemeFilePath(RegistryManager.ClientThemeName));
		CustomPictureBox.UpdateImagesFromNewDirectory("");
	}

	private static void CopyEverything(string SourcePath, string DestinationPath)
	{
		string[] directories = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
		for (int i = 0; i < directories.Length; i++)
		{
			Directory.CreateDirectory(directories[i].Replace(SourcePath, DestinationPath));
		}
		directories = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
		foreach (string obj in directories)
		{
			File.Copy(obj, obj.Replace(SourcePath, DestinationPath), overwrite: true);
		}
	}

	private void tabangle_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		if (!ignore)
		{
			if (((ToggleButton)SearchTextBoxCurvature).IsChecked.Value)
			{
				BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm = new SkewTransform(((RangeBase)tabangleX).Value, ((RangeBase)tabangleY).Value);
				BlueStacksUIColorManager.AppliedTheme.TextBoxAntiTransForm = new SkewTransform(((RangeBase)tabangleX).Value * -1.0, ((RangeBase)tabangleY).Value * -1.0);
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
			else if (((ToggleButton)TabTransFormLandscape).IsChecked.Value)
			{
				BlueStacksUIColorManager.AppliedTheme.TabTransform = new SkewTransform(((RangeBase)tabangleX).Value, ((RangeBase)tabangleY).Value);
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
			else
			{
				BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait = new SkewTransform(((RangeBase)tabangleX).Value, ((RangeBase)tabangleY).Value);
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
		}
	}

	private void TabTransFormCheckedPortrait(object sender, RoutedEventArgs e)
	{
		ignore = true;
		((RangeBase)tabangleX).Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX;
		((RangeBase)tabangleY).Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY;
		ignore = false;
	}

	private void SearchTextBoxCurvatureChecked(object sender, RoutedEventArgs e)
	{
		ignore = true;
		((RangeBase)tabangleX).Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleX;
		((RangeBase)tabangleY).Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleY;
		ignore = false;
	}

	private void TabTransFormCheckedLandscape(object sender, RoutedEventArgs e)
	{
		ignore = true;
		((RangeBase)tabangleX).Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX;
		((RangeBase)tabangleY).Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY;
		ignore = false;
	}

	private void cornerRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!ignore)
		{
			((Dictionary<string, CornerRadius>)(object)BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)[((DataGridView)dataGrid1).CurrentRow.Cells["Name"].Value.ToString()] = new CornerRadius(((RangeBase)topleftCornerRadius).Value, ((RangeBase)toprightcornerradius).Value, ((RangeBase)bottomrightcornerradius).Value, ((RangeBase)bottomleftCornerRadius).Value);
			try
			{
				((DataGridView)dataGrid1)[1, ((DataGridViewBand)((DataGridView)dataGrid1).CurrentRow).Index].Value = (object)new CornerRadius(((RangeBase)topleftCornerRadius).Value, ((RangeBase)toprightcornerradius).Value, ((RangeBase)bottomrightcornerradius).Value, ((RangeBase)bottomleftCornerRadius).Value);
			}
			catch (Exception)
			{
			}
			BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
		}
	}

	private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (((Selector)ListView2).SelectedItem != null)
		{
			selectedItem = ((Selector)ListView2).SelectedItem.ToString();
			CustomPictureBox.SetBitmapImage(pictureBox, ((Selector)ListView2).SelectedItem.ToString(), false);
		}
	}

	private void pictureBox_MouseDown(object sender, MouseButtonEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (string.IsNullOrEmpty(selectedItem))
		{
			return;
		}
		OpenFileDialog val = new OpenFileDialog();
		bool? flag = ((CommonDialog)val).ShowDialog();
		if (flag.HasValue && flag.Value)
		{
			string fileName = ((FileDialog)val).FileName;
			string text = Path.Combine(DraftDirectory, selectedItem.ToString(CultureInfo.InvariantCulture));
			if (!File.Exists(text))
			{
				text += ".png";
			}
			File.Copy(fileName, text, overwrite: true);
			CustomPictureBox.UpdateImagesFromNewDirectory(DraftDirectory);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/themeeditorwindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Expected O, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Expected O, but got Unknown
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Expected O, but got Unknown
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Expected O, but got Unknown
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Expected O, but got Unknown
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Expected O, but got Unknown
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Expected O, but got Unknown
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Expected O, but got Unknown
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Expected O, but got Unknown
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Expected O, but got Unknown
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Expected O, but got Unknown
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Expected O, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			sliderR = (Slider)target;
			((RangeBase)sliderR).ValueChanged += Color_Changed;
			break;
		case 2:
			sliderG = (Slider)target;
			((RangeBase)sliderG).ValueChanged += Color_Changed;
			break;
		case 3:
			sliderB = (Slider)target;
			((RangeBase)sliderB).ValueChanged += Color_Changed;
			break;
		case 4:
			sliderA = (Slider)target;
			((RangeBase)sliderA).ValueChanged += Color_Changed;
			break;
		case 5:
			labelA = (Label)target;
			break;
		case 6:
			labelR = (Label)target;
			break;
		case 7:
			labelG = (Label)target;
			break;
		case 8:
			labelB = (Label)target;
			break;
		case 9:
			sliderX = (Slider)target;
			((RangeBase)sliderX).ValueChanged += Curve_Changed;
			break;
		case 10:
			sliderY = (Slider)target;
			((RangeBase)sliderY).ValueChanged += Curve_Changed;
			break;
		case 11:
			labelX = (Label)target;
			break;
		case 12:
			labelY = (Label)target;
			break;
		case 13:
			AppIcon = (Label)target;
			break;
		case 14:
			tabangleX = (Slider)target;
			((RangeBase)tabangleX).ValueChanged += tabangle_Changed;
			break;
		case 15:
			AngleX = (Label)target;
			break;
		case 16:
			tabangleY = (Slider)target;
			((RangeBase)tabangleY).ValueChanged += tabangle_Changed;
			break;
		case 17:
			AngleY = (Label)target;
			break;
		case 18:
			topleftCornerRadius = (Slider)target;
			((RangeBase)topleftCornerRadius).ValueChanged += cornerRadiusChanged;
			break;
		case 19:
			top = (Label)target;
			break;
		case 20:
			toprightcornerradius = (Slider)target;
			((RangeBase)toprightcornerradius).ValueChanged += cornerRadiusChanged;
			break;
		case 21:
			left = (Label)target;
			break;
		case 22:
			bottomleftCornerRadius = (Slider)target;
			((RangeBase)bottomleftCornerRadius).ValueChanged += cornerRadiusChanged;
			break;
		case 23:
			right = (Label)target;
			break;
		case 24:
			bottomrightcornerradius = (Slider)target;
			((RangeBase)bottomrightcornerradius).ValueChanged += cornerRadiusChanged;
			break;
		case 25:
			bottom = (Label)target;
			break;
		case 26:
			groupBox1 = (GroupBox)target;
			break;
		case 27:
			SearchTextBoxCurvature = (RadioButton)target;
			((ToggleButton)SearchTextBoxCurvature).Checked += new RoutedEventHandler(SearchTextBoxCurvatureChecked);
			break;
		case 28:
			TabTransFormPortrait = (RadioButton)target;
			((ToggleButton)TabTransFormPortrait).Checked += new RoutedEventHandler(TabTransFormCheckedPortrait);
			break;
		case 29:
			TabTransFormLandscape = (RadioButton)target;
			((ToggleButton)TabTransFormLandscape).Checked += new RoutedEventHandler(TabTransFormCheckedLandscape);
			break;
		case 30:
			textBox = (TextBox)target;
			((TextBoxBase)textBox).TextChanged += new TextChangedEventHandler(textBox_TextChanged);
			break;
		case 31:
			gridColor = (Grid)target;
			break;
		case 32:
			btnLoad = (Button)target;
			((ButtonBase)btnLoad).Click += new RoutedEventHandler(Load_Click);
			break;
		case 33:
			btnSave = (Button)target;
			((ButtonBase)btnSave).Click += new RoutedEventHandler(Save_Click);
			break;
		case 34:
			dataGrid = (GroupByGrid)target;
			break;
		case 35:
			dataGrid1 = (GroupByGrid)target;
			break;
		case 36:
			ListView2 = (ListBox)target;
			((UIElement)ListView2).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListViewItem_PreviewMouseLeftButtonDown);
			break;
		case 37:
			pictureBox = (Image)target;
			((UIElement)pictureBox).MouseDown += new MouseButtonEventHandler(pictureBox_MouseDown);
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
