using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Glaxion.Tools;
using System.Runtime.InteropServices;

namespace Glaxion.Music
{
    /// <summary>
    /// Summary description for TreeViewMS.
    /// Enhanced version of window Form TreeView class
    /// that incorporates multiselect functionality and double buffering
    /// </summary>
    public class TreeViewMS : System.Windows.Forms.TreeView
    {
        protected ArrayList m_coll;
        protected TreeNode m_lastNode, m_firstNode;
        public TreeNode hoveredNode;
        public Panel panel;  // I don't see why we need a panel.  get rid of in future
        public List<TreeNode> cachedNodes = new List<TreeNode>();
        public string LastSearchText;
        public string searchText;
        public TreeNode selectedNode;
        private bool _useCahcedNodes;
        public Color FileColor;
        public Color DirectoryColor;

        public TreeViewMS()
        {
            InitializeComponent();
            _useCahcedNodes = true;
            m_coll = new ArrayList();
            AllowDrop = true;
            FileColor = Color.LightYellow;
            DirectoryColor = this.ForeColor;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
        // Pinvoke:
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        //in future we shouldn't need this;
        //however we may think about referencing a dock panel class here instead
        public Panel GetPanel()
        {
            if (panel != null)
                return panel;
            panel = new Panel();
            panel.Controls.Add(this);
            this.Show();
            this.Dock = DockStyle.Fill;
            return panel;
        }

        //gets the colour from whatever we assigned to panel
        public void UpdateBackColor()
        {
            if (panel == null)
                return;
            BackColor = panel.BackColor;
            ForeColor = panel.ForeColor;
        }
        
        public TreeNode MouseSelectNode()
        {
            selectedNode= GetNodeAt(PointToClient(Cursor.Position));
            return selectedNode;
        }
       
        public void SetTree(List<TreeNode> nodes, string searchText)
        {
            PopulateTree(nodes);
            if (tool.StringCheck(searchText))
                SearchForText(searchText);
        }
        
        public void SetTree(TreeNode node, string searchText)
        {
            AddToTree(node);
            if (tool.StringCheck(searchText))
                SearchForText(searchText);
        }
        
        public void CacheNodes()
        {
            cachedNodes.Clear();
            cachedNodes = new List<TreeNode>(CopyTree());
        }
        
        public void AddToTree(TreeNode node)
        {
            if (node == null)
                return;
            Nodes.Add(node); //should thiss be a copy function?
        }

        public void PopulateTree(List<TreeNode> nodes)
        {
            if (nodes == null)
                return;
            if (nodes.Count < 1)
                return;
            Nodes.Clear();
            for (int i=0;i<nodes.Count;i++)
            {
                if(nodes[i]==null)
                {
                    tool.Show("Catch null node REFERENCE");
                }
               // TreeNode n = TreeViewMS.CopyNode(nodes[i]);
                Nodes.Add(nodes[i]);
            }
        }
        public void PopulateTree(TreeNodeCollection nodes)
        {
            if (nodes.Count < 1)
                return;
            Nodes.Clear();
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode n = nodes[i].Clone() as TreeNode;
                Nodes.Add(n);
            }
        }
        
        public TreeNode[] CopyTree()
        {
            TreeNode[] nodes = new TreeNode[Nodes.Count];
            Nodes.CopyTo(nodes,0);
            return nodes;
        }

        static TreeNode CopyNode(TreeNode node)
        {
            TreeNode n = new TreeNode();
            n.Text = node.Text;
            n.Tag = node.Tag;
            n.BackColor = node.BackColor;
            n.ForeColor = node.ForeColor;
            return n;
        }

        public void SearchTree(string text, List<TreeNode> nodes, List<TreeNode> results)
        {
            foreach (TreeNode t in nodes)
            {
                if (t.Text.ToLower().Contains(text.ToLower()))
                {
                    results.Add(t.Clone() as TreeNode);
                }
                SearchTree(text, t.Nodes, results);
            }
        }
        
        public void SearchTree(string text,TreeNodeCollection nodes, List<TreeNode> results)
        {
            foreach (TreeNode t in nodes)
            {
                if (t.Text.ToLower().Contains(text.ToLower()))
                {
                    results.Add(t.Clone() as TreeNode);
                }
                SearchTree(text, t.Nodes, results);
            }
        }

