using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Archive;

namespace MinecraftUArchiveExplorer.Classes.Archive
{
    public class ArchiveActions
    {

        public ConsoleArchive ReplaceItem(ConsoleArchive _arc, string filepath, byte[] replacementData)
        {
            _arc = RemoveItem(_arc, filepath);
            _arc = AddItem(_arc, filepath, replacementData);
            return _arc;
        }
        public ConsoleArchive RenameItem(ConsoleArchive _arc, string filepath, string replacementFilepath)
        {
            byte[] FileData = _arc[filepath];
            _arc = RemoveItem(_arc, filepath);
            _arc = AddItem(_arc, replacementFilepath, FileData);
            return _arc;
        }
        public ConsoleArchive RemoveItem(ConsoleArchive _arc, string filepath)
        {
            _arc.Remove(filepath);
            return _arc;
        }
        public ConsoleArchive AddItem(ConsoleArchive _arc, string filepath, byte[] Filedata)
        {
            _arc.Add(filepath, Filedata);
            return _arc;
        }

    }
}
