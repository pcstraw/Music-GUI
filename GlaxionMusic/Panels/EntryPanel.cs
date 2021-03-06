﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Glaxion.Music
{
    public delegate void TextChangedDelegate();
    [Designer(typeof(Glaxion.Music.Designers.TestControlDesigner))]
    public partial class EntryBox : UserControl
    {
        public EntryBox()
        {
            InitializeComponent();
            textBox.AutoSize = false;
            textBox.MouseWheel += TextBox1_MouseWheel;
        }
        public bool _textChanged;
        
        public TextChangedDelegate DelegateTextChanged;
        //increment the end splitter distance using the mouse wheel
        private void TextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            
            int delta = splitContainer1.SplitterDistance + (e.Delta / 2);
            if (delta < 500)
                return;
            splitContainer1.SplitterDistance = delta;

        }

        public void SetFont(Font f)
        {
            MainLabel.Font = f;
            textBox.Font = f;
        }
        
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public TextBox MainTextBox { get { return textBox1; } }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // public Label MainLabel { get { return label1; } }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // public SplitContainer SplitContainer1 { get { return splitContainer1; } }

        
        public void SetTextbox(string text)
        {
            textBox.Text = text;
        }

        public void TextHasChanged()
        {
            _textChanged = true;
            if (textBox.Focused && DelegateTextChanged != null)
            {
                DelegateTextChanged.Invoke();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextHasChanged();
        }

        public string GetEntry()
        {
            string s = textBox.Text;
            _textChanged = false;
            return s;
        }
        
        private void EntryBox_Load(object sender, EventArgs e)
        {
            //hack to ensure the control always becomes visible
            //when dragging and dropping in designer veiw
            this.BringToFront();
            MainLabel.BringToFront();
            //hack to ensure textbox height is matches the control height
            textBox.Height = this.Height;
        }
    }
}
