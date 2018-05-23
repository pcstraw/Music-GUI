using System;
using System.Drawing;
using System.Windows.Forms;
using Glaxion.Tools;

namespace Glaxion.Music
{
    public partial class PlaybackTrackbarControl : UserControl
    {
        public PlaybackTrackbarControl()
        {
            InitializeComponent();
        }
        public int currentTrack;

        public void Pause()
        {
            MusicPlayer.Player.Pause();
        }

        public void Resume()
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

        public void Play()
        {
            MusicPlayer.Player.PlayFile(MusicPlayer.Player.currentTrackString);
        }

        public void SetPauseIcon()
        {
            resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
        }
        public void SetPlayIcon()
        {
            resumeButton.BackgroundImage = Properties.Resources.media_play_pause_resume;
        }

        private void PlaybackTrackbarControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode && MusicPlayer.Player != null)
            {
                MusicPlayer.Player.PlayEvent += Player_PlayEvent;
                MusicPlayer.Player.ResumeEvent += Player_ResumeEvent;
                MusicPlayer.Player.PauseEvent += Player_PauseEvent;
                MusicPlayer.Player.MediaLoadedEvent += Player_MediaLoadedEvent;
                MusicPlayer.Player.TickEvent += t_Tick;
                DoubleBuffered = true;
            }
        }

        private void Player_PauseEvent(object sender, EventArgs args)
        {
            SetPlayIcon();
        }

        private void Player_MediaLoadedEvent(object sender, EventArgs args)
        {
            SetTrackBarStatus();
        }

        private void Player_ResumeEvent(object sender, EventArgs args)
        {
            SetPauseIcon();
        }

        private void Player_PlayEvent(object sender, EventArgs args)
        {
            SetPauseIcon();
        }

        void t_Tick(object sender, EventArgs e)
        {
            SetTrackBarStatus();
            if(MusicPlayer.Player.IsPaused && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.ArrowDown)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            }
            if (MusicPlayer.Player.IsPlaying && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.ArrowRight)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.ArrowRight;
            }
            if (MusicPlayer.Player.HasStopped() && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.Rectangle)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.Rectangle;
            }
        }
        
        public void SetTrackBarStatus()
        {
            if (MusicPlayer.Player != null)
            {
                if (MusicPlayer.Player.windowsMediaPlayer != null && MusicPlayer.Player.windowsMediaPlayer.currentMedia != null)
                {
                 //   tool.debug("current media loaded");
                    endLabel2.Text = MusicPlayer.Player.windowsMediaPlayer.currentMedia.durationString;
                  //  scaleFont(endLabel2);
                    gTrackBarMain.MaxValue = (int)MusicPlayer.Player.windowsMediaPlayer.currentMedia.duration;
                    int value = (int)MusicPlayer.Player.windowsMediaPlayer.controls.currentPosition;
                    if (value <= gTrackBarMain.MaxValue)
                        gTrackBarMain.Value = value;
                    startLabel.Text = MusicPlayer.Player.windowsMediaPlayer.controls.currentPositionString;
                   
                    // label1.Location = new Point((int)gTrackBarMain.SliderFocalPt.X, label1.Location.Y);
                }
            }
        }

        private void scaleFont(Label lab)
        {
            Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
            Graphics graphics = Graphics.FromImage(fakeImage);
            
            SizeF extent = graphics.MeasureString(lab.Text, lab.Font);
            
            float hRatio = lab.Height / extent.Height;
            float wRatio = lab.Width / extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = lab.Font.Size * ratio;
            lab.Font = new Font(lab.Font.FontFamily, newSize, lab.Font.Style);
        }

        public void SetTrackPosition()
        {
            MusicPlayer.Player.windowsMediaPlayer.controls.currentPosition = gTrackBarMain.Value;
            startLabel.Text = MusicPlayer.Player.windowsMediaPlayer.controls.currentPositionString;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            SetTrackPosition();
        }
        
        private void axWindowsMediaPlayer1_OpenStateChange(object sender, WMPLib.WMPOpenState e)
        {
            if (MusicPlayer.Player.windowsMediaPlayer.openState == WMPLib.WMPOpenState.wmposMediaOpen)
            {
                startLabel.Text = MusicPlayer.Player.windowsMediaPlayer.controls.currentPositionString;
                gTrackBarMain.MaxValue = (int)MusicPlayer.Player.trackDuration/100;
                startLabel.Text = MusicPlayer.Player.windowsMediaPlayer.currentMedia.durationString;
            }
        }

        private void TrackBarMouseDown(MouseEventArgs e)
        {
            double dblValue;

            dblValue = ((double)e.X / (double)gTrackBarMain.Width) * (gTrackBarMain.MaxValue - gTrackBarMain.MinValue);
            gTrackBarMain.Value = Convert.ToInt32(dblValue);
            gTrackBarMain.SliderColorLow.ColorB = Color.Red;
            gTrackBarMain.SliderColorLow.ColorA = Color.DarkBlue;
            SetTrackPosition();
        }

        private void trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void gTrackBarMain_MouseDown(object sender, MouseEventArgs e)
        {
            TrackBarMouseDown(e);
        }

        private void gTrackBarMain_Scroll(object sender, ScrollEventArgs e)
        {
            SetTrackPosition();
        }

        private void gTrackBarMain_ValueChanged(object sender, EventArgs e)
        {
            //label1.Location = new Point((int)gTrackBarMain.SliderFocalPt.X, label1.Location.Y);
        }

        private void resumeButton_Click(object sender, EventArgs e)
        {
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

        public void Play(string track)
        {
            MusicPlayer.Player.PlayFile(track);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.NextTrack();
            // currentTrack = MusicPlayer.Player.currentTrack;
            if (MusicPlayer.Player.IsPlaying)
                resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();

        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.PrevTrack();
            currentTrack = MusicPlayer.Player.currentTrack;
            resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Player.Stop();
            SetPlayIcon();
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            tool.OpenFileDirectory(MusicPlayer.Player.currentTrackString);
        }

        private void gTrackBarMain_MouseUp(object sender, MouseEventArgs e)
        {
            gTrackBarMain.SliderColorLow.ColorB = Color.DarkOrchid;
            gTrackBarMain.SliderColorLow.ColorA = Color.Black;
        }
    }
}
