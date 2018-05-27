using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Glaxion.Music;
//using System.Windows.Forms;

namespace Glaxion.Music
{
    public interface IPlaylistManager
    {
        //update the list of playlists from the UI
        //void UpdateList();
        //update the selected item from the UI
        //void GetSelectedItems();
        //Update UI item colors based on playlist play state
        void RefreshUI();
    }

    public class PlaylistManager : VListView
    {
        public PlaylistManager(IPlaylistManager PlaylistInterface,IListView listViewInterface,ColorScheme colors) : base(listViewInterface)
        {
            _playlist_view = PlaylistInterface;
            CurrentColorScheme = colors;
            //SetListViewInterface(listViewInterface);
        }

        public Dictionary<string, string> tmpPlaylists = new Dictionary<string, string>();
        public ColorScheme ContainsTrackColor = new ColorScheme(Color.Black,Color.SkyBlue);
        public bool ClearTMPFiles = false;
        public Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();
        public ColorScheme CurrentColorScheme;
        IPlaylistManager _playlist_view;

        public VItem FindPlaylistItem(Playlist list)
        {
            foreach (VItem item in Items)
            {
                Playlist p_item = item.Tag as Playlist;
                if (p_item == list)
                    return item;
            }
            return null;
        }

        public VItem FindPlaylistItem(string path)
        {
            foreach (VItem item in Items)
            {
                Playlist p_item = item.Tag as Playlist;
                if (p_item.path == path)
                    return item;
            }
            return null;
        }

        //path to the playlist file
        public VItem AddPlaylistFromPath(int InsertionIndex,string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (Playlists.ContainsKey(path))
            {
                tool.show(1, "", name, "", " already added to the list", "", path);
                VItem item = FindPlaylistItem(path);
                if (item == null)
                    tool.show(1, "", name, "", "...something might be wrong, playlist is in the list but there is no viewBox item", "", path);
                return item;
            }

            Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(path, true);
            VItem i = AddItemFromPlaylist(InsertionIndex,p);
            //not the most optinal place for this
            //_playlist_view.UpdateInterface();
            return i;
        }

        public VItem AddItemFromPlaylist(int index,Playlist p)
        {
            if (!string.IsNullOrEmpty(p.path) && Playlists.ContainsKey(p.path))
            {
                tool.show(1, "", p.name, "", " already contained in list", "", p.path);
                VItem item = FindPlaylistItem(p);
                if (item == null)
                    tool.show(1, "", p.name, "", "...something is wrong, playlist is in list but their is no viewBox item", "", p.path);
                return null;
            }
            
            VItem i = new VItem();
            i.Columns.Insert(0,p.name);
            i.Columns.Insert(1, p.path);
            i.Tag = p;
            i.Name = p.path;
            i.SetColors(CurrentColorScheme);
            Insert(index, i);
           // if (p.path != "")
                Playlists.Add(p.path, p);
            //_playlist_view.UpdateInterface();  //not the most optimal place for this
            return i;
        }

        public Playlist SavePlaylist(string s)
        {
            Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(s, false);
            p.Save();
            return p;
        }

        //should revist the function.  Could be modular
        public void DeleteSelectedPlaylists(bool AllowConfirmation, IEnumerable<int> SelectedIndices)
        {
            //deprecated get selection callback
            //_playlist_view.GetSelectedItems();
            foreach (int i in SelectedIndices)
            {
                Playlist p_item = Items[i].Tag as Playlist;

                if (File.Exists(p_item.path))
                {
                    if (AllowConfirmation)
                    {
                        bool result = tool.askConfirmation(p_item.path);
                        if (!result)
                            continue;
                    }

                    MusicPlayer.Player.fileLoader.RemovePlaylist(p_item);
                    File.Delete(p_item.path);
                    tool.show(2, "File deleted:", p_item.path);
                }
                base.Remove(i);
            }
            //_playlist_view.UpdateInterface();
        }

        public void SaveAll()
        {
            foreach (VItem i in Items)
            {
                Playlist p_item = i.Tag as Playlist;
                p_item.Save();
            }
        }
        /*
        public void SaveChecked()
        {
            foreach (VListItem i in CheckedItems)
            {
                Playlist p = i.Tag as Playlist;
                p.Save();
            }
        }
        */

