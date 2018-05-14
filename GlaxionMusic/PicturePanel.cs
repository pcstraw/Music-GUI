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
using System.Diagnostics;
using System.IO;

namespace Glaxion.Music
{
    public partial class PicturePanel : UserControl
    {
        public PicturePanel()
        {
            InitializeComponent();
            imagePath = "";
            PictureChangedEvent += OnPictureChanged;
        }
        public delegate void PictureChangedEventHandler(object sender, EventArgs args);
        public event PictureChangedEventHandler PictureChangedEvent;
        protected void OnPictureChanged(object sender,EventArgs e)
        {

        }

        public string imagePath;
        public bool pictureChanged;

        public void SaveInfo()
        {
            if (trackInfo == null)
                return;
            /*
            if (trackInfo.path == MusicPlayer.Player.currentTrackString)
            {
                tool.show(5, "Cannot save info while the tarck is playing");
                return;
            }
            */
            //trackInfo.info.ReadID3v1();
            if (pictureChanged)
            {
                pictureChanged = false;
                trackInfo.SetPictureFromImage(this.BackgroundImage);
            }
            trackInfo.SaveInfo();
        }

        /*
        public void ShowInfo(string file)
        {
            
        }
        */

        private void PicturePanel_DoubleClick(object sender, EventArgs e)
        {
            string dp = Path.GetDirectoryName(imagePath);
            if (Directory.Exists(dp))
            {
                tool.OpenFileDirectory(dp);
            }
        }

        public void SetPicture(Image i)
        {
            this.BackgroundImage = i;
            PictureChangedEvent(this, EventArgs.Empty);
        }

        public Song SetSong(Song s)
        {
            trackInfo = s;
            SetDefaultImageAsync();
            return s;
        }

        Song trackInfo;
        private async void SetDefaultImageAsync()
        {
            await trackInfo.LoadAlbumArtAsync();
            Image i = trackInfo.image;
            if (i == null)
                i = MusicPlayer.MusicGUILogo;
            this.BackgroundImage = i;
        }

        private void PicturePanel_DragDrop(object sender, DragEventArgs e)
        {
            //get the drop data
            string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (data == null)
                return;
            if (data.Length > 0)
            {
                foreach (string s in data)
                {
                    if (tool.IsImageFile(s))
                    {
                        if (File.Exists(s))
                        {
                            Image img;
                            using (var bmptmp = new Bitmap(s))
                            {
                                img = new Bitmap(bmptmp);
                                if (img != null)
                                {
                                    SetPicture(img);
                                    CopyToDirectory(s);
                                    pictureChanged = true; //when saving, set the current image in the ID3 tag
                                    //Invalidate();
                                }
                            }
                        }
                    }
                }
            }
            TotalClipboard.Clear();
        }
        public static string GetAlbumArtDirectory()
        {
            string dir = @"Output\Album Art\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static void CopyToDirectory(string file)
        {
            string dir = GetAlbumArtDirectory();
            string ext = Path.GetExtension(file);
            string name = Path.GetFileNameWithoutExtension(file);
            //int count = 1;
            string newFullPath = file;
            if (File.Exists(newFullPath))
            {
                SaveFileDialog od = new SaveFileDialog();
                od.InitialDirectory = dir;
                od.Title = "File Already Exists. Make a New File Name";
                //od.Multiselect = false;
                string tmp = string.Format("{0}({1})", name, 1);
                od.FileName = tmp+ext;

                if (od.ShowDialog() == DialogResult.OK)
                {
                    newFullPath = od.FileName;
                }
                else
                    return;
            }

            File.Copy(file, newFullPath,true);
        }

        public static void OpenAlbumArtFolder()
        {
            string dir = GetAlbumArtDirectory();
            Process.Start(dir);
        }

        private void PicturePanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}
