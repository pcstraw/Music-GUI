using System;
using System.Collections.Generic;
using System.Drawing;
using Glaxion.Tools;

namespace Glaxion.Music
{
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
        public object Tag;
        public string Text;
        public string path;
        public string name;
        public bool Expand;
        public bool selected;
        public bool isFile;
        public Color ForeColor;
        public Color BackColor;

        internal VNode Clone()
        {
            VNode node = new VNode(Text);
            node.Tag = Tag;
            node.path = path;
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
        }
        public int Index; //dep
        public ColorScheme CurrentColor;
        public ColorScheme OldColor;
        public List<string> Columns = new List<string>();
        public object Tag;
        public int State;
        public bool Selected;
        public bool Checked;

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
    }

    public interface IListView
    {
        void PopulateList(List<VItem> tree);
    }

    public class VListView
    {
        public VListView()
        {
        }
        public void SetListViewInterface(IListView ListViewInterface)
        {
            _view = ListViewInterface;
        }
        IListView _view;
        public List<VItem> Items = new List<VItem>();
        public List<VItem> SelectedItems = new List<VItem>();
        public List<VItem> CheckedItems = new List<VItem>();
    }
}
