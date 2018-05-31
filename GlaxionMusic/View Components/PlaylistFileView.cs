using System;
using System.Collections.Generic;
using Glaxion.Tools;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Glaxion.Music
{
    public class PlaylistFileView : TreeViewMS, ITreeView
    {
        public PlaylistFileView()
        {
            InitializeComponent();
            manager = new PlaylistFileManager(this);
            AllowDrop = true;
            AssignEventHandlers();
        }

        // List<string> playlistfiles = new List<string>();
        public PlaylistFileManager manager;
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlaylistFileView
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PlaylistFileView_DragDrop);
            this.ResumeLayout(false);
        }
        
        public void OpenFolder(ListViewItem item)
        {
            if (item != null)
            {
                Playlist p = item.Tag as Playlist;
                if (p != null && Directory.Exists(Path.GetDirectoryName(p.path)))
                    Process.Start(Path.GetDirectoryName(p.path));
            }
        }
        
        private void playlistFileView_DragEnter(object sender, DragEventArgs e)
        {
            tool.AllowDragEffect(e);
        }

        public void AssignEventHandlers()
        {
            DragLeave += playlistFileView_DragLeave;
            ItemDrag += playlistFileView_ItemDrag;
            DragEnter += playlistFileView_DragEnter;
        }

        public void OpenSelectedNodeDirectory()
        {
            TreeNode n = MouseSelectNode();
            if (n == null)
                return;

            if (tool.StringCheck(n.Tag as string))
                tool.OpenFileDirectory(n.Tag as string);
        }

        internal void LoadManager()
        {
            manager.Load();
            LoadPlaylistDirectories();
        }

        private void playlistFileView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(sender, DragDropEffects.Copy);
        }
        //Go to TreeViewMS DragLeave
        /*
        protected override void TreeViewMS_DragLeave(object sender, EventArgs e)
        {
            InternalClipboard.CopyTree(this);
            //otalClipboard.Files.Reverse();
        }
        */

        private void playlistFileView_DragLeave(object sender, EventArgs e)
        {
            //if the clipbaord is not empty, then we are already dragging something in the clipboard
            /*
            if (InternalClipboard.IsEmpty)
            {
                foreach (TreeNode node in SelectedNodes)
                {
                    string s = node.Name;
                    if (tool.IsPlaylistFile(s))
                        InternalClipboard.Files.Add(s);
                }
            }
            */
        }
        
        private void treeView_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            selectedNode = tool.SelectNode(this);
        }
        
        public TreeNode GetNodeAtCursorPosition()
        {
            //the hovered over node
            System.Drawing.Point p = new System.Drawing.Point(0, System.Windows.Forms.Cursor.Position.Y);
            hoveredNode = GetNodeAt(PointToClient(p));
            return hoveredNode; 
        }

        public void PopulateTree(List<VNode> tree)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (VNode n in tree)
            {
                nodes.Add(WinFormUtils.GetTreeNode(n));
            }
            Populate(nodes);
        }

        private void PlaylistFileView_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        internal void LoadPlaylistDirectories()
        {
            manager.LoadPlaylistDirectories();
            if(Nodes.Count > 0)
            {
                Nodes[0].Expand();
            }
        }
    }
}
