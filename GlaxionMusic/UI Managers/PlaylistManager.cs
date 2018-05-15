using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public class PlaylistManager : ViewBox
    {
        public PlaylistManager()
        {
            CheckBoxes = true;
            AllowDrop = true;
        }

        private new void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        public Dictionary<string, string> tmpPlaylists = new Dictionary<string, string>();
        public Color ContainsTrackColor = Color.SkyBlue;
        public bool ClearTMPFiles = false;
        public Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();
        public ListViewItem lastWaitSlectedItem;
        public int hoverSelectThreshold = 25;

        public ListViewItem FindPlaylistItem(Playlist list)
        {
            foreach (ListViewItem item in Items)
            {
                Playlist p = item.Tag as Playlist;
                if (p != null && p == list)
                    return item;
            }
            return null;
        }

        public ListViewItem FindPlaylistItem(string path)
        {
            foreach (ListViewItem item in Items)
            {
                Playlist p = item.Tag as Playlist;
                if (p != null && p.path == path)
                    return item;
            }
            return null;
        }

        //path to the playlist file
        public ListViewItem AddPlaylistFromPath(string path)
        {
            if (!File.Exists(path) || !tool.IsPlaylistFile(path))
                return null;
            string name = Path.GetFileNameWithoutExtension(path);
            if (Playlists.ContainsKey(path))
            {
                tool.show(1, "", name, "", " already added to the list", "", path);
                ListViewItem item = FindPlaylistItem(path);
                if (item == null)
                    tool.show(1, "", name, "", "...something might be wrong, playlist is in the list but there is no viewBox item", "", path);
                return item;
            }

            Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(path, true);
            ListViewItem i = AddItemFromPlaylist(p);
            return i;
        }

        public ListViewItem AddItemFromPlaylist(Playlist p)
        {
            if (p.path != ""&& Playlists.ContainsKey(p.path))
            {
                tool.show(1, "", p.name, "", " already contained in list", "", p.path);
                ListViewItem item = FindPlaylistItem(p);
                if (item == null)
                    tool.show(1, "", p.name, "", "...something is wrong, playlist is in list but their is no viewBox item", "", p.path);
                return null;

            }
            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = p.name;
            i.SubItems.Add(new ListViewItem.ListViewSubItem());
            i.SubItems[1].Text = p.path;
            i.Tag = p;
            Items.Insert(0, i);
            if(p.path != "")
                Playlists.Add(p.path, p);
            return i;
        }

        public Playlist SavePlaylist(string s)
        {
            Playlist p = MusicPlayer.Player.fileLoader.GetPlaylist(s, false);
            p.Save();
            return p;
        }

        public void DeleteSelectedPlaylists(bool AllowConfirmation)
        {
            foreach (ListViewItem i in SelectedItems)
            {
                Playlist p = i.Tag as Playlist;
                if (p == null)
                    continue;

                if (File.Exists(p.path))
                {
                    if (AllowConfirmation)
                    {

                        bool result = tool.askConfirmation(p.path);
                        if (!result)
                            continue;
                    }

                    MusicPlayer.Player.fileLoader.RemovePlaylist(p);
                    File.Delete(p.path);
                    tool.show(2, "File deleted:", p.path);
                }
                RemoveItem(i);
            }
        }

        public void SaveAll()
        {
            foreach (ListViewItem i in Items)
            {
                Playlist p = i.Tag as Playlist;
                p.Save();
            }
        }

        public void SaveChecked()
        {
            foreach (ListViewItem i in CheckedItems)
            {
                Playlist p = i.Tag as Playlist;
                p.Save();
            }
        }

        public void LoadTmpPlaylists()
        {
            string directory = "Output\\tmp\\";
            if (Directory.Exists(directory)) //does the directory exist?
            {
                List<string> files = tool.LoadFiles(directory, ".m3u"); //if so, grab the files

                //search for corresponding playlist
                foreach (string file in files) //load each file into the listview
                {
                    Playlist tmp = new Playlist(file, true);
                    AddItemFromPlaylist(tmp);
                }
            }
        }

        [DllImport("user32")]
        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, bool bShow);
        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const uint ESB_DISABLE_BOTH = 0x3;
        private const uint ESB_ENABLE_BOTH = 0x0;

        private void HideHorizontalScrollBar()
        {
           this.Scrollable = false;
            ShowScrollBar(this.Handle, (int)SB_VERT,true);
            //this.Scrollable = true;
        }

        public void LoadManager()
        {
            MusicPlayer.Player.fileLoader.PlaylistDirectories = tool.GetPropertyList(Properties.Settings.Default.PlaylistDirectories);
            LoadTmpPlaylists();
            AssignEventHandlers();
            //HideHorizontalScrollBar();
        }

        public void AssignEventHandlers()
        {
            MusicPlayer.Player.TrackChangeEvent += PlaylistManager_TrackChangeEvent;
            this.AfterLabelEdit += PlaylistManager_AfterLabelEdit;
            this.DragOver += PlaylistManager_DragOver;
            this.DragEnter += PlaylistManager_DragEnter;
            this.DragDrop += PlaylistManager_DragDrop;
            this.DragLeave += PlaylistManager_DragLeave;
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

        private void PlaylistManager_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        //recursively check the dictionary until
        //we have generated a new name
        public Playlist AppendPlaylistPath(Playlist p)
        {
            if (Playlists.ContainsKey(p.path))
            {
                string path = p.path;
                string dir = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                name += "+";
                path = string.Concat(dir, @"\", name, ext);
                p.name = name;
                p.path = path;
                AppendPlaylistPath(p);
            }
            else
            {
                return p;
            }
            return p;
        }

        public void DisplayTrackInfo()
        {
            foreach (ListViewItem item in Items)
            {
                string name = item.SubItems[0].Text;
                string path = item.SubItems[1].Text;
                Playlist p = item.Tag as Playlist;
                if (p != null)
                {
                    if (p.tracks.Contains(MusicPlayer.Player.currentTrackString))
                    {
                        item.BackColor = ContainsTrackColor;
                    }
                    else if (item.BackColor == ContainsTrackColor)
                    {
                        item.BackColor = BackColor;
                    }
                }
                if (MusicPlayer.Player.playlist != null && MusicPlayer.Player.playlist.name == name)
                {
                    item.BackColor = Color.Yellow;
                    item.ForeColor = Color.Black;
                }
                else if (item.BackColor == Color.Yellow)
                {
                    item.BackColor = BackColor;
                    item.ForeColor = Color.Black;
                }
            }
        }

        public void SendToCheckedPlaylists(string file)
        {
            foreach (ListViewItem item in CheckedItems)
            {
                Playlist p = item.Tag as Playlist;
                p.dirty = true;
                p.tracks.Insert(0, file);
            }
        }
        protected override void ViewBox_DragDrop(object sender, DragEventArgs e)
        {
            if (!TotalClipboard.IsEmpty)
            {
                SelectedItems.Clear();
                foreach (string file in TotalClipboard.Files)
                {
                    ListViewItem i = AddPlaylistFromPath(file);
                    if (i == null)
                        continue;
                    i.Selected = true;
                }
                TotalClipboard.Clear();
                return;
            }
            else
            {
                base.ViewBox_DragDrop(sender, e);
            }
        }

        private void PlaylistManager_DragEnter(object sender, DragEventArgs e)
        {
            // tool.show();
            if (!TotalClipboard.IsEmpty)
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
            ListViewItem targetItem = GetItemAtPoint(new Point(e.X, e.Y));
        }

        private void PlaylistManager_TrackChangeEvent(object sender, EventArgs args)
        {
            DisplayTrackInfo();
        }

        public void SaveTmp()
        {
            string directory = "Output\\tmp\\";
            System.IO.DirectoryInfo di;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            di = new DirectoryInfo(directory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            if (ClearTMPFiles)
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }

            foreach (ListViewItem item in Items)
            {
                Playlist p = item.Tag as Playlist;
                if (p != null)
                {
                    string path = directory + p.name + ".m3u";
                    p.path = path;
                    tool.debug("Save tmp: " + path);
                    p.debugSave = false;
                    p.SaveTo(path);
                }
            }
        }

        public void ChangeItemName(ListViewItem item, string text)
        {
            if (item != null)
            {
                Playlist p = item.Tag as Playlist;
                string oldpath = p.path;
                p.UpdateName(text);
                item.Text = p.name;
                item.SubItems[0].Text = p.name;
                item.SubItems[1].Text = p.path;
                MusicPlayer.WinFormApp.musicControl.UpdateDockedPlaylistNames();
            }
        }
        
        public void OpenHoveredPlaylistDirectory()
        {
            Playlist p = hoveredItem.Tag as Playlist;
            if (p != null)
                tool.OpenFileDirectory(p.path);
        }

        private void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            DisplayTrackInfo();
        }

        protected override void e_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedPlaylists();
            }
        }

        protected override void e_MouseEnter(object sender, EventArgs e)
        {
            DisplayTrackInfo();
        }

        public void RemovePlaylist(ListViewItem item)
        {
            Playlist p = item.Tag as Playlist;
            if (p != null)
            {
                if (Playlists.ContainsKey(p.path))
                    Playlists.Remove(p.path);
            }
            tool.debug(3, "Playlist " + p.name + " removed from Playlist View");
            item.Remove();
        }

        public void RemoveSelectedPlaylists()
        {
            foreach (ListViewItem i in SelectedItems)
            {
                RemovePlaylist(i);
            }
        }

        private void PlaylistManager_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            LabelEdit = false;
            if (e.Label == null || e.Label == "")
            {
                e.CancelEdit = true;
                return;
            }

            ListViewItem item = Items[e.Item];
            ChangeItemName(item, e.Label);
        }
    }
}
