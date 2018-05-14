using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Specialized;
using Glaxion.Libraries;

namespace Glaxion.Music
{
    public class MusicFileManager : Glaxion.Music.TreeViewMS
    {
        public MusicFileManager()
        {
            //InitializeComponent();
            //suppose to help prevent start up crash by loading in the handle.
            //mayeb stick this in the base class as well?
            //var foo = this.Handle;
            if(MusicPlayer.Player != null)
            MusicPlayer.Player.DirectoriesAddedEvent += DirectoryAdded;
            AllowDrop = true;
            audioFileImadeIndex = 1;
        }
        public FileLoader fileLoader;
        public List<TreeNode> albumNodes = new List<TreeNode>();
        public List<TreeNode> artistNodes = new List<TreeNode>();
        public List<TreeNode> musicFileNodes = new List<TreeNode>();
        public List<TreeNode> yearNodes = new List<TreeNode>();
        public int audioFileImadeIndex=0;
        bool loadID3Tags = true;

        public void LoadManager()
        {
            AssignEventHandlers();
            fileLoader = MusicPlayer.Player.fileLoader;
            LoadDirectoriesToTree();
        }

        protected void DirectoryAdded(object o, EventArgs e)
        {
            
        }
        
        public void PlayDirectory(TreeNodeCollection nodes)
        {
            foreach (TreeNode t in nodes)
            {
                if (Path.HasExtension(t.Tag as string))
                {
                    PlaySelectedNode(t);
                    return;
                }
                else
                {
                    if (t.Nodes.Count > 0)
                        PlayDirectory(t.Nodes);
                }
            }
        }

        //some of these cam be made virtual functions
        public void AssignEventHandlers()
        {
            DragLeave += MusicFileManager_DragLeave;
            ItemDrag += MusicFileManager_ItemDrag;
            DragEnter += MusicFileManager_DragEnter;
            MouseDoubleClick += MusicFileManager_MouseDoubleClick;
            MouseDown += MusicFileManager_MouseDown;
            DragDrop += MusicFileManager_DragDrop;
        }

        public bool AddDirectoryFromFile(string s)
        {
            string dir = s;
            if (tool.IsAudioFile(s))
            {
                dir = Path.GetDirectoryName(s);
            }
            if (Directory.Exists(dir))
            {
                fileLoader.AddMusicDirectory(dir);
                return true;
            }
            return false;
        }
        
        public void DropDirectoriesFromClipboard()
        {
            bool addedDir = false;
            if (!TotalClipboard.IsEmpty)
            {
                /*
                foreach (string s in TotalClipboard.Files)
                {
                    addedDir = AddDirectoryFromFile(s);
                }
                */
            }
            else
            {
                // TotalClipboard
                StringCollection collection = Clipboard.GetFileDropList();
                foreach (string s in collection)
                    addedDir = AddDirectoryFromFile(s);
            }
            if (addedDir)
                LoadDirectoriesToTree();
        }

        private void MusicFileManager_DragDrop(object sender, DragEventArgs e)
        {
            DropDirectoriesFromClipboard();
        }
        
        //careful.  seems to be flakey with parallel foreach
        public TreeNode LoadFilesFromDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                return null;
            
            TreeNode rn = new TreeNode(Path.GetFileName(dir));
            rn.Tag = dir;
            bool containsAudio = false;
           
            try
            {
                IEnumerable<string> files = tool.LoadAudioFiles(dir, SearchOption.TopDirectoryOnly);
                IEnumerable<string> subDir = Directory.EnumerateDirectories(dir);
                if (files.Count()>0)
                    containsAudio = true;
                
                foreach (string s in subDir)
                {
                    TreeNode tn = LoadFilesFromDirectory(s);
                    if (tn != null)
                    {
                        containsAudio = true;
                        //if (tn.Nodes.Count > 0)
                        rn.Nodes.Add(tn);
                    }
                }
                
                if (!containsAudio)
                    return null;
                
                AddFilesToNode(rn, files);
                return rn;
                /*
                Parallel.ForEach(subDir, s =>
                {
                    TreeNode tn = LoadDirectory(s);
                    if (tn != null)
                    {
                        containsAudio = true;
                        if (tn.Nodes.Count > 0)
                            rn.Nodes.Add(tn);
                    }
                });
                */
                /*
                Parallel.ForEach(files, s =>
                {
                    TrackInfo.GetInfo(s);
                });
                */

            }
            catch (Exception e)
            {
                tool.show(5,"Exception loading music file:",e.Message);
                return null;
            }
        }

