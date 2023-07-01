using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinecraftUArchiveExplorer.Classes.Networking;

namespace MinecraftUArchiveExplorer.Forms
{
    public partial class ArcCenter : Form
    {
        public API _api = new API();
        public ArcCenter()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach (KeyValuePair<int, CategorieItem> cat in _api.GetCategories()[comboBox1.SelectedIndex].Data)
            {
                ListViewItem item = new ListViewItem();
                item.Text = cat.Value.Name + "(" + cat.Key + ")";
                item.Tag = cat.Key;
                listView1.Items.Add(item);
            }
            richTextBox1.Text = "";
            label1.Text = "";
            pictureBoxWithInterpolationMode1.Image = null;
            button1.Enabled = false;
        }

        private void ArcCenter_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            label1.Text = "";
            pictureBoxWithInterpolationMode1.Image = null;
            Categorie cat = _api.GetCategories()[comboBox1.SelectedIndex];
            string auth = cat.Data[(int)listView1.SelectedItems[0].Tag].Author;
            string desc = cat.Data[(int)listView1.SelectedItems[0].Tag].Description;
            string name = cat.Data[(int)listView1.SelectedItems[0].Tag].Name;
            bool IsComplete = cat.Data[(int)listView1.SelectedItems[0].Tag].Full;

            label1.Text = name;
            richTextBox1.Text = "author: " + auth + "\ndescription: " + desc + "\nIs Complete Archive: " + IsComplete;

            pictureBoxWithInterpolationMode1.Image = _api.GetArchiveImage((int)listView1.SelectedItems[0].Tag);
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "archive File|*.arc";
            saveFileDialog.RestoreDirectory = true;
            Categorie cat = _api.GetCategories()[comboBox1.SelectedIndex];
            string name = cat.Data[(int)listView1.SelectedItems[0].Tag].Name;
            saveFileDialog.FileName = "["+ comboBox1.Text + "]"+name;
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _api.DownloadArchiveToPath((int)listView1.SelectedItems[0].Tag, saveFileDialog.FileName);
            }
        }
    }
}
