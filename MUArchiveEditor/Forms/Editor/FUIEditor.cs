using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.FUI;
using OMI.Formats.FUI.Components;
using OMI.Workers.FUI;
using OMI.Formats.Archive;
using Newtonsoft.Json;
using System.Drawing;
using OMI.Formats.Languages;

namespace MinecraftUArchiveExplorer.Forms.Editor
{
    public partial class FUIEditor : Form
    {
        public FourjUserInterface _fui { get; set; }
        public FourjUserInterface currentfui { get; set; }

        ConsoleArchive _arc;
        string FileName;
        byte[] _file;
        public FUIEditor()
        {
            InitializeComponent();
        }

        private void archiveStructureTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (archiveStructureTreeView.SelectedImageIndex == 1) 
            {
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (archiveStructureTreeView.SelectedImageIndex == 1)
            {
                replaceToolStripMenuItem.Enabled = true;
            }
            else
            {
                replaceToolStripMenuItem.Enabled = false;
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(archiveStructureTreeView.SelectedNode.Text.EndsWith(".jpg"))
                dialog.Filter = "JPEG Image|*.jpg,*.jpeg";
            else
                dialog.Filter = "PNG Image|*.png";
            if (dialog.ShowDialog() == DialogResult.OK) 
            {
                
            }
        }

        private void FUIEditor_Load(object sender, EventArgs e)
        {
            int index = 0;
            foreach (FuiBitmap image in _fui.Bitmaps) 
            {
                switch (image.ImageFormat)
                {
                    case (FuiBitmap.FuiImageFormat.PNG_NO_ALPHA_DATA):
                        archiveStructureTreeView.Nodes.Add(index + ".png");
                        break;
                    case (FuiBitmap.FuiImageFormat.PNG_WITH_ALPHA_DATA):
                        archiveStructureTreeView.Nodes.Add(index + ".png");
                        break;
                    case (FuiBitmap.FuiImageFormat.JPEG_UNKNOWN):
                        archiveStructureTreeView.Nodes.Add(index + ".jpg");
                        break;
                    case (FuiBitmap.FuiImageFormat.JPEG_WITH_ALPHA_DATA):
                        archiveStructureTreeView.Nodes.Add(index + ".jpg");
                        break;
                    case (FuiBitmap.FuiImageFormat.JPEG_NO_ALPHA_DATA):
                        archiveStructureTreeView.Nodes.Add(index + ".jpg");
                        break;
                }

                index++;
            }
        }
    }
}
