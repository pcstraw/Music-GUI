using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glaxion.Libraries
{
    public partial class PanelSwitch : UserControl
    {
        public PanelSwitch()
        {
            InitializeComponent();
        }

        public void Switch()
        {
            textBox.Visible = !textBox.Visible;
           // label.Visible = !label.Visible;
        }

        public void SelectLabel()
        {
            //label.Text = textBox.Text;
            textBox.Hide();
           // label.Visible = true;
           // label.Show();
        }

        public void SelectTextBox()
        {
            textBox.Show();
            //label.Hide();
        }

        public void SetLabel(string text)
        {
           // label.Text = text;
            textBox.Text = text;
            SelectLabel();
        }

        private void label_DoubleClick(object sender, EventArgs e)
        {
            SelectTextBox();
        }
        

        private void label_Click(object sender, EventArgs e)
        {
            SelectTextBox();
        }
    }
}
