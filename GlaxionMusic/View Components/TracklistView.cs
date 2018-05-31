
using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public class TracklistView : EnhancedListController, ITracklistView,IListView
    {
        public TracklistView()
        {
            InitializeComponent();
            OwnerDraw = true;
            AllowDrop = true;
            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            Columns.Clear();
            Columns.Add(new ColumnHeader());
            Columns.Add(new ColumnHeader());
            Columns.Add(new ColumnHeader());
            Columns.Add(new ColumnHeader());
            Columns.Add(new ColumnHeader());
            Columns[0].Width = 400;
            Columns[1].Width = 500;
            Columns[2].Width = 150;
            Columns[3].Width = 150;
            Columns[3].Width = 150;
            Columns[0].Text = "Title";
            Columns[1].Text = "Path";
            Columns[2].Text = "Artist";
            Columns[3].Text = "Year";
            Columns[4].Text = "Genre";
            manager = new TrackManager(this,this,new ColorScheme(ForeColor,BackColor));
            //wasSelectedColor = new ColorScheme(Color.Black, Color.LightSkyBlue);
            wasSelectedColor = new ColorScheme(Color.Black, Color.PaleGoldenrod);
            this.DragLeave += TracklistView_DragLeave;
            this.MouseUp += TracklistView_MouseUp;
            this.MouseDown += TracklistView_MouseDown;
            this.MouseLeave += TracklistView_MouseLeave;
            this.Click += TracklistView_Click;
        }

        private void TracklistView_Click(object sender, EventArgs e)
        {
            manager.UpdateColours();
        }
        
        private void TracklistView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ResetAllColors();
            }
            manager.UpdateColours();
        }
        private void TracklistView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CacheLastSelectedIndices();
            }
            manager.UpdateColours();
        }

        private void TracklistView_MouseLeave(object sender, EventArgs e)
        {
            ShowLastSelected();
        }

        new private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TracklistView
            // 
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.ResumeLayout(false);
        }

        internal void RemoveSelectedTracks()
        {
            BeginUpdate();
            List<int> list = GetSelected().ToList();
            manager.RemoveSelectedTracks(list);
            SelectedIndices.Clear();
            manager.ClearSelection();
            EndUpdate();
        }
        
        public void ResetAllColors()
        {
            for(int i =0;i<manager.Items.Count;i++)
            {
                //manager.Items[i].State = ItemState.Reset;
            }
        }
        
        private void TracklistView_DragLeave(object sender, EventArgs e)
        {
            if (InternalClipboard.IsEmpty)
            {
                if (SelectedItems.Count > 0)
                {
                    foreach (int item in SelectedIndices)
                    {
                        InternalClipboard.Add(manager.Items[item].Name);
                    }
                }
            }
            CacheLastSelectedIndices();
            ShowLastSelected();
        }

        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;
        //[Obsolete("This is not supported in this class.", true)]
        public ColorScheme wasSelectedColor;
        new public TrackManager manager;
        public Color prevRightClickedItemForeColor;
        public bool IsDockedPanel;
        public int lastVisible;
        public string editingProgram;
        public ContextMenuStrip currentListContext;
        
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
            Refresh();
        }
        

        //move this derived class?
        protected override void EnhancedListView_KeyUp(object sender, KeyEventArgs e)
        {
            base.EnhancedListView_KeyUp(sender, e); //do we need to call base?
            if (e.KeyCode == Keys.Z)
                RestoreLastState();
            if (e.KeyCode == Keys.X)
                RestoreNextState();
            if (e.KeyCode == Keys.Delete)
                RemoveSelectedTracks();
        }

        private void TrackManager_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            e.DrawBackground();
        }

        private void TrackManager_CustomDraw(object sender, DrawListViewItemEventArgs e)
        {
            ItemState i = manager.Items[e.Item.Index].State;
            switch (i)
            {
                case ItemState.IsThePlayingTrack:
                    e.Item.BackColor = MusicPlayer.IsPlayingColor;
                    e.Item.ForeColor = Color.Black;
                    break;
                case ItemState.IsPlaying:
                    e.Item.BackColor = MusicPlayer.PlayColor;
                    e.Item.ForeColor = Color.Black;
                    break;
                case ItemState.WasPlaying:
                    e.Item.BackColor = MusicPlayer.OldPlayColor;
                    e.Item.ForeColor = Color.Black;
                    break;
                case ItemState.IsPlayingInOtherPanel:
                    e.Item.BackColor = Color.PaleTurquoise;
                    e.Item.ForeColor = Color.Black;//show current playing item in playlists that are not being played
                    break;
                case ItemState.Missing:
                    e.Item.BackColor = MusicPlayer.MissingColor;
                    e.Item.ForeColor = Color.Black;//show current playing item in playlists that are not being played
                    break;
                case ItemState.Reset: //set default
                    e.Item.BackColor = BackColor; 
                    e.Item.ForeColor = ForeColor;
                    manager.Items[e.Item.Index].State = ItemState.Normal; //set to normal so the rendering is skipped
                    break;
                case ItemState.WasSelected: //set default
                    e.Item.BackColor = wasSelectedColor.backColor; //set to 100 cose we don't need to check it next time
                    e.Item.ForeColor = wasSelectedColor.foreColor;
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
            InsertMusicFiles(files, index);
        }

        //guess we're not using this??
        private void InsertMusicFiles(List<string> files, int index)
        {
            foreach (string file in files)
            {
                if (tool.IsAudioFile(file))
                {
                    VItem i = manager.CreateItem(file);
                    manager.Insert(index, i);
                    i.Selected = true;
                    continue;
                }
                //if the file is a directory, find any audio files
                //and add them recursively
                if (!Path.HasExtension(file))
                {
                    List<string> dirf = tool.LoadAudioFiles(file, SearchOption.TopDirectoryOnly);
                    dirf.Reverse();
                    InsertMusicFiles(dirf, index);
                }
            }
            UpdateColumnsAndColors();
        }

        protected override void e_ItemDrag(object sender, ItemDragEventArgs e)
        {
            base.e_ItemDrag(sender, e);
        }

        public override void SelfDragDrop(int dropIndex)
        {
            //can be simplified
            List<int> list = new List<int>(SelectedIndices.Count);
            foreach(int i in SelectedIndices)
                list.Add(i);
            BeginUpdate();
            manager.MoveSelectedItemsTo(dropIndex, list);
            EndUpdate();
        }

        protected override void EnhancedListBox_DragDrop(object sender, DragEventArgs e)
        {
            //external drag drop
            //in future use tracklist clipboard so it doesn't interfer
            //with copy/paste functions
            if (!InternalClipboard.IsEmpty)
            {
                manager.ClearSelection();
                SelectedIndices.Clear();
                PasteFrom(InternalClipboard.Files.ToArray());
                InternalClipboard.Clear();
                Focus();
                Refresh();
                return;
            }
            else
            {
                string[] data = WinFormUtils.GetExternalDragDrop(e);
                if (data == null)
                {
                    base.EnhancedListBox_DragDrop(sender, e);
                    InternalClipboard.Clear();
                }
                else
                {
                    PasteFrom(data);
                    InternalClipboard.Clear();
                    this.Focus();
                    this.Refresh();
                    return;
                }
            }
        }
        
        internal void MoveSelectedToTop()
        {
            BeginUpdate();
            List<int> indices = GetSelected().ToList();
            manager.MoveIndicesTo(0, indices);
            EndUpdate();
            UpdateColumnsAndColors();
        }

        internal void MoveSelectedToBottom()
        {
            BeginUpdate();
            List<int> indices = GetSelected().ToList();
            manager.MoveIndicesTo(manager.ItemCount,indices);
            EndUpdate();
            UpdateColumnsAndColors();
        }

        internal void MovePreSelectedTo(int index)
        {
            int count = lastSelectedIndices.Count;
            if (count == 0)
                return;
            
            index += 1;
            lastSelectedIndices.Reverse();
            
            List<int> removedItems = new List<int>();
            foreach (int i in lastSelectedIndices)
            {

                VItem newi = manager.Items[i].Clone();
                newi.Selected = true;
                manager.Insert(index, newi);
                removedItems.Add(i);
            }
            
            foreach(int i in removedItems)
                manager.Remove(i);

            UpdateColumnsAndColors();
        }

     
        internal void TextSearch(string text)
        {
            if (text.Length > 1)
                SearchFor(text, SearchColor, Color.White);
            else
                ResetAllBackColor(SearchColor);
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
        
        internal void CheckForRepeat()
        {
            List<int> list = GetSelected().ToList();
            manager.CheckForRepeat(list);
        }

        public void vegasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVegas();
        }
        
        public void updateMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.UpdateMusicPlayer();
        }

        public void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in SelectedItems)
            {
                if (File.Exists(item.Name))
                    File.Delete(item.Name);
            }
        }
        /*
        private void InternalDragDrop(DragEventArgs e)
        {
            //check for internal drag drop first
            if (!InternalClipboard.IsEmpty)
                return;

            if (e.Data.GetDataPresent(typeof(SelectedListViewItemCollection)))
            {
                //if we sent our own selected nodes as arg then we are doing an internal drag drop
                SelectedListViewItemCollection o = (SelectedListViewItemCollection)e.Data.GetData(typeof(SelectedListViewItemCollection));
                if (o != SelectedItems)
                    return;
                if (SelectedItems.Count == 0)
                    return;

                Point cp = PointToClient(new Point(e.X, e.Y));
                ListViewItem dragToItem = GetItemAt(cp.X, cp.Y);
                int dropIndex = 0;
                if (dragToItem == null)
                {
                    dropIndex = Items.Count;
                }
                else
                {
                    dropIndex = dragToItem.Index;
                    if (dropIndex > base.SelectedItems[0].Index)
                    {
                        dropIndex++;
                    }
                }
            }
            tmrLVScroll.Enabled = false;
            return;
        }
        */
        
        protected override void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            ResetAllColors();
            CacheLastSelectedIndices();
            e.Effect = e.AllowedEffect;
            e.Effect = DragDropEffects.All;
        }
        
        public void ShowLastSelected()
        {
            ShowLastSelected(lastSelectedIndices, wasSelectedColor);
            Refresh();
        }

        private void ShowLastSelected(List<int> lastSelected, ColorScheme wasSelectedColor)
        {
            foreach(int i in lastSelected)
            {
                if (i > manager.ItemCount)
                    continue;
                manager.Items[i].State = ItemState.WasSelected;
                //Items[i].BackColor = Color.LightSkyBlue;
            }
        }

        public void ClearLastSelectedDisplay()
        {
            foreach (int i in lastSelectedIndices)
                manager.Items[i].State = ItemState.Reset;
        }
        
        public void UpdateColumnsAndColors()
        {
            manager.UpdateColours();
        }
        
        public void DisplayPlaylist()
        {
            manager.CurrentList.UpdatePaths();
            manager.Items.Clear();
            foreach(string s in manager.CurrentList.tracks)
            {
                VItem i = manager.CreateItem(s);
                manager.Add(i);
            }
            UpdateColumnsAndColors();
        }

        internal void CopySelectedToClipboard()
        {
            List<int> list = GetSelected().ToList();
            manager.CopyToInternalClipboard(list);
        }
        
        public void PasteFrom(string[] files)
        {
            int dropIndex = 0;
            if (hoveredItem != null)
            {
                dropIndex = hoveredItem.Index + 1;
            }
            else
            {
                dropIndex = manager.ItemCount;
            }
            manager.AddFiles(dropIndex, files);
            UpdateColumnsAndColors();
            this.Invalidate();
        }
        
        internal void PasteFromInternalClipboard()
        {
            if (InternalClipboard.IsEmpty)
                return;
            int dropIndex = 0;
            if (hoveredItem != null)
            {
                dropIndex = hoveredItem.Index + 1;
            }
            else
            {
                dropIndex = manager.ItemCount;
            }
            
            manager.AddFiles(dropIndex, InternalClipboard.Files.ToArray());
            UpdateColumnsAndColors();
            Invalidate();
        }
        
        public void Add(VItem item)
        {
            ListViewItem i = new ListViewItem();
            GetColumnInfo(i, item.Tag);
            WinFormUtils.GetItem(i,item);
            Items.Add(i);
        }

        public void Insert(int index, VItem item)
        {
            ListViewItem i = new ListViewItem();
            GetColumnInfo(i, item.Tag);
            WinFormUtils.GetItem(i, item);
            Items.Insert(index,i);
        }

        public void Remove(int index)
        {
            Items.RemoveAt(index);
        }

        public void OpenSelectedID3Tags()
        {
            foreach (int i in SelectedIndices)
            {
                string path = manager.Items[i].Name;
                SongControl sc = SongControl.CreateTagEditor(path, MusicPlayer.WinFormApp);
                sc.squashBoxControl1.Swap();
            } 
        }

        public void RefreshColors()
        {
            this.Invalidate();
        }

        internal void RestoreOldSelections()
        {
            for(int i=0;i<Items.Count;i++)
            {
                VItem item = manager.Items[i];
                if(item.State == ItemState.WasSelected)
                {
                    item.Selected = true;
                    Items[i].Selected = true;
                }

            }
        }

        internal void ClearOldSelections()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                VItem item = manager.Items[i];
                if (item.State == ItemState.WasSelected)
                {
                    item.State = ItemState.Reset;
                    item.Selected = false;
                    Items[i].Selected = false;
                }

            }
        }
    }
}
