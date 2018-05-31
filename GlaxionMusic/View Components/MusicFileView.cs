using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Glaxion.Tools;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Glaxion.Music
{
    public class MusicFileView : TreeViewMS,ITreeView
    {
        public MusicFileView()
        {
            InitializeComponent();
            //suppose to help prevent start up crash by loading in the handle.
            //mayeb stick this in the base class as well?
            //var foo = this.Handle;
            AllowDrop = true;
            audioFileImadeIndex = 1;
            manager = new MusicFileManager(this,new ColorScheme(ForeColor,BackColor));
            FileColor = manager.FileColor;
        }
        public MusicFileManager manager;
        public int audioFileImadeIndex;
        bool loadID3Tags = true;

        public void LoadManager()
        {
            AssignEventHandlers();
            manager.Load();
        }
        
        //some of these cam be made virtual functions
        public void AssignEventHandlers()
        {
            ItemDrag += MusicFileManager_ItemDrag;
           // DragEnter += MusicFileManager_DragEnter;
            MouseDoubleClick += MusicFileManager_MouseDoubleClick;
            MouseDown += MusicFileManager_MouseDown;
            DragDrop += MusicFileManager_DragDrop;
        }
        
        private void MusicFileManager_DragDrop(object sender, DragEventArgs e)
        {
            if (InternalClipboard.IsEmpty)
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (data == null)
                    return;
                manager.DropDirectoriesFromClipboard(data);
            }
        }
        
        private void MusicControl_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Instance != null)
            {
                manager.LoadDirectoriesToTree();
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastTrack))
                {
                    MusicPlayer.Instance.currentTrackString = Properties.Settings.Default.LastTrack;
                }
            }
        }
        /*
        public virtual void FindTrack(string path)
        {
            string s = path;
            TreeNode t = null;
            if (Nodes.Count > 0)
            {

                foreach (TreeNode directory in Nodes)
                {
                    t = SearchTreeView(directory, s);
                    if (t != null)
                    {
                        SelectedNode = t;
                        t.Expand();
                        //Select();
                        t.BackColor = Color.DarkBlue;
                        return;
                    }
                }
            }
        }
        */
        
        public void OpenContaingFolder()
        {
            if (selectedNode != null)
            {
                string s = selectedNode.Name;
                if (Path.HasExtension(s))
                    Process.Start(Path.GetDirectoryName(s));
                else
                    Process.Start(s);
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenContaingFolder();
        }
        
        //for future use
        public void MusicFileManager_DragEnter(object sender, DragEventArgs e)
        {
            tool.AllowDragEffect(e);
        }
        
        public void MusicFileManager_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(sender, DragDropEffects.Copy);
        }
        
        public void MusicFileManager_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedNode = tool.SelectNode(this);
            }
        }

        public void MusicFileManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            manager.PlaySelectedNode(WinFormUtils.GetVNode(selectedNode));
        }

        public void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.fileLoader.SetMusicDirectory();
        }

        public void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.EditMusicDirectories();
        }
        
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.LoadDirectoriesToTree();
        }
        
        private void editID3TagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                string s = selectedNode.Name;
                SongControl.CreateTagEditor(s,MusicPlayer.WinFormApp);
            }
        }
        
        public void GetNodesFolderJPG(TreeNode n)
        {
            string path = n.Name;
            if (!tool.StringCheck(path))
                return;
            string result = tool.GetFolderJPGFile(path);
            if (tool.StringCheck(result) && File.Exists(result))
                Process.Start(result);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MusicFileView
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Yellow;
            this.ResumeLayout(false);

        }

        public void PopulateTree(List<VNode> tree)
        {
            BeginUpdate();
            Nodes.Clear();
            foreach (VNode n in tree)
            {
                TreeNode node = WinFormUtils.GetTreeNode(n);
                //if (n.isFile == true)
                //    node.ForeColor = FileColor;
                Nodes.Add(node);
            }
            EndUpdate();
            //manager.CacheNodes(manager.Nodes);
            //Populate(nodes);
        }

        public void Find(string path)
        {
            if (!tool.StringCheck(path))
                return;
            TreeNode node = FindFileByPath(path, Nodes);
            if (node != null)
            {
                SelectedNode = node;
                Select();
                Focus();
                node.Parent.Expand();
                node.BackColor = Color.DarkBlue;
                node.ForeColor = Color.White;
                node.EnsureVisible();
            }
        }
    }
}
