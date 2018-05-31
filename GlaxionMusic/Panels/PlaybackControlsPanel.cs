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
            if(MusicPlayer.Instance  != null && currentTrack != MusicPlayer.Instance.currentTrack)
                currentTrack = MusicPlayer.Instance.currentTrack;
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
            if (MusicPlayer.Instance  != null)
            {
                nameLabel.Text = Path.GetFileNameWithoutExtension(MusicPlayer.Instance.currentTrackString);
            }
        }

        public void Play(int track)
        {
            MusicPlayer.Instance.Play(track);
            if (currentTrack != MusicPlayer.Instance.currentTrack)
                 UpdateName();
        }

        public void Resume()
        {
            if (currentTrack == MusicPlayer.Instance.currentTrack)
            {
                MusicPlayer.Instance.Resume(MusicPlayer.Instance.currentTrackString, MusicPlayer.Instance.positionIndex);
            }
            else
            {
                MusicPlayer.Instance.Play(currentTrack);
            }
        }

        public void Play(string track)
        {
            MusicPlayer.Instance.PlayFile(track);
        }

        public void Pause()
        {
            MusicPlayer.Instance.Pause();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MusicPlayer.Instance.IsPlaying)
            {
                Pause();
                return;
            }
            
            if (MusicPlayer.Instance.IsPaused)
            {
                Resume();
                return;
            }
            if (MusicPlayer.Instance.HasStopped())
            {
                Play(MusicPlayer.Instance.currentTrackString);
                return;
            }
        }

        //replay?
        private void button2_Click(object sender, EventArgs e)
        {
            MusicPlayer.Instance.Replay();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
                MusicPlayer.Instance.NextTrack();
               // currentTrack = MusicPlayer.Instance.currentTrack;
                if(MusicPlayer.Instance.IsPlaying)
                    resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
  
            
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Instance.PrevTrack();
            currentTrack = MusicPlayer.Instance.currentTrack;
            resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
        }

        private void PlaybackStateControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode && MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.PlayEvent += new MusicPlayer.PlayEventHandler(On_Play);
                MusicPlayer.Instance.PauseEvent += new MusicPlayer.PauseEventHandler(On_Pause);
                MusicPlayer.Instance.ResumeEvent += new MusicPlayer.ResumeEventHandler(On_Resume);
                MusicPlayer.Instance.MediaLoadedEvent += new MusicPlayer.MediaLoadedEventHandler(On_MediaLoaded);
                hideNameButton.BackColor = this.BackColor;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tool.OpenFileDirectory(MusicPlayer.Instance.currentTrackString);
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
            MusicPlayer.Instance.Stop();
            SetPlayIcon();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            MusicPlayer.Instance.PlayPlaylist(MusicPlayer.Instance.playlist, MusicPlayer.Instance.playlist.trackIndex);
        }

        private void loopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loopCheckBox.Checked == true)
                MusicPlayer.Instance.Loop = true;
            else
                MusicPlayer.Instance.Loop = false;
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
