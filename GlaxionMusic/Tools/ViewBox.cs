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
using item_ = System.Windows.Forms.ListViewItem;

namespace Glaxion.Music
{
    public class EnhancedListView : System.Windows.Forms.ListView
    {

        [DllImport("user32")]
        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, bool bShow);
        private const uint SB_HORZ = 0;
        private const uint SB_VERT = 1;
        private const uint ESB_DISABLE_BOTH = 0x3;
        private const uint ESB_ENABLE_BOTH = 0x0;

        public void HideHorizontalScrollBar()
        {
            this.Scrollable = false;
            ShowScrollBar(this.Handle, (int)SB_VERT, true);
        }
        
        private System.ComponentModel.IContainer components;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        const int WM_VSCROLL = 277; // Vertical scroll
        const int SB_LINEUP = 0; // Scrolls one line up
        const int SB_LINEDOWN = 1; // Scrolls one line down
        internal ListViewItem _selectedItem;
        public  ListViewItem hoveredItem;
        private ListViewItem oldhovereditem;
        public List<List<ListViewItem>> states = new List<List<ListViewItem>>();
        public List<ListViewItem> lastSelectedItems = new List<ListViewItem>();
        public List<ListViewItem> preContextSelection = new List<ListViewItem>();
        public List<int> lastSelectedIndices = new List<int>();
        public Panel panel;
        public Timer tmrLVScroll;
        int retraceBottom;
        int scrollBarPos;
        public int mintScrollDirection;
        public int maxStates;
        public int stateindex;
        public int visibleIndex;
        public int lastVisibleIndex;
        public int bxh; //item highlight multiplier for mouse hover
        int positionState;
        public bool trackup;
        public bool trackDown;
        public bool toBottom;
        public bool HasFocus;
        public bool doubleClicked;
        public bool AutoUpdateTracks;
        public string FilteredText;
        public Color PlayColor;
        public Color RepeatColor = Color.DarkSlateBlue;
        public Color SearchColor = Color.Violet;
        public Color MissingColor = Color.Red;
        public Color ConflictColor = Color.Purple;
        private Color oldhoveritemcolor;
        public ColorScheme defaultColorScheme;
        public ListViewItem targetItem;

        private void Construction()
        {
            InitializeComponent();
            
            this.DoubleBuffering(true);
            ColumnHeader name = new ColumnHeader();
            name.Width = 250;
            name.Text = "Name";
            this.Columns.Add(name);

            ColumnHeader path = new ColumnHeader();
            path.Width = 250;
            path.Text = "Path";
            this.Columns.Add(path);
            this.View = View.Details;

            maxStates = 5;
            stateindex = 0;
            bxh = 25;
            FullRowSelect = true;
            PlayColor = MusicPlayer.PlayColor;

            DragEnter += e_DragEnter;
            ItemMouseHover += e_ItemMouseHover;
            KeyUp += e_KeyUp;
            DragLeave += e_DragLeave;
        }


        public VItem GetVListItem(ListViewItem item)
        {
            VItem v_item = new VItem();
            v_item.Columns.Add(item.SubItems[0].Text);
            v_item.Columns.Add(item.SubItems[1].Text);
            v_item.SetColors(new ColorScheme(ForeColor, BackColor));
            v_item.Checked = item.Checked;
            v_item.Selected = item.Selected;
            v_item.Tag = item.Tag;
            return v_item;
        }

