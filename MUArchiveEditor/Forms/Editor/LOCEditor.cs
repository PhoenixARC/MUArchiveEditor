using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Forms.Additional_Popups.Loc;
using OMI.Formats.Languages;
using OMI.Workers.Language;
using OMI.Formats.Pck;
using OMI.Formats.Archive;
using Newtonsoft.Json;
using System.Drawing;

namespace MinecraftUArchiveExplorer.Forms.Editor
{
	public partial class LOCEditor : Form
	{
		public LOCFile _loc { get; set; }
		public LOCFile currentloc { get; set; }

		int LOCType = 0;
		ConsoleArchive _arc;
		string FileName;
		byte[] _file;

		DataTable Table = new DataTable();

		

		Dictionary<string, string> languageMap_id = new Dictionary<string, string>();
		Dictionary<string, string> languageMap_text = new Dictionary<string, string>();
		Dictionary<string, string> languageMap = new Dictionary<string, string>();

		public LOCEditor()
		{
			InitializeComponent();

			currentloc = _loc;


			Table.Columns.Add("ID");
			Table.Columns.Add("Label");
			Table.Columns.Add("Text");

            foreach (string line in Properties.Resources.Language_ID_map.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] parts = line.Split('=');
				if (!languageMap_text.ContainsKey(parts[0]) && !languageMap_id.ContainsKey(parts[1]))
				{
					languageMap_id.Add(parts[1], parts[0]);
					languageMap_text.Add(parts[0], parts[1]);
				}
			}
		}

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			foreach (string locKey in _loc.LocKeys.Keys)
			{

				string ID = "";
				if (languageMap_id.ContainsKey(locKey))
					ID = languageMap_id[locKey];
				else
				{
					ID = locKey;

					string LocValue = _loc.LocKeys[locKey]["en-EN"].Replace("\n", "\\n").Replace("\r", "\\r").ToLower();
					if (!languageMap.ContainsKey(LocValue))
						languageMap.Add(LocValue, locKey);
				}

			}

			foreach (string lang in _loc.Languages)
			{
				comboBox1.Items.Add(lang);
			}

		}


		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (MessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				_loc = currentloc;
				DialogResult = DialogResult.OK;
			}

		}


		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			listView1.Items.Clear();
			Table.Rows.Clear();

			foreach (string locKey in currentloc.LocKeys.Keys)
			{

				string ID = locKey;
				string Text = currentloc.LocKeys[locKey][comboBox1.Text];
				string Label = "";
				if (languageMap_id.ContainsKey(locKey))
					Label = languageMap_id[locKey];

                DataRow Row = Table.NewRow();
				Row["ID"] = ID;
				Row["Label"] = Label;
				Row["Text"] = Text;

                Table.Rows.Add(Row);

			}
			RefreshListView(Table);
        }

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (listView1.SelectedItems[0] == null)
				return;

			LOCEditMessage msg = new LOCEditMessage();
			msg.ID = listView1.SelectedItems[0].SubItems[0].Text;
			msg.Label = listView1.SelectedItems[0].SubItems[1].Text;
			msg.Value = listView1.SelectedItems[0].SubItems[2].Text;

			msg.ShowDialog();

			if (msg.NewValue != msg.Value)
			{
				listView1.SelectedItems[0].ForeColor = Color.Red;
                listView1.SelectedItems[0].SubItems[2].Text = msg.NewValue;

				currentloc.LocKeys[msg.ID][comboBox1.Text] = msg.NewValue;
            }
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(textBox1.Text))
                RefreshListView(Table);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
			if(e.KeyChar == (char)13)
			{
				DataTable dt = SearchInAllColums(Table, textBox1.Text, StringComparison.CurrentCultureIgnoreCase);
				RefreshListView(dt);
            }
        }

		private void RefreshListView(DataTable _table)
		{
			listView1.Items.Clear();
            foreach (DataRow row in _table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < _table.Columns.Count; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                listView1.Items.Add(item);
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

		string GetRandomID() 
		{
            Random res = new Random();

            // String of alphabets  
            string str = "0123456789ABCDEF";
            int size = 8;

            // Initializing the empty string 
            string ran = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly 
                int x = res.Next(str.Length);

                // Appending the character at the  
                // index to the random string. 
                ran = ran + str[x];
            }
			return ran;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            LOCEditMessage msg = new LOCEditMessage();
            msg.ID = GetRandomID();
            msg.Label = "";
            msg.Value = "";

            msg.ShowDialog();

            if (msg.NewValue != msg.Value)
            {
				currentloc.AddLocKey(msg.ID, msg.NewValue);
				comboBox1.SelectedIndex = 0;

				textBox1.Text = msg.ID;
            }
        }

        private void copyEntryIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Clipboard.SetText(listView1.SelectedItems[0].Text);
			MessageBox.Show("Copied '" + listView1.SelectedItems[0].Text + "' to the clipboard!");
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
			if (listView1.SelectedItems.Count == 1)
				copyEntryIDToolStripMenuItem.Enabled = true;
			else
                copyEntryIDToolStripMenuItem.Enabled = false;
        }
    }
}
