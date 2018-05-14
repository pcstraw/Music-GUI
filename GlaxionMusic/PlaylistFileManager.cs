using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glaxion.Tools;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Glaxion.Music
{
    public class PlaylistFileManager : TreeViewMS
    {
        public PlaylistFileManager()
        {
            InitializeComponent();
        }

       // List<string> playlistfiles = new List<string>();
        public ListViewItem hoveredPlaylistItem;
        public List<string> playlistDir = new List<string>();
        public Dictionary<string, string> PlaylistFiles = new Dictionary<string, string>();
        public FileLoader fileLoader;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlaylistFileManager
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.ForeColor = System.Drawing.Color.Black;
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.playlistFileView_DragEnter);
            this.DragLeave += new System.EventHandler(this.playlistFileView_DragLeave);
            this.ResumeLayout(false);

        }

        public void LoadPlaylistDirectories()
        {
            Nodes.Clear();
            PlaylistFiles.Clear();
            int count = fileLoader.PlaylistDirectories.Count;
            for (int i = 0;i < count;i++)
                PopulateTreeFromDirectory(fileLoader.PlaylistDirectories[i]);

            if (Nodes.Count > 0)
            {
                foreach(TreeNode t in Nodes)
                {
                    if (t.Tag as string == Playlist.FindDefaultDirectory())
                        t.Expand();
                }
            }

            if(fileLoader.PlaylistDirectories.Count > 0)
                Playlist.DefaultPlaylistDirectory = Playlist.FindDefaultDirectory();
            CacheNodes();
        }
        
        public void PopulateTreeFromDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;
            TreeNode tn = LoadDirectoryToNode(path, "*.m3u*");
            SetTree(tn, searchText);
        }

        public TreeNode LoadDirectoryToNode(string dir, string fileExtention)
        {
            if (!Directory.Exists(dir))
            {
                tool.debug("Error in tool.LoadDirectory: ", dir, " Does not exist");
                return new TreeNode("No Directory: " + dir);
            }
            else
            {
                string[] files = Directory.GetFiles(dir, fileExtention);
                string[] dirs = Directory.GetDirectories(dir);
                PlaylistFiles.Clear();
                TreeNode rn = new TreeNode(Path.GetFileName(dir));
                rn.Tag = dir;
                foreach (string s in dirs)
                {
                    TreeNode tn = LoadDirectoryToNode(s, fileExtention);
                    tn.Tag = s;
                    if (tn.Nodes.Count > 0)
                        rn.Nodes.Add(tn);
                }

                foreach (string f in files)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = Path.GetFileName(f);
                    tn.Tag = f;
                    rn.Nodes.Add(tn);
                    Playlist p = fileLoader.GetPlaylist(f,true);
                    PlaylistFiles.Add(f, p.name);
                }
                return rn;
            }
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
        
        public void SelectDirectory()
        {
            List<string> sl = new List<string>();
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.IsFolderPicker = true;
            cd.Multiselect = true;
            cd.RestoreDirectory = true;
            List<string> l = new List<string>();
            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string s in cd.FileNames)
                    playlistDir.Add(s);
            }
        }
        
        public void LoadManager()
        {
            fileLoader = MusicPlayer.Player.fileLoader;
            LoadPlaylistDirectories();
            AssignEventHandlers();
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
        
        private void playlistFileView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(sender, DragDropEffects.Copy);
        }
        
        private void playlistFileView_DragLeave(object sender, EventArgs e)
        {
            //if the clipbaord is not empty, then we are already dragging something in the clipboard
            if (TotalClipboard.IsEmpty)
            {
                foreach (TreeNode node in SelectedNodes)
                {
                    string s = node.Tag as string;
                    if(tool.StringCheck(s))
                        TotalClipboard.Files.Add(s);
                }
            }
        }
        
        private void treeView_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            selectedNode = tool.SelectNode(this);
        }
        
        public string FindPlaylistDirectory()
        {
            string dir = "Null";
            if (selectedNode != null)
            {
                dir = Path.GetDirectoryName(selectedNode.Tag as string);
                return dir;
            }
            else
            {
                if (Nodes.Count > 0)
                {
                    string d = Nodes[0].Tag as string;
                    if (Directory.Exists(d))
                        return d;
                }
            }
            return "Null";
        }
        
        public void DeleteSelectedFiles(bool AllowConfirmation)
        {
            bool deleted = false;
            foreach (TreeNode node in SelectedNodes)
            {
                string path = node.Tag as string;

                if (tool.IsPlaylistFile(path) && File.Exists(path))
                {
                    if(AllowConfirmation)
                    { 
                        bool confirm = tool.askConfirmation(path);
                        if (!confirm)
                            continue;
                    }
                    File.Delete(path);
                    string message = string.Concat("file deleted: ", path);
                    tool.show(1000,message);
                    tool.debug(message);
                    deleted = true;
                }
            }
            if(deleted)
                LoadPlaylistDirectories();
        }
        
        public TreeNode GetNodeAtCursorPosition()
        {
            //the hovered over node
            System.Drawing.Point p = new System.Drawing.Point(0, System.Windows.Forms.Cursor.Position.Y);
            hoveredNode = GetNodeAt(PointToClient(p));
            return hoveredNode; 
        }
        
        public List<string> GetSelectedPlaylists()
        {
            List<string> list = new List<string>();
            foreach (TreeNode node in SelectedNodes)
            {
                string s = node.Tag as string;
                if (tool.IsPlaylistFile(s))
                    list.Add(s);
            }
            return list;
        }

        private void DuplicatePlaylistFile()
        {
            if (selectedNode != null)
            {
                string path = selectedNode.Tag as string;
                string s = tool.AppendFileName(path, "+");
                if (!File.Exists(s))
                    File.Copy(path, s);
                LoadPlaylistDirectories();
            }
        }
    }
}
