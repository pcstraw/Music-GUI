﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Glaxion.Tools;

namespace Glaxion.Music
{
    public enum ItemState { Normal, IsPlaying, WasPlaying, WasSelected, Missing, Reset, IsPlayingInOtherPanel,
        IsThePlayingTrack
    }
    public delegate void TracksChangedDelegate();
    public class VColumn
    {
        public VColumn(string aspect)
        {
            AspectName = aspect;
        }

        public string AspectName
        {
            get { return aspectName; }
            set
            {
                aspectName = value;
                this.aspectMunger = null;
            }
        }
        private string aspectName;
        public object GetAspectByName(object rowObject)
        {
            if (this.aspectMunger == null)
                this.aspectMunger = new Munger(this.AspectName);

            return this.aspectMunger.GetValue(rowObject);
        }
        private Munger aspectMunger;

        public object GetValue(object rowObject)
        {
          //  if (this.AspectGetter == null)
                return this.GetAspectByName(rowObject);
            //else
            //    return this.AspectGetter(rowObject);
        }

       // [Category("ObjectListView"),
        // Description("The format string that will be used to convert an aspect to its string representation"),
        // DefaultValue(null)]
        public string AspectToStringFormat
        {
            get { return aspectToStringFormat; }
            set { aspectToStringFormat = value; }
        }
        private string aspectToStringFormat;
        

        public string ValueToString(object value)
        {
            // Give the installed converter a chance to work (even if the value is null)
           // if (this.AspectToStringConverter != null)
            //    return this.AspectToStringConverter(value) ?? String.Empty;

            // Without a converter, nulls become simple empty strings
            if (value == null)
                return String.Empty;

            string fmt = this.AspectToStringFormat;
            if (String.IsNullOrEmpty(fmt))
                return value.ToString();
            else
                return String.Format(fmt, value);
        }

    }


    public class VNode
    {
        public VNode()
        {
        }
        public VNode(string text)
        {
            Text = text;
        }
        public List<VNode> Nodes = new List<VNode>();
        //public object Tag;
        public string Text;
        //public string path;
        public string name;
        public bool Expand;
        public bool selected;
        public bool isFile;
        public Color ForeColor;
        public Color BackColor;

        internal VNode Clone()
        {
            VNode node = new VNode(Text);
            node.name = name;
            node.Expand = Expand;
            node.isFile = isFile;
            node.selected = selected;

            foreach(VNode v in Nodes)
            {
                node.Nodes.Add(v.Clone());
            }
            return node;
        }
    }

    public interface ITreeView
    {
        void PopulateTree(List<VNode> tree);
    }

    public class VTree
    {
        //generates an error in derived classes

        /*
        public VTree(ITreeView TreeViewInterface)
        {
            _view = TreeViewInterface;
        }
        */

        public List<VNode> Nodes = new List<VNode>();
        public List<VNode> cachedNodes = new List<VNode>();
        public List<VNode> SelectedNodes = new List<VNode>();
        public string searchText;
        private bool _useCahcedNodes;
        protected ITreeView _view;
        /*
        public void CacheNodes()
        {
            cachedNodes.Clear();
            cachedNodes = null;
            cachedNodes = CopyTree();
        }
        */

        static void IterateTreeNodes(List<VNode> originalNode, List<VNode> rootNode)
        {
            foreach (VNode childNode in originalNode)
            {
                VNode newNode = childNode.Clone();
                IterateTreeNodes(childNode.Nodes, newNode.Nodes);
                rootNode.Add(newNode);
            }
        }

        public void CacheNodes()
        {
            cachedNodes.Clear();
            IterateTreeNodes(Nodes, cachedNodes);
        }
        public List<VNode> CopyTree()
        {
            List<VNode> copied = new List<VNode>();
            foreach (VNode n in Nodes)
            {
                copied.Add(n.Clone());
            }
            return copied;
        }

