using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using Glaxion.Tools;

namespace Glaxion.Libraries
{
    public partial class ListGUI : Form
    {
        public ListGUI()
        {
            InitializeComponent();
        }

        public ListGUI(List<string> list, bool folderPicker)
        {
            InitializeComponent();
            List = list;
            listControl = new ListControl(list,folderPicker);
            listControl.Dock = DockStyle.Fill;
            this.Controls.Add(listControl);
            listControl.Show();
            this.Show();
            this.Owner = tool.GlobalForm;
        }

        public ListControl listControl;
        public List<string> List;
        public StringCollection stringCollection;

        private void ListGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stringCollection == null)
                return;
            List.Clear();
            foreach(string s in listControl.list)
            {
                List.Add(s);
                //stringCollection.Add(s);
            }
        }

        protected void OkClose(object o, EventArgs e)
        {
            this.Close();
        }

        private void ListGUI_Load(object sender, EventArgs e)
        {
            listControl.OkClose += new ListControl.OKCloseEventHandler(OkClose);
        }
    }
}
