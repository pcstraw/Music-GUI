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
            Song s = MusicPlayer.Player.fileLoader.trackInfoManager.GetInfo(v);
            if (s == null)
            return;
            iD3Control1.SetSong(s);
            picturePanel1.SetSong(s);
            titleLabel.Text = s.title;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, 
            uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        FontFamily ff;
        Font font;
        
        public void LoadFont()
        {
            // Use this if you can not find your resource System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            byte[] fontArray = Properties.Resources.Exo2_RegularExpanded;
            int dataLength = Properties.Resources.Exo2_RegularExpanded.Length; //use font array length instead

            IntPtr ptr = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptr, dataLength);

            uint cFont = 0;

            AddFontMemResourceEx(ptr, (uint)fontArray.Length, IntPtr.Zero, ref cFont);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptr, dataLength);

            Marshal.FreeCoTaskMem(ptr);

            ff = pfc.Families[0];
            font = new Font(ff, 20f, FontStyle.Regular);

        }

        private void SongControl_Load(object sender, EventArgs e)
        {
            iD3Control1.BringToFront();
            picturePanel1.BringToFront();
            LoadFont();
            titleLabel.Font = font;
        }

        private void picturePanel1_PictureChangedEvent(object sender, EventArgs args)
        {
            if (iD3Control1.song == null)
                return;
            iD3Control1.song.SetPictureFromImage(picturePanel1.BackgroundImage);
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
