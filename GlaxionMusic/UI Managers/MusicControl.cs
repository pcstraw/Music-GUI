using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Glaxion.Tools;
using System.IO;
using System.Drawing;
using item_ = System.Windows.Forms.ListViewItem;
using System.Windows.Automation;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Glaxion.Music
{
    public class MusicControl
    {
        private void Construction()
        {
            playlistFileManager = new PlaylistFileManager();
            playlistManager = new PlaylistManager();
            musicFileManager = new MusicFileManager();
        }

        public MusicControl()
        {
            Construction();
        }
        
        public MusicControl(string[] args)
        {
            //need to create a new playlist and dock it
            Construction();

            foreach (string s in args)
            {
                if (tool.IsPlaylistFile(s))
                {
                    playlistManager.AddFileAsItem(s);
                }
            }
        }

        public PlaylistFileManager playlistFileManager;
        public PlaylistManager playlistManager;
        public TrackUserControl trackUser;
        public ContextMenuStrip trackContext;
        public MusicFileManager musicFileManager;
        public List<TrackUserControl> dockedTrackManagers = new List<TrackUserControl>();
       // private Process _vegas_process;

        public TrackManager trackManager
        {
            get
            {
                if (trackUser == null)
                {
                    tool.show(5, "Error:  accessing track manager but no current track user control");
                    return null;
                }
                return trackUser.trackManager;
            }
        }
        
        public void LoadMusicControl()
        {
            //string s = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Music";
            MusicPlayer.Player.GetPlayer();
            playlistFileManager.LoadManager();
            musicFileManager.LoadManager();
            playlistManager.LoadManager();
            AssignEventHanders();
        }
        
        public void RemoveSelectedTracks()
        {
            trackUser.trackManager.RemoveSelectedTracks();
            trackUser.trackManager.UpdateCurrentListTracks();
            trackUser.trackManager.UpdateMusicPlayer();
        }
        
        public void UpdateDockedPlaylistNames()
        {
            foreach (TrackUserControl tm in dockedTrackManagers)
            {
                tm.UpdatePlaylistTitle();
            }
        }

        public void UpdateDockedPlaylistTracks()
        {
            foreach (TrackUserControl tm in dockedTrackManagers)
            {
                if (tm.trackManager.currentList.dirty)
                {
                    tm.trackManager.LoadPlaylistIntoView(tm.trackManager.currentList);
                    tm.trackManager.currentList.dirty = false;
                }
            }
        }

        public TrackUserControl CreatePlaylistPanel(Control control, Playlist p)
        {
            TrackUserControl dp = new TrackUserControl();
            TrackManager tm = dp.trackManager;
            tm.LoadManager();
            tm.BackColor = dp.trackManager.BackColor;
            tm.ForeColor = dp.trackManager.ForeColor;
            tm.LoadIntoView(p);
           // tm.Dock = DockStyle.Top;
           // tm.SendToBack();
            tm.View = View.Details;
            tm.ContextMenuStrip = trackContext;
            tm.AllowDrop = true;
            dp.UpdatePlaylistTitle();
            dp.Dock = DockStyle.Right;
            dp.Show();
            dp.Controls.Add(tm);
            
            Splitter sp = new Splitter();
            sp.Size = new Size(5, sp.Size.Height);
            sp.BackColor = Color.Gray;
            sp.Dock = DockStyle.Right;
            dp.dockSplitter = sp;
            dp.Dock = DockStyle.Right;
            //dp.Controls.Add(tm);
            control.Controls.Add(sp);
            control.Controls.Add(dp);
           // tm.SendToBack();
            //tm.BringToFront();
            tm.Show();
            sp.Show();
            //tm.Invalidate();
            tm.UpdatePlayStateColours();
            return dp;
        }

        private void AssignEventHanders()
        {
            playlistFileManager.DoubleClick += PlaylistFiles_DoubleClick;
            playlistFileManager.DrawNode += PlaylistFileManager_DrawNode;
            playlistFileManager.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            playlistManager.DoubleClick += playlistManager_DoubleClick;
        }
        
        private void TrackManager_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void PlaylistFileManager_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;
            e.Node.ForeColor = Color.DarkRed;
        }
        
        public void LoadPlayfilesintoManager()
        {
            List<Playlist> list = LoadPlaylistFromFileManager();
            if (list.Count > 0)
            {
                playlistManager.SelectedItems.Clear();
                foreach (Playlist p in list)
                {
                    ListViewItem item = playlistManager.AddItemFromPlaylist(p);
                    item.Selected = true;
                    MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, p);
                }
            }
        }

        protected void PlaylistFiles_DoubleClick(object o, EventArgs e)
        {
            TreeNode n = playlistFileManager.MouseSelectNode();
            if (tool.StringCheck(n.Tag as string))
            {
                string path = n.Tag as string;
                Playlist p = new Playlist(path, true);
                CreatePlaylistPanel(MusicPlayer.WinFormApp.musicPanel, p);
                playlistManager.AddItemFromPlaylist(p);
            }
        }
        
        public void FindTrackInMusicFiles()
        {
            string S = trackManager.hoveredItem.Tag as string;
            if (tool.StringCheck(S))
            {
                musicFileManager.SearchForText(null);
                musicFileManager.FindTrack(S);
            }
        }

        private void TrackManager_FindEvent(object sender, EventArgs args)
        {
            FindTrackInMusicFiles();
        }
        
        public void Save()
        {
            if (MusicPlayer.Player != null && !String.IsNullOrEmpty(MusicPlayer.Player.currentTrackString))
            {
                Properties.Settings.Default.LastTrack = MusicPlayer.Player.currentTrackString;
            }
            playlistManager.SaveTmp();
            MusicPlayer.Player.fileLoader.SavePlaylistDirectories();
            MusicPlayer.Player.fileLoader.SaveDirectories();
            Properties.Settings.Default.Save();
        }
        
        public List<Playlist> LoadPlaylistFromFileManager()
        {
            List<Playlist> list = new List<Playlist>();
            if (playlistFileManager.SelectedNodes.Count > 0)
            {
                foreach (TreeNode node in playlistFileManager.SelectedNodes)
                {
                    if (tool.StringCheck(node.Tag as string))
                    {
                        string path = node.Tag as string;
                        Playlist p = new Playlist(path, true);
                        list.Add(p);
                    }
                }
            }
            return list;
        }

        public List<string> LoadPathsFromFileManager()
        {
            List<string> list = new List<string>();
            if (playlistFileManager.SelectedNodes.Count > 0)
            {
                foreach (TreeNode node in playlistFileManager.SelectedNodes)
                {
                    if (tool.StringCheck(node.Tag as string))
                    {
                        string path = node.Tag as string;
                        if(tool.StringCheck(path))
                            list.Add(path);
                    }
                }
            }
            return list;
        }
        
        public void playlistManager_DoubleClick(object o, EventArgs e)
        {
            MusicPlayer.WinFormApp.DockPlaylistItems(MusicPlayer.WinFormApp.musicPanel);
        }

        public void TrackManager_DoubleClick(object o, EventArgs e)
        {
            tool.Show("TODO: current track manager not yet implemented");
            System.Diagnostics.Debugger.Break();
            return;
        }
        
        public void LoadPlaylistHoveredIntoTrackManager()
        {
            if (playlistManager.hoveredItem != null)
            {
                
                Playlist p = playlistManager.hoveredItem.Tag as Playlist;
                if (p != null)
                {
                    trackManager.LoadPlaylistIntoView(p);
                }
            }
        }

        public void LoadPathIntoManager(Playlist p)
        {
            tool.debug("loading playlist file", p.path);
            if(p != null)
            {
                tool.Show("TODO: Dock playlist instead");
                System.Diagnostics.Debugger.Break();
                return;
              //  trackManager.LoadPlaylistIntoView(p);
              //  trackManager.FindMissingFiles();
            }
        }

        public void InvertTrackManagerSelection()
        {
            trackManager.InvertSelection();
        }
        
        internal void FindPlaylistInFileManager()
        {
            Playlist p = playlistManager.hoveredItem.Tag as Playlist;
            if (p != null)
            {
                playlistFileManager.SelectedNodes.Clear();
                TreeNode tn = playlistFileManager.FindFileByPath(p.path,playlistFileManager.Nodes);
                if (tn == null)
                    return;
                tn.Expand();
                playlistFileManager.SelectedNode = tn;
            }
        }

        public void OpenTracksInVegas()
        {
            ListViewItem item = trackUser.trackManager.hoveredItem;
            if (item != null)
                tool.OpenVegas(item.SubItems[1].Text);
            else
            {

                if (trackUser.trackManager.SelectedItems.Count > 0)
                {
                    List<string> list = new List<string>();
                    foreach (ListViewItem i in trackUser.trackManager.SelectedItems)
                    {
                        list.Add(i.SubItems[1].Text);
                    }
                    tool.OpenVegas(list);
                    /*
                    if(_vegas_process != null)
                    {
                        _vegas_process.EnableRaisingEvents = true;
                        _vegas_process.Exited += _vegas_process_Exited;
                    }
                    */
                    return;
                }
            }
        }
        
        public void DockSelectedMusicFiles()
        {
            /*
            List<Playlist> list = LoadPlaylistFromFileManager();
            if (list.Count > 0)
            {
                playlistManager.SelectedItems.Clear();
                foreach (Playlist p in list)
                {
                    ListViewItem item = playlistManager.AddItemFromPlaylist(p);
                    item.Selected = true;
                    MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, p);
                }
            }
            */
            List<string> selectedFiles = new List<string>();
            foreach (TreeNode n in musicFileManager.SelectedNodes)
            {
                string path = n.Tag as string;
                if (string.IsNullOrEmpty(path))
                    continue;
                if (!Path.HasExtension(path))
                {
                    List<string> sl = tool.LoadAudioFiles(path, SearchOption.TopDirectoryOnly);
                    Playlist p = new Playlist(path, false);
                    p.tracks = sl;
                    p.path = "";
                    ListViewItem item = playlistManager.AddItemFromPlaylist(p);
                    MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, p);
                }
                else
                {
                    if(tool.IsAudioFile(path))
                    {
                        selectedFiles.Add(path);
                    }
                }
            }
            if (selectedFiles.Count == 0)
                return;
            Playlist newplaylist = new Playlist(Path.GetFileName(Path.GetDirectoryName(selectedFiles[0])), false);
            newplaylist.path = "";
            newplaylist.tracks = selectedFiles;
            playlistManager.AddItemFromPlaylist(newplaylist);
            MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, newplaylist);

        }

        public void FilterFolder(item_ item)
        {
            if (item == null)
                return;

            string path = item.Tag as string;

            if (!tool.StringCheck(path))
                return;

            string folder = Path.GetDirectoryName(path);

            foreach (item_ i in trackManager.Items)
            {
                string s = i.Tag as string;
                if (!tool.StringCheck(s))
                    continue;

                string d = Path.GetDirectoryName(s);
                if (d == folder)
                    i.Selected = true;
            }
        }

        public void FilterArtist(item_ item)
        {
            if (item == null)
                return;

            string path = item.Tag as string;

            if (!tool.StringCheck(path))
                return;

            Song info = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(path);
            string artist = info.artist;

            foreach (item_ i in trackManager.Items)
            {
                string s = i.Tag as string;
                if (!tool.StringCheck(s))
                    continue;
                Song s_info = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(s);
                string a = s_info.artist;
                if (a == artist)
                    i.Selected = true;
            }
        }

        public void FilterAlbum(item_ item)
        {
            if (item == null)
                return;

            string path = item.Tag as string;

            if (!tool.StringCheck(path))
                return;

            Song info = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(path);
            string album = info.album;

            foreach (item_ i in trackManager.Items)
            {
                string s = i.Tag as string;
                if (!tool.StringCheck(s))
                    continue;
                Song s_info = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(s);
                string a = s_info.album;
                if (a == album)
                    i.Selected = true;
            }
        }

        public void FilterByFolder()
        {
            if (trackManager.hoveredItem == null)
                return;

            if (trackManager.SelectedItems.Count == 0)
            {
                FilterFolder(trackManager.hoveredItem);
            }
            else
            {
                foreach (item_ i in trackManager.SelectedItems)
                {
                    FilterFolder(i);
                }
            }
        }

        public void FilterByArtist()
        {
            if (trackManager.hoveredItem == null)
                return;

            if (trackManager.SelectedItems.Count == 0)
            {
                FilterArtist(trackManager.hoveredItem);
            }
            else
            {
                foreach (item_ i in trackManager.SelectedItems)
                {
                    FilterArtist(i);
                }
            }
        }

        public void FilterByAlbum()
        {
            if (trackManager.hoveredItem == null)
                return;

            if (trackManager.SelectedItems.Count == 0)
            {
                FilterAlbum(trackManager.hoveredItem);
            }
            else
            {
                foreach (item_ i in trackManager.SelectedItems)
                {
                    FilterAlbum(i);
                }
            }
        }

        public void EditSolutionLink()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string path = null;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            Properties.Settings.Default.SolutionFile = path;
        }
    }
}
