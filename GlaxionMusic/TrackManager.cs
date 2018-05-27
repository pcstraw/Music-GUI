﻿using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Glaxion.Music
{

    public interface ITracklistView
    {
        //void GetSelectedItems();
        void DisplayPlaylist();
        //void RefreshUI();
    }

    public class TrackManager : VListView
    {
        public TrackManager(ITracklistView trackInterface, IListView view,ColorScheme scheme) : base(view)
        {
            _trackview = trackInterface;
            CurrentColors = scheme;
        }
        
        
        public delegate void FindTrackEventHander(object sender, EventArgs args);
        public event FindTrackEventHander FindEvent;
        //public VItem rightClickedItem;
        //public VItem leftClickedItem;
        ITracklistView _trackview;
        public List<VItem> preContextSelection = new List<VItem>();
        private bool playing_track_changed;
        private string last_playing_track;

        public Playlist CurrentList { get; set; }
        public VItem IsPlayingItem { get; private set; }

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
            MusicPlayer.Player.TrackChangeEvent += Player_TrackChangeEvent;
        }

        private void Player_TrackChangeEvent(object sender, EventArgs args)
        {
            int current_index = (int)sender;
            if (current_index < 0)
                return; //no track
        }

        public void UpdateColours()
        {
            //find the current playing item
            if (last_playing_track != MusicPlayer.Player.currentTrackString)
            {
                last_playing_track = MusicPlayer.Player.currentTrackString;
                playing_track_changed = true;
            }
            for(int index = 0;index < ItemCount; index++)
            {
                VItem i = Items[index];
                string track_path = i.Name;
                if (!File.Exists(track_path))
                {
                    i.State = ItemState.Missing; //track does not exist
                    //i.HighLightColors(new ColorScheme(Color.Black, Color.Orange));
                    continue;
                }
                if (track_path == MusicPlayer.Player.currentTrackString)
                {
                    if (CurrentList == MusicPlayer.Player.playlist)
                    {
                        //if (index == MusicPlayer.Player.currentTrack)
                        i.State = ItemState.IsThePlayingTrack;
                        //else
                         //   i.State = ItemState.IsPlaying;
                        //i.HighLightColors(new ColorScheme(Color.Black, MusicPlayer.PlayColor));
                        continue;
                    }
                    i.State = ItemState.IsPlayingInOtherPanel; //track is playing in another docked panel
                    continue;
                }
                //last played tracks from 0-3 with 0 being the current track
                if (i.State != ItemState.Normal)
                {
                    ItemState _i = i.State;
                    switch (_i)
                    {
                        case ItemState.IsThePlayingTrack:
                            if (playing_track_changed)
                                i.State = ItemState.IsPlaying;
                            break;
                        case ItemState.IsPlaying:
                            if(playing_track_changed)
                                i.State = ItemState.WasPlaying;
                            break;
                        case ItemState.WasPlaying:
                            {
                                if(playing_track_changed)
                                {
                                    i.State = ItemState.Reset;
                                }
                                //if(track_path != last_string)
                                 //   i.State = ItemState.Reset;
                                //i.RestoreColors();
                            }
                            break;
                        case ItemState.IsPlayingInOtherPanel:
                            {
                                i.State = ItemState.Reset;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            playing_track_changed = false;
            _view.RefreshColors();
        }
        
        public List<string> GetTrackItems()
        {
            List<string> list = new List<string>();
            foreach (VItem li in Items)
            {
                list.Add(li.Name);
            }
            return list;
        }
        
        internal void MusicPlayer_PlayEvent(object sender, EventArgs args)
        {
            
            if (base.Items.Count == 0)
            {
                tool.show(3,"Check item count...possibly empty subscribers");
                return;
            }
            if (CurrentList == MusicPlayer.Player.playlist)
            {
                if (sender is int)
                {
                    int index = (int)sender;
                    IsPlayingItem = this.Items[index];
                    IsPlayingItem.State = ItemState.IsThePlayingTrack;
                }
            }
            UpdateColours();
        }
        
        //dep?
        public void UpdateMusicPlayer(int currentTrackIndex)
        {
            UpdateTracks();
            //load the list view items into the playlist
            int current_track_index = currentTrackIndex;
            MusicPlayer.Player.UpdateMusicPlayer(CurrentList, current_track_index);
        }
        
        //call when changing the playlist name.  This will not save the file, only uipdate the name
        public void UpdatePlaylistName(string name)
        {
            CurrentList.UpdateName(name);
        }
        //require override derived method instead?
        public void DropMusicFiles(List<string> files, int index)
        {
            //StoreCurrentState();
            // 
            // tool.show(5, "Show");
            //SelectedItems.Clear();
            foreach (string file in files)
            {
                if (tool.IsAudioFile(file))
                {
                    VItem item = CreateItem(file);
                    base.Insert(index, item);
                    int i= base.IndexOf(item);
                    item.Selected = true;
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
        //required overrided  method
        public override VItem CreateItem(string file)
        {
            VItem i = new VItem();
            i.Columns[0] = Path.GetFileNameWithoutExtension(file);
            i.Columns[1] = file;
            i.SetColors(CurrentColors);
            i.Name = file;
            Song song = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(file);
            if (song != null)
            {
                i.Tag = song;
            }
            return i;
        }

        public void UpdateTracks()
        {
            CurrentList.tracks = GetTrackItems();
        }

        internal void UpdateMusicPlayer()
        {
            int index = Items.IndexOf(IsPlayingItem);
            UpdateMusicPlayer(index);
        }

        internal void OpenChromeSearch(int index)
        {
            if (index > Items.Count)
                return;
            
            VItem item = Items[index];
            Song s = item.Tag as Song;
            var t= s.artist;
            Process.Start("http://google.com/search?q=" + t);
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
