using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Glaxion.Music
{
    [Designer(typeof(Glaxion.Music.Designers.TestControlDesigner))]
    public partial class EntryBox : UserControl
    {
        public EntryBox()
        {
            InitializeComponent();
            this.textBox1.AutoSize = false;
            textBox1.MouseWheel += TextBox1_MouseWheel;
        }

        //increment the end splitter distance using the mouse wheel
        private void TextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            splitContainer1.SplitterDistance += delta/2;
            if (splitContainer1.SplitterDistance < 500)
                splitContainer1.SplitterDistance = 500;
        }

        public void SetFont(Font f)
        {
            label1.Font = f;
            textBox1.Font = f;
        }
        
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public TextBox MainTextBox { get { return textBox1; } }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // public Label MainLabel { get { return label1; } }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // public SplitContainer SplitContainer1 { get { return splitContainer1; } }

        public bool textChanged;
        public void SetTextbox(string text)
        {
            textBox1.Text = text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
        }

        public string GetEntry()
        {
            string s = textBox1.Text;
            textChanged = false;
            return s;
        }
        
        private void EntryBox_Load(object sender, EventArgs e)
        {
            //hack to ensure the control always becomes visible
            //when dragging and dropping in designer veiw
            this.BringToFront();
            label1.BringToFront();
            //MainLabel.BringToFront();
            //hack to ensure textbox height is matches the control height
            textBox1.Height = this.Height;
        }
    }
}
