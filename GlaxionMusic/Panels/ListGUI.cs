﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Specialized;
using Glaxion.Tools;

namespace Glaxion.Music
{
    public delegate void ListGUICallBack(bool ok);

    public partial class ListGUI : Form
    {
        public ListGUI()
        {
            InitializeComponent();
        }
        public ListGUICallBack callback;
        public ListGUI(List<string> list, bool folderPicker)
        {
            InitializeComponent();
            List = list;
            listControl = new ListControl(list,folderPicker);
            listControl.Dock = DockStyle.Fill;
            this.Controls.Add(listControl);
            listControl.Show();
            this.Show();
            if(tool.GlobalForm != null)
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

        public static void EditStringList(List<string> list)
        {
            ListGUI lg = new ListGUI(list, true);
            lg.Show();
        }

        protected void OkClose(object o, EventArgs e)
        {
            if (callback != null)
                callback.Invoke(true);
            this.Close();
            
        }

        private void ListGUI_Load(object sender, EventArgs e)
        {
            listControl.OkClose += new ListControl.OKCloseEventHandler(OkClose);
        }
    }
}
