using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;
using Glaxion.Libraries;

namespace Glaxion.Music
{
    public class Playlist
    {
        public string path;
        public string name;
        public string ext = ".m3u";
        //public ViewBox viewBox;
        int StoredIndex = 0;
        public int trackIndex;
        public int lastVisible = 0;
        public bool dirty;
        public bool debugSave;
        private List<List<String>> StoredLists = new List<List<String>>();
        public List<string> tracks = new List<string>();
        public static string DefaultPlaylistDirectory = @"Output\Playlists";

        public Playlist()
        {
            debugSave = true;
        }

        //use this in future to get default directory

        public static string FindDefaultDirectory()
        {
            string dd = Glaxion.Music.Properties.Settings.Default.DefaultPlaylistDirectory;
            if (Directory.Exists(dd))
                return dd;
            else
            {
                DefaultPlaylistDirectory = tool.BrowseForDirectory(true, true,"Select Main Playlist Directory");
                if (DefaultPlaylistDirectory == null)
                    return null;
                Glaxion.Music.Properties.Settings.Default.DefaultPlaylistDirectory = DefaultPlaylistDirectory;
                return DefaultPlaylistDirectory;
            }
        }
        
        public object Clone()
        {
            Playlist p = new Playlist();
            p.name = name;
            p.path = path;
            foreach (string s in tracks)
            {
                p.tracks.Add(s);
            }
            p.ext = ext;
            return p;
        }

        public string UpdateFilePath()
        {
            string dir = Path.GetDirectoryName(path);
            path = dir + @"\" + name + ext;
            return path;
        }

        public void StoreCurrentList()
        {
            StoredLists.Add(tracks);
            StoredIndex = StoredLists.Count();
            tool.debug("Undo Count: ", StoredIndex);
        }
        
        public void UpdateName(string newName)
        {
            name = newName;
            UpdateFilePath();
            /*
            name = newName;
            string oldDir = Path.GetDirectoryName(path);
            string newPath = oldDir + @"\" + newName + ext;
            path = newPath;
            */
        }
        
        public Playlist(string filePath, bool readFile)
        {
            path = filePath;
            debugSave = true;

            if (readFile && File.Exists(filePath))
            {
                ReadFile();
            }
            if (!File.Exists(filePath))
            {
                if(DefaultPlaylistDirectory != null)
                {
                    path = DefaultPlaylistDirectory + @"\" + filePath + ext;
                }
            }
            name = Path.GetFileNameWithoutExtension(filePath);
        }

        public void ReadFile()
        {
            tracks.Clear();
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                tracks.Add(line);
            }
            sr.Close();
        }

        public void GetFromFile(string fullpath)
        {
            path = fullpath;
            ReadFile();
        }

        public bool Save()
        {
            bool saved= WriteToFile(false);
            return saved;
        }

        public bool SaveAs()
        {
            return WriteToFile(true);
        }

        public bool SaveTo(string fullpath)
        {
            path = fullpath;
            name = Path.GetFileNameWithoutExtension(fullpath);
            return WriteToFile(false);
        }

        public string GetDirectory()
        {
            return Path.GetDirectoryName(path);
        }

        public static List<string> SelectPlaylistFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            
            ofd.Multiselect = true;
            List<string> ls = new List<string>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in ofd.FileNames)
                {
                    ls.Add(s);
                }
                return ls;
            }
            return ls;
        }

        //replaces the current path to the playlist
        //with the globally set default playlist directory
        public string GetDefaultPlaylistPath()
        {
            if(Path.HasExtension(name))
            {

                name = "error name";
                tool.Show(name);
            }
            path = FindDefaultDirectory() + @"\"+name + ext;
            return path;
        }

        public void SetDefaultPlaylistPath()
        {
            path = GetDefaultPlaylistPath();
        }

        public bool WriteToFile(bool append)
        {
            if (path == null || append)
            {
                SaveFileDialog od = new SaveFileDialog();
                if (Properties.Settings.Default.PlaylistDirectories != null && Properties.Settings.Default.PlaylistDirectories.Count > 0)
                {
                    od.InitialDirectory = FindDefaultDirectory();
                }

                od.FileName = name + ext;
                if (od.ShowDialog() == DialogResult.OK)
                {
                    path = od.FileName;
                    name = Path.GetFileNameWithoutExtension(path);
                    File.WriteAllLines(path, tracks);
                    tool.debug("Saved File: ", path);
                    if(debugSave)
                        tool.show(1, path, "", "Save Successfull");
                    return true;
                }else
                {
                    if (debugSave)
                        tool.show(1, path, "", "Failed to Save");
                    return false;
                }
            }
            if (File.Exists(path))
            {
                File.WriteAllLines(path, tracks);
                tool.debug("Saved File: ", path);
                if (debugSave)
                    tool.show(1, path, "", "Save Successfull");
                return true;
            }
            else
            {
                if (path == null)
                {
                    if (debugSave)
                        tool.show(1, path, "", "Failed to Save");
                    return false;
                }

                if(!Directory.Exists(path))
                {
                    //string dir = FinbdDefaultDirectory();
                    string tmp = @"Output\tmp\";
                    Directory.CreateDirectory(tmp);
                    path = tmp + @"\" + name + ext;
                }

                FileStream fs = File.Create(path);
                fs.Close();
                File.WriteAllLines(path, tracks);
                tool.debug("Created File: ", path);
                if (debugSave)
                    tool.show(1, path, "", "Save Successfull");
                return true;
            }
        }

        public void UpdatePaths()
        {
            //List<string> updated = new List<string>();
            for(int i=0;i<tracks.Count;i++)
            {
                //need to remove illegal characters
                string t = tracks[i];
                if (File.Exists(t))
                    continue;
                List<string> l = MusicPlayer.Player.SearchMusicFiles(Path.GetFileNameWithoutExtension(t));
                if (l.Count  >0)
                {
                    string firstFound = l[0];
                    tracks.RemoveAt(i);
                    tracks.Insert(i, firstFound);
                }
                //TODO handle case where multiple files are found
            }
        }
    }
}
