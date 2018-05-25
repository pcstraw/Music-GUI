using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Glaxion.Music
{
    public class MusicFileManager : VTree
    {
        public MusicFileManager(ITreeView ViewInterface,ColorScheme colors)
        {
            //InitializeComponent();
            //suppose to help prevent start up crash by loading in the handle.
            //mayeb stick this in the base class as well?
            //var foo = this.Handle;
            if (MusicPlayer.Player != null)
                MusicPlayer.Player.DirectoriesAddedEvent += DirectoryAdded;
            _view = ViewInterface;
            FileColor = Color.WhiteSmoke;
            loadID3Tags = true;
            Colors = colors;
            listGUIcallack = new ListGUICallBack(ListGUIDelegateCallback);
        }
        public ColorScheme Colors;
        public FileLoader fileLoader;
        public List<VNode> albumNodes = new List<VNode>();
        public List<VNode> artistNodes = new List<VNode>();
        //public List<VNode> musicFileNodes = new List<VNode>();
        public List<VNode> yearNodes = new List<VNode>();
        bool loadID3Tags;
        public Color FileColor;

        public void Load()
        {
            fileLoader = MusicPlayer.Player.fileLoader;
            LoadDirectoriesToTree();
        }

        protected void DirectoryAdded(object o, EventArgs e)
        {

        }

        public void PlayDirectory(List<VNode> nodes)
        {
            foreach (VNode t in nodes)
            {
                if (Path.HasExtension(t.name))
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
        /*
        public void AssignEventHandlers()
        {
            ItemDrag += MusicFileManager_ItemDrag;
            MouseDoubleClick += MusicFileManager_MouseDoubleClick;
            MouseDown += MusicFileManager_MouseDown;
            DragDrop += MusicFileManager_DragDrop;
        }
        */

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
            if (!InternalClipboard.IsEmpty)
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
                tool.show(2, "External Drag Drop is not yet supported");
                return;
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
        public VNode LoadFilesFromDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                return null;

            VNode rn = new VNode(Path.GetFileName(dir));
           // rn.Tag = dir;
            rn.name = dir;
            rn.ForeColor = Colors.foreColor;
            rn.BackColor = Colors.backColor;
            bool containsAudio = false;

            try
            {
                IEnumerable<string> files = tool.LoadAudioFiles(dir, SearchOption.TopDirectoryOnly);
                IEnumerable<string> subDir = Directory.EnumerateDirectories(dir);
                if (files.Count() > 0)
                    containsAudio = true;

                foreach (string s in subDir)
                {
                    VNode tn = LoadFilesFromDirectory(s);
                    
                    if (tn != null)
                    {
                        tn.name = s;
                        tn.ForeColor = Colors.foreColor;
                        tn.BackColor = Colors.backColor;
                       // tn.Tag = s;
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
                    VNode tn = LoadDirectory(s);
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
                tool.show(5, "Exception loading music file:", e.Message);
                return null;
            }
        }

       
        private void AddFilesToNode(VNode node, IEnumerable<string> files)
        {
            foreach (string f in files)
            {
                VNode tn = new VNode();
                tn.Text = Path.GetFileName(f);
                //tn.Tag  = f;
                tn.name = f;
                //tn.ForeColor = FileColor;
                tn.isFile = true;
                tn.ForeColor = FileColor;
                tn.BackColor = Colors.backColor;
                //tn.StateImageIndex = audioFileImadeIndex;
                node.Nodes.Add(tn);
                fileLoader.AddMusicFile(f);
            }
            //causing VNode.clone implementation to fail.  not all that surprising
            /*
            Parallel.ForEach(files, f =>
            {
                VNode tn = new VNode();
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
            SetTree(Nodes, searchText);
        }
        
        public void LoadArtistNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (artistNodes.Count == 0)
                BuildArtistTree();
            _view.PopulateTree(artistNodes);
           // SetTree(artistNodes, searchText);
        }
        ListGUICallBack listGUIcallack;
        void ListGUIDelegateCallback(bool ok)
        {
            if (ok)
                LoadDirectoriesToTree();
        }
        //use delegate to reload library
        public void EditMusicDirectories()
        {
            ListGUI lg = new ListGUI(fileLoader.MusicDirectories, true);
            lg.callback = listGUIcallack;
        }

        public void LoadAlbumNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (albumNodes.Count == 0)
                BuildAlbumTree();
            _view.PopulateTree(albumNodes);
            //SetTree(albumNodes, searchText);
        }
        public void LoadYearNodesToView()
        {
            if (!fileLoader.HasLoadedTags())
                return;
            if (yearNodes.Count == 0)
                BuildYearTree();
            _view.PopulateTree(yearNodes);
            //SetTree(yearNodes, searchText);
        }

        public void LoadAudioFiles()
        {
            //start afresh
            Nodes.Clear();
            fileLoader.musicDirectories.Clear();
            albumNodes.Clear();
            artistNodes.Clear();
            yearNodes.Clear();
            foreach (string s in fileLoader.MusicDirectories)
            {
                if (!fileLoader.musicDirectories.Contains(s))
                {
                    VNode tn = LoadFilesFromDirectory(s);
                    if (tn == null)
                        continue;
                    tn.Text = s;
                    tn.name = s;
                    //tn.StateImageIndex = -1;
                    if (tn.Nodes.Count > 0)
                    {
                        Nodes.Add(tn);
                    }
                }
            }
            if (Nodes.Count > 0)
            {
                Nodes[0].Expand = true;
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
            _view.PopulateTree(Nodes);
            CacheNodes();
            if (tool.StringCheck(searchText))
                SearchForText(searchText);

            if (loadID3Tags)
                fileLoader.LoadID3Library();
            DateTime endTime = DateTime.Now;
            // double end_in_seconds = TimeSpan.FromTicks(endTime.Ticks).TotalSeconds;
            //tool.debug("End Time: " + endTime);
            double elapsed_seconds = (endTime - startTime).TotalSeconds;
            tool.debug("elapised time: " + elapsed_seconds);
            
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
                VNode tn = new VNode(s);
                tn.name = s;
                tn.ForeColor = Colors.foreColor;
                tn.BackColor = Colors.backColor;
                albumNodes.Add(tn);
                i++;
            }

            foreach (VNode t in albumNodes)
            {
                foreach (KeyValuePair<string, Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.album)
                    {
                        VNode tn = new VNode(info.Value.title);
                        tn.name = info.Value.path;
                        tn.ForeColor = FileColor;
                        tn.BackColor = Colors.backColor;
                        tn.isFile = true;
                        //tn.ForeColor = FileColor;
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
                VNode tn = new VNode(s);
                tn.name = s;
                tn.ForeColor = Colors.foreColor;
                tn.BackColor = Colors.backColor;
                artistNodes.Add(tn);
            }

            foreach (VNode t in artistNodes)
            {
                foreach (KeyValuePair<string, Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.artist)
                    {
                        VNode tn = new VNode(info.Value.title);
                        tn.name = info.Value.path;
                        tn.ForeColor = FileColor;
                        tn.BackColor = Colors.backColor;
                        tn.isFile = true;
                        //tn.ForeColor = FileColor;
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
                VNode tn = new VNode(s);
                tn.name = s;
                tn.ForeColor = Colors.foreColor;
                tn.BackColor = Colors.backColor;
                yearNodes.Add(tn);
            }

            foreach (VNode t in yearNodes)
            {
                foreach (KeyValuePair<string, Song> info in MusicPlayer.Player.fileLoader.trackInfoManager.trackInfos)
                {
                    if (t.Text == info.Value.year)
                    {
                        VNode tn = new VNode(info.Value.title);
                        tn.name = info.Value.path;
                        tn.ForeColor = FileColor;
                        tn.BackColor = Colors.backColor;
                        tn.isFile = true;
                        //tn.ForeColor = FileColor;
                        t.Nodes.Add(tn);
                    }
                }
            }
            Console.WriteLine("   ..Total Year node count: " + yearNodes.Count);
        }

        //save all components
        public void Save()
        {
            if (!String.IsNullOrEmpty(MusicPlayer.Player.currentTrackString))
            {
                Properties.Settings.Default.LastTrack = MusicPlayer.Player.currentTrackString;
            }
            fileLoader.SaveDirectories();
        }

        private void MusicControl_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Player != null)
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
            VNode t = null;
            if (Nodes.Count > 0)
            {
                foreach (VNode directory in Nodes)
                {
                    t = SearchTreeView(directory, s);
                    if (t != null)
                    {
                        t.selected = true;
                        t.Expand = true;
                        //Select();
                        //t.BackColor = Color.DarkBlue;
                        return;
                    }
                }
            }
        }

        public VNode SearchTreeView(VNode tree, string text)
        {
            if ((string)tree.name == text)
                return tree;
            if (tree.Nodes.Count > 0)
            {
                foreach (VNode t in tree.Nodes)
                {
                    if ((string)t.name == text)
                    {
                        return t;
                    }
                    if (t.Nodes.Count > 0)
                    {
                        VNode tn = SearchTreeView(t, text);
                        if (tn != null)
                        {
                            return tn;
                        }
                    }
                }
            }
            return null;
        }

        public List<string> FindFilesInNode(VNode node)
        {
            List<string> l = new List<string>();
            if (node.Nodes.Count > 0)
            {
                foreach (VNode tn in node.Nodes)
                {
                    string s = tn.name;
                    if (tool.StringCheck(s))
                        l.Add(s);
                }
            }
            return l;
        }

        /*
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
        */
        
        public void PlaySelectedNode(VNode node)
        {
            if (node == null)
                return;
            if (tool.IsAudioFile(node.name))
            {
                MusicPlayer.Player.PlayFile(node.name);
            }
        }
        
        public void GetNodesFolderJPG(VNode n)
        {
            string path = n.name;
            if (!tool.StringCheck(path))
                return;
            string result = tool.GetFolderJPGFile(path);
            if (tool.StringCheck(result) && File.Exists(result))
                Process.Start(result);
        }
    }
}
