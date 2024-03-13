using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Archive;
using OMI.Workers.Archive;
using MinecraftUArchiveExplorer.Classes.Archive;
using MinecraftUArchiveExplorer.Forms.Editor;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging;
using OMI.Workers.Language;
using OMI.Formats.Languages;
using MinecraftUArchiveExplorer.Forms.AdditionalPopups;
using OMI.Formats.Color;
using OMI.Workers.Color;

namespace MinecraftUArchiveExplorer
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();


            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(20, 20);

            imageList.Images.Add(Properties.Resources.ZZFolder);
            imageList.Images.Add(Properties.Resources.IMAGE_ICON);
            imageList.Images.Add(Properties.Resources.ZFui);
            imageList.Images.Add(Properties.Resources.LOC_ICON);
            imageList.Images.Add(Properties.Resources.ZCol);
            imageList.Images.Add(Properties.Resources.ZUnknown);

            label2.Text = label2.Text.Replace("%s", Application.ProductVersion);
            archiveStructureTreeView.ImageList = imageList;
        }
        public FormMain(string FilePath)
        {
            InitializeComponent();


            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(20, 20);

            imageList.Images.Add(Properties.Resources.ZZFolder);
            imageList.Images.Add(Properties.Resources.IMAGE_ICON);
            imageList.Images.Add(Properties.Resources.ZFui);
            imageList.Images.Add(Properties.Resources.LOC_ICON);
            imageList.Images.Add(Properties.Resources.ZCol);
            imageList.Images.Add(Properties.Resources.ZUnknown);

            label2.Text = label2.Text.Replace("%s", Application.ProductVersion);
            archiveStructureTreeView.ImageList = imageList;

            try
            {
                Archive.Clear();
                System.GC.Collect();
                Archive = AR.FromFile(FilePath);
                BuildTreeView(archiveStructureTreeView.Nodes, Archive);
                Console.WriteLine("success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening file:");
                Console.WriteLine(ex.Message);
            }
        }

        public ConsoleArchive Archive = new ConsoleArchive();
        ArchiveActions Actions = new ArchiveActions();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        ARCFileReader AR = new ARCFileReader();
        OpenFileDialog ofd = new OpenFileDialog(); 

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Minecraft Archive Files|*.arc";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Archive.Clear();
                System.GC.Collect();
                Archive = AR.FromFile(ofd.FileName);
                BuildTreeView(archiveStructureTreeView.Nodes, Archive);
                Console.WriteLine("success!");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Minecraft Archive Files|*.arc";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                new ARCFileWriter(Archive).WriteToFile(saveFileDialog.FileName);
            }
        }

        private void BuildTreeView(TreeNodeCollection root, ConsoleArchive arcfile)
        {
            archiveStructureTreeView.Nodes.Clear();
            foreach (KeyValuePair<string, byte[]> file in arcfile.OrderBy(x => x.Key))
            {
                Console.WriteLine("never wahen just building eithaer, juast debugginagq");
                TreeNode node = BuildNodeTreeBySeperator(root, file.Key, '\\');
            }
        }

        private TreeNode BuildNodeTreeBySeperator(TreeNodeCollection root, string path, char seperator)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            if (!path.Contains(seperator))
            {
                var finalNode = CreateNode(path);
                root.Add(finalNode);
                return finalNode;
            }
            string nodeText = path.Substring(0, path.IndexOf(seperator));
            string subPath = path.Substring(path.IndexOf(seperator) + 1);
            bool alreadyExists = root.ContainsKey(nodeText);
            TreeNode subNode = alreadyExists ? root[nodeText] : CreateNode(nodeText);
            subNode.ImageIndex = 0;
            subNode.SelectedImageIndex = 0;
            if (!alreadyExists) root.Add(subNode);
            
            return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
        }
        /// <summary>
		/// wrapper that allows the use of <paramref name="name"/> in <code>TreeNode.Nodes.Find(<paramref name="name"/>, ...)</code> and <code>TreeNode.Nodes.ContainsKey(<paramref name="name"/>)</code>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="tag"></param>
		/// <returns>new Created TreeNode</returns>
		public static TreeNode CreateNode(string name, object tag = null)
        {
            TreeNode node = new TreeNode(name);
            node.Name = name;
            node.Tag = tag;
            node.ImageIndex = DetectFiletype(name);
            node.SelectedImageIndex = DetectFiletype(name);
            return node;
        }

        public static int DetectFiletype(string filepath)
        {
            int returnvalue = 5;
            switch (Path.GetExtension(filepath))
            {
                case ".png":
                    returnvalue = 1;
                    break;
                case ".fui":
                    returnvalue = 2;
                    break;
                case ".loc":
                    returnvalue = 3;
                    break;
                case ".col":
                    returnvalue = 4;
                    break;
                case ".txt":
                    returnvalue = 5;
                    break;
            }
            return returnvalue;
        }

        public void RemoveTreeNode(TreeNode tn)
        {
            TreeNode parent = tn.Parent;
            int level = tn.Level;
            tn.Remove();
            if (level != 0 && parent != null)
                if (parent.Nodes.Count == 0)
                    RemoveTreeNode(parent);
        }

        private void archiveStructureTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (pictureBoxWithInterpolationMode1 != null)
            {
                if (archiveStructureTreeView.SelectedNode.ImageIndex == 1)
                {
                    string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
                    byte[] imagedata = Archive[FullPath];
                    MemoryStream ms = new MemoryStream(imagedata);
                    Image img = Image.FromStream(ms);
                    pictureBoxWithInterpolationMode1.Image = img;
                    label1.Text = img.Width + "x" + img.Height;
                    ms.Close();
                }
                else
                {
                    pictureBoxWithInterpolationMode1.Image = null;
                    label1.Text = "0x0";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            System.GC.Collect();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Any File|*";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
                Archive = Actions.ReplaceItem(Archive, FullPath, File.ReadAllBytes(ofd.FileName));
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
            Archive = Actions.RemoveItem(Archive, FullPath);
            RemoveTreeNode(archiveStructureTreeView.SelectedNode);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
            Forms.RenameItem ritem = new Forms.RenameItem(FullPath);
            if (ritem.ShowDialog() == DialogResult.OK)
            {
                string newText = ritem.NewText;
                RemoveTreeNode(archiveStructureTreeView.SelectedNode);
                Archive = Actions.RenameItem(Archive, FullPath, newText);
                BuildNodeTreeBySeperator(archiveStructureTreeView.Nodes, newText, '\\');
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Any File|*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Forms.RenameItem ritem = new Forms.RenameItem(Path.GetFileName(ofd.FileName));
                if(ritem.ShowDialog() == DialogResult.OK)
                {
                    string newText = ritem.NewText;
                    Archive = Actions.AddItem(Archive, newText, File.ReadAllBytes(ofd.FileName));
                    BuildNodeTreeBySeperator(archiveStructureTreeView.Nodes, newText, '\\');
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archiveStructureTreeView.Nodes.Count != 0)
            {
                if (MessageBox.Show("Are you sure you wish to discard unsaved change(s)?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    archiveStructureTreeView.Nodes.Clear();
                    Archive = null;
                }
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archiveStructureTreeView.SelectedNode.ImageIndex == 0)
            {
                string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK) 
                {
                    foreach (KeyValuePair<string, byte[]> pair in Archive) 
                    {
                        if (pair.Key.StartsWith(FullPath))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fbd.SelectedPath + "\\" + pair.Key));
                            File.WriteAllBytes(fbd.SelectedPath + "\\" + pair.Key, pair.Value);
                        }
                    }
                }
            }
            else
            {
                string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
                saveFileDialog.Filter = "File|*" + Path.GetExtension(FullPath);
                saveFileDialog.FileName = archiveStructureTreeView.SelectedNode.Text;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, Archive[FullPath]);
                }
            }
            MessageBox.Show("Extracted!");
        }

        private void aRCCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ArcCenter ac = new Forms.ArcCenter();
            ac.ShowDialog();
        }

        private void archiveStructureTreeView_DoubleClick(object sender, EventArgs e)
        {
            string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
            if (Path.GetExtension(FullPath) == ".loc")
            {
                LOCFile _loc = new LOCFile();
                MemoryStream ms = new MemoryStream(Archive[FullPath]);
                _loc = new LOCFileReader().FromStream(ms);

                LOCEditor LE = new LOCEditor();
                LE._loc = _loc;
                LE.currentloc = _loc;
                if(LE.ShowDialog() == DialogResult.OK)
                {
                    MemoryStream ms2 = new MemoryStream();
                    new LOCFileWriter(LE._loc, 2).WriteToStream(ms2);
                    Archive[FullPath] = ms2.ToArray();
                    ms2.Close();
                    ms2.Dispose();
                }
                ms.Close();
                ms.Dispose();
            }
            else if (Path.GetExtension(FullPath) == ".col")
            {
                ColorContainer _col = new ColorContainer();
                MemoryStream ms = new MemoryStream(Archive[FullPath]);
                _col = new COLFileReader().FromStream(ms);

                COLEditor LE = new COLEditor();
                LE._col = _col;
                LE.currentcol = _col;
                if(LE.ShowDialog() == DialogResult.OK)
                {
                    MemoryStream ms2 = new MemoryStream();
                    new COLFileWriter(_col).WriteToStream(ms2);
                    Archive[FullPath] = ms2.ToArray();
                    ms2.Close();
                    ms2.Dispose();
                }
                ms.Close();
                ms.Dispose();
            }
            else
            {
                if(Path.GetExtension(FullPath) != ".png" && Path.GetExtension(FullPath) != "")
                {
                    MessageBox.Show("No current \'" + Path.GetExtension(FullPath) + "\' editor, extracting file...");
                }
                extractToolStripMenuItem_Click(sender, e);
            }

        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, FormClosingEventArgs e)
        {

        }

        private void cycleSavingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FullPath = archiveStructureTreeView.SelectedNode.FullPath;
            if (Path.GetExtension(FullPath) == ".loc")
            {
                MemoryStream ms = new MemoryStream(Archive[FullPath]);
                LOCFile _loc = new LOCFileReader().FromStream(ms);
                MemoryStream ms2 = new MemoryStream();
                new LOCFileWriter(_loc, 0).WriteToStream(ms2);
                Archive[FullPath] = ms2.ToArray();
            }
        }

        private void programInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About abt = new About();
            abt.ShowDialog();
        }
    }
}