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

namespace Glaxion.Libraries
{
    /// <summary>
    /// Summary description for TreeViewMS.
    /// </summary>
    public class TreeViewMnS : System.Windows.Forms.TreeView
    {
        protected ArrayList m_coll;
        protected TreeNode m_lastNode, m_firstNode;
        public Color FileColor = Color.LightYellow;
        public Color DirectoryColor = Color.Yellow;
        public string searchText;
        public Panel panel;
        public TreeViewMnS()
        {
            m_coll = new ArrayList();
            AllowDrop = true;
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

        public void UpdateBackColor()
        {
            if (panel == null)
                return;
            BackColor = panel.BackColor;
            ForeColor = panel.ForeColor;
        }

        public TreeNode selectedNode;

        public TreeNode MouseSelectNode()
        {
            selectedNode = tool.SelectNode(this);
            return selectedNode;
        }

        public void mouse_DoubleClick(object sender, EventArgs e)
        {

        }

        public List<TreeNode> CurrentNodeMatches = new List<TreeNode>();

        public void SearchForNodes(TreeNode StartNode, string text)
        {
            //  string inputLower = text.ToLower();
            //  string checkLower = StartNode.Text.ToLower();
            // string path = StartNode.Tag as string;

            if (StartNode.Text.ToLower().Contains(text.ToLower()))
            {
                if (!CurrentNodeMatches.Contains(StartNode))
                    CurrentNodeMatches.Add(StartNode);
            }

            foreach (TreeNode node in StartNode.Nodes)
            {
                /*
                if (node.Text.ToLower().Contains(text.ToLower()))
                {
                    if (!CurrentNodeMatches.Contains(node))
                        CurrentNodeMatches.Add(node);
                }*/
                SearchForNodes(node, text);
            }

            /*
            if (!Path.HasExtension(path))
            {
                foreach (TreeNode t in StartNode.Nodes)
                {
                    SearchForNodes(t, text);
                }
            }
            else
            {
                TreeNode node = StartNode;
                
                while (node != null)
                {
                    if (node.Text.ToLower().Contains(text.ToLower()))
                    {
                        if (!CurrentNodeMatches.Contains(node))
                            CurrentNodeMatches.Add(node);
                    };
                    if (node.Nodes.Count != 0)
                    {
                        SearchForNodes(node.Nodes[0], text);//Recursive Search 
                    };
                    if (node == null)
                        break;
                    if (node.NextNode == null)
                        break;
                    node = node.NextNode;
                };
                
            }*/
        }


        public void SetTree(List<TreeNode> nodes, string searchText)
        {
            PopulateTree(nodes);
            CacheNodes();
            if (!string.IsNullOrEmpty(searchText))
                SearchForText(searchText);
        }

        public void SetTree(TreeNode node, string searchText)
        {
            PopulateTree(node);
            CacheNodes();
            if (!string.IsNullOrEmpty(searchText))
                SearchForText(searchText);
        }

        public void CacheNodes()
        {
            cachedNodes.Clear();
            cachedNodes = CopyTree();
        }

        public void PopulateTree(TreeNode node)
        {
            if (node == null)
                return;
            
         //   Nodes.Clear();
            Nodes.Add(node); //should thiss be a copy function?
        }

        public void PopulateTree(List<TreeNode> nodes)
        {
            if (nodes.Count < 1)
                return;

            Nodes.Clear();

            foreach (TreeNode t in nodes)
            {
                Nodes.Add(t);
            }
        }

        public List<TreeNode> CopyTree()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode t in Nodes)
            {
                nodes.Add((TreeNode)t.Clone());
            }
            return nodes;
        }

        public List<TreeNode> cachedNodes = new List<TreeNode>();
        public string LastSearchText;

        public List<TreeNode> SearchTree(string text)
        {
            CurrentNodeMatches.Clear();
            List<TreeNode> results = new List<TreeNode>();
            //pp
            //Tool.Show(text);
            foreach (TreeNode t in cachedNodes)
            {
                //Tool.Debug(0,t.Text);
                SearchForNodes(t, text);
            }

            foreach (TreeNode t in CurrentNodeMatches)
            {
                TreeNode newNode = t.Clone() as TreeNode;
                //newNode.Tag = t.Tag;
                // Tool.Debug(t.Text);
                results.Add(newNode);
            }
            return results;
        }

        public void SearchForText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                PopulateTree(cachedNodes);
                if (Nodes.Count > 0)
                {
                    Nodes[0].Expand();
                }
                return;
            }
            List<TreeNode> results = SearchTree(text);
            if (results.Count > 0)
            {
                //Tool.Show();
                PopulateTree(results);
            }
            /*
            if (text == null | text == "")
            {
                if (cachedNodes.Count > 0)
                {
                    this.Nodes.Clear();
                    foreach (TreeNode t in cachedNodes)
                    {
                        this.Nodes.Add(t);
                    }
                    cachedNodes.Clear();
                    LastSearchText = null;
                }
            }
            else
            {
               // cachedNodes.Clear();
                if (cachedNodes.Count == 0)
                {
                    foreach (TreeNode t in this.Nodes)
                    {
                        cachedNodes.Add(t);
                    }
                }

               // Tool.Show(LastSearchText,text);
                //if (LastSearchText != text)
                if(true)
                {
                    
                    CurrentNodeMatches.Clear();
                    foreach (TreeNode t in cachedNodes)
                    {
                        SearchForNodes(t, text);
                    }

                    this.Nodes.Clear();
                    foreach (TreeNode t in CurrentNodeMatches)
                    {
                        TreeNode newNode = t.Clone() as  TreeNode;
                        //newNode.Tag = t.Tag;
                        // Tool.Debug(t.Text);
                        this.Nodes.Add(newNode);
                    }
                }
                LastSearchText = text;
            }
            */
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        public void DeSelectAll()
        {
            removePaintFromNodes();
            SelectedNodes.Clear();
        }

        public void DeSelectNodes(ArrayList nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                if (SelectedNodes.Contains(tn))
                {
                    tn.BackColor = tn.TreeView.BackColor;
                    tn.ForeColor = DirectoryColor;
                    if (tn.Tag != null && tn.Tag is string)
                    {
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
        //
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



        // Helpers
        //
        //


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

        protected void removePaintFromNodes()
        {
            if (m_coll.Count == 0) return;

            TreeNode n0 = (TreeNode)m_coll[0];
            Color fore = this.ForeColor;

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
                    fore = this.ForeColor;
                }
            }

            Color back = this.BackColor;
            //

            foreach (TreeNode n in m_coll)
            {
                n.BackColor = back;
                n.ForeColor = fore;
            }
        }
    }
}
