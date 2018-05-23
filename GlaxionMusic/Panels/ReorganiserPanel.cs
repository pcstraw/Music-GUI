using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Glaxion.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Glaxion.Music
{
    partial class ReorganiserControl : UserControl
    {

        public ReorganiserControl()
        {
            InitializeComponent();
            treeView.FileColor = treeView.ForeColor;
        }

        public bool failed; //currently not used.  if there is a failure, the calling method can detect it
        List<string> musicFiles = new List<string>(); //a flat heirachy of all the files we want to re-organise
        List<TreeNode> music_file_nodes = new List<TreeNode>();
        Dictionary<string, TreeNode> RootList = new Dictionary<string, TreeNode>();

        public bool addYearNodes { get; private set; }
        private TreeNode[] _nodes;

        public void RetrieveAudioFiles(TreeNode node)
        {
            //if the node is tagged with an audio file 
            //then we can assume it has no children and return early
            string check_current_node = node.Tag as string;
            if (tool.IsAudioFile(check_current_node))
            {
                if (!File.Exists(check_current_node))
                {
                    tool.show(5, "File not found on system", "->", check_current_node);
                    return;
                }
                if (!musicFiles.Contains(check_current_node))
                    musicFiles.Add(check_current_node);
                return;
            }

            foreach (TreeNode t in node.Nodes)
            {
                string s = t.Tag as string;
                if (!tool.IsAudioFile(s))
                    RetrieveAudioFiles(t);
                else
                {
                    if (!musicFiles.Contains(s))
                        musicFiles.Add(s);
                }
            }
        }

        private void TestDuplicateNodes(TreeNode n)
        {
            foreach (TreeNode tn in n.Nodes)
            {
                tn.ForeColor = Color.Red;
                foreach (TreeNode t in treeView.Nodes)
                {
                    if (tn != t && tn.Text == t.Text)
                        t.ForeColor = Color.Blue;
                }
            }
        }

        //used to recursively search the treeView
        public TreeNode FindNodeByTile(TreeNode node, string nodeTitle)
        {
            if (node.Text == nodeTitle)
                return node;

            foreach (TreeNode tn in node.Nodes)
            {
                if (nodeTitle == tn.Text)
                    return tn;
                TreeNode t = FindNodeByTile(tn, nodeTitle);
                if (t != null)
                    return t;
            }
            return null;
        }
        public void SetNodes(TreeNode[] nodes)
        {
            _nodes = nodes;
        }

        //create a tree structure from the node collection
        //based on ID3 Tags
        public void SetFiles()
        {
            treeView.Nodes.Clear();
            musicFiles.Clear();
            foreach (TreeNode n in _nodes)
                RetrieveAudioFiles(n);

            PopulateFilesToTree();
            treeView.ExpandAll();
            treeView.SelectedNode = null;
            treeView.SelectedNodes.Clear();
        }

        //Get the music directory path and use it for the root node
        public TreeNode FindRootDirectorNode(string filePath)
        {
            if (!tool.StringCheck(filePath))
            {
                tool.show(5, "Problem in Reorganiser: queiried node has an invalid tag");
                return null;
            }
            List<string> dirs = MusicPlayer.Player.fileLoader.MusicDirectories;
            foreach (string d in dirs)
            {
                if (filePath.Contains(d))
                {
                    TreeNode rn = GetRootNode(d);
                    if (!treeView.Nodes.Contains(rn))
                        treeView.Nodes.Add(rn);
                    return rn;
                }
            }
            tool.show(5, "Problem in Reorganiser: queiried node cannot be found in any of the music directories");
            return null;
        }

        private TreeNode GetRootNode(string key)
        {
            if (RootList.ContainsKey(key))
                return RootList[key];

            TreeNode tn = new TreeNode(key);
            RootList.Add(key, tn);
            return tn;
        }
        private TreeNode AddToNode(TreeNode node, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                key = "Blank";

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Text == key)
                    return n;
            }

            TreeNode tn = new TreeNode(key);
            node.Nodes.Add(tn);
            return tn;
        }

        private void PopulateFilesToTree()
        {
            RootList.Clear();
            music_file_nodes.Clear();
            foreach (string s in musicFiles)
            {
                string title = Path.GetFileName(s);
                TreeNode t = new TreeNode(title);
                t.ForeColor = treeView.ForeColor;
                t.BackColor = treeView.BackColor;
                t.ImageIndex = 1;
                t.SelectedImageIndex = 1;
                t.Tag = s;

                TreeNode rootNode = FindRootDirectorNode(s);
                //Get ID3 Info
                Song info = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(s);

                if (addYearNodes)
                {
                    string year = info.year;
                    TreeNode yearNode = AddToNode(rootNode, year);
                    string artist = info.artist;
                    TreeNode artistNode = AddToNode(yearNode, artist);
                    string album = info.album;
                    TreeNode albumNode = AddToNode(artistNode, album);
                    albumNode.Nodes.Add(t);
                }
                else
                {
                    string artist = info.artist;
                    TreeNode artistNode = AddToNode(rootNode, artist);
                    string album = info.album;
                    TreeNode albumNode = AddToNode(artistNode, album);
                    albumNode.Nodes.Add(t);
                }
                music_file_nodes.Add(t);
            }
        }
        private string GetParentPath(TreeNode node)
        {
            string text = node.Text;
            if (node.Parent != null)
                text = string.Concat(GetParentPath(node.Parent), @"\", text);
            return text;
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode hovered = treeView.hoveredNode;
            if (hovered == null)
                return;
            if (hovered.Nodes.Count == 0)
                return;

            TreeNode newnode = new TreeNode("New Folder");
            newnode.ForeColor = treeView.ForeColor;
            newnode.BackColor = treeView.BackColor;
            int count = hovered.Nodes.Count;

            foreach (TreeNode n in hovered.Nodes)
            {
                TreeNode t = (TreeNode)n.Clone();
                t.ForeColor = treeView.ForeColor;
                t.BackColor = treeView.BackColor;
                string s = n.Tag as string;
                if (tool.IsAudioFile(s))
                {
                    t.ImageIndex = 1;
                    t.SelectedImageIndex = 1;
                }
                newnode.Nodes.Add(t);
            }
            hovered.Nodes.Clear();
            hovered.Nodes.Add(newnode);
            //treeView.ExpandAll();
            newnode.EnsureVisible();
            newnode.Expand();
            treeView.selectedNode = newnode;
        }

        private void RemoveSelectedNodes(TreeNode node)
        {
            if (node == null)
                return;

            TreeNode parentNode = node.Parent;
            //if hovered node has no parent, or is a file(no children) then cancel remove
            if (parentNode == null)
                return;

            //decouple the selected node from it's parent and copy it's children
            parentNode.Nodes.Remove(node);
            foreach (TreeNode n in node.Nodes)
            {
                TreeNode t = (TreeNode)n.Clone();
                t.ForeColor = treeView.ForeColor;
                t.BackColor = treeView.BackColor;
                string s = n.Tag as string;
                if (tool.IsAudioFile(s))
                {
                    t.ImageIndex = 1;
                    t.SelectedImageIndex = 1;
                    musicFiles.Remove(s);
                    music_file_nodes.Remove(n);
                }
                parentNode.Nodes.Add(t);
            }

            node.Nodes.Clear();
            treeView.ExpandAll();
            treeView.Nodes[0].EnsureVisible();
        }

        private void remove_context_item_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNodes.Count > 0)
            {
                for (int i = 0; i < treeView.SelectedNodes.Count; i++)
                {
                    TreeNode tn = (TreeNode)treeView.SelectedNodes[i];
                    RemoveSelectedNodes(tn);
                }
            }
            else
            {
                if (treeView.hoveredNode != null)
                    RemoveSelectedNodes(treeView.hoveredNode);
            }
        }

        private void treeView_Click(object sender, EventArgs e)
        {
            TreeNode t = treeView.hoveredNode;

            if (t == null)
                return;

            string s = t.Tag as string;
            if (tool.IsAudioFile(s))
            {
                trackInfoUserControl.SetSong(s);
                //tool.show(2,"Show info in main panel");
                //throw new NotImplementedException();
                //trackInfoUserControl.ShowInfo(s);
            }
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                e.CancelEdit = true;
                CommonOpenFileDialog cd = new CommonOpenFileDialog();
                cd.IsFolderPicker = true;
                cd.Multiselect = false;
                cd.DefaultDirectory = e.Node.Text;
                if (cd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    e.Node.Text = cd.FileName;
                }
            }
        }

        private void add_year_label_Click(object sender, EventArgs e)
        {

        }

        private void add_year_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            addYearNodes = !addYearNodes;
            if (addYearNodes)
            {
                add_year_checkbox.ForeColor = Color.Red;
            }
            else
            {
                add_year_checkbox.ForeColor = ForeColor;
            }
            treeView.Nodes.Clear();
            PopulateFilesToTree();
            treeView.ExpandAll();
            treeView.SelectedNode = null;
            treeView.SelectedNodes.Clear();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            List<string> new_dirs = new List<string>();
            foreach (TreeNode t in music_file_nodes)
            {
                TreeNode n = t;
                string s = n.Tag as string;
                //gets the full path of the file we want to create
                //based on the node's parent
                string text = GetParentPath(t);
                //extract the directory and create one if it doesn't exist
                string dir = Path.GetDirectoryName(text);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!new_dirs.Contains(dir))
                    new_dirs.Add(dir);
                //force overwrite for now
                File.Copy(s, text, true);
                File.Delete(s);
            }
            foreach (string dir in new_dirs)
            {
                Process.Start(dir);
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            List<string> new_dirs = new List<string>();
            foreach (TreeNode t in music_file_nodes)
            {
                TreeNode n = t;
                string s = n.Tag as string;
                //gets the full path of the file we want to create
                //based on the node's parent
                string text = GetParentPath(t);
                //extract the directory and create one if it doesn't exist
                string dir = Path.GetDirectoryName(text);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!new_dirs.Contains(dir))
                    new_dirs.Add(dir);
                //force overwrite for now
                File.Copy(n.Tag as string, text, true);
            }
            foreach (string dir in new_dirs)
            {
                Process.Start(dir);
            }
        }

        private void trackInfoUserControl_Load(object sender, EventArgs e)
        {
            trackInfoUserControl.squashBoxControl1.Swap();
        }
    }
}