        private void AddFilesToNode(TreeNode node, IEnumerable<string> files)
        {
            
            foreach (string f in files)
            {
                TreeNode tn = new TreeNode();
                tn.Text = Path.GetFileName(f);
                tn.Tag = f;
                tn.ForeColor = FileColor;
                //tn.StateImageIndex = audioFileImadeIndex;
                node.Nodes.Add(tn);
                fileLoader.AddMusicFile(f);
            }
            //causing treenode.clone implementation to fail.  not all that surprising
            /*
            Parallel.ForEach(files, f =>
            {
                TreeNode tn = new TreeNode();
                tn.Text = Path.GetFileName(f);
                tn.Tag = f;
                tn.ForeColor = Color.LightYellow;
                node.Nodes.Add(tn);
            });
            */
        }
        
        //called from the View context menu
        public void LoadFileNodesToView()
        {
            SetTree(musicFileNodes, searchText);
        }
        

        public void LoadArtistNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (artistNodes.Count == 0)
                BuildArtistTree();
            SetTree(artistNodes, searchText);
        }
        public void LoadAlbumNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (albumNodes.Count == 0)
                BuildAlbumTree();
            SetTree(albumNodes, searchText);
        }
        public void LoadYearNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (yearNodes.Count == 0)
                BuildYearTree();
            SetTree(yearNodes, searchText);
        }

        public void LoadAudioFiles()
        {
            //start afresh
            musicFileNodes.Clear();
            fileLoader.musicDirectories.Clear();
            albumNodes.Clear();
            artistNodes.Clear();
            yearNodes.Clear();
            foreach (string s in fileLoader.MusicDirectories)
            {
                if (!fileLoader.musicDirectories.Contains(s))
                {
                    TreeNode tn = LoadFilesFromDirectory(s);
                    if(tn == null)
                        continue;
                    tn.Text = s;
                    tn.Tag = s;
                    
                    //tn.StateImageIndex = -1;
                    if (tn.Nodes.Count > 0)
                    {
                        musicFileNodes.Add(tn);
                    }
                }
            }
            if (musicFileNodes.Count > 0)
            {
                musicFileNodes[0].Expand();
            }
        }
        
        public async void LoadDirectoriesToTree()
        {
            DateTime startTime = DateTime.Now; // the old date
            //double start_in_seconds = TimeSpan.FromTicks(startTime.Ticks).TotalSeconds;
            
            //tool.debug("Loading audio files");
            //tool.debug(" Start Time: "+startTime);
            await LoadAudioFilesAsync();
            //cannot do ui stuff from anothere thread, so for now populate the treeview afterward
            PopulateTree(musicFileNodes);
            if (tool.StringCheck(searchText))
                SearchForText(searchText);
            
            if(loadID3Tags)
                fileLoader.LoadID3Library();
            DateTime endTime = DateTime.Now;
           // double end_in_seconds = TimeSpan.FromTicks(endTime.Ticks).TotalSeconds;
            //tool.debug("End Time: " + endTime);
            double elapsed_seconds = (endTime - startTime).TotalSeconds;
            tool.debug("elapised time: " + elapsed_seconds);
            CacheNodes();
        }

        public Task LoadAudioFilesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                LoadAudioFiles();
            });
        }

        //TODO:  figure out a ncie way of loading in all the ID3 tags
        public void BuildAlbumTree()
        {
            albumNodes.Clear();
            Console.WriteLine("Music File Manager: Building Album Tree...");
            int i = 0;
            foreach (string s in MusicPlayer.Player.fileLoader.trackInfoManager.albums)
            {
                TreeNode tn = new TreeNode(s); 
                tn.Tag = s;
                albumNodes.Add(tn);
                i++;
            }

            foreach (TreeNode t in albumNodes)
            {
                foreach (KeyValuePair<string,Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.album)
                    {
                        TreeNode tn = new TreeNode(info.Value.title);
                        tn.Tag = info.Key;
                        tn.ForeColor = FileColor;
                        t.Nodes.Add(tn);
                    }
                }
            }
            Console.WriteLine("   ..Total Album node count: " + albumNodes.Count);
        }

        public void BuildArtistTree()
        {
            artistNodes.Clear();
            Console.WriteLine("Music File Manager:", "Building Artist Tree...");
            foreach (string s in MusicPlayer.Player.fileLoader.trackInfoManager.artists)
            {
                TreeNode tn = new TreeNode(s);
                tn.Tag = s;
                artistNodes.Add(tn);
            }

            foreach (TreeNode t in artistNodes)
            {
                foreach (KeyValuePair<string, Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.artist)
                    {
                        TreeNode tn = new TreeNode(info.Value.title);
                        tn.Tag = info.Value.path;
                        tn.ForeColor = FileColor;
                        t.Nodes.Add(tn);
                    }
                }
            }
            Console.WriteLine("   ..Total Artist node count: " + artistNodes.Count);
        }

        public void BuildYearTree()
        {
            yearNodes.Clear();
            Console.WriteLine("Music File Manager:", "Building Year Tree...");
            foreach (string s in MusicPlayer.Player.fileLoader.trackInfoManager.years)
            {
                TreeNode tn = new TreeNode(s);
                tn.Tag = s;
                yearNodes.Add(tn);
            }

            foreach (TreeNode t in yearNodes)
            {
                foreach (KeyValuePair<string, Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.year)
                    {
                        TreeNode tn = new TreeNode(info.Value.title);
                        tn.Tag = info.Value.path;
                        tn.ForeColor = FileColor;
                        t.Nodes.Add(tn);
                    }
                }
            }
            Console.WriteLine("   ..Total Year node count: " + yearNodes.Count);
        }

        
        
        //save all components
        public void Save()
        {
            if (MusicPlayer.Player.Get && !String.IsNullOrEmpty(MusicPlayer.Player.currentTrackString))
            {
                Properties.Settings.Default.LastTrack = MusicPlayer.Player.currentTrackString;
            }
            fileLoader.SaveDirectories();
        }
        
        private void MusicControl_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Player.Get)
            {
                LoadDirectoriesToTree();
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastTrack))
                {
                    MusicPlayer.Player.currentTrackString = Properties.Settings.Default.LastTrack;
                }
            }
        }

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
                        Select();
                        t.BackColor = Color.DarkBlue;
                        return;
                    }
                }
            }
        }

        public TreeNode SearchTreeView(TreeNode tree, string text)
        {
            if ((string)tree.Tag == text)
                return tree;
            if (tree.Nodes.Count > 0)
            {
                foreach (TreeNode t in tree.Nodes)
                {
                    if ((string)t.Tag == text)
                    {
                        return t;
                    }
                    if (t.Nodes.Count > 0)
                    {
                        TreeNode tn = SearchTreeView(t, text);
                        if (tn != null)
                        {
                            return tn;
                        }
                    }
                }
            }
            return null;
        }
        
        public List<string> FindFilesInNode(TreeNode node)
        {
            List<string> l = new List<string>();
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode tn in node.Nodes)
                {
                    string s = tn.Tag as string;
                    if (tool.StringCheck(s))
                        l.Add(s);
                }
            }
            return l;
        }
        
        public void OpenContaingFolder()
        {
            if (selectedNode != null)
            {
                if (selectedNode.Tag != null && selectedNode.Tag is string)
                {
                    string s = selectedNode.Tag as string;

                    if (Path.HasExtension(s))
                        Process.Start(Path.GetDirectoryName(s));
                    else
                        Process.Start(s);
                }
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenContaingFolder();
        }
        
        public void PlaySelectedNode(TreeNode node)
        {
            if (node == null)
                return;
            if (tool.IsAudioFile(node.Tag as string))
            {
                MusicPlayer.Player.PlayFile(node.Tag as string);
            }
        }
        
        //for future use
        public void MusicFileManager_DragEnter(object sender, DragEventArgs e)
        {
            tool.AllowDragEffect(e);
            if (!TotalClipboard.IsEmpty)
            {
            }
        }
        
        public void MusicFileManager_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(sender, DragDropEffects.Copy);
        }

        public void MusicFileManager_DragLeave(object sender, EventArgs e)
        {
           TotalClipboard.CopyTree(this);
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
            PlaySelectedNode(selectedNode);
        }

        public void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileLoader.SetMusicDirectory();
        }

        public void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileLoader.EditMusicDirectories();
        }
        
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDirectoriesToTree();
        }
        
        private void editID3TagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                string s = selectedNode.Tag as string;
                SongControl.CreateTagEditor(s,MusicPlayer.WinFormApp);
            }
        }
        
        public void GetNodesFolderJPG(TreeNode n)
        {
            string path = n.Tag as string;
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
            // MusicFileManager
            // 
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResumeLayout(false);
        }
    }
}
