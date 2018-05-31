using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Glaxion.Music;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Linq;

namespace Glaxion.Music
{
    public class PlaylistManagerView : EnhancedListController, IPlaylistManager,IListView
    {
        new public PlaylistManager manager;

        public PlaylistManagerView()
        {
            InitializeComponent();
            AllowDrop = true;
            Columns[1].Width = 1000;
            manager = new PlaylistManager(this,this,new ColorScheme(ForeColor,BackColor));
        }

        private new void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlaylistManagerView
            // 
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Click += new System.EventHandler(this.PlaylistManagerView_Click);
            this.ResumeLayout(false);

        }
        
        public void AssignEventHandlers()
        {
            MusicPlayer.Instance.TrackChangeEvent += PlaylistManager_TrackChangeEvent;
            this.AfterLabelEdit += PlaylistManager_AfterLabelEdit;
            this.DragEnter += PlaylistManager_DragEnter;
            this.DragLeave += PlaylistManager_DragLeave;
        }

        internal void Load()
        {
            //manager.CurrentColor = new ColorScheme(ForeColor, BackColor);
            manager.LoadManager();
            AssignEventHandlers();
        }

        private void PlaylistManager_DragLeave(object sender, EventArgs e)
        {
            /*
            TotalClipboard.Files.Clear();
            foreach(ListViewItem item in SelectedItems)
            {
                Playlist p = item.Tag as Playlist;
                if (p == null)
                    continue;
                string s = p.path;
                if(tool.IsPlaylistFile(s) && File.Exists(s))
                {
                    TotalClipboard.Files.Add(s);
                }
            }
            this.DoDragDrop(new DataObject(DataFormats.FileDrop, TotalClipboard.Files.ToArray()),
                    DragDropEffects.Copy);
            TotalClipboard.Files.Clear();
            */
        }

        public override void SelfDragDrop(int dropIndex)
        {
            MoveSelectedItemsTo(dropIndex);
        }
        
        protected override void EnhancedListBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!InternalClipboard.IsEmpty)
            {
                Point cp = PointToClient(new Point(e.X, e.Y));
                ListViewItem dragToItem = GetItemAt(cp.X, cp.Y);
                int dropindex = manager.ItemCount;
                if (dragToItem != null)
                    dropindex = dragToItem.Index;
                manager.AddFiles(dropindex, InternalClipboard.Files.ToArray());
                InternalClipboard.Files.Clear();
                this.Focus();
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
                    foreach (string file in data)
                    {
                        Point cp = PointToClient(new Point(e.X, e.Y));
                        ListViewItem dragToItem = GetItemAt(cp.X, cp.Y);
                        int dropindex = manager.ItemCount;
                        if (dragToItem != null)
                            dropindex = dragToItem.Index;
                        VItem i = manager.InsertPlaylistAt(dropindex, file);
                        if (i == null)
                            continue;
                        i.Selected = true;
                    }
                    InternalClipboard.Clear();
                    this.Focus();
                    this.Refresh();
                    return;
                }
            }
        }

        private void PlaylistManager_DragEnter(object sender, DragEventArgs e)
        {
            if (!InternalClipboard.IsEmpty)
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.All;
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
            }
        }
        
        private void PlaylistManager_TrackChangeEvent(object sender, EventArgs args)
        {
            manager.DisplayTrackInfo();
        }
        
        public void OpenHoveredPlaylistDirectory()
        {
            Playlist p = hoveredItem.Tag as Playlist;
            if (p != null)
                tool.OpenFileDirectory(p.path);
        }

        private void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            manager.DisplayTrackInfo();
        }

        protected override void EnhancedListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveSelectedItems();
        }

        protected override void e_MouseEnter(object sender, EventArgs e)
        {
            manager.DisplayTrackInfo();
        }
        
        private void PlaylistManager_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            LabelEdit = false;
            if (e.Label == null || e.Label == "")
            {
                e.CancelEdit = true;
                return;
            }
            manager.LabelEdit(e.Item,e.Label);
        }
        /*
        public void UpdateInterface()
        {
            RefreshList(manager.Items);
        }
        */
        /*
        public void UpdateList()
        {
            manager.Items.Clear();
            foreach(ListViewItem i in Items)
            {
                VItem v_item = GetVListItem(i);
                manager.Items.Add(v_item);
            }
        }
        */
        /*
        public void GetSelectedItems()
        {
            manager.SelectedIndices.Clear();
            foreach(int i in SelectedIndices)
            {
                manager.SelectedIndices.Add(i);
            }
        }
        */
        //replace with custom draw
        public void RefreshUI()
        {
            //RefreshList(manager.Items);
            
            for(int i=0;i<manager.ItemCount;i++)
            {
                ListViewItem item = Items[i];
                VItem vitem = manager.Items[i];
                item.ForeColor = vitem.CurrentColor.foreColor;
                item.BackColor = vitem.CurrentColor.backColor;
            }
        }
        //quick hack for keeping item colour updated
        private void PlaylistManagerView_Click(object sender, EventArgs e)
        {
            manager.DisplayTrackInfo();
        }

        private void PlaylistManagerView_DragDrop(object sender, DragEventArgs e)
        {
            foreach(string s in InternalClipboard.Files)
            {
                tool.show(3, s);
                if(tool.IsPlaylistFile(s))
                {
                    tool.show(2, s);
                }
            }
        }

        public void Add(VItem item)
        {
            if (item.CurrentColor == null)
                tool.show(5, "Current Color hasn't been set");
            ListViewItem i = new ListViewItem();
            i.Name = item.Name;
            i.Tag = item.Tag;
            i.Selected = item.Selected;
            GetColumnInfo(i, i.Tag); //tag is the playlist
            Items.Add(i);
        }

        public void Insert(int index, VItem item)
        {
            if (item.CurrentColor == null)
            {
                tool.show(5, "Ooops, Current Color hasn't been set");
                return;
            } 
            ListViewItem i = new ListViewItem();
            i.Name = item.Name;
            i.Tag = item.Tag;
            i.Selected = item.Selected;
            GetColumnInfo(i, i.Tag); //tag is the playlist
            Items.Insert(index, i);
        }

        public void Remove(int index)
        {
            Items.RemoveAt(index);
        }

        public void RefreshColors()
        {
            this.Invalidate();
        }

        internal void RemoveSelectedItems()
        {
            List<int> list = GetSelected().ToList();
            BeginUpdate();
            manager.RemoveSelectedPlaylists(list);
            EndUpdate();
        }
    }
}
