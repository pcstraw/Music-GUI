using Glaxion.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Glaxion.Music
{
    public class FileLoader
    {
        private FileLoader() { }////lazy singleton
        public static FileLoader Instance { get { return Nested.instance; } }
        private class Nested
        {
            static Nested() { }
            internal static readonly FileLoader instance = new FileLoader();
        }

        bool tagsLoaded;
        //for keeping track of directories added.  Each directory only has one entry in the heirachy
        public List<string> musicDirectories = new List<string>();
        public List<string> PlaylistDirectories = new List<string>();
        public List<string> MusicDirectories = new List<string>();
        public Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();
        public Dictionary<string, string> MusicFiles = new Dictionary<string, string>();
        private MusicInfo musicInfo;

        public void Load()
        {
            musicInfo = MusicInfo.Instance;
            GetSavedDirectories();
            PlaylistDirectories = tool.GetPropertyList(Properties.Settings.Default.PlaylistDirectories);
        }

        public void SavePlaylistDirectories()
        {
            tool.SetPropertyList(PlaylistDirectories, Properties.Settings.Default.PlaylistDirectories);
            Properties.Settings.Default.Save();
        }

        public void SaveDirectories()
        {
            tool.SetPropertyList(MusicDirectories, Properties.Settings.Default.MusicDirectories);
            foreach (string s in Properties.Settings.Default.MusicDirectories)
                tool.debug(s);
            Properties.Settings.Default.Save();
        }

        public bool BrowseMusicDirectory()
        {
            if (!HasLoadedTags())
                return false;
            bool directory_added = false;
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.RestoreDirectory = true;
            cd.IsFolderPicker = true;
            cd.Multiselect = true;

            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string s in cd.FileNames)
                {
                    if (!MusicDirectories.Contains(s))
                    {
                        MusicDirectories.Add(s);
                        directory_added = true;
                    }
                }
            }
            return directory_added;
        }
        //performace critical loop ahead
        //album and artist lists should be stored in the trackinfo class
        public void BuildID3Tags()
        {
            foreach (KeyValuePair<string, string> f in MusicFiles)
            {
                string ext = Path.GetExtension(f.Key);
                if (tool.IsAudioFile(f.Key))
                    musicInfo.GetInfo(f.Key);
            }
            return;
        }

        public Task BuildID3TagsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                BuildID3Tags();
            });
        }
        //get directories saved as properties
        public void GetSavedDirectories()
        {
            foreach (string s in Properties.Settings.Default.MusicDirectories)
            {
                tool.debug(s);
                MusicDirectories.Add(s);
            }
        }
        
        public void AddMusicFiles(string[] list)
        {
            foreach (string f in list)
            {
                if (!MusicFiles.ContainsKey(f))
                    MusicFiles.Add(f, Path.GetFileNameWithoutExtension(f));
            }
        }

        public void AddMusicFile(string path)
        {
            if (!MusicFiles.ContainsKey(path))
                MusicFiles.Add(path, Path.GetFileNameWithoutExtension(path));
        }
        
        public async void LoadID3Library()
        {
            Console.WriteLine("Reading ID3 tags...");
            Console.WriteLine(string.Concat("Total Music Files: ", MusicFiles.Count));

            tagsLoaded = false;
            if (musicInfo.trackInfos.Count == 0)
                await BuildID3TagsAsync();

            tagsLoaded = true;
            tool.SetConsoleErrorState();
            foreach (string s in Song.TagLoadingLog)
                Console.WriteLine(string.Concat(s,"\n"));

            tool.ResetConsoleColor();
            tool.debug("...Finished Loading tags:  ", "Tag count: " + musicInfo.trackInfos.Count());
        }

        public void AddMusicDirectory(string directory)
        {
            if (!MusicDirectories.Contains(directory))
                MusicDirectories.Add(directory);
        }

        //need to revisit this functionality
        public void FindDefaultMusicDirectories()
        {
            string musicFolder = tool.SearchDesktopFolder("Music");
            string _user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string _gdrive = "google drive";
            string googleDriveFolder = tool.SearchDirectoryForFolder(_user, _gdrive);
            // string playlistFolder = tool.SearchDirectoryForFolder(googleDriveFolder, "playlists");
            //string playlistFolder2 = tool.SearchDirectoryForFolder(googleDriveFolder, Properties.Settings.Default.tt);
            AddMusicDirectory(musicFolder);
            AddMusicDirectory(googleDriveFolder);
        }

        public bool HasLoadedTags()
        {
            if (!tagsLoaded)
                tool.show(2, "Info tags have not finished loading.  Check the console");
            return tagsLoaded;
        }


        public Playlist GetPlaylist(string path, bool readfile)
        {
            if (Playlists.ContainsKey(path))
                return Playlists[path];
            else
            {
                Playlist p = new Playlist(path, readfile);
                Playlists.Add(path, p);
                return p;
            }
        }

        public void RemovePlaylist(Playlist playlist)
        {
            Playlists.Remove(playlist.name);
        }
        
        public void AddPlaylistDirectory(string directory)
        {
            if (!PlaylistDirectories.Contains(directory))
                PlaylistDirectories.Add(directory);
        }

        public void SelectPlayListDirectories()
        {
            List<string> dirs = tool.BrowseForDirectories(true, true);
            foreach (string s in dirs)
            {
                if (!PlaylistDirectories.Contains(s))
                {
                    PlaylistDirectories.Add(s);
                }
            }
        }

        public void SetMusicDirectory()
        {
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.RestoreDirectory = true;
            cd.IsFolderPicker = true;
            cd.Multiselect = true;

            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string s in cd.FileNames)
                {
                    if (!MusicDirectories.Contains(s))
                        MusicDirectories.Add(s);
                }
                tool.Show("reload directory tree after selecting folder");
            }
        }
    }
}
