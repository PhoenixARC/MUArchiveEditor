using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Forms.Additional_Popups.Loc;
using OMI.Formats.Color;
using OMI.Workers.Color;
using OMI.Formats.Archive;
using Newtonsoft.Json;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MinecraftUArchiveExplorer.Forms.Editor
{
	public partial class COLEditor : Form
	{
		public ColorContainer _col { get; set; }
		public ColorContainer currentcol { get; set; }

		int LOCType = 0;
		ConsoleArchive _arc;
		string FileName;
		byte[] _file;

		DataTable Colors = new DataTable();
		DataTable WaterColors = new DataTable();

        ImageList colimglst = new ImageList();

        ColorConverter cconv = new ColorConverter();

        public COLEditor()
		{
			InitializeComponent();

            currentcol = _col;


            Colors.Columns.Add("name");
            Colors.Columns.Add("colour");

            WaterColors.Columns.Add("name");
            WaterColors.Columns.Add("surfaceColour");
            WaterColors.Columns.Add("underwaterColour");
            WaterColors.Columns.Add("fogColour");

		}

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			foreach (ColorContainer.Color colour in _col.Colors)
			{
				DataRow dr = Colors.NewRow();
				dr["name"] = colour.Name;
				string ColCode = "#" + (colour.ColorPallette.ToArgb() & 0x00FFFFFF).ToString("X6");
				dr["colour"] = ColCode;

                Colors.Rows.Add(dr);
			}
			foreach (ColorContainer.WaterColor Wcolour in _col.WaterColors)
			{
				DataRow dr = WaterColors.NewRow();
				dr["name"] = Wcolour.Name;
				string surfaceColour = "#" + (Wcolour.SurfaceColor.ToArgb() & 0x00FFFFFF).ToString("X6");
				string underwaterColour = "#" + (Wcolour.UnderwaterColor.ToArgb() & 0x00FFFFFF).ToString("X6");
				string fogColour = "#" + (Wcolour.FogColor.ToArgb() & 0x00FFFFFF).ToString("X6");
				dr["surfaceColour"] = surfaceColour;
				dr["underwaterColour"] = underwaterColour;
				dr["fogColour"] = fogColour;
				WaterColors.Rows.Add(dr);
			}

			comboBox1.SelectedIndex = 0;

		}


		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (MessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DialogResult = DialogResult.OK;
			}

		}


		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			listView1.Items.Clear();

            if (comboBox1.SelectedIndex == 1)
                RefreshListView(WaterColors);
            else
                RefreshListView(Colors);
        }

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBox1.Text))
			{
				if (comboBox1.SelectedIndex == 1)
                    RefreshListView(WaterColors);
				else
                    RefreshListView(Colors);

            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
			if(e.KeyChar == (char)13)
			{
				DataTable dt = SearchInAllColums(Colors, textBox1.Text, StringComparison.CurrentCultureIgnoreCase);
				if (comboBox1.SelectedIndex == 1)
					dt = SearchInAllColums(WaterColors, textBox1.Text, StringComparison.CurrentCultureIgnoreCase);

                RefreshListView(dt);
            }
        }

		private void RefreshListView(DataTable _table)
		{
			listView1.Items.Clear();
            foreach (DataRow row in _table.Rows)
            {
				EXListViewItem lstvitem = new EXListViewItem();
                for (int i = 0; i < _table.Columns.Count; i++)
                {
                    EXControlListViewSubItem SubITM = new EXControlListViewSubItem();
					SubITM.Text = row[i].ToString();
					if (row[i].ToString().StartsWith("#"))
					{
						Color col = (Color)cconv.ConvertFromString(row[i].ToString());
                        colimglst.Images.Add(row[i].ToString(), GetColor(col));
                    }
                    lstvitem.SubItems.Add(SubITM);
                }
                listView1.Items.Add(lstvitem);
            }
        }

        private DataTable SearchInAllColums(DataTable table, string keyword, StringComparison comparison)
        {
            if (keyword.Equals(""))
            {
                return table;
            }
            DataRow[] filteredRows = table.Rows
                   .Cast<DataRow>()
                   .Where(r => r.ItemArray.Any(
                   c => c.ToString().IndexOf(keyword, comparison) >= 0))
                   .ToArray();

            if (filteredRows.Length == 0)
            {
                DataTable dtTemp = table.Clone();
                dtTemp.Clear();
                return dtTemp;
            }
            else
            {
                return filteredRows.CopyToDataTable();
            }
        }

		private Image GetColor(Color colour)
		{
			int w = 8, h = 8;
            Bitmap Bmp = new Bitmap(w, h);
            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new SolidBrush(colour))
            {
                gfx.FillRectangle(brush, 0, 0, w, h);
            }
			return Bmp;
        }
    }
}
