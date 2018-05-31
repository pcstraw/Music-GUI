using System;
using System.Drawing;
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
        public string imagePath;
        public bool pictureChanged;
        Song trackInfo;

        public delegate void PictureChangedEventHandler(object sender, EventArgs args);
        public event PictureChangedEventHandler PictureChangedEvent;
        protected void OnPictureChanged(object sender,EventArgs e)
        {

        }
        
        public void SaveInfo()
        {
            if (trackInfo == null)
                return;

            if (pictureChanged)
            {
                pictureChanged = false;
                trackInfo.SetPictureFromImage(this.BackgroundImage);
            }
            trackInfo.SaveInfo();
        }

        private void PicturePanel_DoubleClick(object sender, EventArgs e)
        {
            string dp = Path.GetDirectoryName(imagePath);
            if (Directory.Exists(dp))
                tool.OpenFileDirectory(dp);
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
            
            string[] data = WinFormUtils.GetExternalDragDrop(e);
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
           // InternalClipboard.Clear();
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
            string newFullPath = string.Concat(dir, name, ext);
            if (File.Exists(newFullPath))
            {
                string new_name = string.Format("{0}({1})", name, 1);
                if (name.ToLower() == "download")
                {
                    newFullPath = string.Concat(dir, new_name, ext);
                }
                else{ 
                    SaveFileDialog od = new SaveFileDialog();
                    od.Title = "File Already Exists. Make a New File Name";
                    od.FileName = new_name;
                    od.AddExtension = true;
                    od.DefaultExt = ext;
                    od.InitialDirectory = dir;
                    if (od.ShowDialog() == DialogResult.OK)
                    {
                        newFullPath = od.FileName;
                    }
                    else
                        return;
                }
            }
            File.Copy(file, newFullPath, true);
            //used for testing
            //tool.OpenFileDirectory(newFullPath);
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
