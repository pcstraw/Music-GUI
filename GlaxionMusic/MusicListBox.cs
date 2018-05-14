using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;
using System.IO;
using System.Reflection;

namespace Glaxion.Music
{

    public class MusicListBox : ListView
    {
        public Timer tmrLVScroll;
        private System.ComponentModel.IContainer components;
        //private ColumnHeader columnHeader1;
        public int mintScrollDirection;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        const int WM_VSCROLL = 277; // Vertical scroll
        const int SB_LINEUP = 0; // Scrolls one line up
        const int SB_LINEDOWN = 1; // Scrolls one line down
        public ListViewItem _selectedItem;
        public int maxStates = 5;
        public int stateindex;
        public List<List<ListViewItem>> states = new List<List<ListViewItem>>();
           
        public Panel panel;

        private void Construction()
        {
            InitializeComponent();
            // BackColor = Color.Silver;
            ExistColor = BackColor;
            this.DoubleBuffering(true);
            ColumnHeader name = new ColumnHeader();
            name.Width = 150;
            name.Text = "Name";
            ColumnHeader path = new ColumnHeader();
            path.Width = 700;
            path.Text = "Path";

            this.Columns.Add(name);
            this.Columns.Add(path);

            FullRowSelect = true;
            BoxChanged += e_BoxChange;
            DragEnter += e_DragEnter;
            ItemMouseHover += e_ItemMouseHover;
            KeyUp += e_KeyUp;
            stateindex = 0;
        }

        public MusicListBox()
        {
            Construction();
        }
        /*
        public MusicListBox(ListViewItemCollection box)
        {
            Construction();
            CopyFrom(box.g);
        }
        */
        public void CopyFrom(List<ListViewItem> box)
        {
            Items.Clear();
            foreach(ListViewItem i in box)
            {
                Items.Add((ListViewItem)i.Clone());
            }
        }

        public List<ListViewItem> GetItemCollection()
        {
            List<ListViewItem> ic = new List<ListViewItem>();
            foreach (ListViewItem i in Items)
            {
                ic.Add(i.Clone() as ListViewItem);
            }
            return ic;
        }

        //limit max amount hear
        int positionState;
        public void StoreCurrentState()
        {
            // states[stateindex] = null;

            states.Add(GetItemCollection());
            // MessageBox.Show("Message");
            //   MessageBox.Show(state.Count.ToString());
            if (hoveredItem != null)
                positionState = hoveredItem.Index;
            stateindex += 1;
            //  tool.Show(stateindex, " state index : count ", states.Count);
            /*
            if (stateindex > (maxStates-1))
                stateindex = 0;
                */
        }

        public void RestoreLastState()
        {
            //tool.Show();
            if (states.Count == 0)
                return;
            stateindex -= 1;
            if (stateindex < 0)
                stateindex = 0;
           
            //      return;
             // tool.Show("restoring state: ",stateindex);
            CopyFrom(states[stateindex]);
            EnsureVisible(positionState);
           // Items.AddRange(states[StateIndex].Items, 0);
       //     Items.AddRange(states[stateindex]);
       
        }

        public void RestoreNextState()
        {
            if (states.Count == 0)
                return;

            stateindex += 1;
            if (stateindex > states.Count-1)
                stateindex = states.Count - 1;

            
            //      return;
            // tool.Show("restoring state: ",stateindex);
            CopyFrom(states[stateindex]);
            //     Items.AddRange(states[stateindex]);
        }

