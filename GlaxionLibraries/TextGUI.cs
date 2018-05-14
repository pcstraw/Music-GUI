using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using UserControl = System.Windows.Forms.UserControl;

namespace Glaxion.Libraries
{
    public partial class TextGUI : UserControl
    {
        public TextGUI()
        {
            InitializeComponent();
            SpellCheck();
            
        }

        public void SpellCheck()
        {
            System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
            textBox.SpellCheck.IsEnabled = true;
            elementHost1.Child = textBox;
            
            richTextBox1.Hide();
            this.Controls.Add(elementHost1);
            elementHost1.Dock = DockStyle.Fill;
            //textBox.
           // this.Controls.Add((System.Windows.Forms.Control)textBox);
        }

       
    }
}
