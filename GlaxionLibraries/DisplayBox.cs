using Glaxion.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glaxion.Libraries
{
    public partial class DisplayBox : Form
    {

       void Construction()
        {
            InitializeComponent();
            lifeTime = 50;
        }

        public DisplayBox()
        {
            Construction();
        }

        public DisplayBox(string text)
        {
            Construction();
            textBox.Text = text;
        }

        public static Timer timer;
        public int lifeTime;
        int tick;

        public static void Open(string text)
        {
            DisplayBox vb = new DisplayBox(text);
            vb.Display();
        }

        public static void Open()
        {
            DisplayBox vb = new DisplayBox("View box");
            vb.Display();
        }

        public void Display()
        {
            StartTimer();
            this.Show();
            this.Owner = tool.GlobalForm;
            this.TopLevel = true;
        }

        public void StartTimer()
        {
            
            timer = null;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += new EventHandler(t_Tick);
            timer.Start();
        }

        private void t_Tick(object sender, EventArgs e)
        {
          
            tick++;
            if(tick > lifeTime)
            {
                this.Close();
                return;
            }
            this.Text = (lifeTime - tick).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lifeTime += 50;
        }

        private void ViewBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Dispose();
        }
    }
}
