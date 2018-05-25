using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Glaxion.Music
{

    public interface ITracklistView
    {
        void GetSelectedItems();
        void DisplayPlaylist();
        //void RefreshUI();
    }

    public class TrackManager : VListView
    {
        public void Construction()
        {
           // lastSelectedIndidex = new SelectedIndexCollection(this);
            /*
            OwnerDraw = true;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            DragDrop += TrackManager_DragDrop;
            DragEnter += ListBox_DragEnter;
            DrawItem += TrackManager_CustomDraw;
            DrawColumnHeader += TrackManager_DrawColumnHeader;
            ColumnClick += listbox_ColumnClick;
            this.Columns[1].Width = 1000;
            */
        }

        public TrackManager(ITracklistView trackInterface,ColorScheme colors)
        {
            Construction();
            _view = trackInterface;
            CurrentColors = colors;
        }

        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;
        //public VItem rightClickedItem;
        //public VItem leftClickedItem;
        ITracklistView _view;
        public List<VItem> preContextSelection = new List<VItem>();
        public ColorScheme CurrentColors;
        public Playlist CurrentList { get; set; }

        //public Color prevRightClickedItemForeColor;
        //public string currentTrack;
        //public bool autoUpdateTracks;
       // public bool autoUpdateMusicPlayer;
       // public bool IsDockedPanel;
       // public bool ControlDown;
       // public bool currentListChanged;
       // bool unselectClick;
        //public int lastVisible;
        //public string editingProgram;
        // ListViewItem unselectItem;
        // public List<ListViewItem> CopiedList = new List<ListViewItem>();
        //List<ListViewItem> contextItems = new List<ListViewItem>();
        //SelectedIndexCollection lastSelectedIndidex;

        public void Load()
        {
            MusicPlayer.Player.PlayEvent += MusicPlayer_PlayEvent;
        }
        
        public void UpdateColours()
        {
            //find the current playing item
            foreach (VItem i in Items)
            {
                string track_path = i.Columns[1];
                if (!File.Exists(track_path))
                {
                    i.State = -1; //track does not exist
                    //i.HighLightColors(new ColorScheme(Color.Black, Color.Orange));
                    continue;
                }
                if (track_path == MusicPlayer.Player.currentTrackString)
                {
                    if (CurrentList == MusicPlayer.Player.playlist && i.Index == MusicPlayer.Player.currentTrack)
                    {
                        i.State = 1;
                        //i.HighLightColors(new ColorScheme(Color.Black, MusicPlayer.PlayColor));
                        continue;
                    }
                    i.State = 10; //track is playing in another docked panel
                    continue;
                }
                //last played tracks from 0-3 with 0 being the current track
                else if (i.State != 0)
                {
                    int index = i.State;
                    switch (index)
                    {
                        case 1:
                            i.State = 2;
                            break;
                        case 2:
                            {
                                i.State = 0;
                                //i.RestoreColors();
                            }
                            break;
                        case 10:
                            {
                                i.State = 0;
                               // i.RestoreColors();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            //_view.RefreshColors();
        }

        internal void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            UpdateColours();
        }
        
        public void CheckForRepeat()
        {
            foreach (VItem item in SelectedItems)
            {
                if (item == null)
                    continue;
                string file = item.Columns[1];
                if (!tool.StringCheck(file))
                    continue;

                foreach (VItem other in Items)
                {
                    string s = other.Columns[1];
                    if (!tool.StringCheck(s))
                        continue;
                    if (s == file)
                        other.Selected = true;
                }
            }
        }
        
        public void MoveSelectedTracksTo(int index)
        {
            //preContextSelection.Reverse();
            _view.GetSelectedItems();
            foreach (VItem i in SelectedItems)
            {
                Items.Remove(i);
                Items.Insert(index + 1, i);
                i.Selected = true;
               // i.BackColor = this.BackColor;
            }
            //_view.RefreshUI();
        }

        public void RemoveSelectedTracks()
        {
            //StoreCurrentState();
            foreach (VItem item in SelectedItems)
                Items.Remove(item);
            SelectedItems.Clear();
        }

        public void UpdateMusicPlayer()
        {
            //load the list view items into the playlist
            int current_track_index = MusicPlayer.Player.currentTrack;
            //find the item who's first subtag is 1.  this is the currently playing track
            //storing it on the item allows us to track the last played song as the user re-orders the playlist
            for(int index =0; index < Items.Count; index++)
            {
                VItem item = Items[index];
                //if (item.State == 0)
                //    continue;
                int i = item.State;
                if (i == 1)
                {
                    string s = item.name;
                    if (s == MusicPlayer.Player.currentTrackString) //double check to see if is still the current track
                    {
                        //grab the new index
                        current_track_index = index;
                        break;
                    }
                }
            }
            CurrentList.tracks = GetTrackItems();
            MusicPlayer.Player.UpdateMusicPlayer(CurrentList, current_track_index);
            //MusicPlayer.Player.playlist = CurrentList;
           // MusicPlayer.Player.currentTrack = current_track_index;
        }

        private List<string> GetTrackItems()
        {
            List<string> list = new List<string>();
            foreach (VItem li in Items)
            {
                list.Add(li.name);
            }
            return list;
        }

        //call when changing the playlist name.  This will not save the file, only uipdate the name
        public void UpdatePlaylistName(string name)
        {
            CurrentList.UpdateName(name);
        }
        

        public void DropMusicFiles(List<string> files, int index)
        {
            //StoreCurrentState();
            // 
            // tool.show(5, "Show");
            //SelectedItems.Clear();
            bool filesdropped = false;
            foreach (string file in files)
            {
                if (tool.IsAudioFile(file))
                {
                    VItem item = CreateItem(file);
                    Items.Insert(index, item);
                    item.Index = Items.IndexOf(item);
                    SelectedItems.Add(item);
                    filesdropped = true;
                    continue;
                }
                
                if (!Path.HasExtension(file))
                {
                    List<string> dirf = tool.LoadAudioFiles(file, SearchOption.TopDirectoryOnly);
                    DropMusicFiles(dirf, index);
                    return;
                }
            }
        }

        public VItem CreateItem(string file)
        {
            VItem i = new VItem();
            i.Columns.Add(Path.GetFileNameWithoutExtension(file));
            i.Columns.Add(file);
            i.Tag = file; //use song instead
            i.SetColors(CurrentColors);
            return i;
        }

        public void FindMissingFiles()
        {
            foreach (VItem i in Items)
            {
                if (i.Tag.GetType() != typeof(string))
                    throw new Exception("No file path set");

                if (!File.Exists(i.Tag as string))
                    i.SetColors(new ColorScheme(Color.Black,MusicPlayer.MissingColor));
            }
        }
        
        internal void MoveSelectedToBottom()
        {
            foreach (VItem i in SelectedItems)
            {
                Items.Remove(i);
                Items.Insert(Items.Count, i);
            }
        }

        internal void MoveSelectedToTop()
        {
            foreach (VItem i in SelectedItems)
            {
                Items.Remove(i);
                Items.Insert(0, i);
            }
        }
      
        public void ShowLastSelected(ColorScheme colors)
        {
            foreach (VItem i in SelectedItems)
            {
                i.HighLightColors(colors);
            }
        }
        
        public void ClearLastSelectedDisplay()
        {
            foreach (VItem i in SelectedItems)
                i.RestoreColors();
        }
        
        /*
        public void DisplayPlaylist()
        {
            Items.Clear();
            CurrentList.UpdatePaths();
            foreach (string track in CurrentList.tracks)
            {
                VItem i = CreateItem(track);
                Items.Add(i);
                if (!File.Exists(track))
                    i.State = -1; //track does not exist
            }
            UpdateColours();
        }
        */
    }
}
