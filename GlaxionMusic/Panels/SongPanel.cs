using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public partial class SongControl : UserControl
    {
        public SongControl()
        {
            InitializeComponent();
            squashBoxControl1.MakeFrontPanel(picturePanel1);
            squashBoxControl1.MakeBackPanel(iD3Control1);
            titleLabel.DoubleBuffering(true);
        }

        public static SongControl CreateTagEditor(string v,Form OwnerForm)
        {
            Form f = new Form();
            f.Text = v;
            f.Owner = OwnerForm;
            f.Icon = OwnerForm.Icon;
            SongControl sc = new SongControl();
            f.Size = sc.Size;
            f.Location = new Point(f.Location.X, 0);
            sc.SetSong(v);
            f.Controls.Add(sc);
            sc.Dock = DockStyle.Fill;
            f.Show();
            return sc;
        }

        public void SetSong(string v)
        {
            Song s = MusicInfo.Instance.GetInfo(v);
            if (s == null)
            return;
            iD3Control1.SetSong(s);
            picturePanel1.SetSong(s);
            titleLabel.Text = s.title;
        }

        
        private void SongControl_Load(object sender, EventArgs e)
        {
            iD3Control1.BringToFront();
            picturePanel1.BringToFront();
            titleLabel.Font = new Font(CustomFont.Exo.ff, titleLabel.Font.Size);
        }

        private void picturePanel1_PictureChangedEvent(object sender, EventArgs args)
        {
            if (iD3Control1.song == null)
                return;
            iD3Control1.song.SetPictureFromImage(picturePanel1.BackgroundImage);
            iD3Control1.OnDelegateTextChanged();
        }

        private void titleLabel_Click(object sender, EventArgs e)
        {
            squashBoxControl1.Swap();
        }

        private void picturePanel1_Click(object sender, EventArgs e)
        {
            squashBoxControl1.Swap();
        }
    }
}
