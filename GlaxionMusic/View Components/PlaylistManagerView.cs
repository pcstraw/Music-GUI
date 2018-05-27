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
        private ColumnHeader columnHeader1;
        new public PlaylistManager manager;

        public PlaylistManagerView()
        {
            InitializeComponent();
            //CheckBoxes = true;
            AllowDrop = true;
            Columns[1].Width = 1000;
            AssignEventHandlers();
            manager = new PlaylistManager(this,this,new ColorScheme(ForeColor,BackColor));
        }

        private new void InitializeComponent()
        {
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Ext";
            // 
            // PlaylistManagerView
            // 
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Click += new System.EventHandler(this.PlaylistManagerView_Click);
            this.ResumeLayout(false);

        }
        
        public void AssignEventHandlers()
        {
            MusicPlayer.Player.TrackChangeEvent += PlaylistManager_TrackChangeEvent;
            this.AfterLabelEdit += PlaylistManager_AfterLabelEdit;
            this.DragOver += PlaylistManager_DragOver;
            this.DragEnter += PlaylistManager_DragEnter;
            //this.DragDrop += PlaylistManager_DragDrop;
            this.DragLeave += PlaylistManager_DragLeave;
           // Font = new Font(CustomFont.Exo.ff, Font.Size);
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
        
        internal void Load()
        {
            manager.CurrentColorScheme = new ColorScheme(ForeColor,BackColor);
            manager.LoadManager();
            AssignEventHandlers();
            //RefreshList(manager.Items);
        }
        
        protected override void ViewBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!InternalClipboard.IsEmpty)
            {
               // SelectedItems.Clear();
                foreach (string file in InternalClipboard.Files)
                {
                    Point cp = PointToClient(new Point(e.X, e.Y));
                    ListViewItem dragToItem = GetItemAt(cp.X, cp.Y);
                    int dropindex = manager.ItemCount;
                    if (dragToItem != null)
                        dropindex = dragToItem.Index;
                    VItem i = manager.AddPlaylistFromPath(dropindex,file);
                    if (i == null)
                        continue;
                    i.Selected = true;
                }
                InternalClipboard.Files.Clear();
                //UpdateList();
                return;
            }
            else
            {
                base.ViewBox_DragDrop(sender, e);
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

        private void PlaylistManager_DragOver(object sender, DragEventArgs e)
        {
            hoveredItem = GetItemAtPoint(new Point(e.X, e.Y));
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

        protected override void e_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                List<int> list = GetSelected().ToList();
                
                manager.RemoveSelectedPlaylists(list);
            }
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

        public void RefreshUI()
        {
            //RefreshList(manager.Items);
            for(int i=0;i<manager.Items.Count;i++)
            {
                ListViewItem item = Items[i];
                item.ForeColor = manager.Items[i].CurrentColor.foreColor;
                item.BackColor = manager.Items[i].CurrentColor.backColor;
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
            ListViewItem i = new ListViewItem();
            i.Name = item.Name;
            i.Tag = item.Tag;
            GetColumnInfo(i, i.Tag); //tag is the playlist
            Items.Add(i);
        }

        public void Insert(int index, VItem item)
        {
            ListViewItem i = new ListViewItem();
            i.Name = item.Name;
            i.Tag = item.Tag;
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
    }
}