        public void RefreshList(List<VItem> VItems)
        {
            Items.Clear();
            SelectedItems.Clear();
            foreach (VItem i in VItems)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems[0].Text = i.Columns[0];
                item.SubItems.Add(new ListViewItem.ListViewSubItem());
                item.SubItems[1].Text = i.Columns[1];
                item.Checked = i.Checked;
                item.Selected = i.Selected;
                item.BackColor = i.CurrentColor.backColor;
                item.ForeColor = i.CurrentColor.foreColor;
                item.Tag = i.Tag;
                Items.Add(item);
            }
        }

        public void InvertSelection()
        {
            foreach (item_ i in Items)
                i.Selected = !i.Selected;
        }

        public EnhancedListView()
        {
            Construction();
        }
        
        public int FirstVisible()
        {
            int i = 1;
            try
            {
                while (GetItemRect(i).X != 0) i++;
            }
            catch
            {
                return 0;
            }
            int rowWidth = i;
            int rowHeight = GetItemRect(i).Y - GetItemRect(0).Y;
            return -((int)GetItemRect(0).Y / rowHeight) * rowWidth;
        }
        
        public int LastVisible()
        {
            if (TopItem == null)
                return 0;
            int first = TopItem.Index;
            int h_tot = ClientRectangle.Height - 1;
            int h_hdr = GetItemRect(first).Y; // Height of ColumnHeader, if any
            int h_item = GetItemRect(0).Height; // Height of a single item
            int cntVis = (h_tot - h_hdr) / h_item; // Number of visible items
            int last = Math.Min(Items.Count - 1, first + cntVis); // Index of last (partially) visible item
            return last;
        }

        public void SetColumnTitle(string s)
        {
            if (this.Columns.Count > 0)
                this.Columns[0].Text = s;
        }

        public string GetTitle()
        {
            if (this.Columns.Count>0)
                return this.Columns[0].Text;
            else
                return null;
        }
        
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
        
        public void StoreCurrentState()
        {
            states.Add(GetItemCollection());
            if (hoveredItem != null)
                positionState = hoveredItem.Index;
            stateindex += 1;
        }

        public void RestoreLastState()
        {
            tool.show(3, "Implement proper undo/redo");
            if (states.Count == 0)
                return;
            stateindex -= 1;
            if (stateindex < 0)
                stateindex = 0;
            CopyFrom(states[stateindex]);
            EnsureVisible(positionState);
        }

        public void RestoreNextState()
        {
            tool.show(3, "Implement proper undo/redo");
            if (states.Count == 0)
                return;

            stateindex += 1;
            if (stateindex > states.Count-1)
                stateindex = states.Count - 1;
            CopyFrom(states[stateindex]);
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
            // ViewBox
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ForeColor = System.Drawing.Color.Aqua;
            this.View = System.Windows.Forms.View.Details;
            this.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListBox_ItemCheck);
            this.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.e_ItemDrag);
            this.Click += new System.EventHandler(this.e_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ViewBox_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListBox_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ListViewBase_DragOver);
            this.DragLeave += new System.EventHandler(this.e_DragLeave);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBox_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.e_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
            this.MouseEnter += new System.EventHandler(this.e_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ListBox_MouseLeave);
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
                RemoveSelectedItems();
        }

        //use this function for copy/adding list view items
        public ListViewItem InsertFileAt(int index,string file)
        {
            ListViewItem item = CreateItemFromFile(file);
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
            ListViewItem item = CreateItemFromFile(file);
            Items.Insert(index, item);
            return item;
        }

        public ListViewItem MoveItemTo(int index, ListViewItem item)
        {
            Items.Remove(item);
            Items.Insert(index+1, item);
            return item;
        }
        
        public void HighlightItem(ListViewItem item)
        {
            if (item == null)
                return;
            if (oldhovereditem != null && oldhoveritemcolor != null)
                oldhovereditem.BackColor = oldhoveritemcolor;

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
                e.Effect = DragDropEffects.All;
                return;
            }
            e.Effect = DragDropEffects.Copy;
            return;
        }

        public List<string> GetTrackItems()
        {
            List<string> list = new List<string>();
            foreach (ListViewItem li in Items)
            {
                list.Add(li.Tag as string);
            }
            return list;
        }
        
        public static ListViewItem CreateAudioItem(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add(new ListViewItem.ListViewSubItem());
            item.SubItems[1].Text = path;
            return item;
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
            ListViewItem i = CreateItemFromFile(s);
            Items.Add(i);
            return i;
        }

        public static ListViewItem CreateItemFromFile(string s)
        {
            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = Path.GetFileNameWithoutExtension(s);
            i.SubItems.Add(new ListViewItem.ListViewSubItem());
            i.SubItems[1].Text = s;
            i.Name = s;
            i.Tag = s;
            return i;
        }
        /*
        public void AddItem(ListViewItem item)
        {
            this.Items.Add(item);
        //    BoxChanged(this, EventArgs.Empty);
        }
        */

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
        
        public void CopyFrom(EnhancedListView listBox)
        {
            if (listBox == null)
                return;
            Items.Clear();
            foreach (ListViewItem item in listBox.Items)
            {
                ListViewItem i = (ListViewItem)item.Clone();
                //i.SubItems[0].Tag = item.SubItems[0].Tag;
                //i.Selected = item.Selected;
                Items.Add(i);
            }
        }
        
        public string UpdateTrackPath(ListViewItem item)
        {
            if (MusicPlayer.Player != null)
                return null;

            string trackname = item.SubItems[0].Text;
            List<string> l = MusicPlayer.Player.SearchMusicFiles(trackname);

            if (l.Count > 0)
            {
                AddFileAsItem(l[0]);
                return l[0];
            }else
            {
                if (l.Count == 0)
                {
                    item.Tag = trackname;
                    item.Text = Path.GetFileNameWithoutExtension(trackname);
                    item.BackColor = Color.Orange;
                    item.SubItems[1].Text = trackname;
                    return "File Missing";
                    // tool.Debug("Tracklist Control: No file found: ", item.SubItems[1].Text);
                }
            }
            return "NuLL";
        }
        
        //highlight searched items with the provided colors
        public void SearchFor(string text, Color backColour,Color foreColour)
        {
            foreach(ListViewItem i in Items)
            {
                string s = i.Tag as string;
                if (tool.StringCheck(s))
                {
                    if (s.ToLower().Contains(text.ToLower()))
                    {
                        i.BackColor = backColour;
                        i.ForeColor = foreColour;
                    }
                    else
                        ResetBackColor(i, backColour);
                }
            }
        }

        //restore all the colours
        public void ResetAllBackColor(Color col)
        {
            foreach(ListViewItem i in Items)
                ResetBackColor(i, col);
        }
        //reset color of the item
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
                UpdateTrackPath(li);
            }
        }
        
        public ListViewItem GetItemAtPoint(Point point)
        {
            Point tp = PointToClient(point);
            ListViewItem targetItem = GetItemAt(tp.X, tp.Y);
            return targetItem;
        }
       
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
        
        public void RestoreSelectedIndices(List<int> indices)
        {
            foreach (int i in indices)
            {
                Items[i].Selected = true;
            }
        }
        
        public void CacheLastSelectedIndices(EnhancedListView box)
        {
            lastSelectedIndices.Clear();
            foreach (int i in box.SelectedIndices)
                lastSelectedIndices.Add(i);
        }
        
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
            }
            SendMessage(Handle, WM_VSCROLL, (IntPtr)mintScrollDirection, IntPtr.Zero);
        }

        protected void e_BoxChange(object o, EventArgs e)
        {
            tool.debug("base calling boxed changed event");
        }
        
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
            SetDragMode(e);
        }

        protected virtual void e_ItemDrag(object sender, ItemDragEventArgs e)
        {
             DoDragDrop(e.Item, DragDropEffects.All);
        }

        protected virtual void ViewBox_DragDrop(object sender, DragEventArgs e)
        {
            int last = 0;
            if (SelectedItems.Count > 0)
            {
                last = SelectedItems[SelectedItems.Count - 1].Index;
            }
            //seems a bit unnecessary just so wew can reverse the list
            //write function to reverse list instead
            ListViewItem[] list = new ListViewItem[SelectedItems.Count];
            SelectedItems.CopyTo(list, 0);
            List<ListViewItem> li = list.ToList();
            if(targetItem == null)
            {
               // tool.show(100, "error: handle case where targetItem is null");
                return;
            }
            if (targetItem.Index > last)
                li.Reverse();
               
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
            //   BoxChanged(this, EventArgs.Empty);
            tmrLVScroll.Enabled = false;
        }
        
        private void e_DragLeave(object sender, EventArgs e)
        {
            tmrLVScroll.Enabled = false;
            if (TotalClipboard.IsEmpty)
            {
                if (SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in SelectedItems)
                        TotalClipboard.Add(item.SubItems[1].Text);

                   // TotalClipboard.Files.Reverse();
                }
            }
        }
        
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
                doubleClicked = true;
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            doubleClicked = false;
        }
        
        protected virtual void e_MouseEnter(object sender, EventArgs e)
        {
            HasFocus = true;
            if (!Focused)
            {
                Focus();
                //restore last selected items to their normal color
                foreach (int i in SelectedIndices)
                {
                    // Items[i].BackColor = this.BackColor;
                    //Items[i].ForeColor = this.ForeColor;
                    Items[i].SubItems[1].Tag = 1;
                    Items[i].Selected = true;
                   // lastSelectedItems.Add(Items[i]);
                }
            }
        }

        private void ListBox_MouseLeave(object sender, EventArgs e)
        {
            lastSelectedItems.Clear();
            //remeber the last selected items as we leave the playlist
            foreach (int i in SelectedIndices)
            {
                Items[i].SubItems[1].Tag = null;
                // Items[i].BackColor = Color.DarkBlue;
                //Items[i].ForeColor = Color.White;  //white colour isn't being retained after leaving
                lastSelectedItems.Add(Items[i]);  //store last selected items
            }
        }
        
        private void e_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem i in lastSelectedItems)
            {
                i.BackColor = BackColor;
                i.ForeColor = ForeColor;
            }
        }
        
        protected virtual void e_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (ListViewItem i in preContextSelection)
                    i.BackColor = this.BackColor;
                preContextSelection.Clear();
                foreach (ListViewItem i in SelectedItems)
                    preContextSelection.Add(i);
            }
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
