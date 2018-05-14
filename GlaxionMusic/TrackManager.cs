using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public class TrackManager : ViewBox
    {
        public void Construction()
        {
            InitializeComponent();
            lastSelectedIndidex = new SelectedIndexCollection(this);
            if(this.Columns[1] != null)
            this.Columns[1].Width = 750;
        }

        public TrackManager()
        {
            Construction();
        }

        public TrackManager(Playlist playList)
        {
            Construction();
            currentList = playList;
        }
        
        public Color prevRightClickedItemForeColor;
        public string currentTrack;
        public string playlistName;
        public bool autoUpdateTracks;
        public bool autoUpdateMusicPlayer;
        public bool IsDockedPanel;
        public bool ControlDown;
        public bool currentListChanged;
        bool unselectClick;
        public int lastVisible;
        public string editingProgram;
        ListViewItem unselectItem;
        ListViewItem doubleClickedItem;
        ListViewItem contextItem;
        public List<ListViewItem> CopiedList = new List<ListViewItem>();
        List<ListViewItem> contextItems = new List<ListViewItem>();
        public ContextMenuStrip currentListContext;
        SelectedIndexCollection lastSelectedIndidex;
        //set up the manager
        public void LoadManager()
        {
            // MusicPlayer.Player.SetLastPlaylistTrack();
            LoadPlaylistIntoView(MusicPlayer.Player.playlist);
            AssignEventHandlers();
            autoUpdateTracks = true;
            autoUpdateMusicPlayer = false;
        }

        public void AssignEventHandlers()
        {
            FindEvent += new FindTrackEventHander(On_FindTrackInTreeView);
            MusicPlayer.Player.PlayEvent += MusicPlayer_PlayEvent;
            //UpdatePlaylistEvent += new UpdatePlaytlistHandler(on_Updateplaylist);
            DragDrop += TrackManager_DragDrop;

            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            // DrawSubItem += TrackManager_DrawSubItem;
            ColumnClick += listbox_ColumnClick;
        }

        private void TrackManager_DragDrop(object sender, DragEventArgs e)
        {
            ClearLastSelectedDisplay();
        }

        private void TrackManager_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            //e.DrawBackground();
        }

        public void UpdatePlayStateColours()
        {
            //find the current playing item
            foreach (ListViewItem i in Items)
            {
                if (i.Index == MusicPlayer.Player.currentTrack)
                {
                    string track_path = i.Tag as string;
                    if (track_path == MusicPlayer.Player.currentTrackString)
                    {
                        i.SubItems[0].Tag = 1;
                        continue;
                    }
                }
                //last played tracks from 0-3 with 0 being the current track
                else if (i.SubItems[0].Tag != null)
                {
                    int index = (int)i.SubItems[0].Tag;
                    switch (index)
                    {
                        case 1:
                            i.SubItems[0].Tag = 2;
                            break;
                        case 2:
                            {
                                i.SubItems[0].Tag = null;
                                i.BackColor = this.BackColor;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //load the current playlist into the list box
        public Playlist LoadIntoView(Playlist list)
        {
            if (list == null)
                return null;
            currentList = list;
            list.UpdatePaths();
            Items.Clear();
            foreach (string track in list.tracks)
            {
                ListViewItem i = AddFileAsItem(track);
                if (!File.Exists(track))
                {
                    i.SubItems[0].Tag = -1;
                }
            }
            currentList = list;
            return list;
        }

        private void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            if (currentList == MusicPlayer.Player.playlist)
                UpdatePlayStateColours();
            else
            {
                foreach (ListViewItem i in Items)
                {
                    string s = i.Tag as string;
                    if (tool.StringCheck(s))
                    {
                        if (s == MusicPlayer.Player.currentTrackString)
                            i.SubItems[0].Tag = 10; //track is playing in another playlist
                        else
                        {
                            if (i.SubItems[0].Tag != null)
                            {
                                i.SubItems[0].Tag = null;
                                i.BackColor = BackColor;
                            }
                        }
                    }
                }
            }
            this.Invalidate();
        }

        protected void listbox_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            UpdateMusicPlayer();
        }

        public void CheckForRepeat()
        {
            foreach (ListViewItem item in SelectedItems)
            {
                if (item == null)
                    continue;
                string file = item.Tag as string;
                if (!tool.StringCheck(file))
                    continue;

                foreach (ListViewItem other in Items)
                {
                    string s = other.Tag as string;
                    if (!tool.StringCheck(s))
                        continue;
                    if (s == file)
                        other.Selected = true;
                }
            }
        }

        public void UpdateListView()
        {
            Items.Clear();
            foreach (string s in currentList.tracks)
            {
                AddFileAsItem(s);
            }
        }

        protected override void e_KeyUp(object sender, KeyEventArgs e)
        {
            base.e_KeyUp(sender, e);
            //don't work well;
            if (e.KeyCode == Keys.Z)
                RestoreLastState();
            if (e.KeyCode == Keys.X)
                RestoreNextState();
        }

        private void TrackManager_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            e.DrawBackground();
        }

        private void TrackManager_CustomDraw(object sender, DrawListViewItemEventArgs e)
        {
            
            /*
            if (e.Item.BackColor != Color.DarkBlue)
            {
                if(e.Item.ForeColor == Color.White)
                    e.Item.ForeColor = Color.Black;
            }else if(e.Item.ForeColor == Color.Black)
            {
                e.Item.ForeColor = Color.White;
            }
            */
            //show last selected item when the control has lost focus
            /*
            if (e.Item.SubItems[1].Tag != null)
            {
                e.Item.BackColor = Color.DarkBlue;
                e.Item.ForeColor = Color.White;
            }else
            {
                e.Item.BackColor = ForeColor;
                e.Item.ForeColor = BackColor;
            }
            */

            if (e.Item.SubItems[0].Tag != null)
            {
                int i = (int)e.Item.SubItems[0].Tag;
                switch (i)
                {
                    case 1:
                        e.Item.BackColor = MusicPlayer.PlayColor;
                        break;
                    case 2:
                        e.Item.BackColor = MusicPlayer.OldPlayColor;
                        break;
                    case 10:
                        e.Item.BackColor = Color.LightSteelBlue;  //show current playing item in playlists that are not being played
                        break;
                    case -1:
                        e.Item.BackColor = MusicPlayer.MissingColor;  //show current playing item in playlists that are not being played
                        break;
                    default:
                        break;
                }
            }
            e.DrawDefault = true;
            e.DrawBackground();
            //base.OnDrawItem(e);
        }

        public void PlayHoveredNode()
        {
            if (hoveredItem == null)
            {
                tool.show(1, "Nothing Selected");
                return;
            }
            UpdateCurrentListTracks();
            hoveredItem.Selected = false;
            if (!MusicPlayer.Player.PlayPlaylist(currentList, hoveredItem.Index))
            {
                MusicPlayer.Player.NextTrack();
            }
        }

        private void TrackManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            doubleClicked = true;
            doubleClickedItem = _selectedItem;
            if (_selectedItem == null)
                return;
            string s = _selectedItem.Tag as string;
            doubleClickedItem.Selected = false;
        }

        public void MoveSelectedTracksToRightClickedItem()
        {
            if (hoveredItem == null)
            {
                tool.Show("null");
                return;
            }
            preContextSelection.Reverse();
            foreach (ListViewItem i in preContextSelection)
            {
                i.Remove();
                Items.Insert(hoveredItem.Index + 1, i);
                i.Selected = true;
            }
        }

        public bool LoadPlaylistIntoView(Playlist p)
        {
            if (p == null)
                return false;

            if (currentList != p)
            {
                LoadIntoView(p);
                if (currentList.lastVisible > 0)
                    EnsureVisible(currentList.lastVisible);
                UpdatePlayStateColours();
                return true;
            }
            return false;
        }

        public void GrabContextItems()
        {
            contextItems.Clear();
            contextItem = hoveredItem;
            foreach (ListViewItem i in SelectedItems)
                contextItems.Add(i);
        }

        public void RemoveSelectedTracks()
        {
            StoreCurrentState();
            foreach (ListViewItem item in SelectedItems)
                item.Remove();
        }

        public void UpdateMusicPlayer()
        {
            //load the list view items into the playlist
            UpdateCurrentListTracks();
            int current_track_index = MusicPlayer.Player.currentTrack;
            //find the item who's first subtag is 1.  this is the currently playing track
            //storing it on the item allows us to track the last played song as the user re-orders the playlist
            foreach (ListViewItem item in Items)
            {
                //null tags have not been played.  Skip
                if (item.SubItems[0].Tag == null)
                    continue;
                int i = (int)item.SubItems[0].Tag;
                if (i == 1)
                {
                    string s = (string)item.Tag;
                    if (s == MusicPlayer.Player.currentTrackString) //double check to see if is still the current track
                    {
                        //grab the new index
                        current_track_index = item.Index;
                        break;
                    }
                }
            }
            //update the media player with the new song and the new index of the current track
            MusicPlayer.Player.UpdateMusicPlayer(currentList, current_track_index);
        }

        //call when changing the playlist name.  This will not save the file, only uipdate the name
        public void UpdatePlaylistName(string name)
        {
            currentList.UpdateName(name);
            playlistName = name;
        }

        public void Save()
        {
            Playlist p = MakePlaylistFromListbox();
            Playlist t = MusicPlayer.Player.fileLoader.GetPlaylist(p.path, true);
            t.tracks = p.tracks;
            if (t != null)
            {
                t.SaveTo(t.path);
            }
        }

        //what is this used for?
        public Playlist MakePlaylistFromListbox()
        {
            if (currentList == null)
                return null;
            UpdatePlaylist();
            Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(currentList.path, false);
            p.name = currentList.name;
            string dir = Path.GetDirectoryName(currentList.path);
            p.path = dir + @"\" + p.name + p.ext;
            return p;
        }

        public void UpdatePlaylist()
        {
            currentList.tracks = GetTracksFromListView();
        }

        public void SaveTrackBox()
        {
            Playlist p = MakePlaylistFromListbox();
            p.Save();
        }

        public void SaveAsTrackBox()
        {
            Playlist p = MakePlaylistFromListbox();
            p.SaveAs();
        }

        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTrackBox();
            currentListContext.Close();
        }

        public void saveAsMenu_Click(object sender, EventArgs e)
        {
            currentListContext.Close();
            SaveAsTrackBox();
        }

        public void Resume()
        {
            MusicPlayer.Player.Resume(MusicPlayer.Player.currentTrackString, MusicPlayer.Player.positionIndex);
            currentTrack = MusicPlayer.Player.currentTrackString;
        }

        public void Pause()
        {
            MusicPlayer.Player.Pause();
        }

        public void Stop()
        {
            MusicPlayer.Player.Stop();
        }

        public void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
        }

        public void PlayOrResume()
        {
            if (!MusicPlayer.Player.Get)
                return;

            if (MusicPlayer.Player.IsPlaying)
            {
                Pause();
                return;
            }
            if (!MusicPlayer.Player.IsPaused)
            {
                PlayHoveredNode();
                return;
            }
            else
            {
                Resume();
                return;
            }
        }

        public void DropMusicFiles(List<string> files, Point point)
        {
            //store the current track state
            StoreCurrentState();
            ListViewItem targetItem = GetItemAtPoint(point);
            SelectedItems.Clear();
            // files.Reverse();
            foreach (string file in files)
            {
                if (tool.IsAudioFile(file))
                {
                    if (targetItem != null)
                    {
                        ListViewItem item = InsertFileAt(targetItem.Index, file);
                        item.Selected = true;
                    }
                    else
                    {
                        ListViewItem item = AddFileAsItem(file);
                        item.Selected = true;
                    }
                }
                if (!Path.HasExtension(file))
                {
                    List<string> dirf = tool.LoadAudioFiles(file, SearchOption.TopDirectoryOnly);
                    DropMusicFiles(dirf, point);
                }
            }
        }

        public void FindMissingFiles()
        {
            foreach (ListViewItem i in Items)
            {
                if (i.Tag.GetType() != typeof(string))
                    new Exception("No file path set");

                if (!File.Exists(i.Tag as string))
                    i.BackColor = MusicPlayer.MissingColor;
            }
        }

        protected override void e_ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.e_ItemDrag(sender, e);
        }

        protected override void ViewBox_DragDrop(object sender, DragEventArgs e)
        {
            /*
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
             // TotalClipboard.CopyFile(files.ToList());
                //DropMusicFiles(files.ToList(), new Point(e.X, e.Y));
                //TotalClipboard.Files.Clear();
                try
                {
                    e.Data.SetData(null);
                }
                catch { }
            }
            */

            if (TotalClipboard.Files.Count > 0)
            {
                SelectedItems.Clear();
                DropMusicFiles(TotalClipboard.Files, new Point(e.X, e.Y));
                TotalClipboard.Files.Clear();
                StoreCurrentState();
                //  UpdateMusicPlayer();
                return;
            }
            else
            {

                base.ViewBox_DragDrop(sender, e);
                StoreCurrentState();
                //  tool.Show();
                return;
            }
        }


        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;

        //dummy delegate do not delete
        protected virtual void On_FindTrackInTreeView(object sender, EventArgs e)
        {

        }

        /*
        public void currentListBox_DragOver(object sender, DragEventArgs e)
        {
            hoveredItem = tool.GetItemFromPoint(this, new Point(e.X, e.Y));
            HighlightItem(hoveredItem);
        }*/

        public ListViewItem rightClickedItem;
        public ListViewItem leftClickedItem;

        public void currentListBox_MouseClick(object sender, MouseEventArgs e)
        {
            doubleClicked = false;
            if (e.Button == MouseButtons.Right)
            {
                rightClickedItem = hoveredItem;
                //GetLastSelectedItems();
            }
            if (e.Button == MouseButtons.Left)
            {
                leftClickedItem = hoveredItem;

            }
            _selectedItem = hoveredItem;
        }

        internal void MoveSelectedToBottom()
        {
            foreach (ListViewItem i in SelectedItems)
            {
                i.Remove();
                Items.Insert(Items.Count, i);
            }
        }

        internal void MoveSelectedToTop()
        {
            foreach (ListViewItem i in SelectedItems)
            {
                i.Remove();
                Items.Insert(0, i);
            }
        }

        public void OpenVegas()
        {
            if (editingProgram == null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                {
                    ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        editingProgram = ofd.FileName;
                    }
                }
            }
            if (editingProgram != null)
            {
                if (rightClickedItem != null)
                {
                    /*
                    string[] ls = new string[1];
                    ls[0] = rightClickedItem.SubItems[1].Text;
                    ProcessStartInfo si = new ProcessStartInfo(editingProgram);
                    si.Arguments = ls[0];
                    */
                    Process.Start(editingProgram);
                }
            }
        }

        public void vegasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVegas();
        }

        public void replayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.Replay();
        }

        public void currentListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (doubleClicked)
            {
                e.NewValue = e.CurrentValue;
                doubleClicked = false;
            }
        }

        public void currentListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks > 1)
                {
                    doubleClicked = true;
                }
                if (hoveredItem != null && hoveredItem.Selected == true)
                {
                    unselectClick = true;
                }

                if (SelectedItems.Count == 1)
                {
                    unselectItem = SelectedItems[0];
                }
            }
        }

        public void currentListBox_KeyDown(object sender, KeyEventArgs e)
        {
            doubleClicked = false;
            if (e.KeyCode == Keys.ControlKey)
            {
                ControlDown = true;
            }
        }

        public void SelectSearchedMenu()
        {
            foreach (ListViewItem item in Items)
            {
                if (item.BackColor == Color.Violet)
                {
                    item.Selected = true;
                }
            }
            Select();
        }

        public bool ToggleAutoUpdatePaths(bool toggle)
        {
            AutoUpdateTracks = toggle;
            return toggle;
        }

        public void selectSearchedMenu_Click(object sender, EventArgs e)
        {
            SelectSearchedMenu();
        }

        public void currentListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (unselectClick)
                {
                    unselectClick = false;
                }
            }
        }

        public void MoveItem(ListViewItem item, int position)
        {
            Items.Remove(item);
            item.Selected = true;
            switch (position)
            {
                case 0:
                    Items.Insert(hoveredItem.Index + 1, item);
                    break;
                case 1: //move to the top
                    Items.Insert(0, item);
                    break;
                case 2:
                    Items.Insert(Items.Count, item);
                    break;
                case 3:
                    {
                        ListViewItem i = FindItemByPath(MusicPlayer.Player.currentTrackString);
                        if (i != null)
                        {
                            Items.Insert(i.Index + 1, item);
                        }
                        else
                        {
                            Items.Insert(Items.Count, item);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public void updateMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMusicPlayer();
        }

        public void ToggleUpdateMusic()
        {
            autoUpdateMusicPlayer = !autoUpdateMusicPlayer;
        }

        public void playToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            PlayHoveredNode();
        }

        public void quickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTrackBox();
            currentListContext.Close();
        }

        public void updateFilePathsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateTrackFilePaths();
        }

        public void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayOrResume();
        }

        public void nextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MusicPlayer.Player != null)
                MusicPlayer.Player.NextTrack();
        }

        public void prevToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MusicPlayer.Player != null)
                MusicPlayer.Player.PrevTrack();
        }

        public void stopToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (MusicPlayer.Player != null)
                MusicPlayer.Player.Stop();
        }

        public void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveSelectedTracks();
        }

        public void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedTracks();
        }

        public void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in SelectedItems)
            {
                if (File.Exists(item.SubItems[1].Text))
                    File.Delete(item.SubItems[1].Text);
            }
        }

        public void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem[] items = new ListViewItem[SelectedItems.Count];
            SelectedItems.CopyTo(items, 0);
            TotalClipboard.CopyItems(items.ToList());
            TotalClipboard.CopyToSystemClipboard(items.ToList());
        }

        public void MoveItem(ListViewItem item, int index, bool move)
        {
            if (move)
            {
                item.Remove();
                Items.Insert(index, item);
                item.Selected = true;
            }
            else
            {
                item.Selected = false;
                ListViewItem i = item.Clone() as ListViewItem;
                Items.Insert(index, i);
                i.Selected = true;
            }
        }

        new private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TrackManager
            // 
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ForeColor = System.Drawing.Color.Black;
            this.OwnerDraw = true;
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.TrackManager_DragOver);
            this.DoubleClick += new System.EventHandler(this.TrackManager_DoubleClick);
            this.Enter += new System.EventHandler(this.TrackManager_Enter);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.TrackManager_Validating);
            this.ResumeLayout(false);

        }

        protected override void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (!TotalClipboard.IsEmpty)
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.Move;
            }
        }

        private void TrackManager_DoubleClick(object sender, EventArgs e)
        {
            PlayHoveredNode();
        }

        private void TrackManager_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //DropMusicFiles(files.ToList(), new Point(e.X, e.Y));
                //  tool.Show();
                TotalClipboard.Files.Clear();
                TotalClipboard.Files = files.ToList();
                try
                {
                    e.Data.SetData(null);
                }
                catch { }
            }
        }
        
        private void TrackManager_Validating(object sender, CancelEventArgs e)
        {
            ShowLastSelected();
        }

        private void ShowLastSelected()
        {
            lastSelectedIndices.Clear();
            foreach (int i in SelectedIndices)
            {
                Items[i].ForeColor = Color.White;
                Items[i].BackColor = Color.DarkBlue;
                lastSelectedIndices.Add(i);
            }
        }

        private void ClearLastSelectedDisplay()
        {
            foreach(ListViewItem i in Items)
            {
                i.ForeColor = ForeColor;
                i.BackColor = BackColor;
            }
        }

        private void TrackManager_Enter(object sender, EventArgs e)
        {
            ClearLastSelectedDisplay();
        }
    }
}
