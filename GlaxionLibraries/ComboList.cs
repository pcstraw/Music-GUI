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

namespace Glaxion.Libraries
{
    public partial class ComboList : UserControl
    {
        public ComboList()
        {
            InitializeComponent();
        }

        //public ProjectGUI sceneGUI;

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            label.Focus();
        }

        private void ComboList_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("combo list");
        }

        private void operationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox.SelectedItem.ToString());
        }

        private void comboBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Process.Start(comboBox.SelectedItem as string);
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void ComboList_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void comboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                /*
                if (sceneGUI != null)
                {
                    sceneGUI.currentScene.RemoveEntry(comboBox.SelectedItem.ToString());

                    comboBox.Items.Remove(comboBox.SelectedItem);
                    comboBox.SelectedItem = 0;
                    sceneGUI.UpdateGUI();
                }
                */

            }
        }
    }
}
