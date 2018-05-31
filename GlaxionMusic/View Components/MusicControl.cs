using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Glaxion.Tools;
using System.IO;
using System.Drawing;
using item_ = System.Windows.Forms.ListViewItem;
using Glaxion.Music;
using System.Linq;

namespace Glaxion.Music
{
    public class MusicControl
    {
        private void Construction()
        {
            playlistFileView = new PlaylistFileView();
            playlistView = new PlaylistManagerView();
            musicFileView = new MusicFileView();
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
                    playlistView.manager.InsertPlaylistAt(0,s);
            }
        }

        public PlaylistFileView playlistFileView;
        public PlaylistManagerView playlistView;
        public TracklistPanel CurrentPlaylistPanel;
        public ContextMenuStrip trackContext;
        public MusicFileView musicFileView;
        public List<TracklistPanel> dockedTrackManagers = new List<TracklistPanel>();
       // private Process _vegas_process;

        public TracklistView trackManager
        {
            get
            {
                if (CurrentPlaylistPanel == null)
                {
                    tool.show(5, "Error:  trying to access track manager but the default TrackVeiw Controller is not set");
                    return null;
                }
                return CurrentPlaylistPanel.tracklistView;
            }
        }
        
        public void LoadMusicControl()
        {
            //string s = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Music";
            MusicPlayer.Instance.Start();
            playlistFileView.LoadManager();
            musicFileView.LoadManager();
            playlistView.Load();
            AssignEventHanders();
        }
        
        public void RemoveSelectedTracks()
        {
            CurrentPlaylistPanel.tracklistView.RemoveSelectedTracks();
            //trackManager.RemoveSelectedTracks();
            //CurrentPlaylistPanel.UpdateTracks();
        }
        
        public void UpdateDockedPlaylistNames()
        {
            foreach (TracklistPanel tm in dockedTrackManagers)
            {
                tm.UpdatePlaylistTitle();
            }
        }

        public void UpdateDockedPlaylistTracks()
        {
            foreach (TracklistPanel tm in dockedTrackManagers)
            {
                if (tm.CurrentList.dirty)
                {

                    tm.tracklistView.DisplayPlaylist();
                    tm.CurrentList.dirty = false;
                }
            }
        }

        //used to create a panel playlistPanel embedded in the right of the control
        public TracklistPanel CreatePlaylistPanel(Control control, Playlist p)
        {
            TracklistPanel dp = new TracklistPanel();
            dp.SetPlaylist(p);
            dp.tracklistView.ContextMenuStrip = trackContext;
            dp.Dock = DockStyle.Right;
            dp.Show();
            dp.tracklistView.DisplayPlaylist();
            Splitter sp = new Splitter();
            sp.Size = new Size(5, sp.Size.Height);
            sp.BackColor = Color.Gray;
            sp.Dock = DockStyle.Right;
            dp.dockSplitter = sp;
            dp.Dock = DockStyle.Right;
            control.Controls.Add(sp);
            control.Controls.Add(dp);
            sp.Show();
            return dp;
        }

        private void AssignEventHanders()
        {
            playlistFileView.DoubleClick += PlaylistFiles_DoubleClick;
            playlistFileView.DrawNode += PlaylistFileManager_DrawNode;
            playlistFileView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            playlistView.DoubleClick += playlistManager_DoubleClick;
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
                //playlistView.SelectedItems.Clear();
                foreach (Playlist p in list)
                {
                    VItem item = playlistView.manager.InsertPlaylistAt(0,p);
                    item.Selected = true;
                    MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, p);
                }
            }
        }

        protected void PlaylistFiles_DoubleClick(object o, EventArgs e)
        {
            TreeNode n = playlistFileView.MouseSelectNode();
            if (n == null)
                return;
            if (tool.StringCheck(n.Name))
            {
                string path = n.Name;
                Playlist p = new Playlist(path, true);
                CreatePlaylistPanel(MusicPlayer.WinFormApp.musicPanel, p);
                playlistView.manager.InsertPlaylistAt(0,p);
            }
        }
        
        public void FindTrackInMusicFiles()
        {
            ListViewItem i = trackManager.hoveredItem;
            if(i== null)return;
            string path = i.Name;
            musicFileView.Find(path);
        }

        private void TrackManager_FindEvent(object sender, EventArgs args)
        {
            FindTrackInMusicFiles();
        }
        
        public void Save()
        {
            if (MusicPlayer.Instance != null && !String.IsNullOrEmpty(MusicPlayer.Instance.currentTrackString))
            {
                Properties.Settings.Default.LastTrack = MusicPlayer.Instance.currentTrackString;
            }
            playlistView.manager.SaveTmp();
            FileLoader.Instance.SavePlaylistDirectories();
            FileLoader.Instance.SaveDirectories();
            Properties.Settings.Default.Save();
        }
        
        public List<Playlist> LoadPlaylistFromFileManager()
        {
            List<Playlist> list = new List<Playlist>();
            if (playlistFileView.SelectedNodes.Count > 0)
            {
                foreach (TreeNode node in playlistFileView.SelectedNodes)
                {
                    if (tool.StringCheck(node.Name))
                    {
                        string path = node.Name;
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
            if (playlistFileView.SelectedNodes.Count > 0)
            {
                foreach (TreeNode node in playlistFileView.SelectedNodes)
                {
                    if (tool.StringCheck(node.Name))
                    {
                        string path = node.Name;
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
            if (playlistView.hoveredItem != null)
            {
                Playlist p = playlistView.hoveredItem.Tag as Playlist;
                if (p != null)
                {
                    //trackManager.LoadPlaylistIntoView(p);
                    throw new NotImplementedException("update panel loading");
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
            Playlist p = playlistView.hoveredItem.Tag as Playlist;
            if (p != null)
            {
                playlistFileView.SelectedNodes.Clear();
                TreeNode tn = playlistFileView.FindFileByPath(p.path,playlistFileView.Nodes);
                if (tn == null)
                    return;
                tn.Expand();
                playlistFileView.SelectedNode = tn;
            }
        }

        public void OpenTracksInVegas()
        {
            ListViewItem item = CurrentPlaylistPanel.tracklistView.hoveredItem;
            if (item != null)
                tool.OpenVegas(item.SubItems[1].Text);
            else
            {

                if (CurrentPlaylistPanel.tracklistView.SelectedItems.Count > 0)
                {
                    List<string> list = new List<string>();
                    foreach (ListViewItem i in CurrentPlaylistPanel.tracklistView.SelectedItems)
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
            foreach (TreeNode n in musicFileView.SelectedNodes)
            {
                string path = n.Name;
                if (string.IsNullOrEmpty(path))
                    continue;
                if (!Path.HasExtension(path))
                {
                    List<string> sl = tool.LoadAudioFiles(path, SearchOption.TopDirectoryOnly);
                    Playlist p = new Playlist(path, false);
                    p.tracks = sl;
                    p.path = "";
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
            playlistView.manager.InsertPlaylistAt(0,newplaylist);
            MusicPlayer.WinFormApp.DockPlaylist(MusicPlayer.WinFormApp.musicPanel, newplaylist);

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
