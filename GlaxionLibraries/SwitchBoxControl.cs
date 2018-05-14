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
    public partial class SwitchBoxControl : UserControl
    {
        public SwitchBoxControl()
        {
            InitializeComponent();
        }

        public void SwitchToTextBox()
        {
            label.Hide();
            textBox.Show();
        }

        public void SetText(string text)
        {
            textBox.Text = text;
            label.Text = text;
            SwitchToLabel();
        }

        public void SwitchToLabel()
        {
            textBox.Hide();
            label.Show();
            if (label.Text == "")
            {
                label.Text = "New Scene";
                textBox.Text = label.Text;
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
            SwitchToTextBox();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            label.Text = textBox.Text;
            
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SwitchToLabel();
            }
        }
    }
}
