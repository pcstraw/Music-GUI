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
using System.IO;
using System.Diagnostics;
using System.Windows.Forms.Design;


namespace Glaxion.Music
{
    [Designer(typeof(Glaxion.Music.Designers.TestControlDesigner))]
    public partial class SquashBoxControl : UserControl
    {
        // [Designer(typeof(Pare))]
        
        public SquashBoxControl()
        {
            InitializeComponent();
            OnSwapEvent += SquashBoxControl_OnSwapEvent;
        }

        protected virtual void SquashBoxControl_OnSwapEvent(object sender, EventArgs args)
        {
        }

        // Note: property added
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitContainer MainSplitContainer { get { return splitContainer; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel TopPanel { get { return topPanel; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel FontPanel { get { return topPanel; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackPanel { get { return backPanel; } }

        Control frontControl;
        Control backControl;

        public delegate void OnSwapEventHandler(object sender, EventArgs args);
        public event OnSwapEventHandler OnSwapEvent;

        public void Swap()
        {
            Control c1 = frontControl;
            Control c2 = backControl;
            MakeFrontPanel(c2);
            MakeBackPanel(c1);
            OnSwapEvent(this, EventArgs.Empty);
        }

        public void MakeFrontPanel(Control control)
        {
            if (control == null)
                return;
            frontPanel.Controls.Add(control);
            control.BringToFront();
            frontControl = control;
        }
        public void MakeBackPanel(Control control)
        {
            if (control == null)
                return;
            backPanel.Controls.Add(control);
            control.BringToFront();
            backControl = control;
        }


        private void picturePanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        bool bringForwardBackPanel;
        private void SwapPanel(Panel p1,Panel p2)
        {
        }

        private void title_label_Click(object sender, EventArgs e)
        {
            if (bringForwardBackPanel)
            {
               // SwapPanel(picturePanel, backPanel);
            }
            else
            {
              //  SwapPanel(backPanel, picturePanel);
            }
            bringForwardBackPanel = !bringForwardBackPanel;
        }

        private void SquashBoxControl_Load(object sender, EventArgs e)
        {
            //start the front panel fully visible
            //splitContainer.SplitterDistance = 0;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Swap();
        }
    }
}
