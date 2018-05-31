using System;
using System.Drawing;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public partial class TracklistPanel : UserControl
    {
        public TracklistPanel()
        {
            InitializeComponent();
             playlistChangedColor = Color.Orange;
            _backColor = this.BackColor;
            tracklistView.DoubleClick += PlaylistView_DoubleClick;
            tracklistView.manager.tracksChangedDelegate = TracksChangedCallBack;
        }
        
        public Splitter dockSplitter;
        public Color playlistChangedColor;
        private Color _backColor;
        public Playlist CurrentList { get; set; }

        public void PlayHoveredItem()
        {
            ListViewItem item = tracklistView.hoveredItem;
            if (item == null)
                return;
            tracklistView.manager.UpdateMusicPlayer(item.Index);
            MusicPlayer.Instance.Play();
            item.Selected = false;
        }
        
        private void Player_MusicUpdatedEvent(object sender, EventArgs args)
        {
            if (CurrentList == MusicPlayer.Instance.playlist)
                EnableUpdateMusicButton(false);
        }
        //show/disable the update music player button
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
        //callback function for whenever the manager modifies the track list
        public void TracksChangedCallBack()
        {
            if (CurrentList == MusicPlayer.Instance.playlist)
                EnableUpdateMusicButton(true);
        }
        
        public void CloseDockedPanel()
        {
            if (dockSplitter != null)
                dockSplitter.Dispose();
            MusicPlayer.Instance.MusicUpdatedEvent -= Player_MusicUpdatedEvent;
            MusicPlayer.Instance.PlayEvent -= tracklistView.MusicPlayer_PlayEvent;
            if (MusicPlayer.WinFormApp.dockedTrackManagers.Contains(this))
                MusicPlayer.WinFormApp.dockedTrackManagers.Remove(this);
            tracklistView.Dispose();
            this.Dispose();
        }
        

        private void closeButton_Click(object sender, EventArgs e)
        {
            CloseDockedPanel();
        }

        //main method used for setting the playlist
        internal void SetPlaylist(Playlist p)
        {
            CurrentList = p;
            tracklistView.manager.CurrentList = p;
            UpdatePlaylistTitle();
        }

        //update playlist name
        public void UpdatePlaylistTitle()
        {
            playlistNameLabel.Text = CurrentList.name;
        }

        //button for saving and closing the playlist
        private void saveAndCloseButton_Click(object sender, EventArgs e)
        {
            tracklistView.manager.UpdateTracks();
            CurrentList.Save();
            CloseDockedPanel();
        }

        //button for updating and closing the playlist without saving
        private void UpdateAndCloseButton_Click(object sender, EventArgs e)
        {
            tracklistView.manager.UpdateTracks();
            CloseDockedPanel();
        }
        
        private void MakeActiveTrackPanel()
        {
            if (MusicPlayer.WinFormApp != null && MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel != this)
                MusicPlayer.WinFormApp.musicControl.CurrentPlaylistPanel = this;
        }

        private void TrackUserControl_MouseEnter(object sender, EventArgs e)
        {
            MakeActiveTrackPanel();
        }

        private void trackManager_MouseEnter(object sender, EventArgs e)
        {
            MakeActiveTrackPanel();
        }
        
        private void TrackUserControl_Load(object sender, EventArgs e)
        {
            if (MusicPlayer.Instance == null || DesignMode)
                return;
            MusicPlayer.Instance.MusicUpdatedEvent += Player_MusicUpdatedEvent;
            MusicPlayer.Instance.PlayEvent += tracklistView.MusicPlayer_PlayEvent;
            playlistNameLabel.Font = new Font(CustomFont.Exo.ff, playlistNameLabel.Font.Size);
            tracklistView.manager.Load();
        }
        
        private void updateMusicPlayerButton_Click(object sender, EventArgs e)
        {
            tracklistView.manager.UpdateMusicPlayer();
        }

        private void PlaylistView_DoubleClick(object sender, EventArgs e)
        {
            PlayHoveredItem();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tracklistView.TextSearch(textBox1.Text);
        }
        //used to clock the search box splitter.  The splitter should be clamped
        //to just below the search box
        private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            if (e.SplitY > 110)
                e.Cancel = true;
        }
    }
}
