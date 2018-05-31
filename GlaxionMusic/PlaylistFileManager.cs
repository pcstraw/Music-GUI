using System;
using System.Collections.Generic;
using Glaxion.Tools;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Glaxion.Music
{
    public class PlaylistFileManager : VTree
    {
        public PlaylistFileManager(ITreeView TreeViewInterface)
        {
            _view = TreeViewInterface;
            editDirectoriesGUIdelegate = new ListGUICallBack(EditDirectoriesCallback);
        }

        // List<string> playlistfiles = new List<string>();
      //  public ListViewItem hoveredPlaylistItem;
        public List<string> playlistDir = new List<string>();
        public Dictionary<string, string> PlaylistFiles = new Dictionary<string, string>();
        public FileLoader fileLoader;

        public void Load()
        {
            fileLoader = fileLoader = FileLoader.Instance;
            //  AssignEventHandlers();
        }
        /*
        public void AssignEventHandlers()
        {
            DragLeave += playlistFileView_DragLeave;
            ItemDrag += playlistFileView_ItemDrag;
            DragEnter += playlistFileView_DragEnter;
        }
        */

        public void LoadPlaylistDirectories()
        {
            Nodes.Clear();
            PlaylistFiles.Clear();
            int count = fileLoader.PlaylistDirectories.Count;
            for (int i = 0; i < count; i++)
                PopulateTreeFromDirectory(fileLoader.PlaylistDirectories[i]);

            if (Nodes.Count > 0)
            {
                foreach(VNode t in Nodes)
                {
                    if (t.name == Playlist.FindDefaultDirectory())
                        t.Expand = true;
                }
            }

            if (fileLoader.PlaylistDirectories.Count > 0)
                Playlist.DefaultPlaylistDirectory = Playlist.FindDefaultDirectory();
            CacheNodes();
        }
        
        public void PopulateTreeFromDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;
            VNode tn = LoadDirectoryToNode(path, "*.m3u*");
            SetTree(tn, searchText);
        }

        public VNode LoadDirectoryToNode(string dir, string fileExtention)
        {
            if (!Directory.Exists(dir))
            {
                tool.debug("Error in tool.LoadDirectory: ", dir, " Does not exist");
                return new VNode("No Directory: " + dir);
            }
            else
            {
                string[] files = Directory.GetFiles(dir, fileExtention);
                string[] dirs = Directory.GetDirectories(dir);
                PlaylistFiles.Clear();
                VNode rn = new VNode(Path.GetFileName(dir));
                rn.name = dir;
                foreach (string s in dirs)
                {
                    VNode tn = LoadDirectoryToNode(s, fileExtention);
                    tn.name = s;
                    if (tn.Nodes.Count > 0)
                        rn.Nodes.Add(tn);
                }

                foreach (string f in files)
                {
                    VNode tn = new VNode();
                    tn.Text = Path.GetFileName(f);
                    tn.name = f;
                    rn.Nodes.Add(tn);
                    Playlist p = fileLoader.GetPlaylist(f, true);
                    PlaylistFiles.Add(f, p.name);
                }
                return rn;
            }
        }

        ListGUICallBack editDirectoriesGUIdelegate;
        public void EditDirectoriesCallback(bool ok)
        {
            //if(ok)
            LoadPlaylistDirectories();
        }
        public void EditPlaylistDirectories()
        {
            ListGUI lg = new ListGUI(fileLoader.PlaylistDirectories, true);
            lg.Text = nameof(fileLoader.PlaylistDirectories);
            lg.callback = editDirectoriesGUIdelegate;
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
        
        public string FindPlaylistDirectory(VNode selectedNode)
        {
            string dir = "Null";
            if (selectedNode != null)
            {
                dir = Path.GetDirectoryName(selectedNode.name);
                return dir;
            }
            else
            {
                if (Nodes.Count > 0)
                {
                    string d = Nodes[0].name;
                    if (Directory.Exists(d))
                        return d;
                }
            }
            return "Null";
        }

        public void DeleteSelectedFiles(bool AllowConfirmation)
        {
            bool deleted = false;
            foreach (VNode node in SelectedNodes)
            {
                string path = node.name;

                if (tool.IsPlaylistFile(path) && File.Exists(path))
                {
                    if (AllowConfirmation)
                    {
                        bool confirm = tool.askConfirmation(path);
                        if (!confirm)
                            continue;
                    }
                    File.Delete(path);
                    string message = string.Concat("file deleted: ", path);
                    tool.show(10, message);
                    tool.debug(message);
                    deleted = true;
                }
            }
            if (deleted)
                LoadPlaylistDirectories();
        }
        
        public List<string> GetSelectedPlaylists()
        {
            List<string> list = new List<string>();
            foreach (VNode node in SelectedNodes)
            {
                string s = node.name;
                if (tool.IsPlaylistFile(s))
                    list.Add(s);
            }
            return list;
        }

        private void DuplicatePlaylistFile(VNode selectedNode)
        {
            if (selectedNode != null)
            {
                string path = selectedNode.name;
                string s = tool.AppendFileName(path, "+");
                if (!File.Exists(s))
                    File.Copy(path, s);
                LoadPlaylistDirectories();
            }
        }
    }
}

