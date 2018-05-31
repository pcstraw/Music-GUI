using Glaxion.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public List<TracklistPanel> dockedTrackManagers;
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
            Panel p = musicControl.playlistFileView.GetPanel();
            musicControl.playlistFileView.Dock = DockStyle.Fill;
            // AddToSplitContainer(fileContainer.Panel1, p);
            musicControl.playlistFileView.ContextMenuStrip = playlistFileContext;
            //musicControl.playlistFileManager.UpdateBackColor();

            //add playlist manager to split container
            // AddToSplitContainer(playlistControlContainer.Panel1, musicControl.playlistManager);
            musicControl.playlistView.Dock = DockStyle.Fill;
            playlistControlContainer.Panel1.Controls.Add(musicControl.playlistView);
            musicControl.playlistView.ContextMenuStrip = playlistContext;
            musicControl.playlistView.BringToFront();

            //add the file manager to split container
           // file_panel.Controls.Add(musicControl.musicFileManager);
            musicControl.musicFileView.ContextMenuStrip = musicFileContext;
            
            //hack to get the desired colour
            //in future make the playlist and music file manager with similar toned backgrounds
            musicControl.musicFileView.BackColor = Color.Black;
            musicControl.musicFileView.ForeColor = Color.Yellow;
            musicControl.musicFileView.Dock = DockStyle.Fill;
           // squashBoxControl1.Swap();
            /*
            foreach (Control c in Controls)
                c.DoubleBuffering(true);

            this.DoubleBuffering(true);
            */
            MusicPlayer.Instance.PlayEvent += MusicPlayer_PlayEvent;
            
            //restored that last state of the window, (normal or maximized)
            SetWindowState();
            tool.musicEditingProgram = Properties.Settings.Default.VegasPath;
            tool.HideConsole();

            musicControl.trackContext = trackContext;
            dockedTrackManagers = musicControl.dockedTrackManagers;
            AdjustAllSplitters(8, this.Controls);
            // musicPanel = playlistContainer.Panel1;
            musicPanel = fileContainer.Panel2;

            fileSquashBox.MakeBackPanel(musicControl.musicFileView);
            fileSquashBox.MakeFrontPanel(musicControl.playlistFileView);
            songControl1.squashBoxControl1.splitContainer.SplitterDistance = 0;
            fileSquashBox.splitContainer.SplitterMoving += fileSplitContainer_SplitterMoving;
            CustomFont.LoadCustomFonts();
            fileViewLabel.Font = CustomFont.Exo.font;

            tool.GlobalForm = this;
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

        private bool switch_label;
        private void fileSplitContainer_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            if (e.SplitY < 5 && !switch_label)
            {
                SetFileBoxLabel();
                switch_label = true;
                return;
            }
            if (e.SplitY > 5 && switch_label)
            {
                switch_label = false;
                SetFileBoxLabel();
            }
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
           // p.BackColor = sc.BackColor;
           // p.ForeColor = sc.ForeColor;
            sc.Controls.Add(p);
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileView.OpenSelectedNodeDirectory();
        }

        private void addDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if(musicControl.musicFileView.manager.fileLoader.BrowseMusicDirectory())
                musicControl.musicFileView.manager.LoadDirectoriesToTree();

        }

        private void editDirectroriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.EditMusicDirectories();
        }

        private void MusicControlGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(tool.musicEditingProgram))
            {
                Properties.Settings.Default.VegasPath = tool.musicEditingProgram;
                Properties.Settings.Default.Save();
            }
            MusicPlayer.Instance.Stop();
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
            FileLoader.Instance.SelectPlayListDirectories();
            musicControl.playlistFileView.manager.LoadPlaylistDirectories();
        }

        private void editDirectoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileView.manager.EditPlaylistDirectories();
        }
        
        private void playlistContext_removeItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistView.RemoveSelectedItems();
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistView.manager.SaveAll();
        }
        
        private void playlistContext_deleteItem_Click(object sender, EventArgs e)
        {
            List<int> lst = musicControl.playlistView.GetSelected().ToList();
            musicControl.playlistView.manager.DeleteSelectedPlaylists(true,lst);
        }

        private void trackContext_remove_button_Click(object sender, EventArgs e)
        {
            musicControl.RemoveSelectedTracks();
        }
        
        private void saveTmpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistView.manager.SaveTmp();
        }
        
        private void folderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(tool.StringCheck(musicControl.trackManager.hoveredItem.Name))
                tool.OpenFileDirectory(musicControl.trackManager.hoveredItem.Name);
        }

        private void musicfileContext_folder_button_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in musicControl.musicFileView.SelectedNodes)
            {
                if (tool.StringCheck(node.Name))
                    tool.OpenFileDirectory(node.Name);
            }
        }

        private void folderToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            musicControl.playlistView.OpenHoveredPlaylistDirectory();
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            musicControl.FindTrackInMusicFiles();
        }

        private void trackContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            musicControl.trackManager.TextSearch(trackContext_search_textBox.Text);
        }
      
        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                musicControl.playlistView.manager.LabelEdit(musicControl.playlistView.hoveredItem.Index, toolStripTextBox2.TextBox.Text);
                playlistContext.Close();
            }
        }

        //music control search context text up
        private void playlistfileContext_search_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                musicControl.playlistFileView.manager.SearchForText(playlistFileContact_search_textBox.TextBox.Text);
            }
        }

        private void playlistfileContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            if(playlistFileContact_search_textBox.TextBox.Text.Length < 1)
            {
                musicControl.playlistFileView.manager.SearchForText(null);
            }else
            {
                musicControl.playlistFileView.manager.SearchForText(playlistFileContact_search_textBox.TextBox.Text);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> pl = Playlist.SelectPlaylistFile();
            foreach(string s in pl)
            {
                Playlist p = FileLoader.Instance.GetPlaylist(s, true);
                musicControl.playlistView.manager.InsertPlaylistAt(0,p);
            }
        }

        private void musicFileContext_search_textBox_TextChanged(object sender, EventArgs e)
        {
            if (musicFileContext_search_textBox.TextBox.Text.Length < 2)
            {
                 musicControl.musicFileView.manager.SearchForText(null);
                //musicControl.musicFileManager.PopulateTree(musicControl.musicFileManager.cachedNodes);
                //restore last opened nodes?
                
            }
            else
            {
                musicControl.musicFileView.manager.SearchForText(musicFileContext_search_textBox.TextBox.Text);
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
        /*
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
        */

        private void bookmarkContextButton_Click(object sender, EventArgs e)
        {
            BookMarkItems();
        }
        /*
        private void moveContextButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //MoveToBookMark((int)e.ClickedItem.Tag);
        }
        */

        private void copyContextMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tool.Show("Delete me");
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileView.LoadPlaylistDirectories();
        }
        
        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadDirectoriesToTree();
        }

        private void saveCheckedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Save check box function not yet implemented");
           // musicControl.playlistManager.manager.SaveChecked();
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
            foreach (ListViewItem i in musicControl.CurrentPlaylistPanel.tracklistView.SelectedItems)
            {
                string s = i.SubItems[1].Text;
                sl.Add(s);
                //  musicControl.playlistManager.SendToCheckedPlaylists(s);
            }
            //sl.Reverse();
            
            foreach(string s in sl)
            {
                tool.Show("sl count: " + sl.Count);
                musicControl.playlistView.manager.SendToCheckedPlaylists(s);
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
            musicControl.trackManager.manager.CurrentList.Save();
        }

        //save to the global playlist directory
        public void DefaultSaveTrackManager()
        {
            musicControl.trackManager.manager.CurrentList.SetDefaultPath();
            musicControl.trackManager.manager.CurrentList.Save();
        }

        private void quickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSaveTrackManager();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TracklistPanel pm = musicControl.CurrentPlaylistPanel;

            pm.tracklistView.manager.UpdateTracks();
            bool saved = pm.CurrentList.SaveAs();
            if (saved)
            {
                //tm.SetTitle(tm.currentList.name);
                //update track manager container name here
                ListViewItem item = musicControl.playlistView.FindItemByPath(pm.CurrentList.path);
                if (item == null)
                {
                    pm.UpdatePlaylistTitle();
                    musicControl.playlistView.manager.InsertPlaylistAt(0,pm.CurrentList);
                    musicControl.playlistFileView.manager.LoadPlaylistDirectories();
                }else
                {
                    pm.CurrentList.ReadFile();
                }
            }
        }
        
        private void updateMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.manager.UpdateMusicPlayer();
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
            musicControl.playlistFileView.manager.LoadPlaylistDirectories();
        }

        private void reloadToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadDirectoriesToTree();
        }
        
        private void moveContextButton_Click(object sender, EventArgs e)
        {
            ListViewItem i = musicControl.trackManager.hoveredItem;
            if (i != null)
            {
                //seems we always have tp reverse the list
                List<int> list = musicControl.trackManager.lastSelectedIndices;
                musicControl.trackManager.manager.MoveIndicesTo(i.Index, list);
                trackContext.Close();
            }
        }

        private void MusicControlGUI_DragEnter(object sender, DragEventArgs e)
        {
            /*
            e.Effect = DragDropEffects.Copy;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                InternalClipboard.Files.Clear();
                InternalClipboard.CopyFile(files.ToList());
                
                try
                {
                    e.Data.SetData(null);
                }
                catch { }
            }
            */
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.trackManager.RestoreLastState();
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            musicControl.trackManager.RestoreNextState();
            //for undo/redo
            //redoStateIndex.Text = musicControl.trackManager.stateindex.ToString();
        }
        
        private void goToEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("call ensure visible on the last item");
            //musicControl.CurrentPlaylistPanel.tracklistView.EnsureVisible(musicControl.trackManager.Items.Count - 1);
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
            musicControl.CreatePlaylistPanel(this, musicControl.trackManager.manager.CurrentList);
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
            {
                musicControl.CreatePlaylistPanel(control, p);
                item.Selected = false;
            }
        }

        public void DockPlaylistItems(Control control)
        {
            if (musicControl.playlistView.SelectedItems.Count < 1)
                DockPlaylistItem(musicControl.playlistView.hoveredItem, control);
            else
            {
                foreach (ListViewItem i in musicControl.playlistView.SelectedItems)
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
            musicControl.musicFileView.manager.PlaySelectedNode(WinFormUtils.GetVNode(musicControl.musicFileView.selectedNode));
        }

        public void SendFilePathToPlaylist(string path)
        {
            foreach (item_ i in musicControl.playlistView.SelectedItems)
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
            SendFilePathToPlaylist(MusicPlayer.Instance.currentTrackString);
        }

        public void SendSelectedTracksToPlaylists()
        {
            tool.Show("Need to use the current track manager");
            System.Diagnostics.Debugger.Break();
            
            for (int i = musicControl.trackManager.SelectedItems.Count - 1; i > -1; i--)
            {
                SendFilePathToPlaylist(musicControl.trackManager.SelectedItems[i].Name);
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
            foreach (TracklistPanel tm in musicControl.dockedTrackManagers)
            {
                ToolStripMenuItem t = new ToolStripMenuItem(tm.CurrentList.name);
                t.Tag = tm;
                tsm.DropDownItems.Add(t);
            }
        }

        private void trackContext_Opening(object sender, CancelEventArgs e)
        {
            //highlight the last selected items

            //int i = musicControl.trackManager.manager.IndexOf(item);
            
            musicControl.CurrentPlaylistPanel.tracklistView.ShowLastSelected();
            
            //musicControl.CurrentPlaylistPanel.tracklistView.ShowLastSelected();
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
            
            TracklistPanel tm = e.ClickedItem.Tag as TracklistPanel;
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
            TracklistView tm = e.ClickedItem.Tag as TracklistView;
            if (tm != null)
            {
                for (int i = list.Count - 1; i > -1; i--)
                {
                    tm.manager.CurrentList.tracks.Insert(0, list[i]);
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
            musicControl.CurrentPlaylistPanel.tracklistView.MoveSelectedToTop();
        }

        private void checkedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendTracksToCheckedPlaylists();
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.MoveSelectedToBottom();
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
            foreach(TracklistPanel tm in musicControl.dockedTrackManagers)
            {
                tm.CurrentList.Save();
            }
        }
        
        public void UpdateDefaultPlaylistPaths()
        {
            foreach (item_ i in musicControl.playlistView.SelectedItems)
            {
                String name = i.SubItems[0].Text;
                Playlist p = i.Tag as Playlist;
                if (p == null)
                    continue;

                TreeNode tn = musicControl.playlistFileView.FindFileByName(name, musicControl.playlistFileView.Nodes);
                if (tn == null)
                    continue;

                p.path = tn.Name;
                i.SubItems[1].Text = p.path;
            }
        }

        private void findDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateDefaultPlaylistPaths();
        }

        private void musicfileContext_view_files_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadFileNodesToView();
        }

        private void musicfileContext_view_album_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadAlbumNodesToView();
        }

        private void musicfileContext_view_artists_button_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadArtistNodesToView();
        }

        private void yearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadYearNodesToView();
        }

        private void reloadToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            musicControl.musicFileView.manager.LoadDirectoriesToTree();
        }

        private void musicfileContext_dock_button_Click(object sender, EventArgs e)
        {
            musicControl.DockSelectedMusicFiles();
        }

        //Get playlists when the music file context opens
        private void musicFileContext_Opening(object sender, CancelEventArgs e)
        {
            BookmarkDockedPlaylists(musicFilContext_send_button);
            if (musicControl.musicFileView.SelectedNodes.Count == 1)
            {
                if (musicControl.musicFileView.hoveredNode != null)
                {
                    musicControl.musicFileView.SelectedNodes.Clear();
                    musicControl.musicFileView.hoveredNode.BackColor = Color.DarkBlue;
                    musicControl.musicFileView.SelectedNodes.Add(musicControl.musicFileView.hoveredNode);
                }
            }
        }
        
        private void musicFilContext_send_button_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TracklistPanel tm = e.ClickedItem.Tag as TracklistPanel;
            if (tm == null)
                return;
            
            foreach(TreeNode n in musicControl.musicFileView.SelectedNodes)
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
                FileLoader.Instance.AddPlaylistDirectory(output);
                tool.Show("Set default playlist directory to: ",output);
                musicControl.playlistFileView.manager.LoadPlaylistDirectories();
            }
        }

        private void setDefaultDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDefaultDirectory();
        }
        
        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("Filter by Folder not implemented");
            //musicControl.FilterByFolder();
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
                musicControl.playlistView.manager.LabelEdit(musicControl.playlistView.hoveredItem.Index, renameContextTextBox.TextBox.Text);
               //playlistContext.Close();
            }
        }
        
        private void playlistfileContext_folder_button_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileView.OpenSelectedNodeDirectory();
        }

        private void playlistfileContext_delete_button_Click(object sender, EventArgs e)
        {
            musicControl.playlistFileView.manager.DeleteSelectedFiles(true);
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
            ListViewItem item = musicControl.playlistView.hoveredItem;
            if (item == null)
                return;
            musicControl.playlistView.LabelEdit = true;
            item.BeginEdit();
        }
        
        private void trackContext_goToTop_button_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.EnsureVisible(0);
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
            musicControl.CurrentPlaylistPanel.tracklistView.OpenSelectedID3Tags();
        }
        
        private void trackContext_filter_artist_button_Click(object sender, EventArgs e)
        {
            throw new Exception("Filter by Artist not implemented");
        }

        private void trackContext_filter_album_button_Click(object sender, EventArgs e)
        {
            throw new Exception("Filter by Album not implemented");
        }
        
        private void toolStripDropDownButton1_MouseEnter(object sender, EventArgs e)
        {
            
        }

       

        private void toolStripDropDownButton2_MouseEnter(object sender, EventArgs e)
        {
            
        }
        
        private void removeFolderArtToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TreeNode n = musicControl.musicFileView.selectedNode;
            if (n == null)
                return;
            musicControl.musicFileView.GetNodesFolderJPG(n);
        }

        private void editTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = musicControl.musicFileView.hoveredNode;
            if (node == null)
                return;
            SongControl.CreateTagEditor(node.Name,this);
        }

        private void MusicControlGUI_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void MusicControlGUI_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (!InternalClipboard.IsEmpty)
                    InternalClipboard.Files.Clear();
            }
           // string[] files = new string[] { @"c:\temp\test.txt" };
           // this.DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.Copy);
        }
        
        private void MusicControlGUI_DragLeave(object sender, EventArgs e)
        {
            return;
            if (!InternalClipboard.IsEmpty)
            {
                StringCollection fileList = new StringCollection();
                foreach (string s in InternalClipboard.Files)
                    fileList.Add(s);
                Clipboard.SetFileDropList(fileList);
            }
            //WinFormUtils.DoDrop(this,InternalClipboard.Files.ToArray(), DragDropEffects.Copy);
            //TotalClipboard.Files.Clear();
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
            ArrayList nodes = musicControl.musicFileView.SelectedNodes;
            if (nodes.Count == 0)
            {
                TreeNode t = musicControl.musicFileView.hoveredNode;
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
            SetFileBoxLabel();
        }

        private void fileViewLabel_Click(object sender, EventArgs e)
        {
            fileSquashBox.Swap();
        }

        private void SetFileBoxLabel()
        {
            musicFilesFront = !musicFilesFront;
            if (!musicFilesFront)
                fileViewLabel.Text = "Music Files";
            else
                fileViewLabel.Text = "Playlist Files";
        }

        private void squashBoxControl1_TopPanel_Click_1(object sender, EventArgs e)
        {
            
        }

        private void songControl1_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.BeforePlayEvent += Player_BeforePlayEvent;
                songControl1.squashBoxControl1.splitContainer.SplitterDistance = 0;
                // songControl1.squashBoxControl1.Swap();
            }
        }

        private void squashBoxControl1_Load(object sender, EventArgs e)
        {
            fileSquashBox.MainSplitContainer.SplitterDistance = 0;
        }

        private void openAlbumArtFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PicturePanel.OpenAlbumArtFolder();
        }

        private void iD3TagToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach(TreeNode n in musicControl.musicFileView.SelectedNodes)
            {
                string s = n.Name;
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
            MusicPlayer.Instance.UseMediaKeys(e.KeyCode);
        }

        private void playlistContext_rename_button_DropDownOpening(object sender, EventArgs e)
        {
            ListViewItem item = musicControl.playlistView.hoveredItem;
            renameContextTextBox.Text = item.SubItems[0].Text;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.trackManager.CopySelectedToClipboard();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.trackManager.PasteFromInternalClipboard();
        }

        private void trackContext_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            musicControl.trackManager.ResetAllColors();
        }

        private void toTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.MoveSelectedToTop();
            trackContext.Close();
        }

        private void toBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.MoveSelectedToBottom();
            trackContext.Close();
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.RestoreOldSelections();
        }

        private void clearOldSelectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicControl.CurrentPlaylistPanel.tracklistView.ClearOldSelections();
        }

        private void chromeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ListViewItem item = musicControl.CurrentPlaylistPanel.tracklistView.hoveredItem;
            if (item == null)
                return;
            int index = item.Index;
            musicControl.CurrentPlaylistPanel.tracklistView.manager.OpenChromeSearch(index);
        }

        private void displayLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tool.DisplayLog(3);
        }
    }
}
