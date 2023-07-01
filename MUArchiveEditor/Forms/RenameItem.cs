using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftUArchiveExplorer.Forms
{
    public partial class RenameItem : Form
    {
        public RenameItem(string InitialText) : this(InitialText, -1)
        {
        }
        public RenameItem(string InitialText, int maxChar)
        {
            InitializeComponent();
            textBox1.Text = InitialText;
            textBox1.MaxLength = maxChar < 0 ? short.MaxValue : maxChar;
        }
        public string NewText => textBox1.Text;


        private void OKBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OKBtn_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void RenameItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
