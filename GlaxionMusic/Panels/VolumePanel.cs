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
using gTrackBar;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Glaxion.Music
{
    public partial class PlaybackVolumeControl : UserControl
    {
        public PlaybackVolumeControl()
        {
            InitializeComponent();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            MusicPlayer.Instance.SetVolume(gTrackBarMain.Value);
            volumeString = gTrackBarMain.Value.ToString();
        }
        private string volumeString;

        ColorScheme unmutedScheme;
        ColorScheme mutedScheme;

        private void PlaybackVolumeControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode && MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.SetVolume(gTrackBarMain.Value);
                volumeString = gTrackBarMain.Value.ToString();
                gTrackBarMain.Label = volumeString;
                mutedScheme = new ColorScheme(Color.Red,Color.Yellow);
                unmutedScheme = new ColorScheme(gTrackBarMain.SliderColorLow.ColorA,gTrackBarMain.SliderColorLow.ColorB);
            }
        }

        private void trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            double dblValue;
            dblValue = ((double)e.X / (double)gTrackBarMain.Width) * (gTrackBarMain.MaxValue - gTrackBarMain.MinValue);
            gTrackBarMain.Value = Convert.ToInt32(dblValue);
            MusicPlayer.Instance.SetVolume(gTrackBarMain.Value);
        }
        
        private void gTrackBarMain_ValueChanged(object sender, EventArgs e)
        {
            gTrackBarMain.Label = volumeString;
        }

        private void gTrackBarMain_Scroll(object sender, ScrollEventArgs e)
        {
            if (MusicPlayer.Instance != null)
            {
                MusicPlayer.Instance.SetVolume(gTrackBarMain.Value);
                volumeString = gTrackBarMain.Value.ToString();
            }
        }

        private void gTrackBarMain_MouseDown(object sender, MouseEventArgs e)
        {
            double dblValue;
            dblValue = ((double)e.X / (double)gTrackBarMain.Width) * (gTrackBarMain.MaxValue - gTrackBarMain.MinValue);
            gTrackBarMain.Value = Convert.ToInt32(dblValue);
            MusicPlayer.Instance.SetVolume(gTrackBarMain.Value);
        }

        void Mute()
        {
            bool  muted = MusicPlayer.Instance.Mute();
            if(muted)
            {
                unmutedScheme.backColor = gTrackBarMain.SliderColorLow.ColorA;
                unmutedScheme.foreColor = gTrackBarMain.SliderColorLow.ColorB;
                gTrackBarMain.SliderColorLow.ColorA = mutedScheme.backColor;
                gTrackBarMain.SliderColorLow.ColorB = mutedScheme.foreColor;
            }
            else
            {
                gTrackBarMain.SliderColorLow.ColorA = unmutedScheme.backColor;
                gTrackBarMain.SliderColorLow.ColorB = unmutedScheme.foreColor;
            }
        }

        private void gTrackBarMain_MouseClick(object sender, MouseEventArgs e)
        {
            //detect if the user clicks on the right mnost side of the trackbar
            int x_pos = gTrackBarMain.Width - e.X;
            if (x_pos < 50 && x_pos > 25)
                Mute();
        }
    }
}
