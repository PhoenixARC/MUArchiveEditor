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

namespace MinecraftUArchiveExplorer.Forms.Editor
{
    public partial class LOCEditor : Form
	{
		DataTable tbl;
		int LOCType = 0;
		LOCFile currentLoc;
		ConsoleArchive _arc;
		Dictionary<string, string> languageMap_id = new Dictionary<string, string>();
		Dictionary<string, string> languageMap_text = new Dictionary<string, string>();
		string FileName;
		byte[] _file;

		public LOCEditor(ConsoleArchive _archive, string path)
		{
			InitializeComponent();
			FileName = path;
			_file = _archive[path];
			using (var ms = new MemoryStream(_file))
			{
				var reader = new LOCFileReader();
				currentLoc = reader.FromStream(ms);
			}

			foreach (string line in Properties.Resources.Language_ID_map.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] parts = line.Split('=');
				if (!languageMap_text.ContainsKey(parts[0]) && !languageMap_id.ContainsKey(parts[1]))
				{
					languageMap_id.Add(parts[1], parts[0]);
					languageMap_text.Add(parts[0], parts[1]);
				}
			}
			tbl = new DataTable();
			tbl.Columns.Add(new DataColumn("Language") { ReadOnly = true });
			tbl.Columns.Add("Display Name");
			dataGridViewLocEntryData.DataSource = tbl;
			DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
			column.Width = dataGridViewLocEntryData.Width;
		}

		private void LOCEditor_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> languageMap = new Dictionary<string, string>();
			foreach (string locKey in currentLoc.LocKeys.Keys)
			{

				string ID = "";
				if (languageMap_id.ContainsKey(locKey))
					ID = languageMap_id[locKey];
				else
				{
					ID = locKey;

					string LocValue = currentLoc.LocKeys[locKey]["en-EN"].Replace("\n", "\\n").Replace("\r", "\\r").ToLower();
					if(!languageMap.ContainsKey(LocValue))
						languageMap.Add(LocValue, locKey);
				}

				treeViewLocKeys.Nodes.Add(ID);
			}

			Dictionary<string, string> languageMap2 = new Dictionary<string, string>();
			Dictionary<string, string> BedrockIn = new Dictionary<string, string>();
			Dictionary<string, string> JavaIn = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("en_us.json")) ;
			string FileIn = File.ReadAllText("en_US.lang");
			foreach (KeyValuePair<string, string> kvp in JavaIn)
			{
                if (languageMap.ContainsKey(kvp.Value.ToLower()) && !languageMap2.ContainsKey(languageMap[kvp.Value.ToLower()]))
                {
					//languageMap2.Add(languageMap[kvp.Value.ToLower()], kvp.Key);

				}
			}

			foreach (string line in FileIn.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] parts = line.Split('=');
				if (parts.Length >= 2 && languageMap.ContainsKey(parts[1].ToLower()) && !languageMap2.ContainsKey(languageMap[parts[1].ToLower()]))
				{
					languageMap2.Add(languageMap[parts[1].ToLower()], parts[0]);

				}
			}

			StreamWriter sw = new StreamWriter("WoW.txt");
			foreach(KeyValuePair<string, string> kvp in languageMap2)
            {
				sw.WriteLine(kvp.Value + "=" + kvp.Key);
            }
			sw.Close();
			
		}

		private void treeViewLocKeys_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var node = e.Node;
			string ID = "";
			if (languageMap_text.ContainsKey(node.Text))
				ID = languageMap_text[node.Text];
			else
				ID = node.Text;

			if (node == null ||
				!currentLoc.LocKeys.ContainsKey(ID))
			{
				MessageBox.Show("Selected Node does not seem to be in the loc file");
				return;
			}
			ReloadTranslationTable(ID);
		}

		private void addDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void deleteDisplayIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			string ID = "";
			if (languageMap_text.ContainsKey(treeViewLocKeys.SelectedNode.Text))
				ID = languageMap_text[treeViewLocKeys.SelectedNode.Text];
			else
				ID = treeViewLocKeys.SelectedNode.Text;
			if (e.ColumnIndex != 1 ||
				treeViewLocKeys.SelectedNode == null)
			{
				MessageBox.Show("something went wrong");
				return;
			}
			currentLoc.SetLocEntry(ID, tbl.Rows[e.RowIndex][0].ToString(), tbl.Rows[e.RowIndex][1].ToString());
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
		}

		private void buttonReplaceAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < tbl.Rows.Count; i++)
			{
				tbl.Rows[i][1] = textBoxReplaceAll.Text;
			}

			currentLoc.SetLocEntry(treeViewLocKeys.SelectedNode.Text, textBoxReplaceAll.Text);
		}

		private void LOCEditor_Resize(object sender, EventArgs e)
		{
			DataGridViewColumn column = dataGridViewLocEntryData.Columns[1];
			column.Width = dataGridViewLocEntryData.Width - dataGridViewLocEntryData.Columns[0].Width;
		}

		private void ReloadTranslationTable(string ID)
		{
			tbl.Rows.Clear();
			foreach (var l in currentLoc.GetLocEntries(ID))
				tbl.Rows.Add(l.Key, l.Value);
		}

		private IEnumerable<string> GetAvailableLanguages()
		{
			foreach (var lang in LOCFile.ValidLanguages)
			{
				if (currentLoc.Languages.Contains(lang)) continue;
				yield return lang;
			}
			yield break;
		}

		private void addLanguageToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ms = new MemoryStream())
			{
				new LOCFileWriter(currentLoc, 0).WriteToStream(ms);
				_arc[FileName] = ms.ToArray();
			}
			DialogResult = DialogResult.OK;
		}

        private void locSort_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
