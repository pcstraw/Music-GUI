using Glaxion.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using item_ = System.Windows.Forms.ListViewItem;

//http://joyfulwpf.blogspot.co.uk/2012/06/drag-and-drop-files-from-wpf-to-desktop.html
//https://github.com/avuserow/ffmpeg-replaygain/blob/master/gain_analysis.c

namespace Glaxion.Music
{
    public partial class MusicControlGUI : Form
    {
        public MusicControlGUI()
        {
            InitializeComponent();
        }

        public MusicControlGUI(string[] args)
        {
            InitializeComponent();
            startArgs = args;
        }

        public MusicControl musicControl;
        public List<PlaylistPanel> dockedTrackManagers;
        public string[] startArgs;
        public Panel musicPanel;
        
        private void MusicControlGUI_Load(object sender, EventArgs e)
        {
            if (startArgs == null)
            {
                musicControl = new MusicControl();
                MusicPlayer.WinFormApp = this;
            }
            else
            {
                //TODO:  should check what files startArgs is
                musicControl = new MusicControl(startArgs);
                MusicPlayer.WinFormApp = this;
            }
            musicControl.LoadMusicControl();

            //add playlist files to split container
            Panel p = musicControl.playlistFileManager.GetPanel();
            musicControl.playlistFileManager.Dock = DockStyle.Fill;
            // AddToSplitContainer(fileContainer.Panel1, p);
            musicControl.playlistFileManager.ContextMenuStrip = playlistFileContext;
            //musicControl.playlistFileManager.UpdateBackColor();

            //add playlist manager to split container
            AddToSplitContainer(playlistControlContainer.Panel1, musicControl.playlistManager);
            musicControl.playlistManager.ContextMenuStrip = playlistContext;
            musicControl.playlistManager.BringToFront();

            //add the file manager to split container
           // file_panel.Controls.Add(musicControl.musicFileManager);
            musicControl.musicFileManager.ContextMenuStrip = musicFileContext;
            
            //hack to get the desired colour
            //in future make the playlist and music file manager with similar toned backgrounds
            musicControl.musicFileManager.BackColor = Color.Black;
            musicControl.musicFileManager.ForeColor = Color.Yellow;
            musicControl.musicFileManager.Dock = DockStyle.Fill;
           // squashBoxControl1.Swap();
            /*
            foreach (Control c in Controls)
                c.DoubleBuffering(true);

            this.DoubleBuffering(true);
            */
            MusicPlayer.Player.PlayEvent += MusicPlayer_PlayEvent;
            
            //restored that last state of the window, (normal or maximized)
            SetWindowState();
            tool.musicEditingProgram = Properties.Settings.Default.VegasPath;
            tool.HideConsole();

            musicControl.trackContext = trackContext;
            dockedTrackManagers = musicControl.dockedTrackManagers;
            AdjustAllSplitters(8, this.Controls);
            // musicPanel = playlistContainer.Panel1;
            musicPanel = fileContainer.Panel2;

            squashBoxControl1.MakeBackPanel(musicControl.musicFileManager);
            squashBoxControl1.MakeFrontPanel(musicControl.playlistFileManager);
            
            /*
            typeof(Panel).InvokeMember("DoubleBuffered",
    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
    null, songControl1.titleLabel, new object[] { true });
    */
            //link the picture main info panel to the background panel image
            // backgroundPanel.BackgroundImageLayout = ImageLayout.Zoom;
            //backgroundPanel.BackColor = Color.Black;
            //mainTrackInfo.exteneralPanel = backgroundPanel;
            //Tests.RunTests();
            //TODO:  remember to load the starting playlist into the music player
            //TODO:  here is where we add the starting playlist as the docked playlists

        }

        public void SetWindowState()
        {
            int state = Properties.Settings.Default.Maximised;
            if(state == 2)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }

        public void EnableBuffering(object thing)
        {
           typeof(Control).InvokeMember("DoubleBuffered",
           BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
           null, thing, new object[] { true });
        }
        
        //set the windows form title to the current tracks path
        private void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            
        }

        //recursively grab all splitContainers and set these values
        public void AdjustAllSplitters(int rate,Control.ControlCollection collection)
        {
            foreach (Control c in collection)
            {
                if (c is SplitContainer)
                    ExpandHandles(c as SplitContainer, rate);
                    
                AdjustAllSplitters(rate,  c.Controls);
            }
        }

