using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glaxion.Tools;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Glaxion.Libraries
{
    public class ImageManager
    {
        public ImageManager(Control c)
        {
            control = c;
        }
        public List<string> defaultimages = new List<string>();
        public List<string> imagefolders = new List<string>();
        public List<string> imagefiles = new List<string>();
        public Dictionary<string, List<string>> imageFolders = new Dictionary<string, List<string>>();
        public bool enableImages;
        public Control control;
        public int imageIndex;
        public string currentImage;
        public ColorScheme oldColor;

        public void Load()
        {
            /*
            defaultimages = GetDefaultImages(list);
            AddImageList("default", defaultimages);

            if (defaultimages.Count > 0)
            {
                imagefiles = defaultimages;
                if (enableImages)
                {
                    SetImage(defaultimages[defaultimages.Count - 1]);

                }
            }

          //  imagefolders = GetSavedFolders();
            foreach (string folder in imagefolders)
            {
                LoadintoImageReferences(folder);
            }
            LoadImageReferencesToContext();
            */
        }

        public ListGUI EditDefaultList(Form owner)
        {
            ListGUI lg = new ListGUI(imageFolders["default"], false);
            lg.Owner = owner;
            return lg;
        }

        public ListGUI EditList(List<string> list, Form owner)
        {
            ListGUI lg = new ListGUI(list, false);
            lg.Owner = owner;
            return lg;
        }

        public ListGUI EditFolderDirectories(Form owner)
        {
            List<string> list = imageFolders.Keys.ToList();
            ListGUI lg = new ListGUI(list, true);
            lg.Owner = owner;
            imageFolders.Clear();
            foreach (string s in list)
                LoadImagesFromDirectory(s);

            return lg;
        }

        public void SetImage(string path)
        {
            if (!tool.StringCheck(path))
                return;
            if (!File.Exists(path))
            {
                // tool.Show("FAILED TO LOAD IMAGE: " + path);
                return;
            }
          //  Properties.Settings.Default.currentImage = path;
            control.BackgroundImage = Image.FromFile(path);
            control.BackgroundImageLayout = ImageLayout.Zoom;
            if (oldColor == null)
                oldColor = new ColorScheme(control.BackColor, control.ForeColor, this);
            control.BackColor = Color.Black;
            control.ForeColor = Color.White;
        }

        public void LoadImagesFromDirectory(string path)
        {
            AddImageList(path, tool.LoadImageFiles(path));
        }

        public void AddImagePathToContext(string path,ToolStripMenuItem contextItem)
        {
            //   folderContextMenu.DropDownItems.Add(path);
            ToolStripMenuItem t = new ToolStripMenuItem(path);
            if (!Directory.Exists(path))
                t.ForeColor = Color.Red;
            contextItem.DropDownItems.Add(t);
        }

        public List<string> OpenImageFolderBrowser()
        {
            //List<string> fl = tool.OpenFiles();
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.IsFolderPicker = true;
            cd.Multiselect = true;
            cd.RestoreDirectory = true;
            List<string> l = new List<string>();
            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (cd.Multiselect)
                {
                    foreach (string s in cd.FileNames)
                        l.Add(s);
                }
                else
                {

                    l.Add(cd.FileName);
                }
            }
            foreach (string s in l)
            {
                if (Path.HasExtension(s))
                {
                    l.Remove(s);
                }
            }
            return l;
        }

        public void AddImageList(string path, List<string> images)
        {
            // KeyValuePair<string,List<string>> kv = new KeyValuePair<string, List<string>(path, images);
            if (!imageFolders.ContainsKey(path))
                imageFolders.Add(path, images);
            if (images.Count > 0)
            {

            }
            else
            {
                tool.debug("no images in directory: ", path);
            }
        }

        public List<string> GetDefaultImages(List<string> list)
        {
            List<string> sl = new List<string>();
            foreach (string s in list)
            {
                sl.Add(s);
            }
            return sl;
        }

        public void Nextimage(int direction)
        {
            if (!enableImages)
                return;

            if (imagefiles.Count == 0)
                return;

            if (imageIndex < 0)
                imageIndex = imagefiles.Count - 1;

           
            if (imageIndex >= imagefiles.Count)
                imageIndex = 0;

            currentImage = imagefiles[imageIndex];
            SetImage(currentImage);
            control.Invalidate();
            imageIndex += direction;
        }

        public List<string> SelectedImageFiles()
        {
            //  List<string> fl = tool.OpenFiles();
            List<string> l = new List<string>();
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.IsFolderPicker = false;
            cd.Multiselect = true;
            cd.RestoreDirectory = true;
            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
                if (cd.Multiselect)
                    foreach (string s in cd.FileNames)
                        if (tool.IsImageFile(s))
                            l.Add(s);
                        else
                   if (tool.IsImageFile(s))
                            l.Add(cd.FileName);
            return l;
        }

    }
}
