using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Music;
using Glaxion.Tools;

namespace Glaxion.Music
{
    public class CustomFont
    {

        public CustomFont(byte[] FontResource)
        {
            LoadFont(FontResource);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont,
            uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        public FontFamily ff;
        public Font font;

        public static CustomFont Exo;
        //static CustomFont Exo2;
        public static void LoadCustomFonts()
        {
            if(Exo == null)
            Exo = new CustomFont(Properties.Resources.Exo2_RegularExpanded);
           // Exo2 = new CustomFont(Properties.Resources.Exo2_RegularExpanded);
        }
        

        public void LoadFont(byte[] FontResource)
        {
            // Use this if you can not find your resource System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            byte[] fontArray = FontResource;
            int dataLength = FontResource.Length; //use font array length instead

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
    }



    public class WinFormUtils
    {




        static public DataObject GetDataObject(string[] files)
        {
            return new DataObject(DataFormats.FileDrop, InternalClipboard.Files.ToArray());
        }

        public static void DoDrop(Control control,string[] files, DragDropEffects DropMode)
        {
            control.DoDragDrop(GetDataObject(files),
                    DropMode);
        }


        static public VNode GetVNode(TreeNode node)
        {
            VNode n = new VNode(node.Text);
            n.name = node.Name;
            n.selected = node.IsSelected;
            n.Expand = node.IsExpanded;
            return n;
        }

        static public VItem GetVItem(SongItem item)
        {
            VItem vi = new VItem();
            vi.Columns.Add(item.song.name);
            vi.Columns.Add(item.song.path);
            vi.Tag = item.Tag;
            vi.name = item.Name;
           // vi.Selected = item.Selected;
            vi.Checked = item.Checked;
            vi.Index = item.Index;
            vi.SetColors(new Tools.ColorScheme(item.ForeColor, item.BackColor));
            return vi;
        }

        public static TreeNode GetTreeNode(VNode node)
        {
            TreeNode n = new TreeNode(node.Text);
            //n.Tag = node.Tag;
            n.Name = node.name;
            n.Text = node.Text;
            n.ForeColor = node.ForeColor;
            n.BackColor = node.BackColor;
            //n.FullPath = node.path;
            if(node.Expand)
            n.Expand();
            foreach(VNode vn in node.Nodes)
                n.Nodes.Add(GetTreeNode(vn));

            return n;
        }

        public static ListViewItem GetItem(string file)
        {
            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = Path.GetFileNameWithoutExtension(file);
            i.SubItems.Add(file);
            i.SubItems[1].Text = file;
            i.Tag = file;
            i.Name = file;
            return i;
        }

        public static ListViewItem GetItem(VItem track)
        {
            ListViewItem i = new ListViewItem();
            i.SubItems[0].Text = Path.GetFileNameWithoutExtension(track.Columns[0]);
            i.SubItems.Add(track.Columns[1]);
            i.Tag = track.Columns[1];
            i.Name = track.Columns[1];
           // i.Selected = track.Selected;
            //i.Checked = track.Checked;
            i.ForeColor = track.CurrentColor.foreColor;
            i.BackColor = track.CurrentColor.backColor;
            return i;
        }
    }
}
