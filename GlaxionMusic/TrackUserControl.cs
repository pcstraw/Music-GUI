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

namespace Glaxion.Music
{
    public partial class TrackUserControl : UserControl
    {
        public TrackUserControl()
        {
            InitializeComponent();
            trackManager.ItemDrag += TrackManager_ItemDrag;
             playlistChangedColor = Color.Orange;
            _backColor = this.BackColor;
        }

        public Splitter dockSplitter;
        public Color playlistChangedColor;
        private Color _backColor;
        
        private void Player_MusicUpdatedEvent(object sender, EventArgs args)
        {
            HideListChangeHighlight();
        }
        
        private void HideListChangeHighlight()
        {
            if (trackManager.currentList == MusicPlayer.Player.playlist)
            {
                trackMenuStrip.BackColor = _backColor;
            }
        }

        private void TrackManager_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if(trackManager.currentList == MusicPlayer.Player.playlist)
                trackMenuStrip.BackColor = playlistChangedColor;
        }
        
        public void CloseDockedPanel()
        {
            trackManager.ItemDrag -= TrackManager_ItemDrag;
            if (dockSplitter != null)
                dockSplitter.Dispose();

            if (MusicPlayer.WinFormApp.dockedTrackManagers.Contains(this))
                MusicPlayer.WinFormApp.dockedTrackManagers.Remove(this);
            trackManager.currentList = null;
            trackManager.Dispose();
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
            trackManager.UpdatePlaylist();
            trackManager.currentList.Save();
            CloseDockedPanel();
        }

        private void UpdateAndCloseButton_Click(object sender, EventArgs e)
        {
            trackManager.UpdatePlaylist();
            CloseDockedPanel();
        }

        public void UpdatePlaylistTitle()
        {
            textLabel.Text = trackManager.currentList.name;
        }

        private void TrackUserControl_MouseEnter(object sender, EventArgs e)
        {
            if (MusicPlayer.WinFormApp != null && MusicPlayer.WinFormApp.musicControl.trackUser != this)
                MusicPlayer.WinFormApp.musicControl.trackUser = this;
        }

        private void trackManager_MouseEnter(object sender, EventArgs e)
        {
            if (MusicPlayer.WinFormApp != null && MusicPlayer.WinFormApp.musicControl.trackUser != this)
                MusicPlayer.WinFormApp.musicControl.trackUser = this;
        }

        private void textLabel_Click(object sender, EventArgs e)
        {
            trackManager.UpdateMusicPlayer();
        }

        private void TrackUserControl_Load(object sender, EventArgs e)
        {
            MusicPlayer.Player.MusicUpdatedEvent += Player_MusicUpdatedEvent;
            
        }
    }
}
