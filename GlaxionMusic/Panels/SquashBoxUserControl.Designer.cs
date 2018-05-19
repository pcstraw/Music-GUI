namespace Glaxion.Music
{
    partial class SquashBoxControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.backPanel = new System.Windows.Forms.Panel();
            this.frontPanel = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 38);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer.Panel1.Controls.Add(this.backPanel);
            this.splitContainer.Panel1MinSize = 0;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.Gray;
            this.splitContainer.Panel2.Controls.Add(this.frontPanel);
            this.splitContainer.Panel2MinSize = 0;
            this.splitContainer.Size = new System.Drawing.Size(326, 333);
            this.splitContainer.SplitterDistance = 109;
            this.splitContainer.TabIndex = 2;
            // 
            // backPanel
            // 
            this.backPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.backPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backPanel.Location = new System.Drawing.Point(0, 0);
            this.backPanel.Name = "backPanel";
            this.backPanel.Size = new System.Drawing.Size(326, 109);
            this.backPanel.TabIndex = 0;
            // 
            // frontPanel
            // 
            this.frontPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.frontPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frontPanel.Location = new System.Drawing.Point(0, 0);
            this.frontPanel.Name = "frontPanel";
            this.frontPanel.Size = new System.Drawing.Size(326, 220);
            this.frontPanel.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.Silver;
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(326, 38);
            this.topPanel.TabIndex = 0;
            this.topPanel.Click += new System.EventHandler(this.panel1_Click);
            // 
            // SquashBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.topPanel);
            this.Name = "SquashBoxControl";
            this.Size = new System.Drawing.Size(326, 371);
            this.Load += new System.EventHandler(this.SquashBoxControl_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Panel backPanel;
        public System.Windows.Forms.Panel frontPanel;
        public System.Windows.Forms.SplitContainer splitContainer;
        public System.Windows.Forms.Panel topPanel;
    }
}
