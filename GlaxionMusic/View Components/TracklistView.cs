using BrightIdeasSoftware;
using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public class SongItem : ListViewItem
    {
        public Song song;
    }


    public class TracklistView : EnhancedListView, ITracklistView
    {
        public TracklistView()
        {
            InitializeComponent();
           // lastSelectedIndidex = new SelectedIndexCollection(this);
           // if(this.Columns[1] != null)
           // this.Columns[1].Width = 100;
           // ColumnHeader c1 = new ColumnHeader();
            //c1.Text = "Name";
           // ColumnHeader c2 = new ColumnHeader();
           // c2.Text = "Path";
            //this.Columns.Add(c2);
            OwnerDraw = true;
            AllowDrop = true;
           // DragDrop += ViewBox_DragDrop;
            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            ColumnClick += listbox_ColumnClick;
            Leave += TracklistView_Leave;
            Columns.Clear();
            //Columns.Add(new OLVColumn("Blank", "None where"));
            Columns.Add(new OLVColumn("Title", "name"));
            Columns.Add(new OLVColumn("Path", "path"));
            Columns.Add(new OLVColumn("Artist", "artist"));
            Columns.Add(new OLVColumn("Year", "year"));
            Columns.Add(new OLVColumn("Genre", "genre"));
            //Columns[0].Width = 0;
            Columns[0].Width = 250;
            Columns[1].Width = 500;
            Columns[2].Width = 150;
            Columns[3].Width = 150;
            manager = new TrackManager(this,new ColorScheme(ForeColor,BackColor));
            wasSelectedColor = new ColorScheme(Color.Black,Color.Lavender);

            this.DragLeave += TracklistView_DragLeave;
            this.MouseLeave += TracklistView_MouseLeave;
        }

        private void TracklistView_MouseLeave(object sender, EventArgs e)
        {
            ShowLastSelected();
            this.Refresh();
        }

        private void TracklistView_Leave(object sender, EventArgs e)
        {
            ShowLastSelected();
        }

        private void TracklistView_DragLeave(object sender, EventArgs e)
        {
            if (InternalClipboard.IsEmpty)
            {
                if (SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in SelectedItems)
                    {
                        SongItem s = (SongItem)item;
                        InternalClipboard.Add(s.song.path);
                    }
                }
            }
            ShowLastSelected();
        }

        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;
        //public ListViewItem rightClickedItem;
        //public ListViewItem leftClickedItem;
        public ColorScheme wasSelectedColor;

        new private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TracklistView
            // 
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.TrackManager_DragOver);
            this.Enter += new System.EventHandler(this.TrackManager_Enter);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.TrackManager_Validating);
            this.ResumeLayout(false);
        }

        public TrackManager manager;
        public Color prevRightClickedItemForeColor;
        public bool IsDockedPanel;
        public int lastVisible;
        public string editingProgram;
        public ContextMenuStrip currentListContext;
        public void ResetItemColors()
        {
            foreach (ListViewItem i in Items)
            {
                i.ForeColor = manager.CurrentColors.foreColor;
                i.BackColor = manager.CurrentColors.backColor;
            }
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
        
        internal void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            //UpdateList();
            manager.UpdateColours();
            Invalidate();
        }

        protected void listbox_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            manager.UpdateMusicPlayer();
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
            int i = manager.Items[e.Item.Index].State;
            switch (i)
            {
                case 1:
                    e.Item.BackColor = MusicPlayer.PlayColor;
                    e.Item.ForeColor = Color.Black;
                    break;
                case 2:
                    e.Item.BackColor = MusicPlayer.OldPlayColor;
                    e.Item.ForeColor = Color.Black;
                    break;
                case 10:
                    e.Item.BackColor = Color.LightSteelBlue;
                    e.Item.ForeColor = Color.Black;//show current playing item in playlists that are not being played
                    break;
                case -1:
                    e.Item.BackColor = MusicPlayer.MissingColor;
                    e.Item.ForeColor = Color.Black;//show current playing item in playlists that are not being played
                    break;
                case 0: //set default
                    e.Item.BackColor = manager.Items[e.Item.Index].CurrentColor.backColor;
                    e.Item.ForeColor = manager.Items[e.Item.Index].CurrentColor.foreColor;
                    manager.Items[e.Item.Index].State = -100; //set to 100 cose we don't need to check it next time
                    break;
                default:
                    break;
            }
            
            e.DrawBackground();
            e.DrawDefault = true;
            
            //base.OnDrawItem(e);
        }
        
        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.CurrentList.Save();
            currentListContext.Close();
        }

        public void saveAsMenu_Click(object sender, EventArgs e)
        {
            currentListContext.Close();
            manager.CurrentList.SaveAs();
        }
        
        public void DropMusicFiles(List<string> files, Point point)
        {
            int index = 0;
            ListViewItem targetItem = GetItemAtPoint(point);
            if (targetItem != null)
                index = targetItem.Index;
            else
                index = manager.Items.Count;
            //perhaps files should be reverse before they are sent to
            //totalclipboard?
            //files.Reverse();
            InsertMusicFiles(files, index);
            SyncManager();
        }

        private void InsertMusicFiles(List<string> files, int index)
        {
            foreach (string file in files)
            {
                if (tool.IsAudioFile(file))
                {
                    ListViewItem i = CreateItemFromFile(file);
                    Items.Insert(index, i);
                    i.Selected = true;
                    continue;
                }

                if (!Path.HasExtension(file))
                {
                    List<string> dirf = tool.LoadAudioFiles(file, SearchOption.TopDirectoryOnly);
                    dirf.Reverse();
                    InsertMusicFiles(dirf, index);
                }
            }
        }

        protected override void e_ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.e_ItemDrag(sender, e);
        }

        protected override void ViewBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!InternalClipboard.IsEmpty)
            {
                PasteFrom(InternalClipboard.Files.ToArray());
                InternalClipboard.Clear();
                SyncManager();
                manager.UpdateColours();
                this.Focus();
                this.Refresh();
                manager.UpdateColours();
                return;
            }
            else
            {
                base.ViewBox_DragDrop(sender, e);
                InternalClipboard.Clear();
                SyncManager();
                manager.UpdateColours();
                this.Refresh();
            }
        }
        
        public void currentListBox_MouseClick(object sender, MouseEventArgs e)
        {
           // doubleClicked = false;
            /*
            if (e.Button == MouseButtons.Right)
            {
                rightClickedItem = hoveredItem;
            }
            if (e.Button == MouseButtons.Left)
            {
                leftClickedItem = hoveredItem;
            }
            */
            //_selectedItem = hoveredItem;
        }

        internal void MoveSelectedToBottom()
        {
            foreach (ListViewItem i in SelectedItems)
            {
                i.Remove();
                Items.Insert(Items.Count, i);
            }
            SyncManager();
        }
        internal void MoveSelectedTo(int index)
        {
            int count = preContextSelection.Count;
            if (count == 0)
                return;
            int last_index = preContextSelection[count - 1].Index;
            if (last_index > index)
            {
                preContextSelection.Reverse();
                index += 1;
            }
            
            foreach (ListViewItem item in preContextSelection)
            {
                item.Remove();
                Items.Insert(index, item);
            }
            SyncManager();
        }

        internal void TextSearch(string text)
        {
            if (text.Length > 1)
                SearchFor(text, SearchColor, Color.White);
            else
                ResetAllBackColor(SearchColor);

        }

        internal void MoveSelectedToTop()
        {
            foreach (ListViewItem i in SelectedItems)
            {
                i.Remove();
                Items.Insert(0, i);
            }
            SyncManager();
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
                Process.Start(editingProgram);
        }

        public void vegasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVegas();
        }
        
        public void currentListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*
            if (doubleClicked)
            {
                e.NewValue = e.CurrentValue;
                doubleClicked = false;
            }
            */
        }

        public void currentListBox_MouseDown(object sender, MouseEventArgs e)
        {
            /*
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
            */
        }

        //deprecate
        public void currentListBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            doubleClicked = false;
            if (e.KeyCode == Keys.ControlKey)
            {
                //ControlDown = true;
            }
            */
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

        //deprecate
        public void currentListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /*
                if (unselectClick)
                {
                    unselectClick = false;
                }
                */
            }
        }
        //dep? move to trackmanager and call refreshUI
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
            SyncManager();
        }

        public void updateMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.UpdateMusicPlayer();
        }
        /*
        public void updateFilePathsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateTrackFilePaths();
        }
        */
        
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
            InternalClipboard.CopyItems(items.ToList());
            InternalClipboard.CopyToSystemClipboard(items.ToList());
        }
        
        protected override void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            if(!InternalClipboard.Empty())
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.Move;
            }
            /*
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
            */
        }

        //move to drag leave surely?
        private void TrackManager_DragOver(object sender, DragEventArgs e)
        {
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            hoveredItem = base.GetItemAt(cp.X, cp.Y);
            /*
            e.Effect = DragDropEffects.All;
            //return;
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
            */
        }
        
        //move this to onLeave
        private void TrackManager_Validating(object sender, CancelEventArgs e)
        {
           // ShowLastSelected();
        }

        private void ShowLastSelected()
        {
            lastSelectedIndices.Clear();
            foreach (int i in SelectedIndices)
            {
                Items[i].ForeColor = wasSelectedColor.foreColor;
                Items[i].BackColor = wasSelectedColor.backColor;
                lastSelectedIndices.Add(i);
            }
            Refresh();
        }
        
        private void TrackManager_Enter(object sender, EventArgs e)
        {
            manager.ClearLastSelectedDisplay();
        }

        //move to enhance viewbox?
        private ListViewItem.ListViewSubItem MakeSubItem(ListViewItem i,object rowObject, OLVColumn column)
        {
            object cellValue = column.GetValue(rowObject);
            ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(i, column.ValueToString(cellValue), ForeColor, BackColor,this.Font);
            return subItem;
        }

        public void GetColumnInfo(ListViewItem item,object o)
        {
            item.SubItems.Clear();
            //item.SubItems[0] = this.MakeSubItem(item, o, Columns[0] as OLVColumn);
            for (int i = 0; i < Columns.Count; i++)
            {
                OLVColumn col = Columns[i] as OLVColumn;
                ListViewItem.ListViewSubItem subItem = this.MakeSubItem(item, o, col);
                //if (subItem == null)
                 //   return;
                if(i==0)
                    item.SubItems[0] = subItem;
                else
                item.SubItems.Add(subItem);
            }
        }
    
        public void UpdateColumns()
        {
            foreach (ListViewItem i in Items)
            {
                GetColumnInfo(i,((SongItem)i).song);
            }
        }
        
        public void DisplayPlaylist()
        {
            manager.CurrentList.UpdatePaths();
            //manager.CurrentList.LoadSongs();
            manager.Items.Clear();
            foreach(string s in manager.CurrentList.tracks)
            {
                SongItem i = CreateSongItem(s);
                if (i == null)
                    continue;
                ListViewItem item = (ListViewItem)i;
                item.Name = s;
                Items.Add(item);
            }
            SyncManager();
            UpdateColumns();
        }

        SongItem CreateSongItem(string s)
        {
            SongItem i = new SongItem();
            Song song = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(s);
            if (song != null)
            {
                i.Name = song.path;
                i.Tag = song.path;
                i.song = song;
            }
            else
                return null;
            return i;
        }


        public void SyncView()
        {

        }
        
        public void SyncManager()
        {
            manager.Items.Clear();
            foreach(ListViewItem item in Items)
            {
                VItem vi = WinFormUtils.GetVItem(item as SongItem);
                manager.Items.Add(vi);
            }
            GetSelectedItems();
            manager.UpdateColours();
            Update();
        }

        public void GetSelectedItems()
        {
            manager.SelectedItems.Clear();
            foreach (int i in SelectedIndices)
            {
                manager.SelectedItems.Add(manager.Items[i]);
            }
        }

        internal void CopySelectedToClipboard()
        {
            StringCollection arr = new StringCollection();
            foreach (int i in lastSelectedIndices)
            {
                SongItem si = (SongItem)Items[i];
                arr.Add(si.song.path);
            }
           // Clipboard.SetFileDropList(arr);
            Clipboard.SetData("String_Array", arr);
        }
        /*
        SongItem GetSongItem(string s)
        {
            Song song = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(s);
            if (song == null)
                return null;
            //   tool.show(2, "Song is NUll");
            SongItem i = CreateSongItem(song);
            return i;
        }
        */

        public void MoveSelectedItemsTo(int dropIndex, string[] arr)
        {
            List<ListViewItem> insertItems = new List<ListViewItem>();
            foreach (string s in arr)
            {
                if(Directory.Exists(s))
                {
                    List<string> s_array = tool.LoadAudioFiles(s,SearchOption.TopDirectoryOnly);
                    foreach(string s2 in s_array)
                    {
                        SongItem i1 = CreateSongItem(s2);
                        if (i1 == null)
                            continue;
                        insertItems.Add(i1);
                    }
                    continue;
                }
                else{
                    SongItem i2 = CreateSongItem(s);
                    if (i2 == null)
                        continue;
                    insertItems.Add(i2);
                }
            }

            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                var insertItem = insertItems[i];
                insertItem.Selected = true;
                base.Items.Insert(dropIndex, insertItem);
            }
        }

        internal void PasteFrom(string[] files)
        {
                int dropIndex = 0;
                if (hoveredItem != null)
                {
                    hoveredItem.Selected = false;
                    dropIndex = hoveredItem.Index + 1;
                }
                else
                {
                    dropIndex = Items.Count;
                }
                MoveSelectedItemsTo(dropIndex, files);
                UpdateColumns();
                SyncManager();
                manager.UpdateColours();
                this.Invalidate();
                return;
        }

        internal void PasteFromClipboard()
        {
            IDataObject d = Clipboard.GetDataObject();
            if (d.GetDataPresent("String_Array"))
            {
                int dropIndex = 0;
                if (hoveredItem != null)
                {
                    hoveredItem.Selected = false;
                    dropIndex = hoveredItem.Index+1;
                }
                else
                {
                    dropIndex = Items.Count;
                }

                string[] arr = (string[])d.GetData("String_Array");
                

                MoveSelectedItemsTo(dropIndex, arr);
                //Clipboard.Clear();
                UpdateColumns();
                SyncManager();
                manager.UpdateColours();
                return;
            }
            
        }
    }
}
