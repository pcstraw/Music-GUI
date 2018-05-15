using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Glaxion.Music.Designers
{
    public class TestControlDesigner : ParentControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);
            
            foreach (Control c in this.Control.Controls)
            {
                this.EnableDesignMode(c, c.GetType().Name);
            }
                /*
                this.EnableDesignMode((
                   (SquashBoxControl)this.Control).splitContainer, "SquashContainer");
                this.EnableDesignMode((
                   (SquashBoxControl)this.Control).TopPanel, "TopPanel");
                */
                // this.EnableDesignMode((
                //  (SquashBoxControl)this.Control).FrontText, "FontText");
            
        }
    }
}