        public void DockPlaylist(Panel controlPanel, Playlist p)
        {
            musicControl.CreatePlaylistPanel(controlPanel, p);
        }
        
        public void ExpandHandles(SplitContainer sc, int rate)
        {
            sc.SplitterWidth = rate;
            //sc.BackColor = col;
        }

        public void AddToSplitContainer(SplitterPanel sc, Panel p)
        {
            p.Dock = DockStyle.Fill;
            p.BackColor = sc.BackColor;
            p.ForeColor = sc.ForeColor;
            sc.Controls.Add(p);
        }

        public void AddToSplitContainer(SplitterPanel sc, Control p)
        {
            p.Dock = DockStyle.Fill;
            p.BackColor = sc.BackColor;
            p.ForeColor = sc.ForeColor;
            sc.Controls.Add(p);
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.OpenSelectedNodeDirectory();
        }

        private void addDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.fileLoader.BrowseMusicDirectory();
        }

        private void editDirectroriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.fileLoader.EditMusicDirectories();
        }

        private void MusicControlGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(tool.musicEditingProgram))
            {
                Properties.Settings.Default.VegasPath = tool.musicEditingProgram;
                Properties.Settings.Default.Save();
            }
            SaveMaximisedState();
            musicControl.Save();
        }

        private void SaveMaximisedState()
        {
            if (WindowState == FormWindowState.Maximized)
                Properties.Settings.Default.Maximised = 2;
            else
                Properties.Settings.Default.Maximised = 0;
        }

        private void addDirectoryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.fileLoader.SelectPlayListDirectories();
            musicControl.playlistFileManager.LoadPlaylistDirectories();
        }

        private void editDirectoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.fileLoader.EditPlaylistDirectories();
            musicControl.playlistFileManager.LoadPlaylistDirectories();
        }
        
        private void playlistContext_removeItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.RemoveSelectedPlaylists();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.SaveAll();
        }
        
        private void playlistContext_deleteItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.DeleteSelectedPlaylists(true);
        }

        private void trackContext_remove_button_Click(object sender, EventArgs e)
        {
            musicControl.RemoveSelectedTracks();
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.AddItemFromPlaylist(new Playlist("output", false));
        }

        private void saveTmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.SaveTmp();
        }
        
        private void folderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(tool.StringCheck(musicControl.trackManager.hoveredItem.Tag as string))
                tool.OpenFileDirectory(musicControl.trackManager.hoveredItem.Tag as string);
        }

        private void musicfileContext_folder_button_Click(object sender, EventArgs e)
        {
            if (tool.StringCheck(musicControl.musicFileManager.selectedNode.Tag as string))
                tool.OpenFileDirectory(musicControl.musicFileManager.selectedNode.Tag as string);
        }

        private void folderToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.OpenHoveredPlaylistDirectory();
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            musicControl.FindTrackInMusicFiles();
        }

        private void trackContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            if(trackContext_search_textBox.Text.Length > 0)
                musicControl.trackManager.SearchFor(trackContext_search_textBox.Text, musicControl.trackManager.SearchColor,Color.White);
            else
                musicControl.trackManager.ResetAllBackColor(musicControl.trackManager.SearchColor);
        }
      
        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                musicControl.playlistManager.ChangeItemName(musicControl.playlistManager.hoveredItem, toolStripTextBox2.TextBox.Text);
                playlistContext.Close();
            }
        }

        //music control search context text up
        private void playlistfileContext_search_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                musicControl.playlistFileManager.SearchForText(playlistFileContact_search_textBox.TextBox.Text);
            }
        }

        private void playlistfileContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            if(playlistFileContact_search_textBox.TextBox.Text.Length < 1)
            {
                musicControl.playlistFileManager.SearchForText(null);
            }else
            {
                musicControl.playlistFileManager.SearchForText(playlistFileContact_search_textBox.TextBox.Text);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> pl = Playlist.SelectPlaylistFile();
            foreach(string s in pl)
            {
                Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(s, true);
                musicControl.playlistManager.AddItemFromPlaylist(p);
            }
        }

        private void musicFileContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            if (musicFileContext_search_textBox.TextBox.Text.Length < 2)
            {
                 musicControl.musicFileManager.SearchForText(null);
                //musicControl.musicFileManager.PopulateTree(musicControl.musicFileManager.cachedNodes);
                //restore last opened nodes?
                
            }
            else
            {
                musicControl.musicFileManager.SearchForText(musicFileContext_search_textBox.TextBox.Text);
            }
        }
        

        private void disableBufferingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Not sure why we want to disable double buffering.  Should probably delete this");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.DoubleBuffering(false);
            musicControl.trackManager.OwnerDraw = false;
        }

        bool db = true;
        bool od = true;
        private void doubleBufferingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            db = !db;
            tool.Show("Looks like we're changing the buffering state for some reason");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.DoubleBuffering(db);
        }

        private void ownerDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            od = !od;
            tool.Show("Looks like we're changing the state of owner draw");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.OwnerDraw = od;
        }

        private void sendToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.MoveSelectedToBottom();
        }

        private void checkDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.trackManager.CheckForRepeat();
        }

        //for tracklist manager.  Once a context menu is added to the track manager internally, move this code to track manager
        public void BookMarkItems()
        {
           // tool.Show("Need to use the current track manager");
           // System.Diagnostics.Debugger.Break();
            
            foreach (ListViewItem i in musicControl.trackManager.SelectedItems)
            {
                ToolStripMenuItem t = new ToolStripMenuItem(i.SubItems[0].Text);
                t.Tag = i.Index;
                i.ForeColor = Color.Red;
                i.Selected = false;
                trackContext_move_button.DropDownItems.Add(t);
            }
        }

        public void MoveToBookMark(int bookmarkIndex)
        {
            //tool.Show("Need to use the current track manager");
           // System.Diagnostics.Debugger.Break();
            if (musicControl.trackManager.SelectedItems.Count == 0)
            {
                if (musicControl.trackManager.hoveredItem != null)
                    musicControl.trackManager.MoveItemTo(bookmarkIndex, musicControl.trackManager.hoveredItem);
            }
            else
            {
                foreach (ListViewItem i in musicControl.trackManager.SelectedItems)
                {
                    musicControl.trackManager.MoveItemTo(bookmarkIndex, i);
                }
            }
        }

        private void bookmarkContextButton_Click(object sender, EventArgs e)
        {
            BookMarkItems();
        }

        private void moveContextButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MoveToBookMark((int)e.ClickedItem.Tag);
        }

        private void copyContextMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tool.Show("Delete me");
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.LoadPlaylistDirectories();
        }

        private void findPlaylistDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.FindPlaylistDirectory();
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadDirectoriesToTree();
        }

        private void saveCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistManager.SaveChecked();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.show(1000,"Remove function");
            Debugger.Break();
          //  musicControl.playlistManager.SaveAsHoveredPlaylist();
        }

        public void SendTracksToCheckedPlaylists()
        {
            List<string> sl = new List<string>();
            foreach (ListViewItem i in musicControl.CurrentPlaylistPanel.playlistView.SelectedItems)
            {
                string s = i.SubItems[1].Text;
                sl.Add(s);
                //  musicControl.playlistManager.SendToCheckedPlaylists(s);
            }
            //sl.Reverse();
            
            foreach(string s in sl)
            {
                tool.Show("sl count: " + sl.Count);
                musicControl.playlistManager.SendToCheckedPlaylists(s);
            }
           // sl = null;
            musicControl.UpdateDockedPlaylistTracks();
        }

        private void sendToCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Delete me");
        }

        public void QuickSaveTrackManager()
        {
            musicControl.trackManager.CurrentList.Save();
        }

        //save to the global playlist directory
        public void DefaultSaveTrackManager()
        {
            musicControl.trackManager.CurrentList.SetDefaultPath();
            musicControl.trackManager.CurrentList.Save();
        }

        private void quickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSaveTrackManager();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaylistPanel pm = musicControl.CurrentPlaylistPanel;

            pm.UpdateTracks();
            bool saved = pm.CurrentList.SaveAs();
            if (saved)
            {
                //tm.SetTitle(tm.currentList.name);
                //update track manager container name here
                ListViewItem item = musicControl.playlistManager.FindItemByPath(pm.CurrentList.path);
                if (item == null)
                {
                    pm.UpdatePlaylistTitle();
                    musicControl.playlistManager.AddItemFromPlaylist(pm.CurrentList);
                    musicControl.playlistFileManager.LoadPlaylistDirectories();
                }else
                {
                    pm.CurrentList.ReadFile();
                }
            }
        }
        
        private void updateMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.UpdateMusicPlayer();
        }

        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                trackContext.Close();
            }
        }

        private void playlistContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            playlistContext.Close();
        }

        private void playlistfileContext_reload_button_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.LoadPlaylistDirectories();
        }

        private void reloadToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadDirectoriesToTree();
        }
        
        private void moveContextButton_Click(object sender, EventArgs e)
        {
           //tool.Show("Need to use the current track manager");
           //System.Diagnostics.Debugger.Break();
           musicControl.trackManager.MoveSelectedTracksToRightClickedItem();
        }

        private void MusicControlGUI_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                TotalClipboard.Files.Clear();
                TotalClipboard.CopyFile(files.ToList());
                
                /*
                try
                {
                    e.Data.SetData(null);
                }
                catch { }
                */
            }
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.RestoreLastState();
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.RestoreNextState();
            //for undo/redo
            redoStateIndex.Text = musicControl.trackManager.stateindex.ToString();
        }
        
        private void goToEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.EnsureVisible(musicControl.trackManager.Items.Count - 1);
        }

        private void sendToCheckedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.MoveSelectedToTop();
        }

        private void playlistContext_newItem_Click(object sender, EventArgs e)
        {
            DockNewPlaylist(this.musicPanel);
        }

        private void dockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            musicControl.CreatePlaylistPanel(this, musicControl.trackManager.CurrentList);
        }

        private void mainFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Delete me");
            System.Diagnostics.Debugger.Break();
            return;
        }

        public void DockPlaylistItem(ListViewItem item,Control control)
        {
            Playlist p = item.Tag as Playlist;
            if (p != null)
                musicControl.CreatePlaylistPanel(control, p);
        }

        public void DockPlaylistItems(Control control)
        {
            if (musicControl.playlistManager.SelectedItems.Count < 1)
                DockPlaylistItem(musicControl.playlistManager.hoveredItem, control);
            else
            {
                foreach (ListViewItem i in musicControl.playlistManager.SelectedItems)
                {
                    DockPlaylistItem(i, control);
                }
            }
        }

        private void playlistContext_newItem_inTrackItem_Click(object sender, EventArgs e)
        {
            DockNewPlaylist(playlistControlContainer.Panel2);
        }
        

        private void musicfileContext_play_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.PlaySelectedNode(musicControl.musicFileManager.selectedNode);
        }

        public void SendFilePathToPlaylist(string path)
        {
            foreach (item_ i in musicControl.playlistManager.SelectedItems)
            {
                Playlist p = i.Tag as Playlist;
                if (p != null)
                {
                    p.tracks.Insert(0, path);
                }
            }
        }

        private void playingToSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendFilePathToPlaylist(MusicPlayer.Player.currentTrackString);
        }

        public void SendSelectedTracksToPlaylists()
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            
            for (int i = musicControl.trackManager.SelectedItems.Count - 1; i > -1; i--)
            {
                SendFilePathToPlaylist(musicControl.trackManager.SelectedItems[i].SubItems[1].Text);
            }
        }

        private void selectedTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendSelectedTracksToPlaylists();
        }

        private void sendToPlaylistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendSelectedTracksToPlaylists();
            musicControl.UpdateDockedPlaylistNames();
        }
        
        //log all the docked playlists in the context menu
        public void BookmarkDockedPlaylists(ToolStripMenuItem tsm)
        {
            tsm.DropDownItems.Clear();
            foreach (PlaylistPanel tm in musicControl.dockedTrackManagers)
            {
                ToolStripMenuItem t = new ToolStripMenuItem(tm.CurrentList.name);
                t.Tag = tm;
                tsm.DropDownItems.Add(t);
            }
        }

        private void trackContext_Opening(object sender, CancelEventArgs e)
        {
            foreach(ListViewItem i in musicControl.trackManager.preContextSelection)
                i.BackColor = Color.LightSkyBlue;
            BookmarkDockedPlaylists(sendToDockedContext);
        }

        private void selectedPlaylistsContext_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
        
            Playlist p = e.ClickedItem.Tag as Playlist;
            if (p != null)
            {
                for (int i = musicControl.trackManager.SelectedItems.Count - 1; i > -1; i--)
                {
                    p.tracks.Insert(0, musicControl.trackManager.SelectedItems[i].Text);
                }
            }
            musicControl.UpdateDockedPlaylistNames();
        }

        private void selectedPlaylistsContext_Click(object sender, EventArgs e)
        {
            //SendSelectedTracksToPlaylists();
        }

        private void sendToDockedContext_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            LoadIntoDocked(e);
        }

        public void LoadIntoDocked(ToolStripItemClickedEventArgs e)
        {
            //tool.Show("Need to use the current track manager");
            //System.Diagnostics.Debugger.Break();
            
            PlaylistPanel tm = e.ClickedItem.Tag as PlaylistPanel;
            if (tm != null && tm != musicControl.CurrentPlaylistPanel)
            {
                for (int i = musicControl.trackManager.SelectedItems.Count - 1; i > -1; i--)
                {
                    tm.CurrentList.tracks.Insert(0, musicControl.trackManager.SelectedItems[i].Text);
                }
                throw new NotImplementedException("update panel loading");
                //tm.playlistView.LoadIntoView(tm.playlistView.currentList);
            }
        }

        public void LoadIntoDocked(ToolStripItemClickedEventArgs e,List<string> list)
        {
            PlaylistView tm = e.ClickedItem.Tag as PlaylistView;
            if (tm != null)
            {
                for (int i = list.Count - 1; i > -1; i--)
                {
                    tm.CurrentList.tracks.Insert(0, list[i]);
                }
                throw new NotImplementedException("update panel loading");
                //tm.LoadIntoView(tm.currentList);
            }
        }
        
        private void byFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.Show("Delete me");
            System.Diagnostics.Debugger.Break();
            return;
        }

        private void getToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tool.Show("Delete me");
            System.Diagnostics.Debugger.Break();
            return;
        }
        
        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.MoveSelectedToTop();
            return;
        }

        private void checkedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendTracksToCheckedPlaylists();
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.MoveSelectedToBottom();
        }

        public void DockNewPlaylist(Control control)
        {
            musicControl.CreatePlaylistPanel(control, new Music.Playlist("new",false));
        }

        private void newToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DockNewPlaylist(this);
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DockNewPlaylist(musicControl.trackManager);
        }

        private void fullSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DefaultSaveTrackManager();
        }

        private void fullSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            QuickSaveTrackManager();
            DefaultSaveTrackManager();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(PlaylistPanel tm in musicControl.dockedTrackManagers)
            {
                tm.CurrentList.Save();
            }
        }
        
        public void UpdateDefaultPlaylistPaths()
        {
            foreach (item_ i in musicControl.playlistManager.SelectedItems)
            {
                String name = i.SubItems[0].Text;
                Playlist p = i.Tag as Playlist;
                if (p == null)
                    continue;

                TreeNode tn = musicControl.playlistFileManager.FindFileByName(name, musicControl.playlistFileManager.Nodes);
                if (tn == null)
                    continue;

                p.path = tn.Tag as string;
                i.SubItems[1].Text = p.path;
            }
        }

        private void findDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateDefaultPlaylistPaths();
        }

        private void musicfileContext_view_files_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadFileNodesToView();
        }

        private void musicfileContext_view_album_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadAlbumNodesToView();
        }

        private void musicfileContext_view_artists_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadArtistNodesToView();
        }

        private void yearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadYearNodesToView();
        }

        private void reloadToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            musicControl.musicFileManager.LoadDirectoriesToTree();
        }

        private void musicfileContext_dock_button_Click(object sender, EventArgs e)
        {
            musicControl.DockSelectedMusicFiles();
        }

        //Get playlists when the music file context opens
        private void musicFileContext_Opening(object sender, CancelEventArgs e)
        {
            BookmarkDockedPlaylists(musicFilContext_send_button);
            if (musicControl.musicFileManager.SelectedNodes.Count == 1)
            {
                if (musicControl.musicFileManager.hoveredNode != null)
                {
                    musicControl.musicFileManager.SelectedNodes.Clear();
                    musicControl.musicFileManager.hoveredNode.BackColor = Color.DarkBlue;
                    musicControl.musicFileManager.SelectedNodes.Add(musicControl.musicFileManager.hoveredNode);
                }
            }
        }
        
        private void musicFilContext_send_button_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            PlaylistPanel tm = e.ClickedItem.Tag as PlaylistPanel;
            if (tm == null)
                return;
            
            foreach(TreeNode n in musicControl.musicFileManager.SelectedNodes)
            {
                List<string> list = tool.GetAllAudioFiles(n);
                foreach (string s in list)
                    tm.CurrentList.tracks.Insert(0,s);
            }
            // tm.playlistView.LoadIntoView(tm.playlistView.currentList);
            throw new NotImplementedException("update panel loading");
            //  tm.trackManager.EnsureVisible(0);
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.InvertTrackManagerSelection();
        }
        
        public void SetDefaultDirectory()
        {
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            string initDir = Properties.Settings.Default.DefaultPlaylistDirectory;

            if (Directory.Exists(initDir))
                cd.InitialDirectory = initDir;
            else
                cd.RestoreDirectory = true;

            cd.IsFolderPicker = true;
            cd.Multiselect = false;

            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string output = cd.FileName;
                Properties.Settings.Default.DefaultPlaylistDirectory = output;
                MusicPlayer.Player.fileLoader.AddPlaylistDirectory(output);
                tool.Show("Set default playlist directory to: ",output);
                musicControl.playlistFileManager.LoadPlaylistDirectories();
            }
        }

        private void setDefaultDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDefaultDirectory();
        }
        
        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.FilterByFolder();
        }

        private void playlistContext_newItem_inAppItem_Click(object sender, EventArgs e)
        {
            DockNewPlaylist(this);
        }

        private void playlistContext_dockItem_inAppItem_Click(object sender, EventArgs e)
        {
            DockPlaylistItems(this);
        }

        private void playlistContext_dockItem_Click(object sender, EventArgs e)
        {
            DockPlaylistItems(musicPanel);
        }

        private void playlistContext_dockItem_inTrackItem_Click(object sender, EventArgs e)
        {
            DockPlaylistItems(musicPanel);
        }

        private void trackContext_search_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                musicControl.trackContext.Close();
        }

        private void renameContextTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                musicControl.playlistManager.ChangeItemName(musicControl.playlistManager.hoveredItem, renameContextTextBox.TextBox.Text);
               //playlistContext.Close();
            }
        }
        
        private void playlistfileContext_folder_button_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.OpenSelectedNodeDirectory();
        }

        private void playlistfileContext_delete_button_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileManager.DeleteSelectedFiles(true);
        }

        private void playlistfileContext_openMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.LoadPlayfilesintoManager();
        }

        private void getToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.show(10000, "implement me");
            Debugger.Break();
        }

        private void findInTreeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.FindPlaylistInFileManager();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = musicControl.playlistManager.hoveredItem;
            musicControl.playlistManager.LabelEdit = true;
            item.BeginEdit();
        }

        private void trackContext_goToBottom_button_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.EnsureVisible(musicControl.trackManager.Items.Count - 1);
        }

        private void trackContext_goToTop_button_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.playlistView.EnsureVisible(0);
        }

        private void trackContext_play_button_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.PlayHoveredItem();
        }

        private void trackContext_vegas_button_Click(object sender, EventArgs e)
        {
            musicControl.OpenTracksInVegas();
        }

        private void iD3TagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = musicControl.CurrentPlaylistPanel.playlistView.hoveredItem;
            if (item == null)
                return;

            SongControl sc = SongControl.CreateTagEditor(item.Tag as string,this);
            sc.squashBoxControl1.Swap();
        }
        
        private void trackContext_filter_artist_button_Click(object sender, EventArgs e)
        {
            musicControl.FilterByArtist();
        }

        private void trackContext_filter_album_button_Click(object sender, EventArgs e)
        {
            musicControl.FilterByAlbum();
        }

        

        private void toolStripDropDownButton1_MouseEnter(object sender, EventArgs e)
        {
            
        }

       

        private void toolStripDropDownButton2_MouseEnter(object sender, EventArgs e)
        {
            
        }
        
        private void removeFolderArtToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TreeNode n = musicControl.musicFileManager.selectedNode;
            if (n == null)
                return;
            musicControl.musicFileManager.GetNodesFolderJPG(n);
        }

        private void editTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = musicControl.musicFileManager.hoveredNode;
            if (node == null)
                return;
            SongControl.CreateTagEditor(node.Tag as string,this);
        }

        private void MusicControlGUI_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void MusicControlGUI_MouseUp(object sender, MouseEventArgs e)
        {
           // string[] files = new string[] { @"c:\temp\test.txt" };
           // this.DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.Copy);
        }
        
        private void MusicControlGUI_DragLeave(object sender, EventArgs e)
        {
            
            this.DoDragDrop(new DataObject(DataFormats.FileDrop, TotalClipboard.Files.ToArray()),
                    DragDropEffects.Copy);
            TotalClipboard.Files.Clear();
        }
        
        private void mainTrackInfo_Load(object sender, EventArgs e)
        {
            
        }

        private void Player_BeforePlayEvent(object sender, EventArgs args)
        {
            string track = sender as string;
            if (!tool.StringCheck(sender as string))
                return;

            Text = track;
           // tool.show(5, "Show main track info");
            songControl1.SetSong(track);
        }

        //nicer way of converting from TreeNode Collection to Array
        //https://stackoverflow.com/questions/29593590/quick-way-to-convert-a-collection-to-array-or-list?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        private void reorganiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList nodes = musicControl.musicFileManager.SelectedNodes;
            if (nodes.Count == 0)
            {
                TreeNode t = musicControl.musicFileManager.hoveredNode;
                if (t == null)
                    return;
                nodes.Add(t);
            }

            ReorganiserControl rc = new ReorganiserControl();
            rc.SetNodes(nodes.Cast<TreeNode>().ToArray());
            rc.SetFiles();
            if (rc.failed)
                return;
            Form f = new Form();
            f.Size = rc.Size;
            rc.Dock = DockStyle.Fill;
            f.Controls.Add(rc);
            f.Show();
            f.Owner = this;
            f.Icon = this.Icon;
        }
        
        private void openTempDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.OpenTempFolder();
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.ToggleConsole();
        }

        bool musicFilesFront = false;

        private void squashBoxControl1_OnSwapEvent(object sender, EventArgs args)
        {
            SetFileControlState();

        }

        private void fileViewLabel_Click(object sender, EventArgs e)
        {
            squashBoxControl1.Swap();
        }

        private void SetFileControlState()
        {
            musicFilesFront = !musicFilesFront;
            if (!musicFilesFront)
                fileViewLabel.Text = "See Music Files";
            else
                fileViewLabel.Text = "See Playlist Files";
        }

        private void squashBoxControl1_TopPanel_Click_1(object sender, EventArgs e)
        {
            
        }

        private void songControl1_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Player != null)
            {
                MusicPlayer.Player.BeforePlayEvent += Player_BeforePlayEvent;
                songControl1.squashBoxControl1.splitContainer.SplitterDistance = 0;
                // songControl1.squashBoxControl1.Swap();
            }
        }

        private void squashBoxControl1_Load(object sender, EventArgs e)
        {
            squashBoxControl1.MainSplitContainer.SplitterDistance = 0;
        }

        private void openAlbumArtFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PicturePanel.OpenAlbumArtFolder();
        }

        private void iD3TagToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach(TreeNode n in musicControl.musicFileManager.SelectedNodes)
            {
                string s = n.Tag as string;
                SongControl.CreateTagEditor(s,this);
            }

            
        }

        public void LoadFont()
        {
            FontDialog fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = songControl1.Font;

            fontDialog1.Color = songControl1.ForeColor;
            if(fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                songControl1.Font = fontDialog1.Font;
                songControl1.ForeColor = fontDialog1.Color;
            }
        }

        public Task LoadFontAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                LoadFont();
            });
        }

        private void setSongInfoFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFont();
        }
        
        private void MusicControlGUI_KeyUp(object sender, KeyEventArgs e)
        {
            MusicPlayer.Player.UseMediaKeys(e.KeyCode);
        }
    }
}