        public void SearchForText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (!_useCahcedNodes)
                {
                    _view.PopulateTree(cachedNodes);
                    _useCahcedNodes = true;
                    //TODO: restore last opened nodes
                    if (Nodes.Count > 0)
                    {
                        Nodes[0].Expand = true;
                    }
                }
                return;
            }
            List<VNode> results = new List<VNode>();
            SearchTree(text, cachedNodes, results);
            if (results.Count > 0)
            {
                _view.PopulateTree(results);
                _useCahcedNodes = false;
            }
        }
        
        //use ref for results to show we are modifying the list
        public void SearchTree(string text, List<VNode> nodes, List<VNode> results)
        {
            foreach (VNode t in nodes)
            {
                if (t.Text.ToLower().Contains(text.ToLower()))
                {
                    results.Add(t.Clone());
                }
                SearchTree(text, t.Nodes, results);
            }
        }

        public void SetTree(List<VNode> nodes, string SearchText)
        {
            foreach (VNode n in nodes)
            {
                Nodes.Add(n);
                if (tool.StringCheck(SearchText))
                    SearchForText(SearchText);
            }
            _view.PopulateTree(Nodes);
        }
        
        public void SetTree(VNode node, string searchText)
        {
            if (node == null)
                return;
            Nodes.Add(node);
            if (tool.StringCheck(searchText))
                SearchForText(searchText);
            _view.PopulateTree(Nodes);
        }
    }
    
    //virtual listbox class
    //used to represent a windows form ListView
    public class VItem
    {
        public VItem()
        {
            CurrentColor = new ColorScheme(Color.Black,Color.White);
            OldColor = CurrentColor;
            Columns.Add("name");
            Columns.Add("path");
        }
        public int Index; //dep
        public ColorScheme CurrentColor;
        public ColorScheme OldColor;
        public List<string> Columns = new List<string>();
        public object Tag;
        public ItemState State;
        public bool Selected;
        public bool Checked;
        public string Name;

        public void HighLightColors(ColorScheme scheme)
        {
            OldColor = CurrentColor;
            CurrentColor = scheme;
        }
        public void RestoreColors()
        {
            CurrentColor = OldColor;
        }
        public void SetColors(ColorScheme scheme)
        {
            CurrentColor = scheme;
            OldColor = scheme;
        }

        internal VItem Clone()
        {
            VItem item = new VItem();
            item.Name = Name;
            item.Columns[0] = Columns[0];
            item.Columns[1] = Columns[1];
            item.Tag = Tag;
            item.CurrentColor = CurrentColor;
            item.OldColor = OldColor;
            item.Index = Index;
            item.Selected = Selected;
            item.State = State;
            item.Checked = Checked;
            return item;
        }
    }

    public interface IListView
    {
        void Add(VItem item);
        void Insert(int index, VItem item);
        void RefreshColors();
        void Remove(int index);
       // void Remove(VItem item);
    }

    public class VListView
    {
        public VListView(IListView ListViewInterface)
        {
            SetListViewInterface(ListViewInterface);
        }
        void SetListViewInterface(IListView ListViewInterface)
        {
            _view = ListViewInterface;
            _items = new List<VItem>();
        }
        protected IListView _view;
        List<VItem> _items;
        //public List<int> SelectedIndices = new List<int>();
        public List<VItem> CheckedItems = new List<VItem>();
        public ColorScheme CurrentColors;
        public TracksChangedDelegate tracksChangedDelegate;
        public virtual VItem CreateItem(string file)
        {
            VItem i = new VItem();
            i.Columns[0] = Path.GetFileNameWithoutExtension(file);
            i.Columns[1] = file;
            i.SetColors(CurrentColors);
            i.Name = file;
            Song song = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(file);
            if (song != null)
            {
                i.Tag = song;
            }
            return i;
        }

        public VItem Add(VItem item)
        {
            item.Index = _items.Count;
            _items.Add(item);
            _view.Add(item);
            return item;
        }

        public VItem Insert(int index, VItem item)
        {
            _items.Insert(index, item);
            _view.Insert(index, item);
            //SortIndices(index);
            return item;
        }

        public VItem Remove(int index)
        {

            VItem item = _items[index];
            _items.Remove(item);
            _view.Remove(index);
            //SortIndices(index);
            return item;
        }
        public VItem Remove(VItem item)
        {
            if (!_items.Contains(item))
            {
                tool.show(5, "Crash averted:  getting the index of an item not in Items");
                //return null;
            }
            int index = IndexOf(item);
            _view.Remove(index);
            _items.Remove(item);
            return item;
        }

        //dep?
        public void MoveIndicesTo(int insertionIndex,List<int> indices)
        {
            List<VItem> removedItems = new List<VItem>();
            indices.Reverse();
            ClearSelection();
            foreach (int i in indices)
            {
                removedItems.Add(_items[i]);
            }

            int _i = insertionIndex;
            if (insertionIndex >= ItemCount)
                _i = ItemCount-1;
            if (insertionIndex < 0)
                _i = 0;
            

            VItem cache_index = _items[_i];
            foreach (VItem i in removedItems)
            {
                Remove(i);
            }
            
            _i = _items.IndexOf(cache_index);
            if (_i != 0)
                _i++;
            foreach (VItem i in removedItems)
            {
                VItem newi = i.Clone();
                newi.Selected = true;
                Insert(_i, newi);
            }
            CallTracksChangedDelegate();
        }

        public void CallTracksChangedDelegate()
        {
            if (tracksChangedDelegate != null)
                tracksChangedDelegate.Invoke();
        }

        public List<VItem> Items
        {
            get
            {
                return _items;
            }
        }

        public void ClearSelection()
        {
            foreach (VItem _sele in Items)
            {
                _sele.Selected = false;
            }
        }
        public void RemoveSelectedTracks(List<int> tracks)
        {
            //StoreCurrentState();
            if (tracks == null)
                return;
            /*
            if(tracks.Count != Items.Count)
            {
                tool.show(1,"Something went wrong: index count and item count do not match");
                throw new Exception("Handle index count != item count");
                return;
            }
            */
            int track_count = tracks.Count;
            List<VItem> list = new List<VItem>(ItemCount);
            foreach (int index in tracks)
            {
                if(index > Items.Count)
                {
                    tool.show(1, "Something went wrong: index count and item count do not match");
                    continue;
                    //throw new Exception("Handle index count != item count");
                    
                }
                list.Add(Items[index]);
                //VItem item = _items[index];
                //_items.Remove(item);
            }
            /*
            foreach (int index in tracks)
            {
                Remove(index);
            }
            */
            foreach (VItem i in list)
                Remove(i);
            CallTracksChangedDelegate();
            //ClearSelection();
        }
        /*
        public void MoveSelectedTracksTo(int index, IEnumerable<int> SelectedIndices)
        {
            foreach (int i in SelectedIndices)
            {
                Remove(i);
                Insert(index + 1, Items[i]);
                Items[i].Selected = true;
            }
            //_view.RefreshUI();
        }
        */
        public void CheckForRepeat(List<int> SelectedIndices)
        {
            foreach (int item in SelectedIndices)
            {
                string file = Items[item].Columns[1];
                if (!tool.StringCheck(file))
                    continue;

                foreach (VItem other in Items)
                {
                    string s = other.Name;
                    if (!tool.StringCheck(s))
                        continue;
                    if (s == file)
                        other.Selected = true;
                }
            }
        }

        //rename method
        public void MoveSelectedToBottom(IEnumerable<int> SelectedIndices)
        {
            foreach (int i in SelectedIndices)
            {
                Remove(i);
                Insert(ItemCount, Items[i]);
            }
            CallTracksChangedDelegate();
        }
        //rename method
        public void MoveSelectedToTop(IEnumerable<int> SelectedIndices)
        {
            foreach (int i in SelectedIndices)
            {
                Remove(i);
                Insert(0, Items[i]);
            }
            CallTracksChangedDelegate();
        }
        //rename method
        /*
        public void ShowLastSelected(IEnumerable<int> SelectedIndices, ColorScheme colors)
        {
            foreach (int i in SelectedIndices)
            {
                //tool.show(2, i);
                Items[i].State = ItemState.WasSelected;
                Items[i].CurrentColor.backColor = Color.LightSkyBlue;
                //Items[i].HighLightColors(colors);
            }
        }
        */
        //use enumerbale instead of list
        public void MoveSelectedItemsTo(int index,List<int> selectedIndices)
        {
            List<VItem> insertItems = new List<VItem>(selectedIndices.Count);
            List<VItem> removedItems = new List<VItem>(selectedIndices.Count);
            //if (index > ItemCount)
             //   index = ItemCount;
            VItem ref_item = Items[index];
            
            //tool.show(5, ref_item.Name);
            if (ref_item == null)
            {
                tool.show(5, "Ref item is null");
                return;
            }
            ClearSelection();
            int last_index = 0;
            foreach (int item in selectedIndices)
            {
                if (item == index)
                    return;
                VItem i = Items[item];
                i.Selected = true;
                insertItems.Add(i);
                removedItems.Add(Items[item]);
                last_index = item;
                
            }
            
            //else
            //    index--;
            foreach (VItem removeItem in removedItems)
            {
                Remove(removeItem);
            }
            index = Items.IndexOf(ref_item);

            if (index == -1)
            {
                tool.show(3, "Index is -1");
                return;
            }
            if (index > last_index)
            {
                index++;
                //index+=2;
                //tool.show(1, "lower drop");
            }
            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                Insert(index, insertItems[i]);
            }
            CallTracksChangedDelegate();
        }
        //override method?
        public void FindMissingFiles()
        {
            foreach (VItem i in Items)
            {
                if (i.Tag.GetType() != typeof(string))
                    throw new Exception("No file path set");

                if (!File.Exists(i.Name))
                    i.SetColors(new ColorScheme(Color.Black, MusicPlayer.MissingColor));
            }
        }
        /*
        public IEnumerable<VItem> SelectedItems
        {
            get
            {
                return GetSelectedItems();
            }
        }
        */
        public int ItemCount
        {
            get
            {
                return _items.Count;
            }
        }

        public IEnumerable<VItem> GetItems()
        {
            foreach(VItem item in _items)
            {
                yield return item;
            }
        }
        /*
        public IEnumerable<VItem> GetSelectedItems()
        {
            foreach(int i in SelectedIndices)
            {
                yield return _items[i];
            }
        }
        */

        public VItem GetItem(int index)
        {
            return _items[index];
        }

        public int IndexOf(VItem item)
        {
            for(int i = 0;i<_items.Count;i++)
            {
                if (_items[i] == item)
                    return i;
            }
            return -1;
        }

        private void SortIndices(int startIndex)
        {
            for(int i =startIndex;i<_items.Count;i++)
            {
                _items[i].Index = i;
            }
        }
    }
}
