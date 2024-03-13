using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftUArchiveExplorer.Forms.Editor
{
    public partial class LOCEditMessage : Form
    {

        public String ID { get; set; }
        public String Label { get; set; }
        public String Value { get; set; }

        public String NewValue { get; set; }
        public LOCEditMessage()
        {
            InitializeComponent();
        }

        private void LOCEditMessage_Load(object sender, EventArgs e)
        {
            textBox1.Text = Value;
            textBox2.Text = ID;
            textBox3.Text = Label;

            NewValue = Value;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            NewValue = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                    this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?","", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                NewValue = Value;
                this.Close();
            }
        }
    }
}
