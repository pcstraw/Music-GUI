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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Glaxion.Libraries
{
    public partial class ListControl : UserControl
    {
        public List<string> list;
        public TreeNode SelectedNode;
        public bool FolderPickerBrowser;
        public bool MultiSelectBrowser = true;
        public bool OK = false;
        public delegate void OKCloseEventHandler(object sourch, EventArgs args);
        public event OKCloseEventHandler OkClose;
        public object LinkedObject;

        public ListControl()
        {
            InitializeComponent();
            //dg.HeaderCell = "string ";
            //dataGridView1.Columns.Add(new DataGridViewColumn());
        }

        public ListControl(List<string> List,bool folderPicker)
        {
            InitializeComponent();
            list = List;
            PopulateTree();
            FolderPicker = folderPicker;
            OkClose += new OKCloseEventHandler(On_okClose);
        }

        public bool FolderPicker;

        protected void On_okClose(object o, EventArgs e)
        {

        }

        public void PopulateTree()
        {
            treeView.Nodes.Clear();
            foreach(string s in list)
            {
                TreeNode tn = new TreeNode(s);
                treeView.Nodes.Add(tn);
            }
        }

        public void WaitForInput()
        {
  
            while (!OK)
            {

            }
        }

        public void StringFromBrowser()
        {
      
            CommonOpenFileDialog cd = new CommonOpenFileDialog();
            cd.IsFolderPicker = FolderPicker;
            cd.Multiselect = MultiSelectBrowser;
            if (cd.ShowDialog() == CommonFileDialogResult.Ok)
            {   
                foreach(string s in cd.FileNames)
                    list.Add(s);

                PopulateTree();
            }
        }

        public void RemoveTreeNodes()
        {
            if (SelectedNode != null)
            {
                foreach (TreeNode t in treeView.SelectedNodes)
                {
                    if (list.Contains(t.Text))
                    {
                        list.Remove(t.Text);
                    }
                }
                PopulateTree();
            }
        }

        private void textBox1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void ListControl_MouseLeave(object sender, EventArgs e)
        {
          
        }

        private void ListControl_MouseMove(object sender, MouseEventArgs e)
        {
          //  SelectedNode = toolSelectNode(treeView);
        }

        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveTreeNodes();
        }

        private void fromFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StringFromBrowser();
        }

        private void treeView_Click(object sender, EventArgs e)
        {
            SelectedNode = tool.SelectNode(treeView);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            RemoveTreeNodes();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            StringFromBrowser();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if(SelectedNode != null)
            {
                Process.Start(SelectedNode.Text);
            }
        }

        public void DoEvents()
        {
            Application.DoEvents();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            OkClose(this, EventArgs.Empty);
            // On_OkClose();
            // OK = true;


            /*
            List<string> ls = new List<string>();
            foreach(TreeNode t in treeView.Nodes)
            {
                ls.Add(t.Text);
            }
            list = ls;
            OK = true;
            */
        }
    }
}
