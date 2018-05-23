using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glaxion.Music;

namespace Glaxion.Music
{
    public class WinFormUtils
    {
        static public VNode GetVNode(TreeNode node)
        {
            VNode n = new VNode(node.Text);
            n.Tag = node.Tag;
            n.name = node.Name;
            n.selected = node.IsSelected;
            n.Expand = node.IsExpanded;
            return n;
        }

        static public VItem GetVItem(ListViewItem item)
        {
            VItem vi = new VItem();
            vi.Columns.Add(item.SubItems[0].Text);
            vi.Columns.Add(item.Name);
            vi.Tag = item.Tag;
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
            n.Name = node.path;
            n.Text = node.Text;
            n.ForeColor = node.ForeColor;
            n.BackColor = node.BackColor;
            //n.FullPath = node.path;
            if(node.Expand)
            n.Expand();
            foreach(VNode vn in node.Nodes)
            {
                n.Nodes.Add(GetTreeNode(vn));
            }
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
