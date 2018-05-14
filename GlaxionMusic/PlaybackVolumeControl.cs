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
            MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
            volumeString = gTrackBarMain.Value.ToString();
        }
        private string volumeString;

        ColorScheme unmutedScheme;
        ColorScheme mutedScheme;

        private void PlaybackVolumeControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode && MusicPlayer.Player != null)
            {
                MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
                //muteCheckBox.Checked = MusicPlayer.Player.Muted;
                volumeString = gTrackBarMain.Value.ToString();
                gTrackBarMain.Label = volumeString;
                mutedScheme = new ColorScheme();
                mutedScheme.foreColor = Color.Red;
                mutedScheme.backColor = Color.Yellow;
                unmutedScheme = new ColorScheme();
                unmutedScheme.backColor = gTrackBarMain.SliderColorLow.ColorA;
                unmutedScheme.foreColor = gTrackBarMain.SliderColorLow.ColorB;
            }
        }

        private void trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            double dblValue;
            dblValue = ((double)e.X / (double)gTrackBarMain.Width) * (gTrackBarMain.MaxValue - gTrackBarMain.MinValue);
            gTrackBarMain.Value = Convert.ToInt32(dblValue);
            MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
        }


        //deprecate
        private void muteCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //muteCheckBox.Checked = MusicPlayer.Player.Mute();
            if (!MusicPlayer.Player.Muted)
            {
                MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
            }
        }

        private void gTrackBarMain_ValueChanged(object sender, EventArgs e)
        {
            gTrackBarMain.Label = volumeString;
        }

        private void gTrackBarMain_Scroll(object sender, ScrollEventArgs e)
        {
            if (MusicPlayer.Player != null)
            {
                MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
                volumeString = gTrackBarMain.Value.ToString();
            }
        }

        private void gTrackBarMain_MouseDown(object sender, MouseEventArgs e)
        {
            double dblValue;
            dblValue = ((double)e.X / (double)gTrackBarMain.Width) * (gTrackBarMain.MaxValue - gTrackBarMain.MinValue);
            gTrackBarMain.Value = Convert.ToInt32(dblValue);
            MusicPlayer.Player.SetVolume(gTrackBarMain.Value);
        }

        void Mute()
        {
            bool  muted = MusicPlayer.Player.Mute();
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
            int x_pos = gTrackBarMain.Width - e.X;
            
            if (x_pos < 50 && x_pos > 25)
                Mute();
        }
    }
}
