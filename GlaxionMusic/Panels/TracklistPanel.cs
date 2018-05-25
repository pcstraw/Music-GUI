﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Tools;
using System.IO;
using BrightIdeasSoftware;
using System.Collections;

namespace Glaxion.Music
{
    public partial class PlaylistPanel : UserControl
    {
        public PlaylistPanel()
        {
            tracklistView = new TracklistView();
            InitializeComponent();
            
             playlistChangedColor = Color.Orange;
            _backColor = this.BackColor;
            //Controller = new PlaylistController(this);
            tracklistView.DoubleClick += PlaylistView_DoubleClick;
            tracklistView.ItemDrag += TrackManager_ItemDrag;
        }
        
        public void PlayHoveredItem()
        {
            ListViewItem item = tracklistView.hoveredItem;
            if (item != null)
            {
                UpdateTracks();
                MusicPlayer.Player.UpdateMusicPlayer(CurrentList, item.Index);
                MusicPlayer.Player.Play();
                item.Selected = false;
            }
        }

        private void PlaylistView_DoubleClick(object sender, EventArgs e)
        {
            PlayHoveredItem();
        }

        public Splitter dockSplitter;
        public Color playlistChangedColor;
        private Color _backColor;
        public Playlist CurrentList { get; set; }

        private void Player_MusicUpdatedEvent(object sender, EventArgs args)
        {
            if (CurrentList == MusicPlayer.Player.playlist)
            {
                EnableUpdateMusicButton(false);
            }
        }
        
        public void EnableUpdateMusicButton(bool show)
        {
            if(show)
            {
                updateMusicPlayerButton.Enabled = true;
                updateMusicPlayerButton.Visible = true;
            }
            else
            {
                updateMusicPlayerButton.Enabled = false;
                updateMusicPlayerButton.Visible = false;
            }
        }

        private void TrackManager_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (CurrentList == MusicPlayer.Player.playlist)
                EnableUpdateMusicButton(true);
        }
        
        public void CloseDockedPanel()
        {
            tracklistView.ItemDrag -= TrackManager_ItemDrag;
            if (dockSplitter != null)
                dockSplitter.Dispose();

            if (MusicPlayer.WinFormApp.dockedTrackManagers.Contains(this))
                MusicPlayer.WinFormApp.dockedTrackManagers.Remove(this);
            tracklistView.Dispose();
            this.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            CloseDockedPanel();
        }

        private void menuStrip1_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Visible = false;
        }

        private void trackMenuStrip_Enter(object sender, EventArgs e)
        {
            MessageBox.Show("Empty");
            closeButton.Visible = true;
        }

        private void trackMenuStrip_MouseHover(object sender, EventArgs e)
        {
            closeButton.Visible = true;
        }
        
        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.Visible = true;
        }
      

        internal void SetPlaylist(Playlist p)
        {
            CurrentList = p;
            tracklistView.manager.CurrentList = p;
            UpdatePlaylistTitle();
            
           // objectListView1.AddObjects(CurrentList.tracks);
           
            // objectListView1.RefreshSelectedObjects();
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Visible = false;
        }

        private void saveAndCloseButton_Click(object sender, EventArgs e)
        {
            UpdateTracks();
            CurrentList.Save();
            CloseDockedPanel();
        }

        private void UpdateAndCloseButton_Click(object sender, EventArgs e)
        {
            UpdateTracks();
            CloseDockedPanel();
        }

        public void UpdatePlaylistTitle()
        {
            playlistNameLabel.Text = CurrentList.name;
        }

        private void TrackUserControl_MouseEnter(object sender, EventArgs e)
        {
            if (MusicPlayer.WinFormApp != null && MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel != this)
                MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel = this;
        }

        private void trackManager_MouseEnter(object sender, EventArgs e)
        {
            if (MusicPlayer.WinFormApp != null && MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel != this)
                MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel = this;
        }
        
        private void TrackUserControl_Load(object sender, EventArgs e)
        {
            MusicPlayer.Player.MusicUpdatedEvent += Player_MusicUpdatedEvent;
            MusicPlayer.Player.PlayEvent += tracklistView.MusicPlayer_PlayEvent;
        }
        
        public void UpdateTracks()
        {
            CurrentList.tracks = tracklistView.GetTrackItems();
        }

        private void updateMusicPlayerButton_Click(object sender, EventArgs e)
        {
            tracklistView.manager.UpdateMusicPlayer();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tracklistView.TextSearch(textBox1.Text);
        }

        private void PlaylistPanel_MouseHover(object sender, EventArgs e)
        {
            textBox1.Visible = false;
        }
    }
}