        public void LoadTmpPlaylists()
        {
            string directory = "Output\\tmp\\";
            if (Directory.Exists(directory)) //does the directory exist?
            {
                List<string> files = tool.LoadFiles(directory, ".m3u"); //if so, grab the files

                //search for corresponding playlist
                foreach (string file in files) //load each file into the listview
                {
                    Playlist tmp = new Playlist(file, true);
                    AddItemFromPlaylist(0,tmp);
                }
            }
        }

        public void LoadManager()
        {
            MusicPlayer.Player.fileLoader.PlaylistDirectories = tool.GetPropertyList(Properties.Settings.Default.PlaylistDirectories);
            LoadTmpPlaylists();
            AssignEventHandlers();
            //HideHorizontalScrollBar();
        }

        public void AssignEventHandlers()
        {
            MusicPlayer.Player.TrackChangeEvent += PlaylistManager_TrackChangeEvent;
            MusicPlayer.Player.PlayEvent += Player_PlayEvent;
        }

        private void Player_PlayEvent(object sender, EventArgs args)
        {
            DisplayTrackInfo();
        }
        
        //recursively check the dictionary until
        //we have generated a new name
        public Playlist AppendPlaylistPath(Playlist p)
        {
            if (Playlists.ContainsKey(p.path))
            {
                string path = p.path;
                string dir = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                name += "+";
                path = string.Concat(dir, @"\", name, ext);
                p.name = name;
                p.path = path;
                AppendPlaylistPath(p);
            }
            else
            {
                return p;
            }
            return p;
        }

        public void DisplayTrackInfo()
        {
            foreach (VItem item in Items)
            {
                string name = item.Columns[0];
                string path = item.Columns[1];
                Playlist p = item.Tag as Playlist;
                item.RestoreColors();
                if (p != null)
                {
                    if (p.tracks.Contains(MusicPlayer.Player.currentTrackString))
                    {
                        //item.OldColor = item.CurrentColor;
                        if (MusicPlayer.Player.playlist.path == p.path)
                            item.HighLightColors(new ColorScheme(Color.Black, Color.Yellow));
                        else
                            item.HighLightColors(ContainsTrackColor);
                        continue;
                    }
                }
            }
            _playlist_view.RefreshUI();
        }

        public void SendToCheckedPlaylists(string file)
        {
            foreach (VItem item in CheckedItems)
            {
                Playlist p = item.Tag as Playlist;
                p.dirty = true;
                p.tracks.Insert(0, file);
            }
        }
       
        private void PlaylistManager_TrackChangeEvent(object sender, EventArgs args)
        {
            DisplayTrackInfo();
        }

        public void SaveTmp()
        {
            string directory = "Output\\tmp\\";
            System.IO.DirectoryInfo di;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            di = new DirectoryInfo(directory);

            foreach (FileInfo file in di.GetFiles())
                file.Delete();

            if (ClearTMPFiles)
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }

            foreach (VItem item in Items)
            {
                Playlist p = item.Tag as Playlist;
                if (p != null)
                {
                    string path = directory + p.name + ".m3u";
                    p.path = path;
                    tool.debug("Save tmp: " + path);
                    p.debugSave = false;
                    p.SaveTo(path);
                }
            }
        }

        public void ChangeItemName(VItem item, string text)
        {
            if (item != null)
            {
                Playlist p = item.Tag as Playlist;
                string oldpath = p.path;
                p.UpdateName(text);
                //item.Text = p.name;
                item.Columns[0] = p.name;
                item.Columns[1] = p.path;
                item.Name = p.path;
                MusicPlayer.WinFormApp.musicControl.UpdateDockedPlaylistNames();
            }
        }
        
        private void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            DisplayTrackInfo();
        }
        
        void RemovePlaylist(VItem item)
        {
            Playlist p = item.Tag as Playlist;
            if (p != null)
            {
                if (Playlists.ContainsKey(p.path))
                    Playlists.Remove(p.path);
            }
            tool.debug(3, "Playlist " + p.name + " removed from Playlist View");
            Remove(item);
        }

        public void RemoveSelectedPlaylists(List<int> selectedIndices)
        {
            //dep
            //_playlist_view.GetSelectedItems();
            // List<int> l = selectedIndices.
            List<VItem> removed_playlists = new List<VItem>();

            foreach (int i in selectedIndices)
                removed_playlists.Add(Items[i]);
            foreach (VItem i in removed_playlists)
            {
                RemovePlaylist(i);
            }
            //_playlist_view.UpdateInterface();
        }

        public void LabelEdit(int Index,string NewName)
        {
            ChangeItemName(Items[Index], NewName);
        }
    }
}