        public void SearchForText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (!_useCahcedNodes)
                {
                    PopulateTree(cachedNodes);
                    _useCahcedNodes = true;
                    //TODO: restore last opened nodes
                    if (Nodes.Count > 0)
                    {
                        Nodes[0].Expand();
                    }
                }
                return;
            }
            List<TreeNode> results = new List<TreeNode>();
            SearchTree(text,cachedNodes,results);
            if (results.Count > 0)
            {
                PopulateTree(results);
                _useCahcedNodes = false;
            }
        }

        //Not being used.  Check if they woprk
        public TreeNode FindFileByName(string name, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (!tool.StringCheck(node.Text))
                    continue;

                string mainDir = Path.GetDirectoryName(node.Tag as string);
                //hu?  
                //if (mainDir != Properties.Settings.Default.defaultDirectory)
                //   continue;
                string n = Path.GetFileNameWithoutExtension(node.Text);
                if (name == n)
                {
                    return node;
                }
                if (node.Nodes.Count > 0)
                {
                    TreeNode tn = FindFileByName(name, node.Nodes);
                    if (tn != null)
                        return tn;
                }
            }
            return null;
        }

        public TreeNode FindFileByPath(string path, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (!tool.StringCheck(node.Tag as string))
                    continue;
                string n = Path.GetFileNameWithoutExtension(node.Tag as string);
                if (path == n)
                {
                    return node;
                }
                if (node.Nodes.Count > 0)
                {
                    TreeNode tn = FindFileByPath(path, node.Nodes);
                    if (tn != null)
                        return tn;
                }
            }
            return null;
        }

        public void DeSelectAll()
        {
            removePaintFromNodes();
            SelectedNodes.Clear();
        }

        //suitable to made virtual?
        public void DeSelectNodes(ArrayList nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                if (SelectedNodes.Contains(tn))
                {
                    //tn.BackColor = tn.TreeView.BackColor;
                    //tn.ForeColor = DirectoryColor;
                    if (tn.Tag != null && tn.Tag is string)
                    {
                        //this check should be moved to the musicFileManager class
                        bool isDir = (File.GetAttributes(tn.Tag as string) & FileAttributes.Directory)
                                            == FileAttributes.Directory;
                        if (!isDir)
                        {
                            tn.ForeColor = FileColor;
                        }
                        else
                        {
                            tn.ForeColor = DirectoryColor;
                        }
                    }
                }
            }
        }

        public ArrayList SelectedNodes
        {
            get
            {
                return m_coll;
            }
            set
            {
                removePaintFromNodes();
                m_coll.Clear();
                m_coll = value;
                paintSelectedNodes();
            }
        }
        
        // Triggers
        // (overriden method, and base class called to ensure events are triggered)
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            // selecting twice the node while pressing CTRL ?
            if (bControl && m_coll.Contains(e.Node))
            {
                // unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // update nodes
                removePaintFromNodes();
                m_coll.Remove(e.Node);
                paintSelectedNodes();
                return;
            }

            m_lastNode = e.Node;
            if (!bShift) m_firstNode = e.Node; // store begin of shift sequence
        }
        
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!m_coll.Contains(e.Node)) // new node ?
                {
                    m_coll.Add(e.Node);
                }
                else  // not new, remove it from the collection
                {
                    removePaintFromNodes();
                    m_coll.Remove(e.Node);
                }
                paintSelectedNodes();
            }
            else
            {
                // SHIFT is pressed
                if (bShift)
                {
                    Queue myQueue = new Queue();

                    TreeNode uppernode = m_firstNode;
                    TreeNode bottomnode = e.Node;
                    // case 1 : begin and end nodes are parent
                    bool bParent = isParent(m_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                    if (!bParent)
                    {
                        bParent = isParent(bottomnode, uppernode);
                        if (bParent) // swap nodes
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!m_coll.Contains(n)) // new node ?
                                myQueue.Enqueue(n);

                            n = n.Parent;
                        }
                    }
                    // case 2 : nor the begin nor the end node are descendant one another
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper) // reversed?
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!m_coll.Contains(n)) // new node ?
                                    myQueue.Enqueue(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            } // end while

                        }
                        else
                        {
                            if (!m_coll.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!m_coll.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }
                    }

                    m_coll.AddRange(myQueue);

                    paintSelectedNodes();
                    m_firstNode = e.Node; // let us chain several SHIFTs if we like it
                } // end if m_bShift
                else
                {
                    // in the case of a simple click, just add this item
                    if (m_coll != null && m_coll.Count > 0)
                    {
                        removePaintFromNodes();
                        m_coll.Clear();
                    }
                    m_coll.Add(e.Node);
                }
            }
        }
        
        protected bool isParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
                return true;

            TreeNode n = childNode;
            bool bFound = false;
            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == parentNode);
            }
            return bFound;
        }

        protected void paintSelectedNodes()
        {
            foreach (TreeNode n in m_coll)
            {
                n.BackColor = SystemColors.Highlight;
                n.ForeColor = SystemColors.HighlightText;
            }
        }

        public void RemovePaintFromNodes(Color back, Color fore)
        {
            if (m_coll.Count == 0) return;

            TreeNode n0 = (TreeNode)m_coll[0];
            // Color back = n0.TreeView.BackColor;
            // Color fore = n0.TreeView.ForeColor;

            foreach (TreeNode n in m_coll)
            {
                n.BackColor = back;
                n.ForeColor = fore;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TreeViewMS
            // 
            this.LineColor = System.Drawing.Color.Black;
            this.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.TreeViewMS_NodeMouseHover);
            this.ResumeLayout(false);

        }
        
        public virtual void TreeViewMS_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
           
            hoveredNode = e.Node;
        }

        protected void removePaintFromNodes()
        {
            if (m_coll.Count == 0) return;

            TreeNode n0 = (TreeNode)m_coll[0];
            Color fore = ForeColor;
            
            if (n0.Tag != null && n0.Tag is string && File.Exists(n0.Tag as string))
            {
                bool isDir = (File.GetAttributes(n0.Tag as string) & FileAttributes.Directory)
                                    == FileAttributes.Directory;
                if (!isDir)
                {
                    fore = FileColor;
                }
                else
                {
                    fore = DirectoryColor;
                }
            }
            
            Color back = this.BackColor;
            foreach (TreeNode n in m_coll)
            {
                n.BackColor = back;
                n.ForeColor = fore;
            }
        }
    }
}
