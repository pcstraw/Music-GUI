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
            MusicPlayer.Instance.Pause();
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

        public void Play()
        {
            MusicPlayer.Instance.PlayFile(MusicPlayer.Instance.currentTrackString);
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
            if (!DesignMode && MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.PlayEvent += Player_PlayEvent;
                MusicPlayer.Instance.ResumeEvent += Player_ResumeEvent;
                MusicPlayer.Instance.PauseEvent += Player_PauseEvent;
                MusicPlayer.Instance.MediaLoadedEvent += Player_MediaLoadedEvent;
                MusicPlayer.Instance.TickEvent += t_Tick;
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
            if(MusicPlayer.Instance.IsPaused && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.ArrowDown)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            }
            if (MusicPlayer.Instance.IsPlaying && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.ArrowRight)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.ArrowRight;
            }
            if (MusicPlayer.Instance.HasStopped() && gTrackBarMain.SliderShape != gTrackBar.gTrackBar.eShape.Rectangle)
            {
                gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.Rectangle;
            }
        }
        
        public void SetTrackBarStatus()
        {
            if (MusicPlayer.Instance != null)
            {
                if (MusicPlayer.Instance.windowsMediaPlayer != null && MusicPlayer.Instance.windowsMediaPlayer.currentMedia != null)
                {
                 //   tool.debug("current media loaded");
                    endLabel2.Text = MusicPlayer.Instance.windowsMediaPlayer.currentMedia.durationString;
                  //  scaleFont(endLabel2);
                    gTrackBarMain.MaxValue = (int)MusicPlayer.Instance.windowsMediaPlayer.currentMedia.duration;
                    int value = (int)MusicPlayer.Instance.windowsMediaPlayer.controls.currentPosition;
                    if (value <= gTrackBarMain.MaxValue)
                        gTrackBarMain.Value = value;
                    startLabel.Text = MusicPlayer.Instance.windowsMediaPlayer.controls.currentPositionString;
                   
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
            MusicPlayer.Instance.windowsMediaPlayer.controls.currentPosition = gTrackBarMain.Value;
            startLabel.Text = MusicPlayer.Instance.windowsMediaPlayer.controls.currentPositionString;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            SetTrackPosition();
        }
        
        private void axWindowsMediaPlayer1_OpenStateChange(object sender, WMPLib.WMPOpenState e)
        {
            if (MusicPlayer.Instance.windowsMediaPlayer.openState == WMPLib.WMPOpenState.wmposMediaOpen)
            {
                startLabel.Text = MusicPlayer.Instance.windowsMediaPlayer.controls.currentPositionString;
                gTrackBarMain.MaxValue = (int)MusicPlayer.Instance.trackDuration/100;
                startLabel.Text = MusicPlayer.Instance.windowsMediaPlayer.currentMedia.durationString;
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

        public void Play(string track)
        {
            MusicPlayer.Instance.PlayFile(track);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Instance.NextTrack();
            // currentTrack = MusicPlayer.Instance.currentTrack;
            if (MusicPlayer.Instance.IsPlaying)
                resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();

        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Instance.PrevTrack();
            currentTrack = MusicPlayer.Instance.currentTrack;
            resumeButton.BackgroundImage = Properties.Resources.Icons8_Windows_8_Media_Controls_Pause.ToBitmap();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            MusicPlayer.Instance.Stop();
            SetPlayIcon();
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            tool.OpenFileDirectory(MusicPlayer.Instance.currentTrackString);
        }

        private void gTrackBarMain_MouseUp(object sender, MouseEventArgs e)
        {
            gTrackBarMain.SliderColorLow.ColorB = Color.DarkOrchid;
            gTrackBarMain.SliderColorLow.ColorA = Color.Black;
        }
    }
}
