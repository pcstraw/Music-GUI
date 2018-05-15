using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Glaxion.Tools;
using System.Diagnostics;

namespace Glaxion.Music
{
    public partial class PlaybackStateControl : UserControl
    {
        public PlaybackStateControl()
        {
            InitializeComponent();
        }
        
        protected void On_Play(object o, EventArgs e)
        {
            SetPauseIcon();
            UpdateName();
        }

        protected void On_Pause(object o, EventArgs e)
        {
            SetPlayIcon();
        }

        protected void On_MediaLoaded(object o, EventArgs e)
        {
            if(MusicPlayer.Player.Get && currentTrack != MusicPlayer.Player.currentTrack)
                currentTrack = MusicPlayer.Player.currentTrack;
        }
        
        protected void On_Resume(object o, EventArgs e)
        {
            SetPauseIcon();
        }

        public void SetPauseIcon()
        {
            resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
        }

        public void SetPlayIcon()
        {
            resumeButton.BackgroundImage = Properties.Resources.media_play_pause_resume;
        }
        public int currentTrack;

        public void UpdateName()
        {
            if (MusicPlayer.Player.Get)
            {
                nameLabel.Text = Path.GetFileNameWithoutExtension(MusicPlayer.Player.currentTrackString);
            }
        }

        public void Play(int track)
        {
            if (MusicPlayer.Player.Get)
                MusicPlayer.Player.Play(track);
            if (currentTrack != MusicPlayer.Player.currentTrack)
                 UpdateName();
        }

        public void Resume()
        {
            if (MusicPlayer.Player.Get)
            {
                if (currentTrack == MusicPlayer.Player.currentTrack)
                {
                    MusicPlayer.Player.Resume(MusicPlayer.Player.currentTrackString, MusicPlayer.Player.positionIndex);
                }
                else
                {
                    MusicPlayer.Player.Play(currentTrack);
                }
            }
        }

        public void Play(string track)
        {
            MusicPlayer.Player.PlayFile(track);
        }

        public void Pause()
        {
            MusicPlayer.Player.Pause();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!MusicPlayer.Player.Get)
                return;

            if (MusicPlayer.Player.IsPlaying)
            {
                Pause();
                return;
            }
            
            if (MusicPlayer.Player.IsPaused)
            {
                Resume();
                return;
            }
            if (MusicPlayer.Player.HasStopped())
            {
                Play(MusicPlayer.Player.currentTrackString);
                return;
            }
        }

        //replay?
        private void button2_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.Replay();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if(MusicPlayer.Player.Get)
            {
                MusicPlayer.Player.NextTrack();
               // currentTrack = MusicPlayer.Player.currentTrack;
                if(MusicPlayer.Player.IsPlaying)
                    resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
  
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (MusicPlayer.Player.Get )
            {
                MusicPlayer.Player.PrevTrack();
                currentTrack = MusicPlayer.Player.currentTrack;
                resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();

            }
        }

        private void PlaybackStateControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode && MusicPlayer.Player != null)
            {
                MusicPlayer.Player.PlayEvent += new MusicPlayer.PlayEventHandler(On_Play);
                MusicPlayer.Player.PauseEvent += new MusicPlayer.PauseEventHandler(On_Pause);
                MusicPlayer.Player.ResumeEvent += new MusicPlayer.ResumeEventHandler(On_Resume);
                MusicPlayer.Player.MediaLoadedEvent += new MusicPlayer.MediaLoadedEventHandler(On_MediaLoaded);
                hideNameButton.BackColor = this.BackColor;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tool.OpenFileDirectory(MusicPlayer.Player.currentTrackString);
            //MusicPlayer.Stop();
            //SetPlayIcon();
        }

        private void nameLabel_TextChanged(object sender, EventArgs e)
        {
            string text = nameLabel.Text;
            Label lbl = nameLabel;
            using (Graphics g = CreateGraphics())
            {
                SizeF size = g.MeasureString(text, lbl.Font, lbl.Width);
                lbl.Height = (int)Math.Ceiling(size.Height);
                lbl.Text = text;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.Stop();
            SetPlayIcon();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (MusicPlayer.Player.playlist == null)
            {
                tool.show();
                return;
            }
            MusicPlayer.Player.PlayPlaylist(MusicPlayer.Player.playlist, MusicPlayer.Player.playlist.trackIndex);
        }

        private void loopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loopCheckBox.Checked == true)
                MusicPlayer.Player.Loop = true;
            else
                MusicPlayer.Player.Loop = false;
        }

        private void PlaybackStateControl_MouseHover(object sender, EventArgs e)
        {
            Focus();
        }

        private void nameLabel_MouseHover(object sender, EventArgs e)
        {
            tool.debug();
        }

        private void nameLabel_MouseEnter(object sender, EventArgs e)
        {
            nameLabel.Visible = true;                     
            nameLabel.Show();
        }
        
        private void hideNameButton_Click(object sender, EventArgs e)
        {
            if(nameLabel.Visible == false)
            {
                nameLabel.Visible = true;
                nameLabel.Show();
                return;
            }
            else
            {
                nameLabel.Visible = false;
            }
        }
        
        public void PicknNameLabelBackColor()
        {
            Color col = tool.PickColor();
            if (col != Color.Empty)
                nameLabel.BackColor = col;
        }

        public void PicknNameLabelForeColor()
        {
            Color col = tool.PickColor();
            if (col != Color.Empty)
                nameLabel.ForeColor = col;
        }
        
        private void nameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PicknNameLabelBackColor();
            }
            if (e.Button == MouseButtons.Middle)
            {
                PicknNameLabelForeColor();
            }
        }
    }
}
