using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;

namespace MinecraftUArchiveExplorer.Classes.Networking
{
    public class API
    {
        public string url = Program.API_URL + "/api/arc/center";
        public WebClient wc = new WebClient();
        public ArchiveIDs archiveIDs = new ArchiveIDs();
        public Categorie PS3 = new Categorie();
        public Categorie WiiU = new Categorie();
        public Categorie Xbox360 = new Categorie();
        public Categorie Vita = new Categorie();

        public API()
        {
            Initialize();
        }

        public void Initialize()
        {
            try
            {
                archiveIDs = JsonConvert.DeserializeObject<ArchiveIDs>(wc.DownloadString(url + "/ArchiveIDs.json"));
                PS3 = JsonConvert.DeserializeObject<Categorie>(wc.DownloadString(url + "/cat/Archive/PS3.json"));
                WiiU = JsonConvert.DeserializeObject<Categorie>(wc.DownloadString(url + "/cat/Archive/WiiU.json"));
                Xbox360 = JsonConvert.DeserializeObject<Categorie>(wc.DownloadString(url + "/cat/Archive/Xbox360.json"));
                Vita = JsonConvert.DeserializeObject<Categorie>(wc.DownloadString(url + "/cat/Archive/Vita.json"));
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an issue with the archive Categories, please check connection and try again...");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public Categorie[] GetCategories()
        {
            return new Categorie[] { PS3, Vita, WiiU, Xbox360 };
        }
        public Image GetArchiveImage(int ID)
        {
            try
            {
                Image img = Image.FromStream(new MemoryStream(wc.DownloadData(url + "/cat/Archive/Image/" + ID + ".png")));
                return img;
            }
            catch(WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an issue with the archive image, substituting default...");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Image.FromStream(new MemoryStream(wc.DownloadData(url + "/cat/Archive/Image/default.png")));
            }
        }
        public void DownloadArchiveToPath(int ID, string FilePath)
        {
            try
            {
                wc.DownloadFile(url + "/cat/Archive/File/" + ID + ".arc", FilePath);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an issue with the archive, please check connection and try again...");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }

    public class ArchiveIDs
    {
        public List<int> PS3, WiiU, Xbox360, Vita;
    }
    public class Categorie
    {
        public Dictionary<int, CategorieItem> Data;
    }
    public class CategorieItem
    {
        public string Name;
        public string Author;
        public string Description;
        public bool Full;
    }
}
