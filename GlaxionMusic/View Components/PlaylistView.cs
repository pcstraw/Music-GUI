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
    public class PlaylistView : ViewBox, IPlaylistView
    {
        public void Construction()
        {
            InitializeComponent();
            lastSelectedIndidex = new SelectedIndexCollection(this);
           // if(this.Columns[1] != null)
           // this.Columns[1].Width = 100;
           // ColumnHeader c1 = new ColumnHeader();
            //c1.Text = "Name";
           // ColumnHeader c2 = new ColumnHeader();
           // c2.Text = "Path";
            //this.Columns.Add(c2);
            OwnerDraw = true;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            DragDrop += TrackManager_DragDrop;
            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            ColumnClick += listbox_ColumnClick;
            this.Columns[1].Width = 1000;
        }

        public PlaylistView()
        {
            Construction();
        }
        
        public Color prevRightClickedItemForeColor;
        public string currentTrack;
        public bool autoUpdateTracks;
        public bool autoUpdateMusicPlayer;
        public bool IsDockedPanel;
        public bool ControlDown;
        public bool currentListChanged;
        bool unselectClick;
        public int lastVisible;
        public string editingProgram;
        ListViewItem unselectItem;
        public List<ListViewItem> CopiedList = new List<ListViewItem>();
        List<ListViewItem> contextItems = new List<ListViewItem>();
        public ContextMenuStrip currentListContext;
        SelectedIndexCollection lastSelectedIndidex;
        
        private void TrackManager_DragDrop(object sender, DragEventArgs e)
        {
            ClearLastSelectedDisplay();
        }

        public void Load()
        {
            MusicPlayer.Player.PlayEvent += MusicPlayer_PlayEvent;
        }

        /*
        public void StoreTopVisibleItem()
        {
            controller.CurrentList.lastVisible = FirstVisible() - 1;
            // Point tp = PointToClient(point);
            //ListViewItem targetItem = GetItemAt(tp.X, tp.Y);
            //  tool.Show(visibleIndex);
        }
        */

        public void UpdateColours()
        {
            //find the current playing item
            foreach (ListViewItem i in Items)
            {
                string track_path = i.Tag as string;
                if (track_path == MusicPlayer.Player.currentTrackString)
                {
                    if (CurrentList == MusicPlayer.Player.playlist && i.Index == MusicPlayer.Player.currentTrack)
                    {
                        i.SubItems[0].Tag = 1;
                        continue;
                    }
                    i.SubItems[0].Tag = 10; //track is playing in another docked panel
                    continue;
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
                        case 10:
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
            this.Invalidate();
        }

        internal void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            UpdateColours();
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
        
        
        public void MoveSelectedTracksToRightClickedItem()
        {
            preContextSelection.Reverse();
            foreach (ListViewItem i in preContextSelection)
            {
                i.Remove();
                Items.Insert(hoveredItem.Index + 1, i);
                i.Selected = true;
                i.BackColor = this.BackColor;
            }
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
            CurrentList.tracks = GetTrackItems();
            MusicPlayer.Player.PlayPlaylist(CurrentList, current_track_index);
        }

        //call when changing the playlist name.  This will not save the file, only uipdate the name
        public void UpdatePlaylistName(string name)
        {
            CurrentList.UpdateName(name);
        }
        
        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentList.Save();
            currentListContext.Close();
        }

        public void saveAsMenu_Click(object sender, EventArgs e)
        {
            currentListContext.Close();
            CurrentList.SaveAs();
        }
        
        public void DropMusicFiles(List<string> files, Point point)
        {
            StoreCurrentState();
            ListViewItem targetItem = GetItemAtPoint(point);
            SelectedItems.Clear();
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
            if (TotalClipboard.Files.Count > 0)
            {
                SelectedItems.Clear();
                DropMusicFiles(TotalClipboard.Files, new Point(e.X, e.Y));
                TotalClipboard.Files.Clear();
                StoreCurrentState();
                return;
            }
            else
            {
                base.ViewBox_DragDrop(sender, e);
                StoreCurrentState();
                return;
            }
        }


        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;


        /*
        public void currentListBox_DragOver(object sender, DragEventArgs e)
        {
            hoveredItem = tool.GetItemFromPoint(this, new Point(e.X, e.Y));
            HighlightItem(hoveredItem);
        }*/

        public ListViewItem rightClickedItem;
        public ListViewItem leftClickedItem;
        
        public Playlist CurrentList { get; set ; }

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
        
        public void updateFilePathsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateTrackFilePaths();
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

        new private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlaylistView
            // 
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ForeColor = System.Drawing.Color.Black;
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.TrackManager_DragOver);
            //this.DoubleClick += new System.EventHandler(this.TrackManager_DoubleClick);
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

        private void TrackManager_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //DropMusicFiles(files.ToList(), new Point(e.X, e.Y));
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

        public void DisplayPlaylist()
        {
            Items.Clear();
            foreach (string track in CurrentList.tracks)
            {
                ListViewItem i = AddFileAsItem(track);
                if (!File.Exists(track))
                {
                    i.SubItems[0].Tag = -1; //track does not exist
                }
            }
            UpdateColours();
        }
    }
}
