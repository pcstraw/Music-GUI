using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Glaxion.Music
{
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
            //DragDrop += ViewBox_DragDrop;
            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            ColumnClick += listbox_ColumnClick;
            this.Columns[1].Width = 1000;
            manager = new TrackManager(this,new ColorScheme(ForeColor,BackColor));
            wasSelectedColor = new ColorScheme(Color.Black,Color.Lavender);
        }

        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;
        public ListViewItem rightClickedItem;
        public ListViewItem leftClickedItem;
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
            this.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.TracklistView_ItemSelectionChanged);
            this.SelectedIndexChanged += new System.EventHandler(this.TracklistView_SelectedIndexChanged);
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
        
        private void TrackManager_DragDrop(object sender, DragEventArgs e)
        {
            ResetItemColors();
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
            
            if (TotalClipboard.Files.Count() > 0)
            {
                DropMusicFiles(TotalClipboard.Files, new Point(e.X, e.Y));
                TotalClipboard.Files.Clear();
                return;
            }
            else
            {
                base.ViewBox_DragDrop(sender, e);
                SyncManager();
                return;
            }
        }
        
        public void currentListBox_MouseClick(object sender, MouseEventArgs e)
        {
            doubleClicked = false;
            if (e.Button == MouseButtons.Right)
            {
                rightClickedItem = hoveredItem;
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
            doubleClicked = false;
            if (e.KeyCode == Keys.ControlKey)
            {
                //ControlDown = true;
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

        //move to drag leave surely?
        private void TrackManager_DragOver(object sender, DragEventArgs e)
        {
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
            
        }
        
        //move this to onLeave
        private void TrackManager_Validating(object sender, CancelEventArgs e)
        {
            ShowLastSelected();
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
        }
        
        private void TrackManager_Enter(object sender, EventArgs e)
        {
            manager.ClearLastSelectedDisplay();
        }
        
        public void DisplayPlaylist()
        {
            manager.CurrentList.UpdatePaths();
            manager.Items.Clear();
            foreach (string item in manager.CurrentList.tracks)
            {
                AddFileAsItem(item);
            }
            SyncManager();
        }

        public void SyncView()
        {

        }
        
        public void SyncManager()
        {
            manager.Items.Clear();
            foreach(ListViewItem item in Items)
            {
                VItem vi = WinFormUtils.GetVItem(item);
                manager.Items.Add(vi);
            }
            GetSelectedItems();
            manager.UpdateColours();
            Invalidate();
        }

        private void TracklistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tool.show(1, "Selection changed");
        }

        private void TracklistView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
           // manager.SelectedItems.Add(manager.Items[e.ItemIndex]);
        }

        public void GetSelectedItems()
        {
            manager.SelectedItems.Clear();
            foreach (int i in SelectedIndices)
            {
                manager.SelectedItems.Add(manager.Items[i]);
            }
        }
    }
}
