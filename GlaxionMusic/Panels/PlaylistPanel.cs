using System;
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

namespace Glaxion.Music
{
    public partial class PlaylistPanel : UserControl,IPlaylistView
    {
        public PlaylistPanel()
        {
            playlistView = new PlaylistView();
            InitializeComponent();
            
             playlistChangedColor = Color.Orange;
            _backColor = this.BackColor;
            //Controller = new PlaylistController(this);
            playlistView.DoubleClick += PlaylistView_DoubleClick;
            playlistView.ItemDrag += TrackManager_ItemDrag;
        }

        
        public void PlayHoveredItem()
        {
            ListViewItem item = playlistView.hoveredItem;
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
            HideListChangeHighlight();
        }
        
        private void HideListChangeHighlight()
        {
            if (CurrentList == MusicPlayer.Player.playlist)
            {
                trackMenuStrip.BackColor = _backColor;
            }
        }

        private void TrackManager_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (CurrentList == MusicPlayer.Player.playlist)
                trackMenuStrip.BackColor = playlistChangedColor;
        }
        
        public void CloseDockedPanel()
        {
            playlistView.ItemDrag -= TrackManager_ItemDrag;
            if (dockSplitter != null)
                dockSplitter.Dispose();

            if (MusicPlayer.WinFormApp.dockedTrackManagers.Contains(this))
                MusicPlayer.WinFormApp.dockedTrackManagers.Remove(this);
            playlistView.Dispose();
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
            textLabel.Text = CurrentList.name;
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

        private void textLabel_Click(object sender, EventArgs e)
        {
            playlistView.UpdateMusicPlayer();
        }

        private void TrackUserControl_Load(object sender, EventArgs e)
        {
            MusicPlayer.Player.MusicUpdatedEvent += Player_MusicUpdatedEvent;
            MusicPlayer.Player.PlayEvent += playlistView.MusicPlayer_PlayEvent;
        }
        

        public void DisplayPlaylist()
        {
            playlistView.CurrentList = CurrentList;
            playlistView.DisplayPlaylist();
            UpdatePlaylistTitle();
        }
        

        public void UpdateTracks()
        {
            CurrentList.tracks = playlistView.GetTrackItems();
        }
    }
}