        protected void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrLVScroll = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrLVScroll
            // 
            this.tmrLVScroll.Tick += new System.EventHandler(this.tmrLVScroll_Tick);
            // 
            // MusicListBox
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ForeColor = System.Drawing.Color.Aqua;
            this.View = System.Windows.Forms.View.Details;
            this.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListBox_ItemCheck);
            this.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.e_ItemDrag);
            this.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListBox_ItemSelectionChanged);
            this.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
            this.Click += new System.EventHandler(this.e_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListBox_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListBox_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ListViewBase_DragOver);
            this.DragLeave += new System.EventHandler(this.ListBox_DragLeave);
            this.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.e_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.e_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
            this.MouseEnter += new System.EventHandler(this.e_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ListBox_MouseLeave);
            this.MouseHover += new System.EventHandler(this.ListBox_MouseHover);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.e_MouseUp);
            this.ResumeLayout(false);

        }

        public void RemoveSelectedItems()
        {
            foreach (ListViewItem i in SelectedItems)
                RemoveItem(i);
        }


        protected virtual void e_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedItems();
            }
        }

        //use this function for copy/adding list view items
        public ListViewItem InsertFileAt(int index,string file)
        {
            ListViewItem item = createItemFromFile(file);
            Items.Insert(index, item);
            return item;
        }
        
        //main one
        public ListViewItem CopyItemTo(int index, ListViewItem item)
        {
            ListViewItem i= (ListViewItem)item.Clone();
            Items.Insert(index, i);
            return i;
        }

        //main one
        public ListViewItem MoveFileTo(int index, string file)
        {
            Items.Remove(Items[index]);
            ListViewItem item = createItemFromFile(file);
            Items.Insert(index, item);
            
            return item;
        }

        public ListViewItem MoveItemTo(int index, ListViewItem item)
        {
            Items.Remove(item);
            Items.Insert(index+1, item);
            return item;
        }

        public int bxh = 25;
        public void HighlightItem(ListViewItem item)
        {
            if (item == null)
                return;
            if (oldhovereditem != null && oldhoveritemcolor != null)
            {
                oldhovereditem.BackColor = oldhoveritemcolor;
                /*
                if(oldhoveritemcolor == MusicPlayer.PlayColor)
                {
                    tool.Show();
                }*/
            }
            oldhovereditem = item;
            oldhoveritemcolor = item.BackColor;

            item.BackColor = tool.AddColor(item.BackColor, bxh);
        }
        private void e_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            Focus();
            hoveredItem = e.Item;
            HighlightItem(hoveredItem);
        }

        private void e_DragEnter(object sender, DragEventArgs e)
        {
            if (!TotalClipboard.IsEmpty)
            {
                //  Thread.Sleep(50);
                e.Effect = DragDropEffects.All;
                //  PreviewPlaylistinTracklistBox(new Playlist(TotalClipboard.Files[0],true));
                return;
            }
            e.Effect = DragDropEffects.Copy;
            return;
        }

        public List<string> GetTracksFromListView()
        {
            List<string> list = new List<string>();
            foreach (ListViewItem li in Items)
            {
                list.Add(li.Tag as string);
            }
            return list;
        }
        private void On_DrawItem(object sender,
        DrawListViewItemEventArgs e)
        {
          //  base.OnDrawItem(e);
        }


        public static ListViewItem CreateAudioItem(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add(new ListViewItem.ListViewSubItem());
            item.SubItems[1].Text = path;
            return item;
        }

        public ListViewItem hoveredItem;
 
        //Next Event handler
        public delegate void BoxChangedHandler(object sender, EventArgs args);
        public event BoxChangedHandler BoxChanged;
        protected virtual void On_ListBoxChange(object sender, EventArgs e)
        {

            // tool.Debug("on list box change");
        }

        public ListViewItem FindItemByPath(string path)
        {
            foreach (ListViewItem item in Items)
            {
                if (item.Tag as string == path)
                {
                    return item;
                }
            }
            return null;
        }



        //main one, use this
        public ListViewItem AddFileAsItem(string s)
        {

            ListViewItem i = createItemFromFile(s);
            Items.Add(createItemFromFile(s));
            return i;
        }

        public ListViewItem createItemFromFile(string s)
        {
            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = Path.GetFileNameWithoutExtension(s);
            i.SubItems.Add(new ListViewItem.ListViewSubItem());

            i.SubItems[1].Text = s;
            i.Tag = s;
            return i;
        }

        public ListViewItem AddItemFromPlaylist(Playlist p)
        {

            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = p.name;
            i.SubItems.Add(new ListViewItem.ListViewSubItem());

            i.SubItems[1].Text = p.path;
            i.Tag = p;
            Items.Add(i);
            return i;
        }

        public Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();

        public Playlist GetCurrentPlaylist()
        {
            if (SelectedItems.Count == 0)
                return null;

            string s = SelectedItems[0].SubItems[1].Text;
            if (currentList == null)
            {
                currentList = new Playlist(s, true);
                Playlists.Add(s, currentList);
                currentList.UpdatePaths();
                return currentList;
            }
            if (currentList.path != s)
            {
                if (Playlists.ContainsKey(s))
                {
                    Playlists[s].UpdatePaths();
                    return Playlists[s];
                }
                else
                {
                    currentList = new Playlist(s, true);
                    currentList.UpdatePaths();
                    Playlists.Add(s, currentList);
                    return currentList;
                }
            }
            return null;
        }

        public bool ContainsFile(string path)
        {
            foreach(ListViewItem i in Items)
            {
                string s = i.SubItems[1].Text;
                if (path == s)
                    return true;

            }
            return false;
        }

        public ColorScheme colorscheme;
        public ColorScheme defaultColorScheme;

        public void UpdateBackColor()
        {
            if (colorscheme != null)
            {
                BackColor = colorscheme.backColor;
                ForeColor = colorscheme.foreColor;
            }
        }
        
        public void AddItem(ListViewItem item)
        {
            this.Items.Add(item);
        //    BoxChanged(this, EventArgs.Empty);
        }

        public void InsertItem(int index, ListViewItem item)
        {
            this.Items.Insert(index, item);
         //   BoxChanged(this, EventArgs.Empty);
        }

        public void RemoveItem(ListViewItem item)
        {
            this.Items.Remove(item);
        //    BoxChanged(this, EventArgs.Empty);
        }

        public void RemoveItems(List<ListViewItem> items)
        {
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                    this.Items.Remove(item);
            //    BoxChanged(this, EventArgs.Empty);
            }
        }

        public void RemoveItemAt(int index)
        {
            this.Items.RemoveAt(index);
           // BoxChanged(this, EventArgs.Empty);
        }

        public void AddItems(List<ListViewItem> items)
        {
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    this.Items.Add(item);
                }

              //  BoxChanged(this, EventArgs.Empty);
            }
        }

        public bool Contains(string path)
        {
            foreach (ListViewItem item in Items)
            {
                if (item.SubItems[1].Text == path)
                    return true;
            }
            return false;
        }


        public Playlist currentList;

        public void CopyFrom(MusicListBox listBox)
        {
            if (listBox == null)
                return;
            Items.Clear();
            foreach (ListViewItem item in listBox.Items)
            {
                ListViewItem i = (ListViewItem)item.Clone();
              //  i.ForeColor = item.ForeColor;
              //  i.BackColor = item.BackColor;
                i.SubItems[0].Tag = item.SubItems[0].Tag;
                i.Selected = item.Selected;
                Items.Add(i);
            }
        }

        /*
        public static MusicListBox CreatePlaylistBox(Playlist playlist, bool updatePaths)
        {
            MusicListBox lb = new MusicListBox();
            lb.AutoUpdateTracks = updatePaths;
            lb.AssignPlaylist(playlist);

            return lb;
        }
        */
        /*
        public void AssignPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                tool.Show("error, playlist is null: ", playlist.name);
                tool.Break();
            }
          //  currentList = playlist;
         //   AssignTracksToBox(playlist.tracks);
        }
        */

        public bool AutoUpdateTracks;
        public enum TrackStatus { Exists, Conflict, Duplicate, Missing, Repeat, WasPlayed, IsPlaying, Null };

        /*
        public void AssignTracksToBox(List<string> tracks)
        {
           // StoreCurrentState();
            Items.Clear();
            foreach (string s in tracks)
            {
                ListViewItem li = new ListViewItem(Path.GetFileNameWithoutExtension(s));
                li.BackColor = BackColor;
                li.ForeColor = ForeColor;
                li.SubItems.Add(new ListViewItem.ListViewSubItem().Text = s);
                Items.Add(li);
                if (!File.Exists(s))
                {
                    li.Tag = TrackStatus.Missing;
                    if (AutoUpdateTracks)
                        UpdateTrackPath(li);
                }
                else
                {
                    li.Tag = TrackStatus.Exists;
                }

                if (li.BackColor == MusicPlayer.PlayColor && !MusicPlayer.IsPlayingTrack(s))
                {
                    RemovePlayColor(li);
                }

                if (MusicPlayer.IsPlayingTrack(s))
                {
                    AssignPlayColor(li);
                }

            }
            /*
            foreach (ListViewItem item in Items)
            {
                CheckForConflicts(item.SubItems[1].Text);
            }
            UpdateTracksStatus();
            StoreCurrentState();
        }
        */

        public string UpdateTrackPath(ListViewItem item)
        {
            string trackname = item.SubItems[0].Text;
            /*
            if (item.BackColor == Color.OrangeRed)
                return;
                */

            if (!MusicPlayer.Get)
                return null;

            List<string> l = MusicPlayer.SearchMusicFiles(trackname);

            if (l.Count > 0)
            {
                AddFileAsItem(l[0]);
                return l[0];
            }else
            {
                if (l.Count == 0)
                {
                    item.Tag = null;
                    item.Text = "";
                    item.SubItems[1].Text = "File Missing";
                    return "File Missing";
                    // tool.Debug("Tracklist Control: No file found: ", item.SubItems[1].Text);
                }
            }
            return "NuLL";
        }

        public void CheckFileConflicts()
        {
            foreach (ListViewItem li in Items)
            {
                if (li.Tag != null && li.Tag is TrackStatus)
                {
                    TrackStatus stat = (TrackStatus)li.Tag;
                    if (stat != TrackStatus.Conflict)
                        UpdateTrackPath(li);
                }

            }
        }

        public void SearchFor(string text, Color col,Color fcol)
        {
            foreach(ListViewItem i in Items)
            {
                string s = i.Tag as string;
                if (tool.StringCheck(s))
                {
                    if (s.ToLower().Contains(text.ToLower()))
                    {
                        i.BackColor = col;
                        i.ForeColor = fcol;
                    }
                    else
                    {
                        ResetBackColor(i, col);
                    }
                }
            }
        }

        public void ResetItemBackColor(ListViewItem item)
        {
            item.BackColor = BackColor;
        }

        public void ResetAllBackColor(Color col)
        {
            foreach(ListViewItem i in Items)
            {
                ResetBackColor(i, col);
            }
        }

        public void ResetBackColor(ListViewItem item, Color col)
        {
            if (item.BackColor == col)
            {
                item.BackColor = BackColor;
                item.ForeColor = ForeColor;
            }
        }

        public void UpdateTrackFilePaths()
        {
            foreach (ListViewItem li in Items)
            {
                // if(!File.Exists(li.SubItems[1].Text))
                UpdateTrackPath(li);
            }
          //  UpdateMusicPlayer();
           // UpdateTracksStatus();
        }

        public Playlist UpdateCurrentListTracks()
        {

            if (currentList == null)
                return null;
            List<string> tracks = GetTracksFromListView();
            currentList.tracks.Clear();
            currentList.tracks = tracks;
            return currentList;
        }



        public ListViewItem GetItemAtMouse(Point point)
        {
            Point tp = PointToClient(point);
            ListViewItem targetItem = GetItemAt(tp.X, tp.Y);
            return targetItem;

        }

       
        /*
        public void CheckForConflicts(string path)
        {
            if (!File.Exists(path))   //do we need check conflicts for broken file paths?
                return;

            string name = Path.GetFileNameWithoutExtension(path);
            List<ListViewItem> dupes = new List<ListViewItem>();
            List<ListViewItem> repeats = new List<ListViewItem>();

            foreach (ListViewItem item in Items)
            {
                if (item.SubItems[0].Text == name)
                {
                    if (item.SubItems[1].Text != path)
                        dupes.Add(item);
                    else
                        repeats.Add(item);
                }
            }

            //if they have the same name but the file path is different
            if (dupes.Count > 1)
            {
                foreach (ListViewItem item in dupes)
                {
                  //  tool.Debug("Music file conflict found :: TrackListControl", item.SubItems[1].Text);
                    item.BackColor = RepeatColor;
                }
            }

            //if two of the same with same path
            if (repeats.Count > 1)
            {
                foreach (ListViewItem item in repeats)
                {
                    item.BackColor = DupeColor;
                }
            }
        }
        */

        private Color previousPlayedItemColor;
        private Color previousPlayedItemForeColor;
        public Color PlayColor = MusicPlayer.PlayColor;

        public void AssignPlayColor(ListViewItem item)
        {
            previousPlayedItemColor = item.BackColor;
            previousPlayedItemForeColor = item.ForeColor;
            item.BackColor = PlayColor;
            item.ForeColor = Color.Black;
            item.Tag = TrackStatus.IsPlaying;
        }

        public void RemovePlayColor(ListViewItem item)
        {
            item.Tag = TrackStatus.WasPlayed;
            item.BackColor = MusicPlayer.PreviousPlayColor;
            item.ForeColor = Color.Black;
            //  tool.Show();
            //  item.BackColor = previousPlayedItemColor;
            //   item.ForeColor = previousPlayedItemColor;
        }

        public ListViewItem targetItem;

        protected void ListViewBase_DragOver(object sender, DragEventArgs e)
        {
            Point position = PointToClient(new Point(e.X, e.Y));
            targetItem = this.GetItemAt(0, position.Y);

            if (position.Y <= (Font.Height / 2))
            {
                // getting close to top, ensure previous item is visible
                mintScrollDirection = SB_LINEUP;
                tmrLVScroll.Enabled = true;
            }
            else if (position.Y >= ClientSize.Height - Font.Height / 2)
            {
                // getting close to bottom, ensure next item is visible
                mintScrollDirection = SB_LINEDOWN;
                tmrLVScroll.Enabled = true;
            }
            else
            {
                tmrLVScroll.Enabled = false;
            }
        }


        public string FilteredText;
        // public SelectedIndexCollection lastSelectedIndices;
        public List<int> lastSelectedIndices = new List<int>();

        public void RestoreSelectedIndices(List<int> indices)
        {
            foreach (int i in indices)
            {
                Items[i].Selected = true;
            }
        }


        public void CacheLastSelectedIndices(MusicListBox box)
        {
            lastSelectedIndices.Clear();
            foreach (int i in box.SelectedIndices)
            {
                //  tool.Debug("Selected index: ", i);
                lastSelectedIndices.Add(i);
            }
        }

        public void checkForRepeats(ListViewItem item)
        {
            if (item.Tag != null)
            {
                TrackStatus st = (TrackStatus)item.Tag;
                if (st == TrackStatus.Repeat || st == TrackStatus.Conflict)
                {
                    //   tool.Debug(0, "track already tagged");
                    return;
                }
            }

            string name = item.SubItems[0].Text;
            string path = item.SubItems[1].Text;

            List<ListViewItem> foundItems = new List<ListViewItem>();
            //tool.Debug(0,tool.Count++);
            foreach (ListViewItem i in Items)
            {
                //check if the name matches
                if (i.SubItems[0].Text == name)
                {
                    foundItems.Add(i);

                }
            }
            if (foundItems.Count > 1)
            {
                foreach (ListViewItem i in foundItems)
                {
                    if (i.SubItems[1].Text == path)
                    {
                        i.Tag = TrackStatus.Repeat;
                    }
                    else
                    {
                        i.Tag = TrackStatus.Conflict;
                    }
                }
            }
            else if (foundItems.Count == 1)
            {

                if (foundItems[0].Tag != null && foundItems[0].Tag is TrackStatus)
                {

                    // tool.Show();
                    TrackStatus stat = (TrackStatus)foundItems[0].Tag;
                    if (stat == TrackStatus.Repeat | stat == TrackStatus.Conflict)
                    {
                        // item.BackColor = BackColor;
                        foundItems[0].Tag = TrackStatus.Exists;
                    }
                }
            }
        }

        public Color RepeatColor = Color.DarkSlateBlue;
        //public Color DupeColor = Color.IndianRed;
        public Color SearchColor = Color.Violet;
        public Color MissingColor = Color.Red;
        public Color ConflictColor = Color.Purple;
        public Color ExistColor;

        string previousTrack;

        public void UpdateTracksStatus()
        {

            string currentTrackName = null;
            string currentPath = null;

            if (MusicPlayer.Get)
            {
                if (File.Exists(MusicPlayer.currentTrack))
                {
                    currentTrackName = Path.GetFileNameWithoutExtension(MusicPlayer.currentTrack);
                    currentPath = MusicPlayer.currentTrack;
                }
            }

            // tool.Debug(0,tool.Count++);
            foreach (ListViewItem item in Items)
            {
                string name = item.SubItems[0].Text;
                string path = item.SubItems[1].Text;
                bool searched = false;
                item.BackColor = BackColor;
                item.ForeColor = ForeColor;
                TrackStatus status = TrackStatus.Null;
                checkForRepeats(item);


                if (item.Tag != null)
                {
                    status = (TrackStatus)item.Tag;
                }

                if (status == TrackStatus.WasPlayed && previousTrack != path)
                {
                    status = TrackStatus.Exists;
                }

                if (status == TrackStatus.IsPlaying && currentPath != path)
                {
                    item.Tag = TrackStatus.WasPlayed;
                    previousTrack = path;
                }

                //check if item is the currently playing track
                if (path == currentPath && currentPath != null)
                {
                    item.Tag = TrackStatus.IsPlaying;
                }

                if (item.SubItems[0].Tag != null)
                {
                    Color group = (Color)item.SubItems[0].Tag;
                    item.ForeColor = group;
                }

                if (item.ForeColor == Color.Black)
                {
                    item.ForeColor = this.ForeColor;
                }

                if (item.Tag != null)
                {
                    switch (status)
                    {
                        /*
                        case TrackStatus.Duplicate:
                            item.BackColor = DupeColor;
                            break;
                            */

                        case TrackStatus.Missing:
                            item.BackColor = MissingColor;
                            break;

                        case TrackStatus.Exists:
                            item.BackColor = ExistColor;
                            break;

                        case TrackStatus.Conflict:
                            item.BackColor = ConflictColor;
                            break;
                        case TrackStatus.Repeat:
                            item.BackColor = RepeatColor;
                            break;
                        case TrackStatus.WasPlayed:
                            {
                                item.ForeColor = Color.Black;
                                item.BackColor = MusicPlayer.PreviousPlayColor;
                            }
                            break;
                        case TrackStatus.IsPlaying:
                            {
                                item.ForeColor = Color.Black;
                                item.BackColor = MusicPlayer.PlayColor;
                            }
                            break;
                        default:
                            break;

                    }
                }

                //apply search color to searched tracks
                if (!string.IsNullOrEmpty(FilteredText))
                {
                    searched = name.ToLower().Contains(FilteredText.ToLower());

                    if (searched)
                    {
                        if (FilteredText.Count() > -1)
                        {
                            if (item.BackColor != MusicPlayer.PlayColor)
                            {
                                item.BackColor = SearchColor;
                                item.ForeColor = Color.Black;
                            }
                        }
                    }
                }
            }
        }

        ListViewItem trackingitem;

        public bool trackup;
        public bool trackDown;
        int scrollBarPos;

        public bool toBottom;

        public void ScrollToBottom()
        {
            mintScrollDirection = SB_LINEDOWN;
            tmrLVScroll.Enabled = true;
            toBottom = true;
        }

        public void ScrollToTop(Point point)
        {

            mintScrollDirection = SB_LINEUP;
            tmrLVScroll.Enabled = true;

        }
        int retraceBottom;
        private void tmrLVScroll_Tick(object sender, EventArgs e)
        {
            if (toBottom == true)
            {
                retraceBottom += 1;
                if (retraceBottom >= scrollBarPos)
                {
                    tmrLVScroll.Enabled = false;
                    retraceBottom = 0;
                    scrollBarPos = 0;

                }
            }
            else if (mintScrollDirection == SB_LINEDOWN && tmrLVScroll.Enabled == true)
            {
                scrollBarPos += 1;
                // tool.Debug(0,scrollBarPos);
            }


            SendMessage(Handle, WM_VSCROLL, (IntPtr)mintScrollDirection, IntPtr.Zero);


        }

        protected void e_BoxChange(object o, EventArgs e)
        {
            tool.debug("base calling boxed changed event");
           //   tool.Show();
          //  MessageBox.Show("");
        }

        public bool BlockInternalDrag;

        private void MoveItem(int index, ListViewItem lvi)
        {
            lvi.Remove();
            this.Items.Insert(index, lvi);
        }

        public void SetDragMode(DragEventArgs e)
        {
            if (!TotalClipboard.IsEmpty)
                return;
            else
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.Move;
            }
        }

        //drag start
        protected virtual void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            // e.Effect = e.AllowedEffect;
            // e.Effect = DragDropEffects.Move;

            SetDragMode(e);
            /*
            if (!TotalClipboard.IsEmpty)
                return;
            else
            {
                e.Effect = e.AllowedEffect;
                e.Effect = DragDropEffects.Move;
            }
            */
        }

        protected virtual void e_ItemDrag(object sender, ItemDragEventArgs e)
        {
            /*
            if (BlockInternalDrag)
                return;
            else
            */
             DoDragDrop(e.Item, DragDropEffects.All);
        }

        protected virtual void ListBox_DragDrop(object sender, DragEventArgs e)
        {
            
            int last = SelectedItems[SelectedItems.Count-1].Index;
             ListViewItem[] list = new ListViewItem[SelectedItems.Count];
            SelectedItems.CopyTo(list, 0);
            List<ListViewItem> li = list.ToList();

            

            if (targetItem.Index > last)
            {
                li.Reverse();
                // list.Reverse();
                //MessageBox.Show(list.Length.ToString());
            }
               
            foreach (ListViewItem lvi in li)
            {
                if (targetItem != null)
                {
                    if (targetItem.Index != lvi.Index)
                        MoveItem(targetItem.Index, lvi);

                }
                else
                    MoveItem(this.Items.Count, lvi);
                
            }
            TotalClipboard.Clear();
          //  tool.Show();
           // tool.Show();

            //   BoxChanged(this, EventArgs.Empty);
            tmrLVScroll.Enabled = false;
        }

        public bool DefaultDragLeavebehaviour = true;

        private void ListBox_DragLeave(object sender, EventArgs e)
        {

            tmrLVScroll.Enabled = false;
            /*
            if(DefaultDragLeavebehaviour)
            {
                //StringBuilder clipData = new StringBuilder();
                if (SelectedItems.Count > 0)
                {
                    tool.TextClipboard.Clear();
                    foreach (ListViewItem item in SelectedItems)
                    {
                        tool.TextClipboard.Add(item.SubItems[1].Text);
                        //clipData.AppendLine(item.SubItems[1].Text);
                        //  MusicPlayer.Clipboard.Add(item.SubItems[1].Text);
                    }
                }

                //tool.Show(clipData.ToString());

               //Clipboard.SetText(clipData.ToString());
              //  tool.Show(clipData.ToString());
                //copy selected items to clipboard
            }
            */



            if (TotalClipboard.IsEmpty)
            {
                if (SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in SelectedItems)
                    {

                        TotalClipboard.Add(item.SubItems[1].Text);
                    }
                    TotalClipboard.SourceListBox = this;
                }
            }
        }

        protected virtual void ListBox_DoubleClick(object sender, EventArgs e)
        {
            //may want to override this behaviour
            //this.SelectedItems[0].BeginEdit();
        }

        private void ListBox_MouseHover(object sender, EventArgs e)
        {

        }

        public bool doubleClicked;

        private void ListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (doubleClicked)
            {
                e.NewValue = e.CurrentValue;
                doubleClicked = false;
            }
        }

        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks > 1))
            {
                doubleClicked = true;
            }
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            doubleClicked = false;
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ListBox_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
        }
        public bool HasFocus;
        private ListViewItem oldhovereditem;
        private Color oldhoveritemcolor;

        protected virtual void e_MouseEnter(object sender, EventArgs e)
        {
            HasFocus = true;
            if (!Focused)
            {
                Focus();
            }
            /*
            foreach (ListViewItem item in Items)
            {
                if (item.BackColor == Color.DarkBlue && item.ForeColor == Color.White)
                {
                    item.BackColor = BackColor;
                    item.ForeColor = ForeColor;
                }
            }
            */
        }

        private void ListBox_MouseLeave(object sender, EventArgs e)
        {
            lastSelectedItems.Clear();
            foreach (int i in SelectedIndices)
            {
                Items[i].BackColor = Color.DarkBlue;
                Items[i].ForeColor = Color.White;
                lastSelectedItems.Add(Items[i]);
            }
        }

        public List<ListViewItem> lastSelectedItems = new List<ListViewItem>();

        private void e_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem i in lastSelectedItems)
            {
                i.BackColor = BackColor;
                i.ForeColor = ForeColor;
            }
        }

        public List<ListViewItem> preContextSelection = new List<ListViewItem>();
        //currentl two places where last selected it being set.  here
        //and in the mouse leaver event.  make sure this doesn't conflict
        protected virtual void e_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                preContextSelection.Clear();
                foreach (ListViewItem i in SelectedItems)
                {
                    preContextSelection.Add(i);
                }
            }
        }

        protected virtual void e_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }

    public static class ControlExtensions
    {
        public static void DoubleBuffering(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}
